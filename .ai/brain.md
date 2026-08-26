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

## 15. SignalR Real-time Mimarisi

### 15.1 Hub Tanımları

| Hub | Amaç | Yetki |
|-----|------|-------|
| ChatHub | Session içi mesajlaşma | Oturum üyesi |
| AgentHub | Agent durum değişiklikleri | Agent koordinatörü |
| NotificationHub | Sistem bildirimleri | Tüm kullanıcılar |
| ToolHub | Tool çıktı streaming | Oturum sahibi |

### 15.2 Hub Implementasyonu

```csharp
// ChatHub.cs — Session içi gerçek zamanlı mesajlaşma
[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IMessageService messageService,
        ISessionManager sessionManager,
        ILogger<ChatHub> logger)
    {
        _messageService = messageService;
        _sessionManager = sessionManager;
        _logger = logger;
    }

    public async Task JoinSession(string sessionId)
    {
        var userId = Context.UserIdentifier;
        
        if (!await _sessionManager.IsMemberAsync(sessionId, userId))
        {
            throw new HubException("Bu oturuma erişim yetkiniz yok.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"session:{sessionId}");
        
        _logger.LogInformation(
            "User {UserId} joined session {SessionId}", userId, sessionId);

        await Clients.Group($"session:{sessionId}").SendAsync("UserJoined", new
        {
            UserId = userId,
            SessionId = sessionId,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session:{sessionId}");
        
        await Clients.Group($"session:{sessionId}").SendAsync("UserLeft", new
        {
            UserId = Context.UserIdentifier,
            SessionId = sessionId,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendMessage(string sessionId, string content)
    {
        var message = await _messageService.SaveMessageAsync(new MessageRequest
        {
            SessionId = sessionId,
            UserId = Context.UserIdentifier,
            Content = content,
            Role = MessageRole.User
        });

        await Clients.Group($"session:{sessionId}").SendAsync("ReceiveMessage", new
        {
            message.Id,
            message.Content,
            message.Role,
            message.CreatedAt,
            Sender = Context.UserIdentifier
        });
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception,
                "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
```

```csharp
// AgentHub.cs — Agent durum değişiklikleri için real-time broadcast
[Authorize]
public class AgentHub : Hub
{
    private readonly IAgentCoordinator _coordinator;
    private readonly ILogger<AgentHub> _logger;

    public AgentHub(IAgentCoordinator coordinator, ILogger<AgentHub> logger)
    {
        _coordinator = coordinator;
        _logger = logger;
    }

    public async Task SubscribeAgent(string agentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"agent:{agentId}");
        _logger.LogDebug("Subscribed to agent {AgentId}", agentId);
    }

    public async Task UnsubscribeAgent(string agentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"agent:{agentId}");
    }

    public async Task BroadcastAgentStatus(AgentStatusUpdate update)
    {
        // Tüm agent grubuna durum değişikliğini yayınla
        await Clients.All.SendAsync("AgentStatusChanged", new
        {
            update.AgentId,
            update.Status,
            update.LastActivity,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task RequestAgentHandover(string sourceAgentId, string targetAgentId, string taskId)
    {
        try
        {
            await _coordinator.InitiateHandoverAsync(sourceAgentId, targetAgentId, taskId);
            
            await Clients.Group($"agent:{targetAgentId}").SendAsync("HandoverReceived", new
            {
                SourceAgentId = sourceAgentId,
                TaskId = taskId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Handover failed: {Source} → {Target}", sourceAgentId, targetAgentId);
            throw new HubException($"Handover başarısız: {ex.Message}");
        }
    }

    public async Task BroadcastTaskProgress(TaskProgressUpdate progress)
    {
        await Clients.Group($"session:{progress.SessionId}").SendAsync("TaskProgress", new
        {
            progress.AgentId,
            progress.TaskId,
            progress.Progress,
            progress.Message,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

```csharp
// ToolHub.cs — Tool çıktı streaming
[Authorize]
public class ToolHub : Hub
{
    private readonly IToolExecutor _toolExecutor;
    private readonly ILogger<ToolHub> _logger;

    public ToolHub(IToolExecutor toolExecutor, ILogger<ToolHub> logger)
    {
        _toolExecutor = toolExecutor;
        _logger = logger;
    }

    public async Task ExecuteTool(string sessionId, string toolName, string parametersJson)
    {
        var connectionId = Context.ConnectionId;
        
        _logger.LogInformation(
            "Tool execution started: {ToolName} in session {SessionId}",
            toolName, sessionId);

        try
        {
            // Streaming olarak tool çıktısını gönder
            await foreach (var chunk in _toolExecutor.ExecuteStreamAsync(
                toolName, parametersJson, sessionId))
            {
                await Clients.Caller.SendAsync("ToolOutput", new
                {
                    ToolName = toolName,
                    Chunk = chunk.Content,
                    IsComplete = chunk.IsComplete,
                    Timestamp = DateTime.UtcNow
                });
            }

            await Clients.Caller.SendAsync("ToolCompleted", new
            {
                ToolName = toolName,
                Success = true,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Tool execution failed: {ToolName}", toolName);

            await Clients.Caller.SendAsync("ToolError", new
            {
                ToolName = toolName,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    public async Task CancelToolExecution(string sessionId, string toolName)
    {
        await _toolExecutor.CancelExecutionAsync(sessionId, toolName);
        
        await Clients.Caller.SendAsync("ToolCancelled", new
        {
            ToolName = toolName,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

### 15.3 Connection Management

```csharp
// SignalR Connection Manager — Bağlantı kopma ve yeniden bağlanma
public class SignalRConnectionManager
{
    private readonly ConcurrentDictionary<string, ConnectionInfo> _connections = new();
    private readonly ILogger<SignalRConnectionManager> _logger;

    public SignalRConnectionManager(ILogger<SignalRConnectionManager> logger)
    {
        _logger = logger;
    }

    public void TrackConnection(string connectionId, string userId, string sessionId)
    {
        var info = new ConnectionInfo
        {
            ConnectionId = connectionId,
            UserId = userId,
            SessionId = sessionId,
            ConnectedAt = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            ReconnectCount = 0
        };

        _connections.AddOrUpdate(connectionId, info, (_, _) => info);
        _logger.LogDebug(
            "Connection tracked: {ConnectionId} for user {UserId}",
            connectionId, userId);
    }

    public async Task HandleReconnect(string connectionId, int maxRetries = 5)
    {
        if (_connections.TryGetValue(connectionId, out var info))
        {
            info.ReconnectCount++;
            info.LastActivity = DateTime.UtcNow;

            if (info.ReconnectCount > maxRetries)
            {
                _logger.LogWarning(
                    "Max reconnect attempts reached for {ConnectionId}", connectionId);
                _connections.TryRemove(connectionId, out _);
                return;
            }

            _logger.LogInformation(
                "Client reconnecting: {ConnectionId} (attempt {Count})",
                connectionId, info.ReconnectCount);

            // Önceki gruba tekrar katıl
            if (!string.IsNullOrEmpty(info.SessionId))
            {
                await AddToGroup(connectionId, $"session:{info.SessionId}");
            }
        }
    }

    public void RemoveConnection(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
        _logger.LogDebug("Connection removed: {ConnectionId}", connectionId);
    }

    public int GetActiveConnectionCount() => _connections.Count;

    public IEnumerable<ConnectionInfo> GetConnectionsBySession(string sessionId)
    {
        return _connections.Values.Where(c => c.SessionId == sessionId);
    }
}

public class ConnectionInfo
{
    public string ConnectionId { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime LastActivity { get; set; }
    public int ReconnectCount { get; set; }
}
```

### 15.4 Message Groups

| Grup Tipi | Format | Kullanım |
|-----------|--------|----------|
| Session | `session:{sessionId}` | Session içi tüm katılımcılar |
| Agent | `agent:{agentId}` | Belirli bir agent'ın durumu |
| User | `user:{userId}` | Kullanıcıya özel bildirimler |
| System | `system:broadcast` | Tüm bağlantılara yayın |

### 15.5 Streaming Implementasyonu

```csharp
// AI yanıt streaming — SignalR üzerinden client'a aktarım
public class AiResponseStreamer
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<AiResponseStreamer> _logger;

    public AiResponseStreamer(
        IHubContext<ChatHub> hubContext,
        ILogger<AiResponseStreamer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task StreamAiResponse(
        string sessionId,
        IAsyncEnumerable<string> responseStream,
        CancellationToken ct = default)
    {
        var group = $"session:{sessionId}";
        var messageId = Guid.NewGuid().ToString();

        try
        {
            await _hubContext.Clients.Group(group).SendAsync("AiStreamStart", new
            {
                MessageId = messageId,
                Timestamp = DateTime.UtcNow
            }, ct);

            var fullContent = new StringBuilder();

            await foreach (var chunk in responseStream.WithCancellation(ct))
            {
                fullContent.Append(chunk);

                await _hubContext.Clients.Group(group).SendAsync("AiStreamChunk", new
                {
                    MessageId = messageId,
                    Chunk = chunk,
                    AccumulatedLength = fullContent.Length,
                    Timestamp = DateTime.UtcNow
                }, ct);
            }

            await _hubContext.Clients.Group(group).SendAsync("AiStreamComplete", new
            {
                MessageId = messageId,
                FullContent = fullContent.ToString(),
                Timestamp = DateTime.UtcNow
            }, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("AI streaming cancelled for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI streaming failed for session {SessionId}", sessionId);
            
            await _hubContext.Clients.Group(group).SendAsync("AiStreamError", new
            {
                MessageId = messageId,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            }, ct);
        }
    }
}
```

### 15.6 Scale-out Stratejisi

| Yöntem | Avantaj | Dezavantaj | Kullanım |
|--------|---------|------------|----------|
| **Redis Backplane** | Düşük maliyet, basit | Tek nokta bağımlılığı | Tek bölge, küçük ölçek |
| **Azure SignalR Service** | Yönetilen servis, high availability | Maliyet, vendor lock-in | Multi-region, büyük ölçek |
| **Sticky Sessions** | Basit | Load balancing zor | Tek server |

```csharp
// Redis Backplane konfigürasyonu (production'da environment variable kullanın)
services.AddSignalR()
    .AddStackExchangeRedis(Configuration["Redis:ConnectionString"], options =>
    {
        options.Configuration.ChannelPrefix = "versacoder";
        options.Configuration.AbortOnConnectFail = false;
        options.Configuration.ConnectTimeout = 5000;
        options.Configuration.SyncTimeout = 5000;
    });

// Azure SignalR Service konfigürasyonu
services.AddSignalR()
    .AddAzureSignalR(Configuration["AzureSignalR:ConnectionString"]);
```

### 15.7 Authentication & Authorization

```csharp
// SignalR için JWT authentication
services.AddSignalR()
    .AddHubOptions<ChatHub>(options =>
    {
        options.EnableDetailedErrors = true;
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.MaximumParallelInvocationsPerClient = 5;
    });

// Hub authorize attribute
[Authorize(Policy = "SessionMember")]
public class ChatHub : Hub { }

// Policy tanımlama
services.AddAuthorization(options =>
{
    options.AddPolicy("SessionMember", policy =>
        policy.RequireClaim("session_access"));

    options.AddPolicy("AgentCoordinator", policy =>
        policy.RequireRole("Coordinator", "Admin"));
});
```

### 15.8 Client Implementasyonları

```csharp
// .NET Client — WinForms entegrasyonu
public class VersaCoderSignalRClient : IDisposable
{
    private readonly HubConnection _connection;
    private readonly ILogger<VersaCoderSignalRClient> _logger;

    public VersaCoderSignalRClient(string hubUrl, string accessToken, ILogger<VersaCoderSignalRClient> logger)
    {
        _logger = logger;

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken);
            })
            .WithAutomaticReconnect(new[] 
            { 
                TimeSpan.Zero, 
                TimeSpan.FromSeconds(2), 
                TimeSpan.FromSeconds(5), 
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            })
            .Build();

        RegisterEventHandlers();
    }

    private void RegisterEventHandlers()
    {
        _connection.On<object>("ReceiveMessage", message =>
        {
            _logger.LogDebug("Message received: {Message}", message);
            OnMessageReceived?.Invoke(this, message);
        });

        _connection.On<object>("AiStreamChunk", chunk =>
        {
            OnAiStreamChunk?.Invoke(this, chunk);
        });

        _connection.On<object>("AiStreamComplete", result =>
        {
            OnAiStreamComplete?.Invoke(this, result);
        });

        _connection.On<object>("AgentStatusChanged", status =>
        {
            OnAgentStatusChanged?.Invoke(this, status);
        });

        _connection.On<object>("ToolOutput", output =>
        {
            OnToolOutput?.Invoke(this, output);
        });

        _connection.Reconnecting += error =>
        {
            _logger.LogWarning("Reconnecting after error: {Error}", error?.Message);
            OnReconnecting?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            _logger.LogInformation("Reconnected: {ConnectionId}", connectionId);
            OnReconnected?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        };

        _connection.Closed += error =>
        {
            _logger.LogWarning("Connection closed: {Error}", error?.Message);
            OnDisconnected?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        };
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        await _connection.StartAsync(ct);
        _logger.LogInformation("SignalR connection started");
    }

    public async Task JoinSession(string sessionId)
    {
        await _connection.InvokeAsync("JoinSession", sessionId);
    }

    public async Task SendMessage(string sessionId, string content)
    {
        await _connection.InvokeAsync("SendMessage", sessionId, content);
    }

    public event EventHandler<object> OnMessageReceived;
    public event EventHandler<object> OnAiStreamChunk;
    public event EventHandler<object> OnAiStreamComplete;
    public event EventHandler<object> OnAgentStatusChanged;
    public event EventHandler<object> OnToolOutput;
    public event EventHandler OnReconnecting;
    public event EventHandler OnReconnected;
    public event EventHandler OnDisconnected;

    public void Dispose()
    {
        _connection?.DisposeAsync().AsTask().Wait();
    }
}
```

```javascript
// JavaScript Client — Web dashboard entegrasyonu
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat", { accessTokenFactory: () => getToken() })
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", (message) => {
    appendMessage(message);
});

connection.on("AiStreamChunk", (chunk) => {
    appendToCurrentResponse(chunk.chunk);
});

connection.on("AiStreamComplete", (result) => {
    finalizeResponse(result.fullContent);
});

connection.on("AgentStatusChanged", (status) => {
    updateAgentStatus(status.agentId, status.status);
});

connection.onreconnecting((error) => {
    showConnectionStatus("Yeniden bağlanılıyor...");
});

connection.onreconnected((connectionId) => {
    showConnectionStatus("Bağlantı kuruldu");
});

connection.onclose((error) => {
    showConnectionStatus("Bağlantı kesildi");
});

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR connected");
    } catch (err) {
        console.error("Connection failed:", err);
        setTimeout(startConnection, 5000);
    }
}
```

### 15.9 Error Handling & Logging

```csharp
// SignalR Hub Exception Filter
public class SignalRErrorFilter : IHubFilter
{
    private readonly ILogger<SignalRErrorFilter> _logger;

    public SignalRErrorFilter(ILogger<SignalRErrorFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask InvokeHubInvocationAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask> next)
    {
        try
        {
            await next(context);
        }
        catch (HubException ex)
        {
            _logger.LogWarning(ex,
                "Hub exception in {Hub}.{Method}: {Message}",
                context.Hub.GetType().Name,
                context.MethodName,
                ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error in {Hub}.{Method}",
                context.Hub.GetType().Name,
                context.MethodName);
            throw new HubException("Beklenmeyen bir hata oluştu.");
        }
    }
}
```

### 15.10 Reconnection Stratejisi

| Durum | Aksiyon | Timeout |
|-------|---------|---------|
| İlk bağlantı hatası | Anında yeniden dene | 0ms |
| 2. deneme | 2 saniye bekle | 2s |
| 3. deneme | 5 saniye bekle | 5s |
| 4. deneme | 10 saniye bekle | 10s |
| 5. deneme | 30 saniye bekle | 30s |
| 5+ deneme | Exponential backoff | Max 5dk |

---

## 16. Monitoring & Observability Mimarisi

### 16.1 Üç Sütun (Three Pillars)

| Sütun | Araç | Amaç |
|-------|------|------|
| **Logs** | Serilog + SEQ | Yapılandırılmış olay kayıtları |
| **Metrics** | Prometheus + Grafana | Performans metrikleri |
| **Traces** | OpenTelemetry + Jaeger | Dağıtık izleme |

### 16.2 Prometheus Metrics Tanımları

```yaml
# prometheus.yml — Prometheus konfigürasyonu
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: 'versacoder'
    static_configs:
      - targets: ['${VERSACODER_HOST}:${VERSACODER_PORT}']
    metrics_path: '/metrics'
```

### 16.3 Custom Metrics (Prometheus Formatı)

```
# HELP agent_task_duration_seconds Süre: Agent görev süresi
# TYPE agent_task_duration_seconds histogram
agent_task_duration_seconds_bucket{agent="build",task_type="code_generation",le="0.1"} 12
agent_task_duration_seconds_bucket{agent="build",task_type="code_generation",le="0.5"} 45
agent_task_duration_seconds_bucket{agent="build",task_type="code_generation",le="1"} 78
agent_task_duration_seconds_bucket{agent="build",task_type="code_generation",le="2"} 95
agent_task_duration_seconds_bucket{agent="build",task_type="code_generation",le="5"} 100
agent_task_duration_seconds_bucket{agent="build",task_type="code_generation",le="+Inf"} 100
agent_task_duration_seconds_sum{agent="build",task_type="code_generation"} 87.5
agent_task_duration_seconds_count{agent="build",task_type="code_generation"} 100

# HELP agent_task_success_total Toplam başarılı agent görevleri
# TYPE agent_task_success_total counter
agent_task_success_total{agent="build"} 1523
agent_task_success_total{agent="plan"} 892
agent_task_success_total{agent="explore"} 2341
agent_task_success_total{agent="general"} 456
agent_task_success_total{agent="summary"} 1123
agent_task_success_total{agent="title"} 567

# HELP agent_task_failure_total Toplam başarısız agent görevleri
# TYPE agent_task_failure_total counter
agent_task_failure_total{agent="build"} 23
agent_task_failure_total{agent="plan"} 12
agent_task_failure_total{agent="explore"} 8
agent_task_failure_total{agent="general"} 5
agent_task_failure_total{agent="summary"} 3
agent_task_failure_total{agent="title"} 1

# HELP llm_request_duration_seconds Süre: LLM isteği süresi
# TYPE llm_request_duration_seconds histogram
llm_request_duration_seconds_bucket{provider="openai",model="gpt-4o",le="0.5"} 150
llm_request_duration_seconds_bucket{provider="openai",model="gpt-4o",le="1"} 320
llm_request_duration_seconds_bucket{provider="openai",model="gpt-4o",le="2"} 450
llm_request_duration_seconds_bucket{provider="openai",model="gpt-4o",le="5"} 480
llm_request_duration_seconds_bucket{provider="openai",model="gpt-4o",le="10"} 490
llm_request_duration_seconds_bucket{provider="openai",model="gpt-4o",le="+Inf"} 500
llm_request_duration_seconds_sum{provider="openai",model="gpt-4o"} 1250.5
llm_request_duration_seconds_count{provider="openai",model="gpt-4o"} 500

# HELP llm_token_usage_total Toplam token kullanımı
# TYPE llm_token_usage_total counter
llm_token_usage_total{provider="openai",model="gpt-4o",type="prompt"} 1250000
llm_token_usage_total{provider="openai",model="gpt-4o",type="completion"} 875000
llm_token_usage_total{provider="anthropic",model="claude-3.5",type="prompt"} 980000
llm_token_usage_total{provider="anthropic",model="claude-3.5",type="completion"} 650000

# HELP db_query_duration_seconds Süre: Veritabanı sorgu süresi
# TYPE db_query_duration_seconds histogram
db_query_duration_seconds_bucket{operation="select",table="sessions",le="0.01"} 500
db_query_duration_seconds_bucket{operation="select",table="sessions",le="0.05"} 750
db_query_duration_seconds_bucket{operation="select",table="sessions",le="0.1"} 900
db_query_duration_seconds_bucket{operation="select",table="sessions",le="0.5"} 980
db_query_duration_seconds_bucket{operation="select",table="sessions",le="1"} 995
db_query_duration_seconds_bucket{operation="select",table="sessions",le="+Inf"} 1000
db_query_duration_seconds_sum{operation="select",table="sessions"} 45.2
db_query_duration_seconds_count{operation="select",table="sessions"} 1000

# HELP active_sessions_total Aktif oturum sayısı
# TYPE active_sessions_total gauge
active_sessions_total 12

# HELP tool_execution_duration_seconds Süre: Tool çalışma süresi
# TYPE tool_execution_duration_seconds histogram
tool_execution_duration_seconds_bucket{tool="read",le="0.01"} 1200
tool_execution_duration_seconds_bucket{tool="read",le="0.05"} 1800
tool_execution_duration_seconds_bucket{tool="read",le="0.1"} 2100
tool_execution_duration_seconds_bucket{tool="read",le="0.5"} 2200
tool_execution_duration_seconds_bucket{tool="read",le="+Inf"} 2250
tool_execution_duration_seconds_sum{tool="read"} 112.5
tool_execution_duration_seconds_count{tool="read"} 2250
```

### 16.4 C# Metrics Implementation

```csharp
// VersaCoderMetrics.cs — Prometheus metric tanımları
using Prometheus;

public class VersaCoderMetrics
{
    // Agent Metrics
    public static readonly Histogram AgentTaskDuration = Prometheus.Metrics
        .CreateHistogram(
            "agent_task_duration_seconds",
            "Agent görev süresi",
            new HistogramConfiguration
            {
                LabelNames = new[] { "agent", "task_type" },
                Buckets = new[] { 0.1, 0.5, 1, 2, 5, 10, 30 }
            });

    public static readonly Counter AgentTaskSuccess = Prometheus.Metrics
        .CreateCounter(
            "agent_task_success_total",
            "Toplam başarılı agent görevleri",
            new[] { "agent" });

    public static readonly Counter AgentTaskFailure = Prometheus.Metrics
        .CreateCounter(
            "agent_task_failure_total",
            "Toplam başarısız agent görevleri",
            new[] { "agent" });

    // LLM Metrics
    public static readonly Histogram LlmRequestDuration = Prometheus.Metrics
        .CreateHistogram(
            "llm_request_duration_seconds",
            "LLM isteği süresi",
            new HistogramConfiguration
            {
                LabelNames = new[] { "provider", "model" },
                Buckets = new[] { 0.5, 1, 2, 5, 10, 30, 60 }
            });

    public static readonly Counter LlmTokenUsage = Prometheus.Metrics
        .CreateCounter(
            "llm_token_usage_total",
            "Toplam token kullanımı",
            new[] { "provider", "model", "type" });

    // Database Metrics
    public static readonly Histogram DbQueryDuration = Prometheus.Metrics
        .CreateHistogram(
            "db_query_duration_seconds",
            "Veritabanı sorgu süresi",
            new HistogramConfiguration
            {
                LabelNames = new[] { "operation", "table" },
                Buckets = new[] { 0.01, 0.05, 0.1, 0.5, 1, 5 }
            });

    // Session Metrics
    public static readonly Gauge ActiveSessions = Prometheus.Metrics
        .CreateGauge(
            "active_sessions_total",
            "Aktif oturum sayısı");

    // Tool Metrics
    public static readonly Histogram ToolExecutionDuration = Prometheus.Metrics
        .CreateHistogram(
            "tool_execution_duration_seconds",
            "Tool çalışma süresi",
            new HistogramConfiguration
            {
                LabelNames = new[] { "tool" },
                Buckets = new[] { 0.01, 0.05, 0.1, 0.5, 1, 5, 30 }
            });
}

// Kullanım örneği
public class MonitoredAgentRunner
{
    private readonly IAgentRunner _runner;

    public async Task<AgentResult> RunWithMetricsAsync(
        string agentName, string taskType, CancellationToken ct)
    {
        using (VersaCoderMetrics.AgentTaskDuration
            .WithLabels(agentName, taskType).NewTimer())
        {
            try
            {
                var result = await _runner.RunAsync(agentName, taskType, ct);
                VersaCoderMetrics.AgentTaskSuccess.WithLabels(agentName).Inc();
                return result;
            }
            catch (Exception)
            {
                VersaCoderMetrics.AgentTaskFailure.WithLabels(agentName).Inc();
                throw;
            }
        }
    }
}
```

### 16.5 Serilog Structured Logging

```csharp
// Program.cs — Serilog + SEQ konfigürasyonu
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Seq;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "VersaCoder")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.File(
        path: "logs/versacoder-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

// Kullanım
Log.Information("Session {SessionId} started for project {ProjectId}",
    sessionId, projectId);

Log.Warning("LLM provider {Provider} slow response: {Duration}ms",
    "openai", elapsedMs);

Log.Error(exception, "Agent {Agent} failed task {TaskId}",
    agentName, taskId);
```

### 16.6 OpenTelemetry Distributed Tracing

```csharp
// OpenTelemetry konfigürasyonu
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("VersaCoder", "1.0.0")
        .AddAttributes(new Dictionary<string, object>
        {
            { "deployment.environment", Environment.GetEnvironmentVariable("ENV") ?? "development" }
        }))
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource("VersaCoder.Agent")
        .AddSource("VersaCoder.LLM")
        .AddJaegerExporter(options =>
        {
            options.AgentHost = "localhost";
            options.AgentPort = 6831;
        }));

// Custom Activity Source
public class AgentActivitySource
{
    private static readonly ActivitySource Source = new("VersaCoder.Agent");

    public Activity StartAgentTask(string agentName, string taskType)
    {
        var activity = Source.StartActivity(
            $"agent.{agentName}.{taskType}",
            ActivityKind.Internal);
        
        activity?.SetTag("agent.name", agentName);
        activity?.SetTag("task.type", taskType);
        
        return activity;
    }
}
```

### 16.7 Health Check Endpoints

```csharp
// HealthCheckService.cs
public class VersaCoderHealthCheck : IHealthCheck
{
    private readonly VersaCoderDbContext _dbContext;
    private readonly ILLMProvider _provider;
    private readonly ILogger<VersaCoderHealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        var checks = new Dictionary<string, HealthCheckResult>();

        // Database health
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", ct);
            checks["database"] = HealthCheckResult.Healthy("SQLite erişilebilir");
        }
        catch (Exception ex)
        {
            checks["database"] = HealthCheckResult.Unhealthy("SQLite erişilemez", ex);
        }

        // AI Provider health
        try
        {
            var isAvailable = await _provider.IsAvailableAsync(ct);
            checks["ai_provider"] = isAvailable
                ? HealthCheckResult.Healthy($"{_provider.Name} erişilebilir")
                : HealthCheckResult.Degraded($"{_provider.Name} kullanılamıyor");
        }
        catch (Exception ex)
        {
            checks["ai_provider"] = HealthCheckResult.Unhealthy("AI provider hatası", ex);
        }

        // Memory health
        var gcMemory = GC.GetGCMemoryInfo();
        var memoryUsage = gcMemory.HeapSizeBytes / 1024.0 / 1024.0;
        checks["memory"] = memoryUsage < 500
            ? HealthCheckResult.Healthy($"Memory: {memoryUsage:F1} MB")
            : HealthCheckResult.Degraded($"Memory yüksek: {memoryUsage:F1} MB");

        // Overall status
        var worstStatus = checks.Values.Min(r => r.Status);
        var description = string.Join("; ", checks.Select(c => $"{c.Key}: {c.Value.Status}"));

        return new HealthCheckResult(worstStatus, description);
    }
}

// Startup.cs
services.AddHealthChecks()
    .AddCheck<VersaCoderHealthCheck>("versacoder", HealthStatus.Unhealthy);
```

### 16.8 Alert Rules (Prometheus Alerting)

```yaml
# alert_rules.yml — Prometheus alert kuralları
groups:
  - name: versacoder_alerts
    rules:
      # Agent görev başarısızlık oranı
      - alert: HighAgentFailureRate
        expr: |
          rate(agent_task_failure_total[5m]) / 
          rate(agent_task_success_total[5m]) > 0.1
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Agent başarısızlık oranı yüksek"
          description: "Son 5 dakikada %10'dan fazla başarısız görev"

      # LLM yanıt süresi
      - alert: SlowLLMResponses
        expr: |
          histogram_quantile(0.95, rate(llm_request_duration_seconds_bucket[5m])) > 10
        for: 3m
        labels:
          severity: warning
        annotations:
          summary: "LLM yanıtları yavaş"
          description: "P95 yanıt süresi 10 saniyeyi aşıyor"

      # Aktif session sayısı
      - alert: TooManyActiveSessions
        expr: active_sessions_total > 50
        for: 2m
        labels:
          severity: info
        annotations:
          summary: "Çok fazla aktif session"
          description: "Aktif session sayısı 50'yi aşıyor"

      # Veritabanı sorgu süresi
      - alert: SlowDatabaseQueries
        expr: |
          histogram_quantile(0.95, rate(db_query_duration_seconds_bucket[5m])) > 1
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Veritabanı sorguları yavaş"
          description: "P95 sorgu süresi 1 saniyeyi aşıyor"

      # Bellek kullanımı
      - alert: HighMemoryUsage
        expr: process_resident_memory_bytes / 1024 / 1024 > 1024
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "Bellek kullanımı çok yüksek"
          description: "Bellek kullanımı 1GB'ı aşıyor"
```

### 16.9 SLI/SLO Tanımları

| SLI | Metrik | Hedef SLO | Periyot |
|-----|--------|-----------|---------|
| Uptime | Health check başarılı | %99.9 | 30 gün |
| Yanıt süresi | LLM P95 latency | < 5 sn | 7 gün |
| Hata oranı | Başarısız istek oranı | < %1 | 30 gün |
| Throughput | Dakika başına istek | > 100 | 7 gün |
| Veri kaybı | Kayıp mesaj oranı | %0 | 30 gün |

### 16.10 Log Aggregation Pipeline

```csharp
// Log Aggregation — Serilog → SEQ → Grafana
public class LogAggregationPipeline
{
    public static void Configure(IServiceCollection services)
    {
        // Serilog ile yapılandırılmış loglama
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Service", "VersaCoder")
            .Enrich.WithProperty("Environment", "Production")
            .WriteTo.Seq("http://seq:5341")
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });
    }
}
```

### 16.11 Dashboard Panels (Grafana JSON - Simplified)

```json
{
  "dashboard": {
    "title": "VersaCoder Overview",
    "panels": [
      {
        "title": "Agent Task Success Rate",
        "type": "stat",
        "targets": [
          {
            "expr": "sum(rate(agent_task_success_total[5m])) / (sum(rate(agent_task_success_total[5m])) + sum(rate(agent_task_failure_total[5m]))) * 100",
            "legendFormat": "{{agent}}"
          }
        ],
        "fieldConfig": {
          "defaults": {
            "unit": "percent",
            "thresholds": {
              "steps": [
                { "color": "red", "value": 0 },
                { "color": "yellow", "value": 90 },
                { "color": "green", "value": 99 }
              ]
            }
          }
        }
      },
      {
        "title": "LLM Request Duration (P95)",
        "type": "timeseries",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(llm_request_duration_seconds_bucket[5m]))",
            "legendFormat": "{{provider}}/{{model}}"
          }
        ]
      },
      {
        "title": "Active Sessions",
        "type": "stat",
        "targets": [
          {
            "expr": "active_sessions_total"
          }
        ]
      },
      {
        "title": "Token Usage by Provider",
        "type": "piechart",
        "targets": [
          {
            "expr": "sum by (provider) (llm_token_usage_total)",
            "legendFormat": "{{provider}}"
          }
        ]
      }
    ]
  }
}
```

### 16.12 Capacity Planning Metrikleri

| Metrik | Eşik | Aksiyon |
|--------|------|---------|
| CPU kullanımı | > %80 | Scale-up |
| Bellek kullanımı | > 1GB | Scale-up |
| Disk kullanımı | > %80 | Temizleme |
| Aktif session | > 100 | Instance ekle |
| LLM token/mi | > 1M | Quota artır |

---

## 17. Dashboard Tasarım Mimarisi

### 17.1 Dashboard Layout

```
┌─────────────────────────────────────────────────────────┐
│                    Header / Navigation                   │
├──────────┬──────────────────────────────────────────────┤
│          │  ┌─────────┐  ┌─────────┐  ┌─────────┐     │
│  Sidebar │  │ System  │  │ Agent   │  │   LLM   │     │
│  (Nav)   │  │ Status  │  │ Perf.   │  │  Usage  │     │
│          │  └─────────┘  └─────────┘  └─────────┘     │
│          │  ┌─────────┐  ┌─────────┐  ┌─────────┐     │
│          │  │ Sessions│  │  Tools  │  │  Errors │     │
│          │  │ Overview│  │  Perf.  │  │  Track  │     │
│          │  └─────────┘  └─────────┘  └─────────┘     │
│          │  ┌─────────────────────────────────────┐    │
│          │  │        Real-time Activity Log        │    │
│          │  └─────────────────────────────────────┘    │
└──────────┴──────────────────────────────────────────────┘
```

### 17.2 Widget Tanımları

| Widget | Tip | Veri Kaynağı | Yenileme |
|--------|-----|--------------|----------|
| System Status | Stat | Health check | 10 sn |
| Agent Performance | Time series | Prometheus | 15 sn |
| LLM Usage | Pie chart | Prometheus | 30 sn |
| Session Overview | Table | Database | 30 sn |
| Tool Performance | Bar chart | Prometheus | 15 sn |
| Error Tracking | Table | SEQ/Logs | 10 sn |
| Activity Log | Live stream | SignalR | Real-time |

### 17.3 Data Refresh Stratejisi

```csharp
// Dashboard Data Refresh Service
public class DashboardRefreshService
{
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly IPrometheusClient _prometheus;
    private readonly Timer _refreshTimer;

    public DashboardRefreshService(
        IHubContext<DashboardHub> hubContext,
        IPrometheusClient prometheus)
    {
        _hubContext = hubContext;
        _prometheus = prometheus;

        // Her 5 saniyede bir veriyi yenile
        _refreshTimer = new Timer(async _ => await RefreshDashboard(),
            null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private async Task RefreshDashboard()
    {
        try
        {
            var metrics = await CollectMetricsAsync();
            
            await _hubContext.Clients.All.SendAsync("DashboardUpdate", new
            {
                SystemStatus = metrics.SystemStatus,
                AgentPerformance = metrics.AgentPerformance,
                LlmUsage = metrics.LlmUsage,
                ActiveSessions = metrics.ActiveSessions,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            // Log error silently
        }
    }

    private async Task<DashboardMetrics> CollectMetricsAsync()
    {
        // Prometheus'tan metrikleri topla
        var agentSuccess = await _prometheus.QueryAsync(
            "sum by (agent) (rate(agent_task_success_total[5m]))");
        var llmDuration = await _prometheus.QueryAsync(
            "histogram_quantile(0.95, rate(llm_request_duration_seconds_bucket[5m]))");
        var activeSessions = await _prometheus.QueryAsync(
            "active_sessions_total");

        return new DashboardMetrics
        {
            SystemStatus = "Healthy",
            AgentPerformance = agentSuccess,
            LlmUsage = llmDuration,
            ActiveSessions = activeSessions
        };
    }
}
```

### 17.4 Widget Implementasyonları

```csharp
// Dashboard Widgets
public interface IDashboardWidget
{
    string Id { get; }
    string Title { get; }
    WidgetType Type { get; }
    int RefreshIntervalSeconds { get; }
    Task<WidgetData> GetDataAsync();
}

public class AgentPerformanceWidget : IDashboardWidget
{
    public string Id => "agent-performance";
    public string Title => "Agent Performance";
    public WidgetType Type => WidgetType.TimeSeries;
    public int RefreshIntervalSeconds => 15;

    private readonly IPrometheusClient _prometheus;

    public AgentPerformanceWidget(IPrometheusClient prometheus)
    {
        _prometheus = prometheus;
    }

    public async Task<WidgetData> GetDataAsync()
    {
        var query = @"
            sum by (agent) (rate(agent_task_success_total[5m]))";
        
        var result = await _prometheus.QueryAsync(query);
        
        return new WidgetData
        {
            Series = result.Data.Select(r => new DataSeries
            {
                Name = r.Labels["agent"],
                Points = r.Values.Select(v => new DataPoint
                {
                    Timestamp = v.Timestamp,
                    Value = v.Value
                }).ToList()
            }).ToList()
        };
    }
}

public class SystemStatusWidget : IDashboardWidget
{
    public string Id => "system-status";
    public string Title => "System Status";
    public WidgetType Type => WidgetType.Stat;
    public int RefreshIntervalSeconds => 10;

    private readonly IHealthCheckService _healthCheck;

    public SystemStatusWidget(IHealthCheckService healthCheck)
    {
        _healthCheck = healthCheck;
    }

    public async Task<WidgetData> GetDataAsync()
    {
        var report = await _healthCheck.CheckHealthAsync();
        
        return new WidgetData
        {
            Value = report.Status == HealthStatus.Healthy ? "1" : "0",
            Label = report.Status.ToString(),
            Color = report.Status switch
            {
                HealthStatus.Healthy => "green",
                HealthStatus.Degraded => "yellow",
                _ => "red"
            }
        };
    }
}

public class LlmUsageWidget : IDashboardWidget
{
    public string Id => "llm-usage";
    public string Title => "LLM Token Usage";
    public WidgetType Type => WidgetType.PieChart;
    public int RefreshIntervalSeconds => 30;

    private readonly IPrometheusClient _prometheus;

    public LlmUsageWidget(IPrometheusClient prometheus)
    {
        _prometheus = prometheus;
    }

    public async Task<WidgetData> GetDataAsync()
    {
        var query = @"
            sum by (provider) (llm_token_usage_total)";
        
        var result = await _prometheus.QueryAsync(query);
        
        return new WidgetData
        {
            Segments = result.Data.Select(r => new PieSegment
            {
                Label = r.Labels["provider"],
                Value = r.Values.Last().Value
            }).ToList()
        };
    }
}
```

### 17.5 User Customization

```csharp
// Dashboard Customization
public class DashboardConfiguration
{
    public string UserId { get; set; }
    public List<WidgetConfig> Widgets { get; set; } = new();
    public LayoutConfig Layout { get; set; } = new();
    public Theme Theme { get; set; } = Theme.Dark;
    public int AutoRefreshInterval { get; set; } = 30;
}

public class WidgetConfig
{
    public string WidgetId { get; set; }
    public int Position { get; set; }
    public int Width { get; set; } = 4;
    public int Height { get; set; } = 3;
    public bool IsVisible { get; set; } = true;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

// API endpoint
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardConfigService _configService;

    [HttpGet("config")]
    public async Task<IActionResult> GetConfig()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var config = await _configService.GetAsync(userId);
        return Ok(config);
    }

    [HttpPost("config")]
    public async Task<IActionResult> SaveConfig([FromBody] DashboardConfiguration config)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        config.UserId = userId;
        await _configService.SaveAsync(config);
        return Ok();
    }

    [HttpPost("widgets/{widgetId}/move")]
    public async Task<IActionResult> MoveWidget(string widgetId, [FromBody] PositionRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _configService.MoveWidgetAsync(userId, widgetId, request.Row, request.Col);
        return Ok();
    }

    [HttpPost("widgets/{widgetId}/resize")]
    public async Task<IActionResult> ResizeWidget(string widgetId, [FromBody] SizeRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _configService.ResizeWidgetAsync(userId, widgetId, request.Width, request.Height);
        return Ok();
    }

    [HttpPost("widgets/{widgetId}/toggle")]
    public async Task<IActionResult> ToggleWidget(string widgetId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _configService.ToggleWidgetAsync(userId, widgetId);
        return Ok();
    }
}
```

### 17.6 Responsive Dashboard Design

| Breakpoint | Width | Layout |
|------------|-------|--------|
| Desktop | > 1200px | 12 sütun grid |
| Tablet | 768-1199px | 8 sütun grid |
| Mobile | < 768px | 4 sütun grid |

```csharp
// Responsive layout engine
public class ResponsiveLayoutEngine
{
    public DashboardLayout CalculateLayout(string viewport)
    {
        return viewport switch
        {
            "desktop" => new DashboardLayout
            {
                Columns = 12,
                RowHeight = 100,
                Margin = 16,
                Widgets = GetDesktopWidgets()
            },
            "tablet" => new DashboardLayout
            {
                Columns = 8,
                RowHeight = 80,
                Margin = 12,
                Widgets = GetTabletWidgets()
            },
            "mobile" => new DashboardLayout
            {
                Columns = 4,
                RowHeight = 60,
                Margin = 8,
                Widgets = GetMobileWidgets()
            },
            _ => GetDefaultLayout()
        };
    }
}
```

### 17.7 Export & Reporting

```csharp
// Dashboard export service
public class DashboardExportService
{
    private readonly IPrometheusClient _prometheus;
    private readonly IReportGenerator _reportGenerator;

    public async Task<byte[]> ExportToPdf(string dashboardId, DateTimeRange range)
    {
        var data = await CollectDashboardDataAsync(dashboardId, range);
        
        var report = await _reportGenerator.GenerateAsync(new ReportRequest
        {
            Title = $"VersaCoder Dashboard Report",
            DateRange = range,
            Sections = new[]
            {
                new ReportSection
                {
                    Title = "System Overview",
                    Content = data.SystemStatus
                },
                new ReportSection
                {
                    Title = "Agent Performance",
                    Content = data.AgentPerformance
                },
                new ReportSection
                {
                    Title = "LLM Usage",
                    Content = data.LlmUsage
                }
            }
        });

        return report;
    }

    public async Task<ExportResult> ExportToCsv(string dashboardId, DateTimeRange range)
    {
        var data = await CollectDashboardDataAsync(dashboardId, range);
        var csv = GenerateCsv(data);
        
        return new ExportResult
        {
            Content = csv,
            ContentType = "text/csv",
            FileName = $"dashboard-export-{DateTime.UtcNow:yyyyMMdd}.csv"
        };
    }

    public async Task<ExportResult> ExportToJson(string dashboardId, DateTimeRange range)
    {
        var data = await CollectDashboardDataAsync(dashboardId, range);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return new ExportResult
        {
            Content = json,
            ContentType = "application/json",
            FileName = $"dashboard-export-{DateTime.UtcNow:yyyyMMdd}.json"
        };
    }
}
```

### 17.8 Dashboard API Endpoints

| Endpoint | Method | Amaç |
|----------|--------|------|
| `/api/dashboard/config` | GET | Dashboard konfigürasyonu al |
| `/api/dashboard/config` | POST | Dashboard konfigürasyonu kaydet |
| `/api/dashboard/widgets/{id}/move` | POST | Widget taşı |
| `/api/dashboard/widgets/{id}/resize` | POST | Widget boyutlandır |
| `/api/dashboard/widgets/{id}/toggle` | POST | Widget göster/gizle |
| `/api/dashboard/export/pdf` | GET | PDF olarak dışa aktar |
| `/api/dashboard/export/csv` | GET | CSV olarak dışa aktar |
| `/api/dashboard/export/json` | GET | JSON olarak dışa aktar |
| `/hubs/dashboard` | WebSocket | Real-time veri akışı |

---

## 18. Gerçek Kod Durumu (Audit - 2026-08-26)

### 15.1 Çalışan Katmanlar

| Proje | Katman | Satır | Durum | Önem |
|-------|--------|-------|-------|------|
| VersaCoder.Domain | L0 | ~800 | ✅ Çalışıyor | ZORUNLU |
| VersaCoder.Abstractions | L1 | ~600 | ✅ Çalışıyor | ZORUNLU |
| VersaCoder.Application | L2 | ~2500 | ✅ Çalışıyor | ZORUNLU |
| VersaCoder.CrossCutting | L3 | ~200 | ✅ Çalışıyor | ZORUNLU |
| VersaCoder.Infrastructure.Data | L4.1 | ~1200 | ✅ Çalışıyor | ZORUNLU |
| VersaCoder.Infrastructure.AI | L4.2 | ~800 | ✅ Çalışıyor | ZORUNLU |
| VersaCoder.Infrastructure.Logging | L4.28 | ~275 | ✅ Çalışıyor | ORTA |
| VersaCoder.Infrastructure.Reporting | L4.29 | ~310 | ✅ Çalışıyor | ORTA |
| VersaCoder.Host | L6 | ~65 | ✅ Çalışıyor | ZORUNLU |

### 15.2 Boş Stub Projeler (26 Proje)

| Proje | Katman | Hedef | Öncelik |
|-------|--------|-------|---------|
| VersaCoder.Protocol | L5 | MCP protokolü | YÜKSEK |
| VersaCoder.Infrastructure.Git | L4.22 | LibGit2Sharp | YÜKSEK |
| VersaCoder.Infrastructure.MCP | L4.3 | MCP client/server | YÜKSEK |
| VersaCoder.Infrastructure.Context | L4.14 | Context assembly | YÜKSEK |
| VersaCoder.Infrastructure.Config | L4.5 | Uygulama ayarları | YÜKSEK |
| VersaCoder.Infrastructure.FileSystem | L4.10 | Dosya sistemi | YÜKSEK |
| VersaCoder.Infrastructure.Auth | L4.4 | API key yönetimi | ORTA |
| VersaCoder.Infrastructure.Security | L4.12 | Şifreleme, token | ORTA |
| VersaCoder.Infrastructure.Plugins | L4.6 | Plugin sistemi | ORTA |
| VersaCoder.Infrastructure.Services | L4.7 | Yardımcı servisler | ORTA |
| VersaCoder.Infrastructure.Caching | L4.8 | Önbellek | ORTA |
| VersaCoder.Infrastructure.Network | L4.11 | HTTP/WebSocket | ORTA |
| VersaCoder.Infrastructure.Messaging | L4.9 | Event bus | DÜŞÜK |
| VersaCoder.Infrastructure.Diagram | L4.16 | Diyagram işleme | DÜŞÜK |
| VersaCoder.Infrastructure.Documentation | L4.19 | Otomatik doküman | DÜŞÜK |
| VersaCoder.Infrastructure.Learning | L4.15 | Öğrenme persistansı | DÜŞÜK |
| VersaCoder.Infrastructure.Backup | L4.26 | Yedekleme | DÜŞÜK |
| VersaCoder.Infrastructure.ProjectAnalysis | L4.17 | Proje analizi | DÜŞÜK |
| VersaCoder.Infrastructure.Versioning | L4.27 | Versiyon yönetimi | DÜŞÜK |
| VersaCoder.Infrastructure.Integration | L4.23 | Dış entegrasyon | DÜŞÜK |
| VersaCoder.Infrastructure.Testing | L4.18 | Test altyapısı | DÜŞÜK |
| VersaCoder.Infrastructure.CodeAnalysis | L4.21 | Roslyn/AST | DÜŞÜK |
| VersaCoder.Infrastructure.Observability | L4.13 | Monitoring | DÜŞÜK |
| VersaCoder.Infrastructure.Templating | L4.24 | Şablon motoru | DÜŞÜK |
| VersaCoder.Infrastructure.Refactoring | L4.20 | Refactoring araçları | DÜŞÜK |
| VersaCoder.Infrastructure.Deployment | L4.25 | Dağıtım | DÜŞÜK |

### 15.3 UI Durumu

| Bileşen | Durum |
|---------|-------|
| VersaCoder.UI (L7) | ❌ Boş form |
| DevExpress Ribbon | Henüz eklenmemiş |
| MVVM (CommunityToolkit) | Henüz kullanılmamış |
| MDI Container | Henüz eklenmemiş |

### 15.4 Kritik Eksikler Sıralaması

```
1. UI katmanı (DevExpress WinForms + MDI + Ribbon)     → YÜKSEK
2. MCP protokolü (Protocol projesi)                     → YÜKSEK
3. Context yönetimi (vault/file/project context)        → YÜKSEK
4. Git entegrasyonu (LibGit2Sharp)                      → YÜKSEK
5. Configuration sistemi                                → YÜKSEK
6. FileSystem servisleri                                → YÜKSEK
7. Auth/Security                                        → ORTA
8. Plugin sistemi                                       → ORTA
9. Caching                                              → ORTA
10. Network servisleri                                  → ORTA
```

---

## 19. Teknoloji Kararları

### 16.1 Teknoloji Seçim Matrisi

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
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode