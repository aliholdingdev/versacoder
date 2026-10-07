---
title: "VersaCoder — Proje Kaydı (Project Registry)"
type: project
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Ana proje künyesi (bacak, katman sırası, yasaklı yığın, 7 ekran), 6 alt iş akışı, salt okunur referans projeler, bağımlılık grafiği, durum+gate tablosu ve güncelleme protokolü"
---

# VersaCoder — Proje Kaydı (`PROJECTS.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[VISION.md]] · [[log.md]] · [Kapsam analizi](reports/00-kapsam-analizi.md) · [Şablon envanteri](.templates/index.md) · [Ajan kaydı](.agents/AGENTS.md)

> **Tek cümlelik sorumluluk:** *Hangi proje, hangi alt iş akışı, kimin elinde, hangi gate'te, sırada ne var* sorusunun tek cevabıdır.
> Bu dosya **kapsam ve durum** tutar; ürün niyeti `VISION.md`'dedir, süreç `WORKFLOW.md`'dedir (planlı) — tekrar yasaktır (SRP).

---

## İçindekiler

| Bölüm | Alt Konular |
|---|---|
| §1 Ana Proje | Künye · bacaklar · katman sırası · yasaklı yığın · 7 ekran |
| §2 Alt İş Akışları | W1–W6: amaç, sahip, girdi/çıktı, durum, sonraki adım, DoD, efor |
| §3 Referans Projeler | Salt okunur kaynaklar + erişim kuralları |
| §4 Bağımlılık Grafiği | ASCII: hangi iş akışı hangisini blokluyor |
| §5 Durum + Gate Tablosu | Tüm iş akışları ↔ gate eşlemesi |
| §6 Durum Göstergesi + Güncelleme Protokolü | Kim, ne zaman, nasıl günceller |
| §7 Risk Devri · §8 Red Team · §9 Doğrulama · §10 Referanslar | Taşıma, hatalar, checklist, kaynaklar |

---

## §1 Ana Proje — VersaCoder

### §1.1 Künye

| Alan | Değer | Kaynak |
|---|---|---|
| Ürün adı | **VersaCoder** (AI Coding Platform / Agentic IDE) | `VISION.md` §1 |
| Repo kökü | `C:\versacoder` (git, `main` dalı, **henüz commit yok**) | 2026-09-25 ölçüm |
| Vault kökü | `C:\versacoder\.ai` (kullanıcı onaylı, `A1`) | `reports/00-kapsam-analizi.md` §3.3 |
| Uygulamalar | `VersaCoder.Desktop` (ana) + `VersaCoder.Cli` (headless) | `CLAUDE.md` §4 |
| Hedef bacaklar | `net48` + `net10.0-windows` | ai.03.md (karar 1) |
| UI yığını | WinForms · `RibbonForm` · `RibbonControl` (**Office2010**) | `CLAUDE.md` G3 |
| Kontrol kütüphanesi | DevExpress Universal **2026 / v26.1** | `CLAUDE.md` G3 |
| Veri erişimi | ADO.NET + **Dapper** (parameterized) | `CLAUDE.md` G2 |
| Test | xUnit + NSubstitute + NetArchTest (`NEEDS_VALIDATION`: kesin sürüm → B2, PART 5) | `reports/00-kapsam-analizi.md` §3.2 |
| Paket yönetimi | CPM — `Directory.Packages.props` | `CLAUDE.md` §4 |
| Mevcut durum | **PART 3 (vault kurulumu) ilerliyor** — kök **13/14** (`MEMORY.md` eksik) + 6 ajan + 28 şablon + 1 rapor yazıldı | `log.md` §3.9, §6.2 |
| Master kapsam | ~500+ MD vault · 14 kök belge (30–88 KB) · 8 part | `reports/00-kapsam-analizi.md` §2 karar 3 |

### §1.2 Katman Sırası (Bağımlılık Yönü: yukarı → aşağı)

| # | Katman | İçerik | Bağımlılığı |
|---|---|---|---|
| L1 | **Abstractions** | `Result`, contract, event, temel interface'ler | — |
| L2 | **Domain** | Entity, value object, invariant, domain event | L1 |
| L3 | **Application** | Use case / handler / CQRS command-query | L1–L2 |
| L4 | **Crosscutting** | Logging, exception, time, id | L1 |
| L5 | **Infrastructure** | Dapper + ADO.NET, dosya, secret vault | L1–L3 |
| L6 | **Protocol** | opencode-uyumlu mesaj/şema, tool sözleşmesi | L1 |
| L7 | **Host** | Composition root, DI, oturum yaşam döngüsü | L1–L6 |
| L8 | **UI (Desktop)** | WinForms + DevExpress ekranları | L7 |
| — | **Cli** | Headless host (aynı L1–L7) | L7 |

```text
        +--------------------------------------------------+
        |  L8  UI (WinForms + DevExpress v26.1)            |
        +--------------------------+-----------------------+
                                   |
        +--------------------------v-----------------------+
        |  L7  Host (composition root, DI, session)        |
        +----+---------------------+-------------------+---+
             |                     |                   |
        +----v----+          +-----v-----+        +----v-----+
        | L6 Proto|          | L5 Infra  |        | L3 Appl. |
        | (msg/   |          | (Dapper/  |        | (usecase)|
        |  tool)  |          |  ADO.NET) |        +----+-----+
        +----+----+          +-----+-----+             |
             |                     |                   |
             +----------+----------+---------+---------+
                        |                    |
                   +----v--------------------v----+
                   |  L1 Abstractions (Result,    |
                   |  contract, event)            |
                   |  L2 Domain   L4 Crosscutting |
                   +------------------------------+

   Kural: ok asla geriye gitmez (SOLID/DIP). UI -> Host -> digerleri.
```

> **KURAL:** Katman sırası `CLAUDE.md` §4 ile birebir aynıdır; sıra sapması mimari RED'tir.
> Katman rehberleri `architecture/` altında **planlı** (PART 3, adım 4).

### §1.3 Yasaklı Yığın (Forbidden Stack)

| # | Yasaklı | Yerine | Guardrail |
|---|---|---|---|
| 1 | **EF Core** (`Microsoft.EntityFrameworkCore*`, `DbContext`) | ADO.NET + Dapper | G2 |
| 2 | String-concat SQL (`$"SELECT …{x}"`) | Parameterized command + `CommandTimeout` | G2 |
| 3 | DevExpress sürümü ≠ **v26.1** (24.2.3, 23.x dahil) | v26.1 tek sürüm | G3 |
| 4 | `XtraForm` ana pencere · Office2007 ribbon stili | `RibbonForm` + `RibbonControlStyle.Office2010` | G3 |
| 5 | Tek bacak tasarım (`net48` sonra "bakılır") | İki bacak + `#if` yalnız platform farkı | G6 |
| 6 | EF migrate / ORM migration | Saf SQL + `schema_version` | G2 |
| 7 | Web UI · WPF · Electron · JS framework | WinForms + C# | G8 |
| 8 | Alternatif ORM / "daha modern" paket ekleme | CPM'de tanımlı paketler | G8 |
| 9 | ai.01 eski yığını (`.NET 8` · EF Core 9 · DevExpress 24.2.3) | ai.03.md kararları | §1.5 |
| 10 | CoreMusic vault'undan içerik kopyası | Elle, sıfırdan yazım | G1 |

### §1.4 7 Ekran Listesi (Ürün Haritası)

| # | Ekran | Sahip | Wireframe durumu |
|---|---|---|---|
| 01 | `01-start-page` | ui-designer | PART 4 (planlı) |
| 02 | `02-project-new` | ui-designer | PART 4 (planlı) |
| 03 | `03-editor` | ui-designer | PART 4 (planlı) |
| 04 | `04-agent-chat` | ui-designer | PART 4 (planlı) |
| 05 | `05-tasks` | ui-designer | PART 4 (planlı) |
| 06 | `06-terminal` | ui-designer | PART 4 (planlı) |
| 07 | `07-settings` | ui-designer | PART 4 (planlı) |

> **UYARI (CONFLICT DETECTED):** `reports/00-kapsam-analizi.md` §4.1 ve `.agents/ui-designer.md` §3
> farklı wireframe dosya adları listeler (`02-main-ide · 03-solution-explorer · 04-code-editor ·
> 05-ai-chat · 06-agent-panel`). Karar **PART 4 başında kullanıcı onayıyla** (GATE-003) verilir.
> Ayrıntı: `VISION.md` §5.2. Çözülene kadar bu tablo **vizyon adlarını** taşır, dosya adı bağlayıcı değildir.

### §1.5 Teknoloji Karar Kaydı (ai.01 ↔ ai.03 Çelişkisi)

| Kaynak | Ne diyor | Karar |
|---|---|---|
| `ai.01.md` (eski) | .NET 8 · EF Core 9 · DevExpress 24.2.3 | **REDDEDİLDİ** — yalnız *yapı* (ekran, gate, şema) alınır |
| `ai.02.md` | Discovery/Q&A ham veri | 5 karar bu veriden doğdu |
| `ai.03.md` (bootstrap) | net48 + net10.0-windows · DevExpress 2026 v26.1 · Dapper/ADO.NET | **ESAS** — bağlayıcı |

Kaynak: `reports/00-kapsam-analizi.md` §2 (5 senior karar) · detay `VISION.md` §11.3.

### §1.6 Hedef Dizin Yapısı (PART 5–6'da Doğrulanacak)

```text
C:\versacoder\
|-- .ai/                         # vault (simdi: 13/14 korek MD + .templates + .agents + reports)
|-- VersaCoder.slnx  |  .sln     # B1 karari ile secilecek (henuz YOK)
|-- Directory.Build.props        # ortak TFM, nullable, analyzer ayarlari
|-- Directory.Packages.props     # CPM: tek surum kaynagi (paket adi = surum)
|-- src/
|   |-- VersaCoder.Abstractions/     # Result, contract, event
|   |-- VersaCoder.Domain/           # entity, invariant
|   |-- VersaCoder.Application/      # use case, CQRS handler
|   |-- VersaCoder.Crosscutting/     # log, time, id
|   |-- VersaCoder.Infrastructure/   # Dapper + ADO.NET + secret vault
|   |-- VersaCoder.Protocol/         # mesaj semasi, tool sozlesmesi
|   |-- VersaCoder.Host/             # composition root, DI, session
|   |-- VersaCoder.Desktop/          # WinForms + DevExpress (Form.cs/.Designer.cs/.resx)
|   \-- VersaCoder.Cli/              # headless ayni L1-L7
|-- tests/
|   |-- VersaCoder.UnitTests/        # xUnit + NSubstitute
|   |-- VersaCoder.IntegrationTests/ # Dapper + Sqlite (yerel dosya)
|   \-- VersaCoder.ArchTests/        # NetArchTest: katman yonu (S1.2)
\-- build/                           # cikti (artifacts)
```

> **UYARI:** Yukarıdaki ağaç **hedeftir** (`INFERRED`), diskte henüz yok. Doğrulama PART 5'te
> `reports/05-*` içinde build çıktısıyla yapılır (G5). Proje adı/tipi B1 kararıyla kesinleşir.

### §1.7 İki Uygulama Karşılaştırması

| Özellik | `VersaCoder.Desktop` | `VersaCoder.Cli` |
|---|---|---|
| Host | WinForms `RibbonForm` | Console host (headless) |
| UI | DevExpress v26.1 (7 ekran) | Yok — stdout/JSON çıktı |
| Ajan döngüsü | Aynı (L1–L7) | Aynı (L1–L7) |
| Terminal ekranı `06` | Görsel panel | Doğrudan process I/O |
| Görev kuyruğu `05` | Grid görünümü | Durum satırı + exit code |
| Ayarlar `07` | `PropertyGridControl` | `--flag` + yerel ayar dosyası |
| Kullanım senaryosu | P1–P3 günlük geliştirme | CI / otomasyon / betik |
| Hedef bacak | `net48` + `net10.0-windows` | `net48` + `net10.0-windows` |
| Ortak kod | L1–L7 tamamı paylaşılır; `#if` yalnız UI/process farkı | (aynı) |

---

## §2 Alt İş Akışları (W1–W6)

### §2.1 Genel Bakış

| # | İş akışı | Amaç (tek cümle) | Sahip ajan | Gate |
|---|---|---|---|---|
| W1 | **Vault build** | `.ai/` 14 kök belge + klasör katmanı üretmek | architect | GATE-004 |
| W2 | **UI wireframes** | 7 ekran ASCII tel kafesini onaya sunmak | ui-designer | **GATE-003** |
| W3 | **Skeleton** | İki bacaklı solution + katman iskeleti + CPM | architect | GATE-005 |
| W4 | **Code** | Uygulama kodu (Desktop + Cli) + Designer/resx | developer | — (reviewer) |
| W5 | **Tests** | Birim + entegrasyon + mimari testleri yeşil tutmak | tester | — (reviewer) |
| W6 | **opencode integration** | Protokol/tool/session uyarlaması ve ADR'ler | architect + developer | — (part 7) |

### §2.2 W1 — Vault Build (`.ai/` Üretimi)

| Alan | Değer |
|---|---|
| **Amaç** | 14 kök belge (30–88 KB) + `architecture/`, `.decisions/`, `.rules/`, `spec/`, `ui-design/`, `reports/`, `scripts/`, `.sql/` katmanlarını üretmek; **0 bozuk link**. |
| **Sahip** | `architect` (`.agents/architect.md`) — elçi/kayıt `.agents/AGENTS.md` |
| **Girdi** | `reports/00-kapsam-analizi.md` · `.templates/` (28 MD, 776.6 KB) · bootstrap MD'ler |
| **Çıktı** | Kök MD'ler · `.rules/*.md` · `spec/*.md` · `architecture/**` · `.decisions/ADR-*` · link tarama raporu |
| **Mevcut durum** | **İlerliyor:** `.templates/` (28 MD) + `.agents/` (6 MD) + kök **13/14** (`MEMORY.md` eksik; `.rules/`, `spec/`, `architecture/`, `.decisions/` henüz yok) — `log.md` §3.9. |
| **Sonraki adım** | `MEMORY.md` → `.rules/` (7 dosya, R3 kapısı) → `spec/` + `architecture/` + `.decisions/` → `scripts/link-scan.ps1`. |
| **DoD** | 14/14 kök frontmatter'lı · `scripts/link-scan.ps1` → 0 bozuk link · her kök 30–88 KB · `index.md` envanteri disk ile tutarlı. |
| **Tahmini efor** | Kalan kök ≈ 2,5–4 saat · katmanlar (.rules/spec/architecture/decisions) ≈ 4–6 saat (**`INFERRED`**) |

### §2.3 W2 — UI Wireframes (7 Ekran)

| Alan | Değer |
|---|---|
| **Amaç** | 7 ekranın ASCII wireframe'ini + kontrol envanterini üretip kullanıcıya sunmak; GATE-003 onayı almak. |
| **Sahip** | `ui-designer` (`.agents/ui-designer.md`) |
| **Girdi** | `VISION.md` §5 (ürün haritası) · `reports/00-kapsam-analizi.md` §4.1 · `.templates/architecture/ascii-diagram-template.md` |
| **Çıktı** | `ui-design/index.md` + `ui-design/01-*.md … 07-*.md` (draft) + `flows.md` + `component-inventory.md` |
| **Mevcut durum** | **Başlamadı** — `ui-design/` klasörü yok. R1 (7 ≠ 16 wireframe) açık. |
| **Sonraki adım** | Ekran adı listesini kullanıcıya sun → onay → 7 dosyayı yaz → GATE-003 sunum paketi. |
| **DoD** | 7 dosya frontmatter'lı (`status: draft`) · her birinde mini künye + ASCII ` ```text ` bloğu · `RibbonControlStyle.Office2010` · net48 uyumluluk notu · **kullanıcı onayı**. |
| **Tahmini efor** | ≈ 2–3 saat (7 wireframe + onay turu) (**`INFERRED`**) |
| **Bağımlılık** | W1'den bağımsız **başlayabilir**; ama W3/W4'ü **bloklar** (§4). |

### §2.4 W3 — Proje İskeleti (Skeleton)

| Alan | Değer |
|---|---|
| **Amaç** | `VersaCoder.slnx|.sln` + katman projeleri + `Directory.Build.props` + `Directory.Packages.props` (CPM) + ilk `dotnet build` kanıtı. |
| **Sahip** | `architect` (iskelet) → `developer` (ilk kod) |
| **Girdi** | Katman sırası §1.2 · yasaklı yığın §1.3 · `.templates/` (documentation/backend) · B1 (slnx vs sln), B2 (test framework) kararları |
| **Çıktı** | `src/VersaCoder.*` + `tests/*` iskeleti · CPM dosyaları · build çıktısı (`reports/05-*`) |
| **Mevcut durum** | **Başlamadı** — `src/`, `tests/`, solution yok (`C:\versacoder` yalnız `.ai/`). |
| **Sonraki adım** | GATE-003 sonrası: B1/B2 kararını ver → solution + CPM yaz → **ilk `dotnet build` (R5 erken doğrulaması)**. |
| **DoD** | İki bacakta build **0 hata** · EF Core referansı 0 · DevExpress yalnız v26.1 · CPM tek sürüm kaynağı · `dotnet test` çalışır (0 test bile olsa koşar). |
| **Tahmini efor** | ≈ 2–3 saat (**`INFERRED`**) |
| **Bloklayan** | **W2 (GATE-003)** — onaysız iskelet kodlaması başlamaz (G12). |

### §2.5 W4 — Kodlama

| Alan | Değer |
|---|---|
| **Amaç** | 7 ekran + ajan döngüsü + veri erişimini üretmek: `.cs` + `.Designer.cs` + `.resx`. |
| **Sahip** | `developer` — inceleme `reviewer`, kabul `tester` |
| **Girdi** | W3 iskeleti · onaylı wireframe (W2) · `.rules/` (W1) · `spec/` (W1) |
| **Çıktı** | `src/VersaCoder.Desktop/**`, `src/VersaCoder.Cli/**`, `src/**.Designer.cs`, `**.resx` |
| **Mevcut durum** | **Başlamadı** — kaynak kod yok. |
| **Sonraki adım** | W3 DoD'su geçince: L1→L8 sırasıyla, her devrede 4 maddelik handover paketi. |
| **DoD** | İki bacakta build 0 hata · EF Core 0 · string-concat SQL 0 · her form `.Designer.cs`+`.resx` · `reviewer` PASS. |
| **Tahmini efor** | ≈ 12–20 saat (en büyük dilim) (**`INFERRED`**) |
| **Sıra** | W3 → W4 (zorunlu) |

### §2.6 W5 — Testler

| Alan | Değer |
|---|---|
| **Amaç** | Birim + entegrasyon + mimari testleri (NetArchTest ile katman yönü) yeşil tutmak; coverage kanıtı üretmek. |
| **Sahip** | `tester` — bulgu `reviewer` → `developer` |
| **Girdi** | W4 çıktısı · `.templates/` (testing → **planlı**, PART 6) · B2 karar (framework) |
| **Çıktı** | `tests/**` + coverage raporu + `log.md` kalite bloğu |
| **Mevcut durum** | **Başlamadı**. M3 (coverage hedefi) `NEEDS_VALIDATION`. |
| **Sonraki adım** | B2 kararını ver (PART 5) → test iskeletini W3'e ekle → TDD akışı W4 ile paralel. |
| **DoD** | `dotnet test` iki bacakta yeşil · katman bağımlılık testi geçer · coverage hedefi B2 ile sabitlenip karşılanır (G5). |
| **Tahmini efor** | ≈ 4–6 saat (**`INFERRED`**) |
| **Sıra** | W4 ile paralel, W4 devresinden sonra `reviewer` |

### §2.7 W6 — opencode Uyarlama

| Alan | Değer |
|---|---|
| **Amaç** | opencode v1.18.32 referansından tool/session/protocol modelini uyarlamak (kopyalamadan), `spec/protocol-spec.md` + ADR üretmek. |
| **Sahip** | `architect` (tasarım) + `developer` (entegrasyon) |
| **Girdi** | `%TEMP%\opencode-fresh` (33 paket; `core` 491 · `opencode` 836 · `tui` 241 · `server` 31 · `sdk` 48 · `protocol` 26 dosya) · `opencode kaynak kodu editle...geit.md` (122.873 B + 34.638 B) |
| **Çıktı** | `spec/protocol-spec.md`, `spec/tool-spec.md`, `spec/session-spec.md`, `spec/mcp-spec.md`, `spec/llm-provider-spec.md` + ADR (ADR-004, ADR-005, ADR-006..011 güncellemesi) |
| **Mevcut durum** | **Envanter alındı** (PART 1), tasarım başlamadı. |
| **Sonraki adım** | PART 7'de: `protocol` paketini (26 dosya) oku → mesaj şemasını VersaCoder'a uyarla → `REFERENCE_VERIFIED` etiketli ADR yaz. |
| **DoD** | 5 spec dosyası + en az 1 ADR · tool sözleşmesi (read/write/edit/bash/grep) tanımlı · B4 (provider önceliği) kararı verilmiş · kopya yok (G1). |
| **Tahmini efor** | ≈ 3–5 saat (**`INFERRED`**) |
| **Bloklayan** | W4'ün protokol kısmını; W3'ten bağımsız okuma yapılabilir. |

### §2.8 Ortak Çıktı Envanteri (Artifact Registry)

> **KURAL:** Her çıktı **tek satır** taşır. Durum `§6.1` göstergeleriyle yazılır; `BİTTİ` için kanıt zorunlu.

| # | Çıktı (yol) | Üreten iş akışı | Durum | Kanıt |
|---|---|---|---|---|
| 1 | `.ai/index.md` · `keys.md` · `CLAUDE.md` | W1 | BİTTİ | disk ölçümü 8.445 / 8.091 / 10.858 B |
| 2 | `.ai/VISION.md` · `PROJECTS.md` · `log.md` | W1 | BİTTİ | `log.md` §5–§7 kayıtları |
| 3 | `.ai/.templates/` (28 MD, 776.6 KB) | W1 | BİTTİ | `.templates/index.md` §1.2 + disk ölçümü |
| 4 | `.ai/.agents/` (6 MD, 29.7 KB) | W1 | BİTTİ | `.agents/AGENTS.md` §2 |
| 5 | `.ai/reports/00-kapsam-analizi.md` | W1 (PART 2) | BİTTİ | GATE-002 onayı |
| 6 | `.ai/MEMORY.md` (tek eksik kök belge) · `AGENTS` `WORKFLOW` `ROLE` `glossary` `brain` `engine` `ULTRA-THINKING` ✓ | W1 | 7/8 **BİTTİ** · `MEMORY.md` BEKLİYOR | `log.md` §3.9 (13/14 kök) |
| 7 | `.ai/.rules/*.md` (7 dosya: coding/security/devexpress/solid/data-access/testing/naming) | W1 | BEKLİYOR | R3 kapısı |
| 8 | `.ai/spec/*.md` (versacoder/protocol/session/tool/mcp/llm-provider/build/test-strategy) | W1 | BEKLİYOR | W6 girdisi |
| 9 | `.ai/architecture/` (00-overview + l0..l7 + index) | W1 | BEKLİYOR | katman rehberi |
| 10 | `.ai/.decisions/adr-index.md` + `accepted/ADR-001..` | W1, W6 | BEKLİYOR | ADR-001 güncelleme, ADR-002 iptal |
| 11 | `.ai/ui-design/index.md` + `01..07` + `flows.md` + `component-inventory.md` | **W2** | BEKLİYOR | GATE-003 |
| 12 | `.ai/scripts/link-scan.ps1` (+ ölçüm scripti) | W1 | BEKLİYOR | R6 kapısı |
| 13 | `.ai/reports/05-<part>-raporu.md` … `09-teslim-raporu.md` | Tümü | BEKLİYOR | G5 kanıtları |
| 14 | Solution + `Directory.Build.props` + `Directory.Packages.props` | **W3** | BEKLİYOR | B1 kararı |
| 15 | `src/VersaCoder.{Abstractions..Cli}/**` | **W4** | BEKLİYOR | 2 bacak build |
| 16 | `tests/**` + coverage raporu | **W5** | BEKLİYOR | B2 kararı |
| 17 | `spec/protocol-spec.md` + ADR (opencode uyarlaması) | **W6** | BEKLİYOR | `REFERENCE_VERIFIED` |

**Toplam izlenen çıktı:** 17 · `BİTTİ` 5 · `BEKLİYOR` 12 (2026-09-25 sayımı).

---

## §3 İlişkili / Referans Projeler (Salt Okunur)

### §3.1 Envanter

| # | Proje / Kaynak | Yol | İçerik | Amaç | Erişim |
|---|---|---|---|---|---|
| R1 | **CoreMusic vault** | `C:\www\coremusic.net\.ai` | 599 MD · 15 kök MD (30–94 KB) · 16 klasör | **YAPI** referansı (ölçek, dizin, kök MD kalıbı) | **Salt okunur** |
| R1b | CoreMusic kök belgeleri | `C:\www\coremusic.net\` | README 16.616 B · WORKFLOW 28.590 B · CLAUDE 3.139 B | Pointer modeli kalıbı | **Salt okunur** |
| R2 | **versacoder.ai yedek vault** | `C:\www\versacoder\versacoder.ai` | `.ai` + `.ai.01..04` ≈ 350+ MD · 16 wireframe (`WF-00..WF-15`) | **Bilgi** kaynağı (ADR listesi, gate listesi, wireframe fikri) | **Salt okunur** |
| R3 | **opencode kaynak** | `%TEMP%\opencode-fresh` | 33 paket · v1.18.32 · `core` 491 / `opencode` 836 / `tui` 241 / `server` 31 / `sdk` 48 / `protocol` 26 | Tool/agent/protocol **referans** uyarlaması | **Salt okunur** |
| R4 | Bootstrap plan dosyaları | `G:\Drive'ım\…\Versacoder\` | `ai.01` 150.166 B · `ai.02` 123.721 B · `ai.03` 21.984 B (CR-only) | Birincil karar kaynağı | **Salt okunur** (ai.03: normalize) |
| R5 | Çalışma deposu | `C:\versacoder` | git `main`, commit yok, `?? .ai/` | **Tek yazma yeri** | Yazılabilir |

### §3.2 Erişim Kuralları

| # | Kural | Kaynak | İhlalde |
|---|---|---|---|
| 1 | CoreMusic'e **hiçbir dosya yazılmaz** (`Copy-Item` dahil) | G1 | RED — dosya silinir |
| 2 | Yedek vault'tan **yalnız bilgi** alınır; blok yapıştırma yok | G1 | RED |
| 3 | Yedek kök MD boyutları (1–16 KB) **kalite çıtası değildir** | `reports/00-kapsam-analizi.md` §2 karar 3 | Yanlış ölçek |
| 4 | opencode kaynak kodundan **kod kopyalanmaz**; fikir/şema uyarlanır | G1 | RED |
| 5 | `ai.03.md` okunurken satır sonu normalize edilir; **diske CR-only yazılmaz** | R2 | Parse hatası |
| 6 | Yazma yalnız `C:\versacoder\` içinde | G7 | CRITICAL |

---

## §4 Bağımlılık Grafiği (Hangi İş Akışı Hangisini Blokluyor)

```text
   W1  VAULT BUILD (.ai/ 14 korek belge, rules, spec, architecture)
   |\
   | \_________________________________________________.
   |                                                   |
   v                                                   |
   W2  UI WIREFRAMES (7 ekran ASCII)                   |
   |                                                   |
   |   [GATE-003  KULLANICI ONAYI - ZORUNLU]           |
   v                                                   |
   W3  SKELETON (sln + katmanlar + CPM + ilk build)    |
   |                                                   |
   |   [GATE-005  IMPLEMENTATION PLAN ONAYI]           |
   v                                                   |
   W4  CODE (Desktop + Cli + Designer/resx) <----------+
   |\
   | \--> W5  TESTS (paralel; review devresiyle)
   |
   v
   W6  OPENCODE INTEGRATION (protocol/tool/session + ADR)
   |
   v
   PART 8  DOGRULAMA (build/test/link kanitlari, teslim raporu)

  Ok yonu = bloklayan yon. Kesikli baglanti (^) = W1 katmanlari
  (.rules/spec) W3/W4 icin gereklidir, W2 icin degil.
```

| # | Bağımlılık | Bloklayan | Bloklanan | Gerekçe |
|---|---|---|---|---|
| 1 | Kurallar yoksa kod yazılmaz | W1 (`.rules/`, `spec/`) | W3, W4 | G2/G3/G6 kuralları kodda uygulanır |
| 2 | Onaysız UI kodlanmaz | **W2 (GATE-003)** | W3, W4 | G12 — en sık ihlal riski |
| 3 | İskeletsiz kodlanmaz | W3 | W4 | Katman sırası §1.2 |
| 4 | Kod yoksa test olmaz | W4 | W5 | Test edilecek hedef |
| 5 | Protokol specsiz entegrasyon olmaz | W1 (`spec/`) + W4 | W6 | Şema önce, entegrasyon sonra |
| 6 | Kanıtsız kapanış olmaz | W4 + W5 | Part 8 | G5 |

> **KURAL:** W2'yi atlayıp W3/W4'e geçmek **en ağır ihlaldir** (G12, `CLAUDE.md` §6.6).

---

## §5 Durum + Gate Tablosu (Tüm İş Akışları)

| # | İş akışı | Durum | Başlangıç gate | Bitiş gate | Kanıt | Sonraki gate |
|---|---|---|---|---|---|---|
| — | PART 1 kaynak okuma | **BİTTİ** | GATE-000 | GATE-001 | `log.md` §4 envanter | GATE-002 ✓ |
| — | PART 2 kapsam analizi | **BİTTİ** | GATE-001 | GATE-002 | `reports/00-kapsam-analizi.md` | GATE-003 (PART 4) |
| W1 | Vault build | **DEVAM** (13/14 kök + 28 şablon + 6 ajan) | GATE-002 | GATE-004 | `log.md` §3.9 · `.templates/index.md` | GATE-004 |
| W2 | UI wireframes | **BEKLİYOR** (R1 açık) | GATE-004 | **GATE-003** | — (dosya yok) | **GATE-003** |
| W3 | Skeleton | **BEKLİYOR** | GATE-003 | GATE-005 | — (solution yok) | GATE-005 |
| W4 | Code | **BEKLİYOR** | GATE-005 | Part 8 | — (`src/` yok) | Part 8 |
| W5 | Tests | **BEKLİYOR** | GATE-005 | Part 8 | — (`tests/` yok) | Part 8 |
| W6 | opencode integration | **ARAŞTIRMA** (envanter ✓, tasarım ✗) | GATE-005 | Part 7 sonu | `log.md` §4 (33 paket) | Part 8 |

> **UYARI:** W1 henüz **GATE-004'ü** (Architecture Approval) geçmedi; W2 erken başlarsa bile
> W3 kapıda durur. Sıra: `W1 kalan → W2 → (GATE-003) → W3 → (GATE-005) → W4/W5 → W6 → Part 8`.

### §5.1 Gate Kanıt Paketleri (Bu Projeye Özgü)

| Gate | Ad | Beklenen kanıt paketi | Sunan | Onaylayan |
|---|---|---|---|---|
| GATE-000 | Discovery Approval | Girdi envanteri + vault kökü onayı | architect | kullanıcı |
| GATE-001 | Reference + Research Approval | 3 plan MD + coremusic + yedek + opencode okuma kaydı (`log.md` §4) | architect | kullanıcı |
| GATE-002 | Requirements Freeze | `reports/00-kapsam-analizi.md` (5 karar + R/B/A + DoD) ✓ | architect | kullanıcı |
| GATE-003 | **UI Approval** | 7 ASCII wireframe + kontrol envanteri + net48 uyumluluk notu + "Bu 7 ekran onaylı mı?" tek sorusu | ui-designer | **kullanıcı** |
| GATE-004 | Architecture Approval | 14 kök MD + `.rules/` + `architecture/` + link taraması 0 bozuk | architect | kullanıcı |
| GATE-005 | Implementation Plan Approval | Solution + CPM + iki bacak ilk build çıktısı (R5 erken sinyali) | architect + developer | kullanıcı |

> **KURAL:** Her gate kanıtı **dosyaya** bağlanır; ağızdan "geçti" geçersizdir (G5). Kanıt yoksa
> gate `BEKLİYOR` kalır.

---

## §6 Durum Göstergesi ve Güncelleme Protokolü

### §6.1 Durum Göstergesi (Legend)

| Gösterge | Anlamı | Kullanım kuralı |
|---|---|---|
| `BİTTİ` | DoD + kanıt tam | Kanıtsız kullanılamaz (G5) |
| `DEVAM` | İş sürüyor, çıktı kısmen var | Yüzde opsiyonel, kanıt zorunlu |
| `ARAŞTIRMA` | Girdi toplandı, tasarım/üretim başlamadı | Çıktı yok demektir |
| `BEKLİYOR` | Bloklayan gate/kapı var | Kapıya `onay` yazmadan `BİTTİ` olamaz |
| `RED` | İnceleme/kapı reddi | Bulgu: konum + neden + düzeltme |
| `IPTAL` | Kapsam dışı (non-goal) | `VISION.md` §6'ya kayıt şart |

**Yasak göstergeler:** `WIP` · `~` · `tahmini bitti` · kanıtsız `BİTTİ`.

### §6.2 Güncelleme Protokolü (Kim, Ne Zaman, Nerede)

| # | Tetikleyici | Kim günceller | Ne günceller | Nereye |
|---|---|---|---|---|
| 1 | Gate sonucu (onay/red) | Kapıyı yürüten ajan | §5 durum satırı + gate sütunu | `PROJECTS.md` + `log.md` kaydı |
| 2 | İş akışı DoD'su karşılandığında | O iş akışının sahibi (§2) | §2 ilgili blok: mevcut durum + sonraki adım | `PROJECTS.md` |
| 3 | Part geçişi | architect (elçi: `AGENTS.md`) | §5 + §4 (yeni bloklayan çıktıysa) | `PROJECTS.md` + `log.md` |
| 4 | Yeni risk/belirsizlik | Bulan ajan | R/B/A numarası → `reports/00-kapsam-analizi.md` §3 + §7 devri | Kapsam raporu + `PROJECTS.md` §7 |
| 5 | Ekran listesi değişimi | ui-designer | §1.4 (GATE-003 onayıyla) | `PROJECTS.md` + `VISION.md` §5.2 |
| 6 | Kural değişikliği | architect | `CLAUDE.md`/`.rules/` + **bu** dosyada yalnız **durum** | `log.md` (append) |
| 7 | Her oturum sonu | Çalışan ajan | `log.md`'ye **alttan** yeni kayıt (append-only) | `log.md` |

> **KURAL:** Güncelleme **üstten değil alttan** yapılır (`log.md` append-only). `PROJECTS.md`
> satırı değişince `log.md`'de karşılık kaydı yoksa kayıt eksiktir (DoD #9).

### §6.3 Sıklık

| Sıklık | İş |
|---|---|
| Her oturum başında | `log.md` son 2 kayıt + bu dosya §5 okunur |
| Her gate'te | §5 satırı güncellenir + kanıt satırı eklenir |
| Her part sonunda | §2 blokları + §4 grafiği gözden geçirilir |
| Part 8 | Tüm tablolar `reports/09-teslim-raporu.md` ile karşılaştırılır |

### §6.4 Örnek Güncelleme Kaydı (Kalıp)

Bir iş akışı tamamlandığında **aynı gün** iki yer güncellenir. Örnek:

```markdown
<!-- PROJECTS.md icinde -->
| W1 | Vault build | BITTI (14/14 korek + link 0 hata) | GATE-002 | scripts/link-scan.ps1 ciktisi | GATE-003 |

<!-- log.md icinde (ALTINDAN, append) -->
### 2026-09-26 | oturum ses_x | PART 3 | W1 kapanis
- AKSIYON : W1 DoD kontrolu + link taramasi
- SONUC   : 14/14 korek MD, 0 bozuk link
- KANIT   : scripts/link-scan.ps1 ciktisi (0 hata)
- SONRAKI : W2 UI wireframes baslat (GATE-003)
```

**Kontrol:** İki kayıt aynı anda yoksa güncelleme eksiktir → §6.2 kural 7 ihlali.
(Not: `log.md` biçimi `log.md` §1'deki resmi kalıptır; bu blok yalnız **örnektir**.)

---

## §7 Risk / Belirsizlik Devri (İş Akışına Bağlama)

| ID | Özet | Sahip iş akışı | Karar zamanı |
|---|---|---|---|
| R1 | 7 ekran ≠ 16 yedek wireframe (isim uyuşmazlığı) | W2 | **PART 4 başında, GATE-003 öncesi** |
| R2 | `ai.03.md` CR-only satır sonu | W1 (okuma) | Sürekli (okumada normalize) |
| R3 | ai.01'den EF Core / DevExpress 24.2 sızıntısı | W1 (`.rules/`) → W4 | PART 3, `.rules/` yazımında |
| R4 | ~500 MD + 30–88 KB kalitesi → zaman aşımı | W1 | Şablon başına bir kez yaz (`.templates/` hazır) |
| R5 | net48 DevExpress v26.1 uyumluluk belirsizliği | W3 | **İlk `dotnet build`** (PART 5) |
| R6 | ~bin iç linkte bozuk link | W1, Part 8 | Link taraması (PART 3 + PART 8) |
| B1 | `slnx` vs `sln` | W3 | PART 5 |
| B2 | Test framework kesin sürümü | W5 | PART 5 |
| B3 | 7 ekranın DevExpress kontrol seti kesinliği | W2 | PART 4, onay öncesi |
| B4 | LLM provider öncelik sırası | W6 | PART 7 |
| A1 | Vault kökü = `C:\versacoder\.ai` | Tümü | ✅ Onaylı |
| A2 | CoreMusic salt okunur yapı referansı | Tümü | ✅ Prompt yasağı |
| A3 | `.templates/` mevcut iş, çakışmasız | W1 | ✅ Doğrulandı (28 MD sayıldı) |

Kaynak: `reports/00-kapsam-analizi.md` §3 — bu tablo yalnız **iş akışına bağlar**, içeriği tekrar etmez.

---

## §8 Red Team (Sık Yapılan Hatalar)

| # | Hata | Doğrusu |
|---|---|---|
| 1 | Bu dosyaya süreç yazmak (gate adımları, part sırası) | Süreç `WORKFLOW.md`'de; burada yalnız **durum** |
| 2 | Durum satırını kanıtsız `BİTTİ` yapmak | G5: build/test/log çıktısı olmadan `BİTTİ` yazılmaz |
| 3 | W2'yi atlayıp iskelete başlamak | GATE-003 zorunlu (G12) |
| 4 | Referans vault'a yazmak | G1 — yalnız okuma; yazma `C:\versacoder\` |
| 5 | Efor tahminini gerçek süre gibi sunmak | Hepsi `INFERRED`; ölçüm `log.md`'de |
| 6 | Ekran listesini iki yerde farklı yazmak | `VISION.md` §5.2 ↔ §1.4 birebir aynı olmalı |
| 7 | Durum göstergesi olarak `WIP`/`~` kullanmak | §6.1 yasak listesi |

---

## §9 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`
- [ ] "Zorunlu Bağlantılar" hedefleri diskte mevcut
- [ ] §1.2 katman sırası `CLAUDE.md` §4 ile birebir aynı
- [ ] §1.3 yasaklı yığın G2/G3/G6/G8 ile örtüşüyor; EF Core yalnız yasak bağlamında
- [ ] §2'de 6 iş akışının her birinde 8 alan da dolu (amaç/sahip/girdi/çıktı/durum/sonraki/DoD/efor)
- [ ] §4 ASCII diyagramda Unicode kutu karakteri yok
- [ ] §5 tablosu `log.md` §3.9/§6.2 kaydıyla tutarlı (13/14 kök, 28 şablon, 6 ajan)
- [ ] §7'deki tüm R/B/A numaraları kapsam raporunda var
- [ ] Bozuk iç link yok (yalnız mevcut hedeflere link)
- [ ] Boyut hedef bandında (30–42 KB)

---

## §10 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kapsam analizi (girdi, 5 karar, R1–R6, B1–B4, A1–A3, DoD) | `.ai/reports/00-kapsam-analizi.md` |
| 2 | Ürün vizyonu (7 ekran, non-goals, metrikler) | `.ai/VISION.md` |
| 3 | Guardrail G1..G12 + teknoloji anayasası | `.ai/CLAUDE.md` |
| 4 | Oturum/activity günlüğü (append-only) | `.ai/log.md` |
| 5 | Şablon envanteri (28 MD) | `.ai/.templates/index.md` |
| 6 | Ajan kaydı + yetki matrisi | `.ai/.agents/AGENTS.md` |
| 7 | Ajan tanımları (architect/developer/reviewer/tester/ui-designer) | `.ai/.agents/` |
| 8 | Bootstrap (karar kaynağı) | `G:\…\Versacoder\versacoder proje planaması ai.03.md` |
| 9 | Salt okunur yapı referansı | `C:\www\coremusic.net\.ai` |
| 10 | Salt okunur bilgi referansı | `C:\www\versacoder\versacoder.ai` |
| 11 | opencode referans kaynağı (v1.18.32, 33 paket) | `%TEMP%\opencode-fresh` |
