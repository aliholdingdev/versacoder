---
title: Result Turu Tasarimi
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# Result Turu Tasarimi

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Hata Kurallari](../../.rules/error-handling-rules.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [C Sharp Index](index.md)

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
- Hata kodunu string ile degil sabitle karsila; degisirse test kirilsin. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Hata Kurallari](../../.rules/error-handling-rules.md) · [Kodlama Standartlari](../../.rules/coding-standards.md) · [Exception Kullanimi](exception-handling-patterns.md) · [C Sharp Index](index.md)

## §9 Kanit

- Istisna/Result ayrimi hata kurallarindaki sinir ve kurtarma maddeleriyle uyumlu. Kaynak: [Hata Kurallari](../../.rules/error-handling-rules.md).
- Onek yapisi kaynak vault C# setinden yeniden kurgulandi, birebir kopya degil. Kaynak: kaynak vault, 2026-09-25.
