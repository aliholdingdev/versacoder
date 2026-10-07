---
title: "dapper-query-mapping - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Dapper ile satir esleme: QueryAsync<T>, record satir tipleri, kolon-ozellik uyumu, custom TypeHandler (DA-05 / DA-08)"
---

# dapper-query-mapping — Dapper Satır Eşleme

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [parameterized-queries](parameterized-queries.md)

> **Tek cumlelik sorumluluk:** *Sorgu satırı hangi tüpe, hangi kuralla eşlenir; sessiz eşleşme hataları nasıl önlenir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`QueryAsync<T>` çağrılır, kolon adı **büyük/küçük harf duyarsız** olarak `T`'nin özelliğine bağlanır; eşleşmeyen kolon sessizce **atlanır**, eşleşmeyen özellik **default**'ta kalır → bu sessizlik DA-08'in denetlediği tehlikedir. Satır tipleri `dynamic` değil, `record`/sınıf yazılır (DA-05). Veritabanı tipi ile C# tipi arasındaki köprü (`DateTime`↔`string` ISO-8601, `bool`↔`int`) **dialect/factory** katmanında ya da `TypeHandler` ile tek yerde çözülür.

---

## §1 Dogru kullanim

```csharp
public sealed record SessionRow(Guid Id, string Title, DateTime CreatedAt, bool IsArchived);

const string sql = "SELECT Id, Title, CreatedAt, IsArchived FROM Sessions WHERE Id = @Id";

var row = await connection.QuerySingleOrDefaultAsync<SessionRow>(sql, new { Id = id },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);

// Eksik kolon / bos satir kontrolu default degerle gizlenmez
if (row is null) return SessionNotFound.Instance;
```

---

## §2 Esleme kurallari tablosu

| Durum | Dapper davranışı | Kural |
|---|---|---|
| `Id` kolonu → `Id` özelliği | büyük/küçük harf duyarsız eşleşir | kolon adını olduğu gibi yaz |
| Eşleşmeyen kolon | sessizce yok sayılır | SELECT listesi ile satır tipi hizalı olmalı |
| Eşleşmeyen özellik | `default` kalır | `default` anlamı olan tipte (`int`) dikkat |
| `NULL` kolon | `null` (nullable tipte) | `string?`, `DateTime?` — `DA-08` |
| Constructor'lı `record` | constructor parametreleriyle eşler | parametre adları kolonla aynı olmalı |
| `enum` kolon | sayı okunur | `TypeHandler` ile isim/dönüşüm tek yerde |
| Çoklu result-set | `QueryMultiple` | bkz. [dapper-multi-map](dapper-multi-map.md) |

---

## §3 Kural tablosu (DA-05 · DA-08)

| # | Kural | İhlal belirtisi |
|---|---|---|
| 1 | `Query<dynamic>` kod boyunca **0** | her yerde `(string)row.Title` |
| 2 | Satır tipi `record` — `Infrastructure` içinde | anonim tip servise taşınmak |
| 3 | `SELECT` listesi = satır tipi özellikleri | sessiz `default` veri |
| 4 | Null, gerçek `?` tipi (`DateTime?`) | `0001-01-01` tarihleri raporda |
| 5 | Eşleme **testle** kanıtlanır | gerçek şema üzerinde 1 entegrasyon testi |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| `record` | C# 9 — paylaşılan satır tipi için **`sealed class` + constructor** yaz (bkz. [null-handling-db-mapping](null-handling-db-mapping.md)) | `record` yazılabilir |
| `init`/`required` | yok | var |
| Nullable anotasyonu | proje kapalı olabilir (`string?` uyarısı yok) | `#nullable enable` |
| Dapper sürümü | **2.1.x** | **2.1.x** — aynı eşleme motoru |

> **KURAL:** Satır tipi sözdizimi iki bacakta da derlenir olmalı; `record` kullanılmak isteniyorsa ortak dil sürümü ADR-001 kapsamında `architect` kararıdır (**NEEDS_VALIDATION**).

---

## §5 Kardesler

[null-handling-db-mapping](null-handling-db-mapping.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [repository-pattern-thin](repository-pattern-thin.md)
