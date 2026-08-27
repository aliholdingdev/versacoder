---
title: "Test Template"
type: template
category: csharp
version: 1.0.0
---

# Test Template

## Kullanım

Yeni bir test oluştururken bu template'i kullanın.

## Unit Test Template

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using {TargetNamespace};

namespace {TestNamespace}
{
    /// <summary>
    /// {ClassName} için unit test'ler
    /// </summary>
    public class {ClassName}Tests
    {
        #region Fields

        private readonly Mock<{DependencyType}> _mock{Dependency};
        private readonly {ClassName} _sut; // System Under Test

        #endregion

        #region Constructor

        public {ClassName}Tests()
        {
            _mock{Dependency} = new Mock<{DependencyType}>();
            _sut = new {ClassName}(_mock{Dependency}.Object);
        }

        #endregion

        #region Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldInitialize()
        {
            // Arrange & Act
            var sut = new {ClassName}(_mock{Dependency}.Object);

            // Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void Constructor_WithNullDependency_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new {ClassName}(null!));
        }

        [Fact]
        public async Task {MethodName}_WithValidInput_ShouldReturnExpectedResult()
        {
            // Arrange
            var input = new {InputType}();
            var expected = new {ReturnType}();

            _mock{Dependency}
                .Setup(x => x.{MethodAsync}(It.IsAny<{InputType}>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _sut.{MethodAsync}(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.{Property}, result.{Property});

            _mock{Dependency}
                .Verify(x => x.{MethodAsync}(It.IsAny<{InputType}>()), Times.Once);
        }

        [Fact]
        public async Task {MethodName}_WithInvalidInput_ShouldThrow{ExceptionType}()
        {
            // Arrange
            var input = new {InputType} { /* invalid data */ };

            _mock{Dependency}
                .Setup(x => x.{MethodAsync}(It.IsAny<{InputType}>()))
                .ThrowsAsync(new {ExceptionType}("Error message"));

            // Act & Assert
            await Assert.ThrowsAsync<{ExceptionType}>(
                () => _sut.{MethodAsync}(input));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task {MethodName}_WithInvalidString_ShouldThrowArgumentException(string invalidInput)
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.{MethodAsync}(invalidInput));
        }

        #endregion
    }
}
```

## Integration Test Template

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using {TargetNamespace};

namespace {IntegrationTestNamespace}
{
    /// <summary>
    /// {ClassName} için entegrasyon test'leri
    /// </summary>
    public class {ClassName}IntegrationTests : IDisposable
    {
        private readonly VersaCoderDbContext _context;
        private readonly {ClassName} _sut;

        public {ClassName}IntegrationTests()
        {
            var options = new DbContextOptionsBuilder<VersaCoderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VersaCoderDbContext(options);
            _sut = new {ClassName}(_context);
        }

        [Fact]
        public async Task {MethodAsync}_WithRealDatabase_ShouldPersistData()
        {
            // Arrange
            var entity = new {EntityName}
            {
                Id = Guid.NewGuid(),
                Name = "Test Entity"
            };

            // Act
            await _sut.AddAsync(entity);
            await _sut.SaveChangesAsync();

            // Assert
            var savedEntity = await _context.Set<{EntityName}>()
                .FindAsync(entity.Id);

            Assert.NotNull(savedEntity);
            Assert.Equal(entity.Name, savedEntity.Name);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
```

## Mock Örnekleri

```csharp
// Servis mock'u
var mockService = new Mock<IUserService>();
mockService
    .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
    .ReturnsAsync(new UserDto { Id = Guid.NewGuid(), Name = "Test User" });

// Repository mock'u
var mockRepo = new Mock<IUserRepository>();
mockRepo
    .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
    .ReturnsAsync(new List<User> { new User { Name = "Test" } });

// Dialog mock'u
var mockDialog = new Mock<IDialogService>();
mockDialog
    .Setup(x => x.ShowConfirmationAsync(It.IsAny<string>()))
    .ReturnsAsync(true);
```

---

## 4. Integration Test Template

### 4.1 API Integration Test

```csharp
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace {TestNamespace}
{
    /// <summary>
    /// API integration test'leri
    /// </summary>
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetSessions_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/sessions");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        [Fact]
        public async Task CreateSession_ReturnsCreated()
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
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetSession_NotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/sessions/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
```

---

## 5. Test Data Builders

### 5.1 Session Builder

```csharp
namespace {TestNamespace}
{
    /// <summary>
    /// Session test data builder
    /// </summary>
    public class SessionBuilder
    {
        private string _name = "Test Session";
        private Guid _projectId = Guid.NewGuid();
        private SessionState _state = SessionState.ACTIVE;
        private DateTime _createdAt = DateTime.UtcNow;

        public static SessionBuilder New() => new();

        public SessionBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public SessionBuilder WithProjectId(Guid projectId)
        {
            _projectId = projectId;
            return this;
        }

        public SessionBuilder WithState(SessionState state)
        {
            _state = state;
            return this;
        }

        public SessionBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public Session Build()
        {
            var session = new Session(_name, _projectId);
            return session;
        }
    }
}
```

### 5.2 User Builder

```csharp
namespace {TestNamespace}
{
    /// <summary>
    /// User test data builder
    /// </summary>
    public class UserBuilder
    {
        private string _name = "Test User";
        private string _email = "test@example.com";
        private bool _isVip = false;

        public static UserBuilder New() => new();

        public UserBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserBuilder AsVip()
        {
            _isVip = true;
            return this;
        }

        public User Build()
        {
            return new User
            {
                Name = _name,
                Email = _email,
                IsVip = _isVip
            };
        }
    }
}
```

---

## 6. Assertion Helpers

### 6.1 Custom Assertions

```csharp
namespace {TestNamespace}
{
    /// <summary>
    /// Özel assertion yardımcıları
    /// </summary>
    public static class AssertExtensions
    {
        public static void shouldBe(this string actual, string expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void shouldNotBeNull(this object? actual)
        {
            Assert.NotNull(actual);
        }

        public static void shouldBeTrue(this bool actual)
        {
            Assert.True(actual);
        }

        public static void shouldBeFalse(this bool actual)
        {
            Assert.False(actual);
        }

        public static void shouldBeGreaterThan(this int actual, int expected)
        {
            Assert.True(actual > expected);
        }
    }
}
```

---

## 7. Test Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Arrange kısmı açık | ☐ |
| 2 | Act kısmı basit | ☐ |
| 3 | Assert kısmı spesifik | ☐ |
| 4 | Test izole | ☐ |
| 5 | Test tekrarlanabilir | ☐ |
| 6 | Mock'lar doğru kullanıldı | ☐ |
| 7 | Edge case'ler test edildi | ☐ |
| 8 | Test ismi açıklayıcı | ☐ |

---

## 8. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Test Types | 3 (Unit, Integration, Builder) |
| Mock Examples | 3 |
| Builder Examples | 2 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
