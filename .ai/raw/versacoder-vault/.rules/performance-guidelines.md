---
title: "Performans Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Arama/IO bütçeleri, N+1 ve allotment yasağı, WinForms render kuralları, streaming/LLM zaman aşımları, ölçüm ve eşikler"
---

# Performans Kuralları (performance-guidelines)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](coding-standards.md) · [data-access-rules](data-access-rules.md) · [devexpress-rules](devexpress-rules.md) · [test-strategy](../spec/test-strategy.md)

> **Tek cümlelik sorumluluk:** *Hangi davranışın hangi **bütçe** içinde kalması gerektiği ve ihlalde kimi RED'leyeceği* sorusunun tek cevabıdır.
> Kural = zorunlu. İhlali `reviewer` RED eder; gerekçeli istisna ADR ister (AGENTS.md §9).

---

## §0 Özet (Answer First)

**Bütçeler:** tek sorgu ≤ 100 ms (yerel DB), UI thread bloklama ≤ 50 ms, tool çağrısı ≤ 30 s (timeout zorunlu), LLM ilk token ≤ 5 s. **N+1, seçilmemiş kolon ve UI thread'de I/O yasak.** Her bütçe ihlali test ile kanıtlanır (GATE-005).

---

## §1 Bütçe tablosu

| # | Alan | Bütçe | Ölçüm | İhlal |
|---|---|---|---|---|
| B1 | Yerel DB sorgusu (SQLite) | ≤ **100 ms** (p95) | stopwatch + log | `reviewer` RED |
| B2 | SQL sorgu süresi (SQL Server) | ≤ **250 ms** (p95), `CommandTimeout = 15` sn | DB istatistiği | RED |
| B3 | UI thread bloklama | ≤ **50 ms** | ayrıştırma + `BeginInvoke` | RED |
| B4 | Dosya okuma (proje ağacı) | ≤ **200 ms** (10 MB altı), asenkron | stopwatch | RED |
| B5 | Tool çağrısı (bash vb.) | ≤ **30 s** + zorunlu `CancellationToken` | test timeout'u | RED |
| B6 | LLM ilk token (streaming) | ≤ **5 s**, streaming zorunlu | smoke log | RED (ADR-011) |
| B7 | Uygulama açılışı (host) | ≤ **2 s** | startup stopwatch | kırmızı → l6 optimize |
| B8 | Arama (grep/glob) | ripgrep benzeri; ≤ **1 s** 10.000 dosyada | benchmark | RED |
| B9 | Bellek (oturum) | transcript ≤ **8 MB** (derleme eşiği %80) | `GC.GetTotalMemory` | otomatik compaction |
| B10 | Bağımlılık | **yeni NuGet = ADR gerekçesi** | `Directory.Packages.props` | `reviewer` RED |

---

## §2 Veri erişimi (l4)

| # | Kural | İyi | Kötü |
|---|---|---|---|
| D1 | **N+1 yasak** — liste sorgusu tek sorguda `JOIN` | tek `SELECT ... JOIN` | döngüde `SELECT` |
| D2 | **Seçilmemiş kolon yasak** — yalnız gerekli alanlar | `SELECT Id, Name` | `SELECT *` |
| D3 | **Parameterized zorunlu** — concat = G2 ihlali | `WHERE Id = @id` | `"..."+id` |
| D4 | **Index** — filtrelenen her kolon için index | migration'da `CREATE INDEX` | full scan |
| D5 | **`CommandTimeout` her komutta** setli | `cmd.CommandTimeout = 15` | varsayılan (30 sn sessiz bekleme) |
| D6 | **Streaming** — büyük sonuçlar `CommandBehavior.SequentialAccess` | `reader.Read()` döngüsü | `ToList()` (bellek patlaması) |
| D7 | **Sayfalama** — liste ekranları `LIMIT/OFFSET` | page=100 | sonsuz `ReadAll` |

---

## §3 WinForms / DevExpress (l7)

| # | Kural | İyi | Kötü |
|---|---|---|---|
| W1 | **UI thread'de I/O yasak** | `async` + `await` + `Progress<T>` | `File.ReadAllText` UI'da |
| W2 | **Kontrol yenileme toplu** | `BeginUpdate/EndUpdate` | her satırda `Refresh` |
| W3 | **Grid sanallaştırma** açık (büyük veri) | `GridControl` server mode | `DataSource = tam liste` |
| W4 | **Ağır iş** `Task.Run` içinde, sonuç `Invoke` ile | arka plan işi | `Thread.Sleep` UI'da |
| W5 | **Resx/ikon** tek seferde yükle (`ResourceManager`), dispose | `using` | her çizimde yeni `Bitmap` |
| W6 | **Ribbon olayları** hafif; ağır iş programla (ribbon disable + progress) | `async` handler | senkron zincir |
| W7 | **Double-buffer** açık, gereksiz `Invalidate()` yok | konteyner seviyesinde | her keystroke'da repaint |

---

## §4 LLM / Agent (l5 + l4)

| # | Kural |
|---|---|
| L1 | **Streaming zorunlu** (SSE); tam yanıt bekleyerek UI bloklanmaz |
| L2 | Zaman aşımı + iptal (`CancellationToken`) her çağrıda; **retry üstel backoff**, maks. 3 |
| L3 | Context bütçesi: subagent paketi **≤ 8 KB** (AGENTS.md §5); transcript derleme %80'de |
| L4 | Tool çağrısı **step limiti** (`LoopPolicy`) — sonsuz döngü yasak |
| L5 | Yanıtı **parça parça** ekle (`AppendText` yerine tek `SuspendLayout` bloğu) |

---

## §5 Bağımlılık / derleme

| # | Kural |
|---|---|
| N1 | Yeni paket = **ADR** (ADR-010 gibi) + CPM'e tek yerden ekleme |
| N2 | `net48` bacağı paket sürümü kilitli (SqlClient **5.x** — ADR-005) |
| N3 | `TreatWarningsAsErrors = true`; yeni uyarı = build kırılır |
| N4 | Kritik yolda **`Span<T>`/`stackalloc`/önbellek** (yalnız ölçülmüş hotspot'ta) |
| N5 | Ölçüm yoksa optimizasyon yok (önce benchmark, sonra kod) |

---

## §6 Denetim

- [ ] `dotnet test` + coverage **≥ %90** ([test-strategy](../spec/test-strategy.md))
- [ ] B1/B3/B7 için stopwatch logları `reports/` altına
- [ ] NetArchTest: l7 → l4 referansı 0 (I/O UI thread'de tespiti)
- [ ] `Directory.Packages.props` değişiklikleri ADR listesiyle eşleşiyor

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect).
