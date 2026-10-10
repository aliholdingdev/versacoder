---
title: "nuget-references-cpm - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DevExpress v26.1 NuGet + Central Package Management: Directory.Packages.props, TFM net48/net10.0-windows, binding redirect ve surum kilidi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# NuGet Referanslari ve CPM

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.decisions/accepted/adr-004-devexpress-universal-v261-office2010|ADR-004]] · [[.decisions/accepted/adr-014-slnx-and-cpm|ADR-014]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 4/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]]

> **Tek cumlelik sorumluluk:** *DevExpress paketleri nerede, hangi surumle sabitlenir ve iki TFM nasil karsilanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Surum **tek yerde** sabitlenir: kok `Directory.Packages.props` (CPM, `ManagePackageVersionsCentrally=true`). `.csproj`'de yalniz `Include` yazilir, `Version` **yazilmaz** (DX-01). DevExpress paketleri **v25.1'ten itibaren NuGet.org'dan** gelir (kendi feed'i yalniz v24.2 ve oncesi icin). TFM ikisi birden: `net48` (yalniz `App.config` binding redirect 26.1'e) + `net10.0-windows` (redirect gerekmez).

```text
+--------------------------------------------------------------+
| /repo/Directory.Packages.props   <- TEK SURUM KAYNAGI (DX-01) |
|    |  <PackageVersion Include="DevExpress.Win" ... />         |
|    +--> *.csproj        <- Version YAZMAZ (yalniz Include)    |
|    +--> App.config      <- yalniz net48: binding redirect     |
|    +--> nuget.config    <- kaynak (NuGet.org), surum DEGIL    |
+--------------------------------------------------------------+
```

| Dosya | Ne tasir | Tasmaz |
|---|---|---|
| `Directory.Packages.props` | tum `PackageVersion` satirlari | kod, TFM |
| `*.csproj` | `PackageReference Include` + TFM | `Version=` |
| `App.config` (net48) | binding redirect | paket adi |
| `nuget.config` | kaynak (NuGet.org) | surum |

---

## §1 Paket tablosu (v26.1)

| Paket | Ne saglar | Nerede kullanilir |
|---|---|---|
| `DevExpress.Win` | master runtime (Editors, Bars, Grid, Docking) | tum uygulama |
| `DevExpress.Win.Design` | tasarim zamani (VS designer) | yalniz tasarim |
| `DevExpress.Win.Grid` | grid | Agent/Start ekranlari |
| `DevExpress.Win.Navigation` | ribbon/bars/docking | ana pencere |
| `DevExpress.Win.RichEdit` | belge duzenleme | editor ekrani |
| `DevExpress.Win.PdfViewer` | PDF gosterme | PDF ekrani |
| `DevExpress.Utils` | ortak yardimcilar | — |

> **SURUM:** `26.1.<patch>` — patch degeri lisansli kurulumdan gelir ve bu dosyada **kilitlenmez** (`DOĞRULAMA GEREKLI`).

---

## §2 Proje dosyasi ornegi

```xml
<!-- Directory.Packages.props (kok) -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="DevExpress.Win" Version="26.1.<patch>" />
    <PackageVersion Include="DevExpress.Win.Design" Version="26.1.<patch>" />
    <PackageVersion Include="DevExpress.Win.Grid" Version="26.1.<patch>" />
  </ItemGroup>
</Project>
```

```csharp
// Her iki bacakta ayni kod - paket farketmez (DX-01)
internal static class AppVersion
{
    public static string DevExpressSeries => "26.1";   // tek etiket
#if NETFRAMEWORK          // yalniz platform farki (net48 bacak)
    public static string RedirectNote => "App.config binding redirect gerekir";
#else
    public static string RedirectNote => "redirect gerekmez";   // net10.0-windows
#endif
}
```

---

## §3 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| TFM | `<TargetFramework>net48</TargetFramework>` | `net10.0-windows` |
| Surum | **CPM'den** (ayni satir) | **CPM'den** (ayni satir) |
| Binding redirect | `App.config` -> yalniz 26.1 (`AutoGenerateBindingRedirects=true`) | gerekmez |
| C# | **LangVersion 7.3** | 14 - ama ortak dosya 7.3 kalir |
| Kaynak | NuGet.org (v25.1+ kurali) | ayni |

> **YASAK:** `.csproj`'de elle `Version="26.1..."` (DX-01), `v26.1` disi surum (DX-02/G3), EF Core (G2), `Office2010` disi ribbon (DX-03).

---

## §4 Dogrulama listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `Directory.Packages.props` var ve `ManagePackageVersionsCentrally=true` | dosya icerigi |
| 2 | `.csproj` icinde `Version=` DevExpress satiri = **0** | grep |
| 3 | `DevExpress.* 24.` / `25.` referansi = **0** (DX-02) | grep |
| 4 | net48 `App.config` redirect yalniz 26.1'e gidiyor (DX-06) | dosya icerigi |
| 5 | `dotnet restore` her iki bacakta 0 hata | cikti |

---

## §5 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Global tema / skin | [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]] |
| Ribbon stili kurali | [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] |
| Designer edit kurallari | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |
| CPM karari | [[.decisions/accepted/adr-014-slnx-and-cpm|ADR-014]] |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\nuget-references-cpm.md` (kaynak vault) | CPM yapisi, paket tablosu, iki bacak tablosu, dogrulama listesi |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | v26.1 tek surum |
| `.decisions\accepted\ADR-014-slnx-and-cpm.md` | CPM karari |
| `.rules\devexpress-rules.md` | DX-01 / DX-02 / DX-03 / DX-06 |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; patch surum degeri bu dosyada kilitlenmez.
