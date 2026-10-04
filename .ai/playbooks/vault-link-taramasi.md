---
title: "vault-link-taramasi - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "check-links + find-mojibake + measure-vault ile vault butunlugu taramasi - yorumlanma, duzeltme ve geri alma"
---

# vault-link-taramasi — Vault Butunluk Taramasi

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [templates](../.templates/new-doc-contract.md) · [gate-004a](../checklists/gate-004a-vault-kapanis.md) · [batch-doc-uretme](../playbooks/batch-doc-uretme.md)

> **Tek cumlelik sorumluluk:** *Vault link/kodlama sagligi hangi komutla kanitlanir, cikti nasil okunur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her batch ve her gate sonunda **3 script** kosulur: `check-links.ps1` (bozuk link **0**), `find-mojibake.ps1` (mojibake **0**), `measure-vault.ps1` (eksik frontmatter **0**). Cikti kirmizisa duserse **neden bazli** giderilir: hedef mi yok, ad mi yanlis, kod blogu mu link sayilmis. Duzeltme yalniz sucu olan dosyaya yapilir; baska dosyaya dokunulmaz. Tarama ≈5 dk + duzeltme ≈10 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Vault `.ai/` altinda | `C:\versacoder\.ai` klasoru var |
| 2 | Scriptler yerinde | `scripts/` altinda 3 script |
| 3 | Dosyalar kaydedilmemis yama ile kalmamis | `git status --porcelain` |
| 4 | Izinli link hedefleri biliniyor | [templates](../.templates/new-doc-contract.md) §3 |
| 5 | Beklenen sayilar not edilmis | onceki batch raporu |

---

## §2 Adimlar

**Toplam ≈15 dk** · yapan: `reviewer` ya da batch'i yapan rol

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Link taramasi | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` -> `BOZUK link : 0` | 2 dk |
| 2 | Kodlama taramasi | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1` -> `Mojibake file count: 0` | 2 dk |
| 3 | Sucu olan dosyayi incele | hangi satir, hangi hedef (tahmin degil, satir no) | 3 dk |
| 4 | Kok nedene gore duzelt | yanlis ad -> dogrusu; olmayan hedef -> link kaldir ya da dosyayi yaz | 5 dk |
| 5 | Olcum + kayit | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1` + `log.md` notu | 3 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | `BOZUK link > 0` | Rezerve ad gec yazilmis | Hedef dosya uretilir ya da link kaldirilir |
| 2 | `Mojibake file count > 0` | CP1254/Latin-1 karisimi metin | Dosya UTF-8 ile yeniden yazilir |
| 3 | `NoFrontmatter > 0` | Iskelet atlanmis | 8 alanli frontmatter tamamlanir |
| 4 | Duzeltme sonrasi sayi artti | Ayni ad yanlis baska yerde de kullanilmis | Tum kullanimlar tek adla hizalanir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Bozuk link duzeltmesi | `git diff <dosya>` + `git checkout -- <dosya>` |
| 2 | Dosya UTF-8'e yeniden yazildi | icerik farki kontrol edilir, gerekirse onceki surume donulur |
| 3 | Link silindi | link geri eklenir; gercek hedef yoksa **dosya** uretilir |
| 4 | Rezerve ad kaydi | `index.md` kayit satiri geri alinir |
| 5 | Tarama kaydi | `log.md` satiri `REVERT` olarak isaretlenir (silinmez) |

---

## §5 Kabul Kriteri

- [ ] `check-links.ps1` -> `BOZUK link : 0`
- [ ] `find-mojibake.ps1` -> `Mojibake file count: 0`
- [ ] `measure-vault.ps1` -> `NoFrontmatter = 0`
- [ ] Duzeltmeler yalniz sucu olan dosyalarda (`git diff` listesi kisa)
- [ ] `log.md`'de tarihli "tarama 0/0/0" satiri var

---

## §6 Ornek Cikti (kirmizi hali)

```text
+--------------------------------------------------+
| KANIT  : check-links -> BOZUK=3 (gruplandi)      |
|          find-mojibake -> file count=1            |
|          measure-vault -> NoFrontmatter=2         |
| DOSYA  : playbooks/<ad>, index.md                |
| EKSIK  : hedef dosya DOĞRULAMA GEREKLI           |
| SONRAKI : hedefi uret ya da linki kaldir (~5 dk) |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 2 | Kodlama | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1` | `Mojibake file count: 0` |
| 3 | Olcum | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1` | `NoFrontmatter = 0` |
| 4 | Butce | adim tablosu | ≈15 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Toplu dosya uretimi | [batch-doc-uretme](../playbooks/batch-doc-uretme.md) |
| 2 | Gate kapanis sirasi (14 soru) | [gate-004a](../checklists/gate-004a-vault-kapanis.md) |
| 3 | Dosya bicimi / boyut kurali | [templates](../.templates/new-doc-contract.md) |
| 4 | Kod tarafindaki bozulma (build/test) | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `scripts/check-links.ps1` | bozuk link sayaci ve gruplama ciktisi |
| 2 | `scripts/find-mojibake.ps1` | mojibake dosya sayaci |
| 3 | `scripts/measure-vault.ps1` | frontmatter / CR-only / MD sayaci |
| 4 | `checklists/vault-link-check.md` | link denetim sorulari |
