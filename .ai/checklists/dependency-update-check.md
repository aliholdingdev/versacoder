---
title: "dependency-update-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Bagimlilik guncelleme oncesi 12 kanitli soru: G8 onayi, CPM tek nokta, iki bacak uyumu, guvenlik notu"
---

# dependency-update-check — Bagimlilik Guncelleme Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu paket surumu degisikligi onayli mi, CPM'te tek satirda mi, iki bacakta da derleniyor mu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Yigin disina cikan **her** paket/surum degisikligi **G8 = kullanici onayi** ister; onaysiz "daha modern" guncelleme `reviewer` tarafindan RED edilir. DevExpress **yalniz 26.1**, ORM **yok** (EF Core yasak), surumler yalniz `Directory.Packages.props` (CPM) uzerinden degistirilir. Kanit = CPM diff'i + iki bacak build ciktisi.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (deneme) · `architect` (karar) |
| Onay | **kullanici** (G8) — ajan tek basina surum degistiremez |
| Kanit paketi | CPM diff + `dotnet build` ×2 |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Degisiklik **G8** olarak raporlandi mi ve kullanici onayi var mi?
  **Kanit:** `log.md` satiri (tarih · paket · eski → yeni · onay veren).
- [ ] 2. Degisiklik yalniz `Directory.Packages.props` uzerinden mi yapildi (proje basina surum **0**)?
  **Kanit:** `Select-String -Path **\*.csproj -Pattern 'Version='` = 0 satir + CPM dosyasinin 1 satiri.
- [ ] 3. DevExpress hala tek satir **26.1** mi (ikinci surum satiri yok)?
  **Kanit:** `Select-String -Path Directory.Packages.props -Pattern 'DevExpress'` ciktisi (1 satir).
- [ ] 4. EF Core paketi eklenmemis mi (0 satir)?
  **Kanit:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` = 0 satir.
- [ ] 5. Test/xUnit/Dapper gibi tanimli yigin disina yeni paket girmis mi (0 olmali)?
  **Kanit:** CPM dosyasinin eski/yeni diff'i; yeni satir varsa gerekce satiri.
- [ ] 6. Guncelleme **iki bacakta** derleniyor mu?
  **Kanit:** `dotnet build -f net48` ve `-f net10.0-windows` ciktilari (ikisi `0 Hata`).
- [ ] 7. Testler guncelleme sonrasi hala yesil mi?
  **Kanit:** `dotnet test` ozet satiri (`Gecen: N · Basarisiz: 0`).
- [ ] 8. Surum kilidi (lock) dosyasi guncellendi mi (CPM + `.csproj` tutarliligi)?
  **Kanit:** `dotnet restore` ciktisi = 0 NU1605/NU1102 uyari/hatasi.
- [ ] 9. Kaldirilan paketin yerine kalan `using` kirligi var mi (0 olmali)?
  **Kanit:** build ciktisinda `CS8019`/kullanilmayan using uyari sayisi.
- [ ] 10. Guvenlik/CVE notu alindi mi (her guncelleme icin 1 satir)?
  **Kanit:** rapor satiri: `paket · eski · yeni · gerekce (CVE/uyumluluk/performans)`.
- [ ] 11. Ilgili ADR acildi mi (yeni teknoloji ya da yigin degisikligi icin)?
  **Kanit:** `.decisions/accepted/ADR-NNN-*.md` dosya yolu + `status: accepted`.
- [ ] 12. Geri alma plani yazildi mi (eski surume donus adimi)?
  **Kanit:** raporda 1 satirlik rollback: hangi satir geri alinir, kac dakikada donulur.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | Kullanici onaysiz surum degisikligi → **G8 ihlali**, degisiklik geri alinir |
| 2 | Soru 3 veya 4 | DevExpress ≠ 26.1 ya da EF Core satiri → **G3/G2 CRITICAL** |
| 3 | Soru 6 | Tek bacak build kirmizi → **G6**, guncelleme reddedilir |
| 4 | Soru 2 | Proje basina surum yazildi → CPM ihlali, tek noktaya tasinir |
| 5 | Soru 5 | Tanimli yigin disi paket → ADR + tekrar onay gerekir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| NuGet gecmis/restore sagligi tek basina | [nuget-package-check.md](nuget-package-check.md) |
| Iki bacak derleme ayrintisi (`#if`, TFM) | [dual-tfm-build-check.md](dual-tfm-build-check.md) |
| CVE/izlenebilirlik denetimi (derin) | [dependency-update-check.md](dependency-update-check.md) §soru 10 · [../CLAUDE.md](../CLAUDE.md) §13 |
| DevExpress surum/kontrol denetimi | [devexpress-control-check.md](devexpress-control-check.md) |
| ADR bicimi ve defteri | [adr-completeness-check.md](adr-completeness-check.md) |
