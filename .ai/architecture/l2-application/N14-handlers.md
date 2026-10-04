---
title: "N14 - Handlers (Adim Sirası, Tek Commit, Hata Yayımı)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N14 - Handlers (Adim Sirası, Tek Commit, Hata Yayımı)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N13-queries.md](N13-queries.md) | [N15-dto-mappers.md](N15-dto-mappers.md) | [N16-validators.md](N16-validators.md) | [N17-use-cases.md](N17-use-cases.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir handler'in kac adim yururdugunu, nerede dallandigini, nerede commit ettigini ve hatayi nasil yayedigini belirler.

## §0 Ozet (Answer First)

Her handler **tek** `Result<T>` dondurur, **tek** `CommitAsync` cagirir ve exception'i **yukari sikturmaz** (yakalayip `Result.Fail` + log yapar). Adim sirasi sabittir: **dogrula -> port'tan yukle -> on kosul -> l1 kural/gecis -> ekle/guncelle -> tek commit -> event yayimi -> DTO**. SQL/Dapper/DevExpress gormez; gorse RED'dir. Dispatcher uygulamasi bu projededir, kaydi l6'dadir.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N14, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Sozlesme | `ICommandHandler<>` / `IQueryHandler<>` (l0) |
| Varsayilan yon | l0 + l1 (asagi) |

## §1 Sorumluluk

Tek sorumluluk: **akis govdesi** — sira, dallanma, tek commit.

| Parca | Imza / kural | Not |
|---|---|---|
| `ICommandHandler<TCmd,TRes>` | `Task<Result<TRes>> HandleAsync(TCmd, CancellationToken)` | l0 sozlesmesi |
| `IQueryHandler<TQry,TRes>` | ayni kalip | l0 sozlesmesi |
| `Dispatcher` (uygulama) | `SendAsync`/`QueryAsync` -> `Dictionary<Type, object>` | kayit **l6**'da |
| Dogrulama | `Validate(cmd)` -> `Fields` | [N16](N16-validators.md) |
| Commit | **tek** `IUnitOfWork.CommitAsync` | iki commit = RED |

```csharp
var errors = Validate(cmd);
if (errors.Count > 0) return Result<ProjectDto>.Fail(Error.Invalid(errors));
var project = Project.Create(cmd.Name, cmd.RootPath, _clock.UtcNow);  // l1 invariant
var added = await _projects.AddAsync(project.Value, ct);
var committed = await _uow.CommitAsync(ct);        // TEK commit
return Result<ProjectDto>.Ok(project.Value.ToDto());  // entity disari cikmaz
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut/sorgu + `CancellationToken` | dispatcher |
| GIRENLER | Port sonucu (entity veya yok) | l0 (uygulama l4) |
| GIRENLER | l1 kural sonucu (gecis/invariant) | l1 aggregate |
| CIKANLAR | `Result<TDto>` | l7/l5 |
| CIKANLAR | Domain event (commit sonrasi) | l6 aboneligi |
| CIKANLAR | Log kaydi | l3 uzerinden |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| Handler -> l4-infrastructure | X | port l0'da; SQL gorurse RED |
| Handler -> l5-protocol | X | agent dongusu l5'te |
| Handler -> l6/l7 | X | kayit/gorunum bakilmaz |
| Handler -> l3-crosscutting | X (matris) | `ILogSink`/`IClock` **DOGRULAMA GEREKLİ** |
| Handler icinde 2. commit | X | kismi yazim riski |

Izinli: handler -> l1 (kural cagrisi), handler -> l0 (port, `Result`, dispatcher sozlesmesi).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N14 | Handlers | Govde | VersaCoder.Application |
| N12 | Commands | Girdi tipi | VersaCoder.Application |
| N16 | Validators | 1. adim | VersaCoder.Application |
| N15 | DTO/Mappers | Son adim | VersaCoder.Application |
| N18 | Scheduling | Calisma zamani | VersaCoder.Application |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, port, dispatcher | asagi | tek omurga |
| l1: entity, invariant, event | asagi | kural cagrisi |
| l3: log, saat, retry | X (matris) | **DOGRULAMA GEREKLİ** |
| l4/l5/l6/l7 + EF | YOK | NetArchTest = 0 |
| Harici paket (MediatR) | YOK | kendi dispatcher'i |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Mutlu yol + her `ErrorCode` yolu | 10 use case x >=2 |
| Unit | Adim sirasi (dogrula -> yukle -> kural -> commit -> event) | `Received.InOrder` |
| Unit | Tek `CommitAsync` | `Received(1)` |
| Unit | Istisna yukari sikiyor mu | firlatan sahte port |
| Arch | -> l4/l5/l6/l7 + EF = 0 | NetArchTest |

## §7 Hata Durumlari

| Durum | ErrorCode | Sonuc |
|---|---|---|
| Alan hatasi | `Invalid` + `Fields` | hicbir port cagrisi yok |
| Kayit yok | `NotFound` | commit yok |
| Invariant ihlali | `Conflict` | commit yok |
| Beklenmedik istisna | `Database`/`Protocol` | `Result.Fail` + log; exception cikmaz |
| 2. `CommitAsync` | - | kod incelemesinde **RED** |
| Kayitsiz tip | `NotImplemented` | l6 kayit eksik |

## §8 Kurallar (5 madde)

- Adim sirasini bozma; sira testle sabitlenir.
- Tek commit; ikinci `CommitAsync` goremezsin.
- Exception'i yutup `Result.Fail` + log ile bitir; yukari sikistirma.
- Invariant'i handler'da tekrar yazma; l1'e birak.
- `net48` bacakta `ConfigureAwait` tek tip tutulur (UI `SynchronizationContext`'e bagli).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
