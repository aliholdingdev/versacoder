---
title: "language-versions-net48-net10 - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "net48 icin C# 7.3, net10.0-windows icin C# 14 varsayilan dili ve ikili bacakta ozellik bulunabilirligi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# language-versions-net48-net10 — Dil sürümü iki bacak

**Zorunlu Baglantilar:** [[knowledge/index|knowledge-index]] · [[knowledge/csharp/index|csharp-index]] · [[.rules/coding-standards|coding-standards]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]]
**Küme + Hub:** [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 5/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** *Hangi C# surumu hangi TFM'de varsayilan gelir ve bir ozellik hangi bacakta derlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Derleyici (Roslyn), hedef paket kimligine gore **varsayilan `LangVersion`** secer: `net48` -> **7.3**, `net10.0` -> **14.0**. Bu vault bu varsayilani **kasten korur**; bir ozellik yalniz iki bacakta da derleniyorsa ortak koda girer, tek bacaktaysa `#if` altina alinir ve `#if` yalnizca **platform/TFM farkinda** kullanilir (`CS-12`, G6). Yani "C# 14 yazabiliyorum" ile "net48'te de calisir" **ayni sey degildir**: surum iddiasi degil, her ozellik icin iki bacak tablosuna bakilir. `LangVersion` `latest` yapmak iki bacagi da sessizce degistirdigi icin **sayi ile** yazilir (`7.3` / `14.0`), `preview` yasaktir.

## §1 Varsayilan surumler

| TFM | Varsayilan `LangVersion` | Derleyici/SDK | Durum (bu vault) |
|---|---|---|---|
| `net48` | **C# 7.3** | .NET SDK 10 / VS 2022 Roslyn, referans .NET Framework 4.8 | bacak A — sabit |
| `net10.0-windows` | **C# 14.0** | .NET SDK 10 | bacak B — sabit |
| `netstandard2.0` | C# 7.3 | — | kullanilmiyor (K01) |
| `preview` | en son dil | — | **yasak** (G8) |

## §2 Iki bacak ozellik tablosu

| Ozellik (surum) | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `var`, `async/await`, `nameof`, expression-bodied | Var | Var |
| `ref struct`, `in`/`ref` returns (7.2) | Var | Var |
| `#nullable` NRT (8.0) | Yok (derleyici reddeder) | Var |
| `using` deklarasyonu, switch expression (8.0) | Yok | Var |
| Record, init-only, target-typed `new` (9.0) | Yok | Var |
| File-scoped namespace, global using (10.0) | Yok | Var |
| Primary constructor, collection expression (12.0) | Yok | Var |
| `field` anahtari, extension members, `partial property` (14.0) | Yok | Var |
| Default interface metodu (8.0, **calisma zamani** ister) | Yok (runtime desteklemiyor) | Var |
| `BinaryFormatter` | Var (ama **kullanma**) | Kapatildi (calisma zamaninda atar) |

> **IKI SEY:** Dil surumu ile runtime yetenegi ayridir. `LangVersion`'i yukseltseniz bile runtime'a bagli ozellikler (DIM, yerlesik `Span<T>`) `net48`'de calismaz — ortak kod en kucuk ortak paydada yazilir.

## §3 Ornek: iki bacakli proje dosyasi

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net48;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <LangVersion>7.3</LangVersion>
  </PropertyGroup>
</Project>
```

Platform farki — `#if` yalnizca burada:

```text
+------------------------------------------------------+
| .csproj : TargetFrameworks = net48;net10.0-windows    |
| #if NET48        -> yalniz API/paket farki            |
| #else            -> net10.0-windows                   |
| is mantigi asla #if icine GIRMEZ (test edilemez)      |
| surum karari    : sayi ile, asla 'latest'/'preview'   |
+------------------------------------------------------+
```

## §4 Sik hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | tek bacakta deneyip "calisiyor" demek | G6 ihlali | iki bacakta `dotnet build` |
| 2 | `LangVersion` = `latest` | surum sapmasi (G8) | sayi ile sabitle |
| 3 | `#if NET48` ile is mantigi gizlemek | test edilemez iki kod yolu | `#if` yalniz platform farki |
| 4 | C# 9+ ozelligini net48'e tasmak | CS8xxx derleme hatasi | §2 tablosundan kontrol |
| 5 | `preview` denemek | beklenmedik dil davranisi | `preview` yasak |

## §5 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| Her iki bacak | `dotnet build` -> 0 hata (iki TFM) |
| Surum sabit mi | `.csproj` icinde yazili `LangVersion` |
| `#if` sayisi minimal | platform disinda `#if` = 0 |
| Ozellik uyumu | §2 tablosu + [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] |

Bilinmeyen surum davranisi gorulurse `DOĞRULAMA GEREKLI` isaretlenir ve mimari karara gidilir.

## §6 Kurallar

1. Ortak dosya **C# 7.3** sinirinda yazilir; yenisi lazimsa dosya bacaga ayrilir.
2. Surum iddiasi her zaman iki bacak tablosu ile birlikte yazilir.
3. `#if` yalniz platform/API varligi farki tasir; is mantigi, SQL, dogrulama `#if`'e girmez.
4. `preview`/`latest` kullanilmaz; surum degisikligi kullanici onayi gerektirir (G8).
5. Dil surumu != calisma zamani yetenegi; ikisi ayri kontrol edilir.

## §7 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| MSBuild/TFM kosulu | [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] |
| NRT gecisi | [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] |
| kalip esleme surumleri | [[knowledge/csharp/pattern-matching|pattern-matching]] |
| kisit dagilimi | [[knowledge/csharp/generics-constraints|generics-constraints]] |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | net48 -> C# 7.3, net10 -> C# 14 | ADR-001 ikili bacak + `.rules/coding-standards.md` CS-12 |
| 2 | `#if` yalniz platform farki | `.rules/coding-standards.md` CS-12 (G6) |
| 3 | `preview` yasagi | G8 (vault anayasasi) — `DOĞRULAMA GEREKLI` |
| 4 | `BinaryFormatter` yasagi | `.rules/coding-standards.md` CS-05 + guvenlik kurallari |
| 5 | surum davranisi iddialari | Microsoft C# surum notlari — kesin dogrulama `DOĞRULAMA GEREKLI` |
