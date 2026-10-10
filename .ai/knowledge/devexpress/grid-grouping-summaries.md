---
title: "grid-grouping-summaries - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "GridView gruplama (GroupIndex), grup alti ve footer ozetleri, istemci tarafi hesabin kapsami ve DB toplami ayrimi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Grid Gruplama ve Ozetleri

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress 7/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] [[knowledge/devexpress/accordion-navigation|accordion-navigation]] [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] [[knowledge/devexpress/bar-status-items|bar-status-items]] [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] [[knowledge/devexpress/skin-customization|skin-customization]] [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

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
| Sayfalanmis gorunum (sayfa 1/N) | yalniz o sayfa | DB toplami zorunlu (bkz. [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]]) |
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
| Filtre + siralama | [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] |
| Sayfalama + sanal mod | [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] |
| Kosullu bicimlendirme | [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] |
| Grid iskeleti | [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] |
| Export (ozetlerle birlikte) | [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] |

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
