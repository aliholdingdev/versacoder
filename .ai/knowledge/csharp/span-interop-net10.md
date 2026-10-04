---
title: Span ve net10.0 Arasinda Entegrasyon
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# Span ve net10.0 Arasinda Entegrasyon

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [ADR-001 Cift Hedef](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) | [Async Kurallari](../../.rules/async-rules.md) | [C Sharp Index](index.md)

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
3. net48'de `System.Memory` paketi eklenmezse `Span<T>` bulunamaz; [Cok Hedefli Props](multi-targeting-props.md) kosuluyla ekle.
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
- Hedef bazli test: her iki TFM'de de ayni test kosmali (Span paketi farkliligi). Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [ADR-001](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) · [Async Kurallari](../../.rules/async-rules.md) · [Cok Hedefli Props](multi-targeting-props.md) · [C Sharp Index](index.md)

## §9 Kanit

- ref struct kisitlari dil spesifikasyonundan; cift hedef uyumu [ADR-001](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) cercevesinde. Kaynak: kabul edilmis ADR.
- `System.Memory` paket ihtiyaci net48 tarafina ait; surum numarasi proje bazinda DOĞRULAMA GEREKLİ.
