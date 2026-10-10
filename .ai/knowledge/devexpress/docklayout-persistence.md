---
title: "Dock ve Form Duzeni Kalıcılığı - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DockManager / LayoutControl / Ribbon duzenini XML, Stream veya Registry'ye kaydetme ve geri yukleme sirasi, surum uyumu"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Dock ve Form Duzeni Kalıcılığı

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001 dual target]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Kullanicinin panel/sayfa duzeni kapat-acildiginda nasil geri gelir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

DevExpress kontrolleri duzeni **`SaveLayoutToXml` -> `RestoreLayoutFromXml`** ikilisiyle kaydeder (registry ve stream varyantlari da var; `LoadLayoutFromXml` **yoktur**, dogru ad `RestoreLayoutFromXml`). Kayit, kontroller **hazir olduktan** sonra (`FormClosing`) yapilir, yukleme `Form.Load` sirasinda kontrol olusturulduktan sonra. Ayni XML'e **birden fazla duzen** yazilmaz; coklu duzen icin Workspace Manager gerekir. Kaydedilecek alan `SerializationOptions` (DockManager) / `OptionsLayout` (Grid, Tile) / `OptionsSerialization` (LayoutControl) ile sinirlanir.

## §1 Temel kullanim

```csharp
using System;
using System.IO;
using DevExpress.XtraBars.Docking;

string file = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "VersaCoder", "dock-layout.xml");

dockManager1.SaveLayoutToXml(file);          // FormClosing'de
if (File.Exists(file))
    dockManager1.RestoreLayoutFromXml(file); // Load sonunda, panel olusturulduktan sonra
```

## §2 Kontrol bazli kayit tablosu

| Kontrol | Kaydet | Geri yukle | Ayar nesnesi |
|---|---|---|---|
| `DockManager` | `SaveLayoutToXml/Stream/Registry` | `RestoreLayoutFromXml/Stream/Registry` | `SerializationOptions` |
| `LayoutControl` | `SaveLayoutToXml` | `RestoreLayoutFromXml` | `OptionsSerialization` |
| `TileControl` | `SaveLayoutToXml` | `RestoreLayoutFromXml` | `OptionsLayout` |
| `GridControl` / `TreeList` | `SaveLayoutToXml` | `RestoreLayoutFromXml` | `OptionsLayout` |
| `BarManager` (+ DockManager) | `SaveLayoutToRegistry` | `RestoreLayoutFromRegistry` | `RegistryPath` |
| `RibbonControl` | `AutoSaveLayoutToXml` | `AutoSaveLayoutToXmlPath` | otomatik |

> **SIRA KURALI:** Yukleme, o layout'i olusturan tum kontroller **olusturulduktan sonra** yapilmalidir; aksi halde kayip panel/id eslesmez.

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Kayit yeri | `%LOCALAPPDATA%` (registry de mumkun) | `%LOCALAPPDATA%`; registry ayni calisir |
| Dosya yolu uretimi | `Application.StartupPath` / `SpecialFolder` | `AppContext.BaseDirectory` + `SpecialFolder` |
| Registry yazma | `Microsoft.Win32.Registry` | ayni namespace, uyari seviyesi farkli olabilir |
| XML serializer | `System.Xml` framework | ayni API (`System.Xml`) |
| `#if` | yalniz yolu/registry farkinda | ayni; kayit kodu `#if` altina ALINMAZ |

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `LoadLayoutFromXml` yazmak | derleme hatasi (boyle metod yok) | `RestoreLayoutFromXml` |
| 2 | Konstruktorun basinda yukleme | panel henuz yok, duzen bos | Load sonunda, kontrollerden sonra |
| 3 | XML'i `Program.cs` ye yazmak (Program Files) | yetki hatasi (UAC) | `LocalApplicationData` |
| 4 | Ayni dosyaya birden cok duzen | son yazilan oncekini siler | Workspace Manager |
| 5 | Eski surum XML'ini yukleme | sessiz bozuk duzen | dosya adina surum/ek (v26.1) |

## §5 Karar tablosu (hangi hedef?)

| Ihtiyac | Hedef | Not |
|---|---|---|
| Panel duzeni + suruklenebilirlik | XML (`SaveLayoutToXml`) | oncelikli yol |
| Bar + panel birlikte kayit | Registry (`SaveLayoutToRegistry`) | `BarManager` |
| Ribbon kendiliginden | `AutoSaveLayoutToXml` | ayri kod gerekmez |
| Birden cok adlandirilmis duzen | Workspace Manager | tek dosya tek duzen |
| Belirli alanlari dislamak | `OptionsLayout` / `SerializationOptions` | kapsami daralt |

## §6 Kodlandirma notu

- Yol tek sabitte tanimlanir, her cagrida yeniden kurulmaz.
- Yukleme/kaydetme tek olayda; tekrar eden cagri yok.
- Kayit kapsami bilincli secilir (tum layout degil, gerekli alan).
- Dosya yolu `Path.Combine` ile uretilir.
- Eski surum dosyasi acilista sessizce yok sayilir/temizlenir.

## §7 Dogrulama

| Kalem | Durum |
|---|---|
| Kontrol bazli metot adlari (`OptionsLayout` vb.) | dokuman karsiligi ile **dogrulandi** |
| Dosya yolu izinleri (net48 UAC) | **DOĞRULAMA GEREKLI** - paket testinde dogrula |
| `dotnet build` | bu uretimde alinmadi -> **DOĞRULAMA GEREKLI** |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| DockManager XML zamanlamasi | [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Panel kurulumu | [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |
| Ribbon stili sabitligi | [[knowledge/devexpress/bar-status-items|bar-status-items]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `docklayout-persistence.md` | `SaveLayoutToXml` / `RestoreLayoutFromXml`, kontrol bazli tablo, `SerializationOptions` / `OptionsLayout` / `OptionsSerialization`, sira kurali, hata tablosu | dogrulandi |
| `docking-saved-layout-xml.md` | zamanlama ve dosya konumu | dogrulandi |
| Dosya yolu izinleri (net48 UAC) | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
