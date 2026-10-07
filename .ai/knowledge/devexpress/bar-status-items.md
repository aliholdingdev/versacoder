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

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [kodlama standartlari](../../.rules/coding-standards.md) · [ADR-001 dual target](../../.decisions/accepted/adr-001-dual-target-net48-net10.md) · [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [dockmanager-panels](dockmanager-panels.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 5/35):** [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [docklayout-persistence](docklayout-persistence.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Erisilebilirlik | okunabilir metin | bkz. [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |

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
| Klavye/erisilebilirlik | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| IDE paneli (status bar'in komşusu) | [dockmanager-panels](dockmanager-panels.md) |
| Panel/XML duzeni | [docking-saved-layout-xml](docking-saved-layout-xml.md) |
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `bar-status-items.md` | oge turu tablosu, ozellik tablosu, tek guncelleme ornegi, ribbon iliskisi, iki bacak | dogrulandi |
| `accessibility-keyboard-nav.md` | erisilebilirlik baglantisi | dogrulandi |
| DPI altinda ikon olcusu | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
