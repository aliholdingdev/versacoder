---
title: "pivotgrid-basics - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "PivotGridControl temelleri: PivotGridField alanlari, Area/SummaryType, Dapper DataTable baglama ve toplam ayarlari (v26.1)"
---

# pivotgrid-basics — PivotGridControl temelleri (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [grid-grouping-summaries](grid-grouping-summaries.md)

> **Tek cumlelik sorumluluk:** *Ham tablo PivotGrid'de alanlara ayrilir, ozet nasil hesaplanir ve veri nereden gelir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Pivot, **düz ham veriyi** (`DataTable`) alanlarla dört bölgeye ayırır: `FilterArea`, `RowArea`, `ColumnArea`, `DataArea`. Hesaplama alanının özeti `PivotGridField.SummaryType` ile belirlenir (`Sum` varsayılan). Veri kaynağı bu vault'ta **Dapper → `DataTable`**'dır; pivot veriyi **istemcide** toplar, dolayısıyla büyük tabloda önce sorguyu daralt (filtreli `WHERE @p1`). Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Bolgeler

| Bolge | Ozellik | Ne is yarar | Ornek alan |
|---|---|---|---|
| Filtre başlığı | `PivotArea.FilterArea` | tüm tabloyu süzer | `Yil` |
| Satır başlığı | `PivotArea.RowArea` | satır etiketleri | `Sehir` |
| Sütun başlığı | `PivotArea.ColumnArea` | sütun etiketleri | `Ay` |
| Veri (hesap) | `PivotArea.DataArea` | özet hücreler | `Tutar` |

```csharp
pivotGridControl1.RetrieveFields();
pivotGridControl1.Fields["Sehir"].Area = PivotArea.RowArea;
pivotGridControl1.Fields["Ay"].Area    = PivotArea.ColumnArea;
pivotGridControl1.Fields["Tutar"].Area = PivotArea.DataArea;
```

`Fields[...]` anahtarı **ham alan adıdır** (`FieldName`); `AreaIndex` aynı bölge içindeki sırayı verir.

## §2 Alan Ozellikleri

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `FieldName` | veri kaynağındaki alan | `"Tutar"` |
| `Area` / `AreaIndex` | bölge ve sıra | `DataArea` / `0` |
| `SummaryType` | hesaplama fonksiyonu | `PivotSummaryType.Sum` |
| `Caption` | ekranda görünen ad | `"Ciro (TL)"` |
| `AllowedAreas` | kullanıcının taşıyabileceği bölgeler | `RowArea + ColumnArea` |
| `Visible` | alanı gizle | `false` |

| SummaryType | Anlam | Not |
|---|---|---|
| `Sum` (varsayılan) | toplam | para/cari alanlar |
| `Count` | adet | `NULL`/`DBNull` atlanır |
| `Average` / `Max` / `Min` | ortalama / en büyük / en küçük | — |
| `CountDistinct` | farklı değer sayısı | yalnız Optimized mod |
| `Median` | medyan | yalnız Optimized mod |
| `Custom` | elle hesap (`CustomSummary` olayı) | Optimized/Server'da **desteklenmez** |

## §3 Veri Baglama (Dapper)

```csharp
using (var conn = new SqlConnection(connString))
{
    var t = new DataTable();
    using (var rd = conn.ExecuteReader(
        "SELECT Sehir, Ay, Tutar FROM dbo.Satis WHERE Yil = @p1", new { p1 = 2026 }))
        t.Load(rd);
    pivotGridControl1.DataSource = t;
}
```

| Ayar | Ozellik | Varsayilan | Ornek |
|---|---|---|---|
| Satır toplamı | `OptionsView.ShowRowGrandTotals` | `true` | `true` |
| Sütun toplamı | `OptionsView.ShowColumnGrandTotals` | `true` | raporda `false` |
| Alan oluşturma | `RetrieveFields()` | — | `DataSource` sonrası 1 kez |
| Yenileme | `DataSource` değiştir + `RetrieveFields()` **tekrar çağırmayın** | — | yalnız veri yenile |

> **UYARI:** `RetrieveFields()` ikinci kez çağrılırsa kullanıcının kurduğu alan bölgeleri (`Area`) sıfırlanır; alanları yalnız ilk kurulumda ayarla.

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak** | C# 14 (paylaşılan dosya 7.3) |
| PivotGrid paketi | `DevExpress.PivotGrid.v26.1.Core` + `DevExpress.XtraPivotGrid.v26.1` | aynı (aynı CPM satırı) |
| Bellek | ham `DataTable` + pivot kopyası → 64-bit önerilir | 64-bit varsayılan |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| Sayısal özet | `{0:n2}` biçimini `ValueFormat` ile ver | aynı |

> **YASAK:** pivot verisini EF Core sorgusuyla doldurmak ve `v26.1` dışı paket (G2 / DX-02).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Gruplama + özet (grid) | [grid-grouping-summaries](grid-grouping-summaries.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Export | [grid-export-excel-pdf](grid-export-excel-pdf.md) |
| Filtre + sıralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
