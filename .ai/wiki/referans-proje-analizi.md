---
title: Referans Proje Analizi
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
tags: [analiz, referans, arastirma, dogrulama]
---

# Referans Proje Analizi

**Amaç:** Referans projelerden yalnız **davranış, UX, workflow, session, MCP ve memory yaklaşımı** analiz edilir; kod, mimari ve isim kopyalamak **yasaktır** (ai-00).

**Referanslar:** `C:\www\versacoder\versacoder\versacoder.ai\.ai`, `github.com/anomalyco/opencode.git` (v2), `C:\opencode`, `C:\www\versacoder\opencode` (ai-00).

**9 analiz alanı:** Repository · Architecture · Agent System · Session · Workflow · MCP · Plugin · Memory · UI/UX (ai-00).

**Web araştırma önceliği:** Official Docs → Official GitHub → RFC/Spec → Primary kaynak → Secondary kaynak (ai-00).

**Doğrulama eşiği:** Kritik bilgi **≥2 bağımsız kaynak** ile doğrulanır; çelişkide `CONFLICT DETECTED` yazılır ve kesin karar verilmez (ai-00).

**Gereksinim formatı:** `REQ-ID / TITLE / DESCRIPTION / SOURCE / PRIORITY / DEPENDENCIES / STATUS`, öncelik P0–P3 (ai-00).

**Çıktılar:** "AI VALIDATION REPORT" ve "CODEBASE INDEX" (ai-00).

## İlgili Sayfalar
- [[versacoder-projesi]] — analizin hedefi
- [[ai-agent-hafizasi]] — `.ai` vault'un analiz edilen kısmı
- [[opencode]] — referans alınan araç
- [[anti-halüsinasyon-kurallari]] — ≥2 kaynak doğrulama kuralı
