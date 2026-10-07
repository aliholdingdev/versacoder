---
title: "ADR Şablonu — DevExpress / Masaüstü UI Kararları (RibbonForm, Designer.cs, Tema)"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "DevExpress masastu UI ADR sablonu: RibbonForm, Designer.cs ve tema kararlari"
---

# ADR Şablonu — DevExpress / Masaüstü UI Kararları

**Zorunlu Bağlantılar:** [Vault Ana Sayfa](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [ADR Dizini](../../.decisions/index.md) · [Genel ADR Şablonu](adr-template.md) · [ADR Dizin Şablonu](adr-index.md)

---

## §1 — Bu Şablonun Kapsamı ve Kullanımı

### §1.1 — Ne zaman kullanılır

VersaCoder.Desktop (WinForms `RibbonForm`) tarafındaki DevExpress kararlarında kullanılır:

1. DevExpress sürüm sabitleme veya yükseltme (ör. 25.2.8).
2. Ribbon / Grid / Dock / tema seçimi ve kişiselleştirmesi.
3. Hand-written `Designer.cs` + `.resx` + `ResourceManager` düzeninin korunması.
4. DevExpress bileşeninin yeni ekrana eklenmesi veya kaldırılması.
5. WinForms ↔ DevExpress API değişim kararları (net48 + net10.0-windows uyumu).
6. Licensing, Skia render, high-DPI, tema çakışması gibi teknik sorunlar.

### §1.2 — Bu şablonu kopyalamadan önce

| Kontrol | Beklenen | Durum |
|---|---|---|
| DevExpress sürümü | 25.2.8 (projede sabit) | ⚠️ VERIFICATION REQUIRED |
| Ribbon stili | Office2010 | ⚠️ VERIFICATION REQUIRED |
| TFM | net48 + net10.0-windows | ⚠️ VERIFICATION REQUIRED |
| Tasarımcı dosyaları | hand-written Designer.cs/.resx | ⚠️ VERIFICATION REQUIRED |
| Ekran sayısı | 7 ekran (Roles, Catalog, Chat, ...) | ⚠️ VERIFICATION REQUIRED |

> **Not:** Yukarıdaki satırlar VersaCoder planındaki varsayımlardır; disk kanıtıyla doğrulanana kadar `⚠️ VERIFICATION REQUIRED` işaretli kalır.

### §1.3 — Şablonun 7 bölümlük zorunlu iskeleti (silinemez)

| Bölüm | Ad | DevExpress bağlamında zorunlu içerik |
|---|---|---|
| §1 | Kapsam ve Kullanım | DevExpress karar tipi + doğrulanacak bileşen envanteri |
| §2 | Bağlam (Context) | DevExpress 25.2.8 / RibbonForm / Designer.cs gerçeği |
| §3 | Karar (Decision) | 11 alt bölüm; §3.10 = dolu ADR-001 örneği |
| §4 | Korkular ve Guardrail'lar | G1–G14; DevExpress'e özgü 4 satır |
| §5 | Uygulama ve Hata Modları | 11 adım; DevExpress hata modları; 6 PowerShell komutu |
| §6 | Doğrulama ve Testler | 18 madde kontrol listesi + DevExpress testleri |
| §7 | Sonuç ve Red Team | Red Team 10+ satır masaüstü bakış açısıyla |

### §1.4 — Bu şablonla yazılan tipik ADR slugsı

`devexpress-version`, `ribbon-style`, `designer-files`, `ui-theme`, `grid-virtualization`, `high-dpi`, `licensing` — İngilizce, tireli, Türkçe karakter yok.

---


### §1.5 — Bu şablonun dayandığı VersaCoder gerçekleri

| # | Gerçek | Kaynak |
|---|---|---|
| 1 | Katmanlar: Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI + CrossCutting + Cli | [ARCHITECTURE](../../architecture/index.md) |
| 2 | Desktop uygulaması: VersaCoder.Desktop (WinForms RibbonForm) | ⚠️ VERIFICATION REQUIRED |
| 3 | CLI uygulaması: VersaCoder.Cli ( DevExpress içermez ) | ⚠️ VERIFICATION REQUIRED |
| 4 | DevExpress 25.2.8 + RibbonControlStyle.Office2010 | ⚠️ VERIFICATION REQUIRED |
| 5 | TFM: net48 + net10.0-windows | ⚠️ VERIFICATION REQUIRED |
| 6 | hand-written Designer.cs / .resx / ResourceManager | ⚠️ VERIFICATION REQUIRED |
| 7 | Test: xunit, %90 coverage, TreatWarningsAsErrors=true | [rules](../../.rules) |
| 8 | Vault kök dosyaları + .ai/.rules/ 7 dosya | [index](../../index.md) |

## §2 — Bağlam (Context)

### §2.1 — Karar ortamı

| Boyut | VersaCoder Gerçeği | Kaynak |
|---|---|---|
| UI framework | DevExpress WinForms, `RibbonForm` | ⚠️ VERIFICATION REQUIRED |
| Sürüm | 25.2.8 | ⚠️ VERIFICATION REQUIRED |
| Ribbon stili | `RibbonControlStyle.Office2010` | ⚠️ VERIFICATION REQUIRED |
| TFM | `net48` + `net10.0-windows` | ⚠️ VERIFICATION REQUIRED |
| Tasarım dosyaları | hand-written `Designer.cs` / `.resx` / `ResourceManager` | ⚠️ VERIFICATION REQUIRED |
| Ekranlar | 7 ekran (Roles, Catalog, Chat, ...) | ⚠️ VERIFICATION REQUIRED |
| Katman konumu | yalnızca `UI` katmanı + `Host` | [ARCHITECTURE](../../architecture/index.md) |

### §2.2 — Kararı tetikleyen tetikleyiciler

1. DevExpress sürümü değişince tüm `Designer.cs` dosyaları kırılabilir.
2. Yeni ekran eklenirken tasarımcı tarafında otomatik kod üretimi hand-written dosyaları ezebilir.
3. Ribbon stili değişimi tüm araç çubuğu düzenini etkiler.
4. `net48` hedefi eski API'leri zorunlu kılar; `net10.0-windows` tarafında obsoletion çıkabilir.
5. Lisans uyarısı ve Skia/render sorunları derlemeyi değil çalışmayı kırar.

---


### §2.3 — DevExpress bileşen envanteri (karardan önce doldur)

| Bileşen | Kullanım | Ekran | Durum |
|---|---|---|---|
| RibbonControl | ana araç çubuğu (Office2010) | tümü | ⚠️ VERIFICATION REQUIRED |
| GridControl / GridView | katalog & rol listeleri | Catalog/Roles | ⚠️ VERIFICATION REQUIRED |
| DockManager | panel yerleşimi | Chat | ⚠️ VERIFICATION REQUIRED |
| LayoutControl | form düzeni | [ekran] | ⚠️ VERIFICATION REQUIRED |
| MemoEdit / TextEdit | giriş alanları | Chat | ⚠️ VERIFICATION REQUIRED |
| SplashScreen | açılış göstergesi | — | ⚠️ VERIFICATION REQUIRED |
| OfficeThemeProvider | tema sağlayıcı | — | ⚠️ VERIFICATION REQUIRED |

> Envanter, §4.5 G4/G7/G8/G9 denetiminin girdisidir; karar kapsamı bu tabloyla daraltılır.

### §2.4 — Kapsam dışı açıkça şunlardır

1. VersaCoder.Cli ( DevExpress bağımlılığı yok ).
2. Application / Domain / Infrastructure / Protocol katmanları ( UI bilmez ).
3. Web/Blazor arayüzü ( VersaCoder'da yok ).
4. DevExpress olmayan üçüncü parti UI kütüphaneleri.

## §3 — Karar (Decision)

> **Şablon Kuralı:** §3.1–§3.9 + §3.11 doldurulur; **§3.10 doldurulmaz, olduğu gibi korunur** (örnek referanstır).

### §3.1 — Karar ifadesi (tek cümle)

**[Bileşen] için [seçim] yaparız; [gerekçe] nedeniyle [kapsam dışı] hariç tutulur.**

### §3.2 — Alternatifler

| # | Alternatif | Neden elendi |
|---|---|---|
| A1 | En son DevExpress sürümüne geç | 25.2.8 dışı → net48 kırılabilir |
| A2 | Hand-written Designer.cs yerine tasarımcı çıktısı | kaynak denetimi kaybolur |
| A3 | DevExpress'siz Standart WinForms | Ribbon/ Grid yeteneği kaybolur |
| A4 | Mevcut karar (buraya yaz) | — |

### §3.3 — Karar gerekçesi

### §3.4 — Kabul kriterleri (ölçülebilir)

1. **KR1:** Derleme `TreatWarningsAsErrors=true` ile temiz. → Kanıt: `dotnet build` çıktısı
2. **KR2:** Hedef bileşen sürümü/konfigürasyonu kayıt altında. → Kanıt: ADR satırı + .csproj
3. **KR3:** Ekran açılır, Ribbon işlevsel. → Kanıt: ekran görüntüsü / manuel test
4. **KR4:** Kapsam %90 test koruması. → Kanıt: coverage raporu

### §3.5 — Kapsam ve kapsam dışı

| Kapsam | Kapsam dışı |
|---|---|
| VersaCoder.Desktop UI katmanı | VersaCoder.Cli |
| DevExpress bileşenleri + Designer.cs | Application/Domain katmanları |
| net48 + net10.0-windows WinForms | web/Blazor tarafı (yok) |

### §3.6 — Bağımlılıklar ve ön koşullar

### §3.7 — Riskler ve azaltıcılar

### §3.8 — Geri alma (reversibility)

Karar **[geri alınabilir / kısmen geri alınabilir / geri alınamaz]**. Geri alma adımları: **[adımlar]**.

### §3.9 — Karar veren ve danışılanlar

| Rol | Kişi/Varlık |
|---|---|
| Karar veren | [rol] |
| Danışılan | [rol/kaynak] |
| Uygulayan | [rol] |
| Reviewer | [rol] |

### §3.10 — ÖRNEK REFERANS: ADR-001 (devexpress-version) — KOPYALANACAKSIN, DEĞİŞTİRİLMEYECEK

> Aşağıdaki blok dolu bir VersaCoder ADR örneğidir. Yeni ADR yazarken bu bloğun **yapısını** kullan; **içeriğini** kendi kararına göre yeniden yaz. Blok silinemez.

#### §3.10.1 — Karar cümlesi
**DevExpress'i 25.2.8'de sabitleriz; sürüm yükselmesi yalnız ADR ile yapılır.**
#### §3.10.2 — Alternatifler
| # | Alternatif | Neden elendi |
|---|---|---|
| A1 | Sınırsız upgrade (en son sürüm) | net48 kırılabilir, ribbon değişir |
| A2 | DevExpress'siz Standart WinForms | Ribbon/Grid yeteneği kaybolur |
| A3 | Sürümü csproj'da floating bırak | derleme gün içinde kırılır |
| A4 | 25.2.8 sabitleme (seçildi) | — |

#### §3.10.3 — Gerekçe
Hand-written `Designer.cs` dosyaları belirli DevExpress API'lerine bağlıdır; sürüm atlaması bu dosyaları sessizce kırar. Sabit sürüm, `net48` + `net10.0-windows` ikili hedefinde bilinen iyi durumu korur.

#### §3.10.4 — Kabul kriterleri
| # | Kriter | Ölçüt | Kanıt |
|---|---|---|---|
| KR1 | Sürüm sabit | 25.2.8, floating yok | .csproj satırı |
| KR2 | Derleme temiz | 0 hata/0 uyarı | dotnet build |
| KR3 | Ribbon açılır | Office2010 stili | manuel test |
| KR4 | Kapsam %90 | - | coverage raporu |

#### §3.10.5 — Kapsam
VersaCoder.Desktop `UI` katmanındaki tüm DevExpress paketleri ve `Designer.cs`/`.resx` dosyaları.
#### §3.10.6 — Kapsam dışı
VersaCoder.Cli, Application/Domain/Infrastructure katmanları, web tarafı.
#### §3.10.7 — Bağımlılıklar
DevExpress 25.2.8 paketleri; `net48` + `net10.0-windows` TFM; hand-written tasarım dosyaları.
#### §3.10.8 — Riskler
| Risk | Olasılık | Etki | Azaltıcı |
|---|---|---|---|
| Sürüm upgrade'ı kırar | orta | yüksek | upgrade = yeni ADR |
| Tasarımcı dosyayı ezer | düşük | yüksek | git diff kapısı |
| Lisans uyarısı | düşük | orta | lisans doğrulama |
| high-DPI bozuk | orta | düşük | DPI testi |

#### §3.10.9 — Geri alma
Sürümü 25.2.8'e döndür + `Designer.cs` diff'ini geri al; reversible.
#### §3.10.10 — Karar veren / danışılan
Karar veren: mimari sorumlusu · Danışılan: UI geliştirici · Uygulayan: UI geliştirici · Reviewer: kod inceleyen.
#### §3.10.11 — İlgili G'ler
G4 (sürüm RED), G7 (Designer.cs korunur), G8 (ribbon stili), G9 (tasarımcı üretimi yasak).


### §3.12 — Karar sonrası izleme metrikleri

| Metrik | Hedef | Sıklık | Kaynak |
|---|---|---|---|
| DevExpress sürüm sapması | 0 (25.2.8) | her build | csproj diff |
| Designer.cs istenmeyen diff | 0 satır | her PR | git diff --stat |
| Derleme uyarısı | 0 | her build | dotnet build |
| Ekran açılış hatası | 0 | manuel | test kaydı |
| Kapsam | ≥ %90 | her PR | coverage raporu |
| high-DPI şikâyeti | 0 | sürümlik | manuel test |

### §3.13 — Karar geçmişi (supersede)

| ADR | Durum | Not |
|---|---|---|
| [önceki karar varsa] | superseded by [bu ADR] | G13 zinciri |
| [bu ADR] | accepted | — |

### §3.11 — Kararla ilgili açık sorular

| # | Soru | Sorumlu | Son tarih |
|---|---|---|---|
| Q1 | [soru] | [rol] | [tarih] |
| Q2 | [soru] | [rol] | [tarih] |

---

## §4 — Korkular, Riskler ve Guardrail'lar

### §4.1 — Bu kararın korkuları

### §4.2 — Risk matrisi

| Risk | Olasılık | Etki | Skor |
|---|---|---|---|
| [risk] | [düşük/orta/yüksek] | [...] | [h/L] |

### §4.3 — Azaltma planı

### §4.4 — Kabullenilemez kriterler (RED)

### §4.5 — Guardrail Tablosu (G1–G14 — silinemez)

| Guardrail | Kural | İhlal Sonucu |
|---|---|---|
| G1 | Frontmatter SSOT alanları eksiksiz | Review RED |
| G2 | İngilizce slug + Türkçe içerik | RED |
| G3 | `keys.md` sızıntısı | kritik RED |
| G4 | DevExpress 25.2.8 dışı sürüm | RED |
| G5 | Kanıtsız iddia | `⚠️` zorunlu |
| G6 | Eski vault'tan kopyalama | RED |
| G7 | Hand-written Designer.cs/.resx ezme | RED |
| G8 | Office2010 dışı ribbon stili | RED |
| G9 | Tasarımcı otomatik kod üretimi | RED |
| G10 | Kapsam %90 altı | RED |
| G11 | Uyarı varlığı | `TreatWarningsAsErrors` → RED |
| G12 | [.decisions/index.md](../../.decisions/index.md) güncellenmedi | RED |
| G13 | Supersede zinciri koptu | RED |
| G14 | PowerShell yalnız-oku ihlali | RED |

---

## §5 — Uygulama ve Hata Modları

### §5.1 — Uygulama Adımları (11 adım)

| # | Adım | Çıktı |
|---|---|---|
| 1 | Mevcut bileşen/sürüm envanterini çıkar | envanter tablosu |
| 2 | csproj paket satırlarını doğrula | .csproj kanıtı |
| 3 | Designer.cs/.resx dosyalarını listele | dosya listesi |
| 4 | Alternatifleri yaz ve el | §3.2 |
| 5 | Kabul kriterlerini yaz | §3.4 |
| 6 | Kararı [ADR Dizini](../../.decisions/index.md)'ne işle | index satırı |
| 7 | Değişikliği uygula | diff |
| 8 | `dotnet build` çalıştır (TreatWarningsAsErrors) | build çıktısı |
| 9 | Ekranı/Ribbon'u manuel doğrula | test notu |
| 10 | Guardrail G1–G14 denetimi | §4.5 işaretleri |
| 11 | Supersede/kapanış kaydı | kapanış notu |

### §5.2 — Hata Modları

| # | Hata Modu | Belirti | Düzeltme |
|---|---||---|
| 1 | Sürüm floating | restore farklı sürüm çekti | pin + lock |
| 2 | Designer.cs ezildi | git diff binlerce satır | revert + yasak |
| 3 | TFM uyuşmazlığı | net48 API bulunamadı | #if / iki hedef |
| 4 | Ribbon stili kayboldu | araç çubuğu boş | Office2010'a dön |
| 5 | Lisans uyarısı | runtime dialog | lisans doğrula |
| 6 | high-DPI bozuk | bulanık/kırık layout | DPI-aware ayar |
| 7 | Skia/render hatası | kontrol çizilmiyor | render ayarı |
| 8 | Coverage düşüşü | %90 altı | test ekle |

### §5.3 — Doğrulama Komutları (yalnız-oku)

```powershell
# 1 — DevExpress sürümünü gör (25.2.8 olmalı)
Select-String -Path .\src\**\*.csproj -Pattern 'DevExpress'
# 2 — ribbon stilini gör (Office2010 olmalı)
Select-String -Path .\src\**\*.cs -Pattern 'RibbonControlStyle'
# 3 — Designer.cs dosyalarını say (7 ekran karşılığı olmalı)
Get-ChildItem -Recurse -Filter *.Designer.cs
# 4 — resx dosyalarını gör (hand-written olmalı)
Get-ChildItem -Recurse -Filter *.resx
# 5 — TFM satırlarını gör (net48 + net10.0-windows olmalı)
Select-String -Path .\src\**\*.csproj -Pattern 'TargetFramework'
# 6 — ResourceManager kullanımını gör (el yazımı olmalı)
Select-String -Path .\src\**\*.cs -Pattern 'ResourceManager'
```

---


### §5.4 — Geri alma koşu senaryosu

| # | Tetik | Geri alma eylemi | Süre |
|---|---|---|---|
| 1 | Sürüm upgrade sonrası derleme hatası | pin'i eski sürüme döndür | 5 dk |
| 2 | Designer.cs ezen commit | `git checkout -- *.Designer.cs` | 2 dk |
| 3 | Ribbon stili değişimi bozuldu | Office2010'a dön | 5 dk |
| 4 | İkinci TFM'de kırılma | #if ile izole et | 20 dk |
| 5 | Lisans/Runtime çökme | önceki paket setine dön | 15 dk |

> Geri alma her zaman **yalnız-oku komutlarla (§5.3) doğrulanır**; yazma işlemi G14 dışıdır ve commit öncesi review gerektirir.

## §6 — Doğrulama ve Testler

### §6.1 — Kontrol Listesi (18 madde — silinemez)

- [ ] Frontmatter alanları eksiksiz (SSOT)
- [ ] H1 + **Zorunlu Bağlantılar** satırı var
- [ ] §1–§7 iskeleti eksiksiz, silinmedi
- [ ] §3.10 örneği olduğu gibi korundu
- [ ] Karar tek cümlede (§3.1)
- [ ] En az 2 alternatif + elenme gerekçesi (§3.2)
- [ ] Kabul kriterleri ölçülebilir (§3.4)
- [ ] Kapsam/kapsam dışı tablosu (§3.5)
- [ ] Geri alma yazıldı (§3.8)
- [ ] G1–G14 tam, "İhlal Sonucu" sütunlu (§4.5)
- [ ] 11 uygulama adımı (§5.1)
- [ ] ≥8 hata modu (§5.2)
- [ ] 6 yalnız-oku PowerShell komutu + "olmalı" yorumu (§5.3)
- [ ] 18 madde işaretli (§6.1)
- [ ] Kanıtsız iddia `⚠️` li (§1.2/§6.3)
- [ ] [ADR Dizini](../../.decisions/index.md)'ne işlendi
- [ ] Supersede zinciri güncellendi (G13)
- [ ] Mojibake yok (Türkçe diacritikler doğru)

### §6.2 — Test stratejisi

| Tip | Kapsam | Araç |
|---|---|---|
| Unit | UI mantığı (varsa) | xunit |
| Build | net48 + net10.0-windows | dotnet build |
| Manual | 7 ekran + Ribbon | manuel test |
| Coverage | %90 | coverage raporu |

### §6.3 — Kanıt tablosu

| Kanıt | Konum | Durum |
|---|---|---|
| [kanıt] | [dosya/çıkıntı] | ⚠️ VERIFICATION REQUIRED |

### §6.4 — Doğrulama komutları

Aynı [§5.3](#§53--doğrulama-komutları-yalnız-oku) komutları + `dotnet build` çıktısı.

### §6.5 — Şablona özgü ek kontroller

1. **Sürüm pin:** DevExpress paketleri floating değil, 25.2.8 sabit. → `Select-String csproj`
2. **Designer bütünlüğü:** `Designer.cs`/`.resx` diff'i istenmeyen değişikliğe karşı. → `git diff --stat`
3. **Ribbon stili:** `RibbonControlStyle.Office2010`. → `Select-String *.cs`
4. **İki TFM:** `net48` ve `net10.0-windows` ikisi de derleniyor. → `dotnet build`
5. **UI katmanı izolasyonu:** DevExpress yalnız `UI` katmanında; `Domain`/ `Application` temiz. → `Select-String` katman taraması
6. **7 ekran:** her ekran açılıyor, Ribbon butonları çalışmanuel test.

---


### §6.6 — Sık yapılan hatalar (SSS)

| # | Soru | Cevap |
|---|---|---|
| 1 | Tasarımcıyı açıp kaydedebilir miyim? | Hayır — G9; değişiklik el yazımı olmalı |
| 2 | Sürüm otomatik güncellensin mi? | Hayır — pin (G4) |
| 3 | Ribbon stilini değiştirebilir miyim? | Yeni ADR gerekir (G8) |
| 4 | CLI'a DevExpress eklenebilir mi? | Hayır — kapsam dışı |
| 5 | net48 API'si net10'da yoksa? | #if / iki hedef stratejisi |
| 6 | Kapsam %90 altına düşerse? | UI testi eklenir (G10) |

## §7 — Sonuç ve Red Team

### §7.1 — Sonuç özeti

| Alan | Sonuç |
|---|---|
| Karar | [tek cümle] |
| Durum | [accepted/draft/rejected/proposed] |
| Versiyon | [x.y.z] |
| Tarih | [YYYY-MM-DD] |
| Sahip | [rol] |
| İlgili ADR'ler | [bağlantılar] |
| Etkilenen G'ler | [G noları] |
| Sonraki adım | [adım + tarih] |

### §7.2 — Red Team (Bu Karar Nasıl Yanlış Gider?) — 10+ satır

| # | İtiraz | Senaryo | Savunma |
|---|---|---|---|
| 1 | Sürüm çok eski | 25.2.8 patch'siz kalır | upgrade = yeni ADR |
| 2 | Designer.cs el yazımı sürdürülemez | merge çakışması | dosya sahipliği |
| 3 | İki TFM bakım maliyeti | #if çoğalır | TFM matrisi |
| 4 | Ribbon eski görünür | Office2010 modası geçer | tema ADR'si |
| 5 | Lisans pahalı | maliyet artar | lisans envanteri |
| 6 | high-DPI bozuk | 4K ekranda kırık | DPI testi |
| 7 | Skia render hatalı | Linux'ta çalışmaz | desteklenen OS listesi |
| 8 | DevExpress'e bağımlılık | vendor lock-in | UI katmanı izolasyonu |
| 9 | CLI tarafı etkilenmez | Desktop çökerse ürün çöker | ekran sağlık testi |
| 10 | Tasarımcı dosyayı ezer | git diff kaybolur | G9 kapısı |
| 11 | Coverage %90 UI'da zor | test reddedilir | kritik UI testleri |

---


### §7.3 — Onay ve dağıtım

| Rol | İsim | Tarih | İmza |
|---|---|---|---|
| Yazar | [rol] | [tarih] | [imza] |
| UI geliştirici | [rol] | [tarih] | [imza] |
| Mimari sorumlu | [rol] | [tarih] | [imza] |
| Reviewer | [rol] | [tarih] | [imza] |

## Ek B — DevExpress Bileşen Rehberi (karar yazarken referans)

### B.1 — Hangi karar hangi bileşeni etkiler

| Karar tipi | Etkilenen bileşen | Tipik slug |
|---|---|---|
| Sürüm sabitleme | tüm paketler | `devexpress-version` |
| Ribbon stili | RibbonControl | `ribbon-style` |
| Grid sanallaştırma | GridControl/GridView | `grid-virtualization` |
| Tema seçimi | OfficeThemeProvider | `ui-theme` |
| high-DPI | tüm kontroller | `high-dpi` |
| Designer dosyaları | Designer.cs/.resx | `designer-files` |
| Lisans | runtime | `licensing` |

### B.2 — Ribbon stili seçenekleri (yalnız-oku denetimi)

| Stil | Durum | Denetim |
|---|---|---|
| Office2010 | varsayılan / sabit | `Select-String -Pattern RibbonControlStyle` |
| Office2007 | eski | RED (G8) |
| OfficeUniversal | eski | RED (G8) |
| Office2013+ | değerlendirilir | yeni ADR gerekir |

### B.3 — Grid sanallaştırma karar kriterleri

| Kriter | Eşik | Aksiyon |
|---|---|---|
| Satır sayısı | < 1.000 | normal binding |
| Satır sayısı | ≥ 1.000 | sanallaştırma değerlendir |
| Uzak veri | varsa | server-mode değerlendir |
| Kapsam etkisi | %90 altı | test ekle (G10) |

> Eşikler VersaCoder plan varsayımıdır; ⚠️ VERIFICATION REQUIRED — ölçümle doğrulanır.

### B.4 — Designer.cs el yazımı kuralları

1. Dosyayı tasarımcıda açma; otomatik üretim yok — el ile düzenlemek zorundasın.
2. Her kontrol kaydı `InitializeComponent` içinde kalır.
3. `.resx` anahtarları `ResourceManager` ile eşleşir.
4. Diff PR'da gözden geçirilir (G7).
5. Yeni ekran: kopyala-yapıştır + isim değiştir; üretici kullanma (G9).
6. Dispose zinciri korunur.

### B.5 — high-DPI ve tema kontrol tablosu

| Kontrol | Beklenen | Doğrulama |
|---|---|---|
| DPI-aware manifest | var | manuel 100%/150% testi |
| Font ölçeği | tutarlı | ekran görüntüsü |
| Tema | Office2010 varsayılan | görsel kontrol |
| Z-order | panel çakışması yok | manuel |
| Resize | pencere küçültünce bozulmaz | manuel |

### B.6 — Lisans ve runtime notları

| Risk | Belirti | Aksiyon |
|---|---|---|
| Lisans uyarısı | açılış dialogu | lisans doğrula, ADR-`licensing` |
| Skia render | kontrol görünmüyor | render ayarı / sürüm kontrolü |
| Paket eksik | `TypeLoadException` | csproj paket listesi denetimi |
| TFM uyuşmazlığı | `net48` API'si yok | #if stratejisi |

---

## Ek A — Hızlı Başlangıç (yeni DevExpress ADR'sı için)

1. §1 kapsamını kontrol et; kopyala.
2. §2.1/§2.2 gerçek envanterle doldur.
3. §3.1–§3.9 ve §3.11'i kararına göre doldur; **§3.10'u değiştirme**.
4. §4.5 G4/G7/G8/G9 satırlarını kararına göre oku; ihlal sonucu RED kalır.
5. §5.1–§5.3, §6, §7'yi uygula → [ADR Dizini](../../.decisions/index.md) → [ADR Şablonu](adr-template.md) → [ADR Dizin Şablonu](adr-index.md).

**Zorunlu Bağlantılar (tekrar):** [Vault Ana Sayfa](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [ADR Dizini](../../.decisions/index.md) · [ADR Şablonu](adr-template.md) · [ADR Dizin Şablonu](adr-index.md)
