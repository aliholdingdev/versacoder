---
title: "unit-of-work-pattern - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Tek baglanti + tek DbTransaction ile birim is: IUnitOfWork, tek commit, rollback, TransactionScope yasagi (DA-13)"
---

# unit-of-work-pattern — Unit of Work

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [transaction-scope-local](transaction-scope-local.md)

> **Tek cümlelik sorumluluk:** *Birden çok repository çağrısı tek atomik işte nasıl birleştirilir ve tek yerde commit edilir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Unit of Work, **tek bağlantı + tek `DbTransaction`** taşır; repository'ler o transaction'ı parametre olarak alır, **commit etmez**. Commit/rollback **tek yerdedir**: `IUnitOfWork.CommitAsync()`. `TransactionScope` **yok** (DA-13). İş bittiğinde unit dispose edilir ve bağlantı kapanır — uzun ömürlü unit, havuz tükenmesi demektir; birim iş **kısa** tutulur.

---

## §1 Arayuz + kullanim

```csharp
public interface IUnitOfWork : IDisposable
{
    Task ExecuteAsync(Func<IDbExecutor, CancellationToken, Task> work, CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);   // TEK commit noktasi
}

// kullanim: tek is, tek commit
await _uow.ExecuteAsync(async (db, ct) =>
{
    await db.ExecuteAsync(insertSql, p1, ct);
    await db.ExecuteAsync(updateSql, p2, ct);
}, cancellationToken);                                       // hepsi ya hep ya hic
```

---

## §2 Sorumluluk tablosu

| Katman | Ne yapar | Ne yapmaz |
|---|---|---|
| `IUnitOfWork` | bağlantı + `DbTransaction` sahibi, commit/rollback | iş kuralı |
| Repository | `transaction`'ı alır, sorguyu onunla çalıştırır | `Commit()` çağırmak |
| Servis/handler | birim işi kurar, hata durumunda yay | bağlantı açmak |
| Migration runner | kendi tek transaction'ı (DA-11) | uygulama transaction'ına girmek |

---

## §3 Kural tablosu (DA-13)

| # | Kural | Yasak |
|---|---|---|
| 1 | Birim = tek bağlantı, tek transaction | her metot kendi bağlantısı + ayrı commit |
| 2 | Commit **tek** noktada | `foreach` içinde satır başına commit |
| 3 | Hata → rollback + `throw;` | yutulan exception, yarım veri |
| 4 | İç içe transaction yok | ikinci `BeginTransaction` |
| 5 | Ömür kısa (iş kadar) | singleton unit / alan olarak tutmak |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `IDbTransaction` | var | var |
| `CommitAsync`/`RollbackAsync` | var | var |
| `await using` | yok — klasik `using (…) { }` | yazılabilir |
| `TransactionScope` | teknik var → **yasak** | yasak |
| SQLite dalı | kullanılmaz (ADR-012 D4) | tek yazıcı — unit iş kısa tutulur |

> **UYARI:** Unit of Work bir **şablon değil**, tek bir `DbTransaction`'ın ömrüdür; repository'ye `IUnitOfWork` enjeksiyonu yerine `work(db, ct)` deseni tercih edilir — böylece transaction yolu görünür kalır (DA-13).

---

## §5 Kardeşler

[transaction-scope-local](transaction-scope-local.md) · [repository-pattern-thin](repository-pattern-thin.md) · [concurrency-optimistic](concurrency-optimistic.md) · [dapper-async-patterns](dapper-async-patterns.md) · [migration-version-table](migration-version-table.md)
