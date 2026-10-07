---
title: "multi-targeting-props - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "net48 + net10.0-windows ikili bacakta MSBuild ozellikleri: Directory.Build.props/.targets sirasi, TFM kosullari, LangVersion/DefineConstants ve CPM"
---

# multi-targeting-props — İki Bacakta MSBuild Özellikleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [coding-standards](../../.rules/coding-standards.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [language-versions-net48-net10](language-versions-net48-net10.md)

> **Tek cümlelik sorumluluk:** *Aynı kod tabanı `net48` ve `net10.0-windows` için nasıl derlenir, özellik (property) nerede tanımlanır ve TFM koşulu nerede çalışır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Tek `.csproj` içinde `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>` yazılır; ortak ayarlar **`Directory.Build.props`**, TFM'e bağlı ayarlar ise **`.csproj` ya da `Directory.Build.targets`** içine konur. **Kritik tuzak:** `Directory.Build.props` çoklu-bacak (outer) derlemede `$(TargetFramework)` **boştur** — oraya yazılan `'$(TargetFramework)' == 'net48'` koşulu **sessizce çalışmaz**. Ortak dosyalarda `LangVersion` **7.3**'te kalır (`#if` yalnız platform farkı), paket sürümleri **CPM** (`Directory.Packages.props`) ile tek yerden yönetilir.

| Dosya | Ne koyulur | Koymak yasak |
|---|---|---|
| `Directory.Build.props` | `LangVersion`, `Nullable`, `TreatWarningsAsErrors`, `ImplicitUsings` (kapatılıyorsa), ortak `DefineConstants` | TFM koşullu özellik |
| `Directory.Build.targets` | TFM koşullu özellik, çıktı yolu düzeltmeleri | iş mantığı içeren hedef |
| `<Proje>.csproj` | `TargetFrameworks`, TFM başına `PackageReference`/`DefineConstants` | paket **sürümü** (CPM varsa) |
| `Directory.Packages.props` | `ManagePackageVersionsCentrally` + tek sürüm kaynağı | bireysel `Version=` |
| `.editorconfig` | stil/analizör kuralları | — |

---

## §1 Sıra ve değerlendirme (neden bu sıra)

```
Directory.Build.props   (TFM BOS burada!)
        |
     <Proje>.csproj      (TargetFrameworks okunur)
        |
Directory.Build.targets (TFM DOLU - kosullar burada calisir)
        |
   Sdk hedefleri (Compile, Copy, ...)
```

| # | Sıra hatası | Belirti | Düzeltme |
|---|---|---|---|
| 1 | TFM koşulu `.props`'a yazıldı | iki bacakta da aynı değer, fark görünmüyor | koşulu `.targets`'a ya da `.csproj`'a taşı |
| 2 | `DefineConstants` **atama** (`=`) ile yazıldı | proje düzeyindeki tanım **silinir** | ekleme: `<DefineConstants>$(DefineConstants);TRACE</DefineConstants>` |
| 3 | Paket sürümü her `.csproj`'da | sürümler dağılıyor (drift) | CPM: `Directory.Packages.props` |
| 4 | `LangVersion` yalnız bir bacakta | ortak dosya net10 sözdiziminde net48'te kırılır | ortak kod **7.3**; fark `#if` içinde |
| 5 | `AppendTargetFrameworkToOutputPath` kapatılmış | iki bacak aynı `bin`'e yazıyor, dosya çakışması | `true` bırak (varsayılan) |

---

## §2 TFM koşullu özellik — doğru yazım

```xml
<!-- .csproj (veya Directory.Build.targets): burada $(TargetFramework) DOLUDUR -->
<PropertyGroup Condition="'$(TargetFramework)' == 'net48'">
  <LangVersion>7.3</LangVersion>
  <DefineConstants>$(DefineConstants);LEGACY_UI</DefineConstants>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-windows'">
  <LangVersion>latest</LangVersion>
  <DefineConstants>$(DefineConstants);MODERN_RUNTIME</DefineConstants>
</PropertyGroup>

<!-- bacak bazli paket (CPM: Version yazilmaz) -->
<ItemGroup Condition="'$(TargetFramework)' == 'net48'">
  <PackageReference Include="System.Memory" />
  <PackageReference Include="System.Threading.Channels" />
</ItemGroup>
```

| Öznitelik | `net48` | `net10.0-windows` | Nerede |
|---|---|---|---|
| `TargetFramework` | `net48` | `net10.0-windows` | `.csproj` |
| `LangVersion` (ortak dosya) | **7.3** | 7.3'te kilitli kalan ortak kod | `.props`/koşul |
| `UseWindowsForms` | her iki bacakta `true` | `true` | `.csproj` |
| `System.Memory` paketi | **gerekli** | gereksiz (runtime'da) | koşullu `ItemGroup` |
| `#if` içeriği | `NETFRAMEWORK` dalı | `NET` dalı | yalnız API/platform farkı |

> **KURAL:** `#if` yalnızca **platform/API varlığı** içindir (G6). İş mantığı, SQL ya da doğrulama kuralı `#if` içine yazılmaz — o kod test edilemez hale gelir.

---

## §3 CPM (Central Package Management) özeti

```xml
<!-- Directory.Packages.props (tek sürüm kaynağı) -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Dapper" Version="2.1.66" />
    <PackageVersion Include="System.Memory" Version="4.5.5" />
  </ItemGroup>
</Project>
```

| Kural | Sonuç |
|---|---|
| `.csproj`'da `Version=` **yazılmaz** | sürüm tek noktadan |
| Yeni paket önce `PackageVersion` ile eklenir | aksi halde NU1008 |
| DevExpress paketleri **yalnız v26.1** | ADR-004 kuralı — bkz. [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) ile ikili bacak |
| Sürüm güncelleme tek commit'te | tüm projeler tutarlı |

---

## §4 Doğrulama (kanıt komutları)

```powershell
# 1) iki bacak da derleniyor mu?  -> 0 hata beklenir
dotnet build VersaCoder.sln -c Debug

# 2) bacaklara ait cikti ayri klasorlerde mi? (dosya cakismasi yok)
Get-ChildItem src\*\bin\Debug -Directory | Select-Object FullName

# 3) TFM kosullu ozellik gercekten uygulandi mi?
dotnet msbuild src\VersaCoder.App\VersaCoder.App.csproj -getProperty:LangVersion -p:TargetFramework=net48
```

| Kanıt | Beklenen |
|---|---|
| `dotnet build` | 0 hata (her iki bacak) |
| `bin/Debug/net48` ve `bin/Debug/net10.0-windows` | ayrı klasörler |
| `-getProperty:LangVersion` | `7.3` (net48) |
| Paket sürümü raporu | CPM'teki tek değer |

---

## §5 Kardesler

| Konu | Dosya |
|---|---|
| Dil sürümü farkının kod karşılığı | [language-versions-net48-net10](language-versions-net48-net10.md) |
| Nullable geçişi | [nullable-reference-types](nullable-reference-types.md) |
| İki bacaklı veri sağlayıcı kararı | [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) |

---

## §6 Yasaklar

1. TFM koşulunu `Directory.Build.props`'a koymak (sessizce çalışmaz — §1).
2. Ortak kodda C# 8+ sözdizimi (`using var`, target-typed `new`, switch expression) — net48 bacağı kırılır.
3. Paket sürümünü proje proje yaymak (CPM ihlali) ya da DevExpress'i v26.1 dışına açmak (G3/G8).
4. EF Core — veri erişimi **ADO.NET + Dapper**, sorgu `@p1` parametreli (`data-access-rules`).
5. Kanıtsız "iki bacak da geçti" (G5) — kanıt §4 çıktısıdır.
