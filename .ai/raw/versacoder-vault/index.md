---
title: "VersaCoder Vault — Master Index"
type: index
category: vault-navigation
status: active
version: 1.1.0
updated: 2026-09-25
authority: SSOT
scope: "Tüm .ai/ vault dosyalarının tek navigasyon noktası — kategori, sorumluluk, boyut, part/gate, katman, ajan ve rapor pusulaları"
---

# VersaCoder Vault — Master Index

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]] · [[ROLE.md]] · [[keys.md]] · [[brain.md]] · [[MEMORY.md]] · [[log.md]] · [[VISION.md]] · [[PROJECTS.md]] · [[glossary.md]] · [[engine.md]] · [[ULTRA-THINKING.md]] · [.templates/index](.templates/index.md) · [.agents/AGENTS](.agents/AGENTS.md)

> **Tek cümlelik sorumluluk:** *Bu vault'ta ne var, nerede, hangi sırada okunur* sorusunun tek cevabıdır.
> İçerik burada **tekrarlanmaz**; yalnız kayıt ve yol gösterilir (SRP). İçerik çelişirse ilgili kaynak dosya kazanır.

---

## §0 Özet (Answer First)

- **91 MD** (14 kök + 77 dizin) · hedef **≥500 MD** (ADR-003) · kök belgeler **30–88 KB** bandında · **bozuk link = 0** (`scripts/check-links.ps1`).
- Vault **4 soru** yanıtlar: *Ne yapıyoruz* (VISION/PROJECTS) · *Nasıl yapıyoruz* (WORKFLOW/ROLE/engine) · *Hangi kuralla* (CLAUDE/brain/.rules) · *Nerede bulurum* (bu dosya + keys.md).
- **Sıra:** [.templates/](.templates/index.md) → [.agents/](.agents/AGENTS.md) → kök MD → [architecture/](architecture/index.md) + [.decisions/](.decisions/index.md) + [.rules/](.rules/) + [spec/](spec/) + [ui-design/](ui-design/README.md) → [reports/](reports/).
- **Yasak:** EF Core · string-concat SQL · DevExpress ≠ v26.1 · Office2010 dışı ribbon · frontmatter'sız MD · kanıtsız "bitti".
- **Sonraki gate:** PART 3 kapanışı (GATE-004a) → PART 4 (**GATE-003 = kullanıcı onayı, HARD STOP**).

---

## §1 Amaç ve Vault Kuralları

| Soru | Tek Kaynak |
|---|---|
| **Ne yapıyoruz?** (vizyon, kapsam) | [[VISION.md]] · [[PROJECTS.md]] |
| **Nasıl yapıyoruz?** (süreç, gate, rol) | [[WORKFLOW.md]] · [[ROLE.md]] · [[engine.md]] |
| **Hangi kuralla?** (teknik kısıt) | [[CLAUDE.md]] · [[brain.md]] · [.rules/](.rules/) |
| **Nerede bulurum?** (arama) | [[keys.md]] · bu dosya |

### §1.1 Vault Kuralları

| # | Kural | İhlalde |
|---|---|---|
| 1 | **SSOT:** her konu **tek** dosyada yaşar; tekrar yasak | Çelişki → ilgili dosya kazanır, tekrar silinir |
| 2 | Her MD **YAML frontmatter** taşır (`title,type,category,status,version,updated,authority,scope`) | G4 RED |
| 3 | **Bozuk link yasak** — link göreli (`../x.md`) ya da Obsidian kökü (`[[x]]`) | `scripts/check-links.ps1` → 0 değilse RED |
| 4 | CoreMusic vault'u **salt okunur**, yalnız **yapı** referansıdır | G1 RED |
| 5 | Yedek vault (`versacoder.ai`) yalnız **bilgi** kaynağıdır — kopyalama yok | G1 RED |
| 6 | Kanıtsız "tamam" yasak — build/test çıktısı gerek | G5 RED |
| 7 | Vault'a yazım **part sırasına** göre ([WORKFLOW.md](WORKFLOW.md) §gate) | Sıra ihlali |
| 8 | **Dizin adları noktalı:** `.agents .templates .decisions .rules .sql .obsidian` | Nokta unutulursa link kırılır |

---

## §2 Hızlı Referans (İhtiyaç → İlk Adım)

| İhtiyaç | İlk Adım | Süre |
|---|---|---|
| Vault genel bakış | Bu dosya | ≈2 dk |
| Arama / anahtar kelime | [[keys.md]] | ≈1 dk |
| AI anayasası, guardrail G1..G12 | [[CLAUDE.md]] | ≈3 dk |
| Süreç + gate + part sırası | [[WORKFLOW.md]] | ≈3 dk |
| Rolüm ne, kime devrediyorum | [[ROLE.md]] · [.agents/AGENTS](.agents/AGENTS.md) | ≈2 dk |
| Devre paketi biçimi, 3-STRIKE | [[AGENTS.md]] §4, §8 | ≈2 dk |
| Mimari kararlar / kısıtlar | [[brain.md]] · [.decisions/index](.decisions/index.md) | ≈3 dk |
| Katman haritası (l0..l7) | [architecture/index](architecture/index.md) | ≈3 dk |
| ADR listesi (hangi karar) | [.decisions/index](.decisions/index.md) | ≈2 dk |
| Hard kural (.rules) | [.rules/](.rules/) (9 dosya) | ≈2 dk |
| Şartname (protokol/tool/session) | [spec/](spec/) (5 dosya) | ≈5 dk |
| Ürün vizyonu, 7 ekran | [[VISION.md]] · [ui-design/README](ui-design/README.md) | ≈3 dk |
| Kapsam, bileşenler, riskler | [[PROJECTS.md]] · [reports/00-kapsam-analizi](reports/00-kapsam-analizi.md) | ≈5 dk |
| Web kanıtları / sürüm doğrulama | [reports/01-web-kaynaklari](reports/01-web-kaynaklari.md) | ≈3 dk |
| Oturum arası bellek | [[MEMORY.md]] | ≈2 dk |
| Değişiklik günlüğü | [[log.md]] | ≈2 dk |
| Orkestrasyon matrisi | [[engine.md]] | ≈2 dk |
| Derin düşünme protokolü | [[ULTRA-THINKING.md]] | ≈3 dk |
| Terim sözlüğü (TR/EN) | [[glossary.md]] | — |
| Şablon bul | [.templates/index](.templates/index.md) | ≈1 dk |
| Ajan bul | [.agents/AGENTS](.agents/AGENTS.md) | ≈1 dk |
| Link taraması / ölçüm | [scripts/check-links.ps1](scripts/check-links.ps1) · [measure-vault.ps1](scripts/measure-vault.ps1) | ≈1 dk |
| SQL örneği (parameterized) | [.sql/](.sql/) | — |
| Kapsam analizi (karar/risk) | [reports/00-kapsam-analizi](reports/00-kapsam-analizi.md) | ≈5 dk |

---

## §3 Kök Envanter (14 Belge)

| # | Dosya | Tek Sorumluluk | `type` | Hedef KB |
|---|---|---|---|---|
| 1 | [[index.md]] | Navigasyon (bu dosya) | `index` | 30–45 |
| 2 | [[AGENTS.md]] | Ajan protokolü: devre, subagent, 3-STRIKE, DoD | `registry` | 30–45 |
| 3 | [[CLAUDE.md]] | AI anayasası — G1..G12 guardrail | `constitution` | 30–60 |
| 4 | [[WORKFLOW.md]] | Süreç, part sırası, GATE-001..006 | `workflow` | 30–65 |
| 5 | [[ROLE.md]] | Mutlak rol + öncelik çatışma kuralı | `role` | 30–50 |
| 6 | [[keys.md]] | Anahtar kelime → hedef dosya haritası | `navigation` | 30–70 |
| 7 | [[glossary.md]] | TR/EN terim sözlüğü | `reference` | 30–50 |
| 8 | [[brain.md]] | ADR özetleri + mühendislik kısıtları | `decisions` | 30–88 |
| 9 | [[MEMORY.md]] | Oturumlar arası kalıcı bellek | `memory` | 30–50 |
| 10 | [[VISION.md]] | Vizyon + problem–çözüm matrisi + 7 ekran | `vision` | 30–50 |
| 11 | [[PROJECTS.md]] | Kapsam, hedefler, bileşen envanteri | `project` | 30–50 |
| 12 | [[log.md]] | Kural değişikliği + kalite raporu günlüğü | `log` | 30–60 |
| 13 | [[engine.md]] | Gate ↔ part orkestrasyon matrisi | `engine` | 30–50 |
| 14 | [[ULTRA-THINKING.md]] | Derin düşünme protokolü | `protocol` | 30–45 |

**Hedef toplam:** 14 dosya · **~500–800 KB** · 100% frontmatter'lı (bugün: 12/14 bantta, `index.md` + `keys.md` bu sürümle banda alındı).

---

## §4 Dizin Haritası (11 dizin, 77 MD)

```
.ai/                                        91 MD
│
├── index.md  AGENTS.md  CLAUDE.md  WORKFLOW.md  ROLE.md
├── keys.md   glossary.md  brain.md  MEMORY.md  VISION.md
├── PROJECTS.md  log.md  engine.md  ULTRA-THINKING.md     (14 kök)
│
├── .templates/            33 MD   ─ üretim kalıpları (PART 3, adım 1 ✓)
│     adr/  agents/  architecture/  backend/  documentation/
│     frontend/  infrastructure/  query/  testing/  ui-design/
│
├── .agents/                6 MD   ─ 5 ajan tanımı + ajan kaydı (adım 2 ✓)
│     AGENTS.md  architect.md  developer.md  reviewer.md
│     tester.md  ui-designer.md
│
├── architecture/          11 MD   ─ katman şartnameleri + diyagramlar (adım 4 ✓)
│     index.md
│     00-overview/  system-overview.md · dependency-map.md
│     l0-abstractions/ l1-domain/ l2-application/ l3-crosscutting/
│     l4-infrastructure/ l5-protocol/ l6-host/ l7-ui/  (index.md)
│
├── .decisions/             7 MD   ─ ADR defteri (adım 4 ✓)
│     index.md · adr-index.md
│     accepted/ADR-001..ADR-005 (+ ADR-006..013 planlı)
│
├── .rules/                 9 MD   ─ hard guardrail karşılıkları (adım 4 ✓)
│     coding-standards · data-access-rules · deployment-guide
│     devexpress-rules · naming-conventions · performance-guidelines
│     security-rules · solid-rules · testing-rules
│
├── spec/                   5 MD   ─ şartnameler (adım 4 ✓)
│     versacoder-spec · protocol-spec · session-spec
│     tool-spec · test-strategy
│
├── ui-design/              1 MD   ─ 7 ekran wireframe (PART 4 — GATE-003)
│     README.md  →  01-start-page … 07-settings (planlı)
│
├── reports/                2 MD   ─ analiz + teslim kanıtları
│     00-kapsam-analizi.md ✓ · 01-web-kaynaklari.md ✓
│     02..08-<part>-raporu (planlı) · 09-teslim-raporu (planlı)
│
├── scripts/                4 dosya─ check-links.ps1/.md · measure-vault.ps1/.md
├── .sql/                   1 MD   ─ migration + parameterized örnekler
└── .obsidian/                    ─ Obsidian ayarları
```

---

## §5 Katman Pusulası (l0..l7)

| lN | Katman | Proje | Tek cümle sorumluluk | İzinli referans |
|---|---|---|---|---|
| **l0** | Abstractions | `VersaCoder.Abstractions` | `Result<T>`, `ErrorCode`, port arayüzleri — implementasyon yok | yalnız BCL |
| **l1** | Domain | `VersaCoder.Domain` | Entity, value object, invariant — saf C# | l0, l3 |
| **l2** | Application | `VersaCoder.Application` | Use case, command/query handler, DTO | l0, l1, l3 |
| **l3** | Crosscutting | `VersaCoder.Crosscutting` | Logging, cache, clock, şifreleme, config, retry | yalnız l0 |
| **l4** | Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, Sqlite/SqlClient, migration, dosya, git, LLM | l0, l1, l3 |
| **l5** | Protocol | `VersaCoder.Protocol` | opencode adaptörü: tool şeması, session, mesaj tipleri | l0, l1, l2, l3 |
| **l6** | Host | `VersaCoder.Host` | Composition root: DI, config, açılış sırası, `#if` köprüsü | l0..l5 |
| **l7** | UI + CLI | `VersaCoder.Desktop` / `.Cli` | 7 ekran (WinForms + DevExpress v26.1) ve CLI sunumu | l0, l2, l3, l5, l6 |

```
            tek yon:  l7 --> l6 --> l5 --> l4 --> l2 --> l1 --> l0
                                     |                 ^
                                     +-------------> l3 +  (paylasilan, yalniz l0'a bakar)

   ┌──────────────────────────────────────────────────────────────────────┐
   │  l7 UI/CLI ──▶ l6 Host ──▶ l5 Protocol ──▶ l4 Infrastructure        │
   │      │             │            │                 │                 │
   │      │             └────────────┴───────┬─────────┘                 │
   │      │                                  ▼                           │
   │      └──────────▶ l2 Application ──▶ l1 Domain ──▶ l0 Abstractions  │
   │                        │                │                 ▲         │
   │                        └──── l3 Crosscutting ─────────────┘         │
   └──────────────────────────────────────────────────────────────────────┘
```

**Ayrıntı:** [architecture/index](architecture/index.md) · [dependency-map](architecture/00-overview/dependency-map.md) · [system-overview](architecture/00-overview/system-overview.md) · her katman: `architecture/lN-*/index.md`.

---

## §6 Part ve Gate Pusulası

| Part | Üretim | Vault hedefi | Gate | Durum |
|---|---|---|---|---|
| 1 | Kaynak okuma | okuma notu → rapor | **GATE-001** | ✅ kapandı (4/4) |
| 2 | Kapsam analizi | [reports/00-kapsam-analizi](reports/00-kapsam-analizi.md) v1.1.0 | **GATE-002** | ✅ kapandı (kapsam + §4.4) |
| 3 | Vault kurulumu | `.templates/` → `.agents/` → kök → katmanlar | **GATE-004a** (vault yeşil) | 🔄 %80 — link 0 ✓, boy bandı ✓, katalog (52 düğüm/5000+ bileşen) bekliyor |
| 4 | UI + ASCII art | `ui-design/01..07` | **GATE-003** | ⏳ **HARD STOP — kullanıcı onayı** |
| 5 | Proje iskeleti | `VersaCoder.slnx` + `Directory.*.props` + formlar | **GATE-004** | ⏳ |
| 6 | Kodlama (L1→L7) | `.rules/` + `log.md` + test | **GATE-005** | ⏳ |
| 7 | opencode adaptasyonu | `spec/protocol-spec.md` + ADR-006..011 | — | ⏳ |
| 8 | Doğrulama | [reports/09-teslim-raporu] (planlı) | **GATE-006** (kapanış) | ⏳ |

> **KURAL:** Bir part bitmeden diğeri başlamaz ([WORKFLOW.md](WORKFLOW.md) §1). Gate onayı **kullanıcıya** aittir (AGENTS.md §3.2 #10).

---

## §7 Ajan Pusulası

| Ajan | Tek cümle | X (son söz) | Devre → |
|---|---|---|---|
| [architect](.agents/architect.md) | Sistemin **nasıl** kurulacağı + geri alınabilir karar | `.decisions/` · `architecture/` · `.rules/` · `spec/` | `developer` |
| [developer](.agents/developer.md) | Sözleşmeyi **derlenen koda** çevirir | `src/**/*.cs` imzası | `reviewer` |
| [reviewer](.agents/reviewer.md) | Kod **geçti mi** — PASS/RED | PASS/RED + bulgu seviyesi | PASS → `tester` · RED → `developer` |
| [tester](.agents/tester.md) | Davranışın **kanıtını** üretir | `tests/**` · coverage | `reviewer` |
| [ui-designer](.agents/ui-designer.md) | Ekran **görünümünü + kontrol setini** kilitler | `ui-design/**` · `.Designer.cs` · `.resx` | `developer` (yalnız GATE-003 sonrası) |

```
architect ──ADR+sözleşme──▶ developer ──kod+build──▶ reviewer
                               ▲                        │
                               │        RED ────────────┤
                               └──────────┘             │ PASS
                                                        ▼
        ui-designer ──GATE-003 onaylı wireframe──▶ developer   tester ──▶ rapor
```

**Protokol:** [AGENTS.md](AGENTS.md) §4 devre paketi (4 madde) · §5 subagent/inline · §6 context bütçesi · §8 3-STRIKE.

---

## §8 Rapor İndeksi

| # | Rapor | İçerik | Durum |
|---|---|---|---|
| 1 | [00-kapsam-analizi](reports/00-kapsam-analizi.md) | Girdi envanteri, 11 senior karar, riskler, gate tablosu, §4.4 kapsam dışı | ✅ v1.1.0 |
| 2 | [01-web-kaynaklari](reports/01-web-kaynaklari.md) | 8 kaynak, URL + erişim 2026-09-25, çelişki kayıtları C1..C6 | ✅ |
| 3 | 02..08-`<part>`-raporu | Part bazlı kanıt (build/test/link) | ⏳ |
| 4 | 09-teslim-raporu | Ne çalıştı, kanıt, kalan işler | ⏳ |

---

## §9 Hızlı Erişim (Ne arıyorsun → dosya)

| Aranan | Git |
|---|---|
| G1..G12 guardrail metni | [[CLAUDE.md]] |
| GATE-001..006 tanımı | [[WORKFLOW.md]] §3 |
| Devre paketi şablonu | [[AGENTS.md]] §4.2 |
| 3-STRIKE / escalation | [[AGENTS.md]] §8 |
| ADR yazım şablonu | [.templates/adr/adr-nygard-template](.templates/adr/adr-nygard-template.md) |
| ADR-001 iki bacak | [.decisions/accepted/ADR-001-dual-target-net48-net10](.decisions/accepted/ADR-001-dual-target-net48-net10.md) |
| ADR-004 DevExpress v26.1 | [.decisions/accepted/ADR-004-devexpress-universal-v261-office2010](.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) |
| ADR-005 ADO.NET + Dapper | [.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core](.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) |
| EF Core yasağı kuralı | [.rules/data-access-rules](.rules/data-access-rules.md) |
| Kod stili / async | [.rules/coding-standards](.rules/coding-standards.md) |
| İsimlendirme | [.rules/naming-conventions](.rules/naming-conventions.md) |
| SOLID | [.rules/solid-rules](.rules/solid-rules.md) |
| Güvenlik / secret | [.rules/security-rules](.rules/security-rules.md) |
| DevExpress kuralları | [.rules/devexpress-rules](.rules/devexpress-rules.md) |
| Test kuralları | [.rules/testing-rules](.rules/testing-rules.md) |
| Test stratejisi / kapsam | [spec/test-strategy](spec/test-strategy.md) |
| Performans bütçeleri | [.rules/performance-guidelines](.rules/performance-guidelines.md) |
| Dağıtım / CI / rollback | [.rules/deployment-guide](.rules/deployment-guide.md) |
| Protokol mesaj şeması | [spec/protocol-spec](spec/protocol-spec.md) |
| Oturum/transcript | [spec/session-spec](spec/session-spec.md) |
| Tool sözleşmesi | [spec/tool-spec](spec/tool-spec.md) |
| Ürün şartnamesi | [spec/versacoder-spec](spec/versacoder-spec.md) |
| Bağımlılık matrisi 8×8 | [architecture/00-overview/dependency-map](architecture/00-overview/dependency-map.md) |
| Sistem genel bakış | [architecture/00-overview/system-overview](architecture/00-overview/system-overview.md) |
| Katman kuralı (hangi lN kime bakar) | [architecture/index](architecture/index.md) |
| 7 ekran listesi | [[VISION.md]] · [ui-design/README](ui-design/README.md) |
| Kontrol adayları (Ribbon, Grid…) | [reports/00-kapsam-analizi](reports/00-kapsam-analizi.md) §4.1 |
| DevExpress sürüm doğrulaması | [reports/01-web-kaynaklari](reports/01-web-kaynaklari.md) |
| Anahtar kelime → dosya | [[keys.md]] |
| Terim anlamı | [[glossary.md]] |
| Son karar özeti | [[brain.md]] |
| Oturum durumu | [[MEMORY.md]] · [[log.md]] |
| Vault boyutu ölçümü | [scripts/measure-vault.ps1](scripts/measure-vault.ps1) |
| Link denetimi | [scripts/check-links.ps1](scripts/check-links.ps1) |
| SQL örneği | [.sql/](.sql/) |
| Şablon listesi | [.templates/index](.templates/index.md) |
| Ajan yetki matrisi | [.agents/AGENTS](.agents/AGENTS.md) §3 |

---

## §10 Karar Pusulası (ADR)

| ADR | Konu | Karar özeti | Durum |
|---|---|---|---|
| [001](.decisions/accepted/ADR-001-dual-target-net48-net10.md) | dual-target | Tek projede `net48` + `net10.0-windows`; ortak kaynak | ✅ accepted |
| [002](.decisions/accepted/ADR-002-handwritten-content-no-copypaste.md) | handwritten | Yedek içerik **elle sıfırdan**; COPY=NO | ✅ accepted |
| [003](.decisions/accepted/ADR-003-vault-structure-500md-frontmatter.md) | vault-structure | ≥500 MD · 8 alan frontmatter · 30–88 KB · **0 bozuk link** | ✅ accepted |
| [004](.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) | devexpress v26.1 | Tek sürüm + `RibbonControlStyle.Office2010`; web doğrulandı | ✅ accepted |
| [005](.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) | ado-net-dapper | EF Core YOK; parameterized; SqlClient **5.x** iki bacak | ✅ accepted |
| 006..011 | tool / session / context / mcp / skill-memory / llm-provider | opencode adaptasyonu (PART 7) | ⏳ planlı |
| 012 | dual-tfm-data-provider | SqlClient 5.x iki bacak; Sqlite `NEEDS_VALIDATION` | ⏳ PART 5 |
| 013 | layered-architecture-52-nodes | **7 makro × 45 alt katman = 52 düğüm**, ≥5000 bileşen kaydı | ⏳ PART 3 katalog |

**Defter:** [.decisions/index](.decisions/index.md) · [adr-index](.decisions/adr-index.md)

---

## §11 Kurallar Pusulası (.rules, 9 dosya)

| Dosya | Neyi zorunlu kılar | Bağlı guardrail |
|---|---|---|
| [coding-standards](.rules/coding-standards.md) | Stil, `async`, expression body, `nameof` | G6 |
| [data-access-rules](.rules/data-access-rules.md) | EF Core YOK, parameterized, `CommandTimeout` | **G2** |
| [deployment-guide](.rules/deployment-guide.md) | İki bacak publish, CI 4 hücre, rollback planı | G6 |
| [devexpress-rules](.rules/devexpress-rules.md) | Yalnız v26.1, `Office2010`, designer üçlüsü | **G3** |
| [naming-conventions](.rules/naming-conventions.md) | Dosya/tip/metot adlandırma | G4 |
| [performance-guidelines](.rules/performance-guidelines.md) | Bütçeler B1..B10, N+1/`SELECT *` yasağı | G6 |
| [security-rules](.rules/security-rules.md) | Secret, DPAPI, injection | G7 |
| [solid-rules](.rules/solid-rules.md) | SRP/DIP, katman yönü | G1 |
| [testing-rules](.rules/testing-rules.md) | xUnit/NSubstitute/NetArchTest, kapsam ≥ %90 | G5 |

---

## §12 Spec Pusulası (5 dosya)

| Dosya | Kapsam |
|---|---|
| [versacoder-spec](spec/versacoder-spec.md) | Ürün kapsamı, 7 ekran, kabul kriterleri |
| [protocol-spec](spec/protocol-spec.md) | Mesaj şeması, `protocolVersion`, fail-closed |
| [session-spec](spec/session-spec.md) | Oturum/transcript, derleme, loop policy |
| [tool-spec](spec/tool-spec.md) | `read/write/edit/bash/grep/glob/list/...` sözleşmesi |
| [test-strategy](spec/test-strategy.md) | Piramit, iki bacak test matrisi, GATE-005 kanıtları |

---

## §13 Bilinen Eksikler (dürüst liste)

| # | Eksik | Neden | Plan |
|---|---|---|---|
| 1 | `architecture/catalog/` (52 düğüm · ≥5000 bileşen kaydı) | ADR-013 henüz yazılmadı | PART 3 — sıradaki iş |
| 2 | `ui-design/01..07` wireframe'leri | GATE-003 öncesi taslak | PART 4 |
| 3 | `reports/02..09` part raporları | part'lar henüz çalışmadı | PART 4..8 |
| 4 | `md` sayısı 91 (hedef ≥500) | katalog üretilmedi | ADR-003 + ADR-013 ile |
| 5 | `Microsoft.Data.Sqlite` ikili destek | NuGet teyidi gerekli | PART 5 → ADR-012 (`NEEDS_VALIDATION`) |
| 6 | `.obsidian/` boş | ayar dosyaları parti bekliyor | PART 3 kapanışı |

---

## §14 Doğrulama

- [x] Frontmatter 8 alan + `authority: SSOT`
- [x] 14 kök belge mevcut; `index.md` + `keys.md` 30–88 KB bandına alındı
- [x] `scripts/check-links.ps1` → **0 bozuk link** (91 MD / 700+ link)
- [x] `.agents/AGENTS.md` ↔ `.agents/` dosya sayısı eşit (5+1)
- [x] `.rules/` = 9 dosya · `.decisions/` = 7 dosya · `spec/` = 5 dosya
- [ ] `architecture/catalog/` → 52 düğüm + ≥5000 bileşen (ADR-013)
- [ ] Toplam MD ≥ 500 (ADR-003)
- [ ] `.templates/index.md` disk ölçümü ile tutarlı

---

## §15 Değişiklik Günlüğü

| Tarih | Sürüm | Değişiklik |
|---|---|---|
| 2026-09-25 | 1.0.0 | İlk yazım (vault kurulumu PART 3) |
| 2026-09-25 | 1.1.0 | §0 Özet, §4 dizin haritası, §5 katman pusulası, §6 part/gate, §7 ajan pusulası, §9 hızlı erişim, §10 ADR pusulası, §11–12 kurallar/spec pusulaları, §13 bilinen eksikler eklendi; `ARCHITECTURE.md` bağlantıları `architecture/index.md`'ye yeniden hedeflendi; noktalı dizin adları (`.decisions`, `.rules`) düzeltildi |

---

## §16 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kapsam analizi | `reports/00-kapsam-analizi.md` |
| 2 | Web kanıtları (WEB GATE) | `reports/01-web-kaynaklari.md` |
| 3 | Bootstrap kaynağı | `G:\Drive'ım\Projelerim\Project-notes\...\Versacoder\` (ai.01/02/03) |
| 4 | Yapı referansı (salt okunur) | `C:\www\coremusic.net\.ai` |
| 5 | Bilgi referansı (kopya yok) | `C:\www\versacoder\versacoder.ai` |
| 6 | opencode referansı | `%TEMP%\opencode-fresh` (v1.18.32, 33 paket) |
| 7 | Proje kökü | `C:\versacoder` |

---

## §17 Ek — Vault Ölçüm Tablosu (2026-09-25)

| Dizin | MD | Boyut | Frontmatter | Görev | Gate |
|---|---:|---:|:---:|---|---|
| kök (14 belge) | 14 | ~480 KB | ✅ 14/14 | rol, anayasa, süreç, arama | GATE-004a |
| `.templates/` | 33 | ~977 KB | ✅ | üretim kalıpları | PART 3.1 |
| `.agents/` | 6 | ~30 KB | ✅ 6/6 | ajan tanımları (5+1) | PART 3.2 |
| `architecture/` | 11 | ~151 KB | ✅ | katman şartnameleri | PART 3.4 |
| `.decisions/` | 7 | ~90 KB | ✅ | ADR defteri (001..005) | PART 3.4 |
| `.rules/` | 9 | ~115 KB | ✅ | hard kurallar | PART 3.4 |
| `spec/` | 5 | ~67 KB | ✅ | şartnameler | PART 3.4 |
| `reports/` | 2 | ~20 KB | ✅ | analiz + web kanıtı | GATE-001/002 |
| `scripts/` | 2 (+2 `.ps1`) | ~12 KB | ✅ | link tarama + ölçüm | GATE-004a |
| `.sql/` | 1 | ~5 KB | ✅ | parameterized örnek | PART 3.4 |
| `ui-design/` | 1 | ~2 KB | ✅ | wireframe girişi | **GATE-003** |
| `.obsidian/` | 0 | — | — | Obsidian ayarları | PART 3 kapanış |
| **TOPLAM** | **91** | **~1,96 MB** | **✅ 91/91** | hedef **≥500 MD** | ADR-003 |

**Kalite ölçütleri:**

| # | Ölçüt | Bugün | Hedef | Araç |
|---|---|---|---|---|
| 1 | Bozuk link | **0** | 0 | `scripts/check-links.ps1` |
| 2 | İç link sayısı | **830** | artan | aynı |
| 3 | Kök 30–88 KB bandı | 14/14 | 14/14 | `scripts/measure-vault.ps1` |
| 4 | Frontmatter oranı | 91/91 | 100% | tarama |
| 5 | Toplam MD | 91 | **≥500** | `measure-vault.ps1` |
| 6 | Bileşen kaydı | 0 | **≥5000** | `count-components.ps1` (ADR-013) |

---

## §18 Ek — Yeni Oturum Okuma Sırası

| # | Adım | Dosya | Süre | Ne alınıyor |
|---|---|---|---|---|
| 1 | Rolü al | [[ROLE.md]] | ≈2 dk | kimlik, ses, öncelik çatışması |
| 2 | Kuralları al | [[CLAUDE.md]] | ≈3 dk | G1..G12 (anayasa) |
| 3 | Sırayı al | [[WORKFLOW.md]] + [[engine.md]] | ≈3 dk | hangi part, hangi gate |
| 4 | Belleği al | [[MEMORY.md]] + [[log.md]] | ≈2 dk | son durum, açık işler |
| 5 | Ajan protokolü | [[AGENTS.md]] §4–§8 | ≈2 dk | devre, subagent, 3-STRIKE |
| 6 | Arama yap | [[keys.md]] | ≈1 dk | konuma git |
| 7 | İşe başla | ilgili part çıktısı | — | — |

**≈16 dk** — bu adımlar atlanırsa SSOT ihlali ve **tekrar iş** riski doğar (AGENTS.md §6.2: aynı bilgi 2. kez okunuyorsa yanlış yere yazılıyordur).

> **Bütçe kuralı:** tek turda **≤ 8 dosya** oku; uzun dosyada `grep` ile hedefle (AGENTS.md §6.1).

---

## §19 Ek — Çekirdek Sabitler (bu vault'ta tartışılmaz)

| # | Sabit | Değer | Kaynak |
|---|---|---|---|
| S1 | Proje kökü | `C:\versacoder` | [00-kapsam-analizi](reports/00-kapsam-analizi.md) |
| S2 | Hedef bacak | `net48` + `net10.0-windows` (ikisi de yeşil) | [ADR-001](.decisions/accepted/ADR-001-dual-target-net48-net10.md) |
| S3 | DevExpress | **v26.1** (Universal 2026) tek sürüm | [ADR-004](.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) |
| S4 | Ribbon | yalnız `RibbonControlStyle.Office2010` | ADR-004 |
| S5 | Veri | ADO.NET + **Dapper**; **EF Core = 0** | [ADR-005](.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) |
| S6 | SQL | parameterized + `CommandTimeout`; concat = G2 | [.rules/data-access-rules](.rules/data-access-rules.md) |
| S7 | Migration | saf SQL + `schema_version` + `IMigrationRunner` | ADR-005 |
| S8 | Test | xUnit + NSubstitute + NetArchTest; kapsam ≥ %90 | [spec/test-strategy](spec/test-strategy.md) |
| S9 | Katman | l0..l7, tek yön, `#if` yalnız platform farkı | [architecture/index](architecture/index.md) |
| S10 | Protokol kaynağı | opencode **v1.18.32** (33 paket) | [00-kapsam-analizi](reports/00-kapsam-analizi.md) §1.3 |
| S11 | LLM doğrulama sırası | önce OpenRouter `/free` test modeli → Ollama | [01-web-kaynaklari](reports/01-web-kaynaklari.md) |
| S12 | Vault ölçeği | ≥500 MD · 30–88 KB · 0 bozuk link | [ADR-003](.decisions/accepted/ADR-003-vault-structure-500md-frontmatter.md) |

> **Kaynak hiyerarşisi çelişirse** (ROLE.md §3): kullanıcının güncel talimatı → bu vault'taki SSOT dosyası → dış web kanıtı → plan notu.

---

## §20 Ek — SSS (10 Soru)

**S1 — Hangi dosyadan başlarım?**
Bu dosya (§18 sırası) → sonra [keys.md](keys.md) ile hedefe git. Rastgele dosya okuma yasak (bütçe).

**S2 — Bir kuralı nereden değiştiririm?**
`.rules/` dosyası + `CLAUDE.md` G-maddesi + `log.md` kaydı. Tek başına `.rules/` değiştirmek yetersizdir (SSOT çelişkisi).

**S3 — ADR nerede, nasıl açılır?**
[.decisions/index](.decisions/index.md) → `accepted/ADR-NNN-<slug>.md`. Şablon: [.templates/adr/](.templates/index.md). Zorunlu: ≥2 alternatif + sonuç (+/−).

**S4 — Hangi gate'teyim?**
[WORKFLOW.md](WORKFLOW.md) §3 + bu dosya §6. Gate onayı **kullanıcıya** aittir; ajan yalnız raporlar.

**S5 — Bozuk link var mı, nasıl bakarım?**
`pwsh -File .ai/scripts/check-links.ps1` → **BOZUK: 0** olmalı. Varsa hedefi üret ya da linki kaldır.

**S6 — Katman kuralı ihlali nereden anlaşılır?**
[architecture/index](architecture/index.md) §5 + [dependency-map](architecture/00-overview/dependency-map.md). Denetim: NetArchTest (`Architecture/LayerRulesTests.cs`).

**S7 — DevExpress sürümü tartışılabilir mi?**
Hayır. v26.1 + Office2010 = ADR-004. Değişiklik ayrı ADR + **kullanıcı onayı** (G8).

**S8 — EF Core kullanabilir miyiz?**
Hayır. ADR-005 + [.rules/data-access-rules](.rules/data-access-rules.md) + CLAUDE.md **G2**. Alternatifler tablosunda gerekçesi var.

**S9 — Kod yazmadan önce ne şart?**
**GATE-003**: 7 ASCII wireframe kullanıcının onayı (AGENTS.md §9.5, CLAUDE.md G12).

**S10 — İş 3 kez başarısız olursa?**
`3-STRIKE`: dur, **şüpheli varsayımın adını koy**, seçeneklerle escalite et ([AGENTS.md](AGENTS.md) §8.1). Kod yazmaya devam etmek yasak.

---

## §21 Ek — Sık Yapılan Hatalar (Red Team)

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Dosyayı `decisions/` / `rules/` (noktasız) yazmak | link kırılır | `.decisions/` · `.rules/` (§1.1 #8) |
| 2 | `ARCHITECTURE.md` eski hedefi | 21 bozuk link | `architecture/index.md` |
| 3 | Şablon placeholder linki (`../yok.md`, `[…](…)`) | sahte/bozuk tarama | kod içi örnek değildir; gerçek hedef koy |
| 4 | Katalog olmadan "vault bitti" demek | 91/500 MD | §13 #1 |
| 5 | `index.md`'ye içerik kopyalamak | SSOT ihlali | yalnız yol/tablo (SRP) |
| 6 | Ölçüm yapmadan boyut iddiası | band dışı dosya | `measure-vault.ps1` |
| 7 | Gate'i subagent'a onaylatmak | G12 | kullanıcı onayı (AGENTS.md §5.2 #7) |
| 8 | Kanıtsız "0 bozuk link" | sonradan kırık | `check-links.ps1` çıktısını yapıştır |
| 9 | Wireframe'siz Form yazmak | kod çöp | GATE-003 HARD STOP |
| 10 | Aynı bilgiyi 2 dosyaya yazmak | çelişen kural | tek kaynak + link |

---

## §22 Ek — Yeni Dosya Üretim Talimatı (PART 3 kalıbı)

Yeni bir vault dosyası açarken **bu 7 adım** atlanmaz; aksi halde GATE-004a kırmızıdır.

| # | Adım | Somut iş | Kanıt |
|---|---|---|---|
| 1 | **Kalıp seç** | `.templates/index.md` → ilgili şablonu bul (doküman, ADR, katman, test, sorgu) | dosya yolu |
| 2 | **Yolu kur** | konuya göre doğru dizin: `.decisions/accepted/` · `.rules/` · `spec/` · `architecture/lN-*/` · `reports/` · `ui-design/` | dizin listesi |
| 3 | **Frontmatter yaz** | 8 alan: `title, type, category, status, version, updated, authority: SSOT, scope` | ilk 10 satır |
| 4 | **Bağlantıları yaz** | en üstte *Zorunlu Bağlantılar* satırı (≥3 hedef, **göreli yol**, diskte var) | `check-links.ps1` = 0 |
| 5 | **İçeriği yaz** | önce **cevap/özet** (§0), sonra tablo/diyagram; tek cümlelik sorumluluk satırı zorunlu | §0 satırı |
| 6 | **Diyagram** | gerekiyorsa **UTF-8 kutu-art** (`─│┌┐└┘├┤┬┴┼` veya `╔═╗║╚═╝`); ASCII yerel kopya da ekle | diyagram bloğu |
| 7 | **Ölç + doğrula** | `measure-vault.ps1` (bant) + `check-links.ps1` (link) + frontmatter taraması | konsol çıktısı |

**Kalıp ↔ hedef eşlemesi:**

| İçerik türü | Şablon | Hedef dizin |
|---|---|---|
| Karar (ADR) | `.templates/adr/adr-*` | `.decisions/accepted/ADR-NNN-<slug>.md` |
| Kural | `.rules/` mevcut dosya düzeni | `.rules/<konu>-rules.md` (ya da `-guidelines/-guide`) |
| Şartname | `.templates/architecture/*` | `spec/<konu>-spec.md` |
| Katman | `.templates/architecture/layer-spec-template` | `architecture/lN-<ad>/index.md` |
| Rapor | doküman kalıbı | `reports/NN-<part>-raporu.md` |
| Ekran | `.templates/ui-design/*` | `ui-design/NN-<ekran>.md` (GATE-003) |
| Test | `.templates/testing/*` | `tests/` (PART 6) |

> **ÖLÇÜM ZORUNLU:** dosya "bitti" sayılmadan önce `scripts/check-links.ps1` ve `scripts/measure-vault.ps1` çalıştırılır; çıktısı `reports/` part raporuna yapıştırılır (ROLE.md §6 — kanıtsız bitti yasak).
