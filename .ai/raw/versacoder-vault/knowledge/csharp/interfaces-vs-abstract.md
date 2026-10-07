---
title: "interfaces-vs-abstract - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "interface ile abstract class secimi, default interface metodu ve iki bacakta kalitim/arayuz farklari"
---

# interfaces-vs-abstract — Arayüz mü, soyut sınıf mı

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [generics-constraints](generics-constraints.md) · [records-and-immutability](records-and-immutability.md)

> **Tek cumlelik sorumluluk:** *Bir davranis arayuzde mi, soyut sinifta mi yasar; C# 14'te arayuzlara ne eklendi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault'ta secim kurali net: **sozlesme = `interface`, uygulama tekrari = soyut sinif degil, `abstract` yalnizca paylasilan durum/varyasyon noktasinda**. Katmanlar arasi bagimlilik yalnizca `interface` uzerinden yonlenir (l0 portlari, DIP); bir sinif baska katmanin somut tipine bakamaz. C# 8 ile gelen **default interface metodu (DIM)** `net48`'de **calisma zamani destegi olmadigi icin kullanilmaz**; C# 14'ün extension member'lari ise yalniz `net10` bacakta. Sonuc: **ortak sozlesme 7.3 uyumlu `interface` + `abstract` kalir.**

---

## §1 Secim Tablosu

| Soru | Cevap | Secim |
|---|---|---|
| Tip bir **sozlesme** mi (davranis, degismez) mi? | Evet | `interface` |
| Cok paylasilan **durum** (alan) var mi? | Evet | `abstract` sinif (tek kalitim zinciri) |
| Farkli ailelerden tipler ayni davranisi paylasacak mi? | Evet | `interface` (coklu kalitim) |
| Cagiran yalnizca bir imzayi mi biliyor (DIP) | Evet | `interface` + DI |
| Kurucu parametreleri/paylasilan alan zorunlu mu | Evet | `abstract` sinif |
| Test icin taklit (mock) gerekiyor mu | Evet | `interface` (NSubstitute) |

> **KURAL:** `interface`'e davranis, `abstract` sinifa **durum + varyasyon noktasi** konur. `abstract` sinifta tek bir kalirim zinciri oldugu icin onu miras almak bir **taahhuttur**; `interface` imzasi ise genelde geriye donuk uyumludur.

## §2 Iki Bacakta

| Ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `interface`, `abstract class`, `virtual`/`override` | Var | Var |
| Explicit interface implementasyonu | Var | Var |
| Default interface metodu (C# 8) | **Yok** — runtime destegi yok | Var |
| `static abstract` arayuz uyesi (C# 11) | **Yok** | Var |
| `default`/`notnull` konusu arayuzlari (NRT) | Dil surumu 7.3'te annotation yok | Var |
| Arayuz icinde `private` yardimci (C# 8) | **Yok** | Var |
| C# 14 `extension` uye bloklari | **Yok** | Var |
| `record` ile immutable soyut veri (C# 9) | **Yok** (bkz. [records-and-immutability](records-and-immutability.md)) | Var |

> **UYARI:** DIM'i ortak koda yazmak en pahali hatadir: derleyici kabul etmeyebilir ya da `net48` calisma zamaninda sessizce farkli davranir. Ortak sozlesme **soyut imza** olarak kalir, varsayilan davranis **somut sinifta** yasar.

## §3 Ornek

```csharp
// l0 sozlemesi — iki bacakta da ayni
internal interface ISessionStore
{
    Task<Result<SessionDto>> KaydetAsync(SessionDto dto, CancellationToken ct);
}

// ortak davranis — somut sinifta, arayuzde degil
internal abstract class StoreBase : ISessionStore
{
    protected const int ZamanAsimi = 15;          // paylasilan durum
    public abstract Task<Result<SessionDto>> KaydetAsync(SessionDto dto, CancellationToken ct);
}

internal sealed class SqliteStore : StoreBase
{
    public override Task<Result<SessionDto>> KaydetAsync(SessionDto dto, CancellationToken ct)
        => throw new NotImplementedException();     // ornek govde
}
```

| Yapilandirma | Ne kazandirir | Bedeli |
|---|---|---|
| `interface` (l0 port) | Test edilebilir, DIP, mock | Davranis her somutta tekrar |
| `abstract` sinif | Paylasilan alan + degistirilebilir adim | Tek kalirim zinciri |
| Concrete bagimlilik | Hizli baslangic | Katman kiriligi (NetArchTest kirmizi) |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Tek uygulamali `interface` + `Manager` eki | Isim gurultusu, sozlesme degil | Anlamli isim (`ISessionStore`) |
| 2 | Arayuzde varsayilan davranis (DIM) denemesi | net48 kirigi | Somut sinifa tasi |
| 3 | `abstract` sinifi ortak kutu olarak kullanmak | Kalirim kirliligi | `interface` + composition |
| 4 | Somut tipin arayuz olmayan uyesini cagirmak | Katman ihlali | Yalniz arayuz uyesi |
| 5 | Testte sinifi miras alarak mock yapmak | Kirilgan test | `interface` + NSubstitute |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Katman yonu | NetArchTest: somut tip baglantisi 0 |
| DIM kullanimi | Kod taramasinda `net48` ortak dosyada 0 |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |
| Kural | [coding-standards](../../.rules/coding-standards.md) |

Arayuz imzasi degisikligi public/ortak katmandaysa `architect` onayi gerekir (sozlesme degisikligi).
