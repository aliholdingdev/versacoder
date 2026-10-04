---
title: "VersaCoder — Application Servis Şablonu (Service Template)"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "application servis sablonu: islem akisi, yetki kontrolu ve Result donusleri"
---

# VersaCoder — Application Servis Şablonu (Service Template)

**Zorunlu Bağlantılar:** [index](../index.md) · [handler-template](handler-template.md) · [repository-template](repository-template.md) · [ascii-diagram-template](../architecture/ascii-diagram-template.md) · [index.md](../../architecture/index.md) · [coding-standards.md](../../.rules/coding-standards.md)

---

## §1 Amaç

### §1.1 Neden servis şablonu zorunlu

VersaCoder'da Application katmanı, UI ile veri erişimi arasındaki **tek** orkestrasyon alanıdır. Şablon bu katmanın neyi yapıp neyi yapmayacağını sabitler:

- Servis *iş kuralı sahibi* değildir; invariant `VersaCoder.Domain`'dedir.
- SQL `VersaCoder.Infrastructure`'da, ekran `VersaCoder.Desktop`/`.Cli`'dedir.
- Servis yalnızca **use case'i sıraya koyar**: doğrulama → repository çağrısı → `Result<T>` dönüşü.
- Arayüz `IXService.cs`, implementasyon `XService.cs` olarak iki dosyada yaşar.

Şablon olmadan her geliştirici farklı bir şey yapar:

- Kimisi servise SQL yazdırır, kimisi servise iş kuralını gömer.
- Kimisi `async` metotlarda `CancellationToken`'ı unutur.
- Üçü de derlemeden geçer, üçü de gece üretiminde patlar.

Bu şablon tek iskelet dayatır: arayüz → implementasyon → DI → test → ölçüm (§5.1).

| Alan | Değer |
|------|-------|
| Template Name | `service-template.md` |
| Template Path | `.ai/.templates/backend/service-template.md` |
| Hedef Dosya | `src/VersaCoder.Application/**/XService.cs` |
| Hedef Arayüz | `src/VersaCoder.Application/**/IXService.cs` |
| Katman | `VersaCoder.Application` (K3) |
| Üst Katman | `VersaCoder.Host`, `VersaCoder.Protocol` |
| Alt Katman | `VersaCoder.Domain`, `VersaCoder.Abstractions` |
| Sorumlu | `developer` |
| Denetleyen | `reviewer` + `testing` |
| Kodlama | UTF-8 (BOM'suz) — Latin-1/CP1254 YASAK |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — kod/slug adı İngilizce |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz; `⚠️ VERIFICATION REQUIRED` yazılır.

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|-------|--------------------------|-------|
| E1 | `.ai/.templates/backend/` klasörü var | `Test-Path` → **False** | ❌ YOK — bu şablonla oluşacak |
| E2 | `src/VersaCoder.Application/**/XService.cs` | `src/` kökte **yok**; kök = `.ai` + `.git` | ⚠️ VERIFICATION REQUIRED |
| E3 | `.ai/architecture/index.md` katman listesi | `.ai/` altında **yalnız** `.templates/` | ⚠️ VERIFICATION REQUIRED |
| E4 | `.ai/.rules/coding-standards.md` | `.ai/.rules/` klasörü **yok** | ⚠️ VERIFICATION REQUIRED |
| E5 | ADO.NET + Dapper; EF Core yasak (ADR-004) | kod okunamadı (`src/` yok) | ⚠️ VERIFICATION REQUIRED |
| E6 | TFM `net48;net10.0-windows`, DevExpress 26.1 | `Directory.Build.props` / `.sln` **yok** | ⚠️ VERIFICATION REQUIRED |
| E7 | `ascii-diagram-template.md` biçim referansı | **38.121 byte okundu** (644 satır) | ✅ DOĞRULANDI |
| E8 | `index.md` şablon envanteri (#17 `service-template`) | **38.816 byte okundu** (520–680 satır) | ✅ DOĞRULANDI |
| E9 | Bu şablon yazıldı | `.ai/.templates/backend/service-template.md` | ✅ DOĞRULANDI |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır.

Bu şablonu üreten geliştirici, E2–E6 satırlarını ilk kod yazımında kapatmakla yükümlüdür:

- E2/E3 → ilk `XService.cs` + `.ai/architecture/index.md` okunarak kapanır.
- E4/E6 → `.ai/.rules/` + `Directory.Build.props` taranarak kapanır.
- E5 → §5.3/3 çıktısıyla kapanır (Infrastructure referansı = 0).

### §1.3 Servis vs. Alternatifleri

| Karar | Kullanılacak | Kullanılmayacak | Ayrım Testi |
|-------|--------------|-----------------|-------------|
| Birden çok repository çağrısı + akış | **Servis** | Handler (tek komut yeterli) | ≥2 alt adım var mı? |
| Tek komut + validator + pipeline | Handler | Servis | `[[backend/handler-template]]` |
| SQL / `IDbConnection` | Repository | Servis | Metin `SELECT` içeriyor mu? |
| Invariant (`Kod ≥ 0` gibi) | Domain entity | Servis | Kural değişince DB mi değişiyor? |
| Ekran olayı (button click) | Handler → servis | Form kodu | UI'da `await` varsa kural çiğnenmiş |
| Cross-cutting (log, zaman, GUID) | `CrossCutting` | Servis | Durumsuz mu? |

### §1.4 Neden §5.3 Ölçümü Zorunlu

Servis kodu "görünürde doğru" yazılır: derlenir, çalışır, iyi görünür. Yanlışlık ancak üç yerde ortaya çıkar — hepsi ölçülebilir:

| Risk | Ölçülebilir Belirti | Ölçüm |
|------|---------------------|-------|
| Senkron-over-async | `.Result`, `.Wait()` geçen satır | §5.3/2 = 0 |
| Ters bağımlılık | `.csproj`'de `VersaCoder.Infrastructure` | §5.3/3 = 0 |
| Kırık I/O | `async Task` metot sayısı ≠ `CancellationToken` sayısı | §5.3/4 |
| Kaynak sızıntısı | `IDbConnection` serviste elle açılmış | §5.3/5 = 0 |
| Mojibake | Latin-1 artığı iki-bayt deseni geçen satır | §5.3/6 = 0 |

Kanıtsız "servis temiz" iddiası master prompt §6/5'e göre geçersizdir: §5.3 çıktısı olmadan inceleme başlamaz.

---

## §2 Kapsam

**İçinde:** Application katmanı servis arayüzü + implementasyonu, DI yaşam döngüsü, `async`/`CancellationToken`, hata modeli, transaction sınırı, logging, test stratejisi, kabul kriterleri.

**Dışında:**

- Command/Query handler → `[[backend/handler-template]]`
- Repository / SQL / migration → `[[backend/repository-template]]`
- Domain invariant'ları → `VersaCoder.Domain`
- Ekran tasarımı → GATE-003 (`ui-design/*`)

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya Tipi | Hedef Yol | Sorumlu | Bu Şablondan mı? |
|---|-----------|-----------|---------|-------------------|
| 1 | Servis arayüzü | `src/VersaCoder.Application/**/IXService.cs` | developer | ✅ |
| 2 | Servis implementasyonu | `src/VersaCoder.Application/**/XService.cs` | developer | ✅ |
| 3 | Command/Query handler | `src/VersaCoder.Application/**/XHandler.cs` | developer | ❌ → `handler-template.md` |
| 4 | Repository arayüzü+impl | `src/VersaCoder.Infrastructure/**/XRepository.cs` | developer | ❌ → `repository-template.md` |
| 5 | Domain entity / invariant | `src/VersaCoder.Domain/**` | architect | ❌ → domain kuralı |
| 6 | DI kaydı | `src/VersaCoder.Host/**/ServiceCollectionExtensions.cs` | developer | ✅ §3.5 tablosu bağlayıcı |
| 7 | Servis testi | `tests/VersaCoder.Tests/**/XServiceTests.cs` | testing | ✅ §3.8 |
| 8 | Ekran / form kodu | `src/VersaCoder.Desktop/**` | developer | ❌ GATE-003 |
| 9 | Servis belgesi | `.ai/architecture/application-layer.md` | architect | ❌ doküman ≠ kod |

### §2.2 Kapsam Sınırı Testi (S1–S5)

Üretilecek dosyaya sırayla sor; **ilk `HAYIR`** bu şablonun yanlış olduğunu gösterir.

| # | Soru | Beklenen | Aksi Durumda |
|---|------|----------|--------------|
| S1 | Dosya `VersaCoder.Application` altında mı? | **EVET** | Katman yanlış → §2.1 |
| S2 | Sınıf use case orkestrasyonu yapıyor mu (≥2 adım)? | **EVET** | Tek adım → handler şablonu |
| S3 | İçinde iş kuralı / invariant var mı? | **HAYIR** | Domain'e taşı (`G1`) |
| S4 | İçinde SQL / `IDbConnection` / Dapper var mı? | **HAYIR** | Repository'ye taşı (`G2`) |
| S5 | DevExpress / WinForms tipine referans var mı? | **HAYIR** | UI katmanına taşı (`G3`) |

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

```yaml
---
title: "X — <kısa türkçe açıklama>"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
---
```

### §3.2 H1 + Zorunlu Bağlantılar iskeleti

````markdown
# X — <türkçe açıklama>
**Zorunlu Bağlantılar:** [index](.templates/index.md) · [handler-template](.templates/backend/handler-template.md) · [repository-template](.templates/backend/repository-template.md) · [ascii-diagram-template](.templates/backend/../architecture/ascii-diagram-template.md) · [index.md](architecture/index.md) · [coding-standards.md](.rules/coding-standards.md)
````

Bağlantı sayısı **6'dan az olamaz** (`G4`).

### §3.3 Servis Katmanı — Sorumluluk Sınırı (T4 zincir)

```text
+----------------------------------------------------------------+
| VersaCoder.Application — servis katmanı (orkestrasyon)         |
+----------------------------------------------------------------+
| VersaCoder.Desktop / .Cli  →  tetikleyici (UI)                 |
|   v                                                            |
| IXService.ExecuteAsync(ct)                                     |
|   v                                                            |
| I*Repository.AddAsync  →  Dapper (@p1,@p2)                     |
|   v                                                            |
| AdoConnectionFactory → IDbConnection → versacoder.db           |
+----------------------------------------------------------------+
```

**Ne anlatır:** Servis aşağı bakar (`Domain`, `Abstractions`), yukarıdan çağrılır.

**Hiçbir zaman** `Infrastructure`'a doğrudan referans vermez — repository arayüzü `Abstractions`'ta yaşadığı için bağımlılık yönü bozulmaz.
**Kaynak:** [index.md](../../architecture/index.md) · **Doğrulama:** §5.3/3 — 2026-09-25.

### §3.4 Arayüz Sözleşmesi İskeleti (dolu örnek)

````csharp
// src/VersaCoder.Application/Workspaces/IWorkspaceService.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Results;   // Result<T>

namespace VersaCoder.Application.Workspaces;

/// <summary>
/// Workspace use-case orkestrasyonu. İş kuralı taşımaz; invariant Domain’dedir.
/// </summary>
public interface IWorkspaceService
{
    Task<Result<WorkspaceDto>>         OpenAsync(long workspaceId, CancellationToken ct);
    Task<Result<IReadOnlyList<WorkspaceDto>>> ListAsync(long ownerId, CancellationToken ct);
    Task<Result<long>>                 CreateAsync(CreateWorkspaceRequest request, CancellationToken ct);
    Task<Result<bool>>                 ArchiveAsync(long workspaceId, CancellationToken ct);
}
````

**Kural:** Her `async` metot **son parametrede** `CancellationToken ct` taşır (`G5`).

Dönüş her zaman `Task<Result<T>>` — asla çıplak `Task`, `bool` veya `void`.

### §3.5 Implementasyon İskeleti (dolu örnek)

````csharp
// src/VersaCoder.Application/Workspaces/WorkspaceService.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VersaCoder.Abstractions.Results;
using VersaCoder.Abstractions.Repositories;   // IWorkspaceRepository (arayüz burada)
using VersaCoder.Domain.Workspaces;            // invariant sahibi

namespace VersaCoder.Application.Workspaces;

public sealed class WorkspaceService : IWorkspaceService
{
    private readonly IWorkspaceRepository _repository;
    private readonly ILogger<WorkspaceService> _logger;

    public WorkspaceService(
        IWorkspaceRepository repository,
        ILogger<WorkspaceService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger     = logger     ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<WorkspaceDto>> OpenAsync(long workspaceId, CancellationToken ct)
    {
        if (workspaceId <= 0)
            return Result.Fail<WorkspaceDto>("workspace.id.invalid", "workspaceId 0’dan büyük olmalı.");

        var entity = await _repository.GetByIdAsync(workspaceId, ct).ConfigureAwait(false);
        if (entity is null)
            return Result.Fail<WorkspaceDto>("workspace.not_found", "Workspace bulunamadı.");

        // Invariant zorlaması Domain’de: entity.Archive() kuralı çiğnerse burada exception/Result döner.
        if (entity.IsArchived)
            return Result.Fail<WorkspaceDto>("workspace.archived", "Arşivlenmiş workspace açılamaz.");

        _logger.LogInformation("Workspace açıldı: {WorkspaceId}", workspaceId);
        return Result.Ok(WorkspaceDto.From(entity));
    }

    public async Task<Result<long>> CreateAsync(CreateWorkspaceRequest request, CancellationToken ct)
    {
        var entity = Workspace.Create(request.Name, request.OwnerId);  // Domain fabrikası = kural yeri
        var id = await _repository.AddAsync(entity, ct).ConfigureAwait(false);
        _logger.LogInformation("Workspace oluşturuldu: {WorkspaceId}", id);
        return Result.Ok(id);
    }

    // ListAsync / ArchiveAsync aynı kalıp: doğrula → çağır → Result döndür.
}
````

### §3.6 DI Yaşam Döngüsü Tablosu (Host kayıt kuralı)

| Hizmet | Yaşam Döngüsü | Neden | İhlal Belirtisi |
|--------|---------------|-------|-----------------|
| `I*Service` (durumsuz) | **Transient** | Her çağrıda taze; durum taşımaz | Servis içinde field = durum sızıntısı |
| `I*Repository` | **Scoped** | Aynı istek/oturum içinde tek `IDbConnection` | Aynı scope'ta 2 farklı bağlantı |
| `ILogger<T>` | **Singleton** | Konsol/dosya yazıcı ortak | Log dosyası çoğalması |
| Mediatör/dispatcher | **Singleton** | Stateless, kayıt tablosu sabit | — |
| Yapılandırma (`IOptions`) | **Singleton** | Değişmez | — |
| `IDbConnection` / `AdoConnectionFactory` | **Scoped** | Transaction bütünlüğü | Dağınık transaction |
| DevExpress `RibbonForm` | **Geçici (UI)** | Form yaşam döngüsü | DI'a kaydedilmez |

> `net48` dalında DI sağlayıcısı `Microsoft.Extensions.DependencyInjection` 8.x uyumlu paketle gelir; `net10.0-windows` dalında gömülüdür. Kayıt kodu `#if` ile bölünmez — iki dal da aynı `AddVersaCoderApplication()` çağırır (`G6`).

### §3.7 async/await + CancellationToken Kuralları

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Her `async` metot `CancellationToken` alır | `Task<Result<T>> XAsync(..., CancellationToken ct)` | `Task<Result<T>> XAsync(...)` |
| 2 | `ct` aşağıya iletilir | `await repo.GetAsync(id, ct)` | `await repo.GetAsync(id)` |
| 3 | Uzun CPU işi ayırma | `Task.Run(..., ct)` | senkron bloklama |
| 4 | Senkron-over-async yasak | `await` | `.Result`, `.Wait()`, `.GetAwaiter().GetResult()` |
| 5 | `ConfigureAwait(false)` | Kütüphane/servis kodunda | UI iş parçacığında (Desktop) |
| 6 | Token iptali `Result` olur | `ct.ThrowIfCancellationRequested()` → iptal | iptali "hata" gibi loglamak |

### §3.8 Hata Yönetimi: Domain Failure vs. Exception

| Durum | Tür | Dönüş | Log Seviyesi | Örnek |
|-------|-----|-------|--------------|-------|
| Kural ihlali (iş kuralı) | **Failure** | `Result.Fail(code, msg)` | `Warning` | `workspace.archived` |
| Bulunamayan kayıt | **Failure** | `Result.Fail("not_found")` | `Information` | `workspace.not_found` |
| Doğrulama hatası | **Failure** | `Result.Fail("validation")` | `Debug` | `workspace.id.invalid` |
| Beklenmeyen (DB koptu) | **Exception** | yukarı fırlatılır → host yakalar | `Error` | `TimeoutException` |
| İptal (`OperationCanceled`) | **Exception** | yakalanmaz, üstte iletilir | `Debug` | `ct` iptali |
| Doğrulanmamış kod hatası | **Bug** | derleme zamanı | — | `TreatWarningsAsErrors` |

**Transaction sınırı:** transaction'ın sahibi repository/unit-of-work katmanıdır (`[[backend/repository-template]]` §3.5) — servis sarmaz. Gerekçe: Dapper'da `IDbTransaction` bağlantıyla yaşar; iki sahip olursa commit/rollback çakışır. Servis yalnızca *çoklu yazım* senaryosunda transaction scope'u ister ve `G8` ile denetlenir.

**Logging tablosu:**

| Olay | Seviye | Şablon Alanları | Yasak |
|------|--------|-----------------|-------|
| Use case başlangıcı | `Debug` | `{UseCase}`, `{CorrelationId}` | request body |
| Başarılı sonuç | `Information` | `{EntityId}`, `{ElapsedMs}` | PII, token |
| Kural ihlali | `Warning` | `{Code}`, `{EntityId}` | stack trace |
| Altyapı hatası | `Error` | `{Exception}`, `{CorrelationId}` | connection string |
| İptal | `Debug` | `{UseCase}` | "hata" etiketi |

### §3.9 Test Stratejisi + Kabul Kriterleri

| Katman | Araç | Kapsam | Hedef |
|--------|------|--------|-------|
| Servis birim testi | xunit + **Moq** (veya NSubstitute) | Her public metot: mutlu + 3 hata yolu | %90 coverage |
| İptal testi | xunit `CancellationTokenSource` | `ct` iptali → `OperationCanceledException` | zorunlu |
| Bağımlılık testi | `dotnet test` + ref. taraması | `Infrastructure` referansı = 0 | zorunlu |
| Pipeline testi | handler testleri | `[[backend/handler-template]]` | %90 |
| Entegrasyon | test DB (`versacoder.test.db`) | gerçek repository | §3.8 |

**Kabul kriterleri (release gate):**

- [ ] AC-1 Arayüz + implementasyon §3.4/§3.5 kalıbına uyuyor
- [ ] AC-2 Her `async` metotta `CancellationToken` var; `.Result`/`.Wait()` = 0
- [ ] AC-3 `.csproj`'de `VersaCoder.Infrastructure` referansı yok
- [ ] AC-4 İş kuralı Domain'de; serviste `if` ile invariant yok
- [ ] AC-5 `dotnet test` yeşil, coverage ≥ %90
- [ ] AC-6 DI kaydı §3.6 tablosuna uygun (Transient servis)
- [ ] AC-7 §5.3 altı komutun hepsi beklenen çıktıda
- [ ] AC-8 Türkçe log mesajı + kod çifti (`workspace.not_found` gibi)

---

## §4 Kurallar

### §4.1 Servis Yazım Sırası

1. **Önce arayüz** (`IXService.cs`) — sözleşmeyi yaz, implementasyonu sonra.
2. **Sonra implementasyon** — §3.5 kalıbı; ilk metot en basit use case.
3. **DI kaydı** — §3.6 tablosu; kaydı unutmak runtime `InvalidOperationException` verir.
4. **Test** — §3.8; %90 olmadan inceleme açılmaz.
5. **Ölçüm** — §5.3 altı komut; çıktı yoksa inceleme yok.

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Çıktı | Süre |
|-------|-----|-------|------|
| 1 | `developer` — arayüz + impl + DI | kod + test | 45 dk |
| 2 | `reviewer` — §6.1 checklist (18 madde) | ✅/RED | 15 dk |
| 3 | `testing` — `dotnet test` + coverage raporu | %90 kanıtı | 20 dk |
| 4 | `security` — §5.3/2, /3, /5 taraması | 0 ihlal | 10 dk |
| 5 | `docs-writer` — `.ai/log.md` + envanter senkronu | kayıt | 5 dk |

**Toplam ≈ 95 dk/servis.** 3 RED sonrası iş durur, gerekçe `architect`'e yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Metot/servis | ≤ 10 public metot | aşımda servisi böl |
| Metot gövdesi | ≤ 40 satır | aşımda helper çıkar (private) |
| Parametre | ≤ 5 | request DTO'su |
| Coverage | ≥ %90 | `G10` — job kırmızı |
| Frontmatter | 7 alan eksiksiz | `G11` |
| Satır aralığı | 480–660 | `G12` |
| Dosya boyu | 15–32 KB | `G12` |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm | Ek İşlem |
|-----------|-------|----------|
| Sadece yorum/dolgu | aynı | — |
| Yeni servis metodu | `minor` | `index.md` hedef satırı + `log.md` |
| Arayüz kırma (imza değişti) | `major` | tüm çağıranlar + testler |
| Hata kodu değişimi | `minor` | UI mesaj tablosu |
| DI yaşam döngüsü değişimi | `minor` | §3.6 tablosu + `G7` denetimi |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | İş kuralı/invariant serviste | **DUR** — Domain'e taşınır, kod reddedilir |
| G2 | SQL, `IDbConnection`, Dapper serviste | **DUR** — repository'ye taşınır |
| G3 | DevExpress / WinForms tipi Application'da | **DUR** — katman ihlali, derleme hedefi kırılır |
| G4 | `**Zorunlu Bağlantılar:**` < 6 bağlantı | dosya RED |
| G5 | `async` metotta `CancellationToken` yok | **DUR** — imza düzeltilir |
| G6 | `.Result` / `.Wait()` / `GetAwaiter().GetResult()` | **DUR** — async zincir kırılmış |
| G7 | Scoped bağımlılık Singleton serviste | **DUR** — yaşam döngüsü yeniden seçilir |
| G8 | Serviste elle `IDbConnection.Open()` / transaction | **DUR** — repository katmanına |
| G9 | `catch (Exception) { /* yut */ }` | **DUR** — en az `Result.Fail` veya log + rethrow |
| G10 | Coverage < %90 veya `dotnet test` kırmızı | iş birleştirilemez |
| G11 | Frontmatter 7 alan eksiksiz değil / `authority: SSOT` yok | dosya RED |
| G12 | Satır < 480 veya > 660 / KB < 15 veya > 32 | dosya RED — §5.3/7 ölçülür |
| G13 | `CoreMusic` adı geçerse (ürün = VERSA CODER) | dosya RED + ad temizlenir |
| G14 | Mojibake (Latin-1 artığı iki-bayt desen) veya CP1254 kodlaması | dosya UTF-8 BOM'suz yeniden yazılır |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış (tam tablo)

| # | Adım | Girdi | Çıktı | Yapan | Süre | Kapı |
|---|------|-------|-------|-------|------|------|
| 1 | Use case'i listele | gereksinim + `[[../index]]` | ≥2 alt adım | developer | 5 dk | — |
| 2 | Sınır testini koş | §2.2 S1–S5 | 5× EVET/HAYIR | developer | 3 dk | S3–S5 `HAYIR` ise dur |
| 3 | Arayüzü yaz | §3.4 kalıp | `IXService.cs` | developer | 10 dk | — |
| 4 | Implementasyonu yaz | §3.5 kalıp | `XService.cs` | developer | 25 dk | — |
| 5 | DI kaydı | §3.6 tablosu | Host kayıt satırı | developer | 5 dk | — |
| 6 | Test yaz | §3.8 | `%90` test | testing | 30 dk | — |
| 7 | Ölç | §5.3/1–/6 | 6 çıktı | developer | 5 dk | G6, G8 |
| 8 | İncele | §6.1 (18 madde) | ✅/RED | reviewer | 15 dk | 3 RED → dur |
| 9 | Quality Report | §5.4 kalıp | report bloğu | reviewer | 5 dk | — |
| 10 | Senkron | `log.md` + envanter | kayıt | docs-writer | 5 dk | — |

**Zaman bütçesi:** adım 1–7 ≈ 83 dk, 8–10 ≈ 25 dk → **≈ 108 dk/servis** (§4.2 ile uyumlu, %15 tampon dahil).

### §5.2 Hata Modları

| Kod | Hata modu | Belirti | Kök neden | Düzeltme |
|-----|-----------|---------|-----------|----------|
| H1 | Servis SQL yazmış | `SELECT` geçen satır | repository sınırı unutulmuş | `[[backend/repository-template]]` |
| H2 | İş kuralı serviste | `throw new BusinessException` Domain'de yoksa | invariant yanlış yerde | Domain entity'ye taşı |
| H3 | `.Result` kullanımı | §5.3/2 > 0 | senkron-over-async | `async`/`await` zinciri |
| H4 | Ters bağımlılık | §5.3/3 > 0 | `.csproj` referansı | arayüzü `Abstractions`'a al |
| H5 | Eksik `ct` | §5.3/4 farkı > 0 | unutulmuş parametre | her imzaya `CancellationToken` |
| H6 | DI `InvalidOperationException` | runtime "no service for type" | kayıt yok / yanlış ömür | §3.6 |
| H7 | Ölü kod | hiç çağrılmayan public metot | gereksinim değişti | sil veya `internal` yap |
| H8 | Mojibake | iki-bayt Latin-1 artığı görünüyor | Latin-1 ile kaydetme | UTF-8 BOM'suz yeniden yaz |
| H9 | Coverage düşüşü | < %90 | eksik hata yolu testi | §3.8 test matrisi |

### §5.3 Salt-Okunur Doğrulama Komutları (6)

Aşağıdaki komutların **hiçbiri dosya değiştirmez** (`Set-`, `Remove-`, `New-`, `Move-` yoktur).

```powershell
# 1) SERVİS DOSYASI SAYISI — hedef: ≥ 1 (src/ yoksa uyarı satırı)
Get-ChildItem -Path 'C:\versacoder\src\VersaCoder.Application' -Recurse -Filter '*Service.cs' -ErrorAction SilentlyContinue |
  Measure-Object | Select-Object -ExpandProperty Count

# 2) SENKRON-OVER-ASYNC — hedef: 0
Select-String -Path 'C:\versacoder\src\VersaCoder.Application\**\*.cs' -Pattern '\.Result|\.Wait\(\)|GetAwaiter\(\)' -ErrorAction SilentlyContinue |
  Measure-Object | Select-Object -ExpandProperty Count

# 3) TERS BAĞIMLILIK (Infrastructure referansı) — hedef: 0
Select-String -Path 'C:\versacoder\src\VersaCoder.Application\*.csproj' -Pattern 'VersaCoder\.Infrastructure' -ErrorAction SilentlyContinue |
  Measure-Object | Select-Object -ExpandProperty Count

# 4) CancellationToken KAPSAMI — 'async Task' sayısı ile 'CancellationToken' sayısı karşılaştırılır
$a = (Select-String -Path 'C:\versacoder\src\VersaCoder.Application\**\*.cs' -Pattern 'async\s+Task' -ErrorAction SilentlyContinue | Measure-Object).Count
$b = (Select-String -Path 'C:\versacoder\src\VersaCoder.Application\**\*.cs' -Pattern 'CancellationToken' -ErrorAction SilentlyContinue | Measure-Object).Count
"async Task=$a  CancellationToken=$b  (b < a ise G5 ihlali)"

# 5) ELLE BAĞLANTI / SQL — hedef: 0
Select-String -Path 'C:\versacoder\src\VersaCoder.Application\**\*.cs' -Pattern 'IDbConnection|SELECT\s|INSERT\s|ExecuteNonQuery' -ErrorAction SilentlyContinue |
  Measure-Object | Select-Object -ExpandProperty Count

# 6) MOJİBAKE — hedef: 0
Select-String -Path 'C:\versacoder\.ai\.templates\backend\*.md' -Pattern '[\u00C3\u00C2]|\uFFFD' |
  Measure-Object | Select-Object -ExpandProperty Count

# 7) (ek) TEST + ÖLÇÜM — satır/KB aralığı 480-660 / 15-32
$f = 'C:\versacoder\.ai\.templates\backend\service-template.md'
$satir = (Get-Content -LiteralPath $f -Encoding UTF8 | Measure-Object -Line).Lines
$kb = [math]::Round((Get-Item -LiteralPath $f).Length / 1KB, 1)
"satır=$satir (480-660)  KB=$kb (15-32)"
```

`dotnet test` komutu (salt-okunur sayılır; yalnızca `bin/obj` üretir, kaynak dokunmaz):

```powershell
dotnet test 'C:\versacoder\tests\VersaCoder.Tests' --filter 'FullyQualifiedName~Service' --no-restore
```

### §5.4 Quality Report

```markdown
## Servis Kalite Raporu — {{DOSYA_ADI}} (YYYY-MM-DD)

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Satır / KB | {{N}} / {{KB}} | 480–660 · 15–32 | 🟢/🔴 |
| Public metot | {{N}} | ≤ 10 | 🟢/🔴 |
| `CancellationToken` kapsama | {{b}}/{{a}} | b ≥ a | 🟢/🔴 |
| `.Result`/`.Wait()` | {{N}} | 0 | 🟢/🔴 |
| Ters bağımlılık (§5.3/3) | {{N}} | 0 | 🟢/🔴 |
| Elle SQL/IDbConnection (§5.3/5) | {{N}} | 0 | 🟢/🔴 |
| `dotnet test` | {{PASS}} | yeşil | 🟢/🔴 |
| Coverage | {{%}} | ≥ 90 | 🟢/🔴 |
| Guardrail G1–G14 | ihlal {{N}} | 0 | 🟢/🔴 |

**Karar:** 🟢 inceleme açıldı / 🔴 düzeltmeye gönderildi ({{G-KOD}})
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (18 madde)

- [ ] 01 Frontmatter 7 alan eksiksiz; `authority: SSOT`
- [ ] 02 H1 + `**Zorunlu Bağlantılar:**` 6 bağlantı
- [ ] 03 §1.1–§1.4 dolu; §1.2'de `⚠️ VERIFICATION REQUIRED` olan satır var
- [ ] 04 §1.2'de yorum yok — yalnız iddia/kanıt/durum
- [ ] 05 §2.1 dosya tipi → sorumlu tablosu ≥ 9 satır
- [ ] 06 §2.2 S1–S5 cevaplanmış (S3–S5 `HAYIR`)
- [ ] 07 §3.3 kutu 66 sütuna hizalı (§5.3/1 = 0 uyuşmayan)
- [ ] 08 §3.4 arayüz kalıbı `{{PLACEHOLDER}}` içermez
- [ ] 09 §3.5 implementasyon `Result<T>` + `ConfigureAwait(false)` içerir
- [ ] 10 §3.6 DI tablosu ≥ 7 satır; servis = Transient
- [ ] 11 §3.7 altı kural; `.Result` yasağı yazılı
- [ ] 12 §3.8 hata tablosu ≥ 6 satır; failure/exception ayrımı net
- [ ] 13 §3.8 transaction sahibi repository olarak gösterilmiş
- [ ] 14 §3.9 test stratejisi Moq/NSubstitute + %90
- [ ] 15 §4.5 G1–G14 = 14 satır, "İhlal Sonucu" dolu
- [ ] 16 §5.1 10 adım + zaman bütçesi
- [ ] 17 §5.3 ≥ 5 salt-okunur komut (`Set-`/`Remove-` yok)
- [ ] 18 §7.2 Red Team ≥ 10 satır

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Tetikleyici | Aksiyon | Sorumlu |
|--------|-------------|---------|---------|
| RS-1 | §5.3/3 > 0 (Infrastructure referansı) | **DUR** — `.csproj` düzeltilir | developer |
| RS-2 | §5.3/2 > 0 (`.Result`) | **DUR** — async zincir onarılır | developer |
| RS-3 | Serviste `SELECT` geçen satır | **DUR** — repository'ye taşınır | reviewer |
| RS-4 | Coverage < %90 | iş birleştirilemez | testing |
| RS-5 | §5.3/6 > 0 (mojibake) | dosya UTF-8 ile yeniden yazılır | docs-writer |
| RS-6 | Satır/KB aralığı dışında | **DUR** — G12 | reviewer |
| RS-7 | `CoreMusic` geçen satır | **DUR** — G13 | reviewer |
| RS-8 | §1.2'de kanıtsız `✅` | iddia `⚠️`'ye çekilir | architect |

### §6.3 Sürüm / Geçiş Kuralları

| Değişiklik | Sürüm | Durum | Ek |
|-----------|-------|-------|----|
| Dolgu/hizalama düzeltmesi | `patch` | `active` | §5.3/1 yeniden |
| Yeni alt bölüm (`§x.y`) | `minor` | `active` | Quality Report |
| Kalıp değişimi (§3.5) | `minor` | `active` | üretilen dosyalar taranır |
| Bölüm silme / yeniden adlandırma | `major` | `review` | bağlantı + `log.md` |
| Şablonun iptali | `major` | `deprecated` | yerine geçen dosya yazılır |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/backend/handler-template.md` | tek komut senaryosu bu şablona devredilir |
| `.ai/.templates/backend/repository-template.md` | transaction + SQL sahibi |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu kuralı (66 sütun) |
| `.ai/.templates/index.md` | envanter satırı #17 |
| `.ai/architecture/index.md` | katman gerçeği (⚠️ doğrulanacak) |
| `.ai/.rules/coding-standards.md` | kod standardı (⚠️ doğrulanacak) |

### §6.5 Şablona Özgü Kontroller

- [ ] K1 Her servis arayüzü `Result<T>` döndürüyor (çıplak `void`/`bool` yok)
- [ ] K2 `Workspace` gibi domain tipine `using` var; DTO ile entity karışmıyor
- [ ] K3 DI kaydı §3.6 ile çelişmiyor (Transient servis, Scoped repository)
- [ ] K4 Servis metodu ≤ 40 satır (§4.3)
- [ ] K5 Log mesajı Türkçe + `{Placeholder}` structured alan içeriyor
- [ ] K6 Hata kodu tutarlı (`entity.reason` deseni, `.` ile ayrık)
- [ ] K7 `ConfigureAwait(false)` servis/kütüphane kodunda var
- [ ] K8 Serviste `static` yardımcı yok (test edilemez; `CrossCutting`'e)
- [ ] K9 Her public metot için en az 1 test metodu var (§3.8)
- [ ] K10 §5.3/4 çıktısı `async Task` ≤ `CancellationToken` (G5 ihlali yok)

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| # | Kaynak | Yol | Ne İçin | Kanıt |
|---|--------|-----|---------|-------|
| 1 | Şablon envanteri | `.ai/.templates/index.md` | #17 kayıt + §3.7 kalite sözleşmesi | ✅ OKUNDU (38.816 B) |
| 2 | Kutu kuralı | `.ai/.templates/architecture/ascii-diagram-template.md` | §3.3 çizimi, §5.3/1 | ✅ OKUNDU (38.121 B) |
| 3 | Handler şablonu | `.ai/.templates/backend/handler-template.md` | §1.3 ayrım | ⚠️ VERIFICATION REQUIRED |
| 4 | Repository şablonu | `.ai/.templates/backend/repository-template.md` | §3.8 transaction | ⚠️ VERIFICATION REQUIRED |
| 5 | Katman gerçeği | `.ai/architecture/index.md` | §3.3 zincir | ⚠️ VERIFICATION REQUIRED |
| 6 | Kod standardı | `.ai/.rules/coding-standards.md` | §4.3 ölçek | ⚠️ VERIFICATION REQUIRED |
| 7 | ADR-004 veri yığını | `.ai/adr/ADR-004-*` | Dapper/EF yasağı | ⚠️ VERIFICATION REQUIRED |
| 8 | Kapı planı | `.ai/WORKFLOW.md` | §5.1 kapı sütunu | ⚠️ VERIFICATION REQUIRED |
| 9 | Anahtar sözlük | `.ai/keys.md` | terim tutarlılığı | ⚠️ VERIFICATION REQUIRED |

### §7.2 Red Team — Sık Yapılan Hatalar (10 satır)

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Servise SQL yazmak | Katman ihlali; `Infrastructure` bağımlılığı doğar | Repository arayüzü + `[[backend/repository-template]]` |
| 2 | İş kuralını servise gömmek | Kural iki yerde yaşar, er geç çelişir | Invariant `VersaCoder.Domain`'de |
| 3 | `.Result` ile beklemek | Deadlock riski (`net48` WinForms iş parçacığında) | `async`/`await` + `CancellationToken` |
| 4 | `CancellationToken`'ı unutmak | İptal edilemeyen uzun I/O | §3.7 kural 1 — her imzada `ct` |
| 5 | Servisi Singleton yapmak | Scoped repository'yi taşır, durum sızar | §3.6 — servis Transient |
| 6 | `catch (Exception) { }` | Hata sessizce yutulur, üretimde bulunamaz | `Result.Fail` veya log + rethrow (G9) |
| 7 | Transaction'ı serviste açmak | Çift sahip; repository ile çakışır | Sahip repository/unit-of-work |
| 8 | Testi "çalışıyor" diye geçirmek | %90 kanıtsız; hata yolları açık | §3.8 + `dotnet test` çıktısı |
| 9 | §5.3 ölçümü çalıştırılmadan "temiz" demek | Kanıtsız iddia (master prompt §6/5) | 6 komutun çıktısını rapora yapıştır |
| 10 | `CoreMusic` adını kod/belgeye taşımak | Ürün **VERSA CODER**; yanlış marka sızıntısı | G13 — ad temizlenir, dosya RED |

**SSUT beyanı:** Bu dosya Application servis iskeletinin **tek** kaynağıdır. Çoğaltan her dosya bu dosyaya bağlanır; içerik kopyalanmaz.
