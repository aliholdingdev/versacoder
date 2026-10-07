---
title: "VersaCoder — Performans Ölçüm Şartnamesi"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Bütçe değerleri `.rules/performance-guidelines.md` (B1..B10); bu belge ÖLÇÜM YÖNTEMİ — ne, nerede, kaç örnek, hangi araçla ölçülür"
---

# VersaCoder — Performans Ölçüm Şartnamesi (`spec/performance-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [architecture/index](../architecture/index.md) · [test-strategy](test-strategy.md) · [release-spec](release-spec.md)

> **Tek cümlelik sorumluluk:** *Bütçe ihlalini **kim, hangi araçla, kaç örnekle** kanıtlar ya da temizler* sorusunun tek cevabıdır.
> Bütçe **değerleri** `.rules/performance-guidelines.md` §1 (B1..B10) dosyasındadır — burada **tekrar edilmez** (SSOT); bu belge yalnız **ölçüm yöntemi + kabul kanıtı**dır.

---

## §0 Özet (Answer First)

**Ölçüm yoksa optimizasyon yok (N5):** her bütçe ya **otomatik test içinde** (assert'li stopwatch) ya da **manuel ölçümde** (BenchmarkDotNet / stopwatch log'u) doğrulanır; sonuç **p95 ve örnek sayısıyla** `reports/` altına yazılır, sadece "hızlı hissettiriyor" geçersizdir (G5).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Ölçüm yöntemi, araç, örnek sayısı, ortam | Bütçe **değerleri**: `.rules/performance-guidelines.md` §1 |
| 2 | p95 hesabı ve rapor satırı biçimi | Test framework/kapsamı: [test-strategy](test-strategy.md) |
| 3 | Regresyon eşiği (ne zaman RED) | Kod inceleme seviyeleri: [review-spec](review-spec.md) |
| 4 | Performans kanıtının release'e girişi | Sürüm/not akışı: [release-spec](release-spec.md) |
| 5 | Yavaşlık durumunda kök neden adımı | Teknik destek/tekrar: `.ai` içinde ilgili playbook |

> **YASAK:** Kanıtsız "optimized" iddiası (G5) · benchmark'ı tek çalıştırmak · Debug derlemesiyle Release sonucu ölçmek · ölçüm kodunu koda gömüp yayınlamak.

---

## §2 Davranış

### §2.1 Bütçe → yöntem eşlemesi

| Bütçe | Ölçüm yöntemi | Araç | Örnek | Kanıt |
|---|---|---|---|---|
| B1 Yerel DB sorgusu ≤ 100 ms | Test içi stopwatch, **p95** | xUnit + gerçek SQLite | ≥ 100 çağrı | `reports/perf-db.md` |
| B3 UI thread bloklama ≤ 50 ms | Bölünmüş ölçüm (başlangıç/bitiş) | stopwatch + log | ≥ 20 olay | log satırı |
| B5 Tool çağrısı ≤ 30 sn | Test timeout'u | xUnit `[Fact(Timeout=…)]` | 1 | test çıktısı |
| B6 LLM ilk token ≤ 5 sn | Smoke ölçümü (gerçek sağlayıcı, manuel) | stopwatch + SSE log | ≥ 3 istek | manuel rapor (NEEDS_VALIDATION) |
| B7 Açılış ≤ 2 sn | Pipeline stopwatch | host log'u | ≥ 5 açılış | açılış logu |
| B8 Arama ≤ 1 sn / 10.000 dosya | Benchmark | BenchmarkDotNet | ≥ 16 ölçüm | `.md` benchmark çıktısı |

### §2.2 Ölçüm kuralı

| # | Kural | Gerekçe |
|---|---|---|
| 1 | Önce **ısınma** (warm-up) çalıştırılır; ilk ölçüm sayılmaz | JIT/cachesapması |
| 2 | Sonuç **p95** + örnek sayısı + ortam (TFM, makine) ile yazılır | Tek değer yanıltıcı |
| 3 | Release derlemesiyle ölçülür (`-c Release`) | Debug 3-5× yanlıştır |
| 4 | Regresyon eşiği: bütçenin **%120**'sini aşan sonuç RED sayılır | Gürültüyü ayıklar |
| 5 | Ölçüm çıktısı dosyaya (rapor) yazılır, oturum belleğine değil | SSOT + tekrar okuma yok |

### §2.3 Regresyon akışı

```
  test / benchmark kosar
        |
        v
  p95 hesapla  --<= butce----------▶ yeşil (rapora "geçti" satiri)
        |
   > %120 butce
        v
  RED --▶ somut hedef (dosya:metot) --▶ once olc -- sonra kod (N5)
        |
        3 deneme cozulmediyse --▶ 3-STRIKE raporu (AGENTS.md §8.1)
```

---

## §3 TFM Notu

| Bacak | Ölçüm | Not |
|---|---|---|
| `net48` | Ayrı ölçüm seti | BCL API farkları ölçüm kodunu etkiler |
| `net10.0-windows` | Ayrı ölçüm seti | İki bacak **aynı bütçeye** tabidir |
| Karşılaştırma | Sonuçlar **yan yana** yazılır | Tek bacak ölçümü = eksik kanıt (G6) |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| PF-01 | B1 bütçesi p95 ile | Test assert'i + `reports/perf-db.md` |
| PF-02 | Regresyon eşiği RED üretir | Kasıtlı yavaş sorgu testi kırmızıya döner |
| PF-03 | Release derlemesi ölçümü | Benchmark çıktısında `-c Release` görünür |
| PF-04 | İki bacak ölçümü | net48 ve net10 satırları birlikte mevcut |
| PF-05 | Ölçüm yoksa kapanış yok | GATE-006/`release-spec` §4 kontrol satırı |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
