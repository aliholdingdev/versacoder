---
title: "localization-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Yerellestirme denetimi icin 12 kanitli soru: resx kaynagi, kod/sabit metin, kulturel format, dil anahtari"
---

# localization-check — Yerellestirme (i18n) Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Gorunur metinlerin tumu resx'ten mi geliyor, tarih/sayi formati kulturel mi, yeni dil eklenebilir mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kural: **ekranda gorunen metin kodun icinde yazilmaz** — `.resx` (ya da tek bir metin deposu) uzerinden gelir; aksi halde yeni dil eklemek tum `.cs` dosyalarini kirar. Tarih/sayi formatlari `CultureInfo` ile gecilir, sabit `"dd.MM.yyyy"` yazilmaz. Kanit = grep ciktisi + 1 resx anahtari ornegi.

| Alan | Deger |
|---|---|
| Kim doldurur | `ui-designer` (resx) · `developer` (kod) |
| Kanit paketi | 3 grep ciktisi + 1 resx satiri |
| Depo | `.Form.resx` / ortak metin deposu |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Gorunur metinler `.resx`'ten mi geliyor (kodda sabit `Text = "..."` sayisi)?
  **Kanit:** `Select-String -Path src\**\*.Form.cs -Pattern 'Text = "[^"]{3,}"'` = 0 satir (onayli istisnalar hariç).
- [ ] 2. Her resx anahtari **en az 1 yerde** kullaniliyor mu (olum anahtar 0)?
  **Kanit:** resx `data name` listesi ile `ResourceManager`/`GetString` cagrilari karsilastirmasi (fark = 0).
- [ ] 3. Her kullanilan anahtar **resx'te var** mu (eksik anahtar 0)?
  **Kanit:** ters karsilastirma da 0 (2 rakam).
- [ ] 4. Tarih/sayi bicimleri `CultureInfo` ile mi (sabit `"dd.MM.yyyy"` / `ToString("N2")` yasak)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"dd\.|"#,##'` = 0 satir + `CultureInfo` kullanim ornegi.
- [ ] 5. Kullanici dili ayari bir noktadan mi yonetiliyor (dagitilmis sabit 0)?
  **Kanit:** tek ayar dosyasi/property yolu (dosya:satır).
- [ ] 6. `NeutralResourcesLanguage` / kultur atamasi tanimli mi?
  **Kanit:** ilgili attribute/ayar satiri ya da "YOK (planla)".
- [ ] 7. Yeni dil eklemek icin **tek dosya yeterli** mi (kod degisikligi 0)?
  **Kanit:** eklenecek dosya yolu + "kod degismez" notu.
- [ ] 8. Buton metinleri uzadiginda tasmıyor mu (dinamik boyut/otomatik sarma)?
  **Kanit:** en uzun anahtarin karakter sayisi + kontrolun boyut ayari (1 satir).
- [ ] 9. Hata mesajlari da kaynakli mi (kodda sabit mesaj 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'throw new .*\("[A-Z][a-z]{4,} '`` = 0 satir ya da 1 istisna notu.
- [ ] 10. Tooltip / placeholder metinleri de resx'te mi?
  **Kanit:** tooltip atamasi ornegi (dosya:satır).
- [ ] 11. Türkçe'ye ozel karakterler (ş, ğ, İ, ı) UTF-8 mi, bozulma yok mu?
  **Kanit:** `powershell -File .ai\scripts\find-mojibake.ps1` ciktisi = `Mojibake file count: 0`.
- [ ] 12. Yerellestirme testi var mi (anahtar eslesme testi)?
  **Kanit:** test adi + beklenti (eksik anahtar = fail) ya da `NEEDS_VALIDATION`.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 3 | Sabit metin / eksik anahtar → metin deposu kirik, RED |
| 2 | Soru 4 | Sabit tarih/sayi bicimi → kulturel hata riski, HIGH |
| 3 | Soru 7 | Yeni dil icin kod degistirmek gerekiyorsa mimari yetersiz |
| 4 | Soru 11 | Mojibake > 0 → dosyalar UTF-8 yeniden yazilir (G4) |
| 5 | Soru 5 | Dagitilmis dil ayari → tutarsiz ekranlar, MEDIUM |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Erisilebilirlik (klavye, odak, kontrast) | [accessibility-check.md](accessibility-check.md) |
| Form/Designer/resx ucyusu ve `ResourceManager` | [winforms-designer-check.md](winforms-designer-check.md) |
| Vault MD'lerinin Turkce/UTF-8 bicimi | [documentation-check.md](documentation-check.md) · [vault-link-check.md](vault-link-check.md) |
| Ribbon buton/metin yapisi | [ribbon-style-check.md](ribbon-style-check.md) |
| Dosya adlandirma ve kod standartlari | [../CLAUDE.md](../CLAUDE.md) §5 |
