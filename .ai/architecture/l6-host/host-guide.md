---
title: "Versa Coder — L6 Host Layer Guide"
type: architecture
category: layer
layer: L6
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L6 Host Layer Guide

**Zorunlu Bağlantılar:** [[architecture/l5-protocol/protocol-guide]] · [[brain.md]]

---

## 1. Amaç

Host katmanı, uygulama **başlangıcını, DI (Dependency Injection) konfigürasyonunu ve uygulama yaşam döngüsünü** yönetir.

---

## 2. Başlangıç Akışı

```
Program.cs → Host.CreateDefaultBuilder()
    → ConfigureServices()
        ├── Infrastructure.Data DI
        ├── Infrastructure.AI DI
        ├── Infrastructure.Logging DI
        ├── Infrastructure.Reporting DI
        ├── Application DI (MediatR)
        └── CrossCutting DI
    → Build()
    → Run()
```

---

## 3. DI Konfigürasyonu

### 3.1 Infrastructure.Data

```csharp
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
```

### 3.2 Infrastructure.AI

```csharp
services.AddSingleton<ProviderRouter>();
services.AddScoped<IAgentRunner, AgentRunner>();
services.AddSingleton<ToolRegistry>();
services.AddSingleton<ILLMProvider, OpenAIProvider>();
services.AddSingleton<ILLMProvider, AnthropicProvider>();
services.AddSingleton<ILLMProvider, OllamaProvider>();
services.AddSingleton<ILLMProvider, CustomProvider>();
```

### 3.3 Application

```csharp
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateSessionCommand).Assembly));
services.AddAutoMapper(typeof(MappingProfile).Assembly);
services.AddScoped<ISessionManager, SessionManagerService>();
services.AddScoped<IContextManager, ContextManagerService>();
services.AddScoped<ILogService, LogService>();
services.AddScoped<ILearningService, LearningService>();
```

### 3.4 CrossCutting

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

---

## 4. appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;"
  },
  "AI": {
    "DefaultProvider": "OpenAI",
    "Providers": {
      "OpenAI": {
        "ApiKey": "${OPENAI_API_KEY}",
        "BaseUrl": "https://api.openai.com/v1",
        "Models": ["gpt-4o", "gpt-4.1", "o3"],
        "DefaultModel": "gpt-4o",
        "Timeout": 30,
        "MaxRetries": 3
      },
      "Anthropic": {
        "ApiKey": "${ANTHROPIC_API_KEY}",
        "BaseUrl": "https://api.anthropic.com/v1",
        "Models": ["claude-opus-4", "claude-sonnet-4"],
        "DefaultModel": "claude-sonnet-4"
      },
      "Google": {
        "ApiKey": "${GOOGLE_AI_API_KEY}",
        "Models": ["gemini-2.5-pro", "gemini-2.5-flash"]
      },
      "Ollama": {
        "BaseUrl": "http://localhost:11434",
        "Models": ["llama3.1", "qwen2.5", "codellama"]
      }
    }
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/versacoder-.log", "rollingInterval": "Day" } }
    ]
  }
}
```

---

## 5. Mevcut Durum

| Bileşen | Durum | Satır |
|---------|-------|-------|
| Startup.cs | ✅ Kısmi | 64 |
| appsettings.json | ✅ Tam | — |
| Program.cs | ✅ Tam | — |
| Eksik DI kayıtları | ❌ Logging, Reporting | — |

---

## 6. Kurallar

| # | Kural |
|---|-------|
| 1 | Host → Protocol ✅, Infrastructure ✅ |
| 2 | Host → Application ❌ (dolaylı) |
| 3 | DI registration bu katmanda |
| 4 | Startup.cs tek giriş noktası |
| 5 | appsettings.json ile yapılandırma |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
