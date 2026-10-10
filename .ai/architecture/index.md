---
title: "architecture/index — Ürün Mimari Haritası"
type: index
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "l0..l7 ürün katman dokümanı girişi; .ai katmanlarından bağımsız (D36), belgesel ilişki"
tags: [architecture, index, layers, l0-l7]
source: "Hedef ağaç ADR-032 §2.4 + ADR-028 §2.1 (ürün katmanları)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[index|vault index]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[.decisions/accepted/adr-030-5000-layers-class-level|ADR-030]] · [[.rules/layer-rules|layer-rules]] · [[.decisions/brain|brain]]

# architecture — Ürün Mimari Haritası

> **Tek cümlelik sorumluluk:** VersaCoder **ürün** katmanlarının (l0..l7, ADR-028)
> vault içindeki doküman haritasıdır. Bu klasör `.ai`'ın kendi L0-L4 katman
> modelinden **bağımsızdır** (D36); ilişki belgeseldir: bu klasör ürün
> mimarisini **tutar**, `.ai` katmanlarını **yansıtmaz**.

## §0 Özet (Answer First)

- **Ürün baz kararı:** ADR-028 — 8 makro katman (l0..l7) · 52 düğüm · 9 üretim +
  1-2 test · çift TFM. Tam tablo: [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]].
- **Yön:** `l7→l6→l5→l4→l2→l1→l0`; `l3` yalnız `l0`; yasak oklar `l4→l2 X` · `l5→l4 X`.
- **5000+ modeli:** ADR-030 — sınıf/namespace düzeyi kayıt; proje sabit 9+2.

## §1 Harita

| Dizin | İçerik | Durum |
|---|---|---|
| [[architecture/00-overview|00-overview]] | Genel bakış + yön diyagramı | ✅ üretildi |
| [[architecture/layer-5000|layer-5000]] | 5000+ katman/katalog iskeleti (ADR-030) | ✅ iskelet |
| `catalog/` | Bileşen kataloğu (N01..N44 satırları) | ⏳ Faz 2+ — 5000+ katalog verisi gelince |
| `l0/`..`l7/` | Makro katman başına detay dokümanı | ⏳ ürün kodu yazılırken (GATE-004 sonrası) |

> `src/**` henüz **yok** (GATE-003 kapalı); l0..l7 detay dokümanları ürün
> iskeleti kurulunca (TODOS faz 03/06) üretilir. Bu iskelet şimdiden **bozuk
> link hedeflerini karşılar** ve haritayı sabitler.

## §2 Kural Köprüleri

- Kurallar: [[.rules/layer-rules|../.rules/layer-rules]] ·
  [[.rules/solid-rules|../.rules/solid-rules]]
- Ölçüm: `../scripts/count-components.ps1` (ADR-027)
- Sınır kontrolü: [[checklists/layer-boundary-check|layer-boundary-check]]

## §3 İlgili

- [[index|vault index]] · [[.decisions/adr-index|adr-index]]
- [[.decisions/accepted/adr-032-ai-control-plane|ADR-032]] — .ai katmanları (bağımsız)
