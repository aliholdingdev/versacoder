---
title: "language-versions-net48-net10 - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "net48 icin C# 7.3, net10.0-windows icin C# 14 varsayilan dili ve ikili bacakta ozellik bulunabilirligi"
---

# language-versions-net48-net10 — Dil surumu iki bacak

**Zorunlu Baglantilar:** [index](../index.md) · [vault](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [multi-targeting-props](multi-targeting-props.md) · [nullable-reference-types](nullable-reference-types.md)

> **Tek cumlelik sorumluluk:** *Hangi C# surumu hangi TFM'de varsayilan olarak gelir ve bir ozellik hangi bacakta derlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Derleyici (Roslyn), hedef paket kimligine gore **varsayilan `LangVersion`** secer: `net48` → **7.3**, `net10.0` → **14.0**. Bu vault bu varsayilani **kasten korur**; bir ozellik yalniz iki bacakta da derleniyorsa ortak koda girer, tek bacaktaysa `#if` altina alinir ve `#if` yalnizca **platform/TFM farkinda** kullanilir (sozlesme §4.3). Yani "C# 14 yazabiliyorum" ile "net48'te de calisir" ayni sey degildir; surum iddiasi degil, her ozellik icin **iki bacak tablosu** bakilir.

---

## §1 Varsayilan Surumler

| TFM | Varsayilan `LangVersion` | Derleyici/SDK | Durum (bu vault) |
|---|---|---|---|
| `net48` | **C# 7.3** | .NET SDK 10 / VS 2022 Roslyn, referans **.NET Framework 4.8 ref paketi** | Bacak A — sabit |
| `net10.0-windows` | **C# 14.0** | .NET SDK 10 | Bacak B — sabit |
| `netstandard2.0` (paket varsayiligi) | C# 7.3 | — | Kullanilmiyor (K01) |
| `preview` | en son dil | — | **Yasak** (G8, surum karari kullaniciya ait) |

> **KURAL:** `LangVersion`'i `latest` yapmak iki bacagi da sessizce degistirir; surum karari proje dosyasinda **sayi ile** yazilir (`7.3` / `14.0`).

## §2 Iki Bacak Ozellik Tablosu

| Ozellik (surum) | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `var`, `async/await`, `nameof`, expression-bodied | Var | Var |
| `ref struct`, `in`/`ref` returns (7.2) | Var | Var |
| Nullable reference types `#nullable` (8.0) | Yok (derleyici reddeder) | Var |
| `using` deklarasyonu, switch expression (8.0) | Yok | Var |
| Record, init-only, target-typed `new` (9.0) | Yok | Var |
| File-scoped namespace, global using (10.0) | Yok | Var |
| Primary constructor, collection expression (12.0) | Yok | Var |
| `field` anahtari, extension members, `partial property` (14.0) | Yok | Var |
| Default interface metodu (8.0, **çalışma zamanı** destegi ister) | Yok (runtime desteklemiyor) | Var |
| `BinaryFormatter` | Var (ama **guvenlik nedeniyle kullanma**) | Kapatildi (calisma zamaninda atar) |

> **UYARI:** Dil surumu ile runtime yetenegi **iki ayri seydir**. C# 8'i `net48`'de `LangVersion` yukselttirseniz bile *runtime'a* bagli ozellikler (default interface metodu, `Span<T>`'nin yerlesik formu) calismaz — bu yüzden ortak kod **en kucuk ortak paydada** yazilir.

## §3 Ornek: Iki Bacakli Proje Dosyasi

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net48;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <LangVersion>7.3</LangVersion> <!-- varsayilan; TFM basina dusmus hali -->
  </PropertyGroup>
</Project>
```

Platform farki — `#if` yalnizca burada (sözlesme §4.3):

```csharp
#if NET48
    using System.Threading.Tasks;          // net48: ValueTask icin paket gerekir
#else
    using System.Threading.Tasks;          // net10: yerlesik
#endif

internal static class Tfm
{
    internal const string Label =
#if NET48
        "net48 / C# 7.3";
#else
        "net10.0-windows / C# 14";
#endif
}
```

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Tek bacakta deneyip "calisiyor" demek | G6 ihlali | Iki bacakta da `dotnet build` |
| 2 | `LangVersion` = `latest` | Surum sapmasi, G8 | Sayi ile sabitle |
| 3 | `#if NET48` ile is mantigi gizlemek | Test edilemez iki kod yolu | `#if` yalniz platform farki |
| 4 | C# 9+ ozelligini net48'e tasmak | CS8xxx derleme hatasi | Ozelligi iki bacak tablosundan kontrol et |
| 5 | Surumu unutup `preview` denemek | Beklenmedik dil davranisi | `preview` yasak |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Her iki bacak derleniyor mu | `dotnet build` -> 0 hata (iki TFM) |
| Surum sabit mi | `.csproj` icinde yazili `LangVersion` |
| `#if` sayisi minimal mi | Platform disinda `#if` = 0 |
| Ozellik uyumu | §2 tablosu + [multi-targeting-props](multi-targeting-props.md) |

Bilinmeyen bir surum davranisi gorulurse `NEEDS_VALIDATION` isaretlenir ve [index](../index.md) uzerinden `architect`'e sorulur.
