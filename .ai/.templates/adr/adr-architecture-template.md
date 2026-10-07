---
title: "ADR Şablonu — Mimari / Katman Sınırı, Bağımlılık Akışı ve TFM Varyantı"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "mimari/katman siniri ADR sablonu: baglam, karar, bagimlilik akisi ve TFM varyanti"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ADR Şablonu — Mimari / Katman Sınırı, Bağımlılık Akışı ve TFM Varyantı

**Zorunlu Bağlantılar:** [adr-template](adr-template.md) · [adr-nygard](adr-nygard-template.md) · [Vault](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [Karar Defteri](../../.decisions/index.md) · [solid-rules](../../.rules/solid-rules.md) · [coding-standards](../../.rules/coding-standards.md)

---

## §1 Amaç

### §1.1 Neden bu varyant gerekli

VersaCoder katmanları sabittir: `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI` + `CrossCutting` + `Cli`. Tek bir ters `ProjectReference` mimariyi çürütür — derleme yine yeşil olduğu için kimse fark etmez. Katman sınırı, bağımlılık akışı ve TFM kararı bu yüzden tek sayfaya sığdırılamaz: kanıt olarak bağımlılık matrisi ve TFM matrisi gerekir.

Bu varyant şu durumlarda zorunludur:

1. **Proje/katman sınırı** değişiyor (yeni proje, birleştirme, ayırma, `CrossCutting`'e taşınma).
2. **Bağımlılık akışı** tanımlanıyor veya ihlal şüphesi var (UI → Infrastructure gibi ters referans).
3. **TFM kararı** (`net48` + `net10.0-windows`) veya koşullu derleme alanı (`#if`) genişliyor.
4. **Uygulama sınırı** (`VersaCoder.Desktop` WinForms `RibbonForm` vs `VersaCoder.Cli`) netleştiriliyor.

Yüksek riskli (H) ve çoğunlukla `reversible: false` olan bu kararlar 4 aşamalı onaydan geçer (§4.2) ve `related_gates` alanında gate bağı taşır (GATE-004 mimari).

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır.

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|---|---|---|
| E1 | 8 katman sırası `Abstractions → … → UI` | `.ai/architecture/index.md` yok | ⚠️ VERIFICATION REQUIRED |
| E2 | `VersaCoder.Desktop` = WinForms `RibbonForm` | `.csproj`/`Form` görülmedi | ⚠️ VERIFICATION REQUIRED |
| E3 | TFM `net48` + `net10.0-windows` | `Directory.Build.props` yok | ⚠️ VERIFICATION REQUIRED |
| E4 | `CrossCutting` ve `Cli` ayrı katman | Proje ağacı yok | ⚠️ VERIFICATION REQUIRED |
| E5 | Gate `GATE-004 architecture` | Brief; `.ai` kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E6 | Bu şablon yazıldı | `.ai/.templates/adr/adr-architecture-template.md` | ✅ Doğrulandı |
| E7 | EF Core yasak; ADO.NET + Dapper | `.rules/` kurulmadı | ⚠️ VERIFICATION REQUIRED |

**YAGNI kararı:** Bağımlılık matrisi üretilmeden "katman doğru" iddiası yazılmaz. Matris yoksa satır `⚠️` kalır ve `§5.2` H1'e bağlanır.

### §1.3 Karar Türleri tablosu

| Karar Türü | Örnek Konu | Bu varyant? | Çıktı Kanıtı |
|---|---|---|---|
| Proje/katman sınırı | Yeni `VersaCoder.Protocol` | ✅ Evet | Proje + `ProjectReference` matrisi |
| Bağımlılık akışı | `Host → Application` tek yön kuralı | ✅ Evet | Ters referans taraması = 0 |
| TFM / koşullu derleme | `net48` + `net10.0-windows`, `#if` alanı | ✅ Evet | 2 hedef yeşil |
| Uygulama sınırı | Desktop (UI) vs Cli ayrımı | ✅ Evet | Hangi proje hangi uygulamaya referans verir |
| Veri yığını / şema / migration | Dapper, `schema_version` | Hayır → `adr-data-template.md` | SQL kanıtı |
| Secret / auth / oturum | `keys.md`, API anahtarları | Hayır → `adr-security-template.md` | Threat model |
| DevExpress sürüm/kontrol | 25.2.8, Office2010 | Hayır → `adr-devexpress-template.md` | Sürüm kilidi |
| Dosya/dizin adı | Log biçimi | Hayır → `adr-nygard-template.md` | 1 sayfa ADR |

### §1.4 Neden web araştırması zorunlu

Katman kararları "iyi pratik" iddiasıyla savunulur; iyi pratik iddiası kaynaqsız yazılamaz:

1. **TFM uyumu** — DevExpress 25.2.8 + `net48`/`net10.0-windows` iddiası → resmi sürüm notu; yoksa `⚠️ VERIFICATION REQUIRED`.
2. **Platform davranışı** — `net48` üzerinde WinForms, `net10.0-windows` üzerinde aynı kodun davranışı → resmi belge.
3. **Paket uyumu** — Dapper / `Microsoft.Data.SqlClient` hangi TFM'leri hedefliyor → paket sayfası.
4. **Bağımlılık kuralı** — "UI alt katmana referans vermez" universally doğru; buna karşı çıkan istisna iddiası (varsa) kaynak ister.
5. **Versiyon tarihi** — TFM adlarının resmî yazımı (TFM moniker) → Microsoft belgesi.

Sonuç `§7.1`'e URL + erişim tarihiyle girer; tarihsiz satır `§6.1` kutusundan düşer.

---

## §2 Kapsam

| Alan | Kapsam (BU ŞABLON) | Kapsam Dışı |
|---|---|---|
| Proje/katman sınırı | Proje listesi, `ProjectReference` yönü, istisnalar | İç sınıf tasarımı (SOLID ayrı) |
| Bağımlılık akışı | Katmanlar arası tek yön kuralı + ters referans taraması | Paket içi bağımlılıklar |
| TFM | `net48`, `net10.0-windows`, `#if` alanı, CI matrisi | Tek hedefe düşürme kararı |
| Uygulama sınırı | Desktop vs Cli | ✅ Evet | Proje→uygulama eşlemesi |
| Test stratejisi | Katman testi + coverage etkisi (%90) | Test yazımı ayrı iş kalemi |
| DevExpress | Sürüm/TFM etkileşimi (kilit ADR-001'dir) | Stil/kontrol seçimi |

### §2.1 Dosya Tipi → Sorumlu eşlemesi

| Dosya Tipi | Sorumlu Rol | Bu şablon? | Not |
|---|---|---|---|
| `*.sln`, `*.csproj`, `Directory.Build.props` | `architect` | ✅ Evet | TFM ve `ProjectReference` kaynağı |
| `.ai/architecture/index.md` | `architect` | ✅ Evet | Bu ADR'den türetilir, çakışmamalı |
| `.ai/.decisions/ADR-NNN-*.md` | `architect` | ✅ Evet | Bu şablonla yazılır |
| `CrossCutting/` ve `Cli/` kodu | `developer` | Kısmen | Yalnız sınır ihlali kanıtı |
| `.rules/solid-rules.md` | `reviewer` | Hayır | Kural ≠ karar |
| `REQUIREMENTS.md` | `business-analyst` | Hayır | Gereksinim mimariyi dayatmaz |
| CI matrisi / pipeline | `devops-engineer` | Kısmen | TFM matrisi kanıtı sağlar |

### §2.2 Kapsam Sınırı Testi (4 soru)

1. **S1 Proje/katman sınırı veya referans yönü içeriyor mu?** → `EVET` beklenir.
2. **S2 Tek `ProjectReference` kararı tek paragrafa sığıyor mu?** → `HAYIR` beklenir (sığıyorsa Nygard hafif).
3. **S3 Bağımlılık matrisi üretilebiliyor mu?** → `EVET` beklenir (üretilemiyorsa kanıt yok, G5).
4. **S4 Veri şeması / secret / DevExpress stili ana konu mu?** → `HAYIR` beklenir (içeriyorsa ilgili varyant).

S1 `HAYIR` ise bu şablon fazladır → `adr-nygard-template.md`. S4 `EVET` ise bu şablon **yanlış varyanttır**.

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

```yaml
---
title: "ADR-NNN — <ingilizce-slug>"
type: adr
category: architecture
status: draft            # draft | review | accepted | rejected | superseded
version: 0.1.0
updated: 2026-09-25
authority: primary
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H                  # mimari sınır genelde H
reversible: false
related_gates: [GATE-004]
related_rules: [solid-rules, coding-standards]
template: adr-architecture-template.md
---
```

### §3.2 H1 iskeleti

```markdown
# ADR-NNN: <İngilizce karar başlığı (slug ile aynı)>
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [ARCHITECTURE](../../architecture/index.md) · [solid-rules](../../.rules/solid-rules.md)
```

### §3.3 §1 İskeleti (Amaç)
```markdown
## §1 Amaç
### §1.1 Neden bu varyant gerekli        ← sınır/akış/TFM gerekçesi
### §1.2 Disk kanıtları: iddia | kanıt | durum (⚠️ VERIFICATION REQUIRED)
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
### §3.1 Seçilen karar  ← proje sınırı + referans yönü, "…karar verildi." ile biter
### §3.2 Bağımlılık matrisi: kaynak | hedef | yön | istisna
### §3.3 TFM matrisi: proje | net48 | net10.0-windows | koşul
### §3.4 Akış ASCII'si + ters referans taraması sonucu
```
### §3.6 §4 İskeleti (Sonuçlar)
```markdown
## §4 Kurallar ve Sonuçlar
### §4.1 Kazanımlar (ölçülebilir: proje sayısı, derleme sayısı)
### §4.2 Ödenen bedel (yeniden yazım maliyeti, CI süresi)
### §4.3 Guardrails → bu şablon §4.5
```
### §3.7 §5 İskeleti
```markdown
## §5 Workflow  →  §5.1 11 adım | §5.2 hata modları | §5.3 salt-okunur PowerShell
```
### §3.8 §6 İskeleti
```markdown
## §6 Doğrulama  →  §6.1 ≥18 kutu | §6.2 red sinyalleri | §6.3 sürüm | §6.4 report | §6.5 mimari kontroller
```
### §3.9 §7 İskeleti
```markdown
## §7 Referanslar  →  §7.1 bağlantıklar | §7.2 Red Team (hata | neden yanlış | doğrusu)
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği (ADR-002 solution-tier-scope)

Dosya: `.ai/.decisions/draft/ADR-002-solution-tier-scope.md`

```markdown
---
title: "ADR-002 — Solution Tier Scope: 9 proje, tek yön referans"
type: adr
category: architecture
status: draft
version: 0.1.0
updated: 2026-09-25
authority: primary
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H
reversible: false
related_gates: [GATE-004]
related_rules: [solid-rules, coding-standards]
template: adr-architecture-template.md
---
# ADR-002: Solution Tier Scope (`VersaCoder.sln` — 9 proje, tek yön referans)
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [ARCHITECTURE](../../architecture/index.md)

## §1 Amaç
### §1.1 Neden bu varyant gerekli
Çözüm, 2 uygulama (VersaCoder.Desktop — WinForms `RibbonForm`, VersaCoder.Cli) ve 7 katman
projesinden oluşuyor. Sınır tanımlanmazsa `UI` projesi doğrudan `Infrastructure`'e referans
verebilir; o zaman katman fiksi kalır, kod sağlam kalmaz. Bu karar, çözümün proje kapsamını ve
referans yönünü sabitler.
### §1.2 Disk kanıtları
| İddia | Kanıt | Durum |
|---|---|---|
| 9 proje (7 katman + CrossCutting + Cli) | `VersaCoder.sln` yok (2026-09-25) | ⚠️ VERIFICATION REQUIRED |
| TFM `net48` + `net10.0-windows` | `Directory.Build.props` yok | ⚠️ VERIFICATION REQUIRED |
| Test: xunit, hedef %90 coverage | Brief | ⚠️ VERIFICATION REQUIRED |
### §1.3 Karar türü
Proje sınırı + bağımlılık akışı → bu varyant (`adr-architecture-template.md`).
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim | İddia |
|---|---|---|---|
| DevExpress 25.2.8 sistem gereksinimleri | resmi sürüm notu | 2026-09-25 | 25.2.8 → `net48` dalı |
| .NET 10 SDK çoklu hedef | resmi belge | 2026-09-25 | `net10.0-windows` derlemesi |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| Proje listesi ve referans yönü | İç sınıf/interfaz tasarımı |
| TFM ataması (hangi proje hangi hedef) | DevExpress sürüm/kontrol seçimi (ADR-001) |
| Ters referans taraması | Veri erişim tekniği (ADR-004) |
### §2.1 Dosya tipi → sorumlu
`.sln`/`.csproj`/`Directory.Build.props` → `architect`; CI matrisi → `devops-engineer`; tarama → `reviewer`.
### §2.2 Kapsam sınırı testi
S1 **EVET** (proje sınırı). S2 **HAYIR** (tek paragrafa sığmaz → uzun varyant). S3 **EVET**
(bağımlılık matrisi üretilebilir). S4 **HAYIR** (şema/secret/stil ana konu değil).

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
`VersaCoder.sln` 9 projeyle sınırlandırılır; referans yalnız aşağı yönlüdür:
`Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`; `CrossCutting`
yalnız dışa bakar (hiçbir katman ona referans vermez, o hepsine referans verebilir); `Cli` → `Host`
bağlantısıyla kurulur, katmanlara doğrudan referans veremez — katman kapsamı ve yönü karar verildi.
### §3.2 Bağımlılık matrisi (kaynak → hedef)
| Kaynak | Hedef | Yön | İstisna |
|---|---|---|---|
| Domain | Abstractions | ↓ tek yön | Yok |
| Application | Domain, Abstractions | ↓ tek yön | Yok |
| Infrastructure | Application, Domain | ↓ tek yön | Yok |
| Protocol | Application, Abstractions | ↓ tek yön | Yok |
| Host | Protocol, Infrastructure | ↓ tek yön | Yok |
| UI (Desktop) | Host | ↓ tek yön | Yok |
| Cli | Host | ↓ tek yön | Yok |
| CrossCutting | hiçbiri | ↓ yalnız dışa | Yok |
Ters `ProjectReference` hedefi: **0**. Tarama §5.3 komut 3 ile üretilir.
### §3.3 TFM matrisi
| Proje | net48 | net10.0-windows | Koşul |
|---|---|---|---|
| Abstractions…Protocol (6 kütüphane) | ✅ | ✅ | Çift hedef |
| Host, CrossCutting | ✅ | ✅ | Çift hedef |
| UI (VersaCoder.Desktop) | ✅ | — | WinForms `RibbonForm` |
| Cli (VersaCoder.Cli) | — | ✅ | .NET 10 CLI davranışı |
### §3.4 Akış
`Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI` (+ `CrossCutting`, `Cli`)
CI matrisi: 2 TFM × 9 proje = 18 derleme; `TreatWarningsAsErrors=true`.

## §4 Kurallar ve Sonuçlar (özet — §4.5/§5.1/§6.1 karşılıklarıyla yazılır)
- §4.1: 0 ters referans; 18 yeşil derleme; katman ihlali derleme anında görünür.
- §4.2: her yeni proje CI süresine ~%11 ekler; `#if` alanı iki TFM arasında paylaşılmak zorunda.
- §4.3: G1, G5, G6, G7 (katman sırası), G8 (TFM sabiti), G9 (ters bağımlılık), G10, G11.
```

### §3.11 Kopyala-yapıştır tam kalıp (`{{PLACEHOLDER}}` alanlı)

```markdown
---
title: "{{ADR_ID}} — {{ingilizce-slug}}"
type: adr
category: architecture
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
related_rules: [solid-rules, coding-standards]
template: adr-architecture-template.md
---
# {{ADR_ID}}: {{INGILIZCE_KARAR_BASLIGI}}
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [ARCHITECTURE](../../architecture/index.md) · [solid-rules](../../.rules/solid-rules.md)

## §1 Amaç
### §1.1 Neden bu varyant gerekli
{{SINIR/AKIŞ/TFM GEREKÇESİ — tek paragraf}}
### §1.2 Disk kanıtları
| İddia | Kanıt (yol) | Durum |
|---|---|---|
| {{IDDIYA_1}} | {{YOL_1}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
| {{IDDIYA_2}} | {{YOL_2}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
### §1.3 Karar türü ve seçilen varyant
{{TUR}} → adr-architecture-template.md.
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
1. S1 proje/katman sınırı veya referans yönü içeriyor mu? {{EVET|HAYIR}}.
2. S2 tek paragrafa sığıyor mu? {{EVET|HAYIR}}.
3. S3 bağımlılık matrisi üretilebiliyor mu? {{EVET|HAYIR}}.
4. S4 şema/secret/stil ana konu mu? {{EVET|HAYIR}}.

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
{{PROJE_SINIRI + REFERANS_YÖNÜ — "…karar verildi." ile biter}}
### §3.2 Bağımlılık matrisi
| Kaynak | Hedef | Yön | İstisna |
|---|---|---|---|
| {{KATMAN}} | {{HEDEF}} | ↓ tek yön | {{ISTISNA|Yok}} |
| {{KATMAN_2}} | {{HEDEF_2}} | ↓ tek yön | {{ISTISNA|Yok}} |
Ters `ProjectReference` hedefi: {{SAYI}}.
### §3.3 TFM matrisi
| Proje | net48 | net10.0-windows | Koşul |
|---|---|---|---|
| {{PROJE}} | {{✅|—}} | {{✅|—}} | {{KOSUL}} |
### §3.4 Akış
{{ASCII_AKIS}} · CI: {{N}} TFM × {{N}} proje = {{N}} derleme.

## §4 Kurallar ve Sonuçlar
### §4.1 Kazanımlar
- {{KAZANIM}} — {{ÖLÇÜT}}
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
Select-String -Path "{{CSPROJ}}" -Pattern "ProjectReference"
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
### §6.5 Mimari ek kontrolleri
{{EK-1..EK-4}}

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
| `draft` | Sınır taslağı yazılıyor | `architect` | §6.1 ≥18 kutu + bağımlılık matrisi |
| `review` | Mimari gözden geçirmede | `reviewer` | Ters referans = 0 |
| `accepted` | Katman kuralı bağlayıcı | `master-orchestrator` | 4. aşama + defter + `architecture/index.md` senkronu |
| `rejected` | Sınır reddedildi | `master-orchestrator` | Gerekçe ≥3 satır + alternatif |
| `superseded` | Yeni sınır eskisini ezdi | `docs-writer` | `superseded_by` dolu |

### §4.2 4 aşamalı onay akışı

| Aşama | Ad | Yapan | Girdi | Çıktı | Süre |
|---|---|---|---|---|---|
| 1 | Matrisleri üret | `architect` | Proje ağacı + `.csproj` | Bağımlılık + TFM matrisi | ≤45 dk |
| 2 | Mimari denetim | `reviewer` | Matrisler | Ters referans raporu | ≤45 dk |
| 3 | Test + güvenlik | `testing` + `security` | Revize ADR | Coverage etkisi + kural çelişkisi | ≤30 dk |
| 4 | Kabul / red | `master-orchestrator` | §6.4 report | `accepted`/`rejected` + defter | ≤30 dk |

**Toplam bütçe: 150 dakika.** Aşama 4'te `.ai/architecture/index.md` ile çelişki varsa karar `rejected` olur (defter değil, mimari doğru değildir — ikisi ADR'den türetilir).

### §4.3 Risk ölçeği L / M / H

| Seviye | Tanım | Mimari eşikleri | Onay | Geri dönüş |
|---|---|---|---|---|
| **L** | Tek `ProjectReference` ekleme | Test projesinin katmana referansı | Aşama 1–2 | Dakika |
| **M** | Proje adlandırma/sıralama | `CrossCutting` içeriği, CLI bayrakları | Aşama 1–3 | Saat |
| **H** | Katman sırası, TFM, uygulama sınırı | Yeni katman, `#if` alanı, referans yönü | 1–4 + GATE-004 | Gün–hafta (kod tabanı yeniden yazımı) |

Bu varyant **H riski için tasarlanmıştır**; L riskli karar buraya yazılmaz, `adr-nygard-template.md` kullanılır.

### §4.4 Frozen / değişiklik kontrolü (2 istisna)

`accepted` mimari ADR frozen'dır. Yalnız 2 istisna:

| İstisna | Koşul | Yapılabilir | Yasak |
|---|---|---|---|
| **1 — Matris düzeltmesi** | Matris hücresi/uydurma hatası | `version.patch` + `updated` + §7.1 revizyon satırı | Karar metni, referans yönü, TFM ataması |
| **2 — Yeni katmanla supersede** | Yeni proje/katman kararı eskisini ezdi | Yeni ADR; eski `superseded` + `superseded_by` | Eski dosyayı silmek, matrisi sessiz değiştirmek |

Unutulmamalı: `.ai/architecture/index.md` bu ADR'den **türetilir**; türetilen dosyayı düzeltmek ADR'yi düzeltmez.

### §4.5 Hard Guardrails tablosu (G1..G14)

| # | Guardrail | Kural | İhlal Sonucu |
|---|---|---|---|
| G1 | Frontmatter SSOT | `authority: primary` + 11 alan eksiksiz | Aşama 1 **DUR**; `draft` |
| G2 | Slug İngilizce, metin Türkçe | mojibake yasak | Commit geri alınır |
| G3 | Secret sızıntısı yasak | Bağlantı dizesi/anahtar ADR'ye girmez | **DUR** + `security`; `rejected` |
| G4 | İstisna gerekçeli | `CrossCutting`/`Protocol` istisnası 1 cümle gerekçe ister | Gerekçesiz istisna satırı **silinir** |
| G5 | Kanıt zorunlu | Matris olmadan "doğru" iddiası yazılamaz | `⚠️` + H1; <3 kanıt → `draft` |
| G6 | Kopyalama yasak | CoreMusic / vs-aivault `.ai` içeriği kopyalanamaz | ADR **reddedilir** |
| G7 | Katman sırası sabit | `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI` | Ters akış → Aşama 2 **DUR** |
| G8 | TFM sabiti | Yalnız `net48`, `net10.0-windows`; `#if` alanı §3.3'te | Farklı TFM derlenemez; `draft` |
| G9 | Ters bağımlılık = 0 | UI/Cli/CrossCutting → alt katman `ProjectReference` yasak | **DUR**; `.csproj` düzelmeden aşama 4 yok |
| G10 | Coverage korunur | Katman değişikliği %90 altına düşüremez | Job kırmızı; ADR `review` |
| G11 | TreatWarningsAsErrors | Taşınan kod uyarı hatasız derlenmeli | Derleme kırılırsa `draft` |
| G12 | Defter + mimari senkronu | `accepted` → `index.md` + `architecture/index.md` | Eksik senkron → ADR `draft` |
| G13 | Supersede zinciri | Ezilen ADR `superseded` + `superseded_by`; silinmez | Zincir koparsa defter denetimi **DUR** |
| G14 | Salt-okunur doğrulama | §5.3 yalnız `Get-*`/`Select-String`/`Test-Path` | Yazma komutlu blok silinir |

---

## §5 Workflow

### §5.1 11 adımlı tablo + zaman bütçesi

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Karar anını yakala | GATE-004 notları | "ADR gerekli" | `master-orchestrator` | 5 dk |
| 2 | Varyant seç | §1.3 + §2.2 | Bu şablon | `architect` | 5 dk |
| 3 | Numara tahsis et | `.decisions/index.md` | `ADR-NNN` | `docs-writer` | 5 dk |
| 4 | Proje ağacını çıkar | `.sln`/`.csproj` taraması | Proje listesi | `developer` | 20 dk |
| 5 | Bağımlılık matrisi | `ProjectReference` listesi | §3.2 | `architect` | 20 dk |
| 6 | TFM matrisi + kaynak | `.props` + belge | §3.3 + §1.4 | `researcher` | 20 dk |
| 7 | Ters referans taraması | §5.3 komut 3 | Sıfır sonuç | `reviewer` | 10 dk |
| 8 | Karar + sonuçlar | Matrisler | §3.1, §4.1–§4.3 | `architect` | 25 dk |
| 9 | Doğrula | §6.1 kutular | İşaretli liste | `reviewer` | 15 dk |
| 10 | Quality Report | §6.4 kalıp | `report` bloğu | `reviewer` | 10 dk |
| 11 | Kabul + defter + mimari | Report | `accepted` + `index.md` + `architecture/index.md` | `master-orchestrator` | 15 dk |

**Bütçe: 150 dakika** (§4.2 ile birebir). 150 dk aşılırsa karar H2'ye değil, parçalara bölünür: ADR-NNN (sınır) + ADR-NNN+1 (TFM).

### §5.2 Hata Modları tablosu

| Kod | Hata modu | Belirti | Kurtarma | Guardrail |
|---|---|---|---|---|
| H1 | Matrisiz karar | §3.2 tablosu boş veya "tümü doğru" | §5.3 komutlarla matrisi üret, `draft`'a dön | G5 |
| H2 | Ters `ProjectReference` | UI/Cli → alt katman referansı bulundu | `.csproj`'ten kaldır, derle, taramayı tekrarla | G9 |
| H3 | Tek TFM'ye düşme | Yalnız `net48` derleniyor | `Directory.Build.props` `TargetFrameworks` alanını ikiye çıkar | G8 |
| H4 | Katman sırası ihlali | `Domain → Application` gibi ters akış | Bağımlılığı interfaz (`Abstractions`) üzerinden taşı | G7 |
| H5 | Coverage düşüşü | %90 altı | Eksik katman testini ekle; ADR `review`'a dön | G10 |
| H6 | ADR/mimari çelişkisi | `architecture/index.md` farklı söylüyor | İkisini ADR'den türet; defter satırını ekle | G12 |
| H7 | Kanıtsız TFM iddiası | §1.4 boş | URL + tarih ekle; yoksa iddiayı kaldır | G5 |
| H8 | Frozen ihlali | `accepted` matris sessizce değişti | Değişikliği geri al; yeni ADR yaz | §4.4 |

### §5.3 Salt-okunur PowerShell komut bloğu (6 komut)

```powershell
# 1 — Proje sayısı ne olmalı: 9
(Get-ChildItem -Path "." -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue).Count

# 2 — Tanımlı TFM'ler ne olmalı: net48, net10.0-windows
Select-String -Path ".\Directory.Build.props" -Pattern "TargetFramework"

# 3 — Ters referans sayısı ne olmalı: 0
Select-String -Path ".\*.csproj" -Pattern "ProjectReference" -ErrorAction SilentlyContinue

# 4 — Çözüm dosyası var mı olmalı: True
Test-Path -Path ".\VersaCoder.sln"

# 5 — `netstandard2.0` geçen satır ne olmalı: 0
(Select-String -Path ".\.ai\decisions\*.md" -Pattern "netstandard2.0").Count

# 6 — Bu şablonun satır sayısı ne olmalı: 480-660
(Get-Content -Path ".\.ai\.templates\adr\adr-architecture-template.md").Count
```

Blok salt-okunurdur (G14): `Set-`, `Remove-`, `New-`, `Move-` yoktur. Yazma editör aracılığıyla yapılır.

---

## §6 Doğrulama

### §6.1 Onay kutusu listesi (18 madde)

- [ ] 01 — Frontmatter 11 alan eksiksiz (`title…template`).
- [ ] 02 — `authority: primary` birebir yazılı.
- [ ] 03 — `status` ∈ {draft, review, accepted, rejected, superseded}.
- [ ] 04 — H1 `ADR-NNN:` ile başlıyor.
- [ ] 05 — `**Zorunlu Bağlantılar:**` satırı var, göreli wiki-link.
- [ ] 06 — §1…§7 bölümlerinin 7'si de mevcut, silinmemiş.
- [ ] 07 — §1.2 her satırda `✅` ya da `⚠️ VERIFICATION REQUIRED`.
- [ ] 08 — §1.4'te URL **ve** erişim tarihi var.
- [ ] 09 — §2.2 dört soru cevaplanmış (S1 EVET, S4 HAYIR).
- [ ] 10 — §3.2 bağımlılık matrisi dolu, "İstisna" sütunu boş değil.
- [ ] 11 — §3.3 TFM matrisinde her proje için 2 sütun durumu var.
- [ ] 12 — §3.4'te ters referans hedefi yazılı (beklenen: 0).
- [ ] 13 — §4.1 kazanımları ölçülebilir (derleme/proje/test sayısı).
- [ ] 14 — §4.5 G1..G14 = 14 satır, "İhlal Sonucu" dolu.
- [ ] 15 — §5.1 11 adım + 150 dk bütçe.
- [ ] 16 — §5.2 hata modu ≥7 satır.
- [ ] 17 — §5.3 6 salt-okunur komut, her biri "— N olmalı" yorumlu.
- [ ] 18 — §6.5 mimari kontrolleri (EK-1..EK-5) doldurulmuş.

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| Sinyal | Tetikleyici | Sonuç | Sorumlu |
|---|---|---|---|
| RS-1 | Bağımlılık matrisi yok veya tek satır | Aşama 2 **DUR** | `architect` |
| RS-2 | Ters `ProjectReference` tespiti | Aşama 4 **DUR** + `.csproj` düzeltmesi | `reviewer` |
| RS-3 | §3.3'te 3. bir TFM adı geçiyor | Aşama 1 **DUR** | `architect` |
| RS-4 | ADR ile `architecture/index.md` çelişiyor | Aşama 4 **DUR** | `master-orchestrator` |
| RS-5 | `reversible: true` iken karar katman sırasını değiştiriyor | Aşama 4 **DUR** (G9/§4.3) | `architect` |
| RS-6 | Secret/connection string deseni metinde | **DUR** + `security`; `rejected` | `security` |
| RS-7 | Mojibake (bozuk kodlama artıkları) | Commit **DUR** | `reviewer` |

### §6.3 Sürüm / supersede kuralları tablosu

| Değişiklik | version | status | Ek işlem |
|---|---|---|---|
| Matris hücresi hatası, kırık link | `patch` | Aynı | §7.1 revizyon satırı |
| Aynı sınıra yeni istisna/kanıt | `minor` | `review` → `accepted` | §6.4 rapor yenilenir |
| Referans yönü veya TFM değişikliği | **Yeni ADR** | Eski → `superseded` | `architecture/index.md` yeniden türetilir |
| Kapı (gate) çıktısı değişikliği | `minor` + gate notu | Aynı | `related_gates` güncellenir |
| Şablonun kendisi | Şablon `minor` | `active` | Doğrulanmış ADR'ler dokunulmaz |

### §6.4 Quality Report (markdown bloğu)

```markdown
## Quality Report — {{ADR_ID}}
- **Tarih:** {{YYYY-MM-DD}} | **Şablon:** adr-architecture-template.md v{{X.Y.Z}}
- **Durum:** {{…}} → hedef: accepted | **Risk:** {{L|M|H}} | **Reversible:** {{true|false}}
- **Proje sayısı:** {{N}} | **TFM:** net48={{N}}, net10.0-windows={{N}}
- **Bağımlılık matrisi:** {{N}} satır | **Ters referans:** {{0|N → DUR}}
- **Onay kutusu:** {{NN}}/20 → {{YETERLİ|YETERSİZ}}
- **Kanıt:** ✅ {{N}}, ⚠️ {{N}} → {{KANIT_YETERLI|DEGIL}}
- **Web kaynağı:** {{N}} URL + tarih → {{TAM|EKSIK}}
- **Red sinyali:** {{YOK|RS-N}} | **Guardrail ihlali:** {{YOK|G-N → İhlal Sonucu}}
- **Senkron:** `index.md` + `architecture/index.md` → {{İKİSİ DE|EKSIK}}
- **Karar özeti (1 cümle):** {{NE SINIR, NEDEN, NEYE MAL OLDU}}
- **Sonraki adım:** {{ROL + 1 eylem + süre}}
```

### §6.5 Varyant özgü ek kontroller (Mimari)

| # | Kontrol | Doğru koşul | Yanlışsa |
|---|---|---|---|
| EK-1 | Katman listesi senkronu | §3.1'deki sıra `.ai/architecture/index.md` ile aynı | `draft` + mimari dosyayı ADR'den türet |
| EK-2 | TFM matrisi tam | Her proje için 2 sütun dolu (`✅`/`—`) | Eksik hücre → G8 ihlali |
| EK-3 | Ters referans = 0 | §5.3 komut 3 çıktısı beklenenle eşleşiyor | G9 → Aşama 4 DUR |
| EK-4 | İstisnalar gerekçeli | `CrossCutting`/`Protocol` istisnası en az 1 cümle gerekçe içerir | G4 ihlali → satır silinir |
| EK-5 | CI matrisi | 2 TFM × ilgili proje = yeşil derleme | G11/G10 ihlali → `draft` |

---

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Bağlantı | Göreli Yol | İlişki |
|---|---|---|
| Vault ana sayfa | `index.md` | Envanterin üst kaynağı |
| Mimarî | `architecture/index.md` | Bu ADR'den türetilir |
| Karar defteri | `.decisions/index.md` | Numara + supersede zinciri |
| Altın şablon | `.templates/adr/adr-template.md` | Uzun varyant |
| SOLID kuralları | `.rules/solid-rules.md` | §4.5 G4 denetimi |
| Kodlama standartları | `.rules/coding-standards.md` | §4.5 G2, G11 |
| Dağıtım | `.rules/deployment-guide.md` | TFM/CI matrisi |
| Gate planı | `WORKFLOW.md` | `related_gates: [GATE-004]` |
| DevExpress ADR | `.decisions/ADR-001-devexpress-version.md` | Sürüm/TFM çapraz bağ |
| Veri ADR | `.decisions/ADR-004-data-stack.md` | Infrastructure sınırı |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Neden Yanlış | Doğrusu |
|---|---|---|---|
| 1 | Matris yazmadan "katmanlar doğru" demek | Kanıtsız iddia (G5) | §5.3 komutlarla matris üret |
| 2 | UI → Infrastructure doğrudan referans | Katman fikri kalır, kod çürür | UI → Host → Infrastructure zinciri |
| 3 | `CrossCutting`'e herkesin referans vermesi | Gizli ters bağımlılık doğar | `CrossCutting` yalnız dışa bakar |
| 4 | Üçüncü bir TFM eklemek ("test için") | CI matrisi 2 katına çıkar, DevExpress doğrulanmaz | Yalnız `net48` + `net10.0-windows` (G8) |
| 5 | ADR ile `architecture/index.md`'yi ayrı yazmak | İki SSOT doğar, biri er geç yalan söyler | Mimari ADR'den türetilir (G12) |
| 6 | Kural dosyasını ADR'ye kopyalamak | Kural ≠ karar; ikisi ayrı güncellenir | `.rules/` dosyasına bağlantı ver |
| 7 | Kararı tek satıra indirip Nygard'a kaçmak | Sınır kararı 1 sayfa değil matris ister | S2 `HAYIR` → bu varyant zorunlu |
| 8 | Ters referansı "#if ile kapatırım" diye savunmak | Derleme çözümü değil, mimari ihlal | Referansı interfaz üzerinden yeniden kur |
| 9 | Coverage etkisini yazmamak | %90 hedefi sessizce düşer | §6.5 EK-5 zorunlu |
| 10 | `accepted` olup defteri/mimariyi unutmak | Karar bulunamaz, kod farklı yöne gider | G12: `index.md` + `architecture/index.md` eşzamanlı |
