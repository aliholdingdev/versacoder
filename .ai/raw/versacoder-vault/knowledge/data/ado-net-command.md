---
title: "ado-net-command - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DbCommand olusturma, calistirma yontemleri, parametre ekleme ve CommandTimeout - Dapper disi ham ADO.NET yolu"
---

# ado-net-command — ADO.NET Komut Çalıştırma

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [parameterized-queries](parameterized-queries.md)

> **Tek cumlelik sorumluluk:** *Bir `DbCommand` nasıl kurulur, hangi `Execute*` hangi işte kullanılır, timeout nereye yazılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Komut ya Dapper (`QueryAsync`/`ExecuteAsync`) ya da ham `DbCommand` ile çalıştırılır; **ikisinde de** gövde sabit metin, değerler parametre, `CommandTimeout` açık. `DbCommand`, sahibi olan bağlantının `CreateCommand()`'ından gelir — `new SqlCommand(...)` repository'de **yasak**. İade değeri yoksa `ExecuteNonQuery`, tek değer `ExecuteScalar`, satır akışı `ExecuteReader`; Dapper bu üçünü sarar ve elle yazmayı gereksiz kılar.

---

## §1 Dogru kullanim (ham ADO.NET)

```csharp
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);
    using (var cmd = connection.CreateCommand())
    {
        cmd.CommandText = "UPDATE Sessions SET Title = @Title WHERE Id = @Id";
        cmd.CommandTimeout = DbTimeouts.Query;            // DA-09: 0 yasak
        var pTitle = cmd.CreateParameter();
        pTitle.ParameterName = "@Title"; pTitle.Value = title;
        cmd.Parameters.Add(pTitle);
        var pId = cmd.CreateParameter();
        pId.ParameterName = "@Id"; pId.Value = id;
        cmd.Parameters.Add(pId);
        var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
```

---

## §2 Execute yontemi secimi

| İhtiyaç | Yöntem | Dapper karşılığı |
|---|---|---|
| Satır dönmüyor (UPDATE/DELETE) | `ExecuteNonQueryAsync` | `ExecuteAsync` |
| Tek skaler değer (COUNT, MAX) | `ExecuteScalarAsync` | `ExecuteScalarAsync<T>` |
| Birden çok satır | `ExecuteReaderAsync` | `QueryAsync<T>` |
| Birden çok result-set | `NextResult` | `QueryMultiple` |
| Depolama prosedürü | `CommandType.StoredProcedure` | `CommandDefinition` + `CommandType` |

---

## §3 Kural tablosu (DA-03 · DA-06 · DA-09)

| # | Kural | Değer | İhlal sonucu |
|---|---|---|---|
| 1 | `CommandText` | `const string` / sabit alan (DA-06) | plan yeniden kullanılmaz, denetim zor |
| 2 | Değer | **hiç** metne girmez, `@ad` parametresi (DA-03) | G2 ihlali → `reviewer` RED |
| 3 | `CommandTimeout` | `DbTimeouts.Query` 15 · `Report` 60 · `Migration` 300 | `0` = sınırsız = yasak |
| 4 | `CommandType` | `Text` sabit; `StoredProcedure` yalnız kasıtlı | karışık kullanım |
| 5 | Ömür | `using` içinde, bağlantı kapanınca yok | sızan `DbCommand` |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Async API | `ExecuteReaderAsync` vb. var (4.5+) | aynı imza |
| `await using` | yok (C# 8) — klasik `using (…) { }` | yazılabilir |
| `SqlParameter` tipi | `Microsoft.Data.SqlClient` 5.2.x | aynı paket 5.2.x (ADR-012 D1) |
| `CreateParameter()` | sağlayıcıya özgü tip gerekmeden güvenli yol | aynı |

> **NOT:** Sağlayıcıya özgü parametre boyu/tipi (`SqlParameter.Size`, `SqlDbType`) yalnız SQL Server dalında; SQLite dalı aynı repository kodunu çalıştırmalıdır (DA-04).

---

## §5 Kardesler

[parameterized-queries](parameterized-queries.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [command-timeout-policy](command-timeout-policy.md) · [sql-injection-defense](sql-injection-defense.md) · [dapper-async-patterns](dapper-async-patterns.md)
