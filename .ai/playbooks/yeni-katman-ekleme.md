---
title: "yeni-katman-ekleme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l0..l7 mimarisine yeni katman veya dugum ekleme - ADR, katalog uretimi, build kaniti ve geri alma"
---

# yeni-katman-ekleme — Katman / Dugum Ekleme

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [adr-acma](../playbooks/adr-acma.md)

> **Tek cumlelik sorumluluk:** *Mimariye yeni katman veya dugum hangi sirayla eklenir, kaniti nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Yeni katman/dugumu yalniz `architect` acar; **kod bu isin parcasi degildir** (GATE-003 acikken `.cs` yazmak G12 ihlalidir). Sira degistirilemez: **karar -> ADR -> katman node dosyasi -> katalog uretimi -> sayi + link dogrulamasi**. Kanit yoksa is bitmis sayilmaz (G5): katalog uretilmeden "tamam" denmez. Toplam ≈25 dk; kapi acilisinda [layer-boundary-check](../checklists/layer-boundary-check.md) 13 sorusu cevaplanir.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Katman baglilik yonu biliniyor (ic bagimlilik yalniz `interface`) | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) |
| 2 | Mevcut node listesi okunmus | `powershell -NoProfile -ExecutionPolicy Bypass -File .ai\scripts\count-components.ps1` |
| 3 | Karar en az 2 alternatifli yazilmis | [adr-acma](../playbooks/adr-acma.md) §2 |
| 4 | Kod yazilacaksa gate kapali mi | [WORKFLOW](../WORKFLOW.md) §1.2 (GATE-003) |
| 5 | Kural karsiligi bu karari zorunlu kiliyor | [rules index](../.rules/index.md) |

---

## §2 Adimlar

**Toplam ≈25 dk** · yapan: `architect`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Karari ver + ADR ac | 2 alternatif, (+)/(−) sonuc, `hedef_bacak` alani dolu | 8 dk |
| 2 | Node dosyasini yaz | katman klasorunde node detay: arayuz, akis, test plani, TFM | 7 dk |
| 3 | Katalogu yeniden uret | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\build-catalog.ps1 -Force` -> yeni kartlar | 3 dk |
| 4 | Sayiyi ve linki dogrula | `count-components.ps1` -> `PASS` + `check-links.ps1` -> `BOZUK link : 0` | 4 dk |
| 5 | Defter + kural guncelle + devir | kayit satiri, kural maddesi, 4 maddelik devre paketi (`developer`'a) | 3 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Katalog sayisi artmadi | Elle tablo yazilmis ya da script eksik | Uretim script ile tekrarlanir, elle tablo yasak |
| 2 | `BOZUK link > 0` | Node adi katalogda/eskilmis | Ad hizalanir, tarama tekrar kosulur |
| 3 | 13 sorudan biri cevapsiz | Kapi acilisi yarim | [layer-boundary-check](../checklists/layer-boundary-check.md) tamamlanir |
| 4 | Kod bu isin icine girmis | Gate acikken `.cs` yazilmis | Kod geri alinir, is vault asamasina doner |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni node dosyasi | dosya silinir |
| 2 | Katalog uretimi | katalog script'i eski tohumlarla yeniden basilir |
| 3 | ADR | `status: accepted` -> `superseded` + gerekce |
| 4 | `log.md` + index satiri | satir `REVERT <tarih>` olarak isaretlenir (silinmez) |
| 5 | Koda yansimasi varsa (gate sonrasi) | `git revert <sha>` + iki bacak build |

---

## §5 Kabul Kriteri

- [ ] ADR `status: accepted`, 2 alternatif ve `hedef_bacak` alani dolu
- [ ] Node dosyasi frontmatter'li, en az 1 tablo, iki TFM yazili
- [ ] `count-components.ps1` -> `PASS`
- [ ] `check-links.ps1` -> `BOZUK link : 0`
- [ ] 4 maddelik devre paketi `developer`'a teslim edilmis (AGENTS §3)

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : count-components -> PASS (kayit += 1)    |
|          check-links      -> BOZUK=0              |
|          katman sorulari  -> 13/13 cevapli        |
| DOSYA  : ADR-0NN-<konu>.md, katman node dosyasi   |
| EKSIK  : YOK                                       |
| SONRAKI : devre paketiyle developer'a gec (~2 dk) |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Katalog | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` | `PASS` |
| 2 | Link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 3 | Olcum | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1` | `NoFrontmatter = 0` |
| 4 | Butce | adim tablosu | ≈25 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Katman icine kod yazimi | `developer` (AGENTS §1) |
| 2 | Katman ihlali suphesi / 13 soru denetimi | `reviewer` + [layer-boundary-check](../checklists/layer-boundary-check.md) |
| 3 | Kural metni | [rules index](../.rules/index.md) |
| 4 | Ekran/kapi (GATE-003) | [WORKFLOW](../WORKFLOW.md) §2.2 |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | katman, proje ve blok temeli |
| 2 | `checklists/layer-boundary-check.md` | 13 soruluk kapi kontrolu |
| 3 | `scripts/count-components.ps1` | katalog sayac ciktisi |
| 4 | `AGENTS.md` | `architect` profili, devre paketi, paralel sinir |
