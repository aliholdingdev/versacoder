---
title: "WinForms Control Disposal ve Bellek Yönetimi"
type: reference
category: winforms
status: active
version: 1.0.0
updated: 2026-09-24
authority: SSOT
scope: winforms-control-disposal
---

# Control Disposal ve Bellek Yönetimi

## §0 Özet (Answer First)

WinForms bellek yönetiminin tek SSOT'u: **GDI+ nesnelerini (`Brush`,`Pen`,`Font`,`Icon`) `using` veya field ile yönet, event aboneliklerini `Dispose`'da kes, `Control`'ları `Controls.Remove + Dispose` sırasıyla bırak.** `Form` asla GC'ye bırakılmaz — `ShowDialog` sonrası `using`, `Show` sonrası `Close` tetikler `Dispose`.

| Nesne | Yaşam döngüsü | Kural |
|---|---|---|
| `SolidBrush`/`Pen` | `_Paint` başına | static field veya `using` |
| `Font` | kontrol ömrü | field; `OnFontChanged`'de eskisini `Dispose` |
| Event aboneliği | yayıncı ömrü | `Dispose`'da `-=` (bkz. code-behind-event-wiring) |
| Görsel (`Image`) | kullanım | `using` veya `_ImageList.Dispose` |
| `Timer` | form | `Dispose`'da `_Stop` |

## §1 Teknik Gövde

### Form disposal sırası
```csharp
using (var dlg = new EditCustomerForm())
{
    if (dlg.ShowDialog(this) == DialogResult.OK)
        LoadCustomers();     // OK → iş
}  // Dispose → tüm child'lar, event abonelikleri temizlenir
```

`Show` (modeless) için:
```csharp
_detailForm = new DetailForm();   // field: yaşam döngüsü formla eşit
_detailForm.Show(this);
// kapanınca Dispose tetiklenir (Close → Dispose); field'ı null yap
_detailForm.Closed += (s, e) => _detailForm = null;
```

### GDI+ nesneleri: en sık sızıntı
```csharp
// YANLIŞ: her Paint'te new → GC + GDI handle baskısı
protected override void OnPaint(PaintEventArgs e)
{
    using var b = new SolidBrush(Color.Red);   // DOĞRU: using
    e.Graphics.FillRectangle(b, ClientRectangle);
}

// DOĞRU: field olarak bir kez
private static readonly Brush _bg = new SolidBrush(Color.White);
```

### Event aboneliklerini kesme (Dispose'da)
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
            components?.Dispose();                    // Designer bileşenleri
        }
        base.Dispose(disposing);
    }
}
```

### `Font` değişimi
Kontrol `Font`'unu değiştirdiğinde eski `Font` dispose edilmezse GDI handle sızar. Çözüm: `OnFontChanged` override + field font.

### `Image`/`Icon` IDisposable
`PictureBox.Image`, `Form.Icon`, `ImageList.Image` hep `IDisposable`. Form `Dispose`'da serbest bırakılır; paylaşılan image'lar `using` ile değil, sahibi olan `ImageList.Dispose` ile.

### Bellek sızıntısı teşhisi
Bir form 100 kez aç-kapa yap; Task Manager'da (~) GDI Objects sayacı sabit kalmalı. Artıyorsa: event aboneliği veya GDI+ sızıntısı var.

## §2 Uygulama Kontrol Listesi

| # | Adım | Doğrulama |
|---|---|---|
| 1 | `ShowDialog` formları `using` ile mi? | Dispose garantili |
| 2 | Event abonelikleri `Dispose`'da `-=` mi? | Yayıncı → form döngüsü kırılır |
| 3 | `_Paint` brush/pen `using` veya static mi? | GDI handle 0 sızıntı |
| 4 | `components?.Dispose()` `Dispose(bool)` içinde mi? | Designer nesneleri temiz |
| 5 | Paylaşılan `ImageList` sahibince dispose mı? | Görsel GC'si |
| 6 | 100x aç-kapa sonrası GDI Objects sabit mi? | Sızıntı yok |

## §3 İzinli Link Hedefleri

| Hedef | Durum |
|---|---|
| `../../CLAUDE.md` | Var → link verilir |
| `../index.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| `form-close-and-dispose.md` | Kardeş → link verilir |
| `code-behind-event-wiring.md` | Kardeş → link verilir |

## §4 Yasaklar

1. `_Paint` içinde `new SolidBrush`/`new Pen` `using`'siz (GDI sızıntısı).
2. Event `-=` yapmadan uzun ömürlü yayıncıya abone olmak (memory leak).
3. Modeless `Form`'u field'sız yerel değişkende tutmak (yerine field + `Closed` null).
4. `components?.Dispose()`'u `Dispose(bool)`'dan çıkarmak (Designer nesneleri sızar).
5. `Image`/`Icon`'u sahibi olmadan static tutmak (kalıcı GDI handle).
