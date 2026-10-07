# VERSA CODER — PROJECT DEFINITION ANALYSIS PROMPT

####################################################################################
#                                                                                  #
#                 Versa Coder AI My Coding Assistant Agentic IDE                   #
#          Visual Studio IDE / Xcode / Cursor AI IDE / Antigravity AI IDE          #
#                                                                                  #
#                              VERSION 1.0.0                                       #
#                                                                                  #
#                    Designed & Coding by Bayram Ali Akşit                         #
#                                                                                  #
#                           Agent Chat Wizard System                               #
#                                                                                  #
####################################################################################

### 1. Wizard Tetikleme ve Niyet Ayrıştırma Mekanizması (Intent-Triggered Activation)

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│                             CHAT AGENT GİRİŞ ALANI                               │
│  "Bu modüldeki tüm SQL sorgularını EF Core 8'e geçir ve xUnit testlerini yaz"    │
└──────────────────────────────────────────────────────────────────────────────────┘
                                        │
                           [ "GÖNDER" BUTONUNA BASILDI ]
                                        │
                                        ▼
┌──────────────────────────────────────────────────────────────────────────────────┐
│                    NİYET VE RİSK ANALİZ MOTORU (Intent Engine)                   │
│  - Görev Kapsamı: Refactoring + ORM Migration + Unit Test                        │
│  - Tespit Edilen Roller: Architect, Developer, Testing, Security                 │
│  - Önerilen MCP/Araçlar: Roslyn AST, EF Migration CLI, xUnit Runner             │
└──────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌──────────────────────────────────────────────────────────────────────────────────┐
│               SESSION AGENTIC AGENT WIZARD (DevExpress UI Açılır)                │
└──────────────────────────────────────────────────────────────────────────────────┘

Kullanıcı komutu gönderdiğinde arka plandaki `IntentAnalysisEngine` talebi ayrıştırır. Görevin katman sınırlarını, risk seviyesini, ihtiyaç duyulan uzmanlıkları hesaplar ve **Session Agentic Agent Wizard** ekranını bu ön analiz verileriyle önceden doldurulmuş (pre-populated) olarak kullanıcının onayına açar.

---

### 2. Session Agentic Agent Wizard Adım Adım Yapılandırma Mimarisi

Wizard, kullanıcıya karmaşık ayarları basitleştiren ve her aşamayı tam kontrolde tutan **DevExpress Wizard Control / Tabbed View** tabanlı 6 adımlı bir konfigürasyon akışı sunar:

#### Adım 1: Chat Session Yapılandırması (Oturum Tipi ve Bağlam Ayarları)
* **Oturum Tipi Seçimi:**
  - *Single Agent Session:* Hızlı analiz ve tekil dosya düzenlemeleri.
  - *Multi-Agent Swarm Session:* Paralel çalışan mimar, kodlayıcı, testçi ve denetleyici ajanlar.
  - *Chained Pipeline Session:* Sıralı onay ve aşamalı geçiş gerektiren refactoring akışları.
  - *Distributed Node Session:* Farklı sunuculardaki MCP veya arka plan servislerine dağıtılmış görevler.
* **Bağlam Bellek Stratejisi (Epoch Memory):** Oturuma projenin hangi kısımlarının (Tüm AST, sadece ilgili DLL'ler, veritabanı şeması, `.ai` kural dosyaları) dahil edileceğinin seçimi.
* **Otonomluk ve Onay Seviyesi:** Tam Otonom (Full Auto), Yarı Otonom (Human-in-the-Loop Approval Gates) veya Yalnızca İnceleme (Review Only).

#### Adım 2: Agent Session Yapılandırması (Rol ve Model Eşleştirme)
* **Ajan Takımı Seçimi:** Görevde yer alacak uzman rollerin (Architect, Developer, Reviewer, Security, Testing, Sektörel Uzman) aktif/pasif edilmesi.
* **Model Sağlayıcı Eşleştirmesi:** Her ajan için en uygun LLM modelinin seçilmesi (Örn: Mimar için *Claude 3.5 Sonnet / Reasoning Modeli*, Hızlı kodlama için *GPT-4o / DeepSeek-Coder*, Güvenlik için *Yerel LLM*).
* **Hiperparametre Ayarları:** Sıcaklık (Temperature), Top-P, Maksimum Token ve Düşünme Bütçesi (Thinking Budget) değerlerinin role özel kalibrasyonu.

#### Adım 3: Araç (Tool) ve MCP İzin Matrisi (Capability & Security Gates)
* **MCP Servis İzinleri:** Hangi ajanların hangi MCP sunucularına (Dosya sistemi, Roslyn AST, SQL Server, Git CLI, Web Search) erişebileceğinin belirlenmesi.
* **Yıkıcı İşlem Koruması:** Dosya silme, veritabanı şeması değiştirme veya terminal komutu çalıştırma işlemlerinde zorunlu kullanıcı onay kutularının atanması.

#### Adım 4: Sektörel Standartlar ve Mimari Kural Enjeksiyonu
* Projeye özel regülasyonların seçimi (Otomotiv için *MISRA/AUTOSAR*, Sağlık için *HL7/FHIR*, Web/API için *OWASP Top 10:2025*, Finans için *ISO 20022*).
* Test kapsam şartı (Örn: *"%90 üzeri code coverage sağlanmadan oturumu tamamlama"* kuralının zorunlu kılınması).

---

### 3. "Bitti" (Complete) Butonu ve Dinamik System Prompt Derleyicisi (Prompt Synthesis Engine)

Kullanıcı tüm ayarları tamamlayıp **“Bitti”** butonuna bastığında sistem aşağıdaki kritik adımları deterministik olarak icra eder:

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│                         WIZARD "BİTTİ" TETİKLEMESİ                               │
├──────────────────────────────────────────────────────────────────────────────────┤
│  1. [WIZARD CONFIG SPEC] ──> Immutable JSON State olarak serialize edilir.       │
│  2. [PROMPT COMPILER]   ──> Her ajan için amaca özel Meta-System-Prompt üretir. │
│  3. [DAG WORKFLOW]      ──> Ajanlar arası görev grafı (DAG) inşa edilir.         │
│  4. [SESSION REGISTRY]  ──> SQLite oturum tabanına ve Audit loguna kaydedilir.   │
│  5. [RUNTIME LAUNCH]    ──> Agent Swarm Orchestrator yürütmeyi başlatır.         │
└──────────────────────────────────────────────────────────────────────────────────┘
```

#### Dinamik Meta-System-Prompt Sentezi Nasıl Çalışır?
System Prompt derleyicisi, genel ve belirsiz metinler yerine sihirbazda seçilen parametrelerden **katı, matematiksel ve çelişkisiz direktifler** oluşturur:
- *"Sen bu oturumda **Architect Agent** rolündesin. Yalnızca `VersaCoder.Domain` ve `VersaCoder.Abstractions` katmanlarının kontratlarını tasarlayacaksın."*
- *"Sana tanımlanan model kısıtı: EF Core 8 Fluent API kullanılacak, ham SQL kesinlikle yazılmayacak."*
- *"Çıktın doğrudan **Developer Agent**'ın girdisi olacaktır; kod üretmeden önce interface kontratlarını ve DTO yapılarını doğrula."*
- *"Güvenlik kuralı: OWASP 2025 standartlarında input sanitization uygulamayan kodları **Reviewer Agent** otomatik reddedecektir."*

---

### 4. Deterministik Yürütme ve Gözlemlenebilirlik (Execution & Observability)

Wizard kapandığı anda kullanıcı arka planda kaybolan bir süreçle baş başa kalmaz:

1. **Görsel İş Akış Ekranı (Visual Execution DAG):** Ajanların birbirine veri aktarımı, hangi ajanın o an hangi dosyayı işlediği gerçek zamanlı görsel nodlar üzerinde parıldayarak (active glow) gösterilir.
2. **Canlı Telemetri ve Token Sayacı:** Kullanılan modellerin anlık token tüketimi, gecikme süreleri (latency) ve maliyet hesabı anlık güncellenir.
3. **Kendi Kendini Onarma (Self-Healing Logu):** Derleme veya test hatası oluştuğunda sihirbazda belirlenen `Fallback/Correction` politikasına göre ilgili ajanın hatayı nasıl düzelttiği adım adım loglanır.

Bu mimari sayesinde **Session Agentic Agent Wizard**, kullanıcı niyetini en yüksek mühendislik disiplinine bağlayan, yapay zekânın rastgele kararlar almasını engelleyen ve geliştiriciye tam kurumsal kontrol sağlayan ana orkestrasyon merkezidir.


# Agent Inc. — Wizard / Agent Workflow Yönetim Sistemi (Deterministik ve Görsel Yönlendirme Mimarisi)

**Agent Inc. — Wizard & Workflow Yönetim Sistemi**, yapay zekânın yazılım geliştirme sürecindeki kontrolsüz kararlarını, bağlam kopmalarını ve halüsinasyon (saçmalama) risklerini **deterministik mühendislik kuralları**, **3 seviyeli yapılandırma sihirbazı** ve **hibrit görsel akış diyagramları (Visual Workflows / Use-Flows)** ile sıfırlayan kurumsal bir yönetim mimarisidir.

Bu mimarinin temel amacı; yapay zekâyı kendi başına rastlantısal kod üreten bir "kara kutu" olmaktan çıkarıp, yazılımcının çizdiği veya sistemin modellediği **kesin akış sınırları (Deterministic Guardrails & State Machines)** içinde çalışan, denetlenebilir bir mühendislik icra motoruna dönüştürmektir.

---

### 1. Üç Kademeli (3-Tier) Wizard ve Konfigürasyon Hiyerarşisi

Sistem, yapay zekânın davranışlarını tek bir prompt alanından yönetmek yerine birbirini ezmeyen, hiyerarşik 3 ana katmanda yapılandırır:

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│ 1. GLOBAL PROGRAM SETTINGS WIZARD (IDE & Ortam Düzeyi)                          │
│    - Global Model Sağlayıcıları (Claude, OpenAI, Local LLM)                     │
│    - Global MCP Server ve Araç Gateway Kayıtları                                │
│    - Genel Güvenlik Politikaları (AES-256 Kasa, Yetkilendirme Standartları)     │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼ (Override & Spesifikasyon)
┌─────────────────────────────────────────────────────────────────────────────────┐
│ 2. PROJECT SETTINGS & ARCHITECTURE WIZARD (Proje ve Mimari Düzeyi)             │
│    - Proje Türü (net48+net10, DevExpress WinForms, Embedded C/C++, Web API)     │
│    - Katman Hiyerarşisi ve DLL Kontratları (Clean Architecture Sınırları)       │
│    - Sektörel Standartlar (MISRA, OWASP 2025, ISO 20022, HL7)                   │
│    - Projeye Özel Görsel İş Akışları (Visual Workflows & Use-Flows)             │
│    - .ai Bilgi Kasası (Knowledge Vault SSOT) Yapılandırması                     │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼ (Göreve Özel İcra)
┌─────────────────────────────────────────────────────────────────────────────────┐
│ 3. CHAT / SESSION AGENTIC WIZARD (Oturum ve Görev Düzeyi)                       │
│    - Anlık Görev Hedefi ve Risk Seviyesi (Task Intent & Decomposition)          │
│    - Görevlendirilen Ajan Takımı (Architect, Developer, Reviewer, Tester)       │
│    - Anlık Araç & İzin İcrası (Tool Permissions & Approval Gates)                │
│    - Dinamik Meta-System-Prompt Sentezi ve İmmutable Session State              │
└─────────────────────────────────────────────────────────────────────────────────┘
```

1. **Global Program Settings Wizard (Sistem Düzeyi):**
   Geliştirme ortamının temel kurallarını belirler. Tüm projelerde geçerli LLM API anahtarları, global MCP sunucu havuzu, telemetri/loglama tercihleri (Serilog, OpenTelemetry) ve genel sandbox çalıştırma kısıtları bu sihirbaz ile yönetilir.

2. **Project Settings & Architecture Wizard (Proje ve Mimari Düzeyi):**
   Açık olan projenin mimari DNA'sını tanımlar. Projenin hangi katmanlara sahip olduğu (`Domain`, `Abstractions`, `Application`, `Infrastructure.*`, `UI`), hangi tasarım desenlerinin zorunlu kılındığı (CQRS, Repository, MediatR Pipeline), veri modellerinin durum geçişleri ve kodlama standartları burada yapılandırılır. Ajanlar bu kuralların dışına kesinlikle çıkamaz.

3. **Chat / Session Agentic Wizard (Görev Düzeyi):**
   Geliştirici chat ekranından bir komut verdiğinde devreye girer. İşin kapsamını analiz eder, projeye özel kuralları yükler, gerekli ajanları seçer ve kullanıcıya görev akışını onaylatarak `Immutable Session Configuration` üretir.

---

### 2. Çift Yönlü Görsel Akış ve Karar Ağacı Tasarım Motoru (Visual Workflow & Use-Flow Engine)

Projenin tüm kod blokları, dosya ilişkileri, veri akışları ve kullanım senaryoları (Use-Flows) **hem yapay zekâ destekli otomatik olarak hem de geliştirici tarafından elle (Manuel)** çizilebilir ve düzenlenebilir:

* **DevExpress Diagram / Flow Control Entegrasyonu:**
  - Kullanıcı, IDE içerisindeki görsel tasarım ekranında düğümler (Nodes) ve bağlantılar (Edges) ekleyerek iş akışını bir durum makinesi (State Machine) gibi modelleyebilir.
  - Kodlama aşamaları, veri tabanı sorgu akışları, yetkilendirme kontrolleri ve hata senaryoları görsel bloklar halinde bağlanır.
* **Hibrit Çizim ve Senkronizasyon (AI + Human Co-Design):**
  - Geliştirici akışı elle çizebilir; yapay zekâ bu akışa uygun C# kodlarını ve arayüz bileşenlerini üretir.
  - Ya da yapay zekâ karmaşık bir use-case'i analiz edip akış diyagramını görsel olarak çizer; geliştirici diyagram üzerindeki nodları sürükleyip koşulları düzelterek son onayı verir.
* **JSON / DSL / C# State Machine Çıktısı:**
  - Çizilen tüm diyagramlar arka planda standart bir State Machine / Workflow DSL (Domain Specific Language) formatına dönüştürülür ve projenin `.ai/workflows/` dizininde saklanır.

---

### 3. Deterministik Yönlendirme ve Anti-Halüsinasyon (Anti-Saçmalama) Kural Motoru

Yapay zekânın ezbere, hatalı veya projeden bağımsız kod üretmesini (hallucination) engellemek için **Koşullu Yönlendirme ve Kural Sistemi (Conditional Guardrail Engine)** uygulanır:

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                       KOŞULLU KURAL VE YÖNLENDİRME AKIŞI                        │
├─────────────────────────────────────────────────────────────────────────────────┤
│  [DURUM A]  Eğer kullanıcı Veri Tabanı Değişikliği istiyorsa:                   │
│             ──> Zorunlu Kural: Önce Entity'yi güncelle, EF Migration üret,       │
│                 asla ham SQL yazma, Repository kontratını güncelle.             │
│                                                                                 │
│  [DURUM B]  Eğer kod derleme hatası verirse (Build Error):                     │
│             ──> Zorunlu Kural: Chat'e dönme, Roslyn Diagnostic çıktısını al,    │
│                 Self-Healing modülüne gönder, hatayı izole edip düzelt.        │
│                                                                                 │
│  [DURUM C]  Eğer güvenlik riski (OWASP/Credential) algılanırsa:                │
│             ──> Zorunlu Kural: Kod üretimini durdur, Human Approval Gate aç,    │
│                 kullanıcı onaylamadan dosya sistemine yazma.                    │
│                                                                                 │
│  [DURUM D]  Eğer UI bileşeni tasarlanıyorsa:                                    │
│             ──> Zorunlu Kural: Sadece DevExpress 2026 MVVM şablonlarını kullan, │
│                 doğrudan Form code-behind'ına iş mantığı (business logic) yazma.│
└─────────────────────────────────────────────────────────────────────────────────┘
```

Bu yapı sayesinde yapay zekâ promptlarına serbest yorum hakkı bırakılmaz. Her duruma özel **"IF-THIS-THEN-THAT" (Eğer bu olursa bunu uygula)** kural şablonları dinamik olarak ajanların sistem istemine (System Prompt) enjekte edilir ve Roslyn AST analizörleriyle kod seviyesinde doğrulanır.

---

### 4. Proje ve Program Yönetim Tasarım Wizard Arayüzü (Design Wizard UI/UX)

Kullanıcının projeyi ve yapay zekâ orkestrasyonunu parmaklarının ucunda tutabilmesi için DevExpress Universal bileşenleriyle zenginleştirilmiş modern bir **Tasarım Wizard Ekranı** sunulur:

1. **Görsel Akış Tasarımcısı (Visual Flow Canvas):** Durum makinelerinin, veri yollarının ve ajan görevlerinin interaktif olarak bağlandığı ana canvas.
2. **Kural ve Koşul Matrisi (Rule & Guardrail Editor):** Her duruma (State) özel prompt yönlendirmelerinin, sınırlandırmaların ve güvenlik kısıtlarının tanımlandığı kural tablosu.
3. **Ajan Rol & Sektör Yapılandırıcı (Agent Persona & Domain Matrix):** Projede aktif olacak uzman ajanların (Otomotiv, Sağlık, Finans, Embedded, Sürücü vb.) bilgi ve kütüphane seviyelerinin belirlendiği panel.
4. **Gerçek Zamanlı Simülasyon ve Doğrulama (Live Simulation & Dry-Run):** Tanımlanan akışın ve kural setinin çalıştırılmadan önce yapay zekâ tarafından sanal bir görevle test edildiği ve olası sapmaların önceden raporlandığı test alanı.

Bu bütüncül yapı ile Versa Coder; yazılımcının vizyonunu, projenin mimari disiplinini ve yapay zekânın otonom gücünü **tamamen deterministik, hatasız ve yönetilebilir bir mühendislik standardında** birleştirir.

# AGENT DAVRANIŞ KURALLARI

Agent:

- Tahmin yapmaz.
- Bilgi uydurmaz.
- Gereksiz kod yazmaz.
- Kullanıcıdan gizli mimari karar vermez.
- Referans projeyi analiz etmeden implementasyon yapmaz.
- ".ai" bilgisini yok saymaz.
- Katmanları ihlal etmez.
- UI'ya business logic gömmez.
- Domain'i DevExpress'e bağımlı yapmaz.
- Concrete dependency'leri gereksiz yere yaymaz.
- Büyük class oluşturmaz.
- Gereksiz abstraction üretmez.
