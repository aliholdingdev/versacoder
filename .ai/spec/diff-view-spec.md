---
title: "Diff Görünümü Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Değişiklik gösterimi (satır diff), kabul/geri alma eylemleri, onay kuralı ve değişiklik sonrası dosya yazımı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Diff Görünümü Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [code-editor](code-editor-spec.md) · [error-handling](error-handling-spec.md) · [persistence](persistence-spec.md)

> **Tek cümlelik sorumluluk:** *İki metin arasındaki fark nasıl gösterilir ve bir değişiklik ne zaman kabul edilir/geri alınır* sorusunun tek cevabıdır.
> Diff **salt okunur bir görüntüdür**; dosyaya yalnız kabul/geri alma eylemi yazar.

---

## §0 Özet (Answer First)

Görünüm iki sütunlu (`+`/`−` satır vurgulu) ya da birleşik (unified) moddadır; hesaplama **satır bazlı** yapılır ve UI dışında çalışır. Eylemler: **Kabul et** (diff satırını dosyaya uygular), **Geri al** (özgün hale döner), **Tümünü kabul**. Geri alma **yıkıcıdır** → onay zorunludur (G7). Uygulama sonrası dosya UTF-8 olarak yazılır, editör sekmesi yenilenir ve kirli işaretlenir.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Satır farkı hesaplama + görselleştirme | Kod editörü sekmesi/dirty | [code-editor](code-editor-spec.md) |
| 2 | Kabul/geri alma eylemi ve onayı | Kayıt/oturum saklama biçimi | [persistence](persistence-spec.md) |
| 3 | Hata durumu (dosya değişmiş/kilitli) | Hata kodu sınıflandırma | [error-handling](error-handling-spec.md) |
| 4 | Agent çıktısı diff'inin açılması | Görev kuyruğu/durumu | [task-queue](task-queue-spec.md) |
| 5 | Ağaçtaki "değişti" rozetinin kaynağı | Ağaç gösterimi | [solution-explorer](solution-explorer-spec.md) |

---

## §2 Davranış

### §2.1 Görünüm

```
+--------------------------------------------------------------+
| Diff: src/Core/Result.cs        [2 sütun ▾]  [Undo tümü]     |
+------------------+-------------------------------------------+
| 40  -            | 40  +                                     |
|     public bool  |     public bool IsSuccess => Error is null|
| 41  HasValue()   | 41  // not: Türkçe yorum                  |
+------------------+-------------------------------------------+
|  +3 eklendi · −1 silindi · 4 dosya değişti                   |
|  [Kabul et ▾]  [Geri al]  [Tümünü kabul]                     |
+--------------------------------------------------------------+
```

### §2.2 Eylem tablosu

| # | Eylem | Sonuç | Onay |
|---|---|---|---|
| D1 | Kabul et (tek satır/hunk) | o hunk dosyaya uygulanır, görsel yeşile döner | hayır |
| D2 | Geri alma (tek hunk) | özgün satırlara dönülür | **evet (G7)** |
| D3 | Tümünü kabul | tüm hunks uygulanır; dosya tek yazarışta yazılır | hayır |
| D4 | Tümünü geri al | başlangıç görüntüsüne tam dönüş | **evet (G7)** |
| D5 | Dosya bu arada değişmiş (kilit/çakışma) | eylem reddedilir + "yeniden yükle" | — |

### §2.3 Davranış kuralları

| # | Kural | Kabul |
|---|---|---|
| H1 | Fark hesabı **satır bazlı** (LCS); 5.000 satırda ≤ 500 ms | ölçüm kanıtı |
| H2 | Eşzamanlı tek diff hesabı; yeni istek eskisini iptal eder | öksüz hesap = 0 |
| H3 | Yazma **tek seferde** (atomik); kısmi yazım yok | yarım dosya üretemezsin |
| H4 | Eylem sonrası satır sayısı gösterilir (`+3/−1`) | sayılar dosyayla eşleşir |
| H5 | Eylemler günlüğe yazılır (hangi dosya, ne zaman) | [observability](observability-spec.md) kaydı |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Diff algoritması | ortak C# (saf) | aynı | **fark yok** |
| Dosya yazma | `File.WriteAllText` (UTF-8) | aynı | fark yok |
| Dosya kilidi | `FileShare` bayrakları | aynı | fark yok |

> Orak kaynak; `#if` yok (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Diff doğruluğu (ekle/sil/değiştir) | birim test (bilinen örnekler) | 3/3 senaryo |
| 2 | Kabul → dosya içeriği | entegrasyon (temp dosya) | bayt = beklenen |
| 3 | Geri alma onayı | birim test | onaysız geri alma = 0 |
| 4 | Atomik yazım (hata durumunda) | birim test (kilitli dosya) | eski içerik korunur |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
