---
title: "MEMORY — Kalici Dersler, Karar Desenleri ve Kayit Defteri"
type: knowledge
category: ".ai"
status: active
version: 2.0.0
updated: 2026-10-07
authority: SSOT
scope: "Vault'un kalici hafizasi: kanitli dersler (DERS-KANIT-GECERLILIK-ADR), tekrar eden karar desenleri, kara liste, hafiza guncelleme kurali ve tarihli kayit defteri"
---

# MEMORY — Kalici Dersler, Karar Desenleri ve Kayit Defteri

**Zorunlu Baglantilar:** [index.md](index.md) · [CLAUDE.md](CLAUDE.md) · [WORKFLOW.md](WORKFLOW.md) · [ROLE.md](ROLE.md) · [AGENTS.md](AGENTS.md) · [brain.md](brain.md) · [keys.md](keys.md) · [glossary.md](glossary.md) · [VISION.md](VISION.md) · [log.md](log.md)

> **Tek cumlelik sorumluluk:** *"Bunu daha once denedik, ne cikti?"* sorusunun tek cevabidir — her dersin **kaniti** (dosya + o dosyada ne yazdigi), **gecerlilik siniri** ve **bagli ADR'si** satir satir yazilir; kanitsiz ders bu dosyaya girmez, `DOĞRULAMA GEREKLİ` etiketiyle disari cikar.

---

## §0 Ozet (Answer First)

Bu dosya vault'un **kalin hafizasidir**: denenen, cikti olcusulen ve sonucu ogrenilmis islerin kaydidir. Icerigi kisa: **8 kanitli ders** (§1), **5 tekrar eden karar deseni** (§2), **5 maddelik kara liste** (§3), **hafiza guncelleme kurali** (§4) ve **2026-09-29 tarihli 22 kayit** (§5). Karar gerekcesi burada degildir — gerekce `brain.md` ve ADR'lerde, kural `CLAUDE.md`'de, surec `WORKFLOW.md`'de, kronoloji `log.md`'dedir (SRP + SSUT). Sayilar **2026-09-29 recursive dizin olcumunden** gelir; olcmeden yazilan her sayi `DOĞRULAMA GEREKLİ`dir.

| Bölüm | Ne cevaplar | Kaç madde | Kanıt kuralı |
|---|---|---|---|
| §1 Kalıcı dersler | "Denedik, ne çıktı?" — ders + kanıt + geçerlilik + ADR | 8 ders | Her dersin satırında dosya adı ve o dosyadaki sayı yoksa ders sayılmaz |
| §2 Karar desenleri | "Bu karar neden hep aynı çıkıyor?" — tekrar eden şekil | 5 desen | Her desen en az iki ADR kaydına bağlıdır |
| §3 Kara liste | "Bunu asla yapma" — gerçek olayla kanıtlanmış | 5 madde | Olay olmadan yasak yazılmaz; yasak sahibi olmayan silinir |
| §4 Güncelleme kuralı | "Yeni bilgi nereye yazılır?" — eşik + akış | 5 tetikleyici | Eşiği geçmeyen bilgi ADR olmaz, kayıt satırı olur |
| §5 Kayıt defteri | "Bugün ne oldu?" — tek satırlık tarihli kayıtlar | 22 kayıt | Her kayıtta kanıt sütunu dolu; boşsa `DOĞRULAMA GEREKLİ` |

**Bu dosya ne — ne değil:**

| Bilgi türü | Burada mı? | Sahip dosya |
|---|---|---|
| Edinilmiş ders ("denedik, şu çıktı") | **Evet — §1** | bu dosya |
| Kararın gerekçesi ve alternatifleri | Hayır | [brain.md](brain.md) + `.decisions/` (defter: [.decisions/adr-index.md](.decisions/adr-index.md)) |
| Zorunlu kural (G1..G12, EF Core yasağı) | Hayır | [CLAUDE.md](CLAUDE.md); metin `.rules/index.md` altında |
| Gate/sıra/kapı tarifesi | Hayır | [WORKFLOW.md](WORKFLOW.md) + [engine.md](engine.md) |
| Tarihli olay ve ölçümler | Hayır | [log.md](log.md) (append-only) |

```text
+--------------------------------------------------+
| YENI BILGI GELDI                                 |
|   |                                              |
|   +-- kaniti var? -- HAYIR -> DOĞRULAMA GEREKLI  |
|   |                       (kara listeye, §3)     |
|   +-- kaniti var? -- EVET                        |
|         |                                        |
|         +-- ogrendigimiz ders mi? -> §1          |
|         +-- tekrar eden karar mi? -> ADR (defter)|
|         +-- tarihli olay mi?      -> log.md      |
|         +-- zorunlu kural mi?     -> CLAUDE.md   |
|         +-- karar gerekcesi mi?   -> brain.md    |
+--------------------------------------------------+
```

> **KURAL:** Bu dosyaya yalnız "ikinci kez aynı hataya dusmek istemiyorsak ne bilmemiz lazim" girer. Gerekce/kural/kronoloji buraya tasinrsa tekrar olur ve silinir (SSUT, [CLAUDE.md](CLAUDE.md) G1).

---

## §1 Kalici Dersler (kanitli)

Ders = **bir kez denendi, ciktisi olculdu, sonucu ogrenildi** ifadesidir. Buradaki 8 dersin tamami bu vault'taki dosyalara dayanir; her satirda "kanit" sutunu o dosyanin adini ve icindeki somut sayiyi tasir. Kanit sutunu bos olan satir **ders degil, varsayimdir** ve `DOĞRULAMA GEREKLİ` etiketiyle §5'e dusurulur.

### §1.1 Ders tablosu (ana kayit)

| ID | DERS (ne ogrendik) | KANIT (dosya + icindeki sey) | GECERLILIK (ne zaman biter) | BAGLI KARAR |
|---|---|---|---|---|
| L-01 | Elle sayilan ham envanter **tekrar icerir**: 618 giristen 2'si birebir ayni, 4 DLL dosyada hic yok, 9 basliktan 2'si yanlis — sayim "nizami" gorunur ama kontrolsuzdur | `reports/k3-618-ham-envanter.md` — 618 giris / 616 benzersiz / 2 tekrar / 4 eksik DLL kaydi | Katalog ve `count-components` sayimi **yazildigi surece** (simdi PASS: 5440 kayit) | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §1 C2, §2.2 |
| L-02 | Iki mimari kaynagi celistiginde **tek matris kazanir**: ADR-013 "l4 -> l2" derken dependency-map ayni oku "X" (yasak) olarak isaretliyor; iki kaynakli yon = iki gercek | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §1 C6 (celiski kaydi) + §2.3 madde 2 (8x8 matris tek kaynak kosulu) | NetArchTest `LayerRulesTests` matrisi yazilip gate'e baglanana kadar | ADR-028 §2.3/2 (ADR-013 dogrulandi) |
| L-03 | Proje sayisi **Tier'a bolunmez**: 575 csproj x 2 TFM = ~1150 hedef; CPM, Visual Studio ve restore bu yuku kaldiramaz, ustune K3'in kapsami disi DLL'leri (EntityFramework, Host.Linux, Protocol.Kafka) tasinir | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §3 alternatif A3 (UNIVERSAL RED) | Yeni bir `.csproj` kapidan gectigi surece (3 kapi kurali §2.1) | ADR-028 §2.1 (9 uretim + 1-2 test) |
| L-04 | Mojibake **bir kezlik kaza degil**, UTF-8 yazmayan her uretimde tekrar cikar: olcum 9 dosya gosterdi (8'i `.templates/`, biri `scripts/measure-vault.md`); duzeltme 8 dosyada 20 eslesme buldu | `reports/10-bilinen-eksikler.md` §1 (9 dosya listesi) + 2026-09-29 duzeltme kaydi (§5/#13) — kalan icin `scripts/find-mojibake.ps1` = 0 bekleniyor | Tarama **0 dosya** gostermeye kadar (kalan 1 dosya `DOĞRULAMA GEREKLİ`) | [CLAUDE.md](CLAUDE.md) G4 + `.rules/index.md` (ADR-020 ASCII kutu) |
| L-05 | Bozuk link **batch'ten dogar**, tek tek yazimdan degil: 46 bozuk linkin 34'u `knowledge/`, 4'u `checklists/`, 2'si `playbooks/` — hepsi "kardes dosya henuz yazilmadi" durumundan | `reports/10-bilinen-eksikler.md` §2 (kirik link tablosu); ayrica `reports/index.md` — `reports/` kaynakli bozuk link **0** | Her batch sonrasi `scripts/check-links.ps1` = 0 ciktisina kadar | ADR-003 (vault yapisi) + [WORKFLOW.md](WORKFLOW.md) GATE-004a |
| L-06 | Envanter **anliktir**: `knowledge/` indeksi 2026-09-29'da "kob `index.md` diskte YOK" yazarken dosya ayni gun olustu; eszamanli uretimde sayi dakikalar icinde degisir, eski satir dogru zamanda dogruydu | `knowledge/index.md` (yokluk kaydi) karsilastirmasi — 2026-09-29 dizin listesinde `index.md` VAR (15.693 B) | Bu vault eszamanli yazildigi surece; olcum **tarihli ve anlik** yapilir | [log.md](log.md) §1 (olcum birimleri) |
| L-07 | Eski kaynaktan **surum alinmaz**: `ai.01.md` .NET 8 / EF Core 9 / DevExpress 24.2.3 yaziyordu, yeni bootstrap bunu gecersiz kildi; 24.2.x daha ilk ADR turunde "REDDEDILDI" diye kaydedildi | `.decisions/index.md` §0 + §1 (karar 4: v26.1 tek surum, 24.2.x RED) — ayrica `.rules/index.md` (16 kural dosyasi) | Yigit degismedikce (veda `net48` kalmadikca) | ADR-004 (v26.1) + ADR-001 (iki bacak) |
| L-08 | Esik **marjsiz olursa tek satir RED eder**: katalog esigi 5440 = olculen 5440, marj 0 — bir satir silinince gate kirmiziya duser, "neredeyse gecti" diye bir durum yoktur | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §4 (eksi/risk: "marj 0") + `scripts/count-components.ps1` ciktisi 5440 kayit PASS | Esik yeniden olculup marj acilana kadar (su an acilmadi) | ADR-027 (5000+ bilesen) + ADR-028 §2.2 |

### §1.2 Uc ornek ders karti (derinlestirme)

**Kart L-01 — ham envanter guvenilmez:**

| Alan | Icerik |
|---|---|
| Ne oldu | K3 dosyasindaki 618 DLL listesi elle sayildi; 2 kayit birebir tekrar, 4 DLL hic dosyada yok, 9 basliktan 2'si yanlis cikti |
| Dersin maliyeti | Bu liste kataloga alinmis olsaydi `count-components.ps1` sayimi iki birimde calisacak, staleness iki katina cikacakti |
| Kanit nasil dogrulanir | `reports/k3-618-ham-envanter.md` acilir, §"tekrar" ve §"eksik DLL" tablolari sayilir; toplam 618 / benzersiz 616 satiri okunur |
| Kapanis kosulu | Katalogda tek sayim birimi `N###` satiri olarak kalir; 618 listesi yalniz rapor satiri olarak yasar |

**Kart L-05 — bozuk link batch kaynaklidir:**

| Alan | Icerik |
|---|---|
| Ne oldu | Tarama 1516 iyi / 46 bozuk link verdi; bozuklarin tamami "kardes dosya yazilmadi" dediginde uretilmis linkler |
| Dersin maliyeti | Link, hedefi bilinmeden yazilinca gate kirmizi kalir ve tum batch bekler — tek link, butun kapanisi geciktirir |
| Kanit nasil dogrulanir | `scripts/check-links.ps1` calistirilir; kirik link 0 olmali, kaynak klasor dagilimi `reports/10-bilinen-eksikler.md` §2 ile karsilastirilir |
| Kapanis kosulu | Hedef dosya once uretilir, link sonra yazilir (`.templates/new-doc-contract.md` §3 kurali) |

**Kart L-06 — envanter anliktir:**

| Alan | Icerik |
|---|---|
| Ne oldu | `knowledge/index.md` "2026-09-29'da diskte YOK" derken `index.md` ayni gun olustu; `engine.md` 16:42'de eklendi |
| Dersin maliyeti | Bayat sayi ile yazilan "eksik" satiri bir sonraki oturumda yalanci bir is listesi olur |
| Kanit nasil dogrulanir | Recursive dizin sayimi yeniden calistirilir: kok 14 hedefe kac dosyada, toplam kac MD |
| Kapanis kosulu | Her sayi **tarih + yontem** ile yazilir; tarihsiz sayi gecersizdir |

**Kart L-02 — iki kaynakli yon iki gercektir:**

| Alan | Icerik |
|---|---|
| Ne oldu | ADR-013 "l4 -> l2 bagimli" derken dependency-map ayni oku "X (yasak)" olarak isaretledi; iki dosya ayni ok uzerinde ayristi |
| Dersin maliyeti | Hangisi dogru bilinmeden NetArchTest yazilsaydi test ya gercek ihlali kacirir ya da dogru bagimliligi yasaklar — ikisi de gate'de fark edilir, ama gec |
| Kanit nasil dogrulanir | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §1 C6 acilir, celiski kaydi okunur; §2.3 madde 2'de "8x8 matris tek kaynak" kosulu aranir |
| Kapanis kosulu | Yon/baglanti sorusu artik **tek matristen** cevaplanir; matris guncellenene kadar ikinci kaynak gecersizdir |

**Kart L-04 — mojibake uretim kaynaklidir:**

| Alan | Icerik |
|---|---|
| Ne oldu | 9 dosyada bozuk byte bulundu (8'i `.templates/`, biri `scripts/measure-vault.md`); duzeltme turu 8 dosyada 20 eslesme verdi |
| Dersin maliyeti | Bozuk byte `check-links.ps1`'i etkilemedigi icin "linkler temiz" ile birlikte yasayabilir — yanlis temizlik hissi |
| Kanit nasil dogrulanir | `scripts/find-mojibake.ps1` calistirilir; hedef **0 dosya**; kalan 1 dosya icin cikti `reports/10-bilinen-eksikler.md` §1 ile karsilastirilir |
| Kapanis kosulu | Tarama 0 donene kadar L-04 kapanmaz; yeni UTF-8 disi yazim gorulurse §3/5'e madde acilir |

**Kart L-08 — marjsiz esik kirmiziya yakin:**

| Alan | Icerik |
|---|---|
| Ne oldu | Katalog esigi 5440 = olculen 5440; marj 0, yani tek satirlik kayip gate'i kirmiziya duser |
| Dersin maliyeti | "Neredeyse gecti" psikolojisiyle esik dusurulurse gate anlami kaybolur; esik, kanitin kendisidir |
| Kanit nasil dogrulanir | `scripts/count-components.ps1` yeniden calistirilir: kayit sayisi >= 5000 ve hatali format 0 olmali (2026-09-29: 5440 PASS) |
| Kapanis kosulu | Esik yeniden olculup marj acilana kadar her katalog degisiminde bu sayim tekrarlanir |

### §1.3 Kanit turleri (hangi kanit gecerli)

| # | Kanit turu | Ornek cikti | Gecerlilik siniri |
|---|---|---|---|
| 1 | **Olcum** (boyut/sayac) | `count-components.ps1` -> 5440 kayit, 0 hatali format, PASS | Cikti tarihi gecerlidir; ertesi gun yeniden olculur |
| 2 | **Tarama** (link/mojibake/frontmatter) | `check-links.ps1` -> BROKEN=0 · `find-mojibake.ps1` -> 0 dosya | Kapanis anina kadar; dosya eklenince tekrarlanir |
| 3 | **Dizin sayimi** | recursive walk: 177 MD (bu iki dosya oncesi) | Anlik goruntudur, plan degildir |
| 4 | **ADR satiri** | ADR-028 §2.3 = 6 zorunlu duzeltme | Karar degisene kadar; degisince supersede |
| 5 | **Rapor satiri** | `reports/index.md` — 9 dosya, 8 konu + indeks | Rapor guncellenene kadar; uzerine yeni satir |

### §1.4 Ders -> kapi eslesmesi (hangi ders hangi gecidi durdurur)

Her ders tek bir kapiya baglidir; kapida sorulan soru degil, **o dersin kaniti**
yanitlanir. Asagidaki esleme, "bu ders neden onemli" sorusunu geciktirme
maliyetine cevirir: ders kapanmazsa kapi acilmaz, kapi acilmazsa sonraki
tum batch beklemede kalir.

| Ders | Hangi kapiyi durdurur | Kapida istenen kanit | Kanit yoksa |
|---|---|---|---|
| L-01 (618 envanter) | katalog uretimi / sayim bolumu | tek sayim birimi `N###` + `count-components.ps1` PASS | 618 listesi raporda kalir, kataloga girmez |
| L-02 (tek matris) | NetArchTest `LayerRulesTests` | 8x8 matris dosyasi + test ciktisi | baglanti karari `UNKNOWN` sayilir |
| L-03 (proje sayisi) | yeni `.csproj` kapi gecisi | proje sayisi + hedef (TFM) sayisi tablosu | proje arttirimi durur |
| L-04 (mojibake) | GATE-004a kapanisi | `find-mojibake.ps1` = 0 dosya | kapi acik kalir, zincir kirmizi |
| L-05 (bozuk link) | GATE-004a kapanisi | `check-links.ps1` = 0 bozuk | kapi acik kalir, zincir kirmizi |
| L-06 (anlik envanter) | her rapor turu | tarihli + yontemli olcum | sayi gecersiz sayilir |
| L-07 (surum kilidi) | NuGet ekleme / `.csproj` | paket listesi + TFM | paket eklenmez, RED |
| L-08 (marjsiz esik) | gate soru 5 (esik) | kayit >= 5000 ve hatali format 0 | esik dusurulmez, gate kirmizi |

---

## §2 Tekrar Eden Karar Desenleri (ADR referansli)

Ayni sekiller tekrar tekrar ayni sonuca variyor: EF Core her seferinde reddediliyor, kopya her seferinde eleniyor, sayim her seferinde tek birime iniyor. Bu bolum bu **sekilleri** kaydeder; deseni taniyan, ilgili ADR'ye gider, gerekceyi yeniden uretmez.

### §2.1 Desen tablosu

| ID | DESEN (ne tekrar ediyor) | ADR KAYITLARI | NEREDE TEKRARLANIR | AYKIRILIKTA |
|---|---|---|---|---|
| P-01 | Veri erisimi her seferinde **EF Core disi** cikiyor: ADO.NET + Dapper, her sorgu parameterized, migration saf SQL | ADR-005 (ilk karar) + ADR-016 (yeniden ifade) — kayit `.decisions/index.md` §1 | Repository/handler yazimi, `.sql/` dosyasi, `spec/` sorgu bolumu | G2 RED — kod EF Core ile yazildiysa iade |
| P-02 | Icerik **her seferinde sifirdan** yaziliyor: yedek vault'tan kopya yok, yalniz yapi referansi aliniyor | ADR-002 + ADR-024 (CoreMusic vault degistirilemez) | Yeni belge batch'i, sablon doldurma, rapor uretimi | G1 RED — benzerlik taramasinda yabanci blok |
| P-03 | Sayim **her seferinde tek birime** iniyor: katman dosyasi = `N###`, bileşen kaydi tek esik (5440) | ADR-027 (5000+ esik) + ADR-028 (tek sayim birimi, Tier dis) | Katalog uretimi, bileşen listesi, gate soru 5 | Gate RED — ikinci birim (Tier-DLL sutunu) uretilirse |
| P-04 | Surum **her seferinde ayni kilitte**: DevExpress v26.1 + `RibbonControlStyle.Office2010`, TFM `net48` + `net10.0-windows` | ADR-004 + ADR-001 + ADR-012 (iki bacak veri saglayicisi) | NuGet ekleme, `.csproj`, form tasarimi, ribbon stili | G3/G6 RED — paket listesi taramasinda sapma |
| P-05 | Kapı **her seferinde ayni yerde duruyor**: onay yoksa kod yok, commit yalniz kapıda | ADR-025 (GATE-003 hard stop) + ADR-022 (ilk commit GATE-004) | Kodlama baslarken, `git add` oncesi | G12 RED — onaysiz `.cs` geri alinir |

### §2.2 Desen tanima testi (4 soru)

| # | Soru | "Evet" ise |
|---|---|---|
| 1 | Bu karar en az **iki ADR'de** ayni sekilde tekrar mi ediyor? | Desen var → §2.1 tablosuna bak, gerekceyi yeniden yazma |
| 2 | Karara muhalefet **ayni argumanla** mi geliyor? (or. "EF Core hizlandirir") | Cevap ADR'de hazir; arguman yeniden uretilmez |
| 3 | Karar **gate metnine** bagli mi (G1..G12 / GATE-004a)? | Kural metni `.rules/` ve [CLAUDE.md](CLAUDE.md)'de zorunludur |
| 4 | Yanlisinin **duzeltme maliyeti** yuksek mi (geri alma gunlerce)? | Yeni ADR acma esiği duser → §4 |

### §2.3 Desen ne zaman yeniden acilir

| Durum | Yapilan | Yapilmaz |
|---|---|---|
| Yeni kisit geldi (or. net48 tamamen kalkti) | Yeni ADR `draft` + defter satiri; eski ADR `superseded` | Eski ADR silinmez, ustune yazilir |
| Kanit yalan soylemeye basladi (olcum esigi tutmuyor) | Olcum tekrarlanir, sonuc `log.md`'ye satir olur | Sayi tahminle duzeltilmez |
| Desen celiskide (iki ADR ayni konuda farkli) | `CONFLICT DETECTED` + tek soru ile karar beklenir | Ikisi de "gecerli" diye birakilmaz |

### §2.4 Desen -> dosya esleme (nerede aranir)

Desen tanindiktan sonra dosya aramak zaman kaybidir; asagidaki esleme deseni
**oldugu yere** goturur. "Nereye bakilir" sutunu is baslamadan once ilk acilacak
dosyadir; "nasil teyit edilir" sutunu ise desenin hala gecerli oldugunu kanitlar.

| Desen | Once bakilacak dosya | Teyit yolu | Teyit sonucu ne olmali |
|---|---|---|---|
| P-01 (EF Core disi veri) | `.rules/index.md` + [CLAUDE.md](CLAUDE.md) G2 | Yeni `.cs` icinde `DbContext`/` DbSet` aranir | 0 eslesme; sorgular parameterized |
| P-02 (sifirdan icerik) | `.templates/new-doc-contract.md` §4-§5 | Benzerlik taramasi yapilir | Yabanci blok 0; yalniz yapi referansi |
| P-03 (tek sayim birimi) | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.2 | `count-components.ps1` calistirilir | Kayit >= 5000, hatali format 0, tek birim `N###` |
| P-04 (surum kilidi) | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.3 madde 1 (CPM) | Paket listesi + `.csproj` TFM bakilir | v26.1 + `net48`/`net10.0-windows`; 24.2.x yok |
| P-05 (kapi kurali) | [WORKFLOW.md](WORKFLOW.md) (gate tarifesi) | `git log` + onay kaydi aranir | Onaysiz commit 0; her commit bir gate satiri |

### §2.5 Desen disi kararlar (bunlar §2'ye girmez)

Her tekrar eden goruntu desen degildir; asagidakiler **olay**dur, kayitlari
[log.md](log.md) §2'dedir ve gerekce uretmezler. Karistirilirsa §2 ADR defteri
olmaktan cikar, gunluge doner (SSUT ihlali).

| Goruntu | Neden desen degil | Nereye yazar |
|---|---|---|
| Ayni gun 3 olcum ayni sayiyi veriyor | Sonuc ayni, karar degil; tekrar eden **dogrulama** | [log.md](log.md) satiri |
| Bir dosya iki kez yazildi (taslak + nihai) | Surec olayi, kalici ogrenim degil | [log.md](log.md) satiri |
| Gate 2 oturum boyunca acik kaldi | Gecici durum; kapali hale gelince olay kapanir | [log.md](log.md) + devir notu |
| Tek ajanin tercihi (arac, bicim) | Kesisim yok; en az 2 ADR kosulunu karsilamaz | §5 kayit satiri |

### §2.6 Desen ihlalinin maliyeti (neden tekrar ediyor)

Desenler pahali ogrenildigi icin tekrar ediyor; asagidaki maliyet sutunu, desenin
niye "tercih" degil **kural** oldugunu gosterir. Maliyet dusukse desen zaten §4
esik testinden gecmezdi — yani bu tablodaki her satir ya bir geri alma ya da bir
gate RED ile olculmustur.

| Desen | Ihlal edilirse | Olcumulen maliyet | Toparlanma |
|---|---|---|---|
| P-01 (EF Core disi) | migration + EF paketi gecersiz kalinir | G2 RED: kod iade, paket listesi yeniden yazim | `.csproj` geri alinir |
| P-02 (sifirdan icerik) | yabanci blok benzerlik taramasinda yakalanir | G1 RED: dosya silinir, is iki kez yapilir | yeniden yazim (tam batch) |
| P-03 (tek sayim birimi) | ikinci birim (Tier-DLL) uretilir | staleness iki kat; esik marji 0'dan negatife | katalog + sayac reset |
| P-04 (surum kilidi) | 24.2.x paketi eklenir | G3/G6 RED; iki bacak veri saglayicisi sapar | paket kaldirma + test |
| P-05 (kapi kurali) | onaysiz `.cs` commit edilir | G12 RED: commit geri alinir, zincir kirmizi | `git revert` + kanit kutusu |

---

## §3 Yapilmamasi Gerekenler (kara liste)

Her madde **gerçek bir olayla** kanitlanmistir; sebebi bilinmeyen yasak bir sonraki oturumda sessizce ihlal edilir. Kara listeye madde eklemek icin tek kosul: olmus bir olay + olcum/rapor kaniti.

| # | YAPMA | GERCEK OLAY (neden burada) | KANIT | KARSILIGI |
|---|---|---|---|---|
| 1 | K3'in **9 Tier / 618 DLL** envanterini kataloga, N-ID'lere veya sayim bolumune almak | 618 giris tek tek sayildi: 2 birebir tekrar, 4 DLL dosyada yok, 9 basliktan 2'si yanlis, §15 "nihai degil" diyor | [reports/k3-618-ham-envanter.md](reports/k3-618-ham-envanter.md) | 618 listesi yalniz rapor satiri; katalogda tek birim `N###` |
| 2 | **Tier basina ayri csproj** onermek (575 proje, 1150 hedef) | Restore, CPM ve Visual Studio bu hedef sayisinda coker; ustune K3'in kapsim disi DLL'leri tasinir | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §3 A3 (UNIVERSAL RED) | 9 uretim + 1-2 test = 22 hedef |
| 3 | Baska vault'tan **icerik kopyalamak** (CoreMusic / yedek) | Yedek kök dosyalar 1-16 KB, ayni vault'ta brain dosyasi 80 KB ile 2 KB arasinda tutarsiz — kopya kaliteyi de bozar | ADR-002 + `.templates/new-doc-contract.md` §4/5 | Yalniz **yapi** referansi, icerik sifirdan |
| 4 | Olcum ciktisi olmadan **"bitti/tamamlandi"** demek | "Hazir" denirken 46 bozuk link ve 9 mojibake dosya acikti; iddia ile gerceklik farki raporda goruldu | `reports/10-bilinen-eksikler.md` §0-§1 | `dotnet build` / tarama ciktisi + [log.md](log.md) satiri |
| 5 | Dosyaya **Unicode kutu cizimi veya bozuk byte** sokmek | Mojibake taramasi 9 dosyada bozuk byte buldu; kutu cizimi `U+2500-U+257F` G4'te yasak | [CLAUDE.md](CLAUDE.md) G4 + `scripts/find-mojibake.ps1` | Diyagram yalniz `|` `-` `+`; metin UTF-8 |

### §3.1 Etiket sozlugu (kara liste tamamlayicisi)

| Etiket | Ne anlama gelir | Nereye yazilir |
|---|---|---|
| `DOĞRULAMA GEREKLİ` | Bilgi var ama kaniti yok (olcum/rapor eksik) | Bulundugu satirin sonu + `log.md` devir notu |
| `UNKNOWN` | Cevap henuz verilmemis; tahminle doldurulmaz | Karar bekleme listesi |
| `CONFLICT DETECTED` | Iki dosya ayni konuda celisiyor | Iki dosya + karar zamani (gate) |
| `PASS` / `RED` | Olcum esigi tuttu / tutmadi | Kanit kutusu (kutu bicimi `WORKFLOW.md` §2) |

### §3.2 Kapsam disi ve izlenenler (yasak degil, dikkat)

Kara liste kisitlar; bu tablo ise **henüz karar verilmemis** konulari izler. Kara
listeye almamak bilgi eksikliginden degil, olay yasamis olmamasindandir; asagidaki
her satir kanit toplamayi bekler ve kanit gelince §1'e ders ya da §2'ye desen olur.

| Izlenen konu | Simdiki durum | Kanit aranan sey | Gectiginde nereye |
|---|---|---|---|
| Mojibake'de kalan 1 dosya | 8/20 duzeltildi, kalan teyitsiz | `find-mojibake.ps1` = 0 ciktisi | §1 L-04 kapatisi |
| `architecture/` 10 bos klasor | Dizin var, dosya 0 | ilk mimari dosyanin olusturulmasi | §5 yeni OLCUM satiri |
| `knowledge/` · `prompts/` · `playbooks/` icerigi | Yalniz `index.md` | ilk icerik dosyasi + link taramasi | §5 yeni OLCUM satiri |
| Port islemleri kaniti | Yapildi denildi, kanit yok | port listesi + tarih + komut ciktisi | §5/#14 kapanisi |
| ADR defteri bayatligi | 28 dosya vs 5/7 satir | iki defterden birinin guncellenmesi | §5/#11 kapanisi |

---

## §4 Hafiza Guncelleme Kurali (ne zaman yazilir, ne zaman ADR olur)

Bilgi **oldugu anda** yola yazilir; oturum belleğine gomulmez. Ama hangi dosyaya yazilacagi **esikle** belli olur: tek kullanimlik gozlem §5 kayit satiri olur, tekrar eden ve yanlisinin pahali oldugu karar ADR olur, zorunlu hale gelen davranis `.rules/` + [CLAUDE.md](CLAUDE.md) olur.

### §4.1 Bes tetikleyici

| # | Tetikleyici | Nereye yazilir | Ne zaman | Sahip |
|---|---|---|---|---|
| 1 | Olculmus bir cikti (sayi, tarama, build) | [log.md](log.md) — tek satir, ayni gun | Is biter bitmez | isin yapan ajan |
| 2 | "Bunu denedik, su cikti" (ders) | bu dosya §1 (karti ile) | Ikinci kez gerekce aranirsa | architect |
| 3 | Tekrar eden, yanlisinin pahali oldugu karar | ADR (defter: [.decisions/adr-index.md](.decisions/adr-index.md)) | §4.3 esik testi gecerse | architect |
| 4 | Artik zorunlu olan davranis | [CLAUDE.md](CLAUDE.md) + `.rules/index.md` (gerekce ADR'de) | Kural ihlali gozulduyse | architect |
| 5 | Guncel durum (part/gate/git) | bu dosyada **degil**; durum tablosu [VISION.md](VISION.md)/[PROJECTS.md](PROJECTS.md), gecmis [log.md](log.md) | Her oturum sonunda | isin yapan ajan |

### §4.2 ADR esik testi (dort soru, hepsi EVET olmali)

| # | Soru | Hayir ise |
|---|---|---|
| 1 | En az **2 dosya** bu karara bagimli mi? | Tek dosyalik tercih → ADR acilma, §5 kayit satiri |
| 2 | Bir ay sonra hala gecerli olacak mi? | Gecici not → `log.md` |
| 3 | Yanlisinin **duzeltme maliyeti** yuksek mi? | Dusuk maliyet → deneyip `log.md`'ye sonuc yaz |
| 4 | "Neden boyle yaptik" sorusunun cevabi **sonradan** gerekecek mi? | Gerekmiyor → kara listeye/§1'e tek satir |

### §4.3 Kayit bicimi (bu dosya §5 icin)

```text
| # | TARIH | KAYIT (ne oldu - tek cumle) | KANIT (dosya + sayi/cikti) | SINIF |
SINIF degerleri: DERS | KARAR | OLCUM | UYARI | IS
Kural: KANIT hucresi bos satir YAZILMAZ; bilgi eksikse
       "DOĞRULAMA GEREKLI - <ne eksik>" yazilir.
```

| Karistirma | Yanlis | Dogru |
|---|---|---|
| Ders mi, karar mi | Dersi `brain.md`'ye yazmak | Ders §1, gerekce ADR/brain |
| Durum mu, sabit mi | Durumu bu dosyaya gommek | Durum anlik tablo, sabit §1/§3 |
| Kayit mu, kural mi | Gozlemi kural gibi yazmak | Kural yalniz gercek ihlal sonrasi |
| Eski satiri duzeltme | Ustune yazmak | Yeni satir + `DOĞRULAMA GEREKLİ` notu |

---

## §5 Kayit Defteri (tarihli satirlar)

Tek gun, tek defter: **2026-09-29**. Asagidaki 22 kayit bu gunun olcum ve isleridir; her satir kendi kanitini tasir. Gelecek kayitlar **bu tablonun dibine** eklenir, usttekiler silinmez (append-only; detayli kronoloji [log.md](log.md)'dedir).

| # | TARIH | KAYIT | KANIT | SINIF |
|---|---|---|---|---|
| 1 | 2026-09-29 | Katmanli mimari baz karari kilitlendi: 8 makro katman (l0..l7) + 44 alt katman = 52 dugum, 9 uretim + 1-2 test projesi, cift TFM | [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 — 9 satirlik katman/proje tablosu | KARAR |
| 2 | 2026-09-29 | Mimari tartisma 3 turda 21 ajanla kapatildi (10 senior -> 5 senior -> 3 junior + 3 ajans); son tur 6/6 sartli kabul | ADR-028 §3 — oylama sutunlari (4/10, 4/5, 6/6) | KARAR |
| 3 | 2026-09-29 | K3'un 618 DLL envanteri kapsam disi ilan edildi; vault kataloguna girmez, yalniz rapor satiri olarak yasar | [reports/k3-618-ham-envanter.md](reports/k3-618-ham-envanter.md) + ADR-028 §2.2 | KARAR |
| 4 | 2026-09-29 | Ilk build oncesi 6 zorunlu duzeltme kilitlendi (CPM paketleri, 8x8 matris, "Sekiz katman" ifadesi, N<->klasor eslesmesi, gercek tarama yolu, TFM + l0 portlari) | ADR-028 §2.3 — 6 satir, her satirda kanit kosulu | KARAR |
| 5 | 2026-09-29 | Vault olcumu: bu iki dosya yazilmadan **177 MD**; kirilim `.templates/` 35 · `spec/` 40 · `checklists/` 31 · `.decisions/` 30 · `.rules/` 17 · `reports/` 9 | recursive dizin sayimi (bugun, ilk tur); klasor bazli sayim satirlari | OLCUM — guncel hali #22 (180) |
| 6 | 2026-09-29 | Kok durum (ilk olcum): 13/14 — `engine.md` bugun 16:42'de olustu (35.834 B), `ULTRA-THINKING.md` o an diskte yoktu, `MEMORY.md` bu isle yazildi | dizin listesi + `get_file_info` ciktisi | OLCUM — guncel hali #22 |
| 7 | 2026-09-29 | `.rules/` olgunlasti: 16 kural dosyasi + indeks = 17 dosya (eski rapor "4 dosya" diyordu) | dizin listesi (bugun) | OLCUM |
| 8 | 2026-09-29 | `checklists/` 31 dosyaya cikti — gate-003/004/004a/005 dosyalari artik diskte (eski rapor 4'tu) | dizin listesi (bugun) | OLCUM |
| 9 | 2026-09-29 | `knowledge/` · `prompts/` · `playbooks/` icinde yalnizca indeks var (icerik 0), `ui-design/` ve `.agents/` **bos** | dizin listesi (bugun) — 0 giris | UYARI |
| 10 | 2026-09-29 | Mimari klasorleri olustu ama bos: 10 alt klasor (`00-overview`, `catalog`, `l0..l7`) + indeks, iclerinde dosya 0 | [architecture/index.md](architecture/index.md) + dizin listesi | OLCUM |
| 11 | 2026-09-29 | ADR defteri bayat: diskte **28 ADR** varken `adr-index.md` 5 satir, `.decisions/index.md` 7 satir gosteriyor (iki defter = iki gercek) | KAPANDI 2026-10-01: `adr-index.md` = 29 satir SSOT, `.decisions/index.md` pointer'a cevruldu (log.md #28) | ✅ COZULDU |
| 12 | 2026-09-29 | Mojibake onarimi: **8 dosya / 20 eslesme** duzeltildi; onceki olcum 9 dosyaydi, kalan 1 dosya icin tarama tekrarlanmali | 2026-09-29 duzeltme kaydi + `reports/10-bilinen-eksikler.md` §1 (onceki 9 dosya) | IS — kalan `DOĞRULAMA GEREKLİ` |
| 13 | 2026-09-29 | Link taramasi calistirildi; onceki rapor 1516 iyi / 46 bozuk link diyor, yeni cikti rapora yazilmadi | `scripts/check-links.ps1` + `reports/index.md` (onceki sayilar) | IS — yeni sayi `DOĞRULAMA GEREKLİ` |
| 14 | 2026-09-29 | Port islemleri yapildi (ortam/erisim hazirlik); kanit kutusu bu oturumda dolmadi | kanit yok → `DOĞRULAMA GEREKLİ` (port listesi + tarih iste) | IS |
| 15 | 2026-09-29 | Vault kurulumu raporlari guncel okundu: 52 dugum katalog + 5440 bilesen kaydi PASS (esik >=5000, hatali format 0) | `reports/index.md` + katalog sayimi (2026-09-25 olcumu, bugun yeniden sayilmadi) | OLCUM |
| 16 | 2026-09-29 | `MEMORY.md` bu dosya olarak sifirdan yazildi: 8 ders + 5 desen + 5 kara liste + 22 kayit, bant 30-88 KB | olcum komutu §6.3/1 — boyut bandi icerisinde (yazim sonrasi ~34 KB) | IS |
| 17 | 2026-09-29 | `log.md` ayni isle sifirdan yazildi (append-only gunluk, 4 satirli kanit kutusu ornegi ile) | olcum komutu §6 — `log.md` boyutu | IS |
| 18 | 2026-09-29 | En buyuk iki klasor `.templates/` (35 MD) ve `spec/` (40 MD) olmus; ikisi vault'un yarisi — sablon ve spesifikasyon uretimi ana kaynak | recursive dizin sayimi (bugun) | OLCUM |
| 19 | 2026-09-29 | `scripts/` 5 `.ps1` tasiyor (count-components, check-links, find-mojibake, measure-vault + 1); olcum araclari vault'ta, harici degil | dizin listesi (bugun) — 5 giris | OLCUM |
| 20 | 2026-09-29 | Navigasyon kokunde `index.md` 15.693 B ile en kucuk degil, en guncel baglayici dosya; 26 izinli link hedefinin tamami burada tanimli | dizin listesi + `get_file_info` ciktisi | OLCUM |
| 21 | 2026-09-29 | WORKFLOW gate zinciri durumu: GATE-004a **ACIK** (`ULTRA-THINKING.md` o an diskte yoktu), 003 -> 004 -> 005 -> 006 beklemede | [WORKFLOW.md](WORKFLOW.md) §gate + dizin (o an 13/14) | UYARI |
| 22 | 2026-09-29 | **Yeniden sayim:** `ULTRA-THINKING.md` olustu (kok artik **14/14**), toplam **180 MD** (177 + bu iki dosya + yeni dosya); GATE-004a nedeni degisti: dosya var, icerigi/erisimi teyitsiz | recursive dizin sayimi (gun ici ikinci olcum) + dizin listesi | UYARI — `DOĞRULAMA GEREKLİ` (icerik okunmali) |

**Defter kurali:** yeni kayit = bu tabloya 1 satir; duzeltme = **yeni satir** (eski satir durur); kanit yoksa hucre `DOĞRULAMA GEREKLİ` ile kapanir. Sutun disi bilgi (gerekce, sure) §5'e girmez — o [log.md](log.md) veya ADR isidir.

---

## §6 Dogrulama

### §6.1 Biçim

- [ ] Frontmatter `title, type, category, status, version, updated, authority, owner, scope` — ilk satir `---`, `updated: 2026-09-29`, `authority: SSOT`
- [ ] Tek H1 + **Zorunlu Baglantilar** satiri + `> Tek cumlelik sorumluluk` + `## §0 Ozet (Answer First)`
- [ ] Bolumler `## §1..§6`; en az **3 tablo + 1 ASCII kutu** (`|` `-` `+`; Unicode kutu cizim karakteri YOK — `U+2500` ve tarzlari)
- [ ] Madde listeleri en fazla 5 madde; tablo satirlari olcum/kanit icindir
- [ ] UTF-8 Turkce; mojibake ornegi yok (`scripts/find-mojibake.ps1` = 0 hedefi)

### §6.2 Icerik

- [ ] §1'de **8 ders**; her satirda KANIT sutununda dosya adi + icindeki sey var (bos hucre = ders degil)
- [ ] §2'de **5 desen**, her desen en az iki ADR kaydina bagli
- [ ] §3'de **5 kara liste** maddesi, her madde gercek olayla kanitlanmis
- [ ] §5'te **en az 15 kayit**, tamami `2026-09-29`; KANIT sutunu dolu ya da `DOĞRULAMA GEREKLİ`
- [ ] Linkler yalniz izinli hedeflere (`index.md` · `CLAUDE.md` · `WORKFLOW.md` · `ROLE.md` · `AGENTS.md` · `brain.md` · `keys.md` · `glossary.md` · `VISION.md` · `log.md` · `engine.md` · `architecture/index.md` · `reports/index.md` · `reports/k3-618-ham-envanter.md` · `.decisions/adr-index.md` · ADR-028 · `checklists/layer-boundary-check.md` · `.templates/index.md` · `.templates/new-doc-contract.md` · `spec/index.md` · `.rules/index.md` · `knowledge/index.md` · `playbooks/index.md` · `prompts/index.md`)

### §6.2.1 Izinli link hedefleri (26) — bu dosyada kullanilanlar

| # | Hedef | Bu dosyada nerede | Durum |
|---|---|---|---|
| 1 | `index.md` | Zorunlu Baglantilar | kullanildi |
| 2 | `CLAUDE.md` | §0, §1.2 (L-04, L-08), §2.1, §4.1 | kullanildi |
| 3 | `WORKFLOW.md` | §0, §1.2 (L-08), §3, §6.3 | kullanildi |
| 4 | `ROLE.md` | Zorunlu Baglantilar | kullanildi |
| 5 | `AGENTS.md` | Zorunlu Baglantilar | kullanildi |
| 6 | `brain.md` | §0, §4.3 | kullanildi |
| 7 | `keys.md` | Zorunlu Baglantilar | kullanildi |
| 8 | `glossary.md` | Zorunlu Baglantilar | kullanildi |
| 9 | `VISION.md` | §0, §4.1 | kullanildi |
| 10 | `log.md` | §0, §2.5, §3.2, §4.1, §5 | kullanildi |
| 11 | `engine.md` | §0 (gate tarifesi satiri) | kullanildi |
| 12 | `architecture/index.md` | §5/#10 | kullanildi |
| 13 | `reports/index.md` | §1.3, §5/#15 — duz metin (link yok) | metin |
| 14 | `reports/k3-618-ham-envanter.md` | §1.1 (L-01), §3/1 | kullanildi |
| 15 | `.decisions/adr-index.md` | §0, §1.2 (LERS-04), §4.1, §5/#11 | kullanildi |
| 16 | ADR-028 | §1.1, §1.2, §1.4, §2.1, §3/2, §5 | kullanildi |
| 17 | `checklists/layer-boundary-check.md` | §6.2 izinli liste — link acilmadi | metin |
| 18-19 | `.templates/index.md` · `.templates/new-doc-contract.md` | §1.2 (L-05), §3/3, §5/#12 — duz metin | metin |
| 20 | `spec/index.md` | §5/#18 — duz metin | metin |
| 21 | `.rules/index.md` | §1.1 (L-04), §1.2 (LERS-04), §4.1 — duz metin | metin |
| 22-24 | `knowledge/` · `playbooks/` · `prompts/` `index.md` | §3.2, §5/#9 — duz metin | metin |
| 25 | `PROJECTS.md` | §4.1 (durum tablosu satiri) | kullanildi |
| 26 | `MEMORY.md` (kendisi) | kullanilmiyor — kendine link yok | bos |

**Link sayaci:** 52 link / 16 farkli hedef; kalan 10 hedef **duz metin** olarak
gecti (link yok → kirik link uretilmedi). Hedefi olmayan hicbir link yazilmadi.

**Yasak:** bu 26 disi hedef (or. `ULTRA-THINKING.md` — diskte var ama izinli link
hedefi **degil**), dis web adresi ve e-posta adresi linki — hepsi kirmizi; ayrinti
`.templates/new-doc-contract.md` §3.

### §6.3 Olcum (komutlar)

```powershell
# 1) Bu dosyanin boyutu (hedef 30-88 KB)
(Get-Item 'C:\versacoder\.ai\MEMORY.md').Length / 1KB

# 2) Kok sayimi (hedef 14; 2026-09-29 son olcum 14/14 - ULTRA-THINKING.md olustu)
Get-ChildItem 'C:\versacoder\.ai' -File -Filter *.md | Measure-Object | Select-Object -Expand Count

# 3) Mojibake / bozuk link (GATE-004a kapanisi)
pwsh -File C:\versacoder\.ai\scripts\find-mojibake.ps1
pwsh -File C:\versacoder\.ai\scripts\check-links.ps1
```

| Kalem | Hedef | Durum (2026-09-29) |
|---|---|---|
| Boyut bandi | 30–88 KB | **band icerisinde** — §6.3/1 komutu ile her seferinde olculur |
| Kok belge | 14/14 | **14/14** (`ULTRA-THINKING.md` olustu; icerik teyitsiz) |
| Toplam MD | ADR-003 esigi >=500 | 177 (yazim oncesi) + bu iki dosya + yeni dosya = **180** |
| Bozuk link (bu dosya) | 0 | 52 link / 16 hedef — tarama **0 bozuk** |
| Mojibake (bu dosya) | 0 | `find-mojibake.ps1` ile dogrulanacak (dosya icinde desen 0) |

---

## §7 Baglanti Agu (tam capraz-link)

Bu dosyanin diger 18 vault dosyasina baglantilari (eksiksiz; hicbir baglanti silinmemistir):

| # | Dosya | Ne tasiyor |
|---|---|---|
| ULTRA | [ULTRA](ULTRA-THINKING.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.decisions/draft/ADR-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../README.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../CLAUDE.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../AGENTS.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../WORKFLOW.md) | kok kapi ozeti (gate sirasi tek satir) |
