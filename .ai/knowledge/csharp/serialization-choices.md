---
title: Serilestirme Secimleri
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

# Serilestirme Secimleri

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005 ADO NET Dapper]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

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

**Bacak 2 - Nerede serilestirme:** Serilestirme sinir katmaninda (API, dosya, mesaj) yapilir; domain tipi JSON ozellikleriyle kirletilmez. [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] cercevesinde veritabani yolunu ORM'e birakma - Dapper satiri dogrudan okur.

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
4. String birlestirerek JSON/SQL uretilir; kacis ve enjeksiyon riski. [[.rules/coding-standards|Yasaklar]].

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
- Eksik/ fazla alanli kirli JSON ile test et; serilestirici nereye patlar. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[knowledge/csharp/multi-targeting-props|Cok Hedefli Props]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- ORM yasagi ve Dapper secimi [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] ile sabit. Kaynak: kabul edilmis ADR.
- System.Text.Json / Newtonsoft karsilastirmasi kaynak vault C# setinden derlendi; surum bazli ayrintilar icin DOĞRULAMA GEREKLİ.
