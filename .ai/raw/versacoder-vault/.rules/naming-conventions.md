---
title: "VersaCoder — İsimlendirme Kuralları"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "VersaCoder'da isimlendirme tek kaynağı — namespace, proje, sınıf/arayüz, async, dosya/klasör, WinForms Form eki, DTO, event, ADR/kural/rapor dosya adı, İngilizce slug ve Türkçe-İngilizce ayrımı"
---

# VersaCoder — İsimlendirme Kuralları (`.rules/naming-conventions.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [coding-standards.md](./coding-standards.md) · [devexpress-rules.md](./devexpress-rules.md) · [testing-rules.md](./testing-rules.md) · [../spec/versacoder-spec.md](../spec/versacoder-spec.md)

> **Tek cümlelik sorumluluk:** *Bir şeyin adı ne olmalı — kodda, dosyada, vault'ta; hangi dil, hangi ek, hangi kalıp* sorusunun tek cevabıdır.
> Bu dosya `keys.md` §3/§6 dosya adı haritası ile birlikte okunur; çelişirse bu dosya (isim kuralı) kazanır, `keys.md` hedefi güncellenir (G10).

---

## §1 Kapsam

**İçinde:** C# tanımlayıcıları, proje/klasör/dosya adları, vault `.md` dosya adları, test adları.

**Dışında:** dosya *içeriği* biçimi (`CLAUDE.md` G4), SQL tablo/kolon adları (`data-access-rules.md`), UI yerleşimi (`devexpress-rules.md`).

> **KURAL (özet):** **Kod, dosya, slug → İngilizce.** **Vault prose → Türkçe.** Tanımlayıcıda Türkçe karakter (ı ğ ş İ ö ç ü) **kullanılmaz**; Türkçe yalnız belge metninde yaşar.
> **YASAK:** Karışık dil ("OturumuYukle", "LoadOturum"), ek tutarsızlığı (`Manager`/`Helper` sığınıcıları), ad ile içerik uyuşmazlığı.

### §1.1 Dil Haritası (tek bakışta)

```
kaynak kod (.cs)        -> Ingilizce     (identifikator, yorum, XML doc)
proje / klasor / dosya  -> Ingilizce     (PascalCase klasor, dosya = tip adi)
vault MD (.rules/spec/..)-> slug Ingilizce (kebab-case) + govde Turkce
vault MD (kok + rapor)  -> mevcut adlar korunur (bkz. NC-15 istisna)
test adlari             -> Ingilizce     (Method_Scenario_Expected)
SQL tablo / kolon       -> Ingilizce     (PascalCase tablo, PascalCase kolon)
UI metinleri (resx)     -> Turkce        (kullanici gorur) + anahtar Ingilizce
hata mesaji (kullanici) -> Turkce        ; kod ici log etiketi Ingilizce
```

---

## §2 Kurallar (NC-01 .. NC-18)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| NC-01 | **Namespace = `VersaCoder.<Katman>[.<Özellik>]`** — `VersaCoder.Application.Sessions`, kök atlanmaz | Namespace proje adıyla başlamazsa kayıp tip araması ve çakışma riski | `namespace Application.Sessions;` / `namespace Versacoder...` (büyük/küçük tutarsız) | `VersaCoder.` kökü + katman |
| NC-02 | **Proje adları**: `VersaCoder.<Ad>` — `Abstractions`, `Domain`, `Application`, `Infrastructure`, `Protocol`, `Host`, `CrossCutting`, `Desktop`, `Cli`, `VersaCoder.Tests`; `src/` ve `tests/` altında | Proje adı = namespace kökü = paket kimliği; tek yazım | `VersaCoder.Infrastructure.csproj` ≠ `Versacoder.Infrastructure` | Proje adını NC-02 listesine göre düzelt (dosya dahil) |
| NC-03 | **Özellik klasörleri** altında feature adı İngilizce tekil: `Features/Sessions/`, `Features/Tools/`; namespace klasörle birebir | Klasör ≠ namespace = derlemede gizli bağımlılık | `Oturumlar/` klasörü | İngilizceye çevir; namespace'i hizala |
| NC-04 | **Sınıf adları isimdir, ek yalnız onaylı**: `Service`, `Handler`, `Repository`, `Provider`, `Factory`, `Options`, `Settings`, `Processor` (dar); `Manager`/`Helper`/`Utils`/`Core` **yasak** | Sığ ek = sorumluluk belirsizliği (SOL-01) | `CommonHelper.DoStuff()` | Ekli, amaca dönük ad + sorumluluğu sınırla |
| NC-05 | **Arayüz = `I` + adı**; implementasyon adı arayüzü **eşler** (`ISessionRepository` → `DapperSessionRepository`/`SqliteSessionRepository`) | `I` yoksa bağımlılık hattı testte kör kalır | `SessionRepo` interface; `SessionRepoImpl` belirsiz | `I` ekle; implementasyonu taşıyıcı adıyla ayır (hangi strateji? `Dapper*`) |
| NC-06 | **Async metot `Async` son eki** (`ExecuteAsync`, `FindAsync`); senkron `Async` **eklemez**; `Task`/`Task<T>` döner (`CS-06`) | Eksik ek `await` unutulmasına yol açar | `async Task Load()` / `LoadAsync()` senkron | Eki düzelt; çağrıda `await` |
| NC-07 | **Dosya = tip adı, tip başına tek dosya** (partial dâhil: `MainForm.cs` + `MainForm.Designer.cs`); `@`-escaped ad yok; alt sınıflar kendi dosyasında | Dosya araması tip aramadır | 3 sınıf tek `.cs` içinde | Böl; adı dosyaya taşı |
| NC-08 | **Klasör adları**: `src/` altında **PascalCase** (`Features/Sessions`), kökler `src/`, `tests/`, `db/migrations/`; dosya uzantısı küçültük `.cs`/`.md`/`.sql` | Karışık casing arama/bağlantı kırar | `features/sessions/` + `Features/` yan yana | Tek casing'e hizala (PascalCase klasör) |
| NC-09 | **WinForms: `Form` eki zorunlu** (`SessionListForm`, `SettingsForm`) + üçlü ad birebir aynı (`X.cs`/`X.Designer.cs`/`X.resx` — DX-05); ana pencere `MainForm` | Eksik ek/grid vs. form ayrımı kaybolur; üçlü adı kayarsa Designer açılmaz | `SessionList.cs` form; `.resx` adı farklı | `Form` eki + üçlüyü eşle (`devexpress-rules.md` DX-05) |
| NC-10 | **DTO ekleri**: katmanlar arası veri `*Dto`; handler sözleşmesi `*Request`/`*Response`; DB satırı `*Row` (Dapper); wire mesajı `*Message`/`*Envelope` | Aynı nesnenin 3 ismi olursa sözleşme dağılır | Servis `SessionDto`, repository `SessionEntity` aynı şey | Ek seçim tablosu: Dto/Request/Response/Row/Message |
| NC-11 | **Event'ler geçmiş zamanlı isim** (`SessionCompleted`, `StateChanged`); **handler** `On<Event>` ya da `XxxEventHandler`; delegate `XxxEventHandler`; `On...` üye adı olarak **olmaz** | .NET event kılavuzu ile uyum, otomatik tamamlama tutarlılığı | `EventSessionCompleted()` metodu; `OnCompleted` field'ı | Üye: `SessionCompleted`; handler: `OnSessionCompleted` |
| NC-12 | **Alan/sabit**: private alan `_camelCase`; `public/internal const` ve `static readonly` **PascalCase**; `static` alan `s_` **kullanılmaz** (tek stil) | Karışık stil görsel gürültü; yanlış yerde `static` gizlenir | `_MaxRetries`, `maxRetries` (const) | `_camelCase` / `MaxRetries` |
| NC-13 | **Boolean `is`/`has`/`can`/`should`**; koleksiyon **çoğul** (`Sessions`, `ToolCalls`); parametre/özellik `Get...` eki **olmaz** | `GetName()` property ile çakışır; tek başına `Data` ne olduğu belirsiz | `bool Delete()`; `List<Session> Data` | `CanDelete`; `Sessions` |
| NC-14 | **Hata/exception**: `*Exception` (`SessionNotFoundException`); uygulama hata kodu `VC-E-NNNN`; log etiketi `[Tag]` İngilizce, kullanıcı mesajı Türkçe (`security-rules.md` SEC-13) | Kod ile mesaj dili karışınca hem test hem lokalizasyon kırılır | `HataOlmusException` | NC-18 ayrımına dön |
| NC-15 | **Vault dosya adı**: `.rules/<konu>.md` (İngilizce kebab: `security-rules.md`), `spec/<konu>-spec.md` ya da `test-strategy.md`, `.decisions/ADR-NNN-<english-kebab>.md`, `reports/NN-<english-kebab>.md`, `ui-design/NN-<english-kebab>.md` — **istisna**: mevcut `reports/00-kapsam-analizi.md` (Türkçe slug) **kabul edilmiş tek istisnadır**, yenisi üretilmez | Arama/tarama scriptleri ad kalıbına güvenir; iki dilde slug = tutarsız arama | Yeni rapor `01-kapsam-raporu.md` | İngilizce kebab: `01-scope-report.md`; mevcut dosya **değiştirilmez** |
| NC-16 | **Test adları** (`testing-rules.md` TS-08): sınıf `<Sınıf>Tests`; metot `Method_Scenario_Expected`; dosya `<Sınıf>Tests.cs` | Test = okunur senaryo; `Test1` hiçbir şey anlatmaz | `FooWorks()` | Senaryoyu isme yaz |
| NC-17 | **Yorum & XML doc İngilizce**; `///` yalnız public yüzey; vault metni Türkçe (`CS-10`) | Kod İngilizce ise yorumun Türkçe'si iki dil arasında köprü kurar, aramayı böler | `// oturumu yukler` | `// Loads the session and its transcript.` |
| NC-18 | **Türkçe-İngilizce ayrımı (mutlak)**: tanımlayıcı/yorum/slug **İngilizce**; vault prose, açıklamalar, kullanıcıya görünen UI metni (resx) **Türkçe**; Türkçe karakter tanımlayıcıda yok | Karışım her dosyada tekrar eden kararı yeniden açar; encoding riski (`CS-01` benzeri) | `class OturumYukleyici`, `void IslemYap()` | İngilizce tanımlayıcı + Türkçe açıklama |

> **YASAK:** "Küçük istisna" diyerek NC-04 (`Manager`/`Helper`), NC-06 (`Async` eksik), NC-09 (`Form` eksik) kurallarını esnetmek. İstisna = `log.md` kaydı + inceleme onayı.

---

## §3 Karar Tabloları

### §3.1 NC-05 — Arayüz/Implementasyon Eşlemesi

| Strateji | Ad |
|---|---|
| Repository (Dapper) | `DapperSessionRepository : ISessionRepository` |
| Factory (provider) | `SqliteConnectionFactory : IDbConnectionFactory` |
| Client (LLM) | `OllamaProvider : IModelProvider` |
| Handler | `RunAgentTaskHandler : IRunAgentTaskHandler` |
| Clock seam | `TimeProvider` (framework tipi, `IRandom`/`IGuidFactory` elde) |

### §3.2 NC-10 — DTO Eki Seçimi

| Durum | Ek | Örnek |
|---|---|---|
| Katmanlar arası veri | `*Dto` | `SessionDto` |
| Komut/sorgu girdisi | `*Request` | `RunAgentTaskRequest` |
| Komut/sorgu çıktısı | `*Response` | `RunAgentTaskResponse` |
| DB satırı (Dapper) | `*Row` | `SessionRow` |
| Protokol çevrim içi | `*Envelope`/`*Message` | `ProtocolEnvelope` |
| Domain nesnesi | **ek yok** | `Session`, `SessionId` (value object) |

### §3.3 NC-13 — Ek Kalıpları

| Kalıp | Kullanım | Örnek |
|---|---|---|
| `Can*` / `Is*` / `Has*` | bool | `CanResume`, `IsRunning`, `HasTranscript` |
| `*Id` | value object / key | `SessionId`, `ToolCallId` |
| `*Options` | yapılandırma | `AgentOptions` |
| `*Factory` | nesne üretimi | `DbConnectionFactory` |
| `*Provider` | dış sağlayıcı | `ModelProvider` |
| `*Handler` | tek use-case | `CreateProjectHandler` |
| `*Service` | ≥2 adım orkestrasyon | `SessionService` |

### §3.4 NC-15 — Vault Dosya Adı Kalıpları

| Klasör | Kalıp | Örnek |
|---|---|---|
| `.rules/` | `<konu>-<tür>.md` ya da `<konu>.md` (kebab) | `data-access-rules.md`, `naming-conventions.md` |
| `spec/` | `<konu>-spec.md` / `<konu>-strategy.md` | `protocol-spec.md`, `test-strategy.md` |
| `.decisions/` | `ADR-NNN-<english-kebab>.md` | `ADR-001-dual-tfm.md` |
| `reports/` | `NN-<english-kebab>.md` | `01-architecture-report.md` (istisna: `00-kapsam-analizi.md`) |
| `ui-design/` | `NN-<english-kebab>.md` | `01-start-page.md` |
| `architecture/lN-*/` | `<english-kebab>.md` | `dependency-map.md` |

---

## §4 Örnek: Doğru -> Yanlış

```
dogru                                    yanlis
--------------------------------------   ---------------------------------
namespace VersaCoder.Application.Sessions;   namespace Application.Sessions;
class SessionService                     class OturumServisi
interface ISessionRepository             interface SessionRepository
Task LoadAsync(CancellationToken ct)     async void Load() / Task Load()
SessionListForm.cs + .Designer.cs + .resx   SessionList.cs (eksik "Form")
public const int MaxRetries = 3;         public const int _maxRetries = 3;
class SessionNotFoundException           class HataException
reports/01-architecture-report.md        reports/01-mimari-raporu.md  (yeni dosya)
// Loads the session transcript.        // oturumu yukler
```

---

## §5 Review Checklist (incelemede işaretlenir)

- [ ] NC-01: tüm namespace'ler `VersaCoder.<Katman>...`
- [ ] NC-02: proje adları listeyle birebir; `src/`+`tests/` düzeni
- [ ] NC-03: Türkçe klasör/feature adı = 0
- [ ] NC-04: `Manager`/`Helper`/`Utils` = 0
- [ ] NC-05: her `interface` `I` + eşleşen implementasyon adı
- [ ] NC-06: her `async` metot `Async` eki + `Task` dönüşlü
- [ ] NC-07: tip ↔ dosya 1:1
- [ ] NC-08: klasör casing tek stil
- [ ] NC-09: her form `Form` eki + tam üçlü
- [ ] NC-10: DTO eki tutarlı (tablo §3.2)
- [ ] NC-11: event geçmiş zaman, handler `On...`
- [ ] NC-12: `_camelCase` alan / PascalCase sabit
- [ ] NC-13: bool `is/has/can`, koleksiyon çoğul
- [ ] NC-14: `*Exception` + `VC-E-NNNN` kodu
- [ ] NC-15: yeni vault dosyası İngilizce kebab (istisna yok)
- [ ] NC-16: test adları `Method_Scenario_Expected`
- [ ] NC-17: kod içi yorum İngilizce
- [ ] NC-18: tanımlayıcıda Türkçe karakter = 0

---

## §6 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme |
|---|---|---|---|---|
| 1 | `class OturumServisi` | NC-04/18 | arama kırılır, tutarsız | `SessionService` |
| 2 | `Task Load()` (`Async` eksik) | NC-06 | `await` unutulur | `LoadAsync` |
| 3 | `interface IRepo` tek impl | NC-05 | soyutlama anlamsız | adlandır + gerekliliği gözden geçir (SOL-08) |
| 4 | Form adında `Form` yok | NC-09 | üçlü uyuşmaz, Designer açılmaz | `XxxForm` |
| 5 | `Helper` içinde 300 satır | NC-04 | god-class | amaca dönük sınıf |
| 6 | Yeni rapor Türkçe slug | NC-15 | arama tutarsız | `NN-english-kebab.md` |
| 7 | Dosyada 3 üst düzey tip | NC-07 | bulunamayan tip | dosyalara böl |
| 8 | `_MaxRetries` / `maxRetries` | NC-12 | stil gürültüsü | `_camelCase` / `MaxRetries` |
| 9 | Yorumda Türkçe, kodda İngilizce karışık | NC-17/18 | encoding + arama sorunu | ayrımı uygula |
| 10 | UI metninde resx anahtarı Türkçe | NC-18 | anahtar kırılır | anahtar İngilizce, değer Türkçe |

---

## §7 Doğrulama

- [ ] 18 kural `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] NC-15 istisnası (`00-kapsam-analizi.md`) açık ve tek
- [ ] Dil haritası §1.1 ASCII, kutu çizimi yok
- [ ] Bağlantılar yalnız mevcut dosyalara (sibling `.rules/*.md`, `../index.md`, `../CLAUDE.md`, `../keys.md`, `../reports/00-kapsam-analizi.md`, `../spec/versacoder-spec.md`)
- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`

---

## §8 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa + dosya biçimi (G4) | `../CLAUDE.md` |
| 2 | Anahtar kelime/dosya haritası | `../keys.md` |
| 3 | Kod içinde yorum kuralı | `coding-standards.md` |
| 4 | Form üçlüsü (DX-05/DX-09) | `devexpress-rules.md` |
| 5 | Test adlandırması (TS-08) | `testing-rules.md` |
| 6 | Gereksinim ID'leri (`VC-XXX-NNN`) | `../spec/versacoder-spec.md` |
