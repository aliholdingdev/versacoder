---
title: "Layout: TableLayoutPanel - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms duzen kalip: TableLayoutPanel/FlowLayoutPanel, Dock-Anchor hiyerarsisi, AutoSize ve olcu; DPI altinda davranis; net48 vs net10.0-windows farklari"
---

# Layout: `TableLayoutPanel` ve Dock/Anchor Kalip

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md) · [form-lifecycle](form-lifecycle.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Pencere boyutunca sikisan kontroller hangi layout motoru ile duzelir, `TableLayoutPanel` ne zaman, `Dock/Anchor` ne zaman kullanilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

WinForms'ta tek dogru duzen motoru **parent'in child'lari konumlandirma hiyerarsisidir**: `TableLayoutPanel` (satir/sutun, yuzde + mutlak), `FlowLayoutPanel` (etiket+girdi sirasi), `Dock` (kenara yapisan panel), `Anchor` (iki kose arasina gerilen control). **Once `Dock`, sonra `Anchor`, en son elle `Location`** - `Location` elle ayarlaniyorsa layout tasarimi kiriktir. `AutoSize = true` + `DockStyle.Fill` birlikte `PerformLayout` dongusu olusturabilir; tablo hucresinde `AutoSize` yalnizca bir eksende birakilir. DPI artisinda kontrol boyutlari pixel uzerinden degil, `AutoScaleMode.Dpi`/`Font` ile olculur (bkz. [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md)).

---

## §1 Hangi layout ne icin

| Kalip | Ne yapar | Tipik yer | Risk |
|---|---|---|---|
| `TableLayoutPanel` | satir/sutun; `Percent` + `Absolute` + `AutoSize` | form govdesi, ayar sayfalari | `Percent` + `AutoSize` ayni hucrede -> olcum dongusu |
| `FlowLayoutPanel` | sigmayan satirda yenisine gecer | etiket + girdi listesi, toolbar | `FlowDirection` tersse okuma sirasi bozulur |
| `Dock` | kenara yapistirir (`Top/Bottom/Left/Right/Fill`) | ust baslik, alt durum, yan panel | `Dock` sirasi **z-index** gibidir, `Controls.Add` sirasi onemli |
| `Anchor` | iki kose arasina gerer | tekil kontrol, buton grubu | 4 kose de anchor -> `Dock`'a benzeyen davranis |
| `SplitContainer` | iki panel + suruklenebilir ayirac | ana pencere (cozunur yan panel) | `FixedPanel` verilmezse yeniden boyutlandirmada ziplama |

**Sira kurali (tek satir):** `Dock` -> `Anchor` -> `AutoSize` -> en son `Location` (sadece layout disi serbest kalip).

---

## §2 `TableLayoutPanel` ornegi

```csharp
private void BuildLayout(TableLayoutPanel grid)
{
    grid.ColumnCount = 2;
    grid.RowCount = 3;
    grid.Dock = DockStyle.Fill;
    grid.AutoSize = true;

    grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
    grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
    grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

    grid.Controls.Add(nameLabel, 0, 0);
    grid.Controls.Add(nameBox,   1, 0);
    grid.Controls.Add(infoBox,   0, 2);
    grid.SetColumnSpan(infoBox, 2);
}
```

| Ayar | Anlam | Ne zaman |
|---|---|---|
| `SizeType.Absolute` | sabit piksel | etiket kolonu (140F gibi) |
| `SizeType.Percent` | kalan bosluga paylasir | doluluk sutunu |
| `SizeType.AutoSize` | icerik kadar | satir yuksekligi |
| `SetColumnSpan` | hucreyi genislet | mesaj alani, grid |

> **KURAL:** `Percent` degerleri toplami 100'den buyukse hedef payin disina tasar; `Absolute` piksel degeri **DPI'da buyur** - `HighDpiAwareness` acikken `AutoScaleMode.Dpi` ile tasarla, sabit piksel yazma (bkz. [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md)).

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Layout motoru (`LayoutEngine`) | ayni (`DefaultLayout`) | ayni - **davranis farki yok** |
| `TableLayoutPanel`/`FlowLayoutPanel` | var | var |
| DPI varsayilani | uygulama manifest / app.config ile `DpiAwareness` | MSBuild `<ApplicationHighDpiMode>` (SystemAware/PerMonitorV2) |
| `AutoScaleMode` | `Dpi` / `Font` (designer `Dpi` yazar) | ayni; `ApplicationConfiguration.Initialize()` DPI modunu onceden kurar |
| Tasarim pikseli | 96 DPI reference | ayni reference, fark uygulama DPI modunda |

> **Sonuc:** layout kodu **iki bacakta da aynidir**; tek fark formun **hangi DPI modunda** acildigidir. `#if` gerekmez - farki yoneten `ApplicationConfiguration.Initialize()` (net10) karsiligi `EnableVisualStyles` + app.config (net48) zinciridir.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
List<Control> items = new List<Control>();
string[] labels = new string[] { "Ad", "Soyad" };

for (int i = 0; i < labels.Length; i++)
{
    var lbl = new Label();
    lbl.Text = labels[i];
    lbl.AutoSize = true;
    items.Add(lbl);
}

foreach (Control c in items)
{
    flow.Controls.Add(c);
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
List<Control> items = [];
string[] labels = ["Ad", "Soyad"];

for (int i = 0; i < labels.Length; i++)
{
    Label lbl = new() { Text = labels[i], AutoSize = true };
    items.Add(lbl);
}

foreach (Control c in items)
{
    flow.Controls.Add(c);
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| Coleksiyon baslatma | `new List<Control>()` | `[]` | 7.3'te `new` zorunlu |
| Nesne baslatma | `new Label()` + satir satir | `new() { ... }` | object initializer 7.3'te de var |
| Tip yazma | her satirda | `var` / hedef tip | ortak dosyada **tip acik** yaz (CS-01) |
| `switch` ifadesi | statement | expression | ortak kodda statement (8.0+ ozelligi) |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `Location` ile elle konum | pencere boyutunda kontrol tasmasi | `Dock`/`Anchor`/`TableLayoutPanel` |
| `AutoSize` + `DockStyle.Fill` | LayoutException / sonsuz olcum | hucresinde yalnizca biri |
| `Dock` sirasi yanlis | panel birbirini yiyor | `Controls.Add` sirasini Top -> Fill seklinde yaz |
| Sabit piksel ile tasarim | 150% DPI'da kirik ekran | `AutoScaleMode.Dpi` + yuzde/autosize |
| Elle `PerformLayout()` cagrisi | gereksiz olcum dongusu | layout motoruna birak |

> **Iliskili:** DPI ayari icin [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md) · kapatma/odak sirasi icin [form-lifecycle](form-lifecycle.md) · olay baglantilari icin [controls-lifecycle-events](controls-lifecycle-events.md).
