---
title: "Hata Modu Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Build/test kirikken devreye giren daralan hata modu kurali"
---

# Hata Modu Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [debugger](debugger-prompt.md) | [hata-ayiklama](../playbooks/debug-hatayi-bulma.md)

> **Tek cumlelik sorumluluk:** Build veya test kirmizisa döndüğünde yeni is acma; tek kok-nedene kilitlen, 4 satir mod kutusu ver.

## §0 Ozet (Answer First)

Build veya test kiriksa panel **hata moduna** gecer: yeni feature/gorev acilmaz, tum dikkat **tek kok-nedene** odaklanir, her adim build+test ile dogrulanir, **15 dk'da cozulmezse eskaliere** cikilir. Mod sonrasi [gate-acma](gate-acma-prompt.md) ile yeniden devam edilir; mod disinda calismak [WORKFLOW.md](../WORKFLOW.md)'e aykiridir.

## §1 Girdi

- Kirik build / test ciktisi (tam metin)
- [hata-ayiklama](../playbooks/debug-hatayi-bulma.md) — adim adim ayiklama
- [debugger](debugger-prompt.md) — kok-neden promptu

## §2 Prompt

```text
HATA MODU aktif: build/test kirmizi.
1. Yeni is ACMA. Tek kok-nedene kilitlen.
2. debugger-prompt ile konum + kok-neden cikar.
3. Her adimdan sonra build + test gec.
4. 15 dk'da cozulmediyse eskaliere cik; modu bildir.
4 satir mod kutusunu yaz. Cozulunce gate-acma'ya don.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : HATA MODU ACIK | KAPANDI (test yesil)
KOK NEDEB : dosya:satir + tek cumle
SURE      : <dk> / 15 dk (asildiysa eskalasyon)
KANIT     : build+test sonucu (once kirmizi, sonra ...)
------------------------------------------------------------
```

## §4 Kabul

- Mod acildiysa yeni is acilmadi (tek kok-neden)
- Her adim build+test ile kanitlandi
- 15 dk asildiysa eskalasyon yazildi (sessiz devam yok)

## §5 RED Kosulu

Modda yeni feature uzerinde calisildi — SEBEP: kirik build uzerine ozellik eklemek hasari buyutur; mod tek kok-nedene kilitlidir.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Hatayi yutma / sessiz gecme yok. Asla `as`, `bang`, `any` ekleme.

## §7 Sure

Tek deneme: 15 dk. 2. deneme: eskalasyon. Cikti kutusu: 1 dk.

## §8 Kardesler

[debugger](debugger-prompt.md) | [gate-acma](gate-acma-prompt.md) | [test-sonucu](test-sonucu-prompt.md) | [seyirci-sef](seyirci-sef-prompt.md)

## §9 Kanit

Kanit: 4 satir hata modu kutusu. Referans: [hata-ayiklama](../playbooks/debug-hatayi-bulma.md), [AGENTS.md §1.1](../AGENTS.md), [WORKFLOW.md](../WORKFLOW.md). Mod kapanisi = yesil build + yesil test, sonra [gate-acma](gate-acma-prompt.md).
