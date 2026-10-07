---
title: "vault-link-taramasi - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "check-links + find-mojibake + measure-vault ile vault butunlugu taramasi - yorumlanma, hatanin bulunmasi ve geri alma"
---

# vault-link-taramasi — Vault Butunluk Taramasi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [batch-doc-uretme](batch-doc-uretme.md)

> **Tek cumlelik sorumluluk:** *Vault link/kodlama sagligi hangi komutla kanitlanir, cikti nasil okunur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her batch ve her gate sonunda **3 script** kosulur: `check-links.ps1` (bozuk link 0), `find-mojibake.ps1` (mojibake 0), `measure-vault.ps1` (frontmatter 0 eksik). Cikti kirmizisa duserse **neden bazli** giderilir: hedef mi yok, ad mi yanlis, kod blogu mu link sayildi. Toplam ≈5 dk (tarama) + ≈10 dk (duzeltme).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Vault `.ai/` altinda | `C:\versacoder\.ai` klasoru var |
| 2 | Scriptler yerinde | `.ai/scripts/{check-links,find-mojibake,measure-vault}.ps1` |
| 3 | Guncel dosyalar kaydedilmemis yama ile kalmamis | `git status --porcelain` |
| 4 | Izinli link hedefleri biliniyor | `new-doc-contract.md` §3 |
| 5 | Beklenen sayilar not edilmis | onceki batch raporu |

---

## §2 Adimlar

**Toplam ≈15 dk** · yapan: `reviewer` ya da batch'i yapan rol

1. **Link taramasi calistir** — kod bloglari ve satir ici kod link sayilmaz (sahte alarm yok).
   - Komut: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Cikti: `BOZUK link : 0` (kirmizi ise hedef bazli gruplama listesi)

2. **Kodlama taramasi calistir** — UTF-8 Latin-1'e cevirilmis metin izleri.
   - Komut: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1`
   - Cikti: `Mojibake file count: 0` (0 degilse dosya adi listelenir)

3. **Tek dosyayi incele** — bulunan dosyada hangi satir, hangi hedef; duzeltme tahminle degil satirla.
   - Komut: `powershell -File C:\versacoder\.ai\scripts\show-mojibake.ps1 -Path C:\versacoder\.ai\<dosya>.md`
   - Cikti: `L<satir> [U+XXXX ...] <metin>`

4. **Kök nedene gore duzelt** — yanlis ad -> dogrusu; olmayan hedef -> linki kaldir ya da dosyayi yaz; bozuk kodlama -> dosyayi UTF-8 yeniden yaz.
   - Dosya: yalniz sucu olan dosya (`edit` ile tek satir)
   - Cikti: adim 1-2'yi tekrar -> 0 / 0

5. **Olcum + kayit** — frontmatter/CR-only/500 MD kapisi ve `log.md` notu.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\measure-vault.ps1`
   - Cikti: `NoFrontmatter 0 · Mojibake 0 · CROnly 0` + kayit satiri

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Link | `BOZUK link : 0` |
| K2 | Mojibake | `Mojibake file count: 0` |
| K3 | Frontmatter | `NoFrontmatter = 0` |
| K4 | CR-only | `CROnly = 0` |
| K5 | Kayit | `log.md`'de tarih + "tarama 0/0/0" satiri |

**Ornek cikti (kirmizi hali - gruplama):**

```text
HEDEF BAZLI GRUPLAMA (azalan):
    3x  ../spec/session-spec.md
DOSYA BAZLI (ilk 40):
    playbooks\oturum-devir-teslim.md  ->  ../spec/session-spec.md
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Bozuk link duzeltmesi (ad/defter) | `git diff <dosya>` + `git checkout -- <dosya>` |
| 2 | Dosya UTF-8'e yeniden yazildi | icerik farkini kontrol et, gerekirse onceki surume don |
| 3 | Link silindi | linki geri ekle; gercek hedefi yoksa **dosyayi** uret (batch'e don) |
| 4 | Eski index/rezerve ad | `index.md`'deki kayit satirini geri al |
| 5 | Tarama kaydi | `log.md` satirini `REVERT` olarak isaretle (silme) |

