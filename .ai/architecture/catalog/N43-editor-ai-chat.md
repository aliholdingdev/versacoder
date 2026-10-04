---
title: Katalog Dugumu N43 - Editor ve AI Sohbet
type: catalog-node
category: l7-ui
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: kod editoru, AI sohbet paneli ve fark gorunumu
---

# Katalog Dugumu N43 - Editor ve AI Sohbet

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l7 UI |
| Proje | VersaCoder.Desktop |
| Kapsam | kod editoru, AI sohbet paneli ve fark gorunumu |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l7-ui/N43-editor-ai-chat.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Kod editoru belge sekmeleriyle calisir, her sekme bagimsiz belge durumu tasir.
- AI sohbet paneli akis cevabini parca parca cerezer, tam metni tek seferde beklemez.
- Fark gorunumu iki surumu karsilastirir, oneri cubugu degisiklikleri uygulamadan once gosterir.
- Token olcer baglam penceresini ekranda bildirir, tasimayi sohbet katmanina birakir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Editor sekmelerini, sohbet panelini ve fark gorunumunu uretir | LLM cevabini uretir (l5 protokol + l4 is) |
| | l0 + l2 + l5 + l6 (l1 kabul) | - |
| | ekran, panel ve CLI etkilesimi (ne) | - |

## §2 Bagimlilik Yonu

| Yon | Durum | Aciklama |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l6 | EVET | asagidan tek yon |
| l1 | HAYIR | yon asagi dogru |
| l3 + l4 | HAYIR | yon asagi dogru |

## §3 Aileler

| # | Aile | Sorumluluk |
| --- | --- | --- |
| 1 | CodeEditor | Kod editoru kontrolu |
| 2 | DocumentTab | Belge sekmesi |
| 3 | AiChatPanel | AI sohbet paneli |
| 4 | StreamRenderer | Akis renderlayici |
| 5 | DiffView | Fark gorunumu |
| 6 | SuggestionBar | Oneri cubugu |
| 7 | PromptBox | Komut kutusu |
| 8 | TokenMeter | Token olcer |
| 9 | ChatBubble | Sohbet balonu |
| 10 | RenderScheduler | Render zamanlayicisi |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | CodeEditor | CodeEditor | belge 1 |
| 2 | CodeEditor | DocumentTab | N19-logging.md |
| 3 | DocumentTab | AiChatPanel | aktif |
| 4 | DocumentTab | StreamRenderer | parca 3 |
| 5 | AiChatPanel | DiffView | 2 fark |
| 6 | StreamRenderer | TokenMeter | 62% |

## §5 Yon Ozeti

| Katman | Izin | Yon |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l6 | EVET | asagidan tek yon |
| l1 | HAYIR | yon asagi dogru |
| l3 + l4 | HAYIR | yon asagi dogru |

## §6 Testler

| Test | Aile | Dogruluk |
| --- | --- | --- |
| SekmeDegistirmeTesti | CodeEditor | degisen |
| AkisRenderTesti | CodeEditor | cizilen |
| FarkGorunumTesti | DiffView | gosterilen |
| TokenOlcerTesti | DiffView | olculen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Render kilitleme | takilan ekran | zamanlayici |
| Akis kaybi | eksik metin | parca sirasi |
| Belge catismasi | veri kaybi | tek yazici |

## §8 Disbaglanti

| Dis Kaynak | Yon | Not |
| --- | --- | --- |
| ADR-028 | - | VersaCoder.Desktop baz projeleri |
| ADR-013 | - | 52 dugum |
| layer-matrix.md | referans | yon matrisi |
| N19-logging | - |

## §9 Kanit

| Kanit | Durum |
| --- | --- |
| Klasor | .ai/architecture/catalog/ (duz) |
| Dosya | N43-editor-ai-chat.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Desktop |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N43-001 | `ICodeEditor` | interface | CodeEditor sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-002 | `CodeEditor` | class | CodeEditor cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-003 | `CodeEditorDto` | record | CodeEditor veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-004 | `CodeEditorMapper` | class | CodeEditor <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-005 | `CodeEditorValidator` | class | CodeEditor dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-006 | `CodeEditorFactory` | class | CodeEditor uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-007 | `CodeEditorOptions` | record | CodeEditor yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-008 | `CodeEditorProvider` | class | CodeEditor saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-009 | `CodeEditorRegistry` | class | CodeEditor kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-010 | `CodeEditorConverter` | class | CodeEditor bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-011 | `CodeEditorTests` | class | CodeEditor testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-012 | `CodeEditorBuilder` | class | CodeEditor uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | CodeEditorTests | planli |
| N43-013 | `IDocumentTab` | interface | DocumentTab sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-014 | `DocumentTab` | class | DocumentTab cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-015 | `DocumentTabDto` | record | DocumentTab veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-016 | `DocumentTabMapper` | class | DocumentTab <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-017 | `DocumentTabValidator` | class | DocumentTab dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-018 | `DocumentTabFactory` | class | DocumentTab uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-019 | `DocumentTabOptions` | record | DocumentTab yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-020 | `DocumentTabProvider` | class | DocumentTab saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-021 | `DocumentTabRegistry` | class | DocumentTab kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-022 | `DocumentTabConverter` | class | DocumentTab bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-023 | `DocumentTabTests` | class | DocumentTab testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-024 | `DocumentTabBuilder` | class | DocumentTab uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | DocumentTabTests | planli |
| N43-025 | `IAiChatPanel` | interface | AiChatPanel sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-026 | `AiChatPanel` | class | AiChatPanel cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-027 | `AiChatPanelDto` | record | AiChatPanel veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-028 | `AiChatPanelMapper` | class | AiChatPanel <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-029 | `AiChatPanelValidator` | class | AiChatPanel dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-030 | `AiChatPanelFactory` | class | AiChatPanel uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-031 | `AiChatPanelOptions` | record | AiChatPanel yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-032 | `AiChatPanelProvider` | class | AiChatPanel saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-033 | `AiChatPanelRegistry` | class | AiChatPanel kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-034 | `AiChatPanelConverter` | class | AiChatPanel bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-035 | `AiChatPanelTests` | class | AiChatPanel testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-036 | `AiChatPanelBuilder` | class | AiChatPanel uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | AiChatPanelTests | planli |
| N43-037 | `IStreamRenderer` | interface | StreamRenderer sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-038 | `StreamRenderer` | class | StreamRenderer cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-039 | `StreamRendererDto` | record | StreamRenderer veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-040 | `StreamRendererMapper` | class | StreamRenderer <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-041 | `StreamRendererValidator` | class | StreamRenderer dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-042 | `StreamRendererFactory` | class | StreamRenderer uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-043 | `StreamRendererOptions` | record | StreamRenderer yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-044 | `StreamRendererProvider` | class | StreamRenderer saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-045 | `StreamRendererRegistry` | class | StreamRenderer kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-046 | `StreamRendererConverter` | class | StreamRenderer bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-047 | `StreamRendererTests` | class | StreamRenderer testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-048 | `StreamRendererBuilder` | class | StreamRenderer uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | StreamRendererTests | planli |
| N43-049 | `IDiffView` | interface | DiffView sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-050 | `DiffView` | class | DiffView cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-051 | `DiffViewDto` | record | DiffView veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-052 | `DiffViewMapper` | class | DiffView <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-053 | `DiffViewValidator` | class | DiffView dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-054 | `DiffViewFactory` | class | DiffView uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-055 | `DiffViewOptions` | record | DiffView yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-056 | `DiffViewProvider` | class | DiffView saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-057 | `DiffViewRegistry` | class | DiffView kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-058 | `DiffViewConverter` | class | DiffView bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-059 | `DiffViewTests` | class | DiffView testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-060 | `DiffViewBuilder` | class | DiffView uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | DiffViewTests | planli |
| N43-061 | `ISuggestionBar` | interface | SuggestionBar sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-062 | `SuggestionBar` | class | SuggestionBar cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-063 | `SuggestionBarDto` | record | SuggestionBar veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-064 | `SuggestionBarMapper` | class | SuggestionBar <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-065 | `SuggestionBarValidator` | class | SuggestionBar dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-066 | `SuggestionBarFactory` | class | SuggestionBar uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-067 | `SuggestionBarOptions` | record | SuggestionBar yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-068 | `SuggestionBarProvider` | class | SuggestionBar saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-069 | `SuggestionBarRegistry` | class | SuggestionBar kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-070 | `SuggestionBarConverter` | class | SuggestionBar bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-071 | `SuggestionBarTests` | class | SuggestionBar testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-072 | `SuggestionBarBuilder` | class | SuggestionBar uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | SuggestionBarTests | planli |
| N43-073 | `IPromptBox` | interface | PromptBox sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-074 | `PromptBox` | class | PromptBox cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-075 | `PromptBoxDto` | record | PromptBox veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-076 | `PromptBoxMapper` | class | PromptBox <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-077 | `PromptBoxValidator` | class | PromptBox dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-078 | `PromptBoxFactory` | class | PromptBox uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-079 | `PromptBoxOptions` | record | PromptBox yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-080 | `PromptBoxProvider` | class | PromptBox saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-081 | `PromptBoxRegistry` | class | PromptBox kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-082 | `PromptBoxConverter` | class | PromptBox bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-083 | `PromptBoxTests` | class | PromptBox testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-084 | `PromptBoxBuilder` | class | PromptBox uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | PromptBoxTests | planli |
| N43-085 | `ITokenMeter` | interface | TokenMeter sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-086 | `TokenMeter` | class | TokenMeter cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-087 | `TokenMeterDto` | record | TokenMeter veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-088 | `TokenMeterMapper` | class | TokenMeter <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-089 | `TokenMeterValidator` | class | TokenMeter dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-090 | `TokenMeterFactory` | class | TokenMeter uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-091 | `TokenMeterOptions` | record | TokenMeter yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-092 | `TokenMeterProvider` | class | TokenMeter saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-093 | `TokenMeterRegistry` | class | TokenMeter kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-094 | `TokenMeterConverter` | class | TokenMeter bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-095 | `TokenMeterTests` | class | TokenMeter testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-096 | `TokenMeterBuilder` | class | TokenMeter uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | TokenMeterTests | planli |
| N43-097 | `IChatBubble` | interface | ChatBubble sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-098 | `ChatBubble` | class | ChatBubble cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-099 | `ChatBubbleDto` | record | ChatBubble veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-100 | `ChatBubbleMapper` | class | ChatBubble <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-101 | `ChatBubbleValidator` | class | ChatBubble dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-102 | `ChatBubbleFactory` | class | ChatBubble uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-103 | `ChatBubbleOptions` | record | ChatBubble yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-104 | `ChatBubbleProvider` | class | ChatBubble saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-105 | `ChatBubbleRegistry` | class | ChatBubble kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-106 | `ChatBubbleConverter` | class | ChatBubble bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-107 | `ChatBubbleTests` | class | ChatBubble testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-108 | `ChatBubbleBuilder` | class | ChatBubble uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ChatBubbleTests | planli |
| N43-109 | `IRenderScheduler` | interface | RenderScheduler sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-110 | `RenderScheduler` | class | RenderScheduler cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-111 | `RenderSchedulerDto` | record | RenderScheduler veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-112 | `RenderSchedulerMapper` | class | RenderScheduler <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-113 | `RenderSchedulerValidator` | class | RenderScheduler dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-114 | `RenderSchedulerFactory` | class | RenderScheduler uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-115 | `RenderSchedulerOptions` | record | RenderScheduler yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-116 | `RenderSchedulerProvider` | class | RenderScheduler saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-117 | `RenderSchedulerRegistry` | class | RenderScheduler kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-118 | `RenderSchedulerConverter` | class | RenderScheduler bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
| N43-119 | `RenderSchedulerTests` | class | RenderScheduler testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N43-120 | `RenderSchedulerBuilder` | class | RenderScheduler uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | RenderSchedulerTests | planli |
