---
title: Katman Bagimlilik Yonu
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

# Katman Bagimlilik Yonu

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028 Katman Temeli]] | [[.rules/solid-rules|SOLID Kurallari]] | [[knowledge/csharp/solid-principles-applications|SOLID Uygulamalari]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 2/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Ok yonu disaridan ice dogrudur; Domain hicbir katmani referans almaz, Application Infrastructure'a bakmaz.

## §0 Ozet (Answer First)

Yon tek: **UI -> Host -> Application -> Domain**. Infrastructure, Domain'e baglidir (arayuzu orada degil); baglama (DI) Host'ta yapilir. Cizgi asimini test degil inceleme yakalarsa kural olur; otomatik denetim (NetArchTest benzeri) olmazsa iki haftada kural ölür.

## §1 Yon Tablosu

| Katman | Ne referans alabilir | Ne alamaz |
| --- | --- | --- |
| Domain | hicbir VersaCoder.* katmani | Infrastructure, UI, Host |
| Application | Domain + Abstractions | Infrastructure, DevExpress |
| Infrastructure | Domain (arayuz/sozlesme) | Protocol, UI |
| Host | hepsi (baglama noktasi) | - |
| UI | Host, Application (arayuz) | Infrastructure dogrudan |

## §2 Iki Bacak

**Bacak 1 - Arayuz nerede, uygulamasi nerede:** Arayuz (port) Application/Abstractions'ta; somut uygulama Infrastructure'da; baglama Host'ta tek satir. Boylece Application, somutu bilmeden calisir ve test edilir.

**Bacak 2 - Denetim otomatiktir:** Yukaridaki cizgi, derleme analiziyle test edilen bir iddiadir. El incelemesi tek basina yeterli degildir; kural testte yasamalidir.

## §3 Ornek

```text
 +-----------+   +---------+   +--------------+   +---------------+
 |    UI     |-->|  Host   |-->| Application  |-->|    Domain     |
 +-----+-----+   +----+----+   +------+------+   +-------+-------+
       |              |               |                  |
       | DevExpress    | DI baglama   | I... arayuzleri   | sabitler
       | (yalniz UI)   v              v                  v
       |         +-----+------------------------------------------+
       |         | Infrastructure (Dapper, provider) -> Domain    |
       +-------->+  Paylasimli crosscutting (log, zaman, sonuc)   |
                 +------------------------------------------------+
```

```csharp
// Host'ta tek baglayici satir - somutu burada bilirsin, baska yerde degil
services.AddSingleton<ISessionRepository, DapperSessionRepository>();
```

## §4 Hata Modlari

1. Application -> Infrastructure referansi: test edilemezlik + provider degisimi kirilir.
2. Domain'de log veya dosya cagrisi: en alt katman en ustteki tasiyamaz.
3. UI'dan dogrudan SQL: katman atlama, her degisiklik risk.
4. `#if` ile bolumlenen is mantigi: iki kod tabani, yarisi testsiz.

## §5 Kurallar

- Oku disaridan ice oku; ters ok gorunce once sorma, durma.
- Arayuzu kullanilan tarafa yaz; uygulamasi ayni yerde olmasin.
- Baglamayi tek yerde topla (Host); servis yasam dongusunu orada belirle.
- Crosscutting (log, zaman, sonuc) ortak katmanda, hepsi kullanir.
- Yeni csproj kapisi acarken sorumluluk / yon / TFM ucunu birlikte yaz.

## §6 Yasaklar

- Domain'den herhangi bir katman referansi **YASAK**.
- Application'dan Infrastructure veya DevExpress referansi **YASAK**.
- Test edilmis olmayan "yon dogru" iddiasi **YASAK**.
- Infrastructure'dan UI/Protocol'e geri ok **YASAK**.

## §7 Test Baglantisi

- Mimari test: Domain -> VersaCoder.* referansi = 0; Application -> Infrastructure = 0.
- Test dosyasi derleme analiziyle calisir; yeşil degilse kod yazilmis sayilmaz.
- Katman test matrisi zorunlu; eksik katman coverage boslugudur. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[.rules/solid-rules|SOLID Kurallari]] · [[knowledge/csharp/solid-principles-applications|SOLID Uygulamalari]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Yon diyagrami ve kural metni SOLID kural dosyasindaki katman bolumunden derlendi. Kaynak: [[.rules/solid-rules|SOLID Kurallari]].
- 9 proje / katman temeli [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] ile sabit; proje sayisi/guncel matris DOĞRULAMA GEREKLİ.
