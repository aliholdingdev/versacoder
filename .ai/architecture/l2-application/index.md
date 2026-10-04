---
title: "Katman Spec - l2 Application (VersaCoder)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# Katman Spec - l2 Application

**Zorunlu Baglantilar:** [index.md](../index.md) | [dependency-map](../00-overview/dependency-map.md) | [N12-commands.md](N12-commands.md) | [N13-queries.md](N13-queries.md) | [N14-handlers.md](N14-handlers.md) | [N15-dto-mappers.md](N15-dto-mappers.md) | [N16-validators.md](N16-validators.md) | [N17-use-cases.md](N17-use-cases.md) | [N18-scheduling.md](N18-scheduling.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Kullanici bir isi istediginde hangi adimlarin, hangi sira ile ve hangi hata diliyle yurutuldugunun tek cevabidir; burada **akis** yasar, kural l1'de, erisim l4'tedir.

## §0 Ozet (Answer First)

l2 = **akis katmani**: dogrula -> l1 kuralini cagir -> l0 portunu kullan -> **tek commit** -> **DTO** dondur. SQL metni, Dapper, `DbConnection`, protokol semasi, DevExpress **gormez**; gorse RED'dir. Entity asla disari cikmaz, ciksa `l7 -> l1` ihlali dogar. Disariya acilan yuzey `IDispatcher` + komut/sorgu + DTO'dur.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2 Application (insa sirasi 4) |
| Proje | VersaCoder.Application (ADR-028, 9 projeden biri) |
| N kodlari | N12..N18 + N47 (makro) |
| Izinli referans | l0 (port, Result, handler sozlesmesi) + l1 (entity, invariant, event) |
| Yasak referans | l4, l5, l6, l7 · ADO.NET/Dapper · protokol semasi · DevExpress |

## §1 Sorumluluk

| # | Bu katman NE YAPAR | Kanit |
|---|---|---|
| A1 | Use case'leri handler olarak uygular | l0 sozlesmesine uyar |
| A2 | Girdiyi **alan bazinda** dogrular (`Invalid` + `Fields`) | hata listesi DTO'ya doner |
| A3 | Entity'yi port uzerinden okur/yazar, invariant'i **l1'e birakir** | tekrarli kural yok |
| A4 | Transaction sinirini kurar: **tek** `IUnitOfWork.CommitAsync` | kismi yazim kalmaz |
| A5 | DTO uretir; entity'yi disari **cikarmaz** | imza `Result<ProjectDto>` |

| # | Bu katman NE YAPMAZ | Neden |
|---|---|---|
| B1 | SQL metni yazmaz, `@pN` kurmaz | erisim l4'un port uygulamasi |
| B2 | `DbConnection`/Dapper/`IDataReader` tipine bakmaz | l2 -> l4 **yasak** |
| B3 | Protokol mesaji uretmez/parcalamaz | sema l5'tedir |
| B4 | Invariant'i **yeniden** yazmaz | iki gercek kaynak dogar |
| B5 | Kendi zamani/`DateTime.Now`'u kullanmaz | saat kaynagi disarida |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut/sorgu (ham veri, `sealed record`) | l7 ekran, l6 kayit |
| GIRENLER | Kural (entity olusturma, gecis) | l1 aggregate/invariant |
| GIRENLER | Port cagrisi sonucu (kayit durumu) | l0 sozlesmesi (uygulama l4'te) |
| CIKANLAR | `Result<TDto>` (DTO disinda sey yok) | l7/l5 tuketir |
| CIKANLAR | Domain event (commit sonrasi) | l6 aboneligi uzerinden |
| CIKANLAR | Katman logu (`ILogSink`) | l3 uzerinden |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l2 -> l4-infrastructure | X | port l0'da, uygulamasi l4'te (ADR-028) |
| l2 -> l5-protocol | X | transcript/agent dongusu l5'te |
| l2 -> l6-host | X | DI kaydi l6'da, l2 bakmaz |
| l2 -> l7-ui | X | ekran tipi gorulmez |
| l2 -> **l3-crosscutting** | **X** (matris) | kaynak vault `l2 -> l3 ✓` yazar; hedef matriste X — **DOGRULAMA GEREKLİ** |

Izinli: l2 -> l1 (OK), l2 -> l0 (gecis/transitive, `.`). Matris notlari [index.md](../index.md) §4.1 ve [dependency-map](../00-overview/dependency-map.md).

## §4 N Kodlari ve Proje

| N | Ad | Cikti | Tuketici |
|---|---|---|---|
| N12 | commands | 5 komut | l7 uretir, l6 kaydeder |
| N13 | queries | 5 sorgu | l7 |
| N14 | handlers | `ICommandHandler<>`/`IQueryHandler<>` + Dispatcher | l6 kayit, l7 cagri |
| N15 | dto-mappers | `ProjectDto`, `TaskDto`, `SettingDto`, `FileContentDto`, `PagedResult<T>` | l7, l5 |
| N16 | validators | alan dogrulama + `Fields` | l7 gosterir |
| N17 | use-cases | U1..U10 | l7 |
| N18 | scheduling | calisma zamani kalibi | l6 is parcacigi |
| N47 | macro-l2 | makro ozet | bu klasor |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: port, `Result`, handler, `IDispatcher` | asagi | tek omurga |
| l1: entity, invariant, `TaskStateMachine`, event | asagi | kural cagrisi |
| l3: `ILogSink`, `IClock`, `IRetryPolicy` | X (matris) | **DOGRULAMA GEREKLİ**: kaynak ✓, hedef X |
| l4/l5/l6/l7 | YOK | hepsi CRITICAL |
| Harici NuGet (MediatR, FluentValidation) | YOK | dispatcher + dogrulama bu projede (G8) |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Her handler: mutlu yol + her `ErrorCode` yolu | 10 use case x >=2 yol |
| Unit | Dogrulama matrisi sinir degerleri | 5/5 alan |
| Unit | Orkestrasyon sirasi (U6 9 adim) | `Received.InOrder` |
| Unit | Tek `CommitAsync` | `Received(1)` |
| Arch | l2 -> l4/l5/l6/l7 + `EntityFramework*` bagimliligi | **0** |

## §7 Boundary Check Sorulari

1. Bu handler'da SQL/Dapper/`@pN` var mi? (varsa RED, kod l4'e ait)
2. Donus tipi her yerde `Result<DTO>` mu, entity siziyor mu?
3. Iki `CommitAsync` var mi? (kismi yazim riski)
4. Invariant l2'de tekrar yazildi mi? (iki gercek kaynak)
5. `l2 -> l5` (transcript) kenari var mi? (NetArchTest = 0)
6. `#if` iki bacak kodu var mi? (G6, hedef `#if` = 0)

Cevaplama ve 13 soru: [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §8 Kurallar (5 madde)

- Her handler **tek** `Result<T>` dondurur; exception **yukari siktirilmaz** (`Result.Fail` + log).
- Commit **bir** yerde yapilir; iki commit = RED.
- Entity DTO'dan disari cikmaz; `ToDto()` **tek** yerde yasar.
- Invariant tekrarlanmaz, kural l1'dedir; l2 yalniz girdi bicimine bakar.
- SQL/protokol/DevExpress gorulurse aninda RED; dogru refaktor portu l0'a acmak.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT (matris) + kaynak vault detayi |
