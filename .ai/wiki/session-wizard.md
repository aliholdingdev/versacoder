---
title: Session Wizard
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-02.md
  - raw/versacoder-proje-planlamasi-ai-03.md
  - raw/versacoder-proje-planlamasi-ai-06.md
tags: [wizard, session, human-in-the-loop, orkestrasyon]
---

# Session Wizard

**Ne işe yarar:** "Gönder" tuşuna basıldığında statik chat yerine **Agent Swarm Control Station** açılır; `IntentAnalysisEngine` ön analiz yapar ve wizard **pre-populated** dolar (ai-02, ai-03).

**4 oturum tipi:** Single Agent · Multi-Agent Swarm · Chained Pipeline · Distributed Node (ai-03 §5.3, ai-06 §5.3).

**3 kademeli yapı:** Global Program Settings → Project Settings & Architecture → Chat/Session Agentic Wizard (ai-03).

**Otonomluk modları:** Full Auto · Human-in-the-Loop Approval Gates · Review Only (ai-03).

**"Bitti" basamağı (5 adım):**
1. Immutable `JSON State` (SessionConfiguration)
2. Prompt Compiler (Meta-System-Prompt)
3. DAG Workflow
4. Session Registry (SQLite + audit)
5. Runtime Launch (ai-03)

**Wizard içeriği:** Chat/Session ayarları, seçilecek ajanlar, ileri seviye ayarlar; proje türünü (.NET, Embedded, C++, Web, Driver) algılar. Meta-System-Prompt = sektör kuralları + dil standartları + güvenlik + bellek derinliği (ai-06 §5.3).

**Kural:** Kullanıcı "Onayla/Başlat" demeden immutable konfigürasyon üretilmez; wizard parametreleri sihirbazdan gelir, modelin tahminine bırakılmaz (ai-02, ai-03).

## İlgili Sayfalar
- [[agent-workflow]] — wizard'ın başlattığı yürütme akışı
- [[prompt-engineering]] — Meta-System-Prompt / Prompt Synthesis Engine
- [[versacoder-projesi]] — platformun oturum katmanı
- [[insan-onayi-ve-denetlenebilirlik]] — Approval Gate yaklaşımı
