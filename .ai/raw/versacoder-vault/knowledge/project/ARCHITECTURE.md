---
title: 'Versa Coder - ARCHITECTURE: Katmanlı Mimari, Arayüz Kontratları ve Doğrulama'
source: 'USER-BOOTSTRAP §9 (Root Documentation); versacoder proje planaması ai.md'
date: '2026-09-25'
status: 'ACTIVE'
confidence: 'SOURCE_VERIFIED'
related_decisions: ['ADR-001', 'ADR-003']
related_requirements: ['REQ-ARCH-001', 'REQ-ARCH-002']
---

# 1. Amaç ve Kapsam

Bu dosya Versa Coder'ın **mimari iskeletini** tanımlar: katmanlar ve bağımlılık yönü, katman sorumlulukları, arayüz kontratları, çift hedefin (`net48` + `net10.0-windows`) mimari yeri, veri katmanı ve Clean Architecture doğrulama kuralları.

Kapsam: katman tanımı, tek yönlü bağımlılık, kontrat listesi, DI kuralları, mimari ihlal sonucu, ADR bağlantısı. Kapsam dışı: kod stili (`.ai/.rules/coding.md`), SQL kuralları (`data.md`), ekran tasarımı (`ui.md`), gate işleyişi (`WORKFLOW.md`), güvenlik (`SECURITY.md`).

Neden bu dosya var: depoda 2026-09-24 itibarıyla **kod ve `*.csproj` yok** (`REPOSITORY_VERIFIED`). Mimari, kod yazıldıktan sonra **değişmez**; önce sabitlenir. Bu dosya GATE-004 (architecture) kanıtının kendisidir.

İçindekiler: 1 Amaç / 2 Katmanlar / 3 Sorumluluklar / 4 Projeler / 5 Kontratlar / 6 DI ve Sözleşmeler / 7 Çift Hedef / 8 Veri Katmanı / 9 Sistem Bileşenleri / 10 Doğrulama / 11 ADR ve Çakışmalar / 12 İhlal.

# 2. Katmanlar ve Bağımlılık Yönü

## 2.1 Dört katman

| Katman | Ne taşır | Bağımlı olabilir |
|---|---|---|
| `Core` | iş kuralları, saf hesaplama, **tüm arayüz kontratları** | **hiçbir şeye** |
| `Data` | veri erişimi (ADO.NET + Dapper) | `Core` |
| `Services` | orkestrasyon, akış, policy uygulaması | `Core`, `Data` |
| `UI` | ekranlar, olaylar, DevExpress bileşenleri | `Services`, `Core` |

Katman adları **iskele**dir; gereksinimle (`REQ-ARCH-001`) eşleşmesi `NEEDS_VALIDATION` işaretlidir.

## 2.2 Tek yön diyagramı

```text
        +-----------------------------+
        |  VersaCoder.Desktop / Cli   |
        +-------------+---------------+
                      | (UI / host)
                      v
        +-----------------------------+
        |          Services           |
        |  orkestrasyon, workflow, DI  |
        +------+----------------------+
               |                |
               v                v
     +---------------+   +--------------+
     |     Data      |   |     Core     |
     | ADO.NET+Dapper|-->|  is kurallari|
     +-------+-------+   |  KONTRATLAR  |
               |         +------+-------+
               |                ^
               +----------------+
    YON: UI -> Services -> Data -> Core   (asagi)
    GERI DONUS YASAKTIR                   (yukari referans yok)
```

## 2.3 Geri dönüş yasakları

| İhlal | Örnek | Sonuç |
|---|---|---|
| `Core` → `UI` | iş kuralında ekran çağrısı | mimari ihlal, incelemede döner |
| `Core` → `Data` | kuralda SQL çağrısı | ihlal; sorgu `Data`'da kalır |
| `Data` → `UI` | hata mesajını ekrana vermek | ihlal; hata tipi yukarı taşınır |
| `UI` → `Data` | ekrandan doğrudan SQL | **kesin yasak** |

Bağımlılık **arayüzle** ifade edilir; somut sınıf yukarı katmanda kullanılmaz. Yön kuralı yalnız kod incelemesiyle değil, proje referanslarıyla da zorlanabilir (Bölüm 4.2).

# 3. Katman Sorumlulukları

## 3.1 `Core`

Taşır: iş kuralları, saf hesaplama, değer tipleri, **tüm arayüz kontratları** (Bölüm 5), hata/tip tanımları. Yapamaz: ekran çağırmak, SQL yazmak, dosya/zaman/ortam okumak (seam gerekir), dış servis çağırmak. Bağımlılık: **yok** (bileşen dışına sıfır referans).

## 3.2 `Data`

Taşır: ADO.NET bağlantıları, Dapper sorguları, parametreli SQL (`@p1..@pN`), migration, `IRepository`/`IUnitOfWork` uygulamaları. Yapamaz: ekran bilgisi üretmek, iş kuralı yorumlamak, ham veriyi doğrulama katmanı olmadan yukarı göndermek. Veritabanı: SQL Server + SQLite (ADR-004). **EF Core yasaktır.**

## 3.3 `Services`

Taşır: akış orkestrasyonu (ana akıştaki halkalar), policy değerlendirmesi, oturum/workflow yürütmesi, tool/MCP çağrısı koordinasyonu, onay (approval) akışı. Yapamaz: doğrudan DevExpress bileşenine dokunmak (`UI` işidir), iş kuralını `Core` dışında yeniden tanımlamak. Kural: iş mantığı `UI`'a **gömülmez**; `UI` yalnız gösterir ve olay iletir.

## 3.4 `UI`

Taşır: WinForms ekranları, `RibbonControl`/`DockManager`/`DocumentManager` yerleşimi, `.Designer.cs` + `.resx` + `ResourceManager`, kullanıcı olayları. Yapamaz: SQL yazmak, iş kuralı hesaplamak, sırrı ekranda/logda göstermek. Ekran metninin tamamı `.resx` içindedir (`.ai/.rules/ui.md`).

# 4. Uygulama Projeleri

## 4.1 İki uygulama

| Uygulama | Rol | UI |
|---|---|---|
| `VersaCoder.Desktop` | görsel çalışma ortamı | WinForms + DevExpress v26.1 |
| `VersaCoder.Cli` | başsız/betik erişimi | yok (konsol) |

İkisi de `net48` **ve** `net10.0-windows` hedeflerinde çalışmalıdır (ADR-003). `Cli`, `Services` + `Core`'u kullanır; `UI` katmanını **kullanmaz**.

## 4.2 Proje bölme (fiziksel zorlama)

**Kural:** her katman ayrı sınıf kütüphanesi (DLL); **döngüsel bağımlılık yasak**. Dağıtım biçimi (katman başına tek `*.csproj` mi, birkaç katmanın birleşmesi mi) `NEEDS_VALIDATION`dır: ilk `*.csproj` yazımında `architect` karar verir ve karar ADR'ye işlenir. Karar verildiğinde bağımlılık yönü **proje referanslarıyla fiziksel zorlanır**; fiziksel zorlama kod incelemesinden güvenlidir.

```text
VersaCoder.Desktop --> Services --> Data --> Core
VersaCoder.Cli     --> Services --> Data --> Core
                              \\--> Core
Core --> (hicbir projeye referans vermez)
```

# 5. Arayüz Kontratları

Tüm kontratlar **`Core` katmanında** tanımlanır; uygulaması ait katmandadır. Bağımlılık daima bu yönde: `UI`/`Services` → `Core` arayüzü → somut sınıf DI ile enjekte edilir.

| # | Kontrat | Ne yapar | Uygulayan | Kullanan |
|---|---|---|---|---|
| 1 | `IAgentRunner` | agent yürütmesi başlatır/durdurur | `Services` | `Services`, `UI` |
| 2 | `IToolGateway` | tool çağrısını güvenli kapıdan geçirir | `Services` | `Services` |
| 3 | `IMcpClient` | MCP sunucusuyla konuşur | `Services` | `Services` |
| 4 | `ISmartModelRouter` | model/LLM seçimini yapar (LLM = bileşen) | `Services` | `Services` |
| 5 | `ISessionManager` | oturum yaşam döngüsü + kayıt | `Services` | `Services`, `UI` |
| 6 | `IWorkflowEngine` | 13 halka + kapı işleyişi | `Services` | `Services`, `UI` |
| 7 | `ISkillRegistry` | skill kaydı ve çağrısı | `Services` | `Services` |
| 8 | `ISecurityService` | policy değerlendirme, en az yetki | `Services` | `Services`, `UI` |
| 9 | `ICredentialVault` | sır saklama (AES-256-GCM) | `Data` | `Services` |
| 10 | `IRepository<T>` | veri okuma/yazma soyutlaması | `Data` | `Services` |
| 11 | `IUnitOfWork` | işlem (transaction) bütünlüğü | `Data` | `Services` |
| 12 | `IAuditTrail` | denetim kaydı yazma | `Data` | `Services` |
| 13 | `IContextStore` | bağlam saklama/geri çağırma | `Data` | `Services` |
| 14 | `IPolicyEngine` | kural/izin değerlendirme | `Core` (tip) / `Services` | `Services` |

Notlar: 1-11 numaralı kontratlar görev kaynağıyla (`USER-BOOTSTRAP §9`) verilmiştir; 12-14 mimari gereklilikten eklenmiştir. `ICredentialVault`/`IAuditTrail`'in hangi katmanda uygulanacağı **`NEEDS_VALIDATION`** (`Data` = kodlanmış tahmin; `SECURITY.md` ile birlikte değerlendirilip ADR'ye taşınır). Her kontratın tek implementasyonu olmak zorunda değildir; seçim DI yapılandırmasında yapılır, kod içinde somut sınıf `new` ile **yaratılmaz**.

# 6. DI ve Sözleşme Kuralları

| # | Kural | Neden |
|---|---|---|
| 1 | Bağımlılık **açık kontratla** (interface) verilir | gizli bağımlılık = döngüsel risk |
| 2 | Somut sınıf doğrudan `new` edilmez | test edilebilirlik |
| 3 | Her katman ayrı DLL; döngüsel referans **yasak** | derleme + bakım |
| 4 | İş mantığı `UI`'a **gömülmez** | aynı kural `Cli` için de gerekli |
| 5 | `Core`, dış sisteme (ekran, dosya, ağ, saat) **bağımlı olmaz** | saf çekirdek |
| 6 | DI yaşam süresi kayıtlıdır (singleton/scoped/transient) | ömür hataları |
| 7 | Düşük bağlama / yüksek cohesion: yeni sorum = yeni kontrat | refactor maliyeti |
| 8 | Güvenlik **by design**: policy, denetim ve onay `Services` akışında | sonradan eklenemez |

# 7. Çift Hedefin Mimari Yeri

Çift hedef (`net48` + `net10.0-windows`, ADR-003) **bir mimari özelliktir**; kod içine yayılmaz, **sınırda** durur: hedef listesi proje dosyasında tek yerde; paket sürümü iki hedefte de erişilebilir olmalı (`.ai/.rules/dependencies.md`); katman sınırında yalnız gerekiyorsa `#if`; test **iki hedefte de** çalışır (`GATE-007`).

| Fark sınıfı | ADR gerekir mi? |
|---|---|
| Davranış farkı (iki hedefte farklı sonuç) | **Evet** |
| API yokluğu (bir hedefte API yok) | **Evet** (alternatif tasarım) |
| Yalnız sözdizimi / derleme farkı | Hayır (`coding.md` yeterli) |

`Data` katmanı TFM'den **bağımsız** olmalıdır: TFM farkı `IRepository` gibi arayüzlerin imzasına **yansımaz**; yansırsa arayüz ikiye bölünür ve bölünme ADR ile kayda geçer.

# 8. Veri Katmanı Mimarisi

| Kural | Değer | Kaynak |
|---|---|---|
| Teknoloji | ADO.NET + Dapper | ADR-004 (`accepted`) |
| SQL stili | parametreli, tehlikesiz (`@p1..@pN`, "PDO gibi") | `data.md` |
| EF Core | **yasak** (net48 desteklemiyor) | `data.md` |
| Veritabanı | SQL Server + SQLite | ADR-004 |
| Soyutlama | `IRepository<T>` + `IUnitOfWork` | Bölüm 5 |
| UI'dan SQL | **kesin yasak** | Bölüm 2.3 |
| Bağlantı yaşam döngüsü | `Data` yönetir; `UI`/`Services` bağlantı **açmaz** | `data.md` |

```text
UI --(istem)--> Services --(IRepository/IUnitOfWork)--> Data
                                                 --(@p1..@pN)--> DB
  ham SQL, baglanti nesnesi, Dapper tipi asagi inmez (Core/UI'a)
```

# 9. Sistem Bileşenleri ve Ana Akış

Bileşen kümesi: `Agent, Session, Workflow, Tool, MCP, Plugin, Skill, Memory, Policy, Permission, Context, Validation, Execution, Observability`.

| Bileşen | Kontrat |
|---|---|
| Agent | `IAgentRunner` |
| Session | `ISessionManager` |
| Workflow | `IWorkflowEngine` |
| Tool | `IToolGateway` |
| MCP | `IMcpClient` |
| Skill | `ISkillRegistry` |
| Policy / Permission | `IPolicyEngine`, `ISecurityService` |
| Memory / Context | `IContextStore` |
| Validation | `IPolicyEngine` + `Services` akışı (`NEEDS_VALIDATION`) |
| Execution | `IAgentRunner` + `IToolGateway` |
| Observability | `IAuditTrail` (`NEEDS_VALIDATION`: metrik/günlük sözleşmesi) |
| Plugin | **kontrat yok** → `NEEDS_VALIDATION`, P0 |

Ana akış (`README.md` Bölüm 2.2 ile aynı): `User → Intent → Context → Requirement → Session → Agent/Model/Tool/MCP seçimi → Policy Evaluation → Workflow → Approval → Agent Execution → Tool Execution → Validation → Review → Memory → Persistence → Result`.

# 10. Clean Architecture Doğrulama Kuralları

## 10.1 Üç denetlenebilir sınır

| Sınır | Soru | Denetimi |
|---|---|---|
| Kod sınırı | hangi klasör hangi katmanda? | dosya yolu |
| Bağımlılık sınırı | hangi proje/paket referansı? | `*.csproj` |
| Karar sınırı | bu davranış hangi ADR'de? | `.ai/.decisions/` |

Denetlenebilir olmayan sınır kural sayılmaz: "katmanlara dikkat et" gibi ifadeler yasaktır; kural dosya yolu ve yön ile yazılır.

## 10.2 Dosya bazında dört soru

```text
[ ] Bu dosya hangi katmanda?        (Core / Data / Services / UI)
[ ] Bagimligi asagi mi gidiyor?     (evet olmali; geri donus YASAK)
[ ] Somut sinif mi, arayuz mu?      (arayuz olmali; DI ile)
[ ] Bu karar bir ADR'de kayitli mi? (yeni kararsa draft ADR ac)
```

Cevaplardan biri "hayır"sa kod yazımı durur (`WORKFLOW.md` Bölüm 5).

## 10.3 ADR zorunluluğu

| Durum | ADR zorunlu mu |
|---|---|
| Mimari sınır değişikliği | **Evet** |
| Teknoloji/paket seçimi (ilk kez) | **Evet** |
| Kural **gevşetme** (daha az kısıt) | **Evet** |
| Çift hedef TFM değişikliği | **Evet** |
| Stil tercihi (renk, hizalama) | Hayır (P2/P3) |
| Aynı kararın tekrarı | Hayır (önceki ADR okunur) |

Format Nygard'dır: `1. Durum` / `2. Bağlam` / `3. Karar` / `4. Kanıtlar` / `5. Tarihçe`. Ad: `ADR-<NNN>-<slug>.md`; dizin: `.ai/.decisions/accepted/` ya da `draft/`. `draft` karar üretim için **yeterli değildir**.

# 11. ADR Bağlantısı ve Çakışmalar

| ADR | Konu | Durum |
|---|---|---|
| ADR-001 | DevExpress sürümü + yerel feed gerçekliği | `accepted` |
| ADR-002 | Solution tier scope | `draft` |
| ADR-003 | Çift hedef | `accepted` |
| ADR-004 | Veri yığını | `accepted` |
| ADR-005 | Elle tasarlayıcı | `accepted` |
| ADR-006 | DevExpress API snippet'leri | `draft` |
| ADR-007 | Rol kataloğu ile disk uyumsuzluğu (katalog dışı 9 rol + ux/ui ad farkı) | `accepted` |

| Çakışma | Konu | Çözüm |
|---|---|---|
| C-ARCH-001 | eski yazım `net462`/`net8.0-windows` | **ADR-003 kazanır**; kök yazım revize edilir |
| C-ARCH-002 | feed'de `net10` varyantı örneklemede **yok** | ilk restore'da doğrulanır; paket `net10` isterse ADR-003 yeniden açılır |
| C-ARCH-003 | ADR-006 `draft` üretimde geçerli mi? | **hayır**; `coding.md` kalıcı kuraldır (`DURUM: OPEN`) |

# 12. Mimari İhlal, Sonuç ve Doğrulama

## 12.1 Tespit ve sonuç

Tespit üç yerde: kod incelemesi (`reviewer`), proje dosyası (`build-engineer`), ADR ile çelişen kod (`architect`).

```text
1) ihlal raporlanir (dosya yolu + satir)
2) kod geri alinir ya da duzeltilir (snapshot)
3) gerekce: kural mi eksik, kod mu yanlis?
4) kural eksikse -> 7 adimli genisletme (AGENTS.md Bolum 6.3)
5) kod yanlissa -> duzeltme + test (GATE-007)
6) oturuma SINIR: EVET ya da KARAR satiri
```

Aynı hatada **3 başarısız düzeltme** sonrası durulur; şüpheli varsayım adlandırılır, karar `architect`'e gider (`.ai/.rules/workflow.md` Bölüm 7).

## 12.2 Doğrulama kriterleri

```text
[ ] Metadata ilk 12 satirda 7 alan; Bolum 1-12 sirasi
[ ] 4 katman + ASCII yon diyagrami + 4 geri donus yasagi (Bolum 2)
[ ] Katman sorumluluklari "tasier/yapamaz" biciminde (Bolum 3)
[ ] 2 uygulama + proje bolme NEEDS_VALIDATION (Bolum 4)
[ ] Kontrat listesi 14 arayuz + sahip/kullanan (Bolum 5)
[ ] 8 DI/sozlesme kurali (Bolum 6)
[ ] Cift hedef 3 fark sinifi + TFM bagimsizligi (Bolum 7)
[ ] Veri: Dapper, parametreli SQL, EF Core yasak (Bolum 8)
[ ] 13 sistem bileseni + kontrat eslesmesi (Bolum 9)
[ ] 3 sinir + 4 soru + ADR zorunluluk tablosu (Bolum 10)
[ ] 7 ADR + 3 CONFLICT (Bolum 11)
[ ] Ihlal 6 adim + 3 deneme (Bolum 12.1)
[ ] Uydurma sinif/paket/surum adi yok; bilmeyen NEEDS_VALIDATION
[ ] 240-320 satir araliginda
```

| Tarih | Değişiklik |
|---|---|
| 2026-09-24 | İlk yazım - 4 katman + tek yön, 14 kontrat listesi, DI kuralları, çift hedef yeri, veri katmanı, 13 bileşen eşlemesi, doğrulama 3 sınır + 4 soru, 6 ADR + 3 çakışma |
| 2026-09-25 | Denetim düzeltmesi - ADR-007 eklendi (7 ADR), C-ARCH-003 `DURUM: OPEN`; kanıt: .ai denetim raporu SES-20260925-ARCH |
