---
title: "Katman Şartnamesi — l0 Abstractions (VersaCoder)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l0 Abstractions katmaninin sorumlulugu, giren/cikan akisi, ADR-028 yasak yonleri, N01-N05+N45 dugumleri, bagimlilik listesi ve test stratejisi"
---

# Katman Şartnamesi — l0 Abstractions

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test-strategy](../../spec/test-strategy.md) · [N45-macro-l0](N45-macro-l0.md)

> **Tek cumlelik sorumluluk:** *VersaCoder'da tip olarak neyin var oldugu, nasil adlandigi ve hangi sozlesmeyi bozamayacagi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l0 **hicbir sey calistirmez, yalniz imza yayimlar**: `Result`/`Error` hata dili, port sozlesmeleri, olay ABI'si, guard/extension ve sabit adlar. Davranis l1..l5'tedir (SRP); bu katman test edilebilirligin kaynagidir. Yon **tek yonludur `l1..l7 -> l0`**; l0'in `VersaCoder.*` ve NuGet referansi **0** olmalidir (ADR-028 §2.1).

| Alan | Deger |
|---|---|
| Katman kodu | l0 — inşa sırasi 1, ilk derlenen katman |
| Proje / TFM | `VersaCoder.Abstractions` · `net48;net10.0-windows` |
| N araligi | N01..N05 (alt dugum) + N45 (makro ozet) |
| Sahip rol / gate | `architect` · GATE-004a -> GATE-003 -> GATE-004 |
| Buna bakabilen | l1, l2, l3, l4, l5, l6, l7 — tumu |
| Buna bakmasi yasak | l0'in kendisi hicbir `VersaCoder.*` tipine bakamaz |

---

## §1 Sorumluluk (NE YAPAR)

| # | Sorumluluk | Kanit (nasil anlasilir) | Cikti |
|---|---|---|---|
| A1 | `Result<T>`, `Result`, `Error`, `ErrorCode` tiplerini yayimlar | tum katmanlar ayni hata dilini kullanir | `.cs` |
| A2 | Generic repository portu tanimlar (`IRepository<TEntity,TId>`, `IUnitOfWork`) | tip-spesifik genisletme l1'de, uygulama l4'te | arayuz |
| A3 | Veri saglayici soyutlamasini tanimlar (`IDbConnectionFactory`, `ISqlDialect`) | Sqlite/SqlClient gecisi yalniz l4'te olur | arayuz |
| A4 | Dosya ve proses portlarini tanimlar (`IFileSystem`, `IProcessRunner`) | tool uygulamasi l5'ten l4'e iner | arayuz |
| A5 | Olay ve handler sozlesmelerini tanimlar (`IDomainEvent`, `IAgentEvent`, `IDispatcher`) | ureten l1/l5, tuketen l2/l7 | arayuz |
| A6 | LLM tasma portunu tanimlar (`ILlmTransport`) | saglayici degisiminde yalniz l5 degisir | arayuz |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l1, l2, l3, l4, l5, l6, l7 -> l0 | izinli ok | dogrudan `ProjectReference`, arayuz uzerinden |
| GIREN | test projesi -> l0 | izinli ok | `VersaCoder.Tests` (unit + arch) |
| CIKAN | `Result<T>` / `Error` donusu | cikti | istisna katman sinirini **cikmaz** |
| CIKAN | port imzalari | cikti | govde yok; uygulayici l4 / l5 |
| CIKAN | olay ABI'si + sabit adlar | cikti | zaman l3 `IClock`'tan gelir |
| YASAK | l0 -> l1..l7 (herhangi biri) | yasak ok | `using VersaCoder.` = 0 satir |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dosyadaki anlami |
|---|---|---|---|
| 1 | `l0 -> l1..l7` | ADR-028 §2.1 | en alt sabit; tek istisna yok, CRITICAL |
| 2 | `l2 -> l4` | ADR-028 §2.1 | repository portu l0'da, uygulamasi l4'te — dogrudan ok YOK |
| 3 | `l4 -> l2` | ADR-028 §2.1 + §2.3/2 | sira degil kenar; port disi tip paylasimi yasak |
| 4 | `l5 -> l4` | ADR-028 §2.3/2 | protokol katmani altyapıya bakmaz |
| 5 | `l3 -> l1, l2, l4..l7` | ADR-028 §2.1 | l3 yalniz l0'a bakar (IClock burada degil, l3'te) |
| 6 | `l7 -> l1, l2, l4, l5` | ADR-028 §2.1 | UI yalniz l6 uzerinden baglanir |

---

## §4 N Kodlari ve Projeler

| N | Alt katman | Klasor | Proje |
|---|---|---|---|
| N01 | ports-core | `src/VersaCoder.Abstractions/Ports/` | `VersaCoder.Abstractions` |
| N02 | result-error | `src/VersaCoder.Abstractions/Results/` | `VersaCoder.Abstractions` |
| N03 | events-abi | `src/VersaCoder.Abstractions/Events/` | `VersaCoder.Abstractions` |
| N04 | extensions | `src/VersaCoder.Abstractions/Guard/` | `VersaCoder.Abstractions` |
| N05 | constants | `src/VersaCoder.Abstractions/Constants/` | `VersaCoder.Abstractions` |
| N45 | makro l0 ozeti | bu klasor | katman ozeti (proje degil) |

**Klasor seviyesi kurali:** N01..N44 klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL (`System`, `System.Collections`, `System.Threading.Tasks`) | l0 -> System | **izinli** | iki bacakta da ayni |
| 2 | NuGet / `Directory.Packages.props` paketi | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | `VersaCoder.*` tipleri | l0 -> diger katmanlar | **yasak** | hizli duzeltme yok, port eksiktir |
| 4 | Ust katmanlarin l0'a bakmasi | l1..l7 -> l0 | **izinli** | herkes bu katmani okuyabilir |
| 5 | Test paketleri (yalniz test projesinde) | test -> l0 | **kosullu** | urun `.csproj`'ina girmez |

---

## §6 Test Stratejisi

| Test turu | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | `Result`/`Error` kurallari: `Ok`, `Fail`, `Value` davranisi | **%100** satir |
| Unit | guard istisna tipleri + sabit ad benzersizligi | %100 |
| Arch (NetArchTest) | `VersaCoder.Abstractions -> VersaCoder.*` bagimliligi | **0** |
| Arch | NuGet paketi + `DateTime.Now` + `File.*` kullanimi | **0** |
| Sozlesme | her `public interface` XML yorumu tasiyor | 0 eksik |

---

## §7 Sinir Sorulari (layer-boundary-check)

| # | Soru | Kanit |
|---|---|---|
| 2 | l0 hicbir seye bagimli degil mi? | `.csproj` `ProjectReference` listesi bos ya da yalniz test paketleri |
| 1 | NetArchTest 0 ihlal mi? | `dotnet test --filter Category=Architecture` -> `0 failure` |
| 8 | Dairesel bag var mi (A -> B -> A)? | NetArchTest `NoCircularDependencies` -> `0 failure` |
| 9 | `#if` yalniz platform farkinda mi? | l0 icinde `#if` hedefi **0** |

---

## §8 Kurallar

- Yeni port eklemek = kardes [N01-ports-core](N01-ports-core.md) §1 tablosuna **ayni gun** satir eklemek; tabloya yazilmayan port mimari olarak yoktur.
- `Result<T>` bu katman disinda **yeniden tanimlanamaz**; ikinci sonuc tipi iki hata dili dogurur.
- `DateTime.Now` / `Guid.NewGuid()` / `Environment.*` / `File.*` / `Console.*` l0 dosyasinda **gizli bagimliliktir**.
- `net48` dili C# 7.3 oldugundan ortak kaynakta `record`/`init` kullanilmaz; aksi halde tek bacak derlenir.
- Port govdesine is mantigi yazilmaz; yazildigi an bu katman l1'e tasmistir (reviewer bulgusu: HIGH).

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Bu dosya 3-8 KB araliginda | `Get-Item ...l0-abstractions\index.md).Length` | olcum |
| 3 | Unicode kutu cizim (U+2500-U+257F) yok | diyagram yok; tablo `| -` ile cizildi | gozden gecirme |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 + §2.3 | dogrulandi |
| 5 | Madde listeleri <= 5 madde | §8 = 5 madde | sayim |
