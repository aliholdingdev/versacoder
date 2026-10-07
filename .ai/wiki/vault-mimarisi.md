---
title: Vault Mimarisi
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/coremusic-vault-template.md
  - raw/versacoder-proje-ai-00.md
  - raw/versacoder-proje-planlamasi-ai-06.md
  - raw/versacoder-proje-planlamasi-ai-08.md
tags: [vault, .ai, mimari, obsidian, markdown]
---

# Vault Mimarisi (`.ai`)

**Fiziksel düzen (coremusic-vault-template):** kök `CLAUDE.md` pointer + `.ai/` içinde 15 kök `.md`, `.rules/`, `.templates/` (36 dosya, 10 kategori), `.decisions/`, `.agents/`, `architecture/`, `scripts/`.

**Kurulum sırası (versacoder-proje-ai-00, part 94):** `templates → agents → kök MD → mimari + onay kuralı`.

**30 dakikalık yeni proje kurulumu (coremusic-vault-template):** 0-5 dizin → 5-10 pointer + 14 boot dosyası → 10-15 rules + workflow → 15-20 agent + config → 20-25 şablon + script → 25-30 verify.

**`.workflows/` 9 dosya:** CLAUDE + session-init (12 aşama), vault-sync (8), adr-creation (6), security-audit, deployment (8 + rollback), hallucination-control, orchestrator-flow (11), architecture-write (9, batch ≤31 dosya / 0 silme).

**Versa Coder entegrasyonu:** `.ai` Domain veya UI içine konmaz; `Application → IKnowledgeService → Knowledge Infrastructure → .ai` (ai-08 §29). Bellek ayrımı: Working / Long-Term (`.ai`) / Session Memory.

**Arama:** Markdown/Obsidian SSOT (`rules/`, `skills/`, `decisions/`, `memory/`) + SQLite tabanlı embedding indeksi ile vektörel semantik arama (ai-06 §5.5).

**Eksikler (coremusic-vault-template):** `session-save.mjs`, `vault-post-update.mjs`, `vault-cmd.mjs`, `project-state.md`.

## İlgili Sayfalar
- [[coremusic-vault]] — mimarinin doğrulandığı şablon
- [[versacoder-vault]] — ham vault paketinin çekirdek içeriği (raw/versacoder-vault/)
- [[ssot]] — kural önceliği
- [[ai-agent-hafizasi]] — hafıza katmanı olarak kullanımı
- [[versacoder-projesi]] — uygulandığı proje
- [[llm-wiki]] — bu depodaki basitleştirilmiş hali
