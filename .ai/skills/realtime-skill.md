---
title: "Versa Coder — SignalR Real-time Skill"
type: skill
category: realtime
date: 2026-08-26
updated: 2026-08-26
status: active
version: 1.0.0
---

# Versa Coder — SignalR Real-time Skill

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]] · [[WORKFLOW.md]]

---

## 1. Amaç

Versa Coder ekosistemindeki tüm real-time iletişim ihtiyaçlarını karşılayan **SignalR entegrasyon skill'ini** tanımlar. Bu skill, AI yanıtlarının streaming'i, bildirimler, agent durumu izleme ve gerçek zamanlı dashboard güncellemeleri için kullanılır.

---

## 2. Skill Tanımı

| Özellik | Değer |
|---------|-------|
| Skill Adı | `realtime-signalr` |
| Versiyon | 1.0.0 |
| Bağımlılıklar | Microsoft.AspNetCore.SignalR, Microsoft.AspNetCore.SignalR.Client |
| Kullanım Alanları | AI Streaming, Bildirimler, Dashboard, Agent Durumu |

---

## 3. Hub Tanımları

### 3.1 ChatHub — AI Yanıt Streaming

```csharp
[Authorize]
public class ChatHub : Hub
{
    private readonly IAiService _aiService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IAiService aiService, ILogger<ChatHub> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı prompt gönderir, AI yanıtı streaming olarak döner
    /// </summary>
    public async Task SendPrompt(string sessionId, string prompt)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("Prompt received: Session={SessionId}, User={UserId}", sessionId, userId);

        // Gruba katıl
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session:{sessionId}");

        try
        {
            await foreach (var chunk in _aiService.StreamResponseAsync(sessionId, prompt, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("ReceiveChunk", new
                {
                    SessionId = sessionId,
                    Content = chunk.Content,
                    IsComplete = chunk.IsComplete,
                    TokenCount = chunk.TokenCount,
                    Model = chunk.Model
                });
            }

            await Clients.Caller.SendAsync("ResponseComplete", new
            {
                SessionId = sessionId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Stream cancelled: Session={SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stream error: Session={SessionId}", sessionId);
            await Clients.Caller.SendAsync("StreamError", new
            {
                SessionId = sessionId,
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Mevcut AI yanıtını durdur
    /// </summary>
    public async Task StopGeneration(string sessionId)
    {
        await _aiService.CancelGenerationAsync(sessionId);
        await Clients.Caller.SendAsync("GenerationStopped", new { SessionId = sessionId });
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await Clients.Caller.SendAsync("Connected", new
        {
            ConnectionId = Context.ConnectionId,
            Timestamp = DateTime.UtcNow
        });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}, Error: {Error}",
            Context.ConnectionId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }
}
```

### 3.2 AgentHub — Agent Durumu İzleme

```csharp
[Authorize]
public class AgentHub : Hub
{
    private readonly IAgentRegistry _registry;

    public AgentHub(IAgentRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// Agent durumu değişikliğini tüm clientslara bildir
    /// </summary>
    public async Task BroadcastAgentStatus(AgentStatusUpdate update)
    {
        await Clients.Group("agents").SendAsync("AgentStatusChanged", update);
    }

    /// <summary>
    /// Görev ilerlemesini bildir
    /// </summary>
    public async Task BroadcastTaskProgress(TaskProgress progress)
    {
        await Clients.Group("tasks").SendAsync("TaskProgress", progress);
    }

    /// <summary>
    /// Agent grubuna katıl
    /// </summary>
    public async Task JoinAgentGroup(string agentRole)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"agent:{agentRole}");
    }

    /// <summary>
    /// Tüm agent durumlarını iste
    /// </summary>
    public async Task RequestAllAgentStatus()
    {
        var statuses = await _registry.GetAllStatusesAsync();
        await Clients.Caller.SendAsync("AllAgentStatus", statuses);
    }
}
```

### 3.3 NotificationHub — Bildirim Sistemi

```csharp
[Authorize]
public class NotificationHub : Hub
{
    /// <summary>
    /// Kullanıcıya özel bildirim gönder
    /// </summary>
    public async Task SendPersonalNotification(string userId, Notification notification)
    {
        await Clients.User(userId).SendAsync("Notification", notification);
    }

    /// <summary>
    /// Sistem genelinde broadcast
    /// </summary>
    public async Task BroadcastSystemNotification(SystemNotification notification)
    {
        await Clients.All.SendAsync("SystemNotification", notification);
    }

    /// <summary>
    /// Bildirim grubuna katıl
    /// </summary>
    public async Task JoinNotificationGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
}
```

### 3.4 ToolHub — Tool Çıktı Streaming

```csharp
[Authorize]
public class ToolHub : Hub
{
    /// <summary>
    /// Tool執行 sonucunu streaming olarak gönder
    /// </summary>
    public async Task StreamToolOutput(string taskId, string toolName)
    {
        await Clients.Caller.SendAsync("ToolOutputStarted", new
        {
            TaskId = taskId,
            ToolName = toolName,
            Timestamp = DateTime.UtcNow
        });

        // Tool çıktısını izle ve streaming yap
        await foreach (var output in MonitorToolOutputAsync(taskId, Context.ConnectionAborted))
        {
            await Clients.Caller.SendAsync("ToolOutputChunk", new
            {
                TaskId = taskId,
                Content = output.Content,
                IsError = output.IsError,
                Timestamp = output.Timestamp
            });
        }

        await Clients.Caller.SendAsync("ToolOutputComplete", new { TaskId = taskId });
    }

    private async IAsyncEnumerable<ToolOutputChunk> MonitorToolOutputAsync(
        string taskId, [EnumeratorCancellation] CancellationToken ct)
    {
        // Tool output monitoring implementation
        yield break;
    }
}
```

---

## 4. Connection Management

### 4.1 Bağlantı Yapılandırması

```csharp
// Program.cs'de SignalR yapılandırması
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = false; // Production'da false
    options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
})
.AddStackExchangeRedis(builder.Configuration["Redis:ConnectionString"]!, options =>
{
    options.Configuration.ChannelPrefix = "versacoder";
});
```

### 4.2 Reconnection Stratejisi

```csharp
// Client-side reconnection with exponential backoff
public class ReconnectionHandler
{
    private readonly HubConnection _connection;
    private int _retryCount = 0;
    private readonly int _maxRetries = 10;
    private readonly int[] _delays = [1000, 2000, 4000, 8000, 16000, 30000, 30000, 30000, 60000, 60000];

    public async Task StartAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await _connection.StartAsync(ct);
                _retryCount = 0;
                Console.WriteLine("Bağlantı kuruldu");
                return;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var delay = _delays[Math.Min(_retryCount, _delays.Length - 1)];
                Console.WriteLine($"Bağlantı başarısız. {delay}ms sonra yeniden denenecek... ({_retryCount + 1}/{_maxRetries})");
                await Task.Delay(delay, ct);
                _retryCount++;
            }
        }
    }
}
```

### 4.3 Client Builder

```csharp
public static HubConnection CreateHubConnection(string hubUrl, string accessToken)
{
    return new HubConnectionBuilder()
        .WithUrl(hubUrl, options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(accessToken)!;
            options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents;
            options.SkipNegotiation = true;
        })
        .WithAutomaticReconnect(new[]
        {
            TimeSpan.Zero,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(30)
        })
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        })
        .Build();
}
```

---

## 5. Message Protocol

### 5.1 Mesaj Formatları

```csharp
// AI yanıt chunk'ı
public record AiResponseChunk
{
    public string SessionId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public bool IsComplete { get; init; }
    public int TokenCount { get; init; }
    public string Model { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

// Agent durumu güncelleme
public record AgentStatusUpdate
{
    public string AgentRole { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty; // idle, working, error
    public string? CurrentTask { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

// Görev ilerleme
public record TaskProgress
{
    public string TaskId { get; init; } = string.Empty;
    public string Agent { get; init; } = string.Empty;
    public int PercentComplete { get; init; }
    public string StatusMessage { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

// Bildirim
public record Notification
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Type { get; init; } = string.Empty; // info, success, warning, error
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
```

### 5.2 Grup Yönetimi

| Grup | Amaç | Katılım |
|------|------|---------|
| `session:{id}` | Oturum bazlı | Oturum başında |
| `agent:{role}` | Agent bazlı | Agent başlatıldığında |
| `user:{id}` | Kullanıcı bazlı | Giriş yaptığında |
| `admin` | Yönetici grubu | Admin kullanıcılar |
| `dashboard` | Dashboard izleme | Dashboard açıldığında |

---

## 6. Scale-Out Stratejisi

### 6.1 Redis Backplane

```csharp
// Redis yapılandırması
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "versacoder-signalr";
        options.Configuration.AbortOnConnectFail = false;
    });
```

### 6.2 Azure SignalR Service

```csharp
// Azure SignalR Service kullanımı
builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration["SignalR:ConnectionString"]!);
```

### 6.3 Karşılaştırma

| Özellik | Redis Backplane | Azure SignalR |
|---------|-----------------|---------------|
| Kurulum | Kendi sunucunuz | Managed service |
| Maliyet | Düşük (Redis zaten varsa) | Orta |
| Ölçek | İyi | Çok iyi |
| Bakım | Kendi bakımınız | Microsoft bakım |
| Availability | Tek nokta riski | SLA %99.9 |
| Bağlantı limiti | Redis limiti | 1K-100K |

---

## 7. Authentication & Authorization

### 7.1 JWT Token Doğrulama

```csharp
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = false;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth:Authority"];
    options.Audience = "versacoder-api";
    options.RequireHttpsMetadata = true;
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // SignalR'da query string'den token al
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
```

### 7.2 Hub Authorization

```csharp
[Authorize(Policy = "CanAccessChat")]
public class ChatHub : Hub
{
    // Sadece yetkili kullanıcılar erişebilir
}

[Authorize(Roles = "Admin,Agent")]
public class AgentHub : Hub
{
    // Sadece admin ve agent'lar erişebilir
}
```

---

## 8. Error Handling

### 8.1 Hub Exception Filter

```csharp
public class HubExceptionFilter : IHubFilter
{
    private readonly ILogger<HubExceptionFilter> _logger;

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, HubDelegate next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (OperationCanceledException)
        {
            // İstemci bağlantısı kesildi — normal durum
            return null;
        }
        catch (HubException ex)
        {
            _logger.LogWarning(ex, "Hub error in {Method}", invocationContext.HubMethodName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", invocationContext.HubMethodName);
            throw new HubException("Beklenmeyen bir hata oluştu");
        }
    }
}
```

### 8.2 Client-Side Error Handling

```csharp
_hubConnection.On<AiResponseChunk>("ReceiveChunk", chunk =>
{
    if (chunk.IsComplete)
    {
        OnResponseComplete(chunk);
    }
    else
    {
        AppendToResponse(chunk.Content);
    }
});

_hubConnection.On("StreamError", (dynamic error) =>
{
    ShowError($"AI yanıt hatası: {error.Error}");
    IsGenerating = false;
});

_hubConnection.Closed += async (error) =>
{
    ShowWarning("Bağlantı kesildi, yeniden bağlanıyor...");
    await Task.Delay(5000);
    await _hubConnection.StartAsync();
};
```

---

## 9. Performance Optimizasyonları

### 9.1 Streaming Optimizasyonu

| Teknik | Açıklama | Kazanç |
|--------|----------|--------|
| Chunked transfer | Küçük parçalar halinde gönderim | Düşük bellek |
| Compression | gzip/brotli sıkıştırma | %60-80 bant genişliği |
| Batching | Mesajları toplu gönderme | Azaltılmış overhead |
| Throttling | Aşırı gönderimi engelleme | Kaynak koruma |

### 9.2 Connection Pooling

```csharp
// Singleton HubConnection kullanımı
public class HubConnectionPool
{
    private readonly ConcurrentDictionary<string, HubConnection> _connections = new();

    public HubConnection GetOrCreate(string hubUrl, string token)
    {
        return _connections.GetOrAdd(hubUrl, url =>
        {
            return new HubConnectionBuilder()
                .WithUrl(url, options => options.AccessTokenProvider = () => Task.FromResult(token)!)
                .WithAutomaticReconnect()
                .Build();
        });
    }
}
```

---

## 10. Testing

### 10.1 Unit Test

```csharp
public class ChatHubTests
{
    private readonly Mock<IAiService> _aiServiceMock = new();
    private readonly ChatHub _hub;

    public ChatHubTests()
    {
        _hub = new ChatHub(_aiServiceMock.Object, Mock.Of<ILogger<ChatHub>>());
    }

    [Fact]
    public async Task SendPrompt_ShouldStreamResponse()
    {
        // Arrange
        var context = new HubCallerContextMock();
        _hub.Context = context;

        var clients = new Mock<IHubCallerClients>();
        var callerMock = new Mock<IClientProxy>();
        clients.Setup(c => c.Caller).Returns(callerMock.Object);

        _hub.Clients = clients.Object;

        // Act
        await _hub.SendPrompt("session-1", "Hello");

        // Assert
        callerMock.Verify(c => c.SendCoreAsync(
            "ReceiveChunk", It.IsAny<object[]>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }
}
```

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Hub Count | 4 (Chat, Agent, Notification, Tool) |
| Message Types | 4 (Chunk, Status, Progress, Notification) |
| Reconnection Delays | 10 (1s → 60s exponential) |
| Scale-Out Options | 2 (Redis, Azure SignalR) |
| Auth Methods | 2 (JWT, Policy-based) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
