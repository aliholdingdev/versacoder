---
title: Sabitler ve Sihirli Degerler
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

# Sabitler ve Sihirli Degerler

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[knowledge/csharp/result-type-design|Result Turu]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 3/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Degisecek deger kodda gomulu kalmaz; sabit / enum / yapilandirma icine alinir, dosya yolu ve timeout literal yazilmaz.

## §0 Ozet (Answer First)

Sihirli deger dort sekilde gorunur: **tekrar eden sayisal literal** (2+ yer), **dosya yolu / URL literal**, **string mesaj ile kod karisimi**, **magic timeout**. Cozum: `internal static class Constants` veya enum, yollar `IAppPaths`, sureler yapilandirma. Kural amaci degistir: tek sefer gorunen anlasilir sabit, degisecek her sey merkezde.

## §1 Karar Tablosu

| Deger tipi | Saklama | Ornek |
| --- | --- | --- |
| Sayi tekrari | `const` / `static readonly` | `MaxDeneme = 3` |
| Kategori anahtari | `enum` | `ErrorCode.NotFound` |
| Dosya yolu | `IAppPaths` + yapilandirma | `"C:\\..."` literal yok |
| Timeout | Yapilandirma + sabit | `DbTimeouts.Query` |
| Mesaj kodu | Sabit kod + ayri metin | `kod=NotFound` + metin |

## §2 Iki Bacak

**Bacak 1 - Merkezde tut:** Ayni deger 2+ yerde geciyorsa adlandirilmistir; adlandirilmamis deger tekrar yer degistirirken her noktayi degistirmek demektir.

**Bacak 2 - Kod ile metin ayrilir:** Hata kodu sabittir (enum, yerellestirme anahtari); metin degistirilebilir ve yerellestirilebilir. Mesajdan kod ayristirmak kacincidan sonra kirilir.

## §3 Ornek

```csharp
// KOTU: iki yerde ayni sihir, biri unutulur
if (deneme >= 3) Bitti();
...
await Task.Delay(1000);

// IYI: adlandirilmis, tek kaynak
internal static class RetryAyarlari
{
    public const int MaxDeneme = 3;
    public const int IlkBeklemeMs = 250;
}

// KOTU: yol literal
var db = new SqliteConnection("Data Source=C:\\data\\app.db");

// IYI: sahiplik bir arayuzde
public interface IAppPaths
{
    string VeritabaniDosyasi { get; }
}
```

## §4 Hata Modlari

1. Literal 2+ yerde tekrar: degisiklikte biri kacar, tutarsiz davranis dogar.
2. Hardcoded yol: tasima, kurulum ve test ortaminda kirilir.
3. Mesajdan kod ayristirma: yerellestirmede ilk kirilan yer.
4. Enum'a cevrilmemis kategori string'i: buyuk/kucuk harf ve yazim farki sessiz eslesmez.

## §5 Kurallar

- 2+ tekrar eden degeri adlandir; isim neyse o.
- Yollari ve ortam bilgisini `IAppPaths` benzeri tek arayuzden ver.
- Timeout/sayac degerini yapilandirmaya bagli tut, kodda dogrudan yazma.
- Hata kodlarini enum'da tut; metni ayri yerde uret.
- Sabit sinifini `internal static` ile sinirla; disariya gereksiz acilma.

## §6 Yasaklar

- Ayni literal'in 2+ yerde kopyasi **YASAK**.
- `"C:\\..."` / URL literal kodda **YASAK**.
- String mesajdan hata kodu ayristirmak **YASAK**.
- Sayisal magic'i yorumla aciklamak **YASAK**; adlandir.

## §7 Test Baglantisi

- Sabit degeri assert'te ayni sabitten oku; kopyalanmis beklenti yazma (ikisi de degisirse test gecmez).
- Yapilandirma degeri testte sabitlenir; ortama bagli test kararsiz olur.
- Enum kodlarini dogrudan karsila; string ile degil. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[knowledge/csharp/result-type-design|Result Turu]] · [[knowledge/csharp/naming-and-comments|Isimlendirme ve Yorum]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Yasaklar (tekrar literal, yol, mesajdan kod) kodlama standartlarinin magic deger maddesinde. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
- `IAppPaths` ornegi ayni dosyada gecen duzeltme yolu; proje gerceklilesmesi DOĞRULAMA GEREKLİ.
