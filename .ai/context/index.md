---
title: "Versa Coder — Context Management Index"
type: system
category: context
date: 2026-08-25
version: 1.0.0
---

# Versa Coder — Context Management Index

## Context Sources

| Dosya | Amaç |
|-------|------|
| context/sources/project-context.md | Proje bağlamı |
| context/sources/file-context.md | Dosya bağlamı |
| context/sources/session-context.md | Session bağlamı |
| context/sources/history-context.md | Geçmiş bağlamı |
| context/sources/skill-context.md | Skill bağlamı |
| context/sources/diagram-context.md | Diyagram bağlamı |
| context/sources/learning-context.md | Öğrenme bağlamı |
| context/sources/custom-context.md | Özel bağlam |

## Context Assembly

| Dosya | Amaç |
|-------|------|
| context/assembly/context-rules.md | Bağlam kuralları |
| context/assembly/context-priorities.md | Bağlam öncelikleri |

---

## 4. Context Assembly

### 4.1 Context Assembly Process

```csharp
public class ContextAssembler
{
    private readonly List<IContextSource> _sources;
    private readonly ILogger<ContextAssembler> _logger;
    
    public ContextAssembler(
        IEnumerable<IContextSource> sources,
        ILogger<ContextAssembler> logger)
    {
        _sources = sources.ToList();
        _logger = logger;
    }
    
    public async Task<AssembledContext> AssembleContextAsync(
        ContextRequest request,
        CancellationToken ct = default)
    {
        var context = new AssembledContext();
        
        foreach (var source in _sources.OrderByDescending(s => s.Priority))
        {
            try
            {
                var sourceContext = await source.GetContextAsync(request, ct);
                if (sourceContext != null)
                {
                    context.Merge(sourceContext);
                    _logger.LogDebug("Merged context from {Source}", source.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get context from {Source}", source.Name);
            }
        }
        
        return context;
    }
}
```

### 4.2 Context Priority Rules

| # | Öncelik | Kaynak | Açıklama |
|---|---------|--------|----------|
| 1 | Yüksek | Proje bağlamı | Proje yapısı ve ayarları |
| 2 | Yüksek | Dosya bağlamı | Açık dosya ve bağımlılıkları |
| 3 | Orta | Session bağlamı | Mevcut oturum geçmişi |
| 4 | Orta | Öğrenme bağlamı | Düzeltme ve kalıplar |
| 5 | Düşük | Diyagram bağlamı | Mimari diyagramlar |
| 6 | Düşük | Özel bağlam | Kullanıcı tanımlı bağlam |

---

## 5. Context Epochs

### 5.1 Epoch Tanımı

```csharp
public class ContextEpoch
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public List<ContextSnapshot> Snapshots { get; set; } = new();
    
    public TimeSpan Duration => (EndTime ?? DateTime.UtcNow) - StartTime;
    
    public bool IsActive => EndTime == null;
    
    public void End()
    {
        EndTime = DateTime.UtcNow;
    }
}
```

### 5.2 Epoch History

```csharp
public class EpochHistory
{
    private readonly List<ContextEpoch> _epochs;
    
    public EpochHistory()
    {
        _epochs = new List<ContextEpoch>();
    }
    
    public void StartNewEpoch(string name)
    {
        var currentEpoch = _epochs.FirstOrDefault(e => e.IsActive);
        if (currentEpoch != null)
        {
            currentEpoch.End();
        }
        
        _epochs.Add(new ContextEpoch
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartTime = DateTime.UtcNow
        });
    }
    
    public ContextEpoch? GetCurrentEpoch()
    {
        return _epochs.FirstOrDefault(e => e.IsActive);
    }
    
    public IReadOnlyList<ContextEpoch> GetEpochs()
    {
        return _epochs.AsReadOnly();
    }
}
```

---

## 6. Context Sources

### 6.1 Project Context

```csharp
public class ProjectContextSource : IContextSource
{
    public string Name => "Project";
    public int Priority => 100;
    
    private readonly IProjectRepository _projectRepository;
    
    public ProjectContextSource(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
    
    public async Task<ContextData?> GetContextAsync(
        ContextRequest request,
        CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, ct);
        if (project == null) return null;
        
        return new ContextData
        {
            Type = ContextType.Project,
            Data = new
            {
                project.Id,
                project.Name,
                project.Path,
                FileCount = project.Files.Count,
                LastModified = project.LastModifiedAt
            }
        };
    }
}
```

### 6.2 File Context

```csharp
public class FileContextSource : IContextSource
{
    public string Name => "File";
    public int Priority => 90;
    
    private readonly IFileService _fileService;
    
    public FileContextSource(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    public async Task<ContextData?> GetContextAsync(
        ContextRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.FilePath))
            return null;
        
        var fileInfo = await _fileService.GetFileInfoAsync(request.FilePath, ct);
        if (fileInfo == null) return null;
        
        return new ContextData
        {
            Type = ContextType.File,
            Data = new
            {
                fileInfo.Path,
                fileInfo.Name,
                fileInfo.Extension,
                fileInfo.Size,
                fileInfo.LastModified,
                Dependencies = fileInfo.Dependencies
            }
        };
    }
}
```

---

## 7. Context Rules

### 7.1 Context Assembly Rules

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Priority ordering | Yüksek öncelikli kaynaklar önce |
| 2 | Merge strategy | Son eklenen overwrite eder |
| 3 | Size limits | Maksimum boyut 10KB |
| 4 | Cache duration | 5 dakika cache |
| 5 | Error handling | Hatalı kaynak atlanır |

### 7.2 Context Usage Rules

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Relevance | İlgili bağlamı kullan |
| 2 | Freshness | Güncel bağlam tercih et |
| 3 | Completeness | Eksik bağlamı tamamla |
| 4 | Accuracy | Doğruluğunu kontrol et |
| 5 | Privacy | Gizli bilgileri koru |

---

## 8. Context Cache

### 8.1 Cache Implementation

```csharp
public class ContextCache
{
    private readonly Dictionary<string, CachedContext> _cache;
    private readonly TimeSpan _defaultExpiration;
    private readonly ILogger<ContextCache> _logger;
    
    public ContextCache(
        TimeSpan defaultExpiration,
        ILogger<ContextCache> logger)
    {
        _cache = new Dictionary<string, CachedContext>();
        _defaultExpiration = defaultExpiration;
        _logger = logger;
    }
    
    public async Task<ContextData?> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            if (cached.ExpirationTime > DateTime.UtcNow)
            {
                _logger.LogDebug("Cache hit for {Key}", key);
                return cached.Data;
            }
            
            _cache.Remove(key);
            _logger.LogDebug("Cache expired for {Key}", key);
        }
        
        return null;
    }
    
    public void Set(string key, ContextData data, TimeSpan? expiration = null)
    {
        var exp = expiration ?? _defaultExpiration;
        
        _cache[key] = new CachedContext
        {
            Data = data,
            ExpirationTime = DateTime.UtcNow.Add(exp)
        };
        
        _logger.LogDebug("Cached context for {Key}", key);
    }
    
    public void Clear()
    {
        _cache.Clear();
        _logger.LogInformation("Context cache cleared");
    }
}
```

---

## 9. Context Testing

### 9.1 Unit Tests

```csharp
public class ContextAssemblerTests
{
    private readonly Mock<IContextSource> _sourceMock;
    private readonly ContextAssembler _assembler;
    
    public ContextAssemblerTests()
    {
        _sourceMock = new Mock<IContextSource>();
        _sourceMock.Setup(s => s.Name).Returns("TestSource");
        _sourceMock.Setup(s => s.Priority).Returns(100);
        
        _assembler = new ContextAssembler(
            new[] { _sourceMock.Object },
            Mock.Of<ILogger<ContextAssembler>>());
    }
    
    [Fact]
    public async Task AssembleContext_WithValidSource_ReturnsContext()
    {
        // Arrange
        _sourceMock.Setup(s => s.GetContextAsync(
            It.IsAny<ContextRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContextData
            {
                Type = ContextType.Project,
                Data = new { Name = "Test" }
            });
        
        // Act
        var result = await _assembler.AssembleContextAsync(
            new ContextRequest(),
            CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
    }
}
```

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Context Sources | 5 |
| Context Rules | 10 |
| Cache Features | 3 |
| Test Types | 1 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
