---
title: "memory/index — L2 Memory (derived)"
type: index
category: "memory"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "MEMORY.md burada yaşar; historical ≠ authority (D9/D28)"
tags: [['memory', 'derived', 'l2', 'd9']]
source: "ADR-032 §2.10 + D9 (tam ayrım) + D38"
supersedes: ""
---

**Zorunlu Bağlantılar:** [vault index](../index.md) · [knowledge hub](../knowledge/index.md) · [adr-index](../.decisions/adr-index.md)

# memory — L2 Memory (derived)

| Dosya | Ne taşır |
|---|---|
| [MEMORY](MEMORY.md) | kalıcı dersler, karar desenleri, kara liste, kayıt defteri |

**Authority: `derived`.** Memory = geçmiş/öğrenilen; **authoritative değildir**:
- Bir karar ADR'yi (L0) **override edemez** (D5).
- Bilgi eskidiğinde: `supersedes` → [archive](../archive/) → stale taraması (D28/D10).
- Memory ↔ knowledge ayrımı (D9): memory = "ne yaşadık", knowledge = "kanıtlı olgu".

## İlgili

- [knowledge hub](../knowledge/index.md) · [patterns](../patterns/index.md)
