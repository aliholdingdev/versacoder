---
title: "Katman Matrisi - 8x8 Yon Tablosu"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "l0..l7 arasi 8x8 izinli/yasak kenar matrisi, kilit hucre kararlari ve NetArchTest denetim kaniti (ADR-028 2.3/2)"
---

# Katman Matrisi - 8x8 Yon Tablosu

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *Bir katman digerine dogrudan referans verebilir mi* sorusunun tek cevabi bu 8x8 tablodur.

---

## §0 Ozet (Answer First)

**Matris 64 hucre = 20 OK + 40 X + 4 ayni-katman (`-`); satir = bagimli olan, sutun = bagimli olunan.** Insa sirasi `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l3` yalniz `l0`'a bakar; `l2` ile `l4` arasinda dogrudan ok YOK (repository portu `l0`'da, uygulamasi `l4`'te). Bu dosya [dependency-map](../00-overview/dependency-map.md) §2'nin katalog aynasidir; iki dosya celisirse dependency-map kazanir.

```text
+---------------------------------------------------+
| matris  : 8 x 8 = 64 hucre                        |
| deger   : OK (izinli) | X (yasak) | - (ayni katman)|
| zincir  : l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0  |
| acik    : gelen l3 sutunu = X + DOGRULAMA GEREKLI |
+---------------------------------------------------+
```

| Olcut | Deger | Kaynak |
|---|---|---|
| Hucre | 64 (20 OK · 40 X · 4 -) | dependency-map §2 |
| Kapali dogrudan kenar | `l5->l4` · `l4->l2` · `l2->l4` | ADR-028 2.1 + 2.3/2 |
| Giden `l3` | yalniz `l0` OK, digerleri X | ADR-028 2.1 |
| Gelen `l3` | X + DOĞRULAMA GEREKLI | ADR-028 sessizdir |
| authority | derived (ayna dosya) | bu dosya |

---

## §1 Okuma Kurali

| Terim | Anlam |
|---|---|
| Referans | `ProjectReference` veya `using` ile baska katman tipine bakmak |
| OK | izinli kenar; NetArchTest kuralinda serbest |
| X | yasak kenar; ihlal = NetArchTest RED |
| `-` | ayni katman ici; assembly siniri yok |

Yon kurali: **satir = bagimli olan (from), sutun = bagimli olunan (to).** Ters ok (`ProjectReference` yukari) her kosulda yasak.

---

## §2 Yon Matrisi (8 x 8)

| satır \ sutun | l0 | l1 | l2 | l3 | l4 | l5 | l6 | l7 |
|---|---|---|---|---|---|---|---|---|
| **l0** | - | X | X | X | X | X | X | X |
| **l1** | OK | - | X | X | X | X | X | X |
| **l2** | OK | OK | - | X | X | X | X | X |
| **l3** | OK | X | X | - | X | X | X | X |
| **l4** | OK | OK | X | X | - | X | X | X |
| **l5** | OK | OK | OK | X | X | - | X | X |
| **l6** | OK | OK | OK | X | OK | OK | - | X |
| **l7** | OK | X | OK | X | X | OK | OK | - |

**Sutun ozeti:** `l0` = l1..l7 hepsi · `l1` = l2,l4,l5,l6 · `l2` = l5,l6,l7 · `l3` = hicbiri · `l4` = l6 · `l5` = l6,l7 · `l6` = l7 · `l7` = hicbiri.

---

## §3 Kilit Hucreler

| # | Hucre | Karar | Gerekce |
|---|---|---|---|
| 1 | `l4 -> l0` | OK | portlar `l0`'a yazilir, uygulamasi `l4`'te (ADR-028 2.3/6) |
| 2 | `l5 -> l4` | X | tool cagrisi `l0` portundan gecer (ADR-028 2.3/2) |
| 3 | `l4 -> l2` | X | zincirdeki `l4 -> l2` **siradir, kenar degildir** (ADR-028 2.3/2) |
| 4 | `l2 -> l4` | YOK | repository portu `l0`'da; iki yonlu akis dogurur (ADR-028 2.1) |
| 5 | gelen `l3` (l1/l2/l4/l5/l6/l7 -> l3) | X + DOĞRULAMA GEREKLI | ADR-028 gelen oklara sessizdir |

Madde 5 yuzunden bu dosya `authority: derived` + `status: needs-validation` tasir; yeni ADR maddesi gelince once dependency-map §2.1 guncellenir, sonra bu dosya onu tekrarlar.

---

## §4 Ihlal Denetimi

```powershell
# K1 - PROJECTREFERENCE GRAFI: cikti §2 matrisiyle birebir olmali
Get-ChildItem 'C:\versacoder\src' -Recurse -Filter *.csproj |
  ForEach-Object { $f = $_.BaseName
    Select-String -Path $_.FullName -Pattern 'ProjectReference Include="([^"]+)"' -AllMatches |
    ForEach-Object { $_.Matches } | ForEach-Object {
      '{0} -> {1}' -f $f, [IO.Path]::GetFileNameWithoutExtension($_.Groups[1].Value) } }
```

| Komut | Beklenen |
|---|---|
| K1 (graf) | kenar listesi §2 matrisiyle birebir |
| K2 (yasak using) | 0 satir ya da acik `DIZIN YOK` |
| K3 (EF / concat SQL / TFM) | 0 satir ya da acik `DIZIN YOK` |
| NetArchTest `LayerRulesTests` | 0 failure |

`src/` dizini su an yoksa cikti `DIZIN YOK` olur; **PASS yazilmaz** -> DOĞRULAMA GEREKLI. Tarama yolu gercek proje adini icerir (`src/VersaCoder.Domain/`); genis kalip `src/**/Domain/**` sahte PASS uretir (ADR-028 2.3/5).

---

## §5 Kirmizi Cizgiler

| # | Yasak | Kırilrsa |
|---|---|---|
| R1 | `l0` hicbir `VersaCoder.*` namespace'ine `using` yapamaz | CRITICAL |
| R2 | `l3` yalniz `l0`'a bakar | CRITICAL |
| R3 | `l1` `System.Data` / Dapper / dosya / DevExpress gormez | CRITICAL |
| R10 | `ProjectReference` ters (yukari) olamaz | CRITICAL |

Tam liste (R1-R10) [layer-boundary-check](../../checklists/layer-boundary-check.md) 13 sorusunda yasar; bu dosya katalog aynasidir, SSOT degildir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Hucre sayisi | 64 | §2'de 8 x 8 |
| 2 | Lejant | 3 deger (OK / X / -) | §1 |
| 3 | Kilit hucre | 5 satir | gelen `l3` acik |
| 4 | authority | derived | §0 |
| 5 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §7 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu matrise girmez; tek iz `reports/k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- NetArchTest kural dosyasi bu dosya degil; `LayerRulesTests` bu tabloya gore yazilir.
- Katman adlari, N araliklari ve proje eslemesi [node-inventory](node-inventory.md) dosyasindadir.

---

## §8 Ilgili Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [dependency-map](../00-overview/dependency-map.md) | matrisin SSOT'u + denetim komutlari |
| 2 | [mimari index](../index.md) | 8 makro katman ozeti |
| 3 | [catalog index](index.md) | 52 dugum + N <-> klasor eslemesi |
| 4 | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | karar kaynagi (§2.3 madde 2) |
| 5 | [layer-boundary-check](../../checklists/layer-boundary-check.md) | 13 soru denetim listesi |

---

## §9 Kanit

```powershell
# 1) matris veri satiri - beklenen: 8
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\layer-matrix.md' -Pattern '^\| \*\*l[0-7]\*\*').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\layer-matrix.md').Length
# 3) yasak Unicode kutu karakteri (U+2500-U+257F) - beklenen: 0
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\layer-matrix.md' -Pattern ('[' + (((0x2500..0x257F) | ForEach-Object { [char]$_ }) -join '') + ']')).Count
```

Olculen degerler `check-links.ps1` (bozuk link 0) ve `measure-vault.ps1` (NoFrontmatter 0 · Mojibake 0) ile birlikte raporlanir.
