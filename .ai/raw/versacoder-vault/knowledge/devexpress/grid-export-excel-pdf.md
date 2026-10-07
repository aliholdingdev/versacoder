---
title: "grid-export-excel-pdf - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridControl'dan XLSX/CSV/PDF/HTML export, export oncesi editör kapatma, dosya ve kodlama kurallari (v26.1)"
---

# grid-export-excel-pdf — Excel ve PDF export (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md)

> **Tek cumlelik sorumluluk:** *Gorunen grid nasil XLSX/CSV/PDF'e cevrilir, export oncesi hangi hazirlik yapilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Export tek çağrıyla yapılır: `gridControl1.ExportToXlsx(path)` / `ExportToPdf` / `ExportToCsv` / `ExportToHtml`. Export **ekrandaki görünümü** (filtre, grup, sıra, sütun genişliği) dosyaya yazdığı için, önce düzenlemeyi kapat (`CloseEditor`) ve gerekirse görünümü sıfırla. XLSX'te iki mod vardır: **WYSIWYG** (görünen hâli kopyala) ve **DataAware** (ham veri + Excel filtreleri) — rapor için WYSIWYG, makro/analiz için DataAware seçilir. Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Export Yontemleri

| Yontem | Uzanti | Tipik kullanim |
|---|---|---|
| `ExportToXlsx(path)` | `.xlsx` | kullanıcı raporu (varsayılan) |
| `ExportToCsv(path)` | `.csv` | başka sisteme aktarım, `;` ayırıcı |
| `ExportToPdf(path)` | `.pdf` | baskı / arşiv |
| `ExportToHtml(path)` | `.html` | e-posta gövdesi, intranet |
| `ShowRibbonPrintPreview()` | — | önizleme + "Excel'e aktar" düğmesi |

```csharp
var opt = new XlsxExportOptions();
opt.ExportType = DevExpress.XtraExport.ExportType.WYSIWYG;
opt.SheetName = "Faturalar";
gridControl1.ExportToXlsx(path, opt);
```

## §2 Export Oncesi Hazirlik

```csharp
gridView1.CloseEditor();
gridView1.UpdateCurrentRow();
gridControl1.ForceInitialize();
gridControl1.ExportToPdf(path);
```

| Hazirlik | Neden |
|---|---|
| `CloseEditor()` + `UpdateCurrentRow()` | açık hücre değeri dosyaya **yazılmaz** |
| Dosya varsa `File.Delete` / farklı ad | `IOException` (dosya Excel'de açıkse) |
| `SaveFileDialog` ile `.xlsx` filtresi | kullanıcı yanlış uzantı seçemesin |
| Sayfalama varsa toplamı DB'den al | export yalnız o sayfayı içerir |
| `BeginUpdate/EndUpdate` içinde toplu veri | export sırasında yeniden çizim olmaz |

> **UYARI:** Export, veritabanına **gitmez**; ekrandaki `DataTable`'ın kopyasıdır. 50.000+ satırda önce sayfala ya da DB'den toplamı ayrıca ver (bkz. [grid-paging-virtual-mode](grid-paging-virtual-mode.md)).

## §3 Mod ve Kodlama

| Ayar | Deger | Sonuc |
|---|---|---|
| `ExportType` | `WYSIWYG` | başlık, renk, grup satırı aynen |
| `ExportType` | `DataAware` | ham veri; Excel'de filtre/grup Excel'in |
| `SheetName` | `"Faturalar"` | sayfa adı Türkçe olabilir (UTF-8) |
| CSV | `ExportToCsv(path)` | varsayılan ayraç locale göre — TR'de `;` |
| PDF | `ExportToPdf(path)` | gömülü fontlar; Türkçe karakterler dahil |

```csharp
var opt = new CsvExportOptions();
opt.Separator = ";";
gridControl1.ExportToCsv(path, opt);
```

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak**; `SaveFileDialog` `System.Windows.Forms` | C# 14 (paylaşılan dosya 7.3) |
| Export assembly | `DevExpress.XtraGrid.v26.1` (+ `DevExpress.XtraPrinting.v26.1`) | aynı paket referansları |
| Dosya yolu | `Path.Combine` + uzun yol sınırı (260) | `\\?\` uzun yol serbest |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| Encoding | CSV'de `Encoding.UTF8` (BOM'lu) | aynı; BOM Excel-Türkçe'de doğru açılır |

> **YASAK:** export çıktısını üçüncü parti (OpenXML/EPPlus) ile **ikinci kez** üretmek; DevExpress export tek kaynaktır (G3 sürüm bütünlüğü).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Gruplama + özet | [grid-grouping-summaries](grid-grouping-summaries.md) |
