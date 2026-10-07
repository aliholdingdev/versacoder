---
title: "ADR-013 — 8 makro katman + 44 alt katman = 52 düğüm"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her düğümde bileşen kaydı; toplam ≥5000; N01..N44 alt katman adları"
tags: [adr, architecture, layers, nodes]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Katman/ölçü belirsiz kalsaydı herkes kendi sayısını uydurur, "5000+" iddiası
kanıtsız kalırdı.

## §2 Karar

Mimari = **8 makro katman (l0..l7) + 44 alt katman = 52 düğüm**. Her düğümde
bileşen kaydı tutulur; toplam **≥5000** kayıt hedeflenir. Alt katman adları
`N01..N44` aralığındadır; `architecture/` klasör yapısı bu düğümleri karşılar.
Sayım tek kaynaktan gelir (ADR-027); sayaç script'tedir, elle sayılmaz.

## §3 Alternatifler

- **Katmansız yapı:** bağımlılık kaosu. ❌
- **Aşırı ince katman (575 csproj senaryosu):** ~1150 hedef, restore/IDE çöker
  (ADR-028 §3 A3 — RED). ❌
- **8 makro + 52 düğüm (seçilen):** denetlenebilir sınır. ✅

## §4 Sonuçlar

**Ters-senaryo:** Katman/ölçü belirsiz kalsaydı herkes kendi sayısını uydurur,
"5000+" iddiası kanıtsız kalırdı.

**Yakalama:** `count-components.ps1` + `checklists/layer-boundary-check.md`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `architecture/` klasör yapısı + N01–N44 | 52 düğüm dosyası, ≥5000 kayıt |

## §6 Kaynak

- brain.md §1 satır 013 · §1.1 · §1.2

## §7 İlgili

- [ADR-028-layered-baseline-9-projects.md](ADR-028-layered-baseline-9-projects.md)
- [ADR-030-5000-layers-class-level.md](ADR-030-5000-layers-class-level.md)
- [adr-index](../adr-index.md) · [brain](../../brain.md)
