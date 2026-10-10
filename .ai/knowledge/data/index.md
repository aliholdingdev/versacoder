---
title: "data - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: reference
scope: "knowledge/data altindaki 25 dosyanin kayit defteri: ADO.NET + Dapper, parameterized SQL, migration, provider, secret, timeout"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Veri Erisimi Bilgi Bankasi (knowledge/data/)

**Zorunlu Baglantilar:** [[index|vault index]] · [[knowledge/index|knowledge index]] · [[.rules/data-access-rules|data-access-rules]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.decisions/accepted/adr-012-dual-tfm-data-provider|ADR-012]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]

> **Tek cumlelik sorumluluk:** *Veri erisimi konusunda hangi dosya hangi soruyu cevaplar* sorusunun tek cevabidir.
> Karar `.decisions/`'te (ADR-005 · ADR-012), kural `.rules/data-access-rules.md`'dedir; bu klasor **bilgi** verir (SRP).

---

## §0 Ozet (Answer First)

25 dosya, 4 baslikta: **ADO.NET + Dapper cekirdegi**, **politika/sir/timeout**, **saglayici kurulumu (SqlClient / Sqlite)**, **migration + ince repository kaliplari**. Her dosya 3-8 KB, gercek C# ornegi + kural tablosu + `net48 / net10.0-windows` farki notu tasir. Okuma sirasi: kural -> karar -> bilgi.

```text
+---------------------------------------------------------------+
| kural (.rules/data-access-rules.md)  ->  karar (ADR-005/012)  |
|                      ->  bilgi (bu klasor, 25 dosya)          |
| EF Core YASAK; string-concat SQL YASAK; @p1 parametre         |
+---------------------------------------------------------------+
```

---

## §1 Dosya kayit defteri

| # | Dosya | Tek cevap |
|---|---|---|
| 1 | [[knowledge/data/ado-net-connection|ado-net-connection]] | `DbConnection` nereden gelir, ne zaman kapanir |
| 2 | [[knowledge/data/ado-net-command|ado-net-command]] | `DbCommand` kurulumu ve `Execute*` secimi |
| 3 | [[knowledge/data/parameterized-queries|parameterized-queries]] | `@ad` zorunlulugu, IN/LIKE/ORDER BY kaliplari |
| 4 | [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] | satir -> tip eslemesi, sessiz `default` tuzaqi |
| 5 | [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] | anonim nesne vs `DynamicParameters` |
| 6 | [[knowledge/data/dapper-multi-map|dapper-multi-map]] | `splitOn`, `QueryMultiple`, N+1 yerine JOIN |
| 7 | [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] | async zincir, iptal, deadlock kacinma |
| 8 | [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] | prosedur politikasi ve cagrim deseni |
| 9 | [[knowledge/data/transaction-scope-local|transaction-scope-local]] | tek transaction, rollback, `TransactionScope` yasagi |
| 10 | [[knowledge/data/connection-string-secrets|connection-string-secrets]] | baglanti metni DPAPI ile nasil saklanir |
| 11 | [[knowledge/data/command-timeout-policy|command-timeout-policy]] | `DbTimeouts` 15/60/300, `0` yasak |
| 12 | [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] | SqlClient + CPM kurulumu |
| 13 | [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] | PRAGMA seti, yalniz `net10` testi |
| 14 | [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] | iki bacakta hangi paket, `#if` disiplini |
| 15 | [[knowledge/data/migration-version-table|migration-version-table]] | `__Migrations` + SHA-256 fail-fast |
| 16 | [[knowledge/data/migration-script-writing|migration-script-writing]] | `V0001__ad.sql`, idempotent `up` |
| 17 | [[knowledge/data/migration-rollback|migration-rollback]] | `down` / `-- irreversible`, onay + yedek |
| 18 | [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] | ince repository siniri, SQL yalniz orada |
| 19 | [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]] | tek baglanti + tek commit |
| 20 | [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] | havuz davranisi ve tukenme belirtileri |
| 21 | [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] | `NULL` -> nullable tip, `default` tuzaqi |
| 22 | [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] | OFFSET vs keyset, sayfa tavani |
| 23 | [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] | `Version` + etkilenen satir = 0 |
| 24 | [[knowledge/data/audit-columns-convention|audit-columns-convention]] | UTC audit kolonlari, soft delete |
| 25 | [[knowledge/data/sql-injection-defense|sql-injection-defense]] | injection kapilari ve allowlist |

---

## §2 Okuma sirasi

| Amac | Once | Sonra |
|---|---|---|
| Kurallari ogren | [[.rules/data-access-rules|data-access-rules]] | ilgili dosya |
| Kod yazmak | [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] | [[knowledge/data/parameterized-queries|parameterized-queries]] |
| Sema/ortam | [[knowledge/data/migration-version-table|migration-version-table]] | saglayici dosyalari |
| Inceleme (review) | [[knowledge/data/sql-injection-defense|sql-injection-defense]] | [[knowledge/data/command-timeout-policy|command-timeout-policy]] |

---

## §3 Kardesler

| Konu | Baglanti |
|---|---|
| Bilgi indeksi | [[knowledge/index|knowledge index]] |
| Vault girisi | [[index|vault index]] |
| Kural | [[.rules/data-access-rules|data-access-rules]] |
| Karar | [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\data\index.md` (kaynak vault) | 25 dosyalik kayit defteri, okuma sirasi, kardes baglantilari |
| `knowledge\data\` (hedef vault) | 25 dosyanin varligi disktan dogrulandi |
| `.rules\data-access-rules.md` | kural katmani |
| `.decisions\accepted\ADR-005-ado-net-dapper-no-ef-core.md` | karar katmani |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dizinde her satirdaki dosya disktaki varligiyla eslestirilmistir.
