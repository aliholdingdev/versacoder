---
title: "Versa Coder — Testing Skill"
type: skill
category: testing
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Testing Skill

---

## 1. Amaç

Test yazma ve çalıştırma görevleri için **özel skill**.

---

## 2. Test Türleri

| Tür | Kütüphane | Kullanım |
|-----|-----------|----------|
| Unit Test | xUnit | Tek başına testler |
| Integration Test | xUnit + TestServer | Servis entegrasyonu |
| E2E Test | xUnit + Selenium | UI testleri |

---

## 3. Test Pattern'ı (AAA)

```csharp
[Fact]
public async Task CreateSession_ShouldReturnSuccess()
{
    // Arrange
    var command = new CreateSessionCommand { Title = "Test" };
    var handler = new CreateSessionHandler(mockRepository);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
}
```

---

## 4. Test Kuralları

| # | Kural |
|---|-------|
| 1 | Minimum %80 code coverage |
| 2 | Her handler için test |
| 3 | Edge case'leri test et |
| 4 | Mock kullan (Moq) |
| 5 | Arrange-Act-Assert pattern |

---

## 5. Unit Test Örnekleri

### 5.1 Handler Test

```csharp
public class CreateSessionHandlerTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateSessionHandler>> _loggerMock;
    private readonly CreateSessionHandler _handler;
    
    public CreateSessionHandlerTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateSessionHandler>>();
        _handler = new CreateSessionHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateSessionCommand("Test Session", Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_EmptyName_ReturnsFailure()
    {
        // Arrange
        var command = new CreateSessionCommand("", Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("name", result.Error, StringComparison.OrdinalIgnoreCase);
    }
    
    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsFailure()
    {
        // Arrange
        var command = new CreateSessionCommand("Test Session", Guid.NewGuid());
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Database error", result.Error);
    }
}
```

### 5.2 Repository Test

```csharp
public class SessionRepositoryTests : IDisposable
{
    private readonly VersaCoderDbContext _context;
    private readonly SessionRepository _repository;
    
    public SessionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VersaCoderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new VersaCoderDbContext(options);
        _repository = new SessionRepository(_context);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddSession()
    {
        // Arrange
        var session = new Session("Test Session", Guid.NewGuid());
        
        // Act
        await _repository.AddAsync(session);
        await _context.SaveChangesAsync();
        
        // Assert
        var result = await _repository.GetByIdAsync(session.Id);
        Assert.NotNull(result);
        Assert.Equal("Test Session", result.Name);
    }
    
    [Fact]
    public async Task GetByIdAsync_ExistingSession_ReturnsSession()
    {
        // Arrange
        var session = new Session("Test Session", Guid.NewGuid());
        await _repository.AddAsync(session);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetByIdAsync(session.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(session.Id, result.Id);
    }
    
    [Fact]
    public async Task GetByIdAsync_NonExistingSession_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateSession()
    {
        // Arrange
        var session = new Session("Original Name", Guid.NewGuid());
        await _repository.AddAsync(session);
        await _context.SaveChangesAsync();
        
        // Act
        session.UpdateName("Updated Name");
        _repository.Update(session);
        await _context.SaveChangesAsync();
        
        // Assert
        var result = await _repository.GetByIdAsync(session.Id);
        Assert.Equal("Updated Name", result.Name);
    }
    
    [Fact]
    public async Task RemoveAsync_ShouldRemoveSession()
    {
        // Arrange
        var session = new Session("Test Session", Guid.NewGuid());
        await _repository.AddAsync(session);
        await _context.SaveChangesAsync();
        
        // Act
        _repository.Remove(session);
        await _context.SaveChangesAsync();
        
        // Assert
        var result = await _repository.GetByIdAsync(session.Id);
        Assert.Null(result);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

---

## 6. Integration Test

### 6.1 API Integration Test

```csharp
public class SessionApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public SessionApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateSession_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateSessionCommand("Test Session", Guid.NewGuid());
        var content = new StringContent(
            JsonSerializer.Serialize(command),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/sessions", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetSession_ReturnsSuccess()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/api/sessions/{sessionId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

---

## 7. Mocking

### 7.1 Moq Examples

```csharp
// Basic mock
var repositoryMock = new Mock<ISessionRepository>();
repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(new Session("Test", Guid.NewGuid()));

// Mock with verification
repositoryMock.Verify(r => r.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);

// Mock with callback
repositoryMock.Setup(r => r.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
    .Callback<Session, CancellationToken>((session, ct) => 
    {
        // Perform additional checks
        Assert.NotNull(session.Name);
    })
    .ReturnsAsync(new Session("Test", Guid.NewGuid()));

// Mock exception
repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
    .ThrowsAsync(new Exception("Database error"));
```

---

## 8. Test Coverage

### 8.1 Coverage Report

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutput=../coverage/ /p:CoverletOutputFormat=cobertura

# View coverage report
reportgenerator -reports:coverage/**/coverage.cobertura.xml -targetdir:coverage/report
```

### 8.2 Coverage Targets

| Metric | Target |
|--------|--------|
| Line coverage | >= 80% |
| Branch coverage | >= 70% |
| Method coverage | >= 90% |

---

## 9. Testing Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Unit testler yazıldı | ☐ |
| 2 | Integration testler yazıldı | ☐ |
| 3 | Edge case'ler test edildi | ☐ |
| 4 | Error cases test edildi | ☐ |
| 5 | Mock'lar doğru kullanıldı | ☐ |
| 6 | AAA pattern uygulandı | ☐ |
| 7 | Code coverage >= 80% | ☐ |
| 8 | Testler çalışıyor | ☐ |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Test Types | 3 (Unit, Integration, E2E) |
| Test Patterns | 1 (AAA) |
| Coverage Targets | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
