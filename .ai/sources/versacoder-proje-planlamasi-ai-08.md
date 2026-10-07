---
title: "VERSA CODER — 50+ Yıllık Senior Architect Katmanlı Mimari Planlama Promptu"
raw_path: "raw/versacoder-proje-planlamasi-ai-08.md"
ingested: 2026-10-06 18:59
tags: [versacoder, mimari, clean-architecture, katmanli-mimari, agent-runtime, dll, security]
---

# Versa Coder Proje Planlaması ai.08 — Katmanlı Mimari Planlama

## Genel Özet
Bu dosya (`versacoder-proje-planlamasi-ai-08.md`), Versa Coder'ın 50+ yıllık deneyim seviyesinde bir senior architect rolüyle katmanlı mimarisini planlayan kapsamlı bir prompttur (72 madde). Temel prensip "Önce mimariyi doğru kur. Sonra implementation yap." şeklindedir; bu çalışma doğrudan kod yazmak için değildir. Başlangıç mimari modeli `Presentation → Application → Agent Runtime → Domain ← Abstractions ← Infrastructure` katmanlarını ve bağımlılık yönünü tanımlar. AI Provider'lar, External Agent'lar (OpenCode/Codex/Claude Code/Abacus), Tool/MCP, Project Analysis, Knowledge (`.ai`), Persistence ve Security ayrı bounded responsibility'ler olarak ayrıştırılır. Belirli teknolojiler (database, ORM, broker, cloud, CQRS, CLI syntax) kullanıcı belirtmediyse `[BELİRTİLMELİ]`/`[DOĞRULAMA GEREKLİ]` olarak bırakılır.

## Ana Fikirler
- Versa Coder "Agentic Software Engineering & IDE Operating Platform"dur; basit UI→Service→DB mimarisi yetersizdir (ai-08.md §1).
- Mimari karar önceliği: `USER INTENT > EXPLICIT REQUIREMENTS > PROJECT CONTEXT > DOMAIN RESPONSIBILITIES > ARCHITECTURAL BOUNDARIES > DEPENDENCY DIRECTION > CORRECTNESS > SECURITY > MAINTAINABILITY > TESTABILITY > PERFORMANCE > SIMPLICITY` (ai-08.md §2).
- Domain katmanında iş kavramları bulunur (Project, Requirement, Task, Agent, Session, Workflow, Execution, Policy, Approval, Validation, KnowledgeItem vb.); DevExpress, HttpClient, DbContext, CLI command'lar Domain'de yasaktır (ai-08.md §5-7).
- Application katmanı use-case orchestration'dır (StartChatSession, AnalyzeUserIntent, ExecuteExternalAgent, ValidateResult vb.); Domain "ne olmalı", Application "nasıl yürütülmeli" (ai-08.md §8-9).
- Agent Runtime en kritik katmandır: lifecycle, task decomposition, swarm/handoff, context assembly, tool invocation, policy enforcement (ai-08.md §10-11); Orchestrator UI/db/CLI/provider-api içermez (ai-08.md §12).
- External Agent katmanı: `IExternalAgentExecutor` + Infrastructure Adapter (OpenCode/Codex/Claude Code/Abacus ayrı adapter); CLI syntax Application'a sızmaz; process execution Infrastructure concern'idir (ai-08.md §20-22).
- `.ai` Vault IKnowledgeService üzerinden erişilir, Domain Entity değildir; Memory Working/Long-Term/Session olarak ayrılır (ai-08.md §29-30). DevExpress yalnız Presentation'da (ai-08.md §38).
- Bağımlılık yasakları: `Domain → UI/DevExpress/Infrastructure/Database/AI Provider ❌`, `Application → DevExpress/CLI/Database API/Provider SDK ❌`, `UI → Infrastructure/Database/CLI ❌`; circular dependency yasak (ai-08.md §49, §54).
- Önerilen DLL organizasyonu 11 grup (~35 proje: Domain, Abstractions, Application*, AgentRuntime*, AI*, Tools/MCP, ProjectAnalysis/CodeAnalysis/Repository, ExternalAgents*, Infrastructure*, Knowledge*, Plugin*, Presentation.WinForms) — kesin değildir (ai-08.md §50).
- "50+ DLL oluşturmak tek başına iyi Architecture değildir"; boundary + responsibility + independent change gerçekten gerekiyorsa proje/DLL açılır (ai-08.md §51). Modüler monolith önceliği; Microservices, CQRS, Event Sourcing otomatik zorunluluk değildir (ai-08.md §45, §63).
- Kod yazma sırası: `UNDERSTAND → ANALYZE → MODEL → SEPARATE → VALIDATE → PLAN → APPROVE → IMPLEMENT`; her önemli karar için ADR oluşturulur (ai-08.md §60-61, §71).

## Önemli Alıntılar/Veriler
- "Önce mimariyi doğru kur. Sonra implementation yap." (ai-08.md §0).
- "Bir kavramın bulunması, otomatik olarak bir layer veya project oluşturulacağı anlamına gelmez." (ai-08.md §1).
- "Layer sayısı hedef değildir. Doğru boundary hedeftir. DLL sayısı hedef değildir. Bağımsız değiştirilebilirlik hedeftir." (ai-08.md §72).
- Son katman modeli: `PRESENTATION ↓ APPLICATION ↓ AGENT RUNTIME ↓ DOMAIN ↑ ABSTRACTIONS/CONTRACTS ↑ INFRASTRUCTURE` (ai-08.md §72).
- Doğrulama işaretleri: `[DATABASE BELİRTİLMELİ]`, `[CQRS GEREKLİLİĞİ DOĞRULANMALI]`, `[BELİRTİLMELİ]`, `[DOĞRULAMA GEREKLİ]` (ai-08.md §31, §45, §62).
- Test seviyeleri: Domain→Unit; Application→Unit+Integration; Agent Runtime→Unit+Integration+Scenario; External Agents→Contract+Integration; Presentation→UI/E2E/Smoke (ai-08.md §57).
- Kalite gate sorusu: "Bu sistemi ben bugün tasarlayıp yarın 10 yıl boyunca geliştirmek zorunda olsaydım, hangi karar beni ileride en fazla teknik borca sokardı?" (ai-08.md §70).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[versacoder-projesi]] — katmanlı mimari modeli, DLL organizasyonu ve quality gate
- [[agent-workflow]] — Agent Runtime, Orchestrator ve Workflow/DAG katman sorumlulukları
- [[opencode]] — ExternalAgents.OpenCode Infrastructure adapter'ı ve model router
- [[vault-mimarisi]] — `.ai` Knowledge katmanının IKnowledgeService ile mimari konumu
- [[ssot]] — `.ai`'ın Domain Entity olmayan SSOT kaynağı olarak mimari tanımı
