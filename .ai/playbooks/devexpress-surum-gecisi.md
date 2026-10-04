---
title: "devexpress-surum-gecisi - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DevExpress Universal 2026 / v26.1 tek surum gecisi - paket kilidi, Office2010 ribbon, iki bacak kanit ve geri alma"
---

# devexpress-surum-gecisi — DevExpress v26.1 Gecisi

**Zorunlu Baglantilar:** [playbooks index](index.md) · [rules index](../.rules/index.md) · [nuget-surum-yukseltme](../playbooks/nuget-surum-yukseltme.md) · [dual-tfm-build](../playbooks/dual-tfm-build.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *DevExpress surumu neden tek (v26.1) ve gecis nasil kanitlanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Vault'ta DevExpress **tek surum: Universal 2026 / v26.1**; ikinci bir surum satiri G3 CRITICAL'tir. Ribbon stili **yalniz** `RibbonControlStyle.Office2010` (`Office2007`, `Office2010Silver` ve digerleri yasak). Gecis = kilit dosyasi + ribbon taramasi + **iki bacak build/test**; API degisikligi cikarsa kod v26.1'e hizalanir, surum dusurulmez (G8 onayi). Toplam ≈45 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Karar dosyasi okunmus | `.decisions/accepted/` icindeki DevExpress karari |
| 2 | Kurulum mevcut | DevExpress Universal 2026 (v26.1) makinede kurulu |
| 3 | Tek kilit dosyasi | `Directory.Packages.props` icinde tek DevExpress satiri |
| 4 | Kural dosyasi okunmus | [rules index](../.rules/index.md) uzerinden devexpress kurallari |
| 5 | Kullanici onayi (G8) | `log.md`'de onay satiri |

---

## §2 Adimlar

**Toplam ≈45 dk** · yapan: `developer` · inceleme: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Kilit + kurulum dogrula | `Select-String -Path 'Directory.Packages.props' -Pattern 'DevExpress\.\w+ \d+\.\d+'` -> tek surum | 5 dk |
| 2 | Ribbon stilini tara | `Select-String -Path 'src\**\*.cs' -Pattern 'RibbonControlStyle\.\w+'` -> yalniz `Office2010` | 5 dk |
| 3 | Kontrol ailesini tara | `Select-String -Path 'src\**\*.cs' -Pattern 'DevExpress\.'` -> v26.1 karsiligi var | 10 dk |
| 4 | Iki bacak build + test | `dotnet build` + `dotnet test` (net48 ve net10.0-windows) -> 2 x 0 hata, 2 x 0 basarisiz | 20 dk |
| 5 | Rapor + kayit | G3 tarama ciktisi + build/test satirlari `log.md`'ye | 5 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | 2'den fazla surum satiri | Proje bazli `Version=` kalmis | Proje dosyasindaki surum satiri silinir, tek kilit kalir |
| 2 | Ribbon stilinde baska deger | Kopya kod veya eski sablon | `Office2010`'a cevrilir, 2. build kaniti alinir |
| 3 | Kaldirilmis API hatasi | v26.1 disi referans | API v26.1 karsiligina cevrilir; surum dusurulmez |
| 4 | Tek bacakta yeşil | Build filtresi atlanmis | Her TFM ayri ayri tekrar kosulur |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Kilit dosyasi | `git checkout -- Directory.Packages.props` + `dotnet restore` |
| 2 | API duzeltmeleri (kod) | `git revert <sha>`; surum dusurme G8 onayi ister |
| 3 | Ribbon stili degisti | `Office2010`'a geri don + 2. build kaniti |
| 4 | Makine kurulumu | DevExpress 2026 kurulumu geri alinir; vault koduna dokunulmaz |
| 5 | Onay kaydi | `log.md`'ye `REVERT <tarih>` satiri eklenir |

---

## §5 Kabul Kriteri

- [ ] Surum taramasi **tek** `26.1` veriyor; baska surum satiri 0
- [ ] Ribbon taramasi yalniz `Office2010` veriyor
- [ ] `dotnet build` -> 0 hata (net48 + net10.0-windows)
- [ ] `dotnet test` -> 0 basarisiz (iki bacak)
- [ ] G8 onayi `log.md`'de tarihli olarak mevcut

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : surum -> tek 26.1 | ribbon -> Office2010 |
|          build -> 2 x 0 Hata | test -> 2 x 0 kaldi |
| DOSYA  : Directory.Packages.props, src/**         |
| EKSIK  : G8 onayi tarihli olarak log.md'de        |
| SONRAKI : devir paketiyle reviewer'a gec (~10 dk) |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Surum | `Select-String -Path 'Directory.Packages.props' -Pattern 'DevExpress'` | tek satir, 26.1 |
| 2 | Ribbon | `Select-String -Path 'src\**\*.cs' -Pattern 'RibbonControlStyle\.\w+'` | yalniz Office2010 |
| 3 | Build/Test | iki bacak ayri kosum | 2 x 0 hata / 2 x 0 basarisiz |
| 4 | Butce | adim tablosu | ≈45 dk (20 dk asiminda dur + rapor) |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | DevExpress disi paket yukseltme | [nuget-surum-yukseltme](../playbooks/nuget-surum-yukseltme.md) |
| 2 | Katman/proje kapisinin acilmasi | [yeni-katman-ekleme](../playbooks/yeni-katman-ekleme.md) |
| 3 | Ekran tasarimi ve wireframe | [wireframe-uretme](../playbooks/wireframe-uretme.md) |
| 4 | Surum kararinin kendisi | ADR (kayit: [adr-index](../.decisions/adr-index.md)) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `.rules/index.md` | devexpress kural dosyasina giden tek yol |
| 2 | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | katman/proje sinirlari ve 9 uretim projesi |
| 3 | `scripts/check-links.ps1` | link taramasi (§7 madde 1 oncesi) |
| 4 | `playbooks/index.md` | adim tablosu, sure ve geri alma kabul kriteri |
