---
title: "VersaCoder — Güvenlik Denetimi Raporu Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "guvenlik denetimi raporu sablonu: OWASP bulgulari, risk register ve duzeltme plani"
---

# VersaCoder — Güvenlik Denetimi Raporu Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [docs-md](./docs-md-template.md)

---

## §1 Amaç

`C:\versacoder\.ai\reports\security-audit-YYYY-MM-DD-<kapsam>.md` dosyasının
**tek üretim kaynağıdır**. Güvenlik denetimi raporu korku metni değil,
**kanıtlı bulgu listesidir**: hangi kontrol çalıştı, ne buldu, kanıtı ne,
severitesi ne, kime ne zaman düzeltilecek. İddia kanıtsızsa bulgu sayılmaz.

SRP gereği bu şablon **kod yazmaz, exploit üretmez, düzeltme uygulamaz**:
tarama §5.3 salt-okunur komutlarıyla yapılır, düzeltme kodu ilgili katman
şablonuna, karar ADR’e devredilir — burada yalnız **denetim raporu**
üretilir.

| Alan | Değer |
|------|-------|
| Template Name | `security-audit-template.md` |
| Template Path | `.ai/.templates/documentation/security-audit-template.md` |
| Hedef Dosya | `.ai/reports/security-audit-YYYY-MM-DD-<kapsam>.md` |
| Hedef Dizin | `.ai/reports/` — mevcut (içinde `conflict-register-001-master-prompts.md` var) |
| Zorunlu Yapı | 7 bölüm `§1…§7` + §3.4 bulgu kalıbı |
| Üretilen Uzunluk | `240–600 satır` · `14–30 KB` |
| Kodlama | **UTF-8 (BOM’suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe (ç ğ ı İ ö ş ü) — dosya yolu, alan adı, komut İngilizce |

### §1.1 Ne üretir

Bu şablon dört çıktı verir:

1. **OWASP bulgu listesi** — kontrol / bulgu / kanıt / şiddet / düzeltme.
2. **Risk register** — Risk ID, tetikleyici, azaltma, sahip, durum.
3. **Düzeltme önceliklendirme planı** — CRITICAL → HIGH → MEDIUM → LOW.
4. **İmza / tarih bloğu** — denetleyen, onaylayan, yeniden tarih.

### §1.2 Disk Kanıtları

Şablon yazım öncesi ve sonrası şu disk gerçeklerini okur; hiçbiri
tahminle doldurulmaz:

```powershell
  Get-ChildItem C:\versacoder\.ai\reports -Filter security-audit-*.md
  Get-Item    C:\versacoder\.ai\.templates\documentation\security-audit-template.md
  (Get-Content C:\versacoder\.ai\reports\security-audit-<tarih>-<kapsam>.md).Count
```

Beklenen: rapor önce varsa son sürüm, yoksa `0` dosya. Dizin mevcut;
yoksa bu şablon önce dizini üretir, sonra raporu yazar.

### §1.3 Ne zaman kullanılır

| Durum | Kullanım |
|-------|----------|
| Yeni agent / MCP eklendi | Denetim raporu `draft` açılır |
| Kapı geçişi (GATE) öncesi | §3.4 bulguları kapatılmadan kapı açılmaz |
| Kritik bulgu düzeltildi | §6.4 yeniden doğrulama + tarih tazelenir |
| 90 gün geçti | Rapora “süresi doldu” damgası, §7.2 kaydı |

### §1.4 Kapsam dışı (SRP)

Bu şablon **şunları üretmez**: exploit/payload (üretimi yasak), düzeltme
kod diff’i (bkz. `../backend/`), karar metni (bkz. [adr-security-template](../adr/adr-security-template.md)),
genel doküman (bkz. `./docs-md-template.md`). Karışan konu
**CONFLICT DETECTED** olarak §7.2’ye yazılır.

+----------------------------------------------------------------+
|Soru: Bu icerik bir guvenlik denetimi raporu mu?                |
|                                                                |
|  Karar agaci                                                   |
|  ----------                                                    |
|   bulgu + kanit + siddet  -- EVET --> BU SABLON                |
|   karar (neden politika)  -- EVET --> adr-security-template    |
|   duzeltme kodu           -- EVET --> katman sablonu           |
|   sadece tarama ciktisi    -- EVET --> BU SABLON (ham bulgu)   |
|   genel belge              --------> docs-md-template.md       |
|                                                                |
|Kirmizi bayrak: kanitsiz "kritik acik" iddiasi (G7) reddedilir. |
+----------------------------------------------------------------+

---

## §2 Kapsam

### §2.1 Kapsam içi

Bu şablon şu denetim alanlarını kapsar:

- **Dış ajan CLI süreçleri** — working directory, environment, child
  process, ağ erişimi, dosya kapsamı;
- **MCP yetkileri** — sunucu listesi, izin verilen araçlar, tool argüman
  sınırları;
- **Tool Gateway politikası** — onay/red listesi, policy bypass yolları;
- **Sızıntı yüzeyi** — secret sızıntısı, log/çıkış taşması, prompt
  injection;
- **Girdi işleme** — path injection, command injection, deserialization;
- OWASP Top 10 kontrol seti (§3.3) ve VersaCoder’a özel alanlar (§3.5).

### §2.2 Kapsam dışı

Aşağıdakiler bu şablonun dışındadır; sahipleri parantez içinde:

- Fiziksel/ortam güvenliği ve sosyal mühendislik (insan süreçleri);
- Güvenlik kararlarının gerekçesi (`../adr/adr-security-template.md`);
- Düzeltme kodunun kendisi (ilgili katman şablonu: `../backend/`, `../infrastructure/`);
- Performans/erişilebilirlik denetimleri (`./docs-md-template.md`);
- Üçüncü taraf sözleşme hukuku (hukuk ekibi).

Kapsam sınırı ihlali = **CONFLICT DETECTED** etiketi ve §7.2 Red Team
kaydı (G6).

## §3 Mimari

Şablonun gövde mimarisi altı katmanlıdır; her katman bir alt bölüme
karşılık gelir ve sırayla doldurulur (G12 · §4.5).

### §3.1 Frontmatter bloğu

İlk 9 satır 8 alan içerir; kapanış `---` 10. satırdadır (G1 · §4.1).

| # | Alan | Zorunlu değer | Not |
|---|------|--------------|-----|
| 1 | `title` | `"… - <kapsam>"` | Raporun tam adı |
| 2 | `type` | `report` | Denetim raporu tipi |
| 3 | `category` | `report` | `reports/` ile aynı kategori |
| 4 | `status` | `draft` / `review` / `active` / `deprecated` | Yaşam döngüsü |
| 5 | `version` | `x.y.z` | Semver; bkz. §3.10 |
| 6 | `updated` | `YYYY-MM-DD` | Denetim tarihi |
| 7 | `authority` | `SSOT` | Tek doğruluk kaynağı iddiası |
| 8 | `scope` | Denetilen yüzeyin tek cümlesi | Sınırlar |

Örnek (dolu):

```yaml
---
title: "VersaCoder — Güvenlik Denetimi - Agent CLI ve MCP"
type: report
category: report
status: draft
version: 0.1.0
updated: 2026-10-05
authority: SSOT
scope: "dis ajan CLI surecleri, MCP yetkileri ve Tool Gateway politikasi"
---
```

### §3.2 Tek cümlelik özet (lede)

H1’den hemen sonra tek paragraf, en fazla 25 kelime: **hangi yüzey, hangi
tarih, hangi sonuç**. Lede dolmadan §6.1 maddeleri 1-3 geçersizdir.

Kalıp: `Bu rapor, <tarih> denetiminde <yüzey> yüzeyini <sonuç> açısından inceler.`

Örnek: “Bu rapor, 2026-10-05 denetiminde dış ajan CLI süreçleri ve MCP
yetkilerini policy bypass açısından inceler.”

### §3.3 OWASP Top 10 kontrol seti

Her OWASP başlığı bir kontrol satırıdır; çalıştırılmayan kontrol “temiz”
değil, **N/A + gerekçe** sayılır (G12).

| # | Kontrol başlığı | Ne sorulur |
|---|-----------------|------------|
| A01 | Broken Access Control | Ajan/MCP kimliği yetki sınırını aşıyor mu? |
| A02 | Cryptographic Failure | Secret açık metin mi, log’a mı düşüyor? |
| A03 | Injection | Prompt/command/path injection girişi var mı? |
| A04 | Insecure Design | Tool Gateway varsayılan red kuralına sahip mi? |
| A05 | Security Misconfiguration | Ajan ortam değişkenleri fazla geniş mi? |
| A06 | Vulnerable Components | Dış CLI/MCP sürümü biliniyor mu (G9)? |
| A07 | Identification & Auth | MCP sunucuları kimlik doğruluyor mu? |
| A08 | Software & Data Integrity | Deserialization imzasız mı? |
| A09 | Logging & Monitoring Failures | Red kararları loglanıyor mu? |
| A10 | SSRF | Ajanın ağ erişimi hedef sınırına sahip mi? |

### §3.4 Bulgu kalıbı (zorunlu 9 alan)

Bir bulgu aşağıdaki dokuz alan olmadan tamamlanmaz; alan eksikse bulgu
**draft** kalır ve §6.1 madde 8 FAIL olur:

| # | Alan | İçerik | Kural |
|---|------|--------|-------|
| 1 | Finding ID | `SEC-<NNN>` | Sıralı, tekrarsız |
| 2 | Başlık | Tek satır, ne + nerede | Jargon yok |
| 3 | Kontrol | §3.3 OWASP satırı ya da §3.5 özel alan | Bağlantısız bulgu yok |
| 4 | Kanıt | §6.3 biçimlerinden biri | Kanıtsız = bulgu değil |
| 5 | Şiddet | `LOW` / `MEDIUM` / `HIGH` / `CRITICAL` | Ölçek §3.4.1 |
| 6 | Etki | Ne kaybedilir (veri / bütünlük / süre) | Somut, abartısız |
| 7 | Düzeltme | Tek aksiyon, sahibiyle | Fikir değil görev |
| 8 | Sahip | Ekip adı (tek) | Boşsa bulgu kapanmaz |
| 9 | Durum | `open` / `in-progress` / `fixed` / `accepted-risk` | Kapalıysa kanıt tazelenir |

#### §3.4.1 Şiddet ölçeği

| Şiddet | Tanım | SLA |
|--------|-------|-----|
| CRITICAL | Politika bypass + secret/agentsız erişim; istismar kanıtlı | 24 saat |
| HIGH | Yetki sınırı aşımı ya da sızıntı yolu açık, istismar koşulu az | 7 gün |
| MEDIUM | Savunma derinliği eksik, tek başına istismar zor | 30 gün |
| LOW | Hijyen/iyileştirme; kullanıcı etkisi yok | 90 gün |

SLA, düzeltme **tamamlanma** tarihidir; “başlandı” SLA’yı kapatmaz.

### §3.5 VersaCoder’a özel denetim alanları

OWASP dışında bu depoya özgü sekiz alan; her biri ayrı kontrol satırı
olarak §3.4’e bağlanır:

| Alan | Kontrol sorusu |
|------|----------------|
| Dış ajan CLI süreci | Working directory, environment, child process, ağ ve dosya kapsamı sınırında mı? |
| MCP yetkileri | Sunucu başına izinli araç listesi var mı, argüman sınırları tanımlı mı? |
| Tool Gateway politikası | Onay/red listesi varsayılan red mi, bypass denetleniyor mu? |
| Policy bypass | Politika atlatma yolu (override, fallback, ikinci giriş noktası) var mı? |
| Secret sızıntısı | Anahtar/oken yerine: gerçek anahtar, parola ve token log, hata, çıktı ya da prompt içine taşmış mı? |
| Prompt injection | Dış kaynaklı metin araç çağrısını yönlendiriyor mu? |
| Path / command injection | Girdi yolu ve komut satırı doğrudan birleştiriliyor mu? |
| Deserialization | İmzasız nesne çözümleme var mı? |

### §3.6 Risk register formatı

Register, bulgu listesinden ayrıdır: bulgu **gerçekleşmiş** olayı, register
**olağanı** yazar. Kolonlar sabittir:

| Kolon | İçerik | Kural |
|-------|--------|-------|
| Risk ID | `RSK-<NNN>` | Bulgu ID’siyle ilişkili olabilir |
| Olasılık | `düşük / orta / yüksek` | **Sayısal yüzde YASAK — uydurma** |
| Etki | `düşük / orta / yüksek` | Somut kayıp tanımı |
| Tetikleyici | Riski fiile dönürecek olay | Koşul yazılır, korku değil |
| Azaltma | Önleyici kontrol | Sahibiyle |
| Sahip | Ekip adı (tek) | Boşsa register eksik |
| Durum | `izleniyor` / `azaltıldı` / `kapandı` | Kanıtsız kapandı olmaz |

Olasılık kaynakları: gözlem, olay kaydı ya da üretici dokümanı. Kaynak
yoksa olasılık `[NOT SPECIFIED]` + soru sahibi + tarih kalır (G9);
yüzde uydurmak halisasyondur.

### §3.7 Kalite sözleşmesi

Bu şablon kalite sözleşmesini `../index.md` §3.7’den devralır ve
denetim raporuna uyarlar:

| Sözleşme | Rapor karşılığı |
|----------|-----------------|
| 7 bölüm eksiksiz | §1…§7 başlıkları sırayla |
| 8 alanlı frontmatter | §3.1 tablosu |
| Kanıt satırı | §6.3 kanıt biçimi |
| Bozuk link yok | §3.11 denetimi + §6.1 madde 10 |
| Halisasyon etiketleri | §3.8 |
| Türkçe, UTF-8, BOM’suz | §6.1 madde 2 |
| G1..G12 | §4.1-§4.5 |

Not: rapor uzunluğu §1 tablosunda (240–600 satır) belirlenir; şablonun
kendisi `[NOT SPECIFIED]` bırakılan alanları saymaz (G9).

### §3.8 Kanıt ve etiket sistemi

Üç zorunlu etiket; biri olmadan rapor `active` olamaz:

| Etiket | Ne zaman | Eylem |
|--------|----------|-------|
| `⚠️ VERIFICATION REQUIRED` | İddia disk kanıtıyla görülmedi | Tarama yapılana kadar kalır |
| `[NOT SPECIFIED]` | Bilgi vault’ta yok | Soru sahibi + tarih yazılır |
| `CONFLICT DETECTED` | İki kaynak çelişiyor | İkisi de linklenir, çelişki yazılır |

Bulgu metninde etiket, ilgili kanıt satırının hemen altına konur; lede’ye
sığdırılıp sonra unutulmaz (G12).

### §3.9 Dolu VersaCoder Örneği + Kalıp Blok

Önce dolu örnek (Agent CLI denetimi, tek bulgu):

```markdown
### SEC-001 — Dış ajan süreç working directory sınırı yok

- **Kontrol:** `.ai/` özel alanı — Dış ajan CLI süreci
- **Kanıt:** `KANIT: Get-Process ... -> StartInfo.WorkingDirectory boş`
- **Şiddet:** `HIGH`
- **Etki:** Ajan repo dışı dosyayı okuyup yanıtına taşıyabilir.
- **Düzeltme:** Gateway’de sabit working directory zorunluluğu (sahip: agent ekibi)
- **Durum:** `open` · Bulgu tarihi: 2026-10-05

## §6 Doğrulama

Kanıt: `Select-String -Path *.md -Pattern "TODO|FIXME"` -> 0 eşleşme
```

Sonra kalıp blok — bu şablonda `{{PLACEHOLDER}}` adları **yalnızca**
aşağıdaki blokta geçer; dosyanın başka yerinde `{{` açılışı yasaktır
(§6.1 madde 13):

```markdown
## §6 Doğrulama

### §6.1 Doğrulama Maddeleri

| # | Madde | Durum |
|---|-------|-------|
| 1 | Frontmatter 8 alan, 10. satırda kapanır | {{V_FM}} |
| 2 | H1 + zorunlu bağlantı satırı birebir | {{V_H1}} |
| 3 | Lede en fazla 25 kelime | {{V_LEDE}} |
| 4 | §1…§7 başlıkları sırayla mevcut | {{V_SEC}} |
| 5 | §3.3 OWASP 10/10 ya da N/A+gerekçe | {{V_OWASP}} |
| 6 | §3.4 bulgu kalıbı 9 alan eksiksiz | {{V_FINDING}} |
| 7 | §3.5 özel alanlar 8/8 denetlendi | {{V_VERSA}} |
| 8 | §3.6 risk register kolonları dolu | {{V_RISK}} |
| 9 | Link whitelist ihlali yok | {{V_WHITE}} |
| 10 | Kırık link sayısı 0 | {{V_BROKEN}} |
| 11 | Halisasyon etiketleri gerekli yerlerde | {{V_TAG}} |
| 12 | UTF-8 (BOM’suz), Türkçe karakterler sağlam | {{V_ENC}} |
| 13 | Yer tutucu deseni yalnız §3.9’da | {{V_PLACE}} |
| 14 | Şiddet ölçeği §3.4.1’e uygun | {{V_SEV}} |
| 15 | Risk register’da sayısal olasılık yok | {{V_PCT}} |
| 16 | Sahip + statü + sürüm yazıyor | {{V_OWNER}} |
| 17 | İmza / tarih bloğu dolu (§3.10) | {{V_SIGN}} |
| 18 | Kanıt satırları §6.3 biçiminde | {{V_EVID}} |

Test: `Test-Path .ai/reports/security-audit-{{TARIH}}-{{KAPSAM}}.md` -> `True`
```

### §3.10 Düzeltme önceliklendirme ve imza / tarih bloğu

Öncelik sırası sabittir: `CRITICAL → HIGH → MEDIUM → LOW`; eşitlikte
**istismar kanıtı olan** önce gelir, sonra SLA tarihi küçük olan. Her
düzeltme üç bilgiyle kaydedilir: bulgu ID, sahip, tamamlanma tarihi.

İmza bloğu raporun sonunda `## §7`’den hemen önce yer alır:

```
  Denetleyen   : <ad, ekip>          Tarih: YYYY-MM-DD
  Onaylayan    : <ad, ekip>          Tarih: YYYY-MM-DD
  Yeniden denetim (zorunlu): YYYY-MM-DD
  Onceki surum : security-audit-<oncedeki-tarih>-<kapsam>.md
```

Red Team notu: “onaylayan” satırı boşken rapor `review`’da kalır;
baskı yapılmış seenaryoda bu satır **CONFLICT DETECTED** sayılır.

Versiyonlama semver’dir: bulgu ekleme **minor**, düzeltme durumu/küçük
düzeltme **patch**, şiddet ölçeği ya da bölüm formatı değişimi **major**.
`updated` her geçişte tazelenir.

### §3.11 Kırık link denetimi ve Quality Report

Yazım sonrası zorunlu iki çıktı:

1. **Kırık link taraması** — §5.3’teki salt-okunur komut; hedef 0 kırık.
2. **Quality Report** — §5.4 kalıbı; satır / KB / bölüm / bulgu /
   register sayaçları.

```
SecurityAudit Quality Report — <kapsam>.md (YYYY-MM-DD HH:MM)
  satır: <n>  KB: <n,n>  §1-§7: <7/7>  OWASP: <10/10|N/A>
  bulgu: <n> (CRITICAL/HIGH/MEDIUM/LOW: a/b/c/d)  register: <n>
  link: <toplam>  kirik: 0  whitelist-ihlali: 0
  PLACEHOLDER: yalniz §3.9  G1..G12: 12/12  karar: PASS | FAIL
```

CRITICAL > 0 ise karar otomatik `FAIL`’dir; rapor ‘ aktif kapıya
bağlanamaz (G6 · §4.4).

---

## §4 Kurallar

### §4.1 G1–G3: Yapı ve kodlama

| Kural | Metin | İhlal cezası |
|-------|-------|--------------|
| G1 | Frontmatter ilk 9 satırda 8 alan; kapanış `---` 10. satır; 8. alan `scope` | Rapor `draft`’a döner |
| G2 | EF Core tek ORM’dir; başka ORM adı geçmez | CONFLICT DETECTED |
| G3 | Dosya UTF-8 (BOM’suz), Türkçe (ç ğ ı İ ö ş ü); Latin-1/CP1254 YASAK | Rapor reddedilir |

G1 detayı: alan sırası `title, type, category, status, version, updated,
authority, scope`. `updated: 2026-10-05` bu şablonun kendi tarihidir;
üretilen raporda denetim tarihi kullanılır.

### §4.2 G4–G6: Bağlantı ve kapsam

| Kural | Metin | İhlal cezası |
|-------|-------|--------------|
| G4 | Bağlantı zorunlu: üst + en az 2 ilişkili; hiç linksiz rapor yazılmaz | Kırmızı bayrak |
| G5 | Link yalnız whitelist’ten (§7.1); yasaklı hedefe link yasak | Bozuk link sayılır |
| G6 | Kapsam dışı konu + Red Team kaydı: `CONFLICT DETECTED` etiketi | Rapor review’a düşer |

### §4.3 G7–G9: Kanıt ve doğrulama

| Kural | Metin | İhlal cezası |
|-------|-------|--------------|
| G7 | Kanıtsız bulgu yok; bulgu = kontrol + kanıt + şiddet üçlüsü | `⚠️ VERIFICATION REQUIRED` |
| G8 | `GATE-003` onayından önce `src/**` altına satır yazılmaz; rapor da uygulama yapmaz | Kapalı kapı |
| G9 | Bilinmeyen bilgi uydurulmaz: `[NOT SPECIFIED]` + soru sahibi + tarih | Hallisasyon |

G7 detayı: “yüksek riskli görüldü” ifadesi tek başına bulgu değildir;
kontrol satırı (§3.3/§3.5) ve kanıt biçimi (§6.3) zorunludur.

### §4.4 G10–G12: İçerik ve format

| Kural | Metin | İhlal cezası |
|-------|-------|--------------|
| G10 | `SELECT *` yasak; tarama sorguları sütun listesiyle yazılır | Bulgu (HIGH) |
| G11 | Ürün adı **VersaCoder**, katman adı `l0..l7`; yasaklı eski marka adı geçmez | Marka ihlali |
| G12 | 7 bölüm + §3.4 dokuz alan + §3.6 register kolonları eksiksiz | Bölüm/alan silinemez |

### §4.5 Kural özeti (G1..G12)

```
  G1  Frontmatter 8 alan / 10. satir kapanis
  G2  EF Core tek ORM
  G3  UTF-8 BOM-suz + Turkce
  G4  Baglanti zorunlu (ust + 2 iliskili)
  G5  Link yalniz whitelist
  G6  Kapsam disi = CONFLICT DETECTED + Red Team
  G7  Kanitsiz bulgu yok (kontrol + kanit + siddet)
  G8  GATE-003 oncesi src/** kapali
  G9  [NOT SPECIFIED] + soru sahibi + tarih
  G10 SELECT * yasak
  G11 VersaCoder / l0..l7 / yasakli marka adi gecmez
  G12 7 bolum + 9 bulgu alani + register kolonlari
```

Aynı anda ihlal edilen kural sayısı ikiyi geçerse rapor doğrudan
`draft` statüsüne geri alınır ve §7.2 Red Team listesine eklenir.

---

## §5 Workflow

### §5.1 Girdi

| Girdi | Kaynak |
|-------|--------|
| Denetim kapsamı + tarih | Talep metni |
| Bağlı ADR (güvenlik) | [adr-security-template](../adr/adr-security-template.md) |
| Disk kanıtları | §5.3 salt-okunur tarama |
| Önceki rapor | `.ai/reports/security-audit-*.md` |
| Politika tanımları | Tool Gateway / MCP konfigürasyonu |

### §5.2 Adımlar

| # | Adım | Çıktı |
|---|------|-------|
| 1 | Önceki raporları ve kapsamı tara (§5.3) | Dosya listesi |
| 2 | Frontmatter + H1 + zorunlu bağlantı satırı | 10 satır |
| 3 | Lede (≤25 kelime) + bağlantılar | G4 |
| 4 | §3.3 OWASP 10 kontrolü + §3.5 özel 8 alan | Kontrol tablosu |
| 5 | §3.4 bulgular (9 alan) + §3.6 register | Bulgu + risk |
| 6 | §1…§7 tamamla, kanıt satırlarını işle | Gövde |
| 7 | Kırık link denetimi (§5.3) | 0 kırık |
| 8 | Quality Report (§5.4) + imza bloğu (§3.10) | Rapor + imza |
| 9 | §6.1 18 madde denetimi, statü geçişi | review |

### §5.3 Salt-okunur PowerShell taraması

Hiçbiri dosyaya yazmaz; exploit üretmez, payload çalıştırmaz:

```powershell
# 1) Onceki denetim raporlari
Get-ChildItem C:\versacoder\.ai\reports -Filter security-audit-*.md |
  Sort-Object Name | Select-Object Name, Length, LastWriteTime

# 2) Satir / KB sayaci (sartname: >=240 satir, >=14 KB)
$f = "C:\versacoder\.ai\reports\security-audit-<tarih>-<kapsam>.md"
if (Test-Path $f) {
  $c = Get-Content -LiteralPath $f -Encoding UTF8
  [pscustomobject]@{ Satir = $c.Count; KB = [math]::Round((Get-Item $f).Length / 1KB, 1) }
}

# 3) Bolum basliklari (beklenen: 7 adet "## §")
(Get-Content -LiteralPath $f -Encoding UTF8) -match "^## §"

# 4) SECRET YUZEYİ — dosyada gercek anahtar deseni var mi? (salt okunur)
$desen = "(?i)(api[_-]?key|secret|token|password)\s*[:=]\s*[^\s]{8,}"
 $eslesme = Select-String -LiteralPath $f -Pattern $desen
$eslesme | ForEach-Object { "$($_.LineNumber): $($_.Line.Substring(0,40))" }

# 5) Turevli ad tespiti (G11) — birlestirilmis literal, dosyada gecmez
$yasak = ("Core" + "Music")
(Get-Content -LiteralPath $f -Encoding UTF8) | Where-Object { $_ -match $yasak }

# 6) Siddet kelimesi sayimi (rapor disi belgelerde gecmez)
(Get-Content -LiteralPath $f -Encoding UTF8) |
  ForEach-Object { ([regex]::Matches($_, "CRITICAL|HIGH|MEDIUM|LOW")).Count } |
  Measure-Object -Sum
```

Beklenen: madde 4 boş (rapor kendisi secret içermemeli — kanıt olarak
yer tutucu kullanılır), madde 5 boş; madde 1’de en yeni rapor adı §5.4
girdisidir.

### §5.4 Quality Report kalıbı

Yazım biterken tek blok halinde üretilir:

```
SecurityAudit Quality Report — <kapsam>.md (YYYY-MM-DD HH:MM)
  satır: <n> (>=240)   KB: <n,n> (>=14,0)
  §1-§7: <7/7>   OWASP: <10/10 | N/A+gerekce>   ozel alan: <8/8>
  bulgu: <n> (C/H/M/L: <a>/<b>/<c>/<d>)   register: <n>
  link: <toplam>  kirik: 0  whitelist-ihlali: 0  secret-deseni: 0
  imza: dolu   PLACEHOLDER: yalniz §3.9   G1..G12: 12/12
  karar: PASS | FAIL   (CRITICAL>0 ise otomatik FAIL)
```

---

## §6 Doğrulama

### §6.1 Doğrulama Maddeleri

Rapor §5.2 adım 9’da **18 maddenin tamamı** yeşil olmadan review’a
alınmaz. Durum sütunu: `PASS` / `FAIL` / `N/A` (N/A gerekçesi zorunlu).

| # | Madde | Kanıt |
|---|-------|-------|
| 1 | Frontmatter ilk 9 satır, 8 alan, sırası §4.1 ile uyumlu | satır 1-10 |
| 2 | UTF-8 (BOM’suz); Türkçe karakterler sağlam | §5.3 madde 2 |
| 3 | H1 tek tane, dosya adıyla uyumlu | §5.3 madde 3 |
| 4 | Zorunlu bağlantı satırı birebir §7.1-A | satır no |
| 5 | Lede ≤25 kelime, yüzey + tarih + sonuç içeriyor | kelime sayacı |
| 6 | §1…§7 başlıkları tam ve sırayla (7/7) | §5.3 madde 3 |
| 7 | §3.3 OWASP 10/10 ya da N/A + gerekçe | kontrol tablosu |
| 8 | §3.4 bulgu kalıbı 9 alan, bulgu başına eksiksiz | bulgu blokları |
| 9 | §3.5 VersaCoder özel 8 alanı denetlendi | özel alan tablosu |
| 10 | §3.6 register kolonları dolu; sayısal olasılık yok | register tablosu |
| 11 | Şiddet §3.4.1 ölçeğinde (LOW/MEDIUM/HIGH/CRITICAL) | şiddet sütunu |
| 12 | Düzeltme önceliklendirme sırası §3.10’a uygun | plan tablosu |
| 13 | Yer tutucu deseni yalnız §3.9 bölümünde | desen taraması |
| 14 | Kırık link = 0 | §5.3 madde 4 |
| 15 | Link whitelist (§7.1) ihlali = 0 | §5.3 madde 4 |
| 16 | Halisasyon etiketleri gerektiği yerde (§3.8) | etiket listesi |
| 17 | İmza / tarih bloğu dolu, onaylayan satırı boş değil | §3.10 |
| 18 | Quality Report üretildi, kararı PASS; CRITICAL = 0 | §5.4 |

### §6.2 Red (ne zaman reddedilir)

Aşağıdakilerden **herhangi biri** yeterli reddir; müzakere yok:

1. Frontmatter 8 alan değil ya da kapanış 10. satırda değil (G1).
2. §1…§7 başlıklarından biri eksik ya da yer değiştirmiş (G12).
3. Kanıtsız bulgu: kontrol + kanıt + şiddet üçlüsünden biri eksik (G7).
4. Whitelist dışı ya da yasaklı hedefe link var (G5).
5. Register’da sayısal olasılık yüzdesi (G9 — uydurma).
6. Raporun kendisinde secret deseni yakalandı (§5.3 madde 4).
7. Mojibake, Latin-1/CP1254 ya da BOM tespiti (G3).
8. Aynı raporda iki kural ihlali birden (§4.5 son cümlesi).

### §6.3 Kanıt formatı

Kanıt tek satırda; dört biçimden biri:

```
  KANIT: <komut> -> <ciktinin ilgili bolumu>
  KANIT: <dosya yolu>:<satir no>
  KANIT: <uretici/standart belge> + <surum> (bilesen bulgulari icin)
  KANIT: <ekran goruntusu dosyasi> (yalniz arayuz iddialari icin)
```

Yasak: “kritik görünüyor”, “muhtemelen açık” gibi kanıtsız ifadeler.
Kanıt gelmemişse bulgu `draft` kalır ve metne `⚠️ VERIFICATION REQUIRED` konur.
Secret örneği kanıta asla gerçek değerle değil, `<REDACTED>` yer tutucusuyla
yazılır.

### §6.4 Yeniden doğrulama

Düzeltme sonrası **tüm** §6.1 18 maddesi yeniden çalıştırılır; kısmi
tarama geçersizdir (G12). Düzeltmenin kendisi de yeniden taranır:
kapalı bulgu yalnız yeni kanıt satırıyla `fixed` olur. İmza bloğundaki
“yeniden denetim” tarihi bu adımda tazelenir (§3.10). Statü ancak ikinci
tarama PASS ise review → active’e geçer.

### §6.5 Sözleşme özeti

Sözleşmenin otomatik denetlenebilir üçlüğü: **8 alanlı frontmatter +
7 bölüm + 9 alanlı bulgu kalıbı**. Bu üçlüden biri kırılırsa rapor
kalitesi otomatik FAIL’tir; manuel yorum üçlüğü değiştirmez.

---

## §7 Referanslar

### §7.1 Link beyaz listesi

Bu şablondan ve üretilen raporlardan link atılabilecek **tek** hedefler
aşağıdadır; listede olmayan her hedef G5 ihlalidir.

**A — Bağımlı hedefler (göreli: documentation/ altından):**

| # | Hedef | Not |
|---|-------|-----|
| 1 | `../index.md` | Zorunlu bağlantı satırının üst kaynağı |
| 2 | `./api-doc-template.md` | Kardeş |
| 3 | `./claude-md-template.md` | Kardeş |
| 4 | `./docs-md-template.md` | Zorunlu bağlantı satırı |
| 5 | `./katman-readme-template.md` | Kardeş |
| 6 | `./readme-template.md` | Kardeş |
| 7 | `./root-md-template.md` | Kardeş |
| 8 | `./WikiPage-Template.md` | Kardeş |
| 9 | `../adr/adr-security-template.md` · `adr-template.md` · `adr-index.md` | ADR (3/7) |
| 10 | `../adr/adr-nygard-template.md` · `adr-architecture-template.md` · `adr-data-template.md` · `adr-devexpress-template.md` | ADR (4/7) |
| 11 | `../agents/agents-template.md` · `subagent-template.md` | 2/2 |
| 12 | `../architecture/architecture-template.md` · `ascii-diagram-template.md` · `layer-spec-template.md` | 3/4 (bkz. §7.2-7) |
| 13 | `../backend/handler-template.md` · `repository-template.md` · `service-template.md` | 3/3 |
| 14 | `../infrastructure/configuration-template.md` · `dapper-mapping-template.md` · `migration-template.md` · `provider-template.md` | 4/4 |
| 15 | `../query/Query-Template.md` | 1/1 |
| 16 | `../testing/test-plan-template.md` · `unit-test-template.md` | 2/3 (bkz. §7.2-8) |
| 17 | `../frontend/form-template.md` | 1/2 (bkz. §7.2-9) |
| 18 | `../ui-design/wireframe-template.md` | 1/5 (bkz. §7.2-10) |
| 19 | `../session-log-template.md` | Oturum kaydı |
| 20 | `../../../CLAUDE.md` · `../../../AGENTS.md` · `../../../README.md` · `../../../WORKFLOW.md` | Depo kökü (4/4) |

**B — Yasaklı hedefler (metin olarak anılabilir, link olarak HAYIR):**

| Hedef | Gerekçe |
|-------|---------|
| `.ai/index.md` · `.ai/CLAUDE.md` · `.ai/WORKFLOW.md` | Diskte yok (`.ai/` altında yalnız `.templates` ve `reports`) |
| `.ai/.rules/` · `.ai/.decisions/` · `.ai/architecture/` | Diskte yok |
| `../CLAUDE.md` | `.ai/.templates/CLAUDE.md` var ama kaynak zinciri dışında; bkz. §7.2-2 |
| `.ai/keys.md` · `.ai/log.md` | Diskte yok |
| `../../../src/` · `../../../tests/` | Diskte yok; G8 ayrıca yasak |

### §7.2 Red Team notları (≥10 satır)

1. Bu şablon diske **önce** yazıldı, **sonra** kurallara denetlendi;
   tersi olsaydı G12 ihlali kaçınılmazdı.
2. `../CLAUDE.md` hedefi teknik olarak mevcut (`.ai/.templates/CLAUDE.md`);
   yine de yasaklıdır: kaynak zinciri (`../index.md`) dışına link
   çekmek denetim raporunu genel kural dosyasına bağlar — konu kayması.
3. Rapor **kendisinde** secret deseni bulunması en utanç verici hatadır;
   §5.3 madde 4 bu yüzden kanıt satırlarında `<REDACTED>` zorunlu kılar.
4. Sayısal olasılık (“%70 ihtimal”) G9 ihlalidir: ölçüm yoksa yüzde de
   yoktur; §3.6 yalnız kategorik ölçek kabul eder.
5. Diskte `src/` ve `tests/` yok: OWASP A06/A08 kontrollerinde
   sürüm/süreç iddiaları `⚠️ VERIFICATION REQUIRED` ile başlamalıdır (G7).
6. Yasaklı eski marka adı bu dosyada hiç geçmez;
   §5.3 madde 5 birleşik literal kurar — dosya içi arama boş döner.
7. `../architecture/` 4 kardeş, whitelist 3: `dependency-rule-template.md`
   dışarıda — rapor kural metni üretmez (SRP, §1.4).
8. `../testing/` 3 kardeş, 2 linklenir: `integration-test-template.md`
   yalnız API dokümanının konusudur.
9. `../frontend/` 2, `../ui-design/` 5 kardeş; whitelist sırasıyla
   `form-template.md` ve `wireframe-template.md` ile sınırlı —
   geri kalanı ilgili şablonun sorumluluğu.
10. En olası hata: yer tutucu kalıbının §3.9 dışına taşması; §6.1 madde 13
    bunu yakalar, yakalamazsa §5.4 sayacı kırmızı kalır.
11. En pahalı hata: kırık link. §5.3 madde 4 hiçbir koşulda “atla”
    denemez; hedef 0’dır.
12. `CRITICAL` bulgusu varken imza bloğu dolu olsa bile karar FAIL’tir;
    imza, riskin kabulü değildir (§5.4 son satır).

---

*Bu dosya bir şablondur; üretilen raporda yer tutucu adları doldurulur,
shablon metni olduğu gibi kopyalanmaz.*