---
title: "00-overview — Katman Yönü Diyagramı"
type: doc
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADR-028 §2.1 yön diyagramı (D22: Unicode izinli); vault'un tek yön haritası"
tags: [['architecture', 'overview', 'layers', 'direction', 'd22']]
source: "ADR-028 §2.1 + .ai/CLAUDE.md §5.1 (çapraz kontrol); D22 Unicode politikası"
supersedes: ""
---


**Zorunlu Bağlantılar:** [architecture index](../index.md) · [ADR-028](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [layer-rules](../../.rules/layer-rules.md)

# Katman Yönü (tek kaynak)

```text
 l7-ui  ──────►  l6-host  ──────►  l5-protocol
                     │                  │
                     ▼                  ▼
             l4-infrastructure ──► l2-application ──► l1-domain ──► l0-abstractions
                     │                     │                 ▲
                     └──► l3-crosscutting ──┘                 │
                            (l3 yalnız l0'a bağlanır)         │
                                                              │
                  (her katman içeriye yalnız interface ile bağlanır)
```

**Yasak oklar:** `l4 → l2` **X** · `l5 → l4` **X** · `l3` yalnız `l0`.

**Not (D22):** Bu diyagram U+2500..U+257F Unicode kutu karakterleri kullanır —
kullanıcı kararı "Unicode kazanır" (ADR-032 §2.16); CLI/headless çıktısı ASCII kalır.

**Kanıt:** NetArchTest `LayerRulesTests` 8×8 matris — elle çizilmiş matris kanıt
değildir (ADR-028 §2.3).

## İlgili

- [layer-rules](../../.rules/layer-rules.md) · [ADR-030](../../.decisions/accepted/adr-030-5000-layers-class-level.md)
