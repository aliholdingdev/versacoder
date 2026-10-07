---
title: "VersaCoder — Katman Şartnamesi (Layer Spec) Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "katman sartnamesi sablonu: sozlesmeler, bagimlilik yasagi ve dogrulama adimlari"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Katman Şartnamesi (Layer Spec) Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[../CLAUDE.md]] · [[architecture-template]] · [[dependency-rule-template]] · [[ascii-diagram-template]] · [[../adr/adr-architecture-template]]

---

## §1 Amaç

Bu şablon, VersaCoder K0..K8 katmanlarından **birinin** şartnamesini (`.ai/architecture/layer-<ad>.md`) tek biçimde üretir: sınır iki tabloda, bağımlılık tek tabloda, kanıt §5.3 komut setinde durur.

SRP: genel yığın `architecture-template`'in, matris `dependency-rule-template`'in, çizim `ascii-diagram-template`'in işidir; bu dosya **tek katmanın içine** bakar, kardeşlerini tekrarlamaz (envanter G12).

### §1.1 Künye

| Alan | Değer |
|------|-------|
| Şablon / Yol | `layer-spec-template.md` · `.ai/.templates/architecture/` |
| Hedef Dosya | `.ai/architecture/layer-<ad>.md` → `C:\versacoder\.ai\architecture\` |
| Kapsam | tam **1** katman (K0..K8'den biri); tetikleyici: sınır/port/`.csproj` değişimi |
| Sorumlu / Onay | `architect` / `reviewer` · kabul `master-orchestrator` · gate `GATE-004` |
| Bağlı Şablonlar | `architecture-template` · `dependency-rule-template` · `ascii-diagram-template` |
| Kodlama / Sürüm | UTF-8 BOM'suz (mojibake YASAK) · `1.0.0` / `SSOT` |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz; satır `⚠️ VERIFICATION REQUIRED` kalır. Ölçüm: `C:\versacoder`, 2026-09-25.

| # | İddia | Disk Kanıtı | Durum |
|---|-------|-------------|-------|
| E1 | `.ai/.templates/architecture/` klasörü | `architecture-template.md` 34.059 B · `ascii-diagram-template.md` 38.121 B | ✅ Doğrulandı |
| E2 | `layer-spec-template.md` envanterde `PLANLI` | bu dosya yazıldı; envanter satırı hâlâ `PLANLI` | ✅ Doğrulandı |
| E3 | Şablon envanteri `.ai/.templates/index.md` | 606 satır / 38.816 B · 44 şablon kaydı | ✅ Doğrulandı |
| E4 | Dizin kuralı `.ai/.templates/CLAUDE.md` | 132 satır / 7.047 B · 14 rol listesi | ✅ Doğrulandı |
| E5 | `.ai/architecture/` (hedef dizin) | `.ai/` altında yalnız `.templates/` var | ❌ YOK |
| E6 | `.ai/architecture/index.md` · `.ai/WORKFLOW.md` | iki dosya da yok | ❌ YOK |
| E7 | `.ai/.rules/` (7 MD) · `.ai/keys.md` · `.ai/log.md` | `.rules/` dizini yok, 0 dosya | ❌ YOK |
| E8 | `.ai/adr/` + `ADR-001…008` | dizin yok | ❌ YOK |
| E9 | `src/**`, `VersaCoder.sln`, `tests/**` | kökte yalnız `.ai` + `.git` | ❌ YOK |
| E10 | `Directory.Packages.props` · `global.json` · `versacoder.db` | kökte üçü de yok | ❌ YOK |
| E11 | DevExpress `25.2.8` · ADO.NET + Dapper · `@p1..@pN` · ikili TFM | yalnız şablon metinlerinde; kod kanıtı yok | ⚠️ VERIFICATION REQUIRED |
| E12 | Yasaklı vault'lar (`C:\www\` altındaki iki referans deposu) | yalnız **yapı** referansı; tek kelime kopyalanmadı | 🔒 YASAK |

**Yorum yasağı:** `❌` ve `⚠️` satırlarına "şurada olmalıydı", "muhtemelen vardır" yazılmaz. Boşluk ya §5.1 Adım 1'de kapatılır ya da satır kalır; kanıtsız satır süs olarak girmez.

### §1.3 Ne Zaman Bu Şablon Kullanılır

| # | Tetikleyici | Bu Şablon? | Çıktı |
|---|-------------|-----------|-------|
| 1 | Tek katman sınırı/izin/portu yazılmıyor · port veya `.csproj` değişti | ✅ Evet | `.ai/architecture/layer-<ad>.md` |
| 2 | YAPAR/YAPMAZ sınırı tartışmaya açıldı | ✅ Evet | aynı dosya + ADR (karar ise) |
| 3 | K0..K8 yığını, proje ağacı, TFM · 9×9 matris, ihlal taraması, çizim | ❌ | `architecture-template` / `dependency-rule` / `ascii-diagram` |

### §1.4 Özet

| Kapsam (BU ŞABLON) | Kapsam Dışı (hangi şablon) |
|--------------------|----------------------------|
| Tek katman kimliği: kod, ad, proje, sahip rol, TFM | Katman **yığını** + proje ağacı (`architecture-template`) |
| YAPAR / YAPMAZ sınırı (iki ayrı tablo) | **9×9 matris** + ihlal komutu (`dependency-rule`) |
| İzinli / yasak bağımlılık + kırmızı çizgiler | Çizim, ASCII kutu (`ascii-diagram-template`) |
| API + hata + performans + test (%90) + DoD | Karar (ADR) · kod (`src/**`) · gate · testin yazımı |

---

## §2 Kapsam

**İçinde:** tek katman şartnamesi, sınır tabloları, API/imza listesi, hata sözleşmesi, performans ve test bütçesi, kırmızı çizgiler, salt-okunur doğrulama.
**Dışında:** çok katmanlı genel belge, bağımlılık matrisi, diyagram, kod üretimi, ADR.

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya / Nesne | Sorumlu Rol | Bu Şablondan mı? | Not |
|---|---------------|-------------|-------------------|-----|
| 1 | `.ai/architecture/layer-<ad>.md` | `architect` | ✅ Evet | bu şablonun tek çıktısı |
| 2 | `<konu>.md` (genel) · `dependency-rule.md` | `architect` | ❌ | kardeş şablonlar |
| 3 | `.ai/adr/ADR-NNN-*.md` | `architect` | ❌ | karar ≠ şartname |
| 4 | `src/**` (`.cs`, `.csproj`) · `tests/**` · `README.md` · `index.md` | `developer` / `devops` / `testing` / `docs-writer` | Kaynak/Kayıt | `ProjectReference` kanıtı §5.3/2 · `PLANLI → YAZILDI` |

### §2.2 Kapsam Sınırı Testi (5 soru)

Dosyaya sırayla sor; **ilk `HAYIR`** yanlış şablonu işaret eder:

| # | Soru | Beklenen | Beklenmezse |
|---|------|----------|-------------|
| S1 | Dosya **tek** bir katmanı (K0..K8) anlatıyor mu? | `EVET` | iki+ katman → `architecture-template` |
| S2 | İçinde 9×9 matris veya ihlal tarama komutu var mı? | `HAYIR` | varsa → `dependency-rule-template` |
| S3 | Konu geri alınabilir tek bir **karar** mı? | `HAYIR` | evetse → `adr-template` |
| S4 | Çıktı bir `.cs` iskeleti mi? | `HAYIR` | evetse → `backend/*-template` |
| S5 | Dosya yalnız çizim bloğu üretiyor mu? | `HAYIR` | evetse → `ascii-diagram-template` |

S1 `HAYIR` ise bu şablon fazladır; S2 `EVET` ise bu şablon **yanlış** şablondur.

---

## §3 Mimari

### §3.1 Üretilen Dosyanın İskeleti (frontmatter + H1 + 7 bölüm)

Yer tutucu yalnız bu blokta yaşar; üretilen şartnamede adedi **0** olmalıdır (§4.5 G4).

````markdown
---
title: "Katman Şartnamesi — {{KATMAN_ADI}} (VersaCoder)"
type: architecture
status: {{DRAFT|REVIEW|ACCEPTED}}
version: {{X.Y.Z}}
updated: {{YYYY-MM-DD}}
authority: primary
layer_code: {{K0_K1_K2_K3_K4_K5_K6_K7_K8}}
project: {{VersaCoder.Ad}}
owner: {{ROL}}
related_gates: [GATE-004]
related_adrs: [{{ADR_LISTESI}}]
template: layer-spec-template.md
---

# Katman Şartnamesi — {{KATMAN_ADI}}

**Zorunlu Bağlantılar:** [[{{BAG_1}}]] · [[{{BAG_2}}]] · [[{{BAG_3}}]] · [[{{BAG_4}}]] · [[{{BAG_5}}]] · [[{{BAG_6}}]]

Bölümler: `§1` Amaç (künye, kanıtlar, tetikleyici, özet) · `§2` Kapsam · `§3` Mimari (iskelet → dolu örnek) · `§4` Kurallar (G1..G12) · `§5` Workflow · `§6` Doğrulama · `§7` Referanslar
````

### §3.2 Katman Kimliği (K0..K8 kartı)

Kodlar sabittir (`architecture-template` §3.2 ile aynı); şartnamede **tek** satır işaretlenir.

| Kod | Katman | Proje | Sahip Rol | TFM | Şartname |
|-----|--------|-------|-----------|-----|----------|
| K8 | Test | `tests/VersaCoder.Tests`, `tests/VersaCoder.IntegrationTests` | `testing` | `net10.0-windows` | `layer-test.md` |
| K7 | Host | `VersaCoder.Host` | `devops-engineer` | `net48;net10.0-windows` | `layer-host.md` |
| K6 | UI | `VersaCoder.Desktop`, `VersaCoder.Cli` | `ui-designer` | Desktop `net48` · Cli `net10.0-windows` | `layer-ui.md` |
| K5 | Protocol | `VersaCoder.Protocol` | `developer` | `net48;net10.0-windows` | `layer-protocol.md` |
| K4 | Infrastructure | `VersaCoder.Infrastructure` | `database-optimizer` | `net48;net10.0-windows` | `layer-infrastructure.md` |
| K3 | Application | `VersaCoder.Application` | `developer` | `net48;net10.0-windows` | `layer-application.md` |
| K2 | Domain | `VersaCoder.Domain` | `architect` | `net48;net10.0-windows` | `layer-domain.md` |
| K1 | Abstractions | `VersaCoder.Abstractions` | `architect` | `net48;net10.0-windows` | `layer-abstractions.md` |
| K0 | CrossCutting | `VersaCoder.CrossCutting` | `developer` | `net48;net10.0-windows` | `layer-crosscutting.md` |

Kimlik satırı tek formatta: `K4 · Infrastructure · VersaCoder.Infrastructure · sahip database-optimizer · TFM net48;net10.0-windows · spec layer-infrastructure.md · gate GATE-004`.

### §3.3 Sorumluluk Sınırı — İki Ayrı Tablo

**Tablo A — Bu katman NE YAPAR** (her satır ölçülebilir; boş hücre yok):

| # | Sorumluluk | Kanıt (nasıl anlaşılır) | Çıktı |
|---|-----------|-------------------------|-------|
| A1 | Port uygulaması (`I*Repository`, `IUnitOfWork`) | implementasyon derlenir, test edilir | `.cs` |
| A2 | Parametreli veri erişimi | her SQL `@p1..@pN` içerir, concat yok | sorgu |
| A3 | Sözleşme dışı hatayı sarmalama | exception `Result<T>`'ye çevrilir | `Result<T>` |
| A4 | Kaynak kilit yönetimi | bağlantı/dosya `using` + `Dispose` | — |
| A5 | Katman içi log çağrısı | log kaydı üretilir | log satırı |
| A6 | Kendi invariant/doğrulama kontrolü | kural ihlali testte yakalanır | `Result.Invalid` |

**Tablo B — Bu katman NE YAPMAZ** (her satır kırmızı çizgi; ihlal = §6.2):

| # | Yapmaz | Neden | İhlalde |
|---|--------|-------|---------|
| B1 | Yetkisiz katman tipine doğrudan bakmak | bağımlılık yönü bozulur | §5.3/3–4 |
| B2 | UI ekranı / mesaj kutusu açmak | sunum katmanın işi değil | RED |
| B3 | EF Core / `DbContext` kullanmak | ADR-004 — ADO.NET + Dapper | RED (G2) |
| B4 | String concat SQL yazmak | injection yüzeyi açılır | RED (G10) |
| B5 | Kural/metin kararı gömmek | kural ≠ kod → `.ai/.rules/` | RED |
| B6 | Secret/anahtar barındırmak | `.ai/keys.md` tek kaynak | RED |

Kural: her madde ölçülebilir olur; "gerekirse yapılır" yazılmaz.

### §3.4 İzinli / İzinli Olmayan Bağımlılık

Yön tablosu `architecture-template` §3.4; 9×9 yorumu `dependency-rule-template` §3.3; buraya **kopyalanmaz**.

| Yön | Hedefler | Koşul | Sınır Testi |
|-----|----------|-------|-------------|
| İzinli (`✓`) | K1 Abstractions (+ §3.2 alt katmanları) | doğrudan `ProjectReference` | §5.3/2 tutarlı |
| Koşullu (`△`) | Application/Infrastructure port eşlemesi | yalnız arayüz; somut tip yok | §5.3/4 `using` temiz |
| Yasak (`✗`) | UI, Test ve yetkisiz katmanlar | hiçbir koşulda yok | §5.3/3 = 0 |
| Dışa | BCL + CPM paketi (`Directory.Packages.props`) | tek sürüm kaynağı | paket adı ADR'ye bağlı |

**Katmana özel kırmızı çizgiler** (hiçbiri esnetilemez):

| # | Kırmızı Çizgi | Gerekçe |
|---|---------------|---------|
| R1 | Infrastructure → Domain yalnız entity/`Result` | port/adapter (ADR-004) |
| R2 | Infrastructure → UI **asla** | ters akış, döngü riski |
| R3 | UI (Desktop/Cli) → yalnız Host | katmanlara doğrudan erişim yok |
| R4 | Domain → `System.Data` / Dapper / dosya / git **asla** | saf C# katmanı |
| R5 | Herhangi bir katman → Test **asla** | üretim kodu K8'e bakamaz |
| R6 | CrossCutting'e yalnız Host ve Test bakar | en alt sabit |

### §3.5 Public API Yüzeyi (arayüz + imza tablosu)

| # | Arayüz | Metot İmzası | Dönüş | Sözleşme |
|---|--------|--------------|-------|----------|
| 1 | `IItemRepository` | `GetAsync(Guid id, CancellationToken ct)` | `Task<Result<Item>>` | yoksa `Fail(NotFound)`, null değil |
| 2 | `IItemRepository` | `SearchAsync(string term, int take, CancellationToken ct)` | `Task<Result<IReadOnlyList<Item>>>` | `take` 1..100 |
| 3 | `IItemRepository` | `AddAsync(Item item, CancellationToken ct)` | `Task<Result<Guid>>` | invariant ihlali `Conflict` |
| 4 | `IUnitOfWork` | `CommitAsync(CancellationToken ct)` | `Task<Result<bool>>` | rollback en fazla 1 |
| 5 | `IAdoConnectionFactory` | `Open()` | `IDbConnection` | `using` ile kapanır |
| 6 | `IClock` | `UtcNow` | `DateTimeOffset` | testte sabitlenir |

Kurallar: herkese açık yüzey **arayüz** üzerinden tanıtılır · `CancellationToken`sız asenkron imzanın gerekçesi yazılır · `Result<T>` tek dönüş tipidir.

### §3.6 Girdi/Çıktı Sözleşmesi ve Hata Yönetimi

| Durum | Mekanizma | Örnek | Kural |
|-------|-----------|-------|-------|
| İş kuralı ihlali | `Result<T>` + `Error` | `Result.Fail(Error.Conflict(...))` | exception **değil** |
| Bulunamayan kayıt | `Result<T>` `NotFound` | `GetAsync` → `Fail` | `null` dönmez |
| Altyapı hatası | yakalanır → `Fail` + log | Dapper hatası | dışarı **sızmaz** |
| İptal / zaman aşımı | `CancellationToken` + tek retry | 1 deneme | 2. retry yasak |
| Programlama hatası | `ArgumentException` (fail-fast) | null argüman | yakalanmaz, test edilir |
| Girdi doğrulama | `Result.Invalid` + alan listesi | form hatası | mesaj Türkçe |

Sözleşme: **Çıkan tek şey `Result<T>` ya da programa ait istisnadır; üçüncü hata yolu (boş `null`, `0`, sessiz `catch`) yok.**

### §3.7 Performans Bütçesi ve Kilit Kaynaklar

| # | Kaynak | Bütçe / Limit | Kilit Kuralı | Ölçüm |
|---|--------|---------------|--------------|-------|
| P1 | Tek çağrı gecikmesi | ≤ 5 ms (katman içi) | — | stopwatch |
| P2 | Veritabanı sorgusu | ≤ 50 ms (yerel SQLite) | bağlantı çağrı bazlı | log süresi |
| P3 | Thread | UI thread'e dokunma yok | `async/await` tutarlı | code review |
| P4 | Dosya | tek yazıcı | `FileShare.None` + `using` | kilit ihlali testi |
| P5 | Bellek | ≤ 256 MB | bağlantı/stream `Dispose` | `dotnet-counters` |
| P6 | Eşzamanlılık | aynı anda 1 transaction | `IUnitOfWork` tekil | entegrasyon testi |

### §3.8 Test Stratejisi, Kabul Kriterleri ve DoD

| Test Türü | Katman | Ne Kapsar | Hedef |
|-----------|--------|-----------|-------|
| Unit | `tests/VersaCoder.Tests` | sınır mantığı, `Result` yolları, invariant | **%90** satır |
| Integration | `tests/VersaCoder.IntegrationTests` | gerçek `versacoder.db` + `@pN` sorguları | her port ≥ 1 yol |
| Arch | mimari test (§5.3/3–4) | yasak `ProjectReference` = 0, yasak `using` = 0 | 0 ihlal |
| Perf | §3.7 bütçesi | ms değerleri | bütçe aşılmaz |

Kabul kriterleri:

- [ ] `layer-<ad>.md` 7 bölüm eksiksiz; §1.2 her satır `✅`/`❌`/`⚠️`
- [ ] §3.2 kimlik satırı `architecture-template` §3.2 ile birebir
- [ ] §3.3 Tablo A ≥ 6 ve Tablo B ≥ 6 dolu satır
- [ ] §3.4 izin/yasak listesi §5.3/2–3 ile tutarlı (yasak kenar 0)
- [ ] §3.5 imza tablosu ≥ 6 satır; dönüş `Result<T>` ya da gerekçeli istisna
- [ ] §3.7 bütçe sayısal (ms) · §3.8 hedef %90 yazılı
- [ ] §5.3 komutları çalıştırıldı, çıktılar §5.4'e işlendi
- [ ] Yer tutucu yalnız §3.1 · mojibake 0 · kutu (varsa) 66

**Definition of Done (DoD):**

| # | Koşul | Kanıt |
|---|-------|-------|
| 1 | Şartname `accepted` | §6.3 geçişi + `log.md` |
| 2 | Kod şartnameyle uyumlu | §5.3/2–4 = 0 ihlal |
| 3 | Test ≥ %90 + derleme temiz | `dotnet test` çıktısı |
| 4 | İhlal düzeltildi, yeniden ölçüldü | §5.3 tekrar çıktısı |
| 5 | Envanter `YAZILDI` + KB ölçüldü | `index.md` satır #14 |

### §3.9 Dolu VersaCoder Örneği — `VersaCoder.Infrastructure`

> Gerçek VersaCoder verisiyle doldurulmuş **K4** şartnamesi. `src/` diskte olmadığı için (§1.2 E9) imza ve ms değerleri **taslaktır**; kod yazıldığında §5.3/2–6 ile güncellenir.

| Alan | Değer |
|------|-------|
| Kimlik | `K4 · Infrastructure · VersaCoder.Infrastructure · database-optimizer · net48;net10.0-windows · layer-infrastructure.md` |
| NE YAPAR | ADO.NET + Dapper ile `versacoder.db` · `@p1..@pN` sorgu · `I*Repository` portları · migration `V0xx__*.sql` · `schema_version` |
| NE YAPMAZ | EF Core / `DbContext` (ADR-004) · UI tipine bakmak · concat SQL · use case · DevExpress · secret |
| İzinli bağımlılık | `✓` K2, K1 · `△` K3 (yalnız port eşlemesi) · `✗` K5, K6, K7, K0, K8 |
| Kırmızı çizgi | `Microsoft.EntityFrameworkCore` geçen satır = 0 · her sorgu `@p1/@p2` içerir · `System.Data` yalnız bu katmanda |
| Public API | `IItemRepository.GetAsync(Guid, CancellationToken)` → `Task<Result<Item>>` · `IUnitOfWork.CommitAsync(CancellationToken)` → `Task<Result<bool>>` |
| Hata yolu | `SqliteException` → `Result.Fail(Error.Database(...))` + log · null argüman → `ArgumentException` · exception dışarı **çıkmaz** |
| Performans | sorgu ≤ 50 ms · açık bağlantı kalmaz · kil `versacoder.db-wal` · retry ≤ 1 |
| Test | unit: port mock `%90` · integration: gerçek SQLite + `@pN` · arch: §5.3/3 yasak kenar = 0 |
| İlgili karar | ADR-004 (veri yığını) · ADR-001 (DevExpress — bu katman **kullanmaz**) · ⚠️ numaralar diskte doğrulanmadı (§1.2 E8) |

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Sıra | Ne Yapılır | Çıktı | Yapılmazsa |
|---|------|-----------|-------|------------|
| 1 | Kanıt + şablon + kimlik | §1.2 tarama, §2.2, §3.2 | kanıt tablosu + K kodu | kanıtsız iddia (G5) |
| 2 | Sınırı yaz | §3.3 Tablo A + B | 12+ satır | "yapar" şişer, "yapmaz" kaybolur |
| 3 | Bağımlılığı bağla | §3.4 + kırmızı çizgiler | izin/yasak listesi | ters referans gizlenir |
| 4 | Yüzey + sözleşme | §3.5–§3.7 | 3 tablo | API sonradan kırılır |
| 5 | Test + kabul | §3.8 | %90 + checkbox + DoD | kanıtsız "bitti" |
| 6 | Doğrula | §5.3 komutları + §6.1 | 0 ihlal | kanıtsız "tamam" |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | `architect` | §3.2–§3.7 dolu + yön tutarlı | `draft` | 35 dk |
| 2 | `reviewer` | §6.1 checklist + §5.3 çıktıları | rapor | 20 dk |
| 3 | `testing` | §3.8 %90 etkisi + test katmanı | not | 15 dk |
| 4 | `master-orchestrator` | `GATE-004` + §5.4 Quality Report | `accepted` + `log.md` | 15 dk |

**Toplam 85 dakika.** 3 kırmızı sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir ve §6.2'ye dönülür.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır / KB | 480–660 / 25–34 | §5.2 H8 |
| Bölüm | §1…§7 + §1.1–1.4, §2.1–2.2, §3.1–3.9, §4.1–4.5, §5.1–5.4, §6.1–6.5, §7.1–7.2 | eksik → red |
| Tablo / Kutu | her §'de ≥ 1 tablo, toplam ≥ 25 satır · kutu (varsa) 66, `-|++++` | az → §6.1 K9 · hizasız → `ascii-diagram` G2 |
| Kodlama | UTF-8 BOM'suz | mojibake → otomatik red |

### §4.4 Değişiklik Kontrolü

| Değişiklik | `version` | `status` | Ek İşlem |
|------------|-----------|----------|----------|
| Yazım/ımle hatası, dolgu, kırık bağlantı | `patch` | Aynı | `updated` değişmez; §5.3/6 yeniden ölç |
| Yeni tablo/alt bölüm (`§x.y`) | `minor` | Aynı | §5.4 Quality Report |
| Katman sınırı ya da izin değişti | **`major`** | `draft` → `review` | ADR + `dependency-rule` + `architecture-template` yeniden üretilir |
| Port eklendi/çıktı | `minor` | Aynı | §3.5 + mimari test |
| Şartname `accepted` oldu | `minor` | `accepted` | `.ai/log.md` tek satır |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Yasaklı vault'lardan cümle kopyalanamaz; yalnız **yapı** referans | Şartname **reddedilir**, satır silinir |
| G2 | EF Core / `DbContext` anlatılamaz — ADO.NET + Dapper | **DUR** — §3.9 ADR-004 satırı düzeltilir |
| G3 | Frontmatter 7 alan + `authority: primary` ilk 9 satırda | **DUR** — dosya `draft` kalır |
| G4 | Yer tutucu yalnız §3.1 bloğunda; şartnamede = 0 | **DUR** — doldurulmadan teslim yok |
| G5 | §1.2 kanıtı olmadan "var / uygulanmıştır" iddiası | `⚠️ VERIFICATION REQUIRED` + §1.2 satırı |
| G6 | 480–660 satır / 25–34 KB dışı | **DUR** — §5.3/6 ölçümü geçene kadar |
| G7 | Kırık bağlantı (`](…)` hedefi diskte yok) | **DUR** — hedef üretilir ya da bağlantı silinir |
| G8 | TFM yalnız `net48` · `net10.0-windows` (3. TFM yok) | **DUR** — ADR-005/006'ya dön |
| G9 | `.ai/adr/` dışında karar belgesi | **DUR** — karar ADR'ye taşınır |
| G10 | String concat SQL anlatılamaz; örnek `@pN` içerir | **DUR** — §3.9 yeniden yazılır |
| G11 | `GATE-004` etiketi yokken "onaylandı" denemez | `draft` + §6.3'e dön |
| G12 | Mojibake, karışık dil ya da başka ürün adı | Dosya UTF-8 BOM'suz yeniden yazılır |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Disk taraması + boşluk işaretle | `.ai/`, `src/`, `tests/`, kök | §1.2 kanıt tablosu + `⚠️` | `architect` | 15 dk |
| 2 | Şablon + katman seç | §2.2 S1..S5, §3.2 | doğru şablon + K kodu | `architect` | 3 dk |
| 3 | Sınır + bağımlılık | §3.3, §3.4 | 12+ satır + izin/yasak | `architect` | 30 dk |
| 4 | API + hata + performans | §3.5–§3.7 | 3 tablo | `developer` | 20 dk |
| 5 | Test + kabul + DoD | §3.8 | %90 + checkbox | `testing` | 12 dk |
| 6 | Doğrulama | §5.3 komut 1–6 | 0 ihlal | `reviewer` | 10 dk |
| 7 | Checklist + rapor + kabul | §6.1, §5.4 | işaretli liste + `accepted` | `reviewer` → `master-orchestrator` | 18 dk |

**Zaman bütçesi:** 1–2 ≈ 18 dk · 3–5 ≈ 62 dk · 6–7 ≈ 28 dk → **≈ 108 dk/katman** (§4.2: 85 dk onay + %27 tampon). 120 dk aşımı §1.3 sapmasıdır: §3.3'e dönülür.

### §5.2 Hata Modları

| Kod | Hata Modu | Belirti | Kurtarma | Guardrail |
|-----|-----------|---------|----------|-----------|
| H1 | Kanıtsız iddia | §1.2 `⚠️` varken kesin ifade | komutla kanıt ya da iddiayı sil | G5 |
| H2 | "YAPAR" şişkinliği | Tablo A'da muğlak 15+ satır | ölçülebilir yap ya da sil | §3.3 |
| H3 | Sınır tek tablo | YAPAR/YAPMAZ birleşik | iki ayrı tabloya böl | §3.3 |
| H4 | Ters bağımlılık | §3.4'te yasak hedef `✓` | `dependency-rule` §3.3'e göre düzelt | G6 |
| H5 | EF Core / concat SQL sızması | §3.9'da ORM cümlesi | ADO.NET + Dapper + `@pN` | G2, G10 |
| H6 | Port'suz API | §3.5 somut sınıf listesi | arayüz sütununu zorunlu kıl | §3.5 |
| H7 | Yer tutucu sızıntısı | §3.1 dışında `{{…}}` | dışarıdakini sil, yeniden ölç | G4 |
| H8 | Uzunluk/KB aşımı | §5.3/6 > 660 satır ya da > 34 KB | içeriği değil **tekrarı** kısalt | G6 |
| H9 | Kanıtsız "bitti" | DoD işaretli, komut çıktısı yok | §5.3'ü çalıştır, çıktıyı işle | §5.3/6 |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**.

```powershell
# 1) PROJE + TFM ENVANTERI — kaynak yoksa "DIZIN YOK" der
$src = 'C:\versacoder\src'
if (-not (Test-Path $src)) { 'DIZIN YOK -> 0 proje (kanit: §1.2 E9)' }
else { Get-ChildItem $src -Recurse -Filter *.csproj | ForEach-Object {
  $t = (Select-String -Path $_.FullName -Pattern '<TargetFrameworks?>([^<]+)' | Select-Object -First 1).Matches.Groups[1].Value
  [PSCustomObject]@{ Proje = $_.BaseName; TFM = $t } } | Format-Table -AutoSize }
```

```powershell
# 2) PROJECTREFERENCE TARAMASI — bu katmanin tum kenarlari listelenir
Get-ChildItem 'C:\versacoder' -Recurse -Filter *.csproj -ErrorAction SilentlyContinue | ForEach-Object {
  $k = $_.BaseName
  Select-String -Path $_.FullName -Pattern 'ProjectReference Include="([^"]+)"' -AllMatches |
    ForEach-Object { $_.Matches } | ForEach-Object {
      [PSCustomObject]@{ Kaynak = $k; Hedef = [IO.Path]::GetFileNameWithoutExtension($_.Groups[1].Value) } } } |
  Sort-Object Kaynak, Hedef | Format-Table -AutoSize
```

```powershell
# 3) YASAK KENARLAR — cikti 0 satir OLMALI (matris: dependency-rule §3.3)
$yasak = 'VersaCoder\.Domain\s*->\s*VersaCoder\.(Application|Infrastructure|Protocol|Host|Desktop|Cli)',
  'VersaCoder\.Application\s*->\s*VersaCoder\.(Infrastructure|Host|Desktop|Cli)',
  'VersaCoder\.(Abstractions|CrossCutting)\s*->\s*VersaCoder\.',
  'VersaCoder\.(Desktop|Cli)\s*->\s*VersaCoder\.(Domain|Application|Infrastructure|Protocol|CrossCutting)',
  'VersaCoder\.Host\s*->\s*VersaCoder\.(Desktop|Cli)'
Get-ChildItem 'C:\versacoder' -Recurse -Filter *.csproj -ErrorAction SilentlyContinue | ForEach-Object {
  $k = $_.BaseName
  Select-String -Path $_.FullName -Pattern 'ProjectReference Include="([^"]+)"' -AllMatches |
    ForEach-Object { $_.Matches } | ForEach-Object {
      $h = [IO.Path]::GetFileNameWithoutExtension($_.Groups[1].Value)
      foreach ($r in $yasak) { if ("$k -> $h" -match $r) { "YASAK KENAR -> $k -> $h" } } } }
```

```powershell
# 4) NAMESPACE SINIRI — katman disi "using" 0 satir OLMALI
$kalip = @{ 'VersaCoder.Domain' = 'VersaCoder\.(Application|Infrastructure|Protocol|Host|Desktop|Cli|CrossCutting)'
  'VersaCoder.Application' = 'VersaCoder\.(Infrastructure|Host|Desktop|Cli)'
  'VersaCoder.Infrastructure' = 'VersaCoder\.(Host|Desktop|Cli)'
  'VersaCoder.Desktop' = 'VersaCoder\.(Domain|Application|Infrastructure|Protocol)' }
foreach ($d in $kalip.Keys) {
  $yol = Join-Path 'C:\versacoder\src' $d
  if (-not (Test-Path $yol)) { "DIZIN YOK -> $d (§1.2 E9)"; continue }
  Get-ChildItem $yol -Recurse -Filter *.cs -ErrorAction SilentlyContinue |
    Select-String -Pattern ('^\s*using\s+' + $kalip[$d]) |
    ForEach-Object { "USING IHLALI -> {0}:{1}" -f $d, $_.LineNumber } }
```

```powershell
# 5) VERI YIGINI — EF Core 0, concat SQL 0 (ADR-004)
if (-not (Test-Path 'C:\versacoder\src')) { 'DIZIN YOK -> kod kaniti yok (§1.2 E9)' }
else { $kod = Get-ChildItem 'C:\versacoder\src','C:\versacoder\tests' -Recurse -Include *.cs -ErrorAction SilentlyContinue
  $kod | Select-String -Pattern 'EntityFramework|DbContext' | ForEach-Object { "EF YASAK -> {0}:{1}" -f $_.Filename, $_.LineNumber }
  $kod | Select-String -Pattern 'SELECT[^"`+]*\+\s*(str|@|\$)' | ForEach-Object { "CONCAT SQL -> {0}:{1}" -f $_.Filename, $_.LineNumber } }
```

```powershell
# 6) OLCUM — 480-660 satir · 25-34 KB · §=7 · mojibake=0 · kutu=66
$f = 'C:\versacoder\.ai\.templates\architecture\layer-spec-template.md'
$lines = Get-Content -LiteralPath $f -Encoding UTF8
$hizasiz = ($lines | Where-Object { $_ -match '^[+|]' -and $_.Length -ne 66 }).Count
[PSCustomObject]@{ Satir = $lines.Count; KB = [math]::Round((Get-Item -LiteralPath $f).Length / 1KB, 1);
  Bolum = (Select-String -Path $f -Pattern '^## §[1-7] ').Count;
  Mojibake = (Select-String -Path $f -Pattern '[\xC2\xC3][\x80-\xBF]').Count; KutuHizasiz = $hizasiz }
```

**Geçme kriteri:** 1 → TFM yalnız iki değer · 2 → kenar listesi §3.4 ile tutarlı · 3, 4, 5 → **0 satır** (dizin yoksa dürüst `DIZIN YOK`) · 6 → `480–660`, `25–34`, `7`, `0`, `0`.

### §5.4 Quality Report

```markdown
## Quality Report — layer-spec-template (YYYY-MM-DD HH:MM)
- Ölçüm:     <satır> satır / <KB> KB  (hedef 480–660 · 25–34)  ·  §1..§7 = <7/7>
- Tablo:     <N> satır (≥25)  ·  Kutu: <0 uyumsuz> (66)  ·  Yer tutucu: 0 (§3.1 dışı)
- Kanıt:     §1.2 → ✅ <N> · ❌ <N> · ⚠️ <N>  ·  yorum satırı 0
- Komut:     §5.3 1–6 → yasak kenar <0> · yasak using <0> · EF <0> · mojibake 0
- Guardrail: G1..G12 ihlal yok  ·  Red sinyali §6.2 → 0  ·  Checklist §6.1 → <NN>/<NN>
- Onay/Karar: architect → reviewer → testing → master-orchestrator · Gate GATE-004 · Part B · ONAYLANDI | RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 7 alan eksiksiz | 7/7 | G3 |
| K2 | `authority: primary` birebir | `SSOT` | G3 |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (6 wiki bağlantı) | 1 satır | §3.1 |
| K4 | §1.1–§1.4 dolu; §1.4 = Özet tablosu | 4/4 | §3.7 |
| K5 | §1.2 her satır `✅`/`❌`/`⚠️` + yorum yasağı | 0 boş hücre | G5 |
| K6 | §2.1 dosya tipi → sorumlu · §2.2 S1..S5 cevaplı | 5/5 | §2.2 |
| K7 | §3.1 kalıp bloğu dört tırnak içinde; `{{…}}` yalnız orada | 0 dışarıda | G4 |
| K8 | §3.2 kimlik = K0..K8'den **tek** satır | 1 işaretli | §3.2 |
| K9 | §3.3 Tablo A ≥ 6 ve Tablo B ≥ 6 dolu | 2 tablo | §3.3 |
| K10 | §3.4 izin/yasak + kırmızı çizgi ≥ 6 satır | dolu | §3.4 |
| K11 | §3.5 imza tablosu ≥ 6 satır | dolu | §3.5 |
| K12 | §3.6 `Result<T>`/exception matrisi ≥ 6 satır | dolu | §3.6 |
| K13 | §3.7 performans + kilit ≥ 6 satır, sayısal ms | dolu | §3.7 |
| K14 | §3.8 test %90 + kabul checkbox ≥ 8 + DoD ≥ 5 | dolu | %90 |
| K15 | §3.9 dolu VersaCoder örneği (K4) mevcut | var | §3.9 |
| K16 | §4.5 G1..G12, "İhlal Sonucu" sütunu boş değil | 12/12 | §4.5 |
| K17 | §5.1 7 adım + zaman bütçesi | dolu | §5.1 |
| K18 | §5.2 hata modu ≥ 8 | 9 | §5.2 |
| K19 | §5.3 ≥ 5 salt-okunur komut (`Get-*`/`Select-*`/`Test-Path`) + §5.4 Quality Report | 6 + var | §5.3, §5.4 |
| K20 | §6.5 envantere özgü · §7.2 Red Team ≥ 10 satır · uzunluk 480–660 / 25–34 KB · mojibake 0 · kutu 66 | dolu / geç | §7.2, G6, G12 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Sonuç | Sorumlu |
|---|--------|-------|---------|
| R1 | §1.2 `⚠️` varken §3.9'da kesin "uygulanmıştır" | **DUR** — iddia `⚠️`'ye çekilir | `architect` |
| R2 | §3.2'de 1'den fazla K kodu işaretli | **DUR** — şartname = tek katman | `architect` |
| R3 | Bölüm adları bu şablonla uyuşmuyor | **DUR** — §1..§7 eşleşmeli | `reviewer` |
| R4 | `EntityFramework` / `DbContext` geçen satır | **DUR** — G2 ihlali | `reviewer` |
| R5 | YASAK KENAR çıktısı > 0 (§5.3/3) | **DUR** — §3.4 yeniden yazılır | `architect` |
| R6 | Kutu satırı ≠ 66 | **DUR** — `ascii-diagram` §5.3/1 | `architect` |
| R7 | Yer tutucu §3.1 dışında · uzunluk dışı | **DUR** — G4, G6 | `reviewer` |
| R8 | Komut çıktısız "tamam/onaylandı" · başka ürün adı | **DUR** — §5.3 çıktısı / satır silinir | `master-orchestrator` |

### §6.3 Tür / Sürüm Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|-----------|------|
| `draft` → `review` | §6.1 20/20 · §5.3 1–6 geç · §1.2 eksiksiz | `architect` | taslak denetime gider |
| `review` → `accepted` | yasak kenar 0 · kırmızı çizgiler testte · %90 yazılı | `master-orchestrator` | `GATE-004` + `log.md` |
| `accepted` → `superseded` | sınır değişti ya da ADR supersede | `architect` + orchestrator | yeni şartname, eskisi işaretlenir (silinemez) |
| `major` sürüm | sınır/izin değişikliği | ADR zorunlu | `dependency-rule` + `architecture-template` güncellenir |
| `minor` / `patch` | port/tablo eklendi · yazım hatası | `reviewer` / yazar | Quality Report · `updated` değişmez |

### §6.4 İlgili Dosyalar

| Dosya | İlişki | Ne Verir |
|-------|--------|----------|
| `.ai/.templates/index.md` · `CLAUDE.md` | Envanter + dizin kuralı | satır #14 · Part B · §3.7 · erişim sırası, ≥480, 14 rol |
| `.ai/.templates/architecture/architecture-template.md` | Kardeş | K0..K8 kartı + yön tablosu (§3.2, §3.4 kaynağı) |
| `.ai/.templates/architecture/dependency-rule-template.md` | Kardeş | 9×9 matris, ihlal komutu, ban listesi |
| `ascii-diagram-template.md` · `adr/adr-architecture-template.md` | Kardeş | kutu alfabesi + 66 hizası · ADR biçimi |
| `.ai/architecture/layer-<ad>.md` · `.ai/log.md` | **üretilen** | ikisi de diskte YOK (§1.2 E5, E7) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Komut / Yöntem |
|---|---------|----------|----------------|
| K1 | Dosya envanterde #14 mü | `layer-spec-template` · `T-MIM` · Part B · `PLANLI` | `index.md` §3.3 |
| K2 | Satır + KB aralığı (envanter tahmini `16,0`) | 480–660 / 25–34; **ölçüm kazanır**, tahmin `docs-writer` günceller | §5.3/6 |
| K3 | Yer tutucu yalnız §3.1 | 0 dışarıda | §5.3/6 + gözden geçirme |
| K4 | Yasaklı ad geçen satır | 0 | `Select-String -Path $f -Pattern '[Cc]ore[Mm]usic'` |
| K5 | Envanter satırı üretim sonrası | `PLANLI → YAZILDI` + ölçülü KB | `docs-writer` (bu dosya güncellenmez) |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| # | Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|---|--------|-----|---------|---------------------|
| 1 | Envanter + dizin kuralı | `index.md` · `CLAUDE.md` | satır #14, §3.7 · ≥480, 7 bölüm, 14 rol | ✅ 606 / 132 satır |
| 2 | Biçim altın örneği | `architecture/ascii-diagram-template.md` | 7 bölüm yoğunluğu, 66 hizası | ✅ 38.121 B |
| 3 | Kardeşler | `architecture-template.md` · `dependency-rule-template.md` | K0..K8 kartı, yön, matris | ✅ 34.059 B · bu Part'de yazılıyor |
| 4 | İkinci format referansı | `adr/adr-template.md` | §1.2 kanıt disiplini, §5.3 komutları | ✅ 39.870 B |
| 5 | Katman / gate / kural / karar / kod gerçeği | `.ai/architecture/index.md` · `WORKFLOW.md` · `.rules/*.md` · `adr/ADR-*` · `src/**` | §3.2, `GATE-004`, G2/G10, §5.3 hedefleri | ❌ / ⚠️ (§1.2 E6–E10) |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Katman sınırını tek tabloda anlatmak | "yapar" şişer, "yapmaz" görünmez | §3.3 **iki ayrı tablo** (A ≥ 6, B ≥ 6) |
| 2 | Şartnameye 9×9 matrisi kopyalamak | İki SSOT doğar, biri yalan söyler | matris `dependency-rule`'de; burada tek katman satırı |
| 3 | Genel mimariyi buraya tekrar yazmak | K0..K8 yığını iki yerde yaşar | yalnız **bir** K kodu işaretlenir (§3.2) |
| 4 | UI'ın Infrastructure'e erişimine izin vermek | Katman fikri koddan kopar | R2 + §5.3/3 yasak kenar |
| 5 | EF Core geçmişi bırakmak | ADR-004 ihlali, kopya kaynak | ADO.NET + Dapper + `@p1..@pN` |
| 6 | Exception'ı tek hata yolu yapmak | UI'da iş kuralı hatası patlar | `Result<T>` + fail-fast istisna (§3.6) |
| 7 | `⚠️` satırlarına yorum eklemek | Kanıtsız iddia süslenir | yorum yasağı (§1.2); boşluk komutla kapatılır |
| 8 | Dosyayı yasaklı vault altına kaydetmek | Yanlış depo, geri alınamaz karmaşa | `C:\versacoder\.ai\architecture\` |
| 9 | Yer tutucularla teslim etmek | Şartname okunamaz, üretim zinciri kırılır | `{{…}}` yalnız §3.1 (G4) |
| 10 | Komut çalıştırmadan "0 ihlal" demek | Kanıtsız iddia (master prompt §6/5) | §5.3 çıktılarını §5.4 raporuna yaz |

**SSUT beyanı:** Bu dosya tek katman şartnamesinin tek kaynağıdır; yığın `architecture-template`'in, matris `dependency-rule-template`'indir — `.ai/architecture/index.md` yalnız buna bağlanır (tekrar = iki gerçek kaynağı doğurur, envanter G12).
