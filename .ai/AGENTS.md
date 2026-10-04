---
title: AGENTS - Alt-ajan Kullanım Politikası (dağıtım, çıktı kutusu, eşzamanlılık)
type: policy
category: ".ai/2.beyin"
status: active
version: 2.0.0
updated: 2026-09-29
authority: SSOT
scope: [".ai/2.beyin"]
---

# AGENTS - Alt-ajan Kullanım Politikası

> **Tek cümlelik sorumluluk:** İstenen iş tipini doğru ajana dağıtır, çıktı kutusunu
> ve kanıt zorunluluğunu dayatır, aynı anda en fazla 3 ajan çalıştırır.

## §0 Özet (Answer First)

- **§1 dağıtım matrisi** — mimari→`architect`, kod→`developer`, inceleme→`reviewer`,
  test→`tester`, güvenlik→`security-auditor`, UI→`ui-designer`; matris dışı = `general`.
- **§2 çıktı kutusu** — her ajan `BAŞLIK / KAPSAM / KARAR / RİSK / KANIT` kutusuyla
  döner; kutusuz çıktı kaba metindir ve işlenmez.
- **§3 devre paketi** — devirde 4 madde zorunlu (`KALAN İŞ / DOSYA / KANIT / SINIR`);
  eksik devir iş emri değil, taslaktır.
- **§4 3-STRIKE + bloke raporu** — aynı iş 3 kez düşerse dur; rapor şablonu
  `ETIKET · KONUM · NEDEN · VARSAYIM · SECENEKLER · SORU`.
- **§5 eşzamanlılık** — en fazla **3** paralel ajan (rate limit); paylaşılan dosyada
  ezilme varsa önce seriye al.

Kaynak: [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) — 8 makro
katman, 9 üretim + 1-2 test proje. Ajanların model/ID listesi `DOĞRULAMA GEREKLIDIR`.

## §1 İş tipi → ajan matrisi

| İş tipi | Ajan | Çıktı formatı | Kanıt |
|---|---|---|---|
| Mimari / katman kararı | `architect` | Karar + §2 kutusu | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) + [layer-boundary-check](checklists/layer-boundary-check.md) 13 soru |
| Kod yazımı / refactor | `developer` | Diff + §2 kutusu | Build + test kırmızı→yeşil |
| İnceleme / review | `reviewer` | §2 kutusu (RİSK satırı dolu) | Suçlu dosya + satır no |
| Test / kapsam | `tester` | Test dosyası + §2 kutusu | `dotnet test` çıktısı |
| Güvenlik | `security-auditor` | Bulgu listesi + §2 kutusu | OWASP kalemi + severity |
| UI / ekran | `ui-designer` | [glossary](glossary.md) terimli tasarım + §2 kutusu | 7 ekrandan birinin adı (`01-start-page` .. `07-settings`) |
| Tanım dışı (hepsi dışında) | `general` (varsayılan) | Serbest + §2 kutusu | Kendi sözü değil, kanıta dayalı |

**Tek satırlık kural:** istek matriste yoksa `general`; varsa atanmış ajana gider —
`general` her işi yapabilir ama matristeki uzman işi yapamaz (yanlış uzmanlık riski).

**Yönlendirme örnekleri:**

```text
"Bu katman ihlali değil mi?"   -> reviewer (inceleme tipi)
"l1'e servis ekle"             -> developer; mimari etki varsa önce architect
"CSRF bulgularını geç"         -> security-auditor
"Ayarlar ekranını tasarla"     -> ui-designer -> 07-settings
```

**Zincir kuralı:** architect → developer → reviewer → tester sırası,
[WORKFLOW](WORKFLOW.md) §4'teki A→B→C akışıyla aynıdır; atlanan halkada kabul
kriteri kanıtsız kalır.

**Yanlış ajan maliyeti:** uzmanlık dışı ajan = yanlış kanıt; yanlış kanıt = gate RED
ve iş iki kez yapılır. Bu, 3-STRIKE'dan (§4) daha pahalıdır — dağıtım hatası gate'e
kadar gizlenir, geç fark edilir.

### §1.1 Ajan profilleri — tipik görev · kanıt · gerekçe · `general` eşiği

Matris (§1) işin **kime** gideceğini söyler; bu alt bölüm **neden** o ajana
gittiğini ve işin ne zaman `general`'a düştüğünü yazar. Her ajan için tek blok:
3 tipik görev, ürettiği kanıt, uzmanlık dışı atamanın maliyeti, `general` eşiği.

```text
+-- ARCHITECT ---------------------------------------------------------------+
| TIPIK 3 GOREV  : (1) "l4 -> l2 bagi acilsin mi?" onayi                     |
|                  (2) yeni .csproj kapisi (sorumluluk / yon / TFM)          |
|                  (3) ADR taslagi: karar + alternatif + sonuc               |
| URETTIGI KANIT : karar tablosu +                                           |
|                  checklists/layer-boundary-check.md 13 soru cevabi         |
| NEDEN BU AJAN  : mimari hata geri alinamaz; acilan bir ok silinse bile     |
|                  "neden acildi" kaynagi vault'ta kalmaz                    |
| MALIYET        : 2x is - kod yazilir, sonra silinir; gate RED gec          |
| GENERAL'A DUSER: "nasil olmali" sorusu yoksa ve is yalnizca arastirma /    |
|                  dokuman ozeti ise (matriste karsiligi yok)                |
+----------------------------------------------------------------------------+
```

```text
+-- DEVELOPER ---------------------------------------------------------------+
| TIPIK 3 GOREV  : (1) use-case metodu yazmak / refactor                     |
|                  (2) testi kirmizi->yesil cevirmek                         |
|                  (3) ADR-028'de atanmis katmana dosya eklemek              |
| URETTIGI KANIT : diff + `dotnet build` ciktisi + `dotnet test` sonucu      |
| NEDEN BU AJAN  : degisikligin tek yazari olur; baska ajan ayni dosyaya     |
|                  yazarsa ezme (AGENTS §7) olusur                           |
| MALIYET        : architect atarsa karar kagitda kalir, kod hala yok        |
| GENERAL'A DUSER: is "kod yaz" ama matristeki uzman disi bir uygulama      |
|                  ise (ornek: toplu dosya tasima, betik kurulumu)           |
+----------------------------------------------------------------------------+
```

```text
+-- REVIEWER ----------------------------------------------------------------+
| TIPIK 3 GOREV  : (1) mimari / katman ihlali suphesi                        |
|                  (2) pull request benzeri inceleme (diff uzeri)            |
|                  (3) "bu kod SOLID'e aykiri mi?" denetimi                  |
| URETTIGI KANIT : suc dosya + satir no + §2 kutusunda dolu RİSK satiri     |
| NEDEN BU AJAN  : kendi kodunu kendi denetlemek kopruleri gormez;           |
|                  uzman disi inceleme "gecisli" der ve hata kacar           |
| MALIYET        : gate RED en gec GATE-003'te cikar; o noktada kod           |
|                  yazilmis oldugu icin geri alinmasi 2x is                  |
| GENERAL'A DUSER: inceleme bir uzmanlik alani disi ise (ornek:              |
|                  terim tutarliligi, link taramasi raporu)                  |
+----------------------------------------------------------------------------+
```

```text
+-- TESTER ------------------------------------------------------------------+
| TIPIK 3 GOREV  : (1) eksik yol icin test yazmak                            |
|                  (2) kapsam raporunu olcmek (hangi test ne yakalar)        |
|                  (3) kirilan testi onarilmaya gondermeden once ayiklamak   |
| URETTIGI KANIT : test dosyasi + `dotnet test` ciktisi (gecti/kaldi sayisi) |
| NEDEN BU AJAN  : test yazimi ayri bir urundur; developer ayni varsayimla   |
|                  test yazarsa hatali varsayim iki yerde tekrarlanir        |
| MALIYET        : "testler gecti" kaniti sahte cikar; gate yeşil gorunur,   |
|                  uretimde kirmizi kalir                                    |
| GENERAL'A DUSER: is kod degil, test altyapisi/ortam kurulumu ise           |
|                  (ornek: runsettings, filtre sozdizimi)                    |
+----------------------------------------------------------------------------+
```

```text
+-- SECURITY-AUDITOR ---------------------------------------------------------+
| TIPIK 3 GOREV  : (1) OWASP kalemleri uzerinden denetim                    |
|                  (2) secret / anahtar kasasi incelemesi                    |
|                  (3) SQL/parametrelastirme denetimi (string concat yok)   |
| URETTIGI KANIT : bulgu listesi + severity + etkilenen dosya/satir         |
| NEDEN BU AJAN  : guvenlik bulgusu "RİSK" satirina sigmaz; aciklik          |
|                  uygulama disinda surer ve kanit formati farklidir         |
| MALIYET        : reviewer "kod temiz" der, gate acilir, aciklik            |
|                  production'a gider; duzeltme isi iki kat buyur            |
| GENERAL'A DUSER: is guvenlik degil, sadece yapi/ortam bilgisi toplamak    |
|                  ise (ornek: paket surumu listesi cikarimi)                |
+----------------------------------------------------------------------------+
```

```text
+-- UI-DESIGNER --------------------------------------------------------------+
| TIPIK 3 GOREV  : (1) 01..07 ekranindan birinin tasarimi                    |
|                  (2) `.Designer.cs` / `.resx` duzenlemesi (U5 sahipligi)   |
|                  (3) ekran akisi + erisilebilirlik notu yazmak             |
| URETTIGI KANIT : [glossary](glossary.md) terimli tasarim +                 |
|                  ekran adi (01-start-page .. 07-settings) + F3 onayi       |
| NEDEN BU AJAN  : ekranda kod degil sozlesme uretir; developer eliyle      |
|                  `.Designer.cs` yazimi kaynak urununu kirar                |
| MALIYET        : onay zinciri (05 -> onay -> 06 -> 04) kopar;              |
|                  ADR-025 gate'i acilamaz                                   |
| GENERAL'A DUSER: is ekran degil, metin/terminoloji ise                     |
|                  (ornek: glossary'a terim eklemek)                         |
+----------------------------------------------------------------------------+
```

```text
+-- GENERAL (varsayilan) -----------------------------------------------------+
| TIPIK 3 GOREV  : (1) matriste karsiligi olmayan tek seferlik is           |
|                  (2) iki uzman arasi arastirma / kaynak derleme            |
|                  (3) dosya tasima / yeniden adlandirma gibi nötr is        |
| URETTIGI KANIT : kendi sozu degil; komut ciktisi, satir no ya da           |
|                  var olan rapor baglantisi                                 |
| NEDEN BU AJAN  : matris disi isi uzmansiz yapmak ucuza gelir; ama          |
|                  uzmansiz is "gecisli" onay uretir                          |
| MALIYET        : uzman isi general'a verilirse kanit eksik kalir ve        |
|                  is §4'teki 3-STRIKE'a kadar sessizce tekrarlanir          |
| GENERAL'A DUSER : zaten oradadir; soru "bu is matriste var mi?"            |
|                   - yoksa general, varsa atanmis ajan                      |
+----------------------------------------------------------------------------+
```

**Tek satırlık eşik:** istek `l0..l7` katmanına, `.Designer.cs`'e, OWASP
kalemine ya da `dotnet test` çıktısına dokunuyorsa `general` **değildir**;
matristeki uzmana gider. Dokunmuyorsa `general`.

### §1.2 Yanlış ajan maliyeti tablosu (iş tipi × yanlış ajan × sonuç)

§1.1'deki `MALIYET` satırları burada iş tipine göre karşılaştırılır. Sütunlar:
hangi iş, kime verildi, ne olur, ölçülebilir sonuç, doğrusu.

| # | İş tipi | Yanlış ajan | Ne olur | Ölçülebilir sonuç | Doğru ajan |
|---|---|---|---|---|---|
| 1 | Katman / mimari karar | `developer` | Doğru katmana yazmak yerine kolay olan katmana yazar; `l2 -> l4` gibi ters ok açılır | NetArchTest RED + yazılmış kodun geri alınması = **2x iş** | `architect` |
| 2 | Kod yazımı / refactor | `architect` | Karar çıkar, kod çıkmaz; "bitti" için kanıt üretilmez | Gate açılmaz, iş hiç başlamamış sayılır = **1x kayıp** (erken fark) | `developer` |
| 3 | İnceleme / review | `developer` (kendi kodu) | Aynı varsayım hem yazılmış hem doğrulanmıştır; körlük oluşur | Hatalı kanıt gate'e kadar gizlenir = **geç RED** | `reviewer` |
| 4 | Test / kapsam | `general` | Test adları kural dışı kalabilir, veri seti tek yolda toplanır | Kapsam raporu şişer, gerçek boşluk örtbas = **yanlış kanıt** | `tester` |
| 5 | Güvenlik | `reviewer` | Bulgu listesi yerine tek `RİSK` satırı çıkar; severity yok | OWASP kalemi yok sayılır = **gate RED + tekrar denetim** | `security-auditor` |
| 6 | UI / ekran | `developer` | `.Designer.cs` elle yazılır (U5 ihlali), ekran akışı tanımsız kalır | F3 onay zinciri kopar = **ADR-025 gate'i açılmaz** | `ui-designer` |
| 7 | UI / ekran | `ui-designer` + kod yazımı aynı elde | Tasarım sozleşmesi ile uygulama aynı anda değişir, ayrışma noktası kaybolur | İnceleme yapılamaz (kendi işi) = **geç RED** | `ui-designer` → `developer` |
| 8 | Mimari inceleme (ihlal suphesi) | `general` | 13 soru cevaplanmaz, "benzer görünüyor" ile geçilir | Katman ihlali production'a kadar taşınır = **geç RED + 2x iş** | `architect` + `reviewer` |

**Tablonun okunması:** maliyet iki çeşittir — **erken RED** (ucuz: iş henüz
yazılmamıştır) ve **geç RED** (pahalı: kod yazılmış, gate'e dayanmıştır).
Yanlış ajan ataması nadiren erken RED üretir; çoğunlukla **geç RED** ile
biter, çünkü uzmanlık dışı ajan eksikliği kendi çıktısında görünmez.

**Bu tablo §4 ile karıştırılmaz:** yanlış ajan hatası strike **sayılmaz**
(ajan yanlışdır, yöntemi değil); sayaç işin kendisine aittir. Strike, aynı
doğru ajanın aynı işi 3 kez düşürmesidir (§4).

### §1.3 Ajan çıktısının gate'e girişi (hangi kutu hangi kapıda okunur)

Dağıtım (§1) işi **başlatır**, gate işi **kapatır**; ikisinin buluşma noktası
aşağıdaki tablodur. Bir ajanın kutusu, gate'in kanıt şartını karşılamıyorsa
iş **o ajana geri döner**, başkasına devredilmez (kendi işini kendi kanıtlar).

| # | Ajan | Ürettiği ana kanıt | Okuyan kapı | Kapıda aranan şart |
|---|---|---|---|---|
| 1 | `architect` | ADR taslağı + 13 soru cevabı | GATE-004a (vault) | Katman/proje kararı tek kaynakta, çelişki yok |
| 2 | `developer` | diff + build + test çıktısı | GATE-003 (onay, HARD STOP) | Kullanıcı onayı + 0 hata / yeşil test |
| 3 | `reviewer` | suç dosya/satır + dolu `RİSK` | GATE-003 öncesi inceleme | `RİSK` satırı boş değil, madde ≤ 1 |
| 4 | `tester` | `dotnet test` çıktısı + kapsam raporu | GATE-004 (ilk commit) | Yeşil test yoksa commit yok |
| 5 | `security-auditor` | OWASP bulgusu + severity | Güvenlik geçişi (`DOĞRULAMA GEREKLI`) | Severity sıfırlanmamış ya da gerekçelenmemiş |
| 6 | `ui-designer` | Ekran adı + tasarım sözleşmesi + onay | ADR-025 gate'i | F3 akışı (05 → onay → 06 → 04) tam |
| 7 | `general` | Kendi sözü değil, dayandığı komut çıktısı | İlgili gate, ek şart yok | Kural 7: kanıt somut olmalı (§2.1) |

**Ters akış:** kapı RED verirse kutu **üreten ajana** geri gider; iş başka
ajana devredilecekse §3'teki 4 madde zorunludur. Kapı RED'i strike **sayılmaz**
(gate kuralıdır, ajan yöntemi değil) — strike sayımı yalnız §4'teki aynı iş
tekrarlarında işler.

---

## §2 Çıktı kutusu (zorunlu format)

Her ajan çıktısını şu ASCII kutuyla verir (Unicode kutu çizimleri `U+2500-U+257F` YASAKTIR):

```text
+--------------------------------------------------+
| BASLIK : 2-4 kelime, ne yapıldı                  |
| KAPSAM : dokunulan dosyalar (yol) veya "yok"     |
| KARAR  : seçim + tek satır gerekçe               |
| RİSK   : en fazla 1 madde; yoksa "yok"           |
| KANIT  : komut çıktısı / satır no / test sonucu  |
+--------------------------------------------------+
```

- **KANIT zorunlu** — kanıtsız çıktı kaba metindir; işlenmez, aynı kutuyla tekrar istenir.
- **RİSK zorunlu** — "yok" yazılabilir ama boş bırakılamaz.
- **KAPSAM "yok"** = dosyaya dokunulmadı (salt okuma iş).

**Uygulama örneği (birim test ekleme):**

```text
+--------------------------------------------------+
| BASLIK : ClimbingLogTest'e 3 yol testi eklendi   |
| KAPSAM : tests/VersaClimb.ClimbingLog.Tests/...  |
| KARAR  : Theory ile 3 veri; ayrı test = tekrar   |
| RİSK   : tekrar kapsamadı; kapsam raporu güncel  |
| KANIT  : dotnet test -> 3 geçti, 0 kaldı         |
+--------------------------------------------------+
```

**Karıştırma yasağı:** bu 5 alanlı ajan kutusu ile gate'in 4 satırlı kanıt kutusu
(`KANIT/DOSYA/EKSIK/SONRAKI`, [WORKFLOW](WORKFLOW.md) §2) aynı şey değildir; biri
diğerinin yerine kullanılamaz (SSOT).

### §2.1 Kabul / RED kriterleri (kutu tek başına değerlendirilir)

Kutu, üreten ajanın iddiasına göre değil, **bu tabloya** göre kabul edilir.
Bir satır bile sağlanmazsa kutu RED edilir ve aynı kutuyla tekrar istenir.

| # | Kriter | Kabul | RED |
|---|---|---|---|
| 1 | 5 alan dolu mu | `BASLIK` · `KAPSAM` · `KARAR` · `RİSK` · `KANIT` yazıyor | Eksik alan var |
| 2 | `RİSK` satırı | En fazla 1 madde ya da kelimesi kelimesine `yok` | Boş bırakılmış ya da 2+ madde |
| 3 | `KANIT` satırı | Komut çıktısı, satır no ya da test sonucu (somut) | "bitti / hazır / tamam gibi" — kanıt sayılmaz |
| 4 | `KAPSAM` satırı | Dokunulan dosya yolları ya da `yok` (salt okuma) | Yok halde dosya var ya da yol uydurulmuş |
| 5 | Kutu biçimi | ASCII `+ - \|` ile çizilmiş, 5 satır | Unicode kutu çizim (`U+2500-U+257F`) |
| 6 | Kutu türü | 5 alanlı ajan kutusu (bu §) | Gate'in 4 satırlı kanıt kutusuyla karıştırılmış |
| 7 | Ajan ataması | İş, §1 matrisine göre doğru ajandan gelmiş | Uzman dışı ajan (bkz. §1.2) |
| 8 | Kapsam bütçesi | Aynı anda ≤ ~8 dosya; oturumda ≤ ~3 dosya | Aşım özetlenmemiş (bkz. §6) |
| 9 | Katman sınırı | Mimari etkili çıktı 13 sorudan geçmiş | `layer-boundary-check` cevapsız |

**Kırmızı çizgi:** `KANIT` satırı boşsa diğer 8 satır ne kadar dolu olursa
olsun kutu işlenmez — kanıtsız teslim, teslim değildir (G5).

### §2.2 Dolu örnek — mimari karar (architect)

```text
+--------------------------------------------------+
| BASLIK : l0'a IHttpGateway portu eklendi          |
| KAPSAM : src/VersaCoder.Abstractions/Net/IHttpGateway.cs (yeni) |
|          src/VersaCoder.Infrastructure/Net/HttpGateway.cs       |
| KARAR  : port l0'da, uygulama l4'te; l2 ile l4 arasinda dogrudan |
|          ok yok (ADR-028 yon kurali)                             |
| RİSK   : l4'un tek projede sismesi; NetArchTest matrisi ile denetlenecek |
| KANIT  : dotnet build -> 0 hata / layer-boundary-check 13 soru: 13 cevapli |
+--------------------------------------------------+
```

**Neden kabul:** `KANIT` somut (komut çıktısı + soru sayısı), `KARAR` tek
satırda seçim ve gerekçeyi birlikte veriyor, `RİSK` somut bir denetim
aracına bağlı. **Neden RED olurdu:** `KANIT` "derlendi" dese (komut yok),
ya da `KAPSAM` "ilgili dosyalar" dese (yol yok).

### §2.3 Dolu örnek — bug düzeltme (developer)

```text
+--------------------------------------------------+
| BASLIK : Oturum listesi UTC kaydirma duzeltildi   |
| KAPSAM : src/VersaCoder.Infrastructure/Store/SessionStore.cs |
|          src/VersaCoder.Host/Session/SessionLoader.cs        |
| KARAR  : donusturme tek noktada (SessionStore); ikinci yerde  |
|          tekrar = ayni bug'in iki kopyasi                      |
| RİSK   : eski kayitlar yerel saatte kalmis olabilir;           |
|          migration gerekliligi NEEDS_VALIDATION                |
| KANIT  : dotnet test -> 12 gecti, 0 kaldi /                    |
|          SessionStore.cs : 42 -> 51 (tek donusturme noktasi)   |
+--------------------------------------------------+
```

**Neden kabul:** `KAPSAM` gerçek iki dosyayı veriyor, `KARAR` neden tek
noktada düzeltildiğini açıklıyor, `KANIT` sayı ve satır no taşıyor.
**Neden RED olurdu:** `RİSK` boş ya da "eski veri sorun olabilir" gibi
denetlenemeyen bir cümle olsaydı (kontrol aracı yok = kanıt yok).

---

## §3 Devre paketi (ajanda devrinde 4 madde)

Bir ajan işi başka ajana devrederse **4 madde zorunludur**; eksik devir iş emri
değil, taslaktır ve alıcı tarafından geri çevrilebilir.

```text
1. KALAN İŞ  : ne bitti, ne kaldı (madde madde)
2. DOSYA      : dokunulan dosya yolları + satır aralığı
3. KANIT      : eldeki kanıt (build/test çıktısı varsa)
4. SINIR      : aşılmayacak katman/kapsam sınırı (ADR-028)
```

- **Alıcı hakkı:** 4 madde eksikse devri geri çevir, `bekle` durumuna geç ve
  tamamlat — eksik devirle çalışmak, hatalı kanıtla çalışmaktır.
- **Geri sayım:** [ROLE](ROLE.md) §5'teki 3 hamlelik devir zinciri bu 4 maddeyi
  her adımda taşır; zincir koparsa iş kullanıcıya döner.
- **Bilgi notu:** 18 kaynak rolün 5'e indirilmesi (architect, developer, reviewer,
  tester, ui-designer) ve devir zinciri `DOĞRULAMA GEREKLIDIR` — bkz. [ROLE](ROLE.md) §1.
- **Sınır tek satır:** devir paketi en fazla ~2 KB'tır (bkz. [§6](#6-bütçe-token--byte));
  aşımda açıklama kısaltılır, 4 madde korunur.

## §4 3-STRIKE (aynı iş 3 kez düşerse dur) + bloke raporu

```text
Strike 1 -> aynı yöntemle tekrar dene (küçük düzeltme)
Strike 2 -> yöntemi değiştir (farklı yaklaşım, aynı hedef)
Strike 3 -> DUR - başka ajana devret veya kullanıcıya sor
```

- **Kural:** 3. strike'tan sonra denemeye devam ETME — tahmin artık israfın
  işaretidir; [ROLE](ROLE.md) §5'teki `dur/bekle` kuralı buradan tetiklenir.
- **Sayaç sıfırlanmaz:** bir strike aynı iş içinde geçerlidir; yeni iş = yeni sayaç.
- **Ne sayılmaz:** kullanıcının yeni veri/koşul getirmesi = yeni iş, strike değil.
- **Butçe aşımı strike değildir** ([§6](#6-bütçe-token--byte)): bütçe aşımı işi
  küçültür, işi ajan değiştirmez.

**Bloke raporu şablonu** (3. strike ve `dur/bekle` etiketlerinde zorunlu):

```text
ETIKET     : 3-STRIKE | GATE-OPEN | G7 ONAYI | UNKNOWN | CONFLICT DETECTED
KONUM      : <dosya yolu> : <satır>   ya da   <gate adı>
NEDEN      : <denenip düşen şey, 1 cümle>
VARSAYIM   : <şüpheli varsayım, tek madde>
SECENEKLER : A) ...   B) ...   C) ...
SORU       : <kullanıcıya tek soru; evet/hayır olabilmeli>
```

**Dolu örnek:**

```text
ETIKET     : 3-STRIKE
KONUM      : src/VersaClimb.L1.ClimbingLog/LogService.cs : 42
NEDEN      : 3 düzeltmede aynı test `Assert.Fail` ile düştü
VARSAYIM   : test, saat dilimi yerine UTC bekliyor olabilir
SECENEKLER : A) testi UTC'ye çek  B) servisi UTC'ye çek  C) kapsamı dondur
SORU       : Beklenen kaynak saati UTC mi, yerel saat mi?
```

- **Şablon SSOT:** biçimi (`ETIKET · KONUM · NEDEN · VARSAYIM · SECENEKLER · SORU`)
  yalnız burada yazılır; [WORKFLOW](WORKFLOW.md) §4 bu şablonu referans verir.
- **Tek soru kuralı:** bloke raporu en fazla **1** soru içerir; çok soru = karar
  verilmemiştir, ROLE §3 tek soru kuralına düşer.

### §4.1 İkinci dolu örnek — gate RED tekrarı (architect)

```text
ETIKET     : 3-STRIKE
KONUM      : checklists/layer-boundary-check.md : 7 (yon satiri)
NEDEN      : 3 duzeltmede matris ile checklist arasindaki l4 -> l2 uyuşmadi
VARSAYIM   : checklist hala ADR-013'un eski "yedi katman" metnini tasiyor
SECENEKLER : A) checklist'i matrise cevir  B) matrisi checklist'e cevir
             C) iki kaynagi tek dosyada birlestir
SORU       : Tek kaynak dependency-map.md mi, bu checklist mi?
```

**Neden 3. strike:** 1. ve 2. denemede dosyanın yalnız bir satırı
düzeltilmişti, ikinci kaynak yerinde kaldı; yöntem aynı kaldı (tek satır
oynatma), hedef değişmedi. 3. denemede yöntem yine aynı olduğu için aynı
sonuç çıktı → `DUR`.

**Bu örnek neyi gösterir:** sorun çoğu zaman "düzeltme yanlış" değil,
**"iki kaynak"tır** (G10 çakışması); tek soru kuralı sayesinde karar bir
seçeneğe indirgenir ve kilit dışarıda kalır.

### §4.2 Üçüncü dolu örnek — onay kopması (ui-designer)

```text
ETIKET     : 3-STRIKE
KONUM      : src/VersaCoder.Desktop/Chat/ChatPanel.Designer.cs : 1
NEDEN      : 3 denemede ekran kaydirma bozuk; kod her seferinde elde yazildi
VARSAYIM   : ekran kodu, uygulayici eliyle degil tasarim sozlesmesinden
             turetilmeli (F3 akisi 05 -> onay -> 06 -> 04)
SECENEKLER : A) tasarimi sozlesmeye bagla  B) mevcut Designer kodunu
             geri al  C) ekrani 04-code-editor gecici cozumuyle ac
SORU       : Bu ekranin tasarim sozlesmesi var mi, yoksa olusturulsun mu?
```

**Neden 3. strike:** aynı `.Designer.cs` dosyası 3 kez elle müdahale
gördü; elle müdahale bu işin geçerli yöntemi değildi (U5 sahipliği
`ui-designer`'da, uygulama `developer`'da). Yöntem değişmediği için sonuç
değişmedi.

**Bu örnek neyi gösterir:** `3-STRIKE` yalnızca "kod hatası" için değil,
**"yanlış yöntem ısrarı"** için de çalışır; 3. denemede sorulacak şey
artık düzeltme değil, yöntemin kendisidir.

### §4.3 Strike sayılmayan durumlar (sayaç artmaz)

Aşağıdakilerde işin düşmesi ajanın yöntemini göstermez; sayaç **aynı işte
yerinde kalır**, iş seriye alınır ya da koşul değişir.

| # | Durum | Neden strike değil | Ne yapılır |
|---|---|---|---|
| 1 | `Rate limit exceeded` (bkz. §5.1 kaza kaydı) | Düşüşün kaynağı sağlayıcı sınırı; ajan yöntemi denenemedi | Paralelliği 3'e indir, aynı sayaçla seri devam |
| 2 | `Model unavailable: ...` | Erişilebilirlik sorunu, yaklaşım hatası değil | Modeli sabitle + işi aynı ajanla tekrar başlat |
| 3 | Bütçe aşımı ([§6](#6-bütçe-token--byte)) | Bütçe işi **küçültür**, ajanı **değiştirmez** | İşi böl; gate kabul kriteri kazanır, bütçe kısaltılır |
| 4 | Kullanıcının yeni veri / koşul getirmesi | Yeni bilgi = yeni iş (§4 "Ne sayılmaz") | Yeni iş, **yeni sayaç** |

**Ortak kural:** strike, **aynı doğru ajanın aynı işi aynı yöntemle 3 kez
düşürmesidir.** Dışsal kesintiler (sağlayıcı, bütçe, yeni veri) sayaç
işletmez; ama aynı dışsal kesinti 3. kez olursa artık yöntem değil,
**ortam** sorgulanır → bloke raporu yine yazılır, `ETIKET` bu kez `UNKNOWN`.

---

## §5 Eşzamanlılık (en fazla 3 paralel ajan)

- **Sınır:** aynı anda **en fazla 3** ajan çalışır (rate limit; WORKFLOW §3.2/B
  bloğuyla aynı sayı).
- **Paylaşılan dosya:** aynı dosyaya iki ajan aynı anda yazamaz — seriley al ya da
  [ROLE](ROLE.md) §5'teki devir zincirine geç.
- **Sıralanması gerekenler:** bağımlı işler (önce architect, sonra developer), aynı
  modülü değiştirenler, aynı test dosyası.
- **Sıralanması gerekmeyenler:** farklı katmanlardaki bağımsız işler (ADR-028 katman
  bağları dahilinde) — onları paralel çalıştır.

```text
[ A: mimari inceleme ]--+
[ B: kod yazımı      ]--+--> eşzamanlı (sınır: 3 ajan)
[ C: test yazımı     ]--+
        |
        +--> aynı dosya? -> SERİ (sırala) | farklı dosya? -> PARALEL
```

- **Kanıt:** paralel blok kapanırken her ajan kendi §2 kutusunu döner; kutusuz ajan
  blok sonucuna sayılmaz.

### §5.1 Kaza kaydı — 2026-09-29: 8 paralel ajan (gerçek olay)

Bu olay **kurgu değil, bu vault'ta yaşandı**; kaynak bu oturumun kaydıdır ve
aynı olay [engine](engine.md) §6.1 hata modunda tekrar işlenmiştir. Buradaki
işlevi, §5 sınırının **neden 3 olduğunu** kanıtlanmasıdır.

| Alan | Kayıt |
|---|---|
| Tarih | 2026-09-29 (bu oturum) |
| Tetik | 8 ajan **aynı anda** çağrıldı; eşzamanlılık sınırı hiç konuşulmadı |
| Belirti 1 | İlk gelen yanıt: `Rate limit exceeded` |
| Belirti 2 | Çağrılan **8 ajanın 8'i** `Model unavailable: anthropic/...` ile düştü |
| Sonuç | Blok sonucu boş döndü; hiçbir ajan kendi §2 kutusunu dönemedi |
| Kök neden 1 | Fan-out, sağlayıcı limiti bilinmeden yapıldı (sayı uyduruldu, sınır sorulmadı) |
| Kök neden 2 | Seçilen model erişilebilir değildi; "geçici" varsayılıp devam edildi |
| Çözüm 1 | Paralel ajan sayısı **3'e** indirildi — §5'in bugünkü sınırının kaynağı |
| Çözüm 2 | Model **`opencode/mimo-v2.6-flash-free`** olarak sabitlendi (önceki model çağrıları düşmüştü) |
| Doğrulama | Kalan iş **3'erli batch'lerde** çalıştırıldı; batch'lerin tamamı geçti |
| Kalan açık | Hangi sağlayıcı/limit tam olarak döktüğü sayısal olarak yazılmadı → `DOĞRULAMA GEREKLI` |

**Zaman çizelgesi:**

```text
 8 paralel ajan cagrisi  (2026-09-29)
       |
       +--> "Rate limit exceeded"                (ilk belirti)
       +--> "Model unavailable: anthropic/..."    (8/8 ajan dustu)
       |
       v
 KOK NEDEN:  fan-out (8) > saglayici limiti  +  model erisilebilir degil
       |
       v
 COZUM     :  paralellik = 3  +  model opencode/mimo-v2.6-flash-free
       |
       v
 3'erli batch -> 1. batch GECTI -> 2. batch GECTI -> ... (hepsi gecti)
       |
       v
 KANIT     :  her ajan kendi 5 alanli kutusunu doner (kutusuz sayilmaz)
```

**Bu kazadan çıkan 5 kural:**

| # | Çıkan kural | Nereye yazıldı |
|---|---|---|
| 1 | Paralel ajan ≤ 3 — sınır tahmin değil, **olayla** belirlendi | [§5](#5-eşzamanlılık-en-fazla-3-paralel-ajan) bu bölüm |
| 2 | Model adı sabitlenir; her batch'te farklı model denenmez | [§5.1](#51-kaza-kaydı--2026-09-29-8-paralel-ajan-gerçek-olay) bu tablo |
| 3 | Rate limit düşüşü strike sayılmaz (ajan yöntemi değil, ortam) | [§4.3](#43-strike-sayılmayan-durumlar-sayaç-artmaz) |
| 4 | Sağlayıcı/model hatasında `UNKNOWN` yazılır, "geçici" denip geçilmez | [§4](#4-3-strike--aynı-iş-3-kez-düşerse-dur--bloke-raporu) + [engine](engine.md) §6.2 |
| 5 | Batch sonunda her ajan kutusunu döner; kutusuz blok kapanmaz | [§2](#2-çıktı-kutusu-zorunlu-format) |

**Ölçülebilir sonraki adım:** 3 sınırı sayısal limit değerine
dayandırılmamıştır (sağlayıcı limiti bilinmiyor) → ilk başarısız batch
sayısı `log.md`'ye tarihli yazılmalıdır; oran çıkmazsa sınır **varsayım**
olarak `DOĞRULAMA GEREKLI` etiketiyle kalır.

---

## §6 Bütçe (token / byte)

| Kalem | Sınır | Aşımında |
|---|---|---|
| Tek ajan çıktısı | en fazla ~8 KB | Özetle; detayı [MEMORY](MEMORY.md)'e taşı |
| Devre paketi ([§3](#3-devre-paketi-ajanda-devrinde-4-madde)) | en fazla ~2 KB | 4 maddeyi koru, açıklamayı kısalt |
| Aynı anda dokunulan dosya | en fazla ~8 dosya | Bölümlere ayır; [WORKFLOW](WORKFLOW.md) §4'e geç |
| Bir oturumda toplam dosya | en fazla ~3 dosya | Yeni oturuma böl, kanıtı [log.md](log.md)'e yaz |
| RİSK maddesi | en fazla 5 madde | En yüksek severity'yi al, gerisini arşivle |
| Bloke raporu sorusu | en fazla 1 soru | Kalanı SECENEKLER'e taşı |

- **Kanıt usulu:** `~` ile yazılan limitler **token/byte kestirimidir** (ölçülmemiştir)
  — `INFERRED` etiketiyle çalışır; kesin ölçüm gerekirse sayacla.
- **Kestirim nedeni:** ADR-028 kapsamı (9 üretim + 1-2 test proje, 52 düğüm) dosya
  sayısını belirler; buna karşılık gelen byte eşikleri henüz sayılmamıştır.

## §7 Tek dosya = tek sahip (X ownership)

- **Kural:** her dosyanın **bir sahibi** (ajan) vardır; sahip dışındaki ajan o
  dosyaya **yazamaz** — okuyabilir, yorum bırakabilir, sahipten ister.
- **X sınırı:** bir ajan aynı anda en fazla **X dosyanın sahibidir**; X = sahiplik
  sınırı `DOĞRULAMA GEREKLIDIR` (ölçüm: dosya yolu listesi). Sınır aşılırsa dosya
  sahipliğini devret ([§3](#3-devre-paketi-ajanda-devrinde-4-madde)).
- **Ezme çözümü:** iki ajan aynı dosyayı istiyorsa önce kimin işinin kanıtı varsa o
  kazanır; kanıt yoksa [WORKFLOW](WORKFLOW.md) §3.2'deki "aynı `.cs` dosyasına eş
  zamanlı iki yazıcı yok" kuralı çalışır, ardından ROLE §5 devri gelir.
- **Sahiplik matrisi:** hangi dosyanın kime ait olduğu proje ölçeğinde tutulur;
  güncel liste `DOĞRULAMA GEREKLIDIR`.

## §8 Doğrulama

- [ ] **Kutu zorunlu:** [§2](#2-çıktı-kutusu-zorunlu-format) kutusu eksikse çıktı
  işlenmez; `KANIT` satırı boş ise iş tamamlanmış sayılmaz.
- [ ] **Doğru ajan:** iş, [§1](#1-iş-tipi--ajan-matrisi) matrisine göre atanmış;
  matris dışıysa `general` kullanılmış.
- [ ] **Devir paketi:** devir olduysa 4 madde dolu ([§3](#3-devre-paketi-ajanda-devrinde-4-madde)).
- [ ] **Katman sınırı:** mimari etkili her çıktı
  [layer-boundary-check](checklists/layer-boundary-check.md) 13 sorusundan geçer
  (ADR-028 baglamında).
- [ ] **Eş zaman:** [§5](#5-eşzamanlılık-en-fazla-3-paralel-ajan) 3 ajan sınırı ve
  tek dosya sahipliği aşılmaz; aşım varsa iş durur.
- [ ] **Report:** iş sonunda rapor biçimi [WORKFLOW](WORKFLOW.md) §5'e göre;
  burada yalnız ajanın kendi §2 kutusu döner.

### §8.1 Geniş kontrol listesi (batch kapanışında 20 madde)

§8 maddeleri **kural** hatırlatmasıdır; bu liste **işlem sırasıdır** — bir
batch biterken yukarıdan aşağısı tek tek işaretlenir, işaretlenmeyen madde
işi durdurur.

- [ ] 1. İstek §1 matrisinde mi? Yoksa `general` kullanıldı mı?
- [ ] 2. Doğru ajan, §1.1 profiline göre mi seçildi (`MALIYET` satırı okundu mu)?
- [ ] 3. Yanlış ajan riski §1.2 tablosundan geçirildi mi (geç RED mi, erken RED mi)?
- [ ] 4. Aynı anda çalışan ajan ≤ 3 mü (§5)?
- [ ] 5. Aynı dosyaya ikinci yazıcı yok mu (§7)?
- [ ] 6. Dosya sahipliği doğru ajan üzerinde mi (§7)?
- [ ] 7. Her ajan 5 alanlı kutusunu döndü mü (§2)?
- [ ] 8. `KANIT` satırı somut çıktı içeriyor mu (§2.1 / madde 3)?
- [ ] 9. Kutu ASCII `+`, `-` ve `|` karakterleriyle mi çizildi (Unicode `U+2500-U+257F` yok)?
- [ ] 10. Gate'in 4 satırlı kanıt kutusu ajan kutusuyla karıştırıldı mı?
- [ ] 11. Mimari etkili çıktılar 13 sorudan geçti mi?
- [ ] 12. `.Designer.cs` / `.resx` işlemleri `ui-designer`'dan mı geçti (U5)?
- [ ] 13. Devir olduysa 4 madde dolu mu (§3)?
- [ ] 14. Devir zinciri 3 hamle sınırını aştı mı (ROLE §5)?
- [ ] 15. Aynı işte sayaç 3'e ulaştı mı? Ulaştıysa bloke raporu yazıldı mı (§4)?
- [ ] 16. Dışsal kesinti (rate limit / model / bütçe) var mı? Varsa sayaç işletildi mi (§4.3)?
- [ ] 17. Çıktı bütçesi aşıldı mı? Aşıldıysa özetlendi mi (§6)?
- [ ] 18. Rapor biçimi WORKFLOW §5'e uygun mu?
- [ ] 19. Kural değişikliği varsa `log.md`'ye tarihli yazıldı mı?
- [ ] 20. Kanıtsız "bitti/tamam" ifadesi satır satır var mı (G5)?

**Sayım notu:** 20 madde = 20 işaret; 19 işaretli 1 boşsa iş **tamamlanmış
sayılmaz**. Kısmi geçiş raporlanır, "neredeyse tamam" olarak sunulmaz.

---

## §9 Referanslar

- [index.md](index.md) - dış navigasyon
- [MEMORY.md](MEMORY.md) / [log.md](log.md) - rapor kanıtı
- [engine.md](engine.md) - orkestrasyon
- [brain.md](brain.md) - beyin yapısı
- [VISION.md](VISION.md) / [PROJECTS.md](PROJECTS.md) - bağlam
- [keys.md](keys.md) - anahtarlar / [glossary.md](glossary.md) - terimler
- [ULTRA-THINKING.md](ULTRA-THINKING.md) - uzun düşünme
- [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) - katman temeli
- [architecture/index.md](architecture/index.md) - mimari indeks
- [checklists/layer-boundary-check.md](checklists/layer-boundary-check.md) - 13 soru
- [CLAUDE.md](CLAUDE.md) / [WORKFLOW.md](WORKFLOW.md) / [ROLE.md](ROLE.md) - kardeş dosyalar
