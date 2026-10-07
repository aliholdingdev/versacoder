---
title: "pdf-image-viewer - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 PdfViewer + PictureEdit kullanimi: paket adlari, LoadDocument akisi, ozellikler ve iki bacak farki"
---

# pdf-image-viewer — PDF ve Görüntü Görüntüleyici (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [richedit-document-api](richedit-document-api.md) · [serialization-layout-persistence](serialization-layout-persistence.md)

> **Tek cumlelik sorumluluk:** *PDF ve görsel nasıl güvenle açılır, hangi paketten gelir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

PDF görüntüleme `DevExpress.XtraPdfViewer.PdfViewer` ile olur; paket adı **`DevExpress.Win.PdfViewer`** (v26.1, NuGet.org). Görseller için `PictureEdit` kullanılır. İkisi de `DevExpress.Win` ailesindendir ve **yalnız UI katmanında** yaşar (DX-15). Dosya yolu **kullanıcı seçimi ya da profil klasörü**nden gelir; sabit yol ve repo içi ikili dosya **yok**. Açma işlemi dosya var mı kontrolü **sonrası** yapılır.

---

## §1 Paket ve Kontrol Haritasi

| İhtiyaç | Kontrol | Namespace | NuGet paketi (v26.1) |
|---|---|---|---|
| PDF okuma/görüntüleme | `PdfViewer` | `DevExpress.XtraPdfViewer` | `DevExpress.Win.PdfViewer` |
| Görsel gösterme | `PictureEdit` | `DevExpress.XtraEditors` | `DevExpress.Win` |
| Belge düzenleme (metin) | `RichEditControl` | `DevExpress.XtraRichEdit` | `DevExpress.Win.RichEdit` |
| Genel paket (master) | — | — | `DevExpress.Win` (+ `DevExpress.Win.Design` tasarım için) |

```
PdfViewer.LoadDocument(path)
   |
   +--> bicim: docx/pdf uzantisindan (NEEDS_VALIDATION: imza varyantlari)
   +--> ReadOnly gorsellestirme (düzenleme yok)
   +--> Layout: Dock = Fill  (DX-14 - sabit boyut yasak)
```

---

## §2 Anahtar Ozellikler

| Ozellik / API | Ne yapar | Not |
|---|---|---|
| `PdfViewer.LoadDocument(string)` | PDF'i açar | dosya yoksa hata fırlatır |
| `PdfViewer.Dock` | yerleşim | `DockStyle.Fill` |
| `PictureEdit.Image` | görsel kaynağı | `Bitmap`/`Image` |
| `PictureEdit.SizeMode` | sığdırma | `Zoom` (DPI güvenli) |
| `File.Exists(path)` | açılış öncesi kontrol | **zorunlu** |

---

## §3 Kod (guvenli acilis)

```csharp
using DevExpress.XtraPdfViewer;

private void OpenReport(string pdfPath, string imagePath)
{
    if (!File.Exists(pdfPath)) return;               // once guvenlik

    pdfViewer1.Dock = DockStyle.Fill;                // DX-14: sabit boyut yok
    pdfViewer1.LoadDocument(pdfPath);                 // DevExpress.Win.PdfViewer

    if (File.Exists(imagePath))
        pictureEdit1.Image = new Bitmap(imagePath);   // PictureEdit (DevExpress.Win)
}
```

| Yapılan | Yapılmayan |
|---|---|
| `File.Exists` sonra `LoadDocument` | doğrudan `LoadDocument` (hata yığını) |
| `Dock=Fill` | sabit `Size` (DPI'da taşar) |
| Yolu profil/selection'dan almak | `"C:\\sabit\\rapor.pdf"` |

---

## §4 Kurallar (en fazla 5 madde)

1. PDF/görsel **veri değil görüntüleme** katmanındadır; repository'ye girmez.
2. Kaynak dosya **repo'ya girmez** (`.gitignore`); test için geçici dizin.
3. `LoadDocument` hataları `try/catch` + kullanıcı mesajı (G5: kanıt = log).
4. İkili (binary) sızıntısına karşı lisans/dosya repo'ya **kaydedilmez** (DX-06 ruhu).
5. Kontrol `VersaCoder.Desktop`'ta (DX-15); çekirdek katman `DevExpress.*` **görmez**.

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Paket | `DevExpress.Win.PdfViewer` 26.1 (net48) | aynı paket (net10) |
| `PdfViewer.LoadDocument` | aynı imza | aynı imza |
| Dosya yolu | `Environment.GetFolderPath(ApplicationData)` | aynı |
| `Bitmap` | `System.Drawing` | `System.Drawing` (WinForms paketi) |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |

> **YASAK:** EF Core (G2), `v26.1` dışı DevExpress sürümü (DX-01/DX-02), `Office2010` dışı ribbon (DX-03).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Belge düzenleme (metin) | [richedit-document-api](richedit-document-api.md) |
| Dosya/layout yolu | [serialization-layout-persistence](serialization-layout-persistence.md) |
| Yerelleştirme | [localizing-ui-text](localizing-ui-text.md) |
| Paket/CPM | [nuget-references-cpm](nuget-references-cpm.md) |
