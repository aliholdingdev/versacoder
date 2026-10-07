---
title: "AI Agentic IDE — Prompt Maker (Yazımı Düzelt + Prompt Üret)"
raw_path: "raw/yazimi-duzelt-promt-maker.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, prompt-engineering, yazim-duzeltme, metin-normalizasyonu, tr]
---

# AI Agentic IDE — Prompt Maker (Yazım Düzeltme)

## Genel Özet
`yazimi-duzelt-promt-maker.md`, kullanıcının ham, dağınık, yazım hatalı veya eksik metnini (dağınık not, typo, kısa fikir, gereksinim, ham prompt) profesyonel ve doğrudan kullanılabilir bir AI Prompt'a dönüştüren 41 bölümlük bir sistem promptudur (Version 1.0.0, Bayram Ali Akşit) (yazimi-duzelt-promt-maker.md). Dosyanın başlığı "AI AGENTIC IDE — PROMPT MAKER" olmakla birlikte işlevi yazım düzeltme + anlama koruyarak prompt mimarlığıdır; son bölümde kendini "Text Correction Tool" değil "AI Prompt Architect + Prompt Engineer + Technical Prompt Maker" olarak tanımlar (yazimi-duzelt-promt-maker.md). Ana dil Türkçedir ve TDK yazım kuralları uygulanır; buna karşılık yazılım/mühendislik/UI-UX teknik terimleri Türkçeleştirilmez (yazimi-duzelt-promt-maker.md). Temel kural kullanıcının fikrini ve kısıtlarını korumak, eksik bilgiyi tahmin etmemek ve placeholder kullanmaktır (yazimi-duzelt-promt-maker.md).

## Ana Fikirler
- Rol: AI Prompt Engineer, Prompt Architect, Senior Software Engineer, Software/System/AI Agent Architect, Technical Writer; 50+ yıllık deneyim (yazimi-duzelt-promt-maker.md).
- Temel prensip: "Kullanıcının fikrini değiştirme; fikrini profesyonel bir Prompt haline getir." (yazimi-duzelt-promt-maker.md)
- Dil politikası: ana dil Türkçe, TDK/grammar/noktalama/typo düzeltilir; teknik terimler (Frontend, API, Router, SOLID, CI/CD...) İngilizce kalır (yazimi-duzelt-promt-maker.md).
- Normalizasyon yalnızca typo, yazım, grammar, noktalama, eksik Türkçe karakter ve açık ifade bozuklukları içindir; yeni bilgi eklenmez (yazimi-duzelt-promt-maker.md).
- Zero hallucination: framework, library, API, version, architecture, file path, database, dependency uydurulmaz; eksik bilgi `[BELİRTİLMELİ]` placeholder'ıyla bırakılır (yazimi-duzelt-promt-maker.md).
- Prompt mimarisi 23 hiyerarşik bölümdür: ROLE, EXPERIENCE, LANGUAGE, CONTEXT, OBJECTIVE, SCOPE, REQUIREMENTS, TECHNICAL REQUIREMENTS, ARCHITECTURE, RULES, CONSTRAINTS, WORKFLOW, ..., OUTPUT FORMAT, VALIDATION, ERROR HANDLING, FINAL CHECKLIST (yazimi-duzelt-promt-maker.md).
- Over-engineering yasağı: kullanıcı belirtmedikçe Microservices, Kubernetes, CQRS, Event Sourcing, Distributed Systems eklenmez (yazimi-duzelt-promt-maker.md).
- Final çıktı tek bloktur: `# GENERATED PROMPT` altında doğrudan kullanılabilir prompt (yazimi-duzelt-promt-maker.md).

## Önemli Alıntılar/Veriler
- **Rol:** AI Prompt Engineer + Prompt Architect + Technical Prompt Maker; "50+ yıllık Senior Engineering deneyimine sahipmiş gibi hareket et." (yazimi-duzelt-promt-maker.md)
- **Bağlam girişi:** dağınık metin, typo, kısa fikir, teknik not, gereksinim, ham prompt, madde listesi, kodlama/UI-UX/AI Agent talebi (yazimi-duzelt-promt-maker.md).
- **Kısıtlar:** anlam değiştirilemez, requirement eklenemez/çıkarılamaz, constraint zayıflatılamaz, teknik terim Türkçeleştirilemez, gereksiz complexity eklenemez (yazimi-duzelt-promt-maker.md).
- **Örnek normalizasyon:** ham `"promt olustur yazımı duzlet"` → `"Prompt oluştur, yazımı düzelt."` (yazimi-duzelt-promt-maker.md)
- **Örnek anlama korunarak profesyonelleştirme:** ham `"önce projeyi anla sonra ai klasörü oluştur ve her şeyi oraya yaz"` → `"Öncelikle mevcut project structure analiz edilmelidir... .ai klasörü oluşturulmalı ve AI-related project documentation burada Single Source of Truth olarak yapılandırılmalıdır."` (yazimi-duzelt-promt-maker.md)
- **Placeholder listesi:** `[PROJECT_NAME]`, `[PROJECT_PATH]`, `[FRAMEWORK]`, `[LANGUAGE]`, `[VERSION]`, `[DATABASE]`, `[API_TYPE]`, `[ENVIRONMENT]`, `[OUTPUT_FORMAT]`, `[BELİRTİLMELİ]` (yazimi-duzelt-promt-maker.md)
- **Karar sırası:** User Intent → Explicit Requirements → Explicit Constraints → Existing Project Structure → Correctness → Security → Maintainability → Performance → Simplicity (yazimi-duzelt-promt-maker.md)
- **8 aşamalı çekirdek mod:** ANLA → NORMALIZE ET → AYRIŞTIR → YAPILANDIR → DERİNLEŞTİR → PROMPT OLUŞTUR → VALIDATE ET → FINAL OUTPUT (yazimi-duzelt-promt-maker.md)
- **12 kritik kural:** RULE 01 amaç değiştirilemez; RULE 03 bilgi uydurulamaz; RULE 04 terim Türkçeleştirilemez; RULE 06 eksik bilgi tahmin edilemez; RULE 11 derinlik gereksiz uzunluk değildir (yazimi-duzelt-promt-maker.md)
- **Master transformation:**
  ```text
  USER RAW INPUT → TEXT NORMALIZATION → INTENT DETECTION → REQUIREMENT EXTRACTION
  → CONSTRAINT EXTRACTION → TECHNICAL TERM PRESERVATION → LOGICAL STRUCTURING
  → PROMPT ARCHITECTURE → WORKFLOW DESIGN → VALIDATION DESIGN → QUALITY CONTROL
  → FINAL PROFESSIONAL PROMPT
  ```
  (yazimi-duzelt-promt-maker.md)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[yazim-duzeltme-prompt]] — normalizasyon kapsamı, anlam koruma kuralı, dil/teknik terim politikası.
- [[prompt-engineering]] — 23 bölümlük prompt mimarisi, placeholder sistemi, 8 aşama ve 12 kural.
- [[bayram-ali-prompt-arsivi]] — dosyanın Prompt Arşivi üyesi olduğu (Version 1.0.0).
