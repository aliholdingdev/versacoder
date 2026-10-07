---
title: "Hata Yönetimi Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Hata yakalama, tip sözleşmesi (Result), kullanıcı mesajı, yeniden deneme/timeout, loglama ve taşıma sırası; UI ve katman başına sorumluluk"
---

# Hata Yönetimi Kuralları (`.rules/error-handling-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [logging-rules.md](logging-rules.md) · [security-rules.md](security-rules.md) · [async-rules.md](async-rules.md)

> **Tek cümlelik sorumluluk:** *Hata nerede yakalanır, nasıl adlandırılır, kullanıcıya nasıl döner, ne zaman yeniden denenir* sorusunun tek cevabıdır.
> Hata **içeriği ve biçimi** bu dosyada; hata **olayının loglanması** [logging-rules.md](logging-rules.md) ile birlikte okunur; gizlilik [security-rules.md](security-rules.md) kazanır.

---

## §0 Özet (Answer First)

Hata **yükseltilir, yutulmaz**: her katman yalnız **kendi diline çevirebildiği** hatayı yakalar (`catch` yalnız kendi sorumluluğunda), gerisini yukarı taşır. Kullanıcıya **asla** `ex.Message`/stack gösterilmez; dönen değer tip sözleşmesidir (`Result<T>` / tip’li hata kodu), `null` ve boş string yoktur. Yeniden deneme **yalnız** geçici ve idempotent hata içindir, üstel geri çekilir (3 deneme, 250→500→1000 ms) ve **3 başarısız düzeltmeden sonra kod yazmak yasaktır** (protokol §8.1) — o noktada durulur, **şüpheli varsayım adıyla** kullanıcıya escalite edilir. EF Core **yok**; veri katmanı hataları ADO.NET exception'larıdır.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| E1 | `catch` yalnız kendi sorumluluğunu sarar | Repository: `SqlException` → tip'li hata kodu | UI'da her yerde `catch (Exception)` | `reviewer` |
| E2 | Hata tip sözleşmesidir | `Result<User>` + `ErrorCode.NotFound` | `null` / `-1` / boş string ile "başarısız" | `reviewer` |
| E3 | Kullanıcıya stack/mesaj gitmez | `"Kayıt bulunamadı."` + log `evt=query.fail` | `MessageBox.Show(ex.ToString())` | `reviewer` |
| E4 | Yutma yasak (boş `catch`) | yakala → sınıflandır → dön/taşı | `catch { }` ya da `catch { return null; }` | `reviewer` (**HIGH**) |
| E5 | Yeniden deneme yalnız geçici + idempotent'te | timeout/`SqlConnection` kapanma → 3 deneme | `DuplicateKeyException`'ı tekrar dene | `tester` |
| E6 | Üstel geri çekilme zorunlu | 250 → 500 → 1000 ms (sabit 3 deneme) | 10 ms arayla 50 deneme | `reviewer` |
| E7 | Timeout her sorguda açık | `CommandTimeout = 30` | varsayılan 30 saniyeden uzun bekleme | `reviewer` |
| E8 | Hata kodu + hata metni ayrılır | `kod=NotFound` (sabit) + metin (yerelleştirilebilir) | kodu string mesajdan ayrıştırmak | `architect` |
| E9 | Katman sınırında çevrilir | `SqlException` → `ErrorCode.Deadlocked` (Infrastructure) | `SqlException`'ı UI'a kadar taşımak | `architect` |
| E10 | Log + dönüş birlikte | dönmeden önce 1 `ERROR` satırı (ADR-026) | sessiz dönüş ya da iki kez log | `reviewer` |
| E11 | `finally`/`using` ile kaynak | `using var conn = new SqlConnection(cs)` | `conn.Close()` yolu unutulmuş | `tester` |
| E12 | İptal hata değildir | `OperationCanceledException` → `Result.Cancelled` | iptali `ERROR` sayıp yığılmak | `tester` |

---

## §2 Katman Sorumluluğu

| Katman | Yakalar | Çıktı | Yapmaz |
|---|---|---|---|
| **Domain** | iş kuralı ihlali | tip’li domain hata kodu | exception fırlatmak (kural ihlali değilse) |
| **Application** | eksik girdi, ön koşul | `Result<T>` | log (Infrastructure yapar) |
| **Infrastructure** (Data) | `SqlException` · timeout · deadlock | çevrilmiş `ErrorCode` | kullanıcı metni üretmek |
| **Protocol / Cli** | taşınım, JSON ayrıştırma | JSON hata nesnesi (`kod`, `mesaj`) | stack döndürmek |
| **UI (WinForms)** | beklenmeyen tek nokta | dialog + `log ERROR` | yutmak (`catch {}`) |
| **Test** | — | assert: kod + mesaj | üretim kodunu düzeltmek (`tester`) |

```
hata uretimi        hata cevirme          hata gosterimi
----------------    ------------------    -----------------------
SqlException   -->  ErrorCode.Timeout --> Result<T>      --> UI: mesaj + log
(lock timeout)      (Infrastructure)      (Application)    (yalniz son nokta catch)
```

---

## §3 Yeniden Deneme Politikası

| # | Durum | Yeniden dene mi | Aksiyon |
|---|---|---|---|
| 1 | Bağlantı zaman aşımı / geçici ağ | **Evet** (3, üstel) | aynı istek, geri çekilme |
| 2 | Deadlock | **Evet** (1 kez) | yeniden çalıştır |
| 3 | Unique/`DuplicateKey` | Hayır | `ErrorCode.Conflict` dön |
| 4 | Doğrulama hatası (400) | Hayır | kullanıcıya dön |
| 5 | Yetkisizlik | Hayır | `ErrorCode.Unauthorized` |
| 6 | 3. deneme de başarısız | Hayır | `ERROR` log + `3-STRIKE` escalation |
| 7 | Aynı hata 3 düzeltmede çözülmedi | Hayır (kod yazma yasak) | protokol §8.1 raporu |

---

## §4 Kullanıcıya Dönen Biçim

| Alan | Kural | Örnek |
|---|---|---|
| `kod` | sabit enum, yerelleştirme anahtarı | `NotFound` |
| `mesaj` | tek cümle, teknik terim yok, noktalı değil | `Kayıt bulunamadı` |
| `ayrinti` | yalnız `DEBUG`/`TRACE` derlemesinde dolar | stack + `evt` |
| `istekId` | korelasyon (ADR-026 `evt` alanı ile aynı) | `7f3a…` |
| stack / SQL | **asla** UI'a | yalnız log dosyası |

---

## §5 İstisnalar (ADR gerekir mi?)

| # | İstisna durumu | ADR gerekir mi | Onay |
|---|---|---|---|
| I1 | Genel `catch (Exception)` UI kökünde (tek nokta) | Hayır — zorunlu son savunma | `architect` |
| I2 | Yeniden deneme sayısının 3'ten fazlası | **Evet** (G8, davranış + maliyet) | kullanıcı |
| I3 | Yeni hata kodu eklemek | Hayır — `ErrorCode` enum'u genişler | `architect` |
| I4 | Hata metnini 3. parti kütüphaneye bırakmak | **Evet** (G8) | kullanıcı |
| I5 | `Try...Catch` ile `Result`'ı atlayıp doğrudan fırlatmak | Hayır, ama **tek stil** gerekir | `architect` (stil kararı: **Result**) |

---

## §6 Yasaklar

| # | Yasak | Karşılığı |
|---|---|---|
| 1 | `catch (Exception) { }` / boş blok | E4 |
| 2 | `ex.Message` UI'a göstermek | E3 |
| 3 | `return null` ile hata bildirmek | E2 |
| 4 | String-concat SQL kaynaklı hata gizlemek | ADR-005 (parameterized) |
| 5 | İptali `ERROR` saymak | E12 |

---

## §7 Doğrulama

- [ ] `catch` bloğu başına bir sorumluluk; yutma **0**
- [ ] Her hata yolu bir `ErrorCode`'ye bağlanıyor (enum'da tanımlı)
- [ ] UI'da stack gösteren pencere **0**
- [ ] Yeniden deneme: 3 deneme + üstel geri çekilme, test edilmiş
- [ ] Log çıktısında secret **0** (`sk-***`, parola `****`)

---

## §8 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Yapısal log biçimi | `.ai/.decisions/accepted/ADR-026-logging-structured.md` |
| 2 | Parameterized SQL (hata kaynağı) | `.ai/.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md` |
| 3 | Secret maskeleme | `.ai/.decisions/accepted/ADR-018-secret-store.md` |
| 4 | 3-Strike escalation | `.ai/AGENTS.md` §8.1 |
| 5 | Loglama kuralları | `.ai/.rules/logging-rules.md` |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G2/G5 + ADR-005/018/026.
