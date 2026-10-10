---
title: "engine - Orkestrasyon Motoru (cok-ajan is akisi)"
type: knowledge
category: ".ai"
status: active
version: 2.0.0
updated: 2026-10-07
authority: primary
scope: "Giris (prompt) -> hazirlik (vault okuma) -> dagitim (AGENTS matrisi) -> kontrol (checkpoint) -> teslim (kutu) zinciri; paralellik limiti, devir paketi, 3-STRIKE, butce, hata modlari ve end-to-end agentic model"
tags: [identity]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# engine — Orkestrasyon Motoru

**Zorunlu Bağlantılar:** [[index|index]] · [[definitions/agents|AGENTS]] · [[governance/workflow|WORKFLOW]] · [[definitions/role|ROLE]] · [[claude|CLAUDE]] · [[definitions/ultra-thinking|ULTRA-THINKING]] · [[memory/memory|MEMORY]] · [[ops/log|log]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[checklists/layer-boundary-check|layer-boundary-check]]

> **Tek cümlelik sorumluluk:** *Bir istek prompt'un geldiği andan teslim kutusuna kadar çok-ajan iş akışının hangi sırayla, hangi limitlerle ve hangi durdurucularla yürüdüğü* sorusunun tek cevabıdır.
> Bu dosya **orkestrasyonun (yürütmenin) SSOT**'udur: ajanın kendisi [[definitions/agents|AGENTS.md]]'tedir, gate'in tanımı [[governance/workflow|WORKFLOW.md]]'tedir, rolün sesi ve karar hiyerarşisi [[definitions/role|ROLE.md]]'dedir — burada yalnız *motorun nasıl aktığı* yazılır (SRP, tekrar yok).

---

## §0 Özet (Answer First)

Motor **tek yönü olan 5 bileşenden** oluşur: **Giriş (prompt) -> Hazırlık (vault okuma) -> Dağıtım (AGENTS matrisi) -> Kontrol (checkpoint) -> Teslim (kutu)**. Geri dönüş yalnız **kanıtlı RED** ile olur; düz çizgi halinde ilerler, atlanan halka geri gelir. Aynı anda **en fazla 3 ajan** çalışır (2026-09-29 rate limit olayı, §6.1), paylaşılan dosyada **tek yazıcı** vardır, her ajan işi **5 alanlı kutuyla** teslim eder ve kutusuz çıktı **kaba metindir, işlenmez**. Gate'i ajan **hazırlar**, kapatan **kullanıcı**dır.

| # | Bileşen | Tek satır iş | Çıktı | Bu bileşen yapmaz |
|---|---|---|---|---|
| 1 | **Giriş** | Prompt + gate durumu + tetik etiketi ayrımı | Sınıflandırılmış istek | Karar vermek |
| 2 | **Hazırlık (kayıt)** | Gerekli vault dosyalarını okuma, çelişki taraması | Okuma notu + kısıt listesi | Üretim yapmak |
| 3 | **Dağıtım** | İş tipini AGENTS §1 matrisine göre ajana atamak | Görev + hedef ajan | Ajanın işini yapmak |
| 4 | **Kontrol** | Checkpoint: 4 satır kanıt kutusu + gate sırası | GREEN ya da RED döngüsü | Gate'i kapatmak |
| 5 | **Teslim** | 5 alanlı ajan kutusu + bütçe + log kaydı | Kutu + kanıt eki | Kanıtsız "bitti" demek |

**Motorun üç demir kuralı (hepsi ihlalde RED):**

| Kural | Kaynak | İhlalde |
|---|---|---|
| Aynı anda **≤ 3 ajan**; aynı dosyada **tek yazıcı** | [[definitions/agents|AGENTS.md]] §5 · [[governance/workflow|WORKFLOW.md]] §3.2 | İş durur, çakışan yazım geri alınır |
| **Kanıtsız çıktı işlenmez** (kutu + komut çıktısı zorunlu) | [[claude|CLAUDE.md]] G5 · [[definitions/agents|AGENTS.md]] §2 | Aynı kutuyla tekrar istenir |
| **3. strike'tan sonra deneme yok**; `dur/bekle` + bloke raporu | [[definitions/agents|AGENTS.md]] §4 · [[definitions/role|ROLE.md]] §5 | Sayaç asla sıfırlanmaz, tahmin israftır |

> **UYARI:** Bu dosyadaki hiçbir madde tek başına "bitti" kanıtı değildir. Kanıt = çalıştırılmış komutun çıktısı (`dotnet build`, `dotnet test`, link taraması) ve [[claude|CLAUDE.md]] §3.2'deki kanıt satırıdır.

---

## §1 Motor Bileşenleri

### §1.1 Genel akış (saf ASCII)

```text
 KULLANICI PROMPT
       |
       v
 +-----------+   okuma notu   +-----------+   gorev      +-------------+
 |  1 GIRIS  | --------------> | 2 KAYIT   | -----------> | 3 DAGITIM   |
 | prompt +  |   (vault oku)   | kisit +   |  (AGENTS    | matristen   |
 | gate dur. |                 | celişki   |   matrisi)  | ajana ata   |
 +-----------+                 +-----------+              +------+------+
       ^                                                    |         |
       |                                        en fazla 3 ajan       |
       |                                                    v         v
       |                                            +---------+  +--------+
       |                                            | PARALEL |  |  SERI  |
       |                                            | <=3 ajan|  | 1 ajan |
       |                                            +----+----+  +---+----+
       |                                                 |           |
       |                                                 v           |
       |                                     +---------------------+  |
       |                                     | 4 KONTROL           |<-+
       |                                     | checkpoint          |
       |                                     | 4 satir kanit kutusu|
       |                                     +----------+----------+
       |                                          GREEN |    | RED
       |                                                |    +--> (2) KAYIT'a don
       |                                                v
       |                                     +---------------------+
       +-----------  kullanici ONAYI ------- | 5 TESLIM            |
                                             | 5 alanli ajan kutusu|
                                             +----------+----------+
                                                        v
                                             log.md + MEMORY.md kaydi
```

> **KURAL:** Akış **tek yönlüdür**; geri dönüş yalnız **kanıtlı RED raporu** ile olur ([[governance/workflow|WORKFLOW.md]] §1.2 madde 4). Sessiz geri sarım yasaktır.

### §1.1.1 Prompt işleme hattı (girdi hattı)

§1.1'deki motor akışı bu hattı "1 GİRİŞ" kutusunda karşılar. İkisi çatışmaz:
biri **girdi hattı** (prompt nereden geçer), diğeri **üretim hattı** (kod nasıl
üretilir).

```text
 KULLANICI PROMPTU
        |
        v
 KNOWLEDGE MODEL        model elle atanmaz (AGENTS S6)
        |
        v
 ASKING QUESTIONS       MOD A - proje ozel, jenerik 8 yasak
        |
        v
 .AI SECOND BRAIN       vault - navigasyon .ai/index.md S3
        |
        v
 ASKING QUESTIONS       devam sorulari - tavan 10 / "yeter" ile biter
        |
        v
 +-------------------------+
 |  AGENT SYSTEM           |
 |  INSTRUCTIONS           |  -> .ai/AGENTS.md S1 matrisi
 +------------+------------+
              |
      +-------+-------+
      |       |       |
      v       v       v
  DISCOVERY ANALYSIS VALIDATION
      |       |       |
      v       v       v
  FILE     CODE      EVIDENCE
  INDEX    INDEX
      |       |       |
      +---+---+-------+
          |
          v
  REQUIREMENT ENGINE
          |
          v
  ASKING QUESTIONS     karar sorulari
          |
          v
  ARCHITECTURE         architect + layer-boundary-check 13 soru
          |
          v
  ASKING QUESTIONS     son belirsizlikler
          |
          v
  APPROVAL             kullanici onayi; GATE-OPEN / G7 ONAYI burada biter
          |
          v
  IMPLEMENTATION       WORKFLOW S1: 004a -> 003 -> 004 -> 005 -> 006
```

> **KURAL:** 4 `ASKING QUESTIONS` durağı da **MOD A**'dır — tek soru, A/B/C
> seçenekli, cevap beklemeden diğerine geçilmez.
> `.opencode/skills/prompt-maker/SKILL.md` §1.

### §1.1.2 Hedef mimari (TARGET SYSTEM)

Nihai hedef; VersaCoder kendi kütlesini bu sırayla besler.

```text
                          OPENCODE
                              |
                         ORCHESTRATOR
                              |
              +---------------+---------------+
              |               |               |
          RESEARCH       REQUIREMENTS      CONTEXT
              |               |               |
              +---------------+---------------+
                              |
                       ARCHITECTURE & .AI
                              |
                           PLANNER
                              |
                      TASK DECOMPOSER
                              |
                    SPECIALIZED AGENTS
                              |
         +--------------------+--------------------+
         |                    |                    |
       .NET                UI/UX              SECURITY
   l0..l7 (ADR-028)    01..07 ekran            OWASP
         |                    |                    |
         +--------------------+--------------------+
                              |
                           SKILLS
                              |
                             MCP
                              |
                           TOOLS
                              |
                       IMPLEMENTATION
                              |
                         CODE TRACKER
                              |
                    BUILD / TEST / REVIEW
                              |
                         VERIFICATION
                              |
                           .AI BRAIN
```

> **Düzeltme (G10):** özgün şablonda `SPECIALIZED AGENTS` 5'e bölünüyordu
> (.NET / Backend / Frontend / UI-UX / Security). [[definitions/agents|AGENTS.md]] §5
> **en fazla 3 paralel** der; `Backend` + `Frontend` bu projede **kapsam dışı**
> (G8: WinForms masaüstü; G6: `net48 + net10.0-windows`). 3'e indirildi.

### §1.1.3 Çok-ajan sürüsü (MULTI-AGENT SWARM)

Bağımsız işler için paralel; bağımlı işler **seri** zincirdedir (§2.1).

```text
                          ORCHESTRATOR
                                 |
        +------------------------+----------------------------+
        |      PARALEL BLOK - en fazla 3 ajan (AGENTS S5)     |
        |      farkli dosya -> ezme yok (AGENTS S7)           |
        +-----------+---------------------+-------------------+
                    |                     |                   |
                    v                     v                   v
                ARCHITECT            UI-DESIGNER           GENERAL
              13 soru cevabi         01..07 ekran        matris disi
                    |                     |                   |
                    +----------+----------+-------------------+
                               |  mimari KAPANDI / ekran onayi
                               v
                          DEVELOPER
                     diff + dotnet build + dotnet test
                               |
                               v
                           REVIEWER
                     suc dosya + satir + RISK satiri
                               |
                 +-------------+-------------+
                 |                           |
                 v                           v
               TESTER                 SECURITY-AUDITOR
          dotnet test + kapsam        OWASP + severity
                 |                           |
                 +-------------+-------------+
                               |
                               v
                             GATE  -> [[governance/workflow|WORKFLOW.md]] §1
```

> **Düzeltme (3 madde):** (1) `RESEARCHER` ajanı **yok** → `general`
> ([[definitions/agents|AGENTS.md]] §1, matris dışı); (2) `VERIFY` ajanı **yok** →
> **`GATE`** (WORKFLOW §1 — kapı işi ajan işi değildir); (3) zincir
> `architect -> developer -> reviewer -> tester` atlanamaz (AGENTS §1
> "Zincir kuralı"); `REVIEWER` ile `TESTER` paralel koşabilir, ikisi de
> `DEVELOPER`'ın diff'i olmadan çalışmaz.

### §1.1.4 End-to-end agentic model (prompt kaynağı: ai.05 §47)

Nihai çalışma modeli — VersaCoder'ın Chat'ten kullanıcıya dönüşüne kadar
tek yönlü akışı. Bu şema §1.1'in motor akışını **harici ajan boyutuyla**
tamamlar (Internal/External ayrımı [[definitions/agents|AGENTS.md]] §4).

```text
┌─────────────────────────────┐
│           USER              │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│            CHAT             │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│       INTENT ENGINE         │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│ SESSION AGENTIC AGENT WIZARD│
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│       ORCHESTRATOR          │
└──────────────┬──────────────┘
               │
       EXECUTION STRATEGY
               │
       ┌───────┴────────┐
       ▼                ▼
 INTERNAL AGENT    EXTERNAL AGENT
       │                │
       │          ┌─────┼──────────────┐
       │          ▼     ▼              ▼
       │       OpenCode Codex     Claude Code
       │                         / Abacus CLI
       │
       └──────────┬───────────────┘
                  ▼
            EXECUTION
                  ▼
            RESULT INGESTION
                  ▼
              VALIDATION
                  ▼
             REVIEW / AUDIT
                  ▼
                CHAT
                  ▼
                USER
```

**Öncelik sırası (ai.05 §51):**

```text
USER INTENT > EXPLICIT REQUIREMENTS > PROJECT CONTEXT > POLICY
   > CORRECTNESS > SECURITY > VALIDATION > MAINTAINABILITY
   > PERFORMANCE > SIMPLICITY
```

> **Çekirdek kural (ai.05 §48):** VersaCoder'ın amacı **AI'a kod yazdırmak
> değildir**; AI Agent'lar ile **kontrollü yazılım mühendisliği** yapmaktır.
> External CLI entegrasyonları VersaCoder'ı başka bir coding-agent'a
> dönüştürmek değil, gerektiğinde güçlü harici runtime'ları VersaCoder'ın
> kontrol ettiği Session/Workflow/Policy/Validation sistemi içinde
> **execution provider** olarak kullanabilmektir.

### §1.2 Bileşen 1 — Giriş (prompt)

Giriş, motorun **sınıflandırma** kapısıdır: gelen istek önce *ne olduğu* (iş tipi), *hangi kapıda durduğu* (gate durumu) ve *hangi etiketle* (tetik) işleneceğine bakılır. Sınıflandırma yapılmadan üretime geçen ajan, ya yanlış ajana gider ya da kapalı bir gate'i yok sayar; ikisi de geç keşfedilir. Bu bileşen **karar vermez**, yalnız etiketler ve sıradaki bileşene devreder.

| # | Girdi kontrolü | Soru | Çıkarsa yapılacak |
|---|---|---|---|
| 1 | İş tipi | İstek AGENTS §1 matrisinde mi? | Matrisdeyse 3'e, değilse `general` |
| 2 | Gate durumu | GATE-003 kapalı mı ve istek `.cs` içeriyor mu? | `GATE-OPEN` → kod yazılmaz, onay istemi gönderilir |
| 3 | Tetik etiketi | 3 hata / çelişki / yıkıcı işlem var mı? | `3-STRIKE` · `CONFLICT DETECTED` · `G7 ONAYI` → §4 |
| 4 | Kapsam | İstek yeni dosya/paket/sürüm içeriyor mu? | Yeni paket/sürüm/ORM = `G8` + kullanıcı onayı |
| 5 | Kanıt ihtiyacı | "bitti/tamam" iddiası var mı? | Çıktı yoksa madde yazılmaz (G5) |
| 6 | Bütçe | Aynı oturumda kaç dosyaya dokunuldu? | ≤ 3 dosya sınırı → yeni oturuma böl (§5) |
| 7 | Kaynak | Birden fazla kaynak mı konuşuyor? | Çelişki varsa `CONFLICT DETECTED` → [[definitions/ultra-thinking|ULTRA-THINKING.md]] |

**Giriş etiketleri (tek kelime, tek anlam):**

| Etiket | Ne anlama gelir | Sıradaki bileşen |
|---|---|---|
| `WORK` | Normal iş, gate açık, kanıt üretilebilir | 2 Hazırlık |
| `GATE-OPEN` | İstenen iş kapalı bir gate'in arkasında | Dur + onay istemi |
| `DECISION BLOCKED` | Çelişki çözülmedi | 2 Hazırlık → ULTRA-THINKING |
| `G7 ONAYI` | Yıkıcı / geri alınamaz işlem | Dur + tek soruluk onay |
| `UNKNOWN` | Bilgi yok, tahmin yasak | Dur + kullanıcıya sor |

### §1.3 Bileşen 2 — Hazırlık (kayıt / vault okuma)

Hazırlık, üretimin **girdi**sidir: motor kodu ya da raporu yazmadan önce ilgili vault dosyalarını okur, kısıt listesini çıkarır ve kaynak çelişkisini arar. Okuma notu alınmayan iş, kaynaksız iş demektir ve `SOURCE_VERIFIED` etiketi üretilemez. Bu bileşen **sadece okur ve listeler**; seçenek üretmek ve seçmek ULTRA-THINKING protokolüne aittir.

| # | Okunacak hedef | Ne verir | Okunmazsa |
|---|---|---|---|
| 1 | [[index|index.md]] | Vault navigasyonu, güncel dosya listesi | Var olmayan hedefe link yazılır (bozuk link) |
| 2 | [[claude|CLAUDE.md]] | G1..G12 guardrail, kanıt formatı, dur/bekle | Yasak iş (EF Core, string-concat SQL) fark edilmez |
| 3 | [[governance/workflow|WORKFLOW.md]] | Gate zinciri, kabul kriteri, kanıt kutusu | Sıra atlanır, kanıt eksik gate kapanır |
| 4 | [[definitions/agents|AGENTS.md]] | Dağıtım matrisi, kutu, 3-STRIKE, eşzamanlılık | Yanlış ajana iş gider (§2, §4) |
| 5 | [[definitions/role|ROLE.md]] | Karar hiyerarşisi, tek soru kuralı, devir zinciri | Karar çelişkesi çözümsüz kalır |
| 6 | [[governance/keys|keys.md]] · [[definitions/glossary|glossary.md]] | Anahtar kelime haritası, TR/EN terim | Aynı konu iki adla iki yerde açılır |
| 7 | [[.decisions/brain|brain.md]] · [[.decisions/adr-index|.decisions/adr-index.md]] | Karar özeti, ADR listesi | Karar tekrar tartışılır (kayıt dışı iş) |
| 8 | İlgili ADR (örn. [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]]) | Bağlayıcı karar + şartlar | Katman/proje sayısı yeniden "tahmin" edilir |
| 9 | [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] | Katman sınırı 13 sorusu | Mimari ihlal gate dışında kalır |
| 10 | [[memory/memory|MEMORY.md]] · [[ops/log|log.md]] | Oturumlar arası durum, tarihli kapanış | Aynı hata ikinci kez denenir |

**Hazırlık çıktı kalitesi:**

| Çıktı | İçerik | Zorunluluk |
|---|---|---|
| Okuma notu | Hangi dosya, hangi satır, hangi tarih | Her işte |
| Kısıt listesi | Geçerli kural (G maddesi, ADR, gate) | Karar işlerinde |
| Çelişki raporu | `CONFLICT DETECTED` + hangi basamak kazanır | Çelişki varsa |
| Etiket | `SOURCE_VERIFIED` / `INFERRED` / `NEEDS_VALIDATION` | Teknik her iddiada |

### §1.4 Bileşen 3 — Dağıtım (AGENTS matrisi)

Dağıtım, işi **tek doğru ajana** vermektir; matris dışındaki iş `general`'a düşer. Yanlış ajan = yanlış kanıt = gate RED ve iş iki kez yapılır; bu, 3-STRIKE'dan daha pahalıdır çünkü dağıtım hatası gate'e kadar gizlenir. Dağıtım ayrıca **sırayı** da kurar: `architect -> developer -> reviewer -> tester` halkası [[governance/workflow|WORKFLOW.md]] §4'teki A→B→C akışıyla aynıdır.

| İş tipi | Ajan | Çıktı formatı | Kanıt |
|---|---|---|---|
| Mimari / katman kararı | `architect` | Karar + 5 alanlı kutu | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] + [[checklists/layer-boundary-check|layer-boundary-check]] 13 soru |
| Kod yazımı / refactor | `developer` | Diff + 5 alanlı kutu | Build + test kırmızı→yeşil |
| İnceleme / review | `reviewer` | Kutu, **RİSK satırı dolu** | Suçlu dosya + satır no |
| Test / kapsam | `tester` | Test dosyası + kutu | `dotnet test` çıktısı |
| Güvenlik | `security-auditor` | Bulgu listesi + kutu | OWASP kalemi + severity |
| UI / ekran | `ui-designer` | [[definitions/glossary|glossary.md]] terimli tasarım | 7 ekrandan birinin adı (`01-start-page` .. `07-settings`) |
| Tanım dışı | `general` | Serbest + kutu | Kendi sözü değil, kanıta dayalı |

**Dağıtım karar ağacı (tek satır kural):**

```text
 istek matriste var mi?  -- EVET --> atanmis ajana ver
        |
       HAYIR
        |
        v
    general (varsayilan)  --  ama matristeki UZMAN isi yapamaz
        |
        +--> mimari etki var mi?  -- EVET --> once architect, sonra developer
```

> **UYARI:** `general` her işi yapabilir ama matristeki uzman işi yapamaz. Uzmanlık dışı ajan = yanlış kanıt (AGENTS §1, "Yanlış ajan maliyeti").

### §1.5 Bileşen 4 — Kontrol (checkpoint)

Checkpoint, **kanıtın gate kriteriyle birebir karşılaştırıldığı** duraktır; burada "neredeyse geçti" diye bir durum yoktur — ya kriterlerin tamamı kanıtlıdır (GREEN) ya da iş RED ile bir önceki bileşene döner. Ajan checkpoint'i **hazırlar**, gate'i **kapatmaz**; kapatan her zaman kullanıcıdır. Kanıtsız kapatma G5 ihlali sayılır ve gate yeniden açılır.

| Gate | Ne kapatır | Üreten | Kapatan | Süre |
|---|---|---|---|---|
| GATE-004a | Vault yeşil: 500 MD · 0 bozuk link · 0 mojibake | `architect` | kullanıcı | ≈45 dk |
| **GATE-003** | 7 wireframe + **kullanıcı onayı** (HARD STOP) | `ui-designer` | **yalnız kullanıcı** | ≈60 dk + onay |
| GATE-004 | İskelet + 2 bacak build + ilk commit | `architect` + `developer` | kullanıcı | ≈90 dk |
| GATE-005 | Test + NetArchTest 0 ihlal | `tester` | kullanıcı | ≈120 dk |
| GATE-006 | Teslim (4 kanıt satırı) | inline oturum | kullanıcı | ≈40 dk |

**Checkpoint'in 4 sorusu (hepsi "evet" değilse RED):**

| # | Soru | Hayır ise |
|---|---|---|
| 1 | Gate'in **kabul kriteri** maddenin içinde birebir sayılıyor mu? | Kanıt kutusu eksik → RED |
| 2 | Çıktı **çalıştırılmış komutun** ekli mi (build/test/tarama)? | G5 ihlali → madde silinir |
| 3 | İlgili **bacaklar** da mı yeşil (`net48` + `net10.0-windows`)? | Tek bacak kanıt değildir (G6) |
| 4 | Bağlantılı dosyalar (**frontmatter**, izinli link) temiz mi? | Bozuk link / eksik frontmatter → vault RED |

**Kanıt kutusu — gate kapanışı (4 satır, [[governance/workflow|WORKFLOW.md]] §2):**

```text
KANIT   : <komut> -> <sayilar>      (CIKTI EKI zorunlu)
DOSYA   : <yazilan dosya yollari>
EKSIK   : YOK   (varsa: DOĞRULAMA GEREKLI maddesi)
SONRAKI : <adim, ≈ sure>
```

> **UYARI:** Bu 4 satır, ajanın 5 alanlı çıktı kutusuyla ([[definitions/agents|AGENTS.md]] §2) **aynı şey değildir**; biri diğerinin yerine kullanılamaz (SSOT). Hata raporu biçimi ise [[claude|CLAUDE.md]] §3.3'tedir — üçü ayrı kutulardır.

### §1.6 Bileşen 5 — Teslim (kutu)

Teslim, işin **5 alanlı kutuyla** geri verilmesidir: `BASLIK / KAPSAM / KARAR / RİSK / KANIT`. Kutu yoksa ya da `KANIT` satırı boşsa çıktı **kaba metindir** ve aynı kutuyla tekrar istenir; iş tamamlanmış sayılmaz. Kutudan sonra iki kayıt açılır: [[ops/log|log.md]]'e tarihli kapanış, [[memory/memory|MEMORY.md]]'e oturumlar arası durum.

```text
+--------------------------------------------------+
| BASLIK : 2-4 kelime, ne yapıldı                  |
| KAPSAM : dokunulan dosyalar (yol) veya "yok"     |
| KARAR  : seçim + tek satır gerekçe               |
| RİSK   : en fazla 1 madde; yoksa "yok"           |
| KANIT  : komut çıktısı / satır no / test sonucu  |
+--------------------------------------------------+
```

| # | Teslim öncesi kontrol | Beklenen | Kaynak |
|---|---|---|---|
| 1 | Ajan kutusu dolu | 5/5 alan, `KANIT` boş değil | [[definitions/agents|AGENTS.md]] §2 |
| 2 | Gate kanıt kutusu dolu | 4/4 satır | [[governance/workflow|WORKFLOW.md]] §2 |
| 3 | Bütçe aşılmadı | ≤ 8 KB çıktı, ≤ 3 dosya/oturum | §5 bu dosya |
| 4 | Yasaklı tarama temiz | EF Core 0 · v26.1 dışı 0 · string-concat 0 | [[claude|CLAUDE.md]] §2 |
| 5 | Kayıt yazıldı | `log.md` kapanış satırı + [[memory/memory|MEMORY.md]] durumu | GATE-006 |

---

## §2 Paralel Çalışma Kuralları

### §2.1 Sınır: en fazla 3 ajan

| # | Kural | Limit | Kaynak | İhlalde |
|---|---|---|---|---|
| 1 | Aynı anda çalışan ajan | **≤ 3** | [[definitions/agents|AGENTS.md]] §5 | Rate limit → iş durur (§6.1) |
| 2 | Aynı dosyaya eş zamanlı yazan | **1** (tek yazıcı) | [[governance/workflow|WORKFLOW.md]] §3.2 | Ezilme varsa seriley al |
| 3 | Aynı oturumda dokunulan dosya | **≤ 3** | [[definitions/agents|AGENTS.md]] §6 | Yeni oturuma böl |
| 4 | Aynı anda dokunulan dosya (blok içi) | **≤ 8** | [[definitions/agents|AGENTS.md]] §6 | Bölümlere ayır |
| 5 | Paralel bloktaki bağımlılık | **0** (birbirine bağlanmayan iş) | [[governance/workflow|WORKFLOW.md]] §3.2 | Bağımlıysa sırala |

**Kayıt — 2026-09-29 rate limit olayı (gerçek):**

| Alan | Değer |
|---|---|
| **Tarih** | 2026-09-29 |
| **Deneme** | Aynı anda **8 paralel ajan** başlatıldı |
| **Belirti** | Ajanların tamamına yakın `Rate limit exceeded` — işler tamamlanmadan düştü |
| **Neden** | Sağlayıcı hız sınırı; eşzamanlı istek limiti aşıldı |
| **Müdahale** | Paralel ajan sayısı **3'e** düşürüldü; kalan iş seriye alındı |
| **Kurala dönüş** | [[definitions/agents|AGENTS.md]] §5 = "en fazla 3 paralel ajan"; [[governance/workflow|WORKFLOW.md]] §3.2 blok kuralı aynı sayıyı tekrarlar |
| **Kalan belirsizlik** | Hangi model/sağlayıcının limiti döktüğü ve ajan ID listesi `DOĞRULAMA GEREKLI` (AGENTS §0 kaynak satırı) |

> **KURAL:** 3 sayısı **kestirim değil, ölçülmüş sonuçtur** (2026-09-29 olayı). Yeni bir eşzamanlılık denemesi ancak `log.md`'ye tarihli kayıtla ve kullanıcı onayıyla yapılır.

### §2.2 Paralel mi, seri mi? (karar matrisi)

| Durum | Mod | Neden |
|---|---|---|
| Farklı katmanlarda, bağımsız işler (ADR-028 katman bağları dahilinde) | **PARALEL** | Bağımlılık yok, kanıt birbirinden bağımsız |
| Aynı `.cs` dosyasına iki yazıcı gerekiyor | **SERİ** | Çakışma kesindir |
| Aynı modülü / aynı test dosyasını değiştirenler | **SERİ** | Aynı kanıt satırı iki ajan tarafından üretilir |
| Bağımlı sıra: `architect -> developer` | **SERİ** | Sözleşme yoksa kod tahminle yazılır (G9) |
| `l0 -> l1 -> l2` (BLOK A) | **SERİ** | Her katman bir öncekinin sözleşmesine bağlı |
| `l3 \|\| l4 \|\| l5` (BLOK B) | **PARALEL (≤3)** | Üçü birbirine doğrudan bağlanmaz |
| `l6 -> (l7 Desktop \|\| l7 Cli)` (BLOK C) | l6 tek, sonra **PARALEL (≤3)** | DI kaydı iki kez değişmesin |

**Blok şeması (saf ASCII):**

```text
  BLOK A - SERI            BLOK B - PARALEL (<=3)       BLOK C
  ----------------          ----------------------       ----------------
   (0) l0 Abstractions             |
         |                         |   +-- l3 Crosscutting
         v                         |   |
   (1) l1 Domain                   |   +-- l4 Infrastructure
         |                         |   |
         v                         |   +-- l5 Protocol
   (2) l2 Application              |
         |                         |
         +------ A bitti ----------+------ B bitti ------+
                                           |
                                           v
                                  (6) l6 Host
                                           |
                             +-------------+-------------+
                             v                           v
                       (7a) l7 Desktop            (7b) l7 Cli
```

> Kaynak: [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] §4 + [[governance/workflow|WORKFLOW.md]] §3.1. Makro katmanlar `l0..l7` = Abstractions, Domain, Application, Crosscutting, Infrastructure, Protocol, Host, UI+CLI.

### §2.3 Ezme (write conflict) çözümü

| Adım | Kural | Detay |
|---|---|---|
| 1 | **Tespit** | İki ajan aynı dosyayı istiyorsa fark edilir; eşzamanlı yazım kaybı `KAPSAM` satırından görünür |
| 2 | **Kanıt yarışı** | Kimin işinin kanıtı varsa (build/test çıktısı) o kazanır |
| 3 | **Seriye alma** | Kanıt yoksa [[governance/workflow|WORKFLOW.md]] §3.2 kuralı devreye girer: tek yazıcı |
| 4 | **Devir** | İkinci ajan [[definitions/role|ROLE.md]] §5 zincirine ve §3'teki 4 maddelik pakete geçer |
| 5 | **Sahiplik** | Tek dosya = tek sahip; sahip dışındaki ajan okur, yazar değil ([[definitions/agents|AGENTS.md]] §7) |

**Ezme önleme iş akışı:**

```text
 [ ajan A ] --+                    [ ajan B ]
              |                          |
              +--> ayni dosya?  <--+
                     |
             +-------+-------+
             |               |
            EVET           HAYIR
             |               |
             v               v
          SERI            PARALEL
     (kanitli olan kazanir)   (<=3 ajan)
```

---

## §3 Devir Zinciri

### §3.1 Devre paketi — 4 madde zorunlu

Bir ajan işi başka ajana devrettiğinde **4 madde** yazmak zorundadır; eksik devir iş emri değil, **taslaktır** ve alıcı tarafından geri çevrilebilir.

| # | Madde | İçerik | Eksikse |
|---|---|---|---|
| 1 | **KALAN İŞ** | Ne bitti, ne kaldı (madde madde) | Alıcı kapsamı tahmin eder (G9) |
| 2 | **DOSYA** | Dokunulan dosya yolları + satır aralığı | Ezme tespiti imkânsız (§2.3) |
| 3 | **KANIT** | Eldeki kanıt (build/test çıktısı varsa) | G5 ihlali — iş "bitti" sayılmaz |
| 4 | **SINIR** | Aşılmayacak katman/kapsam sınırı (ADR-028) | Katman ihlali gate dışında kalır |

> **Bütçe:** Devre paketi en fazla **~2 KB**'tır (§5); aşım açıklamayı kısaltır, **4 madde asla silinmez**.

### §3.2 Zincirin halkaları (ROLE §5 — 3 hamle)

| Hamle | Kim | Taşıdığı | Koparsa |
|---|---|---|---|
| 1 | `architect` → `developer` | Katman/proje kararı + SINIR + ADR linki | Kod sözleşmesiz yazılır → NetArchTest RED |
| 2 | `developer` → `reviewer` | Diff + DOSYA + KANIT + kalan iş | İnceleme kanıtsız yapılır → sahte GREEN |
| 3 | `reviewer` → `tester` | Bulgu listesi + RİSK + satır no | Test yanlış yere yazılır → kapsam boş |

**Zincir kuralı:** halkalar atlanamaz; atlanan halkada **kabul kriteri kanıtsız** kalır. Zincir 3 hamleden sonra koparsa iş **kullanıcıya döner** (bunun adı `dur/bekle`, §4).

### §3.3 Dolu devre paketi örneği

```text
1. KALAN IS  : SqliteProvider yazildi (4/4 metod). Kalan: kontrat testi
               + NetArchTest "Application has no ADO.NET" yeşil degil.
2. DOSYA     : src/VersaCoder.Infrastructure/Data/SqliteProvider.cs : 12-164
               tests/.../ProviderContractTests.cs : 1-58 (eski hali)
3. KANIT     : dotnet build -> 0 hata (net48, net10.0-windows)
               dotnet test  -> 41 gecti / 1 basarisiz / 2,4 sn
4. SINIR     : l4 tek basina; l2'ye ok YOK (ADR-028 8x8 matris),
               EF Core yok, string-concat SQL yok.
ALICI        : tester   |  IADE HAKKI: 4 madde eksikse devir geri cevrilir
```

### §3.4 Devirde yapılmayanlar

| # | Yasak | Yerine |
|---|---|---|
| 1 | Sözel devir ("bakar mısın?") | 4 maddelik yazılı paket (§3.1) |
| 2 | Kanıtsız devir (`KANIT` satırı boş) | Çıktıyı çalıştır, sonra devret |
| 3 | SINIR'sız devir (katman sınırı yazılmadan) | ADR-028 + [[checklists/layer-boundary-check|layer-boundary-check]] satırı |
| 4 | Devirden sonra işi takip etmeye devam | Dosya sahibi devredilir ([[definitions/agents|AGENTS.md]] §7) |
| 5 | Eksik devri alıp devam etmek | Alıcı hakkı: geri çevir + `bekle` + tamamlat |

---

## §4 3-STRIKE ve Dur / Bekle

### §4.1 Strike tablosu

```text
Strike 1 -> ayni yontemle tekrar dene (kucuk duzeltme)
Strike 2 -> yontemi DEGISDIR (farkli yaklasim, ayni hedef)
Strike 3 -> DUR - baska ajana devret veya kullaniciya sor
```

| # | Strike | Ne yapılır | Yapılmaz | Sayaç |
|---|---|---|---|---|
| 1 | Aynı yöntemle 2. deneme | Küçük düzeltme + hipotezi not al | Kapsamı sessizce genişletmek | Açık |
| 2 | Yöntem değişikliği | Farklı yaklaşım, aynı hedef | Aynı düzeltmenin birebir tekrarı | Açık |
| 3 | **DUR** | Bloke raporu (§4.3) + `dur/bekle` | **4. deneme yasak** | Kilitlenir |

**Sayaç kuralları:**

| Durum | Sayılır mı? | Neden |
|---|---|---|
| Aynı iş içinde art arda düzeltme | **Evet** | Strike sayacı iş içindedir |
| Yeni işe geçildi | Hayır — **yeni sayaç** | İş değişti, yöntem değişti |
| Kullanıcı yeni veri/koşul getirdi | Hayır | Yeni bilgi = yeni girdi |
| Bütçe aşımı | Hayır | Bütçe işi küçültür, işi ajan değiştirmez ([[definitions/agents|AGENTS.md]] §4) |
| 3. strike sonrası "bir daha deneyeyim" | **Yasak** | Tahmin artık israfın işaretidir |

### §4.2 Dur / bekle etiketleri

| Tetikleyici | Etiket | Yanıt |
|---|---|---|
| GATE-003 kapalı, kod isteniyor | `GATE-OPEN` | Kod yazılmaz; onay istemi gönderilir |
| Aynı hata 3 düzeltmede çözülmedi | `3-STRIKE` | Durdur + şüpheli varsayımı adlandır + A/B/C |
| Yıkıcı işlem (silme, git reset, migration) | `G7 ONAYI` | Onay gelmeden komut çalışmaz |
| Yeni paket / sürüm / ORM | `G8` | Kullanıcı onayı; "geçici çözüm" eklenemez |
| Kaynak çelişti | `CONFLICT DETECTED` → `DECISION BLOCKED` | Hiyerarşi denenir, çözülmezse tek soru |
| Kanıt yok | `UNKNOWN` / `NEEDS_VALIDATION` | Madde yazılmaz, iş durur |

### §4.3 Bloke raporu (şablon + dolu örnek)

**Şablon (biçim yalnız burada; [[governance/workflow|WORKFLOW.md]] §4 bunu referans verir):**

```text
ETIKET     : 3-STRIKE | GATE-OPEN | G7 ONAYI | UNKNOWN | CONFLICT DETECTED
KONUM      : <dosya yolu> : <satir>   ya da   <gate adi>
NEDEN      : denenip dusen sey, 1 cumle
VARSAYIM   : supheli varsayim, tek madde
SECENEKLER : A) ...   B) ...   C) ...
SORU       : kullaniciya tek soru (evet/hayir olabilmeli)
```

**Dolu örnek:**

```text
ETIKET     : 3-STRIKE
KONUM      : src/VersaClimb.L1.ClimbingLog/LogService.cs : 42
NEDEN      : 3 duzeltmede ayni test Assert.Fail ile dustu
VARSAYIM   : test, saat dilimi yerine UTC bekliyor olabilir
SECENEKLER : A) testi UTC'ye cek   B) servisi UTC'ye cek   C) kapsami dondur
SORU       : Beklenen kaynak saati UTC mi, yerel saat mi?
```

**Rapor kuralları:**

| Kural | Değer | Aşımında |
|---|---|---|
| Soru sayısı | **1** | Kalanı SECENEKLER'e taşı |
| RİSK maddesi | **≤ 5** | En yüksek severity'yi al, gerisini arşivle |
| Konum | Dosya + satır **ya da** gate adı | Tahmini konum yazılmaz |
| Durma noktası | 3. strike = protokol zorunlu | Kod yazımı durur, varsayım sorgulanır |

> **UYARI:** 3. strike'tan sonra çözüm aramak yasaktır. Sorulacak soru tek: *"Hangi varsayım yanlış olabilir?"* — cevap [[definitions/ultra-thinking|ULTRA-THINKING.md]] protokolünün girişidir.

---

## §5 Bütçe (token / byte)

| Kalem | Sınır | Aşım belirtisi | Aşımda ne yapılır |
|---|---|---|---|
| Tek ajan çıktısı | **≤ ~8 KB** | Yanıt taşması, özet kaybı | Özetle; detayı [[memory/memory|MEMORY.md]]'e taşı |
| Devre paketi (§3) | **≤ ~2 KB** | Kutu içinde açıklama şişmesi | 4 maddeyi koru, açıklamayı kısalt |
| Aynı anda dokunulan dosya | **≤ ~8 dosya** | Kapsam belirsizleşiyor | Bölümlere ayır, [[governance/workflow|WORKFLOW.md]] §4'e geç |
| Bir oturumda toplam dosya | **≤ ~3 dosya** | Sahiplik dağılıyor | Yeni oturuma böl, kanıtı [[ops/log|log.md]]'e yaz |
| RİSK maddesi | **≤ 5 madde** | Liste uzuyor | En yüksek severity + arşiv |
| Bloke raporu sorusu | **≤ 1 soru** | Karar verilmemiş görünüyor | Kalanı SECENEKLER'e taşı |
| Paralel ajan | **≤ 3** | Rate limit (§6.1) | 3'e düşür + seriley al |
| Gate süresi aşımı | tahminin **20 dk** üzerinde | Kapı uzuyor | İş durur, raporlanır ([[governance/workflow|WORKFLOW.md]] §2) |

**Bütçe ölçüm notu:**

| Not | Değer |
|---|---|
| `~` ile yazılan limitler | **kestirim** (`INFERRED`), ölçülmemiş token/byte eşiği |
| Kesin ölçüm gerektiğinde | Sayac/çıkıntı ile ölçülür, `log.md`'ye tarihli yazılır |
| Bütçe aşımı strike mı? | **Hayır** — bütçe işi küçültür, ajanı değiştirmez |
| Bütçe ile gate çakışırsa | Gate kabul kriteri kazanır; bütçe kısaltılır |

> **KURAL:** Bütçe aşımı bir **durma etiketi değildir**; iş bölünür. Durma etiketleri yalnız §4.2'deki 6 tanedir.

---

## §6 Hata Modları

> Her hata modu üç sütunla yazılır: **belirti -> neden -> müdahale**. Müdahaleden sonra **kanıt** satırı zorunludur; kanıtsız müdahale işe yaramamış sayılır.

### §6.1 Rate limit (en sık, 2026-09-29 kaydı)

| Alan | Belirti | Neden | Müdahale |
|---|---|---|---|
| 1 | `Rate limit exceeded` — ajanlar tamamlanmadan düşüyor | Eşzamanlı istek sayısının sağlayıcı limitini aşması | Paralel ajanı **3'e indir**; kalan işi seriye al (§2.1) |
| 2 | Aynı anda 8 ajan çalıştırıldı | Limit bilinmeden fan-out | Kayıt: 2026-09-29 olayı → kural [[definitions/agents|AGENTS.md]] §5 |
| 3 | Seriye alınan işte de tekrar limit | Kalan isteklerin birikmesi | İşleri [[memory/memory|MEMORY.md]]'e böl, oturumu kapat/aç |
| 4 | Hangi modelin limiti döktüğü bilinmiyor | Model/sağlayıcı listesi kayıtlı değil | `DOĞRULAMA GEREKLI` — tahmin yazılmaz (G9) |

**Rate limit sonrası normal akış:**

```text
 8 paralel ajan
      |
      +--> "Rate limit exceeded"  (2026-09-29 kaydi)
      |
      v
 PARALEL SAYISI = 3  +  kalan is SERI
      |
      v
 her ajan kendi 5 alanli kutusunu doner
      |
      +--> kutusuz ajan blok sonucuna SAYILMAZ
```

### §6.2 Model unavailable

| Alan | Belirti | Neden | Müdahale |
|---|---|---|---|
| 1 | Ajan hiç başlamıyor / sağlayıcı hatası döner | Model adı, sağlayıcı ya da anahtar geçersiz | Sağlayıcı + model adını doğrula; `UNKNOWN` yaz, tahmin etme |
| 2 | Bazı ajanlar düşüyor, bazıları çalışıyor | Seçilen model erişimli değil | İşi erişilebilir modele devret (§3 paketiyle) |
| 3 | Hata tekrar tekrar aynı | Sağlayıcı tarafında süreklilik | 3. kezse **3-STRIKE** → bloke raporu (§4.3) |
| 4 | "Geçici olarak yok" denip devam edildi | Kanıtsız varsayım | `NEEDS_VALIDATION` + komut; iş durur |

### §6.3 Bozuk link

| Alan | Belirti | Neden | Müdahale |
|---|---|---|---|
| 1 | Link taraması `BROKEN > 0` | Hedef dosya yok / ad yanlış / yol yanlış | Hedefi düzelt ya da dosyayı oluştur; sonra **yeniden tara** |
| 2 | Gate kırmızı (GATE-004a/006) | 0 bozuk link kabul kriteri | Kriter sağlanmadan gate kapanmaz |
| 3 | `../` ile izinli olmayan hedef | Link kuralı ihlali | Sadece izinli hedeflere göreli link yaz |
| 4 | Var ama okunmamış dosyaya link | Navigasyon kırık | [[index|index.md]] güncellenir |

### §6.4 Tam hata modu tablosu

| # | Belirti | Kök neden | Müdahale | Etiket |
|---|---|---|---|---|
| 1 | `Rate limit exceeded` | ≤ 3 ajan sınırı aşıldı | Paraleli 3'e indir, kalanı seriye al | §2.1 |
| 2 | Ajan hiç dönmüyor (model yok) | Model/sağlayıcı erişilebilir değil | Devret + `UNKNOWN` yaz | `UNKNOWN` |
| 3 | `BROKEN > 0` | Var olmayan hedefe link | Hedefi düzelt, yeniden tara | vault RED |
| 4 | Frontmatter'sız dosya | İlk satır `---` değil | 8 alanı tamamla (G4) | vault RED |
| 5 | Mojibake (`C3 B6` kalıntıları) | UTF-8 bozuk yazım | Dosyayı UTF-8 ile yeniden yaz | vault RED |
| 6 | Diyagramda `U+2500-U+257F` | Unicode kutu çizim kullanılmış | `\| - +` ASCII'ye çevir | vault RED |
| 7 | İki ajan aynı dosyada | Tek yazıcı kuralı ihlali | Kanıt yarışı → seriley al (§2.3) | ezme |
| 8 | `KANIT` satırı boş kutu | Kanıtsız teslim (G5) | Aynı kutuyla tekrar isten | reddedildi |
| 9 | Gate kapalı, `.cs` yazıldı | `GATE-OPEN` (G12) | Yazılan geri alınır, onay istenir | `GATE-OPEN` |
| 10 | 3. düzeltme de kırmızı | Yöntem değişmedi / yanlış varsayım | DUR + bloke raporu (§4.3) | `3-STRIKE` |
| 11 | İki kaynak farklı diyor | Kaynak çelişkisi (G10) | `CONFLICT DETECTED` → protokol | `DECISION BLOCKED` |
| 12 | Gate tahminin 20 dk üzerinde | Süre bütçesi aşıldı | İş durur, raporlanır | gate RED |
| 13 | Aynı iş 2. kez yapılıyor | Devir paketi eksik/kanıtsız | 4 maddeyi tamamlat, geri çevir | iade |
| 14 | "Muhtemelen geçer" raporu | Çıktısız tamamlama (G5) | Komutu çalıştır, çıktıyı yapıştır | RED |
| 15 | Kapsam isteği büyüdü | Kapsam kayması | İlgili gate yeniden açılır | [[governance/workflow|WORKFLOW.md]] §1.2 |

**Müdahale sırası (her hata için tek yol):**

```text
 belirti  -->  kok nedeni adlandir  -->  mudahaleyi uygula
                                              |
                                              v
                                      kanit uret (komut ciktisi)
                                              |
                                    +---------+---------+
                                    |                   |
                                kanit var           kanit yok
                                    |                   |
                                    v                   v
                             5 alanli kutu         NEEDS_VALIDATION
                             + log kaydi            + dur/bekle
```

---

## §7 Doğrulama

| # | Kontrol | Beklenen | Kanıt |
|---|---|---|---|
| 1 | Frontmatter 11 alan dolu | `authority: primary` · `owner: architect` · `updated: 2026-09-29` | İlk satır `---` |
| 2 | Bu dosyanın tüm linkleri izinli hedefe | Bozuk link **0** | link taraması |
| 3 | Diyagramlar saf ASCII | `\| - +` dışında kutu çizim **0** | `U+2500-U+257F` taraması = 0 |
| 4 | 5 bileşen tek tek tanımlı | Giriş · Hazırlık · Dağıtım · Kontrol · Teslim | §1 |
| 5 | Paralel limiti yazılı | **≤ 3 ajan** | §2.1 + [[definitions/agents|AGENTS.md]] §5 |
| 6 | Rate limit olayı kayıtlı | 2026-09-29 · 8 ajan → 3 | §2.1 olay tablosu |
| 7 | Paylaşılan dosya = seri | Karar matrisi var | §2.2 |
| 8 | Devir paketi 4 madde | KALAN İŞ · DOSYA · KANIT · SINIR | §3.1 + dolu örnek |
| 9 | 3-STRIKE kuralı | 3. strike = DUR, sayaç sıfırlanmaz | §4.1 |
| 10 | Bloke raporu biçimi | 6 alanlı şablon + dolu örnek | §4.3 |
| 11 | Bütçe tablosu | 8 kalem, `~` = kestirim notu | §5 |
| 12 | Hata modları 3 sütunlu | belirti → neden → müdahale | §6 |
| 13 | Kutu ayrımı net | ajan kutusu (5) ≠ gate kutusu (4) ≠ hata raporu | §1.5 · §1.6 |
| 14 | Gate'lerde kapatan = kullanıcı | 5 gate de öyle | §1.5 + [[governance/workflow|WORKFLOW.md]] §2 |
| 15 | Katman adları tutarlı | l0..l7 = ADR-028 ile aynı | §2.2 + [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] |
| 16 | Kanıtsız "bitti" yok | Her iddia komut çıktılı | [[claude|CLAUDE.md]] G5 |
| 17 | Uydurma sayı yok | Bilinmeyen = `DOĞRULAMA GEREKLI` | §2.1 kalan belirsizlik satırı |
| 18 | Boyut bandı | **30–88 KB** | dosya ölçümü |

---

## §8 Referanslar

| # | Kaynak | Ne alındı | Yol |
|---|---|---|---|
| 1 | Ajan matrisi · kutu · devir · 3-STRIKE · eşzamanlılık · bütçe | §2–§5 | [[definitions/agents|AGENTS.md]] |
| 2 | Gate zinciri · kanıt kutusu · paralel bloklar · süreler | §1.5 · §2.2 | [[governance/workflow|WORKFLOW.md]] |
| 3 | Karar hiyerarşisi · tek soru · devir zinciri (3 hamle) | §3.2 · §4.3 | [[definitions/role|ROLE.md]] |
| 4 | G1..G12 · kanıt formatı · dur/bekle · yasaklı tarama | §0 · §1.5 · §4.2 | [[claude|CLAUDE.md]] |
| 5 | Katman/proje/blok kararı (bağlayıcı) | §2.2 | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] |
| 6 | Katman sınırı 13 sorusu | §1.4 · §3.1 | [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] |
| 7 | Derin düşünme protokolü (karar, kanıt, tuzak) | §4.3 | [[definitions/ultra-thinking|ULTRA-THINKING.md]] |
| 8 | Oturumlar arası durum · tarihli kapanış | §1.6 · §5 | [[memory/memory|MEMORY.md]] · [[ops/log|log.md]] |
| 9 | Navigasyon · anahtar kelimeler · terimler | §1.3 | [[index|index.md]] · [[governance/keys|keys.md]] · [[definitions/glossary|glossary.md]] |
| 10 | Yeni dosya sözleşmesi (frontmatter/link/boyut) | §7 | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] · [[.templates/index|.templates/index.md]] |
| 11 | Mimari indeks · rapor indeksi | §1.3 | [[architecture/index|architecture/index.md]] · [[ops/log|reports/index.md]] |
| 12 | ADR navigasyonu | §1.3 | [[.decisions/adr-index|.decisions/adr-index.md]] |
| 13 | Prompt kaynağı (end-to-end model, öncelik sırası) | §1.1.4 | `G:/…/versacoder proje planlaması ai.05.md` §47-§51 |
| 14 | Prompt kaynağı (wizard, MCP sandbox, hibrit bellek) | §1.1 | `G:/…/versacoder proje planlaması ai.06.md` §5.3-§5.6 |

**Doğrulama etiketleri (bu dosya için):** eşzamanlılık limiti + rate limit olayı (2026-09-29, 8 ajan → 3) → `SOURCE_VERIFIED` ([[definitions/agents|AGENTS.md]] §7.3, [[governance/workflow|WORKFLOW.md]] §3.2) · gate süreleri ve kabul kriterleri → `SOURCE_VERIFIED` ([[governance/workflow|WORKFLOW.md]] §2) · bütçe eşikleri → `INFERRED` (§5'te `~` ile işaretli, ölçülmemiş kestirim) · rate limit olayının model/sağlayıcı ayrıntısı → `DOĞRULAMA GEREKLI` · §1.1.4 end-to-end model → `SOURCE_VERIFIED` (ai.05 §47, 2026-10-07).
| VISION | [[definitions/vision|VISION]] | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [[definitions/projects|PROJECTS]] | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| ADR-031 | [[.decisions/draft/adr-031-visual-production-provider|ADR-031]] | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
