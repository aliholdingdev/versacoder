---
title: "versioning — 3 Seviyeli Sürüm Modeli"
type: policy
category: "governance"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "vault-wide + dosya frontmatter + ADR status/date (D14)"
tags: [['versioning', 'policy', 'd14']]
source: "D14 kararı (2026-10-07 onaylı plan) + kök CLAUDE.md B9 (mevcut tetikleyici)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [VAULT-GATE](VAULT-GATE.md) · [adr-index](../.decisions/adr-index.md) · [CLAUDE](../CLAUDE.md)

# versioning — 3 Seviye (D14)

| Seviye | Nerede | Ne zaman değişir |
|---|---|---|
| **1. Vault-wide** | `index.md` frontmatter `version` (major.minor.patch) | major: yapı/katman değişimi · minor: yeni alan/kural · patch: düzeltme |
| **2. Dosya** | her `.md` frontmatter `version` + `updated` | içerik değiştiğinde minor; imleç/düzeltme patch |
| **3. ADR** | `status` + `updated` (+ `supersedes`) | lifecycle: draft→proposed→accepted→implemented→superseded→deprecated |

**Kural:** Sayı eklemek için sürüm **artırılmaz** — sürüm, **anlam değiştiğinde**
artar (G9: amaçsız sürüm yasak). `implemented` olan ADR = drift reconcile
tetikleyicisi (D25).

## İlgili

- [ADR-032](../.decisions/accepted/ADR-032-ai-control-plane.md) §2.9
