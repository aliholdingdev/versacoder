---
title: "gate-004-ilk-commit - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GATE-004 (Build Green) ile birlikte atilacak ILK commit oncesi 13 kanitli soru: iki bacak build, commit bicimi, dosya bazli staging"
---

# gate-004-ilk-commit — GATE-004 Build Green + Ilk Commit Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Depo ilk kez mi commit ediliyor, build iki bacakta 0 hata mi ve staging yalnizca kasitli dosyalari mi kapsiyor* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Depoda (2026-09-25) **commit yok**; ilk commit yalnizca GATE-004 kapandiktan sonra atilir — yarim vault asla commit edilmez (WORKFLOW §7.2 #1). Listeyi `developer` hazirlar, `reviewer` kanit kontrolu yapar, kapanisi **kullanici** verir. Kanit = `dotnet build` ciktisi + `git log -1` + `git show --stat`.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (kanit) · `reviewer` (denetim) |
| Kim kapatir | **kullanici** |
| Kanit paketi | `dotnet build` · `git log -1` · `git show --stat` |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [x] 1. `dotnet build` **0 hata** mi?
  **Kanit:** `dotnet build -v m` ciktisinin son satiri (`0 Hata`) — proje adiyla birlikte alinti.
  **Cikti (2026-10-01):** `Oluşturma başarılı oldu. / 0 Uyarı / 0 Hata` — 11 proje,
  2.50 sn (`VersaCoder.Cli.exe`, `VersaCoder.Desktop.exe` + 9 dll).
- [x] 2. **Iki bacak** da ayri ayri derleniyor mu (`net48` + `net10.0-windows`)?
  **Kanit:** `dotnet build -f net48` ve `dotnet build -f net10.0-windows` ciktilarinin ikisi de `0 Hata`.
  **Cikti (2026-10-01):** `-f net48` = **0 Hata / 2 Uyarı**; `-f net10.0-windows` = **0 Hata / 2 Uyarı**
  (2 uyari da `DX1001` lisans — bkz. §2.1).
- [x] 3. Hata `NoWarn` ile gizlenmis mi (0 olmali)?
  **Kanit:** `Directory.Build.props` + `*.csproj` icinde `<NoWarn>` aramasinin ciktisi; varsa satir adi.
  **Cikti (2026-10-01):** `Select-String 'NoWarn|TreatWarningsAsErrors'` (props + 11 csproj) = **0 satir**.
- [x] 4. Ilk commit, GATE-004 kapanis **sonrasi** mi atildi?
  **Kanit:** `git log --reverse --oneline` ilk satiri + tarih karsilastirmasi (build ciktisi tarihi ≤ commit tarihi).
  **Cikti (2026-10-01):** `git log --reverse` ilk satiri = `4ccaa44` (ilk ve tek commit, 23:21:52);
  build olcumu ayni oturumda oncesinde alindi (§1/1-2 ciktisi).
- [x] 5. Commit message ilk satiri `part:` / `gate:` / `iskelet:` / `test:` on ekli mi?
  **Kanit:** `git log -1 --format=%s` ciktisi.
  **Cikti (2026-10-01):** `gate: iskelet + iki bacak build (ilk commit)`.
- [x] 6. Ilk satir **≤ 72 karakter** ve kucuk harf mi?
  **Kanit:** `git log -1 --format=%s | Measure-Object -Character` sonucu (karakter sayisi rakamla).
  **Cikti (2026-10-01):** **44 karakter** (<= 72); kucuk harf + Türkçe karakter ı (buyuk harf yok).
- [x] 7. Commit govdesinde `KANIT:` satiri var mi?
  **Kanit:** `git log -1 --format=%B` ciktisi — `KANIT: dotnet build -> 0 hata (net48 + net10.0-windows)`.
  **Cikti (2026-10-01):** `KANIT: dotnet build -> 0 hata (net48 + net10.0-windows); 11 proje; uyari = DX1001 ...`
  (govde `--amend` ile eklendi; log.md satir 34 kaydi).
- [x] 8. Staging **dosya bazli** mi yapildi (`git add <yol>`, `git add -A` degil)?
  **Kanit:** uygulanan komut gecmisi + `git show --name-only --format=` listesi.
  **Cikti (2026-10-01):** 13 ayri `git add <yol>` (`.ai .opencode src tests` + 9 kok dosya);
  `git add -A` / `git add .` **0 kez**; `git show --name-only` = 544 dosya.
- [x] 9. Kapsam disi dosya stage edilmemis mi (`*.old.*`, `C:\www\` kopyasi, `.obsidian` gurultusu)?
  **Kanit:** `git show --name-only` ciktisinda bu desenlerin 0 eslesmesi.
  **Cikti (2026-10-01):** `\.old\.|obsidian|www\` taramasi = **0**; `node_modules/`, `bin/`, `obj/` `.gitignore` + 0 eslesme.
- [x] 10. Commit oncesi link taramasi **0 bozuk** miydi?
  **Kanit:** `powershell -File .ai\scripts\check-links.ps1` ciktisi (`BOZUK link : 0`).
  **Cikti (2026-10-01):** `Taranan MD 506 · Iyi link 5800 · BOZUK link 0`.
- [x] 11. Yasak madde taramasi 0 mu (EF Core · v26.1 disi · string-concat SQL)?
  **Kanit:** CLAUDE.md §10 G2 + G3 uc komutunun da `0 satir` veren ciktisi.
  **Cikti (2026-10-01):** `EntityFramework|EntityFrameworkCore` = **0** · `Directory.Packages.props` v26.1-disi surum =
  **0** · concat-SQL deseni = **0**.
- [x] 12. Yikici git islemi (`reset --hard`, `push --force`) yapildi mi (yapilmamali)?
  **Kanit:** komut gecmisi — yapildiysa `G7 ONAYI` satiri; yoksa "YOK".
  **Cikti (2026-10-01):** **YOK** — tek git yazma islemi `add` + `commit` + `commit --amend`
  (yerel, 1 dk onceki ilk commit, uzak depo yok; G7 yikici listesi disi).
- [x] 13. `log.md`'ye GATE-004 + ilk commit kaydi dustu mu?
  **Kanit:** `log.md` son kayit satiri (tarih · commit hash · build sayisi).
  **Cikti (2026-10-01):** `log.md` **satir 34** — 2026-10-01 · `4ccaa44` · 11 proje / 0 hata / 544 dosya.

### §2.1 Bilinen uyari (RED degil)

```text
DX1001 (2 uyari/bacak): DevExpress lisans anahtari yok - "For evaluation purposes
only. Redistribution prohibited." Build 0 hatada kalir; cozum kullanici islemi:
 DevExpress_License.txt -> %AppData%\DevExpress\ (devexpress.com/DX1001).
 Kapanis: kullanici - bu not GATE-004 kapanisinda gorulur.
```

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Tek bacak kirmizi / `> 0` hata → GATE-004 **acik**, commit atilmaz (G6) |
| 2 | Soru 4 | Build oncesi commit atilmisa yarim vault → commit geri alinir (onay ile, G7) |
| 3 | Soru 9 | Kapsam disi dosya stage edilmis → `G1`/`G7` ihlali, commit iptal |
| 4 | Soru 11 | Yasak madde satiri > 0 → **G2/G3 CRITICAL**, kod RED |
| 5 | Soru 8 | `git add -A` kullanilmis → staging tekrar dosya bazli yapilir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Vault MD bicimi (frontmatter · link · mojibake) | [gate-004a-vault-kapanis.md](gate-004a-vault-kapanis.md) |
| Testlerin yeşil olmasi ve coverage | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| PR acma, inceleme ve birlestirme akisi | [pr-pre-check.md](pr-pre-check.md) |
| Branch adlandirma ve release adimi | [release-readiness.md](release-readiness.md) · [deployment-rollback-check.md](deployment-rollback-check.md) |
| Sifirdan commit/git kurallarinin tami | [../WORKFLOW.md](../WORKFLOW.md) §7 |
