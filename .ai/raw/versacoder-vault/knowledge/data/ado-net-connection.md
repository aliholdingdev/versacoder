---
title: "ado-net-connection - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DbConnection yasam dongusu: factory ile acma, kisa omurlu kullanma, kapatma (DA-04/DA-10) ve iki bacak notu"
---

# ado-net-connection — ADO.NET Bağlantı Yaşam Döngüsü

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [connection-pooling-notes](connection-pooling-notes.md)

> **Tek cumlelik sorumluluk:** *Bir `DbConnection` nereden gelir, ne zaman açılır, ne zaman kapanır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Bağlantı **her metotta yeni** üretilir: `IDbConnectionFactory.Create()` **kapalı** `DbConnection` döndürür, sahibi onu `using` bloğu içinde açar ve blok sonunda kapatır. Alan seviyesinde / statik tutulan bağlantı **yasaktır** (DA-10). Somut sınıf adı (`SqlConnection`, `SqliteConnection`) yalnızca factory'nin içindedir; repository yalnız `DbConnection` görür (DA-04). Kapatmak fiziksel bağlantıyı yok etmez, onu havuza iade eder — ayrıntı kardeş dosyada.

```
repository metodu
   |
   +-- _factory.Create()     -> DbConnection (KAPALI)
   +-- OpenAsync(ct)         -> havuzdan gelir ya da yeni acilir
   +-- Query/ExecuteAsync    -> CommandTimeout sabiti zorunlu (DA-09)
   +-- using blogu kapanir   -> Close() + havuza iade
```

---

## §1 Dogru kullanim (3-10 satir)

```csharp
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);
    const string sql = "SELECT Id, Title FROM Sessions WHERE Id = @Id AND DeletedAt IS NULL";
    var row = await connection.QuerySingleOrDefaultAsync<SessionRow>(
        sql, new { Id = id }, commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
}
```

---

## §2 Kural tablosu (DA-04 · DA-10 · DA-09)

| # | Konu | Doğru | Yanlış |
|---|---|---|---|
| 1 | Bağlantı kaynağı | `_factory.Create()` | `new SqlConnection(...)` repository içinde |
| 2 | Ömür | metot içi `using` | static/alan seviyesinde `DbConnection` |
| 3 | Açma | `OpenAsync(ct)` | async metotta senkron `Open()` |
| 4 | Kapatma | `using` bloğu sonu (finally yok, blok yeter) | `GC`'ye bırakmak / açık unutmak |
| 5 | Timeout | her komutta `DbTimeouts.Query` (15 sn) | yok ya da `0` |

---

## §3 net48 / net10.0-windows farkı

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| `using var` / `await using` | C# 8 söz dizimi — **paylaşılan koda yazılmaz**, klasik `using (…) { }` | yazılabilir (`data-access-rules.md` §3.4 biçimi) |
| `IAsyncDisposable` | `DbConnection` bu arayüzü **implemente etmez** | implemente eder → `await using` geçerli |
| Somut sağlayıcı | `Microsoft.Data.SqlClient` **5.2.x** (ADR-012 D1) | **aynı paket, aynı sürüm** |
| SQLite bağlantısı | **yok** (ADR-012 D4) | `Microsoft.Data.Sqlite` 8.0.x — yalnız test |

> **KURAL:** Paylaşılan kod iki bacakta da derlenir → dil kesişimi (C# 7.3) yazılır; `#if` yalnız platform API farkı içindir (ADR-001).

---

## §4 Sık hatalar

| Hata | Sonuç | Düzeltme |
|---|---|---|
| Alan seviyesinde `DbConnection` | yarış durumu, havuz kirliliği | metot içi `using` |
| `Open()` sonrası unutulan bağlantı | havuz tükenir, 30 sn bekleme | `using` bloğu |
| Repository'de `new SqliteConnection(...)` | DA-04 ihlali → `reviewer` RED | `IDbConnectionFactory` |
| Kapalı bağlantıda sorgu | `InvalidOperationException` | `OpenAsync` sonra sorgu |
| `catch` içinde sessiz geç | yarım iş görünmez (CS-05) | `throw;` ile yeniden yay |

---

## §5 Kardesler

[connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [ado-net-command](ado-net-command.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md)
