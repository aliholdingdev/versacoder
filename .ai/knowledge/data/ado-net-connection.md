---
title: "ado-net-connection - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DbConnection yasam dongusu: factory ile acma, kisa omurlu kullanma, kapatma ve havuza iade (DA-04 / DA-09 / DA-10)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ado-net-connection - ADO.NET Baglanti Yasam Dongusu

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[.rules/async-rules|async-rules]] · [[.rules/error-handling-rules|error-handling-rules]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Bir `DbConnection` nereden gelir, ne zaman acilir, ne zaman kapanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Baglanti **her metotta yeni** uretilir: `IDbConnectionFactory.Create()` **kapali** bir `DbConnection` dondurur, sahibi onu `using` blogu icinde acar ve blog sonunda kapatir. Alan seviyesinde ya da statik tutulan baglanti yasaktir (DA-10). Somut sinif adi (`SqlConnection`, `SqliteConnection`) yalnizca factory'nin icindedir; repository yalniz `DbConnection` gorur (DA-04). Kapatmak fiziksel baglantiyi yok etmez, onu havuza iade eder.

```
repository metodu
   |
   +-- _factory.Create()   -> DbConnection (KAPALI)
   +-- OpenAsync(ct)       -> havuzdan gelir ya da yeni acilir
   +-- Query / Execute     -> CommandTimeout sabittir (DA-09)
   +-- using blogu biter   -> Close() + havuza iade
```

---

## §1 Dogru kullanim

```csharp
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);

    const string sql =
        "SELECT Id, Title FROM Sessions WHERE Id = @Id AND DeletedAt IS NULL";

    var row = await connection.QuerySingleOrDefaultAsync<SessionRow>(
        sql,
        new { Id = id },
        commandTimeout: DbTimeouts.Query,
        cancellationToken: cancellationToken);
}
```

---

## §2 Kural tablosu (DA-04 - DA-09 - DA-10)

| # | Konu | Dogru | Yanlis |
|---|---|---|---|
| 1 | Baglanti kaynagi | `_factory.Create()` | repository icinde `new SqlConnection(...)` |
| 2 | Omur | metot ici `using` | static / alan seviyesinde `DbConnection` |
| 3 | Acma | `OpenAsync(cancellationToken)` | async metotta senkron `Open()` |
| 4 | Kapatma | `using` blogu sonu | `GC`'ye birakmak ya da acik unutmak |
| 5 | Timeout | her cagrida `DbTimeouts.Query` (15 sn) | yok ya da `0` |

---

## §3 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| `using var` / `await using` | C# 8 sozdizimi - paylasilan koda yazilmaz, klasik `using ( ... ) { }` | yazilabilir |
| `IAsyncDisposable` | `DbConnection` bu arayuzu **implemente etmez** | implemente eder -> `await using` gecerli |
| Somut saglayici | `Microsoft.Data.SqlClient` **5.2.x** | **ayni paket, ayni surum** |
| SQLite baglantisi | **yok** (test dalinda kullanilmaz) | `Microsoft.Data.Sqlite` 8.0.x - yalniz test |

> **KURAL:** Paylasilan kod iki bacakta da derlenir; dil kesisimi C# 7.3'te yazilir. `#if` yalniz platform API farki icin kullanilir (ADR-001).

---

## §4 Sik hatalar

| Hata | Sonuc | Duzeltme |
|---|---|---|
| Alan seviyesinde `DbConnection` | yaris durumu, havuz kirliligi | metot ici `using` |
| `Open()` sonrasi unutulan baglanti | havuz tukener, 30 sn bekleme | `using` blogu |
| Repository'de `new SqliteConnection(...)` | DA-04 ihlali -> `reviewer` RED | `IDbConnectionFactory` |
| Kapali baglantida sorgu | `InvalidOperationException` | `OpenAsync` sonra sorgu |
| `catch` icinde sessiz gecis | yarim is gorunmez | `throw;` ile yeniden yay |

---

## §5 Baglanti metni ve sifre

Baglanti metni duz metin durmaz; sifreli saklanir ve calistirma aninda cozulur. Ayrinti kardes dosyadadir:

| Soru | Cevap | Dosya |
|---|---|---|
| Parola nerede saklanir? | DPAPI ile korunan blob | [[knowledge/data/connection-string-secrets|connection-string-secrets]] |
| Havuzda ne olur? | `Close()` baglantiyi iade eder | [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] |
| Komut nereden gelir? | `connection.CreateCommand()` | [[knowledge/data/ado-net-command|ado-net-command]] |
| Saglayici nasil secilir? | DI kaydi, `#if` degil | [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] |

---

## §6 Sorumluluk siniri

| Katman | Sorumluluk |
|---|---|
| `IDbConnectionFactory` | somut saglayiciyi bilir, kapali baglanti dondurur |
| Repository | `DbConnection` gorur, `using` ile yonetir |
| Servis / UI | baglanti gormez, yalniz CancellationToken verir |
| Test | ayni arayuz, farkli factory kaydi |

---

## §7 Kardesler

[[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]]

---

## §8 Kisa karar

`Create()` -> `OpenAsync(ct)` -> sorgu -> `using` kapanis. Dordusu ayni metot icinde, araya baskasi girmeyen kisa bir birim halinde. Alan seviyesinde baglanti, senkron `Open()` ve timeout'suz cagri ucu hata kapidir.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `ado-net-connection.md` | factory + `using` deseni, DA-04/DA-10 kural tablosu, sik hata tablosu, iki bacak farki |
| `connection-pooling-notes.md` | `Close()` - havuza iade davranisi, havuz tukemesi belirtileri |
| `connection-string-secrets.md` | baglanti metni sifreleme yonu |
| `dual-tfm-provider-choice.md` | SqlClient 5.2.x ortak surum, SQLite yalniz net10 test |
| `ado-net-command.md` | `CommandTimeout` zorunlulugu |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
