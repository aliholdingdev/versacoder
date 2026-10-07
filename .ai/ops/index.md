---
title: "ops/index — L4 Operations"
type: index
category: "ops"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "log (append-only), TODOS (faz+gate), VAULT-GATE raporları, scripts köprüsü"
tags: [['ops', 'log', 'todos', 'l4']]
source: "ADR-032 §2.3 (L4) + D45 (raporlar ops/ altına) + D49"
supersedes: ""
---

**Zorunlu Bağlantılar:** [vault index](../index.md) · [VAULT-GATE](../governance/VAULT-GATE.md) · [governance index](../governance/index.md)

# ops — L4 Operations

| Öğe | Ne taşır |
|---|---|
| [log](log.md) | append-only iş günlüğü + Quality Report (D12) |
| [TODOS/](TODOS/) | proje fazları + gate takibi (D49 — ops katmanına bağlı) |
| VAULT-GATE raporları | faz sonu 10 kontrol çıktıları (D45) — üretilecek |
| [../scripts/](../scripts/) | 14 mevcut + 10 yeni `check-*` (D40, Faz 4) |
| [../archive/](../archive/) | supersedes + eski ağaç (D26/D27/D28) |

**Authority: `reference/shadow`** — ops kayıtları kanıt taşır, kural **koymaz**.
