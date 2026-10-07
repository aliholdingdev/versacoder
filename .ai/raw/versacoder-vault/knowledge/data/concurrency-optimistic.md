---
title: "concurrency-optimistic - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Optimistic eszamanlilik: surum kolonu / WHERE .. AND Version, etkilenen satir sayisi, uyusmazlikta hata ve retry"
---

# concurrency-optimistic — Optimistic Eşzamanlılık

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cümlelik sorumluluk:** *Aynı satıra iki istek aynı anda yazarsa veri kaybı nasıl önlenir, çakışma nasıl anlaşılır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Kilit **yok**; bunun yerine **optimistic** yöntem: her değiştirebilir satırda `Version` (int) ya da `UpdatedAt` kolonu taşınır, `UPDATE … WHERE Id=@Id AND Version=@Version` çalıştırılır ve **etkilenen satır sayısı 0 ise** başka bir kullanıcının yazdığı anlaşılır → domain hatası + kullanıcıya "veri başkasınca değişti" mesajı. Kilitli satır (`SELECT … FOR UPDATE`) **kullanılmaz**: uzun transaction UI'ı ve havuzu bağlar (DA-13). Retry **otomatik değil**, açık ve sınırlıdır (en fazla 1 kez, aynı veri yeniden okunur).

---

## §1 Dogru kullanim

```csharp
const string sql = @"
    UPDATE Sessions
       SET Title = @Title, Version = Version + 1, UpdatedAt = @UpdatedAt
     WHERE Id = @Id AND Version = @Version AND DeletedAt IS NULL";

var affected = await connection.ExecuteAsync(sql,
    new { Id = id, Title = title, Version = expectedVersion, UpdatedAt = nowUtc },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);

if (affected == 0) throw new ConcurrencyConflictException(nameof(Session), id);  // baskasi yazdi
```

---

## §2 Yontem karsilastirmasi

| Yöntem | Nasıl | Artı | Eksi |
|---|---|---|---|
| **Version kolonu** (seçilen) | `WHERE Version=@V` + sayaç artırımı | saf SQL, iki sağlayıcıda aynı | her tabloda kolon |
| `UpdatedAt` imzası | `WHERE UpdatedAt=@E` | kolon eklemeye gerek | hassas saat eşitliği |
| `ROWVERSION` (SQL Server) | donanım üretimi | güvenilir | **SQLite'ta yok** → sağlayıcıya bağımlı |
| Pessimistic kilit | `FOR UPDATE` / transaction | çakışma yok | UI bloğu, havuz riski |
| Son yazan kazanır | `WHERE` yok | — | **veri kaybı — yasak** |

---

## §3 Kural tablosu

| # | Kural | Yasak |
|---|---|---|
| 1 | Güncelleme/deletion her zaman `WHERE` + durum koşulu | koşulsuz `UPDATE` |
| 2 | Etkilenen satır `0` → domain hatası (sessiz geç yok) | `affected` yok saymak |
| 3 | UI'da kullanıcıya açık çakışma mesajı | veriyi kör ezme |
| 4 | Retry: en fazla 1, veriyi yeniden okuyup | kör döngü retry |
| 5 | Version değeri istemciden değil **okumadan** gelir | istemcinin uydurduğu sürüm |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Etkilenen satır (`ExecuteAsync`) | `int` — aynı | aynı |
| `ROWVERSION` | yalnız SQL Server dalı | aynı — **dialect'te** |
| SQLite versiyon kolonu | sağlayıcı kullanılmaz (D4) | `INTEGER` sayaç ile aynı desen |
| Hata tipi | `Exception` alt sınıfı — iki bacakta ortak | aynı |
| Retry | elle (kod) | aynı — ek paket **yok** (YAGNI) |

---

## §5 Kardeşler

[unit-of-work-pattern](unit-of-work-pattern.md) · [transaction-scope-local](transaction-scope-local.md) · [audit-columns-convention](audit-columns-convention.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [repository-pattern-thin](repository-pattern-thin.md)
