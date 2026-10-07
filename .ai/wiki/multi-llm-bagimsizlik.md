---
title: Çoklu LLM Bağımsızlığı
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-04.md
  - raw/versacoder-proje-planlamasi-ai-05.md
tags: [multi-llm, fallback, bagimsizlik, model-routing]
---

# Çoklu LLM Bağımsızlığı

**İlke:** Her LLM modeli/agenta **bağımsız çalışır**; bir modelin oturum durumu diğerine devredilmez, ortak tek gerçek kaynak `.ai` vault'tur (ai-04, ai-05 §1).

**Smart Model Router:**
- `free` tercih → Zen free havuzu
- `paid` tercih → Claude Sonnet / GPT-4o (ai-04)

**Fallback zinciri (ai-04):** DeepSeek V4 Free → Nemotron 3.5 Free → OpenRouter Free → Ollama Local. `429 / timeout` durumunda otomatik geçiş.

**Dış ajan zinciri (ai-05 §26):** Internal Free Model → OpenCode → Codex → Claude Code → Abacus AI CLI.

**Kullanıcı notu (ai-04):** OpenCode free çalışmazsa arka planda OpenCode/Codex/Claude Code/Abacus AI CLI çalıştırılıp sonuç Versa Coder'a aktarılacak — "2 tane LLM çalıştıracağız".

**Yasaklar:** Kullanıcı belirtmeden provider önceliği kurulmaz; **sessizce provider değişimi yasaktır** (ai-05 §26, §37). Harici ajan Versa Coder global policy'sini bypass edemez (ai-05 §30).

**Sıfır maliyet hedefi:** API anahtarı/bakiye girilse bile kutudan free modellerle çalışır (ai-04).

## İlgili Sayfalar
- [[saglayici-soyutlamasi]] — mimari temeli
- [[opencode]] — fallback zincirinin ilk harici adayı
- [[ucretsiz-model-havuzu]] — free model envanteri
- [[versacoder-projesi]] — uygulandığı katman
- [[chatgpt]] — bağımsız çalışan diğer model ailesi
- [[llm-wiki]] — bağımsız modeller için ortak hafıza
- [[fallback]] — model ve ajan geçiş zincirinin tanımı
