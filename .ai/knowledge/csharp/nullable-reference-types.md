---
title: Nullable Reference Types
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

# Nullable Reference Types

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[.rules/error-handling-rules|Hata Kurallari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Nullable reference types ile null'i tipte goster; akistan kirpma yerine akisi dogrula ve dondur.

## §0 Ozet (Answer First)

Nullable iki bacaktir: **acik deklarasyon** (`string?` semantigi) ve **akisa dayali denetim**. `!` (null-forgiving) bildirim kapatir, denetim yapmaz - yalnizca eski kodla gecis doneminde kullan. Cozum: metot donusunu `?` ile gercekten ac, giriste kirpma yapma, cagiran taraf null'i islesin.

## §1 Karar Tablosu

| Durum | Doğru | Yanlis |
| --- | --- | --- |
| Deger gercekten yok olabilir | `string? ad` | `string ad` + yorum |
| Emin degilsen | `string? ad` + kontrol | `ad!` ile gec |
| Cagiran null alamaz | `string` (null olmamali) | Her yerde `?` |
| Gecis donemi | Kademeli aciklama | Tek seferde her yeri degistir |
| Sarmalayici null | `List<string>?` | `List<string?>` (yanlis katman) |

## §2 Iki Bacak

**Bacak 1 - Bildirim akisi:** Nullable acikca yazilan yerde derleyici dal kurar: null kontrolu yapilmadan kullanildiginda uyarir. Uyari, kural ihlalinin kanitidir.

**Bacak 2 - Runtime dogrulama:** Tip sistemi sinir disindan (JSON, veritabani, dosya) gelen null'i bilmez. Sinirda dogrula, iceriye temiz veri gecir. [[.rules/error-handling-rules|Hata Kurallari]] gecer.

## §3 Ornek

```csharp
// IYI: donus nullable, cagiran isler
public string? FindName(int id)
{
    var row = Load(id);
    return row is null ? null : row.Name;
}

var name = FindName(7);
var shown = name ?? "bilinmiyor";   // kirpma yok, akis var

// KOTU: kirpma ile bildirim kapatma
var bad = FindName(7)!;             // NullReferenceException riski
```

## §4 Hata Modlari

1. `!` ile kapatilan bildirim sonradan gercek null'da patlar; bulmasi zor.
2. Her yere `?` yayilir; cagiran "her sey null olabilir" diye savasir, denetim bosal.
3. Sinirdan gelen null dogrulanmazsa iceride gec yakalanir; hata mesaji kaynagi soylemez.
4. Eski kodda nullable kapaliyken acmak yuzlerce uyarilar acar; kademeli git.

## §5 Kurallar

- Nullable'i proje genelinde tek tanimdan ac; gecisi kademeli yap.
- Donus tipi yalanci olmasin: null mumkunse `?`, mumkun degilse `?` yazma.
- Sinir (dosya, JSON, DB) null'u iceri girmeden dogrula.
- `!` kullanacagin yeri once yeniden tasarla; tasaramuyorsa yorumla acikla.
- Nullable kapali dosyalarda `#nullable enable` ile tek dosya acabilirsin.

## §6 Yasaklar

- Yalanci bildirim **YASAK**: `string` yazip surekli `?.` kullanma.
- `!` ile testten kacmak **YASAK**; testi geciren bildirim degildir.
- Sinirda dogrulamadan iceri null gecirmek **YASAK**.
- Tek seferde butun kodu nullable'a cevirmek **YASAK**; kademeli ilerle.

## §7 Test Baglantisi

- Null donen dal icin ayri test yaz; `Assert.IsNull` ile kilitle.
- Sinir dogrulamasi icin kirli girdi testi yaz: bos, eksik, fazla alan.
- Kirpma (`!`) iceren satirin testi yanlis guven verir; oncelikle o satiri yeniden yaz. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[.rules/error-handling-rules|Hata Kurallari]] · [[knowledge/csharp/generics-constraints|Tip Guvenligi]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Nullable karar noktalari kodlama standartlarinin tip guvenligi basliginda toplandi. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
- Sinir dogrulama ilkesi hata kurallarindaki dogrulama maddeleriyle uyumlu. Kaynak: [[.rules/error-handling-rules|Hata Kurallari]]; `!`/`as`/`any` yasagi [[.rules/coding-standards|Kodlama Standartlari]] icinde tekrarlanir.
