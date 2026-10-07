---
title: "brain.md — Beyin: ADR Özeti, SOLID ve Kod Kuralları"
type: reference
category: ".ai"
status: active
version: 3.0.0
updated: 2026-10-07
authority: derived
scope: "29 ADR tek satır özeti, ters-senaryo/yakalama, SOLID kısıtları, 6 sert kod kuralı, çökme noktaları ve prompt kaynağındaki mimari kararlar"
tags: [adr]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# brain.md — Beyin

> **Tek cümlelik sorumluluk:** 29 ADR'nin tek satırlık özetini, her kararın ters-senaryosunu, SOLID kısıtlarını ve yazılacak her kodun uymak zorunda olduğu kuralları tek ekranda tutmak.

## §0 Özet (Answer First)

- **ADR-001..029 = kayıtlı 29 karar (28 accepted + ADR-020 superseded).** Özet §1'de; her kararın "tersi yapılsaydı" cevabı ve yakalama noktası §1.1'de (29 satır). Tam metin `.ai/.decisions/accepted/`.
- **SOLID vault'a nasıl çakılır:** §2 — beş ilke + bir ek ilke; her biri kural / kötü örnek / iyi örnek / yakalama yöntemi dörtlüsüyle.
- **Kod kuralları (6 sert kural):** `Result<T>` her dönüşte · structured log 5 seviye (ADR-026) · TDD %90+ hedef · `nullable` açık · parametreli SQL · secret kodda yok. §3; örnekler §3.7.
- **Çökme noktaları:** EF Core yasağı, string-concat SQL, DevExpress sürüm sapması, WPF/MAUI, Unicode kutu çizimi — her biri neden + yakalama ile §4.2 kara listesinde.
- Vault kök `.md` dosyaları bu dosyanın bağlamıdır: `index.md`, `CLAUDE.md`, `WORKFLOW.md`.

## §1 ADR-001..029 — Tek Satır Özet Tablosu

Karar sütunu, her ADR'nin kendi "tek cümlelik karar/ifade" bölümünden özetlenmiştir; son sütun tek kelimelik gerekçedir.

| ADR | Karar (tek satır) | Gerekçe |
|-----|-------------------|---------|
| 001 | Tüm projeler `net48` + `net10.0-windows` çift TFM ile derlenir; `#if` yalnız platform farkına izin verilir | kanıt |
| 002 | Vault içeriği elle sıfırdan yazılır; referans kaynaklar salt okunur, hiçbir metin kopyalanmaz | mülkiyet |
| 003 | Vault ≥500 MD, kök belgeler 30–88 KB, her `.md` 8 alan frontmatter, bozuk link = 0 | ölçek |
| 004 | DevExpress **tek sürüm v26.1** + tek ribbon stili `RibbonControlStyle.Office2010` | sürüm |
| 005 | Veri erişimi ADO.NET + Dapper; **EF Core yok**; parametreli SQL zorunlu; migration = saf SQL | hafiflik |
| 006 | opencode araç kümesi (`read/write/edit/bash/grep/glob/list`) tek C# sözleşmesi: `VersaCoder.Protocol` | sözleşme |
| 007 | Çalışma zamanı = 1 ana oturum + alt oturumlar; alt oturum yetki devretmez, derinlik 2 | hiyerarşi |
| 008 | Transcript 8 MB'ı aşarsa compaction: son 20 tur + özet kalır, özgün metin arşive silinmez | sınır |
| 009 | Her araç çağrısı 60 sn timeout (`bash` 300 sn), iptal edilebilir token, çalışma klasörü = alan kökü | sandbox |
| 010 | Desktop ve CLI `l0..l6` çekirdeğini birebir paylaşır; ayrışma yalnız `l7` | tekrar |
| 011 | LLM sırası `openrouter-free → ollama → openai-compatible`; streaming (SSE) zorunlu; anahtar secret store'dan | fallback |
| 012 | SQL Server = `Microsoft.Data.SqlClient` — **iki bacakta aynı major**; karar anındaki pin `5.x`, **güncel stable 7.1.1 (2026-09-29, WEB_VERIFIED)** → pin yenilenmeli | uyum |
| 013 | Mimari = **8 makro katman (l0..l7) + 44 alt katman = 52 düğüm**; her düğümde bileşen kaydı, toplam ≥5000 | sınır |
| 014 | Çözüm `VersaCoder.slnx`; paket sürümleri yalnız `Directory.Packages.props` (CPM), `.csproj`'da `Version` yok | merkez |
| 015 | Her ekran `.Form.cs + .Form.Designer.cs + .resx` üçlüsü; Designer/resx yalnız tasarımcıdan, son söz `ui-designer` | tasarımcı |
| 016 | ADR-005 geçerli; ek sınır: `DbConnection` yalnız `l4 ado-dapper`'da, başka katman bağlantı tutamaz | kapsam |
| 017 | Tüm SQL parametreli (`@p1..@pN`); string-concat SQL = kod incelemesinde otomatik RED | enjeksiyon |
| 018 | Secret'lar DPAPI ile şifreli yerel depoda; `keys.md` yalnız harita, değer içermez | gizlilik |
| 019 | Her değişiklik **iki bacakta da** build + test; tek bacak yeşil = bitmemiştir | doğrulama |
| 020 | Diyagramlar UTF-8 box-drawing; mermaid varsa yanında düz metin karşılığı da zorunlu — **D1 ASCII ile `superseded` (ADR-029); D3 mermaid kuralı yürürlükte** | okunabilirlik |
| 021 | Vault navigasyonu tek `index.md`; her dizinde en fazla bir, hiçbir liste iki dosyada tekrarlanmaz | SRP |
| 022 | İlk commit yalnız GATE-004'te; önce 0 bozuk link + GATE-003 wireframe onayı | eşik |
| 023 | `%TEMP%\opencode-fresh` referans ağaçı salt okunur; yapı öğrenilir, kod/metin kopyalanmaz | referans |
| 024 | CoreMusic vault (`C:\www\coremusic.net\.ai`) dokunulmaz; yalnız yapı referansı, içerik kopyalanmaz | koruma |
| 025 | 7 ekranın wireframe'i GATE-003'te onaylanmadan hiçbir `.Form.cs/.Designer.cs/.resx` yazılmaz | onay |
| 026 | Tüm loglar tek structured şablon + 5 seviye + iki bacakta aynı alan adları; serbest metin log yasak | şablon |
| 027 | "5000+ bileşen" elle sayılmaz; tek kanıt `scripts/count-components.ps1` (`-MinRecords 5000`, `-MinNodeFiles 52`) | ölçüm |
| 028 | 8 makro katman / 52 düğüm / 9 üretim projesi + `VersaCoder.Tests` (+ ArchitectureTests) baz alınır | taban |
| 029 | Diyagramlar **saf ASCII `+ - |`**; Unicode box-drawing `U+2500-U+257F` vault'ta yasak (G4 taraması = 0); ADR-020 D1'i supersede eder | uyumluluk |

### §1.1 Ters-Senaryo ve Yakalama Karşılığı (29 satır)

Her ADR için iki ek satır: **tersi yapılsaydı ne olurdu** ve **bu hatayı hangi kontrol yakalar**. Kontrol adları `.ai/checklists/` altındaki gerçek dosyalardır; yalnız `layer-boundary-check` link verilmiştir, gerisi düz metin adıdır.

| ADR | Tersi yapılsaydı ne olurdu | Yakalama noktası |
|-----|----------------------------|------------------|
| 001 | Tek TFM seçilseydi öbür bacak hiç gerçek kullanıcıda denenmezdi; legacy ekran ya modern API tarafı feda edilirdi | `dual-tfm-build-check` + §4.1 "tek TFM'de yeşil test" satırı |
| 002 | Kopya içerik başlasaydı başka projenin kuralı burada geçerli olurdu; iki vault çelişince hangisinin doğru olduğu bilinmezdi | `documentation-check` + cümle-bazlı spot check |
| 003 | Dosyalar özet seviyesinde (1–16 KB) kalsaydı vault aranamaz; frontmatter düşerse link ve ölçüme kapısı da düşerdi | `vault-link-check` + bozuk link = 0 komutu |
| 004 | Sürüm/stil sapması olsaydı `.Designer.cs` ve `.resx` tasarımcıda açılmaz, ribbon düzeni kayardı | `ribbon-style-check`, `devexpress-control-check` |
| 005 | EF Core girseydi net48 bacagında sürüm kilidi oluşur, SQL görünmez olurdu; migration denetlenebilir olmazdı | `data-access-check`, `migration-check` + §4.1 EF satırı |
| 006 | Araçlar ayrı ayrı sarmalansaydı tek bir araç adı değişimi tüm çağrı noktalarını kırar, hata noktası belli olmazdı | `tool-contract-spec` (spec/) ile sözleşmenin birebir eşleşmesi |
| 007 | Derinlik sınırsız olsaydı alt oturum kendi yetkisini devredebilir, token ve dosya yazma patlardı | `session-handoff-check` + yetki devri 4 madde kuralı |
| 008 | Compaction olmasaydı transcript belleği taşır; özgün metin silinseydi geçmiş konuşma kanıtı kaybolurdu | ayrı denetim listesi yok → **DOĞRULAMA GEREKLİ** |
| 009 | Timeout olmasaydı tek araç çağrısı gate'i sonsuza kadar askıda tutar; çalışma klasörü sabitlenmezse betik yanlış dizine yazar | ayrı denetim listesi yok → **DOĞRULAMA GEREKLİ** |
| 010 | İki ayrı çekirdek yazılsaydı her özellik iki kez implemente edilir, davranış sapması ve 2x iş oluşurdu | ortak çekirdek testi: "aynı iş, Desktop ve Cli'de aynı sonuç" |
| 011 | Sıra değişseydi ücretsiz sağlayıcı çağrılmaz ya da tek sağlayıcı kesilince ürün susardı; anahtar koda girerdi | `secret-leak-check` + l4 llm-http sıra testi |
| 012 | Bacaklar farklı major kullansaydı NU1201 benzeri uyumsuzluk ve sessiz davranış farkı çıkardı | `dependency-update-check` + iki bacak matrisi (`PROJECTS.md` §2.1) |
| 013 | Katman/ölçü belirsiz kalsaydı herkes kendi sayısını uydurur, "5000+" iddiası kanıtsız kalırdı | `count-components.ps1` + [layer-boundary-check](../checklists/layer-boundary-check.md) |
| 014 | `.csproj` başına `Version` satırı olsaydı 8 paket × 11 proje sürüm sapması restore'u kırardı | `dependency-update-check` + `.csproj` içinde `Version` = 0 araması |
| 015 | Designer elle yazılsayda kaynak ürün bozulur, ekran tasarımcıda açılmaz ve geri alınamazdı | `winforms-designer-check` |
| 016 | Bağlantı üst katmanda tutulsaydı katman testi izole edilemez, transaction bütünlüğü dağılırdı | `data-access-check` + NetArchTest "bağlantı yalnız l4" kuralı |
| 017 | Concat SQL kalsaydı enjeksiyon doğrudan veritabanına açılırdı; kod incelemesi bunu yakalayamayabilir | `sql-injection-review` + §4.1 `"SELECT ..." + değişken` satırı |
| 018 | Anahtar koda/repoya girerse depo kamuya açılır; DPAPI olmasa yerel dosya düz metin okunurdu | `secret-leak-check`, `security-review-checklist` |
| 019 | Tek bacakta yeşil "bitti" sayılsaydı kullanıcı makinesinde kırılır, kanıt sahte olurdu | `dual-tfm-build-check`, `gate-005-test-kapsami` |
| 020 | Diyagram düz metin karşılıksız kalsaydı komut çıktısı ve arka uç okuyucu diyagramı göremezdi | `documentation-check`; D1 kapsamı ADR-029 ile ASCII'ye döndü (§4.2 madde 5 kapandı) |
| 021 | İki indeks olsaydı iki gerçek doğar, link listesi bayatlar ve vault'un yarısı görünmez kalırdı | `vault-link-check` + "her dizinde en fazla bir liste" taraması |
| 022 | Erken commit olsaydı bozuk linkli, kapısız depo geçmişi oluşur ve geri alma pahalılaşırdı | `gate-004-ilk-commit`, `gate-004a-vault-kapanis` |
| 023 | Referans ağaçına yazılsaydı ikinci oturum artık temiz referans göremez, öğrenme kaynağı kirlenirdi | yalnız-oku komut denetimi → **DOĞRULAMA GEREKLİ** |
| 024 | Başka vault'a yazılsaydı ADR-002 ihlali ve başkasının verisinin bozulması birlikte oluşurdu | `documentation-check` + kopya blok taraması |
| 025 | Onaysız Form kodu yazılsaydı tasarım değişince kod iki kez yazılır, onay zinciri koparıdı | `gate-003-ui-onay` |
| 026 | Serbest metin log kalsaydı iki bacakta alan adları ayrışır, hata günlükten çözülemezdi | `logging-observability-check` + §4.1 log satırı |
| 027 | Elle sayıma izin verilseydi "5000+" iddiası her raporda farklı çıkardı; kapı sayıya güvenirdi | `gate-004a-vault-kapanis` (Q5) + §4.1 elle yazılmış sayı satırı |
| 028 | K3'ün 9 Tier / 575 csproj senaryosu kabul edilseydi ~1150 hedef oluşur, restore/IDE çökerdi | [layer-boundary-check](../checklists/layer-boundary-check.md) + `PROJECTS.md` §3 altı düzeltme |
| 029 | Unicode box-drawing kalsaydı her terminal/monospace olmayan ortamda kutu bozulur, G4 taraması vault genelinde sürekli RED verirdi | kutu taraması `U+2500-U+257F` = 0 + `documentation-check` |

**Okuma kuralı:** bir kod değişikliği bu tabloda bir satırı ihlal ediyorsa, o satırın "Yakalama noktası" sütunundaki komut **değişiklikten önce** çalıştırılır; komut çıkışı olmayan düzeltme tamamlanmış sayılmaz.

### §1.2 ADR → Etkilediği Dosya ve Ölçülebilir Çıktı

§1 kararı, §1.1 ters-senaryoyu, bu tablo ise kararın **hangi dosyada gözlemleneceğini** yazar. Çıktı sütunu sayı/komut içerir; sayı içermeyen satır kanıt sayılmaz.

| ADR | Etkilediği dosya / kapı | Ölçülebilir çıktı |
|-----|-------------------------|-------------------|
| 001 | her `.csproj`'ın `TargetFramework` satırı | `dotnet build` × 2 = 0 hata |
| 002 | tüm `.md` gövdeleri | yabancı/kopya blok = 0 |
| 003 | kök `.md` frontmatter + boyut bandı | NoFrontmatter = 0, band 30–88 KB |
| 004 | `.resx`, `.Form.Designer.cs`, `Directory.Packages.props` | DevExpress ≠ 26.1 satırı = 0, `Office2010` dışı stil = 0 |
| 005 | Infrastructure veri kodu | `using Microsoft.EntityFrameworkCore` = 0 |
| 006 | Protocol `tool-schema` + `spec/tool-contract-spec.md` | sözleşmedeki araç adı = 7 (`read/write/edit/bash/grep/glob/list`) |
| 007 | oturum ve alt oturum yönetim kodu | derinlik ≤ 2, yetki devri 4 madde dolu |
| 008 | transcript depolama kodu | 8 MB eşiği aşınca compaction; arşivden silinen özgün metin = 0 |
| 009 | araç çalıştırıcı (tool-runner) | timeout 60 sn / bash 300 sn, iptal token mevcut |
| 010 | `VersaCoder.Desktop` + `VersaCoder.Cli` | ortak çekirdek testi iki girişte de aynı sonucu verir |
| 011 | Infrastructure `llm-http` adaptörleri | sıra = openrouter-free → ollama → openai-compatible |
| 012 | `Microsoft.Data.SqlClient` / `Sqlite` paket satırları | iki bacakta aynı major |
| 013 | `architecture/` klasör yapısı + N01–N44 | 52 düğüm dosyası, ≥5000 kayıt |
| 014 | `VersaCoder.slnx` + `Directory.Packages.props` | `.csproj` içinde `Version` satırı = 0 |
| 015 | 7 ekranın `.Form.cs/.Designer.cs/.resx` üçlüsü | her üçlü dosya var, tasarımcıda açılıyor |
| 016 | Infrastructure `ado-dapper` | `DbConnection` yalnız l4; NetArchTest RED = 0 |
| 017 | tüm SQL çağıran nokta | concat deseni = 0, `@p` kullanılmayan sorgu = 0 |
| 018 | secret store + `keys.md` | `keys.md` içinde değer = 0, repoda anahtar = 0 |
| 019 | `VersaCoder.Tests` koşuları | iki bacakta `dotnet test` = 0 başarısız |
| 020 | diyagram içeren tüm `.md` | diyagramın düz metin karşılığı var |
| 021 | `index.md` + dizin listeleri | ikinci indeks dosyası = 0 |
| 022 | git commit akışı | GATE-004 öncesi commit = 0 |
| 023 | `%TEMP%\opencode-fresh` | o ağaça yazma işlemi = 0 (salt okunur) |
| 024 | `C:\www\coremusic.net\.ai` | o vault'a yazma işlemi = 0 |
| 025 | GATE-003 onay çıktısı | 7 ekran wireframe onayı = 7 |
| 026 | log şablonu ve alan adları | 5 seviye dışı seviye adı = 0 |
| 027 | `scripts/count-components.ps1` | ≥5000 kayıt, 52 düğüm dosyası |
| 028 | `PROJECTS.md` §3 + `architecture/index.md` | 6 düzeltme = 6/6 kapandı |

## §2 SOLID Kısıtları (Vault'a Çakılmış Hâli)

| İlke | Vault karşılığı | Kaynak |
|------|-----------------|--------|
| **S** — Tek sorumluluk | Bir dosyada tek konu; tek `index.md` navigasyonu; ekran = tek Form üçlüsü | ADR-021, ADR-015 |
| **O** — Açık/kapalı | Yeni LLM sağlayıcısı = yeni adaptör, mevcut zincir koddan değişmez; provider sırası sabit | ADR-011 |
| **L** — Liskov | `net48` ve `net10.0-windows` bacakları aynı arayüzü aynı davranışla sunar; tek bacak sapması RED | ADR-019, ADR-001 |
| **I** — Arayüz ayrımı | Araç sözleşmeleri ince uçlar (`read/write/edit/bash/grep/glob/list`); devasa interface yasak | ADR-006 |
| **D** — Bağımlılık tersine | Katmanlar yalnız altına referans verir (`l7→…→l0`, `l3→l0`); üst katman `DbConnection`'ı tanımaz, `ISqlConnectionFactory`/`IDbExecutor`'a bağımlıdır | ADR-005, ADR-016, ADR-028 |
| **I** — Niyet gizleme (ek) | Çağırılan, ne yaptığını değil, neyi başlattığını söyler; ödeme/oturum gibi niyetler arayüz adında kalır | ADR-006, ADR-011 |

Ek kural: **yön = yukarıdan aşağıya**, ters referans NetArchTest ile kanıtlanır ([layer-boundary-check](../checklists/layer-boundary-check.md)).

### §2.1 S — Tek Sorumluluk (SRP)

**Kural:** Bir sınıf/metin tek bir sorumluluk üstlenir; iki farklı değişiklik nedeni varsa sınıf bölünür.

```csharp
// KOTU — hem sorgu yazıyor, hem dosya açıyor, hem logluyor
public User Load(int id) {
    var u = _db.QuerySingle<User>("SELECT ... WHERE Id = @id", new { id });
    _fs.Append("audit.txt", u.Name); return u;
}
```

```csharp
// IYI — tek sorumluluk: sorgu; dosya ve log başka katmanda
public Result<User> Load(int id) =>
    Result<User>.From(_db.QuerySingleOrDefault<User>(ById, new { id }));
```

**Yakalama yöntemi:** sınıf adında iki işlev ailesi yan yana geliyorsa (ör. `FileUserStore`) `reviewer` bölünmesini ister; vault tarafında aynı kural `SRP` = tek dosya tek konu (ADR-021) olarak denetlenir.

### §2.2 O — Açık/Kapalı (OCP)

**Kural:** Davranış eklemek için mevcut kod **değiştirilmez**; yeni davranış yeni adaptör/sınıf ile eklenir.

```csharp
// KOTU — her yeni saglayici icin mevcut zincire if eki
if (name == "openrouter") return new OpenRouterClient();
else if (name == "ollama") return new OllamaClient();
```

```csharp
// IYI — zincir degismez, yeni saglayici yeni adaptor
providers.Add(new OpenRouterAdapter());   // sira: ADR-011
providers.Add(new OllamaAdapter());
```

**Yakalama yöntemi:** mevcut dosyaya `else if` / `switch` ile **yeni dal** ekleniyorsa önce sorulur: "bu zaten bir adaptör mü?" Cevap hayırsa kod reddedilir. Sıra değişikliği ayrıca ADR-011'e bağlıdır.

### §2.3 L — Liskov Yerine Koyma (LSP)

**Kural:** Alt tip, üst tipin yerine geçtiğinde çağıran hiçbir koşul bozulmamalıdır; iki bacak aynı sözleşmeyi aynı davranışla sunmalıdır.

```csharp
// KOTU — net10'da calisir, net48'de basarisiz doner
public bool Send(string url) {
#if NET10_0_WINDOWS
    return _http.Send(url).IsSuccess;
#else
    return false;   // sahte basari
#endif
}
```

```csharp
// IYI — iki bacak ayni sozlesmeyi ayni anlamda uygular
public bool Send(string url) =>
    _sender.Send(url).IsSuccess;   // gerekce farki icin #if yalniz platform farkinda
```

**Yakalama yöntemi:** her iki bacakta aynı test koşulu çalıştırılır (`dotnet test` × 2); tek bacak yeşil ise LSP ihlali sayılır (ADR-019). `#if` bloğunun platform farkı dışında kullanıldığı yer `reviewer` raporuna girer.

### §2.4 I — Arayüz Ayrımı (ISP)

**Kural:** Çağırılmayan üye arayüzde durmaz; arayüz ince ve tek amaçlıdır.

```csharp
// KOTU — tek arayuz her seyi tasiyor, uygulayici bos metotlarla dolu
public interface ITool {
    string Read(string p); void Write(string p, string c);
    void RunShell(string c); void RestartApp(); void PaintIcon();
}
```

```csharp
// IYI — tek tek ince arayuzler; l0'a yalniz gerekenler yazilir
public interface IFileReader { string Read(string path); }
public interface IShellRunner { string Run(string command); }
```

**Yakalama yöntemi:** bir arayüzde uygulayıcının **boş bıraktığı üye** varsa uyarı; arayüz üye sayısı eşiği için ayrı sayı `DOĞRULAMA GEREKLİ`. Araç sözleşmesi tarafında denetim `tool-contract-spec` ile yapılır.

### §2.5 D — Bağımlılık Tersine (DIP)

**Kural:** Üst katman, alt katmanın **somut** sınıfına değil, `l0`'daki portuna bakar; port `l0`'da, uygulama `l4`'tedir.

```csharp
// KOTU — l2, l4'un somut sinifini biliyor (yon ters)
public class SaveCase {
    readonly SqlServerStore _db = new SqlServerStore();
}
```

```csharp
// IYI — l2 yalniz l0 portuna bakar, kurulum l6 Host'ta
public class SaveCase { readonly IRowWriter _db; public SaveCase(IRowWriter db) => _db = db; }
```

**Yakalama yöntemi:** NetArchTest `LayerRulesTests` 8×8 matrise göre ters kenarı RED eder; ayrıca kod içinde `new SqlServerStore(` / `new DbConnection(` ifadesi `l2..l7` içinde yakalanırsa RED.

### §2.6 I (ek) — Niyet Gizleme

**Kural:** Bir çağrı "ne yapacağını" değil "neyi başlattığını" söyler; uygulama ayrıdır, niyet ayrıdır.

```csharp
// KOTU — niyet ile mekanizma ayni yerde; degisiklik uygulamayi kirar
public void ChargeCardAndWriteLedgerAndSendMail(Order o) { ... }
```

```csharp
// IYI — niyet arayuzde, mekanizma ayri saglayicida
public interface IPayment { Task<Result> Capture(OrderId id); }
```

**Yakalama yöntemi:** metod adında "And" ile yan yana gelen üç eylem varsa görev birleştirme (SRP) işaretidir; `reviewer` bunu §1.1 tablosundaki ADR-006/ADR-011 satırlarına bağlayarak raporlar.

## §3 Kod Kuralları (Sert)

| # | Kural | Nerede yaşar | Kaynak | Yakalama |
|---|-------|--------------|--------|----------|
| 1 | **`Result<T>` her dönüşte.** İşlem sonucu fırlatmak yerine `Result<T>` döndürür; exception yalnız beklenmeyen çökme içindir | ortak tipler `VersaCoder.Crosscutting` (l5); konum tartışması ADR-028 §2.3/6 ile | ADR-028 §2.3 | §3.7/1 örneği + kod incelemesi; kesin tip konumu `DOĞRULAMA GEREKLİ` |
| 2 | **Structured log, 5 seviye.** Tek şablon, iki bacakta aynı alan adları; serbest metin log yasak. `Trace/Debug/Info/Warn/Error` dışında isim uydurulmaz | disk log kaydı + `.ai/log.md` özeti | ADR-026 | `logging-observability-check` + §4.1 log satırı |
| 3 | **TDD, %90+ coverage hedefi.** Test önce, implementasyon sonra; her iki bacakta `dotnet test` | `VersaCoder.Tests` | ADR-019 | `gate-005-test-kapsami`, `coverage-gate-check`; eşik altı = tamamlanmadı |
| 4 | **`nullable` açık.** Her projede `<Nullable>enable</Nullable>`; `null` akışı compiler'a gösterilir, `!` savsaklama olarak kullanılmaz | tüm `.csproj` | ADR-001 yan kural | derleme uyarısı = derleme hatası (TreatWarningsAsErrors bağlamı ADR-028 §3) |
| 5 | **Parametreli SQL tek doğrudur.** `@p1..@pN`; string-concat SQL = `reviewer` otomatik RED. Her komutta `CommandTimeout` açık set | Infrastructure (l4) | ADR-017, ADR-005 | `sql-injection-review` + §4.1 concat satırı |
| 6 | **Secret kodda/repoda durmaz.** DPAPI şifreli depo; `keys.md` yalnız harita | secret store | ADR-018 | `secret-leak-check`, `security-review-checklist` |

### §3.7 Kod Örnekleri (BAD → GOOD)

**1) Hata yönetimi — exception yerine `Result<T>`:**

```csharp
// BAD — çağıran catch etmek zorunda, akış kayboluyor
public User GetUser(int id) => _db.QuerySingle<User>("SELECT ... WHERE Id = @id", new { id });

// GOOD — sonuç nesnesiyle döner, çağıran karar verir
public Result<User> GetUser(int id)
{
    var user = _db.QuerySingleOrDefault<User>("SELECT ... WHERE Id = @id", new { id });
    return user is null
        ? Result<User>.Fail($"user.not_found:{id}")
        : Result<User>.Ok(user);
}
```

**2) SQL — string-concat otomatik RED (ADR-017):**

```csharp
// BAD — RED (G2 ihlali)
var sql = "SELECT * FROM Files WHERE Name = '" + name + "'";

// GOOD — parametreli
var sql = "SELECT * FROM Files WHERE Name = @name";
conn.Query<FileRow>(sql, new { name });
```

**3) Log — serbest metin yasak (ADR-026):**

```csharp
// BAD — iki bacakta da yakalanamaz, alanı yok
logger.LogInformation("dosya açılamadı " + path);

// GOOD — tek şablon, sabit alan adları (her iki bacakta aynı)
logger.LogInformation("file.open_failed {Path} {ErrorCode}", path, "E_ACCESS");
```

**4) Null — `nullable` açık, `!` savsaklama:**

```csharp
// BAD — compiler'a güven vermeyen körleştirme
var name = user!.Name!;

// GOOD — akış görünür, yoksa alternatif dal
var name = user?.Name ?? "<anonymous>";
```

**5) TDD akışı — kırmızı, yeşil, sadeleştir (ADR-019):**

```csharp
// ADIM 1 KIRMIZI  : test ilk yazilir, derlenmez/ucusur  -> dotnet test = 1 kaldi
[Fact] public void Load_missing_user_returns_fail() =>
    Assert.True(repo.Load(999).IsFail);

// ADIM 2 YESIL    : en kucuk uygulama -> dotnet test = 0 kaldi (iki bacakta)
// ADIM 3 SADELESTIR: kurali bozmadan tekrar; kapsam raporu guncellenir
```

**6) Bağlantı — `CommandTimeout` açık set ve port kullanımı (ADR-005/016):**

```csharp
// BAD — timeout devrede degil, baglanti katman disina siziyor
using var c = new SqlConnection(cs); c.Open(); c.Query(sql);

// GOOD — timeout acik, baglanti yalniz l4 ado-dapper'da
cmd.CommandTimeout = 15;            // sure, cagriyi bekletmemek icin acik
```

**7) DI — port l0'da, kayıt l6 Host'ta (ADR-028 §2.3/6):**

```csharp
// BAD — somut sinif ust katmanda new ediliyor
services.AddSingleton(new SqlServerStore());

// GOOD — l0 portu kaydediliyor, uygulamayi l4 veriyor
services.AddSingleton<IRowWriter, SqlRowWriter>();
```

**8) Migration — saf SQL + version tablosu (ADR-005):**

```csharp
// BAD — ORM migration, surum kaydi kendi icinde gizli
dbContext.Database.Migrate();

// GOOD — surumu dosyada, SQL gorunur, geri alinabilir
Execute("CREATE TABLE schema_version(v int NOT NULL);");   // + dosya basina birim test
```

### §3.8 Katmanlar Arası Yolculuk — Gerçek Bir Senaryo

Kurallar tek tek okununca anlaşılır; bu alt bölüm aynı kuralların **birlikte** nasıl çalıştığını tek bir senaryoda gösterir: kullanıcının bir ayar satırını kaydetmesi (07-settings ekranı ↔ CLI `settings set`).

```text
[Desktop 07-settings / Cli settings set]
        |  1) giris dogrulama  (l2 validators) - kural 3 TDD ile onceden yazildi
        v
[Host l6]  di-registration -> hangi IRowWriter kayitli? (kural D, l0 portu)
        |
        v
[Application l2 SaveSetting]  -- Result<Setting> dondurur (kural 1)
        |                        exception firlatmaz
        v
[Infrastructure l4 SqlRowWriter]  -- @p1 parametreli SQL (kural 5)
        |                          CommandTimeout acik
        v
[Crosscutting l3]  -- setting.save_ok {Key} {ElapsedMs}  (kural 2, structured log)
```

**1) Giriş katmanı — hata fırlatmaz, sonuç döndürür:**

```csharp
// l2 (Application) : validator ilk, sonra handler
var parsed = SettingKey.Parse(raw);              // Result<SettingKey>
if (parsed.IsFail) return Result<Setting>.From(parsed);
```

**2) Host — port kaydı, somut sınıf değil:**

```csharp
// l6 : somut sinif burada bilinir, ust katmanlar portu bilir
services.AddSingleton<IRowWriter, SqlRowWriter>();   // §3.7/7 ile ayni kural
```

**3) Altyapı — parametreli SQL ve açık timeout:**

```csharp
// l4 : tek DbConnection sahibi (ADR-016), concat yok (ADR-017)
const string sql = "UPDATE settings SET value = @v WHERE key = @k";
cmd.CommandTimeout = 15;
cmd.ExecuteNonQuery();   // parametreler @v / @k
```

**4) Kesit — aynı anda log, sabit alan adlarıyla:**

```csharp
// l3 : iki bacakta ayni alan adlari (ADR-026)
logger.LogInformation("setting.save_ok {Key} {ElapsedMs}", key, sw.ElapsedMilliseconds);
```

**5) Çıkış — isteğe bağlı CLI çıktısı:**

```csharp
// l7 Cli : ayni Result<T> gorsellestirmesi, ayni mesaj kodu
if (r.IsFail) { Console.Error.WriteLine(r.Code); return 1; }   // kod uydurulmaz
```

**Neden tek senaryo:** beş bloğun beşi de §3.1'deki altı kuralı farklı yerde uygular; biri eksilirse zincir o halkada kopar ve belirti en uzak uçta (CLI çıkış kodu) görünür — bu yüzden inceleme yukarıdan aşağı değil, zincir boyunca yapılır.

### §3.9 Kod İncelemesi Hızlı Kontrolü (10 soru)

§3.9, §3.7 örneklerinin inceleme sırasında tek bakışta uygulanması içindir. Sütunlar: soru, "evet" ne demek, "hayır" ise ne olur.

| # | Soru | Evet ise | Hayır ise |
|---|------|----------|-----------|
| 1 | Bu metot tek iş mi yapıyor? | SRP geçti | §2.1'e göre bölme öner |
| 2 | Yeni davranış için mevcut `switch/else if` mi genişletildi? | OCP ihlali → adaptör iste | kabul |
| 3 | `#if` yalnız platform farkı mı içeriyor? | LSP/ADR-001 geçti | kodu ikiye böl |
| 4 | Arayüzde hiç kullanılmayan üye var mı? | ISP ihlali → arayüzü incelt | kabul |
| 5 | Üst katmanda `new ...Store(...)` / `new SqlConnection` var mı? | DIP ihlali → `l0` portu iste | kabul |
| 6 | Her dönüş `Result<T>` mi, exception mı? | istisna gerekçelendirilmeli | kırmızı |
| 7 | Log çağrısında sabit şablon + alan adı var mı? | ADR-026 geçti | kırmızı (ADR-026) |
| 8 | SQL'de `+` ile birleştirme var mı? | otomatik RED (ADR-017) | kabul |
| 9 | `!` (null-forgiving) savsaklama olarak mı kullanılmış? | kırmızı, `?.`/`??` iste | kabul |
| 10 | Bu değişiklik iki bacakta da testten geçti mi? | kanıt eklenebilir | "bitti" denemez (ADR-019) |

**Uygulama sırası:** sorular 1–5 mimariye, 6–9 koda, 10 kanıta aittir; 1–5'ten biri "hayır" ise kod incelemesine devam edilmez, iş §1.1'deki ilgili ADR satırına döner.

## §4 Sert Duruşlar (Otomatik RED)

| # | Duruş | Ne yapılır | ADR |
|---|-------|-----------|-----|
| 1 | EF Core / ORM referansı | RED — kod durur, veri erişimi ADO.NET + Dapper'a çevrilir | ADR-005, ADR-016 |
| 2 | Wireframe onaysız Form kodu | DUR — kapı kapalı, önce GATE-003 onayı | ADR-025 |
| 3 | Tek TFM'de yeşil test ile "bitti" | RED — ikinci bacak çalıştırılır, kanıt yoksa teslim sayılmaz | ADR-019 |
| 4 | Elle sayım/uydurma sayı | rapora yazılmaz; tek kanıt `count-components.ps1` (→ 5440 kayıt, 52 düğüm dosyası) | ADR-027 |
| 5 | Başka vault'a / `%TEMP%` referans ağacına yazma | RED — salt okunur; yalnız yapı referans alınır | ADR-023, ADR-024 |
| 6 | Frontmatter eksik dosya | RED — vault'a ait değildir, üretime alınmaz | ADR-003 |

### §4.1 Otomatik RED Tablosu

| Desen | Bulunduğu yer | Karar | ADR |
|-------|---------------|-------|-----|
| `using Microsoft.EntityFrameworkCore` | herhangi bir .cs | RED | 005/016 |
| `"SELECT ..." + değişken` | SQL çağıran her katman | RED (G2) | 017 |
| `logger.LogInformation("serbest metin")` | log satırı | RED (format) | 026 |
| Tek TFM'de yeşil test ile "bitti" | PR/commit öncesi | RED (kanıt eksik) | 019 |
| Elle yazılmış bileşen sayısı | rapor/özet | RED (kanıtsız iddia) | 027 |
| Onaysız `.Form.Designer.cs` | Desktop (l7) | DUR (kapı kapalı) | 025 |
| Koda/repoda API anahtarı | herhangi bir dosya | RED | 018 |
| İki dosyada aynı liste | vault .md | RED (SRP/tekrar) | 021 |

### §4.2 Yasaklar Kara Listesi (neden + yakalama)

§4.1 desen tablosu **yeri** gösterir; bu tablo **neden yasak** olduğunu ve ilk bakışta nasıl yakalanacağını yazar.

| # | Yasak | Neden | Yakalama (desen / komut) | Kaynak |
|---|-------|-------|--------------------------|--------|
| 1 | **EF Core** (ve herhangi bir ORM) | net48 bacağında sürüm kilidi, görünmeyen SQL, migration'ın denetim dışı kalması | `.cs` içinde `using Microsoft.EntityFrameworkCore` → RED; ayrıca `data-access-check` | ADR-005, ADR-016 |
| 2 | **String-concat SQL** | SQL enjeksiyonu; birleştirme hatası çalışma zamanında değil kodda yakalanmalı | `"SELECT` ... `+` deseni → RED (G2); `sql-injection-review` | ADR-017, ADR-005 |
| 3 | **DevExpress sürüm/stil sapması** | farklı sürüm `.resx`/`.Designer.cs`'i bozar, ribbon düzeni kayar; 26.1 dışındaki her sürüm RED | paket satırında `DevExpress` sürümü ≠ 26.1 veya `RibbonStyle` ≠ `Office2010` → RED; `ribbon-style-check` | ADR-004 |
| 4 | **WPF / MAUI / web** | WinForms (DevExpress v26.1) tek hedef; ikinci UI teknolojisi çekirdeği böler ve iki kez iş demektir | `.csproj` içinde `UseWPF` / `UseMaui` / `Microsoft.AspNetCore` → RED | ADR-018, ADR-010 |
| 5 | **Unicode kutu çizim karakterleri** | arama, diff ve kopyala-yapıştırmada bozulur; ölçülebilir ASCII çıktısı gerekir | `U+2500..U+257F` aralığı → RED; ASCII `+ - \|` ile çizilir. Kapsam farkı **ADR-029 ile kapandı** (2026-10-01): ASCII-only karar, ADR-020 D1'i supersede etti | AGENTS §2, yeni-dosya sözleşmesi, ADR-029 |
| 6 | **Elle yazılmış bileşen sayısı** | sayı uydurmaya açıktır; kapı değeri gerçeği değil yazılanı ölçer | raporda elle sayı → RED; tek kanıt `count-components.ps1` | ADR-027 |

### §4.3 Açık Konular (DOĞRULAMA GEREKLİ)

1. **ADR-028 yön tutarsızlığı:** matris `l4→l2 X`, `l5→l4 X` derken §2.1 izinli yazıyor — düzeltme 2 ile tek yöne sabitlenecek (`PROJECTS.md` §3).
2. **`Result<T>` tipinin kesin konumu:** l0 `result-error` (N02) ile l5 Crosscutting ifadesi çelişir; kod yazılırken sabitlenecek.
3. **CI/CD yok:** kapılar local (script + checklist); CI'ya taşınma tarihi belirsiz (`PROJECTS.md` §5).
4. **K3 99 kayıtsız dosya:** katalog kapsamı dışı; üretim ağacına taşınmayacak, kaydı ADR-028 §C1'de.
5. **Semantik atlas güncellemesi:** S01–S07 bayatlarsa [layer-boundary-check](../checklists/layer-boundary-check.md) tek kurtarıcı (risk R4).

### §4.4 Red Team — En Sık Yapılan 8 Hata

§4.1 deseni, §4.2 yasağı, bu tablo ise **insanın gerçekten neyi yanlış yaptığını** ve doğrusunu yazar. Her satır bu vault'ta beklenen bir hatadır; "olmaz" denip geçilmez, yakalanma yolu yazılır.

| # | Sık yapılan hata | Neden yanlış | Doğrusu | Yakalama |
|---|------------------|--------------|---------|----------|
| 1 | "EF Core daha hızlı olurdu" diye ek paket koymak | iki bacakta sürüm kilidi + görünmeyey SQL | ADO.NET + Dapper, sorgu elle yazılır | `data-access-check` + EF `using` taraması |
| 2 | Testi tek bacakta geçirip "bitti" demek | kanıtın yarısı üretilmemiştir | iki bacakta `dotnet test`, ikisi de 0 başarısız | `dual-tfm-build-check` |
| 3 | Yeni kodu "en yakın katmana" (l4) yazmak | yön matrisi çöker, l4 şişer | önce "hangi katman?" sorusu, port `l0`'a | `layer-boundary-check` 13 soru |
| 4 | `.Designer.cs`'i elle düzenlemek | kaynak ürün bozulur, tasarımcıda açılmaz | tasarım sözleşmesi → `ui-designer` → onay → kod | `winforms-designer-check`, GATE-003 |
| 5 | Log'a serbest metin yazmak | iki bacakta alan adları ayrışır, çözümleme çalışmaz | sabit şablon + 5 seviye | `logging-observability-check` |
| 6 | Anahtarı `.env`/koda yazmak | depo sızarsa erişim sızar | DPAPI şifreli depo, `keys.md` yalnız harita | `secret-leak-check` |
| 7 | Katalog "Durum" sütununu elle yeşile almak | kanıt değil iddia üretir | kodla eşleşme ölçülür, sayı script'ten gelir | `count-components.ps1` + semantik denetim |
| 8 | Bozuk link varken commit atmak | vault'un yarısı gezilemez hale gelir | önce 0 bozuk link, sonra commit | `check-links.ps1` + GATE-004 |

**Ortak örüntü:** sekiz hatanın yedisi "kanıt yerine iddia" üretir; bu yüzden vault'un "kanıtsız iddia işlenmez" kuralı (G5) tek başına çoğu hatayı yakalar.

## §5 İlgili Dosyalar

| # | Dosya | Ne için |
|---|-------|---------|
| 1 | `.ai/.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | güncel baz karar (9 proje, 52 düğüm) |
| 2 | `.ai/.decisions/adr-index.md` | 29 ADR'nin durum defteri (SSOT) |
| 3 | `.ai/VISION.md` | 7 ekran ve farklılaştırıcılar |
| 4 | `.ai/PROJECTS.md` | proje envanteri, dış bağımlılık, risk defteri |
| 5 | `.ai/keys.md` | anahtar kelime → dosya hız haritası |
| 6 | `.ai/glossary.md` | terim sözlüğü |
| 7 | `checklists/layer-boundary-check.md` | katman sınırı 13 sorusu |
| 8 | `.ai/.rules/solid-rules.md` | SOLID kurallarının ayrıntılı metni |
| 9 | `.ai/CLAUDE.md` · `.ai/WORKFLOW.md` · `.ai/MEMORY.md` · `.ai/index.md` | anayasa ve akış |
| 10 | Prompt kaynağı (mimari karar felsefesi, boundary) | `G:/…/versacoder proje planlaması ai.08.md` |
| index | [index](../index.md) | vault navigasyonu — harita, hizli yol tarifleri, kalite sayaclari |
| CLAUDE | [CLAUDE](../claude.md) | AI anayasa — G1..G12 hard guardrail, kanit formati, dur/bekle |
| AGENTS | [AGENTS](../definitions/agents.md) | ajan matrisi — Chat!=Agent!=LLM, cikti kutusu, 3-STRIKE, gorsel uretim hatti |
| WORKFLOW | [WORKFLOW](../governance/workflow.md) | gate zinciri — GATE-004a..006, kabul kriteri, kanit kutusu |
| ROLE | [ROLE](../definitions/role.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](../definitions/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| VISION | [VISION](../definitions/vision.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](../definitions/projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](../definitions/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| keys | [keys](../governance/keys.md) | anahtar kelime -> hedef dosya hiz haritasi (101 satir) |
| MEMORY | [MEMORY](../memory/memory.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [log](../ops/log.md) | append-only is gunlugu + Quality Report |
| ULTRA | [ULTRA](../definitions/ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok CLAUDE | [kok CLAUDE](../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
| kok README | [kok README](../../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
