---
title: "N33 Arac Semasi"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N33 - arac JSON semasi, fail-closed dogrulama, izin haritasi (kayit tablosu DEGIL)
---

# N33 Arac Semasi

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N34-messages.md](N34-messages.md)
- [N50-macro-l5.md](N50-macro-l5.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)
- [../../spec/test-strategy.md](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** N33, bir aracin (tool) JSON semasinin hangi alanlardan olustugunu ve sema disi cagrinin nasil reddedildigini (fail-closed, yorumlama yok) tek yerde tanimlar.

## §0 Ozet (Answer First)

- Araclar (`read/write/edit/bash/grep/glob/list/webfetch/task/todowrite`) **tek bir kayit tipinde** (`ToolDefinition`) tanimlanir; ikinci bir tanim yolu yoktur.
- `ISchemaValidator.Validate()` **fail-closed** calisir: eksik `zorunlu` alan, taninmayan arac, tip uyuşmazligi => `Result.Fail(ProtocolViolation)`; "en iyi niyetle" coerce etme **yok**.
- Sahip oldugu nuans: hata mesaji **hangi alanin** eksik oldugunu soyler ama ham girdi metnini loglamaz (gizli veri sizintisi yok).
- Ilerleme kosulu: 10 arac x gecerli cagri + zorunlu alan cikarma vaka tablosu (§9 P33-1, P33-2).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `IToolDefinition.Name` / `.Description` / `.InputSchema` | aracin tek kayit karti |
| `interface` | `IToolParameterSchema.Properties` / `.Required` | JSON Schema alt kumesi |
| `interface` | `IToolRegistry.Get(string name)` | isim -> tanim; yoksa `null` degil `Result` |
| `interface` | `ISchemaValidator.Validate(object input, IToolParameterSchema s)` | fail-closed dogrulama |
| `interface` | `IToolPermissionMap.IsAllowed(string tool, string action)` | aracin izin haritasi |

```csharp
public interface ISchemaValidator
{
    Result Validate(object input, IToolParameterSchema schema);
}
// Kurallar: bilinmeyen ANAHTAR = hata; zorunlu eksik = hata; tip uyumsuz = hata.
// asla coerce etme -> ProtocolViolation (l5 P5, fail-closed).
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `ToolCall` JSON | ureten taraf (N32/l4 ya da N43/l7) | `IToolCallSchema.FromJson` ile tipe cevrilir |
| Arac tanimlari | Bu katman (N33) | Kayit + `Validate()` girdisi |
| Izin haritasi | l6 config (N40) | `IsAllowed` - kapaliysa cagri daha once dus |
| `protocolVersion` | N34/N37 | Sema surumuyle uyum kontrolu |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Dogrulanmis `Result<ToolCall>` | N32 (l4) | gecerliyse eyleme gecer |
| `ProtocolViolation(UNKNOWN_TOOL/MISSING_FIELD/TYPE_MISMATCH/UNKNOWN_FIELD/PERMISSION_DENIED/MALFORMED)` | N37 hata katalogu | kod tek yerden |
| Tool katalogu (isim + sema) | N34 mesaj uretimi | model'e giden arac listesi |
| Reddedilen cagri kaydi | l0 olay kanali | ham metin yok, sadece kod |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N33 notu |
|---|---|---|
| l5 -> l4 | **?** | DOĞRULAMA GEREKLİ - port l0'da, uygulama l4'te; matris >2.1 cekismesi |
| l5 -> l6, l5 -> l7 | X | config/gosterim okunmaz; izin haritasi parametre olarak gelir |
| l5 -> l3 | X | Crosscutting tasiyamaz |
| l5 -> l0, l1, l2 | . | gecisli; `Result`/`Error` tipleri l0 uzerinden |
| l6 -> l5, l7 -> l5 | OK / . | sema **okunur**, l5 geri bakmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N33 | Arac semasi | Bu dosya |
| N34 | Mesaj tipleri | `IToolCallPart` icinde tasir |
| N36 | Dongu politikasi | Arac cagrisi adim butcesinden duser |
| N37 | Surum/hata | `ProtocolViolation` kodu buradan |
| N32 | Tool runner (l4) | Dogrulanmis cagrinin eylemcisi |

## §6 Projeler

| Proje | N33 katkisi |
|---|---|
| `VersaCoder.Protocol` | Kayit tipleri, validator, izin haritasi (tek uretim projesi) |
| `VersaCoder.Tests` | Vaka tablosu testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `VersaCoder.Protocol` -> Infrastructure/Host/UI = 0 |

TFM: `net48;net10.0-windows` - `net48` C# 7.3 alt kumesi (`record` yalniz `#if NET`), `net10.0-windows` C# 14; serializer iki bacakta **ayni** (`System.Text.Json`, surum `Directory.Packages.props` G8 - `NEEDS_VALIDATION`).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l5 -> l0 | `Result` / `Error` / events-abi (`VersaCoder.Abstractions`) | izinli (ProjectReference) |
| l5 -> l1, l2 | tipler (matriste `.`) | gecisli |
| l5 -> l3 | `VersaCoder.Crosscutting` | matris **X** - yasak |
| l5 -> l4 | `VersaCoder.Infrastructure` | **?** - DOĞRULAMA GEREKLİ |
| harici | `System.Text.Json` (NuGet) | surum sabit (G8) |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Gecerli cagri kabulu | `dotnet test --filter Category=Protocol` | 10 arac x 1 |
| Zorunlu alan cikarma | vaka tablosu (Theory) | her `Required` alan tek tek |
| Sert mod (UNKNOWN_FIELD) | birim test | fail-closed kaniti |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N33 sinirlari |
| Mimari | NetArchTest | l5 -> l3/l6/l7 = 0 kenar |

Kosum: `dotnet test --filter Category=Protocol`.

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P33-1 | `dotnet build VersaCoder.Protocol` | 0 hata |
| P33-2 | Vaka tablosu: eksik zorunlu alan -> reddedilir | hepsi RED gecti - **DOĞRULAMA GEREKLİ** |
| P33-3 | Taninmayan arac adi eyleme donusmez | sahte ile: l4'e gitmedi dogrulanir |
| P33-4 | NetArchTest: `VersaCoder.Protocol` -> Infrastructure = 0 | 0 ihlal - **DOĞRULAMA GEREKLİ** |
| P33-5 | Dosya boyutu 2.600-8.500 bayt (`Get-Item .Length`) | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir; ilk build sonrasi NetArchTest ciktisi bu tabloya islenir.
