---
title: "N12 - Commands (Durum Degistiren Istekler)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N12 - Commands (Durum Degistiren Istekler)

**Zorunlu Baglantilar:** [index.md](index.md) | [N13-queries.md](N13-queries.md) | [N14-handlers.md](N14-handlers.md) | [N16-validators.md](N16-validators.md) | [N17-use-cases.md](N17-use-cases.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir komutun ne tasidigini, kimde tanimli oldugunu ve handler'a nasil ulastigini belirler.

## §0 Ozet (Answer First)

Command = **durum degistiren tek yonlu istek**: yalniz girdi tasir, donus tipi handler'da `Result<TDto>` olarak belirlenir. Tanimi **l2**'dedir ve DTO'yu cagirir, l1 entity'yi **degil**. `sealed record`/`sealed class`, **mutasyon yok** (`set` yok). Ulasim: `IDispatcher.SendAsync` (l0 sozlesmesi) -> kayitli handler; kayit **l6**'dadir. Komut icine entity, `DateTime`, baglam nesnesi **konmaz**.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N12, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Envanter | 5 komut (asagidaki tablo) |
| Varsayilan yon | l0 + l1 (asagi) |

## §1 Sorumluluk

Tek sorumluluk: **niyeti tasiyan degistirilemez paket**. Envanter:

| Komut | Alanlar | Ekran | Donen sonuc |
|---|---|---|---|
| `CreateProjectCommand` | `Name`, `RootPath` | 02-project-new | `Result<ProjectDto>` |
| `SaveFileCommand` | `RelativePath`, `Text` | 03-editor | `Result<FileContentDto>` |
| `StartAgentTaskCommand` | `ProjectId`, `PromptRef` | 04-agent-chat / 05-tasks | `Result<TaskDto>` |
| `CancelAgentTaskCommand` | `TaskId` | 05-tasks | `Result<TaskDto>` |
| `UpdateSettingCommand` | `Key`, `Scope`, `Value` | 07-settings | `Result<SettingDto>` |

```csharp
public sealed record CreateProjectCommand(string Name, string RootPath);
public interface ICommandHandler<TCommand, TResult>   // l0 sozlesmesi
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken ct);
}
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Kullanici niyeti (deger alanlar) | l7 ekran |
| GIRENLER | Dispatcher kaydi (typeof -> handler) | l6 composition |
| CIKANLAR | `Result<TDto>` | l7 |
| CIKANLAR | Kayitsiz tip icin `NotImplemented` | dispatcher |
| CIKANLAR | Katman logu | l3 uzerinden |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| Komut -> l4-infrastructure | X | port l0'da, uygulama l4'te |
| Komut -> l5-protocol | X | transcript komutlari l5'te |
| Komut -> l6-host | X | kayit orada, komut bakmaz |
| Komut -> l7-ui | X | ekran tipi tanimlanmaz |
| Komut -> l3-crosscutting | X (matris) | saat/log baglantisi **DOGRULAMA GEREKLİ** |

Izinli: komut -> l1 (entity degil, tip olarak bakmaz), komut -> l0 (`Result`, handler sozlesmesi).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N12 | Commands | 5 komut tanimi | VersaCoder.Application |
| N14 | Handlers | Komutu isler | VersaCoder.Application |
| N16 | Validators | Girdi kontrolu | VersaCoder.Application |
| N17 | Use Cases | U2/U5/U6/U7/U10 | VersaCoder.Application |
| N39 | di-registration | Dispatcher kaydi | VersaCoder.Host |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, `ICommandHandler` | asagi | tek omurga |
| l1 | asagi | komut entity tasimaz |
| l4/l5/l6/l7 | YOK | sifir |
| NuGet (MediatR vb.) | YOK | dispatcher bu projede (G8) |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Her komut icin en az 2 yol (mutlu + hata kodu) | once RED |
| Unit | Kayitsiz komut -> `NotImplemented` | dispatcher kaydi |
| Unit | Komut degistirilemez (`record`/`init`) | derleme denetimi |
| Unit | Alan bazli `Fields` dolulugu | 5 alan |
| Arch | Komut siniflarinda `System.Data`/DevExpress yok | NetArchTest |

## §7 Hata Durumlari

| Durum | ErrorCode | Sonuc |
|---|---|---|
| Alan dogrulamasi basarisiz | `Invalid` + `Fields` | entity uretilmez |
| Komsu kayit yok | `NotFound` | islem baslamaz |
| Durum catismasi | `Conflict` | l1 makinesi engeller |
| Dispatcher kaydi yok | `NotImplemented` | l6 kayit eksik |
| `ct` iptali | `Cancelled` | kismi yazim yok |

## §8 Kurallar (5 madde)

- Komuta entity, `DateTime`, `ILogger` koyma; yalniz niyet degerleri tasisin.
- Donus tipi her zaman handler'da `Result<TDto>`.
- Kayitta olmayan tip `NotImplemented` doner; envanter disi komut **uydurulmaz**.
- `net48` bacakta `record` karari (sealed class mi, `IsExternalInit` mi) bir kez verilir, iki bacak ayni olur — `NEEDS_VALIDATION`.
- Komutlar `#if` icermaz (iki bacak ayni kod).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
