---
title: "dual-tfm-build-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Iki bacak build denetimi icin 13 kanitli soru: net48 + net10.0-windows, #if siniri, uyari sayaci, platform farklari"
---

# dual-tfm-build-check — Iki Bacak (net48 / net10) Build Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Her iki TFM de ayni kodla 0 hata mi derleniyor, `#if` yalnizca platform farkinda mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Tek bacak dogrulama yok sayilir** (G6): yalniz `net10.0-windows` gecti diye GATE-004 kapandi denemez. Her iki build ayri ayri calistirilir, ayri ayri ciktisi rapora yapistirilir. `#if` icinde **is mantigi** yazmak yasaktir; cozum ortak `interface`'tir. Kanit = iki ayri build ciktisi + `#if` taramasi.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (build) · `reviewer` (dogrulama) |
| Kanit paketi | 2 build ciktisi + 1 `#if` taramasi |
| TFM | `net48` · `net10.0-windows` |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. `dotnet build -f net48` **0 hata** mi?
  **Kanit:** ciktinin son satiri (`0 Hata`) — proje adiyla.
- [ ] 2. `dotnet build -f net10.0-windows` **0 hata** mi?
  **Kanit:** ayni bicimde ikinci cikti.
- [ ] 3. Her iki `.csproj`/proje gercekten **iki TFM** mi tanimliyor?
  **Kanit:** `Select-String -Path **\*.csproj -Pattern 'TargetFramework'` ciktisi (her projede 2 deger).
- [ ] 4. `#if` kullanimi **0** mi, varsa yalniz platform/API farki mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '#if\s+(NET48|NET10)'` ciktisi + her satirin platform notu.
- [ ] 5. `#if` icinde is mantigi (dongu, kosul, donus degeri) var mi (0 olmali)?
  **Kanit:** satirlarin icerik incelemesi; ornek dosya:satır ya da "0 adim".
- [ ] 6. C# surum farklari ortak kaynagi kiriyor mu (orn. `"""..."""` yalniz net10'da)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"""|\brecord\b|init;'` ciktisi + net48'de karsiligi.
- [ ] 7. Uyarilar gizlenmis mi (`NoWarn` / `#pragma warning disable` = istenmiyor)?
  **Kanit:** `Select-String -Path **\*.csproj,Directory.Build.props -Pattern 'NoWarn|TreatWarningsAsErrors'` ciktisi.
- [ ] 8. Uyari sayisi iki bacakta da **0** mi (ya da yazili gerekce var mi)?
  **Kanit:** build ozetinde `Uyari: 0` iki ciktida da.
- [ ] 9. DevExpress v26.1 her iki bacakta da yukleniyor mu (R5 riski erken dogrulandi mi)?
  **Kanit:** iki build ciktisinda ayni DevExpress referansinin gorunmesi + `RibbonForm` kodunun derlenmesi.
- [ ] 10. Platforma bagli `DllImport`/PInvoke yalniz `l4`/`l6` icinde mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'DllImport'` ciktisi + katman yorumu.
- [ ] 11. Testler de **iki bacakta** calisiyor mu?
  **Kanit:** `dotnet test -f net48` ve `-f net10.0-windows` ozet satirlari (ayni gecen sayisi).
- [ ] 12. NuGet paket uyumluluk uyarisi (NU1202/NU1701) var mi (0 olmali)?
  **Kanit:** `dotnet restore` ciktisinda NU1701/NU1202 aramasi = 0 satir.
- [ ] 13. Sonuc `log.md`/rapora iki sayiyla yazildi mi?
  **Kanit:** rapor satiri: `net48: 0 hata · net10.0-windows: 0 hata`.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Herhangi bir bacakta hata → GATE-004 **acik** (G6) |
| 2 | Soru 4 veya 5 | `#if` icinde is mantigi → mimari RED, ortak `interface`'e tasinir |
| 3 | Soru 7 | `NoWarn` ile hata gizlenmis → build gecersiz sayilir |
| 4 | Soru 11 | Tek bacakta test → coverage ve GATE-005 kanitsiz |
| 5 | Soru 9 | DevExpress net48'de yuklenmiyor → **R5 + G8**, kullaniciya rapor |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Asil GATE-004 kapanis karari ve ilk commit | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) |
| Katman bagimlilik yonu | [layer-boundary-check.md](layer-boundary-check.md) |
| Paket surumu ve CPM | [nuget-package-check.md](nuget-package-check.md) · [dependency-update-check.md](dependency-update-check.md) |
| Testlerin icerigi ve coverage | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) · [coverage-gate-check.md](coverage-gate-check.md) |
| Iki bacak kuralinin anayasasi | [../CLAUDE.md](../CLAUDE.md) §1 G6 |
