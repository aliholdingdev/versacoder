---
title: "SplashScreen ve WaitForm Gostergeleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "SplashScreenManager ile splash / wait form / overlay / progress panel secimi, acma-kapama sirasi ve yaygin tuzaklar"
---

# SplashScreen ve WaitForm Gostergeleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-form-basics](ribbon-form-basics.md) · [alertcontrol-notifications](alertcontrol-notifications.md)

> **Tek cumlelik sorumluluk:** *Uzun suren is sirasinda kullaniciya hangi gosterge gosterilir, neden kapanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek giris noktasi `DevExpress.XtraSplashScreen.SplashScreenManager`'dir; dort gosterge tipi vardir: **splash** (acilista), **Wait Form** (butun pencereyi bloklar), **Overlay** (tek kontrolu bloklar) ve **ProgressPanel** (satir ici, bloklamaz). Wait/Overlay kendi thread'inde animasyon surer; is `async/await` ile **ana thread'i bloklamadan** yapilir (`Application.DoEvents()` dongusu yasaktir). Acma-kapama eslesemezse splash uygulamayi kapanisinda ayakta tutar; her akista `try/finally` ile kapatilir. Host form her zaman `XtraForm` veya `RibbonForm` olmalidir (bizde zaten RibbonForm).

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

```
Acilis:   ShowFluentSplashScreen()  ->  Application.Run(form)  ->  OnShown: CloseForm()
Uzun is:  ShowWaitForm() -> SetWaitFormCaption/Description -> is -> CloseWaitForm()
Tek panel: ShowOverlayForm(ctrl) -> ... -> CloseOverlayForm(handle)
Satir ici:  ProgressPanel (Visible = true/false)
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
| `ProgressPanel.WaitAnimationType` | `Default`/`Line`/`Ring`/`Bar` (`WaitingAnimatorType`) |
| `ShowOverlayForm` overload'lari | `OverlayWindowOptions` **veya** isimli parametreler; karistirma |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Paket | `DevExpress.Win.Navigation` + `DevExpress.XtraEditors.v26.1` gomye referanslari | Ayni paket, `PackageReference` |
| `async/await` | C# 7.3 async yeterli | C# 14; `await` + `Task.Run` ayni |
| Acilis | `Application.EnableVisualStyles()` | `ApplicationConfiguration.Initialize()` |
| Overlay onkosulu | Handle olusmus olmali (`Form.Load` sonrası) | Ayni |
| `#if` | Yalniz platform farkinda | Ayni; gosterge kodu #if altina ALINMAZ |

---

## §4 Yaygin hatalar (tuzaklar)

| # | Tuzak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `CloseForm()` cagirmadan cikmak | Splash uygulamayi ayakta tutar | `OnShown` icinde kapat |
| 2 | `SetWaitFormCaption`'i `ShowWaitForm`'dan once | Etkisiz | Sira: ac -> ayarla -> kapat |
| 3 | WaitForm kontrolunu ana thread'den yazmak | Yanlis/kirilgan | `SendCommand` |
| 4 | `Application.DoEvents()` ile bekleme | Donuk UI, yarim cizim | `async/await` + `Task.Run` |
| 5 | Skin/kayit kodu eklemek (`BonusSkins.Register()` vb.) | Gosterge kapsam disi, karisir | Uygulama skin'i tek noktadan |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `ShowWaitForm` / `SetWaitFormCaption` / `CloseWaitForm` / `ShowOverlayForm` / `SendCommand` | v26.1 API referansi ile **dogrulandi** |
| `ShowFluentSplashScreen` parametreleri (adlandirilmis argumanlar) | `NEEDS_VALIDATION` (IDE imzasindan dogrula) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural | DX-13 (ince handler), DX-11 (gosterge Designer'da degil kodda) |

**Kardesler:** [ribbon-form-basics](ribbon-form-basics.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [dockmanager-panels](dockmanager-panels.md) · [tile-control-startpage](tile-control-startpage.md)
