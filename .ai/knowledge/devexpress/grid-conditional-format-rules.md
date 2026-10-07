---
title: "Grid Kosullu Biçimlendirme Kurallari - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "GridFormatRule ile kosullu bicimlendirme: Value/Expression/ColorScale/DataBar/IconSet ve RowCellStyle, elle hucre boyama yasagi"
---

# Grid Kosullu Biçimlendirme Kurallari (v26.1)

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [kodlama standartlari](../../.rules/coding-standards.md) · [ADR-005 ado.net dapper](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [bar-status-items](bar-status-items.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress · 5/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [dateedit-timeedit](dateedit-timeedit.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookUpEdit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Durum metni (status bar) | [bar-status-items](bar-status-items.md) |
| Alan hatasi gostergesi | [dx-validation-provider](dx-validation-provider.md) |
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `grid-conditional-format-rules.md` | kural tipi tablosu, `GridFormatRule` kodu, filtre ayrimi, `RowCellStyle` notu, iki bacak | dogrulandi |
| `datasource-binding-dapper.md` | kuralin beslendigi veri yolu | dogrulandi |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
