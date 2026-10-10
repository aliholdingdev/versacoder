---
title: "Docking Duzeni XML - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DockManager panel duzeninin XML'e kaydi/geri yuklenmesi: SaveLayoutToXml zamanlamasi, dosya konumu ve iki bacak farki"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Docking Duzeni XML (v26.1)

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001 dual target]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *DockManager panel duzeni ne zaman XML'e yazilir, ne zaman geri yuklenir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

`DockManager.SaveLayoutToXml(path)` duzeni yazar, `RestoreLayoutFromXml(path)` geri yukler. **Zamanlama kritik:** geri yukleme `InitializeComponent` ten **sonra**, form gorunmeden **once**; kaydetme `FormClosing` te. Panel adlari (`Name`) sabit tutulmazsa XML'deki panel bulunamaz ve **sessizce yok sayilir**. IDE ekranı `DockManager` + `StatusBarControl` uzerine kurulur; duzen dosyasi kullanici profiline yazilir, repo'ya girmez.

## §1 Zamanlama tablosu

| An | Ne | Kod |
|---|---|---|
| Form ctor | Designer duzeni | `InitializeComponent()` |
| ctor sonu / `OnLoad` | XML'i uygula | `dockManager1.RestoreLayoutFromXml(path)` |
| Kullanici paneli tasiir | - | (runtime durum) |
| `FormClosing` | XML'e yaz | `dockManager1.SaveLayoutToXml(path)` |
| Panel adi degisti | **eski XML'i sil** | yoksa panel kaybolur |

```
InitializeComponent()  -->  RestoreLayoutFromXml()  -->  form GOSTER
                                   |
                     (kullanici panel dizer)
                                   |
                            FormClosing -> SaveLayoutToXml()
```

## §2 Anahtar API'ler

| API / Ozellik | Ne yapar | Not |
|---|---|---|
| `SaveLayoutToXml(string)` | tum panel duzenini yazar | dock/float/auto-hide |
| `RestoreLayoutFromXml(string)` | duzeni geri yukler | dosya yoksa sessizce gec |
| `DockPanel.Name` | XML'deki anahtar | **sabit** tutulmali |
| `DockPanel.Visibility` | `Visible` / `Hidden` / auto-hide | durum XML'e yazilir |
| `DockPanel.FloatSize` | yuzen panel olcusu | DPI'a gore degisir |
| `dockManager1.SerializationOptions` | kaydet/yukle kapsami | bkz. [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] |

## §3 Kod (kaydet + geri yukle)

```csharp
private const string DockLayout = "dock-layout.xml";   // profil klasoru - §4

// Form acilisinda - InitializeComponent'ten SONRA
if (File.Exists(DockLayout))
    dockManager1.RestoreLayoutFromXml(DockLayout);

// Form kapanisinda - tek cagri
private void OnFormClosing(object sender, FormClosingEventArgs e)
    => dockManager1.SaveLayoutToXml(DockLayout);
```

| Durum | Sonuc | Onlem |
|---|---|---|
| Dosya yok | ilk acilista Designer duzeni | normal |
| Panel `Name` degisti | panel kaybolur (eski XML) | acilista XML'yi sil |
| Surum degisimi (26.1 -> 26.2) | sema farki riski | **DOĞRULAMA GEREKLI** - sema uyumu test edilir |

## §4 Dosya konumu

| Kaynak | Yol (net48 ve net10 ortak) |
|---|---|
| Kullanici duzeni | `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VersaCoder", "dock-layout.xml")` |
| Gecici test | `Path.GetTempPath()` |
| Repo ici | **yok** (`.gitignore`) |

> **YASAK:** sabit `C:\...` yol; string-concat ile yol/SQL uretimi.

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `SaveLayoutToXml` / `RestoreLayoutFromXml` | v26.1 (net48) | v26.1 (net10) - ayni imza |
| XML semasi | surumle eslesmeli | ayni surum -> ayni sema |
| `ApplicationData` yolu | `%APPDATA%\VersaCoder` | ayni (API ortak) |
| Dil | **C# 7.3** - `using var` yasak | C# 14; ortak dosya 7.3 kalir |

## §6 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `LoadLayoutFromXml` yazmak | boyle metod yok, derleme hatasi | `RestoreLayoutFromXml` |
| 2 | Gosterden once kaydetme | bos/eksik duzen | `FormClosing` te kaydet |
| 3 | Panel `Name` degistirmek | sessizce panel kaybi | ad sabit; gerekirse XML sil |
| 4 | XML'i repo icine yazmak | kirli diff | `%APPDATA%` |
| 5 | Ayni XML'e baska kontrolun duzenini yazmak | cakisan sema | ayri dosya adlari |

## §7 Kodlandirma notu

- Dosya yolu tek sabitte; her yerde tekrarlanmaz.
- Kaydetme tek olayda (`FormClosing`), cagri tekrari yok.
- Yol uretimi `Path.Combine` ile; betik/icinde sabit surat yok.
- Acilista dosya kontrolu `File.Exists` ile yapilir.
- Panel adlari kod ile degistirilmez; Designer'da sabitlenir.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Kontrol bazli kayit (Grid/Layout/Ribbon) | [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] |
| Panel kurulumu | [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |
| Durum cubugu (komşu) | [[knowledge/devexpress/bar-status-items|bar-status-items]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `docking-saved-layout-xml.md` | zamanlama tablosu, `SaveLayoutToXml` / `RestoreLayoutFromXml`, `DockPanel.Name` anahtari, dosya konumu tablosu, iki bacak | dogrulandi |
| `docklayout-persistence.md` | kontrol bazli kayit eslesmesi | dogrulandi |
| XML sema uyumu (surum degisimi) | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
