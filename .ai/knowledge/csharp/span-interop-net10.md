---
title: Span ve net10.0 Arasinda Entegrasyon
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

# Span ve net10.0 Arasinda Entegrasyon

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001 Cift Hedef]] | [[.rules/async-rules|Async Kurallari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** `Span<T>`/`ReadOnlySpan<T>` stack-uzeri hiz icindir; `ref struct` oldugu icin async, generic yakalama ve boxlama katmanlarindan gecemez.

## §0 Ozet (Answer First)

Span dort sinirla gelir: **async'te kullanilamaz**, **generic type argumani olamaz**, **boxing yapilmaz**, **await uzerinde yasanamaz**. Cift hedefte (`net48;net10.0`) ortak kodda Span kullanacaksan platform paketini (`System.Memory`) ekle; yalnizca net10 tarafinda birakiyorsan `#if` ile sinirla.

## §1 Karar Tablosu

| Ihtiyac | Secim | Neden |
| --- | --- | --- |
| Bellek kopyasiz dilimleme | `Span<T>` | Sifir kopya |
| Metot cagrisinda salt okunur | `ReadOnlySpan<T>` | Guvenli gecis |
| Async akis icinde | `Memory<T>` | ref struct yasak |
| `Task`/generic icinde | Dizi veya `Memory<T>` | Generic arguman yasak |
| Gecici buffer | `stackalloc` + Span | Stack uzeri |

## §2 Iki Bacak

**Bacak 1 - Hiz siniri:** Span, ayni bellek bolgesini uzerinde tasiyan ince bir gorunumdur. Kopyalama yok, tahsis yok; ayrica olcumle - Span her derde deva degil, karar icin sayac gerek.

**Bacak 2 - Yasak siniri:** `ref struct` turu her yerde calismaz. Turu yaymak istiyorsan (async, koleksiyon, generic), Span'i metot sinirinda `T[]` veya `Memory<T>` ile bitir.

## §3 Ornek

```csharp
// IYI: Span metot sinirinda, kopya yok
static int Toplam(ReadOnlySpan<int> veri)
{
    int t = 0;
    foreach (var v in veri) t += v;
    return t;
}

// IYI: cift hedefte ortak kod - Memory async'e uyar
static async Task<int> ToplamAsync(Memory<int> veri)
{
    await Task.Yield();
    return Toplam(veri.Span);
}

// YASAK: ref struct async'de
// static async Task<int> Kotu(Span<int> s)
// {
//     await Task.Delay(1);   // derleme hatasi
//     return Toplam(s);
// }
```

## §4 Hata Modlari

1. Span'i `await` sonrasi kullanmak: stack frame'i kapanmis olabilir; derleyici engellemezse bellek bozulur.
2. Span'i boxing ile `object` yapma denemesi derleme hatasi verir; çözüm turu degistirmek, etrafina sarmak degil.
3. net48'de `System.Memory` paketi eklenmezse `Span<T>` bulunamaz; [[knowledge/csharp/multi-targeting-props|Cok Hedefli Props]] kosuluyla ekle.
4. Span'i closure (lambda) icinde yakalamak derleme hatasina donusur; yerel degiskeni kopyala.

## §5 Kurallar

- Span'i metot parametresi ve yerel alanla sinirla; turu disariya Span verme.
- Async gerekcekse `Memory<T>` ile devam et, Span'i cagri icinde kullan.
- Olcum yapmadan Span'a gecme; kopya zaten hizliysa maliyet anlamsiz.
- Cift hedefte Span'i ortak kodda tutacaksan `System.Memory`'yi kosullu ekle.
- `stackalloc` boyutunu sinirla; buyuk tahsis stack overflow getirir.

## §6 Yasaklar

- `Span<T>` ile async metot imzasi **YASAK**.
- `Span<T>`'yi generic tip argumani veya field olarak kullanmak **YASAK**.
- Panikle `new Span<T>(new T[n])` ile "kopyasiz" gorunum yaratmak **YASAK**; zaten kopya yaptin.
- `stackalloc` sonucunu geri dondurme (Span disari tasinamaz) **YASAK**; kopyala veya kullaniciya ver.

## §7 Test Baglantisi

- Span yolu ile normal yol ayni sonucu vermeli: `Assert.AreEqual(diziToplam, Toplam(dizi))`.
- Kenar durumlari test et: bos span, tek eleman, tam buffer.
- Hedef bazli test: her iki TFM'de de ayni test kosmali (Span paketi farkliligi). Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[.rules/async-rules|Async Kurallari]] · [[knowledge/csharp/multi-targeting-props|Cok Hedefli Props]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- ref struct kisitlari dil spesifikasyonundan; cift hedef uyumu [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] cercevesinde. Kaynak: kabul edilmis ADR.
- `System.Memory` paket ihtiyaci net48 tarafina ait; surum numarasi proje bazinda DOĞRULAMA GEREKLİ.
