---
title: "Kod Editörü Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Metin/kod görüntüleme ve düzenleme, sekme yaşam döngüsü, sözdizimi gösterimi, satır numarası, kaydetme ve kirli durum"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Kod Editörü Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [solution-explorer](solution-explorer-spec.md) · [diff-view](diff-view-spec.md) · [search-grep](search-grep-spec.md)

> **Tek cümlelik sorumluluk:** *Dosya nasıl açılır, nasıl gösterilir, nasıl kaydedilir ve sekme hangi kurallarla kapanır* sorusunun tek cevabıdır.
> Editör **gösterim + tampon**dur; dosya I/O'nu yapan katman l4, komut yönlendirmesi l2 handler'dır.

---

## §0 Özet (Answer First)

Görüntüleme yüzeyi `RichEditControl` (sözdizimi renklendirme + satır numarası) ile `XtraTabControl` (bir sekme = bir dosya) üzerinden kurulur; `MemoEdit` hafif yedek yüzeydir (**OQ-3** kararı GATE-003'te kesinleşir). Dosya UTF-8 açılır, `Ctrl+S` ile diske yazar, sekme başlığındaki `*` kirli durumu gösterir. Kapanışta kirli sekme **3 seçenekli** onay ister; kaydarma veri kaybı üretmez (VC-PRD-012).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Sekme aç/kapa/odakla, kirli gösterge | Ağaçtan çift tık tetikleyicisi | [solution-explorer](solution-explorer-spec.md) |
| 2 | Metin gösterimi, satır numarası, sözdizimi | Kaynak dosyanın modeli/DTO'su | [project-model](project-model-spec.md) |
| 3 | Kaydet / tümünü kaydet / geri al-yinele | Yerel arama çubuğu dışında proje araması | [search-grep](search-grep-spec.md) |
| 4 | Değişiklik kabul/geri alma görünümü çağrısı | Diff algoritması ve kabul akışı | [diff-view](diff-view-spec.md) |
| 5 | Dosya I/O çağrısı (l4 portu) | DB/LLM/agent görevi | [task-queue](task-queue-spec.md) |

---

## §2 Davranış

### §2.1 Editör yerleşimi

```
+------------------------------------------------------------+
| XtraTabControl: [Program.cs*] [Result.cs] [util.py]  [✕]   |
+------+-----------------------------------------------------+
|  12  | using VersaCoder.Domain;          ◀ satır numarası  |
|  13  |                                              |      |
|  14  | public static class Util {                   |      |
|  15  |     // TODO: yeniden adlandır                 |      |
+------+-----------------------------------------------------+
| [SearchControl: bul/değiştir]   UTF-8 · CRLF · C# · satır  |
+------------------------------------------------------------+
```

### §2.2 Sekme kuralları

| # | Kural | Kabul |
|---|---|---|
| T1 | Aynı dosya ikinci kez açılmaz → mevcut sekme odaklanır | sekme sayısı artmaz (VC-PRD-010) |
| T2 | Kirli sekme başlıkta `*` ile işaretlenir | kaydetme sonrası `*` kalkar |
| T3 | Kapanışta kirli: Kaydet / Vazgeç / İptal | İptal = sekme açık kalır |
| T4 | Otomatik yedek **yalnız** bellek içi (crash recovery) | diske gölge dosya yazılmaz |
| T5 | Yeniden açılışta önceki sekme kümesi geri gelir | [persistence](persistence-spec.md) oturum kaydı |

### §2.3 Görüntüleme ve dil

| # | Özellik | Davranış |
|---|---|---|
| G1 | Satır numarası | sol marj; kaydırma ile senkron; gizlenebilir |
| G2 | Sözdizimi | dosya uzantısına göre kural seti (`.cs/.py/.sql/.md/.json`) |
| G3 | Kelime/dizi sarma | `#region` **yok** (coding-standards); katlama yalnız görsel |
| G4 | Kodlaması bozuk dosya | okunamazsa Türkçe hata + "hex/ham aç" seçeneği yok → [error-handling](error-handling-spec.md) |
| G5 | Büyük dosya (≥ 2 MB) | salt-okunur uyarı + ikinci parça yükleme (`NEEDS_VALIDATION`) |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Encoding API | `Encoding.UTF8` (BOM ayırt edilir) | aynı | **fark yok** |
| Sözdizimi motoru | DevExpress **v26.1** aynı | aynı | G3 |
| Yerel dosya uzun yolu | sınırlı | destekli | `#if` yalnız gerekirse, platform farkı |

> Ortak kaynak dosyası her iki bacakta **aynı**dır; `#if` yalnız dosya yolu/uzun yol gibi platform farkında kullanılır (G6).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | UTF-8 tur (aç → kaydet) | birim test: bayt eşitliği | BOM/değişme kayıpsız |
| 2 | Kapanış akışı (kirli sekme) | birim test: Kaydet/Vazgeç/İptal dalları | 3 dal da deterministik |
| 3 | Tekrarlı açma | smoke: aynı dosya 2. sekme yapmaz | sekme artışı 0 |
| 4 | İki bacak build | `dotnet build -f net48` · `-f net10.0-windows` | 0 hata |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
