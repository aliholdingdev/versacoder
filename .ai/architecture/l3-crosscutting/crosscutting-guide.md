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

| Behavior | Dosya | Tanım | Satır |
|----------|-------|-------|-------|
| `LoggingBehavior<TRequest,TResponse>` | `Behaviors/LoggingBehavior.cs` | Her handler öncesi/sonrası log | 29 |
| `PerformanceBehavior<TRequest,TResponse>` | `Behaviors/PerformanceBehavior.cs` | 500ms üzeri yavaş handler uyarı | — |
| `ValidationBehavior<TRequest,TResponse>` | `Behaviors/ValidationBehavior.cs` | FluentValidation doğrulama | 43 |

---

## 3. Exception Tipleri

| Exception | Dosya | Tanım |
|-----------|-------|-------|
| `DomainException` | `Exceptions/DomainException.cs` | Domain kural ihlali |
| `NotFoundException` | `Exceptions/NotFoundException.cs` | Kaynak bulunamadı |
| `ValidationException` | `Exceptions/ValidationException.cs` | Validasyon hatası |
| `GlobalExceptionHandler` | `Exceptions/GlobalExceptionHandler.cs` | Merkezi hata yönetimi |

---

## 4. Pipeline Akışı

```
Request
  → LoggingBehavior (log yaz)
    → PerformanceBehavior (süre ölç)
      → ValidationBehavior (FluentValidation kontrol)
        → Handler (iş mantığı)
          → Response
```

---

## 5. Hata Hiyerarşisi

```
VersaCoderException (Base)
  ├── DomainException
  │     ├── ValidationException
  │     ├── NotFoundException
  │     └── DuplicateException
  ├── InfrastructureException
  │     ├── DatabaseException
  │     ├── ProviderException
  │     └── NetworkException
  └── ProtocolException
        ├── MCPException
        └── AgentException
```

---

## 6. Logging Stratejisi

| Level | Kullanım | Örnek |
|-------|----------|-------|
| Verbose | Detaylı debug | Variable values |
| Debug | Geliştirme bilgisi | Method entry/exit |
| Information | Normal olaylar | Request completed |
| Warning | Uyarılar | Slow query |
| Error | Hatalar | Exception thrown |
| Fatal | Kritik hatalar | System crash |

---

## 7. Kurallar

| # | Kural |
|---|-------|
| 1 | CrossCutting, Application'a bağımlı (L3 → L2 ✅) |
| 2 | CrossCutting, Domain'e bağımlı DEĞİL (L3 → L0 ❌) |
| 3 | Tüm handler'lar pipeline behaviors'lardan geçer |
| 4 | Hatalar merkezi olarak yönetilir |
| 5 | Structured logging zorunlu (Serilog) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
