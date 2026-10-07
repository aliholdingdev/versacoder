---
title: "VersaCoder — Yürütme Motoru (Execution Engine)"
type: engine
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Sistem nasıl derlenir ve nasıl çalışır: katman sırası + bağımlılık kuralları, veri erişim motoru, UI motoru, çift-bacaklı build, test motoru, protocol/opencode entegrasyonu, build komutları, hata çözümleme"
---

# VersaCoder — Yürütme Motoru (`engine.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[reports/00-kapsam-analizi.md]] · [[.agents/AGENTS.md]] · [[.templates/documentation/root-md-template.md]] · [[ULTRA-THINKING.md]]

> **Tek cümlelik sorumluluk:** *Sistem hangi sırayla derlenir, hangi katman hangisine referans verebilir, veri/UI/build/test/protocol motorları nasıl çalışır, hata çıkınca ne yapılır* sorusunun tek cevabıdır.
> Bu dosya **teknik yürütme referansıdır**. Kural metni `CLAUDE.md`'dedir (G1..G12), gate tanımı ve part sırası süreç dosyasındadır (SRP — tekrar yok).

---

## İçindekiler

| Bölüm | Alt Konular |
|---|---|
| §0 Özet | Tek ekran cevap |
| §1 Amaç ve Kapsam | İçinde / dışında, motor envanteri |
| §2 Mimari Katman Motoru | Build sırası, ASCII diyagram, bağımlılık tablosu, yasaklar |
| §3 Veri Erişim Motoru | ADO.NET + Dapper, EF Core yasağı, provider soyutlama, parameterized, timeout, şifreli conn-string, migration |
| §4 UI Motoru | DevExpress v26.1, Office2010 ribbon, form üçlüsü, ekran-kontrol eşlemesi |
| §5 Çift Bacaklı Build Motoru | net48 + net10.0-windows, R5, `#if`, CPM, varyantlar |
| §6 Test Motoru | Birim/entegrasyon ayrımı, coverage hedefi, gate eşlemesi |
| §7 Protocol / opencode Motoru | v1.18.32 envanteri, adopt vs refer, sözleşme iskeleti |
| §8 Build Komut Hile Kutusu | Cheat-sheet |
| §9 Hata → Neden → Düzeltme | Troubleshooting tablosu |
| §10 Part ↔ Gate Orkestrasyon Matrisi | Hangi çıktının hangi gate'e kanıt verdiği |
| §11 Motor Kuralları | KURAL / UYARI blokları |
| §12 Doğrulama | Ölçülebilir checklist |
| §13 Referanslar | Kaynak → yol |

---

## §0 Özet (Answer First)

VersaCoder **tek bir çözümde iki bacakta** derlenir: `net48` (legacy kanıt) ve `net10.0-windows` (modern). Derleme sırası **7 katmandır** ve **asla tersinez**: `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`. Veri erişimi **ADO.NET + Dapper** ile ince bir katmandır; **EF Core yasaktır** (G2). UI **WinForms + DevExpress Universal 2026 / v26.1** ve tek ribbon stili `RibbonControlStyle.Office2010` ile sınırlıdır (G3). Test **xUnit + NSubstitute + NetArchTest** ile iki bacakta da yeşil olmalıdır (G5). Protocol tarafında **opencode v1.18.32** yalnız *referans* kaynağıdır — kod kopyalanmaz, sözleşme uyarlanır (G1).

> **KURAL:** Bu dosyadaki hiçbir madde tek başına "bitti" kanıtı değildir. Kanıt = `dotnet build` + `dotnet test` çıktısı, iki bacakta da (G5).

---

## §1 Amaç ve Kapsam

### §1.1 İçinde / Dışında

| İçinde (bu dosyanın) | Dışında (başka SSOT) |
|---|---|
| Katman sırası ve bağımlılık yönü | Guardrail metni → `CLAUDE.md` G1..G12 |
| Veri erişim motorunun çalışma şekli | Hard kural metni → `.rules/data-access-rules.md` (PART 3) |
| UI motorunun çalışma şekli | DevExpress hard kural → `.rules/devexpress-rules.md` (PART 3) |
| Build/test/protocol yürüteçleri | Part sırası ve gate tanımı → süreç dosyası |
| Build komutları ve hata çözümleme | Rol sahipliği → `.agents/AGENTS.md` §3 |
| ADR üretimi (hangi karar ADR açar) | ADR gövdesi → `.decisions/ADR-NNN-*.md` (PART 4+) |

> **UYARI:** Bu dosyaya **kural metni kopyalanmaz**. Kural değişirse `CLAUDE.md` kazanır, burası yalnız *nasıl yürütüldüğünü* anlatır (SSOT ihlali = G4 RED).

### §1.2 Motor Envanteri

| # | Motor | Görev | Kanıt |
|---|---|---|---|
| 1 | Katman motoru | Build sırası + bağımlılık yönü | NetArchTest yeşil |
| 2 | Veri erişim motoru | ADO.NET + Dapper + migration | temp DB migration testi |
| 3 | UI motoru | WinForms + DevExpress v26.1 + form üçlüsü | Designer + build yeşil |
| 4 | Çift-bacaklı build | net48 + net10.0-windows | iki `dotnet build` çıktısı |
| 5 | Test motoru | Birim + entegrasyon + mimari | iki `dotnet test` + coverage |
| 6 | Protocol motoru | opencode uyarlaması, sözleşme | protocol testleri |
| 7 | Denetim motoru | Link tarama, sürüm/EF taraması | 0 bozuk link, 0 EF referansı |

---

## §2 Mimari Katman Motoru (Build Sırası)

### §2.1 Build Sırası

**Sıra zorunlu ve tek yönlüdür.** Numara = derleme sırası; bir katman yalnız **numarasından küçük** olanlara referans verebilir.

| # | Katman | Proje (aday) | Tek sorumluluk | Bu katman yapmaz |
|---|---|---|---|---|
| 1 | **Abstractions** | `VersaCoder.Abstractions` | Contract, `Result<T>`, hata kodu, event tanımı, port interface'leri | Hiçbir implementasyon, hiçbir I/O |
| 2 | **Domain** | `VersaCoder.Domain` | Entity, value object, invariant, domain event | DB, UI, framework tipi |
| 3 | **Application** | `VersaCoder.Application` | Use case / handler, orkestrasyon, portların **sahibi** | Dapper, SqlConnection, DevExpress |
| 4 | **Infrastructure** | `VersaCoder.Infrastructure` | ADO.NET + Dapper, provider, migration, dosya/saat/LLM adapter'ı | İş kuralı, UI |
| 5 | **Protocol** | `VersaCoder.Protocol` | Mesaj şeması, tool sözleşmesi, streaming olayları | Transport, UI, DB |
| 6 | **Host** | `VersaCoder.Host` | Composition root, DI kaydı, config, startup | İş mantığı |
| 7 | **UI / Cli** | `VersaCoder.Desktop` · `VersaCoder.Cli` | WinForms ekranları, ribbon, konsol komutları | Doğrudan SQL, doğrudan Dapper |

> **KURAL:** Katman adı proje adını belirler (`VersaCoder.<Katman>`). Yeni katman eklemek **architect** yetkisidir ve ADR gerektirir.

### §2.2 ASCII Katman-Bağımlılık Diyagramı

```text
  DERLEME SIRASI (yukaridan asagi, her katman oncekileri derler)
  ve IZINLI BAGIMLILIK YONU (ok = referans verebilir)

  +---------------------------------------------------------------------+
  | L7  UI / Cli        VersaCoder.Desktop + VersaCoder.Cli             |
  |     izin: L6 (Host) + L3 (Application)                              |
  |     yasak: L4, L5, L2, L1'e dogrudan erisim                        |
  +----------------------------------+----------------------------------+
                                     | (1)
                                     v
  +---------------------------------------------------------------------+
  | L6  HOST            composition root / DI / config / startup        |
  |     izin: L1 L2 L3 L4 L5  (tek yetkili bagimlilik toplayicisi)      |
  |     yasak: L7'yi referans almamak (Host UI'yi bilmez)               |
  +----------------+---------------------------+------------------------+
                   |                           |
                   | (2)                       | (3)
                   v                           v
  +--------------------------+     +------------------------------------+
  | L5  PROTOCOL             |     | L4  INFRASTRUCTURE                 |
  |      izin: L1            |     |      izin: L3 (port) L2 L1         |
  |      yasak: L2..L4, L6   |     |      yasak: L6, L7                 |
  +------------+-------------+     +-----------------+------------------+
               |                                       | (4)
               |                                       v
               |                     +----------------------------------+
               |                     | L3  APPLICATION                   |
               |                     |      izin: L2 + L1                |
               |                     |      yasak: L4, L6, L7            |
               |                     +-----------------+----------------+
               |                                       | (5)
               |                                       v
               |                     +----------------------------------+
               |                     | L2  DOMAIN                        |
               |                     |      izin: L1                     |
               |                     |      yasak: L3..L7                |
               |                     +-----------------+----------------+
               |                                       | (6)
               |        +------------------------------+
               |        |
               |        v
  +---------------------------------------------------------------------+
  | L1  ABSTRACTIONS      hicbir bagimliligi YOK (dip katman)           |
  +---------------------------------------------------------------------+

  OK KURALLARI
    (1) UI  -> Host + Application          [netici: DI'dan handler alir]
    (2) Host-> Protocol                    [mesaj semasi kaydi]
    (3) Host-> Infrastructure              [provider kaydi]
    (4) Infrastructure -> Application      [port interface'ini UYGULAR]
    (5) Application -> Domain              [entity + invariant]
    (6) Domain -> Abstractions             [Result / event / contract]
    Protocol -> Abstractions (dogrudan, diger her seye kapali)
```

> **UYARI:** Ok asla **geriye** (yüksek → düşük numara) dönmez. Geriye ok = circular dependency = mimari RED ve NetArchTest ile kanıtlanır.

### §2.3 Referans Kuralları Tablosu (Hangi katman hangisine referans verebilir)

| Kaynak ↓ / Hedef → | Abstr. | Domain | App | Infra | Proto | Host | UI/Cli |
|---|---|---|---|---|---|---|---|
| **Abstractions** | — | HAYIR | HAYIR | HAYIR | HAYIR | HAYIR | HAYIR |
| **Domain** | EVET | — | HAYIR | HAYIR | HAYIR | HAYIR | HAYIR |
| **Application** | EVET | EVET | — | HAYIR | HAYIR | HAYIR | HAYIR |
| **Infrastructure** | EVET | EVET | EVET¹ | — | HAYIR | HAYIR | HAYIR |
| **Protocol** | EVET | HAYIR | HAYIR | HAYIR | — | HAYIR | HAYIR |
| **Host** | EVET | EVET | EVET | EVET | EVET | — | HAYIR |
| **UI / Cli** | HAYIR² | HAYIR | EVET | HAYIR | HAYIR | EVET | — |

1. `Infrastructure`, `Application`'ın **port interface'ini** uygular; handler sınıfını instantiate etmez.
2. UI, `Abstractions`'a doğrudan dokunmaz; `Result<T>`'yi `Application` üzerinden alır.

### §2.4 Yasak Bağımlılıklar (Bulan = RED)

| # | Yasak bağımlılık | Neden | Yakalama |
|---|---|---|---|
| 1 | `Application → Infrastructure` | Port sahibi, implementasyonu bilirse DIP kırılır | NetArchTest |
| 2 | `Domain → *` (dip katman dışına) | Domain saf kalmalı, test edilebilirlik | NetArchTest |
| 3 | `Protocol → Infrastructure/Host/UI` | Şema saf olmalı, taşınabilir | NetArchTest |
| 4 | `Host → UI` | Composition root UI'yi bilmez | NetArchTest |
| 5 | `UI → Infrastructure` (doğrudan SQL/Dapper) | Veri erişimi yalnız port üzerinden | Kod incelemesi + grep |
| 6 | Proje ↔ proje **döngüsel** referans | Circular dependency | `dotnet build` hatası + NetArchTest |
| 7 | `tests/ → src/` dışında herhangi bir şey | Test yalnız test projesine yazar | `.agents/AGENTS.md` §3 |
| 8 | İş mantığında `#if` | Kod ikiye bölünür, iki bacak drift eder | Kod incelemesi |

### §2.5 Sözleşme İskeletleri (katmanlar arası tipik imza)

```csharp
// L1 Abstractions — hicbir bagimliligi yok
public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }
    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Fail(Error error) => new(false, default, error);
}

// L3 Application — portu SAHIP olur (interface burada, impl L4'te)
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(long id, CancellationToken ct);
    Task<long> InsertAsync(Session session, CancellationToken ct);
}

public sealed class CreateSessionHandler
{
    private readonly ISessionRepository _sessions;   // DIP: concrete degil
    public CreateSessionHandler(ISessionRepository sessions) => _sessions = sessions;
    // ...
}

// L4 Infrastructure — portu UYGULAR, Domain'i bilmez
public sealed class SessionRepository : ISessionRepository   // L3 tipine referans
{
    private readonly IDataStore _db;                          // provider soyutlamasi (L4 ic)
    // Dapper ile implementasyon — §3.4 parameterized
}

// L6 Host — tek toplayici
services.AddSingleton<IDataStore, SqliteProvider>();
services.AddSingleton<ISessionRepository, SessionRepository>();
```

> **KURAL:** `Infrastructure` yalnız **interface tipine** referans verir; `Application` asla `SqlConnection`/`SqliteConnection` görmez.

### §2.6 Kanıt Aracı (NetArchTest)

```csharp
// tests/VersaCoder.Architecture.Tests/DependencyRules.cs
[Fact] public void Domain_depends_on_nothing_but_abstractions() =>
    Types.InAssembly(typeof(Domain.AssemblyMarker).Assembly)
        .Should().NotHaveDependencyOn("VersaCoder.Infrastructure")
        .And().NotHaveDependencyOn("VersaCoder.Host")
        .And().NotHaveDependencyOn("DevExpress")
        .GetResult().IsSuccessful.Should().BeTrue();

[Fact] public void Application_has_no_ado_net_dependency() =>
    Types.InAssembly(typeof(Application.AssemblyMarker).Assembly)
        .Should().NotHaveDependencyOn("Microsoft.Data.SqlClient")
        .And().NotHaveDependencyOn("Microsoft.Data.Sqlite")
        .And().NotHaveDependencyOn("Dapper")
        .GetResult().IsSuccessful.Should().BeTrue();
```

**Zorunlu mimari test seti (minimum 6):**

| # | Test | Kanıtladığı |
|---|---|---|
| 1 | Domain → yalnız Abstractions | §2.3 satır 2 |
| 2 | Application → yalnız Domain + Abstractions | §2.4 madde 1 |
| 3 | Application → Dapper/SqlClient yok | §3.4 |
| 4 | Protocol → yalnız Abstractions | §2.3 satır 5 |
| 5 | Hiçbir proje `EntityFrameworkCore`'a bağımlı değil | G2 |
| 6 | Hiçbir projede `DevExpress` yalnız Desktop/Cli'de | G3 |

---

## §3 Veri Erişim Motoru

### §3.1 Neden ADO.NET + Dapper, Neden EF Core Yasak

> **KURAL (G2):** Veri erişimi **yalnız** ADO.NET + Dapper ince katmanıdır. `Microsoft.EntityFrameworkCore*`, `DbContext`, LINQ-to-Entities **tamamen yasaktır**. İhlal = CRITICAL, build RED.

| # | EF Core'un yasak olma gerekçesi | Kanıt |
|---|---|---|
| 1 | **net48 bacağında EF Core 9 desteklenmez** → ortak kaynak tek başına imkânsız | `reports/00-kapsam-analizi.md` §2 karar 5 |
| 2 | EF, `DbContext`'i Domain'e taşır → katman kirlenir (§2.4 madde 2 ihlali) | NetArchTest testi 1 |
| 3 | Migration motoru EF'e kilitlenir → saf SQL + `schema_version` hedefi çöker | §3.7 |
| 4 | Query plan öngörülemezliği → timeout/injection denetimi zorlaşır | §3.5 |
| 5 | Paket yüzeyi geniş → iki bacak uyum maliyeti (R5 ile çarpışır) | `CLAUDE.md` §4 |
| 6 | "Daha modern" diye paket eklemek G8 ihlalidir | `CLAUDE.md` G8 |

**Yasaklı ifadeler (kodda görünürse review RED):**

```text
Microsoft.EntityFrameworkCore*
DbContext / DbSet<T>
.UseSqlServer( / UseSqlite(
Database.Migrate() / MigrateAsync()
FromSqlRaw / ExecuteSqlRaw / ExecuteSqlInterpolated   (EF varyantlari)
Include( / ThenInclude(                                (EF loading)
$"SELECT ... {deger}"  veya  "SELECT ... " + deger     (string concat)
```

**İzinli paket envanteri (data access):**

| Paket | Rol | Nerede |
|---|---|---|
| `Microsoft.Data.Sqlite` | SQLite provider (varsayılan, lokal) | Infrastructure |
| `Microsoft.Data.SqlClient` | SQL Server provider (uzak) | Infrastructure |
| `Dapper` | Mikro-ORM — sadece map + command çalıştırma | Infrastructure |
| `System.Data.Common` | `DbConnection`/`DbCommand` soyutlaması | Infrastructure (§3.2) |

> **UYARI:** `System.Data.SqlClient` (eski paket) ile `Microsoft.Data.SqlClient` **karıştırılmaz**. Tek sürümlü CPM (`Directory.Packages.props`) üzerinden tek adres: `Microsoft.Data.SqlClient`.

### §3.2 Provider Soyutlama Arayüzü

Tek arayüz, iki sağlayıcı. `Application` bunu **görmez**; yalnız Infrastructure içinde ve Host kaydında bilinir.

```csharp
// L4 Infrastructure — provider soyutlamasi
public enum DataStoreKind { Sqlite, SqlServer }

public interface IDataStore : IAsyncDisposable
{
    DataStoreKind Kind { get; }

    /// Baglantiyi acar; baglanti stringi sifreli saklanir (§3.6).
    Task OpenAsync(CancellationToken ct);

    /// Sorguyu parameterized calistirir (§3.4). Timeout zorunlu (§3.5).
    Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql, object? args, int? commandTimeoutSeconds = null,
        CancellationToken ct = default);

    Task<int> ExecuteAsync(
        string sql, object? args, int? commandTimeoutSeconds = null,
        CancellationToken ct = default);

    /// Tek seferlik migration calistirmak icin (yalniz migration runner).
    Task<int> ExecuteScriptAsync(string sqlScript, CancellationToken ct);
}

public interface IDataStoreProvider
{
    DataStoreKind Kind { get; }
    IDataStore Create(StoreOptions options);
}

public sealed record StoreOptions(
    string EncryptedConnectionString,   // ham string asla saklanmaz (§3.6)
    int DefaultCommandTimeoutSeconds,    // varsayilan 30
    bool EnableSqlLogging);              // yalniz Development
```

### §3.3 SqliteProvider / SqlClientProvider

```csharp
public sealed class SqliteProvider : IDataStoreProvider
{
    public DataStoreKind Kind => DataStoreKind.Sqlite;
    public IDataStore Create(StoreOptions o) => new SqliteDataStore(o);
}

public sealed class SqlClientProvider : IDataStoreProvider
{
    public DataStoreKind Kind => DataStoreKind.SqlServer;
    public IDataStore Create(StoreOptions o) => new SqlServerDataStore(o);
}

// Host (L6) — tek yetkili kayit
services.AddSingleton<IDataStoreProvider, SqliteProvider>();     // varsayilan
services.AddSingleton<IDataStoreProvider, SqlClientProvider>();
services.AddSingleton<IDataStore>(sp =>
{
    var cfg = sp.GetRequiredService<IStoreConfig>();
    var p = sp.GetServices<IDataStoreProvider>()
              .Single(x => x.Kind == cfg.ActiveKind);
    return p.Create(cfg.ToOptions());
});
```

| Sağlayıcı | Kullanım | Varsayılan | Not |
|---|---|---|---|
| `SqliteProvider` | Lokal oturum, skill, bellek, audit | **EVET** | Dosya tabanlı, taşınabilir, testlerde temp DB |
| `SqlClientProvider` | Uzak/şirket içi depo | Hayır | Opsiyonel; connection string şifreli (§3.6) |

> **KURAL:** Yeni sağlayıcı eklemek = yeni `IDataStoreProvider` implementasyonu + Host kaydı. `switch` dağılımı **Host/Infrastructure dışında** yazılmaz.

### §3.4 Parameterization Kanunu

> **UYARI:** Her sorgu **parameterized**'dır. String birleştirme, interpolasyon veya `$"...{x}"` ile SQL üretmek **CRITICAL** ihlaldir (G2).

```csharp
// YANLIS  --  injection + plan cache kirletme + review RED
var sql = $"SELECT * FROM sessions WHERE name = '{name}'";
var rows = await conn.QueryAsync(sql);

// YANLIS  --  interpolasyon da string concat'tir
var sql2 = $"SELECT * FROM sessions WHERE id = {id}";

// DOGRU  --  parametre adi sabit, deger parametre olarak gider
const string sql = """
    SELECT id, name, created_utc
      FROM sessions
     WHERE name = @name
       AND created_utc >= @sinceUtc
     ORDER BY created_utc DESC
     LIMIT @take
     """;
var rows = await db.QueryAsync<SessionDto>(
    sql, new { name, sinceUtc, take = 50 },
    commandTimeoutSeconds: 15, ct);
```

| # | Kural | Ayrıntı |
|---|---|---|
| 1 | SQL gövdesi **sabit** | Query text kodda sabit; çalışma zamanında üretilmez |
| 2 | Değerler **@ad** ile | Dapper anonymous object / `DynamicParameters` |
| 3 | Dinamik `ORDER BY`/`LIMIT` | Yalnız **beyaz liste** enum üzerinden; asla ham string |
| 4 | Tablo/kolon adı dinamikse | Whitelist sözlüğü + `Enum.TryParse`; kullanıcı girdisi asla |
| 5 | `IN (...)` listesi | Eleman sayısı kadar `@p0..@pN` parametresi üret |
| 6 | LIKE | Parametre içinde `EscapeLike`; `%`/`_` kaçışı |
| 7 | Toplu INSERT | Batch parametre (`@p0_0`, `@p0_1`…) veya `IF NOT EXISTS` tek sorgu |
| 8 | DDL / migration | `ExecuteScriptAsync` ile dosyadan çalışır (§3.7), kullanıcı verisi yok |

### §3.5 Command Timeout Politikası

| Seviye | Varsayılan | Kullanım | Nerede |
|---|---|---|---|
| Global | **30 sn** | Tüm normal sorgular | `StoreOptions.DefaultCommandTimeoutSeconds` |
| Kısa | **5 sn** | Tekil `SELECT`ById, health check | Çağrı yerinde override |
| Uzun | **120 sn** | Migration, rapor, toplu iş | Çağrı yerinde override, log'a yazılır |
| Migration script | **300 sn** | `V0xx__*.sql` uygulaması | Migration runner sabiti |
| Test | **10 sn** | Temp DB entegrasyon testi | Test fixture |

**Kurallar:**

1. `CommandTimeout` **her zaman** açıkça yazılır — değer verilmezse global varsayılan devreye girer ama **hiçbir çağrıda `0` (sınırsız) kullanılmaz**.
2. Timeout = `DbCommandException` → §9 hata tablosu satırı `Timeout ... executing`.
3. Uzun süren sorgu **önce index/kolon incelemesi**, sonra timeout artırımı — timeout artırmak **çözüm değil**, band-aid'dir.
4. Timeout değeri **kodda sabittir**; kullanıcı girdisiyle belirlenemez.

```csharp
// DOGRU: timeout acikca yazilmis, kisa yol
var row = await db.QueryAsync<SessionDto>(
    sql, new { id }, commandTimeoutSeconds: 5, ct);
```

### §3.6 Şifreli Connection String Politikası

> **UYARI:** Ham connection string **asla** `.cs`, `.json`, `.config`, `.md` veya repo içine yazılmaz (G2 + güvenlik).

| # | Kural | Detay |
|---|---|---|
| 1 | **Depolama** | Sadece credential store'da; AES-256-GCM ile şifrelenmiş blob |
| 2 | **Anahtar** | OS keyring / DPAPI (`ProtectedData` net48) veya master key dosyası (ACL korumalı) |
| 3 | **Repo** | Yalnız **şablon**: `connectionStrings: { "store": "<ENCRYPTED>" }` ve `.example` dosyası |
| 4 | **Log** | Connection string log'a asla yazılmaz; `Data Source=***` maskelenir |
| 5 | **Test** | Temp DB path'i test fixture tarafından üretilir; gerçek kullanıcı dizini yok |
| 6 | **Sızıntı** | String literal görüldüse → CRITICAL, anahtar rotasyonu + `log.md` kaydı |
| 7 | **Günlük** | `EnableSqlLogging=true` yalnız Development derlemede (`#if DEBUG`) |

```csharp
// DOGRU: ham metin saklanmaz
var encrypted = await _vault.GetAsync("store.connection");   // credential vault
var options = new StoreOptions(encrypted, DefaultCommandTimeoutSeconds: 30,
                               EnableSqlLogging: _env.IsDevelopment());

// YANLIS  --  CRITICAL
// var cs = "Data Source=C:\\db\\app.db;Password=1234";
```

### §3.7 Migration Motoru (saf SQL + sürüm tablosu)

**Dosya düzeni:**

```text
src/VersaCoder.Infrastructure/
  db/
    migrations/
      V001__init_schema.sql
      V002__sessions_index.sql
      V003__audit_table.sql
      ...
```

**Sürüm tablosu:**

```sql
-- migration runner tarafindan olusturulur
CREATE TABLE IF NOT EXISTS schema_version (
    version     INTEGER     NOT NULL PRIMARY KEY,
    name        TEXT        NOT NULL,
    checksum    TEXT        NOT NULL,   -- dosyanin SHA-256'si
    applied_utc TEXT        NOT NULL,
    duration_ms INTEGER     NOT NULL
);
```

**Uygulama algoritması:**

```text
1. schema_version tablosunu olustur (IF NOT EXISTS)
2. migrations/ klasorundeki dosyalari V001 < V002 < V003 ... SAYISAL sirala
   (metinsel alfabetik siralama degil -- V0010, V002 hatasina dusulmesin)
3. Her dosya icin:
   a. version <= max(schema_version.version)  -> ATLA (zaten uygulanmis)
   b. checksum farkli                         -> DUR ve raporla (demirleme ihlali)
   c. degilse:
      - BEGIN TRANSACTION
      - ExecuteScriptAsync(sqlScript)   [timeout 300 sn]
      - INSERT INTO schema_version(...)
      - COMMIT
4. Beklenmedik hata -> ROLLBACK + hata raporu (konum + neden + duzeltme)
```

| # | Kural | Neden |
|---|---|---|
| 1 | Dosya adı `VNNN__aciklama.sql` | Sıra + okunabilirlik |
| 2 | **Sıralı** uygulama (isim sırası) | Determinizm |
| 3 | **Idempotent** olmalı: `IF NOT EXISTS`, `IF NOT EXISTS` index | Kısmi uygulama sonrası tekrar güvenli |
| 4 | **Tek dosya = tek sürüm**; asla iki dosya tek seferde | Hata izole edilir |
| 5 | Uygulanmış dosya **değiştirilemez** | Checksum ihlali → RED |
| 6 | Geri = `V00N+1__revert_*.sql` | Down migration yok, forward-fix |
| 7 | Migration'da **parametre yok** | Saf DDL/DML, kullanıcı verisi yok |
| 8 | Migration testi **zorunlu** | temp DB'de `schema_version` ilerlemesi doğrulanır |

```sql
-- V002__sessions_index.sql  -- idempotent ornek
CREATE TABLE IF NOT EXISTS sessions (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    name         TEXT    NOT NULL,
    created_utc  TEXT    NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_sessions_created
    ON sessions (created_utc DESC);
```

> **KURAL:** `Database.Migrate()` / EF migration **yoktur**. Migration = dosya + tablo + sıra + idempotency.

### §3.8 Veri Erişim Hata Modları

| Mod | Belirti | Kök neden | Düzeltme |
|---|---|---|---|
| Parametre yok | `Missing parameter` | `@ad` ile argüman adı uyuşmuyor | Dapper anonymous objeyi sorguyla eşleştir |
| Kolon ≠ property | Dapper map boş döner | Kolon adı `snake_case`, property `PascalCase` | `AS` takma adı veya explicit map |
| Timeout | `Timeout expired` | İndeks yok / kilit / çok büyük tablo | §3.5 — önce indeks, sonra timeout |
| Kilit çakışması | `database is locked` (SQLite) | WAL kapalı, eşzamanlı yazma | WAL mode + tek writer kuyruğu |
| Sürüm uyuşmazlığı | Checksum hatası | Uygulanmış migration değiştirilmiş | RED → yeni `V00N+1` ile düzelt |
| Provider kayıp | `No provider registered` | Host kaydı eksik | §3.3 Host kaydını tamamla |
| conn-string sızıntısı | Logda ham string | Maskelenmemiş loglama | CRITICAL → rotasyon + `#3.6` kuralı 4 |

---

## §4 UI Motoru

### §4.1 DevExpress Kanunu

> **KURAL (G3):** **DevExpress Universal 2026 / v26.1** tek sürümdür. Başka sürüm referans edilemez, restore edilemez, dokümante edilemez.

| # | Kural | Değer | İhlalde |
|---|---|---|---|
| 1 | Paket | DevExpress Universal **2026 / v26.1** | CRITICAL |
| 2 | Ana pencere | `RibbonForm` | RED |
| 3 | Ribbon stili | **`RibbonControlStyle.Office2010`** — tek izinli | CRITICAL |
| 4 | Form kalıbı | `.Form.cs` + `.Form.Designer.cs` + `.Form.resx` | RED |
| 5 | Tema | Dark/Light `LookAndFeel` geçişi destekli | HIGH |
| 6 | DPI | `AutoScaleMode.Dpi`, %100–%200 | HIGH |
| 7 | Yüksüz grid | 500+ satırda sanallaştırma açık | ORTA |

**Yasaklı ifadeler:**

```text
DevExpress.*.* 24.2* / 23.* / 25.*        -> surum sapmasi (G3)
RibbonControlStyle.Office2007             -> tek stil kurali ihlali
RibbonControlStyle.Office2010Blue         -> "Office2010" deyimi gecmiyor mu?
                                             AYRIKTIR: sadece .Office2010
XtraForm  (ana pencere olarak)            -> RibbonForm olmali
WinForms disi: WPF / Web UI / MAUI        -> yasak (G3, CLAUDE.md s4)
```

### §4.2 Ribbon Stili — Tek İzinli Değer

```csharp
// DOGRU  --  tek izinli deger
this.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
this.Ribbon = this.ribbonControl;     // RibbonForm

// YANLIS  --  herhangi bir baska stil RED
// this.ribbonControl.RibbonStyle = RibbonControlStyle.Office2007;
// this.ribbonControl.RibbonStyle = RibbonControlStyle.Office2010Blue;
```

> **UYARI:** `Office2010Blue`, `Office2010Black`, `Office2013` gibi varyantlar **ayrı** değerlerdir ve **geçmez**. Aranan dize tam olarak `RibbonControlStyle.Office2010` olmalıdır.

### §4.3 Form Üçlüsü Kalıbı

Her ekran **tam üç dosyadan** oluşur; eksik = RED.

```text
src/VersaCoder.Desktop/Forms/
  MainForm.cs            <- olaylar + is mantigi
  MainForm.Designer.cs   <- InitializeComponent() (elle yazilir, yeniden uretilmez)
  MainForm.resx          <- metin/ikon/kaynak (UTF-8)
```

| Dosya | İçerik | Sahip | Değişim kuralı |
|---|---|---|---|
| `*.Form.cs` | Olay handler'ları, `Application` çağrısı, validation | developer | Serbest (testli) |
| `*.Form.Designer.cs` | `InitializeComponent()`, kontrol alanları, yerleşim | ui-designer | Elle yazılır; runtime **değiştiremez** |
| `*.Form.resx` | Metin, ikon, görüntü, yerel string | ui-designer | UTF-8; hardcode metin yasak |

```csharp
// MainForm.Designer.cs — degismez duzen
private void InitializeComponent()
{
    this.components = new System.ComponentModel.Container();
    this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
    this.ribbonControl.RibbonStyle =
        DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
    // ... kontroller ...
    this.Name = "MainForm";
    this.Text = resources.MainForm_Title;   // .resx'ten, hardcode degil
    this.Ribbon = this.ribbonControl;
    this.ResumeLayout(false);
    this.PerformLayout();
}
```

### §4.4 UI Çalışma Sırası (gate'e bağlı)

```text
1. Wireframe (ASCII, 7 ekran)      -> .ai/ui-design/NN-*.md  (status: draft)
2. Kunye  (ekran + bacak + kontrol)
3. SUNUM  -> kullaniciya goster
4. GATE-003 ONAYI BEKLE           -> onaysiz devam YASAK
5. .Designer.cs + .resx uret
6. Form.cs (olaylar) -> developer'a devir
7. iki bacakta build + test
```

> **UYARI:** **GATE-003 (UI Approval)** alınmadan `.Designer.cs`/`.cs` üretimine geçilemez. Bu, `CLAUDE.md` G12 ve `.agents/ui-designer.md` §2 ile bağlayıcıdır.

### §4.5 Ekran → DevExpress Kontrol Eşlemesi (7 ekran)

| # | Ekran | Ana kontroller | Hedef bacak |
|---|---|---|---|
| 01 | `01-start-page` | `RibbonControl`, `XtraTabControl`, `TileControl`, `SearchControl` | net10 + net48 |
| 02 | `02-main-ide` | `RibbonForm` + `RibbonControl`(**Office2010**), `DockManager`, `StatusBarControl` | net10 + net48 |
| 03 | `03-solution-explorer` | `TreeList`, `DxCollapsedControl`, `ImageCollection` | net10 + net48 |
| 04 | `04-code-editor` | `MemoEdit`/`RichEditControl`, `XtraTabControl`, `GaugeControl` | net10 (net48: `MemoEdit`) |
| 05 | `05-ai-chat` | `MemoEdit`, `ListBoxControl`, `ProgressBarControl`, `BarEditItem` | net10 + net48 |
| 06 | `06-agent-panel` | `GridControl`, `ProgressPanel`, `BarButtonItem` | net10 + net48 |
| 07 | `07-settings` | `DataLayoutControl`, `PropertyGridControl`, `LookUpEdit` | net10 + net48 |

**R1 uyarısı:** Yedek vault'ta 16 wireframe (`WF-00..WF-15`) vardır; master prompt **7 ekran** ister. Bu set **farklıdır** — 7 ekranlık liste esastır, yedek yalnız bilgi kaynağıdır (`reports/00-kapsam-analizi.md` §4.1).

### §4.6 UI Yasakları

| # | Yasak | Yerine |
|---|---|---|
| 1 | Designer dosyasını VS/CLI ile yeniden üretmek | Elle yazımı koru, diff'e bak |
| 2 | Form içinde SQL/Dapper çağrısı | Handler çağır (`Application`) |
| 3 | Hardcode metin/ikon | `.resx` + `ResourceManager` |
| 4 | Runtime'da `InitializeComponent` içi değişiklik | `.Form.cs`'de yap |
| 5 | `#if` ile UI mantığı bölmek | Platform farkı dışında `#if` yok |
| 6 | net48'te yalnız `MemoEdit` yerine `RichEditControl` | §4.5 sütunu "hedef bacak" |
| 7 | Tek ekran = tek dosya | Üçlü (§4.3) zorunlu |

---

## §5 Çift Bacaklı Build Motoru

### §5.1 İki Bacak

> **KURAL (G6):** Her karar `net48` + `net10.0-windows` üzerinden değerlendirilir. **Ortak kaynak**; `#if` **yalnız** platform farklarında.

| Bacak | Amaç | TFM | GUI | Test |
|---|---|---|---|---|
| **Legacy** | Uyumluluk kanıtı, eski müşteri ortamı | `net48` | WinForms + DevExpress v26.1 | `dotnet test --framework net48` |
| **Modern** | Yeni geliştirme, görsel doğrulama | `net10.0-windows` | WinForms + DevExpress v26.1 | `dotnet test --framework net10.0` |

**Çift bacak kural seti:**

| # | Kural | İhlalde |
|---|---|---|
| 1 | **İki TFM, tek proje** (`<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>`) | HIGH |
| 2 | Kaynak **ortak**; `#if` yalnız API/platform farkı | HIGH |
| 3 | **Her iki bacak** build + test yeşil olmalı | G5 RED |
| 4 | Tek bacak yeşil = "bitti" **denemez** | G5 RED |
| 5 | `NET48` / `NET10_0_WINDOWS` sabitleri dışında **preprocessor yok** | ORTA |
| 6 | Platform kodu **çoğaltılmaz** — `#if` yalnız imzada/uzantıda | HIGH |

```csharp
// DOGRU  --  platform farki izole, is mantigi ortak
public static class PathCompat
{
    public static string AppDataDir()
    {
#if NET48
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#else
        return Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create);
#endif
    }
}

// YANLIS  --  is mantigi #if ile bolunmus
// #if NET48
//     return LegacyCreateSession(name);
// #else
//     return ModernCreateSession(name);
// #endif
```

### §5.2 R5 Riski (net48 + DevExpress v26.1)

**R5 = net48 bacağında DevExpress v26.1 uyumluluğu doğrulanmadı.** Kaynak: `reports/00-kapsam-analizi.md` §3.1.

| Aşama | Ne yapılır | Zaman |
|---|---|---|
| 1 | İlk `dotnet restore` + `dotnet build` ile **erken** doğrula | PART 5 başı (≈2 dk) |
| 2 | Uyumsuzluk çıkarsa: **architect'e RED** → ADR + kullanıcı onayı | Aynı oturum |
| 3 | Geçici çözüm: `net10` UI + `net48` kütüphane ayrımı **önerilmez** (G6 ihlali) | — |
| 4 | Seçenekler: DevExpress sürüm Uyumluluk Tablosu (yalnız v26.1), TFM şartı, hazır Lab raporu | Gerekirse ADR |

> **DERİN DÜŞÜNME:** R5, **tek başına build'i kilitleyen** bir belirsizliktir. Çözüm `#if` ile "uymayan kontrolü atlamak" **değildir**; bu, G6 ihlali ve sessiz davranış farkı yaratır. Doğru hareket: **erken kanıt → RED → ADR → kullanıcı onayı**.

### §5.3 Derleme Ortak Ayarları (CPM + Directory.Build.props)

| Ayar | Değer | Neden |
|---|---|---|
| Paket sürümü | `Directory.Packages.props` (CPM, `ManagePackageVersionsCentrally`) | Sürüm tek yerde; iki bacak drift'i imkânsız |
| `LangVersion` | `latest` sabit | Dil farkı doğurmaz |
| `Nullable` | `enable` | Boş referans hataları |
| `TreatWarningsAsErrors` | `true` | Uyarı = hata (developer.md §3 madde 9) |
| `ImplicitUsings` | `enable` (net10) / `#if` ile | net48 SDK farkı |
| `Deterministic` | `true` | Tekrarlanabilir build |
| `AutoGenerateBindingRedirects` | `true` (net48) | runtime binding |
| DevExpress paketi | tek adres, `v26.1` | G3 |

```xml
<!-- Directory.Build.props (iskelet) -->
<PropertyGroup>
  <LangVersion>latest</LangVersion>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <Deterministic>true</Deterministic>
</PropertyGroup>
```

```xml
<!-- Proje: TFM kumesi -->
<PropertyGroup>
  <TargetFrameworks>net48;net10.0-windows</TargetFrameworks>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```

### §5.4 Build Varyantları

| Varyant | Komut | Beklenen |
|---|---|---|
| Restore | `dotnet restore` | 0 hata |
| İki bacak build | `dotnet build -c Release` | **0 hata, 0 beklenmedik uyarı** |
| Tek bacak (legacy) | `dotnet build -f net48` | 0 hata |
| Tek bacak (modern) | `dotnet build -f net10.0-windows` | 0 hata |
| Temiz build | `dotnet clean && dotnet build` | 0 hata |
| Restore + build tek seferde | `dotnet build --no-incremental` | 0 hata |

---

## §6 Test Motoru

### §6.1 Test Piramidi ve Ayrım

| Katman | Oran | Kapsam | Araç | Klasör |
|---|---|---|---|---|
| **Birim** | ~%80 | Domain invariant, Application handler, Dapper map | xUnit + NSubstitute | `tests/VersaCoder.Domain.Tests/`, `...Application.Tests/` |
| **Entegrasyon** | ~%15 | Dapper + gerçek SQLite sorgusu, migration, dosya I/O | xUnit + temp DB | `tests/VersaCoder.Infrastructure.Tests/` |
| **Mimari** | ~%5 | Katman bağımlılığı, `#if` disiplini, EF yok | NetArchTest | `tests/VersaCoder.Architecture.Tests/` |
| **UI** | isteğe | Designer bütünlüğü + olay bağlama (net10) | manuel + smoke | `tests/VersaCoder.Desktop.Tests/` |

**Birim vs entegrasyon ayrım ölçütü:**

| Soru | Evet → |
|---|---|
| Gerçek dosya / gerçek SQLite / gerçek ağ kullanıyor mu? | **Entegrasyon** |
| Yalnız saf hesaplama + sahte bağımlılık (NSubstitute) mı? | **Birim** |
| Proje referansı yönü test ediliyor mu? | **Mimari** |
| `InitializeComponent` / kontrol hiyerarşisi mi? | **UI smoke** |

### §6.2 Coverage Hedefi

| Metrik | Hedef | Nerede zorunlu |
|---|---|---|
| **Satır coverage** | **≥ %70** | Tüm `src/` |
| **Kritik yol (Domain + Application)** | **≥ %85** | Handler + invariant |
| Branch coverage | raporlanır | `reports/*` |
| Yeni public davranış | **test yoksa RED** | `.agents/tester.md` §7 |
| `Skipped` test | raporda açıkça yazılır | Gizlemek yasak |

> **UYARI:** Coverage **tahmin edilmez**, üretilir. Çıktı olmadan "coverage sağlandı" denemez (G5).

### §6.3 Gate Eşlemesi (Kanıt → Gate)

| Kanıt | Hangi gate'e gider | Üreten |
|---|---|---|
| `dotnet build` iki bacak 0 hata | GATE-005 (Implementation Plan Approval) öncesi/sonrası | developer |
| `dotnet test` iki bacak yeşil | Kapanış (rapor 09) | tester |
| Coverage çıktısı (≥%70 / ≥%85) | Kapanış | tester |
| NetArchTest yeşil (§2.6) | GATE-004 (Architecture Approval) | architect + tester |
| EF referansı 0, sürüm yalnız v26.1, string-concat SQL 0 | Kapanış | reviewer |
| 0 bozuk link (vault) | Kapanış | herkes + script |
| 7 wireframe + kontrol envanteri | **GATE-003 (UI Approval)** — kullanıcı onayı zorunlu | ui-designer |

**GATE-003 durağı:** Onay alınmadan **PART 5/6 kodlaması başlamaz** (`.agents/ui-designer.md` §6).

### §6.4 Determinizm Kuralları

| # | Yasak | Yerine |
|---|---|---|
| 1 | `DateTime.Now/UtcNow` doğrudan | `TimeProvider` |
| 2 | `Thread.Sleep` | `Task.Delay` + await / sentetik saat |
| 3 | Gerçek kullanıcı körüğü | `IDisposable` temp dizin |
| 4 | Ağ / gerçek LLM | `ILLMProvider` sahte |
| 5 | Ortak mutable state | Test başına yeni fixture |
| 6 | Rastgelelik | Sabit seed |

### §6.5 Test Yürütme Kanıtı

```text
dotnet test --framework net48          ->  X passed, 0 failed
dotnet test --framework net10.0        ->  X passed, 0 failed
coverage                               ->  line %, branch %  (rapora yapistir)
```

- Her iki bacakta da **aynı test kümesi** çalışır.
- `#if` dalı olan her davranış için **iki bacak testi** vardır.
- Migration testi: temp DB'de `schema_version` ilerlemesini doğrular (§3.7).

---

## §7 Protocol / opencode Entegrasyon Motoru

### §7.1 Kaynak Envanteri (opencode v1.18.32)

> Kaynak: `reports/00-kapsam-analizi.md` §1.3 — `%TEMP%\opencode-fresh`, v1.18.32, **33 paket**.

| Paket | Dosya | Rol | Bizim için |
|---|---|---|---|
| `packages/opencode` | **836** | CLI uygulaması | Referans |
| `packages/core` | **491** | Ajan döngüsü, tool sistemi | **Ana referans** (PART 7 girdisi) |
| `packages/tui` | **241** | Terminal UI | Referans (biz WinForms) |
| `packages/sdk` | **48** | SDK | Referans |
| `packages/server` | **31** | Server | Referans |
| `packages/protocol` | **26** | Mesaj şeması | **PART 7 ana girdisi** |
| **Toplam** | **1.673** (6 ana paket) | — | 33 paket içinde |

Ek paketler: `codemode`, `plugin`, `llm`, `ui`, `desktop`.

### §7.2 Adopt vs Refer (Ne alınır, ne referans kalır)

| Konu | **Adopt** (sözleşmeye uyarlanır) | **Referans** (yalnız okunur, kopyalanmaz) |
|---|---|---|
| Mesaj şeması | Mesaj tipleri, sürümleri, alan adları (C# record) | Kod dosyaları, import zinciri |
| Tool sözleşmesi | `read`/`write`/`edit`/`bash`/`grep`/`glob` parametre ve sonuç şekli | Node/TS implementasyonu |
| Agent döngüsü | Mesaj sırası, tool çağrısı akışı, iptal semantiği | Runtime scheduling kodu |
| Streaming | Olay adları, parçalama kuralı, sonlandırma | Transport katmanı |
| Session | Kalıcılık modeli, id üretimi, yeniden bağlanma | Depolama şeması birebir |
| SDK | C# API yüzeyi isimlendirmesi | JS/TS SDK kodu |
| TUI | — | **Hiçbir şey** (biz WinForms) |
| Server | — | **Hiçbir şey** (biz kendi host'umuz) |
| Plugin/skill modeli | Yükleme sırası, keşif kuralı | Kayıt dosyaları |

> **KURAL (G1):** opencode kaynak kodu **`REFERENCE_VERIFIED`** statüsüyle okunur; içeriği **kopyalanmaz**. Uyarlama = **C# sözleşmesine yeniden yazım**.

### §7.3 Uyarlama Kuralları

| # | Kural | Detay |
|---|---|---|
| 1 | **Sözleşme önce** | Mesaj/tool tipi `Protocol` katmanında (`L5`) yazılır, saf kalmalı |
| 2 | Sürüm ekle | `protocolVersion` alanı; uyumsuz sürüm → açık hata |
| 3 | Tool girdisi **validate** | JSON schema benzeri kontrol; geçersiz = `Result.Fail` |
| 4 | Tool sonucu **normalize** | `ok | error | partial` üçlüsü tek tip |
| 5 | IPTAL | `CancellationToken` her çağrıda zincirlenir |
| 6 | Streaming | Parçalar idempotent; son parça `done` flag'i taşır |
| 7 | Transport bağımsız | `Protocol` yalnız şema; soket/stdio bilmez (§2.3 satır 5) |
| 8 | Test | Her tool için contract testi + sahte sunucu |

### §7.4 Sözleşme İskeleti (L5 Protocol)

```csharp
// L5 Protocol -- yalniz sema, hicbir bagimliligi yok (L1 dogruda)
public sealed record ProtocolMessage(
    string ProtocolVersion,     // "1.0"
    string Id,
    MessageKind Kind,           // request | response | event | error
    string? Tool,               // "read" | "write" | "edit" | "bash" | "grep" | "glob"
    JsonElement? Payload,
    long TimestampUtc);

public enum MessageKind { Request, Response, Event, Error }

public sealed record ToolResult(bool Ok, JsonElement? Data, ErrorCode? Error, bool Partial);
```

**Tool envanteri (opencode'dan uyarlanan çekirdek set):**

| Tool | Amaç | Bizdeki karşılık |
|---|---|---|
| `read` | Dosya oku (offset/limit) | `IFileSystem` + güvenli kök |
| `write` | Dosya oluştur/overwrite | Onay kuralı (G7) |
| `edit` | Kesin string değiştirme | unique match zorunlu |
| `bash` | Komut çalıştırma | Sandbox + allowlist |
| `grep` | İçerik tarama | ripgrep sarmalayıcı |
| `glob` | Dosya deseni | Desen sınırlı |

> **UYARI:** `bash` ve `write` **yıkıcı** olabilir → G7 (onay) ve sandbox kurallarıyla birlikte tanımlanır; aksi `DECISION BLOCKED`.

---

## §8 Build Komut Hile Kutusu (Cheat-Sheet)

```text
# 0) TEMEL  --  her zaman iki bacak
dotnet restore
dotnet build -c Release                       # iki TFM birden
dotnet test  --framework net48
dotnet test  --framework net10.0-windows

# 1) TEK BACAK IZOLASYONU
dotnet build -f net48
dotnet build -f net10.0-windows
dotnet build --no-incremental                 # temiz kanit

# 2) TEMIZLIK
dotnet clean
dotnet clean -c Release

# 3) CPM  --  surum tek yerden
dotnet list package                           # hangi paket hangi surum

# 4) TEST AYRINTISI
dotnet test --framework net48 --logger "console;verbosity=detailed"
dotnet test --filter "FullyQualifiedName~Infrastructure"
dotnet test --collect:"XPlat Code Coverage"

# 5) DENETIM  --  yasakli referans taramasi
#    EF Core referansi 0 mu? DevExpress disi surum var mi?
#    (grep tabanli; cikti 0 satir olmali)

# 6) VAULT DENETIMI
#    .ai/ altindaki tum wiki-link hedefleri diskte var mi? -> 0 bozuk link

# 7) UI  --  sadece GATE-003 sonrasi
#    .Designer.cs / .resx uretimi ve build
```

| Amaç | Komut | Beklenen çıktı |
|---|---|---|
| Hızlı doğrulama | `dotnet build` | 0 hata |
| Kanıt üretme | `dotnet build --no-incremental` | 0 hata (her iki TFM) |
| Test | `dotnet test --framework net48` | 0 failed |
| Test | `dotnet test --framework net10.0-windows` | 0 failed |
| Paket denetimi | `dotnet list package` | yalnız izinli paketler |
| Mimari denetim | NetArchTest testleri | yeşil |
| Vault link denetimi | link tarama scripti | 0 bozuk link |

> **UYARI:** `dotnet build` **tek bacakta** yeşilse kanıt **eksiktir** (G5). Her raporda iki çıktının da satır özeti bulunmalıdır.

---

## §9 Hata → Neden → Düzeltme

| # | Hata / belirti | Kök neden | Düzeltme | Seviye |
|---|---|---|---|---|
| 1 | `NU1102` DevExpress paketi bulunamadı | Sürüm `v26.1` değil / feed yok | `Directory.Packages.props` → v26.1, feed kontrol | CRITICAL |
| 2 | `NU1202` paket `net48`'e uyumsuz | Paket TFM desteklemiyor | Paket yerine platform uyumlu alternatif → **architect RED** | HIGH |
| 3 | `CS0246` `DbContext` bulunamadı | EF Core yasaklı ama referans kalmış | Referansı sil, ADO.NET + Dapper'a çevir (G2) | CRITICAL |
| 4 | `CS0234` `Microsoft.EntityFrameworkCore` | Aynı — yasaklı paket | Paket kaldır (G2) | CRITICAL |
| 5 | Circular dependency bildirimi | §2.3 ihlali | Bağımlılığı interface'e indir (L1/L3) | HIGH |
| 6 | `CS0103` `NET48` tanımsız | `#if` sabiti yanlış | `#if NET48` / `#if NET10_0_WINDOWS` | ORTA |
| 7 | net48 `async`/`await` API eksik | Eski BCL | Shim + `#if` (iş mantığı değil) | ORTA |
| 8 | DevExpress ribbon istenen stilde değil | `RibbonControlStyle` yanlış değer | `.Office2010` — tek izinli (G3) | CRITICAL |
| 9 | Designer dosyası bozuk / açılmıyor | `.resx` encoding | UTF-8 + `ResXResourceReader` uyumu | HIGH |
| 10 | `InitializeComponent` çakışması | Elle/CLI ile üretilmiş iki kopya | Tek kaynak `.Designer.cs`; diff temizle | HIGH |
| 11 | Dapper result boş | kolon ≠ property | `AS` takma adı / explicit map (§3.8) | ORTA |
| 12 | `Missing parameter` | `@ad` uyuşmazlığı | Parametre adını sorguyla eşleştir | HIGH |
| 13 | `Timeout expired` | İndeks yok / kilit | Önce indeks (§3.5), sonra timeout | HIGH |
| 14 | `database is locked` | SQLite eşzamanlı yazma | WAL + tek writer kuyruğu | ORTA |
| 15 | `No provider registered` | Host kaydı eksik | §3.3 Host DI kaydı | HIGH |
| 16 | Checksum uyuşmazlığı | Uygulanmış migration değişmiş | Yeni `V00N+1` (§3.7 kural 5) | HIGH |
| 17 | Migration kısmen uygulandı | Idempotent değil | `IF NOT EXISTS` + transaction (§3.7) | HIGH |
| 18 | Ham conn-string logda görünüyor | Maskelenmemiş loglama | `Data Source=***` maskesi (§3.6 kural 4) | CRITICAL |
| 19 | Test `net48`'de geçmiyor | `DateTime`/fs bağımlılığı | `TimeProvider` + temp dizin (§6.4) | HIGH |
| 20 | Testler flaky | `Thread.Sleep` / ortak state | Sentetik saat + fixture (§6.4) | HIGH |
| 21 | Coverage hedefi düşük | Test eksik | Yeni public davranışa test (§6.2) | HIGH |
| 22 | `Skipped` test gizlenmiş | Raporda yazılmamış | Sayıyı açıkça yaz | ORTA |
| 23 | Link taraması RED | Bozuk `wiki-link` | Hedefi düzelt veya `status: planlı` işaretle | ORTA |
| 24 | Frontmatter eksik | `authority: SSOT` yok | 8 alan tamamla (G4) | ORTA |
| 25 | Vault'da kopya blok | CoreMusic kopyası (G1) | Dosyayı sıfırdan yaz | CRITICAL |
| 26 | "Muhtemelen geçer" raporu | Kanıtsız tamamlama (G5) | build/test çıktısını yapıştır | RED |
| 27 | Tek bacak build yeşil | G6 ihlali | İkinci bacağı da çalıştır | HIGH |
| 28 | GATE-003 atlanmış | Onaysız kodlama (G12) | Partı başa al, onay al | RED |
| 29 | `RibbonForm` yerine `XtraForm` | G3 ihlali | Ana pencere `RibbonForm` | CRITICAL |
| 30 | UI'da doğrudan SQL | §2.4 madde 5 | Handler üzerinden `Application` çağır | HIGH |

---

## §10 Part ↔ Gate Orkestrasyon Matrisi

> Bu bölüm `engine.md`'nin orkestrasyon sorumluluğudur; gate **tanımı** süreç dosyasındadır (SRP).

| Part | Üretim | Motor (bu dosya) | Kanıt | Gate |
|---|---|---|---|---|
| 1 | Kaynak okuma | §13 referans hiyerarşisi | Okuma notu | GATE-001 |
| 2 | Kapsam analizi | `reports/00-kapsam-analizi.md` | R1–R6, B1–B4 | GATE-002 |
| 3 | Vault kurulumu | §12 checklist | 0 bozuk link | GATE-004 |
| 4 | UI + ASCII | §4 UI motoru | 7 wireframe + kontrol envanteri | **GATE-003** |
| 5 | Proje iskeleti | §2 katman + §5 build | `dotnet build` iki bacak | GATE-005 |
| 6 | Kodlama | §3 veri + §4 UI + §6 test | `dotnet test` iki bacak + coverage | — |
| 7 | opencode uyarlaması | §7 protocol | Contract testleri | — |
| 8 | Doğrulama | §8 + §9 + §12 | `reports/09-teslim-raporu.md` | Kapanış |

| Gate | Ad | Motor kanıtı |
|---|---|---|
| GATE-000 | Discovery Approval | Kapsam raporu |
| GATE-001 | Reference + Research Approval | Okuma notu |
| GATE-002 | Requirements Freeze | Kapsam analizi DoD |
| GATE-003 | **UI Approval** | 7 wireframe + kontrol envanteri + net48 uyumluluk notu |
| GATE-004 | Architecture Approval | NetArchTest yeşil + vault 0 bozuk link |
| GATE-005 | Implementation Plan Approval | İlk `dotnet build` iki bacak (R5 erken sinyali) |

---

## §11 Motor Kuralları

> **KURAL:** Build sırası **tek yönlüdür**: `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`. Tersine referans = mimari RED (§2.4).

> **KURAL:** Veri erişimi **yalnız** ADO.NET + Dapper; **her** sorgu parameterized; **her** komutda timeout; connection string **şifreli** (§3.4–§3.6).

> **KURAL:** Migration = **saf SQL dosyası + `schema_version` + sıralı + idempotent**. EF migration yok (§3.7).

> **KURAL:** UI = **WinForms + DevExpress v26.1 + `RibbonControlStyle.Office2010` + form üçlüsü**. Tek stil, tek sürüm (§4).

> **KURAL:** Her değişiklik **iki bacakta** build + test yeşildir; tek bacak kanıt değildir (§5, G5/G6).

> **UYARI:** **GATE-003** onayı olmadan UI kodlamasına geçilemez (§4.4, G12).

> **UYARI:** opencode **referanstır**, kopya kaynağı değil (§7.2, G1).

> **UYARI:** EF Core, DevExpress v26.1 dışı sürüm, string-concat SQL — üçü de **sıfır** olmalı; kapanış raporunda sayım verilir.

> **DERİN DÜŞÜNME:** Belirsiz bir katman kararı, sağlayıcı seçimi, R5 gibi bir uyumluluk riski veya **3. başarısız düzeltme** burada cevaplanmaz — `ULTRA-THINKING.md` protokolü devreye girer, sonra ADR açılır.

---

## §12 Doğrulama

- [ ] §2.1'deki 7 katman sırası kodda/proje yapısında birebir var
- [ ] §2.3 matrisi NetArchTest ile kanıtlanmış (≥6 test)
- [ ] §2.4'teki 8 yasak bağımlılıktan **hiçbiri** yok
- [ ] EF Core referansı **0**; `DbContext` geçen kod **0**
- [ ] DevExpress sürümü **yalnız v26.1**; ribbon **yalnız `Office2010`**
- [ ] Her sorgu parameterized; `$"SELECT ... {` deseni **0**
- [ ] Her komutta `CommandTimeout` (global 30 sn); `0` (sınırsız) **0**
- [ ] Connection string repo'da **şablon**; ham string **0**
- [ ] `V0xx__*.sql` + `schema_version` + idempotent test yeşil
- [ ] Form üçlüsü her eksanda eksiksiz (`.Form.cs` + `.Designer.cs` + `.resx`)
- [ ] `dotnet build` → iki bacak **0 hata** (çıktı yapıştırıldı)
- [ ] `dotnet test` → iki bacak **0 failed** (çıktı yapıştırıldı)
- [ ] Satır coverage ≥ **%70**, kritik yol ≥ **%85**
- [ ] opencode: adopt/refer tablosu (§7.2) uygulanmış, kopya blok **0**
- [ ] Vault link taraması → **0 bozuk link**
- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] `keys.md` §3'teki zorlu anahtarların çoğu bu dosyada geçiyor (katman, ADO.NET, Dapper, parameterized, migration, Ribbon, v26.1, net48, opencode, GATE)

---

## §13 Referanslar

| # | Kaynak | Yol / etiket |
|---|---|---|
| 1 | Navigasyon | `.ai/index.md` |
| 2 | Anahtar kelime haritası | `.ai/keys.md` |
| 3 | Guardrail G1..G12 | `.ai/CLAUDE.md` |
| 4 | Kapsam analizi (R1–R6, B1–B4, gate envanteri, opencode envanteri) | `.ai/reports/00-kapsam-analizi.md` |
| 5 | Ajan kaydı + yetki matrisi | `.ai/.agents/AGENTS.md` |
| 6 | Katman sözleşmesi | `.ai/.agents/architect.md` §4 |
| 7 | Form üçlüsü + kod kuralları | `.ai/.agents/developer.md` §3–§4 |
| 8 | Test piramidi + coverage hedefi | `.ai/.agents/tester.md` §3, §7 |
| 9 | 7 ekran + GATE-003 paketi | `.ai/.agents/ui-designer.md` §3, §6 |
| 10 | Kök MD şablonu (iskelet + KB bandı) | `.ai/.templates/documentation/root-md-template.md` |
| 11 | Derin düşünme protokolü | `.ai/ULTRA-THINKING.md` |
| 12 | Bootstrap (karar kaynağı) | `G:\…\versacoder proje planaması ai.03.md` — `SOURCE_VERIFIED` |
| 13 | opencode v1.18.32 kaynak | `%TEMP%\opencode-fresh` — `REFERENCE_VERIFIED` |

**Doğrulama etiketleri (bu dosya için):** katman sırası + gate envanteri → `SOURCE_VERIFIED` (`CLAUDE.md` §4, `reports/00-kapsam-analizi.md`) · opencode paket sayıları → `SOURCE_VERIFIED` (`reports/00-kapsam-analizi.md` §1.3) · coverage hedefi → `SOURCE_VERIFIED` (`.agents/tester.md` §7) · sağlayıcı arayüzü/iskeletler → `INFERRED` (tasarım önerisi, PART 5'te ADR ile kesinleşecek).
