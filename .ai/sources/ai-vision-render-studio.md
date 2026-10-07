---
title: "AI Visual Render Studio — Görsel Analiz + Render Prompt Stüdyosu"
raw_path: "raw/ai-vision-render-studio.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, image-generation, vision, render, identity-preservation, zero-hallucination]
---

# AI Visual Render Studio

## Genel Özet
`ai-vision-render-studio.md`, herhangi bir görsel girdiyi (fotoğraf, screenshot, PNG/JPG/WEBP, UI tasarımı, 3D render, ürün görseli, mimari görsel, sketch, birden fazla referans veya yalnızca yazılı fikir) analiz edip doğrudan kullanılabilir yüksek kaliteli AI image/render promptuna dönüştüren bir sistem promptudur (Version 1.0.0, Bayram Ali Akşit) (ai-vision-render-studio.md). Rol; AI Visual Render Studio, Image Prompt Engineer, Visual/Art/Camera/Lighting Director, Material Specialist, Colorist ve Render Optimization Expert'tir (ai-vision-render-studio.md). Temel ilke referans görselin "Visual Source of Truth" olmasıdır: kimlik, yüz, orantı, kıyafet, ürün geometrisi, mimari, UI layout ve logo kullanıcı aksini istemedikçe korunur (ai-vision-render-studio.md). Dosya, girdiye göre TEXT-TO-IMAGE / IMAGE-TO-IMAGE / IMAGE EDITING / RESTORATION / PRODUCT / ARCHITECTURAL / CHARACTER / UI-UX RENDER / PHOTOGRAPHIC TRANSFORMATION modlarından otomatik seçim yapar ve 25+ visual preset ile çalışır (ai-vision-render-studio.md).

## Ana Fikirler
- Rol: AI Visual Render Studio + Image Prompt Engineer + Visual/Art/Camera/Lighting Director + Material Specialist + Colorist + Render Optimization Expert (ai-vision-render-studio.md).
- Ana akış: `INPUT → ANALYZE → UNDERSTAND → EXTRACT → PLAN → PROMPT → OPTIMIZE → FINAL`; kullanıcı fikri değiştirilmez, yalnızca daha kontrollü/üretilebilir hale getirilir (ai-vision-render-studio.md).
- Visual Source of Truth: referans görsel birincil kaynaktır; "PRESERVE THE ORIGINAL STRUCTURE UNLESS EXPLICITLY REQUESTED OTHERWISE." (ai-vision-render-studio.md)
- Visual analiz blokları: SUBJECT, COMPOSITION, CAMERA, LIGHTING, COLOR, MATERIAL, IMAGE QUALITY (ai-vision-render-studio.md)
- Soru sistemi opsiyoneldir: "Direkt prompt ver" diyene soru sorulmaz, yalnızca sonucu ciddi değiştirecek kritik eksiklikte soru sorulur (ai-vision-render-studio.md)
- Uzman alt modüller: Camera Director, Lighting Director, Material Director, Colorist — her biri kendi parametre listesiyle tanımlanır (ai-vision-render-studio.md)
- Kalite kuralı: "Sadece '8K' veya '16K' yazmak kalite garantisi değildir"; kalite yalnızca çözünürlükle tanımlanmaz (ai-vision-render-studio.md)
- Identity Preservation: kullanıcı istemedikçe beautification, face redesign, facial reconstruction veya identity alteration yapılmaz (ai-vision-render-studio.md)

## Önemli Alıntılar/Veriler
- **Rol:** "AI Visual Render Studio, Image Prompt Engineer, Visual Director, Art Director, Camera Director, Lighting Director, Material Specialist, Colorist ve Render Optimization Expert" (ai-vision-render-studio.md)
- **Bağlam girişi:** görsel/fotoğraf/screenshot/UI/3D render/product image/sketch/concept art/mimari görsel/çoklu referans/sadece yazılı fikir (ai-vision-render-studio.md)
- **Kısıtlar:** bilinmeyen model parametresi, lens, render engine, resolution, kamera spec, materyal, software feature, API parametresi uydurulmaz; `[BELİRTİLMELİ]` / `[UNKNOWN]` kullanılır (ai-vision-render-studio.md)
- **9 üretim modu:** TEXT TO IMAGE, IMAGE TO IMAGE, IMAGE EDITING, IMAGE RESTORATION, PRODUCT RENDER, ARCHITECTURAL RENDER, CHARACTER RENDER, UI/UX RENDER, PHOTOGRAPHIC TRANSFORMATION (ai-vision-render-studio.md)
- **Preset örnekleri:** Hyper Realistic, Photorealistic, Cinematic, Product Studio, Luxury Product, Dark Luxury, Premium Commercial, Fashion Editorial, Architecture Visualization, Automotive Studio, HDRI Studio, Cyberpunk, Sci-Fi, Fantasy, Anime, Game Asset, Concept Art, Advertising... (ai-vision-render-studio.md)
- **Prompt sırası:** `Subject → Environment → Composition → Camera → Lens → Lighting → Materials → Texture → Color → Atmosphere → Depth → Rendering → Quality` (ai-vision-render-studio.md)
- **Çıktı formatı:**
  ```text
  ## FINAL PROMPT   [FINAL IMAGE GENERATION / EDITING PROMPT]
  ## NEGATIVE PROMPT [NEGATIVE PROMPT]
  ## CAMERA          [CAMERA SETTINGS]
  ## LIGHTING        [LIGHTING SETUP]
  ## OUTPUT          [RESOLUTION / ASPECT RATIO / FORMAT]
  ```
  (ai-vision-render-studio.md)
- **Çoklu görsel rolleri:** `Image 1 → Subject, Image 2 → Style, Image 3 → Environment, Image 4 → Material, Image 5 → Composition` (ai-vision-render-studio.md)
- **Karar önceliği:** Explicit User Intent → Reference Image → Explicit Requirements → Visual Constraints → Composition → Camera → Lighting → Materials → Color → Quality Optimization (ai-vision-render-studio.md)
- **Direct render mode:** "render yap", "bunu üret", "generate", "create" gibi talimat gelirse görev render production task sayılır; yalnızca analiz isteniyorsa render üretilmez (ai-vision-render-studio.md)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[vision-render-studio]] — visual source of truth, 9 üretim modu, direkt render modu ve koruma kuralları.
- [[image-generation-prompt]] — preset seti, prompt sıralaması ve çıktı blokları.
- [[girl-gorsel-prompt]] — referans görselde kimlik/kompozisyon koruma yaklaşımı.
- [[bayram-ali-prompt-arsivi]] — dosyanın Prompt Arşivi üyesi olduğu (Version 1.0.0).
