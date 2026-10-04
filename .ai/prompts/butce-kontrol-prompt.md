---
title: "Butce Kontrol Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Sure butcelerini gate bazinda denetleyip asimi bildiren prompt"
---

# Butce Kontrol Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [gate-acma](gate-acma-prompt.md) | [panel-devam](panel-devam-prompt.md)

> **Tek cumlelik sorumluluk:** Aktif gate'in butcesini ve gecen sureyi karsilastir; asim varsa 4 satir butce kutusuyla RED ver.

## §0 Ozet (Answer First)

Butce kontrol promptu **[WORKFLOW.md](../WORKFLOW.md) gate butcelerini** (45/60/90/120/40 dk) aktif gate'in gecen suresiyle karsilastirir; **asim varsa is durdurulur**, kalan sure ve kesisim riski 4 satir kutuya yazilir. Butce asimi sessiz gecilmez; [panel-devam](panel-devam-prompt.md) ile kapsam kucultulur veya gate RED kapanir.

## §1 Girdi

- Aktif gate + baslangic zamani
- [WORKFLOW.md](../WORKFLOW.md) — gate butceleri (45/60/90/120/40 dk)
- [gate-acma](gate-acma-prompt.md) — gate kutulari

## §2 Prompt

```text
Butce kontrolu basliyor: <gate>.
1. Gate butcesini oku (WORKFLOW.md).
2. Gecen sureyi hesapla (baslangic -> simdi).
3. KALAN butce = butce - gecen. Asim varsa RED.
4. Asimda: durdur + kapsam kucult onerisi + kesisim riski.
4 satir butce kutusunu yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : ICINDE | ASIM - DURDUR
GATE      : <gate> butce <dk> dk | gecen <dk> dk | kalan <dk>
AKSIYON   : kapsam kucult (1 satir) | devam
KANIT     : WORKFLOW.md butce satiri + zaman damgasi
------------------------------------------------------------
```

## §4 Kabul

- Butce, gecen ve kalan sure rakamla yazildi
- Asimda KARAR = ASIM - DURDUR (devam etmedi)
- Kesisim riski (sonraki gate'e tasima) 1 satirda

## §5 RED Kosulu

Asim varken kutuya "ICINDE" yazildi — SEBEP: rakamsal asim objektif; saklamak gate zincirini bozar; asim acikca yazilmali.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Rakam uydurma (olcum yoksa "olcum yok" yaz). Butceyi sessiz uzatma.

## §7 Sure

Kontrol: 2 dk. Cikti kutusu: 1 dk. Asim durumunda kesintisiz karar.

## §8 Kardesler

[gate-acma](gate-acma-prompt.md) | [panel-devam](panel-devam-prompt.md) | [wip-kontrol](wip-kontrol-prompt.md) | [session-kapanis](session-kapanis-prompt.md)

## §9 Kanit

Kanit: 4 satir butce kutusu. Referans: [WORKFLOW.md](../WORKFLOW.md) gate butceleri, [AGENTS.md §2](../AGENTS.md). Her gate acilisinda ve 1/2 gecisinde butce yeniden okunur.
