---
title: "datasource-binding-dapper - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress kontrollerine Dapper ile veri kaynagi: IDataReader -> DataTable, Query/Execute, parametreli SQL ve secim tablosu (net48 + net10.0-windows)"
---

# datasource-binding-dapper — Dapper ile veri kaynagi (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md)

> **Tek cumlelik sorumluluk:** *Kontrolun DataSource'u hangi Dapper yontemiyle, hangi kisitlarla doldurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault'ta **tek veri yolu** vardır: ADO.NET bağlantısı + **Dapper** + **`IDataReader` → `DataTable.Load`** (liste gerektiğinde `Query<T>`). EF Core, DbContext, migration **yok** (G2). Her sorgu **parametreli** (`@p1`, anonim nesne), string-concat SQL **yasak**; `CommandTimeout` her çağrıda açık set edilir. Kontrol yalnız `DataSource` alır — sorgu kontrolün içinde değil, **repository**'dedir. Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Dapper Yontem Secimi

| Yontem | Ne dondurur | Ne zaman | Kontrol baglamasi |
|---|---|---|---|
| `ExecuteReader(sql, param)` | `IDataReader` | tablo/ızgara (grid, tree, pivot, chart) | `DataTable.Load(reader)` |
| `Query<T>(sql, param)` | `IEnumerable<T>` | form / liste, tip güvenli | `List<T>` → `DataSource` |
| `Execute(sql, param)` | etkilenen satır sayısı | `INSERT/UPDATE/DELETE` | sonra `RefreshDataSource()` |
| `ExecuteScalar<T>(sql, param)` | tek değer | `COUNT`, `SUM`, tek id | sayaç / doğrulama |

```csharp
using (var conn = new SqlConnection(connString))
{
    var table = new DataTable();
    using (var rd = conn.ExecuteReader(sql, new { p1 = aktif, p2 = 100 }))
        table.Load(rd);
    gridControl1.DataSource = table;
}
```

> **KURAL:** `IDataReader` blok içinde kapanır (`using`); `Load` sonrası reader **bittir** — aynı reader'ı ikinci kez kullanma.

## §2 Kaynak Secim Tablosu

| Kaynak | Arti | Eksi | Hangi kontrol |
|---|---|---|---|
| `DataTable` | kolon adı = `FieldName`, hızlı bağlama | bellek kopyası | `GridControl`, `TreeList`, `PivotGrid`, `ChartControl` |
| `List<T>` | tip güvenli, nullable net | `FieldName` = özellik adı | formlar, `LookUpEdit` |
| `IDataReader` | en az kopya | tek geçişli, kontrole **doğrudan** verilemez | yalnız `DataTable.Load` için |
| `BindingList<T>` | değişiklik izleme | az gerekir | satır ekleme/silme akışlarında |

`FieldName` ↔ kolon/özellik adı eşleşmesi **büyük/küçük harf duyarsızdır**; eşleşmeyen alan sessizce boş görünür (ilk kontrol edilecek yer).

## §3 Parametre ve Zaman Asimligi

```csharp
conn.Execute("UPDATE dbo.Musteri SET Sehir = @p1, Guncelleme = GETDATE() WHERE Id = @p2",
             new { p1 = sehir, p2 = id }, commandTimeout: 30);
```

| Kural | Neden | Ihlal belirtisi |
|---|---|---|
| Parametre **hep** anonim nesne ile | SQL injection (G2) | `"..." + x` birleştirme |
| `commandTimeout` açık yaz | rapor sorgusu donmasın | 30 varsayılan, rapor 60 |
| `SELECT` yalnız gerekli kolonlar | bellek + paket | `SELECT *` |
| Bağlantı `using` bloğunda | havuz sızıntısı | static açık bağlantı |
| Yazma sonrası tek `RefreshDataSource()` | N+1 yenileme | her satırda refresh |

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak**, blok `using` zorunlu | C# 14 (paylaşılan dosya 7.3 kalır) |
| SqlClient | `Microsoft.Data.SqlClient` 5.2.x + `App.config` binding redirect 26.1 | aynı paket, redirect gerekmez |
| `DataTable.Load` | `System.Data` (aynı API) | aynı |
| Dapper paketi | CPM'te tek `Version` satırı | aynı |
| Nullable | `#nullable disable` (proje geneli) | `#nullable` proje ayarı (aynı dosya 7.3'te kalır) |

> **YASAK:** EF Core / LINQ-to-Entities / `Include` / migration (G2) ve `v26.1` dışı DevExpress paketi (DX-02).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
| Sayfalama + `OFFSET/FETCH` | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| lookUpEdit kademeli yükleme | [lookUpEdit-cascading](lookUpEdit-cascading.md) |
| TreeList yükleme | [treelist-hierarchical-data](treelist-hierarchical-data.md) |
