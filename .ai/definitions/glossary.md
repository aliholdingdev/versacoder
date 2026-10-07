---
title: "glossary.md — Terimler Sözlüğü (TR | EN | ELI10)"
type: reference
category: ".ai"
status: active
version: 3.0.0
updated: 2026-10-07
authority: SSOT
scope: "Vault'ta geçen terimlerin TR/EN/ELI10 tanımları (124 terim), sık karışan çiftler, RED listesi"
---

# glossary.md — Terimler Sözlüğü

**Zorunlu Bağlantılar:** [index](../index.md) · [keys](../governance/keys.md) · [brain](../.decisions/brain.md) · [CLAUDE](../claude.md) · [WORKFLOW](../governance/workflow.md) · [VISION](vision.md) · [PROJECTS](projects.md) · [AGENTS](agents.md) · [spec/index](../spec/index.md) · [.rules/index](../.rules/index.md)

> **Tek cümlelik sorumluluk:** Vault'ta geçen her terimi Türkçe karşılığı ve "10 yaşındaki çocuğa anlatır gibi" tek cümlele tanımlayıp hangi dosyaya bağlandığını yazmak.

## §0 Özet (Answer First)

| # | Soru | Cevap |
|---|------|-------|
| 1 | Bu dosya ne yapar? | §1 tablosunda terimi bulursun: TR karşılığı, EN karşılığı, tek cümlelik ELI10 tanımı ve bağlı dosya/ADR birlikte durur. |
| 2 | Kaç terim var? | §1'de **136 terim** (64 temel + 72 vault/mimari terimi); sayı, tablonun satır sayısından okunur. |
| 3 | Yeni terim geldiğinde ne olur? | Önce bu tabloya satır açılır, sonra dosyalarda kullanılır; uydurma sözcük yerine tabloya ekleme yapılır. |
| 4 | Terim ile karar aynı şey mi? | Hayır: **tanım** burada yaşar, **karar gerekçesi** `brain.md` §1 ve `.decisions/` altındaki ADR'dedir. |
| 5 | Tabloda yoksa? | `keys.md` §1'de kelimeyi ara; o da yoksa `index.md`'den gir ve `log.md`'ye kayıt aç. |

**Tek satırlık kural:** aynı kelime iki farklı dosyada iki farklı anlamda kullanılamaz — anlam değişiyorsa önce bu tablo değişir, sonra dosyalar yazılır.

---

## §1 Terimler Tablosu (136 terim)

Sütunlar: TR karşılığı · EN karşılığı · ELI10 tek cümle · terimin nerede kullanıldığı (bağlı dosya/ADR). Tanım uzunluk değil, **tek cümle** olmak zorundadır; iki cümleye düşen tanım ikiye bölünür.

| # | TR | EN | ELI10 (tek cümle) | Bağlı dosya / ADR |
|---|----|----|-------------------|-------------------|
| 1 | Kasa / belge deposu | Vault | Bütün proje bilgilerinin saklandığı tek kilitli dolaptır; ne eklersen orada durur ve oradan okunur. | `index.md` |
| 2 | Tek doğru kaynak | SSOT (Single Source of Truth) | Bir bilginin tek bir dosyada yaşamasıdır; aynı bilgi ikinci yerde yazılıysa bir gün yalan söyler. | `brain.md` §1 (ADR-021) |
| 3 | Kapı / denetim noktası | Gate | Yoldaki kontrol noktasıdır; yeşil ışık yanmadan bir sonraki adıma geçemezsin. | `WORKFLOW.md` §2 |
| 4 | Kütüphane dosyası | DLL | Programın parça parça saklandığı kutudur; exe onları çalışırken açar. | `PROJECTS.md` §4 |
| 5 | Katman | Layer | Katmanlı pastadır: her katman yalnız altındaki katmana değer, üstüne çıkmaz. | `VISION.md` §2 |
| 6 | Ajanlı çalışma | Agentic | Yapay zekânın sadece cevap değil, araç kullanıp işi kendisi bitirdiği çalışma biçimidir. | `AGENTS.md` §1 |
| 7 | Oturum | Session | Sohbeti başlattığın andan kapanana kadar geçen konuşma ve iş akışının tamamıdır. | `brain.md` §1 (ADR-007) |
| 8 | Sihirbaz | Wizard | Elini tutarak adım adım gören kurulum ekranıdır. | `VISION.md` §4 |
| 9 | Model Context Protocol | MCP | Yapay zekâ asistanına ekstra araç bağlayan ortak fiş ve duvar standardıdır. | `brain.md` §1 (ADR-006) |
| 10 | Katman / kademe | Tier | Aynı şeyin farklı kalitede kademeleridir (ör. ücretsiz → yerel → güçlü model). | `reports/k3-618-ham-envanter.md` |
| 11 | Düğüm | Node | Mimari ağaçtaki her bir kutudur; VersaCoder'da 52 tane vardır. | `architecture/index.md` |
| 12 | Bileşen | Component | Sayılabilir en küçük kod parçasıdır (dosya, sınıf, metot); katalogda kaydı tutulan birim budur. | `architecture/index.md` |
| 13 | Kum havuzu | Sandbox | Çocuğun oynadığı sınırlı alandır; dışarı çıkamaz, zarar veremez — araç çalıştırma da böyledir. | `brain.md` §1 (ADR-009) |
| 14 | Kapı / uç | Port | Bir hizmetin bağlandığı numaralı kapıdır (ör. 8080); yanlış numara gidilmeyen eve denk gelir. | `architecture/index.md` |
| 15 | Uyarlayıcı | Adapter | Farklı iki fişi birbirine bağlayan çeviricidir; aynı işi farklı sistemlere yaptırır. | `brain.md` §2 (ADR-011) |
| 16 | Sonuç sarmalayıcısı | `Result<T>` | "Başardım + değer" ya da "Başaramadım + sebep" diyen kutudur; exception fırlatmak yerine bu döner. | `brain.md` §3 kural 1 |
| 17 | Mimari test kütüphanesi | NetArchTest | Kodun kurallara uyduğunu (ör. katman tersine bakıyor mu?) kodu okuyarak otomatik denetleyen test paketidir. | `checklists/layer-boundary-check.md` |
| 18 | Merkezî paket yönetimi | CPM (Central Package Management) | Bütün paket sürüm numaralarının tek dosyada tutulmasıdır; her proje kendi sürümünü yazamaz. | `brain.md` §1 (ADR-014) |
| 19 | Hedef çerçeve | TFM (Target Framework) | Kodun hangi .NET sürümünde derleneceğini söyleyen etikettir (ör. `net48`). | `brain.md` §1 (ADR-001) |
| 20 | Çift bacak / iki bacak | Dual target (dual TFM) | Aynı kodu hem eski (`net48`) hem yeni (`net10.0-windows`) dünyada derleyip test etmektir. | `PROJECTS.md` §2.1 |
| 21 | Dosya başlığı bilgisi | Frontmatter | `.md` dosyasının en üstündeki `---` arası 8 alanlık künyedir; alanı eksikse dosya vault'a ait değildir. | `.templates/new-doc-contract.md` |
| 22 | Mimari karar kaydı | ADR (Architecture Decision Record) | "Bunu şu sebeple böyle kararlaştırdık" diyen, tarihli ve numaralı tek sayfalık kayıttır. | `.decisions/adr-index.md` |
| 23 | Taslak çizim | Wireframe | Ekranın kutu çizgilerle, renksiz iskelet hâlidir; kod yazılmadan önce onaylanır. | `VISION.md` §5 |
| 24 | Konuşma kırpma | Compaction | Uzar diyerek sohbeti silmek değil; son 20 tur + özet kalır, gerisi arşive taşınır. | `brain.md` §1 (ADR-008) |
| 25 | Konuşma kaydı | Transcript | Oturumda olan bitenin ham yazılmış hâlidir; hafızanın ham verisi budur. | `MEMORY.md` |
| 26 | Yedek zincir | Fallback | Bir model çalışmazsa sıradakine düşme zinciridir (ücretsiz → yerel → uyumlu uç). | `brain.md` §1 (ADR-011) |
| 27 | Sağlayıcı | Provider | Hizmeti veren taraftır; LLM sağlayıcısı, modelin sunulduğu şirkettir. | `VISION.md` §4 |
| 28 | Akış | Streaming (SSE) | Cevabın parça parça, yazıldıkça gelmesidir; bitmesini beklemek yoktur. | `brain.md` §1 (ADR-011) |
| 29 | Boş denetimi | Nullable enable | Derleyiciye "null gelebilecek yerleri işaretle" demektir; `null` sürprizi kod yazmadan belli olur. | `brain.md` §3 kural 4 |
| 30 | Örtüşme oranı | Coverage | Testlerin kodun yüzde kaçını gerçekten dolaştığının ölçüsüdür; VersaCoder hedefi %90+. | `spec/test-strategy.md` |
| 31 | Şema taşıma | Migration | Veritabanı tablosunu güvenle değiştirme dosyasıdır; saf SQL + `schema_version` tablosu. | `brain.md` §1 (ADR-005) |
| 32 | Parametreli sorgu | Parameterized SQL | Sorguya değerleri değil, yer tutucuyu (`@p1`) göndermektir; hırsızın kapı kodunu değiştirmesi gibi. | `brain.md` §3 kural 5 |
| 33 | Hizmet bağımlılığı enjeksiyonu | DI (Dependency Injection) | Sınıfın kendi ihtiyacını kendine kurması yerine dışarıdan "şunu kullan" diye alınmasıdır. | `brain.md` §2 |
| 34 | Tek sorumluluk ilkesi | SRP (Single Responsibility) | Her dosyanın ve sınıfın tek bir işi olmalıdır; iki iş, iki dosya demektir. | `brain.md` §2 |
| 35 | Arayüz tersine çevrilmiş | DIP (Dependency Inversion) | Üst katman, alt katmanın somut sınıfını değil, kendi çizdiği arayüzü tanır. | `brain.md` §2 |
| 36 | Çözüm dosyası | `.slnx` | Projeleri listeleyen yeni nesil çözüm dosyasıdır; eski `.sln`'in XML'li hâli. | `PROJECTS.md` §1 |
| 37 | Sürümlü kayıt defteri | Changelog | Sürüm sürüm ne değiştiğini tarih sırasıyla listeleyen dosyadır. | `PROJECTS.md` |
| 38 | Mimari atlas | Architecture atlas | 52 düğümün ve bileşen kayıtlarının yaşadığı haritadır (`architecture/` dizini). | `architecture/index.md` |
| 39 | İş akışı | Workflow | Kapıdan kapıya (gate → gate) izlenen adım sırasıdır. | `WORKFLOW.md` |
| 40 | Kod incelemeci (rol) | Reviewer | Kodu kurallara göre denetleyen, RED/GREEN veren ajan rolüdür. | `AGENTS.md` §1 |
| 41 | Zorunlu bağlantı | Mandatory link | Her belgenin üstünde bulunması zorunlu, birbirine bağlayan standart link satırıdır. | `.templates/new-doc-contract.md` |
| 42 | Bozuk link | Broken link | Gitmediği yere gösteren bağlantıdur; vault kuralında sayısı sıfır olmalıdır. | `index.md` §4 |
| 43 | Serbest kaynak modeli | Free-model router | Ücretsiz modelleri sırayla deneyen, hiçbiri yoksa düşen yönlendiricidir. | `VISION.md` §4 |
| 44 | Uçtan uca test | E2E test | Uygulamayı kullanıcı gibi baştan sona kullanarak doğrulayan testtir. | `spec/test-strategy.md` |
| 45 | Kod incelemesi | Code review | Kodu bir gözden geçirme işidir; VersaCoder'da `reviewer` rolü RED/GREEN verir. | `AGENTS.md` §1 |
| 46 | Kabul kriteri | Acceptance criteria | "Bitti" demeden önce karşılanması gereken, ölçülebilir maddeler listesidir. | `WORKFLOW.md` §2 |
| 47 | Geri alma | Reversibility | Bir kararı geri döndürmenin maliyetidir; kolay dönülebilir kararlar tercih edilir. | `brain.md` §4 |
| 48 | Risk azaltıcı | Mitigation | Bir riske karşı önceden alınan tedbirdir (ör. spike, fallback, kapı). | `PROJECTS.md` §4 |
| 49 | Kanıt | Evidence | "Yaptım" demek yerine gösterilen çıktıdır (script çıktısı, test log'u, satır no). | `CLAUDE.md` G5 |
| 50 | Tarama | Scan | Depoyu veya çıktıyı otomatik aramaktır (bozuk link, mojibake, kayıt sayısı). | `index.md` §4 |
| 51 | Mojibake | Mojibake (encoding bug) | Türkçe karakterlerin bozuk görünmesidir; UTF-8 dışında kayıt hata sayılır. | `reports/index.md` |
| 52 | Kod haritası | Code map | Hangi modülün nerede olduğunu gösteren basit liste veya diyagramdır. | `architecture/index.md` |
| 53 | Hata modu | Failure mode | Bir şeyin ne şekilde bozulabileceği ve sonucudur; her kararın listesi olur. | `brain.md` §4 |
| 54 | Yeniden deneme | Retry | Aynı işi tekrar denemektir; sınırsız değil, geri çekilme (backoff) ile yapılır. | `.rules/index.md` |
| 55 | Özellik bayrağı | Feature flag | Kodu kapalı tutan anahtardır; yayından önce denemeyi güvenli yapar. | `.rules/index.md` |
| 56 | Teslim | Delivery | Değişikliğin gereken yere tamamlanmış şekilde ulaşmasıdır. | `WORKFLOW.md` §2.5 |
| 57 | Aşama | Phase | İşin içinde bulunduğu numaralı kutudur (vault kurulum, kod, test…). | `WORKFLOW.md` §1 |
| 58 | İş kuralı | Business rule | "Şöyle olmalı" diyen kuraldır; kodda Domain katmanında yaşar. | `architecture/index.md` |
| 59 | Değer objesi | Value object | Sadece değeriyle tanımlanan küçük nesnedir (para, kimlik); birbiriyle eşitse aynıdır. | `architecture/index.md` |
| 60 | Varlık | Entity | Kimliği olan ve değişebilen nesnedir (ör. bir dosya kaydı). | `architecture/index.md` |
| 61 | Yaşam döngüsü | Lifetime | Bir nesnenin ne kadar yaşadığıdır (transient / scoped / singleton). | `brain.md` §2 |
| 62 | Zaman aşımı | Timeout | İşin en fazla ne kadar süreceğidir; aşarsa iptal edilir (ADR-009: 60 sn, bash 300 sn). | `brain.md` §1 (ADR-009) |
| 63 | İptal yayılımı | Cancellation propagation | "Dur" denince zincirdeki her işin de durmasıdır. | `brain.md` §1 (ADR-009) |
| 64 | Performans ölçümü | Benchmark | Kodun gerçekten hızlı olduğunu sayılarla kanıtlayan ölçümdür. | `.rules/index.md` |
| 65 | Yetki alanı | Authority (frontmatter) | Künyedeki bu satır, dosyanın o konudaki tek doğru kaynak olduğunu ilan eder. | `.templates/new-doc-contract.md` |
| 66 | Kanıt kutusu | Evidence box | Gate'in dört satırlı kutusudur: `KANIT` · `DOSYA` · `EKSIK` · `SONRAKI`. | `WORKFLOW.md` §2 |
| 67 | Ajan kutusu | Agent output box | Ajanın beş alanlı teslim kutusudur: `BASLIK` · `KAPSAM` · `KARAR` · `RİSK` · `KANIT`. | `AGENTS.md` §2 |
| 68 | Devir paketi | Handover package | Bir ajan işi başka ajana verirken doldurması zorunlu dört maddedir: kalan iş, dosya, kanıt, sınır. | `AGENTS.md` §3 |
| 69 | Strike | 3-STRIKE counter | Aynı işin aynı yöntemle üç kez düşmesidir; üçüncüden sonra denemeye devam edilmez. | `AGENTS.md` §4 |
| 70 | Bütçe | Budget | Çıktı, dosya ve soru için konan token/byte sınırıdır; aşınca iş küçülür, ajan değişmez. | `AGENTS.md` §6 |
| 71 | Toplu dağıtım | Batch | Aynı anda çağrılan ajan grubudur; bu vault'ta üçerli çalıştırılır. | `AGENTS.md` §5.1 |
| 72 | Eşzamanlılık sınırı | Concurrency limit | Aynı anda çalışabilecek en fazla ajan sayısıdır ve sağlayıcı limitiyle belirlenir. | `AGENTS.md` §5 |
| 73 | Kontrol durağı | Checkpoint | Kanıtın gate kriteriyle birebir karşılaştırıldığı duraktır; "neredeyse geçti" diye durum yoktur. | `engine.md` §1.5 |
| 74 | Anlık görüntü | Snapshot | Bir anki durumun dondurulmuş kopyasıdır (ör. oturumun sağlayıcı/model seçimi). | `spec/index.md` |
| 75 | Çapraz sorgu | N+1 query | Döngü içinde her satır için ayrı sorgu atmaktır; tek JOIN ile değiştirilir. | `.rules/index.md` |
| 76 | Aynı sonuçlu işlem | Idempotent | Aynı işlemin ikinci kez çalıştırılmasının zararsız olmasıdır; migration scriptleri böyle yazılır. | `spec/index.md` |
| 77 | Bileşim kökü | Composition root | Bağımlılıkların yalnızca tek noktada kurulduğu yerdir; başka yerde new'lenmez. | `WORKFLOW.md` §3 |
| 78 | Depo deseni | Repository pattern | Veriye erişimi ince bir arayüz arkasına saklar; sorgu detayı dışarı taşmaz. | `architecture/index.md` |
| 79 | Ömür ve düşürme | TTL / invalidation | Cache'in ne kadar yaşayacağını ve ne zaman düşürüleceğini yazan iki sayıdır. | `.rules/index.md` |
| 80 | Dosya özeti | Checksum | Dosyanın parmak izidir; uygulanmış bir migration sonradan değiştiyse özeti tutmaz ve sistem durur. | `brain.md` §1 (ADR-005) |
| 81 | Geri çekilme | Backoff | Yeniden denemeler arası bekleme süresinin artmasıdır; sabit aralık deneme sayısını bitirir. | `.rules/index.md` |
| 82 | Yapılandırılmış log | Structured log | Alan adları olan tek şablondur; "bir şey oldu" gibi serbest metin log yasaktır. | `brain.md` §3 kural 2 |
| 83 | Log seviyesi | Log level | Beş tanedir: `Trace`, `Debug`, `Info`, `Warn`, `Error`; dışı uydurulmaz. | `brain.md` §1 (ADR-026) |
| 84 | Hata kodu | Error code | `VC-E-NNNN` biçiminde üretilir; kullanıcıya insanca mesaj, sızdırmaya teşhis kimliği gösterir. | `spec/index.md` |
| 85 | Log yazma noktası | `ILogSink` | Toplanan logun düştüğü tek arayüzdür; l3 katmanında yaşar. | `spec/index.md` |
| 86 | Adlandırma | Naming convention | Tip, alan ve dosya adının kuralıdır; ad seçimi zevk meselesi değil denetim konusudur. | `.rules/index.md` |
| 87 | Erişilebilirlik | Accessibility | Klavyeyle gezilebilir ve ekran okuyucuyla okunabilir olma zorunluluğudur. | `.rules/index.md` |
| 88 | Tema / görünüm | Theme / skin | Uygulamanın renk ve stil paketidir; ayarlar ekranından seçilir, koda gömülmez. | `VISION.md` §3 (07) |
| 89 | Fark görünümü | Diff | Değişen satırları gösteren karşılaştırmadır; incelemenin somut kanıtıdır. | `AGENTS.md` §1 |
| 90 | Konsol yüzeyi | Terminal | Tek pencere içindeki komut satırıdır; ekranların yanında durur. | `VISION.md` §3 (02) |
| 91 | Görev kuyruğu | Task queue | Ajan işlerinin durumla birlikte sıralandığı listedir; 06 ekranında görünür. | `VISION.md` §3 (06) |
| 92 | Bildirim | Notification | Olayın kullanıcıya gösterilmesidir; toast ve birikimli hata merkezi bunun iki kanalıdır. | `spec/index.md` |
| 93 | Gözlemlenebilirlik | Observability | Sistemin içeriden nasıl görülebileceğidir: log, izleme ve ölçüm birlikte düşünülür. | `spec/index.md` |
| 94 | Metin tarama aracı | grep / glob | Biri içeriği, diğeri dosya adını arar; ikisi de ADR-006'da sözleşmeyle kilitlidir. | `brain.md` §1 (ADR-006) |
| 95 | Araç sözleşmesi | Tool contract | Aracın girdisi ve çıktısıdır: hata fırlatmaz, başarısızlığı `ToolResult` olarak değer döner. | `brain.md` §1 (ADR-006) |
| 96 | Kalıcılık | Persistence | Oturum ve ayarların diske yazılıp sonraki açılışta geri gelmesidir. | `MEMORY.md` |
| 97 | Arayüz kabuğu | UI shell | Ekranları barındıran ana penceredir; kendisi iş mantığı taşımaz. | `VISION.md` §3 (02) |
| 98 | Makro katman | Macro layer | `l0`..`l7` sekiz büyük kutudur; her biri tek bir `.csproj`'a karşılık gelir. | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 99 | Alt katman | Sub-layer | `N01`..`N44` klasör düzeyindeki bölünmedir; proje değil, klasör ve namespace'tir. | `architecture/index.md` |
| 100 | Düğüm kodu | Node id (`N##`) | Katalog satırını açığa çıkaran `N01` biçimindeki etikettir; sayım birimi de budur. | `architecture/index.md` |
| 101 | Bağımlılık oku | Dependency direction | İznin verildiği yön `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` biçiminde tek yönlüdür. | `checklists/layer-boundary-check.md` |
| 102 | Risk defteri | Risk register | Açık risklerin R1'den R5'e kadar numaralı tutulduğu defterdir. | `PROJECTS.md` §4 |
| 103 | Kapsam dışı | Out of scope | Bu sürümde yapılmayan işin ve gerekçesidir; listeye yazılmayan kapsam dağınıklıktır. | `PROJECTS.md` §5 |
| 104 | K3 kaynağı | K3 (mimari-02 prompt) | Rakip mimari taslağın adıdır; 618 DLL'lik listesi bu kaynaktan gelir. | `reports/k3-618-ham-envanter.md` |
| 105 | Ham envanter | Raw inventory | Elle sayılmış, temizlenmemiş listedir; kataloga girmez, `DOĞRULAMA GEREKLİ` ile yaşar. | `reports/k3-618-ham-envanter.md` |
| 106 | Marj | Margin | Eşiğin payıdır; 5440 kaydı ölçülenden alındığında marj sıfırdır, tek satır silinmesi RED üretir. | `PROJECTS.md` §4 (R3) |
| 107 | Ölçüm scripti | Measurement script | Sayıyı elle değil komutla üreten betiktir; dördü de `scripts/` altındadır. | `index.md` §4 |
| 108 | Kalite raporu | Quality Report | Bir işin bittiğinde `log.md`'ye düşülen tarihli ölçüm kaydıdır. | `log.md` |
| 109 | Doğrulama etiketi | Verification label | Kanıt durumunu tek kelimeyle söyleyen etikettir: doğrulanmış, çıkarım veya doğrulama gerekli. | `CLAUDE.md` |
| 110 | Kaynak doğrulanmış | `SOURCE_VERIFIED` | İddianın komut çıktısı veya açık kaynakla kanıtlandığını söyler. | `CLAUDE.md` |
| 111 | Çıkarım | `INFERRED` | Ölçülmemiş bir kestirim olduğunu ve öyle sunulması gerektiğini söyler. | `CLAUDE.md` |
| 112 | Doğrulama gerekli | `NEEDS_VALIDATION` / `DOĞRULAMA GEREKLİ` | Bilinmeyen maddenin etiketidir; etiketsiz devam etmek kanıtsız iddiadır. | `CLAUDE.md` |
| 113 | Durdurucu kapı | HARD STOP | Onay gelmeden tek satır kodun yazılmayacağı kapıdır; GATE-003 bu kapıdırl. | `WORKFLOW.md` §2.2 |
| 114 | İlk commit | First commit | Depoya atılan ilk kayıttır; yalnız vault yeşil ve build yeşilden sonra atılır. | `WORKFLOW.md` §2.3 |
| 115 | Sert boyut kapısı | Size band gate | Kök dosyanın 30 KB altında RED, 88 KB üstünde RED olma kuralıdır. | `index.md` §5.2 |
| 116 | Açık/kapalı ilkesi | Open-Closed | Yeni ekleme için kodu değil, yeni bir adaptörü yazarsın; mevcut zincir değişmez. | `brain.md` §2 |
| 117 | Arayüz ayrımı | Interface Segregation | Araç arayüzleri ince uçlardır; devasa bir interface yazmak yasaktır. | `brain.md` §2 |
| 118 | Bağımlılığın tersi | Dependency Inversion | Üst katman veritabanı bağlantısını tanımaz, onu saran arayüze bağlıdır. | `brain.md` §2 |
| 119 | Yer değiştirme | Liskov Substitution | İki bacak aynı arayüzü aynı davranışla sunar; tek bacak sapması RED sebebidir. | `brain.md` §2 |
| 120 | Dosya sahipliği | X ownership | Her dosyanın bir ajanı vardır; sahibi dışında kimse o dosyaya yazamaz. | `AGENTS.md` §7 |
| 121 | Tek yazıcı | Single writer | Aynı dosyaya iki ajan aynı anda yazamaz; çatışma varsa iş seriye alınır. | `AGENTS.md` §7 |
| 122 | Mimari test | Architecture test | Katman yönünü kod üzerinden deneyen testtir; 8x8 matrisle eşleşir. | `checklists/layer-boundary-check.md` |
| 123 | Sürüm tablosu | Version table | Migration dosyalarının sırasını ve uygulanma kaydını tutan tablodur. | `brain.md` §1 (ADR-005) |
| 124 | İptal belirteci | CancellationToken | "Dur" haberini zincir boyunca taşıyan nesnedir; araç çağrısı onunla dinler. | `brain.md` §1 (ADR-009) |
| 125 | Orkestratör | Orchestrator | İşi parçalayıp hangi ajanın ne yapacağını belirleyen koordinatördür; kodun kendisi değil yöneticisidir. | `engine.md` §1.1.4 |
| 126 | Harici ajan | External coding agent | VersaCoder'ın kendi dışındaki kod yazma aracıdır (OpenCode, Codex, Claude Code); yalnızca yürütme hizmetçisidir. | `AGENTS.md` §4 |
| 127 | Kontrol düzlemi | Control plane | VersaCoder'ın kendisi: niyet, oturum, kural, onay ve doğrulamayı yöneten kısım. | `AGENTS.md` §4.2 |
| 128 | Yürütme düzlemi | Execution plane | Kodu gerçekten yazan, testi koşturan dış ajan katmanıdır; kural koymaz, uygular. | `AGENTS.md` §4.2 |
| 129 | Otonomik özgürlük | Autonomy | Ajanın kurallar içinde kendi başına ilerleyebilme yeteneğidir; sınırsızlık değildir, sınır içinde özgür olmaktır. | `AGENTS.md` §7 |
| 130 | Niyet motoru | Intent engine | Kullanıcının yazdığı cümleyi ne istediğini anlamaya çalışan ilk aşamadır. | `engine.md` §1.1.1 |
| 131 | Stüdyo | Studio | Yalnız kod değil, dokümantasyon ve görsel üretimi de tek ortamda yapan genişletilmiş IDE'dir. | `VISION.md` §0.3 |
| 132 | Görsel üretim hattı | Visual production pipeline | Girdi görsel/fikirden alınır, analiz edilir, prompta dönüştürülür ve render edilir. | `AGENTS.md` §8.5 |
| 133 | Görsel direktör | visual-director | Render prompt'unu üreten ajan; lens/ışık/kompozisyon parametrelerini çıkarır, uydurmaz. | `AGENTS.md` §2 |
| 134 | Kimlik koruma | Identity preservation | Görsel üretimde yüz/beden/oranların kullanıcı istemedikçe değiştirilmemesidir. | `AGENTS.md` §8.5 |
| 135 | Görsel doğrulanmış kaynak | Visual source of truth | Referans görsel, ondan çıkarılan her bilginin birincil kaynağıdır. | `AGENTS.md` §8.5 |
| 136 | Adaptif soru motoru | Adaptive question engine | Cevaba göre bir sonraki soruyu belirleyen, aynı soruyu tekrarlamayan sorma döngüsüdür. | `AGENTS.md` §5.4 |

**İlgili (bu tablodaki terimlerin evleri):** [AGENTS](agents.md) · [CLAUDE](../claude.md) · [WORKFLOW](../governance/workflow.md) · [ROLE](role.md) · [brain](../.decisions/brain.md) · [VISION](vision.md) · [PROJECTS](projects.md) · [keys](../governance/keys.md) · [engine](engine.md) · [MEMORY](../memory/memory.md) · [index](../index.md).
### §1.1 Terim kümeleri (hangi küme nerede kullanılır)

| # | Küme | Terim aralığı | Kullanıldığı dosya | Ne zaman okunur |
|---|------|---------------|--------------------|-----------------|
| 1 | Vault ve belge kuralı | 1–4, 21–22, 41–42, 65 | `.templates/new-doc-contract.md` | yeni dosya üretilirken |
| 2 | Mimari ve katman | 5, 11–12, 14–15, 58–60, 98–101, 122 | `architecture/index.md` | dosya nereye denilince |
| 3 | Derleme ve paket | 18–20, 36 | `PROJECTS.md` §1–§2 | derleme veya paket hatasında |
| 4 | Veri erişim | 31–33, 75–80, 123 | `brain.md` §1 (ADR-005) | SQL veya migration yazılırken |
| 5 | Arayüz ve ekran | 8, 23, 43, 87–90, 92, 97 | `VISION.md` §3 | ekran işi başlarken |
| 6 | Ajan ve süreç | 6–7, 39–40, 45–47, 56–57, 67–73 | `AGENTS.md` | iş dağıtılırken |
| 7 | Kod kalitesi | 16–17, 29–30, 34–35, 54, 61–64, 81–86 | `brain.md` §3 | kod yazılırken |
| 8 | Ölçüm ve kanıt | 49–53, 106–115 | `index.md` §4 | sayı veya "bitti" denilirken |
| 9 | Sol dil ve prensip | 88, 116–121, 124 | `brain.md` §2 | inceleme talebi gelince |
| 10 | Kapsam ve risk | 37–38, 44, 48, 102–105 | `PROJECTS.md` §4–§5 | kapsam tartışılınca |

### §1.2 Sık kısaltmalar

| # | Kısaltma | Tam hâli | İlk nerede geçer |
|---|----------|----------|------------------|
| 1 | UI | User Interface (arayüz) | `VISION.md` §3 |
| 2 | CLI | Command Line Interface (konsol) | `PROJECTS.md` §1 |
| 3 | SSOT | Single Source of Truth | bu dosya (2) |
| 4 | ORM | Object-Relational Mapper (EF benzeri) | `PROJECTS.md` §5 (yasak) |
| 5 | SSE | Server-Sent Events (akış) | `brain.md` §1 (ADR-011) |
| 6 | TFM | Target Framework | bu dosya (19) |
| 7 | CPM | Central Package Management | bu dosya (18) |
| 8 | NRT | Nullable Reference Types | `.rules/index.md` |
| 9 | ADR | Architecture Decision Record | bu dosya (22) |
| 10 | DI | Dependency Injection | bu dosya (33) |
| 11 | SRP | Single Responsibility Principle | bu dosya (34) |
| 12 | DIP | Dependency Inversion Principle | bu dosya (35) |
| 13 | OWASP | Open Web Application Security Project | `.rules/index.md` |
| 14 | DPAPI | Data Protection API | `brain.md` §1 (ADR-018) |
| 15 | E2E | End-to-End | bu dosya (44) |
| 16 | TTL | Time To Live | bu dosya (79) |

**Kısaltma kuralı:** kısaltma ilk geçtiği yerde tam hâliyle yazılır; kısaltmayı yalnız bilen kişi yazarsa okurucu arama yapmak zorunda kalır ve harita işlevini kaybeder.

---

## §2 Kullanım Kuralları

| # | Kural | Kırılırsa ne olur | Doğrusu |
|---|-------|-------------------|---------|
| 1 | Terim önce bu tabloya girer, sonra dosyalarda kullanılır | aynı kelime iki dosyada farklı anlam taşır | tabloya satır aç, sonra yaz |
| 2 | Kısaltma ilk geçtiği yerde tam hâliyle yazılır | arama yalnız kısaltmayla yapılıp uzun adı kaçırır | "Hedef çerçeve (TFM)" biçiminde ikisi birden |
| 3 | TR karşılığı tutarlı kullanılır | `katman` / `layer` karışımı aramayı kırar | hep `katman`, hep `oturum`, hep `kapı (gate)` |
| 4 | Tanım ile karar ayrı dosyada yaşar | karar özeti sözlüğe kopyalanır, iki kaynak oluşur | tanım burada, gerekçe `brain.md` ve ADR'de |
| 5 | Yeni sözcük uydurulmaz | vault'a ait olmayan terim sozlüğü şişirir | karşılığı yoksa `keys.md` §1'e bak, yoksa `log.md`'ye sor |

### §2.1 Sık karışan terim çiftleri

| # | Terim A | Terim B | Fark (tek cümle) | Nerede karışır |
|---|---------|---------|------------------|----------------|
| 1 | Katman (layer) | Tier (kademe) | Katman kodun nereye konacağını, tier modelin hangi güçte olduğunu söyler. | `VISION.md` §2 vs `reports/k3-618-ham-envanter.md` |
| 2 | Gate | Gate numarası (`GATE-003`) | Gate kavramının kendisi kontroldür, numara o kontrolün adıdır. | `WORKFLOW.md` §2 |
| 3 | Kanıt kutusu | Ajan kutusu | Biri gate'in dört satırı, diğeri ajanın beş alanıdır; biri diğerinin yerine geçmez. | `WORKFLOW.md` §2 vs `AGENTS.md` §2 |
| 4 | Düğüm (node) | Bileşen (component) | Düğüm ağacıdaki kutudur, bileşen o kutunun içinde sayılan kod parçasıdır. | `architecture/index.md` |
| 5 | Katman (layer) | Proje (`.csproj`) | Sekiz makro katmanın her biri tek projedir; alt katmanlar proje değil klasördür. | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 6 | Migration | Seed verisi | Migration şemayı değiştirir, seed dolgu yapar; ikisi de saf SQL'dir ama işi farklıdır. | `spec/index.md` |
| 7 | Retry | Timeout | Biri işi tekrar dener, diğeri beklemeyi keser; ikisi aynı kapıda buluşmaz. | `.rules/index.md` |
| 8 | Log | Bildirim | Log sistem için, bildirim kullanıcı içindir; birinin kaydı diğerinin gösterimi değildir. | `spec/index.md` |
| 9 | SSOT | Sahiplik (ownership) | SSOT "bilgi tek yerde", sahiplik "o yeri kim yazar" der. | `brain.md` §1 (ADR-021) vs `AGENTS.md` §7 |
| 10 | Wireframe | Kod | Wireframe onaylanmadan kod yazılmaz; ikisi aynı kapıdan geçer ama farklı ürünlerdir. | `VISION.md` §5 vs `WORKFLOW.md` §2.2 |
| 11 | Çıkarım (`INFERRED`) | Doğrulanmış (`SOURCE_VERIFIED`) | Birinin ölçülmemiş kestirim, diğerinin komutla kanıtlanmış iddia olduğunu söyler. | `CLAUDE.md` |
| 12 | Sandbox | Port | Birisi çalıştırmanın güvenli alanı, diğeri hizmetin bağlandığı numaralı kapıdır. | `brain.md` §1 (ADR-009) vs `architecture/index.md` |
| 13 | Provider | Adapter | Sağlayıcı hizmeti veren taraftır, uyarlayıcı onu zincire bağlayan çeviri parçasıdır. | `VISION.md` §4 vs `brain.md` §2 |
| 14 | Kapsam dışı | Reddedilmiş (rejected) | Kapsam dışı "bilerek yapmıyoruz", reddedilmiş "tartışıldı ve kapıdan dönüldü" demektir. | `PROJECTS.md` §5 vs `.decisions/adr-index.md` |

### §2.2 Vault'a girmeyen terimler (RED listesi)

| # | Terim | Neden RED | Yerine ne yazılır | Kural |
|---|-------|-----------|-------------------|-------|
| 1 | EF Core · ORM · `DbContext` | iki bacağı taşıyamaz ve migration'ı sürüm tablosundan koparır | ADO.NET + Dapper | `CLAUDE.md` G2 |
| 2 | WPF · web arayüzü | ürün WinForms masaüstündedir | WinForms + DevExpress | `PROJECTS.md` §5 |
| 3 | "500+ DLL = proje" | ikinci sayım birimi 5440 kaydını anlamsızlaştırır | `N###` katalog kaydı | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` §2.2 |
| 4 | v26.1 dışı DevExpress sürümü | iki bacak doğrulaması yalnız bu sürümde yapılmıştır | DevExpress v26.1 | `.rules/index.md` |
| 5 | `Office2010` dışı ribbon stili | tek izinli stil budur, diğerleri RED | `RibbonControlStyle.Office2010` | `.rules/index.md` |

**RED listesi kuralı:** bu satırlardan biri kodda veya belgede geçiyorsa madde silinmez, etiketlenir ve `log.md`'ye kayıt açılır; karar mercii `architect`, onay mercii kullanıcıdır.

---

## §3 Örnek Sorgular (6 soru)

| # | Soru | 1. adım | 2. adım | Sonuç |
|---|------|---------|---------|-------|
| 1 | "Bu projede `tier` ne demek?" | §1 satır 10: kademe — ücretsiz → yerel → güçlü model | `brain.md` §1 (ADR-011) ve `VISION.md` §4 | model sırası ve yedek zinciri netleşir |
| 2 | "'Gate' ile 'GATE-003' aynı şey mi?" | §1 satır 3: gate = yeşil ışık yanmadan geçilmeyen nokta | §2.1 satır 2: numara o kontrolün adıdır | kavram ile numara ayrışır |
| 3 | "EF Core kullanabilir miyim?" | §2.2 satır 1: RED listesi | `brain.md` §1 (ADR-005/016) gerekçesi | ret cevabı tek satırda, gerekçesi ADR'de |
| 4 | "Bu terim daha önce tanımlandı mı?" | §1'de TR veya EN sütununda ara | yoksa §2 kural 5 → `log.md`'ye kayıt | tanımlanmamış terim tabloya girer |
| 5 | "Test neden bu davranışı bekliyor?" | §1 satır 30 (coverage) ve 44 (E2E) | `spec/test-strategy.md` | beklenen davranış spec'te, kod uygulamadır |
| 6 | "Kanıtım yoksa ne yazarım?" | §1 satır 109–112: etiket seçimi | `CLAUDE.md` G9 | `NEEDS_VALIDATION` + komut, madde kapanmaz |

**Soru kuralı:** cevap §1'de çıkıyorsa ikinci adıma geçme; çıkmıyorsa doğrudan "bağlı dosya" sütunundaki hedefe git — sözlük gezilmez, aranır.

### §3.1 Ters arama — dosyadan o dosyanın terimleri

Soru kelimeyle değil, dosyayla başlıyorsa ters arama yapılır: dosya açılır, §1.1 küme aralığındaki terimler tek seferde okunur.

| # | Dosya | O dosyada yaşayan terim kümesi | Ne zaman ters aranır |
|---|-------|--------------------------------|----------------------|
| 1 | `AGENTS.md` | 66–72, 120–121 (kutu, devir, strike, bütçe, sahiplik) | ajan işi dağıtılırken |
| 2 | `WORKFLOW.md` | 3, 46, 56–57, 113–114 (kapı, kabul, teslim, durdurucu kapı) | kapı kapanırken |
| 3 | `brain.md` | 16, 29, 34–35, 62–64, 116–119 (kod kuralları ve SOLID) | kod yazılırken |
| 4 | `architecture/index.md` | 5, 11–12, 14, 58–60, 98–101 (düğüm, katman, port) | dosya nereye konacağı |
| 5 | `VISION.md` | 8, 23, 27, 43, 88, 90–92, 97 (ekran ve yüzey terimleri) | ekran işi başlarken |
| 6 | `PROJECTS.md` | 4, 18–20, 36–37, 48, 102–106 (envanter ve risk) | kapsam tartışılınca |
| 7 | `spec/index.md` | 74, 76, 84–85, 92–93 (davranış terimleri) | davranış tanımı aranınca |
| 8 | `CLAUDE.md` | 49, 109–112 (kanıt ve etiketler) | iddia yazılırken |

**Ters arama kuralı:** bu tablo, kelimeyi bulamayan okuyucunun ikinci yoludur; iki yoldan biri mutlaka sonuç verir, vermezse terim tanınmamıştır ve §2 kural 5 işletilir.

### §3.2 Yeni terim ekleme akışı (6 adım)

| # | Adım | Yapılan iş | Kanıt |
|---|------|------------|-------|
| 1 | Kelime vault'ta gerçekten geçiyor mu? | en az bir dosyada yazılı olduğunu göster | dosya adı + geçtiği yer |
| 2 | TR karşılığı seçilir | mevcut TR ile çakışıp çakışmadığına bak | §1'de aynı anlamda TR var mı kontrolü |
| 3 | ELI10 tek cümleye indirilir | iki cümleye düştüyse ikinci satırı ayrı terim yap | tek cümle sayımı |
| 4 | Bağlı dosya/ADR yazılır | karar gerekçesi varsa ADR, kural ise `.rules/index.md` | hedef envanterde var mı |
| 5 | §1.1 kümesine oturt | satır aralığına göre küme satırı güncelle | aralık = tablo son satırı |
| 6 | Doğrulama çalıştır | link ve karakter taraması | `check-links` + `find-mojibake` çıktısı |

**Akış kuralı:** 1. adım geçilmeyen terim 2. adıma bile uğramaz; "işimize yarar" gerekçesi tek başına satır açmaz.

---

## §4 İlgili Dosyalar

| # | Dosya | Bu dosyayla ilişkisi | Ayrık olan |
|---|-------|----------------------|------------|
| 1 | `keys.md` | kelime → dosya hız haritası | tanım yok, yalnız gösterir |
| 2 | `brain.md` | ADR özeti ve kod kuralları (terimlerin karar karşılıkları) | tanım tekrarlanmaz |
| 3 | `VISION.md` | ekran sözcükleri (wizard, agent panel, router…) | proje tanımı yok |
| 4 | `PROJECTS.md` | proje/bağımlılık sözcükleri (CPM, K3, dual bacak…) | karar özeti yok |
| 5 | `CLAUDE.md` | yasak terimler ve etiketler | sözlük işi yok |
| 6 | `WORKFLOW.md` | gate ve süreç terimleri | tanım burada taşınmaz |
| 7 | `AGENTS.md` | ajan, kutu, strike, bütçe terimleri | kod kuralları yok |
| 8 | `index.md` | vault girişi ve harita | konu içeriği yok |
| 9 | `architecture/index.md` | düğüm, katman ve `N` kodu terimleri | süreç yok |
| 10 | `spec/index.md` | davranış terimleri (bildirim, gözlemlenebilirlik, oturum) | karar gerekçesi yok |
| 11 | `.rules/index.md` | kural terimleri (adlandırma, caching, erişilebilirlik) | tanım yerine kural metni |
| 12 | `.decisions/adr-index.md` | karar adları ve numaraları | tanım değil, gerekçe |

---

| ROLE | [ROLE](role.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| MEMORY | [MEMORY](../memory/memory.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [log](../ops/log.md) | append-only is gunlugu + Quality Report |
| ULTRA | [ULTRA](ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |

## §5 Doğrulama

| # | Kontrol | Hedef | Nasıl bakılır |
|---|---------|-------|---------------|
| 1 | Frontmatter | 8 alan, `authority: SSOT`, `updated: 2026-10-07` | ilk satır `---`, 8. satır `---` |
| 2 | Terim sayısı | §1 tablosu 136 satır | satır sayımı; §0'daki sayı ile aynı olmalı |
| 3 | Sütun doluluğu | her satırda ELI10 tek cümle ve bağlı dosya var | boş hücre = tanımsız terim |
| 4 | Link | yalnız mevcut dosyalara | `pwsh -File scripts/check-links.ps1` → `BOZUK = 0` |
| 5 | Karakter | emoji yok, Unicode kutu çizimi yok, mojibake yok | `pwsh -File scripts/find-mojibake.ps1` → `0` |
| 6 | Boyut | 30–88 KB bandında | `(Get-Item .ai/glossary.md).Length/1KB` — ölçüm yazılır |

---

## §6 Kızıl Ekip — Sözlük Bozulursa 6 Hata

| # | Hata | Belirtisi | Neden zararlı | Anında düzeltme |
|---|------|-----------|---------------|-----------------|
| 1 | Tanımı dosyaya kopyalamak | aynı ELI10 iki dosyada birebir yazılı | ikinci kopya güncellenmez, biri eski kalır | kopyayı sil, tek satırda `glossary.md` referansı bırak |
| 2 | Uydurma terim eklemek | kelime vault'ta hiç geçmiyor | arama sonuçsuz döner, okuyucu dosya tarar | satırı sil; gerçekten gerekiyorsa 1. adımdan başla (§3.2) |
| 3 | Tanımı iki cümleye taşımak | ELI10 satırı uzayıp virgülle iki işi birden anlatır | tanım değil, açıklama başlar | tek cümleye indir ya da iki terime böl |
| 4 | Bağlı dosya boş bırakmak | 5. sütun `-` | terim nerede kullanılıyor bilinmez, ters arama kırılır | hedefi yaz, envanterde yoksa `DOĞRULAMA GEREKLİ` koy |
| 5 | Sayıyı elle güncellemek | §0'daki toplam ile §1 satır sayısı uyuşmuyor | okuyucu eksik terim olduğunu düşünür | tabloyu say, §0'ı sayıyla eşitle |
| 6 | Kısaltmayı tam hâlsiz kullanmak | dosyada yalnız `TFM` geçiyor | uzun adı arayan kişi bulamaz | ilk geçtiği yerde "Hedef çerçeve (TFM)" yaz |

**Ortak kural:** altı maddeden biri fark edilirse sözlük genişletilir değil, onarılır; onarım bittikten sonra §5 doğrulaması çalıştırılır ve sonuç `log.md`'ye Quality Report olarak yazılır. Kanıtsız "düzeldi" ifadesi maddeyi kapatmaz.
