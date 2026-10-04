---
title: "Önbellek Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Ne cache'lenir, TTL, invalidation, bellek bütçesi, anahtar/versiyon, thread-safety, negatif cache ve ölçüm zorunluluğu"
---

# Önbellek Kuralları (`.rules/caching-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [performance-guidelines.md](performance-guidelines.md) · [data-access-rules.md](data-access-rules.md) · [error-handling-rules.md](error-handling-rules.md) · [testing-rules.md](testing-rules.md)

> **Tek cümlelik sorumluluk:** *Neyi, ne kadar süre ve hangi invalidation ile cache'leriz; cache ne zaman yasak* sorusunun tek cevabıdır.
> Cache bir **hızlandırıcıdır**, doğruluk kaynağı değil: doğruluk her zaman [data-access-rules.md](data-access-rules.md)'ndeki sorgudadır (SSOT).

---

## §0 Özet (Answer First)

Yalnızca **değiştirilebilir (idempotent) ve okuma ağırlıklı** veri cache'lenir; her girişin **TTL'si ve invalidation yolu** yazma kodunda birlikte tanımlanır, bellek bütçesi aşılmaz ve cache **ölçülmeden** eklenir — ihlal (özellikle TTL'siz giriş ve invalidation unutulması) `reviewer` RED eder.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| C1 | Yalnız değiştirilebilir, salt-okunur veri cache'lenir | oturum listesi `Dto`'su | izin/rol kararı yalnız cache'ten | `reviewer` |
| C2 | Her giriş için TTL zorunlu ve görünür | `Ttl = TimeSpan.FromMinutes(5)` | süresiz giriş (`TimeSpan.MaxValue`) | `reviewer` |
| C3 | Yazma yolunda invalidation zorunlu | kayıtta `Remove(key)` | TTL'ye güvenip invalidasyonu unutmak | `reviewer` |
| C4 | Bellek bütçesi sabittir | transcript ≤ 8 MB (B9); cache toplamı ≤ 50 MB | sınırsız büyüyen `Dictionary` | `architect` |
| C5 | Anahtar normalize ve **versiyonludur** | `session:{id}:v2` | ham kullanıcı girdisi + sürüm yok | `reviewer` |
| C6 | Negatif (boş) sonuç kısa süreli cache'lenir | `null` → 30 sn | hatayı/istisnayı cache'lemek | `reviewer` |
| C7 | Thread-safety sağlanır | `ConcurrentDictionary` / async kilit | paylaşılan `Dictionary` üzerine yazma | `reviewer` |
| C8 | Cache doldurma UI thread'ini bloklamaz | async yükleme + `await` | UI'da senkron `Doldur()` (B3 ihlali) | `reviewer` |
| C9 | Tek başına çalışan cache yoktur | ilk istekte DB'ye düşen `GetOrAddAsync` | DB'siz/cache'siz ortamda çökme | `architect` |
| C10 | Cache ölçülmeden eklenmez | hit/miss logu + benchmark | "hızlı diye" tahmini cache | `architect` |

---

## §2 İstisnalar (ADR gerekir mi?)

| # | İstisna durumu | ADR gerekir mi | Onay |
|---|---|---|---|
| E1 | Üretim için paylaşılan cache (Redis vb.) | **Evet** — yeni bağımlılık = ADR (G8) | `architect` + kullanıcı |
| E2 | TTL'siz kalıcı yerleşik önbellek (disk) | **Evet** — invalidation planı zorunlu | `architect` |
| E3 | Negatif cache uzatılması (> 1 dk) | Hayır — ama gerekçesi kod yorumunda | `reviewer` |
| E4 | Kritik veride stale okuma kabulü | **Evet** — dayanıklılık/kurallılık gerekçesi | `architect` + kullanıcı |
| E5 | Bellek bütçesinin (50 MB) aşılması | **Evet** — ölçüm + alternatif ≥ 2 | `architect` |

---

## §3 Kanıt

| # | Kontrol | Komut / Dosya | Beklenen |
|---|---|---|---|
| K1 | TTL varlığı | kod taraması: cache kayıt noktaları | her girişte TTL satırı |
| K2 | Invalidation eşleşmesi | yazma metotları ↔ `Remove(...)` | her yazma yolu için eşleşme |
| K3 | Bellek ölçümü | `GC.GetTotalMemory(true)` + tekrarlı yükleme | bütçe ≤ 50 MB, artış yok |
| K4 | Stale okuma testi | `tests/**` cache invalidation testi | yazma sonrası okuma yeni değer |
| K5 | Ölçüm | benchmark (`performance-guidelines.md` N5) | cache öncesi/sonrası süre |

- Önbellek **performans kanıtı** olmadan eklenemez: ölçüm yoksa optimizasyon yok (N5).
- Cache kaynaklı hata yolu [error-handling-rules.md](error-handling-rules.md)'a uyar: cache hatası istisnayı yutar, işlemi durdurmaz (fallback DB'ye).
- Testte cache deterministik kurulur (TS-04): paylaşılan statik cache testler arası kirlilik üretir (TS-14).

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect).
