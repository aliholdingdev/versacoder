---
title: Result Turu Tasarimi
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

# Result Turu Tasarimi

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/error-handling-rules|Hata Kurallari]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Beklenen basarisizligi istisna degil veri olarak dondur; `Result<T>` hata kodunu, mesaji ve degeri ayni imzada tasir.

## §0 Ozet (Answer First)

Istisna olağan disi icin, Result beklenen icin. Tasarim dort parca: **deger** (`Value`), **hata** (`Error`), **yardimci** (`IsSuccess`, `Match`) ve **kural** (hata varsa deger yok - diskoidans imkansiz). Hata tipleri enum/kayit olur, serbest string degil; boylece cagiran dallanabilir.

## §1 Karar Tablosu

| Secenek | Ne is yapar | Nerede kullanilir |
| --- | --- | --- |
| `Result` | Deger yok, sonuc var | Komutlar |
| `Result<T>` | Deger + hata | Sorgular |
| Hata kaydi | Kod + mesaj + alan | Dogrulama |
| `Match` | Iki dali zorla | Cagiran taraf |
| `Try` sarmalayici | Istisnayi Result'a cevir | Sinir katmani |

## §2 Iki Bacak

**Bacak 1 - Diskoidans imkansiz:** `IsSuccess == true` iken `Error == null`, degilse `Value` yok. Bunu yapici (private ctor + fabrika) zorlar; disariya set birakma.

**Bacak 2 - Cagiran zorlanir:** `Match` veya `IsSuccess` kontrolu imzayi gorunur kilar. Uzaklarda firlatilan hata yerine, donus tipi hata aykiri oldugunu soyler.

## §3 Ornek

```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value) { IsSuccess = true; Value = value; }
    private Result(Error error) { IsSuccess = false; Error = error; }

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(Error error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> ok, Func<Error, TResult> fail)
        => IsSuccess ? ok(Value!) : fail(Error!);
}

public sealed record Error(string Code, string Message, string? Field = null);
```

```csharp
var sonuc = Bul(7);
var ekran = sonuc.Match(v => $"adi: {v.Ad}", e => $"hata: {e.Code}");
```

## §4 Hata Modlari

1. `Value` ve `Error` ayni anda set edilirse cagiran hangisine guvenecegini bilemez; yapi buna izin vermemeli.
2. `Error` serbest string olursa dal yazilamaz; kod enum veya sabit olmali.
3. Sonuc yok sayilir (`_ = sonuc`); o zaman Result yazmanin anlami yok, istisna kullan.
4. Tum hatalar Result'a sarilirsa olağan disi (NullReference, disk bozuklugu) gizlenir; sinir disi istisna Result olmaz.

## §5 Kurallar

- Beklenen basarisizlik (dogrulama, bulunamadi, yetki yok) Result ile.
- Hata turunu kucuk ve kategorik tut: kod + mesaj + istege bagli alan.
- Cagiran icin `Match` veya acik `IsSuccess` kontrolu; hicbiri yoksa hata.
- Ust katmanlara cikarken istisna kuralini uygula; sinirda cevir.
- `Result<T>`'yi generic birak; is kuralina gore ayri tipler uretme.

## §6 Yasaklar

- Hata firlatip "kullanici goruyor" demek **YASAK**; beklenen hata Result.
- Bos (kodsuz) hata **YASAK**; her hatanin kodu olur.
- `Result` disinda `IsSuccess` tutmak **YASAK**; dogruluk turde kalsin.
- Diskoidans (Value + Error ayni anda) mumkun kilan yapilandirici **YASAK**; private tut.

## §7 Test Baglantisi

- Dali `Match` uzerinden test et: ok dal, fail dal ve kod eslesmesi uc test.
- Yapi kurallarini test et: `Fail` ile `Value` null'dir, `Ok` ile `Error` null'dir.
- Hata kodunu string ile degil sabitle karsila; degisirse test kirilsin. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/error-handling-rules|Hata Kurallari]] · [[.rules/coding-standards|Kodlama Standartlari]] · [[knowledge/csharp/exception-handling-patterns|Exception Kullanimi]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Istisna/Result ayrimi hata kurallarindaki sinir ve kurtarma maddeleriyle uyumlu. Kaynak: [[.rules/error-handling-rules|Hata Kurallari]].
- Onek yapisi kaynak vault C# setinden yeniden kurgulandi, birebir kopya degil. Kaynak: kaynak vault, 2026-09-25.
