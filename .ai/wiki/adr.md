---
title: ADR (Mimari Karar Kaydı)
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/coremusic-vault-template.md
  - raw/versacoder-proje-planlamasi-ai-09.md
  - raw/versacoder-proje-planlamasi-ai-05.md
tags: [adr, mimari-karar, immutable, karar-kaydi]
---

# ADR (Architecture Decision Record)

**Ne:** Mimari kararların kalıcı kayıt belgesi. Vault'ta `.decisions/` altında saklanır (coremusic-vault-template).

**Değişmezlik kuralı:** **Frozen ADR immutable'dır** — onaylandıktan sonra düzenlenmez; yeni bilgi gelirse yeni ADR açılır, eski üzerine yazılmaz (coremusic-vault-template, 14 değişmez kural).

**Oluşturma akışı:** `.workflows/adr-creation.md` (6 adım) ile üretilir; ayrıca `archived ADR` indeksi tutulur (coremusic-vault-template).

**Versa Coder örneği:** **ADR-010** — TUI/CLI/Blazor'un aynı çekirdeği (`l0..l6`) çağırması kararı; `Terminal.Gui`'a iş mantığı girmez (versacoder-proje-planlamasi-ai-09 §2).

**Senkron kuralı:** Oturum kapanışında Git staging + ADR + oturum geçmişi + `.ai` vault güncellemesi birlikte yazılır (versacoder-proje-planlamasi-ai-05/ai-02 §6).

**Uyarı:** ADR numaralandırması ve şablon alanları bu iki kaynakta sınırlı; tam şablon `coremusic-vault-template` içindeki `.templates/` referansına bağlıdır.

## İlgili Sayfalar
- [[ssot]] — immutable kuralının dayanağı
- [[vault-mimarisi]] — `.decisions/` klasörünün yer aldığı düzen
- [[versacoder-projesi]] — ADR-010 kararı
- [[terminal-tui-kabugu]] — çekirdek/uyarlı ayrımı
