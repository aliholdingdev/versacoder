---
title: "Enterprise Polyglot Software Prompt Architect — Adaptive Question → Prompt Maker"
raw_path: "raw/ai-agentic-promt-maker.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, prompt-engineering, enterprise-architect, ddd-cqs, adaptive-questioning, zero-hallucination]
---

# Enterprise Polyglot Software Prompt Architect (AI Agentic IDE — Prompt Maker)

## Genel Özet
`ai-agentic-promt-maker.md`, kullanıcının ham/dağınık isteğini doğrudan kullanılabilir, test edilebilir ve teknolojiye uygun bir AI Prompt'a dönüştüren, polyglot (çok dilli) ve Enterprise odaklı iki katmanlı bir sistem promptudur (Version 1.0.0, Bayram Ali Akşit) (ai-agentic-promt-maker.md). İlk katman Template Engine'dir: intent, requirements, constraints, architecture, security, testing gibi alanları çıkarıp SYSTEM/TASK/CODE REVIEW/SECURITY AUDIT/REVERSE ENGINEERING/REFACTOR/ARCHITECTURE/MIGRATION template'lerinden birine oturtur (ai-agentic-promt-maker.md). İkinci katman (v1.1) ise bu promptun içine gömülü "ENTERPRISE POLYGLOT INTERACTIVE PROMPT MAKER"dır: ilk mesajda prompt üretmez, adaptif soru ağacıyla (discovery) ihtiyacı keşfeder ve `DISCOVERY COMPLETE` olmadan final üretmez (ai-agentic-promt-maker.md). Dosya, DDD/CQRS, güvenlik, veritabanı, API, legacy modernization ve anti-pattern tespiti için ayrı soru dalları tanımlar (ai-agentic-promt-maker.md).

## Ana Fikirler
- Rol: Enterprise Software Architect, Principal Engineer, AI Prompt Engineer, Security Architect, Reverse Engineering Specialist, Code Auditor, DDD/CQRS Architect, Legacy Modernization Architect (ai-agentic-promt-maker.md).
- Ana prensip: `HAM INPUT → NORMALIZE → INTENT → CONTEXT → REQUIREMENTS → CONSTRAINTS → TECHNICAL ANALYSIS → ARCHITECTURE → WORKFLOW → OUTPUT CONTRACT → VALIDATION → FINAL PROMPT` (ai-agentic-promt-maker.md).
- Polyglot kapsamı: PHP, C/C++/C#, Java, Kotlin, Swift, Go, Rust, Python, JS/TS, Dart, Ruby, Perl, Lua, R, Scala, Delphi, VB/VBA, Fortran, COBOL, Assembly (x86/x64/ARM/ARM64/MIPS/RISC-V yalnızca belirtilmiş veya doğrulanmışsa); teknoloji belirtilmezse `[LANGUAGE]`, `[FRAMEWORK]`, `[DATABASE]` placeholder'ı kullanılır (ai-agentic-promt-maker.md).
- Enterprise mimari kuralı: SOLID, Clean/Hexagonal, DDD, CQRS, Event-Driven, Microservices vb. yalnızca gereksinim destekliyorsa kullanılır; **CQRS ≠ otomatik Event Sourcing** (ai-agentic-promt-maker.md).
- Adaptif Discovery: statik questionnaire yok; her cevap analiz edilir, eksik bilgi belirlenir, yeni soru ağacı açılır (`CEVAP A → ANALİZ → EKSİK BİLGİ → SORU B`) (ai-agentic-promt-maker.md).
- Question Memory: `[ANSWERED] Language = PHP` gibi internal state tutulur, kesinleşen bilgi tekrar sorulmaz, çelişkide `CONFLICT DETECTED` gösterilir (ai-agentic-promt-maker.md).
- Zero-hallucination: version, API, dependency, file path, database, architecture, benchmark, CVE uydurulmaz; `[UNKNOWN]`, `[NOT PROVIDED]`, `[VERIFY REQUIRED]` kullanılır (ai-agentic-promt-maker.md).
- Reverse engineering modu: kod/ZIP/binary verilirse önce 20 adımlık analiz (file tree → build → dependencies → ... → technical debt), hemen rewrite yapılmaz; big-bang rewrite varsayılan değildir (ai-agentic-promt-maker.md).

## Önemli Alıntılar/Veriler
- **Rol:** "Enterprise Software Architect, Principal Engineer, AI Prompt Engineer, Security Architect, Reverse Engineering Specialist, Code Auditor, DDD/CQRS Architect, Legacy Modernization Architect ve Technical Documentation Specialist" (ai-agentic-promt-maker.md)
- **Bağlam girişi:** ham metin, kod deposu, ZIP, binary veya teknik gereksinim (ai-agentic-promt-maker.md).
- **Kısıtlar:** kullanıcı teknik amacı değiştirilemez; belirtilmeyen teknoloji/version/database/architecture gerçek kabul edilmez; gereksiz Enterprise complexity eklenemez (ai-agentic-promt-maker.md).
- **Template Engine (8 şablon):**
  ```text
  SYSTEM PROMPT:        ROLE → CONTEXT → OBJECTIVE → RULES → CONSTRAINTS → WORKFLOW → OUTPUT → VALIDATION
  TASK PROMPT:          TASK → INPUT → REQUIREMENTS → CONSTRAINTS → ACCEPTANCE CRITERIA → OUTPUT
  CODE REVIEW:          CONTEXT → CODE → ANALYSIS → FINDINGS → RISK → FIX → VERIFICATION
  SECURITY AUDIT:       SCOPE → ASSET → ATTACK SURFACE → THREATS → FINDINGS → IMPACT → MITIGATION → VERIFICATION
  REVERSE ENGINEERING:  INPUT → STRUCTURE → DEPENDENCIES → CALL FLOW → DATA FLOW → SECURITY → ARCHITECTURE → REPORT
  REFACTOR:             CURRENT STATE → PROBLEMS → ROOT CAUSE → TARGET STATE → SAFE CHANGES → TESTING → ROLLBACK
  ARCHITECTURE:         REQUIREMENTS → CONTEXT → DOMAIN → COMPONENTS → DATA FLOW → API → SECURITY → SCALABILITY → TRADE-OFFS → ADR
  MIGRATION:            CURRENT STATE → TARGET STATE → GAP → STRATEGY → PHASES → COMPATIBILITY → RISK → ROLLBACK → VALIDATION
  ```
  (ai-agentic-promt-maker.md)
- **Soru önceliği (18 kademe):** Amaç, Problem, Project Context, Technology Stack, Existing System, Functional/Technical Requirements, Architecture, Data, API, Security, Performance, Infrastructure, Testing, Deployment, Documentation, Expected Output, Acceptance Criteria (ai-agentic-promt-maker.md).
- **Soru derinliği:** WHAT/WHY/HOW/WHO/WHEN/WHERE/CONSTRAINT/FAILURE CASE/SECURITY IMPACT/PERFORMANCE IMPACT/ACCEPTANCE CRITERIA (ai-agentic-promt-maker.md)
- **Karar sırası:** 1. User Intent, 2. Explicit Requirements, 3. Explicit Constraints, 4. Existing Project Structure, 5. Correctness, 6. Security, 7. Maintainability, 8. Performance, 9. Simplicity (ai-agentic-promt-maker.md)
- **Token-limit modu:** çıktı `PART 1/N` diye bölünür; devam komutu `CONTINUE FROM LAST VERIFIED SECTION` (ai-agentic-promt-maker.md)
- **Ana döngü:** `ASK → RECEIVE → ANALYZE → UPDATE REQUIREMENTS → DETECT GAPS → ASK NEXT → VALIDATE → GENERATE PROMPT` (ai-agentic-promt-maker.md)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[agentic-prompt-maker]] — adaptive discovery, question engine/memory/batching, completion gate ve template engine.
- [[prompt-engineering]] — 8 prompt template'i, output contract, karar önceliği ve validation checklist.
- [[bayram-ali-prompt-arsivi]] — dosyanın Prompt Arşivi üyesi olduğu (Version 1.0.0).
