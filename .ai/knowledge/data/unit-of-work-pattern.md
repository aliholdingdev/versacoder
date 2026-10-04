---
title: "unit-of-work-pattern — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Tek baglanti + tek DbTransaction ile birim is: IUnitOfWork, tek commit, rollback, TransactionScope yasagi (DA-13)"
---

# unit-of-work-pattern — Unit of Work

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [transaction-scope-local](transaction-scope-local.md) · [repository-pattern-thin](repository-pattern-thin.md) · [migration-version-table](migration-version-table.md)

> **Tek cumlelik sorumluluk:** *Birden çok repository çağrısı tek atomik işte nasıl birleştirilir ve tek yerde commit edilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Unit of Work **tek bağlantı + tek `DbTransaction`** taşır; repository'ler o transaction'ı parametre olarak alır ve **commit etmez**. Commit/rollback tek noktadadır: `IUnitOfWork.CommitAsync()`. `TransactionScope` kullanılmaz (DA-13). İş bitince unit dispose edilir ve bağlantı kapanır: uzun ömürlü unit = havuz tükenmesi, bu yüzden birim iş **kısa** tutulur.

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

`work(db, ct)` deseni transaction yolunu görünür tutar: hangi komutun hangi bağlantıyla gittiği çağrı yerinde okunur.

## §2 Sorumluluk tablosu

| Katman | Ne yapar | Ne yapmaz |
|---|---|---|
| `IUnitOfWork` | bağlantı + `DbTransaction` sahibi, commit/rollback | iş kuralı |
| Repository | `transaction`'ı alır, sorguyu onunla çalıştırır | `Commit()` çağırmak |
| Servis / handler | birim işi kurar, hatayı yukarı yayar | bağlantı açmak |
| Migration runner | kendi tek transaction'ı (DA-11) | uygulama transaction'ına girmek |

## §3 Kural tablosu (DA-13)

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Birim = tek bağlantı, tek transaction | her metot kendi bağlantısı + ayrı commit |
| 2 | Commit tek noktada | `foreach` içinde satır başına commit |
| 3 | Hata → rollback + `throw;` | yutulan exception, yarım veri |
| 4 | İç içe transaction yok | ikinci `BeginTransaction` |
| 5 | Ömür iş kadar kısa | singleton unit / alan olarak tutmak |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `IDbTransaction` | var | var |
| `CommitAsync` / `RollbackAsync` | var | var |
| `await using` | yok → klasik `using ( ... ) { }` | yazılabilir |
| `TransactionScope` | teknik var → yasak | yasak |
| SQLite dalı | bu bacakta kullanılmaz | tek yazıcı — birim iş kısa tutulur |

## §5 Ne zaman kullanilir

| Senaryo | Yaklaşım |
|---|---|
| İki repository çağrısı, tek iş | tek `IUnitOfWork` içinde `work(db, ct)` |
| Tek okuma | unit gerekmez — doğrudan repository |
| Birden çok iş, arada commit gerekiyor | ayrı birimler; iç içe transaction yok |
| Migration | runner'ın kendi transaction'ı (DA-11) |
| Dağıtık iş | `TransactionScope` yine yok (DA-13) |

## §6 Sik hatalar

- Unit'i singleton kaydedip bağlantıyı açık tutmak
- Repository'ye `IUnitOfWork` enjeksiyonu ile transaction yolunu gizlemek
- Birden çok unit'i aynı anda başlatmak (iç içe transaction)
- Commit'i servis ile repository'ye dağıtmak
- İş yerine unit'i `await using` ile uzun ömürde tutmak

## §7 Dogrulama notlari

`IUnitOfWork` arayüzünün kesin imzası (`ExecuteAsync` mi, yoksa açık `BeginAsync`/`CommitAsync` mi) vault'ta tek örnekle sabitlenmiştir — **DOĞRULAMA GEREKLI**. İmza değişirse [transaction-scope-local](transaction-scope-local.md) ile birlikte güncellenir.

## §8 Kardesler

[transaction-scope-local](transaction-scope-local.md) · [repository-pattern-thin](repository-pattern-thin.md) · [concurrency-optimistic](concurrency-optimistic.md) · [dapper-async-patterns](dapper-async-patterns.md) · [migration-version-table](migration-version-table.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `unit-of-work-pattern.md` (kaynak) | arayüz + kullanım örneği, sorumluluk tablosu, DA-13 kuralları, TFM farkları |
| `transaction-scope-local.md` | tek `DbTransaction` disiplini ve `TransactionScope` yasağı |
| `repository-pattern-thin.md` | repository'nin `Commit()` çağrımaması |
| `dapper-async-patterns.md` | async çalışma ve iptal edilebilirlik |
| `migration-version-table.md` | migration runner'ın ayrı transaction'ı |
