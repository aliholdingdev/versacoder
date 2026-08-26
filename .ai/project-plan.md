---
title: "Versa Coder — Kapsamlı Proje Planı"
type: plan
category: project-plan
date: 2026-08-25
updated: 2026-08-25
status: draft
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
---

# Versa Coder — Kapsamlı Proje Planı

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]] · [[index.md]]

---

## 1. Proje Özeti

Versa Coder, çok kapsamlı bir AI destekli kod geliştirme platformudur. Bu plan, tüm sistem özelliklerini, mimarisini ve geliştirme yol haritasını tanımlar.

### 1.1 Proje Hedefleri

| Hedef | Açıklama |
|-------|----------|
| Çoklu Platform | DevExpress WinForms + WPF + MAUI + Web |
| Çoklu Dil | C#, C++, C, Python, JavaScript, TypeScript, Java, Go, Rust, vb. |
| Embedded Sistemler | PIC, AVR, Atmel, Arduino, ESP32, STM32, ARM, RISC-V |
| Driver Geliştirme | Windows, Linux, macOS, Embedded, ASIO, WebRTC |
| Sektörel Agent'lar | Otomotiv, Sağlık, Finans, Oyun, IoT, Siber Güvenlik, Yapay Zeka, Blockchain |
| Kurumsal Güvenlik | OWASP, encryption, token management, audit trail |
| Test Coverage | %90+ |
| Deployment | Hybrid (otomatik test, manuel onay) |

---

## 2. Mimari Katman Yapısı (50+ Katman)

### 2.1 Temel Katmanlar (L0-L7)

| Katman | Modül | Sorumluluk |
|--------|-------|------------|
| L0 | Domain | Varlıklar, değer objeleri, domain event'leri |
| L1 | Abstractions | Arayüzler, kontratlar |
| L2 | Application | Use case'ler, DTO'lar, servisler |
| L3 | CrossCutting | Logging, exception, validation |
| L4 | Infrastructure | 28+ altyapı modülü |
| L5 | Protocol | AI protokolü, MCP, Provider iletişimi |
| L6 | Host | Uygulama başlangıcı, DI, konfigürasyon |
| L7 | UI | DevExpress WinForms, WPF, MAUI, Web |

### 2.2 Infrastructure Katmanları (L8-L30)

| Katman | Modül | Sorumluluk |
|--------|-------|------------|
| L8 | Infrastructure.Data | SQLite, EF Core, repository |
| L9 | Infrastructure.AI | LLM provider, agent runner |
| L10 | Infrastructure.MCP | MCP client/server |
| L11 | Infrastructure.Auth | API key, credential yönetimi |
| L12 | Infrastructure.Config | uygulama ayarları |
| L13 | Infrastructure.Plugins | Plugin sistemi |
| L14 | Infrastructure.Services | Yardımcı servisler |
| L15 | Infrastructure.Caching | Önbellek yönetimi |
| L16 | Infrastructure.Messaging | Event bus, messaging |
| L17 | Infrastructure.FileSystem | Dosya sistemi erişimi |
| L18 | Infrastructure.Network | HTTP client, WebSocket |
| L19 | Infrastructure.Security | Şifreleme, token |
| L20 | Infrastructure.Observability | Monitoring, metrics |
| L21 | Infrastructure.Context | Context assembly, epoch |
| L22 | Infrastructure.Learning | Pattern, düzeltme, bilgi |
| L23 | Infrastructure.Diagram | Diyagram okuma, AI'a öğretme |
| L24 | Infrastructure.ProjectAnalysis | Proje indeksleme, analiz |
| L25 | Infrastructure.Testing | Test altyapısı |
| L26 | Infrastructure.Documentation | Otomatik dokümantasyon |
| L27 | Infrastructure.Refactoring | Refactoring araçları |
| L28 | Infrastructure.CodeAnalysis | Kod analizi, metrikler |
| L29 | Infrastructure.Git | Git entegrasyonu |
| L30 | Infrastructure.Integration | Üçüncü parti entegrasyon |

### 2.3 Ek Katmanlar (L31-L50+)

| Katman | Modül | Sorumluluk |
|--------|-------|------------|
| L31 | Infrastructure.Embedded | Embedded sistem desteği |
| L32 | Infrastructure.Driver | Driver geliştirme |
| L33 | Infrastructure.Web | Web geliştirme |
| L34 | Infrastructure.Mobile | Mobil geliştirme |
| L35 | Infrastructure.Desktop | Masaüstü geliştirme |
| L36 | Infrastructure.Cloud | Bulut entegrasyonu |
| L37 | Infrastructure.DevOps | CI/CD, deployment |
| L38 | Infrastructure.Analytics | Kullanım analitiği |
| L39 | Infrastructure.Monitoring | Performans izleme |
| L40 | Infrastructure.Logging | Gelişmiş loglama |
| L41 | Infrastructure.Caching | Gelişmiş önbellek |
| L42 | Infrastructure.Queue | Kuyruk yönetimi |
| L43 | Infrastructure.Scheduler | Zamanlama |
| L44 | Infrastructure.Workflow | İş akışı motoru |
| L45 | Infrastructure.Rule | Kural motoru |
| L46 | Infrastructure.Decision | Karar destek |
| L47 | Infrastructure.Recommendation | Öneri sistemi |
| L48 | Infrastructure.Search | Arama motoru |
| L49 | Infrastructure.Index | İndeksleme |
| L50 | Infrastructure.Cache | Gelişmiş cache |

---

## 3. Agent Sistemi (50+ Agent)

### 3.1 Temel Agent'lar

| # | Agent | Kod Adı | Uzmanlık | Deneyim |
|---|-------|---------|----------|---------|
| 1 | **Master Orchestrator** | `mo` | Görev dağıtımı, koordinasyon | Expert |
| 2 | **Build Agent** | `build` | Kod yazma, dosya oluşturma | Expert |
| 3 | **Plan Agent** | `plan` | Mimari planlama, task dağıtımı | Expert |
| 4 | **Explore Agent** | `explore` | Kod analizi, dosya tarama | Expert |
| 5 | **General Agent** | `general` | Genel amaçlı görevler | Expert |
| 6 | **Summary Agent** | `summary` | Özetleme, dokümantasyon | Expert |
| 7 | **Title Agent** | `title` | Başlık oluşturma, isimlendirme | Expert |

### 3.2 Dil Uzmanı Agent'lar

| # | Agent | Kod Adı | Dil | Deneyim |
|---|-------|---------|-----|---------|
| 8 | **C# Agent** | `csharp` | C# .NET 8 | Senior |
| 9 | **C++ Agent** | `cpp` | C++ | Senior |
| 10 | **C Agent** | `c` | C | Senior |
| 11 | **Python Agent** | `python` | Python | Senior |
| 12 | **JavaScript Agent** | `javascript` | JavaScript | Senior |
| 13 | **TypeScript Agent** | `typescript` | TypeScript | Senior |
| 14 | **Java Agent** | `java` | Java | Senior |
| 15 | **Go Agent** | `go` | Go | Senior |
| 16 | **Rust Agent** | `rust` | Rust | Senior |
| 17 | **Swift Agent** | `swift` | Swift | Senior |
| 18 | **Kotlin Agent** | `kotlin` | Kotlin | Senior |
| 19 | **PHP Agent** | `php` | PHP | Senior |
| 20 | **Ruby Agent** | `ruby` | Ruby | Senior |

### 3.3 Platform Uzmanı Agent'lar

| # | Agent | Kod Adı | Platform | Deneyim |
|---|-------|---------|----------|---------|
| 21 | **Web Agent** | `web` | Web geliştirme | Senior |
| 22 | **Mobile Agent** | `mobile` | Mobil geliştirme | Senior |
| 23 | **Desktop Agent** | `desktop` | Masaüstü geliştirme | Senior |
| 24 | **Cloud Agent** | `cloud` | Bulut entegrasyonu | Senior |
| 25 | **DevOps Agent** | `devops` | CI/CD, deployment | Senior |

### 3.4 Embedded Sistem Agent'ları

| # | Agent | Kod Adı | Platform | Deneyim |
|---|-------|---------|----------|---------|
| 26 | **PIC Agent** | `pic` | PIC microcontroller | Senior |
| 27 | **AVR Agent** | `avr` | AVR (Atmel) | Senior |
| 28 | **Arduino Agent** | `arduino` | Arduino | Senior |
| 29 | **ESP32 Agent** | `esp32` | ESP32 | Senior |
| 30 | **STM32 Agent** | `stm32` | STM32 | Senior |
| 31 | **ARM Agent** | `arm` | ARM | Senior |
| 32 | **RISC-V Agent** | `riscv` | RISC-V | Senior |
| 33 | **FPGA Agent** | `fpga` | FPGA | Senior |

### 3.5 Driver Geliştirme Agent'ları

| # | Agent | Kod Adı | Platform | Deneyim |
|---|-------|---------|----------|---------|
| 34 | **Windows Driver Agent** | `windriver` | Windows driver | Senior |
| 35 | **Linux Driver Agent** | `linuxdriver` | Linux driver | Senior |
| 36 | **macOS Driver Agent** | `macosdriver` | macOS driver | Senior |
| 37 | **ASIO Agent** | `asio` | ASIO driver | Senior |
| 38 | **WebRTC Agent** | `webrtc` | WebRTC | Senior |
| 39 | **USB Agent** | `usb` | USB driver | Senior |
| 40 | **Bluetooth Agent** | `bluetooth` | Bluetooth driver | Senior |

### 3.6 Sektörel Agent'lar

| # | Agent | Kod Adı | Sektör | Deneyim |
|---|-------|---------|--------|---------|
| 41 | **Otomotiv Agent** | `automotive` | Otomotiv | Senior |
| 42 | **Sağlık Agent** | `healthcare` | Sağlık | Senior |
| 43 | **Finans Agent** | `finance` | Finans | Senior |
| 44 | **Oyun Agent** | `gaming` | Oyun | Senior |
| 45 | **IoT Agent** | `iot` | IoT | Senior |
| 46 | **Siber Güvenlik Agent** | `cybersecurity` | Siber Güvenlik | Senior |
| 47 | **Yapay Zeka Agent** | `ai` | Yapay Zeka | Senior |
| 48 | **Blockchain Agent** | `blockchain` | Blockchain | Senior |
| 49 | **Eğitim Agent** | `education` | Eğitim | Senior |
| 50 | **E-ticaret Agent** | `ecommerce` | E-ticaret | Senior |

### 3.7 Deneyim Seviyeleri

| Seviye | Tanım | Varsayılan mı? |
|--------|-------|----------------|
| Junior | 0-2 yıl deneyim | Hayır |
| Mid | 2-5 yıl deneyim | Hayır |
| Senior | 5-10 yıl deneyim | Evet (varsayılan) |
| Lead | 10+ yıl deneyim | Hayır |
| Principal | 15+ yıl deneyim | Hayır |

**Not:** Kullanıcılar chat ekranından seviye seçebilir. Varsayılan seviye Senior'dur.

---

## 4. Tool Sistemi (100+ Araç)

### 4.1 Temel Araçlar

| Kategori | Araçlar | Sayı |
|----------|---------|------|
| Dosya | Read, Write, Edit, Glob, Grep, Copy, Move, Delete, Rename | 9 |
| Terminal | Bash, PowerShell, CMD, SSH | 4 |
| Git | Status, Diff, Commit, Push, Pull, Branch, Merge, Revert | 8 |
| Test | Run Tests, Coverage, Mock, Assert | 4 |
| AI | LLM Query, Embedding, Completion, Chat | 4 |
| MCP | Resource Read, Resource Write, Tool Call, Tool Register | 4 |
| Proje | Index, Analyze, Diagram, Structure | 4 |
| Session | Save, Load, Branch, Fork, Merge, Revert | 6 |
| Context | Assemble, Update, Validate, Optimize | 4 |

### 4.2 Gelişmiş Araçlar

| Kategori | Araçlar | Sayı |
|----------|---------|------|
| Code Analysis | Syntax Check, Type Check, Security Scan, Performance Analysis | 4 |
| Refactoring | Rename, Extract, Inline, Move, Copy | 5 |
| Testing | Unit Test, Integration Test, E2E Test, Performance Test | 4 |
| Documentation | Generate Docs, Update Docs, Validate Docs | 3 |
| Deployment | Build, Package, Deploy, Rollback | 4 |
| Monitoring | Log Analysis, Metrics Collection, Alert Management | 3 |

### 4.3 Embedded Araçlar

| Kategori | Araçlar | Sayı |
|----------|---------|------|
| PIC | PIC Compiler, PIC Debugger, PIC Programmer | 3 |
| AVR | AVR Compiler, AVR Debugger, AVR Programmer | 3 |
| Arduino | Arduino CLI, Arduino IDE, Arduino Library Manager | 3 |
| ESP32 | ESP-IDF, ESP32 Compiler, ESP32 Flasher | 3 |
| STM32 | STM32CubeIDE, STM32 Compiler, STM32 Debugger | 3 |
| ARM | ARM Compiler, ARM Debugger, ARM Programmer | 3 |
| RISC-V | RISC-V Compiler, RISC-V Debugger, RISC-V Programmer | 3 |
| FPGA | FPGA Synthesis, FPGA Place&Route, FPGA Bitstream | 3 |

### 4.4 Driver Araçları

| Kategori | Araçlar | Sayı |
|----------|---------|------|
| Windows Driver | WDK, WDF, KMDF, UMDF | 4 |
| Linux Driver | Kernel Module, Device Tree, Sysfs | 3 |
| macOS Driver | IOKit, Kernel Extension | 2 |
| ASIO | ASIO SDK, ASIO Driver Template | 2 |
| WebRTC | WebRTC Library, ICE/STUN/TURN | 2 |
| USB | USB Driver, USB Library | 2 |
| Bluetooth | Bluetooth Driver, Bluetooth Library | 2 |

### 4.5 Sektörel Araçlar

| Kategori | Araçlar | Sayı |
|----------|---------|------|
| Otomotiv | CAN Bus, OBD-II, AUTOSAR | 3 |
| Sağlık | HL7, FHIR, DICOM | 3 |
| Finans | Trading API, Risk Analysis, Compliance | 3 |
| Oyun | Game Engine, Physics Engine, Audio Engine | 3 |
| IoT | MQTT, CoAP, Zigbee | 3 |
| Siber Güvenlik | Vulnerability Scanner, Penetration Testing | 2 |
| Yapay Zeka | ML Framework, Model Training, Inference | 3 |
| Blockchain | Smart Contract, DApp, Web3 | 3 |

**Toplam Araç:** 100+

---

## 5. UI Framework

### 5.1 Mevcut Platform

| Platform | Teknoloji | Durum |
|----------|-----------|-------|
| WinForms | DevExpress 2026 Universal | Mevcut |

### 5.2 Ek Platformlar

| Platform | Teknoloji | Öncelik |
|----------|-----------|---------|
| WPF | DevExpress WPF | Yüksek |
| MAUI | .NET MAUI | Orta |
| Web | Blazor + React | Orta |
| Mobile | Xamarin / .NET MAUI | Düşük |

---

## 6. Veritabanı Mimarisi

### 6.1 Mevcut Veritabanı

| Özellik | Değer |
|---------|-------|
| Motor | SQLite |
| Mod | WAL (Write-Ahead Logging) |
| ORM | EF Core (DbContext ONLY) |
| Naming | PascalCase (C# convention) |
| Migration | EF Core Migrations |
| Connection | IConfiguration + .env |

---

## 7. MCP Entegrasyonu

### 7.1 MCP Destek Seviyesi

| Özellik | Durum |
|---------|-------|
| MCP Client | Destekleniyor |
| MCP Server | Destekleniyor |
| Custom MCP Tools | Destekleniyor |
| MCP Resources | Destekleniyor |

---

## 8. Plugin Sistemi

### 8.1 Plugin Özellikleri

| Özellik | Durum |
|---------|-------|
| Plugin Geliştirme | Destekleniyor |
| Plugin Market | Destekleniyor |
| Plugin Güvenliği | Destekleniyor |
| Plugin Sandbox | Destekleniyor |

---

## 9. Real-time Özellikler

### 9.1 Real-time Özellik Listesi

| Özellik | Durum |
|---------|-------|
| Real-time Code Completion | Destekleniyor |
| Real-time Collaboration | Destekleniyor |
| Real-time Notifications | Destekleniyor |
| Real-time Sync | Destekleniyor |

---

## 10. Analytics & Monitoring

### 10.1 Analytics Özellikleri

| Özellik | Durum |
|---------|-------|
| Usage Analytics | Destekleniyor |
| Performance Monitoring | Destekleniyor |
| Error Tracking | Destekleniyor |
| User Behavior Analysis | Destekleniyor |

---

## 11. CI/CD Entegrasyonu

### 11.1 CI/CD Platformları

| Platform | Durum |
|----------|-------|
| GitHub Actions | Destekleniyor |
| GitLab CI | Destekleniyor |
| Jenkins | Destekleniyor |
| Azure DevOps | Destekleniyor |

---

## 12. Test Coverage

### 12.1 Test Hedefi

| Metrik | Hedef |
|--------|-------|
| Code Coverage | %90+ |
| Unit Test | %95+ |
| Integration Test | %85+ |
| E2E Test | %80+ |

---

## 13. Deployment Stratejisi

### 13.1 Deployment Modeli

| Model | Açıklama |
|-------|----------|
| Hybrid | Otomatik test + Manuel onay |
| CI/CD Pipeline | Otomatik build ve test |
| Manual Approval | Manuel onay sonrası deployment |
| Rollback | Otomatik rollback desteği |

---

## 14. Güvenlik

### 14.1 Güvenlik Özellikleri

| Özellik | Durum |
|---------|-------|
| OWASP Compliance | Destekleniyor |
| Encryption | AES-256 |
| Token Management | JWT + Refresh Token |
| Audit Trail | append-only log |
| Zero Trust | Destekleniyor |

---

## 15. Token Bütçesi

### 15.1 Token Ayarları

| Ayar | Varsayılan | Açıklama |
|------|------------|----------|
| Max Token/Session | 100K | Kullanıcı tarafından ayarlanabilir |
| Context Window | Dual context | Agent paneli açılıp kapatılabilir |
| Token Optimization | Otomatik | Compaction agent ile sıkıştırma |

---

## 16. Geliştirme Yol Haritası

### 16.1 Faz 1 — MVP (3-6 ay)

| Görev | Süre | Çıktı |
|-------|------|-------|
| Temel IDE + AI entegrasyonu | 3 ay | Çalışan uygulama |
| Tek provider (OpenAI) | 1 ay | AI entegrasyonu |
| Temel agent sistemi | 2 ay | 7 agent |
| SQLite veritabanı | 1 ay | Veritabanı entegrasyonu |
| DevExpress WinForms UI | 3 ay | Arayüz |

### 16.2 Faz 2 — Pro (6-12 ay)

| Görev | Süre | Çıktı |
|-------|------|-------|
| Çoklu provider desteği | 3 ay | 5 provider |
| Agent sistemi genişletme | 6 ay | 20+ agent |
| Context yönetimi | 3 ay | Context assembly |
| Öğrenme sistemi | 3 ay | Learning system |
| MCP entegrasyonu | 3 ay | MCP client/server |
| Plugin sistemi | 3 ay | Plugin market |

### 16.3 Faz 3 — Enterprise (12-18 ay)

| Görev | Süre | Çıktı |
|-------|------|-------|
| Embedded sistem desteği | 6 ay | 8 platform |
| Driver geliştirme | 6 ay | 7 platform |
| Sektörel agent'lar | 6 ay | 10 sektör |
| Çoklu platform UI | 6 ay | 4 platform |
| CI/CD entegrasyonu | 3 ay | 4 platform |
| Güvenlik hardening | 3 ay | Enterprise security |

### 16.4 Faz 4 — Ultimate (18-24 ay)

| Görev | Süre | Çıktı |
|-------|------|-------|
| 50+ agent | 6 ay | Tam agent sistemi |
| 100+ tool | 6 ay | Tam tool sistemi |
| 50+ katman | 6 ay | Tam mimari |
| Real-time özellikler | 6 ay | Tam real-time |
| Analytics | 3 ay | Tam analytics |

---

## 17. Kaynak Gereksinimleri

### 17.1 Geliştirme Ekibi

| Rol | Sayı | Sorumluluk |
|-----|------|------------|
| Tech Lead | 1 | Mimari kararlar |
| Senior Developer | 3 | Core geliştirme |
| Mid Developer | 5 | Feature geliştirme |
| Junior Developer | 3 | Destek |
| QA Engineer | 2 | Test |
| DevOps Engineer | 1 | CI/CD |
| UI/UX Designer | 1 | Arayüz tasarımı |
| **Toplam** | **16** | — |

### 17.2 Altyapı

| Kaynak | Gereksinim |
|--------|------------|
| Geliştirme Ortamı | Visual Studio 2026, VS Code |
| CI/CD | GitHub Actions, Azure DevOps |
| Bulut | Azure, AWS |
| Veritabanı | SQLite (geliştirme), PostgreSQL (produksiyon) |
| Monitoring | Serilog, Application Insights |

---

## 18. Risk Analizi

| Risk | Olasılık | Etki | Mitigasyon |
|------|----------|------|------------|
| Kapsam genişlemesi | Yüksek | Yüksek | Phased approach |
| Teknik borç | Orta | Yüksek | Code review, refactoring |
| Güvenlik açıkları | Düşük | Yüksek | Security audit |
| Performans sorunları | Orta | Orta | Performance testing |
| Bakım yükü | Yüksek | Orta | Automation |

---

## 19. Kalite Metrikleri

| Metrik | Hedef |
|--------|-------|
| Code Coverage | %90+ |
| Bug Density | < 1 bug/KLOC |
| Mean Time to Recovery | < 4 saat |
| Deployment Frequency | Haftada 1 |
| Lead Time | 2 hafta |

---

## 20. Onay

### 20.1 Onay Bekleyen Maddeler

| # | Madde | Durum |
|---|-------|-------|
| 1 | Mimari katman yapısı (50+ katman) | ☐ Onay bekliyor |
| 2 | Agent sistemi (50+ agent) | ☐ Onay bekliyor |
| 3 | Tool sistemi (100+ araç) | ☐ Onay bekliyor |
| 4 | UI framework (çoklu platform) | ☐ Onay bekliyor |
| 5 | Embedded sistem desteği | ☐ Onay bekliyor |
| 6 | Driver geliştirme desteği | ☐ Onay bekliyor |
| 7 | Sektörel agent'lar | ☐ Onay bekliyor |
| 8 | Güvenlik seviyesi | ☐ Onay bekliyor |
| 9 | Test coverage hedefi | ☐ Onay bekliyor |
| 10 | Deployment stratejisi | ☐ Onay bekliyor |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Status:** Draft — Onay bekliyor
**Mode:** Red Team · Human Mode · Truth Mode