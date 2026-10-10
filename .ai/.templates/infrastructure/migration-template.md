---
title: "VersaCoder — Şema Migration Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "sema migration sablonu: hedef surum, SQL adimlari ve geri alma notu"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Şema Migration Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/index]] · [[.templates/query/Query-Template]] · [[.templates/infrastructure/dapper-mapping-template]] · [[.templates/adr/adr-data-template]] · [[architecture/index]]

---

## §1 Amaç

### §1.1 Neden bu şablon gerekli

Migration, **geri alınabilir bir şema kararıdır**: hangi tabloyu açtığı, hangi sürümle imzalandığı, ikinci çalıştırmada ne yapacağı ve geri alındığında veriyi koruyup korumayacağı burada yazılıdır. VersaCoder'da migration **saf `.sql` dosyasıdır**; çalıştırıcı `schema_version` tablosunu okur ve sırayla uygular (EF Core migration **yoktur**, ADR-004). Şablonsuz yazılan script; **idempotent olmayan**, **down'sız**, **yedeğiz** girer ve ilk tekrar çalıştırmada veritabanını bozar.

| Alan | Değer |
|------|-------|
| Template Name | `migration-template.md` |
| Template Path | `.ai/.templates/infrastructure/migration-template.md` |
| Hedef Dosya | `db/migrations/V0xx__<slug>.sql` + `.down.sql` (şablon: `.ai/migration/V0XX-*.md`) |
| Hedef Veritabanı | `versacoder.db` (SQLite, birincil) · SQL Server (ikincil, `schemaversion` uyumu) |
| Sürümleme Tablosu | `schema_version (version INTEGER PK, name TEXT, applied_at TEXT, checksum TEXT)` |
| Dosya Adı | `V014__add_session_index.sql` — `V` + 3 hane + `__` + kebab-case |
| İdempotentlik | `CREATE TABLE IF NOT EXISTS` / `CREATE INDEX IF NOT EXISTS` — **zorunlu** |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK · dosya sonu tek yeni satır |
| Sorumlu | `database-optimizer` (yazım) · `reviewer` (denetim) · `security` (yedek/rollback) |
| Versiyon | 1.0.0 · Authority: SSOT · Bootstrap: 2026-09-25 |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

Ölçüm tarihi: **2026-09-25**, kök `C:\versacoder`.

| # | İddia | Disk Kanıtı | Durum |
|---|-------|-------------|-------|
| E1 | `.ai/.templates/` = 3 klasör + 3 kök dosya (12 MD) | `adr`=6, `agents`=2, `architecture`=1, kök=3 | ✅ Doğrulandı |
| E2 | `.ai/.templates/infrastructure/` klasörü | bu dosya yazıldığı anda oluşur | ✅ Doğrulandı (bu dosya) |
| E3 | `db/migrations/*.sql` dosyaları | `db/` klasörü **YOK** | ⚠️ VERIFICATION REQUIRED |
| E4 | `versacoder.db` + `schema_version` tablosu | `*.db` dosyası bulunamadı | ⚠️ VERIFICATION REQUIRED |
| E5 | `src/` katmanları (Domain…Host/Desktop/Cli) | `src/` klasörü **YOK** | ⚠️ VERIFICATION REQUIRED |
| E6 | `.ai/architecture/index.md`, `.ai/.rules/`, `.ai/keys.md` | `.ai\` altında yalnız `.templates` | ⚠️ VERIFICATION REQUIRED |
| E7 | ADO.NET + Dapper, EF Core yasak (ADR-004) | `.csproj`/paket kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E8 | `Microsoft.Data.Sqlite` / `SqlClient` sağlayıcı | paket kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E9 | Yedek dosyası (`versacoder.db.bak`) politikası | diskte yedek yok | ⚠️ VERIFICATION REQUIRED |

**Yorum yasağı:** `⚠️ VERIFICATION REQUIRED` satırına "yoktur, eninde sonunda gelecek" gibi yorum yazılmaz; yalnız §5.3 komutunun **çıktısı** satırı ✅'ye çeker. Var olmayan dosya için "şurada" denmez; yol + komut yazılır.

### §1.3 Migration Türleri

| Tür | Ne Yapar | Örnek | Zorunlu Çift |
|-----|----------|-------|---------------|
| **M1 Şema ekleme** | `CREATE TABLE IF NOT EXISTS` | `session` tablosu | up + down |
| **M2 Şema genişletme** | `ALTER TABLE ADD COLUMN` | `session.tag TEXT` | up + down |
| **M3 Kısıt/index** | `CREATE UNIQUE INDEX IF NOT EXISTS` | `idx_user_email` | up + down |
| **M4 Veri backfill** | tek seferlik `UPDATE … WHERE col IS NULL` | `status=0` doldurma | up + **idempotent** |
| **M5 Kolon daraltma** | expand-and-contract adımı 2 | eski kolonu silme | yalnız contract fazı |
| **M6 Seed** | `INSERT … WHERE NOT EXISTS` | `schema_version`/demo satır | idempotent |
| **M7 Drop (tehlikeli)** | tablo/kolon silme | — | yedek zorunlu, `security` onayı |

### §1.4 Neden İdempotent + Neden Çift (up/down)

| Kural | Neden | İhlal Sonucu |
|-------|-------|--------------|
| `IF NOT EXISTS` her `CREATE`'te | script 2. kez çalışır (`retry`, yeni makine) | `table already exists` → kurulum çöker |
| `UPDATE … WHERE col IS NULL` | backfill tekrar uygulanır | ikinci çalıştırma veriyi sıfırlar |
| Her up'a bir down | geri alma planı (§3.8) | DB kilitli kalır, sürüm atlanır |
| Sıralı `V0xx` numarası | iki script aynı numarayı alamaz | yarım şema |
| `schema_version` satırı + checksum | hangisinin uygulandığı bilinir | drift (şema ≠ script) |
| Yedek önce | geri dönüşü olmayan `DROP` var | veri kaybı (§7.2 R1) |

---

## §2 Kapsam

**İçinde:** hedef DB tanımı, `schema_version` sürümlemesi, idempotent script kuralı, up/down çifti, expand-and-contract, veri backfill, sıralı script listesi, rollback planı, yedekleme zorunluluğu, lock/döngü riskleri, salt-okunur doğrulama, kabul kriterleri.
**Dışında:** sorgu metni (→ `../query/Query-Template`), C# eşleme (→ `dapper-mapping-template`), bağlantı stringi/secret (→ `keys.md`), DBA sunucu kurulumu, backup altyapısı politikası.

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya Tipi | Sorumlu | Bu Şablon? |
|---|------------|---------|-------------|
| 1 | `db/migrations/V0xx__*.sql` (up) | `database-optimizer` | ✅ |
| 2 | `db/migrations/V0xx__*.down.sql` (down) | `database-optimizer` | ✅ |
| 3 | `.ai/migration/V0XX-<slug>.md` (plan/geri alma) | `database-optimizer` | ✅ (bu şablon) |
| 4 | `schema_version` okuma/çalıştırıcı kodu | `developer` | ❌ kod, §6.5 K3 kanıtı |
| 5 | Sorgu/rapor (`SELECT`) | `database-optimizer` | ❌ → `../query/Query-Template` |
| 6 | Dapper kolon eşlemesi | `database-optimizer` | ❌ → `dapper-mapping-template` |
| 7 | `.ai/adr/ADR-NNN-data-*.md` | `architect` | ❌ (karar ≠ script) |
| 8 | `.ai/.rules/*.md` | `reviewer` | ❌ (kural ≠ script) |

### §2.2 Kapsam Sınırı Testi

| # | Soru | Beklenen | Uymazsa |
|---|------|----------|---------|
| S1 | Veritabanı **şemasını** değiştiriyor mu (tablo/kolon/index/kısıt)? | `EVET` | yalnız veri güncelleme → yine bu şablon (M4) ama §3.6 zorunlu |
| S2 | `V0xx` numarası atanabiliyor mu (sıradaki boş numara)? | `EVET` | numara çakışması → **DUR** (§5.3/3) |
| S3 | Her `CREATE`/`ALTER` için geri alma (down) yazılabiliyor mu? | `EVET` | yazılamıyorsa M7 → `security` onayı |
| S4 | Yedek alınabiliyor mu (§3.8)? | `EVET` | `HAYIR` → **DUR**, script çalışmaz |
| S5 | Ana konu sorgu performansı mı? | `HAYIR` | `../query/Query-Template` |

---

## §3 Mimari

### §3.1 Migration Künyesi

```yaml
# .ai/migration/V0XX-<slug>.md — ilk blok
migration_id: V014
slug: add_session_index
target: versacoder.db            # sqlite | sqlserver
type: M3                        # §1.3
up_file:   db/migrations/V014__add_session_index.sql
down_file: db/migrations/V014__add_session_index.down.sql
idempotent: true
backup_required: true
estimated_rows: 0                # etkilenecek satır (M4/M5 için)
lock_risk: low                   # low | medium | high
status: draft | applied | reverted
```

| Alan | Zorunlu | Örnek | Eksikse |
|------|---------|-------|---------|
| `migration_id` | Evet | `V014` | RED |
| `target` | Evet | `versacoder.db` | RED |
| `up_file` / `down_file` | Evet | `V014__*.sql` | RED (§4.5 G2) |
| `idempotent` | Evet | `true` | RED |
| `backup_required` | Evet | `true` | RED |
| `lock_risk` | Evet | `low` | RED |

### §3.2 Hedef DB ve Sürümleme (`schema_version`)

```text
-- Kurulum scripti (idempotent, tek seferlik tanım)
CREATE TABLE IF NOT EXISTS schema_version (
    version     INTEGER    NOT NULL PRIMARY KEY,
    name        TEXT       NOT NULL,
    applied_at  TEXT       NOT NULL,
    checksum    TEXT       NOT NULL
);
```

| Alan | Tip | Anlam | Doğrulama |
|------|-----|-------|-----------|
| `version` | `INTEGER` PK | `V014` → `14` | artan, boşluk yok |
| `name` | `TEXT` | dosya slug'ı | benzersiz |
| `applied_at` | `TEXT` | ISO-8601 UTC | `strftime('%Y-%m-%dT%H:%M:%SZ')` |
| `checksum` | `TEXT` | SHA-256 (script metni) | drift tespiti |

| Sağlayıcı | Sürüm Tablosu | Okuma Sorgusu | Not |
|-----------|---------------|----------------|-----|
| SQLite (`versacoder.db`) | `schema_version` | `SELECT MAX(version) FROM schema_version;` | birincil |
| SQL Server | `schemaversion` | `SELECT MAX(version) FROM schemaversion;` | ikincil uyum |

**Tek doğruluk kaynağı:** sürüm her zaman `schema_version`'daki **MAX(version)**'dir; script dosyası adı değil.

### §3.3 Idempotent Script Kalıbı (up)

```text
-- {{V0XX}}__{{SLUG}}.sql    hedef: {{VERSACODER_DB}}
-- idempotent: EVET   backup: EVET   lock_risk: {{LOW_MEDIUM_HIGH}}

BEGIN;

CREATE TABLE IF NOT EXISTS {{TABLO_ADI}} (
    {{KOLON_1}}  {{TIP_1}} NOT NULL,
    {{KOLON_2}}  {{TIP_2}} NULL,
    PRIMARY KEY ({{PK}})
);

CREATE INDEX IF NOT EXISTS {{IDX_ADI}}
    ON {{TABLO_ADI}} ({{SUTUN_1}}, {{SUTUN_2}});

-- backfill (M4): ikinci çalıştırmada veriyi bozmaz
UPDATE {{TABLO_ADI}}
   SET {{KOLON_2}} = {{VARSAYILAN_DEGER}}
 WHERE {{KOLON_2}} IS NULL;

INSERT OR IGNORE INTO schema_version (version, name, applied_at, checksum)
VALUES ({{VNO}}, '{{SLUG}}', strftime('%Y-%m-%dT%H:%M:%SZ','now'), '{{SHA256}}');

COMMIT;
```

**Yasaklar:** `DROP TABLE` (down'suz), `ALTER`'ı tekrarlanamaz yapan sözdizimi, `IF NOT EXISTS`'siz `CREATE`, script içinde `BEGIN` dışında transaction, kullanıcı girdisiyle derlenen ad.

### §3.4 Up / Down Çifti

| # | Up (ileri) | Down (geri) | Not |
|---|------------|-------------|-----|
| 1 | `CREATE TABLE IF NOT EXISTS x` | `DROP TABLE IF EXISTS x` | down'da veri kaybı bilinçli → §3.8 |
| 2 | `ALTER TABLE x ADD COLUMN c` | `ALTER TABLE x DROP COLUMN c` | down, taşınan veriyi siler |
| 3 | `CREATE INDEX IF NOT EXISTS i` | `DROP INDEX IF EXISTS i` | güvenli |
| 4 | `UPDATE x SET c=1 WHERE c IS NULL` | `UPDATE x SET c=NULL WHERE c=1` | yalnız M4 |
| 5 | `INSERT OR IGNORE … seed` | `DELETE … WHERE id IN (seed)` | seed listesi sabit |

**Kural:** down her zaman **aynı dosyanın yanındaki** `*.down.sql` dosyasıdır; ayrı bir "geri alma planı" dosyası olarak **yazılmaz** (iki gerçek kaynağı doğurur, SSUT).

### §3.5 Expand-and-Contract

| Faz | Ne Yapılır | Script | Yayıncı Sürüm | Risk |
|-----|-----------|--------|----------------|------|
| **1 Expand** | yeni kolon/index **eklenir**, eski kullanılır | `V014` (up) | N | düşük |
| **2 Migrate** | backfill + kod iki okuma yapar | `V015` (up) | N+1 | orta |
| **3 Contract** | eski kolon kaldırılır | `V016` (up) | N+2 | **yüksek — yalnız 2 sürüm sonra** |

```text
+----------------------------------------------------------------+
| Faz 1 EXPAND    ALTER ADD COLUMN / yeni index (V014)           |
|    v                                                           |
| Faz 2 MIGRATE   backfill UPDATE … WHERE col IS NULL (V015)     |
|    v                                                           |
| GATE            <== KAPI: N+1 sürüm yayında ve geri dönüş yok  |
|    v                                                           |
| Faz 3 CONTRACT  DROP eski kolon (V016) + yedek zorunlu         |
|    v                                                           |
| schema_version  MAX(version) = 16 → drift yok                  |
+----------------------------------------------------------------+
```

**Ne anlatır:** değişimin 3 fazda yürümesi; kapalı fazda (CONTRACT) eski kolon **asla aynı script'te** düşürülmez.
**Doğrulama:** §5.3/1 — satır uzunlukları 66'ya eşit olmalı.

### §3.6 Veri Backfill

| Kural | Yazım | Neden |
|-------|-------|-------|
| Tekrar güvenli | `WHERE kolon IS NULL` | script 2. kez koşar |
| Partiyel | `WHERE id IN (SELECT … LIMIT 1000)` + döngü | kilit süresi (§3.8) |
| Sayım öncesi | `SELECT COUNT(*)` → tahmin | `estimated_rows` dolar |
| Transaction | her parti ayrı `BEGIN/COMMIT` | uzun kilit yok |
| Geri alma | down, `SET kolon = NULL WHERE ...` | §3.4 #4 |
| Doğrulama | `SELECT COUNT(*) WHERE kolon IS NULL` = 0 | §6.5 K5 |

**Backfill tablosu (plan):**

| Adım | İş | Beklenen | Bütçe |
|------|----|----------|-------|
| 1 | Yedek al | `.bak` boyutu > 0 | 30 sn |
| 2 | Etkilenecek satır sayısı | `COUNT` yaz | 5 sn |
| 3 | Parti 1 (1000 satır) | 0 hata | 2 sn |
| 4 | Kalan partiler | 0 hata | ≤ 60 sn |
| 5 | Doğrulama sorgusu | `IS NULL` = 0 | 5 sn |

### §3.7 Sıralı Script Listesi

| # | Dosya | Version | Tür | İdempotent | Down | Durum |
|---|-------|---------|-----|------------|------|-------|
| 1 | `V001__init_schema.sql` | 1 | M1 | ✅ | ✅ | ⚠️ VERIFICATION REQUIRED |
| 2 | `V002__seed_roles.sql` | 2 | M6 | ✅ | ✅ | ⚠️ VERIFICATION REQUIRED |
| 3 | `V014__add_session_index.sql` | 14 | M3 | ✅ | ✅ | bu şablon |
| 4 | `V015__backfill_session_status.sql` | 15 | M4 | ✅ | ✅ | planlı |
| 5 | `V016__drop_legacy_tag.sql` | 16 | M7 | ✅ | ⚠️ | planlı (yedek zorunlu) |

**Sıra kuralları:** numara **boşluklu artmaz** (1,2,4 yasak); uygulama sırası dosya adı ≠ `schema_version`; uygulanmamış script varsa uygulama **durmaz** yalnız raporlar; iki script aynı numarayı alırsa **DUR** (§5.3/3).

### §3.8 Yedekleme, Rollback ve Lock (Döngü) Riskleri

**Yedekleme zorunluluğu:**

| Koşul | Yedek | Yedek Yoksa |
|-------|-------|-------------|
| Her `M7 DROP` | **zorunlu** | **DUR** (§4.5 G6) |
| `estimated_rows > 1000` | **zorunlu** | **DUR** |
| Yalnız `CREATE INDEX` | önerilir | serbest + not |
| Contract fazı | **zorunlu** | **DUR** |

**Rollback planı:**

| # | Adım | Komut/İş | Süre |
|---|------|----------|------|
| 1 | Hata tespiti | `SELECT MAX(version) FROM schema_version;` | 5 sn |
| 2 | Uygulamayı durdur | yazma trafiği kapatılır | 30 sn |
| 3 | Down script | `V0xx__*.down.sql` sırayla (tersten) | 1 dk |
| 4 | Doğrulama | `PRAGMA table_info` + satır sayısı | 30 sn |
| 5 | Yedekten dönüş | yalnız down yoksa; `.bak` geri yüklenir | 5 dk |
| 6 | Kayıt | `status: reverted` + `log.md` | 5 dk |

**Lock / döngü riskleri:**

| Risk | Belirti | Önlem |
|------|---------|-------|
| Uzun transaction | `database is locked` | parti büyüklüğü ≤ 1000 |
| Writer + okuyucu döngüsü | UI yazamıyor | migration öncesi yazma kapatılır |
| İki runner eşzamanlı | çift uygulama | tek process kilidi (`BEGIN IMMEDIATE`) |
| Contract + eski sürüm | eski kod okuyamıyor | 2 sürüm gecikme (§3.5) |
| Yarım script | `COMMIT`'e ulaşılmadı | transaction + `BEGIN` |

### §3.9 Dolu Örnek ve Tam Kalıp

**Dolu örnek (`V014`, gerçek VersaCoder planı):**

```text
V014__add_session_index.sql   target=versacoder.db   type=M3
Schema:  session (session_id PK, user_id INT, created_at TEXT, status INT)
Up:      CREATE INDEX IF NOT EXISTS idx_session_user_created
             ON session (user_id, created_at);
Down:    DROP INDEX IF EXISTS idx_session_user_created;
Sürüm:   INSERT OR IGNORE INTO schema_version(version,name,applied_at,checksum)
             VALUES (14,'add_session_index', ..., '<sha256>');
Etki:    estimated_rows=0  lock_risk=low  backup=önerilir
Doğrulu.: PRAGMA index_list('session') -> idx_session_user_created
```

**Tam kalıp (`.ai/migration/V0XX-<slug>.md` için `{{…}}` doldurulur):**

````markdown
---
title: "V0XX — {{SLUG}}"
type: migration
category: infrastructure
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: primary
---
# V0XX — {{SLUG}}

**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/query/Query-Template]] · [[.templates/infrastructure/dapper-mapping-template]] · [[.templates/adr/adr-data-template]] · [[architecture/index]] · [[.templates/index]]

## §1 Amaç — {{BIR_CUMLE}}
## §2 Künye
| Alan | Değer |
|---|---|
| target | {{versacoder_db \| sqlserver}} |
| type | {{M1..M7}} |
| idempotent | true |
| backup_required | {{true\|false}} |
## §3 Up SQL
```text
{{UP_SQL}}
```
## §4 Down SQL
```text
{{DOWN_SQL}}
```
## §5 Doğrulama + Rollback — {{KOMUTLAR}} · {{ROLLBACK_ADIMLARI}}
## §6 Kabul Kriterleri — {{K1}} … {{K8}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Çıktı | Atlanırsa |
|---|------|-------|-----------|
| 1 | Sıradaki `V0xx` numarasını al | §3.1 `migration_id` | numara çakışması |
| 2 | Türü seç (M1–M7) | §1.3 | risk bilinmez |
| 3 | Hedef DB + `schema_version` yaz | §3.2 | sürüm okunamaz |
| 4 | Idempotent up yaz | §3.3 | 2. çalıştırma hata |
| 5 | Down çiftini yaz | §3.4 | geri alınamaz |
| 6 | Expand-and-contract gerekçesi | §3.5 | erken silme |
| 7 | Backfill + sayım | §3.6 | uzun kilit |
| 8 | Yedek + rollback + lock | §3.8 | veri kaybı |
| 9 | Doğrulama + Red Team | §5.3, §7.2 | kanıtsız onay |

### §4.2 Onay Akışı

| Aşama | Rol | Ne Onaylar | Süre |
|-------|-----|-----------|------|
| 1 | `database-optimizer` | script + §5.3 çıktısı | 25 dk |
| 2 | `reviewer` | §6.1 (17/17) + idempotent | 10 dk |
| 3 | `security` | M7 yedek + rollback (§3.8) | 10 dk |
| 4 | `performance-engineer` | lock riski + parti büyüklüğü | 8 dk |
| 5 | `master-orchestrator` | Quality Report → `log.md` | 5 dk |

### §4.3 Ölçek ve Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Dosya uzunluğu | 480–660 satır | dışı RED |
| Dosya boyutu | 15–32 KB | dışı RED |
| Tablo satırı | ≥ 25 (her §'de tablo) | eksik RED |
| Versiyon aralığı | `V001..V999`, boşluksuz | çakışma = DUR |
| Parti büyüklüğü | ≤ 1000 satır | kilit RED |
| Contract gecikmesi | ≥ 2 sürüm | erken silme RED |
| Kodlama | UTF-8 BOM'suz | Latin-1 RED |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm | Zorunlu Güncelleme |
|-----------|-------|---------------------|
| Script uygulandı | `minor` | `status: applied` + `log.md` |
| Down eklendi | `minor` | §3.4 tablosu |
| Sadece yazım | `patch` | `updated` |
| Script geri alındı | `major` | `status: reverted` + §3.8 kaydı |
| Sıra numarası değişti | `major` | §3.7 tablosu + tüm alt dosyalar |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu | Tespit |
|---|-----------|--------------|--------|
| G1 | Başka vault'tan içerik kopyalanması | RED + sil | §7.2 R10 |
| G2 | `down` dosyasız up | **DUR** | §5.3/2 |
| G3 | `CREATE`'te `IF NOT EXISTS` yok | **DUR** | §5.3/2 |
| G4 | Backfill'de `WHERE` yok | **DUR** | §5.3/4 |
| G5 | `DROP TABLE/DATABASE` yedeğiz | **DUR** | §4.5 + `security` |
| G6 | Aynı `V0xx` numarası iki dosyada | **DUR** | §5.3/3 |
| G7 | `schema_version` satırı yok | RED | §6.5 K3 |
| G8 | EF Core migration referansı | RED (ADR-004) | `Select-String EntityFramework` |
| G9 | Contract fazı aynı script'te | RED | §3.5 |
| G10 | Uzun transaction (partisiz) | RED | §3.8 lock |
| G11 | Kanıtsız "uygulandı" iddiası | **DUR** | §5.3/5-6 çıktısı |
| G12 | Mojibake / Türkçe bozuk | RED | §5.3/5 |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Numara ata | `schema_version` MAX | `V0xx` | database-optimizer | 5 dk |
| 2 | Tür + risk | değişiklik | M1–M7 + lock | database-optimizer | 5 dk |
| 3 | Yedek planı | §3.8 | `.bak` adı | security | 5 dk |
| 4 | Up script | §3.3 kalıp | `V0xx__*.sql` | database-optimizer | 15 dk |
| 5 | Down script | §3.4 | `*.down.sql` | database-optimizer | 10 dk |
| 6 | Backfill + parti | §3.6 | plan tablosu | database-optimizer | 10 dk |
| 7 | Rollback + lock | §3.8 | 6 adım | security | 10 dk |
| 8 | Doğrulama | §5.3 (6 komut) | 0 ihlal | database-optimizer | 5 dk |
| 9 | Onay + kayıt | §6.1 + `log.md` | Quality Report | reviewer | 5 dk |
| | **Toplam** | | | | **≈ 70 dk** |

### §5.2 Hata Modları

| # | Hata | Belirti | Kök Neden | Düzeltme |
|---|------|---------|-----------|----------|
| 1 | `table already exists` | 2. çalıştırma | `IF NOT EXISTS` yok | §3.3 |
| 2 | Yarım şema | script ortada koptu | transaction yok | `BEGIN/COMMIT` |
| 3 | Çift uygulama | iki runner | eşzamanlılık | `BEGIN IMMEDIATE` kilidi |
| 4 | Veri kaybı | satır sayısı düştü | yedeksiz `DROP` | §3.8 zorunlu yedek |
| 5 | Drift | script ≠ DB | `schema_version` yok | checksum + okuma |
| 6 | Kilit | `database is locked` | parti çok büyük | ≤ 1000 satır |
| 7 | Geri alınamıyor | down eksik | §3.4 atlanmış | down yazılır |
| 8 | Erken contract | eski kod hata | 2 sürüm kuralı yok | §3.5 GATE |

### §5.3 Salt Okunur Doğrulama Komutları

Aşağıdaki 6 komut **hiçbir dosyayı/değeri değiştirmez** (salt okunur).

```powershell
# 1) ÖLÇÜM — 480-660 satır ve 15-32 KB; kutu varsa 66 karakter hizası
$p = 'C:\versacoder\.ai\.templates\infrastructure\migration-template.md'
$t = [System.IO.File]::ReadAllText($p)
"satir={0} (480-660)  kb={1} (15-32)" -f (($t -split "`n").Count), [math]::Round((Get-Item $p).Length/1KB,1)
Get-Content -LiteralPath $p | Where-Object { $_ -match '^[+|]' -and $_.Length -ne 66 } |
  ForEach-Object { "HIZASIZ: " + $_.Length }
```

```powershell
# 2) IDEMPOTENT + DOWN TARAYICISI — 'IF NOT EXISTS' her CREATE'te, down dosyası var mı
$sql = Get-ChildItem 'C:\versacoder\db\migrations' -Filter *.sql -ErrorAction SilentlyContinue
"dosya={0}  ifnotexists_eksik={1}" -f $sql.Count,
  (($sql | Select-String -Pattern 'CREATE\s+(TABLE|INDEX|UNIQUE)' | Where-Object {
      $_.Line -notmatch 'IF NOT EXISTS' }).Count)
```

```powershell
# 3) SIRA NUMARASI ÇAKIŞMASI — benzersiz versiyon sayısı = dosya sayısı olmalı
Get-ChildItem 'C:\versacoder\db\migrations' -Filter *.sql -ErrorAction SilentlyContinue |
  ForEach-Object { if ($_.Name -match '^V(\d+)__') { [int]$Matches[1] } } |
  Group-Object | Where-Object { $_.Count -gt 1 } | Format-Table -AutoSize
```

```powershell
# 4) TEHLİKELİ İFADE TARAYICISI — DROP/DELETE dışarıda kalmamalı (gerekçe zorunlu)
Select-String -Path 'C:\versacoder\db\migrations\*.sql' `
  -Pattern 'DROP\s+TABLE|DROP\s+DATABASE|TRUNCATE|DELETE\s+FROM' -ErrorAction SilentlyContinue |
  Select-Object Filename, LineNumber, Line
```

```powershell
# 5) DOTNET + KODLAMA (salt okunur — derleme/yazma YOK)
dotnet --list-sdks
dotnet --info
$m = Select-String -Path $p -Pattern '[\u00C3\u00C2\u00C4\uFFFD]' | Measure-Object   # mojibake = 0
Select-String -Path $p -Pattern 'EntityFramework' | Measure-Object # EF = 0 (ADR-004)
```

```powershell
# 6) SQLITE SALT OKUNUR — PRAGMA table_info + schema_version okuma + satır sayısı
$db = 'C:\versacoder\db\versacoder.db'
if (Test-Path $db) {
  $ro = "file:$($db -replace '\\','/')?mode=ro"
  sqlite3 $ro "PRAGMA table_info(session);"                 # sütun sayısı/tipleri
  sqlite3 $ro "SELECT MAX(version), COUNT(*) FROM schema_version;"
  sqlite3 $ro "SELECT COUNT(*) FROM sqlite_master WHERE type='table';"
} else { "VERIFICATION REQUIRED: versacoder.db yok (§1.2 E4)" }
```

**Geçme kriteri:** #1 aralıkta + hizasız 0 · #2 `ifnotexists_eksik=0` · #3 boş çıktı · #4 yalnız onaylı `DROP` · #5 mojibake 0, EF 0 · #6 sürüm + tablo sayısı gelir.
**Not:** `sqlite3`/`db/` yoksa (§1.2 E3–E4, E8) çıktı `VERIFICATION REQUIRED` kalır; **kod/DB değişikliği bu şablonun işi değildir.**

### §5.4 Quality Report

```markdown
## Migration Kalite Raporu — YYYY-MM-DD

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Dosya satır / KB | N / N | 480-660 / 15-32 | 🟢/🔴 |
| Tablo satırı | N | >= 25 | 🟢/🔴 |
| Kutu hizasız satır (§5.3/1) | N | 0 | 🟢/🔴 |
| `IF NOT EXISTS` eksik (§5.3/2) | N | 0 | 🟢/🔴 |
| Versiyon çakışması (§5.3/3) | N | 0 | 🟢/🔴 |
| Onaysız `DROP` (§5.3/4) | N | 0 | 🟢/🔴 |
| EF / mojibake (§5.3/5) | N | 0 | 🟢/🔴 |
| Red Team taraması (§7.2) | N/10 | 10/10 | 🟢/🔴 |

**Karar:** 🟢 `db/migrations/`'e serbest / 🔴 düzeltmeye gönderildi
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] Frontmatter 7 alan tam (`title,type,category,status,version,updated,authority: primary`)
- [ ] H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı)
- [ ] §1.1–§1.4 dolu (§1.2 gerçek disk kanıtı + ⚠️ + yorum yasağı)
- [ ] §2.1–§2.2 dolu
- [ ] §3.1–§3.9 dolu; §3.9'da dolu örnek + `{{…}}` kalıp bloğu
- [ ] §3.2 `schema_version` + `schemaversion` okuma sorgusu
- [ ] §3.3 idempotent kalıp (`IF NOT EXISTS`, `WHERE col IS NULL`)
- [ ] §3.4 up/down çifti 5 satır
- [ ] §3.5 expand-and-contract + GATE kutusu
- [ ] §3.7 sıralı script listesi
- [ ] §3.8 yedek + rollback (6 adım) + lock tablosu
- [ ] §4.5 G1..G12 (≥10 madde)
- [ ] §5.1 9 adım + ~70 dk
- [ ] §5.2 ≥8 hata modu
- [ ] §5.3 **6 salt-okunur komut** (PRAGMA + schema_version + dotnet dahil)
- [ ] §5.4 Quality Report bloğu
- [ ] §6.5 kabul kriterleri (≥8) + test verisi
- [ ] §7.2 Red Team ≥10 satır
- [ ] Kutu satırı = 66 karakter · `{{…}}` yalnız kalıp bloklarında · mojibake = 0

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Aksiyon |
|--------|---------|
| `down` dosyası yok | **DUR** — §3.4 yazılır |
| `IF NOT EXISTS` eksik (§5.3/2 > 0) | **DUR** — §3.3 düzeltilir |
| `V0xx` çakışması (§5.3/3) | **DUR** — yeniden numaralandırılır |
| Yedeğiz `DROP` | **DUR** — yedek + `security` onayı |
| `schema_version` satırı yok | **DUR** — §3.3'e eklenir |
| Backfill `WHERE`siz | **DUR** — §3.6 |
| Ölçüm çıktısı yok ("uygulandı") | **DUR** — §5.3/6 istenir |
| Mojibake / EF geçiyor | **DUR** — UTF-8 + ADO.NET |

### §6.3 Sürüm ve Geçiş Kuralları

| Değişiklik | Sürüm | Etkilenen | Kayıt |
|-----------|-------|-----------|-------|
| İlk yazım | `0.1.0 → 1.0.0` | tek dosya | `updated` + `log.md` |
| Script uygulandı | `minor` | `status: applied` | `schema_version` satırı |
| Down eklendi/değişti | `minor` | §3.4 | Quality Report |
| Script geri alındı | `major` | `status: reverted` | §3.8 rollback kaydı |
| Yeni `V0xx` | `minor` | §3.7 listesi | sıra tablosu |
| Şema ≥ 2 sürüm değiştirdi | — | `../query/Query-Template` | sorgu şeması güncellenir |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi |
| `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm biçimi, §5.3/§5.4 standardı |
| `.ai/.templates/query/Query-Template.md` | şemayı okuyan sorgular |
| `.ai/.templates/infrastructure/dapper-mapping-template.md` | yeni kolonların C# eşlemesi |
| `.ai/.templates/adr/adr-data-template.md` | saf SQL + `schema_version` kararı (ADR-004) |
| `db/migrations/V0xx__*.sql` | bu şablondan üretilen gerçek dosya |

### §6.5 Envantere Özgü Kontroller + Kabul Kriterleri

**Test verisi (boş `versacoder.db` üzerinde):**

| Veri | Amaç | Beklenen |
|------|------|----------|
| Boş DB + up | ilk kurulum | `schema_version` = 1 satır |
| Aynı up 2. kez | idempotentlik | hata yok, satır sayısı değişmez |
| Up → down → up | çift | şema geri gelir, veri kaybı bilinçli |
| 1000 satırlı backfill | parti + kilit | 0 hata, `IS NULL` = 0 |
| Eşzamanlı 2 runner | kilitleme | biri bekler, çift kayıt yok |

**Kabul kriterleri:**

| # | Kriter | Kanıt |
|---|--------|-------|
| K1 | §5.3/2 `ifnotexists_eksik = 0` | PowerShell çıktısı |
| K2 | §5.3/3 versiyon çakışması = 0 | PowerShell çıktısı |
| K3 | `SELECT MAX(version) FROM schema_version` = beklenen | §5.3/6 |
| K4 | `PRAGMA table_info(x)` kolon listesi = §3.9'daki liste | §5.3/6 |
| K5 | Backfill sonrası `COUNT(*) WHERE col IS NULL` = 0 | §5.3/6 |
| K6 | Down sonrası tablo/index yok (`sqlite_master` = 0 satır) | §5.3/6 |
| K7 | 2. çalıştırma hata vermiyor (idempotent) | test çıktısı |
| K8 | Dosya 480–660 satır / 15–32 KB, kutu 66 | §5.3/1 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin |
|--------|-----|---------|
| Şablon envanteri | `.ai/.templates/index.md` | kayıt, §3.7 kalite sözleşmesi |
| Biçim referansı | `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm, §5.3, §5.4 |
| Sorgu şablonu | `.ai/.templates/query/Query-Template.md` | şemayı okuyan sorgular |
| Dapper eşlemesi | `.ai/.templates/infrastructure/dapper-mapping-template.md` | yeni kolon eşlemesi |
| Veri kararı (ADR) | `.ai/.templates/adr/adr-data-template.md` | saf SQL + `schema_version` |
| Dizin kuralı | `.ai/.templates/CLAUDE.md` | erişim sırası |
| Katman gerçeği | `.ai/architecture/index.md` | §1.2 E6 — ⚠️ VERIFICATION REQUIRED |
| Ürün adı | VERSA CODER / VersaCoder | CoreMusic kullanılmaz |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Sinyal | Doğrusu | Kayıp |
|---|------|--------|---------|-------|
| R1 | Yedeğiz `DROP TABLE` | §5.3/4 + G5 | yedek + `security` onayı | **veri kaybı** |
| R2 | Script'i 2. kez çalıştırınca şema bozulması | `already exists` | `IF NOT EXISTS` (§3.3) | 2 saat kurulum |
| R3 | `DELETE FROM tablo` (tüm satırlar) | §5.3/4 | `WHERE` + parti | **veri kaybı** |
| R4 | `DROP DATABASE` / `PRAGMA writable_schema` | §5.3/4 | yalnız şema script'i | DB silme |
| R5 | Down'sız genişleme | §5.3/2 | her up'a down | geri alınamaz |
| R6 | Aynı `V0xx` numarası | §5.3/3 | sıradaki boş numara | yarım şema |
| R7 | Backfill'i `WHERE`siz yapmak | §5.3/4 | `WHERE col IS NULL` | veriyi sıfırlama |
| R8 | Uzun transaction ile kilitleme | `database is locked` | parti ≤ 1000 + COMMIT | uygulama durur |
| R9 | Contract'ı aynı sürümde yapmak | §3.5 GATE yok | ≥ 2 sürüm gecikme | eski kod kırılır |
| R10 | Başka vault'tan script kopyalamak | `coremusic`/`versacoder.ai` | yalnız yapı referansı | G1 ihlali |
| R11 | "Uygulandı" diye ölçüm olmadan | §5.3/6 yok | `schema_version` + PRAGMA çıktısı | kanıtsız onay |
| R12 | EF Core migration'a dönmek | `EntityFramework` | saf `.sql` + `schema_version` | ADR-004 ihlali |
