---
title: "AI Chat Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Sohbet paneli: mesaj akışı, bağlam çipleri, streaming yanıt gösterimi, iptal/duraklat ve hata geri bildirimi"
---

# AI Chat Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [protocol-spec](protocol-spec.md) · [session-spec](session-spec.md) · [agent-panel](agent-panel-spec.md)

> **Tek cümlelik sorumluluk:** *Kullanıcı mesajı nasıl gönderilir, model yanıtı ekranda nasıl akar ve akış nasıl iptal edilir* sorusunun tek cevabıdır.
> Panel **gösterimdir**: ağ çağrısı (l4) ve şema doğrulaması (l5) bu dosyada değildir.

---

## §0 Özet (Answer First)

Sohbet yüzeyi: geçmiş `ListBoxControl` (mesaj balonları), girdi `MemoEdit` (Ctrl+Enter gönder), ilerleme `ProgressBarControl` + iptal butonu. Yanıt **token token** akar (`VC-PRD-030`); ilk token hedefi **≤ 2 sn**. İptal 2 sn içinde akışı durdurur, yarım yanıt transcript'te `cancelled` etiketiyle kalır. Bağlam (aktif proje + seçili dosya) çip olarak görünür ve **çıkarılan içerik isteğe gitmez**.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Mesaj listesi, girdi, gönderme, otomatik kaydırma | Sağlayıcı/model seçimi | [settings](settings-spec.md) |
| 2 | Streaming parçalarının birleştirilmesi + iptali | HTTP/stream şeması, retry/timeout | [protocol-spec](protocol-spec.md) |
| 3 | Bağlam çipleri (proje/dosya ekle-çıkar) | Dosya okuma (I/O) | [solution-explorer](solution-explorer-spec.md) |
| 4 | Hata/hosgeldin durumlarının görünümü | Hata kodu sınıflandırma | [error-handling](error-handling-spec.md) |
| 5 | Transcript'e yazım tetikleyicisi | Oturum/compaction modeli | [session-spec](session-spec.md) |

---

## §2 Davranış

### §2.1 Panel yerleşimi

```
+--------------------------------------------+
| AI Chat          [bağlam: MySolution +2 ✕] |
+--------------------------------------------+
| +- Kullanıcı -------------------------+    |
| | derleyiciyi çalıştır                 |   |
| +--------------------------------------+    |
| +- Asistan ---------------------------+    |
| | `dotnet build` çalıştırıyorum…  ▍   |    |
| | (cursor = akış sürüyor)             |    |
| +--------------------------------------+    |
+--------------------------------------------+
| ▓▓▓▓▓▓▓░░░  [Stop]                        |
| +--------------------------------------+   |
| | Mesajınızı yazın (Ctrl+Enter)        |   |
| +--------------------------------------+   |
+--------------------------------------------+
```

### §2.2 Akış kuralları

| # | Olay | Davranış | Sınır |
|---|---|---|---|
| A1 | Gönder (Ctrl+Enter) | Mesaj listeye hemen düşer, girdi temizlenir | boş mesaj gönderilemez |
| A2 | İlk token | imleç görünür; `ProgressBarControl` belirsiz mod | hedef ≤ 2 sn (VC-PRD-030) |
| A3 | Parça gelir | birleştirilir, liste **tek** öğe güncellenir (yeniden çizim yok) | parça başına 1 repaint |
| A4 | Stop | akış iptal → yanıt `cancelled` etiketi + devam butonu | iptal ≤ 2 sn (VC-PRD-022) |
| A5 | Ağ/şema hatası | balonda Türkçe hata + "Tekrar dene" | panel kilitlenmez |
| A6 | Oturum yeniden açılınca | transcript geçmişten yüklenir | yeni istek gönderilmez |

### §2.3 Bağlam ve durum

| # | Durum | Gösterim |
|---|---|---|
| C1 | Bağlam açık | çip: proje adı + dosya adı, her biri `✕` ile çıkarılabilir |
| C2 | Bağlam kapalı | "bağlam yok" rozeti; model yalnız mesajı görür |
| C3 | Beklemede (tool onayı) | panel kilitli + [agent-panel](agent-panel-spec.md) onay satırına yönlendirme |
| C4 | Sağlayıcı yok | ayar yönlendirmeli hata kartı ([settings](settings-spec.md)) |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Streaming okuma | `HttpClient` + `CancellationToken` (l4) | aynı | **fark yok** |
| Arayüz güncellemesi | `Control.BeginInvoke` | aynı (UI thread kuralı) | fark yok |
| Charset ayrıştırma | BCL `Encoding` | aynı | fark yok |

> Panel kodu platform-specific API içermez; `#if` kullanılmaz (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Akış birleştirme (parça → tam metin) | birim test (sahte stream) | 100 parça → 1 öğe |
| 2 | İptal gecikmesi | birim test + kronometre | ≤ 2 sn, yarım yanıt etiketli |
| 3 | Boş/uzun mesaj sınırı | birim test | boş reddedilir, taşma kırpılır |
| 4 | Bağlam çıkarmanın etkisi | sahte `ILLMProvider` | çıkarılan çip istekte YOK |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
