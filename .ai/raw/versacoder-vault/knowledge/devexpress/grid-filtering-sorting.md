---
title: "grid-filtering-sorting - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridView istemci tarafi filtre (ActiveFilterString/kriter sozdizimi) ve coklu siralama (SortInfo) - Dapper ile DB tarafinda filtre ayrimi (v26.1)"
---

# grid-filtering-sorting — Filtre ve siralama (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-search-panel-find](grid-search-panel-find.md)

> **Tek cumlelik sorumluluk:** *Filtre ve siralama istemci tarafinda nasil kurulur, ne zaman sorguya Dapper parametresi olarak gitmelidir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid filtresi **istemci tarafındadır**: `ActiveFilterString` gecersiz kılmaz, yalnız ekrandaki `DataTable`'ı süzer — veritabanına gitmez. Bu yüzden büyük tabloda filtre **Dapper sorgusuna parametre** olarak yazılır, grid filtresi yalnız küçük sayfada son rötuş olur. Sıralama aynı mantıkla iki katmanlıdır: `SortOrder`/`SortInfo` görünümde, `ORDER BY` sorguda. Kriter sözdizimi DevExpress criteria dili (`[Alan]`), ham SQL **değildir**.

---

## §1 Filtre API

| Ozellik / olay | Ne yapar | Ornek |
|---|---|---|
| `gridView1.ActiveFilterString` | tüm görünüme kriter uygula | `"[Sehir] = 'Ankara'"` |
| `col.FilterInfo` | tek kolonda filtre | `new GridColumnFilterInfo(text)` |
| `col.FilterMode` | kriter `DisplayText` mi `Value` mı | `GridColumnFilterMode.Value` |
| `col.FilterPopupMode` | popup filtre türü | `FilterPopupMode.CheckedList` |
| `gridView1.ClearColumnsFilter()` | tüm filtreleri temizle | çağrı |
| `gridView1.OptionsFilter.AllowColumnFilterIndicator` | kolon başlığındaki filtre simgesi | `true` |

## §2 Sıralama API

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `col.SortOrder` | tek kolon sıralama | `ColumnSortOrder.Descending` |
| `gridView1.SortInfo` | çoklu kolon sıralaması | `SortInfo.AddRange(...)` |
| `gridView1.ClearSorting()` | sıralamayı temizle | çağrı |
| `gridView1.OptionsCustomization.AllowSort` | kullanıcı sıralamasını kapat | `false` |
| `gridView1.ActiveFilter...` ile birlikte | filtre + sıralama sırası | sıralama filtre sonra uygulanır |

```csharp
gridView1.CloseEditor();
gridView1.UpdateCurrentRow();
gridView1.Columns["Tarih"].SortOrder = ColumnSortOrder.Descending;
gridView1.Columns["Ad"].SortOrder = ColumnSortOrder.Ascending;
```

> **UYARI:** Düzenleme açıkken sıralama/filtre değişirse önce `CloseEditor()` + `UpdateCurrentRow()` çağır; yoksa yazılan değer kaybolur.

## §3 Kriter Sozdizimi

| Amaç | Kriter |
|---|---|
| eşitlik | `[Durum] = 1` |
| içerir | `Contains([Ad], 'Ahmet')` |
| başlar | `StartsWith([Kod], 'A')` |
| aralık | `[Tutar] Between 100 And 500` |
| mantıksal | `Contains([Ad], 'a') And [Sehir] = 'Izmir'` |
| null kontrolü | `IsNull([SilinmeTarihi])` |

Kriteri metin birleştirmeyle **değil**, criteria nesnesiyle kur — tırnak/SQL enjeksiyonu girişimi criteria katmanında da kırılır:

```csharp
var kriter = new FunctionOperator(FunctionOperatorType.Contains,
    new OperandProperty("Ad"), new OperandValue(kullaniciGirdisi));
gridView1.ActiveFilterString = kriter.ToString();
```

> **YASAK:** kriter metnine girdiyi `+` ile **yapıştırma**; criteria dilinde parametre yoktur, bu yüzden girdi `OperandValue` üzerinden nesne olarak verilir. Veritabanı filtresi ise aynı girdiyle Dapper `@p1` parametresi olarak gönderilir.

## §4 Istemci mi, DB mi (karar)

| Durum | Nereye konur | Neden |
|---|---|---|
| Tablo ≤ ~5.000 satır | grid `ActiveFilterString` | anında tepki, sorgu değişmez |
| Tablo > ~5.000 satır | Dapper `WHERE @p1` | `SELECT *` ile çekip süzmek bellek/paket taşırır |
| Kullanıcı "tümü" derse | Dapper parametresi `null` | `WHERE x = @p1` + `@p1 = null` eşleşmez; `IS NULL` dalı yaz |
| Sıralama kalıcı olacaksa | sorguda `ORDER BY` | sayfalama sırası bozulur |

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — interpolated string ve LINQ serbest, `using var` **yasak** | C# 14 (paylaşılan dosya 7.3 kalır) |
| DevExpress criteria | `DevExpress.Data.Filtering` (aynı namespace) | aynı |
| Dapper | parametre nesnesi anonim tip | aynı |
| Kültür (tarih karşılaştırma) | `tr-TR` açık ver | aynı; aksi halde `dd.MM` ile `MM.dd` farkı |

> **YASAK:** kriterleri SQL `WHERE` metnine gömme; kriter dilini ham SQL'e çevirme (G2).

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Arama paneli | [grid-search-panel-find](grid-search-panel-find.md) |
| Gruplama ve özet | [grid-grouping-summaries](grid-grouping-summaries.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
