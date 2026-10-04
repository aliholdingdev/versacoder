---
title: "N36 Dongu Politikasi"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N36 - adim butcesi, derinlik, eszamanlilik, iptal; tek kapili ILoopGuard (kayit tablosu DEGIL)
---

# N36 Dongu Politikasi

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N35-session-transcript.md](N35-session-transcript.md)
- [N37-versioning-errors.md](N37-versioning-errors.md)
- [N50-macro-l5.md](N50-macro-l5.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N36, agent dongusunun kac adimda, hangi kosulda duracagini ve paralel alt ajanlara ne kadar izin verildigini tek kapida (ILoopGuard) buyur.

## §0 Ozet (Answer First)

- Dongu **sinir politikasiyla** yonetilir: `IStepBudget` adim sayisini, `IDepthLimit` alt ajan derinligini, `IParallelPolicy` eszamanlilik kotalar.
- Butce ya da derinlik asiminda dongu **once temiz durur** (`LOOP_BUDGET_EXCEEDED` / `LOOP_DEPTH_EXCEEDED`) ve `PauseSignal` olarak doner - **sonsuz dongu mumkun degildir**.
- Karar **tek noktadan** cikar: her modul kendi sinirini AYRI uygularsa dongu kirilir; ayni `LoopState` girisi iki bacakta **ayni** karari verir (determinizm).
- Ilerleme kosulu: butce 3 ise 4. adim asla baslamaz + stall korumasi (`LOOP_NO_PROGRESS`) (§9 P36-1, P36-4).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `ILoopGuard.Check(LoopState s)` | tek kapi: tum sinirlari toplar |
| `interface` | `IStepBudget.Remaining` / `.Consume()` | kalan adim hakki |
| `interface` | `IDepthLimit.Current` / `.CanNest()` | alt ajan derinligi |
| `interface` | `IParallelPolicy.MaxConcurrency` | eszamanli alt ajan ust siniri |
| `interface` | `IStopCondition.IsMet(LoopState s)` | donguyu bitiren kosul |

```csharp
public interface ILoopGuard
{
    Result<LoopDecision> Check(LoopState s);   // Continue | Stop | Pause
}
// karar tek noktadan cikar; budget asimi -> Stop(reason); iptal -> Pause.
// ikisi de Result ile ifade edilir, exception degil.
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `LoopState` (adim no, durum ozeti) | l6 agent dongusu | Her adim basinda `Check` |
| Butce/derinlik/eszamanlilik sinirlari | l6 config (N40) | Politika nesnesi; limit <= 0 ise acilista reddet |
| `CancellationToken` | cagiran | iptal -> `Pause` / `CANCELLED` |
| Durum imzasi (ilerleme kontrolu) | N35 SessionState | Ayni durum 2. kez -> `LOOP_NO_PROGRESS` |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `LoopDecision.Continue` | l6 dongu | araca / N31'e devam |
| `Result.Fail(LOOP_BUDGET_EXCEEDED / LOOP_DEPTH_EXCEEDED / LOOP_NO_PROGRESS / CANCELLED)` | l6 + l7 | dongu durur, yapilan is korunur |
| `PauseSignal` | kullanici / l7 | devam icin onay kapisi |
| `ProtocolViolation` (tutarsiz politika) | N37 hata katalogu | acilista reddedilir |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N36 notu |
|---|---|---|
| l5 -> l6 | X | dongu **kodunu** okumaz; politika **tanimi** burada, baglama l6'da |
| l5 -> l7 | X | PauseSignal gosterimi dogrudan cekilmez |
| l5 -> l4 | **?** | DOĞRULAMA GEREKLİ - arac eylemi l4'te, karar l5'te |
| l5 -> l3 | X | Crosscutting tasinamaz; zaman `ISystemClock` (l3) uzerunden, `DateTime.Now` asla |
| l5 -> l0, l1, l2 | . | gecisli; `Result`/`LoopDecision` l0 tipleri |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N36 | Dongu politikasi | Bu dosya |
| N35 | Oturum transkripti | `ISessionState.LoopPolicy` + ilerleme imzasi |
| N33 | Arac semasi | Adim butcesi arac cagrisindan duser |
| N32 | Tool runner (l4) | Continue kararinin hedefi |
| N50 | Makro l5 | `ILoopKit` |

## §6 Projeler

| Proje | N36 katkisi |
|---|---|
| `VersaCoder.Protocol` | ILoopGuard, butce/derinlik/eszamanlilik politikalari (saf mantik) |
| `VersaCoder.Tests` | Sinir + iptal + determinizm testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: bu dugumde I/O cagrisi = 0 |

TFM: `net48;net10.0-windows` - saf mantik, **kod iki bacakta birebir ayni**; bu dugumde `#if` kullanilmaz (G9 gerekcesi yok). Iptal `CancellationTokenSource` + `CancelAfter` ile test edilir.

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l5 -> l0 | `Result` / `Error` / dongu tipi abi | izinli |
| l5 -> l1, l2 | tipler (matriste `.`) | gecisli |
| l5 -> l3 | `ISystemClock` (matris **X** dikkat) | `dependency-map.md` - DOĞRULAMA GEREKLİ |
| l5 -> l4 | `VersaCoder.Infrastructure` | **?** - DOĞRULAMA GEREKLİ |
| harici | yok | saf mantik - paket bagimliligi yok |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Butce siniri | `dotnet test --filter Category=Protocol` | 3 ise 4. adim baslamaz |
| Derinlik siniri | birim test | 3. katman alt ajan acamaz |
| Iptal (orta adim) | birim test | temiz CANCELLED + durum kaydi |
| Stall korumasi | birim test | ayni durum 2x -> LOOP_NO_PROGRESS |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N36 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P36-1 | Butce sinir testi (3 adim / 4. baslamaz) | gecti - **DOĞRULAMA GEREKLİ** |
| P36-2 | Derinlik sinir testi (3. katman reddi) | gecti - **DOĞRULAMA GEREKLİ** |
| P36-3 | Ayni LoopState iki bacakta ayni karar | determinizm - **DOĞRULAMA GEREKLİ** |
| P36-4 | `dotnet build VersaCoder.Protocol` | 0 hata |
| P36-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
