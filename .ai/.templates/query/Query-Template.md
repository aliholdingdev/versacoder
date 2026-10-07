---
title: "VersaCoder — Veri Sorgusu / Rapor Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "veri sorgusu/rapor sablonu: SELECT yapisi, parametreler ve cikti bicimi"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Veri Sorgusu / Rapor Şablonu

**Zorunlu Bağlantılar:** [[../index]] · [[../CLAUDE]] · [[../adr/adr-data-template]] · [[../infrastructure/dapper-mapping-template]] · [[../infrastructure/migration-template]] · [[../../architecture/index.md]]

---

## §1 Amaç

### §1.1 Neden bu şablon gerekli

Bir veri sorgusu kod yazmadan önce **sözleşmedir**: hangi tablodan okuyacağı, hangi parametreleri alacağı, hangi index'e bağımlı olduğu, kaç milisaniyede döneceği ve boş sonuçta ne göstereceği burada yazılıdır. VersaCoder veri erişimi **ADO.NET + Dapper** ile yürür (EF Core yasak, ADR-004); sorgu metni **her zaman parametrelidir** (`@p1..@pN`). Şablonsuz üretilen sorgu; string concat, bilinmeyen index, sınırsız `LIMIT` ve N+1 ile repo'ya girer.

| Alan | Değer |
|------|-------|
| Template Name | `Query-Template.md` |
| Template Path | `.ai/.templates/query/Query-Template.md` |
| Hedef Dosya | `.ai/query/<sorgu-slug>.md` (ör. `.ai/query/select-active-sessions.md`) |
| Eşleşen Kod | `src/VersaCoder.Infrastructure/**/*Repository.cs`, `*Query.cs` |
| Sorumlu Rol | `database-optimizer` (yazım) · `reviewer` (denetim) · `security` (parametre) |
| SQL Diyalekti | SQLite (`Microsoft.Data.Sqlite`) birincil · SQL Server ikincil |
| Parametre Biçimi | `@p1, @p2, …` — **string concat yasak** (§4.5 G3) |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — SQL/kolon adları İngilizce |
| Versiyon | 1.0.0 · Authority: SSOT · Bootstrap: 2026-09-25 |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

Ölçüm tarihi: **2026-09-25**, kök `C:\versacoder\.ai\.templates\`.

| # | İddia | Disk Kanıtı | Durum |
|---|-------|-------------|-------|
| E1 | `.templates/` altında 3 klasör + 3 kök dosya var | `adr`=6, `agents`=2, `architecture`=1, kök=3 → toplam 12 MD | ✅ Doğrulandı |
| E2 | `.templates/query/` klasörü | yazım öncesi **YOK**; bu dosya ile açılır | ✅ Doğrulandı (bu dosya) |
| E3 | `.templates/infrastructure/` kardeş şablonlar | yazım anında **YOK** (2 dosya bu oturumda) | ⚠️ VERIFICATION REQUIRED |
| E4 | `.ai/architecture/index.md`, `.ai/.rules/` (7 MD), `.ai/keys.md` | `.ai\` altında yalnız `.templates` klasörü var | ⚠️ VERIFICATION REQUIRED |
| E5 | Katmanlar: `VersaCoder.Domain … .Host/.Desktop/.Cli` | `C:\versacoder\src\` klasörü yok → derlenmiş kanıt yok | ⚠️ VERIFICATION REQUIRED |
| E6 | `versacoder.db` + `schema_version` + `db/migrations/*.sql` | `db/` ve `*.db` bulunamadı | ⚠️ VERIFICATION REQUIRED |
| E7 | ADO.NET + Dapper (EF Core yasak, ADR-004) | `.csproj` yok → paket kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E8 | DevExpress 25.2.8 · `net48;net10.0-windows` | paket/proje kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E9 | `sqlite3` CLI bu makinede kurulu | `Get-Command sqlite3` → YOK | ⚠️ VERIFICATION REQUIRED |

**Yorum yasağı:** `⚠️ VERIFICATION REQUIRED` satırına "aslında vardır / artık yoktur" yorumu **yazılmaz**. Satır yalnız §5.3 komutunun **çıktısı eklendiğinde** ✅'ye döner. Var olmayan dosya için "şurada" denmez; yol + komut yazılır.

### §1.3 Sorgu Türleri

| Tür | Ne üretir | Tipik Hedef | Zorunlu Bölüm |
|-----|-----------|-------------|----------------|
| **Q1 Liste/rapor** | çok satırlı okuma | grid, export | §3.6, §3.7 |
| **Q2 Tekil okuma** | tek satır / tek değer | detay ekranı | §3.2, §3.7 |
| **Q3 Sayaç/özet** | `COUNT`/`SUM`/`GROUP BY` | dashboard | §3.8 bütçe |
| **Q4 Varlık kontrolü** | `EXISTS` → bool | doğrulama | §3.4 |
| **Q5 Yazma (`INSERT/UPDATE/DELETE`)** | etkilenen satır sayısı | command handler | §4.5 G8 |
| **Q6 Toplu okuma (sayfalı)** | `LIMIT/OFFSET` + toplam | liste + sayfalama | §3.6 |

### §1.4 Neden parametreli SQL tek yoldur

| Yaklaşım | SQL Injection | Plan yeniden kullanımı | VersaCoder Kararı |
|----------|---------------|--------------------------|-------------------|
| String concat (`$"...{ad}..."`) | **açık** | her seferinde yeni plan | **YASAK** (G3) |
| `string.Format` ile derleme | **açık** | yeni plan | **YASAK** |
| Kullanıcı SQL'i ham derleme | **açık** | — | **YASAK** (§7.2 R1) |
| `@p1..@pN` + `DynamicParameters` | kapalı | **evet** (parameterized plan) | **ZORUNLU** |
| `IN` listesi | — | her eleman ayrı parametre (`@p1,@p2…`) | §3.2 kuralı 5 |

---

## §2 Kapsam

**İçinde:** sorgu amacı, kaynak tablolar, parametre tablosu, SQL gövdesi, JOIN gerekçesi, index bağımlılığı, sayfalama/sıralama, sonuç şeması, boş sonuç davranışı, performans bütçesi, N+1, hata senaryoları, salt-okunur doğrulama, test verisi, kabul kriterleri.
**Dışında:** şema değişikliği (→ `../infrastructure/migration-template`), C# `Query<T>` eşlemesi (→ `../infrastructure/dapper-mapping-template`), gizli anahtar/parola (→ `keys.md`), ekran tel kafesi (→ `../ui-design/`).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya Tipi | Sorumlu | Bu Şablon? |
|---|------------|---------|-------------|
| 1 | `.ai/query/SELECT-*.md` rapor sorgusu | `database-optimizer` | ✅ |
| 2 | `.ai/query/EXISTS-*.md` / `COUNT-*.md` | `database-optimizer` | ✅ |
| 3 | `src/**/XRepository.cs` içindeki sorgu metni | `developer` | ✅ (şablon değil, §6.5 K4 kanıtı) |
| 4 | `db/migrations/V0xx__*.sql` şema değişikliği | `database-optimizer` | ❌ → `migration-template` |
| 5 | `Dapper` kolon→özellik eşlemesi | `database-optimizer` | ❌ → `dapper-mapping-template` |
| 6 | `.ai/reports/performance-*.md` ölçüm raporu | `performance-engineer` | ❌ (rapor ≠ şablon) |
| 7 | `.ai/.rules/*.md` kural dosyası | `reviewer` | ❌ (kural ≠ sorgu) |
| 8 | `.ai/keys.md` secret | `security` | ❌ (G7) |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `HAYIR`** yanlış şablonu gösterir:

| # | Soru | Beklenen | Uymazsa |
|---|------|----------|---------|
| S1 | Dosya **okunan veriyi** mi tanımlıyor (SELECT/EXISTS/COUNT)? | `EVET` | yazma ise §4.5 G8 ile ayrıca değerlendirilir |
| S2 | Tek bir `@p1..@pN` parametre listesi yazılabiliyor mu? | `EVET` | birden çok sorgu → 2 şablon |
| S3 | Bir tablo/sütun **eklenmiyor** mu? | `EVET` | `migration-template` |
| S4 | Kolon→özellik eşlemesi ana konu mu? | `HAYIR` | `dapper-mapping-template` |
| S5 | Kanıt olarak satır sayısı + ms çıktısı üretilebiliyor mu? | `EVET` | kanıtsız → **DUR** |

---

## §3 Mimari

### §3.1 Sorgu Künyesi

```yaml
# .ai/query/<slug>.md — ilk blok
query_id: QRY-NNN              # benzersiz, artan
slug: select-active-sessions   # kebab-case, hedef kod adıyla aynı
type: Q1                       # §1.3 türü
owner: database-optimizer
source_tables: [session, user] # §3.4 JOIN kanıtı buradan okunur
status: draft | active | deprecated
budget_ms: 150                 # §3.8
index_required: idx_session_user_created
parameterized: true            # her zaman true
```

| Alan | Zorunlu | Örnek | Eksikse |
|------|---------|-------|---------|
| `query_id` | Evet | `QRY-014` | RED |
| `slug` | Evet | `select-active-sessions` | RED |
| `type` | Evet | `Q1` | RED |
| `source_tables` | Evet | `session, user` | RED |
| `budget_ms` | Evet | `150` | RED |
| `parameterized` | Evet | `true` | RED |

### §3.2 Parametre Tablosu

| # | Parametre | Tip | Zorunlu | Varsayılan | Doğrulama | SQL Kullanımı |
|---|-----------|-----|---------|------------|-----------|----------------|
| 1 | `@p1` | `INTEGER` | Evet | — | `> 0` | `s.user_id = @p1` |
| 2 | `@p2` | `TEXT` | Hayır | `NULL` | `MAX 64` | `s.tag LIKE @p2` |
| 3 | `@p3` | `TEXT` | Hayır | `''` | enum listesi | `s.status IN (@p3, @p4)` |
| 4 | `@p4` | `INTEGER` | Hayır | `0` | `>= 0` | `LIMIT @p4` |
| 5 | `@pN` (toplu `IN`) | `INTEGER[]` | Hayır | boş | eleman ≤ 200 | her eleman ayrı `@pK` |

**Parametre kuralları (§3.2 özeti):**

1. Ad **`@p1..@pN`** olur; işlevsel ad (`@userId`) kullanılmaz → iki sağlayıcıda aynı metin.
2. `NULL` gelen opsiyonel parametre **koşul bloğu ile** yazılır (`AND (@p2 IS NULL OR …)`), `LIKE`'a `NULL` verilmez.
3. `IN` listesi **tek parametre olarak verilemez**; eleman sayısı kadar `@pK` üretilir (Dapper + ADO.NET kısıtı).
4. Kullanıcı girdisi **asla** parametre adına, tablo/sütun adına dönüşmez (§4.5 G4).
5. Zorunlu parametre `null`/boş gelirse sorgu **çalışmaz** → §3.8 H2 hata senaryosu.

### §3.3 SQL Gövdesi (kalıp — `{{…}}` doldurulur)

```text
SELECT  {{KOLON_1}}, {{KOLON_2}}
FROM    {{TABLO_1}} AS t1
        {{JOIN_TIPI}} JOIN {{TABLO_2}} AS t2
          ON t2.{{ANAHTAR}} = t1.{{ANAHTAR}}
WHERE   t1.{{FILTRE_KOLONU}} = @p1
  AND   (@p2 IS NULL OR t1.{{IKINCIL_KOLON}} = @p2)
  AND   t1.{{TARIH_KOLONU}} >= @p3
ORDER BY t1.{{SIRALAMA_KOLONU}} {{YON}}       -- DESC | ASC (§3.6)
LIMIT   @p4 OFFSET @p5                        -- sayfalama (§3.6)
```

**Yasaklar:** `--` yorumu ile gizlenen dinamik parça, `GO`, dil bilgisi (`EXEC`/`sp_executesql`), string birleştirme operatörü (`+`, `||`) kullanıcı verisiyle.

### §3.4 JOIN / JOIN OLMAMA Gerekçesi

| # | Bağlantı | Tip | Gerekçe | N+1 Riski |
|---|----------|-----|---------|-----------|
| 1 | `session` → `user` | `INNER JOIN` | yalnız oturum sahibi görünür | yok |
| 2 | `session` → `tag` | `LEFT JOIN` | etiketsiz oturum da listede kalmalı | yok |
| 3 | `session` → `file` | `LEFT JOIN` + `COUNT(*)` | özet sütunu; ayrı sorgu **yasak** | **yüksek → JOIN'e çevrildi** |
| 4 | alt sorgu (`EXISTS`) | `EXISTS` | yalnız varlık kanıtı, satır gerekmez | yok |
| 5 | `UNION ALL` | birleşim | iki kaynaktan aynı şema | orta — bütçe §3.8 |

**Kural:** Aynı veriyi 2. sorguyla çekmek yerine **JOIN/`EXISTS`** tercih edilir; ayrı sorgu zorunluysa bu tabloya satır eklenir ve gerekçe tek cümle yazılır.

### §3.5 Index Bağımlılığı

| Index | Sütun(lar) | Bu sorguda | Kanıt |
|-------|-----------|------------|-------|
| `idx_session_user_created` | `(user_id, created_at)` | `WHERE` + `ORDER BY` | `PRAGMA index_list` §5.3/6 |
| `idx_session_status` | `(status)` | `WHERE` | `EXPLAIN QUERY PLAN` |
| `idx_user_email` | `(email)` | `Q2` tekil okuma | — |
| — | — | `LIKE '%x%'` | **index kullanılmaz** → bütçe uyarısı |

**Bağımlılık yazımı:** Sorgu, index'in **ilk sütunundan** filtrelemiyorsa (`user_id` yok, yalnız `created_at` var) plan `SCAN` olur → §6.5 K3 RED.

### §3.6 Sayfalama ve Sıralama

| Öğe | Karar | Neden |
|-----|-------|-------|
| Sayfalama | `LIMIT @p4 OFFSET @p5` | SQLite uyumu; `OFFSET` parametreli |
| Maks sayfa | `LIMIT ≤ 500` | ağ/UI bütçesi |
| Sıralama | `ORDER BY` **tek deterministik kolon + PK** | `id DESC` eklenmezse sayfalar kayar |
| Kullanıcı sıralaması | **enum whitelist** (`created_at`,`name`) | kolon adı girdi olarak **yasak** (G4) |
| Derin sayfalama | `keyset` (`WHERE id < @p6`) önerilir | `OFFSET` 10.000+ pahalı |
| Toplam sayı | ayrı `COUNT` sorgusu, aynı `WHERE` | `COUNT(*)` ana sorguya **konmaz** |

### §3.7 Sonuç Şeması (kolon tablosu) + Boş Sonuç

| # | Kolon (SQL) | Tip | C# Karşılığı | Nullable | Not |
|---|-------------|-----|--------------|----------|-----|
| 1 | `session_id` | `INTEGER` | `long` | Hayır | PK, asla `SELECT *` |
| 2 | `user_name` | `TEXT` | `string` | Evet | `LEFT JOIN` → `?? ""` (§3.8) |
| 3 | `created_at` | `TEXT/TEXTS` | `DateTime` | Hayır | `DateTimeOffset` → §6.5 K6 |
| 4 | `file_count` | `INTEGER` | `int` | Hayır | alt sorgu kolonu |
| 5 | `status` | `INTEGER` | `SessionStatus` | Hayır | enum dönüşümü → dapper şablonu |

| Boş Sonuç Senaryosu | Davranış | UI Karşılığı |
|---------------------|----------|--------------|
| 0 satır + geçerli parametre | **hata değil**, boş liste döner | "Kayıt bulunamadı" |
| 0 satır + zorunlu parametre `NULL` | **hata** (§3.8 H2) | "Eksik parametre" |
| `Q2` tekil okumada 0 satır | `null` / `Result.NotFound` | detay yok |
| `COUNT` = 0 | `0` döner, `null` değil | sayaç `0` |

### §3.8 Performans Bütçesi, N+1 ve Hata Senaryoları

**Bütçe tablosu:**

| Tür | Bütçe (ms) | Ölçüm | Aşarsa |
|-----|-----------|-------|--------|
| Q1 liste (≤ 500 satır) | 150 | §5.3/6 stopwatch | index incele |
| Q2 tekil | 50 | aynı | plan `SCAN` ise index |
| Q3 sayaç/özet | 200 | aynı | `GROUP BY` index'i |
| Q4 `EXISTS` | 30 | aynı | — |
| Q5 yazma | 100 | `ExecuteAsync` | transaction kontrolü |

**N+1 risk tablosu:**

| Desen | Belirti | Önlem |
|-------|---------|-------|
| Döngüde sorgu | `foreach` içinde `Query` | tek sorgu + `IN (@pK…)` |
| Satır başına ek kolon | 2. sorgu ile dosya adı | `LEFT JOIN` (§3.4 #3) |
| Sayfa başına toplam | her satır için `COUNT` | tek özet sorgusu |
| Cache'siz tekrar | aynı parametre 10× | `MemoryCache` notu |

**Hata senaryoları:**

| # | Hata | Belirti | Dönüş |
|---|------|---------|-------|
| H1 | Tablo/kolon yok | `SQLiteException: no such column` | `Result.Fail(SQL_SCHEMA)` |
| H2 | Zorunlu parametre `null` | doğrulama | `Result.Invalid` |
| H3 | Bütçe aşımı | `> budget_ms` | uyarı log + §6.5 K7 |
| H4 | Bağlantı yok/dosya kilitli | `database is locked` | tekrar yok → hata |
| H5 | Dönüşüm hatası (`"abc"` → `long`) | `DataException` | `Result.Fail(SQL_MAPPING)` |

**Çalışma akışı (kutu):**

```text
+----------------------------------------------------------------+
| [1] İstemci  VersaCoder.Desktop / VersaCoder.Cli               |
|    v                                                           |
| [2] Application  IQueryHandler.HandleAsync(p1, p2)             |
|    v                                                           |
| [3] Infrastructure  Dapper QueryAsync<T>(sql, dyn)             |
|    v                                                           |
| [4] AdoConnectionFactory  IDbConnection (SQLite, mode=ro)      |
|    v                                                           |
| [5] versacoder.db  tablo + index (§3.5) + schema_version       |
+----------------------------------------------------------------+
```

**Ne anlatır:** sorgu akışı katman sırasıyla iner (Application → Infrastructure → veritabanı); ters ok **ihlal** sayılır.
**Doğrulama:** §5.3/1 — satır uzunlukları 66'ya eşit olmalı.

### §3.9 Dolu Örnek ve Tam Kalıp

**Dolu örnek (gerçek VersaCoder sorgusu — `QRY-014`):**

```text
QRY-014  select-active-sessions   type=Q1   budget_ms=150
Kaynak:  session (PK session_id), user (PK user_id), file (FK session_id)
JOIN:    session INNER JOIN user  -- zorunlu: sahibi görünmeli
         session LEFT JOIN (SELECT session_id, COUNT(*) AS file_count FROM file GROUP BY session_id)
WHERE:   session.status = @p1 AND session.user_id = @p2
ORDER:   session.created_at DESC, session_id DESC   -- deterministik (§3.6)
LIMIT:   @p3 OFFSET @p4                             -- LIMIT <= 500
Index:   idx_session_user_created (user_id, created_at)
```

**Tam kalıp (üretilecek `.ai/query/<slug>.md` için `{{…}}` doldurulur):**

````markdown
---
title: "QRY-NNN — {{SLUG}}"
type: query
category: query
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: primary
---
# QRY-NNN — {{SORGU_ADI}}

**Zorunlu Bağlantılar:** [[../index]] · [[../infrastructure/dapper-mapping-template]] · [[../infrastructure/migration-template]] · [[../adr/adr-data-template]] · [[../../architecture/index.md]] · [[../CLAUDE]]

## §1 Amaç — {{BIR_CUMLE}}
## §2 Parametreler
| # | Parametre | Tip | Zorunlu | Doğrulama |
|---|-----------|-----|---------|-----------|
| 1 | @p1 | {{TIP}} | Evet | {{KOSUL}} |
## §3 SQL Gövdesi
```text
{{SQL_GOVDESI}}
```
## §4 Sonuç Şeması
| # | Kolon | Tip | Nullable |
|---|-------|-----|----------|
| 1 | {{KOLON}} | {{TIP}} | {{EVET_HAYIR}} |
## §5 Kabul Kriterleri — {{KABUL_1}} … {{KABUL_N}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Çıktı | Atlanırsa |
|---|------|-------|-----------|
| 1 | Gerekçeyi tek cümlede yaz | §1.1 | sorgunun neden olduğu belirsiz |
| 2 | Kaynak tabloları listele | §3.1 `source_tables` | JOIN sürprizi |
| 3 | Parametreleri tabloya koy | §3.2 | concat eğilimi |
| 4 | SQL gövdesini kalıptan yaz | §3.3 | injection riski |
| 5 | JOIN gerekçesi + index | §3.4, §3.5 | sessiz tablo taraması |
| 6 | Sayfalama/sıralama + şema | §3.6, §3.7 | kayan sayfalar |
| 7 | Bütçe + N+1 + hata | §3.8 | üretimde yavaşlık |
| 8 | Doğrulama komutunu çalıştır | §5.3 | kanıtsız onay |
| 9 | Red Team'i tara | §7.2 | bilinen açık kalır |

### §4.2 Onay Akışı

| Aşama | Rol | Ne Onaylar | Süre |
|-------|-----|-----------|------|
| 1 | `database-optimizer` | şablon dolumu + §5.3 çıktısı | 20 dk |
| 2 | `reviewer` | §6.1 checklist (17/17) | 10 dk |
| 3 | `security` | §4.5 G3/G4 + §7.2 R1–R3 | 8 dk |
| 4 | `performance-engineer` | §3.8 bütçe + index kanıtı | 8 dk |
| 5 | `master-orchestrator` | Quality Report → `log.md` | 5 dk |

### §4.3 Ölçek ve Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Dosya uzunluğu | 480–660 satır | dışı RED |
| Dosya boyutu | 15–32 KB | dışı RED |
| Tablo satırı | ≥ 25 (her §'de tablo) | eksik RED |
| `LIMIT` | ≤ 500 | gerekçe + §7.2 |
| Bütçe | Q1 ≤ 150 ms | §6.5 K7 |
| Parametre | `@p1..@pN` | concat RED |
| Kodlama | UTF-8 BOM'suz | Latin-1 RED |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm | Zorunlu Güncelleme |
|-----------|-------|---------------------|
| Kolon/parametre eklendi | `minor` | §3.2 + §3.7 + `updated` |
| SQL gövdesi değişti | `minor` | §3.3 + §5.3 çıktısı |
| Sadece yazım/hizalama | `patch` | `updated` |
| Sorgu kaldırıldı | `major` | `status: deprecated` + §7.1 |
| Bütçe değişti | `minor` | §3.8 + ölçüm kanıtı |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu | Tespit |
|---|-----------|--------------|--------|
| G1 | Başka vault'tan (`coremusic`, `versacoder.ai`) içerik kopyalanması | RED + sil | §7.2 R10 |
| G2 | EF Core / ORM referansı | RED (ADR-004) | `Select-String EntityFramework` |
| G3 | SQL'de string concat / `string.Format` | **DUR** | §5.3/2 |
| G4 | Tablo/sütun/kolon adı kullanıcı girdisinden | **DUR** | gözden geçirme |
| G5 | `SELECT *` kullanımı | RED | §5.3/3 |
| G6 | Parametre tablosu eksik / `@pN` adsız | RED | §3.2 |
| G7 | `keys.md`/connection string içermek | RED | §7.2 R3 |
| G8 | Yazma sorgusunda transaction yok (Q5) | RED | §6.5 K8 |
| G9 | `ORDER BY` deterministik değil | RED | §3.6 |
| G10 | `LIMIT` > 500 veya `LIMIT` yok | RED | §4.3 |
| G11 | §5.3 çıktısı olmadan "hızlı" iddiası | **DUR** | §6.2 R5 |
| G12 | Mojibake / İngilizce-Türkçe karışık | RED | §5.3/4 |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | İhtiyacı yaz | kullanıcı isteği | 1 cümle | database-optimizer | 3 dk |
| 2 | Tablo/şema oku | `PRAGMA table_info` | kolon listesi | database-optimizer | 5 dk |
| 3 | Parametre tablosu | gereksinim | §3.2 | database-optimizer | 5 dk |
| 4 | SQL yaz | §3.3 kalıp | gövde | database-optimizer | 10 dk |
| 5 | JOIN + index gerekçe | §3.4, §3.5 | 2 tablo | database-optimizer | 5 dk |
| 6 | Sayfalama + şema | §3.6, §3.7 | 2 tablo | database-optimizer | 5 dk |
| 7 | Bütçe + N + hata | §3.8 | 3 tablo | database-optimizer | 5 dk |
| 8 | Doğrula | §5.3 (6 komut) | 0 ihlal | database-optimizer | 5 dk |
| 9 | Onay + kayıt | §6.1, `log.md` | Quality Report | reviewer | 7 dk |
| | **Toplam** | | | | **≈ 50 dk** |

### §5.2 Hata Modları

| # | Hata | Belirti | Kök Neden | Düzeltme |
|---|------|---------|-----------|----------|
| 1 | Concat sızıntısı | `$"...{x}..."` | tembellik | `@pN` + `DynamicParameters` |
| 2 | Sessiz `SELECT *` | şema değişince kırılır | kolon listesi yazılmamış | §3.7 zorunlu |
| 3 | Kayan sayfa | `ORDER BY` tek kolon | deterministik eklenmemiş | `id DESC` ekle |
| 4 | N+1 | döngüde sorgu | ayrı sorgu tercihi | `IN (@pK…)` / JOIN |
| 5 | `SCAN` plan | yavaş liste | index yok/yanlış sıralı | §3.5 kanıtı |
| 6 | Boş sonuç şoku | ekran boş | davranış yazılmamış | §3.7 tablosu |
| 7 | Bütçe aşımı | > 150 ms | join patlaması | `EXISTS` + index |
| 8 | Dönüşüm hatası | `DataException` | tip/nullable uyumsuz | dapper şablonu §6.4 |

### §5.3 Salt Okunur Doğrulama Komutları

Aşağıdaki 6 komut **hiçbir dosyayı değiştirmez**; dosya yazmaz, DB'ye yazmaz.

```powershell
# 1) ÖLÇÜM — satır (480-660) ve KB (15-32), iki değer de aralıkta OLMALI
$p = 'C:\versacoder\.ai\.templates\query\Query-Template.md'
$t = [System.IO.File]::ReadAllText($p)
"satir={0} (480-660)  kb={1} (15-32)" -f (($t -split "`n").Count), [math]::Round((Get-Item $p).Length/1KB,1)
```

```powershell
# 2) CONCAT / SQL INJECTION TARAYICISI — sonuç 0 OLMALI
Select-String -Path $p -Pattern 'string\.Format|\$"[^"]*\{|''\s*\+|EXEC\s*\(|sp_executesql' | Measure-Object
```

```powershell
# 3) PARAMETRE VE 'SELECT *' KANITI — @pN >= 1, 'SELECT *' = 0
$q = [System.IO.File]::ReadAllText($p)
"pN={0}  star={1}" -f ([regex]::Matches($q,'@p\d+').Count), ([regex]::Matches($q,'(?i)SELECT\s+\*').Count)
```

```powershell
# 4) KODLAMA + PLACEHOLDER — mojibake 0; {{…}} yalnız kalıp bloğu dışında 0
$m = Select-String -Path $p -Pattern '[\u00C3\u00C2\u00C4\uFFFD]' | Measure-Object   # mojibake = 0
$ph = Select-String -Path $p -Pattern '\{\{[A-Z0-9_]+\}\}' | Measure-Object
"mojibake={0}  placeholder={1}" -f $m.Count, $ph.Count
```

```powershell
# 5) DOTNET ARAÇ ZİNCİRİ (salt okunur — derleme/yazma YOK)
dotnet --list-sdks
dotnet --info
# hedef TFM kanıtı (yalnız evaluation, build YAZMAZ):
Get-ChildItem 'C:\versacoder\src' -Recurse -Filter *.csproj -ErrorAction SilentlyContinue |
  Select-Object -First 5 -ExpandProperty FullName
```

```powershell
# 6) SQLITE SALT OKUNUR — schema/index/satır sayımı (mode=ro)
$db = 'C:\versacoder\db\versacoder.db'
if (Test-Path $db) {
  sqlite3 "file:$($db -replace '\\','/')?mode=ro" "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;"
  sqlite3 "file:$($db -replace '\\','/')?mode=ro" "PRAGMA table_info(session);"
  sqlite3 "file:$($db -replace '\\','/')?mode=ro" "EXPLAIN QUERY PLAN SELECT 1 FROM session WHERE user_id=1;"
} else { "VERIFICATION REQUIRED: versacoder.db yok (§1.2 E6)" }
```

**Geçme kriteri:** #1 aralıkta · #2 = 0 · #3 `pN>=1, star=0` · #4 = 0/0 · #5 SDK listesi gelir · #6 şema çıktısı gelir (yoksa §1.2 E6 kalır).
**Not:** `sqlite3` CLI kurulu değilse (§1.2 E9) çıktı `VERIFICATION REQUIRED` olarak kalır; **kurulum/kod değişikliği bu şablonun işi değildir.**

### §5.4 Quality Report

```markdown
## Query Kalite Raporu — YYYY-MM-DD

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Dosya satır / KB | N / N | 480-660 / 15-32 | 🟢/🔴 |
| Tablo satırı | N | >= 25 | 🟢/🔴 |
| Concat ihlali (§5.3/2) | N | 0 | 🟢/🔴 |
| `SELECT *` (§5.3/3) | N | 0 | 🟢/🔴 |
| Mojibake (§5.3/4) | N | 0 | 🟢/🔴 |
| Bütçe ihlali (§3.8) | N | 0 | 🟢/🔴 |
| Red Team taraması (§7.2) | N/10 | 10/10 | 🟢/🔴 |

**Karar:** 🟢 `.ai/query/`'ye serbest / 🔴 düzeltmeye gönderildi
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] Frontmatter 7 alan tam (`title,type,category,status,version,updated,authority: primary`)
- [ ] H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı)
- [ ] §1.1–§1.4 dolu (§1.2 gerçek disk kanıtı + ⚠️ + yorum yasağı)
- [ ] §2.1–§2.2 dolu
- [ ] §3.1–§3.9 dolu; §3.9'da dolu örnek + `{{…}}` kalıp bloğu
- [ ] §3.2 parametre tablosu: `@pN`, tip, zorunluluk
- [ ] §3.5 index bağımlılığı yazılı
- [ ] §3.7 sonuç şeması + boş sonuç davranışı
- [ ] §3.8 bütçe (ms) + N+1 + ≥5 hata senaryosu
- [ ] §4.1–§4.4 dolu
- [ ] §4.5 G1..G12 (≥10 madde)
- [ ] §5.1 9 adım + zaman bütçesi
- [ ] §5.2 ≥8 hata modu
- [ ] §5.3 **6 salt-okunur komut** (dotnet + sqlite3 dahil)
- [ ] §5.4 Quality Report bloğu
- [ ] §6.5 kabul kriterleri + test verisi
- [ ] §7.2 Red Team ≥10 satır
- [ ] Kutu varsa satır = 66 karakter (§5.3/1)
- [ ] `{{…}}` yalnız kalıp bloklarında · mojibake = 0

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Aksiyon |
|--------|---------|
| §5.3/2 çıktısı > 0 | **DUR** — concat temizlenir |
| `SELECT *` geçiyor | **DUR** — §3.7 kolon listesi |
| Parametre tablosu boş | **DUR** — §3.2 doldurulur |
| Bütçesiz sorgu | **DUR** — §3.8 yazılır |
| `ORDER BY` tek kolonsuz | **DUR** — PK eklenir |
| Kanıtsız "hızlı" iddiası | **DUR** — §5.3 çıktısı istenir |
| Mojibake / bozuk kodlama | **DUR** — UTF-8 BOM'suz |
| Kalıp bloğu dışında `{{…}}` | **DUR** — doldurulur |

### §6.3 Sürüm ve Geçiş Kuralları

| Değişiklik | Sürüm | Etkilenen | Kayıt |
|-----------|-------|-----------|-------|
| Yeni sorgu (ilk yazım) | `0.1.0 → 1.0.0` | tek dosya | `updated` + `log.md` |
| Parametre/kolon eklendi | `minor` | §3.2, §3.7 | Quality Report |
| Sorgu deprecated | `major` | §7.1 | `status` hücresi |
| Şema değişti | — | `migration-template` | migration + bu dosya birlikte |
| Eşleme değişti | — | `dapper-mapping-template` | §6.4 satırı |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi (§3.7) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm biçimi, §5.3/§5.4 standardı |
| `.ai/.templates/infrastructure/dapper-mapping-template.md` | `Query<T>` kolon eşlemesi (Q1–Q6 çıktısı) |
| `.ai/.templates/infrastructure/migration-template.md` | kaynak tablo şeması + `schema_version` |
| `.ai/.templates/adr/adr-data-template.md` | ADO.NET + Dapper kararı (ADR-004) |
| `.ai/query/<slug>.md` | bu şablondan üretilen gerçek dosya |

### §6.5 Envantere Özgü Kontroller + Kabul Kriterleri

**Test verisi (her sorgu için en az 3 satır):**

| Veri | Amaç | Beklenen |
|------|------|----------|
| `user_id=1`, 3 satır | normal yol | 3 satır, < 150 ms |
| `user_id=999` (yok) | boş sonuç | 0 satır, hata değil |
| `@p1 = NULL` (zorunlu) | doğrulama | `Result.Invalid` |
| `tag = "a'b; DROP"` | injection | literal eşleşme, DB sağ |

**Kabul kriterleri:**

| # | Kriter | Kanıt |
|---|--------|-------|
| K1 | §5.3 #2 = 0 (concat yok) | PowerShell çıktısı |
| K2 | §5.3 #3 `pN ≥ 1` ve `star = 0` | PowerShell çıktısı |
| K3 | `EXPLAIN QUERY PLAN` → `USING INDEX` | §5.3/6 |
| K4 | Test verisi 4/4 beklenen sonuçla geçer | test çıktısı |
| K5 | Bütçe: Q1 ≤ 150 ms (ölçüm) | stopwatch logu |
| K6 | `DateTime`/`decimal`/`enum` dönüşümü hatasız | dapper şablonu testi |
| K7 | Dosya 480–660 satır / 15–32 KB | §5.3/1 |
| K8 | Q5'te transaction (yazma) | kod incelemesi |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin |
|--------|-----|---------|
| Şablon envanteri | `.ai/.templates/index.md` | kayıt, §3.7 kalite sözleşmesi |
| Biçim referansı | `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm, §5.3, §5.4 |
| Dapper eşlemesi | `.ai/.templates/infrastructure/dapper-mapping-template.md` | `Query<T>` kolon eşlemesi |
| Migration | `.ai/.templates/infrastructure/migration-template.md` | tablo + `schema_version` |
| Veri kararı (ADR) | `.ai/.templates/adr/adr-data-template.md` | ADO.NET + Dapper, `@pN` |
| Dizin kuralı | `.ai/.templates/CLAUDE.md` | erişim sırası |
| Katman gerçeği | `.ai/architecture/index.md` | §1.2 E4 — ⚠️ VERIFICATION REQUIRED |
| Ürün adı | VERSA CODER / VersaCoder | CoreMusic kullanılmaz |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Sinyal | Doğrusu | Kayıp |
|---|------|--------|---------|-------|
| R1 | SQL injection: kullanıcı verisiyle concat | §5.3/2 > 0 | `@p1..@pN` her yerde | güvenlik olayı |
| R2 | Kolon adını girdiyle derlemek (`ORDER BY {x}`) | §4.5 G4 | enum whitelist + sabit kolon | RED |
| R3 | Connection string / `keys.md` sorguya yapıştırmak | §5.3/4 | secret yalnız `keys.md` | RED |
| R4 | Kod enjeksiyonu: `EXEC` / `sp_executesql` / `GO` | §5.3/2 | yalnız `SELECT/INSERT/UPDATE/DELETE` | RED |
| R5 | Veri sızıntısı: `SELECT *` ile fazla kolon | §5.3/3 = 0 değil | §3.7 kolon listesi | KVKK benzeri risk |
| R6 | Kişisel veriyi log'a yazmak | log satırında `email/phone` | yalnız `session_id` | sızıntı |
| R7 | `WHERE` olmayan rapor (tüm tablo) | `WHERE` sayısı = 0 | filtre + `LIMIT` zorunlu | 10 sn tarama |
| R8 | N+1 ile 10.000 sorgu | döngüde `Query` | tek sorgu + `IN` | 30 sn → 150 ms |
| R9 | Test verisi gerçek kullanıcı verisi | testte gerçek e-posta | sentetik veri | sızıntı |
| R10 | Başka vault'tan sorgu kopyalamak | `coremusic`/`versacoder.ai` | yalnız yapı referansı, içerik özgün | G1 ihlali |
| R11 | Ölçüm olmadan "hızlı" demek | §5.3 çıktısı yok | 6 komutun çıktısı eklenir | kanıtsız onay |
| R12 | Boş sonucu hata sanmak | UI'da "hata" | §3.7 boş davranış tablosu | yanlış ürün kararı |
