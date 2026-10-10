---
title: "Solution Explorer Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Proje/ağaç modelinin dosya sistemi görünümü - tarama, yenileme, workspace sınırı, dosya işlemleri ve editöre çift tık köprüsü"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Solution Explorer Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/project-model-spec|project-model]] · [[spec/code-editor-spec|code-editor]] · [[spec/search-grep-spec|search-grep]]

> **Tek cümlelik sorumluluk:** *Açık projenin dosya ağacı nereden gelir, nasıl tazelenir ve hangi dosya işlemi hangi sınır içinde kalır* sorusunun tek cevabıdır.
> Ağaç **gösterimdir**: diske yazar, veritabanına yazar; modeli [[spec/project-model-spec|project-model]] taşır.

---

## §0 Özet (Answer First)

Ağaç `TreeList` üzerinde yaşar; düğümler **proje → klasör → dosya** hiyerarşisidir ve kaynak **gerçek disk**tir (workspace kök dizini). Tarama `System.IO` ile yapılır, **arka planda** yürür; 1.000 dosyada ≤ 2 sn (VC-PRD-081). Yenileme düğüm bazlıdır, tüm ağaç yeniden kurulmaz. Dosya/dizin işlemleri workspace kökünün **dışına çıkamaz** (`..`/mutlak yol reddedilir, SEC-11), yıkıcı olanlar onay ister (G7).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Ağaç yükleme, tarama, filtre, yenileme | Proje kaydının veritabanı modeli | [[spec/project-model-spec|project-model]] |
| 2 | Dosya/dizin oluşturma, taşıma, yeniden adlandırma | Editör sekmesi/dirty davranışı | [[spec/code-editor-spec|code-editor]] |
| 3 | Silme **onayı** + sonucun gösterimi | Bildirim/toast biçimi | [[spec/notification-spec|notification]] |
| 4 | Çift tık → `open.file` komutu (l2 handler) | Komutun çalıştırılması (kuyruk) | [[spec/task-queue-spec|task-queue]] |
| 5 | Dosya adı filtresi (yerel) | Proje geneli grep/glob araması | [[spec/search-grep-spec|search-grep]] |

---

## §2 Davranış

### §2.1 Ağaç yapısı

```
+-----------------------------------------------------+
| [SearchControl: dosya adı filtresi]      [Yenile ▻] |
+-----------------------------------------------------+
| ▼ MySolution                                        |
|   ▼ src                                             |
|     ▶ VersaCoder.Core        (klasör)               |
|       Program.cs             (dosya) ◀ çift tık     |
|     ▼ docs                                         |
|       README.md                                   |
|   ▶ tests                                          |
|   ⚠ build.log            (izin yok / okunamadı)     |
+-----------------------------------------------------+
```

### §2.2 Tarama ve yenileme

| # | Olay | Davranış | Sınır |
|---|---|---|---|
| E1 | Proje açılınca | Kökten derinlik öncelikli tarama, düğümler **lazy** (klasör genişletilince) | debounce 300 ms |
| E2 | `Refresh` (F5) | Yalnız **seçili** düğümün alt ağacı tazelenir; silinen düğüm kaldırılır | tam ağaç yeniden kurulmaz |
| E3 | Dış değişiklik (`FileSystemWatcher`) | StatusBar'da "diske değişiklik algılandı" + tek tıkla yenileme | otomatik sürüklenme yok |
| E4 | Tarama sürerken | `MarqueeProgressBarControl` görünür; iptal edilebilir | iptal = kısmi ağaç + uyarı |
| E5 | Uzun klasör (> 500 çocuk) | Düğüm "…daha fazla göster" ile sayfalanır | UI donması yok |

### §2.3 Dosya işlemleri

| # | İşlem | Arayüz | Kural |
|---|---|---|---|
| F1 | Yeni dosya/klasör | `ContextMenuStrip` | ad doğrulama; çakışma = hata mesajı |
| F2 | Yeniden adlandırma | satır içi düzenleme (`TextEdit`) | yalnız workspace içi, geçersiz ad reddedilir |
| F3 | Taşıma | sürükle-bırak | hedef workspace dışındaysa **reddedilir** (SEC-11) + log |
| F4 | Silme | onay dialoğu (**G7**) | iptal = hiçbir değişiklik yok |
| F5 | Çift tık / Enter | `open.file` komutu | aynı dosya ikinci kez açılmaz, sekme odaklanır (VC-PRD-010) |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Tarama API | `Directory.EnumerateDirectories` (BCL) | aynı | **fark yok** |
| Dosya izni | `UnauthorizedAccessException` yakalama | aynı | fark yok |
| Uzun yol (`\\?\`) | sınırlı | `longPathAware` varsayılan | `#if` yalnız gerekirse |

> Kod taraması `System.IO` üzerinden yürür; **EF Core yok**, dosya listesi DB'ye kopyalanmaz (yalnız son proje kaydı DB'dedir — [[spec/persistence-spec|persistence]]).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Tarama hızı (1.000 dosya) | ölçüm çıktısı | ≤ 2 sn, UI akıcı |
| 2 | Workspace kaçışı | birim test: `../` ve `C:\` hedefi | 0 kabul, olay loglanır |
| 3 | Yenileme tutarlılığı | birim test: silinen dosya ağaçta kalmaz | 0 sahte düğüm |
| 4 | Çift tık → sekme | smoke: aynı dosya 2. kez açılmaz | sekme sayısının artışı 0 |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
