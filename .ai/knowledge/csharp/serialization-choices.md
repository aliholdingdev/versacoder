---
title: Serilestirme Secimleri
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: SSOT
scope: csharp
---

# Serilestirme Secimleri

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [ADR-005 ADO NET Dapper](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (csharp · 2/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** JSON'da iki motor var (System.Text.Json hizli, Newtonsoft esnek); veritabani serilestirmesi baska konudur - oran ORM'e degil ADO NET + Dapper.

## §0 Ozet (Answer First)

| Ihtiyac | Secim |
| --- | --- |
| Varsayilan JSON | System.Text.Json |
| Eski format / ozel converter | Newtonsoft.Json |
| Veritabani okuma | ADO NET + Dapper (ADR-005) |
| Formatted string | Kullanma |
| Binary protokol | Ayriltilmis karar |

Ilk kural: serilestirmeyi dogru yerde yap (sinirda), model icine gizleme.

## §1 Karar Tablosu

| Ozellik | System.Text.Json | Newtonsoft.Json |
| --- | --- | --- |
| Hiz | Yuksek | Orta |
| Converter yazma | Kolay | Genis ekosistem |
| Döngüsel referans | Kisitli | Destekli |
| Tarih bicimi | ISO varsayilan | Ozellestirilebilir |
| Cift hedef | Iki TFM de destekler | Destekler |

## §2 Iki Bacak

**Bacak 1 - Motor secimi:** Varsayilan System.Text.Json; eski musteri veya ozel converter gerekiyorsa Newtonsoft. Motoru bir kez sec, her yerde ayni ayarla kullan (kayit adlari, tarih bicimi ayni olmali).

**Bacak 2 - Nerede serilestirme:** Serilestirme sinir katmaninda (API, dosya, mesaj) yapilir; domain tipi JSON ozellikleriyle kirletilmez. [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) cercevesinde veritabani yolunu ORM'e birakma - Dapper satiri dogrudan okur.

## §3 Ornek

```csharp
// IYI: tek yerde, tek ayarla
public static readonly JsonSerializerOptions JsonAyarlari = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false,
};

var json = JsonSerializer.Serialize(kayit, JsonAyarlari);
var geri = JsonSerializer.Deserialize<Kayit>(json, JsonAyarlari);

// KOTU: her cagirida farkli ayar, farkli cikti
var k2 = JsonSerializer.Serialize(kayit);
```

```csharp
// Veritabani: ORM yok, satiri dogru oku (ADR-005)
var satir = await conn.QuerySingleOrDefaultAsync<Musteri>(
    "SELECT Id, Ad FROM dbo.Musteri WHERE Id = @Id", new { Id = id });
```

## §4 Hata Modlari

1. Farkli yerlerde farkli serializer ayari: ayni model iki sekilde doner; istemci kirilir.
2. Tarih bicimi yerel/kultur bagimli yazilirsa okuyan yer yanlis yorumlar; ISO kullan.
3. Serilestirme domain icine girerse model JSON attributelariyla dolar; test ve okunurluk duser.
4. String birlestirerek JSON/SQL uretilir; kacis ve enjeksiyon riski. [Yasaklar](../../.rules/coding-standards.md).

## §5 Kurallar

- Serializer ayarini tek kaynaktan tanimla, her yerde gec.
- Tarih ve sayi bicimini acikca yaz; kulture birakma.
- Serilestirmeyi sinirda yap; domain tipi kaldi ki temiz kalsin.
- Veritabani donuslerini Dapper ile satir bazinda dogrula.
- Tanim disindaki tipleri explicit converter ile ac; sihirli beklenti kurma.

## §6 Yasaklar

- Formatli string ile JSON/rapor uretmek **YASAK**; formatter veya serilestirici kullan.
- Serilestirilmis string'i karsilastirma anahtari yapmak **YASAK**; alanlara gore karsilastir.
- Iki serializer motorunu ayni akista karmak **YASAK**.
- Serilestirme hatasini yutup bos nesne dondurmek **YASAK**; hatayi ust katmana tasi.

## §7 Test Baglantisi

- Round-trip testi: serialize -> deserialize -> ayni alanlar; `Assert.AreEqual` ile.
- Ayar degisikligini test et: camelCase ciktisini kilitlestir.
- Eksik/ fazla alanli kirli JSON ile test et; serilestirici nereye patlar. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Kodlama Standartlari](../../.rules/coding-standards.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [Cok Hedefli Props](multi-targeting-props.md) · [C Sharp Index](index.md)

## §9 Kanit

- ORM yasagi ve Dapper secimi [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) ile sabit. Kaynak: kabul edilmis ADR.
- System.Text.Json / Newtonsoft karsilastirmasi kaynak vault C# setinden derlendi; surum bazli ayrintilar icin DOĞRULAMA GEREKLİ.
