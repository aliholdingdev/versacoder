---
title: "AI UI/UX Designer Prompt Maker & Analyst (GPOS) — Otonom UI/UX → HTML Implementation"
raw_path: "raw/ui-uix-anayze-promt-md.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, ui-ux, frontend, figma, html-css-js, design-system]
---

# AI UI/UX Designer Prompt Maker & Analyst

## Genel Özet
`ui-uix-anayze-promt-md.md`, kullanıcıya verilen Markdown dokümanları, Figma/Figma API çıktıları, PNG/JPG/SVG ekran görüntüleri ve mevcut HTML/CSS/JS kodunu tek bir "Project Knowledge Base" olarak değerlendirip UI/UX analizi ve frontend implementasyonu üreten 30 bölümlük bir sistem promptudur (Version 1.0.0, Bayram Ali Akşit) (ui-uix-anayze-promt-md.md). Prompt, genel AI asistanı yerine "Project Operating System (GPOS), AI UI/UX Analyst, Senior Frontend Engineer, Software Architect" rolünü tanımlar ve kesin bir bilgi öncelik sırası (Source of Truth) dayatır (ui-uix-anayze-promt-md.md). Varsayılan davranış AUTONOMOUS EXECUTION MODE'dur: kullanıcıya gereksiz soru sorulmaz, workflow onay beklemeden ilerler; DESIGN PRESERVATION MODE mevcut tasarımı korur ve gereksiz redesign yapmaz (ui-uix-anayze-promt-md.md). Kod tarafında semantic HTML, clean CSS, vanilla JavaScript ve framework eklememe kuralı hâkimdir (ui-uix-anayze-promt-md.md).

## Ana Fikirler
- Rol: Project Operating System (GPOS), AI UI/UX Analyst, Senior Frontend Engineer, Software Architect, Technical Project Agent (ui-uix-anayze-promt-md.md).
- Source of Truth sırası: kullanıcı son talimatı → onaylanmış proje kararları → proje dokümantasyonu → Figma/API verisi → Active Design System → mevcut UI/UX → genel mühendislik bilgisi (ui-uix-anayze-promt-md.md).
- AUTONOMOUS EXECUTION MODE: `INPUT → CONTEXT → SOURCE ANALYSIS → UI/UX ANALYSIS → REQUIREMENTS → IMPLEMENTATION PLAN → HTML/CSS/JS → VALIDATION`; kullanıcı açıkça istemedikçe soru/onay beklemez (ui-uix-anayze-promt-md.md).
- DESIGN PRESERVATION MODE: redesign, modernize, renk/typography/layout/spacing değişikliği otomatik yapılmaz; mevcut UI elementleri protected asset kabul edilir (ui-uix-anayze-promt-md.md).
- Figma verisi birincil kaynaktır: Page/Frame/Component/Auto Layout/Constraints/padding/gap/typography gibi alanlar çıkarılır; Figma yoksa Figma varmış gibi davranılmaz (ui-uix-anayze-promt-md.md).
- PNG/JPG/SVG analizi estetik değil implementation açısından yapılır: layout, spacing, typography, renk, komponent, interaction state, responsive ve accessibility (ui-uix-anayze-promt-md.md).
- FACT / INFERENCE / UNKNOWN ayrımı: `VERIFIED`, `INFERENCE`, `UNKNOWN`; inference verified gibi sunulmaz, unknown uydurulmaz (ui-uix-anayze-promt-md.md).
- Çıktı modları: ANALYSIS MODE (kod yok), IMPLEMENTATION MODE (analiz+kod), FULL MODE (doğrudan "bunu kodla" talebi) (ui-uix-anayze-promt-md.md).

## Önemli Alıntılar/Veriler
- **Rol:** "Sen genel amaçlı bir AI Assistant değilsin." → GPOS + AI UI/UX Analyst + Senior Frontend Engineer + Software Architect (ui-uix-anayze-promt-md.md).
- **Bağlam girişi:** Markdown dosyaları, Design System, Figma API çıktıları, PNG/JPG/SVG, mevcut HTML/CSS/JS, onaylanmış kararlar (ui-uix-anayze-promt-md.md).
- **Kısıtlar:** gereksiz redesign yok, gereksiz soru yok, gereksiz dependency/framework yok; "Button rengini değiştir." denirse yalnızca o değişiklik yapılır (ui-uix-anayze-promt-md.md).
- **Placeholder sistemi:** `[UNKNOWN]`, `[TO_BE_VERIFIED]`, `[NOT_AVAILABLE]` (ui-uix-anayze-promt-md.md).
- **HTML varsayılanı:** semantic HTML, clean CSS, vanilla JavaScript, responsive layout, accessible markup, reusable components, minimal dependencies; React/Vue/Angular/Tailwind kullanıcı istemedikçe eklenmez (ui-uix-anayze-promt-md.md).
- **Karar sırası:** `User Intent → Approved Decisions → Project Documentation → Figma/API → Design System → Existing Implementation → UI Reference → Correctness → Accessibility → Maintainability → Performance → Simplicity` (ui-uix-anayze-promt-md.md).
- **Ana workflow:**
  ```text
  INPUT → ACTIVE PROJECT CONTEXT → PROJECT KNOWLEDGE → FIGMA/API ANALYSIS
  → PNG/SVG/SCREENSHOT ANALYSIS → UI/UX ANALYSIS → COMPONENT MAPPING
  → DESIGN SYSTEM MAPPING → TECHNICAL ANALYSIS → IMPLEMENTATION PLAN
  → HTML → CSS → JAVASCRIPT → VALIDATION → FINAL OUTPUT
  ```
  (ui-uix-anayze-promt-md.md)
- **Validation kontrolü:** scope aşıldı mı, Design System korundu mu, hallucination var mı, test edilmemiş sonuç `verified/working/production-ready` yazılamaz (ui-uix-anayze-promt-md.md).
- **Engineering principles:** SOLID, Clean Code, DRY, KISS, YAGNI — ancak basit UI taskı Enterprise Architecture'a dönüştürülmez (ui-uix-anayze-promt-md.md).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[ui-ux-analiz-prompt]] — GPOS rolü, Source of Truth sırası, autonomous execution ve design preservation kuralları.
- [[prompt-engineering]] — FACT/INFERENCE/UNKNOWN modeli, validation checklist ve output mode tasarımı.
- [[bayram-ali-prompt-arsivi]] — dosyanın Prompt Arşivi üyesi olduğu (Version 1.0.0).
