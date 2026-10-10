---
title: "AccordionControl Yan Navigasyonu - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "AccordionControl ile outlook benzeri yan menu: grup/oge hiyerarsisi, ElementClick, arama kutusu ve gorunum modlari"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# AccordionControl Yan Navigasyonu

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028 layered baseline]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Sol taraftaki acilir-kapanir menu hangi kontrolle kurulur ve bir ogeye tiklaninca ne olur* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

`AccordionControl`, NavBar'in modern halefi olarak **grup + oge** hiyerarsisi tutar: kok gruplar `accordionControl1.Elements`'te, alt ogeler `AccordionControlElement.Elements`'tedir. Bir oge `ElementStyle.Item`, bir bolum `ElementStyle.Group` olur; tiklama `ElementClick` olayiyla tek noktadan yakalanir ve genelde `NavigationFrame` sayfasini degistirir. Dahili arama kutusu (`ShowFilterControl`) ve tek/cok acilir mod (`ExpandElementMode`) ile kullanici deneyimi ayarlanir; `ViewType` hamburger menu gorunumune gecis saglar.

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Navigation;

AccordionControlElement group = new AccordionControlElement {
    Style = ElementStyle.Group, Text = "Oturumlar" };
group.Elements.Add(new AccordionControlElement {
    Style = ElementStyle.Item, Text = "Yeni Oturum" });
accordionControl1.Elements.Add(group);

accordionControl1.ElementClick += (s, e) => NavigateTo(e.Element);
```

```
AccordionControl
  +-- Elements (kok)
  |     +-- Element "Oturumlar"   (Style = Group)
  |     |     +-- Elements
  |     |           +-- "Yeni Oturum"   (Style = Item)  -> ElementClick
  |     |           +-- "Liste"         (Style = Item)
  |     +-- Element "Ayarlar"     (Style = Group)
  +-- ShowFilterControl (arama kutusu)
  +-- ExpandElementMode / AnimationType / ViewType
```

## §2 Ozellik tablosu

| Uye | Tur | Ne is yapar |
|---|---|---|
| `AccordionControl.Elements` | koleksiyon | Kok grup/ogeler |
| `AccordionControlElement.Elements` | koleksiyon | Alt seviye (sinirsiz derinlik) |
| `AccordionControlElement.Style` | `ElementStyle` | `Group` / `Item` / `Header` |
| `Text` / `.Image` | `string` / `Image` | Baslik ve ikon |
| `ElementClick` | olay | Tiklama; `e.Element` tiklanan oge |
| `AllowItemSelection` | `bool` | Tiklanan ogeyi vurgular |
| `ExpandElementMode` | enum | Ayni anda tek mi/cok oge acik kalsin |
| `ShowFilterControl` | `bool` | Dahili arama kutusu |
| `ViewType` + `AccordionOptionsHamburgerMenu` | enum/nesne | Hamburger menu gorunumu |
| `ContentContainer` | ozellik | Oge altina acilan ozel icerik alani |

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Paket | `DevExpress.XtraBars.v26.1` (Win.Navigation) | ayni paket |
| Oge kurulumu | Designer "Elements" sekmesi | ayni |
| C# surumu | **7.3**: object initializer serbest | 14: ayni yazim |
| Nullable | `e.Element` null gelmez | `Nullable=enable` ise `e.Element?` dikkat |
| `#if` | yalniz platform farkinda | menu mantigi `#if` altina ALINMAZ |

## §4 Karar tablosu (hangi menu?)

| Ihtiyac | Kontrol | Neden |
|---|---|---|
| Grup + oge + arama + hamburger | `AccordionControl` | modern, tek kaynak |
| Klasik yan sekmeler | `NavBarControl` | eski proje |
| IDE benzeri suruklenebilir panel | `DockManager` | bkz. [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] |
| Sayfa degistirme (menu disi) | `NavigationFrame` | menu yalniz sayfa secer |
| Baslangic karo sayfasi | `TileControl` | klasik acilis ekrani |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Her ogeye ayri `Click` handler | God form | tek `ElementClick` |
| 2 | Navigasyonu oge metni ile `switch` | metin degisince kirilir | sabit `Tag`/kimlik kullan |
| 3 | Ogeyi kodda `Elements.Add` ile tekrar | her acilimda kopyalanir | Designer'da bir kez ekle |
| 4 | `ShowFilterControl` acikken metni kodda gizlemek | arama sonucu bos kalir | filtre sonucunu da isle |
| 5 | Accordion'u `Dock=Fill` yapip govdeyi ustune almak | govde gorunmez | sol `Dock=Left`, govde `Fill` |

## §6 Dogrulama ve sinir

| Kural | Yeri | Not |
|---|---|---|
| Secim durumu (vurgu) | kontrol uzerinde | `AllowItemSelection` |
| Menu item kimligi | sabit `Tag` | metne bagimli degil |
| Islem (kayit/ac/sil) | servis / ViewModel | `ElementClick` icine gomulmez |
| Kalici veri | ADO.NET + Dapper, `@p1` | EF Core onerilmez |

## §7 Kodlandirma notu

- `ElementClick` handler'i ince kalir; tek sorumluluk secimdir.
- `Text` yerellestirmesi `.resx` uzerinden verilir, sabit literal degil.
- Designer disi elle eklenen oge yeniden uretimde kaybolabilir.
- Ayni `Tag` iki ogede kullanilmaz; kimlik benzersizdir.
- `ContentContainer` icerigi agir is ise UserControl'e tasinir.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| IDE paneli | [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] |
| Panel duzenini XML'e kaydetme | [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |
| Alan dogrulama | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `accordion-navigation.md` | `Elements` / `Style` / `ElementClick` / `ShowFilterControl` / `ExpandElementMode` / `ViewType`, hata tablosu, iki bacak tablosu | dogrulandi |
| `ElementClick` arguman tipi | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
