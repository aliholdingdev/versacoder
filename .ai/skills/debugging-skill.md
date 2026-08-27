---
title: "Versa Coder — Debugging Skill"
type: skill
category: debugging
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Debugging Skill

---

## 1. Amaç

Hata ayıklama ve sorun giderme görevleri için **özel skill**.

---

## 2. Debugging Akışı

```
Hata Tanımı → Kök Neden Analizi → Düzeltme → Test → Doğrulama
```

---

## 3. Kontrol Listesi

| # | Kontrol |
|---|---------|
| 1 | Hata mesajını analiz et |
| 2 | Stack trace'i incele |
| 3 | İlgili kodu bul |
| 4 | Reproduction adımlarını belirle |
| 5 | Kök nedeni tespit et |
| 6 | Düzeltme uygula |
| 7 | Regression testi yap |
| 8 | Log kaydını güncelle |

---

## 4. Hata Türleri

### 4.1 Compile-Time Hataları

```csharp
// CS0246: Type or namespace not found
// Çözüm: using ekle veya NuGet paketi kur
using VersaCoder.Domain.Entities;

// CS0103: Variable not defined
// Çözüm: Değişkeni tanımla veya yazım hatasını düzelt
var result = await _repository.GetByIdAsync(id);

// CS0029: Cannot implicitly convert type
// Çözüm: Dönüşüm operatörü ekle veya uygun tip kullan
var dto = entity.ToDto(); // Extension method kullan
```

### 4.2 Runtime Hataları

```csharp
// NullReferenceException
// Çözüm: Null check ekle veya ?? operatörünü kullan
var session = await _repository.GetByIdAsync(id);
if (session == null)
    throw new NotFoundException("Session", id);

// InvalidOperationException
// Çözüm: Validasyon ekle veya durumu kontrol et
if (!sessions.Any())
    throw new InvalidOperationException("No sessions available");

// TaskCanceledException
// Çözüm: CancellationToken'ı propagate et
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await _service.DoWorkAsync(cts.Token);
```

### 4.3 Asenkron Hataları

```csharp
// Deadlock
// Çözüm: Async all the way - .Result veya .Wait() kullanma
// ❌ Yanlış
var result = _repository.GetByIdAsync(id).Result;

// ✅ Doğru
var result = await _repository.GetByIdAsync(id);

// Race Condition
// Çözüm: SemaphoreSlim veya Lock kullan
private readonly SemaphoreSlim _semaphore = new(1, 1);

public async Task IncrementCounterAsync()
{
    await _semaphore.WaitAsync();
    try
    {
        _counter++;
    }
    finally
    {
        _semaphore.Release();
    }
}
```

---

## 5. Debugging Araçları

### 5.1 Visual Studio Debugging

```csharp
// Conditional breakpoint
// Debug → Windows → Breakpoints → New Breakpoint → Function Breakpoint
// Condition: session.Name == "Test Session"

// Data Tips
// Hover over variable to see value

// Watch Window
// Debug → Windows → Watch → Watch 1

// Call Stack
// Debug → Windows → Call Stack

// Locals
// Debug → Windows → Locals
```

### 5.2 Logging Debugging

```csharp
// Serilog structured logging
_logger.LogInformation("Processing request {RequestId}", requestId);
_logger.LogWarning("Slow query: {QueryTime}ms", queryTime);
_logger.LogError(ex, "Failed to process {Operation}", operation);

// Debug output
System.Diagnostics.Debug.WriteLine($"Debug: {variable}");

// Trace
System.Diagnostics.Trace.TraceInformation($"Trace: {variable}");
```

---

## 6. Kök Neden Analizi

### 6.1 5 Why Tekniği

```markdown
# Problem: Application crashes on startup

## Why 1: Why did it crash?
Because of NullReferenceException in SessionService

## Why 2: Why was SessionService null?
Because dependency injection failed to resolve it

## Why 3: Why did DI fail?
Because ISessionRepository was not registered

## Why 4: Why was it not registered?
Because we forgot to add it in Startup.cs

## Why 5: Why did we forget?
Because we don't have a checklist for DI registration

## Root Cause: Missing DI registration checklist
## Solution: Add DI registration to architecture tests
```

### 6.2 Fishbone Diagram

```markdown
# Problem: Application crashes on startup

## People
- New developer joined
- No documentation

## Process
- No DI checklist
- No architecture tests

## Technology
- Complex DI configuration
- Multiple assemblies

## Environment
- Development vs Production differences
- Missing configuration

## Root Cause: Inadequate onboarding process
## Solution: Create onboarding checklist and architecture tests
```

---

## 7. Debugging Stratejileri

### 7.1 Binary Search

```csharp
// Kodun ortasına breakpoint koyarak sorunun hangi tarafta olduğunu bulma
public void ProcessData(List<Item> items)
{
    // Breakpoint 1: Başlangıç
    var filtered = items.Where(i => i.IsValid).ToList();
    
    // Breakpoint 2: Orta
    var transformed = filtered.Select(i => i.Transform()).ToList();
    
    // Breakpoint 3: Son
    var result = transformed.Aggregate((a, b) => a.Merge(b));
}
```

### 7.2 Rubber Duck Debugging

```markdown
# Rubber Duck Debugging Adımları

1. Sorunu basitçe tanımla
2. Sorunu birine (ya daoplastik ördeğe) açıkla
3. Adım adım çözümü açıkla
4. Kendini dinlerken sorunu fark et
```

---

## 8. Debugging Testleri

### 8.1 Reproduction Test

```csharp
public class BugReproductionTests
{
    [Fact]
    public void Bug123_ShouldNotThrowNullReference()
    {
        // Arrange
        var service = new SessionService(
            Mock.Of<ISessionRepository>(),
            Mock.Of<ILogger<SessionService>>());
        
        // Act & Assert
        var exception = Record.Exception(() => 
            service.ProcessSession(null));
        
        Assert.Null(exception); // Should not throw
    }
    
    [Fact]
    public async Task Bug456_ShouldHandleCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Pre-cancel
        
        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _service.DoWorkAsync(cts.Token));
    }
}
```

---

## 9. Debugging Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Hata mesajını oku | ☐ |
| 2 | Stack trace'i incele | ☐ |
| 3 | Reproduction adımlarını belirle | ☐ |
| 4 | Breakpoint'leri ayarla | ☐ |
| 5 | Variable değerlerini kontrol et | ☐ |
| 6 | Kök nedeni tespit et | ☐ |
| 7 | Düzeltme uygula | ☐ |
| 8 | Regression testi yap | ☐ |
| 9 | Log kaydını güncelle | ☐ |
| 10 | Dokümantasyon güncelle | ☐ |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Error Types | 3 (Compile, Runtime, Async) |
| Debugging Tools | 2 (VS, Logging) |
| Analysis Techniques | 2 (5 Why, Fishbone) |
| Strategies | 2 (Binary Search, Rubber Duck) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
