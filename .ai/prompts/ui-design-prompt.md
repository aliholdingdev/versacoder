---
title: "UI Design Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Backend'ten once arayuz tasarimi icin React + Tailwind + shadcn/ui promptu"
---

# UI Design Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [ui-designer](../.agents/ui-designer-agent.md) | `frontend`

> **Tek cumlelik sorumluluk:** Backend'ten once ekran tasarla; tek dosyada React + Tailwind + shadcn/ui ile component kutusu ver.

## §0 Ozet (Answer First)

UI Design promptu **backend'ten once calisir**: ekranin amacini, durumlarini (bos / yukleniyor / hata / dolu) ve component agini tek dosyada React + Tailwind + shadcn/ui ile uretir; **backend baglantisi yok**, `frontend-prompt`'e devredilir. Tasarim [VISION.md](../VISION.md) tek bakista anlasilir kuralina uyar.

## §1 Girdi

- Ekranin amaci (tek cumle) ve durumlari
- [VISION.md](../VISION.md) — tek bakista anlasilirlik
- [ui-designer](../.agents/ui-designer-agent.md) — rol tanimi ve cikti kutusu

## §2 Prompt

```text
UI tasarimi basliyor: <ekran amaci>.
1. Durumlari listele: bos / yukleniyor / hata / dolu.
2. Component agini yaz (parent-child, en fazla 5 dugum).
3. Tek dosya component: React + Tailwind + shadcn/ui.
4. Backend baglantisi YOK; veri prop ile gelir.
4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : TASARIM HAZIR | REVIZE
DURUMLAR  : bos | yukleniyor | hata | dolu (tek satir)
COMPONENT : <Root> -> <Child> (max 5 dugum)
KANIT     : tek dosya adi + shadcn/ui bilesenleri
------------------------------------------------------------
```

## §4 Kabul

- 4 durum eksiksiz (bos / yukleniyor / hata / dolu)
- Component agi max 5 dugum, parent-child acik
- Backend baglantisi yok; veri prop ile

## §5 RED Kosulu

Tasarimda veri cekme kodu var — SEBEP: bu prompt backend oncesi; veri baglantisi `frontend-prompt`'in isi.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Backend / API cagrisi yok. Asla `as`, `bang`, `any` ekleme.

## §7 Sure

Tasarim: 20 dk. Cikti kutusu: 1 dk. Butce asarsa durum sayisi kisilir (min 2 durum).

## §8 Kardesler

`frontend` | [ui-designer](../.agents/ui-designer-agent.md) | [genel-ozet](genel-ozet-prompt.md) | [developer](developer-prompt.md)

## §9 Kanit

Kanit: 4 satir tasarim kutusu. Referans: [ui-designer](../.agents/ui-designer-agent.md), [AGENTS.md §1.1](../AGENTS.md), [VISION.md](../VISION.md). Tasarim kabulunden sonra `frontend-prompt`'e gecilir.
