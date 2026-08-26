---
title: "Versa Coder — Teknik Terimler Sözlüğü"
type: reference
category: glossary
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Teknik Terimler Sözlüğü

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[index.md]]

---

## A

| Terim | Tanım |
|-------|-------|
| **ADR** | Architecture Decision Record — Mimari karar kaydı |
| **Agent** | Belirli bir alanda uzmanlaşmış AI birimi |
| **AI** | Artificial Intelligence — Yapay zeka |
| **Assembly** | Derlenmiş .NET kodu (.dll, .exe) |
| **Async/Await** | Asenkron programlama deseni |

## B

| Terim | Tanım |
|-------|-------|
| **Bağımlılık Tersi** | Dependency Inversion — High-level modüller low-level modüllere bağımlı olmaz |
| **Build** | Derleme süreci |

## C

| Terim | Tanım |
|-------|-------|
| **Clean Architecture** | Katmanlı mimari yapısı (Uncle Bob) |
| **Context** | AI'ın çalışma bağlamı |
| **CQRS** | Command Query Responsibility Segregation |
| **CRUD** | Create, Read, Update, Delete |

## D

| Terim | Tanım |
|-------|-------|
| **DDD** | Domain-Driven Design — Alan odaklı tasarım |
| **DI** | Dependency Injection — Bağımlılık enjeksiyonu |
| **DLL** | Dynamic Link Library — Dinamik bağlantı kütüphanesi |
| **DTO** | Data Transfer Object — Veri transfer nesnesi |

## E

| Terim | Tanım |
|-------|-------|
| **EF Core** | Entity Framework Core — ORM |
| **Entity** | Varlık nesnesi |

## F

| Terim | Tanım |
|-------|-------|
| **Fallback** | Hata durumunda yedek seçeneğe geçiş |
| **FluentValidation** | Kural tabanlı validasyon kütüphanesi |

## G

| Terim | Tanım |
|-------|-------|
| **Guard Clause** | Erken dönüş kalıbı |

## H

| Terim | Tanım |
|-------|-------|
| **Handover** | Görev transferi |

## I

| Terim | Tanım |
|-------|-------|
| **ISP** | Interface Segregation Principle |
| **IoC** | Inversion of Control — Kontrol tersi |

## L

| Terim | Tanım |
|-------|-------|
| **LLM** | Large Language Model — Büyük dil modeli |
| **LINQ** | Language Integrated Query |

## M

| Terim | Tanım |
|-------|-------|
| **MCP** | Model Context Protocol |
| **MediatR** | Mediator pattern implementasyonu |
| **MO** | Master Orchestrator |
| **MVVM** | Model-View-ViewModel |

## P

| Terim | Tanım |
|-------|-------|
| **Pipeline** | İşlem hattı |
| **Provider** | LLM sağlayıcı |

## R

| Terim | Tanım |
|-------|-------|
| **Repository** | Veri erişim kalıbı |
| **Result Monad** | Success/Failure dönüşüm deseni |

## S

| Terim | Tanım |
|-------|-------|
| **SOLID** | 5 nesne yönelimli tasarım ilkesi |
| **SSOT** | Single Source of Truth |
| **Session** | Oturum |
| **Streaming** | Gerçek zamanlı veri akışı |

## T

| Terim | Tanım |
|-------|-------|
| **Tool** | AI'ın kullanabileceği araç |
| **Template** | Şablon |

## V

| Terim | Tanım |
|-------|-------|
| **Value Object** | Değer nesnesi (immutable) |
| **Vault** | Bilgi depolama sistemi |

## W

| Terim | Tanım |
|-------|-------|
| **WAL** | Write-Ahead Logging |

---

## Ek Terimler

| Terim | Tanım |
|-------|-------|
| **Agent Pool** | Tüm aktif ajanların bulunduğu havuz |
| **Tool Registry** | Tüm araçların kayıtlı olduğu defter |
| **Event Bus** | Ajanlar arası iletişim mekanizması |
| **Context Lock** | Eşzamanlı dosya erişimini önlemek için kilitleme |
| **Health Check** | Ajanların çalışma durumunu kontrol eden mekanizma |
| **Task Queue** | Görevlerin öncelik sırasıyla beklediği kuyruk |
| **Domain Boundary** | Her ajanın yalnızca kendi alanında çalışması kuralı |
| **Eskalasyon** | Bir sorunun çözülemediği durumda daha üst seviyeye çıkması |

---

## Terim Detayları

### Agent Pool

| Özellik | Tanım |
|---------|-------|
| Tanım | Tüm aktif ajanların bulunduğu havuz |
| Kullanım | Görev dağıtımı |
| Yönetim | Master Orchestrator |
| Durum | Active, Paused, Failed |

### Tool Registry

| Özellik | Tanım |
|---------|-------|
| Tanım | Tüm araçların kayıtlı olduğu defter |
| Kullanım | Tool seçimi ve çağrısı |
| Yönetim | Tool system |
| Kategori | Dosya, Terminal, Git, Test, AI, MCP |

### Event Bus

| Özellik | Tanım |
|---------|-------|
| Tanım | Ajanlar arası iletişim mekanizması |
| Kullanım | Asenkron iletişim |
| Yönetim | Event system |
| Event türleri | task.created, task.completed, handover.requested |

### Context Lock

| Özellik | Tanım |
|---------|-------|
| Tanım | Eşzamanlı dosya erişimini önlemek için kilitleme |
| Kullanım | Dosya koruması |
| Yönetim | Lock manager |
| Süre | Max 30 saniye |

### Health Check

| Özellik | Tanım |
|---------|-------|
| Tanım | Ajanların çalışma durumunu kontrol eden mekanizma |
| Kullanım | Sağlık izleme |
| Yönetim | Health system |
| Durumlar | Healthy, Degraded, Failed |

### Task Queue

| Özellik | Tanım |
|---------|-------|
| Tanım | Görevlerin öncelik sırasıyla beklediği kuyruk |
| Kullanım | Görev yönetimi |
| Yönetim | Queue manager |
| Öncelik | CRITICAL, HIGH, MEDIUM, LOW |

### Domain Boundary

| Özellik | Tanım |
|---------|-------|
| Tanım | Her ajanın yalnızca kendi alanında çalışması kuralı |
| Kullanım | Yetki kontrolü |
| Yönetim | Agent system |
| İhlal | Layer Violation |

### Eskalasyon

| Özellik | Tanım |
|---------|-------|
| Tanım | Bir sorunun çözülemediği durumda daha üst seviyeye çıkması |
| Kullanım | Sorun çözme |
| Yönetim | Escalation system |
| Seviyeler | L1, L2, L3, L4 |

---

## Terim Kategorileri

### Mimari Terimler

| Terim | Kategori |
|-------|----------|
| Clean Architecture | Mimari |
| DDD | Mimari |
| CQRS | Mimari |
| SOLID | Mimari |
| Layer Violation | Mimari |

### Teknoloji Terimleri

| Terim | Kategori |
|-------|----------|
| EF Core | Teknoloji |
| SQLite | Teknoloji |
| DevExpress | Teknoloji |
| LibGit2Sharp | Teknoloji |
| Serilog | Teknoloji |

### AI Terimleri

| Terim | Kategori |
|-------|----------|
| LLM | AI |
| MCP | AI |
| Agent | AI |
| Tool | AI |
| Context | AI |

### Süreç Terimleri

| Terim | Kategori |
|-------|----------|
| Workflow | Süreç |
| Handover | Süreç |
| Eskalasyon | Süreç |
| ADR | Süreç |
| Session | Süreç |

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Active |
| Total Terms | 30+ |
| Categories | 4 |
| Detailed Terms | 8 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25