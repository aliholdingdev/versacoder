---
title: "WinForms — SplitContainer, Panel Sırası ve Boyut Kısıtları"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "`SplitContainer` kullanımının koşullarını, panel sırasının neden önemli olduğunu ve `MinimumSize`/`SplitterDistance` tuzaklarını tanımlar"
---

# `SplitContainer`, Panel Sırası ve Boyut Kısıtları

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`SplitContainer` **iki** paneli birlikte yönetir (`Panel1` + `Panel2`) ve **yalnızca dock'lanmış** olarak kullanılır; `Anchor` ile yerleştirilirse beklenmedik boyut davranışı çıkar. `SplitterDistance`, `Panel1MinSize`, `Panel2MinSize` üçlüsü birlikte ayarlanır — aksi halde `ArgumentException` (splitter mesafesi geçersiz) fırlar. Panel **sırası** `.Designer.cs`'teki `Controls.Add` sırasıyla belirlenir ve dock yerleşimini doğrudan etkiler.

| # | Özellik | Ne işe yarar | Sınırsız davranış |
|---|---|---|---|
| 1 | `Orientation` | dikey/yatay bölme | `Vertical` (varsayılan) |
| 2 | `SplitterDistance` | ayracın konumu | `Panel1` yüksekliğine eşitlenir |
| 3 | `Panel1MinSize` | sol panelin minimumu | 0 → kapanabilir |
| 4 | `Panel2MinSize` | sağ panelin minimumu | aşılırsa istisna |
| 5 | `FixedPanel` | yeniden boyutlanırken sabitle | `None` |

### §1.1 Doğru yerleşim

```csharp
// XxxForm.Designer.cs
this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
this.splitContainer1.Panel1MinSize = 220;
this.splitContainer1.Panel2MinSize = 360;
this.splitContainer1.SplitterDistance = 260;
```

> **KURAL:** `SplitterDistance` ataması, `Panel1MinSize`/`Panel2MinSize` atamalarından **sonra** yapılır; `ClientSize` henüz hesaplanmamışken verilen değer forma uymazsa istisna fırlar. Sıra `.Designer.cs`'in içindedir, `.cs`'e **taşınmaz**.

### §1.2 Ortak hatalar

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | `SplitContainer` `Anchor` ile konumlanmış | ayracın davranışı tutarsız | `Dock = Fill` |
| 2 | `SplitterDistance` < `Panel1MinSize` | `ArgumentException` | değerleri birlikte ayarla |
| 3 | İki `SplitContainer` iç içe (aynı formda 3+ kat) | performans + ölçü hatası | `TableLayoutPanel` + tek split |
| 4 | `Panel2`'ye dock'lu kontrol yanlış sırayla eklendi | kontrol taşar | `Controls.Add` sırasını `.Designer.cs`'te düzelt |
| 5 | Min boyut `MinimumSize` ile verilmemiş | pencere küçültünce kırılır | forma `MinimumSize` ver |

### §1.3 Dock sırası kuralı

```
Form
 +-- RibbonControl        (Dock = Top)      -> ilk eklenir
 +-- StatusBarControl     (Dock = Bottom)    -> sonra
 +-- SplitContainer       (Dock = Fill)      -> en son
```

| # | Sıra kuralı | Neden |
|---|---|---|
| 1 | `Fill` **en son** eklenir | dolu alana yer kalmaz |
| 2 | `Top`/`Bottom` sırayla | ilk eklenen dışa doğru yerleşir |
| 3 | `Left`/`Right` ardından | yığınsa sıralama belirleyicidir |
| 4 | Sıra `.Designer.cs`'te değişir | `.cs`'e taşınırsa tasarımcı çöker |

### §1.4 Alternatif: `TableLayoutPanel`

Küçük, sabit hizalı form alanları (ayar satırları) için `SplitContainer` yerine `TableLayoutPanel` kullanılır: satır/sütun `SizeType.Percent` olduğunda DPI'da çok daha kararlıdır.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | `SplitContainer` `Dock = Fill` | kod incelemesi |
| 2 | `SplitterDistance` ≥ `Panel1MinSize` ve ≤ `ClientSize - Panel2MinSize` | kod incelemesi |
| 3 | Form `MinimumSize` tanımlı | tasarımcı |
| 4 | Ayracı en sola/sağa sürükleme testi yapıldı | görsel kanıt |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` / dock-ataması yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
