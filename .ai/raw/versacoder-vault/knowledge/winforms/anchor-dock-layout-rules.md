---
title: "WinForms — Anchor ve Dock ile Layout Kuralları"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Pencere boyutlandığında kontrollerin nasıl davranacağını, `Anchor`/`Dock` farkını, `AutoSize` tuzağını ve ayar penceresi kuralını tanımlar"
---

# `Anchor` ve `Dock` ile Layout Kuralları

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Boyut değişimini **yalnız** `Anchor` ve `Dock` yönetir; `Resize` olayında elle `Location`/`Size` **yazılmaz** (o kod `.Form.cs`'e tasarım kodu sokar ve DPI'da bozulur). Yatay bir şerit `Dock = Top/Bottom`, tüm alanı kaplayan panel `Dock = Fill`, sabit boyutlu etiket `Anchor = Top, Left`, genişleyen grid `Anchor = Top, Bottom, Left, Right` olur.

| # | Araç | Ne yapar | Ne zaman |
|---|---|---|---|
| 1 | `Anchor` | kaboya göre **bağıl** konum/boyut korur | grid/panel gibi serbest öğeler |
| 2 | `Dock` | kenara **yapışır**, sırayla yığılır | araç çubuğu, alt durum, içerik alanı |
| 3 | `AutoSize` | içerik kadar yer kaplar | buton/etiket |
| 4 | `TableLayoutPanel` | hücre hizası | form alanları |
| 5 | `SplitContainer` | kullanıcı böler | iki panelli ekran |

### §1.1 Karar tablosu

| Öğe | `Anchor` | `Dock` | Not |
|---|---|---|---|
| Alt durum çubuğu | — | `Bottom` | sırayla |
| Üst ribbon / araç şeridi | — | `Top` | `RibbonForm`'da ribbon zaten dock'lu |
| Sol gezinme paneli | — | `Left` | `SplitContainer` ile değişken |
| Merkez grid | `Top, Bottom, Left, Right` | `Fill` | ikisi de olur; `Fill` tercih |
| Form üzerindeki buton | `Top, Right` | — | sağ üstte sabit |
| Metin kutusu (tam genişlik) | `Top, Left, Right` | — | |
| Etiket (sabit) | `Top, Left` | — | `AutoSize = true` |

### §1.2 `.Designer.cs` içindeki görünüm

```csharp
// XxxForm.Designer.cs — layout ayarları tasarım kodudur, .cs'e yazılmaz
this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)
    ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
this.txtQuery.Anchor = ((System.Windows.Forms.AnchorStyles)
    (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
    | System.Windows.Forms.AnchorStyles.Right)));
```

> **KURAL:** `Anchor`/`Dock`/`AutoSize` değerleri **yalnız `.Designer.cs`** içindedir. `.Form.cs` içinde `this.btnOk.Anchor = ...` satırı varsa form üçlüsü kuralı ihlal edilmiştir.

### §1.3 `AutoSize` tuzağı

| # | Belirti | Neden | Düzeltme |
|---|---|---|---|
| 1 | Buton metni değişince arkasındaki kutu kırpılıyor | `AutoSize` sadece butonda | kapsayıcı `TableLayoutPanel` |
| 2 | Grid büyüyünce buton altına kaçıyor | `Anchor` eksik | `Top, Right` ata |
| 3 | Pencere küçülünce grid kayboluyor | `Anchor` yalnız `Top, Left` | `Fill` kullan |
| 4 | Elle `Resize` ile yerleşim | DPI'da bozulur | `Anchor`/`Dock`'a taşı |
| 5 | Şerit sırası ters | dock sırası yanlış | `Controls.Add` sırası `.Designer.cs`'te |

### §1.4 `Resize` olayı

`Form.Resize` yalnız **görsel efekt / durum geçişi** içindir (ör. taşma menüsünü gizle). **Yerleşim hesabı** oraya yazılmaz; bu, `.Form.cs`'in iş mantığına dönmesine ve iki bacakta farklı DPI sonuçlarına yol açar.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Yerleşim `Anchor`/`Dock` ile; `Resize`'ta konum kodu yok | grep `.Resize` gövdeleri |
| 2 | `Anchor`/`Dock` atamaları yalnız `.Designer.cs`'te | grep = 0 `.Form.cs` |
| 3 | 100% / 150% DPI'da form açılıp kontrol edildi | görsel kanıt |
| 4 | Pencere küçültme/genişletme testi yapıldı | görsel kanıt |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` / yerleşim ataması yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
