---
title: "DXErrorProvider Dogrulama - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DXErrorProvider ile alan dogrulamasi: hata ikonu, Validating akisi, katman siniri ve iki bacak farki"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# DXErrorProvider Dogrulama (v26.1)

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [hata kurallari](../../.rules/error-handling-rules.md) · [ADR-005 ado.net dapper](../../.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) · [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [dateedit-timeedit](dateedit-timeedit.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 5/35):** [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *Alan dogrulamasi hangi olayda, hangi saglayiciyla gosterilir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Alan dogrulamasi `DXErrorProvider` ile yapilir: **`ContainerControl` forma baglanir**, `Validating` olayinda hata metni yazilir, bos metin **temizleme** anlamina gelir. Dogrulama **iki katmandir**: View'da anlik geri bildirim (`SetError`), is kurali servis/ViewModel'de. UI'da `e.Cancel = true` ile gecersiz odak **engellenmez**, yalnizca gorsel uyari verilir - asil kilidi kaydetme butonunun `Enabled` durumu saglar. Surum yalniz v26.1, Ribbon stili yalniz `Office2010`.

## §1 Dogrulama akisi

```
Kullanici alani birakir (Validating)
        |
   kural saglanir mi? --EVET--> SetError(control, "")  (hata temizlenir)
        |
      HAYIR
        |
   SetError(control, "mesaj")  +  ikon (ErrorIconAlignment)
        |
   Kaydet butonu: tum alanlar temizse Enabled = true
```

| Olay | Ne yapilir | Neden kacinilir |
|---|---|---|
| `Validating` | kural + `SetError` | is kuralini burada **cozme** (servis katmani) |
| `Validated` | temiz bilgi | kaydetme islemi |
| `EditValueChanging` | canli maskeleme | kural ile karistirma |

## §2 Anahtar ozellikler

| API / Ozellik | Ne yapar | Not |
|---|---|---|
| `dxErrorProvider1.ContainerControl` | hangi forma ait | **ilk baglanacak** |
| `SetError(control, msg)` | hata yaz/temizle (`""` = temiz) | tek cagri noktasi |
| `SetIconAlignment` | ikon konumu | `ErrorIconAlignment.MiddleRight` |
| `IconAlignment` / `ErrorStyle` | ikon stili | skin'e birak |
| `ClearErrors()` | tumunu temizle | form resetinde |

## §3 Kod (olay + durum)

```csharp
using DevExpress.XtraEditors.DXErrorProvider;

dxErrorProvider1.ContainerControl = this;                       // forma bagla
dxErrorProvider1.SetIconAlignment(txtName, ErrorIconAlignment.MiddleRight);

private void txtName_Validating(object sender, CancelEventArgs e)
{
    bool hatali = string.IsNullOrWhiteSpace(txtName.Text);      // kural
    dxErrorProvider1.SetError(txtName, hatali ? "Oturum adi zorunlu" : "");
    btnSave.Enabled = !hatali;                                  // kilitleme
}
```

| Kural turu | Nerede | Not |
|---|---|---|
| Zorunlu / uzunluk / regex | View (`Validating`) | anlik geri bildirim |
| Tekrar eden ad (DB) | servis (Dapper `@p1`) | parametreli sorgu |
| Capraz alan (bitis > baslangic) | ViewModel | komut `CanExecute` |

## §4 Kurallar (en fazla 5 madde)

1. `ContainerControl` **tek sefer** baglanir; her olayda tekrar baglanmaz.
2. Hata metni **`.resx`/kaynaktan** gelir, literal yazilmaz.
3. Is kurali veritabani kontrolu **ViewModel/serviste** kalir.
4. Yalniz kirmizi ikon ile yetinme: metin + `Tab` sirasi erisilebilir olmali.
5. Kaydetmeden once tum hatalar `ClearErrors()` ile sifirlanir (yanlis kilit kalmasin).

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `DXErrorProvider` sinifi | `DevExpress.XtraEditors` (v26.1 net48) | ayni sinif (v26.1 net10) |
| `Validating` / `CancelEventArgs` | WinForms standart | ayni |
| Ikon hizasi | `ErrorIconAlignment` | ayni (DPI'da ikon olcusu farkli) |
| Dil | **C# 7.3** - `using var`, target-typed `new` **yasak** | C# 14; ortak dosya 7.3 kalir |

## §6 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `ContainerControl` baglamamak | ikon hic gorunmez | forma bagla (ilk is) |
| 2 | `SetError` i her yerden cagirmak | dağınık temizlik | tek guncelleme noktasi |
| 3 | `e.Cancel = true` ile odak kilitlemek | kullanici takilir | yalniz uyari + `Enabled` |
| 4 | Is kuralini `Validating` e gommek | test edilemez | servis/ViewModel |
| 5 | Hati temizlemeyi unutmak | yanlis kilit | gecis = `SetError("", ...)` |

## §7 Karar ozeti

| Ihtiyac | Katman | Arac |
|---|---|---|
| Zorunlu/regex/uzunluk | View | `Validating` + `SetError` |
| Benzersizlik (DB) | servis | Dapper `@p1` |
| Capraz alan | ViewModel | komut `CanExecute` |
| Kalici yazim | repository | ADO.NET + Dapper (EF Core onerilmez) |
| Erisilebilir geri bildirim | View | metin + odak sirasi |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Erisilebilirlik / klavye | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| Tarih araligi kurali | [dateedit-timeedit](dateedit-timeedit.md) |
| Cok secim dogrulamasi | [checkedcomboedit-values](checkedcomboedit-values.md) |
| Kisa liste dogrulamasi | [comboboxedit-autocomplete](comboboxedit-autocomplete.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `dx-validation-provider.md` | akis diyagrami, `ContainerControl` / `SetError` / `SetIconAlignment` / `ClearErrors`, kural katman tablosu, 5 kural, iki bacak | dogrulandi |
| `dateedit-timeedit.md` | `[CustomValidation]` + `SetError` zinciri | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
