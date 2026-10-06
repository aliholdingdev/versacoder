---
title: "VersaCoder — Entegrasyon Test Şablonu (Integration Test)"
type: template
category: templates/testing
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "entegrasyon test sablonu: fixture guni, sahte ve gercek collaborator, iptal/tekrar/timeout, transaction siniri ve %90 coverage kaniti"
---

# VersaCoder — Entegrasyon Test Şablonu (Integration Test)

**Zorunlu Bağlantılar:** [index](../index.md) · [unit-test-template](../testing/unit-test-template.md) · [test-plan-template](../testing/test-plan-template.md) · [repository-template](../backend/repository-template.md) · [dapper-mapping-template](../infrastructure/dapper-mapping-template.md) · [CLAUDE](../../../CLAUDE.md)

## §1 Amaç

### §1.1 Bu şablon ne üretir?

**`tests/VersaCoder.IntegrationTests/**` altında tek bir entegrasyon test dosyası**: gerçek
ADO.NET + Dapper yolunu, gerçek şema ve gerçek transaction sınırını kanıtlayan xUnit testleri.
Birim testi sahte repo ile **sözleşmeyi** sınar; bu şablon sahte olmayan tek şeyi sınar —
`@p1..@pN` parametre bağlama, `LIMIT/OFFSET` sayfalama, idempotency, transaction rollback ve
bağlantı havuzu davranışı. Çıktı kod olduğu için bu dosya **kod iskeleti değil, test sözleşmesidir**:
test kimliği (5 alan), sınıf/metot isimlendirme, fixture dayanağı, collaborator seçimi, veri
hazırlama-temizleme, determinizm, kategori etiketleri, `%90` coverage ve **neyin test edilmediği**
burada kilitlenir.

| Öğe | Değer |
|------|-------|
| Şablon adı | `integration-test-template.md` |
| Şablon yolu | `.ai/.templates/testing/integration-test-template.md` |
| Hedef dosya | `tests/VersaCoder.IntegrationTests/**/*.cs` |
| Hedef kod | C# · xUnit (`[Fact]`/`[Theory]`/`[Trait]`) · NSubstitute |
| Proje | `VersaCoder.IntegrationTests` — **ayrı proje**, katman **DEĞİLDİR** (kök `CLAUDE.md` B5) |
| Katman erişimi | yalnız `l4` (ADO.NET + Dapper) + `l0` portları · `l2` handler · `l3` `IClock` |
| Veri erişimi | ADO.NET + Dapper · parametre `@p1..@pN` (G2, G10) · `SELECT *` yasak |
| Kodlama | UTF-8 (BOM'suz) · Türkçe · mojibake yasak (G5) |
| Üretilen uzunluk | 150–320 satır (kod) + bu şablonun asgari 480 satırı (G6) |
| Sürüm / Authority | `1.0.0` · `SSOT` |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz; `⚠️ VERIFICATION REQUIRED` yazılır ve satır olduğu yerde
durur. Ölçüm: `C:\versacoder`, 2026-10-05 (PowerShell).

| # | İddia (SPEKÜLASYON) | Kanıt yolu | Durum |
|---|---|---|---|
| 1 | `tests/VersaCoder.IntegrationTests/**` klasör ağacı var | `Test-Path C:\versacoder\tests` → **False** | ⚠️ VERIFICATION REQUIRED |
| 2 | `dotnet test` çalışıp yeşil dönüyor | `.sln` = **0 dosya** → `MSB1003` | ⚠️ VERIFICATION REQUIRED |
| 3 | `versacoder.db` var, migration uygulanabilir | `Get-ChildItem -Recurse -Filter versacoder.db` = **0** | ⚠️ VERIFICATION REQUIRED |
| 4 | Testcontainer / docker dayanağı kurulabilir | docker kanıtı **0** → bu şablon dayanağı **SQLite kopyası** ile kurar | ⚠️ VERIFICATION REQUIRED |
| 5 | `.ai/.rules/` entegrasyon kuralını taşır | `.ai/` altında **yalnız** `.templates/` (38 MD) | ⚠️ VERIFICATION REQUIRED |

**Doğrulanmış gerçekler (disk ölçümü 2026-10-05):**

| # | Kanıt | Değer | Komut |
|---|---|---|---|
| 1 | `.NET SDK` | **10.0.301** ✅ | `dotnet --version` |
| 2 | Test yığını beyanı | **xUnit + NSubstitute + NetArchTest** | kök `CLAUDE.md` B5 (okundu) |
| 3 | Hedef coverage + uyarı | **%90** + `TreatWarningsAsErrors=true` | kök `CLAUDE.md` B6 G11 |
| 4 | Veri erişimi yasağı | **EF Core yasak** → ADO.NET + Dapper | kök `CLAUDE.md` B6 G2 |
| 5 | Şablon envanteri | **38 MD · 10 alt klasör** | `Get-ChildItem .ai\.templates -Recurse -Filter *.md` |
| 6 | `src/` üretim kodu | **False** (9 proje yazılmadı) | `Test-Path` |
| 7 | Kapı durumu | `GATE-003` açık · `GATE-005` **BLOKE** (`tests/` YOK) | `WORKFLOW.md` §3 (okundu) |
| 8 | Küçük kardeş şablonlar | `unit-test-template.md` · `test-plan-template.md` ✅ | `Get-ChildItem .templates\testing` |

**ÇELİŞKİ PROTOKOLÜ:** kök `CLAUDE.md` B5 "test ayrı projedir (9 üretim + 1-2 test, en fazla 11)"
der; `unit-test-template.md` §1.2 ise `VersaCoder.Tests` adını kullanır. Bu şablon **iki adı da**
kullanmaz: entegrasyon hattı **her zaman** `VersaCoder.IntegrationTests` projesindedir, birim
hattı `VersaCoder.Tests`. Çelişki çözülene kadar ikisi ayrı kalır (`CONFLICT DETECTED` → sahibi
`tester`/`architect`, son söz `kullanıcı`).

### §1.3 Terminoloji

| Terim | Anlamı | Terim | Anlamı |
|---|---|---|---|
| Entegrasyon | Gerçek I/O + gerçek şema | Fixture | Test grubu başına kurulum/temizlik |
| Dayanak | SQLite/ in-memory sahte DB | Persistence abstraction | `l0` portu — şema onu uygular |
| Idempotency | Aynı işlem 2. kez aynı sonucu verir | Transaction sınırı | Commit/rollback kapsamı |
| Cancellation | `ct` ile iptal edilebilirlik | Timeout | Azami süre → `TimeoutException` |
| Retry | Geçici hata için tekrar deneme | Collaborator | SUT'un komşusu (repo/handler/servis) |
| Sahte (fake) | Arayüzü uygulayan hızlı test çifti | Gerçek | Gerçek ADO.NET + Dapper yolu |
| Determinizm | Saat/rastgelelik enjeksiyonu | Kategori etiketi | `[Trait("Category","Integration")]` |

### §1.4 Karşılık gelen (veya karşılığı olmayan) şablonlar

| Aşama | Şablon | Sebep |
|---|---|---|
| Saf davranış / sahte repo | `unit-test-template` | I/O yok, hızlı, `%90`'ın ana gövdesi |
| Ne test edilecek, risk, takvim | `test-plan-template` | Kapsam ve release gate planı |
| Repo implementasyon sözleşmesi | `backend/repository-template` §3.8 | Sahte repo örneği orada |
| Kolon ↔ özellik eşlemesi | `infrastructure/dapper-mapping-template` | `SqlMapper` ayarları orada |
| Sorgu metni + indeks notu | `query/Query-Template` | `@p1..@pN` kalıbının kendisi |
| SQL migration + şema | `infrastructure/migration-template` | Şema burada değil, testin **girdisi** |
| Form/CLI davranış doğrulaması | `frontend/form-template` §3.7 · `other/cli-command-template` §5.3 | UI/CLI kabuğu ayrı hattır |
| Diyagram kutu standardı | `architecture/ascii-diagram-template` | 66 sütun kuralının tek kaynağı |

**KAPSAM SINIRI:** Bu dosya **yalnız** `*.cs` entegrasyon testi üretir. Kapsam/risk/takvim →
`test-plan-template`. Saf birim → `unit-test-template`. Şema/migration → `infrastructure/*`.

## §2 Kapsam

### §2.1 Bu şablonu kullan

| Durum | ✅ Kullan |
|---|---|
| 1 | **Gerçek SQL yolu** kanıtlanacak (`@p1..@pN` bağlama, `LIMIT/OFFSET`, `ORDER BY`) |
| 2 | **Transaction sınırı** (rollback, kısmi yazım, hata sonrası tutarlılık) |
| 3 | **Idempotency** — aynı komut 2. kez verildiğinde ikinci kayıt doğmamalı |
| 4 | **Migration + seed** sonrası uçtan uca akış (schema → seed → sorgu → assert) |
| 5 | **Retry / timeout / cancellation** davranışı gerçek bağlantı üzerinde |
| 6 | **N+1** şüphesi: çağrı sayısı ve satır sayısı birlikte doğrulanacak |
| 7 | Birim testte sahte ile **gerçek** davranış farkı çıktı (§3.5) |

**YAPAR — kanıt üretir:** fixture'ı **persistence abstraction üzerinden** kurar · her testte
**en az 2 anlamlı assertion** (dönüş + yan etki) tutar · veriyi **kendi** testi için hazırlar ve
**temizler** (§3.6) · saati ve rastgeleliği **enjekte** eder (§3.7) · her testi **kategori
etiketler** (§3.2) · `%90` coverage kanıtını **raporlar** (§3.11) · §5.3 komutlarıyla **salt-okunur**
denetlenir.

**YAPMAZ:** **EF Core / `DbContext` / `DbSet` / `IQueryable` / `InMemory`** (G2) · `SELECT *`
(G10) · string concat SQL (G10) · testte `DateTime.Now` / `Guid.NewGuid` / `new Random()`
(§3.7) · üretim kodunu **düzeltme** · `skip`/`Ignore` ile yeşil gösterme · gerçek **kullanıcı
verisi** (G3) · Testcontainer/id docker **varsayımı** (kanıt yok, §1.2-4) · **CoreMusic** adı (G1)
· kod içi serbest yorum (yalnız §3.1 kimlik bloğu).

### §2.2 Bu şablonu KULLANMA — alternatifler

| Bu dosya değil | Nerede | Ayırt edici işaret |
|---|---|---|
| Saf servis/handler davranışı | `unit-test-template` | I/O yok, sahte arayüz |
| Ne test edilecek / kabul kriteri | `test-plan-template` | `.ai/reports/test-plan-*.md` |
| Şema üretimi / migration dosyası | `infrastructure/migration-template` | `db/migrations/V0xx__*.sql` |
| Sorgu metni tasarımı | `query/Query-Template` | `@p1..@pN` + indeks notu |
| Kolon↔özellik eşlemesi | `infrastructure/dapper-mapping-template` | `SqlMapper` davranışı |
| Mimari yön testi (NetArchTest) | kök `CLAUDE.md` B5 (yön oku) | Bağımlılık, davranış değil |
| Form CRUD olay akışı | `other/winforms-form-template` §3.7 | `l7` kabuğu, DB değil |
| CLI dönüş kodu doğrulaması | `other/cli-command-template` §5.3 | `dotnet run -- …` |
| Dapper mapping birim testi | `unit-test-template` §3.8 | Sahte repo, gerçek DB değil |

## §3 Mimari

### §3.1 Test kimliği — Purpose / Input / Expected Result / Failure Criteria / Evidence

Her entegrasyon testi **beş zorunlu alan** taşır. Bu beş satır, entegrasyon testinin
**tek izinli yorum bloğudur** — dışında kod içi yorum `RED`tir (§4.3-6).

| Alan | Soru | Nereye yazılır | Örnek |
|---|---|---|---|
| `Purpose` | Bu test **neyi** kanıtlar? | 1. `///` satırı | `Aynı komutun ikinci tekrarı ikinci kayıt üretmez.` |
| `Input` | Somut girdi nedir? | 2. `///` satırı | `kod=ANAHTAR, sayfa=1, boyut=5, seed=1042` |
| `Expected Result` | Başarıda ne döner? | 3. `///` satırı | `kayit=1 · dönen satır=1 · kod=KOD_001 yok` |
| `Failure Criteria` | Ne olursa **başarısızdır**? | 4. `///` satırı | `2. kayıt oluşursa ya da rollback yarım kalırsa` |
| `Evidence` | Kanıt nerede? | 5. `///` satırı | `§5.3/2 çıktısı + §5.3/5 coverage satırı` |

```csharp
/// <Purpose>Aynı komut ikinci kez verildiğinde ikinci kayıt oluşmaz.</Purpose>
/// <Input>kod=ANAHTAR, seed=1042, idempotency-key=IDK-7</Input>
/// <Expected Result>kayit=1, yanit=Conflict, toplam sure degismez</Expected Result>
/// <Failure Criteria>ikinci INSERT gecerlesirse ya da TransactionScope yarim kalirsa</Failure Criteria>
/// <Evidence>dotnet test --filter "FullyQualifiedName~ProyectoRepository_idempotency" + coverage raporu</Evidence>
```

1. **Purpose tekil** olur — bir test **tek** davranışı sınar (SRP).
2. **Input** sayısal ve **tekrarlanabilir** olur (seed sabit, §3.7).
3. **Expected Result** ölçülebilir değer taşır (`kayit=1`), "doğru çalışır" değil.
4. **Failure Criteria** testin **ne zaman kırmızıya** döneceğini söyler; boş bırakılamaz.
5. **Evidence** bir §5.3 komutuna bağlanır; kanıtsız test **P1** (§6.1 K14).

### §3.2 İsimlendirme ve kategori etiketleri

**Biçim:** `<Sistem>_<Senaryo>_<Beklenen>` — ASCII, boşluksuz, alt çizgi ayraç, **≤ 96 karakter**.
Sistem adının metotta tekrarı **bilinçlidir**: `--filter` tek başına çalışsın diye sınıf adına
gerek kalmasın.

| Öğe | Biçim | Örnek |
|---|---|---|
| Dosya | `{Sistem}IntegrationTests.cs` | `ProyectoRepositoryIntegrationTests.cs` |
| Sınıf | `{Sistem}IntegrationTests` | `ProyectoRepositoryIntegrationTests` |
| Metot | `{Sistem}_{Senaryo}_{Beklenen}` | `ProyectoRepository_idempotency_ikinci_kayit_olusmaz` |
| Kategori | `[Trait("Category","Integration")]` | sınıf seviyesinde **bir kez** |
| Hız | `[Trait("Speed","Slow")]` | `> 2 sn` süren her test için zorunlu |
| Katman | `[Trait("Layer","l4")]` | sınanan kodun katmanı (B5) |

- **Sınıf adı** = tek sistem = tek test sınıfı; ikinci sınıf yalnız **farklı bir sistem** için açılır.
- **Yasak ön ekler:** `Test_`, `Should_`, `Given_When_` — depo standardı üçlü alt çizgidir.
- **Kategori taraması:** sınıf üstünde `Category=Integration` **yoksa** test filtrede kaybolur → P1 (§5.3/3).

### §3.3 Fixture ve dayanak — persistence abstraction üzerinden

Testcontainer/docker **kanıtı yok** (§1.2-4). Bu şablon dayanağı **basit ve yerel** kurar:
dayanak, `l0` persistence abstraction'ının **kendi** uygulamasıdır — şemanı o açar, seed'i o
yazar, transaction'ı o yürütür.

```text
+----------------------------------------------------------------+
| ► ENTEGRASYON FIXTURE — tek kaynak: persistence abstraction    |
|                                                                |
|  [F1] ITestDatabase (l0 portu)  ->  SQLite kopyasi / in-memory |
|         v                                                      |
|  [F2] ShemaAc        migration dogrulanir, DROP/CREATE tek yer |
|         v                                                      |
|  [F3] SeedEt         sabit seed (1042) + sentetik veri         |
|         v                                                      |
|  [F4] ConnectionAc   ADO.NET + Dapper, @p1..@pN hazir          |
|         v                                                      |
|  [F5] Dispose        ROLLBACK + seed temizligi (her test sonu) |
|  x[] EF Core / InMemory DbSet / docker varsayimi  ->  RED (G2) |
+----------------------------------------------------------------+
```

| Kural | ✅ Yapılır | ❌ Yapılmaz |
|---|---|---|
| Dayanak seçimi | Tek **dosya kopyası** SQLite (ya da gerçek şemanın in-memory kopyası) | Docker/Testcontainer **varsayımı** |
| Şema kaynağı | `infrastructure/migration-template` çıktısı **aynen** uygulanır | Elle yazılmış ikinci şema |
| Ömür | `IAsyncLifetime` / `IDisposable` ile **test sınıfı başına** | Statik global `AppDomain` durumu |
| Bağlantı | Her fixture **tek** havuz; testler havuzu **paylaşmaz** | Ortak tek `IDbConnection` |
| Yalıtım | Her test **kendi** transaction'ında (§3.6) | Testler arası görünür kalıntı |
| EF | **YASAK** — `InMemoryProvider` dahil (G2) | `DbContext` sahtesi |

**Neden basit dayanak:** entegrasyon testinin **sınadığı şey** konteyner değil, SQL'in kendisidir;
konteyner ekibe çalışma süresi maliyeti getirir ama kanıt **eklemez**. Konteyner kararı kanıtla
gelenirse §1.2 tablosu güncellenir, bu şablon **önce** varsaymaz.

### §3.4 Arrange-Act-Assert ve hat diyagramı

```text
+----------------------------------------------------------------+
| ► ENTEGRASYON HATTI — AAA · gercek ADO.NET · deterministik     |
|                                                                |
|  [1] ARRANGE  fixture + seed + somut girdi (Purpose/Input)     |
|         v                                                      |
|  [2] ACT      tek eylem — tek komut, ct ve timeout açık        |
|         v                                                      |
|  [3] ASSERT   donus + yan etki (kayit sayisi / tablo satiri)   |
|         v                                                      |
|  [4] EVIDENCE  §5.3/2 filtresi yeşil · §5.3/5 coverage >= %90  |
|  x[] SELECT * · concat SQL · DateTime.Now  ->  RED (G10 / §3.7)|
+----------------------------------------------------------------+
```

1. **Arrange** bloğu testin başında biter: fixture, seed, somut girdi orada kalır.
2. **Act** bloğunda **tek** eylem vardır (`await sut.XAsync(..., ct)`); ikinci çağrı varsa test
   **iki** teste bölünür (idempotency testi ayrıdır, §3.8).
3. **Assert** bloğu **en az 2** doğrulama ister: dönüş değeri **+** yan etki (tablo satırı ya da
   çağrı sayısı). Tek assertion **1/2 puan** (§6.1 K15).
4. Beklenen hata `await Assert.ThrowsAsync<T>(...)` ile **adlandırılır**; `try/catch` **RED**.
5. **Test başına tek** iddia: "ve" ile başlayan assert mesajı **bölünür**.
6. Kutu satırları tam **66 karakter** (§5.3/6 ölçer); `ascii-diagram-template` primitifleri.

### §3.5 Sahte (fake) vs gerçek collaborator seçimi

| Collaborator | Seçim | Neden | Ne kanıtlamaz |
|---|---|---|---|
| **Veritabanı / şema** | **Gerçek** (SQLite kopyası) | SQL ve tip dönüşümü gerçek | Uzak ağ davranışı |
| **Repository (`l4`)** | **Gerçek** uygulama + sahte bağlantı yok | `@p1..@pN` gerçekten bağlanır | İş kuralı dalları |
| **Handler / servis (`l2`)** | **Gerçek**, repo gerçek | Uçtan uca akış | UI/CLI kabuğu |
| **Saat (`IClock`, `l3`)** | **Sahte** (sabit zaman) | Determinizm (§3.7) | Saat dilimi gerçekçiliği |
| **Dış servis (LLM, git)** | **Sahte** (NSubstitute / yerel çift) | Ağ + maliyet + kararsızlık | Gerçek ağ hataları |
| **Dosya sistemi** | **Gerçek** geçici klasör | Doğru yol davranışı | Paylaşımlı disk kilitleri |
| **E-posta / bildirim** | **Sahte** (spy) | Yan etki yalnız assert'te | Teslim edilme |

**Seçim kuralı:** *gerçek olan şey kanıttır, sahte olan şey hızdır.* Bir testte **> 2 sahte
collaborator** varsa entegrasyon testi kimliğini kaybetmiştir → P2 (§6.2 R5). Sahte ile gerçek
arası fark §3.11 "neyin test edilmediği" listesine **gerekçesiyle** yazılır.

### §3.6 Test verisi hazırlama, temizleme ve transaction sınırı

| # | Kural | Yapılan | Yapılmayan |
|---|---|---|---|
| 1 | **Sentetik veri** | sabit `seed=1042` ile üretilir | gerçek kullanıcı verisi (G3) |
| 2 | **Hazırlama** | `SeedEt` tek yerde, test **içinde** tekrar yazılmaz | her testte elle `INSERT` |
| 3 | **Temizlik** | test sonu `ROLLBACK` + `Dispose` | `DELETE *` ile top-temizlik |
| 4 | **Transaction sınırı** | test **kendi** transaction'ında | paylaşılan tek transaction |
| 5 | **Bağımlı test yasak** | her test tek başına da yeşil | sıra beklentisi |
| 6 | **Yinelenen anahtar** | çakışma `Conflict` döner | ikinci sessiz kayıt |
| 7 | **Idempotency anahtarı** | `@p1..@pN` içinde taşınır | anahtarın metinde birleşimi |
| 8 | **Büyük veri** | 10k satır **tek** batch ile | 10k ayrı insert (N+1) |

**Transaction sınırı testi (zorunlu 1 senaryo):** başlangıçta `kayit=1` · işlem ortasında hata
fırlat · beklenen: `kayit=1` **kalmıştır** (rollback tam) — `Failure Criteria`: `kayit=2` ya da
yarım satır görülürse kırmızı.

### §3.7 Determinizm — saat ve rastgelelik enjeksiyonu

| Kaynak | ❌ Testte | ✅ Testte |
|---|---|---|
| Saat | `DateTime.Now`, `DateTime.UtcNow` | `IClock` (l3) — sabit `2026-10-05T00:00:00Z` |
| Rastgelelik | `new Random()`, `Guid.NewGuid()` | `seed=1042` deterministik üreteç |
| Sayısal | ölçeklenen süre (`> 50 ms`) | mutlak eşik + `Speed=Slow` etiketi |
| Sıra | `ORDER BY`sız koleksiyon | `ORDER BY` sabit + `Assert.Equal` sıralı |
| Culture | varsayılan kültür | `CultureInfo.InvariantCulture` |
| Para/ondalık | `1,5` ↔ `1.5` | `decimal` + tolerans (`InRange`) |

**Kural:** determinizm kırıcı her kullanım `RED` (§4.3-7) ve §5.3/4 taramasında görünür. Test
**iki kez** çalıştırıldığında da aynı sonucu vermeli (flaky → P0).

### §3.8 İdempotency, cancellation, timeout, retry

| # | Davranış | Kurulum | Beklenen | Test adı parçası |
|---|---|---|---|---|
| 1 | İdempotency | aynı anahtarla 2 çağrı | 2. çağrı çakışma döner, kayıt **1** | `_idempotency_ikinci_kayit_olusmaz` |
| 2 | Cancellation | `ct` önceden iptal | `OperationCanceledException` | `_iptal_edilmis_ct_ile_firlatir` |
| 3 | Cancellation (ortada) | 1. await sonrası iptal | kısmi yazım **yok** (rollback) | `_iptalde_kismi_yazim_olmaz` |
| 4 | Timeout | sahte `TimeoutException` kaynağı | hata **yükseltilir**, yutulmaz | `_timeout_yukseltilir` |
| 5 | Retry (geçici) | 1. deneme başarısız, 2. başarılı | **2** çağrı, sonuç başarılı | `_gecici_hatada_tekrar_dener` |
| 6 | Retry (kalıcı) | her deneme başarısız | son hata döner, sayı **sınırında** durur | `_sinirda_durdurur` |
| 7 | Transaction | ara adımda hata | rollback, `kayit` değişmez | `_hatada_rollback_yapar` |
| 8 | Eşzamanlık | ardışık/paralel 2 çağrı | deterministik sıra, yarış yok | `_paralel_cagri_tek_kayit` |

**Kural:** retry **sınırı** (ör. 3) sabittir ve assert'te **doğrudan** yazılır; sonsuz döngü
testte `Timeout` ile **adlandırılır** (§4.5 G11).

### §3.9 ADO.NET + Dapper doğrulama (G2 / G10)

| # | Kontrol | ✅ Doğru | ❌ Yanlış (RED) |
|---|---|---|---|
| 1 | Parametre | `@p1..@pN` + `DynamicParameters` | `"...'" + kod + "'"` (concat) |
| 2 | Kolon listesi | `SELECT id, kod, ad FROM ...` | `SELECT *` |
| 3 | EF | yok | `EntityFramework`, `DbContext`, `DbSet<` |
| 4 | Sayfalama | `LIMIT @p2 OFFSET @p3` | bellekte `Skip/Take` |
| 5 | Sıra | `ORDER BY` sabit + ikinci anahtar | `ORDER BY`sız sayfalama |
| 6 | Transaction | `BeginTransaction` / `Commit`-`Rollback` | otomatik-commit varsayımı |
| 7 | Bağlantı | `await using` + havuz | açık bağlantı sızıntısı |
| 8 | N+1 | döngü **dışında** tek sorgu | satır başına sorgu |

**Doğrulama testi:** sorgu metnini assert ile **değil**, §5.3/4 taraması ile denetle; davranış
assert'i ile **sonucu** doğrula (metin testi kırılgandır).

### §3.10 Dolu VersaCoder örneği ve tam kalıp

**Dolu örnek (`tests/VersaCoder.IntegrationTests/ProyectoRepositoryIntegrationTests.cs`):**

```csharp
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Xunit;

namespace VersaCoder.IntegrationTests;

[Trait("Category", "Integration")]
[Trait("Layer", "l4")]
public sealed class ProyectoRepositoryIntegrationTests : IAsyncLifetime
{
    private ITestDatabase _db = null!;
    private IProyectoRepository _sut = null!;

    public async Task InitializeAsync()
    {
        _db = await TestDatabase.CreateAsync(seed: 1042);
        _sut = new ProyectoRepository(_db);
    }

    public Task DisposeAsync() => _db.DisposeAsync();

    /// <Purpose>Ayni komut ikinci kez verildiginde ikinci kayit olusmaz.</Purpose>
    /// <Input>kod=ANAHTAR, idempotency-key=IDK-7, seed=1042</Input>
    /// <Expected Result>kayit=1 ve ikinci cagri Conflict doner</Expected Result>
    /// <Failure Criteria>ikinci INSERT gecerleserse TransactionScope yarim kalirsa</Failure Criteria>
    /// <Evidence>dotnet test --filter "FullyQualifiedName~idempotency" + coverage raporu</Evidence>
    [Fact]
    public async Task ProyectoRepository_idempotency_ikinci_kayit_olusmaz()
    {
        using var tx = await _db.BeginTransactionAsync();

        var birinci = await _sut.EkleAsync(new ProyectoEkleKomut("ANAHTAR", "IDK-7"), tx);
        var ikinci = await _sut.EkleAsync(new ProyectoEkleKomut("ANAHTAR", "IDK-7"), tx);

        var sayi = await _db.SayAsync<int>("SELECT COUNT(*) FROM proyecto", tx: tx);

        Assert.True(birinci.Basarili, "Ilk ekleme basarili olmali");
        Assert.False(ikinci.Basarili, "Ikinci ayni komut Conflict donmeli");
        Assert.Equal(1, sayi, "Tabloda tek kayit kalmali");
    }

    [Fact]
    public async Task ProyectoRepository_iptalde_kismi_yazim_olmaz()
    {
        using var tx = await _db.BeginTransactionAsync();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _sut.EkleAsync(new ProyectoEkleKomut("IPTAL", "IDK-9"), tx, cts.Token),
            "Iptal edilmis ct firlatilmali");

        var sayi = await _db.SayAsync<int>("SELECT COUNT(*) FROM proyecto", tx: tx);
        Assert.Equal(0, sayi, "Iptalde hicbir satir yazilmamali");
    }
}
```

**Tam kalıp (yalnız bu blokta `{{…}}` yaşar; üretilen kodda adedi 0 olmalıdır):**

````markdown
```csharp
[Trait("Category", "Integration")]
public sealed class {{SISTEM}}IntegrationTests : IAsyncLifetime
{
    private ITestDatabase {{DB}} = null!;
    private {{ARAYUZ}} {{SUT}} = null!;

    public async Task InitializeAsync()
    {
        {{DB}} = await TestDatabase.CreateAsync(seed: {{SEED}});
        {{SUT}} = new {{SISTEM}}({{DB}});
    }

    public Task DisposeAsync() => {{DB}}.DisposeAsync();

    /// <Purpose>{{AMAC}}</Purpose>
    /// <Input>{{GIRINTI}}</Input>
    /// <Expected Result>{{BEKLENEN}}</Expected Result>
    /// <Failure Criteria>{{BASARISIZLIK}}</Failure Criteria>
    /// <Evidence>{{KANIT}}</Evidence>
    [Fact]
    public async Task {{SISTEM}}_{{SENARYO}}_{{BEKLENEN}}()
    {
        using var tx = await {{DB}}.BeginTransactionAsync();

        var sonuc = await {{SUT}}.{{METOT}}({{GIRINTI}}, {{CT}});

        Assert.True(sonuc.Basarili, "{{BASARI_CUMLESI}}");
        Assert.Equal({{BEKLENEN_SAYI}}, {{SAYIM}}, "{{YAN_ETKI_CUMLESI}}");
    }
}
```
````

### §3.11 Coverage hedefi %90 ve neyin test edilmediği

| Metrik | Hedef | Aşılırsa | Eksik kalırsa |
|---|---|---|---|
| Satır kapsamı (entegrasyon + birim birlikte) | **%90** | — | **RED** (G11) |
| `TreatWarningsAsErrors` | `true` | — | **RED** (G11) |
| Assertionsız test | **0** | — | P0 |
| Kapsamsız saf dal | **0** | — | P2 |
| Kategori etiketsiz test | **0** | — | P1 (§3.2) |

**Neyin test edilmediği (zorunlu liste — her PR'da yazılır):**

| Kapsam dışı | Gerekçe | Nerede kanıtlanır |
|---|---|---|
| Gerçek uzak sunucu / production DB | ortam + gizlilik | §1.2 + test-plan |
| Docker/Testcontainer dayanağı | kanıt yok (§1.2-4) | kararı kullanıcı verir |
| WinForms `InitializeComponent` | tasarım-süresi kodu | `other/winforms-form-template` §5.3 |
| CLI ayrıştırma dalları | `l7` kabuğu ayrı hat | `other/cli-command-template` §5.3 |
| Üçüncü taraf kütüphane içi | onarılabilirlik yok | — |
| Ağ kesintisi / paket kaybı | fiziksel ortam gerekir | manuel senaryo |
| `l0` port **arayüzü** yalnız | sözleşme birimde | `unit-test-template` §3.8 |

> **Kural:** kapsam dışı her madde **gerekçesiyle** yazılır; gerekçesiz kapsam dışı = **P1**.

## §4 Kurallar

### §4.1 Bu depoda

1. **xUnit** (`[Fact]`/`[Theory]`/`[Trait]`) **birincildir**; ek çerçeve yalnız **yazılı gerekçe**
   ile girer ve §6.1 K5'te kaydedilir. MSTest kalıntısı **RED**.
2. Entegrasyon hattı **`VersaCoder.IntegrationTests`** projesinde yaşar; test **katman değildir**
   (kök `CLAUDE.md` B5).
3. Veri erişimi **ADO.NET + Dapper**, parametre **`@p1..@pN`** (G2, G10); `SELECT *` **RED**.
4. Her test **deterministiktir**: `IClock` + sabit seed (§3.7).
5. Her testte **≥ 2 anlamlı assertion** ve **5 alanlı kimlik bloğu** vardır (§3.1).
6. Her test **kategori etiketlidir**: `Category=Integration` + `Layer` (§3.2).
7. Kapsam hedefi **%90** ve `TreatWarningsAsErrors=true` **zorunludur** (G11).
8. Kapsam dışı her durum §2.2'ye yönlendirilir; üretim kodu **düzeltilemez** (tester yetkisi dışı).

### §4.2 Kompozisyon

1. Fixture **tek** yerde (`TestDatabase`); test içi tekrarlı kurulum **RED** (§3.3).
2. Test verisi **sentetik + sabit seed**; kopyala-yapıştır `INSERT` bloğu **RED** (§3.6).
3. AAA blokları ayrı; `Arrange` içinde `Act` **RED**.
4. Transaction **testin kendi** sınırında; testler arası paylaşım **RED** (§3.6-4).
5. Sahte collaborator **≤ 2**; fazlası testin entegrasyon kimliğini bozar (§3.5).
6. Beklenen hata `ThrowsAsync` ile adlandırılır; `try/catch` **RED**.
7. `await` edilmeyen `Task` → **false green**; her async çağrı `await` edilir (P0).
8. Kod içi yorum **yalnız** §3.1 kimlik bloğunda (5 satır); gerisi **RED**.

### §4.3 Yasaklar (RED)

1. **EF Core / `DbContext` / `DbSet` / `IQueryable` / InMemory** — **RED** (G2).
2. **`SELECT *`** ve **string concat SQL** — **RED** (G10).
3. **Boş assertion / always-true test** (`Assert.True(true)`, assertion'sız) — **RED**.
4. **`async void`** test metodu · **boş `catch { }`** · exception **swallow** — **RED**.
5. **`DateTime.Now` / `Guid.NewGuid` / `new Random()`** (sebepsiz) — **RED** (§3.7).
6. §3.1 **dışındaki** kod içi yorum (`//`, `/* */`) — **RED**.
7. **`skip` / `Ignore`** ile yeşil gösterme — **RED** (kanıtsız).
8. **Gerçek kullanıcı verisi** (isim, e-posta, kart) test verisine — **RED** (G3).
9. **CoreMusic** adı — **RED**; ürün **VERSACODER / VersaCoder** (G1).
10. Testin **üretim kodunu düzeltmesi** — **RED**; kök neden `developer`'a döner.

### §4.4 Guardrail serisi (entegrasyon testi için genişletilmiş)

1. Test **tek** davranışı sınar; "ve" ile başlayan assert mesajı → **böl** (P1).
2. **Yinelenen seed bloğu** iki dosyada → `TestDatabase`'e taşı (P2).
3. **Sıra bağımlı** testler → fixture yalıtımı (P0, §5.2 E4).
4. Sahte ile gerçek **davranış farkı** → kaydı §3.11 listesine yaz (P1).
5. Coverage şişiren **saf getter**'lar sayılmaz; gerekçe raporda (§3.11).
6. Türkçe karakter `ç ğ ı İ ö ş ü` içeren veri testi **var**; mojibake **0** (G5).
7. `Speed=Slow` etiketsiz 2 sn üstü test → P2 (filtre kırılır).
8. **Tekrarlı test** (aynı Arrange iki dosyada) → `TestData`'ya taşı (P2).

### §4.5 Hard Guardrails (G1..G12 — otomatik DUR)

| ID | Guardrail | Kapsam | İhlal | Tespit |
|---|---|---|---|---|
| **G1** | Harici vault'tan cümle kopyalanamaz; yapı referans, içerik özgün | tümü | Red + sil | §6.2 R1 |
| **G2** | **EF Core yasak** — ADO.NET + Dapper, `@p1..@pN` | test + hedef | Red | §5.3/4 |
| **G3** | Frontmatter'sız Markdown yasak (8 alan) · gerçek kullanıcı verisi yasak | tümü | Red | §6.1 K1 |
| **G4** | Üretilen şablonda `{{…}}` yalnız §3.10 kalıp bloğu | tümü | Red | §5.3/7 |
| **G5** | Türkçe zorunlu; mojibake yasak | tümü | Red | §5.3/7 |
| **G6** | `< 480` satır reddedilir (bu dosya: asgari 480) | tümü | Red | §5.3/7 |
| **G7** | Bozuk link yasak — her `](…)` hedefi diskte var | tümü | Red | §5.3/7 |
| **G8** | `GATE-003` onayı olmadan `src/**`'a kod üretilmez | üretim | Duraklat | `WORKFLOW.md` |
| **G9** | ADR'siz mimari karar yok; kanıtsız sayı yazılmaz | tümü | Red | §1.2 |
| **G10** | Concat SQL yasak; `@p1..@pN` + `SELECT *` yasağı + indeks notu | sorgu | Red | §3.9 |
| **G11** | `TreatWarningsAsErrors=true` + **%90** coverage ihlali = red | test | Red | §3.11 |
| **G12** | İçerik tekrarı yasak (SRP) — kural kendi şablonunda | tümü | Red | §7.1 |

## §5 Workflow

### §5.1 Adımlar (entegrasyon testi için)

| # | Adım | Çıktı | Zaman |
|---|---|---|---|
| 1 | Kapsamı ayıkla (§2.1/§2.2) — birim mi, gerçek SQL mi? | doğru şablon | 2 dk |
| 2 | Senaryoyu seç (§3.8 idempotency/iptal/timeout/retry) | senaryo listesi | 4 dk |
| 3 | Kimlik 5 alanını **önce** yaz (§3.1) | Purpose/Failure kriteri | 5 dk |
| 4 | Test adını `<Sistem>_<Senaryo>_<Beklenen>` + etiketle (§3.2) | test imzası | 3 dk |
| 5 | Fixture + veri hazırlama/temizleme (§3.3, §3.6) | yeşil Arrange | 10 dk |
| 6 | AAA + ≥2 assertion (dönüş + yan etki) (§3.4) | yeşil test | 8 dk |
| 7 | Determinizm + collaborator denetimi (§3.5, §3.7) | flaky=0 | 5 dk |
| 8 | Coverage + gerekçeli dışarıda bırakma (§3.11) | %90 kanıtı | 5 dk |
| 9 | §5.3 komutlarını çalıştır → §6.1'den raporla | Quality Report | 6 dk |
| | **Toplam/test dosyası** | | **≈ 48 dk** |

### §5.2 Hata modları (≥8 — hepsi fiili)

| # | Hata | Sonuç | Önleme |
|---|---|---|---|
| E1 | Fixture **paylaşılmış** bağlantı | birbirine geçen testler (P0) | §3.3-4 tek havuz, test başına transaction |
| E2 | Seed **kayan** (zaman/ID) | flaky assertion | §3.7 sabit seed + `IClock` |
| E3 | `await` yok → **false green** | hata kaçar | §4.2-7 |
| E4 | Testler **sıra bağımlı** | tek başına kırmızı | §4.4-3 |
| E5 | Transaction **commit edilmiş** | sonraki test kirlenir | §3.6-3 rollback + Dispose |
| E6 | EF/InMemory sızdı | yanlış SQL yanılsaması | G2 + §5.3/4 |
| E7 | Kategori etiketi yok | filtrede kayıp test | §3.2 + §5.3/3 |
| E8 | N+1 gizleniyor | test yeşil, üretim yavaş | §3.9-8 + satır/sorgu sayacı |
| E9 | Retry sonsuz döngü | test **timeout**'ta patlar | §3.8-6 sabit sınır |
| E10 | Kapsam `bin/obj` hariç ölçülmemiş | kanıtsız %90 | §5.3/5 rapor satırı |

### §5.3 Salt-okunur doğrulama komutları (değiştirmez — PowerShell)

Yedi komut da depo **kaynak** dosyalarına dokunmaz (`dotnet test` yalnız `bin/obj` yazar);
`tests/` yoksa dürüstçe **"proje bulunamadı"** döner — bu hata değil, §1.2'deki boşluğun kanıtıdır.

```powershell
# 1) TEST ENVANTERİ — salt okunur keşif (proje yoksa "hedef bulunamadi" beklenir)
dotnet test C:\versacoder\tests\VersaCoder.IntegrationTests --list-tests --nologo
```

```powershell
# 2) KATEGORI FILTRESI — yalnizca entegrasyon hatti (§3.2 etiketi zorunlu)
dotnet test C:\versacoder\tests\VersaCoder.IntegrationTests --filter "Category=Integration" --nologo
```

```powershell
# 3) ETIKET EKSIKLIGI — Category=Trait gecmeyen test 0 olmali
$p = 'C:\versacoder\tests\VersaCoder.IntegrationTests'
if (-not (Test-Path $p)) { 'proje YOK -> etiketsiz_test=0 (kanit yok)' } else {
  $f = Get-ChildItem $p -Recurse -Filter *IntegrationTests.cs -EA SilentlyContinue
  $eksik = $f | Where-Object { (Get-Content $_.FullName -Raw) -notmatch 'Trait\("Category"' }
  "etiketsiz_test=$(@($eksik).Count)" }
```

```powershell
# 4) YASAK KALIP — hepsi 0 olmali (EF / SELECT * / concat SQL / Now / Random)
$p = 'C:\versacoder\tests\VersaCoder.IntegrationTests'
'EntityFramework|DbContext|DbSet<|InMemory','SELECT\s+\*','\+\s*\w+\s*\+\s*''',
'DateTime\.Now','new Random\(','Guid\.NewGuid' |
  ForEach-Object { $m = @(Select-String -Path "$p\**\*.cs" -Pattern $_ -EA SilentlyContinue)
    "{0,-32} = {1}" -f $_, $m.Count }
```

```powershell
# 5) COVERAGE — %90 (G11); rapor satiri kalici kanittir
dotnet test C:\versacoder\tests\VersaCoder.IntegrationTests `
  --collect:"XPlat Code Coverage" --results-directory C:\temp\opencode\coverage --nologo
Get-ChildItem C:\temp\opencode\coverage -Recurse -Filter coverage.cobertura.xml -EA 0 |
  Select-Object -First 1 -ExpandProperty FullName
```

```powershell
# 6) KIMLIK BLOGLU TEST — Purpose/Evidence satiri olmayan test 0 olmali
$p = 'C:\versacoder\tests\VersaCoder.IntegrationTests'
if (-not (Test-Path $p)) { 'kimliksiz_test=0 (proje yok)' } else {
  $f = Get-ChildItem $p -Recurse -Filter *Tests.cs -EA SilentlyContinue
  $bos = foreach ($x in $f) {
    $t = [System.IO.File]::ReadAllText($x.FullName, [System.Text.Encoding]::UTF8)
    [regex]::Matches($t, 'public (?:async )?Task \w+\([^)]*\)\s*\{') | ForEach-Object {
      $bas = $_.Index; $son = $t.IndexOf('    }', $bas)
      if ($son -gt 0 -and $t.Substring($bas, $son - $bas) -notmatch 'Assert\.') { $x.Name } } }
  "kimliksiz_test=$(@($bos).Count)" }
```

```powershell
# 7) BU DOSYANIN OLCUMU — satir >= 480, kb >= 27, bolum 7, mojibake 0
$enc = New-Object System.Text.UTF8Encoding($false)
$f = 'C:\versacoder\.ai\.templates\testing\integration-test-template.md'
$n = [System.IO.File]::ReadAllLines($f, $enc)
"satir=$($n.Count) kb=$([math]::Round((Get-Item $f).Length/1KB,1))"
"bolum=$(@($n | Where-Object { $_ -match '^## §' }).Count) altbolum=$(@($n | Where-Object { $_ -match '^### §' }).Count)"
"mojibake=$(@($n | Where-Object { $_ -match '[\u00C3\u00C2]|\uFFFD' }).Count) placeholder=$(@($n | Where-Object { $_ -match '\{\{[A-Z0-9_]+\}\}' }).Count)"
```

> **Not:** 4–5 numaralı komutlar **üretim** koduna değil, test projesine bakar; §3.9/§3.11
> kanıtı buradan gelir. Depoda `tests/` yoksa 1–6 dürüstçe boş döner.

### §5.4 Quality Report (zorunlu çıktı)

```markdown
## Kalite Raporu — integration-test-template (P2, read-only)
- Tarama: §5.3 komut 1..7 (yalnız okuma, 2026-10-05 HH:MM UTC)
- Test projesi: 0 (tests/ yok — §1.2) · etiketsiz test: 0 · kimliksiz test: 0 · assertionsız test: 0
- EF / SELECT * / concat / Now / Random: 0/0/0/0/0 (G2, G10, §3.7)
- Kutu 66: hatali=0 · mojibake=0 · bolum=7 · placeholder yalnız §3.10
- Kapsam: hedef %90 — ölçüm YOK (proje yok → ⚠️ VERIFICATION REQUIRED)
- Guardrail G1..G12 → ihlal yok · Red sinyali §6.2 → 0
- Sonuç: 0 P0 · 0 P1 · 0 P2 — "yapısal olarak temiz, kanıt bekliyor"
```

## §6 Doğrulama

### §6.1 Kod içi kontrol listesi (≥17 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---|---|---|
| K1 | Frontmatter **8 alan** (`title…scope`) + `authority: SSOT` ilk 9 satırda | 8/8 | G3 |
| K2 | H1 + tek satır `**Zorunlu Bağlantılar:**` (hedefler diskte) | 1 satır · 0 kırık | G7 |
| K3 | 7 bölüm tam (`§1…§7`) | 7/7 | §3.7 (envanter) |
| K4 | `§1.1–§1.4` dolu · §1.2 disk kanıtları + `⚠️` | 4/4 | §1.2 |
| K5 | Test çerçevesi **xUnit**; ek çerçeve yazılı gerekçeli | xUnit | §4.1-1 |
| K6 | `§2.1–§2.2` dolu · YAPAR/YAPMAZ ayrımı net | 2/2 | §2 |
| K7 | `§3.1–§3.11` tam (11 alt bölüm) | 11/11 | §3 |
| K8 | 5 alanlı kimlik tablosu + dolu örnek | Purpose…Evidence | §3.1 |
| K9 | İsimlendirme `<Sistem>_<Senaryo>_<Beklenen>` + `[Trait]` | kalıp | §3.2 |
| K10 | Fixture dayanağı persistence abstraction + kutu 66 | 0 uyuşmayan | §3.3 |
| K11 | AAA kutusu + ≥2 assertion kuralı | kural | §3.4 |
| K12 | Sahte/gerçek collaborator tablosu (≤2 sahte) | tablo | §3.5 |
| K13 | Veri hazırlama/temizleme + transaction sınırı 8 madde | 8 | §3.6 |
| K14 | Determinizm tablosu (`IClock`, seed) · kanıtsız iddia yok | tam | §3.7 |
| K15 | İdempotency/cancellation/timeout/retry **8** satır | 8 | §3.8 |
| K16 | ADO.NET+Dapper 8 madde · `SELECT *` yasağı | 8 + yasak | §3.9 |
| K17 | Dolu örnek + `{{…}}` kalıp bloğu | var | §3.10 |
| K18 | **%90** + `TreatWarningsAsErrors` yazılı · "neyin test edilmediği" dolu | var | §3.11 |
| K19 | `§4.1–§4.5` · §4.5 **G1..G12** 12 satır | 5 + 12 | G-serisi |
| K20 | `§5.1` adım + **Toplam** · `§5.2` hata modu **≥ 8** (E1..E10) | 10 | §5 |
| K21 | `§5.3` salt-okunur komut **≥ 5** (1..7) · `§5.4` Quality Report dolu | 7 | §5.3 |
| K22 | `§6.1` madde **≥ 17** · `§6.2–§6.5` mevcut | ≥17 | §6 |
| K23 | `§7.1` referans **≥ 6** · `§7.2` Red Team **≥ 10 satır** | ≥6 · ≥10 | §7 |
| K24 | Ürün = VERSA CODER · CoreMusic **0** · mojibake **0** | 0/0/0 | G1, G5 |

### §6.2 Red Sinyalleri (otomatik DUR)

| # | Sinyal | Neden Red | Aksiyon |
|---|---|---|---|
| R1 | Cümle başka vault'tan birebir geldi | G1 | Sil + özgün yaz |
| R2 | `§3.1` dışındaki kod içi yorum | §4.3-6 | Kimlik bloğuna taşı ya da sil |
| R3 | `EntityFramework` / `DbContext` geçen satır | G2 | ADO.NET + Dapper'e çevir |
| R4 | `SELECT *` ya da `+` ile birleşen SQL | G10 | `@p1..@pN` + kolon listesi |
| R5 | Bir testte **> 2 sahte** collaborator | §3.5 | SUT'u böl ya da birime devret |
| R6 | `DateTime.Now` / `NewGuid` / `new Random()` | §3.7 | `IClock` + sabit seed |
| R7 | `skip`/`Ignore` ile yeşil | §4.3-7 | Testi düzelt, gerekçesiz atlanma yok |
| R8 | Kategori etiketsiz test | §3.2 | `[Trait("Category","Integration")]` ekle |
| R9 | Kutu satırı ≠ 66 · mojibake · bozuk link | G5, G7 | Ölçümle hizala, UTF-8 BOM'suz yeniden yaz |
| R10 | `{{…}}` §3.10 dışında | G4 | Temizle, yeniden ölç |

### §6.3 Tür / Sürüm Geçiş Kuralları

| Değişiklik | Etki |
|---|---|
| Yalnız test metni/README düzeltmesi | `version` patch + `updated` |
| Yeni senaryo sınıfı (idempotency → retry) | §3.8 tablosu + `version` minor + §6.1 yeniden |
| Fixture dayanağı değişimi (SQLite → konteyner) | §1.2 kanıtı **önce** güncellenir, `version` minor |
| Ek test çerçevesi talebi | §4.1-1 gerekçesi + §6.1 K5 kaydı · yoksa **RED** |
| Coverage eşiği değişikliği | G11 → envanter (`../index.md`) sahibine bağlanır |

### §6.4 İlgili Dosyalar ve İş Hedefi Hizası

| Dosya | İlişki |
|---|---|
| `../testing/unit-test-template.md` | Kardeş — saf birim hattı; bu dosyanın tamamlayıcısı |
| `../testing/test-plan-template.md` | Kapsam/risk/release gate planı |
| `../backend/repository-template.md` | Sahte repo sözleşmesi (§3.5 ayrımı) |
| `../infrastructure/dapper-mapping-template.md` | Kolon↔özellik eşlemesi (§3.9) |
| `../infrastructure/migration-template.md` | Şema girdisi (§3.3-2) |
| `../query/Query-Template.md` | `@p1..@pN` + indeks notu (G10) |
| `../index.md` | Envanter + §3.7 kalite sözleşmesi (satır #42) |
| `../../../CLAUDE.md` | Katmanlar `l0..l7`, G1..G12, %90 (B5/B6) |

| # | Hedef | Evet |
|---|---|---|
| 1 | `%90` kapsam kanıta dayanır (G11) | ☐ |
| 2 | İdempotency / iptal / timeout / retry kapsandı (§3.8) | ☐ |
| 3 | Transaction sınırı rollback ile kanıtlandı (§3.6) | ☐ |
| 4 | Test **düzeltmez**, kanıt üretir | ☐ |
| 5 | Entegrasyon işi birim şablonuna **devredilmedi**, sınır korundu (§2.2) | ☐ |

### §6.5 Dış Doğrulama (diskten — beklenen değerlerle)

```powershell
$enc=New-Object System.Text.UTF8Encoding($false)
$f='C:\versacoder\.ai\.templates\testing\integration-test-template.md'
$t=[System.IO.File]::ReadAllText($f,$enc); $n=($t -split "`n")
$ne=@($n | Where-Object { $_ -match '\S' }).Count
"satir=$($n.Count) bos=$ne kb=$([math]::Round((Get-Item $f).Length/1KB,1))"
"bolum=$(@($n | Where-Object { $_ -match '^## §' }).Count) alt=$(@($n | Where-Object { $_ -match '^### §' }).Count)"
"mojibake=$(@($n | Where-Object { $_ -match '[\u00C3\u00C2]|\uFFFD' }).Count)"
$kutu = $n | Where-Object { $_ -match '^[+|]' -and $_.Length -ge 40 }
"kutu_satir=$($kutu.Count) kutu_uyusmayan=$(@($kutu | Where-Object { $_.Length -ne 66 }).Count)"
"placeholder=$(@($n | Where-Object { $_ -match '\{\{[A-Z0-9_]+\}\}' }).Count)"
$k=$n | Select-String -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches | ForEach-Object { $_.Matches } |
     ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
     Where-Object { -not (Test-Path (Join-Path (Split-Path $f) $_)) }
"kirik_link=$(@($k).Count)"
```

Beklenen: `satir` **480–660** · `kb` **≥ 27,0** · `bolum=7` · `alt=11+5+5+4+2` · `mojibake=0` ·
`kutu_uyusmayan=0` · `placeholder` **yalnız §3.10** · `kirik_link=0`.

## §7 Referanslar

### §7.1 Bağlantılar (≥6 — hepsi diskte)

| # | Kaynak | Yol | Kullanım | Kanıt |
|---|---|---|---|---|
| 1 | Şablon envanteri (SSUT) | `../index.md` | §3.7 kalite sözleşmesi · satır #42 | ✅ okundu |
| 2 | Kardeş: birim test | `../testing/unit-test-template.md` | §3.5 sahte/gerçek ayrımı | ✅ okundu |
| 3 | Kardeş: test planı | `../testing/test-plan-template.md` | §2.2 kapsam dışı devri | ✅ okundu |
| 4 | Repository sözleşmesi | `../backend/repository-template.md` | §3.5 sahte repo kalıbı | ✅ okundu |
| 5 | Dapper eşleme | `../infrastructure/dapper-mapping-template.md` | §3.9 kolon↔özellik | ✅ okundu |
| 6 | Parametreli sorgu | `../query/Query-Template.md` | G10 `@p1..@pN` kalıbı | ✅ okundu |
| 7 | Migration | `../infrastructure/migration-template.md` | §3.3-2 şema girdisi | ✅ okundu |
| 8 | Anayasa (katman + test) | `../../../CLAUDE.md` | B5 `l0..l7` · B6 G1..G12 · %90 | ✅ okundu |
| 9 | Kapı durumu | `../../../WORKFLOW.md` | §1.2-7 `GATE-005` BLOKE | ✅ okundu |
| 10 | Diyagram standardı | `../architecture/ascii-diagram-template.md` | §3.3/§3.4 kutu 66 | ✅ okundu |

### §7.2 Red Team — Sık Yapılan Hatalar (≥10 satır)

| # | İddia (SPEKÜLASYON — doğrula) | Savunma | Q |
|---|---|---|---|
| 1 | "Test yeşil, o zaman gerçek SQL de doğru" | §3.9 + §5.3/4 gerçek yol taraması | `Q: EF sızarsa? → G2 RED` |
| 2 | "Testcontainer kur, en gerçekçi o" | §1.2-4 kanıt yok + §3.3 basit dayanak | `Q: docker kanıtı? → yok, varsayma` |
| 3 | "Seed'i her test kendisi yazsın" | §3.6-2 tek `SeedEt` | `Q: kopya INSERT? → P2 §4.4-2` |
| 4 | "Hata yolunu sonra ekleriz" | §3.8 idempotency/iptal zorunlu 8 satır | `Q: iptalde yarım kayıt? → üretimde veri bozulur` |
| 5 | "Saat testte zaten sabit" | §3.7 `IClock` yasağı | `Q: flaky çıkarsa? → 2× çalıştırma farkı` |
| 6 | "Transaction her şeyi kurtarır" | §3.6-4 testin **kendi** transaction'ı | `Q: commit sızarsa? → sonraki test kirlenir` |
| 7 | "Kategori etiketi sonra eklenir" | §3.2 + §5.3/3 filtrede kayıp test | `Q: filtrede 0 test? → sahte yeşil` |
| 8 | "%90 zaten tutuyor" | §5.3/5 rapor satırı yoksa kanıtsız (G11) | `Q: ölçüm yoksa? → red` |
| 9 | "MSTest de olur" | §4.1-1 + kök `CLAUDE.md` B5 xUnit | `Q: `[TestMethod]` görünürse? → RED` |
| 10 | "CoreMusic şablonundan örnek alırız" | G1 kopya yasağı | `Q: sızarsa? → marka ihlali` |
| 11 | "`tests/` zaten var" | §1.2-1 `Test-Path = False` | `Q: yoksa? → ⚠️ kanıt beklenir` |
| 12 | "N+1'i sonra düşünürüz" | §3.9-8 + §5.2 E8 | `Q: 10k satırda kaç sorgu? → satır başına` |

**SSUT beyanı:** Bu dosya VersaCoder **entegrasyon testi** üretiminin tek kaynağıdır; birim hat
`../testing/unit-test-template.md`, kapsam `../testing/test-plan-template.md` dosyalarındadır.
Çoğaltılan her satır ikinci gerçek kaynağı doğurur (G12).
