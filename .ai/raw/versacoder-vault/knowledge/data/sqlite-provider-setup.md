---
title: "sqlite-provider-setup - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Microsoft.Data.Sqlite kurulumu, WAL/PRAGMA acilis seti, test veritabani olusturma - yalniz net10.0-windows entegrasyon testi (ADR-012 D3/D4)"
---

# sqlite-provider-setup — SQLite Sağlayıcı Kurulumu

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [data index](../index.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md)

> **Tek cümlelik sorumluluk:** *SQLite sağlayıcısı hangi bacakta, hangi PRAGMA setiyle açılır, test veritabanı nasıl üretilir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`Microsoft.Data.Sqlite` **yalnız `net10.0-windows` entegrasyon testlerinde** kullanılır (ADR-012 D3); `net48` bacağında **yoktur** (D4) — `net48` veri testleri SQL Server ve sahte sağlayıcı ile yürür. Her yeni bağlantıda **PRAGMA seti zorunlu**: `journal_mode=WAL`, `foreign_keys=ON`, `busy_timeout` (DA-14). Test veritabanı her çalıştırmada geçici dosya olarak üretilir; **sahte veritabanı yok**, gerçek şema kullanılır (ADR-012 D5).

---

## §1 Acilis seti

```csharp
// Create() KAPALI doner (DA-04 sozlesmesi); PRAGMA baglanti bazlidir, acilistan hemen sonra uygulanir.
public DbConnection Create() =>
    new SqliteConnection($"Data Source={_path};Mode=ReadWriteCreate;Cache=Shared");

public async Task<DbConnection> OpenAsync(DbConnection connection, CancellationToken ct)
{
    await connection.OpenAsync(ct);
    using (var cmd = connection.CreateCommand())
    {
        cmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA foreign_keys=ON; PRAGMA busy_timeout=5000;";
        cmd.CommandTimeout = DbTimeouts.Query;               // DA-09
        await cmd.ExecuteNonQueryAsync(ct);
    }
    return connection;
}
```

> **KURAL:** PRAGMA uygulaması **tek yerdedir** (`SqliteConnectionFactory`); repository bunu bilmez — yalnız `_factory.Create()` + `_factory.OpenAsync()` çağırır.

---

## §2 Ayar tablosu

| PRAGMA / Anahtar | Değer | Neden |
|---|---|---|
| `journal_mode` | `WAL` | eşzamanlı okuma, kilit yarışı azalır (DA-14) — **dosyada kalıcıdır** |
| `foreign_keys` | `ON` | SQLite'ta **varsayılan kapalı** — FK'lar aksi halde yok sayılır, **bağlantıya özeldir** |
| `busy_timeout` | `5000` ms | "database is locked" rastgele hatası — **bağlantıya özeldir** |
| `Mode` | `ReadWriteCreate` | ilk çalıştırmada dosya doğar |
| `Cache` | `Shared` | aynı dosyayı paylaşan test paralelliği |
| Dosya yolu | geçici dizin + GUID | testler birbirine karışmaz |

---

## §3 Kural tablosu (ADR-012 · DA-14)

| # | Kural | Yasak |
|---|---|---|
| 1 | Sağlayıcı yalnız test projesinde | üretim kodunda `SqliteConnection` |
| 2 | PRAGMA seti **tek yerde** (factory) | her sorguda PRAGMA tekrarı |
| 3 | Gerçek şema üzerinde entegrasyon | elle mock'lanmış veri (D5) |
| 4 | Tek yazıcı stratejisi | paralel testte aynı dosyaya 2 yazıcı |
| 5 | Sorgu gövdesi sağlayıcıdan bağımsız | `INSERT OR IGNORE` üretimi kodda |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Microsoft.Data.Sqlite` 8.0.x | **YOK** (ADR-012 D4) | **var** — yalnız entegrasyon testi |
| `using var` | C# 8 — klasik `using (…) { }` | yazılabilir |
| Paralel test | test tek bacakta (`net10`) | `xUnit` paralel + WAL |
| Veri testi stratejisi | SQL Server + sahte sağlayıcı | SQLite gerçek şema |

> **NEEDS_VALIDATION:** `Microsoft.Data.Sqlite`'in `net48` bacağında derlenip derlenmediği PART 5'te `dotnet build -f net48` ile teyit edilir; doğrulanırsa D4 gevşetilir, doğrulanmazsa D4 kalır (ADR-012 §2).

---

## §5 Kardeşler

[dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [migration-script-writing](migration-script-writing.md) · [test-strategy notlari](../../.rules/testing-rules.md)
