---
title: "VersaCoder — README"
type: readme
category: docs
status: active
version: 2.0.0
updated: 2026-10-07
authority: SSOT
scope: "Depo kökü README: künye bannerı, ne yapar, hızlı başlangıç, katmanlar, yapılandırma, testler, mimari not, hedef sistem diyagramı, referanslar"
---

# VersaCoder

```text
################################################################################
#                                                                              #
#                 Versa Coder AI My Coding Assistant Agentic IDE               #
#           Visual Studio IDE / Xcode / Cursor AI IDE / Antigravity AI IDE     #
#                                                                              #
#                              VERSION 1.0.0                                   #
#                                                                              #
#                    Designed & Coding by Bayram Ali Akşit                     #
#                                                                              #
################################################################################
```

> **Ajan/üretim SSOT'lu `.ai/` vault'u + henüz kurulmamış VersaCoder masaüstü kod tabanı.**
> Şu an depo **yalnızca belgedir**: `src/`, `tests/` ve `.sln` diskte **yok** (2026-10-05 ölçümü).

## 1. Ne Yapar (Answer First)

- **Studio / IDE:** VersaCoder yalnız kod IDE'si değil bir **Studio**'dur — yapay
  zekâyı daha iyi kullanmak için **kod + dokümantasyon + görsel üretim** aynı
  çalışma ortamından yürütülür (bkz. `6.1` altı not + [VISION](.ai/definitions/vision.md) §0.3).
- **vault:** **255 Markdown dosyasıyla** (`.ai/` · ölçüm 2026-10-07) kural, şartname,
  şablon ve bilgi notu tutar; toplam depo **264 MD**.
- **kapı:** `GATE-003` kullanıcı onayından önce tek satır `.cs` yazılmaz.
- **kod:** 9 üretim projesi (K0..K8) + test projesi **henüz yazılmadı** — `DOĞRULAMA GEREKLİ`.
- **görsel üretim:** `visual-director` ajanı render prompt hattını yürütür
  ([AGENTS](.ai/definitions/agents.md) §8.5); sağlayıcı entegrasyonu **planlı**.

## 2. Hızlı Başlangıç

```powershell
# on (bu makinede dogrulanmis)
git status --short
dotnet --version                                   # 10.0.301
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\check-links.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\measure-vault.ps1
```

```powershell
# kod kismi - .sln yokken CALISMAZ (DOĞRULAMA GEREKLİ)
dotnet build                                       # .sln YOK -> "MSB1003"
dotnet test                                        # tests/ YOK -> "hedef bulunamadi"
```

**Sıralama:** `.ai/.templates/index.md` → görev şablonu → `.ai/.rules/` → kod → `rules-check`.

## 3. Katmanlar / Kullanım

| Katman | Sorumluluk | Referans | Diskte (ölçüm 2026-10-07) |
|---|---|---|---|
| `.ai/.templates/` | şablon envanteri | [templates](.ai/.templates/index.md) | ✅ **36 MD** (10 alt klasör + kök) |
| `.ai/knowledge/` | bilgi notları | [index](.ai/index.md) | ✅ **131 MD** |
| `.ai/spec/` | şartnameler | [index](.ai/index.md) | ✅ **45 MD** |
| `.ai/ui-design/` | ekran wireframe | [index](.ai/index.md) | ✅ **14 MD** |
| `.ai/.agents/` | ajan profilleri | [index](.ai/index.md) | ✅ **7 MD** |
| `.ai/TODOS/` | görev fazları | [index](.ai/index.md) | ✅ **7 MD** |
| `.ai/scripts/` | ölçüm betikleri | [check-links.ps1](.ai/scripts/check-links.ps1) | ✅ **14 dosya (PS1)** |
| `.ai/.decisions/` | ADR kayıt defteri | — (`adr-index.md` **YOK**) | ⚠️ **1 MD** (yalnız `draft/ADR-031`) |
| `.ai/.rules/` | kalıcı kural metni | — (`index.md` **YOK**) | ❌ **YOK (0)** → `DOĞRULAMA GEREKLİ` |
| `.ai/architecture/` | katman spec + catalog | — (`index.md` **YOK**) | ❌ **YOK (0)** → `DOĞRULAMA GEREKLİ` |
| `.ai/checklists/` | gate listeleri | — (`index.md` **YOK**) | ❌ **YOK (0)** → `DOĞRULAMA GEREKLİ` |
| `.ai/reports/` · `playbooks/` · `prompts/` | rapor · playbook · prompt | — | ❌ **YOK (0)** → `DOĞRULAMA GEREKLİ` |
| `src/**` | 9 üretim projesi (K0..K8) | [CLAUDE](claude.md) §B5 | ❌ **YOK** |
| `tests/**` | unit + integration, hedef %90 | — | ❌ **YOK** |

> **192 kırık link** yukarıdaki ❌ satırlarına gidiyor ve **bilinçli olarak korunmuştur**
> (silinmedi; gelecek sürümlerde bağlanacak).

## 4. Yapılandırma

| Anahtar | Varsayılan | Not |
|---|---|---|
| `global.json` | SDK 10.0.301 | dosya **YOK** — `DOĞRULAMA GEREKLİ` |
| `Directory.Packages.props` | CPM kilidi | dosya **YOK** — `DOĞRULAMA GEREKLİ` |
| Frontmatter | 8 alan, UTF-8 (BOM'suz) | `.ai/**` **255/255 uyumlu** (2026-10-07 ölçümü) |
| Token bütçesi | 50.000 / oturum | kök dosya payı ≤ %3 |
| DevExpress | **yalnız v26.1** (ADR) | paket henüz kurulmadı |
| Veri erişimi | ADO.NET + Dapper | EF Core **yok** (G2 · ADR-005) |

## 5. Testler

```powershell
# su an calisir (belge dogrulamasi)
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\check-links.ps1
# kod testleri - tests/ YOK oldugu icin CALISMAZ (DOĞRULAMA GEREKLİ)
dotnet test --filter "FullyQualifiedName~VersaCoder"
```

## 6. Mimari Not

```text
C:\versacoder
+-- CLAUDE.md ............. kural + K0..K8 + G1..G12 (SSOT baglantisi)
+-- AGENTS.md ............. golge (20 satir, "Kaynak: CLAUDE.md")
+-- README.md ............. bu dosya
+-- WORKFLOW.md ........... kapi/kapı ozeti
+-- .ai/ .................. 255 MD vault (2026-10-07)
     +-- .templates/ ...... 38   sablon
     +-- .rules/ .......... 18   kural
     +-- .decisions/ ...... 32   (ADR-001..030)
     +-- architecture/ .... 129  katman spec
     +-- scripts/ ......... 14   ps1
+-- src/ .................. YOK (GATE-004 onayi bekliyor)
+-- tests/ ................ YOK
```

Yön: `K6 -> K7 -> {K5, K4} -> K3 -> K2 -> K1 -> K0`; yukarı bakan referans ihlaldir.

### 6.1 Hedef sistem diyagramı (prompt kaynağı: ai.04 §3)

```text
                          OPENCODE
                              │
                         ORCHESTRATOR
                              │
              ┌───────────────┼───────────────┐
              │               │               │
          RESEARCH      REQUIREMENTS       CONTEXT
              │               │               │
              └───────────────┼───────────────┘
                              │
                     ARCHITECTURE & .AI
                              │
                           PLANNER
                              │
                      TASK DECOMPOSER
                              │
                    SPECIALIZED AGENTS
                              │
         ┌────────────────────┼────────────────────┐
         │                    │                    │
       .NET                UI/UX              SECURITY
   l0..l7 (ADR-028)    01..07 ekran            OWASP
         │                    │                    │
         └────────────────────┼────────────────────┘
                              │
                           SKILLS
                              │
                             MCP
                              │
                            TOOLS
                              │
                       IMPLEMENTATION
                              │
                         CODE TRACKER
                              │
                    BUILD / TEST / REVIEW
                              │
                         VERIFICATION
                              │
                          .AI BRAIN
```

**Çekirdek felsefe (ai.04):** VersaCoder'ın amacı editör + chatbot değil;
**Agent + Session + Workflow + Tool + Memory + Model + Policy** kombinasyonunu
kullanan Agentic Software Engineering Platform'dur. LLM sistemin kendisi değil,
içinde çalışan bileşenlerden biridir.

## 7. Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kök kural (SSOT) | [CLAUDE.md](claude.md) |
| 2 | Gölge ajan dosyası | [AGENTS.md](agents.md) |
| 3 | Kapı özeti | [WORKFLOW.md](workflow.md) |
| 4 | Vault navigasyonu | [.ai/index.md](.ai/index.md) |
| 5 | Şablon envanteri | [.ai/.templates/index.md](.ai/.templates/index.md) |
| 6 | Mimari indeks | [.ai/architecture/index.md](.ai/architecture/index.md) |
| ROLE | [ROLE](.ai/definitions/role.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](.ai/definitions/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| brain | [brain](.ai/.decisions/brain.md) | ADR ozetleri (29) + SOLID kisitlari + 6 sert kod kurali |
| PROJECTS | [PROJECTS](.ai/definitions/projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](.ai/definitions/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| keys | [keys](.ai/governance/keys.md) | anahtar kelime -> hedef dosya hiz haritasi (101 satir) |
| MEMORY | [MEMORY](.ai/memory/memory.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [log](.ai/ops/log.md) | append-only is gunlugu + Quality Report |
| ULTRA | [ULTRA](.ai/definitions/ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.ai/.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| .ai CLAUDE | [.ai CLAUDE](.ai/claude.md) | AI anayasa — G1..G12 hard guardrail, kanit formati, dur/bekle |
| .ai WORKFLOW | [.ai WORKFLOW](.ai/governance/workflow.md) | gate zinciri — GATE-004a..006, kabul kriteri, kanit kutusu |
