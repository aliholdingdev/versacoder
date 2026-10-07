---
title: "transaction-scope-local — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Yerel DbTransaction ile atomiklik: tek birim-tek commit, rollback, izolasyon seviyesi, TransactionScope yasagi (DA-13)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# transaction-scope-local — Yerel Transaction Politikası

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [error handling](../../.rules/error-handling-rules.md) · [unit-of-work-pattern](unit-of-work-pattern.md) · [dapper-async-patterns](dapper-async-patterns.md) · [sqlite-provider-setup](sqlite-provider-setup.md)
**Küme + Hub:** [data index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (data · 6/25):** [ado-net-command](ado-net-command.md) · [ado-net-connection](ado-net-connection.md) · [audit-columns-convention](audit-columns-convention.md) · [command-timeout-policy](command-timeout-policy.md) · [connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [dapper-query-mapping](dapper-query-mapping.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [parameterized-queries](parameterized-queries.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sql-injection-defense](sql-injection-defense.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md)

> **Tek cumlelik sorumluluk:** *Atomik bir iş nasıl tek transaction'a alınır, rollback nerede çağrılır, neden `TransactionScope` kullanılmaz* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Tek birim iş = **tek `DbTransaction`**; commit **tek yerde**, `catch` bloğunda `Rollback` + `throw;` ile biter. `TransactionScope` yasaktır (DA-13): `net48` ile iki sağlayıcı arasında davranış farklılığı ve distribüte algılama maliyeti taşır. SQLite dalında **tek yazıcı** kuralı vardır (WAL + `busy_timeout`), bu yüzden uzun transaction yerine kısa iş paketleri tercih edilir. İzolasyon seviyesi bilinçli seçilir; sağlayıcı varsayılanına bırakılmaz.

## §1 Dogru kullanim

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

`tx`, çalıştırılan **her** komuta açıkça verilir; verilmeyen komut transaction dışında kalır.

## §2 Politika tablosu (DA-13)

| # | Kural | Yasak / belirti |
|---|---|---|
| 1 | Tek birim iş → tek transaction | döngüde satır başına commit |
| 2 | `tx` tüm komutlara açıkça iletilir | `tx` unutulup bağımsız commit |
| 3 | Rollback `catch` içinde, `throw;` ile | yutulan exception |
| 4 | İç içe transaction yok | aynı bağlantıda ikinci `BeginTransaction` |
| 5 | `TransactionScope` yok | `using (new TransactionScope(...))` |

## §3 Izolasyon seviyesi

| Seviye | Ne korur | Tipik kullanım |
|---|---|---|
| `ReadCommitted` | okuma sırasında değişen satır | varsayılan rapor işi |
| `RepeatableRead` | aynı satırın iki okuması | sayaç / tekrar okuma |
| `Serializable` | aralık kayması (range) | seri numarası üretimi |
| `ReadUncommitted` | hiçbir şeyi korumaz | **kullanılmaz** |
| SQLite: tek yazıcı | WAL + `busy_timeout` | eşzamanlı yazma yerine sıralama |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `BeginTransactionAsync` | var (4.5+) | var |
| `CommitAsync` / `RollbackAsync` | var | var |
| `await using` | yok → klasik `using ( ... ) { }` | yazılabilir |
| `TransactionScope` | teknik olarak var → **yine yasak** | yasak (tutarlılık) |
| SQLite transaction | bu bacakta kullanılmaz | WAL altında tek transaction, dikkatli |

`System.Transactions` için `net48`'de ek referans **eklenmez**; kural her iki bacakta da geçerlidir.

## §5 Hata senaryolari

| Durum | Sonuç | Yapılacak |
|---|---|---|
| Commit'ten önce exception | değişiklik kalmaz | rollback + `throw;` |
| `tx` verilmeyen komut | komut transaction dışında kalır | her komuta `tx` geçir |
| Uzun transaction (SQLite) | yazıcılar bekler / kilit | işi kısa paketlere böl |
| Yutulan exception | yarım iş görünür | `catch` → rollback + `throw;` |
| Distribüte sanılan transaction | `TransactionScope` maliyeti | zaten yasak (DA-13) |

## §6 Sik hatalar

- `foreach` içinde satır başına commit
- Boş `catch (Exception) { }` ile rollback'i gizlemek
- Aynı bağlantıyla iç içe `BeginTransaction`
- İzolasyon seviyesini sağlayıcıya bırakmak
- `TransactionScope`'u "sonradan dağıtıma hazırlık" diye kullanmak

## §7 Dogrulama notlari

`ReadCommitted` yerine `RepeatableRead`/`Serializable` seçiminin hangi senaryolarda zorunlu kılınacağı vault'ta senaryo bazında sabitlenmemiştir — **DOĞRULAMA GEREKLI**. Seviye kararı kod incelemesinde gerekçesiyle belirtilir.

## §8 Kardesler

[unit-of-work-pattern](unit-of-work-pattern.md) · [dapper-async-patterns](dapper-async-patterns.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [concurrency-optimistic](concurrency-optimistic.md) · [migration-rollback](migration-rollback.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `transaction-scope-local.md` (kaynak) | transaction örneği, DA-13 kural tablosu, izolasyon tablosu, TFM farkları |
| `unit-of-work-pattern.md` | tek bağlantı + tek transaction'ın sahipliği |
| `dapper-async-patterns.md` | async `ExecuteAsync` ve `CancellationToken` kullanımı |
| `sqlite-provider-setup.md` | WAL, `busy_timeout` ve tek yazıcı kuralı |
| `migration-rollback.md` | geri alma akışının uygulama durdurmayı gerektirmesi |
