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

## 13. Sektörel Keyword Haritası

Bu bölüm, farklı sektörlerdeki keyword'lerin hangi dosyalara ve agent'lara yönlendirileceğini tanımlar. Her sektör grubu, o alanda çalışırken aranacak keyword'leri ve hedef projeleri belirler.

### 13.1 Sanayi Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| üretim, imalat, fabrika, production, manufacturing | `src/VersaCoder.Application/Services/` | Build Agent | Üretim yönetimi servisleri |
| makine, machine, ekipman, equipment | `src/VersaCoder.Domain/Entities/` | Build Agent | Makine entity tanımları |
| kalite kontrol, quality, QC, ISO | `src/VersaCoder.Application/Validators/` | Build Agent | Kalite doğrulama kuralları |
| stok, depo, warehouse, inventory | `src/VersaCoder.Infrastructure.Data/Repositories/` | Build Agent | Stok yönetimi repository |
| sipariş, order, iş emri, work order | `src/VersaCoder.Application/Commands/` | Build Agent | Sipariş iş akışı komutları |
| tedarik, supplier,采购, procurement | `src/VersaCoder.Application/Services/` | Build Agent | Tedarik zinciri servisleri |
| PLC, SCADA, otomasyon, automation | `src/VersaCoder.Infrastructure/` | Build Agent | Endüstriyel otomasyon |
| bakım, maintenance, preventif | `src/VersaCoder.Application/Handlers/` | Build Agent | Bakım yönetimi handler'ları |
| enerji tüketimi, energy, kilowatt | `src/VersaCoder.Application/Queries/` | Build Agent | Enerji izleme sorguları |
| tolerans, spec, teknik resim | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Teknik değer nesneleri |

### 13.2 Teknoloji & Yazılım Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| API, REST, GraphQL, endpoint | `src/VersaCoder.Infrastructure.AI/` | Build Agent | API tasarım kalıpları |
| microservice, mikro servis | `src/VersaCoder.Infrastructure*/` | Plan Agent | Mikroservis mimarisi |
| container, Docker, konteyner | `*.yml, docker-compose` | Plan Agent | Container yapılandırması |
| Kubernetes, K8s, orkestrasyon | `deploy/k8s/` | Plan Agent | K8s manifest dosyaları |
| CI/CD, pipeline, boru hattı | `.github/workflows/` | Plan Agent | Süreç otomasyonu |
| monitoring, izleme, observability | `src/VersaCoder.Infrastructure.AI/` | Explore Agent | İzleme altyapısı |
| logging, loglama, structured log | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Yapılandırılmış loglama |
| caching, önbellek, Redis | `src/VersaCoder.Infrastructure.Data/` | Build Agent | Önbellek stratejileri |
| message queue, kuyruk, RabbitMQ | `src/VersaCoder.Infrastructure.Messaging/` | Build Agent | Mesaj kuyruğu entegrasyonu |
| gRPC, protobuf, Remote Procedure | `src/VersaCoder.Infrastructure.Network/` | Build Agent | RPC iletişimi |

### 13.3 Finans Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| muhasebe, accounting, defter | `src/VersaCoder.Application/Services/` | Build Agent | Muhasebe servisleri |
| fatura, invoice, billing | `src/VersaCoder.Application/Commands/` | Build Agent | Fatura oluşturucu |
| cari hesap, ledger, hesap | `src/VersaCoder.Domain/Entities/` | Build Agent | Cari hesap entity'si |
| ödeme, payment, tahsilat | `src/VersaCoder.Application/Handlers/` | Build Agent | Ödeme handler'ları |
| banka, bank, EFT, havale | `src/VersaCoder.Infrastructure.Data/` | Build Agent | Banka entegrasyonu |
| kredi, credit, loan, faiz | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Kredi değer nesneleri |
| portföy, portfolio, yatırım | `src/VersaCoder.Application/Queries/` | Build Agent | Portföy sorguları |
| döviz, currency, FX, kur | `src/VersaCoder.Application/Services/` | Build Agent | Kur servisleri |
| BIST, borsa, hisse, stock | `src/VersaCoder.Application/Handlers/` | Build Agent | Borsa işlem handler'ları |
| reel sektör, real economy, GSYİH | `src/VersaCoder.Application/Queries/` | Build Agent | Ekonomik göstergeler |
| Stopaj, vergi, tax, withholding | `src/VersaCoder.Application/Validators/` | Build Agent | Vergi hesaplama |

### 13.4 Sağlık Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| hasta, patient, medikal | `src/VersaCoder.Domain/Entities/` | Build Agent | Hasta entity tanımları |
| doktor, physician, hekim | `src/VersaCoder.Domain/Entities/` | Build Agent | Sağlık personeli |
| randevu, appointment, muayene | `src/VersaCoder.Application/Commands/` | Build Agent | Randevu yönetimi |
| reçete, prescription, ilaç | `src/VersaCoder.Application/Handlers/` | Build Agent | Reçete iş akışı |
| tahlil, test, laboratuvar, lab | `src/VersaCoder.Application/Services/` | Build Agent | Laboratuvar servisleri |
| MR, röntgen, ultrason, imaging | `src/VersaCoder.Infrastructure.Reporting/` | Build Agent | Görüntüleme raporlama |
| SGK, sigorta, insurance, provizyon | `src/VersaCoder.Application/Handlers/` | Build Agent | Sigorta provizyon |
| ameliyat, surgery, operasyon | `src/VersaCoder.Application/Commands/` | Build Agent | Cerrahi işlem komutları |
| vital, Nabız, tansiyon, temperature | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Vital değer nesneleri |
| epidemiyoloji, salgın, pandemic | `src/VersaCoder.Application/Queries/` | Build Agent | Halk sağlığı sorguları |
| e-Devlet, e-Nabız, MHRS | `src/VersaCoder.Infrastructure.Integration/` | Build Agent | Kamu entegrasyonu |

### 13.5 Eğitim Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| öğrenci, student, talebe | `src/VersaCoder.Domain/Entities/` | Build Agent | Öğrenci entity'si |
| öğretmen, teacher, eğitmen | `src/VersaCoder.Domain/Entities/` | Build Agent | Eğitim personeli |
| ders, course, müfredat, curriculum | `src/VersaCoder.Application/Services/` | Build Agent | Ders yönetim servisi |
| not, grade, değerlendirme | `src/VersaCoder.Application/Handlers/` | Build Agent | Not değerlendirme |
| Sınav, exam, test, assessment | `src/VersaCoder.Application/Commands/` | Build Agent | Sınav yönetimi |
| devamsızlık, attendance, yoklama | `src/VersaCoder.Application/Handlers/` | Build Agent | Yoklama handler'ı |
| diploma, sertifika, belge | `src/VersaCoder.Infrastructure.Reporting/` | Build Agent | Belge üretimi |
| LMS, e-öğrenme, online eğitim | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Uzaktan eğitim |
| YÖK, ÖSYM, MEB, bakanlık | `src/VersaCoder.Infrastructure.Integration/` | Build Agent | Kamu kurum entegrasyonu |
| akreditasyon, accreditation | `src/VersaCoder.Application/Validators/` | Build Agent | Akreditasyon doğrulama |

### 13.6 Kamu Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| belediye, municipality, yerel yönetim | `src/VersaCoder.Application/Services/` | Build Agent | Yerel yönetim servisleri |
| valilik, kaymakamlık, il / ilçe | `src/VersaCoder.Domain/Entities/` | Build Agent | İdari birim entity'leri |
| e-Devlet, governorship, resmi | `src/VersaCoder.Infrastructure.Integration/` | Build Agent | e-Devlet entegrasyonu |
| ihale, tender, procurement, auction | `src/VersaCoder.Application/Commands/` | Build Agent | İhale yönetimi |
| mevzuat, regulation, kanun, tüzük | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Mevzuat değer nesneleri |
| Nüfus, population, göç, migrate | `src/VersaCoder.Application/Queries/` | Build Agent | Nüfus sorguları |
| tapu, kadastro, cadde, sokak | `src/VersaCoder.Infrastructure.Data/Repositories/` | Build Agent | Kadastro repository |
| zabıta, denetim, kontrol | `src/VersaCoder.Application/Handlers/` | Build Agent | Denetim handler'ları |
| muhtarlık, village, köy | `src/VersaCoder.Domain/Entities/` | Build Agent | Köy/muhtarlık entity'si |
| resmi gazete, official gazette | `src/VersaCoder.Infrastructure.Documentation/` | Build Agent | Resmi doküman işleme |

### 13.7 Enerji Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| elektrik, electric, enerji, energy | `src/VersaCoder.Application/Services/` | Build Agent | Enerji yönetim servisleri |
| trafo, transformer, şebeke, grid | `src/VersaCoder.Domain/Entities/` | Build Agent | Şebeke entity tanımları |
| güneş, solar, fotovoltaik, PV | `src/VersaCoder.Application/Handlers/` | Build Agent | Güneş enerjisi handler'ları |
| rüzgar, wind, türbin, turbine | `src/VersaCoder.Application/Commands/` | Build Agent | Rüzgar enerjisi komutları |
| doğal gaz, natural gas, LNG | `src/VersaCoder.Application/Services/` | Build Agent | Gaz yönetimi servisleri |
| petrol, oil, rafineri, refinery | `src/VersaCoder.Infrastructure.Data/` | Build Agent | Petrol veri yönetimi |
| enterkoneksiyon, interconnection | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Şebeke entegrasyonu |
| EPDK, BOTAŞ, TEİAŞ, trader | `src/VersaCoder.Infrastructure.Integration/` | Build Agent | Kurum entegrasyonları |
| sayaç, meter, ölçüm, measurement | `src/VersaCoder.Application/Queries/` | Build Agent | Ölçüm sorguları |
| karbon, emisyon, carbon, CO2 | `src/VersaCoder.Application/Services/` | Build Agent | Karbon ayak izi |
| nükleer, nuclear, reaktör | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Nükleer değer nesneleri |

### 13.8 Lojistik Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| nakliye, transport, kargo, cargo | `src/VersaCoder.Application/Services/` | Build Agent | Nakliye yönetimi |
| depo, warehouse, dağıtım merkezi | `src/VersaCoder.Domain/Entities/` | Build Agent | Depo entity tanımları |
| gümrük, customs, ithalat / ihracat | `src/VersaCoder.Application/Handlers/` | Build Agent | Gümrük handler'ları |
| filo, fleet, araç takip | `src/VersaCoder.Application/Commands/` | Build Agent | Filo yönetimi |
| rot optimizasyonu, routing, planlama | `src/VersaCoder.Application/Services/` | Build Agent | Rota optimizasyonu |
| tedarik zinciri, supply chain, SCM | `src/VersaCoder.Application/Handlers/` | Build Agent | SCM handler'ları |
| soğuk zincir, cold chain, fridge | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Soğuk zincir değerleri |
| irsaliye, waybill, sevk | `src/VersaCoder.Application/Commands/` | Build Agent | İrsaliye yönetimi |
| GPS, konum, location, tracking | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Konum izleme |
| depolama, storage, stok yönetimi | `src/VersaCoder.Infrastructure.Data/Repositories/` | Build Agent | Depolama repository |

### 13.9 Medya Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| yayın, broadcast, medya, media | `src/VersaCoder.Application/Services/` | Build Agent | Medya yönetim servisleri |
| içerik, content, produced, üretilen | `src/VersaCoder.Domain/Entities/` | Build Agent | İçerik entity tanımları |
| video, ses, multimedia, multimedya | `src/VersaCoder.Infrastructure.Reporting/` | Build Agent | Multimedya işleme |
| abone, subscriber, üye, membership | `src/VersaCoder.Application/Commands/` | Build Agent | Abonelik yönetimi |
| reklam, advertising, kampanya | `src/VersaCoder.Application/Handlers/` | Build Agent | Reklam handler'ları |
| editoryal, editorial, haber, news | `src/VersaCoder.Application/Services/` | Build Agent | Haber yönetimi |
| DRM, telif hakkı, copyright, lisans | `src/VersaCoder.Infrastructure.Security/` | Build Agent | Dijital hak yönetimi |
| CDN, akış, streaming, broadcast | `src/VersaCoder.Infrastructure.Network/` | Build Agent | İçerik dağıtımı |
| podcast, vlog, blog, post | `src/VersaCoder.Application/Commands/` | Build Agent | Medya içerik yönetimi |
| rating, reyting, istatistik, analitik | `src/VersaCoder.Application/Queries/` | Build Agent | Reyting sorguları |

### 13.10 Savunma Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| askeri, military, savunma, defense | `src/VersaCoder.Application/Services/` | Build Agent | Savunma servisleri |
| strateji, strategy, planlama | `src/VersaCoder.Domain/Entities/` | Build Agent | Stratejik planlama entity'leri |
| istihbarat, intelligence, analiz | `src/VersaCoder.Application/Handlers/` | Build Agent | İstihbarat analiz handler'ları |
| lojistik, tedarik, harp | `src/VersaCoder.Application/Commands/` | Build Agent | Askeri lojistik komutları |
| radar, sensör, algılama, detection | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Sensör değer nesneleri |
| siber, cyber, network güvenliği | `src/VersaCoder.Infrastructure.Security/` | Build Agent | Siber güvenlik altyapısı |
| TSK, Jandarma, Emniyet, kuvvet | `src/VersaCoder.Infrastructure.Integration/` | Build Agent | Kurum entegrasyonları |
| envanter, malzeme, unsurlar | `src/VersaCoder.Infrastructure.Data/Repositories/` | Build Agent | Envanter repository |
| tatbikat, drill, simülasyon, training | `src/VersaCoder.Application/Services/` | Build Agent | Tatbikat yönetimi |
| koordinasyon, haberleşme, comm | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Haberleşme altyapısı |

### 13.11 Tarım Sektörü

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| tarım, agriculture, çiftçilik, farming | `src/VersaCoder.Application/Services/` | Build Agent | Tarım yönetim servisleri |
| sulama, irrigation, su yönetimi | `src/VersaCoder.Application/Handlers/` | Build Agent | Sulama handler'ları |
| gübre, fertilizer, toprak, soil | `src/VersaCoder.Domain/ValueObjects/` | Build Agent | Toprak değer nesneleri |
| hasat, harvest, üretim, verim | `src/VersaCoder.Application/Commands/` | Build Agent | Hasat yönetimi |
| zirai ilaç, pestisit, bitki koruma | `src/VersaCoder.Application/Validators/` | Build Agent | Zirai ilaç doğrulama |
| sera, greenhouse, kontrollü tarım | `src/VersaCoder.Domain/Entities/` | Build Agent | Sera entity tanımları |
| GAP, DSI, ziraat, bakanlık | `src/VersaCoder.Infrastructure.Integration/` | Build Agent | Kurum entegrasyonları |
| kooperatif, birlik, meslek örgütü | `src/VersaCoder.Application/Services/` | Build Agent | Kooperatif yönetimi |
| hayvancılık, livestock, besi, sürü | `src/VersaCoder.Application/Commands/` | Build Agent | Hayvancılık yönetimi |
| organik, sertifika, coğrafi işaret | `src/VersaCoder.Application/Validators/` | Build Agent | Organik sertifikasyon |

---

## 14. Real-time & Monitoring Keyword Haritası

Bu bölüm, gerçek zamanlı iletişim ve izleme ile ilgili keyword'lerin eşleme haritasını tanımlar.

### 14.1 SignalR & WebSocket Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| SignalR, hub, HubConnection | `src/VersaCoder.Infrastructure.Network/` | Build Agent | SignalR hub implementasyonu |
| WebSocket, WS, WSS, soket | `src/VersaCoder.Infrastructure.Network/` | Build Agent | WebSocket bağlantısı |
| bağlantı, connection, reconnect | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Bağlantı yönetimi |
| broadcast, yayım, group | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Grup yayını |
| messaging, mesajlaşma, chat | `src/VersaCoder.Application/Handlers/` | Build Agent | Gerçek zamanlı mesajlaşma |
| duplex, çift yönlü, bidirectional | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Çift yönlü iletişim |
| negotiate, el sıkışma, handshake | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Bağlantı kurma protokolü |
| ping, pong, keepalive, alive | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Bağlantı canlılık kontrolü |
| stream, akış, real-time stream | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Gerçek zamanlı veri akışı |
| transport,长短轮询, long-polling | `src/VersaCoder.Infrastructure.Network/` | Build Agent | Transport katmanı |

### 14.2 Grafana & Prometheus Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| Grafana, panel, dashboard, gösterge | `config/grafana/` | Plan Agent | Grafana yapılandırması |
| Prometheus, metrics, metrik | `config/prometheus/` | Plan Agent | Prometheus metric tanımları |
| counter, sayaç, increment | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Sayaç metrikleri |
| histogram, dağılım, latency | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Histogram metrikleri |
| gauge, gösterge, temperature | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Gauge metrikleri |
| scrape, toplama, collection | `config/prometheus/` | Plan Agent | Veri toplama yapılandırması |
| alert rule, kural, tetikleme | `config/grafana/provisioning/` | Plan Agent | Alarm kuralları |
| exporter, dışa aktarma, adapter | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Metrik dışa aktarıcılar |
| SLA, SLO, SLI, hedef | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Seviye hedefleri |
| time series, zaman serisi, TSDB | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Zaman serisi verisi |

### 14.3 Alert & Dashboard Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| alarm, alert, uyarı, bildirim | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Alarm sistemi |
| severity, önem, kritik, critical | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Alarm önem seviyeleri |
| threshold, eşik, limit, maxValue | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Eşik değerleri |
| dashboard, panolar, kontrol paneli | `src/VersaCoder.Infrastructure.Reporting/` | Build Agent | Dashboard oluşturma |
| KPI, gösterge, indicator | `src/VersaCoder.Application/Queries/` | Build Agent | KPI sorguları |
| canary, golden signal, sinyal | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Golden signal izleme |
| on-call, nöbet, duty | `src/VersaCoder.Application/Services/` | Build Agent | Nöbet yönetimi |
| incident, olay, response | `src/VersaCoder.Application/Handlers/` | Build Agent | Olay müdahale handler'ları |
| uptime, çalışma süresi, availability | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Kullanılabilirlik izleme |
| error rate, hata oranı, failure | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Hata oranı takibi |

### 14.4 Logging & Tracing Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| structured logging, yapılandırılmış log | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Yapılandırılmış loglama |
| correlation ID, izleme, tracing | `src/VersaCoder.CrossCutting/` | Build Agent | İstek izleme |
| OpenTelemetry, OTel, distributed tracing | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Dağıtık izleme |
| span, iz, track | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Trace span'ları |
| log level, log seviyesi, verbosity | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Log seviye yönetimi |
| Serilog, seq, ELK,索引 | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Loglama framework'leri |
| audit log, denetim kaydı | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Denetim logları |
| retention, saklama, arşiv | `src/VersaCoder.Infrastructure.Logging/` | Build Agent | Log saklama politikaları |

---

## 15. CI/CD & Deployment Keyword Haritası

Bu bölüm, sürekli entegrasyon, sürekli dağıtım ve dağıtım ile ilgili keyword'lerin eşleme haritasını tanımlar.

### 15.1 GitHub Actions & Pipeline Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| GitHub Actions, workflow, iş akışı | `.github/workflows/` | Plan Agent | GitHub Actions tanımları |
| pipeline, boru hattı, CI/CD | `.github/workflows/` | Plan Agent | Pipeline yapılandırması |
| build, derleme, compile | `.github/workflows/` | Plan Agent | Build adımları |
| test, deneme, test suite | `.github/workflows/` | Plan Agent | Test adımları |
| lint, kod analizi, static analysis | `.github/workflows/` | Plan Agent | Kod kalite adımları |
| artifact, eser, output | `.github/workflows/` | Plan Agent | Artifact yönetimi |
| matrix, çoklu, strateji | `.github/workflows/` | Plan Agent | Matrix build stratejisi |
| secret, gizli, environment variable | `.github/workflows/` | Plan Agent | Gizli değişken yönetimi |
| trigger, tetikleme, on push | `.github/workflows/` | Plan Agent | Tetikleme kuralları |
| runner, koşucu, agent | `.github/workflows/` | Plan Agent | Runner yapılandırması |

### 15.2 Docker & Container Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| Dockerfile, image, görüntü | `Dockerfile` | Plan Agent | Docker görüntü tanımları |
| docker-compose, compose, multi-service | `docker-compose.yml` | Plan Agent | Multi-service yapılandırma |
| container, konteyнер, pod | `deploy/` | Plan Agent | Container yapılandırması |
| layer, katman, optimize | `Dockerfile` | Plan Agent | Görüntü katman optimizasyonu |
| volume, birim, persistent | `docker-compose.yml` | Plan Agent | Kalıcı veri birimleri |
| network, ağ, bridge | `docker-compose.yml` | Plan Agent | Container ağları |
| health check, sağlık kontrolü | `Dockerfile` | Plan Agent | Container sağlık kontrolü |
| .dockerignore, hariç tutma | `.dockerignore` | Plan Agent | hariç tutma listesi |
| registry, kayıt, depo, harbor | `deploy/` | Plan Agent | Görüntü kayıt defteri |
| multi-stage, çoklu aşama | `Dockerfile` | Plan Agent | Çoklu aşama derleme |

### 15.3 Kubernetes & Orchestrasyon Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| Kubernetes, K8s, küme | `deploy/k8s/` | Plan Agent | Kubernetes yapılandırması |
| Deployment, dağıtım, replica | `deploy/k8s/*.yaml` | Plan Agent | Deployment tanımları |
| Service, servis, cluster IP | `deploy/k8s/*.yaml` | Plan Agent | Service tanımları |
| Ingress, giriş, load balancer | `deploy/k8s/*.yaml` | Plan Agent | Ingress tanımları |
| ConfigMap, yapılandırma, ayar | `deploy/k8s/*.yaml` | Plan Agent | ConfigMap tanımları |
| Secret, gizli, credential | `deploy/k8s/*.yaml` | Plan Agent | Kubernetes Secret tanımları |
| PersistentVolume, PV, kalıcı | `deploy/k8s/*.yaml` | Plan Agent | PersistentVolume tanımları |
| HPA, autoscaling, otomatik ölçek | `deploy/k8s/*.yaml` | Plan Agent | Otonom ölçekleme |
| namespace, ad alanı, izolasyon | `deploy/k8s/*.yaml` | Plan Agent | Namespace yönetimi |
| Helm, chart, paket | `deploy/helm/` | Plan Agent | Helm chart yapılandırması |
| kubectl, komut, CLI | `deploy/scripts/` | Plan Agent | kubectl komutları |
| pod, konteyner grubu | `deploy/k8s/*.yaml` | Plan Agent | Pod yapılandırması |

### 15.4 Deployment & Release Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| deployment, dağıtım, publish | `deploy/` | Plan Agent | Dağıtım yapılandırması |
| release, sürüm, versioning | `deploy/` | Plan Agent | Sürüm yönetimi |
| rollback, geri alma, revert | `deploy/scripts/` | Plan Agent | Geri alma betikleri |
| blue-green, mavi-yeşil, strateji | `deploy/` | Plan Agent | Blue-green dağıtım |
| canary, stripe, kademeli | `deploy/` | Plan Agent | Kademeli dağıtım |
| staging, ortam, environment | `deploy/environments/` | Plan Agent | Staging yapılandırması |
| production, canlı, prod | `deploy/environments/prod/` | Plan Agent | Production yapılandırması |
| feature flag, özellik bayrağı | `src/VersaCoder.Infrastructure.Config/` | Build Agent | Özellik bayrakları |
| A/B test, deneme, split test | `src/VersaCoder.Application/Services/` | Build Agent | A/B test servisi |
| smoke test, duman testi | `tests/` | Build Agent | Smoke testleri |
| E2E test, uçtan uca | `tests/` | Build Agent | Uçtan uca testleri |

### 15.5 Infrastructure as Code (IaC) Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| Terraform, altyapı, IaC | `deploy/terraform/` | Plan Agent | Terraform yapılandırması |
| ARM template, Azure Resource | `deploy/arm/` | Plan Agent | ARM template tanımları |
| Ansible, otomasyon, playbook | `deploy/ansible/` | Plan Agent | Ansible playbook'ları |
| Pulumi, programmatic IaC | `deploy/pulumi/` | Plan Agent | Pulumi yapılandırması |
| cloud, bulut, AWS / Azure / GCP | `deploy/cloud/` | Plan Agent | Bulut yapılandırması |
| state, durum, lock | `deploy/terraform/` | Plan Agent | State yönetimi |
| module, modül, yeniden kullanım | `deploy/terraform/modules/` | Plan Agent | Terraform modülleri |
| variable, değişken, input | `deploy/terraform/variables.tf` | Plan Agent | Değişken tanımları |
| output, çıktı, export | `deploy/terraform/outputs.tf` | Plan Agent | Çıktı tanımları |
| provider, sağlayıcı, plugin | `deploy/terraform/` | Plan Agent | Provider yapılandırması |

---

## 16. UI Framework Keyword Haritası

Bu bölüm, UI framework'leri ve MVVM kalıpları ile ilgili keyword'lerin eşleme haritasını tanımlar.

### 16.1 DevExpress & WinForms Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| DevExpress, DX, WinForms | `src/VersaCoder.UI/` | Build Agent | DevExpress WinForms UI |
| Ribbon, kurdele, toolbar | `src/VersaCoder.UI/Forms/` | Build Agent | Ribbon kontrolü |
| GridControl, grid, tablo | `src/VersaCoder.UI/Controls/` | Build Agent | Grid kontrolü |
| XtraEditors, editör, input | `src/VersaCoder.UI/Controls/` | Build Agent | Editör kontrolleri |
| XtraBars, bar, menü | `src/VersaCoder.UI/Forms/` | Build Agent | Bar/menü kontrolleri |
| XtraLayout, layout, yerleşim | `src/VersaCoder.UI/Forms/` | Build Agent | Yerleşim düzeni |
| XtraReports, rapor, report | `src/VersaCoder.UI/Reports/` | Build Agent | Rapor tasarımı |
| SplashScreen, splash, loading | `src/VersaCoder.UI/Forms/` | Build Agent | Splash ekranı |
| MDI, çoklu belge, child form | `src/VersaCoder.UI/Forms/` | Build Agent | MDI ana form |
| dock,Panel, panel, yan panel | `src/VersaCoder.UI/Forms/` | Build Agent | Dock panel |
| LookUpEdit, lookup, arama | `src/VersaCoder.UI/Controls/` | Build Agent | Lookup editör |
| TreeList, ağaç, hierarchical | `src/VersaCoder.UI/Controls/` | Build Agent | Ağaç listesi |
| MemoEdit, text, çoklu satır | `src/VersaCoder.UI/Controls/` | Build Agent | Metin editörü |
| DateEdit, tarih, takvim | `src/VersaCoder.UI/Controls/` | Build Agent | Tarih editörü |
| PictureEdit, resim, görsel | `src/VersaCoder.UI/Controls/` | Build Agent | Resim editörü |

### 16.2 MAUI & Blazor Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| MAUI, multi-platform, cross-platform | `src/VersaCoder.UI.Maui/` | Build Agent | .NET MAUI uygulaması |
| Blazor, Razor, component | `src/VersaCoder.UI.Blazor/` | Build Agent | Blazor bileşenleri |
| WASM, WebAssembly, tarayıcı | `src/VersaCoder.UI.Blazor/` | Build Agent | Blazor WebAssembly |
| Hybrid, hibrit, native | `src/VersaCoder.UI.Maui/` | Build Agent | Hibrit uygulama |
| Razor Component, partial, view | `src/VersaCoder.UI.Blazor/Components/` | Build Agent | Razor bileşenleri |
| CascadingParameter, parametre | `src/VersaCoder.UI.Blazor/` | Build Agent | Kademeli parametreler |
| EditForm, form, veri girişi | `src/VersaCoder.UI.Blazor/Components/` | Build Agent | Form bileşenleri |
| StateHasChanged, durum, refresh | `src/VersaCoder.UI.Blazor/` | Build Agent | Durum güncelleme |
| NavigationManager, yönlendirme | `src/VersaCoder.UI.Blazor/` | Build Agent | Sayfa yönlendirme |
| JSInterop, JavaScript, bridge | `src/VersaCoder.UI.Blazor/` | Build Agent | JS köprüleme |
| MAUI Handler, native, platform | `src/VersaCoder.UI.Maui/` | Build Agent | Platform handler'ları |
| Shell, güzergah, routing | `src/VersaCoder.UI.Maui/` | Build Agent | MAUI Shell yapılandırması |

### 16.3 MVVM & CommunityToolkit Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| MVVM, Model-View-ViewModel | `src/VersaCoder.UI/ViewModels/` | Build Agent | MVVM mimarisi |
| ViewModel, görünüm modeli | `src/VersaCoder.UI/ViewModels/` | Build Agent | ViewModel sınıfları |
| ObservableProperty, gözlemci | `src/VersaCoder.UI/ViewModels/` | Build Agent | Observable property'ler |
| RelayCommand, komut, command | `src/VersaCoder.UI/ViewModels/` | Build Agent | Komut tanımları |
| CommunityToolkit, MVVM toolkit | `src/VersaCoder.UI/ViewModels/` | Build Agent | CommunityToolkit.Mvvm |
| INotifyPropertyChanged, INPC | `src/VersaCoder.UI/ViewModels/` | Build Agent | Değişim bildirimi |
| Binding, bağlama, data bind | `src/VersaCoder.UI/Views/` | Build Agent | Veri bağlama |
| DataTemplate, şablon, template | `src/VersaCoder.UI/Views/` | Build Agent | Veri şablonları |
| UserControl, özelleştirilmiş, custom | `src/VersaCoder.UI/Controls/` | Build Agent | Özel kontroller |
| Ioc.Default, servis, DI | `src/VersaCoder.UI/App.xaml.cs` | Build Agent | UI DI контейнера |
| IsBusy, meşgul, loading indicator | `src/VersaCoder.UI/ViewModels/` | Build Agent | Yükleme göstergesi |
| ValidatableObject, doğrulama | `src/VersaCoder.UI/ViewModels/` | Build Agent | ViewModel doğrulama |
| AsyncRelayCommand, async, bekleyen | `src/VersaCoder.UI/ViewModels/` | Build Agent | Async komutlar |
| WeakReferenceMessenger, mesaj | `src/VersaCoder.UI/ViewModels/` | Build Agent | Zayıf referanslı mesajlaşma |
| FuncValueConverter, dönüştürücü | `src/VersaCoder.UI/Converters/` | Build Agent | Değer dönüştürücüler |
| MultiplexingBinding, çoklu bağlama | `src/VersaCoder.UI/Views/` | Build Agent | Çoklu bağlama desteği |

### 16.4 UI Tasarım & UX Anahtar Kelimeleri

| Keyword | Hedef Dosya | Hedef Agent | Açıklama |
|---------|-------------|-------------|----------|
| responsive, duyarlı, adaptive | `src/VersaCoder.UI/Styles/` | Build Agent | Duyarlı tasarım |
| theme, tema, görünüm | `src/VersaCoder.UI/Themes/` | Build Agent | Tema yönetimi |
| dark mode, karanlık mod | `src/VersaCoder.UI/Themes/` | Build Agent | Karanlık mod desteği |
| localization, yerelleştirme, i18n | `src/VersaCoder.UI/Resources/` | Build Agent | Yerelleştirme |
| accessibility, erişilebilirlik | `src/VersaCoder.UI/` | Build Agent | Erişilebilirlik |
| animation, animasyon, geçiş | `src/VersaCoder.UI/Animations/` | Build Agent | Animasyonlar |
| dialog, iletişim kutusu, popup | `src/VersaCoder.UI/Dialogs/` | Build Agent | Diyalog pencereleri |
| notification, bildirim, toast | `src/VersaCoder.UI/Notifications/` | Build Agent | Bildirim sistemi |
| status bar, durum çubuğu | `src/VersaCoder.UI/Forms/` | Build Agent | Durum çubuğu |
| context menu, sağ tık menüsü | `src/VersaCoder.UI/Controls/` | Build Agent | Bağlam menüsü |
| keyboard shortcut, klavye kısayolu | `src/VersaCoder.UI/Forms/` | Build Agent | Klavye kısayolları |
| drag & drop, sürükle bırak | `src/VersaCoder.UI/Controls/` | Build Agent | Sürükle bırak desteği |

---

## 17. Versiyon & Güncelleme

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-25 | İlk sürüm, temel keyword haritası |
| 1.1.0 | 2026-08-26 | Enhanced - Dosya yapısı, agent yönlendirme, sorun çözüm haritası |
| 1.2.0 | 2026-08-26 | Enhanced - Entegrasyon kalıpları, workflow kısayolları, bağımlılık haritası, performans referansları |
| 1.3.0 | 2026-08-26 | Sektörel keyword haritası, real-time/monitoring, CI/CD, UI framework bölümleri eklendi |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26