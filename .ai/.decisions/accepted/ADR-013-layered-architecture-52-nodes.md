---
title: "ADR-013 — Katmanlı Mimari: 52 Düğüm / 5000+ Bileşen Kataloğu"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "katmanli mimari: 52 dugum / l0..l7 katalogu, bagimlilik yonu ve katman sozlesmeleri"
---

# ADR-013 — Katmanlı Mimari: 52 Düğüm / 5000+ Bileşen Kataloğu

**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [architecture/index](../../architecture/index.md) · [CLAUDE.md](../../CLAUDE.md) · [keys.md](../../keys.md) · [Kapsam analizi](../../reports/00-kapsam-analizi.md)

> **Tek cümlelik karar:** VersaCoder **8 makro katman (l0..l7) + 44 alt katman = 52 düğüm** olarak modellenir; her düğüm `architecture/catalog/` altında **bileşen kaydı tablosu** taşır ve toplam kayıt **≥5000**'dir.

---

## §1 Bağlam (Context)

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Prompt §4.3: katmanlı mimari **52 düğüm / 5000+ bileşen** kataloğu zorunlu | `reports/00-kapsam-analizi.md` §2 karar 9 |
| C2 | Vault'ta `l0..l7` = **8 makro katman** şartnamesi (`index.md` + 8 katman dosyası) yazılmış durumda | `architecture/l0..l7/index.md` |
| C3 | `8 makro + 44 alt = 52` toplamı korunabilir; `7 makro + 45 alt` formülü **8 makro ile çelişiyordu** | bu ADR §2 (çözüm) |
| C4 | Bağımlılık yönü ve `#if` kuralı NetArchTest ile denetlenecek | `spec/test-strategy.md` §3 |
| C5 | Bileşen envanteri koddan **önce** tanımlanmazsa REVIEW/TDD hedefi ölçülemez (ROLE.md §6 — kanıtsız iddia yasak) | `AGENTS.md` §9 |
| C6 | Web araştırması katman/disiplin kararlarını destekliyor (katmanlı mimari + tek yön) | `reports/01-web-kaynaklari.md` (2026-09-25) |

---

## §2 Karar (Decision)

**VersaCoder mimarisi 52 düğümlük bir ağaçtır:**

| # | Makro | Alt katman | N kodu | Alt katman kodları | Proje | TFM |
|---|---|---:|---|---|---|---|
| 1 | **l0 Abstractions** | 5 | N01–N05 | ports-core · result-error · events-abi · extensions · constants | `VersaCoder.Abstractions` | iki bacak |
| 2 | **l1 Domain** | 6 | N06–N11 | entities · value-objects · aggregates · domain-events · invariants · enums | `VersaCoder.Domain` | iki bacak |
| 3 | **l2 Application** | 7 | N12–N18 | commands · queries · handlers · dto-mappers · validators · use-cases · scheduling | `VersaCoder.Application` | iki bacak |
| 4 | **l3 Crosscutting** | 6 | N19–N24 | logging · caching · clock-time · crypto-sec · config-opts · resilience | `VersaCoder.Crosscutting` | iki bacak |
| 5 | **l4 Infrastructure** | 8 | N25–N32 | ado-dapper · sqlite-provider · sqlserver-provider · migrations · fs-tools · git-adapter · llm-http · tool-runner | `VersaCoder.Infrastructure` | iki bacak |
| 6 | **l5 Protocol** | 5 | N33–N37 | tool-schema · messages · session-transcript · loop-policy · versioning-errors | `VersaCoder.Protocol` | iki bacak |
| 7 | **l6 Host** | 3 | N38–N40 | bootstrap-pipeline · di-registration · config-bridges | `VersaCoder.Host` | iki bacak |
| 8 | **l7 UI + CLI** | 4 | N41–N44 | shell-ribbon · panels-explorer · editor-ai-chat · settings-cli | `VersaCoder.Desktop` / `.Cli` | iki bacak |
| | **TOPLAM makro** | **44** | **8 düğüm** | N45–N52 = makro özet düğümleri | — | — |

**52 = 8 makro düğüm (N45–N52 özeti) + 44 alt katman düğümü (N01–N44).**

### 2.1 Katalog biçimi (format)

```
architecture/catalog/
+-- index.md                      - 52 düğüm listesi + sayaç özeti
+-- l0-abstractions/
|   +-- n01-ports-core.md         - 1 düğüm = 1 bileşen kaydı tablosu
|   +-- n02-result-error.md
|   +-- ... (5 dosya)
+-- l1-domain/      (6 dosya)
+-- l2-application/ (7)
+-- l3-crosscutting/(8 → 6)
+-- l4-infrastructure/ (8)
+-- l5-protocol/    (5)
+-- l6-host/        (3)
+-- l7-ui/          (4)         - toplam 44 alt düğüm dosyası
```

Her kayıt satırı **8 sütun** zorunlu:

| Sütun | Açıklama | Örnek |
|---|---|---|
| ID | `<N>[-<seq>]` | `N27-014` |
| Bileşen | tip adı | `SessionRepository` |
| Tip | class / interface / record / enum / struct | `interface` |
| Sorumluluk | tek cümle | `Session kayıtlarını oku/yaz (Dapper)` |
| Proje | hangi `.csproj` | `VersaCoder.Infrastructure` |
| TFM | `net48;net10.0-windows` | `net48;net10.0-windows` |
| Test | test sınıfı adı | `SessionRepositoryTests` |
| Durum | `planlı / yapıldı / testli` | `planlı` |

### 2.2 Sayım kuralı (adjudication)

- **Kayıt = tablo satırı** (`| N.. |` başlangıçlı). Başlık, ayraç ve kod bloğu **sayılmaz**.
- Toplam **≥5000** kayıt şart. Dağılım: 44 alt düğüm × **≥112** = ≥4928 + 8 makro düğüm × **≥20** = ≥160 → **≥5088**.
- Sayım aracı: `scripts/count-components.ps1` (geçerli = `N\d{2}` ID deseni).
- Katalog dosyası sayısı: **44 alt + 8 makro + index = 53 dosya**.
- **MD hedefi:** ADR-003 (≥500 MD) katalogla birlikte karşılanır (91 → 144+ katalog + diğer part çıktıları).

### 2.3 Katman kuralı (değişmez)

```
  l7 --▶ l6 --▶ l5 --▶ l4 --▶ l2 --▶ l1 --▶ l0
                |                 ▲
                +------- l3 ------+   (yalnız l0'a bakar)
```

- **Tek yön; geri dönüş yalnız `interface` (l0)** ile — NetArchTest `Architecture/LayerRulesTests.cs`.
- `#if` yalnız platform farkı: `l6` (Host) ve `l7` (UI) + ortak kaynak istisnası.
- Her alt katman düğümü, ilgili `architecture/lN-*/index.md` §2 (YAPAR/YAPMAZ) ile **çelişemez**.

---

## §3 Alternatifler

| # | Alternatif | Artı (+) | Eksi (−) | Neden reddedildi |
|---|---|---|---|---|
| A1 | **7 makro + 45 alt = 52** (prompt'un ilk formülü) | 7 sayı olarak prompt'a birebir uyar | `l0` (Abstractions) `l1` (Domain) içine gömülür; 8 katman şartnamesi, 8×8 bağımlılık matrisi ve NetArchTest kuralları yeniden yazılmalı; `Result<T>` portları domain'e girerse DIP zayıflar | Mevcut 8 dosyalık mimari + dependency-map + lN şartnameleriyle çelişir (≈40 KB yeniden iş); `l0`'ın ayrılığı ADR-001/005 ile tutarlı |
| A2 | **Tek düz paket + klasör ayırımı** (katmansız) | Kurulum hızlı | Bağımlılık yönü denetlenemez, NetArchTest anlamsız, iki bacak korumaz | Katman zaten onaylı (ADR-001 + prompt) |
| A3 | **52 klasör = 52 ayrı `.csproj`** | Maksimum izolasyon | 44 proje derlemeyi ve `Directory.Packages.props` karmaşasını büyütür; VS/TUI performansı düşer | `8 proje (l0..l7)` yeterli; alt katmanlar **klasör** düzeyinde |
| A4 | **Katalog olmadan kod yaz, sonra envanter çıkar** | İlk kodlama hızlı | ROLE.md §6 ihlali: `5000+ bileşen` iddiası kanıtsız; REVIEW/TDD kapsamı ölçülemez | Prompt §4.3 kataloğu **önce** istiyor |
| A5 | **Bileşen başına ayrı MD (5000 dosya)** | Tek kayıt = tek dosya | 5000 dosya arama/denetimi bozar, link taraması ölçeklenmez | Tablo satırı ölçümü yeterli (§2.2) |

---

## §4 Sonuçlar (Consequences)

**Artı (+)**

1. 52 düğüm = **tek bakışta** mimari envanter; `catalog/index.md` tek giriş kapısı.
2. **≥5088 kayıt** ile ADR-003'ün 500 MD hedefine ve prompt'un `5000+` şartına **ölçülebilir** cevap (`count-components.ps1`).
3. Her bileşen baştan **Proje / TFM / Test** sütunuyla tanımlandığından TDD ve NetArchTest hedefi net.
4. Alt katmanlar klasörde kaldığından **8 `.csproj`** sınırlı kalır; derleme iki bacakta da kontrol edilebilir.
5. Durum sütunu (`planlı/yapıldı/testli`) ile PART 6 ilerlemesi **katalogdan** izlenir.

**Eksi (−) / Riskler**

1. Katalog **elle/generator ile üretilen bir plandır**; kod yazıldıkça `Durum` sütunu güncellenmezse **stale** olur → PART 6 DoD'suna "katalog = kod" kontrolü eklenir.
2. 44 dosyalık katalog + 53 dosya link taramasını büyütür → `check-links.ps1` her part sonunda zorunlu.
3. Üretim şablonu `build-catalog.ps1` kullanılırsa satırlar **benzer isim kalıpları** içerir → kod yazımında gerçek tip adlarıyla **eşleştirme zorunlu** (uyuşmayan satır RED).
4. `8 makro` (prompt'un `7 makro` formülünden sapma) → §2 C3'te gerekçelendirildi; kullanıcı onayı **GATE-004a** ile alınır.

---

## §5 Kabul Kriterleri

- [x] 52 düğüm tanımı (8 makro + 44 alt) yazılmış ve `architecture/catalog/index.md`'ye bağlı
- [x] Katalog formatı (8 sütun) tanımlı
- [ ] `architecture/catalog/` = **44 + 8 + index** dosya
- [ ] `scripts/count-components.ps1` → **≥5000** kayıt
- [ ] 52 düğüm ↔ `architecture/lN-*/index.md` sorumluluklarıyla **çelişmiyor**
- [ ] `dependency-map.md` §2 (8×8) ile aynı yön
- [ ] GATE-004a raporunda `8 makro` sapması kullanıcıya bildirildi

**İlgili:** [ADR-001](ADR-001-dual-target-net48-net10.md) (iki bacak) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) (veri katmanı) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) (≥500 MD)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Gerekçe: prompt §4.3 + mevcut l0..l7 yapısı.
