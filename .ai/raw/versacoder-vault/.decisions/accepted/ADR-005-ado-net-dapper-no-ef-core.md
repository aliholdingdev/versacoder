---
title: "ADR-005 — Veri Erişimi: ADO.NET + Dapper, EF Core YOK"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "ORM yasağı, parameterized SQL zorunluluğu, provider abstraction (Sqlite + SqlClient 5.x), migration = saf SQL + schema_version, secret yönetimi"
---

# ADR-005 — ADO.NET + Dapper (EF Core YOK)

**Durum:** accepted · **Tarih:** 2026-09-25 · **Karar sahibi:** `architect` · **Hedef bacak:** `net48 + net10.0-windows`

**İlgili:** [[../../index.md]] · [[../../CLAUDE.md]] · [[../../reports/01-web-kaynaklari.md]] · [[ADR-001-dual-target-net48-net10.md]] · [[ADR-004-devexpress-universal-v261-office2010.md]]

---

## Context

Prompt §2.5: EF Core YOK; ADO.NET + Dapper; her sorgu parameterized; `Microsoft.Data.Sqlite` + `Microsoft.Data.SqlClient` provider abstraction üzerinden; migration = saf SQL + version tablosu. ai.01'deki EF Core 9.0.0 **REDDEDİLDİ** (R3). Web: Dapper **2.1.89** güncel; **SqlClient 6.0 .NET Standard'ı düşürdü** → net48'de 5.x zorunlu.

## Decision

1. **ORM yok.** EF Core / NHibernate / LiteDB referans edilemez. Veri katmanı = `DbConnection` + **Dapper**.
2. **Parameterized zorunlu.** Tüm parametreler `@ad`; string-concat SQL = `G2` ihlali → `reviewer` RED. Her komutta `CommandTimeout` açıkça set edilir.
3. **Provider abstraction:** `ISqlConnectionFactory` + `IDbExecutor` (L4.1). Implementasyonlar: `SqliteProvider` (`Microsoft.Data.Sqlite`), `SqlServerProvider` (**`Microsoft.Data.SqlClient` 5.x — iki bacakta da 5.x**).
4. **Migration = saf SQL:** `.sql/migrations/NNN-*.sql` + `schema_version (version, applied_at, checksum)` + `IMigrationRunner`.
5. **Secret:** connection string düz metin durmaz → DPAPI (`ProtectedData`) ile şifreli saklama; `keys.md` yalnız anahtar kelime haritasıdır.

## Alternatifler

| # | Alternatif | + | − | Neden reddedildi |
|---|---|---|---|---|
| A | **Seçilen:** ADO.NET + Dapper | İki bacakta tek API; SQL görünür | Elle mapping/migration | — |
| B | EF Core 9 | Migration + LINQ | `net48` ile uyumsuz (modern .NET ister) | Teknik engel + yasak |
| C | EF Core 8 + 6.x karması | Kısmen iki bacak | Sürüm kırığı, DbContext sızıntısı | G2 + sürüm karması |
| D | Raw ADO.NET (Dapper'sız) | Bağımlılık sıfır | Elle parametre tekrarı, n-hata riski | Gereksiz emek |

## Consequences

- **(+)** İki bacakta aynı veri kodu; testler SQLite gerçek şema üzerinde; güvenlik denetimi tek noktada (`IDbExecutor`); küçük paket yüzeyi.
- **(−)** Mapping/migration elle; net48 SqlClient 5.x'e kilitli (6.0 = ayrı ADR).

## Web kanıtı (erişim 2026-09-25)

| Bulgu | Kaynak |
|---|---|
| Dapper 2.1.89 — ADO.NET micro-ORM | `nuget.org/packages/dapper` |
| SqlClient 6.0 .NET Standard desteğini düşürdü | `github.com/dotnet/SqlClient/discussions/3115` |
| `Microsoft.Data.Sqlite` net48+net10 ikili | **`NEEDS_VALIDATION`** — PART 5 NuGet teyidi |

## Kabul kriterleri

- [ ] Paket referanslarında **EF Core = 0**
- [ ] Kod taraması: `DbContext|FromSql|SqlDataAdapter` = 0; SQL birleşim taraması = 0
- [ ] Her `DbCommand` → `CommandTimeout` setli
- [ ] `dotnet test` entegrasyon = yeşil (GATE-005)
