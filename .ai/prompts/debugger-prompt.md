---
title: "Debugger Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Hata kovugu icin sistemli kok-neden analizi"
---

# Debugger Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | `error-triage` | [hata-modu](hata-modu-prompt.md)

> **Tek cumlelik sorumluluk:** Hatanin konumunu, kokenini ve tek satirlik kok-nedenini 4 satir kutuyla ver; duzeltme onerisi kod degil aciklama olsun.

## §0 Ozet (Answer First)

Debugger hata mesajini **konum (dosya:satir) -> koken (neden boyle) -> kok neden (tek cumle)** sirasiyla ayiklar, duzeltme yonunu 4. satira yazar; **kodu duzeltmez**, [developer-prompt](developer-prompt.md)'e devreder. Ayni hatayi 2. kez gorurse eskaliere not dusur.

## §1 Girdi

- Hata mesaji / stack trace (tam metin)
- `error-triage` — oncelik siniflandirmasi
- [hata-modu](hata-modu-prompt.md) — hata moduna gecis kurali

## §2 Prompt

```text
Hata ayiklama basliyor: <hata mesaji / stack trace>.
1. KONUM: dosya:satir (stack trace'ten).
2. KOKEN: neden boyle oluyor (1 cumle).
3. KOK NEDEB: tek cumle, en derin neden.
4. YON: duzeltme yonu (kod degil aciklama).
4 satir kutuya yaz. Kod duzeltme yapma; developer-prompt'a devret.
2. kez ayni hata: eskalasyon + eskaliere not.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : KOK-NEDEB bulundu | BULUNAMADI (kapsam disi)
KONUM     : dosya:satir | stack trace yoksa "bilinmiyor"
KOK NEDEB : tek cumle
YON       : duzeltme yonu (1 satir) | eskaliere devir
------------------------------------------------------------
```

## §4 Kabul

- Kok-neden tek cumle; koken ve kok neden birbirinden ayrildi
- Yon satiri kod degil, aciklama
- Bulunamadi ise kapsam siniri yazildi (uydurma yok)

## §5 RED Kosulu

Kok-neden tahmin edilip dosya:satir verilmedi — SEBEP: konumsuz kok-neden yanlisa goturur; konum zorunlu veya "bilinmiyor" acikca yazilmali.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Stack trace'i kesme. Kod duzeltme bu promptun isi degil. Asla `as`, `bang`, `any` ekleme.

## §7 Sure

Ayiklama: 15 dk (hata modu butcesi [WORKFLOW.md](../WORKFLOW.md)). Cikti kutusu: 1 dk.

## §8 Kardesler

[hata-modu](hata-modu-prompt.md) | [test-sonucu](test-sonucu-prompt.md) | [developer](developer-prompt.md) | [seyirci-sef](seyirci-sef-prompt.md)

## §9 Kanit

Kanit: 4 satir kok-neden kutusu. Referans: `error-triage`, [AGENTS.md §1.1](../AGENTS.md). Cikti [developer-prompt](developer-prompt.md) veya [hata-modu](hata-modu-prompt.md)'ne devredilir.
