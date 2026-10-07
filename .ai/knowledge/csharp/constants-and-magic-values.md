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

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../claude.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [Result Turu](result-type-design.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 3/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

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
- Enum kodlarini dogrudan karsila; string ile degil. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Kodlama Standartlari](../../.rules/coding-standards.md) · [Result Turu](result-type-design.md) · [Isimlendirme ve Yorum](naming-and-comments.md) · [C Sharp Index](index.md)

## §9 Kanit

- Yasaklar (tekrar literal, yol, mesajdan kod) kodlama standartlarinin magic deger maddesinde. Kaynak: [Kodlama Standartlari](../../.rules/coding-standards.md).
- `IAppPaths` ornegi ayni dosyada gecen duzeltme yolu; proje gerceklilesmesi DOĞRULAMA GEREKLİ.
