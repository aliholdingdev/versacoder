---
title: "VERSA CODER — Ajan Dosyası (Gölge)"
type: constitution
category: root
status: active
version: 3.0.0
updated: 2026-10-07
authority: SSOT
scope: "Kök AGENTS.md: kural metni değil yalnız bağlantı; asıl belge .ai/AGENTS.md'dir"
---

# VERSA CODER — Ajan Dosyası (Gölge)

**Kaynak:** [.ai/AGENTS.md](.ai/definitions/agents.md) — yalnız **gölge**; Chat≠Agent≠LLM ayrımı,
dağıtım matrisi, çıktı kutusu, Internal/External Agent, orchestrator, wizard, 3-STRIKE
ve bütçe oradadır, burada tekrar edilmez (G12).

**Zorunlu Bağlantılar:** [index](.ai/index.md) · [templates](.ai/.templates/index.md) · [rules](.ai/.rules/index.md) · [architecture](.ai/architecture/index.md) · [WORKFLOW](.ai/governance/workflow.md) · [decisions](.ai/.decisions/adr-index.md)

- **Ne okurum:** yukarıdaki 6 bağlantı; bu dosya kural metni barındırmaz.
- **Ne üretirim:** 5 alanlı ASCII ajan kutusu; `KANIT` satırı komut çıktısı / satır no.
- **Kapı:** `GATE-003` onayından önce `src/**` altına satır yazılmaz (G8 · G12).
- **Temel ayrım:** `Chat ≠ Agent` · `Agent ≠ LLM` · `LLM ≠ VersaCoder` ·
  `External CLI ≠ VersaCoder Agent Runtime` — ayrıntı [.ai/AGENTS.md](.ai/definitions/agents.md) §1.

---

## §1 Baglanti Agu (tam capraz-link)

Bu dosyanin diger 18 vault dosyasina baglantilari (eksiksiz; hicbir baglanti silinmemistir):

| # | Dosya | Ne tasiyor |
|---|---|---|
| CLAUDE | [CLAUDE](.ai/claude.md) | AI anayasa — G1..G12 hard guardrail, kanit formati, dur/bekle |
| ROLE | [ROLE](.ai/definitions/role.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](.ai/definitions/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| brain | [brain](.ai/.decisions/brain.md) | ADR ozetleri (29) + SOLID kisitlari + 6 sert kod kurali |
| VISION | [VISION](.ai/definitions/vision.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](.ai/definitions/projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](.ai/definitions/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| keys | [keys](.ai/governance/keys.md) | anahtar kelime -> hedef dosya hiz haritasi (101 satir) |
| MEMORY | [MEMORY](.ai/memory/memory.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [log](.ai/ops/log.md) | append-only is gunlugu + Quality Report |
| ULTRA | [ULTRA](.ai/definitions/ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.ai/.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok CLAUDE | [kok CLAUDE](claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok README | [kok README](readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok WORKFLOW | [kok WORKFLOW](workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
