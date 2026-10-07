---
title: "ADR-009 — Araç sandbox'ı: timeout 60 sn / bash 300 sn + iptal + çalışma klasörü"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her araç çağrısı timeout'lu; iptal edilebilir token; çalışma klasörü = alan kökü"
tags: [adr, tools, sandbox, timeout]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Timeout olmasaydı tek bir araç çağrısı gate'i sonsuza kadar askıda tutar;
çalışma klasörü sabitlenmezse betik yanlış dizine yazar.

## §2 Karar

- Her araç çağrısı **60 sn** timeout (`bash` için **300 sn**).
- İptal edilebilir **token** mevcuttur.
- Çalışma klasörü = **alan kökü** (sabitlenir).

## §3 Alternatifler

- **Timeout yok:** askıda kalma. ❌
- **Timeout + iptal + sabit kök (seçilen):** güvenli yürütme. ✅

## §4 Sonuçlar

**Ters-senaryo:** Timeout olmasaydı tek araç çağrısı gate'i sonsuza kadar askıda
tutar; çalışma klasörü sabitlenmezse betik yanlış dizine yazar.

**Yakalama:** Ayrı denetim listesi yok → **DOĞRULAMA GEREKLİ**.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| araç çalıştırıcı (tool-runner) | timeout 60 sn / bash 300 sn; iptal token mevcut |

## §6 Kaynak

- brain.md §1 satır 009 · §1.1 · §1.2

## §7 İlgili

- [ADR-006-opencode-tool-contract.md](ADR-006-opencode-tool-contract.md)
- [adr-index](../adr-index.md) · [brain](../../brain.md)
