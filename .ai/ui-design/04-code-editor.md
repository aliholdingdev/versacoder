---
title: "UI Wireframe 04 — Code Editor (Kod Editörü)"
type: spec
category: ui
status: draft
version: 1.1.0
updated: 2026-09-29
authority: reference
scope: "04-code-editor ekranının wireframe'i: sekme şeridi, kod alanı, outline ve problemler paneli, dosya üçlüsü, etkileşim kuralları — GATE-003 onayı bekliyor"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 04 — CODE EDITOR

**Zorunlu Bağlantılar:** [ui-design/index](readme.md) · [02-main-ide](02-main-ide.md) · [03-solution-explorer](03-solution-explorer.md) · [.rules/data-access-rules.md](../.rules/data-access-rules.md) · [spec/test-strategy.md](../spec/test-strategy.md) · [devexpress-rules](../.rules/devexpress-rules.md)

> **Tek cümlelik sorumluluk:** Kod editörünün hangi panellerden oluştuğu, kaydetme/analiz
> akışının nereye gittiği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`04-code-editor`, `02-main-ide`'nin **merkez bölgesidir**: sekme şeridi (`XtraTabControl`) +
kod alanı (`RichEditControl`) + sağ panelde Outline (`TreeList`) ve Problemler (`GridControl`) +
altta autosave/derleme durumu. Kaydetme **servis üzerinden** (`IFileWriter`) — editör dosyaya
doğrudan yazmaz. **Durum: GATE-003 onayı alındı (2026-10-01).**

| Bölge | Kontrol | Gösterim |
|---|---|---|
| Sekme şeridi | `XtraTabControl` | dosya adı + kirli (`*`) + kapat (x) |
| Kod alanı | `RichEditControl` | satır no paneli, seçili satır vurgusu |
| Sağ üst | `TreeList` (Outline) | `v`/`>` düğüm aç/kapat, `o` üye |
| Sağ alt | `GridControl` (Problems) | seviye `WAR`/`BIL` + kod + satır |
| Alt bar | `ProgressBarControl` | autosave sayacı + derleme sonucu |

---

## §1 Wireframe

```text
+----------------------------------------------------------------------+
| [Program.cs x] [MainForm.cs x] [Domain/Entity.cs x]   [+ yeni sekme] |
+----------------------------------------------------------------------+
| 1  | using System;                 | WAR: 0   BIL: 2                 |
| 2  | using VersaCoder.Domain;      +---------------------------------+
| 3  |                               | OUTLINE (TreeList)              |
| 4  | namespace VersaCoder.Desktop  | v class MainForm                |
| 5  | {                             |     o InitializeComponent       |
| 6  |   public partial class Main.. |     o OnLoad                    |
| 7  |   {                           | v class Program                 |
| 8  |     > InitializeComponent();  |     o Main                      |
| 9  |   }                           +---------------------------------+
| 10 | }                             | PROBLEMLER                      |
|    |                               | BIL CS0168 (satır 12)           |
+----------------------------------------------------------------------+
| [Ctrl+B derle] | Kaydedildi 09:41 | Dosya: MainForm.cs | C# | UTF-8  |
+----------------------------------------------------------------------+
  ^ satır numarası       ^ seçili satır vurgusu   ^ autosave progress
```

**Durumlar:** dosya yükleniyor → kod alanında tek satır ProgressBar;
analiz yok → Problems "tarama yapılmadı" satırı; derleme hatası → ilgili satır arkası vurgu + çift tık Problemler'de satıra gider.

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `CodeEditorPanel.cs` | sekme aç/kapat, dosya oku-yaz, dirty flag, autosave tetikleyici, `ICodeAnalysisService` aboneliği | `developer` |
| `CodeEditorPanel.Designer.cs` | `XtraTabControl` + her sekme editör kurulumu, outline `TreeList`, Problems `GridControl` | **`ui-designer`** |
| `CodeEditorPanel.resx` | sekme ikonları, dil etiketleri, durum metinleri | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | Kaydet `Ctrl+S` | servis (`IFileWriter`); UI dosyaya doğrudan yazmaz |
| 2 | Kirli sekme kapanışı | **onay** dialog'u (kaydet / kapat / vazgeç) |
| 3 | Değişiklik | Outline + Problems `ICodeAnalysisService` olayıyla yenilenir |
| 4 | Syntax renklendirme | editör kontrolünün kendi özelliği; tema `.resx`/theme dosyasından |
| 5 | Problemler'de çift tık | dosya + satır sekmeye açılır |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| `RichEditControl` syntax modu | her iki bacakta mevcut (DevExpress.Win 26.1 — `net462` + `net8.0` varlıkları); API farkı çıkarsa `#if NETFRAMEWORK` **yalnız** bu metotta |
| UTF-8 | dosya kodlaması UTF-8; bozuk bayt `U+FFFD` olarak gösterilir, **çökme yok** (spec/terminal-spec §2.2 O3) |
| Katman | editör `l7`; analiz servisi `l6`→`l2`; derleme çıktısı `l5` (tool schema) |
| Erişilebilirlik | `Ctrl+Tab` sekme döngüsü; satır no okunabilir (kontrast); yalnız renkle hata ayrımı **yapılmaz** (kod + metin) |

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
| 1 | Kaynak wireframe `ui-design/04-code-editor.md` (eski vault) | panel/yerleşim listesi |
| 2 | [spec/test-strategy.md](../spec/test-strategy.md) | editör yollarının test kapsamı |
| 3 | [devexpress-rules](../.rules/devexpress-rules.md) | sürüm + `#if` kuralı |
| 4 | [.rules/data-access-rules.md](../.rules/data-access-rules.md) DA-01 | dosya I/O serviste |

> `ICodeAnalysisService` adı taslaktır — `l0` içinde kesinleşmesi `DOĞRULAMA GEREKLİ`.
