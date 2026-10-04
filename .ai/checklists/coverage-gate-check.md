---
title: "coverage-gate-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Coverage eşiği denetimi icin 12 kanitli soru: %90 bandi, kapsam disi gerekce, filtre, rapor kaydi"
---

# coverage-gate-check — Coverage Esigi (%90) Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md) · [kayit defteri](index.md)

> **Tek cumlelik sorumluluk:** *Coverage rakami gercekten olcum edildi mi, %90 altindaysa yazili gerekce var mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Coverage **olculur**, tahmin edilmez: rakam coverage aracindan gelir, "yaklasik %90" gibi bir cumle **G5 ihlalidir**. Hedef `../spec/test-strategy.md`'deki %90 bandidir; altinda kalindiginda **her eksik dosya icin gerekce** yazilir. Kanit = coverage raporundaki yuzde + rapor dosyasinin yolu.

| Alan | Deger |
|---|---|
| Kim doldurur | `tester` · denetim `reviewer` |
| Hedef | %90 (test-strategy.md bandi) |
| Kanit paketi | coverage raporu + `dotnet test` ozeti |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Coverage raporu **gercekten uretildi mi** (dosya mevcut)?
  **Kanit:** rapor dosya yolu + olusturma tarihi (build tarihi ile ayni gun).
- [ ] 2. Toplam coverage **rakamla** yazildi mi (`%NN`)?
  **Kanit:** rapordan dogrudan alinti: `Line: %NN · Branch: %NN`.
- [ ] 3. Rakam **≥ %90** mu (degilse gerekce var mi)?
  **Kanit:** yuzde + gerekce satiri (ikisi de ayni raporda).
- [ ] 4. Coverage yalniz **uretim kodunu** mu olcuyor (test dosyalari disarida mı)?
  **Kanit:** arac filtresi/ayar dosyasi; `tests/` klasoru coverage disinda.
- [ ] 5. Kapsam disi birakilan dosyalar **ad ad** mi listelenmis?
  **Kanit:** disarida birakilan dosya adlarinin listesi (bos olmali ya da tam liste).
- [ ] 6. %0 coverage alan **yeni** dosya var mi (diff'te eklenen, hic test edilmeyen)?
  **Kanit:** diff'teki yeni `src/**` dosyalari ile %0 alanlarin karsilastirmasi (fark = 0).
- [ ] 7. Kritik yollar (SQL, migration, security) icin coverage **≥ %90** mi (ayri)?
  **Kanit:** ilgili klasorler icin ayri yuzdeler (rakamla).
- [ ] 8. Branch coverage da raporda mi (yalniz line yeterli degil)?
  **Kanit:** `Branch: %NN` satiri.
- [ ] 9. Coverage **iki bacakta** da alindi mi (ya da neden tek bacak aciklandi)?
  **Kanit:** iki komut ciktisi ya da yazili gerekce (`NEEDS_VALIDATION`).
- [ ] 10. Sayılar `dotnet test` ozeti ile tutarlı mı (test sayisi esit mi)?
  **Kanit:** `Gecen: N` + coverage raporundaki test sayisi (2 rakam).
- [ ] 11. Sonuc `.ai/reports/**` altina yazildi mi?
  **Kanit:** rapor dosya yolu + coverage satiri.
- [ ] 12. Coverage **korelasyon** acisindan en dusuk 3 dosya incelendi mi (action var mi)?
  **Kanit:** en dusuk 3 dosya adi + her biri icin 1 satirlik aksiyon.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Rapor yok / rakam yok → **G5**, GATE-005 kapansamaz |
| 2 | Soru 3 | < %90 ve gerekce yok → coverage kanitsiz |
| 3 | Soru 6 | Yeni dosya %0 → yeni kod testsize girmis, RED |
| 4 | Soru 4 | Test dosyasi coverage'i sisiriyorsa rakam gecersiz |
| 5 | Soru 11 | Rapora yazilmayan coverage sonraki gate'te kaybolur (G5) |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| GATE-005'in tam cikis kriteri (test yesil mi) | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| Test dosyasinin kendisi kalitesi | [unit-test-quality.md](unit-test-quality.md) |
| Mimari testler (NetArchTest) | [layer-boundary-check.md](layer-boundary-check.md) |
| Teslim raporundaki 4 satirlik kanit paketi | [release-readiness.md](release-readiness.md) |
| Kapsam/tanım ve hedef bandi | [../spec/test-strategy.md](../spec/test-strategy.md) |
