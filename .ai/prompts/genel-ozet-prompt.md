---
title: "Genel Ozet Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Oturum ve gate ciktilarindan karar + risk ozeti cikarma promptu"
---

# Genel Ozet Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [session-kapanis](session-kapanis-prompt.md) | [risk-degerlendirme](risk-degerlendirme-prompt.md)

> **Tek cumlelik sorumluluk:** Daginik ciktilari tek ozette topla: karar ne, risk ne, sonraki adim ne.

## §0 Ozet (Answer First)

Genel ozet, **kaynak dosyalardan** okuyup karar + risk ozeti uretir; hafizadan yazmaz. Ozetin uc zorunlu parcasi vardir: **KARAR** (ne kapatildi, ne acik), **RISK** (en yuksek 1 kalem, 1-5 skor), **SONRAKI** (bir sonraki adim + sahip). Ozet [docs-prompt](docs-prompt.md) ile belgeye, [session-kapanis](session-kapanis-prompt.md) ile log'a, [risk-degerlendirme](risk-degerlendirme-prompt.md) ile skorlara baglanir; ayni bilgi iki yerde farkli yazilamaz (SSUT).

| Kaynak | Ozete giren | Girmeyen |
|---|---|---|
| [log.md](../log.md) | ne yapildi, kanit | tahmin |
| [PROJECTS.md](../PROJECTS.md) | aktif proje + skor | kapali proje |
| [WORKFLOW.md](../WORKFLOW.md) | gate durumu | gate tanimi tekrari |

## §1 Girdi

- [log.md](../log.md) — oturum satirlari (ne yapildi + kanit)
- [PROJECTS.md](../PROJECTS.md) + [VISION.md](../VISION.md) — proje ve one cikarma kriteri
- [TODOS](../TODOS/index.md) — acik maddeler ve aktif faz

## §2 Prompt

```text
Genel ozet basliyor.
1. Kaynaklari oku: log.md, PROJECTS.md, TODOS/index.md.
2. KARAR: ne kapatildi / ne acik (1 satir).
3. RISK: en yuksek 1 kalem + 1-5 skor (risk-degerlendirme ile ayni olmali).
4. SONRAKI: adim + sahip + gate.
4 satir kutuya yaz. Kaynaksiz cumle yazma.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : ne kapatildi / ne acik (1 satir)
RISK      : <kalem> skor <1-5> (tek kalem)
SONRAKI   : <adim> + sahip + gate
KANIT     : kaynak dosya adlari (log / PROJECTS / TODOS)
------------------------------------------------------------
```

## §4 Kabul

- Her satir kaynak dosyaya dayaniyor (adlari yazildi)
- RISK tek kalem; risk-degerlendirme skoru ile ayni
- SONRAKI satirinda adim + sahip + gate var

## §5 RED Kosulu

Kaynak okunmadan ozet yazildi — SEBEP: hafizadan ozet kanitsizdir; AGENTS.md §2.1 madde 3 geregi "hazir" yazimi islenmez.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Kaynaksiz iddia yok. 2. bir RISK maddesi yok (en fazla 1). Madde listesi 5 maddeyi gecmez.

## §7 Sure

Ozet: 5 dk. Cikti kutusu: 1 dk. Kaynak okumasi ozensiz yapilirsa ozet gecersizdir.

## §8 Kardesler

[docs](docs-prompt.md) | [session-kapanis](session-kapanis-prompt.md) | [risk-degerlendirme](risk-degerlendirme-prompt.md) | [sezon-sonu](sezon-sonu-prompt.md)

## §9 Kanit

Kanit: 4 satir genel ozet kutusu. Kaynak dosyalar: log.md, PROJECTS.md, TODOS/index.md, AGENTS.md (§2 cikti kutusu), risk-degerlendirme-prompt.md (skor esitligi), docs-prompt.md (belgeye yazim).
