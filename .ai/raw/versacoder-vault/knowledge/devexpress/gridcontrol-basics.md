---
title: "gridcontrol-basics - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridControl + GridView iskeleti, baglama sirasi, veri doldurma ve olay noktalari (DevExpress Universal 2026 / v26.1, Dapper IDataReader/DataTable)"
---

# gridcontrol-basics — GridControl + GridView iskeleti (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [gridview-columns-format](gridview-columns-format.md)

> **Tek cumlelik sorumluluk:** *GridControl hangi veriyle, hangi sirayla ve hangi olay noktalariyla kurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid'de tek bir çift vardır: **`GridControl` veri taşır, `GridView` görünümü yönetir** (kolon, sıralama, gruplama, editör). Bu vault'ta veri kaynağı **Dapper `ExecuteReader` → `DataTable.Load`** yoludur; EF Core **yasak**. Bağlama sırası sabittir: `DataSource` → `PopulateColumns()` → görünüm ayarları; tersi kurulursa kolonlar boş kalır. Kolon ve görünüm ayarları **Designer'da**, veri doldurma ve olay mantığı **kodda** yaşar (DX-08). Sürüm **yalnız DevExpress Universal 2026 / v26.1** (ADR-004).

---

## §1 Sorumluluk Dagilimi

| Nesne | Ne yapar | Nerede yazilir |
|---|---|---|
| `GridControl` | `DataSource`, seviye (level), gömülü görünüm, export | Designer + kod (yalnız `DataSource`) |
| `GridView` | kolon, sıralama, gruplama, filtre, editör, özet | Designer + kod (ayarlar) |
| `GridColumn` | `FieldName`, `Caption`, `VisibleIndex`, biçim | Designer |
| `GridLevelDescriptor` | master-detail (çocuk görünüm) hiyerarşisi | Designer |

> **KURAL:** Kolon ekleme/silme kodda tekrarlanmaz; Designer'da kurulur, kod yalnız otomatik üretimi (`PopulateColumns`) çağırır.

## §2 Kurulum Sırası (kod)

```csharp
gridControl1.Dock = DockStyle.Fill;
gridControl1.DataSource = LoadMusteriler();   // Dapper -> DataTable
gridView1.PopulateColumns();                  // kolonlari uret
gridView1.BestFitColumns();
gridView1.OptionsBehavior.Editable = false;   // salt okunur ornek
```

Veri doldurma — Dapper `IDataReader` → `DataTable` (string-concat SQL **yok**, G2):

```csharp
using (var conn = new SqlConnection(connString))
{
    var table = new DataTable();
    using (var reader = conn.ExecuteReader(sql, new { p1 = "Aktif" }))
        table.Load(reader);
    gridControl1.DataSource = table;
}
```

> **UYARI:** `PopulateColumns()` çağrısı `DataSource`'tan **sonra** gelir; aksi halde kolon listesi boştur ve hata da vermez.

## §3 Temel Ozellikler

| Ozellik | Varsayilan | Ne is yarar | Ornek deger |
|---|---|---|---|
| `gridControl1.DataSource` | `null` | veri seti | `DataTable` / `List<T>` |
| `gridView1.Name` | — | layout kaydı anahtarı | `"gridViewMusteri"` |
| `gridView1.OptionsView.ShowGroupPanel` | `true` | sürükle-bırak grup paneli | `false` |
| `gridView1.OptionsBehavior.Editable` | `true` | hücre düzenleme | `false` (rapor ekranı) |
| `gridView1.OptionsSelection.MultiSelect` | `false` | çoklu seçim | `true` (toplu işlem) |
| `gridView1.OptionsView.ShowFooter` | `false` | özet satırı | `true` (tutar kolonu varsa) |

`Name` alanı benzersiz tutulmalı; `SaveLayoutToXml` / `RestoreLayoutFromXml` bu adla çalışır.

## §4 Olay Noktalari

| Olay | Ne zaman tetiklenir | Tipik kullanim |
|---|---|---|
| `FocusedRowObjectChanged` | odak satırı değişir | sağ paneli / detayı besle |
| `DoubleClick` | satıra çift tıklama | detay formunu aç |
| `CustomColumnDisplayText` | hücre metni üretilir | maskeleme (IBAN, telefon) |
| `ShownEditor` | editör açıldı | etkin editörün özelliğini değiştir |
| `DataSourceChanged` | veri yenilendi | `BestFitColumns()` / sayaç güncelle |

Handler ince kalır; iş mantığı veri katmanındaki repository'ye gider (SOL-02).

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var`, target-typed `new` **yasak** | C# 14 (aynı dosya 7.3'te kalır) |
| DevExpress paketi | `Directory.Packages.props` 26.1 + `App.config` binding redirect | aynı CPM satırı, redirect gerekmez |
| `SqlClient` | `Microsoft.Data.SqlClient` 5.2.x | aynı paket |
| `DataTable` / `IDataReader` | `System.Data` — aynı API | aynı API, davranış farkı yok |

> **YASAK:** bu sayfada EF Core, `v26.1` dışı sürüm, `RibbonControlStyle.Office2010` dışı ribbon (G2 / G3 / DX-02 / DX-03).

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Sütun ve biçim | [gridview-columns-format](gridview-columns-format.md) |
| Filtre ve sıralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
