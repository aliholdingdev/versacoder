---
title: "grid-filtering-sorting - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "GridView istemci tarafi filtre (ActiveFilterString/kriter sozdizimi) ve coklu siralama (SortInfo) - Dapper ile DB tarafinda filtre ayrimi (v26.1)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Grid Filtre ve Siralama

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.rules/data-access-rules|data-access-rules]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 3/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Filtre ve siralama istemci tarafinda nasil kurulur, ne zaman sorguya parametre olarak gitmelidir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid filtresi **istemci tarafindadir**: `ActiveFilterString` veritabanina **gitmez**, yalniz ekrandaki `DataTable`'i suzer. Bu yuzden buyuk tabloda filtre **Dapper sorgusuna parametre** olarak yazilir, grid filtresi yalniz kucuk sayfada son rotus olur. Siralama ayni mantikla iki katmanlidir: `SortOrder`/`SortInfo` gorunumde, `ORDER BY` sorguda. Kriter sozdizimi DevExpress criteria dili (`[Alan]`), ham SQL **degildir**.

```text
+---------------------------------------------------------------+
| Buyuk veri  ->  Dapper WHERE/ORDER BY (@p1)  ->  grid'e az veri|
| Kucuk veri  ->  grid ActiveFilterString / SortInfo            |
|                                                               |
| criteria dili [Alan] = deger  ;  ham SQL'e cevirme YASAK      |
+---------------------------------------------------------------+
```

---

## §1 Filtre API

| Ozellik / olay | Ne yapar | Ornek |
|---|---|---|
| `gridView1.ActiveFilterString` | tum gorunume kriter uygula | `"[Sehir] = 'Ankara'"` |
| `col.FilterInfo` | tek kolonda filtre | `new GridColumnFilterInfo(text)` |
| `col.FilterMode` | kriter `DisplayText` mi `Value` mi | `GridColumnFilterMode.Value` |
| `col.FilterPopupMode` | popup filtre turu | `FilterPopupMode.CheckedList` |
| `gridView1.ClearColumnsFilter()` | tum filtreleri temizle | cagri |
| `gridView1.OptionsFilter.AllowColumnFilterIndicator` | kolon basligindaki filtre simgesi | `true` |

---

## §2 Siralama API

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `col.SortOrder` | tek kolon siralama | `ColumnSortOrder.Descending` |
| `gridView1.SortInfo` | coklu kolon siralamasi | `SortInfo.AddRange(...)` |
| `gridView1.ClearSorting()` | siralamayi temizle | cagri |
| `gridView1.OptionsCustomization.AllowSort` | kullanici siralamasini kapat | `false` |

```csharp
gridView1.CloseEditor();
gridView1.UpdateCurrentRow();
gridView1.Columns["Tarih"].SortOrder = ColumnSortOrder.Descending;
gridView1.Columns["Ad"].SortOrder = ColumnSortOrder.Ascending;
```

Duzenleme acikken siralama/filtre degisirse once `CloseEditor()` + `UpdateCurrentRow()` cagrilir; yoksa yazilan deger kaybolur.

---

## §3 Kriter sozdizimi

| Amac | Kriter |
|---|---|
| esitlik | `[Durum] = 1` |
| icerir | `Contains([Ad], 'Ahmet')` |
| baslar | `StartsWith([Kod], 'A')` |
| aralik | `[Tutar] Between 100 And 500` |
| mantiksal | `Contains([Ad], 'a') And [Sehir] = 'Izmir'` |
| null kontrolu | `IsNull([SilinmeTarihi])` |

Kriteri metin birlestirmekle **degil**, criteria nesnesiyle kur; girdi `OperandValue` uzerinden nesne olarak verilir (dogrudan metin yapistirma yasaktir). Veritabani filtresi ayni girdiyle Dapper `@p1` parametresi olarak gonderilir.

---

## §4 Istemci mi, DB mi (karar)

| Durum | Nereye konur | Neden |
|---|---|---|
| Tablo kucuk (binlerce satir alti) | grid `ActiveFilterString` | aninda tepki, sorgu degismez |
| Tablo cok buyuk | Dapper `WHERE @p1` | ham veriyi cekip suzmek bellek/paket tasirir |
| Kullanici "tumu" derse | Dapper parametresi `null` | `x = @p1` eslesmez; `IS NULL` dali yaz |
| Siralama kalici olacaksa | sorguda `ORDER BY` | sayfalama sirasi bozulur |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Buyuk tabloda filtre sorguda, grid'de degil | kod incelemesi |
| 2 | Kriter metnine girdi `+` ile yapistirilmamis | grep |
| 3 | Duzenleme acikken `CloseEditor()` var | kod incelemesi |
| 4 | Kalici siralama `ORDER BY` ile sorguda | sorgu metni |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\grid-filtering-sorting.md` (kaynak vault) | filtre/siralama API tablolari, kriter sozdizimi, istemci-DB karar tablosu |
| `.rules\devexpress-rules.md` | grid kurali |
| `.rules\data-access-rules.md` | parametreli sorgu, `SELECT *` yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur. "5.000 satir" esigi kaynakta kaba tahmin olarak vardi; kesin esik `DOĞRULAMA GEREKLI`dir.
