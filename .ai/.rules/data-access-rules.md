---
title: "VersaCoder — Veri Erişimi Kuralları (ADO.NET + Dapper)"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Veri erişim tek yolu: ADO.NET + Dapper, EF Core yasak, parameterized-only, provider abstraction (Sqlite/SqlClient), migration = saf SQL + __Migrations (checksum), timeout/connection/transaction/N+1 kuralları (CLAUDE.md G2 derinliği)"
---

# VersaCoder — Veri Erişimi Kuralları (`.rules/data-access-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [security-rules.md](./security-rules.md) · [coding-standards.md](./coding-standards.md) · [solid-rules.md](./solid-rules.md) · [testing-rules.md](./testing-rules.md)

> **Tek cümlelik sorumluluk:** *VersaCoder'da veritabanına nasıl erişilir, EF Core neden yasak, migration/timeout/connection/transaction kuralları nelerdir* sorusunun tek cevabıdır.
> Bu dosya `CLAUDE.md` **G2 (EF Core Yasak)** guardrail'inin teknik derinliğidir; çelişirse `CLAUDE.md` kazanır.

---

## §1 Kapsam

**İçinde:** tüm SQL okuma/yazma, provider abstraction, migration, timeout, bağlantı yaşam döngüsü, transaction, N+1, Dapper eşleme.

**Dışında:** sorgu *tasarımı* raporları (`.templates/query/`), secret/şifreleme (`security-rules.md` SEC-02/07), repository isimlendirmesi (`naming-conventions.md`).

> **YASAK (G2):** `Microsoft.EntityFrameworkCore*` · `DbContext` · LINQ-to-Entities · EF `migrate` · `$"SELECT ... {deger}"` birleştirmesi.
> **İhlalde:** CRITICAL — **build RED**; kod silinir, ADO.NET + Dapper ile yeniden yazılır.

### §1.1 Tek Yol

```
Application (handler/servis)
      |  I...Repository arayuzu (SOL-05)
      v
Infrastructure (Dapper repository)
      |  IDbConnection + parametreli komut (DA-03) + CommandTimeout (DA-09)
      v
IDbConnectionFactory  <-- provider abstraction (DA-04)
      |                    |
      v                    v
Microsoft.Data.Sqlite   Microsoft.Data.SqlClient
(birincil: yerel)       (ikincil: kurumsal)
```

---

## §2 Kurallar (DA-01 .. DA-16)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| DA-01 | **Yol tek**: ADO.NET + Dapper ince katman. Sorgu yalnız repository'de (`Infrastructure`); servis/form SQL görmez | Tek yol = tek denetim noktası, tutarlı hata/timeout davranışı | `Application`/`Domain` içinde SQL metni; formda `Query(` | SQL'i repository'ye taşı (SOL-01) |
| DA-02 | **EF Core yasak (G2) — gerekçe**: (1) güncel EF Core sürümleri **net48'i hedeflemez** (.NET Framework 4.8 yalnız .NET Standard 2.0 tüketir) → ortak kaynak imkânsız; (2) değişen/otomatik SQL planlanabilirliği bozar; (3) migration EF'e kilitlenir; (4) N+1 sessizce çoğalır; (5) denetlenebilir ham SQL yerine soyut sorgu ağacı | İki bacak + denetim + öngörülebilir performans | `.csproj`'de EF paketi; `DbContext` türevi sınıf | Paketi sil; Dapper repository + migration (DA-11) |
| DA-03 | **Yalnız parameterized**: değer **hiç** sorgu metnine girmez; `IN` listesi, `LIKE` kalıbı, sıralama değeri de parametre olur; kolon/tablo adı **allowlist** ile seçilir (parametrelenemez) | Injection tek başına DB'yi ele geçirir; concat = yeni plan her seferinde | `$"..."` + SQL; `string.Format` + SQL; kullanıcı girdisi kolon adı | `@p1..@pN` + `DynamicParameters`; §3.1 |
| DA-04 | **Provider abstraction**: üretim kodu `SqliteConnection`/`SqlConnection` **biliyorsa** ihlal; yalnız `DbConnection`/`IDbConnectionFactory` + `ISqlDialect`; somutu yalnız kayıt noktasında seç | Provider değişiminde uygulamanin değişmemesi gerekir | Serviste `using var c = new SqliteConnection(...)` | `IDbConnectionFactory` enjekte et — §3.2 |
| DA-05 | **Dapper yalnızca ince katman**: `QueryAsync<T>`/`ExecuteAsync`; `dynamic` yerine tip(`record`) satır; `CommandType.Text` sabit; **Dapper dışında ORM yok** | Tip güvenliği + değiştirilebilirlik | `Query<dynamic>` ile kod boyunca dolaşım | `SessionRow` benzeri satır tipleri |
| DA-06 | **SQL metni `const`/sabit alanda**: sorgu gövdesi sabit; dinamik parça yalnız parametre adları/allowlist | Gövde sabitse denetim ve plan yeniden kullanımı mümkün | Sorgu gövdesinin interpolasyonla kurulması | Gövdeyi `const string` yap; değişen kısımı parametrele |
| DA-07 | **Şema bilgisi kodda değil migration'da**: `CREATE TABLE/INDEX` yalnız migration dosyalarında | Elle şema = ortam farkı | Kodda `ALTER TABLE` çalıştırma | Migration ekle (DA-11) |
| DA-08 | **Eşleme kuralı**: kolon = özellik adı (case-insensitive Dapper varsayılanı); `record` ile immutable satır; null `default` değil gerçek `?` tipi | Yanlış eşleme sessizce yanlış veri üretir | Sütun/özellik uyuşmazlığı sessiz `default` | `?` tipi + eşleme testi (`testing-rules.md` TS-11) |
| DA-09 | **CommandTimeout zorunlu**: normal sorgu **15 sn** · rapor/export **60 sn** · migration/bootstrap **300 sn** · `0` (sınırsız) **yasak**; süreler sabitlerde (`DbTimeouts`) | Sınırsız bekleme = UI donması, bağlantı havuzu tükenmesi | `CommandTimeout` yok ya da `0` | `DbTimeouts.Query/Report/Migration` sabitlerini kullan (SEC-12) |
| DA-10 | **Bağlantı yaşam döngüsü**: kısa ömürlü — metot başında `using`/`await using`, iş bitince kapat; **asla** static/paylaşılan `IDbConnection`; async ise `OpenAsync` + `await` | Paylaşılan bağlantı = yarış durumu ve havuz kirliliği | Statik alan `DbConnection`; `Open()` ile uzun ömürlü tutma | Metot içi `using` (CS-15), factory'den her seferinde yeni |
| DA-11 | **Migration = saf SQL + sürüm tablosu `__Migrations` + SHA-256 checksum**: her dosya `V0001__ad.sql` biçiminde; tabloda `Id, Name, Checksum, AppliedAt`; uygulama **başlangıçta** uygulanmış kayıtların dosyayla checksum'ını doğrular, uyuşmazlıkta **başlamaz** (fail fast) | EF migrate yasak (G2); checksum’suz migration sessiz drift üretir | Elle `ALTER`; tablo yok; dosya sonradan değiştirilmiş | Yeni migration dosyası + sıralı uygulama; §3.3 |
| DA-12 | **Geri alma (down) politikası**: her migration'ın `-- down` bloğu ya da gerekçesiyle `-- irreversible` notu vardır; yıkıcı migration onaysız çalışmaz (SEC-15, G7) | Geri alınamaz şema değişimi veri kaybı üretir | `DROP` içeren up migration'sız dosya | down bloğu ya da açık onay + yedek |
| DA-13 | **Transaction politikası**: tek birim iş = tek `DbTransaction`; commit/rollback tek yerde; iç içe transaction **yasak**; `TransactionScope` **yok** (net48 + provider uyumsuzluğu riski); SQLite'da tek yazıcı (DA-14) | Yarım kalmış iş = bozuk veri; dağınık commit noktaları hata kaynağını gizler | `foreach` içinde ayrı ayrı commit; try'siz commit | `using` + `try { ...; tx.Commit(); } catch { tx.Rollback(); throw; }` — §3.4 |
| DA-14 | **SQLite kipleri**: WAL modu açık (eşzamanlı okuma), `PRAGMA foreign_keys=ON`, `busy_timeout` tanımlı; uygulama içinde tek süreç/kilit stratejisi belirli | Varsayılan journal'da yazma kilitleri testleri ve UI'ı seyrekleştirir | Testlerde "database is locked" rastgele hataları | Açılışta PRAGMA seti tek yerde (`SqliteConnectionFactory`) |
| DA-15 | **Audit/soft delete**: kritik tablolarda `CreatedAt/UpdatedAt` (UTC, ISO-8601) ve gerektiğinde `DeletedAt`; zaman **tek** kaynaktan (`TimeProvider`, `CS`/`TS-06`) | Saçılık zaman damgası raporları bozar | Bazı satırlarda `NULL` tarih, bazılarında yerel saat | UTC + `TimeProvider` ile yazım |
| DA-16 | **N+1 yasak**: döngü içinde sorgu çalıştırmak yerine JOIN/`IN` partisi/tek toplu sorgu; kaçınılmazsa toplu `IN` parametresi ve **100'lük sayfa**; sorgu sayısı testle ölçülür | Her satır başına sorgu = O(n) gidiş-dönüş; UI'da milisaniye bütçesini patlatır | `foreach` içinde `await repo.GetAsync(...)`; UI'da 50+ sorgu | Toplu okuma (`IN @p1..@pN`), JOIN, `_db.QueryMultiple`; §3.5 |

> **UYARI:** Bu dosya `__Migrations` adını **kanonik** kabul eder. `CLAUDE.md` G2 ve `keys.md` §3/28 aynı tabloyu `schema_version` olarak anar → **`CONFLICT DETECTED`**: tek tablo adı `__Migrations` (bu görevin açık talimatı), vault metinlerindeki `schema_version` ifadesi **PART 6'da** hizalanacak — bu dosya hariç hiçbir dosya burada değiştirilmez (G10).

---

## §3 Kritik Kuralların Ayrıntısı

### §3.1 DA-03 — Parameterized (BAD -> GOOD)

```csharp
// BAD 1 — string concat (injection acik)
var sql = "SELECT Id FROM Sessions WHERE Title = '" + title + "'";

// BAD 2 — interpolasyon (gorsel temiz, ayni acik)
var sql2 = $"SELECT Id FROM Sessions WHERE Title = '{title}'";

// BAD 3 — string.Format
var sql3 = string.Format("SELECT Id FROM Sessions WHERE Title = '{0}'", title);

// GOOD — sabit gowde + parametre (plan yeniden kullanilir)
const string sql = "SELECT Id, Title FROM Sessions WHERE Title = @Title AND DeletedAt IS NULL";
var rows = await connection.QueryAsync<SessionRow>(
    sql,
    new { Title = title },
    commandTimeout: DbTimeouts.Query,
    cancellationToken: cancellationToken);

// GOOD — IN listesi (parametre sayisi kadar ad uretilir)
const string inSql = "SELECT Id FROM Sessions WHERE Id IN @Ids";
var parameters = new DynamicParameters();
parameters.Add("Ids", ids);                 // Dapper listeyi @Ids olarak genisletir
var found = await connection.QueryAsync<Guid>(inSql, parameters, commandTimeout: DbTimeouts.Query);
```

| Kalıp | Karar |
|---|---|
| `IN` listesi | Dapper liste parametresi (yukarıdaki gibi) |
| Kolon adı dinamikliği | **allowlist**: `allowed.Contains(name)` → sabit SQL dalı |
| Sıralama yönü | `ORDER BY CreatedAt` sabit; yön `@Desc` ile iki sabit dal |
| `LIKE` | kalıp parametre: `@Pattern = $"%{term}%"` |

### §3.2 DA-04 — Provider Abstraction Sözleşmesi

```csharp
namespace VersaCoder.Abstractions.Data;   // ornek: arayuzlerin yasiyagi katman

public enum DatabaseProvider { Sqlite, SqlServer }

public interface IDbConnectionFactory
{
    DatabaseProvider Provider { get; }
    DbConnection Create();          // kapali doner; acmak sahibin isidir
}

public interface ISqlDialect
{
    string ParameterPrefix { get; }              // "@"
    string PagingClause(int skip, int take);     // LIMIT/OFFSET vs OFFSET..FETCH
    string QuoteIdentifier(string name);
}
```

| Sözleşme | Kural |
|---|---|
| Somut sınıf adı nerede görünür? | **Yalnız** `Host` DI kaydı + `Infrastructure` factory |
| Diyalekt farkı | `ISqlDialect` içinde; repository gövdesi provider'dan **kör** |
| Yeni provider | Yeni `IDbConnectionFactory` + `ISqlDialect` implementasyonu; kod değişmez (SOL-02) |
| Test | Geçici SQLite dosyası (`spec/test-strategy.md`) — gerçek sunucu **kullanılmaz** |

### §3.3 DA-11 — Migration Politikası

```
db/migrations/
  +---------------------------+------------------+--------------------+
  | V0001__init.sql           | up: CREATE ...   | down: DROP ...     |
  | V0002__sessions_index.sql | up: CREATE INDEX | down: DROP INDEX   |
  +---------------------------+------------------+--------------------+
        |
        v  uygulama acilisinda
  +----------------------------------+     +---------------------------+
  | __Migrations                    | <-> | dosya SHA-256             |
  | Id | Name | Checksum | AppliedAt|     | (ayni dosyadan uretilir)  |
  +----------------------------------+     +---------------------------+
        |
        +--> checksum uyusmuyorsa  -> FAIL FAST, uygulama BASLAMAZ (G5 tarzi kanit)
        +--> eksik migration varsa -> sirayla uygula (tekrarsiz, tek tek transaction)
```

```sql
-- __Migrations surum tablosu (kanoik ad)
CREATE TABLE IF NOT EXISTS __Migrations
(
    Id         INTEGER PRIMARY KEY,
    Name       TEXT    NOT NULL UNIQUE,
    Checksum   TEXT    NOT NULL,
    AppliedAt  TEXT    NOT NULL            -- ISO-8601 UTC
);
```

| Kural | Değer |
|---|---|
| Dosya adı | `V<4 hane>__<english-kebab>.md` yerine `V0001__init.sql` (SQL, İngilizce slug) |
| Sıralama | dosya adıyla; atlanan numara yasak |
| Uygulama | tek tek `DbTransaction` içinde; başarılıysa `__Migrations` satırı |
| Checksum | SHA-256 dosya içeriği; değişiklik = uygulama açılmaz |
| Seeder | şema migration; veri seed'i ayrı `-- seed` bloğu ve onaylı |

### §3.4 DA-13 — Transaction Örneği

```csharp
// GOOD — tek birim is, tek transaction, tek commit
await using var connection = _connectionFactory.Create();
await connection.OpenAsync(cancellationToken);
await using var tx = await connection.BeginTransactionAsync(cancellationToken);
try
{
    await connection.ExecuteAsync(insertSessionSql, param, tx, commandTimeout: DbTimeouts.Query);
    await connection.ExecuteAsync(insertMessageSql, param, tx, commandTimeout: DbTimeouts.Query);
    await tx.CommitAsync(cancellationToken);
}
catch
{
    await tx.RollbackAsync(cancellationToken);
    throw;                       // bos catch YASAK (CS-05)
}
```

| | |
|---|---|
| **YASAK** | Döngüde satır başına commit · `TransactionScope` · commit'i `catch` dışında bırakmak · timeout'suz transaction |
| **Not** | Uzun transaction (rapor) yerine kısa iş paketleri; DA-09 süreleri geçerli |

### §3.5 DA-16 — N+1 (BAD -> GOOD)

```csharp
// BAD — her oturum icin ayri sorgu (N+1)
foreach (var session in sessions)
    session.Messages = (await _msgRepo.ListAsync(session.Id, ct)).ToList();

// GOOD A — tek sorgu, JOIN + gruplama
const string sql = """
    SELECT s.Id, m.Id, m.Role, m.Content
    FROM Sessions s
    JOIN Messages m ON m.SessionId = s.Id
    WHERE s.Id IN @SessionIds
    ORDER BY s.Id, m.CreatedAt
    """;

// GOOD B — toplu IN (sayfa sinirli), sonuc tek yerde gruplanir
```

| Ölçüm | Eşik |
|---|---|
| Ekran açılışında toplam sorgu | ≤ **10** |
| Listeleme sorgusu | ≤ **1000 ms** yerel SQLite'ta (hedef, `spec/test-strategy.md` §performans) |
| Döngü içinde sorgu | **0** (entegrasyon testi sayacı ile kanıtlanır) |

---

## §4 Review Checklist (incelemede işaretlenir)

- [ ] DA-01: SQL yalnız `Infrastructure` repository'lerinde
- [ ] DA-02: EF Core paketi/referansı = **0** (G2)
- [ ] DA-03: SQL birleştiren satır = **0**; kolon adı allowlist'li
- [ ] DA-04: somut provider yalnız factory/DI'da
- [ ] DA-05: sorgular `QueryAsync<T>` ile tipli
- [ ] DA-06: SQL gövdesi `const`/sabit
- [ ] DA-07: şema değişikliği yalnız migration'da
- [ ] DA-08: satır tipleri `?` dahil doğru; eşleme testi var
- [ ] DA-09: her komutta `CommandTimeout` (`DbTimeouts`), `0` yok
- [ ] DA-10: paylaşılan/static connection = 0
- [ ] DA-11: migration + `__Migrations` + checksum doğrulaması çalışıyor
- [ ] DA-12: yıkıcı migration onaylı/`down`'lu
- [ ] DA-13: tek yerde commit; iç içe transaction yok
- [ ] DA-14: PRAGMA seti (WAL, FK, busy_timeout) açılışta
- [ ] DA-15: zaman UTC + `TimeProvider`
- [ ] DA-16: döngü içi sorgu = 0; sorgu sayacı testi yeşil

---

## §5 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme |
|---|---|---|---|---|
| 1 | `$"SELECT ... {x}"` | DA-03/SEC-03 | injection + plan kaybı | parametreli sorgu |
| 2 | `DbContext`'e dönüş teklifi ("EF daha hızlı") | DA-02 | net48 kırılır, G2 RED | Dapper + migration |
| 3 | `CommandTimeout` yok | DA-09 | sonsuz bekleme | `DbTimeouts` |
| 4 | Static `DbConnection` alanı | DA-10 | yarış / havuz kirliliği | metot içi `using` |
| 5 | Elle `ALTER TABLE` | DA-07/DA-11 | ortam farkı | migration dosyası |
| 6 | Döngüde satır sorgusu | DA-16 | 50+ sorgu, yavaş ekran | toplu `IN`/JOIN |
| 7 | Migration dosyasını sonradan düzenleme | DA-11 | checksum FAIL | yeni `V00xx` dosyası |
| 8 | `foreach` + ayrı commit | DA-13 | yarım veri | tek transaction |
| 9 | Yerel saat damgası | DA-15 | rapor tutarsız | UTC + `TimeProvider` |
| 10 | `Query<dynamic>` yayılımı | DA-05 | sessiz yanlış eşleme | tip satırları |

---

## §6 Doğrulama

- [ ] 16 kural `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] §3'te BAD/GOOD, provider sözleşmesi, migration şeması, transaction ve N+1 örnekleri var
- [ ] `__Migrations` çelişkisi açıkça `CONFLICT DETECTED` olarak işaretli (G10)
- [ ] EF Core hiçbir yerde *öneri* olarak geçmiyor; yalnız yasak
- [ ] Diyagramlar saf ASCII; frontmatter 8 alan, `authority: SSOT`

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G2) | `../CLAUDE.md` |
| 2 | Anahtar: Dapper, ADO.NET, migration, schema_version | `../keys.md` |
| 3 | SQL şablonu (parametre tablosu) | `.ai/.templates/query/Query-Template.md` |
| 4 | Migration şablonu | `.ai/.templates/infrastructure/migration-template.md` |
| 5 | Dapper eşleme şablonu | `.ai/.templates/infrastructure/dapper-mapping-template.md` |
| 6 | Güvenlik çerçevesi | `security-rules.md` |
| 7 | Test stratejisi (geçici SQLite) | `../spec/test-strategy.md` |
