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
| Slug dili | İngilizce, tireli | [rules](../../.rules/) |
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
| 4 | GATE-004a..006 kapıları ADR'lerle ilişkili | ✅ WORKFLOW §0 doğruladı 2026-10-04 |
| 5 | Frontmatter 7 alan, `authority: SSOT` | [rules](../../.rules/) |
| 6 | Slug'lar İngilizce, içerik Türkçe | [rules](../../.rules/) |
| 7 | Test: xunit, %90 coverage | [rules](../../.rules/) |
| 8 | Vault kök: index.md, architecture/index.md, keys.md | [index](../../index.md) |

### §1.6 — Sıfırdan ADR yazma akışı (9 adım)

1. **Kapı:** `../batch-manifest-gate004a.md` içinde bu dosyanın satırı var mı ve GATE durumu `onaylandı` mı gör; `beklemede` ise doldurmaya başlama (AGENTS.md G8 kapısı).
2. **Şablon seç:** standart karar → `adr-template.md`; Nygard 14 alan dönüşümü → `adr-nygard-template.md`; DevExpress bileşen/libs kararı → `adr-devexpress-template.md`.
3. **Örnek oku:** §3.10'daki ADR-007 (agent-catalog-reconciliation) **referanstır, kopya değildir** — kopyalarsan §3.10 başlığındaki uyarıyı ihlal edersin.
4. **Numara tahsis et:** §2.5 serisindeki bir sonraki **boş** No'yu al; elde boşluk varsa atlanan No'yu değil, gerçekten boş olanı kullan (CF-13).
5. **Dosya adı:** `.ai/.decisions/<no>-<slug>.md` — baştaki sayı, frontmatter'daki `No` ile birebir aynı olur (`005-devextreme-lisans.md` ↔ `No: 005`).
6. **Frontmatter:** 8 alan byte-byte: `title,type,category,status,version,updated,authority,scope`. Sekizinci alan `scope`'tur; `part` alanı envanter sütunudur, bu dosyada geçerli değildir (CF-03).
7. **Nygard 14 alanını** doldur; `{{...}}` yalnız hâlâ bilinmeyen **tek bir değer** için kalabilir (K15: doldurulmuş/açıklamalı satır yazılacak).
8. **Doğrula:** §5.3 komutları 1–6 + §6.7 komutları 7–12; `git diff --check` temiz değilse commit etme (K3).
9. **Kaydet:** §7.3 onayı bekle; `status: proposed` ile bırak, `accepted`'e GATE-003 sonrası geç.
## §2 — Bağlam (Context)

### §2.1 — Karar ortamı

| Boyut | VersaCoder Gerçeği | Kaynak |
|---|---|---|
| Dizin yolu | `.ai/.decisions/index.md` | ⚠️ VERIFICATION REQUIRED |
| Klasörler | `accepted/`, `draft/`, `rejected/` | ⚠️ VERIFICATION REQUIRED |
| Dosya adı formatı | `ADR-NNN-<english-slug>.md` | ⚠️ VERIFICATION REQUIRED |
| Numara planı | ADR-001..008 (+010 örneği) | plan belgesi ⚠️ |
| Frontmatter | `title,type,category,status,version,updated,authority` | [rules](../../.rules/) |
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

### §2.5 — Numara tahsisi: seri tablosu ve durum geçişi

**Tek doğruluk kaynağı:** dizin satırındaki `No:` alanı ve `.ai/.decisions/` altındaki dosya adıdır (CF-13).

| Kural | Uygulama | Sahip |
|---|---|---|
| Sıra korunur | `No` küçükten büyüğe gider; `001` atlanmaz, geriye sayı yazılmaz | defter |
| Boşluk gizlenmez | ADR silinirse No iade edilmez; yeni karar **bir sonraki gerçekten boş** No'yu alır | §3.13 |
| slug = No | Dosya adındaki sayı = `frontmatter` = dizin satırındaki `No` (üçü aynı) | §6.7-11 |
| Aralığa bakma | README'deki "001…030" **hedef** aralıktır; bugünkü seri diskteki dosyalarla sınırlıdır | CF-13 |

Durum geçişleri tek yönlüdür ve her geçişin kanıtı vardır:

```
proposed ──GATE-003 onayı──▶ accepted ──yeni ADR ile──▶ superseded
   │                           │
   │                           ├──▶ deprecated   (bir özelliğin kalkması)
   └──▶ rejected               └──▶ (kalır; en az bir R-nedeni ile birlikte yazılır)
```

- `proposed → accepted`: **yalnız** GATE-003 onayı ile; onaysız `accepted` yazan şablon kullanılamaz.
- `accepted → superseded`: yeni ADR §3.13'te `Yerine gelen:` satırını yazmadan eski dosyanın `status`'ü oynatılmaz (çift yönlü supersede yasak).
- `rejected`: §6.8 sözlüğünden en az bir `R` nedeni ile birlikte yazılır; yalnız `rejected` etiketi geçersizdir.
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
`.ai/.decisions/index.md` + katalog dosyaları + ilgili [rules](../../.rules/) satırları.
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

### §3.14 — Numara / yol / CF guardrail tablosu (bu dosyanın çelişki günlüğü)

| # | Tuşak | Bu dosyada kural | İlgili CF |
|---|---|---|---|
| 1 | Yol çatışması | ADR yolu: `.ai/.decisions/`. `.ai/adr/` veya `.ai/.decisions/accepted/` yazan her metin **düzeltilecek belgedir**; kaynağa değil | CF-13 |
| 2 | Aralık çatışması | ADR no aralığı diskteki dosyalarla sınırlıdır; "001…030" hedef aralıktır | CF-13 |
| 3 | Frontmatter | 8 alan: `title,type,category,status,version,updated,authority,scope`; `part` frontmatter alanıdır **değil** | CF-03 |
| 4 | K15 aralığı | 26–44 KB · 520–680 satır (26–38 ve 480–660 yazan her satır eski aralıktır) | CF-06 |
| 5 | `.ai/` disk | `.ai/` altında yalnız `.templates/`, `prompts/`, `reports/` vardır; `.rules/`, `.decisions/`, kök `index.md`/`CLAUDE.md`/`keys.md` hedeflerine **yeni link yazılmaz** | CF-08/CF-09/CF-15 |
| 6 | Boşluk-çift tire | slug'da `--` yasak: `005--lisans` değil `005-devextreme-lisans` | CF-13 |
| 7 | SSOT çoğalması | Kural metni yalnız `.ai/.templates/index.md` + ilgili şablonda yaşar; ikinci kopya yazılmaz | CF-15 |

> **Uygulama:** bir belge bu tabloya aykırıysa ADR yine yazılır ama `status: proposed` kalır; düzeltme ayrı iş kalemidir — ADR, belge tamiratının yerine geçmez.

Kaynak: [Çakışma Kaydı 001](../../reports/conflict-register-001-master-prompts.md).
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

### §4.6 — G1..G12 ↔ bu defterde karşılığı (kim neyi kapatır)

| Guardrail | Karşılığı | Kapatma kanıtı |
|---|---|---|
| G1 · Zorunlu Bağlantılar | satır 14 + §7.5 (bu dosya) | `Select-String "Zorunlu Bağlantılar"` = 2 satır |
| G2 · 7 bölüm silinemez | `## §1`…`## §7` | `Select-String -Pattern "^## §"` = 7 satır |
| G3 · Türkçe + UTF-8 | dosyanın tamamı | mojibake = 0 |
| G4 · ASCII diyagram | §7.4 zincir çizimi | `../architecture/ascii-diagram-template.md` ile uyum |
| G5 · frontmatter 8 alan | §2.5 + §3.14-3 | 8 anahtar |
| G6 · disk kanıtı zorunlu | §5.3 + §6.7 | her iddia için komut çıktısı |
| G7 · link hedefi diskte | §3.14-5 | kırık link = 0 |
| G8 · GATE-003 kapısı | §1.6 adım 1 + §2.5 | `onaylandı` satırı |
| G9 · ADR yolu/no | §2.5 + §3.14-1 | `.ai/.decisions/` + slug = No |
| G10 · mojibake | §6.7-9 | `Ã` / `â€` / `�` = 0 |
| G11 · TreatWarningsAsErrors + %90 coverage | §6.7-12 | coverage raporu `≥ 90` (SSUT: `.ai/CLAUDE.md` + `index.md`) |
| G12 · tek SSOT | §3.14-7 | bu dosya + `index.md` dışında kural kopyası yok |
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

### §6.7 — Doğrulama komutları 7–12 (dosya sonu / K15 katmanı)

> Hepsi **yalnız okur**; hiçbiri dosya/DB yazmaz (yalnız-oku, G6).

````powershell
# 7 — K15 + K3 (bu üç şablon; 520–680 satır · 26–44 KB)
$f='adr-index.md','adr-devexpress-template.md','test-plan-template.md'
foreach($n in $f){
  $p=(Get-ChildItem .ai\.templates -Recurse -Filter $n | Select-Object -First 1).FullName
  $l=(Get-Content $p).Count; $kb=[math]::Round((Get-Item $p).Length/1KB,1)
  "{0} | {1} satır | {2} KB | K3={3} | K15={4}" -f $n,$l,$kb,
    $(if($l -ge 520 -and $l -le 680){'OK'}else{'DUS'}),
    $(if($kb -ge 26 -and $kb -le 44){'OK'}else{'DUS'}) }

# 8 — BOM (üçü de YOK olmalı)
foreach($n in $f){
  $p=(Get-ChildItem .ai\.templates -Recurse -Filter $n | Select-Object -First 1).FullName
  $b=[IO.File]::ReadAllBytes($p)
  "{0} BOM={1}" -f $n,$(if($b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF){'VAR'}else{'YOK'}) }

# 9 — mojibake (0 olmalı)
(Select-String -Path .ai\.templates\**\*.md -Pattern 'Ã|â€|�' -EA 0 | Measure-Object).Count

# 10 — yeni link hedefleri diskte mi (kırık = 0; G7)
Select-String -Path .ai\.templates\adr\adr-index.md -Pattern '\]\(([^)]+)\)' -EA 0 |
  ForEach-Object { $_.Matches[0].Groups[1].Value } | Sort-Object -Unique

# 11 — slug = No (CF-13)
Get-ChildItem .ai\.decisions -Filter *.md | ForEach-Object Name

# 12 — silinen satır (numstat 2. sütun = 0 olmalı)
git diff --numstat -- .ai/.templates/adr/adr-index.md
````

**Kabul:** 7 = 3/3 `OK` · 8 = `YOK` · 9 = `0` · 12 = silinen `0`. Dördü birden sağlanmadan §7.3 onayı verilmez.

### §6.8 — Red Team sözlüğü (R-neden kodları — `rejected` yazarken zorunlu)

| Kod | Red nedeni | Tipik tetikleyici |
|---|---|---|
| R1 | K15 ihlali | 26 KB altı / 44 KB üstü / 520–680 dışı |
| R2 | Silme tespiti | `git diff --numstat` 2. sütun ≠ 0 |
| R3 | BOM / EOL | UTF-8 BOM var ya da LF↔CRLF karışık |
| R4 | Mojibake | `Ã`, `â€` veya `�` |
| R5 | Ölü link | `]()` hedefi diskte yok (G7) |
| R6 | Kapı ihlali | GATE-003 `onaylandı` değilken dosyaya satır yazılmış |
| R7 | SSOT ihlali | kural metni ikinci yerde kopyalanmış (CF-15) |
| R8 | Frontmatter | 8 alan değil ya da `updated` oynatılmış (CF-03) |
| R9 | Uydurma veri | `{{...}}` dışına yazılmış ama doğrulanamayan iddia |
| R10 | Çelişki saklama | CF satırı varsa §3.14 tablosuna işlenmemiş |
| R11 | Şablon ihlali | `## §` başlığı eklendi/çıkarıldı (7 bölüm bozuldu) |
| R12 | Doldurulmuş satır | anlamsız/sahte tamamlama, tekrar eden cümle |

> Her `rejected` kaydı en az bir kod taşır; kod içermeyen red gerekçesi red-team'ten geri döner.
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
| ADR-NNN | [başlık](accepted/ADR-NNN-slug.md) | slug | status | YYYY-MM-DD | superseded-by-ADR-MMM / — |
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

## Ek E — CF çapraz indeksi (bu şablon nereye işledi?)

| CF | Özet | İşlendiği yer |
|---|---|---|
| CF-03 | Frontmatter 8. alan = `scope` (`part` değil) | §2.5 kural 6 · §3.14-3 |
| CF-05 | Satır/kb alt sınır | §6.7-7 (K3/K15 ölçer) |
| CF-06 | K15 26–38 → **26–44** | §3.14-4 · §6.7-7 |
| CF-08 | `.rules/` diskte yok → yeni link yasağı | §3.14-5 |
| CF-09 | `.ai/` disk sayımı ("14/2/2" beyanı yanlış) | §3.14-5 |
| CF-13 | ADR yolu `.ai/.decisions/` + no aralığı | §2.5 · §3.14-1/2/6 |
| CF-15 | Kök bağlantıların 5'i diskte yok → yeni link yasağı + sözlük | §3.14-5 · §6.8 |

> CF-16 (Part KB) bu dosyada değil, envanter raporunda işlenir.
**Zorunlu Bağlantılar (tekrar):** [Vault Ana Sayfa](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [ADR Dizini](../../.decisions/index.md) · [ADR Şablonu](adr-template.md) · [Nygard Şablonu](adr-nygard-template.md)
