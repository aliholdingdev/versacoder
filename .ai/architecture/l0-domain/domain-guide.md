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

| Entity | Dosya | Tanım |
|--------|-------|-------|
| `Session` | `VersaCoder.Domain/Entities/Session.cs` | Oturum bilgisi |
| `Message` | `VersaCoder.Domain/Entities/Message.cs` | Mesaj içeriği |
| `Project` | `VersaCoder.Domain/Entities/Project.cs` | Proje bilgisi |
| `FileEntry` | `VersaCoder.Domain/Entities/FileEntry.cs` | Dosya kaydı |
| `LearningEntry` | `VersaCoder.Domain/Entities/LearningEntry.cs` | Öğrenme kaydı |
| `Setting` | `VersaCoder.Domain/Entities/Setting.cs` | Uygulama ayarı |

---

## 3. Değer Objeleri (Value Objects)

| Value Object | Dosya | Tanım |
|--------------|-------|-------|
| `FilePath` | `VersaCoder.Domain/ValueObjects/FilePath.cs` | Dosya yolu (record, validation) |
| `ModelName` | `VersaCoder.Domain/ValueObjects/ModelName.cs` | Model adı |
| `SessionId` | `VersaCoder.Domain/ValueObjects/SessionId.cs` | Session UUID |
| `Timestamp` | `VersaCoder.Domain/ValueObjects/Timestamp.cs` | Zaman damgası |

---

## 4. Enum'lar

| Enum | Dosya | Değerler |
|------|-------|----------|
| `AgentRole` | `VersaCoder.Domain/Enums/AgentRole.cs` | MO, Build, Plan, Explore, General, Summary, Title, Compaction |
| `ContextType` | `VersaCoder.Domain/Enums/ContextType.cs` | Project, File, Session, Skill, Diagram |
| `FileType` | `VersaCoder.Domain/Enums/FileType.cs` | Source, Config, Documentation, Test |
| `LearningCategory` | `VersaCoder.Domain/Enums/LearningCategory.cs` | Pattern, Correction, Knowledge, Rule |
| `Priority` | `VersaCoder.Domain/Enums/Priority.cs` | Critical, High, Medium, Low |
| `SessionState` | `VersaCoder.Domain/Enums/SessionState.cs` | Active, Paused, Completed |

---

## 5. Sabitler (Constants)

| Sabit | Dosya | İçerik |
|-------|-------|--------|
| `AgentNames` | `VersaCoder.Domain/Constants/AgentNames.cs` | 8 agent adı |
| `ToolNames` | `VersaCoder.Domain/Constants/ToolNames.cs` | 47 tool adı |
| `SystemConstants` | `VersaCoder.Domain/Constants/SystemConstants.cs` | Sistem sabitleri |

---

## 6. Domain Event'ler

| Event | Tetikleyici | Aksiyon |
|-------|-------------|---------|
| `SessionCreated` | Yeni oturum | Log + UI güncelle |
| `SessionCompleted` | Oturum sonu | Summary + Arşivle |
| `MessageAdded` | Yeni mesaj | Context güncelle |
| `LearningRecorded` | Öğrenme | Pattern kaydet |

---

## 7. Kurallar

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Hiçbir dış bağımlılık yok | Domain, sadece .NET BCL'ye bağımlı |
| 2 | Business logic burada | Validasyon, state transition |
| 3 | Layer Violation yasağı | L0 → L2/L3 referans YOK |
| 4 | Value Object validation | Record'larda constructor validation |
| 5 | Entity behavior | Sadece data değil, davranış da içerebilir |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
