---
title: "Start Page Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Karşılama ekranı: son projeler, hızlı aksiyonlar, arama ve açılış performansı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Start Page Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/persistence-spec|persistence]] · [[spec/project-model-spec|project-model]] · [[spec/ui-shell-spec|ui-shell]]

> **Tek cümlelik sorumluluk:** *Uygulama açıldığında kullanıcı ne görür, son proje listesi nereden gelir ve hızlı aksiyonlar hangi komuta gider* sorusunun tek cevabıdır.
> Start Page bir **karşılama görünümüdür**; proje açma işleminin kendisi [[spec/project-model-spec|project-model]] kapsamındadır.

---

## §0 Özet (Answer First)

Açılış ekranı `TileControl` (hızlı aksiyonlar) + son projeler listesi + `SearchControl` (listede filtre) + `MarqueeProgressBarControl` (yükleniyor) katmanlarından oluşur. Son projeler **kayıtlı oturumdan** (kalıcı liste) okunur, en son üste olmak üzere 10 kalem gösterilir. "New Project" ve "Open" akışları Main IDE'yi ([[spec/ui-shell-spec|ui-shell]]) başlatır. Hedef: Start Page görünür olana kadar **≤ 5 sn** (soğuk açılış, VC-PRD-080).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Son projeler listesi (sıra, tıklama, kaldırma) | Proje kaydının şeması | [[spec/persistence-spec|persistence]] |
| 2 | Hızlı aksiyon karoları (New/Open/Import/Docs) | Proje oluşturma doğrulaması | [[spec/project-model-spec|project-model]] |
| 3 | Listede arama/filtre (yerel) | Proje içi grep | [[spec/search-grep-spec|search-grep]] |
| 4 | Açılış yükleme durumu ve süresi | Kabuk yerleşimi/pencere | [[spec/ui-shell-spec|ui-shell]] |
| 5 | Son proje tıklanamaz hale gelmesi (dizin yok) | Hata kodu sınıflandırma | [[spec/error-handling-spec|error-handling]] |

---

## §2 Davranış

### §2.1 Yerleşim

```
+----------------------------------------------------------+
|  VersaCoder                              [Ayarlar ⚙]     |
+----------------------------------------------------------+
|  +----------+ +----------+ +----------+ +------------+   |
|  | + New    | | ↗ Open   | | ⇩ Import | | ? Docs     |   |
|  +----------+ +----------+ +----------+ +------------+   |
|  Son Projeler                    [SearchControl: filtre] |
|  +----------------------------------------------------+  |
|  | MySolution        C:\work\mysolution    2 dk önce  |  |
|  | VersaCore         C:\work\versacore     dün        |  |
|  | eski-proje        C:\old\eski           ✖ dizin yok|  |
|  +----------------------------------------------------+  |
|  ▓▓▓░░░ MarqueeProgressBarControl  "Yükleniyor…"         |
+----------------------------------------------------------+
```

### §2.2 Davranış tablosu

| # | Eylem | Sonuç | Sınır |
|---|---|---|---|
| P1 | Açılış | Listeyi oku, en yeni üste; **10 kalem** | 10'dan fazlası "Tümünü gör" |
| P2 | Karo tıklama (New/Open/Import) | l2 handler → Main IDE açılır | Start Page kapanır (tek pencere) |
| P3 | Son proje tıklama | proje açılır, ağacı dolu gelir | > 3 sn'de yükleme göstergesi |
| P4 | Proje kaldır (satır ✕) | listeden çıkarılır (silmek ≠ kaydı silmek) | onay istemez, geri alınabilir |
| P5 | Dizin yok / izin yok | satır **pasif** + açıklama; "Konumu değiştir" | tıklama çalışmaz, hata yığını yok |
| P6 | Filtre | anlık daraltma (case-insensitive) | 200 kayıtta ≤ 100 ms |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Açılış süresi ölçümü | `Stopwatch` (BCL) | aynı | **fark yok** |
| Son proje kaynağı | SQLite (ADO.NET + Dapper) | aynı | fark yok |
| Görsel stıl/skin | DevExpress **v26.1** | aynı | G3 |

> Start Page kodunda `#if` yoktur (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Sıralama (en yeni üste) + 10 sınırı | birim test | deterministik (sabit `IClock`) |
| 2 | Dizin yok durumu | birim test | satır pasif, istek gitmez |
| 3 | Filtre davranışı | birim test | büyük/küçük harf duyarsız |
| 4 | Açılış süresi | smoke ölçüm | ≤ 5 sn (VC-PRD-080) |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
