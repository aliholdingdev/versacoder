---
title: "ADR-023 — %TEMP%\opencode-fresh referans ağacı salt okunur"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Yapı öğrenilir, kod/metin kopyalanmaz"
tags: [adr, reference, readonly, g1]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Referans ağaçına yazılsaydı ikinci oturum artık temiz referans göremez, öğrenme
kaynağı kirlenirdi.

## §2 Karar

`%TEMP%\opencode-fresh` referans ağacı **salt okunurdur**. Yapı öğrenilir;
kod/metin **kopyalanmaz** (ADR-002 ile uyumlu).

## §3 Alternatifler

- **Yazılabilir referans:** kirlenme. ❌
- **Salt okunur (seçilen):** temiz referans. ✅

## §4 Sonuçlar

**Ters-senaryo:** Referans ağaçına yazılsaydı ikinci oturum artık temiz referans
göremez, öğrenme kaynağı kirlenirdi.

**Yakalama:** Yalnız-oku komut denetimi → **DOĞRULAMA GEREKLİ**.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `%TEMP%\opencode-fresh` | o ağaça yazma işlemi = 0 (salt okunur) |

## §6 Kaynak

- brain.md §1 satır 023 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-002-vault-ownership-no-copy|ADR-002-vault-ownership-no-copy.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
