---
title: "VersaCoder — Ana Yürütme Sırası & GATE Sistemi"
type: workflow
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "P1..P8 master yürütme sırası, GATE-001..006 giriş/çıkış kriterleri, katman inşa sırası, DoD, günlük döngü, branch/commit, escalation, bloke protokolü, rapor formatı"
---

# VersaCoder — Ana Yürütme Sırası (`WORKFLOW.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[AGENTS.md]] · [[ROLE.md]]

> **Tek cümlelik sorumluluk:** *Hangi iş hangi sırayla, hangi gate'ten geçer, ne zaman durulur, nasıl raporlanır* sorusunun tek cevabıdır.
> Bu dosya **gate kimliğinin ve part sırasının SSOT**'udur. Ajan tanımı [`.agents/AGENTS.md`](.agents/AGENTS.md) ve protokol [AGENTS.md](AGENTS.md) içindedir; rol/ses kuralları [ROLE.md](ROLE.md)dadır.

---

## §0 Özet (Answer First)

Sekiz part **sırayla** yürür: `P1 kaynak okuma → P2 kapsam → P3 vault → P4 wireframe → P5 iskelet+kod → P6 test → P7 opencode entegrasyon → P8 kapanış`. Toplam bütçe **≈8 sa 10 dk**. Aralarda **6 gate** var; **GATE-003 (UI onayı) herhangi bir kodlamadan önce HARD STOP**'tur ve yalnız kullanıcı kapatabilir. Katman inşa sırası sabittir: `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`. Rapor her turda `(n/8)` + cevap önce + ≤ 5 madde + somut süre (`≈15 dk`) biçimindedir. Depoda şu an **commit yok**; ilk commit GATE-004 kapanınca atılır (§7).

---

## §1 Pipeline Genel Görünüm

### §1.1 Tam boru hattı (ASCII)

```
                    [ KULLANICI ]
                         |
                         v
 +-----------+   GATE-001   +-----------+   GATE-002   +-----------+
 | P1 KAYNAK | -----------> | P2 KAPSAM | -----------> | P3 VAULT  |
 |   OKUMA   |  requirements |  ANALIZ   |    scope     |  KURULUM  |
 |  ~45 dk   |   freeze  ✓   |  ~30 dk   |   freeze  ✓  |  ~45 dk   |
 +-----------+               +-----------+              +-----+-----+
                                                               |
                                    +--------------------------+
                                    v
 +-----------+   GATE-003     +-----------+   GATE-004   +-----------+
 | P4 WIREFR | -------------> | P5 ISKELET| -----------> | P6 TEST   |
 |  ~60 dk   |  UI approval   |  + KOD    |  build green |  ~120 dk  |
 +-----+-----+   HARD STOP    |  ~90 dk   |     ✓        +-----+-----+
       |                       +-----------+                   |
       |  (onay yoksa KOD YOK)                                 v
       |                                             GATE-005
       |                                          test green  ✓
       v                                                       |
 +-----------+                                                 |
 | P7 OPENCD | <-----------------------------------------------+
 |  ~60 dk   |
 +-----+-----+
       |
       v
 +-----------+   GATE-006   +-----------+
 | P8 KAPAN | ------------> | TESLIM    |
 |  ~30 dk  |    ship  ✓    | raporu    |
 +-----------+               +-----------+

 KATMAN INSA SIRASI (P5 icinde, ok yonu asla ters degilmez):
   Abstractions -> Domain -> Application -> Infrastructure -> Protocol -> Host -> UI
```

### §1.2 Sıra kuralı

| # | Kural | İhlalde |
|---|---|---|
| 1 | Part'lar **sırayla** yürür; atlanan part geri gelir | Sıra RED → part baştan |
| 2 | Gate **yalnız giriş/çıkış kriteri** tamamlayınca kapanır | Sahte kapatma = G5 ihlali |
| 3 | GATE-003 kapandıktan **önce** `.cs` kodu yazılamaz | G12 → iş RED |
| 4 | Geri dönüş yalnız **RED raporu** ile olur (geriye sarım serbest) | Sessiz geri sarım yasak |
| 5 | Kapsam değişikliği gate'i yeniden açar | Kapsam kayması |

### §1.3 Zaman Bütçesi Özeti

| Part | Ad | Bütçe | Kümülatif | Gate |
|---|---|---|---|---|
| P1 | Kaynak okuma | ≈45 dk | ≈45 dk | GATE-001 |
| P2 | Kapsam analizi | ≈30 dk | ≈1 sa 15 dk | GATE-002 |
| P3 | Vault kurulumu | ≈45 dk | ≈2 sa | GATE-004a (vault) |
| P4 | Wireframe + ASCII | ≈60 dk | ≈3 sa | **GATE-003** |
| P5 | İskelet + kod | ≈90 dk | ≈4 sa 30 dk | GATE-004 (build) |
| P6 | Test | ≈120 dk | ≈6 sa 30 dk | GATE-005 |
| P7 | opencode entegrasyonu | ≈60 dk | ≈7 sa 30 dk | — (GATE-005 devam) |
| P8 | Kapanış | ≈40 dk | **≈8 sa 10 dk** | GATE-006 |

> **KURAL:** Bütçe **tahmin değil, tavadır**. Aşım 20 dk'yu geçerse o part durdurulur, [AGENTS.md](AGENTS.md) §8 escalation'a geçilir.

---

## §2 P1..P8 — Part Ayrıntıları

> Her part için aynı 6 alan yazılır: **Girdi · Çıktı · Bütçe · Gate · Adımlar · Çıkış kontrolü**.

### §2.1 P1 — Kaynak Okuma (≈45 dk)

| Alan | Değer |
|---|---|
| **Girdi** | `G:\…\Bayram Ali Prompt Arşivi & Notlar\Versacoder\` altındaki `ai.01/02/03.md`, `notes.prompt.md`, `notes.prompt2.txt`, opencode editleme notları |
| **Çıktı** | Okuma notları + doğrulanmış karar listesi (ham veri yok) |
| **Bütçe** | ≈45 dk |
| **Gate** | **GATE-001 — Requirements Freeze** |
| **Sorumlu** | inline (tek oturum) · sonuç `reports/00-kapsam-analizi.md` |

**Adımlar:**

| # | Adım | Süre | Not |
|---|---|---|---|
| 1 | `ai.03.md` oku (**bootstrap kaynağı**) | ≈10 dk | CR-only ise okuma anında normalize (`\r`→`\n`) |
| 2 | `ai.02.md` (Discovery/Q&A) tara | ≈8 dk | 5 senior kararın gerekçesi |
| 3 | `ai.01.md` yalnız **yapıbilgi** tara | ≈8 dk | Teknoloji sürümleri **ASLA** alınmaz (R3) |
| 4 | opencode editleme notları tara | ≈10 dk | PART 7 girdisi |
| 5 | Çelişkileri işaretle | ≈5 dk | `CONFLICT DETECTED` / `DECISION BLOCKED` |
| 6 | Doğrulanabilir + doğrulanamayan listesi | ≈4 dk | `UNKNOWN` / `NEEDS_VALIDATION` |

**Çıkış kontrolü:** 3 plan dosyası okundu ✓ · CR-only normalize ✓ · teknoloji sürümü ai.01'den alınmadı ✓ · çelişki listesi yazılı ✓ → **GATE-001 kapanır**.

> **UYARI:** P1'de **hiçbir dosya yazılmaz**, yalnız okunur. Çıktı bir sonraki partın girdisidir; P1'i atlayan P2 tahminle yazar (G9 ihlali).

---

### §2.2 P2 — Kapsam Analizi (≈30 dk)

| Alan | Değer |
|---|---|
| **Girdi** | P1 okuma notları |
| **Çıktı** | `reports/00-kapsam-analizi.md` — girdi envanteri, 5 karar gerekçesi, risk/belirsizlik/varsayım, envanterler, DoD |
| **Bütçe** | ≈30 dk |
| **Gate** | **GATE-002 — Scope Freeze** |
| **Sorumlu** | `architect` (inline ya da subagent) |

**Adımlar:**

| # | Adım | Süre | Çıktı |
|---|---|---|---|
| 1 | Girdi envanteri (dosya + boyut + satır sonu) | ≈5 dk | Tablo |
| 2 | 5 senior kararın gerekçesi | ≈7 dk | Karar tablosu (kaynak çelişkisi ile) |
| 3 | Risk (R1..R6) / belirsizlik (B1..B4) / varsayım (A1..A3) | ≈8 dk | 3 tablo |
| 4 | Envanterler: 7 ekran · ADR · gate | ≈6 dk | 3 tablo |
| 5 | Kapsam dışı listesi + DoD | ≈4 dk | Sınır çizgisi |

**Çıkış kontrolü:** 7 ekran listesi sabit ✓ · kapsam dışı açık ✓ · riskler azaltma aksiyonlu ✓ → **GATE-002 kapanır**.

> **KURAL:** GATE-002 kapandıktan sonra **kapsam dışına çıkılmaz**. Yeni istek gelirse → kapsam değişikliği, gate yeniden açılır, kullanıcı onayı istenir ([AGENTS.md](AGENTS.md) §3.2 #11).

---

### §2.3 P3 — Vault Kurulumu (≈45 dk)

| Alan | Değer |
|---|---|
| **Girdi** | `.templates/` (24 hazır dosya) · `reports/00-kapsam-analizi.md` |
| **Çıktı** | `.agents/` (5 ajan + kayıt) · kök MD'ler · `architecture/`, `.decisions/`, `.rules/`, `spec/`, `ui-design/`, `reports/`, `scripts/`, `.sql/`, `.obsidian/` |
| **Bütçe** | ≈45 dk |
| **Gate** | **GATE-004a — Vault green** (link + frontmatter) |
| **Sorumlu** | `architect` · şablon `.templates/documentation/root-md-template.md` |

**Adımlar (sıra zorunlu):**

```
1. .templates/  mevcut 24 dosyayi DOGRULA     (varlik + boyut)      ~5 dk
2. .agents/     5 rol + AGENTS.md kaydi        (agents-template.md) ~8 dk
3. KOK MD       index, AGENTS, CLAUDE, WORKFLOW,
                ROLE, keys, glossary, brain,
                MEMORY, VISION, PROJECTS, log,
                engine, ULTRA-THINKING          (30-88 KB bandi)    ~18 dk
4. Katmanlar    architecture/ .decisions/ .rules/
                spec/ ui-design/ reports/
                scripts/ .sql/ .obsidian/        (dizin + index)     ~8 dk
5. LINK TARAMA  scripts/ ile 0 bozuk link                            ~4 dk
6. log.md kaydi                                                    ~2 dk
```

**Çıkış kontrolü:** 14 kök belge var + frontmatter'lı ✓ · link taraması **0 bozuk** ✓ · `.agents/` dosya sayısı 5+1 ✓ · boyutlar 30–88 KB bandında ✓ → **GATE-004a kapanır**.

> **UYARI:** Bozuk link = G4 ihlali = RED. Henüz yazılmamış dosyaya link atılmaz; `status: planlı` işareti kullanılır.

---

### §2.4 P4 — Wireframe + ASCII (≈60 dk) → GATE-003

| Alan | Değer |
|---|---|
| **Girdi** | 7 ekran listesi (`reports/00` §4.1) · `VISION.md` / `PROJECTS.md` · kontrol adayları |
| **Çıktı** | `ui-design/01-start-page.md … 07-settings.md` + `flows.md` + `component-inventory.md` + `architecture/*.ascii` |
| **Bütçe** | ≈60 dk (7 ekran × ≈7 dk + ≈11 dk akış/envanter) |
| **Gate** | **GATE-003 — UI Approval (HARD STOP)** |
| **Sorumlu** | `ui-designer` · mimari kısıt `architect` |

**Adımlar:**

| # | Adım | Süre | Not |
|---|---|---|---|
| 1 | 7 ekranın kontrol setini kilitle | ≈10 dk | `reports/00` §4.1 adayları |
| 2 | Ekran başına ASCII wireframe | ≈7 dk/ekran | Saf ASCII, Unicode kutu **yok** |
| 3 | `flows.md` — ekranlar arası akış | ≈8 dk | Başlangıç → IDE → chat → ayarlar |
| 4 | `component-inventory.md` | ≈6 dk | DevExpress v26.1 kontrol listesi |
| 5 | Mimari diyagramlar (ASCII) | ≈7 dk | katman + bağımlılık yönü |
| 6 | Kullanıcıya sun + **onay bekle** | ≈12 dk | **DUR** |

**Çıkış kontrolü:** 7 dosya var ✓ · her biri frontmatter'lı ✓ · saf ASCII ✓ · kontrol listesi yalnız v26.1 ✓ · **kullanıcı onayı yazılı** ✓ → **GATE-003 kapanır**.

> **UYARI — HARD STOP:** GATE-003 kapandıysa **hiçbir `.cs` dosyası yazılamaz**. Onay alınmadan kod yazan ajan işi RED eder ve part baştan döner (G12). Bu, vault'un tek **durdurucu** gate'idir: diğer gate'ler kanıtla, bu gate **kullanıcı onayıyla** kapanır.

**Onay istemi şablonu:**

```markdown
GATE-003 - UI onayi bekleniyor (HARD STOP)
  Ekran   : 7/7 tamam (ui-design/01..07)
  Kontrol : DevExpress v26.1 + RibbonForm + Office2010
  Secenek : A) onayla  B) <ekran> icin degisiklik iste  C) 7 ekran listesini degistir
  Sure    : onay ~5 dk -> P5 hemen baslar; onaysiz P5 BASLAMAZ
```

---

### §2.5 P5 — İskelet + Kod (≈90 dk)

| Alan | Değer |
|---|---|
| **Girdi** | Kapalı GATE-003 · `spec/` · `.decisions/` (ADR) · `.rules/` |
| **Çıktı** | `VersaCoder.slnx`(veya `.sln`) + 7 katman + `VersaCoder.Desktop` + `VersaCoder.Cli` + yeşil `dotnet build` |
| **Bütçe** | ≈90 dk |
| **Gate** | **GATE-004 — Build Green** |
| **Sorumlu** | `architect` (iskelet) → `developer` (kod) |

**Adımlar:**

| # | Adım | Süre | Kanıt |
|---|---|---|---|
| 1 | Çözüm + `Directory.Build.props` + `Directory.Packages.props` (CPM) | ≈10 dk | Paket sürümü tek yerde |
| 2 | **Katman sırasıyla** iskelet (§4) | ≈25 dk | Proje referansları yön doğru |
| 3 | `net48` + `net10.0-windows` çift hedefleme | ≈10 dk | İki bacak derleniyor mu (R5!) |
| 4 | DevExpress v26.1 referansı + `RibbonForm` | ≈10 dk | İlk görsel doğrulama |
| 5 | Provider soyutlama + Dapper + parameterized | ≈15 dk | EF Core 0 referans |
| 6 | `dotnet build` → 0 hata (iki bacak) | ≈10 dk | **GATE-004 kanıtı** |
| 7 | `developer` → `reviewer` devre paketi | ≈10 dk | 4 madde |

**Çıkış kontrolü:** `dotnet build` **0 hata** × 2 bacak ✓ · EF Core 0 ✓ · DevExpress yalnız v26.1 ✓ · katman bağımlılığı NetArchTest ile doğrulanır ✓ → **GATE-004 kapanır**.

> **UYARI:** R5 riski (net48 + DevExpress v26.1 uyumluluğu) **P5'in ilk build'inde** erken doğrulanır. Sorun çıkarsa `DECISION BLOCKED` + kullanıcı raporu; sessizce sürüm düşürmek G8 ihlalidir.

---

### §2.6 P6 — Test (≈120 dk)

| Alan | Değer |
|---|---|
| **Girdi** | Yeşil build (GATE-004) · kabul kriterleri |
| **Çıktı** | `tests/**` + yeşil `dotnet test` + coverage kanıtı |
| **Bütçe** | ≈120 dk |
| **Gate** | **GATE-005 — Test Green** |
| **Sorumlu** | `tester` · PASS/RED `reviewer` |

**Adımlar:**

| # | Adım | Süre | Kanıt |
|---|---|---|---|
| 1 | Kabul kriterlerini `spec/`'ten listele | ≈10 dk | Kriter ↔ test eşlemesi |
| 2 | Birim testler (iş mantığı) | ≈40 dk | Yeşil |
| 3 | Entegrasyon testleri (SQLite + gerçek schema) | ≈35 dk | Yeşil |
| 4 | Mimari testler (NetArchTest: yön + circular) | ≈15 dk | 0 ihlal |
| 5 | Hata yolu + sınır testleri (null, timeout, bozuk input) | ≈15 dk | Yeşil |
| 6 | Coverage raporu + kapsam dışı gerekçesi | ≈5 dk | Sayı |
| 7 | `reviewer` son PASS'i → rapor | ≈10 dk | GATE-005 |

**Çıkış kontrolü:** `dotnet test` tümü yeşil ✓ · iki bacak ✓ · NetArchTest 0 ihlal ✓ · coverage kanıtı ya da gerekçe ✓ → **GATE-005 kapanır**.

---

### §2.7 P7 — opencode Entegrasyonu (≈60 dk)

| Alan | Değer |
|---|---|
| **Girdi** | `%TEMP%\opencode-fresh` (v1.18.32, 33 paket; `packages/protocol` 26 dosya, `packages/core` 491) |
| **Çıktı** | `spec/protocol-spec.md` · `spec/tool-spec.md` · `spec/session-spec.md` + ADR |
| **Bütçe** | ≈60 dk |
| **Gate** | — (GATE-005 devamı; tasarım gate'i değil) |
| **Sorumlu** | `architect` + `developer` |

**Adımlar:**

| # | Adım | Süre | Çıktı |
|---|---|---|---|
| 1 | `packages/protocol` mesaj şemasını oku | ≈15 dk | Mesaj tipleri |
| 2 | `packages/core` tool setini (`read`/`write`/`edit`/`bash`/`grep`) | ≈12 dk | Tool sözleşmesi |
| 3 | C# sözleşmesine çevir (interface + DTO) | ≈15 dk | `spec/protocol-spec.md` |
| 4 | LLM provider öncelik sırası kararı (B4) | ≈10 dk | ADR |
| 5 | İki bacakta derleme doğrulaması | ≈8 dk | `dotnet build` |

**Çıkış kontrolü:** spec dosyaları frontmatter'lı ✓ · karar ADR'ye bağlı ✓ · build yeşil ✓.

> **UYARI:** opencode kaynağı **REFERENCE_VERIFIED** (dosya + satır) ile referanslanır; paket adı/ürün adı tahmin edilmez (G9).

---

### §2.8 P8 — Kapanış (≈40 dk)

| Alan | Değer |
|---|---|
| **Girdi** | P1..P7 çıktıları + tüm gate kanıtları |
| **Çıktı** | `reports/09-teslim-raporu.md` + `log.md` kaydı |
| **Bütçe** | ≈40 dk |
| **Gate** | **GATE-006 — Ship** |
| **Sorumlu** | inline (tek oturum) + kullanıcı onayı |

**Adımlar:**

| # | Adım | Süre | Kanıt |
|---|---|---|---|
| 1 | `dotnet build` son çalıştırma (iki bacak) | ≈8 dk | 0 hata çıktısı |
| 2 | `dotnet test` son çalıştırma | ≈8 dk | Yeşil çıktı |
| 3 | Vault link taraması | ≈5 dk | 0 bozuk link |
| 4 | Yasaklı madde taraması (EF Core 0 · v26.1 dışı 0 · string-concat 0) | ≈5 dk | grep çıktıları |
| 5 | `reports/09-teslim-raporu.md` yaz (kanıtlı) | ≈10 dk | Rapor |
| 6 | `log.md` + kullanıcı onayı | ≈4 dk | Kapanış |

**Çıkış kontrolü:** 5 DoD maddesi de ✓ → **GATE-006 kapanır**, iş teslim.

---

### §2.9 Part Bağımlılık ve Paralelleştirme Matrisi

**Bağımlılık tablosu** (satır = girdi, sütun = sağlayan part; `X` = zorunlu, `○` = faydalı):

| Girdi ↓ / Sağlayan → | P1 | P2 | P3 | P4 | P5 | P6 | P7 |
|---|---|---|---|---|---|---|---|
| Kaynak okuma notları | — | **X** | ○ | ○ | | | |
| Kapsam raporu (`reports/00`) | | — | **X** | **X** | **X** | ○ | |
| Vault iskeleti (kök MD + `.rules/`) | | | — | ○ | **X** | ○ | ○ |
| Onaylı wireframe (GATE-003) | | | | — | **X** | | |
| ADR + katman sözleşmesi | | | ○ | | **X** | **X** | **X** |
| Yeşil build (GATE-004) | | | | | — | **X** | **X** |
| opencode envanteri | ○ | | | | | | — |

**Paralelleştirme kuralları:**

| # | Paralelleşebilir | Asla paralelleşmez |
|---|---|---|
| 1 | P4 wireframe üretimi (7 ekran, bağımsız) | Gate kapatışları (sıralı) |
| 2 | P6 birim + mimari testleri | P5 → P6 (build yoksa test yok) |
| 3 | `.rules/` yazımı ↔ `ui-design/` yazımı (P3-P4) | P4 ↔ P5 (onay zinciri) |
| 4 | P7 spec okuması ↔ P6 düzeltmeleri | Kapsam değişikliği ↔ herhangi bir kod |
| 5 | Aynı partta 2 subagent (farklı rol öneki) | Aynı dosyaya 2 yazıcı ([AGENTS.md](AGENTS.md) §3) |

**Kritik yol (en uzun zincir):**

```
P1 (45) -> P2 (30) -> P3 (45) -> P4 (60 + ONAY) -> P5 (90) -> P6 (120) -> P8 (40)
                                                                 ^
                                                          GATE-005 burada
P7 (60) paralel yuruyebilir: P6 sirasinda opencode okumasi yapilabilir,
                              ama build/test GATE-005 oncesinde YENI KOD URETMEMELI
```

> **UYARI:** Paralelleştirme **kod üretimine** değil, **okumaya ve raporlamaya** uygulanır. İki subagent aynı `.cs` dosyasını yazarsa çakışma kesindir → dosya bazlı yetki ([`.agents/AGENTS.md`](.agents/AGENTS.md) §3) ihlal edilir.

---

## §3 GATE Sistemi (GATE-001..006)

### §3.1 Genel kurallar

| # | Kural | Ayrıntı |
|---|---|---|
| 1 | Gate = **giriş ve çıkış kriteri** çifti | Kriter yoksa gate yoktur |
| 2 | Kapatan = **kullanıcı** (kendi işinin gate'inde ajan raporlar) | GATE-003'te ajan **asla** kendi kendine kapatamaz |
| 3 | Gate **sıralıdır**; sonraki gate öncekinden geçemez | Atlanan gate = RED |
| 4 | Yeni kapsam isteği → ilgili gate **yeniden açılır** | Kapı geri gelir |
| 5 | Kanıtsız kapatma **G5 ihlali** | İş bitmedi sayılır |
| 6 | Her kapanış `log.md`'ye tarihli yazılır | İzlenebilirlik |

### §3.2 GATE-001 — Requirements Freeze ✓

| | |
|---|---|
| **Ne onaylar** | Kaynaklar okundu, gereksinimler dondu |
| **Giriş kriteri** | P1 başlangıcı: kaynak dosyalar erişilebilir, `ai.03.md` normalize edildi |
| **Çıkış kriteri** | 3 plan dosyası okundu · teknoloji sürümü ai.01'den **alınmadı** · çelişki listesi (`CONFLICT DETECTED`) var · `UNKNOWN`/`NEEDS_VALIDATION` işaretli |
| **Kanıt** | Okuma notu + envanter tablosu |
| **Red nedenleri** | Dosyadan biri okunmadı · CR-only hâlâ CR-only · sürüm ai.01'den kopyalanmış |
| **Kapatan** | kullanıcı |

### §3.3 GATE-002 — Scope Freeze ✓

| | |
|---|---|
| **Ne onaylar** | Kapsam sınırı çizildi, ne yapmayız belli |
| **Giriş kriteri** | GATE-001 kapalı |
| **Çıkış kriteri** | `reports/00-kapsam-analizi.md` tam · 5 karar gerekçeli · risk R1..R6 + azaltma · belirsizlik B1..B4 + karar zamanı · varsayım A1..A3 · **kapsam dışı** listesi |
| **Kanıt** | Rapor dosyası + DoD işaretleri |
| **Red nedenleri** | Kapsam dışı yok · risk azaltmasız · ekran listesi belirsiz |
| **Kapatan** | kullanıcı |

### §3.4 GATE-003 — UI Approval — **HARD STOP**

| | |
|---|---|
| **Ne onaylar** | 7 ekranın görünümü ve kontrol seti kesin |
| **Giriş kriteri** | GATE-002 kapalı · `ui-design/01..07` dosyaları var |
| **Çıkış kriteri** | 7/7 wireframe tam · saf ASCII · kontrol listesi **yalnız DevExpress v26.1** · `RibbonForm` + `RibbonControlStyle.Office2010` · **kullanıcının açık onayı** |
| **Kanıt** | Onay mesajı (`log.md`'ye tarihli) |
| **Red nedenleri** | Onaysız kodlama başladı · Unicode kutu çizim · v26.1 dışı kontrol · eksik ekran |
| **Kapatan** | **kullanıcı — ajan kapatamaz** |
| **Sonuç** | Kapalı değilse **herhangi bir `.cs` yazımı yasak** (G12) |

**Neden HARD STOP:** Wireframe değişirse kod çöp olur (R1). UI kararı **teknik değil, ürün kararıdır**; yetkisi kullanıcıdadır ([AGENTS.md](AGENTS.md) §3.2 #12).

### §3.5 GATE-004 — Build Green

| | |
|---|---|
| **Ne onaylar** | Derleme temiz — vault (004a) ve kod (004b) |
| **Giriş kriteri** | 004a: P3 bitti · 004b: GATE-003 kapalı + iskelet kuruldu |
| **Çıkış kriteri** | **004a:** 14 kök belge + 0 bozuk link + frontmatter ✓ · **004b:** `dotnet build` → **0 hata**, **iki bacak** (`net48` + `net10.0-windows`) · EF Core **0** · DevExpress yalnız **v26.1** · katman yönü NetArchTest ile 0 ihlal |
| **Kanıt** | Build çıktısı (proje adı + hata sayacı), link tarama çıktısı |
| **Red nedenleri** | Tek bacak derleniyor · hata gizlenmiş (`NoWarn`) · sürüm sapması |
| **Kapatan** | kullanıcı (`reviewer` kanıtı sunar) |

### §3.6 GATE-005 — Test Green

| | |
|---|---|
| **Ne onaylar** | Davranış kanıtlandı |
| **Giriş kriteri** | GATE-004 kapalı |
| **Çıkış kriteri** | `dotnet test` **tümü yeşil** · iki bacak · NetArchTest 0 ihlal · coverage kanıtı ya da kapsam dışı gerekçesi · `reviewer` PASS |
| **Kanıt** | Test çıktısı: geçen / başarısız / süre |
| **Red nedenleri** | Atlanan test · kanıtsız coverage · tek bacakta test |
| **Kapatan** | kullanıcı (`tester` + `reviewer` kanıtı sunar) |

### §3.7 GATE-006 — Ship

| | |
|---|---|
| **Ne onaylar** | Teslime hazır |
| **Giriş kriteri** | GATE-001..005 hepsi kapalı · P7 spec dosyaları yazıldı |
| **Çıkış kriteri** | 5 DoD maddesi kanıtla sağlandı: build 0 hata · test yeşil · link 0 bozuk · EF Core 0 / v26.1 dışı 0 / string-concat 0 · `reports/09-teslim-raporu.md` mevcut |
| **Kanıtı** | Rapor + konsol çıktıları |
| **Red nedenleri** | Kanıtsız "bitti" (G5) · yasaklı madde taraması temiz değil |
| **Kapatan** | **kullanıcı** |

### §3.8 Gate ↔ Part eşlemesi

| Part | Kapatılan gate | Kanıt tipi |
|---|---|---|
| P1 Kaynak okuma | GATE-001 | Okuma envanteri |
| P2 Kapsam analizi | GATE-002 | Rapor |
| P3 Vault kurulumu | GATE-004a (vault green) | Link tarama |
| P4 Wireframe | **GATE-003 (HARD STOP)** | Kullanıcı onayı |
| P5 İskelet + kod | GATE-004b (build green) | `dotnet build` |
| P6 Test | GATE-005 | `dotnet test` |
| P7 opencode | — (GATE-005 devamı) | Spec + build |
| P8 Kapanış | GATE-006 | Teslim raporu |

### §3.9 Sayılandırma mutabakatı (CONFLICT DETECTED → çözüm)

`reports/00-kapsam-analizi.md` §4.3'teki envanter **yedek vault'un** (`gates/GATE-000..005`) numaralandırmasıdır; bu dosya vault'un **SSOT**'udur. Eşleme:

| Yedek envanter | Bu dosya (SSOT) | Not |
|---|---|---|
| GATE-000 Discovery Approval | P1 giriş kontrolü | Gate değil, ön hazırlık |
| GATE-001 Reference + Research | **GATE-001 Requirements Freeze** | P1 |
| GATE-002 Requirements Freeze | **GATE-002 Scope Freeze** | P2 (kapsam genişletildi) |
| GATE-003 UI Approval | **GATE-003 UI Approval** ✓ | Aynı — HARD STOP |
| GATE-004 Architecture Approval | **GATE-004 Build Green** | Kanıt tipi değişti: onay → build çıktısı |
| GATE-005 Implementation Plan Approval | **GATE-005 Test Green** | Kanıt tipi değişti: plan → test çıktısı |
| — | **GATE-006 Ship** (yeni) | Teslim kanıtı |

> **UYARI:** `index.md` §3/§6 özetlerindeki `GATE-000..005` yazımı bu mutabakat tablosuna göre okunur. Gate **kimliği, kriteri ve kapanış kuralı** bu dosyadadır (SSOT). Çelişki varsa bu dosya kazanır.

---

## §4 Katman İnşa Sırası

### §4.1 Sıra (değişmez)

```
 1  Abstractions    <- hicbir seyden bagimsiz (Result, contract, event)
 2  Domain          <- yalniz Abstractions'a bagimli (invariant, aggregate)
 3  Application     <- Domain + Abstractions (use case, CQRS handler)
 4  Infrastructure  <- Abstractions; Domain'i TANIMAZ (Dapper, SQLite/SqlClient)
 5  Protocol        <- yalniz Abstractions (mesaj semasi saf)
 6  Host            <- her seyi birlestirir (composition root, DI)
 7  UI              <- yalniz Application + Host (WinForms + DevExpress)
                      Desktop ve Cli AYNI Host'u kullanir
```

### §4.2 Bağımlılık kuralı tablosu

| # | Katman | Şunlara bağımlı olabilir | Şunlara ASLA |
|---|---|---|---|
| 1 | `Abstractions` | (hiçbirine) | herhangi bir katman |
| 2 | `Domain` | `Abstractions` | `Infrastructure`, `UI`, `Protocol` |
| 3 | `Application` | `Domain`, `Abstractions` | `Infrastructure`, `UI` |
| 4 | `Infrastructure` | `Abstractions` | `Domain` (yalnız interface'ini uygular) |
| 5 | `Protocol` | `Abstractions` | `Domain`, `Application`, `Infrastructure` |
| 6 | `Host` | hepsi (composition root) | — |
| 7 | `UI` | `Application`, `Host` | `Infrastructure`, `Domain` |

### §4.3 Sıra neden değişmez

| Sıra bozulursa | Sonuç | Kanıt |
|---|---|---|
| Önce `Infrastructure` yazıldı | Domain interface'leri veritabanına göre bükülür (DIP ihlali) | NetArchTest RED |
| Önce `UI` yazıldı | İş mantığı code-behind'a sızar, tekrar test edilemez | Test coverage 0 |
| `Protocol` sonra yazıldı | Mesaj şeması host detayına kirlenir | `spec/protocol-spec.md` revizyon |
| `Host` erken yazıldı | DI kayıtları iki kez değişir | Build RED ×2 |
| İki katman aynı PR'de | Review sınırı kalkar, hata gizlenir | Devre paketi RED |

> **KURAL:** Katman sırası **[AGENTS.md](AGENTS.md) §9.1 (architect DoD)** ve [`.agents/architect.md`](.agents/architect.md) §4 ile aynıdır; çelişirse bu dosya (süreç) uygulanır, o dosya (sözleşme) tanınır.

---

## §5 Definition of Done (DoD)

### §5.1 Part DoD'u

| Part | "Bitti" anlamına gelen tek cümle |
|---|---|
| P1 | 3 kaynak okundu, çelişkiler işaretli, hiçbir teknoloji sürümü kopyalanmadı |
| P2 | `reports/00-kapsam-analizi.md` DoD'su tam, kapsam dışı çizildi |
| P3 | 14 kök belge + 0 bozuk link + 5 ajan + katman dizinleri |
| P4 | 7 wireframe + kullanıcı onayı (GATE-003) |
| P5 | `dotnet build` = 0 hata, iki bacak |
| P6 | `dotnet test` = tümü yeşil, iki bacak |
| P7 | Spec + ADR yazıldı, build hâlâ yeşil |
| P8 | `reports/09-teslim-raporu.md` + 5 kanıt maddesi |

### §5.2 Proje geneli DoD (P8'de kanıtlanır)

- [ ] `dotnet build` → **her iki bacakta 0 hata**
- [ ] `dotnet test` → **tümü yeşil**
- [ ] Vault link taraması → **0 bozuk link**
- [ ] EF Core referansı → **0** · DevExpress sürümü → **yalnız v26.1** · string-concat SQL → **0**
- [ ] `reports/09-teslim-raporu.md` → build/test çıktılarıyla kanıtlı

### §5.3 İşin "bitmediği" belirtileri (anti-DoD)

| Belirti | Karşılığı |
|---|---|
| "Muhtemelen derlenir" | G5 ihlali — DoD açık |
| "Testler geçer" ama çıktı yok | G5 ihlali |
| Bozuk link + `planlı` işaretlenmemiş | G4 ihlali |
| Tek bacakta doğrulandı | G6 ihlali |
| Gate "ben kapattım" diyen ajan | §3.1 #2 ihlali |

---

## §6 Günlük Döngü (Daily Loop)

### §6.1 Tek tur şeması (her turda tekrarlanır)

```
 1  ILERLEMEYI YENILE          (n/8) + kapanmis gate
 2  SIRADAKI ADIMI AL          WORKFLOW.md 2 -> ilgili partin adimlari
 3  BAGLAMI MINIMUM OKU        keys.md -> hedef dosya (tum vault OKUNMAZ)
 4  UYGULA                     tek sinirli eylem (1 dosya / 1 karar)
 5  KANITLA                    build / test / link tarama / olcum
 6  RAPORLA                    cevap once, <= 5 madde, ~ZAMAN
 7  SONRAKI ADIM               TEK adim, < 2 dk
```

### §6.2 Günlük tablo

| Zaman | İş | Kanıt |
|---|---|---|
| Oturum açılışı | [ROLE.md](ROLE.md) §2 + `keys.md` §2 (≈2 dk) | Okuma |
| İlk tur | `(n/8)` + gate durumu raporu | Rapor satırı |
| Her tur sonu | Tek sonraki adım (< 2 dk) | — |
| Kapı beklerken (GATE-003) | **Dur**; kod yok, "hazırlık" yok | Onay beklentisi |
| Kapanış | `log.md` tarihli kayıt + (n/8) güncel | `log.md` |

### §6.3 Tur ritmi

| # | Kural | Ölçüt |
|---|---|---|
| 1 | Her tur **tek** sınırlı eylem içerir | 1 birincil çıktı |
| 2 | Her tur **kanıtla** biter | Çıktı yoksa tur sayılmaz |
| 3 | Her tur **(n/8)** günceller | İlerleme görünür |
| 4 | Tur ≥ 3 dosya dokunuyorsa böl | [AGENTS.md](AGENTS.md) §6.1 #5 |
| 5 | Tur sonunda beklenen süre var | ≈15 dk gibi |

### §6.4 Tur Sonu Kontrol Listesi (12 madde)

Tur kapanmadan önce tek tek işaretlenir; **1'i bile açık ise tur sayılmaz**:

- [ ] `(n/8)` raporun ilk satırında mı?
- [ ] Cevap **ilk cümlede** mi (giriş paragrafı yok)?
- [ ] Madde sayısı **≤ 5** mü?
- [ ] Her sürede `≈` + birim var mı (`≈15 dk`)?
- [ ] Tek sonraki adım yazıldı ve **< 2 dk** mı?
- [ ] Kanıt var mı (build/test/tarama çıktısı ya da `UNKNOWN`)?
- [ ] Yazılan dosyanın **frontmatter'ı** var mı?
- [ ] Eklenen iç link **diskte mevcut** mi?
- [ ] Saf ASCII mi (Unicode kutu çizim yok)?
- [ ] Yasaklı madde sızıntısı yok mu (EF Core / v26.1 dışı / string-concat)?
- [ ] Yıkıcı işlem olduysa **onay alındı** mı (G7)?
- [ ] İlgili gate'in kapanış/kalış durumu güncellendi mi?

> **KURAL:** Checklist'ten çıkan **her tur** [AGENTS.md](AGENTS.md) §9.6 (ortak DoD) ile çakışır; iki liste çelişirse bu dosya (süreç) değil, anayasa (`CLAUDE.md` G1..G12) kazanır.

---

## §7 Branch / Commit Kuralı

### §7.1 Depo durumu (2026-09-25)

```
repo    : C:\versacoder   (git repo)
branch  : main
commit  : YOK  -  "your current branch 'main' does not have any commits yet"
status  : ?? .ai/    (tek untracked girdi)
```

### §7.2 İlk commit kuralı

| # | Kural | Neden |
|---|---|---|
| 1 | **İlk commit GATE-004 kapanınca** atılır (vault yeşil + build yeşil) | Yarım vault commit edilmez |
| 2 | Commit message ilk satırı **part + gate** taşır | İzlenebilirlik |
| 3 | Tek commit = tek gate kapanışı | Atomic history |
| 4 | Kapsam dışı dosya **asla** stage edilmez (`*.old.*`, CoreMusic kopyası) | G1/G7 |
| 5 | Yıkıcı git işlemi (`reset --hard`, `push --force`) → **kullanıcı onayı** | G7 |

### §7.3 Branch adlandırma

```
part/<NN>-<konu>          orn.  part/03-vault-kurulumu
gate/<NNN>-<konu>         orn.  gate/003-ui-onayi
fix/<kisa-aciklama>       orn.  fix/net48-devexp-build
```

### §7.4 Commit message biçimi

```
<part/ gate tipi>: <kisa ozet>          (max 72 karakter, kucuk harf)

<BOY>: neden (1-3 satir)               (opsiyonel)
<KANIT>: dotnet build -> 0 hata (net48 + net10.0-windows)
```

**Geçerli örnekler:**

| Message | Kanıt satırı |
|---|---|
| `vault: 14 kok belge + link taramasi temiz` | `link-scan -> 0 bozuk link` |
| `iskelet: 7 katman + CPM kuruldu` | `dotnet build -> 0 hata` |
| `test: birim + entegrasyon yesil` | `dotnet test -> 24 gecti, 0 basarisiz` |
| `gate: GATE-003 UI onayi alindi` | `onay: kullanic, 2026-09-25` |

**Geçersiz:** `güncelleme` · `wip` · `asdf` · `dosya düzeltildi` (neden yok) · `v1.0!!!`

> **UYARI:** Depoda commit yokken `git add -A` **yıkıcıdır** (istenmeyen her şey girer). Her zaman **dosya bazlı** `git add <yol>` kullan (G7).

---

## §8 Escalation Merdiveni

```
SEVIYE 0   ajan kendi yetki alani, max 3 deneme        [AGENTS.md 8.1]
   |  basarisiz (3-STRIKE)
   v
SEVIYE 1   rol ici ust yetkili
           developer -> architect | tester -> reviewer
   |  cozulmediyse
   v
SEVIYE 2   cross-rol inceleme (ilgili X sahibi ile)
   |  cozulmediyse
   v
SEVIYE 3   KULLANICI
           - 3 basarisiz deneme raporu  VEYA
           - DECISION BLOCKED          VEYA
           - G7 (yikici islem) / G8 (teknoloji degisikligi)
   |
   v
SEVIYE 4   ISI DONDUR  ->  9 (bloke protokolu)  ->  gate'e don
```

| Tetikleyici | Etiket | Beklenen yanıt |
|---|---|---|
| Aynı hata 3 düzeltmede çözülmedi | `3-STRIKE` | ≈10 dk |
| Kaynak çelişti, hiyerarşi çözmedi | `DECISION BLOCKED` | ≈15 dk |
| Yeni paket/ORM/sürüm isteği | `G8` | Kullanıcı onayı |
| Silme / overwrite / `git reset` | `G7` | Onay |
| Gate onayı gereken adım | `GATE-OPEN` | Kapalıysa bekle |
| Bilgi yok | `UNKNOWN` | Uydurma yasak |

> **KURAL:** Escalation **raporlanır, gizlenmez**. Kullanıcıya sunulan escalation'ın 3 maddesi vardır: hata · şüpheli varsayım · seçenekler. Seçeneksiz escalation yarım rapordur.

---

## §9 Blokeli Durum Protokolü (Blocked-State)

### §9.1 Ne sayılır bloke

| # | Bloke tipi | Örnek |
|---|---|---|
| 1 | Kapı açık, onay bekleniyor | GATE-003 kapalı, kod yazılamıyor |
| 2 | Belirsizlik karar bekliyor | B1 `.slnx` vs `.sln`, B4 LLM sırası |
| 3 | Teknik engel | net48 + DevExpress v26.1 build RED (R5) |
| 4 | 3-Strike | Aynı hata 3 kez çözülmedi |
| 5 | Kaynak çelişimi | `CONFLICT DETECTED` çözülmedi |

### §9.2 Bloke protokolü (5 adım)

| # | Adım | Çıktı | Süre |
|---|---|---|---|
| 1 | **DUR** — kod yazmayı bırak | — | anında |
| 2 | Bloke tipini sınıflandır (§9.1) | Etiket | ≈1 dk |
| 3 | Kanıtı topla (çıktı alıntısı, dosya:satır) | Kanıt bloğu | ≈3 dk |
| 4 | Şüpheli varsayımı **adlandır** | Tek cümle | ≈2 dk |
| 5 | Seçeneklerle raporla (A/B/C) + **bekle** | Rapor | ≈4 dk |

**Bloke raporu şablonu:**

```markdown
(n/8) BLOKE - <gate/part>
- ETIKET    : <3-STRIKE | DECISION BLOCKED | GATE-OPEN | G7 | G8 | UNKNOWN>
- KONUM     : <dosya yolu veya part>
- NEDEN     : <1-3 satir, kanitli>
- SUPHLI VARSAYIM : <tek cumlede adlandirilmis varsayim>
- DENEMELER : 1) ... 2) ... 3) ...
- SECENEKLER: A) <...>  B) <...>  C) <...>
- SORU      : <tek kisa soru>
- TAHMIN    : secenek secilince ~ZAMAN
```

### §9.3 Blokede yapılmayanlar

| # | Yasak | Alternatif |
|---|---|---|
| 1 | Farklı çözümü denemeye körlemesine devam | §8 merdiveni |
| 2 | Yamayla kapatmak | Kök neden + ADR |
| 3 | Kanıtsız "çözüldü" | Build/test çıktısı |
| 4 | Kapsamı sessizce küçültmek | Kullanıcı onayı (GATE-002 yeniden) |
| 5 | Blokeyi raporlamamak | §9.2 adım 5 zorunlu |

### §9.4 Bloke → serbest akış

```
RAPOR gonderildi
      |
      +-- kullanici secenek secti  ->  ilgili gate'e DON (kapanisa kadar)
      +-- kullanici kapsam degistirdi -> GATE-002 yeniden AC -> P2'ye DON
      +-- teknik engel dogrulandi  ->  ADR ac (architect) -> P5'e DON
      +-- yanlis varsayim cikti   ->  UNKNOWN isaretle, P1'e KISA donus
```

---

## §10 Rapor Formatı

### §10.1 Zorunlu 5 kural

| # | Kural | Doğru | Yanlış |
|---|---|---|---|
| 1 | **`(n/8)`** her turun başında | `(3/8) P3 vault kuruluyor` | "vault'a devam" |
| 2 | **Cevap önce** (answer-first) | "Link taraması temiz: 0 bozuk." | Uzun giriş paragrafı |
| 3 | **En fazla 5 madde** | 5 madde + tablo | 12 maddelik liste |
| 4 | **Somut süre** (`≈15 dk`) | "≈25 dk sürer" | "birazdan", "kısa sürede" |
| 5 | **Tek sonraki adım** < 2 dk | "Sonraki: `ui-design/01` yarat (≈90 sn)." | 3 adımlık liste |

### §10.2 Rapor iskeleti

```markdown
(n/8) <CEVAP - tek cumle>

- <madde 1>
- <madde 2>
- <madde 3>

Sonraki adim: <TEK somut adim, < 2 dk>
```

### §10.3 Kapı raporu (gate kapanışında)

```markdown
(n/8) GATE-00X <AD> -> KAPANDI
- KANIT   : <konsol ciktisi alintisi, sayilarla>
- DOSYA   : <uretilen dosya yollari>
- EKSIK   : <YOK ya da NEEDS_VALIDATION maddeleri>
- SONRAKI : <gate sonrasi ilk adim, ~ZAMAN>
```

### §10.4 Örnek (geçerli)

```markdown
(3/8) GATE-004a VAULT GREEN -> KAPANDI
- KANIT   : link-scan -> 0 bozuk link; 14/14 kok belge frontmatter'li
- DOSYA   : .ai/AGENTS.md (37.617 B), .ai/WORKFLOW.md, .ai/ROLE.md
- EKSIK   : .rules/ ve spec/ dizinleri planli (P3 adim 4)
- SONRAKI : ui-design/01-start-page.md yaz (~7 dk)

Sonraki adim: ui-design/01-start-page.md olustur (≈90 sn).
```

### §10.5 Hatalı raporlar

| Hatalı | Sorun | Doğrusu |
|---|---|---|
| "Vault kurulumuna devam ediyorum." | `(n/8)` yok, cevap yok | `(3/8) …` + tek cümle cevap |
| 9 maddelik liste | Sınır aşıldı | Tabloya böl ya da 2 tur |
| "Biraz süre alacak" | Somut değil | `≈15 dk` |
| "Sonraki adımlar: 1) … 2) … 3)" | 1'den fazla adım | Tek adım |
| "Tamamlandı gibi görünüyor" | Kanıtsız (G5) | Konsol çıktısı alıntısı |

### §10.6 Canlı Rapor Örnekleri (senaryo bazlı)

**Senaryo A — normal ilerleme:**

```markdown
(4/8) Cevap: 7 ekran wireframe'i tamam, GATE-003 onayinizi bekliyor.

- ui-design/01..07 yazildi (hepsi saf ASCII, frontmatter'li)
- flows.md + component-inventory.md tamam (DevExpress v26.1, 23 kontrol)
- mimari diyagramlar architecture/ altinda (bagimlilik yonu dogrulandi)
- eksik: 05-ai-chat icin ProgressBarControl mu BarEditItem mi -> INFERRED

Sonraki adim: GATE-003 onayini verebilir misiniz? (A) onayla (B) ekran degisikligi iste  ~2 dk
```

**Senaryo B — RED devresi:**

```markdown
(5/8) Cevap: reviewer RED verdi - SessionRepository'de 1 CRITICAL + 1 MEDIUM bulgu.

- [CRITICAL] src/VersaCoder.Data/SessionRepository.cs:64 - string-concat SQL (G2)
- [MEDIUM]   ayni dosya:81 - CommandTimeout set edilmemis
- kanit: build 0 hata, test 24/24 yesil - bulgu STATIK incelemeden
- devre: reviewer -> developer (4 madde gonderildi, ~10 dk SLA)

Sonraki adim: SessionRepository.cs:64-72 arasini parametreli sorguya cevir (~6 dk).
```

**Senaryo C — 3-Strike escalation:**

```markdown
(5/8) DUR - 3 basarisiz deneme, kod yazmayi biraktim.

- ETIKET: 3-STRIKE  |  KONUM: tests/VersaCoder.Data.Tests (net48 bacak)
- NEDEN : net48'de xunit analyzers "async void" hatasi veriyor (3 deneme)
- SUPHLI VARSAYIM: net48'de test projesi `EnableMSTestRunner` gerektiriyor
  (NEEDS_VALIDATION - dogrulanmadi)
- SECENEKLER: A) testler yalniz net10 bacakta  B) analyzer'i NoWarn ile kapat
  C) test framework'unu gorus (B2 belirsizligi)
- SORU: hangi secenek?  |  TAHMIN: secimden sonra ~20 dk
```

---

## §11 Hata Modları

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Part atlandı | Sonraki part girdisi eksik | Geri dön, §1.2 #1 |
| 2 | Gate sahte kapandı | Kanıt yok, iş sonra patlar | G5 — gate'i yeniden aç |
| 3 | GATE-003'ten önce kod | Wireframe değişince kod çöp | Sil + P4'e dön (G12) |
| 4 | Katman sırası ters | NetArchTest RED | §4.1 sırasına dön |
| 5 | Tek bacak doğrulama | net48 kırık, sonradan farkedilir | Her gate'te iki bacak |
| 6 | Bütçe aşımı sessiz | Part 2× sürüyor | 20 dk aşımda dur + escalation |
| 7 | Rapor formatı ihlali | Uzun, maddesiz, süresiz | §10 |
| 8 | Commit erken | Yarım vault | §7.2 #1 |
| 9 | Bloke gizlendi | Kullanıcı sonradan öğrenir | §9.2 |
| 10 | Çelişki sessiz çözüldü | İki sürüm birden | G10 → `CONFLICT DETECTED` |
| 11 | Kapsam kayması | P5'te yeni ekran | GATE-002 yeniden aç |
| 12 | Kanıtsız "bitti" | Rapor sayı yok | G5 |

---

## §12 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] P1..P8'in **her birinde** Girdi / Çıktı / Bütçe / Gate / Adım / Çıkış kontrolü var
- [ ] GATE-001..006 **her birinde** giriş + çıkış kriteri + kanıt + kapatan var
- [ ] GATE-003 `HARD STOP` olarak açıkça işaretli
- [ ] Katman sırası `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`
- [ ] §5 DoD — proje geneli 5 madde `reports/00` §5 ile aynı
- [ ] §7 depo durumu (commit yok) doğru yansıtılmış
- [ ] §8 escalation merdiveni + §9 bloke protokolü ASCII (saf ASCII)
- [ ] §10 rapor formatı: `(n/8)` · cevap önce · ≤5 madde · `≈` süre · tek <2 dk adım
- [ ] Tüm markdown linkleri diskte mevcut
- [ ] Unicode kutu çizim (`─│┌└▶`) **hiç yok** (yalnız ASCII)

---

## §13 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kapsam analizi (part 2 çıktısı) | `.ai/reports/00-kapsam-analizi.md` |
| 2 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |
| 3 | Ajan protokolü + DoD | `.ai/AGENTS.md` |
| 4 | Ajan kaydı (yetki matrisi) | `.ai/.agents/AGENTS.md` |
| 5 | Mutlak rol + ses kuralları | `.ai/ROLE.md` |
| 6 | Anahtar kelime haritası | `.ai/keys.md` |
| 7 | Navigasyon | `.ai/index.md` |
| 8 | Kök belge şablonu | `.ai/.templates/documentation/root-md-template.md` |
| 9 | Bootstrap kaynağı | `G:\…\versacoder proje planaması ai.03.md` |
