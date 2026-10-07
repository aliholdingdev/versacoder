---
title: "yeni-rule-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: ".rules/ altinda yeni kural dosyasi veya madde acma - kural, ihlal sonucu, tespit komutu uclusu ve ADR/keys guncelleme"
---

# yeni-rule-acma — Yeni Kural Acma

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](../.rules/coding-standards.md) · [adr-acma](adr-acma.md)

> **Tek cumlelik sorumluluk:** *Bir kural neden zorunlu olur, ihlali nasil yakalanir, nasil geri alinir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kural tek basina **yetmez**: her madde `kural · ihlal sonucu · tespit komutu` uclusuyle yazilir; tespiti olmayan madde denetlenemez ve RED sayilir. `.rules/<konu>-rules.md` icine acilir, dayanagi olan ADR varsa baglanir, `CLAUDE.md` G-madde eslemesi gerekirse guncellenir. Toplam ≈20 dk. Vault'ta kural degisikliginin X sahibi `architect`tir (AGENTS §3.2 #9).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Ilgili ADR accepted | `.ai/.decisions/accepted/ADR-0NN-*.md` |
| 2 | Mevcut kural dosyasi okunmus (tekrar yok) | `.ai/.rules/` klasor listesi |
| 3 | G-madde eslemesi biliniyor | `.ai/CLAUDE.md` G1..G12 tablosu |
| 4 | Tespit komutu calisir durumda | komutun kendi ciktisi (0/1 satir) |
| 5 | Cakisan madde aranmis | `Select-String -Path '.ai\.rules\*.md' -Pattern '<anahtar>'` |

---

## §2 Adimlar

**Toplam ≈20 dk** · yapan: `architect` (inceleme: `reviewer`)

1. **Konu + dosya sec** — yeni alan mi, var olan dosyaya madde mi? Ayni bilgi 2 dosyaya yazilmaz (SSOT).
   - Klasor: `C:\versacoder\.ai\.rules\`
   - Cikti: tek hedef dosya adi

2. **Maddeyi uclusuyle yaz** — `ID | Kural | Neden | Ihlal belirtisi | Duzeltme` sutunlari.
   - Ornek: `DA-17 | parameterized-only ... | injection ... | $"" + SQL | @p1 + DynamicParameters`
   - Cikti: her satirda 5 sutun dolu

3. **Tespit komutunu ekle** — ihlali yakalayan tek satir PowerShell; ciktisi "0 satir" ise dogru.
   - Ornek: `Select-String -Path 'src\**\*.cs' -Pattern 'RibbonControlStyle\.\w+'`
   - Cikti: beklenen cikti + sapma belirtisi (tablo)

4. **Bag + defter** — ADR, `keys.md` ve gerekiyorsa `CLAUDE.md` G-madde satiri.
   - Dosya: `.ai\keys.md` §3/§4, `.ai\CLAUDE.md` G-tablosu
   - Cikti: kural -> ADR -> G-madde zinciri gorunur

5. **Dogrula** — link ve kendi tespit komutunu calistir.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Cikti: `BOZUK link : 0` + tespit komutu beklenen ciktida

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Madde 5 sutun | `Kural · Neden · Ihlal · Duzeltme` dolu |
| K2 | Tespit komutu | temiz vault'ta `0 satir`, bilinen ihlalde `> 0` |
| K3 | ADR/G bagi | dosyada `ADR-0NN` ve/veya `G2..G12` referansi |
| K4 | Tekrar yok | ayni madde baska `.rules/` dosyasinda yok |
| K5 | Link | `check-links.ps1` -> `BOZUK link : 0` |

**Ornek cikti (K2):**

```text
PS> Select-String -Path 'src\**\*.cs' -Pattern 'EntityFramework'
(sonuc yok)  ->  0 satir  ->  K2 PASS
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni dosya | dosyayi sil + `keys.md`/index bagini geri al |
| 2 | Yeni madde | satiri sil, kardes dosyada karsiligi varsa onu guncelle |
| 3 | ADR bagi | ADR `superseded` yapilir; kural silinmeden once ADR karariyla birlikte geri alinir |
| 4 | `CLAUDE.md` G-eslemesi | G-tablosunu eski satirina dondur (anayasa degisikligi = kullanici onayi, G7) |
| 5 | Tespit komutu | komutu da geri al; kalmis komut yanlis alarm uretir |

