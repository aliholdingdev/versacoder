---
title: "StatusBar ve Bar Ogeleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "StatusBarControl + BarStaticItem/BarEditItem kullanimi, guncelleme noktalari, ribbon ile iliski ve iki bacak farki"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# StatusBar ve Bar Ogeleri (v26.1)

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001 dual target]] · [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Durum cubugu ogeleri nerede kurulur, ne zaman guncellenir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Durum cubugu `StatusBarControl` + `BarStaticItem` / `BarEditItem` uclusuyle kurulur; **ogeler Designer'da** eklenir, **caption/durum kodda** guncellenir. Status bar, IDE ekraninin (`DockManager` + `StatusBarControl`) parcasidir ve ribbon'a bagli degildir - yine de tema/stil `Office2010` ailesinden gelir. Guncelleme noktasi **tek handler** olmalidir; oge basina ayri ayri `Caption` yazan kod tekrari tek sorumluluk ilkesini kirar.

## §1 Oge turleri

| Oge | Ne gosterir | Tiklanabilir mi | Tipik kullanim |
|---|---|---|---|
| `BarStaticItem` | sabit metin / sayac | hayir | `"Oturum: X"`, `"12 satir"` |
| `BarEditItem` | editor (kombobox/spin) | evet | filtre, zoom |
| `BarButtonItem` | eylem | evet | `"Yenile"` (nadir) |
| `BarCheckItem` | ac/kapa | evet | `"Auto-fit"` |

```
StatusBarControl
   +-- BarStaticItem bsiSession   (sol: oturum adi)
   +-- BarStaticItem bsiRows      (sag: satir sayisi)
   +-- BarEditItem   beiZoom      (zoom editoru)
   +-- BarButtonItem bsiRefresh   (yenile eylemi)
```

## §2 Anahtar ozellikler

| Ozellik | Ne is yarar | Not |
|---|---|---|
| `BarStaticItem.Caption` | gorunen metin | **kodda guncellenen tek alan** |
| `BarStaticItem.Alignment` | sola/saga hizalama | oge baglantisinda |
| `BarEditItem.EditValue` | editor degeri | `DataSource` ile de beslenir |
| `BarEditItem.Edit` | bagli editor (`RepositoryItem`) | Designer |
| `barManager.Items` | tum bar ogeleri | paylasim kaynagi |

> **KURAL:** Ogeler **tek bir** `BarManager` uzerinde tanimlanir; iki manager'da ayni ad = cakisma.

## §3 Kod (tek guncelleme noktasi)

```csharp
// Olay yonlendirici - tek yerde guncelleme
private void OnSessionLoaded(string name, int rowCount)
{
    bsiSession.Caption = "Oturum: " + name;   // BarStaticItem
    bsiRows.Caption    = rowCount + " satir";  // sag blok
    beiZoom.EditValue  = 100;                 // BarEditItem degeri
}

// Grid odagi degistiginde status bar'i besle
gridView1.FocusedRowObjectChanged += (s, e) => OnRowStatus();
```

| Yapilan | Yapilmayan |
|---|---|
| Designer'da oge ekleme + adlandirma | kodda `new BarStaticItem()` ile kalici oge kurma |
| Tek handler'da toplu guncelleme | her yerde ayri `bsiX.Caption = ...` |
| Durum verisi servisten | status bar icinde sorgu calistirma |

## §4 Ribbon ile iliski

| Soru | Cevap |
|---|---|
| Status bar ribbon'a bagli mi? | Hayir - `RibbonControl` olmadan da calisir |
| Stil ne? | global skin'den gelir; ribbon stili **yalniz** `Office2010` |
| Ribbon stili degisir mi? | Hayir - sabit (Universal 2026 / v26.1) |
| Yerellestirme? | metin `.resx` / kod kaynagi |

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `StatusBarControl` / `BarStaticItem` | v26.1 (net48) | v26.1 (net10) - ayni sinif |
| `Caption` guncelleme | UI thread'de | UI thread'de (`Invoke` gerekirse ayni) |
| Yuksek DPI | `AutoScaleMode` korunur | .NET 10 DPI algisi farkli |
| Dil | **C# 7.3** - `using var` yasak | C# 14; ortak dosya 7.3 kalir |

## §6 Dogrulama ve sinir

| Katman | Gorev | Not |
|---|---|---|
| Status bar | yalniz gosterim (ad, sayac, zoom) | sorgu calistirmaz |
| Servis / repository | veri uretimi | ADO.NET + Dapper, `@p1` |
| ViewModel | guncelleme tetikleyicisi | tek handler buradan cagrilir |
| Erisilebilirlik | okunabilir metin | bkz. [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] |

## §7 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ogeleri kodda `new` ile kurmak | her acilimda tekrar | Designer'da bir kez |
| 2 | Her olayda ayri `Caption` yazmak | dağınık guncelleme | tek `OnSessionLoaded` |
| 3 | Iki `BarManager` kullanmak | oge cakismasi | tek manager |
| 4 | Status bar icinde sorgu calistirmak | UI donmasi | servis + event |
| 5 | Caption'i sabit yazmak | yanlis durum | olayda guncelle |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Klavye/erisilebilirlik | [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] |
| IDE paneli (status bar'in komşusu) | [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] |
| Panel/XML duzeni | [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `bar-status-items.md` | oge turu tablosu, ozellik tablosu, tek guncelleme ornegi, ribbon iliskisi, iki bacak | dogrulandi |
| `accessibility-keyboard-nav.md` | erisilebilirlik baglantisi | dogrulandi |
| DPI altinda ikon olcusu | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
