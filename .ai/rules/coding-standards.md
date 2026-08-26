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

## 17. C# 12 Özellik Kullanım Standartları

### 17.1 Primary Constructors

```csharp
// ✅ Doğru — Primary constructor
public class OrderService(IOrderRepository repository, ILogger<OrderService> logger)
{
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct)
    {
        logger.LogInformation("Creating order for {CustomerId}", request.CustomerId);
        var order = new Order(request.CustomerId, request.Items);
        await repository.AddAsync(order, ct);
        return order;
    }
}

// ❌ Yanlış — Geleneksel constructor (primary constructor tercih edilmeli)
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository repository, ILogger<OrderService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}
```

### 17.2 Collection Expressions

```csharp
// ✅ Doğru — Collection expression
int[] numbers = [1, 2, 3, 4, 5];
List<string> names = ["Alice", "Bob", "Charlie"];
Dictionary<string, int> ages = { ["Alice"] = 30, ["Bob"] = 25 };

// ✅ Doğru — Spread operator
int[] moreNumbers = [..numbers, 6, 7, 8];

// ❌ Yanlış — Eski syntax
int[] numbers = new int[] { 1, 2, 3, 4, 5 };
List<string> names = new List<string> { "Alice", "Bob", "Charlie" };
```

### 17.3 Raw String Literals

```csharp
// ✅ Doğru — Raw string literal
string json = """
    {
        "name": "VersaCoder",
        "version": "1.0.0"
    }
    """;

string sql = """
    SELECT s.Id, s.Name, COUNT(m.Id) as MessageCount
    FROM Sessions s
    LEFT JOIN Messages m ON s.Id = m.SessionId
    GROUP BY s.Id, s.Name
    """;

// ❌ Yanlış — Escape character kullanımı
string json = "{\n    \"name\": \"VersaCoder\"\n}";
```

### 17.4 Pattern Matching

```csharp
// ✅ Doğru — Enhanced pattern matching
public string GetStatusDescription(SessionStatus status) => status switch
{
    SessionStatus.Active => "Oturum aktif",
    SessionStatus.Paused => "Oturum duraklatıldı",
    SessionStatus.Completed => "Oturum tamamlandı",
    SessionStatus.Archived => "Oturum arşivlendi",
    _ => "Bilinmeyen durum"
};

// ✅ Doğru — Property pattern
public bool IsHighPriority(TaskItem task) => task is
{
    Priority: Priority.CRITICAL or Priority.HIGH,
    Status: not TaskStatus.Completed
};

// ✅ Doğru — List pattern
public bool IsEmptyList(int[] numbers) => numbers is [];
```

### 17.5 File-scoped Namespaces

```csharp
// ✅ Doğru — File-scoped namespace
namespace VersaCoder.Domain.Entities;

public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
}

// ❌ Yanlış — Block-scoped namespace
namespace VersaCoder.Domain.Entities
{
    public class Session
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
    }
}
```

### 17.6 Nullable Reference Types

```csharp
// ✅ Doğru — Nullable reference types kullanımı
public class UserService
{
    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    public async Task<User> GetRequiredUserAsync(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        return user ?? throw new NotFoundException($"User {id} not found");
    }
}

// ✅ Doğru — Null-conditional operator
public string GetDisplayName(User? user) =>
    user?.DisplayName ?? user?.Email ?? "Anonymous";

// ✅ Doğru — Null-coalescing assignment
private List<string> _tags ??= [];
```

### 17.7 Async Streams

```csharp
// ✅ Doğru — IAsyncEnumerable kullanımı
public async IAsyncEnumerable<Message> StreamMessagesAsync(
    Guid sessionId,
    [EnumeratorCancellation] CancellationToken ct)
{
    await foreach (var message in _repository.GetMessagesStreamAsync(sessionId, ct))
    {
        yield return message;
    }
}

// Kullanım
await foreach (var message in StreamMessagesAsync(sessionId, CancellationToken.None))
{
    Console.WriteLine($"[{message.Timestamp}] {message.Role}: {message.Content}");
}
```

### 17.8 Required Members

```csharp
// ✅ Doğru — Required members
public class CreateSessionRequest
{
    public required string Name { get; init; }
    public required Guid ProjectId { get; init; }
    public string? Description { get; init; }
    public string? BranchName { get; init; }
}

// Kullanım — compile-time kontrolü
var request = new CreateSessionRequest
{
    Name = "Yeni Oturum",      // Zorunlu
    ProjectId = Guid.NewGuid() // Zorunlu
    // Description opsiyonel
};
```

---

## 18. SOLID Prensip Uygulama Örnekleri

### 18.1 Single Responsibility (SRP)

```csharp
// ✅ Doğru — Her sınıfın tek bir sorumluluğu
public class OrderCalculator
{
    public decimal CalculateTotal(Order order) =>
        order.Items.Sum(i => i.Price * i.Quantity);
}

public class OrderValidator
{
    public ValidationResult Validate(CreateOrderRequest request)
    {
        var errors = new List<string>();
        if (request.Items.Count == 0)
            errors.Add("En az bir ürün eklenmeli");
        return new ValidationResult(errors);
    }
}

public class OrderPersister
{
    public async Task SaveAsync(Order order, CancellationToken ct) =>
        await _repository.AddAsync(order, ct);
}

// ❌ Yanlış — Tek sınıfda birden fazla sorumluluk
public class OrderManager
{
    public decimal CalculateTotal(Order order) { ... }
    public ValidationResult Validate(CreateOrderRequest request) { ... }
    public async Task SaveAsync(Order order, CancellationToken ct) { ... }
    public void SendConfirmationEmail(Order order) { ... }
}
```

### 18.2 Open/Closed (OCP)

```csharp
// ✅ Doğru — Interface ile open/closed
public interface IReportGenerator
{
    ReportType Type { get; }
    Task<Report> GenerateAsync(ReportRequest request, CancellationToken ct);
}

public class PdfReportGenerator : IReportGenerator
{
    public ReportType Type => ReportType.PDF;
    public async Task<Report> GenerateAsync(ReportRequest request, CancellationToken ct) { ... }
}

public class ExcelReportGenerator : IReportGenerator
{
    public ReportType Type => ReportType.Excel;
    public async Task<Report> GenerateAsync(ReportRequest request, CancellationToken ct) { ... }
}

public class ReportService
{
    private readonly Dictionary<ReportType, IReportGenerator> _generators;

    public ReportService(IEnumerable<IReportGenerator> generators)
    {
        _generators = generators.ToDictionary(g => g.Type);
    }

    public async Task<Report> GenerateAsync(ReportRequest request, CancellationToken ct)
    {
        var generator = _generators[request.Type];
        return await generator.GenerateAsync(request, ct);
    }
}
```

### 18.3 Liskov Substitution (LSP)

```csharp
// ✅ Doğru — Tüm implementasyonlar interface'e uygun
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
}

// Her repository aynı contract'a uygun
public class SessionRepository : IRepository<Session>
{
    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken ct) { ... }
    public async Task AddAsync(Session entity, CancellationToken ct) { ... }
}

public class ProjectRepository : IRepository<Project>
{
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct) { ... }
    public async Task AddAsync(Project entity, CancellationToken ct) { ... }
}
```

### 18.4 Interface Segregation (ISP)

```csharp
// ✅ Doğru — İnce granüllü interface'ler
public interface IReadableRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);
}

public interface IWritableRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
    Task DeleteAsync(T entity, CancellationToken ct);
}

public interface IRepository<T> : IReadableRepository<T>, IWritableRepository<T>
    where T : class { }

// ❌ Yanlış — Çok geniş interface
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
    Task DeleteAsync(T entity, CancellationToken ct);
    Task BulkInsertAsync(IEnumerable<T> entities, CancellationToken ct);
    Task ExecuteSqlAsync(string sql, CancellationToken ct);
}
```

### 18.5 Dependency Inversion (DIP)

```csharp
// ✅ Doğru — Düşük seviyeli modüller soyutlara bağımlı
// Abstraction katmanı (L1)
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct);
}

// Infrastructure katmanı (L4)
public class SmtpEmailService : IEmailService
{
    private readonly SmtpClient _client;

    public SmtpEmailService(IConfiguration config)
    {
        _client = new SmtpClient(config["Smtp:Host"]);
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct)
    {
        var message = new MailMessage("noreply@versacoder.com", to, subject, body);
        await _client.SendMailAsync(message, ct);
    }
}

// Application katmanı (L2) — SmtpEmailService'e bağımlı değil
public class NotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task NotifyUserAsync(User user, string message, CancellationToken ct)
    {
        await _emailService.SendAsync(user.Email, "Bildirim", message, ct);
    }
}
```

---

## 19. Refactoring Kalıpları

### 19.1 Extract Method

```csharp
// ❌ Önce — Uzun metod
public void ProcessOrder(Order order)
{
    // 100+ satır kod...
    var total = order.Items.Sum(i => i.Price * i.Quantity);
    var discount = order.Customer.IsPremium ? total * 0.1m : 0;
    var tax = (total - discount) * 0.2m;
    order.TotalAmount = total - discount + tax;
}

// ✅ Sonra — Ayrılmış metodlar
public void ProcessOrder(Order order)
{
    var total = CalculateSubtotal(order);
    var discount = CalculateDiscount(order, total);
    var tax = CalculateTax(total, discount);
    order.TotalAmount = total - discount + tax;
}

private decimal CalculateSubtotal(Order order) =>
    order.Items.Sum(i => i.Price * i.Quantity);

private decimal CalculateDiscount(Order order, decimal subtotal) =>
    order.Customer.IsPremium ? subtotal * 0.1m : 0m;

private decimal CalculateTax(decimal subtotal, decimal discount) =>
    (subtotal - discount) * 0.2m;
```

### 19.2 Extract Class

```csharp
// ❌ Önce — Çok sorumlu sınıf
public class SessionManager
{
    public void CreateSession(...) { ... }
    public void SendMessage(...) { ... }
    public void GenerateReport(...) { ... }
    public void BackupData(...) { ... }
}

// ✅ Sonra — Ayrılmış sınıflar
public class SessionService { ... }
public class MessageService { ... }
public class ReportService { ... }
public class BackupService { ... }
```

### 19.3 Replace Temp with Query

```csharp
// ❌ Önce
var basePrice = _item.Price * _quantity;
var discount = basePrice > 100 ? basePrice * 0.1 : 0;
var finalPrice = basePrice - discount;

// ✅ Sonra
private decimal BasePrice => _item.Price * _quantity;
private decimal Discount => BasePrice > 100 ? BasePrice * 0.1m : 0m;
private decimal FinalPrice => BasePrice - Discount;
```

---

## 20. Hata Yönetimi Kod Kalitesi

### 20.1 Result Pattern

```csharp
// ✅ Doğru — Result pattern
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string error, ErrorType errorType)
    {
        IsSuccess = false;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, ErrorType type) => new(error, type);
}

// Kullanım
public async Task<Result<Session>> GetSessionAsync(Guid id, CancellationToken ct)
{
    var session = await _repository.GetByIdAsync(id, ct);
    if (session is null)
        return Result<Session>.Failure("Session bulunamadı", ErrorType.NotFound);
    return Result<Session>.Success(session);
}
```

### 20.2 Global Exception Handler

```csharp
// ✅ Doğru — Merkezi hata yönetimi
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async Task HandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, ve.Message),
            NotFoundException ne => (StatusCodes.Status404NotFound, ne.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Yetki yok"),
            TimeoutException => (StatusCodes.Status408RequestTimeout, "Zaman aşımı"),
            _ => (StatusCodes.Status500InternalServerError, "Sunucu hatası")
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = message }, ct);
    }
}
```

### 20.3 FluentValidation Kullanımı

```csharp
// ✅ Doğru — FluentValidation
public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Session adı boş olamaz")
            .MaximumLength(200).WithMessage("Session adı 200 karakterden uzun olamaz");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Proje ID'si boş olamaz");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama 1000 karakterden uzun olamaz")
            .When(x => x.Description is not null);
    }
}
```

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Enhanced |
| C# Features | 8 (C# 12) |
| SOLID Examples | 5 |
| Refactoring Patterns | 3 |
| Error Handling Patterns | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
