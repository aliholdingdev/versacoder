---
title: "N42 Cozum Gezgini"
type: architecture
category: l7-ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N42 - 03-solution-explorer: sanal TreeList, talep uzerine cocuk, IExpandState, PATH_ESCAPE (kayit tablosu DEGIL)
---

# N42 Cozum Gezgini

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N41-shell-ribbon.md](N41-shell-ribbon.md)
- [N43-editor-ai-chat.md](N43-editor-ai-chat.md)
- [N52-macro-l7.md](N52-macro-l7.md)
- [../../index.md](../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N42, proje agacinin `TreeList` uzerinde **sanal modda, durum kaybisiz ve dosya I/O'su l4 portlari uzerinden** gosterildigini buyur.

## §0 Ozet (Answer First)

- 03-solution-explorer ekranidir: `TreeList` **sanal modda** doldurulur (10.000+ dugumde donmaz), cocuklar yalnizca genisletilince okunur (`IFileTreeController.FetchChildren`).
- Dosya erisimi **l4 portlari** uzerindedir (`IFileSystemStore` / `IGlobScanner`); gercek uygulama l4'te, baglama l6 DI'da - **l7 -> l4 referansi 0** (NetArchTest).
- `IExpandState` klasor acilma durumunu yeniden yuklemelerde korur; sag tik menusu yalniz **bilinen** eylemleri gosterir, bilinmeyen id `UNKNOWN_COMMAND` (N41).
- Ilerleme kosulu: 10.000 dugum < 1 sn + kok disi hedefe surukleme `PATH_ESCAPE` (§9 P42-1, P42-4).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `ISolutionExplorer.Load(root, CancellationToken ct)` | kokten agac yukleme |
| `interface` | `ITreeListNode.Id` / `.Text` / `.HasChildren` | sanal dugum |
| `interface` | `IFileTreeController.FetchChildren(nodeId, ct)` | talep uzere cocuk |
| `interface` | `IExpandState.IsExpanded(nodeId)` / `.Toggle(nodeId)` | acilma hafizasi |
| `interface` | `IFilterModel.Apply(text)` + `IContextMenuBuilder.Build(node)` + `IDragDropHandler.CanDrop(src, dst)` | filtre + menu + surukleme |

```csharp
public interface IFileTreeController
{
    Task<IReadOnlyList<ITreeNode>> FetchChildren(string nodeId, CancellationToken ct);
}
// TreeList sanal modda calisir: cocuklar genisletilince OKUNUR (port uzerinden).
// UI asla dosyaya DIREKT gitmez -> l7 -> l4 referansi 0 (G2 ihlali yok).
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Kok dizin | N41 `IDockHost` acilisi | `ISolutionExplorer.Load` |
| Cocuk dugumler | l4 `IFileSystemStore` / `IGlobScanner` (DI, l6) | genisletme aninda |
| Filtre metni | kullanici (`SearchControl`) | `IFilterModel.Apply` |
| Dizin degisikligi olayi | watcher (l4 portu) | agac tazelenir, `IExpandState` korunur |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Sanal dugum listesi | `TreeList` | bellekte tamami degil, sayfalama |
| `commandId` (cift tik) | N41 dispatcher -> N43 editor | metin, veri degil |
| `Result.Fail(IO_ERROR / PATH_ESCAPE / UNKNOWN_COMMAND)` | StatusBar / rozet | istisna degil sonuc |
| `IExpandState` guncellemesi | UI durumu | yeniden yuklemede kaybolmaz |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N42 notu |
|---|---|---|
| l7 -> l4 | **X** | dosyaya dogrudan erisim yok; yalniz **port** (l0/l2) + l6 DI |
| l7 -> l1 | **X** | domain'e referans yok |
| l7 -> l2, l6 | OK | handler + DI |
| l6 -> l7 | **X** | composition root UI'i acmaz |
| -> l7 | **X** | hicbir katman gezgine bakmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N42 | Cozum gezgini | Bu dosya - 03-solution-explorer |
| N41 | Ribbon kabuk | DockManager sol bolge sahibi |
| N43 | Editor + AI Chat | Cift tik hedefi (editor) |
| N29 | Dosya araclari (l4) | Portun gercek uygulamasi |
| N52 | Makro l7 | `IPanelKit` |

## §6 Projeler

| Proje | N42 katkisi |
|---|---|
| `VersaCoder.Desktop` | `SolutionExplorerControl` (designer uclusu: **ui-designer**) |
| `VersaCoder.Tests` | Sanal mod + filtre + PATH_ESCAPE testleri (`Category=Ui`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `l7 -> l4` = 0 (yalniz port) |

TFM: `net48;net10.0-windows` - `TreeList` v26.1 sanal mod iki bacakta ayni; async yukleme `async/await(ct)`; `#if` yalniz dosya tarama API farki varsa, agac modeli ortak (G9).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l7 -> l0 | `IFileSystemStore` / `IGlobScanner` portlari | izinli |
| l7 -> l2, l6 | handler + DI | izinli |
| l7 -> l4 | `VersaCoder.Infrastructure` | **X** - yasak (port uzerinden) |
| l7 -> l1 | `VersaCoder.Domain` | **X** - yasak |
| harici | DevExpress v26.1 `TreeList` (NuGet) | surum G8/G3 |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Sanal mod performansi | `dotnet test` | 10.000 dugum < 1 sn |
| ExpandState hafizasi | birim test | ac/kapa + yeniden yukleme |
| Filtre | birim test | yalniz eslesen dugumler kalir |
| PATH_ESCAPE | birim test | kok disi surukleme reddi, **RED** |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N42 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P42-1 | Sanal mod testi (10.000 dugum) | < 1 sn - **DOĞRULAMA GEREKLİ** |
| P42-2 | `dotnet build VersaCoder.Desktop` | 0 hata |
| P42-3 | NetArchTest: `VersaCoder.Desktop` -> Infrastructure = 0 | 0 ihlal |
| P42-4 | PATH_ESCAPE testi (kok disi) | reddedildi |
| P42-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
