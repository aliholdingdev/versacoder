---
title: "release-readiness - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Teslim oncesi 15 kanitli soru: 5 gate kaniti, 4 satirlik kanit paketi, yasak madde taramasi, teslim raporu"
---

# release-readiness — Teslim (GATE-006) Hazirlik Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *GATE-001..005 hepsi kanitli mi kapali, 4 satirlik kanit paketi eksiksiz mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

GATE-006 **5 maddelik kanit paketi** istiyor: build 0 hata · test yesil · link 0 bozuk · yasak madde 0 · `reports/09-teslim-raporu.md` mevcut. Bir madde bile eksikse teslim **yapilamaz** (G5). Listeyi `architect`+`reviewer` birlikte doldurur, kapanisi **kullanici** verir.

| Alan | Deger |
|---|---|
| Kim doldurur | `architect` + `reviewer` |
| Kim kapatir | **kullanici** (GATE-006) |
| Kanit paketi | 4 konsol ciktisi + 1 teslim raporu |
| Soru sayisi | 15 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. GATE-001 ve GATE-002 **kapali** mi (kaynak okuma + kapsam raporu)?
  **Kanit:** `log.md` kapanis kayitlari (2 satir, tarihli).
- [ ] 2. GATE-003 (UI onayi) kullanici imzasiyla **kapali** mı?
  **Kanit:** `log.md` `GATE-003 ONAY` satiri (tarih + veren).
- [ ] 3. GATE-004 (build) iki bacak **0 hata** ile kapali mi?
  **Kanit:** iki `dotnet build` ciktisi (net48 · net10.0-windows).
- [ ] 4. GATE-005 (test) **tumu yesil** ve coverage kanitli mi?
  **Kanit:** `dotnet test` ozeti + coverage yuzdesi.
- [ ] 5. Vault link taramasi **0 bozuk** mu?
  **Kanit:** `powershell -File .ai\scripts\check-links.ps1` → `BOZUK link : 0`.
- [ ] 6. Frontmatter'siz dosya **0** mi?
  **Kanit:** `measure-vault.ps1` → `NoFrontmatter = 0`.
- [ ] 7. Mojibake **0** mi?
  **Kanit:** `find-mojibake.ps1` → `Mojibake file count: 0`.
- [ ] 8. EF Core referansi **0** mu?
  **Kanit:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` = 0 satir.
- [ ] 9. DevExpress surumu tek satir `26.1` ve ribbon yalniz `Office2010` mu?
  **Kanit:** CPM satiri + `RibbonControlStyle\.\w+` tarama ciktisi.
- [ ] 10. String-concat SQL **0** mu?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id|Name)'` = 0 satir.
- [ ] 11. `reports/09-teslim-raporu.md` mevcut ve 4 satirlik kanit paketi dolu mu?
  **Kanit:** rapordaki `build · test · vault · link` 4 satiri (sayilarla).
- [ ] 12. Tum raporlarda `NEEDS_VALIDATION`/`UNKNOWN` maddeleri **acikca** mi yazildi (gizlenmis 0)?
  **Kanit:** `Select-String -Path .ai\**\*.md -Pattern 'NEEDS_VALIDATION|UNKNOWN'` ciktisi + sayi.
- [ ] 13. `log.md`'de kapanis kaydi var mi (tarih + gate + rapor yolu)?
  **Kanit:** `log.md` son 3 satiri.
- [ ] 14. Kapsam disi dosya depoda var mi (`*.old.*`, CoreMusic kopyasi = 0)?
  **Kanit:** `Get-ChildItem C:\versacoder -Recurse -Include *.old.*` = 0 dosya + `git status --porcelain`.
- [ ] 15. Yikici islem/onay kayitlari tamam mi (G7/G11 = aciklama yok)?
  **Kanit:** `log.md` + raporda onay satirlari; aciklama olmayan islem = 0.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1..4 | Herhangi bir gate aciksa GATE-006 **kapansamaz** (sirali gate) |
| 2 | Soru 5 veya 6 | Bozuk link / frontmatter eksigi → **G4 RED** |
| 3 | Soru 8 veya 10 | EF Core / string-concat → **G2 CRITICAL** |
| 4 | Soru 11 | Teslim raporu yok/eksik → teslim yasak (G5) |
| 5 | Soru 12 | Gizlenmis `UNKNOWN` → **G9**, rapor gecersiz |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Vault kapanis script ciktilari (500 MD) | [gate-004a-vault-kapanis.md](gate-004a-vault-kapanis.md) |
| Build/test gate'lerinin ayrintisi | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) · [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| Deployment ve geri alma adimlari | [deployment-rollback-check.md](deployment-rollback-check.md) |
| Gunluk kod incelemesi | [code-review-daily.md](code-review-daily.md) |
| Gate sirasi ve rapor formati | [../WORKFLOW.md](../WORKFLOW.md) §3 · §10 |
