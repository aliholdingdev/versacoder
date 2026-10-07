---
title: "VersaCoder — Ürün Vizyonu (Product Vision)"
type: vision
category: ".ai"
status: active
version: 3.0.0
updated: 2026-10-07
authority: SSOT
scope: "VersaCoder'in ne ürettiği, 7 ekran + CLI ürün haritası, platform anatomisi, 4 farklılaştırıcı, 3 persona, kapsam dışı ve doğrulama listesi"
---

# VersaCoder — Ürün Vizyonu (`VISION.md`)

**Zorunlu Bağlantılar:** [index](index.md) · [PROJECTS](PROJECTS.md) · [brain](brain.md) · [keys](keys.md) · [glossary](glossary.md) · [WORKFLOW](WORKFLOW.md) · [CLAUDE](CLAUDE.md) · [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [architecture/index](architecture/index.md)

> **Tek cümlelik sorumluluk:** *VersaCoder ne üretir, hangi 7 ekranla sunulur, her ekranı kim kullanır ve hangi katman besler, rakiplerden 4 farkı nedir* sorusunun tek cevabıdır.
> Bu dosya **niyet ve ürün haritası** taşır; kapsam/kayıt `PROJECTS.md`, karar gerekçesi `brain.md`, süreç `WORKFLOW.md` üzerindedir (SRP).

---

## §0 Özet (Answer First)

**VersaCoder, .NET masaüstünde çalışan bir agentic IDE'dir** (Visual Studio / Cursor sınıfı ama WinForms + DevExpress üzerinde): geliştirici tek pencerede kod yazar, ajanın tool çağrısını canlı görür, onaysız yıkıcı işlem yapılmaz ve "bitti" iddiası yalnız build/test çıktısıyla söylenir. Üretim **9 proje + 1-2 test projesi**, **8 makro katman (l0..l7)** ve **7 ekran + CLI** üzerinden yürür (ADR-028).

```text
+=====================================================================+
|  VersaCoder.Desktop   RibbonForm + RibbonControlStyle.Office2010    |
|  DevExpress Universal 2026 / v26.1   (ADR-004)                     |
+---------------------------------------------------------------------+
|  01-start-page        giris, son oturum, model saglik rozeti        |
|  02-main-ide          ribbon + docking kabuk (diger 6 ekran burada) |
|  03-solution-explorer cozum/dosya agaci, git durumu                 |
|  04-code-editor       sekmeli kod, satir, arama, diff               |
|  05-ai-chat           streaming yanit, tool cagrisi, diff onayi     |
|  06-agent-panel       gorev kuyrugu, durum, kanit baglantisi        |
|  07-settings          model/endpoint, secret, tema, bacak           |
+---------------------------------------------------------------------+
|  VersaCoder.Cli        ayni cekirdek l0..l6, headless (ADR-010)     |
+=====================================================================+
|  alt: 8 makro katman | 52 dugum | 9 uretim + 1-2 test projesi      |
+=====================================================================+
```

| Gösterge | Değer | Kaynak |
|---|---|---|
| Ekran | **7** sabit + CLI | ADR-025 · `ui-design/` |
| Katman | **8** makro (l0..l7) + 44 alt = **52 düğüm** | ADR-013 · ADR-028 |
| Proje | **9 üretim** + 1-2 test = en fazla 11 `.csproj` | ADR-028 §2.1 |
| Bacak | `net48` + `net10.0-windows` (çift TFM) | ADR-001 · ADR-019 |
| Farklılaştırıcı | 4 (hibrit bellek · wizard · deterministik anti-hallucination · free-model router) | §4 |

### §0.1 Platform anatomisi (prompt kaynağı: ai.04 §4)

```text
┌──────────────────────────────────────────────────────────────────────────┐
│                         VERSA CODER PLATFORM ANATOMİSİ                  │
├──────────────────────────────────────────────────────────────────────────┤
│  [ PRESENTATION  ] DevExpress 2026 WinForms · Docking · Realtime Graph  │
├──────────────────────────────────────────────────────────────────────────┤
│  [ APPLICATION   ] Intent Engine · Session Wizard · CQRS · Orchestrator │
├──────────────────────────────────────────────────────────────────────────┤
│  [ AGENT RUNTIME ] Swarm Coordinator · Role Matrix · Task Decomposer    │
│                    · Policy Gate                                        │
├──────────────────────────────────────────────────────────────────────────┤
│  [ PROTOCOL/MCP  ] MCP Client/Server · Tool Gateway · Plugin Sandbox    │
│                    · JSON-RPC                                           │
├──────────────────────────────────────────────────────────────────────────┤
│  [ INFRASTRUCTURE] Modüler DLL (Roslyn, Data, Git, Observability,       │
│                    Security)                                            │
├──────────────────────────────────────────────────────────────────────────┤
│  [ DOMAIN/CORE   ] Immutable Session State · Entities · Value Objects   │
│                    · Events                                             │
└──────────────────────────────────────────────────────────────────────────┘
```

### §0.2 Proje hangi sorunu çözer (7 problem — prompt kaynağı: ai.04 §4.8)

| # | Karşılaşılan kritik problem | Geleneksel AI sonucu | VersaCoder çözümü |
|---|---|---|---|
| 1 | Mimari disiplin eksikliği | Monolitik spagetti kod, God Class | Katmanlı mimari (l0..l7) + Roslyn katman koruması |
| 2 | Sığ & parçalı bağlam | Halüsinasyon, var olan kodun tekrar yazılması | AST semantik indeksleme + Epoch Assembly |
| 3 | Manuel mikro-yönetim | Her adımı tek tek promptlama | Multi-Agent Swarm ile otonom görev ayrıştırma |
| 4 | Araç & veri izolasyonu | Yalnız metin tamamlama, araçlara erişememe | Tool Gateway + MCP + Plugin Sandbox |
| 5 | Güvenlik & denetim riski | OWASP açıkları, sızan anahtarlar | AES-256 kasa, OWASP taraması, audit trail |
| 6 | Embedded / Driver yetersizliği | Donanım kısıtlarını bilmeme | Embedded/Driver katmanları + sektörel uzmanlık |
| 7 | Oturum hafıza kaybı | Her açılışta bağlamın unutulması | Knowledge Vault (.ai SSOT) + sürekli öğrenme |

### §0.3 Studio / IDE kimliği — kod + dokümantasyon + görsel üretim

> VersaCoder yalnız bir **kod IDE'si** değil, bir **Studio**'dur: yapay zekâyı
> daha iyi kullanmak için **dokümantasyon** ve **görsel üretim** de aynı
> çalışma ortamından yürütülür. Üç yetenek tek `l0..l6` çekirdeği üzerinde
> yaşar; ayrışma yalnız `l7` yüzeydedir (ADR-010).

```text
┌─────────────────────────────────────────────────────────────┐
│                  VERSACODER STUDIO / IDE                    │
├───────────────┬───────────────────┬─────────────────────────┤
│  KOD          │  DOKÜMANTASYON    │  GÖRSEL ÜRETİM          │
│  04-code-edit │  .ai vault (SSOT) │  05-ai-chat → prompt    │
│  03-solution  │  reports/spec     │  → render sağlayıcı     │
│  build/test   │  frontmatter+link │  → 06-agent-panel takip │
├───────────────┴───────────────────┴─────────────────────────┤
│  ORTAK ÇEKİRDEK: l0..l6  ·  Agent Runtime  ·  Policy Gate  │
│  (docs-writer · visual-director · developer aynı matriste)  │
└─────────────────────────────────────────────────────────────┘
```

| Yetenek | Ne üretir | Ajan (AGENTS §2) | Kanıt |
|---|---|---|---|
| **Kod** | `.cs` + build + test | `developer` → `reviewer` → `tester` | `dotnet build/test` çıktısı |
| **Dokümantasyon** | `.md` (vault/spec/report) | `docs-writer` | Frontmatter 8 alan + `check-links` = 0 |
| **Görsel üretim** | render prompt + görsel | `visual-director` | Render çıktısı ya da `[VERIFY]` parametre |

**Görsel üretim hattının ayrıntısı** [AGENTS](AGENTS.md) §8.5'tedir (8 adım:
INPUT → ANALYZE → UNDERSTAND → EXTRACT → PLAN → PROMPT → OPTIMIZE → FINAL);
**dil/teknik terim politikası** AGENTS §3.1'dedir. Bu dosya yalnız **niyet ve
ürün haritası** taşır (SRP).

**Sınır:** Görsel üretim yeteneği **tasarım kararıdır**; hangi render
sağlayıcısının entegre edileceği, model lisansları ve çıktı kalitesi ölçümü
henüz **planlıdır** → `DOĞRULAMA GEREKLI`. "Daha iyi görsel üretir" gibi
ölçülmemiş başarı iddiası yazılmaz (G9).

**Karar durumu:** Sağlayıcı entegrasyonu için taslak ADR yazıldı →
[ADR-031](.decisions/draft/ADR-031-visual-production-provider.md)
(`status: draft`; U1–U8 açık konular kapatılmadan `accepted` olmaz).

---

## §1 Ne Üretiyoruz

> **Misyon:** VersaCoder, geliştiriciye **kod yazdıran, görevi planlayan, tool çağrısını gösteren ve kanıtla teslim eden** ajan çalışma ortamı sunar. Tek pencere altında editör, ajan sohbeti, görev kuyruğu, terminal ve ayarlar yaşar; ağ kesilse bile yerel modelle (Ollama) akış sürer.

| Biz EVETİZ | Biz DEĞİLİZ |
|---|---|
| Masaüstü agentic IDE: WinForms + DevExpress v26.1, tek `RibbonForm` | Tarayıcı/SaaS IDE, VS Code eklentisi, VSIX |
| `net48` + `net10.0-windows` iki bacakta **aynı kaynak** | Tek bacaklı tasarım ("net48'e sonra bakarız") |
| tool çağrısı + diff + görev durumu **görünür** | Arka planda sessiz işlem yapan chatbot |
| Kanıt disiplini: build/test çıktısı = tek "bitti" kanıtı | "Muhtemelen derlenir" ile kapanan iş |
| Dar yığın: DevExpress v26.1 · Dapper/ADO.NET · xUnit | Genel amaçlı IDE, binlerce özellik, eklenti pazarı |

**Kapsam sınırı:** ne yapılmayacağı `PROJECTS.md` §Kapsam dışı tablosundadır; bu dosyaya kapsam maddesi yazmak **yasaktır** (tekrar = SSOT ihlali).

---

## §2 7 Ekran — Ne Yapar, Kim Kullanır, Hangi Katman Besler

> **Not (kanıt):** Katman sütunu ADR-028 §2.1'deki katman sorumluluklarından **türetilmiştir**; nihai sahiplik `architecture/index.md` ve `checklists/layer-boundary-check.md` ile denetlenir.

| # | Ekran | Ne yapar (2-3 cümle) | Kim kullanır | Besleyen katman |
|---|---|---|---|---|
| 01 | `01-start-page` | Açılışta son oturumları ve bekleyen görevleri listeler. Hızlı ajan başlangıcı ve model sağlık rozeti verir. Oturum bozuksa salt okunur açar, olay `log.md`'ye düşer. | **Tümü (P1-P3)** — günlük giriş noktası | `l6` Host (oturum yaşam döngüsü, DI) + `l3` Crosscutting (structured log) |
| 02 | `02-main-ide` | `RibbonForm` + `Office2010` ribbon altında tek pencere kabuğu; docking panelleri ve durum çubuğu ile diğer 6 ekranı barındırır. Bu ekran **kabuktur**, iş mantığı taşımaz. | **P2 ana**, P1 ve P3 de kullanır | `l7` UI (Desktop) + `l6` Host (composition root) |
| 03 | `03-solution-explorer` | Çözüm/proje/dosya ağacı, hızlı arama ve git durumu satırı. Dosya gezgini `04-code-editor`'e çift tıklamayla gider. Ajanın dosya işlemleri de burada işaretlenir. | **P1** (legacy gezinme) + P2 | `l4` Infrastructure (fs + git tool-runner) + `l2` Application (görünüm use-case) |
| 04 | `04-code-editor` | Sekmeli kod yüzeyi: satır numarası, arama, diff görünümü ve kaydetme kapısı. Ajan değişikliği onaylanmadan dosyaya yazılmaz (GATE-003 benzeri onay disiplini). | **Tümü** — işin kendisi | `l2` Application (düzenleme use-case) + `l4` (dosya yazma) + `l0` (`Result<T>` geri dönüşü) |
| 05 | `05-ai-chat` | Streaming yanıt, tool çağrısı satırı (read/edit/bash/grep), bağlam editörü ve diff onayı. Her çağrı ekranda **görünür**, gizli işlem yoktur. | **P2** ana ekranı; P1/P3 de | `l5` Protocol (mesaj şeması, tool sözleşmesi, SSE streaming) + `l6` Host (session) |
| 06 | `06-agent-panel` | Görev kuyruğu: bekliyor / çalışıyor / bitti+kanıt / RED durumları ve yeniden çalıştır. Tool akışının kalıcı log karşılığı burada yaşar. | **P2** + **P1** (denetim) | `l2` Application (durum makinesi) + `l3` (log) + `l5` Protocol |
| 07 | `07-settings` | Model/endpoint seçimi, secret kasası, tema/skin, dil ve hedef bacak. Bağlantı testi butonu zorunludur; çevrimdışıysa yalnız ilgili provider pasifleşir. | **P3** (kurulum) + proje sahibi | `l4` Infrastructure (DPAPI secret store, provider abstraction) + `l6` Host |

### §2.1 Ekranlar Arası Zorunlu Akışlar

| # | Akış | Adım | Kural |
|---|---|---|---|
| F1 | Giriş | 01 → 02 → 03 | Kabuksuz ekran açılmaz |
| F2 | Ajan görevi | 03 → 05 → 06 | Görev "bitti" sayılması için kanıt bağlantısı şart |
| F3 | UI değişikliği | 05 (talep) → onay → 06 → 04 | Onaysız `.Form.Designer.cs` yazılmaz (ADR-025) |
| F4 | Ayar değişimi | 07 → ilgili ekran | Model/tema değişimi oturumu kırmaz |
| F5 | Hata / RED | 06 → bulgu → 05 düzeltme → 04 | Bulgu biçimi: konum + neden + düzeltme |

### §2.2 CLI (`VersaCoder.Cli`)

`VersaCoder.Cli`, Desktop ile **l0..l6 çekirdeğini birebir paylaşır**; ayrışma yalnız `l7`'dedir ve çekirdeğe `DevExpress.*` / `System.Windows.Forms` referansı girmez (ADR-010). Ekran karşılıkları: `05-ai-chat` → stdout/JSON akışı, `06-agent-panel` → durum satırı + exit code, `07-settings` → `--flag` + yerel ayar dosyası. Kullanım senaryosu: CI, otomasyon, betik.

---

### §2.3 `01-start-page` — Açılış, son oturum, model sağlığı

**Ne gösterir:** Uygulamanın ilk yüzeyidir: son oturumların kısa listesi,
bekleyen görevlerin sayısı ve model sağlık rozeti. Kullanıcı burada "hangi
işe devam edeceğine" karar verir; dosya ya da kod göstermez, yalnız **durum**
gösterir.

**Kullanıcı akışı (4 adım):**
1. `VersaCoder.Desktop` açılır; `l6` Host oturum deposunu okur ve listeyi doldurur.
2. Kullanıcı ya son oturumlardan birini seçer ya da "yeni oturum" başlatır.
3. Model sağlık rozeti okunur; sağlayıcı erişilebilir değilse rozet uyarıya döner (yerel provider devrede kalır).
4. Kabuk `02-main-ide` üzerinden açılır; açılış hatası olay olarak `log.md`'ye yazılır.

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l6` Host | Oturum yaşam döngüsü, açılış bağımlılıkları, composition root |
| `l3` Crosscutting | Structured log — açılış olayları buraya yazılır |
| `l5` Protocol | Sağlayıcı sağlık sinyali (erişilebilirlik rozeti kaynağı) |
| `l0` Abstractions | Liste öğesi sözleşmesi (`Result<T>` ile hata dönüşü) |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Son oturum listesi | `l6` Host | Uygulama içi oturum deposu | Yerleşim `DOĞRULAMA GEREKLI` |
| Bekleyen görev sayısı | `l2` Application | Görev kuyruğu durumu | ADR-028 kapsamı |
| Model sağlık rozeti | `l5` → `l4` | Sağlayıcı uç noktası testi | Sıra ADR-011 |
| Açılış olayı | `l3` | Structured log → `log.md` | Kanıt zorunlu (G5) |

**Erişilebilirlik notu:** Rozet renkle tek başına bilgi taşımaz; yanında
metin etiketi zorunludur. Liste tamamen klavye ile gezilebilmeli, odak
halkası görünür kalmalıdır — bu madde bir **tasarım gereksinimidir**, ölçüm
yapılmamıştır → `DOĞRULAMA GEREKLI`.

### §2.4 `02-main-ide` — Ribbon kabuğu ve docking pencere

**Ne gösterir:** Tek `RibbonForm` + `RibbonControlStyle.Office2010` altında
durum çubuğu ve docking panelleri; diğer 6 ekranın **taşıyıcısıdır**. İş
mantığı, veri ya da dosya erişimi barındırmaz — barındırırsa kabuk işlevi
kaybolur ve ekranlar arası akış (§2.1) kırılır.

**Kullanıcı akışı (4 adım):**
1. Kabuk açılır; panel düzeni ve tema `l6` üzerinden uygulanır.
2. Kullanıcı ribbon grubundan bir işlev seçer; komut ilgili ekrana yönlendirilir.
3. Seçilen panel (03..07) docking alanında açılır; diğerleri görünür kalır.
4. Durum çubuğu aktif işlemin kısa özetini gösterir; kapanışta düzen kaydedilir (kayıt yeri `DOĞRULAMA GEREKLI`).

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l7` UI (Desktop) | Ribbon, docking, durum çubuğu — yalnız kabuk |
| `l6` Host | Composition root: ekranların bağlanması |
| `l3` Crosscutting | Ribbon komutu tetiklenen olayların log'u |
| `l0` Abstractions | Komut sözleşmesi (ekranlar arası tek tip imza) |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Ribbon komut tablosu | `l7` | Sabit komut tanımı | ADR-025 |
| Tema / skin | `l6` | Ayar deposu (`07-settings`) | F4 akışı |
| Panel düzeni | `l7` | Yerel yapılandırma | `DOĞRULAMA GEREKLI` |
| Aktif işlem özeti | `l2` | Görev durumu (06) | ADR-028 |

**Erişilebilirlik notu:** Ribbon kısayolları klavyeyle erişilebilir olmalı,
her ribbon düğmesinin adı okunabilir metni olmalıdır (adlandırma
`glossary.md` ile hizalanır). Ribbon'da yalnız ikon taşıyan düğme
`DOĞRULAMA GEREKLI` sayılır — erişilebilirlik ölçümü yapılmamıştır.

### §2.5 `03-solution-explorer` — Çözüm ağacı ve git durumu

**Ne gösterir:** Çözüm / proje / dosya ağacı, hızlı arama alanı ve dosya
başındaki git durumu satırı. Ajanın dosya işlemleri (oluştur, sil, taşı)
burada işaretle görünür; ekran **gezinme** yüzeyidir, içerik göstermez
(içerik `04-code-editor`'dedir).

**Kullanıcı akışı (4 adım):**
1. Kabuk açılışında ağaç `l4` üzerinden dosya sisteminden okunur.
2. Kullanıcı arama alanına yazar; eşleşmeler ağaçta vurgulanır.
3. Dosyaya çift tıklanır → `04-code-editor` o dosyayla açılır (F1 akışı).
4. Ajan dosyada değişiklik yaptığında satır işareti güncellenir; git satırı tazelenir.

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l4` Infrastructure | Dosya sistemi okuma + git tool-runner |
| `l2` Application | Görünüm use-case: ağaç, arama, işaretleme |
| `l5` Protocol | Ajan dosya olayının ekrana taşınması |
| `l3` Crosscutting | Dosya işlemi log'u |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Çözüm/proje/dosya ağacı | `l4` | Dosya sistemi | ADR-005 (ADO.NET dışı, fs katmanı) |
| Git durumu satırı | `l4` | git tool-runner çıktısı | `DOĞRULAMA GEREKLI` |
| Ajan dosya işareti | `l5` → `l2` | Tool çağrısı akışı | ADR-010 |
| Arama sonucu | `l2` | Yerel indeks | `DOĞRULAMA GEREKLI` |

**Erişilebilirlik notu:** Ağaç düğümleri artır/azalt tuşlarıyla
gösterilmelidir; yalnız renk ile "değişti" bilgisi verilemez, yanına metin
işareti gerekir. Ağaçta sanal (lazy) yükleme varsa durum metni
(`DOĞRULAMA GEREKLI`) zorunludur.

### §2.6 `04-code-editor` — Sekmeli kod, arama ve diff

**Ne gösterir:** Satır numaralı, sekmeli kod yüzeyi; arama kutusu, diff
görünümü ve kaydetme kapısı. Ajanın önerdiği değişiklik burada **onaylanmadan
diske yazılmaz**; onay disiplini GATE-003 ile aynı mantığı taşır.

**Kullanıcı akışı (5 adım):**
1. 03'ten çift tıklama ya da 05'teki diff onayı ile dosya açılır.
2. Kullanıcı düzenler; dosya tamponsuz (unsaved) işaretlenir.
3. 05'ten gelen diff önerisi yan yana görünümde gösterilir.
4. Onay verilirse yazma `l4` üzerinden gerçekleşir; reddedilirse dosya değişmez.
5. Kaydetme sonucu (başarı/hata) durum çubuğuna ve `log.md`'ye düşer.

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l2` Application | Düzenleme use-case'i, diff birleştirme kararı |
| `l4` Infrastructure | Dosya okuma / yazma (onay sonrası) |
| `l0` Abstractions | `Result<T>` ile geri dönüş; hata metni uydurulmaz |
| `l7` UI (Desktop) | Yüzey: sekmeler, satır, arama, diff görünümü |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Dosya içeriği | `l4` | Dosya sistemi (salt okuma) | ADR-005 |
| Diff önerisi | `l5` → `l2` | Tool çağrısı sonucu | ADR-010 |
| Onay kararı | `l7` → `l6` | Kullanıcı etkileşimi | ADR-025 (onaysız yazım yasak) |
| Yazma sonucu | `l4` | I/O sonucu → `Result<T>` | ADR-017 / ADR-019 |

**Erişilebilirlik notu:** Satır numarası ve odak satırı kontrastı
ölçülmemiştir → `DOĞRULAMA GEREKLI`. Diff'te eklenen/silinen satır **yalnız
renkle** ayırt edilemez; `+`/`-` işareti zorunludur. Arama kutusunda sonuç
sayısı metinle bildirilir.

### §2.7 `05-ai-chat` — Streaming yanıt, tool çağrısı, diff onayı

**Ne gösterir:** Kullanıcı ile ajan arasındaki konuşma yüzeyi: streaming
yanıt satırı, her tool çağrısının (read / edit / bash / grep) görünür satırı,
bağlam editörü ve diff onay bloğu. **Gizli işlem yoktur** — çağrının kendisi
ekranda yazılır.

**Kullanıcı akışı (5 adım):**
1. Kullanıcı isteğini yazar; `l6` oturumu başlatır, `l5` mesaj şemasına çevirir.
2. Sağlayıcı yanıtı streaming olarak akar; tampon 8 MB olursa yerinde compaction (ADR-008).
3. Tool çağrısı gelir; satır olarak gösterilir, çağrının sonucu satırın altında özetlenir.
4. Değişiklik önerisi diff bloğu olarak açılır; kullanıcı onaylar/reddeder.
5. Onay → `04`'e yazım talebi; red → çağrının dosya etkisi geri alınmaz (zaten yazılmadı).

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l5` Protocol | Mesaj şeması, tool sözleşmesi, SSE streaming |
| `l6` Host | Oturum, sağlayıcı seçimi, yeniden deneme |
| `l2` Application | Diff üretimi ve onay durum makinesi |
| `l3` Crosscutting | Tool çağrısı ve karar log'u |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Streaming yanıt | `l5` | Sağlayıcı SSE akışı | ADR-011 (ücretsiz → yerel → açık) |
| Tool çağrısı satırı | `l5` | Tool sözleşmesi | ADR-010 |
| Diff önerisi | `l5` → `l2` | Tool sonucu + yerel karşılaştırma | `DOĞRULAMA GEREKLI` |
| Bağlam / transcript | `l6` | Oturum içi bellek | ADR-008 (8 MB compaction) |

**Erişilebilirlik notu:** Yeni gelen token'lar için okuyucu (screen reader)
sürekli yeniden okumamalıdır; yanıt tamamlandığında **odakla**
bildirilebilmelidir (`DOĞRULAMA GEREKLI`). Tool satırı yalnız ikon değil,
açık metin taşır; diff onay butonlarının etiketi eylem adıdır (Onayla /
Reddet).

### §2.8 `06-agent-panel` — Görev kuyruğu ve kanıt bağlantısı

**Ne gösterir:** Görev kuyruğunun durum satırları: bekliyor / çalışıyor /
bitti + kanıt / RED. Her görevin kanıt bağlantısı burada görünür; **kanıt
olmadan "bitti" yazısı üretilmez**. Yeniden çalıştır düğmesi de buradadır.

**Kullanıcı akışı (4 adım):**
1. 05'te başlayan görev buraya kuyruğa düşer; durumu `bekliyor` olur.
2. Çalışırken tool akışı izlenir; kalıcı karşılığı log katmanına yazılır.
3. Görev biterse kanıt bağlantısı (komut çıktısı / test sonucu) eklenir ve durum `bitti + kanıt` olur.
4. Kanıt yoksa durum `RED` kalır; kullanıcı düzeltmeyi 05'e yazıp tekrar dener (F5 akışı).

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l2` Application | Görev durum makinesi (bekliyor → çalışıyor → bitti/RED) |
| `l3` Crosscutting | Kalıcı log karşılığı — tool akışının arşivi |
| `l5` Protocol | Görev ve tool olaylarının taşıyıcısı |
| `l6` Host | Kuyruk yaşam döngüsü, yeniden deneme politikası |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Görev satırı + durum | `l2` | Durum makinesi | WORKFLOW §4 kanıt kutusu |
| Kanıt bağlantısı | `l2` | Build/test çıktısı, satır no | G5 (kanıtsız teslim yasak) |
| Kalıcı tool log'u | `l3` → `log.md` | Structured log | Tarihli kayıt zorunlu |
| RED bulgusu | `l5` → `l2` | Gate RED dönüşü | F5 akışı |

**Erişilebilirlik notu:** Durum **yalnız renkle** verilemez; metin etiketi
zorunludur (`RED` yazısı gibi). Yeniden çalıştır düğmesi klavyeyle
erişilebilir olmalı, tekrar eden işlem için sayısal geri bildirim
verilmelidir — ölçüm yok → `DOĞRULAMA GEREKLI`.

### §2.9 `07-settings` — Model, secret, tema, bacak

**Ne gösterir:** Model/endpoint seçimi, secret kasası, tema ve dil seçimi,
hedef bacak tercihi. Zorunlu bileşen **Bağlantı testi** düğmesidir;
çevrimdışı olunduğunda yalnız ilgili sağlayıcı pasifleşir, ekran çalışmaz
kalmaz.

**Kullanıcı akışı (5 adım):**
1. Kullanıcı ayarlar panelini açar; mevcut değerler `l6` ve `l4`'ten okunur.
2. Sağlayıcı ve model seçilir; anahtar yalnız şifreli depoya yazılır (DPAPI).
3. "Bağlantı testi" basılır; sonuç rozet olarak döner (01'deki rozetin kaynağı).
4. Tema/dil değişir; değişiklik oturumu kırar (F4 akışı).
5. Kapatıldığında değerler yerel ayar dosyasına yazılır; CLI aynı ayarı `--flag` ile okur.

**Besleyen katman (ADR-028):**

| Katman | Bu ekrandaki görevi |
|---|---|
| `l4` Infrastructure | DPAPI secret store + sağlayıcı soyutlaması |
| `l6` Host | Ayar yaşam döngüsü, geçerli değerlerin uygulanması |
| `l5` Protocol | Sağlayıcı uç noktası ve streaming yapılandırması |
| `l7` UI (Desktop) | Panel yüzeyi, tema uygulaması |

**Gelen veri:**

| Veri | Gelen katman | Kaynak | Kanıt durumu |
|---|---|---|---|
| Model/endpoint listesi | `l5` | Yapılandırılmış sağlayıcı listesi | ADR-011 sırası |
| Secret değeri | `l4` | DPAPI şifreli depo (asla düz metin log'a) | `DOĞRULAMA GEREKLI` (kasa yerleşimi) |
| Tema / dil | `l6` | Yerel ayar dosyası | F4 akışı |
| Hedef bacak tercihi | `l6` | Yapılandırma | İkili bacak ADR-001 / ADR-019 |

**Erişilebilirlik notu:** Bağlantı testi sonucu yalnız renk değil metin
olmalıdır; secret alanı kopyalamaya izin verirken parola alanı davranışı
korunur (`DOĞRULAMA GEREKLI`). Ayar panelinde her alanın etiketi görünür
olmalı, ipucu yalnız tooltip'e sıkışmamalıdır.

### §2.10 CLI ↔ 7 ekran ilişkisi (hangi işlem nereden yapılır)

CLI, Desktop ile `l0..l6` çekirdeğini paylaşır; ayrışma yalnız `l7`'dedir.
Aşağıdaki tablo bir işin **ekrandan mı, CLIden mı** yapıldığını ve kaynağını
yazır. Kaynak görülmüşse yazılmış, **görülmemişse `DOĞRULAMA GEREKLI`**
olarak bırakılmıştır.

| # | İşlem | Ekran (GUI) | CLI karşılığı | Kaynak not |
|---|---|---|---|---|
| 1 | Oturumu listele / aç | `01-start-page` | Aynı oturum listesi CLIden de okunur | `VC-SES-015` (`spec/session-spec.md`) — CLI ve Desktop aynı listeyi görür |
| 2 | Görev çalıştır / durdur | `06-agent-panel` | Headless çalıştırma + exit code ile sonucu | `ADR-010` — CLI çekirdek paylaşımı; komut adı `DOĞRULAMA GEREKLI` |
| 3 | Yanıt akışını izleme | `05-ai-chat` | stdout / JSON akışı | Bölüm §2.2 (mevcut metin) |
| 4 | Model / endpoint değiştirme | `07-settings` | `--flag` + yerel ayar dosyası | Bölüm §2.2 (mevcut metin); bayrak adları `DOĞRULAMA GEREKLI` |
| 5 | Dosya gezinme + diff onayı | `03-solution-explorer` + `04-code-editor` | Karşılığı yok (CLI dosyayı göstermez) | `DOĞRULAMA GEREKLI` |
| 6 | UI kodu / `.Designer.cs` değişikliği | `05` → onay → `06` → `04` (F3 akışı) | **CLIden yapılmaz**; tasarım sahipliği `ui-designer`'da | `U5` (`.rules/ui-rules.md`) — `.Designer.cs` / `.resx` sahipliği |
| 7 | Katman sınırı denetimi | Ekran değil — gate çıktısı | NetArchTest çalıştırması | `ADR-028` §2.3 şart 2 (matris = test) |
| 8 | Vault belgesi düzenleme | Ekran değil — ajan işi | Yok | [AGENTS](AGENTS.md) §1 matrisi |

**Ortak kural:** Aynı veri **iki yerde ayrı ayrı güncellenmez**; ekran ne
yazarsa CLI onu okur, tersi de geçerlidir (kaynak: `VC-SES-015`). İkisi farklı
değer gösterirse çakışma protokolü (G10) çalışır, son değer yazılmaz.

---

## §3 Farklılaştırıcılar (4)

| # | Farklılaştırıcı | Bir cümlede | Somut karşılık | Kanıt durumu |
|---|---|---|---|---|
| 1 | **Hibrit bellek** | Oturum içi bellek (transcript, görev durumu) ile kalıcı `.ai` vault'u çift katmanlı olarak birlikte çalışır: oturum kapanınca kural kaybolmaz. | Oturum başında vault okunur, sonunda `log.md` + `MEMORY.md` güncellenir; transcript 8 MB'da yerinde compaction (ADR-008). | Vault tarafı **kabul edilmiş**; ürün içi "çift katmanlı hibrit bellek" motoru **planlı** (DOĞRULAMA GEREKLI) |
| 2 | **Wizard** | Niyet (Intent) → sihirbaz → değişmez yapılandırma → tool/policy gate zinciri; kullanıcı tek yüksek seviyeli istek yazar, adım adım onay verir. | `02-main-ide` kabuğunda proje/oturum sihirbazı; onay kapıları ADR-009 ve ADR-025 ile kilitli. | Onay modeli **kabul edilmiş**; wizard motorunun ayrı spec'i **yok** (DOĞRULAMA GEREKLI) |
| 3 | **Deterministik anti-hallucination** | Uydurma yasağı yalnız metin kuralı değil, **çalışan araç** olarak kurulur: sayımlar script ile, katman yönü NetArchTest ile, "bitti" yalnız build/test çıktısı ile kanıtlanır. | `scripts/count-components.ps1` (ADR-027) · NetArchTest matrisi (ADR-028 §2.3) · 7 doğrulama etiketi · `Result<T>` (ADR-017/019). | **Kabul edilmiş** (ADR-017, 019, 027, 028) |
| 4 | **Free-model router** | Sağlayıcı sırası: OpenRouter `/free` → Ollama (yerel) → OpenAI-uyumlu uç; her sağlayıcıda streaming zorunlu, anahtar yalnız şifreli depodan okunur. | `l5` Protocol + `l6` Host yapılandırması; offline-first davranış (ağ yoksa yerel provider devrede, UI bloke olmaz). | Sıra **kabul edilmiş** (ADR-011); ücretsiz model havuzu envanteri **planlı** (DOĞRULAMA GEREKLI) |

### §3.1 Hibrit bellek — oturum içi + kalıcı vault çift katmanı

VersaCoder belleği iki yerde tutar: **oturum içi** (transcript, görev durumu,
tool akışı) ve **kalıcı** (`.ai` vault'u, `MEMORY.md`, `log.md`). Amacı,
uygulama kapanınca kuralın ve kararın kaybolmamasıdır: oturum başında vault
okunur, oturum sonunda kazanım vault'a yazılır. Ürün tarafındaki "çift
katmanlı bellek motoru" henüz yazılmamıştır; var olan şey, bu davranışı
**zorunlu kılan süreç kurallarıdır** — bu ayrım kritiktir, çünkü aksi halde
"özellik var" iddiası kanıtsız olur.

| Katman / bileşen | Rol | Ne saklar | Ne zaman yazılır | Kanıt durumu |
|---|---|---|---|---|
| Oturum içi bellek | `l6` Host | Transcript, görev durumu, tool akışı | Akış sürerken; 8 MB'da yerinde compaction | ADR-008 (kural) |
| Kalıcı vault | `.ai/` dosyaları | Kural, karar, envanter, rapor | Oturum sonunda, gate geçişinde | Vault kanıtları (gate) |
| Bellek özeti | `MEMORY.md` | Oturumlar arası durum | Oturum kapanışında | Zorunlu, silinemez |
| Günlük | `log.md` | Kural değişikliği + kalite raporu | Değişiklik anında, tarihli | Tarihli kayıt zorunlu |
| Ürün içi motor (planlı) | `l5` + `l6` | Oturum/kalıcı senkronu | — | **DOĞRULAMA GEREKLI** |

**Sınır:** Vault tarafı **kabul edilmiş** (işleyen süreç); ürün içi otomatik
senkron **planlı**. İkisini aynı cümleye yazmak bu dosyadaki en kolay
kanıtsız iddia hatasıdır.

### §3.2 Wizard — niyetin adım adım onaya bağlanması

Wizard, kullanıcının tek yüksek seviyeli isteğini (niyet) alınca onu
**adımlara böler** ve her adımda açık onay ister; onay verilmeden tool
çağrısı ve dosya yazımı gerçekleşmez. Böylece "hızlı olsun" diye yıkıcı işlem
otomatikleşmez. Onay modelinin kendisi kilitli (ADR-009 / ADR-025); eksik
olan, sihirbazın kendi adım kataloğu ve durum makinesi için ayrı bir
spesifikasyondur.

| Adım | Ne olur | Onay türü | Engel | Kaynak |
|---|---|---|---|---|
| 1. Niyet | Kullanıcı tek cümle yazar | Yok (girdi) | — | ADR-009 (niyet modeli) |
| 2. Plan | Sihirbaz adımları listeler | Bilgilendirme | Adım listesi yoksa devam yok | **DOĞRULAMA GEREKLI** |
| 3. Yapılandırma | Sabit ayarlar gösterilir | Açık onay | Onaysız kaydetme yok | ADR-025 |
| 4. Tool/policy gate | Tool çağrısı zinciri çalışır | Kademeli onay | Yıkıcı işlem = ek onay | ADR-009 |
| 5. Sonuç + kanıt | Build/test çıktısı eklenir | Kabul / RED | Kanıtsız "bitti" yok | G5 |

**Sınır:** Onay modeli **kabul edilmiş**; wizard motorunun ayrı spec'i
**yok** → wizard ekranı `02-main-ide` kabuğu içinde geçici çözümle
açılabilir, bu da `DOĞRULAMA GEREKLI` bir tasarım kararıdır.

### §3.3 Deterministik anti-hallucination — uydurmaya karşı çalışan araç

"Uydurma yasağı" yalnız bir üslup kuralı değil, **çalışan araç** olarak
kurulur: sayımlar script ile alınır, katman yönü NetArchTest ile denetlenir,
"bitti" iddiası yalnız build/test çıktısıyla söylenir. Amaç, modelin
hatasını **insan gözünün** değil, **aracın** yakalamasıdır; araç yoksa iddia
yeniden doğrulanamaz ve güven kişisel kalır.

| Araç | Ne ölçer | Çıktı | Ne zaman çalışır | Kaynak |
|---|---|---|---|---|
| `scripts/count-components.ps1` | Katalog kayıt sayısı | Sayı + PASS/RED | Vault gate'inde | ADR-027 |
| NetArchTest 8x8 matris | Katman yönü ihlali | Test sonucu | Build/test sırasında | ADR-028 §2.3 |
| `scripts/check-links.ps1` | Bozuk link | BROKEN sayacı | Gate kapanışında | Vault kuralı |
| 7 doğrulama etiketi | İddia güvenilirliği | Etiket (kanıtlı / `DOĞRULAMA GEREKLI` / `NEEDS_VALIDATION` ...) | Belge yazımında | CLAUDE.md guardrail'ları |
| `Result<T>` dönüşü | Hata saklama | Tipi değişen sonuç | Kod içinde | ADR-017 / ADR-019 |

**Sınır:** Bu maddenin iddiası "daha az halüsinasyon" **değildir** — ölçülmüş
başarı iddiası yazılamaz. İddia, **mekanizmanın varlığıdır**; doğrulaması
gate çıktısı ve `log.md` üzerinden yapılır.

### §3.4 Free-model router — sağlayıcı sırası ve çevrimdışı dayanıklılık

Router, tek sağlayıcıya bağlanmaz: önce OpenRouter `/free` havuzu, sonra
yerel Ollama, sonda OpenAI-uyumlu uç denenir; her adımda streaming zorunlu,
anahtar yalnız şifreli depodan okunur. Gerçek faydası, **ağ kesilince işin
durması değil**, yerel modelle akışın sürdürülmesidir — bu da P3 (tek
kişilik ürün sahibi) vaadinin taşıyıcısıdır.

| # | Sıra | Sağlayıcı | Ne zaman devrede | Anahtar kaynağı | Kanıt durumu |
|---|---|---|---|---|---|
| 1 | Birinci | OpenRouter `/free` | Ücretsiz havuz erişilebilirken | Şifreli depo (DPAPI) | Sıra ADR-011 |
| 2 | İkinci | Ollama (yerel) | Ağ yokken / birinciden dönüyorken | Yerel uç nokta (anahtar yok) | Offline-first davranış |
| 3 | Üçüncü | OpenAI-uyumlu uç | Yapılandırmışsa | Şifreli depo | **DOĞRULAMA GEREKLI** |
| 4 | Hepsi | Streaming zorunlu | Her sağlayıcıda | — | ADR-011 |
| 5 | Red | Erişilemeyen model | İstek anında | — | 2026-09-29 kaza kaydı |

**Sınır:** Sıra **kabul edilmiş**; ücretsiz model havuzunun güncel envanteri
(bazı modellerin düşmesi) **planlı** → envanter yoksa `07-settings` listesi
elle doğrulanmalıdır (`DOĞRULAMA GEREKLI`).

> **UYARI:** Bu dört madde **iddia değil, tasarım kararı**dır. "Daha iyi IDE" gibi ölçülmüş başarı iddiası yazılamaz; ölçüm `WORKFLOW.md` gate kanıtları ve `log.md` üzerinden yapılır (kanıtsız iddia yasağı).

---

## §4 Kim Kullanır (3 Persona)

| # | Persona | Ana acı | VersaCoder vaadi | Ana ekranları |
|---|---|---|---|---|
| P1 | **Legacy .NET koruyucusu** (8+ yıl .NET Framework/DevExpress) | Ajan "modern" diye kodu bozar, build kırılır | Üretim çıktısı `net48`'de de derlenir; iki bacak eşit doğrulanır | 03, 04, 06, 07 |
| P2 | **Ajan odaklı geliştirici** (junior/orta) | Ajanın ne yaptığını göremiyor | tool çağrısı + diff görünür, görev kuyruğu izlenebilir | 05, 06, 04 |
| P3 | **Tek kişilik ürün sahibi (indie .NET)** | Kurulum maliyeti: IDE + LLM + ayarlar ayrı | Tek pencere, yerel modelle çevrimdışı çalışma, ayar tek yerde | 01, 07, 02 |

Persona listesi **tasarım girdisidir**; doğrulanması pazar araştırması ile değil, ürün içi kullanım gözlemiyle yapılır → `DOĞRULAMA GEREKLI`.

---

## §5 Non-Goals (özet — ayrıntı `PROJECTS.md`)

| # | Yapılmaz | Neden |
|---|---|---|
| 1 | EF Core / herhangi bir tam ORM | `net48` bacağında desteklenmez; ADO.NET + Dapper (ADR-005/016) |
| 2 | WPF · WinUI · MAUI · Web UI | DevExpress WinForms yığını sabit; tek platform |
| 3 | VS Code eklentisi / VSIX / eklenti pazarı | Kendi host'umuz var |
| 4 | macOS / Linux hedefi | `net10.0-windows` + WinForms Windows'a bağlı |
| 5 | CI/CD hattı | **Henüz yok** (durum, non-goal değil) — repo 2026-09-29'da yalnız `.ai/` + `.git/` |

---

### §5.1 Kapsam dışı envanter — ne yapılmaz, nereden biliniyor

§5 beş maddenin özetidir; bu tablo **envanterin kendisi değil**, kararın
hangi belgede durduğunu gösteren **göstergedir** (envanter SSOT'u
[PROJECTS](PROJECTS.md) §Kapsam dışı'dır). "Yapılmaz" sütunu kalıcı karar,
"Kaynak" sütunu onu veren belgedir; kaynak görülmemiş satır
`DOĞRULAMA GEREKLI` ile işaretlenir ve yazılmadan geçer.

| # | Yapılmaz | Neden yapılmaz | Kaynak |
|---|---|---|---|
| 1 | EF Core / herhangi bir tam ORM | `net48` bacağında taşınabilirliği yok; veri erişimi ADO.NET + Dapper, her sorgu parametreli | ADR-005 (bkz. [adr-index](.decisions/adr-index.md)) |
| 2 | Migration aracı (EF benzeri) | Migration = saf SQL + version tablosu; otomasyon katmanı kurulmaz | [new-doc-contract](.templates/new-doc-contract.md) §4 |
| 3 | WPF · WinUI · MAUI · Web UI | DevExpress WinForms yığını sabit; tek platform, tek kabuk | ADR-004 çizgisi (bkz. [adr-index](.decisions/adr-index.md)) |
| 4 | Tarayıcı / SaaS IDE, VS Code eklentisi, VSIX | Kendi host'umuz var; eklenti pazarı iş kapsamı değil | Bölüm §1 "Biz DEĞİLİZ" |
| 5 | macOS / Linux hedefi | `net10.0-windows` + WinForms Windows'a bağlı | Bölüm §1 "Biz DEĞİLİZ" |
| 6 | CI/CD hattı | **Henüz yok** (durum, non-goal değil): repo 2026-09-29'da yalnız `.ai/` + `.git/` | Bölüm §5 madde 5 |
| 7 | K3'un 618 DLL'lik tier envanterinin kataloga girmesi | Tek sayim birimi `N###` kayittir; türetilmiş veri rapor satırıdır | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.2 |
| 8 | 8. ve üzeri ekran / 16 wireframe hedefi | 7 ekran sabittir; eski wireframe referanstır | Bölüm §7 Red Team madde 2 |
| 9 | DevExpress sürüm karması (birden çok v sürümü) | Tek sürüm: Universal 2026 / v26.1 | [new-doc-contract](.templates/new-doc-contract.md) §4 |
| 10 | EF Core içeren herhangi bir "yardımcı" paket | Paket, taşıma yasağını dolaylı yoldan kırar | ADR-005 + [new-doc-contract](.templates/new-doc-contract.md) §4 |
| 11 | Model eğitimi / kendi LLM'imizi barındırmak | Router yalnız **çağrı** yapar; eğitim ve ağırlık saklama yok | `DOĞRULAMA GEREKLI` |
| 12 | Eklenti / genişletilebilir eklenti pazarı | Kapalı kapsam; uzantılar vault (`.ai`) ile yapılır | `DOĞRULAMA GEREKLI` |

**Okuma kuralı:** 1-10 numaralı satırlar **kaynaklı karar**dır, pazarlık
konusu değildir; 11-12 ise henüz yazılmamış kapsamdır ve kapıdan geçerken
`DOĞRULAMA GEREKLI` etiketi taşır. Kapsam dışı bir iş istenirse bu dosyada
"neden yapmayız" aranmaz — karar burada değil, **kaynak belgede** durur
(SSOT): kapsam envanteri [PROJECTS](PROJECTS.md)'tedir.

---

## §6 Başarı Ölçütleri (nitel, kanıt zorunlu)

| # | Ölçüt | Kabul kanıtı | Kanıt olmayan ifade |
|---|---|---|---|
| O1 | İki bacakta build 0 hata | `dotnet build` çıktısı (ADR-019) | "derleniyor gibi" |
| O2 | Testler yeşil + mimari yön 0 ihlal | `dotnet test` + NetArchTest çıktısı | "katmanlara uygun" |
| O3 | Vault'ta 0 bozuk link | `scripts/check-links.ps1` | "linkler tamam" |
| O4 | Katalog ≥ 5000 kayıt | `scripts/count-components.ps1` (ADR-027) | elle sayılmış "5000+" |
| O5 | Kullanıcı onaysız UI kodu yok | GATE-003 onay kaydı (ADR-025) | "onaylandı diye düşünüyorum" |

---

## §7 Red Team (Sık Yapılan Hatalar)

| # | Hata | Doğrusu |
|---|---|---|
| 1 | Bu dosyaya bileşen/kapsam envanteri yazmak | Envanter `PROJECTS.md`'dedir; burada yalnız **niyet** |
| 2 | Ekran sayısını 8 veya 16 yapmak | **7** sabit (ADR-025); 16 wireframe yalnız eski referanstır |
| 3 | Farklılaştırıcıları "başarıldı" diye yazmak | Hepsi tasarım kararı; kanıt gate çıktısıdır |
| 4 | Katman sütununu kesin sahiplik sanmak | Türetilmiştir; kesinleşmesi `architecture/index.md` |
| 5 | Unicode kutu çizimi kullanmak | Yalnız ASCII (`+ - | < >`) |

---

## §8 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-29`
- [ ] H1 + tek cümlelik sorumluluk + §0 Özet (Answer First) var
- [ ] 7 ekran adı ADR-025 ile birebir; her satırda 2-3 cümle + kullanıcı + katman dolu
- [ ] ASCII diyagramda Unicode kutu karakteri yok
- [ ] Bağlantılar yalnız izinli hedeflerde (index, PROJECTS, brain, keys, glossary, WORKFLOW, CLAUDE, ADR-028, architecture/index)
- [ ] Sayılar kaynaklı: 7 ekran · 8 katman · 52 düğüm · 9+1-2 proje
- [ ] Kanıtsız başarı iddiası yok; belirsiz maddeler `DOĞRULAMA GEREKLI` işaretli
- [ ] Boyut 30-88 KB bandında (hedef 32-45 KB; ±%10 marj)

---

## §9 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Katman/proje baz kararı (8/52/9) | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 2 | Kapsam, proje listesi, risk defteri | `PROJECTS.md` |
| 3 | ADR özeti ve kod kuralları | `brain.md` |
| 4 | Anahtar kelime → dosya | `keys.md` |
| 5 | Terimler (TR/EN) | `glossary.md` |
| 6 | Süreç ve gate'ler | `WORKFLOW.md` |
| 7 | Guardrail G1..G12 | `CLAUDE.md` |
| 8 | Katman haritası | `architecture/index.md` |
| 9 | Prompt kaynağı (platform anatomisi, 7 problem) | `G:/…/versacoder proje planlaması ai.04.md` |
| 10 | Prompt kaynağı (proje tanımı, "Proje nedir") | `G:/…/versacoder proje planlaması ai.02.md` |
| ROLE | [ROLE](ROLE.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| MEMORY | [MEMORY](MEMORY.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [log](log.md) | append-only is gunlugu + Quality Report |
| ULTRA | [ULTRA](ULTRA-THINKING.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| kok README | [kok README](../README.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../CLAUDE.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../AGENTS.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../WORKFLOW.md) | kok kapi ozeti (gate sirasi tek satir) |
