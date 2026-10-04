---
title: "layer-boundary-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Katman sinirlari icin 13 kanitli soru: bagimlilik yonu, circular dependency, NetArchTest, #if, code-behind siniri"
---

# layer-boundary-check — Katman Siniri Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Yedi katmanin bagimlilik yonu tek yonlu mu, dairesel bag var mi, `#if` ve code-behind sinirlari korunuyor mu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kabul edilen yon degismez: `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`; her katman yalniz `interface` uzerinden `l0`'a baglanir. Kanit iki ayri kaynaktan gelir: **NetArchTest** (otomatik) + **`using` taramasi** (elle); ikisinden biri kirmizi ise mimari RED. Incelemeyi `architect` yapar, sonucu `reviewer` bulgu raporuna isler.

| Alan | Deger |
|---|---|
| Kim doldurur | `architect` (sozlesme) · `reviewer` (bulgu) |
| Kanit paketi | NetArchTest ciktisi + `using` taramasi |
| Yon | `l0 ← l1 ← l2 ← l3/l4 ← l5 ← l6 ← l7` |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. NetArchTest **0 ihlal** mi veriyor (yon + circular)?
  **Kanit:** `dotnet test --filter Category=Architecture` ciktisi: `0 failure`.
- [ ] 2. `l0-abstractions` hicbir seye bagimli degil mi?
  **Kanit:** proje `ProjectReference` listesi (bos ya da yalniz test paketleri).
- [ ] 3. `l1-domain` yalniz `l0`'a bagimli mi (Infrastructure/UI/Protocol `using` i 0)?
  **Kanit:** `Select-String -Path src\**\Domain\**\*.cs -Pattern 'using .*Infrastructure|using .*UI'` = 0 satir.
- [ ] 4. `l2-application` Infrastructure'i **TANIMIYOR** mu?
  **Kanit:** ayni tarama `Application` klasoru uzerinde = 0 satir.
- [ ] 5. `l4-infrastructure` Domain'i **tanimaz**, yalniz interface'ini uygular mi?
  **Kanit:** `Select-String -Path src\**\Infrastructure\**\*.cs -Pattern 'using .*\.Domain[^.]*;'` ciktisi + yorum.
- [ ] 6. `l7-ui` dogrudan `Infrastructure` ya da `Domain` kullanmiyor mu?
  **Kanit:** `UI` klasorunde bu iki `using` in 0 eslesmesi.
- [ ] 7. Composition root disinda `Host`'a baglilik var mi (0 olmali)?
  **Kanit:** `l6` disindaki dosyalarda `using .*Host` aramasi = 0 satir.
- [ ] 8. Dairesel bag var mi (A → B → A)?
  **Kanit:** NetArchTest `NoCircularDependencies` sonucu `0 failure`.
- [ ] 9. `#if` yalnizca platform/API farkinda mi (is mantagi yok)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '#if\s+(NET48|NET10)'` ciktisi + her satirin platform notu.
- [ ] 10. Code-behind (`.Form.cs`) yalniz olay yonlendirici mi (≤ 100 satir)?
  **Kanit:** en buyuk 3 `.Form.cs` dosyasinin satir sayisi (rakamla).
- [ ] 11. SQL yalniz `l4-infrastructure` (repository) icinde mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'SELECT |INSERT |UPDATE ' | Where path -notlike *Infrastructure*` = 0 satir.
- [ ] 12. `DateTime.Now` / `File.*` / `Environment.*` saf katmanlarda kullanilmis mi (0 olmali)?
  **Kanit:** `l1` + `l2` uzerinde bu cagrilari aratan tarama = 0 satir.
- [ ] 13. Inceleme sonucu `.ai/reports/**` altina yazildi mi?
  **Kanit:** rapor dosyasi + `0 CRITICAL · 0 HIGH` satiri.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 8 | NetArchTest > 0 ihlal → mimari RED, kod `developer`'a doner |
| 2 | Soru 5 veya 6 | Infrastructure/Domain sizmasi → HIGH, katman sozlesmesi ihlali |
| 3 | Soru 9 | `#if` icinde is mantagi → **G6 ihlali** |
| 4 | Soru 11 | SQL disari sizdi → **G2 ihlali** (repository disinda sorgu) |
| 5 | Soru 13 | Kanitsiz "mimari temiz" beyani → **G5**, inceleme gecersiz |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Asil kod incelemesi (bulgu seviyeleri) | [code-review-daily.md](code-review-daily.md) |
| Sorgularin parametreli olmasi | [sql-injection-review.md](sql-injection-review.md) |
| Iki bacak derleme sonucu | [dual-tfm-build-check.md](dual-tfm-build-check.md) |
| DI kayitlarinin dogrulugu | [logging-observability-check.md](logging-observability-check.md) §DI |
| Katman tanimlari ve N01..N52 dugumleri | [../CLAUDE.md](../CLAUDE.md) §17 · [../spec/test-strategy.md](../spec/test-strategy.md) |
