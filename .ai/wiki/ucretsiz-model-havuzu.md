---
title: Ücretsiz Model Havuzu
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-04.md
tags: [ucretsiz-model, zen, openrouter, ollama, context]
---

# Ücretsiz Model Havuzu

**Zen free modelleri ve context (ai-04):**

| Model | Context |
|---|---|
| DeepSeek V4 Flash Free | 64K |
| MiMo V2.5 Free | 32K |
| Hy3 Free | 32K |
| Laguna S 2.1 Free | 32K |
| Nemotron 3 Ultra Free | 64K |
| Nemotron 3.5 Lightning Free | 64K |
| Big Pickle | 128K |

**Ek havuzlar:** Gemini 2.5 flash / flash-thinking / pro · OpenRouter free (llama-3.3-70b, deepseek-r1, qwen-2.5-coder-32b) · Ollama (`localhost:11434`) · Groq (ai-04).

**Fallback zinciri:** DeepSeek V4 Free → Nemotron 3.5 Free → OpenRouter Free → Ollama Local; `429`/timeout'ta otomatik geçiş (ai-04).

**Uygulama:** `ModelCostType` (`Free / Freemium / Paid`) + `ModelDescriptor` record; `OpenCodeFreeHttpClient` (stream + non-stream, temperature 0.2), `ModelsDevCatalogSyncService` (ai-04).

**Sınırlama:** Model katalogları değişkendir; context ve model adları canlı listeden (`models.opencode.ai/api.json`) doğrulanır, ezberlenmez.

## İlgili Sayfalar
- [[opencode]] — havuzu sunan araç
- [[multi-llm-bagimsizlik]] — yönlendirme ve fallback
- [[saglayici-soyutlamasi]] — envanterin soyutlandığı katman
- [[versacoder-projesi]] — uygulandığı proje
- [[ollama]] — fallback zincirinin yerel halkası
- [[fallback]] — zincirin kendisi (429/timeout tetikli otomatik geçiş)
