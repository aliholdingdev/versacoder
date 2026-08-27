---
title: "Versa Coder — L3 CrossCutting Layer Guide"
type: architecture
category: layer
layer: L3
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L3 CrossCutting Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/l2-application/application-guide]] · [[brain.md]]

---

## 1. Amaç

CrossCutting katmanı, uygulama genelinde **logging, exception handling ve validation** gibi kesen endişeleri (cross-cutting concerns) yönetir.

---

## 2. MediatR Pipeline Behaviors

| Behavior | Dosya | Tanım | Satır |
|----------|-------|-------|-------|
| `LoggingBehavior<TRequest,TResponse>` | `Behaviors/LoggingBehavior.cs` | Her handler öncesi/sonrası log | 29 |
| `PerformanceBehavior<TRequest,TResponse>` | `Behaviors/PerformanceBehavior.cs` | 500ms üzeri yavaş handler uyarı | — |
| `ValidationBehavior<TRequest,TResponse>` | `Behaviors/ValidationBehavior.cs` | FluentValidation doğrulama | 43 |

---

## 3. Exception Tipleri

| Exception | Dosya | Tanım |
|-----------|-------|-------|
| `DomainException` | `Exceptions/DomainException.cs` | Domain kural ihlali |
| `NotFoundException` | `Exceptions/NotFoundException.cs` | Kaynak bulunamadı |
| `ValidationException` | `Exceptions/ValidationException.cs` | Validasyon hatası |
| `GlobalExceptionHandler` | `Exceptions/GlobalExceptionHandler.cs` | Merkezi hata yönetimi |

---

## 4. Pipeline Akışı

```
Request
  → LoggingBehavior (log yaz)
    → PerformanceBehavior (süre ölç)
      → ValidationBehavior (FluentValidation kontrol)
        → Handler (iş mantığı)
          → Response
```

---

## 5. Hata Hiyerarşisi

```
VersaCoderException (Base)
  ├── DomainException
  │     ├── ValidationException
  │     ├── NotFoundException
  │     └── DuplicateException
  ├── InfrastructureException
  │     ├── DatabaseException
  │     ├── ProviderException
  │     └── NetworkException
  └── ProtocolException
        ├── MCPException
        └── AgentException
```

---

## 6. Logging Stratejisi

| Level | Kullanım | Örnek |
|-------|----------|-------|
| Verbose | Detaylı debug | Variable values |
| Debug | Geliştirme bilgisi | Method entry/exit |
| Information | Normal olaylar | Request completed |
| Warning | Uyarılar | Slow query |
| Error | Hatalar | Exception thrown |
| Fatal | Kritik hatalar | System crash |

---

## 7. Kurallar

| # | Kural |
|---|-------|
| 1 | CrossCutting, Application'a bağımlı (L3 → L2 ✅) |
| 2 | CrossCutting, Domain'e bağımlı DEĞİL (L3 → L0 ❌) |
| 3 | Tüm handler'lar pipeline behaviors'lardan geçer |
| 4 | Hatalar merkezi olarak yönetilir |
| 5 | Structured logging zorunlu (Serilog) |

---

## 6. Behavior Detayları

### 6.1 LoggingBehavior

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();
        
        _logger.LogInformation("[{RequestId}] Handling {RequestName}",
            requestId, requestName);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            _logger.LogInformation(
                "[{RequestId}] Handled {RequestName} in {ElapsedMs}ms",
                requestId, requestName, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "[{RequestId}] Error handling {RequestName} in {ElapsedMs}ms",
                requestId, requestName, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
```

### 6.2 PerformanceBehavior

```csharp
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly int _thresholdMs;
    
    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        int thresholdMs = 500)
    {
        _logger = logger;
        _thresholdMs = thresholdMs;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var response = await next();
        
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > _thresholdMs)
        {
            _logger.LogWarning(
                "Slow request: {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                _thresholdMs);
        }
        
        return response;
    }
}
```

### 6.3 ValidationBehavior

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();
        
        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        
        if (failures.Any())
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }
        
        return await next();
    }
}
```

---

## 7. Exception Detayları

### 7.1 DomainException

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
    
    public DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = "DOMAIN_ERROR";
    }
}
```

### 7.2 NotFoundException

```csharp
public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object? Key { get; }
    
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" with key {key} was not found.")
    {
        EntityName = entityName;
        Key = key;
    }
    
    public NotFoundException(string entityName, object key, Exception innerException)
        : base($"Entity \"{entityName}\" with key {key} was not found.", innerException)
    {
        EntityName = entityName;
        Key = key;
    }
}
```

### 7.3 ValidationException

```csharp
public class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }
    
    public ValidationException(IReadOnlyList<string> errors)
        : base("Validation failed.")
    {
        Errors = errors;
    }
    
    public ValidationException(IEnumerable<string> errors)
        : base("Validation failed.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
```

### 7.4 GlobalExceptionHandler

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        
        var (statusCode, message) = exception switch
        {
            ValidationException validationEx => 
                (StatusCodes.Status400BadRequest, string.Join(", ", validationEx.Errors)),
            NotFoundException notFoundEx => 
                (StatusCodes.Status404NotFound, notFoundEx.Message),
            DomainException domainEx => 
                (StatusCodes.Status400BadRequest, domainEx.Message),
            _ => 
                (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };
        
        httpContext.Response.StatusCode = statusCode;
        
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = "Error",
            Detail = message,
            Instance = httpContext.Request.Path
        }, cancellationToken);
        
        return true;
    }
}
```

---

## 8. Pipeline Yapılandırması

### 8.1 DI Kayıtları

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddCrossCutting(
        this IServiceCollection services)
    {
        // Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // Exception handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        return services;
    }
}
```

### 8.2 Pipeline Sıralaması

```csharp
// Startup.cs'de pipeline sıralaması
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssembly).Assembly);
    
    // Sıralama önemli: İlk eklenen ilk çalışır
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
```

---

## 9. Middleware

### 9.1 ExceptionHandlingMiddleware

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = GetStatusCodeAndMessage(exception);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        var response = new
        {
            error = new
            {
                message,
                type = exception.GetType().Name
            }
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
    
    private (int statusCode, string message) GetStatusCodeAndMessage(Exception exception)
    {
        return exception switch
        {
            ValidationException => (400, "Validation failed"),
            NotFoundException => (404, "Resource not found"),
            DomainException => (400, "Domain error"),
            UnauthorizedAccessException => (401, "Unauthorized"),
            _ => (500, "Internal server error")
        };
    }
}
```

---

## 10. CrossCutting Testleri

### 10.1 Behavior Testleri

```csharp
public class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_ValidRequest_LogsInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(loggerMock.Object);
        var request = new TestRequest();
        var handler = new Mock<RequestHandlerDelegate<TestResponse>>();
        handler.Setup(h => h()).ReturnsAsync(new TestResponse());
        
        // Act
        await behavior.Handle(request, handler.Object, CancellationToken.None);
        
        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
```

---

## 11. CrossCutting Gelecek Planı

### 11.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Yeni behavior'lar | Yüksek |
| Exception handling geliştirme | Yüksek |
| Logging optimizasyonu | Orta |

### 11.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Custom behaviors | Orta |
| Metrics integration | Orta |
| Distributed tracing | Düşük |

### 11.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| OpenTelemetry integration | Orta |
| Custom middleware | Düşük |
| Performance optimization | Düşük |

---

## 12. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Behaviors | 3 |
| Exceptions | 4 |
| Middleware | 1 |
| Design Patterns | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
<<<<<<< HEAD
=======
**Mode:** Red Team · Human Mode · Truth Mode
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
