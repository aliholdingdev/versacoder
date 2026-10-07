---
title: "ADR-001 — Dual Target: net48 + net10.0-windows"
type: adr
category: adr
status: accepted
adr_id: ADR-001
date: 2026-09-25
updated: 2026-09-25
version: 1.0.0
authority: SSOT
tags: [tfm, net48, net10.0-windows, dual-target, build, architecture, conditional-compilation]
---

# ADR-001 — Dual Target: `net48` + `net10.0-windows`

**Zorunlu Bağlantılar:** [ADR Defteri](../adr-index.md) · [Vault Ana Sayfa](../../index.md) · [Anayasa](../../CLAUDE.md) · [Anahtar Kelime Haritası](../../keys.md) · [Kapsam Analizi](../../reports/00-kapsam-analizi.md) · [ADR-002](ADR-002-handwritten-content-no-copypaste.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)

> **Tek cümlelik sorumluluk:** *Hangi framework'lerde derlenip test edileceğiz, `#if` politikası nedir, net48+DevExpress riski nasıl azaltılır* sorusunun tek cevabıdır.

---

## §1 Kapsam ve Kullanım

### §1.1 — Ne zaman kullanılır

1. Yeni bir `.csproj` oluşturulurken (`TargetFramework(s)` satırı yazılacakken).
2. Bir API, NuGet paketi veya BCL tipi seçilecekken ("bu net48'te var mı?").
3. `#if` direktifi yazılma/inceleme anında (yalnız platform farkı mı, iş mantığı mı?).
4. Test projesi eklenirken ("hangi bacakta koşuyor?").
5. DevExpress bağımlılığı eklenirken ([ADR-004](ADR-004-devexpress-universal-v261-office2010.md) §1.1).
6. `Directory.Build.props` / solution dosyası düzenlenirken.
7. "net48'i düşürelim mi?" gibi bir teklif geldiğinde — bu ADR **yenisi yazılmadan değiştirilemez**.

### §1.2 — Karar kapsamındaki TFM envanteri

| # | Proje (planlanan) | TFM | Not |
|---|---|---|---|
| 1 | `VersaCoder.Desktop` | `net48` + `net10.0-windows` | WinForms + DevExpress ([ADR-004](ADR-004-devexpress-universal-v261-office2010.md)) |
| 2 | `VersaCoder.Cli` | `net48` + `net10.0-windows` | konsol; DevExpress **içermez** |
| 3 | `VersaCoder.Application` / `Domain` / `Infrastructure` / `Protocol` | `net48` + `net10.0-windows` | ortak kaynak katmanları |
| 4 | Test projeleri (xUnit) | **her iki bacak** ayrı ayrı koşar | §6.2 test matrisi |
| 5 | `src/`, `.csproj`, `global.json` | ⚠️ VERIFICATION REQUIRED | diskte henüz yok (PART 5) |

> **UYARI:** Bu tablo **plan** tablosudur; disk kanıtı `dotnet build` çıktısıdır (G5). Kanıt gelene kadar satırlar `⚠️ VERIFICATION REQUIRED` taşır.

### §1.3 — Kapsam ve kapsam dışı

| Kapsam | Kapsam dışı |
|---|---|
| Tüm `.csproj`'lerin `TargetFramework(s)` değeri | Hangi **paket**lerin kurulacağı (CPM — `Directory.Packages.props`) |
| Ortak kaynak dosyalarının API yüzeyi sınırı | Veri erişimi seçimi ([ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)) |
| `#if NET48` / `#if NET10_0_WINDOWS` politikası | DevExpress **sürümü** ([ADR-004](ADR-004-devexpress-universal-v261-office2010.md)) |
| Her iki bacakta **build + test** zorunluluğu | Deployment/publish profilleri (PART 5) |
| net48 + DevExpress v26.1 riskinin spike planı | Linux/macOS hedefi (Windows-only ürün) |

### §1.4 — Neden "iki bacak" tek başına bir ADR'dir

```
          VersaCoder kod tabanı (tek kaynak)
                     |
     +---------------+---------------+
     |                               |
 net48 bacaği                  net10.0-windows bacaği
 legacy kanit                  modern platform
 eski API'ler                  yeni API'ler
 DevExpress v26.1              DevExpress v26.1
     |                               |
     +---------> AYNI TEST <---------+
                 (her iki bacak)
```

Tek bacak seçilirse ikinci bacak "sonra bakarız" olur ve G6 (tek bacaklı tasarım yasağı) ihlal edilir. Bu ADR, iki bacağı **eşit vatandaş** ilan eder: ikisi de derlenir, ikisi de test edilir.

---

## §2 Bağlam (Context)

### §2.1 — Karar ortamı

| Boyut | VersaCoder gerçeği | Kaynak |
|---|---|---|
| Hedef TFM | `net48` + `net10.0-windows` | `CLAUDE.md` §4 Teknoloji Anayasası |
| Karar kaynağı | ai.03.md §"TARGET FRAMEWORKS" (birebir) | `reports/00-kapsam-analizi.md` §2/1 |
| Reddedilen kaynak | ai.01.md `.NET 8` (eski yığın) | aynı rapor: ai.03 ai.01'i geçersiz kılmış |
| Legacy kanıt | net48 bacağı: eski Windows/.NET Framework ortamlarında çalışabilirlik | karar #1 gerekçesi |
| Modern kanıt | net10.0-windows: güncel runtime, dil ve BCL yenilikleri | karar #1 gerekçesi |
| UI | WinForms + DevExpress v26.1 (her iki bacakta) | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) |
| Bağlayıcı kural | **G6** — her karar iki bacakta değerlendirilir; `#if` yalnız platform farklarında | `CLAUDE.md` §1/G6 |
| Belirsizlik | `.sln` vs `.slnx` (B1) · test framework (B2, xUnit lehine döndü) | rapor §3.2 |

### §2.2 — Kararı tetikleyen tetikleyiciler

1. **ai.01 ↔ ai.03 çelişkisi:** eski plan `.NET 8` diyor, yeni bootstrap `net48 + net10.0-windows` diyor — biri seçilmeliydi; rapor ai.03'ü esas aldı (G10: çelişki çözüldü, kaynak hiyerarşisi uygulandı).
2. **Legacy kanıt şartı:** ürünün yalnız modern runtime'da değil, eski kurumsal makinede de açılması bekleniyor; tek `net10.0-windows` bunu kanıtlayamaz.
3. ** DevExpress bağımlılığı:** UI kütüphanesi her iki bacakta da çalışmak zorunda — bu, seçimin maliyetini artıran ana etken (R5).
4. **EF Core'un elenmesi:** net48 bacağı, EF Core tercihini zaten [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) ile kapattı; veri katmanı da iki bacakta aynı davranmalı.
5. **Test kanıtı:** "derleniyor" tek başına yeter değil; her iki bacakta test koşmadan G5 (kanıtsız tamamlama) ihlal edilir.

### §2.3 — Masadaki seçenekler (≥2 alternatif)

Masada en az dört seçenek vardı: **(A)** `net48` + `net10.0-windows` ikili hedef (SEÇİLDİ) · **(B)** yalnız `net10.0-windows` · **(C)** yalnız `net48` · **(D)** yalnız `net8.0`. Tam eleme gerekçeleri §3.2 tablosundadır; bu bölüm seçeneklerin **bağlamını** verir: B, legacy kanıtı yok eder; C, modern platformu ve dil yeniliklerini dondurur; D, hem eski planın hem yeni bootstrap'in dışındadır ve üçüncü bir versiyon karmaşası doğurur.

### §2.4 — Risk R5 / belirsizlik B3 bağlamı (net48 + DevExpress v26.1)

| ID | İfade | Etki | Karar anındaki durum |
|---|---|---|---|
| **R5** | `net48` bacağı ile DevExpress Universal 2026 / v26.1 uyumluluğunun **doğrulanmamış** olması | Yüksek | açık — ilk `dotnet build` ile kapatılacak |
| **B3** | DevExpress kontrol setinin (7 ekran adayları) kesinleşmemesi — R5 ile aynı ailede belirsizlik | Orta | PART 4/5'te kapanacak |
| A3 | `.templates/` mevcut iş, çakışma yok | Düşük | ✅ doğrulandı |

> **UYARI:** R5 için **kanıt yoktur**; uyumluluk varsayımı **`INFERRED`** etiketi taşır. Doğrulama anı: ilk `dotnet build` (PART 5). Çıkmazsa §4.3 azaltma planı işletilir ve kullanıcıya raporlanır (G10 → `DECISION BLOCKED`, sessiz çözülmez).

---

## §3 Karar (Decision)

### §3.1 — Karar ifadesi (tek cümle)

**Tüm VersaCoder projeleri `net48` ve `net10.0-windows` olmak üzere çift TFM ile derlenir, ortak kaynakta tutulur ve her iki bacakta da test edilir; `#if` yalnız platform farklarına izin verilir — karar verildi.**

### §3.2 — Alternatifler

| # | Alternatif | Artı | Eksi | Neden elendi |
|---|---|---|---|---|
| A — `net48` + `net10.0-windows` (SEÇİLDİ) | legacy kanıt + modern kanıt; ortak kaynak; DevExpress tek sürümde iki bacakta | iki kat build/test maliyeti; R5 riski | **elenmedi** — tek seçenek ikisini de kanıtlıyor |
| B — Yalnız `net10.0-windows` | en az build maliyeti, modern API serbestliği | legacy kanıt yok; eski makinede açılmaz; **G6 doğrudan ihlal** | "net48 sonra bakarız" yasağı → elendi |
| C — Yalnız `net48` | en geniş çalışır-alanı, DevExpress sorunu minimum | dil/BCL yenilikleri kapalı; modern kanıt yok; teknoloji düşmanı görünüm | iki amacın yarısını karşılar → elendi |
| D — Yalnız `net8.0` | ai.01'in eski yığınıyla uyumlu | ai.03 tarafından **geçersiz kılınmış**; G8 (teknoloji anayasası) ihlali; üçüncü versiyon karmaşası | kaynak çelişkisi G10 ile çözüldü → elendi |
| E — `net48` + `net10.0-windows` ama test tek bacakta | test süresi yarıya iner | kanıtsız ikinci bacak; G5 ihlali; platform farkları ancak testte görünür | "neden test ediyoruz" sorusunun cevabı kalmaz → elendi |

### §3.3 — Gerekçe

Kısıt: hem eski kurumsal ortamda çalışma (legacy kanıt) hem modern runtime'da çalışma kanıtı isteniyor; tek TFM bunların **ikisini birden** kanıtlayamaz. Ölçülebilir etki: her iki bacakta `dotnet build` → 0 hata ve `dotnet test` → yeşil, G5 kanıtının kendisidir. Elenen B seçeneğinin bedeli: satış/kurulum anında "eski makinede açılmıyor" hatası — kod hatası değil, **kanıt** hatası; düzeltmesi tüm projeyi yeniden hedeflemek (günler) olurdu. C seçeneğinin bedeli: ürünün modern ekosisteme (nullable referans, yeni BCL, performans) kilitlenmesi. D seçeneği ise iki kaynak arasındaki çelişkiyi G10'a göre çözmeyi reddeder. E seçeneği cazip görünür (yarı test maliyeti) ama platform farkları ancak ikinci bacakta ortaya çıkar; kanıt olmadan "derlenir" demek G5 ihlalidir.

### §3.4 — Koşullu derleme politikası (`#if`)

| # | Kural | Doğru | Yanlış |
|---|---|---|---|
| 1 | `#if` **yalnız** platform/uyumluluk farklarında | `#if NET48` içinde `AppDomain`/özel API uyarlaması | `#if` ile `if (isPro)` iş mantığı |
| 2 | Ortak kaynak dosyası, **her iki TFM'de de aynı dosya** | tek `Foo.cs` | `Foo.net48.cs` + `Foo.net10.cs` kopyası (platform kodunu çoğaltmak yasak — G6) |
| 3 | API yüzeyi = **iki bacağın kesişimi** | yalnız ikisinde var olan API çağrılır | yalnız net10'da var olan API doğrudan çağrılır |
| 4 | Kesişimde olmayan ihtiyaç → soyutlama | `InternalsVisibleTo`/arayüz/`partial` ile platform adaptörü | kopyala-yapıştır iki implementasyon |
| 5 | `#if` bloğu minik kalır | 1–5 satır adaptasyon | 50 satırlık gövde (test edilemez) |
| 6 | TFM sembolleri | `NET48` · `NET10_0_WINDOWS` | `NETFRAMEWORK`/`NETCOREAPP` karışımı (tutarlı değil) |
| 7 | Yeni API ihtiyacı → önce kesişim ara, sonra `#if`, en son **açık soru** olarak §3.11'e yaz | sıralama: kesişim → `#if` → soru | doğrudan `#if` |

```text
API ihtiyaci dog
   |
   +--> Her iki TFM'de var mi?  --EVET--> ortak kod (dogru)
   |
   +--> Yalniz birinde var?    --EVET--> minik #if adaptasyonu
   |                                    (yalniz platform farki)
   +--> Ikisinde de yok / riskli? --> soyutla (arayuz) veya
                                      acik soru olarak §3.11'e yaz
```

### §3.5 — Kabul kriterleri (ölçülebilir)

| # | Kriter | Ölçüt | Kanıt |
|---|---|---|---|
| KR1 | Her proje çift TFM'de derleniyor | `dotnet build` → **0 hata, her iki `-f` için** | build çıktısı (§5.3/1–2) |
| KR2 | Testler **her iki bacakta** koşuyor | `dotnet test -f net48` + `-f net10.0-windows` → yeşil | test çıktısı (§6.2) |
| KR3 | Tek bacaklı `.csproj` yok | `TargetFramework` satırı tek başına hiçbir projede değil | §5.3/3 |
| KR4 | `#if` yalnız platform farkı | her `#if` bloğu ≤ 10 satır + gerekçe yorumu | §5.3/4 incelemesi |
| KR5 | API yüzeyi kesişimde | yalnız bir bacakta derlenen çağrı **0** | KR1 derlemesi zorlar |
| KR6 | R5 spike sonucu yazılı | ilk build sonrası uyum raporu (`log.md` tek satır) | §4.3 |

### §3.6 — Bağımlılıklar ve ön koşullar

| # | Bağımlılık | Durum |
|---|---|---|
| 1 | .NET 10 SDK + `global.json` (sürüm sabitleme) | ⚠️ VERIFICATION REQUIRED (PART 5) |
| 2 | .NET Framework 4.8 Developer Pack (net48 hedefleme paketi) | ⚠️ VERIFICATION REQUIRED |
| 3 | DevExpress Universal 2026 / v26.1 — **net48 uyumluluğu** (R5) | ⚠️ VERIFICATION REQUIRED — spike |
| 4 | CPM `Directory.Packages.props` (tek sürüm kaynağı) | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) §3.6 |
| 5 | `src/` solution (`.sln`/`.slnx` — B1) | ⚠️ VERIFICATION REQUIRED (PART 5) |
| 6 | Veri erişimi iki bacakta aynı davranış ([ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)) | kabul edildi |

### §3.7 — Riskler ve azaltıcılar (R5 spike planı dahil)

| Risk | Olasılık | Etki | Azaltıcı |
|---|---|---|---|
| **R5:** net48 + DevExpress v26.1 uyumsuzluğu | orta | **yüksek** | §4.3 spike + fallback zinciri |
| Build/test süresi 2× | kesin | orta | paralel `-f` koşuları; incrementality |
| `#if` sayısı şişer (G6 ihlali) | orta | yüksek | §3.4/7 denetimi + NetArchTest kuralı |
| Yalnız modern API bilen geliştirici net48'i kırar | orta | orta | KR1 her PR'da; derleyici hata verir |
| net48 paket Restorasyon farkı (CPM) | düşük | orta | CPM tek sürüm + `dotnet restore` kontrolü |
| Test fakes'leri TFM'e özgü davranır | düşük | orta | ortak test kaynağı, TFM'e özgü yalnız fixture |

### §3.8 — Geri alma (reversibility)

Karar **kısmen geri alınabilir** (pahalı ama mümkündür). Geri alma adımları:

| # | Tetik | Geri alma | Süre |
|---|---|---|---|
| 1 | DevExpress net48'te kesin uyumsuz | §4.3 Fallback-1 → Fallback-2 zinciri + **kullanıcı onayı** | ~1 gün |
| 2 | Build süresi katlanılmaz | test matrisini PR'da 1 bacak + gecede 2 bacak yap | ~1 saat |
| 3 | `#if` kirlılığı | soyutlama katmanına taşı (refactor, davranış değişmez) | ~4 saat |
| 4 | Tek bacağa düşme kararı | **yeni ADR** gerekir; bu ADR `superseded` olur, silinmez (G7) | ~40 dk yazımda |

> **UYARI:** Tek bacağa düşmek **G6 ihlalidir**; sessiz yapılamaz — yeni ADR + kullanıcı onayı (G7) zorunludur.

### §3.9 — Karar veren ve danışılanlar

| Rol | Kişi/Varlık |
|---|---|
| Karar veren | architect (senior karar #1 — `reports/00-kapsam-analizi.md` §2) |
| Kaynak | ai.03.md §TARGET FRAMEWORKS (bootstrap) |
| Danışılan | reviewer · tester (test matrisi) |
| Uygulayan | developer (PART 5 proje iskeleti) |
| Reviewer | reviewer (`#if` ve TFM denetimi) |

### §3.10 — Açık sorular

| # | Soru | Sorumlu | Son tarih |
|---|---|---|---|
| Q1 | DevExpress v26.1 net48'te tam destekli mi? (R5 spike sonucu) | developer | PART 5 ilk build |
| Q2 | Solution `.sln` mi `.slnx` mi? (B1) | architect | PART 5 |
| Q3 | net48 test bacağında hangi xUnit sürümleri destekleniyor? (B2) | tester | PART 5 |
| Q4 | CLI projesi tek bacak (`net10.0-windows`) istisnası mı, yoksa çift mi? (G6 yorumu) | architect | PART 5 |

### §3.11 — Karar sonrası izleme metrikleri

| Metrik | Hedef | Sıklık | Kaynak |
|---|---|---|---|
| `dotnet build` net48 | 0 hata | her PR | §5.3/1 |
| `dotnet build` net10.0-windows | 0 hata | her PR | §5.3/2 |
| `dotnet test` iki bacak | yeşil | her PR | §6.2 |
| Tek bacaklı `.csproj` | 0 | her PR | §5.3/3 |
| `#if` bloğu / dosya | ≤ 3 (eğilim) | aylık | §5.3/4 |
| Sürüm/teknoloji sapması (EF Core vb.) | 0 | her PR | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §6.5 |

---

## §4 Korkular ve Guardrails

### §4.1 — Bu kararın korkuları

1. **En kötü korku:** net48 + DevExpress v26.1 birleşimi **derlenmez** → tüm UI planı yeniden yazılır (R5).
2. İkinci korku: `#if` ormanı → kodun yarısı test edilemez, iki ürün haline gelir (G6 ihlali).
3. Üçüncü korku: bir bacak "kayıp" — sadece lokal ortamda derlenir, CI'da ihmal edilir (G5).
4. Dördüncüsü: geliştirici modern API kullanır, net48'de **çalışma anında** kırılır (derleme değil runtime).
5. Beşincisi: paket sürüm farkı — CPM dışı bir `.csproj` elde paket çeker.

### §4.2 — Risk matrisi

| Risk | Olasılık | Etki | Skor |
|---|---|---|---|
| R5 — DevExpress net48 uyumsuzluğu | orta | çok yüksek | **HH** |
| `#if` kirlılığı | orta | yüksek | **H** |
| İkinci bacağın ihmal edilmesi | orta | yüksek | **H** |
| Runtime-only API kullanımı | yüksek | orta | **H** |
| Build süresi 2× | kesin | düşük | M |
| CPM dışı paket sürümü | düşük | orta | M |

### §4.3 — Azaltma planı (R5 spike + fallback zinciri)

**Spike (ilk `dotnet build` anında, PART 5 başı):**

```text
Adim 1  En kucuk UI projesini (tek form) net48 hedefiyle derle
Adim 2  DevExpress v26.1 paketlerini net48'e gecir, restore et
Adim 3  Hata cikarsa: hata metnini log.md'ye YAZ (kanit)
Adim 4  Hata yoksa: tek formu net48'de CALISTIR (runtime kaniti)
Adim 5  Sonucu R5 satirina isle: kapat (0 hata) veya acik birak
```

| # | Fallback | Koşul | Sonuç |
|---|---|---|---|
| **F0** | Sorunsuz derlenir | spike temiz | R5 kapanır, plan aynen yürür |
| **F1** | Paket/TFM uyumsuzluğu → DevExpress paket setini net48 uyumlu yapılandırma (referans/paket seçimi) | ilk denemede hata | ~1 gün; `log.md` + rapor |
| **F2** | UI katmanını soyutla: **ortak çekirdek** iki bacakta, **DevExpress barındıran form katmanı** stratejisi ayrı ADR ile tanımlanır | F1 başarısız | yeni ADR + **kullanıcı onayı** (G7/G10) |
| **F3** | `DECISION BLOCKED` — kullanıcıya raporla, seçenekleri sun | F2 de imkânsız | sessiz çözüm **yasak** (G10) |

> **KURAL:** Fallback'lerin **hiçbiri sessiz uygulanmaz**; her adım `log.md`'ye kanıt satırıyla yazılır. R5, kanıt gelene kadar `INFERRED` kalır.

### §4.4 — Kabullenilemez kriterler (otomatik RED)

| # | Kriter | İhlalde |
|---|---|---|
| 1 | Tek bacaklı `.csproj` (`TargetFramework` tekil) | HIGH — mimari RED (G6) |
| 2 | `#if` ile iş mantığı bölünmesi | HIGH — kod RED |
| 3 | İki bacak kopyası (`Foo.net48.cs` / `Foo.net10.cs`) | HIGH — G6 ihlali |
| 4 | "net48'i sonra ekleriz" ifadesi | RED — bu ADR bağlar |
| 5 | `net8.0` / `net9.0` / başka TFM referansı | RED — G8, teknoloji anayasası |
| 6 | Tek bacakta test + "her iki bacakta geçer" iddiası | CRITICAL — G5 ihlali |

### §4.5 — Guardrail Tablosu (AG1–AG14)

| # | Guardrail | Kural | İlgili CLAUDE.md | İhlal Sonucu |
|---|---|---|---|---|
| AG1 | Frontmatter 10 alan eksiksiz | `authority: SSOT` dahil | G4 | RED |
| AG2 | Slug/tipografi | İngilizce slug; ı ğ ş İ ö ç ü doğru | G4 | RED |
| AG3 | Secret yok | bağlantı stringi / lisans anahtarı bu dosyada değil | G9 | kritik RED |
| AG4 | Çift TFM | her `.csproj` `net48;net10.0-windows` | **G6** | HIGH — mimari RED |
| AG5 | Kanıtsız iddia `⚠️` | R5 uyumluluğu doğrulanana kadar `INFERRED` | G9 | RED |
| AG6 | Kopyalama yok | şablon/plan metni blok blok taşınmadı | G1 | RED |
| AG7 | Numara tekilliği | `ADR-001` tek dosya | — | RED |
| AG8 | Slug: `dual-target-net48-net10` | Türkçe karakter/boşluk yok | — | RED |
| AG9 | Supersede zinciri | bu ADR henüz ezilmedi (`superseded_by` boş) | — | RED |
| AG10 | Test kapsamı | iki bacakta da test koşuyor | **G5** | CRITICAL |
| AG11 | Uyarı yok | `TreatWarningsAsErrors` temiz | G5 | RED |
| AG12 | Defter senkronu | `.decisions/adr-index.md` satırı var | G4 | RED |
| AG13 | Yeni TFM = yeni ADR | TFM değişikliği bu dosyadan yapılır | G8 | RED |
| AG14 | Yalnız-oku denetimi | §5.3 komutları yazmaz | G7 | CRITICAL |

---

## §5 Uygulama ve Hata Modları

### §5.1 — Uygulama adımları (11 adım)

| # | Adım | Çıktı |
|---|---|---|
| 1 | SDK kurulumu + `global.json` sabitleme (B1 ile birlikte) | SDK sürüm kanıtı |
| 2 | `.NET Framework 4.8 Developer Pack` doğrulama | `Reference Assemblies` yolu kontrolü |
| 3 | Solution dosyasını oluştur (`.sln`/`.slnx` — Q2) | solution dosyası |
| 4 | Her `.csproj`'e `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>` | csproj diff'i |
| 5 | **R5 spike'ı** (§4.3 Adim 1–5) | spike raporu (`log.md`) |
| 6 | Ortak katmanları derle (Abstractions → … → UI sırası) | 0 hata |
| 7 | Test projelerini **iki bacakta** bağla | test matrix çıktısı |
| 8 | `#if` politikasını NetArchTest ile denetle (§3.4) | mimari test |
| 9 | `dotnet build` + `dotnet test` iki bacak | kanıtlı yeşil (G5) |
| 10 | R5 sonucunu ADR/`log.md`'ye işle; açık kaldıysa Fallback zinciri | durum satırı |
| 11 | `.decisions/adr-index.md` + `log.md` senkronu | defter + günlük |

### §5.2 — Hata modları

| # | Hata modu | Belirti | Kök neden | Düzeltme |
|---|---|---|---|---|
| 1 | net48 restore hatası | `NU1202`/benzeri, paket TFM desteklemiyor | paket net48'i desteklemiyor | paket alternatifi / soyutlama (§4.3 F1) |
| 2 | DevExpress net48'te yüklenmiyor | `TypeLoadException` / derleme hatası | **R5** gerçekleşti | §4.3 F1 → F2 → F3 |
| 3 | Tek bacak unutuldu | bir `.csproj`'de tek `TargetFramework` | elle yazılmış | `TargetFrameworks` çoklu yap (AG4) |
| 4 | `#if` şişmesi | dosyada 10+ blok | iş mantığı `#if`'e girmiş | soyutla; `#if` ≤ 10 satır |
| 5 | Yalnız net10 API çağrısı | net48 build'de CS1061/C0103 benzeri | kesişim ihlali | kesişim API'sine dön veya minik adaptör |
| 6 | Test tek bacakta | `dotnet test` tek `-f` ile | KR2 unutulmuş | iki `-f` koşusu zorunlu |
| 7 | Runtime kırılması (yalnız net48) | uygulama net48'de açılırken patlar | derlemede olmayan API/runtime farkı | `#if` adaptörü + regresyon testi |
| 8 | CPM dışı sürüm | iki projede farklı paket sürümü | `Directory.Packages.props` bypass | CPM'e dön (ADR-004 §3.6) |
| 9 | Uyarılar sessizce geçiliyor | build uyarı dolu | `TreatWarningsAsErrors` kapalı | aç; AG11 |
| 10 | Spike atlandı | R5 hâlâ doğrulanmadan UI kurulmuş | G5/G12 ihlali | UI inşası durdurulur, spike geriye çalıştırılır |

### §5.3 — Doğrulama komutları (yalnız-oku)

```powershell
# 1 -- net48 bacak derlemesi (0 hata OLMALI)
dotnet build .\src\VersaCoder.sln -f net48 -warnaserror
# 2 -- net10.0-windows bacak derlemesi (0 hata OLMALI)
dotnet build .\src\VersaCoder.sln -f net10.0-windows -warnaserror
# 3 -- tek bacakli .csproj KALMAMALI (cikti bos olmali)
Select-String -Path .\src\**\*.csproj -Pattern '<TargetFramework>' 
# 4 -- #if kullanimi (yalniz platform farki OLMALI; bloklar kisa)
Select-String -Path .\src\**\*.cs -Pattern '#if\s+(NET48|NET10_0_WINDOWS)'
# 5 -- TFM satirlari (hepsi cift hedef OLMALI)
Select-String -Path .\src\**\*.csproj -Pattern 'TargetFrameworks'
# 6 -- yasak TFM / eski surum referansi CIKMEMELI (net8.0, net9.0)
Select-String -Path .\src\**\*.csproj -Pattern 'net[0-9]\.0(?!-windows)' |
  Where-Object { $_.Line -notmatch 'net48|net10.0-windows' }
```

> **UYARI:** 1–2 numaralı komutlar **yazma yapmaz ama derleme çalıştırır**; çıktı `0 Hata` satırı içermelidir (G5). `src/` diskte yoksa komut dürüstçe hata verir — bu da bir kanıttır (`⚠️ VERIFICATION REQUIRED`, PART 5'te kapanır).

---

## §6 Doğrulama ve Testler

### §6.1 — Kontrol listesi (18 madde)

- [ ] 1. Frontmatter 10 alan eksiksiz (`title,type,category,status,version,updated,authority,adr_id,date,tags`)
- [ ] 2. `authority: SSOT` birebir
- [ ] 3. H1 + **Zorunlu Bağlantılar** satırı (9 bağlantı, tümü diskte var)
- [ ] 4. §1–§7 iskeleti eksiksiz
- [ ] 5. §3.1 karar tek cümlede (`… karar verildi.`)
- [ ] 6. §3.2 alternatifler ≥2 satır (5 satır), 4 sütun dolu
- [ ] 7. `net8.0` yalnız "elenen alternatif" olarak geçiyor (seçilen teknoloji değil)
- [ ] 8. DevExpress yalnız **v26.1 / Universal 2026** (25.2.8, 24.2.x **yok**)
- [ ] 9. EF Core yalnız "yasak" bağlamında (seçilen değil)
- [ ] 10. R5/B3 azaltma planı yazılı (§4.3 spike + F0–F3)
- [ ] 11. `#if` politikası 7 kural (§3.4)
- [ ] 12. Kabul kriterleri ölçülebilir KR1–KR6
- [ ] 13. Geri alma tablosu (§3.8) + reversibility beyanı
- [ ] 14. AG1–AG14 "İhlal Sonucu" sütunlu
- [ ] 15. 11 uygulama adımı + ≥10 hata modu (§5.2)
- [ ] 16. 6 yalnız-oku komutu + "olmalı" yorumu
- [ ] 17. §7'de **gerçek itiraz + cevap** var (`> **RED TEAM:**` bloğu)
- [ ] 18. Mojibake yok; ASCII diyagram **saf ASCII** (Unicode kutu karakteri 0)

### §6.2 — Test stratejisi

| Tip | Kapsam | Araç | Beklenen |
|---|---|---|---|
| Build (net48) | tüm projeler | `dotnet build -f net48 -warnaserror` | 0 hata / 0 uyarı |
| Build (net10.0-windows) | tüm projeler | `dotnet build -f net10.0-windows` | 0 hata / 0 uyarı |
| Unit (net48) | servis/domain testleri | `dotnet test -f net48` | yeşil |
| Unit (net10.0-windows) | aynı test kaynağı | `dotnet test -f net10.0-windows` | yeşil |
| Architecture | `#if` politikası + çift TFM | NetArchTest | 0 ihlal |
| Manual (R5 spike) | tek form net48'de açılıyor mu | çalıştırma | ekran açılıyor |
| Coverage | hedef %90 (vault standardı) | coverage raporu | ≥ %90 |

### §6.3 — Kanıt tablosu

| Kanıt | Konum | Durum |
|---|---|---|
| Karar kaynağı (ai.03 §TARGET FRAMEWORKS) | `reports/00-kapsam-analizi.md` §2/1 | ✅ SOURCE_VERIFIED (rapor üzerinden) |
| G6 metni | `CLAUDE.md` §1/G6 | ✅ okundu 2026-09-25 |
| Teknoloji anayasası satırı | `CLAUDE.md` §4 | ✅ okundu 2026-09-25 |
| `dotnet build` iki bacak | `src/` | ⚠️ VERIFICATION REQUIRED — PART 5 |
| DevExpress net48 uyumluluğu (R5) | ilk build çıktısı | ⚠️ VERIFICATION REQUIRED — spike |
| `global.json` / `Directory.Packages.props` | repo kökü | ⚠️ VERIFICATION REQUIRED — PART 5 |

### §6.4 — Doğrulama komutları

§5.3 komutları (1–6) + `dotnet test` iki bacak. Geçme eşiği: **hepsi "olmalı" yorumlarını karşılamalı**; herhangi biri >0 satır dönerse §5.2 hata moduna düşülür.

### §6.5 — Şablona özgü ek kontroller

1. **TFM çiftliği:** `TargetFrameworks` değeri birebir `net48;net10.0-windows` (sıra dahil).
2. **Tek dosya kuralı:** hiçbir `*.cs` dosyasının TFM'e özel kopyası yok (`*.net48.cs` = 0).
3. **Kesişim API'si:** yalnız bir bacakta var olan çağrı derleme ile düşer (KR5) — derleyici birincil denetçidir.
4. **Yasaklı referans taraması:** `net8.0`, `net9.0`, `netstandard2.1` (tek başına) → 0.
5. **EF Core / DevExpress sürümü:** yalnız ilgili ADR'lerin diliyle geçer → [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §6.5, [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) §6.5.
6. **Boyut bandı:** bu dosya 20–30 KB → `(Get-Item .ai\decisions\accepted\ADR-001-*.md).Length`.

---

## §7 Sonuç ve Red Team

### §7.1 — Sonuç özeti

| Alan | Sonuç |
|---|---|
| Karar | Çift TFM `net48` + `net10.0-windows`; ortak kaynak; `#if` yalnız platform farkı; iki bacakta build **ve** test |
| Durum | `accepted` |
| Versiyon | 1.0.0 |
| Tarih | 2026-09-25 |
| Sahip | architect |
| İlgili ADR'ler | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) (UI + sürüm) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) (veri erişimi) · [ADR-002](ADR-002-handwritten-content-no-copypaste.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) · [Defter](../adr-index.md) |
| Etkilenen G'ler | **G5** · **G6** · G8 · G9 · G10 |
| İlişkili risk | **R5** (+ B3 DevExpress belirsizliği ailesi) |
| Sonraki adım | PART 5 ilk `dotnet build` + R5 spike → Q1/Q2/Q3/Q4 kapanışı |

### §7.2 — Red Team (bu karar nasıl yanlış gider?) — gerçek itiraz + cevap

> **RED TEAM:** *"Çift TFM, DevExpress v26.1 ile net48'te **bilinmeyen** bir uyumsuzluk çıkarsa bu karar daha ilk build'de çöker ve tüm UI planı (7 ekran, Ribbon, Designer.cs düzeni) çöpe gider. Ayrıca iki bacak testi, tek kişilik bir ekipte 'her PR'da iki kez test' tembelliğine yol açar: ilk ay herkes net10'da koşar, net48 bacağı **kayip bacak** olur ve legacy kanıt iddiası kanıtsız kalır. Yani bu karar ya maliyetiyle (süre) ya da kanıtsızlığıyla (ihmal) yüzleşir — ikisi de G5'in ruhuna aykırıdır."*

**Cevap (rebuttal):** İtiraz iki iddia taşıyor ve ikisi de ölçülebilir. (1) **R5 çökme senaryosu:** doğru — bu yüzden §4.3'te spike **ilk build'de**, UI inşasından **önce** çalıştırılır; F0–F3 fallback zinciri hazır ve F3'te karar **sessizce değil**, kullanıcıya raporlanarak durur (G10). Yani senaryo gerçekleşirse kayıp "tüm UI planı" değil, **≈1 günlük spike** olur; gerçekleşmezse R5 kapanır. (2) **Kayıp bacak ihmali:** doğru tespit — bu yüzden KR1/KR2 **her PR'da zorunlu** bir CI kapısı olarak tanımlanmıştır; "net48'de geçti" iddiası yalnız `dotnet build -f net48` çıktısıyla söylenir (G5). İhmali ölçülebilir kılan metrik §3.11'de: tek bacaklı `.csproj` = 0, iki `-f` build = her PR. Sonuç: itirazın ikisi de bu ADR'nin **kontrol listesine** dönüşmüştür; savunma "güveniyoruz" değil, "ölçüyoruz" der.

| # | İtiraz | Senaryo | Savunma |
|---|---|---|---|
| 1 | R5 gerçeğe dönüşür | DevExpress net48'te derlenmez | spike ilk build'de + F0–F3 + kullanıcı onayı |
| 2 | İkinci bacak ihmal edilir | herkes net10'da koşar | KR2 CI kapısı; iki `-f` build/test |
| 3 | `#if` ormanı olur | iki ürün haline gelir | §3.4 ≤10 satır + NetArchTest |
| 4 | Build/test süresi 2× | PR döngüsü yavaşlar | paralel `-f`; gecede tam matris |
| 5 | Modern API cazibesi | net48'de çalışma anı kırılması | derleyici + TFM-specific regresyon testi |
| 6 | Legacy kanıt gösterişli | kimse eski makinede denemiyor | R5 spike'sında **çalıştırma** kanıtı zorunlu |
| 7 | `net8.0` geri gelir | eski plan çekiciliği | G8 + bu ADR §3.2/D; yalnız yeni ADR ile |
| 8 | CPM bozulur | projeler arası sürüm farkı | ADR-004 §3.6 CPM tek sürüm |
| 9 | Test framework bacak ayrışır | net48'te xUnit kısıtı | Q3 açık soru; ortak test kaynağı |
| 10 | Solution biçimi yanlış seçer | `.sln`/`.slnx` karmaşası | Q2 açık soru; PART 5'te kapanır |
| 11 | Tek bacak "geçici" kabul edilir | G6 ihlali normalleşir | AG4 = HIGH RED; §3.8/4 yeni ADR zorunlu |

### §7.3 — Onay ve dağıtım

| Rol | İsim | Tarih | İmza |
|---|---|---|---|
| Yazar | architect | 2026-09-25 | — |
| Uygulayan | developer | 2026-09-25 | — |
| Mimari sorumlu | architect | 2026-09-25 | — |
| Reviewer | reviewer | 2026-09-25 | — |

---

**Zorunlu Bağlantılar (tekrar):** [ADR Defteri](../adr-index.md) · [Vault Ana Sayfa](../../index.md) · [Anayasa](../../CLAUDE.md) · [Kapsam Analizi](../../reports/00-kapsam-analizi.md)
