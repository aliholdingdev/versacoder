---
title: "VersaCoder — Dapper Kolon→Özellik Eşleme Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "Dapper esleme sablonu: kolon-ozellik eslemesi, tipler ve sorgu ornegi"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Dapper Kolon→Özellik Eşleme Şablonu

**Zorunlu Bağlantılar:** [[../index]] · [[../CLAUDE]] · [[../query/Query-Template]] · [[../infrastructure/migration-template]] · [[../adr/adr-data-template]] · [[../../architecture/index.md]]

---

## §1 Amaç

### §1.1 Neden bu şablon gerekli

Dapper **kazanır** ama **sessizce yutar**: kolon adı uyuşmazsa özellik `default` kalır, `NULL` gelirse `DBNull` patlar, `SELECT *` bir migration'la sessizce bozulur. Bu şablon, sorgu çıktısının C# tipine **yazılı bir eşleme sözleşmesi** kurar: hangi kolon hangi özelliğe, tip dönüşümü nasıl, hangi Dapper metodu nerede kullanılır, hata modu nedir. VersaCoder'da veri erişimi **ADO.NET + Dapper**'dır (EF Core yasak, ADR-004); EF'nin otomatik eşlemesi **olmadığı için** bu sözleşme tek korumadır.

| Alan | Değer |
|------|-------|
| Template Name | `dapper-mapping-template.md` |
| Template Path | `.ai/.templates/infrastructure/dapper-mapping-template.md` |
| Hedef Dosya | `src/VersaCoder.Infrastructure/Dapper/<Tip>Mapping.cs` (şablon: `.ai/mapping/<sorgu>.md`) |
| Kütüphane | Dapper (`SqlMapper`) + `Microsoft.Data.Sqlite` / `Microsoft.Data.SqlClient` |
| Metod Ailesi | `Query<T>` · `QueryFirst<T>` · `QuerySingle<T>` · `Execute` · `QueryMultiple` (+`Async`) |
| Parametre | `@p1..@pN` + `DynamicParameters` — **concat yasak** |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Sorumlu | `database-optimizer` (yazım) · `reviewer` (denetim) · `testing` (eşleme testi) |
| Versiyon | 1.0.0 · Authority: SSOT · Bootstrap: 2026-09-25 |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

Ölçüm tarihi: **2026-09-25**, kök `C:\versacoder`.

| # | İddia | Disk Kanıtı | Durum |
|---|-------|-------------|-------|
| E1 | `.ai/.templates/` = 3 klasör + 3 kök dosya (12 MD) | `adr`=6, `agents`=2, `architecture`=1, kök=3 | ✅ Doğrulandı |
| E2 | `.ai/.templates/infrastructure/` klasörü | bu dosya ile 2 kardeş dosya oluşur | ✅ Doğrulandı (bu dosya) |
| E3 | `src/VersaCoder.Infrastructure/Dapper/*.cs` | `src/` klasörü **YOK** | ⚠️ VERIFICATION REQUIRED |
| E4 | Dapper paket sürümü (`.csproj`) | `.csproj` **YOK** → sürüm kanıtsız | ⚠️ VERIFICATION REQUIRED |
| E5 | `versacoder.db` + gerçek kolon adları | `*.db` bulunamadı | ⚠️ VERIFICATION REQUIRED |
| E6 | ADO.NET + Dapper, EF Core yasak (ADR-004) | paket kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E7 | `.ai/query/*.md` (girdi sorgular) | `query/` klasörü bu oturumda açıldı | ⚠️ VERIFICATION REQUIRED |
| E8 | `.ai/architecture/index.md`, `.ai/.rules/`, `.ai/keys.md` | `.ai\` altında yalnız `.templates` | ⚠️ VERIFICATION REQUIRED |
| E9 | Test projesi + `%90` coverage | `tests/` klasörü **YOK** | ⚠️ VERIFICATION REQUIRED |

**Yorum yasağı:** `⚠️ VERIFICATION REQUIRED` satırına "yok ama olacak" yorumu yazılmaz; yalnız §5.3 komutunun **çıktısı** satırı ✅'ye çeker. Var olmayan dosya için "şurada" denmez; yol + komut yazılır.

### §1.3 Eşleme Türleri

| Tür | Senaryo | Dapper Metodu | Zorunlu Not |
|-----|---------|----------------|-------------|
| **D1 Çoklu satır** | liste/rapor | `QueryAsync<T>` | 0 satır = boş liste |
| **D2 Tekil zorunlu** | detay (PK ile) | `QuerySingleAsync<T>` | 0/2 satır → exception |
| **D3 Tekil opsiyonel** | detay (bulunabilir) | `QueryFirstOrDefaultAsync<T>` | `null` yolunu yaz |
| **D4 Etkilenen satır** | yazma (Q5) | `ExecuteAsync` | dönen `int` = satır sayısı |
| **D5 Çoklu result set** | 2+ sorgu tek çağrı | `QueryMultipleAsync` | `SplitOn` zorunlu |
| **D6 Anonim/dinamik** | tek seferlik okuma | `QueryAsync<dynamic>` | şema adı açıkça yazılır |
| **D7 Valuetuple** | 2-3 kolonluk özet | `QueryAsync<(long,string)>` | kolon sırası = tuple sırası |

### §1.4 Neden Eşleme Sözleşmesi Şart

| Tehlike | Belirti | Bu Şablonun Karşılığı |
|---------|---------|------------------------|
| Kolon sessiz kaybı | özellik `default` kalır, hata yok | §3.2 + §3.5 (kolon listesi zorunlu) |
| `SELECT *` kırılganlığı | migration sonrası sessiz veri kaybı | §4.5 G5 |
| `DBNull` patlaması | `InvalidCastException` | §3.6 |
| Tip dönüşümü | `decimal`→`double`, `long`→`int` | §3.6 dönüşüm tablosu |
| Metodu yanlış seçmek | 0 satırda exception / 2 satırda veri kaybı | §3.3 |
| String concat SQL | injection | §3.7 + §7.2 R2 |
| Bağlantı sızıntısı | havuz tükenir | §3.7 `using` |

---

## §2 Kapsam

**İçinde:** `SqlMapper` kolon→özellik eşlemesi, metod seçimi (`Query`/`QueryFirst`/`QuerySingle`/`Execute`/`QueryMultiple`), `DynamicParameters`, `SplitOn`/takma ad, `nullable`+`DBNull`, enum/tarih/decimal dönüşümü, parametreli SQL, injection önleme, bağlantı yaşam döngüsü, async, N+1, hata modları, test stratejisi + PowerShell doğrulama.
**Dışında:** SQL sorgu tasarımı (→ `../query/Query-Template`), şema/migration (→ `migration-template`), repository arayüzü tasarımı (→ `../backend/repository-template`), secret (→ `keys.md`).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya Tipi | Sorumlu | Bu Şablon? |
|---|------------|---------|-------------|
| 1 | `src/VersaCoder.Infrastructure/Dapper/*Mapping.cs` | `database-optimizer` | ✅ |
| 2 | `.ai/mapping/<sorgu>.md` eşleme sözleşmesi | `database-optimizer` | ✅ (bu şablon) |
| 3 | `*Repository.cs` içindeki çağrı (`QueryAsync`) | `developer` | ❌ (kod, §6.5 K4 kanıtı) |
| 4 | SQL gövdesi/parametreler | `database-optimizer` | ❌ → `../query/Query-Template` |
| 5 | Yeni kolon/tablo | `database-optimizer` | ❌ → `migration-template` |
| 6 | Eşleme testleri | `testing` | ❌ (test ≠ şablon, §6.5 K6) |
| 7 | `.ai/adr/ADR-004-*.md` | `architect` | ❌ (karar ≠ eşleme) |

### §2.2 Kapsam Sınırı Testi

| # | Soru | Beklenen | Uymazsa |
|---|------|----------|---------|
| S1 | SQL sorgusunun **çıktısı** bir C# tipine mi bağlanıyor? | `EVET` | sorgu tasarımı → `../query/Query-Template` |
| S2 | Kolon adı listesi yazılabiliyor mu (§3.2)? | `EVET` | şema bilinmiyorsa **DUR** |
| S3 | Birden çok result set var mı? | `EVET` → §3.5 `SplitOn` | tek set ise §3.3 |
| S4 | EF Core / `DbSet` geçen bir şey mi? | `HAYIR` | `EVET` → **RED** (ADR-004) |
| S5 | Kod `await`'li async olarak yazılabiliyor mu? | `EVET` | senkron-only ise §4.5 G8 |

---

## §3 Mimari

### §3.1 Eşleme Künyesi

```yaml
# .ai/mapping/<slug>.md — ilk blok
mapping_id: MAP-NNN
slug: session-row-mapper
query_ref: QRY-014              # ../query/Select-*.md bağlantısı
target_type: VersaCoder.Domain.Session.SessionRow
source_result: single | multiple
dapper_method: QueryAsync<T>    # §3.3
split_on: null                  # §3.5 (multiple ise kolon adı)
nullable_columns: [user_name, tag]
conversions: [enum, datetime, decimal]
parameterized: true             # her zaman true
status: draft | active | deprecated
```

| Alan | Zorunlu | Örnek | Eksikse |
|------|---------|-------|---------|
| `mapping_id` | Evet | `MAP-014` | RED |
| `query_ref` | Evet | `QRY-014` | RED (sorgusuz eşleme olmaz) |
| `target_type` | Evet | `…SessionRow` | RED |
| `dapper_method` | Evet | `QueryAsync<T>` | RED (§3.3) |
| `nullable_columns` | Evet | `[user_name]` | RED (§3.6) |
| `parameterized` | Evet | `true` | RED |

### §3.2 `SqlMapper` Kolon → Özellik Kuralları

| # | Kural | Örnek | İhlal Sonucu |
|---|-------|-------|--------------|
| 1 | Eşleme **varsayılan** kolon adını **özellik adının ta kendisi** alır | `session_id` → `SessionId` (case-insensitive) | — |
| 2 | Kolon adında `_` varsa Dapper onu **sarma işareti** sayar | `user_name` → `UserName` | çakışma §3.5 |
| 3 | Aynı **sarma işareti** iki kolonda → **sessiz üstüme yazma** | `user_name` + `UserName` | ⚠️ RED |
| 4 | Eşleşmeyen kolon **sessizce atlanır**, hata vermez | `SELECT a,b` → tip yalnız `a` | §3.5 G2 |
| 5 | Eşleşmeyen özellik **default kalır** (hata yok) | `Tag = null` | §6.5 K2 |
| 6 | Kolon listesi **`*` ile değil**, tek tek yazılır | §3.7 | G5 |
| 7 | Tipler **birebir** tanımlanır (`long`,`string`,`DateTime`) | `object`/`var` yasak | dönüştürme belirsiz |
| 8 | Aday kolon → özellik eşlemesi **bu tabloya yazılır** | §3.9 | okunmaz sözleşme |

**Eşleme tablosu (dolu örnek — `QRY-014`):**

| # | SQL Kolonu | Sarmalama | Hedef Özellik | Tip | Nullable |
|---|-----------|-----------|----------------|-----|----------|
| 1 | `session_id` | — | `SessionId` | `long` | Hayır |
| 2 | `user_id` | — | `UserId` | `long` | Hayır |
| 3 | `user_name` | `user_name` → `UserName` | `UserName` | `string` | Evet |
| 4 | `created_at` | `created_at` → `CreatedAt` | `CreatedAt` | `DateTime` | Hayır |
| 5 | `file_count` | `file_count` → `FileCount` | `FileCount` | `int` | Hayır |
| 6 | `status` | — | `Status` | `SessionStatus` | Hayır |

### §3.3 Metod Karşılaştırma Tablosu

| Metod | 0 satır | 2+ satır | Dönüş | Tipik kullanım | Async |
|-------|---------|----------|-------|----------------|-------|
| `Query<T>` | boş liste | **tümü** | `IEnumerable<T>` | liste/rapor (Q1, Q6) | `QueryAsync<T>` |
| `QueryFirst<T>` | **exception** | ilkini alır | `T` | "en az 1 satır garantili" | `QueryFirstAsync<T>` |
| `QueryFirstOrDefault<T>` | `null`/`default` | ilkini alır | `T?` | opsiyonel tekil (Q2) | `QueryFirstOrDefaultAsync<T>` |
| `QuerySingle<T>` | **exception** | **exception** | `T` | aggregate tek değer | `QuerySingleAsync<T>` |
| `QuerySingleOrDefault<T>` | `null` | **exception** | `T?` | 0 veya 1 beklenen | `QuerySingleOrDefaultAsync<T>` |
| `Execute` | 0 (etkilenen satır) | — | `int` | `INSERT/UPDATE/DELETE` (Q5) | `ExecuteAsync` |
| `ExecuteScalar` | `null` | — | `object` | tek değer (`COUNT`) | `ExecuteScalarAsync<T>` |
| `QueryMultiple` | — | — | `GridReader` | 2+ result set (D5) | `QueryMultipleAsync` |

**Seçim kuralı:** sonuç **çok satır** ise → `QueryAsync`; **0 veya 1** → `QueryOrDefault`; **kesin 1** → `QuerySingle`; **yazma** → `Execute`. Yanlış seçilen metod §7.2 R5'tir.

### §3.4 `DynamicParameters` Kalıbı

```text
-- SQL (../query/Query-Template §3.3'ten gelir)
SELECT  s.session_id, s.user_id, u.user_name, s.created_at, s.file_count, s.status
FROM    session AS s
        INNER JOIN user AS u ON u.user_id = s.user_id
WHERE   s.user_id = @p1
  AND   (@p2 IS NULL OR s.tag = @p2)
ORDER BY s.created_at DESC, s.session_id DESC
LIMIT   @p3 OFFSET @p4;

-- C# karşılığı
var p = new DynamicParameters();
p.Add("p1", {{KULLANICI_ID}}, DbType.Int64);
p.Add("p2", {{ETIKET}},      DbType.String);   -- null kabul edilir
p.Add("p3", {{SAYFA_BOYU}},  DbType.Int32);    -- <= 500
p.Add("p4", {{DESI}},        DbType.Int32);
var rows = await conn.QueryAsync<{{HEDEF_TIP}}>(sql, p);   -- async + parametreli
```

| Kural | Yazımda | İhlal |
|-------|---------|-------|
| Parametre adı SQL ile **birebir aynı** (`p1`) | `p.Add("p1", …)` | `InvalidCastException` / eksik parametre |
| Tip açık (`DbType.*`) | `DbType.Int64` | sağlayıcı varsayımı |
| `IN` listesi | her eleman ayrı `pK` | tek değer verilmez |
| SQL metni **değişkendir değil sabittir** | `sql` sabit string + `@pN` | concat RED |
| Aynı `DynamicParameters` 2. sorguda yeniden kullanılır | yeni instance | yarış durumu |

### §3.5 Kolon Eşleşmezse: `SplitOn`, `_` Takma Adı

| Durum | Belirti | Çözüm |
|-------|---------|-------|
| Kolon hedefte yok | özellik **default** kalır, hata yok | §3.2 #4 — kolon listesi + §6.5 K2 testi |
| Öznitelik SQL'de yok | `Query` çalışır, `null` döner | `ALTER`(migration) ya da `COALESCE` (§3.6) |
| İki kolon aynı sarmaya düşer | **sessiz üstüme yazma** | takma ad: `u.user_name AS user_name_alias` |
| Çakışma kuralı | `user_name` ve `UserName` birlikte | alias **`_` kullanmayan** ad (`userNameCol`) |
| `QueryMultiple` 2. set | tip ikinci seti okur | `splitOn: "session_id"` — ayrım kolonu |
| Ayrım kolonu yok | setler birbirine kayar | her set'e **aynı adlı** ayrım kolonu ekle |

**SplitOn tablosu:**

| Set | İçerik | `splitOn` | Ayrım Kolonu |
|-----|--------|-----------|---------------|
| 1 | `SessionRow` | `session_id` | `session_id` |
| 2 | `FileRow` | `session_id` | `session_id` |
| 3 | `UserRow` | `user_id` | `user_id` |

### §3.6 Nullable, DBNull ve Dönüşümler

| Kaynak | Hedef | Kural | Hata Modu |
|--------|-------|-------|-----------|
| `NULL` TEXT | `string` | `?? string.Empty` ya da nullable bırak | `NullReferenceException` |
| `NULL` INT | `long?`/`int?` | nullable tip kullan | `InvalidCastException` |
| `INTEGER` | `enum` | `Convert.ToInt32` → `(T)` | tanımsız değer → §3.8 H4 |
| `TEXT` ISO-8601 | `DateTime` | `DateTime.Parse(…, CultureInfo.InvariantCulture)` | yerel ayar farkı |
| `TEXT` + offset | `DateTimeOffset` | `DateTimeOffset.Parse` | kayıp offset |
| `NUMERIC` | `decimal` | **`decimal`**, `double` **DEĞİL** | para yuvarlama kaybı |
| `BLOB` | `byte[]` | doğrudan | — |
| `0/1` | `bool` | `!= 0` | SQLite `INTEGER` |

**Dönüşüm kanıtı:** her dönüşüm satırı §6.5 test verisiyle (T3 enum · T4 tarih · T5 decimal) kanıtlanır.

### §3.7 Parametreli SQL, Injection Önleme ve Bağlantı Yaşam Döngüsü

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Parametre | `WHERE id = @p1` | `"...id=" + id` |
| 2 | Tablo/sütun adı | sabit kod içinde | kullanıcı girdisiyle |
| 3 | `IN` listesi | `(@p1,@p2,@p3)` | `IN ('" + x + "')` |
| 4 | Bağlantı | `using var conn = factory.Open();` | paylaşılan statik `conn` |
| 5 | Async | `await conn.QueryAsync…` | `conn.Query(...).Result` |
| 6 | Transaction | `conn.BeginTransaction()` + `Commit/Rollback` | yazmada transactionsız |
| 7 | Timeout | `conn.CommandTimeout = 30` | sonsuz bekleme |
| 8 | N+1 | tek sorgu + `IN`/JOIN | döngüde sorgu |

**Yaşam döngüsü (kutu):**

```text
+----------------------------------------------------------------+
| [1] factory.Open()  using IDbConnection (SQLite mode=ro)       |
|    v                                                           |
| [2] DynamicParameters  p.Add(p1..pN, DbType.*)                 |
|    v                                                           |
| [3] conn.QueryAsync<T>(sql, p)   -- sql SABIT, @pN parametre   |
|    v                                                           |
| [4] SqlMapper  kolon -> ozellik (§3.2) + DBNull (§3.6)         |
|    v                                                           |
| [5] Dispose  -- using blogu kapandi, havuza geri dondu         |
+----------------------------------------------------------------+
```

**Ne anlatır:** bağlantının tek yönlü yaşam döngüsü — `using` dışında kalan her çağrı **bağlantı sızıntısı**dır.
**Doğrulama:** §5.3/1 — satır uzunlukları 66'ya eşit olmalı.

### §3.8 N+1, Hata Modları ve Test Stratejisi

**N+1:**

| Desen | Belirti | Önlem |
|-------|---------|-------|
| `foreach` içinde `Query` | satır başına 1 sorgu | tek sorgu + `IN (@pK…)` |
| Satır başına 2. tip | dosya listesi ayrı sorgu | `QueryMultiple` (D5) |
| Sayfa başına `COUNT` | N+1 toplam | ayrı özet sorgusu |
| `QuerySingle` döngüsü | "bulamayınca" sorgu | `FirstOrDefault` + önbellek |

**Hata modları:**

| # | Hata | Exception | Çözüm |
|---|------|-----------|-------|
| H1 | Kolon uyuşmazlığı | **yok** (sessiz default) | §6.5 K2 eşleme testi |
| H2 | 0/2 satır, `QuerySingle` | `InvalidOperationException` | §3.3 doğru metod |
| H3 | `DBNull` → değer tip | `InvalidCastException` | §3.6 nullable |
| H4 | Geçersiz enum değeri | `InvalidCastException`/`Overflow` | bilinmeyen değer `Unknown` |
| H5 | Parametre adı uyuşmazlığı | `MissingParameterException` | §3.4 `p1` birebir |
| H6 | Bağlantı sızıntısı | havuz exhaustion | §3.7 `using` |
| H7 | String concat | injection | §7.2 R2 |
| H8 | Eşzamanlı paylaşımlı `IDbConnection` | `InvalidOperationException` | çağrı başına `Open` |

**Test stratejisi:** birim (kolon→özellik, `DBNull`, enum/tarih/decimal) · entegrasyon (geçici `versacoder.db` kopyası, salt okunur) · sözleşme (§3.2 ↔ `PRAGMA table_info`) · güvenlik (concat / `SELECT *`) · regresyon (`QuerySingle` 0/2 satır) — kanıt §6.5.

### §3.9 Dolu Örnek ve Tam Kalıp

**Dolu örnek (`MAP-014` — `QRY-014` için gerçek eşleme):**

```text
MAP-014  session-row-mapper   query_ref=QRY-014   method=QueryAsync<SessionRow>
SQL:  SELECT s.session_id, s.user_id, u.user_name, s.created_at,
             s.file_count, s.status
      FROM session s INNER JOIN user u ON u.user_id = s.user_id
      WHERE s.user_id = @p1 ... LIMIT @p3 OFFSET @p4
Map:  session_id -> SessionId (long, NN)      user_name -> UserName (string, NULL)
      created_at  -> CreatedAt (DateTime, ISO) file_count -> FileCount (int, NN)
      status      -> Status (SessionStatus)    user_id -> UserId (long, NN)
Nasil: SqlMapper varsayilan esleme + DynamicParameters(p1..p4, DbType.*)
Risk : user_name LEFT JOIN -> NULL -> ?? string.Empty   (§3.6)
```

**Tam kalıp (`.ai/mapping/<slug>.md` için `{{…}}` doldurulur):**

````markdown
---
title: "MAP-NNN — {{SLUG}}"
type: mapping
category: infrastructure
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: primary
---
# MAP-NNN — {{SLUG}}

**Zorunlu Bağlantılar:** [[../index]] · [[../query/Query-Template]] · [[../infrastructure/migration-template]] · [[../adr/adr-data-template]] · [[../../architecture/index.md]] · [[../CLAUDE]]

## §1 Amaç — {{BIR_CUMLE}}
## §2 Eşleme Tablosu
| # | SQL Kolonu | Hedef Özellik | Tip | Nullable |
|---|-----------|----------------|-----|----------|
| 1 | {{KOLON}} | {{OZELLIK}} | {{TIP}} | {{EVET_HAYIR}} |
## §3 Metod + Parametre
- Metod: {{QueryAsync<T> | QuerySingleAsync<T> | ExecuteAsync | QueryMultipleAsync}}
- Parametre: @p1..@pN via DynamicParameters (DbType.*)
## §4 Dönüşümler — {{ENUM}} · {{TARIH}} · {{DECIMAL}}
## §5 Test + Kabul — {{TEST_1}} … {{TEST_N}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Çıktı | Atlanırsa |
|---|------|-------|-----------|
| 1 | Girdi sorguyu al | `query_ref` (QRY-NNN) | kopuk sözleşme |
| 2 | Kolon listesini tek tek yaz | §3.2 tablosu | sessiz kayıp |
| 3 | Metodu seç | §3.3 | 0/2 satır patlaması |
| 4 | `DynamicParameters` | §3.4 | parametre uyuşmazlığı |
| 5 | `SplitOn`/alias | §3.5 | setlerin kayması |
| 6 | Nullable + dönüşüm | §3.6 | `DBNull`/enum hatası |
| 7 | `using` + async + N | §3.7 | sızıntı / enjeksiyon |
| 8 | Hata + test stratejisi | §3.8 | kanıtsız güven |
| 9 | Doğrulama + Red Team | §5.3, §7.2 | RED |

### §4.2 Onay Akışı

| Aşama | Rol | Ne Onaylar | Süre |
|-------|-----|-----------|------|
| 1 | `database-optimizer` | eşleme + §5.3 çıktısı | 20 dk |
| 2 | `reviewer` | §6.1 (17/17) + metod seçimi | 10 dk |
| 3 | `security` | §3.7 + §4.5 G3/G5 | 8 dk |
| 4 | `testing` | §6.5 test verisi 5/5 | 15 dk |
| 5 | `master-orchestrator` | Quality Report → `log.md` | 5 dk |

### §4.3 Ölçek ve Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Dosya uzunluğu | 480–660 satır | dışı RED |
| Dosya boyutu | 15–32 KB | dışı RED |
| Tablo satırı | ≥ 25 (her §'de tablo) | eksik RED |
| Eşleme satırı | her SELECT kolonu için 1 satır | eksik = G6 |
| Async oranı | `*Async` (senkron çağrı yok) | §4.5 G8 |
| `CommandTimeout` | ≤ 30 sn | §3.7 #7 |
| Kodlama | UTF-8 BOM'suz | Latin-1 RED |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm | Zorunlu Güncelleme |
|-----------|-------|---------------------|
| Kolon eklendi/çıkarıldı | `minor` | §3.2 + §6.5 K2 |
| Metod değişti (`Query`→`QuerySingle`) | `minor` | §3.3 + test |
| Dönüşüm değişti (`double`→`decimal`) | `minor` | §3.6 + test |
| Sadece yazım | `patch` | `updated` |
| Sorgu `deprecated` | `major` | `status` + §7.1 |
| Migration ile şema değişti | — | `migration-template` ile birlikte |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu | Tespit |
|---|-----------|--------------|--------|
| G1 | Başka vault'tan içerik kopyalanması | RED + sil | §7.2 R11 |
| G2 | **EF Core kalıntısı** (`DbSet`, `DbContext`, `EntityFramework`) | **DUR** (ADR-004) | §5.3/2 |
| G3 | String concat SQL (`$"...{x}"`, `string.Format`) | **DUR** | §5.3/3 |
| G4 | `SELECT *` kullanan eşleme | **DUR** | §5.3/3 |
| G5 | Parametre adı SQL ile uyuşmuyor (`p1` ≠ `@p1`) | RED | §3.4 |
| G6 | Kolon→özellik tablosu eksik (sessiz kayıp) | RED | §6.5 K2 |
| G7 | `DynamicParameters` `DbType` belirtilmeden | RED | §3.4 |
| G8 | `.Result` / `.Wait()` (sync-over-async) | RED | §3.7 #5 |
| G9 | `using`'siz `IDbConnection` | **DUR** | §3.7 #4 |
| G10 | Döngüde sorgu (N+1) | RED | §3.8 |
| G11 | `decimal` → `double` dönüşümü | RED | §3.6 |
| G12 | Mojibake / ölçüm iddiasız "çalışıyor" | RED | §5.3/4–6 |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Sorguyu bağla | `QRY-NNN` | `query_ref` | database-optimizer | 5 dk |
| 2 | Kolonları listele | SQL + `PRAGMA` | §3.2 tablosu | database-optimizer | 10 dk |
| 3 | Metodu seç | §3.3 | `dapper_method` | database-optimizer | 5 dk |
| 4 | Parametre yaz | §3.4 | `DynamicParameters` | database-optimizer | 10 dk |
| 5 | `SplitOn`/alias | §3.5 | §3.5 tablosu | database-optimizer | 5 dk |
| 6 | Nullable + dönüşüm | §3.6 | §3.6 tablosu | database-optimizer | 10 dk |
| 7 | `using` + async + N+1 | §3.7–§3.8 | 2 tablo | database-optimizer | 10 dk |
| 8 | Test + doğrulama | §6.5, §5.3 | 0 ihlal | testing | 15 dk |
| 9 | Onay + kayıt | §6.1 + `log.md` | Quality Report | reviewer | 5 dk |
| | **Toplam** | | | | **≈ 75 dk** |

### §5.2 Hata Modları

| # | Hata | Belirti | Kök Neden | Düzeltme |
|---|------|---------|-----------|----------|
| 1 | Sessiz veri kaybı | özellik `default` | kolon≠özellik | §3.2 + alias |
| 2 | `InvalidCastException` | `DBNull` | nullable tip yok | §3.6 |
| 3 | `QuerySingle` patlıyor | 0/2 satır | yanlış metod | §3.3 |
| 4 | Parametre hatası | eksik `pN` | ad uyuşmazlığı | §3.4 |
| 5 | Set kayması | `QueryMultiple` yanlış veri | `splitOn` yok | §3.5 |
| 6 | Bağlantı sızıntısı | havuz exhaustion | `using` yok | §3.7 |
| 7 | Yavaş liste | 10.000 sorgu | N+1 | §3.8 |
| 8 | Enjeksiyon | concat sızıntısı | tembellik | §7.2 R2 |

### §5.3 Salt Okunur Doğrulama Komutları

Aşağıdaki 6 komut **hiçbir dosyayı/değeri değiştirmez** (salt okunur).

```powershell
# 1) ÖLÇÜM — 480-660 satır / 15-32 KB + kutu hizası (varsa 66)
$p = 'C:\versacoder\.ai\.templates\infrastructure\dapper-mapping-template.md'
$t = [System.IO.File]::ReadAllText($p)
"satir={0} (480-660)  kb={1} (15-32)" -f (($t -split "`n").Count), [math]::Round((Get-Item $p).Length/1KB,1)
Get-Content -LiteralPath $p | Where-Object { $_ -match '^[+|]' -and $_.Length -ne 66 } |
  ForEach-Object { "HIZASIZ: " + $_.Length }
```

```powershell
# 2) EF CORE KALINTISI TARAYICISI — sonuç 0 OLMALI (ADR-004)
Select-String -Path 'C:\versacoder\src\**\*.cs','C:\versacoder\*.csproj' `
  -Pattern 'EntityFramework|DbSet<|DbContext|Microsoft\.EntityFrameworkCore' -ErrorAction SilentlyContinue |
  Measure-Object
```

```powershell
# 3) CONCAT + 'SELECT *' TARAYICISI — iki değer de 0 OLMALI
$q = [System.IO.File]::ReadAllText($p)
"concat={0}  star={1}  async={2}  using={3}" -f `
  ([regex]::Matches($q,'(?i)\$"[^"]*\{|string\.Format').Count), `
  ([regex]::Matches($q,'(?i)SELECT\s+\*').Count), `
  ([regex]::Matches($q,'QueryAsync|ExecuteAsync').Count), `
  ([regex]::Matches($q,'using\s+var\s+conn|using\s*\(.*Open').Count)
```

```powershell
# 4) KODLAMA + PLACEHOLDER + EF KELİMESİ (bu dosya) — 0/0/0
$m = Select-String -Path $p -Pattern '[\u00C3\u00C2\u00C4\uFFFD]' | Measure-Object   # mojibake = 0
$ph = Select-String -Path $p -Pattern '\{\{[A-Z0-9_]+\}\}' | Measure-Object
"mojibake={0}  placeholder={1}" -f $m.Count, $ph.Count
```

```powershell
# 5) DOTNET ARAÇ ZİNCİRİ (salt okunur — derleme/yazma YOK)
dotnet --list-sdks
dotnet --info
Get-ChildItem 'C:\versacoder\src' -Recurse -Filter *.csproj -ErrorAction SilentlyContinue |
  Select-Object -First 5 -ExpandProperty FullName
```

```powershell
# 6) SQLITE SALT OKUNUR — PRAGMA kolon listesi ↔ §3.2 eşlemesi sözleşmesi
$db = 'C:\versacoder\db\versacoder.db'
if (Test-Path $db) {
  $ro = "file:$($db -replace '\\','/')?mode=ro"
  sqlite3 $ro "PRAGMA table_info(session);"                # kolon adları
  sqlite3 $ro "SELECT sql FROM sqlite_master WHERE name='session';"
  sqlite3 $ro "SELECT COUNT(*) FROM session;"               # test verisi satır sayısı
} else { "VERIFICATION REQUIRED: versacoder.db yok (§1.2 E5)" }
```

**Geçme kriteri:** #1 aralıkta + hizasız 0 · #2 = 0 · #3 `concat=0, star=0, async≥1, using≥1` · #4 = 0/0 · #5 SDK listesi · #6 kolon listesi gelir.
**Not:** `sqlite3`/`src/` yoksa (§1.2 E3–E5) çıktı `VERIFICATION REQUIRED` kalır; **kod/DB değişikliği bu şablonun işi değildir.**

### §5.4 Quality Report

```markdown
## Dapper Mapping Kalite Raporu — YYYY-MM-DD

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Dosya satır / KB | N / N | 480-660 / 15-32 | 🟢/🔴 |
| Tablo satırı | N | >= 25 | 🟢/🔴 |
| Kutu hizasız satır (§5.3/1) | N | 0 | 🟢/🔴 |
| EF Core kalıntısı (§5.3/2) | N | 0 | 🟢/🔴 |
| Concat / SELECT * (§5.3/3) | N | 0 | 🟢/🔴 |
| Async + using kanıtı (§5.3/3) | N | >= 1 / >= 1 | 🟢/🔴 |
| Test verisi (§6.5) | N/6 | 6/6 | 🟢/🔴 |

**Karar:** 🟢 `Infrastructure/Dapper/`'e serbest / 🔴 düzeltmeye gönderildi
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] Frontmatter 7 alan tam (`title,type,category,status,version,updated,authority: primary`)
- [ ] H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı)
- [ ] §1.1–§1.4 dolu (§1.2 gerçek disk kanıtı + ⚠️ + yorum yasağı)
- [ ] §2.1–§2.2 dolu
- [ ] §3.1–§3.9 dolu; §3.9'da dolu örnek + `{{…}}` kalıp bloğu
- [ ] §3.2 eşleme kuralları (8 madde) + dolu kolon tablosu
- [ ] §3.3 metod karşılaştırma tablosu (≥8 metod, async sütunu)
- [ ] §3.4 `DynamicParameters` kalıbı (`@p1..@pN`, `DbType`)
- [ ] §3.5 `SplitOn` + `_` takma adı çakışması
- [ ] §3.6 nullable/`DBNull` + enum/tarih/decimal dönüşümü
- [ ] §3.7 parametreli SQL + injection + `using` + async (kutu dahil)
- [ ] §3.8 N+1 + ≥8 hata modu + test stratejisi
- [ ] §4.5 G1..G12 (≥10 madde)
- [ ] §5.1 9 adım + ~75 dk
- [ ] §5.2 ≥8 hata modu
- [ ] §5.3 **6 salt-okunur komut** (EF + concat taraması, `PRAGMA`, dotnet)
- [ ] §5.4 Quality Report bloğu
- [ ] §6.5 kabul kriterleri (≥8) + test verisi (≥5)
- [ ] §7.2 Red Team ≥10 satır
- [ ] Kutu satırı = 66 · `{{…}}` yalnız kalıp bloklarında · mojibake = 0

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Aksiyon |
|--------|---------|
| §5.3/2 > 0 (EF geçiyor) | **DUR** — ADR-004 ihlali, satır silinir |
| §5.3/3 `concat > 0` | **DUR** — `@pN` + `DynamicParameters` |
| §5.3/3 `star > 0` | **DUR** — kolon listesi §3.2 |
| Eşleme tablosu eksik | **DUR** — her kolon için satır |
| `using` kanıtı 0 | **DUR** — §3.7 |
| Metod `QuerySingle` ama 0 satır mümkün | **DUR** — §3.3 |
| Test verisi yok (§6.5) | **DUR** — 5 satır zorunlu |
| Kanıtsız "eşleniyor" iddiası | **DUR** — §5.3/6 çıktısı |

### §6.3 Sürüm ve Geçiş Kuralları

| Değişiklik | Sürüm | Etkilenen | Kayıt |
|-----------|-------|-----------|-------|
| İlk yazım | `0.1.0 → 1.0.0` | tek dosya | `updated` + `log.md` |
| Kolon eklendi | `minor` | §3.2 + §6.5 K2 | Quality Report |
| Metod değişti | `minor` | §3.3 + test | test çıktısı |
| Dönüşüm değişti | `minor` | §3.6 | §4.4 |
| Şema değişti | — | `migration-template` | iki dosya birlikte |
| Sorgu kaldırıldı | `major` | `status: deprecated` | §7.1 |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi |
| `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm biçimi, §5.3/§5.4 standardı |
| `.ai/.templates/query/Query-Template.md` | girdi sorgu (`query_ref`) |
| `.ai/.templates/infrastructure/migration-template.md` | kolon şemasının kaynağı |
| `.ai/.templates/adr/adr-data-template.md` | ADO.NET + Dapper kararı (ADR-004) |
| `src/VersaCoder.Infrastructure/Dapper/*.cs` | bu şablondan üretilen gerçek kod |

### §6.5 Envantere Özgü Kontroller + Kabul Kriterleri

**Test verisi (≥5 satır):**

| # | Veri | Amaç | Beklenen |
|---|------|------|----------|
| T1 | 3 satırlı liste | `QueryAsync` çoklu | 3 nesne, sıralı |
| T2 | `user_name = DB NULL` | nullable | `?? string.Empty`, exception yok |
| T3 | `status = 999` | enum tanimsiz | `Unknown`, exception yok |
| T4 | `created_at = 2026-09-25T10:00:00Z` | tarih | `DateTime`/`DateTimeOffset` doğru |
| T5 | `amount = 19.99` | decimal | birebir, `double` **değil** |
| T6 | `tag = "a'b; DROP"` | injection | literal, DB sağlam |

**Kabul kriterleri:**

| # | Kriter | Kanıt |
|---|--------|-------|
| K1 | §5.3/2 = 0 (EF yok) | PowerShell çıktısı |
| K2 | Eşleme testi: her kolon doğru özellikte | test çıktısı |
| K3 | `PRAGMA table_info` ↔ §3.2 tablosu birebir | §5.3/6 |
| K4 | `using` + `*Async` kanıtı ≥ 1 | §5.3/3 |
| K5 | Test verisi T1–T6 geçer (6/6) | test çıktısı |
| K6 | `QuerySingle` 0/2 satır davranışı doğrulanmış | negatif test |
| K7 | N+1 taraması: döngüde sorgu = 0 | kod incelemesi |
| K8 | Dosya 480–660 satır / 15–32 KB, kutu 66 | §5.3/1 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin |
|--------|-----|---------|
| Şablon envanteri | `.ai/.templates/index.md` | kayıt, §3.7 kalite sözleşmesi |
| Biçim referansı | `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm, §5.3, §5.4 |
| Sorgu şablonu | `.ai/.templates/query/Query-Template.md` | girdi SQL + kolon listesi |
| Migration şablonu | `.ai/.templates/infrastructure/migration-template.md` | kolon şeması kaynağı |
| Veri kararı (ADR) | `.ai/.templates/adr/adr-data-template.md` | ADO.NET + Dapper, EF yasak |
| Dizin kuralı | `.ai/.templates/CLAUDE.md` | erişim sırası |
| Katman gerçeği | `.ai/architecture/index.md` | §1.2 E8 — ⚠️ VERIFICATION REQUIRED |
| Ürün adı | VERSA CODER / VersaCoder | CoreMusic kullanılmaz |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Sinyal | Doğrusu | Kayıp |
|---|------|--------|---------|-------|
| R1 | **EF Core kalıntısı** (`DbContext`/`DbSet`) | §5.3/2 > 0 | saf ADO.NET + Dapper (ADR-004) | RED + refactor |
| R2 | **String concat SQL** | §5.3/3 `concat > 0` | `@p1..@pN` + `DynamicParameters` | injection |
| R3 | **Kolon sessiz kaybı** | özellik `default` | §3.2 tablosu + K2 testi | yanlış veri, hata yok |
| R4 | `SELECT *` ile eşleme | §5.3/3 `star > 0` | tek tek kolon listesi | migration'da sessiz kopuş |
| R5 | Yanlış metod (`QuerySingle` + 0 satır) | `InvalidOperationException` | §3.3 seçim kuralı | üretim exception'ı |
| R6 | `using`siz bağlantı | havuz exhaustion | §3.7 #4 | uygulama kilitlenir |
| R7 | `.Result`/`.Wait()` | deadlock riski | `await *Async` | asılı UI |
| R8 | Parametre adı uyuşmazlığı (`p1` vs `@p1`) | eksik parametre | §3.4 birebir | çalışma hatası |
| R9 | `decimal` → `double` | para yuvarlama | `decimal` (§3.6) | finansal sapma |
| R10 | `DbNull.Value`'ı değere dönüştürmek | `InvalidCastException` | nullable + `??` (§3.6) | çökme |
| R11 | Başka vault'tan eşleme kopyalamak | `coremusic`/`versacoder.ai` | yalnız yapı referansı | G1 ihlali |
| R12 | "Eşleniyor" diye test olmadan | §6.5 boş | 6 test + `PRAGMA` kanıtı | kanıtsız onay |
