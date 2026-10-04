---
title: "adr-completeness-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "ADR tamamlanma denetimi icin 13 kanitli soru: Nygard sablonu, 2 alternatif, sonuc +/-, hedef bacak, defter kaydi"
---

# adr-completeness-check — ADR Tamamlanma Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu ADR alternatifleri gercekten tartisti mi, sonucu +/- ile yazildi mi, deftere islenmis mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

ADR = **geri alinabilir kararin kaydi**; "boyle karar verdik" cumlesi degildir. Eksik ADR (tek alternatif, sonucusuz, bacasiz) `architect` DoD'unu kirar ve gate'te kanit sayilmaz. Kanit = ADR dosyasinin ilgili basliklarinin dolu olmasi + defter satiri + `.rules/` guncellemesi.

| Alan | Deger |
|---|---|
| Kim doldurur | `architect` · denetim `reviewer` |
| Sablon | `.templates/adr/*` (Nygard vb.) |
| Kanit paketi | ADR dosyasi + adr-index satiri + `.rules/` satiri |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. ADR dosyasi `.decisions/accepted/` altinda ve `status: accepted` mi?
  **Kanit:** dosya yolu + frontmatter `status` satiri.
- [ ] 2. ADR numarasi **benzersiz** mi (ayni NNN iki kez yok)?
  **Kanit:** `Get-ChildItem .ai\.decisions\accepted -Filter 'ADR-NNN*'` listesi; tekrar = 0.
- [ ] 3. Frontmatter **8 alan + `authority: SSOT`** dolu mu?
  **Kanit:** dosyanin ilk 10 satiri.
- [ ] 4. `hedef_bacak: "net48 + net10.0-windows"` alani **dolu** mu?
  **Kanit:** ilgili frontmatter satiri.
- [ ] 5. **En az 2 alternatif** yazilmis mi?
  **Kanit:** alternatif basliklarinin listesi (adet ≥ 2).
- [ ] 6. Her alternatifin **sonuc (+)** ve **sonuc (−)** kisimi var mi?
  **Kanit:** `+`/`−` (artı/eksi) satirlari; eksik alternatif = 0.
- [ ] 7. **Neden bu karar** (gerekce) yazilmis mi, "daha iyi" gibi genel ifade yok mu?
  **Kanit:** gerekce paragrafi + genel ifade araması (`Select-String 'daha iyi|daha modern'` = 0 satır).
- [ ] 8. Katman bagimlilik yonu ASCII diyagramda gorunuyor mu (Unicode kutu yok)?
  **Kanit:** diyagram blogu + `Select-String '[\u2500-\u257F]'` = 0 satir.
- [ ] 9. Sonuc, ilgili `.rules/` dosyasini **zorunlu** kiliyor mu?
  **Kanit:** `.rules/` dosyasindaki kural satiri (ADRsiz kural = 0).
- [ ] 10. `adr-index.md`'ye kayit eklendi mi?
  **Kanit:** defterdeki yeni satir (no · baslik · tarih · status).
- [ ] 11. `log.md`'ye tarihli karar kaydi dustu mu?
  **Kanit:** `log.md` satiri (tarih · ADR no · gerekce ozeti).
- [ ] 12. Karar `CLAUDE.md` §13 surum kilitleri ile **celisiyor** mu (celisiyorsa `CONFLICT DETECTED`)?
  **Kanit:** karsilastirma notu (0 celiski ya da etiket).
- [ ] 13. Ilgili checklist/prompts/playbook bu karari **referans** aliyor mu (bilgi tekrari yok)?
  **Kanit:** referans veren dosya adlari; ayni metnin iki dosyada tekrari = 0.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 5 veya 6 | Tek alternatif / sonucusuz → ADR **eksik**, DoD kirik |
| 2 | Soru 4 | `hedef_bacak` bos → G6 ihlali, ADR gecersiz |
| 3 | Soru 9 | `.rules/` guncellenmemis → karar baglayici degil (architect DoD) |
| 4 | Soru 10 veya 11 | Defter/log kaydi yok → izlenebilirlik yok, gate'te kanit sayilmaz |
| 5 | Soru 12 | Anayasayla celiski → **CONFLICT DETECTED**, kullaniciya gider |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| ADR **sablonunun** kendisi ve hangi sablonun secilecegi | [../CLAUDE.md](../CLAUDE.md) §4 · [../WORKFLOW.md](../WORKFLOW.md) §2.1 |
| Vault link/bicim denetimi geneli | [vault-link-check.md](vault-link-check.md) · [documentation-check.md](documentation-check.md) |
| Paket surumu kararlari (G8 onayi) | [dependency-update-check.md](dependency-update-check.md) |
| Test kapsamı kararlari | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| Aci acma adim adim akisi | [../spec/test-strategy.md](../spec/test-strategy.md) |
