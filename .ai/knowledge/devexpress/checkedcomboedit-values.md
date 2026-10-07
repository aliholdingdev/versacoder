---
title: "CheckedComboBoxEdit Degerleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "CheckedComboBoxEdit ile cok secimli liste: eleman ekleme, GetCheckedValues/SetEditValue, separator ve dogrulama"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# CheckedComboBoxEdit Degerleri

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [hata kurallari](../../.rules/error-handling-rules.md) · [ADR-005 ado.net dapper](../../.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [dx-validation-provider](dx-validation-provider.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress 5/35):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *CheckedComboBoxEdit'e eleman nasil eklenir, secilen degerler nasil okunur/yazilir ve dogrulanir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Cok secimli kisa listelerin DevExpress v26.1 karsiligi `CheckedComboBoxEdit`'tir. Elemanlar `Properties.Items.Add(deger, aciklama, CheckState, true)` ile eklenir; secilenler `Properties.Items.GetCheckedValues()` ile `object[]` olarak okunur, geri yazilirken `SetEditValue(...)` veya `EditValue` kullanilir. Ayirac `Properties.SeparatorChar` ile belirlenir (varsayilan `,`). Depolamada virgulle ayrilmis tek metin uretmek **string-concat SQL gibi kural kirar**; deger listesi ADO.NET + Dapper ile `@p1` parametreli sorguyla yazilir (EF Core onerilmez).

## §1 Cok secim mekanizmalari

| Ihtiyac | Kontrol | Deger bicimi |
|---|---|---|
| <= 15 sabit secenek | `CheckedComboBoxEdit` | ayirac ile birlesik metin / `object[]` |
| 15+ secenek + arama | `LookUpEdit` cok secim modu | id listesi |
| Serbest + onayli etiket | `TokenEdit` | ayiracli metin |
| Form icerisinde kalici isaret | `CheckEdit` grubu | her deger ayri alan |

> **KURAL:** Cok secimli deger **listesinin** dogrulamasi DTO uzerinde (`[MinLength(1)]`, ozel `ValidateMultiple`) yapilir; kontrolun kendisi yalniz gosterimdir.

## §2 Kod ve ozellik tablosu

```csharp
// FormName.cs - liste doldurma + deger okuma
var items = this.checkedComboBoxEditModules.Properties.Items;
items.Add(1, "Editor", System.Windows.Forms.CheckState.Checked, true);
items.Add(2, "Agent", System.Windows.Forms.CheckState.Unchecked, true);
this.checkedComboBoxEditModules.Properties.SeparatorChar = ',';
object[] secilen = this.checkedComboBoxEditModules.Properties.Items.GetCheckedValues();
```

| Ozellik / metot | Ne yapar | Ornek |
|---|---|---|
| `Properties.Items.Add(deger, aciklama, state, true)` | eleman ekler | `Add(1, "Editor", CheckState.Checked, true)` |
| `Properties.Items.GetCheckedValues()` | isaretli deger dizisi dondurur | `object[] { 1 }` |
| `SetEditValue(object)` | degerleri programatik isaretler | `SetEditValue("1,2")` |
| `Properties.SeparatorChar` | metindeki ayirac | `','` |
| `EditValue` | ham deger (ayiracli metin) | `"1,2"` |
| `Properties.NullText` | hicbir sey secilmediginde gosterim | `"Modul seciniz"` |

```
 +---------------------------+        +---------------------------+
 | Items.Add(...)            | -----> | Popup: isaretli satirlar  |
 +---------------------------+        +-------------+-------------+
                                                      |
                      GetCheckedValues() <------------+
                              |
                              v
                    object[] { 1, 2 }  --> DTO.Liste  --> Dapper (@p1)
                                                     (EF Core onerilmez)
```

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 | C# 14 |
| Dizi donusumu | `object[]` + elle `Convert.ToInt32` | `System.Linq` ile tip donusturme kisa |
| Nullable | nullable uyarisi yok | `object?[]` imasi; `?? Array.Empty<object>()` gerekir |
| Binding redirect | `App.config` zorunlu | gerekmez |
| Satir olcusu | MS Sans Serif 8.25pt | Segoe UI 9pt - popup yuksekligi farkli |

```csharp
#if NETFRAMEWORK
    int id = System.Convert.ToInt32(secilen[0]);   // net48: box'a dokunma
#else
    int id = (int)secilen[0];                      // net10: ayni tip, unboxing net
#endif
```

> **KURAL:** `#if` yalnizca platform API farkinda kullanilir; iki bacakta da calisan donusum icin `#if` yazilmaz.

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class ModuleSelection
{
    [Required(ErrorMessage = "En az bir modul seciniz.")]
    [MinLength(1, ErrorMessage = "En az bir modul seciniz.")]
    public List<int> Modules { get; } = new List<int>();
}
```

```csharp
object[] raw = this.checkedComboBoxEditModules.Properties.Items.GetCheckedValues();
var dto = new ModuleSelection();
foreach (object v in raw) dto.Modules.Add(System.Convert.ToInt32(v));
bool ok = dto.Modules.Count > 0;
this.dxErrorProvider1.SetError(this.checkedComboBoxEditModules,
    ok ? string.Empty : "En az bir modul seciniz.");
```

| Katman | Sorumluluk | Not |
|---|---|---|
| DataAnnotations (`[MinLength]`) | "en az bir" kurali | DTO = tek kaynak |
| `GetCheckedValues()` | UI'dan degeri alir | kontrol |
| `DXErrorProvider` | ikon + mesaj | gorsel |
| Dapper (`@p1`) | kalici yazim | parametreli, `CommandTimeout` setli |

## §5 Iki bacak disi hata ve kararlar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `EditValue` yu virgulle birlestirilmis string sanmak | parse kirilir | `GetCheckedValues()` kullan |
| 2 | Ayiriciyi sonradan degistirmek | eski degerler okunamaz | `SeparatorChar` sabit kalinir |
| 3 | Ayni `deger` iki kez eklemek | yanlis isaretlenir | degerler benzersiz |
| 4 | `SetEditValue` ile isim yazmak | deger gecersiz | id degeri ver |
| 5 | Listeyi her acilista tekrar doldurmak | tekrar eden eleman | tek doldurma noktasi |

## §6 Kodlandirma notu

- Eleman doldurma tek yerde (form acilisi / servis) yapilir.
- `SeparatorChar` dosya boyunca ayni kalir; baska yerde degistirilmez.
- `object[]` donusumu DTO katmaninda yapilir, her yerde tekrarlanmaz.
- Yazma islemi parametreli Dapper sorgusuyla olur; metin birlestirme yok.
- Hata metni `.resx` / kaynak uzerinden verilir.

## §7 Karsilastirma (editor secimi)

| Secenek | Arti | Eksi |
|---|---|---|
| `CheckedComboBoxEdit` | tek satir, hafif | 15+ secenekte arama zayif |
| `LookUpEdit` cok secim | arama kolonu | daha fazla kurulum |
| `TokenEdit` | serbest etiket | deger tipi metin |
| `CheckEdit` grubu | her deger ayri alan | forma sigmaz |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Tek secimli kisa liste | [comboboxedit-autocomplete](comboboxedit-autocomplete.md) |
| Hata ikonu | [dx-validation-provider](dx-validation-provider.md) |
| Veri kaynagi | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Tarih/saat editoru | [dateedit-timeedit](dateedit-timeedit.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `checkedcomboedit-values.md` | `Items.Add` / `GetCheckedValues` / `SetEditValue` / `SeparatorChar` tablosu, `#if` ornegi, dogrulama zinciri, hata tablosu | dogrulandi |
| `comboboxedit-autocomplete.md` | editor secim karsilastirmasi | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
