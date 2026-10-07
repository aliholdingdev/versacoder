---
title: ChatGPT
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/electronic-gpt-ozle-promt.md
  - raw/ai-agentic-promt-maker.md
  - raw/ai-vision-render-studio.md
tags: [chatgpt, llm, model, prompt-target]
---

# ChatGPT

**Rol:** Prompt Arşivi'nin hedef model ailesi. 8 prompt dosyası doğrudan ChatGPT system prompt formatında yazılmıştır (`raw/` altında `Chatgpt\` kaynağından kopyalandı).

**Prompt aralığı:** devre analizi / BOM ([[elektronik-prompt]]), sinematik wallpaper üretimi ([[image-generation-prompt]]), görsel tersine mühendislik, UI/UX analizi ([[ui-ux-analiz-prompt]]), Türkçe yazım düzeltme ([[yazim-duzeltme-prompt]]), enterprise prompt üretimi ([[agentic-prompt-maker]]), render stüdyosu ([[vision-render-studio]]).

**Bağımsız çalışma kuralı:** ChatGPT ile yapılan oturumlar OpenCode/Claude gibi diğer modellerin oturum durumundan **bağımsızdır**; ortak tek hafıza bu `.ai/` wikisidir (bkz. [[multi-llm-bagimsizlik]], CLAUDE.md §2).

**Sürüm notu:** Prompt dosyalarının tamamı `Version 1.0.0` künyelidir; model sürümü iddiası hiçbir kaynakta yoktur (kaynak yoksa iddia yok).

## İlgili Sayfalar
- [[bayram-ali-prompt-arsivi]] — bu model için yazılan promptlar
- [[multi-llm-bagimsizlik]] — model bağımsızlığı ilkesi
- [[opencode]] — bağımsız çalışan diğer model/agent ailesi
- [[prompt-engineering]] — promptların ortak kural seti
