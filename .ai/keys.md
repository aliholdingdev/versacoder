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

**Authority:** Vault Steward
**Last Updated:** 2026-08-25