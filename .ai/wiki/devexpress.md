---
title: DevExpress
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/versacoder-proje-planlamasi-ai-03.md
  - raw/versacoder-proje-planlamasi-ai-06.md
  - raw/versacoder-proje-planlamasi-ai-07.md
tags: [devexpress, ui, winforms, toolbar]
---

# DevExpress

**Kullanım:** Versa Coder'ın masaüstü UI kütüphanesi — **DevExpress 25.2.8 Universal**, .NET hedefiyle (versacoder-proje-planlamasi-ai-00).

**Bileşenler:** `DockManager` / `DocumentManager` (çoklu panel düzeni), Visual Execution Graph (görsel DAG), canlı telemetri (token/latency/maliyet), Approval Gate UI (versacoder-proje-planlamasi-ai-06 §5.6).

**Guardrail (ai-03, Durum D):** UI'da **yalnız DevExpress MVVM** kullanılır; `code-behind`'a business logic gömülmesi yasaktır. Kural Conditional Guardrail Engine ile system prompt'a enjekte edilir ve Roslyn AST ile denetlenir.

**Ekosistem kaynağı:** `DevExpress/agent-skills` reposu, skill katmanı için envantere alındı (versacoder-proje-planlamasi-ai-07).

**Kapsam dışı:** Sürüm-changelog, lisanslama ve bileşen API detayları hiçbir kaynakta yok — kaynak yoksa iddia yok.

## İlgili Sayfalar
- [[versacoder-projesi]] — UI teknoloji kararları
- [[anti-halüsinasyon-kurallari]] — MVVM guardrail'i (Durum D)
- [[agent-skills-ekosistemi]] — DevExpress skill repoları
- [[coremusic-vault]] — benzer UI katmanı şablonu
