---
title: "C# / .NET Knowledge Index - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: reference
scope: "knowledge/csharp altindaki 29 teknik dosyanin kayit defteri: sorumluluk, izinli link hedefleri ve konuya gore yonlendirme"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# C# / .NET Knowledge Index (csharp/)

**Zorunlu Baglantilar:** [[knowledge/index|knowledge-index]] · [[claude|CLAUDE]] · [[governance/keys|keys]] · [[.rules/coding-standards|coding-standards]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]]
**Küme + Hub:** [[knowledge/csharp/index|csharp index]] · [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]]

> **Tek cumlelik sorumluluk:** *Bu vault'ta hangi C#/.NET ozelligi iki bacakta nasil davranir, o cevap hangi dosyada* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu dizin `.ai/knowledge/csharp/` altindaki **29 dosyanin** kayit defteridir. Tamami **iki bacak** (`net48` + `net10.0-windows`) boyunca cevap verir: hangi ozellik hangi bacakta derlenir, hangi API net48'de paket ister, `#if` neden yalniz platform farkiyla sinirlidir (G6). Oncelik sirasi: **anayasa (G1..G12) > `.rules/coding-standards.md` > bu dizindeki anlatim** — celişirse kural kazanir, dosya `DOĞRULAMA GEREKLI` ile isaretlenir. Karar (ne yapilacagi) burada degil, [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] ve [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] altindadir.

## §1 Dosya kayit defteri (29)

| # | Dosya | Sorumluluk | Kaynak turu |
|---|---|---|---|
| 1 | [[knowledge/csharp/async-await-basics|`async-await-basics.md`]] | async tum yol, `ConfigureAwait` secimi | csharp vault'u |
| 2 | [[knowledge/csharp/cancellation-token-policy|`cancellation-token-policy.md`]] | token imzasi, iptal zinciri, linkli timeout | csharp vault'u |
| 3 | [[knowledge/csharp/channel-queue-patterns|`channel-queue-patterns.md`]] | `Channel<T>` kuyruk, Complete/Completion | csharp vault'u |
| 4 | [[knowledge/csharp/collections-choice|`collections-choice.md`]] | topluluk secimi, kapasite, disari gorunurluk | csharp vault'u |
| 5 | [[knowledge/csharp/exception-handling-patterns|`exception-handling-patterns.md`]] | catch sirasi, `when`, `throw;`, katman siniri | csharp vault'u |
| 6 | [[knowledge/csharp/generics-constraints|`generics-constraints.md`]] | `where` kisitlari ve bacak dagilimi | csharp vault'u |
| 7 | `index.md` | bu kayit defteri | csharp vault'u |
| 8 | [[knowledge/csharp/interfaces-vs-abstract|`interfaces-vs-abstract.md`]] | arayuz mu soyut sinif mi, DIM yasagi | csharp vault'u |
| 9 | [[knowledge/csharp/language-versions-net48-net10|`language-versions-net48-net10.md`]] | C# 7.3 vs C# 14, ozellik tablosu | csharp vault'u |
| 10 | [[knowledge/csharp/linq-performance-pitfalls|`linq-performance-pitfalls.md`]] | erteleme, cift sayim, kapali ifade | csharp vault'u |
| 11 | [[knowledge/csharp/multi-targeting-props|`multi-targeting-props.md`]] | `Directory.Build.props/.targets`, TFM kosulu, CPM | csharp vault'u |
| 12 | [[knowledge/csharp/nullable-reference-types|`nullable-reference-types.md`]] | NRT gecisi dosya bazli mi, `#nullable` | csharp vault'u |
| 13 | [[knowledge/csharp/pattern-matching|`pattern-matching.md`]] | kalip turleri ve surum haritasi | csharp vault'u |
| 14 | [[knowledge/csharp/records-and-immutability|`records-and-immutability.md`]] | `record`, value-equality, `with` | csharp vault'u |
| 15 | [[knowledge/csharp/result-type-design|`result-type-design.md`]] | `Result`/`ErrorCode` sozlesmesi onizlemesi | csharp vault'u |
| 16 | [[knowledge/csharp/serialization-choices|`serialization-choices.md`]] | JSON/XML secimi, `BinaryFormatter` yasagi | csharp vault'u |
| 17 | [[knowledge/csharp/span-interop-net10|`span-interop-net10.md`]] | `Span`/`Memory`, P/Invoke marshalling | csharp vault'u |
| 18 | [[knowledge/csharp/task-vs-value-task|`task-vs-value-task.md`]] | `Task` vs `ValueTask`, tekrar await yasagi | csharp vault'u |
| 19 | [[knowledge/csharp/threading-locking|`threading-locking.md`]] | kilit secimi, olu kilit, kilit sirasi | csharp vault'u |
| 20 | [[knowledge/csharp/solid-principles-applications|`solid-principles-applications.md`]] | SOL-01..SOL-08'in kod karsiligi | `.rules/solid-rules.md` |
| 21 | [[knowledge/csharp/retry-and-timeout-policy|`retry-and-timeout-policy.md`]] | ustel geri cekilme, timeout butcesi | `.rules/error-handling-rules.md` |
| 22 | [[knowledge/csharp/testing-determinism|`testing-determinism.md`]] | saat/rastgele/sebeke determinizmi (TS-04..06) | `.rules/testing-rules.md` |
| 23 | [[knowledge/csharp/assertion-quality|`assertion-quality.md`]] | iddia cesitliligi ve hata yolu testleri (TS-07/09) | `.rules/testing-rules.md` |
| 24 | [[knowledge/csharp/guard-clauses-and-method-shape|`guard-clauses-and-method-shape.md`]] | guard clause, metot/sinif boyutu (CS-08/09) | `.rules/coding-standards.md` |
| 25 | [[knowledge/csharp/naming-and-comments|`naming-and-comments.md`]] | isimlendirme, yorum politikasi (CS-01/07/10/16) | `.rules/coding-standards.md` |
| 26 | [[knowledge/csharp/resource-disposal-patterns|`resource-disposal-patterns.md`]] | `using`/`await using`, kaynak sahipligi (CS-15) | `.rules/coding-standards.md` |
| 27 | [[knowledge/csharp/constants-and-magic-values|`constants-and-magic-values.md`]] | sabit, enum, yapilandirma (CS-14) | `.rules/coding-standards.md` |
| 28 | [[knowledge/csharp/layer-dependency-direction|`layer-dependency-direction.md`]] | katman yonu ve NetArchTest denetimi (SOL-06) | `.rules/solid-rules.md` + ADR-028 |
| 29 | [[knowledge/csharp/async-ui-state-flows|`async-ui-state-flows.md`]] | busy durum, fire-and-forget, UI akisi (A7/A8) | `.rules/async-rules.md` |

## §2 Konuya gore yonlendirme

| Ihtiyaç | Gidilecek dosya |
|---|---|
| Bu ozellik `net48`'de var mi | `language-versions-net48-net10.md` · `generics-constraints.md` |
| async / iptal nasil | `async-await-basics.md` · `cancellation-token-policy.md` |
| `#if` ne zaman yazabilirim | `coding-standards` CS-12 + `multi-targeting-props.md` |
| Hata sozlesmesi `Result` mi | `result-type-design.md` + `error-handling-rules` |
| Derleme hatasi / TFM kosulu | `multi-targeting-props.md` |
| Kilit mi kuyruk mu | `threading-locking.md` · `channel-queue-patterns.md` |
| Test neden kararsiz | `testing-determinism.md` · `assertion-quality.md` |

## §3 Izinli link hedefleri

| Hedef | Durum |
|---|---|
| `../index.md` (knowledge indeksi) · `../CLAUDE.md` · `../keys.md` · `../VISION.md` | Var -> link verilir |
| `../.rules/coding-standards.md` · `solid-rules.md` · `async-rules.md` | Var -> link verilir |
| `../.rules/error-handling-rules.md` · `testing-rules.md` | Var -> link verilir |
| `../.decisions/adr-index.md` · `ADR-028` · `ADR-005` · `ADR-001` (accepted) | Var -> link verilir |
| §1'de adi gecen 28 kardes dosya | Var -> adiyla yazilir |
| Bunun disindaki hedef (`spec/*`, `playbooks/*`, `architecture/l*`) | **Yasak** -> link kirilir (GATE-004a) |

## §4 Yasaklar

1. §1'de kaydi olmayan dosya eklemek (once satir, sonra icerik).
2. §3'te yasak yazan hedefe link vermek (liste bilincli olarak dar).
3. Kardes dosya adini yanlis yazmak (ad = link).
4. Karsilastirmayi tek bacakta yapıp "calisiyor" demek (G6) — her iddia iki bacak build ciktisi ister.

## §5 Kullanim notu

| Durum | Yapilacak |
|---|---|
| Dosya yeni eklendi | bu tabloya 1 satir, sonra icerik |
| Celiski .rules ile | `.rules` kazanir, dosya `DOĞRULAMA GEREKLI` |
| Kanitsiz "bitti" | G5 ihlali -> RED |
| Karar gerektiren konu | [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] / [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | 29 dosya sayimi | `Get-ChildItem C:\versacoder\.ai\knowledge\csharp\*.md` = 29 (2026-09-29) |
| 2 | 19 dosyanin kaynagi | `C:\www\versacoder\versacoder\.ai\knowledge\csharp\` = 19 MD (ayni tarih olcumu) |
| 3 | 10 dosyanin konu kaynagi | `.rules/coding-standards.md`, `solid-rules.md`, `async-rules.md`, `error-handling-rules.md`, `testing-rules.md` |
| 4 | ikili bacak baglami | `.decisions/accepted/ADR-001-dual-target-net48-net10.md` |
| 5 | katman/protokol baglami | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
