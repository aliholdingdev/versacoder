---
title: "Katman Kuralları (G12)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADR-028 bağlayıcı: yön, yasaklar, yeni tip nereye, gate disiplini"
tags: [rules, layers, architecture, g12, adr-028]
source: "Birleştirme: .ai/CLAUDE.md §5 + ADR-028 + G12 (gate disiplini)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

# Katman Kuralları (G12 · ADR-028)

## G12 — Yetki + gate disiplini

Kod **yalnız rolün alanında**; gate'leri **kullanıcı** kapatır (VAULT-GATE →
004a → 003 → 004 → 005 → 006, D21). Ajan kapıyı hazırlar, asla kapatmaz.
Kapalı gate'de `.cs` yazımı = RED; dosyalar geri alınır.

## ADR-028 bağlayıcı özet

- **8 makro katman l0..l7** + 44 alt katman = **52 düğüm**; **9 üretim + 1-2 test**
  projesi; çift TFM.
- **Yön:** `l7→l6→l5→l4→l2→l1→l0`; `l3` yalnız `l0`.
- **İki doğrudan ok yasağı:** `l4→l2 X` · `l5→l4 X`
  (port `l0`'da, uygulama `l4`'te).
- **Yeni tip nereye:** arayüz→l0 · iş kuralı→l1 · senaryo→l2 · kesit→l3 ·
  somut dış dünya→l4 · araç sözleşmesi→l5 · kurulum→l6 · ekran/komut→l7.
- Mimari sınır yasakları: Domain → UI/DevExpress/Infrastructure/Database/AI
  Provider **❌**; UI → Infrastructure/Database/CLI **❌**;
  Agent Runtime → UI/DevExpress **❌** (ai.08 §49).

## 5000+ notu (ADR-030)

"5000+ katman" = **sınıf/namespace düzeyi kayıt**; proje sayısı **sabit 9+2**.
Sayım tek kaynaktan: `count-components.ps1` (ADR-027).

## İhlalde

RED — mimari; NetArchTest RED = 0 olmadan commit yok (GATE-005).

## İlgili

- [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]]
- [[.decisions/accepted/adr-030-5000-layers-class-level|ADR-030]]
- [[.rules/solid-rules|solid-rules]] · [[architecture/index|architecture/index]]
