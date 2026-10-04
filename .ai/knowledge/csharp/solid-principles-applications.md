---
title: SOLID İlkeleri Uygulamada
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# SOLID İlkeleri Uygulamada

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [SOLID Kurallari](../../.rules/solid-rules.md) | [Katman Baglanti Yonu](layer-dependency-direction.md) | [C Sharp Index](index.md)

> **Tek cumlelik sorumluluk:** SOLID, soyutlama icin soyutlama degildir; her ilkenin VersaCoder kodunda karsiligi asagidaki ihlal ve duzeltme tablosudur.

## §0 Ozet (Answer First)

Sekiz kural uc gruptur: **bes ilke** (SRP, OCP, LSP, ISP, DIP) ve **uc uygulama kurali** (denetim otomatik, hata akimi Result, YAGNI). Temel es: her `interface` gercek ikinci kullanim ya da test seam'i tasir. Kural yalniz incelemedeyse iki haftada olur - mimari iddia NetArchTest ile otomatik denetlenir.

## §1 Karar Tablosu

| Ilke | Tek soru | Ihlal belirtisi |
| --- | --- | --- |
| SRP | Tek sinif, tek degisim sebebi mi? | 800 satirlik god form |
| OCP | Yeni davranis acik uzantı mi? | Buyuyen switch zinciri |
| LSP | Gecerli girdide `NotSupportedException` var mi? | Eksik implementasyon |
| ISP | Arayuzde bos metot var mi? | 15 metotlu tek arayuz |
| DIP | Serviste `new SqliteConnection` var mi? | Somut bagimlilik |

## §2 Iki Bacak

**Bacak 1 - Ilke + ornek:** Her ilke, kodda kisa bir BAD/GOOD ile gorulur. Ilkeyi metinden degil orneginden tanirsan ihlali fark edersin.

**Bacak 2 - Otomatik denetim:** Katman baglilik kurallari derleme analiziyle (NetArchTest benzeri) test edilir. "Mimari dogru" iddiasi bu testler yesil olmadan gecersizdir.

## §3 Ornek

```csharp
// BAD - her yeni saglayici mevcut kodu degistirir (OCP ihlali)
public string Complete(string provider, string prompt)
    => provider switch
    {
        "openai" => CallOpenAi(prompt),
        "ollama" => CallOllama(prompt),
        _ => throw new NotSupportedException(provider),
    };

// GOOD - arayuz + Host kaydi; yeni saglayici = yeni sinif + tek DI satiri
public interface IModelProvider
{
    string Name { get; }
    Task<string> CompleteAsync(PromptRequest request, CancellationToken ct);
}
```

## §4 Hata Modlari

1. Tek implementasyonlu `IFoo` + `Foo` + `FooFactory` zinciri: dolgu katman (YAGNI ihlali).
2. Arayuzde `NotSupportedException` firlatan implementasyon: cagiran kod implementasyonu bilmeden degistirilemez.
3. Formda SQL + log + is kurali bir arada: her degisiklik uc riski tetikler.
4. Mimari test yokken "SOLID" onayi: kanitsiz onay, gate RED.

## §5 Kurallar

- Yeni sinifin tek sorumlulugu tek cumleyle yazilabilmeli (SRP).
- Davranis ekleme yolu: mevcut switch'i buyutmek degil, arayuz + DI kaydi.
- Interfacelari yetenege gore bol; istemci yalniz ihtiyacini gorur (ISP).
- Somutu `Host` baglar; serviste `new` ile altyapı yok (DIP).
- Soyutlamayi ikinci gercek kullanim gelince yap; once sinifla basla (YAGNI).

## §6 Yasaklar

- "Daha sonra lazim olur" diye ilke uygulamak **YASAK**.
- Ihlali yorumla savunmak **YASAK**.
- Test edilmemis mimari kural iddiasi **YASAK** (SOL-06).
- Normal akisi exception ile tasmak **YASAK**; hata `Result<T>` (SOL-07).

## §7 Test Baglantisi

- Mimari testler: Domain hicbir referans almaz, Application Infrastructure'a bakmaz - bu iddialar test dosyasinda yasar.
- Handler/servis akislari unit testte substitute ile; gercek kaynak integration testinde. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [SOLID Kurallari](../../.rules/solid-rules.md) · [Katman Baglanti Yonu](layer-dependency-direction.md) · [Kodlama Standartlari](../../.rules/coding-standards.md) · [C Sharp Index](index.md)

## §9 Kanit

- Sekiz kural (SOL-01..SOL-08) ve ornekleri kaynak dosyadan derlendi. Kaynak: [SOLID Kurallari](../../.rules/solid-rules.md), 2026-09-25.
- Otomatik denetim iddiasi ayni dosyanin denetim tablosuna bagli; calisirlik durumu DOĞRULAMA GEREKLİ.
