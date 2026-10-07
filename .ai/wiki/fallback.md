---
title: Fallback Zinciri
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-04.md
  - raw/versacoder-proje-planlamasi-ai-05.md
tags: [fallback, model, saglayici, dayaniklilik]
---

# Fallback Zinciri

**Ne:** Bir model veya sağlayıcı başarısız olduğunda (HTTP 429, timeout, hata), sistemin kullanıcıyı beklemeden **sıradaki izinli seçeneğe** otomatik geçişi.

**Model fallback zinciri (ai-04):** `DeepSeek V4 Free → Nemotron 3.5 Free → OpenRouter Free → Ollama Local`. Kademeli fallback, yoğunluk kaynaklı `HTTP 429 Too Many Requests` veya zaman aşımında tetiklenir; kullanıcı yeni modeli elle seçmez.

**Harici ajan fallback zinciri (ai-05):** control plane ayrımı altında ajan sırası `OpenCode → Codex → Claude Code → Abacus`; zincir boyunca validation/audit kuralları geçerlidir, her geçiş kayda alınır.

**Kurallar:**
- Fallback yalnız **ücretsiz/izinli** havuzdan seçilir; izinli model yoksa sistem durur ve açık hata döner — havuza kaynaksız model eklenmez (bkz. [[anti-halüsinasyon-kurallari]]).
- Geçişte bağlam (conversation state) korunur; her geçiş `log.md`'ye düşer (bkz. [[ssot]]).
- Zincirin tamamı da başarısızsa uydurma çıktı üretilmez; kullanıcıya dürüst hata döner.

## İlgili Sayfalar
- [[ucretsiz-model-havuzu]] — zincirin model envanteri (7 Zen free + OpenRouter + Ollama)
- [[multi-llm-bagimsizlik]] — sağlayıcı bağımsızlığı, smart router ve fallback'in yeri
- [[saglayici-soyutlamasi]] — model ≠ runtime ayrımı; adapter'lar fallback'i mümkün kılar
- [[opencode]] — model zincirinin ilk halkası (Zen free endpoint)
- [[versacoder-projesi]] — fallback kararı platform mimarisinde nerede durur
