---
title: "N39 DI Kayitlari"
type: architecture
category: l6-host
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N39 - tek ServiceCollection dosyasi, modul registrar, Probe/LifetimeMap saglik denetimi (kayit tablosu DEGIL)
---

# N39 DI Kayitlari

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N38-bootstrap-pipeline.md](N38-bootstrap-pipeline.md)
- [N40-config-bridges.md](N40-config-bridges.md)
- [N51-macro-l6.md](N51-macro-l6.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N39, hangi tipin hangi omur (lifetime) ile kime baglandiginin **tek dosyada** tutuldugunu ve her portun gercekten cozulebildiginin kanitlanmasini saglar.

## §0 Ozet (Answer First)

- **Tek yazildigi yerdir**: tum `Add*` cagrilari `ServiceCollectionExtensions` icindedir; baska hicbir katmanda container gorunmez (NetArchTest ihlali = gate kapali).
- `IResolverProbe` her l0 portunu acilista **cozulebilir mi** diye dogrular; `ILifetimeMap` omur ihlalini (captive dependency) reddeder - ikisi de `Result.Fail(DI_MISSING / DI_DUPLICATE / DI_LIFETIME)`.
- Test ikamesi (`ITestDoubleRegistrar`) **uretimde yoktur**; bulunursa `CRITICAL` sayilir.
- Ilerleme kosulu: her l0 portu `Probe` ile + `ServiceCollection` yalniz `VersaCoder.Host` icinde (§9 P39-1, P39-4).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `class` | `AddVersaCoder(services, config)` | tum kayitlar tek cagri |
| `interface` | `IModuleRegistrar.Register(IServiceCollection s)` | kit basina kayit (N49..N52) |
| `interface` | `IContainerHealth.Probe()` / `IResolverProbe.Resolve(Type t)` | her port cozulebilir mi |
| `interface` | `ILifetimeMap.Verify()` | scoped/singleton uyumu |
| `interface` | `IDisposalOrder` + `ITestDoubleRegistrar` | kapanis sirasi + test ikamesi |

```csharp
public static IServiceCollection AddVersaCoder(this IServiceCollection s, IConfiguration c)
{
    s.AddInfrastructureCore(c);   // N49 -> N25..N32
    s.AddProtocolCore();          // N50 -> N33..N37
    s.AddHostCore(c);             // N51 -> N38..N40
    s.AddDesktopCore();           // N52 -> N41..N44
    return s;                     // baska hicbir katmanda ServiceCollection YOK
}
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `IServiceCollection` + `IConfiguration` | l7 `Program.Main` | tek `AddVersaCoder` cagrisi |
| Config objeleri | N40 `IConfigBootstrapper` | kayit aninda parametre |
| Port katalogu (l0) | test verisi | `Probe` listesi surulebilir |
| Test ikamesi | `VersaCoder.Tests` yalniz | `Replace` - uretimde yok |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Kurulu container | N38 `IStartupPipeline` | saglikliysa devam |
| `Result.Fail(DI_MISSING / DI_DUPLICATE / DI_LIFETIME / SHUTDOWN_DIRTY)` | acilis raporu | hangi port/omur adıyla |
| NetArchTest bulgusu | gate | `ServiceCollection` disi hata |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N39 notu |
|---|---|---|
| l0..l5 -> l6 | **X** | baska katmanda container = ihlal |
| l6 -> l7 | **X** | Desktop/Cli tipleri yalniz `AddDesktopCore` altinda, geri bakis yok |
| l6 -> l0..l5 | OK | tek katman hepsine baglanir |
| l6 -> l3 | OK | logger portu uzerunden; elle `new` yasak |
| test -> prod | yasak | `ITestDoubleRegistrar` uretim kanalinda yok |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N39 | DI kayitlari | Bu dosya |
| N38 | Acilis boru hatti | Kayit sonrasi pipeline |
| N40 | Config/TFM koprusu | Kayit girdisi |
| N49/N50/N51/N52 | Makro registrar'lar | 4 kit `IModuleRegistrar` |
| N25..N44 | alt dugumler | Kaydedilen gercek tipler |

## §6 Projeler

| Proje | N39 katkisi |
|---|---|
| `VersaCoder.Host` | `ServiceCollectionExtensions`, registrar'lar, Probe/LifetimeMap |
| `VersaCoder.Tests` | Port cozum + cift kayit + captive testleri (`Category=Host`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `ServiceCollection` yalniz Host'ta |

TFM: `net48;net10.0-windows` - container ayni `Microsoft.Extensions.DependencyInjection` API'si (G8 surum sabit - `NEEDS_VALIDATION`); yalniz scope olusturma API farki `#if` ile N40'a tasinir.

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l6 -> l0..l5 | portlar + tipler (ProjectReference) | izinli |
| l6 -> l7 | `VersaCoder.Desktop` / `VersaCoder.Cli` | matris **X** - yalniz `AddDesktopCore` uzerinden kodlanir, geri bakis yok |
| l0..l5 -> l6 | `VersaCoder.Host` | **X** - NetArchTest = 0 |
| harici | `Microsoft.Extensions.DependencyInjection` (NuGet) | surum G8 |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Port cozumu | `dotnet test --filter Category=Host` | her l0 portu `Probe` |
| Cift kayit | birim test | ayni tip 2x -> acilis reddi |
| Captive dependency | birim test | `DI_LIFETIME` uretilir |
| Mimari | NetArchTest | `ServiceCollection` yalniz Host'ta |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N39 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P39-1 | Probe testi (port listesi) | 0 eksik port - **DOĞRULAMA GEREKLİ** |
| P39-2 | `dotnet build VersaCoder.Host` | 0 hata |
| P39-3 | Captive + cift kayit senaryolari | DI_LIFETIME / DI_DUPLICATE |
| P39-4 | NetArchTest: disinda `ServiceCollection` = 0 | 0 ihlal |
| P39-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
