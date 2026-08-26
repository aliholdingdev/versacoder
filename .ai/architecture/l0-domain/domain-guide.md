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

## 8. Entity Detayları

### 8.1 Session Entity

```csharp
public class Session : BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid ProjectId { get; private set; }
    public SessionState Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? BranchName { get; private set; }
    public Guid? ParentSessionId { get; private set; }
    
    // Navigation
    public Project Project { get; private set; }
    public Session? ParentSession { get; private set; }
    public ICollection<Session> ChildSessions { get; private set; }
    public ICollection<Message> Messages { get; private set; }
    
    private Session() { }
    
    public Session(string name, Guid projectId)
    {
        Id = Guid.NewGuid();
        Name = name;
        ProjectId = projectId;
        Status = SessionState.Active;
        CreatedAt = DateTime.UtcNow;
        ChildSessions = new List<Session>();
        Messages = new List<Message>();
    }
    
    public void Complete()
    {
        Status = SessionState.Completed;
        CompletedAt = DateTime.UtcNow;
    }
    
    public void Pause()
    {
        Status = SessionState.Paused;
    }
    
    public void Resume()
    {
        Status = SessionState.Active;
    }
}
```

### 8.2 Message Entity

```csharp
public class Message : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string Role { get; private set; }
    public string Content { get; private set; }
    public DateTime Timestamp { get; private set; }
    public int TokenCount { get; private set; }
    
    // Navigation
    public Session Session { get; private set; }
    
    private Message() { }
    
    public Message(Guid sessionId, string role, string content, int tokenCount)
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        Role = role;
        Content = content;
        Timestamp = DateTime.UtcNow;
        TokenCount = tokenCount;
    }
}
```

### 8.3 Project Entity

```csharp
public class Project : BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Navigation
    public ICollection<Session> Sessions { get; private set; }
    
    private Project() { }
    
    public Project(string name, string path)
    {
        Id = Guid.NewGuid();
        Name = name;
        Path = path;
        CreatedAt = DateTime.UtcNow;
        Sessions = new List<Session>();
    }
}
```

---

## 9. Value Object Detayları

### 9.1 FilePath Value Object

```csharp
public record FilePath
{
    public string Value { get; }
    
    public FilePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("File path cannot be empty", nameof(value));
        
        Value = value;
    }
    
    public FilePath Combine(string relativePath)
    {
        return new FilePath(System.IO.Path.Combine(Value, relativePath));
    }
    
    public string GetFileName()
    {
        return System.IO.Path.GetFileName(Value);
    }
    
    public string GetExtension()
    {
        return System.IO.Path.GetExtension(Value);
    }
}
```

### 9.2 SessionId Value Object

```csharp
public record SessionId
{
    public Guid Value { get; }
    
    public SessionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Session ID cannot be empty", nameof(value));
        
        Value = value;
    }
    
    public static SessionId New() => new SessionId(Guid.NewGuid());
}
```

### 9.3 Timestamp Value Object

```csharp
public record Timestamp
{
    public DateTime Value { get; }
    
    public Timestamp(DateTime value)
    {
        Value = value;
    }
    
    public static Timestamp Now => new Timestamp(DateTime.UtcNow);
    
    public Timestamp Add(TimeSpan span)
    {
        return new Timestamp(Value.Add(span));
    }
    
    public TimeSpan ElapsedSince(Timestamp other)
    {
        return Value - other.Value;
    }
}
```

---

## 10. Enum Detayları

### 10.1 AgentRole Enum

```csharp
public enum AgentRole
{
    MasterOrchestrator = 0,
    Build = 1,
    Plan = 2,
    Explore = 3,
    General = 4,
    Summary = 5,
    Title = 6,
    Compaction = 7
}
```

### 10.2 SessionState Enum

```csharp
public enum SessionState
{
    Active = 0,
    Paused = 1,
    Completed = 2,
    Archived = 3
}
```

### 10.3 Priority Enum

```csharp
public enum Priority
{
    Critical = 0,
    High = 1,
    Medium = 2,
    Low = 3
}
```

---

## 11. Domain Event Detayları

### 11.1 SessionCreated Event

```csharp
public class SessionCreatedEvent : INotification
{
    public Guid SessionId { get; }
    public string SessionName { get; }
    public Guid ProjectId { get; }
    public DateTime CreatedAt { get; }
    
    public SessionCreatedEvent(Guid sessionId, string sessionName, Guid projectId)
    {
        SessionId = sessionId;
        SessionName = sessionName;
        ProjectId = projectId;
        CreatedAt = DateTime.UtcNow;
    }
}
```

### 11.2 MessageAdded Event

```csharp
public class MessageAddedEvent : INotification
{
    public Guid MessageId { get; }
    public Guid SessionId { get; }
    public string Role { get; }
    public DateTime Timestamp { get; }
    
    public MessageAddedEvent(Guid messageId, Guid sessionId, string role)
    {
        MessageId = messageId;
        SessionId = sessionId;
        Role = role;
        Timestamp = DateTime.UtcNow;
    }
}
```

---

## 12. Domain Servisleri

### 12.1 Session Domain Service

```csharp
public class SessionDomainService
{
    public Session CreateSession(string name, Guid projectId)
    {
        // Domain kurallarını uygula
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Session name cannot be empty");
        
        if (name.Length > 100)
            throw new DomainException("Session name cannot exceed 100 characters");
        
        return new Session(name, projectId);
    }
    
    public void TransitionState(Session session, SessionState newState)
    {
        // State transition kurallarını uygula
        if (!IsValidTransition(session.Status, newState))
            throw new DomainException($"Invalid state transition from {session.Status} to {newState}");
        
        switch (newState)
        {
            case SessionState.Completed:
                session.Complete();
                break;
            case SessionState.Paused:
                session.Pause();
                break;
            case SessionState.Active:
                session.Resume();
                break;
        }
    }
    
    private bool IsValidTransition(SessionState current, SessionState next)
    {
        return (current, next) switch
        {
            (SessionState.Active, SessionState.Paused) => true,
            (SessionState.Active, SessionState.Completed) => true,
            (SessionState.Paused, SessionState.Active) => true,
            (SessionState.Paused, SessionState.Completed) => true,
            _ => false
        };
    }
}
```

---

## 13. Domain İstisnaları

### 13.1 DomainException

```csharp
public class DomainException : Exception
{
    public string ErrorCode { get; }
    
    public DomainException(string message) : base(message)
    {
        ErrorCode = "DOMAIN_ERROR";
    }
    
    public DomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
```

### 13.2 İstisna Kullanım Kalıpları

| Durum | İstisna | Mesaj |
|-------|---------|-------|
| Boş isim | DomainException | "Name cannot be empty" |
| Geçersiz state | DomainException | "Invalid state transition" |
| Boş ID | ArgumentException | "ID cannot be empty" |
| Geçersiz yol | ArgumentException | "Invalid file path" |

---

## 14. Domain Testleri

### 14.1 Entity Testleri

```csharp
public class SessionTests
{
    [Fact]
    public void CreateSession_ValidName_ReturnsSession()
    {
        // Arrange
        var name = "Test Session";
        var projectId = Guid.NewGuid();
        
        // Act
        var session = new Session(name, projectId);
        
        // Assert
        Assert.Equal(name, session.Name);
        Assert.Equal(projectId, session.ProjectId);
        Assert.Equal(SessionState.Active, session.Status);
    }
    
    [Fact]
    public void CompleteSession_FromActive_ReturnsCompleted()
    {
        // Arrange
        var session = new Session("Test", Guid.NewGuid());
        
        // Act
        session.Complete();
        
        // Assert
        Assert.Equal(SessionState.Completed, session.Status);
        Assert.NotNull(session.CompletedAt);
    }
}
```

### 14.2 Value Object Testleri

```csharp
public class FilePathTests
{
    [Fact]
    public void CreateFilePath_ValidPath_ReturnsFilePath()
    {
        // Arrange
        var path = @"C:\test\file.txt";
        
        // Act
        var filePath = new FilePath(path);
        
        // Assert
        Assert.Equal(path, filePath.Value);
    }
    
    [Fact]
    public void CreateFilePath_EmptyPath_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new FilePath(""));
    }
}
```

---

## 15. Domain Kuralları

### 15.1 İş Kuralları

| Kural | Açıklama | Uygulama |
|-------|----------|----------|
| Session ismi boş olamaz | Name required | Entity constructor |
| Session 100 karakterden uzun olamaz | Max length | Domain service |
| State geçişleri geçerli olmalı | Valid transitions | Domain service |
| ID boş olamaz | Required ID | Value Object |

### 15.2 Validation Kuralları

| Kural | Tür | Örnek |
|-------|-----|-------|
| Required | Zorunlu alan | Name, ID |
| MaxLength | Maksimum uzunluk | Name: 100 |
| Pattern | Desen eşleşme | Email format |
| Range | Aralık | Token count > 0 |

---

## 16. Domain Gelecek Planı

### 16.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Yeni entity'ler ekleme | Yüksek |
| Value Object zenginleştirme | Yüksek |
| Domain service ekleme | Orta |

### 16.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Aggregate root tasarlama | Orta |
| Domain event zenginleştirme | Orta |
| Specification pattern | Düşük |

### 16.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Event sourcing | Düşük |
| CQRS integration | Orta |
| DomainDrivenDesign maturity | Düşük |

---

## 17. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Entities | 6 |
| Value Objects | 4 |
| Enums | 6 |
| Events | 4 |
| Domain Services | 1 |
| Exceptions | 1 |
| Test Examples | 2 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
