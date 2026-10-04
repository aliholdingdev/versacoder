---
title: "UI Wireframe 02 — Main IDE (Ana Pencere / Kabuk)"
type: spec
category: ui
status: draft
version: 1.1.0
updated: 2026-10-04
authority: SSOT
scope: "02-main-ide ekranının wireframe'i: dock bölgeleri, panel haritası, dosya üçlüsü, etkileşim kuralları — MainForm tasarım girdisi, GATE-003 onayı bekliyor"
---

# 02 — MAIN IDE

**Zorunlu Bağlantılar:** [ui-design/index](README.md) · [01-start-page](01-start-page.md) · [03-solution-explorer](03-solution-explorer.md) · [04-code-editor](04-code-editor.md) · [05-ai-chat](05-ai-chat.md) · [06-agent-panel](06-agent-panel.md) · [07-settings](07-settings.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cümlelik sorumluluk:** Ana pencerenin hangi bölgelerden oluştuğu ve her bölgenin hangi
> ekran dosyasına karşılık geldiği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`02-main-ide`, **`MainForm` tasarım girdisidir**: ribbon (üst) + sol dock (çözüm ağacı) +
merkez (düzenleyici sekmeleri + çıktı) + sağ dock (sohbet + görev paneli) + StatusBar (alt).
Bölge adları **sabittir** (`.resx` anahtarları = API sözleşmesi). **Durum: GATE-003 onayı alındı (2026-10-01).**

| Bölge | Panel | Karşılık gelen dosya | Kontrol |
|---|---|---|---|
| **Form sınıfı** | Ana pencere | bu dosya | **`MainForm : RibbonForm`** (DevExpress v26.1, `RibbonControlStyle.Office2010`) |
| Ribbon | Dosya/Düzen/Görünüm/Agent | — | `RibbonControl` (Office2010) |
| Sol dock | Solution Explorer | `03-solution-explorer` | `TreeList` + Find Panel |
| Merkez üst | Kod editörü sekmeleri | `04-code-editor` | `XtraTabControl` |
| Merkez alt | Çıktı / Terminal / Problemler | `04-code-editor` (alt şerit) | `XtraTabControl` |
| Sağ dock üst | AI Chat | `05-ai-chat` | `RichEditControl` + giriş |
| Sağ dock alt | Agent Paneli | `06-agent-panel` | `GridControl` + görev ağacı |
| Alt | Status | — | `StatusBarControl` |
| Modal | Ayarlar | `07-settings` | `XtraTabControl` diyaloğu |

---

## §1 Wireframe

```text
+----------------------------------------------------------------------+
| RibbonControl (Office2010):                                          |
| [Dosya:Yeni|Aç|Kaydet|Çık] [Düzen:Geri|İleri] [Görünüm] [Agent]      |
+----------------------------------------------------------------------+
| +----------+------------------------------------------------+-------+|
| | SOL    | MERKEZ (XtraTabControl)                         | SAG   | |
| | DOCK  | [Program.cs x][MainForm.cs x][StartPage.cs x]    | DOCK  | |
| |          | +----------------------------------------------+ | |    |
| | Solution | |  1  | using System;                     |     | | |   |
| | Explorer | |  2  | namespace VersaCoder.Desktop...     |     | | | |
| | (03)     | |  3  | {                                  |     | | |  |
| |          | |  4  |   ...                              |     | | |  |
| |          | +----------------------------------------------+ | |    |
| |          |  Alt: [Çıktı] [Terminal] [Problemler]            | |    |
| |          |  $ dotnet build -> 0 hata                        | |    |
| +----------+------------------------------------------------+   |    |
| | AGENT PANEL (06) - görev ağacı + olay akışı                  |     |
| +------------------------------------------------------------------+ |
| StatusBarControl: Derlendi 09:41 | UTF-8 | C# | 0 hata | ram 210MB   |
+----------------------------------------------------------------------+
```

**Durumlar:** açılışta merkez "Son proje yok → `01-start-page`" (boş alan yasak);
derleme akarken StatusBar "Derleniyor..."; hata varsa StatusBar kırmızı + sayı.

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `MainForm.cs` | Panel aç/kapat, ribbon olay handler'ları, servis bağlamaları (yalnız yönlendirme) | `developer` |
| `MainForm.Designer.cs` | `DockLayoutManager` + tüm panel kurulumu, `RibbonControl`, `StatusBarControl` | **`ui-designer`** |
| `MainForm.resx` | Ribbon metinleri, panel başlıkları, ikonlar (kod içi sabit string **YASAK**) | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | `Ctrl+S` → aktif sekmeyi kaydet (`IFileWriter`) | UI dosyaya doğrudan yazmaz |
| 2 | Panel görünürlüğü `Görünüm` ribbon sayfasından | durum `IDockStateService`'e yazılır |
| 3 | Agent cevabı akarken StatusBar "Yanıt yazılıyor..." | async + `CancellationToken` |
| 4 | Kapanışta açık oturum | `IProjectSession`'a kaydedilir |
| 5 | `DockLayoutManager` panel adları sabittir | `.resx` anahtarları = ad sözleşmesi |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| API farkı | `DockLayoutManager` her iki bacakta aynı API (DevExpress.Win 26.1); fark çıkarsa `#if NETFRAMEWORK` **yalnız** o noktada |
| Mevcut iskelet | var olan `RibbonControl` + `RibbonStatusBar` korunur, panel sistemi sonraki adımda ekler |
| Katman | kabuk `l7` (Desktop); veri/servis çağrıları `l6` → `l2`; formda SQL **yok** (DA-01) |
| Erişilebilirlik | `Ctrl+1..6` panel odak kısayolları; odak halkası görünür; renk tek başına bilgi taşımaz |

---

## §5 GATE-003 Durumu

| # | Öğe | Durum |
|---|---|---|
| 1 | Wireframe yazıldı | evet (bu dosya) |
| 2 | 7 ekran paketi | `README.md` §4 |
| 3 | Kullanıcı onayı | **bekliyor** |
| 4 | Kod izni (`MainForm.Designer.cs` büyütmesi) | kapalı |

---

## §6 Kanıt

| # | Kaynak | Ne kanıtlar |
|---|---|---|
| 1 | Kaynak wireframe `ui-design/02-main-ide.md` (eski vault) | bölge/panel haritası |
| 2 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | l7 + 9 proje düzeni |
| 3 | [devexpress-rules](../.rules/devexpress-rules.md) | v26.1 + `Office2010` |
| 4 | [.rules/data-access-rules.md](../.rules/data-access-rules.md) DA-01 | formda SQL yok |

> `IDockStateService` / `IProjectSession` / `IFileWriter` arayüz adları **taslak**tır —
> l0 (`VersaCoder.Abstractions`) içinde kesinleşmesi `DOĞRULAMA GEREKLİ`.
