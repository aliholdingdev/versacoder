---
title: "Security Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Kod ve konfigurasyon icin OWASP odakli guvenlik denetimi promptu"
---

# Security Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [security-auditor](../.agents/security-auditor-agent.md) | [dependency-audit](../playbooks/guvenlik-denetimi.md)

> **Tek cumlelik sorumluluk:** Girdi yuzeyinde OWASP kategorilerini tara, bulgulari onceliklendir, tek satirlik guvenlik karari ver.

## §0 Ozet (Answer First)

Security promptu **OWASP kategorileri** (enjeksiyon, kimlik dogrulama, veri ifsa, guvensiz deserializasyon, XSS) uzerinden kod ve konfigurasyon tarar, bulguyu **YUKSEK / ORTA / DUSUK** onceliklendirir ve 4 satir kutuyla karar verir; **kodu bu prompt duzeltmez**, [security-auditor](../.agents/security-auditor-agent.md) veya developer'a devreder.

## §1 Girdi

- Taranacak dosya / endpoint / konfigurasyon
- [dependency-audit](../playbooks/guvenlik-denetimi.md) — bagimlilik acigi kaynagi
- [AGENTS.md](../AGENTS.md) — Security rolu ve cikti kutusu

## §2 Prompt

```text
Guvenlik denetimi basliyor: <kapsam>.
1. OWASP kategorilerini tara (enjeksiyon / auth / veri ifsa / XSS).
2. Her bulgu: ONCELIK (YUKSEK/ORTA/DUSUK) + konum + tek cumle etki.
3. En yuksek 2 bulgu icin yon (kod degil aciklama).
4 satir kutuya yaz. Kod duzeltme yapma; devret.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : GECTI | BULGU VAR | KRITIK
BULGU     : oncelik - dosya:satir - etki (max 2 satir)
YON       : duzeltme yonu (kod degil, 1 satir)
KANIT     : OWASP kategorisi + dosya referansi
------------------------------------------------------------
```

## §4 Kabul

- Her bulgu oncelik + konum + etki ile yazildi
- Kritik bulgu varsa KARAR = KRITIK (sessiz gecis yok)
- Duzeltme yonu kod degil aciklama

## §5 RED Kosulu

Bulgu "guvenli sanildi" diye atlandi — SEBEP: tarama kapsam disi acikca yazilmali; sessiz atlama denetlenemez.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Hard-coded sifre/anahtar bulursan **maskele** (kod parcasini kopyalama). Asla `as`, `bang`, `any` ekleme.

## §7 Sure

Denetim: 15 dk. Cikti kutusu: 1 dk. Kritik bulgu: aninda eskalasyon.

## §8 Kardesler

[security-auditor](../.agents/security-auditor-agent.md) | [dependency-audit](../playbooks/guvenlik-denetimi.md) | [reviewer](reviewer-prompt.md) | [sezon-sonu](sezon-sonu-prompt.md)

## §9 Kanit

Kanit: 4 satir guvenlik kutusu. Referans: [AGENTS.md §1.1](../AGENTS.md), [dependency-audit](../playbooks/guvenlik-denetimi.md). KRITIK karari [WORKFLOW.md](../WORKFLOW.md) gate zincirine baglidir; kapanis onu tekrar okunur.
