---
title: "Versa Coder — Context Assembly Protokolü"
type: context
category: assembly
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Context Assembly Protokolü

**Zorunlu Bağlantılar:** [[CLAUDE.md]] §15 · [[engine.md]]

---

## 1. Amaç

Tüm bilgi kaynaklarının **birleştirilerek AI'a sunulması** sürecini tanımlayan protokol.

---

## 2. Assembly Akışı

```
Session Init → Vault Oku → Learning Yükle → Session Yükle → Proje Analiz → Context Assembly
```

| Adım | Kaynak | Max Süre |
|------|--------|----------|
| 1 | .ai/ CLAUDE.md, AGENTS.md, WORKFLOW.md, brain.md | 25s |
| 2 | .ai/learning/ patterns, corrections, knowledge | 5s |
| 3 | .ai/memory/ session state | 3s |
| 4 | .ai/project/ structure, analysis | 5s |
| 5 | .ai/context/ sources, rules, priorities | 2s |
| 6 | Kullanıcı girdisi | — |
| 7 | Birleştirilmiş context + prompt | — |

---

## 3. Context Öncelik Sırası

| Sıra | Kaynak | Ağırlık |
|------|--------|---------|
| 1 | Vault (guardrails) | Yüksek |
| 2 | Learning (patterns) | Yüksek |
| 3 | Session (geçmiş) | Orta |
| 4 | Proje (yapı) | Orta |
| 5 | Diyagram (görsel) | Düşük |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
