---
title: "control-disposal-and-memory - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "WinForms'ta GDI+ nesneleri, event abonelikleri, Form ve Control disposal sirasini ve bellek sizintisi teshisini tanimlar."
---

# Control Disposal ve Bellek Yonetimi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [form-close-and-dispose](form-close-and-dispose.md) · [winforms-rendering-performance](winforms-rendering-performance.md) · [performance-budget-check](../../checklists/performance-budget-check.md) · [performance-guidelines](../../.rules/performance-guidelines.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md)

> **Tek cumlelik sorumluluk:** *Hangi nesne ne zaman birakilir ve bellek/GDI sizintisi nasil teshis edilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

WinForms bellek yonetiminin tek kaynagi: **GDI+ nesnelerini (`Brush`, `Pen`, `Font`, `Icon`) `using` veya field ile yonet, event aboneliklerini `Dispose`'da kes, `Control`'lari `Controls.Remove + Dispose` sirasiyla birak.** `Form` asla GC'ye birakilmaz - `ShowDialog` sonrasi `using`, `Show` sonrasi `Close` tetikler `Dispose`.

```text
+------------------------------------------------------------------+
| Brush/Pen  : Paint basina using, ya da static field              |
| Font       : field; degisince eskisini Dispose                   |
| Event      : Dispose'da `-=` (yayinci uzun omurlu ise)           |
| Image      : using ya da sahibi ImageList.Dispose                |
| Timer      : Dispose'da Stop + Dispose                           |
+------------------------------------------------------------------+
```

---

## §1 Nesne omur tablosu

| Nesne | Yasam dongusu | Kural |
|---|---|---|
| `SolidBrush` / `Pen` | `_Paint` basina | static field veya `using` |
| `Font` | kontrol omru | field; `OnFontChanged`'de eskisini `Dispose` |
| Event aboneligi | yayinci omru | `Dispose`'da `-=` |
| Gorsel (`Image`) | kullanim | `using` veya `_ImageList.Dispose` |
| `Timer` | form | `Dispose`'da `Stop` |

---

## §2 Form disposal ornekleri

Modal (`ShowDialog`) icin:

```csharp
using (var dlg = new EditCustomerForm())
{
    if (dlg.ShowDialog(this) == DialogResult.OK)
        LoadCustomers();     // OK -> is
}  // Dispose -> tum child'lar, event abonelikleri temizlenir
```

Modeless (`Show`) icin:

```csharp
_detailForm = new DetailForm();   // field: yasam dongusu formla esit
_detailForm.Show(this);
// kapaninca Dispose tetiklenir (Close -> Dispose); field'i null yap
_detailForm.Closed += (s, e) => _detailForm = null;
```

---

## §3 GDI+ nesneleri: en sik sizinti

```csharp
// YANLIS: her Paint'te new -> GC + GDI handle baskisi
protected override void OnPaint(PaintEventArgs e)
{
    using var b = new SolidBrush(Color.Red);   // DOGRU: using
    e.Graphics.FillRectangle(b, ClientRectangle);
}

// DOGRU: field olarak bir kez
private static readonly Brush _bg = new SolidBrush(Color.White);
```

`Font` degisiminde eski `Font` dispose edilmezse GDI handle sizar; cozum `OnFontChanged` override + field font. `PictureBox.Image`, `Form.Icon`, `ImageList.Image` hep `IDisposable`; paylasilan gorseller `using` ile degil, sahibi olan `ImageList.Dispose` ile birakilir.

---

## §4 Event aboneliklerini kesme

```csharp
public partial class DashboardForm : Form
{
    private readonly IOrderService _orders;

    public DashboardForm(IOrderService orders)
    {
        InitializeComponent();
        _orders = orders;
        _orders.OrderChanged += OnOrderChanged;   // abone
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _orders.OrderChanged -= OnOrderChanged;  // DISPOSE'da kes
            components?.Dispose();                    // Designer bilesenleri
        }
        base.Dispose(disposing);
    }
}
```

---

## §5 Bellek sizintisi teshisi

Bir form **100 kez ac-kapa** yap; Task Manager'da GDI Objects sayaci sabit kalmalidir. Artiyorsa kaynak iki tanidir: event aboneligi ya da GDI+ sizintisi. Sonuc olcumu bu vault'ta yapilmamistir -> **DOĞRULAMA GEREKLI**.

| # | Belirti | Sik kok neden | Duzeltme |
|---|---|---|---|
| 1 | GDI Objects sayaci yukseliyor | `Paint` icinde `using`'siz `new SolidBrush` | `using` veya static field |
| 2 | Form GC'den kurtuluyor | uzun omurlu yayinciya `-=` yok | `Dispose`'da `-=` |
| 3 | Ac-kapa sonrasi hala timer tikliyor | `Timer` Stop/Dispose yok | `Dispose`'da `Stop` + `Dispose` |
| 4 | Gorunum bulanik / handle hatasi | `Image` static tutulmus, sahibi yok | sahibi `ImageList.Dispose` |
| 5 | Designer nesneleri siziyor | `components?.Dispose()` cikarilmis | geri `Dispose(bool)` icine koy |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `ShowDialog` formlari `using` ile mi | Dispose garantisi (kod incelemesi) |
| 2 | Event abonelikleri `Dispose`'da `-=` mi | yayinci -> form dongusu kirilir |
| 3 | `_Paint` brush/pen `using` veya static mi | kod incelemesi |
| 4 | `components?.Dispose()` `Dispose(bool)` icinde mi | kod incelemesi |
| 5 | 100x ac-kapa sonrasi GDI Objects sabit mi | olcum ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\control-disposal-and-memory.md` (kaynak vault) | nesne omur tablosu, modal/Modeless disposal ornegi, GDI+ kalibi, abonelik kesme, sizinti teshis adimlari |
| `.rules\performance-guidelines.md` | Paint yolu ve GDI handle kurali |
| `checklists\performance-budget-check.md` | bellek/GDI butcesi denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
