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

| # | Modül | Katman | Durum | Tanım |
|---|-------|--------|-------|-------|
| 1 | Infrastructure.Data | L4 | ✅ Implemente | SQLite, EF Core, Repository |
| 2 | Infrastructure.AI | L4 | ✅ Implemente | LLM Provider, Agent Runner |
| 3 | Infrastructure.MCP | L5 | 🔄 Stub | MCP Client/Server |
| 4 | Infrastructure.Auth | L7 | 🔄 Stub | API Key, Credential |
| 5 | Infrastructure.Config | L8 | 🔄 Stub | Uygulama Ayarları |
| 6 | Infrastructure.Plugins | L9 | 🔄 Stub | Plugin Sistemi |
| 7 | Infrastructure.Services | L10 | 🔄 Stub | Yardımcı Servisler |
| 8 | Infrastructure.Caching | L11 | 🔄 Stub | Önbellek Yönetimi |
| 9 | Infrastructure.Messaging | L12 | 🔄 Stub | Event Bus, Messaging |
| 10 | Infrastructure.FileSystem | L13 | 🔄 Stub | Dosya Sistemi |
| 11 | Infrastructure.Network | L14 | 🔄 Stub | HTTP Client, WebSocket |
| 12 | Infrastructure.Security | L15 | 🔄 Stub | Şifreleme, Token |
| 13 | Infrastructure.Observability | L16 | 🔄 Stub | Monitoring, Metrics |
| 14 | Infrastructure.Context | L17 | 🔄 Stub | Context Assembly |
| 15 | Infrastructure.Learning | L18 | 🔄 Stub | Pattern, Düzeltme |
| 16 | Infrastructure.Diagram | L19 | 🔄 Stub | Diyagram OKuma |
| 17 | Infrastructure.ProjectAnalysis | L20 | 🔄 Stub | Proje İndeksleme |
| 18 | Infrastructure.Testing | L21 | 🔄 Stub | Test Altyapısı |
| 19 | Infrastructure.Documentation | L22 | 🔄 Stub | Otomatik Doc |
| 20 | Infrastructure.Refactoring | L23 | 🔄 Stub | Refactoring |
| 21 | Infrastructure.CodeAnalysis | L24 | 🔄 Stub | Kod Analizi |
| 22 | Infrastructure.Git | L25 | 🔄 Stub | Git Entegrasyonu |
| 23 | Infrastructure.Integration | L26 | 🔄 Stub | Üçüncü Parti |
| 24 | Infrastructure.Templating | L27 | 🔄 Stub | Şablon Sistemi |
| 25 | Infrastructure.Deployment | L28 | 🔄 Stub | Dağıtım |
| 26 | Infrastructure.Backup | L29 | 🔄 Stub | Yedekleme |
| 27 | Infrastructure.Versioning | L30 | 🔄 Stub | Versiyon |

---

## 3. Implemente Edilmiş Modüller

### 3.1 Infrastructure.Data

| bileşen | Dosya | Tanım |
|---------|-------|-------|
| `VersaCoderDbContext` | `Infrastructure.Data/Context/VersaCoderDbContext.cs` | EF Core DbContext |
| `Repository<T>` | `Infrastructure.Data/Repositories/Repository.cs` | Genel repository |
| `SessionRepository` | `Infrastructure.Data/Repositories/SessionRepository.cs` | Session CRUD |
| `MessageRepository` | `Infrastructure.Data/Repositories/MessageRepository.cs` | Message CRUD |
| Entity Configurations | `Infrastructure.Data/Configurations/` | EF config |

### 3.2 Infrastructure.AI

| bileşen | Dosya | Tanım |
|---------|-------|-------|
| `AgentRunner` | `Infrastructure.AI/AgentRunner.cs` | Agent çalıştırma |
| `ProviderRouter` | `Infrastructure.AI/ProviderRouter.cs` | Provider yönlendirme |
| `ToolRegistry` | `Infrastructure.AI/ToolRegistry.cs` | Tool kayıt |
| `OpenAIProvider` | `Infrastructure.AI/Providers/OpenAIProvider.cs` | OpenAI entegrasyonu |
| `AnthropicProvider` | `Infrastructure.AI/Providers/AnthropicProvider.cs` | Anthropic entegrasyonu |
| `OllamaProvider` | `Infrastructure.AI/Providers/OllamaProvider.cs` | Ollama entegrasyonu |
| `CustomProvider` | `Infrastructure.AI/Providers/CustomProvider.cs` | Özel provider |

---

## 4. DI Registration

```csharp
// Infrastructure.Data
services.AddDbContext<VersaCoderDbContext>(options =>
    options.UseSqlite("Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;"));
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<ISessionRepository, SessionRepository>();

// Infrastructure.AI
services.AddSingleton<ProviderRouter>();
services.AddScoped<IAgentRunner, AgentRunner>();
services.AddSingleton<ToolRegistry>();
services.AddSingleton<ILLMProvider, OpenAIProvider>();
```

---

## 5. Kurallar

| # | Kural |
|---|-------|
| 1 | Infrastructure → CrossCutting ✅, Application ❌, Domain ❌ |
| 2 | Her modül ayrı proje |
| 3 | Dependency Injection zorunlu |
| 4 | Interface-first tasarım |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
