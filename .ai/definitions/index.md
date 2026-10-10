---
title: "definitions/index — L1 Definitions & Identity"
type: index
category: "definitions"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ROLE, AGENTS, engine, glossary, VISION, PROJECTS, ULTRA burada yaşar"
tags: [definitions, identity, l1]
source: "ADR-032 §2.3 (L1) + D38 (kök dağıtımı)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [[index|vault index]] · [[CLAUDE|CLAUDE]] · [[governance/index|governance index]]

# definitions — L1 Definitions & Identity

| Dosya | Ne taşır |
|---|---|
| [[definitions/ROLE|ROLE]] | roller (18→5), karar hiyerarşisi 7 basamak, devir zinciri |
| [[definitions/AGENTS|AGENTS]] | ajan protokolü: Chat≠Agent≠LLM, dağıtım, çıktı kutusu, 3-STRIKE |
| [[definitions/engine|engine]] | orkestrasyon motoru: giriş → hazırlık → dağıtım → kontrol → teslim |
| [[definitions/glossary|glossary]] | terim sözlüğü (136 terim TR/EN/ELI10) |
| [[definitions/VISION|VISION]] | 7 ekran, Studio/IDE kimliği, 4 farklılaştırıcı |
| [[definitions/PROJECTS|PROJECTS]] | proje envanteri, dış bağımlılık, risk defteri |
| [[definitions/ULTRA-THINKING|ULTRA-THINKING]] | uzun düşünme protokolü: 5 adım, kanıt hiyerarşisi, red-team |

**Authority:** ROLE/AGENTS/engine = primary (tanım); glossary = primary (terim);
VISION/PROJECTS/ULTRA = primary (karar kaydı) — hepsi L0'ın (ADR/kural)
altındadır: çelişirse ADR kazanır (D5).
