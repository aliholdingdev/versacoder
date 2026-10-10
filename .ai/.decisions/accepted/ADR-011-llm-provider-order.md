---
title: "ADR-011 — LLM sağlayıcı sırası + streaming zorunlu"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "openrouter-free → ollama → openai-compatible; SSE zorunlu; anahtar secret store'dan"
tags: [adr, llm, providers, fallback, streaming]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Sıra değişirse ücretsiz sağlayıcı çağrılmaz ya da tek sağlayıcı kesilince ürün
susar; anahtar koda girerse depo sızar.

## §2 Karar

- LLM sağlayıcı sırası: **`openrouter-free → ollama → openai-compatible`**.
- **Streaming (SSE) zorunlu** — token akarken UI donmaz.
- Anahtarlar **secret store'dan** okunur (ADR-018); koda/repoya yazılmaz.
- Yeni sağlayıcı = yeni adaptör; mevcut zincir kodu **değişmez** (OCP — brain §2.2).

## §3 Alternatifler

- **Tek sağlayıcı:** kesintide ürün susar. ❌
- **Sıralı fallback + streaming (seçilen):** dayanıklılık. ✅

## §4 Sonuçlar

**Ters-senaryo:** Sıra değişseydi ücretsiz sağlayıcı çağrılmaz ya da tek sağlayıcı
kesilince ürün susardı; anahtar koda girerdi.

**Yakalama:** `secret-leak-check` + l4 llm-http sıra testi.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| Infrastructure `llm-http` adaptörleri | sıra = openrouter-free → ollama → openai-compatible |

## §6 Kaynak

- brain.md §1 satır 011 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-018-secret-dpapi-keys-map|ADR-018-secret-dpapi-keys-map.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
