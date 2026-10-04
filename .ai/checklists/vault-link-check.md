---
title: "vault-link-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Vault link bicim denetimi icin 13 kanitli soru: 0 bozuk, izinli hedef, yasak protokol, frontmatter, mojibake"
---

# vault-link-check — Vault Link / Bicim Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Her goreli link diskteki dosyaya mi gidiyor, izinli hedefler disina cikan link var mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bozuk link = **G4 ihlali = RED**; tek bir olu hedef bile batch'i kapatmaz. Link yalniz `new-doc-contract.md` §3'te izinlenen goreli hedeflere yazilir; `http(s)://`, `mailto:` ve dosya adi olmayan hedefler uretilmez. Kanit = `check-links.ps1` ciktisi (toplam/iyi/bozuk) + hedef bazli gruplama.

| Alan | Deger |
|---|---|
| Kim doldurur | `architect` · denetim `reviewer` |
| Kanit | `check-links.ps1` → `BOZUK link : 0` + `exit 0` |
| Kural | Izinli hedef listesi `.templates/new-doc-contract.md` §3 |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. `check-links.ps1` **0 bozuk** mu ve `exit 0` mi?
  **Kanit:** script ciktisi (Toplam / Iyi / BOZUK satirlari).
- [ ] 2. `measure-vault.ps1` `NoFrontmatter = 0` mi?
  **Kanit:** ilgili satir (rakamla).
- [ ] 3. `find-mojibake.ps1` `Mojibake file count: 0` mi?
  **Kanit:** script ciktisinin ilk satiri.
- [ ] 4. Her MD'nin **ilk satiri** `---` mi (frontmatter'suz dosya 0)?
  **Kanit:** `Get-ChildItem .ai -Recurse -Filter *.md | Where { (Get-Content $_.FullName -TotalCount 1) -ne '---' }` = 0 dosya.
- [ ] 5. Linkler **goreli** mi (mutlak `C:\` yolu 0)?
  **Kanit:** `Select-String -Path .ai\**\*.md -Pattern '\]\(C:\\'` = 0 satir.
- [ ] 6. `mailto:` / `file:` / `vscode:` linki var mi (0 olmali)?
  **Kanit:** `Select-String -Path .ai\**\*.md -Pattern '\]\((mailto|file|vscode):'` = 0 satir.
- [ ] 7. Harici `http(s)://` link sayisi **az** mi ve gercekten gerekiyor mu?
  **Kanit:** `Select-String -Path .ai\**\*.md -Pattern '\]\(https?://'` sayisi (rakam + not).
- [ ] 8. `[[wiki]]` biciminde olmayan `[[…]]` link var mi (checklist'te `[[` kullanimi 0)?
  **Kanit:** `Select-String -Path .ai\checklists\*.md -Pattern '\[\['` = 0 satir.
- [ ] 9. Ayni hedefe **iki farkli yazim** ile link veriliyor mu (tutarlilik: `../index.md` mi, `/index.md` mi)?
  **Kanit:** yazim varyantlarinin `Group-Object` sonucu (varyant = 0 ya da teklestirme notu).
- [ ] 10. Link verilen kardes dosyalar **adini tam** yaziyor mu (kisa ad `index` yok)?
  **Kanit:** ornek 3 link + hedef dosya yolu eslesmesi (`Test-Path = True`).
- [ ] 11. `#anchor` disinda bos link var mi (0 olmali)?
  **Kanit:** `Select-String -Path .ai\**\*.md -Pattern '\]\(\s*\)'` = 0 satir.
- [ ] 12. Yeni dosyalar `.ai/checklists/` altinda **kebab-case + ASCII** adli mi (Turkce karakter 0)?
  **Kanit:** `Get-ChildItem .ai\checklists -Filter *.md | Where { $_.Name -match '[^\x00-\x7F]' }` = 0 dosya.
- [ ] 13. `checklists/index.md` kayit tablosundaki her link yesil mi?
  **Kanit:** `check-links.ps1` ciktisinda `checklists\index.md` satirinin olmamasi.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | `BOZUK link > 0` → **G4 RED**, o dosya duzeltilir |
| 2 | Soru 4 | Frontmatter'siz dosya → G4, bicim duzeltilir |
| 3 | Soru 3 | Mojibake > 0 → UTF-8 yeniden yazim |
| 4 | Soru 5 veya 6 | Mutlak yol / `mailto:` → izin disi link, silinir |
| 5 | Soru 13 | Index linki kırık → kayit defteri gecersiz, batch tekrarlanir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Vault **icerik** kalitesi (soru-kanit bicimi) | [documentation-check.md](documentation-check.md) |
| GATE-004a kapanis script paketi | [gate-004a-vault-kapanis.md](gate-004a-vault-kapanis.md) |
| Kod linki/git baglantisi degil | [pr-pre-check.md](pr-pre-check.md) |
| Session/handoff notlarinin guncel olmasi | [session-handoff-check.md](session-handoff-check.md) |
| Link kurali tanimi (izinli hedefler) | [../CLAUDE.md](../CLAUDE.md) §1 G4 · [../spec/test-strategy.md](../spec/test-strategy.md) |
