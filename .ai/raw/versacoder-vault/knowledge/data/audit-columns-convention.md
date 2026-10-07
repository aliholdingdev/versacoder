---
title: "audit-columns-convention - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Audit kolonlari: CreatedAt/UpdatedAt/DeletedAt (UTC, ISO-8601), soft delete, TimeProvider tek kaynagi (DA-15)"
---

# audit-columns-convention — Audit Kolon Sözleşmesi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [data index](../index.md) · [migration-script-writing](migration-script-writing.md)

> **Tek cümlelik sorumluluk:** *Hangi tabloda hangi denetim kolonu bulunur, zaman nereden gelir, soft delete nasıl yazılır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Kritik tablolarda üç kolon: **`CreatedAt`**, **`UpdatedAt`** (ve gerekiyorsa **`DeletedAt`**) — hepsi **UTC, ISO-8601** (`2026-09-25T10:30:00Z`) biçiminde. Zaman **tek kaynaktan** gelir: `TimeProvider` (veya vault'taki saat soyutlaması) — `DateTime.Now`/`DateTime.UtcNow` doğrudan çağrılmaz (DA-15). Silme **yıkıcı değil, soft**'tur: `DeletedAt` doldurulur ve tüm sorgular `DeletedAt IS NULL` koşulu taşır; kalıcı silme yalnız açık onayla (G7).

---

## §1 Kolon sozlesmesi

```csharp
var now = _timeProvider.GetUtcNow().UtcDateTime;      // TEK KAYNAK (DA-15)

const string insertSql = @"
    INSERT INTO Sessions (Id, Title, CreatedAt, UpdatedAt)
    VALUES (@Id, @Title, @CreatedAt, @UpdatedAt)";

await connection.ExecuteAsync(insertSql, new
{
    Id = id, Title = title,
    CreatedAt = now, UpdatedAt = now
}, commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §2 Tablo / kolon matrisi

| Kolon | Tip | Zorunlu | Dolduran |
|---|---|---|---|
| `Id` | `TEXT` (GUID) / `uniqueidentifier` | evet | uygulama |
| `CreatedAt` | ISO-8601 UTC | evet | INSERT |
| `UpdatedAt` | ISO-8601 UTC | evet | her UPDATE |
| `DeletedAt` | ISO-8601 UTC, `NULL` | soft delete tabloları | DELETE yerine UPDATE |
| `CreatedBy` / `UpdatedBy` | `TEXT`/`UNIQUEIDENTIFIER` | gerekiyorsa | kimlik bağlamı |
| `Version` | `INTEGER` | çakışma korumalı tablolar | UPDATE (`+1`) |

---

## §3 Kural tablosu (DA-15)

| # | Kural | Yasak |
|---|---|---|
| 1 | Saat = `TimeProvider` (tek kaynak) | `DateTime.Now` / `DateTime.UtcNow` |
| 2 | Biçim ISO-8601, **UTC** (`Z`) | yerel saat, `dd.MM.yyyy` |
| 3 | `UpdatedAt` her UPDATE'te | güncellenmeyen satır |
| 4 | Soft delete + `DeletedAt IS NULL` filtresi | doğrudan `DELETE` |
| 5 | Fiziksel silme onaylı (G7) + yedek | onaysız `DELETE FROM` |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `TimeProvider` | BCL'de **yok** → vault'ta soyut arayüz (`IClock`) ya da paket | .NET 8+ **yerleşik** `TimeProvider` |
| Ortak kod | arayüz iki bacakta ortak (`IClock`) — `#if` yok | aynı arayüz |
| Tarih saklama | SQLite `TEXT` (test) / SQL Server `datetime2` (üretim) | aynı sözlük |
| `DateTimeKind` | `Utc` korunur (`ToUniversalTime` değil) | aynı |

> **NOT:** `net48`'de `TimeProvider` bulunmadığı için **tek ortak arayüz** (`IClock.GetUtcNow()`) tanımlanır; tek uygulama, `#if` gerekmez (ADR-001).

---

## §5 Kardeşler

[null-handling-db-mapping](null-handling-db-mapping.md) · [migration-script-writing](migration-script-writing.md) · [concurrency-optimistic](concurrency-optimistic.md) · [repository-pattern-thin](repository-pattern-thin.md) · [transaction-scope-local](transaction-scope-local.md)
