---
title: "keys.md — Anahtar Kelime Hız Haritası"
type: reference
category: ".ai"
status: active
version: 3.0.0
updated: 2026-10-07
authority: SSOT
scope: "Kelime/soru → hedef dosya hız haritası, 91 satır anahtar + soru kalıpları + klasör eşlemesi"
---

# keys.md — Anahtar Kelime Haritası

**Zorunlu Bağlantılar:** [index](index.md) · [glossary](glossary.md) · [brain](brain.md) · [CLAUDE](CLAUDE.md) · [WORKFLOW](WORKFLOW.md) · [AGENTS](AGENTS.md) · [MEMORY](MEMORY.md) · [log](log.md) · [architecture/index](architecture/index.md) · [adr-index](.decisions/adr-index.md)

> **Tek cümlelik sorumluluk:** Aklına gelen kelimeyi (veya soruyu) yazdığında hangi dosyaya gideceğini 3 saniyede buldurmak.

## §0 Özet (Answer First)

| # | Soru | Cevap |
|---|------|-------|
| 1 | Bu dosya ne yapar? | Kelimeyi §1'de bulur, "Gidilecek dosya" sütunundaki hedefi verir; cevabın kendisi burada **değil**, hedef dosyadadır. |
| 2 | Hızlı yol hangisi? | Soru kalıbıysa §2, kelimeyse §1; ikisi de tek satırda dosya adı döndürür. |
| 3 | Derinlik nerede? | §1.1 gruplar, §2.1 ADR soruları, §2.2 kök envanter, §2.3 klasör eşlemesi, §3.1 arama kalıpları. |
| 4 | Ne zaman durulur? | §1'de satır yoksa `index.md` §1 haritasına inilir; o da yoksa `log.md`'ye kayıt açılır ve `architect` beklenir. |
| 5 | SSOT ne demek? | `authority: SSOT` bu konuda tek doğru dosya demektir; ikinci kopya açmak yasaktır (ADR-021). |

**Kullanım sırası:** kelimeyi bul → hedef dosyayı aç → hedefteki § numarasına in. Bu dosyada okumaya devam etmek, aramanın kendisini ertelemektir.

---

## §1 Anahtar Kelime → Dosya (101 satır)

Sütunlar: hangi kelime hangi dosyaya gider, o dosya **neyi cevaplar**, o satır **ne zaman** açılır. "Ne zaman açılır" sütunu, kelimenin geçtiği anı işaret eder; dosya o an açılır, kelime burada çözülmez.

| # | Anahtar kelime / ifade | Gidilecek dosya | Ne cevaplar | Ne zaman açılır |
|---|------------------------|-----------------|-------------|-----------------|
| 1 | `EF Core` · ORM · `DbContext` | `brain.md` §1 (ADR-005/016) | ORM yasağının gerekçesi ve yerine konan yol | biri "EF kuralım" dediğinde |
| 2 | `Dapper` · `DbConnection` · provider | `brain.md` §1 (ADR-005/016), `PROJECTS.md` §5 | hangi sürücüyle hangi bacakta bağlanıldığı | veri katmanı kodu konuşulurken |
| 3 | `Dapper` parametresi · `@p1..@pN` | `brain.md` §3 kural 5 | sorguya değeri değil yer tutucuyu gönderme kuralı | SQL cümlesi yazılacakken |
| 4 | SQL injection · string-concat | `brain.md` §3 kural 5, `.rules/index.md` | birleştirmeli SQL'in neden otomatik RED olduğu | güvenlik denetimi istenince |
| 5 | migration · `schema_version` | `brain.md` §1 (ADR-005) | saf SQL dosyalarının sırasını ve imzasını | şema değişikliği düşünüldüğünde |
| 6 | transaction · `CommandTimeout` | `.rules/index.md` | tek bağlantıda işi bitirme ve bekleme sınırı | çok adımlı yazma eklenirken |
| 7 | secret · DPAPI · bağlantı parolası | `brain.md` §3 kural 6, `brain.md` §1 (ADR-018) | anahtarın nerede ve nasıl saklandığı | ayar veya anahtar konuşulduğunda |
| 8 | `N+1` · döngüde sorgu | `.rules/index.md` | liste sorgusunun neden tek sorgu olması gerektiği | yavaşlık şikayeti gelince |
| 9 | DevExpress · v26.1 | `brain.md` §1 (ADR-004), `PROJECTS.md` §2 | tek geçerli sürümün hangisi olduğu | paket sürümü sorulduğunda |
| 10 | ribbon · `RibbonControl` | `.rules/index.md`, `VISION.md` §3 | şerit bileşeninin ekranlarda nasıl durduğu | 02-main-ide ekranı konuşulurken |
| 11 | `Office2010` · ribbon stili | `.rules/index.md` | izin verilen tek stilin adı ve RED nedeni | stil değişikliği istenince |
| 12 | `.Designer.cs` · `.resx` | `AGENTS.md` §1, `.rules/index.md` | tasarım dosyasının sahibi ve yazım yasağı | ekran kodu elden yazılmak istenince |
| 13 | tema · skin · dil | `VISION.md` §3 (07) | ayarlar ekranının taşıdığı seçenekler | görünüm değiştirilirken |
| 14 | erişilebilirlik · accessibility | `.rules/index.md` | klavye ve ekran okuyucu kuralları | ekran kabulü konuşulurken |
| 15 | UI kabuğu · tek pencere | `VISION.md` §3 (02) | ana pencerenin ne taşıdığı, neyi taşımadığı | panel yerleşimi sorulduğunda |
| 16 | 7 ekran · ekran listesi | `VISION.md` §3 | ekran adlarının tamamı ve öncelikleri | yeni ekran talebi gelince |
| 17 | `TFM` · `net48` · `net10.0-windows` | `brain.md` §1 (ADR-001) | hangi etiketle hangi sürümün derlendiği | derleme hatası göründüğünde |
| 18 | iki bacak · dual TFM · `#if` | `PROJECTS.md` §2.1 | iki bacağın paylaştığı kodun sınırı | platform farkı eklenirken |
| 19 | `csproj` · `slnx` · çözüm | `PROJECTS.md` §1 | çözümde kaç proje olduğu ve adları | yeni dosya veya proje eklenirken |
| 20 | `CPM` · `Directory.Packages.props` | `brain.md` §1 (ADR-014), `PROJECTS.md` §2 | paket sürümünün neden `.csproj`'da olmadığı | NuGet hatası alındığında |
| 21 | 9 proje · proje listesi | `PROJECTS.md` §1 | üretim projesi adlarının tamamı | "hangi proje" denildiğinde |
| 22 | derleme · build · `-f net48` | `PROJECTS.md` §7 | iki bacakta derleme komutu ve bilinen hatalar | build kırmızıyken |
| 23 | paket sürümü · `NU1008` | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | test paketlerinin `PackageVersion` satırları | restore hata verdiğinde |
| 24 | iki bacak matrisi | `PROJECTS.md` §2.1 | hangi paketin hangi bacakta oturduğu | paket eklenmeden önce |
| 25 | katman · `l0..l7` · makro katman | `VISION.md` §2, `PROJECTS.md` §3 | sekiz makro katmanın birer cümlelik işi | dosyanın hangi katmana gideceği |
| 26 | 52 düğüm · düğüm | `architecture/index.md` | ağacı oluşturan kutuların sayısı ve adları | katalogda arama başlarken |
| 27 | `N` kodu · `N01..N44` | `architecture/index.md` | katalog kaydının hangi klasöre denk geldiği | "yeni tip nereye yazılır" denilince |
| 28 | yön · bağımlılık oku · NetArchTest | `checklists/layer-boundary-check.md` | izin verilen ok yönü ve 13 denetim sorusu | ters referans şüphesinde |
| 29 | `l0` · port · soyut arayüz | `architecture/index.md` | sözleşmelerin hangi katmanda durduğu | interface eklenirken |
| 30 | `l5` · Protocol · VersaProtocol | `PROJECTS.md` §1, `architecture/index.md` | ortak sözlemenin hangi projede yaşadığı | paylaşılacak tip aranırken |
| 31 | `l6` · Host · composition root | `VISION.md` §2, `architecture/index.md` | bağımlılıkların tek noktada kurulduğu yer | DI kaydı konuşulurken |
| 32 | `l4` · Infrastructure | `architecture/index.md` | somut uygulamaların (db, dosya, ağ) yeri | somut sınıf nereye sorulduğunda |
| 33 | tier · DLL · envanter | `reports/k3-618-ham-envanter.md` | tier listesinin neden kapsam dışında kaldığı | K3'ten DLL sorulduğunda |
| 34 | `K3` · 618 · 616 | `PROJECTS.md` §4 (R1) | ham envanterdeki tekrar ve eksik kayıtlar | sayı tartışması açılınca |
| 35 | 5440 · bileşen sayısı · marj | `PROJECTS.md` §4 (R3), `brain.md` §4 | katalog sayımının eşiği ve sıfır marjı | sayı tartışması açılınca |
| 36 | bileşen · katalog kaydı | `architecture/index.md`, `reports/index.md` | sayılabilir birimin ne olduğu | "sayım neyi sayıyor" denilince |
| 37 | ADR-028 · 8 makro katman | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | baz kararın tam metni, 6 düzeltme | katman projesi tartışılınca |
| 38 | gate · kapı | `WORKFLOW.md` | sıradaki kontrol noktasının adı ve sahibi | "şimdi neredeyim" denildiğinde |
| 39 | `GATE-004a` · vault green | `WORKFLOW.md` §2.1, `brain.md` §4 | vault ölçümünün geçme şartları | dosya üretiminden hemen önce |
| 40 | `GATE-003` · wireframe onayı · hard stop | `VISION.md` §5, `brain.md` §4 | onaysız `.cs` yazımının yasağı | `src/` altına dosya açılırken |
| 41 | `GATE-004` · ilk commit | `brain.md` §1 (ADR-022) | ilk commit'in hangi kapıdan sonra atıldığı | commit düşünüldüğünde |
| 42 | `GATE-005` · test green | `WORKFLOW.md` §2.4, `spec/test-strategy.md` | test kapısının kabul kriteri | testler düştüğünde |
| 43 | `GATE-006` · teslim | `WORKFLOW.md` §2.5, `engine.md` | teslim raporunun içerik şartı | iş kapanırken |
| 44 | kanıt kutusu · `KANIT/DOSYA/EKSIK/SONRAKI` | `WORKFLOW.md` §2, `AGENTS.md` §2 | gate kutusunun dört satırı | kanıt hazırlanırken |
| 45 | çıktı kutusu · 5 alan | `AGENTS.md` §2 | ajan kutusunun beş alanı ve RED kriteri | ajan çağrılırken |
| 46 | RED · otomatik reddedilen desenler | `brain.md` §4.1 | başlamadan elenen desenlerin listesi | kalıp veya şablon seçilirken |
| 47 | 3-STRIKE · bloke raporu | `AGENTS.md` §4 | üç başarısızlıkta durma biçimi ve rapor şablonu | aynı düzeltme üçüncü kez düşerse |
| 48 | devir · devre paketi | `AGENTS.md` §3, `ROLE.md` §5 | devirde zorunlu dört maddenin adı | işi başka ajana verirken |
| 49 | bütçe · token · byte | `AGENTS.md` §6 | çıktı, dosya ve soru sınırları | yanıt uzadıkça |
| 50 | rate limit · paralel ajan | `AGENTS.md` §5, `engine.md` | aynı anda çalışacak ajan sayısının sınırı | fan-out planlanırken |
| 51 | ajan · rol · dağıtım matrisi | `AGENTS.md` §1, `ROLE.md` | işin kime gideceğini ve önceliği | yeni iş başlarken |
| 52 | X sahipliği · tek yazıcı | `AGENTS.md` §7 | dosyanın hangi ajanın olduğu | iki ajan aynı dosyada buluşunca |
| 53 | mojibake · UTF-8 | `glossary.md` (51), `reports/index.md` | bozuk Türkçe karakterin nedeni ve arayışı | metin bozuksa |
| 54 | bozuk link · link kontrolü | `brain.md` §1 (ADR-003), `index.md` §4 | hedefi olmayan bağlantının bulunma yolu | kapanış ölçümünde |
| 55 | frontmatter · 8 alan | `brain.md` §1 (ADR-003), `.templates/new-doc-contract.md` | dosya künyesinin zorunlu alanları | yeni MD üretilirken |
| 56 | SSOT · tek doğru kaynak | `glossary.md`, `brain.md` §1 (ADR-021) | aynı bilginin neden tek yerde durduğu | bilgi ikinci kez yazılınca |
| 57 | boyut bandı · 30–88 KB | `index.md` §5.2 | kök dosyanın alt ve üst sınırı | dosya kısaltılırken veya genişletilirken |
| 58 | ölçüm scripti · measure/check/count | `index.md` §4 | dört sayacın komutu ve okuma biçimi | bir sayı iddia edilirken |
| 59 | kanıt etiketi · `SOURCE_VERIFIED` | `CLAUDE.md` | doğrulanmış iddianın etiketi | iddia cümlesi yazılırken |
| 60 | `INFERRED` · `NEEDS_VALIDATION` | `CLAUDE.md`, `brain.md` §4.2 | ölçülmemiş ve bilinmeyen madde etiketi | kanıt elde yoksa |
| 61 | Quality Report · log | `log.md` | değişikliğin tarihli kaydı ve sayacı | iş biterken |
| 62 | checklist · 13 soru | `checklists/layer-boundary-check.md` | mimari çıktının denetim listesi | mimari değişiklikte |
| 63 | `Result<T>` · hata fırlatma | `brain.md` §3 kural 1 | dönüş tipinin neden sarmalayıcı olduğu | metot imzası yazılırken |
| 64 | structured log · 5 seviye | `brain.md` §3 kural 2, `brain.md` §1 (ADR-026) | log şablonu ve seviye adları | log satırı yazılırken |
| 65 | TDD · %90 · coverage | `brain.md` §3 kural 3 | test önce kuralı ve kapsam hedefi | test konuşulurken |
| 66 | test stratejisi | `spec/test-strategy.md` | test türlerinin dağılımı ve kapsam beklentisi | plan sorulduğunda |
| 67 | `nullable` · NRT · `!` | `brain.md` §3 kural 4, `.rules/index.md` | null akışının zorunlu işareti | derleyici uyarısı çıktığında |
| 68 | error handling · hata kodu | `.rules/index.md`, `spec/index.md` | hatanın yükseltilme ve gösterilme biçimi | `catch` bloğu yazılınca |
| 69 | naming · adlandırma | `.rules/index.md` | tip, alan ve dosya adı kuralları | ad seçimi tartışılınca |
| 70 | timeout · `CancellationToken` · 60/300 sn | `brain.md` §1 (ADR-009), `.rules/index.md` | bekleme sınırları ve iptal zinciri | araç çağrısı eklenirken |
| 71 | caching · TTL · invalidation | `.rules/index.md` | cache'e alma şartı ve düşürme yolu | cache önerilince |
| 72 | SOLID · SRP/O/L/I/D | `brain.md` §2, `.rules/index.md` | beş ilkenin vault'taki karşılığı | inceleme talebi gelince |
| 73 | session · oturum · transcript | `brain.md` §1 (ADR-007), `MEMORY.md` | oturumun sınırları ve ham kaydı | oturum sorulduğunda |
| 74 | memory · hafıza · kalıcılık | `MEMORY.md` | oturumlar arası neyin kaldığı | yeni oturum başlarken |
| 75 | agent · ajan · alt ajan | `AGENTS.md`, `ROLE.md` | ajan hiyerarşisi ve yetki sınırı | görev dağıtılırken |
| 76 | MCP · tool · araç sözleşmesi | `brain.md` §1 (ADR-006) | aracın giriş ve çıkış biçimi | araç eklenirken |
| 77 | sandbox · araç çalıştırma | `brain.md` §1 (ADR-009) | çalıştırmanın izin ve süre sınırları | komut çalıştırılınca |
| 78 | streaming · SSE | `brain.md` §1 (ADR-011) | cevabın parça parça gelme zorunluluğu | akış sorulduğunda |
| 79 | compaction · 8 MB · 20 tur | `VISION.md` §4, `brain.md` §1 (ADR-008) | sohbetin kırpılma ve arşiv kuralı | sohbet şişince |
| 80 | wizard · sihirbaz | `VISION.md` §4, `glossary.md` | adım adım kurulum ekranının işi | ilk kurulum konuşulunca |
| 81 | görev kuyruğu · task queue | `VISION.md` §3 (06) | ajan iş listesinin hangi ekranda olduğu | panel adı sorulunca |
| 82 | bildirim · notification · toast | `spec/index.md` | gösterim kanalları ve susturma kuralları | hata gösterimi konuşulunca |
| 83 | gözlemlenebilirlik · observability | `spec/index.md` | log, izleme ve ölçümün kapsamı | ölçüm sorulunca |
| 84 | terminal · konsol yüzeyi | `VISION.md` §3 (02) | pencere içinde konsolun yeri | ekran yerleşimi konuşulunca |
| 85 | diff · değişiklik görünümü | `AGENTS.md` §1 | inceleme kanıtının biçimi | inceleme istenince |
| 86 | grep · glob · arama aracı | `brain.md` §1 (ADR-006) | dosya ve metin tarama araçlarının adı | arama yapılmadan önce |
| 87 | anayasa · `G1..G12` | `CLAUDE.md` | yasakların tam listesi ve RED koşulu | "bunu yapabilir miyim" denilince |
| 88 | OpenRouter · NVIDIA NIM · LLM provider | `spec/llm-provider-spec.md`, `spec/index.md` | sağlayıcı portu, endpoint, anahtar ve hata kodları | LLM bağlantısı konuşulunca |
| 89 | `IAgentProvider` · port · adapter | `spec/llm-provider-spec.md` §2 | sağlayıcının l0 portundaki yeri ve bağımlılık oku | provider eklenirken |
| 90 | l0 portlar · Result · ITool | `prompts/l0-portlar.md`, `TODOS/05` §0 | port tasarımı ve tam kod | l0 kodlanırken |
| 91 | scaffold fazı · K0 · N1..N4 · count-dll | `prompts/scaffold-k0.md` … `scaffold-n4.md`, `TODOS/06` §1 | 5000 .csproj üretimi ve kapı | scaffold açılırken |
| 92 | orchestrator · orkestratör · niyet | `engine.md` §1.1.4, `AGENTS.md` §6 | intent → task → plan → agent akışı | görev dağıtılırken |
| 93 | external agent · harici ajan · OpenCode/Codex/Claude Code CLI | `AGENTS.md` §4 | Control Plane vs Execution Plane, adapter sözleşmesi | harici ajan çalıştırılınca |
| 94 | Session Wizard · sihirbaz · immutable config | `AGENTS.md` §5, `engine.md` §1.1 | niyet → yapılandırma → immutable session config | wizard konuşulunca |
| 95 | Chat ≠ Agent ≠ LLM · temel ayrım | `AGENTS.md` §1 | üçlünün tanımı ve karıştırma belirtileri | mimari tartışılınca |
| 96 | self-healing · doğrulama zinciri | `AGENTS.md` §8.3, `engine.md` §1.1.4 | build → test → static analysis → review | "tamamlandı" denilince |
| 97 | 100 part · analiz seti · ELI10 | `G:/…/versacoder proje-ai.00.md` (kaynak) | 13 blok / 100 part analiz seti haritası | set okunurken |
| 98 | görsel üretim · render · visual-director | `AGENTS.md` §8.5, `VISION.md` §0.3 | 8 adımlı görsel hattı + Studio/IDE kimliği | görsel işi gelince |
| 99 | Studio · IDE · dokümantasyon | `VISION.md` §0.3, `README.md` §1 | kod + docs + görsel üç yetenek | kimlik sorulunca |
| 100 | adaptif soru · DISCOVERY COMPLETE | `AGENTS.md` §5.4 | cevap→analiz→soru döngüsü | eksik bilgi olunca |
| 101 | identity preservation · visual source of truth | `AGENTS.md` §8.5 | görselde korunacaklar | referans görselle |

**İlgili (hedef dosyaların tamamı):** [index](index.md) · [CLAUDE](CLAUDE.md) · [AGENTS](AGENTS.md) · [WORKFLOW](WORKFLOW.md) · [ROLE](ROLE.md) · [engine](engine.md) · [brain](brain.md) · [VISION](VISION.md) · [PROJECTS](PROJECTS.md) · [glossary](glossary.md) · [MEMORY](MEMORY.md) · [log](log.md).
### §1.1 Satır grupları (hangi aralık hangi soruyu çözer)

| # | Grup | Satır aralığı | Ana soru | Tipik ilk hedef |
|---|------|---------------|----------|-----------------|
| 1 | Veri erişim ve güvenlik | 1–8 | "SQL/veri nasıl yazılır?" | `brain.md` §3 kural 5 |
| 2 | Arayüz ve DevExpress | 9–16 | "Ekran ve stil kuralları ne?" | `.rules/index.md` |
| 3 | TFM, proje, paket | 17–24 | "Nerede derlenir, paket nerede?" | `PROJECTS.md` §1 |
| 4 | Mimari ve katalog | 25–37 | "Dosya hangi katmana girer?" | `architecture/index.md` |
| 5 | Süreç ve kapılar | 38–52 | "Sıradaki adım ne, kim kapatır?" | `WORKFLOW.md` |
| 6 | Kalite ve ölçüm | 53–62 | "Sayı/kanıt nereden gelir?" | `index.md` §4 |
| 7 | Kod kuralları | 63–72 | "Bu kod kurala uyuyor mu?" | `brain.md` §3 |
| 8 | Vault ve ajan zihniyeti | 73–87 | "Kim yapar, nerede durur?" | `AGENTS.md` |

**Grup okuma kuralı:** bir soru iki gruba giriyorsa önce 4. ve 5. grup okunur; mimari ve kapı, kod kuralından önce gelir, aksi halde yazılan kod geri alınır.

### §1.2 Sütun sözlüğü (tabloyu okuma biçimi)

| # | Sütun | Ne taşır | Okuma hatası |
|---|-------|----------|--------------|
| 1 | Anahtar kelime | aranacak sözcük veya kısaltma | kelimenin tanımını burada aramak |
| 2 | Gidilecek dosya | tek hedef dosya, gerekirse § numarası | birden çok dosyayı üst üste açmak |
| 3 | Ne cevaplar | hedef dosyanın bu kelimeye verdiği cevabın konusu | cevabın kendisini bu tabloda beklemek |
| 4 | Ne zaman açılır | tetikleyici an (soru gelince, kod yazılarken) | dosyayı sürekli açık tutmak |

### §1.3 Yeni anahtar satırı açma kuralı

Bir kelime §1'de yoksa tablonun sonuna satır eklenir; ekleme kriteri "işime yarar" değil, aşağıdaki beş şarttır.

| # | Şart | Soru | Uymazsa |
|---|------|------|---------|
| 1 | Kelime vault'ta gerçekten geçiyor mu? | Dosyada en az bir yerde yazılı mı? | uydurma terim satırı silinir |
| 2 | Hedef tek dosya mı? | En fazla iki hedef yazılabiliyor mu? | hedef belirsizse satır açılmaz, `log.md`'ye sorulur |
| 3 | "Ne cevaplar" sütunu dolu mu? | Cevabın konusu tek cümleyle yazılabiliyor mu? | boş sütun = yönlendirme değil, yönlendirme vaadi |
| 4 | Mevcut bir satırla çakışıyor mu? | Aynı kelime başka satırda var mı? | varsa tek satırda birleştirilir (tekrar yasak) |
| 5 | Bağlı dosya envanterde mi? | Hedef `index.md` §2'de listeleniyor mu? | envanterde yoksa bozuk link üretilir (G4) |

**Ekleme sonrası:** satır numaraları kaymaz, tablo sonuna eklenir; numarayı yeniden sıralamak §4.1 hız testindeki referansları bozar.

### §1.4 Kelime arama akışı (dört karar noktası)

```text
SORU GELDI
   |
   +--> §2'de soru kalibi var mi? --EVET--> dogrudan hedef dosya (2. dosya acilmaz)
   |
   +--> HAYIR
         |
         +--> §1'de anahtar kelime var mi? --EVET--> hedef dosya + § numarasi
         |
         +--> HAYIR
               |
               +--> §1.1 grup tablosundan soruyu en yakin gruba oturt
               |
               +--> index.md §1 haritasindan klasor sec
               |
               +--> hala yoksa log.md kaydi + architect bekle
```

**Akış kuralı:** her ok bir dosya açma kararıdır; dördüncü noktaya kadar dosya açılmamışsa arama yapılmamış, yalnız düşünülmüştür.

---

## §2 Soru → Dosya (hız tablosu)

Kelime değil, cümleyi yazıyorsan bu tablo §1'den hızlıdır; her satır tam soru ifadesiyle eşleşir.

| # | Soru kalıbı | Tam ifade | Doğrudan dosya |
|---|-------------|-----------|----------------|
| 1 | "Bu ekran nasıl görünüyor?" | "01-start-page..07-settings ekranlarının tasarımı ve wireframe listesi nerede?" | `VISION.md` §3, §5 |
| 2 | "Hangi projeler var?" | "9 üretim projesi, katman bağımlılığı ve TFM listesi hangi dosyada?" | `PROJECTS.md` §1–§3 |
| 3 | "Risk ne, neden yapmıyoruz?" | "R1..R5 risk defteri ve kapsam dışı gerekçeleri nerede?" | `PROJECTS.md` §4–§5 |
| 4 | "Bu karar neden alındı?" | "ADR numarası, alternatifler ve sonuç nerede yazıyor?" | `brain.md` §1, `.decisions/adr-index.md` |
| 5 | "Kod yazarken neye uymalıyım?" | "Zorunlu kod kuralları ve BAD → GOOD örnekleri hangi dosyada?" | `brain.md` §3 |
| 6 | "Bu kelime ne demek?" | "Terimin TR/EN karşılığı ve ELI10 tanımı nerede?" | `glossary.md` |
| 7 | "Şu konu hangi dosyada?" | "Bu anahtar kelime hangi hedef dosyaya gider?" | bu dosya §1 |
| 8 | "Nasıl başlarım, sıradaki adım ne?" | "Gate zinciri, kabul kriteri ve kapatan kişi kim?" | `WORKFLOW.md` |

**Tablonun dışı:** sekizinci satırdan sonra aranan soru `index.md` §3'e, oradan da bu dosyaya döner; döngü ikinci turda da kapanmazsa soru `log.md`'ye kayıt olur.

### §2.1 ADR → sık sorulan soru (hız)

| # | "Neden ...?" | ADR | Tek satır | Okunan dosya |
|---|-------------|-----|-----------|--------------|
| 1 | "Neden eski .NET de destekleniyor?" | 001 | `net48` kanıtlanmış legacy bacaktır, ikinci bacak olmadan ürün yarımdır | `brain.md` §1 |
| 2 | "Neden EF kullanmıyoruz?" | 005/016 | Dapper + saf SQL iki bacağı da taşır, plan öngörülebilir kalır | `brain.md` §1 |
| 3 | "Neden DevExpress güncellenmiyor?" | 004 | v26.1 + `Office2010` kilitli, iki bacak doğrulaması bu sürümde yapıldı | `brain.md` §1 |
| 4 | "Neden cevap parça parça geliyor?" | 011 | streaming (SSE) olmadan kullanıcı bekler, iptal edilemez | `brain.md` §1 |
| 5 | "Neden sohbetin eski kısmı gidiyor?" | 008 | 8 MB compaction şişmeyi durdurur, arşiv silinmez | `VISION.md` §4 |
| 6 | "Neden kod daha önce yazılmıyor?" | 025 | wireframe onayı hard stop'tur, onaysız kod geri alınır | `WORKFLOW.md` §2.2 |
| 7 | "Neden paket sürümü `.csproj`'da yok?" | 014 | CPM tek kilit dosyasında sürüm tutar, çakışma baştan biter | `PROJECTS.md` §2 |
| 8 | "Neden ilk commit bu kadar geç?" | 022 | önce vault yeşil, sonra build yeşil, sonra dosya bazlı commit | `WORKFLOW.md` §2.3 |
| 9 | "Neden tier listesi katalogda yok?" | 028 | iki sayım birimi olursa 5440 eşiği anlamsızlaşır | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |

### §2.2 Vault kök dosya envanteri

| # | Dosya | Ne taşır | Ne taşımaz | Sık aranan anahtar |
|---|-------|----------|------------|--------------------|
| 1 | `index.md` | tek navigasyon girişi ve harita | konu içeriği (listeyi tekrar etmez) | "X nerede" |
| 2 | `CLAUDE.md` | anayasa, `G1..G12` yasakları | süreç akışı ve gate sırası | yasak, RED, etiket |
| 3 | `WORKFLOW.md` | gate → gate adım sırası ve kabul kriteri | karar gerekçeleri (ADR'ın işi) | `GATE-004a`, hard stop |
| 4 | `ROLE.md` / `AGENTS.md` | roller, dağıtım, kutu, strike | kod kuralları | ajan, devir, bütçe |
| 5 | `MEMORY.md` | oturumlar arası kalıcı notlar | SSOT belgeleri | hafıza, kaldığım yer |
| 6 | `VISION.md` | 7 ekran ve farklılaştırıcılar | proje ve risk listesi | ekran, wireframe, tema |
| 7 | `PROJECTS.md` | envanter ve risk defteri | karar özeti | 9 proje, K3, 5440 |
| 8 | `brain.md` | ADR özeti + kod kuralları | ekran ve tasarım | `Result<T>`, SOLID, timeout |
| 9 | `keys.md` | yalnız gösterir (yönlendirir) | cevabın kendisi | kelime → dosya |
| 10 | `glossary.md` | terim tanımı | karar gerekçesi | TR/EN, ELI10 |
| 11 | `log.md` / `engine.md` | günlük ve orkestrasyon notları | mimari kararlar | Quality Report, checkpoint |
| 12 | `architecture/index.md` | katman, düğüm ve katalog haritası | gate ve süreç | `N` kodu, `l0..l7` |

### §2.3 Klasör → sık anahtar eşlemesi

| # | Klasör | Sık geçen anahtarlar | Örnek soru | İlk satır |
|---|--------|----------------------|------------|-----------|
| 1 | `.ai/` (kök) | gate · rol · anayasa · hafıza | "Sıradaki adım ne?" | §2 satır 8 |
| 2 | `.decisions/` | ADR · alternatif · supersede | "Bu neden böyle kararlaştı?" | §2.1 |
| 3 | `.rules/` | kural · ihlal · RED | "Kod kuralları ne diyor?" | §2 satır 5 |
| 4 | `.templates/` | şablon · frontmatter · boyut | "Bu dosya hangi şablonla?" | §1 satır 55 |
| 5 | `spec/` | davranış · test · bildirim | "Test neyi test eder?" | §1 satır 66 |
| 6 | `architecture/` | düğüm · `N` kodu · katman | "Dosya hangi katmanda?" | §1 satır 27 |
| 7 | `checklists/` | 13 soru · kontrol · kanıt | "Sınır nasıl denetlenir?" | §1 satır 62 |
| 8 | `reports/` | ölçüm · ham envanter · teslim | "Sayı nerede ölçüldü?" | §1 satır 35 |
| 9 | `scripts/` | measure · check-links · count | "Sayıyı nasıl alırım?" | §1 satır 58 |
| 10 | `ui-design/` | wireframe · ekran · onay | "Tasarım onayı nerede?" | §1 satır 40 |
| 11 | `playbooks/` | adım · sıra · rol | "Adımlar hangi sırayla?" | §1 satır 51 |
| 12 | `prompts/` | prompt · şablon · varyasyon | "Prompt nerede başlar?" | §2 satır 5 |
| 13 | `knowledge/` | bilgi notu · ders · tekrar | "Bu daha önce yazıldı mı?" | §2 satır 7 |
| 14 | `.sql/` | migration · `schema_version` · checksum | "Şema nasıl değişir?" | §1 satır 5 |

### §2.4 Sık ikilemler (hangisi kazanır — karar tablosu)

| # | İkilem | Kazanan | Kaybeden | Gerekçe |
|---|--------|---------|----------|---------|
| 1 | `brain.md` özeti vs ADR metni | ADR metni | tek satır özettir, tartışma taşımaz | çelişkide asıl kayıt kazanır |
| 2 | `keys.md` vs hedef dosya | hedef dosya | bu dosya yalnız gösterir | cevap burada yasayamaz |
| 3 | `glossary.md` vs kod yorumu | `glossary.md` | yorumdaki tanım ikinci kaynaktır | terim tekrarı aramayı kırar |
| 4 | `.rules/index.md` vs kod | `.rules/index.md` | kod kuralı uygular, tanımlamaz | kural metni tek yerde durur |
| 5 | `PROJECTS.md` vs `VISION.md` | ikisi de, ayrı soruda | kapsam ile vizyon karıştırılırsa ikisi de yanlış okunur | VISION "neden", PROJECTS "ne" der |
| 6 | `WORKFLOW.md` vs `AGENTS.md` | kendi sorusunda | kapı tanımı ajan kutusunda tekrarlanamaz | süreç ile dağıtım ayrı SSOT'tur |
| 7 | Sahipli dosya vs tahmin | sahipli dosya | tahminle yazılmış hedef bozuk link üretir | envanter her tahmini ezer |
| 8 | Ölçülen sayı vs hatırlanan sayı | script çıktısı | elle söylenen sayı kanıt değildir | sayı hatırlanmaz, ölçülür |
| 9 | `index.md` haritası vs klasör gezinmesi | harita | dosya adı tahmini süre kaybıdır | harita 15 klasörü de kapsar |
| 10 | Bu tablo §1 vs §2.3 klasör eşlemesi | §1 (kelime netse) | klasör eşlemesi kelimeye karar verilemediğinde kullanılır | çakışmada kelime kazanır |

---

## §3 6 Sık Hata

| # | Hata | Neden | Doğru yön | Dosya |
|---|------|-------|-----------|-------|
| 1 | Soruyu bu dosyada çözmeye çalışmak | `keys.md` yalnız **gösterir**, cevabı taşımaz | satırı bul, hedef dosyayı aç, § numarasına in | §1 |
| 2 | Özet ile aslı arasında ikilik çıkarmak | `brain.md` §1 tek satır özettir; tartışma metni ADR'nin kendisidir | çelişki varsa asıl ADR kazanır | `.decisions/adr-index.md` |
| 3 | Aynı bilgiyi ikinci dosyaya kopyalamak | ikinci liste bir gün yalan söyler (ADR-021 ihlali) | bilgiyi tek SSOT'ta tut, buradan link ver | `brain.md` §1 (ADR-021) |
| 4 | Anahtarı ezberlemek | harita değişir, ezber kalmaz; satır numarası kaynak değildir | kelimeyi her seferinde tabloda ara | §1 |
| 5 | Dosya adını tahminle yazmak | tahminle yazılan hedef bozuk link üretir (G4) | envantere bak, listede yoksa kayıt aç | `index.md` §2 |
| 6 | Kanıtsız "buldum" demek | bulunmayan satır da kanıtsız sayılır | bulunamadıysa `log.md`'ye kayıt aç ve `architect` bekle | `log.md` |

### §3.1 Arama ipuçları (VS Code / terminal — 5 kalıp)

| # | Soru | Kalıp | Araç | Ne bulur |
|---|------|-------|------|----------|
| 1 | "Bu kelime hangi dosyada geçiyor?" | `GATE-004a` — düz metin, **regex KAPALI** | VS Code `Ctrl+Shift+F` | kelimenin geçtiği tüm dosyalar; nokta kaçışı gerekmez |
| 2 | "Dosya adını hatırlamıyorum ama içinde var" | `layer-boundary-check` — düz metin, **regex KAPALI** | VS Code dosya adı araması | adı kısmen bilinen tek dosya |
| 3 | "Bu dosyanın bölümleri ne?" | `^## §[0-9]+ ` — **regex AÇIK** | VS Code `Ctrl+Shift+F` + regex | açılış satırları ve § sırası |
| 4 | "Hangi dosya bu ADR'ye dayanıyor?" | `ADR-028` — düz metin, **regex KAPALI** | VS Code / `Select-String` | kararı referans alan belgeler |
| 5 | "Kanıtsız iddia var mı, mojibake izi var mı?" | `DOĞRULAMA GEREKLİ` — **regex AÇIK** | terminal: `pwsh -File scripts/check-links.ps1` · `pwsh -File scripts/find-mojibake.ps1` | etiketli maddeler, bozuk link, bozuk byte |

**Kalıp kuralı:** önce regex'siz (düz metin) dene; sonuç çoksa `^`/`$`/grup ile daralt. Tarama sonucunu dosyaya **yazma** — sayı `log.md`'ye kanıt olarak düşülür.

### §3.2 Hata modları (bu dosya bozulursa ne kaybolur)

| # | Hata modu | Belirti | Sonuç | Düzeltme |
|---|-----------|---------|-------|----------|
| 1 | Sütunlardan biri kırpılmış | "Ne cevaplar" ya da "Ne zaman açılır" boş | kelime bulunur ama okuma yeri bilinmez, dosya yeniden taranır | satırı 5 sütuna tamamla |
| 2 | Ölü hedef yazılmış | satırda adı geçen dosya silinmiş | bozuk link üretir (G4) | hedefi envanterden çıkar ya da `index.md` §2'ye ekle |
| 3 | Aynı kelime iki satırda | arama iki sonuç döndürür | okuyucu ikisini de açar, SSOT zedelenir | iki satırı birleştir |
| 4 | Satır numaraları kaymış | §4.1 testindeki referans yanlış satırı gösterir | hız testi yanlış hedefi doğru sanır | tablo sonuna ekle, ortaya ekleme |
| 5 | Cevap bu dosyaya kaçmış | §1 hücresinde tanım cümlesi var | sözlük (`glossary.md`) işlevsizleşir | hücreyi konu adıyla sınırla |
| 6 | Grup aralığı tutmuyor | §1.1 aralığı ile §1 satır sayısı uyuşmuyor | grup okuması yanlış dosyaya götürür | aralığı son satır numarasıyla eşitle |
| 7 | Uydurma terim girmiş | kelime vault'ta hiç geçmiyor | arama sonuçsuz döner, kullanıcı dosya tarar | satırı sil, `log.md`'ye not et |
| 8 | Emoji / Unicode kutu | taramada `U+2500-U+257F` veya emoji | ölçü scripti RED üretir | `|`, `-`, `+` ile yeniden çiz |

**Hata modu kuralı:** iki satırda belirtilen bozulma fark edilirse bu dosyanın genişletilmesi durur; önce §6 doğrulaması geçilir, sonra içerik eklenir.

---

## §4 Kullanım Örnekleri (3 senaryo)

| # | Senaryo | 1. adım | 2. adım | Kapanış |
|---|---------|---------|---------|---------|
| A | "DevExpress sürümü kaç?" | §1 satır 9 → `brain.md` §1 (ADR-004) | cevap: **v26.1**, ribbon yalnız `Office2010` | ikinci dosya: `PROJECTS.md` §2 (yalnız gerekirse) |
| B | "Bu dosya hangi katmanda?" | §1 satır 25 → `VISION.md` §2 + `PROJECTS.md` §3 | `checklists/layer-boundary-check.md` (13 soru) | karar ve gerekçe tek yerde kalır |
| C | "Test neden geçmiyor?" | §1 satır 65 → `brain.md` §3 kural 3 | sebep arıyorsan `brain.md` §4.1 RED tablosu | şüpheli K3/CPM ise `PROJECTS.md` §4 (R1) |

**Kural:** her senaryo **en fazla 2 dosya** açar. Üçüncü dosya gerekiyorsa §1'de yanlış kelime arıyor olabilirsin; kelimeyi grup tablosu (§1.1) ile yeniden seç.

### §4.1 Hız testi (beş satır — kendini dene)

| # | Soru | Doğru satır | Beklenen hedef | Süre |
|---|------|-------------|----------------|------|
| 1 | "CPM nerede?" | §1 satır 20 | `brain.md` §1 (ADR-014) | 3 sn |
| 2 | "3-STRIKE nerede?" | §1 satır 47 | `AGENTS.md` §4 | 3 sn |
| 3 | "Neden EF yok?" | §2.1 satır 2 | `brain.md` §1 (ADR-005/016) | 5 sn |
| 4 | "618 ne anlama geliyor?" | §1 satır 34 | `PROJECTS.md` §4 (R1) | 3 sn |
| 5 | "Gate 005 kabul kriteri?" | §1 satır 42 | `WORKFLOW.md` §2.4 | 5 sn |

**Test kuralı:** beş sorudan biri 10 saniyeden uzun sürerse tablo değil, o satırın kelimesi düzeltilir; düzeltme `log.md`'ye tek satır yazılır.

### §4.2 Kapsam dışı — bu dosya hangi soruyu CEVAPLAMAZ

Kapsam dışı satırı, bilgi eksikliği değil yönlendirme borcudur; her satır cevabın yaşadığı tek dosyayı da yazar.

| # | Bu dosyada OLMAYAN soru | Cevabı nerede yasar | Neden burada değil |
|---|-------------------------|---------------------|--------------------|
| 1 | "Bu terim tam olarak ne demek?" | `glossary.md` | tanım taşımak bu dosyayı sözlüğe çevirir |
| 2 | "Bu gate'in kabul kriteri hangi madde?" | `WORKFLOW.md` | kriter metni burada kopyalanırsa iki gerçek oluşur |
| 3 | "Bu karar neden alındı, alternatifi neydi?" | `.decisions/adr-index.md` | gerekçe ADR'nin içindedir |
| 4 | "Hangi kuralın tam metni?" | `.rules/index.md` | kural metni tekrar edilemez |
| 5 | "Bu iş hangi ajana gider?" | `AGENTS.md` | dağıtım matrisi ayrı SSOT'tur |
| 6 | "Simdi hangi kapıdayım?" | `WORKFLOW.md` | kapı durumu anlıktır, harita taşımaz |
| 7 | "Önceki oturumda nerede kalmıştım?" | `MEMORY.md` | kalıcı durum burada tutulursa iki bellek olur |
| 8 | "Vault'ta kaç dosya var, bozuk link kaç?" | `index.md` §4 | canlı sayı komutla alınır, tabloya yazılmaz |
| 9 | "Dosya hangi katmanda?" | `architecture/index.md` | `N` kodu eşlemesi katalogdadır |
| 10 | "Risk ne, neden yapmıyoruz?" | `PROJECTS.md` §4–§5 | risk defteri kendi dosyasında yaşar |
| 11 | "Uzun iş nasıl bölünür?" | `ULTRA-THINKING.md` | protokol burada tekrar yazılamaz |
| 12 | "Hangi dosya hangi şablonla üretilir?" | `.templates/index.md` | şablon envanteri ayrı envanterdir |

**Kapsam dışı güncelleme kuralı:** cevabın taşındığı dosya silinmişse satır silinmez, `DOĞRULAMA GEREKLİ` etiketiyle işaretlenir ve `architect` kararı beklenir.

---

## §5 İlgili Dosyalar

| # | Dosya | Ne için açılır | Buradaki karşılığı |
|---|-------|----------------|--------------------|
| 1 | `index.md` | vault girişi ve harita (bu dosyanın yedeği) | §2.2, §3.5 |
| 2 | `glossary.md` | terim karşılıkları (TR \| EN \| ELI10) | §1'e eklenen her yeni terim |
| 3 | `brain.md` | ADR özeti ve sert kod kuralları | §1 satır 1–8, 63–72 |
| 4 | `VISION.md` | ekranlar ve farklılaştırıcılar | §1 satır 13–16, 79–84 |
| 5 | `PROJECTS.md` | envanter, riskler, kapsam dışı | §1 satır 21–35 |
| 6 | `CLAUDE.md` | anayasa ve etiketler | §1 satır 59–60, 87 |
| 7 | `WORKFLOW.md` | gate zinciri ve kabul kriteri | §1 satır 38–43, §2 satır 8 |
| 8 | `AGENTS.md` | ajan matrisi, kutu, strike, bütçe | §1 satır 44–52, 85 |
| 9 | `MEMORY.md` | oturumlar arası durum | §1 satır 73–74 |
| 10 | `architecture/index.md` | katman ve katalog haritası | §1 satır 25–36 |
| 11 | `.decisions/adr-index.md` | 29 ADR'nin tam listesi | §2.1 |
| 12 | `spec/test-strategy.md` | test türleri ve kapsam beklentisi | §1 satır 66 |

---

| ROLE | [ROLE](ROLE.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| VISION | [VISION](VISION.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](PROJECTS.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| ULTRA | [ULTRA](ULTRA-THINKING.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.decisions/draft/ADR-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../README.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../CLAUDE.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../AGENTS.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../WORKFLOW.md) | kok kapi ozeti (gate sirasi tek satir) |

## §6 Doğrulama

| # | Kontrol | Hedef | Nasıl bakılır |
|---|---------|-------|---------------|
| 1 | Frontmatter | 8 alan, `authority: SSOT`, `updated: 2026-10-07` | ilk satır `---`, 8. satır `---` |
| 2 | Boyut | 30–88 KB bandında | `(Get-Item .ai/keys.md).Length/1KB` — ölçüm yazılır, tahmin değil |
| 3 | Tablo bütünlüğü | §1'de 101 satır, her satır 5 sütun dolu | satır sayımı; boş sütun = o kelime cevapsız demektir |
| 4 | Link | Yalnız mevcut dosyalara | `pwsh -File scripts/check-links.ps1` → `BOZUK = 0` |
| 5 | Karakter | Emoji yok, Unicode kutu çizimi yok, mojibake yok | `pwsh -File scripts/find-mojibake.ps1` → `0` |
| 6 | Uydurma | Kaynaksız sayı veya terim yok | kanıtsız madde `DOĞRULAMA GEREKLİ` etiketi alır |
