---
title: "VersaCoder — Genel Mimari Şablonu (K0..K8 Katman Yığını)"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "genel mimari sablonu: K0..K8 katman yigini, sorumluluklar ve bagimlilik kurallari"
---

# VersaCoder — Genel Mimari Şablonu (K0..K8 Katman Yığını)

**Zorunlu Bağlantılar:** [[.templates/index]] · [[../CLAUDE.md]] · [[../adr/adr-architecture-template]] · [[ascii-diagram-template]] · [[layer-spec-template]] · [[dependency-rule-template]]

---

## §1 Amaç

### §1.1 Bu şablon ne üretir

Bu şablon, `.ai/architecture/` altına yazılan **genel mimari belgelerinin** (`tiers.md`, `project-tree.md`, `dependency-flow.md`, `technology-decisions.md`) tek iskeletidir. Bir mimari belge; katman yığını, proje ağacı, bağımlılık yönü, arayüz sözleşmeleri, karar özeti ve teknoloji gerekçesini **aynı sayfada** ve **kanıtlanabilir** biçimde taşır. Kural basittir: katman sırası, TFM ataması ve veri yığını üç ayrı yerde yazılarsa biri er geç yalan söyler; bu şablon hepsini tek tablo ailesine bağlar.

SRP gerekçesi: bu dosya yalnız **iskelet** üretir. Katmanın teknik şartnamesi `layer-spec-template`'in, bağımlılık yasağı `dependency-rule-template`'in, çizim `ascii-diagram-template`'in işidir; burada tekrarlanırsa üç SSOT doğar.

| Alan | Değer |
|------|-------|
| Şablon / Yol | `architecture-template.md` · `.ai/.templates/architecture/architecture-template.md` |
| Hedef Dosya | `.ai/architecture/<konu>.md` → `C:\versacoder\.ai\architecture\` (asla `C:\www\...`) |
| Üretim Anı | yeni katman/proje/TFM/karar değişiminde |
| Sorumlu Rol | `architect` · denetim `reviewer` · kabul `master-orchestrator` |
| Gate | `GATE-004 architecture` |
| Bağımlı Şablonlar | `layer-spec-template` · `dependency-rule-template` · `ascii-diagram-template` |
| Kodlama | UTF-8 (BOM'suz) — ç ğ ı İ ö ş ü doğru; mojibake YASAK |
| Versiyon / Authority | `1.0.0` / `SSOT` |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz; satır `⚠️ VERIFICATION REQUIRED` olarak kalır ve **hiçbir yorum yazılmaz**.

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|-------|--------------------------|-------|
| E1 | `.ai/.templates/architecture/` klasörü var | `ascii-diagram-template.md` (37,2 KB / 644 satır) | ✅ Doğrulandı |
| E2 | Şablon envanteri `.ai/.templates/index.md` | 606 satır / 37,9 KB — 44 şablon kaydı | ✅ Doğrulandı |
| E3 | Dizin kuralı `.ai/.templates/CLAUDE.md` | 132 satır / 6,9 KB | ✅ Doğrulandı |
| E4 | Altın ADR şablonu `.ai/.templates/adr/adr-template.md` | klasörde **yok**; 7 varyant var (`adr-architecture` vb.) | ❌ YOK |
| E5 | `.ai/architecture/index.md` (katman gerçeği) | `.ai/` altında yalnız `.templates/` klasörü var | ❌ YOK |
| E6 | `.ai/WORKFLOW.md` (GATE-000…005) | dosya yok | ❌ YOK |
| E7 | `.ai/.rules/` = 7 MD | `.rules/` klasörü yok | ❌ YOK |
| E8 | `.ai/keys.md` · `.ai/log.md` | dosyalar yok | ❌ YOK |
| E9 | `C:\versacoder` kökü = `.ai` + `.git` | `Get-ChildItem -Force` çıktısı 2 kayıt | ✅ Doğrulandı |
| E10 | `VersaCoder.sln`, `Directory.Build.props`, `src/**` | kökte kaynak kod yok | ❌ YOK |
| E11 | ADR-001 / ADR-004 / ADR-005 / ADR-006 numaraları | `.ai/adr/` klasörü yok; yalnız şablon metinlerinde geçiyor | ⚠️ VERIFICATION REQUIRED |
| E12 | DevExpress 25.2.8 · `net48` + `net10.0-windows` · SQLite `versacoder.db` | kod/sln/props kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E13 | Bu dosya yazıldı | `.ai/.templates/architecture/architecture-template.md` | ✅ Doğrulandı |

**Yorum yasağı:** `❌ YOK` ve `⚠️` satırları için "şurada olmalıydı", "muhtemelen vardır", "eskiden vardı" gibi açıklama yazılmaz. Boşluk ya §5.1 Adım 2'de kapatılır ya da satır olduğu gibi kalır.

### §1.3 Ne Zaman Bu Şablon Kullanılır

| # | Tetikleyici | Bu Şablon? | Çıktı |
|---|-------------|------------|-------|
| 1 | Yeni katman/proje eklendi veya biri kaldırıldı | ✅ Evet | `.ai/architecture/project-tree.md` |
| 2 | Bağımlılık yönü ilk kez tanımlanıyor / ihlal şüphesi | ✅ Evet | `.ai/architecture/dependency-flow.md` |
| 3 | TFM (`net48`, `net10.0-windows`) ataması veya teknoloji kararı (DevExpress, Dapper, SQLite) değişiyor | ✅ Evet | `.ai/architecture/tiers.md` · `technology-decisions.md` |
| 4 | Tek katman şartnamesi, ihlal taraması ya da tek bir ADR | ❌ → `layer-spec` · `dependency-rule` · ADR şablonu | ilgili şablonun hedefi |

### §1.4 Kapsam Dışı ve Özet

| Kapsam (BU ŞABLON) | Kapsam Dışı (hangi şablon) |
|--------------------|----------------------------|
| K0..K8 katman yığını + proje ağacı + ikili TFM ataması | Katmanın iç sınıf tasarımı (`layer-spec`) |
| Bağımlılık yönü (genel akış) | 9×9 ihlal matrisi + tarama komutu (`dependency-rule`) |
| Arayüz sözleşmeleri listesi (portlar) | Arayüzün imza düzeyinde şartnamesi (`layer-spec`) |
| Mimari karar özeti + teknoloji gerekçesi (ADR-001/004/005/006) | Kararın alternatif bölümü, sürüm kilidi (ADR şablonu) |
| Katmanlar arası iletişim anlatımı | ASCII çizim üretimi (`ascii-diagram-template`) |

---

## §2 Kapsam

Kapsam: mimari katman, proje ağacı + TFM, bağımlılık yönü, port sözleşmeleri, ADR özeti ve teknoloji gerekçesi. Kapsam dışı: sınıf içi tasarım, ihlal tarama komutu (→ `dependency-rule-template`), method imzası (→ `layer-spec-template`), sürüm bump süreci (ADR şablonu).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya / Nesne | Sorumlu Rol | Bu Şablondan mı? | Not |
|---|---------------|-------------|-------------------|-----|
| 1 | `.ai/architecture/<konu>.md` | `architect` | ✅ Evet | bu şablonun tek çıktısı |
| 2 | `.ai/architecture/index.md` (kök özet) | `architect` | Kısmen | yalnız §3.2/§3.4 özetlenir, SRP gereği tekrar yok |
| 3 | `.ai/architecture/layer-<ad>.md` · `dependency-rule.md` | `architect` | ❌ | `layer-spec-template` · `dependency-rule-template` |
| 4 | `.ai/adr/ADR-NNN-*.md` | `architect` | ❌ | karar ≠ mimari belge |
| 5 | `*.sln`, `*.csproj`, `Directory.Build.props`, `src/**`, `.ai/.rules/*.md` | `developer` / `reviewer` | Kaynak | şablon bu dosyaları **okur**, yazmaz |
| 6 | `.ai/.templates/index.md` | `docs-writer` | Kayıt | üretim sonrası tek satır `Durum` güncellenir |

### §2.2 Kapsam Sınırı Testi (4 soru)

Üretilecek dosyaya sırayla sor; **ilk `HAYIR`** yanlış şablonu işaret eder:

| # | Soru | Beklenen | Beklenmezse |
|---|------|----------|-------------|
| S1 | Dosya iki veya daha çok katmanı/projenin **genel** yapısını anlatıyor mu? | `EVET` | tek katman → `layer-spec-template` |
| S2 | İçinde 9×9 bağımlılık matrisi veya ihlal komutu var mı? | `HAYIR` | varsa → `dependency-rule-template` |
| S3 | İçinde ASCII kutu/çizim bloğu var mı (üretilecekse)? | `EVET` | çizim kuralı → `ascii-diagram-template` |
| S4 | Konu geri alınabilir tek bir **karar** mı? | `HAYIR` | evetse → ADR şablonu |

S1 `HAYIR` ise bu şablon fazladır; S2 `EVET` ise bu şablon **yanlış** şablondur.

---

## §3 Mimari

### §3.1 Üretilen Dosyanın İskeleti (frontmatter + H1)

```yaml
---
title: "Mimari — <konu> (VersaCoder)"
type: architecture
category: architecture
status: draft            # draft | review | accepted
version: 0.1.0
updated: 2026-09-25
authority: SSOT
related_gates: [GATE-004]
related_adrs: [ADR-001, ADR-004, ADR-005, ADR-006]
template: architecture-template.md
---
```

```markdown
# Mimari — <konu>
**Zorunlu Bağlantılar:** [[../ARCHITECTURE]] · [[../WORKFLOW]] · [[../adr/index]] · [[layer-<ad>]] · [[dependency-rule]] · [[../.templates/index]]
```

Üretilen dosyanın 7 bölümü bu şablonun §1..§7'siyle **aynı adları** taşır (tutarsız bölüm adı = §6.2 R3).

### §3.2 K0..K8 Katman Yığını

K kodları sabittir; her mimari belgede aynı anlam taşır. **K8 bu şablonun ekidir** (kod katmanı değil, denetim katmanıdır).

| Kod | Katman | Proje | Temel Sorumluluk | Kim referans verebilir |
|-----|--------|-------|------------------|------------------------|
| K8 | Test | `tests/VersaCoder.Tests`, `tests/VersaCoder.IntegrationTests` | kanıt üretir | yalnız test projesi (üretim kodu K8'e bakamaz) |
| K7 | Host | `VersaCoder.Host` | composition root, DI, `appsettings*.json` | K6 (UI), K5, K4, K0 |
| K6 | UI | `VersaCoder.Desktop`, `VersaCoder.Cli` | RibbonForm sunumu / CLI ayrıştırma | yalnız K7 |
| K5 | Protocol | `VersaCoder.Protocol` | mesaj şeması, tool sözleşmeleri | K7, K6 |
| K4 | Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, dosya, git, LLM sağlayıcı | K7, K5 |
| K3 | Application | `VersaCoder.Application` | use case, CQRS handler, orkestrasyon | K7, K5, K4 |
| K2 | Domain | `VersaCoder.Domain` | entity, invariant, domain event | K7, K5, K4, K3 |
| K1 | Abstractions | `VersaCoder.Abstractions` | port arayüzleri, `Result<T>`, event sözleşmesi | hepsi (en alt sabit) |
| K0 | CrossCutting | `VersaCoder.CrossCutting` | logging, `IClock`, dosya/yardımcılar | yalnız dışa bakar — kimse ona referans vermez |

```text
+----------------------------------------------------------------+
| K8  Test              tests/VersaCoder.Tests · IntegrationTests|
+----------------------------------------------------------------+
| K7  Host              composition root + DI + appsettings      |
| K6  UI                Desktop (RibbonForm) + Cli (parser)      |
+----------------------------------------------------------------+
| K5  Protocol          mesaj şeması + tool sözleşmeleri         |
| K4  Infrastructure    ADO.NET + Dapper · dosya · git · LLM     |
+----------------------------------------------------------------+
| K3  Application       use case + CQRS handler                  |
| K2  Domain            entity + invariant + domain event        |
| K1  Abstractions      contract + result + event                |
+----------------------------------------------------------------+
| K0  CrossCutting      logging · zaman · yardımcılar            |
+----------------------------------------------------------------+
```

**Okuma kuralı:** en üst = en çok bağımlı olan, en alt = en az bağımlı olan. Aşağı doğru ok gider, yukarı ok **ihlaldir** (§4.5 G7). Çizim bu şablonun işi değil → `ascii-diagram-template` §3.3 (T1).

### §3.3 Proje Ağacı ve İkili TFM Ataması

| # | Proje | TFM `net48` | TFM `net10.0-windows` | Uygulama | Not |
|---|-------|-------------|------------------------|----------|-----|
| 1 | `VersaCoder.Abstractions` | ✅ | ✅ | — | 6 katman kütüphanesinden biri |
| 2 | `VersaCoder.Domain` | ✅ | ✅ | — | saf C#, paket bağımlılığı yok |
| 3 | `VersaCoder.Application` | ✅ | ✅ | — | use case + handler |
| 4 | `VersaCoder.Infrastructure` | ✅ | ✅ | — | Dapper + SQLite `versacoder.db` |
| 5 | `VersaCoder.Protocol` | ✅ | ✅ | — | mesaj şeması |
| 6 | `VersaCoder.CrossCutting` | ✅ | ✅ | — | logging, `IClock`, yardımcılar |
| 7 | `VersaCoder.Host` | ✅ | ✅ | — | DI + `appsettings*.json` |
| 8 | `VersaCoder.Desktop` | ✅ | — | WinForms `RibbonForm` | DevExpress 25.2.8 |
| 9 | `VersaCoder.Cli` | — | ✅ | konsol ayrıştırıcı | .NET 10 davranışı |
| 10 | `tests/VersaCoder.Tests` | — | ✅ | — | unit, hedef %90 coverage |
| 11 | `tests/VersaCoder.IntegrationTests` | — | ✅ | — | SQLite dosya ile |

**Toplam 11 proje · 2 TFM · 20 derleme hedefi.** `Directory.Build.props` içinde `TargetFrameworks = net48;net10.0-windows` tek tanımdır; projeler tek hedefe düşmez (§4.5 G8). DevExpress 25.2.8 her iki hedefte de aynı sürümdür (§3.6 ADR-001).

### §3.4 Bağımlılık Yönü (genel akış)

| # | Kaynak | Hedef (izinli) | Yön | İstisna |
|---|--------|----------------|-----|---------|
| 1 | K2 Domain | K1 Abstractions | ↓ | Yok |
| 2 | K3 Application | K2, K1 | ↓ | Yok |
| 3 | K4 Infrastructure | K3, K2, K1 | ↓ | K3 yalnız port üzerinden |
| 4 | K5 Protocol | K3, K1 | ↓ | Yok |
| 5 | K6 UI (Desktop/Cli) | K7 Host | ↓ | katmanlara **asla** doğrudan |
| 6 | K7 Host | K5, K4, K3, K2, K1, K0 | ↓ | tek kabuk (composition root) |
| 7 | K0 CrossCutting | dış dünya (log, dosya, zaman) | ↓ yalnız dışa | hiçbir katman K0'a referans **vermez** |
| 8 | K8 Test | herhangi bir katman | ↓ | üretim kodu K8'e bakamaz |
| 9 | Ters `ProjectReference` | — | ↑ **YASAK** | hedef: 0 (§5.3 komut 3) |

Genel akış tek satırda: `K6 UI → K7 Host → {K5 Protocol, K4 Infrastructure} → K3 Application → K2 Domain → K1 Abstractions → K0 CrossCutting`.

### §3.5 Arayüz Sözleşmeleri (Portlar)

Sözleşme = `K1 Abstractions` içinde yaşar; **uygulaması** `K4 Infrastructure`'tedir. Bu listeye yeni port eklemek `layer-spec` gerektirir.

| # | Port | Ne Taşır | Üreten Katman | Tüketen Katman |
|---|------|----------|---------------|----------------|
| 1 | `I{X}Repository` | kalıcı veri erişimi (Dapper sorgusu) | K4 | K3 |
| 2 | `IUnitOfWork` | işlem sınırı + commit/rollback | K4 | K3 |
| 3 | `IUseCase<TIn,TOut>` | use case girişi | K3 | K7, K6 |
| 4 | `IQueryHandler<TQuery,TResult>` | sorgu sonucu | K3 | K3 (mediator içi) |
| 5 | `ICommandHandler<TCommand>` | yazma komutu | K3 | K3 |
| 6 | `IClock` | saati test edilebilir kılmak | K0 | tümü |
| 7 | `ILlmClient` | model çağrısı (sağlayıcı değiştirilebilir) | K4 | K3, K6 |
| 8 | `Result<T>` | başarı/başarısızlık sözleşmesi (exception değil) | K1 | tümü |

### §3.6 Mimari Karar Özeti Tablosu

Karar metni `.ai/adr/`'dedir; buraya yalnız **öz** girer (SRP). Numaralar şablon metinlerinden gelir, disk kanıtı yok → §1.2 E11.

| ADR | Karar (özet) | Etkilenen Katman | Etki | Kanıt |
|-----|--------------|-------------------|------|-------|
| ADR-001 | DevExpress **25.2.8** sürümü kilitlendi | K6 UI | Sürüm değişirse tüm Form/Designer yeniden gözden geçer | ⚠️ VERIFICATION REQUIRED |
| ADR-004 | **EF Core yerine** ADO.NET + Dapper (`@p1..@pN`) | K4 | ORM sihrini kaldırır; SQL görünür kalır | ⚠️ VERIFICATION REQUIRED |
| ADR-005 | `net48` dalı korunur (DevExpress/legacy kanıt) | K0–K7 | Çift derleme, `#if` alanı | ⚠️ VERIFICATION REQUIRED |
| ADR-006 | `net10.0-windows` modern dalı | K0–K5, K8 | Yeni API yüzeyi | ⚠️ VERIFICATION REQUIRED |
| — | SQLite `versacoder.db` + `schema_version` + saf `.sql` | K4 | Tek dosya DB, migration V0xx | ⚠️ VERIFICATION REQUIRED |

### §3.7 Teknoloji Seçimi Gerekçesi

| # | Karar | Reddedilen | Neden | Geri Dönüş |
|---|-------|------------|-------|------------|
| 1 | ADO.NET + Dapper | EF Core, ORM sihri | Sorgu görünür, paket ince, ADR-004 | Sadece ADR-004 supersede edilerek |
| 2 | Parameterized SQL `@p1..@pN` | string concat | Injection yüzeyi sıfır (§4.5 G10) | Yok — kalıcı |
| 3 | İkili TFM | Tek `net10.0` | DevExpress 25.2.8 + legacy kanıt aynı kalmalı | ADR-005/006 |
| 4 | SQLite `versacoder.db` | PostgreSQL/SQL Server | Yerel, dosya tabanlı, migration saf SQL | ADR-004 kapsamı |
| 5 | DevExpress 25.2.8 | Yükseltilmiş sürüm | Sürüm kilidi ADR-001; sürüm = tüm kontrol kırılabilir | Yeni ADR gerekir |
| 6 | Port/adapter (`I*Repository`) | Doğrudan DbContext/SQL | Test edilebilirlik + katman izolasyonu | Yok |
| 7 | `Result<T>` | Exception ile hata akışı | Hata bir değerdir; K6'da UI'a taşınabilir | Yok |

### §3.8 Katmanlar Arası İletişim

| # | Adım | Ne Olur | Taşınan | Hata Yolu |
|---|------|---------|---------|-----------|
| 1 | K6 → K7 | Form/CLI, Host'a use case'i sorar | `IUseCase<TIn,TOut>` | — |
| 2 | K7 → K3 | DI, doğru handler'ı çözümler | komut/query nesnesi | çözümlenemezse `Result.Fail` |
| 3 | K3 → K2 | Invariant ve kural doğrulaması | domain entity | ihlal → gate (§4.5 G11) |
| 4 | K3 → K4 | Port üzerinden kalıcı yazma | `IUnitOfWork` | Dapper hatası → `Result.Fail` + log |
| 5 | K4 → DB | Parametreli sorgu (`@p1..@pN`) | `IDbConnection` (SQLite) | zaman aşımı → log + retry (tek) |
| 6 | K6 | `Result<T>`'yi alır ve UI'a çevirir | ekran durumu | exception asla dışarı sızmaz; kullanıcı dili Türkçe |

**Zaman bütçesi (hedef):** K6→K3 ≤5 ms, K3 invariants ≤2 ms, K4 sorgu ≤50 ms (yerel SQLite), toplam ekran yan ≤150 ms.

### §3.9 Tam Kalıp İskeleti (kopyala-yapıştır)

````markdown
---
title: "Mimari — {{KONU}} (VersaCoder)"
type: architecture
category: architecture
status: {{DRAFT|REVIEW|ACCEPTED}}
version: {{X.Y.Z}}
updated: {{YYYY-MM-DD}}
authority: SSOT
related_gates: [GATE-004]
related_adrs: [{{ADR_LISTESI}}]
template: architecture-template.md
---

# Mimari — {{KONU}}
**Zorunlu Bağlantılar:** [[{{BAG_1}}]] · [[{{BAG_2}}]] · [[{{BAG_3}}]] · [[{{BAG_4}}]] · [[{{BAG_5}}]] · [[{{BAG_6}}]]

## §1 Amaç
### §1.1 Amaç   {{NE ÜRETİR — tek paragraf}}
### §1.2 Disk kanıtları
| # | İddia | Disk Kanıtı | Durum |
|---|-------|-------------|-------|
| {{E1}} | {{IDDIYA}} | {{YOL}} | {{✅ Doğrulandı|⚠️ VERIFICATION REQUIRED}} |
### §1.3 Tetikleyiciler  {{NE ZAMAN KULLANILIR}}
### §1.4 Özet            {{KAPSAM | KAPSAM DIŞI}}
## §2 Kapsam
### §2.1 Dosya tipi → sorumlu  {{TABLO}} · §2.2 Sınır testi {{S1..S4}}
## §3 Mimari
### §3.1 İskelet  {{frontmatter + H1}}
### §3.2 K0..K8 yığını  {{TABLO + ASCII kutu 66}}
### §3.3 Proje ağacı + TFM  {{11 proje × 2 TFM}}
### §3.4 Bağımlılık yönü  {{kaynak → hedef → yön → istisna}}
### §3.5 Arayüz sözleşmeleri  {{I* port tablosu}}
### §3.6 Karar özeti  {{ADR no + özet + kanıt}}
### §3.7 Teknoloji gerekçesi  {{seçim | reddedilen | neden}}
### §3.8 Katmanlar arası iletişim  {{adım | ne olur | hata yolu}}
### §3.9 Bu kalıp  {{sen buradasın}}
## §4 Kurallar
### §4.1 Yazım sırası · §4.2 Onay · §4.3 Ölçek · §4.4 Değişiklik · §4.5 G1..G14
## §5 Workflow
### §5.1 Adımlar · §5.2 Hata modları · §5.3 Salt-okunur PowerShell · §5.4 Quality Report
## §6 Doğrulama
### §6.1 Kontrol listesi · §6.2 Red sinyalleri · §6.3 Geçiş · §6.4 İlgili · §6.5 Envantere özgü
## §7 Referanslar
### §7.1 VersaCoder bağlantısı · §7.2 Red Team ({{EN_AZ_10_SATIR}})
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Sıra | Ne Yapılır | Çıktı | Yapılmazsa |
|---|------|-----------|-------|------------|
| 1 | Kanıt + şablon seç | §1.2 disk taraması, §2.2 S1..S4 | kanıt tablosu + doğru şablon | Kanıtsız iddia (G5) |
| 2 | Katmanı sabitle | §3.2 K0..K8 tablosu | 9 satır | Kod değişince belge çürür |
| 3 | Ağacı yaz | §3.3 proje + TFM | 11 satır | TFM kayması derlemede patlar |
| 4 | Yönü çiz | §3.4 + `ascii-diagram` T1 | yön tablosu | Ters referans gizlenir |
| 5 | Sözleşmeyi ve kararı bağla | §3.5 portlar + §3.6 ADR | port listesi + ADR bağlantısı | ADR'siz mimari karar (G9) |
| 6 | Doğrula | §6.1 checklist + §5.3 | işaretli liste + 0 ihlal | Kanıtsız "tamam" |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|------------|-------|------|
| 1 | `architect` | §3.2–§3.8 doluluğu + tutarlılık | taslak (`draft`) | 45 dk |
| 2 | `reviewer` | §6.1 checklist + §5.3 komut çıktıları | rapor | 30 dk |
| 3 | `testing` | §3.5 portlarının test edilebilirliği + %90 etkisi | not | 20 dk |
| 4 | `master-orchestrator` | `GATE-004` + §6.4 Quality Report | `accepted` + `log.md` | 25 dk |

**Toplam bütçe: 120 dakika.** 3 kırmızı sonrası iş durur ve gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır aralığı | 480–660 | aşım → §5.2 H7 |
| KB aralığı | 15–32 | aşım → kısalt, içerik değil iskelet |
| Tablo satırı | ≥25 (her §'de en az 1 tablo) | az → §6.1 K7 red |
| Bölüm | §1…§7 + §1.1–§1.4, §2.1–§2.2, §3.1–§3.9, §4.1–§4.5, §5.1–§5.4, §6.1–§6.5, §7.1–§7.2 | eksik → red |
| Kutu genişliği | 66 karakter (§5.3 komut 6) | hizasız → `ascii-diagram` G2 |
| Kodlama | UTF-8 BOM'suz | mojibake → otomatik red |

### §4.4 Değişiklik Kontrolü

| Değişiklik | `version` | `status` | Ek İşlem |
|------------|-----------|----------|----------|
| Yazım hatası, kırık bağlantı | `patch` | Aynı | `updated` değişmez |
| Yeni tablo/alt bölüm (`§x.y`) | `minor` | Aynı | §6.4 Quality Report |
| K katmanı eklendi/çıkarıldı | **`major`** | `draft` → `review` | `index.md` + ADR + `layer-spec` yeniden üretilir |
| Yalnız ASCII dolgu düzeltmesi | Aynı | Aynı | §5.3 komut 6 yeniden alınır |
| Belge `accepted` oldu | `minor` | `accepted` | `.ai/log.md` tek satır |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | Kural | İhlal Sonucu |
|---|-----------|-------|--------------|
| G1 | Kopyalama yasak | `C:\www\coremusic.net\.ai` ve `C:\www\versacoder\versacoder.ai` içeriği kopyalanamaz; yalnız yapı referanstır | Belge **reddedilir**, silinir |
| G2 | EF Core yasak | Veri erişimi **ADO.NET + Dapper**, `@p1..@pN` | **DUR** — §3.6/§3.7 ADR-004 satırı düzeltilir |
| G3 | Frontmatter zorunlu | 7 alan + `authority: SSOT` ilk 9 satırda | **DUR** — belge `draft` kalır |
| G4 | Yer tutucu temizliği | Üretilen belgede çift süslü parantezli tutucu = 0 (yalnız §3.9) | **DUR** — doldurulmadan teslim yok |
| G5 | Kanıt zorunlu | Matris/tablo olmadan "doğru" iddiası yazılamaz | `⚠️ VERIFICATION REQUIRED` + §1.2 |
| G6 | Uzunluk | 480–660 satır / 15–32 KB dışı | **DUR** — ölçüm (§5.3/1) geçene kadar |
| G7 | Bağımlılık yönü | Yukarı ok / ters `ProjectReference` gösterilemez | **DUR** — §3.4 yeniden yazılır |
| G8 | TFM sabiti | Yalnız `net48`, `net10.0-windows`; 3. TFM yazılamaz | Belge `draft` + ADR-005/006'ya dön |
| G9 | ADR'siz karar yok | `.ai/adr/` dışında karar belgesi üretilmez | **DUR** — karar ADR'ye taşınır |
| G10 | SQL güvenliği | String concat sorgu anlatılamaz; her örnek `@pN` içerir | Belge reddedilir |
| G11 | Kapı görünür | Mimariye dair gate (`GATE-004`) etiketi yoksa "onaylandı" denemez | `draft` + §6.3'e dön |
| G12 | Mojibake / dil | Bozuk UTF-8 artığı (§5.3 komut 2 = 0 olmalı) veya İngilizce gövde metni | Dosya UTF-8 BOM'suz yeniden yazılır |
| G13 | Ürün adı | VERSA CODER / VersaCoder; başka ürün adı geçemez | Satır silinir |
| G14 | Çıktı yolu | `C:\versacoder\...`; `C:\www\...` yol üretilemez | **DUR** — yol düzeltilir |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Disk taraması | `.ai/`, kök, `src/` | §1.2 kanıt tablosu | `architect` | 10 dk |
| 2 | Boşlukları kapat veya işaretle | §1.2 `❌/⚠️` satırları | kapatılmış satır ya da `⚠️` | `architect` | 15 dk |
| 3 | Şablon doğrulama | §2.2 S1..S4 | "bu şablon" kararı | `architect` | 3 dk |
| 4 | K0..K8 + proje ağacı | §3.2, §3.3 | 2 tablo | `architect` | 20 dk |
| 5 | Bağımlılık yönü | §3.4 | yön tablosu + T1 çizim | `architect` | 15 dk |
| 6 | Port + karar + gerekçe + iletişim | §3.5–§3.8 | 4 tablo | `architect` | 30 dk |
| 7 | Doğrulama | §5.3 komut 1–6 | 0 ihlal | `reviewer` | 10 dk |
| 8 | Checklist + rapor + kabul | §6.1, §6.4 | işaretli liste + rapor + `accepted` | `reviewer` → `master-orchestrator` | 27 dk |

**Zaman bütçesi:** adım 1–6 ≈ 93 dk, 7–8 ≈ 37 dk → **≈ 130 dk/mimari belge** (§4.2 120 dk bütçesi + %8 tampon).

### §5.2 Hata Modları

| Kod | Hata modu | Belirti | Kurtarma | Guardrail |
|-----|-----------|---------|----------|-----------|
| H1 | Kanıtsız iddia | §1.2'de `⚠️` varken "doğru" denmiş | Tarama komutuyla kanıt üret ya da iddiayı sil | G5 |
| H2 | Ters bağımlılık anlatımı | §3.4'te yukarı ok | Yönü interfaz (K1) üzerinden yeniden kur | G7 |
| H3 | EF Core sızması | §3.6/§3.7'de ORM sözü | ADO.NET + Dapper cümlesine çevir | G2 |
| H4 | 3. TFM | §3.3'te farklı moniker | `net48` / `net10.0-windows` dışına çıkma | G8 |
| H5 | Port listesi kodla çelişiyor | §3.5'te olmayan port kodda var | Belgeyi kod hizasına çek, `draft`'a al | §4.4 |
| H6 | Yer tutucu sızıntısı | Üretimde çift süslü parantezli tutucu | §3.9 dışı temizle, yeniden ölç | G4 |
| H7 | Uzunluk/KB aşımı | §5.3/1 > 660 satır veya >32 KB | İçerik değil **tekrar** kısalt | G6 |
| H8 | ADR numarası uydurma | Diskte `.ai/adr/` yokken kesin numara | `⚠️ VERIFICATION REQUIRED` ile işaretle | G9, G5 |

### §5.3 Salt-Okunur Doğrulama Komutları

```powershell
# 1) SATIR + KB — hedef: 480-660 satır ve 15-32 KB
$f = 'C:\versacoder\.ai\.templates\architecture\architecture-template.md'
[PSCustomObject]@{ Satir = (Get-Content -LiteralPath $f).Count;
                   KB = [math]::Round((Get-Item -LiteralPath $f).Length / 1KB, 1) }

# 2) MOJIBAKE — gerçek bozuk kodlama 0 olmalı (desen kod nokta ile kurulur)
$pat = -join ([char]0x00C3, [char]0x00C2, [char]0xFFFD)
Select-String -Path 'C:\versacoder\.ai\.templates\architecture\*.md' -Pattern $pat |
  Measure-Object | Select-Object -ExpandProperty Count

# 3) BÖLÜM SAYISI — §1..§7 başlık sayısı 7 olmalı
(Select-String -Path $f -Pattern '^## §[1-7] ').Count

# 4) TABLO YOĞUNLUĞU — tablo satırı en az 25
(Select-String -Path $f -Pattern '^\|' | Measure-Object).Count

# 5) YER TUTUCU — kalıp bloğu dışındaki tutucu 0 olmalı
(Select-String -Path $f -Pattern '\{\{[A-Z0-9_]+\}\}' |
  Where-Object { $_.LineNumber -lt 300 -or $_.LineNumber -gt 420 }).Count

# 6) KUTU HİZASI — uyuşmayan satır 0 olmalı (66 karakter)
$box = $false; $bad = 0
Get-Content -LiteralPath $f -Encoding UTF8 | ForEach-Object {
  if ($_ -match '^````markdown') { $box = $true }
  elseif ($_ -match '^````' -and $box) { $box = $false }
  if (-not $box -and $_ -match '^[+|]' -and $_.Length -ne 66) { $bad++; $_ }
}
"huysuz satir: $bad (0 olmali)"
```

**Geçme kriteri:** 1 → `480–660` ve `15–32`; 2 → `0`; 3 → `7`; 4 → `≥25`; 5 → `0`; 6 → `0`.

### §5.4 Quality Report

```markdown
## Quality Report — architecture-template (YYYY-MM-DD HH:MM)
- Ölçüm:          <satır> satır / <KB> KB   (hedef 480–660 · 15–32)
- Bölümler:       §1..§7 = <7/7>  ·  alt bölüm <NN/NN>
- Tablo satırı:   <N> (≥25)  ·  Kutu hizası: <0/uyuşmayan> (66)
- Yer tutucu:     0 (§3.9 dışı)  ·  Mojibake: 0  ·  Kırık bağlantı: 0
- Disk kanıtları: ✅ <N>  ·  ⚠️ <N>  ·  ❌ <N>  → yorum satırı 0
- Guardrail:      G1..G14 → ihlal yok  ·  Red sinyali: §6.2 → 0
- Checklist:      §6.1 → <NN>/<NN> geç  ·  Part: B
- Onay:           architect → reviewer → master-orchestrator (GATE-004)
- Karar:          ONAYLANDI | RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] 01 Frontmatter 7 alan eksiksiz + `authority: SSOT`
- [ ] 02 H1 + `**Zorunlu Bağlantılar:**` (6 wiki bağlantı)
- [ ] 03 §1.1–§1.4 dolu; §1.2'de her satır `✅` / `❌` / `⚠️ VERIFICATION REQUIRED`
- [ ] 04 §1.2 sonunda yorum yasağı cümlesi var
- [ ] 05 §2.1 dosya tipi → sorumlu tablosu + §2.2 S1..S4 cevaplanmış (S1 EVET, S2 HAYIR)
- [ ] 06 §3.2 K0..K8 = 9 katman satırı + ASCII kutu (66)
- [ ] 07 §3.3 11 proje × 2 TFM hücresi dolu
- [ ] 08 §3.4 bağımlılık yönü tablosu + "ters hedef: 0"
- [ ] 09 §3.5 port listesi ≥8 satır · §3.6 ADR + kanıt sütunu dolu
- [ ] 10 §3.7 gerekçe tablosu ≥7 satır · §3.8 iletişim + ms bütçesi
- [ ] 11 §3.9 kalıp bloğu çift aralıklı markdown içinde, yer tutucu yalnız orada
- [ ] 12 §4.5 G1..G14 dolu, "İhlal Sonucu" sütunu boş değil
- [ ] 13 §5.1 adım tablosu + süre bütçesi · §5.2 ≥8 hata modu
- [ ] 14 §5.3 ≥5 salt-okunur komut (`Get-*`/`Select-*`/`Test-Path`) + §5.4 rapor bloğu
- [ ] 15 §6.5 envantere özgü kontroller doldurulmuş · §7.2 Red Team ≥10 satır
- [ ] 16 Uzunluk 480–660 / 15–32 KB · Mojibake 0 · Kutu 66'ya hizalı (§5.3/1, /2, /6)

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Sonuç | Sorumlu |
|---|--------|-------|---------|
| R1 | §1.2'de `⚠️` varken §3.6'da kesin "uygulanmıştır" denmiş | **DUR** — iddia `⚠️`'ye çekilir | `architect` |
| R2 | §3.2'de 9'dan farklı K kodu | **DUR** — K0..K8 sabittir | `architect` |
| R3 | Bölüm adları bu şablonla uyuşmuyor | **DUR** — §1..§7 eşleşmeli | `reviewer` |
| R4 | `EntityFramework` / ORM geçen satır | **DUR** — G2 ihlali | `reviewer` |
| R5 | 3. bir TFM adı | **DUR** — G8 ihlali | `architect` |
| R6 | Kutu satırı ≠ 66 | **DUR** — `ascii-diagram` §5.3/1 | `architect` |
| R7 | Yer tutucu §3.9 dışında · uzunluk 480–660 dışı | **DUR** — G4, G6 | `reviewer` |

### §6.3 Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan |
|-------|----------------------|-----------|
| `draft` → `review` | §6.1 16/16 · §5.3 komutları geç · §1.2 eksiksiz | `architect` |
| `review` → `accepted` | Ters bağımlılık 0 · ADR bağları mevcut · %90 etkisi notu | `master-orchestrator` |
| `accepted` → `superseded` | K katmanı değişti ya da ADR supersede · geri dönüş serbest | Yeni belge + `log.md` |

### §6.4 İlgili Dosyalar

| Dosya | İlişki | Ne Verir |
|-------|--------|----------|
| `.ai/.templates/index.md` | Envanter (SSOT) | satır #13, Part B, KB |
| `.ai/.templates/architecture/layer-spec-template.md` | Bağımlı | katman şartnamesi |
| `.ai/.templates/architecture/dependency-rule-template.md` | Bağımlı | ihlal matrisi + komutlar |
| `.ai/.templates/architecture/ascii-diagram-template.md` | Bağımlı | çizim kuralı (66) |
| `.ai/architecture/index.md` | Hedef özet | bu belgeden türetilir (diskte yok → §1.2 E5) |
| `.ai/WORKFLOW.md` | Gate gerçeği | `GATE-004` (diskte yok → §1.2 E6) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Komut / Yöntem |
|---|---------|----------|----------------|
| K1 | Dosya envanterde #13 mü | `architecture-template` · `T-MIM` · Part B | `index.md` §3.3 |
| K2 | Hedef `.ai/architecture/<konu>.md` yolu | `C:\versacoder\` altı | §4.5 G14 |
| K3 | `ascii-diagram` D1 katman adları = §3.2 | birebir aynı | gözden geçirme |
| K4 | ADR numaraları diskte var mı | yoksa `⚠️` (§1.2 E11) | `Test-Path C:\versacoder\.ai\adr` |
| K5 | Ürün adı = VersaCoder | başka ad yok | `Select-String -Pattern 'CoreMusic'` = 0 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| # | Kaynak | Yol | Ne İçin | Durum |
|---|--------|-----|---------|-------|
| 1 | Şablon envanteri | `.ai/.templates/index.md` | satır #13, kalite sözleşmesi | ✅ Doğrulandı |
| 2 | Dizin kuralı | `.ai/.templates/CLAUDE.md` | 7 bölüm + ≥480 kuralı | ✅ Doğrulandı |
| 3 | Çizim şablonu | `.ai/.templates/architecture/ascii-diagram-template.md` | T1 kutu + 66 hizası | ✅ Doğrulandı |
| 4 | ADR mimari varyantı | `.ai/.templates/adr/adr-architecture-template.md` | bağımlılık/TFM matrisi biçimi | ✅ Doğrulandı |
| 5 | Altın `adr-template.md` | `.ai/.templates/adr/adr-template.md` | — | ❌ YOK (§1.2 E4) |
| 6 | Katman / gate / kural gerçeği | `.ai/architecture/index.md` · `.ai/WORKFLOW.md` · `.ai/.rules/*.md` | §3.2, `GATE-004`, §4.5 G2/G10 | ❌ YOK (§1.2 E5–E7) |
| 7 | Karar defteri | `.ai/adr/ADR-001…006` | §3.6 numaraları | ⚠️ VERIFICATION REQUIRED |
| 8 | Çıktı yolu | `C:\versacoder\.ai\architecture\` | üretilen belge | — |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Katman adlarını serbestçe uydurmak | Tüm vault K0..K8'e bağlanır, tek sapma her belgeyi çürütür | §3.2 tablosu sabittir; `ascii-diagram` D1 ile aynı ad |
| 2 | UI'ın doğrudan Infrastructure'e referans verdiğini yazmak | Katman fikri kalır, kod çürür | K6 → yalnız K7 Host (§3.4 satır 5) |
| 3 | `CrossCutting`'e herkesin referans verdiği anlatımı | Gizli ters bağımlılık doğar | K0 **yalnız dışa** bakar |
| 4 | EF Core geçmişi bir yerde bırakmak | ADR-004 ihlali, kopya kaynak | ADO.NET + Dapper + `@p1..@pN` |
| 5 | Tek TFM yazmak ("net10 yeter") | `net48` dalı ve DevExpress kanıtı kaybolur | §3.3 iki sütun da dolu (G8) |
| 6 | ADR numarasını kesin gibi yazmak | Diskte `.ai/adr/` yok → kanıtsız iddia | §1.2 E11 + `⚠️ VERIFICATION REQUIRED` |
| 7 | Dosyayı `C:\www\...` altına kaydetmek | Yanlış depo, geri alınamaz karmaşa | `C:\versacoder\.ai\architecture\` (G14) |
| 8 | Başka ürün/vault adını anmak | Marka ve vault tutarsızlığı | VERSA CODER / VersaCoder (G13) |
| 9 | Yer tutucularla teslim etmek | Belge okunamaz, üretim zinciri kırılır | §3.9 dışı yer tutucu = 0 (G4) |
| 10 | Kanıt taramasını atlayıp "mimari doğru" demek | §1.2 olmadan iddia spekülasyondur (G5) | §5.3 komutlarıyla ölç, sonucu §1.2'ye yaz |
