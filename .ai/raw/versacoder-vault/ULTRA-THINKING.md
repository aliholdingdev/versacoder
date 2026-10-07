---
title: "VersaCoder — Derin Düşünme Protokolü (Ultra-Thinking)"
type: protocol
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Zor problemlerde uygulanan 7 adımlı derin düşünme protokolü: tetikleyiciler, stres-testi, ikinci derece etkiler, uygulamalı örnekler, red-team, çıktı sözleşmesi ve gate etkileşimi"
---

# VersaCoder — Derin Düşünme Protokolü (`ULTRA-THINKING.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[engine.md]] · [[reports/00-kapsam-analizi.md]] · [[.agents/AGENTS.md]] · [[.templates/documentation/root-md-template.md]]

> **Tek cümlelik sorumluluk:** *Bu problem gerçekten zor mu, ne zaman durmalı, nasıl karar vermeli, karar yanlışsa ne kırılır, sonuç kimin onayına gider* sorusunun tek cevabıdır.
> Bu dosya **süreç değil, düşünme yöntemidir**. Part sırası ve gate tanımı ayrı dosyadadır; hard kural metni `CLAUDE.md`'dedir (SRP).

---

## İçindekiler

| Bölüm | Alt Konular |
|---|---|
| §0 Özet | 7 adım tek satırda |
| §1 Ne Zaman Tetiklenir | Belirsizlik, katmanlar arası hata, gate kararı, R1–R6, 3. başarısız düzeltme |
| §2 Protokol Akışı | ASCII diyagram + adım listesi |
| §3 Adım Detayları | Girdi / işlem / çıktı / süre / hata modu |
| §4 Stres-Testi Kontrol Listesi | 24 soru — yanlış olma ihtimalini ölç |
| §5 İkinci Derece Etkiler | Katman, test, doküman, gate, risk dalgası |
| §6 Uygulamalı Yürüyüşler | 3 gerçek senaryo (R1, R5/B3, sağlayıcı sırası) |
| §7 Red-Team | Karara karşı argüman + cevap |
| §8 Çıktı Sözleşmesi | Bir derin- düşünme sonucu ne içermek zorunda |
| §9 Gate Etkileşimi | GATE-003'ten geçme yasağı |
| §10 Anti-Patternler | Yapılmaması gerekenler |
| §11 Doğrulama | Ölçülebilir checklist |
| §12 Referanslar | Kaynak → yol |

---

## §0 Özet (Answer First)

Derin düşünme **7 adımlık** kapalı bir protokoldür: **Yeniden ifade et → Kısıtları çıkar → Seçenekleri üret → Stres-testi → Seç → Doğrula → Kaydet.** Protokol, **tek başına karar vermez**; çıktısı her zaman bir **karar + gerekçe + kanıt + etkilenen dosyalar + doğrulama adımı + ADR göstergesi**dir ve **GATE-003'ün ötesine asla kullanıcı onayı olmadan geçmez.**

> **KURAL:** Derin düşünme sonucu **kanıtsız "bitti" değildir** (G5). Doğrulama adımı çalıştırılmadan sonuç `NEEDS_VALIDATION` olarak kalır.

---

## §1 Ne Zaman Tetiklenir

### §1.1 Aktivasyon Tetikleyicileri

| # | Tetikleyici | Belirti | Protokol zorunlu mu? |
|---|---|---|---|
| 1 | **Belirsizlik (ambiguity)** | İki kaynak farklı diyor, requirements açık değil | Evet |
| 2 | **Katmanlar arası hata (cross-layer bug)** | Hata tek dosyada değil; UI → Application → Infrastructure zincirinde | Evet |
| 3 | **Gate kararı** | Bir gate geçilip geçilemeyeceği, hangi kanıtın yeteceği | Evet |
| 4 | **Risk R1–R6** | R1 (7 ekran ≠ 16 WF), R2 (CR-only), R3 (EF/24.2 sızıntısı), R4 (ölçek), **R5 (net48 + DevExpress v26.1)**, R6 (bozuk link) | Evet |
| 5 | **3. başarısız düzeltme** | Aynı hatada 2 düzeltme denendi, hâlâ kırmızı | **Zorunlu — dur** |
| 6 | **Belirsizlik B1–B4** | slnx/sln, test framework, kontrol seti, LLM sırası | Evet |
| 7 | Yıkıcı / geri alınamaz işlem | Silme, migration, git reset (G7) | Evet + kullanıcı onayı |
| 8 | Kaynak çelişkisi | `CONFLICT DETECTED` yazıldı (G10) | Evet |

### §1.2 Ne Zaman **Ateşlenmez** (Protokol Dışı)

| Durum | Yapılacak |
|---|---|
| Açık kural, tek doğru cevap var (`CLAUDE.md` G3: ribbon stili) | Uygula, protokol yok |
| Biçim düzeltmesi (frontmatter, link) | Düzelt, protokol yok |
| Halihazırda ADR'de kararlaştırılmış konu | ADR'yi uygula |
| Mekanik refactor (rename, format) | `dotnet build` + test kanıtı yeter |

> **UYARI:** Protokol **her işe** uygulanırsa yavaşlık ve karar yorgunluğu doğar (G5'ten kaçış olarak kullanılması yasak).

### §1.3 Üçüncü Başarısız Düzeltme Kuralı

```text
Deneme 1  ->  hata ayni            ->  not al, hipotez degistir
Deneme 2  ->  hata ayni            ->  not al, kapsam genislet
Deneme 3  ->  hata ayni            ->  DUR.  Protokol ZORUNLU.
                                        Artik kod degil, VARSAYIM sorgulanir.
```

> **KURAL:** Aynı semptomda **3. düzeltmeden sonra** çözüm aramak yasaktır. Sorulacak soru: *"Hangi varsayım yanlış olabilir?"* — bkz. §4 stres-testi.

### §1.4 Tetik → İlk 60 Saniye Aksiyonu

Protokole giriş **anında** yapılır; "sonra bakarım" diye beklenen iş, kaynak hiyerarşisi çelişkisine dönüşür.

| # | Tetik | İlk 60 saniyede yapılacak | Yapılmayacak |
|---|---|---|---|
| 1 | İki kaynak çelişiyor | `CONFLICT DETECTED` yaz + hangi basamak kazanır onu işaretle | İkisini de tutmak |
| 2 | Ekran/liste uyuşmazlığı (R1) | Hangisi esas, hangisi bilgi — ayrıştır | İkisini birleştirip kapsam şişirmek |
| 3 | net48'de derleme hatası | Sistematik kategori bul (API / dil / paket) | `#if` ile hatalı kodu gizlemek |
| 4 | DevExpress sürüm/stil hatası | G3'e dön: tek sürüm, tek stil | Alternatif stil denemek |
| 5 | EF Core / `DbContext` hatası | G2: paketi ve referansı tara, kaldır | "Neden çalışmıyor" diye debug etmek |
| 6 | SQL hata / timeout | Önce §4.5: parametre + timeout + indeks | Timeout'u 300 sn yapmak |
| 7 | Test kırmızı, ne olduğu belirsiz | Testi izole et → hangi katman? | Toplu test çalıştırmaya devam |
| 8 | Aynı hata 2. kez | Hipotezi **değiştir**, kapsamı genişlet | Aynı düzeltmeyi tekrar |
| 9 | Aynı hata 3. kez | **DUR** → §1.3 → protokol zorunlu | 4. deneme |
| 10 | Gate kanıtı eksik | Kanıt boşluğunu adlandır (`NEEDS_VALIDATION`) | "Yeterli" demek |
| 11 | Yıkıcı işlem ihtiyacı | G7 → kullanıcı onayı iste | Sessiz `Remove-Item` |
| 12 | Kapsam değişikliği isteği | Seçenek üret + maliyet yaz | Hemen uygulamak |
| 13 | Belirsizlik B1–B4 | Karar **zamanını** yaz (hangi part) | Şimdi karar vermek |
| 14 | Mimari şüphe | NetArchTest/iletişimi kanıtla | Sezgiyle katman değiştirmek |
| 15 | "Muhtemelen geçer" hissi | G5: çıktıyı üret | His ile devam etmek |

---

## §2 Protokol Akışı

### §2.1 ASCII Akış Diyagramı

```text
                 +--------------------------------------------------+
   TETIK         |  G1  YENIDEN IFADE ET (restate)                  |
  ----------->   |  - problemi tek cumlede yaz                       |
                 |  - kimi etkiliyor, kriter ne                      |
                 +-------------------------+------------------------+
                                           |
                       belirsizlik mi? ----+---- evet --> §4 stres-testi
                           |                              |
                          hayir                           |
                           |                              |
                           v                              |
                 +-------------------------+              |
                 |  G2  KISITLARI CIKAR    |<-------------+
                 |  - CLAUDE.md G1..G12     |
                 |  - net48 + net10         |
                 |  - gate durumu           |
                 +-------------------------+
                           |
                           v
                 +-------------------------+
                 |  G3  SECENEKLERI URET   |
                 |  - en az 3 seçenek       |
                 |  - her biri: arti/eksi   |
                 +-------------------------+
                           |
                           v
                 +-------------------------+
                 |  G4  STRES-TESTI        |--- > basarisiz ---> SECENEK ELIMINE
                 |  §4 checklist 24 soru    |                       |
                 +-------------------------+                       |
                           |                                       |
                      hepsi gecti <--------------------------------+
                           |
                           v
                 +-------------------------+
                 |  G5  SEC                |
                 |  - tek secenek + gerekce |
                 |  - neden digerleri yok  |
                 +-------------------------+
                           |
                           v
                 +-------------------------+
                 |  G6  DOGRULA            |--- > basarisiz ---> §1.3 (3. deneme)
                 |  - build / test / link   |                       |
                 |  - kanit yapistir        |                       v
                 +-------------------------+                 YONETICIYE RAPOR
                           |                                 (kullaniciya sor)
                      kanit var
                           |
                           v
                 +-------------------------+
                 |  G7  KAYDET             |
                 |  - cikti sozlesmesi (§8) |
                 |  - ADR ac / log'a dus    |
                 +-------------------------+
                           |
                           v
                 +-------------------------+
                 |  GATE KONTROLU  (§9)    |
                 |  GATE-003 onaysiz = STOP|
                 +-------------------------+
```

### §2.2 Adım Sırası (Tek Satır)

| Adım | Ad | Soru | Çıktı |
|---|---|---|---|
| 1 | **Yeniden ifade** | Problem tek cümlede ne? | Tek cümle problem tanımı |
| 2 | **Kısıtları çıkar** | Hangi kural bu kararı kilitliyor? | Kısıt listesi |
| 3 | **Seçenekleri üret** | En az 3 yol var mı? | Seçenek A/B/C (+/-) |
| 4 | **Stres-testi** | Bu yanlışsa ne olur? | Elenen seçenekler |
| 5 | **Seç** | Neden bu, diğerleri değil? | Tek karar + gerekçe |
| 6 | **Doğrula** | Kanıt ne? | Build/test/link çıktısı |
| 7 | **Kaydet** | Nerede yaşayacak? | §8 çıktı sözleşmesi + ADR |

> **UYARI:** Adımlar **atlanamaz**. 3'ten 5'e geçmek (seçeneksiz seçim) ve 5'ten 7'ye geçmek (doğrulamasız kayıt) protokol ihlalidir.

### §2.3 Giriş Noktası Tablosu (Hangi belirti hangi adıma düşer)

Belirti, protokolün **hangi adımından** girileceğini belirler; her zaman 1'den başlamak zorunda değildir, ama **3. adımdan önce** hiçbir seçenek değerlendirilemez.

| Belirti | Giriş adımı | Gerekçe |
|---|---|---|
| İhtiyaç net değil, iki yorum var | **1 (Yeniden ifade)** | Kriter yazılmadan kısıt çıkmaz |
| Kriter net, ama hangi kurallar geçerli belirsiz | **2 (Kısıtlar)** | Seçenekler kısıtsız üretilirse elenemez |
| Kısıtlar biliniyor, yol bilinmiyor | **3 (Seçenekler)** | Üretim başlar |
| Seçenek var ama güven yok | **4 (Stres-testi)** | Eleme mekanizması |
| Eleme sonrası tek seçenek kaldı | **5 (Seç)** | Kısa devre — ama 6 zorunlu |
| Karar verilmiş ama kanıt yok | **6 (Doğrula)** | G5 — kayıt yapılmaz |
| Kanıt var, kayıt yok (unutulma riski) | **7 (Kaydet)** | SSOT — tek yaşam yeri |
| Çelişki (`CONFLICT DETECTED`) | **1** → 2 → 3 | Önce problem, sonra kısıt |
| Gate kanıtı tartışması | **2** (gate kısıtı) → 4 → 6 | Kriter önce, kanıt sonra |

> **KURAL:** 3. adıma **girişten önce** §2.2'nin 1 ve 2 adımları tamamlanmış olmalıdır. Atlanmış giriş = **seçeneksiz karar** = §10 anti-pattern 1.

---

## §3 Adım Detayları

### §3.1 Adım 1 — Yeniden İfade Et (Restate)

| Alan | İçerik |
|---|---|
| **Girdi** | Belirsiz/dağınık problem, hata çıktısı, çelişen kaynaklar |
| **İşlem** | Problemi **tek cümlede** yaz; kime/neye etkilediğini ve "iyi sonuç" kriterini ekle |
| **Çıktı** | `PROBLEM: <tek cümle>` + `KRITER: <ölçülebilir>` |
| **Süre** | ≤ 2 dk |
| **Hata modu** | Çözümü problem yazmak (çözüm önceden seçilmişse süreç bozulur) |

```text
PROBLEM : 7 ekranlik master liste ile yedek vaulttaki 16 wireframe arasindaki
          isim uyuşmazliği (R1) PART 4'te neyi gecersiz kilar?
KRITER  : PART 4 ciktisi yalnizca 7 ekrani kapsasin, yedek vaulttan gelen
          16 WF bilgi kaynagi olarak kalsin, 0 yeni ekran uydurulsun.
```

### §3.2 Adım 2 — Kısıtları Çıkar (Constrain)

Her problem **en az şu katmanlardan** birine dokunur; ilgili kısıtlar çıkarılır:

| Kısıt kaynağı | Ne getirir | Örnek |
|---|---|---|
| `CLAUDE.md` G1..G12 | Yasak/izin sınırı | G2 → EF Core yok |
| `engine.md` §2.3 | Katman bağımlılık yönü | UI → yalnız Host + Application |
| İki bacak (G6) | Her karar net48 + net10'da sınanır | `#if` yalnız platform farkı |
| Gate durumu (§9) | Hangi onay beklemede | GATE-003 |
| Roller | Kimin son sözü | `.agents/AGENTS.md` §3 |
| Kaynak hiyerarşisi | Çelişkide hangi kaynak kazanır | `CLAUDE.md` §3 |

**Çıktı:** `KISIT: [liste]` — bu liste olmadan üretilen her seçenek **eksiktir**.

### §3.3 Adım 3 — Seçenekleri Üret (Generate Options)

| # | Kural | Detay |
|---|---|---|
| 1 | **En az 3** seçenek | 2 seçenek = çoğunlukla "evet/hayır" gizli varsayım |
| 2 | Her seçenekte **+ / -** | Artı tek başına satış konuşmasıdır |
| 3 | "Yapma / ertele" seçeneği dâhil | Bazen doğru cevap bu |
| 4 | Her seçenek için **maliyet** (yaklaşık süre) | Somut: "≈45 dk" — "biraz" yasak |
| 5 | Kaynak kod/ürün etkisi | Hangi katman değişir |

### §3.4 Adım 4 — Stres-Testi (bkz. §4)

Seçenekler §4'teki **24 sorudan** geçmeyenler elenir. Elenecek seçenek **yoksa** protokol §5'e geçer; **tek** seçenek kaldıysa §5 kısa devre olur ama §6 doğrulama **yine** zorunludur.

### §3.5 Adım 5 — Seç (Pick)

| Alan | Zorunluluk |
|---|---|
| Seçilen seçenek | Tek, açık |
| **Neden bu?** | 1–3 madde, ölçülebilir |
| **Neden diğerleri değil?** | Her elenen için tek cümle |
| Geri alma maliyeti | Düşük/orta/yüksek + nasıl geri alınır |

### §3.6 Adım 6 — Doğrula (Verify)

| Karar tipi | Kanıt |
|---|---|
| Kod/mimari | `dotnet build` (iki bacak) + `dotnet test` (iki bacak) |
| Katman bağımlılığı | NetArchTest yeşil |
| Vault | Link taraması 0 bozuk link |
| UI | Wireframe + kontrol envanteri + GATE-003 onayı |
| Bilgi/statü | `UNKNOWN` / `NEEDS_VALIDATION` etiketi |

> **UYARI:** Kanıt yoksa sonuç **`NEEDS_VALIDATION`** kalır ve §8'de "verification step" alanı **boş bırakılamaz**.

### §3.7 Adım 7 — Kaydet (Record)

Kayıt yeri önceliği:

| Öncelik | Yer | Ne yazılır |
|---|---|---|
| 1 | `.decisions/ADR-NNN-<konu>.md` | Karar + alternatif + sonuç |
| 2 | `engine.md` | Yürütme değişikliği varsa |
| 3 | `log.md` | Tarihli kayıt |
| 4 | `reports/*` | Kanıt çıktısı |

### §3.8 Adım Bütçesi ve Zaman Kutusu

Derin düşünme **sınırsız değildir**; bütçe aşımı = karar yorgunluğu ve G5'ten kaçış.

| Adım | Bütçe | Aşım belirtisi | Aşımda ne yapılır |
|---|---|---|---|
| 1 Yeniden ifade | ≤ 2 dk | Tek cümle yazılamıyor | `DECISION BLOCKED` → kullanıcıya sor |
| 2 Kısıtlar | ≤ 5 dk | Kısıt listesi boş | Kaynak hiyerarşisine dön (§3.2) |
| 3 Seçenekler | ≤ 10 dk | 2 seçenekten fazla çıkamıyor | "Yapma" seçeneğini ekle |
| 4 Stres-testi | ≤ 15 dk | Sorular "bilmiyorum" ile dolu | `NEEDS_VALIDATION` → 6'ya git |
| 5 Seç | ≤ 3 dk | Hâlâ iki seçenek | §7 red-team ile kır |
| 6 Doğrula | ≤ 20 dk | Kanıt üretilemiyor | Sonuç `NEEDS_VALIDATION` kalır |
| 7 Kaydet | ≤ 10 dk | Şablon eksik dolduruluyor | §8.3 hata tablosuna bak |
| **Toplam** | **≤ 65 dk** | — | Süre aşımı raporlanır |

**Ölçülebilir işaretler:**

```text
IYI  : adimlar sirali, her adimda bir cikti var, kanit yapistirilmis
KOTU : adim atlandi, "bence" ile basladi, kanit yok
KOTU : sure belirsiz ("biraz surdurdu"), olcum yok
IDDI : uc deneme, ayni hata, protokol yok   -> s1.3 ihlali
```

---

## §4 Stres-Testi Kontrol Listesi

> Bu liste **bir seçeneği elemek için** vardır. Soruların hepsine cevap verilmeden adım 5'e geçilmez.

### §4.1 Çekirdek 5 Soru (Mutlak)

| # | Soru | "Hayır" cevabının anlamı |
|---|---|---|
| 1 | **Bu yanlışsa ne bozulur?** | Etki bilinmiyorsa seçenek elenir |
| 2 | **net48'de ne kırılır?** | G6 ihlali — elenir |
| 3 | **EF-free ADO.NET düzeni ne olur?** | G2 ihlali — elenir |
| 4 | **SQL parameterized mı?** | CRITICAL — elenir |
| 5 | **GATE kriterlerini karşılar mı?** | Geçersiz kanıt — elenir |

### §4.2 A. Doğruluk / Yanlış Olma İhtimali

| # | Soru | Bakılan yer |
|---|---|---|
| 1 | Bu kararı **destekleyen** tek bir kanıt var mı, yoksa tahmin mi? | Çıktı |
| 2 | Kaynak hiyerarşisinde **hangi basamak** bu kararı belirliyor? | `CLAUDE.md` §3 |
| 3 | İki kaynak **çelişiyor mu**? (`CONFLICT DETECTED`) | Kaynaklar |
| 4 | Bu bir **çıkarım (`INFERRED`) mı**? Öyleyse ADR'de işaretlendi mi? | Çıktı |
| 5 | **Kimse bu kararın yanlış olabileceğini söylemedi mi?** (red-team §7) | §7 |
| 6 | Karar **tahmin edilen değil, ölçülen** bir gerekçeye mi dayanıyor? | §3.6 |

### §4.3 B. İki Bacak (net48 + net10.0-windows)

| # | Soru | Kırılırsa |
|---|---|---|
| 7 | Bu karar **net48**'de de geçerli mi? | G6 |
| 8 | **net10.0-windows**'ta farklı davranıyor mu? | G6 |
| 9 | `#if` **yalnız platform farkı** mı, iş mantığını bölüyor mu? | G6 |
| 10 | Yeni paket **her iki TFM'de** restore ediliyor mu? | NU1202 |
| 11 | Testler **iki bacakta** da mı yeşil? | G5 |
| 12 | DevExpress v26.1 **net48'te** de kullanılabilir mi? (R5) | R5 → §6.2 |

### §4.4 C. EF-Free ADO.NET Düzeni

| # | Soru | Kırılırsa |
|---|---|---|
| 13 | Kodda `Microsoft.EntityFrameworkCore*` / `DbContext` **geçiyor mu**? | G2 CRITICAL |
| 14 | Yeni bir **sorgu/şema değişikliği** saf SQL dosyasına mı gidiyor? | §3.7 migration |
| 15 | `Application` katmanı `SqlConnection` **görüyor mu**? | `engine.md` §2.4 |
| 16 | Migration **`schema_version` + sıralı + idempotent** mi? | §3.7 |
| 17 | Provider **`SqliteProvider`/`SqlClientProvider`** üzerinden mi, `switch` dağılımıyla mı? | Provider soyutlama |

### §4.5 D. SQL Parameterized / Güvenlik

| # | Soru | Kırılırsa |
|---|---|---|
| 18 | **Her** sorgu `@parametre` ile mi? `$"...{x}"` **0** mü? | G2 CRITICAL |
| 19 | Dinamik `ORDER BY` / `IN (...)` **beyaz listeli** mi? | Injection |
| 20 | `CommandTimeout` **yazılı** ve `0` (sınırsız) **kullanılmamış** mı? | `engine.md` §3.5 |
| 21 | Connection string **şifreli** ve logda **maskeli** mi? | `engine.md` §3.6 |

### §4.6 E. Gate / Süreç Kriterleri

| # | Soru | Kırılırsa |
|---|---|---|
| 22 | İlgili gate'in **kanıtı** bu kararla mı karşılanıyor? | GATE RED |
| 23 | **GATE-003** onayı alınmadan UI kodlamasına **geçiliyor mu**? | G12 |
| 24 | Yıkıcı işlem (silme/migration/reset) **onaylı mı**? (G7) | CRITICAL |

### §4.7 Stres-Testi Sonuç Tablosu

| Sonuç | Aksiyon |
|---|---|
| 24/24 geçti | §5'e geç |
| 1–2 "bilinmiyor" | `NEEDS_VALIDATION` işaretle, §3.6'da doğrula |
| Herhangi bir **CRITICAL** (18, 23, 24) | Seçenek **elenir**, yeni seçenek üret |
| 3. başarısız düzeltme | §1.3 → dur → kullanıcıya rapor |

### §4.8 Puanlama ve Kapılar (Gate Criteria)

Stres-testi **sayıya** dökülür; "geçti" hissi değil, **eşik** geçerli olsun diye.

| Puan | Anlam | Aksiyon |
|---|---|---|
| 24/24 **Evet** | Seçenek olgun | §5'e geç |
| 20–23 **Evet**, kalanı `NEEDS_VALIDATION` | Şartlı olgun | §3.6 ile doğrula, sonra geç |
| 17–19 | Zayıf | Seçeneği **geri** gönder (§3.3) |
| ≤ 16 veya herhangi bir **CRITICAL** (soru 18/23/24) | Uygun değil | **ELENDİ** — yeni seçenek üret |
| 3. başarısız düzeltme | Kör nokta | §1.3 → kullanıcıya rapor |

**Kapı (gate) kriteri olarak kullanımı:**

```text
GATE-003 icin : 7 ekran dosyasi + kontrol envanteri + net48 uyumluluk notu
                + s4.6 soru 22 "evet"   ->  SUN, ONAY BEKLE
GATE-004 icin : NetArchTest yeşil (>=6 test) + s4.3 sorulari hepsi "evet"
                + ADR linki               ->  SUN, ONAY BEKLE
GATE-005 icin : dotnet build iki bacak 0 hata + R5 erken sinyali
                + s4.5 sorulari "evet"    ->  SUN, ONAY BEKLE
```

> **UYARI:** Puan **kanıtın yerine geçmez**. Puan iyi, kanıt yoksa sonuç yine `NEEDS_VALIDATION` (G5).

---

## §5 İkinci Derece Etkiler (Ripple)

Bir karar **yalnız** dosyayı değiştirmez; aşağıdaki 6 dalga hesaplanır.

### §5.1 Dalga Tablosu

| # | Dalga | Sorulacak | Örnek (sağlayıcı seçimi) |
|---|---|---|---|
| 1 | **Katman** | Hangi katmanlar değişir? | Infrastructure + Host + Protocol mü? |
| 2 | **Test** | Hangi test kırılır, hangisi eklenmeli? | Temp-DB testleri, NetArchTest |
| 3 | **Doküman** | Hangi `.ai/` dosyası güncellenmeli? | `engine.md` §3, `.rules/data-access-rules.md` |
| 4 | **Gate** | Hangi gate kanıtı etkilenir? | GATE-004 (mimari), GATE-005 (build) |
| 5 | **Risk** | Hangi R/B güncellenmeli? | R5, B4 |
| 6 | **Geri alma** | Nasıl geri alınır? | ADR `superseded` + yeni ADR |

### §5.2 Etki Değerlendirme Tablosu

| Soru | Ölçek | Eşik |
|---|---|---|
| Kaç dosya değişir? | 1–3 / 4–10 / 10+ | 10+ → ADR zorunlu |
| Kaç katman etkilenir? | 1 / 2–3 / 4+ | 4+ → mimari RED |
| Geri alma maliyeti | dakika / saat / gün | gün → kullanıcı onayı (G7) |
| İlerleme veya gerileme? | ± | gerileme → red-team §7 |
| Gate etkisi | yok / kanıt değişir / gate değişir | gate değişimi → süreç dosyası |

### §5.3 Dalga Takip Şablonu

```text
KARAR  : <tek satir>
D1 KATMAN   : <etkilenen katmanlar>        -> engine.md s2.3 kontrol
D2 TEST     : <kirilan/eklenen testler>     -> iki bacakta yeniden kos
D3 DOKUMAN  : <guncellenecek .ai dosyalari> -> frontmatter + link tarama
D4 GATE     : <etkilenen gate kaniti>       -> kanit guncelle
D5 RISK     : <R/B guncelleme>              -> reports/00-kapsam-analizi.md
D6 GERI AL  : <nasil>                       -> ADR superseded
```

> **UYARI:** Dalga hesaplanmadan üretilen karar, **sessiz borç** yaratır: kırık test, bayat doküman, bayat gate kanıtı.

### §5.4 Dalga Örneği (Tek Karar → 6 Dalga)

**Karar:** "Yeni bir `IDataStoreProvider` (SqlClient) ekliyoruz" diyelim.

| Dalga | Ne olur | Somut iş | Yaklaşık |
|---|---|---|---|
| D1 Katman | Infrastructure + Host | Yeni provider sınıfı + DI kaydı; `Application` **değişmez** (§2.3) | ≈2 saat |
| D2 Test | 2 test kırılmaz, 4 test eklenir | Kontrat testi (gerçek sunucu yok) + NetArchTest s4 madde 5 | ≈1.5 saat |
| D3 Doküman | `engine.md` §3.3 tablosu, `.rules/data-access-rules.md` | Satır + provider adı; frontmatter `updated` | ≈15 dk |
| D4 Gate | GATE-004 kanıtı | NetArchTest çıktısı tazelenir | ≈2 dk |
| D5 Risk | Yeni risk doğmaz | Ama B4 (LLM sırası) ile ilişkisi not edilir | ≈5 dk |
| D6 Geri alma | ADR `superseded` | DI kaydını kaldır + provider sınıfını sil | ≈20 dk |

**Toplam:** ≈4 saat 40 dk. **Dalga hesaplanmadan** yapılan tahmin "≈45 dk" olurdu → **10 kat sapma**.

> **DERİN DÜŞÜNME:** Dalga tablosu bir **tahmin aracı değil, taahhüt aracıdır**. D3 ve D6 genellikle atlanır; atlanan D3 = bayat doküman (SSOT ihlali), atlanan D6 = geri alınamaz karar (G7 ihlali).

---

## §6 Uygulamalı Yürüyüşler

### §6.1 Örnek 1 — R1: 7 Ekran (master) ≠ 16 Wireframe (yedek)

| Alan | İçerik |
|---|---|
| **Tetik** | Risk **R1** + belirsizlik **B3** |
| **PROBLEM** | Master prompt **7 ekran** ister; yedek vaultta **16 wireframe** (`WF-00..WF-15`), ai.01 BÖLÜM 12 ise **8 ana ekran** tanımlar. PART 4 hangisini üretecek? |
| **KRITER** | 0 uydurma ekran · 0 eksik master ekran · yedek vault yalnız bilgi · kullanıcı onayı alınmış |

**Kısıtlar (adım 2):**

```text
KISIT-1  G1  : yedek vault YALNIZ bilgi kaynagi (kopyalama yok)
KISIT-2  G12 : GATE-003 onayi alinmadan devam yok
KISIT-3  R1  : 7 ekranlik liste master prompt'tan esas
KISIT-4  B3  : DevExpress kontrol seti kesinlesmedi (PART 4 onayi oncesi)
KISIT-5  G9  : "16 ekrani da yapariz" demek uydurma olurdu (kapsam degisikligi)
```

**Seçenekler (adım 3):**

| Seçenek | + | − | Maliyet |
|---|---|---|---|
| **A. 7 ekranı üret, 16 WF'yi bilgi tara** | Kapsam sabit, R1 kapanır | 9 WF bilgisi kullanılmaz | ≈45 dk |
| B. 16 wireframe'i tamamla | Zengin kapsam | Kapsam şişer, GATE-003 sunumu bulanık, master prompt'a sapma | ≈2 saat |
| C. 7 + seçmeli 9 ekran | Uzlaşma | Kapsam belirsiz, onay karmaşası | ≈1.5 saat + ek onay |

**Stres-testi (§4):** B → KISIT-1/G1 **elenir**. C → KISIT-2/GATE-003 kapsamı bulanıklaşır, **elenir** (tek soruluk onay bozulur). A → 24/24 geçer.

**Karar:** **A** — 7 ekran master prompt'tan üretilir; 16 WF yalnız *bilgi* kaynağıdır; sunumda kullanıcıya tek soru sorulur ("Bu 7 ekran onaylı mı?").

**Doğrulama (adım 6):** `ui-design/` altında **7** dosya, hepsi frontmatter'lı; kontrol envanteri dolu; net48 uyumluluk notu mevcut.

**Kayıt (adım 7):** `reports/00-kapsam-analizi.md` §4.1 zaten kayıtlı → yeni ADR **gerekmez** (kapsam kararı, mimari değil); GATE-003 paketi ile sunulur.

> **DERİN DÜŞÜNME:** R1'in asıl tuzağı "ikisini de yapmak" değil, **hangisinin esas olduğunu kanıtsız varsaymaktır**. Esas = master prompt (kullanıcı gereksinimi, hiyerarşi basamağı 1).

---

### §6.2 Örnek 2 — B3 / R5: net48 + DevExpress v26.1 Uyumluluğu

| Alan | İçerik |
|---|---|
| **Tetik** | Risk **R5** (yüksek) + belirsizlik **B3** |
| **PROBLEM** | DevExpress Universal 2026 / v26.1'in `net48` bacağında desteklenip desteklenmediği **doğrulanmadı**; bu, tüm build'i kilitleyebilir. |
| **KRITER** | İlk `dotnet build` ile erken kanıt; sorun çıkarsa sessiz çözüm yok → RED + kullanıcı onayı |

**Kısıtlar (adım 2):**

```text
KISIT-1  G3   : DevExpress yalniz v26.1 (baska surum secenek degil)
KISIT-2  G6   : "net48 sonra bakariz" yasak
KISIT-3  G8   : "daha modern ORM/alternatif" ile kacis yasak
KISIT-4  G5   : kanitsiz "uyumludur" denemez
KISIT-5  R5   : erken dogrulama zorunlu (PART 5 basi)
```

**Seçenekler (adım 3):**

| Seçenek | + | − | Maliyet |
|---|---|---|---|
| **A. Erken restore+build ile kanıtla, olmazsa RED+ADR** | R5'i zamanında kapatır, grev alanı dar | Test beklemeli | ≈2 dk |
| B. Varsay "uyumludur", sonra bakarız | Anlık hız | G5 ihlali, geç keşif, büyük revizyon | Risk: gün |
| C. `#if` ile uymayan kontrolü atla | Build geçer | G6 ihlali + sessiz davranış farkı | Yanlış yol |
| D. DevExpress sürümünü değiştir | Teknik olarak çözüm olabilir | **G3 ihlali — seçeneğin kendisi yasak** | Elenecek |

**Stres-testi (§4):** B → KISIT-4 (G5) **elenir**. C → KISIT-2 (G6) **elenir**. D → KISIT-1 (G3) **elenir**. A → geçer.

**Karar:** **A** — PART 5'in ilk `dotnet build` adımı R5'in **kanıt aracıdır**; uyumsuzluk çıkarsa mimari RED açılır, seçenekler kullanıcıya sunulur.

**Doğrulama (adım 6):** `dotnet restore` + `dotnet build -c Release` → her iki TFM 0 hata, **çıktı yapıştırılmış**.

**Kayıt (adım 7):** Uyumsuzluk varsa → `.decisions/ADR-NNN-net48-devexpress-v26-1.md` (`status: proposed`) + kullanıcı onayı. Uyumluluk kanıtlanırsa → R5 satırı `reports/00-kapsam-analizi.md` §3.1'de kapatılır (yeni dosya değil, **güncelleme**).

> **UYARI:** Bu örnekte **hiçbir seçenek kod workaround'ı değildir**. Teknik borç ile süreç borcunu ayırmak protokolün temelidir.

---

### §6.3 Örnek 3 — Sağlayıcı Sırası: Önce SQLite mı, SQL Server mı?

| Alan | İçerik |
|---|---|
| **Tetik** | Belirsizlik (hangi `IDataStoreProvider` önce yazılacak) |
| **PROBLEM** | `SqliteProvider` ve `SqlClientProvider` hangi sırayla implemente edilmeli; varsayılan hangisi? |
| **KRITER** | Testler harici DB **olmadan** koşabilmeli; entegrasyon testi temp DB ile kanıt üretebilmeli |

**Kısıtlar (adım 2):**

```text
KISIT-1  G2   : EF migration yok -> saf SQL + schema_version
KISIT-2  engine s3.2 : TEK arayuz IDataStore, iki saglayici
KISIT-3  G5   : entegrasyon testi gercek DB bagimliligi tasiyamaz
KISIT-4  G8   : yeni paket/dogal framework eklenemez
KISIT-5  G6   : iki bacakta da ayni davranis
```

**Seçenekler (adım 3):**

| Seçenek | + | − | Maliyet |
|---|---|---|---|
| **A. Önce SQLite (varsayılan), sonra SqlClient** | Temp DB ile test edilebilir, CI hızlı, migration hemen kanıtlanır | Uzak depo gecikir | ≈½ gün |
| B. Önce SQL Server | Kurumsal yol erken | Test için gerçek DB gerekir, CI yavaş/kırılgan, net48 bağlantı külfeti | ≈1.5 gün |
| C. İkisi paralel | Hızlı görünen | İki test altyapısı, migration iki kez doğrulanır, tekrar | ≈2 gün + borç |

**Stres-testi (§4):** B → KISIT-3 (test bağımsızlığı) zorlanır, **şartlı elenir**. C → §5.1 dalga 2 (test) iki katına çıkar, KISIT-3 riski, **elenir**. A → geçer; §4.4/§4.5 sorularının tamamı "evet".

**Karar:** **A** — `SqliteProvider` önce ve **varsayılan**; `IDataStore` arayüzü ilk gün `SqlClientProvider`'ı taşıyacak şekilde yazılır (arayüz **sonradan değişmez**).

**Doğrulama (adım 6):** Temp DB'de migration testi yeşil (`schema_version` ilerliyor) + `SqlClientProvider` yalnız **kontrat testiyle** (gerçek sunucusuz) kayıt altında.

**Kayıt (adım 7):** `.decisions/ADR-00X-data-store-provider-order.md` — alternatifler B ve C açıkça yazılır; `engine.md` §3.3 güncellenir.

> **DERİN DÜŞÜNME:** Sıra kararı **mimari** değil gibi görünür, ama arayüzü sonradan değiştirmek pahalıdır. Bu yüzden ADR açılır; küçük karar, büyük geri alma maliyeti = ADR.

---

## §7 Red-Team (Karara Karşı Argüman + Cevap)

> **KURAL:** Her derin- düşünme sonucu, kendi **karşı** argümanını ve cevabını üretir. Cevap üretilemiyorsa karar **`NEEDS_VALIDATION`** kalır.

### §7.1 Red-Team Şablonu

```text
KARAR    : <tek satir>
KARSI    : Bu kararin yanlis olma argumani (en guclu hali, adilce)
ETKI     : Dogruysa ne kaybederiz?  (somut: sure, para, kapsam, guvenlik)
CEVAP    : Argumani neden bu karari yikmiyor?  (kanit + kaynak hiyerarsisi)
KALAN    : Cevabin zayif kismi -> NEEDS_VALIDATION + dogrulama adimi
```

### §7.2 Örnek Red-Team — §6.3 (Önce SQLite)

| Alan | İçerik |
|---|---|
| **KARAR** | Önce `SqliteProvider`, varsayılan SQLite |
| **KARSI** | VersaCoder kurumsal bir agentic IDE olacak; üretim senaryosu büyük olasılıkla SQL Server. SQLite ile başlamak, migration sözdizimini SQLite'a kilitler, `schema_version` içerikleri (tip sözdizimi, auto-increment, transaction davranışı) iki motor arasında fark eder ve sonradan **iki migration seti** taşımak gerekir. Ayrıca SQLite'ın eşzamanlı yazma kısıtı, agent oturumlarının paralel yazımıyla çakışabilir. |
| **ETKI** | Gerçek: migration'ların ikiye bölünmesi (≈2–4 gün ek), üretim geçişinde sessiz veri tipi farkları, "works on dev, fails on prod" sınıfı hata |
| **CEVAP** | (1) Kapsam analizinde **her iki sağlayıcı da zaten planlı** (`CLAUDE.md` §4: "Tek sağlayıcıya kilit" yasak) — karar sıradır, kilit **değildir**. (2) `IDataStore` soyutlaması (engine §3.2) SQL sözdizimini **saf dosyalarda** tutmaya zorlar; `V0xx__*.sql` dosyaları sağlayıcıya göre ayrışabilir (aynı `V0NN` numarası, farklı dosya) — bu, tasarımın izin verdiği bir dallanmadır. (3) İlk hedef **kanıt**tır: testlerin harici DB olmadan koşabilmesi (G5) SQLite'sız sağlanamaz. (4) SQLite WAL + tek writer kuyruğu, agent oturumu yazma modeliyle uyumludur; eşzamanlık testi §6.4 determinizm kurallarıyla zaten zorunludur. |
| **KALAN** | Sağlayıcıya özgü SQL dosyası ayrışmasının **ne zaman** başlayacağı net değil → `NEEDS_VALIDATION` → ilk gerçek `SqlClientProvider` entegrasyonunda ADR'ye taşınır. |

### §7.3 Örnek Red-Team — §6.2 (R5 / net48 + v26.1)

| Alan | İçerik |
|---|---|
| **KARAR** | Erken build ile kanıtla, olmazsa RED + kullanıcı onayı |
| **KARSI** | "Önce kanıtla" yaklaşımı riski erteler gibi görünmüyor mu? DevExpress zaten net48'i bırakıyor olabilir; belki baştan `net10.0-windows` UI + net48 kütüphane ayrımı yapılmalı ya da net48 tamamen bırakılmalı. |
| **ETKI** | Gerçek: PART 5 sonunda uyumsuzluk çıkarsa **bütün UI katmanı** yeniden planlanır (≈çok yüksek maliyet) |
| **CEVAP** | (1) G6 "net48 sonra bakarız" yasağını **her iki yönde** de korur — ayrım şimdi "erken karar", sonradan "kaçış" olurdu. (2) net48 **kapsam gereği** (legacy kanıt), keyfi değil; bırakma kararı **kullanıcı kararıdır**, mühendislik kararı değil → `DECISION BLOCKED`. (3) Erken kanıt ≈2 dk'dır; erken kanıt **ertelemek değil**, **ileri çekmektir**. (4) Uyumsuzluk çıkarsa seçenekler ADR'de yazılı olarak sunulur. |
| **KALAN** | Gerçek DevExpress v26.1 TFM matrisi elimizde yok → `SOURCE_VERIFIED` değil, `UNKNOWN` → ilk restore ile **doğrulanacak**. |

### §7.4 Red-Team Sıklığı

| Karar tipi | Red-team zorunlu mu? |
|---|---|
| Mimari / katman değişikliği | **Evet**, her zaman |
| Teknoloji/pakat seçimi | **Evet** |
| Risk R1–R6 ile ilgili | **Evet** |
| Küçük, geri alınabilir kod içi | Hayır (build + test yeter) |
| Kapsam/gate kararı | **Evet** + kullanıcı onayı |

### §7.5 Red-Team Sual Seti (En Güçlü Karşı Argümanı Kurmak)

Karşı argüman **zayıf kurulursa** test işe yaramaz. Her karar için şu 8 sual sorulur ve **en zayıf cevap** metne yazılır:

| # | Sual | Ne arıyor |
|---|---|---|
| 1 | **Bu kararı 6 ay sonra geri almak ne kadara mal olur?** | Geri alma maliyeti (§3.5) |
| 2 | **Karar hangi varsayımı besliyor; o varsayım yanlışsa ne olur?** | Kör nokta |
| 3 | **Rakip/alternatif çözüm gerçekten denendi mi, yoksa konfor mu?** | Seçim önyargısı |
| 4 | **Beklenmeyen ikinci derece etki hangisi (§5.1 dalgaları)?** | Sessiz borç |
| 5 | **Kim bundan rahatsız olur — reviewer mı, tester mı, kullanıcı mı?** | Paydaş çatışması |
| 6 | **Bu karar hangi gate kanıtını değiştiriyor?** | Süreç etkisi |
| 7 | **"Bilmiyorum" dediğimiz kısım neresi?** | `NEEDS_VALIDATION` alanı |
| 8 | **Karar tek başına doğru, ama sırası yanlış olabilir mi?** | Zamanlama hatası |

**Red-team çıktısı zorunlu üçlüsü:**

```text
KARSI  : <arguman, adilce ve siddetli>
CEVAP  : <kanit + kaynak hiyerarsisindeki basamak>
KALAN  : <cevabin zayif kismi>  ->  NEEDS_VALIDATION + dogrulama adimi
```

> **UYARI:** `KALAN` alanı **boş bırakılamaz**. "Tamamen ikna oldum" ifadesi red-team'in işlevini yok eder ve gizli varsayım bırakır.

---

## §8 Çıktı Sözleşmesi (Output Contract)

> Bir derin- düşünme sonucu **aşağıdaki 6 alanı içermiyorsa eksiktir** ve `DECISION BLOCKED` sayılır.

### §8.1 Zorunlu Alanlar

| # | Alan | Zorunlu içerik | Boş bırakılabilir mi? |
|---|---|---|---|
| 1 | **Karar (decision)** | Tek, açık, tek cümle + gerekçe (1–3 madde) | HAYIR |
| 2 | **Gerekçe (rationale)** | Neden bu, neden diğerleri değil | HAYIR |
| 3 | **Kanıt (evidence)** | build/test/link çıktısı ya da doğrulanmamışsa `NEEDS_VALIDATION` | HAYIR (etiket zorunlu) |
| 4 | **Etkilenen dosyalar (affected files)** | Yol listesi (`.ai/**` ve/veya `src/**`) | HAYIR |
| 5 | **Doğrulama adımı (verification step)** | Somut, tekrarlanabilir komut/kontrol | HAYIR |
| 6 | **ADR göstergesi (ADR pointer)** | `ADR-NNN-<konu>` **veya** "ADR gerekmez + nedeni" | HAYIR |

### §8.2 Şablon

```markdown
### DERIN DUSUNME SONUCU  <konu>  (<tarih>)

KARAR     : <tek cumle>
GEREKCE   : 1. <olculbilir gerekce>
           2. <olculbilir gerekce>
KANIT     : <dotnet build / dotnet test / link tarama ciktisi>
            ya da  NEEDS_VALIDATION -> <neden>
ETKILENEN : .ai/<yol>            (dosya)
            src/<yol>             (dosya)
DOGRULAMA : <tekrarlanabilir komut / kontrol adimi>
ADR       : .decisions/ADR-NNN-<konu>.md   |  ADR GEREKMEZ: <neden>
RED-TEAM  : <karsi arguman ozeti>  ->  CEVAP: <ozet>
GATE      : <hangi gate'e kanit>  |  ONAY DURUMU: <bekliyor/onayli>
```

### §8.3 Alan Bazlı Hata Tablosu

| Hata | Sonuç | Düzeltme |
|---|---|---|
| Kanıt alanı boş | G5 ihlali | Çalıştır ve yapıştır |
| Etkilenen dosya yok | İkinci derece dalga atlanmış | §5.1'i uygula |
| ADR alanı "—" yazıldı | Karar kayıt dışı kalır | ADR aç veya gerekçesini yaz |
| Gerekçe "daha iyi" | Ölçülemez | Sayı/süre/kriter ekle |
| Red-team yok | §7 zorunlu madde eksik | Karşı argümanı yaz |
| Doğrulama adımı yok | Tekrarlanamaz | Komut yaz |

### §8.4 Dolu Örnek Kayıt (Şablonun Uygulanmış Hâli)

```markdown
### DERIN DUSUNME SONUCU  veri saglayici sirasi  (2026-09-25)

KARAR     : IDataStoreProvider ilk olarak SqliteProvider ile uygulanir,
            varsayilan DataStoreKind.Sqlite olur; SqlClientProvider
            kontrat testiyle kayit altinda tutulur.
GEREKCE   : 1. Entegrasyon testleri harici DB olmadan kosmali (G5) -> yalniz
            SQLite temp DB bunu saglar.
           2. Arayuz ilk gun SqlClient'i tasiyacak sekilde yazildigi icin
            sira, sozlesmeyi sonradan kirmez (geri alma maliyeti dusuk).
           3. "Iki saglayici birden paralel" test altyapisini iki katina
            cikarir -> sessiz borc (s5.1 D2).
KANIT     : dotnet build -c Release   -> 0 hata (net48, net10.0-windows)
            dotnet test --framework net48        -> X passed, 0 failed
            dotnet test --framework net10.0-windows -> X passed, 0 failed
            migration testi: schema_version 1->3 ilerledi (temp DB)
ETKILENEN : .ai/engine.md                      (s3.3 provider tablosu)
            .ai/.rules/data-access-rules.md      (PART 3)
            src/VersaCoder.Infrastructure/Data/SqliteProvider.cs
            src/VersaCoder.Infrastructure/Data/SqlClientProvider.cs
            tests/VersaCoder.Infrastructure.Tests/ProviderContractTests.cs
DOGRULAMA : dotnet test --filter "FullyQualifiedName~ProviderContract" 
            + NetArchTest "Application has no ADO.NET dependency" yeşil
ADR       : .decisions/ADR-010-data-store-provider-order.md
RED-TEAM  : KARSI: SQLite'ya kilit migration'i ikiye boler, eszamanli yazma
            agent oturumlariyla cakisir.
            CEVAP: karar siradir, kilit degildir; iki saglayici zaten kapsamda
            (CLAUDE.md s4 "tek saglayiciya kilit" yasak); saf SQL dosyalari
            saglayiciya gore ayrismalidir.
            KALAN: ayrismanin hangi V0NN'den baslayacagi -> NEEDS_VALIDATION
            -> ilk gercek SqlClient entegrasyonunda ADR'ye tasinir.
GATE      : GATE-004 (Architecture Approval) kaniti
            ONAY DURUMU: beklemede
```

> **KURAL:** Bu kaydın **bir alanı dahi "—" ise** sonuç `DECISION BLOCKED` sayılır; §9'a hiç geçilemez.

---

## §9 Gate Etkileşimi

### §9.1 Temel Kural

> **UYARI:** Derin düşünme sonucu, **GATE-003'ün ötesine kullanıcı onayı olmadan geçemez.** Onay beklemede olduğu bilinmeden verilen "geçti" kararı geçersizdir ve `CLAUDE.md` G12 ile RED'dir.

### §9.2 Gate ↔ Protokol Eşlemesi

| Gate | Ne kararı bu protokol verir | Ne **veremez** |
|---|---|---|
| GATE-001 (Reference + Research) | Kaynak hiyerarşisi + `CONFLICT DETECTED` işleme | Kaynak yeterliliğini tek başına ilan etmek |
| GATE-002 (Requirements Freeze) | Kapsam çelişkisi çözümü (ör. §6.1) | Yeni kapsam eklemek (kullanıcı onayı gerekir) |
| **GATE-003 (UI Approval)** | Sunum paketi + kontrol envanteri + net48 notu | **Onay vermek** — bu yalnız kullanıcıya aittir |
| GATE-004 (Architecture Approval) | Katman kararı + NetArchTest kanıtı + ADR | Mimariyi onaylamak (architect + kullanıcı) |
| GATE-005 (Implementation Plan) | Build kanıtı + R5 erken sinyali | Riski kapatmak (kanıt yoksa açık kalır) |

### §9.3 Gate Dışı Karar Matrisi

| Karar | Sahibi | Protokolün rolü |
|---|---|---|
| UI onayı | **Kullanıcı** | Yalnız sunum + kanıt hazırlar |
| Kapsam değişikliği | **Kullanıcı** | Seçenek üretir, eler |
| Teknoloji sürümü (v26.1) | `CLAUDE.md` G3 (değişmez) | Uygular, sorgulamaz |
| Katman bağımlılığı | architect | Kanıtlar (NetArchTest) |
| Kod içi implementasyon | developer | Stres-testi + doğrulama |
| Test yeterliliği | tester | Coverage kanıtı |

```text
  DERIN DUSUNME SONUCU
          |
          +--> gate disi karar  ->  uygula + §8 kaydet
          |
          +--> gate kaniti      ->  gate dosyasina ekle, ONAY BEKLE
          |
          +--> GATE-003         ->  SUN + BEKLE   <--  DUR NOKTASI
                                      |
                              kullanici ONAYLADI
                                      |
                                      v
                              §8 kaydi + devam
```

> **KURAL:** "Onay verildi" **yalnız** kullanıcı açıkça yazdığında söylenebilir. Varsayılan = **beklemede**.

### §9.4 Gate Durum Sözlüğü

Tek kelime, tek anlam — "geçti" muğlaklığı yasak.

| Durum | Anlam | Kim yazar | Sıradaki adım |
|---|---|---|---|
| `BEKLEMEDE` | Kanıt hazır, onay bekleniyor | Üreten ajan | Kullanıcıya sun |
| `ONAYLI` | Kullanıcı açıkça onayladı | **Kullanıcı** | Devam |
| `RED` | Kanıt yetersiz / karar yanlış | reviewer / kullanıcı | Düzelt → yeniden sun |
| `BLOKE` | `DECISION BLOCKED` — çelişki çözülmedi | herkes | Kullanıcıya sor |
| `ATLANDI` | İhlal tespit edildi (G12) | reviewer | Partı başa al |
| `KAPANDI` | Kanıt arşivlendi, gate artık canlı değil | rapor sahibi | Kapanış raporu |

**Bağlaç kuralları:**

1. Bir gate `ONAYLI` olmadan **sonraki gate'e** geçilemez (sıra zinciri).
2. `BEKLEMEDE` → `ONAYLI` **yalnız** kullanıcıdan gelir.
3. `RED` → aynı kanıtla yeniden sunulamaz; kanıt **değişmelidir**.
4. `ATLANDI` tespiti herhangi bir ajan tarafından yazılır ve **partı başa** alır.
5. Durum **her gate dosyasında** tarihli olarak tutulur; sözlü onay sayılmaz.

```text
  BEKLEMEDE  --(kullanici ONAYLADI)-->  ONAYLI  -->  sonraki gate
      ^                                     |
      |                                     +--> (karsi arguman) --> RED
      +----------- kanit guncellendi <------+
      
  BLOKE  (concelki)  -->  kullaniciya sor  -->  cozulunce BEKLEMEDE
  ATLANDI (ihlal)    -->  part BASA       -->  yeniden uretim
```

---

## §10 Anti-Patternler

| # | Anti-pattern | Sonuç | Doğrusu |
|---|---|---|---|
| 1 | Adım atlayıp seçeneğiz karar vermek | Gizli varsayım | §2.2 sırası |
| 2 | Stres-testini "geçti" diye işaretlemek | Sahte güven | Her soru için kanıt/not |
| 3 | Kanıtsız "bitti" | G5 RED | §3.6 çıktısı |
| 4 | Çıktı sözlesmesini kısaltmak | Karar kayıt dışı | §8 altı alan zorunlu |
| 5 | Red-team'i yazmamak | Kendi kararını onaylamak | §7 |
| 6 | Protokolü her işe uygulamak | Karar yorgunluğu | §1.2 |
| 7 | 3. hatada yeni çözüm denemeye devam etmek | Sonsuz döngü | §1.3 → dur |
| 8 | İkinci derece dalgayı hesaplamamak | Sessiz borç | §5.1 |
| 9 | GATE-003'ü "zaten belli" diye geçmek | G12 RED | §9.3 dur noktası |
| 10 | `DECISION BLOCKED` yazmamak (çelişki varken) | Tahminle köprüleme | `CLAUDE.md` G10 |
| 11 | Uydurma dosya/API/sürüm eklemek | G9 RED | `UNKNOWN` / `NEEDS_VALIDATION` |
| 12 | Sonucu başka dosyaya kopyalamak | SSUT ihlali | Tek kayıt yeri (§3.7) |

---

## §11 Doğrulama

- [ ] §1.1'deki 8 tetikleyici tek tek tanımlı
- [ ] §1.3 "3. başarısız düzeltme → dur" kuralı yazılı
- [ ] §2.2'de 7 adım sırayla, atlanamaz
- [ ] ASCII akış diyagramı **yalnız ASCII** (kutu çizimi yok)
- [ ] §4'te en az **24** stres sorusu; 5 çekirdek soru başlıkta
- [ ] §4.3 iki bacak, §4.4 EF-free ADO.NET, §4.5 parameterized, §4.6 gate soruları var
- [ ] §5'te 6 dalga + eşik tablosu
- [ ] §6'da **3** gerçek yürüyüş (R1, R5/B3, sağlayıcı sırası)
- [ ] §7'de her örnek için **karşı argüman + cevap + kalan**
- [ ] §8'de 6 zorunlu alan + şablon + hata tablosu
- [ ] §9.1'de **GATE-003 kullanıcı onayı** zorunlu
- [ ] §10'da ≥10 anti-pattern
- [ ] Bu dosyadaki tüm `wiki-link` hedefleri diskte var
- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] Dosya boyutu **40–60 KB** bandında

---

## §12 Referanslar

| # | Kaynak | Yol / etiket |
|---|---|---|
| 1 | Navigasyon | `.ai/index.md` |
| 2 | Anahtar kelime haritası | `.ai/keys.md` |
| 3 | Guardrail G1..G12 + kaynak hiyerarşisi + `CONFLICT DETECTED`/`DECISION BLOCKED` | `.ai/CLAUDE.md` |
| 4 | Yürütme motoru (katman, veri, UI, build, test, gate kanıtları) | `.ai/engine.md` |
| 5 | Kapsam analizi — R1–R6, B1–B4, gate envanteri | `.ai/reports/00-kapsam-analizi.md` |
| 6 | Ajan kaydı, yetki matrisi (kimin son sözü) | `.ai/.agents/AGENTS.md` |
| 7 | Karar kuralı + ADR zorunlu alanları | `.ai/.agents/architect.md` §3 |
| 8 | Test kanıt standardı + coverage hedefi | `.ai/.agents/tester.md` §7 |
| 9 | GATE-003 sunum paketi | `.ai/.agents/ui-designer.md` §6 |
| 10 | Kök MD şablonu (iskelet + KB bandı) | `.ai/.templates/documentation/root-md-template.md` |
| 11 | Yürütme referansı | `.ai/engine.md` §11 (bu dosyayla çelişirse **bu dosya değil**, `engine.md` yürütme tarafında, `CLAUDE.md` kural tarafında kazanır) |
| 12 | Bootstrap (karar kaynağı) | `G:\…\versacoder proje planaması ai.03.md` — `SOURCE_VERIFIED` |

**Doğrulama etiketleri (bu dosya için):** tetikleyici/örnek listeleri → `SOURCE_VERIFIED` (`reports/00-kapsam-analizi.md` §3, `.agents/*`) · protokol adımları, stres soruları, çıktı sözleşmesi → `INFERRED` (tasarım önerisi; ilk gerçek kullanımda ADR ile sabitlenecek) · red-team argümanları → `INFERRED` (adil karşı-argüman üretimi, kanıt **değil**).
