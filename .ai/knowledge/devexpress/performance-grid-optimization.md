---
title: "performance-grid-optimization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "DevExpress v26.1 GridControl performansi: veri yukleme, gorunum ayarlari, iki bacak olcum adimi ve kural tablosu"
---

# Grid Performans Optimizasyonu

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [performance-guidelines](../../.rules/performance-guidelines.md) · [ADR-004](../../.decisions/accepted/adr-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 4/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Grid iskeleti/baglama | [gridcontrol-basics](gridcontrol-basics.md) |
| Filtre/siralama maliyeti | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Ribbon formda grid gorseli | [ribbon-form-basics](ribbon-form-basics.md) |
| Varsayilan performans kurallari | [performance-guidelines](../../.rules/performance-guidelines.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\performance-grid-optimization.md` (kaynak vault) | darbozag haritasi, ozellik tablosu, olculu yukleme kodu, iki bacak farki |
| `.rules\devexpress-rules.md` | DX-08 (elle cizim yasagi), DX-10 (Designer) |
| `.rules\performance-guidelines.md` | olc-sonra-degistir ilkesi |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | v26.1 tek surum |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; sanal mod gecis esigi ve hedef ms degeri bu dosyada kilitlenmez.
