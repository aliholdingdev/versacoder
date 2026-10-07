---
title: "gridview-columns-format - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "GridView kolon baglari, tip ve bicim katmanlari, unbound kolonlar ve CustomColumnDisplayText metin uretimi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# GridView Kolon Baglari ve Bicim

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [skin-customization](skin-customization.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress 7/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) [accordion-navigation](accordion-navigation.md) [alertcontrol-notifications](alertcontrol-notifications.md) [bar-status-items](bar-status-items.md) [chartcontrol-basics](chartcontrol-basics.md) [checkedcomboedit-values](checkedcomboedit-values.md) [comboboxedit-autocomplete](comboboxedit-autocomplete.md) [dateedit-timeedit](dateedit-timeedit.md) [designer-runtime-customization](designer-runtime-customization.md) [docklayout-persistence](docklayout-persistence.md) [dockmanager-panels](dockmanager-panels.md) [dx-validation-provider](dx-validation-provider.md) [grid-export-excel-pdf](grid-export-excel-pdf.md) [grid-filtering-sorting](grid-filtering-sorting.md) [grid-grouping-summaries](grid-grouping-summaries.md) [grid-paging-virtual-mode](grid-paging-virtual-mode.md) [grid-search-panel-find](grid-search-panel-find.md) [layoutcontrol-grouping](layoutcontrol-grouping.md) [lookUpEdit-cascading](lookupedit-cascading.md) [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) [nuget-references-cpm](nuget-references-cpm.md) [performance-grid-optimization](performance-grid-optimization.md) [ribbon-control-office2010](ribbon-control-office2010.md) [ribbon-form-basics](ribbon-form-basics.md) [ribbon-pages-groups-items](ribbon-pages-groups-items.md) [splashscreen-waitform](splashscreen-waitform.md) [tabcontrol-multipage](tabcontrol-multipage.md) [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *Bir kolon veriyi nereden alir, nasil bicimlenir ve ne zaman metni kodda degistirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kolonun gorunumu **uc katmanda** uretilir: ham veri (`FieldName`) -> `FormatString` -> gerekirse `CustomColumnDisplayText`. Bag ve bicim **Designer'da** tutulur; kod yalniz hesaplanan (unbound) kolonlar ve maskelenmis metinler icin devreye girer. Elle hucre boyama (`CustomDrawCell`) **yasaktir** (DX-08) - onun yerine `FormatString`, `AppearanceCell` ve format rule kullanilir.

```text
+------------------------------------------------------------------+
| Katman 1: FieldName      -> ham veri (kaynagin alan adi)          |
| Katman 2: FormatString   -> {0:n2}, {0:dd.MM.yyyy}, {0:p0}       |
| Katman 3: CustomColumnDisplayText -> maske/metin (olay kazanir)   |
| YASAK: CustomDrawCell (DX-08) - DPI/tema altinda kirilir         |
+------------------------------------------------------------------+
```

---

## §1 Kolon ozellikleri

| Ozellik | Ne is yarar | Ornek |
|---|---|---|
| `FieldName` | veri kaynagindaki alan adi | `"Tutar"` |
| `Caption` | baslik metni | `"Tutar (TL)"` |
| `VisibleIndex` | siralama ve gorunurluk (`-1` = gizli) | `2` |
| `Width` | sutun genisligi (px) | `120` |
| `FormatType` + `FormatString` | hucre bicimi | `Numeric` + `{0:n2}` |
| `UnboundType` | hesaplanan sutun tipi | `UnboundColumnType.String` |
| `ColumnEdit` | hucre editörü (RepositoryItem) | `RepositoryItemTextEdit` |

`FieldName` degeri veri kaynaginda yoksa kolon **bos gorunur** ve hata da uretmez; ilk kontrol edilecek yer burasidir.

---

## §2 Bicimleme sirasi (kod + eslesme)

```csharp
var col = gridView1.Columns["Tutar"];
col.Caption = "Tutar (TL)";
col.FormatType = DevExpress.Utils.FormatType.Numeric;
col.FormatString = "{0:n2}";
col.VisibleIndex = 2;
```

| Veri tipi | FormatString / yol | Sonuc |
|---|---|---|
| para / ondalik | `{0:n2}` | `1.234,56` |
| tarih | `{0:dd.MM.yyyy}` | `25.09.2026` |
| yuzde | `{0:p0}` | `%12` |
| metin maskesi | `CustomColumnDisplayText` olayi | `TR** **** **** 1234` |
| evet/hayir | `RepositoryItemCheckEdit` + `Caption` | onay kutusu |

---

## §3 Kodda metin uretimi ve unbound kolon

```csharp
private void grid_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
{
    if (e.Column.FieldName == "Iban")
        e.DisplayText = MaskIban(e.Value as string);
}
```

Hesaplanan sutun (veri kaynaginda yok, uretiliyor):

```csharp
var col = gridView1.Columns["DurumText"];
col.UnboundType = DevExpress.Data.UnboundColumnType.String;
gridView1.CustomUnboundColumnData += (s, e) =>
{
    if (e.Column.FieldName == "DurumText")
        e.Value = Convert.ToInt32(e.GetListSourceRowValue("Durum")) == 1 ? "Aktif" : "Pasif";
};
```

`GetListSourceRowValue` satir verisine erisir; bu, `SELECT *` yerine yalniz gereken alanlari cekmeyi mumkun kilir.

---

## §4 Icerik kurallari

- `FormatString` varsa `CustomColumnDisplayText` **gerekmez**; ikisi birden yazma (oncelik: olay kazanir).
- Kolon basligi sabit metinse `Caption`, yerellestirme gerekirse `.resx` (DX-05).
- Sutun sirasi/genisligi `SaveLayoutToXml` ile kullaniciya birakilir; kodda `VisibleIndex` yalniz zorunlu olanlar icin kullanilir.
- `BestFitColumns()` yalniz ilk acilista cagrilir; her `DataSourceChanged`'te cagirmak kaydirma hissi yaratir.
- Maskelenen veri (`Iban`, `Telefon`) icin `CustomColumnDisplayText` yeterli; veritabanina maske **yazma**.

---

## §5 Kim neyi degistirir

| Amaç | Dokunulacak yer | Katman |
|---|---|---|
| Baslik degistirme | `Caption` (Designer) | kolon |
| Sayi/tarih bicimi | `FormatType` + `FormatString` | bicim |
| Kosullu renk/format | format rule | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Maskeleme (IBAN, telefon) | `CustomColumnDisplayText` | olay |
| Zemin/font (genel) | `Appearance` / skin | [skin-customization](skin-customization.md) |
| Sutun sirasi kalici | `SaveLayoutToXml` | [docking-saved-layout-xml](docking-saved-layout-xml.md) |

---

## §6 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - lambda body'de `if` bloku, `var` serbest; `using var` **yasak** | C# 14 (paylasilan dosya 7.3 kalir) |
| `FormatString` davranisi | `n2` / `dd.MM.yyyy` kulture bagli | ayni; `CultureInfo` acik verilirse sonuc ayni |
| DevExpress paketi | 26.1 + binding redirect | ayni CPM satiri |
| `UnboundColumnType` | `DevExpress.Data` (ayni tip) | ayni tip |

> **YASAK:** EF Core, `v26.1` disi surum ve elle hucre cizimi (G2 / DX-02 / DX-08).

---

## §7 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
| Kosullu bicimlendirme | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Duzenleme + editör | [grid-editing-modes](grid-editing-modes.md) |
| Layout kaydi | [docking-saved-layout-xml](docking-saved-layout-xml.md) |

---

## §8 Denetim listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `FieldName` degerleri kaynak sorgudaki alanlarla ayni | sorgu + kolon karsilastirma |
| 2 | `FormatString` ile olay ayni kolonda birlikte degil | grep |
| 3 | `CustomDrawCell` yok (DX-08) | grep = 0 eslesme |
| 4 | Maske DB'ye yazilmadi | veri modeli incelemesi |
| 5 | `BestFitColumns()` yalniz acilista | kod incelemesi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\gridview-columns-format.md` (kaynak vault) | uc katman, kolon ozellik + bicim eslesme tablolari, unbound ornegi, icerik kurallari, TFM farki |
| `.rules\devexpress-rules.md` | DX-08 (elle cizim yasagi), DX-05 (yerellestirme) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
