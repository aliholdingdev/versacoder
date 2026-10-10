---
title: "Agent Panel Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Görev/ajan paneli: görev listesi görünümü, durum geçişleri, tool çağrısı satırları ve canlı log akışı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Agent Panel Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/session-spec|session-spec]] · [[spec/tool-spec|tool-spec]] · [[spec/task-queue-spec|task-queue]]

> **Tek cümlelik sorumluluk:** *Görev listesi hangi sütunları gösterir, durum geçişleri nasıl yansıtılır ve log akışı nereden okunur* sorusunun tek cevabıdır.
> Durumun **kuralı** [[spec/task-queue-spec|task-queue]] ve [[spec/session-spec|session-spec]]’tadır; bu dosya yalnızca **gösterimi** sahiplenir.

---

## §0 Özet (Answer First)

Panel `GridControl` (durum/filtre sütunlu) + detay bölümü (tool çağrıları) + `MemoEdit` (ReadOnly log akışı) olmak üzere üç bölgeden oluşur. Durum geçişleri canlı yansıtılır: `created → running → paused → completed | failed`; geçersiz geçiş UI'da **gösterilmez** (veri katmanı reddeder). İptal butonu 2 sn içinde sonucu günceller; `failed` satırında hata kodu (`VC-E-NNNN`) + insanca mesaj görünür (VC-PRD-051).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Görev listesi sütunları, filtre, sıralama, detay açma | Kuyruk sırası, öncelik, iptal kuralı | [[spec/task-queue-spec|task-queue]] |
| 2 | Durum rozetlerinin **gösterimi** | Durum makinesi doğrulaması | [[spec/session-spec|session-spec]] |
| 3 | Tool çağrısı satırı (ad, özet, süre, sonuç) | Tool şeması ve izin modeli | [[spec/tool-spec|tool-spec]] |
| 4 | Log akışı (satır bazlı, kaydırma, filtre) | Log sink'i ve dosya biçimi | [[spec/observability-spec|observability]] |
| 5 | Hata satırı gösterimi | Hata kodu üretimi | [[spec/error-handling-spec|error-handling]] |

---

## §2 Davranış

### §2.1 Panel yerleşimi

```
+-------------------------------------------------------------+
| Görevler  [durum filtresi ▾] [Tarih ▾]        [Yenile ▻]    |
+-------------------------------------------------------------+
| ID    Başlık            Durum       Başlangıç    Süre       |
| T-104 build düzelt      ● running   14:02        00:41      |
| T-103 test yaz          ● paused    13:47        02:10      |
| T-102 arama indeksi     ✓ completed 13:30        01:05      |
| T-101 sağlayıcı ayarı   ✗ failed    13:12        00:08  ▲   |
+-------------------------------------------------------------+
| T-104 detayı:  [tool] bash: dotnet build   ok  1.9s         |
|                [tool] edit: Result.cs      ok  0.4s         |
+-------------------------------------------------------------+
| 14:03:11 INFO  task.started   id=T-104                      |
| 14:03:13 INFO  tool.call      name=bash                      |
| 14:03:15 WARN  retry          step=1 (tek deneme)           |
+-------------------------------------------------------------+
```

### §2.2 Durum gösterimi

| # | Durum | Rozet | Panel davranışı |
|---|---|---|---|
| 1 | `created` | gri | satır hazır, iptal edilebilir |
| 2 | `running` | mavi + `ProgressBarControl` | canlı log; iptal/pause aktif |
| 3 | `paused` | sarı | devam butonu aktif; log durur |
| 4 | `completed` | yeşil | detay salt-okunur; transcript bağlantısı |
| 5 | `failed` | kırmızı | `VC-E-NNNN` + mesaj + "Tekrar dene" |
| 6 | `failed(cancelled)` | gri-kırmızı | yarım tool çağrısı kilitli değildir |

### §2.3 Akış ve yenileme

| # | Kural | Kabul |
|---|---|---|
| L1 | Log satırı UI'ya **artan** eklenir; tam çizim yok | 1.000 satırda kaydırma akıcı |
| L2 | Otomatik kaydırma yalnız kullanıcı **aşağı** bakmıyorken | elle yukarı kaydıran kullanıcı rahatsız edilmez |
| L3 | Panel 500 satırda kırpılır (ring buffer), tamamı dosyada | bellek sınırlı kalır |
| L4 | Yenileme olay güdümlüdür (polling yok) | işlemsiz bekleyiş ≤ %1 CPU |
| L5 | Filtre durumu oturumda kalır | yeniden açılışta korunur |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Grid/veri bağlama | DevExpress **v26.1** `GridControl` | aynı | **fark yok** |
| Sayaç/timer | `System.Threading.Timer` | aynı | fark yok |
| Log satır kodlaması | UTF-8 (BCL) | aynı | fark yok |

> Panelde `#if` yok; olay akışı (l2 → UI) her iki bacakta aynıdır (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Durum rozeti eşlemesi (5 durum) | birim test | eksik/sahte durum yok |
| 2 | Ring buffer kırpma | birim test (500+ satır) | bellek artışı sınırlı |
| 3 | İptal sonrası panel güncelliği | smoke: iptal → 2 sn içinde rozet | `failed(cancelled)` görünür |
| 4 | İzolasyon | NetArchTest | l7 → l1/l4 referansı = 0 |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
