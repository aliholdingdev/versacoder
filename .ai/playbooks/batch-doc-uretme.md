---
title: "batch-doc-uretme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Toplu MD uretimi - sozlesme once, rezerve adlar, part part yazim, subagent paylastirma ve bitis kontrolu"
---

# batch-doc-uretme — Toplu Dokuman Uretimi

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [templates](../.templates/new-doc-contract.md) · [vault-link-taramasi](../playbooks/vault-link-taramasi.md)

> **Tek cumlelik sorumluluk:** *Bir klasore birden fazla MD dosyasi sorunsuz nasil uretilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Batch = **sozlesme once, yazim sonra**: [templates](../.templates/new-doc-contract.md) okunmadan tek dosya bile uretilmez. Index'in rezerve adlari **ilk sirada** yazilir, aksi link kirar. Her dosya 8 alanli frontmatter + §0 + en az 1 tablo + en fazla 5 madde + 2.600-8.500 bayt olur. Ayni anda en fazla **3** paralel ajan (AGENTS §5); tek dosyaya iki yazici yok. Batch sonu link + kodlama taramasi **0** olmali. 10 dosyalik batch ≈50 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Sozlesme okunmus | `.templates/new-doc-contract.md` §1-§3 |
| 2 | Rezerve ad listesi alinmis | hedef klasorun `index.md` kayit tablosu |
| 3 | Izinli link hedefleri biliniyor | sozlesme §3 satiri (klasor bazli) |
| 4 | Hedef klasor mevcut | `Get-ChildItem .ai\<klasor>` |
| 5 | Bitis komutlari hazir | `check-links.ps1` + `find-mojibake.ps1` |

---

## §2 Adimlar

**Toplam ≈50 dk / 10 dosya** · yapan: `architect` + en fazla 3 paralel ajan

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Sozlesme + rezerve adlari sirala | ad `kebab-case.md`, ASCII; liste dosya bazli | 5 dk |
| 2 | Iskeleti sabitle (frontmatter 8 alan, H1, §0..§9) | sozlesme §1-§2 ornegi | 5 dk |
| 3 | Dosyalari part part yaz (part basina 3-4 dosya) | her dosya 2.600-8.500 bayt, madde <= 5 | 25 dk |
| 4 | Defteri guncelle | hedef `index.md`'ye 1 satir / dosya | 5 dk |
| 5 | Bitis kontrolu (2 script) | `BOZUK link : 0` + `Mojibake file count: 0` | 10 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | `BOZUK link > 0` | rezerve ad gec yazilmis ya da ad yanlis | o dosya duzeltilir, baska dosyaya dokunulmaz |
| 2 | `NoFrontmatter > 0` | iskelet atlannmis | dosya ilk satirdan yeniden tamamlanir |
| 3 | Dosya < 2.600 bayt | icerik ozetlenmiş, tablo eksik | §1-§9 bolumleri tamamlanir |
| 4 | Paralel ezisme | ayni dosyaya 2 ajan | seriye al (AGENTS §5), kaniti olan kazanir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni dosyalar | batch listesiyle birlikte silinir |
| 2 | `index.md` satirlari | eklenen satirlar geri alinir |
| 3 | Kardes dosyada duzeltme | `git diff <dosya>` + `git checkout -- <dosya>` |
| 4 | Rezerve ad kirildi | ilk adlar aynen yeniden yazilir, linkler kurtarilir |
| 5 | Bitis kontrolu kirmizi | duzeltme yerine batch geri alinir + neden `log.md`'ye yazilir |

---

## §5 Kabul Kriteri

- [ ] Her dosya ilk satiri `---` ve 8 alanli frontmatter
- [ ] Rezerve adlar en basa yazildi; hedef `index.md` linklenen her dosya diskte
- [ ] Her dosya 2.600-8.500 bayt; madde listesi en fazla 5
- [ ] `check-links.ps1` -> `BOZUK link : 0`
- [ ] `find-mojibake.ps1` -> `Mojibake file count: 0`

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : measure-vault -> MD=NNN, FM=0 eksik     |
| DOSYA  : .ai/playbooks/<yeni dosyalar>           |
| EKSIK  : YOK                                      |
| SONRAKI : check-links + find-mojibake kos (~10 dk)|
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 2 | Kodlama | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1` | `Mojibake file count: 0` |
| 3 | Olcum | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1` | `NoFrontmatter = 0` |
| 4 | Ajan sayisi | AGENTS §5 | ayni anda <= 3 |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Dosya iceriginin kurali (ne yazilacagi) | ilgili `.rules/` ve spec dosyalari |
| 2 | Gate sirasi ve kanit kutusu | [WORKFLOW](../WORKFLOW.md) §2 |
| 3 | Ajan secimi, cikti kutusu | [AGENTS](../AGENTS.md) §1 ve §2 |
| 4 | Tek dosyalik duzeltme | [vault-link-taramasi](../playbooks/vault-link-taramasi.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `.templates/new-doc-contract.md` | boyut, frontmatter ve link sinirlari |
| 2 | `playbooks/index.md` | batch kabul kriteri (§4) |
| 3 | `checklists/gate-004a-vault-kapanis.md` | kapanis sirasi ve 14 soru |
| 4 | `AGENTS.md` | paralel ajan siniri (§5) ve 5 alanli cikti kutusu (§2) |
