---
title: "DateEdit ve TimeEdit - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "DateEdit ve TimeEdit ile tarih/saat girisi: gosterim formati, MinValue/MaxValue, zaman duzenleme ve tarih dogrulamasi"
---

# DateEdit ve TimeEdit

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [hata kurallari](../../.rules/error-handling-rules.md) · [ADR-005 ado.net dapper](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [dx-validation-provider](dx-validation-provider.md) · [checkedcomboedit-values](checkedcomboedit-values.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress 5/35):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Tarih nerede `DateEdit`, saat nerede `TimeEdit` ile girilir, gosterim formati nasil sabitlenir ve aralik nasil dogrulanir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Gunluk tarih girisi `DateEdit` (`EditValue` = `DateTime`), yalnizca saat `TimeEdit` (`Time`) ile yapilir. Gosterim `Properties.DisplayFormat.FormatString` + `FormatType`, giris kaliibi `EditMask` ile sabitlenir ve **her iki taraf da ayni formatta** tutulur; aksi halde odak degisiminde tarih kayar. Alt/ust sinir `DateEdit.Properties.MinValue` / `MaxValue` ile **gorsel** olarak konur, asil kural DTO'da `[CustomValidation]` ile durur. `TimeEdit` in Min/Max destegi yoktur - sinirli sure araligi gerekiyorsa `TimeSpanEdit` kullanilir (destek kaydi t843740).

## §1 Tarih vs saat secimi

| Ihtiyac | Kontrol | Deger tipi | Sinir |
|---|---|---|---|
| Gunluk tarih | **`DateEdit`** | `DateTime` | `Properties.MinValue/MaxValue` |
| Tarih + saat ayni alanda | `DateEdit` + zaman duzenleme | `DateTime` | `MinValue/MaxValue` |
| Yalnizca saat (HH:mm) | **`TimeEdit`** | `Time` ozelligi | sinir yok |
| Sure araligi (00:00..08:00) | `TimeSpanEdit` | `TimeSpan` | `MinValue/MaxValue` |
| Takvim secimi + coklu tarih | cok secimli liste + kaynak | liste | - |

> **YASAK:** Serbest metin tarih (`TextEdit` + maske) - kultur ve dogrulama kirilmasi garantilidir.

## §2 Kod ve ozellik tablosu

```csharp
// FormName.cs - tarih formati + sinir + saat
var d = this.dateEditDueDate.Properties;
d.DisplayFormat.FormatString = "dd.MM.yyyy";
d.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
d.EditFormat.FormatString = "dd.MM.yyyy";
d.MinValue = DateTime.Today;
this.dateEditDueDate.EditValue = DateTime.Today.AddDays(7);
this.timeEditStart.Properties.EditMask = "HH:mm";
```

| Ozellik | Kontrol | Ne yapar | Ornek |
|---|---|---|---|
| `Properties.DisplayFormat.FormatString` | DateEdit | odak disinda gosterim | `"dd.MM.yyyy"` |
| `Properties.DisplayFormat.FormatType` | DateEdit | format tipi | `FormatType.DateTime` |
| `Properties.EditFormat.FormatString` | DateEdit | odak icinde gosterim | `"dd.MM.yyyy"` |
| `Properties.MinValue` / `MaxValue` | DateEdit | tarih siniri (gorsel) | `DateTime.Today` |
| `Properties.CalendarTimeEditing` | DateEdit | saat parcasi duzenlemeye acik | tipi **DOĞRULAMA GEREKLI** |
| `Properties.EditMask` | TimeEdit | giris kaliibi | `"HH:mm"` |
| `EditValue` | DateEdit | asil deger (`DateTime`) | `AddDays(7)` |

```
takvim / klavye --> EditMask (giris) --> EditValue : DateTime
                                              |
                      DisplayFormat <---------+--> gorunum "dd.MM.yyyy"
                                              |
                              MinValue/MaxValue (gorsel sinir)
                                              |
                            [CustomValidation] + DXErrorProvider
                                              |
                                    Dapper @p1 (DateTime parametre)
```

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 | C# 14 |
| Tarih kulturu | NLS - OS ayari | ICU - bolge verisi |
| `DateTime.Today` | yerel saat | ayni, `TimeZoneInfo` testleri sabitlenmeli |
| Binding redirect | `App.config` | gerekmez |
| Takvim olcusu | dar | Segoe UI 9pt ile genis |

```csharp
// Iki bacakta ayni sonuc - kultur farkina karsi inis sabitleme
DateTime gun = DateTime.Parse("25.09.2026",
    System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
    System.Globalization.DateTimeStyles.None);
```

> **KRITIK:** Format sabitlenmeden `CurrentCulture` ye birakilan `EditValue`, net48 (NLS) ile net10 (ICU) bacakta farkli gun/ay sirasi uretebilir - bu yuzden `FormatString` **kodda sabitlenir**, OS ayarina birakilmaz.

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class DueDateDraft
{
    [Required(ErrorMessage = "Teslim tarihi zorunludur.")]
    [CustomValidation(typeof(DueRules), nameof(DueRules.NotInPast))]
    public DateTime DueDate { get; set; }
}

public static class DueRules
{
    public static System.ComponentModel.DataAnnotations.ValidationResult NotInPast(
        DateTime value, System.ComponentModel.DataAnnotations.ValidationContext ctx)
        => value.Date >= DateTime.Today
            ? System.ComponentModel.DataAnnotations.ValidationResult.Success
            : new System.ComponentModel.DataAnnotations.ValidationResult("Tarih gecmis olamaz");
}
```

```csharp
this.dxErrorProvider1.SetError(this.dateEditDueDate,
    dto.DueDate.Date < DateTime.Today ? "Tarih gecmis olamaz" : string.Empty);
```

| Katman | Arac | Not |
|---|---|---|
| Kural | `[Required]` + `[CustomValidation]` | DTO = tek kaynak |
| Calistirma | `Validator.TryValidateObject` | test edilebilir, I/O yok |
| Gorsel | `DXErrorProvider.SetError` | ikon + mesaj |
| Kalici veri | Dapper `@p1` (`DateTime`) | EF Core onerilmez (ADR-005) |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `DisplayFormat` ile `EditMask` farkli | odakta tarih kayar | ikisini ayni formatta tut |
| 2 | `EditValue` yu `string` okumak | kultur kirilmasi | hep `DateTime` |
| 3 | Siniri yalnizca `MinValue` ile koymak | API ile asilir | DTO `[CustomValidation]` |
| 4 | `TimeEdit` ile sure siniri kurmak | calismaz | `TimeSpanEdit` |
| 5 | Yerel saati veritabanina yazmak | bacak farki | UTC yaz, gosterimde yerel cevir |

## §6 Karar ozeti

| Soru | Cevap |
|---|---|
| Tarih mi? | `DateEdit` |
| Saat mi? | `TimeEdit` |
| Sure araligi mi? | `TimeSpanEdit` |
| Sinir gorsel mi, kural mi? | gorsel: `MinValue/MaxValue`; kural: DTO |
| Format nerede sabit? | `DisplayFormat` + `EditFormat` + `EditMask` |

## §7 Kodlandirma notu

- Format sablounu tek yerde tanimla; odak ici ve disi ayni olsun.
- `EditValue` her yerde `DateTime` okunur, metin cevirisi yapilmaz.
- Geçmis tarih kurali DTO'da; UI yalniz ikon gosterir.
- Veritabanina UTC yaz, ekranda yerel saat goster.
- Tarih parse testleri `tr-TR` kulturu ile yazilir.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Hata ikonu akisi | [dx-validation-provider](dx-validation-provider.md) |
| Cok secimli liste | [checkedcomboedit-values](checkedcomboedit-values.md) |
| Kisa liste + tamamlama | [comboboxedit-autocomplete](comboboxedit-autocomplete.md) |
| Veri yazma (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `dateedit-timeedit.md` | secim tablosu, format/sinir ozellikleri, kultur (NLS/ICU) farki, `TimeSpanEdit` gerekçesi, dogrulama zinciri, hata tablosu | dogrulandi |
| `CalendarTimeEditing` deger tipi | kaynakta `NEEDS_VALIDATION` (destek yaniti "set ... to true") | **DOĞRULAMA GEREKLI** |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
