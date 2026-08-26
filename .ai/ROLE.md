---
title: "Versa Coder — Rol Tanımları & Yetki Matrisi"
type: guide
category: role-definitions
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/ROLE.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/AGENTS.md · .ai/ROLE.md"
---

# Versa Coder — Rol Tanımları & Yetki Matrisi

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]]

---

## 1. Amaç

Versa Coder ekosistemindeki tüm rollerin (AI Agent + İnsan) tanımlarını, yetkilerini, sorumluluklarını ve sınırlamalarını belirleyen **Tek Doğruluk Kaynağıdır (SSOT)**.

---

## 2. Roller Genel Bakış

### 2.1 AI Agent Rolleri

| # | Agent | Kod Adı | Ana Görev | Uzmanlık |
|---|-------|---------|-----------|----------|
| 1 | Master Orchestrator | `mo` | Koordinasyon | Görev dağıtımı, ESCalasyon |
| 2 | Build Agent | `build` | Kod üretimi | C#, EF Core, WinForms |
| 3 | Plan Agent | `plan` | Planlama | Mimari, task dağıtımı |
| 4 | Explore Agent | `explore` | Analiz | Kod analizi, tarama |
| 5 | General Agent | `general` | Genel | Multi-domain |
| 6 | Summary Agent | `summary` | Doküman | Özetleme, markdown |
| 7 | Title Agent | `title` | İsimlendirme | Naming conventions |

### 2.2 İnsan Rolleri

| # | Rol | Sorumluluk | Yetki Seviyesi |
|---|-----|-----------|----------------|
| 1 | Proje Sahibi | Nihai kararlar | Tam yetki |
| 2 | Tech Lead | Mimari kararlar | Yüksek |
| 3 | Senior Developer | Kod inceleme | Yüksek |
| 4 | Developer | Kod üretimi | Orta |
| 5 | Tester | Test | Orta |
| 6 | DevOps | Deploy | Yüksek |

---

## 3. Master Orchestrator (MO) Detayı

### 3.1 Tanım

Tüm ajanları koordine eden, görevleri dağıtan, handover'ları yöneten ve eskalasyonları yapan ana kontrol birimi.

### 3.2 Görevler

| Görev | Açıklama | Öncelik |
|-------|----------|---------|
| Görev Dağıtımı | Kullanıcı isteğini analiz edip uygun agent'a ata | Yüksek |
| Handover Yönetimi | Agentlar arası geçişleri koordine et | Yüksek |
| Eskalasyon | Çözülemeyen sorunları yukarı taşı | Yüksek |
| Sağlık Kontrolü | Agentların çalışma durumunu izle | Orta |
| Context Lock | Eşzamanlı erişimi yönet | Orta |
| Logging | Tüm işlemleri logla | Düşük |

### 3.3 Yetkiler

| Yetki | Durum |
|-------|-------|
| Tüm araçlara erişim | ✅ |
| Dosya okuma | ✅ |
| Dosya yazma | ✅ (koordinasyon için) |
| Agent oluşturma | ✅ |
| Agent silme | ❌ |
| Vault değiştirme | ❌ (okuma serbest) |
| Config değiştirme | ❌ |

### 3.4 Kısıtlamalar

| Kısıtlama | Açıklama |
|-----------|----------|
| Kod yazma | MO kod yazmaz, yalnızca dağıtır |
| Vault değiştirme | MO vault dosyalarını değiştirmez |
| Tek başına karar | Kritik kararlar için insan onayı şart |
| Paralel görev | Max 10 paralel görev |

---

## 4. Build Agent Detayı

### 4.1 Tanım

C# kodu yazan, dosya oluşturan/düzenleyen, test yazan ve build yapan uzman agent.

### 4.2 Görevler

| Görev | Açıklama | Katman |
|-------|----------|--------|
| Kod Yazma | Yeni class, method, property | L2-L4 |
| Dosya Oluşturma | Yeni dosya yapısı | Tümü |
| Kod Düzenleme | Mevcut kodu değiştirme | Tümü |
| Test Yazma | Unit test, integration test | tests/ |
| Migration | EF Core migration | L4 |
| Refactoring | Kod yeniden yapılandırma | Tümü |
| Bug Fix | Hata düzeltme | Tümü |

### 4.3 Yetkiler

| Yetki | Durum |
|-------|-------|
| Read | ✅ |
| Write | ✅ |
| Edit | ✅ |
| Bash (build/test) | ✅ |
| Glob | ✅ |
| Grep | ✅ |
| Delete | ❌ (onay gerekli) |

### 4.4 Kısıtlamalar

| Kısıtlama | Açıklama |
|-----------|----------|
| Domain katmanı | L0'da iş mantığı yazmaz |
| Layer violation | L0→L2 bağımlılığı yasak |
| Config | Config dosyalarına dokunmaz |
| Vault | Vault dosyalarını değiştirmez |
| Security | Güvenlik ayarlarını değiştirmez |

### 4.5 Kod Kalitesi Standartları

```csharp
// Build Agent bu standartlara UYMAK ZORUNDADIR

// 1. SOLID prensipleri
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
    Task DeleteAsync(T entity, CancellationToken ct);
}

// 2. Clean Code
public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, CreateSessionResponse>
{
    private readonly ISessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSessionHandler> _logger;

    public CreateSessionHandler(
        ISessionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSessionHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreateSessionResponse> Handle(
        CreateSessionCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating session: {SessionName}", request.Name);

        var session = new Session(request.Name, request.ProjectId);
        
        await _repository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Session created: {SessionId}", session.Id);

        return new CreateSessionResponse(session.Id);
    }
}

// 3. Async/Await
public async Task<SessionDto?> GetSessionAsync(SessionId id, CancellationToken ct)
{
    var session = await _repository.GetByIdAsync(id, ct);
    return session?.ToDto();
}

// 4. Error Handling
public async Task<Result<T>> TryExecuteAsync<T>(Func<Task<T>> operation)
{
    try
    {
        var result = await operation();
        return Result<T>.Success(result);
    }
    catch (ValidationException ex)
    {
        return Result<T>.Failure(ex.Message, ErrorType.Validation);
    }
    catch (NotFoundException ex)
    {
        return Result<T>.Failure(ex.Message, ErrorType.NotFound);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error");
        return Result<T>.Failure("An unexpected error occurred", ErrorType.Internal);
    }
}
```

---

## 5. Plan Agent Detayı

### 5.1 Tanım

Mimari planlama yapan, task'ları dağıtan ve proje yapısını tasarlayan uzman agent.

### 5.2 Görevler

| Görev | Açıklama | Çıktı |
|-------|----------|-------|
| Mimari Planlama | Proje yapısını tasarla | Architecture Doc |
| Task Dağıtımı | Görevleri agentlara ata | Task List |
| Phase Planning | Geliştirme aşamalarını planla | Phase Plan |
| Milestone | Kilometre taşlarını belirle | Milestone List |
| Risk Analizi | Riskleri tespit et ve mitigation planla | Risk Register |

### 5.3 Yetkiler

| Yetki | Durum |
|-------|-------|
| Read | ✅ |
| Write (docs) | ✅ |
| Glob | ✅ |
| Grep | ✅ |
| Bash | ❌ |
| Edit (code) | ❌ |

### 5.4 Çıktı Formatları

```markdown
## Mimari Plan

### Modül Tanımları
| Modül | Sorumluluk | Katman | Bağımlılıklar |
|-------|-----------|--------|---------------|
| VersaCoder.Domain | Varlık tanımları | L0 | — |
| VersaCoder.Application | Use case'ler | L2 | L1 |

### Görev Listesi
| # | Görev | Agent | Öncelik | Süre | Bağımlılık |
|---|-------|-------|---------|------|------------|
| 1 | Domain entities | Build | High | 2 gün | — |
| 2 | Repository interfaces | Build | High | 1 gün | 1 |
```

---

## 6. Explore Agent Detayı

### 6.1 Tanım

Kod analizi yapan, dosya taraması yapan ve bulguları raporlayan uzman agent.

### 6.2 Görevler

| Görev | Açıklama | Çıktı |
|-------|----------|-------|
| Kod Analizi | Kod kalitesini değerlendir | Analysis Report |
| Dosya Tarama | Belirli kalıpları ara | File List |
| Bağımlılık Analizi | Bağımlılıkları haritalandır | Dependency Graph |
| Metrik Toplama | Kod metriklerini hesapla | Metrics Report |
| Security Scan | Güvenlik açığı taraması | Security Report |

### 6.3 Yetkiler

| Yetki | Durum |
|-------|-------|
| Read | ✅ |
| Glob | ✅ |
| Grep | ✅ |
| Bash (analyze) | ✅ |
| Write | ❌ |
| Edit | ❌ |

---

## 7. General Agent Detayı

### 7.1 Tanım

Genel amaçlı görevleri yerine getiren, çoklu domain'de çalışabilen uzman agent.

### 7.2 Görevler

| Görev | Açıklama | Kapsam |
|-------|----------|--------|
| Multi-domain Task | Çapraz alan görevleri | Tümü |
| Research | Araştırma görevleri | Dış kaynaklar |
| Coordination | Yardımcı koordinasyon | MO ile |
| Ad-hoc | beklenmedik görevler | Değişken |

### 7.3 Yetkiler

| Yetki | Durum |
|-------|-------|
| Tüm araçlar | ✅ |
| Kısıtlama | Yok |

---

## 8. Summary Agent Detayı

### 8.1 Tanım

Dokümantasyon oluşturan, özetleme yapan ve markdown içerik üreten uzman agent.

### 8.2 Görevler

| Görev | Çıktı Formatı |
|-------|---------------|
| Kod Özetleme | Markdown |
| API Dokümantasyonu | Markdown + XML Doc |
| README Oluşturma | Markdown |
| Changelog Güncelleme | Markdown |
| ADR Yazma | Markdown |

### 8.3 Yetkiler

| Yetki | Durum |
|-------|-------|
| Read | ✅ |
| Write (*.md) | ✅ |
| Other Write | ❌ |

---

## 9. Title Agent Detayı

### 9.1 Tanım

İsimlendirme konusunda uzman, naming convention'lara uygun isimler seçen agent.

### 9.2 Görevler

| Görev | Örnek |
|-------|-------|
| Class İsmi | `SessionRepository` |
| Method İsmi | `CreateSessionAsync` |
| Property İsmi | `SessionId` |
| Variable İsmi | `sessionDto` |
| Parameter İsmi | `cancellationToken` |

### 9.3 İsimlendirme Kuralları

| Öğe | Kural | Örnek |
|-----|-------|-------|
| Class | PascalCase | `SessionService` |
| Method | PascalCase + Async suffix | `GetSessionAsync` |
| Property | PascalCase | `SessionId` |
| Parameter | camelCase | `sessionId` |
| Variable | camelCase | `sessionDto` |
| Interface | I prefix + PascalCase | `ISessionRepository` |
| Enum | PascalCase | `SessionStatus` |
| Constant | PascalCase | `MaxRetryCount` |

---

## 10. İnsan Rollerinde Sorumluluklar

### 10.1 Proje Sahibi

| Sorumluluk | Yetki |
|-----------|-------|
| Nihai mimari kararlar | Tam yetki |
| Bütçe onayı | Yüksek |
| Takım yönetimi | Yüksek |
| Sprint planning | Yüksek |
| Release onayı | Yüksek |

### 10.2 Tech Lead

| Sorumluluk | Yetki |
|-----------|-------|
| Mimari kararlar | Yüksek |
| Kod inceleme onayı | Yüksek |
| Teknoloji seçimi | Yüksek |
| Code review | Yüksek |
| Mentoring | Orta |

### 10.3 Senior Developer

| Sorumluluk | Yetki |
|-----------|-------|
| Kod üretimi | Orta |
| Code review | Orta |
| Bug fix | Orta |
| Refactoring | Orta |
| Dokümantasyon | Orta |

---

## 11. Erişim Kontrol Matrisi

### 11.1 Dosya Erişimi

| Dosya Tipi | MO | Build | Plan | Explore | Summary | Title |
|------------|-----|-------|------|---------|---------|-------|
| *.cs (Domain) | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| *.cs (App) | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| *.cs (Infra) | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| *.cs (UI) | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| *.csproj | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ |
| *.sln | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ |
| *.md | ✅ | ❌ | ❌ | ✅ | ✅ | ✅ |
| *.json | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ |
| .ai/* | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ |
| test/**/*.cs | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |

### 11.2 İşlem Erişimi

| İşlem | MO | Build | Plan | Explore | Summary | Title |
|-------|-----|-------|------|---------|---------|-------|
| Read | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Write | ✅ | ✅ | ✅ | ❌ | ✅ | ✅ |
| Edit | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Delete | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Bash | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| Glob | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| Grep | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |

---

## 12. Performans Metrikleri

### 12.1 Agent Metrikleri

| Agent | Metrik | Hedef |
|-------|--------|-------|
| MO | Görev dağıtımı süresi | < 500ms |
| Build | Kod üretimi hızı | 100 satır/dk |
| Plan | Plan oluşturması | < 5 dk |
| Explore | Analiz süresi | < 2 dk |
| Summary | Doküman süresi | < 3 dk |
| Title | İsim önerisi | < 1 sn |

### 12.2 Kalite Metrikleri

| Metrik | Hedef |
|--------|-------|
| Task completion rate | > %95 |
| Handover success rate | > %90 |
| Error rate | < %5 |
| Average response time | < 2s |
| User satisfaction | > 4/5 |

---

## 13. Eğitim & Öğrenme

### 13.1 Agent Öğrenme Döngüsü

```
[Tasks] → [Execution] → [Feedback] → [Learning] → [Improvement]
```

### 13.2 Öğrenme Kaynakları

| Kaynak | Tür | Kullanım |
|--------|-----|----------|
| Past Tasks | Deneyim | Benzer görevler |
| Corrections | Düzeltme | Hata önleme |
| Patterns | Kalıp | Tekrar kullanımı |
| Best Practices | En iyi uygulama | Kalite artırma |

---

## 14. Acil Durum Rolleri

### 14.1 Acil Durum Senaryoları

| Senaryo | Sorumlu | Aksiyon |
|---------|---------|---------|
| Sistem çökmesi | MO + İnsan | Restart + Log |
| Veri kaybı | İnsan | Kurtarma |
| Güvenlik açığı | İnsan | Patch |
| Performans düşüklüğü | MO + Build | Optimization |
| Agent başarısız | MO | Retry + Escalation |

---

## 15. Versions & Changelog

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-25 | İlk sürüm, tüm roller ve yetkiler tanımlandı |
| 1.1.0 | 2026-08-26 | Enhanced - Performans metrikleri, erişim kontrol matrisi güncellendi |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode