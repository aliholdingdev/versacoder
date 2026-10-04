---
title: "splitcontainer-and-sizing - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "SplitContainer kullanim kosullarini, panel sirasinin dock yapisina etkisini ve MinimumSize/SplitterDistance tuzaklarini tanimlar"
---

# SplitContainer, Panel Sirasi ve Boyut Kisitlari

**Zorunlu Baglantilar:** [layout-table-panel](layout-table-panel.md) · [docking-panels-patterns](docking-panels-patterns.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [ui-rules](../../.rules/ui-rules.md) · [winforms-designer-check](../../checklists/winforms-designer-check.md)

> **Tek cumlelik sorumluluk:** *SplitContainer ne zaman kullanilir, SplitterDistance ne zaman istasna verir ve panel sirasi dock yapisini nasil belirler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`SplitContainer` **iki** paneli birlikte yonetir (`Panel1` + `Panel2`) ve **yalnizca dock'lanmis** olarak kullanilir; `Anchor` ile yerlestirilirse beklenmedik boyut davranisi cikar. `SplitterDistance`, `Panel1MinSize`, `Panel2MinSize` uclusu **birlikte** ayarlanir - aksi halde `ArgumentException` (splitter mesafesi gecersiz) firlar. Panel **sirasi** `.Designer.cs`'teki `Controls.Add` sirasiyla belirlenir ve dock yapisini dogrudan etkiler.

```text
Form
 +-- RibbonControl        (Dock = Top)      -> ilk eklenir
 +-- StatusBarControl     (Dock = Bottom)    -> sonra
 +-- SplitContainer       (Dock = Fill)      -> en son
```

---

## §1 Ozellikler ve sinirsiz davranis

| # | Ozellik | Ne ise yarar | Sinirsiz davranis |
|---|---|---|---|
| 1 | `Orientation` | dikey/yatay bolme | `Vertical` (varsayilan) |
| 2 | `SplitterDistance` | ayiracin konumu | `Panel1` yuksekligine esitlenir |
| 3 | `Panel1MinSize` | sol panelin minimumu | 0 -> kapabilir |
| 4 | `Panel2MinSize` | sag panelin minimumu | asilirsa istisna |
| 5 | `FixedPanel` | yeniden boyutlanirken sabitle | `None` |

Dogru yerlesim (`.Designer.cs` icinde, `.cs`'e **tasinmaz**):

```csharp
this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
this.splitContainer1.Panel1MinSize = 220;
this.splitContainer1.Panel2MinSize = 360;
this.splitContainer1.SplitterDistance = 260;
```

> **KURAL:** `SplitterDistance` atamasi, `Panel1MinSize`/`Panel2MinSize` atamalarindan **sonra** yapilir; `ClientSize` hesaplanmamisken verilen deger forma uymazsa istisna firlar.

---

## §2 Ortak hatalar

| # | Hata | Belirti | Duzeltme |
|---|---|---|---|
| 1 | `SplitContainer` `Anchor` ile konumlanmis | ayiracin davranisi tutarsiz | `Dock = Fill` |
| 2 | `SplitterDistance` < `Panel1MinSize` | `ArgumentException` | degerleri birlikte ayarla |
| 3 | iki `SplitContainer` ic ice (ayni formda 3+ kat) | performans + olcu hatasi | `TableLayoutPanel` + tek split |
| 4 | `Panel2`'ye dock'lu kontrol yanlis sirayla eklendi | kontrol tasar | `Controls.Add` sirasini `.Designer.cs`'te duzelt |
| 5 | min boyut `MinimumSize` ile verilmemis | pencere kucultunce kirilir | forma `MinimumSize` ver |

---

## §3 Dock sirasi kurali

| # | Sira kurali | Neden |
|---|---|---|
| 1 | `Fill` **en son** eklenir | dolu alana yer kalmaz |
| 2 | `Top`/`Bottom` sirayla | ilk eklenen disa dogru ylesir |
| 3 | `Left`/`Right` ardindan | yiginsa siralama belirleyicidir |
| 4 | sira `.Designer.cs`'te degisir | `.cs`'e tasinirsa tasarimci coker |

Alternatif: kucuk, sabit hizali form alanlari (ayar satirlari) icin `SplitContainer` yerine `TableLayoutPanel` kullanilir; satir/sutun `SizeType.Percent` oldugunda DPI'da cok daha kararlidir (bkz. [layout-table-panel](layout-table-panel.md)).

---

## §4 Iki bacak farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `DockStyle` / `SplitContainer` | var | var - **ayni LayoutEngine** |
| panel durumu kaydi | `ConfigurationManager` (yerlesik) | `System.Configuration.ConfigurationManager` **paket** gerekir |
| `user.config` yolu | `%LOCALAPPDATA%\<sirket>\<uyg>` | ayni sablon, surum klasoru |
| DPI | panel genisligi sabit piksel -> DPI'da daralir | `ApplicationHighDpiMode` + `AutoScaleMode.Dpi` |

> **Not:** `ConfigurationManager` `net48`'de framework icinde, `net10.0-windows`'ta ayri NuGet paketidir - iki bacakta da ayni `<PackageReference>` kullanilir; `#if` **gerekmez**, yalnizca `.csproj` farkidir (bkz. [settings-user-config](settings-user-config.md)).

---

## §5 Kim neyi degistirir + kontrol listesi

| # | Kim | Neyi degistirir | Kanit |
|---|---|---|---|
| 1 | `ui-designer` | `SplitContainer` degerleri, `Controls.Add` sirasi | `.Designer.cs` diff'i |
| 2 | `developer` | forma `MinimumSize`, icerik paneli icerigi | kod incelemesi |
| 3 | `developer` | `FixedPanel` verilmezse ziplama bulgusu | gorsel kanit |
| 4 | herkes | `SplitterDistance` >= `Panel1MinSize` ve <= `ClientSize - Panel2MinSize` | kod incelemesi |
| 5 | tester | ayraci en sola/saga surukleme testi | gorsel kanit; iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\splitcontainer-and-sizing.md` (kaynak vault) | ozellik tablosu, yerlesim kodu, ortak hata tablosu, dock sirasi, TFM farklari, kontrol listesi |
| `.rules\ui-rules.md` | ekran/duzen kurali |
| `checklists\winforms-designer-check.md` | denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
