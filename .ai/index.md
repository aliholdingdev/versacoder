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
├── spec/                        # ProjeSpec (Teknik Şartname)
│   ├── index.md                 # Spec indeksi
│   ├── versacoder-spec.md       # Ana teknik şartname (~2500+ satır)
│   └── versacoder-spec-summary.md # Şartname özeti
│
├── architecture/                # Katman Dokümanları (20+ dosya)
│   ├── 00-overview/
│   │   ├── architecture-master.md      # Genel mimari (~83 satır)
│   │   └── architecture-detailed.md    # Detaylı mimari (~888 satır)
│   ├── l0-domain/
│   │   └── domain-guide.md             # Domain katmanı (~508 satır)
│   ├── l1-abstractions/
│   │   └── abstractions-guide.md       # Soyutlama katmanı (~445 satır)
│   ├── l2-application/
│   │   └── application-guide.md        # Uygulama katmanı (~533 satır)
│   ├── l3-crosscutting/
│   │   └── crosscutting-guide.md       # Kesenekler (~446 satır)
│   ├── l4-infrastructure/
│   │   ├── infrastructure-guide.md     # Altyapı genel (~547 satır)
│   │   ├── ai/
│   │   │   ├── agent-runner.md         # Agent çalıştırıcı (~133 satır)
│   │   │   ├── provider-router.md      # Provider yönlendirici (~116 satır)
│   │   │   └── tool-system.md          # Tool sistemi (~123 satır)
│   │   └── data/
│   │       └── database-schema.md      # Veritabanı şeması (~125 satır)
│   ├── l5-protocol/
│   │   └── protocol-guide.md           # Protokol katmanı (~462 satır)
│   ├── l6-host/
│   │   └── host-guide.md               # Host katmanı (~574 satır)
│   └── l7-ui/
│       └── ui-guide.md                 # UI katmanı (~4761 satır, DevExpress/WPF/MAUI/Blazor)
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
├── rules/                       # Kurallar Motoru (6 dosya)
│   ├── coding-standards.md          # C# 12, SOLID, refactoring (~769 satır)
│   ├── security-architecture.md     # OWASP, JWT, API güvenliği (~700 satır)
│   ├── performance-guidelines.md    # Async, caching, DB optimizasyonu (~692 satır)
│   ├── plugin-development.md        # Plugin geliştirme (~502 satır)
│   ├── deployment-guide.md          # CI/CD, deployment (~3639 satır)
│   └── mcp-integration.md           # MCP entegrasyonu (~474 satır)
│
├── skills/                      # AI Yetenekleri (11 dosya)
│   ├── index.md
│   ├── architecture-skill.md        # Mimari planlama (~390 satır)
│   ├── cicd-skill.md                # CI/CD otomasyonu (~1557 satır)
│   ├── code-generation-skill.md     # Kod üretimi (~320 satır)
│   ├── debugging-skill.md           # Hata ayıklama (~234 satır)
│   ├── documentation-skill.md       # Dokümantasyon (~275 satır)
│   ├── monitoring-skill.md          # Prometheus, Serilog, OpenTelemetry (~495 satır)
│   ├── realtime-skill.md            # SignalR, real-time özellikler (~538 satır)
│   ├── refactoring-skill.md         # Yeniden yapılandırma (~267 satır)
│   ├── sectoral-agents-skill.md     # Sektörel agent yönetimi (~332 satır)
│   └── testing-skill.md             # Test yazma (~325 satır)
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
| Spec | 3 | ~3000 |
| **Toplam** | **77** | **~22,400** |

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

| Kategori | Dosya | İçerik | Satır |
|----------|-------|--------|-------|
| Coding | coding-standards.md | C# 12, SOLID, refactoring | ~769 |
| Security | security-architecture.md | OWASP, JWT, API güvenliği | ~700 |
| Performance | performance-guidelines.md | Async, caching, DB optimizasyonu | ~692 |
| Plugin | plugin-development.md | Plugin geliştirme | ~502 |
| Deployment | deployment-guide.md | CI/CD, GitHub Actions, Docker | ~3639 |
| MCP | mcp-integration.md | MCP client/server, tool register | ~474 |

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

| Skill | Amaç | Kullanım | Satır |
|-------|------|----------|-------|
| architecture-skill | Mimari planlama | Plan Agent | ~390 |
| cicd-skill | CI/CD otomasyonu, GitHub Actions | DevOps Agent | ~1557 |
| code-generation-skill | Kod üretimi | Build Agent | ~320 |
| debugging-skill | Hata ayıklama | Build Agent | ~234 |
| documentation-skill | Dokümantasyon | Summary Agent | ~275 |
| monitoring-skill | Prometheus, Serilog, OpenTelemetry | General Agent | ~495 |
| realtime-skill | SignalR hub'ları, reconnection | Build Agent | ~538 |
| refactoring-skill | Yeniden yapılandırma | Build Agent | ~267 |
| sectoral-agents-skill | Sektörel agent yönetimi | MO | ~332 |
| testing-skill | Test yazma | Build Agent | ~325 |

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
| Toplam Dosya | 90+ |
| Core Dosya | 10 |
| Agent Profili | 7 |
| Mimari Diyagram | 20+ |
| Şablon | 5 |
| ADR | 11 |
| Skill | 11 |
| Rules | 6 |
| Spec | 3 |
| Toplam Satır | ~45,000+ |
| Tahmini Token | ~55,000+ |

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

## 13. Sektörel Agent Kataloğu (60+ Agent)

### 13.1 Sektörel Agent Listesi

| # | Agent | Kod Adı | Sektör | Uzmanlık |
|---|-------|---------|--------|----------|
| 1 | **Otomotiv Agent** | `automotive` | Otomotiv | CAN Bus, OBD-II, AUTOSAR |
| 2 | **Sağlık Agent** | `healthcare` | Sağlık | HL7, FHIR, DICOM |
| 3 | **Finans Agent** | `finance` | Finans | Trading, Risk, Compliance |
| 4 | **Oyun Agent** | `gaming` | Oyun | Game Engine, Physics, Audio |
| 5 | **IoT Agent** | `iot` | IoT | MQTT, CoAP, Zigbee |
| 6 | **Siber Güvenlik Agent** | `cybersecurity` | Siber Güvenlik | Penetration, Forensics |
| 7 | **Yapay Zeka Agent** | `ai` | Yapay Zeka | ML, DL, NLP, CV |
| 8 | **Blockchain Agent** | `blockchain` | Blockchain | Smart Contract, DApp |
| 9 | **Eğitim Agent** | `education` | Eğitim | LMS, E-learning |
| 10 | **E-ticaret Agent** | `ecommerce` | E-ticaret | Payment, Inventory |
| 11 | **Mühendislik Agent** | `engineering` | Mühendislik | CAD, CAM, Simulation |
| 12 | **Hukuk Agent** | `legal` | Hukuk | Contract, Compliance |
| 13 | **Gayrimenkul Agent** | `realestate` | Gayrimenkul | Property, Valuation |
| 14 | **Lojistik Agent** | `logistics` | Lojistik | Supply Chain, Fleet |
| 15 | **Tarım Agent** | `agriculture` | Tarım | Precision, IoT |
| 16 | **Enerji Agent** | `energy` | Enerji | Smart Grid, SCADA |
| 17 | **Savunma Agent** | `defense` | Savunma | C4ISR, EW |
| 18 | **Uzay Agent** | `aerospace` | Uzay | GNC, Avionics |
| 19 | **Deniz Agent** | `marine` | Deniz | Navigation, Communication |
| 20 | **Madencilik Agent** | `mining` | Madencilik | Exploration, Processing |

### 13.2 Sektör Tespit Algoritması

```csharp
public Sector DetectSector(string userInput)
{
    var input = userInput.ToLowerInvariant();
    
    // Anahtar kelime eşleme
    foreach (var mapping in _sectorKeywords)
    {
        if (mapping.Keywords.Any(k => input.Contains(k)))
            return mapping.Sector;
    }
    
    // Varsayılan: Genel
    return Sector.General;
}
```

### 13.3 Sektör-Spesifik Kurallar

| Sektör | Standart | Kural |
|--------|----------|-------|
| Otomotiv | MISRA-C, AUTOSAR | Memory-safe coding |
| Sağlık | HIPAA, HL7 | Data encryption |
| Finans | PCI-DSS, SOX | Audit trail |
| Savunma | DO-178C, MISRA | Zero defect |
| Gıda | HACCP | Traceability |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode