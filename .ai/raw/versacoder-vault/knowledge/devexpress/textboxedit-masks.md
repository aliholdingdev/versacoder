---
title: "textboxedit-masks - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "TextEdit mask turleri (RegEx/Numeric/DateTime), temel Mask ozellikleri, iki bacak farki ve DataAnnotations + DXErrorProvider dogrulamasi"
---

# textboxedit-masks - TextEdit Mask Kullanimi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [dx-validation-provider](dx-validation-provider.md)

> **Tek cumlelik sorumluluk:** *Bir TextEdit'e hangi mask tasinin nerede uygulanacagi, hangi ozelliklerin ayarlanacagi ve sonucun nasil dogrulanacagi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Text mask DevExpress v26.1'de `Properties.Mask` uzerinden yonetilir; mask motorunu `MaskType`, kalipti `EditMask` belirler. Serbest kaliplar (musteri kodu, plaka, telefon) icin `RegEx`, sayi girisleri icin `SpinEdit`/`CalcEdit`, tarih/saat icin `DateEdit` tercih edilir - `TextEdit` mask'i bunlarin yedejidir. Girdi dogrulamasi **DataAnnotations** (DTO uzerinde) ile yapilir, gorsel geri bildirim **`DXErrorProvider`** ile verilir; veri erisimi EF Core ile degil ADO.NET + Dapper ile yapilir (ADR-005 baglantisi). `net48` ile `net10.0-windows` ayni mask API'ini kullanir; fark derleyici, nullable ve olcu katmanindadir.

---

## §1 Mask Turleri Secimi

| MaskType | Nerede islevlidir | Tipik EditMask | Tercih |
|---|---|---|---|
| `RegEx` | serbest kalipli metin (kod, plaka) | `@"[A-Z]{3}-\d{6}"` | **bu dosyanin ana konusu** |
| `Numeric` | sayi/para girisi | `"n2"` | yerine `SpinEdit`/`CalcEdit` |
| `DateTime` / `DateTimeRegEx` | tarih/saat metni | `"dd.MM.yyyy"` | yerine `DateEdit` |
| `TimeSpan` | sure | `"hh\\:mm"` | nadir |
| `None` | mask yok (serbest metin) | - | varsayilan |

> **KURAL:** Mask girilen veriyi **sinirlar**, dogrulamaz. Is kurali ihlali yine de `DataAnnotations` + `DXErrorProvider` ile gosterilir (`DX-08` mantigi: veri baglamasi ayri, gorsel ayri).

---

## §2 Kod ve Ozellik Tablosu

```csharp
// FormName.cs - mask ayari elde yazilir, Designer'da degil
var p = this.textEditCustomerCode.Properties;
p.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
p.Mask.EditMask = @"[A-Z]{3}-\d{6}";
p.Mask.UseMaskAsDisplayFormat = true;
p.Mask.PlaceHolder = '_';
p.NullText = "ABC-000000";
```

| Ozellik | Ne yapar | Ornek deger |
|---|---|---|
| `Properties.Mask.MaskType` | mask motorunu secer | `MaskType.RegEx` |
| `Properties.Mask.EditMask` | kaliip/metin sablonu | `@"[A-Z]{3}-\d{6}"` |
| `Properties.Mask.UseMaskAsDisplayFormat` | odak disinda da maskeli gosterir | `true` |
| `Properties.Mask.PlaceHolder` | bos karakterin gorunen isareti | `'_'` |
| `Properties.Mask.SaveLiteral` | sabit ayiraclari degerde saklar | `false` |
| `Properties.NullText` | deger nullken gosterilen metin | `"ABC-000000"` |

**Diyagram (odak akisi):**

```
  odak VAR                     odak YOK
+----------------+          +----------------+
| [ABC-123456  ] |   <--    | ABC-123456     |   UseMaskAsDisplayFormat = true
| yer tutucu gorunur        | maskeli gosterim |
+----------------+          +----------------+
        |                            |
        +--> EditValue (deger)  <--  +--> text (gorunum)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 (LangVersion verilmedikce) | C# 14 |
| Nullable | `<Nullable>enable</Nullable>` elle acilir | proje sablonunda genelde acik |
| Binding redirect | `App.config` zorunlu (DX-06) | gerekmez |
| Varsayilan font | Microsoft Sans Serif 8.25pt | Segoe UI 9pt |
| Globalizasyon | NLS (isletim sistemi) | ICU |

**Tek dosyayi etkileyen iki fark:**

1. **Olcu:** Segoe UI 9pt daha genis gorundugu icin `net10.0-windows` bacakta sabit `Width` yerine `Anchor` kullanilmalidir (DX-14).
2. **Kultur:** `MaskType.Numeric` ve RegEx icindeki rakam ayiraci ICU/NLS tarafindan belirlenir; testler `CultureInfo("tr-TR")` ile sabitlenmelidir.

```csharp
using (new System.Globalization.CultureInfoScope("tr-TR")) // test/dogrulama ortami
    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
```

> `CultureInfoScope` vault'ta yok; satir **nasil** oldugunu gosterir - `NEEDS_VALIDATION`: kultur sabitleme yardimcisi `Application` katmaninda tanimlanacak.

---

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class CustomerDraft
{
    [Required(ErrorMessage = "Musteri kodu zorunludur.")]
    [RegularExpression(@"^[A-Z]{3}-\d{6}$", ErrorMessage = "Ornek: ABC-123456")]
    public string CustomerCode { get; set; }   // net10 nullable'da: string? veya initializer
}
```

```csharp
var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
bool ok = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
    dto, new System.ComponentModel.DataAnnotations.ValidationContext(dto), results, true);
this.dxErrorProvider1.SetError(this.textEditCustomerCode,
    ok || results.Count == 0 ? string.Empty : results[0].ErrorMessage);
```

| Katman | Sorumluluk | Dosya |
|---|---|---|
| DataAnnotations (`[Required]`, `[RegularExpression]`) | kuralin tanimi (SSOT) | DTO / `.Domain` |
| `Validator.TryValidateObject` | kurali calistirir | Form handler |
| `DXErrorProvider.SetError` | gorsel ikon + mesaj | `.Form.cs` |
| ADO.NET + Dapper (`@p1`) | kalici veri - **EF Core YASAK** | `VersaCoder.Data` |

`results[i].MemberNames[0]` ile hatali alan kontrolle eslestirilir; eslesmeyen mesaj form ustunde ozet olarak gosterilir.

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Mask'i `.Designer.cs`'de elle degistirmek | Designer yeniden yazinca kaybolur (DX-10) | `.Form.cs`'de ayarla |
| 2 | Mask'i dogrulama sanmak | gecersiz deger kabul | DataAnnotations ekle |
| 3 | `EditValue` yerine `Text` okumak | formatli/metin karisimi | `EditValue` kullan |
| 4 | Sabit `Width` + Segoe UI | net10'da kesilir | `Anchor`/`Dock` (DX-14) |
| 5 | DTO yerine form kontrolunu dogrulamak | test edilemez | DTO + validator |
