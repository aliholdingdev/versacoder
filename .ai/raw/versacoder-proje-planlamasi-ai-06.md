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
#                           Versa Coder Properties                                 #
#                                                                                  #
####################################################################################

### 5.3 Session Agentic Agent Wizard'ın İç Çalışma Mekanizması

Session Wizard, kullanıcının sistem üzerindeki kontrolünü maksimize eden **Human-in-the-Loop** yönetim panelidir:

1. **Akıllı Profilleme:** Wizard açıldığında projenin türünü (.NET, Embedded, C++, Web, Driver) algılar ve varsayılan ajan ekibini otomatik seçer.
2. **Çoklu Oturum Tipleri:**
   - **Tekli Session (Single Agent):** Hızlı ve küçük çaplı sorgular/düzenlemeler için.
   - **Çoklu Sürü Session (Swarm Intelligence):** Mimar, Geliştirici, Reviewer ve Testçinin paralel çalıştığı büyük özellik geliştirmeleri.
   - **Zincirleme Session (Chained Pipeline):** Bir ajanın çıktısının diğerinin girdisi olduğu sıralı analiz süreçleri.
   - **Dağıtılmış Session (Distributed Agentic Node):** Ağ üzerindeki farklı MCP veya arka plan sunucularına dağıtılmış görevler.
3. **Dinamik System Prompt Sentezleyici:** Wizard üzerindeki tüm seçimler (sektör kuralları, dil standartları, güvenlik seviyesi, bellek derinliği) birleştirilerek her ajan için dinamik, çelişkisiz ve amaca özel bir **Meta-System-Prompt** sentezlenir.

---

### 5.4 Model Context Protocol (MCP) ve Plugin Sandbox Mimarisi

Versa Coder, genişletilebilirliğini iki temel taşıyıcı üzerine kurar:

* **Model Context Protocol (MCP) Gateway:**
  - `stdio` ve `SSE (Server-Sent Events)` taşıma protokolleri üzerinden çalışan MCP istemcileri.
  - Veritabanı sorgulayıcıları, API test araçları, dokümantasyon arama servisleri ve harici LLM context sağlayıcıları dinamik olarak sisteme bağlanabilir.
* **İzole Plugin Sandbox (`AssemblyLoadContext`):**
  - Üçüncü parti C# DLL plugin'leri ana uygulama belleğini kirletmeden ayrı bir `AssemblyLoadContext` içinde yüklenir ve çalıştırılır.
  - Eklenti çökerse veya bellek tüketim sınırını aşarsa ana IDE etkilenmez, eklenti güvenle izole edilip bellekten düşürülür (Unload).

---

### 5.5 Çift Katmanlı Hibrit Bellek ve Sürekli Öğrenme

Versa Coder, unutkan yapay zekâ (Amnesic AI) problemini çift katmanlı bellek mimarisi ile ortadan kaldırır:

1. **Kısa Dönemli Çalışma Belleği (Working Memory / Epoch Assembly):**
   - Aktif oturumdaki mesaj geçmişi, çağrılan araçların çıktıları ve token bütçesini aşmayacak şekilde dinamik kayan pencere (Sliding Window) yöntemiyle yönetilir.
2. **Uzun Dönemli Kurumsal Bellek (Knowledge Vault & Vector Store):**
   - **`.ai` Bilgi Kasası:** Markdown / Obsidian tabanlı SSOT (Single Source of Truth) dosyaları (`rules/`, `skills/`, `decisions/`, `memory/`).
   - **Vektörel & Semantik Arama:** Proje dokümantasyonu ve geçmiş oturumlarda çözülen hatalar semantik embedding vektörleri ile SQLite tabanında indekslenir.
3. **Geri Bildirimle Öğrenme Döngüsü (Continuous Learning Loop):**
   - Geliştirici bir kod bloğunu elle düzelttiğinde veya bir kural eklediğinde, sistem aradaki farkı (Diff) analiz eder, öğrenilen kuralı `.ai/learning/` dizinine kaydeder ve gelecekteki oturumlarda aynı hatayı tekrarlamaz.

---

### 5.6 DevExpress Tabanlı UI/UX ve Gerçek Zamanlı Görselleştirme

Kullanıcı deneyimi, kurumsal masaüstü gücünü ve modern reaktif arayüz dinamizmini birleştirir:

* **Çoklu Panel Docking:** DevExpress DockManager ve DocumentManager ile kod editörü, ajan konsolu, test paneli ve proje gezgini esnek bir şekilde düzenlenebilir.
* **Görsel Yürütme Grafiği (Visual Execution Graph):** Hangi ajanın o anda hangi görevi yürüttüğü, hangi araçları çağırdığı ve iş akışının ilerleme yüzdesi gerçek zamanlı nod/grafik tabanlı ekranda izlenir.
* **Canlı Telemetri Paneli:** Anlık token tüketimi, model gecikme süreleri (latency), bellek kullanımı ve operasyonel maliyetler gerçek zamanlı göstergelerle sunulur.
* **Onay Kapıları (Approval Gate UI):** Ajanın yapacağı kritik dosya yazma, Git commit veya terminal çalıştırma işlemleri kullanıcıya net diff blokları ve onay diyalogları ile gösterilir.

