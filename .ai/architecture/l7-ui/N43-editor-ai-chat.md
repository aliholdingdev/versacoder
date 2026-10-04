---
title: "N43 Editor ve AI Chat"
type: architecture
category: l7-ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N43 - 04-code-editor + 05-ai-chat: sekme yonetimi, toplu stream render, iptal, atomik kaydetme (kayit tablosu DEGIL)
---

# N43 Editor ve AI Chat

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N42-panels-explorer.md](N42-panels-explorer.md)
- [N44-settings-cli.md](N44-settings-cli.md)
- [N52-macro-l7.md](N52-macro-l7.md)
- [../../index.md](../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N43, kodun **sekme sekme** acilip kaydedildigini ve LLM yanitinin **akis halinde, UI donmadan** cizildigini buyur.

## §0 Ozet (Answer First)

- 04-code-editor + 05-ai-chat ekranlarini tasir: `RichEditControl` her dosya icin bir `XtraTabPage`; AI Chat gelen `IStreamDelta`'lari **toplu render** (batch) ile cizer - her baytta repaint **yok**.
- `IRenderScheduler` 50 ms'de bir tek repaint yapar; kuyruk esigi asilirsa kirpar - **UI donmaz**; "Durdur" `CancellationToken` ile akisi aninda keser, yarim metin **isaretlenir, silinmez**.
- Kaydetme l4 N29 **atomik yazma** uzerinden (port + l6 DI); `l7 -> l4` referansi 0; dosya baska surecte aciksa `IO_ERROR` -> kirli kalir.
- Ilerleme kosulu: 1.000 delta -> repaint sayisi <= 20 + `Cancel()` 100 ms icinde (§9 P43-1, P43-2).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `ICodeEditor.Open(path, ct)` / `.SaveAsync(ct)` | dosya <-> editor |
| `interface` | `IDocumentTab.IsDirty` / `.CloseAsync()` | sekme + kirli bayragi |
| `interface` | `IAiChatPanel.SendAsync(prompt, ct)` | akis baslatma/durdurma |
| `interface` | `IStreamRenderer.Enqueue(delta)` / `IRenderScheduler.Flush()` | toplu cizim kuyrugu (50 ms) |
| `interface` | `IDiffView.Show(before, after)` + `ITokenMeter.Used/.Limit` | oneri oncesi/sonrasi + butce gostergesi |

```csharp
public interface IStreamRenderer
{
    void Enqueue(IStreamDelta delta);   // UI thread'e kuyruk
    void Flush();                       // cizim tek noktada, toplu
}
// kural: her delta icin Invalidate() YOK (flicker + UI kilitlenmesi).
// Durdur -> CancellationToken -> akis biter, yarim metin silinir/isaretlenir.
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `prompt` + `CancellationToken` | 05-ai-chat (MemoEdit) | `IAiChatPanel.SendAsync` |
| `IStreamDelta` akisi | l4 N31 SSE | `IStreamRenderer.Enqueue` |
| Dosya yolu | N42 cift tik (`commandId`) | `ICodeEditor.Open` |
| Butce gostergesi | l5 `IContextWindow` (port) | `ITokenMeter` |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Balon cizimi (ChatBubble) | kullanici | 50 ms toplu repaint |
| Kaydetme istegi | l4 N29 (port) | atomik yazma, kismi dosya yok |
| `Result.Fail(CANCELLED / IO_ERROR / FILE_TOO_LARGE / LLM_STREAM_BROKEN)` | StatusBar / hata balonu | istisna degil sonuc |
| `IDiffView` (oneri oncesi/sonrasi) | kullanici | kabul -> editor -> kaydet |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N43 notu |
|---|---|---|
| l7 -> l4 | **X** | HTTP/IO dogrudan yok; port + l6 DI |
| l7 -> l1 | **X** | domain'e referans yok |
| l7 -> l2, l5, l6 | OK | handler + sema + DI |
| l6 -> l7 | **X** | composition root UI'i acmaz |
| -> l7 | **X** | hicbir katman editor/chat'e bakmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N43 | Editor + AI Chat | Bu dosya - 04 + 05 |
| N41 | Ribbon kabuk | Komut (kaydet/ara) hedefi |
| N42 | Cozum gezgini | Dosya acma kaynagi |
| N31 | LLM HTTP (l4) | `IStreamDelta` ureticisi |
| N52 | Makro l7 | `IEditorKit` |

## §6 Projeler

| Proje | N43 katkisi |
|---|---|
| `VersaCoder.Desktop` | `CodeEditorControl` + `AiChatPanel` (designer uclusu: **ui-designer**) |
| `VersaCoder.Tests` | Batch render + iptal + kirli sekme testleri (`Category=Ui`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `l7 -> l1/l4` = 0 |

TFM: `net48;net10.0-windows` - DevExpress `RichEditControl` / `XtraTabControl` / `MemoEdit` **v26.1** iki bacakta ayni; `Control.BeginInvoke` ortak; `#if` yalniz metin kodlama/akis API farki (G9); v26.1 iki bacak kaniti GATE-004 (aksi `NEEDS_VALIDATION`).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l7 -> l2 | handler (kaydet/komut) | izinli |
| l7 -> l5 | sema / `IContextWindow` portu | izinli |
| l7 -> l6 | DI + `IShutdownHook` | izinli |
| l7 -> l4 | `VersaCoder.Infrastructure` | **X** - yasak (port + DI) |
| harici | DevExpress v26.1 (NuGet) | surum G8/G3 |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Batch render | `dotnet test --filter Category=Ui` | 1.000 delta -> repaint <= 20 |
| Iptal gecikmesi | birim test | `Cancel()` -> 100 ms icinde biter |
| Atomik kaydetme | birim test (port sahtesi) | kismi dosya yok |
| Kirli sekme kapanisi | birim test | onay ister; "sil"te dosya degismez |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N43 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P43-1 | Repaint sayaci testi (1.000 delta) | <= 20 - **DOĞRULAMA GEREKLİ** |
| P43-2 | Iptal testi (`Cancel()` 100 ms) | gecti - **DOĞRULAMA GEREKLİ** |
| P43-3 | `dotnet build VersaCoder.Desktop` | 0 hata |
| P43-4 | NetArchTest: `VersaCoder.Desktop` -> Domain/Infrastructure = 0 | 0 ihlal |
| P43-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
