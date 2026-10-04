---
title: "Session Kapanis Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Session sonunda log, hafiza ve kanit kapanisi icin prompt"
---

# Session Kapanis Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [log.md](../log.md) | [MEMORY.md](../MEMORY.md) | [sezon-sonu](sezon-sonu-prompt.md)

> **Tek cumlelik sorumluluk:** Session kapanisinda log'a tek satir yaz, hafizayi guncelle, kapanis kutusunu 4 satir ver.

## §0 Ozet (Answer First)

Session bitiminde General **[log.md](../log.md)'a tek satir** (ne yapildi, kanit, sonraki adim), **[MEMORY.md](../MEMORY.md)'a 1-3 madde** (unutulmamasi gereken), kapanis kutusunu **4 satir** ile yazar; WIP kalmadiysa kapanir, **WIP varsa kapanis RED**. Bu prompt [sezon-sonu](sezon-sonu-prompt.md)'nin session-sonu kisidir.

## §1 Girdi

- Session notlari (ne yapildi)
- [log.md](../log.md) — kayit defteri
- [MEMORY.md](../MEMORY.md) — hafiza

## §2 Prompt

```text
Session kapanisi basliyor.
1. log.md'ye tek satir: ne yapildi + kanit + sonraki adim.
2. MEMORY.md'ye 1-3 madde: unutulmazsa maliyeti olan ogeler.
3. WIP durumu: WIP varsa RED (kapanis yok).
4. 4 satir kapanis kutusunu yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : KAPANDI | WIP VAR - KAPANMADI
LOG       : log.md tek satir (tarih + is + kanit)
HAFIZA    : MEMORY.md 1-3 madde
KANIT     : sonraki adim + gate durumu
------------------------------------------------------------
```

## §4 Kabul

- log.md'ye tek satir eklendi (tarih + is + kanit + sonraki adim)
- MEMORY.md'ye 1-3 madde (fazlasi degil)
- WIP yoksa KARAR = KAPANDI

## §5 RED Kosulu

WIP acikken kapanis kutusu "KAPANDI" dedi — SEBEP: [WORKFLOW.md](../WORKFLOW.md) WIP kapanisa izin vermez; RED + neden yaz.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Log satiri 1 satiri asma. MEMORY'ye gereksiz madde doldurma.

## §7 Sure

Kapanis: 10 dk. Cikti kutusu: 1 dk.

## §8 Kardesler

[sezon-sonu](sezon-sonu-prompt.md) | [wip-kontrol](wip-kontrol-prompt.md) | [gate-kapanis](gate-kapanis-prompt.md) | [genel-ozet](genel-ozet-prompt.md)

## §9 Kanit

Kanit: 4 satir kapanis kutusu. Referans: [log.md](../log.md), [MEMORY.md](../MEMORY.md), [WORKFLOW.md](../WORKFLOW.md). Kapanis [gate-004a](../checklists/gate-004a-vault-kapanis.md) gectiyse yapilir.
