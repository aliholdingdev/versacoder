---
title: Versa Coder Projesi
type: proje
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/versacoder-proje-planlamasi-ai-01.md
  - raw/versacoder-proje-planlamasi-ai-05.md
  - raw/versacoder-proje-planlamasi-ai-08.md
  - raw/versacoder-proje-planlamasi-ai-09.md
tags: [versacoder, proje, mimari, agentic-ide]
---

# Versa Coder Projesi

**Tanım:** Versa Coder, kullanıcının doğal dildeki mühendislik görevini anlayan, yapılandıran ve Agent/Tool/Session/Workflow ile yürüten bir **Agentic Software Engineering Platform**'udur; "sohbet eden asistan" değil, insan ile AI Agent arasında kurulan mühendislik çalışma ortamıdır (ai-01, ai-00).

**Teknoloji:** C#, net48 + net10.0-windows, DevExpress 25.2.8 Universal, ASP.NET Core, Class Library/DLL, SOLID; DDD/CQRS "gerektiği yerde" (ai-00).

**Reddedilen model:** `Prompt → LLM → Answer`. Hedef zincir: `User Intent → Context Discovery → … → Auditable Result` (ai-01).

**Platform anatomisi (6 katman):** Presentation / Application / Agent Runtime / Protocol-MCP / Infrastructure (28+ DLL) / Domain-Core. Katman akışı `Domain → Abstractions → Application → Infrastructure → UI`; ihlaller derleme ve review'da engellenir (ai-02, ai-08).

**Kod yazma sırası:** `UNDERSTAND → ANALYZE → MODEL → SEPARATE → VALIDATE → PLAN → APPROVE → IMPLEMENT`. "Önce mimariyi doğru kur, sonra implementation yap." (ai-08)

**Arayüz:** Tam ekran TUI; sıralı çıktı yalnız `--headless` modda. Paketler: Terminal.Gui 2.5.0, PrettyPrompt 6.0.5, Markdig 1.4.0, Spectre.Console. Tahmin: TUI iskeleti ~2.5-3 ay (ai-09).

**Karar hiyerarşisi:** Doğruluk > Eksiksizlik > Hız; Gerçek veri > Varsayım; Kullanıcı onayı > Agent varsayımı (ai-00).

İlgili detaylar: [[versacoder-analiz-seti]], [[referans-proje-analizi]], [[agent-workflow]], [[session-wizard]], [[terminal-tui-kabugu]], [[opencode]].

## İlgili Sayfalar
- [[agent-workflow]] — platformun yürütme modeli
- [[session-wizard]] — oturum başlatma arayüzü
- [[versacoder-analiz-seti]] — 100 partlık analiz seti
- [[referans-proje-analizi]] — opencode referansının analiz yöntemi
- [[terminal-tui-kabugu]] — arayüz teknoloji kararları
- [[devexpress]] — UI kütüphanesi ve MVVM guardrail'i
- [[vault-mimarisi]] — `.ai` knowledge layer entegrasyonu
- [[multi-llm-bagimsizlik]] — sağlayıcı bağımsızlığı
