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

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
