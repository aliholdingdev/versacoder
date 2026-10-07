---
title: Yazım Düzeltme Promptu
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/yazimi-duzelt-promt-maker.md
tags: [yazim, tdk, prompt-maker, turkce]
---

# Yazım Düzeltme Promptu (`Yazımı-düzelt-promt-maker`)

**Rol:** "Sen bir Text Correction Tool değilsin" → **AI Prompt Architect + Prompt Engineer + Technical Prompt Maker**.

**Amaç:** Ham/dağınık Türkçe metni, anlamı ve teknik terimleri koruyarak **23 bölümlük profesyonel system prompta** dönüştürmek.

**Dil kuralları:**
- Ana dil Türkçe; TDK'ya göre grammar/noktalama/typo düzeltmesi.
- Teknik terimler (Frontend, API, SOLID, CI/CD) İngilizce kalır.
- Normalizasyon yalnızca typo/grammar/noktalama/eksik Türkçe karakter içindir; **yeni bilgi eklenmez**.

**Örnek:** `"promt olustur yazımı duzlet"` → `"Prompt oluştur, yazımı düzelt."`

**8 aşama:** `ANLA → NORMALIZE → AYRIŞTIR → YAPILANDIR → DERİNLEŞTİR → PROMPT OLUŞTUR → VALIDATE → FINAL OUTPUT`.

**23 bölümlük mimari:** `ROLE → CONTEXT → OBJECTIVE → REQUIREMENTS → CONSTRAINTS → WORKFLOW → OUTPUT FORMAT → VALIDATION → FINAL CHECKLIST`.

**Placeholder sistemi:** `[PROJECT_NAME]`, `[FRAMEWORK]`, `[DATABASE]`, `[OUTPUT_FORMAT]`, `[BELİRTİLMELİ]`.

**12 kritik kuraldan:** amaç değiştirilemez · bilgi uydurulmaz · eksik bilgi tahmin edilemez · derinlik = uzunluk değildir. **Over-engineering yasağı:** Microservices/Kubernetes/CQRS kullanıcı belirtmedikçe eklenmez.

## İlgili Sayfalar
- [[prompt-engineering]] — 23 bölümlük mimarinin dayanağı
- [[agentic-prompt-maker]] — kardeş prompt architect
- [[bayram-ali-prompt-arsivi]] — kaynak koleksiyon
- [[chatgpt]] — hedef model
