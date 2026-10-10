---
title: "ribbon-pages-groups-items - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "RibbonPage -> RibbonPageGroup -> BarItem zinciri, oge kimligi (Id) ve tek elden ItemClick olayi yonlendirmesi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Ribbon Sayfa, Grup ve Oge Hiyerarsisi

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.rules/solid-rules|solid-rules]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 3/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Ribbon'da bir komutun nereye ait oldugu ve olayin nereden yakalanacagi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Zincir uc katmandir: `RibbonControl.Pages` (sekmeler) -> `RibbonPage.Groups` (bolumler) -> `RibbonPageGroup.ItemLinks` (butonlar). Butonlar aslinda `RibbonControl.Items` koleksiyonunda yasar; grup yalnizca onlara **link** ekler, ayni oge iki grupta gorunebilir. Her `BarItem.Id` benzersiz olmalidir (serilestirme bozulur). Olay yonlendirmesi **tek yerden**: `RibbonControl.ItemClick` handler'i kalin kalmaz, is mantigi ayri sinifa gonderir.

```text
+---------------------------------------------------------------+
| RibbonControl                                                  |
|   +-- Pages ------> RibbonPage "Oturumlar"                    |
|   |      +-- Groups ------> RibbonPageGroup "Yeni"            |
|   |             +-- ItemLinks --> BarButtonItem (Id benzersiz)|
|   +-- Items ------> tum BarItem'larin gercek sahibi           |
|   +-- ItemClick ---> TEK handler -> uygulama katmanina cagri  |
+---------------------------------------------------------------+
```

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

RibbonPage page = new RibbonPage("Oturumlar");
page.Groups.Add(new RibbonPageGroup("Yeni") { AllowTextClipping = false });
ribbonControl1.Pages.Add(page);

ribbonControl1.SelectedPage = ribbonControl1.Pages["Oturumlar"];
ribbonControl1.ItemClick += (s, e) => OnRibbonItemClick(e.Item);   // tek giris noktasi
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `RibbonControl.Pages` | koleksiyon | sekmeler; `Pages.Add(page)` |
| `RibbonPage("ad")` | ctor | yeni sayfa; baslik ctor'da verilir |
| `RibbonPageGroup("ad") { AllowTextClipping = false }` | ctor + ozellik | bolum; basligi kirpmadan goster |
| `RibbonPage.Visible` | `bool` | sayfayi gizler |
| `BarItem.Visibility` | `BarItemVisibility` | `Never` ile oge tamamen gizlenir |
| `BarItem.RibbonStyle` | `RibbonItemStyles` | buyuk/kucuk gosterim |
| `BarItem.Id` | `string` | benzersiz olmali; `Manager.GetNewItemId()` |
| `RibbonControl.SelectedPage` | `RibbonPage` | aktif sayfa; `Pages["ad"]` ile indekslenir |
| `SelectedPageChanging` | olay | secimden once iptal edilebilir |
| `AutoHideEmptyItems` | `bool` | bos oge temizligi |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Lambda handler (`(s, e) => ...`) | C# 7.3 ile serbest | C# 14 ile serbest |
| `e.Item` tipi | `ItemClickEventArgs.Item` (`BarItem`) | ayni |
| Olay `+=` satiri | Designer'da baglanir; kodda tekrar **yasak** | ayni |
| Nullable | `e.Item` null kontrolu genelde gereksiz | `Nullable=enable` ise uyari olabilir |
| Derleme | C# 7.3 sinirlari (`??=` yok) | C# 14; ayni API, kisa yazim |

Oge kurulumu Designer'da; kodda yalnizca `ItemClick` -> handler. Kodda `page.Groups.Add` tekrari Designer'i siler.

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ayni `BarItem`'i iki kez `Items`'e eklemek | Id cakismasi, kayip oge | tek eklem, coklu `ItemLinks` |
| 2 | Her buton icin ayri handler (30+ `+=`) | devasa handler (tek sorumluluk ihlali) | tek `RibbonControl.ItemClick` |
| 3 | `Id`'yi elle yazmak | kaydet/yukle bozulur | `Manager.GetNewItemId()` |
| 4 | Sayfayi kodla `Pages.Add` ile uretmek | Designer aciliminda silinir | Designer |
| 5 | `RibbonPageGroup`'u tam genislik zorlamak | dar pencerede kirilan baslik | `AllowTextClipping = false` |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Zincir uc katman (Page -> Group -> ItemLinks) | kod incelemesi |
| 2 | Tek `ItemClick` handler | grep `ItemClick +=` = 1 |
| 3 | Elle yazilmis `Id` yok | grep `.Id =` = 0 |
| 4 | Kodda `Pages.Add` dongusu yok | grep |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\ribbon-pages-groups-items.md` (kaynak vault) | uc katmanli zincir, kod ornegi, ozellik tablosu, 5 hata |
| `.rules\devexpress-rules.md` | DX-07 (Page -> Group -> Item), DX-13 (ince handler) |
| `.rules\solid-rules.md` | tek sorumluluk / handler bicimi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
