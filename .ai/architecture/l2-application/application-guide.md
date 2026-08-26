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

| Command | Dosya | Tanım |
|---------|-------|-------|
| `CreateSessionCommand` | `VersaCoder.Application/Commands/CreateSessionCommand.cs` | Yeni oturum oluştur |
| `SendPromptCommand` | `VersaCoder.Application/Commands/SendPromptCommand.cs` | Prompt gönder |
| `BranchSessionCommand` | `VersaCoder.Application/Commands/BranchSessionCommand.cs` | Oturum dalı oluştur |
| `CompleteSessionCommand` | `VersaCoder.Application/Commands/CompleteSessionCommand.cs` | Oturumu tamamla |
| `CreateProjectCommand` | `VersaCoder.Application/Commands/CreateProjectCommand.cs` | Proje oluştur |
| `RecordLearningCommand` | `VersaCoder.Application/Commands/RecordLearningCommand.cs` | Öğrenme kaydet |

---

## 3. Queries (CQRS Read)

| Query | Dosya | Tanım |
|-------|-------|-------|
| `GetSessionQuery` | `VersaCoder.Application/Queries/GetSessionQuery.cs` | Tek oturum al |
| `GetSessionMessagesQuery` | `VersaCoder.Application/Queries/GetSessionMessagesQuery.cs` | Oturum mesajlarını al |
| `GetProjectQuery` | `VersaCoder.Application/Queries/GetProjectQuery.cs` | Tek proje al |
| `GetContextQuery` | `VersaCoder.Application/Queries/GetContextQuery.cs` | Context al |
| `GetAllSessionsQuery` | `VersaCoder.Application/Queries/GetAllSessionsQuery.cs` | Tüm oturumları al |
| `GetAllProjectsQuery` | `VersaCoder.Application/Queries/GetAllProjectsQuery.cs` | Tüm projeleri al |

---

## 4. Handlers

| Handler | Command/Query | Tanım |
|---------|---------------|-------|
| `CreateSessionHandler` | `CreateSessionCommand` | Session oluştur |
| `SendPromptHandler` | `SendPromptCommand` | Prompt işle |
| `BranchSessionHandler` | `BranchSessionCommand` | Dal oluştur |
| `CompleteSessionHandler` | `CompleteSessionCommand` | Tamamla |
| `CreateProjectHandler` | `CreateProjectCommand` | Proje oluştur |
| `RecordLearningHandler` | `RecordLearningCommand` | Öğrenme kaydet |
| `GetSessionHandler` | `GetSessionQuery` | Session oku |
| `GetSessionMessagesHandler` | `GetSessionMessagesQuery` | Mesajları oku |

---

## 5. Services

| Servis | Dosya | Tanım |
|--------|-------|-------|
| `AgentSelectorService` | `VersaCoder.Application/Services/AgentSelectorService.cs` | Agent seçimi |
| `ContextManagerService` | `VersaCoder.Application/Services/ContextManagerService.cs` | Context yönetimi |
| `DiagramTeacherService` | `VersaCoder.Application/Services/DiagramTeacherService.cs` | Diyagram öğretme |
| `GitService` | `VersaCoder.Application/Services/GitService.cs` | Git işlemleri |
| `LearningService` | `VersaCoder.Application/Services/LearningService.cs` | Öğrenme |
| `ProjectAnalyzerService` | `VersaCoder.Application/Services/ProjectAnalyzerService.cs` | Proje analizi |
| `SessionManagerService` | `VersaCoder.Application/Services/SessionManagerService.cs` | Session yönetimi |
| `TemplateService` | `VersaCoder.Application/Services/TemplateService.cs` | Şablon sistemi |

---

## 6. DTO'lar

| DTO | Dosya | Tanım |
|-----|-------|-------|
| `SessionDto` | `VersaCoder.Application/DTOs/SessionDto.cs` | Session verisi |
| `MessageDto` | `VersaCoder.Application/DTOs/MessageDto.cs` | Mesaj verisi |
| `ProjectDto` | `VersaCoder.Application/DTOs/ProjectDto.cs` | Proje verisi |
| `ContextDto` | `VersaCoder.Application/DTOs/ContextDto.cs` | Context verisi |
| `AgentDto` | `VersaCoder.Application/DTOs/AgentDto.cs` | Agent verisi |

---

## 7. Ortak Yapılar

| Yapı | Dosya | Tanım |
|------|-------|-------|
| `Result<T>` | `VersaCoder.Application/Common/Result.cs` | Monad pattern (Success/Failure) |
| `PaginatedList<T>` | `VersaCoder.Application/Common/PaginatedList.cs` | Sayfalı liste |

---

## 8. Kurallar

| # | Kural |
|---|-------|
| 1 | Handler'lar MediatR `IRequestHandler` implemente eder |
| 2 | Validation FluentValidation ile |
| 3 | Business logic Domain'e ait — Application sadece orkestra eder |
| 4 | DTO'lar simple data transfer — behavior yok |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
