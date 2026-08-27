---
title: "Versa Coder — L0 Domain Layer Guide"
type: architecture
category: layer
layer: L0
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L0 Domain Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/00-overview/architecture-master]] · [[brain.md]]

---

## 1. Amaç

Domain katmanı, Versa Coder'ın **iş mantığının kalbidir**. Hiçbir dış bağımlılığı yoktur — saf C# sınıfları, enum'lar ve arayüzler içerir.

---

## 2. Mevcut Varlıklar (Entities)

### 2.1 Core Entities

| Entity | Dosya | Tanım | Satır | Durum |
|--------|-------|-------|-------|-------|
| `Session` | `Entities/Session.cs` | AI oturumu, branching, parent-child | 55 | ✅ |
| `Message` | `Entities/Message.cs` | Mesaj içeriği, role-based | 25 | ✅ |
| `Project` | `Entities/Project.cs` | Proje bilgisi, analysis results | 31 | ✅ |
| `FileEntry` | `Entities/FileEntry.cs` | Dosya kaydı, hash tracking | 35 | ✅ |
| `LearningEntry` | `Entities/LearningEntry.cs` | Öğrenme kaydı, 4 kategori | 37 | ✅ |
| `Setting` | `Entities/Setting.cs` | Uygulama ayarı, key-value | 28 | ✅ |

### 2.2 Task Management Entities

| Entity | Dosya | Tanım | Satır | Durum |
|--------|-------|-------|-------|-------|
| `TaskItem` | `Entities/TaskItem.cs` | Görev kalemi, state machine (7 durum) | 214 | ✅ |
| `TaskList` | `Entities/TaskList.cs` | Görev listesi, archiving | 111 | ✅ |
| `TaskTag` | `Entities/TaskTag.cs` | Etiket, name/color validation | 46 | ✅ |
| `TaskDependency` | `Entities/TaskDependency.cs` | Bağımlılık, 4 tip (FS/SS/FF/SF) | 65 | ✅ |
| `TaskReminder` | `Entities/TaskReminder.cs` | Hatırlatma, due check | 42 | ✅ |

### 2.3 Audit Entity

| Entity | Dosya | Tanım | Satır | Durum |
|--------|-------|-------|-------|-------|
| `AuditLog` | `Entities/AuditLog.cs` | Denetim kaydı, structured logging | 192 | ✅ |

---

## 3. Değer Objeleri (Value Objects)

| Value Object | Dosya | Tanım | Properties |
|--------------|-------|-------|------------|
| `FilePath` | `ValueObjects/FilePath.cs` | Dosya yolu (record, validation) | Path, Extension, IsDirectory |
| `ModelName` | `ValueObjects/ModelName.cs` | Model adı | Provider, Model, Version |
| `SessionId` | `ValueObjects/SessionId.cs` | Session UUID | Value (Guid) |
| `Timestamp` | `ValueObjects/Timestamp.cs` | Zaman damgası | Value (DateTime) |

---

## 4. Enum'lar

| Enum | Dosya | Değerler |
|------|-------|----------|
| `AgentRole` | `Enums/AgentRole.cs` | MO, Build, Plan, Explore, General, Summary, Title, Compaction |
| `SessionState` | `Enums/SessionState.cs` | Active, Paused, Completed |
| `TaskItemStatus` | `Enums/TaskItemStatus.cs` | Todo, InProgress, Done, Cancelled, Blocked, Deferred, Review |
| `Priority` | `Enums/Priority.cs` | Critical, High, Medium, Low |
| `ContextType` | `Enums/ContextType.cs` | Project, File, Session, Skill, Diagram |
| `FileType` | `Enums/FileType.cs` | Source, Config, Documentation, Test |
| `LearningCategory` | `Enums/LearningCategory.cs` | Pattern, Correction, Knowledge, Rule |
| `DurationType` | `Enums/DurationType.cs` | Minutes, Hours, Days |
| `DependencyType` | `Enums/DependencyType.cs` | FinishToStart, StartToStart, FinishToFinish, StartToFinish |
| `AuditLogLevel` | `Enums/AuditLogLevel.cs` | Info, Warning, Error, Critical |
| `ReportType` | `Enums/ReportType.cs` | Session, Project, Task, Learning, Audit, Performance, Cost, Usage, Summary, Custom |
| `ReportFormat` | `Enums/ReportFormat.cs` | Markdown, Html, Json, Csv, Excel, Pdf |

---

## 5. Domain Event'ler

| Event | Tetikleyici | Aksiyon |
|-------|-------------|---------|
| `SessionCreatedEvent` | Yeni oturum | Indexleme başlat |
| `PromptSentEvent` | Prompt gönderimi | AI çağrısı |
| `ResponseReceivedEvent` | AI yanıtı | Message kaydı |
| `ToolExecutedEvent` | Tool kullanımı | Sonuç işlenir |
| `LearningRecordedEvent` | Öğrenme | Knowledge base güncelleme |
| `AgentHandoverEvent` | Agent değişimi | Context transfer |

---

## 6. Sabitler (Constants)

| Sabit | Dosya | İçerik |
|-------|-------|--------|
| `AgentNames` | `Constants/AgentNames.cs` | 8 agent adı (MO, Build, Plan, Explore, General, Summary, Title, Compaction) |
| `ToolNames` | `Constants/ToolNames.cs` | 48 tool adı (dosya, terminal, git, ai, mcp, proje, session, context) |
| `SystemConstants` | `Constants/SystemConstants.cs` | Sistem sabitleri |

---

## 7. Domain Interfaces

| Interface | Dosya | Tanım |
|-----------|-------|-------|
| `IRepository<T>` | `Interfaces/IRepository.cs` | Genel repository arayüzü |
| `IUnitOfWork` | `Interfaces/IUnitOfWork.cs` | İşlem yönetimi |
| `IDomainEventBus` | `Interfaces/IDomainEventBus.cs` | Domain event yayını |

---

## 8. Kurallar

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Hiçbir dış bağımlılık yok | Domain, sadece .NET BCL'ye bağımlı |
| 2 | Business logic burada | Validasyon, state transition |
| 3 | Layer Violation yasağı | L0 → L2/L3 referans YOK |
| 4 | Value Object validation | Record'larda constructor validation |
| 5 | Entity behavior | Sadece data değil, davranış da içerebilir |
| 6 | State Machine | TaskItem 7 durumlu state machine |
| 7 | Domain Events | Tüm önemli olaylar event olarak yayınlanır |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
