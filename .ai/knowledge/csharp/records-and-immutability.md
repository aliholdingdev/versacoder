---
title: Record ve Degismezlik
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

# Record ve Degismezlik

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../claude.md) | [SOLID Kurallari](../../.rules/solid-rules.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 2/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [pattern-matching](pattern-matching.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** Record, deger semantigini (esitlik + kopya + degistirme) tek anahtarla verir; class'i yalnizca kimlik ve davranis gereginden sec.

## §0 Ozet (Answer First)

**Record = deger, class = kimlik.** Record'a `with` ile degistirirsin, esitlik tum alanlara gore calisir, `init` alanlar disindan disaridan degistirilemez. Tehlike: record icindeki referans alan (List, Mutable obje) hala degisebilir - degismezlik ust seviyede kalir, derin degismezlik elle kurulur.

## §1 Karar Tablosu

| Soru | Cevap | Secim |
| --- | --- | --- |
| Ayni deger mi, ayni sey mi? | Ayni deger | record |
| Kimlik mi (id) onemli? | Evet | class |
| Disaridan degistirilebilsin mi? | Hayir | record + init |
| Kopya + kucuk degisiklik? | Evet | `with` |
| Referans alan derin korunsun mu? | Evet | elle kopyala |

## §2 Iki Bacak

**Bacak 1 - Deger semantigi:** Record'ta esitlik tum alanlara gore, referans esitligi degil. Bu, koleksiyonda arama, testte karsilastirma ve delta hesabini basitlestirir.

**Bacak 2 - Degistirme disindan koruma:** Init-only alanlar constructor disinda kapanir. Ancak icindeki `List<T>` ayni kalir; paylasilan koleksiyon kopyasi disaridan kirilabilir. Paylasim riskini bilerek sec.

## §3 Ornek

```csharp
public sealed record SiparisSatiri(string Urun, int Adet, decimal Fiyat)
{
    public decimal Tutar => Adet * Fiyat;
}

var s1 = new SiparisSatiri("kalem", 3, 10m);
var s2 = s1 with { Adet = 5 };

Console.WriteLine(s1.Adet);            // 3 - degismedi
Console.WriteLine(s1 == new SiparisSatiri("kalem", 3, 10m));  // True

// KOTU: disaridan degistirilebilir alan record'u kirar
public sealed record Bozuk(string Ad)
{
    public List<string> Notlar { get; } = new();  // paylasilabilir
}
```

## §4 Hata Modlari

1. Record icindeki mutable alan paylasilir; biri ekler, digeri farkina varmaz.
2. Class ile record karistirilinca kimlik esitligi beklenir ama deger esitligi gelir; test yanlis gecer.
3. Record'a miras hiyerarsisi kurulur; record zaten yalnizca `sealed` olmayan tek seferlik miras icin tasarli.
4. `with` ile kopyalanirken referans alanlar kopyalanmaz; derin kopya unutulur.

## §5 Kurallar

- Deger tasiyan veri (DTO, satir, olcum) icin record; davranis ve kimlik icin class.
- Paylasilan referans alanlari record icine almadan once kopyalama maliyetini hesapla.
- `sealed record` ile miras kapat; kapali hiyerarsi esitlik davranisini sabitler.
- Testte karsilastirmayi record esitligine birak; elle alan karsilastirma yazma.
- Degistirme yerine `with` kullan; nesne uzerinde yan etki yapma.

## §6 Yasaklar

- Record uzerinde set ile disaridan degistirme **YASAK**; `with` kullan.
- Kimlik esitligi gereken varlik (Aggregate, Entity) icin record **YASAK**; [SOLID Kurallari](../../.rules/solid-rules.md) cercevesinde sorumluluk yanlis dagilir.
- Paylasilan koleksiyonu "deger" diye tanimlamak **YASAK**; kopyala veya class sec.
- Record'a anlamli davranis (yan etki) yuklemek **YASAK**; davranis ayri servis.

## §7 Test Baglantisi

- `with` sonrasi orijinalin degismedigini dogrula: `Assert.AreEqual(3, s1.Adet)`.
- Esitlik testi iki ayri kurulum, ayni alanlarla yapilir (kimlik degil).
- Paylasilan alan testinde iki ornegi ayri ayri kur; ayni liste referansini paylasma. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [SOLID Kurallari](../../.rules/solid-rules.md) · [Kodlama Standartlari](../../.rules/coding-standards.md) · [Nullable Reference Types](nullable-reference-types.md) · [C Sharp Index](index.md)

## §9 Kanit

- Record/class ayrimi deger-kimlik temelinde yapildi; kaynak vault C# seti, 2026-09-25.
- Degismezlik kisiti tek seviyeli (ust alan) olarak yazildi; derin kopya ihtiyaci proje bazli DOĞRULAMA GEREKLİ.
