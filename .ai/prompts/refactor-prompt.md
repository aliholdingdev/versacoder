---
title: "Refactor Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Davranisi degistirmeden kod iyilestirmesi icin adimli refactor promptu"
---

# Refactor Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [saf-refactor](saf-refactor-prompt.md) | [developer](developer-prompt.md)

> **Tek cumlelik sorumluluk:** Davranis degisikligi olmadan kod iyilestirmesi yap; her adimda build + test kaniti ver.

## §0 Ozet (Answer First)

Refactor promptu **davranisi koruyarak** okunabilirligi ve yapilandirmayi iyilestirir: adim kucuk tutulur (tek dosya veya tek fonksiyon), **her adimdan sonra build + test gecisi kanitlanir**, refactor onu ve sonrasi ayni kalmali. Davranis degisikligi gerekiyorsa refactor degil, feature olarak [developer-prompt](developer-prompt.md)'e gecilir.

## §1 Girdi

- Refactor edilecek dosya / fonksiyon
- [saf-refactor](saf-refactor-prompt.md) — kapsam siniri
- Mevcut build + test durumu (baz kanit)

## §2 Prompt

```text
Refactor basliyor: <dosya/fonksiyon>.
Kural: davranis DEGISEMEZ. Build + test baz aliniyor.
1. Tek adimlik degisiklik planla (1 dosya / 1 fonksiyon).
2. Degisikligi uygula.
3. Build + test gec; baz ile ayni sonuc mu?
4. Gecmediyse adimi geri al.
4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : REFACTOR TAMAM | GERI ALINDI
KAPSAM    : dosya:fonksiyon (tek adim)
DAVRSANIS : build + test baz ile ayni
KANIT     : build gecti / test sayisi onceki ile ayni
------------------------------------------------------------
```

## §4 Kabul

- Davranis degismedi (build + test baz ile ayni)
- Adim tek dosya / tek fonksiyon ile sinirli
- Geri alma yolu denendi (test kirmizisa adim geri alindi)

## §5 RED Kosulu

Davranis testi degisti veya yeni test eklendi — SEBEP: refactor davranis degistirmez; degisiklik feature ise bu prompt gecersizdir.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Davranis degisikligi yok. Asla `as`, `bang`, `any` ekleme. Butceyi asma.

## §7 Sure

Refactor: 20 dk (max). Cikti kutusu: 1 dk. Butce asarsa adim kucultulur.

## §8 Kardesler

[saf-refactor](saf-refactor-prompt.md) | [developer](developer-prompt.md) | [reviewer](reviewer-prompt.md) | [test-sonucu](test-sonucu-prompt.md)

## §9 Kanit

Kanit: 4 satir refactor kutusu. Referans: [saf-refactor](saf-refactor-prompt.md), [AGENTS.md §1.1](../AGENTS.md), [WORKFLOW.md](../WORKFLOW.md). Gecmiste refactor adimlari [log.md](../log.md)'a not edilir.
