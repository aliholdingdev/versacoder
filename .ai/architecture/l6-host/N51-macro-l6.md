---
title: "N51 MAKRO - l6 Host"
type: architecture
category: l6-host
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N51 - l6 tamamini kapsar: 5 kit, katman yonu, katman geneli hata ve test (kayit tablosu DEGIL)
---

# N51 MAKRO - l6 Host

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N38-bootstrap-pipeline.md](N38-bootstrap-pipeline.md)
- [N39-di-registration.md](N39-di-registration.md)
- [N40-config-bridges.md](N40-config-bridges.md)
- [../../index.md](../../index.md)
- [../00-overview/system-overview.md](../00-overview/system-overview.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N51, l6'in tamamini hangi isi tek elde topladigini, kime bakabildigini ve disariya hangi imzalari actigini tek yerde toplar.

## §0 Ozet (Answer First)

- l6 **composition root'un tek sahibidir**: baglama (N39), ayarlar (N40) ve acilis sirasi (N38) burada toplanir; disariya acilan yuzey **kurulum + acilis + kapanis** ile sinirlidir.
- l6 **l0..l5'in tamamina bakabilir**; **yalniz l7** ona bakar; **l6 -> l7 X** ve **l0..l5 -> l6 X** (NetArchTest: yone referans = 0).
- SQL, dosya, HTTP, mesaj semasi, Paint bu katmanda **YOK** - yalnizca baglar; `#if` sahibi **yalniz N40** (`ITfmBridge`), N38/N39'da `#if` yok (G9).
- Ilerleme kosulu: `Probe = 0 eksik port` + disindaki hicbir projede `ServiceCollection` yok (§9 P51-1, P51-5).

## §1 Sorumluluk (5 kit)

| Kit (makro arayuzu) | Alt dugum | Disariya acilan tek cumlelik imza |
|---|---|---|
| `IHostCore` | N38 + N39 | `IServiceCollection.AddVersaCoder(IConfiguration)` |
| `IBootstrapKit` | N38 | `IStartupPipeline.RunAsync(CancellationToken ct)` |
| `IRegistrationKit` | N39 | `IContainerHealth.Probe()` + `ILifetimeMap.Verify()` |
| `IConfigKit` | N40 | `IConfigBootstrapper.Load()` -> `IBridgeOptions` |
| `ILifetimeKit` | N38 + N39 | `IShutdownHook.OnStopAsync(ct)` + `IDisposalOrder` |

```csharp
// l6'un disariya tek yuzeyi: kurulum + acilis + kapanis.
// SQL, dosya, HTTP, mesaj semasi, Paint burada YOK (yalnizca baglar).
public static IServiceCollection AddVersaCoder(this IServiceCollection s, IConfiguration c);
public static Task<StartupReport> RunAsync(this IStartupPipeline p, CancellationToken ct);
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `Program.Main` (l7 Desktop/Cli) | uygulama girisi | `AddVersaCoder` + `RunAsync` |
| Config dosya + ortam | N40 | 1. acilis adimi |
| Kit registrar'lar (N49..N52) | `IModuleRegistrar` | N39 icinde sirayla |
| `CancellationToken` | kullanici/kapanis | `IShutdownHook` |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Kurulu container | N38 pipeline | her port `Probe` ile |
| `StartupReport` | l7 + log | `FailedAt` + `SilentFailure` |
| `IBridgeOptions` | l4/l5 servisleri | sir icermez |
| Kapanis temizligi | isletim sistemi | `IDisposalOrder` |

## §4 Yasak Yonler (ADR-028 matrisi)

| Yon hucresi | Deger | Anlam |
|---|---|---|
| `l6 -> l7` | **X** | UI'a bakis yok |
| `l0..l5 -> l6` | **X** | container'dan habersiz katmanlar |
| `l6 -> l0..l5` | OK | tek katman ki hepsine bakabilir |
| `l7 -> l6` | OK | **tek bakan** katman |
| `l6 -> l3` | OK | `ILogger` portu; elle `new` yasak |

```text
  l0 l1 l2 l3 l4 l5  (hepsine bakabilir)
            |
            v
   +---------------------+
   |  l6 MACRO (N51)     |  composition root (tek yazim, tek sira)
   |  N39 DI -> N40 cfg  |
   |       -> N38 acilis |
   +----------+----------+
              ^  yasak: l6 -> l7 (X) · l0..l5 -> l6 (X)
              |  yalniz l7 bakabilir
   +----------+----------+
   | l7 Desktop / Cli    |
   +---------------------+
```

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N38 | bootstrap-pipeline | N40 | config-bridges |
| N39 | di-registration | N51 | makro l6 (5 kit) |

Seviye kurali: N kodlari klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Host` | `net48;net10.0-windows` | katmanin tek uretim projesi (9 uretimden biri) |
| `VersaCoder.Tests` | cift bacak | acilis + DI + config testleri (`Category=Host`) |
| `VersaCoder.ArchitectureTests` | cift bacak | NetArchTest: `ServiceCollection` yalniz Host'ta |

`net48` C# 7.3 alt kumesi, `net10.0-windows` C# 14; giris/kapanis API farki `#if` ile sinirli, adim sirasi ve kayit listesi **ortak**.

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tur |
|---|---|---|
| l6 | `VersaCoder.Abstractions`..`VersaCoder.Protocol` (l0..l5) | izinli (tek katman hepsine bakar) |
| l6 | `VersaCoder.Crosscutting` (l3) logger portu | izinli; elle `new` yasak |
| l6 | `VersaCoder.Desktop` / `VersaCoder.Cli` (l7) | matris **X** -> yasak |
| l6 | `Microsoft.Extensions.DependencyInjection` (NuGet) | surum G8 (`NEEDS_VALIDATION`) |
| l0..l5 | `VersaCoder.Host` | **X** -> NetArchTest = 0 |

## §8 Test Stratejisi

| Test | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | adim sirasi, fail-fast, Probe, config onceligi | **%90** satir |
| Container saglik | her l0 portu cozulur | 0 eksik port |
| Arch (NetArchTest) | `ServiceCollection` yalniz Host'ta; `l0..l5 -> l6` = 0 | **0 ihlal** |
| Gate | iki bacakta acilis testi (GATE-004 / GATE-005) | her iki TFM yesil |

Kosum: `dotnet test --filter Category=Host` · 13 soru icin [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P51-1 | Probe testi (container sagligi) | 0 eksik port - **DOĞRULAMA GEREKLİ** |
| P51-2 | Adim sirasi + fail-fast testleri | gecti - **DOĞRULAMA GEREKLİ** |
| P51-3 | `#if` taramasi (yalniz N40) | disarida 0 - **DOĞRULAMA GEREKLİ** |
| P51-4 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |
| P51-5 | Klasor sayisi = 5 (index + N38..N40 + N51) | 5 |

`src/` henüz diskte olmadigindan kod bazli ihlal sayilari `DOĞRULAMA GEREKLİ` kalir; ilk build sonrasi NetArchTest ciktisi bu tabloya islenir.
