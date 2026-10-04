---
title: "VersaCoder — Command/Query Handler Şablonu (CQRS İskeleti)"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "CQRS handler sablonu: komut/sorgu sozlesmesi, Result kullanimi ve bagimlilik enjeksiyonu"
---

# VersaCoder — Command/Query Handler Şablonu (CQRS İskeleti)

**Zorunlu Bağlantılar:** [index](../index.md) · [service-template](service-template.md) · [repository-template](repository-template.md) · [ascii-diagram-template](../architecture/ascii-diagram-template.md) · [index.md](../../architecture/index.md) · [coding-standards.md](../../.rules/coding-standards.md)

---

## §1 Amaç

### §1.1 Neden sıfırdan MediatR benzeri iskelet

VersaCoder'da paket bağımlılığı yerine **kendi** dispatcher'ı yazılır: `VersaCoder.Protocol` zaten mesaj şeması ve tool sözleşmelerini taşıdığından, CQRS akışı da kendi kodumuz olmalıdır. Amaç üçtür:

- **Sıralamayı görünür kılmak:** logging → validation → transaction → retry → handler sırası kodda değil, **tabloda** tanımlıdır (`§3.5`).
- **Validator'ı bağımsız tutmak:** FluentValidation paketi kullanılmaz; `IValidator<T>` saf C# arayüzüdür.
- **Sonucu tek biçimde döndürmek:** her handler `Result<T>` verir; istisnası yalnızca altyapı hatasıdır.

Ayrı bir servis sınıfı yazmadan tek bir use case gerekiyorsa handler yeterlidir; **≥2 use case** ise `[[backend/service-template]]` devreye girer.

Ek üç zorunluluk:

- **Paket yok:** MediatR/FluentValidation NuGet olarak eklenmez (§4.5 G9).
- **Sıra görünür:** beş davranış §3.5 tablosunda, kodda değil.
- **Sonuç tek tip:** her yol `Result<T>` ile biter; istisna yalnız altyapı hatası.

| Alan | Değer |
|------|-------|
| Template Name | `handler-template.md` |
| Template Path | `.ai/.templates/backend/handler-template.md` |
| Hedef Dosya | `src/VersaCoder.Application/**/XHandler.cs` |
| Hedef Arayüz | `src/VersaCoder.Application/Common/**` (ICommand, IQuery, IValidator) |
| Katman | `VersaCoder.Application` (K3) |
| Paket | **Yok** — MediatR/FluentValidation kullanılmaz |
| Dönüş Tipi | `Result<T>` (`VersaCoder.Abstractions.Results`) |
| Sorumlu | `developer` |
| Denetleyen | `reviewer` + `testing` |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — kod adı İngilizce |
| Kodlama | UTF-8 (BOM'suz) — Latin-1/CP1254 YASAK |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|-------|--------------------------|-------|
| E1 | `.ai/.templates/backend/` içinde handler şablonu | bu dosya yazıldı | ✅ DOĞRULANDI |
| E2 | `src/VersaCoder.Application/**/XHandler.cs` | `src/` kökte **yok** | ⚠️ VERIFICATION REQUIRED |
| E3 | `ICommand<T>` / `IQuery<T>` sözleşmeleri hazır | `.cs` okunamadı (`src/` yok) | ⚠️ VERIFICATION REQUIRED |
| E4 | MediatR/FluentValidation paket kullanılmıyor | `.csproj` yok → paket listesi okunamadı | ⚠️ VERIFICATION REQUIRED |
| E5 | `Result<T>` tipi `Abstractions`'ta | `.ai/architecture/index.md` yok | ⚠️ VERIFICATION REQUIRED |
| E6 | Test projesi xunit + Moq | `tests/` yok | ⚠️ VERIFICATION REQUIRED |
| E7 | ADR-004: EF Core yasak, Dapper zorunlu | `.ai/adr/` klasörü `.ai/` altında yok | ⚠️ VERIFICATION REQUIRED |
| E8 | `ascii-diagram-template.md` biçim referansı | **38.121 byte okundu** | ✅ DOĞRULANDI |
| E9 | `index.md` envanter #18 `handler-template` | **38.816 byte okundu** | ✅ DOĞRULANDI |

**Yorum yasağı:** E2–E7 için "şurada tanımlı" denmez. Kanıt yoksa `⚠️ VERIFICATION REQUIRED` kalır; iddia kapatılmadan §6.4 Quality Report `ONAYLANDI` yazamaz.

### §1.3 Command vs Query Ayrımı

| Özellik | **Command** | **Query** |
|---------|-------------|-----------|
| Arayüz | `ICommand` / `ICommand<TResult>` | `IQuery<TResult>` |
| Yan etki | **Var** (yazma) | **Yok** (salt okunur) |
| Dönüş | `Result<TId>` veya `Result<bool>` | `Result<TDto>` / `Result<IReadOnlyList<TDto>>` |
| Transaction davranışı | `TransactionBehavior` **devrede** | devre dışı |
| Validator | Zorunlu (`§3.4`) | İsteğe bağlı (yalnız filtre doğrulaması) |
| Cache | Yok | `QueryBehavior` ile önbelleğe alınabilir |
| Idempotency | Command key ile | Anlamsız |
| Repo çağrısı | `AddAsync`/`UpdateAsync`/`DeleteAsync` | `Get*`/`List*` |
| Örnek | `CreateWorkspaceCommand` | `GetWorkspaceQuery` |
| Aynı anda çalıştırma | Seri (transaction kilitlenmesin) | Paralel (`Task.WhenAll`) |
| Hata kodu deseni | `<kaynak>.<neden>` | `<kaynak>.not_found` |
| Test yönü | Yan etki + state değişimi | Dönüş değeri doğruluğu |

**Ayırt etme sorusu:** *Sonuç DB'yi değiştiriyor mu?* → Evet = Command, Hayır = Query. Kararsız kalındıysa **Command** sayılır (yanlışlıkla yazma, yanlışlıkla okumadan tehlikelidir).

### §1.4 Neden Pipeline Sırası Sabit

Sıra değişirse davranış değişir; davranış değişirse sonuç değişir. Ölçülebilir gerekçe:

| Sıra Kayması | Sonuç | Ölçüm |
|--------------|-------|-------|
| Validation → Logging'den sonra | Hatalı istek loglanmaz, denetim boşluğu | §5.3/5 log sayısı |
| Transaction → Validation'dan sonra | Geçersiz veriyle DB'ye girilir | §5.3/4 = 0 beklenir |
| Retry → Transaction'ın içinde | Kısmi commit tekrarlanır (çift kayıt) | §5.3/6 = 0 |
| Handler → Validation'dan sonra | Kural hiç çalışmaz | §6.1 kutu 09 |

---

## §2 Kapsam

**İçinde:** Command/Query sözleşmeleri, handler arayüzü, saf validator, pipeline davranışları (4 adet), mediatör kaydı ve dispatch, `Result<T>` hata modeli, test stratejisi.
**Dışında:** Use case orkestrasyonu (`[[backend/service-template]]`), SQL (`[[backend/repository-template]]`), domain invariant'ları (`VersaCoder.Domain`), ekran (GATE-003).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya Tipi | Hedef Yol | Sorumlu | Bu Şablondan mı? |
|---|-----------|-----------|---------|-------------------|
| 1 | Command tanımı | `src/VersaCoder.Application/**/Commands/*.cs` | developer | ✅ §3.2 |
| 2 | Query tanımı | `src/VersaCoder.Application/**/Queries/*.cs` | developer | ✅ §3.2 |
| 3 | Handler | `src/VersaCoder.Application/**/XHandler.cs` | developer | ✅ §3.3 |
| 4 | Validator (saf) | `src/VersaCoder.Application/**/XValidator.cs` | developer | ✅ §3.4 |
| 5 | Pipeline davranışı | `src/VersaCoder.Application/Common/Pipeline/*.cs` | developer | ✅ §3.5 |
| 6 | Mediatör + DI kaydı | `src/VersaCoder.Host/**/MediatorExtensions.cs` | developer | ✅ §3.6 |
| 7 | Çok adımlı use case | `src/VersaCoder.Application/**/XService.cs` | developer | ❌ → `service-template.md` |
| 8 | Repository | `src/VersaCoder.Infrastructure/**/XRepository.cs` | developer | ❌ → `repository-template.md` |
| 9 | Handler testi | `tests/VersaCoder.Tests/**/XHandlerTests.cs` | testing | ✅ §3.8 |
| 10 | Domain entity | `src/VersaCoder.Domain/**` | architect | ❌ invariant orada |

### §2.2 Kapsam Sınırı Testi (S1–S5)

| # | Soru | Beklenen | Aksi Durumda |
|---|------|----------|--------------|
| S1 | Dosya tek bir kullanıcı niyeti (tek komut/sorgu) karşılıyor mu? | **EVET** | ≥2 niyet → `service-template.md` |
| S2 | Dönüş tipi `Result<T>` mi? | **EVET** | düz `T`/`void` → `G2` ihlali |
| S3 | İçinde SQL / `IDbConnection` var mı? | **HAYIR** | repository'ye taşı (`G3`) |
| S4 | İçinde iş invariant'ı var mı? | **HAYIR** | Domain'e taşı (`G4`) |
| S5 | Yan etkisiz (salt okunur) ise `IQuery` mi? | **EVET** | `ICommand` → `G6` ihlali |

---

## §3 Mimari

### §3.1 Frontmatter ve H1 İskeleti

```yaml
---
title: "<KomutAdı> — <kısa türkçe açıklama>"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
---
```

````markdown
# <KomutAdı> — <türkçe açıklama>
**Zorunlu Bağlantılar:** [index](.templates/index.md) · [service-template](.templates/backend/service-template.md) · [repository-template](.templates/backend/repository-template.md) · [ascii-diagram-template](.templates/backend/../architecture/ascii-diagram-template.md) · [index.md](architecture/index.md) · [coding-standards.md](.rules/coding-standards.md)
````

### §3.2 Command/Query Sözleşmesi (dolu örnek)

````csharp
// src/VersaCoder.Application/Common/Messaging/ICommand.cs
public interface ICommand { }
public interface ICommand<TResult> { }

// src/VersaCoder.Application/Common/Messaging/IQuery.cs
public interface IQuery<TResult> { }

// src/VersaCoder.Application/Common/Messaging/ICommandHandler.cs
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken ct);
}

// src/VersaCoder.Application/Common/Messaging/IQueryHandler.cs
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken ct);
}
````

### §3.3 CQRS Akış Diyagramı (T3 — pipeline sırası)

```text
+----------------------------------------------------------------+
| ICommand / IQuery  →  Mediator.SendAsync(cmd, ct)              |
+----------------------------------------------------------------+
| 1 LoggingBehavior    correlation + süre                        |
| 2 ValidationBehavior IValidator<T> <== KAPI: hata → Result.Fail|
| 3 TransactionBehavior IDbTransaction scope                     |
| 4 RetryBehavior      Dapper timeout için 3 deneme              |
| 5 Handler            iş + I*Repository çağrısı                 |
+----------------------------------------------------------------+
| Result<T>  →  IsSuccess / Errors[] / Value                     |
+----------------------------------------------------------------+
```

**Ne anlatır:** Beş davranış **her zaman** bu sırayla çalışır; 2. basamak `Result.Fail` döndüğünde 3–5 hiç çalışmaz (kapı).
**Kaynak:** [index.md](../../architecture/index.md) · **Doğrulama:** §5.3/5 — 2026-09-25.

### §3.4 Saf Validator Katmanı (FluentValidation değil)

````csharp
// src/VersaCoder.Application/Common/Validation/IValidator.cs
public interface IValidator<in T>
{
    IReadOnlyList<ValidationError> Validate(T instance);
}

public sealed record ValidationError(string Code, string Message);

// src/VersaCoder.Application/Workspaces/Validators/CreateWorkspaceValidator.cs
public sealed class CreateWorkspaceValidator : IValidator<CreateWorkspaceCommand>
{
    public IReadOnlyList<ValidationError> Validate(CreateWorkspaceCommand c)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(c.Name))
            errors.Add(new("workspace.name.required", "Workspace adı boş olamaz."));
        if (c.Name?.Length > 120)
            errors.Add(new("workspace.name.too_long", "Workspace adı en fazla 120 karakter."));
        if (c.OwnerId <= 0)
            errors.Add(new("workspace.owner.invalid", "OwnerId 0’dan büyük olmalı."));
        return errors;
    }
}
````

**Kural:** Validator **durumsuz**, **DB'ye bakmaz**, **tek sorumluluk = alan doğrulaması**. Kural bazlı doğrulama (ör. "aynı isimde workspace olamaz") repository sorgusu gerektirir → o handler içinde `Result.Fail` olarak döner, validator'a konmaz (`G7`).

### §3.5 Pipeline Davranışları — Sıra Tablosu

| # | Davranış | Ne Yapar | Sıra Gerekçesi | Başarısızlıkta |
|---|----------|----------|----------------|----------------|
| 1 | `LoggingBehavior` | Correlation id + süre + tip | En başta: her istek loglanır, hatta hatalı olanlar da | Asla durdurmaz |
| 2 | `ValidationBehavior` | `IValidator<T>` çalıştırır | DB'ye girmeden önce | `Result.Fail` + **durdur** |
| 3 | `TransactionBehavior` | `IDbTransaction` açar | Yalnız Command; Query'de pasif | Rollback + `Result.Fail` |
| 4 | `RetryBehavior` | Zaman aşımını 3 kez dener | Yalnız altyapı hatası (`TimeoutException`) | 3. denemede rethrow |
| 5 | Handler | Gerçek iş | En sonda: temiz ortam | `Result<T>` döner |

**Davranış kayıt sırası** (Host): `UseLogging → UseValidation → UseTransaction → UseRetry → UseHandler`. Kayıt ters yazılırsa sıra ters çalışır; §5.3/5 ile denetlenir.

### §3.6 Mediatör Kaydı ve Dispatch (dolu örnek)

````csharp
// src/VersaCoder.Application/Common/Messaging/Mediator.cs
public sealed class Mediator
{
    private readonly IServiceProvider _services;
    private readonly ILogger<Mediator> _logger;
    private static readonly Dictionary<Type, Type> Map = BuildMap(); // assembly taraması, 1 kez

    public Mediator(IServiceProvider services, ILogger<Mediator> logger)
    { _services = services; _logger = logger; }

    public async Task<Result<TResult>> SendAsync<TResult>(
        IQuery<TResult> query, CancellationToken ct)
    {
        var handlerType = Map[query.GetType()];
        dynamic handler = _services.GetRequiredService(handlerType);
        var pipeline = new QueryPipeline<dynamic>(handler, _logger);
        return await pipeline.RunAsync((dynamic)query, ct);
    }

    public async Task<Result<TResult>> SendAsync<TResult>(
        ICommand<TResult> command, CancellationToken ct)
    {
        var handlerType = Map[command.GetType()];
        dynamic handler = _services.GetRequiredService(handlerType);
        var pipeline = new CommandPipeline<dynamic>(handler, _logger);
        return await pipeline.RunAsync((dynamic)command, ct);
    }
}
````

**Kayıt kuralı:** Command/Query tipleri **Transient**, handler **Transient**, validator **Transient**, pipeline bileşenleri **Singleton** (durumsuz). Scoped yalnızca repository'dir (`[[backend/service-template]]` §3.6 ile aynı tablo).

### §3.7 `Result<T>` Hata Dönme Biçimi

```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }                       // başarısızsa default
    public IReadOnlyList<ValidationError> Errors { get; }
    public static Result<T> Ok(T value);
    public static Result<T> Fail(string code, string message);
    public static Result<T> Fail(IEnumerable<ValidationError> errors);
}
```

| Durum | Dönüş | İstisna Fırlatır mı? | Örnek |
|-------|-------|------------------------|-------|
| Başarı | `Result.Ok(value)` | Hayır | `workspace.id` |
| Alan hatası (validator) | `Fail(errors[])` | Hayır | `workspace.name.required` |
| Bulunamayan | `Fail("*.not_found")` | Hayır | `workspace.not_found` |
| Kural ihlali | `Fail("*.conflict")` | Hayır | `workspace.already_archived` |
| DB zaman aşımı | İstisna yukarı | **Evet** | `TimeoutException` |
| İptal (`ct`) | İstisna yukarı | **Evet** | `OperationCanceledException` |

**Kural:** Handler **yalnız** Domain/repository hatalarını `Result`'a çevirir; altyapı hatasını yutmaz (`G8`).

Test edilebilirlik kuralı — istisna fırlatma sınırlı tutulur:

| Katman | İstisna Fırlatır mı? | Neden |
|--------|------------------------|-------|
| Validator | Hayır | Alan hatası veri durumudur |
| Handler (domain/repo) | Hayır | İşlenen sonuç `Result`'tır |
| Repository (altyapı) | **Evet** | Yeniden deneme/pipeline kararı gerekir |
| Mediatör/pipeline | **Evet** | Sınır: log + yukarı |
| Host (UI/CLI) | Hayır — yakalar | Kullanıcıya mesaj gösterir |

### §3.8 Test Stratejisi

| Senaryo | Araç | Ne Doğrulanır | Hedef |
|---------|------|---------------|-------|
| Mutlu yol | xunit + **Moq** (NSubstitute kabul) | `IsSuccess = true` + değer | her handler'da |
| Validator hatalı | xunit | `Errors` kodları birebir | ≥3 hata durumu |
| Repo bulunamadı | `I*Repository` mock | `*.not_found` | her Query'de |
| İptal | `CancellationTokenSource` | `OperationCanceledException` | zorunlu |
| Pipeline sırası | Test double log sırası | 1→2→3→4→5 (`§3.5`) | zorunlu |
| Retry | Sahte `TimeoutException` | 3 deneme, sonra rethrow | zorunlu |
| Coverage | `dotnet test` + coverage | satır/dal | **≥ %90** |

**Kabul kriterleri:**

- [ ] AC-1 Her handler `Task<Result<T>>` + `CancellationToken` imzasında
- [ ] AC-2 Command'da validator var; Query'de filtre validator'ı var
- [ ] AC-3 Handler içinde SQL/`IDbConnection` yok
- [ ] AC-4 Pipeline kaydı §3.5 sırasında (test edilmiş)
- [ ] AC-5 `dotnet test` yeşil, coverage ≥ %90
- [ ] AC-6 Yeni paket eklenmedi (MediatR/FluentValidation yok)
- [ ] AC-7 §5.3 altı komutu beklenen çıktıda

---

## §4 Kurallar

### §4.1 Handler Yazım Sırası

1. **Sözleşme önce:** `ICommand<TResult>` / `IQuery<TResult>` tiplerini yaz.
2. **Validator:** alan hatalarını topla (`§3.4`).
3. **Handler:** en küçük doğrulama → repo çağrısı → `Result`.
4. **Kayıt:** Mediatör map'i + DI (Transient).
5. **Test:** mutlu yol + 3 hata yolu + iptal.
6. **Ölçüm:** §5.3 altı komut.

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Çıktı | Süre |
|-------|-----|-------|------|
| 1 | `developer` — command/query + handler + validator | kod | 40 dk |
| 2 | `developer` — test yazımı | ≥7 test metodu | 35 dk |
| 3 | `reviewer` — §6.1 checklist (20 madde) | ✅/RED | 15 dk |
| 4 | `testing` — `dotnet test` + coverage | %90 kanıtı | 20 dk |
| 5 | `docs-writer` — `log.md` + envanter | kayıt | 5 dk |

**Toplam ≈ 115 dk/handler.** 3 RED sonrası iş durur; gerekçe `architect`'e yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Handler / command | 1 handler ↔ 1 command | çoğalt → `G10` |
| Handler gövdesi | ≤ 30 satır | servise böl (`service-template`) |
| Validator hata sayısı | ≤ 8 | fazlası → DTO tasarımı |
| Pipeline davranışı | 4 (log/val/tx/retry) | 5. ekleme `architect` onayı ister |
| Coverage | ≥ %90 | `G11` |
| Satır aralığı | 480–660 | `G12` |
| Dosya boyu | 15–32 KB | `G12` |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm | Ek İşlem |
|-----------|-------|----------|
| Yorum/dolgu düzeltmesi | aynı | §5.3 yeniden |
| Yeni validator kuralı | `minor` | test + Quality Report |
| Pipeline sırası değişikliği | **`major`** | `architect` onayı + §5.3/5 |
| `Result<T>` alan ekleme | `minor` | tüm handler'lar taranır |
| Komut silme | `major` | UI/CLI çağıranları + envanter |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Handler yerine servis/sınıf çifti yazmak (1↔1 kuralı) | **DUR** — birleştirilir |
| G2 | Dönüş tipi `Result<T>` değil (`void`, `T`, `bool`) | **DUR** — imza düzeltilir |
| G3 | Handler içinde SQL / `IDbConnection` / Dapper | **DUR** — repository'ye |
| G4 | Handler içinde domain invariant `if`'i | **DUR** — Domain entity'ye |
| G5 | `.Result` / `.Wait()` / `GetAwaiter()` | **DUR** — async zincir kırık |
| G6 | Yan etki taşıyan sorguya `IQuery` vermek | **DUR** → `ICommand` |
| G7 | Validator'a DB sorgusu koymak | **DUR** — handler'a taşınır |
| G8 | Altyapı istisnasını `Result`'a çevirip yutmak | **DUR** — log + yukarı |
| G9 | FluentValidation/MediatR paketi eklemek | **RED** — saf iskelet zorunlu |
| G10 | Aynı command için 2 handler | **DUR** — kayıt belirsizliği |
| G11 | Coverage < %90 veya test kırmızı | iş birleştirilemez |
| G12 | Frontmatter 7 alan eksiksiz değil | dosya RED |
| G13 | `CoreMusic` adı geçerse (ürün = VERSA CODER) | dosya RED |
| G14 | Mojibake / Latin-1/CP1254 | UTF-8 BOM'suz yeniden yazılır |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış (tam tablo)

| # | Adım | Girdi | Çıktı | Yapan | Süre | Kapı |
|---|------|-------|-------|-------|------|------|
| 1 | Niyeti ayrıştır | gereksinim | Command mı Query mi (§1.3) | developer | 5 dk | kararsızsa Command |
| 2 | Sınır testi | §2.2 S1–S5 | 5 cevap | developer | 3 dk | S3/S4 `EVET` ise dur |
| 3 | Sözleşme | §3.2 | command/query tipi | developer | 10 dk | — |
| 4 | Validator | §3.4 | validator sınıfı | developer | 15 dk | — |
| 5 | Handler | §3.3 akışı | handler | developer | 25 dk | — |
| 6 | Mediatör/DI kaydı | §3.6 | kayıt satırları | developer | 10 dk | — |
| 7 | Test | §3.8 | ≥7 test | testing | 35 dk | — |
| 8 | Ölçüm | §5.3/1–/6 | 6 çıktı | developer | 5 dk | G5, G9 |
| 9 | İnceleme | §6.1 (20 madde) | ✅/RED | reviewer | 15 dk | 3 RED → dur |
| 10 | Quality Report | §5.4 | rapor | reviewer | 5 dk | — |
| 11 | Senkron | `log.md` + envanter | kayıt | docs-writer | 5 dk | — |

**Zaman bütçesi:** 1–7 ≈ 103 dk, 8–11 ≈ 30 dk → **≈ 133 dk/handler** (%15 tampon dahil).

### §5.2 Hata Modları

| Kod | Hata modu | Belirti | Kök neden | Düzeltme |
|-----|-----------|---------|-----------|----------|
| H1 | Sıra ters | hatalı istek DB'ye giriyor | kayıt sırası ters | §3.5 kaydı yeniden |
| H2 | Validator hiç çalışmıyor | hatalı veri geçiyor | validator kaydı yok | DI'a ekle |
| H3 | Query transaction'da | sorgu kilitleniyor | davranış Query'ye de uygulanmış | Query'de pasif yap |
| H4 | Çift kayıt | retry transaction içinde | sıra 3–4 ters | retry'yi dışa al |
| H5 | İstisna yutulmuş | `IsSuccess=false` ama log yok | `G8` ihlali | log + rethrow |
| H6 | `.Result` kullanımı | §5.3/2 > 0 | senkron-over-async | `async`/`await` |
| H7 | Paket sızıntısı | `.csproj`'de MediatR | hız diye eklenmiş | saf iskelete dön |
| H8 | 2 handler aynı komut | kayıt çakışması | kopyala-yapıştır | `G10` — sil |
| H9 | Coverage düşüşü | < %90 | hata yolları testsiz | §3.8 matrisi |
| H10 | Validator 2. kez çalışıyor | iki kat hata listesi | davranış + handler kendi de kontrol ediyor | tek sahip: davranış |
| H11 | Query cache'e alınmamış | aynı sorgu 2× DB'den | `QueryBehavior` kaydı yok | §3.5 kaydını tamamla |
| H12 | Yeni command test'siz | coverage sessiz düşer | §3.8 adımı atlanmış | mutlu yol + 3 hata yolu |

### §5.3 Salt-Okunur Doğrulama Komutları (6)

```powershell
# 1) HANDLER + SÖZLEŞME SAYISI — hedef: ≥ 1 ve birbirine eşit
(Get-ChildItem -Path 'C:\versacoder\src\VersaCoder.Application' -Recurse -Filter '*Handler.cs' -ErrorAction SilentlyContinue | Measure-Object).Count
(Get-ChildItem -Path 'C:\versacoder\src\VersaCoder.Application' -Recurse -Filter '*Command.cs' -ErrorAction SilentlyContinue | Measure-Object).Count

# 2) SENKRON-OVER-ASYNC — hedef: 0
(Select-String -Path 'C:\versacoder\src\VersaCoder.Application\**\*.cs' -Pattern '\.Result|\.Wait\(\)|GetAwaiter\(\)' -ErrorAction SilentlyContinue | Measure-Object).Count

# 3) YASAK PAKET (MediatR / FluentValidation / EF Core) — hedef: 0
(Get-ChildItem -Path 'C:\versacoder\src' -Recurse -Include '*.csproj' -ErrorAction SilentlyContinue |
  Select-String -Pattern 'MediatR|FluentValidation|EntityFramework' -ErrorAction SilentlyContinue | Measure-Object).Count

# 4) PİPELİNE SIRA DENETİMİ — 5 davranış dosyasının varlığı ve sıra yorumu
Get-ChildItem -Path 'C:\versacoder\src\VersaCoder.Application\Common\Pipeline' -Filter '*.cs' -ErrorAction SilentlyContinue |
  Select-Object -ExpandProperty Name

# 5) HANDLER İÇİNDE SQL / IDBCONNECTION — hedef: 0
(Select-String -Path 'C:\versacoder\src\VersaCoder.Application\**\*Handler.cs' -Pattern 'SELECT\s|INSERT\s|IDbConnection|ExecuteNonQuery' -ErrorAction SilentlyContinue | Measure-Object).Count

# 6) MOJİBAKE — hedef: 0
(Select-String -Path 'C:\versacoder\.ai\.templates\backend\handler-template.md' -Pattern '[\u00C3\u00C2]|\uFFFD' | Measure-Object).Count

# 7) (ek) ÖLÇÜM — satır/KB aralığı 480-660 / 15-32
$f = 'C:\versacoder\.ai\.templates\backend\handler-template.md'
$satir = (Get-Content -LiteralPath $f -Encoding UTF8 | Measure-Object -Line).Lines
$kb = [math]::Round((Get-Item -LiteralPath $f).Length / 1KB, 1)
"satır=$satir  KB=$kb"
```

```powershell
# 8) TEST — salt-okunur (kaynak dokunmaz)
dotnet test 'C:\versacoder\tests\VersaCoder.Tests' --filter 'FullyQualifiedName~Handler' --no-restore
```

### §5.4 Quality Report

```markdown
## Handler Kalite Raporu — {{DOSYA_ADI}} (YYYY-MM-DD)

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Satır / KB | {{N}} / {{KB}} | 480–660 · 15–32 | 🟢/🔴 |
| Command ↔ Handler | {{N}}/{{N}} | eşit | 🟢/🔴 |
| `.Result`/`.Wait()` (§5.3/2) | {{N}} | 0 | 🟢/🔴 |
| Yasak paket (§5.3/3) | {{N}} | 0 | 🟢/🔴 |
| Handler içinde SQL (§5.3/5) | {{N}} | 0 | 🟢/🔴 |
| Pipeline testi (sıra 1→5) | {{PASS}} | yeşil | 🟢/🔴 |
| Coverage | {{%}} | ≥ 90 | 🟢/🔴 |
| Guardrail G1–G14 | ihlal {{N}} | 0 | 🟢/🔴 |

**Karar:** 🟢 inceleme açıldı / 🔴 düzeltmeye gönderildi ({{G-KOD}})
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

- [ ] 01 Frontmatter 7 alan eksiksiz; `authority: SSOT`
- [ ] 02 H1 + `**Zorunlu Bağlantılar:**` 6 bağlantı
- [ ] 03 §1.1–§1.4 dolu; §1.2'de `⚠️ VERIFICATION REQUIRED` satırı var
- [ ] 04 §1.2 iddia/kanıt/durum — yorum yok
- [ ] 05 §1.3 Command/Query tablosu ≥ 10 satır
- [ ] 06 §2.1 dosya tipi → sorumlu ≥ 10 satır
- [ ] 07 §2.2 S1–S5 cevaplanmış
- [ ] 08 §3.2 sözleşmeler `ICommand/IQuery/ICommandHandler/IQueryHandler` içeriyor
- [ ] 09 §3.3 kutu 66 sütun + `<== KAPI:` etiketi var
- [ ] 10 §3.4 validator saf (DB erişimi yok) — kodda sorgu yok
- [ ] 11 §3.5 beş davranış sırası + gerekçe dolu
- [ ] 12 §3.6 mediatör kaydı + yaşam döngüsü yazıyor
- [ ] 13 §3.7 `Result<T>` tablosu ≥ 6 satır; istisna ayrımı net
- [ ] 14 §3.8 test stratejisi Moq/NSubstitute + %90
- [ ] 15 §4.5 G1–G14 = 14 satır, "İhlal Sonucu" dolu
- [ ] 16 §5.1 11 adım + zaman bütçesi
- [ ] 17 §5.3 ≥ 5 salt-okunur komut (`Set-`/`Remove-` yok)
- [ ] 18 §5.4 Quality Report bloğu var
- [ ] 19 §6.5 şablona özgü kontroller doldurulmuş
- [ ] 20 §7.2 Red Team ≥ 10 satır

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Tetikleyici | Aksiyon | Sorumlu |
|--------|-------------|---------|---------|
| RS-1 | §5.3/3 > 0 (yasak paket) | **DUR** — paket kaldırılır | reviewer |
| RS-2 | §5.3/5 > 0 (handler'da SQL) | **DUR** — repository'ye taşınır | reviewer |
| RS-3 | §5.3/2 > 0 (`.Result`) | **DUR** — async zincir onarılır | developer |
| RS-4 | Pipeline sırası testi kırmızı | **DUR** — §3.5 kaydına dön | testing |
| RS-5 | Coverage < %90 | iş birleştirilemez | testing |
| RS-6 | `Result<T>` olmayan dönüş | **DUR** — G2 | reviewer |
| RS-7 | Mojibake > 0 | dosya UTF-8 ile yeniden yazılır | docs-writer |
| RS-8 | Satır/KB aralığı dışı | **DUR** — G12 | reviewer |

### §6.3 Sürüm / Geçiş Kuralları

| Değişiklik | Sürüm | Durum | Ek |
|-----------|-------|-------|----|
| Dolgu/hizalama düzeltmesi | `patch` | `active` | §5.3 yeniden |
| Yeni alt bölüm (`§x.y`) | `minor` | `active` | Quality Report |
| Pipeline davranışı ekleme | `minor` | `review` | `architect` onayı |
| Pipeline **sırası** değişikliği | **`major`** | `review` | tüm handler testleri |
| Bölüm silme / yeniden adlandırma | `major` | `review` | bağlantı + `log.md` |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/backend/service-template.md` | ≥2 use case'e geçiş kapısı |
| `.ai/.templates/backend/repository-template.md` | handler'ın çağırdığı arayüz |
| `.ai/.templates/architecture/ascii-diagram-template.md` | §3.3 kutu kuralı (66 sütun) |
| `.ai/.templates/index.md` | envanter satırı #18 |
| `.ai/architecture/index.md` | katman gerçeği (⚠️ doğrulanacak) |
| `.ai/.rules/coding-standards.md` | kod standardı (⚠️ doğrulanacak) |

### §6.5 Şablona Özgü Kontroller

- [ ] K1 Command ile Handler sayısı 1:1 (§5.3/1)
- [ ] K2 Validator sınıfları durumsuz (field yok, ctor'da bağımlılık yok)
- [ ] K3 Query handler'ında yazma repo metodu çağrısı yok (`Add`/`Update`/`Delete`)
- [ ] K4 Pipeline dosyaları 4 adet; 5. dosya varsa `architect` onayı
- [ ] K5 Hata kodu deseni `<kaynak>.<neden>` (nokta ile ayrık)
- [ ] K6 İptal senaryosu için test var (`CancellationTokenSource`)
- [ ] K7 Mediatör map'i `static` ve assembly taraması bir kez çalışır
- [ ] K8 Validator hataları `ValidationError` listesi olarak dönüyor (tek string değil)
- [ ] K9 `IQuery` handler'ı `Task.WhenAll` ile paralel çağrılabilecek durumsuz
- [ ] K10 Pipeline'daki her davranış için en az 1 test var (5 davranış → 5 test)

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| # | Kaynak | Yol | Ne İçin | Kanıt |
|---|--------|-----|---------|-------|
| 1 | Şablon envanteri | `.ai/.templates/index.md` | #18 kayıt + §3.7 kalite sözleşmesi | ✅ OKUNDU (38.816 B) |
| 2 | Kutu kuralı | `.ai/.templates/architecture/ascii-diagram-template.md` | §3.3 çizimi, §5.3/1 | ✅ OKUNDU (38.121 B) |
| 3 | Servis şablonu | `.ai/.templates/backend/service-template.md` | §1.1 geçiş kapısı | ✅ OKUNDU |
| 4 | Repository şablonu | `.ai/.templates/backend/repository-template.md` | §3.7 altyapı hatası | ⚠️ VERIFICATION REQUIRED |
| 5 | Katman gerçeği | `.ai/architecture/index.md` | §3.2 namespace | ⚠️ VERIFICATION REQUIRED |
| 6 | Kod standardı | `.ai/.rules/coding-standards.md` | §4.3 ölçek | ⚠️ VERIFICATION REQUIRED |
| 7 | ADR-004 veri yığını | `.ai/adr/ADR-004-*` | G3 (EF Core yasağı) | ⚠️ VERIFICATION REQUIRED |
| 8 | Gate planı | `.ai/WORKFLOW.md` | §5.1 kapı sütunu | ⚠️ VERIFICATION REQUIRED |
| 9 | Test standardı | `.ai/.templates/testing/unit-test-template.md` | §3.8 | ⚠️ VERIFICATION REQUIRED |

### §7.2 Red Team — Sık Yapılan Hatalar (10 satır)

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | MediatR/FluentValidation paketi eklemek | Karanlık davranış: sıra görünmez, sürüm riski | §3.2/§3.4 saf iskelet (G9) |
| 2 | Pipeline sırasını "öyle daha mantıklı" diye değiştirmek | İptal/çift kayıt gibi yan etkiler sessiz doğar | §3.5 tablosu sabit + §5.3/5 |
| 3 | Validator'a DB sorgusu yazmak | Validator tekrar çalıştırıldığında 2. sorgu | Alan doğrulaması validator'da, kural handler'da (G7) |
| 4 | Handler'a SQL koymak | Katman ihlali; repository'siz test edilemez | `I*Repository` çağrısı (G3) |
| 5 | Dönüşü `void`/`bool` yapmak | Hata kodu kaybolur, UI mesaj üretemez | `Result<T>` (G2) |
| 6 | Altyapı hatasını `Result.Fail`'e çevirmek | Yeniden deneme/pipeline anında duramaz | log + rethrow (G8) |
| 7 | `.Result` ile beklemek | `net48` WinForms iş parçacığında deadlock | `async`/`await` + `ct` (G5) |
| 8 | Yan etki taşıyan sorguya `IQuery` demek | Transaction/cache davranışı yanlış uygulanır | §1.3 sorusu: DB değişiyor mu? (G6) |
| 9 | Aynı komutu iki handler'a bağlamak | Kayıt sırasına göre sonuç değişir | 1:1 kuralı (G10) |
| 10 | §5.3 ölçümü yapmadan "pipeline doğru" demek | Kanıtsız iddia (master prompt §6/5) | 8 komutun çıktısını rapora ekle |

**SSUT beyanı:** Bu dosya CQRS handler iskeletinin **tek** kaynağıdır. Çoğaltan her dosya bu dosyaya bağlanır; içerik kopyalanmaz.
