---
title: "Test Plan Template — Test Planı Şablonu (Part D)"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "test plani sablonu: senaryolar, oncelikler, ortam ve kabul kriterleri"
---

# Test Plan Template — Test Planı Şablonu

**Zorunlu Bağlantılar:**
- [index](../../index.md)
- [CLAUDE](../../CLAUDE.md)
- [glossary](../../glossary.md)
- [keys](../../keys.md)
- [VISION](../../VISION.md)

---

## §1 Amaç

### §1.1 Bu Şablon Ne Yapar

Bu şablon, bir modülün/ürünün **test planını** (plan seviyesi, yani tek test
dosyası değil) tek dosyada, denetlenebilir biçimde yazmak içindir. Test
planı; **ne test edilecek, ne test edilmeyecek, kim, hangi araçla, hangi
kabul kriteriyle, hangi riskle** sorularını SSOT yanıtlar.

Üç katmanlı test sisteminin **orta katmanıdır**:

| Katman | Dosya | Şablon |
|---|---|---|
| Plan (ne, kapsam, risk) | `tests/.../TEST-PLAN.md` | **bu şablon** |
| Senaryo (adım, assert) | `tests/.../*Tests.cs` | [unit-test-template](unit-test-template.md) |
| Entegrasyon (DB, CLI) | `tests/.../Integration/*.cs` | `integration-test-template` |

Ayrıca: bu şablon bir **release gate'i** doldurur — §6.1'deki 20 madde
tamamlanmadan plan "tamamlandı" sayılmaz ve release adayı ilan edilemez.

### §1.2 Disk Kanıtları (2026-09-25, PowerShell — yorum YOK)

Aşağıdaki satırların tamamı bu şablonda **diskten okunmuş, yorumsuz** kanıttır.
Plan yazan kişi bu tabloyu kopyalayıp kendi tarihini basar; **eski tarihli
satırı "hâlâ geçerli" diye yorumlamak yasaktır** — yeniden ölçer.

```text
$ date                          -> 2026-09-25
$ Test-Path C:\versacoder\tests -> False
$ (Get-ChildItem C:\versacoder -Recurse -Include *.csproj,*.sln -EA 0).Count -> 0
$ (Get-ChildItem C:\versacoder\.ai\.templates -Recurse -Filter *.md).Count   -> 29
$ (Get-ChildItem C:\versacoder\.ai\reports -Filter *.md -EA 0).Count         -> 1
$ (Get-ChildItem C:\versacoder\.ai\decisions -EA 0).Count                    -> 0
$ dotnet --version              -> 10.0.401
$ (Get-ChildItem C:\versacoder -Recurse -Filter versacoder.db -EA 0).Count   -> 0
$ (Get-ChildItem C:\versacoder\.ai\rules -EA 0 -File).Count                  -> 0
```

⚠️ **VERIFICATION REQUIRED** — bu plan yazıldığında aşağıdaki iddialar disk
kanıtıyla **doğrulanmamıştır** ve plan "taslak" statüsündedir:

1. `tests/` dizini ve test projesi yok → kapsam tablosundaki modüllerin
   hiçbiri için fiilî test yok; plan **hedef** planıdır, **durum** raporu değil.
2. `src/` yok → modül envanteri `.ai/CLAUDE.md` ve `.ai/index.md` iddiasıdır.
3. `.ai/.decisions/` boş → ADR bağlantısı üretilemez; risk kaydı bu dosyada tutulur.
4. `.ai/.rules/` boş → ".ai/rules G1–G12" iddiası diskte **doğrulanamadı**;
   plan bu kuralları referans alır ama varlıklarını **varsaymaz**.
5. `versacoder.db` yok → DB hedef planı, migration yok.

⚠️ **CONFLICT — test altyapısı SSUT'u çelişiyor:** `.ai/CLAUDE.md` test
sistemi **xUnit + NSubstitute + NetArchTest** der; `backend/repository-template.md`
örnekleri **MSTest** kullanır. SSUT = `.ai/CLAUDE.md`. Çelişki çözülene kadar
plan, iki katmanı da kapsayacak şekilde **çift sürümlü** yazılır:
senaryo dili xUnit, eski örnekler MSTest kalır (kullanıcı çözecek).

⚠️ **CONFLICT — DevExpress sürümü:** `frontend/form-template.md` **26.1**
der, `.ai/CLAUDE.md` **v26.1** der. Plan **sürüm iddiası yapmaz**; uyumluluk
testini §6.1 madde 16'da "sürüm sabitleme + restore" adımı olarak zorlar.

### §1.3 Bu Şablonu Kim, Ne Zaman Kullanır

| Rol | Ne zaman | Girdi | Çıktı |
|---|---|---|---|
| tester (agent) | yeni modül/feature başlamadan | kapsam + risk | `TEST-PLAN.md` taslağı |
| user (insan) | taslak hazırken | taslak | onay / kesme kararı |
| builder (agent) | implementasyon sırasında | onaylı plan | plana uygun testler |
| reviewer | PR'da | plan + kod | §6.1 20 madde denetimi |
| release adayı | §6.1 tamamken | imzalı plan | release izni |

Kural: **plan, koddan önce yazılır.** Koddan sonra yazılan plan, kodu
"geriye doğru meşrulaştırır" ve kapsam kesmelerini gizler → §4.3 bunu yasaklar.

### §1.4 Bu Şablonun Kapsam Dışı Olduğu Konular

- **Tek test dosyasının iç yapısı** (fixture, assert, mock) →
  [unit-test-template](unit-test-template.md) · **DB/CLI entegrasyon testi** → `integration-test-template`
- **Form içi kod** (event handler, layout) → [form-template](../frontend/form-template.md) (frontend);
  WinForms klasör/dosya standartları → `winforms-form-template` (other)
- **Testlerin CI'a alınması** → plan yalnızca *komutu* verir (§5.3); pipeline
  kurulumu bu şablonun dışıdır
- **Performans/ yük testi** — bu şablon fonksiyonel kapsam içindir;
  perf hedefleri VISION'a yazılır, plana değil
- **Kod incelemesi** → [code-review-daily](../../checklists/code-review-daily.md) şablonu; burada yalnızca §6.1'e referans

---

## §2 Kapsam

### §2.1 Kapsam İçindeki Varlıklar

Plan bu **yedi varlığı** birer bölüm olarak içerir; biri yoksa plan eksiktir:

1. **Modül envanteri** — test edilecek modüller + disk kanıtıyla sayısı
2. **Test türleri matrisi** — unit / integration / mimari(NetArchTest) / UI
3. **Kabul kriterleri (ÖLÇÜLEBİLİR)** — geç/ kal eşikleri, % eşiği
4. **Risk kaydı** — en az 5 risk, olasılık × etki, azaltma
5. **Test verisi planı** — sahte veri üretimi, seed, gerçek veri yasağı
6. **Çalıştırma planı** — komutlar, sıralama, filtre, tahmini süre
7. **Çıkış (exit) kriterleri** — release için zorunlu koşullar

### §2.2 Kapsam Dışı (Explicit Out of Scope)

Aşağıdakiler **bu planda yoktur** ve plana eklenmesi talep edilirse
"ayrı plan aç" denir (kapsam şişirmesi yasak → §4.3):

- Kod kalitesi metrikleri (kalite sözleşmesi G6 → [index](../../index.md) §3.7; orası SSUT)
- Üçüncü parti kütüphane sürüm denetimi (→ §6.1 madde 16 yalnızca restore)
- Kod incelemesi checklist'i (→ [code-review-daily](../../checklists/code-review-daily.md))
- Üretim ortamı monitoring / alerting
- Eğitim dokümantasyonu
- Her bir eski MSTest dosyasının tek tek yeniden yazımı (→ ayrı plan)

---

## §3 Mimari

### §3.1 Şablonun Katmanları

```text
+--------------------------------------------------------------+
|  KATMAN 0 — Sabitler (envanter, sürümler, eşişler)          |
|  KATMAN 1 — Kapsam & matris (ne test edilecek / edilmeyecek)|
|  KATMAN 2 — Kabul kriterleri (geç / kal sayısal eşikleri)   |
|  KATMAN 3 — Risk + veri (olasılık×etki, sahte veri)         |
|  KATMAN 4 — Çalıştırma (komutlar, sıra, filtre)             |
|  KATMAN 5 — Çıkış kriterleri + imza (release gate)          |
+--------------------------------------------------------------+
```

Her katman bir §3.alt bölüme karşılık gelir (§3.2–§3.8) ve hiçbiri
atlanamaz: atlanan katman, §6.1'de madde olarak geri döner.

### §3.2 Modül Envanteri (disk kanıtı zorunlu)

```markdown
| Modül | Test tipi | Dosya | Disk kanıtı | Durum |
|---|---|---|---|---|
| VersaCoder.Core/Domain | unit | tests/.../Core.Tests | ⚠️ doğrulanmadı | planlandı |
```

Kural: **"var" demek için `Test-Path` çıktısı gerekir.** Diskte olmayan modül
"planlandı" kalır; "mevcut" yazılamaz (§1.2 yorum yasağı burada da geçerlidir).

### §3.3 Test Türleri Matrisi

| Tür | Araç (SSUT) | Kime ait | Ne doğrular | Ne Doğrulamaz |
|---|---|---|---|---|
| Unit | xUnit + NSubstitute | tester | tek sınıfın davranışı | DB, dosya, ağ |
| Integration | xUnit + Dapper (gerçek DB) | tester | SQL, migration, transaction | UI |
| Mimari | NetArchTest | tester | katman bağımlılık kuralları | iş mantığı |
| UI (opsiyonel) | — (bu şablon dışı) | user | form davranışı | — |

Araç seçimi `.ai/CLAUDE.md`'den gelir; planda araç **icat edilmez**
(§4.5 G8).

### §3.4 Kabul Kriterleri (sayısal)

Plan, geçiş için **sayısal eşik** verir. Örnek iskelet:

```text
Birim test geçiş oranı      : >= %90 (toplam testin)
Mimari test (NetArchTest)   : %100 — ihlal = build kırık
Entegrasyon test geçişi     : >= %95
Kapsanan satır (branch)     : >= %80 (rapor varsa; yoksa madde 16)
Beklenen hata (known-fail)  : 0 adet — planlanmamış kırık test yasak
```

Eşikler **planda sabitlenir**; test dosyasına gömülmez (§4.2 tek-SSUT).

### §3.5 Risk Kaydı

En az 5 satır; her satır: `Risk | Olasılık (L/M/H) | Etki (L/M/H) | Azaltma | Sahip`:

| Risk | Olasılık | Etki | Azaltma | Sahip |
|---|---|---|---|---|
| DB yok → entegrasyon testi yazılamaz | H | H | in-memory yerine gerçek DB kopyası şartı (§5.1) | tester |
| MSTest/xUnit çelişkisi → çift bakım | M | M | SSUT = CLAUDE.md; eski dosyalar dokunulmadan durur | user |
| DevExpress sürüm farkı (26.1 ↔ v26.1) | M | H | sürüm sabitle + restore doğrula (§6.1 m16) | user |
| Kapsam şişirip planın bitmemesi | M | M | §2.2 explicit out-of-scope + §4.3 kesme kuralı | user |
| Sahte veri yerine gerçek kişisel veri | L | H | gerçek veri yasağı §4.5 G3 + §5.2 hata modu 5 | tester |
| Testlerin birbirine bağımlı olması | M | M | her test izole fixture (integration şablonu §4.1) | tester |

### §3.6 Test Verisi Planı

Kural seti (kısa ve bağlayıcı):

1. **Gerçek kullanıcı verisi yasak** — isim, e-posta, kart, ses kaydı, satın
   alma geçmişi planda ve testte geçemez (§4.5 G3).
2. Sahte veri **sentetik ve deterministik** olur: sabit seed, tekrarlanabilir.
3. Seed dosyaları `tests/.../Fixtures/` altındadır; veritabanına toplu veri
   `tests/.../Fixtures/seed.sql` üzerinden girer.
4. Test sonrası veri **temizlenir** (transaction rollback veya explicit cleanup).
5. Üretim DB'sine yazma yetkisi test hesabında **olmaz** (§4.5 G7).

### §3.7 Çalıştırma Planı (komut katmanı)

Plan, testleri **çalıştırılabilir komutlarla** tanımlar (tahmini süre ile):

```text
# hızlı geri bildirim (unit + mimari)
dotnet test --filter "Category=Unit|Category=Architecture"

# entegrasyon (DB erişimi gerektirir)
dotnet test --filter "Category=Integration"

# tamamı + rapor
dotnet test --logger "trx;LogName=plan-results"
```

Komutların geçerliliği §5.3'te **salt-okunur** şekilde ölçülür; planda
"çalıştırıldı" iddiası yoktur, "çalıştırılacak" vardır.

### §3.8 Çıkış Kriterleri (release gate)

Release adayı yalnızca şunlar **hepsi** true ise ilan edilir:

- §6.1'deki 20 maddenin tamamı ✓ (eksik madde = gate kapalı)
- §3.4 eşiklerinin tamamı karşılanmış sayılarla
- Risk kaydında H×H açık kalem yok (varsa kullanıcı onayı şart)
- Plan dosyası `status: active` ve imza satırı dolu

### §3.9 Doldurulabilir İskelet (kopyala → `tests/<Modul>/TEST-PLAN.md`)

```markdown
# TEST PLAN — <MODUL ADI>
**Durum:** taslak | onaylı | release-gate  **Tarih:** <YYYY-AA-GG>  **Sahip:** tester
**Bağlantılar:** [index](index.md) [CLAUDE](CLAUDE.md) [glossary](glossary.md) [keys](keys.md) [VISION](VISION.md)

## §1 Amaç
<1-3 cümle: bu plan hangi modülün test kapsamını tanımlar, release gate'i nedir>

## §2 Kapsam
Kapsam içi: <modül listesi>   Kapsam dışı: <§2.2'den kopyalanacaklar>

## §3 Envanter (disk kanıtı)
<§3.2 tablosu + tarihli Test-Path çıktısı>

## §4 Kabul Kriterleri
<§3.4 eşikleri — sayılar dolu>

## §5 Risk & Veri
<§3.5 risk tablosu (>=5) + §3.6 veri kuralları>

## §6 Çalıştırma
<§3.7 komutları + sıralama + tahmini süre>

## §7 Çıkış Kriterleri & İmza
<§3.8 + imza satırı>
```

**Zorunlu bağ iskeleti:** her plan, [unit-test-template](unit-test-template.md),
`integration-test-template` ve [code-review-daily](../../checklists/code-review-daily.md) dosyalarına link verir;
kesilmiş kapsam burada açıkça "ayrı plana taşındı" diye işaretlenir.

---

## §4 Kurallar

### §4.1 Zorunlu Kurallar (bu plan bunlara uyar)

1. Plan **koddan önce** yazılır (§1.3).
2. Her kapsam satırının **disk kanıtı** olur (Test-Path çıktısı); yoksa
   satır "planlandı" statüsündedir.
3. Kabul kriterleri **sayısal** olur; "iyi olur", "yaklaşık", "gerekirse"
   gibi belirsizlikler geçersizdir (§4.5 G5).
4. Araç/ dil/ framework **seçilmez, referans verilir** → `.ai/CLAUDE.md` SSUT.
5. Çelişki (MSTest vs xUnit, DevExpress sürümü) **gizlenmez**, §1.2'de
   `⚠️ CONFLICT` olarak yazılır ve sahibine bağlanır.
6. Plan tamamlanınca §6.1'in 20 maddesi **denetlenir**; denetim
   bu dosyada işaretlenir, ayrı rapora sızdırılmaz.

### §4.2 Yasaklar (Hard — ihlali planı geçersiz kılar)

1. Gerçek kullanıcı verisi plana/ test verisine **girmez** (§4.5 G3).
2. Diskte doğrulanmamış varlık **"mevcut"** diye yazılmaz (§1.2 yorum yasağı).
3. Eşik **sayısal değilse** plan release gate'ini **açamaz**.
4. Testlerde `skip`, `Ignore`, `Assert.IsTrue(true)` **planlanamaz** —
   planlanan tek geçici kırık test, sahibi + bitiş tarihi ile kayıt ister.
5. Kapsam dışı madde (§2.2) planda **yer almaz**; eklenirse ayrı plan açılır.
6. Başka şablonun içeriği **kopyalanmaz**; yalnızca link verilir (copy ban).

### §4.3 Kapsam Kesme Kuralı

Plan şişerse kesme sırası **bu sırayladır**: (a) UI türü, (b) opsiyonel
performans maddeleri, (c) eski MSTest dosyalarının tek tek yeniden yazımı.
**Kesilemez:** DB entegrasyon kapsamı, mimari (NetArchTest) kapsamı,
release exit kriterleri. Kesilen her madde "ayrı plan" olarak §3.9'da
işaretlenir — sessizce atılmaz.

### §4.4 Değişiklik Kontrolü

Plan onaylandıktan sonra değişiklik: `version` 1.0.0 → 1.0.1 (imla) veya
→ 1.1.0 (kapsam ekleme/çıkarma); kapsam değişikliğinde §6.1 **baştan**
denetlenir ve imza yenilenir. Değişiklik satırı planın en altına eklenir:
`- <tarih> v<no> — <ne değişti> — <kim onayladı>`.

### §4.5 Hard Guardrails (10 madde — bunlar aşılamaz)

| # | Guardrail | Sonuç |
|---|---|---|
| G1 | Plan koddan **önce** yazılır | koddan sonra plan = meşrulaştırma, reddet |
| G2 | Her kapsam satırı disk kanıtlıdır | kanıtsız satır silinir |
| G3 | Gerçek kullanıcı verisi yok | veri ihlali = plan iptal |
| G4 | Araç seçilmez, SSUT'tan okunur | icat edilen araç = plan geçersiz |
| G5 | Eşikler sayısal | belirsiz eşik = gate kapalı |
| G6 | Çelişkiler `⚠️ CONFLICT` ile yazılır | gizlenen çelişki = usulsüzlük |
| G7 | Üretim yazma yetkisi testte yok | üretim DB'sine yazım = olay |
| G8 | `skip`/`Ignore` planlanamaz | gizli kırık test = geçersiz |
| G9 | Kapsam dışı madde planda yok | şişirilmiş plan = reddet |
| G10 | §6.1 20 madde release gate'idir | eksik madde = release yok |

---

## §5 Workflow

### §5.1 Adım Adım (adım + toplam)

| # | Adım | Araç / Dosya | Toplam |
|---|---|---|---|
| 1 | Sabitleri tazele (§1.2 komutları çalıştır, çıktıyı yapıştır) | PowerShell | ~1 dk |
| 2 | Modül envanterini disk kanıtıyla doldur (§3.2) | PowerShell | ~2 dk |
| 3 | Test türleri matrisini SSUT'tan yaz (§3.3) | `.ai/CLAUDE.md` | ~3 dk |
| 4 | Sayısal kabul kriterlerini sabitle (§3.4) | bu şablon | ~3 dk |
| 5 | Risk kaydını ≥5 satır yaz (§3.5) | bu şablon | ~4 dk |
| 6 | Veri planını yaz (§3.6) | bu şablon | ~3 dk |
| 7 | Çalıştırma planını komutlarıyla yaz (§3.7) | bu şablon | ~3 dk |
| 8 | Çıkış kriterleri + imza (§3.8) | bu şablon | ~2 dk |
| 9 | §6.1 20 madde denetimi, sonuçları işaretle | PowerShell + göz | ~8 dk |
| 10 | Kullanıcı onayı + version sabitle (§4.4) | user | ~2 dk |
| 11 | Dosyayı `tests/<Modul>/TEST-PLAN.md` olarak kaydet, index'e ekle | edit | ~2 dk |
| | | **Toplam** | **~36 dk** |

Adım 1–8 olmadan 9'a geçmek yasaktır: denetim, boş planı "geçmiş" gösterir.

### §5.2 Hata Modları (≥8 — her biri tespit + düzeltme)

| # | Hata modu | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Kanıt eskimiş | §1.2'de eski tarih | adımı yeniden çalıştır, tazele |
| 2 | Varsayım yazımı | "muhtemelen var" satırı | Test-Path yoksa "planlandı"ya çek |
| 3 | Çelişki gizleme | CONFLICT satırı boş | §1.2'ye `⚠️ CONFLICT` ekle, sahibine bağla |
| 4 | Gerçek veri sızıntısı | isim/e-posta/DB kaydı | veriyi sil, sentetik seed ile değiştir |
| 5 | Belirsiz eşik | "yeterince iyi" | sayısal eşiğe çevir ya da maddeyi kes |
| 6 | Kapsam şişmesi | §2.2 dışı madde | §4.3 sırasıyla kes, ayrı plana taşı |
| 7 | Kopya içerik | başka şablonun cümlesi | sil, link ver (copy ban) |
| 8 | Araç icadı | planda yeni framework adı | SSUT'a dön, referans ver |
| 9 | İmza eksik | gate açık ama imza boş | gate'i kapat, onay al |
| 10 | Geç denetim | adım 9 erken atlanmış | 20 maddeyi baştan işaretle |

Tespit edilen her hata modu, §6.1'de karşılık bulan madde ile
**çapraz işaretlenir** (planda `→ §6.1 m<n>` yazılır).

### §5.3 Doğrulama Komutları (≥5 — salt-okunur, yazma YOK)

Aşağıdakiler **hiçbiri dosya/DB değiştirmez**; plan yazan kişi bunları
kendi ortamında çalıştırıp çıktıyı §6.1'e yapıştırır.

```powershell
# 1) Test projesi var mı? (yoksa plan "planlandı" statüsünde kalır)
(Get-ChildItem C:\versacoder -Recurse -Filter *Tests.csproj -EA 0).Count

# 2) Mevcut test dosyalarının türü (xUnit mi MSTest mi) — çelişki kanıtı
Select-String -Path C:\versacoder\tests\**\*.cs -Pattern 'Fact\]|TestMethod\]' -EA 0 |
  Group-Object Pattern | Select-Object Name,Count

# 3) Ortam (SDK hedefleme, test runner) — sürüm çelişkisi kontrolü
dotnet --version

# 4) Şablon envanteri (planın referans verdiği şablonlar diskte mi)
Get-ChildItem C:\versacoder\.ai\.templates -Recurse -Filter *.md | Measure-Object | Select-Object Count

# 5) Plan dosyasının kendisi (7 bölüm + VRR satırı)
$f='C:\versacoder\tests\TEST-PLAN.md'; if (Test-Path $f) {
  $c = Get-Content $f -Encoding UTF8
  "satir=$($c.Count) bolum=$(($c | Where-Object { $_ -match '^## §' }).Count)" }
  else { 'plan dosyasi YOK — plan henuz yazilmadi' }

# 6) Gerçek veri sızıntısı taraması (plan taslağı üzerinde)
Select-String -Path $f -Pattern '@gmail|@hotmail|@\w+\.com' -EA 0
```

> Not: `dotnet test` **koşullu çalıştırılır** (§3.7); koşmak **yazma** değildir
> ama DB üzerinde entegrasyon testi veri değiştirirse §3.6 temizlik şartı devreye
> girer. Salt-okunur olan 6 komut yukarıdadır.

### §5.4 Quality Report (planın kendi kalite tablosu)

| # | Soru | Kanıt (§) | Durum |
|---|---|---|---|
| 1 | Envanter disk kanıtlı mı? | §3.2 + §5.3-1 | ☐ |
| 2 | Tür matrisi SSUT'tan mı? | §3.3 + CLAUDE | ☐ |
| 3 | Eşikler sayısal mı? | §3.4 + §4.5 G5 | ☐ |
| 4 | Risk ≥5 satır mı? | §3.5 | ☐ |
| 5 | Veri planı gerçek veri yasağı içeriyor mu? | §3.6 + G3 | ☐ |
| 6 | Komutlar salt-okunur doğrulandı mı? | §5.3 | ☐ |
| 7 | Çıkış kriterleri + imza var mı? | §3.8 | ☐ |
| 8 | Çelişkiler `⚠️ CONFLICT` ile açık mı? | §1.2 + G6 | ☐ |
| 9 | §6.1 20 madde işaretli mi? | §6.1 | ☐ |
| 10 | Copy ban ihlali yok mu? | §4.2.6 | ☐ |

---

## §6 Doğrulama

### §6.1 Kabul Listesi (20 madde — release gate)

| # | Madde | Kanıt | ✓ |
|---|---|---|---|
| 1 | §1.2 tarihli PowerShell çıktıları yapıştırıldı | tarih satırı | ☐ |
| 2 | Modül envanterinde `Test-Path` kanıtı var | §3.2 | ☐ |
| 3 | Diskte olmayan varlıklar "planlandı" statüsünde | §3.2 | ☐ |
| 4 | Test türleri matrisi `.ai/CLAUDE.md` ile uyumlu | §3.3 | ☐ |
| 5 | MSTest/xUnit çelişkisi `⚠️ CONFLICT` yazıldı | §1.2 | ☐ |
| 6 | DevExpress sürüm çelişkisi `⚠️ CONFLICT` yazıldı | §1.2 | ☐ |
| 7 | Kabul kriterleri tamamı sayısal | §3.4 + G5 | ☐ |
| 8 | Geç/ kal eşikleri planda sabit, testte gömülü değil | §4.2 | ☐ |
| 9 | Risk kaydı ≥5 satır, sahibi belli | §3.5 | ☐ |
| 10 | H×H risk yok ya da kullanıcı onayı var | §3.8 | ☐ |
| 11 | Test verisi planı gerçek veri yasağını içeriyor | §3.6 + G3 | ☐ |
| 12 | Sahte veri deterministik (seed) olarak tanımlı | §3.6 | ☐ |
| 13 | Çalıştırma komutları §5.3 ile doğrulandı | §5.3 | ☐ |
| 14 | `skip`/`Ignore` planlanmadı | G8 | ☐ |
| 15 | Kapsam dışı maddeler planda yok (§2.2) | §4.2.5 | ☐ |
| 16 | SDK/test-runner sürümü restore ile doğrulandı | §5.3-3 | ☐ |
| 17 | Çıkış kriterleri yazıldı, gate mantığı net | §3.8 | ☐ |
| 18 | İmza + version satırı dolu | §4.4 | ☐ |
| 19 | Başka şablondan kopya cümle yok (copy ban) | §4.2.6 | ☐ |
| 20 | §5.4 Quality Report'ta 10/10 ✓ | §5.4 | ☐ |

**Gate kuralı:** 20/20 değilse release adayı **ilan edilemez**; eksik maddeler
`✗` olarak listelenir ve sahibine bağlanır. 19/20 "neredeyse bitti" değildir.

### §6.2 Araç Doğrulama (≥3)

1. Bölüm bütünlüğü: `((Get-Content $f) -match '^## §').Count` → **7**.
2. Satır/ boyut aralığı: satır 480–660, KB 25–34.
3. Mojibake: `Select-String $f -Pattern '[\u00C3\u00C2]|\uFFFD'` → **0**.
4. Kutu genişliği: ```text bloğundaki her satır **66 karakter**
   (`ascii-diagram-template.md` §5.3 kuralı).

### §6.3 Geçmiş Kontroller (her revizyonda tekrar)

- §1.2'deki tarih **bugün mü**? Değilse adım 1 yeniden çalıştırılır.
- CONFLICT satırları hâlâ açık mı? Kapatıldıysa satır **silinmez**,
  `→ çözüldü (tarih)` olarak güncellenir (denetim izi korunur).
- §6.1'deki ✓ işaretleri **gerçek kanıtla** mı verildi? İşaretsiz madde=✗.

### §6.4 Red Flag (derhal dur)

- Planda disk kanıtı olmadan "mevcut/üretimde" yazan satır
- Ayrıntısız eşik ("genelde geçer", "çoğu test") — §4.5 G5 ihlali
- Plan koddan **sonra** yazıldıysa (§4.5 G1 ihlali) → plan yeniden yazılır
- Gerçek kullanıcı verisi planda/geçmişte geçtiyse (G3) → olay, plan iptal
- 20 maddenin biri eksikken release ilanı

### §6.5 Çıkış Kriterleri

Plan yalnızca şunlar **hepsi** tamken çıkar:

1. §6.1 = 20/20 ✓ (kanıt bağlantılı)
2. §5.4 = 10/10 ✓
3. §6.2 araç kontrolleri geçti (7 bölüm / aralık / mojibake 0 / kutu 66)
4. İmza satırı dolu + version sabitlendi (§4.4)
5. Eksik her şey §6.1'de `✗` + sahibi ile listelendi (gizlenen eksik yok)

---

## §7 Referanslar

### §7.1 Bu Şablonun Referans Verdikleri (hiçbiri bu dosyaya kopyalanmadı)

| Kaynak | Ne alındı | Kopya? |
|---|---|---|
| `.ai/CLAUDE.md` | test sistemi SSUT (xUnit, NSubstitute, NetArchTest) | hayır — referans |
| `.ai/index.md` | envanter + §3.7 kalite sözleşmesi bağlamı | hayır — referans |
| `.ai/keys.md` | anahtar/kavram sözlüğü | hayır — referans |
| `.ai/VISION.md` | ürün hedefi, release bağlamı | hayır — referans |
| `.ai/.templates/backend/repository-template.md` | 7 bölüm formatı (yapı referansı) | hayır — yapı |
| `.ai/.templates/testing/unit-test-template.md` | komşu şablon, çapraz link | hayır — link |
| `.ai/.templates/testing/integration-test-template.md` | komşu şablon, çapraz link | hayır — link |
| `.ai/.templates/architecture/ascii-diagram-template.md` | §5.3 kutu genişliği kuralı (66) | hayır — kural |
| `C:\www\coremusic.net\.ai\.templates\` | **yalnızca yapı referansı** | **sıfır kelime** |
| `C:\www\versacoder\versacoder.ai\.ai\.ai\` | **yalnızca yapı referansı** | **sıfır kelime** |

Copy ban (§4.2.6): iki kopya kaynağından **tek kelime** alınmamıştır;
yalnızca "7 bölüm" iskeleti ortak kullanılmıştır.

### §7.2 Red Team — Bu Şablonu Kullanmadan Önce Oku (12 satır)

| # | Senaryo | Sonuç | Savunma |
|---|---|---|---|
| 1 | Plan koddan sonra yazıldı | kapsam geriye doğru meşrulaşır | G1 + §1.3 |
| 2 | Kanıt tarihi eski | plan başka dünyanın planı olur | §1.2 + §6.3 |
| 3 | Çelişki gizlendi (MSTest/xUnit) | yanlış araçta test yazılır | G6 + §1.2 |
| 4 | Eşik belirsiz | gate açılır ama hiçbir şey ölçülmez | G5 |
| 5 | Gerçek veri planda | gizlilik ihlali + temizlik yükü | G3 + §3.6 |
| 6 | Kapsam şişti, kesilmedi | plan hiç bitmez, release gecikir | §4.3 |
| 7 | `skip` ile "geçen" test | yeşil dashboard, boş güvenlik | G8 |
| 8 | Araç icat edildi | CLAUDE.md ile çift bakım | G4 |
| 9 | Envantere disk kanıtsız modül yazıldı | var olmayan şey test edilmez sanılır | G2 |
| 10 | 19/20 ile release | eksik madde sonradan patlar | §6.1 gate |
| 11 | Çevrimdışı / DB yokken entegrasyon planı | plan uygulanamaz kalır | §5.2 m1 + hata modları |
| 12 | Plan tek kişide, imzasız | onaysız release denemesi | §3.8 + §4.4 |

**Kapanış kuralı:** bu şablonun işi **öngörülemeyeni görünür kılmaktır**;
her `⚠️`, `✗` ve `CONFLICT` satırı, ileride patlayacak tek bir hatanın
şimdiden fatura edilmesidir. Şablonu doldururken tek dürüstlük ölçüsü vardır:
**diskte yoksa yoktur.**
