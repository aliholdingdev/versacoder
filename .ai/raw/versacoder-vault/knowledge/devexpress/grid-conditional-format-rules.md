---
title: "grid-conditional-format-rules - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridFormatRule ile kosullu bicimlendirme: Value/Expression/ColorScale/DataBar/IconSet ve RowCellStyle (v26.1, elle hucre boyama YASAK)"
---

# grid-conditional-format-rules — Kosullu bicimlendirme (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridview-columns-format](gridview-columns-format.md) · [gridcontrol-basics](gridcontrol-basics.md)

> **Tek cumlelik sorumluluk:** *Kosullu renk/gorunum kurali nerede tanimlanir, elle hucre boyama neden yasaktir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Koşullu biçim `GridView.FormatRules` koleksiyonuna eklenen **`GridFormatRule`** nesneleriyle tanımlanır; her kural bir `FormatConditionRule*` + bir hedef sütun içerir. Kural veriyi **değiştirmez**, yalnız görünümü değiştirir — filtre değildir. Elle `CustomDrawCell` ile renk vermek **yasak** (DX-08); DPI/tema altında bozulur, kural motifini de görünmez kılar. Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Kural Tipleri

| Kural | Sinif | Ne is yarar | Ornek |
|---|---|---|---|
| Değer karşılaştırma | `FormatConditionRuleValue` | `>`, `<`, `Between`, `IsNull` | `Tutar > 0` → yeşil |
| İfade (kriter) | `FormatConditionRuleExpression` | criteria dili ile karmaşık | `Tarih < Today()` |
| Renk ölçeği | `FormatConditionRuleColorScale` | 2/3 renkli ısı haritası | düşük→yüksek |
| Veri çubuğu | `FormatConditionRuleDataBar` | hücre içinde bar | yüzde sütunu |
| Simge seti | `FormatConditionRuleIconSet` | ok / yıldız / trafik | trend |
| Özel biçim | `FormatConditionRuleCustom` | kendi hesabın | domain kuralı |

## §2 Kural Tanimi (kod)

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
| `GridFormatRule.Column` | hedef sütun | `gridView1.Columns["Tutar"]` |
| `GridFormatRule.ApplyToRow` | tüm satıra uygula | `true` (durum sütunu için) |
| `GridFormatRule.Rule` | kural nesnesi | `FormatConditionRuleValue` |
| `rule.Appearance.*` | renk/renk-yazı/font | `BackColor`, `ForeColor`, `Font` |
| `gridView1.FormatRules` | kural koleksiyonu (Designer'da da düzenlenir) | `Add` / `Clear` |

Designer yolu: grid menüsü → **Conditional Formatting → New Rule** (kural `.Designer.cs`'de yaşar; elle eklenen kural Designer aç-kapa ile kaybolabilir, DX-10).

## §3 Kural ve Filtre Ayrimi

| Ihtiyac | Araç | Sonuc |
|---|---|---|
| "Tutarı 0'dan küçük satırları **gösterme**" | filtre ([grid-filtering-sorting](grid-filtering-sorting.md)) | satır gizlenir |
| "Tutarı 0'dan küçük satırı **kırmızı yap**" | `FormatRule` | satır görünür, renk değişir |
| "Satırın tamamını uyar" | `ApplyToRow = true` | tüm hücreler boyanır |
| "Sütuna göre farklı stil" | ayrı `GridFormatRule` | kural başına sütun |
| "Hesap sonucuna göre renk" | `RowCellStyle` olayı | kural ifadesi yetmediğinde |

`RowCellStyle` olayı `CustomDrawCell`'in **güvenli** yerine geçer: stil nesnesini değiştirmek yeterlidir, `Graphics`'e dokunulmaz.

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — nesne başlatma bloğu (`new X(); x.A = 1;`) zorunlu | C# 14 (`new() { A = 1 }` paylaşılan dosyada **yok**) |
| `Color` / `AppearanceObject` | `DevExpress.Utils.AppearanceObject` (aynı) | aynı |
| DPI | kural renkleri sabit RGB; DPI'da ölçek sorunu yok | aynı |
| DevExpress | 26.1 (aynı CPM satırı) | 26.1 |

> **YASAK:** `CustomDrawCell` içinde `e.Appearance.BackColor` dışında çizim, EF Core, `v26.1` dışı sürüm (DX-08 / G2 / DX-02).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Kolon ve biçim | [gridview-columns-format](gridview-columns-format.md) |
| Gruplama + özet | [grid-grouping-summaries](grid-grouping-summaries.md) |
| Arama paneli | [grid-search-panel-find](grid-search-panel-find.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
