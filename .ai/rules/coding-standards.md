---
title: "Versa Coder — Kod Standartları"
type: rules
category: coding
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Kod Standartları

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]]

---

## 1. C# Naming Convention

| Öğe | Format | Örnek |
|-----|--------|-------|
| Namespace | `VersaCoder.{Layer}.{Module}` | `VersaCoder.Domain.Entities` |
| Class | PascalCase | `SessionManager` |
| Method | PascalCase | `CreateSession()` |
| Property | PascalCase | `SessionId` |
| Field | _camelCase | `_sessionRepository` |
| Parameter | camelCase | `sessionDto` |
| Local Variable | camelCase | `result` |
| Constant | PascalCase | `MaxRetryCount` |
| Enum | PascalCase | `AgentRole.Build` |
| Interface | I{PascalCase} | `ISessionManager` |

---

## 2. Kod Stili

| Kural | Açıklama |
|-------|----------|
| Nullable | `#nullable enable` |
| Async/Await | Asenkron metodlarda zorunlu |
| Pattern matching | `is`, `switch` expression tercih |
| LINQ | Loop yerine LINQ tercih |
| Null check | `?.` ve `??` kullanımı |
| Encapsulation | `{ get; set; }` — public alan yasak |

---

## 3. Dosya Yapısı

| Kural | Açıklama |
|-------|----------|
| Tek class / dosya | Her dosyada tek class |
| Namespace uyumu | Dosya yolu = namespace |
| Using sırası | System → 3rd party → Project |
| Max satır | 1000 satır (MO onayı ile genişletilebilir) |

---

## 4. Yasak Örüntüleri

| ❌ Yasak | ✅ Doğru |
|----------|----------|
| `public` alan | `{ get; set; }` |
| `var` kullanımı belirsizlikte | Explicit type |
| `eval()` | Guvenli alternatifler |
| Magic number | Sabit constant |
| Nested if > 3 seviye | Early return, guard clause |

---

## 5. C# Kod Örnekleri

### 5.1 Class Yapısı

```csharp
// ✅ Doğru - Record type (immutable)
public record SessionDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    int MessageCount);

// ✅ Doğru - Class with encapsulation
public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<Message> _messages = new();
    
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();
    
    public Session(string name, Guid projectId)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CreatedAt = DateTime.UtcNow;
    }
    
    public void AddMessage(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _messages.Add(message);
    }
}
```

### 5.2 Interface Tanımları

```csharp
// ✅ Doğru - Interface-first tasarım
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Session>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Session>> FindAsync(
        Expression<Func<Session, bool>> predicate, 
        CancellationToken ct = default);
    Task<Session> AddAsync(Session session, CancellationToken ct = default);
    void Update(Session session);
    void Remove(Session session);
}

// ✅ Doğru - Generic repository
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}
```

### 5.3 Async/Await Pattern

```csharp
// ✅ Doğru - Async all the way
public async Task<Result<SessionDto>> GetSessionAsync(
    Guid sessionId, 
    CancellationToken ct = default)
{
    var session = await _repository.GetByIdAsync(sessionId, ct);
    
    if (session == null)
        return Result<SessionDto>.Failure("Session not found");
    
    return Result<SessionDto>.Success(session.ToDto());
}

// ❌ Yanlış - Blocking calls
public SessionDto GetSession(Guid sessionId)
{
    var session = _repository.GetByIdAsync(sessionId).Result; // ❌ Blocking!
    return session?.ToDto();
}
```

---

## 6. Error Handling

### 6.1 Exception Hierarchy

```csharp
// Base exception
public abstract class VersaCoderException : Exception
{
    public string ErrorCode { get; }
    
    protected VersaCoderException(string message, string errorCode) 
        : base(message)
    {
        ErrorCode = errorCode;
    }
    
    protected VersaCoderException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

// Domain exceptions
public class DomainException : VersaCoderException
{
    public DomainException(string message) 
        : base(message, "DOMAIN_ERROR") { }
    
    public DomainException(string message, Exception innerException)
        : base(message, "DOMAIN_ERROR", innerException) { }
}

// Application exceptions
public class NotFoundException : VersaCoderException
{
    public string EntityName { get; }
    public object? Key { get; }
    
    public NotFoundException(string entityName, object key)
        : base($"Entity {entityName} with key {key} was not found.", "NOT_FOUND")
    {
        EntityName = entityName;
        Key = key;
    }
}

// Validation exception
public class ValidationException : VersaCoderException
{
    public IReadOnlyList<string> Errors { get; }
    
    public ValidationException(IEnumerable<string> errors)
        : base("Validation failed.", "VALIDATION_ERROR")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
```

### 6.2 Result Pattern

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    
    public Result<TNew> Map<TNew>(Func<T, TNew> map)
    {
        return IsSuccess
            ? Result<TNew>.Success(map(Value!))
            : Result<TNew>.Failure(Error!);
    }
}
```

---

## 7. Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    ISessionRepository Sessions { get; }
    IMessageRepository Messages { get; }
    IProjectRepository Projects { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly VersaCoderDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private ISessionRepository? _sessions;
    private IMessageRepository? _messages;
    private IProjectRepository? _projects;
    
    public UnitOfWork(VersaCoderDbContext context)
    {
        _context = context;
    }
    
    public ISessionRepository Sessions =>
        _sessions ??= new SessionRepository(_context);
    
    public IMessageRepository Messages =>
        _messages ??= new MessageRepository(_context);
    
    public IProjectRepository Projects =>
        _projects ??= new ProjectRepository(_context);
    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
    
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }
    
    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

---

## 8. Logging Standards

```csharp
// ✅ Doğru - Structured logging
_logger.LogInformation("User {UserId} created session {SessionId}", userId, sessionId);
_logger.LogWarning("Slow query detected: {QueryTime}ms", queryTime);
_logger.LogError(ex, "Failed to process request {RequestId}", requestId);

// ❌ Yanlış - String interpolation
_logger.LogInformation($"User {userId} created session {sessionId}");

// ❌ Yanlış - String concatenation
_logger.LogInformation("User " + userId + " created session " + sessionId);
```

---

## 9. Code Review Checklist

| # | Kontrol | Açıklama |
|---|---------|----------|
| 1 | Nullable annotations | `#nullable enable` aktif mi? |
| 2 | Async/Await | Tüm asenkron metodlar `Async` suffix mi? |
| 3 | Error handling | Exception fırlatılıyor mu? Result pattern kullanılıyor mu? |
| 4 | Null checks | `ArgumentNullException.ThrowIfNull` kullanılıyor mu? |
| 5 | Unit tests | Yeterli test coverage var mı? |
| 6 | Documentation | XML comments eklendi mi? |
| 7 | Performance | N+1 query var mı? |
| 8 | Security | SQL injection riski var mı? |
| 9 | Naming | Naming convention uyuyor mu? |
| 10 | Clean Code | Tek sorumluluk ilkesi korunuyor mu? |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Naming Rules | 11 |
| Code Style Rules | 6 |
| File Rules | 4 |
| Forbidden Patterns | 5 |
| Error Types | 4 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
