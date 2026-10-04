---
title: "Dugum Envanteri - 52 Katalog Dugumu"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N01-N52 dugumlerinin katman, proje, src klasoru ve sorumluluk sutunlari (ADR-028 2.1 + 2.3/4)"
---

# Dugum Envanteri - 52 Katalog Dugumu

**Zorunlu Baglantilar:** [catalog index](index.md) · [layer-matrix](layer-matrix.md) · [project-map](project-map.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *Hangi N kodu hangi katmanda, hangi projede ve hangi klasorde yasar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**52 dugum = 44 alt katman (N01-N44, klasor/namespace duzeyi) + 8 makro ozet (N45-N52).** Her dugum bir `.csproj` degildir; proje sutunu onun **hangi tek proje icinde** yasadigini soyler. Klasor sutunu ADR-028 §2.3/4 kaliptir (`N33 -> src/VersaCoder.Protocol/ToolSchema/`); `src/` dizini henuz olusmadigi icin tum klasorler **DOĞRULAMA GEREKLI**dir.

```text
+---------------------------------------------------+
| toplam     : 52 dugum (44 alt + 8 makro)          |
| makro      : N45..N52 = l0..l7 ozet dosyalari     |
| bu batch    : 26 dosya (N01-N23, N45-N47)          |
| authority  : derived (klasor listesi plan)        |
+---------------------------------------------------+
```

| Olcut | Deger |
|---|---|
| Alt katman dugumu | 44 (N01-N44) |
| Makro ozet dugumu | 8 (N45-N52) |
| Makro katman | 8 (l0-l7) |
| Kayit hedefi | 120 (alt) / 20 (makro) |
| authority | derived + needs-validation |

---

## §1 Proje Legendesi

| # | Katman | Proje (.csproj) | Alt N | Makro |
|---|---|---|---|---|
| 1 | l0 Abstractions | `VersaCoder.Abstractions` | N01-N05 | N45 |
| 2 | l1 Domain | `VersaCoder.Domain` | N06-N11 | N46 |
| 3 | l2 Application | `VersaCoder.Application` | N12-N18 | N47 |
| 4 | l3 Crosscutting | `VersaCoder.Crosscutting` | N19-N24 | N48 |
| 5 | l4 Infrastructure | `VersaCoder.Infrastructure` | N25-N32 | N49 |
| 6 | l5 Protocol | `VersaCoder.Protocol` | N33-N37 | N50 |
| 7 | l6 Host | `VersaCoder.Host` | N38-N40 | N51 |
| 8 | l7 UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | N41-N44 | N52 |

---

## §2 Dugum Tablosu (52 satir)

| N | Slug | Katman | Proje | src/ klasoru | Sorumluluk |
|---|---|---|---|---|---|
| N01 | ports-core | l0 | Abstractions | Ports/ | port sozlesmeleri |
| N02 | result-error | l0 | Abstractions | Results/ | result ve error tipi |
| N03 | events-abi | l0 | Abstractions | Events/ | event sozlesmesi |
| N04 | extensions | l0 | Abstractions | Extensions/ | extension methodlar |
| N05 | constants | l0 | Abstractions | Constants/ | sabitler |
| N45 | macro-l0 | l0 | Abstractions | - | l0 makro ozet |
| N06 | entities | l1 | Domain | Entities/ | entity tanimlari |
| N07 | value-objects | l1 | Domain | ValueObjects/ | value object'ler |
| N08 | aggregates | l1 | Domain | Aggregates/ | aggregate kokleri |
| N09 | domain-events | l1 | Domain | Events/ | domain event'ler |
| N10 | invariants | l1 | Domain | Invariants/ | invariant kurallari |
| N11 | enums | l1 | Domain | Enums/ | enum tanimlari |
| N46 | macro-l1 | l1 | Domain | - | l1 makro ozet |
| N12 | commands | l2 | Application | Commands/ | command tanimlari |
| N13 | queries | l2 | Application | Queries/ | query tanimlari |
| N14 | handlers | l2 | Application | Handlers/ | handler'lar |
| N15 | dto-mappers | l2 | Application | Mappers/ | DTO ve mapper'lar |
| N16 | validators | l2 | Application | Validators/ | validator'lar |
| N17 | use-cases | l2 | Application | UseCases/ | use case akisleri |
| N18 | scheduling | l2 | Application | Scheduling/ | zamanlayici |
| N47 | macro-l2 | l2 | Application | - | l2 makro ozet |
| N19 | logging | l3 | Crosscutting | Logging/ | loglama |
| N20 | caching | l3 | Crosscutting | Caching/ | onbellek |
| N21 | clock-time | l3 | Crosscutting | Clock/ | saat ve zaman |
| N22 | crypto-sec | l3 | Crosscutting | Crypto/ | sifreleme ve guvenlik |
| N23 | config-opts | l3 | Crosscutting | Config/ | yapilandirma ve secenekler |
| N24 | resilience | l3 | Crosscutting | Resilience/ | dayaniklilik |
| N48 | macro-l3 | l3 | Crosscutting | - | l3 makro ozet |
| N25 | ado-dapper | l4 | Infrastructure | Data/ | ADO ve Dapper |
| N26 | sqlite-provider | l4 | Infrastructure | Data/Sqlite/ | SQLite saglayici |
| N27 | sqlserver-provider | l4 | Infrastructure | Data/SqlServer/ | SQL Server saglayici |
| N28 | migrations | l4 | Infrastructure | Migrations/ | goc islemleri |
| N29 | fs-tools | l4 | Infrastructure | FileSystem/ | dosya araclari |
| N30 | git-adapter | l4 | Infrastructure | Git/ | git adaptoru |
| N31 | llm-http | l4 | Infrastructure | Llm/ | LLM HTTP istemcisi |
| N32 | tool-runner | l4 | Infrastructure | Tools/ | arac calistirici |
| N49 | macro-l4 | l4 | Infrastructure | - | l4 makro ozet |
| N33 | tool-schema | l5 | Protocol | ToolSchema/ | arac semasi |
| N34 | messages | l5 | Protocol | Messages/ | mesaj tipleri |
| N35 | session-transcript | l5 | Protocol | Transcript/ | oturum dokumu |
| N36 | loop-policy | l5 | Protocol | Loop/ | dongu politikasi |
| N37 | versioning-errors | l5 | Protocol | Versioning/ | surum ve hata kodlari |
| N50 | macro-l5 | l5 | Protocol | - | l5 makro ozet |
| N38 | bootstrap-pipeline | l6 | Host | Bootstrap/ | baslatma hatti |
| N39 | di-registration | l6 | Host | DependencyInjection/ | DI kaydi |
| N40 | config-bridges | l6 | Host | Bridges/ | config koprusleri |
| N51 | macro-l6 | l6 | Host | - | l6 makro ozet |
| N41 | shell-ribbon | l7 | Desktop | Shell/ | kabuk ve ribbon |
| N42 | panels-explorer | l7 | Desktop | Explorer/ | panel ve explorer |
| N43 | editor-ai-chat | l7 | Desktop | Editor/ | editor ve AI chat |
| N44 | settings-cli | l7 | Desktop + Cli | Settings/ + Cli/ | ayarlar ve CLI |
| N52 | macro-l7 | l7 | Desktop | - | l7 makro ozet |

**Proje sütunu kisadır:** tam `.csproj` adlari §1 tablosundadir; `src/` altindaki gercek yol `src/<Proje>/<klasoru>` bicimindedir.

---

## §3 Dosya Adi Kalibi

| # | Kalip | Ornek |
|---|---|---|
| 1 | alt katman | `N01-ports-core.md` |
| 2 | makro ozet | `N45-macro-l0.md` |

---

## §4 Klasor Kaniti (gercek proje yollari)

```powershell
# ADR-028 2.3/4 SSOT ornegi; dizin yoksa False -> DOGRULAMA GEREKLI
Test-Path 'C:\versacoder\src\VersaCoder.Protocol\ToolSchema'
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Ports'
```

Genis kalip `src/**/Domain/**` sahte PASS uretir; tarama yolu gercek proje adini icerir (ADR-028 2.3/5).

---

## §5 Authority ve Durum

| # | Alan | Deger | Gerekce |
|---|---|---|---|
| 1 | authority | derived | klasor listesi `src/` yokken dogrulanamaz |
| 2 | status | needs-validation | kanit komutu False doner |
| 3 | N araliklari | dogrulanmis | ADR-028 §2.1 |

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Dugum satiri | 52 | §2 |
| 2 | Makro satiri | 8 | N45-N52 |
| 3 | Kayit satiri ornegi | node dosyalarinda 6 (makro 4) | planli |
| 4 | Boyut | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §7 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu envantere girmez; tek iz `reports/k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- `Tier-DLL` gibi turetilmis sutun yoktur; sayim birimi yalniz `| N##-### |` kayit satiridir (ADR-013 §2.2).
- Proje ve TFM detayi [project-map](project-map.md) dosyasindadir.

---

## §8 Ilgili Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [catalog index](index.md) | batch durumu + N <-> klasor eslemesi |
| 2 | [layer-matrix](layer-matrix.md) | 8x8 yon matrisi |
| 3 | [project-map](project-map.md) | 9 uretim + 1-2 test proje |
| 4 | [mimari index](../index.md) | 8 makro katman ozeti |
| 5 | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | 52 dugum / 9 proje baz karari |

---

## §9 Kanit

```powershell
# 1) dugum satiri - beklenen: 52
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\node-inventory.md' -Pattern '^\| N\d{2} \|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\node-inventory.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Protocol\ToolSchema')
```

`src/` dizini yokken 3. komut `False` doner -> dosya `needs-validation` kalir; `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` ile bozuk link 0 dogrulanir.
