---
title: "layer-5000 — 5000+ Katman/Katalog İskeleti"
type: index
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADR-030 eksenleri + katalog/layer-5000 hedef yolları; veri henüz yok, iskelet sabit"
tags: [['architecture', 'layer-5000', 'catalog', '5000', 'adr-030']]
source: "ADR-030 + spec/layering-5000-spec.md v2.1.0 (kaynak dosya); veri katmanı henüz üretilmedi (G9)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [architecture index](../index.md) · [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-5000-check](../../checklists/layer-5000-check.md)

# layer-5000 — İskelet

> Bu klasör ADR-030'un **hedef yollarını** sabitler. Katalog verisi
> (`yon-gruplari.md`, `k5000-dll-list.md`, `T*.md` tier dosyaları) henüz
> **üretilmedi** — `gen-layer-5000.ps1` çıktısı bu dizine yazılacaktır (G9:
> veri yoksa "yok" yazılır, uydurulmaz).

## §1 Eksenler (ADR-030)

| Eksen | Hedef | Sayım kaynağı |
|---|---|---|
| Tip (katalog satırı) | ≥5000 | `count-components.ps1` |
| Düğüm (namespace/folder) | ≥5000 | katalog indeksi |
| Yön grubu = üretim `.dll` | ≥5000 (sınıf düzeyi) | `yon-gruplari.md` |

**Proje sayısı sabit 9+2** (ADR-028) — 5000 `.csproj` DEĞİLDİR.

## §2 Hedef Dosyalar (henüz yok)

- `yon-gruplari.md` — yön grubu kayıtları
- `k5000-dll-list.md` — 7 bant (K0000..K5499) + 5 faz
- `T0-foundation.md` … `T8-tests.md` — tier listeleri (gen-layer-5000.ps1 çıktısı)
- `index.md` — bu dosya (PASS/FAIL sayaç özeti)

## §3 Üretim

```powershell
# Veri üretim komutu (kaynak: G: prompt arşivi mimari-02.md + vault katalog)
powershell -NoProfile -File ..\..\scripts\gen-layer-5000.ps1 -Esik 5000
powershell -NoProfile -File ..\..\scripts\count-components.ps1 -MinRecords 5000 -MinNodeFiles 52
```

## İlgili

- [spec/layering-5000-spec](../../spec/layering-5000-spec.md) — davranış spec'i
- [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md)
