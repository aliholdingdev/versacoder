---
title: "AlertControl Bildirimleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "AlertControl ile kayan uyari pencereleri: Show cagrisi, AlertClick verisi, ozel butonlar ve HTML sablonu"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# AlertControl Bildirimleri

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/error-handling-rules|hata kurallari]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028 layered baseline]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/bar-status-items|bar-status-items]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Bilgi/uyari penceresi MessageBox yerine nasil gosterilir ve tiklanan veri nasil geri alinir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

`AlertControl` (`DevExpress.XtraBars.Alerter`) ekranda kayan, kendi kendine kaybolan bildirim penceresi gosterir; `Show(...)` ile bir kere cagrilir, kapatma isini kontrol kendisi yapar. Her uyarinin `Tag` alani oldugu icin **tiklanan bildirimin verisi** `AlertClick` olayindan geri alinir - MessageBox'a dusmeden ayni is akisi korunur. Uyari icerigi HTML/CSS sablonuyla bicimlenebilir, `Buttons` koleksiyonu ile icerisine ozel buton konabilir. Bildirim yalniz **gostergedir**; is kurali servis/ViewModel'de kalir.

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Alerter;

alertControl1.Show(this, "Kaydedildi", "Oturum kaydedildi.", "", null, sessionId);

private void alertControl1_AlertClick(object sender, AlertClickEventArgs e)
{
    string id = e.Info.Tag as string;      // tiklanan uyarinin verisi
    OpenSession(id);                       // ince cagri - is kurali degil
}
```

| Adim | Ne olur |
|---|---|
| `Show(owner, caption, text, html, image, tag)` | uyari olusur ve akar |
| Kullanici tiklar | `AlertClick`, `e.Info.Tag` veriyi tasir |
| Sure dolar | kontrol formu kapatir (elle kapatma gerekmez) |

## §2 Ozellik / olay tablosu

| Uye | Tur | Ne is yapar |
|---|---|---|
| `AlertControl.Show(...)` | method | Uyariyi gosterir; `tag` geri donus verisidir |
| `AlertClick` | olay | Uyariye tiklanir; `e.Info.Tag` veriyi tasir |
| `BeforeFormShow` | olay | Gosterilmeden once ince ayar (`e.AlertForm.OpacityLevel`) |
| `AlertControl.Buttons` | koleksiyon | Uyari icine ozel butonlar |
| `ButtonClick` | olay | Ozel butona basilinca |
| HTML/CSS sablonu | icerik | Uyari govdesini HTML ile bicimleme |
| `AlertContainerControl` | kontrol | Uyariyi form icinde sabit alanda tutar -> **DOĞRULAMA GEREKLI** |

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Referans | `DevExpress.XtraBars.v26.1` | ayni paket |
| `Show(...)` ilk parametre | `this` (Form) | ayni |
| `Tag` veri tipi | `object`; `as` ile indirger | `Nullable=enable` ise `as` sonrasi null kontrolu |
| HTML render | gomulu HTML motoru (framework'e bagli degil) | ayni motor |
| Dil | **C# 7.3** - `using var` yasak | C# 14; ortak dosya 7.3 kalir |

## §4 Karar tablosu (hangi gosterge?)

| Ihtiyac | Arac | Neden |
|---|---|---|
| Kayan, kendiliginden kaybolan uyari | `AlertControl` | MessageBox'suz akis |
| Kullanici onayi zorunlu soru | `XtraMessageBox` | modal karar |
| Uzun sureli islem beklemesi | SplashScreen / WaitForm | surec gostergesi |
| Kalici durum (satir sayisi, oturum) | `BarStaticItem` | bkz. [[knowledge/devexpress/bar-status-items|bar-status-items]] |
| Alan hatasi (ikon + mesaj) | `DXErrorProvider` | bkz. [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |

## §5 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Is kuralini `AlertClick` icine gommek | test edilemez, god handler | cagriyi servise tasi |
| 2 | `Tag` i null birakip sonra `as` ile okumak | anlam verisiz tiklama | her `Show` a anlamli `tag` ver |
| 3 | `MessageBox.Show` ile ayni anda kullanmak | cift bildirim, kafa karisikligi | tek kanal: AlertControl |
| 4 | Uyariyi ana is parcacigindan cagirmak | UI thread disi cagri | UI thread'de `Show` |
| 5 | Gorsel/metni kodda sabitlemek | yerellestirme kaybi | metin `.resx` / kaynak |

## §6 Dogrulama ve sinir

| Katman | Gorev | Not |
|---|---|---|
| `AlertControl` | yalniz gosterge + navigasyon tetikleyici | ekran okuyucu uyumlulugu **DOĞRULAMA GEREKLI** |
| ViewModel / servis | is kurali ve kalici veri | ADO.NET + Dapper, `@p1` |
| `DXErrorProvider` | alan bazli hata | uyari ile karistirilmaz |
| `XtraMessageBox` | yalniz modal onay | asil kanal AlertControl |

## §7 Kodlandirma notu

- `AlertClick` handler'i ince kalir; metin `.resx` uzerinden gelir.
- `Show` cagrisi tek merkezde (bildirim servisi) toplanir.
- Uyari metni kisadir; detay acilan ekranda gosterilir.
- `Buttons` ekleniyorsa buton adlari da yerellestirilir.
- UI thread disindan gelirse `.Invoke` ile UI'ya tasinir.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Alan hatasi gostergesi | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |
| Durum cubugu | [[knowledge/devexpress/bar-status-items|bar-status-items]] |
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |
| Ana ekran navigasyonu | [[knowledge/devexpress/accordion-navigation|accordion-navigation]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `alertcontrol-notifications.md` | `Show(...)` imzasi, `AlertClick` / `BeforeFormShow` / `Buttons` / `ButtonClick`, hata tablosu | dogrulandi |
| `AlertContainerControl` ve gecikme/konum property adlari | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
