---
title: "Test Sonucu Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Test ciktisinin 4 satirlik raporlanmasi icin prompt"
---

# Test Sonucu Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [tester](tester-prompt.md) | [debugger](debugger-prompt.md)

> **Tek cumlelik sorumluluk:** Test ciktisini 4 satirlik sayisal rapor olarak ver; "gecti" yazimi degil, sayi ver.

## §0 Ozet (Answer First)

Test sonucu **sayiyla** raporlanir: kac test, kac gecti, kac kaldi, hangi komutla alindi. AGENTS.md §1 matrisinde test / kapsam isinin kaniti `dotnet test ciktisi`dir; bos "testler gecti" cumlesi kanit sayilmaz (§2.1 madde 3). Kalan test > 0 ise sonuc KIRMIZI ve [debugger](debugger-prompt.md)'a devredilir; hepsi gectiyse sonraki gate kaniti olarak [gate-kapanis](gate-kapanis-prompt.md)'a tasinir.

| Satir | Icerik | Ornek bicimi |
|---|---|---|
| 1 KARAR | GECTI / KALDI | GECTI: 42/42 |
| 2 KOMUT | test komutu | dotnet test -f net48 |
| 3 KONUM | kalan test varsa dosya | tests/.../FooTests.cs |
| 4 SONRAKI | devam ya da devir | gate kaniti / debugger |

## §1 Girdi

- Test komutunun tam ciktisi (kesilmemis)
- [tester](tester-prompt.md) — kapsam ve coverage beklentisi
- [WORKFLOW.md](../WORKFLOW.md) — hangi gate'in kaniti oldugu

## §2 Prompt

```text
Test sonucu raporlaniyor: <komut>.
1. Sayilari cikar: toplam / gecti / kaldi.
2. Kaldi > 0 ise KIRMIZI; dosya ve test adini yaz.
3. Komutu ve TFM'yi aynen yaz (net48 / net10.0-windows).
4. Sonucu 4 satir kutuya yaz; "gecti" cumlesi tek basina gecersiz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : GECTI (<n>/<n>) | KALDI (<n>)
KOMUT     : <test komutu + TFM>
KONUM     : <dosya / test adi> | yok
KANIT     : sayisal cikti (gecen / kaldi)
------------------------------------------------------------
```

## §4 Kabul

- 4 satir dolu; 1. satirda toplam ve gecen sayi var
- Kalan test varsa dosya adi yazildi
- Komut aynen yazildi; TFM belirsizse "belirsiz" notu dusuldu

## §5 RED Kosulu

Kutuda sayi yok, yalniz "testler gecti" yazildi — SEBEP: AGENTS.md §2.1 madde 3; somut olmayan kanit gate'de islenmez.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Sayi uydurma; ciktida olmayan deger yazma. Testi pas gecme (skip) kanit sayilmaz.

## §7 Sure

Rapor: 2 dk. Kalan test varsa [debugger](debugger-prompt.md) ayiklamasi 15 dk (WORKFLOW.md butcesi).

## §8 Kardesler

[debugger](debugger-prompt.md) | [hata-modu](hata-modu-prompt.md) | [refactor](refactor-prompt.md) | [tester](tester-prompt.md)

## §9 Kanit

Kanit: 4 satir test sonucu kutusu. Kaynak dosyalar: AGENTS.md (§1 matrisi test satiri, §2.1 madde 3), WORKFLOW.md (§2 gate kanit kutusu), tester-prompt.md (kapsam), debugger-prompt.md (kalan test devri).
