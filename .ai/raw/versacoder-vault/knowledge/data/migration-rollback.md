---
title: "migration-rollback - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Geri alma politikasi: down blogu, irreversible notu, onay + yedek zorunlulugu, veri kaybi onleme (DA-12)"
---

# migration-rollback — Geri Alma Politikası

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [migration-script-writing](migration-script-writing.md)

> **Tek cümlelik sorumluluk:** *Bir migration geri alınabilir mi, alınamıyorsa ne yapılır, veri kaybı nasıl önlenir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Her migration **ya `down` bloğu taşır ya da `-- irreversible` notuyla gerekçelendirilir** (DA-12). Geri alma = `__Migrations` satırının da geri alınması demektir; yalnız SQL'i çalıştırmak yarım bırakır. `DROP`/`TRUNCATE` içeren her down işlemi **onay (G7) + yedek** olmadan çalışmaz. Down, up'ın aynadaki tersidir: veri **düşürmez**, yalnız şemayı eski durumuna getirir (veri taşıma gerekiyorsa ayrı migration yazılır).

---

## §1 Akis tablosu

| Adım | İş | Kanıt |
|---|---|---|
| 1 | Risk değerlendirmesi: veri kaybı var mı? | migration dosyasındaki `down` |
| 2 | Yedek al (SQL Server `BACKUP`, SQLite dosya kopyası) | yedek dosya yolu |
| 3 | Kullanıcı onayı (G7 / SEC-15) | onay kaydı |
| 4 | Uygulamayı durdur | — |
| 5 | `down`'ı sırayla çalıştır, `__Migrations` satırını sil | runner log'u |
| 6 | Doğrulama sorgusu | `SELECT` çıktısı |

---

## §2 Down ornegi

```sql
-- up: kolon ekler (geri alinabilir)
ALTER TABLE Sessions ADD COLUMN ArchivedAt TEXT NULL;

-- down: kolonu dusurur - VERI KAYBI (SQLite'ta kolon silme yeniden kurulum ister)
-- irreversible: SQLite ALTER TABLE DROP COLUMN kisitlidir; geri almanin yolu
--               tabloyu yeni semayla yeniden kurup veriyi taşimaktir.
```

---

## §3 Politika tablosu (DA-12)

| Durum | Karar |
|---|---|
| `CREATE INDEX` / `ADD COLUMN` | `down` yazılır, onaysız geri alınabilir |
| `DROP TABLE` / `DELETE` | yedek + **onay (G7)** zorunlu |
| Şema + veri dönüşü | iki ayrı migration (tek dosyada karıştırma) |
| Down başarısız | uygulama **başlamaz**, log + `NEEDS_VALIDATION` raporu |
| Acil durum | yedekten geri yükleme > elle down (önce yedek) |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Geri alma kodu | `IMigrationRunner` — iki bacakta **aynı** | aynı |
| SQLite down yeteneği | **yok** (sağlayıcı `net48`'de kullanılmaz) | kısıtlı — `DROP COLUMN` sürüm/bağımlı |
| SQL Server down | `ALTER TABLE … DROP COLUMN` | aynı |
| Onay akışı | uygulama seviyesinde, iki bacakta aynı | aynı |

---

## §5 Kardeşler

[migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [transaction-scope-local](transaction-scope-local.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md)
