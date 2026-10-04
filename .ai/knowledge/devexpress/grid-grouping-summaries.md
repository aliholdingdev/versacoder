---
title: "grid-grouping-summaries - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GridView gruplama (GroupIndex), grup alti ve footer ozetleri, istemci tarafi hesabin kapsami ve DB toplami ayrimi"
---

# Grid Gruplama ve Ozetleri

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [performance-grid-optimization](performance-grid-optimization.md)

> **Tek cumlelik sorumluluk:** *Gruplama ve ozetler nerede tanimlanir, hangi satir uzerinden hesaplanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Gruplama **gorunum katmanindadir**: `GridColumn.GroupIndex = 0` demek "bu sutuna gore grupla" demektir, sorguya `GROUP BY` **eklemez**. Ozetler de ayni yerdedir: `col.SummaryItem` (footer toplami) ve `gridView1.GroupSummary` (grup alti toplami). Hesaplama **ekranda gorunen satirlar** uzerinden yapilir; tablonun tamaminin toplami gerekiyorsa ayri bir Dapper sorgusu (`SUM(...)`) yazilir.

```text
+------------------------------------------------------------------+
| GroupIndex    -> gorunum katmani, SQL'de GROUP BY YOK             |
| SummaryItem   -> footer ozeti (ShowFooter = true)                 |
| GroupSummary  -> grup alti ozeti                                 |
| Hesap kapsami -> ekrandaki satirlar (filtre/sayfa dahil)          |
| Tam toplam    -> ayri Dapper SUM(...) + label                     |
+------------------------------------------------------------------+
```

---

## §1 Gruplama API

| Ozellik / yontem | Ne yapar | Ornek |
|---|---|---|
| `col.GroupIndex` | sutunu grupla (`-1` = gruplama yok) | `0` (birinci grup), `1` (ic grup) |
| `gridView1.OptionsView.ShowGroupPanel` | surukle-birak grup paneli | `false` (raporda) |
| `gridView1.GroupSummary` | grup alti ozet satiri | `Sum` + `{0:n2}` |
| `gridView1.ExpandAllGroups()` / `CollapseAllGroups()` | tumunu ac / kapat | acilista / ozet gorunumu |
| `gridView1.SortInfo` | grup sirasi | `ColumnSortOrder.Descending` |

Grup sirasi, grup anahtarinin siralamasiyla belirlenir; grup + detay icin `SortInfo`'yu birlikte kur.

---

## §2 Ozet API

| Ozellik | Kapsam | Ornek |
|---|---|---|
| `col.SummaryItem.SummaryType` | o sutunun footer ozeti | `SummaryType.Sum` |
| `col.SummaryItem.DisplayFormat` | footer metni | `"Toplam: {0:n2}"` |
| `gridView1.Summary` | tum footer ozetleri (koleksiyon) | `AddRange` |
| `gridView1.GroupSummary` | grup alti ozetleri | `AddRange` |
| `gridView1.OptionsView.ShowFooter` | footer'i gorunur yap | `true` |

```csharp
gridView1.OptionsView.ShowFooter = true;
gridView1.GroupSummary.AddRange(new GridSummaryItem[] {
    new GridColumnSummaryItem(SummaryType.Sum, "Tutar", "Grup toplami: {0:n2}")
});
```

Toplam satiri (footer) ayni kalip `gridView1.Summary` uzerinden eklenir; `FieldName` veri kaynagindaki **ham alan adidir**.

---

## §3 Ozetlerin neyi kapsadigi

| Senaryo | Ozetin gordugu | Dogru cozum |
|---|---|---|
| Filtre uygulanmis gorunum | yalniz gorunen satir(lar) | grid ozeti yeterli |
| Tablonun tamaminin toplami | - | Dapper `SELECT SUM(Tutar)` + label |
| Sayfalanmis gorunum (sayfa 1/N) | yalniz o sayfa | DB toplami zorunlu (bkz. [grid-paging-virtual-mode](grid-paging-virtual-mode.md)) |
| `NULL` hucreler | `Sum` atlar | `Avg`/`Count` icin `SummaryType.Custom` dusun |
| Duzenleme sonrasi | eski deger kalir | `RefreshDataSource()` sonra `LayoutUpdated()` |

> **UYARI:** Sayfalama acikken footer toplami **her zaman yanilticidir**; ekran toplami ile DB toplami cakisiyorsa bunu testle kanitla (G5).

---

## §4 Kim neyi degistirir

| Istenen degisiklik | Dokunulacak yer | Dikkat |
|---|---|---|
| Grup sutunu degisme | `col.GroupIndex` (Designer) | `-1` = gruplamayi kaldir |
| Grup alti toplam ekleme | `gridView1.GroupSummary` | `ShowFooter` sart degil, grup satiri yeterli |
| Footer toplam ekleme | `col.SummaryItem` + `ShowFooter = true` | `FieldName` ham alan adi |
| Metin ceviri | `.resx` yerellestirmesi (DX-05) | grup/ozet metinleri de |
| Elle cizim istegi | **yapilmaz** | `CustomDrawCell` yerine `GroupSummary` (DX-08) |

---

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `new GridSummaryItem[] { ... }` dizi baslatma serbest | C# 14 (paylasilan dosya 7.3) |
| `decimal` hesap | `Sum` sonuc tipi kaynak alan tipine bagli (`decimal` -> `{0:n2}`) | ayni |
| DevExpress paketi | 26.1 + binding redirect | ayni CPM satiri |
| Grup/ozet metni | `.resx` yerellestirmesi (DX-05) | ayni |

---

## §6 Tarama tablosu (kod uzerinde aranacaklar)

| Aranan | Istenen durum | Yorum |
|---|---|---|
| `GROUP BY` | sorguda yok | gruplama gorunum katmaninda |
| `CustomDrawCell` | 0 eslesme | ozet `GroupSummary` ile uretilir (DX-08) |
| `SummaryType.Sum` + `ShowFooter` | birlikte | footer gorunur degilse toplam gorunmez |
| `SELECT SUM(` | yalniz tam toplam icin | DB toplami grid ozetinin yerine gecmez |
| `SelectCountMember` / `COUNT(*)` | ayri sayac | toplam sayfa sayisi icin |

---

## §7 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Filtre + siralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Kosullu bicimlendirme | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
| Export (ozetlerle birlikte) | [grid-export-excel-pdf](grid-export-excel-pdf.md) |

---

## §8 Denetim listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Gruplama `GroupIndex` ile, sorguda `GROUP BY` yok | SQL kod incelemesi |
| 2 | Footer icin `ShowFooter = true` acik | ozellik denetimi |
| 3 | Sayfalamali ekranda DB toplami kullaniliyor | ekran + sorgu incelemesi |
| 4 | Ozet metinleri `.resx` uzerinden | yerellestirme denetimi |
| 5 | Elle hucre cizimi yok (DX-08) | grep `CustomDrawCell` |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\grid-grouping-summaries.md` (kaynak vault) | GroupIndex kurali, ozet API tablosu, kapsama senaryolari, TFM farki |
| `.rules\devexpress-rules.md` | DX-05 (yerellestirme), DX-08 (elle cizim yasagi) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
