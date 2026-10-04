---
title: "dual-tfm-build - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "net48 + net10.0-windows ikili bacak build - komutlar, kanit ciktisi, TFM gerekceleri ve geri alma"
---

# dual-tfm-build — Iki Bacak Build

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [rules index](../.rules/index.md) · [dual-tfm-test](../playbooks/dual-tfm-test.md) · [gate-004a](../checklists/gate-004a-vault-kapanis.md)

> **Tek cumlelik sorumluluk:** *Kod iki bacakta da derleniyor mu, kaniti hangi ciktidir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek `TargetFramework` yetmez: her proje `net48` **ve** `net10.0-windows` icin ayri ayri derlenmelidir (G6). Kanit = **iki ayri build ciktisi, ikisi de 0 hata**; tek bacagin yeşili "bitti" degildir (G5). `#if` yalnizca API/platform farki icerir, is mantigi icine girerse RED. Build yeşil degilse test ve commit yok: GATE-004 oncesi bu playbook zorunludur. Temiz build + iki bacak ≈10 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Cozunluk/proje dosyalari var | `Get-ChildItem C:\versacoder -Recurse -Include *.sln,*.csproj` |
| 2 | `TargetFrameworks` iki bacakli | `.csproj` -> `net48;net10.0-windows` |
| 3 | SDK + DevExpress v26.1 kurulu | `dotnet --list-sdks` + kilit dosyada tek surum |
| 4 | EF Core referansi yok | `Select-String -Path '*.csproj','*.props' -Pattern EntityFramework` -> 0 satir |
| 5 | Onceki kirmizi build kaydi | hata/dosya listesi (daraltma icin) |

---

## §2 Adimlar

**Toplam ≈10 dk** · yapan: `developer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Temiz build al | `dotnet build C:\versacoder\<sln> -v m` -> `0 Hata` | 3 dk |
| 2 | Bacak bacagi dogrula | `-f net48` ve `-f net10.0-windows` ayri kosum -> her komutta 0 hata | 3 dk |
| 3 | TFM gerekcelerini tara | `Select-String -Path 'src\**\*.cs' -Pattern '#if\s+(NET48\|!NET48\|NET10)'` -> yalniz platform/API | 2 dk |
| 4 | Saglik taramasi (surum + ribbon) | `Select-String -Path 'Directory.Packages.props' -Pattern 'DevExpress'` -> tek 26.1 | 1 dk |
| 5 | Kaniti kaydet | iki cikti satiri rapora / devre paketinin `KANIT` maddesine | 1 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Tek bacakta hata | TFM yazilmamis ya da kod o API'yi bilmiyor | `.csproj` iki bacakli mi, kod `#if`'li mi bakilir |
| 2 | Toplu build yeşil, tek bacak kirmizi | Toplu kosum hatayi gizlemis | §2 adim 2 zorunlu tekrarlanir |
| 3 | `NU1008` restore hatasi | Test paketleri kilit dosyada degil | Kilit dosyaya `PackageVersion` eklenir |
| 4 | Uyari sayisi artmis | `NoWarn` ile gizleme denemesi | `NoWarn` satiri silinir, uyari acik kaliyor |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Build yanlisi (kod) | son yeşil commit'e `git revert <sha>` + iki bacak tekrar |
| 2 | TFM tek bacak yapma denemesi | `.csproj` eski `TargetFrameworks` satirina doner |
| 3 | `#if` eklendi | `#if` bloklari kaldirilir, platform farki ayri dosyaya tasinir |
| 4 | Paket surumu degisti | kilit dosya `git checkout --` ile eski surume (G8 onayi) |
| 5 | `bin`/`obj` bozuk | klasor temizligi + tekrar build (yikici islem, G7 onayi) |

---

## §5 Kabul Kriteri

- [ ] `dotnet build -f net48` -> `0 Hata` + `bin\Debug\net48\` ciktisi
- [ ] `dotnet build -f net10.0-windows` -> `0 Hata` + kendi `bin` klasoru
- [ ] `.csproj` icinde `net48;net10.0-windows` birlikte yazili
- [ ] `#if` taramasi is mantigi 0 satir
- [ ] DevExpress tek surum 26.1; ribbon `Office2010`

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : build -f net48        -> 0 Hata          |
|          build -f net10.0-windows -> 0 Hata       |
|          #if taramasi         -> platform/API satir|
|          surum taramasi       -> tek 26.1          |
| DOSYA  : <sln>, src/**/*.csproj                   |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Bacak 1 | `dotnet build <sln> -f net48 -v m` | `0 Hata` |
| 2 | Bacak 2 | `dotnet build <sln> -f net10.0-windows -v m` | `0 Hata` |
| 3 | Yasak tarama | `Select-String ... -Pattern EntityFramework` | `0 satir` |
| 4 | Butce | adim tablosu | ≈10 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Test kosumu ve coverage | [dual-tfm-test](../playbooks/dual-tfm-test.md) |
| 2 | Ilk commit atma sirasi | [WORKFLOW](../WORKFLOW.md) §2.3 (GATE-004) |
| 3 | Paket surumu yukseltme | [nuget-surum-yukseltme](../playbooks/nuget-surum-yukseltme.md) |
| 4 | Build hatasi kok nedeni | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `WORKFLOW.md` | GATE-004 kabul kriteri: 2 bacak, 0 hata, NU1008 |
| 2 | `checklists/gate-004-ilk-commit.md` | build kaniti + commit sirasi |
| 3 | `.rules/index.md` | kodlama/dual-TFM kural dosyalari |
| 4 | `playbooks/index.md` | adim, sure, geri alma kabul kriteri |
