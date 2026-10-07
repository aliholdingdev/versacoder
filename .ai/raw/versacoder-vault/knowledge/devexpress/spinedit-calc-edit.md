---
title: "spinedit-calc-edit - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "SpinEdit ve CalcEdit ile sayi/para girisi - aralik, artis, precision, kultur (ICU/NLS) farki ve [Range] dogrulamasi"
---

# spinedit-calc-edit - SpinEdit ve CalcEdit

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [dx-validation-provider](dx-validation-provider.md)

> **Tek cumlelik sorumluluk:** *Sayi girisi icin SpinEdit mi CalcEdit mi secilir, aralik/precision nasil ayarlanir ve deger nasil dogrulanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tam sayi ve adimli giris icin **`SpinEdit`** (`MinValue`/`MaxValue`/`Increment`), hesap makinesi gerektiren ondalikli giris icin **`CalcEdit`** (`Precision`) kullanilir. Her ikisi de `EditValue` uzerinden `decimal`/`int` deger tasiyir; metin okumak yerine hep `EditValue` okunur. Sinirlar yalnizca **gorsel**dir: asil kural DTO'da `[Range]` ile durur ve `DXErrorProvider` ile gosterilir. Iki bacakta da kritik fark **kultur kaynakli ondalik ayiraci**dir (net48 NLS vs net10 ICU) - para/alani kaydi her iki bacakta ayni `CultureInfo` ile test edilir.

---

## §1 Editor Secimi

| Durum | Editor | Temel ozellikler |
|---|---|---|
| Tam sayi, adimli (0..30) | **`SpinEdit`** | `MinValue`, `MaxValue`, `Increment` |
| Ondalikli, hesap makinesi popup | **`CalcEdit`** | `Precision`, `EditValue` (decimal) |
| Para birimi + binlik ayirac | `CalcEdit` + `DisplayFormat` | `FormatString = "n2"` |
| Salt okunur gosterim | `LabelControl` | deger formati kod tarafinda |
| Tablo hucresinde sayi | `GridControl` kolon editoru | `ColumnEdit` (`gridlookupedit.md` ile ayni RepositoryItem mantigi) |

> **KURAL:** Aralik disi deger girisi engelleniyorsa bu **mimari degil, gorsel** bir katmandir; servis seviyesinde de `[Range]` kurali calisir (test edilebilirlik).

---

## §2 Kod ve Ozellik Tablosu

```csharp
// FormName.cs - aralik ve artis
var s = this.spinEditRetryCount.Properties;
s.MinValue = 0;
s.MaxValue = 30;
s.Increment = 1;
this.spinEditRetryCount.EditValue = 3;

var c = this.calcEditRatio.Properties;
c.Precision = 4;
this.calcEditRatio.EditValue = 1.5m;
```

| Ozellik | Editor | Ne yapar | Ornek |
|---|---|---|---|
| `Properties.MinValue` | SpinEdit | alt sinir (`decimal`) | `0` |
| `Properties.MaxValue` | SpinEdit | ust sinir (`decimal`) | `30` |
| `Properties.Increment` | SpinEdit | artis/azalis adimi | `1` |
| `Properties.Precision` | CalcEdit | ondalik basamak siniri | `4` |
| `EditValue` | ikisi | asil deger (okuma/yazma) | `1.5m` |
| `Properties.NullText` | ikisi | deger yokken gosterim | `"0"` |

**Diyagram (girdi hatti):**

```
 tus / klavye --> [ MinValue .. MaxValue ] --> EditValue (decimal)
                                                  |
                          DTO [Range(0,30)] <-----+--> DXErrorProvider (ikon)
                                                  |
                                        Dapper @p1 (parameterized)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 | C# 14 (`decimal` inis/yukselisleri ayni) |
| Sayi ayiraci | NLS - `.` veya `,` | ICU - bolgeye gore |
| `decimal.Parse` | `CurrentCulture` gecerli | ayni, ama ICU varyantlari farkli olabilir |
| Binding redirect | `App.config` (DX-06) | gerekmez |
| Varsayilan font | 8.25pt | 9pt - SpinEdit genisligi farkli |

**Kritik not:** `EditValue` `decimal` oldugu icin **metin parserina dusmekten kacinilir**; yine de disaridan gelen metin `decimal.Parse(s, CultureInfo.InvariantCulture)` ile parse edilir - iki bacakta da ayni sonucu verir.

```csharp
// Her iki bacakta birebir ayni davranir - #if gerekmez
decimal deger = decimal.Parse("1.5", System.Globalization.CultureInfo.InvariantCulture);
this.calcEditRatio.EditValue = deger;
```

---

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class RetrySettings
{
    [Range(0, 30, ErrorMessage = "Deneme sayisi 0..30 arasinda olmali")]
    public int RetryCount { get; set; }

    [Range(typeof(decimal), "0.01", "100.0", ErrorMessage = "Oran 0.01..100.0")]
    public decimal Ratio { get; set; }
}
```

```csharp
var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
bool ok = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
    dto, new System.ComponentModel.DataAnnotations.ValidationContext(dto), results, true);
this.dxErrorProvider1.SetError(this.spinEditRetryCount,
    ok || results.Count == 0 ? string.Empty : results[0].ErrorMessage);
```

| Katman | Araclar | Not |
|---|---|---|
| Kural tanimi | `[Range]` (DataAnnotations) | DTO = SSOT, test edilebilir |
| Kural calistirma | `Validator.TryValidateObject` | her iki bacakta da calisir |
| Gorsel geri bildirim | `DXErrorProvider.SetError` | ikon + mesaj |
| Kalici veri | ADO.NET + Dapper, `@p1` | **EF Core YASAK** (`G2`) |

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `Text` degerini parse etmek | kultur kirilmasi | hep `EditValue` |
| 2 | Siniri yalnizca UI'da koymak | API uzerinden asilir | DTO `[Range]` + servis dogrulamasi |
| 3 | `Precision` ile `DisplayFormat`'i karistirmak | giris sinirlanmiyor | giris = `Precision`, gosterim = `FormatString` |
| 4 | `Increment` = 0 | artis yok | adim > 0 |
| 5 | Her bacakta farkli kultur varsayimi | test net10'da kirmizi | `CultureInfo.InvariantCulture` veya `tr-TR` sabitle |
