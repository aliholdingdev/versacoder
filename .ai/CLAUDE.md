# Versa Coder — AI Constitution & Boot Protocol

**Type:** Constitution | **Category:** core | **Status:** active | **Version:** 1.0.0
**Authority:** Single Source of Truth (SSOT)
**Governance:** Red Team · Human Mode · Truth Mode
**Token Budget:** ~6000 token

---

## 1. Preamble

Versa Coder, yapay zeka destekli bir IDE (Integrated Development Environment) platformudur. Bu belge, tüm yapay zeka ajanlarının çalışması için zorunlu olan anayasal kuralları, protokolleri ve standartları tanımlar.

### 1.1 Proje Tanımı

| Özellik | Değer |
|---------|-------|
| Proje Adı | Versa Coder |
| Tür | AI-Integrated IDE |
| Platform | DevExpress WinForms |
| Dil | C# .NET 8 |
| Veritabanı | SQLite (WAL) |
| AI | Çoklu provider (OpenAI, Anthropic, Google, Ollama) |

---

## 2. Boot Protocol

### 2.1 Başlangıç Sırası

Her AI session başlatıldığında aşağıdaki sırayla yüklenmelidir:

| Sıra | Dosya | Amaç | Token Limiti |
|------|-------|------|--------------|
| 1 | CLAUDE.md | AI anayasası | ~6000 |
| 2 | AGENTS.md | Ajan kayıt defteri | ~5000 |
| 3 | WORKFLOW.md | Mühendislik süreçleri | ~4000 |
| 4 | brain.md | Mimari kararlar | ~4000 |
| 5 | ROLE.md | Rol tanımları | ~5000 |
| 6 | index.md | Ana katalog | ~1500 |
| 7 | keys.md | Anahtar kelime eşleme | ~1000 |

**Toplam Token Bütçesi:** ~26,500 token

### 2.2 Session Başlatma Prosedürü

```
1. Vault yükle (CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md)
2. Son session log'unu oku
3. Proje durumunu kontrol et
4. Kullanıcı isteğini analiz et
5. Uygun agent'ı seç
6. Görevi başlat
```

### 2.3 Context Assembly

```
Session Başlatma
  → [1. Vault Yükle] — CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md
    → [2. Session Log] — Son session'ı oku
      → [3. Proje Durumu] — Mevcut durumu kontrol et
        → [4. İstek Analizi] — Kullanıcı isteğini analiz et
          → [5. Agent Seçimi] — Uygun agent'ı seç
            → [6. Görev Başlatma] — Görevi başlat
```

---

## 3. Temel İlkeler

### 3.1 Single Source of Truth (SSOT)

- Tüm mimari kararlar `.ai/` vault'unda saklanır
- Dış kaynaklar yalnızca referans için kullanılır
- Çelişki durumunda vault'taki bilgi geçerlidir
- Vault'un güncellenmesi yalnızca Master Orchestrator tarafından yapılabilir

### 3.2 Zero Code Before Plan

- Kod yazmadan önce plan oluşturulmalıdır
- Plan onay aldıktan sonra kodlama başlar
- Plansız kodlama yasaktır
- Plan değişiklikleri loglanmalıdır

### 3.3 Human Approval Gate

- Kritik kararlar için insan onayı gereklidir
- Mimari değişiklikler onay gerektirir
- Güvenlikle ilgili değişiklikler onay gerektirir
- Onay alınmadan işlem yapılmaz

### 3.4 Learn & Adapt

- Her görev sonrası öğrenme kaydı tutulur
- Hatalar tekrarlanmamalıdır
- Başarılı kalıplar kaydedilmelidir
- Knowledge base sürekli güncellenir

---

## 4. Guardrails (Koruyucu Kurallar)

### 4.1 Zorunlu Guardrails

| # | Kural | Sonuç | Kategori |
|---|-------|-------|----------|
| 1 | Kod yazmadan önce plan yap | Kod geri alınır | Process |
| 2 | Vault'tan bilgi almadan kodlama yapma | Kod geçersiz | Knowledge |
| 3 | Uydurma bilgi kullanma | İçerik silinir | Integrity |
| 4 | Dosyaları yerinde değiştir (refactoring) | Dosya geri yüklenir | Code |
| 5 | Tek Doğruluk Kaynağı kullan | Dış bilgi reddedilir | Knowledge |
| 6 | Şablon kullanımı zorunlu | Dosya geçersiz | Code |
| 7 | Session sürekliliği sağla | Bağlam kaybolur | Process |
| 8 | İnsan onayı al | Kod geri alınır | Process |
| 9 | Bağlam toplama önce yap | Yanlış çıktı | Knowledge |
| 10 | Öğrenme aktif tut | Tekrar hata | Learning |
| 11 | Diagram öğretme yap | Yanlış anlama | Knowledge |
| 12 | Çelişki kapısı oluştur | Süreç durur | Process |
| 13 | ORM kullanma (EF Core DbContext ONLY) | SQL enjeksiyonu | Security |
| 14 | WinForms code-behind kullanma | Bakımı zor kod | Code |
| 15 | DevExpress kullanımı zorunlu | Tutarsızlık | UI |
| 16 | SQLite WAL modu kullan | Performans düşüklüğü | Performance |

### 4.2 Guardrail Uygulama

Her guardrail ihlali tespit edildiğinde:
1. Hata loglanır
2. İşlem durdurulur
3. İnsan onayı beklenir
4. Düzeltme yapılır
5. Devam edilir

### 4.3 Guardrail Kategorileri

| Kategori | Guardrail Sayısı | Örnekler |
|----------|-----------------|----------|
| Process | 4 | #1, #7, #8, #12 |
| Knowledge | 3 | #2, #5, #9, #11 |
| Code | 2 | #4, #6, #14 |
| Security | 1 | #13 |
| Performance | 1 | #16 |
| UI | 1 | #15 |
| Learning | 1 | #10 |

---

## 5. Agent Kullanım Protokolü

### 5.1 Agent Seçim Kriterleri

| Durum | Kullanılacak Agent | Öncelik |
|-------|-------------------|---------|
| Kod yazma/düzenleme | Build Agent | Yüksek |
| Mimari planlama | Plan Agent | Yüksek |
| Kod analizi/tarama | Explore Agent | Orta |
| Dokümantasyon | Summary Agent | Orta |
| İsimlendirme | Title Agent | Düşük |
| Genel görevler | General Agent | Düşük |
| Koordinasyon | Master Orchestrator | Yüksek |

### 5.2 Agent Çalıştırma Prosedürü

```
1. Kullanıcı isteğini analiz et
2. Keyword'leri çıkar
3. Uygun agent'ı seç
4. Agent profilini yükle
5. Görevi tanımla
6. Agent'ı çalıştır
7. Çıktıyı doğrula
8. Gerekirse handover yap
```

### 5.3 Agent Seçim Algoritması

```csharp
public AgentRole SelectAgent(string userPrompt)
{
    var prompt = userPrompt.ToLowerInvariant();

    // Priority 1: Build Agent
    if (ContainsAny(prompt, BuildKeywords))
        return AgentRole.Build;

    // Priority 2: Plan Agent
    if (ContainsAny(prompt, PlanKeywords))
        return AgentRole.Plan;

    // Priority 3: Explore Agent
    if (ContainsAny(prompt, ExploreKeywords))
        return AgentRole.Explore;

    // Priority 4: Summary Agent
    if (ContainsAny(prompt, SummaryKeywords))
        return AgentRole.Summary;

    // Priority 5: Title Agent
    if (ContainsAny(prompt, TitleKeywords))
        return AgentRole.Title;

    // Default: General Agent
    return AgentRole.General;
}
```

---

## 6. Mimari Kurallar

### 6.1 Katmanlı Mimari (Clean Architecture)

| Katman | Ad | Sorumluluk | Bağımlılık |
|--------|-----|-----------|-----------|
| L0 | Domain | Varlıklar, Değer Nesneleri, Olaylar | Hiçbiri |
| L1 | Abstractions | Arayüzler, Sözleşmeler | L0 |
| L2 | Application | Use Case'ler, DTO'lar, Handler'lar | L1 |
| L3 | CrossCutting | Loglama, İstisna, Doğrulama | L2 |
| L4 | Infrastructure | Modüller, Servisler | L3 |
| L5 | Protocol | AI Protokol, MCP | L4 |
| L6 | Host | Başlatma, DI, Yapılandırma | L5 |
| L7 | UI | DevExpress WinForms | L6 |

### 6.2 Bağımlılık Kuralları

```
L7 → L6 (İzin verilen)
L6 → L5 (İzin verilen)
L5 → L4 (İzin verilen)
L4 → L3 (İzin verilen)
L3 → L2 (İzin verilen)
L2 → L1 (İzin verilen)
L1 → L0 (İzin verilen)

L0 → L2 (YASAK)
L1 → L3 (YASAK)
L2 → L4 (YASAK)
L3 → L5 (YASAK)
L4 → L6 (YASAK)
L5 → L7 (YASAK)
```

### 6.3 Teknoloji Yığını

| Katman | Teknoloji | Versiyon |
|--------|-----------|----------|
| UI | DevExpress WinForms | 2026 Universal |
| Backend | C# .NET | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| Veritabanı | SQLite | WAL modu |
| AI | Çoklu sağlayıcı | OpenAI, Anthropic, Google, Ollama |
| MCP | Model Context Protocol | Latest |
| Git | LibGit2Sharp | Latest |
| Loglama | Serilog | Latest |
| Test | xUnit | Latest |
| IoC | MS.Extensions.DI | Latest |
| MVVM | CommunityToolkit.Mvvm | Latest |
| Doğrulama | FluentValidation | Latest |
| Dayanıklılık | Polly | Latest |
| Markdown | Markdig | Latest |
| CQRS | MediatR | Latest |

---

## 7. Güvenlik Kuralları

### 7.1 Hassas Veri Koruması

- API anahtarları vault'ta saklanır
- Veritabanı şifreleri şifrelenir
- Loglarda hassas veri bulunmaz
- Günlük erişim logları tutulur

### 7.2 Erişim Kontrolü

| Kaynak | Erişim | Sorumlu |
|--------|--------|---------|
| Kod dosyaları | Build Agent | Sadece o |
| Config dosyaları | Plan Agent | Sadece o |
| Vault dosyaları | Tümü (okuma) | MO (yazma) |
| Log dosyaları | Tümü (append) | — |
| Test dosyaları | Build Agent | Sadece o |

### 7.3 Güvenlik Seviyeleri

| Seviye | Tanım | Aksiyon |
|--------|-------|---------|
| Critical | Sistem açığı | Derhal düzelt |
| High | Veri sızıntısı | 24 saat içinde |
| Medium | Yetki hatası | 1 hafta içinde |
| Low | İyileştirme | Plan dahilinde |

---

## 8. Kalite Standartları

### 8.1 Kod Kalitesi

- SOLID prensiplerine uygunluk
- Temiz Kod (Clean Code) standartları
- Minimum %80 test kapsama oranı
- Statik analiz hataları = 0

### 8.2 Dokümantasyon

- Her public API için XML doc
- Her entity için açıklama
- Her use case için senaryo
- Her karar için ADR (Architecture Decision Record)

### 8.3 Quality Gates

| Gate | Koşul | Zorunlu |
|------|-------|---------|
| Build Pass | 0 hata | ✅ |
| Test Pass | %100 başarılı | ✅ |
| Coverage | ≥ %80 | ✅ |
| Code Review | ≥ 1 onay | ✅ |
| Security Scan | 0 kritik | ✅ |
| Style Check | Uyarı yok | ✅ |

---

## 9. Acil Durum Protokolleri

### 9.1 Sistem Hatası

```
1. Hata türünü belirle
2. Etkilenen ajanı tespit et
3. İnsan onayı al
4. Düzeltme yap
5. Test et
6. Devam et
```

### 9.2 Veri Kaybı Riski

```
1. İşlemi durdur
2. Mevcut durumu kaydet
3. İnsan onayı al
4. Kurtarma yap
5. Devam et
```

### 9.3 Acil Durum Kontakları

| Durum | Sorumlu | İletişim |
|-------|---------|----------|
| Sistem Hatası | Build Agent → MO → İnsan | log.md |
| Güvenlik Açığı | MO → İnsan | Dialog |
| Veri Kaybı | MO → İnsan | Dialog |
| Performans | Build Agent → MO | log.md |

---

## 10. Performans Hedefleri

| Metrik | Hedef | Kritik Eşik |
|--------|-------|-------------|
| Yanıt süresi | < 2 saniye | > 5 saniye |
| Agent geçiş süresi | < 500 ms | > 1 saniye |
| Dosya okuma | < 100 ms | > 500 ms |
| Veritabanı sorgusu | < 50 ms | > 200 ms |
| UI yanıt süresi | < 16 ms (60 FPS) | > 32 ms (30 FPS) |
| Memory kullanımı | < 500 MB | > 1 GB |
| CPU kullanımı | < %30 | > %80 |

---

## 11. ProjeSpec Referansı

### 11.1 Spec Dosyaları

| Dosya | İçerik | Konum |
|-------|--------|-------|
| `spec/versacoder-spec.md` | Ana teknik şartname (~2500+ satır) | `.ai/spec/` |
| `spec/versacoder-spec-summary.md` | Şartname özeti (~200 satır) | `.ai/spec/` |
| `spec/index.md` | Spec indeksi | `.ai/spec/` |

### 11.2 Spec Kullanım Akışı

```
Kullanıcı İsteği
  → Spec oku → Karar ver → Kod yaz → Doğrula → Test et → Logla
```

### 11.3 Spec Güncelleme Protokolü

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Spec değişikliğini belirle | Plan Agent |
| 2 | ADR oluştur (gerekirse) | MO |
| 3 | Spec'i güncelle | Build Agent |
| 4 | Vault senkronizasyonu | MO |
| 5 | Audit trail ekle | MO |

---

## 12. Gerçek Kod Durumu (Audit Trail - 2026-08-26)

### 11.1 Çalışan Katmanlar (9/36 Proje)

| Proje | Katman | Satır | Durum |
|-------|--------|-------|-------|
| VersaCoder.Domain | L0 | ~800 | ✅ Gerçek kod - Entity, VO, Event, Interface |
| VersaCoder.Abstractions | L1 | ~600 | ✅ Gerçek kod - 12 Service, 10 Repository, 2 Provider interface |
| VersaCoder.Application | L2 | ~2500 | ✅ Gerçek kod - 11 Service, 6 Command, 8 Handler, 6 Query |
| VersaCoder.CrossCutting | L3 | ~200 | ✅ Gerçek kod - MediatR pipeline behaviors |
| VersaCoder.Infrastructure.Data | L4.1 | ~1200 | ✅ Gerçek kod - DbContext, 10 Repository, 11 Config |
| VersaCoder.Infrastructure.AI | L4.2 | ~800 | ✅ Gerçek kod - 4 Provider, AgentRunner, ToolRegistry |
| VersaCoder.Infrastructure.Logging | L4.28 | ~275 | ✅ Gerçek kod - JSON file logger |
| VersaCoder.Infrastructure.Reporting | L4.29 | ~310 | ✅ Gerçek kod - PDF, Excel export |
| VersaCoder.Host | L6 | ~65 | ✅ Gerçek kod - DI composition root |

### 11.2 Boş Stub Projeler (26 Proje - Class1.cs_only)

| Proje | Katman | Hedef |
|-------|--------|-------|
| VersaCoder.Protocol | L5 | MCP protokolü |
| VersaCoder.Infrastructure.Git | L4.22 | LibGit2Sharp entegrasyonu |
| VersaCoder.Infrastructure.MCP | L4.3 | MCP client/server |
| VersaCoder.Infrastructure.Services | L4.7 | Yardımcı servisler |
| VersaCoder.Infrastructure.Context | L4.14 | Context assembly |
| VersaCoder.Infrastructure.Auth | L4.4 | API key yönetimi |
| VersaCoder.Infrastructure.Security | L4.12 | Şifreleme, token |
| VersaCoder.Infrastructure.Config | L4.5 | Uygulama ayarları |
| VersaCoder.Infrastructure.FileSystem | L4.10 | Dosya sistemi |
| VersaCoder.Infrastructure.Plugins | L4.6 | Plugin sistemi |
| VersaCoder.Infrastructure.Diagram | L4.16 | Diyagram işleme |
| VersaCoder.Infrastructure.Documentation | L4.19 | Otomatik doküman |
| VersaCoder.Infrastructure.Learning | L4.15 | Öğrenme persistansı |
| VersaCoder.Infrastructure.Backup | L4.26 | Yedekleme |
| VersaCoder.Infrastructure.ProjectAnalysis | L4.17 | Proje analizi |
| VersaCoder.Infrastructure.Versioning | L4.27 | Versiyon yönetimi |
| VersaCoder.Infrastructure.Integration | L4.23 | Dış entegrasyon |
| VersaCoder.Infrastructure.Testing | L4.18 | Test altyapısı |
| VersaCoder.Infrastructure.CodeAnalysis | L4.21 | Roslyn/AST |
| VersaCoder.Infrastructure.Observability | L4.13 | Monitoring |
| VersaCoder.Infrastructure.Messaging | L4.9 | Event bus |
| VersaCoder.Infrastructure.Templating | L4.24 | Şablon motoru |
| VersaCoder.Infrastructure.Caching | L4.8 | Önbellek |
| VersaCoder.Infrastructure.Network | L4.11 | HTTP/WebSocket |
| VersaCoder.Infrastructure.Refactoring | L4.20 | Refactoring araçları |
| VersaCoder.Infrastructure.Deployment | L4.25 | Dağıtım |

### 11.3 UI Durumu

| bileşen | Durum |
|---------|-------|
| VersaCoder.UI (L7) | ❌ Boş form - DevExpress kullanılmamış |
| Program.cs | Varsayılan WinForms |
| Form1.cs | Boş - sadece InitializeComponent() |
| DevExpress Ribbon | Henüz eklenmemiş |
| MVVM (CommunityToolkit) | Henüz kullanılmamış |

### 11.4 Kritik Eksikler

| # | Eksik | Öncelik |
|---|-------|---------|
| 1 | UI katmanı (DevExpress WinForms + MDI + Ribbon) | YÜKSEK |
| 2 | MCP protokolü (Protocol projesi) | YÜKSEK |
| 3 | Context yönetimi (vault/file/project context) | YÜKSEK |
| 4 | Git entegrasyonu (LibGit2Sharp) | YÜKSEK |
| 5 | Plugin sistemi | ORTA |
| 6 | Configuration sistemi | YÜKSEK |
| 7 | Auth/Security | ORTA |
| 8 | FileSystem servisleri | YÜKSEK |
| 9 | Caching | ORTA |
| 10 | Network servisleri | ORTA |

### 11.5 csproj Hataları

| Proje | Hata | Düzeltme |
|-------|------|----------|
| Host.csproj | `<PackagePackageReference>` → `<PackageReference>` typo | Düzelt |

---

## 12. Hata Yönetimi Standartları

### 12.1 Hata Seviyeleri

| Seviye | Tanım | Örnek | Aksiyon |
|--------|-------|-------|---------|
| FATAL | Sistem çökmesi | OutOfMemoryException | Log + dump + bildirim |
| ERROR | İşlev kaybı | Database connection failed | Log + retry + eskalasyon |
| WARNING | Performans düşüklüğü | Slow query > 100ms | Log + izleme |
| INFO | Bilgilendirme | Session started | Log |
| DEBUG | Geliştirme bilgisi | SQL sorgusu | Log (geliştirme modunda) |

### 12.2 Hata Formatı

```csharp
// Tüm hatalar bu formatta loglanmalıdır
public class AppException : Exception
{
    public string ErrorCode { get; }
    public string AgentId { get; }
    public string CorrelationId { get; }
    public Dictionary<string, object> Metadata { get; }

    public AppException(string errorCode, string message,
        string agentId, string correlationId)
        : base(message)
    {
        ErrorCode = errorCode;
        AgentId = agentId;
        CorrelationId = correlationId;
        Metadata = new Dictionary<string, object>();
    }
}
```

### 12.3 Hata Kodlama Standartları

| Prefix | Alan | Örnek |
|--------|------|-------|
| DOM- | Domain katmanı | DOM-001, DOM-002 |
| APP- | Application katmanı | APP-001, APP-002 |
| INF- | Infrastructure katmanı | INF-001, INF-002 |
| UI- | UI katmanı | UI-001, UI-002 |
| AI- | AI servisleri | AI-001, AI-002 |
| DB- | Veritabanı | DB-001, DB-002 |
| SEC- | Güvenlik | SEC-001, SEC-002 |
| NET- | Ağ | NET-001, NET-002 |

### 12.4 Retry Politikaları

| Hata Türü | Max Retry | Delay | Backoff |
|-----------|-----------|-------|---------|
| Network timeout | 3 | 1s | Exponential |
| API rate limit | 5 | 30s | Linear |
| Database locked | 2 | 500ms | Fixed |
| Authentication | 0 | - | - |
| File not found | 0 | - | - |

---

## 13. İsimlendirme Standartları

### 13.1 Genel Kurallar

| Öğe | Format | Örnek |
|-----|--------|-------|
| Namespace | PascalCase | `VersaCoder.Domain.Entities` |
| Class | PascalCase | `ChatSession` |
| Interface | I + PascalCase | `IChatSessionRepository` |
| Method | PascalCase | `GetSessionByIdAsync()` |
| Property | PascalCase | `SessionId` |
| Field | _camelCase | `_sessionRepository` |
| Parameter | camelCase | `sessionId` |
| Variable | camelCase | `sessionCount` |
| Constant | PascalCase | `MaxSessionCount` |
| Enum | PascalCase | `SessionStatus.Active` |
| Event | PascalCase + On prefix | `OnSessionCreated` |

### 13.2 Dosya İsimlendirme

| Dosya Tipi | Format | Örnek |
|------------|--------|-------|
| Entity | `{EntityName}.cs` | `ChatSession.cs` |
| Value Object | `{VOName}.cs` | `Message.cs` |
| Repository | `I{Entity}Repository.cs` | `IChatSessionRepository.cs` |
| Service | `{ServiceName}Service.cs` | `ChatSessionService.cs` |
| Handler | `{Action}{Entity}Handler.cs` | `CreateChatSessionHandler.cs` |
| Command | `{Action}{Entity}Command.cs` | `CreateChatSessionCommand.cs` |
| Query | `{Action}{Entity}Query.cs` | `GetChatSessionQuery.cs` |
| DTO | `{Entity}Dto.cs` | `ChatSessionDto.cs` |
| Config | `{Feature}Settings.cs` | `AiSettings.cs` |
| Test | `{ClassName}Tests.cs` | `ChatSessionServiceTests.cs` |

### 13.3 Kategori Önekleri

| Önek | Kullanım Alanı |
|------|----------------|
| I | Arayüzler (IRepository, IService) |
| Abstract | Soyut sınıflar (AbstractService) |
| Base | Temel sınıflar (BaseEntity, BaseHandler) |
| Async | Async metotlar (GetAsync, SaveAsync) |
| Impl | Somut sınıflar (RepositoryImpl) |
| Extensions | Genişletme metotları (ServiceExtensions) |

---

## 14. Test Stratejisi

### 14.1 Test Piramidi

```
         ┌─────────┐
         │   E2E   │  %10
         ├─────────┤
         │Entegrasyon│  %20
         ├─────────┤
         │  Unit   │  %70
         └─────────┘
```

### 14.2 Test Kapsama Hedefleri

| Katman | Min Kapsama | Kapsam Alanı |
|--------|-------------|--------------|
| L0 Domain | %95 | Entity, VO, Event, Domain Service |
| L1 Abstractions | N/A | Interface tanımları |
| L2 Application | %90 | Handler, Service, Validator |
| L3 CrossCutting | %85 | Behavior, Middleware |
| L4 Infrastructure | %80 | Repository, Service, Provider |
| L5 Protocol | %75 | MCP, AI Protocol |
| L6 Host | %70 | DI, Configuration |
| L7 UI | %60 | ViewModel, Command, Validation |

### 14.3 Test Dosya Yapısı

```
tests/
├── VersaCoder.Domain.Tests/           # L0 unit tests
├── VersaCoder.Application.Tests/      # L2 unit tests
├── VersaCoder.Infrastructure.Tests/   # L4 unit + integration tests
├── VersaCoder.CrossCutting.Tests/     # L3 unit tests
├── VersaCoder.IntegrationTests/       # Entegrasyon testleri
└── VersaCoder.E2ETests/              # Uçtan uca testler
```

### 14.4 Test İsimlendirme Konvansiyonu

```csharp
// Metot adı + Senaryo + Beklenen sonuç
[Fact]
public async Task GetSessionByIdAsync_ValidId_ReturnsSession()
{
    // Arrange
    // Act
    // Assert
}

[Theory]
[InlineData("")]
[InlineData(null)]
public async Task GetSessionByIdAsync_InvalidId_ThrowsValidationException(string id)
{
    // Arrange
    // Act
    // Assert
}
```

### 14.5 Test Araçları

| Araç | Amaç | Kullanım |
|------|------|----------|
| xUnit | Test framework | Tüm testler |
| Moq | Mocking | Dependency’ler |
| FluentAssertions | Assertion | Okunabilir assertion |
| Bogus | Fake data | Test verileri |
| Testcontainers | Container | DB integration testleri |
| Coverlet | Code coverage | Kapsama analizi |

---

## 15. Versiyon Kontrol Protokolü

### 15.1 Branch Stratejisi

```
main (production)
  └── develop (integration)
       ├── feature/TASK-001-add-session
       ├── feature/TASK-002-add-ai-chat
       ├── bugfix/TASK-003-fix-login
       └── hotfix/TASK-004-security-patch
```

### 15.2 Commit Formatı

```
<type>(<scope>): <description>

Type:
  feat     — Yeni özellik
  fix      — Hata düzeltme
  docs     — Dokümantasyon
  style    — Kod stili (mantıksal değişiklik yok)
  refactor — Yeniden düzenleme
  test     — Test ekleme/düzeltme
  chore    — Bakım görevleri

Örnek:
feat(session): CreateSession handler implementasyonu eklendi
fix(ai): API timeout hatası düzeltildi
docs(readme): Kurulum talimatları güncellendi
```

### 15.3 Pull Request Kuralları

| Kural | Açıklama |
|-------|----------|
| Min 1 review | En az 1 kişi onaylamalı |
| passing CI | Tüm testler geçmeli |
| No merge conflicts | Çakışma olmamalı |
| Linked issue | Issue bağlanmalı |
| Description | Detaylı açıklama zorunlu |

### 15.4 Git Hook Zorunlulukları

| Hook | Amaç | Zorunlu |
|------|------|---------|
| pre-commit | Lint + format | ✅ |
| commit-msg | Commit format check | ✅ |
| pre-push | Test çalıştır | ✅ |
| post-merge | Vault sync | ✅ |

---

## 16. İletişim Protokolü

### 16.1 Mesaj Formatları

```json
{
  "type": "handover|escalation|status|error",
  "sourceAgent": "build",
  "targetAgent": "plan",
  "priority": "HIGH|MEDIUM|LOW",
  "correlationId": "uuid",
  "timestamp": "ISO8601",
  "payload": {}
}
```

### 16.2 Bildirim Türleri

| Tür | Tetikleyici | Hedef |
|-----|------------|-------|
| Task Assigned | Görev atandı | İlgili agent |
| Task Completed | Görev tamamlandı | MO + kullanıcı |
| Task Failed | Görev başarısız | MO + escalation |
| Handover Request | Transfer isteği | Hedef agent |
| Health Warning | Sağlık sorunu | MO |
| Security Alert | Güvenlik ihlali | MO + kullanıcı |

### 16.3 Loglama Formatı

```
[2026-08-26T12:00:00Z] [INFO] [build] [TASK-001] ChatSession entity oluşturuldu
[2026-08-26T12:00:01Z] [ERROR] [ai] [TASK-002] OpenAI API timeout - retry 1/3
[2026-08-26T12:00:02Z] [WARN] [db] [TASK-003] Slow query detected: 150ms
```

---

## 17. Monitoring & Observability

### 17.1 Metrikler

| Metrik | Tanım | Hedef |
|--------|-------|-------|
| request_duration | İstek süresi | < 2s |
| agent_task_duration | Agent görev süresi | < 30s |
| error_rate | Hata oranı | < %1 |
| active_sessions | Aktif session sayısı | < 10 |
| token_usage | Token kullanımı | İzleme |
| db_query_duration | DB sorgu süresi | < 50ms |

### 17.2 Health Check Formatı

```json
{
  "status": "healthy|degraded|unhealthy",
  "timestamp": "ISO8601",
  "checks": {
    "database": { "status": "healthy", "latency": "5ms" },
    "ai_providers": { "status": "degraded", "openai": "ok", "anthropic": "timeout" },
    "file_system": { "status": "healthy", "free_space": "50GB" }
  }
}
```

### 17.3 Dashboard Bileşenleri

| Bileşen | İçerik | Güncelleme |
|---------|--------|-----------|
| System Status | Genel durum | Gerçek zamanlı |
| Active Sessions | Aktif oturumlar | 5s |
| Token Usage | Token kullanımı | 1min |
| Error Log | Son hatalar | Gerçek zamanlı |
| Performance | Metrikler | 30s |
| Agent Status | Agent durumları | 10s |

---

## 18. Yedekleme & Kurtarma

### 18.1 Yedekleme Politikası

| Veri | Sıklık | Saklama | Método |
|------|--------|---------|--------|
| Veritabanı | Günlük | 30 gün | SQLite backup |
| Vault dosyaları | Her commit | Sonsuz | Git |
| Session logları | Günlük | 90 gün | Dosya kopyalama |
| API anahtarları | Değişimde | Sonsuz | Şifreli dosya |
| Konfigürasyon | Değişimde | 30 gün | Version control |

### 18.2 Kurtarma Prosedürü

| Senaryo | RTO | RPO | Adımlar |
|---------|-----|-----|---------|
| DB bozulması | 5dk | 1dk | Backup’tan geri yükle |
| Vault kaybı | 1dk | 0 | Git’ten geri al |
| Dosya silinmesi | 5dk | 1dk | Geri dönüşüm + version |
| API key sızıntısı | Anlık | 0 | Key rotasyonu + audit |
| Sistem çökmesi | 15dk | 5dk | Full restore |

---

## 19. Geliştirme Aşamaları (Referans)

### 19.1 5 Aşamalı Plan

| Aşama | Kapsam | Öncelik | Tahmini |
|-------|--------|---------|---------|
| FAZ 1 | Altyapı servisleri (Config, FileSystem, Auth, Security, DB Migration) | YÜKSEK | 2-3 hafta |
| FAZ 2 | UI katmanı (DevExpress WinForms + MDI + Ribbon + MVVM) | YÜKSEK | 3-4 hafta |
| FAZ 3 | Protokol & Entegrasyon (MCP, Protocol, Git, Plugin) | ORTA | 2-3 hafta |
| FAZ 4 | Ek modüller (Caching, Messaging, Network, vb.) | ORTA | 2-3 hafta |
| FAZ 5 | Test & Optimizasyon | YÜKSEK | 1-2 hafta |

### 19.2 FAZ 1 Alt Görevleri

| # | Görev | Proje | Öncelik |
|---|-------|-------|---------|
| 1.1 | Host.csproj typo düzelt | Host | YÜKSEK |
| 1.2 | Infrastructure.Config kur | Config | YÜKSEK |
| 1.3 | Infrastructure.FileSystem kur | FileSystem | YÜKSEK |
| 1.4 | Infrastructure.Auth kur | Auth | ORTA |
| 1.5 | Infrastructure.Security kur | Security | ORTA |
| 1.6 | EF Core migration oluştur | Data | YÜKSEK |

---

## 20. Versions & Changelog

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-26 | İlk sürüm, tüm guardrails tanımlandı |
| 1.1.0 | 2026-08-26 | Vault enhance - Gerçek kod audit trail eklendi |
| 1.2.0 | 2026-08-26 | Enhanced - Hata yönetimi, isimlendirme, test, monitoring, backup bölümleri eklendi |
| 1.3.0 | 2026-08-26 | ProjeSpec referansı eklendi, spec dosyaları oluşturuldu |

---

## 12. Ek Protokoller

### 12.1 Ultra Düşünme Protokolü

Tüm agent'lar kod yazmadan önce bu protokolü uygulamak ZORUNDADIR:

| Adım | Kontrol | Kaynak |
|------|---------|--------|
| 1 | Vault Oku | CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md |
| 2 | Bağlamı Anla | Domain, katman, dosyalar |
| 3 | Hata Kontrolü | Syntax, imports, types |
| 4 | Sonuç Tahmini | Etki alanı, edge cases |
| 5 | Doğrulama | LSP, typecheck, test |

### 12.2 Handover Protokolü

```
[Kaynak Agent] → [Handover Request] → [Hedef Agent] → [Onay/Red] → [Confirmation]
```

### 12.3 Eskalasyon Protokolü

```
Level 1 (Domain Lead) → Level 2 (Tech Lead) → Level 3 (Arch Lead) → İnsan
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode