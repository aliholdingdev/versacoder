---
title: "Veri Erişimi Kuralları (G2)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADO.NET + Dapper; EF Core yok; parametreli SQL; DbConnection yalnız l4"
tags: [rules, data, dapper, sql, g2]
source: "Birleştirme: .ai/CLAUDE.md G2 + brain.md §3/5 + ADR-005/016/017"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

# Veri Erişimi Kuralları (G2)

## G2 — ADO.NET + Dapper; EF Core yok

- Veri erişimi **ADO.NET + Dapper**; ORM (EF Core, NHibernate vb.) **yasak**.
- `using Microsoft.EntityFrameworkCore` = otomatik **RED**.
- Migration = **saf SQL** dosyaları + `schema_version` tablosu; ORM migration yok.
- **DbConnection yalnız `l4 ado-dapper`** katmanında yaşar (ADR-016); üst
  katmanlar yalnız `l0` portuna (`IRowWriter`/`IDbExecutor`) bakar.

## Parametreli SQL zorunlu

```text
KOTU : var sql = "SELECT * FROM T WHERE Id = '" + id + "'";   -> RED
IYI  : var sql = "SELECT * FROM T WHERE Id = @p1";
       conn.Query<T>(sql, new { p1 = id });
```

- `@p1..@pN`; string-concat SQL = kod incelemesinde **otomatik RED** (ADR-017).
- Her komutta `CommandTimeout` açık set edilir.

## İhlalde

CRITICAL — build RED; kod durur, ADO.NET + Dapper'a çevrilir.

## Kanıt

- EF taraması = 0 satır
- concat taraması = 0 satır
- `sql-injection-review` checklist'i yeşil

## İlgili

- [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]]
- [[.decisions/accepted/adr-016-dbconnection-only-l4|ADR-016]]
- [[.decisions/accepted/adr-017-parameterized-sql-only|ADR-017]]
