---
title: "Arşiv Kaydı — Master Prompt A: Master Enterprise Engineering (v2.0.0)"
type: prompt-archive
category: prompts
status: active
version: 2.0.0
updated: 2026-10-06
authority: SSOT
scope: "Master Enterprise Engineering Orchestration promptunun bolum listesi ve isletmen kurallari; tam metin oturumdan, diskte orijinali yok"
---

# Arşiv A — MASTER ENTERPRISE ENGINEERING & ARCHITECTURE, AGENTIC IDE & PROJECT GENERATION PROMPT

**Zorunlu Bağlantılar:** [index](../.templates/index.md) · [arşiv B](./02-master-project-installation-v1.md) · [çakışma kaydı](../reports/conflict-register-001-master-prompts.md) · [CLAUDE](../../CLAUDE.md)

## §1 Kaynak (provenance)

| Alan | Değer |
|---|---|
| Başlık | `# VERSA CODER — MASTER ENTERPRISE ENGINEERING, ARCHITECTURE, AGENTIC IDE & PROJECT GENERATION PROMPT` |
| Version | **2.0.0** |
| Role | Master Enterprise Software Engineering Orchestrator |
| Dil | Türkçe |
| Oluşturulma | 2026-10-06 oturumunda kullanıcı tarafından yapıştırıldı |
| Diskte orijinali | **YOK** — Prompt Arşivi klasöründe (`versacoder proje planlaması ai.*`) bu metne eşleşen dosya yok; `Version: 2.0.0` taraması 0 sonuç |
| Varış | `.ai/prompts/01-…` = bu kayıt. **Tam metin** istenirse kullanıcı dosyayı bu yola bırakır |
| Uygulama | 66 bölüm (§0–§66) |

## §2 Tam Bölüm Listesi (67 kayıt)

```
§0  MASTER ROLE                          §34 AI PROVIDER ARCHITECTURE
§1  MUTLAK DAVRANIŞ                      §35 PROVIDER FALLBACK
§2  VERSA CODER'IN DOĞRU TANIMI          §36 WINDOWS COMPATIBILITY
§3  TEMEL KAVRAM AYRIMLARI               §37 RUNTIME STRATEGY
§4  ANA HEDEF                            §38 DEVEXPRESS
§5  REFERANS PROJE ANALİZİ              §39 TUI / CLI
§6  `.ai` VAULT FIRST                    §40 SOLUTION / PROJECT GENERATION
§7  VAULT GATE                           §41 PROJECT / DLL KARAR ALGORİTMASI
§8  MİMARİ STRATEJİ (Modular Monolith)   §42 DEPENDENCY GOVERNANCE
§9  5.000 ARCHITECTURAL UNIT STRATEGY    §43 TESTING (xUnit primary)
§10 DOMAIN MODEL (Context Map)           §44 ARCHITECTURE TESTS
§11 CQRS                                 §45 FAILURE HANDLING
§12 DEPENDENCY DIRECTION                 §46 CANCELLATION
§13 DOMAIN/APPLICATION/AGENT RUNTIME     §47 WORKFLOW ENGINE
§14 AGENT MODEL                          §48 VALIDATION PIPELINE
§15 MULTI-AGENT                          §49 HUMAN-IN-THE-LOOP
§16 SESSION (immutable snapshot)         §50 OBSERVABILITY ARCHITECTURE
§17 SESSION AGENTIC AGENT WIZARD         §51 CONFIGURATION
§18 POLICY ENGINE                        §52 ZERO HALLUCINATION
§19 RISK MODEL (LOW..CRITICAL)           §53 IMPLEMENTATION ORDER (22 phase)
§20 TOOL GATEWAY                         §54 HER AŞAMADA GATE
§21 MCP (≠ Agent)                        §55 HER LAYER İÇİN ÇIKTI
§22 EXTERNAL CODING AGENT GATEWAY        §56 HER PROJECT/DLL İÇİN ÇIKTI
§23 EXTERNAL PROCESS CONTROL             §57 HER ADR İÇİN ÇIKTI
§24 EXTERNAL AGENT RESULT PIPELINE       §58 ARCHITECTURE DIAGRAMS
§25 CHANGE / ROLLBACK                    §59 MASTER PROJECT OUTPUT (48 §)
§26 PROJECT INTELLIGENCE                 §60 TASK BACKLOG GENERATION
§27 CONTEXT ASSEMBLY                     §61 ACCEPTANCE CRITERIA
§28 KNOWLEDGE / MEMORY                   §62 ARCHITECTURE QUALITY GATE
§29 PERSISTENCE                          §63 MASTER ARCHITECT QUESTION
§30 DATABASE EVOLUTION                   §64 EN ÖNEMLİ KURAL
§31 OBSERVABILITY + AUDIT                §65 FINAL EXECUTION RULE (20 adım)
§32 SECURITY                             §66 FINAL COMMAND
§33 PLUGIN ARCHITECTURE
```

## §3 İşletmen Kurallar (metinden birebir)

- **Etiketler (4):** `[UNKNOWN]` · `[VERIFY REQUIRED]` · `[NOT SPECIFIED]` · `CONFLICT DETECTED`
- **Karar önceliği (§5):** `USER INTENT > EXPLICIT REQUIREMENTS > EXPLICIT CONSTRAINTS > EXISTING PROJECT STRUCTURE > DOMAIN CORRECTNESS > ARCHITECTURAL INTEGRITY > SECURITY > TESTABILITY > MAINTAINABILITY > PERFORMANCE > SIMPLICITY`
- **Yaşam döngüsü:** `UNDERSTAND → DISCOVER → REVERSE ENGINEER → NORMALIZE → DEFINE → MODEL → ARCHITECTURE → DESIGN → PLAN → VALIDATE → GENERATE → IMPLEMENT → TEST → REVIEW → HARDEN → DOCUMENT → RELEASE → OPERATE → IMPROVE`
- **Uygulama sırası (§53):** PHASE 0 Reference Discovery → 1 `.ai` Vault → 2 Requirements → 3 Domain/Context Map → 4 Architecture → 5 Contracts → 6 Persistence Abstraction → 7 Core Application → 8 Agent Runtime → 9 Policy/Security → 10 AI Provider → 11 Tool Gateway → 12 MCP → 13 Project Intelligence → 14 External Agent Gateway → 15 Workflow/Session → 16 Validation/Audit → 17 WinForms/DevExpress IDE → 18 TUI/CLI → 19 Integration → 20 Hardening → 21 Release
- **Layer modeli (§12):** `Presentation → Application → Domain`; Infrastructure contract'ları *implement* eder. DevExpress = **Presentation Boundary**.
- **Ana ayrımlar (§3):** `Chat ≠ Agent` · `Agent ≠ LLM` · `LLM ≠ Versa Coder` · `Session ≠ Chat` · `Workflow ≠ Agent` · `Tool ≠ Agent` · `MCP ≠ Agent` · `AI Provider ≠ External Coding Agent` · `Internal ≠ External Agent` · **`Control Plane ≠ Execution Plane`** (Versa Coder = control, external agent = execution)
- **Runtime stratejisi:** ".NET Framework 4.8 + modern .NET runtime strategy, **exact compatibility to be verified**" → §36 `SUPPORTED / PARTIALLY SUPPORTED / UNSUPPORTED / VERIFY REQUIRED`; §37 `Legacy Host / Modern Host / Shared Core`
- **Persistence (§29):** "Persistence Abstraction + **SQLite candidate**"; `ADO.NET / Microsoft.Data.Sqlite` uygun aday, "implementation öncesi compatibility doğrulanmalıdır" → `[VERIFY REQUIRED]`
- **Test (§43):** **xUnit primary**; ek framework yalnız özel gerekçeyle
- **§62 Quality Gate kontrolü (23 madde):** Domain bağımsız mı · UI core'a sızıyor mu · DevExpress dış katmana sızıyor mu · Agent Runtime UI'ya bağımlı mı · External CLI syntax Application'a sızıyor mu · MCP Domain'e sızıyor mu · circular dependency · policy runtime'da enforce · approval öncesinde · session immutable · rollback · external process iptal · audit · observability · validation · security boundary · test strategy · `.ai` authority · persistence abstraction · runtime compatibility doğrulandı mı · gereksiz DLL · gereksiz microservice · unknown'lar işaretli mi
- **§66 FINAL COMMAND:** "C:\versacoder altında sıfırdan, referans projelerden öğrenilmiş ama körlemesine kopyalanmamış, `.ai` vault-first, modular-monolith-first, Clean Architecture, SOLID, DDD-aware, CQRS-aware, Agentic Runtime merkezli, Policy-controlled, MCP-enabled, External Coding Agent destekli, immutable-session tabanlı, persistence abstraction kullanan, güçlü audit/observability içeren, test edilebilir ve gelecekte modülleri bağımsız ayrılabilen enterprise bir yazılım sistemi olarak analiz et ve planla."

## §4 Vault ile İlişki (çelişenler → kayıt)

| § | Prompt A | Vault | Kayıt |
|---|---|---|---|
| §12/§40 | 3 katman / 14 dizin | `l0..l7` (8) | **CF-01** |
| §54 | "her phase sonunda gate" | 5 kapı | **CF-02** + **D2 ✓** |
| §53 | 22 phase | B 15 phase | **D2 ✓ — bu prompt kazandı** |
| §29 | SQLite *aday*, `[VERIFY REQUIRED]` | G2 kesin: ADO.NET + Dapper, EF Core yasak (ADR-005) | vault kazanır |
| §40/§36 | DevExpress yalnız Presentation | `l7` Presentation sınırı | uyumlu |
| §43 | xUnit | G11 "xunit + coverage" | uyumlu |
| §6 | `.ai/` 19 klasör | `.ai/` = yalnız `.templates/` | **CF-12** + **D3 ✓ → Adım 4 girdisi** |

## §5 Doğrulama

```powershell
$f='C:\versacoder\.ai\prompts\01-master-enterprise-engineering-v2.md'
(Get-Content $f -Encoding UTF8).Count; (Get-Item $f).Length/1KB
([System.IO.File]::ReadAllBytes($f))[0..2] -join ','
```
Beklenen: `45,45,45` (`---`), kırık link 0.

## §6 Referanslar

Tam metin 3–8 KB kuralı nedeniyle kopyalanmadı (`.ai/prompts/01-…-v2.full.md`). Çakışmalar: [çakışma kaydı](../reports/conflict-register-001-master-prompts.md).
