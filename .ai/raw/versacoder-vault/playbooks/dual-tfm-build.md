---
title: "dual-tfm-build - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "net48 + net10.0-windows ikili bacak build - komutlar, kanit ciktisi, TFM gerekceleri ve geri alma"
---

# dual-tfm-build — İki Bacak Build

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](../.rules/coding-standards.md) · [dual-tfm-test](dual-tfm-test.md)

> **Tek cumlelik sorumluluk:** *Kod iki bacakta da derleniyor mu, kaniti hangi ciktidir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek `TargetFramework` yetmez: her proje `net48` **ve** `net10.0-windows` icin ayri ayri derlenmeli (G6). Kanit = **iki ayri build ciktisi, ikisi de 0 hata**; tek bacagin yeşili "bitti" degildir (G5). Platform disi kod `#if`'e girmez; `#if` yalniz API/platform farki icerir. Temiz build + iki bacak ≈10 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Proje/COZUNLUK var | `Get-ChildItem C:\versacoder -Recurse -Include *.sln,*.csproj` |
| 2 | `TargetFrameworks` iki bacakli | `.csproj` -> `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>` |
| 3 | SDK + DevExpress v26.1 kurulu | `dotnet --list-sdks` + `Directory.Packages.props` tek surum |
| 4 | EF Core referansi yok | `Select-String -Path '*.csproj','*.props' -Pattern EntityFramework` -> 0 satir |
| 5 | Onceki kirmizi build kaydi | hata/dosya listesi (daraltma icin) |

---

## §2 Adimlar

**Toplam ≈10 dk** · yapan: `developer`

1. **Temiz build al** — once tam derleme; hata varsa adim 2'ye gecmeden ilk hatayi duzelt.
   - Komut: `dotnet build C:\versacoder\<sln> -v m`
   - Cikti: `0 Hata` + `bin\Debug\net48\` ve `bin\Debug\net10.0-windows\` ciktilari

2. **Bacak bacagi dogrula** — toplu build gizleyebilir; her TFM tek tek istenir.
   - Komut: `dotnet build C:\versacoder\<sln> -f net48 -v m`
   - Komut: `dotnet build C:\versacoder\<sln> -f net10.0-windows -v m`
   - Cikti: her komutta `0 Hata` + kendi `bin\...\<tfm>\` klasoru

3. **TFM gerekcelerini tara** — `#if` icinde is mantigi varsa RED (yalniz platform/API farki).
   - Komut: `Select-String -Path 'src\**\*.cs' -Pattern '#if\s+(NET48|!NET48|NET10)'`
   - Cikti: yalniz platform/API satirlari; is mantigi = 0 satir

4. **Saglik taramasi** — DevExpress surum/ribbon ve klonlama kurali tek bakista.
   - Komut: `Select-String -Path '*.csproj','Directory.Packages.props' -Pattern 'DevExpress\.(\d+\.\d+)'`
   - Cikti: tek surum `26.1`; `RibbonControlStyle` yalniz `Office2010`

5. **Kaniti kaydet** — iki bacak ciktisi rapora/devre paketine yapistirilir; "gecti" yazimi yetmez.
   - Dosya: `.ai/reports/` ilgili rapor + devre paketi (`KANIT` maddesi)
   - Cikti: iki cikti satiri (net48 / net10.0-windows), ikisi `0 Hata`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Bacak 1 | `dotnet build -f net48` -> `0 Hata` + `bin\Debug\net48\*.dll` |
| K2 | Bacak 2 | `dotnet build -f net10.0-windows` -> `0 Hata` + `bin\Debug\net10.0-windows\*.dll` |
| K3 | Cift TFM | `.csproj` -> `net48;net10.0-windows` (iki de yazili) |
| K4 | `#if` disiplini | tarama -> yalniz platform/API satirlari |
| K5 | Surum | DevExpress tek surum `26.1`, ribbon `Office2010` |

**Ornek cikti (K1+K2 - zorunlu):**

```text
PS> dotnet build C:\versacoder\VersaCoder.sln -f net48 -v m
Derleme tamamlandı. 0 Hata. 0 Uyarı.
  VersaCoder.UI -> C:\versacoder\src\VersaCoder.UI\bin\Debug\net48\VersaCoder.UI.dll

PS> dotnet build C:\versacoder\VersaCoder.sln -f net10.0-windows -v m
Derleme tamamlandı. 0 Hata. 0 Uyarı.
  VersaCoder.UI -> C:\versacoder\src\VersaCoder.UI\bin\Debug\net10.0-windows\VersaCoder.UI.dll
```

> **NOT (2026-09-25):** `C:\versacoder` altinda henuz `*.sln`/`*.csproj` **YOK**; ilk iskelet olusturulunca bu playbook gercek cikti uretir - `NEEDS_VALIDATION`.

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Build yanlisi (kod) | son yeşil commit'e `git revert <sha>` + tekrar iki bacak build |
| 2 | TFM'i tek bacak yapma denemesi | `.csproj`'i `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>`'e dondur |
| 3 | `#if` eklendi | `#if` bloklarini kaldir, platform farkini ayri dosyaya tasi |
| 4 | Paket surumu degisti | `Directory.Packages.props`'i `git checkout --` ile eski surume don (G8 onayi) |
| 5 | `bin/obj` bozuk | `Get-ChildItem -Recurse -Directory -Filter bin \| Remove-Item -Recurse` + build (G7 onayi) |

