---
title: "VERSA CODER — Agents & Chat + External Coding Agent Orchestration (System Prompt)"
raw_path: "raw/versacoder-proje-planlamasi-ai-05.md"
ingested: 2026-10-06 18:59
tags: [versacoder, agents-chat, external-agent, orchestration, opencode, fallback, validation]
---

# Versa Coder Proje Planlaması ai.05 — Agents & Chat + External Agent Orchestration

## Genel Özet
Bu dosya (`versacoder-proje-planlamasi-ai-05.md`), Versa Coder'ın Agents & Chat sistemini, Session Agentic Agent Wizard'ını ve harici coding-agent'ların (OpenCode, Codex, Claude Code, Abacus AI CLI) arka planda kontrollü çalıştırılmasını tanımlayan bir sistem promptu/análiz dokümanıdır. Temel ayrım `Chat ≠ Agent ≠ LLM ≠ Versa Coder` ve `External CLI ≠ Versa Coder Agent Runtime` olarak korunur. Versa Coder bir kontrol/orchestration katmanı, external agent ise execution katmanı olarak modellenir. Free Model sistemi çalışmadığında external CLI execution bir fallback yolu olarak tanımlanır. Bilinmeyen CLI/API/versiyon/fiyat bilgileri doğrulanmadan yazılmaz, `[VERIFY REQUIRED]` olarak işaretlenir.

## Ana Fikirler
- Chat klasik chatbot (`User→Prompt→LLM→Answer`) değil; `USER→CHAT→INTENT→CONTEXT→REQUIREMENTS→TASK→AGENT SELECTION→SESSION→WORKFLOW→EXECUTION→VALIDATION→RESULT→CHAT` akışının başlangıç noktasıdır (ai-05.md §2-3).
- Agent yalnız prompt ya da LLM değildir; role, responsibility, context, capability, tool access, permission, policy, workflow, session, model, execution, validation, output ile ilişkilendirilen kontrollü çalışma birimidir (ai-05.md §4).
- Internal Agent (Versa Coder Agent Runtime) ile External Coding Agent ayrımı; external provider'lar OpenCode, Codex, Claude Code, Abacus AI CLI olarak sıralanır (ai-05.md §5).
- OpenCode fallback stratejisi: Free Model varsa kullanılır; yoksa OpenCode arka plan coding-agent execution provider olarak çalıştırılır — Model/API Path ile CLI Agent Path birbirinden ayrılır (ai-05.md §7).
- External Agent Execution Adapter kavramı `IExternalAgentExecutor` altında OpenCodeExecutor/CodexExecutor/ClaudeCodeExecutor/AbacusAiCliExecutor olarak tanımlanır; gerçek executable/command syntax doğrulanmadan yazılmaz (ai-05.md §8-11).
- Process lifecycle: `CREATED→VALIDATED→STARTING→RUNNING→OUTPUT_STREAMING→COMPLETED`; hata/iptal/timeout için ayrı dallar (`FAILED`, `CANCELLING→CANCELLED`, `TIMEOUT→TERMINATION`) (ai-05.md §15).
- Human-in-the-loop (silme, DB değişikliği, yıkıcı komut, security/deployment config için onay), audit kaydı ve bağımsız validation zorunludur; agent "tamamlandı" dese dahi sonuç `VERIFIED` sayılmaz (ai-05.md §24, §31-32).
- Zero-hallucination: CLI command, executable adı/path, version, API endpoint, auth method, pricing, free-tier durumu doğrulanmadan yazılmaz (ai-05.md §46).

## Önemli Alıntılar/Veriler
- "Chat ≠ Agent / Agent ≠ LLM / LLM ≠ Versa Coder" ve "External CLI ≠ Versa Coder Agent Runtime" (ai-05.md §1).
- "VERSA CODER = ORCHESTRATION / CONTROL PLANE" / "EXTERNAL AGENT = EXECUTION / CODING PLANE" (ai-05.md §12, §49).
- Fallback zinciri örneği: `PRIMARY → Internal Free Model → OpenCode → Codex → Claude Code → Abacus AI CLI` — "Bu sıra yalnızca örnektir" (ai-05.md §26).
- Sonuç tipleri: `SUCCESS, PARTIAL_SUCCESS, FAILED, CANCELLED, TIMEOUT, REQUIRES_APPROVAL, REQUIRES_USER_INPUT, VALIDATION_FAILED` (ai-05.md §23).
- Öncelik sırası: `USER INTENT > EXPLICIT REQUIREMENTS > PROJECT CONTEXT > POLICY > CORRECTNESS > SECURITY > VALIDATION > MAINTAINABILITY > PERFORMANCE > SIMPLICITY` (ai-05.md §51).
- "AI'a kod yazdırmak değildir... AI Agent'lar ile kontrollü yazılım mühendisliği yapmaktır." (ai-05.md §48).
- Felsefe: "LLM = Probabilistic Intelligence; Versa Coder Control Plane = Deterministic Rules + Policy + State + Validation" (ai-05.md §45).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[versacoder-projesi]] — external coding agent execution provider modeli, fallback zinciri ve control-plane felsefesi
- [[opencode]] — OpenCode fallback strategy: Model/API path vs CLI agent path ayrımı
- [[agent-workflow]] — chat→intent→wizard→orchestrator→execution→validation akışının tanımı
- [[prompt-engineering]] — zero-hallucination / `[VERIFY REQUIRED]` kuralı ve dynamic system prompt sentezi
- [[multi-llm-bagimsizlik]] — external agent'ların Versa Coder kontrolünü bypass etmemesi
