---
title: "grid-export-excel-pdf - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "GridControl'dan XLSX/CSV/PDF/HTML export, export oncesi editor kapatma, dosya ve kodlama kurallari (v26.1)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Excel ve PDF Export

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [gridcontrol-basics](gridcontrol-basics.md) · [performance-grid-optimization](performance-grid-optimization.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 3/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *Gorunen grid nasil XLSX/CSV/PDF'e cevrilir, export oncesi hangi hazirlik yapilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Export tek cagriyla yapilir: `gridControl1.ExportToXlsx(path)` / `ExportToPdf` / `ExportToCsv` / `ExportToHtml`. Export **ekrandaki gorunumu** (filtre, grup, sira, sutun genisligi) dosyaya yazar; bu yuzden once duzenlemeyi kapat (`CloseEditor`) ve gerekirse gorunumu sifirla. XLSX'te iki mod vardir: **WYSIWYG** (gorunen hali kopyala) ve **DataAware** (ham veri + Excel filtreleri) — rapor icin WYSIWYG, makro/analiz icin DataAware secilir. Surum **yalniz v26.1** (ADR-004).

```text
+-------------------------------------------------------------+
| CloseEditor + UpdateCurrentRow   <- acik hucre yazilmaz     |
|   -> ForceInitialize             <- kolonlar hazir          |
|   -> ExportToXlsx/Pdf/Csv        <- gorunumu dosyaya yaz     |
| DB'ye GITMEZ; ekrandaki DataTable'in kopyasidir             |
+-------------------------------------------------------------+
```

---

## §1 Export yontemleri

| Yontem | Uzanti | Tipik kullanim |
|---|---|---|
| `ExportToXlsx(path)` | `.xlsx` | kullanici raporu (varsayilan) |
| `ExportToCsv(path)` | `.csv` | baska sisteme aktarim, `;` ayirici |
| `ExportToPdf(path)` | `.pdf` | baski / arsiv |
| `ExportToHtml(path)` | `.html` | e-posta govdesi, intranet |
| `ShowRibbonPrintPreview()` | — | onizleme + "Excel'e aktar" dugmesi |

```csharp
var opt = new XlsxExportOptions();
opt.ExportType = DevExpress.XtraExport.ExportType.WYSIWYG;
opt.SheetName = "Faturalar";
gridControl1.ExportToXlsx(path, opt);
```

---

## §2 Export oncesi hazirlik

```csharp
gridView1.CloseEditor();
gridView1.UpdateCurrentRow();
gridControl1.ForceInitialize();
gridControl1.ExportToPdf(path);
```

| Hazirlik | Neden |
|---|---|
| `CloseEditor()` + `UpdateCurrentRow()` | acik hucre degeri dosyaya **yazilmaz** |
| Dosya varsa `File.Delete` / farkli ad | `IOException` (dosya Excel'de aciksa) |
| `SaveFileDialog` ile `.xlsx` filtresi | kullanici yanlis uzanti secemesin |
| Sayfalama varsa toplami DB'den al | export yalniz o sayfayi icerir |
| `BeginUpdate/EndUpdate` icinde toplu veri | export sirasinda yeniden cizim olmaz |

> **UYARI:** Export, veritabanina **gitmez**; ekrandaki `DataTable`'in kopyasidir. Cok satirli tabloda once sayfala ya da DB'den toplami ayrica ver (sayfalama karari icin [performance-grid-optimization](performance-grid-optimization.md)).

---

## §3 Mod ve kodlama

| Ayar | Deger | Sonuc |
|---|---|---|
| `ExportType` | `WYSIWYG` | baslik, renk, grup satiri aynen |
| `ExportType` | `DataAware` | ham veri; filtre/grup Excel'in |
| `SheetName` | `"Faturalar"` | sayfa adi UTF-8 olabilir |
| CSV | `ExportToCsv(path)` | varsayilan ayraca dikkat — TR'de `;` |
| PDF | `ExportToPdf(path)` | gomulu fontlar; Turkce karakterler dahil |

```csharp
var opt = new CsvExportOptions();
opt.Separator = ";";
gridControl1.ExportToCsv(path, opt);
```

---

## §4 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** — `using var` **yasak**; `SaveFileDialog` `System.Windows.Forms` | C# 14 (paylasilan dosya 7.3) |
| Export assembly | `DevExpress.XtraGrid` + `DevExpress.XtraPrinting` (CPM'den, ayni surum) | ayni paket referanslari |
| Dosya yolu | `Path.Combine` + uzun yol siniri (260) | `\\?\` uzun yol serbest |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| Encoding | CSV'de `Encoding.UTF8` (BOM'lu) | ayni; BOM Excel'de dogru acilir |

> **YASAK:** export ciktisini ucinde parti (OpenXML/EPPlus) ile **ikinci kez** uretmek; DevExpress export tek kaynaktir (G3 surum butunlugu).

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Export cagrisindan once `CloseEditor()` var | kod incelemesi |
| 2 | Dosya cakismasi onlenmis (sil/farkli ad) | kod incelemesi |
| 3 | CSV ayirici acikca `";"` yazilmis | kod incelemesi |
| 4 | Toplu veri exportu `BeginUpdate/EndUpdate` icinde | kod incelemesi |
| 5 | Her iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\grid-export-excel-pdf.md` (kaynak vault) | yontem tablosu, hazirlik adimlari, mod/kodlama tablosu, iki bacak farki |
| `.rules\devexpress-rules.md` | G3 surum butunlugu, DX-10 |
| `.rules\data-access-rules.md` | export DB'ye gitmez, toplam sorguda alinir |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | v26.1 tek surum |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; 50.000 satir esigi kaynakta kaba tahmin olarak vardi, kesin esik `DOĞRULAMA GEREKLI`dir.
