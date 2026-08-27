---
title: "Versa Coder — L2 Application Layer Guide"
type: architecture
category: layer
layer: L2
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L2 Application Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/l1-abstractions/abstractions-guide]] · [[brain.md]]

---

## 1. Amaç

Application katmanı, **use case'leri, command/query'leri ve DTO'ları** içerir. CQRS pattern'ı MediatR ile uygulanır.

---

## 2. Commands (CQRS Write)

| Command | Dosya | Tanım | Validation |
|---------|-------|-------|------------|
| `CreateSessionCommand` | `Commands/CreateSessionCommand.cs` | Yeni oturum oluştur | SessionValidator |
| `SendPromptCommand` | `Commands/SendPromptCommand.cs` | Prompt gönder | PromptValidator |
| `BranchSessionCommand` | `Commands/BranchSessionCommand.cs` | Oturum dalı oluştur | BranchValidator |
| `CompleteSessionCommand` | `Commands/CompleteSessionCommand.cs` | Oturumu tamamla | CompleteValidator |
| `CreateProjectCommand` | `Commands/CreateProjectCommand.cs` | Proje oluştur | ProjectValidator |
| `RecordLearningCommand` | `Commands/RecordLearningCommand.cs` | Öğrenme kaydet | LearningValidator |

---

## 3. Queries (CQRS Read)

| Query | Dosya | Tanım | Response |
|-------|-------|-------|----------|
| `GetSessionQuery` | `Queries/GetSessionQuery.cs` | Tek oturum al | SessionDto |
| `GetAllSessionsQuery` | `Queries/GetAllSessionsQuery.cs` | Tüm oturumları al | List<SessionDto> |
| `GetSessionMessagesQuery` | `Queries/GetSessionMessagesQuery.cs` | Oturum mesajlarını al | List<MessageDto> |
| `GetProjectQuery` | `Queries/GetProjectQuery.cs` | Tek proje al | ProjectDto |
| `GetAllProjectsQuery` | `Queries/GetAllProjectsQuery.cs` | Tüm projeleri al | List<ProjectDto> |
| `GetContextQuery` | `Queries/GetContextQuery.cs` | Context al | ContextDto |

---

## 4. Handlers

| Handler | Command/Query | Tanım | Satır |
|---------|---------------|-------|-------|
| `CreateSessionHandler` | `CreateSessionCommand` | Session oluştur | ~80 |
| `SendPromptHandler` | `SendPromptCommand` | Prompt işle (IAgentRunner çağırır) | ~100 |
| `BranchSessionHandler` | `BranchSessionCommand` | Dal oluştur | ~60 |
| `CompleteSessionHandler` | `CompleteSessionCommand` | Tamamla | ~40 |
| `CreateProjectHandler` | `CreateProjectCommand` | Proje oluştur | ~50 |
| `RecordLearningHandler` | `RecordLearningCommand` | Öğrenme kaydet | ~40 |
| `GetSessionHandler` | `GetSessionQuery` | Session oku | ~30 |
| `GetSessionMessagesHandler` | `GetSessionMessagesQuery` | Mesajları oku | ~30 |

---

## 5. Services

### 5.1 Core Servisler

| Servis | Dosya | Tanım | Durum | Satır |
|--------|-------|-------|-------|-------|
| `SessionManagerService` | `Services/SessionManagerService.cs` | Session yönetimi | ✅ Tam | 121 |
| `ContextManagerService` | `Services/ContextManagerService.cs` | Context yönetimi | 🔄 Kısmi | 63 |
| `AgentSelectorService` | `Services/AgentSelectorService.cs` | Agent seçimi | ✅ Tam | 36 |
| `LogService` | `Services/LogService.cs` | Log yönetimi | ✅ Tam | 199 |
| `LearningService` | `Services/LearningService.cs` | Öğrenme | ✅ Tam | 71 |
| `GitService` | `Services/GitService.cs` | Git işlemleri | ❌ Stub | 56 |
| `ProjectAnalyzerService` | `Services/ProjectAnalyzerService.cs` | Proje analizi | 🔄 Basit | 67 |
| `TemplateService` | `Services/TemplateService.cs` | Şablon sistemi | ✅ Tam | — |
| `ReportService` | — | Raporlama | ❌ Eksik | — |
| `DiagramTeacherService` | — | Diyagram öğretme | ❌ Eksik | — |

### 5.2 Task Management Servisleri

| Servis | Dosya | Tanım | Durum | Satır |
|--------|-------|-------|-------|-------|
| `TaskService` | `Services/TaskService.cs` | Görev yönetimi | ✅ Tam (tag stubs) | 563 |

---

## 6. DTO'lar

| DTO | Dosya | Tanım |
|-----|-------|-------|
| `SessionDto` | `DTOs/SessionDto.cs` | Session verisi |
| `MessageDto` | `DTOs/MessageDto.cs` | Mesaj verisi |
| `ProjectDto` | `DTOs/ProjectDto.cs` | Proje verisi |
| `ContextDto` | `DTOs/ContextDto.cs` | Context verisi |
| `AgentDto` | `DTOs/AgentDto.cs` | Agent verisi |
| `LogDto` | `DTOs/LogDto.cs` | Log verisi |
| `ReportDto` | `DTOs/ReportDto.cs` | Rapor verisi |
| `TaskDto` | `DTOs/TaskDto.cs` | Görev verisi |
| `TaskListDto` | `DTOs/TaskListDto.cs` | Görev listesi verisi |

---

## 7. Ortak Yapılar

| Yapı | Dosya | Tanım |
|------|-------|-------|
| `Result<T>` | `Common/Result.cs` | Monad pattern (Success/Failure) |
| `PaginatedList<T>` | `Common/PaginatedList.cs` | Sayfalı liste |

---

## 8. MediatR Pipeline

```
Request → LoggingBehavior → PerformanceBehavior → ValidationBehavior → Handler → Response
```

---

## 9. OpenCode Eşleştirme

| VersaCoder | OpenCode | Durum |
|------------|----------|-------|
| CreateSessionCommand | `session.create()` | ✅ Eşleşti |
| SendPromptCommand | `session.prompt()` | ✅ Eşleşti |
| SessionManagerService | `session` interface | ✅ Eşleşti |
| ContextManagerService | `session.context` | 🔄 Kısmi |
| AgentSelectorService | `agent.select()` | ✅ Eşleşti |

---

## 10. Kurallar

| # | Kural |
|---|-------|
| 1 | Handler'lar MediatR `IRequestHandler` implemente eder |
| 2 | Validation FluentValidation ile |
| 3 | Business logic Domain'e ait — Application sadece orkestra eder |
| 4 | DTO'lar simple data transfer — behavior yok |
| 5 | Her handler için unit test yazılmalı |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
