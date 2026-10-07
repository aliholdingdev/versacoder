---
title: "splashscreen-waitform - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "SplashScreenManager ile splash, wait form, overlay ve progress panel secimi; acma-kapama sirasi ve yaygin tuzaklar"
---

# SplashScreen ve WaitForm Gostergeleri

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [async-rules](../../.rules/async-rules.md) · [ribbon-form-basics](ribbon-form-basics.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [dockmanager-panels](dockmanager-panels.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress 4/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) [accordion-navigation](accordion-navigation.md) [bar-status-items](bar-status-items.md) [chartcontrol-basics](chartcontrol-basics.md) [checkedcomboedit-values](checkedcomboedit-values.md) [comboboxedit-autocomplete](comboboxedit-autocomplete.md) [datasource-binding-dapper](datasource-binding-dapper.md) [dateedit-timeedit](dateedit-timeedit.md) [designer-runtime-customization](designer-runtime-customization.md) [docking-saved-layout-xml](docking-saved-layout-xml.md) [docklayout-persistence](docklayout-persistence.md) [dx-validation-provider](dx-validation-provider.md) [grid-conditional-format-rules](grid-conditional-format-rules.md) [grid-editing-modes](grid-editing-modes.md) [grid-export-excel-pdf](grid-export-excel-pdf.md) [grid-filtering-sorting](grid-filtering-sorting.md) [grid-grouping-summaries](grid-grouping-summaries.md) [grid-paging-virtual-mode](grid-paging-virtual-mode.md) [grid-search-panel-find](grid-search-panel-find.md) [gridcontrol-basics](gridcontrol-basics.md) [gridview-columns-format](gridview-columns-format.md) [layoutcontrol-grouping](layoutcontrol-grouping.md) [lookUpEdit-cascading](lookUpEdit-cascading.md) [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) [nuget-references-cpm](nuget-references-cpm.md) [performance-grid-optimization](performance-grid-optimization.md) [ribbon-control-office2010](ribbon-control-office2010.md) [ribbon-pages-groups-items](ribbon-pages-groups-items.md) [skin-customization](skin-customization.md) [tabcontrol-multipage](tabcontrol-multipage.md) [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *Uzun suren is sirasinda kullaniciya hangi gosterge gosterilir, neden kapanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek giris noktasi `DevExpress.XtraSplashScreen.SplashScreenManager`'dir; dort gosterge tipi vardir: **splash** (acilista), **Wait Form** (butun pencereyi bloklar), **Overlay** (tek kontrolu bloklar) ve **ProgressPanel** (satir ici, bloklamaz). Wait/Overlay kendi thread'inde animasyon surer; is `async/await` ile **ana thread'i bloklamadan** yapilir (`Application.DoEvents()` dongusu yasaktir). Acma-kapama eslemezse splash uygulamayi kapanisinda ayakta tutar; her akista `try/finally` ile kapatilir. Host form her zaman `XtraForm` veya `RibbonForm` olmalidir (bizde zaten RibbonForm).

```text
Acilis:  ShowFluentSplashScreen() -> Application.Run(form) -> OnShown: CloseForm()
Uzun is: ShowWaitForm() -> SetWaitFormCaption/Description -> is -> CloseWaitForm()
Tek panel: ShowOverlayForm(ctrl) -> ... -> CloseOverlayForm(handle)
Satir ici: ProgressPanel (Visible = true/false)
```

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraSplashScreen;

splashScreenManager1.ShowWaitForm();
try
{
    splashScreenManager1.SetWaitFormCaption("Disa aktariliyor...");
    await ExportAsync();
}
finally
{
    splashScreenManager1.CloseWaitForm();   // hata olsa da kapanir
}
```

---

## §2 Gosterge karsilastirmasi

| Tip | Neyi bloklar | Thread | Ne zaman |
|---|---|---|---|
| Splash (`ShowFluentSplashScreen`) | Mesaj dongusu baslamadan | Kendi | Uygulama acilisi |
| Wait Form (`ShowWaitForm` / `CloseWaitForm`) | Tum pencere | Kendi | Uzun senkron is |
| Overlay (`ShowOverlayForm` / `CloseOverlayForm`) | Tek kontrol (orn. grid) | Kendi | Arkada veri yukleme |
| ProgressPanel | Hicbirini (inline animasyon) | Ana | Bolge ici geri bildirim |

| Anahtar API | Aciklama |
|---|---|
| `SetWaitFormCaption` / `SetWaitFormDescription` | Baslik/alt yazi; **`ShowWaitForm`'dan SONRA** cagrilir |
| `SplashScreenManager.Default.SendCommand(...)` | WaitForm'a ozel veri gonderir (dogrudan kontrol yazma) |
| `IOverlaySplashScreenHandle` | Overlay kapatma anahtari; `finally`'de verilir |
| `ProgressPanel.WaitAnimationType` | `Default` / `Line` / `Ring` / `Bar` (`WaitingAnimatorType`) |
| `ShowOverlayForm` overload'lari | `OverlayWindowOptions` **veya** isimli parametreler; karistirma |

---

## §3 Yaygin hatalar (tuzaklar)

| # | Tuzak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `CloseForm()` cagirmadan cikmak | Splash uygulamayi ayakta tutar | `OnShown` icinde kapat |
| 2 | `SetWaitFormCaption`'i `ShowWaitForm`'dan once | Etkisiz | Sira: ac -> ayarla -> kapat |
| 3 | WaitForm kontrolunu ana thread'den yazmak | Yanlis/kirilgan | `SendCommand` |
| 4 | `Application.DoEvents()` ile bekleme | Donuk UI, yarim cizim | `async/await` + `Task.Run` |
| 5 | Skin/kayit kodu eklemek (`BonusSkins.Register()` vb.) | Gosterge kapsam disi, karisir | Uygulama skin'i tek noktadan |

---

## §4 Kim neyi degistirir

| Degisiklik | Nereye dokunulur | Etki |
|---|---|---|
| Gosterge tipi | `Show...` / `Close...` cagri cifti | blokama alani degisir |
| Baslik/yazi | `SetWaitFormCaption` / `SetWaitFormDescription` | kullaniciya mesaj |
| WaitForm icerigi | `SendCommand` | dogrudan kontrol yazma |
| Animasyon tipi | `ProgressPanel.WaitAnimationType` | gorunum |
| Gecikme/iptal | akis metodu + `CancellationToken` | ana thread serbest kalir |

---

## §5 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Paket | `DevExpress.Win.Navigation` + `DevExpress.XtraEditors.v26.1` gomye referanslari | Ayni paket, `PackageReference` |
| `async/await` | C# 7.3 async yeterli | C# 14; `await` + `Task.Run` ayni |
| Acilis | `Application.EnableVisualStyles()` | `ApplicationConfiguration.Initialize()` |
| Overlay onkosulu | Handle olusmus olmali (`Form.Load` sonrasi) | Ayni |
| `#if` | Yalniz platform farkinda | Ayni; gosterge kodu `#if` altina ALINMAZ |

---

## §6 Tarama tablosu

| Aranan | Istenen | Yorum |
|---|---|---|
| `ShowWaitForm` | her `CloseWaitForm` ile eslesmeli | `try/finally` |
| `ShowFluentSplashScreen` | `CloseForm()` cagrisi var | yoksa kapanis takilir |
| `Application.DoEvents` | 0 eslesme | async-rules: dongu yasak |
| `SetWaitFormCaption` | `ShowWaitForm`'dan sonra | sira |
| `ShowOverlayForm` | handle `finally`'de kapatiliyor | akis butunlugu |

---

## §7 Kanit ve dogrulama durumu

| Kalem | Durum |
|---|---|
| `ShowWaitForm` / `SetWaitFormCaption` / `CloseWaitForm` / `ShowOverlayForm` / `SendCommand` | v26.1 API referansi ile **dogrulandi** (kaynak vault) |
| `ShowFluentSplashScreen` parametreleri (adlandirilmis argumanlar) | `DOĞRULAMA GEREKLI` (IDE imzasindan dogrula) |
| `dotnet build` | Bu batch'te alinmadi -> `DOĞRULAMA GEREKLI` |
| Kural | DX-13 (ince handler), DX-11 (gosterge Designer'da degil kodda) |

---

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| RibbonForm host | [ribbon-form-basics](ribbon-form-basics.md) |
| Kullanici bildirimi | [alertcontrol-notifications](alertcontrol-notifications.md) |
| Panel gostergesi | [dockmanager-panels](dockmanager-panels.md) |
| Async kurali | [async-rules](../../.rules/async-rules.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\splashscreen-waitform.md` (kaynak vault) | dort gosterge tipi, akis diyagrami, API tablosu, 5 tuzak, TFM farki |
| `.rules\devexpress-rules.md` | DX-11 (gosterge kodda), DX-13 (ince handler) |
| `.rules\async-rules.md` | `async/await`, `DoEvents` yasagi |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
