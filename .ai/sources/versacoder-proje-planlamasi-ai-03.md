---
title: "Versa Coder — Agent Chat Wizard System (ai-03)"
raw_path: "raw/versacoder-proje-planlamasi-ai-03.md"
ingested: 2026-10-06 19:01
tags: [versacoder, wizard, agent-chat, deterministik-kurallar, anti-halüsinasyon, devexpress]
---

# versacoder-proje-planlamasi-ai-03.md

## Genel Özet

Dosya, Versa Coder'ın **Agent Chat Wizard System**ini tanımlayan mimari şartnamedir (VERSION 1.0.0, "Designed & Coding by Bayram Ali Akşit") (`versacoder-proje-planlamasi-ai-03.md`). Niyet tetiklemesi, 6 adımlı Session Agentic Agent Wizard, "Bitti" butonundaki Prompt Synthesis Engine, deterministik yürütme/gözlemlenebilirlik, 3 kademeli (Global/Project/Chat-Session) wizard hiyerarşisi, çift yönlü görsel akış motoru ve anti-halüsinasyon kural motorunu anlatır (`versacoder-proje-planlamasi-ai-03.md`). Ayrıca dosyanın sonunda 12 maddelik **Agent Davranış Kuralları** listesi yer alır (`versacoder-proje-planlamasi-ai-03.md`). "Agent Inc." başlıklı bölüm, 3 seviyeli yapılandırma sihirbazı ve deterministik guardrails ile halüsinasyon riskini sıfırlamayı hedefler (`versacoder-proje-planlamasi-ai-03.md`).

## Ana Fikirler

- `IntentAnalysisEngine` talebi önceden ayrıştırır (görev kapsamı, roller, MCP araçları); wizard bu ön analizle **pre-populated** açılır (`versacoder-proje-planlamasi-ai-03.md`).
- Wizard 6 adım: (1) Chat Session yapılandırması (4 oturum tipi: Single Agent, Multi-Agent Swarm, Chained Pipeline, Distributed Node), (2) Agent Session (rol + model eşleştirme + hiperparametre), (3) Tool/MCP izin matrisi, (4) sektörel standart enjeksiyonu (`versacoder-proje-planlamasi-ai-03.md`).
- Otonomluk seviyeleri: Tam Otonom (Full Auto), Yarı Otonom (Human-in-the-Loop Approval Gates), Yalnızca İnceleme (Review Only) (`versacoder-proje-planlamasi-ai-03.md`).
- "Bitti" butonu 5 deterministik adım çalıştırır: Immutable JSON State → Prompt Compiler → DAG Workflow → Session Registry (SQLite + audit) → Runtime Launch (`versacoder-proje-planlamasi-ai-03.md`).
- 3 kademeli wizard: 1) Global Program Settings (model sağlayıcıları, global MCP, güvenlik), 2) Project Settings & Architecture (katman hiyerarşisi, sektörel standartlar, `.ai` vault), 3) Chat/Session Agentic Wizard (görev, ajan takımı, izinler, immutable session state) (`versacoder-proje-planlamasi-ai-03.md`).
- Görsel akış motoru: DevExpress Diagram/Flow Control ile durum makinesi modelleme; diyagramlar `.ai/workflows/` dizininde JSON/DSL/C# State Machine olarak saklanır (`versacoder-proje-planlamasi-ai-03.md`).
- Anti-halüsinasyon motoru 4 durum kuralı: veritabanı değişikliği, build hatası, güvenlik riski (Approval Gate), UI bileşeni (DevExpress MVVM) (`versacoder-proje-planlamasi-ai-03.md`).

## Önemli Alıntılar/Veriler

- Örnek meta-system-prompt: *"Sen bu oturumda **Architect Agent** rolündesin. Yalnızca `VersaCoder.Domain` ve `VersaCoder.Abstractions` katmanlarının kontratlarını tasarlayacaksın."* (`versacoder-proje-planlamasi-ai-03.md`)
- *"Sana tanımlanan model kısıtı: EF Core 8 Fluent API kullanılacak, ham SQL kesinlikle yazılmayacak."* (`versacoder-proje-planlamasi-ai-03.md`)
- Güvenlik kuralı: *"OWASP 2025 standartlarında input sanitization uygulamayan kodları **Reviewer Agent** otomatik reddedecektir."* (`versacoder-proje-planlamasi-ai-03.md`)
- Kural formatı: **"IF-THIS-THEN-THAT" (Eğer bu olursa bunu uygula)** (`versacoder-proje-planlamasi-ai-03.md`)
- Yüzde kuralı: *"%90 üzeri code coverage sağlanmadan oturumu tamamlama"* (`versacoder-proje-planlamasi-ai-03.md`)
- Örnek model eşleştirmesi: "Mimar için *Claude 3.5 Sonnet / Reasoning Modeli*, Hızlı kodlama için *GPT-4o / DeepSeek-Coder*, Güvenlik için *Yerel LLM*" (`versacoder-proje-planlamasi-ai-03.md`)
- Agent Davranış Kuralları (12 madde): "Tahmin yapmaz. / Bilgi uydurmaz. / Gereksiz kod yazmaz. / Kullanıcıdan gizli mimari karar vermez. / Referans projeyi analiz etmeden implementasyon yapmaz. / '.ai' bilgisini yok saymaz. / Katmanları ihlal etmez. / UI'ya business logic gömmez. / Domain'i DevExpress'e bağımlı yapmaz. ... / Gereksiz abstraction üretmez." (`versacoder-proje-planlamasi-ai-03.md`)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[session-wizard]] — 6 adımlı wizard, 3 kademeli hiyerarşi, "Bitti" tetikleme zinciri
- [[agent-workflow]] — DAG workflow inşası, deterministik yürütme, görsel DAG/telemetri/self-healing logu
- [[prompt-engineering]] — Prompt Synthesis Engine ve meta-system-prompt örnekleri
- [[anti-halüsinasyon-kurallari]] — Conditional Guardrail Engine, 4 durum kuralı, agent davranış yasakları
