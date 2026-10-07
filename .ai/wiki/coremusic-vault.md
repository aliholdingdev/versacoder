---
title: CoreMusic Vault Şablonu
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/coremusic-vault-template.md
  - raw/versacoder-proje-ai-00.md
tags: [coremusic, vault-template, guardrail, agent]
---

# CoreMusic Vault Şablonu

**Ne:** CoreMusic `.ai/` vault'unun **doğrulanmış, yeniden kullanılabilir şablonu**. Versa Coder analizinde **YAPI referansı** olarak kullanılır; içerik olarak CoreMusic bilgisi kopyalanmaz (versacoder-proje-ai-00, Blok 10).

**Künye (coremusic-vault-template):** `coremusic.net` · vizyon "Aynı Müzik Her Yerde Seninle" · 10 subdomain · envanter 24 klasör / 340 MD dosya · stack PHP 8.4+, Node/TS, Vanilla JS (ITCSS 9 katman + BEM), C++20 JUCE/ASIO, MySQL 9 18 BCNF.

**Kapsam:** 21 katman `K0–K20` / 1.095 bileşen · 10 panel · 7 backend servis · port kaydı 80/81/3001/3306/5000/6000/9741/9742.

**Guardrail seti:** 16 guardrail; öne çıkanlar SSOT (#5), UTF-8 yazım protokolü, append-only `log.md`, 7 alanlı frontmatter, immutable ADR.

**Agent yapısı:** 11 agent (1 MO + 10 uzman) `AGENTS.md §4`; config'te 13 (`plan`, `vault-updater` fazladan). Routing 9 keyword grubu, dispatch 7 adım, öncelik `CRITICAL 5s/1 retry → LOW 60s/2`, eskalasyon L1→L2→L3→İnsan. Review zinciri `IMPLEMENT → BUILD → TEST → ARCH → SECURITY → REVIEW → REFACTOR` — test edilmeden "tamamlandı" denmez.

**✓ Çelişki-1 ÇÖZÜLDÜ (2026-10-06) — Agent sayısı 11 vs 13:**
- `.opencode/opencode.json` → **13** agent anahtarı sayıldı: `master-orchestrator, plan, backend-architect, ui-designer, security-engineer, data-engineer, embedded-engineer, qa-engineer, devops-engineer, audio-hardware-engineer, dsp-firmware-engineer, windows-software-engineer, vault-updater`.
- `.agents/AGENTS.md` satır 29 → "…**11 agent profilinin indeksidir**"; klasörde 12 dosya (11 profil + indeks).
- Sonuç: **13 aktif / 11 dokümante**; eksik 2 = `plan`, `vault-updater`. Config'in kendi yorum satırı da "SSOT - 11 agents" deyip 13 anahtar tanımlıyor → doküman geride. Kesin değer: **13**.

**✓ Çelişki-2 ÇÖZÜLDÜ (2026-10-06 22:13) — Tablo sayısı 156 vs 112 → gerçek: 164:**
- **Kanıt:** `raw/coremusic-db-sql/` (19 DDL dosyası; `C:\www\coremusic.net\.ai\sources\.sql\mysql\` kopyası) → **165 `CREATE TABLE` / 164 benzersiz tablo**, `IF NOT EXISTS` 0.
- **156** → `README.md` satır 146/181 (2×) → eksik 8; **112** → `.ai.OLD/CLAUDE.md` satır 109 (K5) → eksik 52. Her iki iddia disk DDL ile uyuşmaz → düzeltilmesi gereken değer: **164**.
- Not: canlı DB'ye bağlanılmadı; şema dosyaları SSOT'tur (ADR-079 da bunları kullanır); canlı DB'de drift varsa ayrıca teyit gerekir.
- **Yeni tespit (ek çelişki):** `api_keys` hem `coremusic_api.sql` hem `coremusic_auth.sql` içinde, `IF NOT EXISTS` yok → ikinci yüklemede çakışma verir.

**Ek A çelişki kaydı (tamamı):** → [[coremusic-vault-ek-a]] — A-1 (agent 13) ✓, A-3 (tablo 164) ✓; A-2, A-4…A-9 açık (6 madde), ayrıca `api_keys` çift-tanım tespiti.

## İlgili Sayfalar
- [[coremusic-vault-ek-a]] — Ek A çelişki izleme defteri (A-1…A-9)
- [[vault-mimarisi]] — şablonun kurduğu düzen
- [[ssot]] — 14 değişmez kural
- [[agent-workflow]] — agent dispatch/review zinciri
- [[versacoder-analiz-seti]] — şablonun analiz edildiği set
- [[adr]] — frozen ADR immutable kuralı
