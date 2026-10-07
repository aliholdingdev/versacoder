---
title: Cok Hedefli Proje Ozellikleri (Multi-Targeting Props)
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: SSOT
scope: csharp
---

# Cok Hedefli Proje Ozellikleri (Multi-Targeting Props)

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [ADR-001 Cift Hedef](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (csharp · 2/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** Cift hedef `net48;net10.0` derlemesinde farklilasan ozellikleri props iceriginde kosullu tanimla, kaynak kodu `#if` cennetine cevirme.

## §0 Ozet (Answer First)

Props uc bolgede yonetilir: **TargetFrameworks**, **kosullu PackageReference** ve **kosullu DefineConstants**. Farklilasan davranisi memnuniyetle `#if` icine al; yalnizca kucuk ve istikrarli farklarda. Buyuk farkliliklari ayri dosya veya ayri proje olarak dusun.

## §1 Karar Tablosu

| Bolge | net48 | net10.0 | Nerde tanimlanir |
| --- | --- | --- | --- |
| TFM | `net48` | `net10.0` | TargetFrameworks |
| Dil surumu | 7.3 | en yeni | LangVersion (kosullu) |
| Nullable | kisitli | acik | Nullable (kosullu) |
| Analiz paketi | eski surum | yeni surum | PackageReference kosullu |
| Ozellik sembolleri | `NET48` | `NET10_0` | otomatik gelir |

## §2 Iki Bacak

**Bacak 1 - Tanim tek yerde:** TFM listesi tek satirda (`net48;net10.0`). Yeni hedef eklerken tek satir duzeltme yap. Ayni TFM birden fazla yerde gecmesin.

**Bacak 2 - Fark kosullu olmali:** Sadece bir hedefte calisan paket, sembol veya ozellik `Condition` ile ayristilir. Kosul ifadesi hedef adina gore yazilir; yanlis kosul sessizce calismaz.

## §3 Ornek

```xml
<PropertyGroup>
  <TargetFrameworks>net48;net10.0</TargetFrameworks>
  <LangVersion>latest</LangVersion>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)' == 'net48'">
  <Nullable>annotations</Nullable>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)' == 'net10.0'">
  <PackageReference Include="System.Threading.Tasks.Extensions" Version="4.6.0" />
</ItemGroup>
```

```csharp
#if NET48
    // net48'e ozel kucuk uyum
#else
    // net10.0 yolu
#endif
```

## §4 Hata Modlari

1. Yanlis kosul: paket hic eklenmez, derleme sonradan patlar; kosul adini kontrol et.
2. `LangVersion` sabitlenmezse SDK surumune gore dil degisir; ekip arasinda tutarsizlik cikar.
3. `#if` yayildikca okunurluk duser; kural asimi kod incelemesinde yakalanir.
4. Tek hedefe eklenen referans diger hedefte eksik kalir; ayni arayuz iki yerde tanimlanir.

## §5 Kurallar

- TFM listesini tek PropertyGroup icinde tut.
- Kosullari hedef adina gore yaz, guvenli bir sabitle destekle.
- `LangVersion` ve `Nullable` degerlerini proje genelinde acikca yaz.
- `#if` bloklarini kucuk ve fonksiyonel tut; her blok icin yorum ekle.
- Yeni paketi once diger hedefte de gerekli mi diye sor.

## §6 Yasaklar

- Ayni tanimin iki hedefe ayni degerle kopyalanmasi **YASAK**; ortak grup kullan.
- Denenmemis kosul **YASAK**; her kosulu iki hedefte de derleyerek dogrula.
- `#if` icine tam metod govdesi yerlestirmekten kacin; mumkunse tek ifade.
- TFM listesini projeler arasi kopyalayip cekistirmek **YASAK**; paylasilan props'a tasi.

## §7 Test Baglantisi

- Her iki hedefte de ayri derleme ve test kosusu zorunlu; tek hedefte gecen test kafi degil.
- `#if` dallari icin karsilikli test yaz: biri digerinin yoklugunu dogrular.
- Farkli davranislar icin ayri test metodu tercih et; kosullu assert'ten kacin. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Kodlama Standartlari](../../.rules/coding-standards.md) · [Dil Surumleri](language-versions-net48-net10.md) · [C Sharp Index](index.md) · [ADR-001](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md)

## §9 Kanit

- Cift hedef karari [ADR-001](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) ile sabit. Kaynak: kabul edilmis ADR.
- Props ornegi SDK-style proje varsayimiyla yazildi; eski csproj duzeni kullanilmiyor. Kaynak: [Kodlama Standartlari](../../.rules/coding-standards.md); DOĞRULAMA GEREKLİ - proje bazli farklilik olabilir.
