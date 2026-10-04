---
title: "VersaCoder — Kapsam Analizi (PART 2)"
type: report
category: analysis
status: active
version: 1.1.0
updated: 2026-10-04
authority: SSOT
scope: "Kaynak okuma çıktısı → girdi envanteri, senior kararlar, riskler, envanterler, DoD + WEB GATE kanıtları + 52 düğüm katmanlı mimari kararı"
---

# VersaCoder — Kapsam Analizi

**Zorunlu bağlantılar:** [[../index.md]] · [[../WORKFLOW.md]] · [[../CLAUDE.md]] · [[../keys.md]] · [[../brain.md]] · [[../AGENTS.md]]

---

## 0. Özet (Answer First)

Kaynak okuma tamamlandı. **5 senior kararın 5'i de kaynaklarla çelişmeden uygulanabilir**, tek çelişki ai.01.md'nin eski teknoloji yığınıdır ve ai.03.md tarafından zaten geçersiz kılınmıştır. Vault kökü **`C:\versacoder\.ai`** kullanıcı onayıyla sabitlenmiştir. **WEB GATE kapandı** (`01-web-kaynaklari.md`, 8 kaynak): `net48` + `net10.0-windows` ikisi de DevExpress v26.1 ile destekli → R5 kapandı. **25.09.2026 ek kararları:** 52 düğüm katmanlı mimari + ≥5000 bileşen katalogu · SqlClient **her iki bacakta 5.x** (ADR-012) · ekran listesi = prompt'un 7 ekranı · ilk git commit **GATE-004**'te · LLM = tek contract + 2 sağlayıcı, ilk doğrulama **OpenRouter /free** üzerinden. Kritik risk kapanan R1: 7 ekran listesi kesinleşti, 16 yedek WF yalnız kontrol listesi referansıdır.

---

## 1. Girdi Envanteri

### 1.1 Plan dosyaları (birincil kaynak)

| Dosya | Boyut | Satır | Satır sonu | Tarih | Durum |
|---|---|---|---|---|---|
| `versacoder proje planaması ai.01.md` | 150.166 B | 5.415 | LF (temiz) | 2026-09-25 10:02 | **ESKİ yığın** — okundu |
| `versacoder proje planaması ai.02.md` | 123.721 B | 4.087 | LF+CRLF karışık | 2026-09-24 13:32 | Discovery/Q&A — okundu |
| `versacoder proje planaması ai.03.md` | 21.984 B | 1.543 | **CR-only** | 2026-09-25 10:03 | **YENİ bootstrap** — normalize şart |
| `opencode kaynak kodu editle...geit.md` | 122.873 B | — | — | 2026-09-23 | Opencode adaptasyon kaynağı |
| `opencode kaynak kodu editle...geit.02.md` | 34.638 B | — | — | 2026-09-24 | Opencode adaptasyon kaynağı |
| `notes.prompt.md` | 8.945 B | 154 | LF | 2026-09-25 10:32 | Bu master prompt'un kaynağı |
| `notes.prompt2.txt` | 27.679 B | 1.569 | — | 2026-09-25 10:27 | Q&A ham veri (5 karar buradan) |

Dizin: `G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\`

### 1.2 Vault referansları (salt okunur)

| Kaynak | İçerik | Amaç |
|---|---|---|
| `C:\www\coremusic.net\.ai` | **599 MD**, 15 kök MD (30–94 KB), 16 klasör | **YAPI** referansı — içerik değil |
| `C:\www\coremusic.net\` | README 16.616 B, WORKFLOW 28.590 B, CLAUDE 3.139 B | Kök MD kalıbı (pointer modeli) |
| `C:\www\versacoder\versacoder.ai\` | `.ai` + `.ai.01..04` ≈ 350+ MD | **YAPI+İÇERİK** referansı — elle yeniden yazılacak |

### 1.3 opencode envanteri (`%TEMP%\opencode-fresh`, v1.18.32)

| Paket | Dosya | Not |
|---|---|---|
| `packages/opencode` | 836 | CLI uygulaması |
| `packages/core` | 491 | Ajan döngüsü, tool sistemi |
| `packages/tui` | 241 | Terminal UI |
| `packages/sdk` | 48 | SDK |
| `packages/server` | 31 | Server |
| `packages/protocol` | 26 | Mesaj şeması → PART 7 ana girdisi |

Toplam 33 paket. `packages/` altında ayrıca `codemode`, `plugin`, `llm`, `ui`, `desktop` mevcut.

### 1.4 Mevcut iş durumu

- `C:\versacoder` → git repo, **commit yok**, `?? .ai/` tek untracked girdi.
- `C:\versacoder\.ai\.templates\` → **24 dosya hazır** (7–38 KB): adr/, agents/, architecture/, backend/, infrastructure/, query/ alt klasörleri.
- `C:\www\versacoder\versacoder` → **BOŞ** (kullanıcı `C:\versacoder\.ai` seçimini onayladı).

---

## 2. 5 Senior Kararın Gerekçesi

| # | Karar | Gerekçe | Kaynak çelişkisi |
|---|---|---|---|
| 1 | **net48 + net10.0-windows** iki bacak | ai.03 §"TARGET FRAMEWORKS" ile birebir aynı. net48 = legacy kanıt, net10 = modern. Ortak kaynak + `#if` sadece platform farklarında | ai.01 `.NET 8` yazıyor → **ai.03 geçersiz kılmış** |
| 2 | **Sıfırdan yeniden yazım** | ai.01 BÖLÜM 6 `.ai` yapısı eski ve CoreMusic'e özgü değil. Yedek vault'taki içerik tutarsız (`.ai.01` 80 KB brain, `.ai.04` 2 KB brain) | Kopyala-yapıştır yasağı prompt'ta açık |
| 3 | **~500+ MD, 30–88 KB kök** | coremusic `.ai` = 599 MD, kök MD'ler 31–94 KB → hedeflenebilir ölçek | Yedek `.ai.04` kök MD'leri 1–16 KB → kalite çıtası düşük, referans alınmamalı |
| 4 | **DevExpress v26.1** | ai.03 §"DevExpress Universal 2026 / v26.1" + `RibbonControlStyle.Office2010` | ai.01 BÖLÜM 28.3 `DevExpress.Win.All 24.2.3` → **REDDEDİLDİ** |
| 5 | **ADO.NET + Dapper, EF Core YOK** | net48 bacağında EF Core 9 desteklenmez → ortak kaynak için tek yol. Parameterized zorunlu, migration = SQL script + version tablosu | ai.01 BÖLÜM 28.2 EF Core 9.0.0 → **REDDEDİLDİ** |

**Kural:** ai.01'den yalnızca *yapıbilgi* (ekran listesi, gates, mimari şema) alınır; teknoloji sürümleri **asla** alınmaz.

### 2.1 Ek kararlar — 25.09.2026 oturumu (kullanıcı onaylı)

| # | Karar | Ayrıntı | Etki |
|---|---|---|---|
| 6 | **Katmanlı mimari = 52 düğüm** | 7 makro (L1..L7) × 45 alt katman, bağımlılık tek yön L7→L1 | ADR-013 · `architecture/catalog/` |
| 7 | **≥5000 bileşen kaydı** | Her kayıt: `id · katman · tip · sorumluluk · bağımlılık · test id · web URL`; `scripts/count-components.ps1` ile PART 8'de kanıt | ≈90 dk (script destekli) |
| 8 | **7 ekran = prompt listesi** | `01-start-page, 02-main-ide, 03-solution-explorer, 04-code-editor, 05-ai-chat, 06-agent-panel, 07-settings` — 16 yedek WF yalnız kontrol listesi referansı | PART 4 kapsamı kilitli |
| 9 | **İlk git commit = GATE-004** | WORKFLOW §7'ye uyulur; öncesi commit yok | git disiplini |
| 10 | **LLM = tek contract + 2 sağlayıcı** | `ILLMProvider` (chat + SSE streaming); ilk doğrulama **OpenRouter `/free` router** modeliyle, ardından Ollama/OpenAI-uyumlu | PART 7 |
| 11 | **SqlClient 5.x (iki bacak)** | CPM tek sürüm 5.2.x bandı → net48 + net10 uyumu | ADR-012 |

---

## 3. Riskler · Belirsizlikler · Varsayımlar

### 3.1 Riskler (R1–R6)

| ID | Risk | Etki | Azaltma |
|---|---|---|---|
| R1 | **7 ekran ≠ 16 yedek wireframe** isim uyuşmazlığı | Yüksek | PART 4'te 7 ekranı prompt'tan üret, 16 WF'yi yalnızca *bilgi* olarak tara |
| R2 | `ai.03.md` **CR-only** satır sonu → parse hataları | Orta | Okuma anında normalize (`\r` → `\n`); asla diske CR-only yazma |
| R3 | ai.01'de EF Core/DevExpress 24.2 sızıntısı | Yüksek | PART 3'te `.rules/` içine "EF Core YOK" + "v26.1 tek sürüm" hard rule |
| R4 | ~500 MD + 30–88 KB kalitesi → zaman aşımı | Yüksek | `.templates/` zaten hazır; kalıp başına bir kez yaz, katmanlara dağıt |
| R5 | ~~net48 DevExpress v26.1 uyumluluk~~ **KAPANDI (25.09.2026)** — web gate: v24.2–v26.1 `.NET Framework 4.8 = ✓`, v25.2–v26.1 `.NET 10 = ✓` | Düşük | Kanıt: `01-web-kaynaklari.md` §2 (docs.devexpress.com prerequisites) |
| R6 | Bozuk iç link (500 MD, ~bin link) | Orta | PART 3 ve PART 8'de otomatik link taraması |

### 3.2 Belirsizlikler (B1–B4)

| ID | Belirsizlik | Karar zamanı |
|---|---|---|
| B1 | ~~`.slnx` vs `.sln`~~ **KAPANDI** → `.slnx` (prompt SSOT) | — |
| B2 | ~~Test framework~~ **KAPANDI** → xUnit + NSubstitute + NetArchTest ([ROLE.md](../ROLE.md) §1.1) | — |
| B3 | 7 ekranın DevExpress kontrol seti kesinleşmedi | PART 4 (onay öncesi) |
| B4 | ~~LLM provider sırası~~ **KAPANDI (25.09)** → tek `ILLMProvider` contract + iki impl paralel; ilk doğrulama **OpenRouter `/free` router** ile, sonra Ollama | PART 7 |
| B5 | `Microsoft.Data.Sqlite` net48 + net10 ikili desteği | `NEEDS_VALIDATION` → PART 5 NuGet teyidi |

### 3.3 Varsayımlar (A1–A3)

| ID | Varsayım | Doğrulanma |
|---|---|---|
| A1 | Vault kökü = `C:\versacoder\.ai` | ✅ Kullanıcı onaylı |
| A2 | CoreMusic vault'u salt okunur, sadece YAPI referansı | ✅ Prompt yasağı |
| A3 | `.templates/` (24 dosya) mevcut iş olarak korunur, çakışma yok | PART 3 başında doğrula |

---

## 4. Envanterler

### 4.1 7 Ekran (PART 4 hedefi)

| # | Dosya adı | DevExpress kontrolleri (aday) |
|---|---|---|
| 01 | `01-start-page` | RibbonControl, XtraTabControl, TileControl |
| 02 | `02-main-ide` | RibbonForm, RibbonControl (Office2010), DockManager, StatusBarControl |
| 03 | `03-solution-explorer` | TreeList, DxCollapsedControl, ContextMenuStrip |
| 04 | `04-code-editor` | MemoEdit / RichEditControl, XtraTabControl, GaugeControl |
| 05 | `05-ai-chat` | MemoEdit, ListBoxControl, ProgressBarControl, BarEditItem |
| 06 | `06-agent-panel` | GridControl, ProgressPanel, BarButtonItem |
| 07 | `07-settings` | DataLayoutControl, PropertyGridControl, LookUpEdit |

**Çelişki (R1):** yedek vaultta `WF-00..WF-15` (16 adet: ana ekran, chat, wizard, session, agent, skill, memory, workflow, settings, F2 debug, F5 effort, model seçimi, context editor, flow editor, prompt maker, docs). ai.01 BÖLÜM 12 ise 8 ana ekran tanımlar. **PART 4'te 7 ekranlık liste prompt'tan esas alınır**, yedek yalnızca kontrol listesi/bilgi kaynağıdır — ve sunumda kullanıcıya sorulur.

### 4.2 ADR Envanteri (yedek vault — sıfırdan yeniden yazılacak)

| ADR | Konu | Not |
|---|---|---|
| ADR-001 | csharp-net8-devexpress | **Güncellenmeli** → net48+net10 / v26.1 |
| ADR-002 | mvvm-communitytoolkit | **İptal** → prompt MVVM zorunlu kılmıyor, WinForms + DI |
| ADR-003 | sqlite-wal-mode | Geçerli → ADO.NET + Dapper ile uyarla |
| ADR-004 | multi-provider-llm | Geçerli (PART 7) |
| ADR-005 | agent-system | Geçerli (PART 7) |
| ADR-006..011 | tool, session, context, mcp, learning, context-assembly | Geçerli (PART 6–7) |
| **ADR-012** | **dual-tfm-data-provider** — SqlClient **her iki bacakta 5.x** (6.0 .NET Standard'ı düşürdü); Sqlite `NEEDS_VALIDATION` | **YENİ 25.09.2026** → PART 3'te `architect` yazar |
| **ADR-013** | **layered-architecture-52-nodes** — 7 makro × 45 alt katman = 52 düğüm, ≥5000 bileşen kaydı, tek yön bağımlılık | **YENİ 25.09.2026** → PART 3 |

### 4.3 Gates Envanteri — **ESAS: [WORKFLOW.md](../WORKFLOW.md) GATE-001..006** (vault SSOT)

| Gate | Ad | Kapanış | Kapanış şekli |
|---|---|---|---|
| GATE-001 | Requirements Freeze | PART 1 sonu | ✅ **KAPANDI 25.09.2026** (4/4 kriter: 3 plan okundu · CR-only normalize · sürümler ai.01'den alınmadı · çelişki listesi yazıldı) |
| GATE-002 | Scope Freeze | PART 2 sonu | kanıtla kapanır — bu rapor |
| GATE-004a | Vault green (link + frontmatter) | PART 3 sonu | link taraması 0 bozuk + 14 kök 30–88 KB |
| GATE-003 | **UI Approval** | PART 4 sonu | **HARD STOP — kullanıcı onayı ZORUNLU** |
| GATE-004 | Build Green | PART 5 sonu | `dotnet build` 0 hata (iki bacak) + **ilk git commit** |
| GATE-005 | Test Green | PART 6 sonu | `dotnet test` yeşil + NetArchTest |
| GATE-006 | Release / Kapanış | PART 8 | teslim raporu + kanıtlar |

> **ÇÖZÜLEN ÇELİŞKİ (C4):** ai.03'teki 9 gate (GATE-000..008) ve ai.01/ai.02'deki 6 gate (GATE 0–5) **REDDEDİLDİ**; esas olan `WORKFLOW.md`'daki 6 gate'tir. Yedek `gates/` klasöründeki dosyalar yalnız referans.

---

### 4.4 Kapsam Dışı (GATE-002 Scope Freeze)

| # | Kapsam DIŞI — yapılmaz | Neden |
|---|---|---|
| 1 | EF Core, string-concat SQL, magic connection string | CLAUDE.md G2 (yasak) |
| 2 | DevExpress v26.1 dışı sürüm / `Office2010` dışı ribbon stili | G3 |
| 3 | CoreMusic vault'una yazma, `*.old.*` kopyalarına dokunma | G1 + DOKUNULMAZ |
| 4 | 16 yedek wireframe'in ekran olarak üretilmesi (yalnız 7) | karar #8 — 16 WF = kontrol listesi referansı |
| 5 | opencode TypeScript kodunun doğrudan kopyalanması (COPY=NO) | ai.03 §24 — yalnızca **adaptasyon** (PART 7) |
| 6 | MCP server implementasyonu, plugin marketplace, web/mobil istemci | spec seviyesinde kalır (PART 6–7 dışı) |
| 7 | `G:\` notlarına ve `C:\www\coremusic.net` altına yazma | salt okunur |

> **KURAL:** Bu listeden çıkış = kapsam değişikliği → gate yeniden açılır, kullanıcı onayı gerekir ([AGENTS.md](../AGENTS.md) §3.2 #11).

---

## 5. Kabul Kriterleri (Definition of Done)

**PART 2 DoD (bu rapor):**
- [x] 3 plan dosyası okundu, CR-only normalize edildi (okuma anında)
- [x] coremusic `.ai` yapısı tarandı (599 MD, 15 kök MD)
- [x] yedek vault tarandı (`.ai` + `.ai.01..04`)
- [x] opencode paket envanteri çıkarıldı (33 paket, 6 ana paket)
- [x] Girdi envanteri + 5 karar gerekçesi + risk/belirsizlik/varsayım + 3 envanter + DoD yazıldı
- [x] Vault kökü kullanıcı onayıyla sabitlendi

**Proje geneli DoD (PART 8'de kanıtlanacak):**
1. `dotnet build` → **her iki bacakta da 0 hata**
2. `dotnet test` → tüm testler yeşil
3. Vault link taraması → **0 bozuk link**
4. `reports/09-teslim-raporu.md` → build/test çıktılarıyla kanıtlı
5. EF Core referansı **0**, DevExpress sürümü **yalnız v26.1**, string-concat SQL **0**

---

## 6. Bir Sonraki Adım

**PART 3 — .ai Vault + Katmanlı Mimari Kataloğu (≈90 dk)**, sıra zorunlu:
`.templates/` (24 dosyayı doğrula) → `.agents/` → kök MD'ler (14 adet, 30–88 KB) → **`architecture/catalog/` = 52 düğüm + ≥5000 bileşen kaydı** (ADR-013) → `.decisions/` (**ADR-012 + ADR-013**) → `.rules/`, `spec/`, `ui-design/`, `scripts/count-components.ps1`, `.sql/`, `.obsidian/`.
**Çıktı:** `GATE-004a` (link 0 bozuk · frontmatter 8 alan · boyut bandı) → sonra **PART 4 wireframe (GATE-003 HARD STOP)**.
