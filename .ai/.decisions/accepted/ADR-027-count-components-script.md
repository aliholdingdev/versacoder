---
title: "ADR-027 — Bileşen sayımı tek kaynaktan: count-components.ps1"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Elle sayılmaz; -MinRecords 5000, -MinNodeFiles 52; kanıtsız sayı RED"
tags: [adr, measurement, counting, evidence]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Elle sayıma izin verilseydi "5000+" iddiası her raporda farklı çıkardı; kapı
sayıya güvenirdi.

## §2 Karar

"5000+ bileşen" **elle sayılmaz**. Tek kanıt: `scripts/count-components.ps1`
(`-MinRecords 5000`, `-MinNodeFiles 52`). Kanıtsız sayı rapora yazılmaz (G9/G5).

## §3 Alternatifler

- **Elle sayı:** uydurma iddia. ❌
- **Tek script (seçilen):** kanıt. ✅

## §4 Sonuçlar

**Ters-senaryo:** Elle sayıma izin verilseydi "5000+" iddiası her raporda farklı
çıkardı; kapı sayıya güvenirdi.

**Yakalama:** `gate-004a-vault-kapanis` (Q5) + elle yazılmış sayı satırı taraması.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `scripts/count-components.ps1` | ≥5000 kayıt, 52 düğüm dosyası |

## §6 Kaynak

- brain.md §1 satır 027 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-013-layered-architecture-52-nodes|ADR-013-layered-architecture-52-nodes.md]]
- [[.decisions/accepted/adr-030-5000-layers-class-level|ADR-030-5000-layers-class-level.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
