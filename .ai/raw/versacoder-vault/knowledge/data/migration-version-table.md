---
title: "migration-version-table - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "__Migrations surum tablosu semasi, SHA-256 checksum, fail-fast dogrulama ve IMigrationRunner akisi (DA-11)"
---

# migration-version-table — Sürüm Tablosu ve Checksum

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [migration-script-writing](migration-script-writing.md)

> **Tek cümlelik sorumluluk:** *Uygulama hangi migration'ların uygulandığını nereden bilir, dosya değiştiğini nasıl fark eder* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Kanonik tablo adı **`__Migrations`** (`Id, Name, Checksum, AppliedAt`) — `data-access-rules.md` DA-11 bu adı SSOT kabul eder. Uygulama açılışında her dosyanın **SHA-256**'sı kayıtlı checksum ile karşılaştırılır: **uyuşmazlıkta uygulama başlamaz (fail fast)**, eksik migration sırayla ve **tek tek transaction** içinde uygulanır. `schema_version` adı vault'ta geçerli bir geçmiş ifadedir; **CONFLICT DETECTED** — tek tablo adı `__Migrations`, düzeltme PART 6 kapsamındadır (G10: bu dosya hariç başka dosya değiştirilmez).

---

## §1 Sema

```sql
-- kanonik ad: __Migrations
CREATE TABLE IF NOT EXISTS __Migrations
(
    Id         INTEGER PRIMARY KEY,
    Name       TEXT    NOT NULL UNIQUE,   -- V0001__init.sql
    Checksum   TEXT    NOT NULL,          -- SHA-256 (hex), dosya icerigi
    AppliedAt  TEXT    NOT NULL           -- ISO-8601 UTC
);
```

---

## §2 Runner akisi tablosu

| Adım | İş | Hata durumunda |
|---|---|---|
| 1 | Dosyaları adıyla sırala (`V0001__…`) | atlanan numara → **dur** |
| 2 | Uygulanmış her satırın checksum'ını dosyayla karşılaştır | uyuşmazlık → **FAIL FAST, açılış yok** |
| 3 | Eksik dosyayı oku, `DbTransaction` içinde çalıştır | hata → rollback, uygulama **başlamaz** |
| 4 | `__Migrations` satırını ekle, commit | — |
| 5 | `AppliedAt` UTC `TimeProvider` (DA-15) | yerel saat yasak |

---

## §3 Kural tablosu (DA-11 · DA-12)

| # | Kural | Yasak |
|---|---|---|
| 1 | Tablo adı `__Migrations` | ikinci sürüm tablosu açmak |
| 2 | Checksum = SHA-256 dosya içeriği | elle hash, tarih tabanlı |
| 3 | Uygulanmış dosya **dokunulmaz** | sonradan düzenleme → checksum FAIL |
| 4 | Dosya adı `V<4 hane>__<ad>.sql` | boşluk/Türkçe karakter |
| 5 | Uygulama başında doğrulama | sessiz "geç" seçeneği |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| SHA-256 | `SHA256.Create()` + `ComputeHash` | aynı API |
| Dosya okuma | `File.ReadAllBytes` | aynı |
| `await using` | yok — klasik `using (…) { }` | yazılabilir |
| Runner nerede çalışır | uygulama açılışı (her iki bacak) | aynı akış |
| Tablo SQL'i | sağlayıcı farkı varsa migration dialekt ayrımı (**NEEDS_VALIDATION**) | aynı |

---

## §5 Kardeşler

[migration-script-writing](migration-script-writing.md) · [migration-rollback](migration-rollback.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [audit-columns-convention](audit-columns-convention.md)
