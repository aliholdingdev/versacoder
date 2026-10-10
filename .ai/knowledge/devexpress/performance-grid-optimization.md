---
title: "performance-grid-optimization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DevExpress v26.1 GridControl performansi: veri yukleme, gorunum ayarlari, iki bacak olcum adimi ve kural tablosu"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Grid Performans Optimizasyonu

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.rules/performance-guidelines|performance-guidelines]] · [[.decisions/accepted/adr-004-devexpress-universal-v261-office2010|ADR-004]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 4/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Grid neden yavaslar ve v26.1'de hangi ayarlar hiz kazandirir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid yavasliginin buyuk kismi **veri yolu**ndadir: her tusda `DataSource` yenileme, gereksiz `PopulateColumns`, editor aciken refresh. Kalan kismi **gorunum yuku**: grup paneli, otomatik filtre satiri, ozet satirlari, gereksiz format rule'lar. Kural: once **olc**, sonra ayar degistir — `BestFitColumns()` ve `RefreshData()` kod icinde tekrarlanmaz, tek noktaya toplanir.

```text
+-------------------------------------------------------------+
| Veri yolu (sorgu -> DataSource)  <- en pahali, once burasi   |
|   -> Kolon uretimi (PopulateColumns)  <- form basina 1 kez   |
|   -> Gorunum (OptionsView/OptionsBehavior) <- ayarla ucuzla  |
|   -> Cizim (DPI/tema)  <- elle cizim YASAK (elleelle cizim)  |
+-------------------------------------------------------------+
```

---

## §1 Darbozag haritasi

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| Acilis yavas | her formda sorgu + `BestFitColumns` | tek yukleme, olculu BestFit |
| Yazarken kasma | `RefreshData()` her `CellValueChanged`'te | olay yerine tek refresh |
| Kaydirma kasma | gorunum yuku (grup/filtre/ozet) | `Options*` kapat |
| Bellek yuksek | cok satir tam yuklu | sanal/sunucu modu (`DOĞRULAMA GEREKLI`) |

---

## §2 Anahtar ozellikler

| Ozellik | Varsayilan | Maliyet | Oneri |
|---|---|---|---|
| `OptionsBehavior.Editable` | `true` | editor yolu her hucrede | raporda `false` |
| `OptionsView.ShowGroupPanel` | `true` | surukleme altyapisi | grup yoksa `false` |
| `OptionsView.ShowAutoFilterRow` | `false` | satir basina ek editor | gerekmiyorsa kapali |
| `OptionsView.ShowFooter` | `false` | ozet hesabi | yalniz ozetli ekranda |
| `DataSource` yenileme | — | **en pahali** | yalniz veri degince |

---

## §3 Kod (olculu yukleme)

```csharp
// 1) Tek yukleme - Dapper parametreli (data-access-rules): tekrar yok
gridControl1.DataSource = _sessions.LoadAll(new { p1 = "Aktif" });
gridView1.PopulateColumns();                      // 1 kez
gridView1.BestFitColumns();                       // 1 kez, elle

// 2) Gorunum yukunu kapat (rapor/IDE ekranlari)
gridView1.OptionsView.ShowGroupPanel    = false;
gridView1.OptionsBehavior.Editable      = false;
gridView1.OptionsView.ShowAutoFilterRow = false;

// 3) Veri degistiyse TEK refresh
gridView1.RefreshData();
```

---

## §4 Kural tablosu

| # | Kural | Kanit |
|---||---|
| 1 | Once olc, sonra degistir, sonra tekrar olc (`Stopwatch`) | olcum kaydi |
| 2 | `PopulateColumns` / `BestFitColumns` **form basina 1 kez** | kod incelemesi |
| 3 | `RefreshData()` yalniz **veri** degistiginde; hucre olaylarinda degil | olay imzasi |
| 4 | Elle hucre cizimi / `CustomDrawCell` **yok** | grep |
| 5 | `Options*` atamalari Designer'da; kodda tekrar yok (DX-10) | kod incelemesi |

Hedef sure degeri (ms) kullaniciyla sabitlenir; kesin esik `DOĞRULAMA GEREKLI`dir.

---

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Grid API | v26.1 net48 | v26.1 net10 - ayni imza |
| Olcum (`Stopwatch`) | ayni | ayni; `TimeSpan` ile karsilastir |
| Bellek/GC | framework GC | .NET 10 GC farkli davranir -> **iki bacakta ayri olc** |
| Dil | **C# 7.3** - `using var` yasak | C# 14; ortak dosya 7.3 kalir |

---

## §6 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti/baglama | [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] |
| Filtre/siralama maliyeti | [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] |
| Ribbon formda grid gorseli | [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] |
| Varsayilan performans kurallari | [[.rules/performance-guidelines|performance-guidelines]] |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\performance-grid-optimization.md` (kaynak vault) | darbozag haritasi, ozellik tablosu, olculu yukleme kodu, iki bacak farki |
| `.rules\devexpress-rules.md` | DX-08 (elle cizim yasagi), DX-10 (Designer) |
| `.rules\performance-guidelines.md` | olc-sonra-degistir ilkesi |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | v26.1 tek surum |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; sanal mod gecis esigi ve hedef ms degeri bu dosyada kilitlenmez.
