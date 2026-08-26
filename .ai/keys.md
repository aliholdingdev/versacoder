---
title: "Versa Coder — Keyword Haritası"
type: reference
category: navigation
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Keyword Haritası

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[index.md]]

---

## 1. Amaç

Bu dosya, AI ajanlarının hangi keyword'leri hangi vault dosyalarına yönlendireceğini gösteren **keyword → dosya eşleme haritasıdır**.

---

## 2. Keyword Kategorileri

### 2.1 Mimari & Yapı

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| mimari, architecture, katman, layer | [[architecture/00-overview/architecture-master]] | Ana mimari plan |
| L0, domain, varlık, entity | [[architecture/l0-domain/domain-guide]] | Domain katmanı |
| L1, abstractions, arayüz | [[architecture/l1-abstractions/abstractions-guide]] | Abstractions katmanı |
| L2, application, use case | [[architecture/l2-application/application-guide]] | Application katmanı |
| L3, crosscutting | [[architecture/l3-crosscutting/crosscutting-guide]] | CrossCutting katmanı |
| L4, infrastructure | [[architecture/l4-infrastructure/infrastructure-guide]] | Infrastructure katmanı |
| L5, protocol, MCP | [[architecture/l5-protocol/protocol-guide]] | Protocol katmanı |
| L6, host, DI | [[architecture/l6-host/host-guide]] | Host katmanı |
| L7, UI, DevExpress | [[architecture/l7-ui/ui-guide]] | UI katmanı |

### 2.2 AI & Provider

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| provider, LLM, OpenAI, Anthropic | [[architecture/l4-infrastructure/ai/provider-router]] | Provider routing |
| agent, runner, orkestrasyon | [[architecture/l4-infrastructure/ai/agent-runner]] | Agent runner |
| tool, araç, 45+ | [[architecture/l4-infrastructure/ai/tool-system]] | Tool sistemi |
| AI, yapay zeka, model | [[CLAUDE.md]] §6 | AI provider mimarisi |

### 2.3 Agent Sistemi

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| build, kod, yaz, oluştur | [[.agents/build-agent]] | Build Agent |
| plan, planla, tasarla | [[.agents/plan-agent]] | Plan Agent |
| explore, analiz, tara | [[.agents/explore-agent]] | Explore Agent |
| general, genel | [[.agents/general-agent]] | General Agent |
| summary, özet | [[.agents/summary-agent]] | Summary Agent |
| title, başlık, isim | [[.agents/title-agent]] | Title Agent |
| MO, master, orkestratör | [[.agents/master-orchestrator]] | Master Orchestrator |

### 2.4 Veritabanı & Data

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| database, veritabanı, SQLite | [[architecture/l4-infrastructure/data/database-schema]] | DB şeması |
| EF Core, entity, migration | [[architecture/l4-infrastructure/data/database-schema]] | EF config |
| repository, depo | [[architecture/l4-infrastructure/infrastructure-guide]] | Repository pattern |

### 2.5 Süreç & Workflow

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| workflow, süreç, prosedür | [[WORKFLOW.md]] | Tüm süreçler |
| code review, inceleme | [[WORKFLOW.md]] §5.1 | Code review workflow |
| bug fix, hata düzeltme | [[WORKFLOW.md]] §5.2 | Bug fix workflow |
| feature, özellik | [[WORKFLOW.md]] §5.3 | New feature workflow |
| session, oturum | [[WORKFLOW.md]] §5.4 | Session init |
| vault sync, senkronizasyon | [[WORKFLOW.md]] §5.5 | Vault sync |

### 2.6 Güvenlik & Kural

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| security, güvenlik | [[rules/security-architecture]] | Güvenlik mimarisi |
| coding standard, kod standartı | [[rules/coding-standards]] | Kod standartları |
| performance, performans | [[rules/performance-guidelines]] | Performans |
| deployment, dağıtım | [[rules/deployment-guide]] | Dağıtım rehberi |
| plugin, eklenti | [[rules/plugin-development]] | Plugin geliştirme |
| MCP, protocol | [[rules/mcp-integration]] | MCP entegrasyonu |

### 2.7 Skill & Şablon

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| skill, beceri | [[skills/]] | Skill listesi |
| template, şablon | [[.templates/index]] | Template kataloğu |
|ADR, karar | [[decisions/adr-template]] | ADR şablonu |

---

## 3. Hızlı Erişim Tablosu

| İhtiyaç | Keyword Örnekleri | İlk Tıklama |
|---------|-------------------|-------------|
| Yeni dosya oluştur | "oluştur", "yaz", "class" | [[.agents/build-agent]] |
| Mimari planla | "plan", "mimari", "tasarım" | [[.agents/plan-agent]] |
| Kod analiz et | "analiz", "tara", "bul" | [[.agents/explore-agent]] |
| Doküman yaz | "doc", "özet", "markdown" | [[.agents/summary-agent]] |
| İsim bul | "isim", "naming", "başlık" | [[.agents/title-agent]] |
| Hata düzelt | "bug", "hata", "fix" | [[WORKFLOW.md]] §5.2 |
| Test yaz | "test", "xUnit" | [[skills/testing-skill]] |
| Güvenlik kontrol | "security", "güvenlik" | [[rules/security-architecture]] |

---

## 4. Dosya Yapısı Haritası

### 4.1 Vault Dizin Yapısı

```
.ai/
├── CLAUDE.md                    # AI Anayasası (700+ satır)
├── AGENTS.md                    # Agent kayıt defteri
├── WORKFLOW.md                  # Mühendislik süreçleri
├── brain.md                     # Mimari kararlar
├── ROLE.md                      # Rol tanımları
├── index.md                     # Ana katalog
├── keys.md                      # Bu dosya (keyword haritası)
├── MEMORY.md                    # Session hafızası
├── glossary.md                  # Teknik terimler
├── engine.md                    # Orkestrasyon motoru
├── ULTRA-THINKING.md            # Düşünme protokolü
├── log.md                       # İşlem logları
├── vault-summary.md             # Vault özet bilgisi
│
├── .agents/                     # Agent profilleri
│   ├── AGENTS.md               # Agent indeksi
│   ├── master-orchestrator.md  # MO profili
│   ├── build-agent.md          # Build profili
│   ├── plan-agent.md           # Plan profili
│   ├── explore-agent.md        # Explore profili
│   ├── general-agent.md        # General profili
│   ├── summary-agent.md        # Summary profili
│   └── title-agent.md          # Title profili
│
├── architecture/                # Mimari rehberler
│   ├── 00-overview/            # Genel bakış
│   │   ├── architecture-master.md
│   │   └── architecture-detailed.md
│   ├── l0-domain/              # Domain katmanı
│   ├── l1-abstractions/        # Abstractions katmanı
│   ├── l2-application/         # Application katmanı
│   ├── l3-crosscutting/        # CrossCutting katmanı
│   ├── l4-infrastructure/      # Infrastructure katmanı
│   ├── l5-protocol/            # Protocol katmanı
│   ├── l6-host/                # Host katmanı
│   └── l7-ui/                  # UI katmanı
│
├── decisions/                   # Mimari kararlar
│   ├── adr-template.md         # ADR şablonu
│   └── accepted/               # Kabul edilmiş ADR'ler
│
├── rules/                       # Kurallar
│   ├── coding-standards.md     # Kod standartları
│   ├── security-architecture.md # Güvenlik
│   ├── performance-guidelines.md # Performans
│   ├── deployment-guide.md     # Dağıtım
│   ├── plugin-development.md   # Plugin geliştirme
│   └── mcp-integration.md      # MCP entegrasyonu
│
├── skills/                      # Yetenekler
│   ├── index.md                # Skill indeksi
│   ├── code-generation-skill.md
│   ├── testing-skill.md
│   ├── debugging-skill.md
│   ├── refactoring-skill.md
│   ├── documentation-skill.md
│   └── architecture-skill.md
│
├── .templates/                  # Şablonlar
│   ├── index.md                # Template indeksi
│   └── csharp/                 # C# şablonları
│       ├── entity.md
│       ├── repository.md
│       ├── viewmodel.md
│       ├── test.md
│       └── index.md
│
├── context/                     # Bağlam yönetimi
│   ├── index.md
│   ├── assembly/               # Bağlam toplama
│   ├── epochs/                 # Dönemler
│   └── sources/                # Bağlam kaynakları
│
├── learning/                    # Öğrenme sistemi
│   ├── index.md
│   ├── corrections/            # Düzeltmeler
│   ├── knowledge/              # Bilgi birikimi
│   ├── patterns/               # Tasarım kalıpları
│   └── rules/                  # Öğrenilen kurallar
│
├── memory/                      # Hafıza
│   └── sessions/               # Session logları
│
└── project/                     # Proje bilgisi
    └── index.md
```

### 4.2 Kaynak Kod Yapısı

```
src/
├── VersaCoder.Domain/           # L0 - Domain (~800 satır)
│   ├── Entities/               # Varlıklar
│   ├── ValueObjects/           # Değer nesneleri
│   ├── Events/                 # Domain olayları
│   ├── Interfaces/             # Domain arayüzleri
│   └── Exceptions/             # Domain istisnaları
│
├── VersaCoder.Abstractions/     # L1 - Arayüzler (~600 satır)
│   ├── Services/               # Servis arayüzleri
│   ├── Repositories/           # Depo arayüzleri
│   ├── Providers/              # Sağlayıcı arayüzleri
│   └── DTOs/                   # Veri transfer nesneleri
│
├── VersaCoder.Application/      # L2 - Uygulama (~2500 satır)
│   ├── Services/               # Uygulama servisleri
│   ├── Commands/               # CQRS komutları
│   ├── Handlers/               # Komut işleyicileri
│   ├── Queries/                # CQRS sorguları
│   ├── Validators/             # Doğrulama kuralları
│   └── DTOs/                   # Uygulama DTO'ları
│
├── VersaCoder.CrossCutting/     # L3 - Kesişim (~200 satır)
│   ├── Behaviors/              # MediatR davranışları
│   ├── Middleware/              # Middleware'ler
│   └── Interceptors/           # Arounder'lar
│
├── VersaCoder.Infrastructure.Data/      # L4.1 - Veri (~1200 satır)
│   ├── Context/                # DbContext
│   ├── Repositories/           # Repository implementasyonları
│   ├── Configurations/         # EF yapılandırmaları
│   └── Migrations/             # EF migrasyonları
│
├── VersaCoder.Infrastructure.AI/        # L4.2 - AI (~800 satır)
│   ├── Providers/              # AI sağlayıcıları
│   ├── Runner/                 # Agent çalıştırıcı
│   └── Tools/                  # Araç sistemi
│
├── VersaCoder.Infrastructure.Logging/   # L4.28 - Loglama (~275 satır)
├── VersaCoder.Infrastructure.Reporting/ # L4.29 - Raporlama (~310 satır)
├── VersaCoder.Infrastructure.Config/    # L4.5 - Yapılandırma
├── VersaCoder.Infrastructure.FileSystem/ # L4.10 - Dosya sistemi
├── VersaCoder.Infrastructure.Auth/      # L4.4 - Kimlik doğrulama
├── VersaCoder.Infrastructure.Security/  # L4.12 - Güvenlik
└── VersaCoder.Host/            # L6 - Ana bilgisayar (~65 satır)
    ├── Program.cs
    └── Startup.cs
```

---

## 5. Agent → Dosya Yönlendirme Matrisi

### 5.1 Build Agent için

| Görev | Hedef Dosya | Kaynak |
|-------|-------------|--------|
| Yeni entity oluştur | `src/VersaCoder.Domain/Entities/` | `.templates/csharp/entity.md` |
| Repository oluştur | `src/VersaCoder.Abstractions/Repositories/` | `.templates/csharp/repository.md` |
| Handler yaz | `src/VersaCoder.Application/Handlers/` | `brain.md` §7 |
| ViewModel oluştur | `src/VersaCoder.UI/ViewModels/` | `.templates/csharp/viewmodel.md` |
| Test yaz | `tests/` | `.templates/csharp/test.md` |
| Migration oluştur | `src/VersaCoder.Infrastructure.Data/Migrations/` | `architecture/l4-infrastructure/data/database-schema.md` |

### 5.2 Plan Agent için

| Görev | Hedef Dosya | Kaynak |
|-------|-------------|--------|
| Mimari plan | `architecture/` | `architecture/00-overview/architecture-master.md` |
| Task dağıtımı | `project-plan.md` | `brain.md` §15 |
| Phase planla | `project-plan.md` §FAZ | `CLAUDE.md` §19 |
| ADR yaz | `decisions/accepted/` | `decisions/adr-template.md` |

### 5.3 Explore Agent için

| Görev | Hedef Dosya | Kaynak |
|-------|-------------|--------|
| Kod analizi | `src/` | Grep + Glob |
| Bağımlılık analizi | `*.csproj` | Project referansları |
| Vault analizi | `.ai/` | Tüm vault dosyaları |
| Metrik toplama | `src/` + `tests/` | Statik analiz |

### 5.4 Summary Agent için

| Görev | Hedef Dosya | Kaynak |
|-------|-------------|--------|
| API doküman | `docs/api/` | XML doc comments |
| README | `README.md` | Proje yapısı |
| Changelog | `CHANGELOG.md` | Git log |
| Vault doküman | `.ai/*.md` | Vault dosyaları |

### 5.5 Title Agent için

| Görev | Hedef Dosya | Kaynak |
|-------|-------------|--------|
| Class ismi | `*.cs` dosyaları | `rules/coding-standards.md` |
| Method ismi | `*.cs` dosyaları | `CLAUDE.md` §13 |
| Property ismi | `*.cs` dosyaları | `CLAUDE.md` §13 |
| Dosya ismi | Tüm dosyalar | `CLAUDE.md` §13.2 |

---

## 6. Sorun Çözme Haritası

### 6.1 Sık Karşılaşılan Sorunlar

| Sorun | Olası Neden | Çözüm | Hedef Dosya |
|-------|-------------|-------|-------------|
| Build hatası | Package reference eksik | csproj kontrol | `*.csproj` |
| Import hatası | Namespace yanlış | using kontrol | `*.cs` |
| Null reference | Dependency injection eksik | DI kontrol | `Startup.cs` |
| DB hatası | Migration eksik | EF migration | `Migrations/` |
| AI timeout | Provider ayarı | Config kontrol | `AiSettings` |
| UI donması | Async/Await eksik | Async düzeltme | `*.cs` |
| Test başarısız | Mock eksik | Test düzeltme | `tests/` |
| Vault bozulması | Eksik dosya | Vault sync | `.ai/` |

### 6.2 Hata Kodu → Dosya Eşleme

| Hata Kodu | Açıklama | Kontrol Dosyası |
|-----------|----------|-----------------|
| DOM-001 | Entity eksik | `Domain/Entities/` |
| APP-001 | Handler eksik | `Application/Handlers/` |
| INF-001 | Repository eksik | `Infrastructure.Data/Repositories/` |
| DB-001 | Migration eksik | `Infrastructure.Data/Migrations/` |
| AI-001 | Provider yapılandırması | `Infrastructure.AI/Providers/` |
| SEC-001 | Güvenlik açığı | `rules/security-architecture.md` |
| UI-001 | ViewModel eksik | `UI/ViewModels/` |

---

## 7. Hızlı Komutlar

### 7.1 Yaygın Kullanılan Komutlar

| Komut | Amaç | Kullanım |
|-------|------|----------|
| `dotnet build` | Proje derleme | Build Agent |
| `dotnet test` | Test çalıştırma | Build Agent |
| `dotnet run` | Uygulama çalıştırma | Host |
| `dotnet ef migrations add` | Migration oluşturma | Build Agent |
| `dotnet ef database update` | Migration uygulama | Build Agent |
| `dotnet format` | Kod biçimlendirme | Build Agent |

### 7.2 Vault Komutları

| Komut | Amaç | Kullanım |
|-------|------|----------|
| Vault load | Dosyaları oku | Tüm agentlar |
| Vault sync | Dosyaları güncelle | MO |
| Vault stats | İstatistikleri göster | MO |
| Session save | Session kaydet | MO |
| Session load | Session yükle | MO |

---

## 8. Versiyon & Güncelleme

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-25 | İlk sürüm, temel keyword haritası |
| 1.1.0 | 2026-08-26 | Enhanced - Dosya yapısı, agent yönlendirme, sorun çözüm haritası eklendi |

---

## 9. Entegrasyon Kalıpları

### 9.1 Yaygın Kullanılan Entegrasyonlar

| Entegrasyon | Kaynak | Hedef | Kalıp |
|-------------|--------|-------|-------|
| Domain → Abstractions | L0 | L1 | Interface extraction |
| Abstractions → Application | L1 | L2 | DI injection |
| Application → Infrastructure | L2 | L4 | Repository pattern |
| Infrastructure → Data | L4.1 | DB | DbContext |
| Infrastructure → AI | L4.2 | AI Provider | Provider pattern |
| CrossCutting → Application | L3 | L2 | MediatR pipeline |
| Host → Tümü | L6 | L0-L5 | DI composition |

### 9.2 Dependency Injection Kalıbı

```csharp
// Startup.cs'de DI kayıtları
public void ConfigureServices(IServiceCollection services)
{
    // Domain (L0) - Genellikle inject edilmez
    // Abstractions (L1) - Interface'ler
    services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
    services.AddScoped<IChatSessionService, ChatSessionService>();
    
    // Application (L2) - Handler'lar
    services.AddScoped<IRequestHandler<CreateSessionCommand, CreateSessionResponse>,
        CreateSessionHandler>();
    
    // Infrastructure (L4) - Implementasyonlar
    services.AddScoped<DbContext, VersaCoderDbContext>();
    
    // CrossCutting (L3) - Pipeline behaviors
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
}
```

### 9.3 Repository Kalıbı

```csharp
// Interface (L1 - Abstractions)
public interface IChatSessionRepository
{
    Task<ChatSession?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ChatSession>> GetAllAsync(CancellationToken ct);
    Task AddAsync(ChatSession entity, CancellationToken ct);
    Task UpdateAsync(ChatSession entity, CancellationToken ct);
    Task DeleteAsync(ChatSession entity, CancellationToken ct);
}

// Implementasyon (L4.1 - Infrastructure.Data)
public class ChatSessionRepository : IChatSessionRepository
{
    private readonly VersaCoderDbContext _context;

    public ChatSessionRepository(VersaCoderDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSession?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.ChatSessions
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    // ... diğer metodlar
}
```

---

## 10. Workflow Kısayolları

### 10.1 Görev Başlatma

| Adım | Aksiyon | Dosya |
|------|---------|-------|
| 1 | Vault yükle | `CLAUDE.md` → `AGENTS.md` → `WORKFLOW.md` |
| 2 | Son session'ı oku | `MEMORY.md` |
| 3 | Proje durumunu kontrol et | `brain.md` §15 |
| 4 | Kullanıcı isteğini analiz et | Keyword çıkarma |
| 5 | Uygun agent'ı seç | `AGENTS.md` §6 |
| 6 | Görevi başlat | Seçilen agent |

### 10.2 Kod Yazma Akışı

| Adım | Aksiyon | Kaynak |
|------|---------|--------|
| 1 | Şablonu yükle | `.templates/csharp/` |
| 2 | Entity/VO oluştur | `Domain/Entities/` |
| 3 | Interface tanımla | `Abstractions/Repositories/` |
| 4 | Repository implement et | `Infrastructure.Data/Repositories/` |
| 5 | Service oluştur | `Application/Services/` |
| 6 | Handler yaz | `Application/Handlers/` |
| 7 | Test yaz | `tests/` |
| 8 | Build ve test çalıştır | `dotnet build && dotnet test` |

### 10.3 Migration Akışı

| Adım | Aksiyon | Komut |
|------|---------|-------|
| 1 | Entity değişikliğini kontrol et | `Domain/Entities/` |
| 2 | DbContext'i güncelle | `Infrastructure.Data/Context/` |
| 3 | Migration oluştur | `dotnet ef migrations add {Name}` |
| 4 | Migration'ı kontrol et | `Migrations/` |
| 5 | Uygula | `dotnet ef database update` |
| 6 | Test et | `dotnet test` |

---

## 11. Bağımlılık Haritası

### 11.1 Proje Bağımlılıkları

| Proje | Bağımlı Olduğu Projeler |
|-------|------------------------|
| Domain (L0) | Yok |
| Abstractions (L1) | Domain |
| Application (L2) | Domain, Abstractions |
| CrossCutting (L3) | Domain, Abstractions, Application |
| Infrastructure.Data (L4.1) | Domain, Abstractions, Application |
| Infrastructure.AI (L4.2) | Domain, Abstractions, Application |
| Infrastructure.Config (L4.5) | Domain, Abstractions |
| Infrastructure.FileSystem (L4.10) | Domain, Abstractions |
| Infrastructure.Auth (L4.4) | Domain, Abstractions |
| Infrastructure.Security (L4.12) | Domain, Abstractions |
| Host (L6) | Tümü |
| UI (L7) | Host |

### 11.2 NuGet Paket Bağımlılıkları

| Paket | Kullanım | Projeler |
|-------|----------|----------|
| Microsoft.EntityFrameworkCore | ORM | Data |
| Microsoft.EntityFrameworkCore.Sqlite | SQLite provider | Data |
| MediatR | CQRS | Application, CrossCutting |
| FluentValidation | Doğrulama | Application |
| Serilog | Loglama | Logging |
| CommunityToolkit.Mvvm | MVVM | UI |
| DevExpress.* | UI kontrolleri | UI |
| xUnit | Test | Tests |
| Moq | Mocking | Tests |
| Polly | Dayanıklılık | Infrastructure |

---

## 12.performans Referansları

### 12.1 Beklenen Süreler

| İşlem | Beklenen Süre | Maksimum |
|-------|---------------|----------|
| Dosya okuma | < 10ms | 100ms |
| Dosya yazma | < 20ms | 200ms |
| DB sorgusu | < 10ms | 50ms |
| DB yazma | < 20ms | 100ms |
| AI isteği | < 5s | 30s |
| Build | < 30s | 120s |
| Test çalıştırma | < 60s | 300s |
| UI yanıt | < 16ms | 100ms |

### 12.2 Bellek Kullanımı

| Kaynak | Hedef | Maksimum |
|--------|-------|----------|
| Uygulama belleği | < 200MB | 500MB |
| DB belleği | < 50MB | 100MB |
| Cache belleği | < 100MB | 200MB |
| UI belleği | < 150MB | 300MB |

---

## 13. Versiyon & Güncelleme

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-25 | İlk sürüm, temel keyword haritası |
| 1.1.0 | 2026-08-26 | Enhanced - Dosya yapısı, agent yönlendirme, sorun çözüm haritası |
| 1.2.0 | 2026-08-26 | Enhanced - Entegrasyon kalıpları, workflow kısayolları, bağımlılık haritası, performans referansları |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26