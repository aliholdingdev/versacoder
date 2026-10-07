---
title: UI/UX Analiz Promptu
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/ui-uix-anayze-promt-md.md
  - raw/ui-uix-anayze-promt-md-tomd.md
tags: [ui-ux, figma, ascii, analiz]
---

# UI/UX Analiz Promptu

İki varyant: **uygulama** (`ui-uix-anayze-promt-md`) ve **Markdown'a teknik çıktı** (`-tomd`).

## 1) Analiz + Implementasyon
- **Rol:** GPOS + AI UI/UX Analyst + Senior Frontend Engineer + Software Architect; "genel amaçlı AI Assistant değilsin".
- **Source of Truth sırası:** son talimat → onaylı kararlar → dokümantasyon → Figma/API → Design System → mevcut UI → genel bilgi.
- **AUTONOMOUS EXECUTION MODE:** soru/onay beklemeden `INPUT → … → HTML/CSS/JS → VALIDATION`.
- **DESIGN PRESERVATION MODE:** redesign/renk/typography/layout değiştirilmez; mevcut UI protected asset.
- **Çıktı modları:** ANALYSIS / IMPLEMENTATION / FULL.
- **Bilgi sınıflandırması:** `VERIFIED / INFERENCE / UNKNOWN`; inference verified gibi sunulmaz. Placeholder: `[UNKNOWN]`, `[TO_BE_VERIFIED]`, `[NOT_AVAILABLE]`.
- **Varsayılan kod:** semantic HTML, clean CSS, vanilla JS, minimal dependency — framework kullanıcı istemedikçe eklenmez.

## 2) Markdown'a Tersine Mühendislik (-tomd)
- ASCII tasarım önerisi değil, `geometry + hierarchy + component + implementation` **reference modelidir**.
- Component ID sistemi `HEADER-01`, `CARD-01..04`; ID'ler tüm Markdown'da stabil kalır.
- Mapping: geometry tablosu `| ID | Element | x | y | w | h | Parent |` · token eşlemesi · CSS/BEM eşlemesi.
- UI Design Markdown 10 bölüm: ASCII Layout, Component Map, Geometry, CSS/BEM, Tokens, Accessibility, Responsive, State, Conflicts, Source References.
- Kural: `YES → REPRESENT / NO → DO NOT INVENT / UNCERTAIN → UNKNOWN`. Status: `VERIFIED / INFERRED / UNKNOWN / CONFLICT`.
- Kaynak önceliği: `USER → APPROVED DECISION → DOCUMENTATION → PNG → FIGMA/API → CODE → ASCII`.
- "Geometrik doğruluk > ASCII karakter sayısı"; emoji ve hayali coordinate/pixel yasak.

## İlgili Sayfalar
- [[prompt-engineering]] — ortak doğrulama kuralları
- [[agentic-prompt-maker]] — kardeş prompt maker
- [[bayram-ali-prompt-arsivi]] — kaynak koleksiyon
- [[chatgpt]] — hedef model
