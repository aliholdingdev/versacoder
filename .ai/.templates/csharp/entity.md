---
title: "Entity Template"
type: template
category: csharp
version: 1.0.0
---

# Entity Template

## Kullanım

Yeni bir domain entity oluştururken bu template'i kullanın.

## Template

```csharp
using System;
using System.Collections.Generic;

namespace {Namespace}
{
    /// <summary>
    /// {Description}
    /// </summary>
    public class {ClassName}
    {
        #region Properties

{Properties}

        #endregion

        #region Constructor

        /// <summary>
        /// EF Core için varsayılan constructor
        /// </summary>
        protected {ClassName}() { }

        /// <summary>
        /// Yeni {ClassName} oluşturur
        /// </summary>
        public {ClassName}({ConstructorParameters})
        {
{ConstructorBody}
        }

        #endregion

        #region Methods

{Methods}

        #endregion

        #region Equality

        public override bool Equals(object? obj)
        {
            if (obj is not {ClassName} other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==({ClassName}? left, {ClassName}? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=({ClassName}? left, {ClassName}? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
```

## Örnek Kullanım

```csharp
// Template ile entity oluşturma
var template = templateEngine.GetTemplate("entity");
var content = templateEngine.Render(template, new
{
    Namespace = "VersaCoder.Domain.Entities",
    ClassName = "Session",
    Description = "Oturum varlığı",
    Properties = @"        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SessionState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }",
    ConstructorParameters = "string name",
    ConstructorBody = @"        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        State = SessionState.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;",
    Methods = @"        public void UpdateName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            State = SessionState.COMPLETED;
            UpdatedAt = DateTime.UtcNow;
        }"
});
```

---

## 3. Entity Örnekleri

### 3.1 Basit Entity

```csharp
using System;
using System.Collections.Generic;

namespace VersaCoder.Domain.Entities
{
    /// <summary>
    /// Oturum varlığı
    /// </summary>
    public class Session
    {
        #region Properties

        /// <summary>
        /// Oturum benzersiz tanımlayıcısı
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Oturum adı
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Oturum durumu
        /// </summary>
        public SessionState State { get; private set; }

        /// <summary>
        /// Proje ID
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Güncellenme tarihi
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Mesaj listesi
        /// </summary>
        private readonly List<Message> _messages = new();

        /// <summary>
        /// Mesajların salt okunur listesi
        /// </summary>
        public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

        #endregion

        #region Constructor

        /// <summary>
        /// EF Core için varsayılan constructor
        /// </summary>
        protected Session() { }

        /// <summary>
        /// Yeni Session oluşturur
        /// </summary>
        /// <param name="name">Oturum adı</param>
        /// <param name="projectId">Proje ID</param>
        public Session(string name, Guid projectId)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ProjectId = projectId;
            State = SessionState.ACTIVE;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Oturum adını günceller
        /// </summary>
        /// <param name="newName">Yeni ad</param>
        public void UpdateName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Oturumu tamamlar
        /// </summary>
        public void Complete()
        {
            State = SessionState.COMPLETED;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Oturumu askıya alır
        /// </summary>
        public void Suspend()
        {
            State = SessionState.SUSPENDED;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Mesaj ekler
        /// </summary>
        /// <param name="message">Eklenecek mesaj</param>
        public void AddMessage(Message message)
        {
            ArgumentNullException.ThrowIfNull(message);
            _messages.Add(message);
            UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Equality

        public override bool Equals(object? obj)
        {
            if (obj is not Session other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Session? left, Session? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Session? left, Session? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
```

### 3.2 Value Object

```csharp
using System;

namespace VersaCoder.Domain.ValueObjects
{
    /// <summary>
    /// E-posta adresi value object
    /// </summary>
    public record Email
    {
        /// <summary>
        /// E-posta adresi
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Email oluşturur
        /// </summary>
        /// <param name="value">E-posta adresi</param>
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(value));
            
            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email format", nameof(value));
            
            Value = value.ToLowerInvariant();
        }

        /// <summary>
        /// E-posta formatını doğrular
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => Value;
    }
}
```

### 3.3 Enumeration

```csharp
using Ardalis.SmartEnum;

namespace VersaCoder.Domain.Enums
{
    /// <summary>
    /// Oturum durumu enumerasyonu
    /// </summary>
    public class SessionState : SmartEnum<SessionState>
    {
        public static readonly SessionState ACTIVE = new(nameof(ACTIVE), 0);
        public static readonly SessionState SUSPENDED = new(nameof(SUSPENDED), 1);
        public static readonly SessionState COMPLETED = new(nameof(COMPLETED), 2);
        public static readonly SessionState ARCHIVED = new(nameof(ARCHIVED), 3);

        private SessionState(string name, int value) : base(name, value) { }
    }
}
```

---

## 4. Entity Factory

```csharp
namespace VersaCoder.Domain.Factories
{
    /// <summary>
    /// Session entity factory
    /// </summary>
    public static class SessionFactory
    {
        /// <summary>
        /// Yeni Session oluşturur
        /// </summary>
        /// <param name="name">Oturum adı</param>
        /// <param name="projectId">Proje ID</param>
        /// <returns>Oluşturulan session</returns>
        public static Session Create(string name, Guid projectId)
        {
            return new Session(name, projectId);
        }

        /// <summary>
        /// Validasyon ile yeni Session oluşturur
        /// </summary>
        /// <param name="name">Oturum adı</param>
        /// <param name="projectId">Proje ID</param>
        /// <returns>Oluşturulan session</returns>
        /// <exception cref="ArgumentException">Geçersiz parametreler</exception>
        public static Session CreateWithValidation(string name, Guid projectId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Session name cannot be empty", nameof(name));
            
            if (name.Length > 100)
                throw new ArgumentException("Session name cannot exceed 100 characters", nameof(name));
            
            if (projectId == Guid.Empty)
                throw new ArgumentException("Project ID cannot be empty", nameof(projectId));
            
            return new Session(name, projectId);
        }
    }
}
```

---

## 5. Entity Extension Methods

```csharp
namespace VersaCoder.Domain.Extensions
{
    /// <summary>
    /// Session extension methods
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Session'ı DTO'ya dönüştürür
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>Session DTO</returns>
        public static SessionDto ToDto(this Session session)
        {
            return new SessionDto
            {
                Id = session.Id,
                Name = session.Name,
                State = session.State.Name,
                ProjectId = session.ProjectId,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt,
                MessageCount = session.Messages.Count
            };
        }

        /// <summary>
        /// Session aktif mi kontrol eder
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>Aktif mi</returns>
        public static bool IsActive(this Session session)
        {
            return session.State == SessionState.ACTIVE;
        }

        /// <summary>
        /// Session tamamlanmış mı kontrol eder
        /// </summary>
        /// <param name="session">Session</param>
        /// <returns>Tamamlanmış mı</returns>
        public static bool IsCompleted(this Session session)
        {
            return session.State == SessionState.COMPLETED;
        }
    }
}
```

---

## 6. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Entity Examples | 3 (Simple, Value Object, Enumeration) |
| Factory Methods | 2 |
| Extension Methods | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
