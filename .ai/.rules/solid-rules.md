---
title: "VersaCoder — SOLID Kuralları"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder'a özgü SOLID kuralları — her ilke için kısa C# do/don't örneği, katman bağımlılık yönü ve NetArchTest ile otomatik denetim"
---

# VersaCoder — SOLID Kuralları (`.rules/solid-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [coding-standards.md](./coding-standards.md) · [data-access-rules.md](./data-access-rules.md) · [devexpress-rules.md](./devexpress-rules.md) · [testing-rules.md](./testing-rules.md)

> **Tek cümlelik sorumluluk:** *SOLID'in her ilkesi VersaCoder kodunda neye karşılık gelir, ihlal nasıl görünür, nasıl düzeltilir* sorusunun tek cevabıdır.
> Bu dosya `CLAUDE.md` **G8 (yığın sapması yok)** ve `keys.md` #14 (SOLID) karşılığıdır; teknoloji seçiminde `CLAUDE.md` kazanır.

---

## §1 Kapsam

**İçinde:** 8 kural (SOL-01..SOL-08) — beş ilke + üç uygulama kuralı (denetim, hata akımı, YAGNI). Her kuralda VersaCoder'a özgü kısa `BAD`/`GOOD` örneği.

**Dışında:** katmanların fiziksel yapısı (`architecture/`), tipik kod yazımı (`coding-standards.md`), hangi dosyanın hangi katmanda olacağı (`.templates/backend/`).

> **KURAL:** SOLID, *soyutlama için soyutlama* değildir. Her `interface` bir **gerçek ikinci kullanım** ya da **test edilebilirlik** sebebi taşır (SOL-08).
> **YASAK:** Ilkeyi "daha sonra lazım olur" diye uygulamak; ilkeyi ihlal edip yorumla savunmak.

### §1.1 Katman Bağımlılık Yönü (tek yön)

```
 +-----------+   +---------+   +--------------+   +---------------+
 |    UI     |-->|  Host   |-->| Application  |-->|    Domain     |
 | (Desktop/ |   | (DI,    |   | (handler,    |   | (invariant,   |
 |   Cli)    |   |  comp.) |   |  servis)     |   |  entity)      |
 +-----+-----+   +----+----+   +------+------+   +-------+-------+
       |              |               |                  |
       | DevExpress    | orkestrasyon | I... arayuzleri   | sabitler
       | (yalnz UI)    |              v                  v
       |         +-----+--------+-----------------------------+
       |         | Infrastructure (Dapper, provider) -> Domain |
       |         +----------------------+----------------------+
       |                                |
       |         +----------------------v----------------------+
       |         | Protocol (mesaj, tool sozlesmesi)           |
       |         +---------------------------------------------+
       v
  Paylasimli crosscutting (log, zaman, sonuc) -> her katman kullanir
```

> **KURAL:** Ok yönü **dışarıdan içeri**. `Domain` hiçbir katmanı referans almaz; `Application` `Infrastructure`'ı referans almaz (arayüz `Abstractions`/`Application`'da, uygulaması `Infrastructure`'da, bağlama `Host`'ta).

---

## §2 Kurallar (SOL-01 .. SOL-08)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| SOL-01 | **SRP**: bir sınıfın **tek** sorumluluğu, **tek** değişim sebebi vardır — form ekran çizer, servis akış kurar, repository SQL çalıştırır | Aynı dosyada 3 sorumluluk = her değişiklik 3 riski birden tetikler | 800 satırlık `MainForm`; "repository + log + zaman" kodlayan servis | Sorumluluğu sınıfa böl; §3.1 örneği |
| SOL-02 | **OCP**: davranış **açık uzantı** ile değişir (strategy/registry); mevcut `switch` zincirine yeni dal eklemek yerine yeni implementasyon | Yeni LLM provider/tool eklemek mevcut kodu bozmamalı | `if (p=="openai") ... else if (p=="ollama")` zinciri yayılıyor | Arayüz + kayıt (Host DI) — §3.2 |
| SOL-03 | **LSP**: bir arayüzü uygulayan her tip, o arayüzün **tüm** geçerli kullanımını destekler; geçerli girdiyle `NotSupportedException` fırlatmak ihlaldir | Arayüz kullanan kod, implementasyonu bilmeden değişebilmeli | `SqlServerRepository` içinde `CreateTable` → `throw new NotSupportedException()` | Yetenekleri böl (SOL-04) ya da eksik davranışı tamamla/önceden doğrula |
| SOL-04 | **ISP**: "şişman" arayüz yerine küçük, amaca dönük arayüzler; istemci yalnız ihtiyacı olanı görür | Şişman arayüz = sahte implementasyon dolu | 12 metotlu `IToolHost`; her implementasyonda 10'u boş | Arayüzü yeteneğe göre böl — §3.4 |
| SOL-05 | **DIP**: yüksek seviye modül **arayüze** bağımlıdır; somut sınıfı **Host** bağlar | Somutu gömünce test edilemez ve provider değişimi kırılır | `Application` servisinde `new SqliteConnectionFactory()` | `ISessionRepository` enjekte et; kaydı Host'ta — §3.5 |
| SOL-06 | **Denetim otomatiktir**: katman bağımlılık kuralları **NetArchTest** ile test edilir; el incelemesi tek başına yeterli değildir | Kural yalnız incelemedeyse 2 haftada ölür | Ters referans (`.Application` → `.Infrastructure`) derlenmiş, test yeşil | Kuralı `tests/.../ArchitectureTests.cs`'e ekle; RED → taşı |
| SOL-07 | **Hata akımı `Result<T>` + tipi özel exception**: istisna akış kontrolü için kullanılmaz; boş catch yasak (`CS-05`) | Exception ile "bulunamadı" bildirmek akışı gizler, performansı düşürür | `try/catch(KeyNotFoundException)` ile normal akış | `Result<T>.Failure` / tipi özel exception; §3.6 |
| SOL-08 | **YAGNI/soyutlama disiplini**: `interface` + `abstract` + `strategy` yalnız **≥2 gerçek kullanım** ya da **test seam** varsa | Erken soyutlama = anlamsız katman, dolgu metotlar | Tek implementasyonlu `IFoo` + `Foo` + `FooFactory` zinciri | Sınıfla başla; ikinci kullanım gelince soyutla |

> **YASAK:** SOL-06 dahilinde **test edilmemiş** mimari kural iddiası ("bağımlılık yönü doğru" diyen ama NetArchTest'i olmayan inceleme onayı — G5 ihlali).

---

## §3 Kritik Kuralların Ayrıntısı (BAD -> GOOD)

### §3.1 SOL-01 — Tek Sorumluluk

```csharp
// BAD — form: ekran + SQL + log + is kurali bir arada (god form)
public partial class SessionListForm : RibbonForm
{
    private void btnRefresh_Click(object sender, EventArgs e)
    {
        var conn = new SqliteConnection("Data Source=sessions.db"); // DX-15 + DA-04 ihlali
        conn.Open();
        var list = conn.Query("SELECT * FROM Sessions").ToList();   // DA-03 ihlali
        if (list.Count == 0) MessageBox.Show("Kayit yok");          // is kurali burada
        gridControl1.DataSource = list;
    }
}

// GOOD — form yalnizca gosterir; akis handler'da, veri repository'de
public partial class SessionListForm : RibbonForm
{
    private readonly ILoadSessionsHandler _handler;
    public SessionListForm(ILoadSessionsHandler handler) => _handler = handler;

    private async void OnRefreshItemClick(object sender, ItemClickEventArgs e)
    {
        var result = await _handler.ExecuteAsync(new LoadSessionsRequest(), CancellationToken.None);
        gridControl1.DataSource = result.IsSuccess ? result.Value : Array.Empty<SessionRow>();
    }
}
```

### §3.2 SOL-02 — Açık Uzantı (LLM Provider)

```csharp
// BAD - her yeni provider icin mevcut kod degisir (switch buyur)
public string Complete(string provider, string prompt)
    => provider switch
    {
        "openai" => CallOpenAi(prompt),
        "ollama" => CallOllama(prompt),
        _ => throw new NotSupportedException(provider)   // SOL-03 riski
    };

// GOOD — arayuz + Host kaydi; yeni provider = yeni sinif + tek DI satiri
public interface IModelProvider
{
    string Name { get; }
    Task<string> CompleteAsync(PromptRequest request, CancellationToken cancellationToken);
}

// Host/ServiceCollectionExtensions.cs  (oradaki tek degisiklik)
services.AddSingleton<IModelProvider, OllamaProvider>();
services.AddSingleton<IModelProvider, OpenAiCompatibleProvider>();
```

### §3.3 SOL-03 — Yerine Geçebilirlik

```csharp
// BAD — arayuzun gecerli kullanimini reddeder
public sealed class SqliteMigrator : IMigrator
{
    public void Apply(Migration m) { /* sqlite icin tamam */ }

    public void Verify(Migration m)
        => throw new NotSupportedException("Sqlite icin desteklenmiyor"); // LSP ihlali
}

// GOOD — ya davranis tamamlanir, ya yetenek arayuzu bolunur (SOL-04)
public interface IMigrator { void Apply(Migration m); }
public interface IMigrationVerifier { void Verify(Migration m); }  // yalnizca destekleyenler uygular
```

### §3.4 SOL-04 — İnce Arayüz

```csharp
// BAD — sisman arayuz; her arac her metodu uygulamak zorunda
public interface IAgentTool
{
    string Name { get; }
    Task<string> ReadAsync(string path, CancellationToken ct);
    Task WriteAsync(string path, string content, CancellationToken ct);
    Task<string> ShellAsync(string command, CancellationToken ct);
    Task<string> SearchAsync(string pattern, CancellationToken ct);
}

// GOOD — tek yetenek: calistir; arac kendi parametre dogrulamasini yapar
public interface ITool
{
    string Name { get; }
    ToolSchema Schema { get; }
    Task<ToolResult> ExecuteAsync(ToolCall call, ToolContext context, CancellationToken ct);
}
```

### §3.5 SOL-05 — Bağımlılık Tersine Çevirme

```csharp
// BAD — Application, somutu bilir (test edilemez, provider degisimi kirar)
public sealed class SessionService : ISessionService
{
    private readonly SqliteConnectionFactory _factory = new("Data Source=sessions.db");
}

// GOOD — arayuz Application'da, uygulamasi Infrastructure'da, baglama Host'ta
public interface ISessionRepository
{
    Task<Session?> FindAsync(SessionId id, CancellationToken cancellationToken);
}

public sealed class SessionService(ISessionRepository repository) : ISessionService { /* ... */ }

// Host'ta tek baglayici satir:
services.AddSingleton<ISessionRepository, DapperSessionRepository>();
```

### §3.6 SOL-07 — Hata Akımı

```csharp
// BAD — "bulunamadi" istisnasi olarak akis kontrolu
try { return repo.GetAsync(id).Result; }
catch (KeyNotFoundException) { return null; }        // + CS-03 ihlali (.Result)

// GOOD — basari/basarisizlik tipli ve async
Result<Session> found = await _repository.FindAsync(id, cancellationToken);
return found.IsSuccess ? found.Value : null;
```

---

## §4 Otomatik Denetim (SOL-06) — Kural -> Test

| # | Kural (NetArchTest benzeri iddia) | Beklenen |
|---|---|---|
| 1 | `Domain` → hiçbir `VersaCoder.*` referansı **almasın** | 0 |
| 2 | `Application` → `Infrastructure` referansı **almasın** | 0 |
| 3 | `Application`/`Domain` → `DevExpress.*` referansı **almasın** (DX-15) | 0 |
| 4 | `Infrastructure` → `Protocol`/`UI` referansı **almasın** | 0 |
| 5 | Adı `*Handler`/`*Service` olan tipler `interface` ile yaşasın (`I` prefix, NC-05) | istisnasız |
| 6 | `catch { }` içeren tip sayısı | 0 (CS-05) |

> **UYARI:** Bu testler `GATE-005` (Implementation Plan Approval) öncesi **çalışıyor** olmalı; "mimari doğru" iddiası bu tablo yeşil olmadan geçersizdir (G5).

---

## §5 Review Checklist (incelemede işaretlenir)

- [ ] SOL-01: yeni sınıfın tek sorumluluğu tek cümleyle yazılı
- [ ] SOL-02: yeni davranış için mevcut `switch`/`if` zinciri büyütülmedi
- [ ] SOL-03: yeni implementasyonda geçerli girdiye `NotSupportedException` yok
- [ ] SOL-04: ≥6 "boş/desteklenmiyor" metodu olan arayüz yok
- [ ] SOL-05: yüksek seviye sınıfta `new` ile somut altyapı yok
- [ ] SOL-06: §4 tablosundaki mimari testler yeşil
- [ ] SOL-07: normal akış exception ile taşınmıyor; boş catch = 0
- [ ] SOL-08: tek implementasyonlu `interface` gerekçeli (test seam / 2. kullanım)
- [ ] DI kayıt tek yerde (`Host`), servis yaşam döngüsü belirli

---

## §6 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme |
|---|---|---|---|---|
| 1 | 1000 satırlık `MainForm` | SOL-01 | her değişiklik kırar | handler/servis katmanı |
| 2 | Provider `switch` zinciri | SOL-02 | yeni model = kod değişimi | strategy + DI |
| 3 | Arayüzde `NotSupported` fırlatan impl | SOL-03 | çağrı tarafında patlama | yetenek bölünmesi |
| 4 | 15 metotlu tek tool arayüzü | SOL-04 | sahte implementasyonlar | `ITool` |
| 5 | Serviste `new SqliteConnection(...)` | SOL-05 | test edilemez | arayüz + enjeksiyon |
| 6 | Mimari test yokken "SOLID" onayı | SOL-06 | G5 ihlali | NetArchTest ekle |
| 7 | `catch` ile normal akış | SOL-07 | gizli hata, yavaş akış | `Result<T>` |
| 8 | Her sınıf için `interface` + `factory` | SOL-08 | dolgu katman | soyutlamayı ertele |

---

## §7 Doğrulama

- [ ] 8 kural `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] Her kuralda VersaCoder'a özgü kısa `BAD`/`GOOD` C# örneği var
- [ ] Diyagram saf ASCII (katman yönü tek oklu)
- [ ] EF Core'a referans yok; DevExpress yalnız UI örneğinde
- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`

---

## §8 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G5/G8) | `../CLAUDE.md` |
| 2 | Anahtar: SOLID, katman, bağımlılık yönü | `../keys.md` |
| 3 | Kod kuralları | `coding-standards.md` |
| 4 | Veri erişimi arayüzleri | `data-access-rules.md` |
| 5 | DevExpress katman izolasyonu | `devexpress-rules.md` |
| 6 | Mimari test stratejisi | `../spec/test-strategy.md` |
