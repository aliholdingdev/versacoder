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

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../claude.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [Hata Kurallari](../../.rules/error-handling-rules.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 2/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

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

**Bacak 2 - Runtime dogrulama:** Tip sistemi sinir disindan (JSON, veritabani, dosya) gelen null'i bilmez. Sinirda dogrula, iceriye temiz veri gecir. [Hata Kurallari](../../.rules/error-handling-rules.md) gecer.

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
- Kirpma (`!`) iceren satirin testi yanlis guven verir; oncelikle o satiri yeniden yaz. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Kodlama Standartlari](../../.rules/coding-standards.md) · [Hata Kurallari](../../.rules/error-handling-rules.md) · [Tip Guvenligi](generics-constraints.md) · [C Sharp Index](index.md)

## §9 Kanit

- Nullable karar noktalari kodlama standartlarinin tip guvenligi basliginda toplandi. Kaynak: [Kodlama Standartlari](../../.rules/coding-standards.md).
- Sinir dogrulama ilkesi hata kurallarindaki dogrulama maddeleriyle uyumlu. Kaynak: [Hata Kurallari](../../.rules/error-handling-rules.md); `!`/`as`/`any` yasagi [Kodlama Standartlari](../../.rules/coding-standards.md) icinde tekrarlanir.
