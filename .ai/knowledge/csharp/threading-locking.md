---
title: Thread ve Kilit (Locking) Desenleri
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# Thread ve Kilit (Locking) Desenleri

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Async Kurallari](../../.rules/async-rules.md) · [Kodlama Standartlari](../../.rules/coding-standards.md) | [C Sharp Index](index.md)

> **Tek cumlelik sorumluluk:** Kilitleme kucuk ve disari acilamayan olsun; kilit nesnesi disindan erisilebiliyorsa emniyet yoktur.

## §0 Ozet (Answer First)

Dort kural: **kilit disari acilamaz** (`private readonly object`), **kilit icinde await yok** (semaphore'a gerekir), **olcu icin farkli yapilandiricilar yeter** (`Interlocked`, `lock` yerine artis), **durum yapisi kucuk olsun**. Kilitli kodda ayrica iki kilidin oldugu yerde kilit sirasi olcer - ters sirada kilitlenme (deadlock) gelir.

## §1 Karar Tablosu

| Araç | Ne korur | Nerede |
| --- | --- | --- |
| `lock` | Tek nesne, tek is parcasi | Kisa kritik bolge |
| `Interlocked` | Sayac artis/karsilastirma | Kilit gereksizse |
| `SemaphoreSlim` | Async + kaynak sayisi | await'li bolge |
| `Mutex` | Sistem geneli (process arasi) | Nadir; dosya kilit |
| Immutable yapilandirma | Paylasim tamamen | Tercih edilen ilk yol |

## §2 Iki Bacak

**Bacak 1 - Kucuk kritik bolge:** Kilit icinde tek sey yap: durumu guncelle veya kaynagi ayr. Ag, dosya, log, konsol cikisi kilit icinde **YASAK** - sureyi uzatir, deadlock zeminini genisletir.

**Bacak 2 - Kilitten once tasarim:** Paylasilan durumu kucult, deger tipiyle tasima, degistirme yerine yeniden uret. Immutability varsa kilit ihtiyaci duser; kilit son care.

## §3 Ornek

```csharp
private readonly object _kilit = new();
private readonly Dictionary<int, Kayit> _kayitlar = new();
private int _sayac;

public void Ekle(Kayit k)
{
    lock (_kilit)                 // kilit disari acik degil
    {
        _kayitlar[k.Id] = k;
    }
}

public int Artir() => Interlocked.Increment(ref _sayac);  // kilit yok

// YASAK: await'li bolge + lock
// lock (_kilit)
//     await YukleAsync();        // derleyici hata verir, dogru
```

## §4 Hata Modlari

1. Kilidi disaridan vermek (`lock(nesne)` + disaridan `Monitor.Enter`) - emniyet yok; `private` alani kilitle.
2. Kilit disinde await: `lock` async'i desteklemez; `SemaphoreSlim.WaitAsync` gerekir.
3. Iki kilit ayni anda: her yerde ayni sira izle; sirayi kirarsan deadlock.
4. Kilit disinda okuma: tutarli goruntu imkansiz; `volatile` tek basina atomiklik vermez.

## §5 Kurallar

- Kilit nesnesini `private readonly object` ile tanimla; disari acma.
- Kritik bolgeyi kucult; disari cagrilari kilit disina tasi.
- Artis/karsilastirma icin once `Interlocked` dene.
- Async bolge icin `SemaphoreSlim` veya kilitsiz tasarim.
- Coklu kilitte global sirayi kesin tut.

## §6 Yasaklar

- `lock` icinde `await`, IO, log veya GUI guncelleme **YASAK**.
- Disaridan verilen veya `this`, `typeof`, string literal kilidi **YASAK**.
- Kilit disinda paylasilan degisken yazmak **YASAK**.
- `volatile` yazip kilit yerine kullanmak **YASAK**; sadece gorunurluk verir, atomik degil.

## §7 Test Baglantisi

- Eyni anda cagri testi: paralel cagirida beklenen sayiyi `Assert.AreEqual` ile kilitle.
- Deadlock testi icin zaman siniri koy; kilitlenen test suite'i durdurur.
- Kilit mantigi testte dogrulanamazsa kod kucult - buyuk kritik bolge test edilemez. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Async Kurallari](../../.rules/async-rules.md) · [Kodlama Standartlari](../../.rules/coding-standards.md) · [Kanal ve Kuyruk](channel-queue-patterns.md) · [C Sharp Index](index.md)

## §9 Kanit

- Kilit disari acilamaz + await'li bolge Semaphore'a gecer kurallari async kural setiyle uyumlu. Kaynak: [Async Kurallari](../../.rules/async-rules.md).
- Ornekler `Interlocked` ve `lock` sinirlariyla sinirli tutuldu; `Monitor`/`ReaderWriterLockSlim` detaylari kaynak vault'ta, surum notlari icin DOĞRULAMA GEREKLİ.
