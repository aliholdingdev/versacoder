---
title: "ADR-007 — Oturum hiyerarşisi: 1 ana + alt oturumlar, derinlik 2"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Alt oturum yetki devretmez; derinlik sınırı 2"
tags: [adr, session, agent, hierarchy]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Çalışma zamanı oturum modeli tanımsız kalırsa token ve dosya yazma patlar;
alt oturum kendi yetkisini devredebilir.

## §2 Karar

- Çalışma zamanı = **1 ana oturum** + alt oturumlar.
- Alt oturum **yetki devretmez**; devir 4 madde ile sınırlıdır (tanım: AGENTS.md).
- **Derinlik sınırı 2.**

## §3 Alternatifler

- **Sınırsız derinlik:** token/yaşam döngüsü patlar. ❌
- **Tek oturum:** paralel iş yapılamaz. ❌
- **Sınırlı hiyerarşi (seçilen):** denetlenebilir paralellik. ✅

## §4 Sonuçlar

**Ters-senaryo:** Derinlik sınırsız olsaydı alt oturum kendi yetkisini
devredebilir, token ve dosya yazma patlardı.

**Yakalama:** `session-handoff-check` + yetki devri 4 madde kuralı.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| oturum/alt oturum yönetim kodu | derinlik ≤ 2; yetki devri 4 madde dolu |

## §6 Kaynak

- brain.md §1 satır 007 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-009-tool-execution-sandbox|ADR-009-tool-execution-sandbox.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
