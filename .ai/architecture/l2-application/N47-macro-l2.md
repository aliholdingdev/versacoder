---
title: "N47 - Macro View: L2 Application (9 Nodes)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N47 - Macro View: L2 Application (9 Nodes)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N13-queries.md](N13-queries.md) | [N14-handlers.md](N14-handlers.md) | [N15-dto-mappers.md](N15-dto-mappers.md) | [N16-validators.md](N16-validators.md) | [N17-use-cases.md](N17-use-cases.md) | [N18-scheduling.md](N18-scheduling.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** l2 katmanini tek ekranda ozetler: neye yarar, nereden nereye bakabilir ve disariya hangi imzalari acar.

## §0 Ozet (Answer First)

l2 = **akis katmani**: dogrula -> l1 kuralini cagir -> l0 portunu kullan -> **tek commit** -> **DTO** dondur. Bagimlilik **yalniz l0 + l1** (matriste l3 hucresi X — `DOGRULAMA GEREKLİ`); SQL, protokol semasi ve ekran **gormez**. Disari acilan yuzey `IDispatcher` uzerinden **komut/sorgu + DTO**'dur; l6 (kayit), l7 (gorunum) ve l5 (kalıcilik cagrisi) bunlari tuketir.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application (insa sirasi 4) |
| Proje | VersaCoder.Application |
| Node | N12..N18 + N47 (makro) + index |
| Disari acik yuzey | `IDispatcher` + 5 komut + 5 sorgu + 5 DTO |
| Varsayilan yon | l0, l1 (asagi) |

## §1 Sorumluluk

| Alt dugum | Disariya acilan yuzey | Tuketen |
|---|---|---|
| [N12](N12-commands.md) | 5 komut | l7 uretir, l6 kaydeder |
| [N13](N13-queries.md) | 5 sorgu | l7 |
| [N14](N14-handlers.md) | `ICommandHandler<>`/`IQueryHandler<>` + Dispatcher | l6 kayit, l7 cagri |
| [N15](N15-dto-mappers.md) | `ProjectDto`, `TaskDto`, `SettingDto`, `FileContentDto`, `PagedResult<T>` | l7, l5 |
| [N16](N16-validators.md) | alan dogrulama + `Fields` | l7 gosterir |
| [N17](N17-use-cases.md) | U1..U10 | l7 |
| [N18](N18-scheduling.md) | calisma zamani kalibi | l6 is parcacigi |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut/sorgu | l7 ekran, l6 kayit |
| GIRENLER | Entity + invariant | l1 |
| GIRENLER | Port sonucu | l0 (uygulama l4) |
| CIKANLAR | `Result<TDto>` | l7, l5 |
| CIKANLAR | Domain event (commit sonrasi) | l6 aboneligi |
| CIKANLAR | `Fields` hata sozlugu | l7 |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l2 -> l4-infrastructure | X | port l0'da, uygulamasi l4'te |
| l2 -> l5-protocol | X | transcript/agent dongusu l5 |
| l2 -> l6-host | X | DI kaydi l6 |
| l2 -> l7-ui | X | ekran tipi gorulmez |
| l2 -> l3-crosscutting | X (matris) | kaynak vault `✓` yazar — **DOGRULAMA GEREKLİ** |

Izinli: l2 -> l1 (OK), l2 -> l0 (gecis, `.`). Matris ve notlar: [index.md](../index.md) §3-4, [dependency-map](../00-overview/dependency-map.md).

## §4 N Kodlari ve Proje

| N | Ad | Sorumluluk |
|---|---|---|
| N12 | commands | Niyet paketi |
| N13 | queries | Guvenli okuma |
| N14 | handlers | Adim sirasi + tek commit |
| N15 | dto-mappers | Entity -> DTO siniri |
| N16 | validators | Alan dogrulama |
| N17 | use-cases | U1..U10 envanteri |
| N18 | scheduling | Calisma zamani |
| N47 | macro-l2 | Bu dosya |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: port, `Result`, handler, `IDispatcher` | asagi | tek omurga |
| l1: entity, invariant, makine, event | asagi | kural |
| l3 | X (matris) | **DOGRULAMA GEREKLİ** |
| l4/l5/l6/l7 | YOK | hepsi CRITICAL |
| MediatR/FluentValidation/EF | YOK | G8, G2 |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | 10 use case x >=2 yol | tam dallanma |
| Unit | Orkestrasyon sirasi (U6 9 adim) | `Received.InOrder` |
| Unit | Dogrulama matrisi 5/5 | sinir +/- 1 |
| Unit | Tek commit + DTO donus tipi | `Received(1)` |
| Arch | l2 -> l4/l5/l6/l7 + EF = **0** | makro kabul kriteri |

## §7 Kurallar (5 madde)

- l2 yalniz l0 + l1'e bakar; SQL/protokol/DevExpress gorulse RED.
- Her handler tek `Result<T>`, tek commit; exception yukari cikmaz.
- Entity disari cikmaz; `ToDto()` tek yerde.
- Invariant tekrarlanmaz; kural l1'de.
- Envanter (U1..U10, 5 komut, 5 sorgu, 5 DTO) disinda sey uydurulmaz (G9).

## §8 Boundary Check Sorulari

1. Handler'da SQL/`@pN`/Dapper var mi? (CRITICAL)
2. Entity DTO'dan disari cikti mi? (`l7 -> l1` ihlali)
3. Iki `CommitAsync` var mi? (kismi yazim)
4. Invariant l2'de tekrar yazildi mi? (iki gercek)
5. `l2 -> l5` kenari (transcript) NetArchTest = 0 mu?

Kanit: [layer-boundary-check](../../checklists/layer-boundary-check.md) 13 soru, [test-strategy](../../spec/test-strategy.md).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
