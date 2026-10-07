---
title: SSOT (Tek Doğru Kaynak)
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/coremusic-vault-template.md
  - raw/versacoder-proje-planlamasi-ai-08.md
tags: [ssot, guvenilirlik, oncelik, kural]
---

# SSOT — Tek Doğru Kaynak

**Guardrail #5 (coremusic-vault-template):** Bilgi yalnızca `.ai/` içindedir; kök `CLAUDE.md` bir pointer'dır ve "Do not use this file for instructions" der.

**Önem sırası (çelişkide kazanan):**
`.ai/CLAUDE.md > .ai/AGENTS.md > .ai/WORKFLOW.md > .ai/brain.md > .ai/index.md > .ai/.templates/`

Prompt arşivleri ile çelişen vault içeriği **vault lehinedir**; harici kaynaklar (web, model hafızası) her zaman vault'un altındadır.

**Değişmez 14 kuraldan öne çıkanlar:** UTF-8 yazım protokolü, `log.md` append-only, frontmatter 7 alan, wiki-link `[[relative/path]]`, frozen ADR immutable, gizli bilgi `REDACTED` (coremusic-vault-template).

**Versa Coder'daki karşılığı:** `.ai` "Project Knowledge / Documentation / SSOT kaynağıdır"; Domain Entity değildir, Domain/UI içine konmaz → `Application → IKnowledgeService → Knowledge Infrastructure → .ai` zinciri (ai-08 §29).

**Öncelik zinciri (ai-08, ai-01):** `USER INTENT > EXPLICIT REQUIREMENTS > EXISTING CONTEXT > CORRECTNESS > SECURITY > MAINTAINABILITY > PERFORMANCE > SIMPLICITY`.

## İlgili Sayfalar
- [[vault-mimarisi]] — SSOT'un fiziksel düzeni
- [[coremusic-vault]] — bu ilkenin doğrulandığı şablon
- [[ai-agent-hafizasi]] — bilginin hafıza tarafı
- [[llm-wiki]] — bu depodaki uygulaması
