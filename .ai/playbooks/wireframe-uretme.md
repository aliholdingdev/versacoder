---
title: "wireframe-uretme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "7 ekran icin ASCII wireframe uretimi - kutu cizim, kontrol listesi v26.1, GATE-003 onayi ve geri alma"
---

# wireframe-uretme — ASCII Wireframe Uretimi

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [glossary](../glossary.md) · [yeni-form-tasarlama](../playbooks/yeni-form-tasarlama.md)

> **Tek cumlelik sorumluluk:** *Bir ekranin tel kafesi nasil cizilir, ne zaman onaya gider* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Wireframe **koddan once** gelir ve **GATE-003 = kullanici onayi (HARD STOP)** ister; onaysiz kodlama G12 ihlalidir, is basa doner. Dosya `.ai/ui-design/NN-<ekran>.md`, 7 ekran sabittir (`01-start-page` .. `07-settings`); kutu cizim **saf ASCII** (`+ - |` karakterleri; Unicode kutu cizim `U+2500-U+257F` YASAK), kontrol listesi yalniz DevExpress **v26.1**. Tek ekran ≈25 dk (onay suresi haric).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | 7 ekran listesi kilitli | [VISION](../VISION.md) + `PROJECTS.md` |
| 2 | Ekran terimleri net | [glossary](../glossary.md) terimleri |
| 3 | Kontrol listesi v26.1 ile sinirli | kural: [rules index](../.rules/index.md) |
| 4 | Ribbon stili biliniyor | `RibbonControlStyle.Office2010` |
| 5 | Kapali gate var mi | [WORKFLOW](../WORKFLOW.md) §2.2 (GATE-003) |

---

## §2 Adimlar

**Toplam ≈25 dk / ekran** · yapan: `ui-designer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Ekran + rol sec | 7 listeden biri; ad `NN-<ekran>.md`, disina cikilmaz | 3 dk |
| 2 | Kutu cizim ciz | saf ASCII (`+ - \|` karakterleri), ustte ribbon, altta durum cubugu; olcu yazilir | 10 dk |
| 3 | Kontrol listesini yaz | `Bolge / Kontrol / Surum` tablosu; surum yalniz 26.1 | 5 dk |
| 4 | Onaya gonder (HARD STOP) | onay istemi + `log.md`'ye GATE-003 satiri | 4 dk |
| 5 | Onaydan sonra devret | 4 maddelik devre paketiyle `developer`'a | 3 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Unicode kutu cizim satiri | Kopya diyagram / emoji kutu | `+ - |` ile yeniden ciz (GATE-004a BOX=0) |
| 2 | 7'den az ekran | Liste disina cikilmis ya da eksik | 7 dosya tamamlanmadan onaya gidilmez |
| 3 | Kontrol listesinde 26.1 disi surum | Paket/surum eklenmis | Kontrol v26.1'e hizalanir, surum dusurulmez |
| 4 | Onay alinmadan kod yazildi | G12 ihlali | Yazilan geri alinir, gate acik sayilir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Wireframe dosyasi | `status: draft`'a cevrilir ya da silinir, adim 2'den baslanir |
| 2 | GATE-003 onayi | onay satiri `REVERT` olarak isaretlenir, kodlama durur |
| 3 | Onaydan sonra kod yazildi | form uclemesi birlikte silinir (3 dosya) |
| 4 | Kontrol listesi degisti | eski listeye donulur + yeniden onay istenir |
| 5 | `log.md` kaydi | ustune `REVERT <tarih>` satiri eklenir |

---

## §5 Kabul Kriteri

- [ ] `.ai/ui-design/NN-<ekran>.md` var, 8 alanli frontmatter
- [ ] Diyagram satirlari saf ASCII; Unicode kutu cizim **0 satir**
- [ ] Kontrol tablosunda 26.1 disi surum **0 satir**
- [ ] `ui-design/` altinda **7/7** dosya mevcut
- [ ] `log.md`'de GATE-003 onay satiri tarihli

---

## §6 Ornek Kutu Cizim

```text
+--------------------------------------------------+
| Ribbon  : [Dosya] [Gorunum] [Araclar]             |
+--------------------------------------------------+
| Sol panel       |  Calisma alani                 |
|  - proje agaci  |                                |
|  - arama        |                                |
+--------------------------------------------------+
| Durum: hazir                                      |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Ekran sayisi | `Get-ChildItem .ai\ui-design\NN-*.md` | 7 dosya |
| 2 | Kutu cizim | `U+2500-U+257F` taramasi | 0 satir |
| 3 | Vault | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈25 dk / ekran |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Form kodu (.cs + Designer + resx) | [yeni-form-tasarlama](../playbooks/yeni-form-tasarlama.md) |
| 2 | Onay sirasi ve HARD STOP | [WORKFLOW](../WORKFLOW.md) §2.2 |
| 3 | Ekran akisi / gecis tasarimi | [glossary](../glossary.md) + ilgili spec |
| 4 | Kontrol envanteri kaydi | [AGENTS](../AGENTS.md) §1 (ui-designer) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `ui-design/01-start-page.md` .. `07-settings.md` | 7 ekran listesi ve mevcut wireframe'ler |
| 2 | `checklists/gate-003-ui-onay.md` | 13 adimlik onay kontrol listesi |
| 3 | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | katman/proje siniri (ekran kodu nereye gider) |
| 4 | `VISION.md` | ekran adlari ve urun siniri |
