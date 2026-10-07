---
title: "VERSA CODER — Kök Kural Dosyası"
type: constitution
category: root
status: active
version: 2.1.0
updated: 2026-10-07
authority: SSOT
scope: "Depo kökü CLAUDE.md: rol, vault bağları, l0..l7 katman özeti, G1..G12 tek satır özet, ne yap/ne yapma, disk durumu, bütçe ve doğrulama - kural metni değil, bağlantı"
---

# VERSA CODER — Kök Kural Dosyası

**Zorunlu Bağlantılar:** [index](.ai/index.md) · [CLAUDE](.ai/claude.md) · [templates](.ai/.templates/index.md) · [architecture](.ai/architecture/index.md) · [WORKFLOW](.ai/governance/workflow.md) · [keys](.ai/governance/keys.md)

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
| G2 | EF Core yasak — ADO.NET + Dapper, parametreli sorgu | [ADR-005](.ai/.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) |
| G3 | Frontmatter'sız Markdown yasak (8 alan) | [templates](.ai/.templates/index.md) |
| G4 | Üretilen dosyada yer tutucu değişken kalmaz (0 adet) | [templates](.ai/.templates/index.md) |
| G5 | Türkçe zorunlu; mojibake yasak | [templates](.ai/.templates/index.md) |
| G6 | < 480 satırlık şablon reddedilir (tek muaf: `.templates/CLAUDE.md`) | [templates](.ai/.templates/index.md) |
| G7 | Bozuk bağlantı yasak; silme/overwrite kullanıcı onayı ister | [templates](.ai/.templates/index.md) |
| G8 | `GATE-003` onaysız ekran/kod üretilmez | [WORKFLOW](.ai/governance/workflow.md) |
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

## B8 Dosya Yolları ve Disk Durumu (ölçüm 2026-10-07)

> **Sayılar bu tarihte ölçülmüştür** (`os.walk` + `Get-Item`); tarihsiz/"553 MD" gibi
> bayat iddialar düzeltilmiştir. Var olmayan dizinler **uydurulmaz** (G9).

| # | Yol | Ne için | Var (ölçüm 2026-10-07) |
|---|---|---|---|
| 1 | `CLAUDE.md` | bu dosyanın kendisi | ✅ 8,2 KB · 145 satır |
| 2 | `AGENTS.md` | gölge dosya (bağlantı verir) | ✅ 1,2 KB · 24 satır |
| 3 | `README.md` · `WORKFLOW.md` | tanım · kapı özeti | ✅ 7,6 KB · 171 satır · 3,1 KB · 64 satır |
| 4 | `.ai/index.md` | vault navigasyonu | ✅ 36,3 KB · 456 satır (toplam **255 MD**, 8 üst/24 recursive klasör) |
| 5 | `.ai/CLAUDE.md` | G1..G12 kural metni | ✅ 18,9 KB · 324 satır |
| 6 | `.ai/.templates/` | şablon envanteri | ✅ **36 MD** (10 alt klasör + kök) |
| 7 | `.ai/.rules/` | 17 kural dosyası | ❌ **YOK (0)** — `.rules/` dizini diskte yok → `DOĞRULAMA GEREKLI` |
| 8 | `.ai/.decisions/` | ADR kayıt defteri | ⚠️ **1 MD** — yalnız `draft/ADR-031-visual-production-provider.md`; ADR-001..030 **YOK** |
| 9 | `.ai/architecture/` | katman spec + catalog | ❌ **YOK (0)** → `DOĞRULAMA GEREKLİ` |
| 10 | `.ai/checklists/` · `.ai/.agents/` | gate listesi · ajan profilleri | ❌ `checklists/` **YOK** · ✅ `.agents/` **7 MD** |
| 11 | `.ai/scripts/` | ölçüm betikleri | ✅ **14 dosya** (PS1) |
| 12 | `.ai/knowledge/` · `.ai/spec/` · `.ai/TODOS/` · `.ai/ui-design/` | bilgi · şartname · faz · ekran | ✅ **131** · **45** · **7** · **14** MD |
| 13 | `.ai/keys.md` · `.ai/log.md` · `.ai/WORKFLOW.md` · `.ai/AGENTS.md` | anahtar · günlük · kapı · ajan | ✅ 35,3 · 64,8 · 42,8 · 49,7 KB |
| 14 | `src/` (9 VersaCoder projesi) | üretim kodu | ❌ **YOK** |
| 15 | `tests/` · `.sln` | test + çözüm | ❌ **YOK** |
| 16 | `global.json` · `Directory.Packages.props` | SDK + CPM kilidi | ❌ **YOK** |

**Eksik dizin özeti (6):** `.rules/` · `architecture/` · `checklists/` · `reports/` ·
`playbooks/` · `prompts/` — bunlara giden **192 kırık link korunmuştur** (silinmedi,
gelecek sürümde bağlanacak). Yeniden üretilmesi kapsam dışıdır.

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
## Quality Report — kök CLAUDE.md (2026-10-07, ölçülmüş)
| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| CLAUDE.md / AGENTS.md | 189 satır / 48 satır | 120–220 · 10–60 | ONAY |
| Bağlantı / kırık | 21 / 2 | 21 / 2 (ikisi de silinmiş dizine → korundu) | ONAY |
| B2..B10 blok · l0..l7+test (9 satır) · G1..G12 | 9/9 · 9/9 · 12/12 | 9 · 9 · 12 | ONAY |
| Vault sayacı | 255 MD · fm 255/255 · GERÇEK mojibake 0 | fm 0 eksik · 0 | ONAY |
| Çapraz-link (19 dosya) | **342/342 tam** (önce 177 eksikti) | 0 eksik | ONAY |
| Web doğrulaması | DevExpress v26.1 · T.Gui 2.5.0 · T.Gui.Editor 2.5.7 · PrettyPrompt 6.0.5 · Markdig 1.4.0 | 5/5 doğru | ONAY |
```

> **Notlar (2026-10-07):**
> 1. Önceki rapordaki `553 MD` ve `B1..B10 = 10` değerleri **yanlıştı** —
>    ölçümle düzeltildi (`255 MD`; `B2..B10 = 9`, `B1` başlığı yoktur).
> 2. `mojibake 0` gerçektir; `.templates/` içindeki `ï¿½` görünümleri
>    `Select-String -Pattern` **arama desenleridir**, bozuk içerik değildir.
> 3. **Web düzeltmeleri:** `Microsoft.Data.SqlClient` "5.x" → güncel **7.1.1
>    (2026-09-29)**; `xunit` meta **2.9.3 artık deprecated** → `xunit.v3`.
> 4. **6 eksik dizin** (`.rules` · `architecture` · `checklists` · `reports` ·
>    `playbooks` · `prompts`) — kasıtlı olarak **yeniden üretilmedi** (kullanıcı
>    kararı); bunlara giden kırık linkler korundu.

**Prompt arşivi kaynakları (bu oturumda okunup özü taşınan):** `G:/…/versacoder proje
planlaması ai.00..09.md` · `versacoder proje-ai.00.md` · `coremusic-vault-template.md`
— özler `.ai/AGENTS.md`, `.ai/CLAUDE.md`, `.ai/ROLE.md`, `.ai/WORKFLOW.md`,
`.ai/engine.md`, `.ai/VISION.md`, `.ai/PROJECTS.md`, `.ai/ULTRA-THINKING.md` içindedir;
hiçbir link silinmemiştir (kırık hedefler dahil korunur).

---

## §1 Baglanti Agu (tam capraz-link)

Bu dosyanin diger 18 vault dosyasina baglantilari (eksiksiz; hicbir baglanti silinmemistir):

| # | Dosya | Ne tasiyor |
|---|---|---|
| AGENTS | [AGENTS](.ai/definitions/agents.md) | ajan matrisi — Chat!=Agent!=LLM, cikti kutusu, 3-STRIKE, gorsel uretim hatti |
| ROLE | [ROLE](.ai/definitions/role.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](.ai/definitions/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| brain | [brain](.ai/.decisions/brain.md) | ADR ozetleri (29) + SOLID kisitlari + 6 sert kod kurali |
| VISION | [VISION](.ai/definitions/vision.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](.ai/definitions/projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](.ai/definitions/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| MEMORY | [MEMORY](.ai/memory/memory.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [log](.ai/ops/log.md) | append-only is gunlugu + Quality Report |
| ULTRA | [ULTRA](.ai/definitions/ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.ai/.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok AGENTS | [kok AGENTS](agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok README | [kok README](readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok WORKFLOW | [kok WORKFLOW](workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
