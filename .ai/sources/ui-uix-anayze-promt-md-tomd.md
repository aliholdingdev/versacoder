---
title: "Deep UI/UX Reference ASCII Generator — Teknik ASCII Layout Üreticisi"
raw_path: "raw/ui-uix-anayze-promt-md-tomd.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, ui-ux, ascii-layout, reverse-engineering, design-system, figma]
---

# Deep UI/UX Reference ASCII Generator

## Genel Özet
`ui-uix-anayze-promt-md-tomd.md`, PNG + Figma/Figma API + SVG + HTML/CSS/JS + Markdown + Design System kaynaklarından doğrulanabilir bir UI referansı çıkarıp bunu UI Design Markdown içine konulabilecek teknik ASCII layout olarak üreten 20 bölümlük bir sistem promptudur (Version 1.0.0, Bayram Ali Akşit) (ui-uix-anayze-promt-md-tomd.md). ASCII burada tasarım önerisi değil, kaynaklardan türetilen "geometry + hierarchy + component + implementation" reference modelidir (ui-uix-anayze-promt-md-tomd.md). Rol; Senior UI/UX Architect, Frontend Architect, Reverse Engineering Specialist, Design System Architect ve Technical Documentation Engineer'dır (ui-uix-anayze-promt-md-tomd.md). Sistem, doğrulanamayan her değer için `UNKNOWN` / `VERIFICATION REQUIRED` / `CONFLICT` işaretlerini zorunlu kılar ve hayali pixel, coordinate, CSS, token, state yasaktır (ui-uix-anayze-promt-md-tomd.md). Diğer UI/UX promptunun (ui-uix-anayze-promt-md.md) aksine bu dosya kod üretimine değil, tersine mühendislikle referans dokümana odaklanır.

## Ana Fikirler
- Rol: Senior UI/UX Architect, Frontend Architect, Reverse Engineering Specialist, Design System Architect, Technical Documentation Engineer (ui-uix-anayze-promt-md-tomd.md).
- Source of Truth önceliği: `User Instruction → Approved Decisions → Project Documentation → Figma/API → PNG/JPG/SVG → Design System → Existing Code → General Knowledge` (ui-uix-anayze-promt-md-tomd.md).
- DEEP ANALYSIS MODE: `DISCOVER → VERIFY → MEASURE → MAP → MODEL → DRAW → VALIDATE`; gereksiz soru sorulmaz, eksik bilgi kaynaklardan çözülür (ui-uix-anayze-promt-md-tomd.md).
- ASCII geometri kuralı: gerçek canvas genişliği gerekmez, **relative geometry** korunur; "Geometrik doğruluk > ASCII karakter sayısı" (ui-uix-anayze-promt-md-tomd.md).
- Component ID sistemi: her bileşen benzersiz ve stabil ID alır (`HEADER-01`, `CARD-01`, `BUTTON-01`...), tekrar edenler sıralanır (`CARD-01..CARD-04`); ID'ler tüm Markdown boyunca aynı kalır (ui-uix-anayze-promt-md-tomd.md).
- Mapping katmanları: Component/Geometry Map tablosu (`| ID | Element | x | y | w | h | Parent |`), Design Token eşlemesi, CSS/BEM eşlemesi, State Model, Responsive Model, Accessibility (ui-uix-anayze-promt-md-tomd.md).
- ASCII source of truth değildir; öncelik `USER → APPROVED DECISION → DOCUMENTATION → PNG → FIGMA/API → CODE → ASCII` (ui-uix-anayze-promt-md-tomd.md).
- Yasaklar: emoji, rastgele decoration, hayali component/coordinate/pixel/CSS/responsive behavior/state (ui-uix-anayze-promt-md-tomd.md).

## Önemli Alıntılar/Veriler
- **Rol/amaç:** "PNG + Figma/Figma API + SVG + HTML/CSS/JS + Markdown + Design System + proje dokümantasyonundan doğrulanabilir UI Reference çıkarıp, UI Design Markdown içine konulabilecek teknik ASCII Layout üretmektir." (ui-uix-anayze-promt-md-tomd.md)
- **Kısıtlar:** kaynakta olmayan pixel, coordinate, component, class, font, color, spacing, breakpoint, state, behavior uydurmaz; conflict gizlenmez `CONFLICT` / `VERIFICATION REQUIRED` olarak işaretlenir (ui-uix-anayze-promt-md-tomd.md).
- **İlişki zinciri örneği:**
  ```text
  PNG Element ↔ Figma Node ↔ CSS Selector ↔ Component ↔ Design Token
  ```
  Doğrulanamayan ilişki kabul edilmez (ui-uix-anayze-promt-md-tomd.md).
- **Örnek geometri bloğu:**
  ```text
  ┌── HEADER (x:0,y:0,w:1024,h:60) ──────────────────────────────┐
  │ HEADER                                                       │
  ├──────────────────────────────────────────────────────────────┤
  │ CONTENT ...                                                  │
  ```
  (ui-uix-anayze-promt-md-tomd.md)
- **Token eşleme örneği:** `HEADER-01 → token: --cm-header-h-embedded`, `CARD-01 → radius: --cm-radius-md`; token yoksa `TOKEN: UNKNOWN`, yeni token üretilmez (ui-uix-anayze-promt-md-tomd.md).
- **CSS eşleme örneği:** `CARD-01 → class: .home-mini-card`, doğrulanmıyorsa `CLASS: UNKNOWN` (ui-uix-anayze-promt-md-tomd.md).
- **Status değerleri:** `VERIFIED / INFERRED / UNKNOWN / CONFLICT` (ui-uix-anayze-promt-md-tomd.md).
- **UI Design Markdown formatı (10 bölüm):** `## 1. ASCII Layout, 2. Component Map, 3. Geometry, 4. CSS/BEM Mapping, 5. Design Tokens, 6. Accessibility, 7. Responsive Behavior, 8. State Model, 9. Conflicts / Verification, 10. Source References` (ui-uix-anayze-promt-md-tomd.md).
- **Reverse engineering kuralı:** `YES → REPRESENT / NO → DO NOT INVENT / UNCERTAIN → UNKNOWN / VERIFICATION REQUIRED` (ui-uix-anayze-promt-md-tomd.md).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[ui-ux-analiz-prompt]] — ASCII reference üretimi, component ID sistemi, geometry/token/CSS mapping ve quality gate.
- [[prompt-engineering]] — UNKNOWN/CONFLICT işaretleri ve "DO NOT INVENT" tersine mühendislik disiplini.
- [[bayram-ali-prompt-arsivi]] — dosyanın Prompt Arşivi üyesi olduğu (Version 1.0.0).
