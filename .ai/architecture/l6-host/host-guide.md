---
title: "Versa Coder — L6 Host Layer Guide"
type: architecture
category: layer
layer: L6
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L6 Host Layer Guide

**Zorunlu Bağlantılar:** [[architecture/l5-protocol/protocol-guide]] · [[brain.md]]

---

## 1. Amaç

Host katmanı, uygulama **başlangıcını, DI (Dependency Injection) konfigürasyonunu ve uygulama yaşam döngüsünü** yönetir.

---

## 2. Başlangıç Akışı

```
Program.cs → Host.CreateDefaultBuilder()
    → ConfigureServices()
        ├── Infrastructure.Data DI
        ├── Infrastructure.AI DI
        ├── Application DI (MediatR)
        └── CrossCutting DI
    → Build()
    → Run()
```

---

## 3. DI Konfigürasyonu

```csharp
// Startup.cs
public static void ConfigureServices(IServiceCollection services)
{
    // Infrastructure.Data
    services.AddDbContext<VersaCoderDbContext>(options =>
        options.UseSqlite("Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;"));
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<ISessionRepository, SessionRepository>();
    services.AddScoped<IMessageRepository, MessageRepository>();
    services.AddScoped<IProjectRepository, ProjectRepository>();

    // Infrastructure.AI
    services.AddSingleton<ProviderRouter>();
    services.AddScoped<IAgentRunner, AgentRunner>();
    services.AddSingleton<ToolRegistry>();

    // Application (MediatR)
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(CreateSessionCommand).Assembly));
    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    // CrossCutting
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
}
```

---

## 4. appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder.db;Cache=Shared;Journal Mode=WAL;"
  },
  "AI": {
    "DefaultProvider": "OpenAI",
    "Providers": {
      "OpenAI": {
        "ApiKey": "${OPENAI_API_KEY}",
        "BaseUrl": "https://api.openai.com/v1",
        "Models": ["gpt-4o", "gpt-4.1", "o3"],
        "DefaultModel": "gpt-4o"
      },
      "Ollama": {
        "BaseUrl": "http://localhost:11434",
        "Models": ["llama3.1", "qwen2.5", "codellama"]
      }
    }
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/versacoder-.log", "rollingInterval": "Day" } }
    ]
  }
}
```

---

## 5. Kurallar

| # | Kural |
|---|-------|
| 1 | Host → Protocol ✅, Infrastructure ✅ |
| 2 | Host → Application ❌ |
| 3 | DI registration bu katmanda |
| 4 | Startup.cs tek giriş noktası |

---

## 6. Program.cs Detayları

### 6.1 Ana Giriş Noktası

```csharp
using Serilog;
using VersaCoder.CrossCutting;
using VersaCoder.Infrastructure.Data;
using VersaCoder.Infrastructure.AI;
using VersaCoder.Application;
using VersaCoder.Host.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/versacoder-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("VersaCoder starting up...");
    
    // Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    // Infrastructure.Data
    builder.Services.AddDbContext<VersaCoderDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")));
    
    // Infrastructure.AI
    builder.Services.AddHttpClient();
    builder.Services.Configure<OpenAISettings>(
        builder.Configuration.GetSection("Ai:Providers:OpenAI"));
    builder.Services.Configure<OllamaSettings>(
        builder.Configuration.GetSection("Ai:Providers:Ollama"));
    
    builder.Services.AddSingleton<ProviderRouter>();
    builder.Services.AddScoped<IAgentRunner, AgentRunner>();
    builder.Services.AddSingleton<ToolRegistry>();
    
    // Application
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(CreateSessionCommand).Assembly));
    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
    
    // CrossCutting
    builder.Services.AddCrossCutting();
    
    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
    
    var app = builder.Build();
    
    // Middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.UseExceptionHandler("/error");
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>();
    
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    
    app.MapControllers();
    app.MapHealthChecks("/health");
    
    // Database migration
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<VersaCoderDbContext>();
        await context.Database.MigrateAsync();
    }
    
    Log.Information("VersaCoder started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "VersaCoder terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

---

## 7. Middleware Detayları

### 7.1 ExceptionHandlingMiddleware

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
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);
            
            var response = new ErrorResponse
            {
                Message = GetErrorMessage(ex),
                Details = ex.Message,
                Timestamp = DateTime.UtcNow
            };
            
            await context.Response.WriteAsJsonAsync(response);
        }
    }
    
    private int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => 400,
            NotFoundException => 404,
            UnauthorizedAccessException => 401,
            ForbiddenAccessException => 403,
            ConflictException => 409,
            _ => 500
        };
    }
    
    private string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ValidationException => "Validation failed",
            NotFoundException => "Resource not found",
            UnauthorizedAccessException => "Unauthorized access",
            ForbiddenAccessException => "Access forbidden",
            ConflictException => "Resource conflict",
            _ => "An unexpected error occurred"
        };
    }
}
```

### 7.2 RequestTimingMiddleware

```csharp
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    
    public RequestTimingMiddleware(
        RequestDelegate next,
        ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        context.Items["RequestId"] = requestId;
        
        _logger.LogInformation(
            "[{RequestId}] {Method} {Path} started",
            requestId,
            context.Request.Method,
            context.Request.Path);
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            _logger.LogInformation(
                "[{RequestId}] {Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode);
            
            // Performance threshold warning
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "[{RequestId}] Slow request detected: {ElapsedMs}ms",
                    requestId,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
```

---

## 8. Health Checks

### 8.1 Custom Health Checks

```csharp
// Database Health Check
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly VersaCoderDbContext _context;
    
    public DatabaseHealthCheck(VersaCoderDbContext context)
    {
        _context = context;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT 1", cancellationToken);
            
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database is not accessible",
                ex);
        }
    }
}

// AI Provider Health Check
public class AIProviderHealthCheck : IHealthCheck
{
    private readonly ProviderRouter _router;
    
    public AIProviderHealthCheck(ProviderRouter router)
    {
        _router = router;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var providers = _router.GetAvailableProviders();
            if (!providers.Any())
            {
                return HealthCheckResult.Degraded("No AI providers available");
            }
            
            return HealthCheckResult.Healthy(
                $"Available providers: {string.Join(", ", providers)}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "AI provider check failed",
                ex);
        }
    }
}
```

### 8.2 Health Check Yapılandırması

```csharp
// Startup.cs'de health check kayıtları
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(
        "database",
        HealthStatus.Unhealthy,
        new[] { "db", "sqlite" })
    .AddCheck<AIProviderHealthCheck>(
        "ai-provider",
        HealthStatus.Degraded,
        new[] { "ai", "llm" })
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sqlserver",
        tags: new[] { "db", "sql" });

// Health check endpoint'leri
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Sadece uygulamanın çalıştığını kontrol eder
});
```

---

## 9. Configuration Management

### 9.1 Environment-Based Configuration

```csharp
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder-dev.db"
  }
}

// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder-prod.db"
  }
}
```

### 9.2 Secret Management

```csharp
// User secrets (Development)
// dotnet user-secrets init
// dotnet user-secrets set "Ai:Providers:OpenAI:ApiKey" "sk-xxx"

// Environment variables
// set OPENAI_API_KEY=sk-xxx

// Azure Key Vault (Production)
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

## 10. Serilog Yapılandırması

### 10.1 detaylı Log Yapılandırması

```csharp
// Program.cs'de Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "VersaCoder")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/versacoder-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

### 10.2 Structured Logging

```csharp
// Structured logging örnekleri
_logger.LogInformation("User {UserId} created session {SessionId}", userId, sessionId);
_logger.LogWarning("Slow query detected: {QueryTime}ms for {Query}", queryTime, query);
_logger.LogError(ex, "Failed to process request {RequestId}", requestId);

// Log filtering
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Logging.AddFilter("VersaCoder", LogLevel.Debug);
```

---

## 11. Host Testleri

### 11.1 Integration Tests

```csharp
public class HostIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public HostIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }
    
    [Fact]
    public async Task ApiEndpoint_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/sessions");
        
        response.EnsureSuccessStatusCode();
    }
}
```

---

## 12. Host Gelecek Planı

### 12.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Middleware geliştirme | Yüksek |
| Health check ekleme | Yüksek |
| Configuration management | Orta |

### 12.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Serilog integration | Orta |
| Docker support | Orta |
| Performance optimization | Düşük |

### 12.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Microservice migration | Düşük |
| Kubernetes support | Düşük |
| CI/CD pipeline | Orta |

---

## 13. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Middlewares | 2 |
| Health Checks | 2 |
| Configuration Sources | 3 |
| Log Providers | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
