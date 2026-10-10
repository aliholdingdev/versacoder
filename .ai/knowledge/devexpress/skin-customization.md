---
title: "skin-customization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Skin, appearance ve format katmanlarinin yazilis sirasi, AppearanceObject ozellikleri, DX-08 ve DPI sinirlari"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Skin ve Appearance Ozellestirme

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]]

> **Tek cumlelik sorumluluk:** *Skin uzerine appearance nasil yazilir ve nerede durulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

DevExpress gorunumu uc katmandan olusur: **skin** (global renk paketi) -> **appearance** (kontrol/gorunum bazli ustune yazma) -> **format** (hucre bicimi, `DisplayFormat`). Ozellestirme hep **en ustten baslar**: once global skin, sonra gerekirse appearance, en son hucre bicimi. Elle hucre cizimi (`CustomDrawCell`) **yasaktir** (DX-08) - DPI ve tema altinda kirilir; renk/font kodu `.Designer.cs`'de degil, kod tarafinda ve tek yerde toplanir.

```text
skin  -->  appearance  -->  format
(genel)    (ustune yaz)    (hucre bazli)
   |             |              |
   +--- hicbiri elle cizim degil: CustomDrawCell YASAK (DX-08)
```

---

## §1 Katman hiyerarsisi

| Katman | Kapsam | Nerede degisir | Ait oldugu rol |
|---|---|---|---|
| Skin | tum uygulama | `UserLookAndFeel.Default` | `ui-designer` |
| Control appearance | tek kontrol | `ctrl.Appearance.*` | `ui-designer` |
| View appearance | gorunum bolgesi (Header, Row, SelectedRow) | `gridView1.Appearance.*` | `ui-designer` |
| Cell format | tek hucre degeri | `GridColumn.DisplayFormat` | `developer` |

---

## §2 Anahtar ozellikler

| Ozellik | Ne is yarar | Tipik deger |
|---|---|---|
| `AppearanceObject.Font` | yazi tipi | `new Font("Segoe UI", 9f)` |
| `AppearanceObject.ForeColor` | yazi rengi | `Color.FromArgb(40, 40, 40)` |
| `AppearanceObject.BackColor` | zemin | `Color.FromArgb(245, 245, 245)` |
| `AppearanceObject.Options` | hangi alan **aktif** (yoksa skin ezer) | `AppearanceOptions.UseFont` |
| `AppearanceObject.Assign(src)` | gorunumu kopyalama/paylasma | `EvenRow.Assign(OddRow)` |
| `gridView1.AppearancePrint.*` | yazdirma/export gorunumu | ayri set - unutulmasin |

> **UYARI:** `Options` **once** acilir, deger **sonra** atanir; tersi yapilirsa skin degeri ezer ve degisiklik gorunmez.

---

## §3 Kod (appearance yazimi)

```csharp
using DevExpress.Utils;

// 1) Once alan maskesini ac, sonra degeri ver (skin uzerine yazar)
gridView1.Appearance.HeaderPanel.Options = AppearanceOptions.UseFont | AppearanceOptions.UseForeColor;
gridView1.Appearance.HeaderPanel.Font  = new Font("Segoe UI", 9f, FontStyle.Bold);
gridView1.Appearance.HeaderPanel.ForeColor = Color.FromArgb(40, 40, 40);

// 2) Ayni gorunumu paylas - tekrar yok (SRP)
gridView1.Appearance.EvenRow.Assign(gridView1.Appearance.OddRow);
```

| Yapilmayan | Neden |
|---|---|
| `CustomDrawCell` ile "stil" | DX-08 ihlali - DPI/tema altinda bozulur |
| Her satirda ayni renk atamasi | tekrar; tema degince unutulur |
| `.Designer.cs` icinde renk kodu | Designer yeniden uretince silinir (DX-10) |

---

## §4 Kurallar

1. Once **skin** dene; appearance yalniz skin karsilamadiginda.
2. `Options` maskesi olmadan renk/font **yazilmaz**.
3. Renkler sabit `Color.FromArgb` yerine **tek sinifda** sabitlenir (dark/light gecisine hazir olsun).
4. Print/export gorunumu ayridir (`AppearancePrint`) - eksik birakilirsa rapor farkli gorunur.
5. Elle cizim (`CustomDrawCell`) yerine appearance + `ColumnView` ozellikleri (DX-08).

---

## §5 Kim neyi degistirir

| Degisiklik | Dokunulacak katman | Ornek |
|---|---|---|
| Tum uygulama rengi | skin (`UserLookAndFeel.Default`) | tema secimi |
| Tek kontrol stili | `ctrl.Appearance.*` | buton fontu |
| Grid baslik/ satir stili | `gridView1.Appearance.*` | `HeaderPanel` |
| Hucre deger bicimi | `GridColumn.DisplayFormat` | `{0:n2}` |
| Kosullu renk | format rule | [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] |
| Yazdirma gorunumu | `AppearancePrint.*` | export/rapor |

---

## §6 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `AppearanceObject` API | ayni | ayni |
| Font adi | `"Segoe UI"` kurulu - net48'te olcu birimi Font (pt) | C#/GDI+ ayni, DPI algisi farkli |
| DPI | `AutoScaleMode` Designer degeri korunur (DX-14) | DPI algisi daha yuksek -> `#if` yalniz DPI API'sinde |
| Dil | **C# 7.3** - `using var`, target-typed `new` yasak | C# 14 yazilabilir; ortak kaynak 7.3'te kalir |

---

## §7 Tarama tablosu

| Aranan | Istenen | Yorum |
|---|---|---|
| `CustomDrawCell` | 0 eslesme | DX-08 |
| `.Designer.cs` icinde `Color.FromArgb` | 0 eslesme | renk kodu kod tarafinda (DX-10) |
| `Appearance...Options` | deger atamadan **once** | yoksa skin ezer |
| `AppearancePrint` | rapor ekrani varsa dolu | export farkli gorunmesin |
| Sabit renk yayilimi | tek sinifda toplanmis | dark/light gecisine hazir |

---

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Global tema/look & feel | [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]] |
| Kolon bicimi (format katmani) | [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] |
| Kosullu bicim (format rule) | [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] |
| Designer/runtime siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\skin-customization.md` (kaynak vault) | uc katman hiyerarsisi, AppearanceObject ozellik tablosu, yazma sirasi uyari, yapilmayanlar tablosu, TFM/DPI farki |
| `.rules\devexpress-rules.md` | DX-08 (elle cizim yasagi), DX-10 (Designer siniri), DX-14 (DPI) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
