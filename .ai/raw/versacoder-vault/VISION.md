---
title: "VersaCoder — Ürün Vizyonu (Product Vision)"
type: vision
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Misyon, 3 persona, problem–çözüm matrisi, 7 ürün prensibi, 7 ekran ürün haritası, non-goals, farklılaşma, kalite nitelikleri, başarı metrikleri, 8 part yol haritası, bağımlılıklar ve dış kısıtlar"
---

# VersaCoder — Ürün Vizyonu (`VISION.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[PROJECTS.md]] · [[log.md]] · [Kapsam analizi](reports/00-kapsam-analizi.md) · [Şablon envanteri](.templates/index.md) · [Ajan kaydı](.agents/AGENTS.md)

> **Tek cümlelik sorumluluk:** *VersaCoder ne üretir, kimin için neden var, hangi 7 ekranla sunulur ve başarısı neyle ölçülür* sorusunun tek cevabıdır.
> Bu dosya **vizyonu** taşır; bileşen/kapsam envanteri `PROJECTS.md`'dedir, süreç ve gate'ler `WORKFLOW.md`'dedir (planlı) — tekrar yasaktır (SRP, G1).

---

## İçindekiler

| Bölüm | Alt Konular |
|---|---|
| §1 Misyon | Vizyon cümlesi · ne üretiyoruz · ne değiliz |
| §2 Hedef Kullanıcılar | 3 persona · ihtiyaç · başarısızlık senaryosu |
| §3 Problem İfadesi | Problem–çözüm matrisi · kanıt |
| §4 Ürün Prensipileri | 7 prensip · ihlal karşılığı · örnek |
| §5 7 Ekran Ürün Haritası | ASCII harita · ekran başına teslimat + gerekçe |
| §6 Non-Goals | Kapsam dışı (16 madde) |
| §7 Farklılaşma | Generic IDE'lere karşı 8 fark |
| §8 Kalite Nitelikleri | Güvenilirlik · offline-first · çift hedef · güvenlik |
| §9 Başarı Metrikleri | Ölçülebilir hedefler + ölçüm aracı |
| §10 Yol Haritası | 8 master part ↔ çıktı ↔ gate |
| §11 Bağımlılıklar ve Dış Kısıtlar | DevExpress v26.1 · opencode v1.18.32 · net48+net10 |
| §12 Red Team · §13 Doğrulama · §14 Referanslar | Sık hatalar, checklist, kaynaklar |

---

## §1 Misyon

### §1.1 Misyon Bildirimi

> **VersaCoder, .NET masaüstünde çalışan bir AI Coding Platform (Agentic IDE)'dir:**
> geliştiriciye **kod yazdıran**, **görevi planlayan**, **aracı (tool) çağrısını gösteren** ve
> **kanıtla teslim eden** bir ajan çalışma ortamı sunar. Tek pencere (`RibbonForm`) altında
> editör, ajan sohbeti, görev kuyruğu, terminal ve ayarlar yaşar; internet olmadan da
> yerel modelle (Ollama) çalışır.

### §1.2 Vizyon Cümlesi (12 ay sonra)

| # | Boyut | Hedef görüntü |
|---|---|---|
| 1 | Ürün | `VersaCoder.Desktop` + `VersaCoder.Cli` iki uygulama, aynı protokol üzerinde |
| 2 | Kullanıcı | .NET geliştiricisi Windows'ta tek pencerede ajanla birlikte kod yazar |
| 3 | Güven | Her agent tool çağrısı ekranda ve logda görünür; "gizli işlem yok" (G11) |
| 4 | Kalite | `net48` + `net10.0-windows` iki bacakta da build 0 hata, testler yeşil (G5) |
| 5 | Bağımsızlık | Ağ kesilse bile yerel modelle düzenleme/derleme akışı sürer (offline-first) |

### §1.3 Ne Değiliz

| Biz DEĞİLİZ | Neden |
|---|---|
| "Kod yazan chatbot" | Kod üreten değil; **karar veren, kanıtlayan** mühendis akışı (CLAUDE.md §0) |
| Web tabanlı SaaS IDE | Masaüstü WinForms + DevExpress; web UI yasak (G8) |
| Genel amaçlı IDE eklentisi | VS Code extension / VSIX yok (§6 non-goal) |
| Ajan pazar yeri | Marketplace, üçüncü taraf plugin mağazası kapsam dışı |
| Uzaktan çalışma / kolaborasyon aracı | Real-time co-editing, cloud sync yok (§6) |

> **KURAL:** Vizyon cümlesi değiştirilirse `PROJECTS.md` §1 ve `reports/00-kapsam-analizi.md` §2
> birlikte gözden geçirilir; tek başına bu dosya güncellenemez (SSOT zinciri).

---

## §2 Hedef Kullanıcılar (3 Persona)

### §2.1 Persona Özet Tablosu

| # | Persona | Rol | Ana acı | VersaCoder vaadi | Kullanım sıklığı |
|---|---|---|---|---|---|
| P1 | **Legacy .NET Koruyucusu** | 8+ yıl .NET Framework, WinForms/DevExpress ekibi | EF Core vb. modern yığınlar net48'te çalışmıyor; ajan "modern" diye kodu bozuyor | net48'te **derlenen** ajan çıktısı; iki bacak doğrulama | Her gün, 4–8 saat |
| P2 | **Ajan Odaklı Junior/Orta Geliştirici** | Feature üretimini ajanla yürüten | Ajanın ne yaptığını göremiyor; yanlış tool çağrısını geç fark ediyor | tool çağrısı + diff **görünür**, görev kuyruğu **izlenebilir** | Her gün, 6–9 saat |
| P3 | **Tek Kişilik Ürün Sahibi (Indie .NET)** | Tek başına ürün geliştiren | Kurulum maliyeti: IDE + LLM + hata ayıklama hepsi ayrı | Tek pencere, offline yerel model, ayarlar tek yerde | Günde 2–5 saat |

### §2.2 Persona Ayrıntıları

**P1 — Legacy .NET Koruyucusu**

| Alan | Detay |
|---|---|
| Profil | .NET Framework 4.8 üzerindeDevExpress WinForms ürünleri bakım eder |
| Başarı tanımı | Ajan ürettiği kodu **net48'de** derletir; sürüm sapması yapmaz |
| Başarısızlık senaryosu | Ajan EF Core / yeni DevExpress sürümü önerir → build kırılır |
| Kullanım | `03-editor` + `05-tasks` + `06-terminal`; `07-settings`'te bacak seçimi |
| Kritik nitelik | **Dual-target** (§8.3) + sürüm kanunu (G3, G8) |

**P2 — Ajan Odaklı Geliştirici**

| Alan | Detay |
|---|---|
| Profil | Prompt yazar, ajanın çıktısını review eder, testi kendi koşar |
| Başarı tanımı | Bir görevi ajanla **onaysız hiçbir yıkıcı işlem olmadan** tamamlar |
| Başarısızlık senaryosu | Ajan sessizce dosya siler / arka planda paket kurar (G7, G11 ihlali) |
| Kullanım | `04-agent-chat` + `05-tasks` merkezde; `01-start-page`'den geri döner |
| Kritik nitelik | **Şeffaflık** (§8.4) + güvenilirlik (§8.1) |

**P3 — Tek Kişilik Ürün Sahibi**

| Alan | Detay |
|---|---|
| Profil | Her şeyi tek başına yapar; kurulum ve yapılandırma tahammülü düşük |
| Başarı tanımı | `02-project-new` sihirbazıyla yeni projeyi **5 dakikada** ajanlı açar |
| Başarısızlık senaryosu | İnternet yoksa hiçbir şey çalışmıyor (bağlantı bağımlılığı) |
| Kullanım | 7 ekranın tamamı; `01-start-page` günlük giriş noktası |
| Kritik nitelik | **Offline-first** (§8.2) + hız (§9) |

> **UYARI:** Persona listesi `reports/00-kapsam-analizi.md` §1'deki girdi envanterinden türetildi
> (`SOURCE_VERIFIED`). Yeni persona eklemek = `VISION.md` sürüm artışı + `log.md` kaydı.

### §2.3 Tipik Bir Gün (Persona Senaryoları)

**P1 — Legacy .NET Koruyucusu, sabah:**

```text
09:00  01-start-page  ->  dünkü bakim oturumu acilir (kayip yok)
09:05  03-editor      ->  LegacyService.cs acilir
09:07  04-agent-chat  ->  "net48 uyumlu duzenleme yap" prompt'u
09:08  05-tasks       ->  gorev: BEKLIYOR -> CALISIYOR (tool cagrilari gorunur)
09:12  06-terminal    ->  dotnet build -f net48  ->  0 hata (KANIT, G5)
09:13  05-tasks       ->  gorev: BITTI + kanit baglantisi
09:14  Risk yok       ->  EF Core onerisi GELMEZ (yigin sabit, G2)
```

**P2 — Ajan Odaklı Geliştirici, öğle:**

```text
13:30  04-agent-chat  ->  ornek uret prompt'u
13:31  tool: read     ->  ekranda dosya + satir araligi gorunur
13:31  tool: edit     ->  diff onayi ISTENIR (onaysiz uygulanmaz)
13:33  05-tasks       ->  gorev RED -> bulgu: konum + neden + duzeltme
13:34  04-agent-chat  ->  duzeltme prompt'u -> tekrar review
13:40  06-terminal    ->  dotnet test -> yeşil kanit
```

**P3 — Tek Kişilik Ürün Sahibi, akşam (çevrimdışı):**

```text
21:00  internet yok  ->  07-settings: provider = ollama (yerel)
21:01  02-project-new ->  yeni urun iskeleti, 2 bacak, CPM
21:05  04-agent-chat  ->  yerel modelle gorev akisi surer (offline-first)
21:20  06-terminal    ->  build + test yerel SDK ile kosulur
21:21  01-start-page  ->  oturum kayitli, yarin kaldigi yerden
```

> **KURAL:** Senaryolardaki saatler **temsili**dir (`INFERRED`); performans iddiası değildir,
> ölçüm §9 M9/M10 üzerinden yapılır.

---

## §3 Problem İfadesi

### §3.1 Problem

> Modern ajanlı geliştirme araçları ya **tarayıcı/kod editörü eklentisi** olarak yaşar
> (masaüstü .NET bakım ürünlerine uymaz), ya da ajanın **ne yaptığını gizler**
> (tool çağrısı, dosya değişimi, görev durumu görünmez). Aynı anda iki gerçek var:
> **(1)** binlerce satır .NET Framework 4.8 kodu hâlâ yaşıyor ve ajanlar bunu modern
> yığınla bozuyor, **(2)** geliştirici ajanın sağladığı hızı kaybedecek kadar
> denetim eksikliğiyle kullanıyor. Sonuç: **güvensiz ajan çıktısı + masaüstünde ajan boşluğu.**

### §3.2 Problem → Çözüm Matrisi

| # | Problem | Kanıt (kaynak) | VersaCoder Çözümü | Karşılayan ekran |
|---|---|---|---|---|
| 1 | Ajan çıktısı net48'te derlenmiyor | `reports/00-kapsam-analizi.md` §2 karar 1, 5 | İki bacak build + ortak kaynak + `#if` yalnız platform farkı (G6) | `06-terminal` + `05-tasks` |
| 2 | ajan gizli işlem yapıyor (sessiz silme, sessiz kurulum) | `CLAUDE.md` G7/G11 | Onay kapıları + görünür görev kuyruğu + işlem logu | `05-tasks` |
| 3 | tool çağrısı izlenemiyor | opencode protocol envanteri (kapsam analizi §1.3) | Protokol mesajlarını ekranda akıtma + kalıcı log | `04-agent-chat` |
| 4 | Yeni proje kurulumu dağınık (sln, paket, model, tema) | Persona P3 | Tek sihirbaz: hedef bacak + provider + tema tek akışta | `02-project-new` |
| 5 | İnternet/LLM yoksa üretim duruyor | B4 belirsizliği (kapsam analizi §3.2) | Offline-first: Ollama yerel provider, çekirdek akış ağa bağlı değil | `07-settings` |
| 6 | Görevler kayboluyor, durum unutuluyor | `MEMORY.md` kapsamı (planlı) + oturum belleği ihtiyacı | Görev durumu + oturum belleği kalıcı; `01-start-page` geri dönüş | `01-start-page` |
| 7 | Tema/DPI/erişilebilirlik ihmali | `.agents/ui-designer.md` §4 | Office2010 ribbon, dark theme, High DPI, AA kontrast varsayılan | tümü |
| 8 | Kod + terminal + çıktı ayrı pencerelerde | WinForms masaüstü normu | Tek `RibbonForm` içinde docking düzeni | `03-editor` + `06-terminal` |

### §3.3 Çözüm Örneği (Yaşam Döngüsü)

```text
[01-start-page] --> sec --> [02-project-new] --> olustur --> [03-editor]
      ^                                                        |
      |                                                        v
      +--- [07-settings] <--- tercih ---------------- [04-agent-chat]
                                                             |
                                    +------------------------+
                                    v                        v
                             [05-tasks]                [06-terminal]
                             (kuyruk/durum)            (build/test ciktisi)
                                    |                        |
                                    +-----------> kanit <-----+
                                             (G5: build/test)
```

---

## §4 Ürün Prensipileri (7)

| # | Prensip | Bir cümlede | İhlalinde |
|---|---|---|---|
| 1 | **Kanıt önce** | "Bitti" yalnız build/test çıktısıyla söylenir | RED (G5) |
| 2 | **Tek gerçek kaynağı (SSOT)** | Her konunun tek sahip dosyası var; tekrar yasak | Çelişki → ilgili dosya kazanır |
| 3 | **Onay kapıları** | Yıkıcı/UI değişimi kullanıcı onayıyla geçilir | CRITICAL (G7, GATE-003) |
| 4 | **İki bacak eşitlik** | Her karar net48 ve net10'da birlikte değerlendirilir | HIGH (G6) |
| 5 | **Şeffaf ajan** | Her tool çağrısı, dosya değişimi ve görev görünür | RED (G11) |
| 6 | **Offline-first** | Çekirdek akış ağ/LLM olmadan ayakta kalır | HIGH (§8.2) |
| 7 | **Güvenlik varsayılanı** | Parameterized SQL, şifreli secret, audit log varsayılan | CRITICAL (G2) |

### §4.1 Prensip Ayrıntıları

| # | Prensip | Ne demek | Somut karşılık | Örnek senaryo |
|---|---|---|---|---|
| 1 | Kanıt önce | İddia değil, çıktı geçerli | `dotnet build` / `dotnet test` çıktısı `log.md`'ye düşer | Ajan "derlendi" derse çıktıyı yapıştırır |
| 2 | SSOT | Bir bilgi iki yerde yaşarsa biri silinir | Vizyon burada; kapsam `PROJECTS.md` | 7 ekran listesi iki yerde çelişirse §5.1'e bakılır |
| 3 | Onay kapıları | Yıkıcı iş öncesi sor | GATE-003 UI onayı; silme öncesi kullanıcı onayı | Wireframe onaylanmadan `.Designer.cs` yazılmaz |
| 4 | İki bacak eşitlik | net48 "sonra bakılır" değildir | Ortak kaynak; `#if` yalnız platform farkı | Aynı `CommandRunner` iki bacakta derlenir |
| 5 | Şeffaf ajan | Ajanın attığı adım görünür olmalı | tool çağrısı satırı + diff + durum çubuğu | `edit` çağrısı sağ panelde dosya+satır gösterir |
| 6 | Offline-first | Ağ yoksa ürün durmaz | Ollama yerel provider; oturum belleği yerel | Uçak modunda refactor görevi tamamlanır |
| 7 | Güvenlik varsayılanı | Güvenlik sonradan eklenmez | parameterized sorgu, `CommandTimeout`, şifreli conn string | SQL enjeksiyonu derste değil, varsayılan olarak kapalı |

> **KURAL:** Yeni prensip eklemek için mevcut 7'den biriyle çelişmemek gerekir; çelişirse
> `DECISION BLOCKED` yazılır ve kullanıcı kararı beklenir (G10).

---

## §5 7 Ekran Ürün Haritası

### §5.1 ASCII Ürün Haritası

```text
+====================================================================+
|  VersaCoder.Desktop                     RibbonForm                 |
|  RibbonControlStyle.Office2010 (DevExpress Universal 2026 v26.1)   |
+--------------------------------------------------------------------+
|                                                                    |
|   +-------------------+   +-------------------+                    |
|   | 01-start-page     |-->| 02-project-new    |                    |
|   | son oturumlar     |   | sihirbaz: bacak,  |                    |
|   | hizli aksiyon     |   | model, tema, sln  |                    |
|   +-------------------+   +---------+---------+                    |
|             ^                         |                            |
|             |                         v                            |
|   +---------+--------------------------------------------------+   |
|   | 03-editor  (merkez: sekmeli kod yuzeyi, satir, arama, diff) |   |
|   +------+-----------------------------------------------+-----+   |
|          |                                               |         |
|          v                                               v         |
|   +---------------------+   +---------------------+                  |
|   | 04-agent-chat       |   | 06-terminal         |                  |
|   | akis, tool cagrisi  |   | build / test ciktisi|                  |
|   | diff onayi          |   | git, dotnet komutu  |                  |
|   +----------+----------+   +----------+----------+                  |
|              |                         |                             |
|              v                         |                             |
|   +---------------------+              |                             |
|   | 05-tasks            |<-------------+                             |
|   | gorev kuyrugu, durum, kanit bagi                                    |
|   +---------------------+                                            |
|                                                                    |
|   +---------------------+                                          |
|   | 07-settings         |  (saglik: model, guvenlik, tema, diller)  |
|   +---------------------+                                          |
+--------------------------------------------------------------------+
| status: branch | bacak: net48|net10 | model: ollama | tokens: 1.2k |
+====================================================================+
```

### §5.2 Ekran Envanteri (Teslimat + Gerekçe)

> **UYARI (CONFLICT DETECTED):** Bu ürün haritası görev tanımındaki ekran adlarını kullanır
> (`01-start-page · 02-project-new · 03-editor · 04-agent-chat · 05-tasks · 06-terminal · 07-settings`).
> [Kapsam analizi §4.1](reports/00-kapsam-analizi.md) ve [.agents/ui-designer.md §3](.agents/ui-designer.md)
> ise **farklı dosya adları** listeler: `02-main-ide · 03-solution-explorer · 04-code-editor · 05-ai-chat · 06-agent-panel`.
> Karar: **PART 4 başında tek liste kullanıcı onayıyla kesinleşir** (GATE-003). Çözülene kadar
> iki isim de bağlayıcı değildir; **bağlayıcı olan ekranın ne sunduğudur** (bu tablo).
> Kaynak: `R1` riski — `reports/00-kapsam-analizi.md` §3.1.

| # | Ekran (vizyon) | Ne teslim eder | Neden var (persona bağlangıcı) | Öncelik |
|---|---|---|---|---|
| 01 | `01-start-page` | Son oturumlar, hızlı ajan başlangıcı, proje aç/yeni, model sağlık rozeti | Günlük giriş noktası; kayıp durum geri yüklenir (P1–P3) | M1 |
| 02 | `02-project-new` | Sihirbaz: hedef bacak (net48/net10), solution şablonu, Dapper provider, LLM provider, tema | Kurulum tek akışta olmazsa P3 5 dakikada başlayamaz | M1 |
| 03 | `03-editor` | Sekmeli kod yüzeyi: satır numarası, arama, diff görünümü, dosya gezgini bağlanışı | İşin kendisi burada; ajan çıktısı burada gözden geçirilir | M2 |
| 04 | `04-agent-chat` | Streaming yanıt, tool çağrısı satırı, diff/onay akışı, bağlam (context) editörü | Ajan şeffaflığı prensibi (5) burada yaşar; P2'nin ana ekranı | M2 |
| 05 | `05-tasks` | Görev kuyruğu: durum (bekliyor/çalışıyor/bitti/RED), kanıt bağlantısı, yeniden çalıştır | Yıkıcı olmayan izlenebilirlik; G5 kanıtı burada bağlanır | M3 |
| 06 | `06-terminal` | `dotnet build` / `dotnet test` / git çıktıları, hata listesi, kopyalanabilir log | Kanıt ekranı; P1'in net48 doğrulama noktası | M3 |
| 07 | `07-settings` | Model/endpoint, güvenlik (secret vault), tema/DPI, dil, oturum saklama politikası | Tek yapılandırma merkezi; offline-first burada devreye girer | M4 |

### §5.3 Ekran Bazlı DevExpress Adayları (Bağlayıcı Değil — PART 4'te Kesinleşir)

| Ekran | Aday kontroller | Hedef bacak notu |
|---|---|---|
| `01-start-page` | `RibbonControl`, `TileControl`, `XtraTabControl`, `SearchControl` | net48 + net10 |
| `02-project-new` | `WizardControl` / `XtraTabControl` + `DataLayoutControl`, `LookUpEdit` | net48 + net10 |
| `03-editor` | `MemoEdit` / `RichEditControl`, `XtraTabControl`, `LayoutManager` | net10'da zengin editör; net48: `MemoEdit` (R5 sinyali) |
| `04-agent-chat` | `ListBoxControl` (sanallaştırma), `MemoEdit`, `ProgressBarControl`, `BarEditItem` | net48 + net10 |
| `05-tasks` | `GridControl` (sanal mod), `ProgressPanel`, `BarButtonItem` | 500+ satırda sanal mod zorunlu |
| `06-terminal` | `MemoEdit` (salt okunur), `XtraTabControl`, `StatusBarControl` | çıktı akışında scroll performansı |
| `07-settings` | `PropertyGridControl`, `DataLayoutControl`, `ToggleSwitch`, `LookUpEdit` | tema geçişi `LookAndFeel` |

> **UYARI:** Kontrol listesi `NEEDS_VALIDATION` — net48 DevExpress v26.1 uyumluluğu
> **R5** riskidir ve ilk `dotnet build` ile PART 5'te erken doğrulanır
> (`reports/00-kapsam-analizi.md` §3.1).

### §5.4 Ekranlar Arası Zorunlu Akışlar

| # | Akış | Adımlar | Kural |
|---|---|---|---|
| F1 | Yeni proje | 01 → 02 → 03 | Sihirbaz bitmeden editör açılmaz |
| F2 | Ajan görevi | 03 → 04 → 05 → 06 | Görev "bitti" sayılması için 06'da kanıt şart (G5) |
| F3 | UI değişikliği | 04 (talep) → onay → 05 → 03 | GATE-003 onaysız `.Designer.cs` yok |
| F4 | Ayar değişikliği | 07 → ilgili ekran | Model/tema değişimi oturumu kırmaz |
| F5 | Hata/RED | 06 → 05 (RED) → 04 (düzeltme) | Bulgu biçimi: konum + neden + düzeltme |

### §5.5 Ekran Başına Davranış Sözleşmesi

| Ekran | Kullanıcı girdisi | Sistem çıktısı | Varsayılan durum | Tehlike modu |
|---|---|---|---|---|
| `01-start-page` | Proje seçimi, hızlı ajan komutu | Oturum yükleme, model sağlık rozeti | Son oturum listede | Oturum bozuksa → salt okunur aç, `log.md`'ye düş |
| `02-project-new` | Bacak, solution adı, provider, tema | Şablon iskelet + `Directory.Packages.props` | net10.0-windows seçili | Geçersiz yol/izin → alan seviyesinde hata, ileri kapalı |
| `03-editor` | Dosya açma, düzenleme, arama | Diff görünümü, kaydetme | Son dosya + seçim geri gelir | Kirli dosya → "kaydet?" kapısı (G7) |
| `04-agent-chat` | Prompt, bağlam seçimi, onay/red | Streaming yanıt, tool çağrısı satırı | Gönderim kuyrukta bekler | Provider yok → `07-settings` yönlendirmesi (§8.2 O4) |
| `05-tasks` | Görev başlat/durdur/yeniden çalıştır | Durum + kanıt bağlantısı | Kuyruk boş | RED görev → editör değil, görev satırı vurgulanır |
| `06-terminal` | Komut girişi (onaylı), kopyala | Build/test çıktısı, hata listesi | Son komut + temiz buffer | Yıkıcı komut → onay kapısı (G7) |
| `07-settings` | Model/endpoint, tema, güvenlik, dil | Kalıcı yerel tercih | Varsayılanlar (offline-first) | Geçersiz endpoint → test butonu + açık hata |

### §5.6 Boşluk (Empty-State) Kuralları

| # | Ekran | Boş görünüm | Metin kuralı |
|---|---|---|---|
| 1 | `01-start-page` | "Henüz oturum yok — Yeni Proje" | Eylem odaklı, tek buton |
| 2 | `02-project-new` | Sihirbaz 1. adım | Zorunlu alan yıldızı, varsayılan net10 |
| 3 | `03-editor` | Dosya gezgininden seçim önerisi | Kısayol ipucu göster |
| 4 | `04-agent-chat` | 3 hazır başlangıç komutu | Komutlar projeye göre (bilgi: `INFERRED`) |
| 5 | `05-tasks` | "Görev kuyruğu boş" | Kanıt neden gerekir, tek cümle |
| 6 | `06-terminal` | Son build sonucu rozeti | 0 hata / n hata |
| 7 | `07-settings` | Varsayılan özet + "Test et" | Bağlantı testi zorunlu buton |

---

## §6 Non-Goals (Kapsam Dışı)

> **KURAL:** Aşağıdaki 16 madde **bu projede yapılmaz**. Öneri gelirse önce bu tablo güncellenir,
> sonra işe başlanır. Tablosuz yeni özellik = kapsam kayması.

| # | Non-Goal | Neden dışarıda | İlgili guardrail |
|---|---|---|---|
| 1 | Web UI / tarayıcı sürümü | Ürün masaüstü WinForms | G8 |
| 2 | WPF / WinUI / MAUI geçişi | DevExpress WinForms yığını sabit | G8 |
| 3 | EF Core (herhangi bir sürüm) | net48 + ortak kaynak için yol yok; ADO.NET + Dapper | G2 |
| 4 | macOS / Linux hedefi | `net10.0-windows` + WinForms Windows'a bağlı | G6 |
| 5 | VS Code extension / VSIX | Kendi host'umuz var; eklenti ekosistemi ayrı ürün | — |
| 6 | Electron / TypeScript frontend | JS framework yasak (yerleşim C#) | G8 |
| 7 | Real-time ortak çalışma (co-editing) | Senkron protokol + çakışma çözümü kapsam dışı | — |
| 8 | Bulut senkron / hesap sistemi | Yerel-öncelikli ürün; secret yerelde | §8.2 |
| 9 | Marketplace / plugin mağazası | Güvenlik + denetim maliyeti; MCP sonraki faz | §8.4 |
| 10 | Mobil uygulama | Masaüstü ürünü | — |
| 11 | Çoklu dil (UI localization) | İlk sürüm Türkçe/İngilizce etiket; tam i18n sonraki faz | — |
| 12 | Otomatik NuGet yayını | Yayınlama = manuel onaylı iş | G7 |
| 13 | Varsayılan telemetri / usage upload | Gizli işlem yasak; telemetri opt-in, varsayılan kapalı | G11 |
| 14 | Kendi LLM eğitimi / fine-tune | Provider entegrasyonu yeterli (B4) | — |
| 15 | ai.01'deki eski yığın (`.NET 8`, EF Core 9, DevExpress 24.2.3) | ai.03.md tarafından geçersiz kılındı | `CONFLICT` §11.3 |
| 16 | CoreMusic içeriğinin kopyalanması | Salt okunur YAPI referansı | G1 |

---

## §7 Farklılaşma (Generic IDE'lere Karşı)

| # | Özellik | Generic IDE (VS / Rider / VS Code) | VersaCoder farkı |
|---|---|---|---|
| 1 | Ajan bütünlüğü | Eklenti/yan panel; ayrı ürün hissi | Ajan, 7 ekranın **doğal parçası** (04 + 05) |
| 2 | tool çağrısı görünürlüğü | Genelde log/konsol | Ekran yüzeyinde akış + görev kuyruğuna bağlanı |
| 3 | net48 + modern ajan | Modern odaklı; legacy projede kırılır | **İki bacak eşit**; legacy kanıt hedefi (P1) |
| 4 | Onay modeli | Editör değişimi anında uygulanır | Yıkıcı iş + UI değişimi **onay kapılı** (GATE-003) |
| 5 | Kanıt disiplini | Kullanıcı inanır | Build/test çıktısı **görev kanıtı** olarak zorunlu (G5) |
| 6 | Offline çalışırlık | Çoğu ajan bulut bağımlı | Yerel provider (Ollama) ile çekirdek akış çevrimdışı |
| 7 | Teknoloji bağımsızlığı | Geniş ekosistem | Kasıtlı **dar yığın**: DevExpress v26.1 + Dapper + xUnit |
| 8 | Ürün odağı | Genel amaçlı, binlerce özellik | **Tek iş**: .NET masaüstünde güvenli ajanlı geliştirme |

> **UYARI:** "Daha iyi IDE" iddiası yasak (uydurma riski, G9). Farklılaşma **iddia** değil,
> §9'daki **ölçülebilir metriklerle** kanıtlanır.

---

## §8 Kalite Nitelikleri

### §8.1 Güvenilirlik (Reliability)

| # | Nitelik | Ölçülebilir hedef | Sağlama | Doğrulama |
|---|---|---|---|---|
| Q1 | Çökme dayanıklılığı | Uygulama kapanınca oturum + görev durumu geri gelir | Yerel kalıcılık (`MEMORY.md` kapsamı) | Elle kesme testi |
| Q2 | Deterministik build | Aynı girdiyle iki bacakta aynı derleme sonucu | CPM sabit paket sürümleri | CI iki bacak |
| Q3 | Görev bütünlüğü | Görev ya `bitti+kanıt` ya `RED` ya `bekliyor` — ara durum yok | Durum makinesi (`05-tasks`) | Durum geçiş testi |
| Q4 | Tekrarlanabilir migration | Saf SQL + `schema_version`; yanlış sürümde uygulanmaz | Versiyon tablosu | Migration testi |
| Q5 | Hata gizlememe | tool hatası sessiz yutulmaz, ekrana + loga düşer | Protokol hata mesajı | Hata modu tablosu (§12) |

### §8.2 Offline-First

| # | Davranış | Çevrimiçi | Çevrimdışı |
|---|---|---|---|
| O1 | Kod düzenleme, arama, gezinme | Tam | **Tam** (çekirdek UI) |
| O2 | `dotnet build` / `dotnet test` | Tam | **Tam** (yerel SDK) |
| O3 | Ajan görevi (yerel model) | Tam | **Tam** (Ollama provider) |
| O4 | Ajan görevi (uzak provider) | Tam | Kullanılabilir değil → `07-settings` açık uyarı |
| O5 | Vault / doküman okuma | Tam | **Tam** (yerel `.ai/`) |
| O6 | Güncelleme kontrolü | Tam | Atlana bilir, akışı bloke etmez |

> **KURAL:** Ağ erişimi bir ekranın **açılmasını** engelleyemez; yalnız ilgili provider'ı
> pasifleştirir. Aksi davranış = §8.2 ihlali → `log.md`'ye düşülür.

### §8.3 Çift Hedef Destek (net48 + net10.0-windows)

| # | Kural | Detay |
|---|---|---|
| D1 | Ortak kaynak | İş mantığı tek yerde; `#if` yalnız platform API farkı |
| D2 | Eşit build | İki bacakta da **0 hata** (PART 8 DoD #1) |
| D3 | Eşit test | Aynı test seti iki bacakta koşulur |
| D4 | DevExpress v26.1 | Her iki bacakta aynı sürüm (G3); uyumluluk R5 ile erken sınanır |
| D5 | Veri erişimi | ADO.NET + Dapper; EF Core **yok** (G2) — net48 kilit nedeni |

### §8.4 Güvenlik

| # | Tehdit | Kontrol | Varsayılan |
|---|---|---|---|
| S1 | SQL injection | Her sorgu parameterized; string concat yasak | Açık |
| S2 | Secret sızıntısı | AES-256-GCM credential vault; bağlantı stringi şifreli | Açık |
| S3 | Yıkıcı sessiz işlem | Silme/overwrite öncesi onay (G7) + işlem logu | Açık |
| S4 | Arka planda kurulum | Paket/eklenti kurulumu görünür + onaylı (G11) | Açık |
| S5 | Tool yetki aşımı | Protokol seviyesinde izin listesi; her çağrı logda | Açık |
| S6 | Güvenlik iddiası uydurma | Idari iddialar `NEEDS_VALIDATION` işaretlenir (G9) | Açık |

---

## §9 Başarı Metrikleri

> **UYARI:** Bu bölümdeki sayılar **hedeftir**, ölçülmüş sonuç değildir (`INFERRED` /
> `NEEDS_VALIDATION`). Ölçüm sonuçları `log.md`'ye ve `reports/09-teslim-raporu.md`'ye düşer.

| # | Metrik | Tanım | Hedef | Ölçüm aracı | Sıklık |
|---|---|---|---|---|---|
| M1 | Build sağlığı | İki bacakta derleme hatası | 0 hata | `dotnet build` çıktısı | Her part |
| M2 | Test sağlığı | Yeşil test oranı | %100 yeşil | `dotnet test` çıktısı | Her part |
| M3 | Coverage | Satır kapsamı | `NEEDS_VALIDATION` (PART 5'te B2 ile sabitlenecek) | coverage raporu | Part 6–8 |
| M4 | Link bütünlüğü | Vault'ta bozuk link | 0 | `scripts/link-scan.ps1` (planlı) | Part 3 ve 8 |
| M5 | Sürüm tutarlılığı | DevExpress dışı sürüm referansı | 0 | repo taraması | Her part |
| M6 | EF Core sızıntısı | EF Core referansı | 0 | repo taraması | Her part |
| M7 | Ajan görünürlüğü | Ekranlarda gözlenemeyen tool çağrısı | 0 | Protokol logu denetimi | Part 7 |
| M8 | Onay disiplini | Onaysız yıkıcı/UI değişimi | 0 | `05-tasks` onay kayıtları | Sürekli |
| M9 | Başlangıç süresi | `01-start-page` gösterim süresi | < 3 sn (hedef) | Zamanlama testi | Part 6 |
| M10 | Offline görev | Ağsız tamamlanan görev başarısı | Çekirdek akış %100 | Elle çevrimdışı testi | Part 7 |
| M11 | Vault ölçeği | Toplam MD / kök MD boyut bandı | 14 kök · 30–88 KB | PowerShell ölçümü | Part 3, 8 |
| M12 | Bozuk iddia | `UNKNOWN`/`NEEDS_VALIDATION` görmezden gelme | 0 | Red-team denetimi | Part 8 |

---

## §10 Yol Haritası (8 Master Part)

| Part | Ad | Bu vizyonda karşılığı | Ana çıktı | Gate |
|---|---|---|---|---|
| 1 | Kaynak okuma | §3 problem kanıtı | Girdi envanteri | GATE-001 |
| 2 | Kapsam analizi | §1–§3 bu dosyaya girdi | `reports/00-kapsam-analizi.md` ✓ | GATE-002 |
| 3 | Vault kurulumu | SSOT altyapısı (§4 prensibi 2) | Kök MD + `.templates/` + `.agents/` | GATE-004 |
| 4 | UI + ASCII | §5 7 ekran haritası | `ui-design/` 7 wireframe (planlı) | **GATE-003** |
| 5 | Proje iskeleti | §8.3 iki bacak iskeleti | `spec/` + solution + build | GATE-005 |
| 6 | Kodlama | §5 ekranları + §8 nitelikleri | `src/` + `tests/` | — |
| 7 | opencode uyarlaması | §7 fark 1–3 (tool/task akışı) | `spec/protocol-spec.md` + ADR | — |
| 8 | Doğrulama | §9 metrikleri | `reports/09-teslim-raporu.md` | Kapanış |

```text
PART 1 --> PART 2 --> PART 3 --> PART 4 --(GATE-003 ONAY)--> PART 5 --> PART 6 --> PART 7 --> PART 8
okuma     kapsam      vault       UI onayi                    iskelet   kod        opencode   kanit
```

> **UYARI:** Sıra zorunlu. GATE-003 olmadan PART 5/6 kodlaması başlamaz (G12).

---

## §11 Bağımlılıklar ve Dış Kısıtlar

### §11.1 Zorunlu Bağımlılıklar

| # | Bağımlılık | Sürüm / Kaynak | Kullanım alanı | Kısıt |
|---|---|---|---|---|
| 1 | DevExpress Universal | **2026 / v26.1** | Ribbon, Grid, Docking, Editors | Tek sürüm; başka sürüm referans edilemez (G3) |
| 2 | opencode kaynak | **v1.18.32** (`%TEMP%\opencode-fresh`, 33 paket) | Tool/agent/protocol referansı — **salt okunur** | Kopyalama yok; yalnız fikir (G1) |
| 3 | Hedef framework'ler | `net48` + `net10.0-windows` | Çift bacak | Tek bacak tasarım yasak (G6) |
| 4 | Veri erişimi | ADO.NET + Dapper | Repo katmanı | EF Core yasak (G2) |
| 5 | Test | xUnit + NSubstitute + NetArchTest | Test katmanı | Kesin sürüm B2 ile PART 5 |
| 6 | LLM provider | Ollama (yerel) + OpenAI-uyumlu API | Ajan döngüsü | Öncelik sırası B4 (PART 7) |
| 7 | Paket yönetimi | CPM (`Directory.Packages.props`) | Tüm projeler | Proje başına sürüm yasak |
| 8 | İşletim sistemi | Windows | WinForms host | Diğer OS non-goal (§6.4) |

### §11.2 Dış Kısıtlar

| # | Kısıt | Kaynak | Etki | Tepki |
|---|---|---|---|---|
| C1 | DevExpress lisansı + v26.1 kurulumu | Harici | Kontroller olmadan UI üretilemez | PART 4–5 öncesi doğrula |
| C2 | net48 + v26.1 uyumluluk belirsizliği | `R5` | UI kırılabilir | İlk build'de erken doğrula, kullanıcıya raporla |
| C3 | opencode sürüm kayması (≠ v1.18.32) | Harici repo | Protokol örnekleri geçersizleşir | Sürüm sabitle; `REFERENCE_VERIFIED` etiketi |
| C4 | ai.03.md CR-only satır sonu | Girdi dosyası | Parse hatası | Okumada normalize; diske CR-only asla yazılmaz (R2) |
| C5 | ai.01 eski yığın (.NET 8 / EF Core 9 / DevExpress 24.2.3) | Girdi dosyası | Yanlış stack sızıntısı | Yalnız **yapı** alınır, sürüm alınmaz (§11.3) |
| C6 | 7 ekran ≠ 16 yedek wireframe | `R1` | İsim uyuşmazlığı | PART 4'te kullanıcı onayı (§5.2 UYARI) |

### §11.3 Kaynak Çelişkisi Kaydı (G10)

```text
CONFLICT DETECTED
  kaynak A : versacoder proje planamasi ai.01.md  -> .NET 8, EF Core 9, DevExpress 24.2.3
  kaynak B : versacoder proje planamasi ai.03.md  -> net48 + net10.0-windows, DevExpress 2026 v26.1, Dapper
  cozum    : kaynak B gecerli kilir (yeni bootstrap); kaynak A'dan YALNIZCA yapi
             (ekran listesi, gate'ler, mimari sema) alinir, surum asla alinmaz
  kanit    : reports/00-kapsam-analizi.md (PART 2, GATE-002)
  durum    : COZULDU — kullanici onayli 5 karar
```

---

## §12 Red Team (Sık Yapılan Hatalar)

| # | Hata | Doğrusu |
|---|---|---|
| 1 | Bu dosyaya bileşen envanteri yazmak | Envanter `PROJECTS.md`'de; burada yalnız **niyet** |
| 2 | Ekran adlarını PART 4'ten önce sabitlemek | §5.2 UYARI: iki isim de bekliyor, GATE-003 kesinleştirir |
| 3 | Metrikleri "başarıldı" diye yazmak | Hepsi hedef; ölçüm `log.md` + `reports/09-*` |
| 4 | Non-goal listesini atlamak | Kapsam kaymasının tek koruması §6 |
| 5 | EF Core / v26.1 dışı sürüm anmak | G2/G3 ihlali — satır silinir |
| 6 | offline-first'i "internet gerekmiyor" diye çevirmek | §8.2: yalnız provider pasifleşir, UI bloke olmaz |
| 7 | ai.01 sürümünü vizyona taşımak | §11.3: yalnız yapı alınır |
| 8 | Unicode kutu çizimi kullanmak | Yalnız ASCII (`+ - | < >`) |

---

## §13 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`
- [ ] "Zorunlu Bağlantılar" satırındaki tüm hedefler diskte mevcut
- [ ] §5.1 ASCII diyagramda Unicode kutu karakteri yok
- [ ] 7 ekran adı görev tanımıyla birebir; çelişki §5.2'de `CONFLICT DETECTED` işaretli
- [ ] 3 persona, 7 prensip, 16 non-goal, 12 metrik, 8 part tabloda
- [ ] EF Core yalnız "yasak" bağlamında geçiyor; DevExpress yalnız v26.1
- [ ] Tüm sayılar hedef olarak etiketli (kanıtsız başarı iddiası yok, G5)
- [ ] §11 bağımlılıkları `PROJECTS.md` §1 ile çelişmiyor
- [ ] Boyut hedef bandında (30–45 KB)

---

## §14 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kapsam analizi (girdi, 5 karar, R1–R6) | `.ai/reports/00-kapsam-analizi.md` |
| 2 | Navigasyon | `.ai/index.md` |
| 3 | Guardrail G1..G12 | `.ai/CLAUDE.md` |
| 4 | Kapsam + bileşen envanteri | `.ai/PROJECTS.md` |
| 5 | Ajan rolleri ve 7 ekran envanteri | `.ai/.agents/ui-designer.md` |
| 6 | Şablon kalıbı (kök MD bandı) | `.ai/.templates/documentation/root-md-template.md` |
| 7 | Kaynak okuma günlüğü | `.ai/log.md` |
| 8 | Bootstrap kaynağı (karar) | `G:\…\Versacoder\versacoder proje planaması ai.03.md` |
