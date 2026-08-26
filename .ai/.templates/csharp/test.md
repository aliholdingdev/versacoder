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

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
