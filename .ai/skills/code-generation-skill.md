---
title: "Versa Coder — Code Generation Skill"
type: skill
category: code-generation
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Code Generation Skill

---

## 1. Amaç

Kod üretimi görevleri için **özel skill**.

---

## 2. Kullanım Senaryoları

| Senaryo | Komut |
|---------|-------|
| Yeni entity oluştur | `/skill code-gen entity [isim]` |
| Yeni handler oluştur | `/skill code-gen handler [command]` |
| Yeni repository oluştur | `/skill code-gen repo [entity]` |
| Yeni test oluştur | `/skill code-gen test [sınıf]` |

---

## 3. Kod Üretim Kuralları

| # | Kural |
|---|-------|
| 1 | Template uyumluluğu zorunlu |
| 2 | Naming convention'a uy |
| 3 | Layer bağımlılıklarına dikkat |
| 4 | Validation ekle |
| 5 | XML doc comment ekle |

---

## 4. Entity Oluşturma

### 4.1 Entity Template

```csharp
// Entity template
public class {EntityName}
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Constructor
    public {EntityName}(string name)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CreatedAt = DateTime.UtcNow;
    }
    
    // Private constructor for EF Core
    private {EntityName() { }
    
    // Methods
    public void UpdateName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### 4.2 Entity Factory

```csharp
public static class {EntityName}Factory
{
    public static {EntityName} Create(string name)
    {
        return new {EntityName}(name);
    }
    
    public static {EntityName} CreateWithValidation(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(name));
        
        return new {EntityName}(name);
    }
}
```

---

## 5. Handler Oluşturma

### 5.1 Command Handler Template

```csharp
public class {CommandName}Handler : IRequestHandler<{CommandName}, Result<{ResponseDto}>>
{
    private readonly IRepository<{EntityName}> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<{CommandName}Handler> _logger;
    
    public {CommandName}Handler(
        IRepository<{EntityName}> repository,
        IUnitOfWork unitOfWork,
        ILogger<{CommandName}Handler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<{ResponseDto}>> Handle(
        {CommandName} request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Business logic here
            
            var entity = new {EntityName}(request.Name);
            
            await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("{EntityName} created: {Id}", typeof({EntityName}).Name, entity.Id);
            
            return Result<{ResponseDto}>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle {CommandName}", typeof({CommandName}).Name);
            return Result<{ResponseDto}>.Failure(ex.Message);
        }
    }
}
```

### 5.2 Query Handler Template

```csharp
public class {QueryName}Handler : IRequestHandler<{QueryName}, Result<{ResponseDto}>>
{
    private readonly IRepository<{EntityName}> _repository;
    private readonly ILogger<{QueryName}Handler> _logger;
    
    public {QueryName}Handler(
        IRepository<{EntityName}> repository,
        ILogger<{QueryName}Handler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<{ResponseDto}>> Handle(
        {QueryName} request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            
            if (entity == null)
                return Result<{ResponseDto}>.Failure($"{typeof({EntityName}).Name} not found");
            
            return Result<{ResponseDto}>.Success(entity.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle {QueryName}", typeof({QueryName}).Name);
            return Result<{ResponseDto}>.Failure(ex.Message);
        }
    }
}
```

---

## 6. Repository Oluşturma

### 6.1 Repository Template

```csharp
public class {EntityName}Repository : Repository<{EntityName}>, I{EntityName}Repository
{
    public {EntityName}Repository(VersaCoderDbContext context) : base(context) { }
    
    public async Task<{EntityName}?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Name == name, ct);
    }
    
    public async Task<IReadOnlyList<{EntityName}>> GetByCriteriaAsync(
        string? nameFilter,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();
        
        if (!string.IsNullOrEmpty(nameFilter))
        {
            query = query.Where(e => e.Name.Contains(nameFilter));
        }
        
        if (startDate.HasValue)
        {
            query = query.Where(e => e.CreatedAt >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(e => e.CreatedAt <= endDate.Value);
        }
        
        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);
    }
}
```

---

## 7. Test Oluşturma

### 7.1 Unit Test Template

```csharp
public class {ClassName}Tests
{
    private readonly Mock<IRepository<{EntityName}>> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<{ClassName}>> _loggerMock;
    private readonly {ClassName} _sut;
    
    public {ClassName}Tests()
    {
        _repositoryMock = new Mock<IRepository<{EntityName}>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<{ClassName}>>();
        _sut = new {ClassName}(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new {CommandName}("Test Name");
        
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<{EntityName}>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var request = new {CommandName}("Test Name");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<{EntityName}>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Database error", result.Error);
    }
}
```

---

## 8. DTO Oluşturma

### 8.1 DTO Template

```csharp
// Response DTO
public record {EntityName}Dto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

// Request DTO
public record Create{EntityName}Dto
{
    public string Name { get; init; } = string.Empty;
}

// Update DTO
public record Update{EntityName}Dto
{
    public string Name { get; init; } = string.Empty;
}

// Mapping
public static class {EntityName}Mappings
{
    public static {EntityName}Dto ToDto(this {EntityName} entity)
    {
        return new {EntityName}Dto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
    
    public static {EntityName} ToEntity(this Create{EntityName}Dto dto)
    {
        return new {EntityName}(dto.Name);
    }
}
```

---

## 9. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Templates | 5 (Entity, Handler, Repository, Test, DTO) |
| Code Patterns | 10+ |
| Naming Conventions | 5 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
