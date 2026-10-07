---
title: AGENTS — Ajan Sistemi, Chat/Agent Ayrımı ve Orkestrasyon Protokolü
type: policy
category: ".ai"
status: active
version: 3.0.0
updated: 2026-10-07
authority: primary
scope: "Chat ≠ Agent ≠ LLM ayrımı, ajan dağıtım matrisi, çıktı kutusu, External Agent protokolü, orchestrator akışı, eşzamanlılık"
tags: [identity]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# AGENTS — Ajan Sistemi ve Orkestrasyon Protokolü

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE](../claude.md) · [WORKFLOW](../governance/workflow.md) · [ROLE](role.md) · [engine](engine.md) · [VISION](vision.md)

> **Tek cümlelik sorumluluk:** Chat/Agent/LLM ayrımını koruyan, işi doğru ajana
> dağıtan, çıktı kutusunu ve kanıt zorunluluğunu dayatan, External Agent
> execution'ını kontrollü tutan protokolün tek kaynağıdır.

---

## §0 Özet (Answer First)

```text
╔══════════════════════════════════════════════════════════════════╗
║  TEMEL AYRIM:  Chat ≠ Agent  ·  Agent ≠ LLM  ·  LLM ≠ VersaCoder ║
║  Ayrıca:       External CLI ≠ VersaCoder Agent Runtime            ║
╚══════════════════════════════════════════════════════════════════╝
```

- **§1 Çekirdek tanımlar** — Chat, Agent, LLM, Session, Workflow, Orchestrator
  nedir; birbirine karıştırılmaz.
- **§2 Dağıtım matrisi** — mimari→`architect`, kod→`developer`, inceleme→`reviewer`,
  test→`tester`, güvenlik→`security-auditor`, UI→`ui-designer`, orkestrasyon→
  `orchestrator`; matris dışı = `general`.
- **§3 Çıktı kutusu** — 5 alanlı ASCII kutu (`BASLIK/KAPSAM/KARAR/RİSK/KANIT`);
  kutusuz çıktı işlenmez.
- **§4 Internal vs External Agent** — VersaCoder = Control Plane,
  External Agent = Execution Plane.
- **§5 Session Agentic Agent Wizard** — niyet → yapılandırma → immutable config.
- **§6 Orchestrator akışı** — intent → task → plan → agent → workflow → validation.
- **§7 Devre paketi + 3-STRIKE + eşzamanlılık (≤3)**
- **§8 External Agent adapter sözleşmesi** (Request/Result, lifecycle, fallback)
- **§9 Zero-Hallucination yasak listesi**
- **§10 Doğrulama**

Kaynak: [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) — 8 makro
katman, 9 üretim + 1-2 test proje.

---

## §1 Çekirdek Tanımlar (Chat ≠ Agent ≠ LLM)

### §1.1 Tanım tablosu

| # | Kavram | Tek cümle tanım | Ne DEĞİLDİR |
|---|---|---|---|
| 1 | **Chat** | Kullanıcının VersaCoder ile doğal dil iletişim kurduğu çalışma yüzü; niyetin sisteme girdisidir | Klasik `Prompt → LLM → Answer` chatbot değil; kontrolsüz kod motoru hiç değil |
| 2 | **Agent** | Belirli bir mühendislik görevi, rolü, yeteneği, çalışma bağlamı, araç erişimi ve yaşam döngüsü olan **kontrollü çalışma birimidir** | Tek bir prompt değil; tek başına LLM değildir |
| 3 | **LLM** | Agent'ın kullanabileceği karar/üretim bileşenlerinden biri; probabilistic metin/kod üretir | Sistemin kendisi değildir; VersaCoder'ın tamamı değildir |
| 4 | **Session** | Bir mühendislik görevinin kontrollü çalışma alanı: amacı, bağlamı, agent yapılandırması, workflow'u ve durumu | Sıradan chat geçmişi değildir |
| 5 | **Workflow** | Görevin nasıl ilerleyeceğini söyleyen adım/bağımlılık grafiği | Her görev için zorunlu yüzlerce adım değildir; gereksinim önce, complexity sonra |
| 6 | **Orchestrator** | User Intent'i değerlendiren, execution strategy belirleyen, agent seçen, session/workflow yöneten koordinasyon birimi | UI logic, DB logic, CLI syntax içermez |
| 7 | **External CLI** | Gerektiğinde arka planda çalıştırılan harici coding-agent (OpenCode, Codex, Claude Code, Abacus AI CLI) | VersaCoder'ın kendisi değildir; yalnız execution provider'dır |

### §1.2 Chat ≠ Agent akışı

```text
  ┌──────────┐
  │   USER   │
  └────┬─────┘
       │  doğal dil talebi
       ▼
  ┌──────────┐    Chat yalnızca şudur:
  │   CHAT   │───> kullanıcı niyetini sisteme aktarır
  └────┬─────┘
       │
       ▼
  ┌──────────────┐
  │ INTENT ENGINE │  niyeti ayrıştırır: kapsam, risk, uzmanlık
  └────┬─────────┘
       │
       ▼
  ┌────────────────────────────┐
  │ SESSION AGENTIC AGENT      │  yapılandırmayı kullanıcıya gösterir
  │ WIZARD                     │
  └────┬───────────────────────┘
       │  "Onayla / Başlat"
       ▼
  ┌──────────────┐
  │ ORCHESTRATOR │  agent seçimi + workflow + policy
  └────┬─────────┘
       │
       ▼
  ┌─────────────────────────────────────────┐
  │  INTERNAL AGENT   veya   EXTERNAL AGENT │
  └────┬────────────────────────────────────┘
       │
       ▼
  ┌────────────┐   ┌────────────┐   ┌──────────┐
  │ EXECUTION  │──>│ VALIDATION │──>│  RESULT  │
  └────────────┘   └────────────┘   └────┬─────┘
                                         │
                                         ▼
                                     ┌──────┐
                                     │ CHAT │──> USER
                                     └──────┘
```

**Kural:** Chat doğrudan kontrolsüz kod üretim motoru değildir; Chat'in arkasındaki
Agentic sistem niyeti mühendislik görevine dönüştürür.

### §1.3 Ayrım belirtileri (karıştırılırsa ne olur)

| # | Karıştırma | Belirti | Sonuç |
|---|---|---|---|
| 1 | Chat = Agent sanmak | "Chat'e kod yazdırdım" | Niyet → görev dönüşümü atlanır, bağlam kaybolur |
| 2 | Agent = LLM sanmak | "Model ne derse o" | Policy, session, validation yok; kontrolsüzlük |
| 3 | LLM = VersaCoder sanmak | "Prompt yaz yeter" | Agent Runtime, Tool Gateway, Audit yok |
| 4 | External CLI = VersaCoder | "OpenCode'a bağlan yeter" | Control Plane bypass; scope/policy ihlali |

---

## §2 İş tipi → ajan matrisi

| İş tipi | Ajan | Çıktı formatı | Kanıt |
|---|---|---|---|
| Niyet/orkestrasyon | `orchestrator` | Plan + §3 kutusu | Session config + workflow şeması |
| Mimari / katman kararı | `architect` | Karar + §3 kutusu | [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) + [layer-boundary-check](../checklists/layer-boundary-check.md) 13 soru |
| Kod yazımı / refactor | `developer` | Diff + §3 kutusu | Build + test kırmızı→yeşil |
| İnceleme / review | `reviewer` | §3 kutusu (RİSK satırı dolu) | Suçlu dosya + satır no |
| Test / kapsam | `tester` | Test dosyası + §3 kutusu | `dotnet test` çıktısı |
| Güvenlik | `security-auditor` | Bulgu listesi + §3 kutusu | OWASP kalemi + severity |
| UI / ekran | `ui-designer` | [glossary](glossary.md) terimli tasarım + §3 kutusu | 7 ekrandan birinin adı (`01-start-page` .. `07-settings`) |
| Görsel üretim / render | `visual-director` | Prompt + görsel analiz + §3 kutusu | Render çıktısı ya da `[VERIFY]` etiketli parametre |
| Dokümantasyon üretimi | `docs-writer` | Markdown + §3 kutusu | Frontmatter 11 alan + link taraması |
| Harici ajan çalıştırma | `orchestrator` (adapter) | Normalized result + §3 kutusu | Build/test/static analysis çıktısı |
| Tanım dışı (hepsi dışında) | `general` (varsayılan) | Serbest + §3 kutusu | Kendi sözü değil, kanıta dayalı |

**Tek satırlık kural:** istek matriste yoksa `general`; varsa atanmış ajana gider.

**Zincir kuralı:** `orchestrator → architect → developer → reviewer → tester`,
[WORKFLOW](../governance/workflow.md) akışıyla aynıdır; atlanan halkada kabul kriteri kanıtsız kalır.

### §2.1 ASCII/Unicode diyagram üretimi kuralları (kaynak: ui-uix-anayze-tomd + ai.09)

Bu vault'ta diyagramlar **Unicode box-drawing** (`┌ ─ ┐ │ └ ┘ ├ ┤ ┬ ┴ ┼`) ile
çizilir — kullanıcı talimatı. Ama diyagram **süs değil, teknik referanstır**:

| # | Kural | Yasak |
|---|---|---|
| 1 | Geometrik doğruluk > karakter sayısı | Süsleme, dekoratif çizim |
| 2 | Her önemli düğüm için benzersiz, stabil ID (`SCREEN-01`, `HEADER-01`) | Hayali component |
| 3 | Kaynakta olmayan pixel/coordinate/state **uydurulmaz** → `UNKNOWN` | Tahmini gerçek gibi yazmak |
| 4 | Parent-child ilişkisi yalnız kaynakta varsa çizilir | Hayali hiyerarşi |
| 5 | Emoji yasak | `U+2500-U+257F` **izinli** (kullanıcı talimatı), emoji yok |
| 6 | Conflict varsa `CONFLICT` / `VERIFICATION REQUIRED` işaretilir | Çelişkiyi gizlemek |

**Doğrulama zinciri (tomd §19):**

```text
Canvas ──► Geometry ──► Unique IDs ──► Hierarchy ──► x/y/w/h
   │
   └──► ASCII ↔ Geometry tutarlılığı ──► Source conflict ──► UNKNOWN check
```

**Kaynak önceliği:** `User Instruction → Approved Decisions → Documentation →
Figma/API → PNG → Code → ASCII`. ASCII **source of truth değildir**; kaynaklardan
üretilen referans temsilidir.

**TUI/paket notu (ai.09 §2):** Terminal kabuğu için `Terminal.Gui 2.5.0` +
`Terminal.Gui.Editor 2.5.7` + `PrettyPrompt 6.0.5` + `Markdig 1.4.0`
(nuget.org 2026-10-06 doğrulaması); `Terminal.Gui`'a iş mantığı **girmez**.

```text
orchestrator
     │
     ▼
 architect ──(ADR + katman sözleşmesi)──► developer
                                               │
                                               ▼
                                          reviewer ──PASS──► tester ──► rapor
                                               │
                                             RED
                                               │
                                               ▼
                                          developer (geri)
```

---

**İlgili:** [glossary](glossary.md) §1 (terimler) · [CLAUDE](../claude.md) G4 (biçim/kod) · [VISION](vision.md) §5 (red-team) · [keys](../governance/keys.md) §3.1 (arama kalıpları).
## §3 Çıktı kutusu (zorunlu format)

Her ajan çıktısını şu ASCII kutuyla verir (Unicode kutu çizimleri bu dosyada
**izinlidir** — kullanıcı talimatı; `U+2500-U+257F` diyagramlarda kullanılır):

```text
┌──────────────────────────────────────────────────────┐
│ BASLIK : 2-4 kelime, ne yapıldı                      │
│ KAPSAM : dokunulan dosyalar (yol) veya "yok"         │
│ KARAR  : seçim + tek satır gerekçe                   │
│ RİSK   : en fazla 1 madde; yoksa "yok"               │
│ KANIT  : komut çıktısı / satır no / test sonucu      │
└──────────────────────────────────────────────────────┘
```

- **KANIT zorunlu** — kanıtsız çıktı kaba metindir; işlenmez, aynı kutuyla tekrar istenir.
- **RİSK zorunlu** — "yok" yazılabilir ama boş bırakılamaz.

**Kabul/RED kriterleri:**

| # | Kriter | Kabul | RED |
|---|---|---|---|
| 1 | 5 alan dolu mu | `BASLIK·KAPSAM·KARAR·RİSK·KANIT` yazıyor | Eksik alan var |
| 2 | `RİSK` satırı | En fazla 1 madde ya da `yok` | Boş ya da 2+ madde |
| 3 | `KANIT` satırı | Komut çıktısı, satır no ya da test sonucu | "bitti/hazır/tamam" |
| 4 | `KAPSAM` satırı | Dosya yolları ya da `yok` | Yok halde dosya var |
| 5 | Ajan ataması | §2 matrisine göre doğru ajan | Uzman dışı ajan |
| 6 | Eşzamanlılık | Aynı anda ≤ 3 ajan | Aşım özetlenmemiş |

**Kırmızı çizgi:** `KANIT` boşsa kutu işlenmez — kanıtsız teslim, teslim değildir.

**Karıştırma yasağı:** bu 5 alanlı ajan kutusu ile gate'in 4 satırlı kanıt kutusu
(`KANIT/DOSYA/EKSIK/SONRAKI`, [WORKFLOW](../governance/workflow.md)) aynı şey değildir.

### §3.1 Dil ve teknik terim politikası (kaynak: Yazımı-düzelt-promt-maker)

- **İletişim dili: Türkçe.** TDK yazım kuralları, Türkçe grammar, noktalama,
  büyük/küçük harf, Türkçe karakterler ve typo'lar düzeltilir.
- **Technical Terms İngilizce kalır** — Türkçeleştirilmez: `Frontend` ·
  `Backend` · `API` · `Router` · `Component` · `Framework` · `Repository` ·
  `Middleware` · `Dependency Injection` · `Clean Architecture` · `SOLID` ·
  `Refactoring` · `Unit Test` · `Deployment` · `Build` · `Runtime` ·
  `Session` · `Token` · `JWT` · `OAuth` · `CSRF` · `XSS` · `SQL Injection` ·
  `Layer` · `Gate`.

```text
YANLIŞ : "Ön yüz yönlendiricisini kontrol et."
DOĞRU  : "Frontend Router'ı kontrol et."
```

**İlgili:** [glossary](glossary.md) §1.2 (kısaltmalar) · [CLAUDE](../claude.md) G5 (Türkçe + mojibake) · [ROLE](role.md) §1.2 (5 ölçü) · [keys](../governance/keys.md) §1 satır 87 (anayasa).
### §3.2 Ajan davranışının 12 mutlak kuralı (kaynak: Yazımı-düzelt-promt-maker §39)

| # | Kural |
|---|---|
| 01 | Kullanıcının amacı değiştirilemez |
| 02 | Kullanıcının teknik requirement'ı izinsiz değiştirilemez |
| 03 | Bilgi uydurulamaz (Zero-Hallucination) |
| 04 | Technical Terms gereksiz şekilde Türkçeleştirilemez |
| 05 | Basit task gereksiz şekilde Enterprise Architecture'a dönüştürülemez |
| 06 | Eksik bilgi tahmin edilemez (`[BELİRTİLMELİ]` kullan) |
| 07 | Gereksiz kelime ve bölüm eklenemez |
| 08 | Çıktı doğrudan kullanılabilir olmalıdır |
| 09 | Çıktı mümkün olduğunca maddeli ve hiyerarşik olmalıdır |
| 10 | Gerektiğinde tablo ve checklist kullanılmalıdır |
| 11 | Derinlik = gereksiz uzunluk değildir; derinlik = gereksinimlerin doğru yapılandırılmasıdır |
| 12 | Kullanıcının ham metni kötü yazılmış olsa bile teknik niyet korunmalıdır |

**Uygulama örneği:**

```text
┌──────────────────────────────────────────────────────┐
│ BASLIK : Session Wizard akışı diyagramı eklendi      │
│ KAPSAM : .ai/AGENTS.md                               │
│ KARAR  : ai.05 §47 end-to-end modeli ASCII'ye çevrildi│
│ RİSK   : yok                                         │
│ KANIT  : dosya yazıldı, link taraması -> 0 kırık      │
└──────────────────────────────────────────────────────┘
```

---

**İlgili:** [CLAUDE](../claude.md) G1..G12 (kural metni) · [ROLE](role.md) §1.2 (yanıt anatomisi) · [engine](engine.md) §1.6 (teslim kontrolü) · [glossary](glossary.md) §2.2 (RED listesi).
## §4 Internal Agent vs External Coding Agent

### §4.1 İki execution yaklaşımı

```text
  ┌─────────────────────────────────────────────────────────┐
  │                  VERSACODER                             │
  │                                                         │
  │  INTERNAL AGENT YOLU                                    │
  │  ┌─────────────┐   ┌──────────┐   ┌──────────────────┐  │
  │  │Agent Runtime│──>│  Agent   │──>│ Model Provider   │  │
  │  └─────────────┘   └──────────┘   │  Tools / MCP     │  │
  │                                   └────────┬─────────┘  │
  │                                            │            │
  │  EXTERNAL AGENT YOLU                       │            │
  │  ┌─────────────┐   ┌──────────┐   ┌───────▼─────────┐  │
  │  │  Session    │──>│ Execution│──>│ OpenCode CLI    │  │
  │  │             │   │ Adapter  │   │ Codex CLI       │  │
  │  └──���──────────┘   └──────────┘   │ Claude Code CLI │  │
  │                                   │ Abacus AI CLI   │  │
  │                                   └───────┬─────────┘  │
  │                                           │            │
  └───────────────────────────────────────────┼────────────┘
                                              │  Result Ingestion
                                              ▼
                                   ┌────────────────────┐
                                   │  VALIDATION        │
                                   │  (build/test/      │
                                   │   static analysis) │
                                   └─────────┬──────────┘
                                             │
                                             ▼
                                        ┌─────────┐
                                        │  CHAT   │
                                        └─────────┘
```

### §4.2 Control Plane vs Execution Plane

```text
┌────────────────────────────────────────────┐
│        VERSACODER = CONTROL PLANE          │
│  · User Intent  · Session                 │
│  · Workflow     · Policy                  │
│  · Approval     · Validation              │
│  · Audit                                     │
└──────────────────┬─────────────────────────┘
                   │  kontrollü çağrı
                   ▼
┌────────────────────────────────────────────┐
│    EXTERNAL AGENT = EXECUTION PLANE        │
│  · Repository üzerinde kod çalıştırır      │
│  · Harici Tool kullanır                    │
│  · Test/build yürütür                      │
│  · Kendi Agent Runtime'ını kullanır        │
└────────────────────────────────────────────┘
```

**Kritik sınır:** External Agent kullanıldığında VersaCoder'ın kontrolü
kaybolmamalıdır. External Agent:
- VersaCoder'ın global policy'sini **bypass etmez**
- User approval gerektiren işlemi **onaysız yapmaz**
- Session kapsamı **dışına çıkmaz**
- Secret bilgisi **rastgele almaz**
- Result'u **doğrulanmış olarak işaretlemez**

### §4.3 External Agent adapter sözleşmesi

```text
  ┌───────────────────────┐        ┌────────────────────────┐
  │ EXTERNAL AGENT REQUEST│        │  EXTERNAL AGENT RESULT │
  ├───────────────────────┤        ├────────────────────────┤
  │ Provider              │        │ Status                 │
  │ Agent                 │        │ Exit Code              │
  │ Session               │        │ Stdout / Stderr        │
  │ Working Directory     │  ───►  │ Structured Output      │
  │ Prompt / Task         │        │ Files Changed          │
  │ Environment           │        │ Execution Duration     │
  │ Permission Policy     │        │ Errors / Warnings      │
  │ Timeout / Cancel      │        │ Validation State       │
  │ Output Mode           │        │ Metadata               │
  └───────────────────────┘        └────────────────────────┘
```

**Adapter mimarisi:**

```text
  Application / Agent Runtime
            │
            ▼
   IExternalAgentExecutor
            │
   ┌────────┼────────┬──────────────┐
   ▼        ▼        ▼              ▼
OpenCode  Codex  ClaudeCode     AbacusAiCli
Executor  Executor Executor      Executor
```

> Gerçek executable adı, CLI syntax, sürüm ve authentication bilgisi
> **`[VERIFY REQUIRED]`** — doğrulanmadan kesin yazılmaz.

### §4.4 Process lifecycle

```text
  CREATED ──► VALIDATED ──► STARTING ──► RUNNING ──► OUTPUT_STREAMING ──► COMPLETED
                                                  │
                                    ┌─────────────┼──────────────┐
                                    ▼             ▼              ▼
                                 FAILED      CANCELLING       TIMEOUT
                                              │                  │
                                              ▼                  ▼
                                          CANCELLED         TERMINATION
```

### §4.5 Fallback zinciri (örnek)

```text
  PRIMARY: Internal Free Model
       │
       ├── FAILED / UNAVAILABLE ──► OpenCode
       │                              │
       │                    FAILED ──► Codex
       │                              │
       │                    FAILED ──► Claude Code
       │                              │
       │                    FAILED ──► Abacus AI CLI
       ▼
  Kullanıcı hiçbir provider'ı belirtmediyse zincir ÖRNEKTİR;
  gerçek öncelik kullanıcı/proje policy'siyle belirlenir.
```

**Kural:** Kullanıcı açıkça "Claude Code kullan" dediyse sistem sessizce başka
provider'a **geçmez**.

---

## §5 Session Agentic Agent Wizard

### §5.1 Wizard akışı

```text
  ┌────────┐   ┌────────────────┐   ┌────────────────────────────┐
  │  CHAT  │──>│ INTENT ANALYSIS│──>│      WIZARD OPEN           │
  └────────┘   └────────────────┘   └────────────┬───────────────┘
                                                  │
                       ┌──────────────────────────┼──────────────────────────┐
                       ▼                          ▼                          ▼
              ┌────────────────┐        ┌──────────────────┐      ┌──────────────────┐
              │ Chat Session   │        │ Agent Session    │      │ Advanced         │
              │ · oturum tipi  │        │ · rol & model    │      │ · tool/MCP izin  │
              │ · bağlam       │        │ · hiperparametre │      │ · policy         │
              └────────────────┘        └──────────────────┘      └──────────────────┘
                                                  │
                                                  ▼
                                   ┌────────────────────────────┐
                                   ��� CONFIGURATION VALIDATION   │
                                   └────────────┬───────────────┘
                                                │ "Bitti"
                                                ▼
                                   ┌────────────────────────────┐
                                   │ IMMUTABLE SESSION          │
                                   │ CONFIGURATION              │
                                   └────────────┬───────────────┘
                                                │
                                                ▼
                                   ┌────────────────────────────┐
                                   │ SESSION START → RUNTIME    │
                                   └────────────────────────────┘
```

### §5.2 Oturum tipleri

| # | Tip | Ne zaman |
|---|---|---|
| 1 | **Single Agent Session** | Hızlı analiz, tekil dosya düzenlemesi |
| 2 | **Multi-Agent Swarm Session** | Paralel mimar, kodlayıcı, testçi, denetleyici |
| 3 | **Chained Pipeline Session** | Sıralı onay/aşamalı refactoring akışları |
| 4 | **Distributed Node Session** | Farklı MCP / arka plan sunucularına dağıtılmış görev |

### §5.3 "Bitti" butonu — deterministik adımlar

```text
┌──────────────────────────────────────────────────────────┐
│  1. [WIZARD CONFIG SPEC] ──► Immutable JSON serialize    │
│  2. [PROMPT COMPILER]    ──► Ajan başına Meta-System-Prompt │
│  3. [DAG WORKFLOW]       ──► Ajanlar arası görev grafiği │
│  4. [SESSION REGISTRY]   ──► Oturum + audit log kaydı    │
│  5. [RUNTIME LAUNCH]     ──► Orchestrator yürütme başlar │
└──────────────────────────────────────────────────────────┘
```

**Prompt sentezi sırası:**

```text
Global Rules ──► Project Rules ──► Session Rules ──► Agent Role
      │
      ▼
Task ──► Context ──► Policy ──► Provider Constraints ──► Tool Permissions
      │
      ▼
Agent Execution Instruction (çıktı)
```

**Yasak:** User requirement değiştirilemez · Provider capability uydurulamaz ·
Security rule düşürülemez · Permission genişletilemez · Bilinmeyen varsayılamaz.

### §5.4 Adaptif Soru Motoru (kaynak: ai-agentic-promt-maker + Electronic-gpt)

Wizard ya da intent analizi sırasında eksik bilgi varsa ajan **adaptif soru** sorar;
statik questionnaire değildir. Her cevap analiz edilir, bir sonraki soru ona göre
belirlenir, aynı soru **ikinci kez sorulmaz**.

```text
  ┌──────────────┐   ┌──────────┐   ┌─────────────────┐
  │  CEVAP A     │──>│  ANALİZ  │──>│ EKSİK BİLGİ     │
  └──────────────┘   └──────────┘   │ (state güncelle)│
                                    └────────┬────────┘
                                             │
                                             ▼
                                    ┌─────────────────┐
                                    │  SORU B         │──► (döngü)
                                    └─────────────────┘
                                             │
                          kritik eksik kalmadı?
                                             │ EVET
                                             ▼
                                    ┌─────────────────┐
                                    │DISCOVERY COMPLETE│
                                    └─────────────────┘
```

**Soru derinliği (7 boyut):** `WHAT?` · `WHY?` · `HOW?` · `WHO?` · `WHEN?` ·
`CONSTRAINT?` · `FAILURE CASE?` · `SECURITY IMPACT?` · `ACCEPTANCE CRITERIA?` —
ama yalnız kritik requirement için; kullanıcı zaten cevapladıysa **tekrar sorulmaz**.

**Soru belleği (state):**

```text
[ANSWERED]  Dil = C#
[ANSWERED]  Katman = l0..l7 (ADR-028)
[UNKNOWN]   Test kapsamı
[UNKNOWN]   Deployment hedefi
```

Bir bilgi kesinleştiğinde tekrar sorma; çelişki varsa `CONFLICT DETECTED` yaz ve
**yalnız** çelişen noktayı netleştiren tek soruyu sor.

**Soru öncelikleri (Electronic-gpt §22):** `SAFETY → POWER/ELECTRICAL → FUNCTION →
COMPONENT → MECHANICAL → PROCUREMENT → COST` — basit projede gereksiz soru sorma,
karmaşık projede kritik bilgiyi atlama (`N = yalnızca gerekli soru sayısı`).

**Soru formatı (zorunlu):**

```text
┌──────────────────────────────────────────────────────┐
│ GEREKLİ SORULAR — Toplam: N                         │
│ 1. [SORU]                                            │
│    Beklenen cevap: [FORMAT]                          │
│ 2. [SORU]                                            │
│    Beklenen cevap: [FORMAT]                          │
└──────────────────────────────────────────────────────┘
```

Numaralı cevap iste (`1: 12V DC` gibi); kullanıcı "bilmiyorum" derse bilgi
**uydurma** — `[UNDECIDED]` / `[VERIFY REQUIRED]` kullan.

**Bitirme kapısı:** Intent · Objective · Technology · Scope · Requirements ·
Constraints net değilse soru sormaya **devam et**; hepsi netse `DISCOVERY COMPLETE`
durumuna geç ve kullanıcı "devam/yeterli" diyene kadar final üretme.

**İlgili:** [ROLE](role.md) §4 (tek soru · A/B/C · süre) · [engine](engine.md) §1.1.1 (prompt işleme hattı) · [ULTRA-THINKING](ultra-thinking.md) §1.1 (tetik kriterleri) · [glossary](glossary.md) §136 (adaptif soru motoru).
### §5.5 Prompt mimarisi (kaynak: ai-agentic-promt-maker §15)

Prompt üretilecekse türüne göre iskelet seçilir; ilgisiz bölüm eklenmez:

| Prompt türü | İskelet |
|---|---|
| SYSTEM PROMPT | `ROLE → CONTEXT → OBJECTIVE → RULES → CONSTRAINTS → WORKFLOW → OUTPUT → VALIDATION` |
| TASK PROMPT | `TASK → INPUT → REQUIREMENTS → CONSTRAINTS → ACCEPTANCE CRITERIA → OUTPUT` |
| CODE REVIEW | `CONTEXT → CODE → ANALYSIS → FINDINGS → RISK → FIX → VERIFICATION` |
| SECURITY AUDIT | `SCOPE → ASSET → ATTACK SURFACE → THREATS → FINDINGS → IMPACT → MITIGATION → VERIFICATION` |
| ARCHITECTURE | `REQUIREMENTS → CONTEXT → DOMAIN → COMPONENTS → DATA FLOW → API → SECURITY → TRADE-OFFS → ADR` |

**Karar öncelik sırası (ai-agentic-promt-maker §19):** `User Intent → Explicit
Requirements → Explicit Constraints → Existing Structure → Correctness → Security →
Maintainability → Performance → Simplicity` — çelişki gizlenmez, açıkça belirtilir.

---

**İlgili:** [keys](../governance/keys.md) §1 satır 100 (adaptif soru) · [engine](engine.md) §1.1.1 (prompt sentezi) · [ROLE](role.md) §3 (karar hiyerarşisi) · [brain](../.decisions/brain.md) §2 (SOLID kısıtları).
## §6 Orchestrator akışı

### §6.1 Orchestrator sorumlulukları

```text
  ┌──────────┐
  │  INTENT  │
  └────┬─────┘
       ▼
  ┌──────────┐
  │   TASK   │
  └────┬─────┘
       ▼
  ┌──────────┐
  │   PLAN   │
  └────┬─────┘
       ▼
  ┌─────────────────┐
  │ AGENT SELECTION │  internal mı external mı?
  └────┬────────────┘
       ▼
  ┌──────────┐
  │ WORKFLOW │
  └────┬─────┘
       ▼
  ┌──────────┐
  │EXECUTION │
  └────┬─────┘
       ▼
  ┌────────────┐
  │ VALIDATION │
  └────┬───────┘
       ▼
  ┌──────────┐
  │  RESULT  │──► CHAT
  └──────────┘
```

**Orchestrator YAPMAZ:** UI logic · Database logic · CLI syntax ·
Provider-specific API logic.

### §6.2 Multi-Agent / Swarm routing

```text
                    ┌──────────────┐
                    │ ORCHESTRATOR │
                    └──────┬───────┘
           ┌───────────────┼───────────────┐
           ▼               ▼               ▼
      ARCHITECT       DEVELOPER       RESEARCHER
           │               │               │
           └───────────────┼───────────────┘
                           ▼
                      REVIEWER
                           │
                           ▼
                      SECURITY
                           │
                           ▼
                        TEST
                           │
                           ▼
                        VERIFY
```

**Routing kuralı:**

```text
  Basit görev  ──► Single Agent (tek ajan yeter)
  Karmaşık görev ──► Multi-Agent Workflow (gerçek gereksinim varsa)
```

> **Her görev Multi-Agent olmak zorunda değildir.** Gereksiz ajan = gereksiz
> maliyet + gereksiz complexity.

### §6.3 Agent handoff (iç → harici zincir)

```text
  Architect Agent
       │  Implementation Specification
       ▼
  External Coding Agent
       │  Implementation
       ▼
  Testing Agent
       │
       ▼
  Reviewer Agent
```

**Kural:** Her handoff açık bir **contract** ile yapılır; sözel devir geçersizdir
(bkz. §7.1 devre paketi).

---

## §7 Devre paketi, 3-STRIKE ve eşzamanlılık

### §7.1 Devre paketi (4 madde zorunlu)

```text
┌──────────────────────────────────────────────────────┐
│ 1. KALAN İŞ  : ne bitti, ne kaldı (madde madde)      │
│ 2. DOSYA      : dokunulan dosya yolları + satır aralığı│
│ 3. KANIT      : eldeki kanıt (build/test çıktısı)    │
│ 4. SINIR      : aşılmayacak katman/kapsam sınırı     │
└──────────────────────────────────────────────────────┘
```

- **Alıcı hakkı:** 4 madde eksikse devri geri çevir, `bekle` durumuna geç.
- Devir paketi en fazla ~2 KB.

### §7.2 3-STRIKE

```text
Strike 1 ──► aynı yöntemle tekrar dene (küçük düzeltme)
Strike 2 ──► yöntemi değiştir (farklı yaklaşım, aynı hedef)
Strike 3 ──► DUR — başka ajana devret veya kullanıcıya sor
```

**Bloke raporu şablonu:**

```text
┌──────────────────────────────────────────────────────┐
│ ETIKET     : 3-STRIKE | GATE-OPEN | G7 ONAYI |       │
│              UNKNOWN | CONFLICT DETECTED              │
│ KONUM      : <dosya yolu> : <satır> ya da <gate adı> │
│ NEDEN      : <denenip düşen şey, 1 cümle>            │
│ VARSAYIM   : <şüpheli varsayım, tek madde>           │
│ SECENEKLER : A) ...  B) ...  C) ...                  │
│ SORU       : <kullanıcıya tek soru>                  │
└──────────────────────────────────────────────────────┘
```

**Strike sayılmayanlar:** rate limit · model unavailable · bütçe aşımı ·
kullanıcının yeni veri getirmesi (dışsal kesinti sayaç işletmez).

### §7.3 Eşzamanlılık (en fazla 3)

```text
  [ A: mimari inceleme ]──┐
  [ B: kod yazımı      ]──┼──► eşzamanlı (sınır: 3 ajan)
  [ C: test yazımı     ]──┘
          │
          └──�� aynı dosya? ──► SERİ (sırala)
               farklı dosya? ──► PARALEL
```

**Kaynak:** 2026-09-29 kaza kaydı — 8 paralel ajan çağrısı `Rate limit exceeded`
+ `Model unavailable` ile düştü; sınır olayın kendisiyle belirlendi.

- **Tek dosya = tek sahip:** iki ajan aynı dosyaya aynı anda yazamaz.
- Paralel blok kapanırken her ajan kendi §3 kutusunu döner.

---

## §8 Deterministik kontrol ve Zero-Hallucination

### §8.1 Probabilistic vs Deterministic

```text
┌────────────────────────────────────────────────────┐
│  LLM = Probabilistic Intelligence                  │
│  (aynı girdiye farklı çıktı verebilir)             │
├────────────────────────────────────────────────────┤
│  VersaCoder Control Plane =                        │
│  Deterministic Rules + Policy + State + Validation │
│  (compiler, test runner, static analyzer)          │
└────────────────────────────────────────────────────┘
```

**Ayrım zorunlu:** niyeti anlamak → LLM; kodun derlenip derlenmediği → compiler;
testin geçip geçmediği → test runner; configuration validation → deterministic
mekanizma.

### §8.2 Zero-Hallucination yasak listesi

Aşağıdakiler **asla tahmin edilmez**:

```text
┌──────────────────────────────────────────────────────┐
│ CLI command · Executable name/path · Version        │
│ API endpoint · Authentication method                │
│ Environment variable · Tool capability              │
│ Provider capability · Model availability            │
│ Pricing · Free-tier status · MCP server             │
│ Configuration syntax                                │
└──────────────────────────────────────────────────────┘
```

**Bilinmiyorsa:** `[VERIFY REQUIRED]` ya da `DOĞRULAMA GEREKLI` yaz, tahmin etme.

### §8.3 Doğrulama zinciri (External Agent "tamamlandı" dese bile)

```text
  External Agent
       │  Code Changes
       ▼
  Build Verification
       │
       ▼
  Tests
       │
       ▼
  Static Analysis
       │
       ▼
  Requirement Validation
       │
       ▼
  Review
       │
       ▼
  Final Result ──► "VERIFIED" yalnız burada
```

> Test edilmemiş bir sonuç `VERIFIED` olarak işaretlenmez.
> External Agent "tamamlandı" dedi diye işlem otomatik verified **kabul edilmez**.

### §8.4 Error handling

```text
  ERROR ──► ROOT CAUSE ──► IMPACT ──► RESOLUTION ──► VERIFICATION
```

"CLI çalışmadı" yeterli değildir; ayrıştırılır: process başlatılamadı mı ·
executable bulunamadı mı · auth başarısız mı · timeout mu · exit code ne ·
stderr ne diyor · permission mı · network mı · provider unavailable mı.
Bilinmeyen neden: `UNKNOWN`.

### §8.5 Görsel Üretim Hattı (Visual Production — Studio/IDE)

> VersaCoder yalnız kod IDE'si değil, **Studio**'dur: dokümantasyon + görsel
> üretim de desteklenir. Bu hat üç prompt kaynağından sentezlenmiştir:
> `ai-vision-render-studio` · `girl-image-analzye-promt` · `girl-image-8k-generator`.

**Hattın 8 adımı:**

```text
┌──────────────┐   ┌──────────────┐   ┌──────────────────┐
│ 1. INPUT     │──>│ 2. ANALYZE   │──>│ 3. UNDERSTAND    │
│ (görsel/fikir)│   │ (subject/    │   │ (niyet + hedef)  │
└──────────────┘   │  camera/light│   └────────┬─────────┘
                   └──────────────┘            │
┌──────────────┐   ┌──────────────┐   ┌────────▼─────────┐
│ 8. FINAL     │<──│ 7. OPTIMIZE  │<──│ 4. EXTRACT       │
│ (prompt/     │   │ (negative +  │   │ (parametreler)   │
│  render)     │   │  model-opt)  │   └────────┬─────────┘
└──────────────┘   └──────────────┘            │
                                        ┌────────▼─────────┐
                                        │ 5-6. PLAN→PROMPT │
                                        └──────────────────┘
```

**Analiz katmanları (visual-analzye §3-§9):**

| # | Katman | Çıkarılan |
|---|---|---|
| 1 | Composition | framing · subject placement · foreground/background · rule of thirds |
| 2 | Subject / Character | ana/ikincil konu · insan varlığı · poz · kıyafet |
| 3 | Camera & Cinematography | pozisyon · açı · perspektif · focal length · DoF |
| 4 | Lighting | key/fill/rim/backlight · yön · yoğunluk · gölge kalitesi |
| 5 | Color & Grading | baskın renkler · beyaz dengesi · contrast · saturation · cinematic grading |
| 6 | Environment | arka plan · ortam · atmosfer |
| 7 | Style Classification | photorealistic · cinematic · anime · concept art · product |
| 8 | Micro-detail | doku · malzeme · kumaş · ten · saç · yüzey kusurları |

**Görsel üretim modları (vision-render):**

```text
TEXT_TO_IMAGE · IMAGE_TO_IMAGE · IMAGE_EDITING · IMAGE_RESTORATION
PRODUCT_RENDER · ARCHITECTURAL_RENDER · CHARACTER_RENDER
UI_UX_RENDER · PHOTOGRAPHIC_TRANSFORMATION
```

**Hedef model optimizasyonu** (yalnız kullanıcı belirtirse; bilinmeyen model
`[VERIFY REQUIRED]`): `Nano Banana Pro 2` · `FLUX` · `SDXL` · `Pony/Illustrious` ·
`Midjourney` · `GPT Image` · `ComfyUI`.

**Kalite kapısı (8K generator §24 + visual-analzye §28):**

```text
[ ] Niyet (user intent) korundu
[ ] Referans yapı korundu (image-to-image'da identity preservation)
[ ] Parametreler uydurulmadı — lens/model/çözünürlük [VERIFY] ise öyle kaldı
[ ] Negative prompt ilgili (yalnız gerçek riskler)
[ ] 4K/8K enhancement iddiası kanıtsız değil (yalnız çözünürlük = kalite DEĞİL)
[ ] Test edilmemiş output "production ready" DEĞİL
[ ] Confidence sistemi dolu (HIGH/MEDIUM/LOW + gerekçe)
```

**Çıktı iskeleti (8K generator §22):**

```text
┌──────────────────────────────────────────────────────┐
│ WALLPAPER/RENDER TITLE : <tek satır>                │
│ TARGET                 : 4K / 8K · aspect ratio     │
│ CAMERA                 : lens · açı · DoF           │
│ LIGHTING               : key/fill/rim · yön         │
│ COMPOSITION            : framing · hiyerarşi        │
│ MAIN PROMPT            : tam prompt metni           │
│ NEGATIVE PROMPT        : ilgili negatifler          │
│ ENHANCEMENT            : upscaler/model notu        │
│ WALLPAPER NOTES        : güvenlik/uygunluk notu     │
└──────────────────────────────────────────────────────┘
```

**Sınır (Zero-Hallucination görselde):** bilinmeyen lens değeri, çözünürlük,
render engine, model parametresi, malzeme özelliği **uydurulmaz** →
`[BELİRTİLMELİ]` / `[ESTIMATED LENS]` / `[VERIFY REQUIRED]`. Referans görsel
varsa o **visual source of truth**'tur; kimlik/yüz/oranlar kullanıcı istemediği
sürece korunur (identity preservation).

**Studio/IDE kimliği:** VersaCoder bu hattı `05-ai-chat` (prompt üretimi) +
`06-agent-panel` (görev kuyruğu) üzerinden çalıştırır; render çıktısı seçilen
görsel üretim sağlayıcısında üretilir ve §8.3 doğrulama zincirinden geçer.

**Sağlayıcı entegrasyonu:** port/adapter yerleşimi, **üç sağlayıcı yolu (A/B/C)**,
desteklenen model listesi ve açık konular
[ADR-031](../.decisions/draft/adr-031-visual-production-provider.md) dosyasındadır
(**`proposed`** — kapsam kullanıcı onaylı 2026-10-07 "hepsi"; yol A endpoint'i
`WEB_VERIFIED`; `src/**` kodu **yok**, G12).

---

**İlgili:** [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md) §5.1-§5.2 (sağlayıcı doğrulaması) · [VISION](vision.md) §0.3 (Studio/IDE kimliği) · [glossary](glossary.md) §131-§136 (görsel terimler) · [keys](../governance/keys.md) §98-§101 · [README](../../readme.md) §1.
## §9 System Quality Gate (50 madde)

Bu sistemi tanımlarken aşağıdakileri kontrol et:

```text
┌──────────────────────────────────────────────────────┐
│ □ Chat ve Agent ayrımı korunuyor mu?                 │
│ □ Agent ve LLM ayrımı korunuyor mu?                 │
│ □ Internal ve External Agent ayrımı korunuyor mu?    │
│ □ External CLI kontrolsüz bırakılmış mı?             │
│ □ Session scope tanımlı mı?                         │
│ □ Working directory kontrol ediliyor mu?             │
│ □ Permission boundary var mı?                       │
│ □ Approval mekanizması var mı?                      │
│ □ Result VersaCoder'a geri geliyor mu?               │
│ □ Raw CLI output normalize ediliyor mu?              │
│ □ Validation yapılıyor mu?                          │
│ □ Audit yapılabiliyor mu?                           │
│ □ Failure durumunda güvenli davranılıyor mu?        │
│ □ Fallback policy açık mı?                          │
│ □ Provider capability doğrulanıyor mu?               │
│ □ Bilinmeyen CLI/API bilgileri uydurulmuş mu?        │
│ □ Gereksiz Multi-Agent complexity eklenmiş mi?       │
│ □ Kullanıcı tercihi sessizce override ediliyor mu?   │
└──────────────────────────────────────────────────────┘
```

---

## §10 Doğrulama

- [ ] **Temel ayrım korunuyor:** Chat ≠ Agent · Agent ≠ LLM · LLM ≠ VersaCoder
- [ ] **Kutu zorunlu:** [§3](#3-çıktı-kutusu-zorunlu-format) kutusu eksikse çıktı işlenmez
- [ ] **Doğru ajan:** iş [§2](#2-iş-tipi--ajan-matrisi) matrisine göre atanmış
- [ ] **Devir paketi:** 4 madde dolu ([§7.1](#71-devre-paketi-4-madde-zorunlu))
- [ ] **Eş zaman:** ≤ 3 ajan, tek dosya sahipliği aşılmaz
- [ ] **External Agent:** adapter sözleşmesi + validation zinciri uygulanmış
- [ ] **Zero-Hallucination:** doğrulanmamış CLI/API bilgisi yazılmamış
- [ ] **Report:** iş sonunda rapor biçimi [WORKFLOW](../governance/workflow.md) §5'e göre

### §10.1 Geniş kontrol listesi (batch kapanışında 20 madde)

- [ ] 1. İstek §2 matrisinde mi? Yoksa `general` kullanıldı mı?
- [ ] 2. Chat ≠ Agent ≠ LLM ayrımı korundu mu?
- [ ] 3. Aynı anda çalışan ajan ≤ 3 mü?
- [ ] 4. Aynı dosyaya ikinci yazıcı yok mu?
- [ ] 5. Dosya sahipliği doğru ajan üzerinde mi?
- [ ] 6. Her ajan 5 alanlı kutusunu döndü mü?
- [ ] 7. `KANIT` satırı somut çıktı içeriyor mu?
- [ ] 8. Gate'in 4 satırlı kanıt kutusu ajan kutusuyla karıştırıldı mı?
- [ ] 9. Mimari etkili çıktılar 13 sorudan geçti mi?
- [ ] 10. `.Designer.cs` / `.resx` işlemleri `ui-designer`'dan mı geçti?
- [ ] 11. Devir olduysa 4 madde dolu mu?
- [ ] 12. Devir zinciri 3 hamle sınırını aştı mı?
- [ ] 13. Aynı işte sayaç 3'e ulaştı mı? Ulaştıysa bloke raporu yazıldı mı?
- [ ] 14. Dışsal kesinti (rate limit/model/bütçe) var mı? Sayaç işletildi mi?
- [ ] 15. Çıktı bütçesi aşıldı mı? Aşıldıysa özetlendi mi?
- [ ] 16. External Agent kullanıldıysa validation zinciri çalıştırıldı mı?
- [ ] 17. Kullanıcı provider tercihi sessizce değiştirildi mi?
- [ ] 18. Rapor biçimi WORKFLOW §5'e uygun mu?
- [ ] 19. Kural değişikliği varsa `log.md`'ye tarihli yazıldı mı?
- [ ] 20. Kanıtsız "bitti/tamam" ifadesi satır satır var mı?

**Sayım notu:** 20 madde = 20 işaret; 19 işaretli 1 boşsa iş **tamamlanmış sayılmaz**.

---

## §11 Bütçe (token / byte)

| Kalem | Sınır | Aşımında |
|---|---|---|
| Tek ajan çıktısı | en fazla ~8 KB | Özetle; detayı [MEMORY](../memory/memory.md)'e taşı |
| Devre paketi | en fazla ~2 KB | 4 maddeyi koru, açıklamayı kısalt |
| Aynı anda dokunulan dosya | en fazla ~8 dosya | Bölümlere ayır; [WORKFLOW](../governance/workflow.md)'e geç |
| Bir oturumda toplam dosya | en fazla ~3 dosya | Yeni oturuma böl, kanıtı [log.md](../ops/log.md)'e yaz |
| Bloke raporu sorusu | en fazla 1 soru | Kalanı SECENEKLER'e taşı |

- `~` ile yazılan limitler token/byte kestirimidir (`INFERRED`); kesin ölçüm gerekirse sayacla.

---

## §12 Referanslar

- [index.md](../index.md) — vault navigasyonu
- [MEMORY.md](../memory/memory.md) / [log.md](../ops/log.md) — rapor kanıtı
- [engine.md](engine.md) — orkestrasyon
- [brain.md](../.decisions/brain.md) — beyin yapısı
- [VISION.md](vision.md) / [PROJECTS.md](projects.md) — bağlam
- [keys.md](../governance/keys.md) — anahtarlar / [glossary.md](glossary.md) — terimler
- [ULTRA-THINKING.md](ultra-thinking.md) — uzun düşünme
- [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) — katman temeli
- [architecture/index.md](../architecture/index.md) — mimari indeks
- [checklists/layer-boundary-check.md](../checklists/layer-boundary-check.md) — 13 soru
- [CLAUDE.md](../claude.md) / [WORKFLOW.md](../governance/workflow.md) / [ROLE.md](role.md) — kardeş dosyalar
- Prompt arşivi — VersaCoder: `G:/…/versacoder proje planlaması ai.00..09.md` ·
  `proje-ai.00.md` · `coremusic-vault-template.md`
- Prompt arşivi — ChatGpt (ajan içeriği): `ai-agentic-promt-maker.md` (adaptif soru
  motoru, prompt mimarisi) · `Yazımı-düzelt-promt-maker.md` (dil politikası, 12 kural) ·
  `ui-uix-anayze-promt-md.md` (UI analiz) · `ui-uix-anayze-promt-md-tomd.md` (ASCII
  referans) · `Electronic-gpt-özle-promt.md` (soru döngüsü, final status)
- Prompt arşivi — **100 part analiz seti**: `G:/Drive'ım/Projelerim/Project-notes/Bayram
  Ali Prompt Arşivi & Notlar/Versacoder/versacoder proje-ai.00.md` (13 blok / 100 part
  haritası) — **depo dışı hedef olduğu için link değil, düz metin yol** (göreli link
  `..` ile başka sürücüye ulaşılamaz)
| kok README | [kok README](../../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok WORKFLOW | [kok WORKFLOW](../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
| kok AGENTS | [kok AGENTS](../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
