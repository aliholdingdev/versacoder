---
title: "Sezon Sonu Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Cok oturumlu is serisinin (sezon) sonu degerlendirmesi icin prompt"
---

# Sezon Sonu Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [session-kapanis](session-kapanis-prompt.md) | [gate-kapanis](gate-kapanis-prompt.md)

> **Tek cumlelik sorumluluk:** Sezon (cok oturumlu is serisi) kapanirken kapanan ne, kalan ne, risk nedir; bunu 4 satir kutuyla yaz.

## §0 Ozet (Answer First)

**Sezon = cok oturumlu is serisi** (bir faz, bir gate zinciri ya da bir proje parcasi); **session = tek oturum**. [session-kapanis](session-kapanis-prompt.md) bu promptun **session-sonu kisidir** (kendi §0'inda boyle tanimlar): o tek oturumu kapatir, bu prompt ise tum sezonu degerlendirir. Sezon sonunda uc seye bakilir: **kapanan** (hangi gate/faz kapandi), **kalan** (WIP ve acik madde), **risk** (sezon boyunca biriken, sezon sonu skoru).

| Katip | Ne kapatir | Ne bakar |
|---|---|---|
| [session-kapanis](session-kapanis-prompt.md) | tek oturum | log + hafiza + WIP |
| Sezon sonu (bu prompt) | cok oturumlu seri | kapanan / kalan / risk |
| [gate-kapanis](gate-kapanis-prompt.md) | tek gate | gate kanit kutusu |

## §1 Girdi

- [log.md](../log.md) — sezon boyunca biriken oturum satirlari
- [PROJECTS.md](../PROJECTS.md) + [TODOS](../TODOS/index.md) — duran is ve acik maddeler
- [session-kapanis](session-kapanis-prompt.md) — son oturumun kapanis kutusu

## §2 Prompt

```text
Sezon sonu degerlendirmesi basliyor: <sezon adi>.
1. KAPANAN: hangi gate/faz kapandi (ad + kanit).
2. KALAN: WIP + acik madde (adet).
3. RISK: sezon sonu skoru 1-5 (1 kalem).
4. SEZON ACILIR MI: evet/hayir + kosul.
4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : SEZON KAPANDI | SEZON ACIK - KOSUL YAZILDI
KAPANAN   : <gate/faz> + kanit (1 satir)
KALAN     : <n> WIP / <n> acik madde
KANIT     : log.md satir sayisi + TODOS durumu
------------------------------------------------------------
```

## §4 Kabul

- Kapanan kalemin kaniti var (gate kutusu ya da log satiri)
- Kalan kalemler adetle yazildi (WIP + acik madde)
- Sezon sonu skoru 1-5; gerekce en fazla 3 kelime

## §5 RED Kosulu

WIP ve acik madde varken "SEZON KAPANDI" yazildi — SEBEP: session-kapanis §0 kurali sezon icin de gecerlidir; kapanis yalniz WIP = 0 iken mumkundur.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Oturum kapanisi ile sezon kapanisini karistirma (ikisi ayri kutu). Rakam uydurma. Madde listesi 5 maddeyi gecmez.

## §7 Sure

Sezon degerlendirmesi: 10 dk. Cikti kutusu: 1 dk. Son oturumun kapanisi [session-kapanis](session-kapanis-prompt.md) ile Once yapilir.

## §8 Kardesler

[session-kapanis](session-kapanis-prompt.md) | [gate-kapanis](gate-kapanis-prompt.md) | [genel-ozet](genel-ozet-prompt.md) | [docs](docs-prompt.md)

## §9 Kanit

Kanit: 4 satir sezon sonu kutusu. Kaynak dosyalar: session-kapanis-prompt.md (§0 sezon/oturum tanimi), log.md (oturum satirlari), TODOS/index.md (acik maddeler), WORKFLOW.md (gate sirasi), gate-kapanis-prompt.md (gate kanit kutusu).
