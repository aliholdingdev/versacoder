---
title: "N17 - Use Cases (U1..U10 Envanteri, Orkestrasyon)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N17 - Use Cases (U1..U10 Envanteri, Orkestrasyon)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N13-queries.md](N13-queries.md) | [N14-handlers.md](N14-handlers.md) | [N16-validators.md](N16-validators.md) | [N18-scheduling.md](N18-scheduling.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir kullanici isteginin kac adimdan gectigini, sirasini ve her adimda hangi hata oldugunu belirler.

## §0 Ozet (Answer First)

On use case (U1..U10) l2'nin tam dis yuzeyidir; her biri **tek bir handler** ile temsil edilir ve **tek commit** ile biter. En karmasik olan `StartAgentTaskCommand` (U6) 9 adimdan olusur: dogrula -> yukle -> yetki -> gecis -> sinir -> event -> commit -> ilet -> DTO. **Transcript/sohbet isleri burada degildir** — `PostUserMessageCommand` l5'tedir, `l2 -> l5` yasaktir. Agent dongusu l5'tedir; l2 onu **cagirmaz**, yalniz durumu hazirlar ve event'i yayar.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N17, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Envanter | U1..U10 (10 use case) |
| Varsayilan yon | l0 + l1 (asagi) |

## §1 Sorumluluk

Tek sorumluluk: **envanter + adim sirasi**. U1..U10 disinda use case **uydurulmaz** (G9).

| # | Use case | Tip | Ekran | Adim ozeti |
|---|---|---|---|---|
| U1 | `ListProjectsQuery` | query | 01-start-page | port -> `PagedResult<ProjectDto>` |
| U2 | `CreateProjectCommand` | command | 02-project-new | dogrula -> `Project.Create` -> ekle -> commit -> event |
| U3 | `GetProjectQuery` | query | 03-editor | port -> `ProjectDto` |
| U4 | `OpenFileQuery` | query | 03-editor | `IFileSystem` -> icerik + `LineRange` |
| U5 | `SaveFileCommand` | command | 03-editor | `PathPolicy` -> atomik yazma -> sonuc |
| U6 | `StartAgentTaskCommand` | command | 04-agent-chat | 9 adim (asagi §2) |
| U7 | `CancelAgentTaskCommand` | command | 05-tasks | `Running->Cancelled` -> commit -> event |
| U8 | `ListTasksQuery` | query | 05-tasks | port -> `PagedResult<TaskDto>` |
| U9 | `ListSettingsQuery` | query | 07-settings | port -> `SettingDto` listesi |
| U10 | `UpdateSettingCommand` | command | 07-settings | dogrula -> tekil -> guncelle -> commit |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut/sorgu | l7 ekran |
| GIRENLER | U6 adim 1-3: dogrulama, port, yetki | l2 + l0 + l1 |
| GIRENLER | U6 adim 4-6: gecis, Attempt siniri, event | l1 makinesi |
| CIKANLAR | `Result<TDto>` (her adimda hata ile cikis mumkun) | l7 |
| CIKANLAR | Commit sonrasi event | l6 aboneligi |
| CIKANLAR | Rollback (adim 7 hata) | l4 |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l2 -> l5-protocol | X | transcript/agent dongusu l5'te |
| l2 -> l4-infrastructure | X | port uygulamasi |
| l2 -> l6/l7 | X | kayit/gorunum |
| l2 -> l3-crosscutting | X (matris) | saat/retry **DOGRULAMA GEREKLİ** |
| Envanter disi use case | X | G9 (uydurma) |

Izinli: use case -> l1 (kural), use case -> l0 (port, `Result`).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N17 | Use Cases | U1..U10 | VersaCoder.Application |
| N14 | Handlers | Her UC = 1 handler | VersaCoder.Application |
| N18 | Scheduling | U6 calisma zamani | VersaCoder.Application |
| N12/N13 | Commands/Queries | Girdi tipleri | VersaCoder.Application |
| N47 | Macro L2 | Makro ozet | VersaCoder.Application |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: port, `Result`, dispatcher | asagi | tek omurga |
| l1: entity, makine, event | asagi | kural |
| l4/l5/l6/l7 | YOK | NetArchTest = 0 |
| `Task.Delay`/zaman bekleme | YOK | zaman disarida |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | U1..U10 mutlu yolu | envanter kapsami |
| Unit | U6 9 adim **sirasi** | `Received.InOrder` |
| Unit | U6 **her** hata adimi (8 dallanma) | adim adim sahte port |
| Unit | Her use case tek `CommitAsync` | `Received(1)` |
| Arch | `l2 -> l5` = 0 (transcript siniri) | NetArchTest |

## §7 Hata Durumlari

| Adim (U6) | Hata | ErrorCode | Sonuc |
|---|---|---|---|
| 1 | alan hatasi | `Invalid` + `Fields` | durur |
| 2 | gorev/proje yok | `NotFound` | durur |
| 3 | yetki yok | `Unauthorized` | durur |
| 4-5 | gecersiz gecis / deneme siniri | `Conflict` | durur |
| 7 | SQL/IO | `Database` | **rollback**, event yayilmaz |
| 8 | l6 aboneligi yok | `NotImplemented` | is biter, olay kaybi loglanir |
| her an | `ct` iptali | `Cancelled` | rollback, temiz cikis |

## §8 Kurallar (5 madde)

- Her use case = tek handler = tek commit.
- Envanter (U1..U10) disinda senaryo **ekleme**; once envanteri genislet.
- Hata her adimda `Result.Fail` ile **cikar**; akisa devam etme.
- Agent dongusunu l2'den **cagirma**; durumu hazirla, baglantiyi l6 kurar.
- Zaman bekleme (`Task.Delay`) bu katmanda yok; sure testleri `FixedClock` ile yapilir.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
