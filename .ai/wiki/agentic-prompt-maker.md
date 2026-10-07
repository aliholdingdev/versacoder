---
title: Agentic Prompt Maker
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/ai-agentic-promt-maker.md
tags: [prompt-maker, discovery, ddd-cqrs, polyglot]
---

# Agentic Prompt Maker (`ai-agentic-promt-maker`)

**Künye:** "ENTERPRISE POLYGLOT SOFTWARE PROMPT ARCHITECT v1.0" + gömülü **v1.1 interactive maker**; Bayram Ali Akşit.

**Rol:** Enterprise Software Architect + AI Prompt Engineer + Security Architect + DDD/CQRS Architect + Reverse Engineering Specialist + Code Auditor.

**Polyglot kapsamı:** PHP, C/C++/C#, Java, Kotlin, Swift, Go, Rust, Python, JS/TS, Dart, Ruby, Assembly (x86/x64/ARM/ARM64/RISC-V) — **yalnız belirtilmişse**. Belirtilmezse `[LANGUAGE]`, `[DATABASE]`.

**v1.1 kuralı:** İlk mesajda prompt üretilmez; **adaptif soru ağacı** açılır. `DISCOVERY COMPLETE` + kullanıcı onayı olmadan final yoktur.

**Question Memory:** `[ANSWERED] Framework = Symfony` — kesinleşen soru tekrarlanmaz, çelişkide `CONFLICT DETECTED` gösterilir.

**8 template:** SYSTEM PROMPT · TASK PROMPT · CODE REVIEW · SECURITY AUDIT · REVERSE ENGINEERING · REFACTOR · ARCHITECTURE · MIGRATION.

**Karar sırası:** `User Intent → Explicit Requirements → Constraints → Project Structure → Correctness → Security → Maintainability → Performance → Simplicity`.

**Zero-hallucination placeholder:** `[UNKNOWN]`, `[NOT PROVIDED]`, `[VERIFY REQUIRED]`; benchmark/CVE uydurulmaz. "CQRS seçildi diye Event Sourcing varsayma"; big-bang rewrite varsayılan değildir.

**Token-limit modu:** `PART 1/N` + `CONTINUE FROM LAST VERIFIED SECTION`.

## İlgili Sayfalar
- [[prompt-engineering]] — ortak kural seti
- [[yazim-duzeltme-prompt]] — kardeş prompt maker
- [[anti-halüsinasyon-kurallari]] — placeholder/doğrulama ilkesi
- [[bayram-ali-prompt-arsivi]] — kaynak koleksiyon
- [[chatgpt]] — hedef model
