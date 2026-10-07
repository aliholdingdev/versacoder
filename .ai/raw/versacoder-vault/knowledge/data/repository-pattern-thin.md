---
title: "repository-pattern-thin - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Ince repository tasarimi: arayuz Application'da, implementasyon Infrastructure'da, SQL yalnizca orada (DA-01 / SOL-05)"
---

# repository-pattern-thin — İnce Repository Kalıbı

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [dapper-query-mapping](dapper-query-mapping.md)

> **Tek cümlelik sorumluluk:** *Repository'nin sınırı nerededir, içinde ne olur ne olmaz, arayüz kime aittir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Repository **ince**dir: tek aggregate/tek sorgu ailesi, metot başına **tek SQL**, Dapper çağrısı ve satır eşlemeden ibarettir. **Arayüz `Application` katmanında** (dependency yönü içeri doğru, SOL-05), **implementasyon `Infrastructure`'ta**; SQL metni hiçbir yerde repository dışında görünmez (DA-01). İş mantığı, yetki, transaction commit'i repository'de **değildir** — bunlar unit-of-work ve servis katmanındadır (SRP).

---

## §1 Arayuz + implementasyon

```csharp
// Application (arayuz) — SQL ICMEZ
public interface ISessionRepository
{
    Task<SessionRow?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SessionRow>> ListRecentAsync(int take, CancellationToken cancellationToken);
}

// Infrastructure (Dapper) — SQL yalnizca burada
public sealed class SessionRepository : ISessionRepository
{
    private const string GetByIdSql =
        "SELECT Id, Title FROM Sessions WHERE Id = @Id AND DeletedAt IS NULL";
    // QueryAsync + DbTimeouts.Query + CancellationToken (DA-03 / DA-09)
}
```

---

## §2 Sinir tablosu

| Repository'de VAR | Repository'de YOK |
|---|---|
| `const string` SQL gövdesi | iş kuralı / hesaplama |
| Parametre bağlama (DA-03) | `Commit()` / transaction sahipliği |
| Satır eşleme (`QueryAsync<T>`) | loglama politikası, UI mesajı |
| `CommandTimeout` + `CancellationToken` | yetki kontrolü (authz) |
| Domain hatasına çevirme | diğer repository'yi çağırmak |

---

## §3 Kural tablosu (DA-01 · DA-05 · SOL-05)

| # | Kural | İhlal belirtisi |
|---|---|---|
| 1 | Tek sorumluluk: tek aggregate ya da okuma modeli | `UtilRepository` 40 metot |
| 2 | Metot = tek sorgu (toplu `IN`/JOIN serbest) | metot içinde döngüyle sorgu (DA-16) |
| 3 | Döner tip `record`/sınıf — `dynamic` yok | `(string)r.Title` dağılımı |
| 4 | Arayüz uygulama katmanında | formda `SessionRepository` new'lenmesi |
| 5 | Generik repository (`IRepo<T>`) **yok** | gereksiz soyutlama, sızan CRUD |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| Arayüz/imzası | `Task<T>` + `CancellationToken` — **aynı** | aynı |
| `record` dönüş tipi | `sealed class` (C# 9 yok) | `record` yazılabilir → ortak dil kesişimi |
| Konstrüktör enjeksiyonu | `IDbConnectionFactory` (aynı) | aynı |
| Mimari test | NetArchTest — yön + döngüsel bağımlılık **0** | aynı test |

> **NOT:** Aynı arayüz iki bacaktaki tek `VersaCoder.Data` projesinden gelir; TFM'e göre **iki implementasyon** yazılmaz (bkz. [dual-tfm-provider-choice](dual-tfm-provider-choice.md)).

---

## §5 Kardeşler

[unit-of-work-pattern](unit-of-work-pattern.md) · [dapper-query-mapping](dapper-query-mapping.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [concurrency-optimistic](concurrency-optimistic.md)
