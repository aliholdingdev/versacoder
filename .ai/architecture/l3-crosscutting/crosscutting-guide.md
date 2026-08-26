---
title: "Versa Coder — L3 CrossCutting Layer Guide"
type: architecture
category: layer
layer: L3
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L3 CrossCutting Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/l2-application/application-guide]] · [[brain.md]]

---

## 1. Amaç

CrossCutting katmanı, uygulama genelinde **logging, exception handling ve validation** gibi kesen endişeleri (cross-cutting concerns) yönetir.

---

## 2. MediatR Pipeline Behaviors

| Behavior | Dosya | Tanım |
|----------|-------|-------|
| `LoggingBehavior<TRequest,TResponse>` | `VersaCoder.CrossCutting/Behaviors/LoggingBehavior.cs` | Her handler öncesi/sonrası log |
| `PerformanceBehavior<TRequest,TResponse>` | `VersaCoder.CrossCutting/Behaviors/PerformanceBehavior.cs` | 500ms üzeri yavaş handler uyarı |
| `ValidationBehavior<TRequest,TResponse>` | `VersaCoder.CrossCutting/Behaviors/ValidationBehavior.cs` | FluentValidation doğrulama |

---

## 3. Exception Tipleri

| Exception | Dosya | Tanım |
|-----------|-------|-------|
| `DomainException` | `VersaCoder.CrossCutting/Exceptions/DomainException.cs` | Domain kural ihlali |
| `NotFoundException` | `VersaCoder.CrossCutting/Exceptions/NotFoundException.cs` | Kaynak bulunamadı |
| `ValidationException` | `VersaCoder.CrossCutting/Exceptions/ValidationException.cs` | Validasyon hatası |
| `GlobalExceptionHandler` | `VersaCoder.CrossCutting/Exceptions/GlobalExceptionHandler.cs` | Merkezi hata yönetimi |

---

## 4. Pipeline Akışı

```
Request → LoggingBehavior → PerformanceBehavior → ValidationBehavior → Handler → Response
```

---

## 5. Kurallar

| # | Kural |
|---|-------|
| 1 | CrossCutting, Application'a bağımlı (L3 → L2 ✅) |
| 2 | CrossCutting, Domain'e bağımlı DEĞİL (L3 → L0 ❌) |
| 3 | Tüm handler'lar pipeline behaviors'lardan geçer |
| 4 | Hatalar merkezi olarak yönetilir |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
