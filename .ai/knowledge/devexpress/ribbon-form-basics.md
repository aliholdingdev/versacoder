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

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/adr-004-devexpress-universal-v261-office2010.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 3/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *VersaCoder ana penceresi hangi taban siniftan turer ve ribbon hangi tek satirla kilitlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ana pencere `RibbonForm`'dan turer, ribbon stili **yalniz** `RibbonControlStyle.Office2010`'dur. Ribbon, formun ustune `Dock = Top` ile yerlesir; `RibbonStatusBar` altta `Ribbon` baglantisiyla eslesir. Kod dosyasi (`.cs`) yalnizca olay yonlendiricisi tutar, tum yapilandirma Designer'da uretilir. Formun her zaman **uc dosyasi** birlikte olur: `.cs` + `.Designer.cs` + `.resx` (bkz. [../winforms/designer-cs-trio-rule](../winforms/designer-cs-trio-rule.md)).

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
