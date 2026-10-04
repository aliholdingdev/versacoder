---
title: "nuget-surum-yukseltme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "NuGet paket surumu yukseltme - Directory.Packages.props tek kilit, G8 onayi, iki bacak build/test kaniti ve geri alma"
---

# nuget-surum-yukseltme — NuGet Surum Yukseltme

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [keys](../keys.md) · [dual-tfm-build](../playbooks/dual-tfm-build.md) · [devexpress-surum-gecisi](../playbooks/devexpress-surum-gecisi.md)

> **Tek cumlelik sorumluluk:** *Bir paket surumu nasil yukseltilir, kim onaylar, geri nasil alinir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Surum degisikligi **G8 = kullanici onayi** ister; `architect` karari bile olsa sessiz yapilmaz. Tek kilit noktasi `Directory.Packages.props` (CPM); proje dosyasina surum yazmak yasaktir. Yukseltme sonrasi **iki bacak build + test** kanitlanmadan is bitmez (G5). DevExpress bu playbook'un disindadir (ayri playbook). Toplam ≈30 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Kilit dosyasi mevcut ve tek kaynak | `C:\versacoder\Directory.Packages.props` |
| 2 | Guncel surum listesi alinmis | `dotnet list package --outdated` ciktisi |
| 3 | Kullanici onayi (G8) | `log.md`'de onay satiri |
| 4 | Onceki build/test yeşil | dual-tfm-build §5 + dual-tfm-test §5 |
| 5 | Cekirdek surum kilitleri okunmus | [keys](../keys.md) §2 (v26.1, SqlClient, Dapper) |

---

## §2 Adimlar

**Toplam ≈30 dk** · yapan: `developer` · onay: kullanici

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Eski surumu listele | `dotnet list <sln> package --outdated` -> `Paket / Aktif / En yeni` | 3 dk |
| 2 | Kilit dosyasini duzelt | proje dosyalarinda `Version=` -> `0 satir` | 5 dk |
| 3 | Onay al (G8) | `log.md` -> `G8 ONAY: <paket> <eski> -> <yeni>` | 5 dk |
| 4 | Restore + iki bacak build | `dotnet restore` + iki TFM build -> 2 x `0 Hata` | 10 dk |
| 5 | Test + kayit | iki TFM test -> 2 x `Basarisiz: 0` + rapor satiri | 7 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | `NU1008` | Surum proje dosyasinda kalmis | Tek kilit dosyaya tasinir, restore tekrar |
| 2 | Uyumsuzluk build'de cikti | Cekirdek bagimlilik ezismesi | Kilit dosyada tasinan paket de kontrol edilir |
| 3 | Onay yok, is devam etti | G8 atlanmis | Is durur, onay satiri yazilmadan devam edilmez |
| 4 | Tek bacakta test gecti | Filtre/TFM atlanmis | §2 adim 5 iki bacakta tekrarlanir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Kilit dosyasi | `git checkout -- Directory.Packages.props` |
| 2 | Proje dosyasi satirlari | `git revert <sha>` (tek dosya geri alinmaz, tutarsizlik kalir) |
| 3 | Restore artefaktlari | `dotnet nuget locals all --clear` + yeniden restore |
| 4 | Build/test kirildi | eski surume don + iki bacak kanit yeniden alinir |
| 5 | G8 onay kaydi | `log.md`'ye `REVERT <tarih>` satiri eklenir |

---

## §5 Kabul Kriteri

- [ ] Surum yalniz kilit dosyada; proje dosyalarinda `Version=` **0 satir**
- [ ] `log.md`'de `G8 ONAY` satiri tarihli ve kapatilmis
- [ ] `dotnet build` -> 0 hata (net48 + net10.0-windows)
- [ ] `dotnet test` -> 0 basarisiz (iki bacak)
- [ ] `dotnet list package --outdated` hedef paketi artik gostermiyor

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : restore -> 0 NU1008                      |
|          build -> 2 x 0 Hata (net48 + net10.0)    |
|          test  -> 2 x Basarisiz=0                 |
| DOSYA  : Directory.Packages.props                 |
| EKSIK  : YOK                                       |
| SONRAKI : kilit dosyasi commit'i (~5 dk)          |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Tek kilit | `Select-String -Path '*.csproj' -Pattern 'Version='` | 0 satir |
| 2 | Eski surum | `dotnet list <sln> package --outdated` | hedef paket yok |
| 3 | Vault | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈30 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | DevExpress surumu | [devexpress-surum-gecisi](../playbooks/devexpress-surum-gecisi.md) |
| 2 | Yeni paket/ORM ekleme (ilk kez) | G8 karari -> [adr-index](../.decisions/adr-index.md) |
| 3 | Build hatasi kok nedeni | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |
| 4 | Test stratejisi | [spec index](../spec/index.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `keys.md` | cekirdek surum kilitleri (v26.1, SqlClient, Dapper) |
| 2 | `checklists/nuget-package-check.md` | paket kontrol sorulari |
| 3 | `checklists/dependency-update-check.md` | bagimlilik guncelleme kontrol listesi |
| 4 | `WORKFLOW.md` | G8 onay kurali ve hata modlari (§4) |
