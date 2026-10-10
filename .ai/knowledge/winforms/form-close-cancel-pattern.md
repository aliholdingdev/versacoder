---
title: "form-close-cancel-pattern - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "FormClosing iptalini, kaydet-sorma oruntusunu, CloseReason ayrimini ve FormClosed ile Dispose arasindaki sirayi tanimlar."
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Kapanisi Iptal Etme ve Kaydet-Sorma

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[.rules/security-rules|security-rules]] · [[.rules/error-handling-rules|error-handling-rules]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 3/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

> **Tek cumlelik sorumluluk:** *Form kapatilirken veri kaybini onleyen iptal oruntusu nerede kurulur, `FormClosing` ile `FormClosed` arasindaki fark nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kapanista veri kaybi riski varsa tek adres **`FormClosing`**'dir: `e.Cancel = true` ile kapanis durdurulabilir ve kullaniciya "Kaydet / Kaydetme / Vazgec" sorulabilir. `FormClosed` **geri donusu olmayan** asamadir; sadece kalan temizlik (`-=` unsubscribe, `Dispose`) icindir. `CloseReason` okunarak iptal davranisi **yalniz kullanici kapanisinda** uygulanir (uygulama kapanisi sessizce gecilir). Iptal edilen kapanista arka planda calisan is **calismaya devam eder** - iptal bagi `CancellationTokenSource` ile baglanir.

```text
+------------------------------------------------------------------+
| FormClosing : Cancel = true -> kapanis DURUR, form gorunur kalir |
| FormClosed  : geri donus yok - artik olay dinlenemez              |
| Dispose     : designer components + handle (using bunu yapar)    |
| CloseReason : kaydet-sorma yalniz UserClosing'te                 |
+------------------------------------------------------------------+
```

---

## §1 Kapanis asamalari

```text
Close() / X / Application.Exit
        |
        v
  FormClosing (Cancel = true  ->  kapanis DURUR, form gorunur kalir)
        |  e.CloseReason = UserClosing | ApplicationExitCall | ...
        v
  FormClosed  (geri donus yok; artik olay dinlenemez)
        |
        v
  Dispose()   (designer components + handle; using bunu yapar)
```

| Olay | `Cancel` var mi | Tipik is | Risk |
|---|---|---|---|
| `FormClosing` | **Evet** | Kaydet-sorma, dogrulanmamis is uyarisi | Iptal ederse temizlik yapilmamis sayilir |
| `FormClosed` | Hayir | `-=` abonelik temizligi, son log | Gec kaldin - iptal yok |
| `Dispose` | Hayir | Kaynak birakma (image, timer) | Elle cagirmak yerine `using` / `Close()` |

---

## §2 Kaydet-sorma ornegi

```csharp
// DocumentForm.cs - kullanici mantigi; kontrol/SuspendLayout YOK (.Designer.cs)
private bool _dirty;   // degisiklik takibi

private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
{
    if (e.CloseReason != CloseReason.UserClosing) return;  // uygulama kapanisi: sorusuz gec
    if (!_dirty) return;

    var cevap = MessageBox.Show(this, "Kaydedilmemis degisiklik var. Kaydedilsin mi?",
                                "VersaCoder", MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
    if (cevap == DialogResult.Cancel) { e.Cancel = true; return; }   // vazgec -> form acik kalir
    if (cevap == DialogResult.No)     return;                        // kaydetme -> kapan
    e.Cancel = !Kaydet();          // Kaydet() false ise kapanisi IPTAL ET (veri kaybi yok)
}
```

---

## §3 CloseReason tablosu

| `CloseReason` | Kaynak | Kaydet-sorma uygulanir mi |
|---|---|---|
| `UserClosing` | X, ESC, `Close()` kullanici eliyle | **Evet** |
| `ApplicationExitCall` | `Application.Exit()` | Hayir (kullanici zaten uygulamayi kapatiyor) |
| `MdiFormClosing` | Ana form kapaniyor | Hayir |
| `TaskManagerClosing` | Windows kapatiyor | Hayir |
| `FormOwnerClosing` | Sahip form kapandi | Hayir |
| `WindowsShutDown` | Sistem kapanisi | Hayir (islemi kisa tut) |

> **UYARI:** Iptal dalinda `MessageBox` gostermek modal zinciri uzatir; toplu is (binlerce satir kaydet) `async` ise kapanis olayinda senkron blok **yapma** - kapanisi iptal et, isi arka planda bitir.

---

## §4 Iki bacak farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `FormClosingEventArgs.CloseReason` degerleri | **ayni** | ayni |
| `MessageBox` davranisi | **ayni** | ayni |
| Kapanista kaynak temizligi | `Dispose(bool)` + `components` | ayni |
| Kullanilan dil | C# 7.3 (fonksiyonel `if/return` zinciri) | ayni kod her iki bacakta derlenir |

---

## §5 Kim neyi degistirir

| Kim | Ne yapar | Ne yapmaz |
|---|---|---|
| `developer` | `FormClosing` govdesine onay ve `CloseReason` filtresi yazar | `FormClosed`'da iptal denemek |
| `ui-designer` | `Dispose` govdesini `.Designer.cs`'te birakir | kapanis koduna dokunmak |
| `reviewer` | `CloseReason` filtresiz `MessageBox` ve `FormClosing` icinde `await` tarar | - |
| `tester` | Vazgec / Kaydetme / Kaydet yollarini sinar | - |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `CloseReason != UserClosing` erken donusu var | kod incelemesi |
| 2 | `e.Cancel = true` sadece `FormClosing`'te | kod incelemesi |
| 3 | Kirli formda kaydet-sorma davranisi sinandi | davranis testi |
| 4 | `FormClosed` icinde abonelik + IDisposable temizligi var | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\form-close-cancel-pattern.md` (kaynak vault) | asama diyagrami, asama tablosu, kaydet-sorma ornegi, CloseReason tablosu, iki bacak farki |
| `.rules\security-rules.md` | veri kaybi ve onay kurali |
| `.rules\error-handling-rules.md` | `Kaydet()` false yolunun hata sayilmamasi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
