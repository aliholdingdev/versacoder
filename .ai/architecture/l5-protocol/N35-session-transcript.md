---
title: "N35 Oturum Transkripti"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N35 - SessionState, TranscriptEntry, ICompactionRule butce kirpmasi (port l0'da, uygulama l4'te) (kayit tablosu DEGIL)
---

# N35 Oturum Transkripti

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N34-messages.md](N34-messages.md)
- [N36-loop-policy.md](N36-loop-policy.md)
- [N50-macro-l5.md](N50-macro-l5.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N35, gecmisin hangi tiplerle saklandigini ve baglam butcesi asildiginda nasil kirpildigini (silme degil ozetleme) tanimlar.

## §0 Ozet (Answer First)

- `SessionState.messages[]` + `TranscriptEntry` (sira numarali) gecmisi tutar; `ICompactionRule` butce asildiginda **ozet kare uretip eski girdileri kirpar** - silme YOK, `seq`'ler korunur.
- Kalicilik **l4'tedir**: buradaki `ITranscriptStore` yalnizca **porttur** (l0 uzerinden); dosyayi l4 `IFileSystemStore` yazar. l5'te dosya I/O **yoktur**.
- Butce asiminda Once **compact, sonra devam**; ozet uretilemiyorsa oturum durmaz ama kirpma yapilmaz (`COMPACTION_FAILED`, fail-closed).
- Ilerleme kosulu: %80 compact edilmez / %101 compact edilir sinir testi (§9 P35-1, P35-2).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `ISessionState.Id` / `.Messages` / `.LoopPolicy` | oturumun tek kare durumu |
| `interface` | `ITranscriptEntry.Seq` / `.Role` / `.At` / `.Envelope` | sira numarali kayit |
| `interface` | `ITranscriptStore.AppendAsync(entry, ct)` / `.LoadAsync(id, ct)` | **port** (uygulama l4'te) |
| `interface` | `ICompactionRule.ShouldCompact(w)` / `.Summarize(entries)` | kirpma karari + ozet |
| `interface` | `IContextWindow.Used` / `.Limit` | token butcesi olcumu |

```csharp
public interface ICompactionRule
{
    bool ShouldCompact(IContextWindow w);
    SummaryFrame Summarize(IReadOnlyList<TranscriptEntry> older);
}
// kural: silme YOK - eski girdiler SummaryFrame'e donusur, seq'ler korunur.
// limit astiginda Once compact, sonra devam (fail-closed: otesine gecilmez).
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `MessageEnvelope` | N34 | `messages[]`'e eklenir |
| Butce / limit ayari | l6 config (N40) | `IContextWindow.Limit` |
| Var olan transkript | l4 `IFileSystemStore` (port uzerinden) | `LoadAsync` - bozuk satir atlanir |
| `CancellationToken` | l6 dongu | Append/Load iptali |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `SessionState` + `SummaryFrame` | l6 dongu, l7 gosterim | seq artmaya devam eder |
| `Result.Fail(COMPACTION_FAILED/SESSION_PERSIST/SESSION_CORRUPT/SESSION_NOT_FOUND)` | cagiran | hata = Result, exception yok |
| Kalici yazim (port) | l4 `IFileSystemStore` | dosya burada acilmaz |
| Uyarı durumu (bozuk satir) | l7 | oturum acilir + tek uyari |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N35 notu |
|---|---|---|
| l5 -> l4 | **?** | DOĞRULAMA GEREKLİ - port l0'da, uygulama l4'te; dosya I/O burada YOK |
| l5 -> l6, l5 -> l7 | X | dongu/gosterim paketi okunmaz |
| l5 -> l3 | X | `ISystemClock` disinda Crosscutting tasinamaz; tarih l3 uzerunden, `DateTime.Now` asla |
| l5 -> l0, l1, l2 | . | gecisli; port ve tipler l0 |
| l4 -> l5 | YOK | iliski tek yonlu: store **uygulamasi** l4'te, arayuzu l5'te |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N35 | Oturum transkripti | Bu dosya |
| N34 | Mesaj tipleri | Kaydedilen kare |
| N36 | Dongu politikasi | `ISessionState.LoopPolicy` |
| N29 | Dosya araclari (l4) | Portun gercek uygulamasi |
| N50 | Makro l5 | `ISessionKit` |

## §6 Projeler

| Proje | N35 katkisi |
|---|---|
| `VersaCoder.Protocol` | SessionState, TranscriptEntry, compaction kurallari (port arayuzu) |
| `VersaCoder.Tests` | Butce sinir + bozuk satir + determinizm testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `VersaCoder.Protocol` -> System.IO = 0 (port disinda dosya yok) |

TFM: `net48;net10.0-windows` - `net48` C# 7.3 alt kumesi (`IAsyncEnumerable` `NEEDS_VALIDATION`), tarih `ISystemClock` (l3) uzerunden.

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l5 -> l0 | `Result` / events-abi / port arayuzleri | izinli |
| l5 -> l1, l2 | tipler (matriste `.`) | gecisli |
| l5 -> l3 | `ISystemClock` (matris **X** dikkat) | `dependency-map.md` - DOĞRULAMA GEREKLİ |
| l5 -> l4 | `VersaCoder.Infrastructure` | **?** - DOĞRULAMA GEREKLİ |
| harici | `System.Text.Json` (NuGet) | surum sabit (G8) |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Butce siniri | `dotnet test --filter Category=Protocol` | %80 / %101 |
| Kompakt + seq devami | birim test | ozet + son 2 kare = tam baglam |
| Bozuk satir dayanikliligi | birim test | oturum acilir, uyari tek |
| Store port sahtesi | birim test | I/O yok, deterministik |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N35 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P35-1 | Butce sinir testi (80% kalmaz / 101% kisar) | gecti - **DOĞRULAMA GEREKLİ** |
| P35-2 | Ayni girdi listesi iki bacakta ayni ozeti uretir | determinizm - **DOĞRULAMA GEREKLİ** |
| P35-3 | NetArchTest: `VersaCoder.Protocol` -> System.IO = 0 | 0 ihlal |
| P35-4 | `dotnet build VersaCoder.Protocol` | 0 hata |
| P35-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
