---
title: "Arama (grep/glob) Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Kod içinde arama: grep (içerik) ve glob (dosya adı), sonuç listesi, filtreler ve editöre atlama"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Arama (grep/glob) Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/solution-explorer-spec|solution-explorer]] · [[spec/code-editor-spec|code-editor]] · [[spec/task-queue-spec|task-queue]]

> **Tek cümlelik sorumluluk:** *Bir kalıp proje genelinde nasıl taranır, sonuçlar nasıl listelenir ve tıklayınca editör nasıl açılır* sorusunun tek cevabıdır.
> Arama **salt okunurdur**; dosyaya hiç yazmaz, hedefi workspace kökü dışında olamaz.

---

## §0 Özet (Answer First)

İki kip vardır: **grep** (içerik) ve **glob** (dosya adı). Tarama dosya sistemindedir (DB **değil**), `.`/`.git`/`bin`/`obj` gibi dizinler atlanır ve **tek örnek** (async, iptal edilebilir) yürür. Sonuçlar `GridControl` listesinde `dosya:satır` biçiminde gösterilir; tıklama editörü ilgili satıra götürür (VC-PRD-014). Sonuç üst sınırı **1.000** ile kilitlenir; aşımda "daha dar filtre" uyarısı çıkar.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | İçerik araması (regex dahil) ve dosya adı araması | Dosya ağacı yükleme | [[spec/solution-explorer-spec|solution-explorer]] |
| 2 | Sonuç listesi, filtre, sayfalama, atlanan dizinler | Editör sekmesi davranışı | [[spec/code-editor-spec|code-editor]] |
| 3 | Tarama görevinin kuyruğa/iptale girmesi | Öncelik ve iptal kuralı | [[spec/task-queue-spec|task-queue]] |
| 4 | Sonuç içinden değiştirme **niyetinin** tetiklenmesi | Değiştirme onayı ve diff | [[spec/diff-view-spec|diff-view]] |
| 5 | Arama süresi/ölçümü | Log/metrik yazımı | [[spec/observability-spec|observability]] |

---

## §2 Davranış

### §2.1 Kipler ve filtreler

| # | Kip | Kalıp | Varsayılan filtre | Not |
|---|---|---|---|---|
| 1 | grep | literal / regex | `*.{cs,py,sql,md,json,config}` | regex hatası = satır içi uyarı |
| 2 | glob | `**/*.spec.md` | — | klasör + uzunluk desteğidir |
| 3 | büyük/küçük harf | kullanıcı seçimi | **duyarsız** | `Aa` ile değişir |
| 4 | tüm dosya / seçimdeki | — | tüm proje | seçim = Solution Explorer seçimi |

### §2.2 Akış

```
+-----------+  kalıp + filtre  +---------------------------+
| SearchCtrl| ---------------► | tek async tarama (iptal)  |
+-----------+                  | .git/bin/obj ATLANIR      |
                               +------------+--------------+
                                            ▼
                    +------------------------------------------+
                    | GridControl: dosya:satır · eşleşme satırı |
                    | src/Core/Result.cs:42   public Result…   |
                    | tests/Core/ResultTests.cs:7  Assert…     |
                    +------------+-----------------------------+
                                 | tıkla
                                 ▼
                    CodeEditor: sekme aç / odakla + satıra git
```

### §2.3 Davranış kuralları

| # | Kural | Kabul |
|---|---|---|
| A1 | **150 ms** debounce; yeni kalıp eski taramayı iptal eder | öksüz tarama = 0 |
| A2 | Sonuç üst sınırı **1.000**; aşarsa sayaç "1000+" | liste her zaman akıcı |
| A3 | Atlanan dizinler kullanıcıya görünür (".git, bin, obj atlandı") | gizli eksik yok |
| A4 | İkinci tarama varken öncekini **öldür** | eşzamanlı tarama = 1 |
| A5 | Sonuç satırında eşleşme **vurgulanır**, satır numarası tıklanabilir | tıklama satıra gider |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| `Regex` davranışı | BCL `System.Text.RegularExpressions` | aynı | **fark yok** |
| Dosya okuma | `File.ReadLines` (satır tembel) | aynı | fark yok |
| İptal | `CancellationToken` | aynı | fark yok |

> Tarama motoru ortak kaynaktadır; `#if` kullanılmaz (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Regex geçerliliği + hata yolu | birim test | geçersiz regex UI'ı kilitlemez |
| 2 | Atlanan dizinler | birim test (temp ağaç) | `.git`/`bin` sonuçta yok |
| 3 | Üst sınır | birim test | 1.000 + sayaç |
| 4 | İptal | birim test | eski tarama sonucu UI'a düşmez |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
