---
title: "VERSA CODER — Wizard İç Mekanizması, MCP/Plugin Sandbox ve Çift Katmanlı Bellek"
raw_path: "raw/versacoder-proje-planlamasi-ai-06.md"
ingested: 2026-10-06 18:59
tags: [versacoder, session-wizard, mcp, plugin-sandbox, hafiza, devexpress, ssot]
---

# Versa Coder Proje Planlaması ai.06 — Wizard, MCP/Plugin ve Bellek Mimarisi

## Genel Özet
Bu dosya (`versacoder-proje-planlamasi-ai-06.md`) Versa Coder'ın özellik anlatımlarını içerir: 5.3'te Session Agentic Agent Wizard'ın iç çalışma mekanizması, 5.4'te Model Context Protocol (MCP) Gateway ve `AssemblyLoadContext` tabanlı izole plugin sandbox, 5.5'te çift katmanlı hibrit bellek ve sürekli öğrenme döngüsü, 5.6'da DevExpress tabanlı UI/UX ve gerçek zamanlı görselleştirme. Wizard, kullanıcının kontrolünü maksimize eden Human-in-the-Loop yönetim paneli olarak tanımlanır. Bellek mimarisi unutkan yapay zekâ (Amnesic AI) problemini kısa vadeli çalışma belleği + uzun vadeli `.ai` Knowledge Vault ile çözer.

## Ana Fikirler
- Wizard akıllı profilleme yapar: proje türünü (.NET, Embedded, C++, Web, Driver) algılar ve varsayılan ajan ekibini otomatik seçer (ai-06.md 5.3).
- Dört oturum tipi: Tekli Session (Single Agent), Çoklu Sürü Session (Swarm Intelligence), Zincirleme Session (Chained Pipeline), Dağıtılmış Session (Distributed Agentic Node) (ai-06.md 5.3).
- Dinamik System Prompt Sentezleyici: sektör kuralları, dil standartları, güvenlik seviyesi, bellek derinliği birleştirilerek her ajan için çelişkisiz bir Meta-System-Prompt üretilir (ai-06.md 5.3).
- MCP Gateway `stdio` ve `SSE (Server-Sent Events)` taşıma protokolleriyle çalışır; DB sorgulayıcıları, API test araçları, dokümantasyon arama ve harici LLM context sağlayıcıları dinamik bağlanabilir (ai-06.md 5.4).
- Üçüncü parti C# DLL plugin'leri `AssemblyLoadContext` içinde izole yüklenir; eklenti çökerse ana IDE etkilenmez, güvenle Unload edilir (ai-06.md 5.4).
- Çift katmanlı bellek: Working Memory (sliding window, token bütçesi) + Long-Term Knowledge Vault (`.ai` SSOT: `rules/`, `skills/`, `decisions/`, `memory/`; vektörel arama SQLite'ta indekslenir) (ai-06.md 5.5).
- Continuous Learning Loop: geliştirici düzeltme yaptığında Diff analiz edilir, öğrenilen kural `.ai/learning/` dizinine kaydedilir (ai-06.md 5.5).
- DevExpress DockManager/DocumentManager çoklu panel docking; Visual Execution Graph, canlı telemetri paneli ve Approval Gate UI (net diff blokları) (ai-06.md 5.6).

## Önemli Alıntılar/Veriler
- "Akıllı Profilleme: Wizard açıldığında projenin türünü (.NET, Embedded, C++, Web, Driver) algılar ve varsayılan ajan ekibini otomatik seçer." (ai-06.md 5.3).
- Dört oturum tipi adları: "Tekli Session (Single Agent) · Çoklu Sürü Session (Swarm Intelligence) · Zincirleme Session (Chained Pipeline) · Dağıtılmış Session (Distributed Agentic Node)" (ai-06.md 5.3).
- "Üçüncü parti C# DLL plugin'leri ana uygulama belleğini kirletmeden ayrı bir `AssemblyLoadContext` içinde yüklenir" (ai-06.md 5.4).
- "`.ai` Bilgi Kasası: Markdown / Obsidian tabanlı SSOT (Single Source of Truth) dosyaları (`rules/`, `skills/`, `decisions/`, `memory/`)." (ai-06.md 5.5).
- "sistem aradaki farkı (Diff) analiz eder, öğrenilen kuralı `.ai/learning/` dizinine kaydeder" (ai-06.md 5.5).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[session-wizard]] — wizard iç mekanizması, 4 oturum tipi ve Meta-System-Prompt sentezleyici
- [[vault-mimarisi]] — `.ai` Knowledge Vault: Markdown/Obsidian SSOT + SQLite vektörel indeks
- [[ai-agent-hafizasi]] — Working Memory (sliding window) + Continuous Learning Loop
- [[opencode]] — MCP Gateway: stdio/SSE taşıma protokolleri
- [[versacoder-projesi]] — AssemblyLoadContext plugin sandbox ve DevExpress UI/UX katmanı
