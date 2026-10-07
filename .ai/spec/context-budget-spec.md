---
title: "VersaCoder — Bağlam Bütçesi Şartnamesi (Context / Compaction)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Bağlam bütçesi limitleri, compaction (derleme) tetikleyicileri ve derleme sonrası korunanlar — ADR-008 (context-assembly) ile tutarlı"
---

# VersaCoder — Bağlam Bütçesi Şartnamesi (`spec/context-budget-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [architecture/index](../architecture/index.md) · [session-spec](session-spec.md) · [llm-provider-spec](llm-provider-spec.md) · [protocol-layer-spec](protocol-layer-spec.md)

> **Tek cümlelik sorumluluk:** *Bir oturumda bağlam ne kadardır, ne zaman derlenir (compaction) ve derlemeden sonra ne hayatta kalır* sorusunun tek cevabıdır.
> Karar ADR-008'dedir (context-assembly, **planlı** — `.decisions/index.md` §planlı): bu belge **bütçe + tetikleyici** davranışını tanımlar.

---

## §0 Özet (Answer First)

Bağlam üç katmanlı bütçeyle yönetilir: **transcript ≤ 8 MB / %80 derleme eşiği (B9)**, **subagent açılış paketi ≤ 8 KB**, **devre paketi ≤ 2 KB**; eşiği aşan içerik **derlenir (compaction)** ve derleme **tetikleyici tablosuyla** otomatik başlar — derlemeden sonra **özet + karar etiketleri + açık iş kalemleri** hayatta kalır, ham eski mesajlar **arşive** gider (kaybolmaz).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Bütçe limitleri ve ölçüm birimi | Transcript **biçimi**/durum makinesi: [session-spec](session-spec.md) |
| 2 | Compaction tetikleyicileri + sırası | Sağlayıcı çağrısı/streaming: [llm-provider-spec](llm-provider-spec.md) |
| 3 | Derlemede korunan/silinen içerik | Mesaj zarfı: [protocol-layer-spec](protocol-layer-spec.md) |
| 4 | Tool çağrı sonuçlarının kırpılma kuralı | Tool şeması/boyut sınırı: [tool-spec](tool-spec.md) |
| 5 | Ajan çalışma zamanı okuma limitleri (`AGENTS.md` §6) | Ajan/protokol kuralları: `.ai/AGENTS.md` (tekrar yok) |

> **YASAK:** Eşiği aşınca **sessizce** eski mesaj silmek (arşiv yoksa veri kaybı) · derlemeyi token tahminiyle **ölçmeden** yapmak · derleme sonrası karar etiketlerini (`CONFLICT DETECTED`, `UNKNOWN`, `NEEDS_VALIDATION`) kaybetmek.

---

## §2 Davranış

### §2.1 Bütçe limitleri

| # | Kalem | Limit | Ölçüm | Aşınca |
|---|---|---|---|---|
| CB1 | Oturum transcript'i | **8 MB** (derleme eşiği **%80**) | Kalıcı boyut | Otomatik compaction (B9) |
| CB2 | Tek bağlam penceresi | Model bağlam sınırının **%80**'i (token) | Token sayacı | Derleme + kırpma |
| CB3 | Subagent açılış paketi | **≤ 8 KB** | Karakter | Kesit gönder, dosya yolu ver |
| CB4 | Devre (handoff) paketi | **≤ 2 KB** | Karakter | Kanıtı dosyaya, özeti pakete |
| CB5 | Tool sonucu gövdesi | ≤ 64 KB (fazlası `contentRef`) | Karakter | Referans olarak taşınır |

### §2.2 Compaction tetikleyicileri

| # | Tetikleyici | Koşul | Aksiyon |
|---|---|---|---|
| T1 | **Eşik** | Transcript ≥ %80 (8 MB) | Zorunlu derleme |
| T2 | **Token** | Gönderilecek bağlam ≥ pencere %80'i | Pencere içi derleme |
| T3 | **Araç yığını** | Arka arkaya ≥ 20 tool çağrısı | Sonuçları özetle, girdiyi arşivle |
| T4 | **Uzun bekleme** | Oturum `paused` → `resume` | Özetle aç (stale context önlemi) |
| T5 | **Manuel** | Kullanıcı "özetle" | Aynı derleme yolu, elle tetik |

### §2.3 Derleme sırası ve korunanlar

```
  [1] tetikleyiciyi algila (T1..T5)
  [2] korunacaklari AYIKLA  ->  ozet + acik is kalemleri + karar etiketleri
  [3] eski mesajlari ARSIVE tasi (transcript'te silinmez, "compacted" isaretli)
  [4] ozeti transcript'e YENI girdi olarak yaz (Seq devam eder)
  [5] butceyi yeniden olc   --  hala %80 ustuyse  [2]'ye don (limit 2 tur)
```

| Korunur (derlemeden sonra) | Arşive gider (erişilebilir ama pencere dışı) |
|---|---|
| Kullanıcının son talimatları + kısıtlar | Ham eski konuşma mesajları |
| Karar etiketleri (G9/`G10` işaretleri) | Tool çıktılarının ham gövdesi |
| Açık iş kalemleri / `NEEDS_VALIDATION` | Tekrarlanan hata metinleri |
| Dosya yolları ve kanıt referansları | Araya giren uzun log parçaları |
| Aktif görevin mevcut durumu | Derlenmiş özetlere giren içeriğin kendisi |

---

## §3 TFM Notu

| Bacak | Durum | Not |
|---|---|---|
| `net48` | Sayaçlar ortak kod (dosya boyutu + token tahmini) | `#if` gerekmez |
| `net10.0-windows` | Aynı | Farklı eşik = RED |
| Ölçüm | Birim **aynı** (MB/`%`/token) | İki bacakta farklı eşik davranışı yok |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| CB-01 | T1 eşiği tetikler | Sahte büyük transcript → derleme çağrısı |
| CB-02 | Derleme sonrası korunanlar | Özet + etiket + açık iş kalemleri girdide görünür |
| CB-03 | Arşiv kaybı yok | Eski girdiler `compacted` olarak erişilebilir |
| CB-04 | Subagent paketi ≤ 8 KB | Paket boyutu ölçümü (AGENTS.md §5.3) |
| CB-05 | Derleme sınırı (max 2 tur) | Sonsuz döngü testi: 2 turda durur |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
