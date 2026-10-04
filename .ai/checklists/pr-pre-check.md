---
title: "pr-pre-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "PR acilmadan once 14 kanitli soru: build, test, yasak madde, link, boyut, commit bicimi, review paketi"
---

# pr-pre-check — PR Oncesi Hazirlik Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu PR'a acilmadan once hangi kanitlar elde ve hangisi eksikse geri donulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

PR, **kanit paketi hazir degilse acilmaz**: build + test + link + yasak madde + coverage taramasi ekinde gelir. Listeyi `developer` doldurur, `reviewer` devre paketini kontrol eder; 4 maddelik devre (AGENTS.md §4.2) eksikse PR RED'dir. Kapsam disi dosya stage edilirse (G1/G7) PR kapatilir.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (kanit) · `reviewer` (kabul) |
| Kanit paketi | build · test · 3 script · `git show --stat` |
| Devre | `developer` → `reviewer` (4 madde) |
| Soru sayisi | 14 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. `dotnet build` iki bacakta da **0 hata** mi?
  **Kanit:** `dotnet build -f net48` ve `-f net10.0-windows` ciktilari (her ikisi `0 Hata`).
- [ ] 2. `dotnet test` **tumu yesil** ve sayilari raporda mi?
  **Kanit:** `Gecen: N · Basarisiz: 0 · Sure: X sn` ozet satiri.
- [ ] 3. PR'daki her commit'in mesaji `part:`/`gate:`/`fix:` on ekli, ≤ 72 karakter mi?
  **Kanit:** `git log origin/main..HEAD --format=%s` listesi.
- [ ] 4. Her commit'in govdesinde `KANIT:` satiri var mi?
  **Kanit:** `git log origin/main..HEAD --format=%B` ciktisinda KANIT satiri sayisi = commit sayisi.
- [ ] 5. Stage edilen dosyalar yalniz kasitli olanlar mi (`git add <yol>`)?
  **Kanit:** `git diff --cached --name-only` listesi + uygulanan komut gecmisi.
- [ ] 6. Kapsam disi dosya var mi (`*.old.*`, `C:\www\`, `.obsidian/`, kopya vault)?
  **Kanit:** `git diff --cached --name-only` uzerinde bu desenlerin 0 eslesmesi.
- [ ] 7. EF Core referansi **0** mi?
  **Kanit:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` = 0 satir.
- [ ] 8. DevExpress surumu tek satir `26.1` mi, ribbon yalniz `Office2010` mu?
  **Kanit:** CPM satiri alintisi + `Select-String -Path src\**\*.cs -Pattern 'RibbonControlStyle\.\w+'` ciktisi.
- [ ] 9. String-concat SQL **0** mu?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id|Name)'` = 0 satir.
- [ ] 10. Vault link taramasi **0 bozuk** mu (PR dokunuyorsa)?
  **Kanit:** `powershell -File .ai\scripts\check-links.ps1` ciktisi `BOZUK link : 0`.
- [ ] 11. Tek dosya boyutu ve satir sayisi makul mu (≤ 500 satir `.cs`)?
  **Kanit:** `git diff --cached --stat` en buyuk 3 dosyanin satir sayisi.
- [ ] 12. Devre paketi 4 madde dolu mu (NE URETILDI · KANIT · BILINEN EKSIK · SONRAKI ADIM)?
  **Kanit:** PR aciklamasindaki 4 baslik; biri bos ise RED.
- [ ] 13. Bilinen eksikler `NEEDS_VALIDATION` ile mi isaretli (gizlenmis eksik yok mu)?
  **Kanit:** PR aciklamasindaki etiket satirlari; "YOK" yazilmali ya da madde madde.
- [ ] 14. GATE-003 acikken kod degisikligi var mi (0 olmali)?
  **Kanit:** `log.md` GATE-003 onay tarihi ≤ PR'daki ilk kod commit tarihi.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Build/test kirmizi → PR **acilmaz** (GATE-004/005 acik) |
| 2 | Soru 7 veya 9 | EF Core / string-concat satiri > 0 → **G2 CRITICAL** |
| 3 | Soru 6 | Kapsam disi dosya stage'de → G1/G7, PR kapatilir |
| 4 | Soru 12 | Devre paketi eksik → `reviewer` isi geri cevirir (AGENTS.md §4.1) |
| 5 | Soru 14 | GATE-003 onaysiz kod → **G12**, kod silinir, part basa doner |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Kodun kendisi incelemesi (seviyeli bulgu) | [code-review-daily.md](code-review-daily.md) |
| Guvenlik acigi taramasi | [security-review-checklist.md](security-review-checklist.md) · [sql-injection-review.md](sql-injection-review.md) |
| Test dosyasi kalitesi | [unit-test-quality.md](unit-test-quality.md) |
| Branch adlandirma ve release adimi | [release-readiness.md](release-readiness.md) |
| Commit/gate sirasi kurallari | [../WORKFLOW.md](../WORKFLOW.md) §7 · [gate-004-ilk-commit.md](gate-004-ilk-commit.md) |
