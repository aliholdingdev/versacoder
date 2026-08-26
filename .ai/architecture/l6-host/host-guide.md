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
        ├── Application DI (MediatR)
        └── CrossCutting DI
    → Build()
    → Run()
```

---

## 3. DI Konfigürasyonu

```csharp
// Startup.cs
public static void ConfigureServices(IServiceCollection services)
{
    // Infrastructure.Data
    services.AddDbContext<VersaCoderDbContext>(options =>
        options.UseSqlite("Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;"));
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<ISessionRepository, SessionRepository>();
    services.AddScoped<IMessageRepository, MessageRepository>();
    services.AddScoped<IProjectRepository, ProjectRepository>();

    // Infrastructure.AI
    services.AddSingleton<ProviderRouter>();
    services.AddScoped<IAgentRunner, AgentRunner>();
    services.AddSingleton<ToolRegistry>();

    // Application (MediatR)
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(CreateSessionCommand).Assembly));
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    // CrossCutting
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
}
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
        "DefaultModel": "gpt-4o"
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

## 5. Kurallar

| # | Kural |
|---|-------|
| 1 | Host → Protocol ✅, Infrastructure ✅ |
| 2 | Host → Application ❌ |
| 3 | DI registration bu katmanda |
| 4 | Startup.cs tek giriş noktası |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
