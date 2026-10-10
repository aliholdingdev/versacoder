---
title: "async-loading-patterns - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Form acilisinda ve sorgu degisiminde verinin nasil yuklenecegini, yuklenirken arayuzun nasil mesgul gosterilecegini ve veri art arda geldiginde kazananin nasil belirlendigini tanimlar."
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Asenkron Yukleme, Busy Durumu ve Asamali Doldurma

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]] · [[.rules/ui-rules|ui-rules]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 4/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]]

> **Tek cumlelik sorumluluk:** *Veri yuklenirken arayuz ne gosterir, ayni anda iki yukleme olursa kazanan kimdir ve kontrol atamasi hangi thread'de yapilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Yukleme **her zaman `async`** ve **her zaman kullaniciya gorunur bir busy durumuyla** (progress, `UseWaitCursor`, devre disi buton) yapilir. Ayni anda birden cok yukleme tetikleniyorsa **kazanan en son baslayan yuklemedir**: oncekiler `CancellationTokenSource` ile iptal edilir. Veri arka planda getirilir, **kontrol atamasi UI thread'de** yapilir.

```text
+-------------------------------------------------------------------+
| yukleme      : async + gorunur busy (finally'da kapanir)          |
| es zamanli 2 : oncekini iptal et -> EN SON BASLAYAN KAZANIR       |
| atama        : tek seferde, UI thread -> cross-thread hatasi yok   |
+-------------------------------------------------------------------+
```

---

## §1 Senaryo x kalip x tuzak

| # | Senaryo | Kalip | Tuzak |
|---|---|---|---|
| 1 | Form ilk acilis | `Load` -> `await InitializeAsync()` | `Load`'ta senkron I/O |
| 2 | Kutu degisimi | debounce + iptal + yeniden yukleme | her tusta yeni task |
| 3 | Butonla calistir | butonu devre disi birak -> `await` -> geri ac | cift tiklama = 2 is |
| 4 | Uzun rapor | `Task.Run` + ilerleme raporu | UI thread'de hesap |
| 5 | Grid doldurma | `BindingSource` + `ResetBindings` | her satirda tek tek `Add` |

---

## §2 Busy durumu kalibi

```csharp
private async Task LoadAsync(CancellationToken token)
{
    _busy.Visible = true;
    btnRefresh.Enabled = false;
    UseWaitCursor = true;
    try
    {
        var rows = await _repo.LoadAsync(token);     // arka plan
        grid.BeginUpdate();                          // tek toplu guncelleme
        try { grid.DataSource = rows; }
        finally { grid.EndUpdate(); }
        status.Caption = rows.Count + " kayit";
    }
    catch (OperationCanceledException)
    {
        status.Caption = "Yukleme iptal edildi.";
    }
    catch (Exception ex)
    {
        _errors.Report(ex);
    }
    finally
    {
        _busy.Visible = false;
        btnRefresh.Enabled = true;
        UseWaitCursor = false;
    }
}
```

### §2.1 Busy yuzeyi kontrol listesi

| # | Yuzey | Ne gosterilir | Kapatilma |
|---|---|---|---|
| 1 | `UseWaitCursor` | genel imlec | `finally` |
| 2 | `ProgressBar` / `Marquee` | sure bilinmiyorsa marquee | `finally` |
| 3 | Devre disi butonlar | cift tiklama engeli | `finally` |
| 4 | Durum cubugu metni | "Yukleniyor..." -> "N kayit" | `finally` |
| 5 | Grid `BeginUpdate` / `EndUpdate` | satir basina yeniden cizim yok | `finally` |

---

## §3 En son kazanir (last-write-wins)

```csharp
private CancellationTokenSource _searchCts;

private async void txtSearch_TextChanged(object sender, EventArgs e)
{
    _searchCts?.Cancel();
    _searchCts?.Dispose();
    _searchCts = new CancellationTokenSource();
    await LoadAsync(_searchCts.Token);
}
```

| # | Kural | Neden |
|---|---|---|
| 1 | Yeni arama baslarken eskisini **iptal et** | bayat veri ekrani kirletir |
| 2 | `DataSource` atamasi **tek seferde** | bin satirda bin cizim |
| 3 | `await` her zaman UI'ya doner | cross-thread hatasi yok |
| 4 | Asamali (`IAsyncEnumerable`) yukleme uzun listede | ilk satir erken gorunur |

---

## §4 Performans karsilastirmasi

| Islem | Yavas | Hizli |
|---|---|---|
| Grid doldurma | satir satir `Add` | `DataSource = liste` / `BeginUpdate` |
| Ag + hesap | UI thread'de | `Task.Run` |
| Ayni anda N sorgu | hepsi tamamlanana kadar | iptal + en son |
| `Text` guncelleme | her karakterde tam `Text` | `StringBuilder` + tek atama |

---

## §5 Kim neyi degistirir

| Kim | Ne yapar | Kanit |
|---|---|---|
| `developer` | `async` yukleme + `finally` ile busy kapatma | kod incelemesi |
| `ui-designer` | busy yuzeylerinin (imlec, progress, durum) tasarimi | ekran tasarimi |
| `reviewer` | `Load` ici senkron I/O ve satir-satir `Add` tarar | grep ciktisi |
| `tester` | "ikinci tik kazanir" ve iptal davranisini sinar | davranis testi |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Her yukleme `async` + busy durumu | kod incelemesi |
| 2 | Busy yuzeyleri `finally`'da kapaniyor | kod incelemesi |
| 3 | Arka plan donusunde UI'a `Invoke` / `await` var | kod incelemesi |
| 4 | Yeniden tetiklemede iptal var | davranis testi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\async-loading-patterns.md` (kaynak vault) | senaryo tablosu, busy kalibi, busy yuzeyi listesi, last-write-wins, performans karsilastirmasi |
| `.rules\ui-rules.md` | arayuz busy durumu ve kod-behind siniri |
| `.rules\performance-guidelines.md` | toplu guncelleme ve arka plan hesap kurali |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
