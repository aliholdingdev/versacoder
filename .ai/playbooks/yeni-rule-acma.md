---
title: "yeni-rule-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "rules/ altinda yeni kural dosyasi veya madde acma - kural, ihlal sonucu, tespit komutu uclusu ve bag guncelleme"
---

# yeni-rule-acma — Yeni Kural Acma

**Zorunlu Baglantilar:** [playbooks index](index.md) · [CLAUDE](../CLAUDE.md) · [rules index](../.rules/index.md) · [adr-acma](../playbooks/adr-acma.md) · [adr-index](../.decisions/adr-index.md)

> **Tek cumlelik sorumluluk:** *Bir kural neden zorunlu olur, ihlali nasil yakalanir, nasil geri alinir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kural tek basina **yetmez**: her madde **kural · ihlal sonucu · tespit komutu** uclusuyle yazilir; tespiti olmayan madde denetlenemez ve RED sayilir. Dosya `.rules/<konu>-rules.md` icine acilir, dayanagi olan ADR baglanir, anayasayla (CLAUDE G-maddeleri) esleme gerekirse guncellenir. Vault'ta kural degisikliginin sahibi `architect`tir (AGENTS §7). Toplam ≈20 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Ilgili karar kabul edilmis | [adr-index](../.decisions/adr-index.md) satiri |
| 2 | Mevcut kural dosyalari okunmus (tekrar yok) | [rules index](../.rules/index.md) |
| 3 | Anayasa eslemesi biliniyor | [CLAUDE](../CLAUDE.md) G-madde tablosu |
| 4 | Tespit komutu calisir durumda | komutun kendi ciktisi (0/1 satir) |
| 5 | Cakisan madde aranmis | `Select-String -Path '.ai\.rules\*.md' -Pattern '<anahtar>'` |

---

## §2 Adimlar

**Toplam ≈20 dk** · yapan: `architect` · inceleme: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Konu + dosya sec | tek hedef dosya; ayni bilgi 2 dosyaya yazilmaz (SSUT) | 3 dk |
| 2 | Maddeyi uclusuyle yaz | sutunlar: `ID / Kural / Neden / Ihlal belirtisi / Duzeltme` | 7 dk |
| 3 | Tespit komutunu ekle | tek satir PowerShell; temiz vault'ta `0 satir` | 4 dk |
| 4 | Bag + defter | ADR, [keys](../keys.md) ve gerekiyorsa anayasa eslemesi | 3 dk |
| 5 | Dogrula | `check-links.ps1` -> `BOZUK link : 0` + komut beklenen ciktida | 3 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Tespit komutu hep 0 veriyor | Kalip gercek ihlali kaciriyor | Bilinen ornek ihlalde `> 0` verdigi dogrulanir |
| 2 | Ayni madde 2 dosyada | Kardes okunmadan yazilmis | Tek dosya kazanir, digeri linklenir |
| 3 | Kural var, ADR yok | Karar kaydedilmemis | Once [adr-acma](../playbooks/adr-acma.md) calisir |
| 4 | Anayasa eslemesi kirildi | G-maddesi silinmis/degistirilmis | Esleme geri alinir (G7 onayi) |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni dosya | dosya silinir + [rules index](../.rules/index.md) kaydi geri alinir |
| 2 | Yeni madde | satir silinir; karsiligi kardes dosyada varsa o guncellenir |
| 3 | Karar bagi | karar `superseded`; kural kararla birlikte geri alinir |
| 4 | Anayasa eslemesi | eski satira donulur (anayasa degisikligi = kullanici onayi) |
| 5 | Tespit komutu | komut da geri alinir; kalmis komut yanlis alarm uretir |

---

## §5 Kabul Kriteri

- [ ] Her madde 5 sutun dolu (`Kural · Neden · Ihlal · Duzeltme` + ID)
- [ ] Tespit komutu temiz vault'ta `0 satir`, bilinen ihlalde `> 0` veriyor
- [ ] Dosyada karar (ADR) ve gerekirse anayasa eslemesi referansi var
- [ ] Ayni madde baska kural dosyasinda tekrarlanmiyor
- [ ] `check-links.ps1` -> `BOZUK link : 0`

---

## §6 Ornek Madde

```text
+--------------------------------------------------+
| ID      : DA-17                                   |
| KURAL   : her sorgu parameterized (@p1)           |
| NEDEN   : injection (ADR-017)                     |
| IHLAL   : "SELECT ..." + deger / $"...{deger}"     |
| Duzeltme : DynamicParameters + @p1                |
| TESPIT  : Select-String concat kalibi -> 0 satir  |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Tekrar | `Select-String -Path '.ai\.rules\*.md' -Pattern '<anahtar>'` | tek dosyada |
| 2 | Tespit | maddeye ait komut | temiz vaultta 0 satir |
| 3 | Link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈20 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Kararin kendisi (2 alternatif + sonuc) | [adr-acma](../playbooks/adr-acma.md) |
| 2 | Denetim listesi (evet/hayir sorulari) | [yeni-checklist-acma](../playbooks/yeni-checklist-acma.md) |
| 3 | Anayasa G-maddesi metni | [CLAUDE](../CLAUDE.md) (degisiklik = kullanici onayi) |
| 4 | Kural ihlalinin canli denetimi | [guvenlik-denetimi](../playbooks/guvenlik-denetimi.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `.rules/index.md` | mevcut kural dosyalari ve tekrar kontrolu |
| 2 | `.decisions/adr-index.md` | kuralin dayandigi karar kaydi |
| 3 | `CLAUDE.md` | G-madde eslemesi (anayasa) |
| 4 | `AGENTS.md` | kural sahipligi ve devir kurali (§3 / §7) |
