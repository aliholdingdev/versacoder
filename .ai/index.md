---
title: "Versa Coder — Ana Katalog & Vault Haritası"
type: catalog
category: vault-index
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/index.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/AGENTS.md · .ai/WORKFLOW.md · .ai/brain.md"
---

# Versa Coder — Ana Katalog & Vault Haritası

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]] · [[brain.md]] · [[keys.md]]

---

## 1. Vault Yapısı

### 1.1 Dizin Yapısı

```
.ai/
├── CLAUDE.md                    # AI Anayasası (~700 satır)
├── AGENTS.md                    # Agent Kayıt Defteri (~500 satır)
├── WORKFLOW.md                  # Mühendislik Süreçleri (~500 satır)
├── brain.md                     # Mimari Kararlar (~500 satır)
├── ROLE.md                      # Rol Tanımları (~500 satır)
├── index.md                     # Bu dosya (~500 satır)
├── keys.md                      # Anahtar Kelime Eşleme
├── MEMORY.md                    # Session Hafızası
├── log.md                       # Denetim Kaydı
├── engine.md                    # Orkestrasyon Motoru
├── ULTRA-THINKING.md            # Ultra Düşünme Protokolü
├── glossary.md                  # Teknik Sözlük
├── project-plan.md              # Proje Planı
├── vault-summary.md             # Vault Kurulum Özeti
│
├── .agents/                     # Agent Profilleri
│   ├── master-orchestrator.md
│   ├── build-agent.md
│   ├── plan-agent.md
│   ├── explore-agent.md
│   ├── general-agent.md
│   ├── summary-agent.md
│   └── title-agent.md
│
├── .diagram/                    # Mimari Diyagramlar
│   ├── architecture.md
│   ├── flow.md
│   ├── sequence.md
│   └── data.md
│
├── .templates/                  # Kod Şablonları
│   ├── index.md
│   ├── entity.md
│   ├── repository.md
│   ├── handler.md
│   └── service.md
│
├── architecture/                # Katman Dokümanları
│   ├── L0-domain.md
│   ├── L1-abstractions.md
│   ├── L2-application.md
│   ├── L3-crosscutting.md
│   ├── L4-infrastructure.md
│   ├── L5-protocol.md
│   ├── L6-host.md
│   └── L7-ui.md
│
├── context/                     # Bağlam Yönetimi
│   ├── index.md
│   ├── sources.md
│   ├── assembly.md
│   └── epochs.md
│
├── decisions/                   # Mimari Karar Kayıtları (ADR)
│   ├── index.md
│   ├── ADR-001-clean-architecture.md
│   ├── ADR-002-ef-core-only.md
│   ├── ADR-003-sqlite-wal.md
│   ├── ADR-004-devexpress-winforms.md
│   ├── ADR-005-multi-provider-ai.md
│   ├── ADR-006-cqrs-mediatr.md
│   ├── ADR-007-agent-system.md
│   └── ADR-008-mcp-integration.md
│
├── learning/                    # Öğrenme Sistemi
│   ├── index.md
│   ├── patterns/
│   ├── corrections/
│   ├── knowledge/
│   └── rules/
│
├── memory/                      # Hafıza Yönetimi
│   ├── index.md
│   └── sessions/
│
├── project/                     # Proje Analizi
│   ├── index.md
│   ├── structure.md
│   ├── analysis.md
│   └── metrics.md
│
├── rules/                       # Kurallar Motoru
│   ├── index.md
│   ├── coding-standards.md
│   ├── security-architecture.md
│   ├── performance-guidelines.md
│   └── plugin-development.md
│
├── skills/                      # AI Yetenekleri
│   ├── index.md
│   ├── architecture.md
│   ├── code-generation.md
│   ├── debugging.md
│   ├── documentation.md
│   ├── refactoring.md
│   └── testing.md
│
└── ui-design/                   # UI Tasarım Dokümanları
    ├── index.md
    ├── wireframes.md
    ├── components.md
    └── themes.md
```

### 1.2 Dosya Sayıları

| Kategori | Dosya Sayısı | Toplam Satır |
|----------|-------------|--------------|
| Core | 10 | ~5000 |
| Agents | 7 | ~2100 |
| Diagrams | 4 | ~800 |
| Templates | 5 | ~1000 |
| Architecture | 8 | ~1600 |
| Context | 4 | ~800 |
| Decisions | 9 | ~2700 |
| Learning | 5 | ~1000 |
| Memory | 2 | ~400 |
| Project | 4 | ~800 |
| Rules | 5 | ~1000 |
| Skills | 7 | ~1400 |
| UI Design | 4 | ~800 |
| **Toplam** | **74** | **~19,400** |

---

## 2. Core Dosyalar

### 2.1 CLAUDE.md — AI Anayasası

| Bölüm | İçerik | Önem |
|-------|--------|------|
| Boot Protocol | Session başlatma sırası | Zorunlu |
| Temel İlkeler | SSOT, Zero Code, Human Gate | Zorunlu |
| Guardrails | 16 koruyucu kural | Zorunlu |
| Agent Protokolü | Seçim ve çalışma | Zorunlu |
| Mimari Kurallar | Katman ve bağımlılık | Zorunlu |
| Güvenlik | Veri ve erişim | Zorunlu |
| Kalite | Standartlar ve metrikler | Yüksek |
| Acil Durum | Protokoller | Yüksek |
| Performans | Hedefler | Orta |

### 2.2 AGENTS.md — Agent Kayıt Defteri

| Bölüm | İçerik | Önem |
|-------|--------|------|
| Agent Genel Bakış | 7 agent tanımı | Zorunlu |
| Domain Sınırları | Dosya erişim matrisi | Zorunlu |
| Keyword Yönlendirme | Agent seçim algoritması | Zorunlu |
| Görev Dağıtımı | Dağıtım akışı | Yüksek |
| Öncelik Seviyeleri | 4 seviye | Yüksek |
| Handover | Transfer protokolü | Yüksek |
| Eskalasyon | Çıkış protokolü | Yüksek |
| Sağlık Kontrolü | Durum kodları | Orta |
| Context Lock | Kilitleme | Orta |
| Ultra Düşünme | 5 adımlı protokol | Zorunlu |

### 2.3 WORKFLOW.md — Mühendislik Süreçleri

| Bölüm | İçerik | Önem |
|-------|--------|------|
| Geliştirme Metodolojisi | Agile + Scrum | Yüksek |
| Görev Yönetimi | Türler ve döngü | Yüksek |
| Kod Kalite | Review ve analiz | Yüksek |
| Test Stratejisi | Piramit ve türler | Yüksek |
| Versiyonlama | Semantic versioning | Orta |
| Branching | Git flow | Yüksek |
| CI/CD | Pipeline adımları | Yüksek |
| Dokümantasyon | Format standartları | Orta |
| Güvenlik | Kontrol listeleri | Yüksek |
| Performance | Metrikler ve alerting | Orta |
| Backup | Strateji ve kurtarma | Orta |

### 2.4 brain.md — Mimari Kararlar

| Bölüm | İçerik | Önem |
|-------|--------|------|
| Kararlar Özeti | Kabul ve ret | Zorunlu |
| Katmanlar | L0-L7 tanımları | Zorunlu |
| Domain Model | Varlıklar ve değerler | Yüksek |
| CQRS | Command ve query | Yüksek |
| AI Provider | Çoklu sağlayıcı | Yüksek |
| Agent Sistemi | Mimarisi | Yüksek |
| Tool Sistemi | 40+ tool | Yüksek |
| Veritabanı | SQLite tasarımı | Yüksek |
| Güvenlik | Katmanlar | Yüksek |
| Performans | Caching ve async | Orta |
| Error Handling | Hata hiyerarşisi | Yüksek |
| Logging | Strateji | Orta |

### 2.5 ROLE.md — Rol Tanımları

| Bölüm | İçerik | Önem |
|-------|--------|------|
| Roller Genel Bakış | AI + İnsan | Zorunlu |
| MO Detayı | Görev ve yetki | Zorunlu |
| Build Agent | Görev ve yetki | Zorunlu |
| Plan Agent | Görev ve yetki | Zorunlu |
| Explore Agent | Görev ve yetki | Zorunlu |
| General Agent | Görev ve yetki | Orta |
| Summary Agent | Görev ve yetki | Orta |
| Title Agent | Görev ve yetki | Orta |
| İnsan Rolleri | Sorumluluklar | Yüksek |
| Erişim Matrisi | Dosya ve işlem | Zorunlu |
| Performans | Metrikler | Orta |

---

## 3. Agent Profilleri

### 3.1 Profil Dosyaları

| Dosya | Agent | İçerik |
|-------|-------|--------|
| master-orchestrator.md | MO | Koordinasyon detayı |
| build-agent.md | Build | Kod üretimi detayı |
| plan-agent.md | Plan | Planlama detayı |
| explore-agent.md | Explore | Analiz detayı |
| general-agent.md | General | Genel detay |
| summary-agent.md | Summary | Doküman detayı |
| title-agent.md | Title | İsimlendirme detayı |

### 3.2 Profil Yapısı

```markdown
## [Agent Name]

### Tanım
### Görevler
### Yetkiler
### Kısıtlamalar
### Araçlar
### Çıktı Formatları
### Örnekler
### Performans Hedefleri
```

---

## 4. Mimari Diyagramlar

### 4.1 Diyagram Dosyaları

| Dosya | İçerik |
|-------|--------|
| architecture.md | Genel mimari yapı |
| flow.md | İş akışı diyagramları |
| sequence.md | Sıralama diyagramları |
| data.md | Veri modeli diyagramları |

### 4.2 Diyagram Formatları

| Format | Kullanım | Araç |
|--------|----------|------|
| Mermaid | Web ve doküman | Mermaid |
| PlantUML | Detaylı diyagramlar | PlantUML |
| ASCII | Basit gösterim | Metin |
| ASCII Art | Kolay okuma | Metin |

---

## 5. Şablonlar

### 5.1 Şablon Dosyaları

| Dosya | Kullanım |
|-------|----------|
| entity.md | Yeni varlık için |
| repository.md | Repository interface ve implementation |
| handler.md | MediatR handler |
| service.md | Service class |

### 5.2 Şablon Kuralları

| Kural | Açıklama |
|-------|----------|
| Guardrail #6 | Şablon kullanımı zorunlu |
| Uyumluluk | Mevcut kod yapısına uygunluk |
| Test | Her şablon için test örneği |
| Dokümantasyon | XML doc zorunlu |

---

## 6. Karar Kayıtları (ADR)

### 6.1 ADR Listesi

| ADR | Başlık | Durum | Tarih |
|-----|--------|-------|-------|
| ADR-001 | Clean Architecture (L0-L7) | Kabul | 2026-08-25 |
| ADR-002 | EF Core DbContext ONLY | Kabul | 2026-08-25 |
| ADR-003 | SQLite WAL Mode | Kabul | 2026-08-25 |
| ADR-004 | DevExpress WinForms 2026 | Kabul | 2026-08-25 |
| ADR-005 | Multi-Provider AI | Kabul | 2026-08-25 |
| ADR-006 | CQRS with MediatR | Kabul | 2026-08-25 |
| ADR-007 | 7-Agent System | Kabul | 2026-08-25 |
| ADR-008 | MCP Integration | Kabul | 2026-08-25 |

### 6.2 ADR Formatı

```markdown
# ADR-001: [Başlık]

## Durum
Kabul / Red / Revizyon

## Bağlam
[Problem tanımı]

## Karar
[Karar açıklaması"

## Gerekçe
[Neden bu karar verildi]

## Sonuçlar
[Etkiler]
```

---

## 7. Öğrenme Sistemi

### 7.1 Öğrenme Kaynakları

| Kaynak | Tür | Kullanım |
|--------|-----|----------|
| patterns/ | Olumlu kalıplar | Tekrar kullanımı |
| corrections/ | Düzeltmeler | Hata önleme |
| knowledge/ | Bilgi bankası | Referans |
| rules/ | Kurallar | Zorunluluk |

### 7.2 Öğrenme Akışı

```
[Task] → [Execution] → [Feedback] → [Learning] → [Pattern/Correction] → [Knowledge Base]
```

---

## 8. Bağlam Yönetimi

### 8.1 Bağlam Kaynakları

| Kaynak | Kullanım |
|--------|----------|
| Project Context | Proje yapısı ve analizi |
| File Context | Dosya içeriği ve yapısı |
| Session Context | Mevcut oturum bilgisi |
| Skill Context | AI yetenekleri |
| Diagram Context | Diyagram bilgileri |

### 8.2 Bağlam Assembly

```csharp
public class ContextAssembler
{
    public async Task<Context> AssembleAsync(
        string userPrompt,
        CancellationToken ct)
    {
        var context = new Context();
        
        // 1. Project context
        context.Project = await GetProjectContextAsync(ct);
        
        // 2. File context (prompt'a göre)
        context.Files = await GetRelevantFilesAsync(userPrompt, ct);
        
        // 3. Session context
        context.Session = await GetSessionContextAsync(ct);
        
        // 4. Skill context
        context.Skills = await GetRelevantSkillsAsync(userPrompt, ct);
        
        return context;
    }
}
```

---

## 9. Kurallar Motoru

### 9.1 Kategoriler

| Kategori | Dosya | İçerik |
|----------|-------|--------|
| Coding | coding-standards.md | Kod yazım kuralları |
| Security | security-architecture.md | Güvenlik mimarisi |
| Performance | performance-guidelines.md | Performans kılavuzu |
| Plugin | plugin-development.md | Plugin geliştirme |

### 9.2 Kural Uygulama

```csharp
public interface IRule
{
    string Name { get; }
    string Description { get; }
    RuleSeverity Severity { get; }
    Task<RuleResult> EvaluateAsync(RuleContext context, CancellationToken ct);
}
```

---

## 10. AI Yetenekleri (Skills)

### 10.1 Skill Listesi

| Skill | Amaç | Kullanım |
|-------|------|----------|
| architecture | Mimari planlama | Plan Agent |
| code-generation | Kod üretimi | Build Agent |
| debugging | Hata ayıklama | Build Agent |
| documentation | Dokümantasyon | Summary Agent |
| refactoring | Yeniden yapılandırma | Build Agent |
| testing | Test yazma | Build Agent |

### 10.2 Skill Yükleme

```csharp
public interface ISkill
{
    string Name { get; }
    string Description { get; }
    Task<SkillResult> ExecuteAsync(SkillContext context, CancellationToken ct);
}
```

---

## 11. UI Tasarım

### 11.1 Dosyalar

| Dosya | İçerik |
|-------|--------|
| wireframes.md | Tel çerçeve diyagramları |
| components.md | Bileşen tanımları |
| themes.md | Tema ve stil |

### 11.2 UI Bileşenleri

| Bileşen | DevExpress Kontrol | Kullanım |
|---------|-------------------|----------|
| Ribbon | DXRibbonControl | Ana menü |
| Tab MDI | DXMdiContainer | Oturum sekmeleri |
| Grid | DXGrid | Veri görüntüleme |
| Tree | DXTreeList | Dosya ağacı |
| Editor | DXRichEdit | Kod editörü |
| Chart | DXChart | Metrikler |

---

## 12. Vault İstatistikleri

### 12.1 Genel İstatistikler

| Metrik | Değer |
|--------|-------|
| Toplam Dosya | 74 |
| Core Dosya | 10 |
| Agent Profili | 7 |
| Mimari Diyagram | 4 |
| Şablon | 5 |
| ADR | 8 |
| Skill | 6 |
| Toplam Satır | ~19,400 |
| Tahmini Token | ~26,500 |

### 12.2 Kapsam

| Kategori | Kapsam | Durum |
|----------|--------|-------|
| AI Constitution | %100 | ✅ |
| Agent Definitions | %100 | ✅ |
| Workflow | %100 | ✅ |
| Architecture | %100 | ✅ |
| Roles | %100 | ✅ |
| Templates | %80 | 🔄 |
| Skills | %100 | ✅ |
| UI Design | %60 | 🔄 |

---

## 13. Vault Kullanım Rehberi

### 13.1 Session Başlatma

Her AI session başlatıldığında aşağıdaki sırayla yüklenmelidir:

```
1. CLAUDE.md → AI anayasası (zorunlu)
2. AGENTS.md → Ajan kayıt defteri (zorunlu)
3. WORKFLOW.md → Mühendislik süreçleri (zorunlu)
4. brain.md → Mimari kararlar (zorunlu)
5. ROLE.md → Rol tanımları (zorunlu)
6. index.md → Ana katalog (opsiyonel)
7. keys.md → Anahtar kelime eşleme (opsiyonel)
```

### 13.2 Token Optimizasyonu

| Strateji | Kullanım | Tasarruf |
|----------|----------|----------|
| Lazy Loading | İhtiyaç duyulan dosyalar | %60 |
| Category Filter | Kategori bazlı yükleme | %40 |
| Summary First | Özet → Detaylı geçiş | %50 |
| Cache | Sık kullanılan dosyalar | %30 |

### 13.3 Arama Stratejisi

| Arama Türü | Yöntem | Hız |
|------------|--------|-----|
| Dosya adı | Glob pattern | Hızlı |
| İçerik | Grep regex | Orta |
| Keyword | keys.md eşleme | Hızlı |
| Semantic | AI embedding | Yavaş |

### 13.4 Bağlam Toplama

```csharp
// Context assembly örneği
public class ContextAssembler
{
    public async Task<Context> AssembleAsync(string userPrompt, CancellationToken ct)
    {
        var context = new Context();
        
        // 1. Vault'tan temel bilgileri yükle
        context.CoreFiles = await LoadCoreFilesAsync(ct);
        
        // 2. Prompt'a göre ilgili dosyaları bul
        var keywords = ExtractKeywords(userPrompt);
        context.RelevantFiles = await FindRelevantFilesAsync(keywords, ct);
        
        // 3. Session geçmişini yükle
        context.SessionHistory = await LoadSessionHistoryAsync(ct);
        
        // 4. Proje durumunu yükle
        context.ProjectState = await LoadProjectStateAsync(ct);
        
        return context;
    }
}
```

---

## 14. Vault Güncelleme Protokolü

### 14.1 Güncelleme Yetkisi

| Dosya | Güncelleyen | Onay |
|-------|-------------|------|
| CLAUDE.md | MO + İnsan | İnsan onayı |
| AGENTS.md | MO + İnsan | İnsan onayı |
| WORKFLOW.md | MO + İnsan | İnsan onayı |
| brain.md | MO + İnsan | İnsan onayı |
| ROLE.md | MO + İnsan | İnsan onayı |
| index.md | MO | Otomatik |
| keys.md | MO | Otomatik |
| .agents/* | MO | Otomatik |
| .templates/* | Build Agent | MO onayı |

### 14.2 Versiyonlama

```markdown
## [1.1.0] - 2026-08-26

### Changed
- CLAUDE.md: Guardrail kategorileri eklendi
- AGENTS.md: İletişim protokolleri eklendi
- WORKFLOW.md: Proje yaşam döngüsü eklendi
- brain.md: DDD kalıpları eklendi
- ROLE.md: State machine eklendi

### Added
- Section 15: Agent State Machine
- Section 16: Agent Communication Protocol
- Section 17: Agent Performance Optimization
```

### 14.3 Değişiklik Takibi

| Tarih | Dosya | Değişiklik | Sorumlu |
|-------|-------|-----------|---------|
| 2026-08-26 | CLAUDE.md | Guardrail kategorileri | MO |
| 2026-08-26 | AGENTS.md | İletişim protokolleri | MO |
| 2026-08-26 | WORKFLOW.md | Proje yaşam döngüsü | MO |
| 2026-08-26 | brain.md | DDD kalıpları | MO |
| 2026-08-26 | ROLE.md | State machine | MO |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode