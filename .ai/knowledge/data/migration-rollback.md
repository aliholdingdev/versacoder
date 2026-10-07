---
title: "migration-rollback — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Geri alma politikasi: down blogu, irreversible notu, onay + yedek zorunlulugu, veri kaybi onleme (DA-12)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# migration-rollback — Geri Alma Politikası

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [rules index](../../.rules/index.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [transaction-scope-local](transaction-scope-local.md)
**Küme + Hub:** [data index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (data · 7/25):** [ado-net-command](ado-net-command.md) · [ado-net-connection](ado-net-connection.md) · [audit-columns-convention](audit-columns-convention.md) · [command-timeout-policy](command-timeout-policy.md) · [concurrency-optimistic](concurrency-optimistic.md) · [connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [dapper-query-mapping](dapper-query-mapping.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [parameterized-queries](parameterized-queries.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sql-injection-defense](sql-injection-defense.md)

> **Tek cumlelik sorumluluk:** *Bir migration geri alınabilir mi, alınamıyorsa ne yapılır, veri kaybı nasıl önlenir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Her migration ya çalıştırılabilir bir `down` bloğu taşır, ya da `-- irreversible` ibaresini **gerekçesiyle** yazar (DA-12); ikisiz dosya vault'a girmez. Geri alma, SQL'i çevirmekten ibaret değildir: `__Migrations` satırı da geri alınır, çünkü sürüm tablosu ile dosya arasındaki uyum açılışta yeniden denetlenir. `DROP`/`TRUNCATE` barındıran her down adımı **yedek + onay (G7)** olmadan çalışmaz. Down, up'ın aynadaki tersidir: veri düşürmez, yalnız şemayı eski durumuna getirir; veri taşıma gerekiyorsa o iş ayrı bir migration'a bırakılır.

## §1 Geri alma adimlari

| # | Yapılacak iş | Beklenen kanıt |
|---|---|---|
| 1 | `down` okunur, veri kaybı riski çıkarılır | risk değerlendirmesi |
| 2 | Yedek alınır: SQL Server `BACKUP`, SQLite dosya kopyası | yedek dosya yolu |
| 3 | Onay alınır (G7 / SEC-15) | onay kaydı |
| 4 | Uygulama durdurulur | süreç kaydı |
| 5 | `down` sırayla çalıştırılır, `__Migrations` satırı silinir | runner log'u |
| 6 | Doğrulama sorgusu çalıştırılır | `SELECT` çıktısı |

Adım 2 ve 3 **atlanamaz**: sırası değişmez, yedek olmadan onay, onaysız yedek kabul edilmez.

## §2 Ornek: up, down ve not

```sql
-- V0004__sessions_archived_at.sql
-- up: geri alinabilir kolon ekleme
ALTER TABLE Sessions ADD COLUMN ArchivedAt TEXT NULL;

-- down: kolonu atar -> veri kaybi.
-- irreversible: SQLite'ta ALTER TABLE DROP COLUMN sinirlidir; geri almanin
-- yolu tabloyu yeni semayla kurup veriyi tasiyan ayri bir migrationdir.
DROP INDEX IF EXISTS IX_Sessions_ArchivedAt;
```

Yıkıcı bir down, yedek yoksa **çalıştırılmaz**; dosyadaki not, geri alınamazlığın sebebini tek satırda söyler.

## §3 Politika tablosu (DA-12)

| Durum | Karar |
|---|---|
| `CREATE INDEX` / `ADD COLUMN` | `down` yazılır; onaysız geri alınabilir |
| `DROP TABLE` / `DELETE` | yedek **ve** onay (G7) zorunlu |
| Şema + veri dönüşü | iki ayrı migration; tek dosyada karıştırılmaz |
| Down başarısız | uygulama başlamaz; log + `NEEDS_VALIDATION` raporu açılır |
| Acil durum | önce yedekten geri yükleme; elle down yalnız yedekten sonra |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Geri alan kod | `IMigrationRunner` — iki bacakta aynı | aynı |
| SQLite down yeteneği | sağlayıcı bu bacakta kullanılmaz | kısıtlı; `DROP COLUMN` sürüm ve sınıra bağlı |
| SQL Server down | `ALTER TABLE ... DROP COLUMN` | aynı |
| Onay akışı | uygulama seviyesinde, bacak bağımsız | aynı |

İki bacak farklı SQL yazmaz; fark yalnız sağlayıcının neyi desteklediğinde ortaya çıkar.

## §5 Hata senaryolari

| Senaryo | Sonuç | Yapılacak |
|---|---|---|
| `__Migrations` satırı unutulmuş | sürüm tablosu dosyayla çelişir | satırı sil, doğrulama sorgusunu çalıştır |
| Down yarım kaldı | şema iki durum arasında | yedekten geri yükle (elle tamamlama değil) |
| Yedek alınmadan onay | G7 ihlali | işlem durdurulur |
| Veri taşıması gereken down | veri kaybı | taşıma migration'ı yazılır, tek dosyada yapılmaz |

## §6 Sik hatalar

- Sadece SQL çalıştırıp `__Migrations` satırını bırakmak
- Down'u üretimde ilk kez denemek (yedeksiz)
- Veri silmeyi "down" sanıp tek dosyada şema + veri değiştirmek
- `-- irreversible` ibaresini gerekçesiz yazmak
- Down hatasında uygulamayı yine de başlatmak

## §7 Dogrulama notlari

SQLite kolon atma desteği sürüme ve tablo yapısına bağlıdır; vault bunu **DOĞRULAMA GEREKLI** olarak işaretler ve taşınabilir tek yol olarak veriyi kopyalayan ayrı migration'ı gösterir. Dialekt ayrımının dosya köküyle mi yapıldığı (ör. `migrations/sqlserver/...`) yoksa tek dosyada taşınabilir alt küme mi kullanılacağı mimari karardır — **DOĞRULAMA GEREKLI**.

## §8 Kardesler

[migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [transaction-scope-local](transaction-scope-local.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `migration-rollback.md` (kaynak) | 6 adımlı geri alma akışı, down örneği, DA-12 politika tablosu, TFM farkları |
| `migration-script-writing.md` | her dosyada `down` ya da `-- irreversible` zorunluluğu |
| `migration-version-table.md` | geri almanın `__Migrations` satırını da kapsaması, fail-fast denetimi |
| `sqlite-provider-setup.md` | SQLite dalının yalnız net10 test bacağında oluşu |
| `sqlserver-provider-setup.md` | SQL Server `BACKUP` ile yedek yolu |
