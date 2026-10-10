---
title: "form-close-and-dispose - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Formun kapanis kararinin nerede verildigini, iptal edilebildigini ve tum yonetilen/yonetilmeyen kaynaklarin ne zaman birakilacagini tanimlar."
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# `FormClosing`, `FormClosed` ve `Dispose` Temizligi

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/logging-integration|logging-integration]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 5/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

> **Tek cumlelik sorumluluk:** *Kapanma karari hangi olayda verilir, temizlik nerede yapilir ve kaynaklar ne zaman birakilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kapatma karari **`FormClosing`**'te verilir (`e.Cancel = true` ile engellenir), temizlik **`FormClosed`**'da yapilir, kaynak birakimi **`Dispose`**'da. `Dispose` govdesi `.Designer.cs`'tir ve elle **dokunulmaz**; kod-behind'a ek temizlik `FormClosed`'a ya da `Dispose(bool disposing)` asiri yuklemesine konur. `IDisposable` tutulan her servis `FormClosed` icinde serbest birakilir - GC'ye birakilmaz.

```text
+---------------------------------------------------------------+
| FormClosing : karar + iptal mumkun  (kod-behind)              |
| FormClosed  : geri donus yok, hala erisilebilir (kod-behind)  |
| Dispose     : components + tutamac  (.Designer.cs - DOKUNMAZ) |
| DialogResult: ShowDialog donus degeri (kod-behind)            |
+---------------------------------------------------------------+
```

---

## §1 Asama tablosu

| # | Olay / metot | Ne yapar | `e.Cancel` mumkun mu | Dosya |
|---|---|---|---|---|
| 1 | `FormClosing` | kapatma karari oncesi | **evet** | `.Form.cs` |
| 2 | `FormClosed` | kapandiktan sonra, hala erisilebilir | hayir | `.Form.cs` |
| 3 | `Dispose(bool)` | tutamacalar, `components`, timer birakimi | - | `.Designer.cs` |
| 4 | `DialogResult` | `ShowDialog` donus degeri | - | `.Form.cs` |

---

## §2 Onayli kapatma

```csharp
private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    if (_dirty == false) return;

    var result = MessageBox.Show(
        this,
        "Kaydedilmemis degisiklikler var. Cikilsin mi?",
        "Onay",
        MessageBoxButtons.YesNoCancel,
        MessageBoxIcon.Warning);

    if (result == DialogResult.Cancel) { e.Cancel = true; return; }
    if (result == DialogResult.Yes)     { _ = SaveAsync(); return; }
    // DialogResult.No -> kapatmaya izin ver
}
```

> **KURAL:** `async` kaydetme `FormClosing` icinde **await edilemez** (form kapanirken beklenemez). Ya senkron kaydet, ya da `FormClosed`'da `Task.Run` ile arkaya al ve sonucu raporla.

---

## §3 Temizlik sirasi

```csharp
// SettingsForm.cs
private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
{
    _settingsService.Changed -= OnSettingsChanged;   // 1) abonelik kaldir
    _autoSaveTimer.Stop();                            // 2) zamanlayici durdur
    _autoSaveTimer.Dispose();                         // 3) IDisposable birak
    _cts.Cancel();                                    // 4) bekleyeni iptal et
    _cts.Dispose();
}
```

| # | Birakilacak | Neden | Birakilmazsa |
|---|---|---|---|
| 1 | Olay abonelikleri | sahibi uzun omurlu servis | form GC'den kurtulur -> **bellek sizintisi** |
| 2 | `Timer` | unmanaged-timer esdegeri | olay tetiklenmeye devam eder |
| 3 | `CancellationTokenSource` | bekleyen `Task` | is form kapandiktan sonra UI'a dokunur |
| 4 | `HttpClient` / `DbConnection` **yalniz sahiplik varsa** | socket / baglanti havuzu | baglanti sizintisi |

---

## §4 `Dispose`'a elle dokunmak yasak

```csharp
// SettingsForm.Designer.cs - bu govde designer'in urettigi koddur
protected override void Dispose(bool disposing)
{
    if (disposing && (components != null))
    {
        components.Dispose();
    }
    base.Dispose(disposing);
}
```

`.Form.cs` icinde ikinci bir `Dispose(bool)` asiri yuklemesi **yazilmaz**; fazla temizlik `FormClosed`'a konur. Boylece `.Designer.cs` <-> `.resx` tutarliligi bozulmaz.

---

## §5 `using` ile modal form

```csharp
using (var dlg = new ConfirmDialogForm())
{
    if (dlg.ShowDialog(this) != DialogResult.OK) return;
    // dlg.Dispose() blok sonunda otomatik
}
```

---

## §6 Kim neyi degistirir

| Kim | Ne yapar | Kanit |
|---|---|---|
| `developer` | `FormClosing` onayi ve `FormClosed` temizligini yazar | davranis testi |
| `ui-designer` | `Dispose(bool)` govdesini korur (`.Designer.cs`) | dosya incelemesi |
| `reviewer` | `.Form.cs`'te ikinci `Dispose(bool)` ve abonelik sizintisi tarar | grep hedefi: 0 eslesme |
| `tester` | kirli formda kapatma onayini sinar | davranis testi |

---

## §7 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Kirli formda `FormClosing` onayi var | davranis testi |
| 2 | Her `IDisposable` alan `FormClosed`'da birakiliyor | kod incelemesi |
| 3 | `.Form.cs`'te `Dispose(bool)` asiri yuklemesi yok | grep ciktisi |
| 4 | Olay abonelikleri kaldiriliyor (uzun omurlu serviste) | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\form-close-and-dispose.md` (kaynak vault) | asama tablosu, onayli kapatma, temizlik sirasi, birakilmayacak kaynaklar, DisposeGovdesi yasagi, using kalibi |
| `.rules\coding-standards.md` | kod-behind / .Designer.cs yetki siniri |
| `checklists\winforms-designer-check.md` | `Dispose(bool)`Govdesinin Designer'da kalmasi (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
