---
title: "logging-observability-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Loglama ve gozlemlenebilirlik denetimi icin 13 kanitli soru: seviye, secret, korelasyon, hata kapsama, DI kaydi"
---

# logging-observability-check — Loglama / Gozlemlenebilirlik Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Hata yasandiginda log'dan ne oldugu anlasilabiliyor mu, log secret sizdiriyor mu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Gozlemlenebilirlik iki kurala sigar: **olay log'da gorunur** (ne, ne zaman, hangi katman) ve **log secret/PII icermiyor**. Kanit = log cagrisinin dosya:satır ornegi + tarama ciktisi. `catch` blogunda sessiz yutma (`catch { }`) **0** olmalidir; yutulan hata gozlemlenemez.

| Alan | Deger |
|---|---|
| Kim doldurur | `developer` (kod) · `reviewer` (denetim) |
| Kanit paketi | 3 grep ciktisi + 1 log ornegi |
| Kritik seviyeler | `Error`/`Critical` her zaman loglanir |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Sessiz yutma (`catch { }` / `catch { return; }`) **0** mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'catch\s*(\w+\s*\w*\s*)?\{?\s*(return;)?\s*\}'` = 0 satir.
- [ ] 2. Her `catch` blogunda log ya da yeniden firlatma var mi (0 eksik)?
  **Kanit:** `catch` sayisi ile `Log`/`throw` sayisi karsilastirmasi (2 rakam).
- [ ] 3. Log'da **secret** geciyor mu (password/token/connection string = 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Log[^;]*(password|token|ConnectionString|apikey)'` = 0 satir.
- [ ] 4. Log'da **PII** (e-posta, kullanici adi, sorgu degeri) var mi (0 ya da maskeli)?
  **Kanit:** 1 ornek log cagrisi + maskeleme notu.
- [ ] 5. Seviye kullanimi mantikli mi (`Debug` uretimde kapali, `Error` her yerde acik)?
  **Kanit:** seviye dagilimi (her seviye icin kullanim sayisi) + yapilandirma satiri.
- [ ] 6. Log mesaji **durumu** anlatiyor mu (deger degil olay)? — "Bosluk gecti" gibi generic mesaj 0?
  **Kanit:** 5 ornek log mesaji + her biri icin yeterlilik notu.
- [ ] 7. Hata mesajinda **korelasyon id** var mi (istek/is akisi takibi)?
  **Kanit:** korelasyon parametresinin 1 ornegi (dosya:satır) ya da "YOK (planla)".
- [ ] 8. Log dosyasi rotasyonu / boyut siniri tanimli mu?
  **Kanit:** yapilandirma satiri (boyut/gun) ya da "YOK (planla)".
- [ ] 9. Log yolu uygulama veri klasorunde mi (proje klasorune yazma 0)?
  **Kanit:** log yolu uretim kodunun 1 satiri + `Get-ChildItem` ile dogrulama.
- [ ] 10. DI kayitlarinda log servisi **tek noktadan** mi (saglayici bagimliligi 0)?
  **Kanit:** `Program.cs`/composition root'taki log kaydi satiri + `Select-String 'ILogger'` da tek kaynak notu.
- [ ] 11. Kritik yollar (SQL, migration, startup) log'lanmis mi (en az 1 satir her biri icin)?
  **Kanit:** 3 ornek dosya:satır listesi.
- [ ] 12. Log uygulamanin **performans butcesini** kiriyor mu (log icin senkron I/O 0)?
  **Kanit:** log yazim cagrisinin asenkron/notu + olcum (`≈` sure) ya da `NEEDS_VALIDATION`.
- [ ] 13. Testlerde log saglayicisi mock'lanmis mi (test gercek dosya yazmiyor)?
  **Kanit:** testte log mock kullaniminin 1 ornegi.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Sessiz yutma → hata gozlemez, **HIGH**, kod RED |
| 2 | Soru 3 | Log'da secret → **CRITICAL** (secret-leak), anahtar yenilenir |
| 3 | Soru 4 | Log'da PII → gizlilik ihlali, maskeleme zorunlu |
| 4 | Soru 9 | Log proje klasorune yaziliyorsa dagitimda kaybolur/kirilir |
| 5 | Soru 5 | Uretimde `Error` kapaliysa hata gorunmez → gozlemlenebilirlik RED |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Secret'in saklanmasi ve repo sizintisi | [secret-leak-check.md](secret-leak-check.md) |
| Genel guvenlik denetimi | [security-review-checklist.md](security-review-checklist.md) |
| Performans butcesi (log maliyeti disi) | [performance-budget-check.md](performance-budget-check.md) |
| DI/katman siniri (composition root) | [layer-boundary-check.md](layer-boundary-check.md) |
| Vault `log.md` kayit defteri (guncelleme) | [documentation-check.md](documentation-check.md) |
