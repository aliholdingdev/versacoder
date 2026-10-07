---
title: "generics-constraints - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "where kisitlarinin sinifi, C# 7.3'te mevcut kisitlar ve C# 8-14'te eklenenlerin iki bacak dagilimi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# generics-constraints — Generic kısıtlar

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [coding-standards](../../.rules/coding-standards.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [pattern-matching](pattern-matching.md)
**Küme + Hub:** [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 5/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** *Bir `where` kisiti hangi bacakta derlenir ve tip T ile ne yapabilecegimi sinirlar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kisit **derleme zamaninda** "T ile neler yapabilirim"i belirler; kisitsiz generic kod yalnizca `object` uzerindeki islemleri yapabilir. `net48` bacakta varsayilan dil **C# 7.3** oldugu icin 7.3'te var olan kisitlar (`class`, `struct`, `unmanaged`, `new()`, `Enum`, `Delegate`, arayuz + temel tip) **iki bacakta da** calisir; `notnull` (C# 8), `allows ref struct` (C# 13) ve statik soyut arayuz uyesi (C# 11) yalniz `net10` bacakta kalir. Bu vault kurali: **ortak imza 7.3 kisitlarinda kalir**; daha yenisi lazimsa once iki bacak tablosuna bakilir, sonra karar verilir. Kisit **genisletilir, arttirilmaz** — `new()` eklemek cagiranlari kirar ve public imza degisikligi review'e girer.

## §1 Kisit katalogu (iki bacak)

| Kisit | Anlam | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|---|
| `where T : class` | referans tipi | Var | Var |
| `where T : struct` | value tipi | Var | Var |
| `where T : class, notnull` | referans + null olamaz (NRT) | **Yok** (C# 8) | Var |
| `where T : unmanaged` | safe handle disi ham bellek | Var (7.3) | Var |
| `where T : new()` | parametresiz kurucu zorunlu | Var | Var |
| `where T : Enum` | bir enum tipi | **Var (7.3 ile geldi)** | Var |
| `where T : Delegate` | bir delegate tipi | **Var (7.3 ile geldi)** | Var |
| `where T : IRepo` (arayuz) | arayuz uyesi | Var | Var |
| `where T : TemelTip` | kalitim + arayuzlar | Var | Var |
| `where T : allows ref struct` | `ref struct` T kullanilsin | **Yok** (C# 13) | Var |
| `where T : IComparable<T>` gibi | sart `&` ile birlesik | Var | Var |
| statik soyut arayuz uyesi | `static abstract` (C# 11) | **Yok** | Var |

## §2 Kisit ne kazandirir

| Kisit eklenince | Yazilabilen kod | Etki |
|---|---|---|
| `class` | `is`, `== null`, referans karsilastirma | en yaygin |
| `struct` | `default(T)`, null yok, stack'te | alloc dusturur |
| `unmanaged` | `Span<byte>` reinterpret, pointer | dusuk seviye |
| `new()` | `new T()` (factory yerine) | kurucu gorevi yukler |
| `Enum` / `Delegate` | `Enum.Parse`, guvenli siniflandirma | C# 7.3'te iki bacakta |
| kisit **yok** | yalniz `object` uyelikleri | en dar |

## §3 Ornek

Iki bacakta derlenen — yalniz 7.3 kisitlari:

```csharp
internal static class Donusturucu<TGirdi, TSonuc>
    where TGirdi : class, new()      // referans + parametresiz kurucu
    where TSonuc : struct            // value tipi
{
    internal static TSonuc Cevir(TGirdi girdi)
    {
        if (girdi is null) throw new ArgumentNullException(nameof(girdi));
        var ara = new TGirdi();
        return Donustur(ara, girdi);
    }

    private static TSonuc Donustur(TGirdi ara, TGirdi girdi) => default;
}
```

net10 bacakta eklenen (ortak dosyaya **yazilmaz**):

```text
+------------------------------------------------------+
| net10 bacakta ek kisitlar  : notnull (8)             |
|                               allows ref struct (13) |
|                               static abstract (11)   |
| ortak dosya kurali          : yalniz 7.3 kisitlari   |
| lazimsa dosya ayrilir       : #if yalniz platform    |
+------------------------------------------------------+
```

## §4 Sik hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `notnull` / `allows ref struct` ortak dosyaya | net48 derlemesi kirilir | §1 tablosundan kontrol |
| 2 | `new()` ekleyip kurucuyu unutmak | `new T()` her yerde hata | kisit + kurucu birlikte |
| 3 | `struct` deyip `default`'u null sanmak | yanlis varsayim | kisitin ne garantiledigini oku |
| 4 | once `object` ile yazip sonra generic yapmak | tip guveni kaybi | en dar kisitle basla |
| 5 | public imzada kisit daraltmak | cagiran kirilir | imza degisikligi = review |

## §5 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| Her `where` | §1 satirinda iki bacak da "Var" mu |
| Build | `net48` + `net10.0-windows` 0 hata |
| Imza degisikligi | architect onayi (sozlesme) |
| Olcusuz iddia | `NEEDS_VALIDATION` etiketi |

## §6 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| dil surumu dagilimi | [language-versions-net48-net10](language-versions-net48-net10.md) |
| arayuz mu soyut mu | [interfaces-vs-abstract](interfaces-vs-abstract.md) |
| kalip esleme | [pattern-matching](pattern-matching.md) |
| degismez tip | [records-and-immutability](records-and-immutability.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | net48 varsayilan dili C# 7.3 | ADR-001 ikili bacak + `.rules/coding-standards.md` CS-12 |
| 2 | `#if` yalniz platform farkinda | `.rules/coding-standards.md` CS-12 (G6) |
| 3 | public imza degisikligi review | `.rules/solid-rules.md` SOL-05 (sozlesme sahipligi) |
| 4 | `Enum`/`Delegate` kisitinin 7.3 gelis sekli | Microsoft C# 7.3 surum notlari — `DOĞRULAMA GEREKLI` |
| 5 | `allows ref struct` C# 13 | Microsoft C# surum notlari — `DOĞRULAMA GEREKLI` |
