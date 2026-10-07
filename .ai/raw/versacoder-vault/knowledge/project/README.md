---
title: 'Versa Coder - README: Proje Açıklaması, Uygulamalar, Teknoloji ve Başlangıç'
source: 'USER-BOOTSTRAP §9 (Root Documentation); versacoder proje planaması ai.md'
date: '2026-09-24'
status: 'ACTIVE'
confidence: 'SOURCE_VERIFIED'
related_decisions: ['ADR-001', 'ADR-003']
related_requirements: ['REQ-DOC-001']
---

# 1. Versa Coder Nedir

**Versa Coder**, Visual Studio ilhamlı bir **agentic IDE / AI coding
platform**dur: yazılım mühendisliğini çoklu-agent bir sistemle yürüten,
agent'ları oturum, workflow, policy ve denetim katmanları altında tutan bir
mühendislik ortamıdır.

Bu depo 2026-09-24 itibarıyla **tasarım ve karar aşamasındadır**: `.ai/`
altında kararlar (7 ADR) ve kurallar (10 dosya) vardır; **kod ve proje
dosyası yoktur** (`REPOSITORY_VERIFIED`). Kod, GATE-005 (implementation-plan)
`PASSED` olduktan sonra yazılmaya başlar.

| Bilgi | Değer |
|---|---|
| Durum | Tasarım/karar aşaması, kod yok |
| Depo dalı | `main` — **0 commit** (`REPOSITORY_VERIFIED`) |
| Kök dokümanlar | bu dosyanın da içinde olduğu 7 MD |
| Kural/karar | `.ai/.rules/` (10), `.ai/.decisions/` (7 ADR) |

# 2. Sistem Felsefesi

## 2.1 `LLM != SYSTEM`

LLM, sistemin kendisi **değil**; sistem içi karar/üretim bileşenlerinden
birisidir. Sistem şunlardan oluşur:

```text
Agent, Session, Workflow, Tool, MCP, Plugin, Skill, Memory, Policy,
Permission, Context, Validation, Execution, Observability
```

## 2.2 Ana akış

```text
User -> Intent -> Context -> Requirement -> Session
  -> Agent/Model/Tool/MCP secimi -> Policy Evaluation -> Workflow
  -> Approval -> Agent Execution -> Tool Execution -> Validation
  -> Review -> Memory -> Persistence -> Result
```

`Approval` = insan onayı (human-in-the-loop). Onay olmadan yürütüm tamamlanmaz
(ayrıntı: `SECURITY.md` Bölüm 6).

# 3. Uygulamalar

Depoda **2 uygulama** planlanmıştır:

| Uygulama | Arayüz | Görev | Kod durumu |
|---|---|---|---|
| `VersaCoder.Desktop` | C# **WinForms** + DevExpress | görsel çalışma ortamı: ribbon, dock, document manager | yok (`REPOSITORY_VERIFIED`) |
| `VersaCoder.Cli` | konsol | betik/otomasyon erişimi, başsız çalışma | yok |

Her iki uygulama da **aynı** hedeflerde çalışmalıdır: `net48` +
`net10.0-windows` (ADR-003).

# 4. Teknoloji Tablosu

| Alan | Seçim | Kaynak |
|---|---|---|
| Hedef framework | `net48` + `net10.0-windows` | ADR-003 (`accepted`) |
| UI framework | C# WinForms | proje tanımı |
| UI kütüphanesi | DevExpress Universal **2026 / v26.1** | ADR-001 (`accepted`) |
| UI bileşenleri | `RibbonControl` + `RibbonControlStyle.Office2010`, `RibbonForm`, `BarAndDockingController`, `DockManager`, `DocumentManager` | proje tanımı |
| Tasarımcı | `.Designer.cs` / `.resx` / `ResourceManager`, WinForms Designer uyumluluğu; Visual Studio yokken **elle** tasarlayıcı | ADR-005 (`accepted`) |
| Veri erişimi | **ADO.NET + Dapper** (parametreli, `@p1..@pN`, "PDO gibi") | ADR-004 (`accepted`) |
| Veritabanı | SQL Server + SQLite | ADR-004 (`accepted`) |
| ORM | **EF Core yasak** (net48 desteklemiyor) | `.ai/.rules/data.md` |
| Mimari | Clean Architecture + Layered; SOLID; DI; her katman ayrı sınıf kütüphanesi | `ARCHITECTURE.md` |
| DDD / CQRS | yalnız gerektiği yerde | proje kuralı |
| Sürüm yönetimi | git (`main`), commit henüz yok | `REPOSITORY_VERIFIED` |

# 5. Geliştirme Ortamı

## 5.1 Doğrulanmış ortam gerçekleri (2026-09-24)

| Öğe | Durum |
|---|---|
| Yerel DevExpress feed'i | **985 nupkg** (`REPOSITORY_VERIFIED`) |
| Feed'de `net10` varyantı | örneklemede **yok** → `NEEDS_VALIDATION` (C-ARCH-002) |
| `nuget.config` | **yok** |
| `Directory.Packages.props` (CPM) | **yok** → sürüm kayması riski |
| `sqlcmd` | **var** |
| `sqlite3` CLI | **yok** |
| SQL Server instance | `MSSQL$ALI` **Running** |
| `.gitignore` | **yok** → ilk commit'ten önce zorunlu |
| .NET SDK sürümü | `UNKNOWN` (kurulum kontrolü yapılmadı) |
| Test çerçevesi | `UNKNOWN` (`.ai/.rules/testing.md` Bolum 9.3) |
| Tarama aracı (secret/dependency) | `UNKNOWN` (ADR gerektirir) |

## 5.2 Kurulum kontrolü (kod yazmadan önce)

```text
[ ] dotnet --list-sdks        -> SDK var mi, net10 hedefi icin yeterli mi?
[ ] yerel feed yolu + surum   -> ADR-001 ile ayni mi?
[ ] nuget.config olusturuldu mu? (feed'i tanimlar)
[ ] Directory.Packages.props  -> surum tek yerde mi? (CPM)
[ ] .gitignore                -> ilk commit oncesi zorunlu
[ ] sqlcmd / sqlite3 erisimi  -> veri isleri icin
```

`dotnet` SDK sürümü, NuGet feed tanımı ve test çerçevesi seçilene kadar
ilgili işler `NEEDS_VALIDATION` ile yürütülür; uydurma sürüm **yazılmaz**.

# 6. Dizin Yapısı

## 6.1 Mevcut (disk, `REPOSITORY_VERIFIED`)

```text
C:\versacoder\
+-- AGENTS.md            agent tetikleyici kurallar
+-- CLAUDE.md            muhendislik baglami
+-- README.md            bu dosya
+-- WORKFLOW.md          13 halka + GATE-000..008
+-- SECURITY.md          guvenlik politikasi
+-- ARCHITECTURE.md      katmanlar + arayuz kontratlari
+-- CONTRIBUTING.md      katki kurallari
+-- .ai/
|   +-- .decisions/
|   |   +-- accepted/    ADR-001, 003, 004, 005
|   |   +-- draft/       ADR-002, 006
|   +-- .rules/          10 kural dosyasi
|   +-- archives/
|       +-- snapshots/   SES-20260924-ADR, SES-20260924-RULES
+-- .git/                main, 0 commit
```

## 6.2 Planlanan (henüz yok → `NEEDS_VALIDATION`)

```text
+-- src/                 VersaCoder.Desktop, VersaCoder.Cli (+ katman projeleri)
+-- tests/               test projeleri
+-- .ai/
    +-- requirements/    gereksinimler (GATE-002 kaniti)
    +-- architecture/    mimari notlar (GATE-004 kaniti)
    +-- gates/           kapi kayitlari
    +-- sessions/        oturum kayitlari
    +-- .agents/         AGENTS.md (rol katalogu)
    +-- implementation/  plan ve gorev dosyalari (T-NNN)
```

Proje bölme (katman başına ayrı `*.csproj` mı, tek proje mi)
**`NEEDS_VALIDATION`**'dır ve ilk `*.csproj` yazımında `architect` karar
verir.

# 7. Başlangıç Adımları

## 7.1 Kod yazma ön koşulları

Kodlama **şu sırayla** başlar; sıranın atlanması gate ihlalidir:

```text
1) workspace dogrulama   -> dizin, git, ortam (Bolum 5.1)
2) backup                -> .ai/archives/snapshots/<SES-ID>/  -> BACKUP_OK
3) kaynak analizi        -> AGENTS/CLAUDE/README/WORKFLOW/SECURITY/ARCHITECTURE
4) .ai kurulumu          -> requirements/ architecture/ gates/ sessions/ olustur
5) mimari analiz         -> katman + kontrat (ARCHITECTURE.md)
6) requirements          -> GATE-002 (REQ_YOK ise DURUR)
7) question engine       -> P0 sorular kapatilir (AGENTS.md Bolum 6)
8) UI ASCII              -> tel kafes, GATE-003
9) mimari onay           -> GATE-004 (ADR_YOK ise DURUR)
                            -> GATE-005 plan -> ancak sonra KOD
```

## 7.2 Günlük akış

1. `AGENTS.md` + `CLAUDE.md` oku (kalıcı bağlam).
2. İşe bağlı kuralı aç (`.ai/.rules/<ilgili>.md`).
3. Yedek al → `BACKUP_OK`.
4. Kapıları sırayla işlet, `KANIT` alanını **dosyaya** yaz.
5. Bütçeyi izle (45000 uyarısı / 50000 duraklatma).

# 8. Kapılar (Özet)

| Gate | Ad | Ne açar |
|---|---|---|
| GATE-000 | discovery | keşif |
| GATE-001 | reference + `.ai` | sınır kaynakları |
| GATE-002 | requirements | gereksinim |
| GATE-003 | ui | tel kafes / Designer |
| GATE-004 | architecture | ADR |
| GATE-005 | implementation-plan | görev kırılımı |
| GATE-006 | build | üretim kodu |
| GATE-007 | test | kanıt / coverage |
| GATE-008 | release | sürüm notu |

Kapılar **atlanamaz** ve **kullanıcı onayı olmadan geçiş yoktur**. Ayrıntı ve
kayıt formatı: `WORKFLOW.md`.

# 9. Doküman Haritası

| Dosya | Kim okur | Ne verir |
|---|---|---|
| `AGENTS.md` | agent | tetikleyici: okuma sırası, yasaklar, gate, bütçe |
| `CLAUDE.md` | agent | global bağlam + 14 halkalı okuma sırası |
| `README.md` | insan + agent | proje, teknoloji, ortam, başlangıç |
| `WORKFLOW.md` | agent + PM | 13 halka, kapılar, task formatı |
| `ARCHITECTURE.md` | agent + mimar | katmanlar, kontratlar, doğrulama |
| `SECURITY.md` | agent + güvenlik | politika, OWASP, vault, audit |
| `CONTRIBUTING.md` | insan geliştirici | branch/commit/PR/test kuralları |
| `.ai/.rules/**` | agent | 10 kural alanı |
| `.ai/.decisions/**` | agent | 7 ADR (kabul + taslak) |

Okuma sırasının tamamı `CLAUDE.md` Bölüm 2.1'dedir; bu tablo yalnız
**haritadır**, sıra **değildir**.

# 10. Referans ve Bilgi Güveni

Davranışsal referans: OpenCode (`https://github.com/anomalyco/opencode`) —
**COPY=NO**; `TRANSLATE` / `IMPROVE` / `RE-DESIGN` = YES. Canlı repo
snapshot'ı üstün gelir (**LIVE SOURCE WINS**).

Etiketler: `CONFIRMED` / `INFERRED` / `UNKNOWN` / `NEEDS_VALIDATION` /
`WEB_VERIFIED` / `REPOSITORY_VERIFIED` / `SOURCE_VERIFIED` /
`REFERENCE_VERIFIED`. Bilinmeyen `UNKNOWN` yazılır; **uydurulmaz**.

# 11. Bilinen Eksikler

| # | Eksik | Etkisi |
|---|---|---|
| 1 | `.gitignore` yok | ilk commit'te `bin/`, `obj/`, sırlar girebilir → commit öncesi zorunlu |
| 2 | NuGet feed tanımı (`nuget.config`) + CPM yok | sürüm kayması riski |
| 3 | Kod/`*.csproj` yok | derleme/test kanıtı yok (GATE-006/007 bekliyor) |

`.ai/requirements/` (13 dosya), `.ai/architecture/` (9 dosya) ve
`.ai/.agents/AGENTS.md` **VAR** (2026-09-25 disk): GATE-002/004 kanıtı
üretilebilir, rol kaynağı tam. Eksik kalan: `.ai/gates/` (**yok** →
`EKSIK_OKUMA`).

**Tier 0-8 notu:** önceki README sürümlerinde "Tier 0-8" tablosu vardı
(`.ai/.rules/ui.md` buna atıf yapar). Tablonun metni diskte **yok** →
`NEEDS_VALIDATION`; yeniden yazılmadan uydurulmaz.

# 12. Doğrulama Kriterleri

```text
[ ] Metadata ilk 12 satirda 7 alan
[ ] Bolum 1-12 sirasi; her baslikta bilgi + kaynak
[ ] LLM != SYSTEM + ana akis (Bolum 2)
[ ] 2 uygulama tablosu (Bolum 3)
[ ] Teknoloji tablosu ADR bagantili, EF Core yasaki icerir (Bolum 4)
[ ] Ortam gercekleri REPOSITORY_VERIFIED + UNKNOWN satirlari (Bolum 5.1)
[ ] Dizin yapisi mevcut/planlanan ayrimi (Bolum 6)
[ ] 9 adimli kod on kosulu (Bolum 7.1)
[ ] 9 kapi + dokuman haritasi (Bolum 8, 9)
[ ] OpenCode COPY=NO + 8 etiket (Bolum 10)
[ ] 3 eksik + gates EKSIK_OKUMA + Tier 0-8 NEEDS_VALIDATION (Bolum 11)
[ ] Uydurma surum/dosya/paket adi yok
[ ] 220-300 satir araliginda
```

| Tarih | Değişiklik |
|---|---|
| 2026-09-24 | İlk yazım - proje tanımı, 2 uygulama, teknoloji/ortam tabloları, dizin yapısı, kod öncesi 9 adım, kapı özeti, bilinen eksikler |
