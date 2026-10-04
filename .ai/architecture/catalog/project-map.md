---
title: "Proje Haritasi - 9 Uretim + 1-2 Test Projesi"
type: catalog
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "9 uretim + 1-2 test projesi, N araliklari, cift TFM (net48;net10.0-windows) ve yeni .csproj kapilari (ADR-028 2.1)"
---

# Proje Haritasi - 9 Uretim + 1-2 Test Projesi

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [layer-matrix](layer-matrix.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *Kac proje var, her projede hangi N araligi yasiyor ve TFM nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**9 uretim + 1-2 test = en fazla 11 proje; her proje cift TFM `net48;net10.0-windows` tasir => toplam en fazla 22 derleme hedefi.** Proje adi tek yazim `VersaCoder.Crosscutting` (NC-02'deki `CrossCutting` yazimi iptal). Alt katmanlar (N01-N44) **klasor/namespace** duzeyindedir, birer proje degildir.

```text
+---------------------------------------------------+
| uretim projesi : 9   test projesi : 1-2           |
| toplam hedef   : en fazla 11 x 2 TFM = 22         |
| TFM            : net48 ; net10.0-windows          |
| yeni .csproj   : yalniz 3 kapidan gecer           |
+---------------------------------------------------+
```

| Olcut | Deger |
|---|---|
| Uretim projesi | 9 |
| Test projesi | 1-2 (en fazla 11 toplam) |
| TFM | `net48` + `net10.0-windows` |
| Derleme hedefi | 22 (11 x 2) |
| Sayim birimi | `| N##-### |` kayit satiri |

---

## §1 Proje Tablosu

| # | Proje (.csproj) | Katman | Alt N | Makro | TFM |
|---|---|---|---|---|---|
| 1 | `VersaCoder.Abstractions` | l0 | N01-N05 | N45 | net48;net10.0-windows |
| 2 | `VersaCoder.Domain` | l1 | N06-N11 | N46 | net48;net10.0-windows |
| 3 | `VersaCoder.Application` | l2 | N12-N18 | N47 | net48;net10.0-windows |
| 4 | `VersaCoder.Crosscutting` | l3 | N19-N24 | N48 | net48;net10.0-windows |
| 5 | `VersaCoder.Infrastructure` | l4 | N25-N32 | N49 | net48;net10.0-windows |
| 6 | `VersaCoder.Protocol` | l5 | N33-N37 | N50 | net48;net10.0-windows |
| 7 | `VersaCoder.Host` | l6 | N38-N40 | N51 | net48;net10.0-windows |
| 8 | `VersaCoder.Desktop` | l7 | N41-N44 | N52 | net48;net10.0-windows |
| 9 | `VersaCoder.Cli` | l7 | N44 (ortak) | - | net48;net10.0-windows |
| 10 | `VersaCoder.Tests` | test | - | - | net48;net10.0-windows |
| 11 | `VersaCoder.ArchitectureTests` | test (gerekirse) | - | - | net48;net10.0-windows |

**N44 iki projede yasar:** `VersaCoder.Desktop/Settings/` + `VersaCoder.Cli/`; ortak sozlesme l0'da tanimlanir, Desktop ve Cli baglanir (ADR-028 §4, junior #3 notu).

---

## §2 Yeni .csproj Kapilari

| # | Kapı | Soru | Gecemezse |
|---|---|---|---|
| 1 | Tek cumle sorumluluk | Bu proje tek cumleyle anlatilabilir mi? | isim `Helper`/`Common` ise RED |
| 2 | Bagimlilik yonu | Ok, [layer-matrix](layer-matrix.md) §2 ile ayni mi? | ters kenar = CRITICAL |
| 3 | Ayri TFM/paket seti | Ayri derleme hedefi gercekten gerekli mi? | ortak TFM ile coz |

Yeni `.csproj` bu 3 kapidan gecmeden olusturulmaz; K3'un 9 Tier / 618 DLL envanteri **kapatilmis bir alternatiftir** (ADR-028 §3/A3: 575 csproj x 2 TFM ~1150 hedef, UNIVERSAL RED).

---

## §3 TFM ve Paketler

| # | Kalem | Karar | Kaynak |
|---|---|---|---|
| 1 | TFM | `net48;net10.0-windows` iki bacak | ADR-028 §2.1 |
| 2 | Test projeleri | cift TFM (net48 kolu korunur) | ADR-028 2.3/6 |
| 3 | Paket versiyonlari | yalniz `Directory.Packages.props` (CPM) | ADR-028 2.3/1 |
| 4 | Test paketleri | `NetArchTest.Rules`, `xunit`, `Microsoft.NET.Test.Sdk`, `NSubstitute` | ADR-028 2.3/1 |
| 5 | DevExpress | yalniz v26.1 (K3 uyumu) | dependency-map §4 K3 |

`dotnet restore` bekleneni: **0 NU1008** (ADR-028 2.3/1 kaniti). Ucuncu bir TFM eklenirse K3'teki CPM/VS/restore catlamasi tekrarlar.

---

## §4 Katman Yonu (ozet)

| # | Kural | Deger |
|---|---|---|
| 1 | Insa sirasi | `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` |
| 2 | `l3` | yalniz `l0`'a bakar |
| 3 | `l2` <-> `l4` | dogrudan ok YOK (repository portu `l0`'da) |
| 4 | Ters ok | hicbir projede `ProjectReference` yukari yone olamaz |

Tam 8x8 tablo [layer-matrix](layer-matrix.md) §2'dedir; denetimi NetArchTest `LayerRulesTests` yapar.

---

## §5 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Proje satiri | 9 uretim + 1-2 test | §1 |
| 2 | TFM sapmasi | 0 (yalniz iki bacak) | §3 |
| 3 | Yeni .csproj | 3 kapi | §2 |
| 4 | `dotnet restore` | 0 NU1008 | planli |
| 5 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §6 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu haritaya girmez; tek iz `reports/k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- `src/` altindaki gercek `.csproj` dosyalari henuz yok; §1 tablosu ADR-028 §2.1 kararidir -> **DOĞRULAMA GEREKLI**.
- Klasor duzeyindeki N eslemesi [node-inventory](node-inventory.md) §2'dedir.

---

## §7 Proje Disi Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [catalog index](index.md) | 52 dugum + batch durumu |
| 2 | [node-inventory](node-inventory.md) | N <-> klasor eslemesi |
| 3 | [layer-matrix](layer-matrix.md) | 8x8 yon matrisi |
| 4 | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | 9 proje + 6 zorunlu duzeltme |
| 5 | [layer-boundary-check](../../checklists/layer-boundary-check.md) | 13 soru denetimi |

---

## §8 Kanit Komutlari

```powershell
# 1) gercek proje dosyalari - dizin yoksa 0
(Get-ChildItem 'C:\versacoder\src' -Recurse -Filter *.csproj -ErrorAction SilentlyContinue).Count
# 2) TFM sapmasi - beklenen: 0 satir (dizin yoksa DIZIN YOK)
Get-ChildItem 'C:\versacoder\src' -Recurse -Filter *.csproj -ErrorAction SilentlyContinue |
  Select-String -Pattern '<TargetFrameworks?>(?!net48|net10\.0-windows)'
```

Dizin yokken cikti bos kalir; **PASS yazilmaz** -> DOĞRULAMA GEREKLI (ADR-028 2.3/5).

---

## §9 Kanit

```powershell
# 1) proje satiri - beklenen: 11
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\project-map.md' -Pattern '^\| 1?[0-9] \| `VersaCoder').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\project-map.md').Length
# 3) bozuk link - beklenen: 0
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1
```
