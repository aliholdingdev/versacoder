---
title: "code-review-daily - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Gunluk kod incelemesi icin 15 kanitli soru: seviyeli bulgu, kanit satiri, G1..G12, iki bacak, PASS/RED karari"
---

# code-review-daily — Gunluk Kod Incelemesi Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu diff PASS mi RED mi, bulgular seviye + konum + neden + duzeltme + kanit biciminde mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`reviewer` her gun bu listeyi gecer: **"bence temiz" PASS degildir** (CLAUDE.md G5) — PASS'in kaniti `dotnet build` ya da test log'udur. Bulgu bicimi sabittir: `[SEVIYE] dosya:satır → NEDEN → DuzELTME → KANIT`. Kanit satiri `UNKNOWN` olabilir; o zaman bile yazilmak zorundadir, bos birakilmaz.

| Alan | Deger |
|---|---|
| Kim doldurur | `reviewer` |
| Karar | Tek kelime: **PASS** ya da **RED** (belirsiz PASS yok) |
| Bulgu bicimi | `SEVIYE · dosya:satır · neden · düzeltme · kanit` |
| Soru sayisi | 15 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Diff'in tamami okundu mu (yalniz ilk ekrani degil)?
  **Kanit:** `git diff --stat` satir sayisi ile incelenen satir sayisinin esitligi (rakamla).
- [ ] 2. Her bulgunun **seviyesi** var mi (CRITICAL/HIGH/MEDIUM/LOW)?
  **Kanit:** bulgu listesinde seviye etiketi olmayan satir = 0.
- [ ] 3. Her bulgu `dosya:satır` konumuyla mi yazildi?
  **Kanit:** bulgu listesinde `yol.cs:NNN` bicimi olmayan satir = 0.
- [ ] 4. Her bulgunun `KANIT` satiri var mi (cikti ya da `UNKNOWN`)?
  **Kanit:** bulgu basinda `KANIT` satiri olmayan madde = 0.
- [ ] 5. G1..G12 maddeleri **tek tek** kontrol edildi mi?
  **Kanit:** inceleme raporunda 12 satirlik G madde tablosu (her satirda EVET/HAYIR).
- [ ] 6. EF Core referansi **0** mi?
  **Kanit:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` = 0 satir.
- [ ] 7. Her SQL sorgusu parametreli mi (`@p1` / `@ad`), string-concat **0** mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id|Name)'` = 0 satir + ornek dosya adi.
- [ ] 8. DevExpress yalniz `26.1` ve ribbon yalniz `Office2010` mi?
  **Kanit:** CPM satiri + `RibbonControlStyle\.\w+` tarama ciktisi.
- [ ] 9. **Iki bacak** birlikte incelendi mi (`net48` + `net10.0-windows`)?
  **Kanit:** her iki `dotnet build` ciktisi ya da `#if` bloklarinin platform farki ile sinirli oldugunun kaniti.
- [ ] 10. `#if` icinde **is mantagi** var mi (0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '#if\s+(NET48|NET10)'` ciktisi + satir yorumlari.
- [ ] 11. Katman yonu ihlali var mi (UI → Infrastructure gibi)?
  **Kanit:** NetArchTest ciktisi `0 failure` ya da elle `using` taramasi = 0.
- [ ] 12. Tekrarlanan kod / SSOT ihlali (ayni kural iki dosyada) var mi?
  **Kanit:** tekrar bulgusu yoksa "0", varsa iki dosya yolu.
- [ ] 13. Test acigi var mi (yeni kod icin test eklenmis mi)?
  **Kanit:** `git diff --name-only` icindeki `tests/**` dosyasi sayisi; 0 ise gerekce satiri.
- [ ] 14. Karar **tek kelime** ile yazildi mi (PASS / RED)?
  **Kanit:** raporun son satiri `PASS` ya da `RED` (belirsiz ifade yok).
- [ ] 15. RED ise devre paketi 4 maddeyle `developer`'a geri gonderildi mi?
  **Kanit:** devre metninde 4 baslik + SLA notu (≈10 dk ilk yanit).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 4 | Kanitsiz PASS → **G5 ihlali**, PASS iptal |
| 2 | Soru 6 veya 7 | EF Core / string-concat > 0 → **CRITICAL**, is durur |
| 3 | Soru 9 | Tek bacak incelenmisse → **G6**, inceleme gecersiz |
| 4 | Soru 14 | Belirsiz karar ("genel olarak iyi") → PASS verilemez |
| 5 | Soru 15 | RED devresi eksik → is `developer`'a ulasmaz, tur kaybi |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Test yazimi ve assertion bicimi | [unit-test-quality.md](unit-test-quality.md) |
| Guvenlik derinlemesine tarama | [security-review-checklist.md](security-review-checklist.md) |
| SQL enjeksyonu ozel incelemesi | [sql-injection-review.md](sql-injection-review.md) |
| Katman bagimlilik yonunun mimari testi | [layer-boundary-check.md](layer-boundary-check.md) |
| PR acma ve devre paketi bicimi | [pr-pre-check.md](pr-pre-check.md) · [../CLAUDE.md](../CLAUDE.md) §1 |
