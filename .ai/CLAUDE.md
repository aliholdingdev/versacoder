---
title: "VERSA CODER — Vault Kural Metni (G1..G12)"
type: rules
category: root
status: active
version: 1.0.0
updated: 2026-10-06
authority: SSOT
scope: "G1..G12 kural metni, vault okuma sırası, hard guardrail tablosu, ne yap/ne yapma ve 50.000 token bütçe — kök CLAUDE.md yalnız bağlantı verir"
---

# VERSA CODER — Vault Kural Metni (G1..G12)

**Zorunlu Bağlantılar:** [kök CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [README](../README.md) · [templates](.templates/index.md) · [templates CLAUDE](.templates/CLAUDE.md)

> **Tek cümlelik sorumluluk:** *Bu vault'ta hangi kural geçerlidir, ihlali ne olur, tespiti nasıl yapılır?* sorusunun tek cevabı bu dosyadır; kök `CLAUDE.md` yalnız tek-satır özet ve bağlantı verir (G12).

---

## §0 Özet (Answer First)

**Cevap:** Kural metni **burada** yaşar (§4.5) · okuma sırası §3.1'dedir (kök CLAUDE §B4) · guardrail tablosu §3.3'tedir · bütçe §5.2'dedir (50.000 token/oturum). Dayanağı olmayan iddia uydurulmaz, `DOĞRULAMA GEREKLİ` ile işaretlenir (G9).

| Soru | Cevap | Bölüm |
|---|---|---|
| Kural metni nerede yaşar? | bu dosya, §4.5 | §4.5 |
| Okuma sırası nedir? | 5 halkalı tek yönlü zincir | §3.1 |
| Guardrail ihlali ne yapar? | Red · Duraklat | §3.3 |
| Bütçe nedir? | 50.000 token/oturum | §5.2 |
| Yeni belge hangi söze uyar? | 8 alan frontmatter · boyut sınıfı | §3.2 |

**İçindekiler:** [§1 Amaç](#1-amaç) · [§2 Kapsam](#2-kapsam) · [§3 Mimari](#3-mimari) · [§4 Kurallar](#4-kurallar) · [§5 Workflow](#5-workflow) · [§6 Doğrulama](#6-doğrulama) · [§7 Referanslar](#7-referanslar)

---

## §1 Amaç

Bu dosya, `.ai/` vault'unun **G1..G12 kural metninin** tek gerçek kaynağıdır (SSOT). Kök [CLAUDE](../CLAUDE.md) §B6 yalnız 12 tek-satır özet ve bağlantı taşır; ayrıntı buraya bağlanır (`[kök] 60`: "metin `.ai/.rules/` ve `.ai/CLAUDE.md` §4.5'tedir"). SRP (G12) gereği kural metni kök dosyaya **kopyalanmaz**.

### §1.1 Dosya Ayrımı

| Dosya | Rol | İçerik | Durum |
|---|---|---|---|
| `CLAUDE.md` (kök) | önyükleme + bağlantı | rol · B4 okuma sırası · B6 tek-satır özet · B9 bütçe | ✅ (`[kök] 32–116`) |
| `.ai/CLAUDE.md` (bu dosya) | kural metni SSOT | G1..G12 kapsam/ihlal/tespit/dayanak · guardrail tablosu | ✅ bu üretim |
| `.templates/CLAUDE.md` | `.templates/` dizin kuralı | ayrı dosya — bu dosyanın **hedefi değil** | ✅ |
| `.ai/.rules/*.md` | kök dosyanın iddia ettiği kural metni | kök "18 MD" der, `[çk] 41` disk = **YOK** der → sayı **DOĞRULAMA GEREKLİ** | ❌ YOK |

**Dayanak:** `[env] 337–338` (envanter §3.10 dizin ağacı: `.rules\` `.decisions\` `architecture\` `checklists\` `scripts\` `index.md` `CLAUDE.md` `WORKFLOW.md` `keys.md` `log.md` `adr\` = "YOK (AŞAMA C)") · `[çk] 41` (CF-08) · `[çk] 24` (kök omurga ertelendi, linkler silinmeyecek).

### §1.2 Dayanak Anahtarı

§4.5'teki `Dayanak` satırları bu kısaltmaları kullanır; satır numaraları 2026-10-06 ölçümüne göredir.

| Kısaltma | Dosya | Aç |
|---|---|---|
| `[kök]` | depo kökü `CLAUDE.md` | [aç](../CLAUDE.md) |
| `[env]` | `.templates/index.md` | [aç](.templates/index.md) |
| `[söz]` | `.templates/new-doc-contract.md` | [aç](.templates/new-doc-contract.md) |
| `[çk]` | `reports/conflict-register-001-master-prompts.md` | [aç](reports/conflict-register-001-master-prompts.md) |

---

## §2 Kapsam

**İçinde:** G1..G12 kural metni · vault okuma sırası · hard guardrail tablosu · frontmatter ve boyut sözleşmesinin özeti · 50.000 token bütçe · ne yap/ne yapma.
**Dışında:** kod (`src/` YOK, `[kök] 108`) · ADR karar metni (`.ai/.decisions/` YOK, `[çk] 62`) · şablon envanteri · kapı tanımı.

| Kapsam | Kapsam Dışı — nereye gider |
|---|---|
| §3.1 okuma sırası (kök CLAUDE §B4) | katman spec'i → `.ai/architecture/` (YOK) |
| §3.2 frontmatter 8 alan sözleşmesi | şablon envanteri → [templates](.templates/index.md) |
| §3.3 + §4.5 G1..G12 guardrail metni | ADR karar metni → `.ai/.decisions/` (YOK) |
| §5.1 üretim akışı · §5.2 bütçe | kapı tanımı → [WORKFLOW](../WORKFLOW.md) |
| §6 doğrulama komutları | ölçüm betikleri → `.ai/scripts/` (YOK, `[çk] 99`) |

### §2.1 Kapsam Sınırı Testi

**İlk `EVET` işi belirler:**

1. Soru "hangi kural geçerli, ihlali ne, tespiti nasıl"? → **EVET** — bu dosya §4.5
2. Soru "yeni MD dosyası hangi söze uyar"? → [new-doc-contract](.templates/new-doc-contract.md)
3. Soru "şablon iskeleti nedir, envanterde kaç şablon var"? → [templates](.templates/index.md)
4. Soru "hangi kapı, hangi faz, ne üretilmez"? → [WORKFLOW](../WORKFLOW.md)
5. Hiçbiri → bu dosya kullanılmaz; önce §3.1 okuma sırası baştan çalıştırılır.

**HAYIR soruları:** harici vault'tan cümle var mı (G1)? · yer tutucu kalmış mı (G4)? · diskte olmayan hedefe bağlantı yazılmış mı (G7)?

---

## §3 Mimari

### §3.1 Vault Okuma Sırası (kök CLAUDE §B4)

Zincir tek yönlüdür; ilk halka eksikse sonrakiler okunmaz. Tablo `[kök] 34–41` ile birebirdir.

| Sıra | Okunan | Ne verir | Tekrar eder mi | Disk (2026-10-06) |
|---|---|---|---|---|
| 1 | `CLAUDE.md` (kök) | rol, vault bağları, katman özeti, yasaklar | Hayır — yalnız bağlantı | ✅ |
| 2 | `.templates/index.md` | 44 şablon envanteri + §3.7 kalite sözleşmesi | Hayır | ✅ |
| 3 | `.ai/.rules/*.md` | kalıcı kural metni | Hayır | ❌ YOK → kural metni **bu dosya §4.5** |
| 4 | `.ai/architecture/index.md` · `.ai/WORKFLOW.md` | katman gerçeği · kapı gerçeği | Hayır | ❌ YOK → kapı [WORKFLOW](../WORKFLOW.md) |
| 5 | Görevin kendi şablonu | üretilcek dosyanın iskeleti | Hayır | göreli |

```text
+--------------------------------------------------------------+
| [1] kok CLAUDE.md     rol + baglanti + tek-satir ozet (B6)    |
|   v                                                           |
| [2] .templates/index.md   44 sablon + kalo sozlesmesi         |
|   v                                                           |
| [3] .ai/.rules/*.md       YOK -> kural metni .ai/CLAUDE.md    |
|   v                                                           |
| [4] architecture + WORKFLOW  YOK -> kopek WORKFLOW.md         |
|   v                                                           |
| [5] gorevin sablonu   <== KAPI: butce 50.000 token/oturum     |
+--------------------------------------------------------------+
```

**Durum:** `[3]`–`[4]` halkaları diskte YOK → zincir **planlıdır**; kural metni bu dosyaya çekilmiştir (`[kök] 60`, `[env] 337–338`).

### §3.2 Frontmatter Sözleşmesi (8 alan)

Dosyanın **ilk 9 satırı**: açılış `---`, ardından sırayla sabit 8 alan; 9. alan (`tags` vb.) redtir (`[söz] 294–295`). Sıra değiştirilemez (D3).

| # | Alan | Bu Dosyadaki Değer | Eksikse |
|---|---|---|---|
| 1 | `title` | `"VERSA CODER — Vault Kural Metni (G1..G12)"` | Red |
| 2 | `type` | `rules` | Red |
| 3 | `category` | `root` | Red |
| 4 | `status` | `active` | Red |
| 5 | `version` | `1.0.0` (üç bölüm) | Red |
| 6 | `updated` | `2026-10-06` (ISO, üretim günü) | Red |
| 7 | `authority` | `SSOT` | Red |
| 8 | `scope` | 1 satır Türkçe kapsam cümlesi | Red |

**Dayanak:** `[env] 269–282` (§3.7) · `[söz] 27–42` (§1) · `[söz] 285–295` (§9.1 D1–D6) · **CF-03** `[çk] 36` + `[çk] 55` — envanterdeki `part` iddiası reddedildi, kazanan **disk: 38/38 `scope`**.

### §3.3 Hard Guardrail Tablosu (G1..G12)

Kaynak `[env] 409–424` (§4.5). Kapsam · İhlal · Tespit sütunları birebir oradan; açıklama ve dayanak §4.5'te.

| ID | Kapsam | İhlal | Tespit yolu |
|---|---|---|---|
| G1 | `tümü` | Red + sil | envanter §7.2 R1 · `[söz]` §11 R14 |
| G2 | B/D | Red | `Select-String 'EntityFramework'` |
| G3 | `tümü` | Red | envanter §6.1 K1 · `[söz]` §9.1 D1–D3 |
| G4 | **üretim çıktısı** (CF-04) | Red | envanter §5.3 K5 |
| G5 | `tümü` | Red | gözden geçirme · `[söz]` §12/4 |
| G6 | `.templates/**` — `prompts/` + `reports/` hariç (D4) | Red | envanter §5.3 K3 |
| G7 | `tümü` | Red | envanter §5.3 K6 · `[söz]` §12/3 |
| G8 | C/D — ekran dosyası + `src/**`; şablon serbest (CF-10) | **Duraklat** | [WORKFLOW](../WORKFLOW.md) |
| G9 | A/B | Red | `ADR-001…030` (`.ai/.decisions/`) |
| G10 | B | Red | `Query-Template.md` |
| G11 | D | Red | xunit + coverage |
| G12 | envanter | Red | envanter §1.1 M7 |

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Kural | İhlal |
|---|---|---|---|
| 1 | Önce `.templates/` | şablonu olmayan çıktı üretilmez | Red (`[env] 367`) |
| 2 | Sıra: envanter → dizin CLAUDE → Part A → B → C → D | part atlanmaz | Red (`[env] 368`) |
| 3 | İçerik özgün, yapı referanslı | cümle kopyalanmaz (G1) | Red (`[env] 369`) |
| 4 | Şablon yazımında envanter değiştirilmez | tek geçiş `PLANLI → YAZILDI` | Red (`[env] 370`) |
| 5 | Her şablondan sonra §6.1 checklist | 17/17 geçmeden sıradakine geçilmez | Red (`[env] 371`) |

### §4.2 Onay Akışı

| Adım | Rol | Ne Onaylar | Süre |
|---|---|---|---|
| 1 | `docs-writer` | Taslak + frontmatter (8 alan) | 10 dk |
| 2 | `reviewer` | `[env]` §6.1 checklist (17 madde) | 8 dk |
| 3 | `architect` | Mimari tutarlılık (katman sırası, G2, G9) | 6 dk |
| 4 | `security` | Part A/B — `adr-security` + G10 | 5 dk |
| 5 | `master-orchestrator` | Part geçişi + Quality Report | 5 dk |

> Toplam ≈ **34 dk/part**. **3 red** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir (`[env] 383`).

### §4.3 Sürüm ve Değişiklik

| Olay | Sürüm | `updated` | Kayıt |
|---|---|---|---|
| İlk yazım `0.1.0 → 1.0.0` | `major` | evet | `updated` + `log.md` |
| Bölüm eklenmesi (`§x.y`) | `minor` | evet | Quality Report |
| Yalnız tablo satırı ekleme | `patch` | evet | `updated` |
| Bölüm silme / yeniden adlandırma | `major` | evet | bağlantı veren dosyalar |

**Kural:** `version` her zaman üç bölüm, `updated` her zaman ISO `YYYY-MM-DD` (`[env] 407`). `log.md` diskte YOK (AŞAMA C) → kayıt **DOĞRULAMA GEREKLİ**, tahmin yazılmaz (`[env] 338`).

### §4.4 Ne Yap / Ne Yapma

Dayanak: `[kök] 79–91` (B7).

| ✅ Yap | ❌ Yapma (ilk satırda DUR) |
|---|---|
| Bağlantı ver, kuralı **okumaya** git | Kural metnini kök dosyaya kopyala (G12) |
| Katmanı `l0..l7` koduyla söyle | Katman adı uydur / numarayı kaydır |
| Olmayana `DOĞRULAMA GEREKLİ` yaz | Olmayan dosyayı "şurada" diye göster |
| Bağlantıyı tek satırda `·` ile tut | 8+ bağlantı · dağınık navigasyon |
| Tarihi ISO yaz (`YYYY-MM-DD`) | Serbest tarih biçimi |
| DevExpress sürümünü ADR'den al (`26.1`) | "en güncel sürüm" kanıtsız iddiası |
| `@p1..@pN` örneğini göster | String concat SQL örneği ver |
| Ölç, sonra "tamam" de | Kanıtsız "hizalı/doğru" iddiası (G9) |
| Ürün adı `VERSACODER` / VersaCoder | Eski ürün adını hiçbir yerde geçirmek |
| İş bitince `log.md`'ye tek satır düş | Kök dosyayı her oturumda yeniden yaz |

### §4.5 Hard Guardrails — G1..G12 Kural Metni

Her kural: **tek satır özet → kapsam → ihlal → tespit yolu → dayanak**. Kapsam/ihlal/tespit `[env] 409–424`'ten birebir; dayanak satır numaraları §1.2 anahtarını kullanır.

#### G1 — Harici vault'tan içerik kopyalanamaz

**Özet (tek satır):** Harici vault'tan içerik kopyalanamaz; yalnız **yapı** referans alınır, içerik özgün yazılır.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `tümü` | **Red + sil** — dosya silinir, özgün yeniden yazılır | envanter §7.2 R1 · `[söz]` §11 R14 | `[env] 413` · `[kök] 64` · `[söz] 90` |

#### G2 — EF Core yasak

**Özet (tek satır):** EF Core yasak; veri erişimi ADO.NET + Dapper, her sorgu `@p1..@pN` parametreli.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `B` (şablon) · `D` (kanıt) | **Red** | `Select-String 'EntityFramework'` — sıfır eşleşme | `[env] 414` · `[kök] 65` · `[söz] 86` |

> **Not:** `src/` diskte yok (`[kök] 108`) → parametreli sorgunun **kod kanıtı DOĞRULAMA GEREKLİ**; kural metni şablon ve ADR düzeyinde geçerlidir.

#### G3 — Frontmatter'sız Markdown yasak

**Özet (tek satır):** Frontmatter'sız Markdown yasak; ilk 9 satırda 8 alan tam ve sırayla (`title, type, category, status, version, updated, authority, scope`).

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `tümü` | **Red** | envanter §6.1 K1 · `[söz]` §9.1 D1–D3 (alan 8 · ayraç 2 · sıra 8) | `[env] 415` · `[env] 269–282` · `[çk] 36, 55` |

> **Not (CF-03):** envanter §3.7 sekizinci alan için `part` öne sürmüş, `[çk] 55` ölçümü `part=0, scope=38` bulmuştur → kazanan **disk**; `part` bilgisi envanterin sütunundadır, frontmatter'da değil.

#### G4 — Üretilen dosyada yer tutucu kalmaz

**Özet (tek satır):** Şablondan üretilen **gerçek dosyada** çift süslü parantezle sarılmış yer tutucu adedi 0 olmalıdır; şablonun kendisinde yer tutucu **serbesttir**.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| **üretim çıktısı** (şablon değil) | **Red** | envanter §5.3 K5 — şablonda çıkan sayı **bilgidir, red değildir** | `[env] 416` · `[env] 500` · `[çk] 37, 56` · `[söz] 207–208` |

#### G5 — Türkçe zorunlu, mojibake yasak

**Özet (tek satır):** Türkçe zorunlu; mojibake, BOM ve bozuk çift bayt yasak — kodlama UTF-8 **BOM'suz**.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `tümü` | **Red** | gözden geçirme · `[söz]` §12/4 (mojibake 0 · BOM yok · Türkçe harf > 0) | `[env] 417` · `[kök] 68` · `[söz] 94` |

#### G6 — 480 satır altı şablon reddedilir

**Özet (tek satır):** `< 480` satırlık şablon reddedilir; tek muaf `.templates/CLAUDE.md` (90–140 satır / 5–8 KB).

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `.templates/**` — `prompts/` ve `reports/` **hariç** (D4) | **Red** | envanter §5.3 K3 — satır sayımı | `[env] 418` · `[çk] 38, 57, 93` · `[söz] 308–309` |

> **Not (CF-05 / D4):** 480 altındaki 9 dosya (24 · 81 · 90 · 102 · 103 · 117 · 122 · 144 · 479 satır) ≥480'e çıkarılıyor; `CLAUDE.md` 139 satır muaf. Kapsam `.templates/**` ile sabitlendi (`[çk] 93`, durum 🔄 üretimde). `prompts/` + `reports/` 3–8 KB sözleşmesine tabidir (`[söz] 190`).

#### G7 — Bozuk bağlantı yasak

**Özet (tek satır):** Bozuk bağlantı yasak; markdown bağlantısının **her hedefi diskte var olmalıdır**; silme/overwrite kullanıcı onayı ister.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `tümü` | **Red** | envanter §5.3 K6 · `[söz]` §12/3 — kırık hedef listesi boş olmalı | `[env] 419` · `[env] 511` · `[kök] 70` · `[söz] 68` |

> **Not:** `.ai/` kök hedefleri (`index.md`, `keys.md`, `.rules/`, `.decisions/`, `architecture/`, `scripts/`, `WORKFLOW.md`, `log.md`) diskte YOK (`[env] 337–338`) → bu dosyada **bağlantı olarak yazılmaz**, düz metin kalır.

#### G8 — GATE-003 onayı

**Özet (tek satır):** `GATE-003` onaysız **üretilmiş ekran dosyası** ve `src/**` kodu üretilmez; şablon üretimi serbesttir.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `C` (ekran) · `D` (kod) — şablon **hariç** | **Duraklat** | [WORKFLOW](../WORKFLOW.md) | `[env] 420` · `[kök] 71` · `[çk] 43, 60` |

> **Not (CF-10):** kök CLAUDE "ekran/**kod**" der (SSOT), envanter "ekran/**dosya**" der → kazanan **kök CLAUDE**: kapı ekran dosyasını ve `src/**` kodunu bloklar, **şablonu bloklamaz**; döngü (şablon → ekran → onay) aksi halde kapanmaz (`[çk] 60`).

#### G9 — ADR'siz mimari karar yok

**Özet (tek satır):** ADR'siz mimari karar yok; `.ai/.decisions/` dışında karar belgesi yazılmaz, kanıtsız sayı yazılmaz.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `A` · `B` | **Red** | `ADR-001…030` — yolu `.ai/.decisions/` | `[env] 421` · `[kök] 72` · `[çk] 46, 62` |

> **Not (CF-13):** iki yol iki aralık çakışıyordu (`.ai/.decisions/` `ADR-001…030` ↔ envanter `.ai/adr/` `ADR-001…008`); kazanan **kök CLAUDE**, 11 noktada düzeltildi. İkisi de diskte yok → ADR'lerin varlığı ve aralık tutarlılığı **DOĞRULAMA GEREKLİ** (`[çk] 62`).

#### G10 — String concat SQL yasak

**Özet (tek satır):** SQL'de string concat yasak; her sorgu `@p1..@pN` parametreli ve indeks notlu yazılır.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `B` | **Red** | `Query-Template.md` — `@pN` kalıbı | `[env] 422` · `[kök] 73` · `[söz] 86` |

#### G11 — Uyarılar hata, coverage %90

**Özet (tek satır):** `TreatWarningsAsErrors=true` ve %90 coverage ihlali = red.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| `D` | **Red** | xunit + coverage çıktısı | `[env] 423` · `[kök] 74` |

> **Not:** `tests/` ve `.sln` diskte YOK (`[kök] 109`) → coverage ölçümü **DOĞRULAMA GEREKLİ**; kural yalnız plan düzeyinde geçerlidir.

#### G12 — Envantere içerik kopyalanamaz (SRP)

**Özet (tek satır):** Envanterde içerik tekrarı yasak; şablon/kural içeriği kendi dosyasında yaşar, envantere yalnız satır + bağlantı girer.

| Kapsam | İhlal | Tespit yolu | Dayanak |
|---|---|---|---|
| envanter (bu dosya hariç) | **Red** | envanter §1.1 M7 | `[env] 424` · `[kök] 75` |

---

## §5 Workflow

### §5.1 9 Adımlı Üretim Akışı

Birebir `[env] 428–441` (§5.1).

| Adım | İş | Çıktı | Sorumlu | Zaman | Kapı |
|---|---|---|---|---|---|
| 1 | Talep ayrıştırma | envanterde `PLANLI` satır | docs-writer | 2 dk | — |
| 2 | İskelet taraması | §3.7 sözleşmesinden bölüm listesi | docs-writer | 3 dk | — |
| 3 | Tür + hedef atama | `Tür`, `Hedef Dosya` | architect | 2 dk | — |
| 4 | Taslak yazım | frontmatter + H1 + 7 bölüm | docs-writer | 20 dk | — |
| 5 | Yer tutucu temizliği | yer tutucu = 0 | docs-writer | 3 dk | G4 |
| 6 | Bağlantı denetimi | kırık bağlantı = 0 | reviewer | 4 dk | G7 |
| 7 | Ölçüm | satır/KB raporu | reviewer | 2 dk | G6 |
| 8 | Part geçiş kontrolü | geç / kal | master-orchestrator | 3 dk | §6.3 |
| 9 | Onay + kayıt | `YAZILDI` + Quality Report | master-orchestrator | 6 dk | §6.4 |
| | **Toplam/şablon** | | | **≈ 45 dk** | |

### §5.2 Bütçe ve Güncelleme Tetikleri

| Öğe | Değer | Dayanak |
|---|---|---|
| Oturum bütçesi | **50.000 token** | `[kök] 114` |
| Kök dosya payı | ≤ %3 (~1.500 token) — > 2.000 → RED | `[kök] 114` |
| Bu dosyanın (`\.ai/CLAUDE.md`) kendi payı | ayrı eşik kaynaklı değil → **DOĞRULAMA GEREKLİ** | — |
| Güncelleme tetigi | kural / katman / rol değişince → `version` minor + `updated` + `log.md` | `[kök] 115` |
| Yalnız düzeltme | bağlantı ya da imleç → `version` patch + tek satır log | `[kök] 116` |
| Süre bütçesi (üretim) | 44 şablon ≈ 275 dk + %15 tampon ≈ 5,0 saat | `[env] 443–451` |

**Aşım davranışı:** bütçe aşılırsa iş değil **okuma** yemiştir → §4.5'te metin değil bağlantı yazılır; kırmızı akış `[söz]` §11 R15.

### §5.3 Hata Modları

| # | Hata Modu | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Kural kopyası | kök dosyada uzun G paragrafı | bağlantıya çevir (G12) |
| 2 | Olmayan hedefe bağlantı | §6.3 çıktısında kırık var | bağlantıyı kaldır, düz metne çevir (G7) |
| 3 | Yer tutucu sızıntısı | üretim çıktısında çift süslü parantez kalıbı | temizle, yeniden ölç (G4) |
| 4 | Katman / kapı kayması | `l7 Host` gibi yanlış kod | kök CLAUDE §B5 · [WORKFLOW](../WORKFLOW.md) |
| 5 | Kanıtsız sayı | komut çıktısız "553 MD" gibi iddia | `DOĞRULAMA GEREKLİ` (G9) |

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

| # | Kontrol | Beklenen | İlgili |
|---|---|---|---|
| K1 | Frontmatter ilk 9 satır · 8 alan · sıra birebir | 8/8 | G3 · §3.2 |
| K2 | `authority: SSOT` · `version` üç bölüm · `updated` ISO | birebir | G3 |
| K3 | H1 tek + `**Zorunlu Bağlantılar:**` 6 bağlantı | 1 satır · 6 | G7 |
| K4 | Kırık bağlantı | 0 | G7 · §6.3 |
| K5 | İlk 3 bayt `45,45,45` (BOM yok) · mojibake | `45,45,45` · 0 | G5 |
| K6 | G1..G12 bloğu 12/12 · her blokta 5 alan + dayanak | 12/12 | §4.5 |
| K7 | Üretim çıktısında yer tutucu | 0 | G4 |
| K8 | Satır aralığı | 300–500 | iş sözleşmesi |
| K9 | `EntityFramework` geçen satır | 0 | G2 · G10 |
| K10 | Kanıtsız sayı / eski ürün adı izi | 0 · 0 | G1 · G9 |

### §6.2 Red Sinyalleri

| # | Sinyal | Aksiyon |
|---|---|---|
| R1 | §6.3 çıktısında kırık bağlantı | **DUR** — hedefi üret ya da bağlantıyı sil (G7) |
| R2 | Üretim çıktısında yer tutucu | **DUR** — doldur (G4) |
| R3 | Diskte olmayan hedefe "şurada" demek | **DUR** — `DOĞRULAMA GEREKLİ` (G9) |
| R4 | Frontmatter 8 alan dışında 9. alan | **DUR** — `[söz] 295` |
| R5 | `EntityFramework` / concat SQL geçen satır | **DUR** — G2 · G10 |
| R6 | Ölçüm çıktısı olmadan "tamam" beyanı | **DUR** — çıktı istenir (G9) |

### §6.3 Salt Okunur Doğrulama Komutu

```powershell
$f = 'C:\versacoder\.ai\CLAUDE.md'
'  satir : {0}   (300-500)' -f (Get-Content $f -Encoding UTF8).Count
'  KB    : {0:N1}' -f ((Get-Item $f).Length / 1KB)
'  ilk3  : {0}      (45,45,45 olmali)' -f (([System.IO.File]::ReadAllBytes($f))[0..2] -join ',')
$t = Select-String -LiteralPath $f -Pattern '\]\(([^)#]+)\)' -AllMatches |
     ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$k = $t | Where-Object { -not (Test-Path (Join-Path 'C:\versacoder\.ai' $_)) }
'  link  : {0} hedef · kiranik {1}  (0 olmali)' -f $t.Count, @($k).Count
$k
```

**Geçme kriteri:** satır 300–500 · ilk 3 bayt `45,45,45` · kırık bağlantı **0** · `DOĞRULAMA GEREKLİ` işaretli iddialar dışında kanıtsız sayı **0**.

---

## §7 Referanslar

### §7.1 Kaynaklar

| Dosya | Ne için | Durum (2026-10-06) |
|---|---|---|
| [kök CLAUDE](../CLAUDE.md) | §B4 okuma sırası · §B6 tek-satır özet · §B7 ne yap/ne yapma · §B9 bütçe | ✅ okundu |
| [templates index](.templates/index.md) | §3.7 frontmatter · §4.5 guardrail · §5.1 akış · §5.3 tespit · §6.1 checklist | ✅ okundu |
| [new-doc-contract](.templates/new-doc-contract.md) | yeni belge sözleşmesi · §1 8 alan · §9.1 denetim · §10.1 boyut · §11 red | ✅ okundu |
| [çakışma kaydı](reports/conflict-register-001-master-prompts.md) | CF-03 · CF-04 · CF-05/D4 · CF-10 · CF-13 | ✅ okundu |
| [adim4 raporu](reports/adim4-vault-skeleton-001.md) | `.ai/` taxonomisi (CF-12 · D3) | ✅ diskte |
| [WORKFLOW](../WORKFLOW.md) | kapı gerçeği — `GATE-003` (G8) | ✅ diskte |
| [AGENTS](../AGENTS.md) · [README](../README.md) | gölge dosya · tanım | ✅ diskte |
| `prompts/01-master-enterprise-engineering-v2.md` · `prompts/02-master-project-installation-v1.md` | planlama çerçevesi — vault'a uymayan yerde **vault kazanır** (`[çk] 73`) | ✅ diskte |
| `prompts/` · `reports/` · `.ai/` kök omurgası | AŞAMA C — `.rules/` `.decisions/` `architecture/` `checklists/` `scripts/` `index.md` `keys.md` `WORKFLOW.md` `log.md` `adr/` | ❌ YOK (`[env] 337–338`) |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|---|---|---|
| 1 | Kuralı kök dosyaya kopyalamak | iki gerçek kaynağı, biri çürür (G12) | tek satır özet + bağlantı (§1.1) |
| 2 | `.ai/` YOK hedefe bağlantı yazmak | kırık bağlantı = G7 ihlali | düz metin + `DOĞRULAMA GEREKLİ` (§3.3 Not) |
| 3 | Şablonda yer tutucu görünce kırmızı alarm | CF-04: şablonun doğası | 0 araması yalnız **üretim çıktısında** (G4) |
| 4 | `.templates/` dışı dosyaya 480 satır dayatmak | D4 kapsamı `.templates/**` | `prompts/` + `reports/` → 3–8 KB (G6) |
| 5 | Ölçüm çıktısız "bağlantılar sağlam" demek | kanıtsız beyan (G9) | §6.3 çıktısını yapıştır |

**SSUT beyanı:** Bu dosya, `.ai/` altındaki **G1..G12 kural metninin tek** kaynağıdır; kök `CLAUDE.md` yalnız bağlantı verir (G12), `.templates/CLAUDE.md` ayrı bir dizin kuralıdır, `.ai/` içine ikinci kök kural dosyası yazılmaz (G7).
