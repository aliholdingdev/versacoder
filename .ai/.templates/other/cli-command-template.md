---
title: "VersaCoder — CLI Komut Şablonu (CLI Command)"
type: template
category: templates/other
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "cli komut şablonu: arg parsing, exit code, stdout/stderr, iptal, test ve %90 coverage kanıtı"
---

# VersaCoder — CLI Komut Şablonu (CLI Command)

**Zorunlu Bağlantılar:** [index](../index.md) · [winforms-form-template](../other/winforms-form-template.md) · [handler-template](../backend/handler-template.md) · [service-template](../backend/service-template.md) · [layer-spec-template](../architecture/layer-spec-template.md) · [unit-test-template](../testing/unit-test-template.md) · [test-plan-template](../testing/test-plan-template.md) · [CLAUDE](../../../CLAUDE.md)

## §1 Amaç

### §1.1 Bu Şablon ne Üretir?

**`src/VersaCoder.Cli/**` altındaki tek bir CLI komutu**: `l7` yüzeyinden çağrılan, argümanları
ayıklayan, işi **`l2` handler'a** devreden, sonucu `stdout`'a yazan ve **çıkış kodu** üreten bir
komut sınıfı. CLI, veritabanına ve domaine **dokunmaz**; yalnız `l6` composition root'tan aldığı
arayüzlerle konuşur. Bu şablon komutun **sözleşmesini** kilitler: hangi argüman var, hangi durumda
hangi çıkış kodu dönüyor, hata nereye yazılıyor, Ctrl+C ne yapıyor ve **neyin test edilmediği**.

```text
+----------------------------------------------------------------+
|        VERSACODER - CLI KOMUT SABLONU - l7 YUZEY               |
+----------------------------------------------------------------+
|  Girdi  : argv (arguman + secenek) + IServiceProvider          |
|  Cikis  : stdout (sonuc) / stderr (hata) + exit code           |
|  Kural  : parse once, is sonra, hata stderr'ye, kod asla 0     |
|  Kanim  : komut ince + test kalin (mantik ayri serviste)       |
+----------------------------------------------------------------+
```

Bu dosya **kod iskeleti değil, komut sözleşmesidir**: komutun adı, parametrelerin sırası ve
tipi, kullanım hatasının biçimi, beş çıkış kodu, `--json` davranışı, iptal (Ctrl+C) akışı ve
mantığın nereye taşındığı burada kilitlenir. Komut sınıfı **ince** kalır; iş mantığı ayrı
serviste durur ki `%90` coverage alınabilsin.

| Öğe | Değer |
|------|-------|
| Şablon adı | `cli-command-template.md` |
| Şablon yolu | `.ai/.templates/other/cli-command-template.md` |
| Hedef dosya | `src/VersaCoder.Cli/Commands/**/*.cs` |
| Hedef kod | C# 12 · `System.CommandLine` (veya el yapımı parser) · .NET 10 |
| Proje | `VersaCoder.Cli` — **`l7` CLI**, katman **DEĞİŞTİRİLMEZ** (küçük `CLAUDE.md` B5) |
| Katman erişimi | yalnız `l2` handler + `l0` port (**doğrudan SQL yok**) |
| Çıkış kodları | `0` başarılı · `1` iş hatası · `2` kullanım hatası · `3` bağımlılık hatası · `130` iptal |
| Kodlama | UTF-8 (BOM'suz) · Türkçe · mojibake yasak (G5) |
| Öğretilen uzunluk | 120–280 satır (kod) + bu şablonun asgari 480 satırı (G6) |
| Sürüm / Authority | `1.0.0` · `SSOT` |

### §1.2 Neden Ayrı Bir Şablon? (Ölçülmüş Disk Kanıtı)

CLI, formdan farklı bir **konuşma sözleşmesi** taşır: DOM yok, olay yok; onun yerine `argv`,
exit code ve `stderr` vardır. WinForms [form-template](../other/winforms-form-template.md)
şablonu `DialogResult` ile biter; bu şablon `Environment.ExitCode` ile biter. Aşağıdaki satırlar
bu ortamda **2026-10-05** tarihinde ölçülmüştür:

| # | Ölçüm | Değer | Etkisi |
|---|-------|-------|--------|
| 1 | `Test-Path src/` | `False` | CLI ağacı henüz yok; şablon **hedef yol sözü** verir |
| 2 | `Test-Path tests/` | `False` | Test projesi yok; test şablonu ile eşleştirilir |
| 3 | `Get-ChildItem -Filter *.sln` | `0` | Çözüm yok; komut tek başına `dotnet build` ile doğrulanır |
| 4 | `Get-ChildItem -Filter global.json` | `0` | SDK sabitlenmemiş; **10.0.301** makine genelinde geçerli |
| 5 | `dotnet --version` | `10.0.301` | `net10.0` + C# 12 varsayımı geçerli |
| 6 | `.ai/.templates` içindeki `.md` | `51` | Bu şablon `other/` klasörünün **2.** üyesidir |
| 7 | `.ai/.templates/other/*.md` | `1` → bu dosya ile `2` | `winforms-form-template.md` kardeştir |
| 8 | `PATH` üzerinde `versacoder` | yok | Komut adı **sözleşmedir**; henüz kurulu değil |

**Sonuç:** bu ortamda CLI komutu **henüz yazılmamıştır**. Şablon, ilk komut yazılırken uyulacak
sözleşmeyi, `GATE-003` öncesi `src/**` kapısını ve test kanıtını tek yerde toplar.

### §1.3 Şablon Kimin İçin?

| Rol | Bu şablondan ne alır? |
|-----|------------------------|
| Uygulama geliştirici (`developer`) | Komut sınıfı sözleşmesini, çıkış kodu tablosunu, iptal akışını |
| Test yazarı (`testing`) | Ayrılabilir sınırı (parser vs servis), 5 satırlık test kimliğini |
| Kod denetçisi (`reviewer`) | §4.5 G-tablosunu; `Console.WriteLine` yerine `stdout`/`stderr` ayrımını |
| DevOps / SRE | `--json` çıktısını ve exit code yorumunu (otomasyon için) |
| Ürün sahibi (`product-manager`) | §2.2 kapsamını; hangi davranışın sözleşmeye girdiğini |

### §1.4 Bu Şablon Hangi Soruyu Cevaplamaz?

- **Uç nokta / HTTP sözleşmesi** — [api-doc-template](../documentation/api-doc-template.md) esastır.
- **Veritabanı erişimi** — [repository-template](../backend/repository-template.md) + [Query-Template](../query/Query-Template.md) esastır.
- **Masastı ekranı** — [winforms-form-template](../other/winforms-form-template.md) esastır.
- **Web formu** — [form-template](../frontend/form-template.md) esastır.
- **Mimari karar** (neden CLI gerekli) — [adr-nygard-template](../adr/adr-nygard-template.md) ile ADR yazılır.
- **Yayın / paketleme** — [WORKFLOW](../../../WORKFLOW.md) §6 gate'leri esastır.

## §2 Kapsam

### §2.1 Kapsam Dışı (Bu Şablonla Üretilmeyen)

| # | Kapsam dışı | Nereye gider? |
|---|-------------|----------------|
| 1 | Yeni katman tanımı (`l0..l7`) | [layer-spec-template](../architecture/layer-spec-template.md) |
| 2 | DI kaydı ve composition root | `l6` Host — [configuration-template](../infrastructure/configuration-template.md) |
| 3 | Doğrudan SQL / Dapper sorgusu | [repository-template](../backend/repository-template.md) |
| 4 | İş kuralı ve handler gövdesi | [handler-template](../backend/handler-template.md) |
| 5 | HTTP/gRPC uç noktası | [service-template](../backend/service-template.md) |
| 6 | GUI formu ve olay döngüsü | [winforms-form-template](../other/winforms-form-template.md) |
| 7 | CI/CD ve paket dağıtımı | [ci-pipeline](../../../WORKFLOW.md) |
| 8 | Test stratejisi ve gate tanımı | [test-plan-template](../testing/test-plan-template.md) |
| 9 | Komutun görsel terminal teması | [wireframe-template](../ui-design/wireframe-template.md) |
| 10 | Sürüm notu / CHANGELOG | [readme-template](../documentation/readme-template.md) |

### §2.2 Kapsam İçi (Bu Şablonla Kilitlenen)

| # | Kapsam içi | Kanıt |
|---|------------|-------|
| 1 | Komut adı, alt komut ve dosya yolu | §3.1 |
| 2 | Argüman/seçenek listesi, tip, varsayılan | §3.2 |
| 3 | Kullanım hatası biçimi ve `stderr` mesajı | §3.3 |
| 4 | Beş çıkış kodu (`0/1/2/3/130`) | §3.4 |
| 5 | `stdout`/`stderr` ayrımı ve `--json` | §3.5 |
| 6 | Ctrl+C iptal akışı (`CancellationToken`) | §3.4 |
| 7 | Mantığın ayrı serviste olması (komut ince) | §3.8 |
| 8 | 5 satırlık test kimliği + `%90` coverage | §3.9 |
| 9 | 8+ hata modu, salt-okunur denetimler, Red Team | §5.2 · §5.3 · §7.2 |

```text
+----------------------------------------------------------------+
|                     KAPSAM SINIRI                              |
+----------------------------------------------------------------+
|  BU SABLON                          BU SABLON DEGIL            |
|  ------------                       ----------------           |
|  argv parse + exit code             Dapper sorgusu             |
|  stdout / stderr ayrimi             DI registration            |
|  Ctrl+C -> CancellationToken        HTTP endpoint              |
|  --json ciktisi                     GUI form + olay dongusu    |
|  komut ince, mantik serviste        CI pipeline                |
+----------------------------------------------------------------+
```
## §3 Mimari

### §3.1 Komut Sınıfı Sözleşmesi

```csharp
// src/VersaCoder.Cli/Commands/CustomerExportCommand.cs
namespace VersaCoder.Cli.Commands;

/// <summary>customer export — l7 yüzeyi; işi IExportHandler'a devreder.</summary>
public sealed class CustomerExportCommand
{
    private readonly IExportHandler _handler;
    private readonly TextWriter _out;
    private readonly TextWriter _err;

    public CustomerExportCommand(IExportHandler handler, TextWriter @out, TextWriter err)
    {
        _handler = handler;
        _out = @out;
        _err = err;
    }

    public async Task<int> ExecuteAsync(string[] args, CancellationToken ct)
    {
        var parsed = Parser.Parse(args);          // hata -> UsageError
        var dto    = MapToRequest(parsed);        // yalniz DTO
        var result = await _handler.RunAsync(dto, ct);
        return Reporter.Write(result, _out, _err, parsed.Json);
    }
}
```

**Kurallar:**

1. Komut adı = `{Alan}{Eylem}Command`, dosya = `{Alan}{Eylem}Command.cs`.
2. `ExecuteAsync(string[] args, CancellationToken ct)` **tek** giriş noktasıdır (test edilebilir).
3. `Main` yalnız: DI kurar, `CancellationTokenSource` bağlar, dönen kodu `Environment.ExitCode` yazar.
4. Komut sınıfı **SQL, repo, domain nesnesi görmez**; yalnız DTO ve arayüz görür.
5. `Console.*` **yasağdır**; `_out` / `_err` `TextWriter` enjeksiyonuyla yazılır (test edilebilirlik).

### §3.2 Argüman ve Seçenek Listesi

| # | Ad | Tip | Zorunlu | Varsayılan | Açıklama |
|---|----|-----|---------|------------|----------|
| 1 | `command` | alt komut | Evet | — | `export`, `import`, `status` |
| 2 | `--from` | `DateOnly` | Evet | — | ISO `yyyy-MM-dd` |
| 3 | `--to` | `DateOnly` | Evet | — | `≥ --from` |
| 4 | `--format` | `enum` | Hayır | `table` | `table`, `csv`, `json` |
| 5 | `--out` | yol | Hayır | `stdout` | Dosya yoksa stdout |
| 6 | `--json` | `bool` | Hayır | `false` | stdout'u tek JSON nesnesi yapar |
| 7 | `--dry-run` | `bool` | Hayır | `false` | Yazma yapmaz, yalnız plan |
| 8 | `-v` / `--verbose` | `bool` | Hayır | `false` | Ayrıntı → **stderr** |

**Tip ve varsayılan kuralları:** tarih `DateOnly` (saat yok), sayı `int`/`decimal` (asla `string`),
seçim `enum` (geçersiz değer → çıkış `2`). `--json` ile `--format` **çatışırsa** `--json` kazanır
ve bu tercih §3.5'te yazılıdır.

### §3.3 Ayrıştırma ve Kullanım Hatası

```text
+----------------------------------------------------------------+
|            PARSE SIRASI (4 ADIM) + HATA KAPISI                 |
+----------------------------------------------------------------+
|  1 alt komut      -> yok/kotu        -> exit 2 + kullanim      |
|  2 zorunlu alan   -> eksik/yanlis    -> exit 2 + eksik alan    |
|  3 secenek tipi   -> donusum hatasi  -> exit 2 + tip mesaji    |
|  4 hepsi gecti    -> handler cagrisi -> exit 0 / 1 / 3 / 130   |
+----------------------------------------------------------------+
```

1. Kullanım hatası (`2`) **hiçbir zaman** iş hatası (`1`) sayılmaz; işe başlanmaz.
2. Hata mesajı **tek satır**, Türkçe, **`stderr`**'ye yazılır; `stdout` temiz kalır.
3. Hata mesajı **kullanımı da öğretir**: `Geçersiz tarih: 'yarin'. Beklenen yyyy-MM-dd.`
4. `--help` → `stdout`, çıkış `0`. Kullanım hatasında `--help` özeti `stderr`'ye eklenir.
5. Bilinmeyen seçenek **sessizce yok sayılmaz**; her zaman `2` döner.

### §3.4 Çıkış Kodları ve Ctrl+C

| Kod | Anlam | Ne zaman | stderr örneği |
|-----|-------|----------|----------------|
| `0` | Başarılı | İş bitti, sonuç yazıldı | (boş) |
| `1` | İş hatası | Handler `false`/hata döndü | `Kayıt bulunamadı: 42.` |
| `2` | Kullanım hatası | Argüman eksik/yanlış | `--from zorunlu (yyyy-MM-dd).` |
| `3` | Bağımlılık hatası | DB/AG erişilemedi, timeout | `Veritabanına ulaşılamadı.` |
| `130` | İptal (SIGINT) | Ctrl+C — `128 + 2` | `İptal edildi.` |

```text
+----------------------------------------------------------------+
|                  EXIT CODE SÖZLEŞMESI (5)                      |
+----------------------------------------------------------------+
|  0    basarili           -> stdout gecerli sonuc               |
|  1    is hatasi          -> is bitti ama sonuc yok             |
|  2    kullanim hatasi    -> argv kusuru, is baslamadi          |
|  3    bagimlilik hatasi  -> db/ag/timeout, tekrar denenebilir  |
|  130  iptal (ctrl+c)     -> 128+2, yarim is guvenli birakilir  |
+----------------------------------------------------------------+
```

**Ctrl+C akışı:** `Console.CancelKeyPress` → `e.Cancel = true` → `_cts.Cancel()` → handler
`OperationCanceledException` fırlatır → komut **kaldığı yerden güvenli çıkar** (tekrar yazılmış
satırlar yok) → çıkış `130`. İkinci Ctrl+C'de **zorla** çıkılır (`_cts.CancelAfter(TimeSpan.Zero)`).

### §3.5 stdout / stderr Ayrımı ve `--json`

| Akış | Ne gider | Ne gitmez |
|------|----------|-----------|
| `stdout` | İstenen sonuç (tablo, CSV, tek JSON) | Uyarı, hata, ilerleme |
| `stderr` | Hata, uyarı, `--verbose` günlüğü | Sonuç verisi |

- `--json` açıkken `stdout` **tek** JSON nesnesidir (biçimli, tek satır sonu ile); başka hiçbir
  şey `stdout`'a yazılmaz — boru (`|`) ile tüketim böyle güvence altına alınır.
- İnsan-okunur çıktı **tablo hizalıdır**; sütun başlıkları Türkçe.
- Hata durumunda `--json` → `stderr`'ye `{ "error": "...", "code": 2 }` biçiminde yazılır.
- Bellek akışı: sonuç **satır satır** yazılır (`await foreach`), tamamı bellekte tutulmaz.

### §3.6 Dosya ve Proje Düzeni

```text
src/VersaCoder.Cli/                                               
├── Program.cs                     (l6 DI + cikti kodu; ince)     
├── Commands/                                                     
│   ├── CustomerExportCommand.cs   (bu sablon)                    
│   └── CustomerExportCommand.Tests.cs?                           
├── Parsing/                                                      
│   └── Parser.cs                  (argv -> ParsedArgs)           
├── Reporting/                                                    
│   └── Reporter.cs                (sonuc -> stdout/stderr)       
└── VersaCoder.Cli.csproj                                         
tests/VersaCoder.Tests/                                           
└── Commands/CustomerExportCommandTests.cs   (§3.9)               
```

### §3.7 Kalite Sözleşmesi (Şablon Düzeyi)

| Metrik | Hedef | Kanıt |
|--------|-------|-------|
| Test coverage | **%90** satır (G11) | `dotnet test --collect:"XPlat Code Coverage"` |
| Uyarılar | `TreatWarningsAsErrors=true` (G11) | `dotnet build` çıkış kodu `0` |
| Kod formatı | `dotnet format --verify-no-changes` | çıkış kodu `0` |
| Mojibake | `0` | §5.3 komut 5 |
| Kırık link | `0` | §5.3 komut 7 |
| ASCII kutu genişliği | `66` karakter | §5.3 komut 8 |

### §3.8 Bağımlılık Kuralları

1. `VersaCoder.Cli` → `VersaCoder.Application` (`l2`) **+** `VersaCoder.Abstractions` (`l0`) + `l3`.
2. `VersaCoder.Cli` → `VersaCoder.Infrastructure` (**yasak** — dolaylı, `l6` üzerinden gelir).
3. Komut → **arayüz**; komut → somut repo/domain (**yasak**).
4. Parser ve Reporter **saf** sınıflardır (bağımlılık almaz) → kolay test.
5. Üçüncü taraf: yalnız `System.CommandLine` (isteğe bağlı); `Moq`/`NSubstitute` yalnız `tests/**`.

### §3.9 Test Kimliği (5 Satır Zorunlu)

Her komut test metodu **tam beş satır** `///` ile başlar; başka yorum biçimi yoktur (G5):

```csharp
/// <Purpose>Geçerli argümanla handler çağrılır ve çıkış kodu 0 döner.</Purpose>
/// <Input>args = ["export","--from","2026-01-01","--to","2026-01-31","--json"]</Input>
/// <Expected Result>Handler bir kez çağrılır; stdout tek JSON; exit 0.</Expected Result>
/// <Failure Criteria>Exit 1/2/3 döner veya stdout'a ikinci nesne yazılır.</Failure Criteria>
/// <Evidence>dotnet test --filter CustomerExport_Valid_ReturnsZero</Evidence>
```

**İsimlendirme:** dosya/sınıf `{KomutAdı}Tests`; metot `{Komut}_{Durum}_{BeklenenKod}` —
ör. `CustomerExport_Valid_ReturnsZero`, `CustomerExport_MissingFrom_ReturnsTwo`,
`CustomerExport_Canceled_Returns130`.

**Test grubu ve oran hedefi:**

| # | Test grubu | Örnek metot | Pay |
|---|------------|-------------|-----|
| 1 | Parse + kullanım hatası | `CustomerExport_MissingFrom_ReturnsTwo` | %30 |
| 2 | Başarılı akış (exit `0`) | `CustomerExport_Valid_ReturnsZero` | %25 |
| 3 | İş/bağımlılık hatası | `CustomerExport_NoDb_ReturnsThree` | %20 |
| 4 | İptal (Ctrl+C) | `CustomerExport_Canceled_Returns130` | %15 |
| 5 | `--json` biçimi | `CustomerExport_Json_WritesSingleObject` | %10 |

### §3.10 Sınır: Ne Test Edilmez?

| Test edilmez | Neden |
|--------------|-------|
| `Main` gövdesi ve `Environment.ExitCode` | Süreç kenarı; yalnız `ExecuteAsync` test edilir |
| Gerçek terminal penceresi / renk | Görsel; `TextWriter` sahtesi yeterli |
| Gerçek `Console.CancelKeyPress` | OS olayı; `CancellationTokenSource` ile taklit edilir |
| Gerçek veritabanı | `l2` **sahte** ile; entegrasyon testi ayrı şablon |
| `System.CommandLine` iç ayrıştırma | 3. taraf; yalnız **bizim** seçimlerimiz test edilir |
| Performans (ms) | Ölçüm işi; §6.5 ölçüm tablosuna ait |

### §3.11 Komut Yardımı (`--help`) Şözleşmesi

`--help` **zorunlu** ve `stdout`'a, çıkış `0` ile yazılır. Dört başlık sabittir:
**Kullanım** (tek satır özet), **Seçenekler** (§3.2 tablosu ile aynı adlar), **Çıkış kodları**
(§3.4 beş satır), **Örnekler** (≥2 gerçek örnek). Yardım metni `{{BIRIM_ADI}}` gibi şablon
değişkeni içeremez — üretilen çıktıda `{{...}}` **yasaktır** (§4.1 R5).

## §4 Kurallar

### §4.1 Zorunlu (Bu Şablonda Asla Aşılmaz)

| # | Kural | İhlal Sonucu |
|---|-------|--------------|
| R1 | Komut asla `l4`/SQL çağırmaz | G8 ihlali — PR reddi |
| R2 | Kullanım hatası = `2`, iş hatası = `1` (karıştırılamaz) | Otomasyon yanlış yorumlar |
| R3 | Sonuç `stdout`, hata `stderr` | Boru (`\|`) kirlenir |
| R4 | Türkçe kaynak; mojibake `0` | G5 ihlali |
| R5 | `{{PLACEHOLDER}}` yalnız şablonda, üretilen çıktıda geçmez | Çıktı bozuk |
| R6 | Test kimliği 5 satır `///` | Test reddi (§3.9) |
| R7 | Coverage **%90**, uyarılar **hata** (G11) | CI kapalı |
| R8 | Ctrl+C → `130`; yarı yazılmış çıktı yok | Veri bozukluğu |

### §4.2 Yapılmayan (Bilinçli Sınır)

- Komut sınıfında **hiçbir** iş kuralı hesabı yapılmaz (o `l1`/`l2` işidir).
- `Console.WriteLine` **hiç kullanılmaz**; her yazım `TextWriter` üzerinden yapılır.
- Çıktı **tamponlanıp tek seferde** yazılmaz; akış (`await foreach`) tercih edilir.
- Komut kendi kendine **process başlatmaz**, `Environment.Exit` **çağırmaz** (yalnız `Program.cs`).
- Bilinmeyen seçenek **sessizce yutulmaz**.

### §4.3 Yapılacak (Minimum)

1. `--help` çıktısı dört sabit başlık (§3.11).
2. Her hata mesajı **tek satır** + beklenen biçim bilgisi.
3. `--verbose` günlüğü yalnız `stderr` ve `[saat] düzey mesaj` biçiminde.
4. İptalde **tekrar yazım yok**; dosya hedefi varsa **geçici dosyadan** atomik taşıma.
5. `ExecuteAsync` **döner** (`Task<int>`); asla process'i öldürmez.

### §4.4 G1..G12 Eşlemesi

| G | Bu şablondaki karşılığı | Kanıt |
|---|-------------------------|-------|
| G1 | CoreMusic **kopyalanmaz**; CLI deseni buradan yazılır | §3.1 kodu özgün |
| G2 | EF Core **yasak**; komut yalnız `l2` çağırır | §3.5/§3.8 |
| G3 | Türkçe karakter `VARCHAR(260)` sınırı **girdide** kontrol edilir | §3.2 + handler |
| G4 | Somut **değil**, arayüz üzerinden DI | `IExportHandler` |
| G5 | UTF-8 BOM'suz, Türkçe, `///` yorum biçimi | §5.3 komut 5–6 |
| G6 | Şablon ≥480 satır, komut kodu 120–280 satır | §6.5 |
| G7 | Ürün adı **VersaCoder**; `CoreMusic` geçmez | §6.1 madde 29 |
| G8 | `src/**` yalnız `GATE-003` sonrası | WORKFLOW §4 |
| G9 | `l7` katmanı **DEĞİŞTİRİLMEZ** | §3.5/§3.8 |
| G10 | Parametreleme `@p1..@pN` — komut bunu **görmez**, `l4` görür | §3.8 |
| G11 | `%90` coverage + `TreatWarningsAsErrors=true` | §3.7 |
| G12 | Anahtar kural metni kök `CLAUDE.md`'de | [CLAUDE](../../../CLAUDE.md) |

### §4.5 G1..G12 Tam Tablo (Bu Şablonun Kırılma Noktaları)

| G | Kural | Kırılırsa | Yakalayan denetim |
|---|-------|-----------|-------------------|
| G1 | CoreMusic'ten kopya kod yasak | Telif + yanlış desen | Kod incelemesi, §6.1 m.29 |
| G2 | EF Core yasak (ADO.NET + Dapper) | Bağımlılık sızıntısı | `Select-String -Path *.csproj -Pattern 'EntityFramework'` |
| G3 | Türkçe karakter `VARCHAR(260)` sınırı | Kayıp veri | Handler testi, §6.1 m.8 |
| G4 | Somut sınıf değil arayüz | Test edilemezlik | `Select-String 'new ExportRepository' src/VersaCoder.Cli` |
| G5 | UTF-8 BOM'suz + mojibake `0` | Bozuk metin | §5.3 komut 5–6 |
| G6 | Şablon ≥480 satır | Eksik öğe | §6.5 ölçüm |
| G7 | Ürün adı VersaCoder | Marka hatası | `Select-String 'CoreMusic'` |
| G8 | `GATE-003` öncesi `src/**` yazılmaz | Kontrolsüz değişiklik | WORKFLOW §4 gate |
| G9 | `l7` katmanı değiştirilmez | Mimari kayma | `.csproj` + namespace denetimi |
| G10 | `@p1..@pN`; `SELECT *` yasak | SQL injection riski | Repo kod incelemesi |
| G11 | `%90` coverage + uyarı hata | Kalite düşüşü | `dotnet test` + `.editorconfig` |
| G12 | SSOT kök `CLAUDE.md` | Kural dağılması | Bağlantı denetimi §6.1 m.22 |

```text
+----------------------------------------------------------------+
|                G-KAPISI - 12 KURAL - 12 KANIT                  |
+----------------------------------------------------------------+
|  G1  kod kopyasi yok          |  G7  VersaCoder adi            |
|  G2  EF Core yok              |  G8  GATE-003 onayi            |
|  G3  VARCHAR(260) siniri      |  G9  l7 katmani degismez       |
|  G4  arayuz + DI              |  G10 @p1..@pN + SELECT * yok   |
|  G5  UTF-8 BOMsuz             |  G11 %90 + warnings=errors     |
|  G6  >=480 satir              |  G12 SSOT = koku CLAUDE.md     |
+----------------------------------------------------------------+
```
## §5 Workflow

### §5.1 Adım Adım Üretim Akışı

| Adım | Eylem | Çıktı | Kapı |
|------|-------|-------|------|
| 1 | [index](../index.md) + bu şablonu oku | Sözleşme | — |
| 2 | [handler-template](../backend/handler-template.md) ile iş sözleşmesini yaz | `IExportHandler` | — |
| 3 | `GATE-003` onayı | Yazma izni | **HARD STOP** |
| 4 | `Parser` + `Reporter` (saf sınıflar) | §3.3 / §3.5 | — |
| 5 | Komut sınıfı + `Program.cs` (DI, iptal) | §3.1 / §3.4 | — |
| 6 | Test dosyasını yaz (5 satır `///`) | `%90` | G11 |
| 7 | §5.3 denetimlerini çalıştır | 8/8 | — |
| 8 | §6.1 ≥17 maddeyi işaretle | Red hattı | §6.2 |
| 9 | [test-plan](../testing/test-plan-template.md) + [unit-test](../testing/unit-test-template.md) ile hizala | Rapor | GATE-004 |
| 10 | [katman-readme](../documentation/katman-readme-template.md) ile `l7` satırını güncelle | Doküman | GATE-006 |

### §5.2 Hata Modları (≥8) ve Küçültme

| # | Hata modu | Belirti | Düzeltme | Küçültme |
|---|-----------|---------|----------|----------|
| 1 | Kullanım hatası `1` dönüyor | Otomasyon yanlış yorumluyor | Parse hatası **hep `2`** | Tek `UsageError` tipi |
| 2 | Hata `stdout`'a yazıldı | Boru kirlendi | `stderr` zorunlu | Tek `Reporter.WriteError` |
| 3 | Çift `--json` çıktısı | Geçersiz JSON | Tek nesne + tek yazım | `--json` bayrağı tek noktada |
| 4 | Ctrl+C sonra dosya bozuk | Yarım çıktı | Geçici dosya + atomik taşıma | Tek `WriteAtomicAsync` |
| 5 | Bilinmeyen seçenek yutuldu | Sessiz yanlış sonuç | Hep `2` döner | `Parser` tek kapı |
| 6 | Mojibake (U+FFFD bozuk harf) | Türkçe bozuk | UTF-8 **BOM'suz** kaydet | §5.3 komut 5 |
| 7 | Kırık iç link | `Test-Path False` | Yalnız diskte var olan link | §5.3 komut 7 |
| 8 | ASCII kutu 66 dışı | Tablo kayması | Kutu satırını 66'ya sabitle | §5.3 komut 8 |
| 9 | Coverage < %90 | CI kapalı | Eksik testi ekle; kodu değil testi | Sahne başına tek test |
| 10 | Komut katman ihlali | `using VersaCoder.Infrastructure` | Yalnız `l2`/`l0` | Tek `using` listesi |
| 11 | `Console.WriteLine` sızıntısı | Testler sessiz geçer | `TextWriter` enjeksiyonu | `-v` çıktısı da `stderr` |
| 12 | Timeout -> `1` | Yeniden deneme anlamsız | `3` (bağımlılık) kullan | `CommandTimeout` tek yerde |

**Küçültme ilkesi:** hata modu 1–5 **sözleşme**, 6–8 **denetim**, 9–12 **tasarım** ile çözülür;
asla tek seferlik `try/catch` yaması ile geçiştirilmez.

### §5.3 Salt-Okunur PowerShell Denetimleri (8)

> Hiçbiri dosya **yazmaz**; yalnız okur ve ölçer. Windows PowerShell 5.1 / pwsh uyumlu.

```powershell
# 1) Satir sayisi (>= 480)
(Get-Content -LiteralPath '.ai\.templates\other\cli-command-template.md').Count

# 2) Boyut KB (>= 27.0)
[math]::Round((Get-Item '.ai\.templates\other\cli-command-template.md').Length/1KB,1)

# 3) Bolum sayisi (## §[1-7] tam = 7)
(Select-String -LiteralPath '.ai\.templates\other\cli-command-template.md' -Pattern '^## §[1-7] ').Count

# 4) Alt bolum sayisi
(Select-String -LiteralPath '.ai\.templates\other\cli-command-template.md' -Pattern '^### §').Count

# 5) Mojibake taramasi (cikti 0 olmali)
(Select-String -LiteralPath '.ai\.templates\other\cli-command-template.md' -Pattern '[\u00C3\u00C2]|\uFFFD').Count

# 6) UTF-8 BOM denetimi (cikti 0 olmali)
$b=[IO.File]::ReadAllBytes('.ai\.templates\other\cli-command-template.md'); if($b[0]-eq 0xEF -and $b[1]-eq 0xBB -and $b[2]-eq 0xBF){1}else{0}

# 7) Kirik link denetimi (cikti 0 olmali)
$f='.ai\.templates\other\cli-command-template.md'; $d=Split-Path $f; $bad=0
Select-String -LiteralPath $f -Pattern '\]\((\.\.?\/[^)#]+)\)' | ForEach-Object {
  foreach($m in [regex]::Matches($_.Line,'\]\((\.\.?\/[^)#]+)\)')){
    if(-not (Test-Path -LiteralPath (Join-Path $d $m.Groups[1].Value))){$bad++} } }; $bad

# 8) ASCII kutu genisligi (kutu satirlari tam 66 olmali)
$in=$false; $bad=0
foreach($l in Get-Content -LiteralPath '.ai\.templates\other\cli-command-template.md'){
  if($l -match '^```text'){$in=$true; continue}
  if($l -match '^```' -and $in){$in=$false; continue}
  if($in -and $l -match '^[+|].*[+|]$' -and $l.Length -ne 66){$bad++} }; $bad
```

### §5.4 Quality Report (Zorunlu Şablon)

```text
+----------------------------------------------------------------+
|                     QUALITY REPORT                             |
+----------------------------------------------------------------+
|  dosya : .ai/.templates/other/cli-command-template.md          |
|  satir : ____ (hedef >= 480)      bolum : 7/7                  |
|  kb    : ____ (hedef >= 27.0)     alt   : ____                 |
|  moji  : 0 (zorunlu)              link  : 0 kirik              |
|  kutu  : 0 uyusmayan (66)         para  : ____ adet            |
|  g1..g12 : 12/12 kanitli          gate  : GATE-003 onayli      |
+----------------------------------------------------------------+
```

`Quality Report` **sonuç alanı doldurulmadan** §6.2'ye geçilmez; eksik alan = otomatik **RED**.
## §6 Doğrulama

### §6.1 Öntarama Listesi (≥17 Madde)

| # | Madde | Beklenen | Kanıt |
|---|-------|----------|-------|
| 1 | Frontmatter 8 alan, ilk 9 satırda | `title,type,category,status,version,updated,authority,scope` | §6.5 |
| 2 | `category: templates/other` | tam yazımda | §6.5 |
| 3 | `version: 1.0.0` + `authority: SSOT` | doğru | §6.5 |
| 4 | `updated: 2026-10-05` | doğru | §6.5 |
| 5 | H1 tek, ardından tek satır **Zorunlu Bağlantılar** | 1 satır | elle |
| 6 | `## §1..§7` tam 7 bölüm | 7 | §5.3 komut 3 |
| 7 | §1.1–§1.4, §2.1–§2.2 | 6 alt bölüm | §5.3 komut 4 |
| 8 | §3.1–**§3.11** | 11 alt bölüm | §5.3 komut 4 |
| 9 | §4.1–§4.5; §4.5 = G1..G12 tablosu | 5 alt | elle |
| 10 | §5.1, §5.2 (**≥8** hata modu), §5.3, §5.4 | hata modu ≥8 | elle |
| 11 | §5.3 salt-okunur PowerShell **8** komut | 8 | elle |
| 12 | §6.1 **≥17** madde | ≥17 | elle |
| 13 | §6.2 Red, §6.3, §6.4, §6.5 ölçüm | var | elle |
| 14 | §7.1 **≥6** referans | ≥6 | elle |
| 15 | §7.2 Red Team **≥10 satır** | ≥10 | elle |
| 16 | Satır ≥480 | ≥480 | §5.3 komut 1 |
| 17 | Boyut ≥27.0 KB | ≥27.0 | §5.3 komut 2 |
| 18 | Mojibake = `0` | 0 | §5.3 komut 5 |
| 19 | UTF-8 BOM = `0` | 0 | §5.3 komut 6 |
| 20 | Kırık link = `0` | 0 | §5.3 komut 7 |
| 21 | ASCII kutu = `66` | 0 uyuşmayan | §5.3 komut 8 |
| 22 | Yalnız var olan linkler | `Test-Path True` | §5.3 komut 7 |
| 23 | `{{PLACEHOLDER}}` ≥1 | ≥1 | `Select-String -Pattern '{{'` |
| 24 | EF Core'dan söz yok | 0 | `Select-String 'EntityFramework'` |
| 25 | `SELECT *` yasağı anlatılmış | var | §4.5 |
| 26 | `@p1..@pN` anlatılmış | var | §4.4/§4.5 |
| 27 | Test kimliği 5 satır `///` | var | §3.9 |
| 28 | Coverage `%90` + `TreatWarningsAsErrors` | var | §3.7 |
| 29 | CoreMusic kopyası yok | 0 | `Select-String 'CoreMusic'` |
| 30 | Beş çıkış kodu tablosu tam | `0/1/2/3/130` | §3.4 |

### §6.2 Red Hatları

```text
+----------------------------------------------------------------+
|                         RED HATLARI                            |
+----------------------------------------------------------------+
|  R1 bolum sayisi 7 disinda            -> dosya reddedilir      |
|  R2 satir < 480 veya KB < 27.0        -> dosya reddedilir      |
|  R3 §6.1 maddesi eksik (>=17 asagi)   -> dosya reddedilir      |
|  R4 §7.2 Red Team < 10 satir          -> dosya reddedilir      |
|  R5 kirik link >= 1  ya da mojibake>0 -> dosya reddedilir      |
|  R6 ASCII kutu satiri 66 disinda      -> dosya reddedilir      |
|  R7 EF Core / SELECT * / CoreMusic    -> dosya reddedilir      |
|  R8 exit kodu eksik veya karisik      -> dosya reddedilir      |
+----------------------------------------------------------------+
```

### §6.3 Geçici (Sarı) — Düzeltilmeden Commitlenir mi?

| # | Durum | Karar |
|---|-------|-------|
| Y1 | §3.6 `Program.cs` henüz yok | **Geçici** — `GATE-003` sonrası yazılır |
| Y2 | Hata modu 12'den sonrası eklenecek | **Geçici** — üretimde yeni mod görüldüğünde |
| Y3 | `--dry-run` davranışı genişletilecek | **Geçici** — ilk gerçek senaryo gelince |
| Y4 | `{{PLACEHOLDER}}` hâlâ dosyada | **Kabul** — şablon olduğu için serbest (üretilen çıktıda yasak) |

### §6.4 Kanıt Biçimi

**Kanıt satırı** iki biçimden biriyle yazılır:

```text
KANIT: <komut> -> <cikti>                                         
KANIT: <dosya> : <satir no> -> <soz>                              
```

Örnek: `KANIT: dotnet run -- export --from 2026-01-01 -> exit 0`.
**Ekran görüntüsü kabul edilmez**; yalnız komut çıktısı, **çıkış kodu** ve satır numarası geçerlidir (G5).

### §6.5 Ölçüm (Bu Şablonun Kendisi İçin)

| Metrik | Hedef | Gerçek |
|--------|-------|--------|
| Satır | ≥480 | `{SATIR}` |
| Boyut | ≥27.0 KB | `{KB}` |
| `## §` | 7 | `{BOLUM}` |
| `### §` | ≥30 | `{ALT}` |
| Mojibake | 0 | `{MOJIBAKE}` |
| Kırık link | 0 | `{KIRIK}` |
| ASCII kutu uyuşmayan | 0 | `{KUTU}` |
| `{{PLACEHOLDER}}` | ≥1 | `{PARA}` |
| §6.1 madde | ≥17 | `{MADDE}` |
| §7.2 satır | ≥10 | `{REDTEAM}` |

## §7 Referanslar

### §7.1 Bağlantılar (≥6, Yalnız Diskte Var Olanlar)

| # | Bağlantı | Yol | Neden |
|---|----------|-----|-------|
| 1 | index | [index](../index.md) | Envanter ve §3.7 kalite sözleşmesi |
| 2 | winforms-form | [winforms-form-template](../other/winforms-form-template.md) | `l7` kardeş yüzey (masaüstü) |
| 3 | handler | [handler-template](../backend/handler-template.md) | İşin gerçek sahibi |
| 4 | service | [service-template](../backend/service-template.md) | Uç nokta karşılığı |
| 5 | repository | [repository-template](../backend/repository-template.md) | `l4` (komutun görmediği kapı) |
| 6 | layer-spec | [layer-spec-template](../architecture/layer-spec-template.md) | `l0..l7` tanımı |
| 7 | dependency-rule | [dependency-rule-template](../architecture/dependency-rule-template.md) | Komut → `l2` kuralı |
| 8 | ascii-diagram | [ascii-diagram-template](../architecture/ascii-diagram-template.md) | Kutu çizim kuralı (66) |
| 9 | configuration | [configuration-template](../infrastructure/configuration-template.md) | DI/`l6` yapılandırması |
| 10 | dapper-mapping | [dapper-mapping-template](../infrastructure/dapper-mapping-template.md) | `@p1..@pN` (komut görmez) |
| 11 | unit-test | [unit-test-template](../testing/unit-test-template.md) | Test kimliği + coverage |
| 12 | test-plan | [test-plan-template](../testing/test-plan-template.md) | Gate ve strateji |
| 13 | integration-test | [integration-test-template](../testing/integration-test-template.md) | Gerçek yol kanıtı |
| 14 | form (Blazor) | [form-template](../frontend/form-template.md) | Web yüzeyi |
| 15 | Query | [Query-Template](../query/Query-Template.md) | Sorgu sözleşmesi |
| 16 | readme | [readme-template](../documentation/readme-template.md) | `--help`/kök anlatımı |
| 17 | katman-readme | [katman-readme-template](../documentation/katman-readme-template.md) | `l7` satırı |
| 18 | adr-nygard | [adr-nygard-template](../adr/adr-nygard-template.md) | CLI kararı ADR'si |
| 19 | CLAUDE | [CLAUDE](../../../CLAUDE.md) | SSOT: B5 katman, B6 G1..G12 |
| 20 | AGENTS | [AGENTS](../../../AGENTS.md) | Gölge bağlantı dosyası |
| 21 | WORKFLOW | [WORKFLOW](../../../WORKFLOW.md) | GATE-003 / GATE-004 |
| 22 | README | [README](../../../README.md) | Depo geneli tanıtım |

### §7.2 Red Team (≥10 Satır)

```text
+----------------------------------------------------------------+
|                      RED TEAM - 12 SATIR                       |
+----------------------------------------------------------------+
|  1  Kullanim hatasi exit 1 mi, 2 mi?          -> 2     (3.4)   |
|  2  Hata stdout'a mi yaziliyor?               -> HAYIR (3.5)   |
|  3  Ctrl+C cikis kodu kac?                    -> 130   (3.4)   |
|  4  Timed out exit kac?                       -> 3     (3.4)   |
|  5  Komut SQL goruyor mu?                     -> HAYIR (3.8)   |
|  6  Console.WriteLine var mi?                 -> HAYIR (4.2)   |
|  7  Bilinmeyen secenek yutuluyor mu?          -> HAYIR (4.2)   |
|  8  --json tek nesne yaziyor mu?              -> EVET  (3.5)   |
|  9  Test kimligi 5 satir mi?                  -> EVET  (3.9)   |
| 10  Coverage < %90 gecilir mi?                -> HAYIR (G11)   |
| 11  Mojibake 0 / kirik link 0 / kutu 66 mi?   -> EVET  (5.3)   |
| 12  GATE-003 onayi alindi mi?                 -> ZORUNLU (G8)  |
+----------------------------------------------------------------+
```

**Soru–yanıt (özet):**

| # | Soru | Yanıt |
|---|------|-------|
| 1 | Kullanım hatası hangi kod? | `2` — iş hiç başlamaz (§3.4) |
| 2 | İptal hangi kod? | `130` = `128 + 2`, yarı çıktı yok (§3.4) |
| 3 | Hata nereye yazılır? | `stderr`; `stdout` boru için temiz (§3.5) |
| 4 | Mantık nerede? | Ayrı serviste; komut sınıfı ince (§3.8) |
| 5 | `--json` ile `--format` çatışırsa? | `--json` kazanır (§3.2) |
| 6 | Test neden kolay? | `TextWriter` + `CancellationToken` enjeksiyonu |
| 7 | Neden `3` ayrı `1`? | Yeniden deneme anlamlı mı, değil mi (§3.4) |
| 8 | Neden kutular 66 karakter? | Depo genelinde ASCII hizası (§6.1 m.21) |

---

**Sürüm:** `1.0.0` · **Authority:** `SSOT` · **Güncelleme:** `2026-10-05`
**Kapı:** `GATE-003` onayından önce `src/**` altına satır yazılmaz (G8 · G12).
