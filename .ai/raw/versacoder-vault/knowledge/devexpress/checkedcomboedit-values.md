---
title: "checkedcomboedit-values - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "CheckedComboBoxEdit ile cok secimli liste, checked deger okuma/yazma (GetCheckedValues, SetEditValue), separator ve dogrulama"
---

# checkedcomboedit-values - CheckedComboBoxEdit Degerleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md)

> **Tek cumlelik sorumluluk:** *CheckedComboBoxEdit'e eleman nasil eklenir, secilen degerler nasil okunur/yazilir ve bu degerler nasil dogrulanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Cok secimli kisa listelerin DevExpress v26.1 karsiligi `CheckedComboBoxEdit`'tir. Elemanlar `Properties.Items.Add(deger, aciklama, CheckState, true)` ile eklenir; secilenler `Properties.Items.GetCheckedValues()` ile `object[]` olarak okunur, geri yazilirken `SetEditValue(...)` veya `EditValue` kullanilir. Ayirac `Properties.SeparatorChar` ile belirlenir (varsayilan `,`). Depolamada virgulle ayrilmis tek metin uretmek **string-concat SQL gibi kural kirar**; deger listesi ADO.NET + Dapper ile `@p1` parametreli sorguyla yazilir (EF Core YASAK).

---

## §1 Cok Secim Mekanizmalari

| Ihtiyac | Kontrol | Deger bicimi |
|---|---|---|
| ≤ 15 sabit secenek | `CheckedComboBoxEdit` | ayirac ile birlesik metin / `object[]` |
| 15+ secenek + arama | `LookUpEdit` cok secim modu | id listesi |
| Serbest + onayli etiket | `TokenEdit` (`tokenedit-tags.md`) | ayiracli metin |
| Form icerisinde kalici isaret | `CheckEdit` grubu | her deger ayri alan |

> **KURAL:** Cok secimli deger **listesinin** dogrulamasi DTO uzerinde (`[MinLength(1)]`, ozel `ValidateMultiple`) yapilir; kontrolun kendisi yalniz gosterimdir.

---

## §2 Kod ve Ozellik Tablosu

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

**Diyagram (deger akisi):**

```
 +---------------------------+        +---------------------------+
 | Items.Add(...)            | -----> | Popup: isaretli satirlar  |
 +---------------------------+        +-------------+-------------+
                                                     |
                     GetCheckedValues() <------------+
                             |
                             v
                   object[] { 1, 2 }  --> DTO.Liste  --> Dapper (@p1, @p2)
                                                     (EF Core YASAK)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 | C# 14 |
| Dizi donusumu | `object[]` + elle `Convert.ToInt32` | `System.Linq` ile tip donusturme kisa |
| Nullable | nullable uyarisi yok | `object?[]` imasi; `?? Array.Empty<object>()` gerekir |
| Binding redirect | `App.config` zorunlu (DX-06) | gerekmez |
| Satir olcusu | MS Sans Serif 8.25pt | Segoe UI 9pt - popup yuksekligi farkli |

```csharp
#if NETFRAMEWORK
    int id = System.Convert.ToInt32(secilen[0]);   // net48: box'a dokunma
#else
    int id = (int)secilen[0];                      // net10: ayni tip, unboxing net
#endif
```

> **KURAL:** `#if` yalnizca platform API farkinda kullanilir; iki bacakta da calisan donusum icin `#if` yazılmaz.

---

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
| DataAnnotations (`[MinLength]`) | "en az bir" kurali | DTO = SSOT |
| `GetCheckedValues()` | UI'dan degeri alir | kontrol |
| `DXErrorProvider` | ikon + mesaj | gorsel |
| Dapper (`@p1`) | kalici yazim | parameterized, `CommandTimeout` setli |

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `EditValue`'yu virgulle birlestirilmis string sanmak | parse kirilir | `GetCheckedValues()` kullan |
| 2 | Ayiriciyi sonradan degistirmek | eski degerler okunamaz | `SeparatorChar` sabit, dosya boyunca ayni |
| 3 | Ayni `deger` iki kez eklemek | yanlis isaretlenir | degerler benzersiz |
| 4 | `SetEditValue` ile isim yazmak | deger gecersiz | id degeri ver |
| 5 | Listeyi her acilista tekrar doldurmak | tekrar eden eleman | tek doldurma noktasi |
