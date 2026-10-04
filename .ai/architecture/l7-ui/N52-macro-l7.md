---
title: "N52 MAKRO - l7 UI + CLI"
type: architecture
category: l7-ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N52 makro - l7 katmaninin tamami: 7 ekran + CLI, kit arayuzleri, disariya acilan tek yuzey, yasak yonler (kayit tablosu DEGIL)
---

# N52 MAKRO - l7 UI + CLI

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N41-shell-ribbon.md](N41-shell-ribbon.md)
- [N44-settings-cli.md](N44-settings-cli.md)
- [../../00-overview/dependency-map.md](../00-overview/dependency-map.md)
- [../../00-overview/system-overview.md](../00-overview/system-overview.md)
- [../../catalog/index.md](../catalog/index.md)
- [../../index.md](../index.md)
- [../../.decisions/adr-index.md](../../.decisions/adr-index.md)

> **Tek cumlelik sorumluluk:** N52, l7'in **tamaminin** neyi gosterip neye izin verdigini, **kime bakabildigini** ve disariya hangi imzalari actiginin tek cevabidir.

## §0 Ozet (Answer First)

- l7 **yalnizca gosterim ve etkileşimdir**: 7 ekran (N41 kabuk, N42 gezgin, N43 editor+chat, N44 ayarlar/CLI) ve CLI ayni l2 handler'larini cagirir; kod/SQL/dosya/HTTP/DI kaydi bu katmanda **YOK**.
- l7 **l0, l2, l3, l5, l6'ya bakabilir**; **l1 ve l4'e asla referans vermez**; **hicbir katman l7'ye bakmaz** (NetArchTest = 0).
- Disariya tek yuzey 5 kit: `IDesktopCore`, `IShellKit`, `IPanelKit`, `IEditorKit`, `ISettingsKit` - hepsi **komut gonderir**, sonuc her zaman `Result<T>`.
- Stil/surum sabiti: DevExpress Universal **v26.1** + `RibbonControlStyle.Office2010` (**tek stil**); sapma = CRITICAL (ADR-004/G3).
- Onay kapisi: wireframe'den sapma = **GATE-003** tekrar; onaysiz kod yok.

## §1 Sorumluluk (5 kit)

| Kit (makro arayuz) | Alt dugum | Disariya acilan tek cumlelik imza |
|---|---|---|
| `IDesktopCore` | N41 + N42 + N43 + N44 | `MainForm : RibbonForm` (`Office2010`) |
| `IShellKit` | N41 | `IRibbonButton.CommandId` -> `ICommandDispatcher.Dispatch(id)` |
| `IPanelKit` | N42 | `ISolutionExplorer.Load(root, CancellationToken)` |
| `IEditorKit` | N43 | `IAiChatPanel.SendAsync(prompt, ct)` / `ICodeEditor.SaveAsync(ct)` |
| `ISettingsKit` | N44 | `IArgParser.Parse(string[] argv)` -> `IExitCodeMap` |

```csharp
// l7'un disariya tek yuzeyi: komut GONDERMEK. Sonuc her zaman Result<T>.
// SQL / dosya / HTTP / tool calistirma / DI kaydi bu katmanda YOK (G2/G5 asla).
public interface IDesktopCore { /* N41..N44 form ve panelleri burada toplanir */ }
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `commandId` | ribbon tusu / kisayol | `ICommandDispatcher.Dispatch` (l2) |
| prompt | 05-ai-chat girisi | `IAiChatPanel.SendAsync` (l5 semasi) |
| `argv` | konsol satiri | `IArgParser.Parse` -> l2 handler |
| Ayar modeli | 07-settings formu | `ISettingsService` (l2) |
| Durum olaylari | l6 orkestrasyon | StatusBar / Agent paneli yansitimi |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `Result.Fail(UNKNOWN_COMMAND)` | StatusBar / `exit 2` | istisna **degil** sonuc |
| Akis chunk'lari | 05-ai-chat render | UI thread marshal; UI donmaz |
| Kaydedilmis oturum | l6 `IProjectSession` | formda ham I/O yok |
| Cikis kodu | kabuk | `IExitCodeMap` matrisi (N44) |
| Tasarim sozlesmesi | GATE-003 paketi | [ui-design/README.md](../../ui-design/README.md) |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N52 notu |
|---|---|---|
| l7 -> l1 | **X** | domain'e dogrudan referans yok |
| l7 -> l4 | **X** | dosya/HTTP/tool cagrisi yok |
| l7 -> l0, l2, l3, l5, l6 | OK | port / handler / tip / sema / DI |
| -> l7 | **X** | hicbir katman kabuga bakmaz |
| l6 -> l7 | **X** | composition root UI'i acmaz |
| Code-behind ic mantik | **X** | ic l2'ye tasiniyor (review RED) |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N41 | Ribbon kabuk | 02-main-ide kabugu |
| N42 | Cozum gezgini | 03-solution-explorer |
| N43 | Editor + AI Chat | 04-code-editor + 05-ai-chat |
| N44 | Ayarlar + CLI | 07-settings + VersaCoder.Cli |
| N52 | Makro l7 | Bu dosya - 5 kit toplamasi |

## §6 Projeler

| Proje | N52 katkisi |
|---|---|
| `VersaCoder.Desktop` | 7 form (designer uclusu: **ui-designer**) + kit kayitlari |
| `VersaCoder.Cli` | argv -> handler -> exit code (N44) |
| `VersaCoder.Tests` | kit eslesme + arguman testleri (`Category=Ui` / `Category=Cli`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `VersaCoder.Desktop/Cli -> Domain/Infrastructure` = 0 |

TFM: `net48;net10.0-windows` - DevExpress v26.1 iki bacakta ayni surum; `WinExe` (Desktop) + `Exe` (Cli); C# 7.3 alt kumesi (net48) / C# 14 (net10); `.Designer.cs` elle duzenlenmez; `#if` yalniz konsol kodlama/platform API farkinda (G9) - v26.1 iki bacak kaniti **GATE-004**, aksi `NEEDS_VALIDATION`.

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l7 -> l2 | `ICommandDispatcher` / handler | izinli |
| l7 -> l6 | DI + oturum/yasam dongusu | izinli |
| l7 -> l5 | tool/agent semasi | izinli |
| l7 -> l0, l3 | port / crosscutting tip | izinli |
| l7 -> l1, l4 | Domain / Infrastructure | **X** - yasak (NetArchTest) |
| harici | DevExpress Universal v26.1 | surum G8/G3 - paket kilitli |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Designer uculusu | dosya kontrolu | `.cs + .Designer.cs + .resx` + `ResourceManager` |
| RibbonStyle assertion | `dotnet test --filter Category=Ui` | `== Office2010` (tek stil) |
| Komut eslemesi | katalog taramasi | her tus bilinen `commandId` |
| NetArchTest 8x8 | `VersaCoder.ArchitectureTests` | `l7 -> l1/l4` = 0, geri ok = 0 |
| CLI arguman + exit | `dotnet test --filter Category=Cli` | vaka tablosu + matris |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N52 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P52-1 | `dotnet build VersaCoder.Desktop VersaCoder.Cli` | 0 hata |
| P52-2 | NetArchTest: `Desktop/Cli -> Domain/Infrastructure` = 0 | 0 ihlal |
| P52-3 | RibbonStyle + komut esleme testleri | gecti - **DOĞRULAMA GEREKLİ** |
| P52-4 | 7 ekran dosya uculusu kontrolu | 7 x 3 dosya |
| P52-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir. Katalog kaydi (`catalog/N52-macro-l7.md`) henuz yazilmadi - [catalog/index.md](../catalog/index.md) uzerinden izlenir.
