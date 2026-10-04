---
title: "nuget-package-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "NuGet/CPM sagligi icin 12 kanitli soru: tek surum noktasi, restore uyarilari, kullanilmayan paket, lisans"
---

# nuget-package-check — NuGet / CPM Paket Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Her paket tek bir surumle mi kilitli, restore temiz mi, kullanilmayan paket var mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Depo **Central Package Management** (CPM) ile yonetilir: surum yalniz `Directory.Packages.props` icindedir, `.csproj` icinde `Version=` **0 satir** olmalidir. Tanimli yigin (WinForms · DevExpress · Dapper · Sqlite/SqlClient · xUnit · NSubstitute · NetArchTest) disina paket eklemek **G8 + kullanici onayi** ister. Kanit = restore ciktisi + grep ciktilari.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` |
| Kaynak dosya | `Directory.Packages.props` (tek surum noktasi) |
| Kanit paketi | `dotnet restore` · 2 grep ciktisi |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. `Directory.Packages.props` dosyasi mevcut ve `ManagePackageVersionsCentrally` acik mi?
  **Kanit:** dosyanin ilk 5 satiri + `ManagePackageVersionsCentrally` satiri.
- [ ] 2. Hicbir `.csproj`'te `Version=` yaziyor mu (0 satir olmali)?
  **Kanit:** `Select-String -Path **\*.csproj -Pattern '<Version>'` = 0 satir.
- [ ] 3. `dotnet restore` **0 hata** mi (NU1102/NU1603/NU1605 yok)?
  **Kanit:** restore ciktisinin son 3 satiri.
- [ ] 4. Tekrar eden paket (ayni ad, iki surum) var mi (0 olmali)?
  **Kanit:** CPM dosyasinda paket adlarinin `Group-Object` sonucu: en buyuk frekans 1.
- [ ] 5. DevExpress yalniz **tek** satir `26.1` mi?
  **Kanit:** `Select-String -Path Directory.Packages.props -Pattern 'DevExpress'` = 1 satir.
- [ ] 6. EF Core / ORM paketi var mi (0 satir olmali)?
  **Kanit:** `Select-String -Path Directory.Packages.props -Pattern 'EntityFramework|NHibernate|Dapper\.Extensions'` = 0 satir.
- [ ] 7. Paket adlari tanimli yigin icinde mi (yeni bir ad varsa gerekce + onay)?
  **Kanit:** CPM satirlari ile izinli liste karsilastirmasi; fark varsa `G8` kaydi.
- [ ] 8. Kullanilmayan paket var mi (`PackageReference` ama hic `using` yok)?
  **Kanit:** en az 1 paket icin `Select-String -Path src\**\*.cs -Pattern 'using <ad>'` = 0 satir ise paket kaldirilir.
- [ ] 9. Test paketleri yalniz `tests/**` altinda mi (uretim projesine sizmamis)?
  **Kanit:** `Select-String -Path src\**\*.csproj -Pattern 'xunit|NSubstitute|NetArchTest'` = 0 satir.
- [ ] 10. Paket lisanslari uyumlu mu (liste + istisna notu)?
  **Kanit:** rapor tablosu: `paket · lisans · durum` (istisna varsa `NEEDS_VALIDATION`).
- [ ] 11. Floating/`*` surum kullanimi var mi (0 olmali)?
  **Kanit:** `Select-String -Path Directory.Packages.props -Pattern '="\*|-\*'` = 0 satir.
- [ ] 12. Guncelleme/ekleme karari `log.md`'ye yazildi mi?
  **Kanit:** `log.md` satiri (tarih · paket · islem · onay).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 2 veya 4 | Proje basina surum / cift surum → CPM ihlali, restore kirmizi |
| 2 | Soru 3 | Restore hatasi (NU1102/NU1605) → build kilitlenir, GATE-004 acik |
| 3 | Soru 6 | EF Core/ORM satiri → **G2 CRITICAL** |
| 4 | Soru 5 | DevExpress ikinci surum satiri → **G3 CRITICAL** |
| 5 | Soru 7 | Yigin disi paket onaysiz → **G8**, paket kaldirilir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Tek bir paket guncelleme kararinin onay akisi | [dependency-update-check.md](dependency-update-check.md) |
| Iki bacak (`net48`/`net10`) derleme sonucu | [dual-tfm-build-check.md](dual-tfm-build-check.md) |
| CVE taramasi ve guvenlik denetimi | [security-review-checklist.md](security-review-checklist.md) |
| DevExpress surumu ve ribbon stili | [devexpress-control-check.md](devexpress-control-check.md) · [ribbon-style-check.md](ribbon-style-check.md) |
| Paket kilidi kararinin anayasasi | [../CLAUDE.md](../CLAUDE.md) §13 |
