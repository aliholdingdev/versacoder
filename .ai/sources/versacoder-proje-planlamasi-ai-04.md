---
title: "Versa Coder — OpenCode Free Modelleri Entegrasyon Şartnamesi (ai-04)"
raw_path: "raw/versacoder-proje-planlamasi-ai-04.md"
ingested: 2026-10-06 19:01
tags: [opencode, ucretsiz-model, provider, csharp, fallback, smart-router]
---

# versacoder-proje-planlamasi-ai-04.md

## Genel Özet

Dosya, OpenCode kaynak kodu ve Zen API altyapısından çıkarılmış **ücretsiz (free) AI modellerini** ve bu modellerin `VersaCoder.Infrastructure.AI` katmanına entegrasyon kurallarını tanımlar (`versacoder-proje-planlamasi-ai-04.md`). 7 OpenCode free modelinin endpoint/protokol/kontekst tablosu, ek ücretsiz sağlayıcı havuzu (Google/Gemini, OpenRouter, Ollama yerel, Groq), C# `ModelDescriptor` ve katalog tanımları, Smart Model Router fallback zinciri ve aboneliksiz HTTP istemcisi/katalog senkronu/DI kaydı kodları içerir (`versacoder-proje-planlamasi-ai-04.md`). Belgenin başında kullanıcının "OpenCode free model sistemi çalışmıyorsa arka planda OpenCode/Codex/Claude Code/Abacus AI CLI çalıştırıp sonucu Versa Coder'a aktaracağız, 2 tane LLM çalıştıracağız" notu yer alır (`versacoder-proje-planlamasi-ai-04.md`). Amaç sıfır maliyet, aboneliksiz çalışabilen bir model altyapısıdır (`versacoder-proje-planlamasi-ai-04.md`).

## Ana Fikirler

- OpenCode Zen free modelleri (7 adet): deepseek-v4-flash-free, mimo-v2.5-free, hy3-free, laguna-s-2.1-free, nemotron-3-ultra-free, nemotron-3.5-lightning-free, big-pickle (`versacoder-proje-planlamasi-ai-04.md`).
- Tüm Zen modelleri `https://opencode.ai/zen/v1/chat/completions` endpoint'inde ve `@ai-sdk/openai-compatible` protokolünde; canlı metadata `https://opencode.ai/zen/v1/models` üzerinden çekilebilir (`versacoder-proje-planlamasi-ai-04.md`).
- Ek ücretsiz havuz: Gemini 2.5 flash/flash-thinking/pro (Google AI Studio), OpenRouter free (llama-3.3-70b, deepseek-r1, qwen-2.5-coder-32b), Ollama yerel (deepseek-coder-v2:16b, qwen2.5-coder:7b, starcoder2:15b), Groq free (llama-3.3-70b, llama-3.1-8b, mixtral-8x7b) (`versacoder-proje-planlamasi-ai-04.md`).
- `VersaCoder.Domain.AI.Models.ModelCostType` enum'u: `Free`, `Freemium`, `Paid`; `ModelDescriptor` record ProviderId/ModelId/EndpointUrl/CostType/ContextWindowTokens/SupportsStreaming/SupportsToolCalling alanlarını taşır (`versacoder-proje-planlamasi-ai-04.md`).
- Smart Model Router: free tercih edilirse OpenCode Zen → 429/hata durumunda OpenRouter free veya Ollama yerel modele kademeli fallback (`versacoder-proje-planlamasi-ai-04.md`).
- HTTP istemcisi OpenCode başlıklarını birebir kullanır: `HTTP-Referer`, `X-Title`, `X-Source`, `X-BILLING-INVOKE-ORIGIN`, User-Agent `opencode/1.0.0/client` (`versacoder-proje-planlamasi-ai-04.md`).
- `ModelsDevCatalogSyncService`, `https://models.opencode.ai/api.json` üzerinden input/output cost = 0 olan modelleri otomatik bulur; hata olursa statik katalog kullanılır (`versacoder-proje-planlamasi-ai-04.md`).

## Önemli Alıntılar/Veriler

- Endpoint: `https://opencode.ai/zen/v1/chat/completions`; modeller: DeepSeek V4 Flash Free (`64_000` ctx), MiMo V2.5 Free (`32_000`), Hy3 Free (`32_000`), Laguna S 2.1 Free (`32_000`), Nemotron 3 Ultra Free (`64_000`), Nemotron 3.5 Lightning Free (`64_000`), Big Pickle (`128_000`) (`versacoder-proje-planlamasi-ai-04.md`).
- Ollama: `http://localhost:11434/v1/chat/completions` — "Sıfır maliyet, %100 yerel güvenlik ve gizlilik" (`versacoder-proje-planlamasi-ai-04.md`).
- Groq: `https://api.groq.com/openai/v1/chat/completions`; OpenRouter: `https://openrouter.ai/api/v1/chat/completions` (`versacoder-proje-planlamasi-ai-04.md`).
- Fallback zinciri: "DeepSeek V4 Free -> Nemotron 3.5 Free -> OpenRouter Free -> Ollama Local" (`versacoder-proje-planlamasi-ai-04.md`).
- Kapanış iddiası: "Versa Coder hiçbir ücretli aboneliğe bağlı kalmadan OpenCode'un tüm ücretsiz modellerini ... sıfır maliyetle çalıştırabilecektir." (`versacoder-proje-planlamasi-ai-04.md`)
- `temperature = 0.2`; DI: Autofac `AIInfrastructureModule` → `OpenCodeFreeHttpClient`, `ModelsDevCatalogSyncService`, `SmartModelRouter` (`versacoder-proje-planlamasi-ai-04.md`)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[opencode]] — Zen API free model altyapısı, endpoint ve başlık (header) imzaları
- [[ucretsiz-model-havuzu]] — 7 Zen modeli + Gemini/OpenRouter/Ollama/Groq fallback havuzu
- [[multi-llm-bagimsizlik]] — tek sağlayıcıya bağımlı olmama, Smart Router ve katalog senkronizasyonu
- [[versacoder-projesi]] — `VersaCoder.Infrastructure.AI` katmanı, ModelDescriptor ve DI kayıtları
