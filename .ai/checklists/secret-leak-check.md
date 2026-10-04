---
title: "secret-leak-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Sizinti denetimi icin 12 kanitli soru: repo gecmisi, log, ekran, yedek dosya, config, obfuscation"
---

# secret-leak-check — Secret / Anahtar Sizinti Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Sifre/anahtar ne repo'ya, ne log'a, ne ekrana, ne de yedek dosyaya yazildi mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bir secret **bir kez** disari ciktiysa (commit, log, ekran) silmek yetmez — **anahtar iptal edilir ve yenilenir**. Bu liste yalniz tarama yapar; bulgu kacisinda ilk islem anahtar yenilemedir, kod degisikligi ikincildir. Kanit = calistirilmis tarama ciktisi; `0 satir` yazilmadan madde kapanmaz.

| Alan | Deger |
|---|---|
| Kim doldurur | `reviewer` (tarama) · `developer` (duzeltme) |
| Ilk aksiyon | Kacis varsa **once anahtar iptal**, sonra kod |
| Kanit paketi | `git log -S` · grep ciktilari · `.gitignore` kaniti |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Calisan kod/suzucu deseni var mi (`password|apikey|secret|token` + uzun deger)?
  **Kanit:** `Select-String -Path **\*.cs,*.json,*.config,*.md -Pattern '(password|apikey|secret|token)\s*[:=]\s*"[^"]{8,}'` = 0 satir.
- [ ] 2. Git gecmisinde (`--all`, silinmis commit dahil) secret izi var mi?
  **Kanit:** `git log --all -p -S 'password' --oneline` ciktisi; 0 ise "0 eslesme" yazilir.
- [ ] 3. Connection string duz metin dosyada mi (sifreli/siradan erisim olmali)?
  **Kanit:** dosya yolu + satir; duz metinse kod blogu + duzeltme.
- [ ] 4. `.gitignore` secret dosyalarini kapsiyor mu (`.env`, `*.pfx`, `*.snk`, `*Local.json`)?
  **Kanit:** `.gitignore` satirlari + `git check-ignore -v <ornek dosya>` ciktisi.
- [ ] 5. Gecmiste `.env`/`*.pfx` gibi bir dosya **track edilmis** mi (olmamali)?
  **Kanit:** `git log --all --name-only --diff-filter=A -- '*.env' '*.pfx'` = 0 satir.
- [ ] 6. Log'a secret yaziliyor mu (0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Log\.(Information|Debug|Warning)\([^)]*(password|token|apikey)'` = 0 satir.
- [ ] 7. Exception mesaji / stack trace UI'a ve CLI'ya mi donuyor (iç detay gizlenmeli)?
  **Kanit:** hata yakalama blogu ornegi + kullaniciya donen mesaj ornegi.
- [ ] 8. Secret ekrana (`Console.WriteLine`, status bar, başlik) mi yaziliyor (0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Console\.WriteLine\([^)]*(password|key|token)'` = 0 satir.
- [ ] 9. Yedek/otomatik klasor (`bin`, `obj`, `*.bak`, `*.old.*`) secret iceriyor mu?
  **Kanit:** `Get-ChildItem C:\versacoder -Recurse -Include *.bak,*.old.* | Select-String <desen>` = 0 satir.
- [ ] 10. Vault `.md` dosyalarinda gercek anahtar var mi (0 olmali; ornekler `xxx` olmali)?
  **Kanit:** `Select-String -Path .ai\**\*.md -Pattern '(password|apikey|secret)\s*[:=]\s*"[^"]{12,}'` = 0 satir.
- [ ] 11. `appsettings*.json` / config dosyasi repo'da secret ile mi commit edilmis (0 olmali)?
  **Kanit:** `git ls-files` + ayni desen taramasi = 0 satir.
- [ ] 12. Kacis bulunduysa anahtar **iptal/yenileme** adimi kayitli mi?
  **Kanit:** `log.md` satiri (tarih · hangi anahtar · yenileme durumu) ya da "KACIS YOK".

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 2 veya 5 | Git gecmisinde secret → **CRITICAL**: anahtar iptal + uyari |
| 2 | Soru 1 veya 11 | Duz metin secret kodda/config'te → CRITICAL, kod RED |
| 3 | Soru 6 veya 8 | Secret logda/ekranda → sızıntı, log temizlenir + anahtar yenilenir |
| 4 | Soru 4 | `.gitignore` eksik → commit atilmaz (gate kapanmaz) |
| 5 | Soru 12 | Kacis varsa yenileme kaydi yok → is kapatilmaz (G11) |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| SQL enjeksiyonu ve parametreli sorgu | [sql-injection-review.md](sql-injection-review.md) |
| Genel guvenlik maddeleri (yetki, dosya, ag) | [security-review-checklist.md](security-review-checklist.md) |
| Vault'ta link/bicim sızıntısı | [vault-link-check.md](vault-link-check.md) |
| Bagimlilik CVE denetimi | [dependency-update-check.md](dependency-update-check.md) |
| Secret'in nerede saklanacaginin tasarimi | [../CLAUDE.md](../CLAUDE.md) §4 · [../spec/test-strategy.md](../spec/test-strategy.md) |
