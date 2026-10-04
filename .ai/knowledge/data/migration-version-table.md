---
title: "migration-version-table — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "__Migrations surum tablosu semasi, SHA-256 checksum, fail-fast dogrulama ve IMigrationRunner akisi (DA-11)"
---

# migration-version-table — Sürüm Tablosu ve Checksum

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [rules index](../../.rules/index.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [migration-script-writing](migration-script-writing.md) · [migration-rollback](migration-rollback.md)

> **Tek cumlelik sorumluluk:** *Uygulama hangi migration'ların uygulandığını nereden bilir, dosya değiştiğini nasıl fark eder* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Tek sürüm tablosu **`__Migrations`**'tır: `Id, Name, Checksum, AppliedAt` (DA-11). Açılışta dosyaların **SHA-256** özleri kayıtlı checksum ile karşılaştırılır; uyuşmazlıkta uygulama **başlamaz** (fail fast), eksik migration ise sırayla ve her biri kendi `DbTransaction`'ı içinde uygulanır. Zaman `TimeProvider` ile UTC yazılır (DA-15). Vault geçmişinde geçen `schema_version` adı geçersizdir: **tek ad `__Migrations`**, ikinci bir sürüm tablosu açılmaz.

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

`Name` benzersizdir; aynı dosya iki kez uygulanamaz. `Checksum` dosyanın bayt içeriğinden üretilir, elle yazılmaz.

## §2 Runner akisi

| # | Adım | Hata durumunda |
|---|---|---|
| 1 | Dosyalar adıyla sıralanır (`V0001__...`) | numara atlanmışsa **dur** |
| 2 | Kayıtlı checksum, dosyanın SHA-256'sı ile karşılaştırılır | uyuşmazlık → **FAIL FAST, açılış yok** |
| 3 | Eksik dosya okunur, `DbTransaction` içinde çalıştırılır | hata → rollback, uygulama başlamaz |
| 4 | `__Migrations` satırı eklenir, commit edilir | — |
| 5 | `AppliedAt` UTC `TimeProvider` ile yazılır (DA-15) | yerel saat yasak |

## §3 Kural tablosu (DA-11 · DA-12)

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Tablo adı `__Migrations` | ikinci sürüm tablosu (`schema_version` dahil) |
| 2 | Checksum = SHA-256 dosya içeriği | elle hash, tarih tabanlı özet |
| 3 | Uygulanmış dosya dokunulmaz | sonradan düzenleme → checksum FAIL |
| 4 | Dosya adı `V<4 hane>__<ad>.sql` | boşluk, Türkçe karakter |
| 5 | Açılışta zorunlu doğrulama | sessiz "geç" seçeneği |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| SHA-256 | `SHA256.Create()` + `ComputeHash` | aynı API |
| Dosya okuma | `File.ReadAllBytes` | aynı |
| `await using` | yok — klasik `using ( ... ) { }` | yazılabilir |
| Runner nerede koşar | uygulama açılışı | aynı akış |
| Tablo SQL'i | sağlayıcı dialect'i farklı olabilir → **DOĞRULAMA GEREKLI** | aynı |

İki bacakta **aynı checksum algoritması** kullanılır; algoritma TFM'e göre değişmez, değişen yalnız tablo DDL'inin dialect'idir.

## §5 Degisiklik yonetimi

| Durum | Davranış |
|---|---|
| Dosya hiç yok | sırayla uygulanır, satır eklenir |
| Dosya değişmiş | checksum uyuşmaz → açılış durur, dosya geri alınır ya da kayıt düzeltilir |
| Satır var, dosya silinmiş | sıralama bozulur → **dur** |
| Down başarılı | `__Migrations` satırı da silinir |

## §6 Sik hatalar

- Checksum'u tarih/saat bilgisinden üretmek
- Uygulanmış dosyayı "ufak düzeltme" diye değiştirmek
- Migration'ı elle SQL Editor'de çalıştırıp tabloya dokunmamak
- Tabloya ikinci bir isimle ayna açmak
- Yerel saat (`DateTime.Now`) ile `AppliedAt` yazmak

## §7 Dogrulama notlari

`__Migrations` DDL'inin iki sağlayıcıda birebir aynı mı yoksa sağlayıcıya göre ayrıca migration mı yazılacağı netleşmemiştir — **DOĞRULAMA GEREKLI**. Checksum formatının (hex, büyük/küçük harf) tek sözlüklüğü de aynı kararla sabitlenmelidir.

## §8 Kardesler

[migration-script-writing](migration-script-writing.md) · [migration-rollback](migration-rollback.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [audit-columns-convention](audit-columns-convention.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `migration-version-table.md` (kaynak) | tablo seması, 5 adımlı runner akışı, DA-11 kural tablosu, TFM farkları |
| `migration-script-writing.md` | dosya adı biçimi `V<4 hane>__<ad>.sql` ve sıralama kuralı |
| `migration-rollback.md` | geri almanın `__Migrations` satırını da silmesi |
| `transaction-scope-local.md` | migration'ın kendi tek transaction disiplini |
| `sqlite-provider-setup.md` · `sqlserver-provider-setup.md` | DDL dialect farkı ve bacak dağılımı |
