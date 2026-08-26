---
title: "Versa Coder — Mimari Kararlar & Beyin Haritası"
type: architecture
category: architecture-decisions
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/brain.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/brain.md · .ai/decisions/"
---

# Versa Coder — Mimari Kararlar & Beyin Haritası

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]] · [[decisions/]]

---

## 1. Amaç

Versa Coder projesinin tüm mimari kararlarını, tasarım tercihlerini ve teknoloji seçimlerini saklayan **Tek Doğruluk Kaynağıdır (SSOT)**.

---

## 2. Mimari Kararlar Özeti

### 2.1 Kabul Edilen Kararlar

| Karar | Durum | Tarih | ADR |
|-------|-------|-------|-----|
| Clean Architecture (L0-L7) | ✅ Kabul | 2026-08-25 | ADR-001 |
| Entity Framework Core (DbContext ONLY) | ✅ Kabul | 2026-08-25 | ADR-002 |
| SQLite WAL Mode | ✅ Kabul | 2026-08-25 | ADR-003 |
| DevExpress WinForms 2026 | ✅ Kabul | 2026-08-25 | ADR-004 |
| Multi-Provider AI | ✅ Kabul | 2026-08-25 | ADR-005 |
| CQRS with MediatR | ✅ Kabul | 2026-08-25 | ADR-006 |
| 7-Agent System | ✅ Kabul | 2026-08-25 | ADR-007 |
| MCP Integration | ✅ Kabul | 2026-08-25 | ADR-008 |

### 2.2 Reddedilen Kararlar

| Karar | Red Sebebi | Tarih |
|-------|-----------|-------|
| MongoDB | EF Core ile uyumsuzluk | 2026-08-25 |
| React Native | DevExpress tercihi | 2026-08-25 |
| Microservices | Monolith öncelik | 2026-08-25 |
| Dapper | EF Core DbContext zorunlu | 2026-08-25 |

---

## 3. Mimari Katmanlar (L0-L7)

### 3.1 Katman Tanımları

| Katman | Ad | Sorumluluk | Bağımlılık |
|--------|-----|-----------|-----------|
| **L0** | Domain | Varlıklar, Değer Nesneleri, Olaylar, Kurallar | Hiçbiri |
| **L1** | Abstractions | Arayüzler, Sözleşmeler, DTO'lar | L0 |
| **L2** | Application | Use Case'ler, Handler'lar, Validasyon | L1 |
| **L3** | CrossCutting | Loglama, İstisna, Doğrulama, Cache | L2 |
| **L4** | Infrastructure | Modüller, Servisler, DbContext, API | L3 |
| **L5** | Protocol | AI Protokol, MCP, Provider | L4 |
| **L6** | Host | Başlatma, DI, Yapılandırma | L5 |
| **L7** | UI | DevExpress WinForms, Ribbon, Tabs | L6 |

### 3.2 Bağımlılık Kuralları (Violations = Build Failure)

```
L7 → L6 (İzin verilen)
L6 → L5 (İzin verilen)
L5 → L4 (İzin verilen)
L4 → L3 (İzin verilen)
L3 → L2 (İzin verilen)
L2 → L1 (İzin verilen)
L1 → L0 (İzin verilen)

L0 → L2 (YASAK)
L1 → L3 (YASAK)
L2 → L4 (YASAK)
L3 → L5 (YASAK)
L4 → L6 (YASAK)
L5 → L7 (YASAK)
```

### 3.3 Katman İhlali Tespiti

```csharp
// Build process'de kontrol edilir
public class LayerViolationAnalyzer
{
    public bool HasViolation(Project source, Project target)
    {
        var sourceLayer = GetLayer(source);
        var targetLayer = GetLayer(target);
        
        // Higher layer cannot depend on lower layer
        return sourceLayer.Number > targetLayer.Number;
    }
}
```

---

## 4. Domain Model

### 4.1 Ana Varlıklar (Core Entities)

| Varlık | Açıklama | Katman |
|--------|----------|--------|
| Session | AI oturumu | L0 |
| Project | Proje tanımı | L0 |
| Message | Oturum mesajı | L0 |
| TaskItem | Görev kalemi | L0 |
| TaskList | Görev listesi | L0 |
| LearningEntry | Öğrenme kaydı | L0 |
| AuditLog | Denetim kaydı | L0 |
| FileEntry | Dosya kaydı | L0 |
| Setting | Uygulama ayarı | L0 |

### 4.2 Değer Nesneleri (Value Objects)

| Nesne | Açıklama | Properties |
|-------|----------|------------|
| FilePath | Dosya yolu | Path, Extension, IsDirectory |
| ModelName | AI model adı | Provider, Model, Version |
| SessionId | Oturum kimliği | Value (Guid) |
| Timestamp | Zaman damgası | Value (DateTime) |

### 4.3 Domain Olayları (Events)

| Olay | Tetikleyici | Etki |
|------|------------|------|
| SessionCreatedEvent | Yeni session | Indexleme başlar |
| PromptSentEvent | Prompt gönderimi | AI çağrısı |
| ResponseReceivedEvent | AI yanıtı | Message kaydı |
| ToolExecutedEvent | Tool kullanımı | Sonuç işlenir |
| AgentHandoverEvent | Agent değişimi | Context transfer |
| LearningRecordedEvent | Öğrenme | Knowledge base güncelleme |

---

## 5. CQRS Yapısı

### 5.1 Commands

| Command | Handler | Validation |
|---------|---------|------------|
| CreateSessionCommand | CreateSessionHandler | SessionValidator |
| SendPromptCommand | SendPromptHandler | PromptValidator |
| BranchSessionCommand | BranchSessionHandler | BranchValidator |
| CompleteSessionCommand | CompleteSessionHandler | CompleteValidator |
| RecordLearningCommand | RecordLearningHandler | LearningValidator |
| CreateProjectCommand | CreateProjectHandler | ProjectValidator |

### 5.2 Queries

| Query | Handler | Response |
|-------|---------|----------|
| GetAllSessionsQuery | GetAllSessionsHandler | List<SessionDto> |
| GetSessionQuery | GetSessionHandler | SessionDto |
| GetSessionMessagesQuery | GetSessionMessagesHandler | List<MessageDto> |
| GetContextQuery | GetContextHandler | ContextDto |
| GetAllProjectsQuery | GetAllProjectsHandler | List<ProjectDto> |
| GetProjectQuery | GetProjectHandler | ProjectDto |

### 5.3 Pipeline Behaviors

| Behavior | Amaç | Sıra |
|----------|------|------|
| LoggingBehavior | İşlem loglama | 1 |
| PerformanceBehavior | Performans ölçümü | 2 |
| ValidationBehavior | Giriş doğrulama | 3 |
| CachingBehavior | Önbellek yönetimi | 4 |
| TransactionBehavior | İşlem yönetimi | 5 |

---

## 6. AI Provider Sistemi

### 6.1 Provider Arabirimi

```csharp
public interface ILLMProvider
{
    string Name { get; }
    bool IsAvailable { get; }
    Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken ct);
    IAsyncEnumerable<LLMResponse> StreamAsync(LLMRequest request, CancellationToken ct);
}
```

### 6.2 Provider Implementasyonları

| Provider | Model | Durum | Öncelik |
|----------|-------|-------|---------|
| OpenAI | GPT-4o, GPT-4-turbo | ✅ Aktif | 1 |
| Anthropic | Claude 3.5 Sonnet | ✅ Aktif | 2 |
| Google | Gemini Pro | ✅ Aktif | 3 |
| Ollama | Llama 3, Mistral | ✅ Aktif | 4 |
| Custom | Özel model | 🔄 Geliştirme | 5 |

### 6.3 Provider Router

```csharp
public class ProviderRouter
{
    private readonly IEnumerable<ILLMProvider> _providers;
    
    public ILLMProvider SelectProvider(string providerName)
    {
        return _providers.FirstOrDefault(p => 
            p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase) 
            && p.IsAvailable)
            ?? throw new ProviderNotFoundException(providerName);
    }
}
```

---

## 7. Agent Sistemi

### 7.1 Agent Mimarisi

```
Master Orchestrator (MO)
  ├── Build Agent (Kod üretimi)
  ├── Plan Agent (Planlama)
  ├── Explore Agent (Analiz)
  ├── General Agent (Genel)
  ├── Summary Agent (Doküman)
  └── Title Agent (İsimlendirme)
```

### 7.2 Agent Communication

| İletişim Türü | Protokol | Kullanım |
|---------------|----------|----------|
| Task Assignment | Direct | MO → Agent |
| Handover | Request/Response | Agent ↔ Agent |
| Escalation | Chain | Agent → MO → Human |
| Health Check | Ping/Pong | MO ↔ Agents |

### 7.3 Agent State Machine

```
[Idle] → [Assigned] → [Executing] → [Completed]
                         ↓
                      [Blocked]
                         ↓
                      [Escalated]
```

---

## 8. Tool Sistemi

### 8.1 Tool Kategorileri

| Kategori | Tool Sayısı | Örnekler |
|----------|-------------|----------|
| File Operations | 8 | Read, Write, Edit, Glob, Grep, Delete, Copy, Move |
| Terminal | 3 | Bash, PowerShell, CMD |
| Git | 7 | Status, Diff, Commit, Push, Pull, Branch, Merge |
| Test | 3 | Run Tests, Coverage, Benchmark |
| AI | 3 | LLM Query, Embedding, Embedding Search |
| MCP | 3 | Resource Read, Tool Call, Resource List |
| Project | 3 | Index, Analyze, Diagram |
| Session | 4 | Save, Load, Branch, Fork |
| Context | 3 | Assemble, Update, Validate |
| **Toplam** | **40+** | — |

### 8.2 Tool Interface

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    ToolParameters Parameters { get; }
    Task<ToolResult> ExecuteAsync(ToolContext context, CancellationToken ct);
}
```

---

## 9. Veritabanı Tasarımı

### 9.1 SQLite Konfigürasyonu

```csharp
// WAL mode - Performans için
PRAGMA journal_mode=WAL;
PRAGMA synchronous=NORMAL;
PRAGMA cache_size=-64000;  // 64MB
PRAGMA foreign_keys=ON;
```

### 9.2 Tablo Yapısı

| Tablo | Amaç | İndeksler |
|-------|------|-----------|
| Sessions | Oturumlar | Id, ProjectId, CreatedAt |
| Messages | Mesajlar | SessionId, CreatedAt |
| Projects | Projeler | Id, Name |
| Tasks | Görevler | SessionId, Status, Priority |
| TaskLists | Görev listeleri | SessionId |
| LearningEntries | Öğrenme kayıtları | Category, Keywords |
| AuditLogs | Denetim kayıtları | Timestamp, ActionType |
| Files | Dosya kayıtları | Path, SessionId |
| Settings | Ayarlar | Key |

### 9.3 Migration Stratejisi

```csharp
// EF Core Code-First Migration
public class VersaCoderDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
```

---

## 10. Güvenlik Mimarisi

### 10.1 Güvenlik Katmanları

| Katman | Koruma | Uygulama |
|--------|--------|----------|
| Network | TLS/HTTPS | Tüm bağlantılar |
| Authentication | API Key | Provider erişimi |
| Authorization | Role-based | Agent yetkileri |
| Data | Encryption | Hassas veriler |
| Audit | Logging | Tüm işlemler |

### 10.2 API Key Yönetimi

```csharp
// Vault'ta saklanır, kodda hardcoded YASAK
public class ApiKeyManager
{
    private readonly IVault _vault;
    
    public string GetApiKey(string provider)
    {
        return _vault.GetSecret($"api:{provider}:key")
            ?? throw new ApiKeyNotFoundException(provider);
    }
}
```

---

## 11. Performans Tasarımı

### 11.1 Caching Stratejisi

| Cache Type | TTL | Kullanım |
|------------|-----|----------|
| Memory Cache | 5 dk | Sık kullanılan veriler |
| Distributed Cache | 1 saat | Paylaşımlı veriler |
| Response Cache | 15 dk | API yanıtları |
| Query Cache | 30 dk | Database sorguları |

### 11.2 Async/Await Kuralları

```csharp
// ✅ Doğru
public async Task<Session> GetSessionAsync(SessionId id, CancellationToken ct)
{
    return await _repository.GetByIdAsync(id, ct);
}

// ❌ Yanlış
public Session GetSession(SessionId id)
{
    return _repository.GetById(id).Result;  // Deadlock risk
}
```

### 11.3 Connection Pooling

```csharp
// SQLite için WAL mode + Connection pooling
services.AddDbContext<VersaCoderDbContext>(options =>
    options.UseSqlite("Data Source=versacoder.db;Cache=Shared"));
```

---

## 12. Error Handling

### 12.1 Hata Hiyerarşisi

```
VersaCoderException (Base)
  ├── DomainException
  │     ├── ValidationException
  │     ├── NotFoundException
  │     └── DuplicateException
  ├── InfrastructureException
  │     ├── DatabaseException
  │     ├── ProviderException
  │     └── NetworkException
  ├── ProtocolException
  │     ├── MCPException
  │     └── AgentException
  └── UIException
        ├── RenderException
        └── InteractionException
```

### 12.2 Global Exception Handler

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public async Task HandleAsync(Exception context, CancellationToken ct)
    {
        _logger.LogError(context, "Unhandled exception: {Message}", context.Message);
        
        switch (context)
        {
            case ValidationException ve:
                await HandleValidationAsync(ve, ct);
                break;
            case DomainException de:
                await HandleDomainAsync(de, ct);
                break;
            default:
                await HandleUnexpectedAsync(context, ct);
                break;
        }
    }
}
```

---

## 13. Logging Stratejisi

### 13.1 Log Seviyeleri

| Level | Kullanım | Örnek |
|-------|----------|-------|
| Verbose | Detaylı debug | Variable values |
| Debug | Geliştirme bilgisi | Method entry/exit |
| Information | Normal olaylar | Request completed |
| Warning | Uyarılar | Slow query |
| Error | Hatalar | Exception thrown |
| Fatal | Kritik hatalar | System crash |

### 13.2 Structured Logging

```csharp
// Serilog ile structured logging
Log.Information("Session created: {SessionId} for project {ProjectId}", 
    sessionId, projectId);

// Output: {"SessionId":"abc-123","ProjectId":"proj-456","Message":"Session created: abc-123 for project proj-456"}
```

---

## 14. Monitoring & Observability

### 14.1 Three Pillars

| Pillar | Araç | Kullanım |
|--------|------|----------|
| Logs | Serilog | Olay kayıtları |
| Metrics | Custom | Performans metrikleri |
| Traces | Custom | İşlem takibi |

### 14.2 Health Checks

```csharp
// Health check endpoint
services.AddHealthChecks()
    .AddSQLite("Data Source=versacoder.db", name: "database")
    .AddCheck<ProviderHealthCheck>("ai-provider")
    .AddCheck<MemoryHealthCheck>("memory");
```

---

## 15. Teknoloji Kararları

### 15.1 Teknoloji Seçim Matrisi

| İhtiyaç | Seçilen | Alternatifler | Sebep |
|---------|---------|----------------|-------|
| ORM | EF Core DbContext | Dapper, NHibernate | Guardrail #13 |
| Database | SQLite WAL | PostgreSQL, MySQL | Guardrail #16 |
| UI | DevExpress WinForms | WPF, MAUI | Guardrail #15 |
| IoC | MS.Extensions.DI | Autofac, Ninject | .NET standard |
| Logging | Serilog | NLog, log4net | Structured logging |
| Testing | xUnit | NUnit, MSTest | Modern, async |
| CQRS | MediatR | Custom | Industry standard |
| Validation | FluentValidation | Data Annotations | Flexible |
| Resilience | Polly | Custom | Industry standard |
| Markdown | Markdig | Custom | Full spec support |

### 15.2 Version Policy

| Package | Version Strategy |
|---------|------------------|
| .NET | LTS (8.0) |
| DevExpress | Latest stable |
| EF Core | Match .NET version |
| Third-party | Latest stable |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Mode:** Red Team · Human Mode · Truth Mode