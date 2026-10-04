---
title: "accessibility-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "WinForms/DevExpress erisilebilirligi icin 13 kanitli soru: klavye, odak, kontrast, etiket, screen reader"
---

# accessibility-check — Erisilebilirlik (WinForms) Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Tum akis klavye ile yuruyebiliyor mu, odak gorunur mu, etiket ve kontrast yeterli mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault **masaustu (WinForms + DevExpress)** uygulamasi oldugundan erisilebilirlik kaniti **klavye + odak + etiket** uzerinden alinir; web WCAG tarayici araclari gecerli degildir. Kanit = tek tek test edilmis davranis (klavye adimi) ya da `NEEDS_VALIDATION`. Bulgu `reviewer` tarafindan seviyeli yazilir.

| Alan | Deger |
|---|---|
| Kim doldurur | `ui-designer` (tasarim) · `reviewer` (denetim) |
| Kanit bicimi | test adimi + beklenen sonuc + durum |
| Arac | klavye (Tab/Enter/Esc) + DevExpress ozellikleri |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Uygulama **hic fare olmadan** tamamlanabiliyor mu (7 ekran icin 1 not)?
  **Kanit:** ekran bazli klavye akis notu (7 satir, her satir `EVET`/`HAYIR`).
- [ ] 2. `Tab` sirasi mantikli mi (gorsel sira ile ayni, geriye sarma var mi)?
  **Kanit:** 1 form icin Tab sirasi listesi + `TabStop`/`TabIndex` satirlari.
- [ ] 3. Odak **gorunur** mi (odak cizgisi/kalirma kapali degil)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'FocusedState|PaintFocused'` ciktisi + odak notu.
- [ ] 4. `Enter`/`Esc` kisayollari tanimli mi (Iptal/Evet butonlari)?
  **Kanit:** `CancelButton`/`AcceptButton` satirlari ya da `KeyDown` handler ornegi.
- [ ] 5. Her input'un **erisilebilir adi** var mi (AccessibleName / etiket)?
  **Kanit:** `Select-String -Path src\**\*.Designer.cs -Pattern 'AccessibleName'` sayisi ile input sayisi karsilastirmasi (2 rakam).
- [ ] 6. Yalniz renge dayali bilgi var mi (renk disi ipucu 0 olmali, orn. hata + ikon)?
  **Kanit:** hata gosterimi ornegi (renk + metin/ikon birlikte).
- [ ] 7. Metin/arkaplan kontrasti yeterli mi (olculmus oran ≥ 4.5:1)?
  **Kanit:** 1 renk cifti icin hesaplanmis oran (rakam + renk kodlari).
- [ ] 8. Font boyutu kucultulebiliyor mu / sabit mi (piksel kilidi notu)?
  **Kanit:** `Font` atama satirlari + DPI/font notu.
- [ ] 9. Hata mesajlari yalniz renkle degil, **metinle** mi veriliyor?
  **Kanit:** hata mesaji gosteren 1 kod blogu (dosya:satır).
- [ ] 10. Uzun metin/tipografi kesiliyor mu (toolip/scroll var mi)?
  **Kanit:** en dar panelin olcumu + `AutoScroll`/`ToolTip` ayari (1 satir).
- [ ] 11. Screen reader icin `AccessibleDescription` dolu mu (bos alan 0)?
  **Kanit:** `AccessibleDescription` dolu/ bos sayilari (2 rakam).
- [ ] 12. Animasyon / otomatik kayan icerik var mi (dikkat dagitani 0 ya da durdurulabilir)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Timer|Animation'` ciktisi + durdurma notu.
- [ ] 13. Bulgular `.ai/reports/**` altina seviyeli yazildi mi?
  **Kanit:** rapor + `[HIGH]/[MEDIUM]/[LOW]` sayilari.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Fare ile yapilabilen is klavye ile yapilamiyorsa → **HIGH** |
| 2 | Soru 5 veya 11 | Erisilebilir ad/tanim yoksa screen reader kör kalir → HIGH |
| 3 | Soru 3 | Odak gorunmuyorsa klavye kullanimi imkansiz → HIGH |
| 4 | Soru 6 veya 9 | Yalniz renge dayali bilgi → **MEDIUM**, metin eklenir |
| 5 | Soru 7 | Kontrast < 4.5:1 → **MEDIUM**, renk paleti guncellenir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Form/Designer/resx ucyusu | [winforms-designer-check.md](winforms-designer-check.md) |
| Ribbon stil ve RibbonForm kalibi | [ribbon-style-check.md](ribbon-style-check.md) |
| GATE-003 wireframe onayi | [gate-003-ui-onay.md](gate-003-ui-onay.md) |
| Localization / metin kaynaklari | [localization-check.md](localization-check.md) |
| Genel kod incelemesi | [code-review-daily.md](code-review-daily.md) · [../spec/test-strategy.md](../spec/test-strategy.md) |
