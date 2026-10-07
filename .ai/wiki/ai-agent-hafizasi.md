---
title: AI Ajan Hafızası
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/versacoder-proje-planlamasi-ai-01.md
  - raw/versacoder-proje-planlamasi-ai-02.md
  - raw/versacoder-proje-planlamasi-ai-06.md
tags: [hafiza, memory, knowledge, context]
---

# AI Ajan Hafızası

**Tanım ayrımı:** *Memory* = geçmişten bağlam taşıma, *Knowledge* = proje mühendislik bilgisi. İkisi karıştırılmaz (ai-01).

**Üç bellek katmanı (ai-08 §30):**
1. **Working Memory** — aktif oturum sliding window, token bütçesi aşınca kırpılır (ai-06 §5.5).
2. **Long-Term Knowledge** — `.ai` vault (Markdown/Obsidian SSOT).
3. **Session Memory** — oturum geçmişi (SQLite/SQL Server + audit) (ai-02).

**`.ai` Second Brain kuralları:**
- Kodlamadan önce kurulur; ~500 satır hedefi ama "satır sayısı kalite ölçütü değildir" (ai-00).
- Dolgu yasak: anlamsız tekrar, aynı bilginin çoklu tekrarı, doğrulanmamış/placeholder bilgi (ai-00).
- Her bilgi `source / date / status / confidence / related decision` metadata'sına sahiptir (ai-00).
- Bellek doğrulanmamış tahminlerle doldurulmaz; **gerçek veri > varsayım** (ai-01).

**Context Awareness:** Tüm repo her çağrıda modele gönderilmez; bağlam seçimi/indekslenmesi ayrı bir engineering problemidir (ai-01). Versa Coder'da bu **Epoch Assembly** ile Basit token penceresine karşı çözülür (ai-02).

**Continuous Learning Loop:** Elle yapılan düzeltmenin Diff'i analiz edilir, kural `.ai/learning/`e yazılır; refactor/feedback ve hata çözümleri Knowledge Vault'a akar (ai-06 §5.5, ai-02).

**Çıktılar:** "AI VALIDATION REPORT" ve "CODEBASE INDEX" (ai-00).

## İlgili Sayfalar
- [[ssot]] — tek doğru kaynak ilkesi
- [[vault-mimarisi]] — `.ai` klasör mimarisi
- [[llm-wiki]] — bu depodaki karşılığı
- [[versacoder-projesi]] — mimarideki yeri
- [[agent-workflow]] — bağlam montajının akış içindeki konumu
