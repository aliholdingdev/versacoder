---
title: "sql-injection-review - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "SQL enjeksiyonu incelemesi icin 13 kanitli soru: parameterized sorgu, allowlist, dinamik adres, EF Core yasagi"
---

# sql-injection-review — SQL Enjeksiyon Incelemesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Her sorgu `@ad` parametresiyle mi gidiyor, dinamik adres (tablo/kolon adi) allowlist'ten mi geciyor* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kural tektir: **gövde sabit, deger `@ad`**. `+`, `$"..."`, `string.Format` ile SQL kurmak `G2` ihlalidir → CRITICAL ve build RED; EF Core ise **hiçbir kosulda** kullanilamaz. Parametrelenemeyen tek sey **adres**dir (tablo/kolon adi) — o da sabit dallara bolunmus allowlist ile cozulur. Inceleme `reviewer` tarafindan yapilir, kanit calistirilmis tarama ciktisidir.

| Alan | Deger |
|---|---|
| Kim doldurur | `reviewer` · duzeltme `developer` |
| Kanit paketi | 3 grep ciktisi + 1 ornek dosya |
| Kaynak | `.ai/knowledge/data/parameterized-queries.md` kalip tablosu |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. String-concat SQL **0** mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id|Name)|string\.Format\([^)]*SELECT'` = 0 satir.
- [ ] 2. `$"SELECT ... {x}"` biciminde interpolasyon **0** mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '\$"SELECT|\$"UPDATE|\$"DELETE'` = 0 satir.
- [ ] 3. EF Core referansi **0** mi?
  **Kanit:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` = 0 satir.
- [ ] 4. Her sorgu `const string` ya da sabit alan uzerinden mi gidiyor (plan yeniden kullanilir)?
  **Kanit:** en az 1 repository dosyasi + satir numarasi (gövde tanimi).
- [ ] 5. Her sorguda parametre anonim nesne ya da `DynamicParameters` ile mi geciyor?
  **Kanit:** 1 ornek `QueryAsync` cagrisi (dosya:satır) + parametre adlari.
- [ ] 6. `IN` listesi elle mi kuruluyor (`IN (1,2,3)` = yasak)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'IN\s*\(\s*@|IN\s*\(\s*\d'` ciktisi; Dapper `IN @Ids` kullaniminin 1 ornegi.
- [ ] 7. `LIKE` degeri kullanici girdisiyle mi kalip birlestiriyor (`"%" + term` yalniz parametre icinde mi)?
  **Kanit:** LIKE kullaniminin tum satirlari (dosya:satır) + `@Pattern` atamasi.
- [ ] 8. `ORDER BY` kolon adi parametre yapilmaya calisilmis mi (yapilamaz — allowlist mi)?
  **Kanit:** siralama kodunun tam blogu + sabit dal sayisi (en az 2).
- [ ] 9. Tablo/kolon adi **allowlist** (`HashSet.Contains` / `Array.IndexOf`) ile mi sinirli?
  **Kanit:** allowlist taniminin dosya:satır alintisi + disi girdi testi.
- [ ] 10. Limit/offset degeri `@Take`/`@Skip` parametresi mi (`"LIMIT " + n` yok)?
  **Kanit:** sayfalama cagrisinin 1 ornegi.
- [ ] 11. Her komutta `commandTimeout: DbTimeouts.Query` set edilmis mi?
  **Kanit:** timeout aramasinin sayisi = komut sayisi (rakamla).
- [ ] 12. Sorgu yalniz repository (Infrastructure) icinde mi, form/serviste SQL **0** mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'SELECT |INSERT ' | Where path -notlike *Infrastructure*` = 0 satir.
- [ ] 13. Enjeksiyon testi var mi (parametre girdisiyle `'; DROP` benzeri deneme reddediliyor)?
  **Kanit:** test adi + beklenti satiri (hata ya da bos sonuc).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Concat/interpolasyon satiri > 0 → **G2 CRITICAL**, kod yeniden yazilir |
| 2 | Soru 3 | EF Core referansi > 0 → **G2 CRITICAL**, paket kaldirilir |
| 3 | Soru 8 veya 9 | Adres allowlist'siz → injection kanali, HIGH |
| 4 | Soru 12 | SQL repository disinda → katman + G2 cift ihlali |
| 5 | Soru 13 | Enjeksiyon testi yok → kanitsiz "guvenli" beyani (G5) |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Diger guvenlik maddeleri (secret, dosya, log) | [security-review-checklist.md](security-review-checklist.md) |
| Secret/sifre saklama tasarimi | [secret-leak-check.md](secret-leak-check.md) |
| Sorgu performansi, timeout butcesi | [performance-budget-check.md](performance-budget-check.md) |
| Migration'da SQL dosyasi bicimi | [migration-check.md](migration-check.md) |
| Parametreli sorgu kalip tablosu (bilgi) | [../CLAUDE.md](../CLAUDE.md) §1 G2 · [../spec/test-strategy.md](../spec/test-strategy.md) |
