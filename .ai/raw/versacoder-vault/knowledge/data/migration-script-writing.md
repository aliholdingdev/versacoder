---
title: "migration-script-writing - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Saf SQL migration yazim kurallari: V0001__ad.sql, idempotent up, down blogu, iki saglayici dialect farki (DA-07 / DA-11)"
---

# migration-script-writing — Migration Yazım Kuralları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [migration-version-table](migration-version-table.md)

> **Tek cümlelik sorumluluk:** *Bir migration dosyası nasıl adlandırılır, içinde ne olur ne olmaz, iki sağlayıcıda nasıl taşınır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Migration = **saf SQL dosyası**, kod değil: `.sql/migrations/V0001__init.sql` biçiminde, **adıyla sıralanır**, numara atlanmaz. Şema değişikliği **yalnız** burada yapılır (DA-07) — uygulama kodunda `ALTER TABLE` yoktur. `up` bloğu mümkün olduğunca **idempotent** (`IF NOT EXISTS`), yıkıcı her işlem için ya `down` bloğu ya da `-- irreversible` gerekçesi vardır (DA-12).

---

## §1 Dosya iskeleti

```sql
-- V0002__sessions_title_index.sql
-- up
CREATE INDEX IF NOT EXISTS IX_Sessions_Title ON Sessions (Title);

-- down
DROP INDEX IF EXISTS IX_Sessions_Title;
```

---

## §2 Dosya adi tablosu

| Öğe | Kural | Örnek |
|---|---|---|
| Sıra | `V` + 4 hane, atlanan numara **yasak** | `V0003__…` |
| Ayraç | `__` (çift alt çizgi) | `V0003__add-deleted-at.sql` |
| Ad | İngilizce kebab-case, Türkçe karakter yok | `add-deleted-at` |
| Uzantı | `.sql` (MD değil) | `.sql/migrations/` |
| İçerik | yalnız DDL/DML saf SQL | `-- seed` bloğu ayrı ve onaylı |

---

## §3 Kural tablosu (DA-07 · DA-11 · DA-12)

| # | Kural | Yasak |
|---|---|---|
| 1 | Şema değişikliği **yalnız** migration'da | kodda `ALTER`/`CREATE` çalıştırma |
| 2 | Uygulanmış dosya **dokunulmaz** | sonradan düzenleme → checksum FAIL |
| 3 | `down` bloğu ya da `-- irreversible` notu | belirsiz yıkıcı dosya |
| 4 | Yıkıcı migration (`DROP`, `TRUNCATE`) onaysız çalışmaz | G7/SEC-15 ihlali |
| 5 | `IF NOT EXISTS` ile tekrar çalışabilir `up` | ilk çalıştırmada patlayan dosya |

---

## §4 net48 / net10.0-windows farkı ve dialect

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Dosyalar | **aynı** `.sql` dosyaları | aynı |
| Yürüten kod | `IMigrationRunner` (her iki bacak) | aynı |
| SQLite dialect | `INTEGER PRIMARY KEY AUTOINCREMENT`, `TEXT` tarih | test bacağında çalışır |
| SQL Server dialect | `IDENTITY(1,1)`, `datetime2`, `BIT` | üretim bacağı |
| Fark yönetimi | `#if` **yok** — runner provider'a göre dosya seçer | aynı |

> **NEEDS_VALIDATION:** Dialekt ayrımı dosya köküyle mi (`migrations/sqlserver/…`) yoksa tek dosyada taşınabilir alt küme mi — **ADR-012 kapsamında `architect` kararıdır**; PART 5'te `dotnet restore` sonrası netleşir.

---

## §5 Kardeşler

[migration-version-table](migration-version-table.md) · [migration-rollback](migration-rollback.md) · [audit-columns-convention](audit-columns-convention.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [sqlite-provider-setup](sqlite-provider-setup.md)
