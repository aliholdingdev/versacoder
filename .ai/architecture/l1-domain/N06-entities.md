---
title: "Dugum Detayı N06 — Domain Varliklari (l1)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Aggregate root entity tanimlari (Project, AgentTask, Setting), uretim metotlari ve alan yazma kurallari"
---

# Dugum Detayı N06 — Domain Varliklari (l1)

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [test-strategy](../../spec/test-strategy.md) · [N07-value-objects](N07-value-objects.md)

> **Tek cumlelik sorumluluk:** *Bir varlik kimden olusur, disaridan nasil kurulur ve alani kim yazabilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l1'de uc aggregate root vardir: `Project`, `AgentTask`, `Setting` — her biri **fabrika metodu** (`Create`) ile kurulur, alan yazimi **kontrolludur** (`private set`), durum degisikligi dogrudan atama ile **yapilmaz**. Entity hicbir altyapi tipine bakmaz (SQL/Form/JSON yok); kalıcilik l4, gorunum l7'dedir. Varsayilan kurucu **yalniz** kalicilik (Dapper esleme) icindir ve `private`'dir.

| Alan | Deger |
|---|---|
| N kodu / alt katman | N06 · entities |
| Makro katman / proje | l1 Domain · `VersaCoder.Domain` |
| Giren cagri | l2 komut handler'i, l4 esleme (private kurucu) |
| Cikti | `Result<Project|AgentTask|Setting>` |
| Bagimli invariant | I1..I7 ([N10-invariants](N10-invariants.md)) |

---

## §1 Sorumluluk (NE YAPAR)

| # | Entity | Temel alanlar | Fabrika | Sorumluluk |
|---|---|---|---|---|
| 1 | `Project` | `ProjectId`, `Name`, `RootPath`, `CreatedUtc` | `Project.Create(name, rootPath, utc)` | calisma alaninin kimligi ve koku |
| 2 | `AgentTask` | `TaskId`, `ProjectId`, `State`, `Attempt`, `PromptRef`, `CreatedUtc`, `CompletedUtc` | `AgentTask.Create(projectId, promptRef, utc)` | gorev yasam dongusu |
| 3 | `Setting` | `Key`, `Scope`, `Value`, `UpdatedUtc` | `Setting.Create(key, scope, value, utc)` | yapilandirma yuzeyi |
| 4 | yazma denetimi | tum property `private set` | - | disaridan alan yazimi derleme hatasi |
| 5 | zaman alanlari | `DateTimeOffset` | `IClock` girdisi | `DateTime.Now` yasak |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l2 `CreateProjectHandler` -> `Project.Create` | izinli ok | `l2 -> l1` matriste OK |
| GIREN | l4 Dapper esleme -> `private` kurucu | izinli ok | `l4 -> l1` matriste `.` (gecis) |
| GIREN | guard (`Guard.NotNull`) arguman kontrolu | girdi | istisna = programlama hatasi |
| CIKAN | `Result<T>` (basari/basarisizlik) | cikti | `Invalid` / `Conflict` |
| CIKAN | domain event (fabrika icinde) | cikti | yayim l2, commit sonrasi |
| YASAK | `l7 -> l1` (ekran entity'ye baglanir) | yasak | ekran l2 DTO'suna baglanir |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dugumdeki karsiligi |
|---|---|---|---|
| 1 | `l1 -> l2, l4, l5, l6, l7` | ADR-028 §2.1 | entity `DbConnection`/`Form` gormez |
| 2 | `l1 -> l3` | ADR-028 matrisi `X` | **DOĞRULAMA GEREKLİ** (zaman portu karari) |
| 3 | `l2 -> l4` | ADR-028 §2.1 | handler `Project`'i port uzerinden yukler |
| 4 | `l5 -> l4` | ADR-028 §2.3/2 | protokol katmani altyapıya bakmaz |
| 5 | `l7 -> l1` | ADR-028 §2.1 | ekran DTO, entity degil |

---

## §4 N Kodu ve Proje

| Alan | Deger |
|---|---|
| Katalog kaydi | `../catalog/index.md` altinda `N06` satiri |
| Kod yolu | `src/VersaCoder.Domain/Entities/` (`Project.cs`, `AgentTask.cs`, `Setting.cs`) |
| Proje | `VersaCoder.Domain` (tek .csproj, cift TFM) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | l0 `Result<T>`, `Guard` | l1 -> l0 | **izinli** | tek hata dili |
| 2 | l0 `IRepository<,>` genisletmesi | l1 -> l0 | **izinli** | `IProjectRepository` vb. |
| 3 | `System.Data` / Dapper / EF | l1 -> paket | **yasak** | `.csproj` = 0 satir |
| 4 | l2, l4 -> entity | ust -> l1 | **izinli** | `ProjectReference` |
| 5 | DevExpress / JSON serializer | l1 -> l7/l5 | **yasak** | sema l5, ekran l7 |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Unit | her `Create` mutlu yolu: `IsSuccess` + alanlar | **once RED** |
| Unit | her `Create` hata yolu: beklenen `ErrorCode` (I1..I7 ile eslesmeli) | 7/7 |
| Unit | `private set` disindan alan yazilamaz | derleme denetimi |
| Arch | `VersaCoder.Domain -> l2/l4/l5/l6/l7` + `EntityFramework*` = 0 | **0** |
| Arch | parametresiz kurucu `private` (reflex testi) | 0 disari acik |

---

## §7 Hata Durumlari

| Durum | `ErrorCode` | Kaynak | Sonuc |
|---|---|---|---|
| `RootPath` bos / `..` icerir (I1, I7) | `Invalid` | `Project.Create` | entity uretilmez |
| Ayni normallesmis ad tekrar (I2) | `Conflict` | l1 + port sorgusu | proje eklenmez |
| Gecersiz durum gecisi (I3) | `Conflict` | `TaskStateMachine` | `State` degismez |
| `Attempt` siniri asildi (I5) | `Conflict` | `AgentTask` | yeniden deneme yok |
| `null` fabrika argumani | **istisna** (fail-fast) | `Guard` | programlama hatasi |

---

## §8 Kurallar

- Varsayilan kurucu `private`'dir; disari `new Project(...)` ile kurulum **yapilmaz**.
- Fabrika mutlaka `Result<T>` dondurur; hata yolu `null` deger uretmez.
- Zaman alanlari `IClock` girdisiyle doldurulur — `DateTime.Now` Arch taramasinda 0 olmalidir.
- Entity'nin gorevi kural degildir; is hesabi domain servisine ([N08-aggregates](N08-aggregates.md)) aittir.
- Yeni entity = bu tabloya + [N10-invariants](N10-invariants.md) invariant satirina ayni gun eklenir.

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N06-entities.md).Length` | olcum |
| 3 | Yazma denetimi | derleme: `private set` disindan yazim hatasi | test |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 5 | Madde listesi <= 5 | §8 = 5 madde | sayim |
