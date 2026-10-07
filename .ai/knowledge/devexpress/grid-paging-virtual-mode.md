---
title: "grid-paging-virtual-mode - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "GridControl'da sayfalama (Dapper OFFSET/FETCH), sanal kolonlar ve buyuk veri icin BeginUpdate/RefreshDataSource"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Grid Sayfalama ve Sanal Mod

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [performance-grid-optimization](performance-grid-optimization.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 6/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *Grid'e sayfalama nasil getirilir ve buyuk veri neden istemciye tamamiyle yuklenmez* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**`GridControl`'un yerlesik sayfalama penceresi yoktur**; sayfalama veri katmaninda yapilir - Dapper `OFFSET/FETCH` ile bir sayfa cekilir, o sayfanin `DataTable`'i baglanir, baska sayfa istendiginde sorgu tekrar calisir. Sanal sütunlar (veri kaynaginda olmayan hesap) `CustomUnboundColumnData` ile istenildigi an uretilir; sanal **satir** icin DevExpress Server Mode bu vault'a girmez (EF/ORM yasaigi nedeniyle Dapper ile birlestirilmez).

```text
[<] [Sayfa 3/128] [>]  butonlari
        |
        v
  Dapper: OFFSET @p2 ROWS FETCH NEXT @p3 ROWS ONLY   (parametreli, G2)
        |
        v
  IDataReader -> DataTable.Load -> gridControl1.DataSource
        |
        v
  grid: PopulateColumns() yalniz 1. acilista + FocusedRowHandle = 0
```

> **KURAL:** `PopulateColumns()` **her sayfada cagrilmaz**; kolon semasi sabittir, yalniz `DataSource` degisir.

---

## §1 Sayfalama kodu

```csharp
const string sql = @"SELECT Id, Ad, Tutar FROM dbo.Fatura
                     WHERE Aktif = @p1
                     ORDER BY Id
                     OFFSET @p2 ROWS FETCH NEXT @p3 ROWS ONLY";
using (var conn = new SqlConnection(connString))
using (var rd = conn.ExecuteReader(sql, new { p1 = 1, p2 = (sayfa - 1) * boyut, p3 = boyut }))
{
    var t = new DataTable(); t.Load(rd);
    gridControl1.DataSource = t;
}
```

---

## §2 Sayfalama kurallari

| Detay | Kural | Neden |
|---|---|---|
| `@p2` / `@p3` tipi | **`int`** olmali (anonim tipte `int`) | string gelirse SQL hata verir |
| Sunucu surumu | SQL Server **2012+** (`OFFSET/FETCH`) | daha eskisi `ROW_NUMBER() OVER` ister; surum `DOĞRULAMA GEREKLI` |
| `ORDER BY` | zorunlu | `OFFSET` `ORDER BY` olmadan calismaz |
| Toplam sayfa | ayri `SELECT COUNT(*)` (ayni `WHERE`) | tek sayaci iki kez cekme |
| Ozet footer | DB toplami (`SUM`) ile goster | grid ozeti yalniz o sayfayi toplar |

---

## §3 Buyuk veri kontrolleri

| Ayar / yontem | Ne yapar | Deger |
|---|---|---|
| `gridControl1.BeginUpdate()` / `EndUpdate()` | yeniden cizmeyi toplu yap | toplu doldurma |
| `gridControl1.RefreshDataSource()` | veriyi tazele | kayit sonrasi |
| `gridView1.TopRowIndex` | gorunur ilk satiri koru | sayfa degisiminde |
| `gridView1.FocusedRowHandle` | odak satiri | `-2147483647` = grup satiri, `-1` = ekleme satiri |
| `SELECT` alanlari | yalniz gereken kolonlar | `SELECT *` **yasak** |
| `CommandTimeout` | Dapper'da acik set | kisa isler kisa, rapor icin uzun (kaynak: 30 sn / 60 sn - `DOĞRULAMA GEREKLI`) |

```csharp
gridControl1.BeginUpdate();
try { gridControl1.DataSource = sayfaYukle(no, 100); }
finally { gridControl1.EndUpdate(); }
```

Sanal sütun ayrintilari: [gridview-columns-format](gridview-columns-format.md) §3.

---

## §4 Kim neyi degistirir

| Degisiklik | Nereye dokunulur | Sonuc |
|---|---|---|
| Sayfa boyutu | yukleme metodundaki `boyut` parametresi | cekilen satir sayisi |
| Sorgu kriteri | Dapper anonim parametre (`@p1`) | filtre DB'de uygulanir |
| Toplam sayfa sayisi | ayri `COUNT(*)` sorgusu | navigasyon dogrulugu |
| Sanal sütun | `CustomUnboundColumnData` + `UnboundType` | veri kaynagina kolon eklemez |
| Yenileme | `RefreshDataSource()` (+ `BeginUpdate/EndUpdate`) | cizim maliyeti |

---

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `using var` **yasak**, `try/finally` bloku zorunlu | C# 14 (paylasilan dosya 7.3) |
| `DataTable.Load` | `System.Data` (ayni) | ayni |
| SqlClient | `Microsoft.Data.SqlClient` 5.2.x + binding redirect | ayni paket, redirect yok |
| Bellek | 32-bit surec riski (DevExpress + buyuk `DataTable`) | 64-bit varsayilan |
| DevExpress | 26.1 | 26.1 (ayni CPM satiri) |

> **YASAK:** tum tabloyu cekip istemcide suremek (G2 performans anti-pattern'i) ve `SELECT *`.

---

## §6 Tarama tablosu

| Aranan | Istenen | Yorum |
|---|---|---|
| `PopulateColumns()` | dosyada 1 kez | her sayfada degil |
| `SELECT *` | 0 eslesme | yalniz gereken kolonlar |
| `OFFSET` + `ORDER BY` | birlikte | ikisiz sayfalama calismaz |
| `BeginUpdate` / `EndUpdate` | eslesmeli | toplu doldurmada zorunlu |
| Tum tablo + `WHERE` istemcide | yok | sureme DB'ye ait (G2) |

---

## §7 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Gruplama + ozet | [grid-grouping-summaries](grid-grouping-summaries.md) |
| Filtre + siralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Sanal sütun bicimi | [gridview-columns-format](gridview-columns-format.md) |
| Performans olcumu | [performance-grid-optimization](performance-grid-optimization.md) |

---

## §8 Denetim listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Sayfalama kaynaginda, grid'de degil | mimari inceleme |
| 2 | Parametreler `int`, `ORDER BY` mevcut | SQL incelemesi |
| 3 | Kolon semasi sabit (tek `PopulateColumns`) | kod grep |
| 4 | Footer toplami DB `SUM` ile | ekran + sorgu |
| 5 | `SELECT *` ve istemci sureme yok | kod taramasi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\grid-paging-virtual-mode.md` (kaynak vault) | akis diyagrami, OFFSET/FETCH kurallari, buyuk veri tablosu, TFM farki |
| `.rules\data-access-rules.md` | parametreli sorgu (G2), `SELECT *` / sureme yasagi |
| `.rules\performance-guidelines.md` | istemci tarafi sureme anti-pattern'i |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
