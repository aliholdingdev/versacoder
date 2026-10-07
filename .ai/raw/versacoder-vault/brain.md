---
title: "VersaCoder — Karar Beyni (Decisions & Engineering Constraints)"
type: decisions
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Karar prosedürü (7 adım), kriter ağırlıkları, kapalı 5 senior karar, R1–R6 / B1–B4 / A1–A3 işleme kuralı, dur-sor eşikleri, akıl yürütme anti-patternleri ve işletilmiş örnekler"
---

# VersaCoder — Karar Beyni (`brain.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[AGENTS.md]] · [[glossary.md]] · [[MEMORY.md]] · [[engine.md]] · [[reports/00-kapsam-analizi.md]] · [[.agents/AGENTS.md]]

> **Tek cümlelik sorumluluk:** *Bu projede karar nasıl verilir, hangi kararlar kapanmıştır, ne zaman durulup kullanıcıya sorulur* sorusunun tek cevabıdır.
> Bu dosya **gerekçe ve kısıt** taşır; **kural `CLAUDE.md`**, **kapı `WORKFLOW.md`**, **tekil karar metni `.decisions/`** üzerindedir (SRP).

---

## §0 Özet (Answer First)

Kararlar **hiçbir zaman sezgiyle** verilmez: 7 adımlı prosedür (§2) + 4 kriterli ağırlık (§3) uygulanır, sonuç bir **ADR** olarak kaydedilir ve **doğrulama** ile kapanır. Bu projede **5 senior karar kapanmıştır** (§4) ve tekrar açılmaz; **6 risk** (R1–R6), **4 belirsizlik** (B1–B4), **3 varsayım** (A1–A3) aktif olarak izlenir (§5–§6). Üç eşikte karar verilmez, **duruulur ve kullanıcıya sorulur**: belirsizlik, yıkıcı işlem, 3 başarısız düzeltme (§7).

| Gösterge | Değer |
|---|---|
| Kapalı senior karar | **5 / 5** |
| Kriter ağırlığı | doğruluk 40 · sürdürülebilirlik 30 · performans 20 · kolaylık 10 |
| Aktif risk | R1–R6 (6) · kapanış ölçütü tanımlı |
| Aktif belirsizlik | B1–B4 (4) · karar zamanı part bazlı |
| Aktif varsayım | A1–A3 (3) · A1 ve A2 doğrulanmış |
| Zorunlu durma eşiği | 3 (belirsizlik · yıkıcı · 3. başarısız düzeltme) |

---

## §1 Kapsam ve Sorumluluk Ayrımı

| Soru | Tek kaynak | Bu dosyada mı? |
|---|---|---|
| **Hangi kural kesinlikle yasak?** | `CLAUDE.md` (G1..G12) | Hayır — burada yalnız gerekçesi |
| **Hangi kapıda ne onaylanır?** | `WORKFLOW.md` · `engine.md` | Hayır |
| **Nasıl karar verilir, ne zaman sorulur?** | **bu dosya** | **Evet** |
| **Tek bir kararın tam metni?** | `.decisions/accepted/ADR-NNN-*.md` · [.decisions/adr-index.md](.decisions/adr-index.md) | Hayır — burada özet + link |
| **Hangi ajan hangi karara son söz der?** | `AGENTS.md` §3.2 | Hayır — burada yalnız prensip |
| **Risk/belirsizlik envanteri** | `reports/00-kapsam-analizi.md` §3 | Hayır — burada **işleme kuralı** |

> **KURAL:** Bu dosyada bir kural yazıyorsa, `CLAUDE.md`'de karşılığı **olmalıdır**. Karşılığı yoksa bu dosya fazladan kural üretmiş demektir → SSOT ihlali, tekrar satırı sil.

---

## §2 Karar Prosedürü (7 Adım)

**Çerçevele → Seçenekler → Kriterler → Değiş tokuş → Karar ver → ADR olarak kaydet → Doğrula.**

| # | Adım | Ne yapılır | Çıktı | Kırmızı çizgi | Süre |
|---|---|---|---|---|---|
| 1 | **Frame** (çerçevele) | Sorun tek cümleye indirilir; kısıtlar (`net48`+`net10.0-windows`, v26.1, EF Core yasak) baştan yazılır | Problem cümlesi + kısıt listesi | Kısıtsız karar = geçersiz | ≈2 dk |
| 2 | **Options** (seçenekler) | En az **2** gerçek alternatif üretilir; "tek yol yok" varsayımı reddedilir | ≥2 seçenek | 1 seçenek = karar değil, kabul | ≈5 dk |
| 3 | **Criteria** (kriterler) | §3'teki 4 kriter + proje özgü kısıt (gate, güvenlik, iki bacak) uygulanır | Kriter tablosu | Ağırlıklar keyfi değişmez | ≈3 dk |
| 4 | **Tradeoff** (değiş tokuş) | Her seçeneğin (+) ve (−) tarafı yazılır; **kayıp gizlenmez** | Kazanım/kayıp sütunu | Yalnız (+) yazılmışsa seçenek yanlıştır | ≈5 dk |
| 5 | **Decide** (karar) | Tek seçenek seçilir; veto kolları uygulanır: doğruluk · kısıt (G8) · gate (GATE-003) | Karar cümlesi | "İkisini de yaparız" = kapsam sessiz artışı | ≈2 dk |
| 6 | **Record** (kaydet) | ADR şablonuyla `.decisions/` altına yazılır; `log.md`'ye tarihli tek satır düşülür | ADR dosyası + log satırı | Kayıtsız karar = açıktır, tartışılır | ≈10 dk |
| 7 | **Verify** (doğrula) | Karar **çalışan kanıtla** sınanır: `dotnet build`, test, link taraması ya da erken spike | Kanıt çıktısı | Kanıtsız kapanan ADR = `status: proposed` kalır | part'a göre |

**Toplam ≈ 27 dk** (kanıt süresi hariç). Adım 1–5 olmadan 6'ya geçmek **yasaktır**: gerekçesiz ADR, yeniden tartışılır.

### §2.1 Prosedür akışı (ASCII)

```
  +--------------------+
  | 1. FRAME           |  sorun + kisitlar (net48|net10, v26.1, EF YASAK)
  +---------+----------+
            |
            v
  +--------------------+
  | 2. OPTIONS  (>= 2) |  her alternatif gercek ve uygulanabilir
  +---------+----------+
            |
            v
  +--------------------+
  | 3. CRITERIA        |  dogruluk 40 > surdurulebilirlik 30 > performans 20 > kolaylik 10
  +---------+----------+
            |
            v
  +--------------------+
  | 4. TRADEOFF        |  (+) ve (-) taraf acikca yazilir
  +---------+----------+
            |
            v
  +--------------------+     veto var mi?
  | 5. DECIDE          | ------------------ EVET ----> SECENEK 2'ye don
  +---------+----------+
            | HAYIR
            v
  +--------------------+
  | 6. RECORD -> ADR   |  .decisions/accepted/ADR-NNN-<konu>.md + log.md
  +---------+----------+
            |
            v
  +--------------------+     gecti mi?
  | 7. VERIFY (kanit)  | ------------------ HAYIR ----> ADR status: proposed
  +---------+----------+                         + kullaniciya rapor
            | EVET
            v
      ADR: accepted
```

> **UYARI:** Adım 7 atlanıp "mantıklı görünüyor" denirse G5 (kanıtsız tamamlama) ihlali doğar. Karar, **çalışan kanıtla** kapanır.

---

## §3 Kriterler ve Ağırlıklar

### §3.1 Ağırlık tablosu

| # | Kriter | Ağırlık | Soru | Tipik uygulama | Veto? |
|---|---|---|---|---|---|
| 1 | **Doğruluk** (correctness) | **40** | Doğru mu, kanıtlanmış mı? | test yeşil, build 0 hata, parameterized SQL | **Evet** — düşerse karar geçersiz |
| 2 | **Sürdürülebilirlik** (maintainability) | **30** | 6 ay sonra anlaşılır mı? | katman düzeni, SRP, ADR gerekçesi, tekrar yok | Hayır ama 2. sıradır |
| 3 | **Performans** (performance) | **20** | Hızlı ve öngörülebilir mi? | `CommandTimeout`, N+1 yok, streaming akıcı | Hayır |
| 4 | **Kolaylık** (convenience) | **10** | Yazması kolay mı? | az kod, az paket, az yapılandırma | **Hayır — asla 1. sıraya geçemez** |

**Toplam = 100.** Ağırlıklar **projeye özeldir** ve `CLAUDE.md` §4 teknoloji anayasasıyla uyumludur.

### §3.2 Ağırlık uygulama kuralları

| # | Kural | Örnek |
|---|---|---|
| 1 | Doğruluk **her zaman** kazanır; en kolay çözüm bile doğrulukla çatışırsa elenir | string-concat SQL (kolay) → parameterized (doğru) |
| 2 | Kolaylık, **10 puandan fazla** etki edemez; "hızlı olsun diye" katman atlanamaz | UI'dan doğrudan SQL çağırmak |
| 3 | Kriter çatışmasında önce **kısıt** (G6/G8/gate) devreye girer, sonra puan | EF Core yüksek "kolaylık" puanı taşır → kısıt vetosu |
| 4 | **Eşit puan** hâlinde: sürdürülebilirlik kazanır, sonra performans | benzer iki kütüphane seçimi |
| 5 | Puanlama **gerekçeyle** yazılır; sayı tek başına yeterli değildir | ADR'de "(−) taraf" zorunlu |
| 6 | Puan **önceden** yazılır; karar sonrası puan uydurmak yasaktır | sonuç → puan değil, puan → sonuç |

### §3.3 Veto kolları (kriter dışı kesin engeller)

```
VETO-1  dogruluk       : kanit yok / test kirmizi        -> SECENEK ELER
VETO-2  kisit (G8)     : yigin disi teknoloji/paket      -> SECENEK ELER
VETO-3  gate           : GATE-003 onaysiz kodlamaya      -> IS DURUR
VETO-4  guvenlik       : string-concat SQL / acik secret  -> SECENEK ELER
VETO-5  iki bacak (G6) : karar yalniz bir bacakta calisiyor -> SECENEK ELER
```

Veto yiyen seçenek **elendiği için değil, geçersiz olduğu için** elenir; puan tablosuna girmeye bile hakkı yoktur.

---

## §4 Kapalı 5 Senior Karar (Tam Gerekçe)

> Bu 5 karar `reports/00-kapsam-analizi.md` §2'de kaynak çelişkisiyle birlikte belgelenmiştir. **Tekrar açılmaz**; yalnız yeni bir kısıt (ör. R5 spike sonucu) gelirse ADR güncellenir.

### §4.1 Karar tablosu

| # | Karar | Reddedilen alternatif | Asıl gerekçe | (+) | (−) |
|---|---|---|---|---|---|
| **D1** | **`net48` + `net10.0-windows` iki bacak** | Tek modern bacak (`net10`); tek legacy bacak (`net48`) | ai.03 "TARGET FRAMEWORKS" ile birebir aynı; net48 = legacy kanıt, net10 = modern yetenek. Ortak kaynak + `#if` yalnız platform farkı | Tek kod tabanı, iki ortamda kanıt | `#if` disiplini + iki test koşumu |
| **D2** | **Sıfırdan yeniden yazım** | CoreMusic'ten/yedek vault'tan kopyalama | ai.01 BÖLÜM 6 yapısı VersaCoder'a özgü değil; yedek vault'ta tutarsızlık (`.ai.01` 80 KB brain, `.ai.04` 2 KB brain). Kopyala-yapıştır yasağı prompt'ta açık (G1) | Temiz SSOT, tutarlı ses | Daha uzun süre (R4) |
| **D3** | **~500+ MD · kök 30–88 KB** ölçek hedefi | Küçük (1–16 KB) kök dosyalar | coremusic `.ai` = 599 MD, kök 31–94 KB → hedeflenebilir, kanıtlanmış ölçek. Yedek `.ai.04` kök MD'leri 1–16 KB → kalite çıtası düşük | Ölçülebilir hedef, doğrulama kolay | Zaman riski → `.templates/` azaltır |
| **D4** | **DevExpress Universal v26.1 tek sürüm** | `DevExpress.Win.All 24.2.3` (ai.01) | ai.03 §"DevExpress Universal 2026 / v26.1" + `RibbonControlStyle.Office2010`. Sürüm sapması tüm denetim API'lerini bozar (G3) | Tek API yüzeyi, tema tutarlı | net48 uyumluluğu doğrulanmalı (R5) |
| **D5** | **ADO.NET + Dapper · EF Core YOK** | EF Core 9 (ai.01) + LINQ-to-Entities | net48 bacağında EF Core 9 **desteklenmez** → ortak kaynak için tek yol bu. Parameterized zorunlu; migration = saf SQL + `schema_version` | İki bacakta tek ORM davranışı, tam SQL kontrolü | Elle migration + elle mapping |

### §4.2 Gerekçe derinliği

**D1 — İki bacak.** Kararın dayandığı tek gerçek, iki hedef ortamın da zorunlu olması: `net48` DevExpress'in tam destekli klasik platformudur, `net10.0-windows` ise ürünün modern ömrünü taşıyacaktır. Tek bacak seçmek, diğerini otomatik olarak kapsam dışına atar; bu da kullanıcı gereksinimiyle (iki ortamda da çalışma) çatışır. Bu yüzden **kaynak paylaşılır, ayrışma yalnız platform farkındadır** ve `#if` iş mantığında kullanılamaz. Reddedilen "önce net10, net48'e sonra bakarız" yaklaşımı, geri dönüşü olmayan bir sözdizimi kaymasına yol açar (G6 HIGH).

**D2 — Sıfırdan yazım.** Kopyalamanın engeli etik değil, **tekniktir**: yedek vault'taki dosyalar kendi içinde bile tutarsızdır (aynı dosyanın 40 kat boyut farkı), yani kopyalanacak bir "doğru" yoktur. Ayrıca `C:\www\coremusic.net\.ai` farklı bir ürünün vault'udur; oradan içerik taşımak VersaCoder'ın sesini ve kural setini bozar. Yapı referansı alınır, içerik **elle** yazılır (G1).

**D3 — Ölçek.** 30–88 KB bandı keyfi değil, **ölçülmüş** bir referanstır (coremusic kök MD'leri 31–94 KB). Aynı zamanda doğrulanabilir bir hedeftir: `(Get-Item x).Length` ile denetlenir. Alt bant tutmazsa dosya "boş", üst bant tutmazsa dosya "dolu ama SRP ihlalli" sayılır.

**D4 — v26.1.** Sürüm tekliği, DevExpress'in `DesignTime`/`ResourceManager`/`resx` zincirinin bozulmaması için gereklidir. İki sürüm yan yana gelirse form Designer'ı açılmaz hâle gelebilir. Ribbon stili de aynı kararın parçasıdır: yalnız `Office2010`, ana pencere yalnız `RibbonForm`.

**D5 — EF'siz.** Teknik zorunluluk: EF Core 9 `net48`'de çalışmaz. Bunun yanında EF'sizlik ayrıca bir kazançtır: hangi SQL'in gittiği tamamen görünürdür, `CommandTimeout` verilir, şifreli connection string yönetilir, migration script'i denetlenebilir. Bedeli (elle migration + elle mapping) kabullenilmiştir ve `.templates/infrastructure/` altında 4 hazır şablonla önden karşılanmıştır.

### §4.3 Çelişki kaydı

| Kaynak | İddiası | Sonuç |
|---|---|---|
| ai.01.md (BÖLÜM 28.2) | EF Core 9.0.0 | **REDDEDİLDİ** — ai.03 geçersiz kılmıştır |
| ai.01.md (BÖLÜM 28.3) | DevExpress.Win.All 24.2.3 | **REDDEDİLDİ** — ai.03 v26.1 demiştir |
| ai.01.md (BÖLÜM 6) | Eski `.ai` yapısı | **REDDEDİLDİ** — VersaCoder'a özgü değil |
| ai.01.md (BÖLÜM 12) | 8 ana ekran | **KISMEN** — 7 ekranlık liste prompt'tan esas (R1) |
| ai.02.md (Q&A) | 5 senior karar gerekçesi | **KABUL** — D1..D5 kaynağı |
| ai.03.md (bootstrap) | Yığın, gate, hedefler | **KABUL** — birincil kaynak (SOURCE_VERIFIED) |

> **KURAL:** ai.01'den yalnızca **yapıbilgi** (ekran listesi, gate, mimari şema) alınır; **teknoloji sürümü asla alınmaz.**

---

## §5 Risk Kaydı (R1–R6) — İşleme Kuralı

### §5.1 Risk işleme döngüsü

```
  RISKI TESPIT ET
       |
       v
  +------------------+      tetikleyici belirlenmis mi?
  | ID + etki + azaltma| -------------------------------+
  +---------+---------+                                  |
            | HAYIR -> belirsizlik (B) olarak tasi        |
            v                                             |
  SAHIP ATAN (hangi part'ta gozden gececek) <-------------+
            |
            v
  AZALTMAYI UYGULA (onleyici kural / olcum / erken kanit)
            |
            v
  KAPANIS OLCTUSU TUTULDU MU?
       | HAYIR                          | EVET
       v                                v
  RISK ACIK KALIR               risk: KAPANDI
  (MEMORY.md anlik gorsunde)     (log.md'ye tek satir)
```

### §5.2 Risk envanteri ve azaltma

| ID | Risk | Etki | Tetikleyici | Azaltma (ne yapılır) | Sahip | Ne zaman gözden geçirilir | Kapanış ölçütü |
|---|---|---|---|---|---|---|---|
| **R1** | 7 ekran ≠ 16 yedek wireframe isim uyuşmazlığı | **Yüksek** | PART 4 başlarken | 7 ekranı **prompt'tan** üret; 16 WF'yi yalnız bilgi olarak tara; **kullanıcıya sun** | ui-designer | GATE-003 öncesi | Kullanıcı 7 ekranı onayladı |
| **R2** | ai.03.md **CR-only** satır sonu → parse hata | Orta | Kaynak okuma anında | Okuma anında `\r` → `\n` normalizasyonu; **asla** diske CR-only yazma | architect | her okuma | Girdi envanteri satır sayıları tutuyor |
| **R3** | ai.01'de EF Core / DevExpress 24.2 **sızıntısı** | **Yüksek** | Vault'a kural yazılırken | `.rules/` içine "EF Core YOK" + "v26.1 tek sürüm" hard rule; otomatik tarama | architect | PART 3 + PART 8 | EF Core referansı **0**, sürüm yalnız v26.1 |
| **R4** | ~500 MD + 30–88 KB kalitesi → **zaman aşımı** | **Yüksek** | Vault üretimi boyunca | `.templates/` hazır; kalıp **bir kez** yazılır, katmanlara dağıtılır | architect | her part sonu | Part süresi tahminin ±%20 içinde |
| **R5** | **net48 DevExpress v26.1** uyumluluk belirsizliği | **Yüksek** | İlk `dotnet build` | **Erken spike**: PART 5'te ilk derleme ile doğrula; sorun çıkarsa kullanıcıya raporla | developer | PART 5 (erken) | İki bacakta 0 hata ya da açık rapor |
| **R6** | **Bozuk iç link** (500 MD, ~bin link) | Orta | Her MD yazımından sonra | Otomatik link taraması; `planlı` işaretli hedefler link yapılmaz | reviewer | PART 3 + PART 8 | Tarama → **0 bozuk link** |

### §5.3 Risk kuralları

| # | Kural | Yasak |
|---|---|---|
| 1 | Risk **adıyla** (R1..R6) anılır; "bir sorun olabilir" dendiği an kaybolur | İsimsiz risk söylemi |
| 2 | Her riskin **tek sahibi** ve **kapanış ölçütü** vardır | Sahipsiz risk |
| 3 | Kapanış ölçütü **ölçülebilirdir** (sayı, onay, çıktı) | "Dikkat edeceğiz" |
| 4 | Azaltma **önleyicidir**; sorun olduktan sonra yapılan iş azaltma değildir | Reaktif düzeltme |
| 5 | Risk realizese (ör. R5'te build kırıldı) → **derhal kullanıcıya rapor**, sessiz çözüm yok (G11) | Sessiz atlatma |
| 6 | Yeni risk tespit edilen varsa `log.md`'ye **R7..** olarak eklenir | Kayıtsız yeni risk |

---

## §6 Belirsizlik (B1–B4) ve Varsayım (A1–A3)

### §6.1 Belirsizlikler — cevap bekleyen açık sorular

| ID | Açık soru | Neden cevapsız | Karar zamanı | Ne yapılacaksa o güne kadar | Çözümde etkilenen |
|---|---|---|---|---|---|
| **B1** | `VersaCoder.slnx` mi `.sln` mi? | Mevcut çözüm dosyası yok | **PART 5** | Projeler üretilecekmiş gibi klasör yapısını standart tut | build, CPM |
| **B2** | Test framework'ü hangisi? | ai.01 "xunit" diyor, prompt söylemiyor | **PART 5** | Test yazma; yalnız testlenebilirlik tasarla (arayüz + DI) | tests/, DoD |
| **B3** | 7 ekranın DevExpress **kontrol seti** kesinleşmedi | Yedek 16 WF ile çelişki (R1) | **PART 4, onay öncesi** | Wireframe'i kontrol **adaylarıyla** çiz, kesin diye yazma | ui-design/ |
| **B4** | LLM provider öncelik sırası (OpenAI-uyumlu / Ollama) | Prompt sıralamıyor | **PART 7** | Provider'ı **soyut** tasarla (tek arayüz, birden çok uygulama) | Protocol, Host |

**Belirsizlik kuralı:**

| # | Kural | Yasak |
|---|---|---|
| 1 | Belirsizlik **adıyla** (B1..B4) taşınır ve karar zamanı gelene kadar **açık kalır** | Cevap uydurmak |
| 2 | Karar zamanı gelmeden **erkeden** karar vermek de yasaktır (erken kilit) | Erken bağlayıcılık |
| 3 | Belirsizlik **erişim engeli** ise iş durur (`DECISION BLOCKED`); değilse **soyut çizilerek** devam edilir | İkisini karıştırmak |
| 4 | Cevap bulunduğunda: ADR açılır → `MEMORY.md` anlık görüntüsü güncellenir → `log.md`'ye tek satır | Cevabın kaybolması |
| 5 | Belirsizlik, bir part'ın DoD'unu engelliyorsa o part **kapanamaz** | "Sonra bakarız" |

### §6.2 Varsayımlar — doğrulanmış görünen kabuller

| ID | Varsayım | Durum | Doğrulama biçimi | Bozulursa |
|---|---|---|---|---|
| **A1** | Vault kökü = `C:\versacoder\.ai` | ✅ **Kullanıcı onaylı** | Kullanıcı onayı kaydı | Tüm vault yolları + `.templates/index.md` hedefleri |
| **A2** | CoreMusic vault'u salt okunur, yalnız **YAPI** referansı | ✅ **Prompt yasağı (G1)** | Kopyalama yokluk denetimi (diff) | İçerik kopyası RED, dosya silinir |
| **A3** | `.templates/` (24 dosya) mevcut iş olarak korunur, çakışma yok | ⏳ **PART 3 başında doğrula** | Dosya sayısı + frontmatter kontrolü | Şablon üretimi yeniden planlanır (R4 artar) |

**Varsayım kuralı:**

```
VARSAYIM  ->  kanit geldi mi?
                 |
        +--------+--------+
        |                 |
       EVET             HAYIR
        |                 |
        v                 v
  GERCEK OLARAK      DEGERLIDIR KALIR
  ISARETLE           (MEMORY.md anlik gorsunde)
  (log.md)            + karar zamanini bekle
```

| # | Kural | Neden |
|---|---|---|
| 1 | Varsayım **adıyla** yazılır (A1..A3), isimsiz kabul yasak | İsimsiz varsayım sorgulanamaz |
| 2 | Doğrulanana kadar `INFERRED` etiketi taşır | G9 |
| 3 | Doğrulanınca **gerçek** sayılır ve silinmez, işaretlenir | Tarihçe korunur |
| 4 | Varsayım, **kararı** etkiliyorsa karar ADR'de `assumption:` alanı ile bağlanır | Gerekçe izlenebilirliği |
| 5 | Kullanıcı onayı gerektiren varsayım **asla** tek taraflı doğrulanmaz (A1 örneği) | G7 |

---

## §7 Ne Zaman Durulup Kullanıcıya Sorulur

### §7.1 Üç mutlak eşik

| # | Eşik | Tetikleyici | Aksiyon | Etiket |
|---|---|---|---|---|
| **1** | **Belirsizlik** | Karar kaynağı çelişiyor ve kaynak hiyerarşisi de çözmedi | 1 kısa soru, bekle | `DECISION BLOCKED` |
| **2** | **Yıkıcı işlem** | Silme · overwrite · `git reset` · geri alınamaz migration | Önce onay, sonra işlem | `G7` |
| **3** | **3 başarısız düzeltme** | Aynı hataya 3 deneme ve hâlâ kırmızı | **Kod yazmayı bırak**; şüpheli varsayımı **adlandır**, raporla, bekle | `3-STRIKE` |

### §7.2 Genişletilmiş dur-sor tablosu

| # | Durum | Etiket | Ne yazılır | Ne yapılmaz |
|---|---|---|---|---|
| 1 | Kaynaklar çelişiyor, hiyerarşi çözmedi | `DECISION BLOCKED` | Çelişki + 2 seçenek + tek soru | İki sürümü de tutmak |
| 2 | Yıkıcı / geri alınamaz işlem | `G7` | Ne silinecek, ne kaybolacak, geri dönüş var mı | Sessiz `Remove-Item -Recurse` |
| 3 | Aynı hata 3 düzeltmede çözülmedi | `3-STRIKE` | Hata · 3 deneme · **şüpheli varsayım** · kanıt · 2-3 seçenek | 4. denemeyi sorgusuz yapmak |
| 4 | Kapsam değişikliği (ekleme/çıkarma) | `KAPSAM` | Ne ekleniyor, neyi ertelemek demek | Sessiz kapsam artışı |
| 5 | Yeni paket / ORM / framework / sürüm | `G8` | Neden mevcut yığın yetmiyor | "Daha modern" diye eklemek |
| 6 | Gate onayı gereken adım (özellikle GATE-003) | `GATE-OPEN` | Hazır paket + kanıt, sonra bekle | Onay verdiğini **varsaymak** |
| 7 | Kriterler eşit puan verdi ve kısıt da çözümlemedi | `TIE` | İki seçenek + hangi kaybın kabullenileceği | "İkisini de yaparız" |
| 8 | Belirsizlik **erişim engeli** (bilgiye hiç ulaşılamıyor) | `UNKNOWN` | Neyi bilmediğin | Tahminle devam (G9) |
| 9 | Ürün metni / ekran adı / marka | `KULLANICI` | Adaylar | Kendi kararını uygulamak |

### §7.3 Soru biçimi (zorunlu şablon)

```markdown
## SORU: <tek cumlelik karar>

- DURUM      : <nereye geldik, ne yaptik>   (kanit satiri)
- ENGEL      : <neden karar veremiyorum>     (kaynak celistirmesi)
- SUPHLI     : <hangi varsayimin yanlis olabilecegi>  (adlandirilmadan gecersiz)
- SECENEK A  : <+ ve ->  ile>
- SECENEK B  : <+ ve ->  ile>
- ONEERI     : <hangisi neden>   (tek satir)
- SORU       : <kisa, tek soru>
```

> **UYARI:** "Şüpheli varsayım" **adlandırılmadan** rapor geçersizdir. "Bilmiyorum" değil; **hangi varsayımın yanlış olabileceği** yazılır. Örn: *"Varsayım: net48 bacağında DevExpress v26.1 `TileControl` desteğini varsayıyorum — doğrulanmadı (`NEEDS_VALIDATION`)."*

---

## §8 Akıl Yürütme Anti-Patternleri

> Bunlar **kod hataları değil, düşünme hatalarıdır**. Bulundukları an durdurulur; sonucu her zaman gecikmiş/yanlış karardır.

| # | Anti-pattern | Belirti | Sonuç | Önleme |
|---|---|---|---|---|
| **1** | **Özet sarma (recap creep)** | Her turda aynı bilgi yeniden anlatılır, madde patlar, cevap geç gelir | Bütçe taşması, işin kendisi geriye kayar | Bütçe: ≤5 madde, ≤8 dosya; ilerleme `(n/8)` tek satır |
| **2** | **GATE-003 öncesi erken kod** | Wireframe onayı yokken form/.cs yazılmış | Onay değişince kod çöp; part baştan (G12 RED) | `GATE-OPEN` etiketi + **kod alanı yok** yazısı |
| **3** | **Sessiz kapsam artışı** | "Bir de şunu ekleyeyim" — kullanıcıya sorulmadan iş büyür | Zaman aşımı (R4), kapsam ihlali, sahiplik kaybı | Kapsam değişikliği = §7.2 #4 dur-sor |
| **4** | **Kanıtsız "bitti"** | Build/test çıktısı olmadan "tamam", "muhtemelen derlenir" | Sonraki part'te patlayan hata, güven kaybı | G5: yalnız `dotnet build` / `dotnet test` çıktısı |
| **5** | **Sonuçtan sonra puanlama** | Önce seçenek beğenilir, sonra gerekçe üretilir | Örtük önyargı; ADR gerekçesi kof | §3.2 #6: puan önce, karar sonra |
| **6** | **Tek seçenek sunumu** | "Yapmamız gereken X" — alternatif yok | Seçim değil kabul; tradeoff görmezden gelinir | §2 adım 2: ≥2 gerçek seçenek |
| **7** | **Erken kilit** | Karar zamanı gelmeden teknoloji seçilmesi (B1..B4) | Gereksiz kısıt, geri alma maliyeti | §6.1 #2: karar zamanını bekle |
| **8** | **Geç kilit** | Karar zamanı geçmişken hâlâ "araştırıyoruz" | İş durur, seri üretim kırılır | Karar zamanı gelince ADR + verify |
| **9** | **Kısıtı yok sayan puanlama** | Kolaylık 10 puana rağmen EF Core öne çıkar | G8 ihlali, iki bacak kırığı | §3.3 veto kolları |
| **10** | **Çelişkiyi sessiz köprüleme** | İki kaynak farklı diyor, "bence şudur" deyip devam | Yanlış temel, G10 ihlali | `CONFLICT DETECTED` → hiyerarşi → sor |
| **11** | **Riski isimsiz tutma** | "Sorun çıkabilir" denip geçilir | Risk unutulur, gerçekleşir | §5.3 #1: R-numarası ver |
| **12** | **Belirsizliği varsayıma çevirme** | B3 "aslında şu kontrollerdir" diye kapanmış | Yanlış wireframe → revizyon | §6.1: karar zamanına kadar açık kalır |
| **13** | **3. hatadan sonra 4. deneme** | Aynı hata farklı isimle tekrar denenir | Sonsuz döngü, bağlam çöpü | §7.1 #3 `3-STRIKE` |
| **14** | **Kanıta rağmen görüşle çelişme** | "Build geçti ama bence yanlış" | Süreç bıkkınlığı, kanıt değersizleşir | Çatışmada kanıt kazanır (`AGENTS.md` §3.3) |

### §8.1 Anti-pattern erken uyarı sinyalleri

```
SINYAL                                            KOK NEDEN
------------------------------------------------  ------------------------------
Cevap 5 maddeyi asti, ozet uzuyor                 recap creep (1)
"wireframe'i sonra yapariz, once kod"              erken kod (2)
"ufak bir sey ekledim"                             sessiz kapsam (3)
"muhtemelen derlenir / testler gecer"              kanitsiz bitti (4)
Tek alternatifli oneri                             tek secenek (6)
"nasil olsa sonra sorariz"                         gec kilit (8)
"Sorun cikabilir" (ad yok)                         isimsiz risk (11)
Ayni hata 3. kez birebir ayni mesajla               3-STRIKE (13)
```

---

## §9 İşletilmiş Örnek Karar Kayıtları

> Aşağıdaki 3 örnek, §2 prosedürünün **gerçek proje sorularına** uygulanmış hâlidir. ADR formatı: `bağlam → seçenek → kriter → tradeoff → karar → kayıt → doğrulama`.

### §9.1 Örnek 1 — Önce hangi provider? SQLite mı, SqlClient mı?

**Adım 1 — Frame.** Provider abstraction zorunlu (G2), iki sağlayıcı da desteklenmek zorunda. Sorun: *ilk çalışan/ilk test edilen sağlayıcı hangisi?* Kısıt: `net48` + `net10.0-windows`, EF yok, migration saf SQL.

**Adım 2 — Options.**

| Seçenek | Tanım |
|---|---|
| **P-A** | Önce `Microsoft.Data.Sqlite` (yerel, dosya tabanlı) |
| **P-B** | Önce `Microsoft.Data.SqlClient` (sunucu, kurulum gerektirir) |
| **P-C** | İkisini aynı anda, ortak arayüzle birlikte |

**Adım 3 — Criteria (puan, önce yazıldı).**

| Kriter | P-A | P-B | P-C |
|---|---|---|---|
| Doğruluk (40) | Testler bağımsız, deterministik → **yüksek** | Sunucu yoksa test koşulamaz → düşük | Doğru ama gecikir → orta |
| Sürdürülebilirlik (30) | Arayüz zaten zorunlu → **yüksek** | Aynı → yüksek | İki kez yazım riski → orta |
| Performans (20) | Yerel I/O hızlı → orta | Ağ I/O → orta | — → orta |
| Kolaylık (10) | Kurulum yok → **yüksek** | Kurulum gerekir → düşük | En pahalı → düşük |

**Adım 4 — Tradeoff.**

| | P-A (Sqlite önce) | P-B (SqlClient önce) | P-C (ikisi birlikte) |
|---|---|---|---|
| **(+) +** | Sıfır kurulumla test; CI'da çalışır; migration ilk kez gerçek şemada denenir | Üretim yolunu erken sıkar | En kapsamlı |
| **(−) −** | Üretim yolundaki farklar geç görülür | Yerel ortam kurulumu engel; CI bağımlılığı; ilk test turu yavaş | İki iş aynı anda, iki hata kaynağı; R4 riski |

**Adım 5 — Decide (veto kontrolü).** VETO-4 uygun (her ikisi de `DbConnection` üzerinden parameterized) → karar verilebilir. **P-A** seçildi; P-B **iptal değil, sıradadır** (provider abstraction sayesinde sonradan eklenir). "İkisini birden" reddedildi → sessiz kapsam artışı (anti-pattern 3).

**Adım 6 — Record.** `.decisions/accepted/ADR-006-provider-onceligi.md` (planlı) + `log.md` satırı: *"2026-09-25 · provider önceliği = Sqlite → SqlClient · gerekçe: deterministik test + sıfır kurulum · A3 benzeri açık: iki provider da henüz derlenmedi (R5)"*.

**Adım 7 — Verify.** Kriter: `dotnet build` her iki bacakta 0 hata **ve** ilk entegrasyon testi SQLite gerçek `schema` üzerinde yeşil. Bu kanıt gelene kadar ADR `status: proposed` kalır.

**Sonuç:** P-A · açık madde → R5 spike ile birlikte doğrulanacak.

---

### §9.2 Örnek 2 — net48 üzerinde DevExpress v26.1 gerçekten çalışıyor mu? (R5)

**Adım 1 — Frame.** D4 kararı v26.1'i sabitledi; fakat **net48 + DevExpress v26.1 kombinasyonu henüz kanıtlanmadı** (R5, Etki: Yüksek). Sorun: *bunu ne zaman ve nasıl doğrularız?* Kısıt: EF yok, WinForms, iki bacak.

**Adım 2 — Options.**

| Seçenek | Tanım |
|---|---|
| **K-A** | Erken spike: PART 5'te iskelet çözüm + boş `RibbonForm` derle |
| **K-B** | Önce tüm vault'u bitir, kod yazarken doğrularız |
| **K-C** | Sürümü düşür (v24.x) — "garantili" görünen eski sürüme geç |

**Adım 3 — Criteria.**

| Kriter | K-A | K-B | K-C |
|---|---|---|---|
| Doğruluk (40) | Kanıt **en erken** gelir → **yüksek** | Kanıt en geç; geç keşif → düşük | D4'ü çiğner → **VETO-2** |
| Sürdürülebilirlik (30) | Vault planı bozulmaz → **yüksek** | Taslak kod yazılabilir → düşük | Tüm şablonlar yanlış sürüme → düşük |
| Performans (20) | Nötr | Nötr | Nötr |
| Kolaylık (10) | ≈20 dk spike → **yüksek** | Zor değil ama riskli → düşük | En kolay → yüksek |

**Adım 4 — Tradeoff.** K-A `(+)` erken uyarı, `(−)` PART 5'in başına ~20 dk ekler. K-B `(+)` sıralamayı bozmaz, `(−)` R5 gerçekleşirse **tüm** parti geri almak gerekir. K-C `(−)` D4 + G3 ihlali, tüm `.resx`/ribbon kalıpları yeniden yazılır.

**Adım 5 — Decide.** VETO-2 devrede → **K-C elendi** (kısıt veto). K-A ile K-B karşılaştırıldığında ağırlıklar doğruluk (40) ekseninde ayrışır → **K-A** seçildi.

**Adım 6 — Record.** R5 satırı `MEMORY.md` anlık görüntüsünde "aktif" kalır; ADR'ye `risk: R5 · spike_kapanis: iki bacakta 0 hata ya da kullanıcı raporu` alanı yazılır.

**Adım 7 — Verify.** Kriter iki kollu: **(a)** iki bacakta `dotnet build` 0 hata → R5 kapanır; **(b)** hata çıkarsa → `3-STRIKE`/`DECISION BLOCKED` değil, **doğrudan kullanıcı raporu** (R5 azaltmasının yazdığı madde).

**Sonuç:** K-A (erken spike, PART 5 başlangıcı) · K-C kesin yasak.

---

### §9.3 Örnek 3 — 7 ekran mı, 16 wireframe mi? (R1 / B3)

**Adım 1 — Frame.** Prompt **7 ekran** istiyor; yedek vault'ta **16** wireframe (`WF-00..WF-15`), ai.01 BÖLÜM 12'de **8** ana ekran var. Kısıt: kapsam donduruldu (GATE-002), GATE-003 kullanıcı onayı zorunlu.

**Adım 2 — Options.**

| Seçenek | Tanım |
|---|---|
| **E-A** | 7 ekranı prompt'tan üret, 16 WF'yi yalnız bilgi tara |
| **E-B** | 16 wireframe'in tamamını VersaCoder kapsamına al |
| **E-C** | 8 ekranı esas al (ai.01) |

**Adım 3 — Criteria.**

| Kriter | E-A | E-B | E-C |
|---|---|---|---|
| Doğruluk (40) | Birincil kaynak (prompt) → **yüksek** | Kapsam genişler, onaysız → düşük (anti-pattern 3) | ai.01 = eski/yapıbilgi kaynağı → düşük |
| Sürdürülebilirlik (30) | 7 ekran = net sahiplik → **yüksek** | 16 ekran sahiplik dağıtır → düşük | Orta |
| Performans (20) | Nötr | — | — |
| Kolaylık (10) | Tek liste → **yüksek** | Üretim süresi ~2× → çok düşük | Orta |

**Adım 4 — Tradeoff.** E-A `(−)` 16 WF'deki fikirler kapsam dışı kalır (ama **bilgi** olarak okunabilir). E-B `(−)` kapsam artışı + R4 zaman aşımı + onaysız ekran. E-C `(−)` kaynak hiyerarşisinde ai.01, ai.03'ün altında.

**Adım 5 — Decide.** VETO-3 (gate) hatırda: **E-A**; 16 WF yalnız `R1` azaltma girdisi olarak tarama listesi. E-B **kullanıcı onayı** olmadan kapalı (kapsam yetkisi kullanıcıda).

**Adım 6 — Record.** `reports/00-kapsam-analizi.md` §4.1'de çelişki zaten kayıtlı → PART 4 başlangıcında bu kayıt **kullanıcıya sunulur**.

**Adım 7 — Verify.** Kriter: kullanıcı 7 ekranı onayladıysa R1 kapanır; kontrol seti (B3) wireframe üzerinde **aday** olarak yazılır, kesin diye sabitlenmez.

**Sonuç:** E-A · R1 açık · karar zamanı = **PART 4, GATE-003 öncesi**.

---

## §10 Karar Ağacı (ASCII)

```
                        YENI KARAR GEREKI
                                 |
                                 v
                   +-----------------------------+
                   | 1. FRAME                    |
                   |   sorun tek cumle + kisitlar |
                   +-------------+---------------+
                                 |
                                 v
                   +-----------------------------+
                   | kisit karari eleyebiliyor mu?|  (G6/G8/gate/guvenlik)
                   +------+-------------+--------+
                          | EVET        | HAYIR
                          v             v
                  KISIT VETO     +-----------------------------+
                  -> adlandir    | 2. OPTIONS (>= 2 gercek)    |
                     ve elen     +-------------+---------------+
                                               |
                                               v
                                  +-----------------------------+
                                  | KAYNAK CELISKISI VAR MI?    |
                                  +------+-------------+--------+
                                         | EVET        | HAYIR
                                         v             v
                              CONFLICT DETECTED   +------------------+
                              -> ROLE hiyerarsisi | 3. CRITERIA      |
                              -> hala yoksa       | 40/30/20/10      |
                              DECISION BLOCKED    +--------+---------+
                              -> KULLANICIYA SOR            |
                                                            v
                                                +-------------------------+
                                                | 5. DECIDE               |
                                                |  dogruluk kazandi mi?   |
                                                +----+-------------+------+
                                                     | EVET        | HAYIR
                                                     v             v
                                                 KARAR VER    GATE-003 acik mi?
                                                     |             | EVET
                                                     v             v
                                          +----------------+  GATE-OPEN
                                          | 6. RECORD      |  -> KULLANICI
                                          |  ADR + log.md  |     ONAYI BEKLE
                                          +--------+-------+
                                                   |
                                                   v
                                          +------------------+
                                          | 7. VERIFY        |
                                          |  build / test /  |
                                          |  link / olcum    |
                                          +---+----------+---+
                                              | EVET     | HAYIR
                                              v          v
                                          accepted   proposed
                                                     + kullaniciya
                                                       rapor (R / B)

        3 BASARISIZ DUZELTME / YIKICI ISLEM / BELIRSIZLIK
                       -> her asamada DUR + SOR (§7)
```

---

## §11 Karar Hataları — Hata Modları

| # | Hata modu | Belirti | Etki | Düzeltme |
|---|---|---|---|---|
| 1 | Prosedür atlanıp doğrudan kod | ADR yok, gerekçe yok | Yeniden yazım | §2'ye dön, 6. adımı geriye dönük yaz |
| 2 | 1 seçenekle karar | "Yapmamız gereken X" | Seçim gizlenir | ≥2 seçenek üret |
| 3 | Puan karardan sonra | Gerekçe sonradan uydurulmuş | Önyargı | Puanı yeniden yaz, kararı yeniden değerlendir |
| 4 | Veto yok sayıldı | Kolay ama yasak yol seçilmiş | G2/G3/G6/G8 ihlali | §3.3 veto listesini zorla |
| 5 | ADR yazılmadı | Karar yalnız sohbette | Son oturumda tekrar tartışılır | `.decisions/` + `log.md` |
| 6 | Verify atlandı | "Uygun görünüyor" | G5 ihlali | Kanıt satırı ekle, `proposed`a düş |
| 7 | Risk isimsiz kaldı | "Sorun olabilir" | Unutulur | R-numarası + sahip + kapanış ölçütü |
| 8 | Belirsizlik erken kapatıldı | B1 hemen karara bağlandı | Gereksiz kısıt | Karar zamanına kadar açık tut |
| 9 | 3. denemeden sonra devam | Sonsuz düzeltme döngüsü | Bağlam israfı | `3-STRIKE` şablonu |
| 10 | Kapsam sessiz arttı | İş tahmini büyüdü | R4 gerçekleşti | Kullanıcıya kapsam sorusu |

---

## §12 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] §2'de **7 adım** tam: frame · options · criteria · tradeoff · decide · record · verify
- [ ] §3 ağırlıkları **40 / 30 / 20 / 10** ve toplamı 100
- [ ] §4'te **5 senior karar**, her biri gerekçe + (+)/(−) + çelişki kaydı ile
- [ ] §5'te **R1–R6** tamamı: sahip · tetikleyici · azaltma · kapanış ölçütü
- [ ] §6'da **B1–B4** ve **A1–A3** karar zamanları yazılı
- [ ] §7'de **3 mutlak eşik** + soru şablonu var
- [ ] §8'de en az 4 anti-pattern: recap creep · erken kod · sessiz kapsam · kanıtsız "bitti"
- [ ] §9'da **3 işletilmiş örnek**, her biri 7 adım uygulanmış
- [ ] §10 karar ağacı **saf ASCII** (Unicode kutu çizim yok)
- [ ] Tek H1, § numaralı bölümler, bozuk iç link yok
- [ ] Boyut **32–50 KB** bandında
- [ ] Bu dosyada yeni **teknoloji kuralı** yok; her hüküm `CLAUDE.md`'ye bağlı

---

## §13 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |
| 2 | Kapsam analizi (R/B/A + gate envanteri) | `.ai/reports/00-kapsam-analizi.md` |
| 3 | Navigasyon | `.ai/index.md` |
| 4 | Anahtar kelime haritası | `.ai/keys.md` |
| 5 | Ajan protokolü (3-STRIKE, escalation, DoD) | `.ai/AGENTS.md` |
| 6 | Ajan kaydı (yetki matrisi) | `.ai/.agents/AGENTS.md` |
| 7 | ADR şablonu (Nygard) | `.ai/.templates/adr/adr-nygard-template.md` |
| 8 | ADR indeks şablonu | `.ai/.templates/adr/adr-index.md` |
| 9 | Terimler sözlüğü | `.ai/glossary.md` |
| 10 | Oturumlar arası bellek | `.ai/MEMORY.md` |
| 11 | Gate ↔ part orkestrasyonu | `.ai/engine.md` |
| 12 | Kök belge şablonu (boyut bandı) | `.ai/.templates/documentation/root-md-template.md` |
