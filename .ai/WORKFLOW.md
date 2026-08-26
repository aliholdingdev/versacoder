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

## 8. CI/CD & Deployment Süreçleri

### 8.1 GitHub Actions Pipeline

#### 8.1.1 Build Workflow (develop/feature push)

```yaml
name: Build & Test
on:
  push:
    branches: [develop, 'feature/**']
  pull_request:
    branches: [develop]

jobs:
  build:
    runs-on: windows-latest
    strategy:
      matrix:
        dotnet-version: ['8.0.x']

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ matrix.dotnet-version }}

    - name: Cache NuGet packages
      uses: actions/cache@v4
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: ${{ runner.os }}-nuget-

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Test
      run: dotnet test --no-build --configuration Release --collect:"XPlat Code Coverage"

    - name: Upload coverage
      uses: codecov/codecov-action@v4
      with:
        files: '**/coverage.cobertura.xml'
```

#### 8.1.2 Release Workflow (main push)

```yaml
name: Release
on:
  push:
    branches: [main]

jobs:
  release:
    runs-on: windows-latest
    environment: production

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    - name: Build
      run: dotnet build --configuration Release

    - name: Test
      run: dotnet test --configuration Release

    - name: Publish
      run: dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj -c Release -o ./publish

    - name: Docker Build & Push
      run: |
        docker build -t versacoder:${{ github.sha }} .
        docker tag versacoder:${{ github.sha }} versacoder:latest
        docker push versacoder:${{ github.sha }}
        docker push versacoder:latest

    - name: Create Release
      uses: softprops/action-gh-release@v2
      with:
        tag_name: v${{ github.run_number }}
        generate_release_notes: true
```

#### 8.1.3 Nightly Build Workflow

```yaml
name: Nightly Build
on:
  schedule:
    - cron: '0 2 * * *'  # Her gece 02:00 UTC

jobs:
  nightly:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    - name: Full Build & Test
      run: |
        dotnet restore
        dotnet build --configuration Release
        dotnet test --configuration Release --logger trx
    - name: SonarQube Analysis
      run: |
        dotnet sonarscanner begin /k:"versacoder-nightly" /d:sonar.host.url="${{ secrets.SONAR_HOST }}"
        dotnet build --configuration Release
        dotnet sonarscanner end
```

### 8.2 Build Pipeline Aşamaları

```
[1. Restore] → [2. Build] → [3. Test] → [4. Analyze] → [5. Security Scan] → [6. Package] → [7. Deploy]
```

| Adım | Araç | Komut | Başarı Koşulu | Timeout |
|------|------|-------|---------------|---------|
| Restore | NuGet | `dotnet restore` | 0 hata | 5 dk |
| Build | MSBuild | `dotnet build -c Release` | 0 hata, 0 uyarı | 10 dk |
| Test | xUnit | `dotnet test -c Release` | %80+ coverage | 15 dk |
| Analyze | Roslyn/SonarQube | `dotnet format --verify-no-changes` | Quality Gate pass | 10 dk |
| Security | Snyk/Trivy | Security scan | 0 kritik hata | 10 dk |
| Package | dotnet | `dotnet publish -c Release` | Başarılı | 5 dk |
| Deploy | Docker/Manual | Container build + push | Başarılı | 10 dk |

### 8.3 Docker Containerization

#### 8.3.1 Multi-stage Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/VersaCoder.Domain/*.csproj src/VersaCoder.Domain/
COPY src/VersaCoder.Abstractions/*.csproj src/VersaCoder.Abstractions/
COPY src/VersaCoder.Application/*.csproj src/VersaCoder.Application/
COPY src/VersaCoder.CrossCutting/*.csproj src/VersaCoder.CrossCutting/
COPY src/VersaCoder.Infrastructure.Data/*.csproj src/VersaCoder.Infrastructure.Data/
COPY src/VersaCoder.Infrastructure.AI/*.csproj src/VersaCoder.Infrastructure.AI/
COPY src/VersaCoder.Host/*.csproj src/VersaCoder.Host/

RUN dotnet restore
COPY . .
RUN dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "VersaCoder.Host.dll"]
```

#### 8.3.2 Docker Compose (Development)

```yaml
version: '3.8'
services:
  versacoder:
    build: .
    ports:
      - "8080:8080"
    volumes:
      - ./data:/app/data
      - ./logs:/app/logs
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Data Source=/app/data/versacoder.db
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 5s
      retries: 3
```

### 8.4 Deployment Stratejileri

| Strateji | Açıklama | Avantaj | Dezavantaj |
|----------|----------|---------|------------|
| Blue-Green | İki tam kopya, geçiş anlık | Sıfır downtime, kolay rollback | 2x kaynak |
| Canary | Yeni versiyon kademeli | Risk azaltma, geri bildirim | Karmaşık routing |
| Rolling | Kademeli güncelleme | Az kaynak | Yavaş geçiş |
| Recreate | Eski sil, yeni kur | Basit | Downtime var |

#### 8.4.1 Blue-Green Deployment Akışı

```
Mevcut (Blue) ←── Load Balancer ──→ Yeni (Green)
                    ↓
            Health Check OK
                    ↓
            Traffic Switch
                    ↓
            Blue → Standby (rollback için)
```

#### 8.4.2 Rollback Prosedürü

| Adım | Aksiyon | Sorumlu | Süre |
|------|---------|---------|------|
| 1 | Sorunu tespit et | Monitoring alert | Anlık |
| 2 | Rollback karar ver | Tech Lead | 5 dk |
| 3 | Traffic'ı Blue'ya çevir | DevOps | 1 dk |
| 4 | Green'ı durdur | DevOps | 1 dk |
| 5 | Database rollback (gerekirse) | DBA | 5-30 dk |
| 6 | Doğrulama yap | QA | 10 dk |
| 7 | Incident report oluştur | Sorumlu | 30 dk |

### 8.5 Release Management

#### 8.5.1 Semantic Versioning

```
MAJOR.MINOR.PATCH

MAJOR: Breaking API changes veya büyük mimari değişiklikler
MINOR: Backward-compatible yeni özellikler
PATCH: Backward-compatible bug düzeltmeleri

Örnek: 1.2.3
  1 → İlk major release
  2 → İkinci minor özellik eklentisi
  3 → Üçüncü patch düzeltmesi
```

#### 8.5.2 Changelog Formatı

```markdown
## [1.2.0] - 2026-08-26

### Added
- SignalR real-time entegrasyonu
- Sektörel agent desteği (60+ sektör)
- GitHub Actions CI/CD pipeline

### Changed
- UI performans optimizasyonu
- Veritabanı query optimizasyonu

### Fixed
- Session branching hatası düzeltildi
- Memory leak giderildi

### Deprecated
- Eski provider API (v1) — v2.0'da kaldırılacak

### Security
- JWT token yenileme mechanizması güçlendirildi
```

### 8.6 Environment Yönetimi

| Ortam | Database | API Keys | Log Level | Monitoring |
|-------|----------|----------|-----------|------------|
| Development | SQLite (local) | Test keys | Debug | Console |
| Staging | SQLite (shared) | Staging keys | Information | Grafana |
| Production | SQLite (WAL) | Prod keys (vault) | Warning | Grafana + Alerting |

#### 8.6.1 Secret Yönetimi

```yaml
# GitHub Secrets yapısı
secrets:
  SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
  DOCKER_REGISTRY_TOKEN: ${{ secrets.DOCKER_TOKEN }}
  AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}
  PROD_DATABASE_STRING: ${{ secrets.PROD_DB_STRING }}
  OPENAI_API_KEY: ${{ secrets.OPENAI_KEY }}
  ANTHROPIC_API_KEY: ${{ secrets.ANTHROPIC_KEY }}
```

### 8.7 Code Quality Gates

| Gate | Koşul | Zorunlu | Araç |
|------|-------|---------|------|
| Build | 0 hata | ✅ | dotnet build |
| Test Coverage | ≥ %80 | ✅ | Coverlet |
| Code Style | 0 uyarı | ✅ | dotnet format |
| SonarQube Quality Gate | Pass | ✅ | SonarQube |
| Security Scan | 0 kritik | ✅ | Snyk |
| License Check | Approved | ✅ | license-checker |

### 8.8 Branching & Merge Stratejisi

```
main (production) ← Release only, 2 reviewer onayı
  ↑
develop (integration) ← Feature merge, CI pass
  ↑
feature/TASK-001-xxx ← Tek developer, PR → develop
hotfix/TASK-002-xxx ← Acil fix, PR → main + develop
```

#### 8.8.1 Branch Protection Kuralları

| Kural | Değer |
|-------|-------|
| Require pull request | ✅ |
| Required reviewers | ≥ 1 |
| Require status checks | ✅ (build + test) |
| Require branches up to date | ✅ |
| Require conversation resolution | ✅ |
| Require linear history | ✅ (squash merge) |
| Include administrators | ✅ |

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

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Mode:** Red Team · Human Mode · Truth Mode