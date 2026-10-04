---
title: "Gözlemlenebilirlik Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Log, metrik ve izleme - biçim, seviye, sink'ler, sayaçlar ve korelasyon kimliği (l3 ile tutarlı)"
---

# Gözlemlenebilirlik Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [error-handling](error-handling-spec.md) · [notification](notification-spec.md)

> **Tek cümlelik sorumluluk:** *Ne loglanır, ne ölçülür, nasıl izlenir ve bunların hepsi l3 Crosscutting ile nasıl tutarlı kalır* sorusunun tek cevabıdır.
> **SSOT:** log/cache/clock/config/retry arayüzleri [l3 Crosscutting](../architecture/index.md) şartnamesindedir; bu dosya yalnız **kullanım sözleşmesini** yazar.

---

## §0 Özet (Answer First)

Tek loglama yüzeyi l3 **`ILogSink`**'tir (`Trace..Error`, alanlı `LogEntry`); sink'ler `FileLogSink` + `DebugLogSink` + `CompositeLogSink` (masaüstü varsayılanı), testte `NullLogSink`. Metrik **BCL ile** üretilir (paket yok — G8): sayaç + süre (`Stopwatch`), gün sonunda tek JSON özete yazılır. Her olayda **`traceId` (oturum) + `taskId`** vardır; hata kaydı [error-handling](error-handling-spec.md) kodunu taşır. Uzun sessizlik görünür olmalıdır (G11).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Log kayıt biçimi, seviye seçimi, alan adları | `ILogSink`/`LogEntry` tipi tanımı | [architecture](../architecture/index.md) l3 |
| 2 | Metrik sayaçları ve toplama periyodu | Harici izleme sunucusu / agent | kapsam dışı (masaüstü, tek kullanıcı) |
| 3 | Teşhis/izleme ekranının verisi | Bildirim/toast kanalları | [notification](notification-spec.md) |
| 4 | Hata kaydının içeriği (kod + alanlar) | Hata kodu üretimi | [error-handling](error-handling-spec.md) |
| 5 | Günlük dosyası döngüsü/boyut sınırı | Dosya yolları/atomik yazım | [persistence](persistence-spec.md) |

---

## §2 Davranış

### §2.1 Üç sinyal

| # | Sinyal | Araç | Ne için |
|---|---|---|---|
| 1 | **Log** | `ILogSink` → `FileLogSink` (+`DebugLogSink`) | ne oldu, hangi alanlarla |
| 2 | **Metrik** | sayaçlar + `Stopwatch` (BCL) | ne sıklıkla, ne kadar sürüyor |
| 3 | **İz** | `traceId` + `taskId` + olay adı | bir işin uçtan uca izi |

### §2.2 Kayıt biçimi

```
2026-09-25T14:03:11.482Z INFO  task.started   trace=T-7f3a task=T-104
  durum=created kuyruk_sira=2
2026-09-25T14:03:15.109Z WARN  retry.attempt  trace=T-7f3a task=T-104
  adim=1 neden=timeout
2026-09-25T14:03:16.770Z ERROR task.failed    trace=T-7f3a task=T-104
  kod=VC-E-0301 sure_ms=5700
```

| # | Kural | Kabul |
|---|---|---|
| O1 | Kayıt **yapılandırılmıştır**: `Message` + `Fields`; serbest metin birleştirme **yok** | `+` ile kurulmuş log = 0 |
| O2 | Olay anahtarı `kategori.olay` biçimindedir (`task.started`) | tutarsız anahtar = 0 |
| O3 | Seviye eşlemesi: bilgi→`Info`, uyarı→`Warn`, `VC-E-03xx..05xx`→`Error`, `0999`→`Error` + istisna | her kodun seviyesi tanımlı |
| O4 | `Sink.Write` **asla exception fırlatmaz**; sink hatası sessizce atlanır | log çökmesi = 0 |
| O5 | Secret/hassas alanlar **kırmızıaltılmış** (`***`) | logda ham secret = 0 (SEC-08) |

### §2.3 Metrikler

| # | Metrik | Tip | Periyot |
|---|---|---|---|
| M1 | `task.completed` / `task.failed` | sayaç | anlık + günlük toplam |
| M2 | `task.duration_ms` | süre (p50/p95 özet) | günlük |
| M3 | `tool.call.count` + hata oranı | sayaç | günlük |
| M4 | `chat.first_token_ms` | süre | günlük (VC-PRD-030 hedefi) |
| M5 | `app.startup_ms` | süre | her açılış |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Log/API | `System.Diagnostics` (BCL) | aynı | **fark yok** |
| Süre ölçümü | `Stopwatch` | aynı | fark yok |
| Hafif sayaç altyapısı | `EventSource` (BCL) | `EventCounter` da kullanılabilir | **`#if` platform farkı** — hedef: tek sayaç kodu |
| Dosya yolu | ortam belirler | ortam belirler | `#if` yalnız yolu ayrımsaz |

> `#if` yalnız buradaki platform farkındadır ve dosya başına **en fazla 1** bloktur (G6); ortak sayaç kodu korunur.

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Alanlı kayıt üretimi | birim test (`FixedClock`, `NullLogSink`) | deterministik çıktı |
| 2 | Sink hata yolu | birim test | fırlatma = 0 |
| 3 | Secret maskelenmesi | birim test + tarama | ham secret = 0 |
| 4 | Metrik sayacı (başarı/başarısız) | birim test | 2 sayaç doğru |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
