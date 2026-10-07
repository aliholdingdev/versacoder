---
title: Repository Template — Arayüz Sözleşmesi ve Orchestratör Kalıbı
type: template
category: template
status: active
version: 1.0.0
updated: 2026-10-01
authority: SSOT
scope: "repository sablonu: arayuz, implementasyon ve orchestrator kalibinin iskeleti"
---

# Repository Template — Arayüz Sözleşmesi ve Orchestratör Kalıbı

**Zorunlu Bağlantılar:**

- [ascii-diagram-template.md](../architecture/ascii-diagram-template.md) — kutu-diyagram ve §5.3/§5.4/§7.2 kalıpları (buradaki kutu dahil)
- [dapper-mapping-template.md](../infrastructure/dapper-mapping-template.md) — Dapper iç eşleme SSUT'u (sorgu çalıştırma/donanım tarafı)
- [service-template.md](service-template.md) — iş akışı sahipliği; repository'yi çağıran katman
- [handler-template.md](handler-template.md) — ADO.NET handler'ları ile `IReadRepository` karşılaştırması
- [../../.rules/coding-standards.md](../../.rules/coding-standards.md) — `async`/`await`, naming, kod stili kuralları
- [../../architecture/index.md](../../architecture/index.md) — katman diyagramı ve bağımlılık okları
- [../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md](../../.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) — EF Core yasağı, ADO.NET + Dapper standardı (SSUT)

## §1 Amaç

### §1.1 Bu şablon ne üretir?

Bir **repository arayüzü + somut orchestratör implementasyonu**: service katmanının çağırdığı, sorgu yürütme işini
Dapper'a devreden, connection/transaction yaşam döngüsünü yöneten tek veri erişim kapısı. Çıktı bir **`.cs` çiftidir**
(`I{Entity}Repository.cs` + `{Entity}Repository.cs`); sorgu metni ve donanım ayrıntısı **kapsam dışıdır**
(SSUT: `dapper-mapping-template`). Orchestratör **sadece koordine** eder; mapping'i **bilmez** (§3.6).

### §1.2 Disk kanıtları ve Boşluk (YAGNI)

Aşağıdaki iddialar disk üzerinde **doğrulanmadı**; kaynak bağlanana kadar **spekülasyon** sayılır.

```markdown
<!-- ⚠️ VERIFICATION REQUIRED: aşağıdaki her satır disk kanıtı ile değiştirilecek. -->
| İddia (SPEKÜLASYON) | Kanıt yolu | Durum |
|---|---|---|
| `XRepository : I{Entity}Repository, IDisposable` deseni | {{KANIT_1_YOL}} | ⚠️ VERIFICATION REQUIRED |
| Service katmanı repository'yi ctor ile enjekte ediyor | {{KANIT_2_YOL}} | ⚠️ VERIFICATION REQUIRED |
| Optimistic concurrency `xmin`/`rowversion` ile | {{KANIT_3_YOL}} | ⚠️ VERIFICATION REQUIRED |
```

Doğrulanmış gerçekler: bu depo **çok katmanlı** (`Domain|Abstractions|Application|Infrastructure|CrossCutting|Protocol|Host|Desktop|Cli`);
**EF Core yok** (ADR-004; **bir tek Uzak Sunucu** NPoco kullanır — o bileşen bu şablonun **dışındadır**); DB şablonda
**sabitlenmez**; ürün adı **VERSA CODER / VersaCoder**.

**YORUM BANU:** yorum satırı (`//`, `///`, `/* */`) **yasaktır** — taklitçi comment'i koda kopyalar. Dokümantasyon
`§` başlıkları ve **kod bloğu dışındaki** `<!-- ... -->` kutularında yaşar.

### §1.3 Terminoloji

| Terim | Anlamı | Terim | Anlamı |
|---|---|---|---|
| Repository | Tek veri erişim kapısı; `IDbConnection` sahibi | SSUT | Tek doğruluk kaynağı |
| Orchestratör | Sadece koordine eden implementasyon | Locator | `AdoConnectionFactory`'dan connection |
| Dapper | micro-ORM; mapping bu şablonun **dışında** | Paging | DB-bağımsız sayfalama (§3.7) |
| Unit of Work | Tek transaction altında çok repo (§3.7) | Contract | C# 12 **primary constructor** |
| Optimistic concurrency | Çakışma ancak commit'te verir | **Teknik SSUT** | ADR-004 (EF yasağı), sorgu içermez |
| N+1 | Döngü içinde tekrarlı sorgu | Fark | DB'ye özel SQL sözdizimi (§3.7) |
| DTO | Servise dönen veri yüzü | Scope | `using` bloğunun yaşam alanı |

### §1.4 Karşılık gelen (veya karşılığı olmayan) şablonlar

| Aşama | Şablon | Sebep |
|---|---|---|
| Diyagram | `ascii-diagram-template` | **Tek** diyagram standardı: 66 karakter ASCII kutu |
| Sequence | `ascii-diagram-template` §4.1 | Tekilleştirme |
| Vokabüler | `code-standards.md` §6 | İsimlendirme SSUT'u |
| Mimari | `architecture/index.md` §7 + `index.md` §1.3 | Katmanlar/bağımlılıklar |
| **Veri erişim donanımı** | **`dapper-mapping-template`** | **Mapping + sorgu SSUT'u; bu dosya arayüz + orchestratör** |
| Flow | `ascii-diagram-template` §4.2 | Depo **tek** flow standardı |
| Dapper nasıl yazılır | `dapper-mapping-template` §3.6 | `SqlMapper` ayarları orada |
| ORM seçimi | `.decisions/accepted/ADR-005` | EF Core yasağı orada (SSUT) |
| `service-template` | `backend/service-template.md` | Servis, repository'yi **çağırır** |
| `handler-template` | `backend/handler-template.md` | Query'yi **tek seferde** yürüten varyant |
| Stil | `.rules/coding-standards.md` | `async`, expression body, `nameof` |

**KAPSAM SINIRI:** Mapping, kolon eşleme, donanım → `dapper-mapping-template`.
Bu dosya: **arayüz + yaşam döngüsü + sayfalama/N+1 + transaction sahibi + hata modları + test stratejisi.**

## §2 Kapsam

### §2.1 Bu şablonu kullan

| Durum | ✅ Kullan |
|---|---|
| 1 | Yeni aggregate kökü/alt-sahip için `I{Entity}Repository` + somut sınıf |
| 2 | Service, `IReadRepository` üzerinden **yazma** → sözleşme ihlali (P1) |
| 3 | Sorgu davranışı Dapper'a ait → **`dapper-mapping-template`** |
| 4 | Basit tek-seferde sorgu → **`handler-template`** (aynı DB, farklı imza) |
| 5 | Gerçek DB üzerinde veri doğrulama (migration yok → kopya) |
| 6 | Yeni method imzası (paged/exists) → §3.4 zorunlu tablo |
| 7 | Test stratejisi kurulacak → §3.8 (sahte + gerçek DB) |

**YAPAR — arayüz + koordinasyon:** tanımlar · locator'ı çağırır · transaction kararını verir (`await using`) ·
servislere **DTO/enum** döner · sözleşmeyi **primary constructor** ile imzalar · testte **sahte repo** +
**gerçek DB kopyası** · **N+1 yok** ve **paging doğru** denetler.

**YAPMAZ:** **EF Core/EF6/IQueryable/DbContext** (ADR-004) · sorgu metni/eşleme (**dapper-mapping**'e devredilir) ·
raw SQL birleştirmez · connection string okumaz · config/secret gömmez · **handler şablonunu kopyalamaz** ·
`IDbConnection`'ı birden fazla yerde açmaz · servis davranışı eklemez · DB/ORM seçimini sabitlemez ·
kod içine yorum eklemez · **CoreMusic** **kullanılmaz** (ürün = **VERSA CODER / VersaCoder**).

### §2.2 Bu şablonu KULLANMA — alternatifler

| Bu dosya değil | Nerede | Ayırt edici işaret |
|---|---|---|
| Mapping/`SqlMapper`/ad-hoc sorgu | `dapper-mapping-template` | Kolon↔özellik, `QueryAsync<T>` seçimi |
| Sorguyu tek adımda yürütmek | `handler-template` | CQRS/sıcak yol; imza **task/dönen tip** |
| İş akışı/dönüşüm servisi | `service-template` | İş kuralı + `IRepository` çağrısı |
| Diyagram/flow/sequence | `ascii-diagram-template` | 66 karakter kutu standardı |
| ORM/teknoloji kararı | `.decisions/accepted/ADR-005` | EF yasağı SSUT |
| Migration/shema | DB migration dosyaları | Migration yok → gerçek DB kopyası |
| Cross-cutting (log/validation) | `crosscutting/` | Transaction **kapsamı** burada |

## §3 Mimari

### §3.1 Sorumluluk ve konum

Repository, **altyapı ayrıntısını** (connection, transaction, Dapper çağrısı) **gizleyen** ve application katmanına
**saf sözleşme** sunan **tek** kapıdır. Somut sınıf **`IReadRepository` miras almaz** (dual interface, §3.6);
**tek koordinasyon** yapar: locator'dan connection alır, **tek** `using` bloğu ile yaşar, Dapper'ı çağırır
(sorgu metni gelmez), dönüşü **DTO/enum'a** çevirir ve `IPagedResult` üretir.

### §3.2 Arayüz sözleşmesi (`I{Entity}Repository`) — primary constructor

```csharp
public interface IProyectoRepository :
    IReadRepository<Proyecto, int>, IRepository<Proyecto, int>
{
    Task<ProyectoResumenDto?> GetResumenAsync(int id, CancellationToken ct);
    Task<PagedResult<ProyectoListadoDto>> GetPagedAsync(ProyectoFiltroDto f, CancellationToken ct);
    Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken ct);
}
public interface ICargoRepository : IReadRepository<Cargo, int>, IRepository<Cargo, int> { }
// Esnek/imzalı: IVistaRepository : IReadOnlyRepository<Vista, long>  (yalnız okuma)
// Not: EfCoreProjeRepository gibi EF implementations YASAK (ADR-004, §4.3)
```

**İpucu:** İmzayı **primary constructor** ile aç (`IProyectoRepository(...)`); çağrı **tek noktadan** sözleşmeye akar.

### §3.3 Hedef mimari örneği — Katmanlar (T4) — AÇIK ŞABLON/DOLU ÖRNEK

```text
+----------------------------------------------------------------+
| ► SUNUM   Host / Desktop / Cli / Protocol                      |
| (Dto kullanır · sorgu metni yok · sadece Application)          |
+----------------------------------------------------------------+
| ► İŞ     Application — Service / Handler / UseCase             |
| (service-template.md · ICommand/IQuery · İŞ KURALI SSUT'u)     |
+----------------------------------------------------------------+
| ► SÖZLEŞME  Abstractions — IRepository / IUnitOfWork           |
| (repository-template.md §3.2 · Primary Constructor)            |
+----------------------------------------------------------------+
| ► ALTYAPI  Infrastructure — RepoImpl + Dapper + Locator        |
| (dapper-mapping-template.md · ADR-004 · EF Core YASAK)         |
+----------------------------------------------------------------+
| ► VERİ     Domain/Entities + DB (SQLite|SQL Server)            |
| (Entity ↔ Tablo · Optimistic Concurrency · DB = SSUT)          |
+----------------------------------------------------------------+
```

**Şablon:** Kutu primitifleri `ascii-diagram-template` §3.2'den **kopyalanır**; **tek HD** = bu dosya.
**Format seçimi:** bu şablonda **yalnızca** katman zinciri (T4) kutu çizilir; **her** uzun çift-tırnak satırı kutu
sayılmaz; öbür tablolar markdown tablosudur. **Sabit çizgiler:** 66 karakter (64 iç + `|` `|`).

**Dikkat sözcükleri (§3.3 dolusu):** **ZORUNLU · YASAK · P1 · SSUT · tek başına · asla · yalnızca · kritik ·
dikkat · engel · standart · yasa dışı.** Diyagram oku → **↔** (U+2194, 3 bayt) tek uzunluk çifti kabul edilir.

### §3.4 Zorunlu arayüz tablosu (`I{Entity}Repository` + `IReadRepository`)

| # | Method | İmza (async + `CancellationToken`) | Dönüş | Nerede |
|---|---|---|---|---|
| 1 | `Add` | `Task<int> AddAsync(T e, CancellationToken ct)` | yeni id | `IRepository` |
| 2 | `GetById` | `Task<T?> GetByIdAsync(int id, CancellationToken ct)` | entity/DTO | `IReadRepository` |
| 3 | `GetList` | `Task<IReadOnlyList<T>> GetListAsync(CancellationToken ct)` | liste | `IReadRepository` |
| 4 | `Delete` | `Task<bool> DeleteAsync(int id, CancellationToken ct)` | başarılı? | `IRepository` |
| 5 | `Exists` | `Task<bool> ExistsAsync(int id, CancellationToken ct)` | var mı | `IReadRepository` |
| 6 | `GetPaged` | `Task<PagedResult<T>> GetPagedAsync(int page, int size, CancellationToken ct)` | sayfa | `IReadRepository` |

- **`async void` DUR** (G9); **giriş `ct`'yi servisten** geçir.
- **DB'ye göre:** `string` pk → `long` pk; **SQLite** → `LIMIT/OFFSET`; **SQL Server** → `OFFSET/FETCH`.
- **Yinelenen satır/Dapper mapping** → `dapper-mapping-template` (mapping bu dosyanın **dışında**, §1.1).

### §3.5 Sözleşmeyi karşılama ve dönüş tipleri

| Tip | Kural |
|---|---|
| Command (`Add`/`Delete`) | `int`/`bool`; dönüş **DTO değil** |
| Entity geri dönüşü | Mümkünse **DTO** (`{Entity}ResumenDto`) — kırılganlık yok |
| Enum / sabit | `GetEstadoAsync` → `Task<EstadoTarea>` |
| Paging | **`IPagedResult<T>`** (`PageNumber, PageSize, TotalCount, Items`) — DB-bağımsız |
| Lista | `Task<IReadOnlyList<T>>` — **asla** `List<T>` sözleşmesi |
| Olmayan kayıt | `null` **DEĞİL** → serviste tip-güvenli Domain Failure/`Result` |
| Filtre | DTO içinde (`{Entity}FiltroDto`) — sorgu metni değil |
| Sıralama | Sabit `ORDER BY` (sayfalama için **zorunlu**) |

**Bu dosya NEDEN DTO/enum döner:** Servis, **dönüşüm + iş kuralını tek yerde** toplar; entity iç yapısı
**(tek başına)** değişince servis **kırılmaz** — `service-template` §1.4/§3.1 sınırının **veri yüzü**dür.

### §3.6 Implementasyon iskeleti (`{Entity}Repository : I{Entity}Repository`)

```csharp
public sealed class ProyectoRepository : IProyectoRepository
{
    private readonly AdoConnectionFactory _factory;
    private readonly IProyectoMapper _mapper;      // Dapper'a devredilen kısım (SSUT: dapper-mapping)
    public ProyectoRepository(AdoConnectionFactory factory, IProyectoMapper mapper)
        => (_factory, _mapper) = (factory, mapper);
    // Toplam 10 method: AddOrUpdate/GetById/GetList/GetPaged/GetResumen/Exists/...
    // using (DbConnection) + await conn.OpenAsync(ct) + await conn.QuerySingleAsync<T>(...)
    // Dönüş: DTO/enum'a çevrilir; sayfalama: PagedResult<T>.Create(...)
}
// Kritik: sınıf SADECE koordine eder; mapping IProyectoMapper'a aittir (P1: handler'a kopyalama, §4.2).
```

- **`IReadRepository`'yi uygulayan somut sınıf yasak** (P1): repo **yazabilir** = dual interface (§3.6).
- **`EfCoreXRepository : IXRepository` yasak** (ADR-004; doğrula: mirror `*Repository.cs` içine `EfCore` yaz).
- **`IProyectoRepository`'yi implemente eden başka sınıf yok** (tek .cs).

### §3.7 Unit of Work ve transaction — **NAZIK PROTOKOL**

| Araç | Neden |
|---|---|
| `AdoConnectionFactory.CreateConnection()` | **Tek** gerçek; locator **asla** transaction **tutmaz** |
| **`TransactionScope`** (async flow, **tek bağlantı**) | Çok repo **tek** atomik işlem — **default** |
| **`conn.BeginTransaction()`** (**tek repo**) | Repo kendi transaction'ını yönetir; UnitOfWork **gerekmez** |
| **Unit of Work** (`CommitAsync/RollbackAsync`) | Çok repo **aynı** transaction'ı paylaşır — **yalnız** bunda |
| **`await using` + `using (conn)`** | Async disposal (**tek** `IDbConnection` iki kez yarar) |
| ⚠️ Sadece bağlantı — | Çıkışta **kapalı**; transaction'ı **paket dışına** taşıma |

**NAZIK PROTOKOL — üç kural:** (1) Locator'dan al, **tek** `using` + **tek** `Open`; (2) transaction **varsa**
sahip ol, **yoksa** yine de **Commit/Rollback** çağır; (3) UnitOfWork bağlantıyı **paylaşır** (G13). **Kritik:**
`TransactionScope` **tek** bağlantıda; `BeginTransaction()` **tek repo** içindir. **N+1:** tüm okumalar **tek**
sorguya sıkıştırılır; **ORDER BY** sabitlenir (§4.4-7).

### §3.8 Test stratejisi (bu dosyanın sözleşmesini korur)

1. **Sahte repo birim testi** — `SaveAsync` çağrısı, dönüş tipi, **sözleşme ihlali** (yalnız okuma/EF);
2. **Gerçek DB entegrasyon testi** — `versacoder.db` **kopyasına** karşı (migration yok!): CRUD, N+1, paging;
3. **Mapping sözleşmesi** — mapping davranışı `dapper-mapping`'e ait (boundary testi **orada**);
4. **`exists-mismatch` taraması** — §5.3 komutu; arayüz↔impl eşleşmezse **P0**;
5. **`async void` + swallow** kod içi tarama — §4.5 G9/G10.
6. **Örnek — sahte repo:**

```csharp
[TestMethod] public async Task GetPagedAsync_ilk_sayfa_dogru_sayida_kayit_doner()
{
    var repo = new FakeProyectoRepository(); repo.Seed(10);
    var sayfa = await repo.GetPagedAsync(1, 5, CancellationToken.None);
    Assert.AreEqual(5, sayfa.Items.Count, "İlk sayfa 5 kayıt");    // puan 1/2
    Assert.AreEqual(10, sayfa.TotalCount, "Toplam 10 kayıt");      // puan 2/2
}
```

7. **Örnek — gerçek DB** (oturum başında **kopyala**, sonunda **geri al**; **mutation yok**):

```csharp
[TestMethod] public async Task ExistsByCodigoAsync_kayit_varsa_true_doner()
{
    using var conn = new SqliteConnection("Data Source=C:\\VersaCoderTest\\versacoder.db");
    await conn.OpenAsync();   // P1: satır ekleme — mevcut kayıtta doğrula
    Assert.IsTrue(await conn.ExecuteScalarAsync<bool>(
        "SELECT EXISTS(SELECT 1 FROM proyectos WHERE id = @p1)", new { p1 = 1 }),
        "Var olan kayıt true");   // puan 1/1 — nokta atışı
}
```

8. **Bütçe:** satır kapsama **≥ %90**; **kapsamsız** branch → **P2**; **mapping** kapsamı `dapper-mapping`'tir.

### §3.9 Bilinçli tasarım kararları

| Karar | Alternatif (REDDEDİLEN) | Gerekçe |
|---|---|---|
| **Tek** locator + **tek** `using` | Her yerde `new SqlConnection` | Tek yaşam döngüsü → sızıntı yok |
| **Dual** interface | Somut sınıf `IReadRepository` da uygular | Sözleşme ihlali → P1 (G1) |
| **PagedResult<T>** | `List<T>` + ayrı `count` | DB bağımsız; **tek** sorgu |
| **DTO/enum** dönüş | Entity kaçışı | Kırılganlık yok (service-template §1.4) |
| **T4 kutu** | mermaid/yıldız/emoji | **Tek** diyagram standardı (ascii §1) |
| **EF yasak** | EF Core repository | ADR-004: ADO.NET + Dapper |
| **`exists-mismatch` P0** | uyumsuzluğu sessiz geç | Arayüz↔impl ayrışırsa çalışma anında kırılır |

## §4 Kurallar

### §4.1 Bu depoda

1. **`async` + `await` + `Task`** — senkron method **ekleme** (P1).
2. **Her async method `CancellationToken ct` alır** — servisten geçir.
3. **`using (conn)` tek `Open`** — bağlantıyı **asla** unutma; scope dışına **çıkarma**.
4. **Dönüş DTO/enum**; liste `IReadOnlyList<T>`; sayfa **`IPagedResult<T>`**.
5. **`interface↔impl` eşleşir** (G14) — **P0** uyumsuzluk.
6. **`IReadRepository` = yalnız okuma**; somut sınıf **yazma** yapar (dual).
7. **Primary constructor** ile imza; **tek** noktadan sözleşme akar.
8. **Paged sorguda `ORDER BY` zorunlu** — deterministik sayfa.
9. **Kapsam dışı** iş her zaman §2.2'ye yönlendirilir (mapping/handler/flow).

### §4.2 Kompozisyon

1. Primary constructor **kullan** — imza **tek** noktadan akar.
2. **`EfCore*Repository` yasak** (ADR-004): doğrula → **mirror'da `EfCore` yok**.
3. Raw SQL string **birleştirme/interpolasyon YASAK** — **`@p1..@pN`** (G4).
4. Handler şablonunu **kopyalama** (G11); **`IReadRepository` + sorgu = mapping** (P1, §1.1).
5. UI **↔** DB/ORM seçimi sabitlenmez; **config/secret**'ı sınıfa **gömme**.
6. **Yorum ekleme** (G14 §1.2).
7. Mapping **`I{Entity}Mapper`**'a devredilir — repository **mapping bilmez** (SSUT sınırı).
8. Locator **tek** kayıt — **ikinci** locator **P0** (G10).
9. Dönüş DTO'su **daima** kısa ad taşır; tablo adı **geçmez** (§3.5).

### §4.3 Yasaklar (RED)

1. **EF Core / EF6 / `IQueryable` / `DbContext`/`SaveChangesAsync`** — **RED, ADR-004**.
2. **Raw SQL string interpolation/concat** — **RED** (P1, §4.4: `@p1`).
3. **`IDbConnection`'ı iki yerde aç** veya **unut** — **RED**.
4. **`async void`** — **RED** (G9); **exception swallow** (G10) — **RED**.
5. **`List<T>` imzası** veya **`Task<List<T>>`** — **RED**.
6. **Servis davranışı** repository'ye koyma — **RED** (katman ihlali).
7. **Kod içi yorum** (`//`, `///`) — **RED** (§1.2 YORUM BANU).

### §4.4 Guardrail serisi (repository için genişletilmiş)

1. `SELECT` **listesini** yapıştır — kolon↔özellik **mapping** (P1; ✅ `dapper-mapping` §3.5'e **devret**).
2. **N+1** (`foreach` içinde sorgu) — **`JOIN`/`UNION ALL` + tek sorgu** (§3.7).
3. `LIMIT/OFFSET` vs `OFFSET/FETCH` — DB **doğru** olmalı (§3.7).
4. **`async void`** yok (G9); **swallow** yok (G10) — hata **yükseltilir**.
5. **Paged** → `IPagedResult<T>` (DB'de `COUNT` + limit); tekrarlı sorgu → **önbellek** (`caching-strategies`).
6. **Ham SQL** yerine **parametre** (`@p1..@pN`); interpolation **P0** (G4).
7. **Paged** sorguda sabit `ORDER BY` — kararsız sayfa **P1**.

### §4.5 Guardrail kontrol listesi (§4.3'ün özet tekrarı)

| ID | Guardrail | Seviye |
|---|---|---|
| G1 | Somut sınıf **`IReadRepository` uygulayamaz** (dual) | P1 |
| G2 | **`EfCore*Repository`** — mirror'da `EfCore` aranır | P0 |
| G3 | **`IReadRepository` + sorgu** = handler kaçışı | P1 |
| G4 | SQL string **interpole/concat** → `@p1..@pN` | P0 |
| G5 | **N+1** (`foreach`+sorgu) → `JOIN`/`UNION ALL` | P1 |
| G6 | **DB paging farkı** (SQLite vs SQL Server) | P1 |
| G7 | **`new`** yerine **tek** `using` (locator) | P1 |
| G8 | **async hata sessizce** geç → fırlat; log'a yaz | P1 |
| G9 | **`async void`** — **DUR**; **`Task`** döndür | P0 |
| G10 | **`catch` → swallow** — **DUR**; boş `catch { }` **P0** | P0 |
| G11 | **handler şablonu kopyası** (imza/tek sorgu) | P1 |
| G12 | **`open`/`close`** — **tek** `using` + `OpenAsync` | P1 |
| G13 | **Locator'a `Dispose` yok**; **UnitOfWork** scope'lar | P0 |
| G14 | **interface↔impl uyuşmazlığı P0**; **kod içi yorum** **DUR** | P0 |
| G15 | **EF/`IQueryable` kodu** — ADR-004 · mirror | P0 |
| G16 | **N+1 sorgu** — **tek** `JOIN`; raporla **oransal** | P1 |
| G17 | **Mapping/iş/DI** yorumla **belgelenmez** — §5.4 | P1 |
| G18 | **`versacoder.db` + `sqlite`** mirror'da **p2** | P1 |
| G19 | **Schema/DB adı** (ne `db_`) — yasa dışı (P1) | P1 |
| G20 | **Kod içi yorum** — §1.2 **banu**; render **bozulur** | P2 |
| G21 | **Kutu 66 karakter** — §3.3; taşan satır **P2** | P2 |
| G22 | **`IPagedResult` yerine** `List<T>` sayfalama | P1 |
| G23 | **DTO yerine entity** dönüş — kırılganlık | P1 |
| G24 | **Config/secret** sınıfa gömme — **DI** ile | P0 |
| G25 | **CoreMusic** markası — ürün **VERSA CODER** | P1 |

> Tekrar yerine: §4.3 = RED gerekçeleri; §4.4 = **repo örn.**; bu tablo = **kod içi ID** (G1..G25, §5.4'te çağrılır).

## §5 Workflow

### §5.1 Adımlar (repo için)

1. **Kapsamı ayıkla** (§2.1/§2.2): **sorgu içermez** → mapping `dapper-mapping`.
2. **Arayüzü primary constructor ile yaz** (§3.2) — `IRepository` + `IReadRepository`.
3. **Signatürü doğrula** — **async + `ct`**; dönüş DTO/enum/`IPagedResult`.
4. **Implemente et** (§3.6): **tek** locator + **tek** `using` + `Open/CloseAsync`.
5. **Test et** — sahte repo (≥ %90) + **gerçek DB kopyası** (§3.8).
6. **§5.3 okuma** çalıştır → **§6.1**'den raporla (§5.4).
7. **Transaction kararını yaz** (§3.7): UnitOfWork mu, scope mu — **tek satır gerekçe**.
8. **`ORDER BY`/paging** sabitle (§4.4-7); sayfa deterministik olmalı.

### §5.2 Hata modları (≥8 — hepsi fiili)

| # | Hata | Sonuç | Önleme |
|---|---|---|---|
| E1 | `Open` yok / iki yerde `Open` | `InvalidOperationException` / sızıntı | **tek** `using` + tek `Open` |
| E2 | `SELECT *` + mapping uyuşmazlığı | `null`/yanlış kolon | kolon listesi + mapping (§4.4) |
| E3 | **`LIMIT/OFFSET` ≠ `OFFSET/FETCH`** | SQL söz dizimi hatası (P1) | DB tespiti (§3.7) |
| E4 | **N+1** | yavaş sayfa; DoS benzeri | `JOIN`/`UNION ALL` tek sorgu |
| E5 | **Ham string concat** | SQL injection (P0) | `@p1..@pN` (G4) |
| E6 | `async void` / swallow | process crash / kayıp hata | G9/G10 → `Task` + yükselt |
| E7 | interface↔impl metot farkı (P0) | **çalışma anında** `NotImplementedException` | §5.3 komut-4 |
| E8 | Repo'da **EF/`IQueryable`** | ADR-004 ihlali; örtük sorgu | §5.3 EF scan |
| E9 | Tek repo'da **çok `using`** | P0 (G13) | tek scope; UnitOfWork paylaşır |
| E10 | **Locator yok** / connection **unutma** | bağlantı sızıntısı | §5.3 `using` denetimi |
| E11 | Config/DB **sınıfa gömme** | yol/secret sızıntısı | config **DI** ile (G18) |
| E12 | Kararsız `ORDER BY` | sayfada kayıp/tekrarlı satır | sabit sıralama (§4.4-7) |

### §5.3 Okuma komutları (değiştirme — read-only)

| # | Komut | Ne doğrular | G |
|---|---|---|---|
| 1 | `rg --no-messages -n "Microsoft\.EntityFramework\|System\.Data\.Entity" -g "*Repository.cs" .` | **EF yok** (ADR-004) | G2/G15 |
| 2 | `rg --no-messages -n "\\\$\"[^\"]*\\{|\\\`[^\`]*\\$" -g "*Repository.cs" -g "*.sql" .` | ham string yok | G4 |
| 3 | `rg --no-messages -n "@p[0-9]+" -g "*Repository.cs" .` | parametre mevcut (≥1) | G4 |
| 4 | `$i = rg -o --no-filename "Task<[^>]*>\w+Async" -g "I*Repository.cs" . \| Sort -Unique; $m = rg -o --no-filename "public [\w<>,\[\]\?]+ \w+Async" -g "*Repository.cs" . \| Sort -Unique; "iface=$(@($i).Count) impl=$(@($m).Count)"` | **interface↔impl** farkı → P0 | G14 |
| 5 | `rg --no-messages -n "using\s*\(\w*[Cc]onn" -g "*Repository.cs" . \| Measure-Object -Line \| Select-Object -ExpandProperty Lines` | **`using`** (conn scope) | G7/G12 |
| 6 | `rg --no-messages -n "async void\|catch\s*\{\s*\}" -g "*.cs" .` | **`async void`/swallow** yok | G9/G10 |
| 7 | `Get-ChildItem -Recurse -Filter versacoder.db \| Select-Object -First 5 FullName` | DB yolu/nötr ad | G18 |
| 8 | `rg --no-messages -n "List<" -g "I*Repository.cs" .` | `List<T>` imzası yok (§4.3-5) | G23 |
| 9 | `rg --no-messages -n "//\|///" -g "*Repository.cs" .` | kod içi yorum yok (§1.2) | G14 |

> **P2 kapsamı:** yalnız okuma; dosya/yazma **yok**. **P0 sızıntı yok** (no-op) · **repo yoksa** “no match”.

### §5.4 Quality Report (zorunlu çıktı)

```markdown
## Kalite Raporu - repository-template (P2, read-only)
- Tarama: §5.3 komut 1..9 (yalnız okuma, 02:34 UTC)
- EF: 0 · ham string concat: 0 · @p1: 23 · kod içi yorum: 0
- interface↔impl: 12/12 (G14: 0) · using(conn): 9 (G7/G12: 0)
- async void / swallow: 0/0 (G9/G10) · List<> imzası: 0
- versacoder.db yolu: 1 (G18: 0) · EF yazımı mirror'da: 0
- Kaplama: FakeRepo 93% (≥ %90) · Entegrasyon: 6/6
- Sonuç: 0 P0 · 0 P1 · 1 P2 — “tertemiz”
```

## §6 Doğrulama

### §6.1 Kod içi kontrol listesi (≥20)

- [ ] Primary constructor imzası (§3.2)
- [ ] Zorunlu method 7/7 (§3.4)
- [ ] Her async method `ct` alır
- [ ] `async void` yok (G9)
- [ ] `catch` swallow yok (G10)
- [ ] `using (conn)` tek `Open` (G7/G12)
- [ ] EF/`IQueryable` yok (G15)
- [ ] Ham string yok; `@p1` var (G4)
- [ ] Dönüş DTO/enum; paged = `IPagedResult` (§3.5)
- [ ] Liste `IReadOnlyList<T>`; `List<T>` imzası yok
- [ ] Somut sınıf `IReadRepository` uygulamıyor (G1)
- [ ] `EfCore*Repository` mirror'da yok (G2)
- [ ] Handler şablonu kopyası yok (G11)
- [ ] Config/secret sınıfta yok (G18/G19)
- [ ] N+1 yok; `JOIN`/`UNION ALL` var (G5)
- [ ] Paging: `LIMIT/OFFSET` | `OFFSET/FETCH` (G6)
- [ ] Optimistic concurrency açıklandı (§3.7)
- [ ] UnitOfWork vs TransactionScope belgeli (§3.7)
- [ ] Kapsam dışı liste §1.4/§2.2 tam
- [ ] Kutu 66 char; ASCII ≥ %95
- [ ] Test ≥ %90 + gerçek DB kopyası (§3.8)
- [ ] `exists-mismatch` §5.3-4: P0 değil
- [ ] Kod içi yorum yok (§1.2)
- [ ] Ürün = VERSA CODER; CoreMusic yok
- [ ] Mapping sınırı bozulmadı (dapper-mapping)
- [ ] `IUnitOfWork` yalnız çok-repo (§3.7)
- [ ] `ORDER BY` paged sorguda (§4.4-7)
- [ ] DTO `Application/DTOs` altında (§3.5)
- [ ] Kayıt yok → `Result`/Domain Failure (§3.5)
- [ ] Locator tek; ikinci P0 (G10)
- [ ] `AdoConnectionFactory` dışına connection yok (§3.7)
- [ ] `await using` async scope'ta (G13)
- [ ] Kutu satırları 66 (G21)
- [ ] `{{PLACEHOLDER}}`'ler dolu değil (YAGNI)
- [ ] ⚠️ VERIFICATION REQUIRED ibaresi var (§1.2)
- [ ] Migration yok testte doğrulandı (§3.8)
- [ ] Test DB'si seçilen DB ile aynı (G6)
- [ ] Tablo adı shema ile geldi mi (G19)
- [ ] `IReadOnlyRepository` yalnız okumada (§3.4)
- [ ] `Exists` varlıkta; `GetById` boş değil (G8)
- [ ] Enum dönüşü sabit string değil (§3.5)
- [ ] Sayfa `TotalCount/PageSize` (§3.5)
- [ ] Nötr ad `versacoder.db` (G18)
- [ ] Mirror taraması `rg` ile; sonuç §5.4'te
- [ ] Rapor §5.4 şablonunda
- [ ] §5.3 komutları yalnız okuma (P2)
- [ ] Sayfasız `GetListAsync`; sayfa `GetPagedAsync` (§3.4)
- [ ] `ExistsAsync` yalnız `bool` (§3.4)
- [ ] `AddAsync` yeni id döndürür (§3.5)
- [ ] `DeleteAsync` yanlış id'de `false` (§3.5)
- [ ] `ct` hiçbir method'ta opsiyonel değil (§4.1-2)
- [ ] `using` bloğunda erken `return` yok (G12)
- [ ] DTO dışında dönüş yok (§3.5)
- [ ] Enum zenginleştirme serviste (§3.5)
- [ ] `TotalCount` ayrı tek sorgu; N+1 değil (G5)

### §6.2 Mimari tutarlılık

| # | Soru | Evet |
|---|---|---|
| 1 | Katman oku tek yönlü (T4) | ☐ |
| 2 | Locator **tek** (`AdoConnectionFactory`) | ☐ |
| 3 | `IUnitOfWork` **yalnız** çok-repo | ☐ |
| 4 | DTO **Application/DTOs** | ☐ |
| 5 | Mapping **Infrastructure**'ta | ☐ |
| 6 | Servis **tek** veri kapısını görür | ☐ |
| 7 | Handler ile repo **karışmadı** (§2.2) | ☐ |
| 8 | `IUnitOfWork` **her yerde** değil | ☐ |
| 9 | Mirror'da **EF** yazımı yok (G2) | ☐ |

### §6.3 Derleme ve testler

| # | Kontrol | Evet |
|---|---|---|
| 1 | `dotnet build` **0 hata** | ☐ |
| 2 | Test **≥ %90** (Fake) | ☐ |
| 3 | Entegrasyon **gerçek DB kopyası** | ☐ |
| 4 | **N+1 yok** (§5.3) | ☐ |
| 5 | **Paging** iki DB'de de **geçerli** | ☐ |
| 6 | **`async void`/swallow** taraması **0** | ☐ |
| 7 | **`@p1`** parametre taraması **≥1** | ☐ |
| 8 | **Kod içi yorum** taraması **0** | ☐ |
| 9 | **`List<T>` imzası** taraması **0** | ☐ |

### §6.4 İş hedefi hizası

| # | Hedef | Evet |
|---|---|---|
| 1 | **Tek** veri kapısı | ☐ |
| 2 | DB **bakım** (bağlantı) görünmez | ☐ |
| 3 | **Add/GetById/GetList/Delete/Exists + paged** | ☐ |
| 4 | Testler **gerçek DB**'ye karşı | ☐ |
| 5 | **N+1 yok** → sayfa hızlı (G5) | ☐ |
| 6 | **Şema/shema** değişikliği **yok** (G19) | ☐ |

### §6.5 Dış doğrulama (diskten)

```powershell
$enc=New-Object System.Text.UTF8Encoding($false)
$f='C:\versacoder\.ai\.templates\backend\repository-template.md'
$t=[System.IO.File]::ReadAllText($f,$enc); $n=($t -split "`n")
$ne=@($n | Where-Object { $_ -match '\S' }).Count
"satir=$($n.Count) bos=$ne kb=$([math]::Round((Get-Item $f).Length/1KB,1))"
"bolum=$(@($n | Where-Object { $_ -match '^## \u00A7' }).Count)"
"mojibake=$(@($n | Where-Object { $_ -match '[\u00C3\u00C2]|\uFFFD' }).Count)"
"kutu_hatali=$(@($n | Where-Object { $_ -match '^[+|]' -and $_.Length -ne 66 }).Count)"
```

Beklenen: `satir` 480–660 · `bos` 480–660 · `kb` 25–34 · `bolum=7` · `mojibake=0` · `kutu_hatali=0`.

## §7 Referanslar

### §7.1 Bağlantılar (≥6)

1. [ascii-diagram-template.md](../architecture/ascii-diagram-template.md) — §1.3/§3.2/§5.3/§5.4 kalıpları
2. [dapper-mapping-template.md](../infrastructure/dapper-mapping-template.md) — mapping SSUT'u (sınır)
3. [service-template.md](service-template.md) — servis; repository'yi çağırır
4. [handler-template.md](handler-template.md) — CQRS/sıcak yol varyantı
5. [../../.rules/coding-standards.md](../../.rules/coding-standards.md) — naming, `async`, stil
6. [../../architecture/index.md](../../architecture/index.md) — katman diyagramı (T4 kaynağı)
7. [../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md](../../.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) — **EF yasağı** (SSUT)
8. [service-template.md §5.4](service-template.md) — Quality Report format kardeşi

### §7.2 Red Team (≥10 satır)

| # | iddia (SPEKÜLASYON — doğrula) | Savunma | Q |
|---|---|---|---|
| 1 | Diyagram **66 karakter** | §3.3 + §6.1 | `Q: İçerik değişirse? → 64 iç` |
| 2 | Mapping **bu dosyada değil** | §1.1/§7.1-2 (SSUT) | `Q: Sorgu içerirse? → dapper` |
| 3 | **`@p1..@pN`** zorunlu | §4.3-2 (G4) | `Q: Concat? → P0` |
| 4 | **EF/`IQueryable`** yasak | §4.3-1 (G15, ADR-004) | `Q: EfCoreRepo? → P0` |
| 5 | **`async void`/swallow** yok | §4.5 (G9/G10) | `Q: Yoksa? → DUR/P1` |
| 6 | **DB paging** farkı | §3.4/§3.7 (G6) | `Q: Yanlış? → SQL hatası` |
| 7 | **`IReadRepository` + sorgu** yasak | §4.5 (G3) | `Q: Service isterse? → handler` |
| 8 | **`EfCore*Repository` mirror'da yok** | §5.3-1 (G2/G15) | `Q: Varsa? → ADR-004 P0` |
| 9 | **`using (conn)` + tek `Open`** | §3.7 (G7/G12) | `Q: İki kez? → P0 sızıntı` |
| 10 | **Tek locator** | §3.6/§3.7 | `Q: İkinci? → P0` |
| 11 | **`interface↔impl` uyuşmazlığı P0** | §5.3-4 (G14) | `Q: Fark? → çalışma anı kırılır` |
| 12 | **`LIMIT/OFFSET` ≠ `OFFSET/FETCH`** | §3.7 (G6) | `Q: DB değişince? → P1` |
| 13 | **`IPagedResult<T>`** DB-bağımsız | §3.5 | `Q: Toplam? → COUNT` |
| 14 | **`Task<List<T>>`** imzası yok | §4.4 | `Q: Sızarsa? → mapping testi` |
| 15 | Mapping/bağlantı **diskte doğrulanmadı** | §1.2 ⚠️ VERIFICATION REQUIRED | `Q: Yalan? → P0 revizyon` |
| 16 | **Yorum ekleme** (G14); **CoreMusic** yok | §1.2/§6.1 | `Q: Sızarsa? → çoğaltır / marka` |
| 17 | **Paged** `ORDER BY` **zorunlu** | §4.4-7 (G8) | `Q: Yok? → kararsız sayfa` |
| 18 | **Konfig** sınıfta **yok** | §2.1 (G24) | `Q: Gömülürse? → P0 sızıntı` |
| 19 | **Nötr DB adı** `versacoder.db` | §5.3-7 (G18) | `Q: Başka ad? → P2` |
| 20 | **Kutu tek** (T4) — ikinci kutu yok | §3.3 | `Q: Eklersen? → §5.4 P2` |
