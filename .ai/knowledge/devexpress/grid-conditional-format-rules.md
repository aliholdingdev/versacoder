---
title: "Grid Kosullu Biçimlendirme Kurallari - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "GridFormatRule ile kosullu bicimlendirme: Value/Expression/ColorScale/DataBar/IconSet ve RowCellStyle, elle hucre boyama yasagi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Grid Kosullu Biçimlendirme Kurallari (v26.1)

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005 ado.net dapper]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/bar-status-items|bar-status-items]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Kosullu renk/gorunum kurali nerede tanimlanir, elle hucre boyama neden yasaktir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Kosullu bicim `GridView.FormatRules` koleksiyonuna eklenen **`GridFormatRule`** nesneleriyle tanimlanir; her kural bir `FormatConditionRule*` + bir hedef sutun icerir. Kural veriyi **degistirmez**, yalniz gorunumu degistirir - filtre degildir. Elle `CustomDrawCell` ile renk vermek **yasaktir**; DPI/tema altinda bozulur, kural motifini de gorunmez kilir. Surum yalniz v26.1, Ribbon stili yalniz `Office2010`.

## §1 Kural tipleri

| Kural | Sinif | Ne is yarar | Ornek |
|---|---|---|---|
| Deger karsilastirma | `FormatConditionRuleValue` | `>`, `<`, `Between`, `IsNull` | `Tutar > 0` -> yesil |
| Ifade (kriter) | `FormatConditionRuleExpression` | criteria dili ile karmasik | `Tarih < Today()` |
| Renk olcegi | `FormatConditionRuleColorScale` | 2/3 renkli isi haritasi | dusuk -> yuksek |
| Veri cubugu | `FormatConditionRuleDataBar` | hucre icinde bar | yuzde sutunu |
| Simge seti | `FormatConditionRuleIconSet` | ok / yildiz / trafik | trend |
| Ozel bicim | `FormatConditionRuleCustom` | kendi hesabin | domain kurali |

## §2 Kural tanimi (kod)

```csharp
var rule = new FormatConditionRuleValue();
rule.Condition = FormatCondition.Less;
rule.Value1 = 0;
rule.Appearance.BackColor = Color.MistyRose;

var fr = new GridFormatRule();
fr.Column = gridView1.Columns["Tutar"];
fr.ApplyToRow = false;
fr.Rule = rule;
gridView1.FormatRules.Add(fr);
```

| Ozellik | Ne yapar | Deger ornegi |
|---|---|---|
| `GridFormatRule.Column` | hedef sutun | `gridView1.Columns["Tutar"]` |
| `GridFormatRule.ApplyToRow` | tum satira uygula | `true` (durum sutunu icin) |
| `GridFormatRule.Rule` | kural nesnesi | `FormatConditionRuleValue` |
| `rule.Appearance.*` | renk / renk-yazi / font | `BackColor`, `ForeColor`, `Font` |
| `gridView1.FormatRules` | kural koleksiyonu (Designer'da da duzenlenir) | `Add` / `Clear` |

> Designer yolu: grid menusu -> **Conditional Formatting -> New Rule** (kural `.Designer.cs` de yasar; elle eklenen kural Designer ac-kapa ile kaybolabilir).

## §3 Kural ve filtre ayrimi

| Ihtiyac | Arac | Sonuc |
|---|---|---|
| "Tutari 0'dan kucuk satirlari **gosterme**" | filtre | satir gizlenir |
| "Tutari 0'dan kucuk satiri **kirmizi yap**" | `FormatRule` | satir gorunur, renk degisir |
| "Satirin tamamini uyar" | `ApplyToRow = true` | tum hucreler boyanir |
| "Sutuna gore farkli stil" | ayri `GridFormatRule` | kural basina sutun |
| "Hesap sonucuna gore renk" | `RowCellStyle` olayi | kural ifadesi yetmediginde |

`RowCellStyle` olayi `CustomDrawCell` in **guvenli** yerine gecer: stil nesnesini degistirmek yeterlidir, `Graphics` e dokunulmaz.

## §4 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - nesne baslatma bloklu yazilir | C# 14 (`new() { A = 1 }` paylasilan dosyada **yok**) |
| `Color` / `AppearanceObject` | `DevExpress.Utils.AppearanceObject` (ayni) | ayni |
| DPI | kural renkleri sabit RGB; DPI'da olcek sorunu yok | ayni |
| DevExpress | 26.1 (ayni CPM satiri) | 26.1 |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `CustomDrawCell` ile elle boyama | tema/DPI'da bozulma | `FormatRules` |
| 2 | Kurali filtre sanmak | yanlis beklenti | kural gorunumu degistirir |
| 3 | Kurali kodda ekleyip Designer'a birakmak | ac-kapada kayip | Designer'da tanimla |
| 4 | Veriyi bicim icinde degistirmek | veri bozulur | veri repository'de, bicim gorunumde |
| 5 | Her sutun icin ayni kurali kopyalamak | bakim yuku | `ApplyToRow` / ortak stil |

## §6 Karar ozeti

| Soru | Cevap |
|---|---|
| Satir gizlensin mi? | filtre |
| Renk degissin mi? | `FormatRule` |
| Tam satir uyarsin mi? | `ApplyToRow = true` |
| Hesap sonucu mu? | `RowCellStyle` |
| Veri nereden? | Dapper -> `DataTable` (ADR-005) |

## §7 Kodlandirma notu

- Kural metni/renkleri `.resx` veya sabit stil uzerinden gelir.
- Kural basina hedef sutun net tanimlanir; belirsiz hedef yok.
- Renk paleti tek yerde; her yerde RGB yazilmaz.
- Elle cizim yerine `RowCellStyle` tercih edilir.
- Kural degisikligi Designer diff'i olarak gecer.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Veri kaynagi (Dapper) | [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] |
| Durum metni (status bar) | [[knowledge/devexpress/bar-status-items|bar-status-items]] |
| Alan hatasi gostergesi | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `grid-conditional-format-rules.md` | kural tipi tablosu, `GridFormatRule` kodu, filtre ayrimi, `RowCellStyle` notu, iki bacak | dogrulandi |
| `datasource-binding-dapper.md` | kuralin beslendigi veri yolu | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
