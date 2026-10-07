---
title: "data - VersaCoder"
type: index
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "knowledge/data altindaki 25 dosyanin kayit defteri: ADO.NET + Dapper, parameterized SQL, migration, provider, secret, timeout"
---

# Veri Erisimi Bilgi Bankasi (`knowledge/data/`)

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md)

> **Tek cumlelik sorumluluk:** *Veri erisimi konusunda hangi dosya hangi soruyu cevaplar* sorusunun tek cevabidir.
> Karar `.decisions/`'te (ADR-005 · ADR-012), kural `.rules/data-access-rules.md`'dedir; bu klasor **bilgi** verir (SRP).

---

## §0 Ozet (Answer First)

25 dosya, 4 başlıkta: **ADO.NET + Dapper çekirdeği**, **politika/sır/timeout**, **sağlayıcı kurulumu (SqlClient 5.2.x / Sqlite)**, **migration + ince repository kalıpları**. Her dosya 3-8 KB, gerçek C# örneği + kural tablosu + `net48 / net10.0-windows` farkı notu taşır. Okuma sırası: kural → karar → bilgi.

---

## §1 Dosya kayit defteri

| # | Dosya | Tek cevap |
|---|---|---|
| 1 | [ado-net-connection](ado-net-connection.md) | `DbConnection` nereden gelir, ne zaman kapanır |
| 2 | [ado-net-command](ado-net-command.md) | `DbCommand` kurulumu ve `Execute*` seçimi |
| 3 | [parameterized-queries](parameterized-queries.md) | `@ad` zorunluluğu, IN/LIKE/ORDER BY kalıpları |
| 4 | [dapper-query-mapping](dapper-query-mapping.md) | satır → tip eşlemesi, sessiz `default` tuzağı |
| 5 | [dapper-parameters-dynamic](dapper-parameters-dynamic.md) | anonim nesne vs `DynamicParameters` |
| 6 | [dapper-multi-map](dapper-multi-map.md) | `splitOn`, `QueryMultiple`, N+1 yerine JOIN |
| 7 | [dapper-async-patterns](dapper-async-patterns.md) | async zincir, iptal, deadlock kaçınma |
| 8 | [dapper-procedures-stored](dapper-procedures-stored.md) | prosedür politikası ve çağrım deseni |
| 9 | [transaction-scope-local](transaction-scope-local.md) | tek transaction, rollback, `TransactionScope` yasağı |
| 10 | [connection-string-secrets](connection-string-secrets.md) | bağlantı metni DPAPI ile nasıl saklanır |
| 11 | [command-timeout-policy](command-timeout-policy.md) | `DbTimeouts` 15/60/300, `0` yasak |
| 12 | [sqlserver-provider-setup](sqlserver-provider-setup.md) | SqlClient 5.2.x + CPM kurulumu |
| 13 | [sqlite-provider-setup](sqlite-provider-setup.md) | PRAGMA seti, yalnız `net10` testi |
| 14 | [dual-tfm-provider-choice](dual-tfm-provider-choice.md) | iki bacakta hangi paket, `#if` disiplini |
| 15 | [migration-version-table](migration-version-table.md) | `__Migrations` + SHA-256 fail-fast |
| 16 | [migration-script-writing](migration-script-writing.md) | `V0001__ad.sql`, idempotent `up` |
| 17 | [migration-rollback](migration-rollback.md) | `down` / `-- irreversible`, onay + yedek |
| 18 | [repository-pattern-thin](repository-pattern-thin.md) | ince repository sınırı, SQL yalnız orada |
| 19 | [unit-of-work-pattern](unit-of-work-pattern.md) | tek bağlantı + tek commit |
| 20 | [connection-pooling-notes](connection-pooling-notes.md) | havuz davranışı ve tükenme belirtileri |
| 21 | [null-handling-db-mapping](null-handling-db-mapping.md) | `NULL` → nullable tip, `default` tuzağı |
| 22 | [pagination-offset-keyset](pagination-offset-keyset.md) | OFFSET vs keyset, sayfa tavanı |
| 23 | [concurrency-optimistic](concurrency-optimistic.md) | `Version` + etkilenen satır = 0 |
| 24 | [audit-columns-convention](audit-columns-convention.md) | UTC audit kolonları, soft delete |
| 25 | [sql-injection-defense](sql-injection-defense.md) | injection kapıları ve allowlist |

---

## §2 Okuma sirasi

| Amaç | Önce | Sonra |
|---|---|---|
| Kuralları öğren | [data-access-rules](../../.rules/data-access-rules.md) | ilgili dosya |
| Kod yazmak | [repository-pattern-thin](repository-pattern-thin.md) | [parameterized-queries](parameterized-queries.md) |
| Şema/ortam | [migration-version-table](migration-version-table.md) | sağlayıcı dosyaları |
| İnceleme (review) | [sql-injection-defense](sql-injection-defense.md) | [command-timeout-policy](command-timeout-policy.md) |

---

## §3 Kardesler

[knowledge index](../index.md) · [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md)
