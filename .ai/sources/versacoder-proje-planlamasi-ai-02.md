---
title: "Versa Coder — Agents & Chat System / Proje Analizi (ai-02)"
raw_path: "raw/versacoder-proje-planlamasi-ai-02.md"
ingested: 2026-10-06 19:01
tags: [versacoder, chat-agent, multi-agent-swarm, wizard, problem-cozum, mimari]
---

# versacoder-proje-planlamasi-ai-02.md

## Genel Özet

Dosya, Versa Coder'ın **Agents & Chat System** yapısını analiz eden bir proje tanımlama promptudur (başlık: "PROJE NEDİR? ANALİZ VE TANIMLAMA", VERSION 1.0.0) (`versacoder-proje-planlamasi-ai-02.md`). İçerik; Chat/Agent/LLM ayrımı, Chat'in amacı, Chat-Agent ilişkisi, platform anatomisi (6 katman), çoklu-ajan rolleri, Versa Coder'ı rakiplerinden ayıran fark tablosu, 7 temel problem ve çözümü ile 6 aşamalı uçtan uca yürütme yaşam döngüsünü kapsar (`versacoder-proje-planlamasi-ai-02.md`). Ayrıca "Gönder" butonuna basılınca açılan **Session Agentic Agent Wizard** istemi ve wizard'ın görevi tanımlanır (`versacoder-proje-planlamasi-ai-02.md`). Belge, referanslarda olmayan bilgiyi gerçekmiş gibi eklememeyi ve "BELİRTİLMEDİ"/"DOĞRULAMA GEREKLI" işaretlemesini ister (`versacoder-proje-planlamasi-ai-02.md`).

## Ana Fikirler

- Temel ayrım: **Chat ≠ Agent**, **Agent ≠ LLM**, **LLM ≠ Versa Coder**; Chat iletişim katmanı, Agent mühendislik çalışma birimi, LLM karar/üretim bileşenidir (`versacoder-proje-planlamasi-ai-02.md`).
- Chat'in amacı yalnızca "User → Prompt → LLM → Answer" olmamalı; "User Intent → Understanding → Agent Selection → Session → Workflow → Execution → Result" zincirinde rol almalıdır (`versacoder-proje-planlamasi-ai-02.md`).
- Platform anatomisi 6 katman: Presentation (DevExpress 2026 WinForms), Application (Intent Engine, Session Wizard, CQRS), Agent Runtime (Swarm Coordinator, Policy Gate), Protocol/MCP (MCP Client/Server, Tool Gateway, Plugin Sandbox, JSON-RPC), Infrastructure (28+ Modüler DLL), Domain/Core (Immutable Session State) (`versacoder-proje-planlamasi-ai-02.md`).
- Multi-Agent Swarm rolleri: Orchestrator, Architect, Developer/Coding, Reviewer/Quality, Security & Policy, Testing, Sektörel Uzman Ajanlar (Otomotiv MISRA/AUTOSAR, Sağlık HL7/FHIR/HIPAA, Finans ISO 20022, IoT/Embedded) (`versacoder-proje-planlamasi-ai-02.md`).
- 7 problem: mimari erozyon/God Class, sığ bağlam (context degradation), orkestrasyon eksikliği, kapalı ekosistem/araç silosu, güvenlik-denetim riski, düşük seviyeli mühendislik/sectörel uzmanlık boşluğu, belleksiz AI (`versacoder-proje-planlamasi-ai-02.md`).
- 6 aşamalı yaşam döngüsü: 1. Intent & AST Analysis → 2. Session Wizard (Immutable SessionConfiguration) → 3. Swarm Task Allocation (DAG) → 4. Execution & Tool Gateway/MCP → 5. Verification & Self-Healing → 6. Persistence & Audit Sync (`versacoder-proje-planlamasi-ai-02.md`).
- Çözüm teknolojileri: Roslyn AST analizörleri, Epoch Assembly, AssemblyLoadContext plugin sandbox, AES-256-GCM credential kasası, Human-in-the-Loop Approval Gates, Knowledge Vault (.ai/Obsidian/Markdown SSOT) (`versacoder-proje-planlamasi-ai-02.md`).

## Önemli Alıntılar/Veriler

- "Referanslarda olmayan bilgileri gerçekmiş gibi ekleme. Bilinmeyen noktaları **BELİRTİLMEDİ**, doğrulanması gereken noktaları **DOĞRULAMA GEREKLI** olarak işaretle." (`versacoder-proje-planlamasi-ai-02.md`)
- Fark tablosu kriterleri: Çalışma Modeli, Mimari Yaklaşım, Karar Mekanizması, Araç Entegrasyonu, Geniş Domain Desteği, Güvenlik ve Onay, Bağlam Yönetimi (`versacoder-proje-planlamasi-ai-02.md`)
- "50+ Katmanlı DLL / Clean Architecture" ve "Tam MCP (Model Context Protocol) + Dinamik C# Plugin Sandbox" (`versacoder-proje-planlamasi-ai-02.md`)
- Test ajanı: "xUnit, NUnit, BenchmarkDotNet ... %90+ coverage" (`versacoder-proje-planlamasi-ai-02.md`)
- Güvenlik: "AES-256-GCM şifreli credential kasası", "OWASP 2025", "değiştirilemez (immutable) audit log" (`versacoder-proje-planlamasi-ai-02.md`)
- Katman akışı: `Domain -> Abstractions -> Application -> Infrastructure -> UI` (`versacoder-proje-planlamasi-ai-02.md`)
- Wizard tetikleyicisi: "Gönder" butonu → "Kurumsal Düzeyde Çoklu-Ajan Yönetim İstasyonuna (Agent Swarm Control Station)" (`versacoder-proje-planlamasi-ai-02.md`)
- Persistence aşaması: Git staging, ADR kaydı, SQLite/SQL Server oturum geçmişi, `.ai` Knowledge Vault güncellemesi (`versacoder-proje-planlamasi-ai-02.md`)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[versacoder-projesi]] — 7 problem + çözüm matrisi ve rakiplerden fark tablosu
- [[agent-workflow]] — 6 aşamalı yaşam döngüsü, DAG tabanlı swarm dağıtımı, self-healing döngüsü
- [[session-wizard]] — Session Agentic Agent Wizard istemi, Immutable SessionConfiguration
- [[prompt-engineering]] — Chat/Agent/LLM ayrımı ve Chat'in prompt'tan bağımsız rolü
- [[ai-agent-hafizasi]] — Knowledge Vault (.ai/Obsidian/Markdown SSOT) ve amnesic AI çözümü
