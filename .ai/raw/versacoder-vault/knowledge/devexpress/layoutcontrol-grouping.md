---
title: "LayoutControl Gruplama ve Tablo Modu - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "LayoutControl ile form duzeni: gruplar, tabb grup, tablo/akis modlari, boyut kisitlari ve etiket ayarlari"
---

# LayoutControl Gruplama ve Tablo Modu

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [docklayout-persistence](docklayout-persistence.md) · [tabcontrol-multipage](tabcontrol-multipage.md)

> **Tek cumlelik sorumluluk:** *Formdaki kontrollerin hizasi, grubu ve boyut siniri nereden belirlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`LayoutControl` form duzenini **tek basina** yonetir: `Root` grubu icinde `AddGroup()` / `AddTabbedGroup()` ile bolumler, `AddItem(text, control)` ile etiketli ogeler olusur; `Dock = Fill` disinda elle `Location/Size` yazmak **yasaktir** (kontroller layout'a aittir). Satir ici hizalama icin `Root.LayoutMode = LayoutMode.Table` + `ColumnDefinition`/`RowDefinition`, serbest hizalama icin `LayoutMode` varsayilan (Classic) kalir. Boyut siniri `SizeConstraintsType = Custom` + `MinSize/MaxSize` ile verilir; etiket `Text`, bosluklar `Padding`/`Spacing` ile ayarlanir.

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

```
LayoutControl (Dock=Fill)
  +-- Root (LayoutMode = Table | Classic)
  |     +-- AddItem("Ad", textEditAd)  ---- LayoutControlItem
  |     +-- AddItem("E-posta", ...)     ---- LayoutControlItem
  |     +-- AddGroup() "Butonlar"       ---- LayoutControlGroup (borderless)
  |     +-- AddTabbedGroup()            ---- TabbedControlGroup (sekmeler)
  +-- OptionsSerialization              ---- kaydet/yukle ayarlari
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `layoutControl.Root` | `LayoutControlGroup` | Kok grup; tum duzenin sahibi |
| `Root.LayoutMode` (`DevExpress.XtraLayout.Utils`) | enum | `Table` (satir/sutun) veya varsayilan (classic) |
| `Root.DefaultLayoutType` | enum | `Vertical` / `Horizontal` akis yonu |
| `Root.AddItem(text, control)` | method | Etiketli oge olusturur (`LayoutControlItem`) |
| `Root.AddGroup()` / `AddTabbedGroup()` | method | Sinirli grup / sekmeli grup |
| `BaseLayoutItem.Text` | `string` | Oge etiketi (label) |
| `LayoutControlItem.SizeConstraintsType` | enum | `Default` / `SupportHorzAlignment` / `Custom` |
| `MinSize` / `MaxSize` | `Size` | `Custom` modda boyut siniri |
| `Padding` / `Spacing` | int | Ici/dis bosluklar |
| `AppearanceItemCaption` | appearance | Etiket yazisi/fontu (grup uzerinde toplu) |
| `OptionsTableLayoutGroup` | nesne | `RowDefinitions` / `ColumnDefinitions`, `AddRow`/`AddColumn` |
| `OptionsTableLayoutItem` | nesne | `RowIndex` / `ColumnIndex` / span |
| `GroupBordersVisible` | `bool` | Grup cercevesini kaldirir |
| `OptionsSerialization` | secenek | Layout kaydet/yukle kapsamı |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Layout API | Ayni (`DevExpress.XtraLayout`) | Ayni |
| `SizeType.Percent` | `System.Windows.Forms.SizeType` (gomye) | Ayni namespace |
| Font/yazi olcusu | Sabit DPI manifesti | Otomatik DPI; `AutoScaleMode` korunur (DX-14) |
| Nullable | `Control` null kontrolu nadir | `Nullable=enable` ise `li.Control?` dikkat |
| `#if` | Yalniz DPI platform farkinda | Ayni; layout kodu #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Layout icinde `Location = new Point(...)` | Layout kontrolu konumu siler | Ogeyi `AddItem`/tasima ile yerlestir |
| 2 | `AddItem` ile eklenen ogeye `Name` vermemek | Kaydet/yukle eslesmez | `li.Name = "liAd"` |
| 3 | `SizeConstraintsType=Custom` + `MinSize>MaxSize` | Layout patlar | Sinir sirasi kontrolu |
| 4 | Grup icinde sabit `Size` yazmak | DPI'de tasma | `MaxSize` ile sinirla |
| 5 | Etiket metnini kodda uretmek | Yerellestirme kaybi | `Text` Designer/resx |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `Root.LayoutMode = LayoutMode.Table`, `AddItem(text, control)`, `AddGroup`, `AddTabbedGroup`, `SizeConstraintsType` | v26.1 WinForms dokumantasyonu ile **dogrulandi** |
| `DefaultLayoutType` / `LayoutType` enum uyesi | `NEEDS_VALIDATION` |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Ilgili kural | DX-14 (DPI/otomatik olcek), DX-10 (Designer) |

**Kardesler:** [docklayout-persistence](docklayout-persistence.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [dockmanager-panels](dockmanager-panels.md) · [ribbon-form-basics](ribbon-form-basics.md)
