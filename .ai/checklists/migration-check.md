---
title: "migration-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Migration denetimi icin 13 kanitli soru: saf SQL, schema_version, checksum, rollback, tekrar calisabilirlik"
---

# migration-check — Migration (Saf SQL + Version Tablosu) Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Migration dosyalari saf SQL mi, `schema_version` kaydi tutarli mi, geri alinabilir mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Migration = **saf SQL dosyasi + `schema_version` (veya `__Migrations`) tablosu**; EF `migrate` **yoktur** (G2). Uygulanmis bir migration dosyasi **sonradan degistirilemez** — degistiyse checksum sasar ve uygulama durur. Kanit = `.sql/` listesi + version tablosu kaydi + checksum taramasi.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (yazar) · `reviewer` (denetler) |
| Kaynak | `.sql/` + `.sql/README.md` |
| Kanit paketi | dosya listesi + SQL ciktisi + checksum |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Her migration **saf SQL** dosyasi mi (`.sql` uzantili, kod icinde SQL **0**)?
  **Kanit:** `Get-ChildItem .ai\.sql -Recurse -Filter *.sql` listesi + `Select-String -Path src\**\*.cs -Pattern 'CREATE TABLE'` = 0 satir.
- [ ] 2. Dosya adlandirmasi sirali mi (`0001_`, `0002_` … bosluk/atlama yok)?
  **Kanit:** dosya adlarinin sirali listesi; atlama varsa adi.
- [ ] 3. `schema_version` / `__Migrations` tablosu tanimli mi?
  **Kanit:** ilk migration dosyasinda `CREATE TABLE ... version` satiri.
- [ ] 4. Her dosya **tek seferde** calisir mi (tekrar calistirilinca hata vermemeli ya da korumali mi)?
  **Kanit:** `IF NOT EXISTS` / `IF OBJECT_ID` korumalarinin en az 1 ornegi.
- [ ] 5. Dosya **sonradan degistirilmis** mi (degistirilmemeli; yeni dosya acilir)?
  **Kanit:** `git log --follow --onsql <dosya>` ciktisi = 1 commit ya da "0 degisiklik".
- [ ] 6. Rollback / geri alma dosyasi ya da adimi var mi?
  **Kanit:** rollback dosyasi yolu ya da `.sql/README.md` icinde rollback adimi (1 satir).
- [ ] 7. `.sql/README.md` dosya listesi ile birebir eslesiyor mu?
  **Kanit:** karsilastirma tablosu (fark = 0).
- [ ] 8. DDL'de `DROP`/`TRUNCATE` var mi (varsa onay + yedek notu ile)?
  **Kanit:** `Select-String -Path .ai\.sql\*.sql -Pattern 'DROP TABLE|TRUNCATE'` ciktisi + `G7 ONAYI` satiri.
- [ ] 9. Migration betigi transaction icinde mi (yari-yarim kalmis schema 0)?
  **Kanit:** `BEGIN TRAN` / `COMMIT` eslesmesinin 1 ornegi.
- [ ] 10. Uygulama migration'i okuyan kod `l4-infrastructure` icinde mi (UI'da **0**)?
  **Kanit:** migration okuyucu dosyanin katmani + `Select-String -Path src\**\UI\**\*.cs -Pattern 'schema_version'` = 0 satir.
- [ ] 11. Veri tasima (INSERT ... SELECT) var mi, kayip sayisi kontrol edilmis mi?
  **Kanit:** ilgili dosya:satır + kontrol sorgusu (sonuc rakami) ya da "0 adim".
- [ ] 12. Testler migration sonrasi gercek schema ile calisiyor mu?
  **Kanit:** entegrasyon testi adi + `dotnet test` ozet satiri.
- [ ] 13. `.sql/README.md`'de surum + tarih + sorumlu var mi?
  **Kanit:** README'de `sürüm · tarih · sorumlu` tablosunun 1 satiri.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | Kod icinde `CREATE TABLE`/EF migrate → **G2 CRITICAL** |
| 2 | Soru 5 | Uygulanmis migration degistirilmis → checksum sasar, schema RED |
| 3 | Soru 8 | Onaysiz `DROP`/`TRUNCATE` → **G7 CRITICAL** |
| 4 | Soru 6 | Rollback adimi yoksa geri alinamaz migration → **G11** |
| 5 | Soru 3 | Version tablosu yoksa migration sirasi dogrulanamaz → mimari RED |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Sorgularin parametreli olmasi | [sql-injection-review.md](sql-injection-review.md) · [data-access-check.md](data-access-check.md) |
| Provider secimi (Sqlite vs SqlClient) | [data-access-check.md](data-access-check.md) §soru 8 |
| Secret / connection string | [secret-leak-check.md](secret-leak-check.md) |
| Paket surumu (EF referansi taramasi disi) | [nuget-package-check.md](nuget-package-check.md) |
| Migration tasarimi kurallari | [../CLAUDE.md](../CLAUDE.md) §4 · [../spec/test-strategy.md](../spec/test-strategy.md) |
