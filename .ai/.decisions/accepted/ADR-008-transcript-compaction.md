---
title: "ADR-008 — Transcript compaction: 8 MB eşiği, son 20 tur + özet"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Aşarsa compaction; özgün metin arşive silinmez"
tags: [adr, session, transcript, compaction]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Transcript sınırsız büyüyünce bellek taşar; özgün metin silinirse geçmiş
konuşma kanıtı kaybolur.

## §2 Karar

- Transcript **8 MB** eşiğini aşarsa compaction uygulanır:
  **son 20 tur + özet** canlı kalır.
- Özgün metin **arşive silinmez** (kayıp yasak).

## §3 Alternatifler

- **Compaction yok:** bellek taşması. ❌
- **Özgün metni silme:** kanıt kaybı. ❌
- **Compaction + arşiv (seçilen):** hem bellek hem kanıt. ✅

## §4 Sonuçlar

**Ters-senaryo:** Compaction olmasaydı transcript belleği taşar; özgün metin
silinseydi geçmiş konuşma kanıtı kaybolurdu.

**Yakalama:** Ayrı denetim listesi yok → **DOĞRULAMA GEREKLİ** (yeni checklist
`session-handoff-check` kapsamına alınmalı).

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| transcript depolama kodu | 8 MB eşiği aşınca compaction; arşivden silinen özgün metin = 0 |

## §6 Kaynak

- brain.md §1 satır 008 · §1.1 · §1.2

## §7 İlgili

- [adr-index](../adr-index.md) · [brain](../../brain.md)
