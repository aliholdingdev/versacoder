---
title: "batch-doc-uretme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GATE-004a gibi toplu MD uretimi - new-doc-contract uyumu, rezerve adlar, subagent paylastirma ve bitis kontrolu"
---

# batch-doc-uretme — Toplu Dokuman Uretimi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [vault-link-taramasi](vault-link-taramasi.md)

> **Tek cumlelik sorumluluk:** *Bir batch klasore birden fazla MD nasil sorunsuz uretilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Batch = **sozlesme once, yazim sonra**: `new-doc-contract.md` okunmadan tek dosya bile uretilmez. Rezerve adlar (index'in link attigi dosyalar) **ilk sirada** yazilir, aksi link kirar. Her dosya 8 alanli frontmatter + §0 + en az 1 tablo + maks 5 madde + 3-7 KB. Batch sonu `check-links.ps1` **0 bozuk** olmali. 10 dosyalik batch ≈50 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Manifest + sozlesme okunmus | `.ai/.templates/batch-manifest-gate004a.md` §1/§4 |
| 2 | Rezerve ad listesi alinmis | hedef klasorun `index.md` kayit tablosu |
| 3 | Izinli link hedefleri biliniyor | `new-doc-contract.md` §3 satiri |
| 4 | Hedef klasor mevcut | `Get-ChildItem .ai\<klasor>` |
| 5 | Bitis komutlari hazir | `check-links.ps1` + `find-mojibake.ps1` |

---

## §2 Adimlar

**Toplam ≈50 dk / 10 dosya** · yapan: `architect` (+ paralel subagent)

1. **Sozlesme + rezerve adlar** — once manifest §4'teki adlar, sonra yeni adlar; ad `kebab-case.md`, ASCII.
   - Dosya: `.ai\.templates\new-doc-contract.md` + hedef `index.md`
   - Cikti: yazilacak dosya adi listesi (sirali)

2. **Iskeleti sabitle** — her dosya ayni iskeletle baslar: frontmatter 8 alan, H1, `Zorunlu Baglantilar`, tek cumlelik sorumluluk, §0..§4.
   - Sablon: `new-doc-contract.md` §1-§2
   - Cikti: ilk satir `---`, `type: playbook|spec|checklist|...`, `updated: 2026-09-25`

3. **Dosyalari yaz (part part)** — tek turda en fazla 3-4 dosya; her dosya 3-7 KB, madde max 5, adim max 5.
   - Klasor: `C:\versacoder\.ai\<hedef>\`
   - Cikti: dosyalar UTF-8, kardes linkleri adlariyla tam

4. **Defteri guncelle** — `index.md`'ye kayit satiri; rezerve adlar linklenmis olmali.
   - Dosya: `.ai\<hedef>\index.md` (bolum: Kayit Tablosu)
   - Cikti: her yeni dosya icin 1 satir

5. **Bitis kontrolu** — iki script; bozuk cikarsa **o dosya** duzeltilir, baska dosyaya dokunulmaz.
   - Komut: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Komut: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1`
   - Cikti: `BOZUK link : 0` + `Mojibake file count: 0`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Rezerve adlar | hedef `index.md`'de linklenen her dosya diskte var |
| K2 | Frontmatter | `Get-Content -TotalCount 1` -> `---` (0 dosya eksik) |
| K3 | Link | `check-links.ps1` -> `BOZUK link : 0` |
| K4 | Kodlama | `find-mojibake.ps1` -> `Mojibake file count: 0` |
| K5 | Boyut | her dosya 3-7 KB (olcum script'i ile) |

**Ornek cikti (K3-K4):**

```text
Taranan MD      : 153
Iyi link        : 1204
BOZUK link      : 0
Mojibake file count: 0
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni dosyalar | batch dosyalarini sil (listeyi yazarken sakla) |
| 2 | `index.md` satirlari | eklenen satirlari geri al |
| 3 | Kardes dosyada duzeltme | `git diff` ile tek dosyaya don, `git checkout -- <dosya>` |
| 4 | Rezerve ad kirildi | ilk 4 adi aynen yeniden yaz, linkleri kurtar |
| 5 | Bitis kontrolu kirmizi | duzeltme yerine batch'i geri al + nedeni `log.md`'ye yaz |

