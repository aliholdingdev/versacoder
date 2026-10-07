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

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [hata kurallari](../../.rules/error-handling-rules.md) · [ADR-028 layered baseline](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [dx-validation-provider](dx-validation-provider.md) · [bar-status-items](bar-status-items.md)
**Küme + Hub:** [devexpress index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 5/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridcontrol-basics](gridcontrol-basics.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Kalici durum (satir sayisi, oturum) | `BarStaticItem` | bkz. [bar-status-items](bar-status-items.md) |
| Alan hatasi (ikon + mesaj) | `DXErrorProvider` | bkz. [dx-validation-provider](dx-validation-provider.md) |

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
| Alan hatasi gostergesi | [dx-validation-provider](dx-validation-provider.md) |
| Durum cubugu | [bar-status-items](bar-status-items.md) |
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |
| Ana ekran navigasyonu | [accordion-navigation](accordion-navigation.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `alertcontrol-notifications.md` | `Show(...)` imzasi, `AlertClick` / `BeforeFormShow` / `Buttons` / `ButtonClick`, hata tablosu | dogrulandi |
| `AlertContainerControl` ve gecikme/konum property adlari | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
