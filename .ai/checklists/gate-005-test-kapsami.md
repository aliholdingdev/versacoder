---
title: "gate-005-test-kapsami - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GATE-005 (Test Green) cikis kriteri: yesil dotnet test, iki bacak, coverage %90, NetArchTest 0 ihlal, atlanan test 0"
---

# gate-005-test-kapsami — GATE-005 Test Green + Kapsam Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md) · [kayit defteri](index.md)

> **Tek cumlelik sorumluluk:** *Testler iki bacakta da mi yesil, coverage kanitli mi, atlanan ve kanitsiz kalmis test var mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

GATE-005, "testler geciyor" cumlesiyle **kapansamaz** (CLAUDE.md G5): kanit = sayilarla dolu `dotnet test` ciktisi + coverage raporu. Listeyi `tester` doldurur, `reviewer` son PASS'ini verir, kapanisi **kullanici** yapar. Hedef coverage `spec/test-strategy.md`'deki %90 bandidir; altinda kalindisa kapsam disi madde gerekcelendirilir.

| Alan | Deger |
|---|---|
| Kim doldurur | `tester` · son PASS `reviewer` |
| Kim kapatir | **kullanici** (tester + reviewer kaniti sunar) |
| Kanit paketi | `dotnet test` · coverage raporu · NetArchTest ciktisi |
| Soru sayisi | 14 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. `dotnet test` **tumu yesil** mi (0 basarisiz)?
  **Kanit:** ciktinin ozet satiri: `Gecen: N · Basarisiz: 0 · Sure: X sn`.
- [ ] 2. Testler **iki bacakta** da calisiyor mu?
  **Kanit:** `dotnet test -f net48` ve `dotnet test -f net10.0-windows` ciktilarinin ikisi de ayni sayida gecen testi veriyor.
- [ ] 3. Coverage **≥ %90** mu?
  **Kanit:** coverage raporu yuzdesi (rakamla) + uretim dosyasi adi.
- [ ] 4. Coverage altinda kalan alan **gerekcelendirilmis** mi?
  **Kanit:** kapsam disi maddelerin listesi ve gerekcesi (`spec/test-strategy.md` ile ayni formatta).
- [ ] 5. NetArchTest katman yonu + circular dependency **0 ihlal** mi?
  **Kanit:** mimari test ciktisi: `0 failure`.
- [ ] 6. `Skip` / `[Fact(Skip=...)]` / `Assert.Ignore` sayisi **0** mu?
  **Kanit:** `Select-String -Path tests\**\*.cs -Pattern 'Skip\s*='` ciktisi = 0 satir.
- [ ] 7. Kabul kriterleri ↔ testler eslesme tablosu var mi?
  **Kanit:** `spec/` kriter listesi ile test adlarinin yan yana durdugu tablo; eslesmeyen kriter = 0.
- [ ] 8. Birim testler I/O'suz ve deterministik mi (sabit seed, `DateTime.UtcNow` yerine sahte saat)?
  **Kanit:** en az 1 test dosyasi ornegi + `Random`/`DateTime.Now` kullanan test sayisi = 0 (varsa adi).
- [ ] 9. Entegrasyon testleri **gercek schema** (SQLite) uzerinde mi, EF Core ile mi?
  **Kanit:** test projesi `*.csproj` icinde `Microsoft.Data.Sqlite` referansi; EF Core referansi **0 satir**.
- [ ] 10. Hata yolu + sinir testleri var mi (null, timeout, bozuk input)?
  **Kanit:** bu uc durumu kapsayan en az 3 test adi.
- [ ] 11. `DbTimeouts` / `CommandTimeout` davranisi test edilmis mi?
  **Kanit:** timeout testinin adi ve beklenti satiri.
- [ ] 12. Uretilen test dosyalari `tests/**` altinda mi, uretim koduna girmemis mi?
  **Kanit:** `git show --name-only` icinde `src/` altinda hicbir `*Tests.cs` olmamasi.
- [ ] 13. `reviewer` son PASS raporu var mi?
  **Kanit:** `.ai/reports/**` altinda `0 CRITICAL · 0 HIGH → PASS` satiri.
- [ ] 14. GATE-004 (build) bu testten **once** kapali mi?
  **Kanit:** build cikti tarihi ≤ test cikti tarihi + `log.md` kaydi.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | 1 basarisiz test ya da tek bacak test → GATE-005 **acik** |
| 2 | Soru 3+4 | Coverage < %90 ve yazili gerekce yok → kanitsiz coverage (G5) |
| 3 | Soru 6 | Atlanan test var → RED; atlama gerekcesi yoksa test geri acilir |
| 4 | Soru 9 | EF Core referansi → **G2 CRITICAL**, test altyapisi yeniden yazilir |
| 5 | Soru 13 | `reviewer` PASS'i yok → gate kapatilamaz (WORKFLOW §3.1 #5) |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Testlerin **nasil** yazilacagi (desen, isimlendirme, fixture) | [../spec/test-strategy.md](../spec/test-strategy.md) · [unit-test-quality.md](unit-test-quality.md) |
| Tek tek test dosyasinin assertion kalitesi | [unit-test-quality.md](unit-test-quality.md) |
| Coverage aracinin kurulumu ve yorumlanmasi | [coverage-gate-check.md](coverage-gate-check.md) |
| Build hatasi / iki bacak derleme | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) · [dual-tfm-build-check.md](dual-tfm-build-check.md) |
| Teslim raporu ve yayin onayi | [release-readiness.md](release-readiness.md) |
