---
title: "VersaCoder — Süreç ve GATE Sistemi"
type: workflow
category: ".ai"
status: active
version: 2.0.0
updated: 2026-10-07
authority: SSOT
scope: "GATE-004a -> GATE-003 -> GATE-004 -> GATE-005 -> GATE-006 zinciri, her gate'in sahibi/kabul kriteri/kanıt kutusu/süre tahmini, paralel geliştirme akışı (l0..l7), commit kuralı ve uçtan uca yaşam döngüsü"
---

# VersaCoder — Süreç ve GATE Sistemi (`WORKFLOW.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE](../claude.md) · [ROLE](../definitions/role.md) · [AGENTS](../definitions/agents.md) · [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [architecture/index](../architecture/index.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [log](../ops/log.md) · [keys](keys.md)

> **Tek cümlelik sorumluluk:** *Hangi iş hangi gate'ten geçer, gate'i kim kapatır, kanıt nasıl sunulur, ne zaman durulur* sorusunun tek cevabıdır.
> Bu dosya **gate kimliğinin ve sıranın SSOT**'udur; ajan tanımı [AGENTS.md](../definitions/agents.md)dadır, rol/ses kuralları [ROLE.md](../definitions/role.md)dadır.

---

## §0 Özet (Answer First)

- **Tek zincir, 5 gate:** `GATE-004a (vault) -> GATE-003 (UI onayı, HARD STOP) -> GATE-004 (iskelet + build + ilk commit) -> GATE-005 (test) -> GATE-006 (teslim)`. Toplam tahmini emek **≈5 sa 55 dk**.
- **Tek durdurucu gate GATE-003'tür:** 7 wireframe + **kullanıcı onayı** olmadan `src/**` altına **tek satır `.cs` bile yazılamaz** (CLAUDE G12). Onaysız yazılan kod geri alınır.
- **Gate'i ajan kapatamaz:** ajan **hazırlar ve kanıt sunar**, kapatan **kullanıcı**dır. Kanıtsız kapatma = G5 ihlali = gate yeniden açılır.
- **İnşa sırası ADR-028 ile kilitli:** `(l0 -> l1 -> l2 seri) -> (l3 || l4 || l5) -> (l6 -> l7 Desktop || Cli)`. Paralel blokta aynı dosyaya iki yazıcı yok.
- **İlk git commit GATE-004'te** atılır; öncesi commit yoktur ve `git add -A` yasaktır (dosya bazlı `add`).

### §0.1 Uçtan uca mühendislik yaşam döngüsü (6 aşama — prompt kaynağı: ai.05 §5.1)

```text
┌─────────────────┐     ┌──────────────────────┐     ┌──────────────────────┐
│ 1. INTENT & AST │ ──► │ 2. SESSION WIZARD    │ ──► │ 3. SWARM ALLOCATION  │
│    ANALYSIS     │     │    (Config Synthesis)│     │    (DAG Workflow)    │
└─────────────────┘     └──────────────────────┘     └──────────────────────┘
         │                                                       │
         ▼                                                       ▼
┌─────────────────┐     ┌──────────────────────┐     ┌──────────────────────┐
│ 6. PERSISTENCE  │ ◄── │ 5. VERIFICATION &    │ ◄── │ 4. EXECUTION, TOOLS  │
│    & AUDIT SYNC │     │    SELF-HEALING      │     │    & MCP INVOCATION  │
└─────────────────┘     └──────────────────────┘     └──────────────────────┘
```

1. **Niyet ve Proje Analizi:** `IntentAnalysisEngine` + Roslyn AST ile bağımlılık ağacı, risk seviyesi taranır.
2. **Session Agentic Agent Wizard:** Ajan kombinasyonu, model, MCP araçları ve güvenlik izinleri önceden yapılandırılır; onayla → immutable `SessionConfiguration`.
3. **Görev Ayrıştırma (DAG):** Orchestrator işi yönlü döngüsüz grafiğe çevirir; Architect → Developer → Security → Testing ajanlarına dağıtır.
4. **Yürütme ve Tool Gateway:** MCP, Roslyn AST, Git, dosya sistemi araçları Policy Engine'den geçerek çalışır.
5. **Doğrulama ve Self-Healing:** Derleme + test + static analysis; hata ilgili ajana geri beslenir, düzelene kadar iterasyon.
6. **Kalıcılık ve Audit:** Git staging, ADR kaydı, oturum geçmişi, `.ai` bilgi kasası senkronu.

> Bu 6 aşama gate zinciriyle (§1) **çelişmez**; gate'ler bu yaşam döngüsünün
> **onay duraklarıdır**: aşama 2 sonunda GATE-003, aşama 4 sonunda GATE-004,
> aşama 5 sonunda GATE-005, aşama 6 sonunda GATE-006 kapanır.

---

**İlgili:** [AGENTS](../definitions/agents.md) §1.4 (prompt işleme hattı) · [engine](../definitions/engine.md) §1.1.2 (hedef sistem) · [VISION](../definitions/vision.md) §2 (7 ekran akışları) · [CLAUDE](../claude.md) §B5 (katman listesi).
## §1 Gate Zinciri

### §1.1 ASCII görünüm

```text
 +-------------+  GATE-004a   +-------------+  GATE-003   +-------------+
 | VAULT YAZIMI| -----------> | 7 WIREFRAME | -----------> |  ISKELET +  |
 |  ~45 dk     |  vault green |  ~60 dk     |  KULLANICI  |  KOD        |
 +-------------+   (olcum)    +------+------+   ONAYI      |  ~90 dk     |
                                            |  HARD STOP   +------+------+
                                            |  (onaysiz .cs YASAK)  |
                                            v                        v
                                     dur / bekle            GATE-004
                                                            build green
                                                            + ILK COMMIT
                                                                   |
                                                                   v
                                                          +-------------+
                                                          |   TEST +    |
                                                          |  NetArchTest|
                                                          |   ~120 dk   |
                                                          +------+------+
                                                                   |
                                                            GATE-005 (test green)
                                                                   |
                                                                   v
                                                          +-------------+  GATE-006
                                                          |  TESLIM     | -------> TESLIM
                                                          |   ~40 dk    |          RAPORU
                                                          +-------------+
```

### §1.2 Sıra kuralları

| # | Kural | İhlalde |
|---|---|---|
| 1 | Gate'ler **sırayla** geçilir; atlanan gate geri gelir | Sıra RED → iş gate başına döner |
| 2 | Kapatan **kullanıcı**; ajan yalnız kanıt sunar | Sahte kapatma = G5 ihlali |
| 3 | GATE-003 kapandıktan **önce** hiçbir `.cs` yazılamaz | G12 → yazılan dosyalar silinir, iş başa alır |
| 4 | Geri dönüş yalnız **kanıtlı RED raporu** ile olur | Sessiz geri sarım yasak |
| 5 | Kapsam değişirse ilgili gate **yeniden açılır** | Kapsam kayması = GATE-004a/003 yeniden |

---

## §2 Gate Ayrıntıları

> Her gate için 4 alan zorunludur: **Sahip · Kabul kriteri · Kanıt kutusu · Süre tahmini**. Süreler bütçedir; tahminin **20 dk** üzerinde aşım olursa iş durur ve raporlanır.
> **Kanıt kutusu** burada 4 satırlıdır (`KANIT/DOSYA/EKSIK/SONRAKI`); ajanların 5 alanlı çıktı kutusu [AGENTS.md](../definitions/agents.md) §2'dedir — ikisi karıştırılmaz (SSOT).

### §2.1 GATE-004a — Vault Green (vault ölçümü)

| Alan | Değer |
|---|---|
| **Sahip** | Üretim: `architect` · Kapatan: **kullanıcı** |
| **Kabul kriteri** | **500 MD** tam · her dosyada **8 alanlı frontmatter** · **0 bozuk link** · **0 mojibake (UTF-8)** · diyagramlarda **0 Unicode kutu çizim** · 4 çekirdek dosya (CLAUDE/WORKFLOW/ROLE/AGENTS) 10-16 KB bandında |
| **Kanıt kutusu** | `KANIT: sayac -> MD=NNN, FM=0 eksik, BROKEN=0, MOJIBAKE=0, BOX=0` · `DOSYA: <yazılanlar>` · `EKSIK: <YOK ya da DOĞRULAMA GEREKLI>` · `SONRAKI: <adım, ≈ süre>` |
| **Süre tahmini** | **≈45 dk** |
| **Red nedenleri** | Eksik frontmatter · bozuk link · mojibake · kutu çizim · ölçüm çıktısı olmadan "vault hazır" beyanı |

```text
KANIT   : <sayac> -> MD=500, FM=0 eksik, BROKEN=0, MOJIBAKE=0
DOSYA   : .ai/CLAUDE.md, .ai/WORKFLOW.md, .ai/ROLE.md, .ai/AGENTS.md
EKSIK   : YOK   (varsa: DOĞRULAMA GEREKLI maddesi yazilir)
SONRAKI : ui-design/01-start-page.md yaz (~7 dk)
```

**Ne test eder:** Bu gate, GATE-003'ten GATE-006'ya kadar dayanılan **bilgi zeminini** ölçer: dosya diskte var mı, frontmatter sekiz alanlı mı, verilen link hedefe gidiyor mu, kayıt UTF-8 mi, diyagram saf ASCII mi. "Vault hazır" cümlesi tek başına **geçersizdir**; kapıdan geçen tek şey sayacın sayısal çıktısıdır (CLAUDE G5). Adım adım soru listesi [gate-004a-vault-kapanis](../checklists/gate-004a-vault-kapanis.md) 14 maddesidir; kutu sonucu, liste yolu verir.

**Kabul kriterleri (madde madde):**

1. `measure-vault.ps1` → `TotalMD` hedefin üstünde, `NoFrontmatter = 0`
2. `check-links.ps1` → `BOZUK link : 0` ve süreç sıfır çıkış koduyla biter
3. `find-mojibake.ps1` → `Mojibake file count: 0`
4. `count-components.ps1` → `PASS`; bileşen kaydı ADR-028 §3'ün ölçüm değerine eşittir (marj 0)
5. 14 kök belge disktedir, her birinin ilk satırı `---`, `authority` alanı doludur

**Kanıt kutusu — dolu örnek:**

```text
KANIT   : measure-vault -> MD=500, FM=0 eksik | check-links -> BOZUK=0
          find-mojibake -> 0 | count-components -> PASS (5440 kayit)
DOSYA   : .ai/WORKFLOW.md, .ai/ROLE.md, .ai/AGENTS.md, .ai/checklists/index.md
EKSIK   : YOK
SONRAKI : ui-design/01-start-page.md yaz (~7 dk)
```

**Kırmızı döndüren nedenler:**

| # | Neden | Konsol belirtisi | Sonucu |
|---|---|---|---|
| 1 | Link verilen hedef diskte yok | `BOZUK link` sayısı 0'dan büyük | G4 ihlali · gate açık kalır |
| 2 | Frontmatter eksik ya da 8 alan değil | `NoFrontmatter` sayısı 0'dan büyük | Dosya tamamlanır, sayaç yeniden koşulur |
| 3 | Kayıt CP1254/latin karışımı | `Mojibake file count` 0'dan büyük | Dosya UTF-8 olarak yeniden yazılır |
| 4 | Diyagramda Unicode kutu çizim satırı | `U+2500-U+257F` taraması 0'dan büyük | Diyagram dikey çizgi, tire ve artı ile yeniden çizilir |
| 5 | Çıktısız "vault hazır" beyanı | Konsol alıntısı yok | Beyan geçersiz, gate RED (G5) |
| 6 | Eski vault'tan kopya izi | `git status` içinde beklenen `.ai/` dışında değişiklik | Dosya silinir, sıfırdan yazılır (G1) |

**Süre tahmini:** **≈45 dk**; iş kalemleri ölçüm → hata listesi → onarım → tekrar ölçüm → rapor sırasıyla yürür. Kalem bazlı dakika ayrımı `DOĞRULAMA GEREKLI` (ilk gerçek ölçümle güncellenir). Bütçe aşımı **20 dk**'yı geçerse iş durur ve raporlanır (§2 giriş kuralı).

**Kim onaylar:** Toplayan `architect` · biçimi denetleyen `reviewer` (AGENTS §1 inceleme tipi) · **kapatan kullanıcı**. Ajan bu gate'i kapatabildiğini söyleyemez; söylemesi G12 ihlalidir.

### §2.2 GATE-003 — UI Approval (HARD STOP)

| Alan | Değer |
|---|---|
| **Sahip** | Üretim: `ui-designer` · Mimari kısıt: `architect` · Kapatan: **yalnız kullanıcı** |
| **Kabul kriteri** | **7/7 wireframe** (`01-start-page` … `07-settings`) · **saf ASCII** · kontrol listesi **yalnız DevExpress v26.1** · `RibbonForm` + `RibbonControlStyle.Office2010` · **kullanıcının açık onayı** yazılı |
| **Kanıt kutusu** | `KANIT: 7/7 dosya + BOX=0 + v26.1 disi kontrol=0` · `DOSYA: ui-design/01..07` · `EKSIK: <kontrol seçimi belirsizlikleri>` · `SONRAKI: <onay istemi>` |
| **Süre tahmini** | **≈60 dk** üretim + **≈12 dk** onay istemi · onay bekleme süresizdir |
| **Red nedenleri** | Onaysız kodlama · eksik ekran · Unicode kutu · v26.1 dışı kontrol · ajanın onayı kendisi vermesi |

**Onay istemi (zorunlu biçim):**

```text
GATE-003 - UI onayi bekleniyor (HARD STOP)
  Ekran   : 7/7 (ui-design/01..07, saf ASCII)
  Kontrol : DevExpress v26.1 + RibbonForm + RibbonControlStyle.Office2010
  Secenek : A) onayla   B) <ekran> icin degisiklik iste   C) 7 ekran listesini degistir
  Sure    : onay ~5 dk -> GATE-004 hemen baslar; onaysiz TEK SATIR .cs YAZILMAZ
```

> **HARD STOP:** Bu gate kapalıyken kod yazımı **yasak** (CLAUDE G12). Onay alınmadan yazılan her `.cs` geri alınır; wireframe değişirse kod çöp olur ve iş başa döner.

**Ne test eder:** Ekranların daha kod yazılmadan **sözleşmeye** bağlanıp bağlanmadığını: 7 wireframe diskte mi, ikisi de saf ASCII mi, kontrol seti yalnız DevExpress v26.1 mi, ana pencere `RibbonForm` ve ribbon stili `RibbonControlStyle.Office2010` mı, kullanıcının **yazılı onayı** var mı. Bu gate aynı zamanda tek **ürün kararı** kapısıdır: kapsam ve ekran adı tartışması buradan öteye taşınmaz. Onaya giden 13 adım [gate-003-ui-onay](../checklists/gate-003-ui-onay.md) listesindedir.

**7 wireframe — dosya ve durum tablosu:**

| # | Dosya | İçerik sorusu | Durum |
|---|---|---|---|
| 1 | `ui-design/01-start-page.md` | Açılışta ne görünür, ilk eylem nedir | Adı kesin (gate-003 kontrolü Soru 1) |
| 2 | `ui-design/02-main-ide.md` | Ribbon, çalışma alanı ve panel düzeni | Adı kesin (gate-003 kontrolü Soru 5) |
| 3 | `ui-design/03-...` | Ekran adı vault SSOT'unda yer almadı | `DOĞRULAMA GEREKLI` |
| 4 | `ui-design/04-...` | Aynı durum | `DOĞRULAMA GEREKLI` |
| 5 | `ui-design/05-...` | Aynı durum | `DOĞRULAMA GEREKLI` |
| 6 | `ui-design/06-...` | Aynı durum | `DOĞRULAMA GEREKLI` |
| 7 | `ui-design/07-settings.md` | Ayarlar ekranı hangi alanları taşır | Adı kesin (gate-003 kontrolü Soru 1) |

> 7 ekran listesinin **tek SSOT'u** usta prompttur (gate-003 kontrolü Soru 6); burada yeniden listelenmez. Aradaki dört dosyanın adı vault'a taşınmadığı için `DOĞRULAMA GEREKLI` işaretidir ve uydurulmaz (G9). Eşlikçi iki dosya zorunludur: kontrol envanteri `component-inventory.md` ve ekranlar arası geçiş `flows.md`.

**Kabul kriterleri (madde madde):**

1. `ui-design/01..07` — **7/7** dosya diskte, her biri 8 alanlı frontmatter'lı
2. Wireframe ve mimari diyagramlarda Unicode kutu çizim **0** satır
3. Envanterdeki her kontrol yalnız DevExpress **v26.1**; sürüm dışı satır 0
4. Ana pencere `RibbonForm`, ribbon stili **yalnız** `RibbonControlStyle.Office2010`
5. Kullanıcının **açık, yazılı** onayı `log.md`'ye tarihli satır olarak düşmüş

**HARD STOP neden HARD STOP:**

| # | Gerekçe | İhlalde ne olur |
|---|---|---|
| 1 | Ekran onayı **ürün kararıdır**; onu ajan veremez (ROLE §3 basamak 1) | Yanlış ekran kodlanır, yazılan her şey çöp olur |
| 2 | Wireframe değişirse form üçlüsü (`.cs` + `.Designer.cs` + `.resx`) birlikte değişir | Geri alma işi gate başından başlar |
| 3 | Kontrol seti geç fark edilirse **G3 CRITICAL**'e düşülür | Sürüm/ribbon düzeltmesi kodu kırar |
| 4 | Onaysız başlamak gizli kapsam genişlemesidir (G11 + G12) | Gate yeniden açılır, süre bütçesi iki kez yanar |

**İhlal edilirse uygulanacak işlem:**

| # | İhlal | Uygulama |
|---|---|---|
| 1 | Onaysız tek `.cs` yazılmış | Dosya geri alınır; gate açık sayılır (CLAUDE G12) |
| 2 | Wireframe eksik (7'den az) | Gate kapalı kalır, kod istemi reddedilir |
| 3 | Ajan onayı kendi kendine vermiş | Onay geçersiz; `GATE-OPEN` etiketiyle kullanıcıya gidilir |
| 4 | Onaydan sonra ekran değiştirilmiş | İlgili kod geri alınır, gate **yeniden** açılır |

**Kanıt kutusu — dolu örnek:**

```text
KANIT   : ui-design -> 7/7 dosya | kutu taramasi -> 0 satir
          envanter -> 26.1 disi kontrol 0 | log.md -> GATE-003 ONAY (tarihli)
DOSYA   : ui-design/01-start-page.md .. 07-settings.md, component-inventory.md, flows.md
EKSIK   : 03..06 ekran adlari usta prompt'ta netlestirilecek (DOĞRULAMA GEREKLI)
SONRAKI : iskelet kurulumuna basla (GATE-004, ~90 dk)
```

**Süre tahmini:** üretim **≈60 dk** + onay istemi **≈12 dk**; onay bekleme süresi **sınırsızdır** ve bütçeye girmez. Onay beş dakika içinde gelirse GATE-004 aynı oturumda başlayabilir (onay istemi biçimi yukarıda).

**Kim onaylar:** Üretim `ui-designer` · mimari kısıt `architect` · **kapatan yalnız kullanıcı**. Kullanıcı yazana kadar gate kapalıdır; ajanın "onaylı sayılır" demesi geçersizdir.

### §2.3 GATE-004 — Build Green + İlk Commit

| Alan | Değer |
|---|---|
| **Sahip** | İskelet: `architect` · Kod: `developer` · Kanıt: `reviewer` · Kapatan: **kullanıcı** |
| **Kabul kriteri** | **9 üretim + 1-2 test projesi** iskeleti (ADR-028 §2.1) · `dotnet build` → **0 hata × 2 bacak** (`net48`, `net10.0-windows`) · EF Core **0** · DevExpress **yalnız 26.1** · ribbon **yalnız Office2010** · `Directory.Packages.props`'a test paketleri eklendi (**NU1008 riski kapatıldı**) · **ilk git commit** atıldı |
| **Kanıt kutusu** | `KANIT: dotnet build -> 0 hata (22 hedef)` · `DOSYA: sln + 9 csproj + Directory.Packages.props` · `EKSIK: <NEEDS_VALIDATION>` · `SONRAKI: <adım>` |
| **Süre tahmini** | **≈90 dk** (iskelet ≈25 dk · iki bacak ≈10 dk · DevExpress ≈10 dk · provider/Dapper ≈15 dk · build ≈10 dk · devre ≈10 dk) |
| **Red nedenleri** | Tek bacak derleme · hata gizleme (`NoWarn`) · sürüm sapması · iskeletsiz commit |

**İlk commit kuralı:**

| # | Kural |
|---|---|
| 1 | İlk commit **yalnız** GATE-004 kapandıktan sonra atılır (vault yeşil + build yeşil) |
| 2 | Her zaman **dosya bazlı** `git add <yol>`; `git add -A` **yıkıcıdır** (G7) |
| 3 | Commit message: `<kapi/ tipi>: <kisa ozet>` (max 72 karakter) + `<KANIT>: ...` satırı |
| 4 | Kapsam dışı dosya (`*.old.*`, eski vault kopyası) **asla** stage edilmez (G1/G7) |

```text
iskelet: 9 uretim + 2 test projesi, cift TFM kuruldu
KANIT : dotnet build -> 0 hata (net48 + net10.0-windows)
```

**Ne test eder:** İskeletin gerçekten **iki bacakta** da derlendiğini, yasaklı içeriğin sıfır olduğunu ve ilk commit'in kurallara uygun atıldığını. Bu gate aynı anda **depo tarihinin başlangıcıdır**: öncesi commit yoktur, sonrası vardır. Commit öncesi 13 adım [gate-004-ilk-commit](../checklists/gate-004-ilk-commit.md) listesindedir.

**Kabul kriterleri (madde madde):**

1. `dotnet build -f net48` ve `dotnet build -f net10.0-windows` — ikisi de **0 hata**
2. Hata sayısını `NoWarn` ile gizlemek yok: proje ve props dosyalarında `NoWarn` araması **0 satır**
3. Yasak tarama üçü de **0 satır**: EF Core · DevExpress v26.1 dışı · string-concat SQL
4. `Directory.Packages.props` test paketlerini içerir → `dotnet restore` **0 NU1008**
5. İlk commit yalnız bu gate kapandıktan sonra, dosya bazlı `git add` ile atılmış

**Kanıt paketi (commit kanıtı olarak sunulan dört çıktı):**

- `git log -1 --format=%s` — ilk satır ön ekli ve 72 karakteri geçmiyor
- `git log -1 --format=%B` — gövdede `KANIT:` satırı duruyor
- `git show --name-only --format=` — stage edilenler yalnız kasıtlı dosyalar
- `git log --reverse --oneline` — ilk commit, build çıktısından **sonra** atılmış

**Kanıt kutusu — dolu örnek:**

```text
KANIT   : dotnet build -f net48 -> 0 hata | -f net10.0-windows -> 0 hata
          git log -1 -> "iskelet: ..." (<= 72 krk) + govdede KANIT satiri
          restore -> 0 NU1008 | yasak tarama -> 0 satir | NoWarn araması -> 0 satir
DOSYA   : sln + 9 uretim csproj + Directory.Packages.props + .ai/checklists/index.md
EKSIK   : DOĞRULAMA GEREKLI - Desktop designer baci TFM secimi (ADR-028 sart 6)
SONRAKI : kabul kriteri <-> test eslesme tablosunu yaz (GATE-005, ~120 dk)
```

**Kırmızı döndüren nedenler:**

| # | Neden | Belirti | Sonucu |
|---|---|---|---|
| 1 | Tek bacak derleniyor | Bir TFM'de hata ya da o bacakta hedef yok | G6 · gate açık (iki bacak zorunlu) |
| 2 | Hata `NoWarn` ile örtülmüş | Proje dosyasında `NoWarn` satırı bulunur | Hata geri açılır, build yeniden koşulur |
| 3 | Restore NU1008 veriyor | Test paketleri `Directory.Packages.props`'ta değil | `PackageVersion` satırları eklenir, restore tekrar |
| 4 | DevExpress sürümü ya da ribbon stili sapmış | 26.1 dışı ya da Office2010 dışı satır | **G3 CRITICAL** · kod RED |
| 5 | EF Core referansı girmiş | Paket satırı ya da `using` satırı | **G2 CRITICAL** · iş durur |
| 6 | Vault yeşil değilken commit atılmış | Commit tarihi gate kapanışından önce | Commit geri alınır (G7 onayı ile) |

**Süre tahmini:** **≈90 dk**. Tabloda altı kalem toplamıdır (25+10+10+15+10+10); kalan ≈10 dk, kanıt taramaları ve `log.md` kaydı için tampondur (hesap: 90 − 80). Aşım **20 dk** üzerindeyse iş durur ve raporlanır.

**Kim onaylar:** İskelet `architect` · kod `developer` · kanıt denetimi `reviewer` · **kapatan kullanıcı**. Commit, kapanıştan **sonra** atılır; ajan commit atıp "gate kapandı" diyemez.

### §2.4 GATE-005 — Test Green

| Alan | Değer |
|---|---|
| **Sahip** | Test üretimi: `tester` · PASS/RED: `reviewer` · Kapatan: **kullanıcı** |
| **Kabul kriteri** | `dotnet test` **tümü yeşil** · **iki bacakta** da çalışıyor · **NetArchTest 0 ihlal** (yön + circular) · kapsam kontrolü [layer-boundary-check](../checklists/layer-boundary-check.md) 13 sorusu **0 yanlis negatif** · coverage kanıtı ya da kapsam dışı gerekçesi yazılı |
| **Kanıt kutusu** | `KANIT: dotnet test -> X gecti / 0 basarisiz / Y sn` · `DOSYA: tests/**` · `EKSIK: <atlanan test varsa gerekce>` · `SONRAKI: <adım>` |
| **Süre tahmini** | **≈120 dk** (kabul kriteri eşlemesi ≈10 dk · birim ≈40 dk · entegrasyon ≈35 dk · mimari ≈15 dk · hata yolu ≈15 dk · coverage ≈5 dk) |
| **Red nedenleri** | Atlanan test · NetArchTest > 0 ihlal · tek bacakta test · kanıtsız coverage |

> NetArchTest matrisi = ADR-028 §2.3 madde 2'deki **8x8 matris** (`l4 -> l2 X`, `l5 -> l4 X`, `l3` yalnız `l0`). Ayrıntı [layer-boundary-check](../checklists/layer-boundary-check.md).

**Ne test eder:** Davranışın **kanıtını** ister: yeşil `dotnet test`, iki bacakta da aynı sonuç, mimari ok yasaklarının ihlalsizliği ve kapsamın yazılı gerekçesi. "Testler geçiyor" cümlesi bu gate'te tek başına yeterli değildir (CLAUDE G5); sayı verilmeyen rapor işlenmez. Kriter karşılığı 14 adım [gate-005-test-kapsami](../checklists/gate-005-test-kapsami.md) listesindedir.

**Kabul kriterleri (madde madde):**

1. `dotnet test` — **0 başarısız**; iki bacakta da çalışır ve test sayıları eşittir
2. NetArchTest — yön + circular **0 ihlal**; matris ADR-028 §2.3 ile **aynı kaynaktan** gelir
3. [layer-boundary-check](../checklists/layer-boundary-check.md) 13 sorusu — **0 yanlış negatif**
4. Coverage hedefi [spec/test-strategy](../spec/test-strategy.md) bandıdır; altındaysa **yazılı gerekçe** zorunlu
5. Atlama (`Skip` / `Assert.Ignore`) **0**; atlanan bir test varsa gerekçesi `EKSIK` satırında yazılıdır

**Kanıt kutusu — dolu örnek:**

```text
KANIT   : dotnet test -> iki bacakta da Gecen=N, Basarisiz=0, Sure=X sn
          NetArchTest -> 0 failure (yon + circular) | kapsam -> <olculen yuzde>
          layer-boundary-check -> 13 soru, 0 yanlis negatif
DOSYA   : tests/**, spec/test-strategy.md, reports/
EKSIK   : YOK (atlanan test yok) ya da <atlanan test + gerekce>
SONRAKI : teslim raporunu yaz (GATE-006, ~40 dk)
```

**Kırmızı döndüren nedenler:**

| # | Neden | Belirti | Sonucu |
|---|---|---|---|
| 1 | Atlanan test var | `Skip` ya da `Assert.Ignore` satırı bulunur | RED · test geri açılır ya da gerekçe yazılır |
| 2 | NetArchTest ihlal ediyor | Mimari testte 0'dan fazla failure | Mimari RED · katman koduna dönülür |
| 3 | Test yalnız bir bacakta çalışıyor | Diğer TFM'de test sayısı 0 ya da farklı | G6 · gate açık |
| 4 | Coverage bandın altında, gerekçesiz | Yüzde rakamı yok ya da gerekce maddesi yazılmamış | Kanıtsız coverage (G5) · gate açık |
| 5 | Test altyapısında EF Core referansı | Test projesinde EF paketi ya da `using` | **G2 CRITICAL** · test altyapısı yeniden yazılır |
| 6 | Yasak tarama temiz değil | EF · v26.1 dışı · string-concat satırı var | **G2/G3** · teslim yolunun önü kapanır |

**Süre tahmini:** **≈120 dk** — kabul kriteri eşlemesi ≈10 dk · birim ≈40 dk · entegrasyon ≈35 dk · mimari ≈15 dk · hata yolu ≈15 dk · coverage ≈5 dk (kalemler §2.4 tablosunda; toplam 120). Bütçe aşımı, §2 girişindeki **20 dk** eşiğine göre değerlendirilir.

**Kim onaylar:** Test üretimi `tester` · PASS/RED kararı `reviewer` · **kapatan kullanıcı**. `tester` kendi üretimini yeşil ilan edip gate'i kapayamaz.

### §2.5 GATE-006 — Ship

| Alan | Değer |
|---|---|
| **Sahip** | Derleme + rapor: inline oturum · Kapatan: **kullanıcı** |
| **Kabul kriteri** | 4 kanıt satırı da dosyada: **build 0 hata · test yeşil · vault 0 bozuk link + 0 mojibake · yasaklı tarama 0** (EF Core · v26.1 dışı · string-concat) · teslim raporu yazıldı · `log.md` kapanış kaydı var |
| **Kanıt kutusu** | `KANIT: 4 satir (build/test/vault/tarama) + CIKTI EKI` · `DOSYA: <teslim raporu>` · `EKSIK: <YOK ya da DOĞRULAMA GEREKLI>` · `SONRAKI: <kullanıcı onayı>` |
| **Süre tahmini** | **≈40 dk** |
| **Red nedenleri** | Kanıtsız "bitti" (G5) · tarama temiz değil · onaysız teslim |

**Ne test eder:** İşin dört bağımsız kanıtının **aynı anda** dosyada olup olmadığını: build · test · vault · yasak tarama. Bu gate tek başına "kod bitti" demez; dört satırın dördü de varsa teslim konuşulur.

**Kabul kriterleri (madde madde):**

1. Dört kanıt satırı da dosyada: **build 0 hata · test yeşil · vault 0 bozuk link + 0 mojibake · yasak tarama 0**
2. Yasak tarama üç kalemi de temiz: EF Core · DevExpress v26.1 dışı · string-concat SQL
3. Teslim raporu yazıldı ve `CIKTI EKI` bölümüne konsol alıntısı yapıştırıldı
4. `log.md`'de tarihli kapanış kaydı var (gate adı + sayılar)
5. Kullanıcının teslim onayı alınmadan iş "bitti" sayılmaz

**Kanıt kutusu — dolu örnek:**

```text
KANIT   : build -> 0 hata (2 bacak) | test -> Gecen=N, Basarisiz=0
          vault -> BOZUK=0, MOJIBAKE=0 | tarama -> EF 0, v26.1 disi 0, concat 0
DOSYA   : reports/teslim-raporu.md, log.md
EKSIK   : YOK ya da DOĞRULAMA GEREKLI maddesi
SONRAKI : kullanici onayi -> teslim
```

**Kırmızı döndüren nedenler:**

| # | Neden | Belirti | Sonucu |
|---|---|---|---|
| 1 | Kanıtsız "bitti" | Dört satırdan biri eksik ya da çıktısı yok | G5 · gate yeniden açılır |
| 2 | Yasak tarama kirli | EF · v26.1 dışı · string-concat satırı > 0 | **G2/G3 CRITICAL** · teslim durur |
| 3 | Teslim raporu yok | `reports/` altında kapanış dosyası yok | Rapor yazılır, gate açık kalır |
| 4 | `log.md` kaydı yok | Tarihli kapanış satırı yok | Kayıt düşülür; yoksa ölçüm tekrarlanır |
| 5 | Onaysız teslim | Kullanıcı onayı yazılmamış | İş tamamlanmış sayılmaz, geri döner |

**Süre tahmini:** **≈40 dk**; kalemler dört kanıtın yeniden koşulması, rapor yazımı ve `log.md` kaydıdır. Kalem ayrımı `DOĞRULAMA GEREKLI`; aşım **20 dk** üzerindeyse iş durur.

**Kim onaylar:** Derleme + rapor inline oturum · **kapatan kullanıcı**. Rapor "bitti" demez, **kanıtı** sunar; karar kullanıcıdadır.

### §2.6 Gate özeti tablosu

| Gate | Ne kapatır | Sahip (üretim) | Kapatan | Süre | Kontrol listesi (soru) |
|---|---|---|---|---|---|
| GATE-004a | Vault 500 MD · 0 bozuk link · 0 mojibake | `architect` | kullanıcı | ≈45 dk | [gate-004a-vault-kapanis](../checklists/gate-004a-vault-kapanis.md) · 14 |
| **GATE-003** | 7 wireframe + kullanıcı onayı | `ui-designer` | **yalnız kullanıcı** | ≈60 dk + onay | [gate-003-ui-onay](../checklists/gate-003-ui-onay.md) · 13 |
| GATE-004 | İskelet + 2 bacak build + ilk commit | `architect` + `developer` | kullanıcı | ≈90 dk | [gate-004-ilk-commit](../checklists/gate-004-ilk-commit.md) · 13 |
| GATE-005 | Test + NetArchTest 0 ihlal | `tester` | kullanıcı | ≈120 dk | [gate-005-test-kapsami](../checklists/gate-005-test-kapsami.md) · 14 |
| GATE-006 | Teslim | inline | kullanıcı | ≈40 dk | — (bu bandda tanımlı liste yok) |
| — | **Toplam** | — | — | **≈5 sa 55 dk** | 4 liste · 54 soru |

**Tablo okuması:** süreler ayrıntı alt bölümlerdeki tahminlerin toplamıdır; her
satır kendi gate bloğundaki `Süre tahmini` ile birlikte okunur ve toplam **≈5 sa 55 dk**
(355 dk) kapı bütçesini verir. **Kontrol listesi sütunu** gate'in adım adım soru
listesinin tek indeksidir; aynı bağlantı ilgili gate bloğunun içinde "N adım"
cümlesiyle de geçer — çelişki olursa checklist dosyası kazanır (SSOT).

### §2.7 Gate kanıt kutusu ile ajan çıktı kutusunun farkı

Bu iki kutu **karıştırılamaz**; biri diğerinin yerine kullanılamaz (AGENTS §2). Karışan yerde kural basittir: gate kapatan kutu `KANIT/DOSYA/EKSIK/SONRAKI`, ajan dönen kutu `BAŞLIK/KAPSAM/KARAR/RİSK/KANIT`.

| Ayırım | Gate kanıt kutusu (4 satır) | Ajan çıktı kutusu (5 alan) |
|---|---|---|
| Sorusu | "Bu kapı kapanabilir mi?" | "Bu ajan ne yaptı, güvenilir mi?" |
| Alanlar | `KANIT` · `DOSYA` · `EKSIK` · `SONRAKI` | `BAŞLIK` · `KAPSAM` · `KARAR` · `RİSK` · `KANIT` |
| Sahibi | [WORKFLOW.md](workflow.md) §2 (bu dosya) | [AGENTS.md](../definitions/agents.md) §2 |
| Ne zaman yazılır | Gate **kapanışında** tek sefer | Her ajan çıktısının **sonunda** zorunlu |
| Kim doldurur | Kanıt toplayan rol, **kapatır kullanıcı** | Üretimi yapan ajan |
| Kapsamı | Tek gate, tek kapı | Tek iş birimi (dosya, diff, bulgu) |
| `SONRAKI` | Vardır — kapıdan sonraki adım | Yoktur — adım zinciri devre paketindedir |
| Karıştırılırsa | Ajan kutusuyla gate kapatılmaya kalkılır | Ajan, gate gibi sahte onay üretir → G5/G12 ihlali |

---

## §3 Paralel Geliştirme Akışı

### §3.1 Üç blok (ADR-028 §4 — bağlayıcı)

```text
  BLOK A - SERI            BLOK B - PARALEL           BLOK C
  ----------------          ----------------           ----------------
   (0) l0 Abstractions             |
         |                         |   +-- l3 Crosscutting
         v                         |   |
   (1) l1 Domain                   |   +-- l4 Infrastructure
         |                         |   |
         v                         |   +-- l5 Protocol
   (2) l2 Application              |
         |                         |
         +-------- A bitti --------+-------- B bitti --------+
                                            |
                                            v
                                   (6) l6 Host
                                            |
                              +-------------+-------------+
                              v                           v
                        (7a) l7 Desktop            (7b) l7 Cli
```

### §3.2 Blok kuralları

| Blok | Kural | Neden |
|---|---|---|
| **A (l0 → l1 → l2)** | **Seri** — her katman bir öncekinin sözleşmesine bağımlı | Sözleşme yoksa kod tahminle yazılır (G9) |
| **B (l3 \|\| l4 \|\| l5)** | **Paralel** — üçü birbirine **doğrudan bağlanmaz**; ortak girdi ADR-028 + katman sözleşmesidir | `l4 -> l2 X`, `l5 -> l4 X` doğrudan ok yasağı |
| **C (l6 → l7)** | l6 **tek başına** yazılır (composition root); sonra **Desktop \|\| Cli paralel** | DI kaydı iki kez değişmesin |
| Tümü | Aynı `.cs` dosyasına **eş zamanlı iki yazıcı yok** (dosya bazlı yetki) | Çakışma kesindir |
| Tümü | Paralel blokta ajan sayısı **en fazla 3** ([AGENTS.md](../definitions/agents.md) §5) | Rate limit |

### §3.3 Bağımlılık yönü (tek çizgi)

| Bağımlılık | Durum |
|---|---|
| `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` | İzinli sıra (ADR-028 §2.1) |
| `l3 -> yalnız l0` | İzinli tek yön |
| `l4 -> l2` doğrudan ok | **YOK (X)** — repository portu `l0`'da, uygulaması `l4`'te |
| `l5 -> l4` doğrudan ok | **YOK (X)** |
| `l2` ile `l4` arası | **Doğrudan ok YOK** |
| İçeriye bağımlılık | Yalnız `interface` üzerinden |

> Tam 8x8 matris ve denetimi: ADR-028 §2.3 madde 2 + [layer-boundary-check](../checklists/layer-boundary-check.md). Matris **NetArchTest `LayerRulesTests`** ile aynı kaynaktan gelir; elle çizilen matris kanıt değildir.

### §3.4 Zamandasır (timeline) tablosu — ADR-028 §4

Blok sırası **zaman ekseni** gibi okunur: soldan sağa ilerler, aynı satırdakiler aynı anda yazılabilir, satır atlanamaz.

| Sıra | Blok | Katman | Eşzamanlılık | Bu adımda ne biter | Kapı |
|---|---|---|---|---|---|
| 1 | A | `l0` Abstractions | **Seri** — tek yazıcı | Portlar ve sözleşmeler yazılır | GATE-004 iskeleti |
| 2 | A | `l1` Domain | **Seri** — `l0` bittikten | İş kuralları ve değer objeleri | GATE-004 |
| 3 | A | `l2` Application | **Seri** — `l1` bittikten | Kullanım senaryoları uçtan uca bağlanır | GATE-004 |
| 4 | B | `l3` Crosscutting | **Paralel** — `l3 \|\| l4 \|\| l5` | Ortak kesitler `l0`'a bakarak yazılır | GATE-005 |
| 5 | B | `l4` Infrastructure | **Paralel** — aynı satır | Dapper erişimi, dosya, git, ağ | GATE-005 |
| 6 | B | `l5` Protocol | **Paralel** — aynı satır | Akış ve tool sözleşmeleri | GATE-005 |
| 7 | C | `l6` Host | **Seri** — blok B kapandıktan | Composition root tek yerde toplanır | GATE-005 |
| 8 | C | `l7` Desktop | **Paralel** — `l7 Desktop \|\| Cli` | Ribbon'lı masaüstü kabuğu | GATE-006 |
| 9 | C | `l7` Cli | **Paralel** — aynı satır | Komut satırı yüzeyi | GATE-006 |

```text
  satir 1-3 : l0 --> l1 --> l2            (SERI, tek yazici)
                    |
                    v
  satir 4-6 : l3  ||  l4  ||  l5          (PARALEL, en fazla 3 ajan)
                    |
                    v
  satir 7   : l6                           (SERI, composition root)
                    |
                    v
  satir 8-9 : l7 Desktop  ||  l7 Cli       (PARALEL, ayni dosyaya iki yazici yok)
```

**Zaman kuralı (5 madde):**

1. Satır 4-6'ya **yalnız** 1-3 kapandıktan sonra girilir; `l4`, `l2`'ye doğrudan bakamaz
2. Aynı satırda ajan sayısı en fazla **3** ([AGENTS.md](../definitions/agents.md) §5) — rate limit budur
3. Aynı `.cs` dosyasına eş zamanlı **iki yazıcı yok**; çakışan iş seriley alınır
4. `l6` yazılmadan `l7` başlamaz; DI kaydı iki kez değişmesin diye
5. Blok içinde bir gate RED olursa **o blok** döner, önceki bloklar yapılmış sayılır

---

### §3.5 Kod yazım döngüsü (START → END, 12 adım)

Tek dosyada değişiklik yaparken izlenen **tek** döngü. Sıra zorunludur; atlanan
adımda kanıt eksik kalır (G5).

```text
START
  |
  v
1  Kok MD OKU : CLAUDE.md, AGENTS.md, README.md, WORKFLOW.md
   && .ai/CLAUDE.md, .ai/AGENTS.md, .ai/WORKFLOW.md, .ai/index.md
  |   (1 KEZ oku - CLAUDE.md S0; "3 kez tekrar oku" tasnifi GECERSIZ)
  v
2  .ai/.rules/index.md + goreve ESLESEN kural dosyalari OKU (17 icinden hepsi degil)
  |
  v
3  Ilgili .ai/*** & architecture/*** OKU   (navigasyon: .ai/index.md S3)
  |
  v
4  Mevcut dosyayi OKU   (degistirmeden once - "mevcut davranisi anlamadan refactor yapma")
  |
  v
5  Kodu YAZ   [!] GATE-003 onayindan ONCE src/** altina tek satir .cs bile YAZMAZ (G12)
  |
  v
6  .ai/.rules/** CALISTIR   ->  /rules [dosya]
  |
  +---- HATA -> 7  .ai/.rules/error-handling-rules.md
  |                 |      (NOT: "error-recovery.md" DOSYASI YOKTUR)
  |                 v
  |              8  Dosyayi DUZELT
  |                 |      (SIL + yeniden yaz = YIKICI, G7 onayi ister)
  |                 v
  |              6'ya DON   (3 kez duserse 3-STRIKE -> .ai/AGENTS.md S4)
  |
  +---- TEMIZ -> 9  UI degisikligi var mi?
                   |
                   +-- EVET -> 10  .ai/.rules/ui-rules.md
                   |              + devexpress-rules.md
                   |                 |
                   |                 v
                   |              11  UI KONTROL
                   |                  .ai/ui-design/01..07 ile karsilastir
                   |                  NOT: "Browser test" YOK - WinForms (G3/G8)
                   |                 |
                   |                 +-- HATA  -> 7'ye DON
                   |                 +-- TEMIZ -> 12
                   |
                   +-- HAYIR -> 12
  |
  v
12 COMMIT      dosya bazli `git add`; `git add -A` YASAK (AGENTS S4)
  |
  v
END
```

**Düzeltmeler (özgün şablona göre — 5 madde):**

| # | Özgün şablonda | Doğrusu | Kaynak |
|---|---|---|---|
| 1 | adım 7 `.ai/.rules/error-recovery.md` | **`error-handling-rules.md`** | 17 kural listesinde `error-recovery.md` **yok** (`Test-Path = False`) |
| 2 | adım 11 `Browser test` | **UI kontrol** (wireframe karşılaştırması) | G3/G8 — tarayıcı yok, WinForms |
| 3 | adım 1 root md **3×** oku | **1×** oku | `CLAUDE.md` §0 |
| 4 | adım 8 `SIL + yeniden yaz` | G7 onaylı — yıkıcı işlem | `CLAUDE.md` G7 |
| 5 | adım 12 `COMMIT` | dosya bazlı `add`; `git add -A` **yasak** | `AGENTS.md` §4 |

---

## §4 Dur / Bekle ve Escalation

| Tetikleyici | Etiket | Yanıt |
|---|---|---|
| GATE-003 kapalı, kod isteniyor | `GATE-OPEN` | Kod yazılmaz; onay istemi gönderilir |
| Aynı hata 3 düzeltmede çözülmedi | `3-STRIKE` | Durdur + şüpheli varsayımı adlandır + A/B/C ([AGENTS.md](../definitions/agents.md) §4) |
| Yıkıcı işlem | `G7 ONAYI` | Onay gelmeden komut çalışmaz |
| Yeni paket / sürüm / ORM | `G8` | Kullanıcı onayı; "geçici" eklenemez |
| Kaynak çelişti | `CONFLICT DETECTED` → `DECISION BLOCKED` | Hiyerarşi denenir, çözülmezse tek soru ([ROLE.md](../definitions/role.md) §3) |
| Kanıt yok | `UNKNOWN` / `DOĞRULAMA GEREKLI` | Madde yazılmaz, iş durur |

### §4.1 Hata modları tablosu (belirti -> müdahale)

Sekiz klasik hata modu; her satırda **önce belirti**, sonra tek elden müdahale yazılıdır. Müdahale bitene kadar iş `dur` sayılır.

| # | Hata modu | Belirti | İlk müdahale | Etiket / kapı etkisi |
|---|---|---|---|---|
| 1 | Rate limit (paralel ajan taşması) | Ajan yanıtları kesiliyor, iş sıraya giriyor | Paralel ajan sayısını **3'ün altına** indir; bloğu serile al ([AGENTS.md](../definitions/agents.md) §5) | Kapı etkisi yok; süre bütçesi aşarsa raporlanır |
| 2 | Model unavailable / ajan düşüşü | İş yarım çıktı, kutu eksik | Aynı işi **tek ajana** devret; yarım çıktıyı kanıt sayma | Kanıtsız çıktı işlenmez (G5) |
| 3 | Bozuk link | `check-links.ps1` 0'dan büyük sayı veriyor | Hedefi düzelt ya da linki kaldır; sonra taramayı yeniden koş | **G4** · GATE-004a kapalı kalır |
| 4 | NU1008 (paket sürümü tanımsız) | `dotnet restore` paket adını çözemedi | Test paketlerinin `PackageVersion` satırlarını ekle; restore tekrar | GATE-004 kapalı; commit atılmaz |
| 5 | NetArchTest ihlali | Mimari testte yön ya da circular failure | Kodu katman sözleşmesine çek; **matrisi elle düzeltme** | Mimari RED · ADR-028 §2.3'e dön |
| 6 | Mojibake | İki bytelik ASCII artıkları (`\xC3\xBC` benzeri) | Dosyayı UTF-8 ile yeniden yaz; kaynağı kopyalama (G1) | **G4** · GATE-004a kapalı kalır |
| 7 | `NoWarn` ile gizlenen hata | Derleme "0 hata" ama uyarı satırları paketlenmiş | `NoWarn` satırını sil, hatayı geri aç | GATE-004 RED · G6 |
| 8 | Aynı dosyaya iki yazıcı | `git status` ya da diff'te beklenmedik çakışma | Bloğu serile al; kanıtı olan kazanır, kanıt yoksa [AGENTS.md](../definitions/agents.md) §3 devri | Blok durur · 3-STRIKE sayılabilir |

> Müdahale sonunda **etiket** ve **kapı etkisi** birlikte raporlanır; yalnız "çözüldü" yazmak kanıt değildir (CLAUDE G5).

> Bloke raporunun biçimi (`ETIKET · KONUM · NEDEN · VARSAYIM · SECENEKLER · SORU`) [AGENTS.md](../definitions/agents.md) §4 şablonundadır; burada tekrarlanmaz (SSOT).

---

## §5 Rapor Formatı

Gate raporunun biçimi (ilk satır · ≤ 5 madde · somut süre · kanıt çıktısı · tek sonraki adım) **SSOT olarak [ROLE.md](../definitions/role.md) §2'dedir**; bu dosyada tekrarlanmaz. Kapı kapanışında eklenen tek alan **kanıt kutusudur**: `KANIT / DOSYA / EKSIK / SONRAKI` (§2).

### §5.1 Kapı kapanışının 5 adımı

| # | Adım | Ne yapılır | Çıktı |
|---|---|---|---|
| 1 | Ölçümü koş | İlgili gate'in script'lerini çalıştır (ölçüm · link · mojibake · derleme · test) | Konsol çıktısı |
| 2 | Çıktıyı kutuya yapıştır | Dört satırlık kanıt kutusunu **sayılarla** doldur; tahmin yazma | Dolu `KANIT/DOSYA/EKSIK/SONRAKI` |
| 3 | Kontrol listesini işaretle | İlgili gate kontrol listesindeki maddeleri tek tek `[ ]` → `[x]` yap | İşaretli checklist + `Kanit:` satırları |
| 4 | Onay iste | Kullanıcıya tek, seçenekli, süreli onay istemi gönder | Yanıt (onay / değişiklik / liste değişikliği) |
| 5 | Kaydı düş | `log.md`'ye tarih, gate adı ve sayılarla kapanış satırı yaz | Tarihli kayıt |

> Adım 4 **zorunlu ara duraktır**: onay gelmeden 5'e geçilmez. Adımlar atlanırsa gate kapalı sayılır ve iş §1.2 kural 1'e göre döner.

### §5.2 RED kanıt kutusu örneği (nasıl olmaz)

```text
KANIT   : testler geciyor, kod temiz
DOSYA   : birkac dosya
EKSIK   : yok
SONRAKI : devam ederiz
```

Yukarıdaki kutu **RED**'dir: sayı yok, dosya yolu yok, süre yok, somut adım yok (CLAUDE G5); iş bitmemiş sayılır ve kapı yeniden açılır. Aşağıdaki ikinci kutu **kıyas bloğudur**: aynı iddianın sayıya nasıl çevrildiğini gösterir; kapı kapanışında kullanılacak dolu kutular ilgili gate'in §2.1-§2.5 bölümündedir ve burada **tekrarlanmaz**.

```text
KANIT   : dotnet test -> Gecen=12, Basarisiz=1, Sure=4,8 sn (net10.0-windows)
          NetArchTest -> 0 failure | link taramasi -> 1 BOZUK (.ai/index.md -> reports/99-yok.md)
DOSYA   : .ai/reports/, .ai/index.md
EKSIK   : bozuk linkin hedefi DOĞRULAMA GEREKLI - dosya olusturulmeli
SONRAKI : hedefi olustur ya da linki kaldir, sonra olcumu tekrar kos (~5 dk)
```

> Bu kıyas bloğu öğretim amaçlıdır; içindeki sayılar **kurgudur** ve gerçek ölçümle değiştirilir. Kapı kapanışında geçerli olan kutu, ilgili gate'in §2.x bölümündeki biçimdir; ayrıca bir yeşil kutu burada geçersizdir (SSOT).

---

## §6 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-10-07`
- [ ] 5 gate'in **hepsinde** Sahip · Kabul kriteri · Kanıt kutusu · Süre tahmini var
- [ ] §0.1 6 aşamalı yaşam döngüsü gate'lerle eşleştirilmiş (prompt kaynağı: ai.05)
- [ ] GATE-003 `HARD STOP` + "onaysız tek satır `.cs` yasak" açıkça yazılı
- [ ] GATE-004'te **ilk git commit** maddesi var
- [ ] §3 paralel akış `A seri -> B paralel -> C` biçiminde ve saf ASCII (Unicode kutu yok)
- [ ] Süreler toplamı tutarlı: 45 + 60 + 90 + 120 + 40 = **355 dk ≈ 5 sa 55 dk**
- [ ] Sayılar yalnız kaynaklı (ADR-028 · eski vault bütçeleri); kaynaksız yer `DOĞRULAMA GEREKLI`
- [ ] Tüm markdown linkleri izinli hedeflere

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Katman/proje/blok kararı (bağlayıcı) | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 2 | Anayasa (G1..G12) · rol ve karar hiyerarşisi | `CLAUDE.md` · `ROLE.md` |
| 3 | Ajan dağıtım matrisi + 3-STRIKE | `AGENTS.md` |
| 4 | Katman sınırı 13 sorusu · katman haritası | `checklists/layer-boundary-check.md` · `architecture/index.md` |
| 5 | Kapanış kayıtları · navigasyon | `log.md` · `index.md` |
| 6 | Prompt kaynağı (yaşam döngüsü, wizard) | `G:/…/versacoder proje planlaması ai.05.md` · `ai.06.md` |
| 7 | Prompt kaynağı (21 adımlık ön plan) | `G:/…/versacoder proje planlaması ai.08.md` §60 |
| engine | [engine](../definitions/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| brain | [brain](../.decisions/brain.md) | ADR ozetleri (29) + SOLID kisitlari + 6 sert kod kurali |
| VISION | [VISION](../definitions/vision.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](../definitions/projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](../definitions/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| MEMORY | [MEMORY](../memory/memory.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| ULTRA | [ULTRA](../definitions/ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
