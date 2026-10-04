---
title: "Panel Oylama Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Kapanis onu onemli kararlar icin 4 goz + 1 General oylama usulu"
---

# Panel Oylama Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [gate-004a](../checklists/gate-004a-vault-kapanis.md)

> **Tek cumlelik sorumluluk:** Kapanis onu ciddi kararlari 4 goz tek tek oyla, General son oy ile baglayip sonucu 4 satir kutuya yazdir.

## §0 Ozet (Answer First)

Kapanis onu onemli bir karar varsa (proje kapandi mi, mimari degisti mi, gate gecti mi) General kendi kararini vermez; 4 gozden (architect, developer, reviewer, tester) tek tek oy ister, **eşitlikte General'in oyu belirleyicidir**, sonucu 4 satir kanit kutusuna yazar. Oylama 12 dakikayi gecmez.

## §1 Girdi

- [index.md](index.md) — kimin oy verecegi (4 rol)
- [AGENTS.md](../AGENTS.md) — General otoritesi ve §1.1 roller
- [WORKFLOW.md](../WORKFLOW.md) — gate kapanis kararlari
- Karar metni: tek cumle, net (ornek: "L4 dokumani kabul")

## §2 Prompt

```text
Panel oylamasi basliyor. Karar: <tek cumle>.
4 goz, oy ver: KABUL / RET / CEKIMSER + tek gerekce satiri.
Oy sirasi: architect, developer, reviewer, tester.
General: 4 oyu oku; esitlik varsa kendi oyunu acikla ve bagla.
Sonucu 4 satir kutuya yaz: KARAR / OYLAR / GEREKCE / KANIT.
12 dakika butce; oylar gelmezse General tek basina karar veremez, kapanis ertelenir.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : KABUL | RET
OYLAR     : arch KABUL | dev KABUL | rev RET | test KABUL
GEREKCE   : tek satir, ret varsa ret gerekcesi zorunlu
KANIT     : dosya + satir, gate kutusu varsa o da
------------------------------------------------------------
```

## §4 Kabul

- 4 oy eksiksiz ve tek tek yazildi (birlestirilmis tek oy yok)
- Eşitlik (2-2) durumunda General'in oyu acikca yazildi
- Ret oyu varsa gerekcesi tek satirda

## §5 RED Kosulu

Biri soyut oy verdi ("bence iyi") — SEBEP: gerekcesiz oy denetlenemez; gerekce satiri zorunlu.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Oylari tahmin etme. Butceyi asma.

## §7 Sure

Panel oylamasi: 12 dk. Cikti kutusu: 1 dk.

## §8 Kardesler

[panel-devam](panel-devam-prompt.md) | [genel-ozet](genel-ozet-prompt.md) | [wip-kontrol](wip-kontrol-prompt.md) | [risk-degerlendirme](risk-degerlendirme-prompt.md)

## §9 Kanit

Kanit: 4 satir oylama kutusu. Referans: [AGENTS.md §1.1](../AGENTS.md), [WORKFLOW.md](../WORKFLOW.md). Ayni anda en fazla tek oylama acik tutulur.
