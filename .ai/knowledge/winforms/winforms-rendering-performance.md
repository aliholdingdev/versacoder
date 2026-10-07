---
title: "winforms-rendering-performance - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "WinForms render performansi: Paint maliyeti, DoubleBuffered/flicker, Invalidate ile Refresh ayrimi, VirtualMode ve olcum yontemi"
---

# WinForms Render Performansi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [performance-guidelines](../../.rules/performance-guidelines.md) · [ui-rules](../../.rules/ui-rules.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [logging-integration](logging-integration.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md)

> **Tek cumlelik sorumluluk:** *WinForms'ta neden donuyor, onlemin kaniti nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Boya maliyetini **olc**, tampondan kacin, flicker'i **DoubleBuffered** ile kes, agir kontrolleri **virtualize** et. `_Paint` icinde yeni `Brush`/`Pen`/`Font` uretilmez; `_Paint` disinda **`Invalidate()`** cagrilir, `Refresh()` degil. Buyuk liste icin `VirtualMode` + `OwnerDraw` kullanilir; `_Paint` icinde dosya/veritabani/HTTP erisimi **yasaktir** (UI thread donar).

```text
+--------------------------------------------------------------+
| OnPaint -> Graphics -> GDI cagrilari -> kuyruk (Invalidate)  |
|     |                                                        |
|     +--> maliyet: yeni GDI nesnesi uretimi (GC baskisi)      |
|     +--> maliyet: senkron Refresh (tam ekran boyama)         |
|     +--> onlem : static brush/pen + Invalidate + bufe        |
+--------------------------------------------------------------+
```

---

## §1 Kural - gerekce tablosu

| Kural | Gerekce |
|---|---|
| `Brush`/`Pen`/`Font`'u `_Paint` icinde her seferinde new'lemek | gereksiz GC baskisi, her karede tahsis |
| `DoubleBuffered = true` + `ControlStyles` ayarlari | flicker (cift boyama) engeli |
| `TextRenderer.DrawText` + hazir brush | GDI handle tasarrufu |
| `Invalidate()` - `Refresh()` degil | `Refresh` senkron tam boyama, `Invalidate` kuyruklar |
| `UpdateStyles()` sonrasi `Invalidate()` | stil degisimi sonrasi tek gecis |

---

## §2 Kalip kod

```csharp
public sealed class GaugeControl : Control
{
    private static readonly Brush FillBrush = new SolidBrush(Color.SteelBlue); // bir kez
    private static readonly Pen OutlinePen = new Pen(Color.Black, 1f);

    public GaugeControl()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        UpdateStyles();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        TextRenderer.DrawText(e.Graphics, Value.ToString(), Font,
            ClientRectangle, ForeColor);          // hazir brush, alloc yok
        e.Graphics.FillEllipse(FillBrush, _rect);
        e.Graphics.DrawEllipse(OutlinePen, _rect);
    }

    private int _value;
    public int Value
    {
        get => _value;
        set { if (_value == value) return; _value = value; Invalidate(); }
    }
}
```

Kok formda flicker varsa `DoubleBuffered` ozel alandir; reflection ile yazmak yerine turetilmis formda `SetStyle` ile acilir.

---

## §3 Olcum ve virtualizasyon

| Adim | Yapilan | Kanit |
|---|---|---|
| 1 | Supheli blok `Stopwatch` ile olculur, `Debug.WriteLine` ile yazilir | ms ciktisi |
| 2 | Supheli yerler: `_Paint` icinde binlerce `DrawText`, `Refresh` zinciri, eksik `SuspendLayout` | satir no |
| 3 | 1.000+ satir icin `VirtualMode = true` + `OwnerDrawFixed` | yalniz gorunen satirlar |
| 4 | `IntegralHeight = false` | olcu degisiminde yeniden hesap azalir |
| 5 | Timer tick'te `Refresh()` yok | surekli repaint biter |

| Nerede kullanilmaz | Sonuc |
|---|---|
| `_Paint` icinde dosya/DB erisimi | senkron donma |
| Timer tick'te `Refresh()` | surekli repaint |
| Her tick'te `CreateControl()` | handle fintasi |

---

## §4 Kontrol listesi

| # | Adim | Dogrulama |
|---|---|---|
| 1 | `Brush`/`Pen`/`Font` static ya da field olarak bir kez uretiliyor | paint alloc 0 |
| 2 | `OptimizedDoubleBuffer + AllPaintingInWmPaint + UserPaint` acik | flicker yok |
| 3 | `Invalidate()` mi `Refresh()` mi kullaniliyor | tek gecis |
| 4 | Ozellik degisimi `Invalidate()` cagiriyor mu | canli guncelleme |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\winforms-rendering-performance.md` (kaynak vault) | 5 kural, `GaugeControl` kalibi, olcum adimlari, virtualizasyon, 3 yasak |
| `.rules\performance-guidelines.md` | performans kurali |
| `.rules\ui-rules.md` | ekran davranisi kurali |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur. "80% maliyet" gibi olcusuz oranlar metinden cikarildi, yazilmadi.
