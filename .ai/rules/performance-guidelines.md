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

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
