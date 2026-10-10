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

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[knowledge/csharp/solid-principles-applications|SOLID Uygulamalari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 3/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

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
- Sekil asimi test sayisini artiriyorsa dogru yoldasin. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[knowledge/csharp/solid-principles-applications|SOLID Uygulamalari]] · [[knowledge/csharp/naming-and-comments|Isimlendirme ve Yorum]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Sinir degerleri (3 / 50 / 10 / 500) kodlama standartlarinin metot sekli maddesinde. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
- "Asim gerekcesi yazilir" notu ayni dosyada; otomatik metrik taramasinin calisirligi DOĞRULAMA GEREKLİ.
