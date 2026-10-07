---
title: "WinForms Render Performansı: Paint, DoubleBuffered, Optimizasyon"
type: reference
category: winforms
status: active
version: 1.0.0
updated: 2026-09-24
authority: SSOT
scope: winforms-rendering-performance
---

# WinForms Render Performansı

## §0 Özet (Answer First)

WinForms render performansının tek SSOT'u: **boya maliyetini ölç, tampondan kaçın, flicker'i DoubleBuffered ile kes, ağır kontrolleri virtualize et.** `_Paint` içinde yeni `Brush`/`Pen` üretme, `_Paint` dışında `_ Invalidate()` çağırma.

| Kural | Gerekçe |
|---|---|
| `Brush`/`Pen`/`Font`'u `_Paint` içinde her seferinde new'leme | Gereksiz GC baskısı, her karede alloc |
| `DoubleBuffered = true` (ve `ControlStyles` ayarları) | Flicker (çift boyama) engeli |
| `_Paint` içinde `TextRenderer.DrawText` + hazır brush | GDI handle sıfırmalloc |
| `_ Invalidate()` — `_ Refresh()` değil | `_Refresh` senkron tam boyama; `_Invalidate` kuyruğa alır |
| `_UpdateStyles()` sonra `_Invalidate()` | Stil değişimi sonrası tek geçiş |

## §1 Teknik Gövde

### Paint pipeline ve maliyet merkezleri
Bir kontrol `_OnPaint` → `PaintEventArgs.Graphics` → GDI draw çağrıları → (_Invalidate ile) kuyruk. Maliyetin %80'i yeni GDI nesnesi alloc + tam ekran `_Refresh` çağrılarından gelir.

```csharp
public sealed class GaugeControl : Control
{
    private static readonly Brush FillBrush = new SolidBrush(Color.SteelBlue); // static: bir kez
    private static readonly Pen OutlinePen = new Pen(Color.Black, 1f);

    public GaugeControl()
    {
        DoubleBuffered = true;                    // flicker yok
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        UpdateStyles();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        TextRenderer.DrawText(e.Graphics, Value.ToString(), Font,
            ClientRectangle, ForeColor);          // hazır brush, alloc yok
        e.Graphics.FillEllipse(FillBrush, _rect);
        e.Graphics.DrawEllipse(OutlinePen, _rect);
    }

    private int _value;
    public int Value
    {
        get => _value;
        set { if (_value == value) return; _value = value; Invalidate(); }  // sadece Invalidate
    }
}
```

### DoubleBuffered ve flicker kontrolü
Flicker, WM_PAINT'in iki aşamada boyamasından çıkar. Çözüm:

```csharp
SetStyle(ControlStyles.OptimizedDoubleBuffer |  // tampondan çiz
         ControlStyles.AllPaintingInWmPaint |    // EraseBkgnd'i atla
         ControlStyles.UserPaint, true);
UpdateStyles();
```

Kök formda flicker varsa `_DoubleBuffered` private — reflection yerine türetilmiş formda `SetStyle` ile açılır.

### Ölçüm: nerede yavaş
```csharp
var sw = Stopwatch.StartNew();
// şüpheli blok
sw.Stop();
Debug.WriteLine($"paint: {sw.ElapsedMilliseconds}ms");
```
Şüpheli blok şunlardır: `_Paint` içinde `_TextRenderer` on binlerce kez (listbox), `_Refresh` zinciri (dock yeniden hesap), `_SuspendLayout` eksikliğinde layout fırtınası (bkz. anchor-dock-layout-rules).

### Virtualization: `VirtualMode`
Büyük veri (>1.000 satır) için `ListBox.DrawMode = OwnerDrawFixed` + `VirtualMode = true`; yalnızca görünen satırlar çekilir.

```csharp
listBox.DrawItem += (s, e) =>
{
    e.DrawBackground();
    TextRenderer.DrawText(e.Graphics, _rows[e.Index], e.Font, e.Bounds, e.ForeColor);
};
listBox.IntegralHeight = false;
```

### Nerede kullanılmaz
- `_Paint` içinde dosya/DB erişimi (senkron donma)
- `_Timer` tick'de `_Refresh()` (sürekli repaint)
- `_TopLevel` her tick `_CreateControl()` (handle fırtınası)

## §2 Uygulama Kontrol Listesi

| # | Adım | Doğrulama |
|---|---|---|
| 1 | `Brush`/`Pen`/`Font` static veya field olarak bir kez üretiliyor mu? | Paint alloc 0 |
| 2 | `ControlStyles.OptimizedDoubleBuffer + AllPaintingInWmPaint + UserPaint` açık mı? | Flicker yok |
| 3 | `_Invalidate()` mi `_Refresh()` mi kullanılıyor? | Tek geçiş |
| 4 | `Value` gibi property değişimi `_Invalidate()` çağırıyor mu? | Canlı güncelleme |
| 5 | 1.000+ satır listelerde `VirtualMode`/`OwnerDraw` düşünüldü mü? | 60 FPS hedefi |
| 6 | `_Paint` içinde IO yok mu? | UI thread donmuyor |

## §3 İzinli Link Hedefleri

| Hedef | Durum |
|---|---|
| `../../CLAUDE.md` | Var → link verilir |
| `../index.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| `designer-cs-trio-rule.md` | Kardeş → link verilir |
| `async-await-ui-thread.md` | Kardeş → link verilir |

## §4 Yasaklar

1. `_Paint` içinde `new SolidBrush`/`new Pen`/`new Font` (GDI alloc — static'e taşı).
2. `_Paint` içinde dosya/DB/HTTP erişimi (UI donması).
3. `_Refresh()` ile gereksiz senkron tam boyama (yerine `_Invalidate`).
4. 1.000+ öğeli `ListBox`'da `VirtualMode` olmadan render (kaydırma donması).
5. `DoubleBuffered`'ı reflection ile private field'a yazmak (yerine türetilmiş formda `SetStyle`).
