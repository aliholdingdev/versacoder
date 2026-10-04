---
title: "VersaCoder — C# Kodlama Standartları Kuralları"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder kaynak kodu için bağlayıcı C# dil kuralları — namespace, nullable, async, exception, yorum, analyzer, iki bacak ve git kuralları (CLAUDE.md G1/G6/G8 teknik derinliği)"
---

# VersaCoder — Kodlama Standartları (`.rules/coding-standards.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [solid-rules.md](./solid-rules.md) · [naming-conventions.md](./naming-conventions.md) · [security-rules.md](./security-rules.md) · [testing-rules.md](./testing-rules.md)

> **Tek cümlelik sorumluluk:** *VersaCoder C# kodu hangi dil kurallarına uyar, ihlal nasıl görülür, nasıl düzeltilir* sorusunun tek cevabıdır.
> Bu dosya `CLAUDE.md` **G1 / G6 / G8** guardrail'larının teknik derinliğidir; çelişirse `CLAUDE.md` kazanır.

---

## §1 Kapsam

**İçinde:** `src/` altındaki **tüm** C# kaynak kodu — `net48` ve `net10.0-windows` **iki bacağı da** aynı kurallara tabidir. Test kodu `.rules/testing-rules.md`, veri erişimi `.rules/data-access-rules.md`, güvenlik `.rules/security-rules.md` ile bu dosyanın kesiştiği yerde **dar olan kural kazanır**.

**Dışında:** vault MD dosyaları (biçim → `CLAUDE.md` G4), SQL metinleri (→ `data-access-rules.md`), DevExpress Designer dosyaları (→ `devexpress-rules.md`).

> **KURAL:** Kural, *öneri* değil **kapı**dır. İhlal = inceleme RED; bulgu biçimi `.agents/reviewer.md` konusudur (konum + neden + düzeltme).

### §1.1 Uygulama Sırası

```
yazım öncesi  ->  bu dosyanin §2 tablosu
yazım sırasi  ->  .editorconfig + analyzer (otomatik yakalar)
inceleme      ->  §5 review checklist
teslim        ->  §6 common violations taranir, 0 kalmali
```

### §1.2 Kural ID Aralığı

| Dosya | Önek | Aralık |
|---|---|---|
| `coding-standards.md` (bu dosya) | `CS-` | CS-01 .. CS-18 |
| `security-rules.md` | `SEC-` | SEC-01 .. SEC-16 |
| `devexpress-rules.md` | `DX-` | DX-01 .. DX-15 |
| `solid-rules.md` | `SOL-` | SOL-01 .. SOL-08 |
| `data-access-rules.md` | `DA-` | DA-01 .. DA-16 |
| `testing-rules.md` | `TS-` | TS-01 .. TS-15 |
| `naming-conventions.md` | `NC-` | NC-01 .. NC-18 |

---

## §2 Kurallar (CS-01 .. CS-18)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| CS-01 | **File-scoped namespace**: `namespace VersaCoder.Domain;` — bloklu `namespace X { }` yasak | Her dosyada 1 seviye girinti azalır; birleştirme çatışması çıkmaz | `namespace X {` satırı ve kapanış `}` | `;` ile file-scoped'a çevir, girintileri bir seviye geri al |
| CS-02 | **`<Nullable>enable</Nullable>`** her projede; `#nullable disable` yasak; `!` (null-forgiving) yalnız gerekçeli | NRT, net48+net10 ortak kodda çalışma anı NullReferenceException'ı erken yakalar | CS86xx uyarısı bastırılmış; dosya başında `#nullable disable` | Uyarıyı düzelt (null kontrolü/`?`/`default!` yerine gerçek tasarım), kodu gizleme |
| CS-03 | **Async sonuna kadar**: `async`/`await` zorunlu; `.Result`, `.Wait()`, `.GetAwaiter().GetResult()` **yasak** | net48'de senkron-context deadlock'u; tutarsız kilit davranışları (G6) | Kodda `.Result` / `.Wait()` geçen satır | Tüm yol `async Task`; üst katmana `await` ile taşı |
| CS-04 | **Her async public metot `CancellationToken` alır** ve aşağıya iletir | İptal edilemeyen görev UI'ı dondurur, oturum kapatıldığında işlem sızmaya devam eder | `async Task` imzasında token yok; token parametresi hiç kullanılmamış | `CancellationToken cancellationToken = default` ekle, her çağrıya geçir |
| CS-05 | **Tipli hata**: özel exception (`SessionException`) ya da `Result<T>`; `catch { }` ve `catch (Exception) { /* */ }` **yasak** | Boş catch sessiz veri kaybı üretir; hata kaynağı kaybolur | Boş catch bloğu; yalnız `throw;` içermeyen genel catch | Ya yukarı taşı (`throw;`), ya tipine göre yakala, ya `Result<T>` ile döndür |
| CS-06 | **`async void` yasak** — yalnız WinForms event handler (günceleme: `EventHandler`) | `async void` istisnası uygulamayı çökertir | `async void Foo()` metot imzası | `async Task` yap; caller `await` etsin; handler dışında hiç kullanma |
| CS-07 | **String**: interpolasyon `$"..."`, çok satırlıda `"""` (net10) / `@"..."`; döngüde >5 birleştirme → `StringBuilder` | + operatörüyle döngü = O(n²) tahsis; `string.Format` tip güvenliği yok | `s = s + x;` geçen döngü; kayan `string.Format` | `StringBuilder` / interpolasyon; **SQL için hiçbiri → DA-03** |
| CS-08 | **Guard clause + düz gövde**: erken `return`; iç içe geçme en fazla **3** seviye | Derin gövde kuralı saklar, incelemeyi imkânsız kılar | `if` blokları 4+ seviye; `else` kuleleri | Koşulu negatifleyip erken `return`/`throw`; metodu böl (CS-09) |
| CS-09 | **Metot ≤ 50 satır, cyclomatic ≤ 10; sınıf ≤ 500 satır** (kılavuz; aşım gerekçesi yazılı) | Uzun metot = birden fazla sorumluluk = SOLID ihlali | Tek metot 100+ satır; tek sınıf 800+ satır | Yardımcı metoda böl; farklı sorumluluk sınıfa taşınır (`SOL-01`) |
| CS-10 | **Yorum politikası**: yorum *ne değil neden* anlatır; commented-out kod **yasak**; public API `///` XML doc; **kod içi yorum ve doc İngilizce** (vault prose Türkçe — NC-18) | Yorumlar kodla birlikte eskir; ölü yorum yanlış yönlendirir | `// TODO eski hali` blokları; kodu tekrar eden yorum | Ölü yorumu sil; *neden* gerekiyorsa tek satır yaz; TODO yerine issue/ADR |
| CS-11 | **Analyzer ciddidir**: `.editorconfig` ile uyarı seviyeleri; uygun uyarılarda `TreatWarningsAsErrors=true`; uyarı **gizlenmez** (`#pragma` yalnız gerekçeli ve tek satır) | Uyarı-görmezden = kural ölü | Yoğun `#pragma warning disable`; CI'da uyarısız ama kodunda CS86xx | Uyarıyı düzelt; `#pragma` gerekiyorsa yanına neden + kural ID yaz |
| CS-12 | **İki bacak**: `#if` yalnız platform farkında (`NET48`/`NET10_0_OR_GREATER`); iş mantığı `#if` ile bölünmez (G6) | `#if` ile bölünen mantık = iki kod tabanı, iki test yükü | Aynı ifade bloğu iki kez `#if` içinde; iş kuralı koşullu derleme | Ortak kaynak yaz; fark **yalnız** platform API'sinde ise `#if` tek satır/tek metot ile |
| CS-13 | **Kopyalama yasak (G1)**: `C:\www\...` vault'ları ve `*.old.*` kopyalar **salt okunur**; içerik elle yazılır | Baştan kopya, VersaCoder'a özgü olmayan kural/teknoloji taşır (ör. EF Core, v24.2 sızıntısı) | Kaynak kod/belge parçası birebir yabancı vault'tan gelmiş | Sil, sıfırdan yaz; yalnız *yapı* referans alınır |
| CS-14 | **Magic değer yok**: sabit/`enum`/yapılandırma; dosya yolu, URL, timeout literal kodda yazılmaz | Değişecek değer kodda gömülüyse her kullanım noktası değişir | Sayısal literal 2+ yerde tekrar; `"C:\..."` literal | `internal static class Constants` ya da yapılandırma; yollar `IAppPaths` |
| CS-15 | **Kaynak yönetimi**: `using` / `await using`; `IDbConnection`, `Stream`, `Process`, timer elle dispose edilmez | Sızan handle → dosya kilidi, bağlantı havuzu tükenmesi | `new SqliteConnection(...)` `using`'siz; `Process.Start` sonrası `Dispose` yok | `using`/`await using` ile sarmala; sahiplik kuralı yorumda değil tasarımda |
| CS-16 | **Değiştirilemezlik tercihi**: DTO/sonuç tipleri `record` + `init`; `var` tip açıkken; identifier karşılaştırmaları `StringComparison.Ordinal` | Paylaşılan durum yarar; kültür duyarlı karşılaştırma yerel ayar bağımlılığı | DTO'da set edilebilir public alan; `==` ile yol/isim karşılaştırması | `record`/`readonly` yap; ordinal karşılaştırma kullan |
| CS-17 | **Git disiplini**: commit küçük ve anlamlı; `bin/`, `obj/`, `*.user`, secret **commit edilmez**; dal adı `feature/<english-slug>`; bırakılmamış `<<<<<<<` yasak | Büyük diffs incelemeyi; sızan artifact/secret kalıcı olur (SEC-01) | Diff'te 1000+ satır tek commit; `.user`/`.db` dosyası stage'de | Bölerek commit; yanlışlıkla eklenmiş dosyayı kaldır; çakışmayı çözmeden push etme |
| CS-18 | **DDD/CQRS yalnız gerektiği yerde** (keys #15): invariant `Domain`'de; use-case akışı `Application` handler/servis; CQRS **gerektiğinde** | Her yere CQRS/DDD = boş katman ve anlamsız `Command/Query` sınıfları | Form kodunda iş kuralı; serviste invariant; her metod `ICommandHandler` | Kuralı `Domain`'e, akışı `Application`'a taşı; basit CRUD için ek soyutlama kurma (YAGNI) |

> **YASAK:** Bu tablodaki bir kuralı "istisna gerekiyor" diyerek sessizce uygulamamak. İstisna = gerekçesiyle birlikte `log.md`'ye kayıt ve incelemede açık onay.

---

## §3 Kritik Kuralların Ayrıntısı

### §3.1 CS-03 + CS-04 — Async Zinciri (BAD -> GOOD)

```csharp
// BAD — net48'de deadlock riski + iptal edilemez
public SessionDto Get(string id)
{
    var rows = _db.Query<SessionDto>(sql).ToList();   // senkron I/O
    return rows.Single();                              // .Result'a düşebilir
}

// GOOD — sonuna kadar async, token taşıyan
public async Task<SessionDto?> FindAsync(string id, CancellationToken cancellationToken)
{
    var rows = await _db.QueryAsync<SessionDto>(sql, new { Id = id },
        commandTimeout: DbTimeouts.Query,
        cancellationToken: cancellationToken).ConfigureAwait(false);
    return rows.SingleOrDefault();
}
```

| İhlal | Yakalayan |
|---|---|
| `.Result` / `.Wait()` | `CS` analyzer + §5.3 taraması (`.Result` geçen satır = 0) |
| token'sız async imza | inceleme (CS-04) + testte iptal senaryosu (`TS-07`) |

### §3.2 CS-02 — Nullable (BAD -> GOOD)

```csharp
// BAD — NRT kapalıymış gibi davranan kod
public string GetTitle(int id)
{
    var row = _rows.FirstOrDefault(r => r.Id == id);
    return row.Title!;                 // null ise çalışma anı patlar
}

// GOOD — tip sistemiyle konuşur
public string? FindTitle(int id)
{
    var row = _rows.FirstOrDefault(r => r.Id == id);
    return row?.Title;
}

public string GetTitle(int id)
    => FindTitle(id) ?? throw new KeyNotFoundException($"Session {id} not found.");
```

### §3.3 CS-12 — İki Bacak (BAD -> GOOD)

```csharp
// BAD — iş mantığı #if ile bölündü (G6 ihlali)
#if NET48
    public string Normalize(string s) => s.Trim().ToLowerInvariant();
#else
    public string Normalize(string s) => s.Trim().ToLowerInvariant();  // kopya
#endif

// GOOD — ortak kaynak; #if yalnız gerçek platform farkında
public string Normalize(string s) => s.Trim().ToLowerInvariant();

#if NET48
    // .NET Framework'te yalnız bu API farklıdır — tek satır, tek yer.
    internal static string GetAppDataPath() => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#else
    internal static string GetAppDataPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif
```

> **UYARI:** Her `#if` bir test yükü ikiye katlar. `#if` sayısı incelemede tek tek gerekçelendirilir; iş mantığı içinde `#if` = otomatik RED.

### §3.4 CS-15 — Kaynak Sahipliği Tablosu

| Kaynak | Sahip | Desen |
|---|---|---|
| `DbConnection` | çağıran repository metodu | `using var conn = ...;` — asla static alan |
| `DbTransaction` | bağlantı sahibi metot | `using var tx = conn.BeginTransaction();` + tek commit |
| `Process` | tool çalıştırıcı | `using` + `WaitForExit(timeout)` + zaman aşımında `Kill` |
| `StreamWriter` (log) | log altyapısı | APP kapalısında flush + dispose |
| DevExpress kontrol | `.Designer.cs` (`IContainer`) | elle `Dispose` **yapılmaz** (DX-12) |

---

## §4 Review Checklist (incelemede işaretlenir)

- [ ] CS-01: tüm dosyalarda file-scoped namespace
- [ ] CS-02: projede `Nullable=enable`; `#pragma`/`!` gerekçeli
- [ ] CS-03: `.Result`, `.Wait()`, `GetAwaiter().GetResult()` geçen satır = **0**
- [ ] CS-04: her `async` public imzada `CancellationToken` var ve kullanılıyor
- [ ] CS-05: boş catch = **0**; hata ya yukarı taşınıyor ya `Result<T>` oluyor
- [ ] CS-06: `async void` yalnız WinForms handler
- [ ] CS-07: döngüde string birleştirme yok; SQL birleştirmesi hiç yok (DA-03)
- [ ] CS-08: iç içe geçme ≤ 3; kule `if/else` yok
- [ ] CS-09: >50 satır metot ya gerekçeli ya bölünmüş
- [ ] CS-10: commented-out kod = 0; public API XML doc tamam; yorumlar İngilizce
- [ ] CS-11: yeni uyarı yok; `#pragma` gerekçeli
- [ ] CS-12: `#if` sayısı artmadı; iş mantığında `#if` yok
- [ ] CS-13: kaynak kod `C:\www` / `*.old.*` içeriğiyle birebir değil
- [ ] CS-14: yinelenen literal ve hardcoded yol yok
- [ ] CS-15: her `new` edilen IDisposable sarılı
- [ ] CS-16: DTO'lar `record`/`init`; identifier karşılaştırmaları ordinal
- [ ] CS-17: staged dosyalarda artifact/secret yok; commit boyutu makul
- [ ] CS-18: iş kuralı doğru katmanda; gereksiz CQRS/DDD soyutlaması yok
- [ ] **İki bacak:** `dotnet build -f net48` ve `-f net10.0-windows` **ikisi de 0 hata**

---

## §5 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme | Bulduğu yer |
|---|---|---|---|---|---|
| 1 | `await ...` sonucu `.Result` ile açmak | CS-03 | UI donması, net48 deadlock | tam async zincir | kod incelemesi |
| 2 | `catch { }` ile "sessizce devam" | CS-05 | kayıp veri, bulunamayan hata | `throw;` ya da `Result<T>` | kod incelemesi + test |
| 3 | `#nullable disable` ile uyarıyı kapatma | CS-02 | çalışma anı NRE | gerçek null tasarımı | build çıktısı |
| 4 | İş mantığının `#if NET48` içine alınması | CS-12 | iki farklı davranış, test yarısı eksik | ortak kaynak | kod incelemesi |
| 5 | Yabancı vault'tan blok kopyası | CS-13 | EF Core/v24.2 sızıntısı | sıfırdan yazım | diff + `link-scan` |
| 6 | Dosya yolu literal (`"C:\\data\\x.db"`) | CS-14 | taşınabilirlik kırılır | `IAppPaths` + yapılandırma | kod incelemesi |
| 7 | Yorumda kalan eski kod bloğu | CS-10 | okunamaz dosya | sil | kod incelemesi |
| 8 | Token alınıp hiç kullanılmamış | CS-04 | iptal çalışmıyor | token'ı çağrılara geçir | test (`TS-07`) |
| 9 | Tek metotta 150 satır + 12 `if` | CS-09 | bakılamaz, test edilemez | böl + isimlendir | metrik taraması |
| 10 | `bin/`, `*.user` veya secret'ın commit'i | CS-17 | repo kirliliği, sızıntı | kaldır + `.gitignore` | git diff |

---

## §6 Doğrulama

- [ ] 18 kuralın tamamı `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] §3 örnekleri `net48` ve `net10.0-windows` için de geçerli
- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`
- [ ] Bağlantılar yalnız mevcut dosyalara (`../index.md`, `../CLAUDE.md`, `../keys.md`, kardeş `.rules/*.md`)
- [ ] Diyagramlar saf ASCII ( kutu çizimi yok )

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G1/G6/G8) | `../CLAUDE.md` |
| 2 | Navigasyon | `../index.md` |
| 3 | Anahtar kelime haritası | `../keys.md` |
| 4 | Kapsam analizi (5 senior karar) | `../reports/00-kapsam-analizi.md` |
| 5 | SOLID kuralları | `solid-rules.md` |
| 6 | Test kuralları | `testing-rules.md` |
| 7 | İsimlendirme | `naming-conventions.md` |
