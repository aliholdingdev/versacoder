---
title: "dapper-async-patterns - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Dapper async kullanimi: QueryAsync/ExecuteAsync, CommandDefinition + CancellationToken, ConfigureAwait, deadlock kacinma"
---

# dapper-async-patterns — Async Davranış Kuralları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [data index](../index.md) · [ado-net-connection](ado-net-connection.md)

> **Tek cümlelik sorumluluk:** *Veri erişimi async'i nerede başlar, iptal nasıl taşınır, WinForms'ta deadlock nasıl önlenir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Sorgular `QueryAsync` / `ExecuteAsync` ile çalışır, her çağrıda `CancellationToken` ve `commandTimeout: DbTimeouts.Query` taşınır. **Kütüphane kodunda** `.Result` / `.Wait()` / `GetAwaiter().GetResult()` **yasaktır** — özellikle `net48` WinForms `SynchronizationContext`'inde UI donmasına yol açar. Tek kural: async zincirin tamamı `await` + (kütüphane katmanında) `ConfigureAwait(false)` ile yürür; senkron sarmalayıcı yazılmaz.

---

## §1 Doğru kullanım

```csharp
public async Task<SessionRow?> GetAsync(Guid id, CancellationToken cancellationToken)
{
    const string sql = "SELECT Id, Title FROM Sessions WHERE Id = @Id AND DeletedAt IS NULL";
    using (var connection = _connectionFactory.Create())
    {
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return await connection.QuerySingleOrDefaultAsync<SessionRow>(
            new CommandDefinition(sql, new { Id = id },
                commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }
}
```

---

## §2 Kural tablosu

| # | Kural | Neden |
|---|---|---|
| 1 | Her async veri metodu `CancellationToken` alır (son parametre) | iptal edilebilir UI |
| 2 | Her çağrıda `CommandTimeout` (DA-09) | iptal ≠ timeout, ikisi de gerekli |
| 3 | Kütüphane katmanında `ConfigureAwait(false)` | WinForms `SynchronizationContext` kilitlenmesi |
| 4 | `.Result` / `.Wait()` = **0** | deadlock + `AggregateException` kaybı |
| 5 | Senkron sarmalayıcı (`Get().Result`) **yazılmaz** | çift API yüzeyi, bloke olan UI |

---

## §3 Async secim tablosu

| İş | Çağrı | Not |
|---|---|---|
| Satır oku | `QueryAsync<T>` | `CommandDefinition` ile iptal |
| Tek satır | `QuerySingleAsync<T>` / `QuerySingleOrDefaultAsync<T>` | fazla satır = exception (kasıtlı) |
| Yaz | `ExecuteAsync` | etkilenen satır sayısı döner |
| Skaler | `ExecuteScalarAsync<T>` | `COUNT`/`EXISTS` |
| Çok set | `QueryMultipleAsync` | `CommandDefinition` içinde |
| Bağlantı açma | `OpenAsync(ct)` | senkron `Open()` async metotta yasak |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| `ConfigureAwait` | gerekli ve yeterli | aynı |
| `await using` | yok — klasik `using (…) { }` | yazılabilir |
| Async `Main` | C# 7.1+ — var | var |
| Eşzamanlılık testi | UI `SynchronizationContext` altında **aynı** test | aynı |
| Dapper iptal | `CommandDefinition.CancellationToken` **2.1.x** | aynı |

> **UYARI:** `net48` bacağında senkron DB çağrısı (`.Result`) ile `net10`'da geçmek yanıltıcıdır: her iki bacakta da test, iptal ve timeout davranışı **aynı** senaryoyla kanıtlanır.

---

## §5 Kardeşler

[ado-net-connection](ado-net-connection.md) · [command-timeout-policy](command-timeout-policy.md) · [transaction-scope-local](transaction-scope-local.md) · [dapper-query-mapping](dapper-query-mapping.md) · [repository-pattern-thin](repository-pattern-thin.md)
