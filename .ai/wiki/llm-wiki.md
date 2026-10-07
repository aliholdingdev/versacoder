---
title: LLM Wiki (Bu Depo)
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/versacoder-proje-ai-00.md
tags: [wiki, hafiza, .ai, kurulum]
---

# LLM Wiki (Bu Depo)

**Ne:** `C:\versacoder\.ai` altında kurulu, **kalıcı LLM hafızası**. Herhangi bir model bağımsız çalışabilir; oturum hafızası silinse bile bilgi burada kalır.

**Okuma sırası (her oturum başında, zorunlu):** `CLAUDE.md` → `index.md` → `log.md` son 10 kayıt.

**Dizin:** `raw/` (ham, değişmez) · `sources/` (kaynak özetleri + ana tablo) · `wiki/` (işlenmiş sayfalar).

**Üç işlem:** [[ingest-proseduru|INGEST]] · QUERY · LINT. Her dosya ekleme/güncelleme/silme `index.md` + `log.md` yazımını zorunlu kılar.

**Kaynak tabanı:** İlk ingest 20 kaynak (12 Versacoder planlama + 8 ChatGPT prompt), hepsi `raw/` altına kopyalandı.

**İlham kaynağı:** Vault kuralları `coremusic-vault-template` (append-only log, 7 alanlı frontmatter, `[[link]]` düzeni, SSOT önceliği) ve Versa Coder `.ai` second brain kurallarından (ai-00, ai-01) türetildi.

**Ayrım:** Bu wiki bir *knowledge vault*; Versa Coder'ın çalışma zamanı hafızası (Working/Session Memory) değildir (bkz. [[ai-agent-hafizasi]]).

## İlgili Sayfalar
- [[ingest-proseduru]] — içerik ekleme akışı
- [[ssot]] — doğru kaynak ilkesi
- [[vault-mimarisi]] — referans vault düzeni
- [[ai-agent-hafizasi]] — ajan hafızası kavramı
- [[multi-llm-bagimsizlik]] — bağımsız model ilkesi
- [[bayram-ali-prompt-arsivi]] — ilk kaynak kümesi
