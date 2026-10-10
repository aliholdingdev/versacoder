---
title: "Dapper ile Veri Kaynagi - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DevExpress kontrollerine Dapper ile veri kaynagi: IDataReader -> DataTable, Query/Execute, parametreli SQL ve secim tablosu"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Dapper ile Veri Kaynagi (v26.1)

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/error-handling-rules|hata kurallari]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005 ado.net dapper]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001 dual target]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Kontrolun DataSource'u hangi Dapper yontemiyle, hangi kisitlarla doldurulur* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Bu vault'ta **tek veri yolu** vardir: ADO.NET baglantisi + **Dapper** + **`IDataReader` -> `DataTable.Load`** (liste gerektiğinde `Query<T>`). EF Core, DbContext, migration **yok** (ADR-005). Her sorgu **parametreli** (`@p1`, anonim nesne), string-concat SQL **yasak**; `CommandTimeout` her cagrida acik set edilir. Kontrol yalniz `DataSource` alir - sorgu kontrolun icinde degil, **repository**'dedir. Surum yalniz v26.1, Ribbon stili yalniz `Office2010`.

## §1 Dapper yontem secimi

| Yontem | Ne dondurur | Ne zaman | Kontrol baglamasi |
|---|---|---|---|
| `ExecuteReader(sql, param)` | `IDataReader` | tablo/izgara (grid, tree, pivot, chart) | `DataTable.Load(reader)` |
| `Query<T>(sql, param)` | `IEnumerable<T>` | form / liste, tip guvenli | `List<T>` -> `DataSource` |
| `Execute(sql, param)` | etkilenen satir sayisi | `INSERT/UPDATE/DELETE` | sonra `RefreshDataSource()` |
| `ExecuteScalar<T>(sql, param)` | tek deger | `COUNT`, `SUM`, tek id | sayac / dogrulama |

```csharp
using (var conn = new SqlConnection(connString))
{
    var table = new DataTable();
    using (var rd = conn.ExecuteReader(sql, new { p1 = aktif, p2 = 100 }))
        table.Load(rd);
    gridControl1.DataSource = table;
}
```

> **KURAL:** `IDataReader` blok icinde kapanir (`using`); `Load` sonrasi reader **bittir** - ayni reader'i ikinci kez kullanma.

## §2 Kaynak secim tablosu

| Kaynak | Arti | Eksi | Hangi kontrol |
|---|---|---|---|
| `DataTable` | kolon adi = `FieldName`, hizli baglama | bellek kopyasi | `GridControl`, `TreeList`, `PivotGrid`, `ChartControl` |
| `List<T>` | tip guvenli, nullable net | `FieldName` = ozellik adi | formlar, `LookUpEdit` |
| `IDataReader` | en az kopya | tek gecisli, kontrole **dogrudan** verilemez | yalniz `DataTable.Load` icin |
| `BindingList<T>` | degisiklik izleme | az gerekir | satir ekleme/silme akislarinda |

`FieldName` <-> kolon/ozellik adi eslesmesi **buyuk/kucuk harf duyarsizdir**; eslesmeyen alan sessizce bos gorunur (ilk kontrol edilecek yer).

## §3 Parametre ve zaman asimligi

```csharp
conn.Execute("UPDATE dbo.Musteri SET Sehir = @p1, Guncelleme = GETDATE() WHERE Id = @p2",
             new { p1 = sehir, p2 = id }, commandTimeout: 30);
```

| Kural | Neden | Ihlal belirtisi |
|---|---|---|
| Parametre **hep** anonim nesne ile | SQL enjeksiyonu | `"..." + x` birlestirme |
| `commandTimeout` acik yaz | rapor sorgusu donmasin | 30 varsayilan, rapor 60 |
| `SELECT` yalniz gerekli kolonlar | bellek + paket | `SELECT *` |
| Baglanti `using` blogunda | havuz sizintisi | static acik baglanti |
| Yazma sonrasi tek `RefreshDataSource()` | N+1 yenileme | her satirda refresh |

## §4 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `using var` **yasak**, blok `using` zorunlu | C# 14 (paylasilan dosya 7.3 kalir) |
| SqlClient | `Microsoft.Data.SqlClient` 5.2.x + `App.config` binding redirect 26.1 | ayni paket, redirect gerekmez |
| `DataTable.Load` | `System.Data` (ayni API) | ayni |
| Dapper paketi | CPM'te tek `Version` satiri | ayni |
| Nullable | `#nullable disable` (proje geneli) | `#nullable` proje ayari (ayni dosya 7.3) |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | EF Core'a gecmek | mimari kirilimi (ADR-005) | ADO.NET + Dapper |
| 2 | Sorguyu kontrolun icine yazmak | test edilemez, katman kirilimi | repository metodu |
| 3 | String birlestirme ile SQL | enjeksiyon | anonim nesne `@p1` |
| 4 | `SELECT *` | gereksiz trafik | gerekli kolonlar |
| 5 | Her satirda `RefreshDataSource()` | yavas yenileme | tek refresh (DML sonrasi) |

## §6 Kaynak secim karari

| Soru | Cevap |
|---|---|
| Grid/Tree/Pivot/Chart mi? | `DataTable` (`ExecuteReader` + `Load`) |
| Form doldurma / tip guvenli liste mi? | `Query<T>` -> `List<T>` |
| Tek deger (sayac) mi? | `ExecuteScalar<T>` |
| Yazma (DML) mi? | `Execute` + tek `RefreshDataSource()` |
| EF Core / migration | **yok** - ADO.NET + Dapper tek yol |

## §7 Kodlandirma notu

- Her sorgu icin `CommandTimeout` acik verilir; varsayilanlara birakilmaz.
- Baglanti metni konfigürasyondan gelir, koda gomulmez.
- Kolon adlari tek bir yerde (sabit/sinif) tekrarlanir.
- DML sonrasi tek refresh; okuma ile yazma metotlari ayrilir.
- Hata yakalama katmaninda toplanir, UI'a ham SQL hatasi gitmez.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Grafik baglama | [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] |
| Kosullu bicim (veri degistirmez) | [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] |
| Dogrulama (DB kontrolu serviste) | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |
| Cok secim degerinin yazimi | [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `datasource-binding-dapper.md` | yontem tablosu, kaynak secim tablosu, parametre/zaman asimligi kurallari, iki bacak tablosu | dogrulandi |
| `chartcontrol-basics.md` | `DataTable` baglama zinciri | dogrulandi |
| ADR-005 | ADO.NET + Dapper, EF Core yok | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
