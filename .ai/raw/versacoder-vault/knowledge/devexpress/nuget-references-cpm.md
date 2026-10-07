---
title: "nuget-references-cpm - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 NuGet + Central Package Management: Directory.Packages.props, TFM net48/net10.0-windows, binding redirect ve surum kilidi"
---

# nuget-references-cpm — NuGet Referansları ve CPM (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [theme-manager-office2010](theme-manager-office2010.md) · [pdf-image-viewer](pdf-image-viewer.md)

> **Tek cumlelik sorumluluk:** *DevExpress paketleri nerede, hangi sürümle sabitlenir ve iki TFM nasıl karşılanır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Sürüm **tek yerde** sabitlenir: kök `Directory.Packages.props` (CPM, `ManagePackageVersionsCentrally=true`). `.csproj`'de yalnız `Include` yazılır, `Version` **yazılmaz** (DX-01). DevExpress paketleri **v25.1+'ten itibaren NuGet.org'dan** gelir (kendi feed'i yalnız v24.2 ve öncesi için); lisans anahtarı DevExpress hesabıyla kayıt altındadır. TFM ikisi birden: `net48` (yalnız `App.config` binding redirect 26.1'e) + `net10.0-windows` (redirect gerekmez).

---

## §1 CPM Yapisi

```
/repo/Directory.Packages.props          <-- TEK SURUM KAYNAGI (DX-01)
   |  <PackageVersion Include="DevExpress.Win" Version="26.1.<patch>" />
   |
   +--> src/VersaCoder.Desktop/*.csproj  <-- Version YAZMAZ (yalniz Include)
   |       <TargetFramework>net48</TargetFramework> | net10.0-windows
   |
   +--> src/VersaCoder.Desktop/App.config (yalniz net48)  <-- binding redirect 26.1
```

| Dosya | Ne taşır | Taşımaz |
|---|---|---|
| `Directory.Packages.props` | tüm `PackageVersion` satırları | kod, TFM |
| `*.csproj` | `PackageReference Include` + TFM | `Version=` |
| `App.config` (net48) | binding redirect | paket adı |
| `nuget.config` | kaynak (NuGet.org) | sürüm |

---

## §2 Paket Tablosu (v26.1)

| Paket | Ne saglar | Hangi ekran |
|---|---|---|
| `DevExpress.Win` | master runtime (Editors, Bars, Grid, Docking) | tümü |
| `DevExpress.Win.Design` | tasarım zamanı (VS designer) | yalnız tasarım |
| `DevExpress.Win.Grid` | grid | Agent/Start |
| `DevExpress.Win.Navigation` | ribbon/bars/docking | ana pencere |
| `DevExpress.Win.RichEdit` | belge düzenleme | Editor |
| `DevExpress.Win.PdfViewer` | PDF görüntüleme | PDF ekranı |
| `DevExpress.Utils` | ortak yardımcılar | — |

> **SÜRÜM:** `26.1.<patch>` — patch değeri lisanslı kurulumdan gelir ve PART 5'te sabitlenir (`NEEDS_VALIDATION`; gözlene en güncel sürüm 26.1.5'tir, **bu dosyada kilidlenmez**).

---

## §3 Kod ve Proje Dosyasi

```csharp
// Her iki bacakta ayni kod - paket farketmez (DX-01)
internal static class AppVersion
{
    public static string DevExpressSeries => "26.1";   // tek etiket
#if NETFRAMEWORK          // yalniz platform farki (net48 bacaği)
    public static string RedirectNote => "App.config binding redirect gerekir";
#else
    public static string RedirectNote => "redirect gerekmez";   // net10.0-windows
#endif
}
```

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

---

## §4 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| TFM | `<TargetFramework>net48</TargetFramework>` | `net10.0-windows` |
| Sürüm | **CPM'den** (aynı satır) | **CPM'den** (aynı satır) |
| Binding redirect | `App.config` → yalnız 26.1 (`AutoGenerateBindingRedirects=true`) | gerekmez |
| C# | **LangVersion 7.3** | 14 — ama ortak dosya 7.3 kalır |
| Kaynak | NuGet.org (v25.1+ kuralı) | aynı |

> **YASAK:** `.csproj`'de elle `Version="26.1..."` (DX-01), `v26.1` dışı sürüm (DX-02/G3), EF Core (G2), `Office2010` dışı ribbon (DX-03).

---

## §5 Dogrulama

- [ ] `Directory.Packages.props` var ve `ManagePackageVersionsCentrally=true`
- [ ] `grep` ile `.csproj` içinde `Version=` DevExpress satırı = **0**
- [ ] `DevExpress.* 24.` / `25.` referansı = **0** (DX-02)
- [ ] net48 `App.config` redirect yalnız 26.1'e gidiyor (DX-06)
- [ ] `dotnet restore` her iki bacakta 0 hata

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Global tema (paketten gelen skin) | [theme-manager-office2010](theme-manager-office2010.md) |
| PDF paketi adı | [pdf-image-viewer](pdf-image-viewer.md) |
| RichEdit paketi adı | [richedit-document-api](richedit-document-api.md) |
| Layout XML dosya yolu | [serialization-layout-persistence](serialization-layout-persistence.md) |
