---
title: "AccordionControl Yan Navigasyonu - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "AccordionControl ile outlook benzeri yan menu: grup/oge hiyerarsisi, ElementClick, arama kutusu ve gorunum modlari"
---

# AccordionControl Yan Navigasyonu

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [kodlama standartlari](../../.rules/coding-standards.md) · [ADR-028 layered baseline](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [dockmanager-panels](dockmanager-panels.md) · [designer-runtime-customization](designer-runtime-customization.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 5/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [docklayout-persistence](docklayout-persistence.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| IDE benzeri suruklenebilir panel | `DockManager` | bkz. [dockmanager-panels](dockmanager-panels.md) |
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
| IDE paneli | [dockmanager-panels](dockmanager-panels.md) |
| Panel duzenini XML'e kaydetme | [docking-saved-layout-xml](docking-saved-layout-xml.md) |
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |
| Alan dogrulama | [dx-validation-provider](dx-validation-provider.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `accordion-navigation.md` | `Elements` / `Style` / `ElementClick` / `ShowFilterControl` / `ExpandElementMode` / `ViewType`, hata tablosu, iki bacak tablosu | dogrulandi |
| `ElementClick` arguman tipi | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
