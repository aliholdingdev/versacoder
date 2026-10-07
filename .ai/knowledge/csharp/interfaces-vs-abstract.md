---
title: "interfaces-vs-abstract - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "interface ile abstract class secimi, default interface metodu yasagi ve iki bacakta kalitim/arayuz farklari"
---

# interfaces-vs-abstract — Arayüz mü, soyut sınıf mı

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [coding-standards](../../.rules/coding-standards.md) · [solid-rules](../../.rules/solid-rules.md) · [generics-constraints](generics-constraints.md) · [layer-dependency-direction](layer-dependency-direction.md)
**Küme + Hub:** [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 5/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** *Bir davranis arayuzde mi, soyut sinifta yasar; default interface metodu neden kullanilmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault'ta secim kurali net: **sozlesme = `interface`**, paylasilan **durum + varyasyon noktasi = `abstract` sinif**; `abstract` ortak kutu olarak kullanilmaz. Katmanlar arasi bagimlilik yalnizca `interface` uzerinden yonlenir (l0 portlari, DIP — `SOL-05`); bir sinif baska katmanin somut tipine bakamaz. C# 8 ile gelen **default interface metodu (DIM)** `net48`'de calisma zamani destegi olmadigi icin **kullanilmaz**; C# 14 extension member'lari yalniz `net10` bacakta. Sonuc: **ortak sozlesme 7.3 uyumlu `interface` + `abstract` kalir**, varsayilan davranis somut sinifta yasar.

## §1 Secim tablosu

| Soru | Cevap | Secim |
|---|---|---|
| Tip bir **sozlesme** mi (davranis, degismez) | Evet | `interface` |
| Cok paylasilan **durum** (alan) var mi | Evet | `abstract` sinif (tek kalitim zinciri) |
| Farkli ailelerden tipler ayni davranisi paylasacak mi | Evet | `interface` (coklu kalitim) |
| Cagiran yalnizca bir imzayi mi biliyor (DIP) | Evet | `interface` + DI |
| Kurucu parametreleri / paylasilan alan zorunlu mu | Evet | `abstract` sinif |
| Test icin taklit (mock) gerekiyor mu | Evet | `interface` (NSubstitute) |

> **KURAL:** `abstract` sinifta tek bir kalitim zinciri oldugu icin onu miras almak bir **taahhuttur**; `interface` imzasi ise genelde geriye donuk uyumludur. `SOL-08` (YAGNI): `interface` yalniz **>= 2 gercek kullanim** ya da **test seam** varsa acilir.

## §2 Iki bacakta

| Ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `interface`, `abstract class`, `virtual`/`override` | Var | Var |
| Explicit interface implementasyonu | Var | Var |
| Default interface metodu (C# 8) | **Yok** — calisma zamani destegi yok | Var |
| `static abstract` arayuz uyesi (C# 11) | **Yok** | Var |
| `default`/`notnull` NRT arayuz konulari | dil 7.3'te annotation yok | Var |
| Arayuz icinde `private` yardimci (C# 8) | **Yok** | Var |
| C# 14 `extension` uye bloklari | **Yok** | Var |
| `record` ile immutable soyut veri (C# 9) | **Yok** (bkt. [records-and-immutability](records-and-immutability.md)) | Var |

> DIM'i ortak koda yazmak en pahali hatadir: derleyici kabul etmeyebilir ya da `net48` calisma zamaninda sessizce farkli davranir.

## §3 Ornek

```csharp
// l0 sozlemesi - iki bacakta da ayni
internal interface ISessionStore
{
    Task<Result<SessionDto>> KaydetAsync(SessionDto dto, CancellationToken ct);
}

// ortak davranis - somut sinifta, arayuzde degil
internal abstract class StoreBase : ISessionStore
{
    protected const int ZamanAsimi = 15;          // paylasilan durum
    public abstract Task<Result<SessionDto>> KaydetAsync(SessionDto dto, CancellationToken ct);
}

internal sealed class SqliteStore : StoreBase
{
    public override Task<Result<SessionDto>> KaydetAsync(SessionDto dto, CancellationToken ct)
        => KaydetCoreAsync(dto, ct);              // ornek govde
}
```

| Yapilandirma | Ne kazandirir | Bedeli |
|---|---|---|
| `interface` (l0 port) | test edilebilir, DIP, mock | davranis her somutta tekrar |
| `abstract` sinif | paylasilan alan + degistirilebilir adim | tek kalitim zinciri |
| Somut bagimlilik | hizli baslangic | katman kiriligi (NetArchTest kirmizi) |

## §4 Sik hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | tek uygulamali `interface` + `Manager` eki | isim gurultusu | anlamlı isim (`ISessionStore`) |
| 2 | arayuzde varsayilan davranis (DIM) | net48 kirigi | somut sinifa tasi |
| 3 | `abstract` sinifi ortak kutu yapmak | kalirim kirliligi | `interface` + composition |
| 4 | somut tipin arayuz olmayan uyesini cagirmak | katman ihlali | yalniz arayuz uyesi |
| 5 | testte sinifi miras alarak mock yapmak | kirilgan test | `interface` + NSubstitute |

## §5 Kurallar

1. Arayuz adi `I` + anlamli isim; uygulama adi pazarlama degil davranistir.
2. `interface` acmadan once `SOL-08` esigini sor: 2 gercek kullanim ya da test seam var mi?
3. Arayuz imzasi degisikligi public/ortak katmandaysa `architect` onayi gerekir.
4. Somut tip baglantisi her katmanda NetArchTest ile denetlenir (`SOL-06`).
5. `net48` ortak dosyasinda DIM, `static abstract` ve extension uye **yazilmaz**.

## §6 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| Katman yonu | NetArchTest: somut tip baglantisi 0 |
| DIM kullanimi | net48 ortak dosyada 0 |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |
| Mock yaklasimi | testlerde somut sinif degil arayuz |

## §7 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| kisitlar | [generics-constraints](generics-constraints.md) |
| katman yonu | [layer-dependency-direction](layer-dependency-direction.md) |
| degismez tip | [records-and-immutability](records-and-immutability.md) |
| SOZLESME | [result-type-design](result-type-design.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | once arayuz kurali | `.rules/solid-rules.md` SOL-04 |
| 2 | DIP + Host kaydi | `.rules/solid-rules.md` SOL-05 |
| 3 | soyutlama esigi (YAGNI) | `.rules/solid-rules.md` SOL-08 |
| 4 | somut bagimlilik denetimi | `.rules/solid-rules.md` SOL-06 (NetArchTest) |
| 5 | net48'de DIM/extension yok | ADR-001 ikili bacak + `.rules/coding-standards.md` CS-12 |
