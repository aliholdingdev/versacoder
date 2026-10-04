---
title: "Katman Şartnamesi — l1 Domain (VersaCoder)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l1 Domain katmaninin sorumlulugu, giren/cikan akisi, ADR-028 yasak yonleri, N06-N11+N46 dugumleri, bagimlilik listesi ve test stratejisi"
---

# Katman Şartnamesi — l1 Domain

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test-strategy](../../spec/test-strategy.md) · [N46-macro-l1](N46-macro-l1.md)

> **Tek cumlelik sorumluluk:** *VersaCoder'in is kurali gercekte ne oldugu ve hangi durumun gecersiz sayildigi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l1'de **kural yasar; veritabani, ekran ve protokol yasamaz** (SRP): entity, value object, aggregate, invariant, durum makinesi ve domain event buradadir. Bagimlilik **yalniz l0** (sozlesme: `Result<T>`, `IDomainEvent`, `IRepository<,>`); SQL, dosya, form, DevExpress, JSON **gormez**. Invariant tek yerde dogrulanir, ihlali **istisna degil `Result.Fail(Invalid|Conflict)`**tir — ayni kurali handler'da tekrar yazmak iki gercek kaynagi dogurur.

| Alan | Deger |
|---|---|
| Katman kodu | l1 — inşa sırasi 3 (l0'dan sonra) |
| Proje / TFM | `VersaCoder.Domain` · `net48;net10.0-windows` |
| N araligi | N06..N11 (alt dugum) + N46 (makro ozet) |
| Sahip rol / gate | `architect` · GATE-004a -> GATE-003 -> GATE-004 |
| Izinli bakis | **l0** (sozlesme) — matriste OK |
| Buna bakmasi yasak | l2, l4, l5, l6, l7 (hepsi CRITICAL) |

---

## §1 Sorumluluk (NE YAPAR)

| # | Sorumluluk | Kanit (nasil anlasilir) | Cikti |
|---|---|---|---|
| A1 | Aggregate root entity'leri tanimlar (`Project`, `AgentTask`, `Setting`) | fabrika metodu + `private set` | `.cs` |
| A2 | Value object'leri tanimlar (kimlik, yol, aralik, ad) | esitlik deger uzerinden | sinif |
| A3 | Tip-spesifik repository portlarini genisletir (`IProjectRepository : IRepository<Project,Guid>`) | l0 ile uyumlu, uygulamasi l4 | arayuz |
| A4 | Domain servisleriyle kural hesaplar (`TaskStateMachine`, `ContextBudget`, `PathPolicy`) | saf fonksiyon, yan etki yok | sinif |
| A5 | Invariant'lari (I1..I7) tek yerde uygular | ihlalde `Result.Fail` | `Result` |
| A6 | Domain event'leri tanimlar, durum gecislerini dis kapali yapar | gecersiz gecis `Conflict` | olay + `Result` |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l2 handler -> entity/metot cagrisi | izinli ok | on kosullari l2 kontrol eder |
| GIREN | l4 adaptoru -> entity + port genisletmesi | izinli ok | Dapper esleme `private` kurucu |
| GIREN | l5 -> `SessionId` (VO) | izinli ok | yalniz anahtar |
| CIKAN | `Result<T>` + `ErrorCode` (`Invalid`/`Conflict`) | cikti | istisna sinir disina cikmaz |
| CIKAN | domain event (`ProjectCreated`, `TaskStarted`, ...) | cikti | **commit sonrasi** l2 yayar |
| YASAK | ekran -> entity (`l7 -> l1`) | yasak | ekran daima l2 DTO'suna baglanir |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Sonuc |
|---|---|---|---|
| 1 | `l1 -> l2, l4, l5, l6, l7` | ADR-028 §2.1 matrisi | CRITICAL (R3) |
| 2 | `l1 -> l3` | ADR-028 matrisi `X` | **DOĞRULAMA GEREKLİ** — kaynak şartname `IClock` icin izinli der; matris `X` yazar |
| 3 | `l2 -> l4` | ADR-028 §2.1 | port l0'da, uygulama l4'te |
| 4 | `l4 -> l2` | ADR-028 §2.3/2 | sira degil kenar |
| 5 | `l3 -> l1, l2, l4..l7` | ADR-028 §2.1 | l3 yalniz l0'a bakar |
| 6 | `System.Data` / Dapper / EF / DevExpress -> l1 | ADR-005 + G2/G3 | `.csproj` = 0 satir |

---

## §4 N Kodlari ve Projeler

| N | Alt katman | Klasor | Proje |
|---|---|---|---|
| N06 | entities | `src/VersaCoder.Domain/Entities/` | `VersaCoder.Domain` |
| N07 | value-objects | `src/VersaCoder.Domain/ValueObjects/` | `VersaCoder.Domain` |
| N08 | aggregates | `src/VersaCoder.Domain/Aggregates/` | `VersaCoder.Domain` |
| N09 | domain-events | `src/VersaCoder.Domain/Events/` | `VersaCoder.Domain` |
| N10 | invariants | `src/VersaCoder.Domain/Common/` | `VersaCoder.Domain` |
| N11 | enums | `src/VersaCoder.Domain/Enums/` | `VersaCoder.Domain` |
| N46 | makro l1 ozeti | bu klasor | katman ozeti (proje degil) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | l0 sozlesmeleri (`Result<T>`, `IDomainEvent`, `IRepository<,>`) | l1 -> l0 | **izinli** | matriste OK |
| 2 | l2, l4, l5, l6 -> l1 (entity + port genisletmesi) | ust -> l1 | **izinli** | `ProjectReference` |
| 3 | `System.Data.*`, Dapper, `EntityFramework*` | l1 -> paket | **yasak** | `.csproj` = 0 satir |
| 4 | DevExpress / `Form` / `Control` | l1 -> l7 | **yasak** | G3 |
| 5 | `DateTime.Now`, `File.*`, `Console.*`, `Environment.*` | l1 -> BCL yan etki | **yasak** | Arch taramasi = 0 |

---

## §6 Test Stratejisi

| Test turu | Ne kapsar | Hedef |
|---|---|---|
| Unit | `TaskStateMachine` gecis matrisi (5 x 5 = 25) | **%100** |
| Unit | her invariant (I1..I7): kirilma yolu + temiz yol | **7/7** |
| Unit | VO esitlik + sinir degerleri (6 VO) | **%100** |
| Arch | `VersaCoder.Domain -> l2/l4/l5/l6/l7` + `EntityFramework*` = 0 | **0** |
| Arch | `DateTime.Now` / `File.*` / handler'da invariant tekrari = 0 | **0** |

---

## §7 Sinir Sorulari (layer-boundary-check)

| # | Soru | Kanit |
|---|---|---|
| 3 | l1 yalniz l0'a bagimli mi (Infrastructure/UI using'i 0)? | `Select-String ...Domain... -Pattern 'using .*Infrastructure|using .*UI'` = 0 satir |
| 12 | `DateTime.Now` / `File.*` / `Environment.*` saf katmanda yok mu? | l1 uzerinde tarama = 0 satir |
| 1 | NetArchTest 0 ihlal mi? | `dotnet test --filter Category=Architecture` -> `0 failure` |
| 8 | Dairesel bag var mi? | `NoCircularDependencies` -> `0 failure` |

---

## §8 Kurallar

- Invariant **entity icinde** dogrulanir; handler'da tekrar dogrulama yazilmaz (tek gercek kaynagi).
- Durum degisikligi **yalniz** `TaskStateMachine` uzerinden olur; `task.State = X` dogrudan yazim `private set` ile derleme denetiminde engellenir.
- Zaman `DateTime.Now` ile alinmaz; l1'de zaman kaynagi §3/2 satiri (`l1 -> l3`) **DOĞRULAMA GEREKLİ** oldugu icin port karari ADR-028'e baglidir.
- Olay adi gecmis zamandir (`TaskStarted`), olay **emir degildir**.
- Bu katmana `EntityFramework*`, `System.Data` ya da `DevExpress.*` **paket** eklemek G2/G3 ihlalidir (`.csproj` 0 satir).

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item l1-domain\index.md).Length` | olcum |
| 3 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 + matris | dogrulandi |
| 4 | `l1 -> l3` celiskisi | §3/2 satiri `DOĞRULAMA GEREKLİ` olarak isaretli | isaretlendi |
| 5 | Madde listeleri <= 5 | §8 = 5 madde | sayim |
