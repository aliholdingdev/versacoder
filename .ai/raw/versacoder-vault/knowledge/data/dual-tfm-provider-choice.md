---
title: "dual-tfm-provider-choice - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "net48 ve net10.0-windows bacaklarinda saglayici secimi: SqlClient 5.2.x ortak, Sqlite yalniz net10 test, #if disiplini (ADR-012)"
---

# dual-tfm-provider-choice — İki Bacakta Sağlayıcı Seçimi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [data-access-rules](../../.rules/data-access-rules.md)

> **Tek cümlelik sorumluluk:** *Hangi TFM'de hangi sağlayıcı paketi kullanılır, `#if` ne zaman serbesttir, hangi karar henüz doğrulanmamıştır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Tek ortak taban: **`Microsoft.Data.SqlClient` 5.2.x + Dapper 2.1.x — her iki bacakta aynı sürüm** (ADR-012 D1/D2, CPM tek satır). **SQLite yalnız `net10.0-windows` testindedir** (D3), `net48`'de yoktur (D4, **NEEDS_VALIDATION** — PART 5'te `dotnet build -f net48` ile teyit). Sağlayıcı tercihi `#if` ile değil, **`IDbConnectionFactory` + `ISqlDialect` kaydı** ile yapılır; `#if` yalnız platform API köprüsünde kalır (ADR-001).

---

## §1 Karar tablosu (ADR-012 §2)

| # | Karar | `net48` | `net10.0-windows` |
|---|---|---|---|
| D1 | SQL Server sağlayıcı | `Microsoft.Data.SqlClient` **5.2.x** | **aynı 5.2.x** |
| D2 | Dapper | **2.1.x** | **2.1.x** |
| D3 | SQLite | — | `Microsoft.Data.Sqlite` 8.0.x (yalnız test) |
| D4 | SQLite `net48` | **YOK** (kullanılmaz) | — |
| D5 | Sahte veritabanı | yok — gerçek şema | yok — gerçek şema |
| D6 | Şema | saf SQL + `__Migrations` + checksum | aynı |

---

## §2 Kod ornegi — saglayici secimi `#if` degil, DI

```csharp
// Host/DI kaydi (tek nokta)
services.AddSingleton<IDbConnectionFactory>(sp => useSqlite
    ? new SqliteConnectionFactory(sp.GetRequiredService<IOptions<TestDbOptions>>())
    : new SqlServerConnectionFactory(sp.GetRequiredService<IOptions<DbOptions>>()));

// Repository bilmez: iki bacakta ayni kod
using (var connection = _connectionFactory.Create()) { /* ... */ }
```

---

## §3 `#if` disiplini tablosu

| Durum | `#if` serbest mi? | Ne yapılır |
|---|---|---|
| Platform API farkı (ör. Windows DPI) | **evet** | `#if NETFRAMEWORK` |
| Dil sürümü farkı (`record`, `await using`) | hayır | paylaşılan kod C# 7.3 kesişimi |
| Sağlayıcı farkı | hayır | `IDbConnectionFactory` / `ISqlDialect` |
| Farklı SqlClient sürümü (CPM koşullu) | hayır | yasak — sürüm kilidi (G8) |
| Test altyapısı | hayır | test projesi ayrı TFM |

---

## §4 Doğrulama listesi

| # | Kontrol | Kanıt |
|---|---|---|
| 1 | `Directory.Packages.props` = 5.2.x / 2.1.x tek satır | dosya çıktısı |
| 2 | `dotnet build -f net48` → 0 hata | build çıktısı |
| 3 | `dotnet build -f net10.0-windows` → 0 hata | build çıktısı |
| 4 | EF Core paket referansı = **0** (G2) | NuGet taraması |
| 5 | `Microsoft.Data.Sqlite` `net48` teyidi | **NEEDS_VALIDATION** → PART 5 |

---

## §5 Kardeşler

[sqlserver-provider-setup](sqlserver-provider-setup.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [ado-net-connection](ado-net-connection.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [pagination-offset-keyset](pagination-offset-keyset.md)
