---
title: "RibbonControl Office2010 Stili - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "RibbonControlStyle.Office2010 tek stil kanunu: hangi davranisi degistirir, hangi ozellikler bu stile bagimlidir, diger stiller neden yasaktir"
---

# RibbonControl Office2010 Stili

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-application-menu](ribbon-application-menu.md)

> **Tek cumlelik sorumluluk:** *Ribbon hangi stile sabitlenir, bu stil neyi degistirir ve baska stil neden yazilmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek izinli deger `DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010`'dur; `Office2007`, `Office2013`, `Office2019`, `OfficeUniversal`, `TabletOffice`, `MacOffice` ve `Office2010Blue/Silver` gibi degerler **G3 ihlali** sayilir. Stil atamasi Designer'da yapilir, kod dosyasinda tekrarlanmaz. Bir kac Ribbon ozelligi **yalnizca belli stillerde etkilidir** (Or: tam ekran butonu Office2013'e bagimlidir); bu ozellikleri Office2010'da kullanmak hata vermez ama **efekt yoktur**. Icerik (sayfa/grup/oge) stilden bagimsizdir; stil yalnizca **gorsel ve davranis** katmanini belirler.

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        // DX-03: satirdaki tek izinli deger
        ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
        ribbonControl1.AllowMinimizeRibbon = true;   // kullanici ribbon'u kicirebilir
    }
}
```

---

## §2 Stil karsilastirmasi (neden yalnizca Office2010)

| Stil | Davranis ozeti | VersaCoder |
|---|---|---|
| `Office2010` | Uygulama butonu + klasik sayfa basliklari; Office2010 skin'i ile tutarli | **IZINLI (tek deger)** |
| `Office2013` | Tam ekran / ribbon goster-gizle butonlari devrede | YASAK |
| `Office2019` | "Simplified" ribbon gecisi ekler | YASAK |
| `OfficeUniversal` | QAT **gizlenir**, tek satir butonlar | YASAK |
| `Office2007` | Yuvarlak uygulama butonu basliga biner | YASAK |
| `TabletOffice` / `MacOffice` | Tablet/Mac ilhamli duzen | YASAK |

> **UYARI:** Stil degistirmek **sadece gorunumu degil**, QAT konumunu, uygulama butonunu ve grup daraltma davranisini da degistirir; testler tek stile gore yazilmalidir.

---

## §3 Ozellik tablosu (stile bagimlilik)

| Ozellik | Tip | Office2010'da etkili mi | Not |
|---|---|---|---|
| `RibbonStyle` | enum | — | Tek atanacak property |
| `ShowApplicationButton` | `bool` | Evet | Uygulama butonunu gizler |
| `ApplicationCaption` / `ApplicationDocumentCaption` | `string` | Evet | RibbonForm'da iki baslik |
| `RibbonCaptionAlignment` | enum | Evet | RibbonForm baslik hizasi |
| `ToolbarLocation` | enum | Evet | QAT konumu/gizliligi → [ribbon-qat-quick-access](ribbon-qat-quick-access.md) |
| `AllowMinimizeRibbon` / `AllowMinimizeRibbonOnDoubleClick` | `bool` | Evet | Ribbon kucultme |
| `AllowKeyTips` | `bool` | Evet | Alt+R klavye ipuclari |
| `ShowFullScreenButton`, `ShowDisplayOptionsMenuButton` | `bool` | **Hayir (Office2013)** | `NEEDS_VALIDATION`: efekt yoksa kaldir |
| `ShowPageHeadersInFormCaption` | `bool` | **Hayir (OfficeUniversal)** | Kullanma |
| `Controller` (`BarAndDockingController`) | nesne | Evet | Renk/font; `GetController()` ile okunur |
| `AutoSaveLayoutToXml` / `AutoSaveLayoutToXmlPath` | `bool`/`string` | Evet | Ribbon duzenini kaydeder |

---

## §4 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Stil satiri | Ayni: `RibbonControlStyle.Office2010` | Ayni |
| Skin yukleme | `Skins.LoadSkin` / `BonusSkins.Register()` proje basinda bir kez | Ayni; ek `ApplicationConfiguration.Initialize()` |
| Binding redirect | `App.config` 26.1'e yonlendirilir (DX-06) | Gerekmez |
| High DPI | Uygulama manifesti / `App.config` | `Application.SetHighDpiMode(...)` ya da manifest; `AutoScaleMode` Designer'da (DX-14) |
| Designer | VS + DevExpress eklentisi, C# 7.3 parser | Ayni VS eklentisi; C# 14 parser |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| Ozellik adlari (`RibbonStyle`, `ToolbarLocation`, `AutoSaveLayoutToXml`) | v26.1 ozellik listesi ile dogrulandi |
| Stile bagimli ozellik efektleri | `NEEDS_VALIDATION` (Office2010'da efekt gozuyle dogrulanmali) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural | DX-02 (surum sapmasi 0) + DX-03 (tek stil) |

**Kardesler:** [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [ribbon-qat-quick-access](ribbon-qat-quick-access.md) · [ribbon-context-categories](ribbon-context-categories.md)
