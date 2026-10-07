---
title: Anti-Halüsinasyon Kuralları
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-03.md
  - raw/versacoder-proje-planlamasi-ai-05.md
  - raw/electronic-gpt-ozle-promt.md
tags: [zero-hallucination, guardrail, dogrulama]
---

# Anti-Halüsinasyon Kuralları

**Tanım:** Zero Hallucination = "doğrulanmamış bilgi gerçek kabul edilmez, bilinmeyen bilinmiyor olarak işaretlenir" (ai-01).

**Conditional Guardrail Engine:** IF-THEN kural şablonları system prompt'a enjekte edilir, **Roslyn AST ile kod seviyesinde** doğrulanır (ai-03).

**Durumlar:**
- **A — DB değişikliği:** önce Entity → EF Migration; ham SQL asla.
- **B — Build hatası:** Roslyn Diagnostic → Self-Healing.
- **C — Güvenlik riski:** kod durur → Approval Gate.
- **D — UI:** yalnız DevExpress MVVM; code-behind'a business logic yasak. (ai-03)

**12 Agent Davranış Kuralı (özet):** tahmin yapmaz, bilgi uydurmaz, gizli mimari karar vermez, referansı analiz etmeden implemente etmez, `.ai`'yi yok saymaz, katman ihlal etmez, UI'ya business logic gömmez, gereksiz abstraction üretmez (ai-03).

**Doğrulama kipleri:** CLI command, executable, version, endpoint, pricing doğrulanmadan yazılmaz; bilinmeyen `[VERIFY REQUIRED]` işaretlenir (ai-05 §46). Elektronik promptta eşdeğer: `[MPN DOĞRULANMALI]`, `[ÜRÜN LİNKİ DOĞRULANMALI]`, final gate `✅ DESIGN READY / ⚠ REQUIRES INFORMATION / 🛑 HARD STOP` (electronic-gpt-ozle-promt).

**Kritik bilgi ≥ 2 bağımsız kaynakla** doğrulanır; çelişkide kesin karar verilmez (ai-00).

## İlgili Sayfalar
- [[prompt-engineering]] — dürüstlük etiketleri
- [[insan-onayi-ve-denetlenebilirlik]] — Approval Gate'ler
- [[agent-workflow]] — Self-Healing döngüsü
- [[versacoder-projesi]] — guardrail'lerin platformdaki yeri
