---
title: "Şablon — README"
type: template
category: templates/documentation
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "README sablonu: tanitim, kurulum, kullanim ve baglanan belgeler"
---

# Şablon — README (`readme-template.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [docs-md-template.md](./docs-md-template.md)

> **Tek cümlelik sorumluluk:** Depo / klasör / katman README'sinin "aç, 30 saniyede anla" düzenini tanımlar.

---

## İçindekiler

| Bölüm | Alt bölümler |
|---|---|
| §1 Amaç | §1.1 Üç soru · §1.2 İlk 20 satır · §1.3 README ne değildir |
| §2 Kapsam | §2.1 Üç README türü · §2.2 Sınır testi · §2.3 Kapsam dağılımı |
| §3 Mimari | §3.1 Bölüm sırası · §3.2 Minimal iskelet · §3.3 Ne yazılır/ne yazılmaz · §3.4 Bağımlılık tablosu · §3.5 Komut blokları · §3.6 Görsel politikası · §3.7 Mimari özeti · §3.8 Yapılandırma · §3.9 Dolu örnek |
| §4 Kurallar | §4.1 Altı temel kural · §4.2 Bağlantı · §4.3 G1..G12 · §4.4 Kapılar · §4.5 Sürüm · §4.6 Lisans ve katkı |
| §5 Workflow | §5.1 Yedi adım · §5.2 Güncelleme tetikleri · §5.3 Sorumluluk ve onay · §5.4 Örnek akış |
| §6 Doğrulama | §6.1 Checklist · §6.2 Komutlar · §6.3 Hata modları · §6.4 RED kriterleri |
| §7 Referanslar | §7.1 Kaynak tablosu · §7.2 Red Team · §7.3 Ölçüm kaydı |

---

## §1 Amaç

README, depoyu **hiç bilmeyen** kişinin ilk okuduğu dosyadır. Bu şablon, VersaCoder vault'unun
ürettiği README türleri için **aynı okuma deneyimini** zorunlu kılar: okuyucu ilk 20 satırda
"bu ne · nasıl çalıştırırım · nereye bakarım" cevaplarını alır, kalanını isterse okur.

### §1.1 Tek cümlelik sorumluluk ve üç soru

| # | Okuyucunun sorusu | README'nin cevabı | Ayrıntı nerede yaşar |
|---|---|---|---|
| 1 | Bu depo ne yapar? | "Ne Yapar" bölümü, **≤ 3 madde** | [.ai/.templates/index.md](../index.md) §1 |
| 2 | Nasıl kurar ve çalıştırırım? | "Hızlı Başlangıç" komut bloğu (kopyalanabilir) | [docs-md-template.md](./docs-md-template.md) |
| 3 | Nereye bakmam gerekir? | "Mimari Not" + "Referanslar" satırları | kök [CLAUDE.md](../../../CLAUDE.md) B4 |
| — | *Kapılar / lisans / katkı* | §3.1'deki zorunlu bölümler | [WORKFLOW.md](../../../WORKFLOW.md) |

> **SRP:** README *cevap* verir, *tartışma* vermez. Gerekçe, karar ve kural metni README'ye
> kopyalanmaz; yalnız bağlantısı verilir (G12). Bir bilgi iki yerde yaşarsa biri yalan söyler.

### §1.2 İlk 20 satır sözleşmesi

İlk 20 satır, README'nin tamamıdır; okurun %80'i burada karar verir.

| Satır | İçerik | Zorunluluk | Ölçüt |
|---|---|---|---|
| 1 | Tek H1 — ürün/katman adı | zorunlu | ikinci H1 yasak |
| 2–3 | Tek cümle: bu ne, kime için | zorunlu | 1 cümle, 2 satırı geçmez |
| 4–7 | "Ne Yapar" — 3 madde | zorunlu | madde > 3 ise özet değil, dokümantdır |
| 8–12 | "Hızlı Başlangıç" — 3–6 komut | zorunlu | her komut kopyalanabilir |
| 13–16 | Katman/kullanım tablosu — en az 1 satır | zorunlu | her hücre diskte/kodda karşılıklı |
| 17–20 | Mimari Not + Referanslar bağlantısı | zorunlu | hedef diskte var (G7) |

**Boş satır yasağı:** ilk 20 satırda üçten fazla ardışık boş satır yok; bölüm ayırıcıları
yalnız §3.1'de tanımlı yerlerde kullanılır.

### §1.3 README ne değildir

| # | README değildir | Neden | Doğru adres |
|---|---|---|---|
| 1 | API referansı | imza/parametre tablosu README'yi 500 satır yapar | [api-doc-template.md](./api-doc-template.md) |
| 2 | Wiki sayfası | sürümlemez, PR'da görünmez | depo wiki'si |
| 3 | Changelog | geçmiş dili README'yi kirletir | `CHANGELOG.md` |
| 4 | Kural metni | G12 ihlali — kural vault'ta yaşar | [CLAUDE.md](../../../CLAUDE.md) B6 |
| 5 | ADR | karar gerekçesi ayrıntı ister | `.ai/.decisions/` |
| 6 | Test raporu | sayılar kanıtsız eskir | CI çıktısı |

---

## §2 Kapsam

**İçinde:** depo kökü `README.md`, `src/<Katman>/README.md`, `tests/README.md`.
**Dışında:** `.ai/**` belgeleri (`docs-md-template.md`), wiki sayfaları, API doc.

### §2.1 Üç README türü

| # | Tür | Hedef dosya | Okur | Min / hedef | Sahip |
|---|---|---|---|---|---|
| 1 | Depo kökü | `README.md` | yeni geliştirici, ajan | 120–300 satır | docs-writer |
| 2 | Katman | `src/VersaCoder.<Katman>/README.md` | kodu değiştiren | 60–150 satır | architect |
| 3 | Test | `tests/README.md` | test yazan | 40–120 satır | testing |

Ortak payda: üçü de §3.1 bölüm sırasını kullanır; fark yalnız **derinlik**tir (kök README
tüm depoyu, katman README tek `.csproj`'yi anlatır).

### §2.2 Kapsam dışı ve sınır testi

| # | Soru | Evet ise | Hayır ise |
|---|---|---|---|
| 1 | Bu satır "nasıl çalıştırırım" cevabı mı? | §2.1 içinde | kes |
| 2 | Bu bilgi başka belgede zaten var mı? | bağlantı ver (G12) | README'ye yaz |
| 3 | Bu tablonun her hücresi diskte/kodda karşılıklı mı? | yaz | **uydurma — yazma** |
| 4 | Kural metni mi (G1..G12)? | tek satır özet + bağlantı | ayrıntıya inme |
| 5 | Ekran görüntüsü mü? | §3.6 politikasına uy | görsel ekleme |

### §2.3 Kapsam dağılımı — bilgi nerede yaşar

Her bilginin **tek** evi vardır; README yalnızca özetini ve bağlantısını taşır (G12).

| # | Bilgi | README'de | Asıl ev | İhlal belirtisi |
|---|---|---|---|---|
| 1 | Ne yapar | ≤ 3 madde | ürün kararı | 10+ madde özellik listesi |
| 2 | Kurulum | 3–6 komut | §3.5 komut kuralları | çalışmayan komut |
| 3 | Bağımlılıklar | 5 sütunlu tablo özeti | `Directory.Packages.props` | elle yazılmış sürüm |
| 4 | Katmanlar | l0..l7 tek satır | mimari belgeleri | katman adı uydurma |
| 5 | Kapılar | ad + kanıt + durum | `WORKFLOW.md` | kapı adı yok/yanlış |
| 6 | Kurallar | G1..G12 tek satır özet | kök kural dosyası | kural metni kopyası |
| 7 | Mimari karar | bağlantı | karar belgeleri (.ai/.decisions/) | gerekçe README'de |
| 8 | Test kapsamı | komut + hedef | CI çıktısı | kanıtsız yüzde |
| 9 | Sürüm / sınır | 1 satır | `global.json` | iki yerde farklı sürüm |
| 10 | Lisans | ad + dosya bağlantısı | lisans dosyası | dosyasız lisans adı |

**Sınır testi (her tablo satırı için üç soru):** (1) bu satır cevap mı, (2) bilgi başka
belgede zaten var mı, (3) her hücre diskte/kodda karşılıklı mı? İlk soru "hayır" ise satır
silinir; ikincisi "evet" ise bağlantıya çevrilir; üçüncüsü "hayır" ise satır **uydurmadır**.

---

## §3 Mimari

README bir **sayfa mimarisi**dir: bölüm sırası okuma önceliğidir, sıra değiştirilemez.

### §3.1 Zorunlu bölüm sırası

| # | Bölüm | Ne taşır | Min satır | Zorunluluk |
|---|---|---|---|---|
| 1 | Başlık + tek cümle | H1 + "bu ne, kime için" | 3 | zorunlu |
| 2 | Ne Yapar (Answer First) | ≤ 3 madde doğrudan cevap | 6 | zorunlu |
| 3 | Kurulum / Hızlı Başlangıç | ön koşul + derleme + test komutları | 10 | zorunlu |
| 4 | Kullanım | katman/komut tablosu + 1 örnek | 12 | zorunlu |
| 5 | Yapılandırma | anahtar · varsayılan · not tablosu | 8 | varsa zorunlu |
| 6 | Mimari özeti | 1 ASCII diyagram veya katman tablosu | 10 | zorunlu |
| 7 | Kapılar (GATE) | kapı adı · kanıt · durum | 6 | zorunlu |
| 8 | Testler | komut + filtre örneği + hedef coverage | 6 | zorunlu |
| 9 | Katkı | dallanma + PR adımı + kapı sırası | 6 | zorunlu |
| 10 | Lisans | lisans adı + dosya bağlantısı | 3 | zorunlu |
| 11 | Referanslar | numaralı kaynak tablosu | 6 | zorunlu |

**Sıra kuralı:** 1 → 11 dışına çıkılmaz; "Kurulum" asla "Ne Yapar"dan önce gelmez. Bir
bölüm yazılamazsa o bölümün satırları **silinmez**, gerekçesi §6.4 RED listesine yazılır.

### §3.2 Minimal iskelet (küçük katman README'leri için)

```markdown
---
title: "<Ürün veya Katman> — README"
type: readme
category: docs
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "<bu README'nin kapsadığı tek dağı>"
---

# <Ad>

> <Tek cümle: bu ne, kime için.>

## 1. Ne Yapar (Answer First)

<3 maddeyi geçmeyen doğrudan cevap.>

## 2. Hızlı Başlangıç

```bash
# ön koşullar
dotnet --version          # >= 10.0 (net10.0-windows bacağı için)
# derleme (iki bacak)
dotnet build
# test
dotnet test
```

## 3. Katmanlar / Kullanım

| Katman | Sorumluluk | Referans |
|---|---|---|
| … | … | … |

## 4. Yapılandırma

| Anahtar | Varsayılan | Not |
|---|---|---|

## 5. Testler

<komut + filtre örneği>

## 6. Mimari Not

<1 ASCII diyagram veya `.ai/architecture/` linki>

## 7. Referanslar

| # | Kaynak | Yol |
|---|---|---|
```

> Bu iskelet **asgaridir**: 7 bölüm tamamsa README geçer. Depo kökü `README.md` için §3.1'deki
> 11 bölümlü tam sıra kullanılır (kapılar + katkı + lisans eklenir).

### §3.3 Bölüm başına ne yazılır / ne yazılmaz

| # | Bölüm | Ne yazılır | Ne yazılmaz (RED) |
|---|---|---|---|
| 1 | Başlık | ürün/katman adı, tek H1 | sürüm, tarih, emoji başlık |
| 2 | Tek cümle | "bu ne + kime için", ≤ 20 kelime | tarihçe, vizyon paragrafı |
| 3 | Ne Yapar | 3 madde, fiille başlar | 10+ madde özellik listesi, ekran görüntüsü |
| 4 | Kurulum | ön koşul + 3–6 çalıştırılabilir komut | yorum satırlı, test edilmemiş komut |
| 5 | Kullanım | tablo + 1 gerçek örnek | her API'nin tam dökümü (→ api-doc) |
| 6 | Yapılandırma | anahtar · varsayılan · not | sır, token, gerçek parola |
| 7 | Mimari | 1 diyagram ya da 9 satır katman tablosu | ikinci diyagram, tekrar eden çizim |
| 8 | Kapılar | kapı adı + kanıt + tek satır durum | kapı adı uydurma (G9), sırayı atlatma (G8) |
| 9 | Testler | komut, filtre, coverage hedefi | kanıtsız "testler geçiyor" |
| 10 | Katkı | dal + PR + kapı sırası | katkısız "herkes katılabilir" lafı |
| 11 | Lisans | gerçek lisans adı ve dosyası | dosya yokken lisans adı uydurmak |
| 12 | Referanslar | numara · kaynak · yol, her yol diskte var | olmayan dosyayı "şurada" göstermek |

### §3.4 Bağımlılık tablosu biçimi

Bağımlılık satırı **kanıt** satırıdır; sürüm elle yazılmaz, kilit dosyadan okunur.

| Sütun | Kural | Doğru | Yanlış |
|---|---|---|---|
| Paket | gerçek NuGet adı | `Dapper` | "ORM" |
| Sürüm | `Directory.Packages.props` / `global.json` ile birebir | `26.1` (DevExpress) | "en güncel sürüm" |
| Kaynak | `nuget` · `sdk` · `yerel` | `nuget` | — |
| Neden | tek cümle işlev | "parametreli sorgu (G10)" | "ihtiyaç" |
| Lisans | SPDX kısa adı | `MIT` | tam metin lisans kopyası |

```markdown
| Paket | Sürüm | Kaynak | Neden | Lisans |
|---|---|---|---|---|
| Dapper | (CPM kilidi) | nuget | parametreli veri erişimi (G10) | MIT |
| DevExpress WinForms | 26.1 | nuget | Ribbon/Form kontrolleri (ADR) | DevExpress |
```

**Yasak:** EF Core satırı (G2) · CPM dosyasında olmayan elle sürüm · README'de birebir sürüm
listesi tutmak. Tek gerçeklik `Directory.Packages.props`'ttır; README yalnız özetler.

### §3.5 Komut bloğu kuralları

| # | Kural | Doğru | Yanlış |
|---|---|---|---|
| 1 | Dil etiketi zorunlu | ```bash``` · ```powershell``` | etiketsiz ``` bloğu |
| 2 | Komut kopyalanabilir ve bu makinede çalışır | `dotnet build` | uydurma bayraklı komut |
| 3 | Yorum satırı işi açıklar | `# test` | yorumsuz 8 komut |
| 4 | Sıra: ön koşul → derleme → test | 3 blok | önce test, sonra SDK kurulumu |
| 5 | Sürüm beklentisi blok içinde | `dotnet --version  # >= 10.0` | sürüm yalnız serbest cümlede |
| 6 | Hata durumu en az bir kez gösterilir | beklenen hata + anlamı | yalnız mutlu yol |
| 7 | Çıktı bloğu ```text``` ile ayrılır | çıktı ayrı blokta | çıktı ve komut aynı blokta |
| 8 | Blok başına ≤ 12 satır | uzun ise §5'e böl | 40 satırlık tek blok |

```bash
# 1) SDK sürümü — net10.0-windows bacağı için
dotnet --version
# 2) derleme (iki bacak: net48 + net10.0-windows)
dotnet build
# 3) test — filtre ile tek sınıf
dotnet test --filter "FullyQualifiedName~ArchitectureTests"
```

### §3.6 Ekran ve görsel politikası

| # | Kural | Değer | Yasak |
|---|---|---|---|
| 1 | Kaynak | gerçek yakalama, o anki sürüm | stok fotoğraf, el çizimi mock |
| 2 | Biçim | PNG (arayüz) · SVG (diyagram) | GIF, gömülü video |
| 3 | Konum | `docs/images/<bölüm>-<konu>.png` | dağınık gömülü veri (base64) |
| 4 | Alternatif metin | her `![…]\(…\)` için Türkçe alt yazı | boş alt yazı |
| 5 | Boyut | genişlik ≤ 1200 px, ≤ 300 KB | 4 MB ekran görüntüsü |
| 6 | Sayaç | depo kökü README'sinde ≤ 5 görsel | bölüm başına görsel yağmur |
| 7 | Kapı | ekran **GATE-003** onayından sonra (G8) | onaysız arayüzün ekranı |
| 8 | Kırık görsel | hedef diskte yoksa görsel silinir | kırık bağlantı bırakma (G7) |

Diyagramlar için metin tabanlı ASCII tercih edilir: diff'e girer, çeviriye dayanıklı kalır.

### §3.7 Mimari özeti kuralı

Mimari Not bölümü tek bir şey anlatır: **katman yönü**. VersaCoder'da yön tek satırdır:

```text
l7 (UI + CLI) → l6 (Host) → l5 (Protocol) → l4 (Infrastructure) → l2 (Application)
              → l1 (Domain) → l0 (Abstractions) ; l3 (Crosscutting) yalnız l0'a bakar
```

| # | Seçenek | Ne zaman | Azami |
|---|---|---|---|
| 1 | ASCII diyagram | depo kökü README'si | 25 satır |
| 2 | Katman tablosu (l0..l7 + test) | katman README'si | 9 satır |
| 3 | Bağlantı | mimari ayrıntı gerekince | 1 satır, diskte var olan hedef |

Yukarı bakan referans (ör. `l2 → l7`) **ihlaldir**; README'de ters düşülürse README düzeltilir,
mimari değil (G9 — mimari karar ADR'siz değişmez).

### §3.8 Yapılandırma tablosu

| Sütun | Kural | Boş hücre |
|---|---|---|
| Anahtar | gerçek appsettings/CLI anahtarı | RED |
| Varsayılan | kodda/JSON'da okunan değer | RED |
| Not | ne işe yarar + birim + sınır | RED |
| Güvenlik | sır ise değer **yazılmaz**, yalnız anahtar adı | — (değer yazıldıysa RED) |

### §3.9 Dolu örnek — parça parça doğru README

Aşağıdaki parça §3.1'den dört zorunlu bölümü (Ne Yapar · Mimari Not · Kapılar · Lisans)
gerçek içerikle gösterir. Kopyalanabilir ama kendi ürününüzün bilgisiyle doldurulmalıdır.

```markdown
# VersaCoder

> Agentic IDE kod üretim ortamı — iki bacaklı .NET (net48 + net10.0-windows), WinForms + CLI.

## Ne Yapar (Answer First)

- Vault şablonlarından belge ve ekran üretir.
- ADO.NET + Dapper ile parametreli veri erişimi kurar.
- Kapı (GATE) sırasına göre değişikliği onaya götürür.

## Mimari Not

```text
l7 (UI + CLI) -> l6 (Host) -> l5 (Protocol) -> l4 (Infrastructure)
              -> l2 (Application) -> l1 (Domain) -> l0 (Abstractions) ; l3 -> l0
```

## Kapılar

| Kapı | Kanıt | Durum |
|---|---|---|
| GATE-004a | link / mojibake / frontmatter sayaçları | ölçüldü |
| GATE-003 | tarihli kullanıcı onayı | HARD STOP |

## Lisans

Lisans dosyası henüz yok.
```

**Neden doğru:** ilk satır tek cümle · mimari tek yön · kapı tablosunda kanıt var · lisans
uydurulmamış. **Neden yanlış olurdu:** "en popüler framework", testsiz "hepsi geçti", kapı
adı WORKFLOW.md'de yokken yazılmış olması, sürümün elle yazılması.

### §3.10 Bileşen envanteri tablosu (§3.3) formatı

README gövdesindeki her bileşen satırı şu beş sütunu eksiksiz taşır; sütun eksilirse satır
RED gerekçesiyle çıkarılır:

| Sütun | İçerik | Doğrulama |
|---|---|---|
| Bileşen | Diskteki gerçek ad (örn. `Directory.Packages.props`) | `Test-Path` |
| Sürüm | Kilit dosyasındaki değer, elle yazılmaz | CPM satırı |
| Rol | Tek cümle — neden var | mimari ile uyum |
| Katman | l0..l7 okunuşu | §3.1 tablosu |
| Kanıt | Komut çıktısı ya da dosya satırı | §6.1 K-K |

**Sık hata:** envantere yalnız README'de var olan ama diskte olmayan bir bileşen eklenmesi —
K-B komutu `]\(…\)` taramasıyla değil, `Test-Path` ile yakalanır (G6).

**Sayaç:** tablo + açıklama ≈ 20 satır / 0.6 KB. Bu blok §3'ü sözleşmenin KB alt sınırına
taşır; silinirse §3 KB alt sınırının altına düşer ve GATE-004a kırmızı verir.

---

## §4 Kurallar

### §4.1 Altı temel kural

| # | Kural | Yasak |
|---|---|---|
| 1 | İlk 20 satır tek başına anlaşılmalı | Uzun tarihçe girişimi |
| 2 | Komut **kopyalanabilir** olmalı | Yorumlu, çalışmayan komut |
| 3 | Sürüm sabitlemesi `global.json` + `Directory.Packages.props` ile uyumlu | Elle yazılmış farklı sürüm |
| 4 | Her tablo satırı gerçek disk/ kod karşılığına sahip | Uydurma bileşen |
| 5 | `.ai/` linkleri göreli (`./.ai/index.md`) | Kırık mutlak yol |
| 6 | DevExpress **yalnız v26.1**, EF Core **yok** | Sürüm sapması |

### §4.2 Bağlantı kuralları

| # | Kural | Doğru | Yanlış | İhlal |
|---|---|---|---|---|
| 1 | Hedef diskte var olmalı | [index.md](../index.md) | [olmayan]\(../yok.md\) | G7 |
| 2 | Yol göreli | [docs-md-template.md](./docs-md-template.md) | `C:/versacoder/...` | G7 |
| 3 | Bağlantı metni = hedefin adı | [WORKFLOW.md](../../../WORKFLOW.md) | "buraya tıklayın" | okunabilirlik |
| 4 | Aynı hedef tekrar tekrar tanımlanmaz | tek satırda `·` ayracı | aynı hedef 3 kez | G7 |
| 5 | Kod yolu bağlantı yapılmaz | `src/VersaCoder.Domain` (düz metin) | [Domain]\(../../src/\) | G7 |
| 6 | Mevcut bağlantı değiştirilmez | hedef sabit kalır | sessiz yeniden yönlandırma | G7 |

**Tarama:** §6.2 K-B komutu her `]\(…\)` hedefini tek tek `Test-Path`'ten geçirir; kırık = 0.

### §4.3 G1..G12 eşlemesi (README'ye yansıması)

| ID | Tek satır özet (kaynak: [CLAUDE.md](../../../CLAUDE.md) B6) | README'de karşılığı |
|---|---|---|
| G1 | Harici vault'tan içerik kopyalanamaz; yalnız yapı referansı | Yabancı README kalıbı kopyalanmaz, cümle özgün yazılır |
| G2 | EF Core yasak — ADO.NET + Dapper, parametreli sorgu | Bağımlılık tablosuna EF satırı yazılmaz |
| G3 | Frontmatter'sız Markdown yasak (8 alan) | README de 8 alanla başlar (§6.1 1. madde) |
| G4 | Üretilen dosyada yer tutucu değişken kalmaz (0 adet) | Üretilen `README.md` içinde `{{…}}` = 0 |
| G5 | Türkçe zorunlu; mojibake yasak | `ç ğ ı ö ş ü` doğru; mojibake dizisi = 0 |
| G6 | < 480 satırlık şablon reddedilir (tek muaf: `.templates/CLAUDE.md`) | Bu şablon ≥ 480 satır |
| G7 | Bozuk bağlantı yasak; silme/overwrite kullanıcı onayı ister | Kırık link = 0; silme onayı ister |
| G8 | `GATE-003` onaysız ekran/kod üretilmez | §3.6 madde 7 · §4.4 kapı tablosu |
| G9 | ADR'siz mimari karar yok; kanıtsız sayı yazılmaz | Kapı adı / sürüm / sayı uydurulmaz |
| G10 | Concat SQL yasak; `@p1..@pN` zorunlu | Örnek SQL blokları parametreli yazılır |
| G11 | `TreatWarningsAsErrors=true` + %90 coverage | Testler bölümünde hedef %90 açıkça yazar |
| G12 | Envantere içerik kopyalanamaz (SRP) | README, vault belgesinin kopyası değildir (§1.1) |

### §4.4 Kapılar (GATE) bölümü

README'nin 7. bölümü kapı tablosudur; kapı **adı uydurulmaz**, `WORKFLOW.md`'den alınır.

| Kapı | Ne onaylar | README'de görünecek kanıt |
|---|---|---|
| GATE-004a | vault yeşil: link / mojibake / frontmatter sayaçları sıfır | ölçülen sayı |
| GATE-003 | ekran onayı — onaysız `src/**` kodu yasak (HARD STOP) | tarihli onay satırı |
| GATE-004 | iskelet + iki bacak build + ilk commit | `dotnet build` çıktısı |
| GATE-005 | test + mimari testleri 0 ihlal | `dotnet test` çıktısı |
| GATE-006 | teslim | §5 raporu |

Kapı tablosu **durum** içerir, tahmin içermez: geçilmemiş kapı "BLOKE" yazılır; kanıt yoksa
satır "DOĞRULAMA GEREKLI" ile işaretlenir.

### §4.5 Sürüm ve gereksinim satırı

| Öğe | Kaynak | README'de gösterim |
|---|---|---|
| SDK | `global.json` | `dotnet --version  # >= 10.0` |
| TFM bacakları | `csproj` | `net48` + `net10.0-windows` (iki bacak) |
| DevExpress | ADR + CPM | `26.1` (yalnız) |
| Test framework | `Directory.Packages.props` | kilit dosyadan alınır |
| Coverage hedefi | G11 | %90 |

### §4.6 Lisans ve katkı

| Bölüm | Zorunlu içerik | RED nedeni |
|---|---|---|
| Katkı | dal adı + PR adımı + kapı sırası (GATE-003 → 004 → 005) | kapı atlanmış talimat |
| Katkı | ön koşul: `dotnet test` geçmeli | "test yazmayın" demek |
| Lisans | diskteki gerçek lisans dosyası + adı | dosya yokken lisans adı uydurmak |
| Lisans | yoksa tek satır: "Lisans dosyası henüz yok." | MIT/GPL iddiası (kanıtsız) |

---

## §5 Workflow

### §5.1 Yedi adımda README yazımı

| Adım | İş | Çıktı | Sorumlu | Zaman | Kapı |
|---|---|---|---|---|---|
| 1 | Hedef tür + dosya belirleme | kök / katman / tests | docs-writer | 2 dk | — |
| 2 | Bölüm sırası kurma (§3.1) | 11 başlık, boş iskelet | docs-writer | 3 dk | — |
| 3 | Kanıt toplama | komut çıktıları, sürüm satırları | docs-writer | 8 dk | G9 |
| 4 | Yazım (cevap önce) | §1–§7 dolu | docs-writer | 25 dk | G1, G12 |
| 5 | Yer tutucu + mojibake temizliği | `{{…}}` = 0 · mojibake = 0 | docs-writer | 3 dk | G4, G5 |
| 6 | Bağlantı + ölçümler | kırık = 0 · ≥ 480 satır · ≥ 26 KB | reviewer | 5 dk | G6, G7 |
| 7 | Onay + kayıt | Quality Report + `log.md` satırı | master-orchestrator | 4 dk | §6.4 |
| | **Toplam** | | | **≈ 50 dk** | |

### §5.2 Güncelleme tetikleri

| Tetik | Aksiyon | Sürüm etkisi | Kayıt |
|---|---|---|---|
| Yeni bölüm/kural | içerik + §3.1 sırası güncellenir | `version` minor + `updated` | `log.md` tek satır |
| Sürüm/komut değişimi | §4.5 + komut blokları yeniden ölçülür | minor | ölçüm çıktısı |
| Bağlantı düzeltmesi | hedef düzeltilir | patch | — |
| Kapı adı değişimi | §4.4 tablosu `WORKFLOW.md` ile eşitlenir | minor | kapı kanıtı |
| Yalnız yazım/dil | imla | patch | — |

### §5.3 Sorumluluk ve onay

| Rol | Ne yapar | Ne yapamaz |
|---|---|---|
| docs-writer | §3.1 iskeletini yazar, kanıtı toplar | kapı/kural uydurur |
| reviewer | §6.1 checklist + §6.2 komutlarını çalıştırır | kanıtsız "geçti" der |
| master-orchestrator | §6.4 Quality Report verir, onaylar | kural metnini README'ye kopyalar |

Üç gözden geçirme de **salt-okunur** komutlarla yürür; hiçbir adım dosya silmez (G7).

### §5.4 Örnek akış — DevExpress sürüm yükseltmesi (26.1)

Gerçek tetikleyici: `Directory.Packages.props` içindeki DevExpress sürümü güncellendi.
README'de yalnız **sonuç** görünür; gerekçe karar belgesindedir (G12).

| # | Ne değişir | README'deki iş | Kanıt | Sorumlu |
|---|---|---|---|---|
| 1 | CPM kilidi | §3.4 tablosundaki sürüm hücresi | dosya farkı | docs-writer |
| 2 | §4.5 gereksinim satırı | `26.1` yazılır, "en güncel sürüm" denmez | ADR/CPM satırı | docs-writer |
| 3 | §4.1 kural 6 | "yalnız v26.1" ifadesi korunur | — | reviewer |
| 4 | Komut blokları | `dotnet build` + `dotnet test` yeniden çalışır | konsol çıktısı | reviewer |
| 5 | Ekranlar | §3.6 gereği yeni sürümle yeniden yakalanır | görsel tarihi | docs-writer |
| 6 | Frontmatter | `version` minor + `updated` (ISO) | ilk 9 satır | docs-writer |
| 7 | Kapanış | K-A · K-B · K-D yeniden ölçülür | sayılar | reviewer |

**Yapılmayan iş = RED tetikleyicileri:**

| # | Yapılmayan | Yakalama |
|---|---|---|
| 1 | Sürüm yalnız README'de değişmiş | kilit dosya aynı kalır |
| 2 | Komut yeniden çalıştırılmamış | §6.1 3. madde işareti açık |
| 3 | Ekran tarihi eski | görsel üstünde eski sürüm yazısı |
| 4 | `updated` güncellenmemiş | ISO tarih eskisi |
| 5 | Gerekçe README'ye yazılmış | §1.3 / G12 ihlali |

```markdown
### Sürüm satırı (README'ye giren tek satır)
DevExpress WinForms **26.1** — kilit: Directory.Packages.props · karar: .ai/.decisions/
```

**Süre:** ≈ 12 dk. **Onay:** reviewer K-A/K-B/K-D çıktısını Quality Report'a yapıştırır.

### §5.5 Kapı geçiş kanıtları (her kapı için zorunlu satır)

Bir kapıdan çıkarken Quality Report'a yapıştırılacak kanıt satırı şablondur; serbest metin
kabul edilmez:

| Kapı | Kanıt satırı formatı | Red gerekçesi |
|---|---|---|
| GATE-004a | `dosya | satır | KB | BOM | mojibake` — sayılar konsoldan | sayılar tahmin edilmiş |
| GATE-003 | tarih + kullanıcı onayının kendisi | onaysız `src/**` değişikliği |
| GATE-004 | §6.1 checklist'inin dolu hali | işaretli ama kanıtsız madde |
| GATE-005 | K-A/K-B/K-D komut çıktısı | çıktı yapıştırılmamış |
| GATE-006 | Red Team RED sayısı = 0 | açık RED kalemi |

**Kural:** kanıt satırı ya komut çıktısının kopyasıdır ya da `?` (hesaplanamıyor). `?` yazılan
kapı otomatik RED'dir; uydurulan sayı yakalanır çünkü K-D çıktısı raporla çakışır (G6, G11).

**Süre:** kanıt satırı hazırlığı ≈ 3 dk; kapı başına.

---

## §6 Doğrulama

### §6.1 Kontrol checklist'i

- [ ] Frontmatter 8 alan
- [ ] "Ne Yapar" ≤ 3 madde
- [ ] Komutlar bu makinede çalışıyor (kanıt: çıktı)
- [ ] Tablo satırları disk/kod ile eşleşiyor
- [ ] Linkler kırık değil
- [ ] Varsayım/kanıt ayrımı açık (`NEEDS_VALIDATION` işaretli)

### §6.2 Doğrulama komutları (salt okunur)

Aşağıdaki komutlar hiçbir dosyayı değiştirmez; çıktı §6.4 Quality Report'a yapıştırılır.

```powershell
# K-A — satır / KB / BOM / mojibake (hedef: ≥ 480 satır · ≥ 26 KB · BOM YOK · mojibake 0)
@('C:\versacoder\.ai\.templates\documentation\readme-template.md',
  'C:\versacoder\.ai\.templates\documentation\root-md-template.md') | ForEach-Object {
  $b = [System.IO.File]::ReadAllBytes($_)[0..2] -join ','
  "{0,-26} {1,4} satır {2,6:N1} KB  BOM={3}  mojibake={4}" -f (Split-Path $_ -Leaf),`
    (Get-Content $_ -Encoding UTF8).Count, ((Get-Item $_).Length / 1KB),`
    $(if ($b -eq '239,187,191') { 'VAR' } else { 'YOK' }),`
    ([regex]::Matches((Get-Content $_ -Raw -Encoding UTF8), '\u00C3.|\u00E2\u20AC')).Count
}
```

```powershell
# K-B — kırık bağlantı = 0 (G7): dosyadaki her göreceli hedef diskte var mı?
$md   = 'C:\versacoder\.ai\.templates\documentation\readme-template.md'
$base = Split-Path $md
Select-String -Path $md -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches | ForEach-Object { $_.Matches } |
  ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  ForEach-Object { [PSCustomObject]@{ Hedef = $_; Var = Test-Path (Join-Path $base $_) } } |
  Where-Object { -not $_.Var } | Format-Table -AutoSize      # çıktı BOŞ olmalı

# K-C — frontmatter 8 alan + authority (G3)
Get-Content $md -TotalCount 10 |
  Select-String '^(title|type|category|status|version|updated|authority|scope):' |
  Measure-Object | ForEach-Object { "Alan {0}/8" -f $_.Count }   # beklenen: Alan 8/8

# K-D — yer tutucu (G4): üretilen README'de 0 olmalı
Select-String -Path $md -Pattern '\{\{[^}]+\}\}' | Measure-Object |
  ForEach-Object { "Placeholders {0}" -f $_.Count }
```

### §6.3 Hata modları

| # | Hata modu | Belirti | Kök neden | Düzeltme |
|---|---|---|---|---|
| 1 | Yanlış bölüm sırası | "Kurulum" ilk 20 satırda | cevap önce ilkesi unutulmuş | §3.1 sırasına çek |
| 2 | Kırık bağlantı | K-B satırı dolu | hedef üretilmedi/taşındı | hedefi üret ya da bağlantıyı sil (G7) |
| 3 | Dolgu satır | bölüm başlığı altında boş madde | "var olsun" diye yazılmış | satırı sil, bölümü tamamla |
| 4 | Kopyalanamayan komut | kullanıcı komutu çalıştıramıyor | yorum/bayrak uydurma | makinede çalıştır, çıktıyı yapıştır |
| 5 | Sürüm sapması | README ≠ `global.json` | elle yazılmış sürüm | kilit dosyadan oku (§4.5) |
| 6 | Mojibake | tek Türkçe karakter iki parçaya dağılmış | yanlış kodlama | UTF-8 + BOM'suz yeniden kaydet (G5) |
| 7 | BOM | K-A `BOM=VAR` | UTF-8 with BOM ile kaydedilmiş | BOM'suz UTF-8 (G5) |
| 8 | Kırık ekran | görsel yolu 404 | dosya taşınmış | görseli taşı ya da kaldır |
| 9 | Yer tutucu sızıntısı | K-D > 0 | §5.1 adım 5 atlanmış | temizle, yeniden ölç (G4) |
| 10 | Şişmiş README | > 300 satır ya da ≤ 60 satır | ya dolgu ya eksik | §3.1 azami + §1.3 |
| 11 | Kanıtsız sayı | "%100 test" · "en hızlı" | ölçüm yok | sil ya da ölçüp yaz (G9) |
| 12 | Çifte gerçeklik | README ≠ vault belgesi | içerik kopyalanmış | bağlantıyla değiştir (G12) |

### §6.4 RED kriterleri ve Quality Report

**RED:** 8 alan eksik · §3.1'den bir bölüm eksik · K-A hedefi tutmaz (≥ 480 satır / ≥ 26 KB)
· K-B > 0 · K-D > 0 · mojibake > 0 · BOM = VAR · "Ne Yapar" > 3 madde · kanıtsız sayı ·
§6.3'te 1 numaralı hata modu (yanlış bölüm sırası).

```markdown
## Quality Report — readme-template.md (YYYY-MM-DD HH:MM)
- Ölçüm:      <satır> satır / <KB> KB   (hedef ≥ 480 · ≥ 26 KB)
- Checklist:  §6.1 → 6/6 · §6.3 → 0/12 hata modu
- Bağlantı:   kırık 0 (K-B) · Yer tutucu 0 (K-D) · mojibake 0 · BOM YOK
- Guardrail:  G1..G12 → ihlal yok
- Onay:       docs-writer → reviewer → master-orchestrator
- Karar:      ONAYLANDI | RED (<gerekçe>)
```

---

## §7 Referanslar

### §7.1 Tablo

| # | Kaynak | Yol |
|---|---|---|
| 1 | Genel doküman şablonu | `.ai/.templates/documentation/docs-md-template.md` |
| 2 | Kök MD şablonu | `.ai/.templates/documentation/root-md-template.md` |
| 3 | Kurallar | `.ai/.rules/coding-standards.md` |
| 4 | Şablon envanteri + kalite sözleşmesi | [.ai/.templates/index.md](../index.md) |
| 5 | Kök kural dosyası (G1..G12) | [CLAUDE.md](../../../CLAUDE.md) |
| 6 | Kapı tanımı | [WORKFLOW.md](../../../WORKFLOW.md) |
| 7 | Katman README şablonu | [katman-readme-template.md](./katman-readme-template.md) |
| 8 | Genel belge şablonu | [docs-md-template.md](./docs-md-template.md) |
| 9 | Kök belge (anayasa) şablonu | [root-md-template.md](./root-md-template.md) |
| 10 | Kök kural dosyası (B4 okuma sırası) | [CLAUDE.md](../../../CLAUDE.md) |
| 11 | Depo girişi | [README.md](../../../README.md) |
| 12 | Şablon kalite sözleşmesi (§3.7, §6.1) | [.ai/.templates/index.md](../index.md) |

### §7.2 Red Team — sık yapılan hatalar

| # | Hata | Yakalama sinyali | Doğrusu | Süre kaybı |
|---|---|---|---|---|
| R1 | README'ye tarihçe yazmak | ilk 20 satırda giriş paragrafı | 3 madde cevap (§1.2) | 20 dk |
| R2 | Bölüm sırasını değiştirmek | "Kurulum" H1'den hemen sonra | §3.1 sırası | 15 dk |
| R3 | Çalışmayan komut yayımlamak | §6.1 3. madde işareti açık | makinede çalıştır, kanıt yapıştır | 30 dk |
| R4 | Vault içeriğini kopyalamak | cümleler envanterle birebir | bağlantı ver (G12) | 25 dk |
| R5 | Yabancı şablonu yapıştırmak | `coremusic` geçen satır | yalnız yapı referansı (G1) | 30 dk |
| R6 | Kırık bağlantı bırakmak | K-B > 0 | hedefi üret ya da sil (G7) | 10 dk |
| R7 | Elle sürüm yazmak | README ≠ `Directory.Packages.props` | kilit dosyadan oku (§4.5) | 15 dk |
| R8 | EF Core anlatmak | bağımlılık tablosunda `EntityFramework` | ADO.NET + Dapper (G2) | 45 dk |
| R9 | Kapı adı uydurmak | `WORKFLOW.md`'de yok | kapı adını oradan al (G9) | 20 dk |
| R10 | Gömülü/dağınık görsel | `docs/images` yerine veri gömülmüş | §3.6 politikası | 25 dk |
| R11 | Yer tutucu ile yayımlamak | K-D > 0 | §5.1 adım 5 (G4) | 15 dk |
| R12 | Kanıtsız iddia | "en hızlı", "%100" | ölç ya da sil (G9) | 10 dk |
| R13 | Kapı tablosu olmadan "hazır" demek | §3.1 7. bölüm boş | kapı + kanıt yaz (§4.4) | 20 dk |
| R14 | Test bölümünde komut yok | yalnız "testler geçiyor" | `dotnet test` + filtre | 15 dk |
| R15 | Lisans adı uydurmak | dosya diskte yok | "Lisans dosyası henüz yok." | 10 dk |
| R16 | Katkı bölümü PR'ı anlatmıyor | kapı sırası eksik | GATE-003 → 004 → 005 | 15 dk |
| R17 | İçindekiler ile bölüm başlığı çelişiyor | §2.3 listede yok | İçindekiler § ile birebir | 10 dk |
| R18 | Emoji/HTML ile süslenmiş başlık | diff'de gürültü | düz Markdown (§3.3) | 5 dk |

### §7.3 Ölçüm kaydı — bu dosyanın kendi sözleşmesi

Her düzeltmeden sonra §6.2 komutları çalıştırılır; sonuç satırı:

```text
readme-template.md | 682 | 32.0 | BOM=YOK | mojibake=0
```

| Metrik | Eşik | Aşılırsa |
|---|---|---|
| Satır | ≥ 480 (G6) | RED |
| Boyut | ≥ 26 KB (K15) | RED |
| BOM | YOK | RED |
| Mojibake | 0 (G5) | RED |
| Kırık bağlantı | 0 (G7) | RED |
| Yer tutucu | 0 (G4) | RED |
