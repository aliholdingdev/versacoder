---
title: "N41 Ribbon Kabuk"
type: architecture
category: l7-ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N41 - 02-main-ide ekran kabugu: RibbonForm + Office2010, commandId eslemesi, StatusBar/DockHost (kayit tablosu DEGIL)
---

# N41 Ribbon Kabuk

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N42-panels-explorer.md](N42-panels-explorer.md)
- [N52-macro-l7.md](N52-macro-l7.md)
- [../../index.md](../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)
- [../../.decisions/adr-index.md](../../.decisions/adr-index.md)

> **Tek cumlelik sorumluluk:** N41, ana pencerenin `RibbonForm` + `RibbonControlStyle.Office2010` ile kuruldugunu ve her ribbon tusunun **tek bir komut kimligine** baglandigini buyur.

## §0 Ozet (Answer First)

- 02-main-ide ekraninin kabugudur: DevExpress **v26.1** `RibbonForm` uzerinde `RibbonControl` **yalniz** `RibbonControlStyle.Office2010` (kural kaynagi [adr-index](../../.decisions/adr-index.md)); baska stil = **CRITICAL**, review RED.
- Hicbir buton is mantigi cagirmaz; yalniz `commandId` ile `ICommandDispatcher`'a (l2) gider - `#region` yok, 200+ satir code-behind yok.
- Etkinlik durumu `EnabledWhen` ile gelir; bilinmeyen `commandId` -> `Result.Fail(UNKNOWN_COMMAND)`, istisna **degil** sonuc.
- Ilerleme kosulu: `RibbonStyle == Office2010` assertion + her tusun bilinen `commandId` icerdigi katalog taramasi (§9 P41-1, P41-2).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `IRibbonModule.Build(RibbonControl rc)` | sekmeler/gruplar ekleme |
| `interface` | `IRibbonButton.CommandId` / `.EnabledWhen` | tus <-> komut eslemesi |
| `interface` | `IStatusBarHost.Show(text, state)` | durum cubugu geri bildirimi |
| `interface` | `IDockHost.Register(control, region)` | panel demirleme (03..07) |
| `interface` | `IThemeManager.Apply(skinName)` + `IKeyBinding.Bind(Keys, id)` | skin tek nokta + kisayol |

```csharp
// MainForm.RibbonControl.RibbonStyle = RibbonControlStyle.Office2010;  // tek stil
private void OnRibbonClick(string commandId)
{
    _dispatcher.Dispatch(commandId);   // yalniz komut; SQL/IO/dosya YOK (l7 kurali)
}
// Etkinlik durumu EnabledWhen ile gelir; buton kendi is mantigini TANIMAZ.
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Ribbon modulleri (seksme/grup) | N41 `IRibbonModule` | kabuk kurulumu |
| `commandId` | tus/kisayol olayi | `ICommandDispatcher.Dispatch` (l2) |
| `EnabledWhen` durumu | l2 handler sonucu | `barItem.Enabled` |
| Skin adi | N44 ayarlar | `IThemeManager.Apply` |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `Result.Fail(UNKNOWN_COMMAND)` | StatusBar tek satir | bilinmeyen tus pasif + log |
| StatusBar mesaji | kullanici | dispatcher `Result.Fail` yutulmaz |
| Demirlenmis paneller | N42/N43/N44 | `IDockHost.Register` |
| Kapanis talebi | N38 `IShutdownHook` (l6) | bekleyen dispatch temizlenir |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N41 notu |
|---|---|---|
| l7 -> l1 | **X** | domain'e dogrudan referans yok |
| l7 -> l4 | **X** | dosya/HTTP/tool cagrisi yok |
| l7 -> l2, l5, l6 | OK | dispatcher (l2), sema (l5), DI (l6) |
| -> l7 | **X** | hicbir katman kabuga bakmaz |
| l6 -> l7 | **X** | composition root UI'i acmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N41 | Ribbon kabuk | Bu dosya - 02-main-ide |
| N42 | Cozum gezgini | DockManager sol bolge |
| N43 | Editor + AI Chat | sekmeler + akis cizimi |
| N44 | Ayarlar + CLI | tema/kisayol ayarlari |
| N52 | Makro l7 | `IShellKit` |

## §6 Projeler

| Proje | N41 katkisi |
|---|---|
| `VersaCoder.Desktop` | `MainForm : RibbonForm` + ribbon modulleri (designer uclusu: **ui-designer**) |
| `VersaCoder.Cli` | kabuk yok; ayni `commandId` -> verb eslemesi (N44) |
| `VersaCoder.Tests` | RibbonStyle assertion + komut esleme testleri (`Category=Ui`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `l7 -> l1/l4` = 0 |

TFM: `net48;net10.0-windows` - DevExpress Universal **v26.1** iki bacakta ayni; form tasarimi `#if` icinde degil; kanit GATE-004 build'i (aksi `NEEDS_VALIDATION`).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l7 -> l2 | `ICommandDispatcher` (handler) | izinli |
| l7 -> l6 | DI + `IShutdownHook` | izinli |
| l7 -> l1, l4 | `VersaCoder.Domain` / `VersaCoder.Infrastructure` | **X** - yasak |
| l7 -> l0, l3, l5 | port/tip/sema | izinli |
| harici | DevExpress Universal v26.1 (NuGet) | surum G8/G3 - paket kilitli |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| RibbonStyle assertion | `dotnet test --filter Category=Ui` | `== Office2010` |
| Komut esleme | katalog taramasi | her tus bilinen `commandId` |
| Bilinmeyen komut | birim test | `UNKNOWN_COMMAND`, istisna yok |
| Designer uyumu | dosya kontrolu | `.cs + .Designer.cs + .resx` + `ResourceManager` |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N41 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P41-1 | `dotnet build VersaCoder.Desktop` | 0 hata |
| P41-2 | RibbonStyle + komut esleme testleri | gecti - **DOĞRULAMA GEREKLİ** |
| P41-3 | NetArchTest: `VersaCoder.Desktop` -> Domain/Infrastructure = 0 | 0 ihlal |
| P41-4 | Stil taramasi: kodda `RibbonControlStyle.` yalniz `Office2010` | 0 baska stil |
| P41-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
