---
title: Prompt Mühendisliği
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/versacoder-proje-planlamasi-ai-03.md
  - raw/ai-agentic-promt-maker.md
  - raw/yazimi-duzelt-promt-maker.md
tags: [prompt, zero-hallucination, soru-muhendisligi]
---

# Prompt Mühendisliği

**Soru öncelikleri:** `P0 BLOCKING` / `P1 IMPORTANT` / `P2 OPTIONAL`. Sorular "uzun, detaylı, teknik ve tek anlamlı" olmalıdır (ai-00).

**Cevap zinciri:** Answer → Requirement Update → Conflict Check → Dependency Check → Requirement Re-validation (ai-00).

**NEVER ASSUME listesi:** database, ORM, auth, AI provider, model, hosting, deployment, vector db asla varsayılmaz (ai-00).

**Dürüstlük etiketleri:** `DOĞRULANAMADI` · `BİLİNMİYOR` · `ÇIKARIM` · `WEB DOĞRULANDI` · `REFERENCE VERIFIED`; çelişkide `CONFLICT DETECTED, VERIFICATION REQUIRED` (ai-00). Görsel/prompt promptlarında eşdeğerler: `HIGH/MEDIUM/LOW CONFIDENCE`, `[UNKNOWN]`, `[TO_BE_VERIFIED]`, `[VERIFY REQUIRED]` (girl-image-analzye, ui-uix-anayze).

**Prompt Synthesis Engine:** Belirsiz metin yerine wizard parametrelerinden katı, çelişkisiz direktif üretilir. Örnek: *"Sen bu oturumda **Architect Agent** rolündesin. Yalnızca `VersaCoder.Domain` ve `VersaCoder.Abstractions` katmanlarının kontratlarını tasarlayacaksın."* (ai-03)

**23 bölümlük şablon standardı:** `ROLE → CONTEXT → OBJECTIVE → REQUIREMENTS → CONSTRAINTS → WORKFLOW → OUTPUT FORMAT → VALIDATION → FINAL CHECKLIST` + placeholder sistemi `[PROJECT_NAME]`, `[FRAMEWORK]`, `[BELİRTİLMELİ]` (yazimi-duzelt-promt-maker).

**Over-engineering yasağı:** Microservices/Kubernetes/CQRS, kullanıcı belirtmedikçe eklenmez (yazimi-duzelt, ai-agentic-promt-maker).

## İlgili Sayfalar
- [[anti-halüsinasyon-kurallari]] — prompt'un kural motoru karşılığı
- [[agentic-prompt-maker]] — adaptif keşif tabanlı prompt üretici
- [[yazim-duzeltme-prompt]] — prompt mimarisi şablonunun uygulaması
- [[versacoder-projesi]] — platformda kullanım yeri
- [[session-wizard]] — role özel prompt sentezi
- [[bayram-ali-prompt-arsivi]] — prompt örneklerinin kaynağı
