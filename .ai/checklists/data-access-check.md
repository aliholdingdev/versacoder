---
title: "data-access-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Veri erisimi denetimi icin 14 kanitli soru: ADO.NET + Dapper, parametre, timeout, provider soyutlama, transaction"
---

# data-access-check — Veri Erisimi (ADO.NET + Dapper) Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Veri erisimi tamamen ADO.NET + Dapper mi, her sorgu parametreli ve time'li mi, provider soyutlamasi saglam mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Veri katmani **ADO.NET + Dapper** ince katmandir; ORM **yoktur** (G2). Her sorgu `@ad` parametresiyle gecer, her komutta `commandTimeout` set edilir, sorgu yalniz repository'de yasar. Kanit = grep ciktilari + 1 ornek repository dosyasi (dosya:satır). Incelemeyi `reviewer`, duzeltmeyi `developer` yapar.

| Alan | Deger |
|---|---|
| Kim doldurur | `reviewer` · duzeltme `developer` |
| Yigin | ADO.NET + Dapper · `Microsoft.Data.Sqlite` + `Microsoft.Data.SqlClient` |
| Kanit paketi | 4 grep ciktisi + 1 dosya ornegi |
| Soru sayisi | 14 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. EF Core referansi **0** mi (proje + paket)?
  **Kanit:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` = 0 satir.
- [ ] 2. String-concat SQL **0** mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id|Name)|\$"SELECT'` = 0 satir.
- [ ] 3. Her sorgu `const string`/sabit alan uzerinden mi (plan yeniden kullanimi)?
  **Kanit:** 1 repository dosyasinda gövde taniminin dosya:satır alintisi.
- [ ] 4. Her sorguda parametre gecildi mi (anonim nesne / `DynamicParameters`)?
  **Kanit:** en az 3 `QueryAsync`/`ExecuteAsync` cagrisinin parametre blogu.
- [ ] 5. `CommandTimeout` her komutta set edilmis mi (eksik 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'commandTimeout'` sayisi = komut cagrisi sayisi (2 rakam).
- [ ] 6. Connection `using` ile mi kapatiliyor / `await using` mi (sizinti 0)?
  **Kanit:** 1 ornek `await using var conn = ...` satiri + `Select-String 'new SqliteConnection'` sayisi ile `using` sayisi esitligi.
- [ ] 7. Sorgu yalniz repository (Infrastructure) icinde mi (UI/Application'ta SQL **0**)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'SELECT |INSERT ' | Where path -notlike *Infrastructure*` = 0 satir.
- [ ] 8. Provider soyutlama (`ISqlDialect` benzeri) var mi ve iki provider da kayitli mi?
  **Kanit:** interface tanimi + `Sqlite`/`SqlClient` kayitlarinin dosya yollari.
- [ ] 9. Sayfalama `OFFSET/FETCH` ya da `LIMIT` olarak provider'a gore mi veriliyor?
  **Kanit:** dialo metodunun 1 ornegi + iki bacak cagrisi.
- [ ] 10. Transaction kullanilan yerlerde `Commit` + `Rollback` (hata yolunda) var mi?
  **Kanit:** 1 transaction blogu ornegi + `catch` icinde rollback.
- [ ] 11. Mappin'ler elle mi (`QueryAsync<T>` + kolon adlari) — uydurma kolon yok mu?
  **Kanit:** 1 mapping blogu + kolon adlarinin SQL ile eslesmesi.
- [ ] 12. Hata loglanirken connection string / parametre degeri sızıyor mu (0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Log[^;]*(ConnectionString|@p1)'` = 0 satir.
- [ ] 13. Testler gercek SQLite schema üzerinde mi ve EF Core'suz mu?
  **Kanit:** test `.csproj` paket listesi (`Microsoft.Data.Sqlite` var, EF Core yok).
- [ ] 14. `.sql/` dosyalari `.sql/README.md` ile eslesiyor mu (migration dahil)?
  **Kanit:** `.sql/` klasor listesi ile README tablosunun karsilastirmasi (fark = 0).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | EF Core referansi → **G2 CRITICAL**, build RED |
| 2 | Soru 2 | String-concat SQL → **G2 CRITICAL**, parametreli sorguya cevrilir |
| 3 | Soru 5 | `CommandTimeout` eksik → HIGH, sonsuz bekleme riski |
| 4 | Soru 7 | SQL repository disinda → katman + G2 cift ihlali |
| 5 | Soru 8 | Provider soyutlama yoksa iki bacak testi imkansiz → mimari RED |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Enjeksiyon ozel incelemesi (IN/LIKE/ORDER BY) | [sql-injection-review.md](sql-injection-review.md) |
| Migration dosyasi ve version tablosu | [migration-check.md](migration-check.md) |
| Secret / connection string saklama | [secret-leak-check.md](secret-leak-check.md) |
| Sorgu performansi ve timeout butcesi | [performance-budget-check.md](performance-budget-check.md) |
| Katman yonu (Infrastructure → Domain) | [layer-boundary-check.md](layer-boundary-check.md) · [../CLAUDE.md](../CLAUDE.md) §17 |
