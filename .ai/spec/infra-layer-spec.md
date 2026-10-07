---
title: "VersaCoder — Altyapı Katmanı Şartnamesi (l4 Infrastructure)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "l4 (N25–N32) sekiz alt katman sınırı — provider, fs, git, llm-http, tool-runner; neye bakabilir, hangi arayüzle döner"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Altyapı Katmanı Şartnamesi (`spec/infra-layer-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [architecture/index](../architecture/index.md) · [data-layer-spec](data-layer-spec.md) · [security-spec](security-spec.md) · [llm-provider-spec](llm-provider-spec.md) · [tool-contract-spec](tool-contract-spec.md)

> **Tek cümlelik sorumluluk:** *l4'ün sekiz alt katmanı hangi somut teknolojiyi kullanır, hangi arayüzle yukarı döner ve nerede durur* sorusunun tek cevabıdır.
> Veri erişim **kuralları** [data-layer-spec](data-layer-spec.md)'te, migration akışı [migration-spec](migration-spec.md)'tadır (tekrar yok).

---

## §0 Özet (Answer First)

l4 = **tek I/O katmanıdır**: veritabanı (ADO.NET + Dapper), dosya sistemi, git, LLM HTTP ve tool çalıştırma yalnız burada yaşar; geri dönüş **yalnız l0 arayüzleriyle** (`Result<T>`, portlar) olur, somut tip (`SqliteConnection`, `HttpClient`, `Process`) **hiçbir zaman yukarı çıkmaz** — l4, `l6` dışındaki hiçbir katmandan **yeşil ışık beklemez**.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | N25 ado-dapper · N26 sqlite-provider · N27 sqlserver-provider · N28 migrations · N29 fs-tools · N30 git-adapter · N31 llm-http · N32 tool-runner | SQL kuralları (`@p1..@pN`, timeout): [data-layer-spec](data-layer-spec.md) |
| 2 | Somut teknoloji: `Microsoft.Data.Sqlite` / `SqlClient 5.2.x`, Dapper, `Process`, `HttpClient` | Paket/sürüm kilidi: [packaging-spec](packaging-spec.md) |
| 3 | Provider soyutlaması (`IDbConnectionFactory`, `ISqlDialect`) | Secret saklama biçimi: [security-spec](security-spec.md) |
| 4 | LLM HTTP taşıması + streaming okuma | Provider önceliği/sağlayıcılar: [llm-provider-spec](llm-provider-spec.md) |
| 5 | tool-runner = çağrıyı sandbox + timeout ile çalıştırma | İzin/sandbox şeması: [tool-contract-spec](tool-contract-spec.md) |

> **YASAK:** EF Core (G2) · string-concat SQL (DA-03) · somut bağlantı tipini `Application`/`Domain`'e açmak (DA-04) · l4 dışına `HttpClient`/`Process` sızdırmak.

---

## §2 Davranış

### §2.1 Sekiz alt katman sınırı

| # | Düğüm | Somut teknoloji | Yukarı dönen arayüz | Bakamaz |
|---|---|---|---|---|
| N25 | `ado-dapper` | ADO.NET + Dapper | `IDbConnectionFactory` + `Result<T>` | UI, iş kuralı |
| N26 | `sqlite-provider` | `Microsoft.Data.Sqlite` (yerel/test/varsayılan) | `ISqlDialect` | provider seçimi kararı (N27 ile birlikte ADR-012) |
| N27 | `sqlserver-provider` | `Microsoft.Data.SqlClient` 5.2.x | `ISqlDialect` | bağlantı string'i üretimi (l3/l6) |
| N28 | `migrations` | saf SQL dosyaları | `IMigrationRunner` | elle şema değişikliği |
| N29 | `fs-tools` | `System.IO` (kanonik yol + kök kontrolü) | `IFileSystem` portu | UI dosya diyaloğu |
| N30 | `git-adapter` | `Process` + `ArgumentList` | `IGitPort` | dal stratejisi kararı |
| N31 | `llm-http` | `HttpClient` + SSE okuma | `ILLMProvider` | sağlayıcı önceliği (ADR-011) |
| N32 | `tool-runner` | `Process` + sandbox + timeout | `ITool` sonucu | izin verme kararı (`allow/ask/deny`) |

### §2.2 Akış (UTF-8 kutu-cizim)

```
   l7 UI -▶ l6 Host -▶ l5 Protocol -▶ l2 Application (handler)
                                              |
                                              v  (yalniz l0 arayuzleri: IPort / Result<T>)
                                    +---------l4---------+
                                    | ado-dapper  N25    |--▶ SQLite / SQL Server
                                    | migrations  N28    |--▶ .sql/*.sql
                                    | fs-tools    N29    |--▶ workspace diski
                                    | git-adapter N30    |--▶ git sureci
                                    | llm-http    N31    |--▶ OpenRouter / Ollama
                                    | tool-runner N32    |--▶ allowlist komutlari
                                    +--------------------+
        Somut tip (SqliteConnection, HttpClient, Process) ASLA yukari cikmaz.
```

### §2.3 Ortak davranış kuralları

| # | Kural | Kanıt şekli |
|---|---|---|
| 1 | Her I/O çağrısı `CancellationToken` alır ve aşağı iletir (CS-04) | İmza denetimi + iptal testi |
| 2 | Her async yol `async`/`await`; `.Result`/`.Wait()` yok (CS-03) | Kod taraması = 0 satır |
| 3 | Kaynak `using`/`await using` (CS-15) | Paylaşılan `IDbConnection`/`Process` = 0 |
| 4 | Hata yukarıya **tipli** döner (`Result<T>` / istisna), ham tip sızmaz (SEC-13) | Hata mesajı testi |
| 5 | Ağ yalnız N31'dedir; `fs/git/tool` ağ istemez | Statik tarama (tool-spec §6 kural 5) |

---

## §3 TFM Notu

| Bacak | Fark | Kural |
|---|---|---|
| `net48` | `HttpClient`/`Process` API'si ortak; `Microsoft.Data.Sqlite` sürümü CPM'de kilitli | `#if` yalnız gerekiyorsa tek satır |
| `net10.0-windows` | Modern API yüzeyi (`Span`, `await using` tam destek) | Ortak kaynak tercih edilir |
| Provider | İki bacakta **aynı** provider sırası (ADR-012) | Farklı TFM'de farklı DB davranışı = RED |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| IF-01 | Somut tip yalnız l4'te | NetArchTest: l2/l5/l7 → l4 = 0 ihlal |
| IF-02 | Gerçek SQLite üzerinde repository entegrasyonu | `Infrastructure/Integration/` testleri yeşil (`test-strategy` §3) |
| IF-03 | İptal ve timeout davranışı | `CancellationToken` testi + `VC-E-0205` |
| IF-04 | EF Core referansı yok | `.csproj`/`.props` taraması = **0 satır** (G2) |
| IF-05 | İki bacakta aynı davranış | `dotnet test -f net48` ve `-f net10.0-windows` = 0 failed |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
