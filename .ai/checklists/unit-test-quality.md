---
title: "unit-test-quality - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Test dosyasi kalitesi icin 15 kanitli soru: assertion, determinizm, izolasyon, isim, tekrar, net48+net10"
---

# unit-test-quality — Birim Test Kalitesi Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md) · [kayit defteri](index.md)

> **Tek cumlelik sorumluluk:** *Yazilan test gercekten davranisi mi olcuyor, deterministik mi, assertion'lari muhtemel mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

"Test var" ile "test iyi" farklidir: bu liste **kaliteyi** olcur, sayiyi degil. Kanitsiz / bos assertion'li / her zaman gecen test **test degildir** ve GATE-005 kanitini kirar. Listeyi `tester` doldurur, `reviewer` dogrular; bulgu bicimi `seviye · dosya:satır · neden · düzeltme`.

| Alan | Deger |
|---|---|
| Kim doldurur | `tester` (yazar) · `reviewer` (denetler) |
| Kaynak | `../spec/test-strategy.md` |
| Kanit paketi | test dosyalari + `dotnet test` ozeti |
| Soru sayisi | 15 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Her testin en az **1 anlamli assertion**'i var mi (bos test 0)?
  **Kanit:** `Select-String -Path tests\**\*.cs -Pattern '\[Fact\]'` sayisi ile `Assert\.` sayisi karsilastirmasi (her dosyada ≥ 1).
- [ ] 2. Assertion **dogru tipte** mi (karsilastirma var, yalniz `Assert.NotNull` ile gecistirilen test 0)?
  **Kanit:** yalniz `NotNull`/`True(true)` iceren test adlarinin listesi (bos olmali).
- [ ] 3. Test adi **ne olcuyor** acikca soyluyor mi (`Method_Condition_Expectation`)?
  **Kanit:** 5 ornek test adi + uyumlu/uyumsuz isareti.
- [ ] 4. Test **deterministik** mi (rastgele seed'li, zamana bagli test 0)?
  **Kanit:** `Select-String -Path tests\**\*.cs -Pattern 'DateTime\.Now|Random\(\)|Guid\.NewGuid'` = 0 satir.
- [ ] 5. Her test kendi verisini kuruyor mu (bir baska testin verisine bagimlilik 0)?
  **Kanit:** fixture/`Initialize` blogu ornegi + paralel calisma ayari.
- [ ] 6. Gercek dosya/AG/ag bagimliligi (I/O) var mi (birim testte 0 olmali)?
  **Kanit:** `Select-String -Path tests\**\*.cs -Pattern 'File\.|HttpClient|new SqliteConnection'` = 0 satir (entegrasyon testleri hariç, ayrica adlandirilmis).
- [ ] 7. Test `catch` icinde assertion **yutuyor** mu (0 olmali — `catch { }`)?
  **Kanit:** `Select-String -Path tests\**\*.cs -Pattern 'catch\s*\{?\s*\}'` = 0 satir.
- [ ] 8. `Skip`/`Ignore` kullanimi **0** mi?
  **Kanit:** `Select-String -Path tests\**\*.cs -Pattern 'Skip\s*=|Ignore'` = 0 satir.
- [ ] 9. Test **sadece** fail edecek sekilde mi yazilmis (mutation'a dayanikli mi)?
  **Kanit:** uretim kodunda 1 deger degistirildiginde testin kirmiziya döndügü notu (ya da `NEEDS_VALIDATION`).
- [ ] 10. Ayni senaryo iki dosyada **tekrar** ediyor mu (copy-paste test 0)?
  **Kanit:** benzer test adlarinin `Group-Object` sonucu (tekrar = 0 ya da birlestirme notu).
- [ ] 11. Test **iki bacakta** da calisiyor mu (`net48` + `net10.0-windows`)?
  **Kanit:** iki `dotnet test -f ...` ozeti (ayni gecen sayisi).
- [ ] 12. NSubstitute mock'lar gercek davranisi **uguylor** mu (olum mock 0)?
  **Kanit:** kullanilan `Substitute.For<...>` sayisi ile gercekten cagrilan mock sayisi; olum mock adi yok.
- [ ] 13. Yeni kod icin test eklenmis mi (diff'te `src` varsa `tests` de var mı)?
  **Kanit:** `git diff --name-only` iki listesinin karsilastirmasi (fark = 0 ya da gerekce).
- [ ] 14. Hata yolu / sinir deger testleri var mi (null, bos, timeout, buyuk girdi)?
  **Kanit:** bu durumlari kapsayan test adlarinin listesi (en az 3).
- [ ] 15. `dotnet test` ciktisi rapora **sayiyla** yazildi mi?
  **Kanit:** `Gecen: N · Basarisiz: 0 · Sure: X sn` satiri (rapora kopyalanmis).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Bos / muhtemel assertion → test gecersiz, GATE-005 kanitsiz |
| 2 | Soru 7 veya 8 | Exception yutan `catch` ya da `Skip` → **RED**, test geri acilir |
| 3 | Soru 4 | Zamana/rastgeleye bagli test → flaky, coverage sayisi gecersiz |
| 4 | Soru 11 | Tek bacakta test → **G6**, coverage gecersiz |
| 5 | Soru 15 | Sayisiz "testler geciyor" → **G5** |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| GATE-005 kapanis karari ve coverage yuzdesi | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| Coverage aracinin ayari ve yorumu | [coverage-gate-check.md](coverage-gate-check.md) |
| Mimari (katman) testleri | [layer-boundary-check.md](layer-boundary-check.md) |
| Kod incelemesi (PASS/RED) | [code-review-daily.md](code-review-daily.md) |
| Test stratejisi ve kapsam tanimi | [../spec/test-strategy.md](../spec/test-strategy.md) |
