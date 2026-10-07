---
title: "togglecheck-edit - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "ToggleSwitch ve CheckEdit ile ikili/uclu secim - IsOn/Checked/CheckState, OnText/OffText, grup davranisi ve dogrulama"
---

# togglecheck-edit - ToggleSwitch ve CheckEdit

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [dx-validation-provider](dx-validation-provider.md)

> **Tek cumlelik sorumluluk:** *Ikili durum icin ToggleSwitch mi CheckEdit mi secilir, durumun dogru ozelligi nedir ve deger nasil DTO'ya gecirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Aninda gorev acma/kapama gibi **etiketli ikili durum** icin `ToggleSwitch` (`IsOn`), form satirindaki **onay kutusu** icin `CheckEdit` (`Checked`) kullanilir; ikisi de `EditValue` ile eszamanlidir. Icinde metin gosterilecekse `ToggleSwitch.Properties.OnText` / `OffText` (ve `ShowText`) ayarlanir. Uc durumlu gereksinim (evet/hayir/bilinmiyor) ikili kontrollerle **zorlanmaz** - `ComboBoxEdit` ya da `RadioGroup` kullanilir. Durumun is kurali DTO'da `bool` + DataAnnotations ile, gorsel geri bildirim `DXErrorProvider` ile durur.

---

## §1 Kontrol Secimi

| Durum | Kontrol | Ozellik |
|---|---|---|
| Etiketli anahtar, ayar satiri | **`ToggleSwitch`** | `IsOn`, `Properties.OnText/OffText` |
| Formda onay kutusu | **`CheckEdit`** | `Checked`, `Properties.Caption` |
| Ayni grupta tek secim | `CheckEdit` grubu / `RadioGroup` | biri her zaman secili |
| Uc durum | `ComboBoxEdit` | bkz. `comboboxedit-autocomplete.md` |
| Coklu onay | `CheckedComboBoxEdit` | bkz. `checkedcomboedit-values.md` |

> **KURAL:** `ToggleSwitch` ve `CheckEdit` is mantigi **yazmaz**; yalnizca durumu okur/yazar. Karar veren katman `Application` (SOL-01 / DX-13).

---

## §2 Kod ve Ozellik Tablosu

```csharp
// FormName.cs - durum okuma/yazma
this.toggleSwitchAutoRun.Properties.ShowText = true;
this.toggleSwitchAutoRun.Properties.OnText = "Acik";
this.toggleSwitchAutoRun.Properties.OffText = "Kapali";
this.toggleSwitchAutoRun.IsOn = true;            // EditValue ile eszamanli

bool otomatik = this.checkEditNotifications.Checked;
this.checkEditNotifications.Properties.Caption = "Bildirim gonder";
```

| Ozellik | Kontrol | Ne yapar | Ornek |
|---|---|---|---|
| `IsOn` | ToggleSwitch | durum (`bool`) | `true` |
| `EditValue` | ikisi | durumun nesne hali | `true` / `null` |
| `Properties.ShowText` | ToggleSwitch | On/Off metnini goster | `true` |
| `Properties.OnText` / `OffText` | ToggleSwitch | durum etiketleri | `"Acik"` / `"Kapali"` |
| `Checked` | CheckEdit | isaret durumu (`bool`) | `false` |
| `CheckState` | CheckEdit | `Unchecked/Checked/Indeterminate` | `CheckState.Unchecked` |
| `Properties.Caption` | CheckEdit | yanindaki metin | `"Bildirim gonder"` |

**Diyagram (durum hatti):**

```
 ToggleSwitch.IsOn  ----+                     +--> DTO.Enabled : bool
                        +--> EditValue ------+
 CheckEdit.Checked ----+                     +--> [Range/Required] + DXErrorProvider
                                                          |
                                                  Dapper @p1 (parameterized)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 | C# 14 |
| Nullable | `bool?` uyari vermez | `<Nullable>enable` ise `bool?` `HasValue` ister |
| Binding redirect | `App.config` (DX-06) | gerekmez |
| Varsayilan font | MS Sans Serif 8.25pt | Segoe UI 9pt |
| ToggleSwitch genisligi | dar | etiketli (`ShowText`) modda genislesir |

```csharp
// Iki bacakta da ayni davranis - #if gerekmez
this.toggleSwitchAutoRun.EditValue = dto.Enabled;
dto.Enabled = this.toggleSwitchAutoRun.IsOn;
```

Tek platform farki olcu katmanindadir: `ShowText = true` iken `ToggleSwitch` **`AutoSize`** birakilmali, sabit `Width` verilmemelidir (DX-14).

---

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class RunOptions
{
    [Required(ErrorMessage = "Bildirim tercihi secilmeli")]
    public bool? Notifications { get; set; }   // null = kullanici hic isaretlemedi
}
```

```csharp
this.checkEditNotifications.Checked = dto.Notifications ?? false;
bool kararVerildi = dto.Notifications.HasValue;
this.dxErrorProvider1.SetError(this.checkEditNotifications,
    kararVerildi ? string.Empty : "Bildirim tercihini isaretleyiniz.");
```

| Katman | Arac | Not |
|---|---|---|
| Kural | DataAnnotations (`[Required]` `bool?` uzerinde) | DTO = SSOT |
| Calistirma | `Validator.TryValidateObject` | test edilebilir |
| Gorsel | `DXErrorProvider.SetError` | ikon + mesaj |
| Kalici veri | ADO.NET + Dapper `@p1` (`bit`) | **EF Core YASAK** |
| Yazma | `EditValue`/`IsOn` -> DTO -> repository | iki yonlu tek kaynak |

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `EditValue`'yu `bool` sanip `Checked` okumak | ToggleSwitch'te tip hatasi | kontrole gore `IsOn`/`Checked` |
| 2 | Durumu olay icinde is mantigina cevirmek | god-form (DX-13) | handler ince kalsin |
| 3 | `bool` ile "hic secilmedi" ayrimi yapilmak | varsayilan `false` sessiz kayit | DTO'da `bool?` |
| 4 | Sabit `Width` + `ShowText` | net10'da metin kesilir | `AutoSize` + `Anchor` |
| 5 | Iki bacakta farkli varsayilan deger | test surprizi | varsayilan DTO'da, UI'da degil |
