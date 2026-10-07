---
title: "memoedit-richedit - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "MemoEdit ile duz metin girdisi ve RichEditControl ile DOCX/RTF yukleme-kaydetme - ozellikler, iki bacak farki ve dogrulama"
---

# memoedit-richedit - MemoEdit ve RichEditControl

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [pictureedit-upload](pictureedit-upload.md)

> **Tek cumlelik sorumluluk:** *Duz metin icin MemoEdit, zengin belge icin RichEditControl'un nerede kullanilacagi ve belgenin nasil yuklenip kaydedilecegi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kisa not/gorus girdisi icin **`MemoEdit`** (`EditValue` = `string`), bicimli belge (DOCX/RTF) icin **`RichEditControl`** kullanilir; ikisi de DX-09 kontrol listesinde IDE ve Chat ekranlari icin adlandirilmistir. Belge `LoadDocument(yol, DocumentFormat.OpenXml)` ile yuklenir, `SaveDocument(...)` ile kaydedilir. Karakter/uzunluk siniri UI'da degil **DTO'da** (`[StringLength]`) tanimlanir; `MemoEdit`'te `WordWrap`/`ScrollBars` yalnizca gorunum ayridir. Dosyaya yazma islemi I/O oldugundan testlerde **`NEEDS_VALIDATION`**: dosya erisimi `Application` katmaninda soyutlandirilir (EF Core YASAK, veri I/O'su ADO.NET + Dapper).

---

## §1 Duz Metin mi, Zengin Belge mi

| Ihtiyac | Kontrol | Deger | Depolama |
|---|---|---|---|
| Tek satir not | `TextEdit` | `string` | kolon (`@p1`) |
| Cok satirli duz metin | **`MemoEdit`** | `string` | kolon (`@p1`) |
| Bicimli belge (baslik, tablo) | **`RichEditControl`** | dosya | dosya yolu kolonu |
| Salt okunur belge gosterimi | `RichEditControl.ReadOnly` | - | - |
| Markdown/HTML onizleme | ozel render | `string` | kolon |

> **KURAL:** Zengin belge **metin olarak** veritabanina gömülmez; dosya diskte/saglayicida, veritabaninda **yolu** tutulur (ADR-005: I/O ADO.NET + Dapper ile parametreli).

---

## §2 Kod ve Ozellik Tablosu

```csharp
// FormName.cs - not alani + belge yukleme
this.memoEditComment.Properties.WordWrap = true;
this.memoEditComment.EditValue = dto.Comment;

var path = @"C:\data\spec.docx";
this.richEditControl1.LoadDocument(path,
    DevExpress.XtraRichEdit.API.Native.DocumentFormat.OpenXml);
this.richEditControl1.ReadOnly = true;
```

| Ozellik / metot | Kontrol | Ne yapar |
|---|---|---|
| `Properties.WordWrap` | MemoEdit | satir kaydirma (acik/kapali) |
| `Properties.ScrollBars` | MemoEdit | gorunen kaydirma cubukleri (`Vertical`) |
| `AcceptsReturn` | MemoEdit | Enter ile yeni satir |
| `EditValue` | MemoEdit | asil metin (`string`) |
| `LoadDocument(path, DocumentFormat)` | RichEditControl | bicim verilerek yukleme |
| `SaveDocument(path, DocumentFormat)` | RichEditControl | bicim verilerek kaydetme |
| `ReadOnly` | RichEditControl | duzenlemeyi kapat |

**Diyagram (belge hatti):**

```
 disk / upload --> LoadDocument(path, OpenXml) --> RichEditControl.Document
                                                        |
                                     SaveDocument(path, Docx)
                                                        |
                                        yol (string) --> Dapper @p1
                                        icerik   --> dosya sistemi (test: NEEDS_VALIDATION)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 | C# 14 |
| Nullable | metin `string` - uyari yok | `<Nullable>enable` ise `string?` gerekir |
| Dosya yolu API | `Path.Combine` ayni | ayni (`System.IO`) |
| Binding redirect | `App.config` zorunlu (DX-06) | gerekmez |
| Belge render olcusu | 8.25pt | Segoe UI 9pt - sayfa sayisi farkli olabilir |

```csharp
// Iki bacakta ayni - platform API'si degil, #if YOK
this.memoEditComment.EditValue = string.IsNullOrEmpty(dto.Comment) ? null : dto.Comment;
string yol = System.IO.Path.Combine(veriKlasoru, "spec.docx");
```

> **NOT:** RichEditControl'un font listesi ve sayfalama sonuclari iki bacakta birebir ayni degildir; **belge icerigi degil gorunum** farklidir. Belge kaynak dosyasi (`.docx`) bacaklar arasinda ortaktir.

---

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class CommentDraft
{
    [StringLength(2000, ErrorMessage = "En fazla 2000 karakter")]
    public string Comment { get; set; }        // net10: string?
}
```

```csharp
var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
bool ok = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
    dto, new System.ComponentModel.DataAnnotations.ValidationContext(dto), results, true);
this.dxErrorProvider1.SetError(this.memoEditComment,
    ok || results.Count == 0 ? string.Empty : results[0].ErrorMessage);
```

| Katman | Arac | Not |
|---|---|---|
| Uzunluk kurali | `[StringLength]` (DTO) | UI'da degil, test edilebilir |
| Calistirma | `Validator.TryValidateObject` | iki bacakta ayni |
| Gorsel | `DXErrorProvider.SetError` | ikon + mesaj |
| Belge I/O | `LoadDocument`/`SaveDocument` | `NEEDS_VALIDATION`: test icin dosya soyutlamasi |
| Veri I/O | Dapper `@p1` | **EF Core YASAK** (`G2`) |

> `RichEditControl` uzerinde `DXErrorProvider` ikonu **desteklenmez** (destek kaydi t426127); hata metni yan `MemoEdit`/`LabelControl` uzerinde gosterilir.

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Belgeyi metin kolonuna yazmak | buyuk veri + bozulma | dosya + yol kolonu |
| 2 | `LoadDocument`'i `Form_Load` icinde senkron | ekran donar | arka plan is + `NEEDS_VALIDATION` |
| 3 | `MemoEdit`'te uzunluk siniri yazmak | kural test edilemez | `[StringLength]` DTO'da |
| 4 | Belge formatini tahmin etmek | `FileLoadException` | `DocumentFormat` parametresi ver |
| 5 | Designer'a `ReadOnly` atamak | DX-10 ihlali | `.Form.cs`'de ayarla |
