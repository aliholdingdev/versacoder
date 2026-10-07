---
title: Görsel Üretim Promptu
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/girl-image-8k-generator-wallpeprs-promt.md
  - raw/girl-image-analzye-promt.md
  - raw/ai-vision-render-studio.md
tags: [image-generation, prompt-zinciri, model-uyumu]
---

# Görsel Üretim Promptu

**Standart prompt sırası (ai-vision-render-studio):**
`Subject → Environment → Composition → Camera → Lens → Lighting → Materials → Texture → Color → Atmosphere → Depth → Rendering → Quality`

**Reconstruction zinciri (girl-image-analzye):**
`SUBJECT → IDENTITY → ANATOMY → POSE → CLOTHING → MATERIALS → ENVIRONMENT → COMPOSITION → CAMERA → LENS → LIGHTING → COLOR → … → FIDELITY`

**Çıktı blokları:** `FINAL PROMPT` · `NEGATIVE PROMPT` · `CAMERA` · `LIGHTING` · `OUTPUT`.

**Varsayılan negative:** bad anatomy, plastic skin, watermark, "cartoon rendering unless requested".

**Upscale pipeline:** `AI upscale → detail reconstruction → artifact cleanup → sharpening → final color correction`.

**Model uyumu:** Midjourney · FLUX · SDXL · Stable Diffusion · DALL·E · GPT Image · ComfyUI · Automatic1111. Model belirtilmezse syntax dayatılmaz; **bilinmeyen modelin syntax'ı uydurulmaz**.

**Model varyantları (analiz tarafı):** Nano Banana Pro 2, FLUX, SDXL, Pony/Illustrious (tag-based), Midjourney (kompozit), GPT Image, ComfyUI.

**Multi-image rolleri:** Image 1 Subject · Image 2 Style · Image 3 Environment · Image 4 Material · Image 5 Composition.

**Kapsam ayrımı:** Bu sayfa **modelden bağımsız ortak prompt zinciri ve negative/upscale kurallarını** kapsar; 4K/8K wallpaper'a, lens ve DOF eşleşmesine özgü kurallar [[girl-gorsel-prompt]] sayfasındadır.

**Uyarı (ai-vision-render-studio):** "Sadece '8K' veya '16K' yazmak kalite garantisi değildir."

## İlgili Sayfalar
- [[girl-gorsel-prompt]] — wallpaper ve analiz promptları
- [[vision-render-studio]] — 9 modluk stüdyo promptu
- [[prompt-engineering]] — ortak kural seti
- [[chatgpt]] — hedef model
