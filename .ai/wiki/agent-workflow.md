---
title: Ajan İş Akışı (Agent Workflow)
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/versacoder-proje-planlamasi-ai-02.md
  - raw/versacoder-proje-planlamasi-ai-05.md
  - raw/versacoder-proje-planlamasi-ai-08.md
tags: [agent, workflow, orchestrator, orkestrasyon]
---

# Ajan İş Akışı (Agent Workflow)

**Genel akış:** `USER → CHAT → INTENT → CONTEXT → TASK → AGENT SELECTION → SESSION → WORKFLOW → EXECUTION → VALIDATION → RESULT → CHAT` (ai-05 §2).

**6 aşamalı yaşam döngüsü:** Intent & AST Analysis → Session Wizard → Swarm Allocation (DAG) → Execution/Tools/MCP → Verification & Self-Healing → Persistence & Audit Sync (ai-02).

**Roller:** Orchestrator, Architect, Developer, Reviewer/Quality, Security & Policy, Testing, Sektörel Uzman Ajanlar. Multi-Agent Swarm zinciri: Orchestrator → Architect/Developer/Researcher → Reviewer → Security → Test → Verify (ai-00, ai-02).

**Sorumluluk ayrımı:** Versa Coder = **control plane**, harici ajan (OpenCode vb.) = **execution plane**. External CLI, Versa Coder'ın Agent Runtime'ı değildir ve global policy'yi bypass edemez (ai-05 §12, §30).

**Süreç durumları:** `CREATED → VALIDATED → STARTING → RUNNING → OUTPUT_STREAMING → COMPLETED`; dallar `FAILED / CANCELLED / TIMEOUT`. Sonuç tipleri: `SUCCESS, PARTIAL_SUCCESS, FAILED, CANCELLED, TIMEOUT, REQUIRES_APPROVAL, REQUIRES_USER_INPUT, VALIDATION_FAILED` (ai-05 §15, §23).

**Validation kuralı:** Ajan "tamamlandı" dese bile sonuç bağımsız validation'dan geçmeden **VERIFIED sayılmaz**; Reviewer onaylamadan süreç kapanmaz (ai-05 §24, ai-02).

**Felsefe:** "AI'a kod yazdırmak değildir… kontrollü yazılım mühendisliği yapmaktır." Öncelik: `USER INTENT > REQUIREMENTS > CONTEXT > POLICY > CORRECTNESS > SECURITY > VALIDATION` (ai-05 §48, §51).

## İlgili Sayfalar
- [[versacoder-projesi]] — bu akışın ait olduğu platform
- [[session-wizard]] — akışın başlatıldığı arayüz
- [[insan-onayi-ve-denetlenebilirlik]] — Approval Gate'ler
- [[anti-halüsinasyon-kurallari]] — Self-Healing ve guardrail'ler
- [[prompt-engineering]] — role özel system prompt sentezi
- [[opencode]] — harici execution plane
