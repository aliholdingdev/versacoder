---
title: "deployment-rollback-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Dagitim ve geri alma oncesi 12 kanitli soru: paket, veri, rollback adimi, onay, dogrulama"
---

# deployment-rollback-check — Dagitim + Geri Alma Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu surum geri alinabilir mi, geri alma adimi yazili ve denendi mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Rollback yazilmamis dagitim = dagitim degildir.** Her surum icin geri alma adimi (hangi dosya/surum, kac dakika) once yazilir, sonra dagitilir; geri alinamaz migration **G11 ihlalidir** ve onay ister. Kanit = yazili rollback plani + dogrulama ciktisi + onay kaydi.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (plan) · `reviewer` (denetim) |
| Onay | Yikici/geri alinamaz islem → **kullanici** (G7/G11) |
| Kanit paketi | rollback plani + build/test ciktisi + onay satiri |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Geri alma (rollback) adimi **yazili** mi (dosya yolu + sure)?
  **Kanit:** plan satiri: `geri alma: <ne> · <kac dakika>`.
- [ ] 2. Geri alma **denendi** mi (kuru test ya da gerekce)?
  **Kanit:** deneme ciktisi ya da `NEEDS_VALIDATION` + gerekce.
- [ ] 3. Dagitim oncesi build **iki bacakta** 0 hata mi?
  **Kanit:** iki `dotnet build` ciktisi.
- [ ] 4. Dagitim oncesi test **tumu yesil** mi?
  **Kanit:** `dotnet test` ozet satiri.
- [ ] 5. Paket/surum bilgisi biliniyor mu (hangi commit → hangi dosya)?
  **Kanit:** `git log -1 --format=%H` (hash) + paket cikti adi.
- [ ] 6. Migration var mi; varsa **geri alinabilir** mi (geri alinamaz ise onay)?
  **Kanit:** migration dosya listesi + rollback notu + `G7 ONAYI` satiri.
- [ ] 7. Yedek alindi mi (veri degisikligi oncesi)?
  **Kanit:** yedek dosya yolu + olusturma tarihi ya da "VERI DEGISIKLIGI YOK".
- [ ] 8. Yapilandirma/secret ortam gecisinde kıriliyor mu (ortam anahtarlari listesi)?
  **Kanit:** yapilandirma anahtarlari listesi + eksik = 0.
- [ ] 9. Geri alma sirasinda **veri semasi** ne olur (eski kod yeni semayla calisir mi)?
  **Kanit:** kisa analiz satiri (evet/hayir + gerekce).
- [ ] 10. Dagitim sonrasi **dogrulama adimi** var mi (saglik kontrolu)?
  **Kanit:** calistirilan dogrulama komutu + beklenen cikti.
- [ ] 11. Yikici isleme **onay alindi** mi (G7/G11)?
  **Kanit:** `log.md` onay satiri (tarih · islem · veren).
- [ ] 12. Geri alma karari **kime** ait, ne zaman verilir (kritere bagli mi)?
  **Kanit:** planda 1 satirlik karar kriteri + sorumlu rol.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Rollback yok/uygulanmamis → **dagitim yasak** (G11) |
| 2 | Soru 6 | Geri alinamaz migration onaysiz → **G7 CRITICAL** |
| 3 | Soru 3 veya 4 | Build/test kirmizi → dagitim durdurulur |
| 4 | Soru 7 | Veri degisikligi yedeksiz → **CRITICAL**, islem baslamaz |
| 5 | Soru 11 | Onaysiz yikici islem → islem durdurulur, raporlanir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Teslim oncesi 5 maddelik kanit paketi | [release-readiness.md](release-readiness.md) |
| Migration dosyasinin kendisi (SQL bicimi) | [migration-check.md](migration-check.md) |
| Sifre/anahtar dagitimi ve sizintisi | [secret-leak-check.md](secret-leak-check.md) |
| Commit ve ilk commit kurallari | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) |
| Onay akisi ve gate sirasi | [../WORKFLOW.md](../WORKFLOW.md) §7 · §8 |
