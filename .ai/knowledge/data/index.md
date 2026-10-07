---
title: "data - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "knowledge/data altindaki 25 dosyanin kayit defteri: ADO.NET + Dapper, parameterized SQL, migration, provider, secret, timeout"
---

# Veri Erisimi Bilgi Bankasi (knowledge/data/)

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md)
**Küme + Hub:** [data index](index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)

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
| 1 | [ado-net-connection](ado-net-connection.md) | `DbConnection` nereden gelir, ne zaman kapanir |
| 2 | [ado-net-command](ado-net-command.md) | `DbCommand` kurulumu ve `Execute*` secimi |
| 3 | [parameterized-queries](parameterized-queries.md) | `@ad` zorunlulugu, IN/LIKE/ORDER BY kaliplari |
| 4 | [dapper-query-mapping](dapper-query-mapping.md) | satir -> tip eslemesi, sessiz `default` tuzaqi |
| 5 | [dapper-parameters-dynamic](dapper-parameters-dynamic.md) | anonim nesne vs `DynamicParameters` |
| 6 | [dapper-multi-map](dapper-multi-map.md) | `splitOn`, `QueryMultiple`, N+1 yerine JOIN |
| 7 | [dapper-async-patterns](dapper-async-patterns.md) | async zincir, iptal, deadlock kacinma |
| 8 | [dapper-procedures-stored](dapper-procedures-stored.md) | prosedur politikasi ve cagrim deseni |
| 9 | [transaction-scope-local](transaction-scope-local.md) | tek transaction, rollback, `TransactionScope` yasagi |
| 10 | [connection-string-secrets](connection-string-secrets.md) | baglanti metni DPAPI ile nasil saklanir |
| 11 | [command-timeout-policy](command-timeout-policy.md) | `DbTimeouts` 15/60/300, `0` yasak |
| 12 | [sqlserver-provider-setup](sqlserver-provider-setup.md) | SqlClient + CPM kurulumu |
| 13 | [sqlite-provider-setup](sqlite-provider-setup.md) | PRAGMA seti, yalniz `net10` testi |
| 14 | [dual-tfm-provider-choice](dual-tfm-provider-choice.md) | iki bacakta hangi paket, `#if` disiplini |
| 15 | [migration-version-table](migration-version-table.md) | `__Migrations` + SHA-256 fail-fast |
| 16 | [migration-script-writing](migration-script-writing.md) | `V0001__ad.sql`, idempotent `up` |
| 17 | [migration-rollback](migration-rollback.md) | `down` / `-- irreversible`, onay + yedek |
| 18 | [repository-pattern-thin](repository-pattern-thin.md) | ince repository siniri, SQL yalniz orada |
| 19 | [unit-of-work-pattern](unit-of-work-pattern.md) | tek baglanti + tek commit |
| 20 | [connection-pooling-notes](connection-pooling-notes.md) | havuz davranisi ve tukenme belirtileri |
| 21 | [null-handling-db-mapping](null-handling-db-mapping.md) | `NULL` -> nullable tip, `default` tuzaqi |
| 22 | [pagination-offset-keyset](pagination-offset-keyset.md) | OFFSET vs keyset, sayfa tavani |
| 23 | [concurrency-optimistic](concurrency-optimistic.md) | `Version` + etkilenen satir = 0 |
| 24 | [audit-columns-convention](audit-columns-convention.md) | UTC audit kolonlari, soft delete |
| 25 | [sql-injection-defense](sql-injection-defense.md) | injection kapilari ve allowlist |

---

## §2 Okuma sirasi

| Amac | Once | Sonra |
|---|---|---|
| Kurallari ogren | [data-access-rules](../../.rules/data-access-rules.md) | ilgili dosya |
| Kod yazmak | [repository-pattern-thin](repository-pattern-thin.md) | [parameterized-queries](parameterized-queries.md) |
| Sema/ortam | [migration-version-table](migration-version-table.md) | saglayici dosyalari |
| Inceleme (review) | [sql-injection-defense](sql-injection-defense.md) | [command-timeout-policy](command-timeout-policy.md) |

---

## §3 Kardesler

| Konu | Baglanti |
|---|---|
| Bilgi indeksi | [knowledge index](../index.md) |
| Vault girisi | [vault index](../../index.md) |
| Kural | [data-access-rules](../../.rules/data-access-rules.md) |
| Karar | [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\data\index.md` (kaynak vault) | 25 dosyalik kayit defteri, okuma sirasi, kardes baglantilari |
| `knowledge\data\` (hedef vault) | 25 dosyanin varligi disktan dogrulandi |
| `.rules\data-access-rules.md` | kural katmani |
| `.decisions\accepted\ADR-005-ado-net-dapper-no-ef-core.md` | karar katmani |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dizinde her satirdaki dosya disktaki varligiyla eslestirilmistir.
