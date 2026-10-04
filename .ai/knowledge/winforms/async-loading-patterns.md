---
title: "async-loading-patterns - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Form acilisinda ve sorgu degisiminde verinin nasil yuklenecegini, yuklenirken arayuzun nasil mesgul gosterilecegini ve veri art arda geldiginde kazananin nasil belirlendigini tanimlar."
---

# Asenkron Yukleme, Busy Durumu ve Asamali Doldurma

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [winforms-rendering-performance](winforms-rendering-performance.md) · [ui-rules](../../.rules/ui-rules.md)

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
