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

| Arayüz | Dosya | Tanım |
|--------|-------|-------|
| `IAgentRunner` | `VersaCoder.Abstractions/Services/IAgentRunner.cs` | Agent çalıştırma |
| `IContextManager` | `VersaCoder.Abstractions/Services/IContextManager.cs` | Context yönetimi |
| `IDiagramTeacher` | `VersaCoder.Abstractions/Services/IDiagramTeacher.cs` | Diyagram öğretme |
| `IGitService` | `VersaCoder.Abstractions/Services/IGitService.cs` | Git işlemleri |
| `ILearningService` | `VersaCoder.Abstractions/Services/ILearningService.cs` | Öğrenme sistemi |
| `IProjectAnalyzer` | `VersaCoder.Abstractions/Services/IProjectAnalyzer.cs` | Proje analizi |
| `ISessionManager` | `VersaCoder.Abstractions/Services/ISessionManager.cs` | Session yönetimi |
| `ITemplateService` | `VersaCoder.Abstractions/Services/ITemplateService.cs` | Şablon sistemi |
| `IToolExecutor` | `VersaCoder.Abstractions/Services/IToolExecutor.cs` | Tool çalıştırma |

---

## 3. Repository Arayüzleri

| Arayüz | Dosya | Tanım |
|--------|-------|-------|
| `IRepository<T>` | `VersaCoder.Abstractions/Repositories/IRepository.cs` | Genel repository |
| `ISessionRepository` | `VersaCoder.Abstractions/Repositories/ISessionRepository.cs` | Session CRUD |
| `IMessageRepository` | `VersaCoder.Abstractions/Repositories/IMessageRepository.cs` | Message CRUD |
| `IProjectRepository` | `VersaCoder.Abstractions/Repositories/IProjectRepository.cs` | Project CRUD |
| `IFileRepository` | `VersaCoder.Abstractions/Repositories/IFileRepository.cs` | File CRUD |
| `ILearningRepository` | `VersaCoder.Abstractions/Repositories/ILearningRepository.cs` | Learning CRUD |
| `ISettingRepository` | `VersaCoder.Abstractions/Repositories/ISettingRepository.cs` | Setting CRUD |

---

## 4. Provider Arayüzleri

| Arayüz | Dosya | Tanım |
|--------|-------|-------|
| `ILLMProvider` | `VersaCoder.Abstractions/Providers/ILLMProvider.cs` | LLM sağlayıcı |
| `IEmbeddingProvider` | `VersaCoder.Abstractions/Providers/IEmbeddingProvider.cs` | Embedding sağlayıcı |

---

## 5. Kurallar

| # | Kural |
|---|-------|
| 1 | Hiçbir implementasyon yok — sadece arayüz |
| 2 | Domain'e bağımlı (L1 → L0 ✅) |
| 3 | Hiçbir başka katmana bağımlı değil |
| 4 | Interface Segregation — her arayüz tek sorumluluk |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
