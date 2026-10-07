---
title: "ComboBoxEdit AutoComplete - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "ComboBoxEdit otomatik tamamlama (AutoComplete), dropdown ayarlari, hangi liste icin hangi editor secimi ve dogrulama"
---

# ComboBoxEdit AutoComplete

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [kodlama standartlari](../../.rules/coding-standards.md) · [ADR-005 ado.net dapper](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [dx-validation-provider](dx-validation-provider.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress 5/35):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Kisa ve sabit bir listede ComboBoxEdit autocomplete'in nerede acilacagi, hangi ozelliklerle ayarlanacagi ve degerin nasil dogrulanacagi* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

`ComboBoxEdit` kisa, sabit ve tek secimli listeler icindir; otomatik tamamlama `Properties.AutoComplete = true` ile acilir ve yalnizca duzenlenebilir metin modunda (`TextEditStyles.Standard`) anlamlidir. Liste 20+ kayit veya veritabanindan geliyorsa `ComboBoxEdit` degil **`LookUpEdit`** kullanilir; cok secim `CheckedComboBoxEdit`, etiket girdisi `TokenEdit`. Otomatik tamamlama yalnizca **gorsel kisayol**dir - girdinin dogrulamasi DataAnnotations + `DXErrorProvider` ile yapilir.

## §1 Editor secim tablosu

| Durum | Editor | Neden |
|---|---|---|
| <= 20 sabit deger, tek secim | **`ComboBoxEdit`** | hafif, dropdown + tamamlama |
| 20+ kayit / sorgudan gelen liste | `LookUpEdit` | arama kolonu, kaynak baglama |
| Birden fazla deger secilecek | `CheckedComboBoxEdit` | bkz. [checkedcomboedit-values](checkedcomboedit-values.md) |
| Serbest + onayli etiket girdisi | `TokenEdit` | ayiracli metin |
| Agac yapisi secimi | `TreeListLookUpEdit` | hiyerarseci secim |

> **YASAK:** Listeyi kod icinde `Items.Add` ile 50+ kez tekrarlamak - sabit dizi kaynak dosyada, kod tekrar yok.

## §2 Kod ve ozellik tablosu

```csharp
// FormName.cs - kisa sabit liste + otomatik tamamlama
string[] yerler = { "tr-TR", "en-US", "de-DE", "fr-FR" };
this.comboBoxEditLocale.Properties.Items.AddRange(yerler);
this.comboBoxEditLocale.Properties.AutoComplete = true;
this.comboBoxEditLocale.Properties.TextEditStyle =
    DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
this.comboBoxEditLocale.Properties.DropDownRows = 8;
```

| Ozellik | Ne yapar | Ornek deger |
|---|---|---|
| `Properties.AutoComplete` | yazarken ilk karaktere gore tamamlar (`bool`) | `true` |
| `Properties.TextEditStyle` | metin kutusu modu | `TextEditStyles.Standard` |
| `Properties.DropDownRows` | gorunen satir sayisi | `8` |
| `Properties.Items` | liste elemanlari (`object[]`) | `"tr-TR"` ... |
| `EditValue` | secili deger (null olabilir) | `"tr-TR"` |
| `Properties.NullText` | deger yokken gosterim | `"Seciniz"` |

```
 kullanic "t" yazar
      |
      v
 +----------+     AutoComplete = true      +-------------+
 | metin kutu | -------------------------> | "tr-TR" vurgu |
 +----------+     Standard mod gerekir     +-------------+
      |
      +--> EditValue = "tr-TR"   (serbest giris de mumkun)
```

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 - `AddRange(params object[])` tercih edilir | C# 14 - `AddRange(string[])` uzerlemesi mumkun |
| Nullable | `EditValue` tipi `object` | `<Nullable>enable` ise `object?` isareti |
| Binding redirect | `App.config` | gerekmez |
| Varsayilan font | Microsoft Sans Serif 8.25pt | Segoe UI 9pt |
| Dropdown olcusu | satir yuksekligi dar | ayni `DropDownRows` daha yuksek gorunur |

1. **Font farki** dropdown'un yer kaplamasini degistirir; satir sayisi sabit `DropDownRows` ile sinirlanir, `Anchor` ile degil.
2. **Kultur (ICU vs NLS):** tamamlama karsilastirmasi `CurrentCulture` ile yapilir; `"tr-TR"` icin `I`/`ı` buyuk/kucuk harf farki test edilmelidir.

```csharp
// Her iki bacakta ayni calisir - platform farki olmadigi icin #if YOK
this.comboBoxEditLocale.Properties.AutoComplete =
    System.Threading.Thread.CurrentThread.CurrentCulture.Name.StartsWith("tr");
```

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class LocaleDraft
{
    [Required(ErrorMessage = "Dil secimi zorunludur.")]
    [RegularExpression("^(tr-TR|en-US|de-DE|fr-FR)$", ErrorMessage = "Listede olmayan dil")]
    public string Locale { get; set; }        // net10: string?
}
```

```csharp
bool listede = dto.Locale != null &&
               this.comboBoxEditLocale.Properties.Items.Contains(dto.Locale);
this.dxErrorProvider1.SetError(this.comboBoxEditLocale,
    listede ? string.Empty : "Listeden bir dil seciniz.");
```

| Katman | Kural | Geri bildirim |
|---|---|---|
| DataAnnotations | deger tipi + sablon | `Validator.TryValidateObject` |
| Liste uyumu | `Items.Contains` | `DXErrorProvider.SetError` |
| Kalici veri | ADO.NET + Dapper, `@p1` parametreli | - |
| Yasak | EF Core / string-concat SQL | mimari kirilimi |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `AutoComplete = true` + `DisableTextEditor` | tamamlama hic calismaz | `TextEditStyles.Standard` |
| 2 | Listeyi `TextChanged` icinde yeniden doldurmak | yanip sonme + kayip secim | kaynagi bir kez doldur |
| 3 | `Text` okuyup `EditValue` yazmak | tip karisimi | iki yonde de `EditValue` |
| 4 | 200 kayitli listeyi ComboBoxEdit'e sokmak | yavas, arama yok | `LookUpEdit` |
| 5 | Dogrulamayi autocomplete'e birakmak | gecersiz deger kabul | §4 zinciri |

## §6 Kodlandirma notu

- Liste doldurma tek noktada; her acilista tekrarlanmaz.
- `EditValue` tek dogru deger kanalidir; `Text` ile yazma yapilmaz.
- Sabit listeler kod/ic kaynakta, satir tekrari olmadan tutulur.
- Karsilastirma kulturu `tr-TR` icin ayri test edilir.
- Eksik secim hata metni `.resx` uzerinden gelir.

## §7 Karar ozeti

| Soru | Cevap |
|---|---|
| Kisa sabit liste mi? | `ComboBoxEdit` + `AutoComplete` |
| 20+ / sorgu listesi mi? | `LookUpEdit` |
| Cok secim mi? | `CheckedComboBoxEdit` |
| Etiket girdisi mi? | `TokenEdit` |
| Dogrulama nerede? | DTO + `DXErrorProvider`, autocomplete degil |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Cok secim | [checkedcomboedit-values](checkedcomboedit-values.md) |
| Tarih/saat editoru | [dateedit-timeedit](dateedit-timeedit.md) |
| Hata ikonu | [dx-validation-provider](dx-validation-provider.md) |
| Grid veri kaynagi | [datasource-binding-dapper](datasource-binding-dapper.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `comboboxedit-autocomplete.md` | editor secim tablosu, `AutoComplete` / `TextEditStyle` / `DropDownRows` ozellikleri, kultur notu, dogrulama zinciri, hata tablosu | dogrulandi |
| `checkedcomboedit-values.md` | karsilastirilan cok secim kontrolu | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
