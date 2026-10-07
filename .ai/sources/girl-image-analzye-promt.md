---
title: "AI Image Girl Analyze Prompt Systems — Forensic Visual Analysis Engine"
raw_path: "raw/girl-image-analzye-promt.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, image-analysis, reverse-engineering, reconstruction-prompt, zero-hallucination]
---

# AI Image Girl Analyze Prompt Systems (Forensic Visual Analysis Engine)

## Genel Özet
`girl-image-analzye-promt.md`, kullanıcının yüklediği görseli "forensic-level visual analysis" ile inceleyip onu yeniden üretmeye yarayan, kopyala-yapıştır hazır image generation promptuna çeviren 29 bölümlük bir sistem promptudur (Version 1.0.0, Bayram Ali Akşit) (girl-image-analzye-promt.md). Temel prensip görseli yorumlamak değil, teknik olarak çözümlemek ve yeniden üretilebilir hale getirmektir; öncelik `VISUAL FIDELITY → ACCURACY → OBSERVABLE EVIDENCE → REPRODUCIBILITY → TECHNICAL PRECISION → CREATIVITY` şeklinde tanımlıdır (girl-image-analzye-promt.md). Sistem, görsel yüklenir analiz komutu beklemeden otomatik başlar; yalnızca görünür bilgiyi kullanır, belirsizliği açıkça belirtir ve `HIGH/MEDIUM/LOW CONFIDENCE` sınıflandırması yapar (girl-image-analzye-promt.md). Çıktı analiz dili Türkçe, reconstruction promptu İngilizcedir; model-specific varyantlar (Nano Banana Pro 2, FLUX, SDXL, Pony, Illustrious, Midjourney, GPT Image, ComfyUI) desteklenir (girl-image-analzye-promt.md).

## Ana Fikirler
- Rol: 50+ yıllık Senior AI Image Analyst, Visual Reverse Engineering Specialist, Prompt Engineer, Computer Vision Analyst, Cinematography Specialist (girl-image-analzye-promt.md).
- Otomatik analiz: "Analyze this image" gibi komut beklemeden görsel yüklenince analiz kendiliğinden başlar (girl-image-analzye-promt.md).
- VISIBLE INFORMATION ONLY kuralı: görünmeyen kıyafet, arka yüz, kamera/lens modeli hakkında kesin bilgi üretilmez; `Belirsiz` / `Görsel kanıt yetersiz` kullanılır (girl-image-analzye-promt.md).
- Analiz alanları: Composition, Subject/Character, Clothing & Materials, Camera & Cinematography, Lighting, Color & Grading, Environment, Style Classification, Micro-Detail, Image Quality (girl-image-analzye-promt.md).
- Confidence System: lens, kamera, görünmeyen anatomi, LUT, AI model gibi doğrudan görülemeyen bilgiler `HIGH/MEDIUM/LOW CONFIDENCE` ile işaretlenir (girl-image-analzye-promt.md).
- Identity & Fidelity: insan karakterde yüz yapısı, orantı, poz, kıyafet korunur; istenirse `EXTREME IDENTITY PRESERVATION` uygulanır (girl-image-analzye-promt.md).
- Model-specific output: SDXL için prompt+negative+weighting, Pony/Illustrious için tag-based, Midjourney için kısa sinematik prompt+parametre, ComfyUI için workflow önerileri (girl-image-analzye-promt.md).
- Dil politikası: ana analiz Türkçe, reconstruction prompt ve teknik terimler İngilizce; terimler gereksiz yere Türkçeleştirilmez (girl-image-analzye-promt.md).

## Önemli Alıntılar/Veriler
- **Rol:** "Senior AI Image Analyst, Visual Reverse Engineering Specialist, Prompt Engineer, AI Art Director, Computer Vision Analyst..." (girl-image-analzye-promt.md).
- **Bağlam girişi:** kullanıcının yüklediği görsel (opsiyonel model/aspect ratio komutu) (girl-image-analzye-promt.md).
- **Kısıtlar:** hallucination yok; uydurma kamera/lens/model/metadata/artist/LUT/LoRA/prompt syntax yok; emin değilse "Bilinmiyor." (girl-image-analzye-promt.md).
- **Reconstruction prompt sırası:** `SUBJECT → IDENTITY → ANATOMY → POSE → CLOTHING → MATERIALS → ENVIRONMENT → COMPOSITION → CAMERA → LENS → LIGHTING → COLOR → ATMOSPHERE → RENDERING → TEXTURE → MICRO-DETAILS → QUALITY → FIDELITY` (girl-image-analzye-promt.md).
- **Çıktı formatı (9 bölüm):** 1. GÖRSEL TESPİTİ, 2. TEKNİK GÖRSEL ANALİZİ, 3. KARAKTER ANALİZİ, 4. CONFIDENCE, 5. RECONSTRUCTION STRATEGY, 6. RECONSTRUCTION PROMPT, 7. NEGATIVE PROMPT, 8. MODEL-SPECIFIC VERSION, 9. PARAMETER RECOMMENDATIONS (girl-image-analzye-promt.md).
- **Örnek güvenli ifade:** `Likely 50mm-equivalent perspective` — buna karşılık `Shot on Canon EOS R5 with 50mm f/1.2` gibi doğrulanamayan kesin bilgi uydurulamaz (girl-image-analzye-promt.md).
- **Negatif prompt odakları:** wrong anatomy, altered identity, extra/missing fingers, oversmoothing, plastic skin, unwanted style changes, watermark (girl-image-analzye-promt.md).
- **Soru kuralı:** yalnızca reconstruction kalitesini ciddi etkileyen eksikliklerde soru sorulur; kullanıcı soru yağmuruna tutulmaz (girl-image-analzye-promt.md).
- **Final rule:** "Görseli tahmin etme. Görseli çözümle. Görünmeyeni uydurma." (girl-image-analzye-promt.md)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[girl-gorsel-prompt]] — forensic analiz + reconstruction prompt zinciri ve identity preservation kuralları.
- [[image-generation-prompt]] — model-specific output varyantları ve negative prompt yapısı.
- [[prompt-engineering]] — confidence system, evidence-based output ve placeholder/disiplin örnekleri.
- [[bayram-ali-prompt-arsivi]] — dosyanın Prompt Arşivi üyesi olduğu (Version 1.0.0).
