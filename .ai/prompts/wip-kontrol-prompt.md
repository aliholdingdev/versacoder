---
title: "WIP Kontrol Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Yari is (WIP) tespiti ve WIP varken kapanis RED kurali icin prompt"
---

# WIP Kontrol Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [session-kapanis](session-kapanis-prompt.md) | [butce-kontrol](butce-kontrol-prompt.md)

> **Tek cumlelik sorumluluk:** Yari kalmis is var mi diye kontrol et; WIP varsa kapanisa izin verme ve RED yaz.

## §0 Ozet (Answer First)

WIP = **baslanmis bitmemis is**: acik dosya degisikligi, cevapsiz soru, tamamlanmamis gate kaniti, yarida birakilmas acik madde. [session-kapanis](session-kapanis-prompt.md) kurali net: **WIP varsa kapanis RED**. Bu prompt kapanistan once kosar ve WIP listesini tek satirda toplar; WIP yoksa kapanis yolu acilir, varsa is kapatilmaz.

| WIP turu | Nerede gorulur | Aksiyon |
|---|---|---|
| Acik degisiklik | dosya / diff | bitir ya da geri al |
| Cevapsiz soru | TODOS, oturum notu | cevap bekle, kapatma |
| Eksik gate kaniti | WORKFLOW §2 kutusu | kaniti uret, sonra kapan |

## §1 Girdi

- [session-kapanis](session-kapanis-prompt.md) — kapanis onu WIP sartini saglar
- [TODOS](../TODOS/index.md) — acik maddeler ve aktif faz
- [log.md](../log.md) — son oturum satiri ve kalan is

## §2 Prompt

```text
WIP kontrolu basliyor.
1. Acik degisiklik var mi? (dosya/diff listesi)
2. Cevapsiz soru veya acik madde var mi? (TODOS + oturum notu)
3. Eksik gate kaniti var mi? (WORKFLOW §2 kutusu bos mu)
4. Toplam WIP sayisi: 0 ise DEVAM; 1 ve uzeri ise KAPANIS RED.
4 satir kutuya yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : WIP YOK - KAPANIS ACIK | WIP VAR - KAPANIS RED
WIP       : <adet> kalem (dosya / soru / kanit)
IKTIBAS   : en pahali WIP + sahip (1 satir)
KANIT     : dosya listesi + TODOS + gate kutusu durumu
------------------------------------------------------------
```

## §4 Kabul

- WIP sayisi rakamla yazildi (0 ya da 1+)
- WIP varsa KARAR = RED; kapanis yok
- Her WIP kalemi 1 satir: ne + nerede + kimde

## §5 RED Kosulu

WIP 1 ve uzeri iken kutuya "WIP YOK" yazildi — SEBEP: sayisiz kontrol sahtedir; kapanis eşiği rakamla denetlenir (session-kapanis §0).

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. WIP gizleme / yuvarlama yok. Tahmini liste yok; dosyadan oku. Madde listesi 5 maddeyi gecmez.

## §7 Sure

Kontrol: 3 dk. Cikti kutusu: 1 dk. WIP cikarsa kapanis ertelenir, sure yeniden baslamaz.

## §8 Kardesler

[session-kapanis](session-kapanis-prompt.md) | [panel-oylama](panel-oylama-prompt.md) | [butce-kontrol](butce-kontrol-prompt.md) | [panel-devam](panel-devam-prompt.md)

## §9 Kanit

Kanit: 4 satir WIP kontrol kutusu. Kaynak dosyalar: session-kapanis-prompt.md (WIP RED kurali), TODOS/index.md (acik maddeler), log.md (son oturum), WORKFLOW.md (gate kanit kutusu).
