---
title: "layoutcontrol-grouping - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "LayoutControl ile form duzeni: gruplar, tabb grup, tablo/akis modlari, boyut kisitlari ve etiket ayarlari"
---

# LayoutControl Gruplama ve Tablo Modu

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ui-rules](../../.rules/ui-rules.md) · [docklayout-persistence](docklayout-persistence.md) · [ribbon-form-basics](ribbon-form-basics.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 3/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *Formdaki kontrollerin hizasi, grubu ve boyut siniri nereden belirlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`LayoutControl` form duzenini **tek basina** yonetir: `Root` grubu icinde `AddGroup()` / `AddTabbedGroup()` ile bolumler, `AddItem(text, control)` ile etiketli ogeler olusur; `Dock = Fill` disinda elle `Location/Size` yazmak **yasaktir** (kontroller layout'a aittir). Satir ici hizalama icin `Root.LayoutMode = LayoutMode.Table` + `ColumnDefinition`/`RowDefinition`, serbest hizalama icin `LayoutMode` varsayilan (Classic) kalir.

```text
LayoutControl (Dock=Fill)
  +-- Root (LayoutMode = Table | Classic)
  |     +-- AddItem("Ad", textEditAd)  ---- LayoutControlItem
  |     +-- AddItem("E-posta", ...)     ---- LayoutControlItem
  |     +-- AddGroup() "Butonlar"       ---- LayoutControlGroup (borderless)
  |     +-- AddTabbedGroup()            ---- TabbedControlGroup (sekmeler)
  +-- OptionsSerialization              ---- kaydet/yukle ayarlari
```

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

layoutControl.Root.LayoutMode = LayoutMode.Table;                 // satir sutun duzeni
LayoutControlItem liAd = layoutControl.Root.AddItem("Ad", textEditAd);
liAd.Name = "liAd";                                               // kaydet/yukle icin zorunlu

LayoutControlGroup lgButonlar = layoutControl.Root.AddGroup();
lgButonlar.GroupBordersVisible = false;
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `layoutControl.Root` | `LayoutControlGroup` | Kok grup; tum duzenin sahibi |
| `Root.LayoutMode` | enum | `Table` (satir/sutun) veya varsayilan (classic) |
| `Root.AddItem(text, control)` | method | Etiketli oge olusturur (`LayoutControlItem`) |
| `Root.AddGroup()` / `AddTabbedGroup()` | method | Sinirli grup / sekmeli grup |
| `BaseLayoutItem.Text` | `string` | Oge etiketi (label) |
| `LayoutControlItem.SizeConstraintsType` | enum | `Default` / `SupportHorzAlignment` / `Custom` |
| `MinSize` / `MaxSize` | `Size` | `Custom` modda boyut siniri |
| `Padding` / `Spacing` | int | Ici/dis bosluklar |
| `AppearanceItemCaption` | appearance | Etiket yazisi/fontu |
| `OptionsTableLayoutGroup` | nesne | `RowDefinitions` / `ColumnDefinitions` |
| `OptionsTableLayoutItem` | nesne | `RowIndex` / `ColumnIndex` / span |
| `GroupBordersVisible` | `bool` | Grup cercevesini kaldirir |

---

## §3 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Layout icinde `Location = new Point(...)` | Layout konumu siler | Ogeyi `AddItem`/tasima ile yerlestir |
| 2 | `AddItem` ile eklenen ogeye `Name` vermemek | Kaydet/yukle eslesmez | `li.Name = "liAd"` |
| 3 | `SizeConstraintsType=Custom` + `MinSize>MaxSize` | Layout patlar | Sinir sirasi kontrolu |
| 4 | Grup icinde sabit `Size` yazmak | DPI'de tasma | `MaxSize` ile sinirla |
| 5 | Etiket metnini kodda uretmek | Yerellestirme kaybi | `Text` Designer/resx |

Boyut siniri `SizeConstraintsType = Custom` + `MinSize/MaxSize` ile verilir; etiket `Text`, bosluklar `Padding`/`Spacing` ile ayarlanir.

---

## §4 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Layout API | Ayni (`DevExpress.XtraLayout`) | Ayni |
| `SizeType.Percent` | `System.Windows.Forms.SizeType` | Ayni namespace |
| Font/olcu | Sabit DPI manifesti | Otomatik DPI; `AutoScaleMode` korunur (DX-14) |
| Nullable | `Control` null kontrolu nadir | `Nullable=enable` ise `li.Control?` dikkat |
| `#if` | Yalniz DPI platform farkinda | Ayni; layout kodu `#if` altina ALINMAZ |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Elle `Location/Size` layout icinde yok | grep |
| 2 | Kaydet/yukle edilen her `LayoutControlItem`'de `Name` var | kod incelemesi |
| 3 | Etiket metinleri Designer/resx'te | dosya icerigi |
| 4 | `MinSize <= MaxSize` her ogede | kod incelemesi |
| 5 | Her iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\layoutcontrol-grouping.md` (kaynak vault) | temel kullanim, ozellik tablosu, 5 yaygin hata, iki bacak farki |
| `.rules\devexpress-rules.md` | DX-14 (DPI/otomatik olcek), DX-10 (Designer) |
| `.rules\ui-rules.md` | form duzeni kurallari |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; `DefaultLayoutType` enum uyesi ve bu batch'te alinamayan `dotnet build` ciktisi `DOĞRULAMA GEREKLI`dir.
