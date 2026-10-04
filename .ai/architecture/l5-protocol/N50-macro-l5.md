---
title: "N50 MAKRO - l5 Protocol"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N50 - l5 tamamini kapsar: 5 kit, katman yonu, katman geneli hata ve test (kayit tablosu DEGIL)
---

# N50 MAKRO - l5 Protocol

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N33-tool-schema.md](N33-tool-schema.md)
- [N34-messages.md](N34-messages.md)
- [N35-session-transcript.md](N35-session-transcript.md)
- [N36-loop-policy.md](N36-loop-policy.md)
- [N37-versioning-errors.md](N37-versioning-errors.md)
- [../../index.md](../../index.md)
- [../00-overview/system-overview.md](../00-overview/system-overview.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N50, l5'in tamamini neyi tanimladigini, kime bakabildigini ve disariya hangi sema imzalarini actigini tek yerde toplar.

## §0 Ozet (Answer First)

- l5 **hicbir sey calistirmaz**: arac, mesaj, oturum, dongu ve surum/hata **semasini** tanimlar ve dogrular (fail-closed); N33..N37 tek bir isi yapar - **bicimi** garanti etmek, eylemi l4'e, gosterimi l7'ye, baglamayi l6'ya birakmak.
- l5 **l4'e dogrudan bakmaz** (port l0'da, uygulama l4'te - yon `?` DOĞRULAMA GEREKLİ), **l6/l7'ye ve l3'e bakamaz** (matris X).
- Disariya acilan yuzey 5 kit ile sinirlidir: `IProtocolCore`, `IToolSchemaKit`, `IMessageKit`, `ISessionKit`, `ILoopKit`.
- Ilerleme kosulu: NetArchTest `VersaCoder.Protocol` -> Infrastructure/Host/UI = 0 + parite testleri (§9 P50-1, P50-2).

## §1 Sorumluluk (5 kit)

| Kit (makro arayuzu) | Alt dugum | Disariya acilan tek cumlelik imza |
|---|---|---|
| `IProtocolCore` | N34 + N37 | `IMessageSerializer.Serialize(MessageEnvelope)` / `.Deserialize(string)` |
| `IToolSchemaKit` | N33 | `ISchemaValidator.Validate(object input, IToolParameterSchema s)` |
| `IMessageKit` | N34 | `MessageEnvelope { protocolVersion, role, content[] }` |
| `ISessionKit` | N35 | `ISessionState` + `ITranscriptStore` (**port**) + `ICompactionRule` |
| `ILoopKit` | N36 | `ILoopGuard.Check(LoopState) -> Result<LoopDecision>` |

```csharp
// l5'in tek yuzeyi: TIPLER + Validate(). Calistirma, dosya, HTTP, SQL burada YOK.
public interface IProtocolCore
{
    Result<T> Validate<T>(string json);   // fail-closed: seydisiz -> Result.Fail
}
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `ToolCall` JSON | l4 N31/N32 ya da l7 N43 | `IToolCallSchema.FromJson` + `Validate()` |
| `MessageEnvelope` | l7 istemci | `protocolVersion` + `role` zorunlu |
| Uzak surum bilgisi | l4/l7 | `IVersionNegotiation.Compatible` |
| Butce/limit ayari | l6 N40 config | `ILoopPolicy`, `IContextWindow` |
| `CancellationToken` | l6 agent dongusu | iptal -> `CANCELLED` |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Dogrulanmis `Result<ToolCall>` | l4 N32 | gecerliyse eyleme gecer |
| `ErrorCode` / `ProtocolViolation` | l4, l7 | tek hata katalogu (N37) |
| `SessionState` + `SummaryFrame` | l6 dongu, l7 gosterim | seq korunur |
| `LoopDecision` (Continue/Stop/Pause) | l6 | `Result` ile |
| JSON cikti (round-trip) | iki bacak ayni | metin karsilastirma testi |

## §4 Yasak Yonler (ADR-028 matrisi)

| Yon hucresi | Deger | Anlam |
|---|---|---|
| `l5 -> l6`, `l5 -> l7` | **X** | ust katmana bakis yok |
| `l5 -> l3` | **X** | matris `l3 yalniz l0` kurali disinda tasiyamaz |
| `l5 -> l4` | **?** | §2.1 zinciri `->` vs §2.3/2 `l5 -> l4 X` -> `DOĞRULAMA GEREKLİ` |
| `l5 -> l0`, `l1`, `l2` | `.` | gecisli; port ve tipler `l0` uzerinden |
| `l6 -> l5`, `l7 -> l5` | OK / `.` | tuketim serbest - sema **okunur**, l5 geri bakmaz |

```text
   l7 (gosterim)      l6 (baglama)
        \                /
         v              v   semayi uygular (tek yon)
              l5 PROTOCOL  --semayi uygular-->  l4 (calistirir)
         \              /
          yasak: l5 -> l6 / l7 / l3 (X) · l5 -> l4 (?) DOĞRULAMA GEREKLİ
```

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N33 | tool-schema | N36 | loop-policy |
| N34 | messages | N37 | versioning-errors |
| N35 | session-transcript | N50 | makro l5 (5 kit) |

Seviye kurali: N kodlari klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Protocol` | `net48;net10.0-windows` | katmanin tek uretim projesi (9 uretimden biri) |
| `VersaCoder.Tests` | cift bacak | sema/round-trip/surum testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | cift bacak | NetArchTest: `l5 -> l3/l6/l7` = 0 |

`net48` C# 7.3 alt kumesi (`record` yalniz `#if NET`), `net10.0-windows` C# 14. Iki bacakta **ayni serializer ve ayni JSON ciktisi** zorunlu (parite testi).

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tur |
|---|---|---|
| l5 | `VersaCoder.Abstractions` (l0): `Result`, `Error`, events-abi | izinli (`ProjectReference`) |
| l5 | `VersaCoder.Domain` (l1) / `VersaCoder.Application` (l2) tipleri | matriste `.` -> gecisli |
| l5 | `VersaCoder.Crosscutting` (l3) | matris **X** -> `dependency-map.md` |
| l5 | `VersaCoder.Infrastructure` (l4) | **?** -> `DOĞRULAMA GEREKLİ` |
| l5 | `System.Text.Json` (NuGet) | surum `Directory.Packages.props` (G8) |

## §8 Test Stratejisi

| Test | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | fail-closed vaka tablosu, `LoopPolicy` sinirlari | **%90** satir |
| Parite | JSON <-> tip round-trip, iki bacak ayni cikti | 0 fark |
| Arch (NetArchTest) | `l5 -> l3/l4/l6/l7` kenari, dosya/HTTP/SQL cagrisi | **0 ihlal** |
| Contract | surum matrisi `(1<->1 ok, 1<->2 yok)` | 100% vaka |

Kosum: `dotnet test --filter Category=Protocol` · 13 soru icin [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P50-1 | NetArchTest: `VersaCoder.Protocol` -> Infrastructure/Host/UI = 0 | 0 ihlal - **DOĞRULAMA GEREKLİ** |
| P50-2 | Parite: JSON round-trip iki bacakta ayni cikti | 0 fark - **DOĞRULAMA GEREKLİ** |
| P50-3 | `dotnet test --filter Category=Protocol` | gecti/kaldi sayisi - **DOĞRULAMA GEREKLİ** |
| P50-4 | Dosya boyutu 2.600-8.500 bayt (`Get-Item .Length`) | aralik icinde |
| P50-5 | Klasor sayisi = 7 (index + N33..N37 + N50) | 7 |

`src/` henüz diskte olmadigindan kod bazli ihlal sayilari `DOĞRULAMA GEREKLİ` kalir; ilk build sonrasi NetArchTest ciktisi bu tabloya islenir.
