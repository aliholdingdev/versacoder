---
title: "WinForms — AutoScaleMode, DPI ve Pencere Ölçekleme"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "`AutoScaleMode` değerinin seçimini, yüksek-DPI'da metin/ikon taşmalarını ve `net48` ile `net10.0-windows` arasındaki DPI başlatma farkını tanımlar"
---

# `AutoScaleMode`, DPI ve Pencere Ölçekleme

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`AutoScaleMode = Font` (varsayılan önerilen) formun **yazı tipi büyüklüğüne** göre yeniden ölçeklenmesini sağlar; `Dpi` seçeneği ekran çözünürlüğüne göre, `None` ise hiç ölçeklemez. Yalnız **bir** mod kullanılır — `AutoScaleDimensions` ile **uyumsuz** mod seçmek formun açılırken **kaymasına** yol açar. Başlatma farkı (`net48` vs `net10.0-windows`) `#if` ile platform farkı olarak izole edilir.

| # | Mod | Ne ölçekler | Ne zaman |
|---|---|---|---|
| 1 | `Font` | yazı tipi boyutu değişince | **önerilen varsayılan** |
| 2 | `Dpi` | ekran DPI'ı değişince | tutarsız font algılaması olan ortamlar |
| 3 | `DpiAndFont` | ikisi | gereksinim netse |
| 4 | `None` | hiç | yalnız test/taşınabilir ekranlarda — **üretimde yasak** |

### §1.1 `.Designer.cs` görünümü

```csharp
// XxxForm.Designer.cs
this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
this.ClientSize = new System.Drawing.Size(1024, 720);
```

> **KURAL:** `AutoScaleDimensions`, form **tasarlanırken kullanılan** değerdir; elle değiştirilirse tüm yerleşim kayar. `.cs` dosyasında bu iki satır **bulunmaz**.

### §1.2 İki bacak: DPI başlatma farkı

| | `net48` | `net10.0-windows` |
|---|---|---|
| Başlatma | `Application.EnableVisualStyles()` + `app.config` `<System.Windows.Forms.DpiAwareness>` | `ApplicationConfiguration.Initialize()` |
| Manifest gerekli mi | genelde `app.config`/manifest | SDK `ApplicationConfiguration` yeterli |
| `#if` | `#if NETFRAMEWORK` | `#else` |
| Form kodu | **aynı** | **aynı** |

```csharp
#if NETFRAMEWORK          // yalniz platform/API farki (G6)
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
#else
    ApplicationConfiguration.Initialize();
#endif
```

### §1.3 Yüksek DPI belirtileri

| # | Belirti | Kök neden | Düzeltme |
|---|---|---|---|
| 1 | 150% DPI'da yazı kırpılıyor | sabit `AutoSize = false` | `AutoSize = true` / `TableLayoutPanel` |
| 2 | Form açılırken kontroller kayıyor | `AutoScaleDimensions` elle oynanmış | tasarımcıda sıfırla |
| 3 | Yalnız bir form bozuk | `AutoScaleMode = None` | `Font` yap |
| 4 | Net10'da farklı, net48'te farklı | başlatma kodu `#if`'in dışında | `#if` içine al (yalnız API farkı) |
| 5 | İkonlar bulanık | `AutoScaleMode.Dpi` + 96dpi ikon | `PictureBox` `SizeMode` / çok çözünürlüklü ikon |

### §1.4 Ölçüm (kanıt üretir)

```powershell
# AutoScaleMode tutarliligi — tum formlarda ayni mod mu?
Select-String -Path "src\**\*.Designer.cs" -Pattern "AutoScaleMode = " |
  ForEach-Object { $_.Line.Trim() } | Sort-Object -Unique
# Beklenen: tek tur cikti (Font), farkli deger = kural ihlali
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Tüm formlarda tek `AutoScaleMode` değeri | §1.4 çıktısı |
| 2 | `AutoScaleDimensions` elle değiştirilmemiş | `git diff` |
| 3 | 100% / 150% / 200% DPI'da görsel kontrol | ekran görüntüsü |
| 4 | `#if` yalnız başlatma API'si | grep `#if` |
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
| 4 | `.Form.cs`'e `AutoScaleDimensions`/`AutoScaleMode`/kontrol üretimi yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) · `#if` ile iş mantığı (G6) | RED |
