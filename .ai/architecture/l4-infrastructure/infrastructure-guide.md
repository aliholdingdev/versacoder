---
title: "Versa Coder — L4 Infrastructure Layer Guide"
type: architecture
category: layer
layer: L4
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L4 Infrastructure Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/l3-crosscutting/crosscutting-guide]] · [[brain.md]]

---

## 1. Amaç

Infrastructure katmanı, **28 altyapı modülünü** barındırır. Her modül ayrı bir DLL class library olarak yerleşiktir.

---

## 2. Modül Haritası

| # | Modül | Durum | Tanım | Satır |
|---|-------|-------|-------|-------|
| 1 | Infrastructure.Data | ✅ Implemente | SQLite, EF Core, Repository | ~1200 |
| 2 | Infrastructure.AI | ✅ Implemente | LLM Provider, Agent Runner | ~600 |
| 3 | Infrastructure.Logging | ✅ Implemente | JsonFileLogger, structured logging | ~300 |
| 4 | Infrastructure.Reporting | 🔄 Kısmi | Excel/PDF export | ~200 |
| 5 | Infrastructure.Git | ❌ Stub | LibGit2Sharp entegrasyonu | ~0 |
| 6 | Infrastructure.Config | ❌ Stub | Uygulama ayarları | ~0 |
| 7 | Infrastructure.Context | ❌ Stub | Context assembly | ~0 |
| 8 | Infrastructure.MCP | ❌ Stub | Model Context Protocol | ~0 |
| 9 | Infrastructure.Plugins | ❌ Stub | Plugin sistemi | ~0 |
| 10 | Infrastructure.Security | ❌ Stub | Şifreleme, token | ~0 |
| 11 | Infrastructure.Caching | ❌ Stub | Önbellek yönetimi | ~0 |
| 12 | Infrastructure.Messaging | ❌ Stub | Event bus, messaging | ~0 |
| 13 | Infrastructure.FileSystem | ❌ Stub | Dosya sistemi | ~0 |
| 14 | Infrastructure.Network | ❌ Stub | HTTP, WebSocket | ~0 |
| 15 | Infrastructure.Auth | ❌ Stub | Kimlik doğrulama | ~0 |
| 16 | Infrastructure.Observability | ❌ Stub | Monitoring, metrics | ~0 |
| 17 | Infrastructure.Learning | ❌ Stub | Pattern, düzeltme | ~0 |
| 18 | Infrastructure.Diagram | ❌ Stub | Diyagram okuma | ~0 |
| 19 | Infrastructure.ProjectAnalysis | ❌ Stub | Roslyn tabanlı analiz | ~0 |
| 20 | Infrastructure.Testing | ❌ Stub | Test altyapısı | ~0 |
| 21 | Infrastructure.Documentation | ❌ Stub | Otomatik doc | ~0 |
| 22 | Infrastructure.Refactoring | ❌ Stub | Refactoring | ~0 |
| 23 | Infrastructure.CodeAnalysis | ❌ Stub | Kod analizi | ~0 |
| 24 | Infrastructure.Integration | ❌ Stub | Üçüncü parti | ~0 |
| 25 | Infrastructure.Templating | ❌ Stub | Şablon sistemi | ~0 |
| 26 | Infrastructure.Deployment | ❌ Stub | Dağıtım | ~0 |
| 27 | Infrastructure.Backup | ❌ Stub | Yedekleme | ~0 |
| 28 | Infrastructure.Versioning | ❌ Stub | Versiyon | ~0 |
| 29 | Infrastructure.Services | ❌ Stub | Yardımcı servisler | ~0 |

---

## 3. Implemente Edilmiş Modüller

### 3.1 Infrastructure.Data

| Bileşen | Dosya | Tanım |
|---------|-------|-------|
| `VersaCoderDbContext` | `Context/VersaCoderDbContext.cs` | EF Core DbContext, 12 DbSet |
| `Repository<T>` | `Repositories/Repository.cs` | Genel repository, 52 satır |
| `SessionRepository` | `Repositories/SessionRepository.cs` | Session CRUD |
| `MessageRepository` | `Repositories/MessageRepository.cs` | Message CRUD |
| `ProjectRepository` | `Repositories/ProjectRepository.cs` | Project CRUD |
| `FileRepository` | `Repositories/FileRepository.cs` | FileEntry CRUD |
| `LearningRepository` | `Repositories/LearningRepository.cs` | Learning CRUD |
| `SettingRepository` | `Repositories/SettingRepository.cs` | Setting CRUD |
| `TaskRepository` | `Repositories/TaskRepository.cs` | Task CRUD (34 method) |
| `TaskListRepository` | `Repositories/TaskListRepository.cs` | TaskList CRUD |
| `AuditLogRepository` | `Repositories/AuditLogRepository.cs` | AuditLog CRUD |
| 12 Entity Config | `Configurations/` | EF config, indexes, relationships |
| `DependencyInjection.cs` | DI registration | SQLite WAL, all repos |

### 3.2 Infrastructure.AI

| Bileşen | Dosya | Tanım |
|---------|-------|-------|
| `AgentRunner` | `AgentRunner.cs` | IAgentRunner implementasyonu, 174 satır |
| `ProviderRouter` | `ProviderRouter.cs` | Multi-provider routing, 53 satır |
| `ToolRegistry` | `ToolRegistry.cs` | 5 built-in tool, 147 satır |
| `OpenAIProvider` | `Providers/OpenAIProvider.cs` | OpenAI entegrasyonu (streaming) |
| `AnthropicProvider` | `Providers/AnthropicProvider.cs` | Anthropic entegrasyonu |
| `OllamaProvider` | `Providers/OllamaProvider.cs` | Ollama entegrasyonu |
| `CustomProvider` | `Providers/CustomProvider.cs` | Özel provider |
| `DependencyInjection.cs` | DI registration | Tüm provider'lar |

### 3.3 Infrastructure.Logging

| Bileşen | Dosya | Tanım |
|---------|-------|-------|
| `JsonFileLogger` | `JsonFileLogger.cs` | Thread-safe, append-only, rotation, 275 satır |
| `DependencyInjection.cs` | DI registration | Logger kaydı |

### 3.4 Infrastructure.Reporting

| Bileşen | Dosya | Tanım |
|---------|-------|-------|
| `ExcelExporter` | `ExcelExporter.cs` | EPPlus tabanlı |
| `PdfExporter` | `PdfExporter.cs` | PDFsharp tabanlı |
| `DependencyInjection.cs` | DI registration | Exporter kaydı |

---

## 4. DI Registration

```csharp
// Infrastructure.Data
services.AddDbContext<VersaCoderDbContext>(options =>
    options.UseSqlite("Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;"));
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<ISessionRepository, SessionRepository>();
services.AddScoped<IMessageRepository, MessageRepository>();
services.AddScoped<IProjectRepository, ProjectRepository>();
services.AddScoped<IFileRepository, FileRepository>();
services.AddScoped<ILearningRepository, LearningRepository>();
services.AddScoped<ISettingRepository, SettingRepository>();
services.AddScoped<ITaskRepository, TaskRepository>();
services.AddScoped<ITaskListRepository, TaskListRepository>();
services.AddScoped<IAuditLogRepository, AuditLogRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Infrastructure.AI
services.AddSingleton<ProviderRouter>();
services.AddScoped<IAgentRunner, AgentRunner>();
services.AddSingleton<ToolRegistry>();
services.AddSingleton<ILLMProvider, OpenAIProvider>();
services.AddSingleton<ILLMProvider, AnthropicProvider>();
services.AddSingleton<ILLMProvider, OllamaProvider>();
```

---

## 5. OpenCode Eşleştirme

| VersaCoder Modülü | OpenCode Karşılığı | Durum |
|-------------------|-------------------|-------|
| Infrastructure.Data | `core/src/database/` | ✅ Eşleşti |
| Infrastructure.AI | `llm/src/providers/` | ✅ Eşleşti |
| Infrastructure.Logging | `core/src/log.ts` | ✅ Eşleşti |
| Infrastructure.Git | `core/src/git/` | ❌ Eksik |
| Infrastructure.MCP | `packages/protocol/` | ❌ Eksik |
| Infrastructure.Plugins | `core/src/plugin.ts` | ❌ Eksik |
| Infrastructure.Config | `core/src/config.ts` | ❌ Eksik |

---

## 6. Kurallar

| # | Kural |
|---|-------|
| 1 | Infrastructure → CrossCutting ✅, Application ❌, Domain ❌ |
| 2 | Her modül ayrı proje |
| 3 | Dependency Injection zorunlu |
| 4 | Interface-first tasarım |
| 5 | SQLite WAL modu zorunlu |
| 6 | EF Core DbContext ONLY (Dapper yasak) |

---

## 6. Infrastructure.Data Detayları

### 6.1 VersaCoderDbContext

```csharp
public class VersaCoderDbContext : DbContext
{
    public VersaCoderDbContext(DbContextOptions<VersaCoderDbContext> options)
        : base(options) { }
    
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VersaCoderDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<string>()
            .HaveMaxLength(500);
        
        builder.Properties<decimal>()
            .HavePrecision(18, 2);
    }
}
```

### 6.2 Repository<T>

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly VersaCoderDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(VersaCoderDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }
    
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }
    
    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(ct);
    }
    
    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }
    
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    
    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);
    }
}
```

### 6.3 SessionRepository

```csharp
public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(VersaCoderDbContext context) : base(context) { }
    
    public async Task<Session?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name, ct);
    }
    
    public async Task<IReadOnlyList<Session>> GetByProjectIdAsync(
        Guid projectId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
    
    public override async Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
```

---

## 7. Infrastructure.AI Detayları

### 7.1 AgentRunner

```csharp
public class AgentRunner : IAgentRunner
{
    private readonly ProviderRouter _router;
    private readonly ToolRegistry _tools;
    private readonly ILogger<AgentRunner> _logger;
    
    public AgentRunner(
        ProviderRouter router,
        ToolRegistry tools,
        ILogger<AgentRunner> logger)
    {
        _router = router;
        _tools = tools;
        _logger = logger;
    }
    
    public async Task<AgentResponse> RunAsync(
        AgentRequest request, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation(
                "Running agent: {AgentRole} with model: {Model}",
                request.AgentRole, request.ModelName);
            
            var provider = _router.GetProvider(request.ProviderName);
            var tools = _tools.GetToolsForAgent(request.AgentRole);
            
            var llmRequest = new LLMRequest
            {
                Messages = request.Messages,
                Model = request.ModelName,
                Tools = tools.Select(t => t.ToToolDefinition()).ToList()
            };
            
            var response = await provider.CompleteAsync(llmRequest, ct);
            
            return new AgentResponse
            {
                Content = response.Content,
                TokenCount = response.TokenCount,
                Model = request.ModelName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent runner failed");
            throw;
        }
    }
}
```

### 7.2 ProviderRouter

```csharp
public class ProviderRouter
{
    private readonly Dictionary<string, ILLMProvider> _providers;
    private readonly ILogger<ProviderRouter> _logger;
    
    public ProviderRouter(
        IEnumerable<ILLMProvider> providers,
        ILogger<ProviderRouter> logger)
    {
        _providers = providers.ToDictionary(p => p.Name, p => p);
        _logger = logger;
    }
    
    public ILLMProvider GetProvider(string name)
    {
        if (_providers.TryGetValue(name, out var provider))
            return provider;
        
        _logger.LogWarning("Provider {Name} not found, using default", name);
        return _providers.Values.First();
    }
    
    public IReadOnlyList<string> GetAvailableProviders()
    {
        return _providers.Keys.ToList().AsReadOnly();
    }
}
```

### 7.3 OpenAIProvider

```csharp
public class OpenAIProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAISettings _settings;
    private readonly ILogger<OpenAIProvider> _logger;
    
    public string Name => "OpenAI";
    
    public OpenAIProvider(
        HttpClient httpClient,
        OpenAISettings settings,
        ILogger<OpenAIProvider> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }
    
    public async Task<LLMResponse> CompleteAsync(
        LLMRequest request, CancellationToken ct = default)
    {
        try
        {
            var apiRequest = new
            {
                model = request.Model ?? _settings.DefaultModel,
                messages = request.Messages.Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                }).ToArray()
            };
            
            var json = JsonSerializer.Serialize(apiRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(
                $"{_settings.BaseUrl}/v1/chat/completions", content, ct);
            
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);
            
            return new LLMResponse
            {
                Content = result?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty,
                TokenCount = result?.Usage?.TotalTokens ?? 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI provider failed");
            throw;
        }
    }
}
```

---

## 8. Entity Configurations

### 8.1 SessionConfiguration

```csharp
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(s => s.CreatedAt)
            .IsRequired();
        
        builder.HasMany(s => s.Messages)
            .WithOne(m => m.Session)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.CreatedAt);
    }
}
```

### 8.2 MessageConfiguration

```csharp
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Role)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(m => m.Content)
            .IsRequired();
        
        builder.Property(m => m.TokenCount)
            .HasDefaultValue(0);
        
        builder.HasIndex(m => m.SessionId);
        builder.HasIndex(m => m.CreatedAt);
    }
}
```

---

## 9. Configuration Patterları

### 9.1 Settings Sınıfları

```csharp
// OpenAISettings.cs
public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com";
    public string DefaultModel { get; set; } = "gpt-4";
    public int MaxTokens { get; set; } = 4096;
    public double Temperature { get; set; } = 0.7;
}

// DatabaseSettings.cs
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = "Data Source=versacoder.db";
    public bool EnableWAL { get; set; } = true;
    public int CommandTimeout { get; set; } = 30;
}

// UiSettings.cs
public class UiSettings
{
    public string Theme { get; set; } = "Dark";
    public string Language { get; set; } = "tr-TR";
    public bool ShowLineNumbers { get; set; } = true;
    public int FontSize { get; set; } = 14;
}
```

### 9.2 Options Pattern

```csharp
// Startup.cs'de yapılandırma
services.Configure<OpenAISettings>(
    configuration.GetSection("Ai:OpenAI"));
services.Configure<DatabaseSettings>(
    configuration.GetSection("Database"));
services.Configure<UiSettings>(
    configuration.GetSection("Ui"));
```

---

## 10. Migration İşlemleri

### 10.1 Migration Oluşturma

```bash
# Migration oluşturma
dotnet ef migrations add InitialCreate \
    --project src/VersaCoder.Infrastructure.Data \
    --startup-project src/VersaCoder.Host

# Migration uygulama
dotnet ef database update \
    --project src/VersaCoder.Infrastructure.Data \
    --startup-project src/VersaCoder.Host

# Migration silme (son eklenen)
dotnet ef migrations remove \
    --project src/VersaCoder.Infrastructure.Data \
    --startup-project src/VersaCoder.Host
```

### 10.2 Auto-Migration

```csharp
// Program.cs'de otomatik migration
public static async Task Main(string[] args)
{
    var host = CreateHostBuilder(args).Build();
    
    using var scope = host.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<VersaCoderDbContext>();
    
    // Otomatik migration
    await context.Database.MigrateAsync();
    
    await host.RunAsync();
}
```

---

## 11. Infrastructure.Testleri

### 11.1 Repository Testleri

```csharp
public class SessionRepositoryTests : IDisposable
{
    private readonly VersaCoderDbContext _context;
    private readonly SessionRepository _repository;
    
    public SessionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VersaCoderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new VersaCoderDbContext(options);
        _repository = new SessionRepository(_context);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddSession()
    {
        // Arrange
        var session = new Session("Test Session", Guid.NewGuid());
        
        // Act
        await _repository.AddAsync(session);
        await _context.SaveChangesAsync();
        
        // Assert
        var result = await _repository.GetByIdAsync(session.Id);
        Assert.NotNull(result);
        Assert.Equal("Test Session", result.Name);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

---

## 12. Infrastructure Gelecek Planı

### 12.1 Kısa Vadeli (1-2 hafta)

| Modül | Görev | Öncelik |
|-------|-------|---------|
| Config | Ayarları yapılandır | Yüksek |
| FileSystem | Dosya işlemleri | Yüksek |
| Auth | Kimlik doğrulama | Yüksek |

### 12.2 Orta Vadeli (1-2 ay)

| Modül | Görev | Öncelik |
|-------|-------|---------|
| Security | Şifreleme | Orta |
| Caching | Önbellek | Orta |
| Observability | Monitoring | Orta |

### 12.3 Uzun Vadeli (3-6 ay)

| Modül | Görev | Öncelik |
|-------|-------|---------|
| Plugins | Plugin sistemi | Düşük |
| Deployment | Dağıtım | Düşük |
| Backup | Yedekleme | Düşük |

---

## 13. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Total Modules | 27 |
| Implemented | 2 (Data, AI) |
| Stub | 25 |
| Entity Configurations | 2 |
| Repositories | 3 |
| Providers | 4 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
<<<<<<< HEAD
=======
**Mode:** Red Team · Human Mode · Truth Mode
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
