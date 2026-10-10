---
title: Pattern Matching
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: primary
scope: csharp
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Pattern Matching

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Yapısal Kurallar]] | [[.rules/error-handling-rules|Hata Kurallari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

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
- `is` deseninin olumsuz dalini da test et (desen tutmamali). Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Yapısal Kurallar]] · [[.rules/error-handling-rules|Hata Kurallari]] · [[knowledge/csharp/nullable-reference-types|Nullable Reference Types]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Switch/exhaustive ve desen kurallari dil ozellikleriyle sinirli tutuldu; proje geneli kural degil. Kaynak: kaynak vault C# seti, 2026-09-25.
- Yan etki yasagi hata ve kodlama kurallarindaki fonksiyonellik ilkesiyle uyumlu. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
