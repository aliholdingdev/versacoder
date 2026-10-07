---
title: "richedit-document-api - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 RichEditControl belge API'si: ac/kaydet, metin ekleme, salt okunur kullanim, paket ve iki bacak farki"
---

# richedit-document-api — RichEdit Belge API'si (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [pdf-image-viewer](pdf-image-viewer.md) · [gridcontrol-basics](gridcontrol-basics.md)

> **Tek cumlelik sorumluluk:** *RichEditControl ile belge nasıl açılır, yazılır, salt okunur gösterilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`RichEditControl`, **Editor ekranının** ana kontrolüdür (DX-09). Belge işlemleri üç adımdır: `LoadDocument(yol)` → `Document` üzerinden düzenleme → `SaveDocument(yol)`. Dosya biçimi uzantıdan çıkar; biçim zorlanacaksa `DocumentFormat` gerekir. Editing/salt-okunur sınırı **kontrol seviyesinde** ayarlanır, metin `string`'e kopyalanarak taşınmaz (performans + biçim kaybı). Paket `DevExpress.Win.RichEdit`, sürüm `v26.1`.

---

## §1 Belge Akisi

```
LoadDocument(path)          <-- bicim uzantidan (docx/rtf/html...)
        |
   Document (API nesnesi)   <-- CreatePosition / InsertText / secim
        |
        +--> ReadOnly = true   -> salt okunur gorunum (Dashboard/Not ekranlari)
        |
SaveDocument(path)          <-- ustune kaydet (yedek alinirsa)
```

| Durum | Ne yapılır | Kaçınılır |
|---|---|---|
| Sadece gösterim | `LoadDocument` + `ReadOnly` | metni `string`'e alıp `Text=` ile geri yazmak |
| Kısa not ekleme | `Document.InsertText` | tüm belgeyi `StringBuilder` ile yeniden kurmak |
| Biçimli dışa aktarma | `SaveDocument` | elle HTML/RTF üretimi |

---

## §2 Anahtar API'ler

| API | Ne yapar | Not |
|---|---|---|
| `LoadDocument(string)` | belgeyi açar | biçim uzantıdan |
| `SaveDocument(string)` | kaydeder | dosya kilitliyse `IOException` |
| `Document.CreatePosition(int)` | konum üretir | `0` = baş |
| `Document.InsertText(pos, text)` | konuma metin | `NEEDS_VALIDATION`: tam imza v26.1'de derlenir |
| `ReadOnly` | düzenlemeyi kapatır | görüntüleme ekranları |
| `DocumentFormat` | biçim zorlama | ad alanı `NEEDS_VALIDATION` |

---

## §3 Kod (ac + ekle + kaydet)

```csharp
using DevExpress.XtraRichEdit;

private void OpenNote(string path)
{
    richEditControl1.Dock = DockStyle.Fill;
    richEditControl1.LoadDocument(path);                 // bicim uzantidan

    var pos = richEditControl1.Document.CreatePosition(0);
    richEditControl1.Document.InsertText(pos, "VersaCoder notu\r\n");

    richEditControl1.SaveDocument(path);                 // ustune kaydet
}
```

| Örnek | Ayar |
|---|---|
| Salt okunur panel | `richEditControl1.ReadOnly = true;` |
| Farklı biçimde kaydet | `SaveDocument(yol, DocumentFormat.<...>)` — enum adı `NEEDS_VALIDATION` |
| Bellek dostu sunum | `RichEditDocumentServer` (görünmeyen), aynı `Document` API'si |

---

## §4 Kurallar (en fazla 5 madde)

1. Belge **diskten** açılır, kod içine gömülmez (repo şişmesin).
2. `ReadOnly` yalnız görüntüleme ekranında; düzenleme UI'da açık.
3. Kaydetme öncesi **yedek** kopya (`.bak`) — veri kaybı riskine karşı.
4. Metin çekimi (`DocumentText`) yalnız dışa aktarım için; UI yolunda değil.
5. Kontrol `VersaCoder.Desktop`'ta yaşar (DX-15); çekirdeğe `RichEdit` **girmez**.

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Paket | `DevExpress.Win.RichEdit` 26.1 (net48) | aynı paket (net10) |
| Belge API'si | aynı sınıf adları | aynı |
| Dosya yolu | `ApplicationData` (bkz. [serialization-layout-persistence](serialization-layout-persistence.md)) | aynı |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |
| `#if` | yalnız platform dosya/interop farkı | aynı |

> **YASAK:** EF Core (G2), `v26.1` dışı sürüm (DX-02), ribbon stili değişikliği (DX-03).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| PDF/görüntüleme | [pdf-image-viewer](pdf-image-viewer.md) |
| Yerelleştirme (menü metinleri) | [localizing-ui-text](localizing-ui-text.md) |
| Dosya yolu stratejisi | [serialization-layout-persistence](serialization-layout-persistence.md) |
| Editor ekranı grid/sekme | [gridcontrol-basics](gridcontrol-basics.md) |
