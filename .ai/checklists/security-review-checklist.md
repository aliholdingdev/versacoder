---
title: "security-review-checklist - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Guvenlik incelemesi icin 14 kanitli soru: secret, yetki, SQL, dosya sistemi, log sizdirma, ag"
---

# security-review-checklist — Guvenlik Incelemesi Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu degisiklikte sifre/anahtar/PII siziyor mu, yetki siniri ve parametreli SQL korunuyor mu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Guvenlik bulgusu **her zaman CRITICAL** baslar; `reviewer` seviye dusurebilir ama kanitsiz dusuremez (AGENTS.md §3.2 #5). Bu liste yalniz **tespit** içindir — derinlesme ve duzeltme karsiligi ilgili uzman listelerindedir. Kanit = calistirilmis tarama komutunun ciktisi; yorum degil.

| Alan | Deger |
|---|---|
| Kim doldurur | `reviewer` (guvenlik rutini) |
| Baslangic seviyesi | Her bulgu once **CRITICAL** yazilir |
| Kanit paketi | grep/Select-String ciktilari + `git log` |
| Soru sayisi | 14 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Diff'te sifre/anahtar/subscription key var mi (0 olmali)?
  **Kanit:** `Select-String -Path **\*.cs,*.json,*.md -Pattern '(password|apikey|secret|token)\s*=\s*"[^"]{8,}'` = 0 satir.
- [ ] 2. Connection string duz metin mi (sifreli ya da `SecretManager` uzerinden olmali)?
  **Kanit:** dosya yolu + satir; duz metin ise `G2`/guvenlik bulgusu.
- [ ] 3. `.gitignore` gizli dosyalari kapsiyor mu (`.env`, `*.pfx`, `appsettings.*.Local.json`)?
  **Kanit:** `.gitignore` ilgili satirlari + `git check-ignore -v <dosya>` ciktisi.
- [ ] 4. Git gecmisinde sifre izi var mi (silinmis de olsa)?
  **Kanit:** `git log --all -p -S 'password' --oneline` ciktisi (bos ise "0 eslesme").
- [ ] 5. Her SQL sorgusu parametreli mi (string-concat 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id|Name)'` = 0 satir.
- [ ] 6. `CommandTimeout` her komutta set edilmis mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'commandTimeout'` sayisi ile komut sayisi esitligi (rakamla).
- [ ] 7. Kullanici girdisi dogrudan dosya yolu oluyor mu (path traversal)?
  **Kanit:** `Path.Combine` + `GetFullPath` kontrolu ornegi ya da "0 adim" satiri.
- [ ] 8. Log'a secret/PII yaziliyor mu (0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Log.*password|Log.*token|Log.*Connection'` = 0 satir.
- [ ] 9. Hata mesajlari ic detay (stack/SQL) kullaniciya donuyor mu?
  **Kanit:** hata yakalama blogunun ornegi + "genel mesaj" satiri.
- [ ] 10. Dosya yazma islemleri whitelist dizinle mi sinirli?
  **Kanit:** dosya yolu ureten kodun 1 ornegi + izinli kok tanimi.
- [ ] 11. Yikici islem (silme, `reset --hard`) onayli mi (G7)?
  **Kanit:** `log.md`'de onay satiri ya da "YOK".
- [ ] 12. Harici `http://` (TLS'siz) istek var mi (0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'http://'` = 0 satir.
- [ ] 13. Guvenlik kurallari `.rules/security-rules.md` ile celisiyor mu?
  **Kanit:** celiski varsa `CONFLICT DETECTED` etiketi, yoksa "0 celiski".
- [ ] 14. Bulgu `.ai/reports/**` altina seviyeli yazildi mi?
  **Kanit:** raporda `[CRITICAL]/[HIGH]/[MEDIUM]/[LOW]` etiketli satir sayisi + toplam.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 4 | Repoda yasanan secret → **CRITICAL**, is durur, anahtar yenilenir |
| 2 | Soru 5 veya 6 | String-concat SQL / timeout yok → **G2 CRITICAL** |
| 3 | Soru 2 | Duz metin connection string → sifreleme + `SecretManager` zorunlu |
| 4 | Soru 8 veya 9 | Secret/PII logda ya da hata mesajinda → sızıntı, kod RED |
| 5 | Soru 11 | Onaysiz yikici islem → **G7**, islem geri alinir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| SQL enjeksiyonunun ayrintili kalip incelemesi | [sql-injection-review.md](sql-injection-review.md) |
| Secret saklama tasarimi ve dosya sifreleme | [secret-leak-check.md](secret-leak-check.md) |
| Bagimli paket CVE denetimi | [dependency-update-check.md](dependency-update-check.md) |
| Vault dosyalarinda sızıntı | [vault-link-check.md](vault-link-check.md) |
| Yazili guvenlik kuralari | [../CLAUDE.md](../CLAUDE.md) §1 · [../spec/test-strategy.md](../spec/test-strategy.md) |
