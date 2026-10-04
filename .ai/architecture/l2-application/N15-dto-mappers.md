---
title: "N15 - DTO ve Mapper (Entity Disari Cikmasin)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N15 - DTO ve Mapper (Entity Disari Cikmasin)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N13-queries.md](N13-queries.md) | [N14-handlers.md](N14-handlers.md) | [N16-validators.md](N16-validators.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Entity'nin disari nasil cikmadigini, DTO'yu kimin urettigini ve donusumun nerede yasadigini belirler.

## §0 Ozet (Answer First)

DTO **yalniz l2'de** tanimlanir ve `public`'tir; entity -> DTO donusumu **tek yerde** (extension metot `ToDto()`) yasar — iki yerde donusum iki surum dogurur. `Result<Project>` gibi **entity disari cikan donusler yasaktir**: aksi halde l7 entity'ye baglanir ve `l7 -> l1` ihlali dogar. DTO'da **domain tipi referansi yoktur** (Guid/string/int/DateTimeOffset/string enum). `MessageDto` **l5**'tendir, buraya **kopyalanmaz** (SSOT).

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N15, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Envanter | 5 DTO |
| Varsayilan yon | l1 (okur) + l0 (Result) |

## §1 Sorumluluk

Tek sorumluluk: **guvenli gecis noktasi** — entity burada biter, DTO baslar.

| DTO | Alanlar | Ureten use case | Donusum |
|---|---|---|---|
| `ProjectDto` | `Id`, `Name`, `RootPath`, `CreatedUtc` | U1, U2, U3 | `Project.ToDto()` |
| `TaskDto` | `Id`, `ProjectId`, `State`, `Attempt`, `CreatedUtc`, `CompletedUtc` | U6, U7, U8 | `AgentTask.ToDto()` |
| `SettingDto` | `Key`, `Scope`, `Value` | U9, U10 | `Setting.ToDto()` |
| `FileContentDto` | `RelativePath`, `Text`, `LineCount`, `ModifiedUtc` | U4, U5 | `IFileSystem` sonucu |
| `PagedResult<T>` | `Items`, `Total`, `Skip`, `Take` | U1, U8 | sarmalayici |

```csharp
public static TaskDto ToDto(this AgentTask t) =>
    new TaskDto(t.Id.Value, t.ProjectId.Value, t.State.ToString(),
                t.Attempt, t.CreatedUtc, t.CompletedUtc);
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Entity (port'tan gelen) | l1 tipi |
| GIRENLER | Alan esleme kurali (tek yer) | bu dosya |
| CIKANLAR | Saf tip DTO (`public`) | l7, l5 tuketir |
| CIKANLAR | Serlestirilebilir veri | BCL `System.Text.Json` |
| CIKANLAR | Eksik alan => test kirmizi | alan bazi testi |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| DTO -> l4/l5/l6/l7 | X | DTO yalniz tanimlanir, bakilmaz |
| DTO -> l3-crosscutting | X (matris) | **DOGRULAMA GEREKLİ** |
| Entity disari cikmak | X | `l7 -> l1` ihlali |
| 2. bir `ToDto` | X | iki surum dogar |
| `MessageDto` kopyasi | X | SSOT ihlali (l5'te) |

Izinli: DTO -> l0 (`Result` sarmalayici), DTO -> l1 (entity alanlarini okur).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N15 | DTO/Mappers | 5 DTO + `ToDto` | VersaCoder.Application |
| N14 | Handlers | Donus tipi | VersaCoder.Application |
| N13 | Queries | `PagedResult<T>` | VersaCoder.Application |
| N11 | Enums | Enum `ToString()` (l1) | VersaCoder.Domain |
| N35 | session-transcript | `MessageDto` sahibi (l5) | VersaCoder.Protocol |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l1 entity | asagi | okuma |
| l0 `Result` | asagi | sarmalayici |
| l4/l5/l6/l7 | YOK | sifir |
| Newtonsoft.Json | YOK | BCL `System.Text.Json` (G8) |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Her DTO icin alan esitligi (entity -> DTO) | kayip yok |
| Arch | DTO'da domain tipi referansi yok | reflex/derleme testi |
| Arch | Tek `ToDto` tanimi | tip arama |
| Arch | `System.Text.Json` paket degil, BCL | `.csproj` denetimi |
| Unit | `PagedResult` `Skip/Take` yansitimi | sinir degeri |

## §7 Hata Durumlari

| Durum | Sorun | Koruma |
|---|---|---|
| Entity DTO'dan disari cikti | `l7 -> l1` ihlali | Arch testi: imza denetimi |
| Alan unutuldu | ekran yanlis veri gosterir | alan bazi testi |
| 2. `ToDto` tanimi | iki surum | tek sinif + Arch testi |
| Enum DTO'da sayi gitti | eski istemci okuyamaz | `ToString()` (l1 kurali) |
| `DateTime` (Kind'siz) | iki bacak farkli gorur | `DateTimeOffset` zorunlu |

## §8 Kurallar (5 madde)

- DTO'yu yalniz burada tanimla; baska katmana **kopyalama**.
- `ToDto()` tek yerde yasar; ikinci tanim yapma.
- DTO alanlari saf tip olsun (Guid/string/int/DateTimeOffset/enum-as-string).
- Entity disari cikan imza (`Result<Project>`) yazma.
- `MessageDto`/transcript l5'tedir; buraya tasima.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
