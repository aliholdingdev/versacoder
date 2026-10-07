---
title: "nullable-reference-types - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "NRT'ye gecis durumu bu vault'ta (parti mi dosya mi), #nullable enable ornegi ve iki bacakta NRT'nin hukmu"
---

# nullable-reference-types — NRT gecis durumu

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [result-type-design](result-type-design.md)

> **Tek cumlelik sorumluluk:** *NRT'ye gecis bu vault'ta parti mi dosya mi yapilir, `#nullable enable` nereye yazilir ve iki bacakta ne olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

NRT gecisi **parti (batch) degil, DOSYA BAZLIDIR**: bir batch'in tamami birden "artik nullable" olmaz; hangi dosya hazirsa o dosyada acilir, digerleri oldugu gibi kalir. Sebebi teknik: `net10.0-windows` bacakta varsayilan dil **C# 14** ve NRT orada hazirken, `net48` bacak varsayilan **C# 7.3**'tur ve `#nullable` direktifi 8.0 gerektirir — yani ortak bir partiyi tek hamlede acmak ya dil surumunu kirmayi ya da iki bacakli derlemeyi kirar. Gecis sirasi: **dosyayi hazirla → `#nullable enable` koy → derle → uyarilari temizle → diger dosyaya gec**. Sifirdan yeni yazilan dosyalarin tamami NRT'li dogar.

---

## §1 Gecis Modeli: Parti mi, Dosya mi?

| Model | Ne demek | Bu vaultta | Gerekce |
|---|---|---|---|
| **Parti gecisi** | 25 dosyanin hepsi tek günde NRT'ye alinir | **Hayir** | `net48` = C# 7.3 bacakta `#nullable` derlenmez; tek hamlede 25 dosya = 25 derleme kirigi |
| **Dosya gecisi** | Dosya hazir olunca acilir, kalanlar eski haliyle kalir | **EVET** | Hasar izole olur, her dosya kendi kanitiyla (build) kapanir |
| **Proje gecisi** | `.csproj` icinde `<Nullable>enable</Nullable>` | **Kismen** | Yalniz `net10.0-windows` bacagi hedeflenerek, ortak dosyalarda uyari temizlenmeden acilmaz |

Gecis asamalari (dosya basina):

| # | Adim | Kanit | Dusme hali |
|---|---|---|---|
| 1 | Dosyanin `null` yollari haritalanir | Yorum + isim | Isim `...OrNull` ise dokunulmaz |
| 2 | `#nullable enable` eklenir | build | Hata varsa geri al |
| 3 | Uyarilar temizlenir | 0 CS86xx | `!` ile bastirma (son care) |
| 4 | Test eklenir | `dotnet test` | Test yoksa dosya yari gecmis sayilir |
| 5 | `log.md` / index kaydi | Kayit satiri | Kayitsiz gecis = kanitsiz (G5) |

## §2 Iki Bacakta NRT

| Konu | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `#nullable enable` (dosya ic direktifi) | Varsayilan dil 7.3 oldugu icin **derleyici 8.0+ ister** — dil surumu acilmadan kullanilamaz | Varsayilan, sorunsuz |
| `<Nullable>enable</Nullable>` (proje) | Ortak proje dosyasinda iki bacagi da etkiler | Etkiler |
| `T?` (referans tipi) annotation | Dil surumu 8.0+ gerektirir | Var |
| Nullable uyarilari (CS8600..CS8629) | Yalniz dil surumu acildiginda | Var, `WarningsAsErrors` ile kilitlenebilir |
| `?` attribute embedding (`NullableAttribute`) | Derleyeri gomur | Derleyeri gomur |
| Pratik durum (bu vault) | Bacak A'da **kapali kalmayi tercih eder** | **Acik** |

> **KURAL:** NRT bir *runtime* ozelligi degil, **derleyici annotation**'dir; `net48`'de "calisiyor mu" sorusu degil "dil surumu buna izin veriyor mu" sorusu gecerlidir. Ortak dosyada ortak davranis isteniyorsa annotation'lar iki bacakta **ayni imzayi** korumalidir.

## §3 `#nullable enable` Ornekleri

Dosya seviyesi (tercih edilen — en ustte, frontmatter'dan hemen sonra kod blogunun ilk satiri):

```csharp
#nullable enable
namespace VersaCoder.Application.Session;

internal sealed class SessionBootstrapper
{
    internal string? SonHata { get; private set; }   // bos birakilabilir

    internal string HataZorunlu =>
        SonHata ?? throw new InvalidOperationException("hata yok");  // bos birakilamaz
}
```

Proje seviyesi + istisna (tek dosyayi kapatmak icin):

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <WarningsAsErrors>$(WarningsAsErrors);CS8600;CS8602;CS8603</WarningsAsErrors>
</PropertyGroup>
```

```csharp
#nullable disable           // gecici: eski dosya, tarama yok
// ... eski kod ...
#nullable restore           // projenin ayarina don
```

| Direktif | Anlam | Kullanim |
|---|---|---|
| `#nullable enable` | Dosyada analiz + annotation acik | Yeni/hazir dosya |
| `#nullable disable` | Analiz kapali, annotation yok | Eski dosya (gecis sirasi bekler) |
| `#nullable annotations` | Annotation yaz, uyari verme | Iki bacak imza esigi |
| `#nullable restore` | Proje ayarina don | Gecici istisna sonrasi |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Partinin tamamina birden `enable` | 25 dosyada binlerce uyari | Dosyaya in, sirayla |
| 2 | `!` ile tum uyarilari bastirmak | Runtime `NullReferenceException` | `?? throw` / `ArgumentNullException` |
| 3 | NRT'yi `null` kontrolü sanmak | Null check zaten compiler'da yoktu | NRT **annotation**, kosul degil |
| 4 | net48'de `LangVersion`'i gizlice yukseltmek | Surum sapmasi (G6/G8) | Surum karari `architect` + kullanici |
| 5 | `string?`'i public API'de unutup imzayi degistirmek | Cagiran kodda kirilma | Imza degisikligi review'e girer |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Dosya derleniyor mu | `dotnet build` -> 0 hata, 0 CS86xx |
| Gecis durumu nerede | Dosyanin basindaki `#nullable` satiri |
| Kurala uyum | [coding-standards](../../.rules/coding-standards.md) |
| Sonraki dosya | Kardes dosyalar: [async-await-basics](async-await-basics.md) · [result-type-design](result-type-design.md) |

`net48` bacakta dil surumu acilip acilmayacagi (proje geneli `LangVersion` yukseltimi) bu dosyanin kapsami disindadir: **`NEEDS_VALIDATION`** + `architect` karari (G8).
