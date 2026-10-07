---
title: "nuget-surum-yukseltme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "NuGet paket surumu yukseltme - Directory.Packages.props tek kilit, iki bacak build/test kaniti, G8 onayi ve geri alma"
---

# nuget-surum-yukseltme — NuGet Sürüm Yükseltme

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](../.rules/coding-standards.md) · [devexpress-surum-gecisi](devexpress-surum-gecisi.md)

> **Tek cumlelik sorumluluk:** *Bir paket sürümü nasıl yükseltilir, kim onaylar, geri nasıl alınır* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sürüm değişikliği **G8 = kullanıcı onayı** ister (yeni teknoloji/sürüm eklemek `architect` karar bile olsa). Tek kilit noktası `Directory.Packages.props` (CPM); proje başına sürüm yazmak yasak. Yükseltme sonrası **iki bacak build + test** kanıtsız iş bitmez. DevExpress hariç (ayrı playbook). ≈30 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Kilit dosyasi mevcut ve tek kaynak | `C:\versacoder\Directory.Packages.props` |
| 2 | Guncel surum listesi alinmis | `dotnet list package --outdated` ciktisi |
| 3 | Kullanici onayi (G8) | `log.md`'de onay satiri |
| 4 | Onceki build/test yeşil | `dual-tfm-build.md` + `dual-tfm-test.md` kaniti |
| 5 | Cekirdek surum kilitleri okunmus | `.ai/keys.md` §2 (v26.1, SqlClient 5.2.x, Dapper) |

---

## §2 Adimlar

**Toplam ≈30 dk** · yapan: `developer` (onay: kullanici)

1. **Eski surumu listele** — ne yukseltilecegi tahminle degil ciktiyla belli.
   - Komut: `dotnet list C:\versacoder\<sln> package --outdated`
   - Cikti: `Paket | Aktif | En yeni | Kaynak` tablosu

2. **Kilit dosyasini duzelt** — yalniz `Directory.Packages.props`; `.csproj`'te surum satiri varsa kaldir.
   - Komut: `Select-String -Path '*.csproj' -Pattern 'Version='` -> 0 satir
   - Cikti: tek dosyada tek surum; `ManagePackageVersionsCentrally` = true

3. **Onay al (G8)** — surum degisikligi kapsam/disiligi kullaniciya sorulur, sessiz yapilmaz.
   - Dosya: `.ai\log.md` -> `G8 ONAY: <paket> <eski> -> <yeni>`
   - Cikti: onay satiri (yoksa is durur)

4. **Restore + iki bacak dogrulama** — uyumsuzluk ilk derlemede cikar.
   - Komut: `dotnet restore C:\versacoder\<sln>` + `dotnet build ... -f net48` + `-f net10.0-windows`
   - Cikti: 2 x `0 Hata`

5. **Test + kayit** — iki bacak test, sonra kilit dosyasi commit'i ve rapor.
   - Komut: `dotnet test C:\versacoder\<sln> -f net48` + `-f net10.0-windows`
   - Cikti: `Basarisiz: 0` iki bacakta + raporda satir

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Tek kilit | surum yalniz `Directory.Packages.props`'ta, `.csproj`'te 0 satir |
| K2 | G8 onayi | `log.md`'de `G8 ONAY` satiri |
| K3 | Build | 2 x `dotnet build` -> `0 Hata` |
| K4 | Test | 2 x `dotnet test` -> `Basarisiz: 0` |
| K5 | Eski surum kalmadi | `dotnet list package --outdated` -> hedef paket listede yok |

**Ornek cikti (K1+K5):**

```text
PS> Select-String -Path 'src\**\*.csproj' -Pattern 'Version='
(sonuc yok) -> 0 satir
PS> dotnet list C:\versacoder\VersaCoder.sln package --outdated
  Dapper  2.1.66  (en guncel)   ->  hedef paket listede yok
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Kilit dosyasi | `git checkout -- Directory.Packages.props` |
| 2 | PackageReference satirlari | `git revert <sha>` (dosyayi tek basina geri alma, tutarsizlik kalir) |
| 3 | Restore artefaktlari | `dotnet nuget locals all --clear` + yeniden `dotnet restore` |
| 4 | Build/test kirildi | eski surume don + `dual-tfm-build`/`dual-tfm-test` kanitini yeniden al |
| 5 | G8 onay kaydi | `log.md`'ye `REVERT <tarih>` satiri ekle |

