---
title: Ollama
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-04.md
tags: [ollama, local-llm, fallback, ucretsiz]
---

# Ollama

**Ne:** Yerel (localhost) LLM çalışma zamanı. Versa Coder'ın **ücretsiz model havuzunun ve fallback zincirinin son halkasıdır** (versacoder-proje-planlamasi-ai-04).

**Adres:** `http://localhost:11434`.

**Fallback sırası:** DeepSeek V4 Free → Nemotron 3.5 Free → OpenRouter Free → **Ollama Local**. `429` / timeout durumunda otomatik geçiş; ağ tamamen kapalıyı da bu halka karşılar (ai-04).

**Konum:** Harici bir **Infrastructure adapter**'ıdır; model API çağrısı ile CLI process yürütme ayrı yollardır (ai-05 §7). Yerel model seçimi `ModelCostType = Free` kapsamında değerlendirilir.

**Kapsam dışı:** Kurulan model listesi, GPU gereksinimleri ve sürüm bilgisi hiçbir kaynakta yok — kaynak yoksa iddia yok.

## İlgili Sayfalar
- [[ucretsiz-model-havuzu]] — fallback zincirinin tamamı
- [[multi-llm-bagimsizlik]] — bağımsız model ilkesi
- [[opencode]] — zincirin ilk harici adayı
- [[saglayici-soyutlamasi]] — adapter katmanı
