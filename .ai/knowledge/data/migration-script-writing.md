---
title: "migration-script-writing — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Saf SQL migration yazim kurallari: V0001__ad.sql, idempotent up, down blogu, iki saglayici dialect farki (DA-07 / DA-11 / DA-12)"
---

# migration-script-writing — Migration Yazım Kuralları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [rules index](../../.rules/index.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [migration-version-table](migration-version-table.md) · [migration-rollback](migration-rollback.md)

> **Tek cumlelik sorumluluk:** *Bir migration dosyası nasıl adlandırılır, içinde ne olur ne olmaz, iki sağlayıcıda nasıl taşınır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Migration bir **kod dosyası değil, saf SQL dosyasıdır**: `.sql/migrations/V0001__init.sql` biçiminde adlandırılır, adı sıraya girer, numara atlanmaz. Şema değişikliği **yalnız** bu dosyalarda yapılır (DA-07); uygulama kodunda tek bir `ALTER TABLE` bile bulunmaz. `up` mümkün olduğunca tekrar çalışabilir (`IF NOT EXISTS`) kurulur; yıkıcı her işlem için ya çalıştırılabilir `down` ya da gerekçeli `-- irreversible` notu vardır (DA-12). Uygulanmış dosyaya sonradan dokunulmaz — içerik değişirse checksum denetimi açılışta durur (DA-11).

## §1 Dosya iskeleti

```sql
-- V0002__sessions_title_index.sql
-- up
CREATE INDEX IF NOT EXISTS IX_Sessions_Title ON Sessions (Title);

-- down
DROP INDEX IF EXISTS IX_Sessions_Title;
```

`up` tekrar çalışabilir, `down` up'ın tersidir; ikisi de aynı dosyada, aynı sırayla durur.

## §2 Adlandirma tablosu

| Öğe | Kural | Örnek |
|---|---|---|
| Sıra | `V` + 4 hane; atlanan numara yasak | `V0003__...` |
| Ayraç | `__` çift alt çizgi | `V0003__add-deleted-at.sql` |
| Ad | İngilizce kebab-case, Türkçe karakter yok | `add-deleted-at` |
| Uzantı | `.sql` (Markdown değil) | `.sql/migrations/` |
| İçerik | yalnız DDL/DML; seed bloğu ayrı ve onaylı | `-- seed` ayrı dosya |

## §3 Kural tablosu (DA-07 · DA-11 · DA-12)

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Şema değişikliği yalnız migration'da | kod içinde `ALTER`/`CREATE` çalıştırma |
| 2 | Uygulanmış dosya dokunulmaz | sonradan düzenleme → checksum FAIL |
| 3 | Her dosyada `down` ya da `-- irreversible` gerekçesi | belirsiz yıkıcı dosya |
| 4 | `DROP`/`TRUNCATE` onay (G7) + yedek ile | onaysız yıkım |
| 5 | `up` idempotent (`IF NOT EXISTS`) | ilk çalıştırmada patlayan dosya |

## §4 net48 / net10.0-windows ve dialect

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Dosya kümesi | aynı `.sql` dosyaları | aynı |
| Yürüten kod | `IMigrationRunner` iki bacakta aynı | aynı |
| SQLite dialect | `INTEGER PRIMARY KEY AUTOINCREMENT`, `TEXT` tarih | test bacağında koşar |
| SQL Server dialect | `IDENTITY(1,1)`, `datetime2`, `BIT` | üretim bacağı |
| Fark yönetimi | `#if` yok; runner sağlayıcıya göre dosya seçer | aynı |

## §5 Sizin SQL kalibiniz

| Yer | Davranış |
|---|---|
| Gövde | sabit metin; değerler `@ad` parametresi (bkz. [parameterized-queries](parameterized-queries.md)) |
| Seed/veri | `INSERT` değerleri parametrelenir ya da onaylı ayrı dosyada durur |
| Zaman damgası | UTC ISO-8601, kaynak `TimeProvider` (DA-15) |
| Yıkıcı DDL | onay kapısına bağlanır |

## §6 Sik hatalar

- Numara atlayıp `V0005`'ten `V0007`'ye geçmek
- Uygulanmış dosyayı düzeltmek (checksum ihlali)
- Aynada ters yazılmayan yıkıcı dosya
- Türkçe adlı dosya: `V0004__kolon-ekle.sql`
- Kod içinde `if (!tableExists) RunSql(...)` ile şema değiştirmek

## §7 Dogrulama notlari

Dialekt ayrımının dosya köküyle mi (`migrations/sqlserver/...`), yoksa tek dosyada taşınabilir alt küme mi olacağı karara bağlıdır — **DOĞRULAMA GEREKLI**; iki seçenek de runner'ın tek giriş noktasından yönetilir. `#if` ile SQL gövdesi bölünmez.

## §8 Kardesler

[migration-version-table](migration-version-table.md) · [migration-rollback](migration-rollback.md) · [audit-columns-convention](audit-columns-convention.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [sqlite-provider-setup](sqlite-provider-setup.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `migration-script-writing.md` (kaynak) | dosya iskeleti, adlandırma tablosu, DA-07/11/12 kural tablosu, dialect karşılaştırması |
| `migration-version-table.md` | uygulanmış dosyaya dokunulmazlığı ve checksum sonucu |
| `migration-rollback.md` | `down` / `irreversible` zorunluluğu ve onay kuralı |
| `parameterized-queries.md` | SQL gövdesinde sabit metin, değerde `@ad` |
| `sqlserver-provider-setup.md` · `sqlite-provider-setup.md` | iki sağlayıcının dialect ve bacak farkı |
