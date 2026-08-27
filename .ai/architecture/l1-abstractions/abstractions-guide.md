---
title: "Versa Coder — L1 Abstractions Layer Guide"
type: architecture
category: layer
layer: L1
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L1 Abstractions Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/l0-domain/domain-guide]] · [[brain.md]]

---

## 1. Amaç

Abstractions katmanı, tüm katmanların uyması gereken **arayüzleri ve kontratları** tanımlar. Interface Segregation Principle (ISP) uygulanır.

---

## 2. Servis Arayüzleri

### 2.1 Core Servisler

| Arayüz | Dosya | Tanım | Method Sayısı |
|--------|-------|-------|---------------|
| `IAgentRunner` | `Services/IAgentRunner.cs` | Agent çalıştırma | 2 (RunAsync, RunStreamingAsync) |
| `IContextManager` | `Services/IContextManager.cs` | Context yönetimi | 3 (Assemble, Get, Update) |
| `ISessionManager` | `Services/ISessionManager.cs` | Session yönetimi | 12 (CRUD, branching, fork, merge) |
| `IToolExecutor` | `Services/IToolExecutor.cs` | Tool çalıştırma | 2 (Execute, GetAvailableTools) |
| `ILogService` | `Services/ILogService.cs` | Log yönetimi | 8 (Write, Read, Statistics, Export) |
| `ILearningService` | `Services/ILearningService.cs` | Öğrenme sistemi | 4 (Record, Query) |
| `IGitService` | `Services/IGitService.cs` | Git işlemleri | 10 (Status, Diff, Commit, Push, Pull) |
| `IProjectAnalyzer` | `Services/IProjectAnalyzer.cs` | Proje analizi | 1 (AnalyzeAsync) |
| `IReportService` | `Services/IReportService.cs` | Raporlama | 10 (10 farklı rapor türü) |
| `IDiagramTeacher` | `Services/IDiagramTeacher.cs` | Diyagram öğretme | 2 (TeachDiagram, ConvertToCode) |
| `ITemplateService` | `Services/ITemplateService.cs` | Şablon sistemi | 3 (Render, List, GetContent) |

### 2.2 Task Management Servisleri

| Arayüz | Dosya | Tanım | Method Sayısı |
|--------|-------|-------|---------------|
| `ITaskManager` | `Services/ITaskManager.cs` | Görev yönetimi | 30+ (CRUD, state machine, subtasks, dependencies, tags, reminders) |
| `ITaskListManager` | `Services/ITaskListManager.cs` | Görev listesi | 10 |
| `ITagManager` | `Services/ITagManager.cs` | Etiket yönetimi | 8 |

---

## 3. Repository Arayüzleri

| Arayüz | Dosya | Tanım | Method Sayısı |
|--------|-------|-------|---------------|
| `IRepository<T>` | `Repositories/IRepository.cs` | Genel repository | 5 (GetById, GetAll, Add, Update, Delete) |
| `ISessionRepository` | `Repositories/ISessionRepository.cs` | Session CRUD | 8 |
| `IMessageRepository` | `Repositories/IMessageRepository.cs` | Message CRUD | 6 |
| `IProjectRepository` | `Repositories/IProjectRepository.cs` | Project CRUD | 6 |
| `IFileRepository` | `Repositories/IFileRepository.cs` | File CRUD | 6 |
| `ILearningRepository` | `Repositories/ILearningRepository.cs` | Learning CRUD | 6 |
| `ISettingRepository` | `Repositories/ISettingRepository.cs` | Setting CRUD | 6 |
| `ITaskRepository` | `Repositories/ITaskRepository.cs` | Task CRUD | 34 |
| `ITaskListRepository` | `Repositories/ITaskListRepository.cs` | TaskList CRUD | 10 |
| `IAuditLogRepository` | `Repositories/IAuditLogRepository.cs` | AuditLog CRUD | 6 |

---

## 4. Provider Arayüzleri

| Arayüz | Dosya | Tanım |
|--------|-------|-------|
| `ILLMProvider` | `Providers/ILLMProvider.cs` | LLM sağlayıcı (Name, IsAvailable, CompleteAsync, StreamAsync) |
| `IEmbeddingProvider` | `Providers/IEmbeddingProvider.cs` | Embedding sağlayıcı |

---

## 5. Plugin Arayüzleri

| Arayüz | Dosya | Tanım |
|--------|-------|-------|
| `IPlugin` | `Plugins/IPlugin.cs` | Plugin tanımı |
| `IPluginManager` | `Plugins/IPluginManager.cs` | Plugin yönetimi |

---

## 6. Kurallar

| # | Kural |
|---|-------|
| 1 | Hiçbir implementasyon yok — sadece arayüz |
| 2 | Domain'e bağımlı (L1 → L0 ✅) |
| 3 | Hiçbir başka katmana bağımlı değil |
| 4 | Interface Segregation — her arayüz tek sorumluluk |
| 5 | DTO'lar simple data transfer — behavior yok |

---

## 6. Servis Arayüz Detayları

### 6.1 IAgentRunner

```csharp
public interface IAgentRunner
{
    Task<AgentResult> RunAsync(
        AgentRole agent,
        string task,
        CancellationToken cancellationToken = default);
    
    Task<AgentResult> RunAsync(
        AgentRole agent,
        string task,
        Context context,
        CancellationToken cancellationToken = default);
    
    bool IsAgentAvailable(AgentRole agent);
    
    IReadOnlyList<AgentRole> GetAvailableAgents();
}
```

### 6.2 IContextManager

```csharp
public interface IContextManager
{
    Task<Context> GetCurrentContextAsync();
    
    Task UpdateContextAsync(ContextUpdate update);
    
    Task<Context> LoadContextAsync(Guid sessionId);
    
    Task SaveContextAsync(Context context);
    
    Task CompactContextAsync(Context context);
}
```

### 6.3 ISessionManager

```csharp
public interface ISessionManager
{
    Task<Session> CreateSessionAsync(string name, Guid projectId);
    
    Task<Session?> GetSessionAsync(Guid sessionId);
    
    Task<IReadOnlyList<Session>> GetAllSessionsAsync();
    
    Task UpdateSessionAsync(Session session);
    
    Task DeleteSessionAsync(Guid sessionId);
    
    Task PauseSessionAsync(Guid sessionId);
    
    Task ResumeSessionAsync(Guid sessionId);
    
    Task CompleteSessionAsync(Guid sessionId);
}
```

### 6.4 IGitService

```csharp
public interface IGitService
{
    Task<string> GetCurrentBranchAsync(string repositoryPath);
    
    Task<IReadOnlyList<string>> GetBranchesAsync(string repositoryPath);
    
    Task<CommitResult> CommitAsync(
        string repositoryPath,
        string message,
        IEnumerable<string> files);
    
    Task PushAsync(string repositoryPath, string branch);
    
    Task PullAsync(string repositoryPath, string branch);
    
    Task<StatusResult> GetStatusAsync(string repositoryPath);
}
```

### 6.5 ILearningService

```csharp
public interface ILearningService
{
    Task<LearningEntry> RecordPatternAsync(string pattern, string description);
    
    Task<LearningEntry> RecordCorrectionAsync(string error, string correction);
    
    Task<LearningEntry> RecordKnowledgeAsync(string key, string value);
    
    Task<IReadOnlyList<LearningEntry>> GetPatternsAsync();
    
    Task<IReadOnlyList<LearningEntry>> GetCorrectionsAsync();
    
    Task<IReadOnlyList<LearningEntry>> GetKnowledgeAsync();
    
    Task<LearningEntry?> FindRelevantLearningAsync(string context);
}
```

### 6.6 IProjectAnalyzer

```csharp
public interface IProjectAnalyzer
{
    Task<ProjectAnalysis> AnalyzeProjectAsync(string projectPath);
    
    Task<IReadOnlyList<string>> GetDependenciesAsync(string projectPath);
    
    Task<IReadOnlyList<string>> GetSourceFilesAsync(string projectPath);
    
    Task<CodeMetrics> GetCodeMetricsAsync(string projectPath);
    
    Task<IReadOnlyList<string>> FindPatternsAsync(string projectPath);
}
```

### 6.7 ITemplateService

```csharp
public interface ITemplateService
{
    Task<Template?> GetTemplateAsync(string templateName);
    
    Task<IReadOnlyList<Template>> GetAllTemplatesAsync();
    
    Task<string> RenderTemplateAsync(
        string templateName,
        Dictionary<string, object> parameters);
    
    Task SaveTemplateAsync(Template template);
    
    Task DeleteTemplateAsync(string templateName);
}
```

### 6.8 IToolExecutor

```csharp
public interface IToolExecutor
{
    Task<ToolResult> ExecuteAsync(
        string toolName,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<ToolInfo>> GetAvailableToolsAsync();
    
    Task<ToolInfo?> GetToolInfoAsync(string toolName);
    
    bool IsToolAvailable(string toolName);
}
```

---

## 7. Repository Arayüz Detayları

### 7.1 Genel Repository (IRepository<T>)

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
}
```

### 7.2 ISessionRepository

```csharp
public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyList<Session>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Session>> GetByStatusAsync(
        SessionState status,
        CancellationToken cancellationToken = default);
    
    Task<Session?> GetWithMessagesAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Session>> GetRecentSessionsAsync(
        int count,
        CancellationToken cancellationToken = default);
}
```

### 7.3 IMessageRepository

```csharp
public interface IMessageRepository : IRepository<Message>
{
    Task<IReadOnlyList<Message>> GetBySessionIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Message>> GetByRoleAsync(
        Guid sessionId,
        string role,
        CancellationToken cancellationToken = default);
    
    Task<Message?> GetLastMessageAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
    
    Task<int> GetTokenCountAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}
```

---

## 8. Provider Arayüz Detayları

### 8.1 ILLMProvider

```csharp
public interface ILLMProvider
{
    string ProviderName { get; }
    
    bool IsAvailable { get; }
    
    Task<LLMResponse> CompleteAsync(
        LLMRequest request,
        CancellationToken cancellationToken = default);
    
    Task<IAsyncEnumerable<LLMResponse>> StreamAsync(
        LLMRequest request,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<string>> GetAvailableModelsAsync();
    
    Task<ProviderStatus> GetStatusAsync();
}
```

### 8.2 IEmbeddingProvider

```csharp
public interface IEmbeddingProvider
{
    string ProviderName { get; }
    
    bool IsAvailable { get; }
    
    Task<IReadOnlyList<float>> GetEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<IReadOnlyList<float>>> GetEmbeddingsAsync(
        IReadOnlyList<string> texts,
        CancellationToken cancellationToken = default);
    
    int GetDimension();
}
```

---

## 9. DTO'lar

### 9.1 Context DTO

```csharp
public record Context
{
    public Guid SessionId { get; init; }
    public IReadOnlyList<ContextSource> Sources { get; init; }
    public IReadOnlyList<string> ActiveFiles { get; init; }
    public string CurrentTask { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
}
```

### 9.2 AgentResult DTO

```csharp
public record AgentResult
{
    public bool Success { get; init; }
    public string Output { get; init; }
    public IReadOnlyList<string> FilesModified { get; init; }
    public TimeSpan Duration { get; init; }
    public string? Error { get; init; }
}
```

### 9.3 ToolResult DTO

```csharp
public record ToolResult
{
    public bool Success { get; init; }
    public object? Output { get; init; }
    public string? Error { get; init; }
    public TimeSpan Duration { get; init; }
}
```

---

## 10. Abstractions Kullanım Kalıpları

### 10.1 Dependency Injection Kalıbı

```csharp
// Startup.cs'de DI kayıtları
services.AddScoped<ISessionManager, SessionManager>();
services.AddScoped<ISessionRepository, SessionRepository>();
services.AddScoped<ILLMProvider, OpenAIProvider>();
services.AddScoped<IToolExecutor, ToolExecutor>();
```

### 10.2 Interface Segregation Kalıbı

```csharp
// Tek sorumluluk prensibi
public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
}

public interface IWriteRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Okuma-yazma ayrı
public interface ISessionRepository : IReadRepository<Session>, IWriteRepository<Session>
{
    // Özel metodlar
}
```

---

## 11. Abstractions Testleri

### 11.1 Mock Kullanımı

```csharp
// Testlerde mock kullanımı
public class SessionServiceTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SessionService _service;
    
    public SessionServiceTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new SessionService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task GetByIdAsync_ExistingSession_ReturnsSession()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session("Test", Guid.NewGuid());
        _repositoryMock.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        
        // Act
        var result = await _service.GetByIdAsync(sessionId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(sessionId, result.Id);
    }
}
```

---

## 12. Abstractions Gelecek Planı

### 12.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Yeni servis arayüzleri | Yüksek |
| Repository zenginleştirme | Yüksek |
| DTO'ları güncelleme | Orta |

### 12.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Yeni provider arayüzleri | Orta |
| Abstraction zenginleştirme | Orta |
| Validation integration | Düşük |

### 12.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Generic repository pattern | Orta |
| Specification pattern | Düşük |
| Unit of work pattern | Düşük |

---

## 13. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Service Interfaces | 9 |
| Repository Interfaces | 7 |
| Provider Interfaces | 2 |
| DTO Types | 3 |
| Design Patterns | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
<<<<<<< HEAD
=======
**Mode:** Red Team · Human Mode · Truth Mode
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
