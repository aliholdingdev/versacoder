---
title: "Dock ve Form Duzeni Kalıcılığı - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "DockManager / LayoutControl / Ribbon duzenini XML, Stream veya Registry'ye kaydetme ve geri yukleme sirasi, surum uyumu"
---

# Dock ve Form Duzeni Kalıcılığı

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [kodlama standartlari](../../.rules/coding-standards.md) · [ADR-001 dual target](../../.decisions/accepted/adr-001-dual-target-net48-net10.md) · [dockmanager-panels](dockmanager-panels.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 5/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| DockManager XML zamanlamasi | [docking-saved-layout-xml](docking-saved-layout-xml.md) |
| Panel kurulumu | [dockmanager-panels](dockmanager-panels.md) |
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |
| Ribbon stili sabitligi | [bar-status-items](bar-status-items.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `docklayout-persistence.md` | `SaveLayoutToXml` / `RestoreLayoutFromXml`, kontrol bazli tablo, `SerializationOptions` / `OptionsLayout` / `OptionsSerialization`, sira kurali, hata tablosu | dogrulandi |
| `docking-saved-layout-xml.md` | zamanlama ve dosya konumu | dogrulandi |
| Dosya yolu izinleri (net48 UAC) | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
