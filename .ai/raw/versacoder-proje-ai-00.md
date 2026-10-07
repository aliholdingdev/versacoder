---
title: "Part 00 — 100 Partlık Analiz Seti İçindekileri ve Haritası"
type: analiz
category: eli10
version: "1.0.0"
status: active
authority: "Master Orchestrator"
updated: "2026-09-25"
---

# Part 00 — İçindekiler: 100 Partın Tam Haritası

> Bu dosya setin **ana kapağıdır**. 100 partın tamamı, 13 blok halinde, her part için
> tek satırlık açıklama ve okuma sırasıyla burada listelenmiştir.

## Bu partta ne öğreneceksin

Bu partı okuyunca 100 dosyalık setin **tamamını tek ekranda** göreceksin: hangi blokta
ne var, hangi part hangi konuyu anlatıyor, sırayla nereden başlanır, hattı zorunlu
kaynak satır aralığı: **kaynak — `_PLAN.md` satır 79–231 (100 PART LİSTE

## ELI10

**Analogi 1 — Dev bir kitabın içindekiler sayfası.**
Bir kitap okumaya başlarken önce kapağı açar, sonra içindekiler sayfasına bakarsın:
"1. Bölüm şurada, 7. Bölüm şurada." Bu dosya da öyle. VersaCoder hakkında yazılan
100 tane küçük kitapçık var; bu dosya hepsinin içindekiler sayfası. İçindekileri
okumak kitabı okumak değildir, ama kitabı **nereden okuyacağını** sana gösterir.
Acele edip atlamak, 700. sayfada "bu ne laf?" demene yol açar.

**Analogi 2 — Metro haritası.**
Bir şehre yeni geldin. Metroda hangi hatta binersen nereye gideceğini bilmezsen
otobüsle dolaşırsın. Bu setin 13 bloğu, 13 farklı metro hattıdır. Bazı hatlar kısa
(2 durak: Blok 11 — Çelişki Defteri), bazıları çok uzun (15 durak: Blok 2 — Ana
Prompt §1–§36). Harita, hangi hatta binip hangi durakta ineceğini söyler.

**Analogi 3 — IKEA kurulum kılavuzu.**
Mobilya kurarken önce klasördeki resimli listeye bakarsın: "1–6 arası parçalar,
7–12 arası bunlar." Parçaları yere sermeden önce listeyi görmek, işi yarı yarıya
kolaylaştırır. Bu dosya, VersaCoder analiz setinin "parça listesi"dir.

Set nasıl bir şey? VersaCoder, "sohbet eden bir yapay zekâ asistanı" değil; **kendi
kendine mühendislik yapan bir platform** hayalidir. Bu hayali anlatmak için 100 part
gerekti çünkü konu tek nefeste anlatılamayacak kadar katmanlı: bir yanda mimari
kurallar (SOLID, katmanlar), bir yanda ajan sürüsü (mimar ajanı, kod yazan ajan,
denetçi ajan), bir yanda onay kapıları (gate) ve bir yanda bellek kasası (.ai vault).

Her part, **ELI10** (10 yaşındaki birinin anlayacağı dil) ile başlar, sonra
**Adım adım** ile pratiğe iner, sonra **Derin dal** ile kaynak metnin gerçek
satırlarını tablo tablo önüne serer. Yani önce "hikâye", sonra "yol", sonra "kanıt".

Bu dosyanın görevi bu üç katmanı **sıraya koymak**. Sıra önemlidir: eğer doğrudan
Blok 6'ya (9 Tier / 618 DLL) gidersen, "bu DLL'ler neden var?" sorusunun cevabını
kaçırdığın için tablolar sadece isim yağmuruna dönüşür. Eğer Blok 0'ı atlayıp
Blok 4'e (40 kural) gidersen, kuralların **niye** yazıldığını bilmeden ezberlemeye
çalışırsın.

Setin kuralı basit: **önce niye, sonra ne, sonra nasıl.** Blok 0–1 "niye", Blok 2–5
"ne", Blok 6–8 "nasıl inşa edilir", Blok 9–12 ise "kurallar, çelişkiler ve gelecek".

Bir de "neredeyse her partın arkasında (set-özü partlar 00/01/07/08/09 hariç) bir kaynak
dosya var" gerçeği var. Bu set uydurma bilgi
üretmez: her derin dal bölümünde `kaynak: dosya satır X–Y` ya da `§` atfı bulunur.
Kaynakta olmayan bir şey yazmak yerine **YOK** ya da **DOĞRULAMA GEREKLI** der.
Bunu setin her yerinde göreceksin — bu bir eksiklik değil, bir disiplindir.

Son olarak: bu seti **defter gibi** kullanabilirsin. Part 03 (sözlük) her zaman
açık kalacak bir sayfa; takıldığın terimi orada ararsın. Part 95 (çelişki defteri)
ise "kaynaklar birbiriyle çelişince ne yapılıyor?"nun cevabıdır.

## Adım adım

1. **Önce bu dosyayı sonuna kadar oku (≈5 dakika).** 13 bloğun hangi işe yaradığını
   görmeden diğer partlara dalmak, haritayı okumadan yola çıkmaktır.
2. **Part 01'i oku (≈8 dakika).** Setin yazım şemasını (ELI10 + Derin Dal) orada
   öğreneceksin; her partın 9 bloktan oluştuğunu ve neden 500+ satır olduğunu anlarsın.
3. **Part 02–05'i sırayla oku (≈30 dakika).** VersaCoder nedir, terimler ne demek,
   neden var, kaynaklar hangi satırlarda — bu dördü olmadan kalan set yarım kalır.
4. **Hızlı yol istiyorsan** Part 02 → Part 04 → Part 58–60 → Part 99 zincirini izle
   (≈45 dakika): tanım, 7 problem, çözüm matrisi, final özeti.
5. **Derin yol istiyorsan** Blok 0'dan başla, blok blok ilerle; her blok sonunda
   "Bu blokta ne öğrendim?" diye bir satır kendi notuna ekle.
6. **Uygulama yolunda** (kod yazacaksan) Blok 3 (1→20 kural) ve Blok 4 (0→40 kural)
   senin çekirdeğindir; Part 33–55 arasını atlamadan oku.
7. **Kaynakla çapraz kontrol** yapmak istediğinde Part 05'e dön: 100 part ↔ kaynak
   eşleme tablosu hangi partın hangi satırları okuduğunu birebir verir.
8. **Çelişki görürsen** Part 95–96'ya git; ORM, framework, katman sayısı gibi
   bilinen çelişkiler orada listelenmiştir.
9. **Yeni bilgi ekleyeceksen** Part 97–98'deki "gelecek özellik" listelerine bak;
   aynı formatta (kapsam + kabul kriteri) ekleme yaparsan set tutarlı kalır.
10. **Bitirdiğinde** Part 99'daki 30 günlük uygulama planını çalıştır.

## Derin dal

> Bu bölümdeki her satır `_PLAN.md` "100 PART LİSTESİ" (satır 79–231) tablolarından
> alınmış, her parta tek satırlık açıklama eklenmiştir. `kaynak: _PLAN.md satır X–Y`.

### Genel bakış — 13 blok / 100 part

| Blok | Part aralığı | Part sayısı | Ana konu | Kaynak ağırlığı |
|---|---|---|---|---|
| 0 — Başlangıç | 00–09 | 10 | Setin kendisi: içindekiler, şema, tanım, sözlük | — (plan + K1, K2) |
| 1 — Kimlik & Roller | 10–17 | 8 | Künye, 18 rol, teknoloji yığını, dizinler, görev planı | K2 |
| 2 — Ana Prompt §1–§36 | 18–32 | 15 | Master Analysis Prompt'un 36 bölümü | K1 A |
| 3 — ANA TALİMAT 1→20 | 33–41 | 9 | Sıralı çalışma adımları + mutlak kurallar | K1 C |
| 4 — Kural Seti 0→40 | 42–55 | 14 | 41 maddelik kural kataloğu | K1 D |
| 5 — Şablonlar & Anlatı | 56–64 | 9 | Rapor şablonları + sistem anlatısı (§3–§5) | K1 B, E, F, G |
| 6 — Mimari 9 Tier | 65–74 | 10 | Tier 0→8, 618 DLL ve sayı çelişkileri | K3 |
| 7 — Wizard & Gelecek | 75–81 | 7 | Wizard, debugger, Agent Inc, prompt sentezi | K1 H, I, J |
| 8 — Ücretsiz Modeller | 82–87 | 6 | Free model havuzu, router, C# istemci | K1 K |
| 9 — Kurallar & Referans | 88–91 | 4 | 16 adım, Zero-Hallucination, repo havuzu, kararlar | K4, K5, K6 |
| 10 — Vault Köprüsü | 92–94 | 3 | Vault şablonu, 14 kural, .ai kurulum sırası | K7, K2 |
| 11 — Çelişki Defteri | 95–96 | 2 | Tüm çelişkiler + çözüm planı | 7 dosya |
| 12 — Gelecek & Final | 97–99 | 3 | Eksikler, ürün vizyonu, 30 günlük plan | — |

`kaynak: _PLAN.md satır 81–231`

### Blok 0 — Başlangıç (part 00–09) — `kaynak: _PLAN.md satır 81–93`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 00 | part-00-index.md | Setin içindekileri: 13 blok, 100 part, okuma sırası — yani şu an okuduğun dosya. |
| 01 | part-01-nasil-okunur.md | Her partın 9 bloktan oluştuğu şema, ELI10/Derin Dal katmanı, 3 okuma yolu. |
| 02 | part-02-versacoder-nedir.md | Tek cümlede VersaCoder tanımı + agentic IDE, multi-agent swarm, 50+ katman. |
| 03 | part-03-sozluk.md | 90 madde (ana liste 6 tablo: 13+16+20+12+23+6): vault, SSOT, gate, DLL, Tier, MCP, wizard… ELI10 + derin karşılık. |
| 04 | part-04-neden-var.md | 7 problem: mimari erozyon, context degradation, belleksiz AI… ve çözüm matrisi. |
| 05 | part-05-kaynak-haritasi.md | 7 kaynak dosyanın rolü + ai.02 blok haritası A–K + part↔satır eşlemesi. |
| 06 | part-06-uc-prompt-iliskisi.md | init-00 (anayasa) ↔ ai.02 (plan) ↔ mimari-02 (iskelet) nasıl besleniyor. |
| 07 | part-07-yol-haritasi.md | 13 blok / 100 part planı, hangi blokta ne var, üretim sırası. |
| 08 | part-08-eli10-kurallari.md | ELI10 yazım kuralları: analogi, adım, "şimdi ne yap" üçlüsü. |
| 09 | part-09-temel-kavramlar.md | Kod bilmeyen için: dosya, proje, DLL, katman, gate, onay ne demek. |

> **Not:** Part 00, 01, 07, 08, 09'un ayrı kaynağı `YOK` — bunlar `_PLAN.md`'den
> türetilmiş set-özü dosyalardır (plan "Kaynak: —" olarak işaretlemiştir).

### Blok 1 — Kimlik, Roller, Görev Planı (part 10–17) — `kaynak: _PLAN.md satır 95–105`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 10 | part-10-kunye.md | Künye tablosu: ürün adı VERSA CODER, hedef uygulamalar, dizinler, şirket, vault. |
| 11 | part-11-18-rol.md | MUTLAK ROL: 18 rolün tamamı ve "kod yazan chatbot değil" cümlesi. |
| 12 | part-12-uygulanan-roller.md | Uygulanacak 5 rol + 7 maddelik teknik karar hiyerarşisi (Doğruluk > Hız…). |
| 13 | part-13-senior-kararlar.md | 4 kapatılmış karar: iki bacak, yedek içerik, DevExpress kanunu, ADO.NET+Dapper. |
| 14 | part-14-teknoloji-yigini.md | Stack listesi + "her teknoloji GEREKLİ/GEREKSİZ diye ayrılır" kuralı. |
| 15 | part-15-dizinler-yollar.md | C:\versacoder, install dir, G: notlar, DOKUNULMAZ coremusic vault'u. |
| 16 | part-16-gorev-plani.md | PART 1→8 görev planı: kaynak okuma, vault kurulumu, UI, iskelet — sırayla. |
| 17 | part-17-acik-sorular.md | Açık sorular: SqlClient 5.x, 52 katman/5000 kayıt, 7 ekran, GATE-004, LLM. |

### Blok 2 — Ana Prompt §1–§36 (part 18–32) — `kaynak: _PLAN.md satır 107–124`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 18 | part-18-akis-diyagrami.md | ASCII akış (USER→…→IMPLEMENTATION) + §1 SYSTEM ROLE ve 5 dürüstlük etiketi (kaynakta 6. yok). |
| 19 | part-19-amaç-ve-proje.md | §2 ANA AMAÇ 11 adım (önce anla, sonra kodla) + §3 PROJECT teknoloji listesi. |
| 20 | part-20-referans-erisim.md | §4 referans projeden yasaklar + §5 ACCESS VALIDATION (BLOCKED) akışı. |
| 21 | part-21-referans-analiz.md | §6 9 analiz alanı (kaynak) / 10 (plan) — part-21'e göre: Repository'den UI/UX'e kadar hangi alanlar taranır. |
| 22 | part-22-evidence-rule.md | §7 kanıt formatı: ID/SOURCE/LOCATION/OBSERVATION/EVIDENCE/CONFIDENCE/STATUS. |
| 23 | part-23-web-ve-kaynak-dogrulama.md | §8 web araştırma önceliği (5 kademe) + §9 CONFLICT / VERIFICATION işaretleri. |
| 24 | part-24-ikinci-beyin.md | §10 .AI SECOND BRAIN + §11 kalite kuralı: 500 satır hedef, dolgu yasak. |
| 25 | part-25-gereksinim-ve-soru.md | §12 REQ-ID formatı ve P0–P3 + §13 Asking Questions Engine döngüsü. |
| 26 | part-26-varsayim-ve-mimari.md | §14 NEVER ASSUME 17 madde + §15 önerilen 15 DLL listesi. |
| 27 | part-27-bagimlilik-solid.md | §16 dependency flow (UI→Application→Domain) + §17 SOLID/DRY/KISS/YAGNI. |
| 28 | part-28-devexpress-agent.md | §18 DevExpress yalnız UI + §19 AI AGENT ARCHITECTURE 9 bileşen. |
| 29 | part-29-session-wizard-mcp.md | §20 SESSION 8 sorumluluk + §21 WIZARD akışı + §22 MCP 10 kontrol. |
| 30 | part-30-plugin-memory-decision.md | §23 PLUGIN 8 değerlendirme + §24 MEMORY 6 katman + §25 DECISION LEDGER 11 alan. |
| 31 | part-31-gate-ve-plan.md | §26 GATE 0–5 + §27 IMPLEMENTATION PLAN (TASK-ID) + §28 kod kuralı 7 soru. |
| 32 | part-32-dogrulama-ve-cikti.md | §29–36: validation zinciri, traceability, change/conflict, no-hallucination, çıktı. |

> **Düzeltme notu (index ↔ part kısıtlamaları — tek kayıt yeri):**
> (a) §19'da kaynakta **9** bileşen sayılıdır (Agent Definition → Agent
> Events, ai.02 satır 655–663). Plan "10 bileşen" der; bu bir **sayım farkıdır** —
> Part 28'de işaretlenecektir. `⚠️ DOĞRULAMA GEREKLI`
> (b) §6'da kaynakta **9** analiz alanı var, `_PLAN.md` satır 113 "10 analiz alanı" der →
> Part 21'de **9/10** olarak işaretlenmiştir (satır 156'daki kısaltılmış hâli).
> (c) §1'de kaynakta **5 dürüstlük etiketi** var (DOĞRULANAMADI, BİLİNMİYOR, ÇIKARIM,
> WEB DOĞRULANDI, REFERENCE VERIFIED); plan "6 bilgi statüsü" der, **6. etiket YOK** →
> Part 18'de **5 etiket** olarak işaretlenmiştir (satır 153'teki kısaltılmış hâli).

### Blok 3 — ANA TALİMAT 1→20 (part 33–41) — `kaynak: _PLAN.md satır 126–137`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 33 | part-33-ana-talimat-kesif.md | ANA TALİMAT'ın sıralı çalışma kuralı + 1 DISCOVERY (bu aşamada yazma yasak). |
| 34 | part-34-legacy-ve-referans.md | 2 LEGACY ANALYSIS + 3 REFERENCE ANALYSIS: eski ve referans kod nasıl okunur. |
| 35 | part-35-indeks-ve-arastirma.md | 4 CODEBASE INDEX + 5 WEB/TECHNICAL RESEARCH adımları. |
| 36 | part-36-ikinci-beyin-kapi.md | 6 BUILD .AI + 8 FIRST APPROVAL GATE (⚠️ 7 no'lu adım başlığı kaynakta yok). |
| 37 | part-37-gereksinim-dongusu.md | 9 REQUIREMENT DISCOVERY + 10 ASKING QUESTIONS MODE + 11 FREEZE. |
| 38 | part-38-mimari-tasarim.md | 12 ARCHITECTURE DESIGN (DLL sayısı yapay artırılmaz) + 13 VALIDATION 10 kriter. |
| 39 | part-39-plan-ve-ikinci-kapi.md | 14 IMPLEMENTATION PLAN 15 faz + 15 SECOND APPROVAL GATE. |
| 40 | part-40-dogrulama-dongusu.md | 17 VALIDATION LOOP + 19 FINAL VALIDATION (⚠️ plan "16 ve 18 eksik" diyor; kaynakta ikisi de VAR — 16 satır 1521, 18 satır 1611). |
| 41 | part-41-mutlak-kural.md | 20 MUTLAK ÇALIŞMA KURALI: kod yazma, dur, bekle, onay al. |

### Blok 4 — Kural Seti 0→40 (part 42–55) — `kaynak: _PLAN.md satır 139–155`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 42 | part-42-temel-kural.md | 0 TEMEL KURAL + 10 maddelik amaç zinciri (anla→analiz→onay→kodla). |
| 43 | part-43-referans-analiz-kurallari.md | 1–3: referansı anla, analiz et, ama asla kopyalama. |
| 44 | part-44-indeks-ve-arastirma-2.md | 4–7: referans indeksi + araştırma kuralları (eksik başlık kontrolü dahil). |
| 45 | part-45-gereksinim-soru.md | 8–11: gereksinim çıkarma, soru modu, soru-cevap döngüsü, varsayım yasağı. |
| 46 | part-46-mimari-ve-dll.md | 12 MİMARİ TASARIM + 13 DLL/CLASS LIBRARY: her DLL'nin gerçek sorumluluğu. |
| 47 | part-47-solid-clean-di.md | 14 SOLID + 15 CLEAN CODE + 16 DI (UserService örneği ile). |
| 48 | part-48-devexpress-ui.md | 17 DEVEXPRESS + 18 UI MİMARİSİ (ViewModel, Dialog, Theme, Nav, Command). |
| 49 | part-49-ai-ve-agent.md | 19 AI MİMARİSİ (Provider) + 20 AGENT MİMARİSİ bileşenleri. |
| 50 | part-50-context-session-workflow.md | 21 PROJECT CONTEXT (4 bağlam) + 22 SESSION + 23 WORKFLOW 17 adım zinciri. |
| 51 | part-51-kapi-task.md | 24 ONAY GATE'LERİ + 25 KODLAMA BAŞLAMADAN ÖNCE + 26 TASK SİSTEMİ (BLOCKED). |
| 52 | part-52-cozum-test-review.md | 27 Solution/Projects 11 madde + 28 TEST + 29 CODE REVIEW 7 kriter. |
| 53 | part-53-refactor-karar-degisiklik.md | 30 REFACTORING + 31 KARAR KAYDI + 32 DEĞİŞİKLİK YÖNETİMİ. |
| 54 | part-54-agent-bilgi-arastirma.md | 33 AGENT DAVRANIŞ KURALLARI + 34 BİLGİ GÜVENİ (UNKNOWN) + 35 WEB ARAŞTIRMASI. |
| 55 | part-55-ilk-cikti-onay-felsefe.md | 36–40: ilk çalışma 12 adım, ilk çıktı, ilk onay, geliştirme felsefesi, başlangıç. |

### Blok 5 — Şablonlar & Anlatı (part 56–64) — `kaynak: _PLAN.md satır 157–168`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 56 | part-56-discovery-status.md | DISCOVERY STATUS şablonu: 7 alan + BLOCKERS + P0/P1/P2 + NEXT ACTION. |
| 57 | part-57-discovery-report.md | DISCOVERY REPORT 16 bölüm şablonu (Reference → Questions). |
| 58 | part-58-proje-nedir.md | §3.1–3.4: çekirdek tanım, 5 mimari katman, multi-agent swarm, fark tablosu. |
| 59 | part-59-yedi-problem.md | §4.1–4.7 her problem ayrı ayrı + ELI10 örnek. |
| 60 | part-60-problem-cozum-matrisi.md | §4.8 Problem–VersaCoder Çözüm Matrisi (7 satır). |
| 61 | part-61-yasam-dongusu.md | §5.1 6 aşamalı yaşam döngüsü + §5.2 katmanlar arası veri akışı. |
| 62 | part-62-wizard-mcp-sandbox.md | §5.3 wizard mekanizması + §5.4 MCP & plugin sandbox (AssemblyLoadContext). |
| 63 | part-63-bellek-ve-ui.md | §5.5 çift katmanlı hibrit bellek + §5.6 DevExpress UI/UX görselleştirme. |
| 64 | part-64-soru-cevap-kayitlari.md | Soru-cevap blokları + 5 açık soru (agent sayısı, UI, CI/CD, SignalR, monitoring). |

### Blok 6 — Mimari: 9 Tier / 618 DLL girişi (part 65–74) — `kaynak: _PLAN.md satır 170–182`

> 618 giriş / **616 benzersiz** (elle sayım, part-74); "500+" kaynak alıntısıdır (k59 notu).

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 65 | part-65-tier0-foundation.md | Tier 0: 50+ DLL (Primitives, Shared.*) — ELI10 + tam tablo. |
| 66 | part-66-tier1-domain.md | Tier 1: 100+ DLL (Domain.*) — ajan, session, skill, memory, workflow varlıkları. |
| 67 | part-67-tier2-application.md | Tier 2: 80+ DLL (Application.Commands.* dev komut listesi). |
| 68 | part-68-tier3-ai.md | Tier 3 AI: 40+ provider + Router 40+ strateji + Token + Roslyn CodeAnalysis. |
| 69 | part-69-tier3-data-guvenlik.md | Tier 3 devam: Data, Context, Learning, Security, Observability, Logging, MCP, Plugins. |
| 70 | part-70-tier3-altyapi.md | Tier 3 son: Embedded, Driver, Git, Terminal, FileSystem, Network, Caching, Messaging… |
| 71 | part-71-tier4-protocol.md | Tier 4: 30+ DLL (protocol, container, CI/CD). |
| 72 | part-72-tier5-6-crosscutting-host.md | Tier 5 CrossCutting 20+ DLL + Tier 6 Host 15+ DLL. |
| 73 | part-73-tier7-ui.md | Tier 7: 50+ UI DLL (Ribbon, Docking, Grid, Wizard, Chat…). |
| 74 | part-74-tier8-ve-sayim-celiskileri.md | Tier 8 Tests 30+ (⚠️ yinelenen Performance/Security) + 50/52/5000+ katman çelişkisi. |

### Blok 7 — Wizard & Agent Gelecek Mimarisi (part 75–81) — `kaynak: _PLAN.md satır 184–193`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 75 | part-75-session-wizard.md | Session Agentic Agent Wizard — üst sistem tanımı (ai.02 Blok H). |
| 76 | part-76-el10-debugger-effort.md | [EL-10] OpenCode F2 Debugger & F5 Effort Engine planı (tui/ui/core/codemode). |
| 77 | part-77-agentinc-3-kademe.md | Agent Inc: 3 kademeli wizard ve konfigürasyon hiyerarşisi. |
| 78 | part-78-gorsel-akis-anti-hallucination.md | Çift yönlü görsel akış/karar ağacı + deterministik anti-halüsinasyon kural motoru. |
| 79 | part-79-design-wizard.md | Proje/program yönetimi tasarım wizard arayüzü (Design Wizard UI/UX). |
| 80 | part-80-dinamik-prompt-sentezi.md | "Bitti" butonu + Dinamik Meta-System-Prompt Derleyicisi. |
| 81 | part-81-deterministik-yurutme.md | Adım adım wizard yapılandırması (4 adım) + deterministik yürütme & gözlemlenebilirlik. |

### Blok 8 — Ücretsiz Model Entegrasyonu (part 82–87) — `kaynak: _PLAN.md satır 195–203`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 82 | part-82-free-model-havuzu.md | OpenCode kaynaklı ücretsiz modeller + Zero-Cost fallback havuzu. |
| 83 | part-83-provider-tanimlari.md | §3.1 C# model ve provider tanımları (`VersaCoder.Infrastructure.AI`). |
| 84 | part-84-registry-ve-smart-router.md | §3.2 varsayılan ücretsiz model kataloğu + §4 Smart Router fallback stratejisi. |
| 85 | part-85-free-http-istemci.md | §5.1 `OpenCodeFreeHttpClient.cs` — aboneliksiz doğrudan free model iletişimi. |
| 86 | part-86-katalog-senkronu.md | §5.2 `ModelsDevCatalogSyncService.cs` dinamik katalog senkronu. |
| 87 | part-87-di-kaydi-ve-ozet.md | §5.3 Autofac/AIInfrastructureModule + "%100 kullanılabilir" kapanışı. |

### Blok 9 — Kurallar & Referans (part 88–91) — `kaynak: _PLAN.md satır 205–211`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 88 | part-88-16-adim-on-analiz.md | rules-03 §1.3: kod yazmadan önceki 16 adım ön-analiz, her adım 1 cümle. |
| 89 | part-89-zero-hallucination.md | §1.4 yasaklı uydurmalar + §1.5 akış + §1.6 bilgi güveni 7 statü. |
| 90 | part-90-referans-kurallari.md | reference-04: OpenCode künyesi, 8 erişim sorusu, yasaklar, 12 analiz alanı, 17.1–17.10 repo havuzu. |
| 91 | part-91-karar-kayitlari.md | questions-05: Autofac, EF Core, xUnit, DevExpress, SSE, SQLite+PostgreSQL, AES-256-GCM + ⚠️ EF Core ↔ Dapper çelişkisi. |

### Blok 10 — Vault Köprüsü (part 92–94) — `kaynak: _PLAN.md satır 213–218`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 92 | part-92-vault-sablonu-nedir.md | coremusic-vault-template nedir, neden YAPI referansı (içerik değil), .ai mantığı. |
| 93 | part-93-vaulttan-al-alma.md | Vault şablonundan alınacak 14 kural vs doğrudan kopyalanmayacak CoreMusic içeriği. |
| 94 | part-94-ai-kurulum-sirasi.md | PART 2 .ai vault kurulum sırası: templates → agents → kök MD → mimari + onay kuralı. |

### Blok 11 — Çelişki Defteri (part 95–96) — `kaynak: _PLAN.md satır 220–224`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 95 | part-95-celiski-defteri.md | Tüm çelişkiler: ORM, framework, katman sayısı, dizin, UI platformu. |
| 96 | part-96-celiski-cozum-plani.md | CONFLICT DETECTED akışı + öncelikli çözüm planı (hangi soru hangi gate'te). |

### Blok 12 — Gelecek & Final (part 97–99) — `kaynak: _PLAN.md satır 226–231`

| # | Dosya | Bu partta ne var (1 satır) |
|---|---|---|
| 97 | part-97-gelecek-1.md | Gelecek özellikler A: eksiklerin üretilmesi (vault şablonu, CI/CD, commit, gate). |
| 98 | part-98-gelecek-2.md | Gelecek özellikler B: ürün vizyonu (wizard motoru, hibrit bellek, MCP gateway…). |
| 99 | part-99-final.md | 100 partın özeti, 10 cümlelik ELI10 anlatım, 30 günlük uygulama planı. |

### Üretim (batch) sırası — `kaynak: _PLAN.md satır 233–243`

| Batch | Partlar | Bloklar | Durum |
|---|---|---|---|
| 1 | 00–17 | Blok 0 + 1 | ⏳ (bu batch'in parçasısın) |
| 2 | 18–32 | Blok 2 | ⏳ |
| 3 | 33–41 | Blok 3 | ⏳ |
| 4 | 42–55 | Blok 4 | ⏳ |
| 5 | 56–74 | Blok 5 + 6 | ⏳ |
| 6 | 75–91 | Blok 7 + 8 + 9 | ⏳ |
| 7 | 92–99 | Blok 10 + 11 + 12 | ⏳ |

### Derin dal (devam) — 100 part okuma takip çizelgesi

> Bu çizelge seti **defter gibi** kullanman içindir: her partı okuyunca `☐` → `☑`
> yap, altına tek kelimelik notunu yaz. Hızlı/derin/uygulama yollarında hangi
> partlarda olduğunu bu sayfadan görürsün. `kaynak: _PLAN.md satır 79–231`

**Nasıl kullanılır:**

1. **Sıfır noktası:** İlk okumada Blok 0 (00–09) satırlarını tek oturumda işaretle;
   bir sonraki oturumda Blok 1'den devam et. Blok = bir oturum birimi.
2. **Çift sütun kuralı:** Her satırda hem "okundu" hem "1 kelime not" alanı var.
   Not alanı boş kalırsa partı **anlamadın** demektir; işaretleme, boş satıra geri döner.
3. **Kaynak doğrulaması:** Derin dal bölümünü okuduysan ama kaynak satırını
   açıp bakmadıysan `☑ (kaynaksız)` yaz — bu işaretleme yarım işarettir.
4. **Çelişki avı:** `⚠️` gördüğün partların numarasını Part 95'in altındaki
   boş satıra ekle; çelişki defteri böyle büyür.
5. **Batch kapama:** Bir blok satırının tamamı `☑` olduğunda Part 00'daki batch
   tablosundaki `⏳`'yi `✅` ile değiştir.
6. **Kırılma durumu:** Arada bırakırsan, kırılan bloğun ilk partına dön; zincir
   (Sonraki part linki) sırası bozulmadan devam edilir.
7. **Süre tutmak:** Satır sonuna dakika yaz (örn. `~6 dk`); batch 1 sonunda toplam
   süreyi Part 01'deki "gelecek özellik 5"e girdi olarak verirsin.
8. **Tekrar okuma:** Bir partı ikinci kez okuyorsan satırın başına `R2:` ekle;
   tekrar okuma, anlamadığın yeri gösterir.

**100 part okuma çizelgesi (Blok 0 → Blok 12):**

| # | Durum | Part | 1 kelimelik not |
|---|---|---|---|
| 00 | ☐ | part-00-index.md — harita | ___ |
| 01 | ☐ | part-01-nasil-okunur.md — 9 blok şeması | ___ |
| 02 | ☐ | part-02-versacoder-nedir.md — tanım | ___ |
| 03 | ☐ | part-03-sozluk.md — 90 madde | ___ |
| 04 | ☐ | part-04-neden-var.md — 7 problem | ___ |
| 05 | ☐ | part-05-kaynak-haritasi.md — K1–K7 | ___ |
| 06 | ☐ | part-06-uc-prompt-iliskisi.md | ___ |
| 07 | ☐ | part-07-yol-haritasi.md | ___ |
| 08 | ☐ | part-08-eli10-kurallari.md | ___ |
| 09 | ☐ | part-09-temel-kavramlar.md | ___ |
| 10 | ☐ | part-10-kunye.md | ___ |
| 11 | ☐ | part-11-18-rol.md | ___ |
| 12 | ☐ | part-12-uygulanan-roller.md | ___ |
| 13 | ☐ | part-13-senior-kararlar.md | ___ |
| 14 | ☐ | part-14-teknoloji-yigini.md | ___ |
| 15 | ☐ | part-15-dizinler-yollar.md | ___ |
| 16 | ☐ | part-16-gorev-plani.md | ___ |
| 17 | ☐ | part-17-acik-sorular.md | ___ |
| 18 | ☐ | part-18-akis-diyagrami.md | ___ |
| 19 | ☐ | part-19-amaç-ve-proje.md | ___ |
| 20 | ☐ | part-20-referans-erisim.md | ___ |
| 21 | ☐ | part-21-referans-analiz.md | ___ |
| 22 | ☐ | part-22-evidence-rule.md | ___ |
| 23 | ☐ | part-23-web-ve-kaynak-dogrulama.md | ___ |
| 24 | ☐ | part-24-ikinci-beyin.md | ___ |
| 25 | ☐ | part-25-gereksinim-ve-soru.md | ___ |
| 26 | ☐ | part-26-varsayim-ve-mimari.md | ___ |
| 27 | ☐ | part-27-bagimlilik-solid.md | ___ |
| 28 | ☐ | part-28-devexpress-agent.md | ___ |
| 29 | ☐ | part-29-session-wizard-mcp.md | ___ |
| 30 | ☐ | part-30-plugin-memory-decision.md | ___ |
| 31 | ☐ | part-31-gate-ve-plan.md | ___ |
| 32 | ☐ | part-32-dogrulama-ve-cikti.md | ___ |
| 33 | ☐ | part-33-ana-talimat-kesif.md | ___ |
| 34 | ☐ | part-34-legacy-ve-referans.md | ___ |
| 35 | ☐ | part-35-indeks-ve-arastirma.md | ___ |
| 36 | ☐ | part-36-ikinci-beyin-kapi.md | ___ |
| 37 | ☐ | part-37-gereksinim-dongusu.md | ___ |
| 38 | ☐ | part-38-mimari-tasarim.md | ___ |
| 39 | ☐ | part-39-plan-ve-ikinci-kapi.md | ___ |
| 40 | ☐ | part-40-dogrulama-dongusu.md | ___ |
| 41 | ☐ | part-41-mutlak-kural.md | ___ |
| 42 | ☐ | part-42-temel-kural.md | ___ |
| 43 | ☐ | part-43-referans-analiz-kurallari.md | ___ |
| 44 | ☐ | part-44-indeks-ve-arastirma-2.md | ___ |
| 45 | ☐ | part-45-gereksinim-soru.md | ___ |
| 46 | ☐ | part-46-mimari-ve-dll.md | ___ |
| 47 | ☐ | part-47-solid-clean-di.md | ___ |
| 48 | ☐ | part-48-devexpress-ui.md | ___ |
| 49 | ☐ | part-49-ai-ve-agent.md | ___ |
| 50 | ☐ | part-50-context-session-workflow.md | ___ |
| 51 | ☐ | part-51-kapi-task.md | ___ |
| 52 | ☐ | part-52-cozum-test-review.md | ___ |
| 53 | ☐ | part-53-refactor-karar-degisiklik.md | ___ |
| 54 | ☐ | part-54-agent-bilgi-arastirma.md | ___ |
| 55 | ☐ | part-55-ilk-cikti-onay-felsefe.md | ___ |
| 56 | ☐ | part-56-discovery-status.md | ___ |
| 57 | ☐ | part-57-discovery-report.md | ___ |
| 58 | ☐ | part-58-proje-nedir.md | ___ |
| 59 | ☐ | part-59-yedi-problem.md | ___ |
| 60 | ☐ | part-60-problem-cozum-matrisi.md | ___ |
| 61 | ☐ | part-61-yasam-dongusu.md | ___ |
| 62 | ☐ | part-62-wizard-mcp-sandbox.md | ___ |
| 63 | ☐ | part-63-bellek-ve-ui.md | ___ |
| 64 | ☐ | part-64-soru-cevap-kayitlari.md | ___ |
| 65 | ☐ | part-65-tier0-foundation.md | ___ |
| 66 | ☐ | part-66-tier1-domain.md | ___ |
| 67 | ☐ | part-67-tier2-application.md | ___ |
| 68 | ☐ | part-68-tier3-ai.md | ___ |
| 69 | ☐ | part-69-tier3-data-guvenlik.md | ___ |
| 70 | ☐ | part-70-tier3-altyapi.md | ___ |
| 71 | ☐ | part-71-tier4-protocol.md | ___ |
| 72 | ☐ | part-72-tier5-6-crosscutting-host.md | ___ |
| 73 | ☐ | part-73-tier7-ui.md | ___ |
| 74 | ☐ | part-74-tier8-ve-sayim-celiskileri.md | ___ |
| 75 | ☐ | part-75-session-wizard.md | ___ |
| 76 | ☐ | part-76-el10-debugger-effort.md | ___ |
| 77 | ☐ | part-77-agentinc-3-kademe.md | ___ |
| 78 | ☐ | part-78-gorsel-akis-anti-hallucination.md | ___ |
| 79 | ☐ | part-79-design-wizard.md | ___ |
| 80 | ☐ | part-80-dinamik-prompt-sentezi.md | ___ |
| 81 | ☐ | part-81-deterministik-yurutme.md | ___ |
| 82 | ☐ | part-82-free-model-havuzu.md | ___ |
| 83 | ☐ | part-83-provider-tanimlari.md | ___ |
| 84 | ☐ | part-84-registry-ve-smart-router.md | ___ |
| 85 | ☐ | part-85-free-http-istemci.md | ___ |
| 86 | ☐ | part-86-katalog-senkronu.md | ___ |
| 87 | ☐ | part-87-di-kaydi-ve-ozet.md | ___ |
| 88 | ☐ | part-88-16-adim-on-analiz.md | ___ |
| 89 | ☐ | part-89-zero-hallucination.md | ___ |
| 90 | ☐ | part-90-referans-kurallari.md | ___ |
| 91 | ☐ | part-91-karar-kayitlari.md | ___ |
| 92 | ☐ | part-92-vault-sablonu-nedir.md | ___ |
| 93 | ☐ | part-93-vaulttan-al-alma.md | ___ |
| 94 | ☐ | part-94-ai-kurulum-sirasi.md | ___ |
| 95 | ☐ | part-95-celiski-defteri.md | ___ |
| 96 | ☐ | part-96-celiski-cozum-plani.md | ___ |
| 97 | ☐ | part-97-gelecek-1.md | ___ |
| 98 | ☐ | part-98-gelecek-2.md | ___ |
| 99 | ☐ | part-99-final.md | ___ |

**Çizelge özet kutusu (batch kapandıkça doldur):**

```text
BLOK DURUMU (okundu / toplam)
Blok 0  : ☐ 0/10     Blok 5  : ☐ 0/9
Blok 1  : ☐ 0/8      Blok 6  : ☐ 0/10
Blok 2  : ☐ 0/15     Blok 7  : ☐ 0/7
Blok 3  : ☐ 0/9      Blok 8  : ☐ 0/6
Blok 4  : ☐ 0/14     Blok 9  : ☐ 0/4
Blok 10 : ☐ 0/3      Blok 11 : ☐ 0/2   Blok 12 : ☐ 0/3
TOPLAM  : ☐ 0/100  ·  en az 500 satır/part ✓
```

### Derin dal (devam) — 7 doğrulama kontrolü (batch sonunda) `kaynak: _PLAN.md satır 61–77, 247`

| # | Kontrol | Nasıl bakılır | Başarısızlık |
|---|---|---|---|
| 1 | Dosya sayısı | 100 dosya (`part-00` … `part-99`) | eksik part |
| 2 | Numara kesintisizliği | 00→99 arası boşluk yok | zincir kopuk |
| 3 | Her part ≥500 satır | Satır sayacı | reddedilir |
| 4 | Frontmatter 7 alan | title, type, category, version, status, authority, updated | kalite hatası |
| 5 | UTF-8 Türkçe | Ç, Ğ, İ, Ö, Ş, Ü bozuk değil | kodlama hatası |
| 6 | 9 blok sırası | başlık → öğren → ELI10 → adım → derin → örnek → dikkat → gelecek → sonraki | şema ihlali |
| 7 | `Sonraki part` linki | Son satırda `→ Part NN` | zincir kopuk |

**Batch raporu şablonu:**

```text
BATCH RAPORU
  dosya sayısı : 100/100   (kontrol 1)
  numara       : 00-99     (kontrol 2)
  satır        : min 500   (kontrol 3)
  frontmatter  : 7 alan    (kontrol 4)
  utf-8        : OK        (kontrol 5)
  9 blok       : OK        (kontrol 6)
  zincir       : 00→99     (kontrol 7)
```

## Örnek / uygulama

**Örnek 1 — 3 okuma yolu (hangi partlar, kaç dakika):**

```text
[ HIZLI YOL — ~45 dk ]          [ DERİN YOL — ~20 saat ]       [ UYGULAMA YOLU — ~6 saat ]
00 içindekiler                    00 → 09 (Blok 0)               33 → 41 (Blok 3: 1→20 kural)
02 VersaCoder nedir               10 → 17 (Blok 1)               42 → 55 (Blok 4: 0→40 kural)
04 neden var (7 problem)          18 → 32 (Blok 2)               51 gate + task
58 tanım (§3)                     33 → 41 (Blok 3)               88 → 89 kurallar
60 çözüm matrisi (§4.8)           42 → 55 (Blok 4)               95 çelişkiler
99 final özeti                    56 → 74 (Blok 5-6)
                                 75 → 91 (Blok 7-9)
                                 92 → 99 (Blok 10-12)
```

**Örnek 2 — Bir bloğun içinde gezi (Blok 2, part 18–32):**

| Gün | Partlar | Kazanım |
|---|---|---|
| 1. gün | 18–20 | Akış diyagramı + rol + referans yasakları |
| 2. gün | 21–24 | Analiz alanları, kanıt formatı, .ai kalite kuralı |
| 3. gün | 25–28 | Gereksinim, varsayım yasağı, bağımlılık, DevExpress/agent |
| 4. gün | 29–32 | Session/wizard/MCP, plugin/memory/decision, gate, doğrulama |

**Örnek 3 — Part dosyası isimlendirme deseni (set genelinde sabit):**

```text
part-<2 hane numara>-<kucuk-harf-turkce-ozet>.md
part-00-index.md
part-31-gate-ve-plan.md
part-95-celiski-defteri.md
```

## Dikkat / çelişki

- ⚠️ **Plan "13 blok" der; blok numaraları 0–12'dir** (0 dahil 13 tane). 0'dan
  saymayı unutup "12 blok" dersen 100 part sayıması şaşar.
- ⚠️ **Blok 3'te 9 part var, 10 değil** (33–41). Plan tablosu 9 satırdır; 40 numaralı
  part son parttır. `kaynak: _PLAN.md satır 126–137`
- ⚠️ **§19 AI AGENT ARCHITECTURE bileşen sayısı:** plan "10 bileşen" der, kaynakta
  9 satır vardır (ai.02 satır 655–663). `DOĞRULAMA GEREKLI` — Part 28'de çözülecek.
- ⚠️ **Part 06, 07, 08, 09 için ayrı kaynak dosya YOK** — `_PLAN.md` türetilmesidir;
  bu partlarda derin dal, set-özü içeriğe dayanır, harici kaynağa değil.
- ⚠️ **Part 00–99 dosya adları planda sabittir**; üretim sırasında farklı ad
  kullanılırsa zincir (`Sonraki part`) bozulur.
- `YOK`: Bu dosyada setin "hedef okuyucu sayısı" veya "tahmini okuma süresi toplamı"
  gibi planlanmamış veriler **YOK**tur; dakika süreleri tahminidir, kaynak değildir.
- `DOĞRULAMA GEREKLI`: Batch durumları planda ⏳ (beklemede) olarak görünüyor; set
  üretilirken bu hücreler güncellenmelidir.

## Gelecek özellik

| # | Öneri | Kapsam | Kabul kriteri | Etkilenen dosya |
|---|---|---|---|---|
| 1 | **Part↔kaynak çift yönlü indeks** | Her partın en az 1 kaynak atfı; kaynaktan geriye doğru part listesi | `grep "kaynak:"` her partta ≥1 sonuç verir | part-05-kaynak-haritasi.md |
| 2 | **Durum rozeti ekleme** | Batch tablosuna ✅/⏳ rozeti (üretim ilerledikçe) | 7 batch hücresinin tamamı ✅ olduğunda set "tamamlandı" sayılır | part-00-index.md |
| 3 | **Terim ↔ part yönlendirme** | Sözlükteki her terim, anlatıldığı part'a link | 90 maddenin tamamında part linki var | part-03-sozluk.md |
| 4 | **Zaman damgası tutma** | Part başına "okundu ✓ / not alındı" kutucuğu | 100 part için işaretleme alanı hazır | part-01-nasil-okunur.md |
| 5 | **Blok mini-özet dosyası** | Her blok sonunda 10 satırlık "bu bloktan 3 şey" özeti | 13 blok × 10 satır = 130 satır ayrı dosya | _PLAN.md (öneri) |

## Sonraki part

Setin şemasını ve 9 bloklu part yazım kuralını öğrenmek için
[part-01-nasil-okunur.md](part-01-nasil-okunur.md) dosyasına geç.
