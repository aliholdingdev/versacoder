---
title: "WORKFLOW — Beş Kapı Kriterleri ve Kanıt Kutusu"
type: index
category: root
status: active
version: 1.0.0
updated: 2026-10-06
authority: SSOT
scope: "GATE-004a/003/004/005/006 kapılarının tam kriter metni, kanıt kutusu KANIT/DOSYA/EKSIK/SONRAKI biçimi ve durum kanıtı - kriter metni bu dosyada yaşar, kök WORKFLOW.md yalnız özet verir"
---

# WORKFLOW — Beş Kapı: Kriter · Kanıt Kutusu · Durum

**Zorunlu Bağlantılar:** [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [WORKFLOW özeti](../WORKFLOW.md) · [templates index](.templates/index.md) · [batch manifest](.templates/batch-manifest-gate004a.md) · [çakışma kaydı](reports/conflict-register-001-master-prompts.md)

> **Tek cümlelik sorumluluk:** Bu dosya beş kapının (GATE-004a · GATE-003 · GATE-004 ·
> GATE-005 · GATE-006) tam kriter metninin, `KANIT/DOSYA/EKSIK/SONRAKI` kanıt kutusu
> biçiminin ve durum kanıtının **tek kaynağıdır**; kök `WORKFLOW.md` yalnız kapı adlarını,
> sırayı ve özet tabloyu verir (`WORKFLOW.md` L16-17 · L39-40).

> **Kapıyı yalnız kullanıcı açar.** Bu dosya kriteri ve kanıtı tanımlar, kapı **açmaz**
> (`WORKFLOW.md` L35 · `batch-manifest-gate004a.md` L160).

## İçindekiler

| Bölüm | Alt konular |
|---|---|
| §1 Amaç | 1.1 Kapsam · 1.2 Disk Kanıtları · 1.3 Hedef Kitle · 1.4 Akış |
| §2 Kapsam | 2.1 Kapsam Dışı · 2.2 Sınır Testi |
| §3 Mimari | 3.1 Kanıt kutusu · 3.2 GATE-004a · 3.3 GATE-003 · 3.4 GATE-004 · 3.5 GATE-005 · 3.6 GATE-006 · 3.7 Durum kanıtı · 3.8 Sıra ve devir |
| §4 Kurallar | 4.1 Zorunlu · 4.2 Yasak |
| §5 Workflow | 5.1 Adım tablosu · 5.2 Hata modları |
| §6 Doğrulama | 6.1 Checklist · 6.2 Red sinyalleri |
| §7 Referanslar | 7.1 Tablo · 7.2 Red Team |

---

## §1 Amaç

### §1.1 Kapsam

**İçinde:** beş kapının kapatma konusu, kabul kriterleri, gerekli kanıt, RED sonrası davranış,
ilgili checklist dosya adı; kanıt kutusu alanları; durumun **nereden** okunduğu.
**Dışında:** kapı özeti zinciri ve doğrulama komutları (kök `WORKFLOW.md` §1, §4), madde
listeleri (`.ai/checklists/` — **ad olarak yazılır, yok**), betik gövdesi (`.ai/scripts/` — yok),
kalıcı kural metni (`.ai/.rules/` — yok), günlük durum (`.ai/log.md` — yok).

### §1.2 Disk Kanıtları

İddia → kanıt. Ölçüm: `C:\versacoder`, **2026-10-06**, `list_directory` + `read_file` çıktısı.

| # | İddia | Durum | Kanıt |
|---|---|---|---|
| 1 | Kök `WORKFLOW.md` = **64 satır** · kapı tablosu L25-31 | ✅ DOĞRULANDI | `read_file` L1-64 |
| 2 | Bu dosya `.ai/WORKFLOW.md` (kriter metni hedefi) | ✅ ÜRETİLDİ | bu part tek dosya yazımı |
| 3 | `.ai/` altında **yalnız 3 klasör**: `.templates` · `prompts` · `reports` | ✅ DOĞRULANDI | `list_directory .ai` → 3 `[DIR]` |
| 4 | Kök `AGENTS.md` · `CLAUDE.md` · `README.md` · `WORKFLOW.md` | ✅ DOĞRULANDI | `list_directory` kök → 4 `[FILE]` |
| 5 | `.ai/checklists/` · `.ai/scripts/` · `.ai/log.md` · `.ai/index.md` · `.ai/CLAUDE.md` | ❌ **YOK** | `list_directory .ai` = 3 girdi; hedefler listede yok |
| 6 | `src/` · `tests/` · `.sln` · `global.json` | ❌ **YOK** | kök listing = `.ai` · `.git` · `.opencode` + 4 MD |
| 7 | Kaynak 5 dosya (manifesto · çakışma kaydı · templates index · kök CLAUDE · kök WORKFLOW) | ✅ OKUNDU | §7.1 satır 1-6 |
| 8 | Checklist adları (4 ad) `batch-manifest-gate004a.md` L77 ile rezerve | ✅ DOĞRULANDI | okunan satır L77 |
| 9 | Kapı başına süre değerleri | ❌ **DOĞRULAMA GEREKLİ** | `WORKFLOW.md` L39 "süreler" der, sayı vermez |

### §1.3 Hedef Kitle

| Rol | Bu belgeden ne alır |
|---|---|
| Kullanıcı (kapı sahibi) | §3.7 durum tablosu + §3.x kabul kriterleri ile kapıyı **açar** |
| Orkestratör / ajan | §3.1 kanıt kutusu biçimini, §4.1-§4.2 kapı kuralını |
| Reviewer | §6.1 checklist + §6.2 RED sinyalleri |
| Docs-writer | §5.2 hata modları + §7 referans zinciri |

### §1.4 Akış

```text
+----------------------------------------------------------------+
|zincir: 004a(vault) -> 003(UI onay) -> 004 -> 005 -> 006        |
|sira: atlanmaz · kapıyı kullanıcı açar · RED üreten ajana döner |
|kanıt: KANIT/DOSYA/EKSIK/SONRAKI · durum: log.md son kayıt      |
+----------------------------------------------------------------+
```

**Ne anlatır:** beş kapı sırayla ilerler; kutu dolmadan kapı sonucu yazılmaz; durum
`.ai/log.md` son kaydından okunur — bu dosya tahmin üretmez (`WORKFLOW.md` L41-42, L61-63).

---

## §2 Kapsam

### §2.1 Kapsam Dışı

| Kapsam dışı soru | Cevabı nerede |
|---|---|
| "Kapı zinciri ve sırası nedir?" | kök `WORKFLOW.md` §1 (L21-23) |
| "Şimdi hangi kapı açık?" | `.ai/log.md` son kaydı — **dosya diskte yok** |
| "Checklist maddeleri neler?" | `.ai/checklists/*.md` — **klasör diskte yok**; yalnız adları §3.x'te |
| "Ölçüm betiği nasıl çalışır?" | `.ai/scripts/*.ps1` — **klasör diskte yok**; betiksiz bloklar `batch-manifest-gate004a.md` §10 |
| "Hangi dosya hangi batch'te?" | `batch-manifest-gate004a.md` §1 |
| "Hangi kural hangi guardrail?" | `.templates/index.md` §4.5 + kök `CLAUDE.md` B6 |

### §2.2 Sınır Testi

İlk `EVET` bu dosyayı bağlar:

1. **Soru bir kapının kriterine, kanıtına ya da RED davranışına mı?** → **bu dosya** (§3).
2. Soru kapının *özetine* (ad, sıra, komut) mi? → kök `WORKFLOW.md` §1, §4.
3. Soru *şu anki* duruma mı? → `.ai/log.md` (yok → `DOĞRULAMA GEREKLİ`).
4. Soru checklist *maddelerine* mi? → `.ai/checklists/*.md` (yok → §3.x yalnız adı verir).
5. Soru betik *içeriğine* mi? → `.ai/scripts/*.ps1` (yok → manifest §10 blokları).
6. Hiçbiri değilse → bu dosya değil; talep `.templates/index.md` §5.1 Adım 1'den açılır.

---

## §3 Mimari

### §3.1 Kanıt Kutusu — `KANIT / DOSYA / EKSIK / SONRAKI`

Kök `WORKFLOW.md` L39-40, kapı kriterlerinin **tam metnini, kutu biçimini ve süreleri** bu
dosyaya havale eder. Kutu **her kapı satırının altında** zorunludur; alan adları değiştirilemez,
sıra sabittir.

| # | Alan | Ne yazılır | Yazılmazsa |
|---|---|---|---|
| 1 | `KANIT` | salt-okunur komut çıktısı **ya da satır numarası** (`AGENTS.md` L19) | kapı RED |
| 2 | `DOSYA` | kanıtın yaşadığı dosya yolu + satır aralığı | kapı RED |
| 3 | `EKSIK` | kanıtı olmayan öğe; eksik yoksa `YOK` | kapı RED |
| 4 | `SONRAKI` | RED ise düzeltilecek **tek** eylem; iş yoksa `YOK` (`session-log-template.md` L207) | iş sahipsiz kalır |

**Zorunlu kurallar:**

1. Sonuçsuz kelimeler ("başarılı", "tamam", "çalıştı") **kanıt sayılmaz** (`session-log-template.md` L349-350).
2. `FAIL` satırı silinmez; `SONRAKI` alanına düzeltme yazılır (`session-log-template.md` L347-348).
3. Kanıt yoksa kutu **uydurulmaz**; `EKSIK` satırına `DOĞRULAMA GEREKLİ` yazılır (kök `CLAUDE.md` B7 L83).

**Dolu örnek (gerçek değer, yer tutucu yok):**

```text
KANIT   : (Get-Content .\WORKFLOW.md).Count
DOSYA   : WORKFLOW.md L1-L64
EKSIK   : YOK
SONRAKI : YOK - kapıyı kullanıcı açar
```

**`DOĞRULAMA GEREKLİ` (süreler):** kök `WORKFLOW.md` L39 kapı kriterleri ve kutu ile birlikte
"süreler" der; hiçbir kaynak kapı başına süre vermez → süre uydurulmadı (G9 · sıfır halüsinasyon).

### §3.2 GATE-004a — Vault Yeşili (ölçüm kapısı)

| Alan | Metin | Kaynak |
|---|---|---|
| **Ne kapatır** | Vault yeşil: ölçüm sayaçları sıfırlarda (link · mojibake · frontmatter) | `WORKFLOW.md` L27 |
| **Checklist** | `gate-004a-vault-kapanis.md` (ad — `.ai/checklists/` yok) | `WORKFLOW.md` L27 · manifest L77 |

**Kabul kriterleri (hepsi VE):**

| # | Kriter | Hedef | Kaynak |
|---|---|---|---|
| K-1 | Bozuk link | **0** | manifest L150 |
| K-2 | Mojibake | **0** | manifest L151 |
| K-3 | Frontmatter eksik | **0** | manifest L152 |
| K-4 | `TotalMD` | **≥ 500** | manifest L153 |
| K-5 | Rezerve ad eksik | **0** | manifest L154 |
| K-6 | Yeni dosya boyutu | 3-8 KB sınıfı (şablonlar hariç) | manifest L155 |

**Gerekli kanıt:** betikli ölçüm `.ai/scripts/check-links.ps1` · `count-components.ps1` ·
`measure-vault.ps1` (manifest L87-89) — **betikler diskte yok** (`conflict-register-001-master-prompts.md`
L99); bu durumda **betiksiz bloklar** manifest §10/1-§10/5 aynı sayaçları üretir (manifest L92-93).

**RED durumunda:** satır `❌` olur, o batch'e dönülür (manifest L145-146, L173); kapı kapalı
kalır ve **GATE-003'e geçilmez** (manifest L159); gerekçe tek satır, `SONRAKI` alanına yazar.

### §3.3 GATE-003 — Ekran Onayı (HARD STOP)

| Alan | Metin | Kaynak |
|---|---|---|
| **Ne kapatır** | Ekran onayı — onaysız `src/**` kodu yasak (HARD STOP) | `WORKFLOW.md` L28 |
| **Kapsam (CF-10)** | Ekran **dosyası** + `src/**` **kodu** bloklanır; **şablon üretimini bloklamaz** | `conflict-register-001-master-prompts.md` L43, L60 · `.templates/index.md` L233, L420 |
| **Checklist** | `gate-003-ui-onay.md` (ad) | `WORKFLOW.md` L28 · manifest L77 |

**Kabul kriterleri:**

| # | Kriter | Kanıt |
|---|---|---|
| 1 | B13 tel kafes seti onaya hazır: **10 dosya** (7 wireframe + index + flows + component-inventory) | manifest L43, L444 |
| 2 | Ekran listesi master prompt SSOT ile birebir: `01-start-page` · `02-main-ide` · `03-solution-explorer` · `04-code-editor` · `05-ai-chat` · `06-agent-panel` · `07-settings` | manifest L62, L446 |
| 3 | Format `.templates/ui-design/wireframe-template.md` (UTF-8 kutu çizimi) | manifest L63 |
| 4 | Kullanıcı onayı **tarihli** ve `.ai/log.md` kaydında | `WORKFLOW.md` L28 · manifest L448 |

**Gerekli kanıt:** kullanıcı onay metni + tarih + dosya listesi (kutu `KANIT` satırında onay
kayıdı, `DOSYA` = `.ai/log.md`). **Not:** `.ai/log.md` diskte **yok** (§1.2/5) → onay kaydı
yazılana kadar kapı kapalıdır.

**RED durumunda:** B13 satırı `⏸` olur (manifest L173, L448); `G8` **Duraklat** (`.templates/index.md`
L420); `src/**` altına tek satır `.cs` yazılmaz (kök `CLAUDE.md` L29 · `AGENTS.md` L20); hata
modu FM7 ile kayıt açılır (manifest L299). Onay **öncesi** üretilmiş ekran dosyaları silinmez,
`⏸` ile işaretlenir.

### §3.4 GATE-004 — İskelet + Build + İlk Commit

| Alan | Metin | Kaynak |
|---|---|---|
| **Ne kapatır** | İskelet + **2 bacak** build + dosya bazlı ilk commit | `WORKFLOW.md` L29 |
| **Checklist** | `gate-004-ilk-commit.md` (ad) | `WORKFLOW.md` L29 · manifest L77 |

**Kabul kriterleri:**

| # | Kriter | Kanıt |
|---|---|---|
| 1 | İskelet diskte: `src/` · `tests/` · `.sln` · `global.json` | `WORKFLOW.md` L52 (bugün **hepsi YOK**) |
| 2 | Bacak 1: `dotnet build` çıktısı **0 hata** | `WORKFLOW.md` L29 |
| 3 | Bacak 2: `dotnet test` çıktısı **0 hata** | `WORKFLOW.md` L29 |
| 4 | İlk commit **dosya bazlı** (dosya listesi + commit çıktısı) | `WORKFLOW.md` L29 |

**Gerekli kanıt:** iki komutun çıktısı, kutu `KANIT` alanında birebir satır.

**RED / bloke durumunda:** `dotnet build` **`MSB1003`** verir (hedef `src/` yok — `WORKFLOW.md`
L52); kapı kapalı kalır, işi **üreten ajana döner**, başka ajana devredilmez (`WORKFLOW.md`
L35-36). **`DOĞRULAMA GEREKLİ`:** "dosya bazlı ilk commit" hangi dosya kümesini ve kaç commiti
kapsıyor — kaynakta tanımlanmamış (`WORKFLOW.md` L29 tek satır).

### §3.5 GATE-005 — Test + NetArchTest

| Alan | Metin | Kaynak |
|---|---|---|
| **Ne kapatır** | Test + **NetArchTest 0 ihlal** | `WORKFLOW.md` L30 |
| **Checklist** | `gate-005-test-kapsami.md` (ad) | `WORKFLOW.md` L30 · manifest L77 |

**Kabul kriterleri:**

| # | Kriter | Kanıt |
|---|---|---|
| 1 | `dotnet test` çıktısı **0 hata** | `WORKFLOW.md` L30 |
| 2 | NetArchTest katman kuralı **0 ihlal** | `WORKFLOW.md` L30 |
| 3 | Test hedefi bulunur | `WORKFLOW.md` L53 — bugün `tests/` **YOK** |

**Gerekli kanıt:** `dotnet test` çıktısı (kutuda `KANIT`), test projesi yolu (`DOSYA`).

**RED / bloke durumunda:** `tests/` yoksa `dotnet test` **hedef bulamaz** (`WORKFLOW.md` L53);
kapı kapalı, iş üreten ajana döner. **`DOĞRULAMA GEREKLİ`:** (a) NetArchTest hangi projede
çalışıyor — kaynak yalnız "0 ihlal" der, proje adı vermez (`WORKFLOW.md` L30); (b) **%90
coverage** hedefi (`CLAUDE.md` L74 · B5 L54) hangi kapının kriteri — kaynakta kapıya
bağlanmamış.

### §3.6 GATE-006 — Teslim

| Alan | Metin | Kaynak |
|---|---|---|
| **Ne kapatır** | Teslim | `WORKFLOW.md` L31 |
| **Checklist** | **bu bandda tanımlı liste yok** — `.ai/checklists/` içinde GATE-006 dosyası rezerve edilmemiş | `WORKFLOW.md` L31 · manifest L77 (yalnız 4 ad) |

**Kabul kriterleri:**

| # | Kriter | Kanıt | Durum |
|---|---|---|---|
| 1 | Önceki dört kapı açık (004a · 003 · 004 · 005) | §3.2-§3.5 kutuları | ✅ kaynağında |
| 2 | Teslim raporu | kök `WORKFLOW.md` L31: "`WORKFLOW.md` §5 raporu" | ⚠️ **DOĞRULAMA GEREKLİ** |

**`DOĞRULAMA GEREKLİ` (2):** (a) kök `WORKFLOW.md` **§1-§4** içerir, **§5 yok** (L19, L46, L56) →
raporun yeri belirsiz; (b) teslim raporunun zorunlu içeriği hiçbir kaynakta yazılmıyor. Kriter
uydurulmadı.

**RED durumunda:** `WORKFLOW.md` L54 — "önceki kapılar kapalı" → GATE-006 **BLOKE**; sıraya
dönülür, teslim iddiası kanıt kutusu olmadan yazılmaz.

### §3.7 Durum Kanıtı

**Kural:** durum tahmin edilmez; **`.ai/log.md` son kaydı** esastır (`WORKFLOW.md` L41-42, L63).
`log.md` diskte yoksa durum **`DOĞRULAMA GEREKLİ`** olarak yazılır.

Kök `WORKFLOW.md` §3'ün kendi beyanı (**ölçüm günü 2026-10-05**, L46):

| Kapı | Durum | Kaynak beyanı | Bu dosyanın notu (2026-10-06) |
|---|---|---|---|
| GATE-004a | ÖLÇÜLEBİLİR | 553 MD · link 0 · mojibake 0 · frontmatter 0 (L50) | Bu sayılar **kapsam dışı, doğrulanmadı** (`conflict-register-001-master-prompts.md` L98) |
| GATE-003 | AÇIK (HARD STOP) | onay yok → `src/**`a yazılmadı (L51) | `.ai/log.md` YOK → onay kanıtlanamaz |
| GATE-004 | BLOKE | `src/` `tests/` `.sln` `global.json` YOK → `MSB1003` (L52) | kök listing ile **doğrulandı** (§1.2/6) |
| GATE-005 | BLOKE | `tests/` YOK (L53) | kök listing ile **doğrulandı** (§1.2/6) |
| GATE-006 | BLOKE | önceki kapılar kapalı (L54) | §3.6/1 |

**Durum kanıtı komutu** (kök `WORKFLOW.md` L63): `Get-Content .\.ai\log.md -Tail 5` — hedef
dosya yoksa komut çalışmaz; kapı **açılmış sayılmaz**.

### §3.8 Sıra, Devir ve Strike

| # | Kural | Kaynak |
|---|---|---|
| 1 | Sıra atlanmaz: `GATE-003` onayından **önce** `src/**` altına tek satır `.cs` yazılmaz | `WORKFLOW.md` L37 · kök `CLAUDE.md` L29 |
| 2 | RED veren kapı işi **üreten ajana** döndürülür, başka ajana devredilmez | `WORKFLOW.md` L35-36 |
| 3 | Kapı RED'i **strike sayılmaz** — bu gate kuralıdır, ajan yöntemi değil | `WORKFLOW.md` L38 |
| 4 | Kapı zinciri kazanır: `GATE-000…014` (Prompt B) reddedildi, **5 kapı** esas | `conflict-register-001-master-prompts.md` L35, L54, D2 L91 |
| 5 | Manifesto kanıt üretir, **kapı açmaz** | `batch-manifest-gate004a.md` L160 |

---

## §4 Kurallar

### §4.1 Zorunlu

1. Beş kapı için **beş şe** yazılır: ne kapatır · kabul kriterleri · gerekli kanıt · RED
   davranışı · checklist dosya adı (§3.2-§3.6).
2. Her kapı satırının altında **`KANIT/DOSYA/EKSIK/SONRAKI` kutusu** vardır (§3.1).
3. Her kriterin yanında **kaynak: dosya + satır** vardır (örn. `WORKFLOW.md` L27).
4. Kanıt yoksa satır `DOĞRULAMA GEREKLİ` alır; kapı **açılmış sayılmaz** (kök `CLAUDE.md` L83).
5. Durum her açılışta `.ai/log.md` son kaydından okunur (`WORKFLOW.md` L63).

### §4.2 Yasak

1. **YASAK:** kapı kriterini kök `WORKFLOW.md`'den kopyalayıp ikinci gerçek kaynak yapmak (G12) —
   kök yalnız özet verir (`WORKFLOW.md` L16-17).
2. **YASAK:** kutu doldurulmadan "kapı geçti" demek — kanıtsız "bitti" (G5).
3. **YASAK:** kapı başına süre, coverage yüzdesi, dosya sayısı **uydurmak** — yoksa `DOĞRULAMA GEREKLİ` (G9).
4. **YASAK:** `.ai/checklists/` · `.ai/scripts/` · `.ai/log.md` dosyalarını varmış gibi göstermek
   (diskte **YOK** — §1.2/5).
5. **YASAK:** `GATE-003` onayı olmadan `src/**` kodu ya da üretilmiş ekran dosyası yazmak (G8) —
   şablon üretimi serbesttir (CF-10).

---

## §5 Workflow

### §5.1 Adım Tablosu

| # | Adım | Çıktı | Kanıt | Süre |
|---|---|---|---|---|
| 1 | Durum oku | `.ai/log.md` son kayıt | kayıt satırı | `—` |
| 2 | GATE-004a ölçümü | 6 kriter (K-1…K-6) | manifest §10 blokları | `—` |
| 3 | GATE-003 onayı | tarihli kullanıcı onayı | `.ai/log.md` kaydı | `—` |
| 4 | GATE-004 | build + test + ilk commit | 2 komut çıktısı | `—` |
| 5 | GATE-005 | 0 test hatası + 0 NetArchTest ihlali | `dotnet test` çıktısı | `—` |
| 6 | GATE-006 | teslim raporu | §3.6/2 (**DOĞRULAMA GEREKLİ**) | `—` |

**Süre sütunu:** `WORKFLOW.md` L39 sürelerin bu dosyada yazacağını söyler, değer vermez →
**`DOĞRULAMA GEREKLİ`** (tahmin yazılmadı).

### §5.2 Hata Modları

| # | Belirti | Kök neden | Düzeltme |
|---|---|---|---|
| 1 | Ölçümden önce `✅` basılmış | adım 2 atlanmış | durum `❌`, §10 blokları yeniden (manifest L171) |
| 2 | Betik çıktısı yok ama "ölçüldü" denmiş | `.ai/scripts/` yok, uydurulmuş (L99) | betiksiz blokları çalıştır (manifest L92-93) |
| 3 | `src/**`'da kod var, onay kaydı yok | GATE-003 atlanmış (G8) | dosyaları `⏸`, onaya dön (manifest L299) |
| 4 | Kapı RED'i "ajan başarısızlığı" sayılmış | gate kuralı bilinmiyor | strike sayılmaz (`WORKFLOW.md` L38) |
| 5 | Süre/coverage/MD sayısı komut çıktısız | G9 ihlali | `DOĞRULAMA GEREKLİ` yaz, kutuyu doldur |
| 6 | Kapı başka ajanaya devredilmiş | devir kuralı ihlali | işi üreten ajana geri gönder (`WORKFLOW.md` L36) |

---

## §6 Doğrulama

### §6.1 Checklist

- [ ] Frontmatter **8 alan**, sıra birebir: `title, type, category, status, version, updated, authority, scope`
- [ ] Dosya **UTF-8 BOM'suz** — ilk 3 bayt `45,45,45`
- [ ] Satır aralığı **250-450**
- [ ] Kırık bağlantı hedefi = **0** (yalnız diskte var olan hedeflere link yazılır)
- [ ] Yer tutucu kalıbı (çift süslü parantezli değişken) = **0** (G4)
- [ ] Mojibake = **0** (G5)
- [ ] Beş kapı için checklist adları yazılı: 4 ad + GATE-006 "liste yok" (`WORKFLOW.md` L27-31)
- [ ] Her kapıda 5 öğe var: ne kapatır · kabul · kanıt · RED · checklist (§3.2-§3.6)
- [ ] Kanıtsız her kriterde `DOĞRULAMA GEREKLİ` etiketi var (§3.1/9 · §3.4 · §3.5 · §3.6 · §5.1)
- [ ] Durum tablosundaki her satırda kaynak `dosya + satır` var (§3.7)

### §6.2 Red Sinyalleri

| # | Sinyal | Aksiyon |
|---|---|---|
| R1 | Kriter metni kök `WORKFLOW.md`'de tekrar edilmiş | **RED** — sil, bu dosyaya bağ (G12) |
| R2 | Kutusuz kapı sonucu | **RED** — §3.1 kutusunu doldur |
| R3 | `.ai/checklists/` · `.ai/scripts/` · `.ai/log.md` link olarak yazılmış | **RED** — düz metne çevir (G7) |
| R4 | `GATE-003` onaysız `src/**` kodu | **RED / DUR** — §3.3 (G8) |
| R5 | Komut çıktısız "553 MD" · "0 kırık link" | **RED** — kapsam dışı beyan (`conflict-register…` L98) |
| R6 | Uydurulmuş süre / coverage / adet | **RED** — `DOĞRULAMA GEREKLİ` (G9) |
| R7 | Yer tutucu kalıbı, mojibake ya da BOM | **RED** — G4 · G5 |
| R8 | Kapıyı ajanın açtığını söyleyen satır | **RED** — kapıyı yalnız kullanıcı açar (`WORKFLOW.md` L35) |

---

## §7 Referanslar

### §7.1 Tablo

| # | Kaynak | Yol | Ne verir |
|---|---|---|---|
| 1 | Kök kural | [CLAUDE](../CLAUDE.md) | B3 kapı kuralı (L29) · B6 G1..G12 · B7 "olmayana `DOĞRULAMA GEREKLİ`" (L83) |
| 2 | Ajan gölgesi | [AGENTS](../AGENTS.md) | `KANIT` satırı = komut çıktısı / satır no (L19) · kapı (L20) |
| 3 | Kapı özeti | [WORKFLOW](../WORKFLOW.md) | zincir (L22) · tablo (L25-31) · kural (L33-44) · durum (L46-54) · komutlar (L56-64) |
| 4 | Şablon envanteri + G8 | [templates index](.templates/index.md) | §4.5 G8 (L420) · §5.1 9 adım (L428) · CF-10 ön koşulu (L233) |
| 5 | Batch manifestosu | [batch manifest](.templates/batch-manifest-gate004a.md) | K-1…K-6 (L148-157) · §5 ölçüm (L84-93) · B13 (L441-448) · FM1-FM10 (L291-302) |
| 6 | Çakışma kaydı | [çakışma kaydı](reports/conflict-register-001-master-prompts.md) | CF-02 = 5 kapı (L35) · CF-10 kapsam (L43, L60) · D2 (L91) · kapsam dışı beyanlar (L98) |
| 7 | Örnek rapor | [adım 4 raporu](reports/adim4-vault-skeleton-001.md) | `DOĞRULAMA GEREKLİ` ile yazılmış kayıp-kayıt örneği (L18-33) |
| 8 | Sözleşme | [new-doc-contract](.templates/new-doc-contract.md) | üretilen dosyanın frontmatter/link/boyut kuralları |
| 9 | Oturum kaydı | [session log](.templates/session-log-template.md) | `SONRAKI_ADIM` = tek eylem + süre (L207) · kanıt satırı biçimi (L328-350) |
| 10 | Vault kuralı | [templates CLAUDE](.templates/CLAUDE.md) | `.templates/` dizin kuralı |
| 11 | Arşivler | [Prompt A](prompts/01-master-enterprise-engineering-v2.md) · [Prompt B](prompts/02-master-project-installation-v1.md) | reddedilen 15/14 kapı modeli (CF-02) |
| 12 | Özet README | [README](../README.md) | depo tanıtımı |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden yanlış | Doğrusu |
|---|---|---|---|
| 1 | "Kapı geçti"yi kutusuz yazmak | kanıt yok, G5 ihlali | §3.1 dört alan dolu |
| 2 | Kriteri kök `WORKFLOW.md`'den kopyalamak | iki gerçek kaynak (G12) | kök özet, kriter burada |
| 3 | `.ai/log.md`'ye link vermek | dosya diskte yok (G7) | düz metin + `DOĞRULAMA GEREKLİ` |
| 4 | `.ai/checklists/gate-003-ui-onay.md` linki | klasör yok (G7) | dosya adı `code span` olarak |
| 5 | GATE-003 kapsamına şablonu eklemek | CF-10: şablon serbest | yalnız ekran dosyası + `src/**` kodu |
| 6 | Betik çıktısı uydurmak | `.ai/scripts/` yok | manifest §10 betiksiz blokları |
| 7 | "≈30 dk" gibi süre yazmak | kaynakta süre yok | `—` + `DOĞRULAMA GEREKLİ` |
| 8 | "553 MD doğrulandı" demek | kapsam dışı beyan (`conflict-register…` L98) | ölçüyü kendin çalıştır |
| 9 | Kapıyı ajanın açtığını yazmak | kapı yalnız kullanıcıya ait (`WORKFLOW.md` L35) | kutu `SONRAKI` = kullanıcı onayı |
| 10 | GATE-006 checklist'i uydurmak | kaynak "liste yok" diyor (`WORKFLOW.md` L31) | "bu bandda tanımlı liste yok" yaz |
| 11 | Kanıtsız %90 coverage | kapıya bağlanmamış | `DOĞRULAMA GEREKLİ` (§3.5) |
| 12 | Mojibake (`?` · `\xc3\xbc`) ya da BOM | G5 | UTF-8 BOM'suz kaydet |

**SSUT beyanı:** Bu dosya beş kapının **kriter metninin** tek kaynağıdır; kök `WORKFLOW.md`
özet verir, `.ai/log.md` durumu, `.ai/checklists/` maddeleri, `.ai/scripts/` ölçüyü taşır —
hiçbiri bu dosyaya kopya satır yazmaz (G12).
