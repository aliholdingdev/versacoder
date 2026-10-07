---
title: Test Determinizmi
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: SSOT
scope: csharp
---

# Test Determinizmi

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Test Kurallari](../../.rules/testing-rules.md) | [Assertion Kalitesi](assertion-quality.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (csharp · 3/28):** [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** Duvar saati, rastgelelik ve ortak durum testi kararsiz yapar; flaky test RED'dir, `Retry` ile gizlenmez.

## §0 Ozet (Answer First)

Alti kararsizlik kaynagi ve karsiligi: `DateTime.UtcNow` -> `TimeProvider`, `Random` -> `IRandom`, `Guid.NewGuid` -> `IGuidFactory`, gercek HTTP/LLM -> fake provider, ortak `.db` -> benzersiz gecici dosya, paralelde port/dosya -> collection + benzersiz ad. Kural: tekrar calistirinca degisen test **hata** sayilir.

## §1 Karar Tablosu

| Kararsizlik kaynagi | Seam / cozum |
| --- | --- |
| `DateTime.UtcNow` | `TimeProvider` (`GetUtcNow()`) |
| `Random` | `IRandom` / sabit tohum |
| `Guid.NewGuid` | `IGuidFactory` (testte sabit sira) |
| Gercek LLM/HTTP | fake `IModelProvider` (sabit yanit) |
| Ortak `.db` dosyasi | Benzersiz gecici dosya |
| Paralelde port/dosya | Collection + benzersiz ad |

## §2 Iki Bacak

**Bacak 1 - Dis bagimliliklari seam'e cevir:** Kod dogrudan `DateTime.Now` cagiriyorsa test yarin farkli sonuc verir. Zaman/rastgele/uretim disari cikarilir; testte sabit deger kurulur.

**Bacak 2 - Izolasyon + kendi verisi:** Her test kendi verisini builder/fixture ile kurar; siraya ve ortak mutable globala bagimli degildir. Temizleme `IDisposable` fixture ile yapilir.

## §3 Ornek

```csharp
// KOTU: duvar saati - yarin farkli sonuc
[Fact]
public void Oturum_24_Saat_Sonra_Suresi_Duser()
{
    var yas = DateTime.UtcNow - olusturmaZamani;
    Assert.True(yas.TotalHours > 24);   // her zaman gecmise gore "dogru": bos test
}

// IYI: sabit saat + kendi verisi + somut iddia
[Fact]
public async Task ExpireAsync_WhenSessionOlderThan24Hours_MarksSessionExpired()
{
    var t0 = new DateTimeOffset(2026, 9, 25, 12, 0, 0, TimeSpan.Zero);
    using var db = TestDb.New();
    var id = await _repo.InsertAsync(TestData.Session(CreatedAt: t0.AddHours(-25)),
                                     CancellationToken.None);

    var sonuc = await _service.ExpireAsync(id, CancellationToken.None);

    Assert.True(sonuc.IsSuccess);
    var kayit = await _repo.FindAsync(id, CancellationToken.None);
    Assert.Equal(SessionState.Expired, kayit!.State);   // kaynak durumu dogrulanir
}
```

## §4 Hata Modlari

1. Saate bagli test: gecmise gore hep gecer, hicbir seyi kanitlamaz (bos test).
2. `[Retry(3)]` / `Skip=` ile gizleme: yeşilin anlami biter, koken bulaniklasir.
3. Ortak `test.db`: paralel kosuda "database is locked".
4. Sira bagimli test: tek basina gecer, tam suite'te duser.
5. Gercek API testi: anahtar, gecikme, suruklenme = kararsizlik + maliyet.

## §5 Kurallar

- Zaman, rastgele, uretim kodu seam uzerinden gelsin; testte sabitle.
- Her test kendi verisini kursun; once/sonra bagimliligi yazma.
- Paralel kosuyu varsay; ortami benzersiz adla izole et.
- Flaky gorunce `Retry` ekleme; determinizm ihlalini bul (TS-04/05/06).
- Gercek ag cagrisi testte **0** olmali; fake provider + sabit govde.

## §6 Yasaklar

- Dogrudan `DateTime.Now/UtcNow`, `Random`, `Guid.NewGuid` **YASAK**.
- `Retry`/`Skip` ile flaky gizlemek **YASAK**.
- Testte gercek `HttpClient` / LLM cagrisi **YASAK**.
- Ortak mutable global durum **YASAK**.
- `// bu test X'ten sonra calismali` yorumu **YASAK**; veriyi testte kur.

## §7 Test Baglantisi

- Determinizm testi basit: ayni testi 3 kez calistir, hepsi ayni sonuc (kanit: cikti sayisi).
- Unit testte veritabani/ekran/dosya yok; gercek SQLite yalniz entegrasyonda (gecici dosya).
- Iptal/hata yollari da test edilir; yalniz mutlu yol eksik kapsamdır. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Test Kurallari](../../.rules/testing-rules.md) · [Assertion Kalitesi](assertion-quality.md) · [Iptal Tokeni](cancellation-token-policy.md) · [C Sharp Index](index.md)

## §9 Kanit

- Alt kaynak/kesim tablosu test kural dosyasinin determinizm bolumunden derlendi. Kaynak: [Test Kurallari](../../.rules/testing-rules.md), 2026-09-25.
- Seam uretecileri (`TimeProvider` vb.) proje bazinda kurulu mu: DOĞRULAMA GEREKLİ.
