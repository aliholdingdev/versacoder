---
title: "ribbon-form-basics - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DevExpress Universal 2026 (v26.1) ana pencere kalibi: RibbonForm tabani, RibbonControl + RibbonStatusBar eslesmesi ve Office2010 stili satiri"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# RibbonForm Temelleri

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.decisions/accepted/adr-004-devexpress-universal-v261-office2010|ADR-004]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 3/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *VersaCoder ana penceresi hangi taban siniftan turer ve ribbon hangi tek satirla kilitlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ana pencere `RibbonForm`'dan turer, ribbon stili **yalniz** `RibbonControlStyle.Office2010`'dur. Ribbon, formun ustune `Dock = Top` ile yerlesir; `RibbonStatusBar` altta `Ribbon` baglantisiyla eslesir. Kod dosyasi (`.cs`) yalnizca olay yonlendiricisi tutar, tum yapilandirma Designer'da uretilir. Formun her zaman **uc dosyasi** birlikte olur: `.cs` + `.Designer.cs` + `.resx` (bkz. [[knowledge/winforms/designer-cs-trio-rule|../winforms/designer-cs-trio-rule]]).

```text
+----------------------------------------------------------+
| MainForm : RibbonForm                                    |
| +------------------------------------------------------+ |
| | RibbonControl  Dock=Top   RibbonStyle = Office2010    | |
| |   QAT | sayfa basliklari | uygulama butonu            | |
| |   RibbonPage -> RibbonPageGroup -> item               | |
| +------------------------------------------------------+ |
| | govde (NavigationFrame / XtraTabControl)              | |
| +------------------------------------------------------+ |
| | RibbonStatusBar   Ribbon = ribbonControl1             | |
+----------------------------------------------------------+
```

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
        ribbonStatusBar1.Ribbon = ribbonControl1;   // durum cubugu ribbon'a baglanir
    }
}
```

---

## §2 Ozellik tablosu

| Nesne | Tur | Ne is yapar | Designer notu |
|---|---|---|---|
| `MainForm` | `RibbonForm` | Ana pencere; QAT basliga gomulur, iki baslik tasinir | formu Ribbon Form olarak ekle, uc dosya otomatik olur |
| `ribbonControl1` | `RibbonControl` | Sayfa/grup/oge sahibi, `RibbonStyle` burada | `RibbonStyle` = `Office2010` |
| `ribbonStatusBar1` | `RibbonStatusBar` | Alt durum cubugu; `Ribbon` ozelligi ile baglanir | `Ribbon` = `ribbonControl1` |
| `applicationMenu1` | `ApplicationMenu` | Uygulama butonunun menusu | ust seviye komutlar burada |
| `ribbonControl1.Manager` | `BarManager` | Oge kimlikleri (Id) uretimi | elle `Id` yazma; `GetNewItemId` kullanilir |
| `controller1` | `BarAndDockingController` | Renk/font/look-and-feel ayari | opsiyonel; `RibbonControl.Controller`'a atanir |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Program baslangici | `Application.EnableVisualStyles()` + `SetCompatibleTextRenderingDefault(false)` | `ApplicationConfiguration.Initialize()` |
| Paket/versiyon | CPM (`Directory.Packages.props`) tek kaynak; `App.config` binding redirect **yalniz** 26.1 | CPM ayni; redirect gerekmez |
| Dil surumu | C# **7.3** | C# **14** |
| Nullable | proje genelinde kapaliysa `?` uyarisi vermez | `Nullable=enable` ise `Ribbon` null kontrolu ister |
| `#if` kullanimi | yalniz platform API farkinda | ayni; is mantigi `#if` altina alinmaz |

Iki bacakta da ayni `.Designer.cs` calisir; fark yalniz baslangic/kurulum katmanidir.

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ana pencere `XtraForm` | QAT baslik disinda kalir | `RibbonForm`'a cevir |
| 2 | `RibbonStyle = Office2013` vb. | stil kurali ihlali | `RibbonControlStyle.Office2010` |
| 3 | Kodda `Pages.Add` dongusu ile sayfa kurmak | Designer her acilista yapiyi siler | yapiyi Designer'da kur |
| 4 | `.cs` tek basina form olusturmak | form Designer'da acilmaz | uc dosyayi tamamla |
| 5 | `barManager1` + `ribbonControl1` ayni formda | iki yonetici carpisir, olaylar calismaz | tek yonetici: `RibbonControl` |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Ana pencere `RibbonForm` | kalip kod |
| 2 | `RibbonStatusBar.Ribbon` baglisi var | kod incelemesi |
| 3 | Tek `RibbonStyle` degeri | grep |
| 4 | `.cs`'te yapilandirma dongusu yok | grep `Pages.Add` |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\ribbon-form-basics.md` (kaynak vault) | taban sinif, ASCII duzen, ozellik tablosu, TFM farki, 5 hata |
| `.rules\devexpress-rules.md` | DX-04 (RibbonForm), DX-05 (uc dosya), DX-10 (Designer) |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | surum karari |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
