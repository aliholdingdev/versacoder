---
title: "ado-net-connection - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DbConnection yasam dongusu: factory ile acma, kisa omurlu kullanma, kapatma ve havuza iade (DA-04 / DA-09 / DA-10)"
---

# ado-net-connection - ADO.NET Baglanti Yasam Dongusu

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-001](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) · [async-rules](../../.rules/async-rules.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [connection-pooling-notes](connection-pooling-notes.md)

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
| Parola nerede saklanir? | DPAPI ile korunan blob | [connection-string-secrets](connection-string-secrets.md) |
| Havuzda ne olur? | `Close()` baglantiyi iade eder | [connection-pooling-notes](connection-pooling-notes.md) |
| Komut nereden gelir? | `connection.CreateCommand()` | [ado-net-command](ado-net-command.md) |
| Saglayici nasil secilir? | DI kaydi, `#if` degil | [dual-tfm-provider-choice](dual-tfm-provider-choice.md) |

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

[connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [ado-net-command](ado-net-command.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [dapper-async-patterns](dapper-async-patterns.md)

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
