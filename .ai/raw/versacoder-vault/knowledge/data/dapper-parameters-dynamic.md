---
title: "dapper-parameters-dynamic - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Dapper parametre yazimi: anonim nesne vs DynamicParameters, cikti parametresi, null/DBNull, IN listesi genisletme (DA-03)"
---

# dapper-parameters-dynamic — Dapper Parametre Yazımı

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [parameterized-queries](parameterized-queries.md)

> **Tek cumlelik sorumluluk:** *Değer hangi nesneyle, hangi kuralla parametrelenir; çıkış parametresi ve `null` nasıl yazılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Sabit set için **anonim nesne** (`new { Ad = x }`), koşullu/çıkış parametresi için **`DynamicParameters`**. Parametre adı `@` **yazılmaz** (Dapper ekler), ad ile SQL'deki `@Ad` birebir aynıdır. `null` → `DBNull.Value` dönüşümünü Dapper yapar; SQLite'ta tip yönü (`TEXT`/`INTEGER`) sağlayıcıya bırakılır, kodda `DBNull` elle karıştırılmaz. `IN` listesi tek parametre adıyla yazılır, Dapper `@Ids`'i satır satır açar.

---

## §1 Dogru kullanim

```csharp
var p = new DynamicParameters();
p.Add("Title", title);                          // @ eklenmez
p.Add("Id", id);
p.Add("Limit", DbTimeouts.Query, DbType.Int32); // acik tip (guiçlu yollar icin)
if (!string.IsNullOrWhiteSpace(tag))
    p.Add("Tag", tag);                          // kosullu parametre

const string sql = @"
    UPDATE Sessions SET Title = @Title, UpdatedAt = @UpdatedAt
    WHERE Id = @Id AND DeletedAt IS NULL";

var affected = await connection.ExecuteAsync(sql, p,
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §2 Parametre secim tablosu

| Durum | Araç | Not |
|---|---|---|
| Hepsi sabit (3-5 adet) | anonim nesne | en kısa, en okunur |
| Koşullu parametre | `DynamicParameters` | eklenen yalnızca çağrıda |
| Çıkış / dönüş değeri | `DynamicParameters.Add(..., ParameterDirection.Output)` | `Get<T>("Ad")` |
| `IN` listesi | `p.Add("Ids", idList)` | Dapper `@Ids`'i genişletir |
| `NULL` olabilir değer | `p.Add("X", maybeNull)` | Dapper `DBNull` yazar |
| Tip/büyüklük zorunlu | `DbType` + `Size` (yalnız SQL Server) | SQLite dalında gereksiz |

---

## §3 Kural tablosu (DA-03)

| # | Kural | Yasak |
|---|---|---|
| 1 | Ad = SQL'deki `@Ad` (noktalanmış `p.Ad` değil) | `@p1` ↔ `"Id"` uyuşmazlığı |
| 2 | Değer **asla** metne girmez | parametreyi `"` + ile birleştirmek |
| 3 | Liste parametresi tek adla | döngüde `"@Id0,@Id1"` yazmak |
| 4 | `dynamic` yerine tip satırı (DA-05) | `row.Id` üzerinde çalışmak |
| 5 | `commandTimeout` her çağrıda | timeout'suz çağrı |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `DynamicParameters` | Dapper **2.1.x** — aynı | aynı |
| `DbType` tanımı | `System.Data.DbType` | aynı |
| `ValueTuple`/`record` parametre | `record` C# 9 — yok | var |
| Parametre davranışı | **fark yok** — tek kod yolu | aynı |

> **NOT:** Parametre katmanı sağlayıcıdan bağımsızdır; `@` ayraçlı iki sağlayıcıda da geçerli (bkz. [dual-tfm-provider-choice](dual-tfm-provider-choice.md)).

---

## §5 Kardesler

[parameterized-queries](parameterized-queries.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-async-patterns](dapper-async-patterns.md) · [sql-injection-defense](sql-injection-defense.md) · [null-handling-db-mapping](null-handling-db-mapping.md)
