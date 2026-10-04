---
title: "Dugum Detayı N01 — Port Sozlesmeleri (l0)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "N01 port sozlesmelerinin imzalari, uygulayan/tuketen katman, hata yollari ve bu dugumun ADR-028 yon kurallari icindeki yeri"
---

# Dugum Detayı N01 — Port Sozlesmeleri (l0)

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [test-strategy](../../spec/test-strategy.md) · [N02-result-error](N02-result-error.md)

> **Tek cumlelik sorumluluk:** *Bir port imzasi kimde tanimlanir, kim uygular ve basarisizligi nasil `Result` olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Portlar **l0**'da tanimlanir, **l4** (LLM icin **l5**) tarafindan uygulanir, **l2/l5** tarafindan cagrilir; bu uc nokta bozulursa katman fikri koddan kopar. Port **asla** exception firlatmaz, her donus [N02-result-error](N02-result-error.md) `Result<T>` icerir. Tip-spesifik port (`IProjectRepository`) l0'da duramaz, cunku entity tipi l1'dedir ve `l0 -> l1` yasaktir.

| Alan | Deger |
|---|---|
| N kodu / alt katman | N01 · ports-core |
| Makro katman | l0 Abstractions |
| Proje / TFM | `VersaCoder.Abstractions` · `net48;net10.0-windows` |
| Uygulayan katman | l4 (Dapper adaptoru), `ILlmTransport` icin l5 |
| Tuketen katman | l2 handler, l5 tool, l6 DI kaydi |

---

## §1 Sorumluluk (NE YAPAR)

| # | Port | Cekirdek imza | Uygulayan | Tuketen |
|---|---|---|---|---|
| 1 | `IRepository<TEntity,TId>` | `GetAsync`, `ListAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | l4 | l2 |
| 2 | `IUnitOfWork` | `CommitAsync`, `RollbackAsync` | l4 | l2 |
| 3 | `IDbConnectionFactory` / `ISqlDialect` | `Open()`, `ParameterPrefix`, `ApplyPaging` | l4 | l4 ici |
| 4 | `IFileSystem` / `IProcessRunner` | `ReadAllTextAsync`, `WriteAllTextAtomicAsync`, `RunAsync` | l4 | l5 |
| 5 | `ILlmTransport` | `SendAsync(LlmRequest)` | **l5** | l5 ici |
| 6 | `IDispatcher` | `SendAsync<TIn,TOut>`, `QueryAsync<TIn,TOut>` | **l2** | l6, l7 |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l1 tip-spesifik genisletme (`IProjectRepository : IRepository<Project,Guid>`) | izinli ok | l1 -> l0 |
| GIREN | l4/l5 adaptoru bu imzalari uygular | izinli ok | govde l0'da degil |
| CIKAN | port imzalari + `Result<T>` donus yolu | cikti | istisna sinir disina cikmaz |
| CIKAN | dispatcher kayit sozlesmesi | cikti | kayit l6, uygulama l2 |
| YASAK | l0 -> `System.Data` / Dapper / DevExpress | yasak ok | saglayici yalniz l4 |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dugumdeki karsiligi |
|---|---|---|---|
| 1 | `l0 -> l1` | ADR-028 §2.1 | `Project` adi port dosyasina yazilamaz |
| 2 | `l2 -> l4` | ADR-028 §2.1 | handler `DbConnection` gormez, portu cagirir |
| 3 | `l4 -> l2` | ADR-028 §2.3/2 | adaptoru use case tiplerine bakamaz |
| 4 | `l5 -> l4` | ADR-028 §2.3/2 | `ILlmTransport` uygulamasi l5'te kalir |
| 5 | `l7 -> l4` | ADR-028 §2.1 | ekran portu l6 DI uzerinden alir |

---

## §4 N Kodu ve Proje

| Alan | Deger |
|---|---|
| Katalog kaydi | `../catalog/index.md` altinda `N01` satiri (sayim birimi `| N##-### |`) |
| Kod yolu | `src/VersaCoder.Abstractions/` icinde `Ports/`, `Repositories/`, `Data/` klasorleri |
| Proje | `VersaCoder.Abstractions` (tek .csproj, cift TFM) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL (`Task`, `CancellationToken`) | l0 -> System | **izinli** | net48 icin ek paket gerekmez |
| 2 | NuGet paketi | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | `VersaCoder.Domain` tipleri | l0 -> l1 | **yasak** | genisletme l1'de yasir |
| 4 | l2, l4, l5 -> N01 portlari | ust -> l0 | **izinli** | `ProjectReference` |
| 5 | NSubstitute (yalniz test) | test -> l0 | **kosullu** | sahte portla handler testi |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Unit | sahte port (NSubstitute) ile handler testi: l2 SQL bilmeden calisir | %90 satir |
| Unit | port imzalari iki TFM'de birebir ayni | `#if` = 0 |
| Arch | `VersaCoder.Abstractions -> VersaCoder.*` = 0 | NetArchTest |
| Arch | NuGet paketi referansi = 0 | `.csproj` denetimi |
| Sozlesme | her `public interface` XML yorumu tasiyor | 0 eksik |

---

## §7 Hata Durumlari

| Durum | `ErrorCode` | Donusu yapan | Not |
|---|---|---|---|
| Kayit bulunamadi | `NotFound` | port uygulayicisi (l4) | bos liste **hata degildir** |
| SQL / IO hatasi | `Database` | l4 adaptoru | mesaj temizlenir, `Detail` loga |
| Sure asimi | `Timeout` | l4 (`CommandTimeout`) | `IRetryPolicy` (l3) devreye girer |
| Iptal | `Cancelled` | her katman | `Result.Fail`, exception ile degil |
| Null arguman | - | **istisna** | programlama hatasi = fail-fast |

---

## §8 Kurallar

- Yeni port = [N01-ports-core](N01-ports-core.md) §1 tablosuna ayni gun satir; tablosuz port mimari olarak yoktur.
- Port govdesine is mantigi yazilmaz; govde yalnizca l4/l5 adaptorundedir.
- `CancellationToken`'siz asenkron imza gerekcesi yazilmadan kabul edilmez.
- Tip-spesifik port l0'da degil, l1'de tanimlanir (genisletme arayuzu).
- Kayit yolu: kod yolu `[N kodu] -> src/VersaCoder.Abstractions/` eslemesi [catalog](../catalog/index.md) uzerindedir.

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N01-ports-core.md).Length` | olcum |
| 3 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 4 | Madde listesi <= 5 | §8 = 5 madde | sayim |
| 5 | Kod `/` deseni `@p1` parametrelendirme disindaki SQL icermez | bu dosyada SQL metni yok | gozden gecirme |
