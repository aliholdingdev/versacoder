---
title: "N49 — MAKRO l4 Infrastructure"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "N49 makro dugumu: l4 katmaninin tamami - 5 kit, sorumluluk/giren/cikan, ADR-028 yasak yonleri, N25-N32 bagimliligi ve test stratejisi (kayit tablosu DEGIL)"
---

# N49 — MAKRO l4 Infrastructure

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [sistem ozeti](../00-overview/system-overview.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** *l4 katmaninin **tamami** hangi isleri yaptigi, **kime bakabildigi** ve disariya hangi imzalarla dondugu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l4, uygulamanin **fiziksel dunyayla temas eden tek katmanidir** (veritabani, dosya, surec, git, LLM agi): N25..N32 alt dugumleri tek bir sorumluluk tasiir — **cagrilan taraftir**, yukariya yalniz `Result<T>` ve domain event doder; **asla** l2/l5/l6/l7'ye bakmaz. Tek proje `VersaCoder.Infrastructure` icinde toplanir; disiplin assembly siniriyla degil **ic kod kurallari + NetArchTest** ile denetlenir. 10 dosyali klasor: bu makro (N49), N25..N32 alt dugumleri ve `index.md` sartnamesi.

---

## §1 Sorumluluk (5 kit)

| Kit | Alt dugum | Disariya acilan tek cumlelik imza |
|---|---|---|
| `IInfrastructureCore` | N25 | `IDapperSession.QueryAsync<T>(string sql, object p, CancellationToken ct)` |
| `IDataKit` | N26 + N27 + N28 | `IDbConnectionFactory.Create()` · `IMigrationRunner.RunAsync(ct)` |
| `IStorageKit` | N29 | `IFileSystemStore.WriteAllTextAtomicAsync(path, text, ct)` |
| `IGitKit` | N30 | `IGitClient.RunAsync(string[] args, CancellationToken ct)` |
| `ILlmKit` | N31 + N32 | `ILlmHttpClient.StreamAsync(request, ct)` · `ToolExecutor.ExecuteAsync(call, ct)` |

**Yapmaz:** use case/DTO tanimaz (l2), sema dogrulamaz (l5), DI kaydetmez (l6), ekran acmaz (l7), EF Core / `DbContext` kullanmaz (ADR-005).

---

## §2 GIRENLER

| # | Giren | Kaynak | Kapı |
|---|---|---|---|
| 1 | Port cagrisi (`IUnitOfWork`, `IRepository`) | `l2` use case | `l0` arayuzu, somut tip yok |
| 2 | Baglanti ayari (sifreli `ConnectionString`) | `l6` N40 config kopusu | DPAPI cozulmus deger |
| 3 | Migration tetikleyici | `l6` N38 acilis boru hatti | `IMigrationRunner.RunAsync` |
| 4 | Arac cagrisi (`ToolCall`) | `l5` N33 semasi | gecerli `ToolResult` semasi |
| 5 | Ekran niyeti (`commandId`) | `l7` | `ICommandDispatcher` (l2) uzerinden |

---

## §3 CIKANLAR

| # | Çıkan | Alıcı | Biçim |
|---|---|---|---|
| 1 | `Result<T>` / `Result.Fail(...)` | `l2`, `l6` | tek donus tipi |
| 2 | Domain event yayini | `l6` subscriber | `l0` events-abi |
| 3 | Kalici veri (`versacoder.db`) | dosya | parametreli SQL (`@p1..@pN`) |
| 4 | `MigrationReport` (ad + checksum + sure) | `l6` | saf SQL + `__Migrations` |
| 5 | Log kaydi | `l3` `ILogSink` portu | sure + satir (hassas deger yok) |

---

## §4 Yasak Yonler (ADR-028 matrisi)

| Yön hücrei | Değer | Anlam |
|---|---|---|
| `l4 -> l2` | **X** | siradaki ok siralamadir, kenar degildir |
| `l4 -> l5`, `l4 -> l6`, `l4 -> l7` | **X** | ust katmana kosulsuz bakis yok |
| `l2 -> l4` | **YOK** | port `l0`'da, uygulamasi `l4`'te; dogrudan ok yok |
| `l4 -> l3` | **X** | `dependency-map.md` kazanir → `DOĞRULAMA GEREKLİ` |
| `l4 -> l0` | `?` | ADR-028 §2.3/6 portlari `l0`'a yazar → `DOĞRULAMA GEREKLİ` |

```text
   l7   l6   l5   l4        (ust katmanlar - bakamaz)
l4 |  X    X    X    -
     |
     v  bakabilir: l0 portlari + izinli alt tipler
   sonuc: yalniz Result<T> + domain event yukari cikar
```

---

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N25 | ado-dapper | N29 | fs-tools |
| N26 | sqlite-provider | N30 | git-adapter |
| N27 | sqlserver-provider | N31 | llm-http |
| N28 | migrations | N32 | tool-runner |
| N49 | makro l4 (5 kit) | — | — |

Seviye kurali: N kodlari klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

---

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Infrastructure` | `net48;net10.0-windows` | katmanin tek projesi (9 uretimden biri) |
| `VersaCoder.Tests` | çift bacak | unit + entegrasyon (`Category=Infrastructure`) |
| `VersaCoder.ArchitectureTests` | çift bacak | NetArchTest yon denetimi |

`net48` ve `net10.0-windows` **ayni kaynak dosyayi** paylasir; `#if` yalniz platform farkindadir (is mantigi, SQL metni, provider secimi `#if` icine girmez).

---

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tür |
|---|---|---|
| l4 | `VersaCoder.Abstractions` (l0): portlar, `Result`, `EntityId` | izinli (`ProjectReference`) |
| l4 | `VersaCoder.Domain` (l1): aggregate/entity tipleri | matriste `.` → gecisli, `DOĞRULAMA GEREKLİ` |
| l4 | `VersaCoder.Crosscutting` (l3): `ILogSink`, `ICacheStore`, `IClock` | matriste `X` → `dependency-map.md` |
| l4 | ADO.NET, `Microsoft.Data.Sqlite`, `Microsoft.Data.SqlClient`, Dapper | surum `Directory.Packages.props` |
| l4 | EF Core / Npgsql / MySql / Mongo | **yasak** (ADR-005 / ADR-028) |

---

## §8 Test Stratejisi

| # | Test | Tip | Beklenen |
|---|---|---|---|
| 1 | NetArchTest: `l4 -> l2/l5/l6/l7` kenari | mimari | **0 ihlal** |
| 2 | Bos DB → migration → sorgu → sonuc | entegrasyon | yeşil |
| 3 | Her `DbCommand` timeout icerir (kod taramasi) | statik | 0 eksik |
| 4 | Her istisna en az bir `Result` koduna eslenir | birim | tablo gudumlu |
| 5 | Sorgu ≤ 50 ms, acik baglanti 0 | perf | butce asilmaz |

13 soru icin [layer-boundary-check](../../checklists/layer-boundary-check.md); kosum `dotnet test --filter Category=Infrastructure`.

---

## §9 Kanit

| # | İddia | Kanıt | Beklenen |
|---|---|---|---|
| 1 | Dosya 2.600–8.500 bayt | `Get-Item .Length` | aralık içinde |
| 2 | Klasor 10 dosya (index + N25..N32 + N49) | klasör listesi | 10 |
| 3 | Bozuk link yok | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | 0 |
| 4 | Concat SQL = 0, EF referansi 0 | kod taramasi | `src/` yokken `DOĞRULAMA GEREKLİ` |
| 5 | Emoji / Unicode kutu / mojibake yok | `measure-vault.ps1` | 0 |
