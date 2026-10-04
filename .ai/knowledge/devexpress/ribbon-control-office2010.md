---
title: "ribbon-control-office2010 - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "RibbonControlStyle.Office2010 tek stil kanunu: hangi davranisi degistirir, hangi ozellikler bu stile bagimlidir, diger stiller neden yasaktir"
---

# RibbonControl Office2010 Stili

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md)

> **Tek cumlelik sorumluluk:** *Ribbon hangi stile sabitlenir, bu stil neyi degistirir ve baska stil neden yazilmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek izinli deger `DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010`'dur; `Office2007`, `Office2013`, `Office2019`, `OfficeUniversal`, `TabletOffice`, `MacOffice` ve `Office2010Blue/Silver` gibi degerler **kural ihlali** sayilir. Stil atamasi Designer'da yapilir, kod dosyasinda tekrarlanmaz. Bir kac ribbon ozelligi **yalnizca belli stillerde etkilidir**; Office2010'da kullanmak hata vermez ama **efekt yoktur**. Icerik (sayfa/grup/oge) stilden bagimsizdir; stil yalnizca **gorsel ve davranis** katmanini belirler.

```text
+-------------------------------------------------------------+
| RibbonControl                                                |
|   +-- RibbonStyle = Office2010     (TEK izinli deger)       |
|   +-- QAT / uygulama butonu / sayfa basliklari -- stile bagli|
|   +-- sayfa/grup/oge icerigi        (stilden bagimsiz)       |
+-------------------------------------------------------------+
```

---

## §1 Stil karsilastirmasi

| Stil | Davranis ozeti | VersaCoder |
|---|---|---|
| `Office2010` | Uygulama butonu + klasik sayfa basliklari, Office2010 skin'i ile tutarli | **IZINLI (tek deger)** |
| `Office2013` | Tam ekran / ribbon goster-gizle butonlari devrede | YASAK |
| `Office2019` | "Simplified" ribbon gecisi ekler | YASAK |
| `OfficeUniversal` | QAT **gizlenir**, tek satir butonlar | YASAK |
| `Office2007` | Yuvarlak uygulama butonu basliga biner | YASAK |
| `TabletOffice` / `MacOffice` | Tablet/Mac ilhamli duzen | YASAK |

Stil degistirmek **sadece gorunumu degil**, QAT konumunu, uygulama butonunu ve grup daraltma davranisini da degistirir; testler tek stile gore yazilmalidir.

---

## §2 Temel kullanim

```csharp
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
        ribbonControl1.AllowMinimizeRibbon = true;   // kullanici ribbon'u kicirebilir
    }
}
```

---

## §3 Ozellik tablosu (stile bagimlilik)

| Ozellik | Tur | Office2010'da etkili mi | Not |
|---|---|---|---|
| `RibbonStyle` | enum | - | Tek atanacak ozellik |
| `ShowApplicationButton` | `bool` | evet | Uygulama butonunu gizler |
| `ApplicationCaption` / `ApplicationDocumentCaption` | `string` | evet | RibbonForm'da iki baslik |
| `ToolbarLocation` | enum | evet | QAT konumu/gizliligi |
| `AllowMinimizeRibbon` | `bool` | evet | Ribbon kucultme |
| `AllowKeyTips` | `bool` | evet | Alt+R klavye ipuclari |
| `ShowFullScreenButton` | `bool` | **hayir (Office2013)** | efekt yoksa `DOĞRULAMA GEREKLI` |
| `ShowPageHeadersInFormCaption` | `bool` | **hayir (OfficeUniversal)** | kullanma |
| `Controller` (`BarAndDockingController`) | nesne | evet | renk/font; `GetController()` ile okunur |
| `AutoSaveLayoutToXml` / `AutoSaveLayoutToXmlPath` | `bool`/`string` | evet | ribbon duzenini kaydeder |

---

## §4 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Stil satiri | ayni: `RibbonControlStyle.Office2010` | ayni |
| Skin yukleme | `Skins.LoadSkin` / `BonusSkins.Register()` proje basinda bir kez | ayni; ek `ApplicationConfiguration.Initialize()` |
| Binding redirect | `App.config` 26.1'e yonlenir | gerekmez |
| High DPI | uygulama manifesti / `App.config` | `Application.SetHighDpiMode(...)` ya da manifest |
| Designer | VS + DevExpress eklentisi, C# 7.3 parser | ayni eklenti; C# 14 parser |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Formda tek `RibbonStyle` degeri var (`Office2010`) | grep = 1 eslesme |
| 2 | Diger stil adlari dosyalarda yok | grep `Office2013|Office2007|OfficeUniversal` = 0 |
| 3 | Stil atamasi Designer'da, kodda tekrar yok | kod incelemesi |
| 4 | Sadece Office2010'da etkisi olmayan ozellikler kullanilmiyor | ozellik listesi kontrolu |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\ribbon-control-office2010.md` (kaynak vault) | tek stil degeri, 6 stil karsilastirmasi, ozellik bagimlilik tablosu, TFM farki |
| `.rules\devexpress-rules.md` | DX-02 (surum), DX-03 (tek stil) |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | surum ve stil karari |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
