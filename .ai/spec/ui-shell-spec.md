---
title: "UI Shell Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Main IDE RibbonForm kabuğu: ribbon/menü, dock panel yerleşimi, durum çubuğu, pencere yönetimi - ne görünür, ne nereye bağlanır"
---

# UI Shell Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [solution-explorer](solution-explorer-spec.md) · [code-editor](code-editor-spec.md) · [theming](theming-spec.md)

> **Tek cümlelik sorumluluk:** *Ana pencere (RibbonForm) neyi barındırır, ribbon/dock/durum çubuğu nasıl yerleşir ve pencere kapanınca ne olur* sorusunun tek cevabıdır.
> Kabuk **yalnızca yerleşim ve yönlendirme** yapar; iş mantığına, SQL'e, dosya I/O'una bakmaz (l7 izolasyonu).

---

## §0 Özet (Answer First)

Tek ana pencere `MainForm` bir **`RibbonForm`**'dur; ribbon stili **yalnız** `RibbonControlStyle.Office2010` (ADR-004). Gövde `DockManager` ile 4 sabit dock bölgesi (Solution Explorer sol, AI Chat sağ, Agent Panel sağ-alt, Terminal alt) + ortada `XtraTabControl` editör alanıdır. Altta `StatusBarControl` durum mesajını gösterir. Kabuk, olayları **yalnız** `ICommandDispatcher` (l2 handler) üzerinden iletir; doğrudan dosya/DB/LLM çağırmaz.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | `MainForm` (`RibbonForm`) + `RibbonControl` yerleşimi | Ribbon **görünüm kararı** (skin/koyu-açık) | [theming](theming-spec.md) |
| 2 | `DockManager` panel yerleşimi, göster/gizle, hizalama | Panel içeriği davranışı | ilgili kardeş şartnameler |
| 3 | `StatusBarControl` mesaj bölgeleri + kısayollar | Hata sınıflandırma ve gösterimi | [error-handling](error-handling-spec.md) |
| 4 | Pencere durumu (aç/kapa/min/max), çıkış onayı, kurtarma | Kalıcılık biçimi (nerede saklanır) | [persistence](persistence-spec.md) |
| 5 | Ribbon butonu → komut yönlendirmesi | Komutun kendisi (görev kuyruğu) | [task-queue](task-queue-spec.md) |

---

## §2 Davranış

### §2.1 Kabuk yerleşimi

```
+----------------------------------------------------------------------+
| RibbonControl (RibbonControlStyle.Office2010)  [Home|Agent|View|Help]|
+----------+-------------------------------------------+---------------+
| TreeList | XtraTabControl  (bir sekme = bir dosya)   | AI Chat       |
| Solution |                                           | (MemoEdit +   |
| Explorer |                                           |  ListBox)     |
| (dock L) +-------------------------------------------+ (dock R)      |
|          | Agent Panel (GridControl, durum sütunlu)  |               |
+----------+-------------------------------------------+---------------+
| Terminal (dock alt, ReadOnly MemoEdit + komut TextEdit)              |
+----------------------------------------------------------------------+
| StatusBarControl: durum · dosya · kodlama · sağlayıcı · görev sayacı |
+----------------------------------------------------------------------+
```

### §2.2 Ribbon grupları

| # | Sekme | Grup | Öğeler (BarButtonItem) | Gittiği komut |
|---|---|---|---|---|
| 1 | Home | Project | New, Open, Save All, Close | `project.*` handler (l2) |
| 2 | Home | Edit | Undo, Redo, Find, Replace | `editor.*` handler |
| 3 | Agent | Run | Run Task, Pause, Cancel, Resume | `task.*` ([task-queue](task-queue-spec.md)) |
| 4 | Agent | Chat | New Chat, Clear Context, Stop | `chat.*` ([ai-chat](ai-chat-spec.md)) |
| 5 | View | Layout | Explorer, Chat, Agent Panel, Terminal (toggle) | dock görünürlüğü |
| 6 | Help | Info | Docs, About, Diagnostics | [observability](observability-spec.md) ekranı |

### §2.3 Kabuk kuralları

| # | Kural | Kabul |
|---|---|---|
| S1 | Her ribbon öğesi **tek** komut çağrısı yapar; SQL/IO/LLM **yok** | Kod taraması: `using ...Data|Infrastructure` = 0 |
| S2 | `RibbonControlStyle` yalnız `Office2010` | Metin araması: `Office2010` dışı stil = 0 (G3) |
| S3 | Panel kapatılırsa içerik **gizlenir**, yok edilmez | Yeniden açılışta durum (seçili sekme, filtre) korunur |
| S4 | Çıkışta kirli sekme varsa 3 seçenekli onay (kaydet/vazgeç/iptal) | İptal = pencere kapanmaz; kaydarma veri kaybı üretmez |
| S5 | Pencere kapanışında çalışan görevler `paused` işaretlenir | Yeniden açılışta kurtarma görünür (VC-PRD-085) |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| DPI | `App.config` `<DpiAwareness>` + DevExpress `DpiHelper` | `Application.SetHighDpiMode(PerMonitorV2)` | `#if` **yalnız** bu farkta; kod ortak |
| Ribbon/Docking sürümü | DevExpress **v26.1** aynı | DevExpress **v26.1** aynı | G3 — sürüm farkı yok |
| Form başlık/çerçeve | `FormBorderStyle` davranışı aynı | aynı | fark yok |

> **KURAL:** Kabuk kodu `#if` içeriyorsa gerekçesi **yalnız platform farkı** olmalıdır (ADR-001 / G6); `#if NET48` altına iş mantığı yazılmaz.

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | İki bacakta derleme | `dotnet build -f net48` · `-f net10.0-windows` | 0 hata |
| 2 | Kabuk smoke: form açılır, 4 dock bölgesi görünür | manuel / smoke test | 4 panel + ribbon + statusbar |
| 3 | Ribbon stili kilidi | kod denetimi / NetArchTest | `Office2010` dışı = 0 |
| 4 | Katman izolasyonu | `Architecture/LayerRulesTests.cs` | l7 → l1/l4 referansı = 0 |
| 5 | Bozuk link | `check-links.ps1` | 0 (GATE-004a) |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
