---
title: "dual-tfm-provider-choice - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "net48 ve net10.0-windows bacaklarinda saglayici secimi: SqlClient ortak surum, SQLite yalniz net10 test, #if disiplini"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# dual-tfm-provider-choice - Iki Bacakta Saglayici Secimi

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[.decisions/adr-index|adr-index]] · [[knowledge/data/ado-net-connection|ado-net-connection]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Hangi TFM'de hangi saglayici paketi kullanilir, `#if` ne zaman serbesttir, hangi karar henuz dogrulanmamistir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek ortak taban: **`Microsoft.Data.SqlClient` 5.2.x + Dapper 2.1.x - her iki bacakta ayni surum**, Central Package Management ile tek satirda tutulur. **SQLite yalniz `net10.0-windows` testindedir**, `net48`'de kullanilmaz. Saglayici tercihi `#if` ile degil, **`IDbConnectionFactory` + dialect kaydi** ile yapilir; `#if` yalniz platform API koprogesinde kalir (ADR-001).

| # | Karar | `net48` | `net10.0-windows` |
|---|---|---|---|
| D1 | SQL Server saglayicisi | `Microsoft.Data.SqlClient` **5.2.x** | **ayni 5.2.x** |
| D2 | Dapper | **2.1.x** | **2.1.x** |
| D3 | SQLite | - | `Microsoft.Data.Sqlite` 8.0.x (yalniz test) |
| D4 | SQLite `net48` | **YOK** (kullanilmaz) | - |
| D5 | Sahte veritabani | yok - gercek sema | yok - gercek sema |
| D6 | Sema | saf SQL + surum tablosu + checksum | ayni |

---

## §1 Kod ornegi - saglayici secimi DI ile

```csharp
// Host / DI kaydi (tek nokta)
services.AddSingleton<IDbConnectionFactory>(sp => useSqlite
    ? new SqliteConnectionFactory(sp.GetRequiredService<IOptions<TestDbOptions>>())
    : new SqlServerConnectionFactory(sp.GetRequiredService<IOptions<DbOptions>>()));

// Repository bilmez: iki bacakta ayni kod
using (var connection = _connectionFactory.Create()) { /* ... */ }
```

---

## §2 `#if` disiplini tablosu

| Durum | `#if` serbest mi? | Ne yapilir |
|---|---|---|
| Platform API farki (orn. Windows DPI) | **evet** | `#if NETFRAMEWORK` |
| Dil surumu farki (`record`, `await using`) | hayir | paylasilan kod C# 7.3 kesisimi |
| Saglayici farki | hayir | `IDbConnectionFactory` / dialect |
| Farkli SqlClient surumu (kosullu paket) | hayir | yasak - surum kilidi |
| Test altyapisi | hayir | test projesi ayri TFM |

---

## §3 Paket ve surum tablosu

| Paket | Surum | Nerede | Amaç |
|---|---|---|---|
| `Microsoft.Data.SqlClient` | 5.2.x | iki bacak | uretim veritabani |
| Dapper | 2.1.x | iki bacak | sorgu / esleme |
| `Microsoft.Data.Sqlite` | 8.0.x | yalniz net10 test | test saglayicisi |
| Paket surumu kaynagi | tek dosya | `Directory.Packages.props` | merkezi yonetim |
| Veri erisim yuzeyi | - | ADO.NET + Dapper | ADR-005 |

---

## §4 Dogrulama listesi

| # | Kontrol | Kanit |
|---|---|---|
| 1 | Paket dosyasinda 5.2.x / 2.1.x tek satir | dosya ciktisi |
| 2 | `net48` derlemesi -> 0 hata | derleme ciktisi |
| 3 | `net10.0-windows` derlemesi -> 0 hata | derleme ciktisi |
| 4 | Veri erisim yuzeyi ADO.NET + Dapper | paket taramasi |
| 5 | `Microsoft.Data.Sqlite` `net48` teyidi | **DOĞRULAMA GEREKLI** |

> D4 (`net48`'de SQLite yok) iddiasi derleme ile teyit edilmelidir: **DOĞRULAMA GEREKLI**.

---

## §5 Kardesler

[[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/ado-net-command|ado-net-command]]

---

## §6 Kisa karar

Ayni surum, iki TFM, tek paket satiri. Saglayici degisikligi DI kaydiyla yapilir; `#if` yalniz platform API'si icin kullanilir. Dil farki (`record`, `await using`) paylasilan kodda C# 7.3 kesisimine uyularak cozulur.

---

## §7 Soru - cevap

| Soru | Cevap |
|---|---|
| Surumler neden ayni? | farkli surum CPM kilidini kirar, test uretimi yansitmaz |
| `#if` neden az? | okunabilirlik ve iki bacakta ayni davranis |
| SQLite neden yalniz test? | uretim SQL Server; SQLite test dali hizli ve yalindir |
| Nerede kaydedilir? | ADR-001 (TFM) ve ADR-005 (veri erisim) |
| Proje katmanlari hangi temelde? | ADR-028 (katmanli temel, 9 proje) |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| Paket surumu tek noktada | `Directory.Packages.props` |
| Iki bacak da derleniyor | CI derleme ciktisi |
| `#if` sayisi asgari | kod incelemesi |
| `net48` icin SQLite paketi yok | **DOĞRULAMA GEREKLI** |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `dual-tfm-provider-choice.md` | D1-D6 kararlari, `#if` disiplini, dogrulama listesi, DI ornegi |
| `ado-net-connection.md` | factory ile saglayici soyutlamasi, `await using` kisiti |
| `dapper-procedures-stored.md` | dialect secimi ile `#if` olmayisi |
| `connection-pooling-notes.md` | SqlClient 5.2.x ortak surum, SQLite kilit notu |
| `connection-string-secrets.md` | `ProtectedData` iki bacak farki |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
