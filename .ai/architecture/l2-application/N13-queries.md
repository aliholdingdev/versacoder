---
title: "N13 - Queries (Okuma, Sayfalama, DTO Donusu)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N13 - Queries (Okuma, Sayfalama, DTO Donusu)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N14-handlers.md](N14-handlers.md) | [N15-dto-mappers.md](N15-dto-mappers.md) | [N17-use-cases.md](N17-use-cases.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir sorgunun ne okudugunu, ne kadar dondugunu ve DTO disinda ne cikarmadigini belirler.

## §0 Ozet (Answer First)

Query **durum degistirmez**, yalniz okur ve donusunu **her zaman DTO** ile yapar (`Result<ProjectDto>`, `Result<PagedResult<TaskDto>>`). Sayfalama zorunludur: `Skip`/`Take` sinirli (`Take` 1..100), sonsuz liste yoktur. Sorgu SQL **gormez**; `IRepository`/port uzerinden okur, hangi saglayicinin calistigini bilmez. Bos sonuc hata **degildir** (`IsSuccess` + bos liste).

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N13, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Envanter | 5 sorgu |
| Varsayilan yon | l0 + l1 (asagi) |

## §1 Sorumluluk

Tek sorumluluk: **guvenli okuma** — sinirli veri, saf DTO, port uzerinden.

| Sorgu | Girdi | Donus | Port cagrisi |
|---|---|---|---|
| `ListProjectsQuery` | `Skip`, `Take` | `Result<PagedResult<ProjectDto>>` | `IRepository.ListAsync` |
| `GetProjectQuery` | `ProjectId` | `Result<ProjectDto>` | `GetAsync` |
| `OpenFileQuery` | `RelativePath`, `LineRange?` | `Result<FileContentDto>` | `IFileSystem.ReadAllTextAsync` |
| `ListTasksQuery` | `ProjectId?`, `Skip`, `Take` | `Result<PagedResult<TaskDto>>` | `ITaskRepository.ListAsync` |
| `ListSettingsQuery` | `Scope?` | `Result<IReadOnlyList<SettingDto>>` | `ISettingRepository.ListAsync` |

```csharp
public sealed record ListProjectsQuery(int Skip, int Take);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Skip, int Take);
```

**SINIR:** `MessageDto` / `GetTranscriptQuery` **burada degil** — l5'tedir; `l2 -> l5` yasaktir.

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Sayfalama/suzgec degerleri | l7 ekran |
| GIRENLER | Port sonucu (kayit veya bos) | l0 sozlesmesi (uygulama l4) |
| CIKANLAR | `Result<PagedResult<DTO>>` | l7 |
| CIKANLAR | Bos sonuc (hata degil) | ekran bos durumu |
| CIKANLAR | `Invalid` (sinir disi) | port cagrilmez |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| Sorgu -> l4-infrastructure | X | SQL gormez |
| Sorgu -> l5-protocol | X | transcript sorgusu l5'te |
| Sorgu -> l6/l7 | X | geri donus yok |
| Sorgu -> l3-crosscutting | X (matris) | **DOGRULAMA GEREKLİ** |
| Entity disari cikmak | X | imza DTO ile kisitli |

Izinli: sorgu -> l0 (port, `Result`), sorgu -> l1 (`PathPolicy` okumasi icin kural).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N13 | Queries | 5 sorgu | VersaCoder.Application |
| N15 | DTO/Mappers | Donus tipi | VersaCoder.Application |
| N16 | Validators | Sayfalama siniri | VersaCoder.Application |
| N14 | Handlers | Sorguyu isler | VersaCoder.Application |
| N29 | repositories | Port uygulamasi | VersaCoder.Infrastructure |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: port, `Result` | asagi | tek omurga |
| l1: `PathPolicy` | asagi | yol kurali |
| l4/l5/l6/l7 | YOK | sifir |
| Harici ORM/paket | YOK | G2 (EF yasak) |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Gecerli sorgu -> DTO listesi, alan kaybi yok | once RED |
| Unit | `Take` sinir degerleri (0, 1, 100, 101) | sinir +/- 1 |
| Unit | Bos sonuc -> `IsSuccess` (hata degil) | sik hata |
| Arch | Entity donmuyor (imza DTO ile kisitli) | imza denetimi |
| Arch | Sorguda SQL metni / `@pN` yok | kod arama testi |

## §7 Hata Durumlari

| Durum | ErrorCode | Sonuc |
|---|---|---|
| `Take` 1..100 disi | `Invalid` | port cagrilmez |
| `Skip` negatif | `Invalid` | port cagrilmez |
| Id bulunamadi | `NotFound` | bos liste **hata degil** |
| Dosya yolu koken disi | `Invalid` (`PathPolicy`) | okuma yapilmaz |
| DB hatasi | `Database` | hata paneli + log no |

## §8 Kurallar (5 madde)

- Donus tipi her zaman DTO; entity hic disari cikmaz.
- Sayfalama zorunlu: `Take` 1..100, `Skip >= 0`.
- Bos sonuc hata degil; `IsSuccess` + bos liste doner.
- Sorgu SQL/port **gormez**; hangi saglayicinin calistigini bilmez.
- Transcript sorgusu (l5) buraya **kopyalanmaz** (SSOT).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
