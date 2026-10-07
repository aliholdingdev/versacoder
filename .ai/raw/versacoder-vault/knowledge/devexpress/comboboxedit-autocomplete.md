---
title: "comboboxedit-autocomplete - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "ComboBoxEdit otomatik tamamlama (AutoComplete), dropdown ayarlari, hangi liste icin hangi editor secimi ve dogrulama"
---

# comboboxedit-autocomplete - ComboBoxEdit AutoComplete

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [checkedcomboedit-values](checkedcomboedit-values.md)

> **Tek cumlelik sorumluluk:** *Kisa ve sabit bir listede ComboBoxEdit autocomplete'in nerede acilacagi, hangi ozelliklerle ayarlanacagi ve degerin nasil dogrulanacagi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`ComboBoxEdit` kisa, sabit ve tek secimli listeler icindir; otomatik tamamlama `Properties.AutoComplete = true` ile acilir ve yalnizca duzenlenebilir metin modunda (`TextEditStyles.Standard`) anlamlidir. Liste 20+ kayit veya veri tabanindan geliyorsa `ComboBoxEdit` degil **`LookUpEdit`** kullanilir; cok secim `CheckedComboBoxEdit`, etiket girdisi `TokenEdit` (`gridlookupedit.md`, `checkedcomboedit-values.md`, `tokenedit-tags.md`). Otomatik tamamlama yalnizca **gorsel kisayol**dur - girdinin dogrulamasi DataAnnotations + `DXErrorProvider` ile yapilir.

---

## §1 Editor Secim Tablosu

| Durum | Editor | Neden |
|---|---|---|
| ≤ 20 sabit deger, tek secim | **`ComboBoxEdit`** | hafif, dropdown + tamamlama |
| 20+ kayit / sorgudan gelen liste | `LookUpEdit` | arama kolonu, kaynak baglama |
| Birden fazla deger secilecek | `CheckedComboBoxEdit` | bkz. `checkedcomboedit-values.md` |
| Serbest + onayli etiket girdisi | `TokenEdit` | bkz. `tokenedit-tags.md` |
| Agac yapisi secimi | `TreeListLookUpEdit` | bkz. `treelistlookedit.md` |

> **YASAK:** Listeyi kod icinde `Items.Add` ile 50+ kez tekrarlamak - kaynak dosyada sabit dizi, kod tekrar yok.

---

## §2 Kod ve Ozellik Tablosu

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
| `Properties.NullText` | deger yokken gosterim | "Seciniz" |

**Diyagram (autocomplete akisi):**

```
 kullanic "t" yazar
      |
      v
 +----------+     AutoComplete = true      +-------------+
 | metin kutu | -------------------------> | "tr-TR" vurgu |
 +----------+     Standard mod gerekir     +-------------+
      |
      +--> EditValue = "tr-TR"   (DropDownStyle/Text ile serbest giris de mumkun)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 - `AddRange(params object[])` tercih edilir | C# 14 - `AddRange(string[])` uzerlemesi mumkun |
| Nullable | `EditValue` tipi `object` - nullable uyarisi yok | `<Nullable>enable` ise `object?` olarak isaretlenmeli |
| Binding redirect | `App.config` (DX-06) | gerekmez |
| Varsayilan font | Microsoft Sans Serif 8.25pt | Segoe UI 9pt |
| Dropdown olcusu | satir yuksekligi dar | ayni `DropDownRows` daha yuksek gorunur |

1. **Font farki** dropdown'un yer kaplamasini degistirir; satir sayisini `net10` bacakta `Anchor` ile degil sabit `DropDownRows` ile sinirlayin.
2. **Kultur (ICU vs NLS):** tamamlama karsilastirmasi `CurrentCulture` ile yapilir; `"tr-TR"` icin `I`/`ı` buyuk/kucuk harf farki test edilmelidir.

```csharp
// Her iki bacakta ayni calisir - platform farki olmadigi icin #if YOK
this.comboBoxEditLocale.Properties.AutoComplete =
    System.Threading.Thread.CurrentThread.CurrentCulture.Name.StartsWith("tr");
```

---

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
| DataAnnotations | deger tipi + enum sablonu | `Validator.TryValidateObject` |
| Liste uyumu | `Items.Contains` | `DXErrorProvider.SetError` |
| Kalici veri | ADO.NET + Dapper, `@p1` parametreli | - |
| Yasak | EF Core / string-concat SQL | `G2` ihlali |

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `AutoComplete = true` + `DisableTextEditor` | tamamlama hic calismaz | `TextEditStyles.Standard` |
| 2 | Listeyi `TextChanged` icinde yeniden doldurmak | yanip sonme + kayip secim | kaynagi bir kez doldur |
| 3 | `Text` okuyup `EditValue` yazmak | tip karisimi | iki yonde de `EditValue` |
| 4 | 200 kayitli listeyi ComboBoxEdit'e sokmak | yavas, arama yok | `LookUpEdit` (`gridlookupedit.md`) |
| 5 | Dogrulamayi autocomplete'e birakmak | gecersiz deger kabul | §4 zinciri |
