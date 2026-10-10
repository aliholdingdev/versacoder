---
title: "ADR-021 — Vault navigasyonu tek index.md"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her dizinde en fazla bir liste; iki dosyada tekrar yok (SRP)"
tags: [adr, vault, navigation, srp]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

İki indeks olsaydı iki gerçek doğar, link listesi bayatlar ve vault'un yarısı
görünmez kalırdı.

## §2 Karar

Vault navigasyonu **tek `index.md`** üzerinden yürür. Her dizinde **en fazla
bir** liste dosyası bulunur; hiçbir liste iki dosyada tekrarlanmaz (SRP/ADR-021).
Klasör index'leri elle yazılır, betik denetler (D52): orphan dosya + kırık indeks
linki = RED.

## §3 Alternatifler

- **Çoklu indeks:** bayat liste, çift gerçek. ❌
- **Tek indeks + klasör index'leri (seçilen):** tek gerçek kaynağı. ✅

## §4 Sonuçlar

**Ters-senaryo:** İki indeks olsaydı iki gerçek doğar, link listesi bayatlar ve
vault'un yarısı görünmez kalırdı.

**Yakalama:** `vault-link-check` + "her dizinde en fazla bir liste" taraması.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `index.md` + dizin listeleri | ikinci indeks dosyası = 0 |

## §6 Kaynak

- brain.md §1 satır 021 · §1.1 · §1.2
- D26/D52 (klasör index zorunlu + betik denetimi)

## §7 İlgili

- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
