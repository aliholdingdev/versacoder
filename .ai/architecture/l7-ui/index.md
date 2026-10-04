---
title: "l7 UI — Katman Şartnamesi (VersaCoder Desktop + Cli)"
type: architecture
category: l7-ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l7 UI katmani: 7 ekran (WinForms + DevExpress v26.1, Office2010 ribbon) + CLI sunumu, U5 Designer sahipligi, N41-N44 + N52, VersaCoder.Desktop + VersaCoder.Cli, bagimlilik listesi ve test stratejisi"
---

# l7 UI — Katman Şartnamesi

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [system-overview](../00-overview/system-overview.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test stratejisi](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *Kullanicinin ne gordugu, hangi tusa bastiginin ve bunun uygulamaya nasil tasindiginin* tek cevabidir; son soz `ui-designer`'dadir, kural/izolasyon konusunda `reviewer` RED yetkilidir (AGENTS.md §8).

## §0 Ozet (Answer First)

l7 **yalnizca gosterim ve etkileşimidir**: 7 ekran (`01-start-page` .. `07-settings`) + CLI ayni l2 handler'larini cagirir; l7 **l0, l2, l3, l5, l6'ya bakabilir**, **l1 ve l4'e asla referans vermez** ve **hicbir katman l7'ye bakamaz**. **GATE-003 HARD STOP:** 7 ASCII wireframe kullanici onayi **kodlamadan once** zorunlu - onaysiz kod yok. **U5 sahipligi:** `Form.Designer.cs` / `.resx` yalnizca `ui-designer` tarafindan duzenlenir (developer eliyle elle yazim = onay zinciri kopar, gate acilamaz). Ribbon yalniz `RibbonControlStyle.Office2010`, DevExpress Universal **v26.1**. Klasordeki 6 dosya: bu sartname (`index.md`), N41..N44 alt dugumleri ve N52 makro dugumu.

## §1 Sorumluluk

**Tablo A — Bu katman NE YAPAR**

| # | Sorumluluk | Kanit | Cikti |
|---|---|---|---|
| U1 | 7 ekran wireframe'e sadik render | ekran adi `01..07` + Form dosyalari | ekranlar |
| U2 | Etkilesim -> komut: buton/ribbon olaylari l2 handler'larina gider | `ICommandDispatcher.Dispatch(commandId)` (SQL/IO yok) | kullanici niyeti |
| U3 | Durum gosterimi: loading/hata/bos liste sablonlari | `ViewState` enum + kontrol sablonu | geri bildirim |
| U4 | CLI sunumu: `VersaCoder.Cli` ayni l2 handler'larini cagirir | `IArgParser` + `IExitCodeMap` | terminal ciktisi |
| U5 | **Designer uyumu**: `Form.cs + Form.Designer.cs + Form.resx` uclusu | her formda 3 dosya + `ResourceManager` | **sahip: ui-designer** |

**Tablo B — Bu katman NE YAPMAZ**

| # | Yapmaz | Nerede | Neden |
|---|---|---|---|
| B1 | SQL / Dapper / baglanti | l4 | G2 ihlali |
| B2 | Entity / domain invariant | l1 | UI != domain |
| B3 | LLM/HTTP cagrisi, tool calistirma | l4 | ag altyapisi + guvenlik |
| B4 | DI kaydi / config okuma | l6 | composition root tekeli |
| B5 | Sema dogrulama | l5 | sema != gosterim |

## §2 GIRENLER

| # | Giren | Kaynak | Hangi kapidan gecer |
|---|---|---|---|
| 1 | Komut (`commandId`) | ribbon/tus (N41) | `ICommandDispatcher` (l2) |
| 2 | Arguman (`argv`) | terminal (N44) | `IArgParser.Parse` -> ayni l2 handler |
| 3 | Stream delta (`IStreamDelta`) | l4 N31 akisi | `IStreamRenderer` toplu cizim |
| 4 | Ayar modeli (`ISettingsModel`) | l6 config + l2 | GUI = CLI ayni model (SSOT) |
| 5 | Durum (`ViewState`) | l2 sonucu (`Result`) | sablon gosterimi |

## §3 CIKANLAR

| # | Cikti | Alici | Not |
|---|---|---|---|
| 1 | `commandId` / `prompt` / `argv` (veri, is degil) | l2 handler | hata = `Result`, istisna yok |
| 2 | `IStreamDelta` -> balon cizimi | kullanici | 50 ms toplu repaint |
| 3 | Dosya kaydi istegi | l4 N29 (port uzerinden) | atomik yazma |
| 4 | CLI ciktisi + exit kodu (`0/2/3/64/70/74`) | betik/CI | `--json` ikili cikti |

## §4 Yasak Yonler (ADR-028 matrisi)

| Yon hucresi | Deger | Anlam |
|---|---|---|
| `l7 -> l1` | **X** | domain'e dogrudan referans yok |
| `l7 -> l4` | **X** | dosya/HTTP/tool erisimi yalniz port + l2 uzerinden |
| `l7 -> l0`, `l2`, `l3`, `l5`, `l6` | OK | izinli referanslar |
| `hicbir katman -> l7` | **X** | UI'a bakis yok (geri donus port + Result ile) |
| `l6 -> l7` | **X** | baglamayi UI degil, composition yapar |

```text
   +---------------------+
   | l7 UI (N52)         |  7 ekran + CLI, yalnizca GOSTERIM
   | N41 kabuk N42 gezgin|
   | N43 editor  N44 ayar|
   +----------+----------+
              |  commandId / prompt / argv  (veri, is degil)
              v
   l2 Application (handler)  .  l5 Protocol (sema)  .  l6 Host (DI)
              |
              |  yasak: l7 -> l1 (X)  .  l7 -> l4 (X)  .  -> l7 (hepsi X)
```

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N41 | shell-ribbon | N43 | editor-ai-chat |
| N42 | panels-explorer | N44 | settings-cli |
| N52 | makro l7 (5 kit) | 7 ekran | 01..07 |

Seviye kurali: N kodlari klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Desktop` | `net48;net10.0-windows` | WinExe, DevExpress Universal **v26.1** |
| `VersaCoder.Cli` | `net48;net10.0-windows` | Exe, ayni l2 handler'lari |
| `VersaCoder.Tests` | cift bacak | komut esleme + CLI arguman testleri (`Category=Ui`) |
| `VersaCoder.ArchitectureTests` | cift bacak | NetArchTest: `l7 -> l1/l4` = 0 |

`net48` C# 7.3 alt kumesi, `net10.0-windows` C# 14; form tasarimi asla `#if` icinde degildir (G9); iki bacak kaniti GATE-004 build'i (aksi `NEEDS_VALIDATION`).

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tur |
|---|---|---|
| l7 | `VersaCoder.Domain` (l1) | matris **X** -> yasak |
| l7 | `VersaCoder.Infrastructure` (l4) | matris **X** -> yasak (yalniz port + l2) |
| l7 | l0, l2, l3, l5, l6 | izinli (ProjectReference) |
| l7 | DevExpress Universal v26.1 (NuGet) | surum G8/G3; ribbon yalniz `Office2010` |
| hicbir katman | `VersaCoder.Desktop` / `VersaCoder.Cli` | **X** -> NetArchTest = 0 |

## §8 Test Stratejisi

| Test | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | komut esleme, CLI arguman vaka tablosu, exit kodlari | **%90** satir |
| Designer | 7 form `Form.cs + .Designer.cs + .resx` + `ResourceManager` tutarliligi | 7/7 |
| Arch (NetArchTest) | `l7 -> l1/l4` = 0; `-> l7` = 0 | **0 ihlal** |
| Gate | GATE-003 wireframe onayi (**HARD STOP**) + GATE-004 iki bacak build | onay + yesil |

Kosum: `dotnet test --filter Category=Ui` · 13 soru icin [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §9 Kanit

| # | Iddia | Kanit komutu | Beklenen |
|---|---|---|---|
| 1 | Dosya boyutu 2.600-8.500 bayt | `Get-Item` ile `.Length` | aralik icinde |
| 2 | Bozuk link yok | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `0` |
| 3 | Frontmatter 9 alan, ilk satir `---` | gozden gecirme | 9/9 |
| 4 | N41..N44 + N52 + index = 6 dosya | klasor listesi | 6 |
| 5 | Emoji / Unicode kutu / mojibake yok | `measure-vault.ps1` | `0` |

`src/` henüz diskte olmadigindan kod bazli ihlal sayilari `DOĞRULAMA GEREKLİ` kalir; ilk build sonrasi NetArchTest ciktisi bu tabloya islenir.
