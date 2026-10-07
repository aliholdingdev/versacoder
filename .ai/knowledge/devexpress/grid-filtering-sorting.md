---
title: "grid-filtering-sorting - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "GridView istemci tarafi filtre (ActiveFilterString/kriter sozdizimi) ve coklu siralama (SortInfo) - Dapper ile DB tarafinda filtre ayrimi (v26.1)"
---

# Grid Filtre ve Siralama

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [gridcontrol-basics](gridcontrol-basics.md) · [performance-grid-optimization](performance-grid-optimization.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress · 3/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookUpEdit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
