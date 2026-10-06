---
title: Şablon Envanteri — VersaCoder .templates/
type: index
category: templates
status: active
version: 1.1.0
updated: 2026-10-06
authority: SSOT
scope: "Hangi sablon var, nerede durur, kim yazar, kac KB, hangi part - sablon kaydi (icerik tekrari yok)"
---

# Şablon Envanteri (Template Inventory)

**Zorunlu Bağlantılar:** [CLAUDE.md](../CLAUDE.md) · [../index.md](../index.md) · [../AGENTS.md](../AGENTS.md) · [../WORKFLOW.md](../WORKFLOW.md) · [../architecture/index.md](../architecture/index.md) · [../.rules/coding-standards.md](../.rules/coding-standards.md)

> ⚠️ **6 bağlantının hedefi diskte YOK** (`.ai/CLAUDE.md` · `.ai/index.md` · `AGENTS.md` ·
> `.ai/WORKFLOW.md` · `.ai/architecture/index.md` · `.ai/.rules/*.md`). Kasıtlıdır: kök `.ai/`
> omurgası **AŞAMA C** hedefi, silinmeyecek ama üretilmeyecek (CF-15). Bunlar dışındaki hiçbir
> bağlantı bu belgede kırık değildir — G7'nin kapsamı yalnız bu satır (§5.3 K6 ile ölçülür).

> **Tek cümlelik sorumluluk:** Bu dosya *hangi şablon var, nerede durur, kim yazar, kaç KB, hangi part* sorusunun tek cevabıdır.
> Şablonun **içeriği** burada tekrarlanmaz; yalnızca **kayıdı** tutulur (SRP, G12).

## İçindekiler

| Bölüm | Alt Konular |
|---|---|
| [§1 Amaç](#1-amaç) | §1.1 Kurallar · §1.2 Disk Kanıtları · §1.3 Türler · §1.4 Senaryolar |
| [§2 Kapsam](#2-kapsam) | §2.1 Dosya Tipi → Şablon Eşlemesi · §2.2 Kapsam Sınırı Testi |
| [§3 Mimari](#3-mimari) | §3.1 Sütunlar · §3.2–3.5 Part A–D · §3.6 Toplamlar · §3.7 Kalite Sözleşmesi · §3.8 Örnek · §3.9 Kalıp · §3.10 Ağacı · §3.11 Bağımlılık |
| [§4 Kurallar](#4-kurallar) | §4.1 Sıra · §4.2 Onay · §4.3 Part Öncelik · §4.4 Versiyonlama · §4.5 G1..G12 |
| [§5 Workflow](#5-workflow) | §5.1 9 Adım + Bütçe · §5.2 Hata Modları · §5.3 6 Komut |
| [§6 Doğrulama](#6-doğrulama) | §6.1 Checklist · §6.2 Red Sinyalleri · §6.3 Geçişi · §6.4 Quality Report · §6.5 Kontroller |
| [§7 Referanslar](#7-referanslar) | §7.1 Tablo · §7.2 Red Team Hataları |

## §1 Amaç

Şablon envanteri **SRP**'dir: bu dosya yalnızca *envanter* tutar. Şablonun iş kuralı, mimari kararı veya kodu **burada barınamaz** — bunlar kendi şablonuna aittir.

| # | SRP Gerekçesi | Bozulursa |
|---|---|---|
| 1 | İçerik tekrarı iki gerçek kaynağı doğurur (SSUT ihlali) | Şablon değişir, envanter eskisi kalır → yanlış üretim |
| 2 | Envanter tek satır = tek denetim noktası | 44 şablonun 16 maddelik checklist'i ayrı ayrı sarmalanır |
| 3 | Ölçüm tek yerden yapılır | `480–780 satır / 26–46 KB` hedefi tutmaz |
| 4 | Part geçişleri tek akıştan yönetilir | A tamamlanmadan C başlar, sözleşme atlanır |
| 5 | `authority: SSOT` anlamı kalmaz | ADR, README ve şablonlar birbirini çeliştirir |

### §1.1 Envanter Kuralları

1. **Tek dosya, tek gerçek.** Şablon listesi yalnız burada yaşar; `AGENTS.md`, `WORKFLOW.md`, kök `index.md` tekrar **yazmaz**, yalnız [./index.md](./index.md)'e **bağlantı verir**.
2. **Satır = şablon.** 44 şablonun her biri tam 1 satır kaplar; 2 satır = mükerrer, 0 satır = kayıp.
3. **Durum üç değer:** `YAZILDI` · `PLANLI` · `MEVCUT`. `WIP`/`~` yasak; iş bitince tek geçişte `YAZILDI`.
4. **Yol kökten yazılır:** `.ai/.templates/…`. Kısaltılmış yol yasaktır.
5. **Tahmini KB, yazıldıktan sonra ölçülen KB ile güncellenir** (§6.5 K5, ±%20).
6. **Part yalnız ileri sıralıdır:** A → B → C → D (§6.3).
7. **Şablon içeriği buraya kopyalanmaz**; uzun bölüm kendi dosyasına taşınır.
8. **Her satırda sorumlu rol vardır**; boş `Sorumlu` = red (§6.2 R3).
9. **Bu dosya envanterin 1 numaralı satırıdır** (§6.5); her değişiklikten sonra Quality Report (§6.4) `log.md`'ye düşülür.

### §1.2 Disk Kanıtları

`.ai/.templates/` altındaki **12 katalog satırı** — 6'sı diskte mevcut, 6'sı planlı (2026-09-25 disk ölçümü):

Kök: `C:\versacoder\.ai\.templates\`

| # | Klasör | Yol | İçerik | Dosya | KB | Kanıt |
|---|---|---|---|---|---|---|
| 1 | `adr` | `.ai/.templates/adr/` | **7 şablon** (adr-template, adr-architecture, adr-data, adr-devexpress, adr-nygard, adr-security, adr-index) | 7 | 198,8 | ✅ Ölçüm 2026-09-25 |
| 2 | `agents` | `.ai/.templates/agents/` | **2 şablon** (agents-template, subagent-template) | 2 | 61,3 | ✅ Ölçüm 2026-09-25 |
| 3 | `architecture` | `.ai/.templates/architecture/` | **3 şablon** (architecture-template, ascii-diagram-template, layer-spec-template) | 3 | 109,5 | ✅ Ölçüm 2026-09-25 |
| 4 | `backend` | `.ai/.templates/backend/` | **3 şablon** (service-template, handler-template, repository-template) | 3 | 93,6 | ✅ Ölçüm 2026-09-25 |
| 5 | `csharp` | `.ai/.templates/csharp/` | **KLASÖR YOK** - katalogda planlı (§2.1: stok, üretim yok) | 0 | 0,0 | ⬜ Planlı |
| 6 | `documentation` | `.ai/.templates/documentation/` | **6 şablon** (api-doc, claude-md, docs-md, katman-readme, readme, root-md) | 6 | 137,6 | ✅ Ölçüm 2026-10-01 |
| 7 | `frontend` | `.ai/.templates/frontend/` | **1 şablon** (form) | 1 | 32,5 | ✅ Ölçüm 2026-10-01 |
| 8 | `infrastructure` | `.ai/.templates/infrastructure/` | **4 şablon** (migration, dapper-mapping, provider, configuration) | 4 | 125,3 | ✅ Ölçüm 2026-09-25 |
| 9 | `other` | `.ai/.templates/other/` | **KLASÖR YOK** - PART 6'da `cli-command` şablonu gelecek | 0 | 0,0 | ⬜ Planlı (PART 6) |
| 10 | `query` | `.ai/.templates/query/` | **1 şablon** (Query-Template) | 1 | 29,2 | ✅ Ölçüm 2026-09-25 |
| 11 | `testing` | `.ai/.templates/testing/` | **2 şablon** (test-plan, unit-test) | 2 | 57,8 | ✅ Ölçüm 2026-10-01 |
| 12 | `ui-design` | `.ai/.templates/ui-design/` | **1 şablon** (wireframe) | 1 | 33,9 | ✅ Ölçüm 2026-10-01 |

**Ölçüm toplamı:** 10 klasör + kök = **36 MD · 946,8 KB** (ölçüm 2026-10-01) · `csharp`/`other` klasörleri hâlâ yok (planlı, 0 MD).

**Dürüstlük notu:** Tablo 2026-09-25'te diskten okunarak ölçüldü (`Get-ChildItem -Recurse -Filter *.md`); tek kelime kopyalanmadı. `⚠️ VERIFICATION REQUIRED` işaretleri kaldırıldı (12/12 satır ölçüldü). **Klasör varlığı ile katalog satırı ayrıdır:** 6 satır "KLASÖR YOK" = katalogda tanımlı ama henüz üretilmemiş (PART sırasına göre §4.1).

Beyan (hedef disk, **ölçüm 2026-10-06**, `Get-ChildItem -Recurse -File -Force`): `.ai/` = `.templates/` **50 MD** · `prompts/` **2** · `reports/` **1** · **kök 0**. **YOK:** `.rules/` · `.agents/` · `ui-design/` · `index.md` · `CLAUDE.md` · `WORKFLOW.md` · `keys.md` · `log.md` · `.decisions/` · `architecture/` · `checklists/` · `scripts/`. §3'ün 44 satırı **katalog**dur; katalog satırı ≠ disk dosyası (§4.1). Kayıt dışı 5 dosya §7.3 E3–E7. Önceki (2026-10-04) beyanı `kök 14 MD · reports 2 MD` iddia ediyordu, ölçülemedi → düzeltildi (**CF-09**).

### §1.3 Şablon Türleri

| Kod | Tür | Ne Üretir | Part |
|---|---|---|---|
| `T-ENV` | Envanter / İndeks | `index.md`, `adr-index.md` | A |
| `T-DK` | Dizin Kuralı | `.templates/CLAUDE.md`, `claude-md-template.md` | A/D |
| `T-KYT` | Kayıt / Log | `session-log-template.md` → `.ai/log.md` | A |
| `T-ADR` | Karar Kaydı | `.ai/.decisions/ADR-NNN-*.md` (5 varyant) | A |
| `T-AGT` | Agent Tanımı | `.ai/.agents/<rol>.md` | A |
| `T-MIM` | Mimari Belge | Katman spec, bağımlılık kuralı, ASCII diyagram | B |
| `T-VER` | Veri / Altyapı | Migration, Dapper map, sağlayıcı, config | B |
| `T-KOD` | Kod İskeleti | Servis, handler, repository (`*.cs`) | B |
| `T-SRG` | Sorgu | Parametreli SQL + indeks notu | B |
| `T-UX` | Tel Kafes / Ekran | 7 ekran, akış, referans, prompt | C |
| `T-FE` | WinForms Bileşeni | Form, bileşen, ribbon sayfası | C |
| `T-DOK` | Dokümantasyon | README, API doc, Wiki, katman README, dizin kuralı | C/D |
| `T-GUV` | Güvenlik | `.ai/reports/security-audit-*.md` | D |
| `T-TST` | Test | Unit / integration / test planı | D |
| `T-CLI` | CLI Komutu | `VersaCoder.Cli` komutu | D |

### §1.4 Kullanım Senaryoları

| Senaryo | Bu Dosyada Ne Yapar | Süre |
|---|---|---|
| Yeni şablon talebi | §3.x'te satır aç, `Durum = PLANLI` | 1 dk |
| Üretime başlama | §4.1 sırayı, §5.1 9 adımı oku | 3 dk |
| Part geçişi | §6.3 koşullarını denetle | 2 dk |
| Kalite denetimi | §6.1 + §6.5 | 8 dk |

## §2 Kapsam

**İçinde:** `.ai/.templates/` altında yaşayan, `{{PLACEHOLDER}}`'lı yeniden üretilebilir iskeletler ve bu iskeletlerin kayıt dosyaları.
**Dışında:** üretilmiş gerçek dosya (ör. `ADR-003-*.md`), kaynak kod (`src/**/*.cs`), veritabanı şeması, `.rules/` kuralları.

### §2.1 Dosya Tipi → Şablon Eşlemesi

| # | Dosya Tipi | Tipik Hedef | Şablon | Part |
|---|---|---|---|---|
| 1 | Mimari / veri / güvenlik / DevExpress kararı | `.ai/.decisions/ADR-NNN-*.md` | `adr/adr-template.md` + 4 varyant | A |
| 2 | Agent / sub-agent tanımı | `.ai/.agents/<rol>.md` | `agents/{agents,subagent}-template.md` | A |
| 3 | Katman spec + bağımlılık + diyagram | `.ai/architecture/*.md` | `architecture/*-template.md` (4) | B |
| 4 | Application servisi | `src/VersaCoder.Application/**/XService.cs` | `backend/service-template.md` | B |
| 5 | Command/Query handler | `src/VersaCoder.Application/**/XHandler.cs` | `backend/handler-template.md` | B |
| 6 | Repository | `src/VersaCoder.Infrastructure/**/XRepository.cs` | `backend/repository-template.md` | B |
| 7 | Parametreli sorgu | `.ai/query/SELECT-*.md` | `query/Query-Template.md` | B |
| 8 | SQL migration + Dapper map | `db/migrations/V0xx__*.sql` | `infrastructure/{migration,dapper-mapping}-template.md` | B |
| 9 | Sağlayıcı + yapılandırma | `src/VersaCoder.Infrastructure/Providers/*.cs` | `infrastructure/{provider,configuration}-template.md` | B |
| 10 | Tel kafes (7 ekran) | `.ai/ui-design/01-start-page.md … 07-settings.md` | `ui-design/wireframe-template.md` | C |
| 11 | Ekran spec + akış + referans + prompt | `.ai/ui-design/{screen,flow,reference,prompts}/*.md` | `ui-design/*-template.md` (4) | C |
| 12 | WinForms form / bileşen / ribbon | `src/VersaCoder.Desktop/**/*.cs` | `frontend/{form,component,ribbon}-template.md` | C |
| 13 | Genel doküman + proje README | `.ai/documentation/*.md`, `README.md` | `documentation/{docs-md,readme}-template.md` | C |
| 14 | Dizin kuralı + API doc + katman README + Wiki + güvenlik denetimi | `CLAUDE.md`, `.ai/documentation/`, `.ai/reports/` | `documentation/*-template.md` (5) | D |
| 15 | Birim / entegrasyon testi + test planı | `tests/**`, `.ai/reports/test-plan-*.md` | `testing/*-template.md` (3) | D |
| 16 | WinForms form (diğer) + CLI komutu | `src/VersaCoder.Desktop/Forms/`, `src/VersaCoder.Cli/Commands/` | `other/{winforms-form,cli-command}-template.md` | D |

> `csharp/` klasörünün §2.1'de karşılığı yoktur → stok tutulur, **üretim yok** (§6.5 K6).

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** şablonu belirler:

1. **Karar mı** (geri alınabilir, gerekçeli)? → `T-ADR` (A)
2. **Agent/sub-agent tanımı mı**? → `T-AGT` (A)
3. **Belge / indeks / kural dosyası mı** (içerik, kod değil)? → `T-DOK` / `T-ENV` (C/D)
4. **`.cs` iskeleti mi** (servis/handler/repository)? → `T-KOD` (B)
5. **`.sql` / mapping / config mi**? → `T-VER` (B)
6. **Ekran / tel kafes / akış mı** (GATE-003 öncesi)? → `T-UX` (C)
7. **WinForms kontrolü / ribbon sayfası mı**? → `T-FE` (C)
8. **Test dosyası mı**? → `T-TST` (D)
9. **Hiçbiri değilse** → şablon yoktur; talep §5.1 Adım 1'den açılır.

`HAYIR` soruları: CoreMusic/versacoder.ai içeriği mi (G1)? · EF Core içeriyor mu (G2)? · `.rules/` tekrarı mı (yalnız bağlantı)? · gerçek ADR/rapor mu (envantere değil)?

## §3 Mimari

**Toplam 44 şablon · 4 Part · ≈ 582 KB · ≈ 275 dakika.**

### §3.1 Sütun Tanımları

| Sütun | Tanım | Zorunlu Değerler |
|---|---|---|
| `#` | 1–44 benzersiz sıra | sayı |
| `Yol` | `.ai/.templates/` altındaki göreli yol | tam yol |
| `Tür` | §1.3 kodu | `T-ENV … T-CLI` |
| `Hedef Dosya` | Şablondan üretilen gerçek dosya | yol veya `*.cs` kalıbı |
| `Durum` | Üretim durumu | `YAZILDI` \| `PLANLI` \| `MEVCUT` |
| `Tahmini KB` | Tahmin / yazıldıktan sonra ölçüm | bir ondalık (`24,0`) |
| `Part` | Üretim partı | `A` \| `B` \| `C` \| `D` |

### §3.2 Part A — Temel ve Karar (12 şablon)

| # | Şablon | Yol | Tür | Hedef Dosya | Durum | Sorumlu | Tahmini KB | Part |
|---|---|---|---|---|---|---|---|---|
| 1 | `index` | `.ai/.templates/index.md` | `T-ENV` | `.ai/.templates/index.md` | YAZILDI | docs-writer | 38,5 | A |
| 2 | `CLAUDE` | `.ai/.templates/CLAUDE.md` | `T-DK` | `.ai/.templates/CLAUDE.md` | YAZILDI | master-orchestrator | 6,9 | A |
| 3 | `session-log-template` | `.ai/.templates/session-log-template.md` | `T-KYT` | `.ai/log.md` | YAZILDI | docs-writer | 22,2 | A |
| 4 | `adr-template` | `.ai/.templates/adr/adr-template.md` | `T-ADR` | `.ai/.decisions/ADR-NNN-*.md` | YAZILDI | architect | 34,9 | A |
| 5 | `adr-nygard-template` | `.ai/.templates/adr/adr-nygard-template.md` | `T-ADR` | `.ai/.decisions/ADR-NNN-*.md` | YAZILDI | architect | 31,1 | A |
| 6 | `adr-architecture-template` | `.ai/.templates/adr/adr-architecture-template.md` | `T-ADR` | `.ai/.decisions/ADR-NNN-arch-*.md` | YAZILDI | architect | 30,5 | A |
| 7 | `adr-data-template` | `.ai/.templates/adr/adr-data-template.md` | `T-ADR` | `.ai/.decisions/ADR-NNN-data-*.md` | YAZILDI | database-optimizer | 30,8 | A |
| 8 | `adr-security-template` | `.ai/.templates/adr/adr-security-template.md` | `T-ADR` | `.ai/.decisions/ADR-NNN-sec-*.md` | YAZILDI | security | 30,3 | A |
| 9 | `adr-devexpress-template` | `.ai/.templates/adr/adr-devexpress-template.md` | `T-ADR` | `.ai/.decisions/ADR-001…008-*.md` | YAZILDI | architect | 21,4 | A |
| 10 | `adr-index` | `.ai/.templates/adr/adr-index.md` | `T-ENV` | `.ai/.decisions/index.md` | YAZILDI | docs-writer | 19,8 | A |
| 11 | `agents-template` | `.ai/.templates/agents/agents-template.md` | `T-AGT` | `.ai/.agents/<rol>.md` | YAZILDI | master-orchestrator | 31,6 | A |
| 12 | `subagent-template` | `.ai/.templates/agents/subagent-template.md` | `T-AGT` | `.ai/.agents/sub/<rol>.md` | YAZILDI | master-orchestrator | 29,7 | A |
| | **Part A toplam** | **12 şablon** | | **12 YAZILDI · 0 MEVCUT · 0 PLANLI** | | | **≈ 327,7 KB** | **≈ 70 dk** |

> **Part A ≈ 70 dakika** (12 şablon ÷ ort. 5,8 dk + envanter güncelleme 10 dk). A kapanmadan hiçbir Part B şablonuna başlanmaz; istisnası yoktur.

### §3.3 Part B — Mimari ve Veri (12 şablon)

| # | Şablon | Yol | Tür | Hedef Dosya | Durum | Sorumlu | Tahmini KB | Part |
|---|---|---|---|---|---|---|---|---|
| 13 | `architecture-template` | `.ai/.templates/architecture/architecture-template.md` | `T-MIM` | `.ai/architecture/<konu>.md` | YAZILDI | architect | 33,3 | B |
| 14 | `layer-spec-template` | `.ai/.templates/architecture/layer-spec-template.md` | `T-MIM` | `.ai/architecture/layer-<ad>.md` | YAZILDI | architect | 39,1 | B |
| 15 | `dependency-rule-template` | `.ai/.templates/architecture/dependency-rule-template.md` | `T-MIM` | `.ai/architecture/dependency-rule.md` | YAZILDI | architect | 10,0 | B |
| 16 | `ascii-diagram-template` | `.ai/.templates/architecture/ascii-diagram-template.md` | `T-MIM` | `.ai/architecture/diagrams/*.md` | YAZILDI | architect | 37,2 | B |
| 17 | `service-template` | `.ai/.templates/backend/service-template.md` | `T-KOD` | `src/VersaCoder.Application/**/XService.cs` | YAZILDI | developer | 31,9 | B |
| 18 | `handler-template` | `.ai/.templates/backend/handler-template.md` | `T-KOD` | `src/VersaCoder.Application/**/XHandler.cs` | YAZILDI | developer | 30,9 | B |
| 19 | `repository-template` | `.ai/.templates/backend/repository-template.md` | `T-KOD` | `src/VersaCoder.Infrastructure/**/XRepository.cs` | YAZILDI | developer | 30,8 | B |
| 20 | `Query-Template` | `.ai/.templates/query/Query-Template.md` | `T-SRG` | `.ai/query/<islem>.md` | YAZILDI | database-optimizer | 29,2 | B |
| 21 | `migration-template` | `.ai/.templates/infrastructure/migration-template.md` | `T-VER` | `db/migrations/V0xx__*.sql` | YAZILDI | database-optimizer | 29,9 | B |
| 22 | `dapper-mapping-template` | `.ai/.templates/infrastructure/dapper-mapping-template.md` | `T-VER` | `src/VersaCoder.Infrastructure/Dapper/*.cs` | YAZILDI | database-optimizer | 31,9 | B |
| 23 | `provider-template` | `.ai/.templates/infrastructure/provider-template.md` | `T-VER` | `src/VersaCoder.Infrastructure/Providers/*.cs` | YAZILDI | developer | 31,4 | B |
| 24 | `configuration-template` | `.ai/.templates/infrastructure/configuration-template.md` | `T-VER` | `src/VersaCoder.Host/appsettings*.json` | YAZILDI | devops-engineer | 32,1 | B |
| | **Part B toplam** | **12 şablon** | | **12 YAZILDI** | | | **393,9 KB** | **≈ 75 dk** |

> **Part B ≈ 75 dakika** (mimari 4 ≈ 30 dk + backend/query 4 ≈ 25 dk + altyapı 4 ≈ 20 dk). Veri erişimi **ADO.NET + Dapper**, `@p1..@pN` (G2, G10).

### §3.4 Part C — Arayüz ve Anlatım (10 şablon)

| # | Şablon | Yol | Tür | Hedef Dosya | Durum | Sorumlu | Tahmini KB | Part |
|---|---|---|---|---|---|---|---|---|
| 25 | `wireframe-template` | `.ai/.templates/ui-design/wireframe-template.md` | `T-UX` | `.ai/ui-design/01-start-page.md … 07-settings.md` | YAZILDI | ui-designer | 14,0 | C |
| 26 | `screen-spec-template` | `.ai/.templates/ui-design/screen-spec-template.md` | `T-UX` | `.ai/ui-design/screen/*.md` | YAZILDI | ui-designer | 16,0 | C |
| 27 | `flow-template` | `.ai/.templates/ui-design/flow-template.md` | `T-UX` | `.ai/ui-design/flow/*.md` | YAZILDI | ui-designer | 12,0 | C |
| 28 | `reference-template` | `.ai/.templates/ui-design/reference-template.md` | `T-UX` | `.ai/ui-design/reference/*.md` | YAZILDI | ui-designer | 10,0 | C |
| 29 | `prompt-template` | `.ai/.templates/ui-design/prompt-template.md` | `T-UX` | `.ai/ui-design/prompts/*.md` | YAZILDI | ui-designer | 9,0 | C |
| 30 | `form-template` | `.ai/.templates/frontend/form-template.md` | `T-FE` | `src/VersaCoder.Desktop/**/XForm.cs` | YAZILDI | developer | 15,0 | C |
| 31 | `component-template` | `.ai/.templates/frontend/component-template.md` | `T-FE` | `src/VersaCoder.Desktop/Controls/*.cs` | YAZILDI | developer | 14,0 | C |
| 32 | `ribbon-template` | `.ai/.templates/frontend/ribbon-template.md` | `T-FE` | `src/VersaCoder.Desktop/**/XRibbonPage.cs` | YAZILDI | developer | 13,0 | C |
| 33 | `docs-md-template` | `.ai/.templates/documentation/docs-md-template.md` | `T-DOK` | `.ai/documentation/<konu>.md` | YAZILDI | docs-writer | 11,0 | C |
| 34 | `readme-template` | `.ai/.templates/documentation/readme-template.md` | `T-DOK` | `README.md` | YAZILDI | docs-writer | 10,0 | C |
| | **Part C toplam** | **10 şablon** | | **10 PLANLI** | | | **≈ 124,0 KB** | **≈ 65 dk** |

> **Part C ≈ 65 dakika** (ui-design 5 ≈ 30 dk + frontend 3 ≈ 25 dk + documentation 2 ≈ 10 dk).
> **Ön koşul (CF-10 düzeltmesi):** **GATE-003** gerçek *ekran dosyalarını* ve `src/**` kodunu bloklar — **şablon üretimini bloklamaz** (k. CLAUDE G8: "ekran/kod"). Aksi halde döngü kapanmaz (şablon → ekran → onay). `form`/`ribbon` şablonu yazılır; **üretilmiş** ekran dosyası yalnız onaydan sonra çıkar.

### §3.5 Part D — Kanıt ve Uygulama (10 şablon)

| # | Şablon | Yol | Tür | Hedef Dosya | Durum | Sorumlu | Tahmini KB | Part |
|---|---|---|---|---|---|---|---|---|
| 35 | `claude-md-template` | `.ai/.templates/documentation/claude-md-template.md` | `T-DOK` | `CLAUDE.md` / `AGENTS.md` | YAZILDI | master-orchestrator | 12,0 | D |
| 36 | `api-doc-template` | `.ai/.templates/documentation/api-doc-template.md` | `T-DOK` | `.ai/documentation/api/*.md` | YAZILDI | docs-writer | 14,0 | D |
| 37 | `katman-readme-template` | `.ai/.templates/documentation/katman-readme-template.md` | `T-DOK` | `src/VersaCoder.<Katman>/README.md` | YAZILDI | docs-writer | 10,0 | D |
| 38 | `WikiPage-Template` | `.ai/.templates/documentation/WikiPage-Template.md` | `T-DOK` | `.ai/documentation/wiki/*.md` | YAZILDI | docs-writer | 12,0 | D |
| 39 | `security-audit-template` | `.ai/.templates/documentation/security-audit-template.md` | `T-GUV` | `.ai/reports/security-audit-*.md` | YAZILDI | security | 16,0 | D |
| 40 | `unit-test-template` | `.ai/.templates/testing/unit-test-template.md` | `T-TST` | `tests/VersaCoder.Tests/**/*Tests.cs` | YAZILDI | testing | 13,0 | D |
| 41 | `test-plan-template` | `.ai/.templates/testing/test-plan-template.md` | `T-TST` | `.ai/reports/test-plan-*.md` | YAZILDI | testing | 14,0 | D |
| 42 | `integration-test-template` | `.ai/.templates/testing/integration-test-template.md` | `T-TST` | `tests/VersaCoder.IntegrationTests/**` | YAZILDI | testing | 12,0 | D |
| 43 | `winforms-form-template` | `.ai/.templates/other/winforms-form-template.md` | `T-FE` | `src/VersaCoder.Desktop/Forms/*.cs` | YAZILDI | developer | 15,0 | D |
| 44 | `cli-command-template` | `.ai/.templates/other/cli-command-template.md` | `T-CLI` | `src/VersaCoder.Cli/Commands/*.cs` | YAZILDI | developer | 11,0 | D |
| | **Part D toplam** | **10 şablon** | | **10 PLANLI** | | | **≈ 129,0 KB** | **≈ 65 dk** |

> **Part D ≈ 65 dakika** (documentation 5 ≈ 35 dk + testing 3 ≈ 20 dk + other 2 ≈ 10 dk).
> `#43` türü `T-FE` olmasına rağmen `other/` altında yaşadığı için Part D'dedir → **klasör ≠ tür** (§6.5 K4).

### §3.6 Part Toplamları

| Part | Dosya | Toplam KB | Süre | Bitiş Kapısı | Önceki |
|---|---|---|---|---|---|
| A | 12 | **336,9** | ≈ 70 dk | `GATE-001` iskelet onayı | — |
| B | 12 | **393,9** | ≈ 75 dk | `GATE-002` mimari onayı | A |
| C | 10 | **334,8** | ≈ 65 dk | `GATE-003` tel kafes/ekran onayı | B |
| D | 10 | **329,6** | ≈ 65 dk | `GATE-005` implementation-plan | C |
| **Toplam** | **44** | **1.395,2** | **≈ 275 dk** | | |

**Ölçüm notu (2026-10-06):** Değerler envanter satırındaki yolların diskten
`Get-Item .Length / 1KB` ölçümüdür; **44/44 dosya mevcut**. Önceki beyan
(`≈ 582,0` = 175/154/124/129) ölçülemedi → düzeltildi (**CF-16**). §3.8'deki
`#18 handler-template` **örnek satırıdır**, toplamlara dahil edilmez.

### §3.7 Bölüm Başına Kalite Sözleşmesi

**Zorunlu frontmatter alanları (dosyanın ilk 9 satırı):**

| # | Alan | Zorunlu | Örnek | Eksikse |
|---|---|---|---|---|
| 1 | `title` | Evet | `adr-security-template` | Red |
| 2 | `type` | Evet | `template` | Red |
| 3 | `category` | Evet | `templates/adr` | Red |
| 4 | `status` | Evet | `draft` \| `active` \| `deprecated` | Red |
| 5 | `version` | Evet | `1.0.0` (üç bölüm) | Red |
| 6 | `updated` | Evet | `2026-09-25` (ISO) | Red |
| 7 | `authority` | Evet | `SSOT` | Red |
| 8 | `scope` | Evet (şablonda) | 1 satırlık Türkçe kapsam cümlesi | Red |

**7 bölüm iskeleti — her şablonda aynen bulunur (bu envanterin kendi iskeleti):**

| Bölüm | İskelet Zorunluluğu | İlgili Alan | Min Satır | Min KB |
|---|---|---|---|---|
| `§1 Amaç` | Neden var, ne üretir, SRP gerekçesi | `title`, `authority` | 40 | 2,0 |
| `§2 Kapsam` | Dosya tipi eşlemesi + sınır testi | `category` | 45 | 2,0 |
| `§3 Mimari` | Envanter/iskelet tabloları + diyagram | `scope` | 150 | 9,0 |
| `§4 Kurallar` | Sıra, onay, versiyonlama, G1..G12 | `status`, `version` | 70 | 4,0 |
| `§5 Workflow` | Adım tablosu + hata modları + PowerShell | `updated` | 60 | 4,0 |
| `§6 Doğrulama` | Checklist + red sinyalleri + Quality Report | `version` | 70 | 4,0 |
| `§7 Referanslar` | Kaynak tablosu + Red Team hataları | `authority` | 45 | 2,0 |
| **Toplam** | **7/7 zorunlu** | **8 alan zorunlu** | **≥ 480** | **≥ 27,0** |

> Ek yük (frontmatter + H1 + bağlantı + içindekiler) ≈ 25 satır; G6 asgariyi **480**'de sabitler.
> **Muaf:** `.templates/CLAUDE.md` (90–140 satır / 5–8 KB). Bu envanter: **480–780 satır / 26–46 KB**.

### §3.8 Dolu Örnek

17. şablonun (Part B) gerçekten doldurulmuş envanter satırı:

```markdown
| 18 | `handler-template` | `.ai/.templates/backend/handler-template.md` | `T-KOD` | YAZILDI | PLANLI | 30.9 | 12,0 | B |
```

Okunuşu: `#18` benzersiz · `handler-template` kebab-case + `-template` · yol kökten · `T-KOD` §1.3'ten · hedef `Application` katmanında · `PLANLI` tek ara değer · `developer` 14 rosterdan · `12,0` bir ondalık · `Part B` `#13–24` içinde. **Hiçbir hücre boş değil.**

### §3.9 Tam Kalıp

Yeni satır eklerken kopyalanacak kalıp (yalnız `{{…}}` doldurulur):

```markdown
| {{NO}} | `{{SABLON_ADI}}` | `.ai/.templates/{{BOLU}}/{{SABLON_ADI}}.md` | `{{TUR}}` | {{HEDEF_DOSYA}} | {{DURUM}} | {{SORUMLU_ROL}} | {{KB}} | {{PART}} |
```

| Placeholder | Kural | Örnek |
|---|---|---|
| `{{NO}}` | 1–44, benzersiz, artan | `21` |
| `{{BOLU}}` | §1.2'deki 12 klasörden biri | `infrastructure` |
| `{{TUR}}` | §1.3 kodu | `T-VER` |
| `{{HEDEF_DOSYA}}` | kökten itibaren gerçek yol | `db/migrations/V0xx__*.sql` |
| `{{SORUMLU_ROL}}` | 14 rolden biri | `database-optimizer` |
| `{{KB}}` | bir ondalıklı sayı | `13,0` |

> `{{PLACEHOLDER}}` yalnız §3.9 bloğunda yaşar; üretilen şablonda adedi **0** olmalıdır (G4).

### §3.10 Dizin Ağacı

```text
C:\versacoder\.ai\                          [ölçüm 2026-10-06]
+-- .templates\  50 MD  ·  12 alt klasör + 9 kök dosya   ← bu envanter (#1–#44)
+-- prompts\      2 MD  ·  01 master-enterprise (v2.0.0) · 02 master-installation (v1.0.0)
+-- reports\      1 MD  ·  conflict-register-001 (CF-01…CF-14)
+-- kök           0 MD  ·  ⚠️ AŞAMA C'ye kadar ÜRETİLMEZ — §4.3 bağımlılıklar yalnız bağlanır
+-- YOK (AŞAMA C)  .rules\  .decisions\  architecture\  checklists\  scripts\  .agents\  ui-design\
+-- YOK (AŞAMA C)  index.md  CLAUDE.md  WORKFLOW.md  keys.md  log.md  adr\
    .templates\ kök dosyalar:
      index.md (#1, BU DOSYA) · CLAUDE.md (#2) · session-log-template.md (#3)
      llm-provider-template · new-doc-contract · winform-form-designer-template
      ascii-box-drawing-prompt · batch-manifest-gate004a · unicode-box-drawing-prompt [EMEKLİ]
    .templates\ alt klasör (MD): adr\ 7 · agents\ 2 · architecture\ 4 · backend\ 3 · query\ 1
      · infrastructure\ 4 · ui-design\ 5 · frontend\ 3 · documentation\ 8 · testing\ 3
      · other\ 1 (cli-command #44 henüz yok) · csharp\ (boş — stok, §6.5 K6)
```

### §3.11 Bağımlılık Haritası

| # | Şablon | Önce Okunur | Sonra Üretir | Kırılırsa |
|---|---|---|---|---|
| 1 | `index` (#1) | `../index.md`, `WORKFLOW.md` | `CLAUDE.md` (#2) | kayıt dağılır |
| 2 | `.templates/CLAUDE.md` (#2) | #1 | Part A–D şablonları | erişim sırası kaybolur |
| 4 | `adr-template` (#4) | `index` | #5–#10 | ADR ailesi tutarsız |
| 13 | `architecture-template` (#13) | #4 | #14–#16 | katman sırası belgelenmez |
| 17–19 | `service/handler/repository` | #13, #15 | #21–#24 | katman ihlali koda sızar |
| 25 | `wireframe-template` | #13 | #26–#32 | GATE-003 geçilmez |
| 30–32 | `form/component/ribbon` | #25, #26 | — | kod, onaysız ekrana döner |
| 40–42 | `test*` | #17–#19 | — | %90 coverage kanıtsız kalır |

**Okuma sırası:** `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI` + yan `CrossCutting`, `Cli`. Uygulama: `VersaCoder.Desktop`, `VersaCoder.Cli`. TFM `net48` + `net10.0-windows`, DevExpress **26.1**.

## §4 Kurallar

### §4.1 Şablon Yazım Sırası

1. **Önce `.templates/`.** Şablonu olmayan çıktı üretilmez; önce şablon yazılır, sonra üretilir.
2. **Sıra:** `.templates/index.md` (satır aç) → `.templates/CLAUDE.md` (kural özeti) → Part A → B → C → D.
3. **İçerik özgün, yapı referanslı** — cümleler başka vault'tan kopyalanmaz (G1).
4. **Şablon yazarken envanter güncellenmez**; şablon bittikten **sonra** tek geçişte `PLANLI → YAZILDI` (çift kayıt G4 ihlali).
5. Her şablondan sonra §6.1 checklist koşulur; **17/17 geçmeden** sıradakine geçilmez.

### §4.2 Onay Akışı

| Adım | Rol | Ne Onaylar | Süre |
|---|---|---|---|
| 1 | `docs-writer` | Taslak + frontmatter | 10 dk |
| 2 | `reviewer` | §6.1 checklist (17 madde) | 8 dk |
| 3 | `architect` | Mimari tutarlılık (katman sırası, G2, G9) | 6 dk |
| 4 | `security` | Part A/B'de `adr-security` + G10 | 5 dk |
| 5 | `master-orchestrator` | Part geçişi (§6.3) + Quality Report (§6.4) | 5 dk |

> Toplam ≈ **34 dk/part**. Red: §6.2 → düzelt → adım 1. **3 red sonrası** iş durur, gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Part Sırası ve Öncelik

| Öncelik | Part | Neden Önce | Paralel? |
|---|---|---|---|
| 1 | A | Envanter + kararlar her şeyin girdisidir | Hayır |
| 2 | B | Mimari, kod iskeletinin tek kaynağı | A içinde yalnız `adr-devexpress` ile |
| 3 | C | Ekran onayı (GATE-003) koddan önce gelir | `ui-design` 5 şablon kendi içinde |
| 4 | D | Kanıt (test/güvenlik) en sona | documentation 5 + testing 3 |

- Aynı part içindeki şablonlar **paralel** yazılabilir; farklı part **asla**. Tek istisna `#3 session-log-template` (Part A içinde).

### §4.4 Versiyonlama

| Olay | Sürüm | Etkilenen | Kayıt |
|---|---|---|---|
| İlk yazım `0.1.0 → 1.0.0` | `major` | tek dosya | `updated` + `log.md` |
| Bölüm eklenmesi (`§x.y`) | `minor` | tek dosya | Quality Report |
| Yalnız tablo satırı ekleme | `patch` | envanter | `updated` |
| Bölüm silme / yeniden adlandırma | `major` | bağlantı veren dosyalar | `log.md` + §7.1 |
| `PLANLI → YAZILDI` | `patch` | envanter satırı | `Durum` hücresi |
| Part geçişi (§6.3) | `minor` | part başlıkları | `master-orchestrator` |

Kural: `version` her zaman üç bölüm (`major.minor.patch`), `updated` her zaman ISO `YYYY-MM-DD`.

### §4.5 Hard Guardrails G1..G12

| ID | Guardrail | Kapsam | İhlal | Tespit |
|---|---|---|---|---|
| **G1** | `C:\www\coremusic.net\.ai` ve `C:\www\versacoder\versacoder.ai` içeriği kopyalanamaz; yapı referans, içerik özgün | tümü | Red + sil | §7.2 R1 |
| **G2** | **EF Core yasak** — ADO.NET + Dapper, `@p1..@pN` | B/D | Red | `Select-String 'EntityFramework'` |
| **G3** | Frontmatter'sız Markdown yasak (8 alan) | tümü | Red | §6.1 K1 |
| **G4** | *Şablondan üretilen gerçek dosyada* `{{…}}` = 0 — **şablonda yer tutucu serbesttir** (CF-04) | üretim çıktısı | Red | §5.3 K5 |
| **G5** | Türkçe zorunlu; mojibake (`\xc3\xbc`, `?`) yasak | tümü | Red | gözden geçirme |
| **G6** | `< 480` satır reddedilir; **tek muaf:** `.templates/CLAUDE.md` (90-140) | **`.templates/**`** — `prompts/` ve `reports/` hariç (orada 3–8 KB sözleşmesi geçerli, D4) | Red | §5.3 K3 |
| **G7** | Bozuk link yasak — her `](…)` hedefi diskte var | tümü | Red | §5.3 K6 |
| **G8** | **GATE-003** onaysız *üretilmiş ekran dosyası* ve `src/**` kodu üretilmez — şablon üretimi serbest (k. CLAUDE G8) | C/D | Duraklat | `WORKFLOW.md` |
| **G9** | ADR'siz mimari karar yok; `.ai/.decisions/` dışında karar belgesi yazılmaz (k. CLAUDE G9) | A/B | Red | `ADR-001…030` |
| **G10** | SQL'de string concat yasak; her sorgu `@pN` + indeks notu | B | Red | `Query-Template` |
| **G11** | `TreatWarningsAsErrors=true` ve **%90** coverage ihlali = red | D | Red | xunit + coverage |
| **G12** | Envanterde içerik tekrarı yasak — şablon içeriği kendi dosyasında | bu dosya | Red | §1.1 M7 |

## §5 Workflow

### §5.1 9 Adımlı Şablon-Üretim Akışı

| Adım | İş | Çıktı | Sorumlu | Zaman | Kapı |
|---|---|---|---|---|---|
| 1 | Talep ayrıştırma | §3.x'te `PLANLI` satır | docs-writer | 2 dk | — |
| 2 | İskelet taraması | §3.7 sözleşmesinden bölüm listesi | docs-writer | 3 dk | — |
| 3 | Tür + hedef atama | `Tür`, `Hedef Dosya` | architect | 2 dk | — |
| 4 | Taslak yazım | frontmatter + H1 + 7 bölüm | docs-writer | 20 dk | — |
| 5 | Placeholder temizliği | `{{…}}` = 0 | docs-writer | 3 dk | G4 |
| 6 | Bağlantı denetimi | kırık link = 0 | reviewer | 4 dk | G7 |
| 7 | Ölçüm | satır/KB raporu | reviewer | 2 dk | G6 |
| 8 | Part geçiş kontrolü | geç / kal | master-orchestrator | 3 dk | §6.3 |
| 9 | Onay + kayıt | `YAZILDI` + Quality Report | master-orchestrator | 6 dk | §6.4 |
| | **Toplam/şablon** | | | **≈ 45 dk** | |

**Zaman bütçesi:**

| Part | Şablon | Bütçe | Tampon (%15) | Kümülatif |
|---|---|---|---|---|
| A | 12 | 70 dk | 10,5 dk | 80,5 dk |
| B | 12 | 75 dk | 11,3 dk | 91,8 dk |
| C | 10 | 65 dk | 9,8 dk | 101,6 dk |
| D | 10 | 65 dk | 9,8 dk | 111,4 dk |
| **Toplam** | **44** | **275 dk** | **41,4 dk** | **≈ 5,0 saat** |

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme | Etki |
|---|---|---|---|---|---|
| 1 | Çift kayıt | Aynı `#` iki kez | Satır iki kez açılmış | Fazla satırı sil | Envanter güvensiz |
| 2 | Sahipsiz şablon | `Sorumlu` boş | Rol atanmadan yazılmış | 14 rolden biri ata | İş durur |
| 3 | Bozuk iç bağlantı | `](../x.md)` 404 | Hedef üretilmedi | Bağlantıyı sil ya da hedefi üret | G7 ihlali |
| 4 | Şişmiş envanter | `> 680` satır | İçerik kopyalanmış | Taşı (G12) | SRP bozulur |
| 5 | Kısmi envanter | `44'ten az` satır | Part atlanmış | §6.3 ile Part'a dön | Sözleşme çöker |
| 6 | Placeholder sızıntısı | Üretimde `{{…}}` | Adım 5 atlanmış | Temizle, yeniden ölç (G4) | Üretim kırılır |
| 7 | Part sıçraması | C yazılırken B yarım | Öncelik çiğnenmiş | Geri al, §4.3 | Mimari boşluk |

### §5.3 Doğrulama Komutları (salt okunur)

Aşağıdaki 6 komut **hiçbir dosyayı değiştirmez**.

```powershell
# K1 — Envanter satır sayımı (hedef 44)
$p = "C:\versacoder\.ai\.templates\index.md"
"Toplam satır: {0} (hedef 44)" -f (Select-String -Path $p -Pattern '^\|\s*\d{1,2}\s*\|\s*`[^`]+`\s*\|\s*`\.ai/\.templates/[^`]+`\s*\|').Count
```

```powershell
# K2 — Eksik dosya taraması (Part A örnek listesi)
$root = "C:\versacoder\.ai\.templates"
'index.md','CLAUDE.md','session-log-template.md','adr\adr-template.md','adr\adr-index.md',
'agents\agents-template.md','architecture\architecture-template.md','backend\service-template.md',
'query\Query-Template.md','ui-design\wireframe-template.md','testing\unit-test-template.md' |
  ForEach-Object { $t = Test-Path (Join-Path $root $_)
    [PSCustomObject]@{ Şablon = $_; Var = $t } } | Where-Object { -not $_.Var } | Format-Table -AutoSize
```

```powershell
# K3 — Satır ölçümü (min 480; G6)
Get-ChildItem "C:\versacoder\.ai\.templates" -Recurse -Filter *.md |
  ForEach-Object { [PSCustomObject]@{ Dosya = $_.Name; Satır = (Get-Content $_.FullName).Count } } |
  Sort-Object Satır | Format-Table -AutoSize
```

```powershell
# K4 — KB ölçümü (envanter 26–46 KB; şablon ≥ 26 KB)
Get-ChildItem "C:\versacoder\.ai\.templates" -Recurse -Filter *.md |
  ForEach-Object { [PSCustomObject]@{ Dosya = $_.Name; KB = [math]::Round($_.Length / 1KB, 1) } } |
  Sort-Object KB -Descending | Format-Table -AutoSize
```

```powershell
# K5 — Placeholder taraması (CF-04: ŞABLONDA {{…}} SERBESTTİR — bu çıktı bilgidir, red değil;
#      G4'ün "0" araması yalnız şablondan ÜRETİLEN gerçek dosyada yapılır)
Get-ChildItem "C:\versacoder\.ai\.templates" -Recurse -Filter *.md |
  Select-String -Pattern '\{\{[^}]+\}\}' | Where-Object { $_.Filename -ne 'index.md' } |
  Group-Object Path | ForEach-Object { [PSCustomObject]@{ Dosya = $_.Name; Adet = $_.Count } } |
  Format-Table -AutoSize
# Gerçek G4 denetimi (üretim çıktısı üzerinde — hedef klasörü siz belirlersiniz):
#   Get-ChildPath '.ai\ui-design' -Recurse -Filter *.md | Select-String '\{\{[^}]+\}\}'  → 0 olmalı
```

```powershell
# K6 — Bağlantı denetimi (kırık link = 0; G7)
$md = "C:\versacoder\.ai\.templates\index.md"; $base = Split-Path $md
Select-String -Path $md -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches | ForEach-Object { $_.Matches } |
  ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  ForEach-Object { [PSCustomObject]@{ Hedef = $_; Var = (Test-Path (Join-Path $base $_)) } } |
  Where-Object { -not $_.Var } | Format-Table -AutoSize
```

## §6 Doğrulama

### §6.1 Doğrulama Checklist (17 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---|---|---|
| K1 | YAML frontmatter, 8 alan eksiksiz | `title,type,category,status,version,updated,authority,scope` | G3 |
| K2 | `authority: SSOT` birebir yazıyor | `SSOT` | §3.7 |
| K3 | H1 + `**Zorunlu Bağlantılar:**` satırı | 1 satır | §3.7 |
| K4 | İçindekiler var ve bağlantılıyor | 7 bölüm | §3.7 |
| K5 | 7 bölüm tam (`§1…§7`) | 7/7 | §3.7 |
| K6 | Alt bölümler `§1.1–§1.4`, `§2.1–§2.2` | eksiksiz | §3.7 |
| K7 | `§3` alt bölümleri `§3.1–§3.11` | 11/11 | §3.7 |
| K8 | `§4.1–§4.5` + G1..G12 | 5 + 12 | §4.5 |
| K9 | `§5.1–§5.3` (9 adım, hata modları, 6 komut) | 3/3 | §5 |
| K10 | `§6.1–§6.5` | 5/5 | §6 |
| K11 | `§7.1–§7.2` (Red Team ≥ 10 satır) | 2/2 | §7 |
| K12 | Envanter satır sayısı | 44 | §3.6 |
| K13 | Part toplamları | 12/12/10/10 | §3.6 |
| K14 | Dosya satır aralığı | 480–780 | G6 |
| K15 | Dosya KB aralığı | 26–46 | §3.7 |
| K16 | `{{…}}` yalnız §3.9'da | 0 (diğer yer) | G4 |
| K17 | Bağlantı hedefleri diskte var + Türkçe mojibake yok | 0 / 0 | G7, G5 |

> **D5 (2026-10-06, kullanıcı onayı):** K14 `520–680` → **`480–780`** · K15 `26–44` → **`26–46`**. Gerekçe: ölçüm 11 dosyayı 682–770 aralığında, 1 dosyayı 44,6 KB buldu; alt sınır ayrıca G6'nın `< 480` eşiğiyle çelişiyordu. Bkz. [çakışma kaydı](../reports/conflict-register-001-master-prompts.md) §6 **D5**.

### §6.2 Red Sinyalleri

| # | Sinyal | Neden Red | Aksiyon |
|---|---|---|---|
| R1 | Bir cümle başka vault'tan birebir geldi | G1/G12 | Sil + özgün yaz |
| R2 | `Durum` = `WIP`, `~` veya boş | §1.1 M3 | Tek geçerli değere çek |
| R3 | `Sorumlu` boş / 14 rolde olmayan | §1.1 M8 | Rol ata |
| R4 | Bölüm eksik (`§5` yok gibi) | §3.7 | Bölümü tamamla |
| R6 | `EntityFramework` geçen satır | G2 | Dapper/ADO.NET'e çevir |
| R9 | `.ai/.decisions/` dışında karar belgesi | G9 | Taşı |
| R10 | `.ai/ui-design/` varken wireframe yok | G8 | GATE-003'e dön |

### §6.3 Part Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan |
|---|---|---|
| A → B | 12/12 satır · 2 `YAZILDI` + 1 `MEVCUT` · K12–K13 geç · `CLAUDE.md` yazıldı | master-orchestrator |
| B → C | 12/12 `YAZILDI` · katman sırası belgelendi · G2/G10 geç | architect |
| C → D | 10/10 `YAZILDI` · **GATE-003** onayı · 7 ekran spec'i tam | ui-designer + orchestrator |
| D → Kapanış | 10/10 `YAZILDI` · %90 coverage planı · Quality Report `log.md`'de | testing + orchestrator |

- Geri dönüş **serbest** (C'de kusur → B'ye dön), **ileri sıçrama yasak**.
- Geçişte ilgili part satırları **toplu** `YAZILDI`'ye çevrilir; geçiş kaydı `log.md` + §6.4 Quality Report.

### §6.4 Quality Report Bloğu

`log.md`'ye değişiklik başına bir kez:

```markdown
## Quality Report — .templates/ (YYYY-MM-DD HH:MM)
- Part / Kapsam:  <A|B|C|D> / # <no> <şablon>
- Ölçüm:          <satır> satır / <KB> KB   (envanter 480–780 · 26–46; şablon ≥480 · ≥26)
- Checklist:      §6.1 → 17/17 geç
- Placeholder:    0 (K5)   ·   Kırık bağlantı: 0 (K6)
- Guardrail:      G1..G12 → ihlal yok   ·   Red sinyali: §6.2 → 0
- Part geçişi:    §6.3 → <A→B | —>
- Onay:           docs-writer → reviewer → master-orchestrator
- Karar:          ONAYLANDI | RED (<gerekçe>)
```

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Komut |
|---|---|---|---|
| K1 | Envanter satır sayısı | 44 | §5.3 K1 |
| K2 | Eksik dosya (PLANLI hariç) | 0 | §5.3 K2 |
| K3 | `index.md` satır aralığı | 480–780 | §5.3 K3 |
| K4 | Klasör ≠ tür tutarlılığı (`#43` `T-FE` ama `other/` altında) | not düşülmüş | gözden geçirme |
| K5 | Tahmini KB ↔ ölçülen KB | ±%20 | §5.3 K4 |
| K6 | `csharp/` klasörü boş | evet — stok, üretim yok | `Get-ChildItem …\csharp` |
| K8 | `.ai/.rules/` klasörü (AŞAMA C) | **0 — YOK** | `Get-ChildItem …\.rules` |
| K9 | Part toplam KB = 336,9 / 393,9 / 334,8 / 329,6 (toplam 1.395,2) | ±%10 | §3.6 · ölçüm 2026-10-06 (CF-16) |

## §7 Referanslar

### §7.1 Tablo

| # | Kaynak | Yol | Kullanım | Kanıt |
|---|---|---|---|---|
| 1 | Şablon altın örneği (yalnız yapı) | `.ai/.templates/adr/adr-template.md` | §3.7 iskeleti — içerik kopyalanmadı | ✅ diskte (34,9 KB) |
| 2 | Vault kökü — **0 MD**, hepsi AŞAMA C | `.ai/*.md` (isimler §3.10) | Bağlantılar, §4.2, §6.4 | ✅ ölçüldü 2026-10-06 |
| 3 | Kurallar — **0 MD, klasör YOK** (AŞAMA C) | `.ai/.rules/*.md` | §1.1, G11, #39 | ✅ ölçüldü 2026-10-06 · CF-08 |
| 4 | Kapılar `GATE-004a -> 003 -> 004 -> 005 -> 006` | `WORKFLOW.md` (kök; `.ai/WORKFLOW.md` YOK — CF-15) | §3.6, §6.3 (GATE-003 önce) | ✅ WORKFLOW §0 doğruladı 2026-10-04 |
| 5 | Dizin kuralı | `.ai/.templates/CLAUDE.md` | Erişim sırası, eşleme | YAZILDI |
| 6 | Bu envanter | `.ai/.templates/index.md` | SSOT | YAZILDI |

> **Kaynak doğrulama kuralı:** `⚠️` yalnız dosya **okunduğunda** kalkar. Okunmadan iddia edilen içerik bu belgede `⚠️ VERIFICATION REQUIRED` olarak kalır.

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Yakalama Sinyali | Doğrusu | Süre Kaybı |
|---|---|---|---|---|
| R1 | CoreMusic/versacoder.ai şablonunu olduğu gibi yapıştırmak | `coremusic` geçen satır | Yalnız **yapı** referans, cümle özgün (G1) | 30 dk |
| R2 | Şablon içeriğini envantere kopyalamak | `index.md` > 680 satır | Envanter = kayıt (G12) | 25 dk |
| R3 | Frontmatter yazmamak / `authority` atlamak | K1, K2 | 8 alan ilk 9 satırda (G3) | 10 dk |
| R4 | Part atlayıp önce C yazmak | §6.3 sıçraması | A → B → C → D | 60 dk |
| R5 | `{{PLACEHOLDER}}` ile yayınlamak | K5 > 0 | Adım 5 zorunlu (G4) | 15 dk |
| R6 | EF Core ile veri erişimi | `EntityFramework` satırı | ADO.NET + Dapper + `@pN` (G2, G10) | 90 dk |
| R7 | Tel kafes onayından önce WinForms kodu | GATE-003 kaydı yok | GATE-003 = koddan ÖNCE (G8) | 120 dk |
| R8 | Bozuk iç bağlantı bırakmak | K6 > 0 | Hedefi üret ya da sil (G7) | 10 dk |
| R9 | `Sorumlu` boş satır | §6.2 R3 | 14 rolden biri zorunlu | 5 dk |
| R10 | Mojibake / karışık dil | `\xc3\xbc`, `?` | Türkçe, UTF-8 (G5) | 20 dk |

### §7.3 Hedefe Özel Ek Şablonlar (kaynak envanter dışı — 7 adet)

| # | Şablon | Yol | Tür | Hedef Dosya | Durum | Sorumlu | Kaynak |
|---|---|---|---|---|---|---|---|
| E1 | `winform-form-designer` | `.ai/.templates/winform-form-designer-template.md` | `T-FE` | `src/**/*Form.Designer.cs` | YAZILDI | ui-designer | ARAŞTIRMA 2026-10-04 · `../reports/12-platform-arastirmasi.md` §4 |
| E2 | `llm-provider` | `.ai/.templates/llm-provider-template.md` | `T-VER` | `opencode.json` provider bloğu | YAZILDI | architect | ARAŞTIRMA 2026-10-04 · `../reports/13-kaynak-kod-arastirmasi.md` §1 |
| E3 | `ascii-box-drawing-prompt` | `.ai/.templates/ascii-box-drawing-prompt.md` | `T-MIM` | ASCII kutu diyagramları | YAZILDI | docs-writer | kaynak vault (byte kopya) |
| E4 | `batch-manifest-gate004a` | `.ai/.templates/batch-manifest-gate004a.md` | `T-ENV` | GATE-004a batch manifest | YAZILDI | architect | kaynak vault (byte kopya) |
| E5 | `new-doc-contract` | `.ai/.templates/new-doc-contract.md` | `T-DK` | **her yeni MD'de zorunlu** | YAZILDI | docs-writer | kaynak vault (byte kopya) |
| E6 | `unicode-box-drawing-prompt` | `.ai/.templates/unicode-box-drawing-prompt.md` | `T-DK` | kutu-artı promptu — halefi `ascii-box-drawing-prompt.md` (içerik 2026-10-01 oraya taşındı) | EMEKLİ | docs-writer | kaynak vault (byte kopya) |
| E7 | `root-md-template` | `.ai/.templates/documentation/root-md-template.md` | `T-DOK` | `.ai/` kök omurga MD | YAZILDI | docs-writer | kaynak vault (byte kopya) |

> E1–E7, §3.2–§3.5'teki 44 şablonluk kaynak envanterin **dışındadır**; Part
> toplamlarına dahil edilmez, sayım `E` ön ekiyle sürer. Yeni ek çıktığında bu
> tabloya **satır eklenir** (silme yok). Tür kodları §1.3'ten alınmıştır.

**SSUT beyanı:** Bu dosya `.templates/` envanterinin **tek** kaynağıdır. Çoğaltan her dosya yalnız [./index.md](./index.md)'e bağlanır.
