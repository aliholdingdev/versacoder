---
title: "governance/index — L0 Governance Haritası"
type: index
category: "governance"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "governance/ klasörünün indeksi (L0 katmanı)"
tags: [governance, index, l0]
source: "ADR-032 §2.3-2.4 (L0 katmanı)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [[index|vault index]] · [[CLAUDE|CLAUDE]] · [[governance/VAULT-GATE|VAULT-GATE]]

# governance — L0 Governance & Authority

| Dosya | Ne taşır |
|---|---|
| [[governance/WORKFLOW|WORKFLOW]] | gate zinciri + kabul kriterleri (gate kimliği SSOT) |
| [[governance/VAULT-GATE|VAULT-GATE]] | `.ai` değişiklik kapısı · oturum protokolü · 10 kontrol |
| [[governance/audit|audit]] | onay kayıtları (append-only) |
| [[governance/keys|keys]] | anahtar kelime → dosya haritası (salt referans, secret yok — D17) |
| [[governance/versioning|versioning]] | 3 seviyeli sürüm modeli (D14) |

L0'ın diğer üyeleri: [[.rules/index|../.rules/]] (G-seti) ·
[[.decisions/adr-index|../.decisions/adr-index]] (ADR defteri) ·
[[CLAUDE|../CLAUDE.md]] (anayasa girişi).
