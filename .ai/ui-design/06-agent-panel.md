---
title: "UI Wireframe 06 — Agent Panel (Ajan Çalışma Paneli)"
type: spec
category: ui
status: draft
version: 1.1.0
updated: 2026-09-29
authority: reference
scope: "06-agent-panel ekranının wireframe'i: ajan listesi, görev ağacı, olay akışı, dosya üçlüsü, etkileşim kuralları — GATE-003 onayı bekliyor"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 06 — AGENT PANEL

**Zorunlu Bağlantılar:** [ui-design/index](readme.md) · [02-main-ide](02-main-ide.md) · [05-ai-chat](05-ai-chat.md) · [.rules/async-rules.md](../.rules/async-rules.md) · [spec/test-strategy.md](../spec/test-strategy.md) · [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md)

> **Tek cümlelik sorumluluk:** Çalışan ajanların, görev ağacının ve olay akışının hangi
> panellerde yaşandığı, başlat/durdur akışının nereye gittiği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`06-agent-panel`, `02-main-ide`'nin **sağ alt dock** bölgesidir: üstte `GridControl` (aktif
ajanlar: rol + durum + ilerleme), ortada `TreeList` (görev ağacı `TASK-001..`), altta olay
akışı (son 20 satır). Başlat/Durdur komutları servis katmanına gider; panel iş **yapmaz**,
durumu **gösterir**. **Durum: GATE-003 onayı alındı (2026-10-01).**

| Alan | Kontrol | Gösterim |
|---|---|---|
| Ajan listesi | `GridControl` + `GridView` | Ajan · Rol · Durum · İlerleme |
| Görev ağacı | `TreeList` | `v`/`>` aç-kapat, durum ikonu |
| Olay akışı | `MemoEdit` (salt okunur) | `HH:MM:SS kim -> kime: ne` |
| Üst bar | `SimpleButton` ×3 | Başlat · Durdur · Yenile |
| Alt | `ProgressBarControl` / `MarqueeProgressBarControl` | adım ilerleme / çalışıyor |

---

## §1 Wireframe

```text
+----------------------------------------------------------------------+
| AGENT PANEL   [Başlat] [Durdur] [Yenile]     durum: ÇALIŞIYOR        |
+----------------------------------------------------------------------+
| AKTIF AJANLAR (GridControl)                                          |
| +--------------+----------+-----------+------------------+           |
| | Ajan         | Rol      | Durum     | İlerleme         |           |
| +--------------+----------+-----------+------------------+           |
| | orchestrator | koord.   | kosuyor   | [#####.....] 50% |           |
| | developer    | kod      | bekle     | [...........]  0% |          |
| | reviewer     | inceleme | bitti     | [###########]100% |          |
| +--------------+----------+-----------+------------------+           |
+----------------------------------------------------------------------+
| GOREV DAGI (TreeList)                                                |
| v TASK-001 Iskelet kur                       [x] bitti               |
|   v TASK-002 Form üçlüsü                     [>] kosuyor             |
|     o TASK-002a Designer.cs yaz              [ ] bekliyor            |
|   > TASK-003 Test yaz                        [ ] bekliyor            |
+----------------------------------------------------------------------+
| OLAY AKISI (son 20)                                                  |
| 09:41:12 orchestrator -> developer: TASK-002 basladi                 |
| 09:41:30 developer: MainForm.Designer.cs yazildi                     |
| 09:41:41 reviewer: onaylandi                                         |
+----------------------------------------------------------------------+
| [##.........] adım 2/3  | sure 00:41 | iptal: Ctrl+.                 |
+----------------------------------------------------------------------+
```

**Durumlar:** hiç ajan yok → \"Görev başlatın\" satırı; hata → olay akışında kırmızı satır +
neden (timeout / model yanıt vermedi); iptal edilen adım → satır `[iptal]` işaretli kalır
(geçmiş silinmez).

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `AgentPanel.cs` | Başlat/Durdur/Yenile handler'ları, `IAgentOrchestrator` durum aboneliği, olay akışı ekleme | `developer` |
| `AgentPanel.Designer.cs` | `GridControl` kolonları, `TreeList` görev ağacı, `MemoEdit` akış alanı, butonlar | **`ui-designer`** |
| `AgentPanel.resx` | durum metinleri (KOŞUYOR/BEKLİYOR/BİTTİ), rol etiketleri, ikonlar | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | Başlat | servis çağrısı; panel yalnız durumu **yansıtır**, iş yapmaz |
| 2 | Durdur | `CancellationToken` → `Cancelling` durumu gösterilir |
| 3 | Görev ağacında çift tık | ilgili dosya `04-code-editor`'da açılır (varsa) |
| 4 | Olay akışı taşması | yalnız son 20 satır; geçmiş dosyaya log olarak serviste saklanır |
| 5 | Yenile | servis tekrar okunur; **panel yeniden kurulmaz** (sekme durumu korunur) |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| Animasyon | `MarqueeProgressBarControl` her iki bacakta mevcut (DevExpress.Win 26.1) |
| Akış | durum olayları `l6` → `l7` yönlü; UI thread marshal (`Invoke`) — `.rules/async-rules.md` |
| Katman | panel `l7`; orkestrasyon `l6`; görev şeması `l5`; portlar `l0` (ADR-028 §2.3) |
| Erişilebilirlik | durum **metin + ikon** ile yazılır (yalnız renk **yasak**); ilerleme yüzdesi sayısal; odak sırası Başlat→Durdur→Yenile |

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
| 1 | Kaynak wireframe `ui-design/06-agent-panel.md` (eski vault) | panel/alan listesi |
| 2 | [.rules/async-rules.md](../.rules/async-rules.md) | akış + iptal kalıbı |
| 3 | [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) | l6/l5/l0 sorumlulukları |
| 4 | [spec/test-strategy.md](../spec/test-strategy.md) | orkestrasyon test kapsamı |

> `IAgentOrchestrator` adı taslaktır — `l0` içinde kesinleşmesi `DOĞRULAMA GEREKLİ`.
> `TASK-001..` numaraları örnek veri; canlı görevler servisten gelir.
