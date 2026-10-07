---
title: "VersaCoder — Oturumlar Arası Kalıcı Bellek (Cross-Session Memory)"
type: memory
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Oturumlar arasında kaybedilmemesi gereken sabit proje gerçekleri, güncel durum anlık görüntüsü, 'bir daha yapma' listesi, dosya haritası ve başlangıç/bitiş protokolü"
---

# VersaCoder — Kalıcı Bellek (`MEMORY.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[WORKFLOW.md]] · [[ROLE.md]] · [[AGENTS.md]] · [[brain.md]] · [[glossary.md]] · [[log.md]] · [[reports/00-kapsam-analizi.md]]

> **Tek cümlelik sorumluluk:** *Yeni bir oturum sıfırdan başlasa bile ilk 2 dakikada bilmesi gerekenler* sorusunun tek cevabıdır.
> Bu dosya **durum ve hatırlatma** tutar; **karar gerekçesi `brain.md`**, **kural `CLAUDE.md`**, **süreç `WORKFLOW.md`**, **geçmiş `log.md`** üzerindedir (SRP).

---

## §0 Özet (Answer First)

Proje **VersaCoder** — `net48` + `net10.0-windows` iki bacaklı, WinForms + DevExpress **v26.1**, ADO.NET + **Dapper** (EF Core **yasak**) bir **agentic IDE**. Vault kökü `C:\versacoder\.ai`. Durum: **(2/8) part raporlandı**, **GATE-001 ✓ · GATE-002 ✓**, **GATE-003 BEKLİYOR = kodlama öncesi HARD STOP**. Git deposunda **hiç commit yok**, tek untracked girdi `?? .ai/`.

| Ne | Değer |
|---|---|
| Ürün | VersaCoder — AI Coding Platform / agentic IDE |
| Bacaklar | `net48` + `net10.0-windows` (ortak kaynak, `#if` yalnız platform farkı) |
| Yasak | **EF Core** · DevExpress v26.1 dışı · Office2010 dışı ribbon · kopyala-yapıştır · kanıtsız "bitti" |
| Vault | `C:\versacoder\.ai` (A1 · kullanıcı onaylı) |
| Durum | (2/8) · GATE-001 ✓ · GATE-002 ✓ · **GATE-003 bekliyor** |
| Git | `main` · **0 commit** · `?? .ai/` |

---

## §1 Bu Dosya Ne — Ne Değil

| Soru | Bu dosyada mı? | Nerede |
|---|---|---|
| Sabit proje gerçeği (yığın, yol, ekran) | **Evet** | §2 |
| Şu an ne durumdayız | **Evet** | §3 |
| Aynı hatayı tekrarlama | **Evet** | §4 |
| Hangi bilgi hangi dosyada | **Evet** | §5 |
| Oturum başlangıç/bitiş protokolü | **Evet** | §6–§7 |
| Kararın **gerekçesi** | Hayır | `brain.md` |
| Kuralın **metni** (G1..G12) | Hayır | `CLAUDE.md` |
| Part/geçiş sırası | Hayır | `WORKFLOW.md` · `engine.md` |
| Değişiklik **geçmişi** | Hayır | `log.md` |
| Ham konuşma kaydı (transcript) | Hayır | oturum kaydı — bu dosyaya **kopyalanmaz** |

> **KURAL:** Bu dosyaya yalnız **"oturum başında bilinmesi zorunlu"** olan girer. Ayrıntılı gerekçe buraya taşınırsa `brain.md` ile çakışır → tekrar silinir.

---

## §2 Kalıcı Proje Gerçekleri

### §2.1 Kimlik

| # | Gerçek | Değer |
|---|---|---|
| 1 | Ürün adı | **VersaCoder** (CoreMusic **değil**) |
| 2 | Kategori | AI Coding Platform / **agentic IDE** |
| 3 | İlham | Visual Studio benzeri masaüstü IDE |
| 4 | Hedef kitle | kod üreten/çalıştıran ajanlarla çalışan geliştirici |
| 5 | Ürün metni / marka kararları | **kullanıcıya ait** (AGENTS.md §3.2 #12) |
| 6 | Bootstrap kaynağı | `versacoder proje planaması ai.01/02/03.md` — **ai.03 = birincil** |

### §2.2 Hedefler

| # | Hedef | Ölçüt |
|---|---|---|
| 1 | İki bacakta da çalışmak | `dotnet build` → **0 hata** (`net48` + `net10.0-windows`) |
| 2 | Testler yeşil | `dotnet test` → tümü geçer + coverage kanıtı |
| 3 | Vault link bütünlüğü | link taraması → **0 bozuk link** |
| 4 | Teslim kanıtı | `reports/09-teslim-raporu.md` — build/test çıktılı |
| 5 | Yığın sadakati | EF Core referansı **0** · DevExpress **yalnız v26.1** · string-concat SQL **0** |
| 6 | Vault ölçeği | 14 kök belge · 30–88 KB bant · ~500 MD hedefi |

### §2.3 Yasaklı teknoloji (asla önerilmez, asla eklenmez)

| # | Yasaklı | Yerine | Kural |
|---|---|---|---|
| 1 | **EF Core** (`Microsoft.EntityFrameworkCore*`, `DbContext`, LINQ-to-Entities) | **ADO.NET + Dapper** | `CLAUDE.md` **G2** |
| 2 | String-concat SQL (`$"SELECT …{x}"`) | parameterized + `CommandTimeout` | G2 / güvenlik |
| 3 | DevExpress **v26.1 dışı** (ör. 24.2.3) | DevExpress Universal **v26.1** | **G3** |
| 4 | Office2010 dışı ribbon stili · `XtraForm` ana pencere | `RibbonControlStyle.Office2010` + `RibbonForm` | G3 |
| 5 | Tek bacaklı tasarım · `#if` ile iş mantığı bölme | Ortak kaynak, `#if` yalnız platform farkı | **G6** |
| 6 | Yığın dışı paket/framework/ORM/JS framework | Mevcut yığın (C# · WinForms · v26.1 · Dapper · xUnit) | **G8** |
| 7 | EF migrate · Elle olmayan migration | Saf SQL + `schema_version` | G2 |
| 8 | Proje başına NuGet sürümü | CPM `Directory.Packages.props` | G8 |
| 9 | CoreMusic vault'una yazma / içerik kopyası | Yalnız **yapı** referansı | **G1** |

### §2.4 Vault yolları

| # | Yol | Ne | Durum |
|---|---|---|---|
| 1 | `C:\versacoder\.ai` | **Vault kökü** (A1) | ✅ kullanıcı onaylı |
| 2 | `C:\versacoder` | Git repo kökü | ✅ mevcut, **0 commit** |
| 3 | `C:\www\coremusic.net\.ai` | Yapı referansı — **SALT OKUNUR** | yasak yazma (G1) |
| 4 | `C:\www\versacoder\versacoder.ai` | Bilgi referansı — kopya yok | yalnız okuma |
| 5 | `G:\…\Versacoder\` (Project-notes) | Bootstrap `ai.01/02/03.md` dizini | okuma anında normalize (R2) |
| 6 | `%TEMP%\opencode-fresh` | opencode v1.18.32 · 33 paket | PART 7 kaynağı |
| 7 | `C:\www\versacoder\versacoder` | **BOŞ** — vault buraya DEĞİL `C:\versacoder\.ai`'ye kondu | — |

**Bootstrap kaynak envanteri (karar girdisi — sabit):**

| Dosya | Boyut | Satır | Satır sonu | Durum |
|---|---|---|---|---|
| `versacoder proje planaması ai.01.md` | 150.166 B | 5.415 | LF (temiz) | **ESKİ yığın** — yalnız yapıbilgi |
| `versacoder proje planaması ai.02.md` | 123.721 B | 4.087 | LF+CRLF karışık | Discovery/Q&A — 5 kararın kaynağı |
| `versacoder proje planaması ai.03.md` | 21.984 B | 1.543 | **CR-only** | **YENİ bootstrap — birincil** (R2) |
| `opencode kaynak kodu editle...geit.md` | 122.873 B | — | — | PART 7 kaynağı |
| `opencode kaynak kodu editle...geit.02.md` | 34.638 B | — | — | PART 7 kaynağı |
| `notes.prompt.md` | 8.945 B | 154 | LF | master prompt kaynağı |
| `notes.prompt2.txt` | 27.679 B | 1.569 | — | Q&A ham veri (5 karar) |

Dizin: `G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\`

> **UYARI:** ai.01'den **teknoloji sürümü asla alınmaz** (§4 madde 2); yalnız yapıbilgi okunur.

### §2.5 Katman sırası (değişmez)

```
Abstractions -> Domain -> Application -> Infrastructure -> Protocol -> Host -> UI
     ^             ^          ^               ^              ^          ^      ^
   sozlesme    is kurali   use case      ADO.NET+Dapper   mesaj/     DI/     WinForms
   Result/     entity/    orkestrasyon  + saf SQL migration  tool/    composition DevExpress
   event       invariant                  + provider        stream   root
```

| # | Kural | İhlalde |
|---|---|---|
| 1 | Bağımlılık **yalnız içe** doğru akar | Mimari RED |
| 2 | UI asla doğrudan SQL çağırmaz | Mimari RED |
| 3 | `Protocol` Host↔UI arasındaki **tek** geçiş yüzeyidir | Mimari RED |
| 4 | Tek uygulama: **`VersaCoder.Desktop` + `VersaCoder.Cli`** | Kapsam ihlali |

### §2.6 Form üçlüsü (WinForms kalıbı)

```
<Ekran>.Form.cs            -> olay yonlendiricileri + Application katmanina cagri
<Ekran>.Form.Designer.cs   -> tasarim (ui-designer sahibi, elle oynanmaz)
<Ekran>.Form.resx          -> kaynaklar / lokalizasyon (designer uzerinden uretilir)
```

| # | Kural | Yasak |
|---|---|---|
| 1 | Üç dosya **her zaman birlikte** var olur | `.Designer.cs`'siz form |
| 2 | `.Designer.cs` / `.resx` son sözü **ui-designer**'dadır | developer'ın elle oynaması |
| 3 | `ResourceManager` adları **bozulmaz** | Ad değişimi → form açılmaz |
| 4 | Code-behind iş mantığı taşımaz | 200+ satır `.Form.cs` → `Application` katmanına |

### §2.7 7 ekran (PART 4 hedefi)

| # | Dosya adı | Aday DevExpress kontrolleri |
|---|---|---|
| 01 | `01-start-page` | RibbonControl, XtraTabControl, TileControl |
| 02 | `02-main-ide` | RibbonForm, RibbonControl (Office2010), DockManager, StatusBarControl |
| 03 | `03-solution-explorer` | TreeList, DxCollapsedControl, ContextMenuStrip |
| 04 | `04-code-editor` | MemoEdit / RichEditControl, XtraTabControl, GaugeControl |
| 05 | `05-ai-chat` | MemoEdit, ListBoxControl, ProgressBarControl, BarEditItem |
| 06 | `06-agent-panel` | GridControl, ProgressPanel, BarButtonItem |
| 07 | `07-settings` | DataLayoutControl, PropertyGridControl, LookUpEdit |

> **UYARI (R1):** Yedek vaultta **16** wireframe (`WF-00..WF-15`), ai.01'de **8** ana ekran var. **Esas olan 7 listedir**; 16 WF yalnız *bilgi* kaynağıdır ve **PART 4'te kullanıcıya sunulur.**

### §2.8 DevExpress kuralı (tek satırda)

> **KURAL:** ** DevExpress Universal 2026 / v26.1 ** · ana pencere **`RibbonForm`** · ribbon stili **yalnız `RibbonControlStyle.Office2010`** · form = `.Form.cs` + `.Designer.cs` + `.resx`. Başka sürüm/stil **CRITICAL** ihlaldir.

### §2.9 Sabit sayılar (ezber)

| Sayı | Anlam |
|---|---|
| **8** | Toplam part |
| **12** | Guardrail (G1..G12) |
| **7** | Ekran + karar prosedürü adımı + anti-pattern alt kümesi |
| **5** | Kapalı senior karar · kayıtlı ajan · Ağırlık 40/30/20/10 toplamı 100 |
| **6** | Risk (R1–R6) + gate (GATE-000..005) |
| **4** | Belirsizlik (B1–B4) + varsayım (A1–A3) + devre paketi maddesi |
| **3** | Mutlak dur-sor eşiği · başarısız düzeltme hakkı |
| **30–88 KB** | Kök MD boyut bandı |
| **14** | Kök belge sayısı |
| **~118** | `glossary.md` terim sayısı |

---

## §3 Güncel Durum (Anlık Görüntü)

> **Bu bölüm günceldir; §2 güncellenmez.** Her oturum sonunda yeniden yazılır. Tarih: **2026-09-25**.

### §3.1 Part ve gate durumu

| Part | Konu | Durum | Gate |
|---|---|---|---|
| **1/8** | Kaynak okuma (ai.01/02/03, opencode envanteri) | ✅ **raporlandı** | **GATE-001 ✓** |
| **2/8** | Kapsam analizi → `reports/00-kapsam-analizi.md` | ✅ **raporlandı** | **GATE-002 ✓** (Requirements Freeze) |
| 3/8 | `.ai` Vault kurulumu | 🔄 dosya üretim aşamasında | GATE-004 |
| 4/8 | UI + ASCII wireframe (7 ekran) | ⏳ başlamadı | **GATE-003 — BEKLİYOR** |
| 5/8 | Proje iskeleti + ilk `dotnet build` | ⏳ | GATE-005 |
| 6/8 | Kodlama | ⏳ | — |
| 7/8 | opencode uyarlaması | ⏳ | — |
| 8/8 | Doğrulama + teslim raporu | ⏳ | Kapanış |

```
  (1/8) OK ----> (2/8) OK ----> (3/8) DEVAM ----> (4/8) BEKLIYOR ----> (5..8)
      |               |               |                 |
  GATE-001 OK     GATE-002 OK    GATE-004         GATE-003 = HARD STOP
                                                  kod yazmak YASAK
```

> **UYARI:** **GATE-003 (UI Approval) kapalı olduğu sürece `.cs` kodu yazılmaz.** Bu, bu oturumun en kritik tek gerçeğidir. İhlali = G12 RED, part baştan.

### §3.2 Git durumu

| Alan | Değer |
|---|---|
| Repo | `C:\versacoder` — `git` repo, **evet** |
| Dal | `main` |
| **Commit sayısı** | **0** (henüz hiç commit yok) |
| Değişiklik | `?? .ai/` — tek untracked girdi |
| Sıradaki commit | Vault PART 3 tamamlanınca ilk commit (kullanıcı onayıyla) |

### §3.3 Vault envanteri (gözlem — her oturumda yeniden sayılır)

| Klasör | İçerik | Durum |
|---|---|---|
| `.ai/` (kök) | `index.md` `keys.md` `CLAUDE.md` `AGENTS.md` `WORKFLOW.md` `ROLE.md` `brain.md` `glossary.md` `MEMORY.md` `VISION.md` `PROJECTS.md` `log.md` `engine.md` `ULTRA-THINKING.md` | 14 hedef |
| `.ai/.templates/` | 26 MD (adr, agents, architecture, backend, documentation, frontend, infrastructure, query) | ✅ hazır (A3) |
| `.ai/.agents/` | `AGENTS.md` + 5 rol tanımı | ✅ hazır |
| `.ai/reports/` | `00-kapsam-analizi.md` | ✅ |
| `.ai/.rules/` | coding-standards · devexpress-rules · security-rules · solid-rules | kısmi (`data-access`, `testing`, `naming` bekleniyor) |
| `.ai/.decisions/` | `adr-index.md` + `accepted/ADR-001..002` | kısmi |
| `.ai/architecture/` | `index.md` + `00-overview/` | kısmi (l0..l7 bekleniyor) |
| `.ai/spec/` · `.ai/ui-design/` · `.ai/scripts/` · `.ai/.sql/` | şartname · wireframe · denetim · SQL | kısmi |

> **UYARI:** Bu tablo **gözlemdir**, SSOT değildir. Kesin sayı için her oturum başında dosya listesi yeniden sayılır (§6 adım 4).

### §3.4 Açık kalemler (bu oturumda kapatılacak değil, izlenecek)

| ID | Açık madde | Karar zamanı |
|---|---|---|
| R1 | 7 ekran ≠ 16 WF | PART 4, GATE-003 öncesi |
| R5 | net48 + DevExpress v26.1 ilk `dotnet build` | PART 5 başlangıcı |
| B1 | `.slnx` vs `.sln` | PART 5 |
| B2 | Test framework'ü | PART 5 |
| B3 | 7 ekranın kesin kontrol seti | PART 4 |
| B4 | LLM provider öncelik sırası | PART 7 |
| A3 | `.templates/` 24 dosya çakışmasızlığı | vault üretim başında doğrula |

### §3.5 Ölçüm anlık görüntüsü (2026-09-25)

> Sayaçlar **gözlemdir**; her oturum sonunda yenilenir. Sayım `scripts/measure-vault.md` usulüyle yapılır.

| Ölçüm | Değer |
|---|---|
| Kök `.md` sayısı (hedef) | 14 |
| Kök `.md` boyut bandı | 30–88 KB (±%10) |
| `.templates/` MD sayısı | 26 (7–51 KB) |
| `.agents/` MD sayısı | 6 (kayıt + 5 rol) |
| `reports/` | `00-kapsam-analizi.md` (PART 2 kanıtı) |
| `.rules/` | 4 dosya yazılı, `data-access` · `testing` · `naming` bekleniyor |
| `.decisions/` | `adr-index.md` + `accepted/ADR-001`, `ADR-002` |
| `architecture/` | `index.md` + `00-overview/` (2 dosya) |
| `.sql/` | `0001_initial_schema.sql`, `README.md`, `queries/session-list.sql` |
| Vault kökü | `C:\versacoder\.ai` (A1) |
| Ölçüm komutu | `(Get-Item '<yol>').Length` |

### §3.6 Bu oturumun KAPSAM DIŞI kalanları

| # | Yapılmayan | Neden | Ne zaman |
|---|---|---|---|
| 1 | `.cs` kodu yazımı | **GATE-003 kapalı** | PART 6 (GATE-003 sonrası) |
| 2 | Çözüm/proje dosyası (`slnx`/`sln`) | B1 açık | PART 5 |
| 3 | Test frameworkü seçimi | B2 açık | PART 5 |
| 4 | 7 ekran wireframe kesinleşmesi | B3 + R1 açık | PART 4 |
| 5 | LLM provider sırası | B4 açık | PART 7 |
| 6 | `git commit` | Kullanıcı onayı yok (G7) | Vault tamamlanınca |
| 7 | CoreMusic / yedek vault'a yazma | G1 yasağı | **Asla** |

---

## §4 Bir Daha Yapma ("Never Again") Listesi

> Her satır **gerçek bir hatadır**; nedeni yazılıdır. Nedeni bilinmeyen yasak, bir sonraki oturumda sessizce ihlal edilir.

| # | Yapma | Neden (gerçek olay) | Sonuç | Karşılığı |
|---|---|---|---|---|
| 1 | **CR-only satır sonu üretilmesi / CR-only dosyanın ham işlenmesi** | `ai.03.md` **CR-only** biçiminde geldi → parse ve satır sayımı bozuluyor (R2) | Bozuk girdi envanteri, yanlış satır sayıları | Okuma anında `\r` → `\n`; **asla** diske CR-only yazma |
| 2 | **Eski yığını referans almak: `.NET 8` · `EF Core 9` · `DevExpress 24.2.3`** | `ai.01.md` BÖLÜM 28 bunları yazıyordu; `ai.03.md` geçersiz kıldı | Yanlış paket kurulumu, G2/G3/G6/G8 ihlali | Yığın yalnız `ai.03` + `CLAUDE.md` §4 |
| 3 | **CoreMusic vault'undan içerik kopyalamak** | G1/SSUT — içerik sıfırdan yazılır | Ürünün sesi bozulur, çelişen kural | Yalnız **yapı** referansı |
| 4 | **Yedek vault kök MD boyutlarını çıta almak** | `.ai.04` kök MD'leri 1–16 KB → kalite çıtası düşük | Silik, işlevsiz vault | Ölçek coremusic ölçümü: 30–88 KB |
| 5 | **Kanıtsız "bitti" demek** | G5 — build/test çıktısı olmadan onay | Sonraki part'te patlayan hata | Yalnız `dotnet build` / `dotnet test` çıktısı |
| 6 | **GATE-003'ten önce kod yazmak** | UI onayı alınmadan form/.cs üretmek | Revizyon = kod çöpü, part baştan | HARD STOP — §3.1 |
| 7 | **16 wireframe'i 7 sanmak** | Yedek vaultta `WF-00..WF-15` var (R1) | Kapsam şişer, onay kirlenir | 7 ekran prompt'tan, 16 WF bilgi |
| 8 | **MVVM zorunlu sanmak** | ADR-002 (yedek) MVVM zorluyordu; prompt zorunlu kılmıyor | Gereksiz katman, WinForms'a aykırı | WinForms + DI; MVVM yalnız gerekirse |
| 9 | **Kapalı 5 kararı yeniden tartışmak** | D1..D5 `brain.md` §4'te gerekçesiyle kapandı | Boşa zaman, karar gevşemesi | Yeni kısıt gelmeden ADR **açılmaz** |
| 10 | **Belirsizliği sessizce cevaplamak** | B1..B4'ün cevabı henüz yok | Yanlış kilit, geri alma maliyeti | `UNKNOWN` / `NEEDS_VALIDATION` etiketi |
| 11 | **Aynı hatanın 4. düzeltme denemesi** | 3 başarısızlık = varsayım yanlış demektir | Sonsuz döngü, bağlam çöpü | `3-STRIKE` + şüpheli varsayımı adlandır |
| 12 | **`.resx`'i elle düzenlemek** | `ResourceManager` adları bozulur → form açılmaz | Tasarım kırığı | Yalnız designer; acilse `ui-designer` |
| 13 | **Bağlamı oturum belleğine gömmek** | Bulunan bilgi o an yola (rapor/`log.md`) yazılır | SSOT ihlali, sonraki oturum bulamaz | §7 kaydetme protokolü |
| 14 | **Bu dosyaya sır/şifre koymak** | Bellek paylaşılan metindir | Güvenlik olayı | §8 |

---

## §5 Anahtar Dosya Haritası

### §5.1 Kök belgeler

| Dosya | Tek sorumluluk | Bana ne zaman lazım |
|---|---|---|
| [index.md](index.md) | Navigasyon — vault'un genel haritası | **İlk okuma** |
| [keys.md](keys.md) | Anahtar kelime → hedef dosya yönlendirme | Bir terim/yol ararken |
| [CLAUDE.md](CLAUDE.md) | Anayasa: G1..G12 + zero-hallucination + hiyerarşi | Kural gerekiyorsa |
| [WORKFLOW.md](WORKFLOW.md) | Süreç, part sırası, gate listesi | Sıra/ne sonra sorusunda |
| [ROLE.md](ROLE.md) | Mutlak rol + öncelik çatışması | Kimin sözü geçer sorusunda |
| [AGENTS.md](AGENTS.md) | Ajan protokolü: devre, subagent, bütçe, escalation, DoD | Ajan işi/teslim paketi |
| [brain.md](brain.md) | Karar prosedürü, kriterler, 5 kapalı karar, R/B/A işleme | **Neden böyle** sorusunda |
| [glossary.md](glossary.md) | TR/EN terim sözlüğü (118 terim) | Terim tanımı gerekiyorsa |
| [MEMORY.md](MEMORY.md) | **Bu dosya** — durum + sabitler + protokol | **Her oturum başında** |
| [log.md](log.md) | Tarihli değişiklik + kalite raporu günlüğü | Son durum geçmişi |
| [VISION.md](VISION.md) | Vizyon, problem–çözüm, 7 ekran bağlamı | Ekran/kapsam konuşurken |
| [PROJECTS.md](PROJECTS.md) | Kapsam, hedefler, bileşen envanteri | Sınır/ne yapmayız |
| [engine.md](engine.md) | Gate ↔ part orkestrasyon matrisi | Sıradaki kapı hangisi |
| [ULTRA-THINKING.md](ULTRA-THINKING.md) | Derin düşünme protokolü | Karmaşık karar öncesi |

### §5.2 Alt klasörler

| Yol | İçerik | Sahip |
|---|---|---|
| `.ai/.templates/**` | 26 üretim kalıbı (adr, agents, architecture, backend, documentation, frontend, infrastructure, query) | — |
| `.ai/.agents/**` | 5 rol tanımı + ajan kaydı | architect (X) |
| `.ai/.rules/**` | Hard kurallar (coding, devexpress, security, solid, …) | architect (X) |
| `.ai/.decisions/**` | ADR defteri: `adr-index.md` + `accepted/` | architect (X) |
| `.ai/architecture/**` | Katman rehberleri + ASCII diyagramlar | architect (X) |
| `.ai/spec/**` | Şartname (protocol, session, tool, test, build) | architect (X) |
| `.ai/ui-design/**` | 7 ekran wireframe | **ui-designer (X)** |
| `.ai/reports/**` | Part raporları + teslim kanıtları | reviewer (W) |
| `.ai/scripts/**` | Link taraması, ölçüme | — |
| `.ai/.sql/**` | Migration + parameterized sorgu örnekleri | developer |
| `.ai/.obsidian/**` | Obsidian ayarları | — |

### §5.3 "Bilgi nerede" hızlı tablosu

| Bilgi | Gidilecek dosya |
|---|---|
| Bu part bitti mi, sıradaki gate ne | `engine.md` → `WORKFLOW.md` |
| Bu kural neden var | `CLAUDE.md` (metin) → `brain.md` (gerekçe) |
| Bu karar neden alındı | `brain.md` §4 → `.decisions/accepted/` |
| Bu terim ne demek | `glossary.md` |
| Bu kelime hangi dosyada | `keys.md` |
| Son ne değişti | `log.md` |
| Ekran nasıl görünecek | `ui-design/` |
| Ajan kime devrediyor, paket biçimi | `AGENTS.md` §4 |
| Risk/belirsizlik açık mı | `reports/00-kapsam-analizi.md` §3 + bu dosya §3.4 |

### §5.4 Sık iş → ilk adım (hızlı kalkış)

| Yapılacak iş | İlk adım (tek dosya) | Kalıp |
|---|---|---|
| Yeni kural yazacağım | `CLAUDE.md`'de karşılığı var mı → yoksa ekleme | `.rules/*.md` |
| ADR açacağım | `.decisions/adr-index.md` → şablon seç | `.templates/adr/adr-nygard-template.md` |
| Katman rehberi yazacağım | `architecture/index.md` sırası | `.templates/architecture/layer-spec-template.md` |
| ASCII diyagram çizeceğim | Saf ASCII, Unicode kutu **yok** | `.templates/architecture/ascii-diagram-template.md` |
| Form/Wireframe üreteceğim | 7 ekran listesi (§2.7) → kontrol adayları | `.templates/frontend/form-template.md` |
| Dapper sorgusu yazacağım | Provider abstraction (§2.3) | `.templates/infrastructure/dapper-mapping-template.md` |
| Migration yazacağım | Saf SQL + `schema_version` | `.templates/infrastructure/migration-template.md` |
| Ham sorgu örnekleyeceğim | Parameterized, `CommandTimeout` | `.templates/query/Query-Template.md` |
| Servis/handler yazacağım | Katman sırası (§2.5) | `.templates/backend/service-template.md` |
| Repository yazacağım | EF **yok**, ADO.NET + Dapper | `.templates/backend/repository-template.md` |
| Oturum sonu notu alacağım | Bu dosya §7 şablonu | `.templates/session-log-template.md` |
| Vault'u ölçeceğim | Boyut + link taraması | `scripts/measure-vault.md` · `scripts/check-links.md` |
| Ajan işi devredeceğim | 4 maddelik paket | `AGENTS.md` §4 |

---

## §6 Oturum Başlangıç Protokolü (Checklist)

> Her adım **tek, sınırlı** bir işlemdir. Sıra **zorunludur**; atlanan adım SSOT ve bağlam hatası üretir. Toplam **≈9 dk**.

1. **`index.md` oku** — vault haritası + §5 okuma sırası. *(≈2 dk, tek dosya)*
2. **`keys.md` §3 tara** — zorlu anahtarlar; işin hangi dosyaya gideceğini belirle. *(≈1 dk, tek dosya)*
3. **`MEMORY.md` §3 oku** — bu dosya: part/gate/git durumu + açık kalemler. *(≈2 dk, tek dosya)*
4. **Vault envanterini yeniden say** — kök `.md` sayısını ve `git status --porcelain` çıktısını doğrula; §3.3 ile fark varsa §3'ü güncelle. *(≈1 dk, tek komut)*
5. **`log.md` son 10 satırı oku** — en son ne kaydedildi, açık iş var mı. *(≈1 dk, tek dosya, sona doğru)*
6. **Sıradaki gate'i teyit et** — `engine.md`/`WORKFLOW.md`: bu işe izin var mı? **GATE-003 kapalıysa kod yazma.** *(≈1 dk)*
7. **Konuya göre derinleş** — `keys.md` yönlendirmesiyle ilgili dosyayı aç (kural → `CLAUDE.md`, karar → `brain.md`, terim → `glossary.md`). *(≈1 dk)*
8. **İşe başla** — tek cümlelik hedef + tahmini süre yaz, sonra uygula. *(—)*

```
1 index.md --> 2 keys.md --> 3 MEMORY.md --> 4 envanter + git sayimi
                                                      |
                                                      v
8 ISE BASLA  <-- 7 konu dosyasi  <-- 6 gate teyidi  <-- 5 log.md son satirlar
```

> **UYARI:** 4. adım atlanırsa §3'teki durum **bayat** kalır ve yanlış part'ta çalışılır. 6. adım atlanırsa GATE-003 ihlali riski doğar.

### §6.1 Bağlam bütçesi (oturum başına)

| # | Limit | Aşılırsa |
|---|---|---|
| 1 | Tek turda okunan dosya ≤ **8** | Özetle, yola devret |
| 2 | Cevapta madde ≤ **5** | Tabloya böl |
| 3 | Dokunulan dosya ≤ **3** | Sırayla parçala |
| 4 | Aktif subagent ≤ **2** | 3.'yü kuyruğa al |
| 5 | Uzun dosyada `grep` + `offset/limit` | Tam yükleme yasak |

---

## §7 Oturum Sonu Kayıt Protokolü

> Bellek **yazılarak** değil, **yola yazarak** yaşar. Amaç: bilgiyi `MEMORY.md`'ye değil, **kendi sahibi dosyaya** koymak.

| # | Adım | Nereye | Sınırlı iş |
|---|---|---|---|
| 1 | **Ne değişti** → tarihli tek blok ekle | `log.md` (**append**, üstünü yazma) | ≤10 satır · tarih + dosya + neden |
| 2 | **Kanıt** → build/test/link çıktısı | ilgili `reports/NN-*.md` | sayılarla, "geçti" değil |
| 3 | **Durum** → part/gate/git/gözlem | `MEMORY.md` §3 (**bu bölüm güncellenir, §2 güncellenmez**) | tablo, ≤20 satır |
| 4 | **Yeni kural** → yalnız gerçek kural ise | `CLAUDE.md` (gerekçe `brain.md`) | tekrar varsa ekleme |
| 5 | **Yeni risk/belirsizlik** → R7../B5../A4.. | `reports/00-kapsam-analizi.md` + `MEMORY.md` §3.4 | ID + sahip + karar zamanı |
| 6 | **Yeni terim** → `glossary.md` | harf bölümüne 1 satır | tek satır, 6 alan |
| 7 | **Bozuk link yok** → tarama | `scripts/check-links.md` usulü | sonuç = **0 bozuk link** |
| 8 | **Yıkıcı iş yoksa** commit öner | git | onay iste (G7) |

**Kayıt şablonu (`log.md` append):**

```markdown
## YYYY-AA-GG — <oturum/konu>   (part n/8, GATE-xxx)
- DEGISIKLIK : <dosya yolu> -> <ne oldu>  (max 5 madde)
- KANIT      : <build/test/link sayisi>
- KAPANDI    : <R/B/A kapandi mi>
- ACIK KALDI : <ID + neden + karar zamani>
- SIRADAKI   : <tek somut adim>
```

> **UYARI:** `MEMORY.md` **§2 (sabitler) oturum sonunda değiştirilmez.** Değişen tek şey §3 (anlık görüntü) ve §4'e yeni madde eklenmesidir. Sabitler değiştiyse bu bir **karardır** → önce `brain.md` ADR, sonra §2.

### §7.1 Oturum sonu self-check

- [ ] `log.md`'ye tarihli blok **eklendi** (üzerine yazılma değil)
- [ ] `MEMORY.md` §3 güncellendi; §2'ye dokunulmadı
- [ ] Kanıt sayıları yazılı ("başarılı" değil)
- [ ] Yeni madde ID'leri (R/B/A) varsa numaralandı
- [ ] Boş/sahte bilgi yok: `UNKNOWN`/`NEEDS_VALIDATION` işaretli
- [ ] Sıradaki adım **tek somut madde**, ≤2 dk'da başlanabilir

### §7.2 Dolu örnek (geçerli oturum sonu kaydı)

```markdown
## 2026-09-25 — vault kurulumu (part 3/8, GATE-004 hazirlik)

- DEGISIKLIK : .ai/glossary.md (118 terim)      -> yeni yazildi
               .ai/brain.md    (7 adimli karar)  -> yeni yazildi
               .ai/MEMORY.md   (bu dosya)        -> yeni yazildi
- KANIT      : (Get-Item glossary.md).Length = 47851 B  (band 30-48 KB OK)
               (Get-Item brain.md).Length    = 38710 B  (band 30-50 KB OK)
               link taramasi                 -> 0 bozuk link
- KAPANDI    : -
- ACIK KALDI : R1 (7 vs 16 ekran) -> PART 4 onayi
               R5 (net48+v26.1)   -> PART 5 ilk build
               B1..B4             -> PART 5 / 4 / 7
- SIRADAKI   : reports/03-vault-kurulum-raporu.md yaz (tek somut adim, ~10 dk)
```

**İlerleme satırı biçimi:** `(n/8)` — örn. `(3/8)`. Tahmin somut verilir (`≈10 dk`), "biraz" yasaktır.

---

## §8 Ne Saklanmaz (Güvenlik)

> **UYARI:** Bu dosya ve vault geneli **metindir**; bir sır kasası **değildir**. Yanlış içerik, tüm vault'un riskli hâle gelmesi demektir.

| # | Saklanmaz | Nerede tutulur | İhlalde |
|---|---|---|---|
| 1 | API key · token · parola · credential | OS keychain / `.env` (gitignore) / secret store | CRITICAL |
| 2 | **Kullanıcılı connection string** (`User Id=…;Password=…`) | Yapılandırma deposu, **şifreli**; vault'ta yalnız **şema** | CRITICAL |
| 3 | Lisans anahtarı · sertifika · private key | Güvenli kasa | CRITICAL |
| 4 | Kişisel veri (ad, e-posta, telefon) gerçek kişilerden | — | CRITICAL |
| 5 | Müşteri / üçüncü parti gizli veri | — | CRITICAL |
| 6 | `.env` dosyasının kendisi | gitignore'da | HIGH |
| 7 | Ham transcript'ün tamamı (şişkinlik + sır riski) | Oturum kaydı; vault'a yalnız **özet** | MEDIUM |

**Bağlantı dizesi kuralı:**

```sql
-- YASAK  : "Server=.;Database=X;User Id=sa;Password=123"   (metin içinde)
-- IZINLI : <ConnectionString> yapilandirmada saklanir; vault'ta yalnizca
--          sahip/tablo/parametre adlari gecer
```

| # | Kural | Neden |
|---|---|---|
| 1 | Vault'ta **örnek** connection string bile şifresiz yazılır | Örnek sır taşır |
| 2 | Şifreli saklama **kod tarafında** (AES-256-GCM) olur, dokümanda değil | SSOT + güvenlik ayrımı |
| 3 | Sır gerektiğinde kod **atıf yapar**, içeriğini **yazmaz** | Taramada ele vermez |
| 4 | Sızan sır bulunduğunda: **değiştir + `log.md`'ye kaydet** (gizleme yok) | Şeffaflık |

### §8.1 Sır sızıntısı müdahale adımları (≤5 dk)

| # | Adım | Sınırlı iş |
|---|---|---|
| 1 | Sır nerede? Vault `.md` mi, kod mu, git geçmişi mi | Tek cümle teşhis |
| 2 | **Üretimi iptal et** (key/token/şifre) | Kasa/güvenlik arayüzü, 1 işlem |
| 3 | Vault'taki metni **sil** | İlgili satır, tek düzenleme |
| 4 | `log.md`'ye tarihli kayıt | Ne, nerede, ne zaman değiştirildi |
| 5 | Yeni sır **hiç** vault'a yazılmaz | §8 tablosuna geri dön |

> **UYARI:** Sızıntıyı **gizlemek** yasaktır (G11). Gizlenen sır, tekrar aynı yere yazılır.

---

## §9 Bellek Sağlığı Kuralları

| # | Kural | Neden |
|---|---|---|
| 1 | **§2 sabittir** yalnız ADR ile değişir | Rastgele güncellenen "sabit" = değildir |
| 2 | **§3 her oturum sonunda yeniden yazılır** | Bayat durum, yanlış part riski |
| 3 | **Tekrar eden bilgi silinir**, sahibi dosyaya link verilir | SSOT (G1) |
| 4 | Süresi dolan madde **silinmez, kapatılır** (✅/❌ + tarih) | Tarihçe |
| 5 | Bilgi **o an yola yazılır**, oturum belleğine değil | Sonraki oturum bulamaz |
| 6 | Dosya ≤ **42 KB** aşarsa → §3/§4 arşivlenir (`log.md`'ye) | Boyut bandı |
| 7 | Sır **hiç** girmez | §8 |
| 8 | "Bitti" iddiası bu dosyada **tutulmaz** | Kanıt `reports/` + `log.md` |

### §9.1 Silme / güncelleme kararı

```
YENI BILGI GELDI
   |
   +-- sabit proje gercegi mi?  -- EVET --> §2'ye EKLE (ID + kaynak)
   |
   +-- guncel durum mu?          -- EVET --> §3'u YENIDEN YAZ (eskisini sil)
   |
   +-- ayni hatanin tekrari mi?  -- EVET --> §4'e EKLE (nedeni ile)
   |
   +-- karar gerekcesi mi?       -- EVET --> brain.md (BURAYA DEGIL)
   |
   +-- kural/metin mi?           -- EVET --> CLAUDE.md (BURAYA DEGIL)
   |
   +-- gecmis kaydi mi?          -- EVET --> log.md (BURAYA DEGIL)
   |
   +-- sir/sifre mi?             -- EVET --> ASLA YAZMA (§8)
   |
   +-- hepsi HAYIR -------------> bu dosyaya GEREK YOK
```

---

## §10 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] §2 sabitler `CLAUDE.md` §4 tablosuyla **birebir** aynı (yığın, katman, uygulamalar)
- [ ] §3 durumu `(2/8) · GATE-001 ✓ · GATE-002 ✓ · GATE-003 bekliyor` + `0 commit · ?? .ai/`
- [ ] §4'te en az şu 4 madde var ve **nedeni** yazılı: CR-only (ai.03) · eski yığın .NET 8/EF Core 9/DevExpress 24.2.3 (ai.01) · kanıtsız "bitti" · GATE-003 öncesi kod
- [ ] §5 haritası her link hedefi diskte mevcut
- [ ] §6 başlangıç checklist'i **numaralı, her adım tek sınırlı iş**
- [ ] §7 sonu kaydı: `log.md` **append** protokolü
- [ ] §8'de secret/connection-string yasağı açık
- [ ] ASCII diyagramlar **saf ASCII** (Unicode kutu yok), CR-only satır sonu yok
- [ ] Boyut **30–42 KB** bandında
- [ ] Tek H1 · § numaralı bölüm · bozuk iç link yok

---

## §11 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Navigasyon | `.ai/index.md` |
| 2 | Anahtar kelime haritası | `.ai/keys.md` |
| 3 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |
| 4 | Süreç + gate | `.ai/WORKFLOW.md` |
| 5 | Orkestrasyon matrisi | `.ai/engine.md` |
| 6 | Kararlar + gerekçe | `.ai/brain.md` |
| 7 | Değişiklik günlüğü | `.ai/log.md` |
| 8 | Terimler sözlüğü | `.ai/glossary.md` |
| 9 | Ajan protokolü | `.ai/AGENTS.md` |
| 10 | Kapsam analizi (R/B/A) | `.ai/reports/00-kapsam-analizi.md` |
| 11 | Kök belge şablonu (boyut bandı) | `.ai/.templates/documentation/root-md-template.md` |
| 12 | Bootstrap (karar kaynağı) | `G:\…\versacoder proje planaması ai.03.md` |
