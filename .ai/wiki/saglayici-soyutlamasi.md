---
title: Sağlayıcı Soyutlaması
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-01.md
tags: [provider, abstraction, llm, mimari]
---

# Sağlayıcı Soyutlaması (Provider Abstraction)

**Konum:** LLM sistemin kendisi değil, Agentic Engine içindeki bir **bileşen / dependency katmanıdır** (ai-01).

**Temel ayrım:** **AI Model ≠ Agent Runtime** — model üretir, runtime bağlamda nasıl kullanacağını yönetir (ai-01).

**Amaç:** Farklı API / auth / capability farkları tüm sisteme yayılmaz; tek sağlayıcı zorunlu merkez değildir (ai-01).

**Model seçimi kriterleri:** görev, kalite, maliyet, latency, capability. Karar, Architecture aşamasında belirlenir — asla tahmin edilmez (ai-01).

**Mimari yerleştirme:** OpenCode gibi harici araçlar yalnızca `Infrastructure` adapter'ıdır (`VersaCoder.ExternalAgents.OpenCode`); CLI syntax Application'a sızmaz, OpenCode değişse Core/Application etkilenmez (ai-08 §20-21, §56).

## İlgili Sayfalar
- [[multi-llm-bagimsizlik]] — soyutlamanın çoklu-model sonucu
- [[ucretsiz-model-havuzu]] — somut model envanteri
- [[opencode]] — adapter ile bağlanan harici araç
- [[versacoder-projesi]] — katman mimarisi içindeki yeri
- [[fallback]] — adapter'ların mümkün kıldığı otomatik geçiş
