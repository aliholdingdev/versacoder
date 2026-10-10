---
title: "ADR-022 — İlk commit yalnız GATE-004'te"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Önce 0 bozuk link + GATE-003 wireframe onayı; erken commit yasak"
tags: [adr, workflow, gate, commit]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Erken commit olsaydı bozuk linkli, kapısız depo geçmişi oluşur ve geri alma
pahalılaşırdı.

## §2 Karar

İlk commit **yalnız GATE-004'te** yapılır. Ön koşullar:
0 bozuk link + GATE-003 wireframe onayı. `git add -A` yasak; commit dosya
dosya, kanıtla yapılır.

## §3 Alternatifler

- **Erken commit:** kirli geçmiş. ❌
- **Kapılı commit (seçilen):** temiz geçmiş. ✅

## §4 Sonuçlar

**Ters-senaryo:** Erken commit olsaydı bozuk linkli, kapısız depo geçmişi oluşur
ve geri alma pahalılaşırdı.

**Yakalama:** `gate-004-ilk-commit`, `gate-004a-vault-kapanis`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| git commit akışı | GATE-004 öncesi commit = 0 |

## §6 Kaynak

- brain.md §1 satır 022 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
