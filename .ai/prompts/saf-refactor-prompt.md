---
title: "Saf Refactor Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Refactor oncesi kapsam siniri tespiti ve guvenli refactor kisitlari icin prompt"
---

# Saf Refactor Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [refactor](refactor-prompt.md) | [developer](developer-prompt.md) | [TODOS](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Refactor oncesi kapsam sinirini belirler; guvenli refactor kisitlarinin tek cevabidir.


## §0 Ozet (Answer First) - Amaç ve kaynak eslestirme

Bu prompt, davranisi degistirmeden kodu yeniden duzenleyen refactor isini yonetir.
Kaynak `refactor-prompt.md` dosyasidir; ayni davranis sozlesmesini paylasir, ancak
burada vurgu "saf" kalmaktadir: hicbir test sonucu degismemelidir.

## 1. Girdi kosullari

- Hedef dosya veya metot, tam yolu ile.
- Refactor gerekcesi: ne koklu, neden sikintili.
- Mevcut test sonuclari; test yoksa bu bir RISK olarak yazilir.

## 2. Cikti sozlesmesi

Saf refactor raporu uc bolumden olusur:

- ONCESI: degistirilen dosya ve satir araligi.
- SONRASI: ayni dosyanin yeni hali, ayni davranis.
- KANIT: test sonuclari, onceden ve sonrasi ayni ise GECTI.

## 3. Adim adim yurutme

1. Davranisi kilitle: calisan testleri once kostur ve sonuclari kaydet.
2. Tek bir kucuk adim degistir; belirsiz birlestirme yapma.
3. Testleri tekrar kostur; tek bir sonuc bile degisti ise geri al.
4. KANIT satirina oncesi ve sonrasi test sayilarini ayni yaz.
5. Refactor ile ozellik ekleme; ozellik lazim ise ayri is baslat.

## 4. Kanit kurallari

- Davranis degisikligi tespit edilirse is DURUR, rapor "SAF DEGIL" der.
- Test yoksa kanit yerine DOĞRULAMA GEREKLI yazilir.
- Dosya boyutu kuculdu ise bu bir gelisimdir, KANIT satirina yazilir.

## 5. Baglanti haritasi

- Ayni konunun tam kalibi: `refactor-prompt.md`.
- Davranis degisikligi suphesi icin `../prompts/risk-farkindaligi-prompt.md`.
- Kanit kutusu ornegi icin `../prompts/docs-prompt.md`.

## 6. Hata modlari

- Test kirmizi: refactor iptal, onceki hali geri yuklenir.
- Test yok: KANIT = DOĞRULAMA GEREKLI, karar ERTELANDI.
- Cok buyuk diff: adim ikiye bolunur, her adim ayri kanitla gecer.

## 7. Cikti sonrasi kontrol

Tek satir sonuc: dosya, adim sayisi, test sonucu degisimi (0 olmali).

## 8. Yasaklar

Emoji, Unicode kutu karakterleri, mojibake, kod parcasi, bes maddeden uzun liste.

## 9. Kanit

Uretim dosyasi: `../prompts/saf-refactor-prompt.md`. Kaynak eslestirme satiri:
`refactor-prompt.md`. Olcum: bu dosya 2600-8500 bayt araliginda, bozuk link yok.

## 10. Dolu ornek (kisa kayit)

    +--------------------+------------------------------------------+
    | BASLIK            | SessionStore okuma yolu sadelestirildi   |
    | KAPSAM            | src/Store/SessionStore.cs : 42 -> 51      |
    | KARAR             | Saf - ayni test sonuclari korundu         |
    | RISK              | Test yok; kanit DOGRLAMA GEREKLI         |
    | KANIT             | dotnet test -> 12 gecti, 0 kaldi         |
    +--------------------+------------------------------------------+

Ornek, oncesi/sonrasi test sayisinin ayni kaldigini gosterir; KARAR alani
"SAF DEGIL" cikarsa is geri alinir. Ornek satirlari bu dosyanin kendi
bicim referansidir, baska bir dosyaya link degildir.