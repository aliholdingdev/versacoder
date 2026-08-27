---
title: "Versa Coder — Mühendislik Süreçleri & Workflow Protokolü"
type: guide
category: engineering-workflow
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/WORKFLOW.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/AGENTS.md · .ai/WORKFLOW.md · .ai/brain.md"
---

# Versa Coder — Mühendislik Süreçleri & Workflow Protokolü

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[brain.md]] · [[index.md]] · [[keys.md]]

---

## 1. Amaç

Versa Coder geliştirme sürecindeki tüm mühendislik süreçlerini, workflow'ları, kalite kontrol mekanizmalarını ve standsartları tanımlayan **Tek Doğruluk Kaynağıdır (SSOT)**.

---

## 2. Geliştirme Metodolojisi

### 2.1 Agile + Scrum Hybrid

| Kavram | Uygulama |
|--------|----------|
| Sprint Süresi | 2 hafta |
| Planning | Her sprint başında |
| Daily Standup | Günde 1 (agent health check) |
| Review | Sprint sonunda |
| Retrospective | Her 4 sprintte bir |
| Backlog Grooming | Haftada 1 |

### 2.2 Development Flow

```
[1. Requirements] → [2. Design] → [3. Plan] → [4. Implementation] → [5. Testing] → [6. Review] → [7. Deploy]
```

---

## 3. Görev Yönetimi

### 3.1 Görev Türleri

| Görev Tipi | Açıklama | Öncelik | Süre |
|------------|----------|---------|------|
| Feature | Yeni özellik | MEDIUM | 1-5 gün |
| Bug | Hata düzeltme | HIGH | 0.5-2 gün |
| Improvement | İyileştirme | LOW | 1-3 gün |
| Architecture | Mimari değişiklik | CRITICAL | 2-7 gün |
| Documentation | Dokümantasyon | LOW | 0.5-2 gün |
| Test | Test yazma | MEDIUM | 1-3 gün |
| Refactoring | Kod yeniden yapılandırma | MEDIUM | 1-5 gün |
| Security | Güvenlik düzeltmesi | CRITICAL | 0.5-3 gün |
| Performance | Performans optimizasyonu | HIGH | 1-5 gün |

### 3.2 Görev Döngüsü

```
[Backlog] → [To Do] → [In Progress] → [In Review] → [Done]
                                ↓
                          [Blocked]
```

### 3.3 Görev Durum Tanımları

| Durum | Kod | Tanım |
|-------|-----|-------|
| Backlog | BLG | Henüz planlanmamış |
| To Do | TD | Planlanmış, bekliyor |
| In Progress | IP | Devam ediyor |
| In Review | IR | İncelemede |
| Done | DN | Tamamlandı |
| Blocked | BLK | Engellendi |
| Cancelled | CNL | İptal edildi |

---

## 4. Kod Kalite Süreçleri

### 4.1 Code Review Prosedürü

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Kodu yaz | Build Agent |
| 2 | Kendi kendine incele | Build Agent |
| 3 | Lint kontrolü yap | Build Agent |
| 4 | Testleri çalıştır | Build Agent |
| 5 | PR oluştur | Build Agent |
| 6 | İncele | MO / İnsan |
| 7 | Onay ver veya düzelt | MO / İnsan |
| 8 | Merge yap | Build Agent |

### 4.2 Code Review Kontrol Listesi

```markdown
## Code Review Checklist

### Genel
- [ ] Kod temiz ve okunabilir mi?
- [ ] Yorumlar yeterli mi?
- [ ] Hata yönetimi doğru mu?
- [ ] Logging yeterli mi?

### Mimari
- [ ] Layer kuralına uygun mu?
- [ ] SOLID prensiplerine uygun mu?
- [ ] Bağımlılık enjeksiyonu doğru mu?
- [ ] Interface kullanımı yeterli mi?

### Güvenlik
- [ ] SQL enjeksiyonu riski var mı?
- [ ] Hassas veriler loglanıyor mu?
- [ ] API anahtarları açık kodda mı?
- [ ] Input validation var mı?

### Performans
- [ ] Gereksiz memory allocation var mı?
- [ ] Async/await doğru kullanılmış mı?
- [ ] Database query optimal mi?
- [ ] Caching stratejisi doğru mu?

### Test
- [ ] Unit test yazıldı mı?
- [ ] Edge cases test edildi mi?
- [ ] Integration test yazıldı mı?
- [ ] Test coverage yeterli mi?
```

### 4.3 Statik Analiz

| Araç | Amaç | Zorunlu |
|------|------|---------|
| Roslyn | C# code analysis | ✅ |
| StyleCop | Kod stili | ✅ |
| SonarQube | Kalite metrikleri | ✅ |
| Security Code Scan | Güvenlik taraması | ✅ |

---

## 5. Test Stratejisi

### 5.1 Test Piramidi

```
        /\
       /  \  E2E Tests (10%)
      /----\
     /      \ Integration Tests (20%)
    /--------\
   /          \ Unit Tests (70%)
  /------------\
```

### 5.2 Test Türleri

| Test Tipi | Amaç | Kapsama | Tool |
|-----------|------|---------|------|
| Unit Test | Bileşen testi | %70 | xUnit |
| Integration Test | Bileşenler arası | %20 | xUnit + Testcontainers |
| E2E Test | Uçtan uca | %10 | Playwright |
| Performance Test | Performans | Kritik yollar | BenchmarkDotNet |
| Security Test | Güvenlik | Tüm API | Security Code Scan |

### 5.3 Test İsimlendirme

```csharp
// Pattern: MethodName_Scenario_ExpectedResult
[Fact]
public void CreateSession_WithValidInput_ReturnsSessionId()
{
    // Arrange
    // Act
    // Assert
}

[Theory]
[InlineData("")]
[InlineData(null)]
public void CreateSession_WithInvalidInput_ThrowsValidationException(string name)
{
    // Arrange
    // Act
    // Assert
}
```

### 5.4 Test Coverage Hedefleri

| Katman | Minimum Kapsama |
|--------|-----------------|
| Domain | %90 |
| Application | %85 |
| Infrastructure | %75 |
| CrossCutting | %80 |
| Protocol | %70 |
| UI | %60 |
| **Genel** | **%80** |

---

## 6. Versiyonlama

### 6.1 Semantic Versioning

```
MAJOR.MINOR.PATCH

MAJOR: Breaking changes
MINOR: Backward-compatible features
PATCH: Backward-compatible bug fixes
```

### 6.2 Version Örnekleri

| Version | Değişiklik |
|---------|-----------|
| 1.0.0 | İlk production sürümü |
| 1.1.0 | Yeni özellik eklendi |
| 1.1.1 | Hata düzeltildi |
| 2.0.0 | Breaking change |

### 6.3 Changelog Formatı

```markdown
## [1.1.0] - 2026-08-25

### Added
- Session branching feature
- Learning system

### Changed
- Improved AI response handling

### Fixed
- Memory leak in provider router

### Removed
- Deprecated legacy provider
```

---

## 7. Branching Stratejisi

### 7.1 Git Flow

```
main (production)
  ↑
develop (integration)
  ↑
feature/xxx
hotfix/xxx
release/xxx
```

### 7.2 Branch Kuralları

| Branch | Amaç | Ömür | Merge |
|--------|------|------|-------|
| main | Production | Kalıcı | — |
| develop | Integration | Kalıcı | feature → develop |
| feature/* | Yeni özellik | Geçici | → develop |
| hotfix/* | Acil düzeltme | Geçici | → main + develop |
| release/* | Release hazırlık | Geçici | → main + develop |

### 7.3 Commit Mesajı Formatı

```
type(scope): description

type: feat, fix, docs, style, refactor, test, chore
scope: domain, application, infrastructure, ui, etc.
description: Kısa açıklama (max 50 karakter)
```

---

## 8. CI/CD Süreçleri

### 8.1 Pipeline Adımları

```
[1. Build] → [2. Test] → [3. Analyze] → [4. Security Scan] → [5. Package] → [6. Deploy]
```

### 8.2 Build Pipeline

| Adım | Araç | Başarı Koşulu |
|------|------|---------------|
| Restore | dotnet restore | 0 hata |
| Build | dotnet build | 0 hata, 0 uyarı |
| Test | dotnet test | %80覆盖 |
| Analyze | SonarQube | Quality Gate pass |
| Security | Security Code Scan | 0 kritik hata |
| Package | dotnet publish | Başarılı |
| Deploy | Docker / Manual | Başarılı |

### 8.3 Deployment Stratejisi

| Ortam | Trigger | Manuel Onay |
|-------|---------|-------------|
| Development | develop push | Hayır |
| Staging | release push | Evet |
| Production | main push | Evet (2 kişi) |

---

## 9. Dokümantasyon Standartları

### 9.1 Doküman Türleri

| Doküman | İçerik | Güncelleme |
|---------|--------|------------|
| README.md | Proje tanıtımı | Feature eklendiğinde |
| ARCHITECTURE.md | Mimari yapı | Mimari değişiklikte |
| API.md | API referansı | Endpoint değiştiğinde |
| CHANGELOG.md | Değişiklik kaydı | Her release'te |
| CONTRIBUTING.md | Katkı rehberi | Gerektiğinde |
| SECURITY.md | Güvenlik politikası | Gerektiğinde |

### 9.2 README Formatı

```markdown
# Project Name

## Overview
## Features
## Requirements
## Installation
## Usage
## Architecture
## Contributing
## License
```

### 9.3 API Dokümantasyonu

```csharp
/// <summary>
/// Yeni bir session oluşturur.
/// </summary>
/// <param name="request">Session oluşturma isteği</param>
/// <returns>Oluşturulan session'ın ID'si</returns>
/// <exception name="ValidationException">Geçersiz input durumunda</exception>
/// <exception name="DuplicateException">Mevcut session durumunda</exception>
public async Task<SessionId> CreateSession(CreateSessionRequest request)
{
    // Implementation
}
```

---

## 10. Güvenlik Süreçleri

### 10.1 Güvenlik Kontrol Listesi

```markdown
## Security Checklist

### Giriş Doğrulama
- [ ] Tüm inputlar validate ediliyor mu?
- [ ] SQL enjeksiyonu koruması var mı?
- [ ] XSS koruması var mı?
- [ ] CSRF koruması var mı?

### Yetkilendirme
- [ ] Roller doğru tanımlanmış mı?
- [ ] Erişim kontrolleri uygulanıyor mu?
- [ ] Hassas veriler korunuyor mu?

### Veri Güvenliği
- [ ] Hassas veriler şifreleniyor mu?
- [ ] Loglarda hassas veri yok mu?
- [ ] API anahtarları güvenli mi?
- [ ] Veritabanı bağlantısı güvenli mi?

### Network
- [ ] HTTPS kullanılıyor mu?
- [ ] CORS doğru yapılandırılmış mı?
- [ ] Rate limiting var mı?
```

### 10.2 Vulnerability Management

| Severity | Response Time | Fix Deadline |
|----------|---------------|--------------|
| Critical | 1 saat | 24 saat |
| High | 4 saat | 72 saat |
| Medium | 1 gün | 1 hafta |
| Low | 1 hafta | 1 ay |

---

## 11. Performance Monitoring

### 11.1 Performans Metrikleri

| Metrik | Hedef | Kritik Eşik |
|--------|-------|-------------|
| API Response Time | < 200ms | > 1000ms |
| Database Query | < 50ms | > 200ms |
| Memory Usage | < 500MB | > 1GB |
| CPU Usage | < 30% | > 80% |
| Error Rate | < 0.1% | > 1% |
| Uptime | %99.9 | < %99 |

### 11.2 Alerting Kuralları

| Alert | Koşul | Aksiyon |
|-------|-------|---------|
| High Response Time | > 1s (5 dk) | Bildirim |
| High Error Rate | > 1% (5 dk) | Bildirim + Escalation |
| High Memory | > 1GB (10 dk) | Restart |
| Service Down | 3 consecutive fails | Auto-restart + Bildirim |

---

## 12. Backup & Recovery

### 12.1 Backup Stratejisi

| Veri | Sıklık | Saklama | Yöntem |
|------|--------|---------|--------|
| Veritabanı | Günlük | 30 gün | SQLite backup |
| Konfigürasyon | Her değişiklik | Kalıcı | Git |
| User Data | Günlük | 90 gün | Encrypted backup |
| Logs | Günlük | 30 gün | Rotating files |

### 12.2 Recovery Prosedürü

```
[1. Veri kaybını tespit et] → [2. Etkilenen veriyi belirle] → [3. Backup'tan kurtar] → [4. Doğrula] → [5. Devam et]
```

---

## 13. Communication Protocols

### 13.1 Agent İletişim Formatı

```json
{
  "type": "HANDOVER | ESCALATION | STATUS_UPDATE | HEALTH_CHECK",
  "from": "agent_id",
  "to": "agent_id | mo | human",
  "priority": "CRITICAL | HIGH | MEDIUM | LOW",
  "payload": {
    "subject": "Brief description",
    "details": "Detailed information",
    "affectedFiles": ["file1.cs", "file2.cs"],
    "timestamp": "2026-08-25T12:00:00Z"
  }
}
```

### 13.2 Bildirim Kanalları

| Kanal | Kullanım | Öncelik |
|-------|----------|---------|
| log.md | Audit trail | Tümü |
| Console | Debug bilgisi | LOW |
| Dialog | İnsan onayı | HIGH |
| Alert | Kritik hatalar | CRITICAL |

---

## 14. Learning & Improvement

### 14.1 Retrospective Formatı

```markdown
## Retrospective - [Tarih]

### Ne iyi gitti?
- [Liste]

### Ne geliştirilebilir?
- [Liste]

### Aksiyon maddeleri
- [ ] [Sorumlu] [Aksiyon] [Deadline]
```

### 14.2 Knowledge Base

| Kategori | İçerik | Güncelleme |
|----------|--------|------------|
| Patterns | Yaygın kullanılan kalıplar | Yeni pattern keşfedildiğinde |
| Anti-patterns | Kaçınılması gereken kalıplar | Hata düzeltildiğinde |
| Lessons Learned | Deneyimler | Her retrospective |
| Best Practices | En iyi uygulamalar | Sürekli |

---

## 15. Quality Gates

### 15.1 Merge Öncesi

| Gate | Koşul | Zorunlu |
|------|-------|---------|
| Build Pass | 0 hata | ✅ |
| Test Pass | %100 başarılı | ✅ |
| Coverage | ≥ %80 | ✅ |
| Code Review | ≥ 1 onay | ✅ |
| Security Scan | 0 kritik | ✅ |
| Style Check | Uyarı yok | ✅ |

### 15.2 Release Öncesi

| Gate | Koşul | Zorunlu |
|------|-------|---------|
| All Tests Pass | %100 | ✅ |
| Performance Test | Hedeflerin altında | ✅ |
| Security Audit | Onaylı | ✅ |
| Documentation | Güncel | ✅ |
| Changelog | Güncel | ✅ |
| Version Bump | Yapıldı | ✅ |

---

## 16. Proje Yaşam Döngüsü

### 16.1 Faz Tanımları

| Faz | Amaç | Çıktı | Süre |
|-----|------|-------|------|
| Inception | Vizyon tanımı | Proje charter'ı | 1 hafta |
| Elaboration | Mimari plan | ADR'ler, prototip | 2 hafta |
| Construction | Geliştirme | Working software | 8-12 hafta |
| Transition | Dağıtım | Production-ready | 1-2 hafta |

### 16.2 Milestone Tanımları

| Milestone | Kriter | Sonraki Faz |
|-----------|--------|-------------|
| M1: Vision Approved | Proje onaylandı | Elaboration |
| M2: Architecture Baseline | Mimari kararlar alındı | Construction |
| M3: Core Features | Temel özellikler tamamlandı | Construction |
| M4: Beta Release | Beta dağıtımı | Transition |
| M5: Production | Üretim dağıtımı | Maintenance |

### 16.3 Risk Yönetimi

| Risk Seviyesi | Aksiyon | Sorumlu |
|---------------|---------|---------|
| Critical | Derhal durdur + insan onayı | MO → İnsan |
| High | Alternatif plan oluştur | Plan Agent |
| Medium | İzle + raporla | Build Agent |
| Low | Dokümante et | Summary Agent |

---

## 17. Entegrasyon Testleri

### 17.1 Entegrasyon Test Stratejisi

| Test Türü | Amaç | Araç | Sıklık |
|-----------|------|------|--------|
| API Test | Endpoint doğrulama | xUnit + HttpClient | Her PR |
| Database Test | Veri bütünlüğü | Testcontainers | Her PR |
| AI Provider Test | Provider entegrasyonu | Mock + Real | Haftada 1 |
| UI Test | Kullanıcı akışı | Playwright | Sprint sonu |

### 17.2 Test Verisi Yönetimi

```csharp
public class TestDataBuilder
{
    public static Session CreateTestSession() => new()
    {
        Id = SessionId.New(),
        Name = "Test Session",
        CreatedAt = DateTime.UtcNow,
        Status = SessionStatus.Active
    };
    
    public static Message CreateTestMessage(SessionId sessionId) => new()
    {
        Id = MessageId.New(),
        SessionId = sessionId,
        Content = "Test message",
        Role = MessageRole.User,
        CreatedAt = DateTime.UtcNow
    };
}
```

### 17.3 Mock Stratejisi

| Bileşen | Mock Türü | Kullanım |
|---------|-----------|----------|
| ILLMProvider | In-Memory Mock | Unit test |
| IRepository | In-Memory Mock | Unit test |
| IDbContext | In-Memory SQLite | Integration test |
| HttpClient | WireMock | External API test |

---

## 18. Dokümantasyon Yaşam Döngüsü

### 18.1 Doküman Versiyonlama

| Doküman | Versiyonlama | Güncelleme |
|---------|-------------|------------|
| README.md | Git tag | Her release |
| ARCHITECTURE.md | ADR-based | Mimari değişiklikte |
| API.md | OpenAPI spec | Endpoint değiştiğinde |
| CHANGELOG.md | SemVer | Her release'te |
| CONTRIBUTING.md | Manuel | Gerektiğinde |

### 18.2 API Dokümantasyonu

```csharp
/// <summary>
/// Yeni bir session oluşturur.
/// </summary>
/// <param name="request">Session oluşturma isteği</param>
/// <returns>Oluşturulan session'ın ID'si</returns>
/// <exception name="ValidationException">Geçersiz input durumunda</exception>
/// <exception name="DuplicateException">Mevcut session durumunda</exception>
/// <example>
/// POST /api/sessions
/// {
///   "name": "My Session",
///   "projectId": "proj-123"
/// }
/// </example>
public async Task<SessionId> CreateSession(CreateSessionRequest request)
{
    // Implementation
}
```

### 18.3 Şablon Kütüphanesi

| Şablon | Kullanım | İçerik |
|--------|----------|--------|
| Entity Template | Yeni varlık | Class, property, validation |
| Repository Template | Veri erişimi | Interface, implementation |
| Handler Template | CQRS handler | Command/Query handler |
| Service Template | İş mantığı | Interface, implementation |
| Test Template | Unit test | Arrange, Act, Assert |

---

## 19. Monitoring & Observability

### 19.1 Three Pillars

| Pillar | Araç | Kullanım |
|--------|------|----------|
| Logs | Serilog | Olay kayıtları |
| Metrics | Custom | Performans metrikleri |
| Traces | Custom | İşlem takibi |

### 19.2 Health Check Endpoint

```csharp
services.AddHealthChecks()
    .AddSQLite("Data Source=versacoder.db", name: "database")
    .AddCheck<ProviderHealthCheck>("ai-provider")
    .AddCheck<MemoryHealthCheck>("memory");
```

### 19.3 Alerting Kuralları

| Alert | Koşul | Aksiyon | Timeout |
|-------|-------|---------|---------|
| High Response Time | > 1s (5 dk) | Bildirim | 15 dk |
| High Error Rate | > 1% (5 dk) | Bildirim + Escalation | 30 dk |
| High Memory | > 1GB (10 dk) | Restart | 5 dk |
| Service Down | 3 consecutive fails | Auto-restart + Bildirim | Anlık |
| Database Slow | > 200ms (10 dk) | Uyarı | 1 saat |

### 19.4 Log Stratejisi

```csharp
// Structured logging ile Serilog
Log.Information("Session created: {SessionId} for project {ProjectId}", 
    sessionId, projectId);

// Output: {"SessionId":"abc-123","ProjectId":"proj-456","Message":"Session created: abc-123 for project proj-456"}
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode