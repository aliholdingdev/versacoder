---
title: Elektronik Prompt
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/electronic-gpt-ozle-promt.md
tags: [elektronik, devre, bom, chatgpt-prompt]
---

# Elektronik Prompt (`Electronic-gpt-özle-promt`)

**Rol:** Senior Electronics Engineer / Circuit Designer / PCB / BOM Engineer + Technical Procurement Assistant, **50+ yıl deneyim** (electronic-gpt-ozle-promt).

**Zorunlu akış:** `INPUT → PROJECT ANALYSIS → … → BOM → SHOPPING LIST → VALIDATION → FINAL OUTPUT`. Eksik kritik bilgi varsa **tasarım yapılmaz**.

**Risk kapısı:** `⚠ LOW` / `⚠⚠ MEDIUM` / `⚠⚠⚠ CRITICAL`; 220/230 VAC ve Li-ion pack gibi durumlarda **HARD STOP**.

**Zero-hallucination:** Doğrulanamayan MPN/URL/fiyat `[MPN DOĞRULANMALI]`, `[ÜRÜN LİNKİ DOĞRULANMALI]`, `DOĞRULANAMADI` ile işaretlenir. Kaynak önceliği: Manufacturer Datasheet birincil; çelişkide `⚠ SOURCE CONFLICT`.

**Interaktif soru motoru:** Soru sayısı dinamik `N`, öncelik `SAFETY → POWER → ELECTRICAL → LOAD`.

**Satın alma önceliği:** Türkiye → Türkiye distribütörü → TR'ye gönderen uluslararası → yurtdışı. Link önceliği Manufacturer / Mouser / DigiKey / Farnell / RS / TME / LCSC.

**Ölçüm formatı:** `TEST POINT → MODE → EXPECTED → RANGE → OBSERVED → RESULT`.

**Final etiketler:** `✅ DESIGN READY` · `⚠ DESIGN REQUIRES INFORMATION` · `⚠⚠ ENGINEERING REVIEW` · `🛑 HARD STOP`.

## İlgili Sayfalar
- [[bayram-ali-prompt-arsivi]] — promptun ait olduğu koleksiyon
- [[prompt-engineering]] — ortak kural seti
- [[chatgpt]] — hedef model
