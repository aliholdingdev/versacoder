---
title: "transaction-scope-local - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Yerel DbTransaction ile atomiklik: tek birim-tek commit, rollback, izolasyon seviyesi, TransactionScope yasagi (DA-13)"
---

# transaction-scope-local — Yerel Transaction Politikası

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cümlelik sorumluluk:** *Atomik bir iş nasıl tek transaction'a alınır, rollback nerede çağrılır, neden `TransactionScope` kullanılmaz* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Tek birim iş = **tek `DbTransaction`**, commit **tek yerde**, `catch` içinde `Rollback` + `throw;`. `TransactionScope` **yasaktır** (DA-13): `net48` + iki sağlayıcıda davranış farklılığı ve distribüte algılama maliyeti taşır. SQLite dalında **tek yazıcı** kuralı vardır (WAL + `busy_timeout`) — uzun transaction yerine kısa iş paketleri tercih edilir; izolasyon seviyesi bilinçli seçilir, varsayılana bırakılmaz.

---

## §1 Doğru kullanım

```csharp
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);
    using (var tx = await connection.BeginTransactionAsync(cancellationToken))
    {
        try
        {
            await connection.ExecuteAsync(insertSql, p1, tx,
                commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(updateSql, p2, tx,
                commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;                       // bos catch YASAK (CS-05)
        }
    }
}
```

---

## §2 Politika tablosu (DA-13)

| # | Kural | Yasak / Belirti |
|---|---|---|
| 1 | Tek birim iş → tek transaction | döngüde satır başına commit |
| 2 | `tx` tüm komutlara açıkça verilir | `tx` unutulup bağımsız commit |
| 3 | Rollback `catch` içinde, `throw;` ile | yutulan exception |
| 4 | İç içe transaction **yok** | ikinci `BeginTransaction` aynı bağlantıda |
| 5 | `TransactionScope` **yok** (DA-13) | `using (new TransactionScope(...))` |

---

## §3 İzolasyon seviyesi seçimi

| Seviye | Ne korur | Tipik kullanım |
|---|---|---|
| `ReadCommitted` | okuma sırasında değişen satır | varsayılan rapor işi |
| `RepeatableRead` | aynı satırın iki okuması | sayaç/tekrar okuma |
| `Serializable` | aralık kayması (range) | seri numarası üretimi |
| `ReadUncommitted` | hiçbir şey | **kullanılmaz** |
| (SQLite) tek yazıcı | WAL + `busy_timeout` | eşzamanlı yazma yerine sıralama |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `BeginTransactionAsync` | var (4.5+) | var |
| `CommitAsync`/`RollbackAsync` | var | var |
| `await using` (bağlantı/transaction) | yok — klasik `using (…) { }` | yazılabilir |
| `TransactionScope` | teknik olarak var → **yine yasak** | yasak (tutarlılık) |
| SQLite transaction | **kullanılmaz** (ADR-012 D4) | WAL + tek transaction dikkatli |

> **UYARI:** `System.Transactions`/`TransactionScope` için `net48`'de ek referans eklenmez — kural DA-13'tür ve iki bacakta da geçerlidir.

---

## §5 Kardeşler

[unit-of-work-pattern](unit-of-work-pattern.md) · [dapper-async-patterns](dapper-async-patterns.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [concurrency-optimistic](concurrency-optimistic.md) · [migration-rollback](migration-rollback.md)
