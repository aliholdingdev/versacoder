---
title: "DockManager ve Panel Yonetimi - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DockManager ile yan paneller olusturma, konumlandirma, AutoHide/Hidden durumlari ve IDE benzeri duzen"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# DockManager ve Panel Yonetimi

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028 layered baseline]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *IDE ekranindaki sol/sag/alt panellerin hangi kontrolle kuruldugu ve durumlarinin nasil degistirildigi* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

`DockManager` (`DevExpress.XtraBars.Docking`) formdaki tum yan panellerin sahibidir; her panel bir `DockPanel` dir ve `DockingStyle.Left/Right/Bottom/Top` ile kenara yapışir. Panel durumu **uc degerle** oynar: `Visible` (acik), `Hidden` (kapali), `AutoHide` (kenarda sekmeli gizli). Paneller **sabit `Dock` lanamaz**; koordinatlar `DockManager` tarafindan yonetilir, formda `Anchor/Dock` ile ugrasmak yanlistir. IDE ekraninda `DockManager` + `StatusBarControl` birlikte kullanilir.

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Docking;

DockPanel panel = dockManager1.AddPanel(DockingStyle.Left);
panel.Text = "Cozum Gezgini";
panel.Visibility = DockPanelVisibility.AutoHide;   // kenarda gizli sekme
```

```
Form (RibbonForm)
  +------------------------------------------+
  | Ribbon (Dock=Top)                        |
  +--------+------------------------+--------+
  | Sol    |                        | Sag    |
  | panel  |   merkez govde         | panel  |
  |(Auto-  |   (NavigationFrame /   |(Hidden)|
  | Hide)  |    XtraTabControl)     |        |
  +--------+------------------------+--------+
  | StatusBar (Dock=Bottom)                  |
  +------------------------------------------+
   DockManager tum panel konumlarini tutar
```

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `dockManager1.AddPanel(DockingStyle)` | method | Yeni panel olusturur ve kenara yerlestirir |
| `dockManager1.DockPanels` | koleksiyon | Mevcut tum paneller |
| `DockPanel.Text` | `string` | Panel basligi (Caption) |
| `DockPanel.Visibility` | enum | `Visible` / `Hidden` / `AutoHide` |
| `DockPanel.Dock` | `DockingStyle` | Panelin kenari (Left/Right/Top/Bottom) |
| `DockPanel.FloatLocation` / `FloatSize` | `Point`/`Size` | Yuzen panel konumu/olcusu -> **DOĞRULAMA GEREKLI** |
| `barManager1.DockManager` | baglanti | Bar + panel duzenini **ayni yerde** kaydeder |
| `dockManager1.SerializationOptions` | secenek | Kaydet/yukle davranisi (bkz. [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]]) |
| `PanelContainer` | tur | Birbirine yapışik panel gruplari |

## §3 Durum gecisleri

| Durum | Ne gorunur | Gecis |
|---|---|---|
| `Visible` | panel acik, icerik tam | varsayilan |
| `Hidden` | panel kapali, tetik ile acilir | `Visibility = DockPanelVisibility.Hidden` |
| `AutoHide` | kenarda sekmeli gizli | `Visibility = DockPanelVisibility.AutoHide` |
| Yuzen | form ustu serbest pencere | `DockingStyle` disi; `FloatSize` -> **DOĞRULAMA GEREKLI** |

## §4 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Panel kurulumu | Ayni API | Ayni API |
| Panel icerigi (kullanici kontrol) | UserControl uctan olusur | Ayni; `partial` + Designer |
| Konum kaydi yolu | `Application.StartupPath` veya `%LOCALAPPDATA%` | `Environment.SpecialFolder.LocalApplicationData` |
| DPI | panel basligi 16px ikon; manifest DPI | otomatik olcek; `AutoScaleMode` sabit |
| `#if` | yalniz dosya yolu/DPI farkinda | ayni; panel mantigi `#if` altina ALINMAZ |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Panelleri `Dock`/`Anchor` ile sabitlemek | boyutlandirma catirdar, DockManager ile carpisir | `AddPanel(DockingStyle)` |
| 2 | Paneli `new DockPanel()` ile eklemek | yoneticiye kayit yok, duzen kaydedilmez | `dockManager1.AddPanel` |
| 3 | `Visibility` yerine `Visible=false` | beklenen davranis degil (AutoHide ayni kalmaz) | `DockPanelVisibility` |
| 4 | Merkez govdeyi de panele koymak | IDE ortasi kaybolur | merkez dogrudan forma `Dock=Fill` |
| 5 | Panel metnini kodda uretmek | yerellestirme kaybi | Caption Designer/resx |

## §6 Dogrulama ve sinir

| Kural | Yeri | Not |
|---|---|---|
| Panel adlari (XML anahtari) | Designer | sabit tutulur |
| Duzenin kaydi | `SaveLayoutToXml` | bkz. [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Panel ici is | UserControl + servis | `AddPanel` icine gomulmez |
| Durum cubugu | `StatusBarControl` | IDE alt kenari |
| Kalici veri | ADO.NET + Dapper | EF Core onerilmez |

## §7 Karar ozeti

| Soru | Cevap |
|---|---|
| Yan panel kimin? | `DockManager` |
| Sabit `Dock=Fill` panel mi? | yalniz merkez govde |
| Gizleme nasil? | `DockPanelVisibility`, `Visible=false` degil |
| Duzen nasil kalinir? | XML kayit (kardes dosya) |
| Menu mi, panel mi? | menu = `AccordionControl`, panel = `DockManager` |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Duzeni XML'e kaydetme | [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Kayit/yukleme sirasi | [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] |
| Yan menu (panel degil) | [[knowledge/devexpress/accordion-navigation|accordion-navigation]] |
| Durum cubugu | [[knowledge/devexpress/bar-status-items|bar-status-items]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `dockmanager-panels.md` | `AddPanel(DockingStyle)`, `DockPanel.Text`, `SerializationOptions`, hata tablosu, iki bacak | dogrulandi |
| `DockPanelVisibility` enum degerleri | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
| `FloatLocation` / `FloatSize` | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
