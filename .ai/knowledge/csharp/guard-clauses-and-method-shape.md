---
title: Guard Clause ve Metot Sekli
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

# Guard Clause ve Metot Sekli

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../claude.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [SOLID Uygulamalari](solid-principles-applications.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 3/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** Erken return ile ic ice gecmeyi 3 seviyede tut; metot 50 satir ve cyclomatic 10 sinirini asarsa sorumluluk yanlistir.

## §0 Ozet (Answer First)

Iki olcum: **sekil** (ic ice <= 3, kule if/else yok, metot <= 50 satir, sinif <= 500 satir) ve **seklin bozulmasinin anlami** (metot uzuyorsa birden fazla sorumluluk = SOLID ihlali). Cozum her zaman ayni: kosulu negatifleyip erken return/throw, sonra metodu bol.

## §1 Karar Tablosu

| Sinir | Deger | Asiminda |
| --- | --- | --- |
| Ic ice gecme | <= 3 seviye | Kosulu negatifleyip erken return |
| Kule if/else | yok | Metodu bol |
| Metot uzunlugu | <= 50 satir | Yardimci metoda bol (gerekce yazilir) |
| Cyclomatic karma siklik | <= 10 | Dalari ayri metoda tasi |
| Sinif uzunlugu | <= 500 satir | Sorumlulugu sinfa tasi (SRP) |

## §2 Iki Bacak

**Bacak 1 - Guard clause on planda:** Gecersiz veya erken sonuc veren durumlar metot basinda temizlenir; gercek is tek duz govdede kalir. Okunurluk artar, inceleme mumkun olur.

**Bacak 2 - Limit bir uyaridir, hedef degil:** Sınir asimi otomatik hata degildir; gerekceli asim kabuldur. Gerekcesiz asim = inceleme RED. Uzunlugu kendin gorebilirsin, kisiti kod incelemesi gorebilir.

## §3 Ornek

```csharp
// KOTU: 4 seviye ic ice + kule
if (kayit != null)
{
    if (kayit.Aktif)
    {
        if (kayit.Bakiye > 0)
        {
            if (kayit.Onayli) IslemYap();
            else HataVer();
        }
        else HataVer();
    }
    else HataVer();
}
else HataVer();

// IYI: guard clause + duz govde
if (kayit is null) throw new ArgumentNullException(nameof(kayit));
if (!kayit.Aktif)  { HataVer(); return; }
if (kayit.Bakiye <= 0) { HataVer(); return; }
if (!kayit.Onayli) { HataVer(); return; }

IslemYap();
```

## §4 Hata Modlari

1. Derin govde kurali saklar: 4. seviyedeki kosul incelemeden kacar.
2. 150 satirlik metot + 12 if: test edilemez, bakilamaz.
3. Bolunurken yanlis bolme: ayni sorumluluk iki metoda yayilir; sinir degil, sorumluluk sorulur.
4. `else` kuleleri: her yeni dal eski dallari kirar; erken return yayginlasirsa kule kalmaz.

## §5 Kurallar

- Gecersiz durumlar metot basinda temizlensin; gercek is tek govdede kalsin.
- Kosulu negatifleyip erken `return`/`throw` sec; okunu asagi tut.
- Metot uzarsa once "ikinci sorumluluk nedir" diye sor; yardimci metot ikinci adim.
- Uzunluk asiminda gerekceyi dosyada degil incelemede yaz.
- Sinif boyutu asildiginda sinif degil sorumluluk tasinir.

## §6 Yasaklar

- 4+ seviye ic ice govde **YASAK**.
- Gerekcesiz 100+ satir metot **YASAK**.
- `else` kulesi kurup her dali iciine gizlemek **YASAK**.
- Limiti asinca yorum satiriyla savunmak **YASAK**; gerekce incelemede.

## §7 Test Baglantisi

- Guard clause'larin her biri ayri test ister: bos, gecersiz, yetkisiz girdi.
- Metot bolundukce testler kuculur; kucuk test = okunur assertion.
- Sekil asimi test sayisini artiriyorsa dogru yoldasin. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Kodlama Standartlari](../../.rules/coding-standards.md) · [SOLID Uygulamalari](solid-principles-applications.md) · [Isimlendirme ve Yorum](naming-and-comments.md) · [C Sharp Index](index.md)

## §9 Kanit

- Sinir degerleri (3 / 50 / 10 / 500) kodlama standartlarinin metot sekli maddesinde. Kaynak: [Kodlama Standartlari](../../.rules/coding-standards.md).
- "Asim gerekcesi yazilir" notu ayni dosyada; otomatik metrik taramasinin calisirligi DOĞRULAMA GEREKLİ.
