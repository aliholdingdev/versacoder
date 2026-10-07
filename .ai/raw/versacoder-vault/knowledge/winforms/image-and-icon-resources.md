---
title: "WinForms — İkon ve Görsel Kaynaklarının Yönetimi"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "İkon, görsel ve SVG/px imzalarının nerede saklanacağını, `Image` nesnelerinin nasıl serbest bırakılacağını ve her çizimde yeni görsel üretmenin yarattığı bellek yükünü"
---

# İkon ve Görsel Kaynaklarının Yönetimi

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

İkonlar **kaynak olarak** saklanır (form `.resx` ya da `Properties/Resources.resx`), klasöre `new Bitmap("...")` ile **dosya yolundan okunmaz**. `Image`/`Bitmap` **`IDisposable`**'dır: kod içinde üretilen her görsel `using` ile sarılır ya da bırakılır. `Paint`/`DrawItem` gibi **her çizimde** yeni `Bitmap` üretmek bellek sızıntısı ve titreme yapar — görsel önceden hazırlanır, yalnızca çizilir.

| # | Kaynak tipi | Nerede | Erişim |
|---|---|---|---|
| 1 | Form ikonu | `XxxForm.resx` | `ComponentResourceManager` |
| 2 | Uygulama ikonu | `Properties/Resources.resx` | `Properties.Resources.AppIcon` |
| 3 | Ribbon buton görseli | form `.resx` (`.Designer.cs` bağlar) | tasarımcı |
| 4 | Çalışma zamanında üretilen görsel | bellek (`Bitmap`) | `using` + `Dispose` |
| 5 | Diskten okunan görsel | **yasak** (mutlak yol) | — |

### §1.1 Doğru / yanlış

```csharp
// YANLIS — her çizimde yeni Bitmap, Dispose yok (bellek sizi)
private void lst_ItemPaint(object sender, DevExpress.XtraEditors.Drawing.ListItemPaintEventArgs e)
{
    var bmp = new Bitmap(32, 32);        // her seferinde
    using (var g = Graphics.FromImage(bmp)) { /* ... */ }
    e.Graphics.DrawImage(bmp, 0, 0);     // bmp hic Dispose edilmez
}
```

```csharp
// DOĞRU — görsel bir kez üretilir, tekrar tekrar çizilir
private static readonly Image _icon = Properties.Resources.StatusOk;   // kaynak

private void lst_ItemPaint(object sender, ... e)
{
    e.Graphics.DrawImage(_icon, 0, 0);   // Dispose YOK — sahibi kaynaktır
}
```

### §1.2 Bellek kuralı tablosu

| # | Durum | Ne yapılır |
|---|---|---|
| 1 | Kaynak resmi (`Properties.Resources.X`) | **`Dispose` etme** — ortak sahip |
| 2 | Kod içinde `new Bitmap(...)` | `using` bloğu **zorunlu** |
| 3 | `Clone()` / `new Bitmap(geçerliResim)` | `using` + `Dispose` |
| 4 | `ImageList`'e eklenen resim | `ImageList` sahiptir; kopyasını ver |
| 5 | Form kapanınca | `_staticImage` hariç tüm görseller bırakılır |

### §1.3 Performans belirtileri

| # | Belirti | Kök neden | Düzeltme |
|---|---|---|---|
| 1 | Bellek sürekli artıyor | `Paint` içinde `new Bitmap` | önceden üret + önbellek |
| 2 | Kaydırırken titreme | her satırda yeni `Image` | `ImageList` / ön bellek |
| 3 | 150% DPI'da bulanık ikon | 96 dpi ikon tek boyut | çok çözünürlüklü kaynak |
| 4 | Yol hatası ("dosya bulunamadı") | göreli dosya yolu | kaynağı `.resx`'e taşı |
| 5 | Form açılışı yavaş | yüzlerce küçük dosya I/O | görselleri kaynakta tek paketle |

### §1.4 Ölçüm (kanıt üretir)

```powershell
# Kaynak diske yazilmis mi? (mutlak/goreli yol taramasi)
Select-String -Path "src\**\*.cs" -Pattern 'new (Bitmap|Icon)\(\s*"'   # beklenen: 0
# Bellek izlemesi: uygulamayi 20 kez form ac/kapa -> Private Bytes sabit kalmali
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Dosya yolundan görsel okuma yok | §1.4 komutu |
| 2 | Kod içinde üretilen her `Bitmap` `using`'de | kod incelemesi |
| 3 | `Paint` içinde görsel üretimi yok | kod incelemesi |
| 4 | 20× form aç/kapa sonrası bellek sabit | bellek ölçümü |
| 5 | İki bacakta `dotnet build` → 0 hata | build çıktısı |

## §3 İzinli Link Hedefleri

| # | Hedef | Bu dosyada |
|---|---|---|
| 1 | Kardeş dosya adları (bu dizindeki 30 dosya) | var |
| 2 | [index.md](index.md) — kayıt defteri | var |
| 3 | [../../CLAUDE.md](../../CLAUDE.md) — anayasa G1..G12 | var |
| 4 | `../../index.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/*.md` · `../index.md` | **yok → link verilmez** (`NEEDS_VALIDATION`) |

## §4 Yasaklar

| # | Yasak | İhlalde |
|---|---|---|
| 1 | DevExpress **v26.1** dışı sürüm referansı (G3/G8) | iş RED |
| 2 | `RibbonControlStyle.Office2010` dışı ribbon stili (G3) | iş RED |
| 3 | EF Core · `DbContext` · string-concat SQL (G2) | CRITICAL |
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · `.resx`'i yetkisiz ellemek | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) · `Dispose` edilmemiş `Bitmap` | RED |
