---
title: "Panel Devam Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Butce asiminda kapsam kucultme ve panelin devam karari icin prompt"
---

# Panel Devam Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [butce-kontrol](butce-kontrol-prompt.md) | [panel-oylama](panel-oylama-prompt.md)

> **Tek cumlelik sorumluluk:** Butce asiminda kapsam kucultme secenegi uret; panelin devam kararini 4 satir kutuya yaz.

## §0 Ozet (Answer First)

[butce-kontrol](butce-kontrol-prompt.md) `ASIM` verdiginde panel **devam etmez**; once kapsam kucultme secenegi uretilir: hangi adim cikarilir, kalan adimlar neler, kalan sure kac dk. Secenek kabul edilirse is kucultulmus kapsamla devam eder; kabul edilmezse is [session-kapanis](session-kapanis-prompt.md) ile kapatilir. Butceyi sessiz uzatmak yok: asim rakamla yazilir (AGENTS.md §2).

| Secenek | Ne degisir | Karar sonucu |
|---|---|---|
| Kucult | 1 adim cikar; kalan adimlar ayni | DEVAM |
| Durdur | is durur, gate kirmizi | KAPANIS |
| Reddet / devam | kapsam degismez | normal akis devam |

## §1 Girdi

- [butce-kontrol](butce-kontrol-prompt.md) — ASIM karari, butce, gecen ve kalan sure
- [WORKFLOW.md](../WORKFLOW.md) — gate sirasi ve gate butceleri (45/60/90/120/40 dk)
- [panel-oylama](panel-oylama-prompt.md) — kapsam degisikligi oylama gerektiriyorsa

## §2 Prompt

```text
Panel devami basliyor: <gate>.
1. butce-kontrol kutusunu oku: KARAR ASIM mi?
2. Kapsam kucultme secenegi: cikacak 1 adim + kalan adimlar + kalan sure.
3. Secenek oylansin (panel-oylama); esitlikte General baglar.
4. Karar: DEVAM | KAPANIS. Butceyi uzatma, yeniden olc.
4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : DEVAM (kapsam kucultuldu) | KAPANIS (secenek yok)
KAPSAM    : cikan adim + kalan adimlar (1 satir)
KALAN     : <dk> kalan / <dk> butce
KANIT     : butce-kontrol kutusu + zaman damgasi
------------------------------------------------------------
```

## §4 Kabul

- Kucultme secenegi somut: cikacak adim adiyla yazildi
- Karar 4 satirda: DEVAM ya da KAPANIS (belirsiz yok)
- Kalan sure rakamla verildi; olcum yoksa "olcum yok" yazildi

## §5 RED Kosulu

Kapsam kucultuldu denip cikan adim yazilmadi — SEBEP: adimsiz kucultme denetlenemez; hangi adimin ciktigi 1 satirda gorunur.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Butceyi sessiz uzatma. Rakam uydurma; olmayan olcumu "olcum yok" diye yaz. Madde listesi 5 maddeyi gecmez.

## §7 Sure

Kucultme secenegi: 3 dk. Karar: 2 dk. Cikti kutusu: 1 dk. Butce asiminda kesintisiz karar verilir.

## §8 Kardesler

[butce-kontrol](butce-kontrol-prompt.md) | [panel-oylama](panel-oylama-prompt.md) | [wip-kontrol](wip-kontrol-prompt.md) | [session-kapanis](session-kapanis-prompt.md)

## §9 Kanit

Kanit: 4 satir panel-devam kutusu. Kaynak dosyalar: butce-kontrol-prompt.md (ASIM kutusu), WORKFLOW.md (gate butceleri), AGENTS.md (§2 cikti kutusu), panel-oylama-prompt.md (kapsam karari oylamasi).
