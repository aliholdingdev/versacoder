---
title: "definitions/index — L1 Definitions & Identity"
type: index
category: "definitions"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ROLE, AGENTS, engine, glossary, VISION, PROJECTS, ULTRA burada yaşar"
tags: [['definitions', 'identity', 'l1']]
source: "ADR-032 §2.3 (L1) + D38 (kök dağıtımı)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [vault index](../index.md) · [CLAUDE](../CLAUDE.md) · [governance index](../governance/index.md)

# definitions — L1 Definitions & Identity

| Dosya | Ne taşır |
|---|---|
| [ROLE](ROLE.md) | roller (18→5), karar hiyerarşisi 7 basamak, devir zinciri |
| [AGENTS](AGENTS.md) | ajan protokolü: Chat≠Agent≠LLM, dağıtım, çıktı kutusu, 3-STRIKE |
| [engine](engine.md) | orkestrasyon motoru: giriş → hazırlık → dağıtım → kontrol → teslim |
| [glossary](glossary.md) | terim sözlüğü (136 terim TR/EN/ELI10) |
| [VISION](VISION.md) | 7 ekran, Studio/IDE kimliği, 4 farklılaştırıcı |
| [PROJECTS](PROJECTS.md) | proje envanteri, dış bağımlılık, risk defteri |
| [ULTRA-THINKING](ULTRA-THINKING.md) | uzun düşünme protokolü: 5 adım, kanıt hiyerarşisi, red-team |

**Authority:** ROLE/AGENTS/engine = primary (tanım); glossary = primary (terim);
VISION/PROJECTS/ULTRA = primary (karar kaydı) — hepsi L0'ın (ADR/kural)
altındadır: çelişirse ADR kazanır (D5).
