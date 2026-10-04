---
title: "N18 - Scheduling (Calisma Zamani, Iptal, Retry)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N18 - Scheduling (Calisma Zamani, Iptal, Retry)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N14-handlers.md](N14-handlers.md) | [N17-use-cases.md](N17-use-cases.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir komutun ne zaman, hangi sira ile calistigini ve iptal edilirse nasil durdugunu belirler.

## §0 Ozet (Answer First)

l2 **kendi kendini zamanlamaz**: is ya senkron (`IDispatcher` cagrisi) ya da **l6 tarafindan baslatilan arka plan isidir**. Kurallar: **tek komut = tek is = tek commit**, `CancellationToken` her adimda kontrol edilir, basarisiz is `IRetryPolicy` altina girer (**en fazla 1 tekrar**, yalniz transient) ve `Failed` durumuna gecer. Sonsuz dongu, `Thread.Sleep`, gorunum is parcacigi cagrisi **yoktur**.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N18, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Calisma zamani | dispatcher + retry + iptal |
| Varsayilan yon | l0 + l1 (asagi) |

## §1 Sorumluluk

Tek sorumluluk: **calisma zamani kalibi** — sira, iptal, tek tekrar.

| Parca | Imza / kural | Sorumluluk |
|---|---|---|
| `IDispatcher.SendAsync/QueryAsync` (l0) | `Task<Result<TOut>>` | is baslatma (senkron) |
| `CancellationToken` | her `HandleAsync` zorunlu parametre | iptal edilebilirlik |
| Retry politikasi | en fazla **1** tekrar, yalniz transient | `Timeout`/baglanti hatasi |
| `TaskStateMachine` (l1) | `Pending->Running->...` | durum gorunurlugu |
| `Attempt` sayaci (l1) | yapilandirilan sinir | yeniden deneme limiti |
| l6 arka plan is parcacigi | `Task.Run`/is parcacigi **l6'da** | zamani l2'ye yikmaz |

```csharp
var started = task.Transition(TaskState.Pending, TaskState.Running);   // l1
if (!started.IsSuccess) return Result<TaskDto>.Fail(started.Error);
var run = await _retry.ExecuteAsync(ct => DispatchCoreAsync(cmd, ct), ct);  // 1 tekrar
if (!run.IsSuccess) { await MarkFailedAsync(task, run.Error, ct); return run; }
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Is baslatma (senkron veya arka plan) | l7 / l6 |
| GIRENLER | Transient hata (`Timeout`, `Database`) | port |
| GIRENLER | Iptal sinyali (`ct`) | l7 / l6 |
| CIKANLAR | Commit + event (basari) | l4 |
| CIKANLAR | `Failed` + `TaskFailed` (1 tekrar sonrasi) | l1 durumu |
| CIKANLAR | `Cancelled` (commit cagrisi 0) | ekran |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l2 -> l4-infrastructure | X | port uygulamasi |
| l2 -> l5-protocol | X | agent dongusu l5 |
| l2 -> l6/l7 | X | is parcacigi l6'da |
| l2 -> l3-crosscutting | X (matris) | `IRetryPolicy` **DOGRULAMA GEREKLİ** |
| Kendi `Thread.Sleep`/`Task.Delay` | X | test edilebilirlik |

Izinli: scheduling -> l1 (durum gecisi), scheduling -> l0 (dispatcher, `Result`).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N18 | Scheduling | Calisma zamani kalibi | VersaCoder.Application |
| N14 | Handlers | Govde | VersaCoder.Application |
| N17 | Use Cases | U6/U7 cagrisi | VersaCoder.Application |
| N38 | bootstrap-pipeline | Arka plan isi baslatir | VersaCoder.Host |
| N10 | Invariants | Attempt siniri (l1) | VersaCoder.Domain |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: dispatcher, `Result` | asagi | tek omurga |
| l1: makine, invariant | asagi | durum |
| Retry/clock servisleri | X (matris) | **DOGRULAMA GEREKLİ** |
| l4/l5/l6/l7 | YOK | sifir |
| Harici scheduler paketi | YOK | — |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | `ct` iptal -> `Cancelled` + commit cagrisi **0** | once RED |
| Unit | Transient -> tam olarak **2** cagri (1+1) | sayac |
| Unit | Non-transient -> **1** cagri | gereksiz tekrar yok |
| Unit | Basarisiz -> `Failed` + `TaskFailed` eventi | olay sayaci |
| Arch | `Thread.Sleep`/`Task.Delay` yok | arama testi |

## §7 Hata Durumlari

| Durum | ErrorCode | Davranis |
|---|---|---|
| `ct` iptal edildi | `Cancelled` | o anki adimda durur, commit **yok** |
| Transient hata | `Timeout`/`Database` | **1 tekrar**, sonra `Failed` |
| Non-transient (`Invalid`, `Conflict`) | ayni | **tekrar YOK** |
| Deneme siniri doldu | `Conflict` | is biter |
| Ayni gorev 2. kez baslatildi | `Conflict` | ikinci is reddedilir |
| `Task.Run` icinde istisna | `Database`/`Protocol` | `Result.Fail` + `TaskFailed` |

## §8 Kurallar (5 madde)

- Tek komut = tek is = tek commit; ikinci commit yok.
- `ct` her `await`'te kontrol edilir; iptalde commit cagrisi 0 olmali.
- Tekrar **en fazla 1** ve yalniz transient hatalarda; kural hatasinda tekrar yok.
- `Thread.Sleep`/`Task.Delay` bu katmanda yazma; sureyi `FixedClock` ile sabitle.
- Arka plan is parcacigi l6'dadir; l2'ye zaman yikma.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
