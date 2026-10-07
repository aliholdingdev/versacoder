---
title: "VersaCoder — Terimler Sözlüğü (TR/EN)"
type: reference
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Proje genelinde 118 terimin Türkçe/İngilizce karşılığı, kısa tanımı, bu projedeki nedeni ve kullanıldığı yer"
---

# VersaCoder — Terimler Sözlüğü (`glossary.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[brain.md]] · [[MEMORY.md]] · [[reports/00-kapsam-analizi.md]] · [[.agents/AGENTS.md]] · [[.templates/index.md]]

> **Tek cümlelik sorumluluk:** *Bu terim ne demek, neden bu projede önemli, nerede kullanılıyor* sorusunun tek cevabıdır.
> Bu dosya **tanım verir, karar vermez** — karar `brain.md`, kural `CLAUDE.md`, yol `keys.md` üzerindedir (SRP).

---

## §1 Kullanım Kuralı

```
1. Terimi bu dosyada harfe göre bul (§4)
2. Tanımı oku, "Neden önemli" sütununu oku
3. "Kullanım" sütunundaki yola git — asıl içerik oradadır
4. 20 temel terimin derinleşmesi için §5'e git
5. Terim yoksa log.md'ye ekle, uydurma tanımlama (G9)
```

| # | Kural | Neden |
|---|---|---|
| 1 | Bağlantı **yalnız var olana** verilir | Bozuk link = G4 RED |
| 2 | Henüz yazılmamış SSOT dosyası `` `yol` ``(planlı) olarak yazılır, **link yapılmaz** | Uydurma path yasak (G9) |
| 3 | Tanım **1–2 cümle**; gerekçe ayrı sütunda | Sözlük uzarsa `keys.md`'ye benzer |
| 4 | TR terim esas, EN eşdeğer yan sütun | Ekip TR konuşur, kod EN kalır |
| 5 | Çelişen tanımda **bu dosya değil**, kaynak dosya kazanır | SSOT korunur |

> **UYARI:** Sözlükte **teknoloji kararı yoktur**. "EF Core kullanılmaz" kararı `CLAUDE.md` G2'de yaşar; burada yalnız *"EF Core nedir, neden burada yasak"* cevaplanır.

---

## §2 Kayıt Biçimi

Her satır 6 alan taşır:

| # | Alan | İçerik | Örnek |
|---|---|---|---|
| 1 | **Terim** | Türkçe terim (kullanılan dil) | veri erişim |
| 2 | **EN** | İngilizce eşdeğer | data access |
| 3 | **Tanım** | 1–2 cümlelik nesnel tanım | ADO.NET'in projedeki karşılığı |
| 4 | **Neden önemli** | Bu projede tek satırlık gerekçe | EF Core yasak olduğu için |
| 5 | **İlgili** | 2–3 yakındaki terim | Dapper · SqlClient |
| 6 | **Kullanım** | İçeriğin gerçekten olduğu yer | [CLAUDE.md](CLAUDE.md) |

---

## §3 Harf Dizini

| Harf | Terim | Harf | Terim | Harf | Terim |
|---|---|---|---|---|---|
| A | 8 | J | 2 | S | 9 |
| B | 5 | K | 5 | T | 5 |
| C | 9 | L | 4 | U | 4 |
| D | 6 | M | 6 | V | 4 |
| E | 4 | N | 4 | W | 4 |
| F | 3 | O | 4 | X | 3 |
| G | 3 | P | 6 | Y | 3 |
| H | 4 | Q | 2 | Z | 2 |
| I | 3 | R | 6 | | |

**Toplam: 118 terim** · 20 tanesi §5'te genişletilmiştir.

---

## §4 A–Z Tabloları

### A

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| agentic IDE | agentic IDE | Ajan üretebilen, çağıran, sonuçlarını ekrana taşıyan geliştirme ortamı. | Ürünün tam vizyonu; 7 ekran bu tanımın görsel karşılığıdır. | AI coding platform · agent | [index.md](index.md) |
| ADO.NET | ADO.NET | .NET'in düşük seviyeli veri erişimi: `DbConnection`, `DbCommand`, parameterized sorgu. | EF Core yasak olduğu için tek veri erişim yoludur. | Dapper · provider abstraction | [CLAUDE.md](CLAUDE.md) |
| ADR | Architecture Decision Record | Bir mimari kararın bağlam + seçenek + karar + sonuç şeklinde tek dosyaya kaydı. | Kararı tartışmaya kapatır; sonraki oturum nedenini okur. | karar · gate | [.templates/adr/adr-nygard-template.md](.templates/adr/adr-nygard-template.md) |
| agent (ajan) | agent | Amaç taşıyan, araç çağıran, kendi döngüsünde karar veren yazılım birimi. | Vault ajanları ile ürünün runtime ajanları aynı terimi paylaşır. | subagent · session · tool | [.agents/AGENTS.md](.agents/AGENTS.md) |
| anayasa | constitution | Vault'un bağlayıcı guardrail seti (G1..G12); çelişen her maddeyi geçersiz kılar. | Tek dosyadır, üstünlüğü tartışılmaz. | guardrail · SSOT | [CLAUDE.md](CLAUDE.md) |
| anti-pattern | anti-pattern | Bilinen ama zararlı, tekrarlanan çözüm kalıbı (ör. magic connection string). | Review'da seviye üreten kavramdır. | bulgu · red team | [.agents/reviewer.md](.agents/reviewer.md) |
| artefakt | artifact | Bir iş adımının somut, ölçülebilir çıktısı: dosya, rapor, log, build çıktısı. | "Bitti" iddiası yalnız artefaktla kanıtlanır (G5). | DoD · checkpoint | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| ASCII diyagram | ASCII diagram | Yalnız düz ASCII karakterlerle çizilen şema veya tel kafes. | Unicode kutu çizimleri yasak; kopya-paste ve link taramasında bozulmaz. | wireframe · WF-xx | [.templates/architecture/ascii-diagram-template.md](.templates/architecture/ascii-diagram-template.md) |

### B

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| bag-of-tasks | bag-of-tasks | Birbirine bağımlı olmayan, sırası önemsiz görev çantası; paralel dağıtılabilir. | Alt ajan dağıtımında hangi işin bölüneceğini belirler. | subagent · checkpoint | [.agents/AGENTS.md](.agents/AGENTS.md) |
| bootstrap | bootstrap | Vault'un doğduğu birincil kaynaklar: ai.01/02/03 plan dosyaları. | Tüm senior kararların gerekçesi buradan gelir. | kaynak hiyerarşisi | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| belirsizlik | unknown / open question | Karar zamanı gelmiş ama cevabı henüz olmayan madde (B1–B4). | Sessiz varsayımla doldurulmaz; karar zamanı gelince sorulur. | varsayım · risk | [brain.md](brain.md) |
| build | build | Derleme; kodun derlendiğine dair tek geçerli kanıt çıktısı. | Kanıtsız "bitti" yasağının Dayanak'ıdır (G5). | DoD · test | [CLAUDE.md](CLAUDE.md) |
| bulgu | finding | Code review çıktısı: seviye + konum + düzeltme üçlüsü. | Drama yok, tahmin yok; düzeltilene kadar RED kalır. | code review · seviye | [.agents/reviewer.md](.agents/reviewer.md) |

### C

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| checkpoint | checkpoint | Bir iş parçasının geri dönülebilir, doğrulanabilir durum anı. | Hata halinde tüm iş değil, son checkpoint kaybedilir. | artefakt · DoD | [brain.md](brain.md) |
| CI/CD | CI/CD | Kodun her değişiklikte otomatik derlenip test edildiği hat. | Teslim kanıtının tekrarlanabilir kaynağıdır. | build · test | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| CLAUDE.md | CLAUDE.md (constitution) | AI anayasası: G1..G12 guardrail + zero-hallucination + çelişki çözümü. | Diğer her vault dosyasıyla çelişirse bu kazanır. | anayasa · guardrail | [CLAUDE.md](CLAUDE.md) |
| Clean Architecture | Clean Architecture | Bağımlılıkların yalnız içe doğru aktığı katmanlı mimari. | Katman sırası ihlali mimari RED sayılır. | katman · SOLID | [index.md](index.md) |
| code review | code review | Kodun ikinci bir gözle, bulgu listesiyle denetlenmesi. | GREEN olmadan teste geçilmez; RED → developer'a döner. | bulgu · devre | [.agents/reviewer.md](.agents/reviewer.md) |
| CQRS | Command Query Responsibility Segregation | Komut (yazma) ve sorgu (okuma) ayrımı. | Gerektiğinde uygulanır; zorunlu katman değildir. | DDD · Domain | [CLAUDE.md](CLAUDE.md) |
| commit | commit | Git deposuna kalıcı, mesajlı kayıt birimi. | "Sıfır commit" durumu bile raporlanacak bir gerçektir. | git repo · log | [MEMORY.md](MEMORY.md) |
| CPM | Central Package Management | `Directory.Packages.props` ile paket sürümlerinin tek yerde tutulması. | Proje başına sürüm yazımı yasak. | NuGet · yığın | [index.md](index.md) |
| CR-only | CR-only line ending | Satır sonunun yalnız `\r` olduğu bozuk dosya biçimi. | ai.03.md bu biçimde; parse hatası üretir, okuma anında normalize edilir. | R2 · satır sonu | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### D

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| Dapper | Dapper | Parameterized sorguyu nesneye eşleyen micro-ORM. | EF Core yerine tercih edilen tek eşleyici; ince katman. | ADO.NET · micro-ORM | [CLAUDE.md](CLAUDE.md) |
| Definition of Done (DoD) | DoD | Bir part'ın bittiğine dair ölçülebilir kabul listesi. | İşaretleme ile değil, çıktı ile kapanır. | artefakt · gate | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| devre | handover | Rol→rol teslim paketi: ne üretildi, kanıt, eksik, sonraki adım. | 4 madde eksikse devre RED'dir. | subagent · bulgu | [.agents/AGENTS.md](.agents/AGENTS.md) |
| Discovery | discovery | Faz 1: kaynak okuma ve keşif. | GATE-001/002 bu fazın kapılarıdır. | gate · part | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| bağımlılık enjeksiyonu | dependency injection (DI) | Bağımlılıkların dışarıdan verilmesi; composition root Host katmanındadır. | Test edilebilirlik ve katman bağımsızlığı için zorunlu. | Host · katman | [index.md](index.md) |
| Domain | Domain | İş kurallarının, entity'lerin ve invariant'ların yaşadığı katman. | Hiçbir katmana bağımlı değildir; en iç halka. | entity · invariant | [index.md](index.md) |

### E

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| EF Core | Entity Framework Core (YASAK) | Microsoft'un ORM'i; DbContext + LINQ-to-Entities. | net48 bacağında desteklenmez; iki bacak tek kaynağı için yasaklanmıştır (G2). | ADO.NET · Dapper | [CLAUDE.md](CLAUDE.md) |
| entity | entity | İş verisinin temel modeli; veritabanı satırının kod karşılığı. | Domain katmanında yaşar, Dapper ile eşlenir. | Domain · Dapper | [index.md](index.md) |
| enjeksiyon (SQL) | SQL injection | Kullanıcı girdisinin sorguya kod olarak sızması. | Tek savunma: parameterized sorgu; string concat yasak. | parameterized · R3 | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| envanter | inventory | Elde bulunan her şeyin liste hâli: dosya, paket, ekran, ADR. | Kapsam analizinin ana çıktısı; varsayım kaynaklarını açığa çıkarır. | kapsam · audit | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### F

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| form üçlüsü | form triplet | `.Form.cs` + `.Designer.cs` + `.resx` dosyasının birlikte varlığı. | Designer'sız form üretilmesi RED sayılır. | resx · WinForms | [.templates/frontend/form-template.md](.templates/frontend/form-template.md) |
| frontmatter | YAML frontmatter | MD dosyasının başındaki `---` ile çevrili metadata bloğu. | 8 alan zorunlu; `authority: SSOT` hepsinde aynıdır (G4). | YAML · SSOT | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| freeze | freeze | Kapsam veya gereksinimin dondurulması. | GATE-002 Requirements Freeze sonrası kapsam değişimi = yeniden onay. | gate · kapsam | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### G

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| gate | gate (GATE-000..005) | İşin bir sonraki faza geçmesi için gereken onaylı kapı. | Onaysız geçiş = süreç ihlali; GATE-003 kodlamaya geçişin kilitidir. | DoD · part | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| guardrail | guardrail (G1..G12) | İhlal edilemez, türüne göre CRITICAL/HIGH/RED olan 12 kural. | Anayasa başlığı altında tanımlıdır; istisnası yoktur. | anayasa · G1..G12 | [CLAUDE.md](CLAUDE.md) |
| git repo | repository | Kodun sürüm kontrol altındaki deposu. | Şu an **commit yok**, tek untracked girdi `?? .ai/`. | commit · MEMORY | [MEMORY.md](MEMORY.md) |

### H

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| handover | handover | Devre; bir rolün işi diğerine 4 maddelik paketle teslimi. | Eksik paket = RED; bkz. devre. | devre · subagent | [.agents/AGENTS.md](.agents/AGENTS.md) |
| hosting model | hosting model | Uygulamanın nasıl ayağa kalktığı: composition root, DI, lifecycle. | `Host` katmanı tek composition root'tur; UI servizleri oradan alır. | Host · DI | [index.md](index.md) |
| High DPI | High DPI scaling | Windows yüksek çözünürlüklü ekran ölçeklemesi. | WinForms + DevExpress tasarımında göz ardı edilirse form bozulur. | WinForms · resx | [.templates/frontend/form-template.md](.templates/frontend/form-template.md) |
| hedef bacak | target framework leg | `net48` veya `net10.0-windows` taraflarından biri. | Her karar iki bacakta da değerlendirilir (G6). | iki bacak · #if | [CLAUDE.md](CLAUDE.md) |

### I

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| iki bacak | dual-target | Aynı kaynak kodun `net48` + `net10.0-windows` üzerinde çalışması. | Tek bacaklı tasarım yasak; `#if` yalnız platform farklarında. | net48 · net10 | [CLAUDE.md](CLAUDE.md) |
| invariant | invariant | Hiçbir koşulda bozulmaması gereken kural. | Domain katmanında korunur; testler bunu savunur. | Domain · test | [.templates/backend/service-template.md](.templates/backend/service-template.md) |
| interface (sözleşme) | interface | Katmanlar arası tek bağlantı yüzeyi. | Alt katman üst katmanı tanımaz; DIP budur. | katman · SOLID | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |

### J

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| JSON | JSON | Protocol mesajlarının taşıdığı metin biçimi. | tool çağrıları ve sonuçları JSON şemasıyla tanımlanır. | Protocol · tool | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| JSON şeması | JSON schema | Mesaj alanlarının ad/tip/zorunluluk tanımı. | Sözleşme bozulursa ajanlar sessizce yanlış anlaşır; spekülasyon yasak. | Protocol · transcript | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |

### K

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| katman | layer | Clean Architecture'da tek sorumluluk taşıyan proje/dizin halkası. | Sıra sabittir; bağımlılık tersine akarsa mimari RED. | katman sırası | [index.md](index.md) |
| katman sırası | layer order | `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`. | Bu sıra değişmez; `.ai` içinde de geçerlidir. | Clean Architecture | [CLAUDE.md](CLAUDE.md) |
| kopyalama yasağı | no-copy rule | CoreMusic vault'unun içerik olarak kopyalanmaması (G1 / SSUT). | Yalnız **yapı** referans alınır; içerik elle sıfırdan yazılır. | SSOT · G1 | [CLAUDE.md](CLAUDE.md) |
| kısıt | constraint | Karar alanını daraltan değişmez kural (teknoloji, lisans, platform). | Kısıt önce tanımlanır, seçenekler sonra üretilir. | kriter · ADR | [brain.md](brain.md) |
| kayıt (log) | log | Tarihli olay günlüğü: ne değişti, neden, kim onayladı. | `log.md` oturum sonunda güncellenir; bellek değildir. | MEMORY · oturum | [index.md](index.md) |

### L

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| legacy | legacy / net48 | Eski .NET Framework tarafı; modern olmayan ama zorunlu kanıt bacağı. | "Sonra bakarız" yasak; her part'ta net48 da doğrulanır. | net48 · hedef bacak | [CLAUDE.md](CLAUDE.md) |
| LLM | large language model | Üretken yapay zeka modeli (OpenAI-uyumlu / Ollama). | Provider öncelik sırası belirsizliktir (B4), PART 7'de karar verilir. | provider abstraction · RAG | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| link taraması | link scan | Vault içindeki tüm linklerin hedefte varlığının otomatik denetimi. | ~500 MD'de bozuk link R6 riskidir; otomatik olmalıdır. | R6 · G4 | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| LINQ | LINQ | .NET'in sorgu sözdizimi. | EF'siz projede yalnız Dapper/ADO.NET nesneleri üzerinde sınırlı kullanılır. | Dapper · EF Core | [.templates/query/Query-Template.md](.templates/query/Query-Template.md) |

### M

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| MCP | Model Context Protocol | Ajanın harici sunucu/araç bağlaması için standart protokol. | Ürünün genişleme yüzeyi; sandbox ve gateway kuralları spec'te. | tool · gateway | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| migration | migration (raw SQL + version table) | Saf SQL script + `schema_version` tablosuyla şema güncelleme. | EF migrate yasak; elle yazılmış, denetlenebilir script. | SQLite · schema_version | [.templates/infrastructure/migration-template.md](.templates/infrastructure/migration-template.md) |
| micro-ORM | micro-ORM | Yalnız eşleme yapan, SORGU ÜRETMEYEN ince ORM (Dapper). | Tam ORM (EF) ile karıştırılmamalı; farkı yasak/izin sınırıdır. | Dapper · ADO.NET | [.templates/infrastructure/dapper-mapping-template.md](.templates/infrastructure/dapper-mapping-template.md) |
| MEMORY.md | MEMORY.md | Oturumlar arası kalıcı bellek: sabitler, anlık durum, yasaklar. | Oturum başında okunur, sonunda güncellenir. | oturum · geri çağırma | [MEMORY.md](MEMORY.md) |
| mimari onay | architecture approval | Mimari kararların kapıdan geçirilmesi (GATE-004). | ADR + katman sözleşmesi olmadan kod başlamaz. | ADR · gate | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| mesaj şeması | message schema | Ajan–tool–UI arasındaki alan adları ve tipleri. | Bozuk şema sessiz hata üretir; `spec/protocol-spec.md` (planlı) tutar. | Protocol · JSON | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |

### N

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| net48 | .NET Framework 4.8 | DevExpress'in tam destekli klasik Windows bacağı. | Legacy kanıt bacağı; modern dil özelliklerinin bir kısmı burada yoktur. | legacy · hedef bacak | [CLAUDE.md](CLAUDE.md) |
| net10.0-windows | .NET 10 (Windows TFM) | Modern, uzun ömürlü Windows bacağı. | Yeni özelliklerin hedefi; iki bacağın ortak kaynağı paylaşılır. | hedef bacak · #if | [CLAUDE.md](CLAUDE.md) |
| nullable | nullable reference types | `string?` ile ifade edilen null güvencesi. | Null kaynaklı hataları derleme anında yakalar; açıkça yönetilir. | C# · kod standardı | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| NuGet | NuGet | .NET paket deposu. | Sürümler CPM ile merkezî tutulur; proje başına sürüm yasak. | CPM · yığın | [index.md](index.md) |

### O

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| Office2010 | RibbonControlStyle.Office2010 | Zorunlu ribbon stili. | Başka stil (Office2007/Blue vb.) yazımı CRITICAL'dir. | ribbon · G3 | [CLAUDE.md](CLAUDE.md) |
| opencode | opencode | Bu vault'un üretildiği ajanlı IDE; referans kaynak kodu 33 paket. | PART 7'de ürünün protokol/tool modeline uyarlanacak. | agent · tool · session | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| ORM | object-relational mapping | Nesne ↔ tablo eşleme katmanı. | Tam ORM (EF) yasak; yerine micro-ORM (Dapper) kullanılır. | Dapper · EF Core | [CLAUDE.md](CLAUDE.md) |
| orkestrasyon | orchestration | Part ↔ gate ↔ ajan sırasının yürütülmesi. | Matris `engine.md` üzerindedir; sırasız başlama yasak. | gate · part | [index.md](index.md) |

### P

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| part | PART 1..8 | İş planının 8 bölümünden her biri. | Sıra zorunlu; atlanan part = atlanan gate. | gate · engine | [index.md](index.md) |
| parameterized | parameterized query | Sorgunun değerlerinin `@p` parametreleriyle verilmesi. | Injection'a karşı tek savunma; string concat RED'dir. | SQL injection · Dapper | [CLAUDE.md](CLAUDE.md) |
| plan mode | plan mode | Kod yazmadan önce planın çıkarılıp onaylandığı çalışma biçimi. | GATE-003'ten önce kod yok; plan onayı olmadan uygulama yok. | gate · GATE-003 | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| prompt | prompt | Ajanlara verilen talimat metni (master prompt dahil). | Prompt kaynak dosyalarıdır; içeriği değiştirilmek yerine referanslanır. | bootstrap · frontmatter | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| provider abstraction | provider abstraction | SQLite ve SQL Server'ın arkasındaki ortak arayüz. | Tek sağlayıcıya kilit yasak; kod provider seçmez, arayüz seçer. | SQLite · SqlClient | [CLAUDE.md](CLAUDE.md) |
| Protocol | Protocol katmanı | Mesaj şeması, tool sözleşmesi ve streaming taşıyan katman. | Host ile UI arasında tek geçiş yüzeyidir. | katman sırası · streaming | [index.md](index.md) |

### Q

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| query (sorgu) | query | Parameterized SQL okuma/ifade sorgusu. | Dapper'a giren ham sorgu; şablon `.templates/query/` altındadır. | Dapper · parameterized | [.templates/query/Query-Template.md](.templates/query/Query-Template.md) |
| Q&A (keşif notları) | Q&A notes | ai.02 dosyasındaki soru-cevap arşivi; 5 senior kararın kaynağı. | Karar gerekçesi buradan gelir; yorumla değiştirilemez. | bootstrap · ADR | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### R

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| RAG | Retrieval-Augmented Generation | Yanıt üretilmeden önce ilgili belgenin getirilmesi. | Ürünün bilgi getirme stratejisi; provider seçiminden (B4) ayrışır. | LLM · provider | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| ribbon | ribbon | DevExpress ribbon denetimi; pencere üstü şerit arayüz. | Yalnız `RibbonControlStyle.Office2010`, pencere yalnız `RibbonForm`. | Office2010 · G3 | [CLAUDE.md](CLAUDE.md) |
| risk | risk (R1–R6) | Gerçekleşme olasılığı olan, etkisi ölçülmüş tehdit. | Her riskin bir azaltması vardır; sahipsiz risk = beklenen sorun. | belirsizlik · varsayım | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| red team | red team | Kendi işine karşı oynama: sık yapılan hataları arama. | Vault'ta `keys.md` §7 ve `brain.md` §8 bu işi yapar. | anti-pattern · bulgu | [keys.md](keys.md) |
| refaktör | refactor | Davranışı değiştirmeden yapıyı iyileştirme. | Test yoksa refaktör yok; davranış kanıtı önce gelir. | test coverage · code review | [.agents/developer.md](.agents/developer.md) |
| rol | role | Mutlak rol tanımı ve öncelik çatışması kuralı. | Roller çatışırsa `ROLE.md` kazanır (henüz planlı). | subagent · devre | [index.md](index.md) |

### S

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| SQLite | SQLite | Dosya tabanlı yerel veritabanı; `Microsoft.Data.Sqlite`. | Yerel geliştirme ve ilk provider bacağı. | SqlClient · provider | [CLAUDE.md](CLAUDE.md) |
| SqlClient | SqlClient | SQL Server istemcisi; `Microsoft.Data.SqlClient`. | Üretim/uzak sunucu bacağı; ikisi de desteklenmek zorunda. | SQLite · provider | [CLAUDE.md](CLAUDE.md) |
| SSOT | Single Source of Truth | Her konunun yalnız bir dosyada yaşaması ilkesi. | Tekrar = çelişki riski; çelişkide kaynak hiyerarşisi kazanır. | authority · anayasa | [CLAUDE.md](CLAUDE.md) |
| session | session | Ajanla yürütülen tek konuşma/çalışma birimi. | Bellek protokolü oturum başına çalışır: başta oku, sonda yaz. | MEMORY · transcript | [MEMORY.md](MEMORY.md) |
| skill | skill | İhtiyaç anında yüklenen, talimat + kaynak paketi yetenek. | Önce skill, sonra iş (G12); sırasız başlama yasak. | G12 · tool | [CLAUDE.md](CLAUDE.md) |
| streaming | streaming | Model çıktısının token token akması. | Protocol katmanı akışı taşır; UI kısmi metni gösterir. | Protocol · transcript | [index.md](index.md) |
| stub | stub | Yer tutucu / henüz gerçekleşmemiş sahte uygulama. | Stub'un "bitti" diye raporlanması yasak; `NEEDS_VALIDATION` ile işaretlenir. | NEEDS_VALIDATION · DoD | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| subagent | subagent | Ana akıştan bağımsız çalışan alt ajan. | Paralel işlerde kullanılır; devre paketi zorunludur. | agent · bag-of-tasks | [.templates/agents/subagent-template.md](.templates/agents/subagent-template.md) |
| şablon | template | `.templates/` altındaki üretim kalıbı. | Kalıp bir kez yazılır, içerik kalıba konur (R4 azaltması). | artefakt · R4 | [.templates/index.md](.templates/index.md) |

### T

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| test kapsamı | test coverage | Testlerin kod tabanını ne kadar kapsadığı. | Kanıtsız coverage hedefi yasak; sayı raporla gelir. | DoD · build | [.agents/tester.md](.agents/tester.md) |
| transcript | transcript | Oturumun ham konuşma kaydı (prompt + yanıt + araç çağrıları). | Hatayı geriye sarmanın tek kanıtı; özetle değiştirilmez. | session · log | [MEMORY.md](MEMORY.md) |
| tool | tool | Ajanın çağırabildiği yetenek (read/write/edit/bash/grep vb.). | opencode 33 paketinde tool sistemi `packages/core` içinde. | MCP · agent | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| teknoloji anayasası | technology constitution | Değişmez teknoloji tablosu: framework, UI, veri erişimi, test, uygulamalar. | Yığın dışına çıkma = RED; `CLAUDE.md` §4'tedir. | yığın · guardrail | [CLAUDE.md](CLAUDE.md) |
| üçüncü deneme kuralı | third-failure rule | Aynı hata için 3 başarısız düzeltmeden sonra durma. | Yalnız varsayımı adlandır ve kullanıcıya sor; devam etmek israftır. | dur & sor · risk | [brain.md](brain.md) |

### U

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| UI wireframe | UI wireframe | Ekranın ASCII tel kafes taslağı; renk/foto yok, yerleşim var. | GATE-003 onayı bu dosyalar üzerinden verilir. | WF-xx · gate | [index.md](index.md) |
| uydurma | hallucination | Var olmayanı varmış gibi yazma (G9). | Karşılığı 7 etiket: UNKNOWN, NEEDS_VALIDATION, INFERRED, WEB/REPOSITORY/SOURCE/REFERENCE_VERIFIED. | G9 · etiket | [CLAUDE.md](CLAUDE.md) |
| uygulama hedefi | target application | İki uygulama: `VersaCoder.Desktop` + `VersaCoder.Cli`. | Tek uygulama tasarımı yasak; ikisi de kapsamdadır. | host · kapsam | [index.md](index.md) |
| kullanıcı onayı | user approval | Yıkıcı veya kapsam değiştiren işlemde kullanıcının açık izni. | G7 (yıkıcı) ve GATE-003 (UI) için zorunludur. | G7 · gate | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### V

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| vault | vault | `.ai/` altındaki tüm bilgi kasası; projenin ikinci beyni. | Kökü `C:\versacoder\.ai` kullanıcı onayıyla sabitlenmiştir (A1). | SSOT · MEMORY | [index.md](index.md) |
| VISION | VISION.md | Ürün vizyonu ve problem–çözüm matrisi. | 7 ekranla hizalıdır; henüz yazılmamıştır. | ekran · kapsam | [index.md](index.md) |
| varsayım | assumption (A1–A3) | Doğrulanmış kabul edilen ama kanıtlanmamış gerçek. | Doğrulama anı gelmezse sessizce kalır; açıkça listelenir. | belirsizlik · risk | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| veri erişim | data access | ADO.NET + Dapper katmanı; her sorgu parameterized. | EF Core yok, migration saf SQL; bkz. `.rules/data-access-rules.md` (planlı). | Dapper · migration | [CLAUDE.md](CLAUDE.md) |

### W

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| WF-xx | wireframe id | Yedek vault'taki 16 wireframe'in numarası (WF-00..WF-15). | R1 riski: 7 ekran ≠ 16 WF; isim farkı kullanıcı onayı ister. | UI wireframe · R1 | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |
| wireframe | wireframe | Ekranın yapısı: yerleşim, bölme, denetim adları. | Renk/tema GATE-003'ten sonra; önce yapı onaylanır. | UI · GATE-003 | [index.md](index.md) |
| WORKFLOW.md | WORKFLOW.md | Süreç, part sırası ve GATE-000..005'in SSOT'u. | Kapılar ve part sırası buradadır; çelişirse bu dosya kazanır. | gate · part | [WORKFLOW.md](WORKFLOW.md) |
| wizard | wizard | Yedek vault'taki adım adım ekran türü. | 7 ekran listesinde yok; PART 4'te yalnız bilgi olarak sayılır. | WF-xx · R1 | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### X

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| XtraForm | XtraForm | DevExpress temel formu. | Ana pencere olarak **yasak**; ana pencere `RibbonForm`'dur. | RibbonForm · G3 | [CLAUDE.md](CLAUDE.md) |
| XtraTabControl | XtraTabControl | DevExpress sekme denetimi. | 7 ekranın çoğunda bölümleme bu denetimle yapılır. | ribbon · UI | [.templates/frontend/form-template.md](.templates/frontend/form-template.md) |
| XML | XML | `.resx`, `.slnx`, ayar dosyalarının biçimi. | `.slnx` mi `.sln` mi belirsizliği B1'dir; PART 5'te karar. | B1 · resx | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### Y

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| YAML frontmatter | YAML frontmatter | Dosyanın başındaki `title,type,category,status,version,updated,authority,scope` bloğu. | 8 alan zorunlu, `authority: SSOT` tüm kök dosyalarda aynı. | frontmatter · G4 | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |
| yığın | stack | Seçilmiş teknoloji kümesi: C# · WinForms · DevExpress v26.1 · Dapper · xUnit. | Yığın dışına çıkılmaz (G8); alternatif ORM/framework eklenemez. | teknoloji anayasası | [CLAUDE.md](CLAUDE.md) |
| yedek vault | backup vault | `C:\www\versacoder\versacoder.ai` — yalnız **bilgi** kaynağı. | Kopyalama yasak (G1); içerik elle yeniden yazılır. | CoreMusic · G1 | [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) |

### Z

| Terim | EN | Tanım | Neden önemli | İlgili | Kullanım |
|---|---|---|---|---|---|
| Zero-Hallucination | zero-hallucination | Bilinmeyeni bilinmeyen olarak işaretleme protokolü (G9). | 7 doğrulama etiketi; uydurma madde silinir. | uydurma · G9 | [CLAUDE.md](CLAUDE.md) |
| zorunlu bağlantılar | required links | Her kök MD'nin başındaki `**Zorunlu Bağlantılar:**` satırı. | Navigasyonun omurgası; 10 hedefi de içermelidir. | index · navigation | [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) |

---

## §5 Genişletilmiş Tanımlar (20 Temel Terim)

> Bu 20 terim, vault'un geri kalanını anlamanın anahtarıdır. Her biri **ne olduğu → bu projede neden kritik → yanlış anlaşılırsa ne olur** sırasını izler.

### §5.1 agentic IDE — Ajanlı Geliştirme Ortamı

**Ne olduğu:** Klasik IDE'nin ajan katmanı eklenmiş hâli: kullanıcı bir hedef yazar, ajan plan çıkarır, dosyaları okur/yazar, araç çağırır, sonucu ekrana basar. Bu ürünün adı **VersaCoder**, kategorisi **AI Coding Platform / agentic IDE**.

**Neden kritik:** Vizyonun tamamı bu terimden türetilir. `index.md` §1'de üç soruya (*ne yapıyoruz / nasıl yapıyoruz / hangi kuralla*) ayrılan yapı, ajanın çalışma döngüsünü (okuma → karar → uygulama → kanıt) yansıtır. 7 ekranın 5'i doğrudan ajan etkileşimidir: ana IDE, chat, agent paneli, kod editörü, ayarlar.

**Yanlış anlaşılırsa:** "Chatbot tabanlı editör" sanılır; o hâlde plan, gate, katman ve test disiplini gereksiz görünür. Oysa bu proje **kanıtlayan mühendislik** ürünüdür, sohbet eklentisi değil.

Kullanım: [index.md](index.md) · [CLAUDE.md](CLAUDE.md)

### §5.2 vault — Bilgi Kasası

**Ne olduğu:** `C:\versacoder\.ai` altındaki, kendi kuralları olan, YAML frontmatter taşıyan Markdown kasası. Kökünde 14 belge, altında `.rules/`, `spec/`, `architecture/`, `.decisions/`, `ui-design/`, `reports/`, `scripts/`, `.templates/`, `.agents/` bulunur.

**Neden kritik:** Vault proje hafızasıdır; repo henüz **sıfır commit** olduğu için tek kalıcı bilgi kaynağı vault'tur. `index.md` navigasyonu, `keys.md` araması, `MEMORY.md` durumu, `brain.md` kararları taşır. Kural: her konu **tek** dosyada yaşar (SSOT), tekrar yasaktır.

**Yanlış anlaşılırsa:** İçerik iki yere yazılır → çelişki → hangisinin doğru olduğu belirsiz → G10 ile DECISION BLOCKED. Kopyalama (CoreMusic'ten) da bu yüzden yasaktır: G1.

Kullanım: [index.md](index.md) · [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md)

### §5.3 SSOT — Tek Bilgi Kaynağı

**Ne olduğu:** Single Source of Truth. Her olgunun **tek** bir dosyada yaşadığı, diğer dosyaların yalnız **bağlantı verdiği** ilkesi. Frontmatter'daki `authority: SSOT` alanı bu dosyanın o konudaki kaynak olduğunu beyan eder.

**Neden kritik:** 14 kök belgenin her biri tek sorumluluk taşır (SRP). `keys.md` içerik vermez, yalnız **gönderir**; `glossary.md` karar vermez, yalnız **tanımlar**; `brain.md` kural yazmaz, yalnız **gerekçelendirir**. Bu ayrım bozulursa vault kendi içinde çelişmeye başlar.

**Yanlış anlaşılırsa:** "Özet olsun diye" ikinci bir kopya yazılır. Çelişki ortaya çıktığında `ROLE.md` §3 kaynak hiyerarşisi uygulanır; çözülmeyene `CONFLICT DETECTED`.

Kullanım: [CLAUDE.md](CLAUDE.md) · [keys.md](keys.md)

### §5.4 katman (layer) — Mimari Halkalar

**Ne olduğu:** Bağımlılığın yalnız içe doğru aktığı sıralı halkalar: `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`. Her katman yalnız kendinden içtekileri tanır.

**Neden kritik:** Bu sıra `CLAUDE.md` §4'te değişmez tablo olarak kayıtlıdır ve hem kod hem vault için geçerlidir. `Infrastructure` veri erişimini (ADO.NET + Dapper) barındırır; `Protocol` ajan mesajlarını; `Host` composition root'u; `UI` WinForms + DevExpress'i.

**Yanlış anlaşılırsa:** UI doğrudan SQL çağırır, Domain UI'a bakar → mimari RED. `#if` ile iş mantığı bölünmesi de aynı hatanın iki bacaklı versiyonudur (G6).

Kullanım: [CLAUDE.md](CLAUDE.md) · [index.md](index.md)

### §5.5 EF Core — Yasak ORM

**Ne olduğu:** `DbContext` + LINQ-to-Entities tabanlı tam ORM. Bu projede **kullanılmaz**: G2 gereği `Microsoft.EntityFrameworkCore*` referansı CRITICAL'dir, derleme RED sayılır.

**Neden kritik:** Tek gerekçe **iki bacak**tır: `net48` üzerinde EF Core 9 desteklenmez. Ortak kaynak tek yolunu kapatırsa EF zorunlu olarak düşer. Yerine **ADO.NET + Dapper** + saf SQL migration gelir.

**Yanlış anlaşılırsa:** "Modern durur" diye EF eklenirse hem G2 hem G6 hem G8 ihlal edilir, hem de net48 bacağı ilk derlemede kırılır. ai.01.md'deki EF Core 9.0.0 referansı bu yüzden **REDDEDİLMİŞ**tir.

Kullanım: [CLAUDE.md](CLAUDE.md) · [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md)

### §5.6 ADO.NET + Dapper — Tek Veri Yolu

**Ne olduğu:** `DbConnection`/`DbCommand` ile ham, **parameterized** sorgu çalıştırmak; sonucu Dapper ile nesneye eşlemek. Migration, saf SQL script + `schema_version` tablosudur.

**Neden kritik:** EF'siz kalmanın bedeli olan kontrolü getirir: hangi sorgunun çalıştığını biliriz, `CommandTimeout` veririz, şifreli connection string saklarız. `.templates/infrastructure/` altında dört şablon hazır: `dapper-mapping-template.md`, `provider-template.md`, `migration-template.md`, `configuration-template.md`.

**Yanlış anlaşılırsa:** Mikro-ORM tam ORM sanılır, sorgu string'i kod içine gömülür (`$"SELECT …{x}"`) → injection + RED. Doğrusu: her değer parametre, her sorgu şablonludur.

Kullanım: [CLAUDE.md](CLAUDE.md) · [.templates/infrastructure/dapper-mapping-template.md](.templates/infrastructure/dapper-mapping-template.md)

### §5.7 provider abstraction — Sağlayıcı Soyutlaması

**Ne olduğu:** Kodun `Microsoft.Data.Sqlite` veya `Microsoft.Data.SqlClient` **bilmemesi**; yalnız ortak `DbConnection`/arayüz üzerinden konuşması. Seçim yapılandırma (Host) katmanında yapılır.

**Neden kritik:** "Tek sağlayıcıya kilit" yasaktır. Yerel geliştirme SQLite, üretim SQL Server olabilmeli; aynı repository kodu ikisinde de çalışmalıdır. SQL farkları migration script'lerinde, sorgularda değil.

**Yanlış anlaşılırsa:** Kod içine `if (sqlite)` gömülür → her yeni provider için tüm sorgular revise. Soyutlama katmanı yıpranır, iki bacak test edilemez hâle gelir.

Kullanım: [CLAUDE.md](CLAUDE.md) · [.templates/infrastructure/provider-template.md](.templates/infrastructure/provider-template.md)

### §5.8 migration — Saf SQL + Version Tablosu

**Ne olduğu:** Şema değişikliğinin elle yazılmış, numaralı `.sql` dosyalarla yapılması ve `schema_version` tablosunda hangisinin uygulandığının tutulması.

**Neden kritik:** EF'nin migration motoru olmadığı için versiyonlama tamamen görünür ve denetlenebilir. Geri alma (rollback) script'i de yazılır; geri alınamaz migration G11 ile raporlanır.

**Yanlış anlaşılırsa:** "Migrasyonu sonra ekleriz" denirse şema versiyonu bilinmez, test verisi tekrarlanamaz olur. Sıralı, idempotent script zorunludur.

Kullanım: [.templates/infrastructure/migration-template.md](.templates/infrastructure/migration-template.md) · [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md)

### §5.9 DevExpress Universal 26.1 — Tek Kontrol Kütüphanesi

**Ne olduğu:** DevExpress Universal 2026 / **v26.1** — WinForms tarafının tek UI kütüphanesi. Başka sürüm referans edilemez (G3).

**Neden kritik:** Hem lisans hem API tutarlılığı bu tek sürüme bağlıdır. Ribbon, Grid, Docking, TreeList, Editors hepsi aynı sürümden gelir. `.templates/frontend/form-template.md` bu sürüme göre yazılmıştır.

**Yanlış anlaşılırsa:** ai.01.md'deki `DevExpress.Win.All 24.2.3` okunursa tüm denetim API'leri yanlış kalır ve G3 CRITICAL ihlali doğar. Eski sürüm referansı **bilgi olarak bile** vault'a taşınmaz.

Kullanım: [CLAUDE.md](CLAUDE.md) · [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md)

### §5.10 ribbon (Office2010) — Tek Ribbon Stili

**Ne olduğu:** `RibbonControl` + `RibbonControlStyle.Office2010` + ana pencere `RibbonForm`. Diğer stiller (Office2007, Blue vb.) ve `XtraForm` ana pencere yasaktır.

**Neden kritik:** Ürünün görsel kimliği tek karara bağlanmıştır; 7 ekranın ana IDE ekranı bu ribbon üzerine kurulur. Kural [.rules/devexpress-rules.md](.rules/devexpress-rules.md) içinde hard rule olarak yaşar.

**Yanlış anlaşılırsa:** Farklı ekranlarda farklı ribbon stilleri karışır, tema kırılır, G3 ihlali doğar.

Kullanım: [CLAUDE.md](CLAUDE.md) · [keys.md](keys.md)

### §5.11 net48 — Legacy Kanıt Bacağı

**Ne olduğu:** .NET Framework 4.8 hedefi. DevExpress'in tam desteği buradadır; modern C# özelliklerinin bir kısmı (nullable reference types, bazı dil sözdizimleri) sınırlıdır.

**Neden kritik:** "Legacy kanıt" bacağıdır: ürün eski kurumsal ortamda da çalışabilmelidir. Her karar `net48`'de de doğrulanır; test projesi de iki bacakta koşar.

**Yanlış anlaşılırsa:** "net48 sonra bakarız" denirse kod modern sözdizimine kayar, sonra geri alınamaz → G6 HIGH ihlali. Ortak kaynak + `#if` yalnız platform farkı kuralı bunun içindir.

Kullanım: [CLAUDE.md](CLAUDE.md) · [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md)

### §5.12 net10.0-windows — Modern Bacak

**Ne olduğu:** .NET 10 Windows hedefi (`net10.0-windows`); WinForms'un modern, uzun ömürlü sürümü.

**Neden kritik:** Yeni yeteneklerin (runtime API, dil özellikleri, performans) hedefidir. İki bacak **aynı kaynak** kodu paylaşır; ayrışma yalnız platform farklarında `#if` ile yapılır.

**Yanlış anlaşılırsa:** İki ayrı kod tabanı çıkar → her özellik iki kez yazılır, ikisi zamanla farklılaşır, R5/tek bacak riski gerçekleşir.

Kullanım: [CLAUDE.md](CLAUDE.md) · [index.md](index.md)

### §5.13 gate (GATE-000..005) — Onaylı Kapılar

**Ne olduğu:** Fazlar arası geçişin onay mekanizması. Kayıtlı liste `reports/00-kapsam-analizi.md` §4.3'tedir:

| Gate | Ad | Karşılığı |
|---|---|---|
| GATE-000 | Discovery Approval | PART 1–2 |
| GATE-001 | Reference + Research Approval | PART 1 — **✓** |
| GATE-002 | Requirements Freeze | PART 2 — **✓** |
| GATE-003 | **UI Approval** | PART 4 — **bekliyor, kodlama öncesi ZORUNLU** |
| GATE-004 | Architecture Approval | PART 3 + PART 5 |
| GATE-005 | Implementation Plan Approval | PART 5 öncesi |

**Neden kritik:** Teslim promptu GATE-001..006 aralığını anar; kesin ve nihai liste [WORKFLOW.md](WORKFLOW.md) SSOT'udur. Kapıya kadar iş yapılmış sayılır, kapı geçilmeden sonraki faza **girilmez**.

**Yanlış anlaşılırsa:** En tehlikelisi GATE-003 atlanıp kod yazılması: onaysız ekranlar kodda gömülür, revizyon maliyeti 10 katına çıkar.

Kullanım: [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) · [index.md](index.md)

### §5.14 ADR — Mimari Karar Kaydı

**Ne olduğu:** Tek bir kararın `bağlam → seçenekler → karar → sonuç` dört parçasıyla yazıldığı dosya. Vault'ta `.templates/adr/` altında 6 şablon hazır: Nygard, architecture, data, devexpress, security, index.

**Neden kritik:** 5 senior karar, ADR-001..011 envanteri ve tüm teknoloji kısıtları ADR diliyle konuşulur. ADR'siz karar = gerekçesiz karar = bir daha tartışılır.

**Yanlış anlaşılırsa:** Kararlar sohbet geçmişinde kaybolur; yeni oturum aynı soruyu yeniden açar. `MEMORY.md` tutar ama **gerekçe** `brain.md` + ADR tutar.

Kullanım: [.templates/adr/adr-index.md](.templates/adr/adr-index.md) · [brain.md](brain.md)

### §5.15 plan mode — Plan Önce, Kod Sonra

**Ne olduğu:** İşe koda atlamadan önce kapsamın, seçeneklerin ve kriterlerin çıkarılıp kullanıcıya/owner'a sunulması. GATE-003 bu modun kapısıdır.

**Neden kritik:** Şu an proje **(2/8)**'dedir; kodlama GATE-003 (UI onayı) sonrasıdır. Bu yüzden bu oturumda üretilen her şey **doküman**, kod değil.

**Yanlış anlaşılırsa:** "Hızlıca bir form yazayım" → onaysız UI → GATE-003'te revizyon → tüm katmanlar yeniden. Süreç ihlali G12/RED ile baştan sayılır.

Kullanım: [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) · [CLAUDE.md](CLAUDE.md)

### §5.16 checkpoint — Geri Dönüş Noktası

**Ne olduğu:** Bir iş parçasının tamamlandığı, kanıtlandığı ve gerektiğinde oradan devam edilebileceği durum anı. Her part kapanışında rapor + `log.md` kaydı = checkpoint.

**Neden kritik:** Uzun işlerde (500+ MD, 8 part) tek seferde bitirme varsayımı yanlıştır. Checkpoint'li ilerleme, hata halinde kaybı son kapıya indirir ve `bag-of-tasks` paralel dağıtımına izin verir.

**Yanlış anlaşılırsa:** Uzun oturum çöker, tüm iş tekrarlanır. Kanıtsız "bitti" de sahte checkpoint'tir (G5).

Kullanım: [brain.md](brain.md) · [.agents/AGENTS.md](.agents/AGENTS.md)

### §5.17 transcript — Ham Oturum Kaydı

**Ne olduğu:** Oturumun gerçek konuşma metni: prompt, yanıt, araç çağrıları, hatalar. Özet (log) ile **ayrı** tutulur.

**Neden kritik:** "Ne oldu?" sorusunun tek tarafsız kanıtıdır. Kapsam analizinde ai.01/02/03 dosyalarının satır/satır sonu/tarih envanteri transcript mantığının ta kendisidir.

**Yanlış anlaşılırsa:** Yalnız özet kalır → neden o kararın alındığı bulunamaz, uydurma anlatı oluşur (G9).

Kullanım: [MEMORY.md](MEMORY.md) · [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md)

### §5.18 MCP — Dış Araç Bağlama Protokolü

**Ne olduğu:** Model Context Protocol: ajanın harici sunuculara (dosya, veritabanı, web, CI) bağlanmasını standartlaştıran protokol. Vault'ta `spec/mcp-spec.md` (planlı) altında tartışılacaktır.

**Neden kritik:** Ürünün genişleme yüzeyidir: üçüncü parti entegrasyonlar MCP ile gelir. Sandbox, gateway ve yetki sınırları burada tanımlanmazsa ajan sınırsız erişim kazanır.

**Yanlış anlaşılırsa:** Her entegrasyon özel yazılır → her biri ayrı güvenlik yüzeyi. MCP'yi "sadece eklenti" sanmak da ölümcüldür: tool sözleşmesi protocol katmanını besler.

Kullanım: [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) · [keys.md](keys.md)

### §5.19 opencode — Referans Ajanlı IDE

**Ne olduğu:** Bu vault'un üretildiği ajanlı IDE. Kaynak kodu `%TEMP%\opencode-fresh` içinde, v1.18.32, **33 paket**; anahtarları `core` (ajan döngüsü + tool sistemi), `tui`, `server`, `sdk`, `protocol` (mesaj şeması → PART 7 ana girdisi).

**Neden kritik:** PART 7'de VersaCoder'ın kendi protokol/tool modeli bu referans üzerinden kurulur. Sıfırdan icat etmek yerine kanıtlanmış modelin uyarlanması kararlaştırılmıştır.

**Yanlış anlaşılırsa:** "opencode'u kopyalayalım" (G1 ihlali) ya da "hiç bakmayalım" (bilgi kaybı) uçları. Doğrusu: **referans olarak oku, ürünü kendin yaz.**

Kullanım: [reports/00-kapsam-analizi.md](reports/00-kapsam-analizi.md) · [index.md](index.md)

### §5.20 YAML frontmatter — Dosya Kimliği

**Ne olduğu:** Dosyanın ilk satırındaki `---` bloğu: `title`, `type`, `category`, `status`, `version`, `updated`, `authority: SSOT`, `scope`.

**Neden kritik:** Vault'un taranabilirliğini sağlar; link taraması, boyut ölçümü ve "hangi dosya neye sorumlu" sorgusu bu alanlarla çalışır. 8 alan zorunlu, eksik = G4 RED. `WIP`/`~` durumu da yasaktır.

**Yanlış anlaşılırsa:** Dosya bulunsuz kalır → `keys.md` yönlendirmesi boşa düşer, navigasyon çöker.

Kullanım: [.templates/documentation/root-md-template.md](.templates/documentation/root-md-template.md) · [CLAUDE.md](CLAUDE.md)

---

## §6 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] Terim sayısı 118, §3 dizini ile §4 tabloları birebir örtüşüyor
- [ ] 20 temel terim §5'te genişletilmiş
- [ ] Her satırda 6 alan da dolu (Terim · EN · Tanım · Neden önemli · İlgili · Kullanım)
- [ ] Tüm `(…)` bağlantıları diskte var; olmayanlar `` `yol` ``(planlı) olarak düz metin
- [ ] Hiçbir yerde `CR-only` satır sonu yok
- [ ] Unicode kutu çizimi yok — yalnız düz ASCII
- [ ] Karar/kural iddiası yok; her teknoloji hükmü `CLAUDE.md`'ye bağlanmış

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Navigasyon | `.ai/index.md` |
| 2 | Anahtar kelime haritası | `.ai/keys.md` |
| 3 | Anayasa (guardrail G1..G12) | `.ai/CLAUDE.md` |
| 4 | Kararlar ve gerekçeler | `.ai/brain.md` |
| 5 | Oturumlar arası bellek | `.ai/MEMORY.md` |
| 6 | Kapsam analizi (R/B/A + gate) | `.ai/reports/00-kapsam-analizi.md` |
| 7 | Kök belge şablonu | `.ai/.templates/documentation/root-md-template.md` |
| 8 | Şablon envanteri | `.ai/.templates/index.md` |
| 9 | Ajan kaydı | `.ai/.agents/AGENTS.md` |
