---
title: Cok Hedefli Proje Ozellikleri (Multi-Targeting Props)
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

# Cok Hedefli Proje Ozellikleri (Multi-Targeting Props)

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001 Cift Hedef]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

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
- Farkli davranislar icin ayri test metodu tercih et; kosullu assert'ten kacin. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[knowledge/csharp/language-versions-net48-net10|Dil Surumleri]] · [[knowledge/csharp/index|C Sharp Index]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]]

## §9 Kanit

- Cift hedef karari [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] ile sabit. Kaynak: kabul edilmis ADR.
- Props ornegi SDK-style proje varsayimiyla yazildi; eski csproj duzeni kullanilmiyor. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]]; DOĞRULAMA GEREKLİ - proje bazli farklilik olabilir.
