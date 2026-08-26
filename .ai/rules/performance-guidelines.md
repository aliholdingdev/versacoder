---
title: "Versa Coder — Performans Rehberi"
type: rules
category: performance
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Performans Rehberi

---

## 1. Performans Hedefleri

| Metric | Hedef | Eşik |
|--------|-------|------|
| Uygulama başlatma | < 3s | 5s |
| UI yanıt süresi | < 100ms | 500ms |
| LLM yanıt süresi | < 30s | 60s |
| Dosya okuma | < 100ms | 500ms |
| Veritabanı sorgusu | < 50ms | 200ms |
| Memory kullanımı | < 500MB | 1GB |

---

## 2. Optimizasyon Kuralları

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Async/await | Tüm I/O işlemleri asenkron |
| 2 | Connection pooling | SQLite WAL + connection pool |
| 3 | Lazy loading | Gerektiğinde yükle |
| 4 | Caching | Semantic cache ile tekrar sorguları önbelleğe alma |
| 5 | Chunked processing | Büyük dosyalar için parçalı işleme |
| 6 | Streaming | LLM yanıtlarında streaming kullanımı |

---

## 3. Bellek Yönetimi

| Kural | Açıklama |
|-------|----------|
| IDisposable | Tüm IDisposable nesneler using ile |
| Large object heap | 85KB+ nesneler LOH'e dikkat |
| GC optimization | GC.Collect() manuel çağırmak yasak |
| Memory leak | Timer, event handler dikkatli yönetim |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
