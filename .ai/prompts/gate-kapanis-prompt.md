---
title: "Gate Kapanis Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Gate kapanisinda 4 satirlik kanit kutusunun degerlendirmesi ve kapat/RED karari icin prompt"
---

# Gate Kapanis Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [session-kapanis](session-kapanis-prompt.md) | [docs](docs-prompt.md) | [TODOS](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Gate kapanisinda 4 satirlik kanit kutusunun dolup dolmadigini degerlendirir; kapat veya RED kararinin tek cevabidir.


## §0 Ozet (Answer First) - Amaç ve kaynak eslestirme

Bu prompt, bir gate'in kapanis kararini kanit kutusu ile birlikte uretir. Kaynak
`WORKFLOW.md` §2 "Gate kanit kutusu" ve `../prompts/docs-prompt.md` dosyalaridir.
Kapanis karari kanitsiz verilemez; kanit kutusu bos ise gate acik kalir.

## 1. Girdi kosullari

- Gatein adi ve numarasi (ornegin gate-004a-vault-kapanis).
- Gatein kapanis kriterleri, madde madde.
- Her kriter icin eldeki kanit: dosya yolu, olcum degeri, komut ciktisi.

## 2. Cikti sozlesmesi

ASCII kutu biciminde tek bir kapanis kaydi:

    +--------------------+------------------------------------------+
    | BASLIK            | Gate adi ve numarasi                     |
    | KAPSAM            | Kapsanan kriterler                       |
    | KARAR             | KAPANDI / ACIK / ERTELANDI               |
    | RISK              | Kalan acik noktalar                      |
    | KANIT             | Dosya yolu ve olcum degeri               |
    +--------------------+------------------------------------------+

## 3. Adim adim yurutme

1. Kapanis kriterlerini listele ve her birini isaretle: GECTI / KALDI.
2. GECTI icin kanit satiri yaz: dosya yolu + olcum degeri.
3. KALDI icin kalan is tanimini yaz ve kriteri ACIK isaretle.
4. Tum kriterler GECTI ise KARAR alanini KAPANDI yap.
5. Tek bir kriter bile kanitsiz ise KARAR asla KAPANDI olamaz.

## 4. Kanit kurallari

- Kanit, olculmus bir degerdir: "0 bozuk link", "30 dosya", "458 satir".
- "Kontrol edildi", "sorunsuz", "uygun" ifadeleri kanit sayilmaz.
- Kanit uydurmak yasaktir; elde olmayan deger icin DOĞRULAMA GEREKLI yaz.

## 5. Baglanti haritasi

- Kapanis kutusu ornegi icin `../prompts/docs-prompt.md`.
- Gate listesi icin `../checklists/gate-004a-vault-kapanis.md`.
- Otomatik olcum icin `../scripts/measure-vault.ps1`.

## 6. Hata modlari

- Eksik kanit: KARAR = ACIK, RISK alanina eksik kriter yazilir.
- Celiskili kanit: iki deger de KANIT alanina yazilir, KARAR = ERTELANDI.
- Dosya yok: kanit satirina dosya yolu yazilmaz, DOĞRULAMA GEREKLI notu dusulur.

## 7. Cikti sonrasi kontrol

Kutu disinda tek satir ozet: gate adi, karar sayisi, kalan acik kriter sayisi.

## 8. Yasaklar

Emoji, Unicode kutu karakterleri, mojibake, kod parcasi, bes maddeden uzun liste.

## 9. Kanit

Uretim dosyasi: `../prompts/gate-kapanis-prompt.md`. Kaynak eslestirme satiri:
`../TODOS/index.md` icindeki gate maddesi. Olcum: bu dosya 2600-8500 bayt araliginda.