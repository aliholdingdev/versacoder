---
title: "performance-budget-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Performans butcesi icin 13 kanitli soru: tick suresi, N+1, timeout, sanal mod, allocation, olcum kaniti"
---

# performance-budget-check — Performans Butcesi Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Olculmus bir butce var mi, asim varsa sayiyla mi raporlandi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

"Yeterince hizli" **olcum degildir**: bu liste yalniz **sayi + butce** kabul eder (süre daima `≈` + birim ile yazilir). Kanit = calistirilmis olcum ciktisi (tick, sure, satir sayisi) ya da `NEEDS_VALIDATION`. Butce asimi is durdurmaz ama **raporlanmak zorundadir** (G11 benzeri seffaflik).

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (olcum) · `reviewer` (rapor) |
| Kanit bicimi | `butce · olculen · durum` (3 rakam) |
| Kritik yol | UI tick ≤ 100 ms · sorgu ≤ 500 ms |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Bu is icin **yazili butce** var mi (sayi + birim)?
  **Kanit:** rapor/belge satiri: `olcu: <deger> ms/sn` — yoksa "YOK (planla)".
- [ ] 2. Olcum **gercekten yapildi** mi (tahmin degil)?
  **Kanit:** olcum ciktisi alintisi (Stopwatch / profilleyici / satir sayisi).
- [ ] 3. UI tick'inde **100 ms'den uzun** is var mi (0 olmali)?
  **Kanit:** en yavas handler olcumu (rakam + dosya:satır).
- [ ] 4. Ayni sorgu dongu icinde tekrarlanıyor mu (N+1 = 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'foreach' ... QueryAsync` taramasi = 0 satir ya da dongu basina sorgu sayisi (rakam).
- [ ] 5. Buyuk listelerde `virtual`/sayfalama kullaniliyor mu (tamami yukleme 0)?
  **Kanit:** grid virtual mode ayari satiri + veri seti boyutu (rakam).
- [ ] 6. `commandTimeout` degeri makul mu (sonsuz = 0)?
  **Kanit:** `DbTimeouts` taniminin degerleri (rakamlarla).
- [ ] 7. Senkron `async` olmayan I/O var mi (`.Result`/`.Wait()` = 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '\.Result|\.Wait\(\)'` = 0 satir.
- [ ] 8. Gereksiz allocation (dongu ici `new` + string concat) var mi (tespit edilen 0)?
  **Kanit:** ilgili dosya:satır listesi ya da "0 adim".
- [ ] 9. Cache kullanilan yerde **TTL/invalidation** tanimli mi?
  **Kanit:** cache cagrisinin 1 ornegi + TTL degeri (rakam).
- [ ] 10. Dosya/AG erisimi batch mi (dosya basina 1 cagri yerine toplu okuma)?
  **Kanit:** ilkli okuma cagrisinin 1 ornegi + cagri sayisi (rakam).
- [ ] 11. Kritik olcum **iki bacakta** da yapildi mi (fark varsa rakamla)?
  **Kanit:** `net48` ve `net10` icin 2 sure (rakam + birim).
- [ ] 12. Butce asimi **raporlandi** mi (gizlenmis asim 0)?
  **Kanit:** rapor satiri: `butce: X · olculen: Y · asim: ±Z`.
- [ ] 13. `NEEDS_VALIDATION` olan olcumler isaretli mi (tahmin yok)?
  **Kanit:** etiketli satir sayisi + hangi olcum (adlari).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 2 | Olcumsuz "hizli" beyani → **G5**, madde silinir |
| 2 | Soru 3 veya 7 | 100 ms'den uzun UI isli / `.Result` → HIGH, kod RED |
| 3 | Soru 4 | N+1 sorgu → HIGH, repository seviyesinde duzeltilir |
| 4 | Soru 6 | Sonsuz timeout → veritabani kilidi riski, HIGH |
| 5 | Soru 12 | Gizlenmis butce asimi → **G11**, is kapatilmaz |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Sorgunun dogru/parametreli olmasi | [data-access-check.md](data-access-check.md) · [sql-injection-review.md](sql-injection-review.md) |
| Test kapsamı ve coverage | [coverage-gate-check.md](coverage-gate-check.md) |
| Bagimlilik/CPM denetimi | [nuget-package-check.md](nuget-package-check.md) |
| Gunluk kod incelemesi | [code-review-daily.md](code-review-daily.md) |
| Butce ve sure raporlama formati | [../WORKFLOW.md](../WORKFLOW.md) §10 |
