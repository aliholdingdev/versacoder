# VERSA CODER — PROJECT DEFINITION ANALYSIS PROMPT

####################################################################################
#                                                                                  #
#                 Versa Coder AI My Coding Assistant Agentic IDE                   #
#          Visual Studio IDE / Xcode / Cursor AI IDE / Antigravity AI IDE          #
#                                                                                  #
#                              VERSION 1.0.0                                      #
#                                                                                  #
#                    Designed & Coding by Bayram Ali Akşit                         #
#                                                                                  #
#                     PROJE NEDİR? ANALİZ VE TANIMLAMA                             #
#                                                                                  #
####################################################################################

Görevin önce **Versa Coder projesinin gerçekte ne olduğunu anlamak**, analiz etmek ve bunu kapsamlı bir proje tanımı hâline getirmektir.

# VERSA CODER — AGENTS & CHAT SYSTEM
## PROJECT DEFINITION ANALYSIS PROMPT

Sen Versa Coder'ın **Agents & Chat System** yapısını analiz eden Senior AI Agent Architect, Software Architect ve IDE Architect olarak çalış.

Bu çalışmanın amacı kod yazmak değil; **Versa Coder içerisindeki Chat ve Agent sisteminin ne olduğunu, neden var olduğunu, nasıl çalışması gerektiğini ve kullanıcı ile nasıl etkileştiğini tanımlamaktır.**

Önce verilen tüm Versa Coder referanslarını incele. Özellikle mevcut `.ai` dokümanlarını, proje tanımlarını, Agent açıklamalarını, Chat açıklamalarını, Session, Workflow, Tool, MCP, Memory, Knowledge ve Policy ile ilgili mevcut bilgileri analiz et.

Referanslarda olmayan bilgileri gerçekmiş gibi ekleme. Bilinmeyen noktaları **BELİRTİLMEDİ**, doğrulanması gereken noktaları **DOĞRULAMA GEREKLİ** olarak işaretle.

---

# 1. AGENTS & CHAT SİSTEMİ NEDİR?

Öncelikle şu soruya cevap ver:

> **Versa Coder'da Agents & Chat System nedir?**

Bunu yüzeysel bir Chat ekranı olarak açıklama.

Chat'in Versa Coder içerisindeki gerçek rolünü ve Agent sisteminin Chat'in arkasındaki mühendislik motoru olarak nasıl çalıştığını açıkla.

Açıklamada özellikle şu ayrımı koru:

**Chat ≠ Agent**

**Agent ≠ LLM**

**LLM ≠ Versa Coder**

Chat kullanıcı ile iletişim katmanıdır.

Agent belirli bir mühendislik görevini gerçekleştiren çalışma birimidir.

LLM ise Agent tarafından kullanılabilecek karar/üretim bileşenlerinden biridir.

---

# 2. CHAT SİSTEMİ

Versa Coder Chat sisteminin ne olduğunu ayrıntılı açıkla.

Şunları değerlendir:

- Kullanıcı Chat'e ne verir?
- Chat kullanıcının niyetini nasıl temsil eder?
- Chat yalnızca soru-cevap sistemi midir?
- Chat ile Agent sistemi arasındaki ilişki nedir?
- Chat bir Session başlatabilir mi?
- Chat bir Workflow başlatabilir mi?
- Chat üzerinden Agent görevlendirilebilir mi?
- Chat hangi bilgileri kullanıcıya göstermelidir?
- Chat Agent'ın çalışma durumunu gösterebilir mi?
- Chat sonucu yalnızca text olarak mı değerlendirilmelidir?
- Chat ile Project Context arasındaki ilişki nedir?

Referanslarda desteklenmeyen kesin davranışları uydurma.

---

# 3. CHAT'İN TEMEL AMACI

Chat sisteminin neden var olduğunu açıkla.

Chat'in amacı yalnızca:

> User → Prompt → LLM → Answer

olmamalıdır.

Versa Coder'ın Agentic yaklaşımı içerisinde Chat'in:

> User Intent → Understanding → Agent Selection → Session → Workflow → Execution → Result

sürecindeki rolünü açıkla.

---

# 4. CHAT İLE AGENT ARASINDAKİ İLİŞKİ

Ayrıntılı şekilde açıkla:

```text
USER
  ↓
CHAT
  ↓
USER INTENT
  ↓
TASK UNDERSTANDING
  ↓
AGENT SELECTION
  ↓
SESSION
  ↓
WORKFLOW
  ↓
AGENT EXECUTION
  ↓
RESULT
  ↓
CHAT
  ↓
USER

┌──────────────────────────────────────────────────────────────────────────────────┐
│                         VERSA CODER PLATFORM ANATOMİSİ                          │
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ PRESENTATION ] DevExpress 2026 WinForms · Docking · Realtime Visual Graph    │
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ APPLICATION  ] Intent Engine · Session Wizard · CQRS · Orchestrator Pipeline  │
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ AGENT RUNTIME] Swarm Coordinator · Role Matrix · Task Decomposer · Policy Gate│
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ PROTOCOL/MCP ] MCP Client/Server · Tool Gateway · Plugin Sandbox · JSON-RPC   │
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ INFRASTRUCTURE] 28+ Modüler DLL (Roslyn, Data, Git, Observability, Security)  │
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ DOMAIN/CORE  ] Immutable Session State · Entities · Value Objects · Events    │
└──────────────────────────────────────────────────────────────────────────────────┘
```

### 3.3 Çoklu-Ajan (Multi-Agent Swarm) ve Rol Ekosistemi

Versa Coder, tek bir büyük prompt ile her işi çözmeye çalışmaz. Bunun yerine ayrık sorumluluk prensibine göre tanımlanmış **Ajan Sürüsü (Swarm)** kullanır:

- **Orchestrator Agent:** İşi analiz eder, iş akışını (DAG/Workflow) planlar, alt ajanları koordine eder ve görev tamamlanma durumlarını izler.
- **Architect Agent:** Clean Architecture, SOLID, Layered DLL hiyerarşisi ve tasarım desenleri doğrultusunda sistem tasarımı ve refactoring planı üretir.
- **Developer / Coding Agent:** Belirlenen mimariye ve katman kurallarına uygun temiz, tip güvenli ve standartlara uygun kod üretir.
- **Reviewer / Quality Agent:** Yazılan kodu SOLID, Clean Code, code smells ve kod kalitesi açısından denetler, onay vermeden kodu bir sonraki aşamaya geçirmez.
- **Security & Policy Agent:** OWASP 2025, güvenlik zafiyetleri, credential sızıntıları, bellek ve threading güvenlik kontrollerini uygular.
- **Testing Agent:** xUnit, NUnit, BenchmarkDotNet testleri üretir; runtime test yürütmesini ve test kapsama (%90+ coverage) oranını kontrol eder.
- **Sektörel Uzman Ajanlar:** Otomotiv (MISRA, AUTOSAR), Sağlık (HL7/FHIR, HIPAA), Finans (ISO 20022, yüksek işlem hacmi), IoT/Embedded (düşük bellek, register bazlı C/C++), Oyun ve Siber Güvenlik gibi sektörel domain bilgisine sahip uzman ajanlar.

### 3.4 Versa Coder'ı Benzerlerinden Ayıran Temel Özellikler

| Kriter | Geleneksel AI Asistanları (Copilot, Cursor vb.) | Versa Coder Platformu |
|---|---|---|
| **Çalışma Modeli** | Basit Chat / Inline autocomplete odaklı | Otonom, çok adımlı, doğrulanabilir **Agentic Swarm Workflow** |
| **Mimari Yaklaşım** | Kod blokları üretir, katman bütünlüğünü koruyamaz | **50+ Katmanlı DLL / Clean Architecture** sınırlarını zorunlu kılar |
| **Karar Mekanizması** | Doğrudan LLM çıktısı | Intent Analysis → Wizard → Immutable Session Config → Tool/Policy Gate |
| **Araç Entegrasyonu** | Kısıtlı dahili fonksiyonlar | **Tam MCP (Model Context Protocol)** + Dinamik C# Plugin Sandbox |
| **Geniş Domain Desteği** | Sadece genel amaçlı web/scripting | **Embedded (STM32/PIC), Driver (ASIO/Kernel), Kurumsal ve Sektörel Uzmanlık** |
| **Güvenlik ve Onay** | Şeffaf olmayan arka plan çalıştırma | **Human-in-the-Loop**, onay kapıları, AES-256 şifreli kasa, Audit Log |
| **Bağlam Yönetimi** | Basit token penceresi | **Epoch Assembly**, Bilgi Kasası (.ai / Obsidian / Markdown SSOT) ve Sürekli Öğrenme |

Özetle Versa Coder; yazılım geliştirme sürecinin her evresinde **yüksek determinizm**, **kurumsal güvenlik**, **katmanlı mimari disiplini** ve **otonom mühendislik zekasını** bir araya getiren yeni nesil bir geliştirme ekosistemidir.
# 4. Proje Hangi Sorunu Çözer

Günümüz yazılım dünyasında yapay zekâ asistanları (Copilot, web tabanlı LLM sohbetleri, basit kod tamamlayıcılar) yaygınlaşmış olsa da, kurumsal ve endüstriyel yazılım geliştirme süreçlerinde **kritik mühendislik açıklarına**, **mimari bozulmalara** ve **yönetilebilirlik krizlerine** yol açmaktadır. 

Versa Coder, yapay zekâyı basit bir "metin tahminleyicisi" olmaktan çıkarıp **deterministik, güvenli ve katmanlı bir mühendislik sistemine** dönüştürerek aşağıdaki temel sorunları kökünden çözer:

### 4.1 Mimari Erozyon ve "God Object / Spagetti Kod" Üretimi

* **Mevcut Sorun:** Geleneksel yapay zekâ araçları, projenin kurumsal mimarisini (Clean Architecture, SOLID, Onion, CQRS, Hexagonal) kavrayamaz. Ürettikleri çözümleri genellikle tek bir dosyaya, Controller veya Form sınıfının içine (God Class) yığar; katman sınırlarını ihlal eder ve kod tabanında çok kısa sürede devasa bir mimari borç (Technical Debt) yaratır.
* **Versa Coder Çözümü:** 50+ bağımsız **Class Library (DLL)** katman hiyerarşisini ve katı mimari kuralları (`Architect Agent` ve Roslyn AST analizörleri) zorunlu kılar. Kod üretimi her zaman `Domain -> Abstractions -> Application -> Infrastructure -> UI` akışına uygun gerçekleşir; katman ihlalleri derleme ve review aşamasında otomatik engellenir.

### 4.2 Sığ Bağlam (Context Degradation) ve Parçalanmış Proje Görüşü

* **Mevcut Sorun:** Standart AI araçları yalnızca açık olan dosyayı veya sınırlı birkaç kod satırını görür. Projenin bağımlılık ağacını, veritabanı şemasını, domain event'lerini ve servis kontratlarını bir bütün olarak algılayamadığı için sıklıkla var olan sınıfları yeniden tanımlar, hatalı DTO eşlemeleri yapar veya halüsinasyon (hallucination) üretir.
* **Versa Coder Çözümü:** **`Infrastructure.Context`** ve **`Infrastructure.ProjectAnalysis`** modülleri üzerinden tüm projeyi semantik olarak indeksler. Roslyn tabanlı AST motoru ve **Epoch Assembly** mekanizması ile projenin tip hiyerarşisini, semantik modellerini ve bağımlılık grafını tam bağlam olarak ajana iletir.

### 4.3 Otonom İş Ayrıştırma ve Orkestrasyon Yokluğu (Orchestration Deficit)

* **Mevcut Sorun:** Geliştiriciler, karmaşık bir modül veya özellik eklerken yapay zekâyı satır satır mikro-yönetmek (Micro-management) zorundadır. "Şimdi arayüzü yaz", "Şimdi servisi yaz", "Şimdi DTO'yu ekle", "Şimdi testini yaz" şeklinde onlarca manuel prompt girilmektedir.
* **Versa Coder Çözümü:** **Multi-Agent Swarm & Task Decomposition** motoru devreye girer. Kullanıcının tek bir yüksek seviyeli gereksinimini (Intent) analiz eder; bunu alt görevlere böler ve sırasıyla **Architect**, **Developer**, **Reviewer**, **Security** ve **Testing** ajanlarına dağıtarak uçtan uca otonom olarak yürütür.

### 4.4 Kapalı Ekosistemler ve Araç İzolasyonu (Tool Silos & Vendor Lock-in)

* **Mevcut Sorun:** Mevcut IDE eklentileri kapalı kutudur. Harici CLI araçlarına, şirket içi API'lara, veritabanlarına veya özel donanım derleyicilerine bağlanmak imkânsızdır ya da karmaşık entegrasyonlar gerektirir.
* **Versa Coder Çözümü:** Açık endüstri standardı olan **Model Context Protocol (MCP)** desteği ve `AssemblyLoadContext` tabanlı **Dinamik C# Plugin Sandbox** mimarisi sunar. Sistem hem MCP Client hem MCP Server olarak çalışarak projenin ihtiyaç duyduğu 100+ harici aracı, CLI'ı ve veri kaynağını güvenle orkestre eder.

### 4.5 Kurumsal Güvenlik, Denetlenebilirlik ve Zafiyet Açıkları

* **Mevcut Sorun:** AI tarafından üretilen kodlar genellikle güvenlik kontrollerinden geçmez; SQL Injection, XSS, CSRF, güvensiz deserialization (OWASP Top 10) gibi açıklar içerir. Ayrıca API anahtarları düz metin olarak promptlara sızabilir ve yapılan değişikliklerin geriye dönük denetim kaydı (Audit Trail) tutulamaz.
* **Versa Coder Çözümü:** **`Infrastructure.Security`** katmanı ile AES-256-GCM şifreli credential kasası sağlar. **Security & Policy Agent** her kod parçasını OWASP 2025 standartlarında tarar. Kritik işlemler (dosya silme, migration çalıştırma, API çağırma) **Human-in-the-Loop Onay Kapıları (Approval Gates)** ile kilitlenir ve tüm session adımları değiştirilemez (immutable) audit loglarına kaydedilir.

### 4.6 Düşük Seviyeli Mühendislik ve Sektörel Uzmanlık Boşluğu

* **Mevcut Sorun:** Standart LLM'ler genel web/scripting dillerinde eğitildiği için; Gömülü Sistemler (Embedded C/C++, STM32, ESP32, AVR, PIC), Donanım Sürücüleri (Audio ASIO, Windows Kernel/User Mode) ve regülasyona tabi sektör standartlarında (MISRA, AUTOSAR, HL7/FHIR, ISO 20022) yetersiz kalır.
* **Versa Coder Çözümü:** **`Infrastructure.Embedded`** ve **`Infrastructure.Driver`** altyapıları ile donatılmıştır. Sektörel uzman ajanlar; donanım register haritalarını, gerçek zamanlı (real-time lock-free) kısıtlamaları, bellek sınırlamalarını ve sektörel uyumluluk kurallarını doğrudan derleme sürecine entegre eder.

### 4.7 Belleksiz Yapay Zekâ (Amnesic AI) ve Sürekli Öğrenme Eksikliği

* **Mevcut Sorun:** AI ile çalışırken oturum kapandığında tüm bağlam kaybolur. Geliştiricinin yaptığı mimari düzeltmeler, kod tercihleri veya proje kuralları bir sonraki oturumda yapay zekâ tarafından unutulur.
* **Versa Coder Çözümü:** **`Infrastructure.Learning`** ve **Knowledge Vault (.ai / Obsidian / Markdown SSOT)** entegrasyonu sunar. Kullanıcının yaptığı düzeltmelerden (Refactor/Feedback), geçmiş oturum kararlarından (Decision Logs) ve hata çözümlerinden ders çıkararak kurumsal hafızayı sürekli günceller.

### 4.8 Problem — Versa Coder Çözüm Matrisi

| # | Karşılaşılan Kritik Problem | Geleneksel AI Sonucu | Versa Coder Çözümü & Etkisi |
|---|---|---|---|
| **1** | Mimari Disiplin Eksikliği | Monolitik spagetti kod, God Class oluşumu | **50+ DLL Clean Architecture & Roslyn Katman Koruması** |
| **2** | Sığ & Parçalı Bağlam | Halüsinasyon, var olan kodların tekrar yazılması | **AST tabanlı Semantik İndeksleme & Epoch Assembly** |
| **3** | Manuel Mikro-Yönetim | Geliştiricinin her adımı tek tek promptlaması | **Multi-Agent Swarm ile Otonom Görev Ayrıştırma** |
| **4** | Araç & Veri İzolasyonu | Sadece metin tamamlama, araçlara erişememe | **Full Model Context Protocol (MCP) & Plugin Sandbox** |
| **5** | Güvenlik & Denetim Riski | OWASP açıkları, sızan anahtarlar, denetimsizlik | **AES-256 Kasa, OWASP 2025 Taraması & Audit Trail** |
| **6** | Embedded / Driver Yetersizliği | Donanım kısıtlarını ve register'ları bilmeme | **Embedded/Driver Katmanları & MISRA/ASIO Uzmanlığı** |
| **7** | Oturum Hafıza Kaybı | Her açılışta kuralların ve bağlamın unutulması | **Knowledge Vault (.ai SSOT) & Sürekli Öğrenme Motoru** |


# 5. Projeyi Açıkla (Detaylı Sistem Mimarisi ve Çalışma Mekanizması)

**Versa Coder**, kullanıcının doğal dilde belirttiği yüksek seviyeli yazılım hedeflerini alan, bu hedefleri çok adımlı deterministik mühendislik süreçlerine dönüştüren, uzman ajan sürülerini (Agent Swarm) koordine eden ve sonucu endüstriyel kalite standartlarında üreten **otonom bir yazılım geliştirme ekosistemidir**.

Bu bölümde sistemin iç işleyişi, katmanlar arası veri akışı, sihirbaz (Wizard) mimarisi, MCP entegrasyonu, hibrit bellek motoru ve kullanıcı arayüzü derinlemesine açıklanmaktadır.

### 5.1 Uçtan Uca Mühendislik Yaşam Döngüsü (End-to-End Execution Lifecycle)

Versa Coder içerisinde bir geliştirme veya refactoring talebi şu 6 ana aşamadan geçerek icra edilir:

```
┌─────────────────┐     ┌──────────────────────┐     ┌──────────────────────┐
│ 1. INTENT & AST │ ──> │ 2. SESSION WIZARD    │ ──> │ 3. SWARM ALLOCATION  │
│    ANALYSIS     │     │    (Config Synthesis)│     │    (DAG Workflow)    │
└─────────────────┘     └──────────────────────┘     └──────────────────────┘
         │                                                       │
         ▼                                                       ▼
┌─────────────────┐     ┌──────────────────────┐     ┌──────────────────────┐
│ 6. PERSISTENCE  │ <── │ 5. VERIFICATION &    │ <── │ 4. EXECUTION, TOOLS  │
│    & AUDIT SYNC │     │    SELF-HEALING      │     │    & MCP INVOCATION  │
└─────────────────┘     └──────────────────────┘     └──────────────────────┘
```

1. **Niyet ve Proje Analizi (Intent & AST Analysis):**
   Kullanıcı Chat ekranından bir talep girdiğinde (Örn: *"Mevcut veritabanı erişim katmanını Dapper'dan EF Core 8'e geçir, Repository ve UnitOfWork desenini Clean Architecture kurallarına göre refactor et ve unit testlerini yaz"*), Niyet Çözümleme Motoru (`IntentAnalysisEngine`) devreye girer. Projenin bağımlılık ağacı, hedef sınıflar ve risk seviyesi Roslyn AST üzerinden taranır.

2. **Session Agentic Agent Wizard (Yapılandırma Sentezi):**
   Sistem doğrudan kontrolsüz bir kod üretimine geçmez. Kullanıcıya özel dinamik bir **Session Wizard** açılır. Bu sihirbaz; göreve en uygun ajan kombinasyonunu, kullanılacak LLM modellerini, gerekli MCP araçlarını ve güvenlik izinlerini önceden yapılandırır. Kullanıcı "Onayla / Başlat" dediğinde **Immutable `SessionConfiguration`** nesnesi üretilir.

3. **Görev Ayrıştırma ve Sürüye Dağıtım (Swarm Task Allocation):**
   Orchestrator Agent, işi bir Yönlü Döngüsüz Graf (DAG - Directed Acyclic Graph) haline getirir:
   - **`Architect Agent`**: Yeni katman sınırlarını, interface kontratlarını ve DTO yapılarını tasarlar.
   - **`Developer Agent`**: Tasarıma uygun C# 12 / net48 + net10.0-windows kodlarını üretir.
   - **`Security Agent`**: SQL injection, hassas veri sızıntısı ve OWASP zafiyet kontrollerini yapar.
   - **`Testing Agent`**: xUnit ve FluentAssertions tabanlı birim testleri oluşturur.

4. **Yürütme, Araç ve MCP Çağrıları (Execution & Tool Gateway):**
   Ajanlar, tanımlı yetkileri dahilinde **MCP Araçları**, **Roslyn AST Manipülatörleri**, **Git Servisi** ve **Dosya Sistemi** araçlarını çalıştırır. Her araç çağrısı güvenlik duvarından (Policy Engine) geçer; izinsiz hiçbir dış kaynak erişimine veya yıkıcı dosya işlemine izin verilmez.

5. **Doğrulama, Derleme ve Kendi Kendini Onarma (Verification & Self-Healing):**
   Kod yazımı bittiğinde Roslyn derleyicisi arka planda kodu derler ve testler otomatik koşulur:
   - Derleme hatası veya kırılan test varsa, `Self-Healing Loop` devreye girer.
   - Hata çıktısı doğrudan ilgili ajana geri beslenir ve ajan kodu düzeltene kadar iterasyon devam eder.
   - Kod kalitesi, SOLID ve Clean Code kuralları `Reviewer Agent` tarafından onaylanmadan süreç tamamlanmaz.

6. **Kalıcılık, Denetim ve Bilgi Kasası Senkronizasyonu (Persistence & Audit Sync):**
   İşlem başarıyla tamamlandığında tüm değişiklikler Git staging alanına alınır, detaylı bir mimari karar kaydı (ADR) oluşturulur, SQLite/SQL Server oturum geçmişine yazılır ve `.ai` bilgi kasasındaki (Knowledge Vault) kural ve hafıza dosyaları güncellenir.

   # Session Agentic Agent Wizard İstemi

Şimdi bir de **Chat Agent** ekranında, benim istediğim şeyi anlatarak veya prompt ile yazarak belirteceğim bir yapı istiyorum.

**“Gönder”** dediğimde bir **Session Agentic Agent Wizard** açılması gerekiyor. Bu wizard içerisinde session’ı istediğimiz gibi kurup yönetebilmeli, istediğimiz agent’ları ve özellikleri seçerek işlemi yapılandırabilmeliyiz.

Wizard içerisinde **Chat Session** ve **Agent Session** ayarlarını yapabilmeli, session’ın nasıl çalışacağını belirleyebilmeli, istediğimiz agent’ları ve gerekli özellikleri seçerek yapılandırabilmeliyiz. Ardından ileri seviye ayarları yapıp **“Bitti”** butonuna basabilmeliyiz.

**“Bitti”** butonuna bastığımızda sistem, wizard içerisinde yaptığımız tüm ayarları dikkate alarak gerekli **System Prompt** ve session yapılandırmasını oluşturmalı ve session’ı başlatmalıdır.

Yapay zekâ, wizard içerisinde belirlediğimiz yapılandırmaya göre çalışmalı ve istediğimiz işlemi bu yapıya uygun şekilde gerçekleştirmelidir.

Ben bunun için, yapay zekânın istediğimiz yapıyı kurmasını ve bu wizard’a uygun şekilde çalışmasını sağlayacak profesyonel bir **istem/prompt sistemi** istiyorum.

Bunu doğru şekilde tasarlayabilmek için önce bana gerekli soruları sor. Sorular üzerinden ilerleyerek sistemi birlikte netleştirelim.

Soruları sormaya başlayalım. **diaframalrı oluştur** .ai ya ekle yaz ver  planı uygula

# Session Agentic Agent Wizard İstemi ve Dinamik Orkestrasyon Mimarisi

Bu istem ve mimari şartname; geliştiricinin **Chat Agent** arayüzünde girdiği doğal dil talebini (Prompt), doğrudan kontrolsüz bir modele göndermek yerine araya giren ve tüm süreci deterministik, kurallı ve denetlenebilir kılan **Session Agentic Agent Wizard (Oturum ve Ajan Yapılandırma Sihirbazı)** mekanizmasını tanımlar.

Sistem, kullanıcının "Gönder" butonuna basmasıyla tetiklenir; statik bir sohbet akışını anında **Kurumsal Düzeyde Çoklu-Ajan Yönetim İstasyonuna (Agent Swarm Control Station)** dönüştürür.

