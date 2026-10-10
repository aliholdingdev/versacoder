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

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.rules/data-access-rules|data-access-rules]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 3/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

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

> **UYARI:** Export, veritabanina **gitmez**; ekrandaki `DataTable`'in kopyasidir. Cok satirli tabloda once sayfala ya da DB'den toplami ayrica ver (sayfalama karari icin [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]]).

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
