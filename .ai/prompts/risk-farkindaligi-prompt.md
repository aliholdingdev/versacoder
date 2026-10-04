---
title: "Risk Farkindaligi Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Erken asama risk kesfi ve onceliklendirme (risk-degerlendirme on adimi) icin prompt"
---

# Risk Farkindaligi Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [VISION.md](../VISION.md) | [risk-degerlendirme](risk-degerlendirme-prompt.md) | [panel-oylama](panel-oylama-prompt.md) | [TODOS](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Erken asamada risk kesfi yapip onceliklendirir; risk-degerlendirme promptuna girecek girdinin tek cevabidir.


## §0 Ozet (Answer First) - Amaç ve kaynak eslestirme

Bu prompt, bir isin risklerini en bastan gorunur kilar. Kaynak
`risk-degerlendirme-prompt.md` ve `../prompts/risk-degerlendirme-prompt.md`
dosyalaridir. Amaç, riski is bittikten sonra degil, baslarken yazdirmaktir.

## 1. Girdi kosullari

- Yapilacak is, tek cumleyle.
- Bilinen kisitlar: sure, izin, bagimlilik, veri erisimi.
- Kime zarar var: kullanici, uretim verisi, gecmis kararlar.

## 2. Cikti sozlesmesi

Risk listesi tablosu, en fazla bes satir:

    +----------------+----------+----------+------------------------+
    | RISK           | OLASILIK | ETKI     | ONLEM                  |
    +----------------+----------+----------+------------------------+

Her satir tek bir gercek risktir; belirsizlik "risk" adina girmez, ayrica
belirsizlik satiri olarak yazilir.

## 3. Adim adim yurutme

1. Isi tek cumleyle tanimla; kapsam disi her seyi ayri satira yaz.
2. Her adim icin sor: burada ne bozulabilir?
3. Bozulma ihtimalini OLASILIK olarak dusuk/orta/yuksek isaretle.
4. Bozulma buyuklugunu ETKI olarak dusuk/orta/yuksek isaretle.
5. ONLEM kolayca yapilabilir olmali; yapilamiyor ise o risk RISK satirina yukselir.

## 4. Kanit kurallari

- Oncelik = OLASILIK x ETKI; ikisi de yuksek olan ilk siraya yazilir.
- Onlemi olmayan risk silinmez, "onlem yok" notu ile listede kalir.
- Gecmis olay kaniti varsa KANIT satirina dosya yolu yazilir.

## 5. Baglanti haritasi

- Tam rehber: `risk-degerlendirme-prompt.md`.
- Prompt kardesi: `../prompts/risk-degerlendirme-prompt.md`.
- Karar kaydi ornegi: `../prompts/gate-kapanis-prompt.md`.

## 6. Hata modlari

- Bes satir asildi: en dusuk ETKILI iki satir tek satirda birlestirilir.
- Bilgi yok: ilgili hucreye DOĞRULAMA GEREKLI yazilir, uydurulmaz.
- Risk listesi bos birakilamaz: "bilinen risk yok" ifadesi kanit sayilmaz,
  en az bir aday risk yazilir ve belirsizlik olarak etiketlenir.

## 7. Cikti sonrasi kontrol

Tek satir sonuc: toplam risk sayisi, yuksek oncelikli risk sayisi.

## 8. Yasaklar

Emoji, Unicode kutu karakterleri, mojibake, kod parcasi, bes maddeden uzun liste.

## 9. Kanit

Uretim dosyasi: `../prompts/risk-farkindaligi-prompt.md`. Kaynak eslestirme satiri:
`risk-degerlendirme-prompt.md`. Olcum: bu dosya 2600-8500 bayt araliginda.

## 10. Dolu ornek (kisa kayit)

    +----------------+----------+----------+------------------------+
    | RISK           | OLASILIK | ETKI     | ONLEM                  |
    +----------------+----------+----------+------------------------+
    | link kirilmasi | orta     | yuksek   | link taramasi her      |
    |                |          |          | is kapanisinda kostur  |
    +----------------+----------+----------+------------------------+

Ornek satiri tek bir riski gosterir: onlem costurulabilir (script zaten
`scripts/check-links.ps1` zaten vault'ta vardir), bu yuzden satir gecerlidir. Tablo bes
satiri asarsa en dusuk ETKILI iki satir birlestirilir; ornek bu kuralin
disinda kalmaz.