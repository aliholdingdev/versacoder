---
title: Kaynak Biritma Desenleri (Resource Disposal)
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

# Kaynak Biritma Desenleri (Resource Disposal)

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[.rules/error-handling-rules|Hata Kurallari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Her `new` edilen IDisposable `using`/`await using` ile sarilir; sahiplik kurali yorumda degil tasarimda.

## §0 Ozet (Answer First)

Sahiplik tablosu: **baglantiyi acan metot kapatir** (`using var conn`), **transaction baglantinin sahibindedir**, **process'i calistiran zaman asiminda kill eder**, **DevExpress kontrolunu elle dispose etmez** (Designer yonetir). Sizan handle → dosya kilidi, baglanti havuzu tukener.

## §1 Karar Tablosu

| Kaynak | Sahip | Desen |
| --- | --- | --- |
| `DbConnection` | cagiran repository metodu | `using var conn = ...;` asla static alan |
| `DbTransaction` | baglanti sahibi metot | `using var tx` + tek commit |
| `Process` | tool calistirici | `using` + `WaitForExit(timeout)` + asimda `Kill` |
| Log `StreamWriter` | log altyapisi | kapanista flush + dispose |
| DevExpress kontrol | Designer (`IContainer`) | elle dispose **yapilmaz** |

## §2 Iki Bacak

**Bacak 1 - `using` varsayilani:** C# `using` deyimi hataya karsi da dispose eder; `try/finally` ile elle yazmak yerine `using var` tek satir. Async kaynaklarda `await using` (IAsyncDisposable).

**Bacak 2 - Sahiplik tasarimda belli:** Bir nesneye disaridan verilen kaynagi dispose etme; sahibi belirsizse kimse kapatmaz (sizma) ya da ikisi kapatir (erken kapanis). Sahipligi imza/constructor ile belli et.

## §3 Ornek

```csharp
// IYI: sahiplik metot basinda, tek satir
public async Task<Satir?> BulAsync(int id, CancellationToken cancellationToken)
{
    await using var conn = _factory.Ac();          // sahip: bu metot
    return await conn.QuerySingleOrDefaultAsync<Satir>(
        "SELECT Id, Ad FROM dbo.Satir WHERE Id = @Id",
        new { Id = id }, cancellationToken: cancellationToken);
}

// KOTU: sahipsiz baglanti
private SqlConnection _conn;                       // kim kapatiyor?
```

```csharp
// Process: zaman asimi kontrolu sart
using var p = Process.Start(bilgi)!;
if (!p.WaitForExit(30_000)) { p.Kill(entireProcessTree: true); throw new TimeoutException(); }
```

## §4 Hata Modlari

1. `using`siz baglanti: havuz tukener, ikinci istekte "pool exhausted".
2. Static alanda tutulan baglanti: kilitli dosya / paylasilan durum.
3. Elle `Dispose` edilmis Designer kontrolu: calisma aninda bozulma.
4. Hata yolunda commit/uncommit unutulur: transaction baglantida kilit kalir.

## §5 Kurallar

- `using`/`await using` varsayilan; elle `try/finally` yalnizca `using` yetmediginde.
- Baglantiyi acan metot kapatir; constructor'da acma.
- Process'te zaman asimi + kill yolu hep yazilsin.
- Sahiplik belirsizse once tasarimi duzelt; yorumla cozme.
- Dispose edilebilir alanlarin IDisposable oldugunu derleyiciye sor; `as` ile kacma.

## §6 Yasaklar

- `new SqliteConnection(...)` `using`siz **YASAK**.
- Static alan baglanti **YASAK**.
- Elle dispose edilmis DevExpress kontrolu **YASAK** (Designer yonetir).
- `Dispose`'u unutmak icin GC'ye guvenmek **YASAK**; finalizer plan degil, son caresizliktir.

## §7 Test Baglantisi

- Entegrasyon testi gecici `.db` dosyasi kullanir ve test sonunda siler; kilit kalirsa sonraki test kirmizi olur.
- Kaynak sizmasi testte gorunur: ard arda cagri sayisini artirip hata almamali.
- Hata yolunda da dispose edildigini test et (finally yolu). Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[.rules/error-handling-rules|Hata Kurallari]] · [[knowledge/csharp/result-type-design|Result Turu]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Sahiplik tablosu kodlama standartlarinin kaynak yonetimi bolumunden derlendi. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
- DevExpress satiri Designer dosyasi tarafindan yonetilir kuralina dayanir; surum bazli istisna DOĞRULAMA GEREKLİ.
