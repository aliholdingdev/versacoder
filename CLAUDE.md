---
title: "VERSA CODER — Kök Kural Dosyası"
type: constitution
category: root
status: active
version: 2.0.0
updated: 2026-10-05
authority: SSOT
scope: "Depo kökü CLAUDE.md: rol, vault bağları, l0..l7 katman özeti, G1..G12 tek satır özet, ne yap/ne yapma, disk durumu, bütçe ve doğrulama - kural metni değil, bağlantı"
---

# VERSA CODER — Kök Kural Dosyası

**Zorunlu Bağlantılar:** [index](.ai/index.md) · [CLAUDE](.ai/CLAUDE.md) · [templates](.ai/.templates/index.md) · [architecture](.ai/architecture/index.md) · [WORKFLOW](.ai/WORKFLOW.md) · [keys](.ai/keys.md)

## B2 Tek Cümlelik Sorumluluk

Bu depoda VersaCoder ürününün kodunu ve kanıtını üretirim; kuralı burada tekrarlamam,
yalnız bağlantısını veririm.

## B3 Rol Tanımı

- **Görev:** Bu depoda `.ai/` vault'unun tek gerçek kaynağı olurum; okuma sırasını, katman
  adlarını ve kapı kuralını söylerim.
- **Sınır:** Kural metnini `.ai/.rules/`, kararı `.ai/.decisions/`, şablonu `.ai/.templates/`
  dışına **taşımam**; yalnız bağlantısını veririm (G12).
- **Çıktı:** 5 alanlı ASCII ajan kutusu (`BASLIK / KAPSAM / KARAR / RİSK / KANIT`);
  `KANIT` satırında komut çıktısı ya da satır numarası vardır.
- **Kapı:** `GATE-003` kullanıcı onayı alınmadan `src/**` altına tek satır `.cs` yazılmaz (G8).
- **Bütçe:** Oturum bütçesi 50.000 token; bu dosyanın payı ≤ %3 (~1.500 token).

## B4 Vault Bağları ve Okuma Sırası

| Sıra | Okunan | Ne verir | Tekrar eder mi |
|---|---|---|---|
| 1 | `CLAUDE.md` (bu dosya) | rol, vault bağları, katman özeti, yasaklar | Hayır — yalnız bağlantı |
| 2 | `.ai/.templates/index.md` | 44 şablon envanteri + §3.7 kalite sözleşmesi | Hayır |
| 3 | `.ai/.rules/*.md` (18 MD) | kalıcı kural metni | Hayır |
| 4 | `.ai/architecture/index.md` · `.ai/WORKFLOW.md` | katman gerçeği · kapı gerçeği | Hayır |
| 5 | Görevin kendi şablonu | üretilcek dosyanın iskeleti | Hayır |

## B5 Katman Listesi (ADR-028 §2.1 — 8 makro katman + test, birebir)

| # | Makro | Proje (.csproj) | Tek satır sorumluluk |
|---|---|---|---|
| 1 | `l0` Abstractions | `VersaCoder.Abstractions` | port arayüzleri, `Result<T>`, event sözleşmesi |
| 2 | `l1` Domain | `VersaCoder.Domain` | entity, invariant, domain event |
| 3 | `l2` Application | `VersaCoder.Application` | use case, CQRS handler, orkestrasyon |
| 4 | `l3` Crosscutting | `VersaCoder.Crosscutting` | logging, `IClock`, dosya/yardımcılar |
| 5 | `l4` Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, dosya, git, LLM sağlayıcı |
| 6 | `l5` Protocol | `VersaCoder.Protocol` | mesaj şeması, tool sözleşmeleri |
| 7 | `l6` Host | `VersaCoder.Host` | composition root, DI, `appsettings*.json` |
| 8 | `l7` UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | RibbonForm sunumu / CLI ayrıştırma |
| — | test | `VersaCoder.Tests` (+ `ArchitectureTests`) | kanıt üretir; hedef %90 coverage |

**Yön:** `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l3` yalnız `l0`'a bakar; `l2` ile `l4`
arasında **doğrudan ok YOK** (repository portu `l0`'da, uygulaması `l4`'te). Yukarı bakan
referans ihlaldir. **Test katman değildir** — ayrı projedir (9 üretim + 1-2 test, en fazla 11).

## B6 Kurallar Özeti (metin `.ai/.rules/` ve `.ai/CLAUDE.md` §4.5'tedir)

| ID | Tek satır özet | Bağlantı |
|---|---|---|
| G1 | Harici vault'tan içerik kopyalanamaz; yalnız yapı referansı | [templates](.ai/.templates/index.md) |
| G2 | EF Core yasak — ADO.NET + Dapper, parametreli sorgu | [ADR-005](.ai/.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) |
| G3 | Frontmatter'sız Markdown yasak (8 alan) | [templates](.ai/.templates/index.md) |
| G4 | Üretilen dosyada yer tutucu değişken kalmaz (0 adet) | [templates](.ai/.templates/index.md) |
| G5 | Türkçe zorunlu; mojibake yasak | [templates](.ai/.templates/index.md) |
| G6 | < 480 satırlık şablon reddedilir (tek muaf: `.templates/CLAUDE.md`) | [templates](.ai/.templates/index.md) |
| G7 | Bozuk bağlantı yasak; silme/overwrite kullanıcı onayı ister | [templates](.ai/.templates/index.md) |
| G8 | `GATE-003` onaysız ekran/kod üretilmez | [WORKFLOW](.ai/WORKFLOW.md) |
| G9 | ADR'siz mimari karar yok; kanıtsız sayı yazılmaz | `ADR-001..030` |
| G10 | Concat SQL yasak; `@p1..@pN` zorunlu | `Query-Template.md` |
| G11 | `TreatWarningsAsErrors=true` + %90 coverage | `test-plan-template.md` |
| G12 | Envantere içerik kopyalanamaz (SRP) | [templates](.ai/.templates/index.md) |

## B7 Ne Yap / Ne Yapma

| ✅ Yap | ❌ Yapma (ilk satırda DUR) |
|---|---|
| Bağlantı ver, kuralı **okumaya** git | Kural metnini kök dosyaya kopyala |
| Katmanı `l0..l7` koduyla söyle | Katman adı uydur / numarayı kaydır |
| Olmayana `DOĞRULAMA GEREKLI` yaz | Olmayan dosyayı "şurada" diye göster |
| 6 bağlantıyı tek satırda `·` ile tut | 8+ bağlantı · dağınık navigasyon |
| Tarihi ISO yaz (`YYYY-MM-DD`) | Serbest tarih biçimi |
| DevExpress sürümünü ADR'den al (`26.1`) | "en güncel sürüm" kanıtsız iddiası |
| `@p1..@pN` örneğini göster | String concat SQL örneği ver |
| Ölç, sonra "tamam" de (B10) | Kanıtsız "hizalı/doğru" iddiası |
| Ürün adı `VERSACODER` / VersaCoder | Eski ürün adını hiçbir yerde geçirmek |
| İş bitince `.ai/log.md`'ye tek satır düş | Kök dosyayı her oturumda yeniden yaz |

## B8 Dosya Yolları ve Disk Durumu (ölçüm 2026-10-05)

| # | Yol | Ne için | Var |
|---|---|---|---|
| 1 | `CLAUDE.md` | bu dosyanın kendisi (2050 B → 8,4 KB) | ✅ |
| 2 | `AGENTS.md` | gölge dosya (10–20 satır) | ✅ 99 → 20 satır |
| 3 | `README.md` · `WORKFLOW.md` | tanım · kapı özeti | ✅ 2653 B · 1630 B |
| 4 | `.ai/index.md` | vault navigasyonu (553 MD, 16 klasör) | ✅ 39,1 KB |
| 5 | `.ai/CLAUDE.md` | G1..G12 kural metni | ✅ 36 KB |
| 6 | `.ai/.templates/index.md` | şablon envanteri (38 MD, 10 alt klasör) | ✅ |
| 7 | `.ai/.rules/` | 18 MD kural metni | ✅ 18 MD |
| 8 | `.ai/.decisions/` | ADR-001..030 (0 boşluk) + `adr-index.md` | ✅ 32 MD |
| 9 | `.ai/architecture/` | katman spec + catalog | ✅ 129 MD |
| 10 | `.ai/checklists/` · `.ai/.agents/` | gate listesi · 7 ajan profili | ✅ 32 MD · 7 MD |
| 11 | `.ai/scripts/` | 14 PS1 ölçüm betiği | ✅ 14 PS1 |
| 12 | `.ai/keys.md` · `.ai/log.md` · `.ai/WORKFLOW.md` | anahtar · günlük · kapı | ✅ |
| 13 | `src/` (9 VersaCoder projesi) | üretim kodu | ❌ **YOK** |
| 14 | `tests/` · `.sln` | test + çözüm | ❌ **YOK** |
| 15 | `global.json` · `Directory.Packages.props` | SDK + CPM kilidi | ❌ **YOK** |

## B9 Bağlam Bütçesi ve Güncelleme Tetikleri

- **Bütçe:** 50.000 token/oturum · bu dosya payı ≤ %3 (> 2.000 token → RED).
- **Tetik:** kural / katman / rol değişince → `version` minor + `updated` + `.ai/log.md` kaydı.
- **Yalnız düzeltme:** bağlantı ya da imleç → `version` patch + tek satır log.

## B10 Doğrulama ve Quality Report

```powershell
# 1) Bağlantı — bu dosyadaki 6 hedefin disk durumu (kırık = 0 olmalı)
$b='C:\versacoder'; $doc="$b\CLAUDE.md"
$t = Select-String -LiteralPath $doc -Pattern '\]\(([^)#]+)\)' -AllMatches |
     ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$k = $t | Where-Object { -not (Test-Path (Join-Path $b $_)) }
'Baglanti {0} · kirik {1} · satir {2} (120-220)' -f $t.Count,@($k).Count,(Get-Content $doc).Count
# 2) Ölçüm — 4 sayac
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1
```

```markdown
## Quality Report — kök CLAUDE.md (2026-10-05)
| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| CLAUDE.md / AGENTS.md | 164 satır / 17 satır | 120–220 · 10–20 | ONAY |
| Bağlantı / kırık | 6 / 0 | tam 6 · kırık 0 | ONAY |
| B1..B10 blok · l0..l7 (9 satır) · G1..G12 | 10/10 · 9/9 · 12/12 | 10 · 9 · 12 | ONAY |
| Vault sayacı | 553 MD · BOZUK 0 · MOJIBAKE 0 | 0 · 0 | ONAY |
```
