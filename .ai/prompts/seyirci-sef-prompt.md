---
title: "Seyirci Sef Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Kod disi gozlemci modu: isi ureten elden bagimsiz gozlem ve rapor promptu"
---

# Seyirci Sef Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [debugger](debugger-prompt.md) | [reviewer](reviewer-prompt.md)

> **Tek cumlelik sorumluluk:** Isi yapan ajanin disindan bak; gozlemini kanit ve konumla 4 satir kutuya yaz, kod degistirme.

## §0 Ozet (Answer First)

**kaynak eslestirme: DOĞRULAMA GEREKLI** — bu promptun adi vault'ta yalnizca [debugger](debugger-prompt.md) ve [hata-modu](hata-modu-prompt.md) kardesler satirlarinda geciyor; tanimini anlatan baska bir kaynak dosya yok. Bu dosya, o linkin karsiligi olmak icin **kod disi gozlemci (seyirci sef)** modu olarak yazildi: gozlemci kod yazmaz, hatayi duzeltmez, yalniz bakar ve raporlar. Tanim vault'ta dogrulanana kadar bu satir gecerlidir.

Gozlemcinin gorevi uc soruyu cevaplamaktir: **ne goruldu** (gozlem), **nerede** (dosya / ekran / komut ciktisi), **sonuc ne** (kabul / red). Gozlemci [reviewer](reviewer-prompt.md)'dan farklidir: reviewer kodu denetler, seyirci sef ise isin **akisanı** izler ve not alir.

| Gozlem alani | Soru | Cikti |
|---|---|---|
| Sira | Adimlar sirayla mi yuruyor | sapma satiri |
| Kanit | Her adimda cikti var mi | kanit yok satiri |
| Kesisim | Baska is bu isi bozuyor mu | kesisim uyarisi |

## §1 Girdi

- Izlenen is akisi (gate sirasi ya da tek adim)
- [debugger](debugger-prompt.md) — hata ayiklama sirasinda gozlem
- [WORKFLOW.md](../WORKFLOW.md) — hangi kapi, hangi kanit kutusu

## §2 Prompt

```text
Seyirci sef modu: <izlenen is>.
1. Ne goruldu? (gözlem, yorum KATMA)
2. Nerede? dosya:satir ya da ekran / komut ciktisi.
3. Kanit var mi? yoksa "kanit yok" yaz.
4. Sonuc: KABUL | RED + tek gerekce.
Kod degistirme. 4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : KABUL | RED | IZLEMEDE
GOZLEM    : ne goruldu (1 satir, yorumsuz)
KONUM     : dosya:satir | ekran | komut ciktisi
KANIT     : gözlem kaniti | KANIT YOK
------------------------------------------------------------
```

## §4 Kabul

- Gozlem yorumsuz: "boyle oldu" degil, "sunu gordum"
- Konum ya da "bilinmiyor" acikca yazildi
- Kod degistirilmedi; karar yalniz gozlemle verildi

## §5 RED Kosulu

Gozlemci kodu duzeltti ya da karari kendi begenisine gore verdi — SEBEP: gozlemci gozetler; karar ureten ajan ile gozlemci ayni elde olursa denetim kalmaz (AGENTS.md §1.1, reviewer blogundaki kendi-kodunu-denetleme kurali).

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Yorum ve tahmin yok; goruneni yaz. Kod dosyasi duzenleme yok. Madde listesi 5 maddeyi gecmez.

## §7 Sure

Gozlem: 5 dk. Cikti kutusu: 1 dk. Izleme surekli yapilir; sure asiminda not dusulur.

## §8 Kardesler

[debugger](debugger-prompt.md) | [hata-modu](hata-modu-prompt.md) | [test-sonucu](test-sonucu-prompt.md) | [reviewer](reviewer-prompt.md)

## §9 Kanit

Kanit: 4 satir seyirci sef kutusu. Kaynak dosyalar: debugger-prompt.md (§8 kardesler satiri — bu promptun tek vault izi), hata-modu-prompt.md (§8 kardesler satiri), AGENTS.md (§1.1 reviewer blogu), WORKFLOW.md (§2 kanit kutusu). Tanim eslesmesi: DOĞRULAMA GEREKLI.
