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

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
