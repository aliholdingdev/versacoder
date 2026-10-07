---
title: "ADR Dizin Şablonu — ADR Kayıt Defteri (index.md) ve Numaralandırma"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "ADR kayit defteri sablonu: index.md satir bicimi, numaralandirma ve durum sutunlari"
---

# ADR Dizin Şablonu — ADR Kayıt Defteri

**Zorunlu Bağlantılar:** [Vault Ana Sayfa](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [ADR Dizini](../../.decisions/index.md) · [Genel ADR Şablonu](adr-template.md) · [Nygard Şablonu](adr-nygard-template.md)

---

## §1 — Bu Şablonun Kapsamı ve Kullanımı

### §1.1 — Ne zaman kullanılır

1. Vault'ta yeni bir ADR açılırken (dizin satırı).
2. Bir ADR status değiştirdiğinde (accepted / draft / rejected / superseded).
3. Bir ADR başka bir ADR'yi ezerse (supersede zinciri).
4. ADR numaralandırma (ADR-NNN) yürütülürken.
5. Dizin dosyası `.decisions/index.md` yeniden yazılırken.
6. Dizin ile klasör yapısı (accepted/draft/rejected) arasında tutarlılık sağlanırken.

### §1.2 — Bu şablonu kopyalamadan önce

| Kontrol | Beklenen | Durum |
|---|---|---|
| Dizin dosyası | `.ai/.decisions/index.md` | ⚠️ VERIFICATION REQUIRED |
| ADR dosyaları | `.ai/.decisions/accepted|draft|rejected/` | ⚠️ VERIFICATION REQUIRED |
| Numara aralığı | ADR-001..ADR-008 planı (+ ADR-010 örneği) | ⚠️ VERIFICATION REQUIRED |
| Slug dili | İngilizce, tireli | [rules](../../.rules) |
| Supersede kuralı | G13 zinciri zorunlu | §4.5 |

> **Not:** Dizin, tüm ADR'lerin tek doğruluk kaynağıdır (SSOT); ADR dosyaları kendi durumunu yazsa bile **dizin satırı geçerlidir**.

### §1.3 — Şablonun 7 bölümlük zorunlu iskeleti (silinemez)

| Bölüm | Ad | Dizin şablonunda zorunlu içerik |
|---|---|---|
| §1 | Kapsam ve Kullanım | dizin ne işe yarar + SSOT kuralı |
| §2 | Bağlam (Context) | ADR yaşam döngüsü ve klasör yapısı |
| §3 | Karar (Decision) | 11 alt bölüm; §3.10 = dolu ADR-007 örneği |
| §4 | Korkular ve Guardrail'lar | G1–G14; dizine özgü G12/G13 satırları |
| §5 | Uygulama ve Hata Modları | dizin güncelleme 11 adımı + hata modları + 6 komut |
| §6 | Doğrulama ve Testler | 18 madde + dizin tutarlılık testleri |
| §7 | Sonuç ve Red Team | Red Team 10+ satır defter bakışıyla |

### §1.4 — Bu şablonla yazılan tipik dosyalar

`.decisions/index.md` (ana defter), `.decisions/README.md` (klasör açıklaması), ADR durum geçiş notları — İngilizce slug'lar, Türkçe açıklama.

---


### §1.5 — Bu şablonun dayandığı VersaCoder gerçekleri

| # | Gerçek | Kaynak |
|---|---|---|
| 1 | Dizin: `.ai/.decisions/index.md` (SSOT) | ⚠️ VERIFICATION REQUIRED |
| 2 | Klasörler: `accepted/`, `draft/`, `rejected/` | ⚠️ VERIFICATION REQUIRED |
| 3 | ADR planı: ADR-001..ADR-008 (+ ADR-010 örneği) | plan belgesi ⚠️ |
| 4 | GATE-000..005 kapıları ADR'lerle ilişkili | ⚠️ VERIFICATION REQUIRED |
| 5 | Frontmatter 7 alan, `authority: SSOT` | [rules](../../.rules) |
| 6 | Slug'lar İngilizce, içerik Türkçe | [rules](../../.rules) |
| 7 | Test: xunit, %90 coverage | [rules](../../.rules) |
| 8 | Vault kök: index.md, architecture/index.md, keys.md | [index](../../index.md) |

## §2 — Bağlam (Context)

### §2.1 — Karar ortamı

| Boyut | VersaCoder Gerçeği | Kaynak |
|---|---|---|
| Dizin yolu | `.ai/.decisions/index.md` | ⚠️ VERIFICATION REQUIRED |
| Klasörler | `accepted/`, `draft/`, `rejected/` | ⚠️ VERIFICATION REQUIRED |
| Dosya adı formatı | `ADR-NNN-<english-slug>.md` | ⚠️ VERIFICATION REQUIRED |
| Numara planı | ADR-001..008 (+010 örneği) | plan belgesi ⚠️ |
| Frontmatter | `title,type,category,status,version,updated,authority` | [rules](../../.rules) |
| Supersede | eski ADR `superseded` + yeni ADR linki | G13 |

### §2.2 — Kararı tetikleyen tetikleyiciler

1. ADR sayısı artınca tek dosyadan takip zorlaşır.
2. Status değişiklikleri dosya içi yazım unutulursa defter bayatlar.
3. Supersede zinciri koparsa "en güncel karar" bilinemez olur.
4. Numara çakışması (iki ADR aynı ADR-NNN) dosyaları birbirine iter.
5. Klasör-dizin tutarsızlığı (dosya `draft/` ama dizinde `accepted`) SSOT'u bozar.

---


### §2.3 — Beklenen dizin görünümü (örnek)

```markdown
# Kararlar (ADR) — SSOT
| Numara | Başlık | Slug | Status | Tarih | Supersede |
|---|---|---|---|---|---|
| ADR-001 | DevExpress sürümü | devexpress-version | accepted | ... | — |
| ADR-002 | Çözüm katmanı kapsamı | solution-tier-scope | accepted | ... | — |
| ADR-003 | Dual-target net48+net10 | dual-target | accepted | ... | — |
| ADR-004 | Veri yığını | data-stack | accepted | ... | — |
| ADR-007 | Ajan kataloğu reconcile | agent-catalog-reconciliation | accepted | ... | — |
| ADR-010 | LLM sağlayıcı secret akışı | llm-provider-secret-flow | draft | ... | — |
```

> Satırlar numara artan sıralanır; status ve tarih ADR frontmatter'ıyla eşleşir (G1/G12).

### §2.4 — Kapsam dışı açıkça şunlardır

1. ADR'lerin içeriği (bak: [ADR Şablonu](adr-template.md)).
2. `.ai/.rules/` kural dosyaları (ayrı SSOT).
3. [ARCHITECTURE](../../architecture/index.md) mimari anlatımı.
4. Kod içi karar yorumları (ADR'ye link verir, defter değildir).

## §3 — Karar (Decision)

> **Şablon Kuralı:** §3.1–§3.9 + §3.11–§3.13 doldurulur; **§3.10 doldurulmaz, olduğu gibi korunur** (örnek referanstır).

### §3.1 — Karar ifadesi (tek cümle)

**[Dizin/format] için [kural] uygularız; [istisna] yalnız [prosedür] ile yapılır.**

### §3.2 — Alternatifler

| # | Alternatif | Neden elendi |
|---|---|---|
| A1 | Dizin olmadan (dosyalar tek kaynak) | status/supersede takibi dağılır |
| A2 | Otomatik üretilen dizin (script) | el yazımı notlar kaybolur |
| A3 | ADR'leri wiki-link'lemek yerine gömme | kırılgan, diff gürültülü |
| A4 | Mevcut karar (buraya yaz) | — |

### §3.3 — Karar gerekçesi

### §3.4 — Kabul kriterleri (ölçülebilir)

1. **KR1:** Dizindeki satır sayısı = diskteki ADR dosyası sayısı. → Kanıt: §5.3 komutu 1
2. **KR2:** Her satırda status + tarih + supersede (varsa) mevcut. → Kanıt: dizin diff'i
3. **KR3:** Numaralar benzersiz ve artan. → Kanıt: komut 2
4. **KR4:** Klasör ile status uyumlu. → Kanıt: komut 3

### §3.5 — Kapsam ve kapsam dışı

| Kapsam | Kapsam dışı |
|---|---|
| `.ai/.decisions/` altındaki tüm ADR'ler | ADR içeriği şablonları (ayrı dosyalar) |
| `index.md` defteri | [ARCHITECTURE](../../architecture/index.md) |
| Durum/supersede kuralları | Kod içi yorumlar |

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

### §3.10 — ÖRNEK REFERANS: ADR-007 (agent-catalog-reconciliation) — KOPYALANACAKSIN, DEĞİŞTİRİLMEYECEK

> Aşağıdaki blok dolu bir VersaCoder ADR örneğidir. Yeni ADR yazarken bu bloğun **yapısını** kullan; **içeriğini** kendi kararına göre yeniden yaz. Blok silinemez.

#### §3.10.1 — Karar cümlesi
**Ajan kataloğu (agent catalog), depodaki gerçek ajan/envanter dosyalarıyla kural olarak her ADR kabulünde veya katalog değişiminde reconcile edilir.**
#### §3.10.2 — Alternatifler
| # | Alternatif | Neden elendi |
|---|---|---|
| A1 | Katalog elle, ara sıra güncelleme | bayatlar, G12 ihlali |
| A2 | Tam otomatik üretim (script tek kaynak) | yorum/sahiplik kaybolur |
| A3 | Katalog yok, dosyalar taranır | tek bakışta görünüm kaybolur |
| A4 | ADR kabulü + katalog reconcile (seçildi) | — |

#### §3.10.3 — Gerekçe
[ADR Dizini](../../.decisions/index.md) SSOT'tur; katalog ise ADR'lerin etkilediği envanterin (ajanlar, ekranlar, kurallar) yansımasıdır. ADR kabul edildiğinde yansıma güncellenmezse vault içinde iki farklı gerçek oluşur.
#### §3.10.4 — Kabul kriterleri
| # | Kriter | Ölçüt | Kanıt |
|---|---|---|---|
| KR1 | Reconcile tetiklenir | her accepted ADR | dizin + katalog diff'i |
| KR2 | Katalog eşleşir | 0 fark | komut 4 |
| KR3 | Supersede zinciri | kopuk yok | komut 5 |
| KR4 | Status tutarlı | klasör=status | komut 3 |

#### §3.10.5 — Kapsam
`.ai/.decisions/index.md` + katalog dosyaları + ilgili [rules](../../.rules) satırları.
#### §3.10.6 — Kapsam dışı
Kod içi ajan implementasyonları; CLI davranışları.
#### §3.10.7 — Bağımlılıklar
G12 (dizin defteri), G13 (supersede), ADR frontmatter SSOT (G1).
#### §3.10.8 — Riskler
| Risk | Olasılık | Etki | Azaltıcı |
|---|---|---|---|
| Katalog bayatlar | orta | yüksek | her accepted ADR'de reconcile |
| Çift kayıt | düşük | orta | komut 3 kontrolü |
| Supersede kopuğu | düşük | yüksek | komut 5 |
| Slug çakışması | düşük | orta | komut 2 |

#### §3.10.9 — Geri alma
Katalog diff'i geri alınır (`git checkout --`); dizin satırı status'e döndürülür; reversible.
#### §3.10.10 — Karar veren / danışılan
Karar veren: vault sorumlusu · Danışılan: mimari sorumlu · Uygulayan: [rol] · Reviewer: kod inceleyen.
#### §3.10.11 — İlgili G'ler
G1 (frontmatter), G12 (dizin), G13 (supersede), G14 (doğrulama).

### §3.11 — Kararla ilgili açık sorular

| # | Soru | Sorumlu | Son tarih |
|---|---|---|---|
| Q1 | [soru] | [rol] | [tarih] |
| Q2 | [soru] | [rol] | [tarih] |

### §3.12 — Karar sonrası izleme metrikleri

| Metrik | Hedef | Sıklık | Kaynak |
|---|---|---|---|
| Dizin ↔ disk farkı | 0 | her PR | komut 1 |
| Numara çakışması | 0 | her PR | komut 2 |
| Klasör/status uyumsuzluğu | 0 | her PR | komut 3 |
| Kopuk supersede | 0 | her PR | komut 5 |
| Bayat tarih (> 90 gün status'süz) | 0 | aylık | manuel |

### §3.13 — Karar geçmişi (supersede)

| ADR | Durum | Not |
|---|---|---|
| [önceki karar varsa] | superseded by [bu ADR] | G13 zinciri |
| [bu ADR] | accepted | — |

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
| G4 | Dizin satırı olmayan ADR | RED |
| G5 | Kanıtsız iddia | `⚠️` zorunlu |
| G6 | Eski vault'tan kopyalama | RED |
| G7 | Numara çakışması (ADR-NNN çift) | RED |
| G8 | Slug tekrarı / Türkçe slug | RED |
| G9 | Supersede zinciri kopuk | RED |
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
| 1 | Diskteki ADR dosyalarını listele | dosya listesi |
| 2 | Mevcut [dizini](../../.decisions/index.md) oku | mevcut satırlar |
| 3 | Yeni/en büyük ADR-NNN numarasını bul | numara |
| 4 | Slug'u İngilizce+tireli yaz, tekrarı denetle | slug |
| 5 | Dizin satırını hazırla (status, tarih, supersede) | satır |
| 6 | Dizini güncelle (SSOT) | index.md diff |
| 7 | Klasörle eşleştir (accepted/draft/rejected) | klasör diff |
| 8 | Supersede zincirini güncelle (eski ADR'yi işaretle) | zincir |
| 9 | §5.3 komutlarını çalıştır | doğrulama çıktısı |
| 10 | Guardrail G1–G14 denetimi | §4.5 işaretleri |
| 11 | PR'da katalog/reconcile notu | PR notu |

### §5.2 — Hata Modları

| # | Hata Modu | Belirti | Düzeltme |
|---|---||---|
| 1 | Dizin unutuldu | diskte dosya, dizinde yok | satır ekle |
| 2 | Numara çakışması | iki dosya ADR-00N | yeniden numaralandır |
| 3 | Status tutarsız | klasör ≠ status | klasörü/satırı eşleştir |
| 4 | Supersede kopuk | eski ADR "accepted" | `superseded` yap + link |
| 5 | Slug çakışması | iki dosya aynı slug | slug'ı ayır |
| 6 | Türkçe slug | `karar-01` | İngilizceye çevir |
| 7 | Bayat tarih | `updated` güncellenmemiş | tarih güncelle |
| 8 | Çift defter | `README` vs `index` farklı | tek SSOT: index.md |

### §5.3 — Doğrulama Komutları (yalnız-oku)

```powershell
# 1 — diskteki ADR sayısını gör (dizinle eşit olmalı)
(Get-ChildItem .ai\decisions -Recurse -Filter ADR-*.md).Count
# 2 — ADR numaralarını gör (benzersiz olmalı)
Select-String -Path .ai\decisions\*.md -Pattern '^## ADR-' | ForEach-Object Line
# 3 — status satırlarını gör (klasörle eşit olmalı)
Select-String -Path .ai\decisions\index.md -Pattern 'status:'
# 4 — supersede işaretlerini gör (kopuk olmamalı)
Select-String -Path .ai\decisions\*.md -Pattern 'superseded'
# 5 — frontmatter alanlarını gör (7 alan eksiksiz olmalı)
Select-String -Path .ai\decisions\*.md -Pattern 'authority: SSOT'
# 6 — index.md dosyalarını gör (tek SSOT olmalı)
Get-ChildItem .ai\decisions -Recurse -Filter *.md
```

---

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
- [ ] Kanıtsız iddia `⚠️` li (§6.3)
- [ ] Dizin ↔ disk tutarlı (komut 1)
- [ ] Supersede zinciri güncellendi (G13)
- [ ] Mojibake yok (Türkçe diacritikler doğru)

### §6.2 — Test stratejisi

| Tip | Kapsam | Araç |
|---|---|---|
| Consistency | dizin ↔ disk sayımı | §5.3 komut 1 |
| Uniqueness | ADR-NNN benzersizliği | komut 2 |
| Status | klasör ↔ status | komut 3 |
| Chain | supersede zinciri | komut 4 |
| Build | vault derleme bütünlüğü | dotnet build |
| Coverage | %90 | coverage raporu |

### §6.3 — Kanıt tablosu

| Kanıt | Konum | Durum |
|---|---|---|
| [kanıt] | [dosya/çıkıntı] | ⚠️ VERIFICATION REQUIRED |

### §6.4 — Doğrulama komutları

Aynı [§5.3](#§53--doğrulama-komutları-yalnız-oku) komutları + `git status` çıktısı.

### §6.5 — Şablona özgü ek kontroller

1. **Tek SSOT:** `.decisions/index.md` dışında ikinci defter yok. → komut 6
2. **Numara artan:** en büyük numara +1 ile yeni ADR. → komut 2
3. **Slug İngilizce:** Türkçe karakter/slug yok. → regex tarama
4. **Status enum:** `accepted|draft|rejected|superseded` dışına çıkış yok. → komut 3
5. **Supersede dolu:** `superseded` her satırda hedef ADR linki var. → komut 4
6. **Katalog reconcile:** ADR kabulü katalog diff'i üretir (ADR-007 örneği). → §3.10

---


### §6.6 — Sık yapılan hatalar (SSS)

| # | Soru | Cevap |
|---|---|---|
| 1 | ADR'yi dizinsiz açabilir miyim? | Hayır — G4/G12 RED |
| 2 | Aynı slug iki ADR'de olabilir mi? | Hayır — G8 |
| 3 | Status'u ADR içinde yazmak yeterli mi? | Hayır — dizin SSOT (G12) |
| 4 | Eski ADR'yi silelim mi? | Hayır — `superseded` yap (G13) |
| 5 | Reddedilen ADR silinir mi? | Hayır — `rejected/` klasöründe kalır |
| 6 | Numara atlanabilir mi? | Evet, gerekçeli; çakışma asla (G7) |

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
| 1 | Dizin elle güncellenir, unutulur | bayat defter | her PR'da komut 1 |
| 2 | İki kişi aynı numarayı alır | çakışan ADR-NNN | komut 2 + PR kapısı |
| 3 | Supersede elle unutulur | iki "en güncel" karar | G13 + komut 4 |
| 4 | Dizin otomasyona bağlanır | el yazımı notlar kaybolur | otomasyon yalnız öneri |
| 5 | Status enum genişler | okunamaz defter | sabit enum |
| 6 | Klasör/status ayrışır | yanlış yere taşınan dosya | komut 3 |
| 7 | Slug Türkçe yazılır | arama/kırılma | G2/G8 |
| 8 | Dizin çok büyür | tarama yavaşlar | yıllara bölme (gelecek) |
| 9 | ADR dosyası silinir | kırık link | komut 1 fark eder |
| 10 | Çift defter (README vs index) | iki gerçek | tek SSOT kuralı |
| 11 | Bayat tarihler | audit zorlaşır | §3.12 metriği |

---


### §7.3 — Onay ve dağıtım

| Rol | İsim | Tarih | İmza |
|---|---|---|---|
| Yazar | [rol] | [tarih] | [imza] |
| Vault sorumlusu | [rol] | [tarih] | [imza] |
| Mimari sorumlu | [rol] | [tarih] | [imza] |
| Reviewer | [rol] | [tarih] | [imza] |

## Ek D — Supersede Zinciri Formatı

| Durum | Eski ADR'de | Yeni ADR'de | Dizinde |
|---|---|---|---|
| Supersede öncesi | `status: accepted` | — | eski satır `accepted` |
| Supersede anında | `status: superseded` + "yenisi: ADR-MMM" | "ezdiği: ADR-NNN" | eski `superseded`, yeni `accepted` |
| Supersede sonrası | arşiv kalır, silinmez | yürürlükte | iki satır da durur |

Örnek dizin satırları:

```markdown
| ADR-003 | Dual-target | dual-target | superseded | 2026-09-25 | superseded-by-ADR-009 |
| ADR-009 | Dual-target v2 | dual-target-v2 | accepted | 2026-09-25 | — |
```

Zincir kuralları:

1. Zincir tek yönlüdür: `ADR-NNN → ADR-MMM`, geriye link yoktur.
2. Her `superseded` satırda hedef linki vardır (G13); kopuk zincir RED.
3. Silme yoktur; defter büyür, geçmiş kaybolmaz.
4. Yeni ADR, ezdiği ADR'nin açık sorularını (§3.11) devralır.
5. Katalog/reconcile (ADR-007) her supersede sonrasında tekrarlanır.

---


## Ek A — Dizin Satırı Formatı (standart)

```markdown
| ADR-NNN | [başlık](accepted/adr-nnn-slug.md) | slug | status | YYYY-MM-DD | superseded-by-ADR-MMM / — |
```

Kurallar:

1. Sütunlar: numara, link, slug, status, tarih, supersede — sabit sıra.
2. Status enum: `accepted`, `draft`, `rejected`, `superseded`.
3. Supersede doluysa hedef `ADR-MMM` linki zorunlu (G13).
4. Tarih = ADR'nin `updated` frontmatter değeri (G1).
5. Satır sırası: numara artan.

---

## Ek B — ADR Yaşam Döngüsü

| Durum | Klasör | Anlamı | Geçiş |
|---|---|---|---|
| draft | `draft/` | taslak, review altında | → accepted / rejected |
| accepted | `accepted/` | yürürlükte (SSOT) | → superseded |
| rejected | `rejected/` | reddedildi, kayıp için | — |
| superseded | `accepted/` (işaretli) | yenisiyle ezbere | — |

Geçiş kuralları:

1. `draft → accepted`: §6.1 kontrol listesi tam + G1–G14 temiz.
2. `accepted → superseded`: yeni ADR kabulünde eski işaretlenir (G13).
3. `draft → rejected`: gerekçe satırı zorunlu.
4. Her geçiş `index.md` satırına ve ADR frontmatter'ına yazılır.

---

## Ek C — Hızlı Başlangıç (yeni ADR dizin satırı)

1. §5.1 11 adımı uygula.
2. §3.10 bloğunu örnek kabul et; içeriği kendine göre yaz.
3. §5.3 komutlarıyla doğrula (1–6 hepsi "olmalı" yorumlarını karşılasın).
4. §6.1 18 maddeyi işaretle; §6.5 ek kontrolleri çalıştır.
5. [ADR Dizini](../../.decisions/index.md) → [ADR Şablonu](adr-template.md) → [Vault Ana Sayfa](../../index.md).

**Zorunlu Bağlantılar (tekrar):** [Vault Ana Sayfa](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [ADR Dizini](../../.decisions/index.md) · [ADR Şablonu](adr-template.md) · [Nygard Şablonu](adr-nygard-template.md)
