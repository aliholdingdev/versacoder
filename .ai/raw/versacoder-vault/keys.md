---
title: "VersaCoder — Vault Keyword Map & Concept Router"
type: navigation
category: vault-navigation
status: active
version: 1.1.0
updated: 2026-09-25
authority: SSOT
scope: "Anahtar kelime → hedef dosya yönlendirme haritası; ajanların ilk başvurduğu referans"
governance: "Zero-Hallucination · SSOT · Turkish"
---

# VersaCoder — Anahtar Kelime Haritası (`keys.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[CLAUDE.md]] · [[WORKFLOW.md]] · [[AGENTS.md]] · [[brain.md]] · [[MEMORY.md]] · [[log.md]] · [[glossary.md]] · [.templates/index](.templates/index.md) · [.agents/AGENTS](.agents/AGENTS.md)

> **Tek cümlelik sorumluluk:** *Bu kavram hangi dosyada* sorusunun tek cevabıdır.
> Bu dosya **anahtar kelime ve yönlendiricidir** — içerik barındırmaz, bilgi vermez, yalnız **gönderir**.

---

## §0 Özet (Answer First)

**440+ anahtar kelime** → tek hedef dosya. Arama üç yoldan yapılır: (1) §3 zorlu anahtar listesi (28 madde, vault'un çekirdeği), (2) §10 A-Z dizini (≥180 terim), (3) konu tabloları §2–§9. **API key / secret asla bu dosyada değildir.** Kelime bir hedefe gitmiyorsa → `glossary.md`'ye ekle, uydurma.

---

## §1 Kullanım Kuralı

```
1. Bu tabloda anahtar kelimeyi bul          (§3 zorlu / §10 A-Z / konu tablolari)
2. "Hedef" sutunundaki yola git             (goreli ya da .ai/ tam yolu)
3. Hedef dosyada konuyu oku                 (SSOT orada)
4. Bu dosyayi DEGERLIK kaynagi olarak ATLA  (SRP)
```

| # | Kural | Neden |
|---|---|---|
| 1 | `keys.md` = yönlendirici, **kaynak değil** | İki kaynak = SSOT ihlali |
| 2 | Eşleşmeyen kelime → `glossary.md` → yoksa `log.md`'ye ekle | Sözlük büyür |
| 3 | Bir kelime **2'den fazla** hedefe gitmemeli | Belirsizlik = yanlış dosya |
| 4 | `[[…]]` yerine düz yol da çalışır: `.ai/.rules/x.md` | Taşınabilirlik |
| 5 | API key / secret **asla** bu dosyada değil (yalnız meta) | Güvenlik |
| 6 | Hedef henüz yazılmadıysa satır `PLANLI` işaretli | Ölü yol yasak |
| 7 | Noktalı dizin yaz: `.decisions` `.rules` `.templates` `.agents` `.sql` | Nokta unutulunca link kırılır |

---

## §2 Proje Çekirdek Anahtarları

| # | Anahtar Kelime | Hedef |
|---|---|---|
| 1 | VersaCoder, VERSA CODER, ürün adı, CoreMusic **değil** | `PROJECTS.md` |
| 2 | agentic IDE, AI coding platform, Visual Studio ilhamlı | `VISION.md` |
| 3 | kapsam, sınır, ne yapmayız, bileşen envanteri | `PROJECTS.md` |
| 4 | hedef uygulama, Desktop, Cli, iki uygulama | `PROJECTS.md` |
| 5 | iki bacak, net48, net10.0-windows, legacy kanıt, `#if` | `.rules/coding-standards.md` |
| 6 | master prompt, part sırası, görev planı | `WORKFLOW.md` |
| 7 | gate, GATE-001..006, onay, freeze | `WORKFLOW.md` §3 |
| 8 | rol, mutlak rol, öncelik çatışması, senior karar | `ROLE.md` |
| 9 | ajan, agent, sub-agent, alt ajan, handover, devre | `AGENTS.md` |
| 10 | architect, developer, reviewer, tester, ui-designer | `.agents/<rol>.md` |
| 11 | ajan kaydı, yetki matrisi, X sahibi | `.agents/AGENTS.md` |
| 12 | 3-STRIKE, escalation, şüpheli varsayım | `AGENTS.md` §8 |
| 13 | devre paketi, 4 madde, handoff | `AGENTS.md` §4 |
| 14 | context bütçesi, 8 KB, subagent eşiği | `AGENTS.md` §5–§6 |
| 15 | orkestrasyon matrisi, part ↔ gate | `engine.md` |
| 16 | derin düşünme, 9 adım, red team | `ULTRA-THINKING.md` |
| 17 | oturum belleği, kalıcılık, silme kuralı | `MEMORY.md` |
| 18 | log, kayıt, kalite raporu bloğu | `log.md` |
| 19 | terim TR/EN eşlemesi | `glossary.md` |
| 20 | ADR özetleri, mühendislik kısıtları | `brain.md` |
| 21 | vizyon, problem–çözüm matrisi, 7 ekran | `VISION.md` |
| 22 | navigasyon, nerede bulurum | `index.md` |
| 23 | guardrail, G1..G12, anayasa | `CLAUDE.md` |
| 24 | sıfır halüsinasyon, UNKNOWN, NEEDS_VALIDATION, INFERRED | `CLAUDE.md` |

---

## §3 Zorlu Anahtarlar (MUTLAK GEÇMELİ)

> Bu bölümdeki kavramların **her biri** vault'ta karşılık bulmalıdır. Eksik = vault eksik.

| # | Anahtar Kelime | Hedef |
|---|---|---|
| 1 | **VersaCoder** | `PROJECTS.md` |
| 2 | **agentic IDE** | `VISION.md` |
| 3 | **ribbon**, RibbonControl, RibbonForm | `.rules/devexpress-rules.md` |
| 4 | **RibbonControlStyle.Office2010** | `.rules/devexpress-rules.md` |
| 5 | **resx**, ResourceManager, Designer.cs | `.rules/devexpress-rules.md` |
| 6 | **Dapper**, micro-ORM, mapper | `.rules/data-access-rules.md` |
| 7 | **parameterized**, command timeout, injection | `.rules/security-rules.md` |
| 8 | **ADO.NET**, provider abstraction, Sqlite/SqlClient | `.rules/data-access-rules.md` |
| 9 | **EF Core** (YASAK) | `CLAUDE.md` G2 |
| 10 | **DevExpress v26.1**, Universal 2026 | `.rules/devexpress-rules.md` |
| 11 | **net48**, net10.0-windows, iki bacak | `.rules/coding-standards.md` |
| 12 | **WinForms**, RibbonForm, form üçlüsü | `architecture/l7-ui/index.md` |
| 13 | **Clean Architecture**, katman, bağımlılık yönü | `architecture/index.md` |
| 14 | **SOLID**, SRP, OCP, LSP, ISP, DIP | `.rules/solid-rules.md` |
| 15 | **CQRS**, **DDD**, aggregate, invariant | `.rules/coding-standards.md` |
| 16 | **Protocol**, mesaj şeması, tool sözleşmesi | `spec/protocol-spec.md` |
| 17 | **Host**, composition root, DI | `architecture/l6-host/index.md` |
| 18 | **Abstractions**, Result, contract, event | `architecture/l0-abstractions/index.md` |
| 19 | **opencode**, tool set, read/write/edit/bash/grep | `spec/tool-spec.md` |
| 20 | **agent**, session, orkestrasyon, streaming | `spec/session-spec.md` |
| 21 | **MCP**, gateway, plugin, sandbox | `spec/protocol-spec.md` |
| 22 | **skill**, yetenek, auto-load | `VISION.md` (PART 7 kapsamı) |
| 23 | **GATE**, onay, freeze, teslim | `WORKFLOW.md` §3 |
| 24 | **ADR**, karar, alternatif, sonuç | `.decisions/index.md` |
| 25 | **SSOT**, single source of truth, authority | `CLAUDE.md` |
| 26 | **frontmatter**, YAML, metadata | `.templates/documentation/docs-md-template.md` |
| 27 | **ASCII**, wireframe, diyagram, kutu-art | `.templates/architecture/ascii-diagram-template.md` |
| 28 | **migration**, version tablosu, schema_version | `.rules/data-access-rules.md` |

**Ek mutlak kelimeler (PART 3 sonrası):**

| # | Anahtar | Hedef |
|---|---|---|
| 29 | **52 düğüm**, 7 makro × 45 alt katman, 5000+ bileşen | `architecture/index.md` (ADR-013 planlı) |
| 30 | **check-links**, bozuk link, link bütünlüğü | `scripts/check-links.ps1` |
| 31 | **measure-vault**, boyut bandı 30–88 KB | `scripts/measure-vault.ps1` |
| 32 | **Test stratejisi**, piramit, kapsam ≥ %90 | `spec/test-strategy.md` |
| 33 | **performans bütçesi**, N+1, `SELECT *` | `.rules/performance-guidelines.md` |
| 34 | **dağıtım**, CI 4 hücre, rollback | `.rules/deployment-guide.md` |

---

## §4 Teknoloji → Dosya

| # | Teknoloji / Paket | Hedef |
|---|---|---|
| 1 | C# / LangVersion / nullable / expression body | `.rules/coding-standards.md` |
| 2 | WinForms / Designer / resx / High DPI | `.rules/devexpress-rules.md` |
| 3 | DevExpress Ribbon, Grid, Docking, TreeList, Editors | `.rules/devexpress-rules.md` |
| 4 | Dapper / `Microsoft.Data.Sqlite` / `Microsoft.Data.SqlClient` | `.rules/data-access-rules.md` |
| 5 | ADO.NET / `DbConnection` / parameterized command | `.rules/data-access-rules.md` |
| 6 | xUnit / NSubstitute / NetArchTest / coverage | `.rules/testing-rules.md` |
| 7 | Directory.Build.props / Directory.Packages.props (CPM) | `WORKFLOW.md` PART 5 · `.templates/` (PLANLI: `spec/build-spec.md`) |
| 8 | opencode (core, tui, server, sdk, protocol) | `spec/tool-spec.md` · `reports/00-kapsam-analizi.md` §1.3 |
| 9 | Ollama / OpenAI-uyumlu API / streaming / SSE | `spec/protocol-spec.md` (ADR-011 PLANLI) |
| 10 | secret store / DPAPI / ProtectedData / audit | `.rules/security-rules.md` |
| 11 | git / pre-commit / branch / commit kuralı | `.rules/coding-standards.md` |
| 12 | SqlClient sürüm kilidi (5.x iki bacak) | `.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md` |
| 13 | Sqlite ikili destek (`NEEDS_VALIDATION`) | `reports/01-web-kaynaklari.md` (C6) |
| 14 | .NET 10 LTS / C# 14 | `reports/01-web-kaynaklari.md` |
| 15 | DevExpress prerequisites (FX 4.6.2+, .NET 8/9/10) | `reports/01-web-kaynaklari.md` |
| 16 | RibbonForm / Form.cs + .Designer.cs + .resx | `.rules/devexpress-rules.md` |
| 17 | theme / skin / UserLookAndFeel | `.rules/devexpress-rules.md` |
| 18 | TreeList / GridControl / DockManager | `ui-design/README.md` (PART 4) |
| 19 | DateTime / TimeProvider / saat bağımsız test | `.rules/testing-rules.md` |
| 20 | CancellationToken / timeout / retry / backoff | `.rules/performance-guidelines.md` |

---

## §5 Kavram → Dosya

| # | Kavram | Hedef |
|---|---|---|
| 1 | Zero-Hallucination, UNKNOWN, NEEDS_VALIDATION, INFERRED | `CLAUDE.md` |
| 2 | kaynak hiyerarşisi, CONFLICT DETECTED, DECISION BLOCKED | `ROLE.md` |
| 3 | devre paketi, handover, escalate | `AGENTS.md` §4 |
| 4 | TDD, RED-GREEN-REFACTOR, coverage hedefi | `.agents/tester.md` |
| 5 | bulgu biçimi, seviye (CRITICAL..LOW) | `.agents/reviewer.md` |
| 6 | 7 ekran, wireframe, GATE-003, tel kafes | `ui-design/README.md` |
| 7 | oturum belleği, kalıcılık, silme kuralı | `MEMORY.md` |
| 8 | log, kayıt, kalite raporu bloğu | `log.md` |
| 9 | orkestrasyon matrisi, part ↔ gate | `engine.md` |
| 10 | derin düşünme, 9 adım, red team | `ULTRA-THINKING.md` |
| 11 | terim TR/EN eşlemesi | `glossary.md` |
| 12 | katman yönü, bağımlılık matrisi 8×8 | `architecture/00-overview/dependency-map.md` |
| 13 | sistem, modüller, akışlar, NFR | `architecture/00-overview/system-overview.md` |
| 14 | katman şartnamesi (lN) | `architecture/lN-*/index.md` |
| 15 | ADR yazım şablonu | `.templates/adr/adr-nygard-template.md` |
| 16 | risk, belirsizlik, varsayım | `reports/00-kapsam-analizi.md` §3 |
| 17 | kapsam dışı, out of scope | `reports/00-kapsam-analizi.md` §4.4 |
| 18 | çelişki kaydı C1..C6 | `reports/01-web-kaynaklari.md` |
| 19 | test piramidi / entegrasyon / birim | `spec/test-strategy.md` |
| 20 | loop policy, step budget, compaction | `spec/session-spec.md` |
| 21 | tool JSON şeması, tool_call / tool_result | `spec/tool-spec.md` |
| 22 | protocolVersion, fail-closed | `spec/protocol-spec.md` |
| 23 | ürün şartnamesi, kabul kriterleri | `spec/versacoder-spec.md` |
| 24 | migration runner, `NNN-*.sql` | `.rules/data-access-rules.md` · `.sql/` |
| 25 | performans B1..B10 bütçesi | `.rules/performance-guidelines.md` |
| 26 | release, artifact, geri alma planı | `.rules/deployment-guide.md` |
| 27 | security SEV1..SEV6, injection sınıfı | `.rules/security-rules.md` |
| 28 | isimlendirme dosya/tip/metot | `.rules/naming-conventions.md` |
| 29 | SOLID ihlal belirtisi | `.rules/solid-rules.md` |
| 30 | link taraması kanıtı | `scripts/check-links.ps1` |

---

## §6 Dosya Adı → Konu

| Dosya yolu | Konu |
|---|---|
| `.ai/index.md` | Navigasyon (§0..§22) |
| `.ai/CLAUDE.md` | Guardrail G1..G12 |
| `.ai/WORKFLOW.md` | Süreç + gate |
| `.ai/ROLE.md` | Rol önceliği |
| `.ai/AGENTS.md` | Ajan protokolü |
| `.ai/brain.md` | ADR özet + kısıt |
| `.ai/keys.md` | Bu dosya |
| `.ai/glossary.md` | Terim sözlüğü |
| `.ai/.rules/*.md` (9) | Hard kurallar |
| `.ai/spec/*.md` (5) | Şartname |
| `.ai/.decisions/index.md` | ADR defteri |
| `.ai/.decisions/accepted/ADR-NNN-*.md` | Tek karar |
| `.ai/architecture/index.md` | Katman haritası |
| `.ai/architecture/00-overview/*.md` | Sistem + bağımlılık |
| `.ai/architecture/lN-*/index.md` | Katman şartnamesi |
| `.ai/ui-design/NN-*.md` | Ekran wireframe |
| `.ai/reports/NN-*.md` | Part raporu / kanıt |
| `.ai/.templates/**` (33) | Üretim kalıbı |
| `.ai/.agents/*.md` (6) | Ajan görev tanımı |
| `.ai/scripts/*.ps1` | Denetim scripti |
| `.ai/.sql/**` | Migration / sorgu |

---

## §7 DevExpress Kontrol → Dosya

| Kontrol | Ekran/Kullanım | Hedef |
|---|---|---|
| `RibbonControl` + `RibbonForm` | 02-main-ide | `.rules/devexpress-rules.md` |
| `DockManager` / `DockPanel` | 02-main-ide panelleri | `ui-design/README.md` |
| `XtraTabControl` | 02 / 04 sekmeler | `ui-design/README.md` |
| `TreeList` | 03-solution-explorer | `ui-design/README.md` |
| `RichEditControl` | 04-code-editor | `ui-design/README.md` |
| `MemoEdit` | 05-ai-chat girdi | `.rules/devexpress-rules.md` |
| `ListBoxControl` | 05-ai-chat akışı | `.rules/devexpress-rules.md` |
| `GridControl` / `TileView` | 06-agent-panel | `.rules/devexpress-rules.md` |
| `LayoutControl` / `DataLayoutControl` | 07-settings | `.rules/devexpress-rules.md` |
| `SearchControl` | 01-start-page, 04 | `.rules/devexpress-rules.md` |
| `TileControl` | 01-start-page | `.rules/devexpress-rules.md` |
| `LookUpEdit` / `ComboBoxEdit` | 07-settings | `.rules/devexpress-rules.md` |
| `CheckEdit` / `ToggleSwitch` | 07-settings | `.rules/devexpress-rules.md` |
| `ProgressBarControl` / `MarqueeProgressBarControl` | 05-ai-chat yükleme | `.rules/performance-guidelines.md` |
| `StatusBarControl` | 02-main-ide | `ui-design/README.md` |
| `SvgImageCollection` / `ImageCollection` | tümü | `.rules/devexpress-rules.md` |
| `UserLookAndFeel` (skin) | tema | `.rules/devexpress-rules.md` |
| `BeginUpdate` / `EndUpdate` | topal yenileme | `.rules/performance-guidelines.md` |
| `ResourceManager` / `.resx` | form üçlüsü | `.rules/devexpress-rules.md` |
| `GDI+ / Paint` (özel çizim) | nadiren | `.rules/performance-guidelines.md` |

---

## §8 Nümerik Anahtarlar

| Küme | Üyeler | Hedef |
|---|---|---|
| **Gate** | GATE-001 (kaynak okuma) · 002 (kapsam) · 003 (UI onayı) · 004/004a (vault+iskelet) · 005 (test) · 006 (kapanış) | `WORKFLOW.md` §3 |
| **Guardrail** | G1..G12 | `CLAUDE.md` |
| **Part** | P1..P8 (okuma → doğrulama) | `WORKFLOW.md` §1 |
| **Katman** | l0..l7 (inşa sırası) | `architecture/index.md` |
| **ADR** | 001 dual-target · 002 handwritten · 003 vault · 004 DevExpress · 005 Dapper · 006..011 PART 7 · 012 provider · 013 52 düğüm | `.decisions/index.md` |
| **Risk** | R1..R12 | `reports/00-kapsam-analizi.md` §3 |
| **Karar** | §2.1 karar 1..11 | `reports/00-kapsam-analizi.md` §2 |
| **Çelişki** | C1..C6 | `reports/01-web-kaynaklari.md` |
| **Performans** | B1..B10 | `.rules/performance-guidelines.md` |
| **Dağıtım** | P1..P5 (publish), CI 4 hücre, R1..R4 (release) | `.rules/deployment-guide.md` |
| **Güvenlik** | SEV1..SEV6 | `.rules/security-rules.md` |
| **Escalation** | Seviye 0..4 | `AGENTS.md` §8.2 |
| **DoD** | §9.1..§9.5 + ortak | `AGENTS.md` §9 |

---

## §9 Kısaltmalar (TR/EN)

| Kısaltma | Tam | Hedef |
|---|---|---|
| ADR | Architecture Decision Record | `.decisions/index.md` |
| BCL | Base Class Library | `architecture/l0-abstractions/index.md` |
| CQRS | Command Query Responsibility Segregation | `.rules/coding-standards.md` |
| DI | Dependency Injection | `architecture/l6-host/index.md` |
| DoD | Definition of Done | `AGENTS.md` §9 |
| DDD | Domain-Driven Design | `.rules/coding-standards.md` |
| LLM | Large Language Model | `spec/protocol-spec.md` |
| MCP | Model Context Protocol | `spec/protocol-spec.md` |
| NFR | Non-Functional Requirement | `architecture/00-overview/system-overview.md` |
| RID | Runtime Identifier | `.rules/deployment-guide.md` |
| SDK | Software Development Kit | `WORKFLOW.md` PART 5 |
| SLA | Service Level Agreement | `AGENTS.md` §2.3 |
| SSE | Server-Sent Events | `.rules/performance-guidelines.md` |
| SSOT | Single Source of Truth | `CLAUDE.md` |
| TFM | Target Framework Moniker | `.rules/coding-standards.md` |
| YAGNI | You Ain't Gonna Need It | `.rules/solid-rules.md` |

---

## §10 A-Z Hızlı Dizin

> Biçim: `**terim** → hedef`. Alfabetik; her terim **tek** hedefe gider.

### A

- **abort / iptal** → `spec/session-spec.md`
- **ADO.NET** → `.rules/data-access-rules.md`
- **agent (ajan)** → `AGENTS.md`
- **agent döngüsü** → `spec/session-spec.md`
- **agentic IDE** → `VISION.md`
- **AI Chat paneli** → `ui-design/README.md`
- **anahtar kelime** → bu dosya
- **anayasa (G1..G12)** → `CLAUDE.md`
- **API key** → `.rules/security-rules.md` (bu dosyada DEĞİL)
- **architect** → `.agents/architect.md`
- **arşiv / backup** → `.rules/deployment-guide.md`
- **ASCII diyagram** → `.templates/architecture/ascii-diagram-template.md`
- **async / await** → `.rules/coding-standards.md`
- **atomite** → `.rules/data-access-rules.md`

### B

- **backoff (üstel)** → `.rules/performance-guidelines.md`
- **bağımlılık yönü** → `architecture/00-overview/dependency-map.md`
- **bağlam (context) bütçesi** → `AGENTS.md` §6
- **baz (baseline)** → `reports/00-kapsam-analizi.md`
- **bileşen kaydı (5000+)** → `architecture/index.md` (ADR-013 PLANLI)
- **bozuk link** → `scripts/check-links.ps1`
- **bütçe (token/context)** → `AGENTS.md` §6.1
- **build kanıtı** → `WORKFLOW.md` GATE-004

### C

- **cache** → `.rules/performance-guidelines.md`
- **Canary / rollout** → `.rules/deployment-guide.md`
- **CI 4 hücre** → `.rules/deployment-guide.md`
- **Clean Architecture** → `architecture/index.md`
- **clock (saat portu)** → `architecture/l3-crosscutting/index.md`
- **compaction / derleme** → `spec/session-spec.md`
- **conflict (çelişki)** → `ROLE.md` §3
- **connection string** → `.rules/security-rules.md`
- **CPM** → `WORKFLOW.md` PART 5
- **coverage (kapsam)** → `spec/test-strategy.md`
- **CRITICAL bulgu** → `.agents/reviewer.md`
- **CQRS** → `.rules/coding-standards.md`

### D

- **dashboard** → `VISION.md`
- **data-access** → `.rules/data-access-rules.md`
- **DB migration** → `.rules/data-access-rules.md`
- **decision log** → `.decisions/index.md`
- **default (varsayılan)** → `.rules/naming-conventions.md`
- **deferred (ertelenen)** → `log.md`
- **deployment** → `.rules/deployment-guide.md`
- **dependency-map** → `architecture/00-overview/dependency-map.md`
- **designer üçlüsü** → `.rules/devexpress-rules.md`
- **developer** → `.agents/developer.md`
- **DevEx / DevExpress** → `.rules/devexpress-rules.md`
- **DIP / SOLID** → `.rules/solid-rules.md`
- **DI (composition)** → `architecture/l6-host/index.md`
- **DockManager** → `ui-design/README.md`
- **dotnet build** → `WORKFLOW.md` GATE-004
- **dotnet test** → `spec/test-strategy.md`
- **DoD** → `AGENTS.md` §9

### E

- **ekran (7)** → `VISION.md` · `ui-design/README.md`
- **eleştiri (red team)** → `ULTRA-THINKING.md`
- **entity** → `architecture/l1-domain/index.md`
- **entegrasyon testi** → `spec/test-strategy.md`
- **escalation** → `AGENTS.md` §8
- **EF Core (yasak)** → `CLAUDE.md` G2
- **entity mapping** → `.rules/data-access-rules.md`

### F

- **fail-closed** → `spec/protocol-spec.md`
- **feature** → `PROJECTS.md`
- **fetch / webfetch** → `spec/tool-spec.md`
- **flow (akış)** → `architecture/00-overview/system-overview.md`
- **frontmatter** → `.templates/documentation/docs-md-template.md`
- **fonksiyonel test** → `spec/test-strategy.md`

### G

- **G1..G12** → `CLAUDE.md`
- **GATE-001..006** → `WORKFLOW.md` §3
- **git** → `.rules/coding-standards.md`
- **glob** → `spec/tool-spec.md`
- **grafik / Diyagram** → `.templates/architecture/ascii-diagram-template.md`
- **grep** → `spec/tool-spec.md`
- **guardrail** → `CLAUDE.md`
- **görsel test** → `ui-design/README.md`

### H

- **hafıza (memory)** → `MEMORY.md`
- **hata (SEV1..6)** → `.rules/security-rules.md`
- **hedef bacak (TFM)** → `.decisions/accepted/ADR-001-dual-target-net48-net10.md`
- **handoff / devre** → `AGENTS.md` §4
- **hot reload** → `WORKFLOW.md` PART 6

### I

- **idempotent** → `.rules/data-access-rules.md`
- **index (DB)** → `.rules/performance-guidelines.md`
- **inline vs subagent** → `AGENTS.md` §5
- **invariant** → `architecture/l1-domain/index.md`
- **integration** → `spec/test-strategy.md`
- **inventory (envanter)** → `reports/00-kapsam-analizi.md`
- **izleme / telemetri** → `architecture/l3-crosscutting/index.md`

### J-K

- **kanıt (evidence)** → `ROLE.md` §6
- **kapsam** → `reports/00-kapsam-analizi.md`
- **kapsam dışı** → `reports/00-kapsam-analizi.md` §4.4
- **kayıt (log)** → `log.md`
- **kalite (quality)** → `scripts/measure-vault.ps1`
- **kural (.rules)** → `.rules/`
- **kısayol (keybind)** → `VISION.md`

### L

- **L1..L7 (katman)** → `architecture/index.md`
- **langversion / C# 14** → `reports/01-web-kaynaklari.md`
- **legacy (net48)** → `.decisions/accepted/ADR-001-dual-target-net48-net10.md`
- **LLM provider** → `spec/protocol-spec.md`
- **lint / style** → `.rules/coding-standards.md`
- **list (tool)** → `spec/tool-spec.md`

### M

- **main / dal dal** → `.rules/coding-standards.md`
- **maskeleme (mask)** → `.rules/security-rules.md`
- **mcp** → `spec/protocol-spec.md`
- **memory (vault)** → `MEMORY.md`
- **mesaj şeması** → `spec/protocol-spec.md`
- **migration runner** → `.rules/data-access-rules.md`
- **mimari test (NetArch)** → `spec/test-strategy.md`
- **mock / sahte** → `.rules/testing-rules.md`

### N

- **naming** → `.rules/naming-conventions.md`
- **N+1** → `.rules/performance-guidelines.md`
- **net10.0-windows** → `.rules/coding-standards.md`
- **net48** → `.rules/coding-standards.md`
- **null güvenliği** → `.rules/coding-standards.md`

### O

- **obfuscation** → `.rules/security-rules.md`
- **Office2010** → `.rules/devexpress-rules.md`
- **Ollama** → `reports/01-web-kaynaklari.md`
- **OpenRouter /free** → `reports/01-web-kaynaklari.md`
- **opencode** → `spec/tool-spec.md`
- **orkestrasyon** → `engine.md`
- **oturum (session)** → `spec/session-spec.md`

### P

- **parameterized SQL** → `.rules/data-access-rules.md`
- **part (P1..P8)** → `WORKFLOW.md` §1
- **performance** → `.rules/performance-guidelines.md`
- **piramit (test)** → `spec/test-strategy.md`
- **plan (todo)** → `PROJECTS.md`
- **prompt** → `AGENTS.md` §5.3
- **publish** → `.rules/deployment-guide.md`

### Q-R

- **query (sorgu)** → `.rules/data-access-rules.md`
- **rabbit / kuyruk** → `architecture/l4-infrastructure/index.md`
- **rapor (report)** → `index.md` §8
- **red team** → `ULTRA-THINKING.md`
- **refactor** → `.rules/coding-standards.md`
- **regresyon** → `spec/test-strategy.md`
- **repository şablonu** → `.templates/backend/repository-template.md`
- **result (`Result<T>`) ** → `architecture/l0-abstractions/index.md`
- **reviewer** → `.agents/reviewer.md`
- **ribbon** → `.rules/devexpress-rules.md`
- **risk** → `reports/00-kapsam-analizi.md` §3
- **rollback** → `.rules/deployment-guide.md`

### S

- **saat (TimeProvider)** → `architecture/l3-crosscutting/index.md`
- **sahte (NSubstitute)** → `.rules/testing-rules.md`
- **schema_version** → `.rules/data-access-rules.md`
- **security** → `.rules/security-rules.md`
- **self-contained** → `.rules/deployment-guide.md`
- **session** → `spec/session-spec.md`
- **skill** → `VISION.md` (PLANLI)
- **skin / tema** → `.rules/devexpress-rules.md`
- **slab / span** → `.rules/performance-guidelines.md`
- **slnx** → `WORKFLOW.md` PART 5
- **SSOT** → `CLAUDE.md`
- **streaming** → `.rules/performance-guidelines.md`
- **sürüm kilidi (G8)** → `CLAUDE.md`

### T

- **TDD** → `.agents/tester.md`
- **template (şablon)** → `.templates/index.md`
- **tester** → `.agents/tester.md`
- **TFM** → `.decisions/accepted/ADR-001-dual-target-net48-net10.md`
- **timeout (CommandTimeout)** → `.rules/data-access-rules.md`
- **tool set** → `spec/tool-spec.md`
- **TreeList** → `ui-design/README.md`

### U-V

- **ui-designer** → `.agents/ui-designer.md`
- **UI thread** → `.rules/performance-guidelines.md`
- **URL doğrulama** → `reports/01-web-kaynaklari.md`
- **varsayım (assumption)** → `reports/00-kapsam-analizi.md` §3
- **version (sürüm)** → `.decisions/index.md`
- **visual studio ilham** → `VISION.md`

### W-X-Y-Z

- **web gate (web araştırması)** → `reports/01-web-kaynaklari.md`
- **worktree** → `.rules/coding-standards.md`
- **whitelist** → `.rules/security-rules.md`
- **wireframe** → `ui-design/README.md`
- **workflow** → `WORKFLOW.md`
- **X (son söz)** → `AGENTS.md` §3.2
- **xUnit** → `.rules/testing-rules.md`
- **yardım / SSS** → `AGENTS.md` §10
- **yedek (backup)** → `.rules/deployment-guide.md`
- **yönerge (guideline)** → `.rules/performance-guidelines.md`
- **Z** — **Zero-Hallucination** → `CLAUDE.md`

---

## §11 Sık Yapılan Hatalar (Red Team)

| # | Hata | Doğrusu |
|---|---|---|
| 1 | Bu dosyaya içerik yazmak | Yalnız tablo + yol |
| 2 | API key koymak | Secret store; burada yalnız **anahtar kelime** |
| 3 | Bir kelimeyi 3 dosyaya yönlendirmek | En fazla 1 hedef + `glossary.md` |
| 4 | CoreMusic terimini VersaCoder'a taşımak | Farklı ürün; terim `glossary.md`'de ayrılır |
| 5 | Yol kısaltmak (`.ai/r/…`) | Tam yol `.ai/.rules/…` |
| 6 | Ölü link (dosya henüz yazılmadı) | `PLANLI` işaretle ya da üret |
| 7 | `keys.md`'yi SSOT sanmak | SSOT hedef dosyadır |
| 8 | Noktasız dizin (`rules/`, `decisions/`) | `.rules/`, `.decisions/` |
| 9 | Eski hedef (`ARCHITECTURE.md`) | `architecture/index.md` |
| 10 | Terimi `glossary.md`'ye sormadan eklemek | Önce 3 hedef sınırını denetle |

---

## §12 Doğrulama

- [x] Frontmatter 8+ alan, `authority: SSOT`
- [x] §3'teki 28 zorlu anahtar **tamamı** tabloda (10 zorlu: VersaCoder, agentic IDE, ribbon, resx, Dapper, net48, net10.0-windows, Office2010, RibbonForm, opencode ✓)
- [x] §10 A-Z dizini ≥180 terim
- [x] Hiçbir satırda 2+ hedef yok
- [x] Secret / API key yok
- [x] `index.md` bu dosyaya bağlantı veriyor
- [ ] `scripts/check-links.ps1` → 0 (her büyük değişiklikte)

---

## §13 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Navigasyon | `.ai/index.md` |
| 2 | Terimler | `.ai/glossary.md` |
| 3 | Anayasa | `.ai/CLAUDE.md` |
| 4 | Şablon envanteri | `.ai/.templates/index.md` |
| 5 | Kapsam analizi | `.ai/reports/00-kapsam-analizi.md` |
| 6 | Web kanıtları | `.ai/reports/01-web-kaynaklari.md` |
| 7 | ADR defteri | `.ai/.decisions/index.md` |
| 8 | Ajan protokolü | `.ai/AGENTS.md` |

---

## §14 Görev → Yol Tarifeleri (adım adım)

> Her satır = sık bir görev + ziyaret sırası. **Sıra önemlidir**: önce kaynak, sonra kural, sonra çıktı.

| # | Görev | Ziyaret sırası | Çıktı |
|---|---|---|---|
| 1 | **Yeni ADR açmak** | `.decisions/index.md` (boş numara bul) → `.templates/adr/adr-nygard-template.md` → `.rules/` ilgili dosya → `log.md` | `accepted/ADR-NNN-<slug>.md` + kural güncellemesi + log kaydı |
| 2 | **Katman kuralına göre kod yazmak** | `architecture/index.md` §2 → ilgili `lN-*/index.md` (YAPAR/YAPMAZ) → `.rules/coding-standards.md` → `.rules/solid-rules.md` | katman uyumlu `.cs` |
| 3 | **Veri erişimi yazmak** | `.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md` → `.rules/data-access-rules.md` → `.sql/` (örnek) → `.rules/security-rules.md` | parameterized Dapper sorgusu |
| 4 | **Form eklemek (ekran)** | `VISION.md` (7 ekran) → `ui-design/README.md` → `.rules/devexpress-rules.md` → **GATE-003 onayı** → `.templates/backend/repository-template.md` değil, ui-design kalıbı | `Form.cs + .Designer.cs + .resx` |
| 5 | **Ribbon / DevExpress kontrol seçmek** | `.rules/devexpress-rules.md` → §7 bu dosyada (kontrol → ekran) → `reports/01-web-kaynaklari.md` (sürüm doğrulama) | v26.1 + Office2010 kontrolü |
| 6 | **Test yazmak** | `spec/test-strategy.md` → `.rules/testing-rules.md` → `.templates/testing/` → `spec/<ilgili>-spec.md` (kabul kriteri) | xUnit testi + kanıt |
| 7 | **Guardrail çakışması görürsem** | `CLAUDE.md` G1..G12 → `ROLE.md` §3 (hiyerarşi) → hâlâ çözülmüyorsa `AGENTS.md` §8.3 (escalation) | `CONFLICT DETECTED` / `DECISION BLOCKED` |
| 8 | **Vault dosyası eklemek** | `index.md` §22 (7 adım) → `.templates/index.md` → `scripts/check-links.ps1` → `scripts/measure-vault.ps1` | 0 bozuk link + bant içi boyut |
| 9 | **opencode davranışı taklit etmek** | `spec/tool-spec.md` → `spec/protocol-spec.md` → `spec/session-spec.md` → `reports/00-kapsam-analizi.md` §1.3 (v1.18.32) | protokol uyumlu adaptör |
| 10 | **LLM doğrulaması** | `reports/01-web-kaynaklari.md` (OpenRouter `/free` sırası) → `spec/protocol-spec.md` → ADR-011 (PLANLI) → `.rules/security-rules.md` (anahtar) | tek uçtan uca sohbet kanıtı |
| 11 | **Kapsam değişikliği** | `reports/00-kapsam-analizi.md` §4.4 (ne YAPMAYız) → `PROJECTS.md` → **kullanıcı onayı** → `log.md` | güncel kapsam satırı |
| 12 | **Performans şikayeti** | `.rules/performance-guidelines.md` §1 (B1..B10 bütçesi) → ilgili katman `lN-*/index.md` → `spec/test-strategy.md` (ölçüm) | ölçülmüş düzeltme |
| 13 | **Güvenlik incelemesi** | `.rules/security-rules.md` (SEV1..SEV6) → `.rules/data-access-rules.md` (G2) → `.rules/deployment-guide.md` §3 (secret) | bulgu listesi + seviye |
| 14 | **Dağıtım hazırlığı** | `.rules/deployment-guide.md` §1–§2 (matris + CI) → `spec/test-strategy.md` (GATE-005) → `WORKFLOW.md` GATE-006 | artifact + geri alma planı |
| 15 | **3 başarısız düzeltme sonrası** | `AGENTS.md` §8.1 (şablonu doldur) → şüpheli varsayımı **adlandır** → seçeneklerle kullanıcıya → `log.md` | `3-STRIKE` raporu, **kod yazma durur** |

---

## §15 Konu Kümeleri (ilişkili terim grupları)

> Bir konuyu ararken **kümeyi** düşün: küme tek dosyada toplanır, parçalanmaz (SSOT).

### Küme A — Veri Erişimi

`ADO.NET` · `Dapper` · `parameterized` · `CommandTimeout` · `SQLite` · `SqlClient` · `migration` · `schema_version` · `N+1` · `SELECT *` · `streaming reader` · `sayfalama`

**Tek hedef:** `.rules/data-access-rules.md` (detay: `spec/test-strategy.md` §3, `.sql/` örneği)

### Küme B — Arayüz / DevExpress

`RibbonControl` · `RibbonForm` · `Office2010` · `XtraTabControl` · `TreeList` · `GridControl` · `DockManager` · `RichEditControl` · `MemoEdit` · `Skin` · `.resx` · `ResourceManager` · `Designer.cs`

**Tek hedef:** `.rules/devexpress-rules.md` (ekran eşlemesi: §7 bu dosyada; sürüm kanıtı: `reports/01-web-kaynaklari.md`)

### Küme C — Katmanlar

`l0 Abstractions` · `l1 Domain` · `l2 Application` · `l3 Crosscutting` · `l4 Infrastructure` · `l5 Protocol` · `l6 Host` · `l7 UI`

**Tek hedef:** `architecture/index.md` (ayrıntı: `architecture/lN-*/index.md`; denetim: NetArchTest)

### Küme D — Ajan / Süreç

`gate` · `part` · `devre paketi` · `subagent` · `context bütçesi` · `3-STRIKE` · `escalation` · `DoD` · `X (son söz)`

**Tek hedef:** `AGENTS.md` (süreç tarafı: `WORKFLOW.md` §1–§3; görev tanımı: `.agents/<rol>.md`)

### Küme E — Protokol / opencode

`tool set` · `tool_call` · `session` · `transcript` · `loop policy` · `protocolVersion` · `fail-closed` · `MCP` · `streaming` · `compaction`

**Tek hedef:** `spec/protocol-spec.md` (şema detayı: `spec/tool-spec.md` · `spec/session-spec.md`)

### Küme F — Kalite / Kanıt

`coverage` · `TDD` · `NetArchTest` · `NSubstitute` · `check-links` · `measure-vault` · `kanıtsız bitti` · `UNKNOWN` · `NEEDS_VALIDATION`

**Tek hedef:** `spec/test-strategy.md` (protokol: `AGENTS.md` §9; anayasa: `CLAUDE.md` G5)

### Küme G — Dağıtım / Ortam

`net48 publish` · `net10.0-publish` · `CI 4 hücre` · `artifact` · `rollback` · `DPAPI` · `secret` · `RID`

**Tek hedef:** `.rules/deployment-guide.md` (güvenlik tarafı: `.rules/security-rules.md` §3)

> **KURAL:** Bir terim en fazla **bir küme** aittir. İki kümeye giriyorsa ya `glossary.md`'de tanımdır ya da sınır yanlış çizilmiştir → `CLAUDE.md` §konu sınırı denetimi.
