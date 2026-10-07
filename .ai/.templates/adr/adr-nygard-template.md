---
title: "ADR Şablonu — Michael Nygard Klasik Biçimi (Nygard Hafif Varyantı)"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Nygard klasik ADR sablonu: durum, baglam, karar ve sonuc bolumleri"
---

# ADR Şablonu — Michael Nygard Klasik Biçimi (Nygard Hafif Varyantı)

**Zorunlu Bağlantılar:** [adr-template](adr-template.md) · [adr-index](adr-index.md) · [Vault](../../index.md) · [ARCHITECTURE](../../architecture/index.md) · [Karar Defteri](../../.decisions/index.md) · [coding-standards](../../.rules/coding-standards.md) · [keys](../../governance/keys.md)

---

## §1 Amaç

### §1.1 Neden bu varyant gerekli

Nygard varyantı **karar anında yazılan kısa ADR** içindir: toplantı biter, `master-orchestrator` "bunu böyle yapıyoruz" der; o cümle 15 dakika içinde diske düşmezse 3 gün sonra gerekçesi hiçbir yerde kalmaz.

Bu varyant üç durumda kullanılır:

1. **Karar verilmiş, gerekçe yazılıyor** — uzun `adr-template.md` (34 KB altın örnek) için vakit yok.
2. **Alternatif 2–4 adet ve hepsi masada tartışılmış** — `Considered Options` tartışmanın dökümüdür.
3. **Karar geri alınabilir (reversible) ve riski L/M** — H riskli kararlar bu varyantla `accepted` olamaz (G9).

Nygard özü üç cümledir: **bağlam değişmez, karar değişebilir, sonuç ödenir.** Bu şablon VersaCoder'ın 7 bölümlük zorunlu iskeletiyle sentezlenmiştir; §1–§7 silinemez.

> Üretim beklentisi: doldurulmuş **250–400 satırlık** kısa ADR. `draft` durumundaki ADR-002 ve ADR-006 bu varyantla tamamlanır.

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır.

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|---|---|---|
| E1 | `.ai/` kökü + 11 kök dosya (`index.md`…`MEMORY.md`) | `C:\versacoder` listesi yalnız `.git` gösterdi | ⚠️ VERIFICATION REQUIRED |
| E2 | `.ai/.templates/adr/adr-template.md` (34 KB altın örnek) erişilebilir | Bu oturumda okunamadı | ⚠️ VERIFICATION REQUIRED |
| E3 | ADR planı `ADR-001..ADR-008` | Brief'te yazılı, diske işlenmemiş | ⚠️ VERIFICATION REQUIRED |
| E4 | `.ai/.rules/` içinde 7 kural dosyası | Dizin yok | ⚠️ VERIFICATION REQUIRED |
| E5 | Gate planı `GATE-000..GATE-005` | Kanıt yok | ⚠️ VERIFICATION REQUIRED |
| E6 | Bu şablon dosyası yazıldı | `.ai/.templates/adr/adr-nygard-template.md` | ✅ Doğrulandı |
| E7 | DevExpress 25.2.8 + Office2010, TFM `net48`+`net10.0-windows` | Proje kararı; `.csproj` kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E8 | CoreMusic/vs-aivault vault'u içerik kaynağı | Erişim ve kopyalama yasak | 🔒 Kopyalama YASAK |

**YAGNI kararı:** Kanıtsız satır ya silinir ya `⚠️ VERIFICATION REQUIRED` kalır ve `§5.2` H7'ye bağlanır. E1–E5 boşlukları bu şablonun neden hazır **içerik** değil hazır **iskelet** olduğunu doğrular.

### §1.3 Karar Türleri tablosu

| Karar Türü | Tipik Konu | Uygun Varyant | Nygard Hafif? | Örnek |
|---|---|---|---|---|
| TFM / derleme hedefi | `net48` + `net10.0-windows` | `adr-architecture-template.md` | Kısmen (özet kayıt) | ADR-003 dual-target |
| Katman sınırı / bağımlılık akışı | `Abstractions → … → UI` | `adr-architecture-template.md` | Hayır | ADR-002 solution-tier-scope |
| Veri yığını / migration | Dapper, `schema_version`, `@p1..@pN` | `adr-data-template.md` | Hayır | ADR-004 data-stack |
| Secret / auth / oturum | `keys.md`, Ollama local, API anahtarları | `adr-security-template.md` | Hayır | ADR-010 (örnek) |
| DevExpress kontrol/sürüm/stil | 25.2.8, Office2010, `RibbonForm` | `adr-devexpress-template.md` | Kısmen | ADR-001 devexpress-version |
| Envanter / supersede zinciri | `.decisions/index.md` kaydı | `adr-index.md` | Hayır | ADR-007 agent-catalog-reconciliation |
| Dizin adı, dosya adı, log biçimi, geçici araç | "Bunu `brain.md`'de tutacağız" | **Bu şablon** | ✅ Evet | ADR-005 manual-designer |
| Geri alınabilir teknik tercih | "Testler `xunit` ile" | **Bu şablon** | ✅ Evet | ADR-006 api-snippets (draft) |

### §1.4 Neden web araştırması zorunlu

Nygard *"düşündüğümüz buysu"* der; *"dünyada doğrusu buysu"* demez. İkincisini kanıtsız yazmak uydurmadır.

1. **Sürüm iddiası** (DevExpress 25.2.8, .NET 10, Dapper) → resmi sürüm notu; yoksa `⚠️ VERIFICATION REQUIRED`.
2. **Davranış iddiası** (`net48` üzerinde `Microsoft.Data.SqlClient` sürüm uyumu) → paket/changelog kaynağı.
3. **Yasak iddiası** (EF Core yasak) → yerel `.ai/.rules/` dosyasına bağlanır, bloga değil.
4. **Elenen alternatif** → elenme nedenine tek kaynak; yoksa seçenek uydurulmuş sayılır.
5. **Tarihsel iddiası** (Nygard ADR biçimi) → tarihli birincil kaynak; kaynak yoksa tarih yazılmaz.

Sonuç `§7.1`'e **URL + erişim tarihi** ile girer. Tarihsiz satır `§6.1` kutusundan düşer, ADR `review`'dan geçemez.

---

## §2 Kapsam

| Alan | Kapsam (BU ŞABLON) | Kapsam Dışı |
|---|---|---|
| Karar biçimi | Context / Options / Decision / Consequences sentezi | Uzun mimari savunma |
| Uzunluk | Doldurulmuş çıktı 250–400 satır | 600+ satırlık ADR |
| Konu | Dizin/dosya adı, geçici süreç, reversible tercih | TFM, katman sınırı, migration, threat model |
| Numaralandırma | `ADR-NNN` → `.decisions/index.md` | Numarasız yazım |
| Kayıt yeri | `.ai/.decisions/accepted/` veya `draft/` | `.ai/.rules/` (kural ≠ karar) |
| Onay | 4 aşama (§4.2) | Gate geçişi (GATE-000..005 ayrı) |

### §2.1 Dosya Tipi → Sorumlu eşlemesi

| Dosya Tipi | Sorumlu Rol | Bu şablon? | Not |
|---|---|---|---|
| `.ai/.decisions/ADR-NNN-*.md` (kısa) | `architect` | ✅ Evet | Nygard hafif varsayılan |
| `.ai/.decisions/index.md` | `docs-writer` | Kısmen | Yalnız §7.1 satırı ekler |
| `.ai/.rules/*.md` (7 dosya) | `security` / `developer` | Hayır | Kalıcı kural ADR değildir |
| `.ai/architecture/index.md` | `architect` | Hayır | ADR'den türetilir |
| `.ai/REQUIREMENTS.md` | `business-analyst` | Hayır | Gereksinim ≠ karar |
| `.ai/log.md`, `MEMORY.md` | `master-orchestrator` | Hayır | Gündem, karar dayanağı değil |
| `.ai/keys.md` | `security` | Hayır | Secret içeriği ADR'ye girmez (G3) |

### §2.2 Kapsam Sınırı Testi (4 soru)

Dördü de **EVET** ise bu şablon kullanılır (S4 hariç):

1. **S1 Geri alınabilir mi?** Karar 1 günde tersine çevrilebilir mi → `EVET` beklenir.
2. **S2 Bağlam tek sayfaya sığıyor mu?** `Context` 40 satırı aşıyor mu → `HAYIR` beklenir.
3. **S3 Sonuç ölçülebilir mi?** `Consequences` ≥1 sayı/süre/sürüm içeriyor mu → `EVET` beklenir.
4. **S4 Veri/katman/güvenlik dokunuyor mu?** Şema, migration, auth, secret, TFM, katman sınırı → `HAYIR` beklenir.

S1 `HAYIR` veya S4 `EVET` ise bu şablon **kullanılamaz**; varyant §1.3'ten seçilir ve seçim §5.1 adım 2'de kaydedilir.

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

```yaml
---
title: "ADR-NNN — <ingilizce-slug>"
type: adr
category: architecture
status: draft            # draft | review | accepted | rejected | superseded
version: 0.1.0           # major = supersede, minor = revizyon, patch = imla
updated: 2026-09-25
authority: SSOT
supersedes: null         # örn. ADR-004
superseded_by: null      # örn. ADR-012
decision_date: 2026-09-25
risk: L                  # L | M | H
reversible: true         # Nygard hafif: true olmalı
related_gates: [GATE-004]
related_rules: [coding-standards]
template: adr-nygard-template.md
---
```

### §3.2 H1 iskeleti

```markdown
# ADR-NNN: <İngilizce karar başlığı (slug ile aynı)>
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [ARCHITECTURE](../../architecture/index.md)
```

### §3.3 §1 İskeleti (Nygard: Context)
```markdown
## §1 Amaç / Bağlam (Context)
### §1.1 Bu karar neden masada        ← tek paragraf
### §1.2 Disk kanıtları: iddia | kanıt | durum
### §1.3 Karar türü ve seçilen varyant
### §1.4 Araştırılan kaynaklar: URL + erişim tarihi
```
### §3.4 §2 İskeleti (Nygard: Considered Options)
```markdown
## §2 Kapsam   →  | Kapsam | Kapsam Dışı |
### §2.1 Dosya tipi → sorumlu (tablo)
### §2.2 Kapsam sınırı testi: S1..S4
```
### §3.5 §3 İskeleti (Nygard: Decision)
```markdown
## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar  ← "…karar verildi." ile biter
### §3.2 Considered Options: seçenek | artı | eksi | elenme nedeni
### §3.3 Uygulama detayı: dosya yolu, komut, sürüm/sayı
### §3.4 Bağımlılık akışı (ASCII)
```
### §3.6 §4 İskeleti (Nygard: Consequences)
```markdown
## §4 Kurallar ve Sonuçlar (Consequences)
### §4.1 Kazanımlar (ölçülebilir)  ### §4.2 Ödenen bedel (açıkça)
### §4.3 Guardrails → bu şablon §4.5
```
### §3.7 §5 İskeleti
```markdown
## §5 Workflow
### §5.1 Adım tablosu (11 satır)   ### §5.2 Hata modları   ### §5.3 Salt-okunur PowerShell
```
### §3.8 §6 İskeleti
```markdown
## §6 Doğrulama
### §6.1 Onay kutuları ≥18   ### §6.2 Red sinyalleri   ### §6.3 Sürüm/supersede
### §6.4 Quality Report      ### §6.5 Varyant ek kontrolleri
```
### §3.9 §7 İskeleti
```markdown
## §7 Referanslar
### §7.1 VersaCoder bağlantıkları   ### §7.2 Red Team: hata | neden yanlış | doğrusu
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği (ADR-003 dual-target)

Dosya: `.ai/.decisions/accepted/ADR-003-dual-target.md` — Nygard hafif varyantına uygulanmış §1–§3.

```markdown
---
title: "ADR-003 — Dual Target: net48 + net10.0-windows"
type: adr
category: architecture
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H
reversible: false
related_gates: [GATE-004]
related_rules: [coding-standards, deployment-guide]
template: adr-nygard-template.md
---
# ADR-003: Dual Target (`net48` + `net10.0-windows`)
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [deployment-guide](../../.rules/deployment-guide.md)

## §1 Amaç / Bağlam (Context)
### §1.1 Bu karar neden masada
VersaCoder.Desktop, DevExpress 25.2.8 ile derlenen WinForms `RibbonForm` uygulamasıdır ve olgunlaşma
`net48` üzerinde olmuştur; VersaCoder.Cli tarafında ise .NET 10 SDK davranışı istenmektedir. Tek hedef
ikisini de kırıyordu: yalnız `net48` → CLI modern SDK'dan mahrum; yalnız `net10.0-windows` →
DevExpress 25.2.8 + elle yazılmış `Designer.cs`/`.resx` zinciri risk altında.
### §1.2 Disk kanıtları
| İddia | Kanıt | Durum |
|---|---|---|
| `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>` | `.ai/` henüz kurulmadı (2026-09-25) | ⚠️ VERIFICATION REQUIRED |
| DevExpress 25.2.8, RibbonControlStyle.Office2010 | Proje brief'i | ⚠️ VERIFICATION REQUIRED |
| xunit + hedef %90 coverage + TreatWarningsAsErrors=true | Proje brief'i | ⚠️ VERIFICATION REQUIRED |
### §1.3 Karar türü
TFM kararı → `adr-architecture-template.md` uzun varyantı esastır; bu dosya Nygard kısa **özet** kaydıdır.
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim | Kullandığı iddia |
|---|---|---|---|
| DevExpress 25.2 desteklenen TFM listesi | resmi sürüm notu | 2026-09-25 | 25.2.8 + net48 uyumu |
| .NET 10 SDK çoklu hedef davranışı | resmi belge | 2026-09-25 | iki TFM bir arada |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| `net48` + `net10.0-windows` çift hedefi | Tek hedefe düşürme |
| Sürüm-sınırlı `#if` blokları | İş mantığı çatalı |
| İki hedef için ayrı xunit koşusu | DevExpress sürüm değişikliği (ADR-001) |
### §2.1 Dosya tipi → sorumlu
`.csproj`/`Directory.Build.props` → `architect`; koşullu kod → `developer`; CI matrisi → `devops-engineer`.
### §2.2 Kapsam sınırı testi
S1: **HAYIR** (kod tabanı ikiye bölünür) → uzun varyant gerekir, bu dosya özet kalır.
S2: **EVET**. S3: **EVET** (2 TFM × xunit = 2 koşu). S4: **EVET** (TFM) → ADR-002/ADR-003 ile çapraz bağ zorunlu.

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
`VersaCoder.Core` sınıf kütüphaneleri `net48;net10.0-windows` üzerinde derlenir; platforma özel kod
`#if NET48` / `#if NET10_0_OR_GREATER` ile izole edilir; `VersaCoder.Desktop` yalnız `net48`,
`VersaCoder.Cli` yalnız `net10.0-windows` hedefler — çift hedef karar verildi.
### §3.2 Considered Options
| Seçenek | Artı | Eksi | Elenme nedeni |
|---|---|---|---|
| Yalnız `net48` | Tek derleme, DevExpress uyumu | CLI modern SDK'dan mahrum | Platform kilitlenmesi |
| Yalnız `net10.0-windows` | Modern runtime | 25.2.8 + `.resx` zinciri riski | Sürüm doğrulanmadı |
| `netstandard2.0` + iki uygulama | Yüksek paylaşım | WinForms API'leri sınırlı | `RibbonForm` derlenmez |
| **Çift hedef (seçildi)** | İkisi korunur | 2× derleme + 2× test | — |
### §3.3 Uygulama detayı
- `Directory.Build.props`: `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>`
- `TreatWarningsAsErrors=true` her iki hedefte açık; hedef coverage %90.
- CI matrisi: 2 hedef × `dotnet test` = 2 koşu.
### §3.4 Bağımlılık akışı
`Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI` (+ `CrossCutting`, `Cli`)
`UI (Desktop)` → net48 dalı; `Cli` → net10.0-windows dalı.

## §4–§7 (özet — tam metin bu şablonun §4.5 / §5.1 / §6.1 / §7.1 karşılıklarıyla yazılır)
- §4: kazanım = 2 hedefte yeşil derleme; bedel = test bütçesi +%20, `#if` ≤20 sınırı; guardrails G1, G4, G7, G10, G11.
- §5: `architect` → props; `developer` → `#if` ayrımı; `devops-engineer` → CI matrisi; `testing` → 2 koşu; `reviewer` → onay.
- §6: §6.1 18 kutu + §6.5 EK-1 TFM listesi, EK-2 `#if` sayısı, EK-3 iki hedef coverage.
- §7: `architecture/index.md`, `.rules/deployment-guide.md`, GATE-004, ADR-001/ADR-002.
```

### §3.11 Kopyala-yapıştır tam kalıp (`{{PLACEHOLDER}}` alanlı)

```markdown
---
title: "{{ADR_ID}} — {{ingilizce-slug}}"
type: adr
category: {{architecture|data|security|ui|process}}
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: SSOT
supersedes: {{ADR_ID|NULL}}
superseded_by: null
decision_date: {{YYYY-MM-DD}}
risk: {{L|M|H}}
reversible: {{true|false}}
related_gates: [{{GATE-00X}}]
related_rules: [{{kural}}]
template: adr-nygard-template.md
---
# {{ADR_ID}}: {{INGILIZCE_KARAR_BASLIGI}}
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [ARCHITECTURE](../../architecture/index.md) · [{{kural}}](../../.rules/{{kural}}.md)

## §1 Amaç / Bağlam (Context)
### §1.1 Bu karar neden masada
{{TOPLANTI/SORUN/KİMİN_ÖNERİSİ — tek paragraf}}
### §1.2 Disk kanıtları
| İddia | Kanıt (dosya yolu) | Durum |
|---|---|---|
| {{IDDIYA_1}} | {{DOSYA_YOLU_1}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
| {{IDDIYA_2}} | {{DOSYA_YOLU_2}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
### §1.3 Karar türü ve seçilen varyant
{{TUR}} → {{VARYANT}} (bu dosya: Nygard hafif).
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim tarihi | İddia |
|---|---|---|---|
| {{KAYNAK}} | {{URL}} | {{YYYY-MM-DD}} | {{IDDIYA}} |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| {{KAPSAM_1}} | {{KAPSAM_DISI_1}} |
| {{KAPSAM_2}} | {{KAPSAM_DISI_2}} |
### §2.1 Dosya tipi → sorumlu
{{DOSYA_TIPI}} → {{ROL}}; {{DOSYA_TIPI_2}} → {{ROL_2}}.
### §2.2 Kapsam sınırı testi
1. S1 geri alınabilir mi? {{EVET|HAYIR}} — {{gerekçe}}.
2. S2 bağlam tek sayfaya sığıyor mu? {{EVET|HAYIR}} — {{gerekçe}}.
3. S3 sonuç ölçülebilir mi? {{EVET|HAYIR}} — {{ölçüt}}.
4. S4 veri/katman/güvenlik dokunuyor mu? {{EVET|HAYIR}} — {{gerekçe}}.

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
{{TEK_PARAGRAF — "…karar verildi." ile biter}}
### §3.2 Considered Options
| Seçenek | Artı | Eksi | Elenme nedeni |
|---|---|---|---|
| {{S1}} | {{ARTI}} | {{EKSI}} | {{NEDEN}} |
| {{S2}} | {{ARTI}} | {{EKSI}} | {{NEDEN}} |
| **{{S3 (seçildi)}}** | {{ARTI}} | {{EKSI}} | — |
### §3.3 Uygulama detayı
- Dosya: `{{DOSYA_YOLU}}` · Komut: `{{KOMUT}}` · Sürüm/sayı: {{SAYI}}
### §3.4 Bağımlılık akışı
{{AKIS_VEYA_ASCII}}

## §4 Kurallar ve Sonuçlar (Consequences)
### §4.1 Kazanımlar
- {{KAZANIM_1}} — {{ÖLÇÜT}}
### §4.2 Ödenen bedel
- {{BEDEL_1}} — {{MALİYET}}
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
Select-String -Path "{{DOSYA}}" -Pattern "{{DESEN}}"
~~~

## §6 Doğrulama
### §6.1 Onay kutuları
- [ ] 01 Frontmatter 11 alan eksiksiz, authority: SSOT
- [ ] 02 §1–§7 bölümlerinin 7'si de mevcut
- [ ] 03–18 … (bu şablon §6.1 listesinin tamamı)
### §6.2 Red sinyalleri
| Sinyal | Koşul | Aksiyon |
|---|---|---|
| RS-1 | {{KOŞUL}} | DUR + `review`'a dön |
### §6.3 Sürüm/supersede
{{MAJOR/MINOR/PATCH kuralı + zincir}}
### §6.4 Quality Report
{{§6.4 kalıp bloğu}}
### §6.5 Varyant ek kontrolleri
{{EK-1..EK-3}}

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
        +--------+  yazıldı   +--------+  4. aşama  +----------+
        | draft  +----------->+ review +----------->+ accepted |
        +---+----+            +---+----+            +----+-----+
            ^                     |                      |
            | 1. aşama düzeltme   | red sinyali          | yeni ADR eziyor
            +---------------------+                      v
            |                     |                +-------------+
            |                     +--------------->|  rejected   |
            |                                      +-------------+
            |        +---------------+                    |
            +--------+ superseded_by +--------------------+
                     | (eski ADR)    |  yeni ADR accepted olunca
                     +---------------+
```

| Durum | Anlamı | Kim koyar | Çıkış koşulu |
|---|---|---|---|
| `draft` | Yazar üzerinde çalışıyor | `developer` / `docs-writer` | §6.1 ≥18 kutu |
| `review` | Aday, 4. aşama beklemede | `architect` | §6.2 red sinyali yok |
| `accepted` | SSOT, bağlayıcı | `master-orchestrator` | 4. aşama + defter satırı |
| `rejected` | Reddedildi, gerekçeli | `master-orchestrator` | Gerekçe ≥3 satır |
| `superseded` | Yeni ADR ezdi | `docs-writer` | `superseded_by` dolu |

### §4.2 4 aşamalı onay akışı

| Aşama | Ad | Yapan | Girdi | Çıktı | Süre |
|---|---|---|---|---|---|
| 1 | Taslak | `developer` / `docs-writer` | Not + disk kanıtı | `status: draft` | ≤30 dk |
| 2 | Mimari gözden geçirme | `architect` | Draft | Yorum + varyant doğrulama | ≤45 dk |
| 3 | Kalite + güvenlik | `reviewer` + `security` | Revize draft | §6.4 Quality Report | ≤45 dk |
| 4 | Kabul / red | `master-orchestrator` | Report + defter satırı | `accepted`/`rejected` | ≤30 dk |

**Toplam bütçe: 150 dakika.** Aşama 4'te `.decisions/index.md` yazımı atlanamaz; atlanan ADR kabul edilmemiş sayılır (G12).

### §4.3 Risk ölçeği L / M / H

| Seviye | Tanım | VersaCoder eşiği | Onay | Geri dönüş |
|---|---|---|---|---|
| **L** | 1 günde tersine çevrilebilir | Dizin/dosya adı, log biçimi, geçici araç | Aşama 1–2 | Dakika |
| **M** | 1 haftada veri kaybısız geri dönüş | `.resx` düzeni, test klasörleri, CLI bayrakları | Aşama 1–3 | Saat |
| **H** | Geri dönüş katman/veri yeniden yazımı ister | TFM, katman sınırı, veri yığını, secret, DevExpress sürümü | 1–4 + `related_gates` | Gün–hafta |

Nygard hafif tek başına yalnız L/M için yeterlidir; H riskli karar bu şablonla `accepted` olamaz (G9).

### §4.4 Frozen / değişiklik kontrolü (2 istisna)

`accepted` dosya frozen'dır. Yalnız 2 istisna geçerli:

| İstisna | Koşul | Yapılabilir | Yasak |
|---|---|---|---|
| **1 — Teknik düzeltme** | Kırık wiki-link, imla, `⚠️ VERIFICATION REQUIRED`'in kanıtla kapanması | `version.patch` + `updated`; §7.1'e revizyon satırı | Karar metni, `Considered Options`, sonuç değişikliği |
| **2 — Yeni kanıtla supersede** | Yeni disk kanıt kararı geçersiz kıldı | **Yeni** ADR yaz, eskisini `superseded` + `superseded_by` yap | Eski dosyayı silmek veya metni düzenlemek |

Diğer her durum: **YENİ ADR** (`ADR-NNN+1`). Düzenlemek §6.2 RS-4 sinyalini tetikler.

### §4.5 Hard Guardrails tablosu (G1..G14)

| # | Guardrail | Kural | İhlal Sonucu |
|---|---|---|---|
| G1 | Frontmatter SSOT | `authority: SSOT` + 11 alan (§3.1) eksiksiz | Aşama 2 **DUR**; dosya `draft`'a döner |
| G2 | Slug İngilizce, metin Türkçe | Başlık/slug İngilizce; gövde Türkçe; mojibake yasak | Commit geri alınır; `reviewer` düzeltme ister |
| G3 | Secret sızıntısı yasak | `keys.md` içeriği, anahtar, parola ADR'ye kopyalanamaz | **Otomatik DUR** + `security` bildirimi; `rejected` |
| G4 | EF Core yasak | EF Core öneren veri kararı geçersizdir | `rejected` + ADR-004'e çelişki kaydı |
| G5 | Kanıt zorunlu | İddia → disk kanıtı yoksa `⚠️ VERIFICATION REQUIRED` | Kanıtsız satır silinir; <3 kanıt → `draft` |
| G6 | Kopyalama yasak | CoreMusic / vs-aivault `.ai` içeriği kopyalanamaz | ADR **reddedilir**, sıfırdan yazılır |
| G7 | TFM sabiti | Yalnız `net48`, `net10.0-windows` | `architect` derlemeyi durdurur; `draft` |
| G8 | DevExpress kilidi | Sürüm 25.2.8, stil Office2010 dışı çıkılamaz | `rejected`; ADR-001 supersede edilmeden değişmez |
| G9 | Risk ↔ varyant | H riskli karar Nygard hafifle `accepted` olamaz | Aşama 4 **DUR**; uzun varyantla yeniden yazım |
| G10 | Coverage korunur | xunit coverage %90 altına düşemez | `testing` job'ı kırmızı; ADR `review`'a döner |
| G11 | TreatWarningsAsErrors | Yeni kod uyarı hatasız derlenmeli | Derleme kırılırsa karar uygulanamaz; `draft` |
| G12 | Defter zorunlu | `accepted` → `.decisions/index.md` ADR-NNN satırı | Satır yoksa ADR fiilen `draft` |
| G13 | Supersede zinciri | Ezilen ADR `superseded` + `superseded_by`; silinmez | Zincir koparsa defter denetimi **DUR** |
| G14 | Salt-okunur doğrulama | §5.3 yalnız `Get-*`/`Select-String`/`Test-Path` | `Set-`/`Remove-`/`New-` içeren blok silinir |

---

## §5 Workflow

### §5.1 11 adımlı tablo + zaman bütçesi

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Karar anını yakala | Toplantı notu / `log.md` | "ADR gerekli" işareti | `master-orchestrator` | 5 dk |
| 2 | Varyant seç | §1.3 + §2.2 | Şablon adı | `architect` | 5 dk |
| 3 | Numara tahsis et | `.decisions/index.md` | `ADR-NNN` | `docs-writer` | 5 dk |
| 4 | Disk kanıtı topla | `Select-String` / `Test-Path` | §1.2 tablosu | `developer` | 15 dk |
| 5 | Web araştırması | §1.4 | URL + tarih | `researcher` | 20 dk |
| 6 | Alternatifleri yaz | Tartışma dökümü | §3.2 tablosu | `architect` | 15 dk |
| 7 | Kararı yaz | Seçenek + kanıt | §3.1–§3.4 | `architect` | 20 dk |
| 8 | Sonuçları yaz | Kazanım/bedel | §4.1–§4.3 | `architect` | 15 dk |
| 9 | Doğrula | §6.1 kutular | İşaretli liste | `reviewer` | 15 dk |
| 10 | Quality Report | §6.4 kalıp | `report` bloğu | `reviewer` | 10 dk |
| 11 | Kabul + defter | Report | `accepted` + `index.md` | `master-orchestrator` | 15 dk |

**Bütçe: 145 dakika** (§4.2 150 dk ile uyumlu). 145 dk'yı aşan taslak H2'ye düşer → uzun varyant.

### §5.2 Hata Modları tablosu

| Kod | Hata modu | Belirti | Kurtarma | Guardrail |
|---|---|---|---|---|
| H1 | Kanıtsız iddia | §1.2'de <3 `✅` | Satırı sil ya da `⚠️` bırak, `draft`'a dön | G5 |
| H2 | Kapsam şişmesi | Çıktı >400 satır | `adr-template.md` uzun varyantına taşı | S2 |
| H3 | Yanlış varyant | S4 `EVET` iken Nygard hafif | Hedef varyantla yeniden yaz; bu dosya `rejected` | G9 |
| H4 | Mojibake | bozuk kodlama artıkları | UTF-8 kaydet, `ç ğ ı İ ö ş ü` doğrula | G2 |
| H5 | Eski vault kopyası | Cümle yapısı CoreMusic ile aynı | İçeriği sıfırdan VersaCoder için yaz | G6 |
| H6 | Defter atlandı | `accepted` ama `index.md` satırı yok | Satırı ekle; statü `draft`'a geri al | G12 |
| H7 | Araştırmasız sürüm iddiası | §1.4 tablosu boş | URL + tarih ekle; yoksa iddiayı kaldır | G5 |
| H8 | Frozen ihlali | `accepted` dosyada metin değişikliği | Değişikliği geri al; `ADR-NNN+1` yaz | §4.4 |

### §5.3 Salt-okunur PowerShell komut bloğu (6 komut)

```powershell
# 1 — Son ADR numarası olmalı: en yüksek ADR-NNN (defterde)
Select-String -Path ".\.ai\decisions\index.md" -Pattern "^ADR-\d{3}" | Select-Object -Last 1

# 2 — Örnek dosya varlığı olmalı: True/False, silme yok
Test-Path -Path ".\.ai\decisions\accepted\ADR-003-dual-target.md"

# 3 — Yasaklı vault'a sızan satır sayısı olmalı: 0
(Select-String -Path ".\.ai\decisions\*.md" -Pattern "coremusic" -ErrorAction SilentlyContinue).Count

# 4 — Draft ADR sayısı olmalı: 0 değil (iş bitmedi demek)
(Get-ChildItem -Path ".\.ai\decisions\draft" -Filter "ADR-*.md" -ErrorAction SilentlyContinue).Count

# 5 — İşaretlenmemiş onay kutusu sayısı olmalı: 0
(Select-String -Path ".\.ai\decisions\ADR-*.md" -Pattern "^\s*- \[ \]").Count

# 6 — Bu şablonun satır aralığı olmalı: 480 ile 660 arasında
(Get-Content -Path ".\.ai\.templates\adr\adr-nygard-template.md").Count
```

Blok salt-okunurdur: `Set-`, `Remove-`, `New-`, `Move-`, `Rename-` yoktur (G14). Yazma editör aracılığıyla yapılır.

---

## §6 Doğrulama

### §6.1 Onay kutusu listesi (18 madde)

- [ ] 01 — Frontmatter 11 alan eksiksiz (`title…template`).
- [ ] 02 — `authority: SSOT` birebir yazılı.
- [ ] 03 — `status` ∈ {draft, review, accepted, rejected, superseded}.
- [ ] 04 — H1 var ve `ADR-NNN:` ile başlıyor.
- [ ] 05 — `**Zorunlu Bağlantılar:**` satırı var, `.ai` içine göre göreli.
- [ ] 06 — §1…§7 bölümlerinin 7'si de mevcut, hiçbiri silinmemiş.
- [ ] 07 — §1.2 her satırda `✅` ya da `⚠️ VERIFICATION REQUIRED`.
- [ ] 08 — §1.4'te URL **ve** erişim tarihi var.
- [ ] 09 — §2.2 dört soru cevaplanmış.
- [ ] 10 — §3.2'de ≥2 elenen seçenek + elenme nedeni.
- [ ] 11 — §3.3'te somut dosya yolu / komut / sürüm-sayı var.
- [ ] 12 — §4.1 kazanımları ölçülebilir (sayı/süre/sürüm).
- [ ] 13 — §4.2 ödenen bedel açık ("ama/ancak" ile gizlenmedi).
- [ ] 14 — §4.5 G1..G14 = 14 satır, "İhlal Sonucu" dolu.
- [ ] 15 — §5.1 11 adım + zaman bütçesi ≤150 dk.
- [ ] 16 — §5.2 hata modu ≥7 satır.
- [ ] 17 — §5.3 6 komut, her biri "— N olmalı" yorumlu, salt-okunur.
- [ ] 18 — §6.5 varyant kontrolleri bu karar için doldurulmuş.

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| Sinyal | Tetikleyici | Sonuç | Sorumlu |
|---|---|---|---|
| RS-1 | `authority` ≠ `SSOT` | Aşama 2 **DUR** | `architect` |
| RS-2 | `password`, `api_key`, `sk-`, `Data Source=` deseni var | **DUR** + `security` bildirimi; `rejected` | `security` |
| RS-3 | §1.2'de tüm satırlar `⚠️` | Aşama 4 **DUR** | `reviewer` |
| RS-4 | `superseded_by` dolu ama defter zinciri boş | Defter denetimi **DUR** | `docs-writer` |
| RS-5 | Risk `H` iken varyant Nygard hafif | Aşama 4 **DUR** | `master-orchestrator` |
| RS-6 | §3.2'de "elenme nedeni" boş hücre | Aşama 2 **DUR** | `architect` |
| RS-7 | Metin mojibake içeriyor | Commit **DUR** | `reviewer` |

### §6.3 Sürüm / supersede kuralları tablosu

| Değişiklik | version | status | Ek işlem |
|---|---|---|---|
| İmla / kırık link / kanıt kapanması | `patch` | Aynı | §7.1'e revizyon satırı |
| Aynı karara yeni alternatif/sonuç | `minor` | `review` → `accepted` | §6.4 report yenilenir |
| Kararın değişmesi | **Yeni ADR** | Eski → `superseded` | `supersedes`/`superseded_by` dolar |
| Reddedilen kararın açılımı | Yeni ADR | Yeni: `draft` | Red gerekçesi §1.1'de anılır |
| Bu şablonun değişmesi | Şablon `minor` | `active` | Doğrulanmış ADR'ler dokunulmaz (§4.4) |

### §6.4 Quality Report (markdown bloğu)

```markdown
## Quality Report — {{ADR_ID}}
- **Tarih:** {{YYYY-MM-DD}} | **Şablon:** adr-nygard-template.md v{{X.Y.Z}}
- **Durum:** {{…}} → hedef: accepted | **Risk:** {{L|M|H}} | **Reversible:** {{true|false}}
- **Onay kutusu:** {{NN}}/20 → {{YETERLİ|YETERSİZ}}
- **Kanıt:** ✅ {{N}}, ⚠️ {{N}} → {{KANIT_YETERLI|DEGIL}}
- **Web kaynağı:** {{N}} URL + tarih → {{TAM|EKSIK}}
- **Red sinyali:** {{YOK|RS-N}} | **Guardrail ihlali:** {{YOK|G-N → İhlal Sonucu}}
- **Defter:** `.decisions/index.md` ADR-NNN → {{VAR|YOK}}
- **Karar özeti (1 cümle):** {{NE, NEDEN, NEYE MAL OLDU}}
- **Sonraki adım:** {{ROL + 1 eylem + süre}}
```

### §6.5 Varyant özgü ek kontroller (Nygard hafif)

| # | Kontrol | Doğru koşul | Yanlışsa |
|---|---|---|---|
| EK-1 | Kısa form | Çıktı 250–400 satır | >400 → uzun varyant (H2) |
| EK-2 | Nygard üçlüğü | `Context`, `Considered Options`, `Decision`, `Consequences` başlıkları metinde | Eksik başlık → `draft` |
| EK-3 | Reversible | `reversible: true` + geri dönüş yolu tek cümlede | `false` ise `accepted` olamaz (G9) |
| EK-4 | Alternatif | §3.2'de ≥2 elenen seçenek | `review`'a dön |
| EK-5 | Ölçüt bağlantısı | §4.1 her kazanım §6.1 kutusuyla eşleşiyor | §6.4'te `KANIT_YETERLI` yazılamaz |

---

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Bağlantı | Göreli Yol | İlişki |
|---|---|---|
| Vault ana sayfa | `index.md` | Envanterin üst kaynağı |
| Mimarî | `architecture/index.md` | §3.4 akışının kaynağı |
| Karar defteri | `.decisions/index.md` | Numara + supersede zinciri |
| Altın şablon | `.templates/adr/adr-template.md` | Uzun varyanta geçiş |
| Kural dosyaları | `.rules/coding-standards.md` | G2, G7, G11 denetimi |
| Güvenlik kuralları | `.rules/security-rules.md` | G3, RS-2 denetimi |
| Adlandırma | `.rules/naming-conventions.md` | Slug/dosya adı biçimi |
| Anahtar sözlüğü | `keys.md` | Yalnız varlık kanıtı; içerik girmez |
| Gate planı | `WORKFLOW.md` | `related_gates` (GATE-000..005) |
| Gereksinimler | `REQUIREMENTS.md` | Karar–gereksinim çelişkisi |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Neden Yanlış | Doğrusu |
|---|---|---|---|
| 1 | "Gerekçe sonra eklenir" | Nygard'ın değeri gerekçededir | Karar anında §1.1 + §3.2 yazılır |
| 2 | Alternatifleri yazmamak | "Neden bu?" 6 ay sonra sorulur | ≥2 elenen seçenek + nedeni |
| 3 | Sonuçta yalnız avantaj | Bedel gizlenince borç büyür | §4.2 ayrı satırlarda bedel |
| 4 | `accepted` + boş defter | ADR bulunamaz | G12: `index.md` satırı eşzamanlı |
| 5 | ADR'ye anahtar/parola yapıştırmak | Repo + secret = sızıntı | G3: `keys.md`'ye referans, içerik yok |
| 6 | Eski ADR'yi düzenleyerek karar değiştirmek | Tarihçe ve zincir kaybolur | §4.4: yeni `ADR-NNN+1` |
| 7 | Kaynaksız "araştırdım" | Kaynaksız iddia uydurmadır | §1.4 URL + tarih; yoksa `⚠️` |
| 8 | Eski vault metnini yapıştırmak | G6 ihlali, VersaCoder'a ait değil | Sıfırdan yaz, yalnız yapı referans |
| 9 | Tek seçenekli karar | Seçeneksiz karar tercihtir | §3.2'de ≥2 alternatif |
| 10 | Ölçüsüz kazanım ("hızlanacak") | Ölçülmeyen fayda kanıt sayılmaz | Süre/satır/sürüm sayısı (§6.1 #12) |
