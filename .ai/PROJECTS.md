---
title: "PROJECTS — Proje Envanteri ve Bağımlılık Matrisi"
type: reference
category: vault-core
status: active
version: 2.0.0
updated: 2026-09-29
authority: SSOT
scope: versacoder/.ai/vault-core
---

# PROJECTS — Proje Envanteri

**Zorunlu Bağlantılar:** [index](index.md) · [AGENTS](AGENTS.md) · [CLAUDE](CLAUDE.md) · [WORKFLOW](WORKFLOW.md) · [keys](keys.md) · [brain](brain.md) · [MEMORY](MEMORY.md) · [log](log.md) · [architecture/index](architecture/index.md) · [.templates/index](.templates/index.md)

> **Tek cümlelik sorumluluk:** VersaCoder'ın 9 üretim + 1-2 test projesini, dış bağımlılıklarını, risk defterini ve kapsam dışı kararlarını tek yerde tutmak.

## §0 Özet (Answer First)

- **9 üretim projesi** = VersaCoder.{Abstractions,Domain,Crosscutting,Infrastructure,Protocol,Host,Desktop,Cli} + l2 (`VersaCoder.Application`, §1.1.3); + `VersaCoder.Tests` (zorunlu) ve `VersaCoder.ArchitectureTests` (opsiyonel, NetArchTest kanıtı için önerilir). Toplam **22 hedef** (dual TFM: `net48;net10.0-windows`).
- **Katman sırası (yukarıdan aşağı = izinli yön):** `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`, `l3 -> l0` (yalnız). Proje adları katman adlarıyla birebir örtüşmez — eşleme §1'de, katman bazlı derinlik §1.1'de.
- **Ana dış bağımlılık:** DevExpress WinForms v26.1; veri erişimi Dapper + Microsoft.Data.SqlClient + Microsoft.Data.Sqlite; mimari test NetArchTest.Rules + xunit. Sürüm gerekçeleri §2.2'de.
- **Risk defteri (10 madde):** K3 kapsam dışı, CPM test paketi eksikliği, 5440 = 0 marj, semantik atlas bayatlığı, l4 şişmesi, Desktop designer TFM, l2/l5 yön çelişkisi, proje adı çelişkisi, boş mimari klasörler, `Result<T>` konumu. §4.
- **Kapsam dışı:** üç tabloda — teknoloji (§5.1), ekran (§5.2), süreç (§5.3); her satırda kaynak.

**DOĞRULAMA GEREKLİ:** bu dosyadaki proje↔katman eşlemesi ADR-028 §2.1'den türetilmiştir; nihai sahiplik `architecture/index.md` yazıldığında doğrulanacak. §1'deki `CallSuite` / §1.1.3'teki `Application` ad farkı da aynı kapsamdadır.

## §1 Proje Envanteri (9 + 1-2)

Kaynak: ADR-028 §2.1 — doğrulanmış envanter, uydurma yok.

| # | Proje | Rol | TFM | Ana bağımlılık |
|---|-------|-----|-----|----------------|
| 1 | `VersaCoder.Abstractions` | En alt sözleşme katmanı (l0) — hiçbir şeye referansı yok | `net48;net10.0-windows` | sadece BCL |
| 2 | `VersaCoder.Domain` | L1 — Varlıklar, değer objeleri, domain kuralları | dual | Abstractions |
| 3 | `VersaCoder.CallSuite` | L2 — Çağrı/paket çözümleme çekirdeği | dual | Domain |
| 4 | `VersaCoder.Crosscutting` | L5 — Logging, Result<T>, zaman, ortak kesitler | dual | Abstractions+Domain |
| 5 | `VersaCoder.Infrastructure` | L4 — Dosya, SQLite, Dapper/SqlClient, adaptörler | dual | CallSuite+Crosscutting |
| 6 | `VersaCoder.Protocol` | L3 — VersaProtocol sözleşmesi (yalnız `l0`'a iner) | dual | Abstractions |
| 7 | `VersaCoder.Host` | L6 — Uygulama orkestrasyonu, yaşam döngüsü | dual | alttaki her şey |
| 8 | `VersaCoder.Desktop` | L7 (birincil) — WinForms ekranları (7 ekran) | dual | Host |
| 9 | `VersaCoder.Cli` | L7 (ikinci bacak) — headless CLI, gate'ler | dual | Host |
| 10 | `VersaCoder.Tests` | Zorunlu test projesi — TDD %90+ hedefi | dual | tümü |
| 11 | `VersaCoder.ArchitectureTests` | Opsiyonel — NetArchTest katman sınırı kanıtı | dual | tümü |

**Dual bacak (ADR-010):** Desktop ve Cli aynı `l0..l6` çekirdeğini paylaşır; sadece L7 farklıdır. 22 hedef = 11 proje × 2 TFM.

**Proje ↔ katman eşlemesi (türetilmiş, DOĞRULAMA GEREKLİ):**

```text
l7 arayüz  : VersaCoder.Desktop (WinForms) + VersaCoder.Cli
l6 host    : VersaCoder.Host
l5 kesit   : VersaCoder.Crosscutting
l4 infra   : VersaCoder.Infrastructure
l3 protokol: VersaCoder.Protocol      (-> l0 ONLY, atlamalı iniş)
l2 suite   : VersaCoder.CallSuite
l1 domain  : VersaCoder.Domain
l0 sözleşmeler: VersaCoder.Abstractions
```

**Ad uyarısı (DOĞRULAMA GEREKLİ):** yukarıdaki `l2 suite : VersaCoder.CallSuite` satırı ile [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1'in `VersaCoder.Application` satırı aynı katmanı iki farklı adla anıyor. Bu dosyada iki kayıt da korunmuştur (yeniden adlandırma yapılmamıştır); nihai ad `architecture/index.md` yazıldığında tek satırda sabitlenir.

## §1.1 Proje Bazlı Derinlik — 9 Üretim Projesi

Aşağıdaki dokuz alt bölüm ADR-028 §2.1 N aralıklarını ve [architecture/index](architecture/index.md) §2 sorumluluk sütununu temel alır. **Alt katman görev tanımları N adlarından türetilmiştir** — kesin şartname `architecture/catalog/` dosyaları üretilinceye kadar `DOĞRULAMA GEREKLİ` kalır; o klasörlerde bugün 0 dosya vardır (bkz. §4 R9).

Her proje için dört soru yanıtlanır: ne yapar · hangi N kodlarını taşır · dış bağımlılığı ve test ilişkisi · bu proje olmasa ne kırılır.

**Katman numarası uyarısı (DOĞRULAMA GEREKLİ):** aşağıdaki diyagram ADR-028 §2.1 numaralandırmasını izler — `l3 = Crosscutting (yalnız l0)` ve `l5 = Protocol`. §1'deki eşleme bloğu ile §6 matrisi aynı iki katmanı ters numarayla (`l5 kesit`, `l3 protokol`) yazar; bu fark §4 **R7** kapsamdadır ve `architecture/index.md` yazılana kadar iki kayıt da korunur.

```text
+-- l7 giris yuzeyi ----------------------------------------------------+
|  VersaCoder.Desktop (WinForms, DevExpress v26.1) + VersaCoder.Cli     |
+------------------------------+---------------------------------------+
                               | Host'a iner, tersi degil
+-- l6 Host -------------------------------------------------------------+
|  VersaCoder.Host : bootstrap-pipeline / di-registration / config-br.  |
+------------------------------+---------------------------------------+
                               |
+-- l5 Protocol ---------------------------------------------------------+
|  tool-schema / messages / session-transcript / loop / versioning      |
+------------------------------+---------------------------------------+
                               |
+-- l4 Infrastructure ---------------------------------------------------+
|  ado-dapper / sqlite / sqlserver / migrations / fs / git / llm / tool |
+------------------------------+---------------------------------------+
                               | port -> l0, uygulama -> l4
+-- l3 Crosscutting      |     +-- l2 Application -----------------------+
|  (YALNIZ l0'a bakar)   |     |  commands / queries / handlers / dto    |
|  logging / caching /   |     |  validators / use-cases / scheduling    |
|  clock / crypto / opts |     |  [ad: Application | CallSuite -> R8]    |
|  resilience            |     +----------------+----------------------+
+-----------+------------+                      |
            | (yalniz l0)                       |
            +-------------------+---------------+
                                |
+-- l1 Domain -----------------------------------------------------------+
|  entities / value-objects / aggregates / events / invariants / enums  |
+------------------------------+---------------------------------------+
                               |
+-- l0 Abstractions (hicbir seye referans VERMEZ) -----------------------+
|  ports-core / result-error / events-abi / extensions / constants      |
+------------------------------------------------------------------------+
```


### §1.1.1 `VersaCoder.Abstractions` — l0 (N01–N05)

**Ne yapar:** VersaCoder'ın en dibindeki sözleşme katmanıdır; hiçbir projeye referans vermez, yalnız BCL'e bakar. Bütün katmanlar buradaki arayüzleri konuşur, böylece yön kuralı (`l7 -> ... -> l0`) tek yerden kurulur.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N01 | ports-core | Dış dünya (dosya, ağ, LLM, araç çalıştırma) için ince port arayüzleri |
| N02 | result-error | `Result<T>` ve hata kodu sözleşmesi (konum tartışması §4 R10) |
| N03 | events-abi | Olay yayımı/tüketimi sözleşmesi (abi seviyesi) |
| N04 | extensions | BCL üzerine ince uzatma metotları |
| N05 | constants | Sabitler, ortak imza ve tanım setleri |

**Dış bağımlılık:** paket yok, yalnız BCL — bu yüzden her iki bacakta da sorunsuz derlenir ve NuGet sürüm sapması riski taşımaz.
**Test ilişkisi:** `VersaCoder.Tests` burada sözleşme davranışını (özellikle `Result<T>` geçişleri) sınar; `VersaCoder.ArchitectureTests` ise "l0 hiçbir şeye referans vermiyor" kuralını NetArchTest ile kanıtlar.
**Olmasa ne kırılır:** portlar burada durmazsa `l4` doğrudan `l2`'ye bağlanır, yön matrisi (§6) ve NetArchTest ilk günRED olur.

### §1.1.2 `VersaCoder.Domain` — l1 (N06–N11)

**Ne yapar:** İş kurallarını framework'ten, ekrandan ve veritabanından ayrı tutar. Varlıklar, değer objeleri, topluluk (aggregate) sınırları, alan olayları ve değişmezler (invariants) burada yaşar; burada `DbConnection`, `HttpClient` veya Form referansı bulunmaz.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N06 | entities | Kimlik taşıyan varlık tanımları |
| N07 | value-objects | Kimliksiz, eşitliği değere göre çalışan tipler |
| N08 | aggregates | Tutarlılık sınırı içindeki topluluk nesneleri |
| N09 | domain-events | Alanda oluşan olayların kaydı |
| N10 | invariants | Bozulamaz iş kuralları |
| N11 | enums | Alan sabiti niteliğindeki enum'lar |

**Dış bağımlılık:** yalnız `VersaCoder.Abstractions` — paket bağımlılığı yok; bu, domain'in iki bacakta da aynı davranışta kalmasını sağlar (ADR-001).
**Test ilişkisi:** `VersaCoder.Tests` içindeki domain testleri en yüksek kapsama hedefini taşır; değişmezler (N10) kırıldığında test kırmızıya döner, kod değil kural değişmiş sayılır.
**Olmasa ne kırılır:** iş kuralı `l4` veya `l7` içine dağılır; aynı kural iki yerde iki farklı yorumlanır, `l2` use-case'leri kural tekrarı üretir.

### §1.1.3 `VersaCoder.Application` — l2 (N12–N18)

**Ne yapar:** Kullanım senaryolarını (command, query, use-case) UI ve veri erişiminden ayırır. `[AD UYARISI]` Bu katman [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1'de `VersaCoder.Application` adıyla geçer; §1 envanter tablosu aynı katmanı `VersaCoder.CallSuite` adıyla listeler — iki ad da korunmuştur, nihai karar `architecture/index.md`'de verilecektir.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N12 | commands | Durum değiştiren istek tipleri |
| N13 | queries | Salt okunur sorgu istekleri |
| N14 | handlers | Command/query işleyicileri |
| N15 | dto-mappers | DTO <-> domain eşleme |
| N16 | validators | Girdi doğrulama kuralları |
| N17 | use-cases | Uçtan uca kullanım senaryoları |
| N18 | scheduling | Zamanlanmış/ertelenen işler |

**Dış bağımlılık:** `VersaCoder.Domain` + `VersaCoder.Abstractions`; veri erişimi **doğrudan** değil, yalnız `l0` portları üzerinden çağrılır (`l2 -> l4` oku yoktur).
**Test ilişkisi:** `VersaCoder.Tests` use-case'leri; `VersaCoder.ArchitectureTests` "l2, l4'e bakmaz" kuralını denetler (§6).
**Olmasa ne kırılır:** ekran kodu iş kuralını kendisi yazar; aynı use-case Desktop ve Cli'de iki kez implemente edilir, ikisi birbirinden sapar.

### §1.1.4 `VersaCoder.Crosscutting` — l3 (N19–N24)

**Ne yapar:** Günlük, önbellek, saat/kripto, yapılandırma ve dayanıklılık gibi kesit işlevlerini tek yerde toplar. Kritik sınır: bu katman **yalnız `l0`'a** bakar; altındaki hiçbir katmanı (`l1..l7`) tanımaz, aksi halde kesit işlevi bütün katmanları sarar ve yön kuralı çöker.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N19 | logging | Structured log şablonu (ADR-026, 5 seviye) |
| N20 | caching | Önbellek anahtarı ve TTL politikaları |
| N21 | clock-time | Saat kaynağı soyutlaması |
| N22 | crypto-sec | Kripto/şifreleme yardımcıları |
| N23 | config-opts | Yapılandırma seçenek tipleri |
| N24 | resilience | Yeniden deneme/koruma desenleri |

**Dış bağımlılık:** `VersaCoder.Abstractions` (+ §1 tablosunda `Domain` de listelenmektedir — ADR-028 §2.1 ile uyumu §4 R7 kapsamında doğrulanacak).
**Test ilişkisi:** log şablonunun alan adlarının iki bacakta aynı olduğu `VersaCoder.Tests` ile, `l3 -> l0` dışına çıkılmadığı `VersaCoder.ArchitectureTests` ile sınanır.
**Olmasa ne kırılır:** log ve saat kodun içine gömülür; iki bacakta farklı alan adları üretilir, günlük okunamaz hale gelir (ADR-026 ihlali).

### §1.1.5 `VersaCoder.Infrastructure` — l4 (N25–N32)

**Ne yapar:** `l0` portlarının somut uygulamasıdır: ADO.NET + Dapper sorguları, SQLite/SQL Server sağlayıcıları, migration, dosya sistemi araçları, git adaptörü, LLM HTTP istemcisi ve araç çalıştırıcı (tool-runner). Projeler içindeki en geniş N aralığını (8 alt katman) taşır; bu yüzden §4 R5 ile birlikte en kırılgan katmandır.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N25 | ado-dapper | Bağlantı ve komut yürütme (tek `DbConnection` sahibi) |
| N26 | sqlite-provider | Yerel SQLite erişimi |
| N27 | sqlserver-provider | SQL Server erişimi (Microsoft.Data.SqlClient) |
| N28 | migrations | Saf SQL migration + version tablosu |
| N29 | fs-tools | Dosya sistemi işlemleri |
| N30 | git-adapter | Git komutu sarmalayıcıları |
| N31 | llm-http | LLM sağlayıcı HTTP istemcisi (ADR-011 sırası) |
| N32 | tool-runner | Araç çağrısı yürütme (ADR-006/009 sınırları) |

**Dış bağımlılık:** Dapper, Microsoft.Data.SqlClient, Microsoft.Data.Sqlite (§2.2 gerekçeleri); **EF Core yasaktır** (ADR-005). Test için `NSubstitute` ile taklit (mock) kullanılır.
**Test ilişkisi:** `VersaCoder.Tests` sorgu/migration testlerini iki bacakta çalıştırır; `VersaCoder.ArchitectureTests` yalnız `l4 -> l0/l1/l2` yönünü değil, `DbConnection`'ın başka katmana sızmamasını da denetler (ADR-016).
**Olmasa ne kırılır:** portların uygulaması olmazsa ekran ile veritabanı doğrudan konuşur; parametreli SQL denetimi (ADR-017) ve migration denetimi (ADR-005) denetlenecek hiçbir nokta bırakmaz.

### §1.1.6 `VersaCoder.Protocol` — l5 (N33–N37)

**Ne yapar:** opencode araç kümesi (`read/write/edit/bash/grep/glob/list`) ile C# tarafı arasındaki sözleşmeyi, mesaj biçimini, oturum dökümünü ve döngü (loop) politikasını taşır. Öne çıkan mimari özelliği: **atlamalı iniş** — `l5` yalnız `l0`'a referans verir, `l1..l4`'ü tanımaz (§3 düzeltme 5).

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N33 | tool-schema | Araç girdi/çıktı şeması |
| N34 | messages | Mesaj tipleri ve sıralama |
| N35 | session-transcript | Oturum dökümü biçimi |
| N36 | loop-policy | Ajan döngüsü durdurma/karar kuralları |
| N37 | versioning-errors | Sürümleme ve protokol hata kodları |

**Dış bağımlılık:** yalnız `VersaCoder.Abstractions`; paket bağımlılığı yoktur (sürümleme protokolün kendi kodunda yaşar).
**Test ilişkisi:** `VersaCoder.Tests` şema doğrulama ve sürüm uyumluluk testlerini taşır; `VersaCoder.ArchitectureTests` "Protocol yalnız `l0`'a bakar" kuralını ayrı bir kural olarak denetler.
**Olmasa ne kırılır:** araç sözleşmesi her iki tarafta ayrı ayrı yazılır; sürüm değişince `Host` ile `Desktop` aynı mesajı iki farklı anlama getirir.

### §1.1.7 `VersaCoder.Host` — l6 (N38–N40)

**Ne yapar:** Uygulamayı birleştirir: açılış boru hattı (bootstrap), bağımlılık enjeksiyonu kaydı ve yapı köprüleri. `l0..l5` arasındaki tüm somut sınıfları yalnız burada bilir; alt katmanların birbirini tanıması gerekmez.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N38 | bootstrap-pipeline | Açılış sırası ve önkoşullar |
| N39 | di-registration | Servis kayıtları |
| N40 | config-bridges | Yapılandırma -> katman köprüsü |

**Dış bağımlılık:** projeler bazında değil, **sınıf bazında** — bu yüzden §4 R5'teki gibi `Host`'a yığma riski yüksektir (ADR-028 §3'te junior #2 itirazı olarak kayıtlıdır).
**Test ilişkisi:** `VersaCoder.Tests` açılış ve kayıt testlerini (composition root) kapsar; mimari test `Host`'un `l7`'ye referans vermediğini doğrular.
**Olmasa ne kırılır:** iki giriş yüzeyi (Desktop ve Cli) aynı kayıtları iki kez kurar; yapı ikiye ayrılır ve davranış sapması üretilir.

### §1.1.8 `VersaCoder.Desktop` — l7 birincil (N41–N43, N44 ortak)

**Ne yapar:** DevExpress WinForms v26.1 ile 7 ekranı (01-start-page .. 07-settings) kurar; ribbon kabuğu, panel/explorer ve editör-AI sohbet yüzeyi buradadır. `.Form.cs + .Form.Designer.cs + .resx` üçlüsü zorunludur ve tasarımcı sahipliği `ui-designer`'dadır (ADR-015/ADR-025).

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N41 | shell-ribbon | Ribbon kabuğu ve pencere iskeleti |
| N42 | panels-explorer | Çözüm/explorer panelleri |
| N43 | editor-ai-chat | Kod editörü ve AI sohbet paneli |
| N44 | settings-cli | Ayarlar ekranı (Cli ile ortak, bkz. §1.1.9) |

**Dış bağımlılık:** DevExpress Universal **v26.1** tek sürüm + `RibbonControlStyle.Office2010` tek stili (ADR-004). Sürüm düşürme veya başka stil = otomatik RED (§2.2).
**Test ilişkisi:** ekran testleri `VersaCoder.Tests` altında toplanır; `.Designer.cs` değişikliklerinin denetimi `winforms-designer-check` listesiyle yapılır (adı metin olarak geçer, link bu dosyada tanımlı değildir → DOĞRULAMA GEREKLİ).
**Olmasa ne kırılır:** ürünün birincil yüzeyi kalkar; Cli tek başına kullanıcıya açılış/ayar/editör akışlarını gösteremez.

### §1.1.9 `VersaCoder.Cli` — l7 ikinci bacak (N44 ortak)

**Ne yapar:** Aynı `l0..l6` çekirdeğini paylaşan headless giriş yüzeyidir; gate'lerin ve otomasyonun bağlandığı komut satırı arayüzüdür (ADR-010). Ayrışma yalnız `l7` seviyesindedir — çekirdek kod **iki bacak arasında birebir aynıdır**.

| N | Alt katman | Burada ne yaşar |
|---|---|---|
| N44 | settings-cli | Ayar okuma/yazma akışı (Desktop ekranıyla ortak) |

**Dış bağımlılık:** ek paket yoktur; Desktop'taki DevExpress'e de, ekran koduna da referans vermez.
**Test ilişkisi:** Cli komutları `VersaCoder.Tests` içinde uçtan uca (headless) sınanır; "aynı işi Desktop ve Cli aynı sonuçla yapar" testi, ADR-010'un paylaşılan çekirdek iddiasının tek kanıdır.
**Olmasa ne kırılır:** CI/otomasyon ve gate betikleri için insansız giriş kalmaz; paylaşılan çekirdek iddiası test edilemez hale gelir.

### §1.1.10 Test Projeleri (1 zorunlu + 1 opsiyonel)

| Proje | Zorunlu mu | Ne sınar | Neden opsiyonel/zorunlu |
|---|---|---|---|
| `VersaCoder.Tests` | **zorunlu** | Birim + use-case + protokol testleri, iki bacakta `dotnet test` | TDD %90+ hedefi ve ADR-019 kanıtının tek üreticisi |
| `VersaCoder.ArchitectureTests` | opsiyonel ama önerilir | NetArchTest `LayerRulesTests` — 8×8 yön matrisi | Yoksa yön kuralı (§6) yalnız gözden geçirmeyle denetlenir |

**Test projesi ortak kuralı:** iki test projesi de çift TFM'de derlenmelidir; tek bacakta yeşil test "bitti" kanıtı değildir (ADR-019) ve test projelerinin TFM hizası düzeltme 6'nın parçasıdır (§3).

**Olmasa ne kırılır:** kapılar (GATE-004a / GATE-004) kanıtsız açılır; "testler geçti" iddiası hiçbir komutla desteklenemez.

## §2 Dış Bağımlılıklar

| Grup | Paket | Sürüm | Nerede | Karar |
|------|-------|-------|--------|-------|
| UI | DevExpress WinForms | **v26.1** | Desktop (l7) | ADR-018 — v26.1 sabit, sürüm düşürme yasak |
| Veri | Dapper | — | Infrastructure (l4) | ADR-019 — micro-ORM tercih |
| Veri | Microsoft.Data.SqlClient | — | Infrastructure (l4) | SQL Server erişimi |
| Veri | Microsoft.Data.Sqlite | — | Infrastructure (l4) | Yerel SQLite (arama/günlük) |
| Test | xunit | — | Tests + ArchitectureTests | varsayılan test framework'ü |
| Test | NetArchTest.Rules | — | ArchitectureTests | katman sınırı kanıtı (ADR-028 §9) |
| Test | Microsoft.NET.Test.Sdk, NSubstitute | — | Tests | test altyapısı + mock |

**CPM durumu (risk R2):** kök `Directory.Packages.props` merkezî sürüm yönetimi için var ama test paketleri (NetArchTest, xunit, Microsoft.NET.Test.Sdk, NSubstitute) henüz içinde değil. Derleme öncesi 6 düzeltmeden biri (ADR-028 §C2).

**Yok sayılanlar:** EF Core (bakınız §5), NuGet paketi olarak yayımlama (paketleme yok), harici mesaj kuyruğu/broker.

### §2.1 İki Bacak Matrisi (ADR-019, ADR-012)

| Alan | net48 | net10.0-windows | Not |
|------|-------|------------------|-----|
| DevExpress WinForms v26.1 | OK (.NET Framework 4.6.2+) | OK (.NET 8/9/10) | ADR-004 web ile doğrulandı |
| Dapper | OK | OK | ORM değil, micro-ORM |
| Microsoft.Data.SqlClient 5.x | OK | OK | iki bacakta **aynı major** (ADR-012) |
| Microsoft.Data.Sqlite | OK | OK | yerel/CI doğrulaması tek bacakta (net10.0-windows) |
| NetArchTest + xunit | OK | OK | CPM'e eklenecek (R2 / §C3) |
| EF Core | — | — | yasak, iki bacakta da yok (ADR-005/016) |

Tek bacakta yeşil = bitmemiştir (ADR-019); istisnalar yalnız ADR-019 matrisinde yazılıdır.

### §2.2 Dış Paket Gerekçeleri — neden bu sürüm, ne işe yarar

§2 tablosu "hangi paket nerede" sorusunu, bu tablo "neden bu sürüm ve ne yapar" sorusunu cevaplar. Sürüm sütunu boş olan satırlarda kesin sürüm `Directory.Packages.props` kapandığında yazılacaktır (düzeltme 1, §3).

| # | Paket | Sürüm | Ne yapar | Neden bu tercih | Kaynak |
|---|-------|-------|----------|-----------------|--------|
| 1 | DevExpress Universal (WinForms) | **26.1** | Ribbon, Form ve tüm UI kontrolleri | Tek sürüm + tek ribbon stili kilitli; farklı sürüm tasarım dosyalarını bozar | ADR-004 |
| 2 | Dapper | `Directory.Packages.props`'a yazılacak | Parametreli sorguyu `IDbCommand` üzerinde çalıştırır | ORM değil micro-ORM: SQL görünür kalır, EF Core ağırlığı yok | ADR-005 |
| 3 | Microsoft.Data.SqlClient | 5.x, iki bacakta aynı major | SQL Server bağlantısı ve komut çalıştırma | Ayrık provider iki bacakta farklı sürüm açığı üretir | ADR-012 |
| 4 | Microsoft.Data.Sqlite | `Directory.Packages.props`'a yazılacak | Yerel SQLite (arama indeksi, günlük) | Yerel/CI doğrulamasını tek bacakta (net10.0-windows) tutar | ADR-012 |
| 5 | NetArchTest.Rules | **CPM'de YOK** | 8×8 yön matrisini kod üzerinde denetler | Elle gözden geçirme geçişli hata üretir; otomatik RED gerekir | ADR-028 §2.3/1 |
| 6 | xunit | **CPM'de YOK** | Test çalıştırıcısı | TDD %90+ hedefinin ölçülebilir altyapısı | ADR-028 §2.3/1 |
| 7 | Microsoft.NET.Test.Sdk | **CPM'de YOK** | Test platformu (VSTest/MTP) köprüsü | `dotnet test` komutu bu paket olmadan çözülemez | ADR-028 §2.3/1 |
| 8 | NSubstitute | **CPM'de YOK** | Arayüz takidi (mock) | Bağımlılığı elle sahtelemek testi kırılganlaştırır | ADR-028 §2.3/1 |

**Dikkat (ad/at farkı):** §2 tablosundaki Dapper satırı ADR-019'u kaynak gösterirken bu tablo ADR-005'i göstermektedir; iki atfın hangisinin geçerli olduğu `DOĞRULAMA GEREKLİ`'dir (karar metni ADR-005'te, iki bacak uyumu ADR-019/ADR-012'dedir).

**Paket politikası tek satır:** sürüm yalnız `Directory.Packages.props` içinde yaşar, `.csproj`'da `Version` satırı yoktur (ADR-014, [ADR-014](.decisions/accepted/ADR-014-slnx-and-cpm.md)).

## §3 Derleme Öncesi Zorunlu Düzeltmeler (ADR-028 §C)

Bu 6 madde derlemeden önce kapatılmalı; aksi halde 22 hedefli çözüm başlamaz/yanlış kurulur:

| # | Dosya | Düzeltme | Kanıt |
|---|-------|----------|-------|
| 1 | K3 kayıtları | Katalog **kapsamı dışı** — hatalı DLL referansları çıkarılacak | kapsam dışı satırı (§5.2) |
| 2 | `architecture/00-overview/dependency-map.md` + ADR-013 ek | Bağımlılık yönü tutarsızlığı: matriste `l4→l2 X` ve `l5→l4 X` işaretli, §2.1'de izinli — **tek yöne** sabitlenecek (checklist `l5 → l4`, yön: yukarıdan aşağı) | matris = NetArchTest `LayerRulesTests` |
| 3 | `Directory.Packages.props` | CPM içine test paketleri eklenecek (NetArchTest, xunit, Microsoft.NET.Test.Sdk, NSubstitute) | `dotnet restore` = 0 NU1008 |
| 4 | tüm `.csproj` | Dual TFM hizası: her projede `net48` + `net10.0-windows` eşit tanılı olacak (22 hedef sayımı buna dayanır) | 22 hedef listesi |
| 5 | sıralı referans kuralı | Protokol projesinin `l3 → l0` atlamalı inişi — sıralı referanslarda Protocol'ü zorlamayacak şekilde düzenlenecek | NetArchTest `ProtocolOnlyAbstractions` benzeri kural |
| 6 | `.ai/architecture/l0..l7` | Boş klasörler ilk dokümanla doldurulacak; mimari atlas üretilmeden kod derlenmeyecek (ADR-027 kapısı) | `count-components.ps1` PASS |

**Kapanış sırası:** altı düzeltmenin tamamı **GATE-004a kapanışından önce** bitmelidir; [WORKFLOW](WORKFLOW.md) bu sırayı kapı adımı olarak taşır.

## §4 Risk Defteri (10 madde)

Sütunlar: **Olasılık** (ne sıklıkla tetiklenir), **Etki** (tetiklenirse ne kaybolur), **Tetikleyici** (hangi eylem), **Erken belirti** (ne görünür), **Müdahale** (ilk hamle). Kanıt sütunu riskin "Risk" hücresine gömülüdür.

| ID | Risk (kanıt) | Olasılık | Etki | Tetikleyici | Erken belirti | Müdahale |
|----|--------------|----------|------|-------------|---------------|----------|
| **R1** | **K3 kapsam dışı** — 618 kayıt / 616 benzersiz; 2 çakışma (642/662, 643/663), 4 DLL eksik; 99 kayıt (~%15) üretim ağacında karşılığı yok | yüksek (kayıt zaten var) | yanlış DLL referansı ile bozuk çözüm | birinin K3 listesinden referans taşıması | katalog satırında `DOĞRULAMA GEREKLİ` olmayan K3 kaynağı | K3'ü kapsam dışı ilan et, catalog'a taşıma yapma, eksikleri §C1 ile temizle |
| **R2** | **CPM test paketi yok** — `Directory.Packages.props` içinde xunit/NetArchTest/Test.Sdk/NSubstitute yok | kesin (bugün eksik) | test projesi derlenemez, kapılar devre dışı | ilk `dotnet restore` | **NU1008** hatası | §C3 ile CPM'e ekle, 0 NU1008 kanıtı al |
| **R3** | **5440 = 0 marj** — `count-components.ps1` → 5440 bileşen (min 5000); 440 payı tükendi | orta | eşik altı = gate RED | tek satır silme / yeniden adlandırma | sayı < 5000 | yeni LLM bileşeni = mimari gözden geçirme şart; l4'e yığma |
| **R4** | **Semantik atlas bayat riski** — semantik node = S01–S07, bileşen bazlı node yok | orta | yanlış katmana dosya taşıma | kod değişir, atlas güncellenmez | "katalog = kod" denetimi tutmaz | [checklists/layer-boundary-check](checklists/layer-boundary-check.md) 13 sorusu her taşıma öncesi |
| **R5** | **l4 şişme riski** — Infrastructure = 231 bileşen (68/25/73/65 fa = l4 içinde en büyük); R3 ile birleşince kırılgan | yüksek | l4 her şeyi yutar, sınır kalmaz | "hangi katman?" sorusu sorulmadan yeni kod | l4 bileşen sayısında sürekli artış | yeni kod önce "hangi katman?" sorusu; l4'e sadece adaptör/fiziksel erişim, port önce `l0` |
| **R6** | **Desktop designer TFM** — ADR-028 §2.3 düzeltme 6: designer bacağı belirlenmedi | orta | net48 kolu NetArchTest'te kalır; NU1201 riski | `.Form.Designer.cs` ile test projesinin TFM'si farklı olmak | tek bacakta açılan tasarımcı, öbür bacakta derleme hatası | designer bacağını belirle + test projeleri çift TFM yap (§C4/§C6) |
| **R7** | **Yön / numara çelişkisi** — ADR-028 §2.3 düzeltme 2: `l2–l4` ve `l5–l4` işaretleri iki belgede farklı; `architecture/index` §4.1 hücre 4-5 `?` işaretli; ayrıca §1/§6 `l3 Protocol / l5 Crosscutting` derken §1.1 ve ADR-028 tersini yazar | yüksek (çelişki zaten kayıtlı) | NetArchTest yanlış kuralı doğrular | 8×8 matris yazılmadan test yazmak | iki belgede aynı okun farklı işareti, iki katmanın farklı numarası | 8×8 matrisi tek kaynağa sabitle, sonra `LayerRulesTests`'i ona göre yaz |
| **R8** | **Proje adı çelişkisi** — §1 `VersaCoder.CallSuite`, ADR-028 §2.1 `VersaCoder.Application` | kesin (metinde mevcut) | iki `.csproj` adı altında aynı katman | envanter tablosu tek başına okunarak iş başlaması | bir dosyada `CallSuite`, ötekinde `Application` | `architecture/index.md` yazılınca tek ad; bu dosyada iki kayıt da korunur → `DOĞRULAMA GEREKLİ` |
| **R9** | **Mimari klasörler boş** — `architecture/` altında 10 klasör, **0 dosya** (2026-09-29 ölçümü) | kesin | ADR-027 kapısı açılmaz, sayım geçersiz | GATE-004a kapanışına boş klasörle girmek | `count-components.ps1` kayıt bulamaz | §C6: ilk dokümanla doldur, sonra ölç |
| **R10** | **`Result<T>` konum çelişkisi** — N02 `result-error` l0'da, kod kuralı ise `Crosscutting` (l5) diyor (`brain` §3.1/§4.2) | orta | ortak tip iki katmanda iki kez tanımlanır | ilk `Result<T>` yazımında konum kararı verilmezse | `using` satırının hangi projeye ait olduğunun belirsizliği | kod yazılmadan önce konumu sabitle: port `l0`, yardımcı mı l5 → `DOĞRULAMA GEREKLİ` |

**Okuma kuralı:** Risk satırı `DOĞRULAMA GEREKLİ` taşıyorsa o satır **kanıt değildir**, açık konudur; kapatılmadan "tamamlandı" denemez. Risk kayıtlarının vault geneli sürümü [reports/08-risk-kayit-defteri](reports/08-risk-kayit-defteri.md) dosyasındadır.

### §4.1 Hata Modları — kural bozulursa ne görünür, ne zaman yakalanır

§4 "ne olabilir" der, bu tablo "ne görünür" der. Sütunlar: **Belirti** (ekranda/komut çıktısında görünen), **Yakalama noktası** (hangi komut/gate), **Sonuç** (geçmezse ne olur).

| # | Hata modu | Belirti | Yakalama noktası | Sonuç |
|---|-----------|---------|------------------|-------|
| 1 | Test paketleri CPM'de yok | `NU1008` restore hatası | `dotnet restore` | Çözüm hiç kurulamaz → R2 |
| 2 | Tek bacakta yeşil test | `net10.0-windows` geçti, `net48` atlandı | §7 komut 1-4 (iki kol) | "bitti" denemez → ADR-019 |
| 3 | Ters referans (`l4 -> l2`) | NetArchTest `LayerRulesTests` RED | `VersaCoder.ArchitectureTests` | Kod geri alınır, yön matrisi güncellenir |
| 4 | K3'ten DLL referansı taşındı | katalogda `DOĞRULAMA GEREKLİ` olmayan K3 kaynağı | [checklists/layer-boundary-check](checklists/layer-boundary-check.md) | Katalog kapsam dışı ihlali → R1 |
| 5 | Bileşen sayısı eşiğin altına düştü | `count-components` < 5000 | §7 komut 2 | Gate RED → R3 |
| 6 | Bozuk link | `check-links.ps1` 0 dışı | §7 komut 3 | GATE-004a kapalı |
| 7 | Designer tek bacakta açılıyor | `.Form.Designer.cs` derleme hatası / NU1201 | dual-TFM build | Ekran kodu durur → R6 |
| 8 | Proje adı iki farklı biçimde | `CallSuite` ve `Application` bir arada | `grep` taraması | İki `.csproj` riski → R8 |

**Genel davranış:** bu sekiz moddan hangisi çıkarsa çıksın, kanıt **komut çıktısı** ile üretilir; "kontrol ettim" cümlesi kanıt sayılmaz (G5).

## §5 Kapsam Dışı (Bilinçli Karar) — üç tablo

### §5.1 Teknoloji Dışı

| Kapsam dışı | Neden | Karar kaynağı |
|-------------|-------|---------------|
| **EF Core** | Dapper + Sqlite/SqlClient yeterli; ORM ağırlığı ve sürüm kilitlenmesi istenmiyor | [ADR-005](.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) |
| **WPF / MAUI / web** | WinForms (DevExpress v26.1) tek hedef; masaüstü + CLI yeterli | ADR-018, ADR-010 |
| **Paketleme (NuGet)** | Ürün içi kullanım; yayımlama paketleme yok | ADR-019 akış |
| **Yeni LLM sağlayıcısı** | Sağlayıcı sırası sabit: `openrouter-free -> ollama -> openai-compatible` | ADR-011 |
| **Harici mesaj kuyruğu / broker** | Ürün tek süreçte çalışıyor; kuyruk katmanı ek maliyet | `DOĞRULAMA GEREKLİ` (karar metni ADR'lerde taramadı) |

### §5.2 Ekran Dışı (7 ekran sınırının dışında kalan varlıklar)

| Kapsam dışı | Neden | Karar kaynağı |
|-------------|-------|---------------|
| **K3 618 DLL envanteri** | Kataloga, N-ID'lere ve `count-components.ps1` sayımına girmez | ADR-028 §2.2 |
| **K3'teki mevcut kod (99 kayıtsız dosya)** | Katalog kapsamı dışı; üretim ağacına taşınmaz | ADR-028 §C1 |
| **`l0..l6` katmanları** | Ekran yalnız `l7`'dedir; alt katmanlarda Form/tasarım dosyası bulunmaz | ADR-028 §2.1 |
| **Vault kök `.md` dosyaları (14 zorunlu belge)** | Bunlar ürün ekranı değil, vault belgesidir | ADR-003 |
| **`architecture/l0..l7/` ve `catalog/` dosyaları** | Bugün 0 dosya; ekranla ilişkilendirilmeden üretilir | [architecture/index](architecture/index.md) §8 |

### §5.3 Süreç Dışı (yapılmayacak/görev dışı işler)

| Kapsam dışı | Neden | Karar kaynağı |
|-------------|-------|---------------|
| **CI/CD pipeline** | Henüz yok; kapılar önce local (checklist + script'ler), CI sonra | depo `.git` var, workflow yok → `DOĞRULAMA GEREKLİ` (eklenme planı) |
| **Elle bileşen sayımı** | Tek kanıt `count-components.ps1`; elle sayı rapora yazılmaz | ADR-027 |
| **ADR'leri script ile otomatik kaydetme** | İlişki/gerekçe notu otomasyona kurban olur; yalnız doğrulama otomatiktir | [.decisions/adr-index](.decisions/adr-index.md) §3.2-C |
| **3 paralel akıştan sıradışı geliştirme** | ADR-028 §4 akışı: (l0->l1->l2 seri) -> (l3 \|\| l4 \|\| l5) -> (l6 -> l7) | ADR-028 §4 |
| **Başka vault'a / `%TEMP%` referans ağacına yazma** | Salt okunur; yalnız yapı referans alınır | ADR-023, ADR-024 |

## §6 Proje → Referans Matrisi (izinli yön)

| Proje | Yukarıdaki (üst) katmana referans verir | Veremez |
|-------|------------------------------------------|---------|
| Abstractions (l0) | — (hiçbir şeye) | her şeye |
| Domain (l1) | Abstractions | CallSuite, Infrastructure, Host… |
| Protocol (l3) | **yalnız Abstractions (l0)** | Domain, CallSuite, Infrastructure |
| CallSuite (l2) | Domain, Abstractions | Infrastructure, Host, UI |
| Infrastructure (l4) | CallSuite, Crosscutting, Domain, Abstractions | Host, Desktop, Cli |
| Crosscutting (l5) | Abstractions, Domain | Infrastructure, Host, UI |
| Host (l6) | l0..l5 | Desktop/Cli (UI'dan Host'a iner, tersi değil) |
| Desktop (l7) | Host + altı | doğrudan l4/l5'e atlamak (bazı istisnalar DOĞRULAMA GEREKLİ) |
| Cli (l7) | Host + altı | aynı |
| Tests / ArchitectureTests | tümü (test eder) | üretime referans (production asla test'e bakmaz) |

Yön çizgisi (tek doğru): `l7 → l6 → l5 → l4 → l2 → l1 → l0`, ayrıca `l3 → l0`. ADR-028 §C6'daki matris tutarsızlığı (`l4→l2 X`, `l5→l4 X`) düzeltme 2 ile kapatılacak.

**Not (ad farkı):** bu tablo §1'deki envanter adlarını (`CallSuite`) kullanır; ADR-028 §2.1'in `Application` adıyla aynı katmanı işaret eder — bkz. §4 R8.

**Not (numara farkı):** bu tablo `Protocol (l3)` ve `Crosscutting (l5)` derken §1.1 diyagramının ve ADR-028 §2.1'in ters numarasını kullanır; iki kayıt da bilinçli olarak korunmuştur — bkz. §4 R7.

## §7 Doğrulama Komutları (yalnız-oku)

```powershell
# 1) İki bacakta derleme (ADR-019: tek bacak yeşil = bitmemiş)
dotnet build VersaCoder.slnx -f net48
dotnet build VersaCoder.slnx -f net10.0-windows

# 2) Bileşen kanıtı (ADR-027): eşik -MinRecords 5000, -MinNodeFiles 52
powershell -NoProfile -ExecutionPolicy Bypass -File .ai/scripts/count-components.ps1 -MinRecords 5000 -MinNodeFiles 52

# 3) Bozuk link = 0 (ADR-003 / GATE-004a)
powershell -NoProfile -ExecutionPolicy Bypass -File .ai/scripts/check-links.ps1

# 4) Test (her iki bacak)
dotnet test --framework net48
dotnet test --framework net10.0-windows
```

Beklenen kanıt: 52 düğüm dosyası, ≥5000 kayıt (şu an 5440, marj 440 = R3), 0 bozuk link.

### §7.1 Kontrol Tablosu — GATE-004a öncesi (tablo = işaretlenebilir)

| # | Kontrol | Yöntem | Beklenen | İlişkili risk |
|---|---------|--------|----------|---------------|
| 1 | 6 düzeltme kapandı mı | §3 tablosu satır satır | 6/6 | R2, R7, R9 |
| 2 | İki bacak da derleniyor mu | §7 komut 1 | 0 hata × 2 | R6 |
| 3 | Sayım eşiği tutuyor mu | §7 komut 2 | ≥5000 kayıt | R3 |
| 4 | Bozuk link | §7 komut 3 | 0 | — |
| 5 | Test iki bacakta yeşil mi | §7 komut 4 | 0 başarısız × 2 | R2 |
| 6 | Katman sınırı 13 sorusu | [checklists/layer-boundary-check](checklists/layer-boundary-check.md) | 13/13 cevaplı | R4, R7 |
| 7 | Proje adı tekleşti mi | `grep` ile `CallSuite` / `Application` | tek ad | R8 |
| 8 | `Result<T>` konumu sabit mi | kod yazılmadan karar | 1 konum | R10 |

**Sayım notu:** 8 kontrol = 8 işaret; 7 işaretli 1 boşsa kapı **açılmaz**, iş "neredeyse tamam" olarak raporlanmaz.

## §8 İlgili Dosyalar

| # | Dosya | Ne için |
|---|-------|---------|
| 1 | `VISION.md` | neden var, 7 ekran |
| 2 | `brain.md` | ADR-001..029 indeksi, kod kuralları |
| 3 | `keys.md` | anahtar kelime → dosya eşlemesi |
| 4 | `glossary.md` | terimler (CPM, TFM, dual bacak, K3…) |
| 5 | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | bu dosyanın birincil kaynağı |
| 6 | `checklists/layer-boundary-check.md` | katman sınırı 13 sorusu (R4 önleyicisi) |
| 7 | `architecture/index.md` | proje↔katman eşlemesinin nihai SSOT'u (yazılacak, DOĞRULAMA GEREKLİ) |
| 8 | `CLAUDE.md` · `WORKFLOW.md` · `MEMORY.md` · `index.md` | vault girişi ve akış |

## §9 Paralel Geliştirme Akışı (ADR-028 §4)

Üç aşamalı akış, 22 hedefli çözümün neden 3 paralel koldan gidebileceğini gösterir:

```text
ASAMAK 1  (seri)    : l0 Abstractions -> l1 Domain -> l2 Application
ASAMAK 2  (paralel) : l3 Crosscutting  ||  l4 Infrastructure  ||  l5 Protocol
ASAMAK 3  (seri+2)  : l6 Host -> l7 Desktop  ||  l7 Cli
          |
          +-- ayni dosya? -> SERI     farkli dosya? -> PARALEL
```

| Aşama | Koşul | Kanıt | Kırılırsa |
|-------|-------|-------|-----------|
| 1 | l0 bitmeden l1/l2 başlamaz | `dotnet build` 0 hata | sözleşmesiz kod yazılır, sonra taşınır |
| 2 | üç kol birbirine dosya yazmaz | dosya sahipliği çakışması yok | ezilme (AGENTS §7) |
| 3 | Host kaydı bitmeden ekran açılmaz | açılış testi yeşil | iki ekran farklı kurulum yapar |
| her aşamada | iki bacakta test | ADR-019 kanıtı | tek bacak yeşil = bitmemiş |
| aşama kapanışı | kutu + kanıt | ajan kutusu 5 alan dolu | kutusuz kapanış sayılmaz |

**Sınır notu:** paralel ajan sayısı **3 ile sınırlıdır** (AGENTS §5, 2026-09-29 olay kaydı); aşama 2'deki üç kol bu nedenle üç ayrı ajanla yürütülür, dördüncü kol eklenmez.

## §10 Sonraki Adım (tek)

§3 tablosundaki 6 düzeltmeyi sırayla kapat — öncelik 1. madde: CPM test paketleri, çünkü R2 ilk `dotnet restore` adımında NU1008 ile durur. Sonra §7 komutlarını çalıştır ve §7.1 tablosunda 8/8 işaretlenmeden GATE-004a kapatılmaz.
