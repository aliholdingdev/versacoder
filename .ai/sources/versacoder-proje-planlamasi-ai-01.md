---
title: "Versa Coder — Proje Nedir? (ai-01)"
raw_path: "raw/versacoder-proje-planlamasi-ai-01.md"
ingested: 2026-10-06 19:01
tags: [versacoder, proje-tanimi, agentic-platform, agent, session, workflow, policy]
---

# versacoder-proje-planlamasi-ai-01.md

## Genel Özet

Dosya, Versa Coder'ın ne olduğunu tek, çok uzun bir tanımlayıcı metinde (başlık: "Proje Nedir? Sorusunun Cevabı") açıklayan kavramsal belgedir (`versacoder-proje-planlamasi-ai-01.md`). Versa Coder'ı IDE, code editor, chatbot veya AI code assistant olarak değil; doğal dildeki yazılım mühendisliği görevlerini anlayan, yapan, Session/Workflow üzerinden yöneten **Agentic Software Engineering & IDE Operating Platform** olarak tanımlar (`versacoder-proje-planlamasi-ai-01.md`). Metin; Agent, Session, Workflow, Tool Gateway, Policy Engine, MCP, Plugin, Memory/Knowledge, Observability/Auditability, Human-in-the-Loop, Provider Abstraction, Context Awareness, Validation/Review gibi bileşenlerin neden gerekli olduğunu ve birbirinden nasıl ayrılması gerektiğini anlatır (`versacoder-proje-planlamasi-ai-01.md`). Kesin mimari, sınıf, DLL, teknoloji seçimi bu belgede verilmez; belgenin amacı "sistem ne olduğu"nu açıklamaktır (`versacoder-proje-planlamasi-ai-01.md`).

## Ana Fikirler

- Sistemin merkezi UI değil, Agent + Session + Workflow + Tool + MCP + Memory + Knowledge + Model + Policy bileşenleridir; UI yalnızca kontrol yüzüdür (`versacoder-proje-planlamasi-ai-01.md`).
- Chat ≠ Agent, Agent ≠ LLM, LLM ≠ Versa Coder: LLM yalnızca üretim/analiz/karar destek bileşenidir, sistem davranışı LLM'e bırakılmaz (`versacoder-proje-planlamasi-ai-01.md`).
- Session bir chat geçmişi değil, mühendislik çalışmasının kontrollü çalışma alanıdır (amaç, bağlam, agent yapılandırması, workflow, tool'lar, kararlar) (`versacoder-proje-planlamasi-ai-01.md`).
- Workflow görevin gerçek ihtiyacına göre kurulur: "gereksinim önce, complexity sonra"; gereksiz agent/service/abstraction üretilmez (`versacoder-proje-planlamasi-ai-01.md`).
- Tool Gateway ve Policy Engine olmadan agent'ların sınırsız erişimi güvenli değildir; agent permission boundary ve tool permission boundary kavramları önemlidir (`versacoder-proje-planlamasi-ai-01.md`).
- Karar önceliği: User Intent > Explicit Requirements > Existing Context > Correctness > Security > Maintainability > Performance > Simplicity (`versacoder-proje-planlamasi-ai-01.md`).
- AI + deterministic engineering ayrımı: derleme compiler, test test runner ile doğrulanır; LLM probabilisttir, "Zero Hallucination" "LLM asla hata yapmaz" değil, "doğrulanmamış bilgi gerçek kabul edilmez" anlamındadır (`versacoder-proje-planlamasi-ai-01.md`).
- `.ai` bir Second Brain'dir: proje için Single Source of Truth niteliğinde kontrollü AI Knowledge Layer; doğrulanmamış bilgi gerçek sayılmaz (`versacoder-proje-planlamasi-ai-01.md`).

## Önemli Alıntılar/Veriler

- "Versa Coder, ... Agentic Software Engineering & IDE Operating Platform'dur" (`versacoder-proje-planlamasi-ai-01.md`)
- "LLM sistemin tamamı değil, Agentic Engineering Platform içerisinde çalışan bileşenlerden biridir" (`versacoder-proje-planlamasi-ai-01.md`)
- Hedeflenen çalışma modeli: "User Intent → Context Discovery → Requirement Understanding → Task Decomposition → Agent Selection → Tool/MCP Selection → Session Configuration → Workflow → User Approval → Agentic Execution → Validation → Review → Auditable Result" (`versacoder-proje-planlamasi-ai-01.md`)
- Reddedilen model: "Prompt → LLM → Answer" (`versacoder-proje-planlamasi-ai-01.md`)
- "Accuracy > Completeness > Speed" ilkesi: hızlı yanlış cevap güvenilir mühendislik değildir (`versacoder-proje-planlamasi-ai-01.md`)
- Örnek kalite sorusu: "Bu özellik Versa Coder'ın insan ve AI Agent'ların birlikte kontrollü, izlenebilir, doğrulanabilir ve sürdürülebilir yazılım mühendisliği yapmasına gerçekten katkı sağlıyor mu?" (`versacoder-proje-planlamasi-ai-01.md`)
- Karar kaynakları: kullanıcı kararı / Repository Evidence / Web Verified / Inference (`versacoder-proje-planlamasi-ai-01.md`)
- Durum statüleri: UNKNOWN, VERIFICATION REQUIRED, INFERENCE (`versacoder-proje-planlamasi-ai-01.md`)
- "human-controlled autonomy" hedefi; otonomi sınırsız yetki değildir (`versacoder-proje-planlamasi-ai-01.md`)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[versacoder-projesi]] — platform tanımı ("Agentic Software Engineering & IDE Operating Platform") ve ne olmadığı
- [[agent-workflow]] — Agent/Runtime/Orchestration, Session, Workflow, Tool Gateway, Validation/Review bileşen ilişkileri
- [[ai-agent-hafizasi]] — `.ai` Second Brain'in SSOT rolü, Memory vs Knowledge ayrımı, Context Awareness
- [[saglayici-soyutlamasi]] — Provider Abstraction, model bağımsızlığı, LLM ≠ Agent Runtime ayrımı
- [[insan-onayi-ve-denetlenebilirlik]] — Human-in-the-Loop, Approval, Auditability/Traceability, Evidence-Based Engineering
