---
title: "devexpress-surum-gecisi - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress Universal 2026 / v26.1 tek surum gecisi - paket kilidi, Office2010 ribbon, iki bacak kanit ve geri alma"
---

# devexpress-surum-gecisi — DevExpress v26.1 Geçişi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](../.rules/coding-standards.md) · [nuget-surum-yukseltme](nuget-surum-yukseltme.md)

> **Tek cumlelik sorumluluk:** *DevExpress surumu neden tek (v26.1) ve gecis nasil kanitlanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Vault'ta DevExpress **tek surum: Universal 2026 / v26.1** (ADR-004). Ikinci bir surum satiri G3 ihlalidir; ribbon stili **yalniz** `RibbonControlStyle.Office2010` (`Office2007`, `Office2010Silver` vb. yasak). Gecis = kilit dosyasi + kontrol ailesi taramasi + **iki bacak build/test**. Ribbon/kontrol API degisikligi cikarsa kod `v26.1`'e hizalanir, surum dusurulmez (G8 onayi ile). ≈45 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | ADR-004 okunmus | `.ai/.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md` |
| 2 | Kurulum mevcut | DevExpress Universal 2026 (v26.1) makinede kurulu |
| 3 | Kilit dosyasi | `Directory.Packages.props` tek DevExpress satiri |
| 4 | Kural dosyasi okunmus | `.ai/.rules/devexpress-rules.md` |
| 5 | Kullanici onayi (G8) | `log.md`'de onay satiri |

---

## §2 Adimlar

**Toplam ≈45 dk** · yapan: `developer` (inceleme: `reviewer`)

1. **Kilit + kurulum dogrula** — surum nerede yazili, kurulu surum ne; ikisi ayni olmali.
   - Komut: `Select-String -Path 'C:\versacoder\Directory.Packages.props','C:\versacoder\*.csproj' -Pattern 'DevExpress\.\w+ (\d+\.\d+)'`
   - Cikti: yalniz `26.1`; birden cok surum = G3 ihlali

2. **Ribbon stilini tara** — tek kabul edilen deger `Office2010`.
   - Komut: `Select-String -Path 'src\**\*.cs' -Pattern 'RibbonControlStyle\.\w+'`
   - Cikti: yalniz `Office2010`; `Office2007|Office2010Silver|...` = 0 satir

3. **Kontrol ailesini tara** — v26.1 disi referans (eski surum API'si, kaldirilmis property) var mi.
   - Komut: `Select-String -Path 'src\**\*.cs' -Pattern 'DevExpress\.' | Measure-Object`
   - Cikti: bulunan API'nin v26.1 karsiligi; bulunamayan = `NEEDS_VALIDATION`

4. **Iki bacak build + test** — surum gecisinin tek gecerli kaniti.
   - Komut: `dotnet build C:\versacoder\<sln> -f net48` + `-f net10.0-windows` (ayrica `dotnet test` ayni iki bacak)
   - Cikti: 2 x `0 Hata` + 2 x `Basarisiz: 0`

5. **Rapor + kayit** — G3 tarama ciktisi + build/test satirlari rapora; onay yoksa is durur.
   - Dosya: `.ai/reports/` + `.ai\log.md`
   - Cikti: `G3 OK: tek surum 26.1, ribbon Office2010`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Tek surum | tarama -> yalniz `26.1` |
| K2 | Ribbon | tarama -> yalniz `Office2010` |
| K3 | Build | 2 x `0 Hata` (net48, net10.0-windows) |
| K4 | Test | 2 x `Basarisiz: 0` |
| K5 | ADR tutarliligi | ADR-004 `status: accepted` + `hedef_bacak` dolu |

**Ornek cikti (K1+K2):**

```text
PS> Select-String -Path 'Directory.Packages.props' -Pattern 'DevExpress\.\w+ (\d+\.\d+)'
Directory.Packages.props:12: <PackageVersion Include="DevExpress.WinForms" Version="26.1.3" />
PS> Select-String -Path 'src\**\*.cs' -Pattern 'RibbonControlStyle\.\w+'
src\MainForm.Designer.cs:41: ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Surum satiri | `git checkout -- Directory.Packages.props` + `dotnet restore` |
| 2 | API duzeltmeleri (kod) | `git revert <sha>`; surum dusurme **G8 onayi** ister |
| 3 | Ribbon stili degisti | `Office2010`'a geri don + 2. build kaniti |
| 4 | Kurulum farki (makine) | DevExpress 2026 kurulumunu geri al; vault koduna dokunma |
| 5 | Onay kaydi | `log.md`'ye `REVERT <tarih>` satiri ekle |

