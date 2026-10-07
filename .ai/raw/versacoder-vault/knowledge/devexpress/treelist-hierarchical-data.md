---
title: "treelist-hierarchical-data - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "TreeList ile kendi kendine referansli (Id/UstId) hiyerarsik veri baglama - Dapper DataTable, KeyFieldName/ParentFieldName (v26.1)"
---

# treelist-hierarchical-data — Hiyerarsik veri baglama (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [treelist-inplace-editing](treelist-inplace-editing.md) · [datasource-binding-dapper](datasource-binding-dapper.md)

> **Tek cumlelik sorumluluk:** *Duz tablo (Id/UstId) TreeList'te agaca nasil donusturulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`TreeList`, düz bir tabloyu **`KeyFieldName` + `ParentFieldName`** ile otomatik ağaca çevirir; ayrı hiyerarşi nesnesi gerekmez. Veri kaynağı yine **Dapper → `DataTable`** yoludur (`SELECT Id, UstId, Ad FROM ...`), EF Core yok. `DataSource` atandıktan sonra `PopulateColumns()` çağrılır, sonra `ExpandAll()` ile açılır. Bu kontrol vault'ta **Solution Explorer ekranının** ana kontrolüdür (DX-09). Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Veri Modeli (diyagram)

```
Tablo (duz)                      TreeList (agac)
+----+-------+---------+        ┌ Kategori (Id=1, UstId=0)
| Id | UstId | Ad      |        ├── ├─ Elektronik (Id=2, UstId=1)
+----+-------+---------+        │   ├── Telefon (Id=5, UstId=2)
| 1  | 0     | Kategori|        │   └── Bilgisayar (Id=6, UstId=2)
| 2  | 1     | Elektronik       │   └─ Ev (Id=3, UstId=1)
| 5  | 2     | Telefon  |        └── Aksesuar (Id=4, UstId=1)
+----+-------+---------+
```

> **KURAL:** kök satırlarda `UstId` `0` **ya da** `NULL` (tablona göre); ikisi karıştırılırsa ağaç boş görünür ve hata da vermez.

## §2 Kurulum (kod)

```csharp
treeList1.KeyFieldName   = "Id";
treeList1.ParentFieldName = "UstId";
treeList1.DataSource = LoadKategoriler();   // Dapper -> DataTable
treeList1.PopulateColumns();
treeList1.ExpandAll();
```

Yükleme (parametreli sorgu, string-concat **yok** — G2):

```csharp
using (var conn = new SqlConnection(connString))
{
    var t = new DataTable();
    using (var rd = conn.ExecuteReader("SELECT Id, UstId, Ad FROM dbo.Kategori WHERE Aktif = @p1",
                                       new { p1 = 1 }))
        t.Load(rd);
    return t;
}
```

## §3 Ozellikler

| Ozellik / yontem | Ne yapar | Ornek deger |
|---|---|---|
| `KeyFieldName` | benzersiz satır anahtarı | `"Id"` |
| `ParentFieldName` | üst satır anahtarı | `"UstId"` |
| `PopulateColumns()` | kolon üretimi (`DataSource` **sonrası**) | çağrı |
| `treeList1.ExpandAll()` / `CollapseAll()` | tümünü aç / kapat | açılışta `ExpandAll` |
| `treeList1.FocusedNode` | odak düğüm | gezinme |
| `treeList1.FindNodeByKeyID(id)` | anahtardan düğüm bul | `Id` ile odaklama |
| `treeList1.OptionsView.ShowCheckBoxes` | onay kutuları (toplu seçim) | `true` |
| `treeList1.OptionsBehavior.AllowIncrementalSearch` | yazarak arama | `true` |

| Alani ayarlama | Ornek |
|---|---|
| `treeList1.Columns["Ad"].Caption = "Kategori";` | başlık (Designer tercih edilir, DX-10) |
| `treeList1.Columns["Ad"].VisibleIndex = 0;` | ilk sütun |
| `treeList1.BestFitColumns();` | genişlik uyumu |

## §4 Ağaç Küçükse mi, Büyükse mi?

| Durum | Yaklasim | Neden |
|---|---|---|
| ≤ ~2.000 düğüm | tümünü tek sorguda çek | basit, anında |
| > ~2.000 düğüm | düğüm açılırken çocukları çek (lazy) | `SELECT` + `WHERE UstId = @p1` |
| Ağaç salt okunur | `OptionsBehavior.Editable = false` | bkz. [treelist-inplace-editing](treelist-inplace-editing.md) |
| Arama gerekirse | `AllowIncrementalSearch` | ek panel gerekmez |

> **NEEDS_VALIDATION:** lazy yükleme için v26.1'de kullanılacak olay/metot adı Designer olay listesinden doğrulanacak (açılış-zinciri `BeforeExpand` varsayımı kanıtlanmadı).

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak**, blok `using` | C# 14 (paylaşılan dosya 7.3) |
| TreeList paketi | `DevExpress.Win.TreeList` / `DevExpress.Win.Navigation` 26.1 | aynı (aynı CPM satırı) |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| `DataTable` | `System.Data` (aynı API) | aynı |
| Boyut | `int` düğüm indeksi (2.147.483.647 sınır) | aynı |

> **YASAK:** EF Core navigasyonu (`Include` ile ağaç), `v26.1` dışı sürüm (G2 / DX-02).

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Yerleşik düzenleme + doğrulama | [treelist-inplace-editing](treelist-inplace-editing.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
