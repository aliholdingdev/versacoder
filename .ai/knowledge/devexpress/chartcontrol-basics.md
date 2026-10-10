---
title: "ChartControl Temelleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "ChartControl temelleri: BindToData ile Dapper DataTable baglama, seri/eksen/etiket/gosterge ayarlari (v26.1)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ChartControl Temelleri (v26.1)

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/testing-rules|test kurallari]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005 ado.net dapper]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Dapper'dan gelen tablo bir grafige nasil baglanir, seri/eksen/etiket nerede ayarlanir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Grafik tek cagriyla baglanir: `BindToData(viewTipi, veriKaynagi, seriAlan, argumanAlan, degerAlanlar)` - veri kaynagi `DataTable` olabilir (chart `IListSource` kabul eder). Arguman (X) `ArgumentDataMember`, deger (Y) `ValueDataMembers` ile atanir; seri basina ayri kaynak gerekirse `Series.DataSource` onceliklidir. Etiket ve gosterge **Designer** + `Series.LabelsVisibility` / `Label.TextPattern` ile duzenlenir; `CustomDrawSeriesPoint` ile elle cizim elle cizim kuralinin disina duser. Surum **yalniz v26.1**, Ribbon stili **yalniz** `Office2010`.

## §1 Baglama (kod)

```csharp
var table = LoadAylikSatis();   // Dapper -> DataTable: Ay, Kategori, Tutar
chartControl1.DataSource = table;
chartControl1.BindToData(ViewType.Column, table, "Kategori", "Ay", "Tutar");
```

```
Dapper (ExecuteReader) -> DataTable
                             |
                             v
                    chartControl1.DataSource
                             |
                      BindToData(ViewType, table,
                                 seri, arguman, deger)
                             |
              +--------------+--------------+
              v                             v
      Series.ArgumentDataMember      Series.ValueDataMembers
              |                             |
              +----> eksen (X) ---- deger (Y)
```

Arguman sutunu **kategorik/metin** de olabilir (`"Ay"` = `Oca`, `Sub`...), tarih de olabilir; eksen tipi veri tipine gore otomatik secilir, gerekirse `XYDiagram.AxisX.DateTimeScaleOptions` ile duzenlenir (Designer).

## §2 Anahtar ozellikler

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `chartControl1.DataSource` | tum grafigin veri kaynagi | `DataTable` |
| `BindToData(...)` | seri + alanlari tek seferde baglar | bkz. §1 |
| `chartControl1.Series` | elle olusturulan seri koleksiyonu | Designer |
| `Series.ArgumentDataMember` | eksen (X) alan adi | `"Ay"` |
| `Series.ValueDataMembers` | deger (Y) alanlari | `"Tutar"` |
| `Series.LabelsVisibility` | nokta etiketleri | `DefaultBoolean.True` |
| `chartControl1.Legend` | gosterge kutusu | yerlesim Designer'dan |

## §3 Seri tipi secimi

| Seri tipi (ViewType) | Ne gosterir | Tipik alan |
|---|---|---|
| `Line` | zaman serisi egilimi | `Ay` -> `Tutar` |
| `Bar` / `StackedBar` | karsilastirma / toplam kirilimi | `Sehir` -> `Tutar` |
| `Pie` | pay dagilimi (tek seri) | `Kategori` -> `Tutar` |
| `Area` | kumulatif hacim | `Ay` -> `Adet` |
| `Scatter` | iki sayisalis iliskinin sacilimi | `X`, `Y` |

## §4 Etiket ve gosterge

```csharp
chartControl1.Series[0].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
var lbl = chartControl1.Series[0].Label as BarSeriesLabel;
if (lbl != null) lbl.TextPattern = "{V:n2}";
```

| Ayar | Ne yapar | Deger ornegi |
|---|---|---|
| `LabelsVisibility` | etiket acik/kapali | `DefaultBoolean.True` |
| `Label as BarSeriesLabel` | bar tipine ozel etiket | `Position`, `BackColor` |
| `TextPattern` | etiket bicimi | `"{V:n2}"`, `"{A}: {VP}"` |
| `chartControl1.Legend` | gosterge | varsayilan sag-alt |
| `Legend` gizleme (tek seri) | yer kazanir | Designer -> `Visibility` |

> **UYARI:** `Label` nesnesi **seri tipine goredir** (`BarSeriesLabel`, `PieSeriesLabel`); tip uyuşmazsa `as` ile aldigin nesne `null` olur - §4'teki `if` bunun icindir.

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `as` + `if` zorunlu, `using var` **yasak** | C# 14 (paylasilan dosya 7.3) |
| Chart paketi | `DevExpress.XtraCharts.v26.1` (+ `DevExpress.Charts`) | ayni (ayni CPM satiri) |
| `DataTable` | `System.Data` (ayni API) | ayni |
| Binding redirect | `App.config` redirect 26.1 | redirect gerekmez |
| DPI | sabit `Font` yerine varsayilan/DPI yardimcisi | ayni, 64-bit |

## §6 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Veriyi EF Core sorgusuyla doldurmak | mimari kirilimi | ADO.NET + Dapper (ADR-005) |
| 2 | `BindToData` alan adini yanlis yazmak | bos grafik | `FieldName` birebir ayni |
| 3 | Her cizimde `Series` yeniden olusturmak | titreme, bellek | tek kurulum, sonra veri |
| 4 | `Label` i tip guvenmeden cast etmek | `NullReferenceException` | `as` + `if (lbl != null)` |
| 5 | Elle `CustomDrawSeriesPoint` ile tema kirmak | DPI/tema altinda bozulur | kural/etiket API'leri |

## §7 Dogrulama ve sinir

| Katman | Gorev | Not |
|---|---|---|
| Repository | `ExecuteReader` -> `DataTable` | parametreli sorgu, `@p1` |
| Binding | `DataSource` / `BindToData` | kontrol sorgu bilmez |
| Gorunum | `FormatRules` benzeri kural degil, seri ayari | bicim kurali gridde |
| Yasak | EF Core, `v26.1` disi paket, elle cizim | Universal 2026 / v26.1 tek surum |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Veri kaynagi (Dapper) | [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] |
| Kosullu bicim (grid) | [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] |
| Dogrulama gostergesi | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `chartcontrol-basics.md` | `BindToData` imzasi, `ArgumentDataMember` / `ValueDataMembers`, seri tipi tablosu, `TextPattern` ornegi, iki bacak | dogrulandi |
| `datasource-binding-dapper.md` | `DataTable` baglama yolu | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
