---
title: "UI Wireframe 05 — AI Chat (Yapay Zeka Sohbeti)"
type: spec
category: ui
status: draft
version: 1.1.0
updated: 2026-10-04
authority: SSOT
scope: "05-ai-chat ekranının wireframe'i: akış render, giriş/kontrol düğmeleri, model seçici, dosya üçlüsü, etkileşim kuralları — GATE-003 onayı bekliyor"
---

# 05 — AI CHAT

**Zorunlu Bağlantılar:** [ui-design/index](README.md) · [02-main-ide](02-main-ide.md) · [06-agent-panel](06-agent-panel.md) · [.rules/async-rules.md](../.rules/async-rules.md) · [.rules/security-rules.md](../.rules/security-rules.md) · [keys](../keys.md)

> **Tek cümlelik sorumluluk:** Sohbet ekranının hangi bölgelerden oluştuğu, akışın/iptalin
> ve "dosyaya uygula" adımının nereye gittiği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`05-ai-chat`, `02-main-ide`'nin **sağ üst dock** bölgesidir: üstte oturum + model seçici,
ortada salt-okunur mesaj akışı (`RichEditControl`, kod blokları dahil), altta giriş kutusu ve
3 aksiyon butonu. Akış (streaming) her chunk'ta UI güncellenir; durdurma `CancellationToken`
iletir. **Durum: GATE-003 onayı alındı (2026-10-01).**

| Bölge | Kontrol | Gösterim |
|---|---|---|
| Üst bar | `LookUpEdit` ×2 + `SimpleButton` | oturum + model seçimi, temizle |
| Mesaj akışı | `RichEditControl` (salt okunur) | rol başlığı `KULLANICI`/`ASSISTANT`, kod blokları |
| Durum satırı | `ProgressBarControl` | akış ilerlemesi + token sayacı |
| Giriş | `MemoEdit` | Enter gönder / Shift+Enter alt satır |
| Aksiyon | `SimpleButton` ×3 | Gönder · Bağlam ekle · Durdur |

---

## §1 Wireframe

```text
+----------------------------------------------------------------------+
| AI Chat   [Oturum: oturum-001 v]  [Model: mimo-free v]  [*] [Sil]    |
+----------------------------------------------------------------------+
| +-- KULLANICI ----------------------------------------------------+  |
| | MainForm'a Ribbon ekle                                           | |
| +-----------------------------------------------------------------+  |
| +-- ASSISTANT ----------------------------------------------------+  |
| | Ribbon, MainForm.Designer.cs icerinde InitializeComponent ile    | |
| | kurulur. .cs'ye UI kodu yazılmaz.                               |  |
| |                                                                 |  |
| | Onerilen değişiklik:                                            |  |
| | ```csharp                                                       |  |
| | // MainForm.Designer.cs                                         |  |
| | this.ribbonControl1 = new DevExpress.XtraBars.Ribbon...         |  |
| | ```                                                             |  |
| |                            [Dosyaya uygula] [Kopyala]           |  |
| +-----------------------------------------------------------------+  |
| ~ akış sürüyor...                                token: 1.240/8.000  |
+----------------------------------------------------------------------+
| [ Mesaj yaz... (Enter=gönder, Shift+Enter=alt satır)               ] |
| [Gönder]  [Bağlam ekle]  [Durdur]                                    |
+----------------------------------------------------------------------+
```

**Durumlar:** boş oturum → "Soru yazın" + 3 öneri; akış durdu → "Durduruldu" + Gönder etkin;
hata (401/429/timeout) → mesaj alanında neden + **retry** butonu (api anahtarı **asla** görünmez).

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `AiChatPanel.cs` | gönder/durdur olayları, `IChatService` akış abonesi (chunk), mesaj ekleme, uygulama onayı | `developer` |
| `AiChatPanel.Designer.cs` | sohbet görünümü, giriş kutusu, butonlar, model `LookUpEdit`, progress bar | **`ui-designer`** |
| `AiChatPanel.resx` | rol etiketleri (KULLANICI/ASSISTANT), boş durum metni, ikonlar | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | Akış her chunk'ta | UI thread marshal (`Invoke`); UI **donmaz** |
| 2 | Durdur | `CancellationToken` servise iletilir; UI iptal durumu gösterir |
| 3 | "Dosyaya uygula" | **onay dialog** → `IDiffService` + `IFileWriter`; doğrudan yazma **YOK** |
| 4 | Model seçimi + endpoint | servis katmanında; [keys](../keys.md) yalnız meta harita |
| 5 | `Bağlam ekle` | seçili dosya/ağaç düğümü prompt'a bağlam olarak girer |
| 6 | "Sekmede Aç" (2026-10-02) | sohbet `10-ai-chat-console` tam ekran sekmesinde açılır (CLI/AI konsolları ayrı yüzey, 02-main-ide dock'u referanstır) |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| Akış render | `RichEditControl` HTML append; `net48`'de `AppendHtml` API farkı çıkarsa `#if` **yalnız** bu metotta |
| Güvenlik | API anahtarı gizli; yanıtta anahtar **asla** gösterilmez (`.rules/security-rules.md`) |
| Katman | UI `l7`; akış/protocol `l5`; model router `l6`; l0 portu `IAgentStreamSink` (ADR-028 §2.3) |
| Erişilebilirlik | mesajlar okuyucu rol etiketiyle (`KULLANICI`/`ASSISTANT`); token sayacı metin olarak da yazılır; yalnız renk/animasyon bilgi taşımaz |

---

## §5 GATE-003 Durumu

| # | Öğe | Durum |
|---|---|---|
| 1 | Wireframe yazıldı | evet (bu dosya) |
| 2 | Kullanıcı onayı | **bekliyor** |
| 3 | Kod izni | kapalı |

---

## §6 Kanıt

| # | Kaynak | Ne kanıtlar |
|---|---|---|
| 1 | Kaynak wireframe `ui-design/05-ai-chat.md` (eski vault) | bölge/buton listesi |
| 2 | [.rules/async-rules.md](../.rules/async-rules.md) | akış + iptal kalıbı |
| 3 | [.rules/security-rules.md](../.rules/security-rules.md) | anahtar gizliliği |
| 4 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.3 | `IAgentStreamSink` portu (l0) |

> `IChatService` / `IDiffService` adları taslaktır — `l0` içinde kesinleşmesi `DOĞRULAMA GEREKLİ`.
