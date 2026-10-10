---
title: "ribbon-control-office2010 - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "RibbonControlStyle.Office2010 tek stil kanunu: hangi davranisi degistirir, hangi ozellikler bu stile bagimlidir, diger stiller neden yasaktir"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# RibbonControl Office2010 Stili

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.decisions/accepted/adr-004-devexpress-universal-v261-office2010|ADR-004]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 3/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

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
