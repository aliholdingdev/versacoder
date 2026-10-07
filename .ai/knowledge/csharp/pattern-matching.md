---
title: Pattern Matching
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: SSOT
scope: csharp
---

# Pattern Matching

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Yapısal Kurallar](../../.rules/coding-standards.md) | [Hata Kurallari](../../.rules/error-handling-rules.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (csharp · 2/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** Pattern matching, tip dondurme ve dal salimini tek ifadeye indirir; switch'i eksiksiz yapmak zorunda oldugunu unutma.

## §0 Ozet (Answer First)

Kalin if-else zincirini uce indiren araclar: **type pattern** (`is T x`), **property pattern** (`is { Prop: ... }`) ve **switch ifadesi**. Kural: switch ifadesinde `default` (veya exhaustive dal) zorunlu; eksik dal derleyici uyarir, yakalayici calisir. Cok dallı mantik icin switch, tek kontrol icin `is`.

## §1 Karar Tablosu

| Arac | Ne yapar | Nerede |
| --- | --- | --- |
| `is T x` | Tip dogrulugu + dondurme tek ifade | Tek kontrol |
| `is { A: 1, B: not null }` | Ozellik deseni | Kayit kontrolu |
| `or` / `and` | Dal birlestirme | Kisa kosullar |
| `switch` ifadesi | Deger dondurucu dal | Cok dallı donusum |
| `switch` ifadesi (statement) | Yan etkili dal | Komut akisi |

## §2 Iki Bacak

**Bacak 1 - Okunur desen:** Her dal ne isledigini tek satirda soyler; `as` + `null` kontrolu + tekrar cast yerine tek `is`. Kod azalir, yanlis tip alma imkansiqlasir.

**Bacak 2 - Eksiksizlik:** Switch ifadesi deger donduruyorsa her dal dondurmus olmali. `default` yazmayip derleyiciye birakmak yerine acikca yaz; aksi halde tip genislediginde sessiz patlar.

## §3 Ornek

```csharp
// KOTU: ucer ifade, ayni is
if (obj is Order o && o.Total > 0)
    Handle(o);

// IYI: tek ifade
if (obj is Order { Total: > 0 } order)
    Handle(order);

string Etiket(object kayit) => kayit switch
{
    Order { Total: var t } => $"siparis {t}",
    Musteri { Ad: var a }  => $"musteri {a}",
    null                    => "bos",
    _                       => "bilinmiyor",
};
```

## §4 Hata Modlari

1. Eksik `default` dal: yeni tip eklendiginde yanlis dala duser veya desenden kacar.
2. `is T x` yerine `as T` + null kontrolu yazildiginda tip disindaki her sey sessiz null dondurur.
3. Cok derin ic ice desenler okunmaz hale gelir; o deseni ayri metoda bol.
4. `and`/`or` oncelik hatasi: parantezsiz karisir; karistirici ifadeyi ikiye bol.

## §5 Kurallar

- Tek tip kontrolu icin `is T x`; kalin zincir yerine.
- Switch ifadesinde `default` dalini acikca yaz.
- Desenlerde sabitleri `const` uzerinden tut; sihirli sayi yazma.
- Yan etki gereken yerde switch ifadesi degil switch ifade (statement) kullan.
- Desenlerde cok satira uzayan kosulu ayri metoda tasi.

## §6 Yasaklar

- `is T` ile dondurup hemen `as T` ile tekrar dondurmek **YASAK**; desen tek adimda yapsin.
- Eksik `default` + yan etkili switch **YASAK**.
- Desen icinde metot cagirip yan etki gerceklestirmek **YASAK**; desen sadece eslestirme olsun.
- Null'u tip deseniyle gizlemek **YASAK**; null ayrik ele alinir.

## §7 Test Baglantisi

- Switch icin her dal + default icin test yaz; dal sayisi test sayisina yansir.
- Eksik dal testini default dalinin yakalamasi gerekir; yakalamadiysa desen yanlis.
- `is` deseninin olumsuz dalini da test et (desen tutmamali). Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Yapısal Kurallar](../../.rules/coding-standards.md) · [Hata Kurallari](../../.rules/error-handling-rules.md) · [Nullable Reference Types](nullable-reference-types.md) · [C Sharp Index](index.md)

## §9 Kanit

- Switch/exhaustive ve desen kurallari dil ozellikleriyle sinirli tutuldu; proje geneli kural degil. Kaynak: kaynak vault C# seti, 2026-09-25.
- Yan etki yasagi hata ve kodlama kurallarindaki fonksiyonellik ilkesiyle uyumlu. Kaynak: [Kodlama Standartlari](../../.rules/coding-standards.md).
