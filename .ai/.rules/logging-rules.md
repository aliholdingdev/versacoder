---
title: "Loglama Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Log seviyesi, tek satır yapısal format, ne loglanır/ne loglanmaz (secret-PII yasağı), hata+stack biçimi, log dosyası döngüsü ve üretimde seviye politikası"
---

# Loglama Kuralları (`.rules/logging-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [security-rules.md](security-rules.md) · [error-handling-rules.md](error-handling-rules.md) · [configuration-rules.md](configuration-rules.md) · [performance-guidelines.md](performance-guidelines.md)

> **Tek cümlelik sorumluluk:** *Hangi olay hangi seviyede ve hangi biçimde loglanır, ne asla loglanmaz* sorusunun tek cevabıdır.
> Log **içeriği** kuralları bu dosyada, log **gizliliği** (secret-PII) [security-rules.md](security-rules.md) ile birlikte okunur; çelişirse güvenlik kazanır.

---

## §0 Özet (Answer First)

Log tek satır **yapısal** olaydır (anahtar + değer), seviye gerçek önemine göre verilir; **secret, connection string, prompt içeriği ve kişisel veri asla loglanmaz**, istisna yalnız `Error`'da maskelenmiş veridir — log hattı UI thread'ini bloklamaz ve dosya döner (rolling), ihlali `reviewer` RED eder, secret sızıntısı **CRITICAL**'dır.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| L1 | Seviye önemle verilir | `Debug` ayrıntı · `Info` dönüm noktası · `Warning` geri kazanılabilir · `Error` işlem kaybı | her yerde `Error` ya da her şey `Debug` | `reviewer` |
| L2 | Olay tek satır, anahtar-değer | `Log.Information("SessionClosed {SessionId} {ElapsedMs}", id, ms)` | birleşik cümle + `+` ile kurulan metin | `reviewer` |
| L3 | Secret / PII loglanmaz | maskelenmiş `Server=...;DB=app` | tam connection string, API anahtarı, prompt metni | `reviewer` (**CRITICAL**) |
| L4 | Hata logu exception + stack, mesaj tek | `Log.Error(ex, "Kayit {Id} eklenemedi", id)` | yalnız `ex.Message` ya da `ex.ToString()` UI'a | `reviewer` |
| L5 | Log hattı UI thread'ini bloklamaz | asenkron sink / kuyruk (B3 ≤ 50 ms) | UI'da `File.WriteAllText` ile log | `reviewer` |
| L6 | Dosya döner ve sınırlıdır | günlük dosya + 7 gün saklama | tek büyüyen `app.log` | `architect` |
| L7 | Ortam seviyesi sabittir | Production `Info`, Development `Debug` | Production'da `Trace` (G5'e aykırı gürültü) | `architect` |
| L8 | Kullanıcıya görünen metin log değildir | resmi UI mesajı + ayrı olay | `MessageBox` metnini loga yapıştırmak | `reviewer` |
| L9 | Testte log içeriği assert edilmez | `ILogger` substitute ile **çağrı sayısı** | dosya okuyup metin karşılayan test | `tester` |
| L10 | Bağlam sabittir, elle tekrar yok | `ForContext("Layer", "l4")` | her satırda dosya yolu/metin tekrarı | `reviewer` |

---

## §2 İstisnalar (ADR gerekir mi?)

| # | İstisna durumu | ADR gerekir mi | Onay |
|---|---|---|---|
| E1 | Üretimde `Warning`/`Error` içeriğinde tanımlayıcı (id) maskelenmeli mi | Hayır — id loglanır, veri gövdesi loglanmaz | `reviewer` |
| E2 | Teşhis için kısa süre `Debug` açılması | Hayır — ortam bayrağı + zaman sınırı | `architect` |
| E3 | Uçtan uca log toplayıcı (ELK/Seq) eklenmesi | **Evet** — yeni bağımlılık = ADR (G8) | `architect` + kullanıcı |
| E4 | Log'a olay verisi (transcript parçası) eklenmesi | **Evet** — gizlilik + boyut gerekçesi | `architect` + `security` incelemesi |
| E5 | 3. parti logunun kapatılması/süzülmesi | Hayır — yalnız seviye filtresi | `architect` |

---

## §3 Kanıt

| # | Kontrol | Komut / Dosya | Beklenen |
|---|---|---|---|
| K1 | Secret sızıntısı taraması | `Select-String -Path logs -Pattern "Password|ApiKey|ConnectionString"` | 0 eşleşme |
| K2 | Biçim tutarlılığı | `Select-String -Path src -Pattern "Log(Error|Warning|Information)"` | yer tutuculu (template) çağrılar |
| K3 | Ham yazım yasağı | `Select-String -Path src -Pattern "Console.WriteLine|Debug.WriteLine"` | yalnız test/araç kodu |
| K4 | Dosya döngüsü | `logs/` envanteri + boyut | tek dosya sınırlı, tarihli adlar |
| K5 | Bütçe | stopwatch logu (B3) | log yazımı ≤ 50 ms |

- Secret taraması CRITICAL bulursa iş durur: `reviewer` → `developer` (≈10 dk SLA), kayıt `reports/` altına.
- Log formatı değişikliği `configuration-rules.md` ile birlikte değerlendirilir (ortam bayrağı).
- Bilinmeyen davranış `NEEDS_VALIDATION` olarak işaretlenir; log üzerinden tahmin yürütülüp "gördük" denmez (G5).

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect).
