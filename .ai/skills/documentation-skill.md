---
title: "Versa Coder — Documentation Skill"
type: skill
category: documentation
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Documentation Skill

---

## 1. Amaç

Dokümantasyon oluşturma ve güncelleme görevleri için **özel skill**.

---

## 2. Doküman Türleri

| Tür | Format | Kullanım |
|-----|--------|----------|
| API Doc | XML Comment | Code documentation |
| README | Markdown | Proje tanımı |
| Guide | Markdown | Kullanım rehberi |
| ADR | Markdown | Mimari kararlar |
| Changelog | Markdown | Değişiklik kaydı |

---

## 3. Dokümantasyon Standartları

| # | Kural |
|---|-------|
| 1 | Türkçe yaz |
| 2 | Markdown formatı |
| 3 | Başlık hiyerarşisi koru |
| 4 | Örnekler ekle |
| 5 | Güncel tut |

---

## 4. XML Documentation

### 4.1 Class Documentation

```csharp
/// <summary>
/// Represents a session in the VersaCoder application.
/// A session contains messages and manages AI interactions.
/// </summary>
/// <remarks>
/// Sessions are created by users to interact with AI models.
/// Each session maintains its own context and message history.
/// </remarks>
/// <example>
/// // Creating a new session
/// var session = new Session("My Session", projectId);
/// </example>
public class Session
{
    /// <summary>
    /// Gets the unique identifier for this session.
    /// </summary>
    /// <value>A GUID representing the session ID.</value>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Gets or sets the name of the session.
    /// </summary>
    /// <value>A string containing the session name.</value>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    public string Name { get; private set; }
    
    /// <summary>
    /// Creates a new session with the specified name and project ID.
    /// </summary>
    /// <param name="name">The name of the session.</param>
    /// <param name="projectId">The ID of the project this session belongs to.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty.</exception>
    public Session(string name, Guid projectId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Session name cannot be empty", nameof(name));
        
        Id = Guid.NewGuid();
        Name = name;
    }
    
    /// <summary>
    /// Adds a message to the session.
    /// </summary>
    /// <param name="message">The message to add.</param>
    /// <returns>True if the message was added successfully; otherwise, false.</returns>
    public bool AddMessage(Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));
        
        _messages.Add(message);
        return true;
    }
}
```

### 4.2 Method Documentation

```csharp
/// <summary>
/// Retrieves a session by its unique identifier.
/// </summary>
/// <param name="sessionId">The unique identifier of the session to retrieve.</param>
/// <param name="cancellationToken">A token to cancel the operation.</param>
/// <returns>
/// A task representing the asynchronous operation.
/// The task result contains the session if found; otherwise, null.
/// </returns>
/// <exception cref="ArgumentException">Thrown when sessionId is empty.</exception>
/// <remarks>
/// This method queries the database for the session with the specified ID.
/// If the session is not found, null is returned instead of throwing an exception.
/// </remarks>
public async Task<Session?> GetByIdAsync(
    Guid sessionId, 
    CancellationToken cancellationToken = default)
{
    if (sessionId == Guid.Empty)
        throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
    
    return await _context.Sessions
        .AsNoTracking()
        .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
}
```

---

## 5. README Template

### 5.1 Project README

```markdown
# Versa Coder

AI-powered IDE platform built with C# .NET 8 and DevExpress 2026 Universal.

## Features

- AI-powered code assistance
- Multiple AI provider support (OpenAI, Anthropic, Ollama)
- Session management
- Clean Architecture (L0-L7)
- MDI + Ribbon UI

## Getting Started

### Prerequisites

- .NET 8 SDK
- DevExpress 2026 Universal
- SQLite

### Installation

1. Clone the repository
2. Open `VersaCoder.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Run the application

## Architecture

The project follows Clean Architecture with the following layers:

- **L0 Domain**: Core business entities and rules
- **L1 Abstractions**: Interfaces and contracts
- **L2 Application**: Business logic and use cases
- **L3 CrossCutting**: Logging, validation, exception handling
- **L4 Infrastructure**: Data access, external services
- **L5 Protocol**: AI protocols and provider communication
- **L6 Host**: Application startup and DI configuration
- **L7 UI**: DevExpress WinForms interface

## Configuration

Configuration is stored in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder.db"
  },
  "AI": {
    "DefaultProvider": "OpenAI",
    "Providers": {
      "OpenAI": {
        "ApiKey": "${OPENAI_API_KEY}",
        "Models": ["gpt-4o", "gpt-4.1"]
      }
    }
  }
}
```

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
```

---

## 6. Changelog Template

### 6.1 Changelog Format

```markdown
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- New session management feature
- AI provider support for Ollama

### Changed
- Improved UI responsiveness
- Updated DevExpress to version 2026

### Fixed
- Fixed null reference exception in SessionService
- Fixed memory leak in ChatPanelView

### Removed
- Removed deprecated LegacyProvider

## [1.0.0] - 2026-08-25

### Added
- Initial release
- Clean Architecture implementation
- AI provider support (OpenAI, Anthropic)
- MDI + Ribbon UI
- Session management
```

---

## 7. ADR Template

### 7.1 Architecture Decision Record

```markdown
# ADR-001: Use Clean Architecture

## Status

Accepted

## Date

2026-08-25

## Context

We need to decide on the architecture for the VersaCoder project. The project will be a long-lived application that needs to be maintainable and testable.

## Decision

We will use Clean Architecture with the following layers:
- L0: Domain
- L1: Abstractions
- L2: Application
- L3: CrossCutting
- L4: Infrastructure
- L5: Protocol
- L6: Host
- L7: UI

## Consequences

### Positive
- Clear separation of concerns
- High testability
- Easy to maintain
- Framework independence

### Negative
- More complex project structure
- Learning curve for new developers
- More files to manage

## Alternatives Considered

### MVC Architecture
- Simpler structure
- Less separation of concerns
- Harder to test

### Hexagonal Architecture
- Similar to Clean Architecture
- Different terminology
- Less community support

## References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/clean-architecture.html)
- [Microsoft Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/clean-architecture)
```

---

## 8. Documentation Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | XML comments eklendi | ☐ |
| 2 | README güncellendi | ☐ |
| 3 | Changelog güncellendi | ☐ |
| 4 | ADR oluşturuldu | ☐ |
| 5 | API docs güncellendi | ☐ |
| 6 | Kullanım rehberi yazıldı | ☐ |
| 7 | Mimari diagram güncellendi | ☐ |
| 8 | Deploy rehberi güncellendi | ☐ |
| 9 | Troubleshooting rehberi yazıldı | ☐ |
| 10 | FAQ güncellendi | ☐ |

---

## 9. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Document Types | 5 (API, README, Guide, ADR, Changelog) |
| Templates | 4 (README, Changelog, ADR, API) |
| Standards | 5 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
