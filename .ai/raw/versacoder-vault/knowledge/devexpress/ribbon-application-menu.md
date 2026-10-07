---
title: "Ribbon Uygulama menusu (ApplicationMenu) - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Office2010 ribbon'da uygulama butonu menusu: ApplicationMenu vs BackstageViewControl secimi, atama, son dosyalar ve cikis olayi"
---

# Ribbon Uygulama menusu (ApplicationMenu)

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-qat-quick-access](ribbon-qat-quick-access.md)

> **Tek cumlelik sorumluluk:** *Uygulama butonuna basinca ne acilir, o menuye komut nasil eklenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Uygulama butonu, ribbon'daki tek "dosya menusu" noktasi iki secenek kabul eder: **`ApplicationMenu`** (sinirli boyutlu klasik panel) ve **`BackstageViewControl`** (tam ekran, Office 2010 tarzi sayfa). v26.1 WinForms'ta ikisi de `RibbonControl`'a atamayla calisir; stil `Office2010` oldugunda butonun davranisi bu atamaya gore belirlenir. Menu icerigi Designer'da kurulur (Son Dosyalar listesi + Aynen Kaydet / Farkli Kaydet / Cikis gibi `BarItem`'ler); kod yalnizca secilen komutu isler. `RibbonForm` kullanildiginda QAT basliga gomuldugu icin menu ile cakisan tek eleman uygulama butonudur.

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
        ribbonControl1.ApplicationMenu = applicationMenu1;  // uygulama butonu menusu
    }

    private void OnBarItemExitItemClick(object sender, ItemClickEventArgs e) => Close();
}
```

---

## §2 Secenek ve ozellik tablosu

| Secenek | Tur | Ne sunar | Ne zaman |
|---|---|---|---|
| `ApplicationMenu` | bileşen | Sutunlu klasik panel + son dosyalar listesi | Klasik "File" menusu istendiginde |
| `BackstageViewControl` | bileşen | Tam ekran sayfa (sekmeler, buyuk butonlar) | Office 2010 tarzi tam sayfa menusu |
| `RibbonControl.ApplicationMenu` | property | `ApplicationMenu`'yu hizli atar | Designer'in urettigi klasik baglanti |
| `RibbonControl.ApplicationButtonDropDownControl` | property | Menu **veya** Backstage kabul eden tek atama noktasi | Belirsiz/karma durumlarda |
| `RibbonControl.ShowApplicationButton` | `bool` | Butonu gizler (menusuz UI) | Menusuz ekranlarda |

| Menu ici eleman | Tur | Not |
|---|---|---|
| `BarItem` (Aynen Kaydet, Cikis...) | oge | `ribbonControl1.Items` icinde, tek `ItemClick` ile islenir |
| Son dosyalar listesi | `ApplicationMenu` ozelligi | Dosya yolunu gecen kod test edilmelidir |
| Menu basligi/gorseli | `ApplicationMenu` ozellikleri | `NEEDS_VALIDATION`: tam property adlari designer'dan dogrulanir |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Menu atamasi | Ayni: `ApplicationMenu` | Ayni |
| Son dosya yolu | `Application.StartupPath` / `Environment.GetFolderPath` | Ayni; `AppContext.BaseDirectory` da kullanilabilir |
| Kayit (registry) yazimi | `Microsoft.Win32.Registry` (yalniz Windows) | `Microsoft.Win32.Registry` paketle gelir |
| Ayrilma (`#if`) | Yalniz registry/IO API farkinda | Ayni; menu mantigi #if altina ALINMAZ |
| Cikis akisi | `Close()` → `FormClosing` → `Dispose` | Ayni; `Application.Exit()` yalniz tum uygulamada |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Menüyü kodda `new` ile olusturup forma eklemek | Designer ile carpisma, her acilimda kayip | Designer + atama |
| 2 | Cikis komutunu menu gorselinde yazmak | Yerellestirme kirilir | Metin resx/Designer'da |
| 3 | `MessageBox`'i menu olayinin icine gommek | Incelemeden kacan UI (DX-11) | Olay → servis/handler |
| 4 | Menüye is kurali koymak | Test edilemez | `Application` katmanina tasima |
| 5 | Stili `Office2013` yapip Backstage beklemek | G3 ihlali | `Office2010` + dogru menu tipi |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `RibbonControl.ApplicationMenu` atamasi | v26.1 ornekleriyle uyumlu |
| `ApplicationButtonDropDownControl` kabul ettigi tam tipler | `NEEDS_VALIDATION` (VS designer'da dogrula) |
| Son dosyalar listesi property adlari | `NEEDS_VALIDATION` |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural | DX-03 (tek stil), DX-13 (ince handler) |

**Kardesler:** [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [alertcontrol-notifications](alertcontrol-notifications.md)
