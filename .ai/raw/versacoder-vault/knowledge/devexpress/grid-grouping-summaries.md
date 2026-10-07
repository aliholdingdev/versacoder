---
title: "grid-grouping-summaries - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridView gruplama (GroupIndex), grup alti ve toplam ozetleri (GroupSummary/Summary) - Dapper veri uzerinde istemci tarafi hesap (v26.1)"
---

# grid-grouping-summaries — Gruplama ve ozetler (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [gridcontrol-basics](gridcontrol-basics.md)

> **Tek cumlelik sorumluluk:** *Gruplama ve ozetler nerede tanimlanir, hangi satir uzerinden hesaplanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Gruplama **görünüm katmanındadır**: `GridColumn.GroupIndex = 0` demek "bu sütuna göre grupla" demektir, sorguya `GROUP BY` **eklemez**. Özetler de aynı yerdedir: `col.SummaryItem` (footer toplamı) ve `gridView1.GroupSummary` (grup altı toplamı). Hesaplama **ekranda görünen satırlar** üzerinden yapılır; veritabanı toplamı gerekiyorsa ayrı Dapper sorgusu (`SUM(...)`) yazılır. Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Gruplama API

| Ozellik / yontem | Ne yapar | Ornek |
|---|---|---|
| `col.GroupIndex` | sütunu grupla (`-1` = gruplama yok) | `0` (birinci grup), `1` (iç grup) |
| `gridView1.OptionsView.ShowGroupPanel` | sürükle-bırak grup paneli | `false` (raporda) |
| `gridView1.GroupSummary` | grup altı özet satırı | `Sum` + `{0:n2}` |
| `gridView1.ExpandAllGroups()` | tüm grupları aç | açılışta |
| `gridView1.CollapseAllGroups()` | tüm grupları kapat | "özet görünüm" düğmesi |
| `gridView1.SortInfo` | grup sırası | `ColumnSortOrder.Descending` |

Grup sırası, grup anahtarının sıralamasıyla belirlenir; grup + detay için `SortInfo`'yu birlikte kur.

## §2 Ozet API

| Ozellik | Kapsam | Ornek |
|---|---|---|
| `col.SummaryItem.SummaryType` | o sütunun footer özeti | `SummaryType.Sum` |
| `col.SummaryItem.DisplayFormat` | footer metni | `"Toplam: {0:n2}"` |
| `gridView1.Summary` | tüm footer özetleri (koleksiyon) | `AddRange` |
| `gridView1.GroupSummary` | grup altı özetleri | `AddRange` |
| `gridView1.OptionsView.ShowFooter` | footer'ı görünür yap | `true` |

```csharp
gridView1.OptionsView.ShowFooter = true;
gridView1.GroupSummary.AddRange(new GridSummaryItem[] {
    new GridColumnSummaryItem(SummaryType.Sum, "Tutar", "Grup toplami: {0:n2}")
});
```

Toplam satırı (footer) aynı kalıpla `gridView1.Summary` üzerinden eklenir; `FieldName` veri kaynağındaki **ham alan adıdır**.

## §3 Ozetlerin neyi kapsadigi

| Senaryo | Ozetin gordugu | Dogru cozum |
|---|---|---|
| Filtre uygulanmış görünüm | yalnız görünen satır(lar) | grid özeti yeterli |
| Tüm tablonun toplamı | — | Dapper `SELECT SUM(Tutar)` + label |
| Sayfalanmış görünüm (sayfa 1/20) | yalnız o sayfa | DB toplamı zorunlu (bkz. [grid-paging-virtual-mode](grid-paging-virtual-mode.md)) |
| `NULL` hücreler | `Sum` atlar | `Avg`/`Count` seçerken `SummaryType.Custom` düşün |
| Düzenleme sonrası | eski değer kalır | `RefreshDataSource()` sonra `LayoutUpdated()` |

> **UYARI:** Sayfalama açıkken footer toplamı **her zaman yanıltıcıdır**; ekran toplamı ile DB toplamı çakışıyorsa bunu testle kanıtla (G5).

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `new GridSummaryItem[] { ... }` dizi başlatma serbest | C# 14 (paylaşılan dosya 7.3) |
| `decimal` hesap | `Sum` sonuç tipi kaynak alan tipine bağlı (`decimal` → `{0:n2}`) | aynı |
| DevExpress paketi | 26.1 + binding redirect | aynı CPM satırı |
| Grup/özet metni | `.resx` yerelleştirmesi (DX-05) | aynı |

> **YASAK:** özetleri grup satırında `CustomDrawCell` ile **elle çizme** (DX-08); `GroupSummary` kullan.

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Filtre + sıralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Koşullu biçimlendirme | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
