---
title: "ChartControl Temelleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "ChartControl temelleri: BindToData ile Dapper DataTable baglama, seri/eksen/etiket/gosterge ayarlari (v26.1)"
---

# ChartControl Temelleri (v26.1)

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [test kurallari](../../.rules/testing-rules.md) · [ADR-005 ado.net dapper](../../.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 5/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [dateedit-timeedit](dateedit-timeedit.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Kosullu bicim (grid) | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Dogrulama gostergesi | [dx-validation-provider](dx-validation-provider.md) |
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `chartcontrol-basics.md` | `BindToData` imzasi, `ArgumentDataMember` / `ValueDataMembers`, seri tipi tablosu, `TextPattern` ornegi, iki bacak | dogrulandi |
| `datasource-binding-dapper.md` | `DataTable` baglama yolu | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
