---
title: "repository-pattern-thin — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Ince repository tasarimi: arayuz Application'da, implementasyon Infrastructure'da, SQL yalnizca orada (DA-01 / SOL-05)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# repository-pattern-thin — İnce Repository Kalıbı

**Zorunlu Baglantilar:** [[index|vault index]] · [[knowledge/index|knowledge index]] · [[.rules/solid-rules|solid rules]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]]

> **Tek cumlelik sorumluluk:** *Repository'nin sınırı nerededir, içinde ne olur ne olmaz, arayüz kime aittir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Repository **incedir**: tek aggregate ya da tek okuma modeli, metot başına **tek SQL**, işi Dapper çağrısı + satır eşlemeden ibarettir. Arayüz `Application` katmanında durur (bağımlılık yönü içeri, SOL-05), implementasyon `Infrastructure`'ta; SQL metni repository dışında hiçbir yerde görünmez (DA-01). İş kuralı, yetki kontrolü ve `Commit()` repository'nin işi değildir — bunlar unit-of-work ve servis katmanındadır (SRP).

## §1 Arayuz + implementasyon

```csharp
// Application — arayuz, SQL ICERMEZ
public interface ISessionRepository
{
    Task<SessionRow?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SessionRow>> ListRecentAsync(int take, CancellationToken cancellationToken);
}

// Infrastructure — SQL yalnizca burada
public sealed class SessionRepository : ISessionRepository
{
    private const string GetByIdSql =
        "SELECT Id, Title FROM Sessions WHERE Id = @Id AND DeletedAt IS NULL";
    // QueryAsync + DbTimeouts.Query + CancellationToken (DA-03 / DA-09)
}
```

Arayüzün imzası iki bacakta aynıdır; implementasyon tek yerde yazılır.

## §2 Sinir tablosu

| Repository'de VAR | Repository'de YOK |
|---|---|
| `const string` SQL gövdesi | iş kuralı / hesaplama |
| Parametre bağlama (DA-03) | `Commit()` / transaction sahipliği |
| Satır eşleme (`QueryAsync<T>`) | loglama politikası, UI mesajı |
| `CommandTimeout` + `CancellationToken` | yetki kontrolü (authz) |
| Domain hatasına çevirme | başka repository'yi çağırmak |

## §3 Kural tablosu (DA-01 · DA-05 · SOL-05)

| # | Kural | İhlal belirtisi |
|---|---|---|
| 1 | Tek sorumluluk: tek aggregate/okuma modeli | `UtilRepository` — 40 metot |
| 2 | Metot = tek sorgu (toplu `IN`/JOIN serbest) | döngü içinde sorgu (DA-16) |
| 3 | Döner tip `record`/sınıf; `dynamic` yok | `(string)r.Title` saçılımı |
| 4 | Arayüz uygulama katmanında | formda `new SessionRepository()` |
| 5 | Generik repository (`IRepo<T>`) yok | soyutlama sızıntısı, CRUD sızması |

## §4 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| Arayüz ve imza | `Task<T>` + `CancellationToken` — aynı | aynı |
| Döner tip | `sealed class` (C# 9 yok) | `record` yazılabilir |
| Konstrüktör enjeksiyonu | `IDbConnectionFactory` — aynı | aynı |
| Mimari test | NetArchTest: yön + döngüsel bağımlılık **0** | aynı test |

Aynı arayüz, iki bacakta tek `VersaCoder.Data` projesinden gelir; TFM başına **iki implementasyon yazılmaz**.

## §5 Sicak bolge

| Belirti | Ne anlama gelir | Düzeltme |
|---|---|---|
| Metot içinde döngüyle sorgu | N+1 (DA-16) | tek `IN`/JOIN sorgusuna çevir |
| `GetAll()` / `Search(filter)` | sızan sorgu dili | somut sorgu metotları |
| Repository'de `if (user.IsAdmin)` | yetki katmanı karıştı | servise taşı |
| Metot gövdesinde `try/catch` + log | hata politikası karıştı | [[.rules/error-handling-rules|error-handling-rules]] |
| 100+ satırlık SQL metni | okumaz hale geldi | parçalı `const` alanlar |

## §6 Sik hatalar

- İş mantığını "repository kolaylığı" diye gömmek
- Generik `IRepository<T>` ile her aggregate'e aynı CRUD'u açmak
- SQL'i `IQueryable`/EF benzeri bir soyut katmana taşımak (ADR-005: **EF Core yok**)
- `Commit()`'i repository metot sonuna koymak
- Arayüzü Infrastructure'a koyup Application'ı oraya bağlamak (yön tersine)

## §7 Dogrulama notlari

"Tek metot = tek sorgu" kuralının toplu güncelleme senaryolarında (batch UPDATE) nasıl istisnaya gireceği vault'ta yazılı değildir — **DOĞRULAMA GEREKLI**. İstisna kabul edilirse yine tek metot, tek SQL ilkesinde kalınır.

## §8 Kardesler

[[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]]

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `repository-pattern-thin.md` (kaynak) | arayüz/implementasyon örneği, sınır tablosu, DA-01/DA-05/SOL-05 kuralları, TFM farkları |
| `unit-of-work-pattern.md` | `Commit()`'in repository'de olmaması |
| `dapper-query-mapping.md` | metot gövdesindeki Dapper çağrısı ve satır tipi |
| `parameterized-queries.md` | gövdenin `const string`, değerin `@ad` olması |
| `dual-tfm-provider-choice.md` | tek implementasyon, iki bacak kararı |
