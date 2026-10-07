---
title: "ADR-026 — Structured log, 5 seviye, tek şablon"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Trace/Debug/Info/Warn/Error; serbest metin log yasak; iki bacakta aynı alan adları"
tags: [adr, logging, observability, structured]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Serbest metin log kalsaydı iki bacakta alan adları ayrışır, hata günlükten
çözülemezdi.

## §2 Karar

- Tek structured log şablonu; **5 seviye**: `Trace/Debug/Info/Warn/Error`
  dışında isim uydurulmaz.
- İki bacakta **aynı alan adları**.
- Serbest metin log **yasak** (kod örneği brain §3.7/3).

## §3 Alternatifler

- **Serbest metin:** ayrışan alanlar. ❌
- **Structured + 5 seviye (seçilen):** çözümlebilir günlük. ✅

## §4 Sonuçlar

**Ters-senaryo:** Serbest metin log kalsaydı iki bacakta alan adları ayrışır,
hata günlükten çözülemezdi.

**Yakalama:** `logging-observability-check` + §4.1 log satırı.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| log şablonu ve alan adları | 5 seviye dışı seviye adı = 0 |

## §6 Kaynak

- brain.md §1 satır 026 · §1.1 · §1.2 · §3 kural 2

## §7 İlgili

- [adr-index](../adr-index.md) · [brain](../brain.md)
