---
title: "sqlite-provider-setup — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Microsoft.Data.Sqlite kurulumu, WAL/PRAGMA acilis seti, test veritabani olusturma - yalniz net10.0-windows entegrasyon testi"
---

# sqlite-provider-setup — SQLite Sağlayıcı Kurulumu

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [testing rules](../../.rules/testing-rules.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md)

> **Tek cumlelik sorumluluk:** *SQLite sağlayıcısı hangi bacakta, hangi PRAGMA setiyle açılır, test veritabanı nasıl üretilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`Microsoft.Data.Sqlite` **yalnız `net10.0-windows` entegrasyon testlerinde** çalışır; `net48` bacağında bu sağlayıcı yoktur — oradaki veri testleri SQL Server ve sahte sağlayıcı ile yürür. Her **yeni bağlantıda** PRAGMA seti uygulanır: `journal_mode=WAL`, `foreign_keys=ON`, `busy_timeout` (DA-14). Test veritabanı her çalıştırmada geçici dosya olarak üretilir ve **gerçek şema** üzerinde koşar: sahte/elle doldurulmuş veritabanı yok (ADR-012 D5).

## §1 Acilis seti

```csharp
// Create() KAPALI doner (DA-04 sozlesmesi); PRAGMA baglanti bazlidir.
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

PRAGMA uygulaması **tek yerdedir** (`SqliteConnectionFactory`); repository bunu bilmez, yalnız `_factory.Create()` ve `_factory.OpenAsync()` çağırır.

## §2 Ayar tablosu

| PRAGMA / Anahtar | Değer | Neden |
|---|---|---|
| `journal_mode` | `WAL` | eşzamanlı okuma, kilit yarışı azalır (DA-14) — dosyada kalıcıdır |
| `foreign_keys` | `ON` | SQLite'ta varsayılan **kapalıdır**; aksi halde FK'lar yok sayılır — bağlantıya özeldir |
| `busy_timeout` | `5000` ms | "database is locked" rastgele hatası — bağlantıya özeldir |
| `Mode` | `ReadWriteCreate` | ilk çalıştırmada dosya doğar |
| `Cache` | `Shared` | aynı dosyayı paylaşan test paralelliği |
| Dosya yolu | geçici dizin + GUID | testler birbirine karışmaz |

## §3 Kural tablosu (ADR-012 · DA-14)

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Sağlayıcı yalnız test projesinde | üretim kodunda `SqliteConnection` |
| 2 | PRAGMA seti tek yerde (factory) | her sorgudan önce PRAGMA tekrarı |
| 3 | Gerçek şema üzerinde entegrasyon | elle mock'lanmış veri (D5) |
| 4 | Tek yazıcı stratejisi | paralel testte aynı dosyaya iki yazıcı |
| 5 | Sorgu gövdesi sağlayıcıdan bağımsız | üretim kodunda `INSERT OR IGNORE` |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Microsoft.Data.Sqlite` 8.0.x | **yok** (ADR-012 D4) | var — yalnız entegrasyon testi |
| `using var` | C# 8 yok → klasik `using ( ... ) { }` | yazılabilir |
| Paralel test | tek bacakta (`net10`) | xUnit paralel + WAL |
| Veri testi stratejisi | SQL Server + sahte sağlayıcı | SQLite gerçek şema |

## §5 PRAGMA disiplini

| Hata | Sonuç | Kural |
|---|---|---|
| `foreign_keys` unutulursa | FK ihlalleri sessizce geçer | her `OpenAsync` içinde |
| `busy_timeout` yoksa | rastgele "database is locked" | bağlantı başına uygulanır |
| PRAGMA repository'ye yazılırsa | çift yol, test kırılır | yalnız factory |
| Aynı dosyaya iki yazıcı | WAL çakışması | tek yazıcı stratejisi |
| Dosya sabit yolda | testler birbirini kirletir | geçici dizin + GUID |

## §6 Sik hatalar

- Sağlayıcıyı üretim projesine eklemek (ADR-012 D4 ihlali)
- `Create()` içinde açık bağlantı döndürmek (DA-04 sözleşmesi kırılır)
- WAL yerine varsayılan journal moduyla paralel test koşturmak
- Testi sahte veriyle, gerçek şema dışında yazmak
- PRAGMA'ları her sorguya tekrar eklemek

## §7 Dogrulama notlari

`Microsoft.Data.Sqlite`'in `net48` bacağında derlenip derlenemediği doğrulanmamıştır — **DOĞRULAMA GEREKLI**. Derlenirse ilgili karar gevşetilir, derlenmezse karar aynen kalır.

## §8 Kardesler

[dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [migration-script-writing](migration-script-writing.md) · [concurrency-optimistic](concurrency-optimistic.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `sqlite-provider-setup.md` (kaynak) | açılış kodu, PRAGMA ayar tablosu, ADR-012/DA-14 kuralları, TFM farkları, NEEDS_VALIDATION |
| `sqlserver-provider-setup.md` | net48 bacağının veri testi stratejisi |
| `dual-tfm-provider-choice.md` | sağlayıcıların bacaklara dağılımı |
| `transaction-scope-local.md` | SQLite tek yazıcı kuralı ve WAL |
| `migration-script-writing.md` | SQLite dialect (AUTOINCREMENT, TEXT tarih) |
