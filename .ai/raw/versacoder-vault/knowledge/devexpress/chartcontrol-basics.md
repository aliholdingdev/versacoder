---
title: "chartcontrol-basics - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "ChartControl temelleri: BindToData ile Dapper DataTable baglama, seri/eksen/etiket/Gosterge ayarlari (v26.1)"
---

# chartcontrol-basics — ChartControl temelleri (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [pivotgrid-basics](pivotgrid-basics.md)

> **Tek cumlelik sorumluluk:** *Dapper'dan gelen tablo bir grafige nasil baglanir, seri/eksen/etiket nerede ayarlanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grafik tek çağrıyla bağlanır: `BindToData(viewTipi, veriKaynagi, seriAlan, argumanAlan, degerAlanlar)` — veri kaynağı `DataTable` olabilir (chart `IListSource` kabul eder). Argüman (X) `ArgumentDataMember`, değer (Y) `ValueDataMembers` ile atanır; seri başına ayrı kaynak gerekirse `Series.DataSource` önceliklidir. Etiket ve gösterge **Designer** + `Series.LabelsVisibility` / `Label.TextPattern` ile düzenlenir; `CustomDrawSeriesPoint` ile elle çizimDX-08'e girer. Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Baglama (kod)

```csharp
var table = LoadAylikSatis();   // Dapper -> DataTable: Ay, Kategori, Tutar
chartControl1.DataSource = table;
chartControl1.BindToData(ViewType.Column, table, "Kategori", "Ay", "Tutar");
```

Argüman sütunu **kategorik/metin** de olabilir (`"Ay"` = `Oca`, `Şub`...), tarih de olabilir; eksen tipi veri tipine göre otomatik seçilir, gerektiğinde `XYDiagram.AxisX.DateTimeScaleOptions` ile düzenlenir (Designer).

## §2 Anahtar Ozellikler

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `chartControl1.DataSource` | tüm grafiğin veri kaynağı | `DataTable` |
| `BindToData(...)` | seri + alanları tek seferde bağlar | bkz. §1 |
| `chartControl1.Series` | elle oluşturulan seri koleksiyonu | Designer |
| `Series.ArgumentDataMember` | eksen (X) alan adı | `"Ay"` |
| `Series.ValueDataMembers` | değer (Y) alanları koleksiyonu | `"Tutar"` |
| `Series.LabelsVisibility` | nokta etiketleri | `DefaultBoolean.True` |
| `chartControl1.Legend` | gösterge kutusu | yerleşim Designer'dan |

| Seri tipi (ViewType) | Ne gosterir | Tipik alan |
|---|---|---|
| `Line` | zaman serisi eğilimi | `Ay` → `Tutar` |
| `Bar` / `StackedBar` | karşılaştırma / toplam kırılımı | `Sehir` → `Tutar` |
| `Pie` | pay dağılımı (tek seri) | `Kategori` → `Tutar` |
| `Area` | kümülatif hacim | `Ay` → `Adet` |
| `Scatter` | iki sayısal ilişkinin saçılımı | `X`, `Y` |

## §3 Etiket ve Gosterge

```csharp
chartControl1.Series[0].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
var lbl = chartControl1.Series[0].Label as BarSeriesLabel;
if (lbl != null) lbl.TextPattern = "{V:n2}";
```

| Ayar | Ne yapar | Deger ornegi |
|---|---|---|
| `LabelsVisibility` | etiket açık/kapalı | `DefaultBoolean.True` |
| `Label as BarSeriesLabel` | bar tipine özel etiket | `Position`, `BackColor` |
| `TextPattern` | etiket biçimi | `"{V:n2}"`, `"{A}: {VP}"` |
| `chartControl1.Legend` | gösterge | varsayılan sağ-alt |
| `Legend` gizleme (tek seri) | yer kazanır | Designer → `Visibility` |

> **UYARI:** `Label` nesnesi **seri tipine göredir** (`BarSeriesLabel`, `PieSeriesLabel`); tip uyuşmazsa `as` ile aldığın nesne `null` olur — §3'teki `if` bunun içindir.

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `as` + `if` zorunlu, `using var` **yasak** | C# 14 (paylaşılan dosya 7.3) |
| Chart paketi | `DevExpress.XtraCharts.v26.1` (+ `DevExpress.Charts`) | aynı (aynı CPM satırı) |
| `DataTable` | `System.Data` (aynı) | aynı |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| Yazı tipi/DPI | sabit `Font` yerine `DpiHelper`/varsayılan | aynı, 64-bit |

> **YASAK:** grafik verisini EF Core sorgusuyla doldurmak, `v26.1` dışı chart paketi ve elle `CustomDrawSeries` çizimi (G2 / DX-02 / DX-08).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Pivot ile karşılaştırma | [pivotgrid-basics](pivotgrid-basics.md) |
| Export (grafik dahil grid export'u) | [grid-export-excel-pdf](grid-export-excel-pdf.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
