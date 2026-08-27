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

<<<<<<< HEAD
## 15. Agent State Machine

### 15.1 Durum Tanımları

| Durum | Tanım | Geçişler |
|-------|-------|----------|
| Idle | Görev bekliyor | → Assigned |
| Assigned | Görev atandı | → Executing, → Cancelled |
| Executing | Çalışıyor | → Completed, → Blocked, → Failed |
| Blocked | Engellendi | → Executing, → Escalated |
| Failed | Başarısız | → Retry, → Escalated |
| Completed | Tamamlandı | → Idle |
| Escalated | Yukarı taşındı | → Assigned |

### 15.2 Durum Geçiş Diyagramı

```
[Idle] → [Assigned] → [Executing] → [Completed]
                         ↓
                      [Blocked]
                         ↓
                      [Failed]
                         ↓
                      [Escalated]
```

### 15.3 Durum Geçiş Kuralları

```csharp
public class AgentStateMachine
{
    private AgentState _currentState = AgentState.Idle;
    
    public void TransitionTo(AgentState newState)
    {
        if (!IsValidTransition(_currentState, newState))
        {
            throw new InvalidStateException(_currentState, newState);
        }
        
        _currentState = newState;
        OnStateChange(newState);
    }
    
    private bool IsValidTransition(AgentState from, AgentState to)
    {
        return (from, to) switch
        {
            (AgentState.Idle, AgentState.Assigned) => true,
            (AgentState.Assigned, AgentState.Executing) => true,
            (AgentState.Assigned, AgentState.Cancelled) => true,
            (AgentState.Executing, AgentState.Completed) => true,
            (AgentState.Executing, AgentState.Blocked) => true,
            (AgentState.Executing, AgentState.Failed) => true,
            (AgentState.Blocked, AgentState.Executing) => true,
            (AgentState.Blocked, AgentState.Escalated) => true,
            (AgentState.Failed, AgentState.Retry) => true,
            (AgentState.Failed, AgentState.Escalated) => true,
            _ => false
        };
    }
}
```

---

## 16. Agent Communication Protocol

### 16.1 Mesaj Formatı

```csharp
public record AgentMessage
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public AgentMessageType Type { get; init; }
    public string FromAgent { get; init; } = string.Empty;
    public string ToAgent { get; init; } = string.Empty;
    public AgentPriority Priority { get; init; }
    public AgentMessagePayload Payload { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}

public record AgentMessagePayload
{
    public string Subject { get; init; } = string.Empty;
    public string Details { get; init; } = string.Empty;
    public List<string> AffectedFiles { get; init; } = new();
    public DateTime? Deadline { get; init; }
    public Dictionary<string, object> Context { get; init; } = new();
}
```

### 16.2 İletişim Akışı Diyagramı

```
Agent A → [Message] → Message Queue → [Router] → Agent B
                                    ↓
                              MO (Logger)
                                    ↓
                              log.md (Audit)
```

### 16.3 Bildirim Kanalları

| Kanal | Kullanım | Öncelik | Implementation |
|-------|----------|---------|----------------|
| log.md | Audit trail | Tümü | Append-only |
| Console | Debug bilgisi | LOW | Serilog |
| Dialog | İnsan onayı | HIGH | DevExpress Dialog |
| Alert | Kritik hatalar | CRITICAL | DevExpress Alert |

---

## 17. Agent Performance Optimization

### 17.1 Performans Metrikleri

| Metrik | Hedef | Kritik Eşik | Ölçüm |
|--------|-------|-------------|-------|
| Yanıt süresi | < 2 sn | > 5 sn | Stopwatch |
| Görev tamamlama | < 30 sn | > 60 sn | Timer |
| Hata oranı | < %1 | > %5 | Counter |
| Başarı oranı | > %95 | < %80 | Calculator |
| Memory kullanımı | < 500 MB | > 1 GB | GC.GetTotalMemory |
| CPU kullanımı | < %30 | > %80 | PerformanceCounter |

### 17.2 Optimization Stratejileri

| Strateji | Kullanım | Etki |
|----------|----------|------|
| Caching | Sık kullanılan veriler | %50 hız kazancı |
| Async/Await | I/O bound işlemler | Thread verimliliği |
| Connection Pooling | DB bağlantıları | Bağlantı yönetimi |
| Lazy Loading | Büyük nesneler | Memory optimizasyonu |
| Batch Processing | Toplu işlemler | Throughput artışı |

### 17.3 Resource Management

```csharp
public class ResourceManager : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(10, 10); // Max 10 concurrent
    private readonly ConcurrentDictionary<string, DateTime> _activeTasks = new();
    
    public async Task<T> ExecuteWithResourceAsync<T>(
        string resourceId,
        Func<Task<T>> operation,
        CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            _activeTasks.TryAdd(resourceId, DateTime.UtcNow);
            return await operation();
        }
        finally
        {
            _activeTasks.TryRemove(resourceId, out _);
            _semaphore.Release();
        }
    }
}
```
=======
## 16. Sektörel Roller & Uzmanlık Alanları

Versa Coder ekosisteminin çoklu sektör desteğini tanımlayan, her sektör için özel agent rollerini, yetki sınırlarını ve uzmanlık alanlarını belirleyen kapsamlı sektörel çerçeve.

---

### 16.1 Sektörel Agent Rolleri Genel Bakış

| # | Sektör | Birincil Agent | İkincil Agent | Öncelik | Domain |
|---|--------|---------------|---------------|---------|--------|
| 1 | Otomotiv | Build Agent | Plan Agent | Yüksek | Endüstriyel |
| 2 | İmalat | Build Agent | Explore Agent | Yüksek | Endüstriyel |
| 3 | Enerji | Build Agent | General Agent | Yüksek | Altyapı |
| 4 | Madencilik | Build Agent | Explore Agent | Yüksek | Endüstriyel |
| 5 | Tekstil | Build Agent | Plan Agent | Orta | Endüstriyel |
| 6 | Gıda | Build Agent | General Agent | Yüksek | Endüstriyel |
| 7 | IoT | Build Agent | Explore Agent | Yüksek | Teknoloji |
| 8 | Siber Güvenlik | Explore Agent | General Agent | Kritik | Teknoloji |
| 9 | Yapay Zeka | Plan Agent | Build Agent | Yüksek | Teknoloji |
| 10 | Blockchain | Build Agent | Explore Agent | Yüksek | Teknoloji |
| 11 | Robotik | Build Agent | Plan Agent | Yüksek | Teknoloji |
| 12 | Edge Computing | Build Agent | Explore Agent | Orta | Teknoloji |
| 13 | Bulut (Cloud) | Plan Agent | Build Agent | Yüksek | Teknoloji |
| 14 | Finans | Build Agent | General Agent | Kritik | Hizmet |
| 15 | Sağlık | Build Agent | Explore Agent | Kritik | Hizmet |
| 16 | Eğitim | Summary Agent | Plan Agent | Orta | Hizmet |
| 17 | Hukuk | Summary Agent | General Agent | Yüksek | Hizmet |
| 18 | Sigorta | Build Agent | Plan Agent | Yüksek | Hizmet |
| 19 | Gayrimenkul | Build Agent | Plan Agent | Orta | Hizmet |
| 20 | Kamu | Build Agent | General Agent | Yüksek | Kamu |
| 21 | Afet Yönetimi | General Agent | Build Agent | Kritik | Kamu |
| 22 | Çevre | Explore Agent | General Agent | Orta | Kamu |
| 23 | Ulaşım | Build Agent | Plan Agent | Yüksek | Kamu |
| 24 | Su Yönetimi | Build Agent | Explore Agent | Orta | Kamu |
| 25 | Metaverse | Build Agent | Explore Agent | Orta | Yeni Nesil |
| 26 | NFT | Build Agent | Plan Agent | Düşük | Yeni Nesil |
| 27 | AR/VR | Build Agent | Explore Agent | Orta | Yeni Nesil |
| 28 | Uzay Teknolojileri | Plan Agent | Build Agent | Yüksek | Yeni Nesil |
| 29 | Nanoteknoloji | Explore Agent | Build Agent | Yüksek | Yeni Nesil |
| 30 | Genetik Mühendislik | Explore Agent | General Agent | Yüksek | Yeni Nesil |
| 31 | 3D Baskı | Build Agent | Plan Agent | Orta | Yeni Nesil |
| 32 | Medya & Eğlence | Summary Agent | Build Agent | Orta | Yeni Nesil |
| 33 | E-ticaret | Build Agent | Plan Agent | Yüksek | Hizmet |
| 34 | Lojistik | Build Agent | Plan Agent | Yüksek | Hizmet |
| 35 | İklim Teknolojileri | Explore Agent | General Agent | Yüksek | Yeni Nesil |
| 36 | Biyoteknoloji | Explore Agent | General Agent | Yüksek | Yeni Nesil |
| 37 | Tarım Teknolojileri | Build Agent | Explore Agent | Orta | Endüstriyel |
| 38 | Maden İşleme | Build Agent | Explore Agent | Yüksek | Endüstriyel |
| 39 | İlaç Sanayi | Build Agent | Explore Agent | Kritik | Endüstriyel |
| 40 | Kimya Sanayi | Build Agent | Explore Agent | Yüksek | Endüstriyel |
| 41 | Demir-Çelik | Build Agent | Plan Agent | Yüksek | Endüstriyel |
| 42 | Havacılık | Build Agent | Plan Agent | Kritik | Endüstriyel |
| 43 | Gemi İnşa | Build Agent | Plan Agent | Yüksek | Endüstriyel |
| 44 | Savunma Sanayi | Explore Agent | Build Agent | Kritik | Endüstriyel |
| 45 | Telekomünikasyon | Build Agent | Explore Agent | Yüksek | Teknoloji |
| 46 | Fintech | Build Agent | Plan Agent | Kritik | Teknoloji |
| 47 | Healthtech | Build Agent | Explore Agent | Yüksek | Teknoloji |
| 48 | Edtech | Summary Agent | Build Agent | Orta | Teknoloji |
| 49 | Legaltech | Summary Agent | Build Agent | Orta | Teknoloji |
| 50 | Insurtech | Build Agent | Plan Agent | Yüksek | Teknoloji |
| 51 | Proptech | Build Agent | Plan Agent | Orta | Teknoloji |
| 52 | Cleantech | Explore Agent | Build Agent | Yüksek | Teknoloji |
| 53 | Foodtech | Build Agent | Explore Agent | Orta | Teknoloji |
| 54 | Space Tech | Plan Agent | Build Agent | Yüksek | Teknoloji |
| 55 | Quantum Computing | Explore Agent | Plan Agent | Yüksek | Teknoloji |
| 56 | 5G/6G Teknolojileri | Build Agent | Explore Agent | Yüksek | Teknoloji |
| 57 | Autonomous Systems | Build Agent | Plan Agent | Yüksek | Teknoloji |
| 58 | Digital Twin | Build Agent | Explore Agent | Orta | Teknoloji |
| 59 | Cyber-Physical Systems | Build Agent | Explore Agent | Yüksek | Teknoloji |
| 60 | Smart Grid | Build Agent | General Agent | Yüksek | Teknoloji |
| 61 | Wearable Tech | Build Agent | Explore Agent | Orta | Teknoloji |
| 62 | Green Energy | Explore Agent | Build Agent | Yüksek | Enerji |
| 63 | Nuclear Tech | Explore Agent | Plan Agent | Kritik | Enerji |

---

### 16.2 Sektörel Rol Grupları

#### 16.2.1 Sanayi Sektörleri

| Sektör | Tanım | Birincil Görev | Kritik Seviye | İlgili Katman |
|--------|-------|----------------|---------------|---------------|
| Otomotiv | Araç üretimi, otonom sürüş, infotainment sistemleri | Kaynak yönetimi, üretim planlama modülleri | Yüksek | L2-L4 |
| İmalat | CNC, robotic assembly, kalite kontrol | Üretim izleme, optimizasyon modülleri | Yüksek | L2-L4 |
| Enerji | Üretim, iletim, dağıtım, enerji depolama | SCADA entegrasyonu, akıllı şebeke | Kritik | L2-L4 |
| Madencilik | Açık ve kapalı ocak işletmeliği | İzleme, güvenlik, otomasyon | Yüksek | L2-L4 |
| Tekstil | Kumaş üretimi, boyama, dikim otomasyonu | Üretim takip, kalite kontrol | Orta | L2-L3 |
| Gıda | Üretim, ambalajlama, soğuk zincir | Gıda güvenliği, izlenebilirlik | Kritik | L2-L4 |

**Sanayi Sektörü Agent Davranışları:**

| Davranış | Açıklama | Agent |
|----------|----------|-------|
| Gerçek zamanlı izleme | PLC/SCADA verilerini analiz etme | Explore Agent |
| Üretim optimizasyonu | Verimlilik artırma | Build Agent |
| Kalite kontrol | Defekt tespiti ve düzeltme | Explore Agent |
| Bakım planlama | Prediktif bakım hesaplama | Build Agent |
| Güvenlik denetimi | Endüstriyel güvenlik kontrolü | General Agent |

#### 16.2.2 Teknoloji Sektörleri

| Sektör | Tanım | Birincil Görev | Kritik Seviye | İlgili Katman |
|--------|-------|----------------|---------------|---------------|
| IoT | Cihaz bağlantısı, veri toplama, edge processing | Protokol entegrasyonu, veri pipeline | Yüksek | L2-L4 |
| Siber Güvenlik | Tehdit analizi, penetrasyon testi, SIEM | Güvenlik taraması, Vulnerability scan | Kritik | Tümü |
| Yapay Zeka | ML modelleri, deep learning, NLP | Model eğitim pipeline, inference servisleri | Yüksek | L2-L4 |
| Blockchain | Dağıtılmış defter, smart contract, DeFi | Contract development, node management | Yüksek | L2-L4 |
| Robotik | Motor kontrol, sensör füzyonu, navigasyon | Gerçek zamanlı kontrol, firmware | Yüksek | L1-L4 |
| Edge Computing | Yerel işleme, düşük gecikme, offline working | Edge deployment, sync management | Orta | L2-L4 |
| Bulut (Cloud) | Scalability, multi-tenancy, microservices | Infrastructure as Code, deployment | Yüksek | L2-L4 |

**Teknoloji Sektörü Agent Davranışları:**

| Davranış | Açıklama | Agent |
|----------|----------|-------|
| Threat modeling | Sistem zafiyetlerini haritalandırma | Explore Agent |
| API design | RESTful/gRPC API tasarımı | Build Agent |
| Model training | ML pipeline oluşturma | Build Agent |
| Smart contract audit | Güvenlik denetimi | Explore Agent |
| Firmware update | Cihaz yazılım güncelleme | Build Agent |
| Container orchestration | Docker/Kubernetes yönetimi | Build Agent |

#### 16.2.3 Hizmet Sektörleri

| Sektör | Tanım | Birincil Görev | Kritik Seviye | İlgili Katman |
|--------|-------|----------------|---------------|---------------|
| Finans | Ödeme, kredi, yatırım, bankacılık | İşlem doğrulama, fraude detection | Kritik | L2-L4 |
| Sağlık | Hasta kaydı, tıbbi cihaz, telemedicine | Veri gizliliği, HL7/FHIR entegrasyonu | Kritik | L2-L4 |
| Eğitim | LMS, Contents, assessment | Öğrenme yolu optimizasyonu | Orta | L2-L3 |
| Hukuk | Dava yönetimi, contract analizi | Doküman analizi, compliance check | Yüksek | L2-L3 |
| Sigorta | Poliçe, tazminat, risk assessment | Risk modelleme, claims processing | Yüksek | L2-L4 |
| Gayrimenkul | Emlak ilanı, değerleme, tapu | Veri analizi, ilan yönetimi | Orta | L2-L3 |

**Hizmet Sektörü Agent Davranışları:**

| Davranış | Açıklama | Agent |
|----------|----------|-------|
| Compliance check | Düzenleyici uyumluluk denetimi | Explore Agent |
| Patient data handling | HIPPD/KVKK uyumlu veri işleme | Build Agent |
| Financial reporting | Mali rapor oluşturma | Build Agent |
| Legal document analysis | Sözleşme ve hukuki doküman analizi | Summary Agent |
| Insurance underwriting | Risk değerlendirmesi ve fiyatlandırma | Build Agent |

#### 16.2.4 Kamu Sektörleri

| Sektör | Tanım | Birincil Görev | Kritik Seviye | İlgili Katman |
|--------|-------|----------------|---------------|---------------|
| Kamu | e-Devlet, Vatandaş hizmetleri | Servis entegrasyonu, veri paylaşımı | Yüksek | L2-L4 |
| Afet Yönetimi | Deprem, sel, yangın, erken uyarı | Sensör füzyonu, alert sistemi | Kritik | L1-L4 |
| Çevre | Hava kalitesi, atık yönetimi, karbon takibi | Veri toplama, analiz ve raporlama | Orta | L2-L3 |
| Ulaşım | Trafik yönetimi, akıllı traffic lights | Gerçek zamanlı optimizasyon | Yüksek | L2-L4 |
| Su Yönetimi | Su kalitesi, dağıtım, arıtma | İzleme, sızıntı tespiti | Orta | L2-L4 |

**Kamu Sektörü Agent Davranışları:**

| Davranış | Açıklama | Agent |
|----------|----------|-------|
| Emergency response | Afet durumunda otomatik yanıt sistemi | General Agent |
| Traffic optimization | Trafik akışı optimizasyonu | Build Agent |
| Water quality monitoring | Su kalitesi izleme ve alarm | Explore Agent |
| Citizen service integration | e-Devlet servisleri entegrasyonu | Build Agent |
| Environmental reporting | Çevre raporu oluşturma | Summary Agent |

#### 16.2.5 Yeni Nesil Sektörler

| Sektör | Tanım | Birincil Görev | Kritik Seviye | İlgili Katman |
|--------|-------|----------------|---------------|---------------|
| Metaverse | Sanal dünya, avatar, etkileşim | 3D engine entegrasyonu, rendering pipeline | Orta | L2-L4 |
| NFT | Non-fungible token, dijital varlık | Smart contract, marketplace development | Düşük | L2-L3 |
| AR/VR | Artırılmış/Sanal gerçeklik | Sensör füzyonu, rendering optimizasyonu | Orta | L2-L4 |
| Uzay Teknolojileri | Uydu, fırlatma, uzay istasyonu | Flight software, telemetri | Kritik | L1-L4 |
| Nanoteknoloji | Nano ölçekli malzemeler, moleküler montaj | Simülasyon, modelleme | Yüksek | L2-L4 |
| Genetik Mühendislik | CRISPR, gen sekanslama, biyoinformatik | Veri analizi, pipeline yönetimi | Kritik | L2-L4 |
| 3D Baskı | Additive manufacturing, rapid prototyping | CAD/CAM entegrasyonu, slicer yazılımı | Orta | L2-L4 |

**Yeni Nesil Sektör Agent Davranışları:**

| Davranış | Açıklama | Agent |
|----------|----------|-------|
| 3D rendering pipeline | Gerçek zamanlı rendering optimizasyonu | Build Agent |
| Smart contract deployment | Blockchain contract dağıtımı | Build Agent |
| Genomic data analysis | Genomik veri analizi pipeline | Explore Agent |
| Satellite telemetry | Uydu telemetri veri işleme | Build Agent |
| Molecular simulation | Nano ölçekli simülasyon | Explore Agent |
| AR scene composition | Gerçeklik artırma sahne oluşturma | Build Agent |

---

### 16.3 Çapraz Sektörel Koordinasyon Rollerinin Tanımları

| # | Koordinasyon Rolü | Tanım | Sorumlu Agent | Kapsam |
|---|-------------------|-------|---------------|--------|
| 1 | Sektörel Değerlendirme Uzmanı | Yeni bir sektör için uygunluğu değerlendirme | MO + Plan Agent | Tümü |
| 2 | Teknoloji Köprüleme Uzmanı | Farklı sektörler arası teknoloji transferi | General Agent | Çapraz |
| 3 | Uyumluluk Denetçisi | Sektörel düzenlemelere uygunluk denetimi | Explore Agent | Tümü |
| 4 | Entegrasyon Koordinatörü | Çoklu sektör entegrasyonu yönetimi | Build Agent | Çapraz |
| 5 | Performans Analisti | Sektörel performans metriklerini analiz etme | Explore Agent | Tümü |
| 6 | Risk Değerlendirmenisi | Sektörel risk haritası çıkarma | General Agent | Tümü |
| 7 | Kalite Güvence Uzmanı | Sektörel kalite standartlarını uygulama | Explore Agent | Tümü |
| 8 | Veri Mimarı | Sektörel veri modellerini tasarlama | Build Agent | Tümü |

**Çapraz Sektörel Koordinasyon Akışı:**

```
[Sektör İsteği] → [MO Analiz] → [Uygun Agent Seçimi] → [Sektörel Uzman Agent]
                                                          ↓
                                            [Çapraz Sektörel Koordinasyon]
                                                          ↓
                                            [Sonuç Doğrulama] → [MO Onay] → [Tamamlama]
```

**Koordinasyon Protokolleri:**

| Protokol | Tetikleyici | Aksiyon | Sorumlu |
|----------|------------|---------|---------|
| Sektörel Bildirim | Yeni sektör eklendiği zaman | Tüm ilgili agentları bilgilendir | MO |
| Teknoloji Transferi | Benzer sektör ihtiyacı | İlgili modülleri paylaş | Build Agent |
| Uyumluluk Kontrolü | Düzenleyici değişiklik | Sektörel kodu yeniden denetle | Explore Agent |
| Entegrasyon Testi | Çoklu sektör entegrasyonu | Kapsamlı test çalıştır | Build Agent |
| Performans Raporlama | Periyodik değerlendirme | Sektörel metrikleri raporla | Summary Agent |

---

### 16.4 Sektörel Yetki Matrisi

#### 16.4.1 Dosya Erişim Matrisi (Sektörel)

| Sektör Grubu | Domain (*.cs) | App (*.cs) | Infra (*.cs) | UI (*.cs) | Config (*.json) | Docs (*.md) | Test (*.cs) |
|---------------|---------------|------------|--------------|-----------|-----------------|-------------|-------------|
| Sanayi | Build ✅ | Build ✅ | Build ✅ | Build ✅ | Plan ✅ | Summary ✅ | Build ✅ |
| Teknoloji | Build ✅ | Build ✅ | Build ✅ | Build ✅ | Plan ✅ | Summary ✅ | Build ✅ |
| Hizmet | Build ✅ | Build ✅ | Build ✅ | Build ✅ | Plan ✅ | Summary ✅ | Build ✅ |
| Kamu | Build ✅ | Build ✅ | Build ✅ | Build ✅ | General ✅ | Summary ✅ | Build ✅ |
| Yeni Nesil | Build ✅ | Build ✅ | Build ✅ | Build ✅ | Plan ✅ | Summary ✅ | Build ✅ |

#### 16.4.2 İşlem Erişim Matrisi (Sektörel)

| Sektör Grubu | Kod Yazma | Dosya Okuma | Dosya Yazma | Dosya Silme | Config Değişikliği | Vault Erişimi | Bash |
|---------------|-----------|-------------|-------------|-------------|-------------------|---------------|------|
| Sanayi | Build ✅ | Tüm ✅ | Build ✅ | ❌ | Plan ✅ | MO ✅ | Build ✅ |
| Teknoloji | Build ✅ | Tüm ✅ | Build ✅ | ❌ | Plan ✅ | MO ✅ | Build ✅ |
| Hizmet | Build ✅ | Tüm ✅ | Build ✅ | ❌ | Plan ✅ | MO ✅ | Build ✅ |
| Kamu | Build ✅ | Tüm ✅ | Build ✅ | ❌ | General ✅ | MO ✅ | Build ✅ |
| Yeni Nesil | Build ✅ | Tüm ✅ | Build ✅ | ❌ | Plan ✅ | MO ✅ | Build ✅ |

#### 16.4.3 Sektörel Kritik Seviye Yetkilendirmeleri

| Kritik Seviye | Onay Süreci | Ek Yetkiler | Geri Alma |
|---------------|-------------|-------------|-----------|
| Kritik | İnsan onayı zorunlu | Tam yetki (+ audit log) | Otomatik rollback |
| Yüksek | MO + İnsan onayı | Genişletilmiş yetki | Manuel rollback |
| Orta | MO onayı | Standart yetki | Standart |
| Düşük | Otomatik | Temel yetki | Kolay geri alma |

#### 16.4.4 Sektörel Veri Hassasiyeti Matrisi

| Sektör | Veri Hassasiyeti | KVKK Uyumu | GDPR Uyumu | Özel Düzenleme |
|--------|-------------------|------------|------------|----------------|
| Finans | Kritik | ✅ | ✅ | BDDK, TCMB |
| Sağlık | Kritik | ✅ | ✅ | HSK, hasta gizliliği |
| Kamu | Yüksek | ✅ | — | 6698 sayılı kanun |
| Eğitim | Orta | ✅ | ✅ | Öğrenci gizliliği |
| Sanayi | Orta | ✅ | ✅ | Endüstriyel casusluk |
| Teknoloji | Yüksek | ✅ | ✅ | Fikri mülkiyet |
| Yeni Nesil | Değişken | ✅ | ✅ | Sektöre özgü |

---

### 16.5 Sektörel Performans Metrikleri

#### 16.5.1 Genel Sektörel Metrikler

| Metrik | Hedef | Kritik Eşik | Uyarı Eşik |
|--------|-------|-------------|------------|
| Sektörel task completion rate | > %95 | < %80 | < %90 |
| Sektörel error rate | < %2 | > %5 | > %3 |
| Sektörel response time | < 3s | > 10s | > 5s |
| Sektörel user satisfaction | > 4.5/5 | < 3.5/5 | < 4.0/5 |
| Cross-sector integration success | > %90 | < %75 | < %85 |
| Sektörel compliance rate | %100 | < %95 | < %98 |

#### 16.5.2 Sektöre Özel Metrikler

**Sanayi Sektörü Metrikleri:**

| Metrik | Hedef | Açıklama |
|--------|-------|----------|
| Üretim hattı uptime | > %99.5 | Sistem使用lılığı |
| Defekt tespit oranı | > %98 | Hatalı ürün yakalama |
| Bakım optimizasyonu | %20 tasarruf | Prediktif bakım etkinliği |
| Güvenlik olayı yanıtı | < 30sn | Acil durum tepki süresi |
| Enerji verimliliği | %15 iyileşme | Enerji tüketim optimizasyonu |

**Teknoloji Sektörü Metrikleri:**

| Metrik | Hedef | Açıklama |
|--------|-------|----------|
| API response time | < 200ms | Servis yanıt hızı |
| Sistem availability | > %99.9 | Kesintisiz hizmet |
| Güvenlik açığı修补 süresi | < 24sa | Kritik yama süresi |
| Model accuracy | > %95 | ML model doğruluğu |
| Container startup time | < 10sn | Hızlı deployment |

**Hizmet Sektörü Metrikleri:**

| Metrik | Hedef | Açıklama |
|--------|-------|----------|
| İşlem doğrulama süresi | < 1sn | Finansal işlem hızı |
| Hasta verisi işleme | < 500ms | Tıbbi veri erişim |
| Compliance audit pass | %100 | Düzenleyici uyumluluk |
| Müşteri memnuniyeti | > 4.5/5 | Hizmet kalitesi |
| Veri gizliliği ihlali | 0 | Sıfır ihlal hedefi |

**Kamu Sektörü Metrikleri:**

| Metrik | Hedef | Açıklama |
|--------|-------|----------|
| Afet uyarı süresi | < 60sn | Erken uyarı hızı |
| Vatandaş hizmet erişimi | > %99 | e-Devlet erişilebilirlik |
| Veri güncellik | < 5dk | Gerçek zamanlı veri |
| Trafik optimizasyonu | %20 iyileşme | Akıllı traffic yönetimi |
| Su kaybı azaltma | %30 tasarruf | Akıllı su yönetimi |

**Yeni Nesil Sektör Metrikleri:**

| Metrik | Hedef | Açıklama |
|--------|-------|----------|
| Rendering FPS | > 60fps | Metaverse/AR-VR performansı |
| Blockchain throughput | > 1000 TPS | İşlem işleme hızı |
| Genomik analiz süresi | < 1sa | DNA sekanslama hızı |
| Uydu bağlantısı latency | < 200ms | Uzay iletişimi |
| Nanomaterial simülasyon accuracy | > %90 | Simülasyon doğruluğu |

#### 16.5.3 Sektörel Dashboard Metrikleri

| Dashboard Bileşeni | Güncelleme Sıklığı | Gösterge Türü |
|---------------------|-------------------|---------------|
| Sektörel Task Durumu | Gerçek zamanlı | Gauge |
| Hata Oranı Trendi | Saatlik | Line Chart |
| Performans Skoru | Günlük | Bar Chart |
| Compliance Durumu | Haftalık | Pie Chart |
| Cross-sector Entegrasyon | Aylık | Heatmap |
| Kullanıcı Memnuniyeti | Aylık | Radar Chart |

#### 16.5.4 Sektörel Alarm ve Bildirimler

| Alarm Türü | Eşik | Bildirim Kanalı | Sorumlu |
|------------|------|-----------------|---------|
| Kritik hata | > %5 hata oranı | Email + Slack + SMS | MO + İnsan |
| Performans düşüklüğü | > 10sn yanıt | Email + Slack | MO |
| Compliance ihlali | Herhangi bir ihlal | Email + Log | MO + İnsan |
| Sistem kesintisi | > 5dk kesinti | SMS + Email | MO + İnsan |
| Veri sızıntısı | Tespit edilen herhangi bir | Acil durum prosedürü | İnsan |
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
<<<<<<< HEAD
=======
**Mode:** Red Team · Human Mode · Truth Mode

---

## 17. Versions & Changelog

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-25 | İlk sürüm, tüm roller ve yetkiler tanımlandı |
| 1.1.0 | 2026-08-26 | Enhanced - Performans metrikleri, erişim kontrol matrisi güncellendi |
| 1.2.0 | 2026-08-26 | Sektörel Roller & Uzmanlık Alanları eklendi - 63 sektör, çapraz koordinasyon rolleri, yetki matrisleri, performans metrikleri |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
**Mode:** Red Team · Human Mode · Truth Mode