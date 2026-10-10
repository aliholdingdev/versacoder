---
title: "ADR Şablonu — Veri Yığını, Şema, SQL Migration ve schema_version Varyantı"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "veri katmani ADR sablonu: sema, SQL migration ve schema_version varyanti"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ADR Şablonu — Veri Yığını, Şema, SQL Migration ve `schema_version` Varyantı

**Zorunlu Bağlantılar:** [[.templates/adr/adr-template|adr-template]] · [[.templates/adr/adr-index|adr-index]] · [[index|Vault]] · [[architecture/index|ARCHITECTURE]] · [[.decisions/index|Karar Defteri]] · [[.rules/security-rules|security-rules]] · [[.rules/performance-guidelines|performance-guidelines]]

---

## §1 Amaç

### §1.1 Neden bu varyant gerekli

VersaCoder veri erişimi **ADO.NET + Dapper** ile yazılır; her sorgu `@p1..@pN` parameterized'tır; migration **saf SQL** + `schema_version` tablosudur; **EF Core yasaktır.** Bu dört kural bir ADR'de kanıtsız kalırsa sonraki geliştirici "normalde EF kullanılır" der ve şema sessizce bozulur. Bu varyant, veri kararını **SQL dosyası, tablo adı, sütun tipi ve sürüm satırı** düzeyinde kanıtlamak için vardır.

Zorunlu durumlar:

1. **Yığın kararı** — Dapper/ADO.NET, sağlayıcı (`Microsoft.Data.Sqlite` / `Microsoft.Data.SqlClient`), bağlantı stratejisi.
2. **Şema kararı** — tablo/sütun/index, FK, kısıt, seed verisi.
3. **Migration kararı** — saf SQL dosya sırası, `schema_version` satırı, geri alma planı.
4. **Sorgu stili kararı** — `@p1..@pN` kuralı, parametre paylaşımı, N+1 yasağı.

Kanıt olmadan `accepted` olamaz: **her iddia bir SQL dosyası / sürüm satırı / tarama çıktısı** ile desteklenir.

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|---|---|---|
| E1 | ADO.NET + Dapper, EF Core yasak | `.rules/` ve `.csproj` henüz yok | ⚠️ VERIFICATION REQUIRED |
| E2 | Her sorgu `@p1..@pN` parameterized | Migration/sorgu dosyaları yok | ⚠️ VERIFICATION REQUIRED |
| E3 | Migration = saf SQL + `schema_version` tablosu | Dizin yok | ⚠️ VERIFICATION REQUIRED |
| E4 | `Microsoft.Data.Sqlite` + `Microsoft.Data.SqlClient` | Paket referansı yok | ⚠️ VERIFICATION REQUIRED |
| E5 | Karar ADR-004 data-stack | `.decisions/` yok | ⚠️ VERIFICATION REQUIRED |
| E6 | Bu şablon yazıldı | `.ai/.templates/adr/adr-data-template.md` | ✅ Doğrulandı |
| E7 | Hedef coverage %90 + `TreatWarningsAsErrors` | Brief | ⚠️ VERIFICATION REQUIRED |

**YAGNI kararı:** SQL kanıtı olmayan "migration hazır" iddiası yazılmaz; satır `⚠️` kalır ve `§5.2` H1'e bağlanır. ADR'ye **gerçek SQL metni** yapıştırılmaz — dosya yolu + sürüm satırı verilir (G3: secret/connection string yasak).

### §1.3 Karar Türleri tablosu

| Karar Türü | Tipik Konu | Bu varyant? | Kanıt |
|---|---|---|---|
| Veri yığını | Dapper + ADO.NET, sağlayıcı | ✅ Evet | Paket referansı + örnek sorgu |
| Şema | Tablo/sütun/FK/kısıt | ✅ Evet | `migrations/*.sql` dosyası |
| Migration akışı | Sıra + `schema_version` | ✅ Evet | `schema_version` tek satır |
| Sorgu stili | `@p1..@pN`, N+1 | ✅ Evet | Tarama çıktısı = 0 concat |
| Katman sınırı / TFM | Proje referansı, `net48` | Hayır → `adr-architecture-template.md` | Bağımlılık matrisi |
| Secret / bağlantı dizesi | Parola saklama | Hayır → `adr-security-template.md` | `keys.md` kanıtı |
| DevExpress görünümü | Grid sütunu, tema | Hayır → `adr-devexpress-template.md` | Sürüm kilidi |
| Dosya adı / dizin düzeni | Log biçimi | Hayır → `adr-nygard-template.md` | 1 sayfa ADR |

### §1.4 Neden web araştırması zorunlu

Veri kararları blog tavsiyesiyle değil, davranış kanıtıyla savunulur:

1. **Sağlayıcı davranışı** — `Microsoft.Data.Sqlite` vs `Microsoft.Data.SqlClient` parametre adlandırma farkı (`@p1` ortak, tip desteği farklı) → paket sayfası.
2. **Dapper sürümü** — hedeflenen sürümün TFM desteği → paket/changelog.
3. **Yasak iddiası** — "EF Core bu projede yasak" → `.ai/.rules/` dosyası (yerel kaynak esastır).
4. **SQL uyumluluğu** — iki sağlayıcıda ortak sözdizimi (tipik: `INSERT`, `CREATE TABLE`) → resmi dil belgesi; fark yaratan ifadeler ADR'de listelenir.
5. **Sürüm/ürün adı** — `schema_version` bir ürün değil, yerel kural → uluslararası kaynak yerine yerel kural dosyası.

URL + erişim tarihi olmadan §1.4 satırı `§6.1` kutusundan düşer.

---

## §2 Kapsam

| Alan | Kapsam (BU ŞABLON) | Kapsam Dışı |
|---|---|---|
| Yığın | ADO.NET + Dapper, sağlayıcı seçimi | ORM alternatif değerlendirmesi (EF Core yasak) |
| Şema | Tablo, sütun, kısıt, FK, index, seed | Uygulama ekranı tasarımı |
| Migration | Saf SQL dosya sırası, `schema_version`, geri alma | DBA operasyon / backup politikası |
| Sorgu stili | `@p1..@pN`, parametre zorunluluğu, N+1 | Query performans ayarı (ayrı kural) |
| Seed / test verisi | Idempotent seed | Test senaryoları (testing rolü) |
| Bağlantı | Havuz, timeout, hata kodu | Secret saklama (ADR-security) |

### §2.1 Dosya Tipi → Sorumlu eşlemesi

| Dosya Tipi | Sorumlu Rol | Bu şablon? | Not |
|---|---|---|---|
| `migrations/*.sql` | `database-optimizer` | ✅ Evet | Saf SQL, sıralı |
| `schema_version` satırı | `database-optimizer` | ✅ Evet | Tek doğruluk kaynağı |
| Dapper sorguları (`*.cs`) | `developer` | ✅ Evet | `@p1..@pN` kanıtı |
| `.ai/.decisions/ADR-004-*.md` | `architect` | ✅ Evet | Bu şablonla yazılır |
| `.rules/coding-standards.md` | `reviewer` | Hayır | Kural ≠ karar |
| `keys.md` | `security` | Hayır | Bağlantı dizesi ADR'ye girmez |
| `.rules/performance-guidelines.md` | `performance-engineer` | Kısmen | N+1/eşik denetimi |

### §2.2 Kapsam Sınırı Testi (4 soru)

1. **S1 Şema, tablo, sütun, migration veya sorgu stili içeriyor mu?** → `EVET` beklenir.
2. **S2 Karar tek SQL dosyasıyla anlatılabiliyor mu?** → `HAYIR` beklenir (sığıyorsa Nygard hafif).
3. **S3 Kanıt dosya yolu + sürüm satırı üretilebiliyor mu?** → `EVET` beklenir.
4. **S4 Secret/connection string veya katman sınırı ana konu mu?** → `HAYIR` beklenir.

S1 `HAYIR` → `adr-nygard-template.md`. S4 `EVET` → yanlış varyant, **DUR**.

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

```yaml
---
title: "ADR-NNN — <ingilizce-slug>"
type: adr
category: data
status: draft            # draft | review | accepted | rejected | superseded
version: 0.1.0
updated: 2026-09-25
authority: primary
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H                  # şema değişikliği geri dönüşü pahalıdır
reversible: false
related_gates: [GATE-004]
related_rules: [coding-standards, security-rules]
template: adr-data-template.md
---
```

### §3.2 H1 iskeleti

```markdown
# ADR-NNN: <İngilizce karar başlığı>
**Zorunlu Bağlantılar:** [[.templates/index|.decisions/index]] · [[architecture/index|ARCHITECTURE]] · [[.rules/security-rules|security-rules]]
```

### §3.3 §1 İskeleti (Amaç)
```markdown
## §1 Amaç
### §1.1 Neden bu varyant gerekli        ← yığın/şema/migration gerekçesi
### §1.2 Disk kanıtları: iddia | kanıt (dosya yolu/sürüm) | durum
### §1.3 Karar türü ve seçilen varyant
### §1.4 Web araştırması: URL + erişim tarihi
```
### §3.4 §2 İskeleti (Kapsam)
```markdown
## §2 Kapsam  →  | Kapsam | Kapsam Dışı |
### §2.1 Dosya tipi → sorumlu
### §2.2 Kapsam sınırı testi: S1..S4
```
### §3.5 §3 İskeleti (Karar)
```markdown
## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar     ← "…karar verildi." ile biter
### §3.2 Migration tablosu: dosya | amaç | sürüm | geri alma
### §3.3 Şema özeti: tablo | sütun(tip) | kısıt | seed
### §3.4 Sorgu kuralı: @p1..@pN + sahipsiz parametre denetimi
```
### §3.6 §4 İskeleti (Sonuçlar)
```markdown
## §4 Kurallar ve Sonuçlar
### §4.1 Kazanımlar (ölçülebilir: dosya sayısı, sürüm satırı, 0 concat)
### §4.2 Ödenen bedel (elle SQL, iki sağlayıcı testi)
### §4.3 Guardrails → bu şablon §4.5
```
### §3.7 §5 İskeleti
```markdown
## §5 Workflow  →  §5.1 11 adım | §5.2 hata modları | §5.3 salt-okunur PowerShell
```
### §3.8 §6 İskeleti
```markdown
## §6 Doğrulama  →  §6.1 ≥18 kutu | §6.2 red sinyalleri | §6.3 sürüm | §6.4 report | §6.5 veri kontrolleri
```
### §3.9 §7 İskeleti
```markdown
## §7 Referanslar  →  §7.1 bağlantıklar | §7.2 Red Team (hata | neden yanlış | doğrusu)
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği (ADR-004 data-stack)

Dosya: `.ai/.decisions/accepted/ADR-004-data-stack.md`

```markdown
---
title: "ADR-004 — Data Stack: ADO.NET + Dapper, saf SQL migration"
type: adr
category: data
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: primary
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H
reversible: false
related_gates: [GATE-004]
related_rules: [coding-standards, security-rules]
template: adr-data-template.md
---
# ADR-004: Data Stack (ADO.NET + Dapper, `@p1..@pN`, `schema_version`)
**Zorunlu Bağlantılar:** [[.templates/index|.decisions/index]] · [[.rules/security-rules|security-rules]]

## §1 Amaç
### §1.1 Neden bu varyant gerekli
VersaCoder iki sağlayıcı kullanır: masaüstünde `Microsoft.Data.Sqlite` (yerel proje verisi),
CLI/otomasyonda `Microsoft.Data.SqlClient`. Ortak katmanda ORM istenmiyor; şemanın
sürüm sürüm ilerlemesi ve her sorgunun parameterized olması gerekiyor. Bu karar yığını,
migration biçimini ve `schema_version` tablosunu sabitler.
### §1.2 Disk kanıtları
| İddia | Kanıt | Durum |
|---|---|---|
| EF Core paket referansı = 0 | `.csproj` diskte yok (2026-09-25) | ⚠️ VERIFICATION REQUIRED |
| `migrations/0001_init.sql` … `0004_*` | Dizin yok | ⚠️ VERIFICATION REQUIRED |
| `schema_version` tablosu, tek satır | Dosya yok | ⚠️ VERIFICATION REQUIRED |
| Sorgularda `@p1..@pN` (concat = 0) | Tarama yapılamadı | ⚠️ VERIFICATION REQUIRED |
### §1.3 Karar türü
Veri yığını + şema + migration → bu varyant (`adr-data-template.md`).
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim | İddia |
|---|---|---|---|
| Microsoft.Data.Sqlite paket sayfası | nuget.org | 2026-09-25 | TFM + parametre davranışı |
| Dapper depo sürüm notları | github.com | 2026-09-25 | Sürüm/TFM uyumu |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| Yığın: ADO.NET + Dapper | ORM/EF Core (yasak, değerlendirilmez) |
| Migration: saf SQL + `schema_version` | Backup/replica operasyonu |
| Sorgu: `@p1..@pN` zorunlu | UI'da veri sunumu (ADR-006) |
### §2.1 Dosya tipi → sorumlu
`migrations/*.sql` → `database-optimizer`; Dapper kodu → `developer`; denetim → `reviewer`.
### §2.2 Kapsam sınırı testi
S1 **EVET** (şema/migration). S2 **HAYIR** (tek dosyaya sığmaz). S3 **EVET** (dosya yolu + sürüm).
S4 **HAYIR** (secret katman konusu değil; bağlantı dizesi `keys.md`'de).

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
Veri erişimi ADO.NET + Dapper ile yazılır; EF Core paketi hiçbir projeye eklenmez; migration saf
SQL dosyalarıdır ve uygulama açılışında `schema_version` tablosuyla karşılaştırılarak sırayla
uygulanır; her sorgu `@p1..@pN` parameterized olmak zorundadır — yığın, migration biçimi ve
sorgu kuralı karar verildi.
### §3.2 Migration tablosu
| Dosya | Amaç | Sürüm | Geri alma |
|---|---|---|---|
| `0001_init.sql` | `sessions`, `settings` tabloları | 1 | `DROP` komutu dosyanın sonunda |
| `0002_agent_catalog.sql` | `agents` tablosu + FK | 2 | Geriye uyumlu (yalnız ekleme) |
| `0003_llm_providers.sql` | `providers` tablosu | 3 | Geriye uyumlu |
| `0004_settings_index.sql` | `settings(key)` unique index | 4 | `DROP INDEX` |
`schema_version` tablosu: `version INTEGER PRIMARY KEY, applied_at TEXT NOT NULL` — uygulama
başında tek sorgu ile okunur, eksik sürümler sırayla uygulanır.
### §3.3 Şema özeti
| Tablo | Sütunlar (tip) | Kısıt | Seed |
|---|---|---|---|
| `sessions` | `id TEXT`, `created_at TEXT` | PK `id` | Yok |
| `settings` | `key TEXT`, `value TEXT` | PK `key` | Varsayılan 7 satır |
| `agents` | `id TEXT`, `role TEXT`, `state TEXT` | FK yok, `role` CHECK | 14 rol (katalog) |
| `providers` | `id TEXT`, `kind TEXT`, `base_url TEXT` | PK `id` | `openai`, `ollama` |
Seed idempotent: `INSERT OR IGNORE` (Sqlite) / `IF NOT EXISTS` (SqlClient) dallanması `#if` ile değil,
sağlayıcıye özel küçük SQL parçasıyla ayrılır.
### §3.4 Sorgu kuralı
- Her sorgu: `SELECT ... WHERE key = @p1` biçiminde; parametre adları `@p1..@pN` sıralı.
- Concat yasağı: SQL metnine kullanıcı verisi birleştirilerek eklenemez (G7).
- Denetim: `Select-String` ile `"'" +`, `$"SELECT`, `string.Format` desenlerinin sayısı **0** olmalı.

## §4 Kurallar ve Sonuçlar (özet — §4.5/§5.1/§6.1 karşılıklarıyla yazılır)
- §4.1: 4 migration dosyası, 1 `schema_version` tablosu, 0 EF Core paketi, 0 concat sorgu.
- §4.2: elle SQL bakım maliyeti; iki sağlayıcıda ayrı test koşusu.
- §4.3: G1, G3, G5, G7 (`@p1..@pN`), G8 (`schema_version`), G9 (geri alma planı), G10, G11.
```

### §3.11 Kopyala-yapıştır tam kalıp (`{{PLACEHOLDER}}` alanlı)

```markdown
---
title: "{{ADR_ID}} — {{ingilizce-slug}}"
type: adr
category: data
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: primary
supersedes: {{ADR_ID|NULL}}
superseded_by: null
decision_date: {{YYYY-MM-DD}}
risk: H
reversible: false
related_gates: [GATE-004]
related_rules: [coding-standards, security-rules]
template: adr-data-template.md
---
# {{ADR_ID}}: {{INGILIZCE_KARAR_BASLIGI}}
**Zorunlu Bağlantılar:** [[.templates/index|.decisions/index]] · [[architecture/index|ARCHITECTURE]] · [[.rules/security-rules|security-rules]]

## §1 Amaç
### §1.1 Neden bu varyant gerekli
{{YIGIN/ŞEMA/MIGRATION GEREKÇESİ — tek paragraf}}
### §1.2 Disk kanıtları
| İddia | Kanıt (dosya yolu / sürüm satırı) | Durum |
|---|---|---|
| {{IDDIYA_1}} | {{YOL_1}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
| {{IDDIYA_2}} | {{YOL_2}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
### §1.3 Karar türü ve seçilen varyant
{{TUR}} → adr-data-template.md.
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim | İddia |
|---|---|---|---|
| {{KAYNAK}} | {{URL}} | {{YYYY-MM-DD}} | {{IDDIYA}} |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| {{KAPSAM}} | {{KAPSAM_DISI}} |
| {{KAPSAM_2}} | {{KAPSAM_DISI_2}} |
### §2.1 Dosya tipi → sorumlu
{{DOSYA_TIPI}} → {{ROL}}; {{DOSYA_TIPI_2}} → {{ROL_2}}.
### §2.2 Kapsam sınırı testi
1. S1 şema/migration/sorgu stili içeriyor mu? {{EVET|HAYIR}}.
2. S2 tek SQL dosyasıyla anlatılabiliyor mu? {{EVET|HAYIR}}.
3. S3 kanıt yolu + sürüm satırı üretilebiliyor mu? {{EVET|HAYIR}}.
4. S4 secret/katman ana konu mu? {{EVET|HAYIR}}.

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
{{YIGIN + MIGRATION BİÇİMİ + SORGU KURALI — "…karar verildi."}}
### §3.2 Migration tablosu
| Dosya | Amaç | Sürüm | Geri alma |
|---|---|---|---|
| `{{NNNN_{{slug}}.sql}}` | {{AMAÇ}} | {{N}} | {{GERI_ALMA}} |
`schema_version`: `{{DDL}}`.
### §3.3 Şema özeti
| Tablo | Sütunlar (tip) | Kısıt | Seed |
|---|---|---|---|
| `{{TABLO}}` | {{SUTUNLAR}} | {{KISIT}} | {{SEED|Yok}} |
### §3.4 Sorgu kuralı
- Parametre biçimi: `@p1..@pN` · Concat denetimi: {{TARAMA_DESENI}} → {{0|N → RED}}.

## §4 Kurallar ve Sonuçlar
### §4.1 Kazanımlar
- {{KAZANIM}} — {{ÖLÇÜT (0 concat, N dosya, N sürüm)}}
### §4.2 Ödenen bedel
- {{BEDEL}} — {{MALİYET}}
### §4.3 Guardrails
{{G_LISTESİ}} — tamı bu şablon §4.5.

## §5 Workflow
### §5.1 Uygulama adımları
| # | Adım | Girdi | Çıktı | Yapan |
|---|---|---|---|---|
| 1 | {{ADIM}} | {{GIRDI}} | {{CIKTI}} | {{ROL}} |
### §5.2 Hata modları
| Kod | Hata | Belirti | Düzeltme |
|---|---|---|---|
| H1 | {{HATA}} | {{BELIRTI}} | {{DUZELTME}} |
### §5.3 Salt-okunur doğrulama
~~~powershell
# 1 — {{AMAÇ}} olmalı: {{BEKLENEN}}
Select-String -Path "{{MIGRATION_DIZINI}}" -Pattern "@p1"
~~~

## §6 Doğrulama
### §6.1 Onay kutuları
- [ ] 01 Frontmatter 11 alan eksiksiz, authority: primary
- [ ] 02 §1–§7 bölümlerinin 7'si de mevcut
- [ ] 03–18 … (bu şablon §6.1 listesi)
### §6.2 Red sinyalleri
| Sinyal | Koşul | Aksiyon |
|---|---|---|
| RS-1 | {{KOŞUL}} | DUR + `review`'a dön |
### §6.3 Sürüm/supersede
{{MAJOR/MINOR/PATCH + zincir}}
### §6.4 Quality Report
{{§6.4 kalıp bloğu}}
### §6.5 Veri ek kontrolleri
{{EK-1..EK-5}}

## §7 Referanslar
### §7.1 VersaCoder bağlantıkları
| Bağlantı | Yol | Rol |
|---|---|---|
| {{AD}} | `{{YOL}}` | {{ROL}} |
### §7.2 Red Team — sık yapılan hatalar
| Hata | Neden yanlış | Doğrusu |
|---|---|---|
| {{HATA}} | {{NEDEN}} | {{DOĞRUSU}} |
```

---

## §4 Kurallar

### §4.1 Durum döngüsü

```text
  draft --yazıldı--> review --4. aşama--> accepted
     ^                  |                    |
     | düzeltme         | red sinyali         | yeni ADR eziyor
     +------------------+--------------------> rejected / superseded

  superseded: eski ADR silinmez; superseded_by alanına yeni ADR yazılır.
```

| Durum | Anlamı | Kim koyar | Çıkış koşulu |
|---|---|---|---|
| `draft` | SQL/migration taslağı | `database-optimizer` | §6.1 ≥18 kutu + migration tablosu |
| `review` | Şema denetiminde | `reviewer` | Concat taraması = 0 |
| `accepted` | Şema kuralı bağlayıcı | `master-orchestrator` | 4. aşama + defter + `schema_version` = N |
| `rejected` | Yığın/şema reddedildi | `master-orchestrator` | Gerekçe ≥3 satır |
| `superseded` | Yeni şema kararı ezdi | `docs-writer` | `superseded_by` dolu |

### §4.2 4 aşamalı onay akışı

| Aşama | Ad | Yapan | Girdi | Çıktı | Süre |
|---|---|---|---|---|---|
| 1 | Kanıt topla | `database-optimizer` | Migration dosyaları | §1.2 + §3.2 + §3.3 | ≤45 dk |
| 2 | Mimari denetim | `architect` | ADR | Katman/`Infrastructure` uyumu | ≤30 dk |
| 3 | SQL + güvenlik denetimi | `reviewer` + `security` | Revize ADR | Concat raporu + secret taraması | ≤45 dk |
| 4 | Kabul / red | `master-orchestrator` | §6.4 report | `accepted`/`rejected` + defter | ≤30 dk |

**Bütçe: 150 dakika.** Aşama 4'te `schema_version` hedef değeri defterle birlikte yazılır; yazılmayan ADR `draft` sayılır (G12).

### §4.3 Risk ölçeği L / M / H

| Seviye | Tanım | Veri eşikleri | Onay | Geri dönüş |
|---|---|---|---|---|
| **L** | Seed/satır verisi, sorgu metni düzeltmesi | `INSERT` seed, yorum satırı | Aşama 1–2 | Dakika |
| **M** | Yeni sütun (nullable), index ekleme | `0004_*.sql` gibi geriye uyumlu | Aşama 1–3 | Saat |
| **H** | Tablo silme/yeniden adlandırma, kısıt değişikliği, yığın değişimi | `DROP`, EF Core alternatifi | 1–4 + `related_gates` | Gün (veri taşıma) |

### §4.4 Frozen / değişiklik kontrolü (2 istisna)

`accepted` veri ADR'si frozen'dır. Yalnız 2 istisna:

| İstisna | Koşul | Yapılabilir | Yasak |
|---|---|---|---|
| **1 — Kanıt/tip düzeltmesi** | Sütun tipi yazım hatası, kırık link, `⚠️`'nin kanıtla kapanması | `version.patch` + `updated` + §7.1 revizyon satırı | Yığın, migration biçimi, sorgu kuralı değişikliği |
| **2 — Yeni şema sürümüyle supersede** | Yeni migration kararı eskiyi ezdi | Yeni ADR; eski `superseded` + `superseded_by` | `accepted` dosyayı düzenlemek, `schema_version` geçmişini silmek |

Migration dosyaları **asla silinmez**; revizyon yeni `NNNN_*.sql` dosyasıdır.

### §4.5 Hard Guardrails tablosu (G1..G14)

| # | Guardrail | Kural | İhlal Sonucu |
|---|---|---|---|
| G1 | Frontmatter SSOT | `authority: primary` + 11 alan eksiksiz | Aşama 1 **DUR**; `draft` |
| G2 | Slug İngilizce, metin Türkçe | mojibake yasak | Commit geri alınır |
| G3 | Secret sızıntısı yasak | Connection string/parola ADR'ye girmez; `keys.md` yalnız varlık kanıtı | **DUR** + `security`; `rejected` |
| G4 | EF Core yasak | EF Core/ORM öneren karar geçersizdir | **RED**: ADR `rejected`, ADR-004'e çelişki kaydı |
| G5 | Kanıt zorunlu | Migration dosya yolu + `schema_version` satırı olmadan `accepted` olunamaz | `⚠️` + H1; <3 kanıt → `draft` |
| G6 | Kopyalama yasak | CoreMusic / vs-aivault `.ai` içeriği kopyalanamaz | ADR **reddedilir** |
| G7 | `@p1..@pN` zorunlu | SQL metnine veri birleştirilerek eklenemez (string concat) | **RED**: concatenation tespiti → Aşama 3 **DUR** |
| G8 | `schema_version` tek doğruluk | Her migration bu tabloya yazılır; elle sürüm değişimi yasak | Sürüm uyuşmazlığı → açılış reddedilir, ADR `draft` |
| G9 | Geri alma planı | Her migration dosyasının geri alma komutu §3.2'de yazılı | Plan yoksa Aşama 4 **DUR** |
| G10 | Coverage korunur | Veri katmanı coverage %90 altına düşemez | Job kırmızı; ADR `review` |
| G11 | TreatWarningsAsErrors | Yeri kod uyarısız derlenmeli | Derleme kırılırsa karar uygulanamaz |
| G12 | Defter zorunlu | `accepted` → `index.md` + hedef `schema_version` değeri | Eksik kayıt → fiilen `draft` |
| G13 | Supersede zinciri | Ezilen ADR `superseded` + `superseded_by`; silinmez | Zincir koparsa defter denetimi **DUR** |
| G14 | Salt-okunur doğrulama | §5.3 yalnız `Get-*`/`Select-String`/`Test-Path` | Yazma komutlu blok silinir |

---

## §5 Workflow

### §5.1 11 adımlı tablo + zaman bütçesi

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Karar anını yakala | GATE-004 / `log.md` | "ADR gerekli" | `master-orchestrator` | 5 dk |
| 2 | Varyant seç | §1.3 + §2.2 | Bu şablon | `architect` | 5 dk |
| 3 | Numara tahsis et | `.decisions/index.md` | `ADR-NNN` | `docs-writer` | 5 dk |
| 4 | Migration dosyalarını tara | `migrations/*.sql` | Dosya + sürüm listesi | `database-optimizer` | 20 dk |
| 5 | Şema özeti çıkar | DDL metinleri | §3.3 tablosu | `database-optimizer` | 20 dk |
| 6 | Concat taraması | §5.3 komut 2 | 0 sonuç | `developer` | 10 dk |
| 7 | Web kaynağı + EF taraması | Paket sayfası | §1.4 + paket=0 | `researcher` | 20 dk |
| 8 | Karar + sonuçlar | Kanıtlar | §3.1–§3.4, §4.1–§4.3 | `architect` | 25 dk |
| 9 | Doğrula | §6.1 kutular | İşaretli liste | `reviewer` | 15 dk |
| 10 | Quality Report | §6.4 kalıp | `report` bloğu | `reviewer` | 10 dk |
| 11 | Kabul + defter | Report | `accepted` + `index.md` | `master-orchestrator` | 15 dk |

**Bütçe: 150 dakika.** 150 dk aşılırsa karar ikiye bölünür: ADR-NNN (yığın) + ADR-NNN+1 (şema).

### §5.2 Hata Modları tablosu

| Kod | Hata modu | Belirti | Kurtarma | Guardrail |
|---|---|---|---|---|
| H1 | Kanıtsız şema iddiası | §3.2/§3.3 boş | Migration dosyasını göster, `draft`'a dön | G5 |
| H2 | String concat SQL | `"SELECT " + sql` / `$"WHERE"` deseni | Sorguyu `@p1..@pN` ile yeniden yaz | G7 |
| H3 | EF Core önerisi | ADR'de `DbContext`, `migration add` geçiyor | **RED**; ADR-004 çelişki kaydı | G4 |
| H4 | `schema_version` uyuşmazlığı | Dosya sürümü ≠ tablo satırı | Tabloyu oku, sırayı yeniden kur | G8 |
| H5 | Geri alma plansız migration | §3.2 "Geri alma" hücresi boş | Geri komutu yaz; yoksa Aşama 4 DUR | G9 |
| H6 | İki sağlayıcıda farklı sözdizimi | Sqlite yeşil, SqlClient kırmızı | Sağlayıcıye özel SQL parçası ayır | G5 |
| H7 | Coverage düşüşü | %90 altı | Veri katmanı testi ekle; `review` | G10 |
| H8 | Frozen ihlili | `accepted` migration dosyası düzenlendi | Değişikliği geri al; yeni `NNNN` yaz | §4.4 |
| H9 | Secret sızıntısı | Bağlantı dizesi metinde | Metni temizle; `keys.md`'ye taşı | G3 |

### §5.3 Salt-okunur PowerShell komut bloğu (6 komut)

```powershell
# 1 — Migration dosyası sayısı ne olmalı: 0 değil, sıralı NNNN_*.sql
(Get-ChildItem -Path ".\migrations" -Filter "*.sql" -ErrorAction SilentlyContinue).Count

# 2 — String concat sorgu sayısı ne olmalı: 0 (G7 ihlali)
(Select-String -Path ".\**\*.cs" -Pattern 'string\.Format\(|"\s*\+\s*(sql|query)' -ErrorAction SilentlyContinue).Count

# 3 — EF Core paket referansı ne olmalı: 0 (G4)
(Select-String -Path ".\**\*.csproj" -Pattern "Microsoft.EntityFrameworkCore" -ErrorAction SilentlyContinue).Count

# 4 — schema_version tablosu geçiyor mu olmalı: en az 1 eşleşme
(Select-String -Path ".\migrations\*.sql" -Pattern "schema_version" -ErrorAction SilentlyContinue).Count

# 5 — Kullanılmayan @pN parametresi ne olmalı: 0
(Select-String -Path ".\**\*.cs" -Pattern "@p9" -ErrorAction SilentlyContinue).Count

# 6 — Bu şablonun satır sayısı ne olmalı: 480-660
(Get-Content -Path ".\.ai\.templates\adr\adr-data-template.md").Count
```

Blok salt-okunurdur (G14): `Set-`, `Remove-`, `New-`, `Invoke-Sqlcmd` yoktur. Migration uygulama işlemi ayrı, onaylı bir iş akışında yapılır.

---

## §6 Doğrulama

### §6.1 Onay kutusu listesi (18 madde)

- [ ] 01 — Frontmatter 11 alan eksiksiz (`title…template`).
- [ ] 02 — `authority: primary` birebir yazılı.
- [ ] 03 — `status` ∈ {draft, review, accepted, rejected, superseded}.
- [ ] 04 — H1 `ADR-NNN:` ile başlıyor.
- [ ] 05 — `**Zorunlu Bağlantılar:**` satırı var, göreli wiki-link.
- [ ] 06 — §1…§7 bölümlerinin 7'si de mevcut.
- [ ] 07 — §1.2 her satırda `✅` ya da `⚠️ VERIFICATION REQUIRED`.
- [ ] 08 — §1.4'te URL **ve** erişim tarihi var.
- [ ] 09 — §2.2 dört soru cevaplanmış (S1 EVET, S4 HAYIR).
- [ ] 10 — §3.2 migration tablosu: dosya, amaç, sürüm, geri alma dolu.
- [ ] 11 — §3.3 şema tablosunda tip + kısıt + seed sütunları dolu.
- [ ] 12 — §3.4'te `@p1..@pN` kuralı ve concat denetimi yazılı.
- [ ] 13 — §4.1 kazanımları sayısal (dosya/sürüm/0 concat).
- [ ] 14 — §4.5 G1..G14 = 14 satır, "İhlal Sonucu" dolu.
- [ ] 15 — §5.1 11 adım + 150 dk bütçe.
- [ ] 16 — §5.2 hata modu ≥7 satır.
- [ ] 17 — §5.3 6 salt-okunur komut, "— N olmalı" yorumlu.
- [ ] 18 — §6.5 veri kontrolleri (EK-1..EK-5) doldurulmuş.
- [ ] 19 — §7.2 ≥10 satır, "doğrusu" dolu.
- [ ] 20 — Dosya adı `ADR-NNN-<slug>.md`; `risk: H` ise `related_gates` dolu.

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| Sinyal | Tetikleyici | Sonuç | Sorumlu |
|---|---|---|---|
| RS-1 | `EFCore`, `DbContext`, `AddDbContext` geçen metin | **RED** (G4) + `security`/`architect` | `architect` |
| RS-2 | Concat deseni (`string.Format`, `$"SELECT`, `" +`) | Aşama 3 **DUR** (G7) | `reviewer` |
| RS-3 | §3.2'de geri alma hücresi boş | Aşama 4 **DUR** (G9) | `database-optimizer` |
| RS-4 | Connection string / parola metinde | **DUR** + `security`; `rejected` | `security` |
| RS-5 | `schema_version` hedefi yazılmamış | Aşama 4 **DUR** (G12) | `master-orchestrator` |
| RS-6 | 3. bir sağlayıcı/ORM adı geçiyor | Aşama 1 **DUR** | `architect` |
| RS-7 | Mojibake (bozuk kodlama artıkları) | Commit **DUR** | `reviewer` |

### §6.3 Sürüm / supersede kuralları tablosu

| Değişiklik | version | status | Ek işlem |
|---|---|---|---|
| DDL yazım hatası, kırık link | `patch` | Aynı | §7.1 revizyon satırı |
| Aynı şemaya yeni sütun/index kanıtı | `minor` | `review` → `accepted` | §6.4 rapor yenilenir |
| Yığın veya migration biçimi değişikliği | **Yeni ADR** | Eski → `superseded` | `schema_version` hedefi güncellenir |
| EF Core yasağı ihlal iddiası | Yeni ADR | Yeni: `rejected` | ADR-004 çelişki kaydı |
| Şablonun kendisi | Şablon `minor` | `active` | Doğrulanmış ADR'ler dokunulmaz |

### §6.4 Quality Report (markdown bloğu)

```markdown
## Quality Report — {{ADR_ID}}
- **Tarih:** {{YYYY-MM-DD}} | **Şablon:** adr-data-template.md v{{X.Y.Z}}
- **Durum:** {{…}} → hedef: accepted | **Risk:** {{L|M|H}} | **Reversible:** {{true|false}}
- **Yığın:** ADO.NET + Dapper | **Sağlayıcı:** Sqlite={{N}}, SqlClient={{N}}
- **Migration:** {{N}} dosya | **schema_version hedefi:** {{N}} | **EF Core paketi:** {{0|N → RED}}
- **Concat taraması:** {{0|N → RED}} | **Onay kutusu:** {{NN}}/20 → {{YETERLİ|YETERSİZ}}
- **Kanıt:** ✅ {{N}}, ⚠️ {{N}} → {{KANIT_YETERLI|DEGIL}}
- **Web kaynağı:** {{N}} URL + tarih → {{TAM|EKSIK}}
- **Red sinyali:** {{YOK|RS-N}} | **Guardrail ihlali:** {{YOK|G-N → İhlal Sonucu}}
- **Defter:** `.decisions/index.md` ADR-NNN → {{VAR|YOK}}
- **Karar özeti (1 cümle):** {{NE YIGIN, NEDEN, NEYE MAL OLDU}}
- **Sonraki adım:** {{ROL + 1 eylem + süre}}
```

### §6.5 Varyant özgü ek kontroller (Veri)

| # | Kontrol | Doğru koşul | Yanlışsa |
|---|---|---|---|
| EK-1 | Şema senkronu | §3.3 tabloları migration dosyalarıyla birebir | `draft`; şemayı dosyadan yeniden üret |
| EK-2 | Concat = 0 | §5.3 komut 2 çıktısı 0 | G7 → **RED**, sorgular yeniden yazılır |
| EK-3 | `schema_version` tek satır | Uygulama açılışında 1 sorgu ile okunur | G8 ihlali → `draft` |
| EK-4 | Seed idempotent | Aynı migration iki kez çalışsa da veri çoğalmaz | Seed'ı `IF NOT EXISTS` biçimine getir |
| EK-5 | FK/index kanıtı | §3.3'te kısıt ve index satırı var | Eksik kanıt → §6.1 #11 düşer |

---

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Bağlantı | Göreli Yol | İlişki |
|---|---|---|
| Vault ana sayfa | `index.md` | Envanterin üst kaynağı |
| Mimarî | `architecture/index.md` | Veri katmanının yeri (`Infrastructure`) |
| Karar defteri | `.decisions/index.md` | Numara + supersede + `schema_version` hedefi |
| Altın şablon | `.templates/adr/adr-template.md` | Uzun varyant |
| Kodlama standartları | `.rules/coding-standards.md` | G7 sorgu stili |
| Güvenlik kuralları | `.rules/security-rules.md` | G3 secret denetimi |
| Performans | `.rules/performance-guidelines.md` | N+1/eşik denetimi |
| Anahtar sözlüğü | `keys.md` | Bağlantı dizesi varlık kanıtı (içerik girmez) |
| Gate planı | `WORKFLOW.md` | `related_gates: [GATE-004]` |
| Veri ADR | `.decisions/ADR-004-data-stack.md` | Bu şablonun ana adayı |
| Mimari ADR | `.decisions/ADR-002-solution-tier-scope.md` | `Infrastructure` sınırı |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Neden Yanlış | Doğrusu |
|---|---|---|---|
| 1 | "EF Core ile daha hızlı olur" | G4/ADR-004 çelişkisi; yasak kural | ADO.NET + Dapper; gerekçe ADR-004 |
| 2 | SQL'i string birleştirme | Enjeksiyon yüzeyi + plan önbelleği kırılır | `@p1..@pN` (G7) |
| 3 | Migration dosyasını silip yeniden yazmak | Geçmiş kaybolur, `schema_version` şaşar | Yeni `NNNN_*.sql`, eskisi kalır |
| 4 | `schema_version`'ı elle artırmak | Uygulama ile veritabanı ayrışır | Migration sırası tabloyu yazar (G8) |
| 5 | Geri alma planı yazmamak | Hatalı migration geri alınamaz | §3.2'de geri komutu zorunlu (G9) |
| 6 | ADR'ye connection string yapıştırmak | Repo + secret = sızıntı (G3) | `keys.md`'ye referans, içerik yok |
| 7 | Şemayı ADR'de değil kodda tanımlamak | İki SSOT doğar | Şema §3.3 + migration dosyası |
| 8 | Seed'i idempotent yazmamak | Her açılışta satır çoğalır | `IF NOT EXISTS` / `INSERT OR IGNORE` |
| 9 | İki sağlayıcıyı tek SQL ile denemek | Sözdizimi farkı kırılır | Sağlayıcıye özel parça + iki test koşusu |
| 10 | Kanıtsız "migration hazır" | G5 ihlali | Dosya yolu + sürüm satırı + tarama çıktısı |
| 11 | Şema değişikliğini Nygard hafifle yazmak | H risk, 1 sayfa savunulmaz | S1 `EVET` → bu varyant zorunlu |
