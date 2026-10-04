---
title: "Risk Degerlendirme Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Session ortasinda proje risklerini 4 satir kutuyla skorlamasi"
---

# Risk Degerlendirme Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [VISION.md](../VISION.md) | [PROJECTS.md](../PROJECTS.md) | [risk-farkindaligi](risk-farkindaligi-prompt.md)

> **Tek cumlelik sorumluluk:** Aktif projeleri tespit et, her birine 1-5 risk skoru ver, en yuksek riske aksiyon ata ve kutuya yaz.

## §0 Ozet (Answer First)

Panel aktif projeleri [PROJECTS.md](../PROJECTS.md) icinden okur, her projeye **1-5** arasi risk skoru verir (5 = kritik: build kirik / gate takili / sahipsiz), **en yüksek riskli 2 projeye aksiyon ve sahip atar**, sonucu 4 satir kutuya yazar. Oylama gerekmez; General skorlar.

## §1 Girdi

- [PROJECTS.md](../PROJECTS.md) — aktif proje listesi
- [VISION.md](../VISION.md) — one cikarma kriteri
- [WORKFLOW.md](../WORKFLOW.md) — gate durumlari (takili gate = risk yukseltir)

## §2 Prompt

```text
Risk degerlendirmesi basliyor.
1. PROJECTS.md'den aktif projeleri oku.
2. Her projeye 1-5 skor ver (5 = build kirik / gate takili / sahipsiz).
3. En yuksek 2 risk icin aksiyon + sahip yaz.
4. Skorlarin gerekcesini en fazla 3 kelime.
4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : <projeyi one cikar> one cikarildi
RISK      : p1 5 | p2 4 | p3 2 | ... (tek satir, skorlu)
AKSIYON   : en yuksek riske sahip + aksiyon + sahip
KANIT     : PROJECTS.md + gate durumu
------------------------------------------------------------
```

## §4 Kabul

- Her aktif proje tek satirda skorlu
- En yüksek 2 riske aksiyon ve sahip atandi
- Skor gerekcesi 3 kelimeyi gecmedi

## §5 RED Kosulu

Skor verilip aksiyon atanmadi — SEBEP: skoru aksiyonsuz risk raporu, is degil; aksiyon + sahip zorunlu.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Tahmini proje listesi yok; dosyadan oku. Skoru sessizce yukseltme, gerekce yaz.

## §7 Sure

Degerlendirme: 10 dk. Cikti kutusu: 1 dk.

## §8 Kardesler

[risk-farkindaligi](risk-farkindaligi-prompt.md) | [panel-oylama](panel-oylama-prompt.md) | [gate-acma](gate-acma-prompt.md) | [genel-ozet](genel-ozet-prompt.md)

## §9 Kanit

Kanit: 4 satir risk kutusu. Referans: [PROJECTS.md](../PROJECTS.md), [VISION.md](../VISION.md), [WORKFLOW.md](../WORKFLOW.md). Skorlar her session tekrarlanir; kapanista tekrar okunur.
