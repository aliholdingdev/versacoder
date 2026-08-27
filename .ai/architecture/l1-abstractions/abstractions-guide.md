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

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
