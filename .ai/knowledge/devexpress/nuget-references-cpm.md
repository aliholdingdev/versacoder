---
title: "nuget-references-cpm - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DevExpress v26.1 NuGet + Central Package Management: Directory.Packages.props, TFM net48/net10.0-windows, binding redirect ve surum kilidi"
---

# NuGet Referanslari ve CPM

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [ADR-014](../../.decisions/accepted/ADR-014-slnx-and-cpm.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Global tema / skin | [theme-manager-office2010](theme-manager-office2010.md) |
| Ribbon stili kurali | [ribbon-control-office2010](ribbon-control-office2010.md) |
| Designer edit kurallari | [designer-runtime-customization](designer-runtime-customization.md) |
| CPM karari | [ADR-014](../../.decisions/accepted/ADR-014-slnx-and-cpm.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\nuget-references-cpm.md` (kaynak vault) | CPM yapisi, paket tablosu, iki bacak tablosu, dogrulama listesi |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | v26.1 tek surum |
| `.decisions\accepted\ADR-014-slnx-and-cpm.md` | CPM karari |
| `.rules\devexpress-rules.md` | DX-01 / DX-02 / DX-03 / DX-06 |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; patch surum degeri bu dosyada kilitlenmez.
