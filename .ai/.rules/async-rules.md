---
title: "Async ve İptal Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "async/await derinliği: senkron bağlam (UI thread), ConfigureAwait, timeout+iptal birleştirme, async event handler durumu, kuyruğa alma — coding-standards CS-03/CS-04/CS-06'nın tamamlayıcısı"
---

# Async ve İptal Kuralları (`.rules/async-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [coding-standards.md](coding-standards.md) · [performance-guidelines.md](performance-guidelines.md) · [error-handling-rules.md](error-handling-rules.md) · [testing-rules.md](testing-rules.md)

> **Tek cümlelik sorumluluk:** *Bir `async` çağrı hangi thread'e döner, iptal nasıl iletilir, hangi kalıp UI'ı dondurur* sorusunun tek cevabıdır.
> Temel yasaklar (`CS-03` `.Result` · `CS-04` token'lı imza · `CS-06` `async void`) [coding-standards.md](coding-standards.md)'dedir; bu dosya **senkron bağlam, timeout birleştirme ve durum geri alma** derinliğini verir (SSOT — tekrar yok).

---

## §0 Özet (Answer First)

UI katmanında `await` zinciri senkron bağlamı koruduğu için `Invoke` gerekmez, kütüphane katmanında `ConfigureAwait(false)` ile bağlam kasten kırılır; her uzun iş **üstten gelen token + timeout** ile birleştirilir, iptal exception'u yutulmaz, ribbon/form durumu `finally`'de geri alınır — ihlal `reviewer` RED eder.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| A1 | UI katmanında `await` sonrası doğrudan kontrol yazılır (bağlam korunur) | `await YukeAsync(); lbl.Text = "ok";` | her devirde `BeginInvoke` sarmalamak | `reviewer` |
| A2 | Kütüphane katmanı (l1–l3) bağlamı `ConfigureAwait(false)` ile kırar | `await sorgu.ConfigureAwait(false);` | UI handler'ında `ConfigureAwait(false)` (durum kırar) | `reviewer` |
| A3 | CPU-bound iş UI'dan `Task.Run` ile ayrılır | `await Task.Run(() => Hesapla(x), ct);` | UI thread'de 5 sn'lik döngü (B3 ihlali) | `reviewer` |
| A4 | Timeout, üstteki token ile **link edilerek** birleştirilir | `CreateLinkedTokenSource(ct)` + `CancelAfter(30_000)` | `Task.WhenAny` + sessizce devam | `reviewer` |
| A5 | İptal exception'u yutulmaz, dar `catch` kullanılır | `catch (OperationCanceledException) when (ct.IsCancellationRequested)` | `catch (Exception) { }` | `reviewer` |
| A6 | `lock` bloğu içinde `await` yasak | `await kilid.WaitAsync(ct);` | `lock (gate) { await is; }` | `architect` |
| A7 | Busy durum `finally`'de geri alınır | `ribbon.Enabled = false; try { ... } finally { ribbon.Enabled = true; }` | hata sonrası pasif kalan ribbon | `reviewer` |
| A8 | Fire-and-forget yalnız kayıtlı ve hata yakalayıcıyla | `_ = Calisiyor();` + hata olayı | `BirIsAsync();` — uyarı + kayıp hata | `reviewer` |
| A9 | `async void` yalnız WinForms event handler (CS-06) | `async void btnKaydet_Click(...)` | `async void` servis metodu | `reviewer` |
| A10 | Uzun işin bütçesi ve token'ı vardır (B5 ≤ 30 sn) | tool çağrısı `CancellationToken` + 30 sn | token'sız, süresiz bekleme | `reviewer` |

---

## §2 İstisnalar (ADR gerekir mi?)

| # | İstisna durumu | ADR gerekir mi | Onay |
|---|---|---|---|
| E1 | 3. parti senkron API'nin UI'dan çağrılması kaçınılmazsa | **Evet** — sarmalayıcı + ölçüm | `architect` |
| E2 | `async void` (handler dışı) talebi | Evet ama CS-06 ihlali → **kapatılamaz** | `reviewer` |
| E3 | Dış API'nin iptal edilememesi | Hayır — `NEEDS_VALIDATION` notu + timeout | `architect` |
| E4 | net48 ile net10'daki bağlam farkına `#if` | Evet — `#if` yalnız platform farkı (G6) | `architect` |
| E5 | Ölçülmüş hotspot'ta `ValueTask`/tekrar kullanılan task | **Evet** — önce benchmark (N5) | `architect` |

---

## §3 Kanıt

| # | Kontrol | Komut / Dosya | Beklenen |
|---|---|---|---|
| K1 | Bloklayıcı kalıp taraması | `Select-String -Path src -Pattern "\.Result|\.Wait\(\)|async void"` | yalnız handler satırları |
| K2 | Bağlam kırma tutarlılığı | `Select-String -Path src -Pattern "ConfigureAwait"` | kütüphane katmanında yaygın, UI'da yok |
| K3 | İptal senaryosu testi | `tests/**/*Cancellation*` + `dotnet test` | iptal/yol testleri yeşil (TS-07) |
| K4 | İki bacak build | `dotnet build` (net48 + net10.0-windows) | 0 hata |
| K5 | Ölçüm | stopwatch logu (`performance-guidelines.md` B3/B5) | UI bloklama ≤ 50 ms |

- İmza ile token arasında bağlantı vardır: çağrı token'ı **hiç yoksa** `CS-04` ihlali, varsa ama kullanılmamışsa `reviewer` aynı RED'i verir.
- Testte iptal yolu zorunludur (`testing-rules.md` TS-07): yalnız mutlu yol testi bu dosyanın kanıtı sayılmaz.
- 3 başarısız düzeltmeden sonra kod yazmak yasaktır (AGENTS.md §8.1) — `3-STRIKE` raporu ile devam.

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect).
