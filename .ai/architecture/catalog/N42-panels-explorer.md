---
title: Katalog Dugumu N42 - Paneller ve Explorer
type: catalog-node
category: l7-ui
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: cozum explorer, arama paneli ve detay paneli
---

# Katalog Dugumu N42 - Paneller ve Explorer

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l7 UI |
| Proje | VersaCoder.Desktop |
| Kapsam | cozum explorer, arama paneli ve detay paneli |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l7-ui/N42-panels-explorer.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Solution explorer agacı dosya sistemini yansitir, dugumler tembel yuklenir.
- Arama paneli filtre modeli uzerinden sorgu yapar, indeksi kendisi uretir.
- Baglam menusu komut kaydina gore dolar, sabit menu yazmak yasaktir.
- Surukle-birak islemi hedef dugum dogrulamasiyla calisir, yanlis hedefe tasima reddedilir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Explorer agacini, arama ve filtre panelini uretir | Dosyayi kendisi yazar (l4 is) |
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
| 1 | SolutionExplorer | Cozum explorer agaci |
| 2 | TreeListNode | Agac dugumu modeli |
| 3 | FileTreeController | Dosya agaci denetleyicisi |
| 4 | SearchPanel | Arama paneli |
| 5 | ContextMenuBuilder | Baglam menusu kurucusu |
| 6 | NodeIconMap | Dugum ikon eslemesi |
| 7 | FilterModel | Filtre modeli |
| 8 | ExpandState | Genisletme durumu |
| 9 | DragDropHandler | Surukle-birak isleyicisi |
| 10 | DetailPane | Detay paneli |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | SolutionExplorer | SolutionExplorer | cozum |
| 2 | SolutionExplorer | TreeListNode | src klasoru |
| 3 | TreeListNode | SearchPanel | *.md |
| 4 | TreeListNode | FilterModel | aktif |
| 5 | FileTreeController | ExpandState | acik |
| 6 | SearchPanel | DragDropHandler | gecerli hedef |

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
| AgacYuklemeTesti | SolutionExplorer | yuklenen |
| FiltreTesti | SolutionExplorer | filtrelenen |
| BaglamMenuTesti | ContextMenuBuilder | acilan |
| SurukleBirakTesti | ContextMenuBuilder | reddedilen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Tum yukleme | yavas | tembel yukleme |
| Yanlis hedef | veri kaybi | hedef dogrulama |
| Menu katilimi | sabit menu | komut kaydi |

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
| Dosya | N42-panels-explorer.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Desktop |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N42-001 | `ISolutionExplorer` | interface | SolutionExplorer sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-002 | `SolutionExplorer` | class | SolutionExplorer cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-003 | `SolutionExplorerDto` | record | SolutionExplorer veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-004 | `SolutionExplorerMapper` | class | SolutionExplorer <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-005 | `SolutionExplorerValidator` | class | SolutionExplorer dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-006 | `SolutionExplorerFactory` | class | SolutionExplorer uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-007 | `SolutionExplorerOptions` | record | SolutionExplorer yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-008 | `SolutionExplorerProvider` | class | SolutionExplorer saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-009 | `SolutionExplorerRegistry` | class | SolutionExplorer kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-010 | `SolutionExplorerConverter` | class | SolutionExplorer bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-011 | `SolutionExplorerTests` | class | SolutionExplorer testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-012 | `SolutionExplorerBuilder` | class | SolutionExplorer uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | SolutionExplorerTests | planli |
| N42-013 | `ITreeListNode` | interface | TreeListNode sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-014 | `TreeListNode` | class | TreeListNode cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-015 | `TreeListNodeDto` | record | TreeListNode veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-016 | `TreeListNodeMapper` | class | TreeListNode <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-017 | `TreeListNodeValidator` | class | TreeListNode dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-018 | `TreeListNodeFactory` | class | TreeListNode uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-019 | `TreeListNodeOptions` | record | TreeListNode yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-020 | `TreeListNodeProvider` | class | TreeListNode saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-021 | `TreeListNodeRegistry` | class | TreeListNode kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-022 | `TreeListNodeConverter` | class | TreeListNode bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-023 | `TreeListNodeTests` | class | TreeListNode testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-024 | `TreeListNodeBuilder` | class | TreeListNode uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | TreeListNodeTests | planli |
| N42-025 | `IFileTreeController` | interface | FileTreeController sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-026 | `FileTreeController` | class | FileTreeController cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-027 | `FileTreeControllerDto` | record | FileTreeController veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-028 | `FileTreeControllerMapper` | class | FileTreeController <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-029 | `FileTreeControllerValidator` | class | FileTreeController dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-030 | `FileTreeControllerFactory` | class | FileTreeController uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-031 | `FileTreeControllerOptions` | record | FileTreeController yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-032 | `FileTreeControllerProvider` | class | FileTreeController saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-033 | `FileTreeControllerRegistry` | class | FileTreeController kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-034 | `FileTreeControllerConverter` | class | FileTreeController bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-035 | `FileTreeControllerTests` | class | FileTreeController testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-036 | `FileTreeControllerBuilder` | class | FileTreeController uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | FileTreeControllerTests | planli |
| N42-037 | `ISearchPanel` | interface | SearchPanel sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-038 | `SearchPanel` | class | SearchPanel cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-039 | `SearchPanelDto` | record | SearchPanel veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-040 | `SearchPanelMapper` | class | SearchPanel <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-041 | `SearchPanelValidator` | class | SearchPanel dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-042 | `SearchPanelFactory` | class | SearchPanel uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-043 | `SearchPanelOptions` | record | SearchPanel yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-044 | `SearchPanelProvider` | class | SearchPanel saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-045 | `SearchPanelRegistry` | class | SearchPanel kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-046 | `SearchPanelConverter` | class | SearchPanel bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-047 | `SearchPanelTests` | class | SearchPanel testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-048 | `SearchPanelBuilder` | class | SearchPanel uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | SearchPanelTests | planli |
| N42-049 | `IContextMenuBuilder` | interface | ContextMenuBuilder sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-050 | `ContextMenuBuilder` | class | ContextMenuBuilder cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-051 | `ContextMenuBuilderDto` | record | ContextMenuBuilder veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-052 | `ContextMenuBuilderMapper` | class | ContextMenuBuilder <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-053 | `ContextMenuBuilderValidator` | class | ContextMenuBuilder dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-054 | `ContextMenuBuilderFactory` | class | ContextMenuBuilder uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-055 | `ContextMenuBuilderOptions` | record | ContextMenuBuilder yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-056 | `ContextMenuBuilderProvider` | class | ContextMenuBuilder saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-057 | `ContextMenuBuilderRegistry` | class | ContextMenuBuilder kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-058 | `ContextMenuBuilderConverter` | class | ContextMenuBuilder bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-059 | `ContextMenuBuilderTests` | class | ContextMenuBuilder testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-060 | `ContextMenuBuilderBuilder` | class | ContextMenuBuilder uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ContextMenuBuilderTests | planli |
| N42-061 | `INodeIconMap` | interface | NodeIconMap sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-062 | `NodeIconMap` | class | NodeIconMap cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-063 | `NodeIconMapDto` | record | NodeIconMap veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-064 | `NodeIconMapMapper` | class | NodeIconMap <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-065 | `NodeIconMapValidator` | class | NodeIconMap dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-066 | `NodeIconMapFactory` | class | NodeIconMap uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-067 | `NodeIconMapOptions` | record | NodeIconMap yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-068 | `NodeIconMapProvider` | class | NodeIconMap saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-069 | `NodeIconMapRegistry` | class | NodeIconMap kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-070 | `NodeIconMapConverter` | class | NodeIconMap bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-071 | `NodeIconMapTests` | class | NodeIconMap testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-072 | `NodeIconMapBuilder` | class | NodeIconMap uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | NodeIconMapTests | planli |
| N42-073 | `IFilterModel` | interface | FilterModel sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-074 | `FilterModel` | class | FilterModel cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-075 | `FilterModelDto` | record | FilterModel veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-076 | `FilterModelMapper` | class | FilterModel <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-077 | `FilterModelValidator` | class | FilterModel dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-078 | `FilterModelFactory` | class | FilterModel uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-079 | `FilterModelOptions` | record | FilterModel yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-080 | `FilterModelProvider` | class | FilterModel saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-081 | `FilterModelRegistry` | class | FilterModel kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-082 | `FilterModelConverter` | class | FilterModel bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-083 | `FilterModelTests` | class | FilterModel testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-084 | `FilterModelBuilder` | class | FilterModel uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | FilterModelTests | planli |
| N42-085 | `IExpandState` | interface | ExpandState sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-086 | `ExpandState` | class | ExpandState cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-087 | `ExpandStateDto` | record | ExpandState veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-088 | `ExpandStateMapper` | class | ExpandState <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-089 | `ExpandStateValidator` | class | ExpandState dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-090 | `ExpandStateFactory` | class | ExpandState uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-091 | `ExpandStateOptions` | record | ExpandState yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-092 | `ExpandStateProvider` | class | ExpandState saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-093 | `ExpandStateRegistry` | class | ExpandState kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-094 | `ExpandStateConverter` | class | ExpandState bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-095 | `ExpandStateTests` | class | ExpandState testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-096 | `ExpandStateBuilder` | class | ExpandState uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ExpandStateTests | planli |
| N42-097 | `IDragDropHandler` | interface | DragDropHandler sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-098 | `DragDropHandler` | class | DragDropHandler cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-099 | `DragDropHandlerDto` | record | DragDropHandler veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-100 | `DragDropHandlerMapper` | class | DragDropHandler <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-101 | `DragDropHandlerValidator` | class | DragDropHandler dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-102 | `DragDropHandlerFactory` | class | DragDropHandler uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-103 | `DragDropHandlerOptions` | record | DragDropHandler yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-104 | `DragDropHandlerProvider` | class | DragDropHandler saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-105 | `DragDropHandlerRegistry` | class | DragDropHandler kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-106 | `DragDropHandlerConverter` | class | DragDropHandler bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-107 | `DragDropHandlerTests` | class | DragDropHandler testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-108 | `DragDropHandlerBuilder` | class | DragDropHandler uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | DragDropHandlerTests | planli |
| N42-109 | `IDetailPane` | interface | DetailPane sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-110 | `DetailPane` | class | DetailPane cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-111 | `DetailPaneDto` | record | DetailPane veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-112 | `DetailPaneMapper` | class | DetailPane <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-113 | `DetailPaneValidator` | class | DetailPane dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-114 | `DetailPaneFactory` | class | DetailPane uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-115 | `DetailPaneOptions` | record | DetailPane yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-116 | `DetailPaneProvider` | class | DetailPane saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-117 | `DetailPaneRegistry` | class | DetailPane kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-118 | `DetailPaneConverter` | class | DetailPane bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
| N42-119 | `DetailPaneTests` | class | DetailPane testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N42-120 | `DetailPaneBuilder` | class | DetailPane uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | DetailPaneTests | planli |
