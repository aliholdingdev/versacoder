---
title: "Versa Coder — Database Schema"
type: architecture
category: data
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Database Schema

**Zorunlu Bağlantılar:** [[architecture/l4-infrastructure/infrastructure-guide]] · [[brain.md]]

---

## 1. Amaç

SQLite veritabanı şeması, EF Core entity konfigürasyonları ve veri modeli.

---

## 2. Veritabanı Bilgileri

| Özellik | Değer |
|---------|-------|
| Motor | SQLite 3.x |
| Mod | WAL (Write-Ahead Logging) |
| ORM | EF Core 8.0 |
| Connection | `Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;` |
| Naming | PascalCase |

---

## 3. Tablolar

### 3.1 Session Tablosu

```sql
CREATE TABLE Sessions (
    Id              TEXT PRIMARY KEY,           -- UUID
    Title           TEXT NOT NULL,
    State           INTEGER NOT NULL DEFAULT 0, -- SessionState enum
    AgentRole       INTEGER NOT NULL,           -- AgentRole enum
    ModelName       TEXT,
    ProjectId       TEXT,
    ParentSessionId TEXT,                       -- Branch/Fork
    TokenCount      INTEGER DEFAULT 0,
    Cost            REAL DEFAULT 0.0,
    CreatedAt       DATETIME NOT NULL,
    UpdatedAt       DATETIME NOT NULL,
    CompletedAt     DATETIME,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
    FOREIGN KEY (ParentSessionId) REFERENCES Sessions(Id)
);
```

### 3.2 Message Tablosu

```sql
CREATE TABLE Messages (
    Id          TEXT PRIMARY KEY,               -- UUID
    SessionId   TEXT NOT NULL,
    Role        TEXT NOT NULL,                  -- user/assistant/system/tool
    Content     TEXT NOT NULL,
    TokenCount  INTEGER DEFAULT 0,
    CreatedAt   DATETIME NOT NULL,
    FOREIGN KEY (SessionId) REFERENCES Sessions(Id) ON DELETE CASCADE
);
```

### 3.3 Project Tablosu

```sql
CREATE TABLE Projects (
    Id          TEXT PRIMARY KEY,               -- UUID
    Name        TEXT NOT NULL,
    RootPath    TEXT NOT NULL,
    Description TEXT,
    CreatedAt   DATETIME NOT NULL,
    UpdatedAt   DATETIME NOT NULL
);
```

### 3.4 FileEntry Tablosu

```sql
CREATE TABLE FileEntries (
    Id          TEXT PRIMARY KEY,               -- UUID
    ProjectId   TEXT NOT NULL,
    FilePath    TEXT NOT NULL,
    FileType    INTEGER NOT NULL,               -- FileType enum
    ContentHash TEXT,
    Size        INTEGER,
    CreatedAt   DATETIME NOT NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);
```

### 3.5 LearningEntry Tablosu

```sql
CREATE TABLE LearningEntries (
    Id          TEXT PRIMARY KEY,               -- UUID
    Category    INTEGER NOT NULL,               -- LearningCategory enum
    Pattern     TEXT NOT NULL,
    Description TEXT,
    Confidence  REAL DEFAULT 0.0,
    UsageCount  INTEGER DEFAULT 0,
    CreatedAt   DATETIME NOT NULL,
    UpdatedAt   DATETIME NOT NULL
);
```

### 3.6 Setting Tablosu

```sql
CREATE TABLE Settings (
    Key         TEXT PRIMARY KEY,
    Value       TEXT NOT NULL,
    Description TEXT,
    UpdatedAt   DATETIME NOT NULL
);
```

---

## 4. EF Core Konfigürasyonları

| Configuration | Dosya | Tanım |
|---------------|-------|-------|
| `SessionConfiguration` | `Infrastructure.Data/Configurations/SessionConfiguration.cs` | Session indexes |
| `MessageConfiguration` | `Infrastructure.Data/Configurations/MessageConfiguration.cs` | Message indexes |
| `ProjectConfiguration` | `Infrastructure.Data/Configurations/ProjectConfiguration.cs` | Project indexes |
| `FileEntryConfiguration` | `Infrastructure.Data/Configurations/FileEntryConfiguration.cs` | FileEntry indexes |
| `LearningEntryConfiguration` | `Infrastructure.Data/Configurations/LearningEntryConfiguration.cs` | Learning indexes |
| `SettingConfiguration` | `Infrastructure.Data/Configurations/SettingConfiguration.cs` | Setting config |

---

## 5. OpenCode Eşleştirme

| VersaCoder | OpenCode |
|------------|----------|
| Sessions | `session` table |
| Messages | `message` + `part` tables |
| Projects | `project` table |
| FileEntries | (OpenCode'da yok — dosya sistemi taraması) |
| LearningEntries | (OpenCode'da yok — learning sistemi) |
| Settings | `credential` + config |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
