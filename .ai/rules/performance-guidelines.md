---
title: "Versa Coder — Performans Rehberi"
type: rules
category: performance
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Performans Rehberi

---

## 1. Performans Hedefleri

| Metric | Hedef | Eşik |
|--------|-------|------|
| Uygulama başlatma | < 3s | 5s |
| UI yanıt süresi | < 100ms | 500ms |
| LLM yanıt süresi | < 30s | 60s |
| Dosya okuma | < 100ms | 500ms |
| Veritabanı sorgusu | < 50ms | 200ms |
| Memory kullanımı | < 500MB | 1GB |

---

## 2. Optimizasyon Kuralları

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Async/await | Tüm I/O işlemleri asenkron |
| 2 | Connection pooling | SQLite WAL + connection pool |
| 3 | Lazy loading | Gerektiğinde yükle |
| 4 | Caching | Semantic cache ile tekrar sorguları önbelleğe alma |
| 5 | Chunked processing | Büyük dosyalar için parçalı işleme |
| 6 | Streaming | LLM yanıtlarında streaming kullanımı |

---

## 3. Bellek Yönetimi

| Kural | Açıklama |
|-------|----------|
| IDisposable | Tüm IDisposable nesneler using ile |
| Large object heap | 85KB+ nesneler LOH'e dikkat |
| GC optimization | GC.Collect() manuel çağırmak yasak |
| Memory leak | Timer, event handler dikkatli yönetim |

---

## 4. Asenkron Programlama

### 4.1 Async/Await Kuralları

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

// ❌ Yanlış - .Wait() usage
public SessionDto GetSession(Guid sessionId)
{
    var session = _repository.GetByIdAsync(sessionId).Wait(); // ❌ Blocking!
    return session?.ToDto();
}
```

### 4.2 CancellationToken Kullanımı

```csharp
// ✅ Doğru - CancellationToken propagation
public async Task<List<Session>> GetSessionsAsync(
    CancellationToken ct = default)
{
    return await _context.Sessions
        .AsNoTracking()
        .ToListAsync(ct);
}

// ✅ Doğru - CancellationToken with timeout
public async Task<Result<SessionDto>> GetSessionWithTimeoutAsync(
    Guid sessionId, 
    CancellationToken ct = default)
{
    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
        ct, timeoutCts.Token);
    
    try
    {
        var session = await _repository.GetByIdAsync(sessionId, linkedCts.Token);
        return Result<SessionDto>.Success(session?.ToDto());
    }
    catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
    {
        return Result<SessionDto>.Failure("Request timed out");
    }
}
```

---

## 5. Veritabanı Optimizasyonu

### 5.1 Query Optimizasyonu

```csharp
// ✅ Doğru - AsNoTracking for read-only queries
public async Task<List<SessionDto>> GetSessionsAsync(CancellationToken ct = default)
{
    return await _context.Sessions
        .AsNoTracking()
        .Select(s => new SessionDto
        {
            Id = s.Id,
            Name = s.Name,
            CreatedAt = s.CreatedAt
        })
        .ToListAsync(ct);
}

// ❌ Yanlış - Tracking for read-only
public async Task<List<SessionDto>> GetSessionsAsync(CancellationToken ct = default)
{
    return await _context.Sessions
        .Include(s => s.Messages) // ❌ Unnecessary include
        .ToListAsync(ct) // ❌ Tracking enabled
        .Select(s => s.ToDto()); // ❌ Client-side evaluation
}
```

### 5.2 Pagination

```csharp
// ✅ Doğru - Server-side pagination
public async Task<PaginatedList<SessionDto>> GetSessionsAsync(
    int pageNumber, 
    int pageSize, 
    CancellationToken ct = default)
{
    var query = _context.Sessions
        .AsNoTracking()
        .OrderByDescending(s => s.CreatedAt);
    
    var totalCount = await query.CountAsync(ct);
    
    var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(s => new SessionDto
        {
            Id = s.Id,
            Name = s.Name,
            CreatedAt = s.CreatedAt
        })
        .ToListAsync(ct);
    
    return new PaginatedList<SessionDto>(items, totalCount, pageNumber, pageSize);
}
```

---

## 6. Bellek Optimizasyonu

### 6.1 StringBuilder Kullanımı

```csharp
// ✅ Doğru - StringBuilder for large string operations
public string BuildLargeString(List<string> items)
{
    var sb = new StringBuilder();
    
    foreach (var item in items)
    {
        sb.AppendLine(item);
    }
    
    return sb.ToString();
}

// ❌ Yanlış - String concatenation
public string BuildLargeString(List<string> items)
{
    var result = string.Empty; // ❌ Creates new string each iteration
    
    foreach (var item in items)
    {
        result += item + Environment.NewLine; // ❌ Memory inefficient
    }
    
    return result;
}
```

### 6.2 Object Pooling

```csharp
// ✅ Doğru - Object pooling for expensive objects
public class PooledHttpClientFactory
{
    private readonly ObjectPool<HttpClient> _pool;
    
    public PooledHttpClientFactory(ObjectPool<HttpClient> pool)
    {
        _pool = pool;
    }
    
    public async Task<string> GetStringAsync(string url)
    {
        var client = _pool.Get();
        try
        {
            return await client.GetStringAsync(url);
        }
        finally
        {
            _pool.Return(client);
        }
    }
}
```

---

## 7. UI Performansı

### 7.1 Virtual Mode for Large Lists

```csharp
// ✅ Doğru - Virtual mode for large datasets
public class VirtualSessionList : VirtualServerModeInstantThreadSafeSession
{
    private readonly List<SessionDto> _sessions;
    
    public VirtualSessionList(List<SessionDto> sessions)
    {
        _sessions = sessions;
    }
    
    public override int SessionCount => _sessions.Count;
    
    public override object GetSessionValue(int index)
    {
        return _sessions[index];
    }
}

// ❌ Yanlış - Loading all items at once
public void LoadSessions(List<SessionDto> sessions)
{
    foreach (var session in sessions)
    {
        listBoxControl.Items.Add(session); // ❌ Performance issue
    }
}
```

### 7.2 Async UI Updates

```csharp
// ✅ Doğru - Async UI updates
public partial class MainForm : RibbonForm
{
    private async void MainForm_Load(object sender, EventArgs e)
    {
        await LoadSessionsAsync();
    }
    
    private async Task LoadSessionsAsync()
    {
        try
        {
            loadingPanel.Visible = true;
            
            var sessions = await Task.Run(() => _sessionRepository.GetAllAsync());
            
            sessionsBindingSource.DataSource = sessions;
        }
        finally
        {
            loadingPanel.Visible = false;
        }
    }
}
```

---

## 8. Monitoring ve Metrics

### 8.1 Performance Counter

```csharp
public class PerformanceMetrics
{
    private readonly Counter _requestCounter;
    private readonly Histogram _requestDuration;
    
    public PerformanceMetrics(IMetrics metrics)
    {
        _requestCounter = metrics.CreateCounter("versacoder_requests_total");
        _requestDuration = metrics.CreateHistogram("versacoder_request_duration_seconds");
    }
    
    public void RecordRequest(string endpoint, double durationSeconds)
    {
        _requestCounter.Increment(1, new KeyValuePair<string, object?>("endpoint", endpoint));
        _requestDuration.Observe(durationSeconds, new KeyValuePair<string, object?>("endpoint", endpoint));
    }
}
```

---

## 9. Performance Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Async/await kullanımı | ☐ |
| 2 | CancellationToken propagation | ☐ |
| 3 | AsNoTracking for read-only | ☐ |
| 4 | Pagination for large datasets | ☐ |
| 5 | StringBuilder for large strings | ☐ |
| 6 | Object pooling | ☐ |
| 7 | Virtual mode for large lists | ☐ |
| 8 | Async UI updates | ☐ |
| 9 | Performance monitoring | ☐ |
| 10 | Memory leak detection | ☐ |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Performance Targets | 6 |
| Optimization Rules | 6 |
| Memory Rules | 4 |
| Monitoring Metrics | 2 |

---

## 18. Async/Await Performans Kalıpları

### 18.1 ConfigureAwait Kullanımı

```csharp
// ✅ Doğru — Library code'da ConfigureAwait(false)
public class RepositoryService
{
    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            .ConfigureAwait(false);
    }
}

// ✅ Doğru — UI code'da ConfigureAwait(true) veya default
private async void OnButtonClick(object sender, EventArgs e)
{
    var data = await _service.GetDataAsync(); // UI thread'e dön
    _label.Text = data.ToString();
}
```

### 18.2 Parallel.ForEachAsync Kullanımı

```csharp
// ✅ Doğru — Paralel processing
public async Task ProcessItemsAsync(List<WorkItem> items, CancellationToken ct)
{
    await Parallel.ForEachAsync(items,
        new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
        async (item, token) =>
        {
            await ProcessSingleItemAsync(item, token);
        });
}

// ❌ Yanlış — Sequential processing
public async Task ProcessItemsAsync(List<WorkItem> items, CancellationToken ct)
{
    foreach (var item in items)
    {
        await ProcessSingleItemAsync(item, ct); // Yavaş!
    }
}
```

### 18.3 ValueTask Kullanımı

```csharp
// ✅ Doğru — Cached/inline dönüşlerde ValueTask
public class CachedDataService
{
    private readonly ConcurrentDictionary<string, CachedItem> _cache = new();

    public ValueTask<CachedItem?> GetCachedItemAsync(string key, CancellationToken ct)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            return new ValueTask<CachedItem?>(cached); // Senkron dönüş
        }

        return new ValueTask<CachedItem?>(LoadFromDbAsync(key, ct)); // Async dönüş
    }
}

// ❌ Yanlış — Her zaman async
public async Task<CachedItem?> GetCachedItemAsync(string key, CancellationToken ct)
{
    if (_cache.TryGetValue(key, out var cached))
        return cached; // Gereksiz async state machine
    return await LoadFromDbAsync(key, ct);
}
```

---

## 19. Bellek Yönetimi

### 19.1 Span<T> ve Memory<T> Kullanımı

```csharp
// ✅ Doğru — Heap allocation azaltma
public static int CountWords(ReadOnlySpan<char> text)
{
    int count = 0;
    int index = 0;
    while (index < text.Length)
    {
        // Skip whitespace
        while (index < text.Length && char.IsWhiteSpace(text[index]))
            index++;

        if (index < text.Length)
        {
            count++;
            while (index < text.Length && !char.IsWhiteSpace(text[index]))
                index++;
        }
    }
    return count;
}

// ✅ Doğru — Stack allocation
public static bool IsValidIp(ReadOnlySpan<char> input)
{
    Span<Range> ranges = stackalloc Range[4];
    return input.Split(ranges, '.').Length == 4;
}
```

### 19.2 ArrayPool Kullanımı

```csharp
// ✅ Doğru — Döngüsel buffer kullanımı
public class DataProcessor
{
    public async Task ProcessLargeFileAsync(string filePath, CancellationToken ct)
    {
        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(8192);

        try
        {
            await using var stream = File.OpenRead(filePath);
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
            {
                ProcessBuffer(buffer.AsSpan(0, bytesRead));
            }
        }
        finally
        {
            pool.Return(buffer);
        }
    }
}
```

### 19.3 Object Pooling (DI Container)

```csharp
// ✅ Doğru — Pooled services
builder.Services.AddObjectPool<PooledHttpClient>(options =>
{
    options.Size = 10;
});

// veya IMemoryCache kullanımı
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
    options.CompactionPercentage = 0.25;
});
```

### 19.4 String Interning

```csharp
// ✅ Doğru — Sık kullanılan string'ler için interning
private static class CommonStrings
{
    public static readonly string Active = string.Intern("Active");
    public static readonly string Completed = string.Intern("Completed");
    public static readonly string Deleted = string.Intern("Deleted");
}

// ✅ Doğru — StringBuilder ile büyük string oluşturma
public string BuildLargeString(IEnumerable<string> items)
{
    var sb = new StringBuilder(1024); // Initial capacity
    foreach (var item in items)
    {
        sb.AppendLine(item);
    }
    return sb.ToString();
}
```

---

## 20. Database Performans Optimizasyonları

### 20.1 AsNoTracking Kullanımı

```csharp
// ✅ Doğru — Read-only sorgularda AsNoTracking
public async Task<List<SessionDto>> GetAllSessionsAsync(CancellationToken ct)
{
    return await _context.Sessions
        .AsNoTracking() // Change tracking yok, ~%30 hızlı
        .Select(s => new SessionDto
        {
            Id = s.Id,
            Name = s.Name,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        })
        .ToListAsync(ct);
}

// ❌ Yanlış — Change tracking gerekmediği halde kullanılmamış
public async Task<List<SessionDto>> GetAllSessionsAsync(CancellationToken ct)
{
    return await _context.Sessions
        .Select(s => new SessionDto { ... })
        .ToListAsync(ct); // Change tracking overhead
}
```

### 20.2 İndeks Stratejisi

```csharp
// ✅ Doğru — EF Core ile indeks tanımlama
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Session>(entity =>
    {
        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => new { e.ProjectId, e.Status });
        entity.HasIndex(e => e.Name).IsFullText();
    });

    modelBuilder.Entity<Message>(entity =>
    {
        entity.HasIndex(e => new { e.SessionId, e.Timestamp });
        entity.HasIndex(e => e.Content).IsFullText();
    });
}
```

### 20.3 compiled Query Kullanımı

```csharp
// ✅ Doğru — Sık kullanılan sorgular için compiled query
public static class SessionQueries
{
    public static readonly Func<VersaCoderDbContext, Guid, Task<Session?>> GetById =
        EF.CompileAsyncQuery((VersaCoderDbContext context, Guid id) =>
            context.Sessions.FirstOrDefault(s => s.Id == id));

    public static readonly Func<VersaCoderDbContext, Guid, IQueryable<Message>> GetMessages =
        EF.CompileQuery((VersaCoderDbContext context, Guid sessionId) =>
            context.Messages.Where(m => m.SessionId == sessionId));
}

// Kullanım
var session = await SessionQueries.GetById(_context, sessionId);
```

### 20.4 Bulk Operations

```csharp
// ✅ Doğru — Bulk insert/update
public async Task BulkInsertMessagesAsync(List<Message> messages, CancellationToken ct)
{
    await _context.BulkInsertAsync(messages, new BulkConfig
    {
        SetOutputIdentity = false,
        BatchSize = 1000
    }, cancellationToken: ct);
}

// ✅ Doğru — ExecuteUpdate ile toplu güncelleme
public async Task ArchiveOldSessionsAsync(DateTime cutoffDate, CancellationToken ct)
{
    await _context.Sessions
        .Where(s => s.CreatedAt < cutoffDate && s.Status == SessionStatus.Completed)
        .ExecuteUpdateAsync(s => s
            .SetProperty(x => x.Status, SessionStatus.Archived)
            .SetProperty(x => x.ArchivedAt, DateTime.UtcNow),
        ct);
}
```

---

## 21. Caching Stratejileri

### 21.1 Multi-Level Caching

```csharp
// ✅ Doğru — L1 (Memory) + L2 (Distributed) caching
public class CachedSessionService : ISessionService
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly ISessionRepository _repository;

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        // L1 Cache (Memory - 5 dk TTL)
        var cacheKey = $"session:{id}";
        if (_l1Cache.TryGetValue(cacheKey, out Session? cached))
            return cached;

        // L2 Cache (Distributed - 15 dk TTL)
        var l2Data = await _l2Cache.GetStringAsync(cacheKey, ct);
        if (l2Data is not null)
        {
            var session = JsonSerializer.Deserialize<Session>(l2Data);
            _l1Cache.Set(cacheKey, session, TimeSpan.FromMinutes(5));
            return session;
        }

        // Database fallback
        var dbSession = await _repository.GetByIdAsync(id, ct);
        if (dbSession is not null)
        {
            var json = JsonSerializer.Serialize(dbSession);
            await _l2Cache.SetStringAsync(cacheKey, json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) }, ct);
            _l1Cache.Set(cacheKey, dbSession, TimeSpan.FromMinutes(5));
        }

        return dbSession;
    }
}
```

### 21.2 Cache Invalidation Patterns

```csharp
// ✅ Doğru — Tag-based cache invalidation
public async Task UpdateSessionAsync(Session session, CancellationToken ct)
{
    await _repository.UpdateAsync(session, ct);

    // İlgili cache'leri temizle
    var cacheKey = $"session:{session.Id}";
    _l1Cache.Remove(cacheKey);
    await _l2Cache.RemoveAsync(cacheKey, ct);

    // Liste cache'lerini de temizle
    _l1Cache.Remove($"sessions:project:{session.ProjectId}");
    await _l2Cache.RemoveAsync($"sessions:project:{session.ProjectId}", ct);
}
```

---

## 22. Connection Pooling & Resource Management

### 22.1 HttpClient Factory Kullanımı

```csharp
// ✅ Doğru — HttpClientFactory ile pooled connections
builder.Services.AddHttpClient<IAiProvider, OpenAiProvider>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", configuration["OpenAI:ApiKey"]);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    MaxConnectionsPerServer = 10
});

// ❌ Yanlış — Doğrudan HttpClient kullanımı (socket exhaustion)
public class BadAiProvider
{
    private readonly HttpClient _client = new(); // Socket exhaustion!
}
```

### 22.2 SemaphoreSlim ile Eşzamanlılık Kontrolü

```csharp
// ✅ Doğru — Rate limiting için SemaphoreSlim
public class ThrottledAiProvider : IAiProvider
{
    private readonly SemaphoreSlim _semaphore = new(10, 10); // Max 10 eşzamanlı

    public async Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            return await _innerProvider.CompleteAsync(request, ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

---

## 23. Benchmark & Profiling

### 23.1 BenchmarkDotNet Kullanımı

```csharp
// ✅ Doğru — Performans benchmark'ı
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class SerializationBenchmark
{
    private readonly Session _session = CreateTestSession();

    [Benchmark(Baseline = true)]
    public string SystemTextJson() => JsonSerializer.Serialize(_session);

    [Benchmark]
    public string NewtonsoftJson() => JsonConvert.SerializeObject(_session);

    [Benchmark]
    public string MessagePack() => MessagePackSerializer.SerializeToJson(_session);
}
```

### 23.2 Performance Counter'lar

```csharp
// ✅ Doğru — Custom performance counters
public class PerformanceMetrics
{
    private long _totalRequests;
    private long _totalErrors;
    private readonly ConcurrentDictionary<string, long> _operationCounts = new();

    public void RecordRequest(string operation)
    {
        Interlocked.Increment(ref _totalRequests);
        _operationCounts.AddOrUpdate(operation, 1, (_, count) => count + 1);
    }

    public void RecordError() => Interlocked.Increment(ref _totalErrors);

    public double ErrorRate => _totalRequests > 0
        ? (double)_totalErrors / _totalRequests * 100
        : 0;
}
```

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Enhanced |
| Async Patterns | 3 (ConfigureAwait, ValueTask, Parallel) |
| Memory Patterns | 4 (Span, ArrayPool, Pooling, Interning) |
| DB Optimization | 4 (AsNoTracking, Index, CompiledQuery, Bulk) |
| Caching Patterns | 2 (Multi-Level, Tag-based) |
| Resource Management | 2 (HttpClientFactory, SemaphoreSlim) |
| Benchmark Patterns | 2 (BenchmarkDotNet, Counters) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
