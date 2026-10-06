---
title: "Arşiv Kaydı — Master Prompt B: Master Project Installation (v1.0.0)"
type: prompt-archive
category: prompts
status: active
version: 1.0.0
updated: 2026-10-06
authority: SSOT
scope: "Master Project Installation & Coding Plan promptunun bolum listesi ve isletmen kurallari; tam metin oturumdan, diskte orijinali yok"
---

# Arşiv B — VERSA CODER: MASTER PROJECT INSTALLATION, ARCHITECTURE & CODING PLAN PROMPT

**Zorunlu Bağlantılar:** [index](../.templates/index.md) · [arşiv A](./01-master-enterprise-engineering-v2.md) · [çakışma kaydı](../reports/conflict-register-001-master-prompts.md) · [CLAUDE](../../CLAUDE.md)

## §1 Kaynak (provenance)

| Alan | Değer |
|---|---|
| Başlık | `# VERSA CODER — MASTER PROJECT INSTALLATION, ARCHITECTURE & CODING PLAN PROMPT` |
| Version | **1.0.0** · Author: **Bayram Ali Akşit** |
| Role | Master Project Planning & Engineering Orchestrator |
| Dil | Türkçe |
| Oluşturulma | 2026-10-06 oturumunda, tek mesajda Prompt A ardına yapıştırıldı |
| Diskte orijinali | **YOK** — Prompt Arşivi taramasında başlık/`Version: 1.0.0` eşleşmesi 0 |
| Varış | `.ai/prompts/02-…` = bu kayıt; tam metin kullanıcı tarafından bırakılabilir |
| Uygulama | 83 bölüm (§0–§82) |

## §2 Tam Bölüm Listesi (83 kayıt)

```
§0  SYSTEM ROLE                          §42 WORKFLOW ENGINE
§1  DOĞRU KONUMLANDIRMA                  §43 DAG / TASK DECOMPOSITION
§2  ANA MİSYON                           §44 GIT / VERSION CONTROL
§3  ZERO-HALLUCINATION (6 etiket)        §45 TEST STRATEGY
§4  KAYNAK HİYERARŞİSİ (8 katman)        §46 PERFORMANCE
§5  DECISION PRIORITY                    §47 DOCUMENTATION SYSTEM
§6  DISCOVERY AŞAMASI                    §48 ADR SYSTEM
§7  REQUIREMENT DISCOVERY (REQ-ID)       §49 DECISION LEDGER
§8  SCOPE MANAGEMENT                     §50 RISK REGISTER
§9  SYSTEM CONTEXT                       §51 TECHNICAL DEBT
§10 DOMAIN ANALYSIS                      §52 IMPLEMENTATION ORDER
§11 ARCHITECTURE PLANNING (6 katman)     §53 MILESTONE PLAN
§12 DEPENDENCY DIRECTION                 §54 PHASE PLAN (15 phase)
§13 SOLUTION PLAN                        §55 TASK BACKLOG
§14 DLL / MODULE PLAN                    §56 TASK GRANULARITY
§15 AGENT RUNTIME PLAN                   §57 CODING PLAN
§16 AGENT MODEL                          §58 CHANGE POLICY
§17 SINGLE vs MULTI AGENT                §59 QUALITY GATES (GATE-000..014)
§18 ORCHESTRATOR                         §60 VALIDATION MODEL
§19 SESSION PLAN (3 sorumluluk)          §61 TRACEABILITY
§20 SESSION AGENTIC AGENT WIZARD         §62 DEPENDENCY GRAPH
§21 PROMPT SYNTHESIS ENGINE              §63 ANTI-PATTERN CHECK
§22 AI PROVIDER ARCHITECTURE             §64 COMPLEXITY CONTROL
§23 MODEL ROUTER                         §65 FINAL PROJECT ROADMAP
§24 OPENCODE / EXTERNAL AGENTS           §66 FINAL OUTPUT DOCUMENT SET
§25 EXTERNAL AGENT ADAPTER               §67 STANDARD FORMAT (17 §)
§26 PROCESS EXECUTION (state machine)    §68 OPEN QUESTIONS ENGINE
§27 WORKING DIRECTORY / ENVIRONMENT      §69 CONFLICT ENGINE
§28 TOOL GATEWAY                         §70 REVIEW ENGINE
§29 MCP ARCHITECTURE                     §71 FINAL QUALITY GATE
§30 POLICY ENGINE                        §72 RESPONSE BEHAVIOR
§31 APPROVAL GATES                       §73 DEEP ANALYSIS RULE
§32 PROJECT ANALYSIS / CODE INTELLIGENCE §74 ENGINEERING TRADE-OFF
§33 KNOWLEDGE VAULT / .AI                §75 IMPLEMENTATION READINESS
§34 MEMORY ARCHITECTURE                  §76 DONE DEFINITION
§35 PERSISTENCE                          §77 RELEASE READINESS
§36 CONFIGURATION                        §78 MASTER EXECUTION FLOW
§37 SECURITY ARCHITECTURE                §79 FINAL OUTPUT FORMAT
§38 PLUGIN ARCHITECTURE                  §80 MASTER RULE
§39 OBSERVABILITY                        §81 MASTER ENGINEERING PRINCIPLE
§40 ERROR HANDLING                       §82 FINAL MISSION
§41 SELF-HEALING
```

## §3 İşletmen Kurallar (metinden birebir)

- **Etiketler (6):** `[NOT SPECIFIED]` · `[VERIFY REQUIRED]` · `[CONFLICT]` · `[INFERENCE]` · `[WEB VERIFIED]` · `[REFERENCE VERIFIED]`
- **Karar önceliği (§5):** `USER INTENT > EXPLICIT REQUIREMENTS > EXPLICIT CONSTRAINTS > EXISTING PROJECT STRUCTURE > DOMAIN CORRECTNESS > ARCHITECTURAL INTEGRITY > SECURITY > TESTABILITY > MAINTAINABILITY > PERFORMANCE > SIMPLICITY` *(Prompt A ile aynı)*
- **Kaynak hiyerarşisi (§4):** `1 Explicit User Requirement · 2 Existing Versa Coder Documentation · 3 Existing .ai Vault · 4 Existing Source Code · 5 Existing Architecture/ADR · 6 Official Technical Documentation · 7 Verified External Reference · 8 Engineering Inference`
- **Layer modeli (§11):** `PRESENTATION → APPLICATION → AGENT RUNTIME → DOMAIN → ↑ABSTRACTIONS/CONTRACTS → ↑INFRASTRUCTURE`
- **Uygulama sırası (§52):** `FOUNDATION → DOMAIN → CONTRACTS → APPLICATION → RUNTIME → INFRASTRUCTURE → PERSISTENCE → AI PROVIDERS → TOOLS/MCP → EXTERNAL AGENTS → PRESENTATION → INTEGRATION → TESTING → HARDENING → RELEASE`
- **Phase planı (§54, 15):** `PHASE 0 DISCOVERY · 1 FOUNDATION · 2 DOMAIN · 3 APPLICATION · 4 AGENT RUNTIME · 5 AI PROVIDERS · 6 TOOL/MCP · 7 EXTERNAL AGENTS · 8 KNOWLEDGE/MEMORY · 9 SECURITY/POLICY · 10 PRESENTATION · 11 INTEGRATION · 12 TESTING · 13 HARDENING · 14 RELEASE`
- **Quality Gates (§59, 15):** `GATE-000 DISCOVERY COMPLETE · 001 REQUIREMENTS FREEZE · 002 ARCHITECTURE · 003 SOLUTION STRUCTURE · 004 FOUNDATION BUILD · 005 DOMAIN · 006 APPLICATION · 007 RUNTIME · 008 INFRASTRUCTURE · 009 AI INTEGRATION · 010 TOOL/MCP · 011 EXTERNAL AGENT · 012 UI · 013 INTEGRATION TESTED · 014 RELEASE READY` → vault'un 5 kapısıyla **çelişir (CF-02)**.
- **§68 Open Questions:** kritikse `STOP — REQUIRED INFORMATION MISSING`; kritik değilse placeholder ile ilerle → **Prompt A §66 "gereksiz soru sorma" ile gerilim (CF-11)**
- **§67 standart dosya formatı (17 §):** frontmatter `id,title,type,status,version,source,authority,created,updated` + `Objective · Scope · Context · Requirements · Architecture · Dependencies · Constraints · Risks · Decisions · Implementation · Validation · Acceptance Criteria · Evidence · Open Questions · References`
- **§69 Conflict Engine:** `CONFLICT-ID · Source A · Source B · Conflict · Impact · Authority · Resolution · Verification` → bu vault'ta [çakışma kaydı](../reports/conflict-register-001-master-prompts.md) bu formatta uygulandı.
- **§71 RED kriterleri:** Hallucinated Requirement · Unverified Technology Presented as Fact · Hidden Dependency · Circular Architecture · Undefined Critical Requirement · Unvalidated Security Assumption · Missing Acceptance Criteria · Unclear Implementation Order · Uncontrolled Scope Expansion
- **§81 öncelik:** `CORRECTNESS > ARCHITECTURAL INTEGRITY > SECURITY > TRACEABILITY > TESTABILITY > MAINTAINABILITY > PERFORMANCE > SPEED`; yasaklar `NO ASSUMPTION · NO HALLUCINATION · NO HIDDEN DECISION · NO UNCONTROLLED COMPLEXITY · NO UNVERIFIED CLAIM · NO UNNECESSARY REFACTOR · NO REQUIREMENT DRIFT`

## §4 Vault ile İlişki (çelişenler → kayıt)

| § | Prompt B | Vault | Kayıt |
|---|---|---|---|
| §59 | 15 gate | WORKFLOW 5 kapı | **CF-02** |
| §54 | 15 phase | **Prompt A 22 phase** (D2 ✓) | **CF-11** kapandı |
| §3 | 6 etiket | Prompt A 4 etiket | **CF-14** |
| §68 | kritikse **STOP** | Prompt A "soru sorma" | **CF-11** (D2: kritikse dur) |
| §66 | `.ai/` 15 klasör | **Prompt A §6 = 19** (D3) | **CF-12** → Adım 4 girdisi |
| §35 | "kullanıcı belirtmediyse uydurma" | G2 kesin ADO.NET + Dapper | vault kazanır |
| §67 | 9 alanlı frontmatter | vault 8 alan (`scope`) | **CF-03** |

## §5 Doğrulama

`(Get-Content $f).Count` · `(Get-Item $f).Length/1KB` · ilk 3 bayt `45,45,45` (`---`) · kırık link 0.

## §6 Referanslar

Özet kayıt — 3–8 KB kuralı nedeniyle tam metin (≈55 KB) kopyalanmadı (`.ai/prompts/02-…-v1.full.md`). Etiket seti + çelişki motoru B'den (CF-14).
