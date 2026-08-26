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

## Ek Terimler (Devam)

### Repository Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Veri erişim soyutlaması |
| Kullanım | Domain ile veritabanı arasındaki köprü |
| Yönetim | Dependency Injection |
| Metotlar | GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync |

### Unit of Work

| Özellik | Tanım |
|---------|-------|
| Tanım | Birden fazla repository'yi tek transaction'da yönetme |
| Kullanım | Atomik işlemler |
| Yönetim | DbContext |
| Metotlar | SaveChangesAsync |

### Value Object

| Özellik | Tanım |
|---------|-------|
| Tanım | Değer tabanlı, immutable nesneler |
| Kullanım | Küçük, ölçülebilir değerler |
| Yönetim | Record struct veya class |
| Örnekler | Money, Address, Email |

### Domain Event

| Özellik | Tanım |
|---------|-------|
| Tanım | Domain'de gerçekleşen önemli olaylar |
| Kullanım | Asenkron iletişim |
| Yönetim | MediatR |
| Örnekler | SessionCreated, MessageSent |

### Anti-Corruption Layer

| Özellik | Tanım |
|---------|-------|
| Tanım | Dış sistemlerle iletişimi soyutlama katmanı |
| Kullanım | Eski sistem entegrasyonu |
| Yönetim | Adapter pattern |
| Kullanım Alanı | Infrastructure katmanı |

### Bounded Context

| Özellik | Tanım |
|---------|-------|
| Tanım | Domain'in sınırlı olduğu bağlam |
| Kullanım | Domain分割 |
| Yönetim | DDD |
| Örnekler | Chat, Session, User |

### Aggregate Root

| Özellik | Tanım |
|---------|-------|
| Tanım | Aggregate'in kök varlığı |
| Kullanım | Transaction kontrolü |
| Yönetim | Domain |
| Örnekler | Session (Message'ların root'u) |

### Domain Service

| Özellik | Tanım |
|---------|-------|
| Tanım | Domain mantığını içeren servis |
| Kullanım | Birden fazla entity gerektiren işlemler |
| Yönetim | Domain katmanı |
| Örnekler | PricingService, ValidationService |

### Application Service

| Özellik | Tanım |
|---------|-------|
| Tanım | Uygulama mantığını içeren servis |
| Kullanım | Use case'leri orkestre etme |
| Yönetim | Application katmanı |
| Örnekler | SessionService, ChatService |

### Infrastructure Service

| Özellik | Tanım |
|---------|-------|
| Tanım | Teknik altyapı servisleri |
| Kullanım | Dosya sistemi, ağ, veritabanı |
| Yönetim | Infrastructure katmanı |
| Örnekler | FileService, EmailService |

### Cross-Cutting Concern

| Özellik | Tanım |
|---------|-------|
| Tanım | Tüm katmanları etkileyen konular |
| Kullanım | Loglama, güvenlik, cache |
| Yönetim | MediatR Pipeline |
| Örnekler | LoggingBehavior, ValidationBehavior |

### Middleware

| Özellik | Tanım |
|---------|-------|
| Tanım | İstek/yanıt hattında çalışan bileşen |
| Kullanım | Önişleme, sonişleme |
| Yönetim | Pipeline |
| Örnekler | ErrorHandlingMiddleware, LoggingMiddleware |

### Decorator Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Mevcut nesneye davranış ekleme |
| Kullanım | Mevcut kodu değiştirmeden genişletme |
| Yönetim | DI Container |
| Örnekler | CachedRepository, LoggedRepository |

### Strategy Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Çalışma zamanında algoritma seçimi |
| Kullanım | Farklı algoritmalar |
| Yönetim | DI Container |
| Örnekler | Different LLM providers |

### Observer Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Olaylara tepki veren nesneler |
| Kullanım | Asenkron iletişim |
| Yönetim | Event Bus |
| Örnekler | Session events |

### Factory Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Nesne oluşturma soyutlaması |
| Kullanım | Karmaşık nesne oluşturma |
| Yönetim | DI Container |
| Örnekler | RepositoryFactory, ServiceFactory |

### Builder Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Adım adım nesne oluşturma |
| Kullanım | Karmaşık yapılandırma |
| Yönetim | Fluent API |
| Örnekler | QueryBuilder, ConfigurationBuilder |

### Pipeline Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | İşlemleri sıralı olarak zincirleme |
| Kullanım | İstek iş akışı |
| Yönetim | MediatR |
| Örnekler | Validation → Logging → Execution |

### Circuit Breaker

| Özellik | Tanım |
|---------|-------|
| Tanım | Hata durumunda çağrıyı kesme |
| Kullanım | Dayanıklılık |
| Yönetim | Polly |
| Örnekler | AI provider circuit breaker |

### Retry Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Başarısız çağrıyı tekrarlama |
| Kullanım | Geçici hatalar |
| Yönetim | Polly |
| Örnekler | Network retry, API retry |

### CQRS

| Özellik | Tanım |
|---------|-------|
| Tanım | Komut ve sorguları ayırma |
| Kullanım | Okuma/yazma optimizasyonu |
| Yönetim | MediatR |
| Componentler | Command, Query, Handler, Result |

### Event Sourcing

| Özellik | Tanım |
|---------|-------|
| Tanım | Olayları depolayarak durum yönetimi |
| Kullanım | Audit trail, time travel |
| Yönetim | Event Store |
| Benefit | Tam geçmiş |

### Saga Pattern

| Özellik | Tanım |
|---------|-------|
| Tanım | Dağıtılmış transaction yönetimi |
| Kullanım | Uzun süren işlemler |
| Yönetim | Choreography veya Orchestration |
| Örnekler | Multi-step AI workflow |

### Idempotency

| Özellik | Tanım |
|---------|-------|
| Tanım | Aynı isteğin tekrar tekrar gönderilmesinin etkisi |
| Kullanım | API güvenliği |
| Yönetim | Idempotency key |
| Örnek | Payment processing |

### Backpressure

| Özellik | Tanım |
|---------|-------|
| Tanım | Yoğun yük altında sistemi koruma |
| Kullanım | Load management |
| Yönetim | Rate limiting |
| Örnek | API rate limiting |

### Graceful Degradation

| Özellik | Tanım |
|---------|-------|
| Tanım | Hata durumunda düşüş gösterme |
| Kullanım | Sistem dayanıklılığı |
| Yönetim | Fallback strategy |
| Örnek | AI provider fallback |

### Observability

| Özellik | Tanım |
|---------|-------|
| Tanım | Sistem iç görürlüğü |
| Kullanım | Monitoring, debugging |
| Yönetim | Logging, Metrics, Tracing |
| Componentler | Logs, Metrics, Traces |

### Telemetry

| Özellik | Tanım |
|---------|-------|
| Tanım | Sistem verilerini toplama |
| Kullanım | Performans izleme |
| Yönetim | OpenTelemetry |
| Componentler | Logs, Metrics, Traces |

### Health Check

| Özellik | Tanım |
|---------|-------|
| Tanım | Sistem sağlığını kontrol etme |
| Kullanım | Awake monitoring |
| Yönetim | ASP.NET Health Checks |
| Durumlar | Healthy, Degraded, Unhealthy |

### Readiness Probe

| Özellik | Tanım |
|---------|-------|
| Tanım | Sistem trafiğe hazır mı kontrolü |
| Kullanım | Kubernetes |
| Yönetim | Startup check |
| Timing | Başlangıç sonrası |

### Liveness Probe

| Özellik | Tanım |
|---------|-------|
| Tanım | Sistem hala çalışıyor mu kontrolü |
| Kullanım | Kubernetes |
| Yönetim | Periodic check |
| Timing | Düzenli aralıklarla |

---

## Terim İlişkileri

### Katman İlişkileri

```
L0 Domain ← L1 Abstractions ← L2 Application
                                    ↓
                              L3 CrossCutting
                                    ↓
                    L4 Infrastructure ← L5 Protocol
                                    ↓
                              L6 Host → L7 UI
```

### Agent İlişkileri

```
MO (Master Orchestrator)
├── Build Agent (Kod)
├── Plan Agent (Planlama)
├── Explore Agent (Analiz)
├── General Agent (Genel)
├── Summary Agent (Doküman)
└── Title Agent (İsimlendirme)
```

### Teknoloji İlişkileri

```
DevExpress (UI) → CommunityToolkit.Mvvm (MVVM)
    ↓
MediatR (CQRS) → FluentValidation (Doğrulama)
    ↓
EF Core (ORM) → SQLite (DB) → WAL Mode
    ↓
Serilog (Log) → Polly (Dayanıklılık)
```

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.2.0 |
| Status | Active |
| Total Terms | 60+ |
| Categories | 4 |
| Detailed Terms | 20+ |
| Relationships | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26