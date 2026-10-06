---
title: "VersaCoder — Wiki Sayfası Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "wiki sayfasi sablonu: frontmatter, ozet, baglanti agi ve 15 bolumlu govde"
---

# VersaCoder — Wiki Sayfası Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [docs-md](./docs-md-template.md)

---

## §1 Amaç

`C:\versacoder\.ai\documentation\wiki\<konu>.md` dosyasının **tek üretim
kaynağıdır**. Wiki sayfası kodun kopyası değil, sistemin **hafızasıdır**:
konu neden var, kimin işi, hangi karar bağlamında yazıldı, hangi dosyalara
dokunuyor. Kod değişince sayfa değil **bağlam** değişir; bu şablon bağlamı
kilitler.

SRP gereği bu şablon **kod yazmaz, SQL yazmaz, ADR üretmez**: karar
`../adr/` altındaki ADR’e, sorgu `../query/Query-Template.md`’ye, test
`../testing/` altındaki şablona devredilir — burada yalnız **bilgi sayfası**
üretilir.

| Alan | Değer |
|------|-------|
| Template Name | `WikiPage-Template.md` |
| Template Path | `.ai/.templates/documentation/WikiPage-Template.md` |
| Hedef Dosya | `.ai/documentation/wiki/<konu>.md` |
| Hedef Dizin | `.ai/documentation/wiki/` — yoksa üretilir |
| Zorunlu Yapı | 7 bölüm `§1…§7` + 15 bölümlü gövde (§3.4) |
| Üretilen Uzunluk | `180–450 satır` · `10–24 KB` |
| Kodlama | **UTF-8 (BOM’suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe (ç ğ ı İ ö ş ü) — dosya yolu, alan adı, komut İngilizce |

### §1.1 Ne üretir

Bu şablon dört çıktı verir:

1. **Frontmatter** — 8 alan, 10. satırda kapanır (G1).
2. **Tek cümlelik özet** — sayfanın ne olduğunu 25 kelimede söyler.
3. **Bağlantı ağı** — üst / alt / ilişkili üç blok; kırmızı bayrak yok.
4. **15 bölümlü gövde** — Amaç’tan Referanslar’a kadar eksiksiz.

### §1.2 Disk Kanıtları

Şablon yazım öncesi ve sonrası şu disk gerçeklerini okur; hiçbiri
tahminle doldurulmaz:

```
  Get-ChildItem C:\versacoder\.ai\documentation\wiki -Filter *.md -Recurse
  Get-Item    C:\versacoder\.ai\.templates\documentation\WikiPage-Template.md
  (Get-Content C:\versacoder\.ai\documentation\wiki\<konu>.md).Count
```

Beklenen: hedef dizin varsa sayfa sayısı, yoksa `0`. Dizin yoksa bu şablon
önce dizini üretir, sonra sayfayı yazar.

### §1.3 Ne zaman kullanılır

| Durum | Kullanım |
|-------|----------|
| Yeni alt sistem öğreniliyor | Sayfa `draft` açılır |
| ADR onaylandı | ADR bağlantısı sayfaya geri eklenir |
| Kod yolu değişti | §3.10 Uygulama + §6.3 Kanıt tazelenir |
| Sayfa 6 ay okunmadı | §7.2 Red Team’e girer, statü düşer |

### §1.4 Kapsam dışı (SRP)

Bu şablon **şunları üretmez**: ADR (bkz. `../adr/`), API sözleşmesi
(bkz. `./api-doc-template.md`), genel doküman (bkz. `./docs-md-template.md`),
kural metni (bkz. `./root-md-template.md`). Karışan konu
**CONFLICT DETECTED** olarak §7.2’ye yazılır.

+----------------------------------------------------------------+
|Soru: Bu içerik bir wiki sayfasina mi ait?                      |
|                                                                |
|  Karar agaci                                                   |
|  ----------                                                    |
|   karar mi?       -- EVET --> ../adr/ adr-template             |
|   sozlesme mi?    -- EVET --> ./api-doc-template.md            |
|   kural mi?       -- EVET --> ./root-md-template.md            |
|   hafiza/baglam   -- EVET --> BU SABLON (WikiPage-Template)    |
|   diger           --------> ./docs-md-template.md              |
|                                                                |
|Kirmizi bayrak: hic baglantisi olmayan sayfa (G4) kapali kalmaz.|
+----------------------------------------------------------------+

---

## §2 Kapsam

### §2.1 Kapsam içi

Bu şablon şu içerikleri kapsar:

- `.ai/documentation/wiki/**/*.md` altındaki tüm sayfalar;
- sayfa frontmatter’ı, lede’si, bağlantı ağı ve 15 bölümlü gövdesi;
- statü geçişleri (`draft → review → active → deprecated`);
- kırık link denetimi ve §5.4 Quality Report çıktısı;
- versiyonlama (§3.10) ve Owner / Read-Write yetkisi (§3.10).
- §6.1 21 maddelik doğrulama listesi, §5.4 Quality Report kalıbı;
- §7.1 link beyaz listesi ve §7.2 Red Team notları (≥10 satır).

### §2.2 Kapsam dışı

Aşağıdakiler bu şablonun dışındadır; sahipleri parantez içinde:

- ADR metni ve karar gerekçesi (`../adr/`);
- endpoint sözleşmeleri, DTO, hata kodları (`./api-doc-template.md`);
- kural metni ve denetim listesi (`./root-md-template.md`);
- mimari diyagram üretimi (`../architecture/ascii-diagram-template.md`);
- test senaryoları ve asanlık raporu (`../testing/`).

Kapsam sınırı ihlali = **CONFLICT DETECTED** etiketi ve §7.2 Red Team
kaydı (G6).

## §3 Mimari

Şablonun gövde mimarisi altı katmanlıdır. Her katman bir alt bölüme
karşılık gelir ve sırayla doldurulur; bir katman tamamlanmadan sonraki
katmana geçilmez (G12 · §4.5).

### §3.1 Frontmatter bloğu

İlk 9 satır 8 alan içerir; kapanış `---` 10. satırdadır (G1 · §4.1).

| # | Alan | Zorunlu değer | Not |
|---|------|--------------|-----|
| 1 | `title` | `"… - <Konu>"` | Wiki sayfasının tam adı |
| 2 | `type` | `template` | Belge tipi |
| 3 | `category` | `template` | Dizin ile aynı kategori |
| 4 | `status` | `draft` / `review` / `active` / `deprecated` | Yaşam döngüsü |
| 5 | `version` | `x.y.z` | Semver; bkz. §3.10 |
| 6 | `updated` | `YYYY-MM-DD` | Düzenleme tarihi |
| 7 | `authority` | `SSOT` | Tek doğruluk kaynağı iddiası |
| 8 | `scope` | Bir cümle | Sayfanın sınırları |

Örnek (dolu):

```yaml
---
title: "VersaCoder — Query Katmanı Wiki"
type: template
category: template
status: draft
version: 0.1.0
updated: 2026-10-05
authority: SSOT
scope: "query katmaninin sorgu yazim kurallari ve dosya haritasi"
---
```

### §3.2 Tek cümlelik özet (lede)

H1’den hemen sonra tek paragraf, en fazla 25 kelime. Cümle şudur:
**bu sayfa neyi, kimin için, hangi bağlamda** tanımlar. Lede dolmadan
§6.1 maddeleri 1-3 geçersizdir.

Kalıp: `Bu sayfa, <hedef kitle> için <konu> <amaç> tanımlar.`

Örnek: “Bu sayfa, VersaCoder katman 6 (query) için sorgu yazım
kurallarını veritabanı bağımsızlığı bozulmadan tanımlar.”

### §3.3 Bağlantı ağı (üst / alt / ilişkili)

Bağlantı ağı üç zorunlu bloktur; bir wiki sayfası bağlantı olmadan
tamamlanmış sayılmaz (G4 · §4.3).

| Blok | Yön | Kaynak | Adet |
|------|-----|--------|------|
| Üst | yukarı | `[index](../index.md)` | 1 (zorunlu satır) |
| Alt | çocuk | `./` altındaki kardeş wiki sayfaları | 0-5 |
| İlişkili | yan | §7.1’deki whitelist’ten | 2-8 |

Her bağlantı **ad + gerekçe** çifti verilir; yalnız URL yazılmaz. Kırmızı
bayrak: hiç bağlantısı olmayan sayfa (G4).

Dolu örnek (bağlantı ağı bloğu):

```markdown
**Bağlantı Ağı**

- Üst: [index](../index.md) — şablon ana dizini (zorunlu)
- Üst: [Query-Template](../query/Query-Template.md) — sorgu kuralları bu sayfayı besler
- Alt: [unit-test-template](../testing/unit-test-template.md) — sorgu testleri bu sayfadan türetilir
- İlişkili: [service-template](../backend/service-template.md) — sorguyu çağıran katman
- İlişkili: [adr-data-template](../adr/adr-data-template.md) — veri kararı bağlamı
```

Sayı salınımı: alt bağlantı 0 ise sayfa bir yapraktır ve §7.2 Red Team
listesinde “yalnız yaprak” notuyla izlenir; ilişkili bağlantı 2’den
azsa konu izole edilmiş, 8’den çoksa konu dağılmış sayılır (G4).
### §3.4 Gövde formatı — 15 bölümlü şablon

Wiki gövdesi aşağıdaki 15 bölümü sırayla kullanır. Her bölüm başlığı
 `###` (H3) seviyesinde açılır; bölüm adları kısaltılmaz,
 yeniden adlandırılmaz.

| # | Bölüm | İçerik | Zorunlu |
|---|-------|--------|---------|
| 1 | Amaç | Neden var, hangi problemi çözer | EVET |
| 2 | Kapsam | Kapsam içi / dışı, sınır çizgisi | EVET |
| 3 | Bağlam | Tarihçe, tetikleyen olay, ilgili sistemler | EVET |
| 4 | Gereksinimler | Numaralı, ölçülebilir maddeler | EVET |
| 5 | Mimari | Diyagram, bileşen listesi, veri akışı | EVET |
| 6 | Bağımlılıklar | Ekip, sistem, paket, sürüm | EVET |
| 7 | Kısıtlar | Performans, güvenlik, uyumluluk, süre | EVET |
| 8 | Riskler | Olasılık × etki, azaltma planı | EVET |
| 9 | Kararlar | ADR bağlantısı + karar özeti | EVET |
| 10 | Uygulama | Kod yolları, dosyalar, giriş noktaları | EVET |
| 11 | Doğrulama | Test, komut, beklenen çıktı | EVET |
| 12 | Kabul Kriterleri | Ölçülebilir, evet/hayır yanıtlanabilir | EVET |
| 13 | Kanıt | Komut çıktısı, satır no, ekran görüntüsü | EVET |
| 14 | Açık Sorular | `[NOT SPECIFIED]` + soru sahibi + tarih | EVET |
| 15 | Referanslar | §7.1 listesi | EVET |

Kural: 15 bölümün hiçbiri silinemez. Boş bırakılacaksa başlık kalır ve
altına `[NOT SPECIFIED]` yazılır (G12).

### §3.5 Bağımlılık haritası

Her wiki sayfası bağımlılıklarını iki tablo halinde verir:

- **Yukarı bağımlılık** — bu sayfayı besleyen kaynaklar (üst sayfa, ADR,
  karar).
- **Aşağı bağımlılık** — bu sayfaya bağlı alt sayfalar ve kod yolları.

Tablo kolonları: `Kaynak | Tür | Sürüm | Etki | Sahip`. Sürüm bilinmiyorsa
 `[NOT SPECIFIED]` yazılır; uydurulmaz (G12).

### §3.6 Yaşam döngüsü ve statü geçişleri

Statüler ve izin verilen geçişler:

```
  draft ---> review ---> active ---> deprecated
    ^          |                      |
    +----------+-- rejected <---------+
```

Geçiş kuralı: `active` yalnız §6.1 maddelerinin tamamı yeşil ise
açılır. `deprecated` sayfa H1’in altına uyarı satırı alır ve §7.2
Red Team listesine girer (G6 · §4.4).

### §3.7 Kalite sözleşmesi

Bu şablon kalite sözleşmesini `../index.md` §3.7’den devralır ve
wiki sayfasına uyarlar:

| Sözleşme | Wiki karşılığı |
|----------|----------------|
| 7 bölüm eksiksiz | §1…§7 başlıkları sırayla |
| 8 alanlı frontmatter | §3.1 tablosu |
| Kanıt satırı | §6.3’te komut çıktısı / satır no |
| Bozuk link yok | §3.11 denetimi + §6.1 madde 10 |
| Halisasyon etiketleri | §3.8 |
| Türkçe, UTF-8, BOM’suz | §6.1 madde 9 |

Not: 15 bölümlü gövde bazı eski notlarda `§67 gövde formatı` diye
anılır; bu adlandırmanın numaralandırma kaynağı **[NOT SPECIFIED]**’tir
(G12 · uydurma yok). Şablon içinde tek geçerli ad §3.4 tablosundakidir.

### §3.8 Kanıt ve etiket sistemi

Üç zorunlu etiket; biri olmadan sayfa `active` olamaz:

| Etiket | Ne zaman | Eylem |
|--------|----------|-------|
| `⚠️ VERIFICATION REQUIRED` | İddia disk kanıtıyla görülmedi | Tarama yapılana kadar kalır |
| `[NOT SPECIFIED]` | Bilgi vault’ta yok | Soru sahibi + tarih yazılır |
| `CONFLICT DETECTED` | İki kaynak çelişiyor | İkisi de linklenir, çelişki yazılır |

Etiketler H1 ile lede arasına, ayrıca ilgili bölümün altına da konur;
tek yerde unutulmaz (G12).

### §3.9 Dolu VersaCoder Örneği + Kalıp Blok

Önce dolu örnek (Query katmanı wiki sayfası, ilk 40 satır):

```markdown
---
title: "VersaCoder — Query Katmanı Wiki"
type: template
category: template
status: draft
version: 0.1.0
updated: 2026-10-05
authority: SSOT
scope: "query katmaninin sorgu yazim kurallari ve dosya haritasi"
---

# VersaCoder — Query Katmanı Wiki

**Zorunlu Bağlantılar:** [index](../index.md) · [Query-Template](../query/Query-Template.md)

Bu sayfa, VersaCoder katman 6 (query) ekibi için sorgu yazım kurallarını
veritabanı bağımsızlığı bozulmadan tanımlar.

## §1 Amaç

N + 1 sorgu ve `SELECT *` (G10) kaynaklı regresyonları önleyen tek
kaynak. Sahip: query ekibi · Durum: draft.

## §6 Doğrulama

Kanıt: `Get-ChildItem .ai/documentation/wiki/*.md | Measure-Object` → 12 sayfa

```

Sonra kalıp blok — bu şablonda `{{PLACEHOLDER}}` adları **yalnızca**
aşağıdaki blokta geçer; dosyanın başka yerinde `{{` açılışı yasaktır
(§6.1 madde 14):

```markdown
## §6 Doğrulama

### §6.1 Doğrulama Maddeleri

| # | Madde | Durum |
|---|-------|-------|
| 1 | Frontmatter 8 alan, 10. satırda kapanır | {{V_FM}} |
| 2 | H1 + zorunlu bağlantı satırı birebir | {{V_H1}} |
| 3 | Lede en fazla 25 kelime | {{V_LEDE}} |
| 4 | §1…§7 başlıkları sırayla mevcut | {{V_SEC}} |
| 5 | §3.4 15 bölümlü gövde eksiksiz | {{V_BODY}} |
| 6 | Bağlantı ağı: üst / alt / ilişkili dolu | {{V_LINK}} |
| 7 | Link whitelist ihlali yok | {{V_WHITE}} |
| 8 | Halisasyon etiketi gerekli yerlerde var | {{V_TAG}} |
| 9 | UTF-8 (BOM’suz), Türkçe karakterler sağlam | {{V_ENC}} |
| 10 | Kırık link sayısı 0 | {{V_BROKEN}} |
| 11 | Sahip + statü + sürüm yazıyor | {{V_OWNER}} |
| 12 | Kanıt satırı komut çıktılı | {{V_EVID}} |

Test: `./check-wiki.ps1 -Path .ai/documentation/wiki/{{KONU}}.md`

```

### §3.10 Düzenleme politikası (Owner / Lifecycle / Read-Write)

Her sayfanın üç meta alanıdır; tablo sayfanın sonunda `### Sahiplik` başlığıyla bulunur:

| Alan | İçerik | Kural |
|------|--------|-------|
| Owner | Ekip adı (tek) | Boşsa sayfa draft’a düşer |
| Lifecycle | `draft → review → active → deprecated` | Geçiş §6.1 yeşil ise |
| Read-Write | Kimler düzenleyebilir | Owner + 1 kişi; diğerleri öneri |

Versiyonlama semver’dir: içerik düzeltmesi **patch** (+0.0.1), yeni
bölüm/alt sistem **minor** (+0.1.0), format değişimi (§3.4’e yeni bölüm)
**major** (+1.0.0). `updated` alanı her geçişte tazelenir.

### §3.11 Kırık link denetimi ve Quality Report

Yazım sonrası zorunlu iki çıktı:

1. **Kırık link taraması** — §5.3’teki salt-okunur komut; hedef 0 kırık.
2. **Quality Report** — §5.4 kalıbı; satır / KB / bölüm / link / etiket
   sayaçları.

Report satırı şu şablondadır (dosya adı değil **rapor gövdesi**):

```
WikiPage Quality Report — <konu>.md (YYYY-MM-DD HH:MM)
  satır: <n>  KB: <n,n>  §1-§7: <7/7>  §3.4 govde: <15/15>
  link: <toplam>  kirik: 0  etiket: <n>
```

Bir sayaç kırmızı ise sayfa statüsü `active` olamaz (G6 · §4.4).

---

## §4 Kurallar

### §4.1 G1–G3: Yapı ve kodlama

| Kural | Metin | İhlal cezası |
|-------|-------|-------------- |
| G1 | Frontmatter ilk 9 satırda 8 alan; kapanış `---` 10. satır; 8. alan `scope` | Sayfa `draft`’a döner |
| G2 | EF Core tek ORM’dir; başka ORM adı geçmez | CONFLICT DETECTED |
| G3 | Dosya UTF-8 (BOM’suz), Türkçe (ç ğ ı İ ö ş ü); Latin-1/CP1254 YASAK | Sayfa reddedilir |

G1 detayı: alan sırası `title, type, category, status, version, updated,
authority, scope`. `updated: 2026-10-05` bu şablonun kendi tarihidir;
üretilen sayfada yazım tarihi kullanılır.

### §4.2 G4–G6: Bağlantı ve kapsam

| Kural | Metin | İhlal cezası |
|-------|-------|--------------
| G4 | Bağlantı ağı (üst / alt / ilişkili) zorunlu; hiç linksiz sayfa yazılmaz | Kırmızı bayrak |
| G5 | Link yalnız whitelist’ten (§7.1); yasaklı hedefe link yasak | Bozuk link sayılır |
| G6 | Kapsam dışı konu + Red Team kaydı: `CONFLICT DETECTED` etiketi | Sayfa review’a düşer |

### §4.3 G7–G9: Kanıt ve doğrulama

| Kural | Metin | İhlal cezası |
|-------|-------|--------------
| G7 | Her iddia kanıtsız yazılmaz; kanıt = komut çıktısı ya da satır no | `⚠️ VERIFICATION REQUIRED` |
| G8 | `GATE-003` onayından önce `src/**` altına satır yazılmaz | Kapalı kapı |
| G9 | Bilinmeyen bilgi uydurulmaz: `[NOT SPECIFIED]` + soru sahibi + tarih | Hallisasyon |

Not: G8 bu şablon için ölü maddedir — şablon zaten `src/**`’e yazılmaz;
yine de madde, üretilen wiki sayfasının “Uygulama” bölümündeki kod
yolu iddialarını bağlar.

### §4.4 G10–G12: İçerik ve format

| Kural | Metin | İhlal cezası |
|-------|-------|--------------
| G10 | `SELECT *` yasak; sütun listesi açık yazılır | Bulgu (HIGH) |
| G11 | Ürün adı **VersaCoder**, katman adı `l0..l7`; yasaklı eski marka adı geçmez | Marka ihlali |
| G12 | 7 bölüm + §3.4 15 gövde bölümü eksiksiz; boş bölüm `[NOT SPECIFIED]` ile kalır | Bölüm silinemez |

G11 not: yasaklı ürün adı bu dosyada yalnızca “adı yasak” metni olarak
geçer; hiçbir yerde ürün adı gibi anılmaz.

### §4.5 Kural özeti (G1..G12)

```
  G1  Frontmatter 8 alan / 10. satir kapanis
  G2  EF Core tek ORM
  G3  UTF-8 BOM-suz + Turkce
  G4  Baglanti agi zorunlu (ust/alt/iliskili)
  G5  Link yalniz whitelist
  G6  Kapsam disi = CONFLICT DETECTED + Red Team
  G7  Her iddia kanitli
  G8  GATE-003 oncesi src/** kapali
  G9  [NOT SPECIFIED] + soru sahibi + tarih
  G10 SELECT * yasak
  G11 VersaCoder / l0..l7 / yasakli marka adi gecmez
  G12 7 bolum + 15 govde bolumu eksiksiz
```

Aynı anda ihlal edilen kural sayısı ikiyi geçerse sayfa doğrudan
`draft` statüsüne geri alınır ve §7.2 Red Team listesine eklenir.

---

## §5 Workflow

### §5.1 Girdi

| Girdi | Kaynak |
|-------|--------|
| Konu + sahip ekip | Talep metni |
| Bağlı ADR / Query / test şablonları | `../` altındaki kardeşler |
| Disk kanıtları | §5.3 salt-okunur tarama |
| Mevcut kardeş sayfalar | `.ai/documentation/wiki/` |

### §5.2 Adımlar

| # | Adım | Çıktı |
|---|------|-------|
| 1 | Dizin ve mevcut sayfaları tara (§5.3) | Dosya listesi |
| 2 | Frontmatter + H1 + zorunlu bağlantı satırı | 10 satır |
| 3 | Lede (≤25 kelime) + bağlantı ağı | 3 blok |
| 4 | §1…§7 + §3.4 15 bölümlü gövde | Gövde |
| 5 | Kanıt satırları + etiketler | §6.3 |
| 6 | Kırık link denetimi (§5.3) | 0 kırık |
| 7 | Quality Report (§5.4) | Rapor satırı |
| 8 | Statü geçişi (§3.6) | draft → review |

### §5.3 Salt-okunur PowerShell taraması

Hiçbiri dosyaya yazmaz; çalıştırılması güvenlidir:

```powershell
# 1) Hedef dizin ve sayfa sayisi
Get-ChildItem C:\versacoder\.ai\documentation\wiki -Filter *.md -Recurse -ErrorAction SilentlyContinue |
  Select-Object FullName, Length, LastWriteTime

# 2) Satir / KB sayaci (sartname: >=180 satir, >=10 KB)
$f = "C:\versacoder\.ai\documentation\wiki\<konu>.md"
if (Test-Path $f) {
  $c = Get-Content -LiteralPath $f -Encoding UTF8
  [pscustomobject]@{ Satir = $c.Count; KB = [math]::Round((Get-Item $f).Length / 1KB, 1) }
}

# 3) Bolum basliklari (beklenen: 7 adet "## §")
(Get-Content -LiteralPath $f -Encoding UTF8) -match "^## §"

# 4) Baglanti hedefleri (salt okunur) — whitelist disi aranir
(Get-Content -LiteralPath $f -Encoding UTF8) |
  Select-String -Pattern "\]\(([^)]+)\)" -AllMatches |
  ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

# 5) Turevli ad tespiti (G11) — birlestirilmis literal, dosyada gecmez
$yasak = ("Core" + "Music")
(Get-Content -LiteralPath $f -Encoding UTF8) | Where-Object { $_ -match $yasak }
```

Beklenen çıktı: madde 4 listesindeki her hedef §7.1 whitelist’inde;
madde 5 boş.

### §5.4 Quality Report kalıbı

Yazım biterken tek blok halinde üretilir:

```
WikiPage Quality Report — <konu>.md (YYYY-MM-DD HH:MM)
  satır: <n> (>=180)   KB: <n,n> (>=10,0)
  §1-§7: <7/7>   §3.4 govde: <15/15>   §3.x alt bolum: <11/11>
  link: <toplam>  kirik: 0  whitelist-ihlali: 0
  etiket: <n>   PLACEHOLDER: yalniz §3.9 bolumunde
  G1..G12: 12/12   karar: PASS | FAIL
```

Dolu örnek (Query katmanı sayfası, çıktı):

```
WikiPage Quality Report — query-katmani.md (2026-10-05 14:32)
  satır: 412 (>=180)   KB: 27,3 (>=10,0)
  §1-§7: 7/7   §3.4 govde: 15/15   §3.x alt bolum: 11/11
  link: 7  kirik: 0  whitelist-ihlali: 0
  etiket: 2   PLACEHOLDER: yalniz §3.9 bolumunde
  G1..G12: 12/12   karar: PASS | FAIL
```

---

## §6 Doğrulama

### §6.1 Doğrulama Maddeleri

Sayfa §5.2 adım 8’de **21 maddenin tamamı** yeşil olmadan review’a
alınmaz. Durum sütunu: `PASS` / `FAIL` / `N/A` (N/A gerekçesi zorunlu).

| # | Madde | Kanıt |
|---|-------|-------|
| 1 | UTF-8 (BOM’suz); Latin-1/CP1254 yok | §5.3 madde 2 |
| 2 | Türkçe karakterler sağlam (ç ğ ı İ ö ş ü) | gözlem |
| 3 | Frontmatter ilk 9 satır, 8 alan, sırası §4.1 ile uyumlu | satır 1-10 |
| 4 | Kapanış `---` tam 10. satırda | satır no |
| 5 | H1 tek tane, dosyanın adıyla uyumlu | §5.3 madde 3 |
| 6 | Zorunlu bağlantı satırı birebir §7.1-A | satır no |
| 7 | Lede ≤25 kelime ve tek paragraf | kelime sayacı |
| 8 | §1…§7 başlıkları tam ve sırayla (7/7) | §5.3 madde 3 |
| 9 | §1.1-§1.4, §2.1-§2.2 alt başlıkları mevcut | başlık listesi |
| 10 | §3.1…§3.11 = 11 alt bölüm eksiksiz | başlık listesi |
| 11 | §3.4 15 bölümlü gövde tablosu dolu (15/15) | tablo |
| 12 | Bağlantı ağı üç blok: üst / alt / ilişkili | §3.3 |
| 13 | Kırık link = 0 | §5.3 madde 4 |
| 14 | Link whitelist (§7.1) ihlali = 0 | §5.3 madde 4 |
| 15 | Yer tutucu deseni (çift süslü parantez) yalnız §3.9’da | desen taraması |
| 16 | Yasaklı link hedefi yok | §7.1-B |
| 17 | §4.1-§4.5 içinde G1..G12’nin 12’si de geçiyor | §4.5 kutusu |
| 18 | Halisasyon etiketleri gerektiği yerde (§3.8) | etiket listesi |
| 19 | Kanıt satırı var: komut çıktısı ya da satır no | §6.3 |
| 20 | Owner / Lifecycle / Read-Write tablosu dolu (§3.10) | tablo |
| 21 | Quality Report üretildi, kararı PASS (§5.4) | rapor bloğu |

### §6.2 Red (ne zaman reddedilir)

Aşağıdakilerden **herhangi biri** yeterli reddir; müzakere yok:

1. Frontmatter 8 alan değil ya da kapanış 10. satırda değil (G1).
2. §1…§7 başlıklarından biri eksik ya da yer değiştirmiş (G12).
3. Whitelist dışı ya da yasaklı hedefe link var (G5); kırık link ayrıca
   bozuk link sayılır.
4. Kanıtsız iddia, etiketsiz bırakılmış (G7, G9).
5. Mojibake, Latin-1/CP1254 ya da BOM tespiti (G3).
6. Aynı dosyada iki kural ihlali birden (§4.5 son cümlesi).
7. §5.4 Quality Report kararı FAIL.

### §6.3 Kanıt formatı

Kanıt tek satırda; üç biçimden biri:

```
  KANIT: <komut> -> <kodun ilgili ciktisi>
  KANIT: <dosya yolu>:<satir no>
  KANIT: <ekran goruntusu dosyasi> (yalniz arayuz iddialari icin)
```

Yasak: “görüldü”, “doğrulandı” gibi kanıtsız onay kelimeleri. Kanıt
gelmemişse madde `FAIL` olur ve ilgili metne `⚠️ VERIFICATION REQUIRED` konur.

Dolu üç örnek:

```
  KANIT: Get-Content .ai/documentation/wiki/query-katmani.md | Measure-Object -> 412
  KANIT: .ai/.templates/documentation/WikiPage-Template.md:10
  KANIT: wireframe-ekran.png (Sidebar bileşeni klavye odağı iddiası)
```

Kanıt satırı aranan dosyada **bulunamazsa** madde otomatik FAIL’tir; ekran görüntüsü tek başına kod yolu iddiasını kanıtlamaz.

### §6.4 Yeniden doğrulama

Düzeltme sonrası **tüm** §6.1 21 maddesi yeniden çalıştırılır; kısmi
tarama geçersizdir (G12). Komut seti §5.3; çıktı karşılaştırması
§5.4 bloğuyla yapılır. Statü ancak ikinci tarama da PASS ise
draft → review’a geçer (§3.6).

### §6.5 Sözleşme özeti

Sözleşmenin otomatik denetlenebilir üçlüğü: **8 alanlı frontmatter +
7 bölüm + 15 gövde bölümü**. Bu üçlüden biri kırılırsa sayfa kalitesi
otomatik FAIL’tir; manuel yorum üçlüğü değiştirmez.

---

## §7 Referanslar

### §7.1 Link beyaz listesi

Bu şablondan ve üretilen wiki sayfalarından link atılabilecek **tek**
hedefler aşağıdadır; listede olmayan her hedef G5 ihlalidir.

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
| 8 | `./WikiPage-Template.md` | Kendisi |
| 9 | `../adr/adr-template.md` · `adr-index.md` · `adr-nygard-template.md` | ADR (3/7) |
| 10 | `../adr/adr-architecture-template.md` · `adr-data-template.md` · `adr-security-template.md` · `adr-devexpress-template.md` | ADR (4/7) |
| 11 | `../agents/agents-template.md` · `subagent-template.md` | 2/2 |
| 12 | `../architecture/architecture-template.md` · `ascii-diagram-template.md` · `layer-spec-template.md` | 3/4 (bkz. §7.2-6) |
| 13 | `../backend/handler-template.md` · `repository-template.md` · `service-template.md` | 3/3 |
| 14 | `../infrastructure/configuration-template.md` · `dapper-mapping-template.md` · `migration-template.md` · `provider-template.md` | 4/4 |
| 15 | `../query/Query-Template.md` | 1/1 |
| 16 | `../testing/test-plan-template.md` · `unit-test-template.md` | 2/3 (bkz. §7.2-7) |
| 17 | `../frontend/form-template.md` | 1/2 (bkz. §7.2-8) |
| 18 | `../ui-design/wireframe-template.md` | 1/5 (bkz. §7.2-9) |
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
   çekmek okuru şablonun anasayfasına değil genel bir kural dosyasına
   götürür — konu kayması.
3. `§67 gövde formatı` adının vault’ta karşılığı yok; şablon adı
   kullanır ama numaralandırmayı `[NOT SPECIFIED]` ile imler (G9).
4. Diskte `src/` ve `tests/` yok: “Uygulama” ve “Doğrulama”
   bölümlerindeki kod yolu iddiaları `⚠️ VERIFICATION REQUIRED` ile
   başlamalıdır (G7).
5. Yasaklı eski marka adı bu dosyada yalnızca kural metni olarak anılır;
   §5.3 madde 5 literal adı dosyada aramaz, birleşik literal kurar.
6. `../architecture/` içinde 4 kardeş var, whitelist 3 tanesiyle
   sınırlı: `dependency-rule-template.md` dışarıda — wiki kural metni
   üretmez (SRP, §1.4).
7. `../testing/` içinde 3 kardeş var, 2 linklenir:
   `integration-test-template.md` yalnız API dokümanının konusudur.
8. `../frontend/` 2, `../ui-design/` 5 kardeşe sahip; whitelist
   sırasıyla `form-template.md` ve `wireframe-template.md` ile
   sınırlı — kalanı wiki’nin değil ilgili şablonun sorumluluğu.
9. En olası hata: yer tutucu kalıbının §3.9 dışına taşması. §6.1 madde 15
   bunu yakalar; yakalamazsa rapor satırındaki sayaç kırmızı kalır.
10. En pahalı hata: kırık link. §5.3 madde 4 hiçbir koşulda “atla”
    denemez; hedef 0’dır.
11. Sayfa `deprecated` olduğunda silinmez: başlık altına uyarı satırı
    alınır, `active` statüsüne geri dönülmez (§3.6).

---

*Bu dosya bir şablondur; üretilen wiki sayfasında yer tutucu adları
doldurulur, şablon metni olduğu gibi kopyalanmaz.*