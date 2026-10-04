---
title: "VersaCoder — Birim Test Şablonu (Unit Test)"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "birim test sablonu: arrange-act-assert akisi, assertion ve kenar durumlar"
---

# VersaCoder — Birim Test Şablonu (Unit Test)

**Zorunlu Bağlantılar:** [index](../../index.md) · [repository-template](../backend/repository-template.md) · [service-template](../backend/service-template.md) · [dapper-mapping-template](../infrastructure/dapper-mapping-template.md) · [ascii-diagram-template](../architecture/ascii-diagram-template.md)

## §1 Amaç

### §1.1 Bu şablon ne üretir?

**Tek birim test dosyası**: `tests/VersaCoder.Tests/**/*Tests.cs` altında, **saf** (I/O yok, saat yok, dosya yok,
ağ yok), **deterministik** ve **tekrarlanabilir** test metotları. Çıktı kod olduğu için bu dosya **kod iskeleti**
değil, **test sözleşmesidir**: test kimliği (What/When/Then), isimlendirme, AAA düzeni, mock stratejisi, edge case
tablosu, assertion kalitesi, **%90** kapsam hedefi ve **neyin test edilmediği** burada kilitlenir. Gerçek veritabanı
işine **bu dosya girmez** → `integration-test-template`.

### §1.2 Disk kanıtları ve Boşluk (YAGNI)

Aşağıdaki iddialar disk üzerinde **doğrulanmadı**; kanıt gelene kadar **spekülasyon** sayılır. Ölçüm: `C:\versacoder`,
2026-09-25.

```markdown
<!-- ⚠️ VERIFICATION REQUIRED: aşağıdaki her satır disk kanıtı ile değiştirilecek. -->
| İddia (SPEKÜLASYON) | Kanıt yolu | Durum |
|---|---|---|
| `tests/VersaCoder.Tests/**/*Tests.cs` klasör ağacı var | `Test-Path C:\versacoder\tests` → **False** | ⚠️ VERIFICATION REQUIRED |
| Test projesi `.csproj` ile derleniyor | depoda **0** `.csproj`/`.sln`/`.cs` dosyası | ⚠️ VERIFICATION REQUIRED |
| `dotnet test` çalışıp yeşil dönüyor | proje yok → komut şu an **çalışmaz** | ⚠️ VERIFICATION REQUIRED |
| Coverage aracı + `%90` eşiği yapılandırılmış | `.coveragerc`/`coverlet`/`Directory.Build.props` **yok** | ⚠️ VERIFICATION REQUIRED |
| `.ai/.rules/coding-standards.md` test kuralını taşır | `.ai/.rules/` = **0 dosya** (boş klasör) | ⚠️ VERIFICATION REQUIRED |
```

**Doğrulanmış gerçekler (disk ölçümü 2026-09-25):**

| # | Kanıt | Değer | Komut |
|---|---|---|---|
| 1 | Depo kökü yalnız `.ai` + `.git` | `src/` = **False**, `tests/` = **False** | `Test-Path` |
| 2 | `.NET SDK` | **10.0.401** ✅ | `dotnet --version` |
| 3 | Test yığını beyanı | **xUnit + NSubstitute + NetArchTest** | `.ai/CLAUDE.md` (okundu) |
| 4 | Coverage eşiği beyanı | **%90** + `TreatWarningsAsErrors` | `.ai/.templates/index.md` G11 |
| 5 | Çerçeve çelişkisi | `CLAUDE.md` = **xUnit** ↔ `repository-template` örneği = **`[TestMethod]`** (MSTest) | **CONFLICT → ⚠️** |
| 6 | Sürüm kilidi çelişkisi | şablonlar **26.1** ↔ `.ai/CLAUDE.md` **v26.1** | **CONFLICT → ⚠️** |
| 7 | ADR-001 / ADR-004 dosyaları | `.ai/.decisions/` = **0 dosya** | **YOK → ⚠️** |
| 8 | `.ai/.templates/` envanteri | **29 MD · 9 klasör** (envanter beyanı 23) | `Get-ChildItem -Recurse` |

**YORUM BANU:** test kodunda da yorum satırı (`//`, `///`, `/* */`) **yasaktır** — ne anlatıldığı `Assert` **mesajında**
yazılır. Dokümantasyon `§` başlıkları ve kod bloğu **dışındaki** `<!-- ... -->` kutularında yaşar.

**ÇELİŞKİ PROTOKOLÜ:** satır 5 ve 6'daki çelişkiler çözülmeden `%90` rakamı **kanıt sayılmaz** (`.ai/AGENTS.md` §3.3:
`CONFLICT DETECTED` → X sahibi `tester`/`architect`, en son `kullanıcı`).

### §1.3 Terminoloji

| Terim | Anlamı | Terim | Anlamı |
|---|---|---|---|
| Birim test | Tek birim; I/O **yok** | AAA | Arrange-Act-Assert (§3.3) |
| What | **Ne** sınanıyor (metot) | When | **Hangi koşulda** (girdi/öncül) |
| Then | **Ne bekleniyor** (assertion) | Mock | Arayüz sahtesi (§3.4) |
| Stub | Dönüşü sabitlenen sahte | Spy | Çağrıyı kaydeden sahte |
| Edge case | Sınır/boşluk/taşma durumu | Fixture | Ortak kurulum (test başına) |
| Coverage | Satır/dal kapsaması (%90) | SSUT | Tek doğruluk kaynağı |
| NetArchTest | Katman yönü testi | NSubstitute | Arayüz sahteleme kütüphanesi |

### §1.4 Karşılık gelen (veya karşılığı olmayan) şablonlar

| Aşama | Şablon | Sebep |
|---|---|---|
| Diyagram / kutu 66 standardı | `ascii-diagram-template` §3.2/§5.3 | **Tek** diyagram standardı |
| Birim test dosyası | **bu şablon** | `tests/VersaCoder.Tests/**` sahihi |
| Gerçek DB / migration testi | `integration-test-template` | SQLite kopyası, transaction, seed |
| Ne test edilecek (kapsam) | `test-plan-template` | Risk, senaryo, takvim, giriş/çıkış |
| Repo sözleşmesi testi | `backend/repository-template` §3.8 | Sahte repo + gerçek DB orada |
| Dapper eşleme testi | `infrastructure/dapper-mapping-template` | Kolon↔özellik sınırı orada |
| Mimari yön + test yığını | `.ai/CLAUDE.md` | xUnit/NSubstitute/NetArchTest |

**KAPSAM SINIRI:** Bu dosya **yalnız** `*Tests.cs` üretir. Veritabanı, migration, ortam, rapor ve takvim
→ `integration-test-template` + `test-plan-template`.

## §2 Kapsam

### §2.1 Bu şablonu kullan

| Durum | ✅ Kullan |
|---|---|
| 1 | Application/Domain'deki **saf** bir davranışın kanıtı gerekiyor |
| 2 | Yeni `XService`/`XHandler` metodu **tüm** dallarıyla (§3.5 edge tablosu) |
| 3 | Repo **arayüz sözleşmesi** sahte repo ile (dönüş tipi, `ct` zorunluluğu) |
| 4 | `%90` kapsam açığı **saf** kodda kapatılacak |
| 5 | Hata senaryosu (`null`, taşma, iptal) — §3.9 |
| 6 | Regresyon: üretimde kırılıp testte kapanan davranış |

**YAPAR — kanıt üretir:** tek birim üzerinde çalışır · **AAA** düzenini korur · her testte **en az bir anlamlı
assertion** · **gerçek** değer doğrular (§3.6) · mock'ı **arayüz üzerinden** kurar · kapsam **%90**'ı raporlar ·
**neyi test etmediğini** yazar (§3.7) · `ct` ve hata yollarını dener.

**YAPMAZ:** **EF Core / `DbContext` / `IQueryable`** (ADR-004 atıfı, dosya diskte **YOK** → ⚠️) · gerçek dosya/ağ/saat
· `versacoder.db`'ye **yazma** (→ entegrasyon) · test içinde **raw SQL** · **aşırı mock** (sahte şeytesti, §3.4) ·
**boş assertion** / always-true test (§4.5) · test kodu içine **yorum** · üretim kodunu **düzeltme** (tester yetkisi
dışı, `.ai/AGENTS.md` §3.4) · **CoreMusic** adı (ürün = **VERSA CODER / VersaCoder**).

### §2.2 Bu şablonu KULLANMA — alternatifler

| Bu dosya değil | Nerede | Ayırt edici işaret |
|---|---|---|
| Gerçek SQLite / migration / seed | `integration-test-template` | `versacoder.db` kopyası + rollback |
| Kapsam, risk, senaryo, takvim | `test-plan-template` | `.ai/reports/test-plan-*.md` |
| Repository implementasyon davranışı | `backend/repository-template` §3.8 | Sahte repo örneği orada |
| Mapping/kolon eşleme | `infrastructure/dapper-mapping-template` | `SqlMapper` ayarları |
| Katman yönü / döngüsel bağımlılık | `.ai/CLAUDE.md` (NetArchTest) | Sınır testi, davranış değil |
| Form olay akışı doğrulaması | `frontend/form-template` §3.7 | UI olayı, birim değil |
| CLI çıkış kodu doğrulaması | `other/cli-command-template` §5.3 | `dotnet run -- …` |

## §3 Mimari

### §3.1 Test kimliği — What / When / Then

Her test **üç parçalı** bir kimlik taşır; test adının **üç alt-satırı** bunu karşılar:

| Parça | Soru | Nereye yazılır | Örnek |
|---|---|---|---|
| **What** | Ne sınanıyor? | Test adının **1. parçası** | `GetPagedAsync` |
| **When** | Hangi koşulda? | Test adının **2. parçası** | `fk_sayfa_boyutu_5_iken` |
| **Then** | Ne bekleniyor? | Test adının **3. parçası** + assert mesajı | `ilk_sayfa_5_kayit_doner` |

1. **What tekil** olur — bir test **tek** davranışı sınar (SRP).
2. **When** yazılabilir koşul (`..._varsa`, `..._yokken`, `..._bos_iken`); koşul **gizlenmez**.
3. **Then** fiilimsiyle biter (`..._doner`, `..._firlatir`, `..._true_doner`).
4. Üç parça **alt çizgi** ile ayrılır; boşluk/Türkçe büyük harf **yasak** (§3.2).
5. `Assert` **mesajı** da Then'i tekrarlar — hata çıktısı tek başına okunur olmalı.

### §3.2 Test isimlendirme kuralı

**Biçim:** `MetotAdi_Kosul_BeklenenSonuc` — ASCII, küçük harf (Türkçe gövde serbest), alt çizgi ayraç, **≤ 96 karakter**.

| ✅ Geçerli | ❌ Geçersiz | Neden |
|---|---|---|
| `GetPagedAsync_ilk_sayfa_5_kayit_doner` | `Test1` | Anlamsız (R1) |
| `AddAsync_ayni_kod_ikinci_kez_false_doner` | `GetPagedAsync test` | Boşluk + noktalama |
| `GetByIdAsync_kayit_yokken_null_doner` | `GetByIdAsync_KayitYokken` | Then yok |
| `DeleteAsync_yanlis_id_ile_cagrilirsa_false_doner` | `Test_GetByIdAsync` | When/Then yok |
| `Ctor_bos_parametre_ile_ArgumentNullException_firlatir` | `metot…çalışıyor` | `…` okunmaz |
| `Bos_metin_girersek_validasyon_hatası_doner` | `Metot_x_1` | Sayısal etiket yasak |

- **Sınıf adı:** `{SınıfAdı}Tests` (`ProyectoServiceTests.cs`) — **tek** test sınıfı = **tek** üretim sınıfı.
- **Dosya adı:** `{SınıfAdı}Tests.cs` — sınıf ile birebir eşleşir (§5.3-2 taraması).
- **Yasak ön ekler:** `Test_`, `Should_`, `Given_When_` — depo standardı **alt çizgi üçlüsüdür** (§3.1).
- **Nasıl test edildiğini değil, ne olduğunu** yaz: `Should_Return_True_When_Valid` → RED.

### §3.3 Arrange-Act-Assert ve katman diyagramı

```text
+----------------------------------------------------------------+
| ► BİRİM TEST HATTI — saf · deterministik · I/O yok             |
|   [1] ARRANGE  sahte arayüz + girdi kur (NSubstitute)          |
|          v                                                     |
|   [2] ACT      tek çağrı — tek davranış (saf, await ct)        |
|          v                                                     |
|   [3] ASSERT   gerçek değer + anlamlı mesaj (§3.6)             |
|          v                                                     |
|   [4] KANIT    dotnet test → yeşil · kapsam %90 (§3.7)         |
|   x[] EF Core / SQL / dosya / saat / ağ  ->  RED (ADR-004)     |
+----------------------------------------------------------------+
```

**Kutu kuralı:** `ascii-diagram-template` §3.2 primitifleri; **tek HD** = bu dosya; sabit çizgi **66 karakter**
(64 iç + iki `|`). **İpucu:** her satır 66'ya hizalanmazsa §5.3-6 ölçer, §6.5 düzeltir.

1. **Arrange** bloğu test **başında** biter; `new` ve sahte kurulum orada kalır.
2. **Act** bloğunda **tek** eylem vardır (`await ...`); ikinci çağrı varsa test **iki** teste bölünür.
3. **Assert** bloğu **en az 2** anlamlı doğrulama ister (dönüş + yan etki); tek assertion **1/2 puan** (§3.6).
4. `try/catch` ile assertion **saklanmaz**; beklenen hata `[Assert.ThrowsAsync]` ile **adlandırılır**.
5. **Test başına tek** mantıksal iddia — “ve” ile başlayan assertion mesajı **RED** sinyalidir.

### §3.4 Mock stratejisi — arayüz üzerinden, aşırı mock YASAK

| Kural | ✅ Yapılır | ❌ Yapılmaz |
|---|---|---|
| Ne sahtelenir | **`I{Şey}` arayüzü** (DI ile gelen) | Somut sınıf, `static`, extension |
| Kütüphane | **NSubstitute** (`.ai/CLAUDE.md` beyanı) | Elle yazılmış `FakeXxx` (sahte repo hariç, §3.8) |
| Ne sahtelenmez | — | `string`, `int`, DTO, enum, kolleksiyon |
| Doğrulama | Dönüş değeri + **gerektiğinde** çağrı sayısı | `Received(1).X()` her yerde |
| Kurulum | Test **içinde**, tek Arrange bloğu | Sınıf seviyesinde paylaşılan sahte |
| EF | **YASAK** (ADR-004 atıfı) | `DbSet` sahtesi, `InMemory` |

**Aşırı mock testi (RED):** sahte sayısının **üretim bağımlılığından** fazla olduğu test iptal edilir. Bir testte
**> 2 sahte arayüz** varsa önce **tasarım** sorusu sorulur (bağımlılık fazlalığı). `VerifyAll` ile **her şeyi**
doğrulayan test, **hiçbir şeyi** doğrulamaz.

**Sahte repo istisnası:** veri erişimi sözleşmesi (dönüş tipi, sayfalama, `false` dönüşü) **sahte repo** ile
kurulur — `backend/repository-template` §3.8 ile **aynı** kalıp; sahte repo **bu şablonun içinde** yaşar.

### §3.5 Edge case tablosu (her birim için zorunlu gözden geçirme)

| # | Sınıf | Durum | Beklenen | Test adı parçası |
|---|---|---|---|---|
| 1 | Sınır | `0`, `1`, `max`, `max+1` | taşma/hata **adlandırılır** | `sinirda` |
| 2 | Boşluk | `null`, boş `string`, boş liste | Domain Failure / `false` | `bos_iken` |
| 3 | Geçersiz | negatif, harf, Türkçe karakter | validasyon hatası | `gecersiz_ile` |
| 4 | Yinelenen | aynı `codigo` 2. kez | `false` / çakışma | `ayni_kod_ile` |
| 5 | Yok | var olmayan `id` | `null` yerine **failure** | `kayit_yokken` |
| 6 | İptal | `ct` önceden iptal | `OperationCanceledException` | `iptal_edilmis_ct_ile` |
| 7 | Eşzamanlık | ardışık 2 çağrı | deterministik sıra | `art_ardisik` |
| 8 | Sıfır | sayfa `0`, boyut `0` | 1. sayfaya normalize | `sifir_degerle` |
| 9 | Uzun | 10k kayıtlık girdi | N+1 yok, sabit süre | `buyuk_girdi_ile` |
| 10 | Karakter | `ç ğ ı İ ö ş ü` metni | korunur, **mojibake yok** | `turekce_metin_ile` |

### §3.6 Assertion kalitesi — dolu, gerçek doğrulama

| Seviye | Davranış | Karar |
|---|---|---|
| **0** | `Assert.True(true)` / assertion yok | **RED** — test bir şeyi **kanıtlamaz** |
| **1** | `Assert.NotNull(x)` tek başına | **P2** — varlık ≠ doğru davranış |
| **2** | `Assert.Equal(beklenen, gercek)` + mesaj | ✅ **kabul** |
| **3** | 2 + yan etki doğrulaması (çağrı/kayıt) | ✅ **tercih** |

1. **Her assertion mesajı Türkçe** ve **Then'i** tekrarlar: `"İlk sayfa 5 kayıt"`.
2. **Sabit `true` yok**: beklenti hesaplanır (`expected: boyut * (sayfa - 1)`).
3. **Koleksiyon** için `Assert.Equal(sayi, liste.Count)` **+** örnek eleman doğrulaması.
4. **Hata** için `await Assert.ThrowsAsync<T>(...)` + mesajı **içerik** doğrulaması.
5. **Floating/para** için tolerans (`InRange`) — birebir eşitlik RED sinyalidir.
6. **Sıra** doğrulanacaksa `ORDER BY` sabitlenmeli (repository §4.4-7); testte `Should()` **yok**.

### §3.7 Coverage hedefi %90 ve neyin test edilmediği

| Metrik | Hedef | Aşılırsa | Eksik kalırsa |
|---|---|---|---|
| Satır kapsamı | **%90** | — | **RED** (`.ai/.templates/index.md` G11) |
| Kapsamsız saf dal | **0** | — | P2 |
| Test başına assert | **≥ 1** (hedef 2) | — | P1 |
| `async void` / boş `catch` | **0** | — | P0 (§4.5) |

**Neyin test edilmediği (zorunlu liste — her PR'da yazılır):**

| Kapsam dışı | Gerekçe | Nerede kanıtlanır |
|---|---|---|
| WinForms `InitializeComponent` / `.Designer.cs` | Tasarım-süresi kodu | `frontend/form-template` §5.3 |
| Gerçek SQLite SQL yolu | DB kurulumu + şema gerekir | `integration-test-template` |
| DevExpress çizim/pencere | UI bağlamı, saf değil | manuel + entegrasyon |
| Üçüncü taraf kütüphane içi | Onarılabilirlik yok | — |
| `Directory.Packages.props` sürüm eşleşmesi | Paket yönetimi | `dotnet list package` (§5.3) |
| Okuma **yolu** (getter) | Satır kapsamını **şişirir** | kapsam raporu notu |

> **Kural:** kapsam dışı her madde **gerekçesiyle** yazılır; gerekçesiz kapsam dışı = **P1**.

### §3.8 Dapper / ADO.NET repository testleri — sahte repo vs gerçek DB

| Yaklaşım | Nerede | Ne sınar | Ne sınamaz |
|---|---|---|---|
| **Sahte repo** (tercih) | `VersaCoder.Tests` (bu dosya) | Servis davranışı, dönüş tipi, `false`/failure | SQL doğru mu, mapping doğru mu |
| **Sahte arayüz** | `VersaCoder.Tests` | `I{X}Repository` sözleşmesi, `ct` | Kolon↔özellik eşlemesi |
| **Gerçek DB kopyası** | `VersaCoder.IntegrationTests` | `@p1..@pN`, `LIMIT/OFFSET`, N+1 | İş kuralı dalları |
| **Mock` DbSet` / EF** | **YOK** | — | **RED, ADR-004** |

1. Sahte repo **yalnız** arayüzü uygular; SQL metni **taşımaz** (repository §1.1 sınırı).
2. Sahte repo'nun `GetPagedAsync`'i **gerçek** `PagedResult` tipini döndürür — sahte DTO **yasak**.
3. Repo'nun **kendi** testi (`*RepositoryTests`) entegrasyon projesine gider; birimde **yalnız** sözleşme testi.
4. `SqlMapper`/mapping davranışı → `dapper-mapping-template` (bu dosyanın **dışında**).
5. `IDbConnection` **açılmaz** — bağlantı **hiçbir** birim testte görünmemeli.

### §3.9 Hata senaryoları + dolu VersaCoder örneği + tam kalıp

**Hata senaryoları (her birim için en az biri zorunlu):**

| # | Senaryo | Beklenen | Assertion |
|---|---|---|---|
| H1 | `null` / boş girdi | Domain Failure (`Result`) | `Assert.False(r.Basarili)` + kod |
| H2 | Var olmayan kayıt | `null` **DEĞİL**, tip-güvenli hata | `Assert.NotNull(failure)` |
| H3 | Yinelenen `codigo` | `false` / `Conflict` | `Assert.Equal("KOD_001", h.Kod)` |
| H4 | `ct` iptal edilmiş | `OperationCanceledException` | `ThrowsAsync` |
| H5 | Dış bağımlılık hata fırlatır | Yükseltilir, **swallow yok** | `ThrowsAsync` + kaynak |
| H6 | Sınır taşması | `ArgumentOutOfRange` / failure | `ThrowsAsync<T>` |

**Dolu VersaCoder örneği (xUnit + NSubstitute; `tests/VersaCoder.Tests/ProyectoServiceTests.cs`):**

```csharp
namespace VersaCoder.Tests.Application;

public sealed class ProyectoServiceTests
{
    [Fact]
    public async Task GetResumenAsync_kayit_varsa_ozet_doner()
    {
        var repo = Substitute.For<IProyectoRepository>();
        repo.GetResumenAsync(7, Arg.Any<CancellationToken>())
            .Returns(new ProyectoResumenDto(7, "ANAHTAR", 12));

        var svc = new ProyectoService(repo);

        var ozet = await svc.GetResumenAsync(7, CancellationToken.None);

        Assert.Equal(7, ozet.Id, "Özet doğru kayda ait");
        Assert.Equal(12, ozet.GorevSayisi, "Özet 12 görev sayar");
    }

    [Fact]
    public async Task GetResumenAsync_kayit_yokken_failure_doner()
    {
        var repo = Substitute.For<IProyectoRepository>();
        repo.GetResumenAsync(999, Arg.Any<CancellationToken>())
            .Returns((ProyectoResumenDto?)null);

        var svc = new ProyectoService(repo);

        var sonuc = await svc.GetResumenAsync(999, CancellationToken.None);

        Assert.False(sonuc.Basarili, "Kayıt yokken başarı=false");
        Assert.Equal("PRJ_404", sonuc.HataKodu, "Doğru hata kodu döner");
    }
}
```

**Tam kalıp (yalnız `{{…}}` doldurulur; üretim dosyasında bu blok bulunmaz):**

```csharp
namespace VersaCoder.Tests.{{KATMAN}};

public sealed class {{SINIF}}Tests
{
    [Fact]
    public async Task {{METOT}}_{{KOSUL}}_{{BEKLENEN}}()
    {
        var bagimli = Substitute.For<{{ARAYUZ}}>();
        bagimli.{{YONTEM}}({{GIRINTI}}, Arg.Any<CancellationToken>()).Returns({{BEKLENEN_DONUS}});

        var sut = new {{SUT}}(bagimli);

        var sonuc = await sut.{{METOT}}({{GIRINTI}}, CancellationToken.None);

        Assert.Equal({{BEKLENEN}}, sonuc.{{ALAN}}, "{{SONUC_CUMLESI}}");
        Assert.True(sonuc.Basarili, "{{BASARI_CUMLESI}}");
    }
}
```

## §4 Kurallar

### §4.1 Bu depoda

1. **xUnit** (`[Fact]`/`[Theory]`) + **NSubstitute** — çerçeve `.ai/CLAUDE.md` ile sabit; **MSTest kalıntısı RED**.
2. Her test **saf**: dosya, ağ, saat, rastgelelik **yok** (determinism).
3. Test adı **`Metot_Kosul_Beklenen`** (§3.2); `Test1`/`Should_` **RED**.
4. Her testte **≥ 1 anlamlı assertion**; mesaj **Türkçe** (§3.6).
5. **%90** satır kapsamı **zorunlu** (G11); kapsam dışı **gerekçeli** (§3.7).
6. `async` testlerde `Task` + `CancellationToken.None` açıkça yazılır.
7. Test sınıfı adı **üretim sınıfıyla** birebir (`{X}Tests`).
8. Kapsam dışı her durum §2.2'ye yönlendirilir.

### §4.2 Kompozisyon

1. Mock **yalnız arayüz üzerinden**; somut/`static` sahte **RED** (§3.4).
2. **Aşırı mock yok**: test başına **≤ 2** sahte arayüz; fazlası tasarım sorusudur.
3. AAA **blokları** ayrı ayrı; `Arrange` içinde `Act` **RED**.
4. Beklenen hata **`ThrowsAsync`** ile adlandırılır; `try/catch` **RED**.
5. Paylaşılan kurulum `IDisposable` **fixture** ile — statik global durum **RED**.
6. Test verisi **tek yerde** (私 helper değil, `TestData` sınıfı) — **kopyala-yapıştır yok**.
7. Üretim kodu **düzeltilemez** (`.ai/AGENTS.md` §3.4): test kırmızıysa kök neden → `developer`.
8. Kod içi yorum **eklenmez** (§1.2 YORUM BANU); açıklama assert mesajındadır.

### §4.3 Yasaklar (RED)

1. **EF Core / `DbContext` / `DbSet` / `IQueryable` / `InMemory`** — **RED** (ADR-004 atıfı; dosya **YOK** → ⚠️).
2. **Boş assertion / always-true test** (`Assert.True(true)`, assertion'sız test) — **RED**.
3. **`async void`** test metodu — **RED**; **boş `catch { }`** — **RED**.
4. **Test içinde raw SQL** veya `versacoder.db`'ye **yazma** — **RED** (entegrasyon).
5. **Mock somut sınıf** / **aşırı mock** (> 2 sahte) — **RED**.
6. **Kod içi yorum** (`//`, `///`) — **RED** (§1.2).
7. **Determinizm kırıcı**: `DateTime.Now`, `Guid.NewGuid`, `Random` **sebepsiz** — **RED**.
8. **CoreMusic** adı — **RED**; ürün **VERSA CODER / VersaCoder**.

### §4.4 Guardrail serisi (birim test için genişletilmiş)

1. Test **tek** davranışı sınar; “ve” ile başlayan assert mesajı → **böl** (P1).
2. **Kapsam şişirme**: yalnız `get` okuyan satırlar kapsam **yüzdesine** sayılmaz (§3.7 notu).
3. `await` edilmeyen `Task` → **false green**; her async çağrıyı `await` et (P0).
4. Sahte ile **gerçek** davranış farkı → entegrasyona devret (§3.8).
5. **Yinelenen test** (aynı Arrange iki dosyada) → `TestData`'ya taşı (P2).
6. Türkçe karakter `ç ğ ı İ ö ş ü` testi **var** (§3.5-10); mojibake **0**.
7. Testin **bağımsızlığı**: tek test tek başına da yeşil olmalı (§5.2 E7).

### §4.5 Hard Guardrails (≥10 — otomatik DUR)

| ID | Guardrail | Seviye | Tespit |
|---|---|---|---|
| **U1** | Assertion **yok** / `Assert.True(true)` | **P0** | §5.3-3 |
| **U2** | Always-true / kendini kanıtlayan test | **P0** | §6.1 |
| **U3** | Mock gerçek yolu **örtbas** ediyor (sahte = tüm yol) | **P1** | §3.4/§7.2 |
| **U4** | **EF Core / `DbSet`** testte | **P0** | §5.3-5 |
| **U5** | Test adı kuralı dışı (`Test1`, boşluk) | **P1** | §5.3-2 |
| **U6** | Test adı ≠ sınıf adı eşleşmesi | **P1** | §5.3-2 |
| **U7** | **`async void`** / `await` edilmemiş `Task` | **P0** | §5.3-4 |
| **U8** | Boş `catch { }` / exception **swallow** | **P0** | §5.3-4 |
| **U9** | Kapsam **< %90** | **P1** (G11 → RED) | kapsam raporu |
| **U10** | **Determinizm** kırıcı (`Now`/`NewGuid`) | **P1** | §5.3-4 |
| **U11** | Testte **dosya/ağ/saat** I/O | **P1** | §6.1 |
| **U12** | **MSTest kalıntısı** (`[TestMethod]`, `Assert.AreEqual`) | **P1** | §5.3-5 |
| **U13** | Test başına sahte **> 2** | **P2** | §3.4 |
| **U14** | Kapsam dışı **gerekçesiz** · CoreMusic markası | **P2** | §3.7/§6.1 |

## §5 Workflow

### §5.1 Adımlar (birim test için)

| # | Adım | Çıktı | Zaman |
|---|---|---|---|
| 1 | Kapsamı ayıkla (§2.1/§2.2) — saf mı, DB mi? | doğru şablon | 2 dk |
| 2 | Birim + edge tablosunu seç (§3.5) | senaryo listesi | 4 dk |
| 3 | Test adını **üçlü** yaz (§3.1/§3.2) — önce RED | test imzası | 3 dk |
| 4 | AAA ile doldur; mock **arayüz** (§3.4) | yeşil test | 8 dk |
| 5 | Assertion'ları **gerçek** değere çek (§3.6) | ≥ 2 assertion | 5 dk |
| 6 | Kapsam + gerekçeli dışarıda bırakma (§3.7) | %90 kanıtı | 5 dk |
| 7 | §5.3 komutlarını çalıştır → §6.1'den raporla | Quality Report | 6 dk |
| | **Toplam/test dosyası** | | **≈ 33 dk** |

### §5.2 Hata modları (≥8 — hepsi fiili)

| # | Hata | Sonuç | Önleme |
|---|---|---|---|
| E1 | Assertion yok / `True(true)` | Sahte yeşil (P0) | U1 + §3.6 |
| E2 | Mock **gerçek yolu** örtbas | Üretimde kırılan test dışı davranış | §3.8 gerçek DB |
| E3 | `await` yok → **false green** | Test erken biter, hata kaçar | U7, `async Task` |
| E4 | Test birbirine bağlı (paylaşılan durum) | Sıra değişince kırmızı | fixture + yalıtım |
| E5 | `DateTime.Now` / `Random` | Kararsız test (flaky) | U10 sabit girdi |
| E6 | MSTest + xUnit karışığı | Derleme hatası ya da sessiz atlanma | U12 + §5.3-5 |
| E7 | EF sahtesi (`InMemory`) | ADR-004 ihlali, yanlış SQL yanılsaması | U4 + §5.3-5 |
| E8 | Test adı != dosya adı | Filtre çalıştırılamaz, envanter kayar | U6 + §5.3-2 |
| E9 | Kapsam şişiren saf getter'lar | %95 görünür, kritik dal açık | §3.7 gerekçe |

### §5.3 Salt-okunur doğrulama komutları (değiştirmez — PowerShell)

| # | Komut | Ne doğrular | ID |
|---|---|---|---|
| 1 | `dotnet test --list-tests` | Test **envanteri** + keşif | U6 |
| 2 | `Select-String` test adı taraması | İsimlendirme ihlali = **0** | U5/U6 |
| 3 | `Select-String` assertion taraması | Assertionsız test = **0** | U1/U2 |
| 4 | `Select-String` yasak kalıp | `async void`/boş catch/`Now` = **0** | U7/U8/U10 |
| 5 | `Select-String` EF + MSTest kalıntısı | **0** | U4/U12 |
| 6 | Kutu 66 + mojibake ölçümü | `hatali=0` · `mojibake=0` | §6.5 |

```powershell
# 1) TEST ENVANTERİ — salt okunur keşif (proje yoksa "proje bulunamadi" beklenir)
dotnet test C:\versacoder\tests\VersaCoder.Tests --list-tests --nologo
```

```powershell
# 2) TEST ADI TARAMASI — ihlal listesi bos olmali (Test1 / bosluk / Should_)
Select-String -Path C:\versacoder\tests\VersaCoder.Tests\**\*Tests.cs -Encoding UTF8 `
  -Pattern 'public (void|async Task) (Test\w*|Should_\w*|\w*\s\w*)\(' |
  ForEach-Object { "{0}:{1}" -f $_.Filename, $_.LineNumber }
```

```powershell
# 3) ASSERTION TARAMASI — "Assert." gecmeyen test metodu 0 olmali
$f = Get-ChildItem C:\versacoder\tests\VersaCoder.Tests -Recurse -Filter *Tests.cs -ErrorAction SilentlyContinue
$bos = foreach ($x in $f) {
  $t = [System.IO.File]::ReadAllText($x.FullName, [System.Text.Encoding]::UTF8)
  [regex]::Matches($t, 'public (?:async )?(?:void|Task) \w+\([^)]*\)\s*\{') | ForEach-Object {
    $bas = $_.Index; $son = $t.IndexOf('    }', $bas)
    if ($son -gt 0 -and $t.Substring($bas, $son - $bas) -notmatch 'Assert\.') { $x.Name }
  }
}
"assertionsiz_test=$(@($bos).Count)"
```

```powershell
# 4) YASAK KALIP — hepsi 0 olmali (async void / bos catch / DateTime.Now / Random)
$p = 'C:\versacoder\tests\VersaCoder.Tests'
'async\s+void','catch\s*\{\s*\}','DateTime\.Now','new Random\(','await\s+\w+\([^)]*\)\s*;' |
  ForEach-Object { $m = @(Select-String -Path "$p\**\*.cs" -Pattern $_ -ErrorAction SilentlyContinue)
    "{0,-28} = {1}" -f $_, $m.Count }
```

```powershell
# 5) EF + MSTEST KALINTISI — 0/0 olmali (ADR-004 / cerceve celiskisi)
$p = 'C:\versacoder\tests\VersaCoder.Tests'
"ef      = " + @(Select-String -Path "$p\**\*.cs" -Pattern 'EntityFramework|DbContext|DbSet<|InMemory' ).Count
"mstest  = " + @(Select-String -Path "$p\**\*.cs" -Pattern '\[TestMethod\]|\[TestClass\]|Assert\.AreEqual' ).Count
"report  = " + @(Select-String -Path "$p\**\*.cs" -Pattern 'CoreMusic' ).Count
```

```powershell
# 6) BOSLUK-KUTU + MOJIBAKE OLCEMI — hatali=0, mojibake=0 (bu dosya)
$enc = New-Object System.Text.UTF8Encoding($false)
$f = 'C:\versacoder\.ai\.templates\testing\unit-test-template.md'
$n = [System.IO.File]::ReadAllLines($f, $enc)
$in = $false; $hatali = 0
foreach ($l in $n) {
  if ($l -match '^```text') { $in = $true; continue }
  if ($l -match '^```' -and $in) { $in = $false; continue }
  if ($in -and $l -match '^[+|]' -and $l.Length -ne 66) { $hatali++ }
}
"kutu_hatali=$hatali mojibake=$(@($n | Where-Object { $_ -match '[\u00C3\u00C2]|\uFFFD' }).Count)"
```

> **P2 kapsamı:** yalnız okuma; dosya/yazma **yok**. Depoda `tests/` yoksa 1–5 **“proje/proje bulunamadi”** verir;
> bu **hata değil**, §1.2'deki boşluğun kanıtıdır.

### §5.4 Quality Report (zorunlu çıktı)

```markdown
## Kalite Raporu - unit-test-template (P2, read-only)
- Tarama: §5.3 komut 1..6 (yalnız okuma, 2026-09-25 HH:MM UTC)
- Test dosyası: 0 (tests/ yok — §1.2) · test adı ihlali: 0 · assertionsız test: 0
- async void / bos catch / Now / Random: 0/0/0/0 (U7/U8/U10)
- EF kalıntısı: 0 · MSTest kalıntısı: 0 · CoreMusic: 0
- Kutu 66: hatali=0 · mojibake=0 · bolum=7
- Kapsam: hedef %90 — ölçüm YOK (proje yok → ⚠️ VERIFICATION REQUIRED)
- Sonuç: 0 P0 · 0 P1 · 0 P2 — "yapısal olarak temiz, kanıt bekliyor"
```

## §6 Doğrulama

### §6.1 Kod içi kontrol listesi (≥20)

- [ ] Frontmatter 7 alan + `authority: SSOT` (ilk 9 satır)
- [ ] H1 + `**Zorunlu Bağlantılar:**` 5 bağlantı
- [ ] 7 bölüm tam (`§1…§7`) · alt bölüm `§1.1–§1.4`, `§2.1–§2.2`
- [ ] `§3.1–§3.9` tam · §3.9 dolu örnek + `{{…}}` kalıp bloğu
- [ ] `§4.1–§4.5` · §4.5 guardrail **≥ 10** madde (U1..U14)
- [ ] `§5.1` adım + **Toplam** · `§5.2` hata modu **≥ 8** (E1..E9) · `§5.4` Quality Report dolu
- [ ] `§5.3` salt-okunur komut **≥ 5** (1..6)
- [ ] `§6.1` madde **≥ 20**
- [ ] `§6.2–§6.5` mevcut · §6.5 ölçüm PowerShell'i beklenen değerlerle
- [ ] `§7.1` referans **≥ 6** · `§7.2` Red Team **≥ 10 satır**
- [ ] Test adı biçimi `Metot_Kosul_Beklenen` (§3.2)
- [ ] Her testte What/When/Then izlenebilir (§3.1)
- [ ] AAA blokları ayrı; Arrange'de Act yok (§3.3)
- [ ] Mock **yalnız arayüz**; somut sahte yok (§3.4) · test başına sahte **≤ 2** (U13)
- [ ] Assertion **gerçek** değer + Türkçe mesaj (§3.6); boş assertion **0** (U1/U2)
- [ ] Edge tablosu **10** satırı gözden geçirildi (§3.5) · H1..H6'dan biri kapsandı (§3.9)
- [ ] Kapsam hedefi **%90** yazılı; **neyin test edilmediği** listesi dolu (§3.7)
- [ ] Sahte repo / gerçek DB ayrımı net (§3.8)
- [ ] EF Core testte **0** (U4, ADR-004) · MSTest kalıntısı **0** (U12, §1.2-5)
- [ ] Determinizm kırıcı **0** (U10) · test kodu içi yorum **0** (§1.2)
- [ ] `{{…}}` yalnız §3.9 kalıp bloğunda
- [ ] ⚠️ VERIFICATION REQUIRED ibaresi §1.2'de mevcut
- [ ] Kutu satırları **66** (§3.3/§5.3-6)
- [ ] Ürün = VERSA CODER; CoreMusic **0** · Bağlantı hedefleri diskte var (§7.1)

### §6.2 Mimari tutarlılık

| # | Soru | Evet |
|---|---|---|
| 1 | Test yalnız **saf** birimde mı (I/O yok)? | ☐ |
| 2 | Mock arayüz üzerinden mi (§3.4)? | ☐ |
| 3 | Repo sözleşmesi sahte ile mi (§3.8)? | ☐ |
| 4 | DB/mapping testi bu dosyada değil mi (§2.2)? | ☐ |
| 5 | Test sınıfı = üretim sınıfı adı mı (§3.2)? | ☐ |
| 6 | Üretim koduna dokunuldu mu? (**HAYIR**, §4.2-7) | ☐ |

### §6.3 Derleme ve testler

| # | Kontrol | Beklenen | Evet |
|---|---|---|---|
| 1 | `dotnet build` | **0 hata** | ☐ |
| 2 | `dotnet test --list-tests` | test keşfi çalışır | ☐ |
| 3 | `dotnet test --filter "FullyQualifiedName~Tests"` | **hepsi yeşil** | ☐ |
| 4 | Satır kapsamı | **≥ %90** | ☐ |
| 5 | Assertionsız test | **0** | ☐ |
| 6 | `async void` / boş `catch` | **0** | ☐ |
| 7 | EF + MSTest kalıntısı | **0 / 0** | ☐ |

### §6.4 İş hedefi hizası

| # | Hedef | Evet |
|---|---|---|
| 1 | **%90** kapsam kanıta dayanır (G11) | ☐ |
| 2 | Hata yolları H1..H6 ile kapsandı | ☐ |
| 3 | İki bacak (`net48` / `net10.0-windows`) testte de | ☐ |
| 4 | Test **düzeltmez**, kanıt üretir (§4.2-7) | ☐ |
| 5 | Entegrasyon işi kardeş şablona devredildi · kapsam dışı gerekçesi raporda | ☐ |

### §6.5 Dış doğrulama (diskten)

```powershell
$enc=New-Object System.Text.UTF8Encoding($false)
$f='C:\versacoder\.ai\.templates\testing\unit-test-template.md'
$t=[System.IO.File]::ReadAllText($f,$enc); $n=($t -split "`n")
$ne=@($n | Where-Object { $_ -match '\S' }).Count
"satir=$($n.Count) bos=$ne kb=$([math]::Round((Get-Item $f).Length/1KB,1))"
"bolum=$(@($n | Where-Object { $_ -match '^## §' }).Count)"
"mojibake=$(@($n | Where-Object { $_ -match '[\u00C3\u00C2]|\uFFFD' }).Count)"
"kutu_hatali=$(@($n | Where-Object { $_ -match '^[+|]' -and $_.Length -ne 66 }).Count)"
"placeholder=$(@($n | Where-Object { $_ -match '\{\{[A-Z0-9_]+\}\}' }).Count)"
```

Beklenen: `satir` 480–660 · `kb` 25–34 · `bolum=7` · `mojibake=0` · `kutu_hatali=0` · `placeholder` yalnız §3.9.

## §7 Referanslar

### §7.1 Bağlantılar (≥6)

| # | Kaynak | Yol | Kullanım | Kanıt |
|---|---|---|---|---|
| 1 | Şablon envanteri (SSUT) | `.ai/.templates/index.md` | §3.7 kapsam sözleşmesi (G11) | ✅ okundu |
| 2 | Repository şablonu (kardeş) | `.ai/.templates/backend/repository-template.md` | §3.8 sahte repo örneği | ✅ okundu |
| 3 | ASCII diyagram (kutu 66) | `.ai/.templates/architecture/ascii-diagram-template.md` | §3.3 kutu kuralı | ✅ okundu |
| 4 | Anayasa (test yığını) | `.ai/CLAUDE.md` | xUnit + NSubstitute + NetArchTest | ✅ okundu |
| 5 | Ajan protokolü (yetki) | `.ai/AGENTS.md` §3.4 | Testler üretim kodunu **düzeltmez** | ✅ okundu |
| 6 | ADR-004 (EF yasağı) | `.ai/.decisions/accepted/ADR-004-*.md` | **DOSYA YOK** | ⚠️ VERIFICATION REQUIRED |
| 7 | Kardeş: entegrasyon | `.ai/.templates/testing/integration-test-template.md` | DB işi oraya devredilir | bu part |

### §7.2 Red Team — Sık Yapılan Hatalar (≥10 satır)

| # | iddia (SPEKÜLASYON — doğrula) | Savunma | Q |
|---|---|---|---|
| 1 | “Test yeşil, o zaman doğru” | U1/U2 + §3.6 seviye tablosu | `Q: Assertion yoksa? → P0 sahte yeşil` |
| 2 | “Mock her şeyi çözüyor” | U3 + §3.8 gerçek DB | `Q: Mock gerçek yolu örtbas mı? → entegrasyon` |
| 3 | “EF ile test verisi kolay” | U4, ADR-004 (dosya **YOK** → ⚠️) | `Q: DbSet sahtesi? → P0` |
| 4 | “MSTest de olur” | U12 + §1.2-5 çelişki kaydı | `Q: [TestMethod] görünürse? → RED` |
| 5 | “Test adı önemsiz” | U5/U6 + §3.2 | `Q: Test1 yazılırsa? → filtre kırılır` |
| 6 | “Hata yolunu sonra yazarız” | §3.9 H1..H6 | `Q: null girdi gelirse? → üretimde patlar` |
| 7 | “Determinizm zaten sağlanıyor” | U10 `Now`/`Random` | `Q: Flaky çıkarsa? → 2× çalıştırma farkı` |
| 8 | “`tests/` klasörü zaten var” | §1.2-1 `Test-Path = False` | `Q: Yoksa? → ⚠️, kanıt beklenir` |
| 9 | “CoreMusic’ten örnek alabiliriz” | G1 (kopya yasağı) + U14 | `Q: Sızarsa? → marka ihlali` |
| 10 | “%90 zaten tutuyor” | §6.3-4 ölçüm **YOK** | `Q: Ölçüm yoksa? → kanıtsız (G5)` |
