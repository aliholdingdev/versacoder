---
title: "form-close-and-dispose - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "Formun kapanis kararinin nerede verildigini, iptal edilebildigini ve tum yonetilen/yonetilmeyen kaynaklarin ne zaman birakilacagini tanimlar."
---

# `FormClosing`, `FormClosed` ve `Dispose` Temizligi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [logging-integration](logging-integration.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 5/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

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
