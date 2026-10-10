---
title: Yeniden Deneme ve Timeout Politikasi
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

# Yeniden Deneme ve Timeout Politikasi

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/error-handling-rules|Hata Kurallari]] | [[.rules/async-rules|Async Kurallari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Yeniden deneme yalnizca gecici ve idempotent hata icindir; 3 deneme, ustel geri cekilme 250-500-1000 ms.

## §0 Ozet (Answer First)

Politika dort parca: **sart** (gecici + idempotent), **sayi** (3), **geri cekilme** (250 -> 500 -> 1000 ms), **bitis** (3. deneme de basarisizsa ERROR log + escalation). Timeout her sorguda aciktir; iptal hata degildir.

## §1 Karar Tablosu

| Durum | Yeniden dene mi | Aksiyon |
| --- | --- | --- |
| Baglanti zaman asimi / gecici ag | Evet (3, ustel) | Ayni istek, geri cekilme |
| Deadlock | Evet (1 kez) | Islemi yeniden calistir |
| Unique / DuplicateKey | Hayir | `ErrorCode.Conflict` don |
| Dogrulama hatasi | Hayir | Kullaniciya don |
| Yetkisizlik | Hayir | `ErrorCode.Unauthorized` |
| 3. deneme de basarisiz | Hayir | ERROR log + 3-STRIKE escalation |

## §2 Iki Bacak

**Bacak 1 - Gecicilik + idempotentlik:** Ayni istegi tekrarlayip ayni etkiyi uretebilmelisin (idempotent degilse tekrar tehlikelidir). Unique ihlali, dogrulama ve yetki hicbir kosulda tekrarlanmaz.

**Bacak 2 - Timeout + iptal birlestigi:** Ustten gelen token ile timeout link edilir (`CreateLinkedTokenSource` + `CancelAfter`). `Task.WhenAny` ile sessizce devam etmek alternatif degil, kacinilacak kalipdir; iptal exception'u yutulmaz.

## §3 Ornek

```csharp
// Ustteki token + timeout link edilerek birlestirilir
using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
linked.CancelAfter(TimeSpan.FromMilliseconds(30_000));

for (var deneme = 1; deneme <= 3; deneme++)
{
    try
    {
        return await CalisAsync(linked.Token).ConfigureAwait(false);
    }
    catch (OperationCanceledException) when (linked.IsCancellationRequested)
    {
        throw;                       // iptal hata degil, yutulmaz
    }
    catch (Exception ex) when (GeciciMi(ex) && deneme < 3)
    {
        var bekle = 250 * (int)Math.Pow(2, deneme - 1);   // 250 -> 500
        await Task.Delay(bekle, cancellationToken);
    }
}
```

## §4 Hata Modlari

1. Idempotent olmayan islemde tekrar: cift kayit, cift odeme.
2. Sabit kisa arayla cok deneme: agi bogar, gercek cozumu geciktirir.
3. Timeout'suz bekleme: uygulama askida kalir, iptal edilemez.
4. Iptali ERROR sayip log yigmak: gercek hata kaybolur.

## §5 Kurallar

- Once gecici mi sor; gecici degilse deneme sayisi 0.
- Geri cekilmeyi ustel yap: 250 -> 500 -> 1000 ms, sabit 3 deneme.
- Her sorguda acik timeout tut (ornek 30 sn).
- Timeout'u ustteki token'a link et; ayni iptal zincirinden yonlendir.
- 3. basarisizlikta dur, escalation yaz; kod yazmaya devam etme.

## §6 Yasaklar

- Gecici olmayan hatayi (duplicate, dogrulama, yetki) tekrar denemek **YASAK**.
- 3'ten fazla deneme **YASAK**; gerekce + onay gerekir.
- `Task.WhenAny` ile sessiz devam **YASAK**.
- Iptali `ERROR` saymak **YASAK**; `Result.Cancelled` ile isaretle.

## §7 Test Baglantisi

- Iptal yolu testi zorunlu: yalniz mutlu yol testi bu politikanin kaniti degildir.
- Geri cekilme araligini testte sabitle (sahte zaman); gercek `Task.Delay` ile test yazma.
- Uc deneme sayisini ve basarisizlik sonucunu assert ile kilitle. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/error-handling-rules|Hata Kurallari]] · [[.rules/async-rules|Async Kurallari]] · [[knowledge/csharp/result-type-design|Result Turu]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- 3 deneme / 250-500-1000 ms / escalation sayfalari hata kural dosyasinin politika bolumunde. Kaynak: [[.rules/error-handling-rules|Hata Kurallari]].
- Token-link ornegi async kural dosyasinin timeout maddesiyle ayni kalip. Kaynak: [[.rules/async-rules|Async Kurallari]]; somut timeout degerleri proje bazli DOĞRULAMA GEREKLİ.
