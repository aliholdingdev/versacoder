---
title: "grid-paging-virtual-mode - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridControl'da sayfalama (Dapper OFFSET/FETCH), sanal kolonlar ve buyuk veri icin BeginUpdate/RefreshDataSource (v26.1)"
---

# grid-paging-virtual-mode — Sayfalama ve sanal mod (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [datasource-binding-dapper](datasource-binding-dapper.md)

> **Tek cumlelik sorumluluk:** *Grid'e sayfalama nasil getirilir ve buyuk veri neden istemciye tamamiyle yuklenmez* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**`GridControl`'ün yerleşik sayfalama penceresi yoktur**; sayfalama veri katmanında yapılır — Dapper `OFFSET/FETCH` ile bir sayfa çekilir, o sayfanın `DataTable`'ı bağlanır, diğer sayfa istendiğinde sorgu tekrar çalışır. Sanal sütunlar (veri kaynağında olmayan hesap) `CustomUnboundColumnData` ile istenilen an üretilir; sanal **satır** için DevExpress Server Mode bu vault'a girmez (EF/ORM yasağı nedeniyle Dapper ile birleştirilmez, `NEEDS_VALIDATION`). Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Sayfalama Akisi (diyagram)

```
[<] [Sayfa 3/128] [>]  butonlari
        |
        v
  Dapper: OFFSET @p2 ROWS FETCH NEXT @p3 ROWS ONLY   (parametreli, G2)
        |
        v
  IDataReader -> DataTable.Load -> gridControl1.DataSource
        |
        v
  grid: PopulateColumns() (yalniz 1. acilista) + FocusedRowHandle = 0
```

> **KURAL:** `PopulateColumns()` **her sayfada çağrılmaz**; kolon şeması sabittir, yalnız `DataSource` değişir.

## §2 Sayfalama Kodu

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

| Detay | Kural | Neden |
|---|---|---|
| `@p2` / `@p3` tipi | **`int`** olmalı (anonim tipte `int`) | string gelirse SQL hata verir |
| Sürüm | SQL Server **2012+** (`OFFSET/FETCH`) | daha eskisi `ROW_NUMBER() OVER` ister (`NEEDS_VALIDATION`: sunucu sürümü) |
| `ORDER BY` | zorunlu | `OFFSET` `ORDER BY` olmadan çalışmaz |
| Toplam sayfa | ayrı `SELECT COUNT(*)` (aynı `WHERE`) | tek sayacı iki kez çekme |
| Özet footer | DB toplamı (`SUM`) ile göster | grid özeti yalnız o sayfayı toplar |

## §3 Buyuk Veri Kontrolleri

| Ayar / yontem | Ne yapar | Deger |
|---|---|---|
| `gridControl1.BeginUpdate()` / `EndUpdate()` | yeniden çizmeyi toplu yap | toplu doldurma |
| `gridControl1.RefreshDataSource()` | veriyi tazele | kayıt sonrası |
| `gridView1.TopRowIndex` | görünür ilk satırı koru | sayfa değişiminde |
| `gridView1.FocusedRowHandle` | odak satırı | `-2147483647` = grup satırı, `-1` = ekleme satırı |
| `SELECT` alanları | yalnız gereken kolonlar | `SELECT *` **yasak** |
| `CommandTimeout` | Dapper'da açık set | 30 sn varsayılan, raporda 60 |

```csharp
gridControl1.BeginUpdate();
try { gridControl1.DataSource = sayfaYukle(no, 100); }
finally { gridControl1.EndUpdate(); }
```

Sanal sütun ayrıntıları: [gridview-columns-format](gridview-columns-format.md) §3.

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak**, `try/finally` bloğu zorunlu | C# 14 (paylaşılan dosya 7.3) |
| `DataTable.Load` | `System.Data` (aynı) | aynı |
| SqlClient | `Microsoft.Data.SqlClient` 5.2.x + binding redirect | aynı paket, redirect yok |
| Bellek | 32-bit süreç riski (DevExpress + büyük `DataTable`) | 64-bit varsayılan |
| DevExpress | 26.1 | 26.1 (aynı CPM satırı) |

> **YASAK:** tüm tabloyu çekip istemcide süzmek (G2 performans anti-pattern'i) ve `SELECT *`.

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Gruplama + özet | [grid-grouping-summaries](grid-grouping-summaries.md) |
| Filtre + sıralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Export | [grid-export-excel-pdf](grid-export-excel-pdf.md) |
