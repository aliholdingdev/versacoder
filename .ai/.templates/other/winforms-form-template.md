---
title: "VersaCoder — WinForms Form Şablonu (WinForms Form)"
type: template
category: templates/other
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "winforms form şablonu: layout, data binding, validation, error provider, test ve %90 coverage kanıtı"
---

# VersaCoder — WinForms Form Şablonu (WinForms Form)

**Zorunlu Bağlantılar:** [index](../index.md) · [form-template](../frontend/form-template.md) · [component-template](../frontend/component-template.md) · [wireframe-template](../ui-design/wireframe-template.md) · [cli-command-template](../other/cli-command-template.md) · [unit-test-template](../testing/unit-test-template.md) · [CLAUDE](../../../CLAUDE.md)

## §1 Amaç

### §1.1 Bu Şablon ne Üretir?

**`src/VersaCoder.UI/**` altındaki tek bir WinForms formu**: `l7` katmanında çalışan, kaydet /
iptal / doğrulama akışlarını taşıyan, ADO.NET + Dapper (`@p1..@pN`) ile konuşan ve `%90`
coverage'a giren bir form sınıfı + Designer dosyası. Blazor [form-template](../frontend/form-template.md)
şablonu `<EditForm>` ve `OnValidSubmit` sözleşmesini kilitler; bu şablon aynı sözleşmeyi
`Form.ShowDialog`, `ErrorProvider` ve `CancelEventArgs` lehine yeniden kilitler.
**UI teknolojisi değişir, doğrulama sözleşmesi değişmez.**

```text
+----------------------------------------------------------------+
|        VERSACODER - WINFORMS FORM SABLONU - l7 UI              |
+----------------------------------------------------------------+
|  Girdi  : form adi + alan listesi + kayit modu (yeni/guncelle) |
|  Cikis  : kayit (persist) veya iptal (rollback)                |
|  Kural  : dogrulama once, yazma sonra, hata ekranda kalmaz     |
|  Kanim  : kod + test + ekran = 3 kanit ayni sozlesmede         |
+----------------------------------------------------------------+
```

Bu dosya **kod iskeleti değil, form sözleşmesidir**: alanların adı, veri tipi, doğrulama sırası,
hata mesajının nerede göründüğü, hangi metotun ne yaptığı ve **neyin test edilmediği** burada
kilitlenir. Designer dosyası (`*.Designer.cs`) elle açılmaz; kod tamamen `*.cs` tarafında durur.

| Öğe | Değer |
|------|-------|
| Şablon adı | `winforms-form-template.md` |
| Şablon yolu | `.ai/.templates/other/winforms-form-template.md` |
| Hedef dosya | `src/VersaCoder.UI/Forms/**/*.cs` + `*.Designer.cs` |
| Hedef kod | C# 12 · WinForms · .NET 10 (`net10.0-windows`) |
| Proje | `VersaCoder.UI` — **`l7` UI**, katman **DEĞİŞTİRİLMEZ** (küçük `CLAUDE.md` B5) |
| Katman erişimi | yalnız `l2` handler + `l0` port (**doğrudan SQL yok**) |
| Veri erişimi | form SQL görmez; `@p1..@pN` yalnız `l4` (G2, G10) |
| Kodlama | UTF-8 (BOM'suz) · Türkçe · mojibake yasak (G5) |
| Öğretilen uzunluk | 150–320 satır (kod) + bu şablonun asgari 480 satırı (G6) |
| Sürüm / Authority | `1.0.0` · `SSOT` |

### §1.2 Neden Ayrı Bir Şablon? (Ölçülmüş Disk Kanıtı)

Blazor [form-template](../frontend/form-template.md) şablonu SPA yaşam döngüsünü varsayar:
`OnInitializedAsync`, `EditContext`, `NavigationManager`, tek sayfa yeniden çizimi. WinForms'ta
bunların hiçbiri yoktur; karşılığı `Load`, `Shown`, `FormClosing`, `DialogResult` ve
`ErrorProvider`'tır. Aşağıdaki satırlar bu ortamda **2026-10-05** tarihinde ölçülmüştür ve
şablonun varsayımlarını sınırlar:

| # | Ölçüm | Değer | Etkisi |
|---|-------|-------|--------|
| 1 | `Test-Path src/` | `False` | Hedef ağaç henüz yok; şablon **hedef yol sözü** verir |
| 2 | `Test-Path tests/` | `False` | Test projesi de yok; test şablonu ile eşleştirilir |
| 3 | `Get-ChildItem -Filter *.sln` | `0` | Çözüm yok; form tek başına `dotnet build` ile doğrulanır |
| 4 | `Get-ChildItem -Filter global.json` | `0` | SDK sabitlenmemiş; SDK **10.0.301** makine genelinde geçerli |
| 5 | `dotnet --version` | `10.0.301` | `net10.0-windows` + C# 12 varsayımı geçerli |
| 6 | `.ai/.templates` içindeki `.md` | `50` | Bu şablon `other/` klasörünün **ilk** üyesidir |
| 7 | `Test-Path .ai/.templates/other` | `True` (bu şablonla açıldı) | Klasör bu şablonla oluşur |
| 8 | DevExpress paketi (küçük `CLAUDE.md` B7) | `26.1` | DevExpress bileşenleri yalnız `26.1` ile kullanılır |

**Sonuç:** bu ortamda WinForms formu **henüz yazılmamıştır**. Şablon, ilk form yazılırken
uyulacak sözleşmeyi, `GATE-003` öncesi `src/**` kapısını ve test kanıtını tek yerde toplar.

### §1.3 Şablon Kimin İçin?

| Rol | Bu şablondan ne alır? |
|-----|------------------------|
| Uygulama geliştirici (`developer`) | Formun sınıf sözleşmesini, alan sırasını, olay metotlarını |
| Test yazarı (`testing`) | Aynı formun unit test kimliğini, mock sınırını, coverage hedefini |
| Kod denetçisi (`reviewer`) | §4.5 G-tablosunu; doğrudan SQL ve **kaydetmeden önce doğrulama** ihlalini |
| Dokümantasyon yazarı (`docs-writer`) | §7.1 referanslarını; `screen-spec` + `wireframe` ile eşleşmeyi |
| Ürün sahibi (`product-manager`) | §2.2 kapsam dışı listesini; hangi ekranın bu şablona girmediğini |

### §1.4 Bu Şablon Hangi Soruyu Cevaplamaz?

- **Görsel tasarım dili** — renk, tipografi, boşluk ritmi: [wireframe-template](../ui-design/wireframe-template.md) ve [screen-spec-template](../ui-design/screen-spec-template.md) esastır.
- **Blazor/SPA formu** — `<EditForm>` / `EditContext`: [form-template](../frontend/form-template.md) esastır.
- **API uç noktası** — HTTP sözleşmesi: [api-doc-template](../documentation/api-doc-template.md) esastır.
- **Veri şeması ve sorgu** — `@p1..@pN`, `LIMIT/OFFSET`: [Query-Template](../query/Query-Template.md) esastır.
- **Mimari karar** — neden WinForms olduğu: [adr-nygard-template](../adr/adr-nygard-template.md) ile ADR yazılır.
- **CI/CD ve yayın** — pipeline: [WORKFLOW](../../../WORKFLOW.md) §6 gate'leri esastır.

## §2 Kapsam

### §2.1 Kapsam Dışı (Bu Şablonla Üretilmeyen)

| # | Kapsam dışı | Nereye gider? |
|---|-------------|----------------|
| 1 | Yeni katman tanımı (`l0..l7`) | [layer-spec-template](../architecture/layer-spec-template.md) |
| 2 | DI kaydı ve composition root | `l6` Host — [configuration-template](../infrastructure/configuration-template.md) |
| 3 | Doğrudan SQL / Dapper sorgusu | [repository-template](../backend/repository-template.md) + [Query-Template](../query/Query-Template.md) |
| 4 | HTTP/gRPC uç noktası | [service-template](../backend/service-template.md) |
| 5 | Ekran akışı ve navigasyon hiyerarşisi | [flow-template](../ui-design/flow-template.md) |
| 6 | Kod inceleme raporu | WORKFLOW §5 gate akışı |
| 7 | DevExpress sürüm yükseltme kararı | [adr-devexpress-template](../adr/adr-devexpress-template.md) |
| 8 | Test stratejisi ve gate tanımı | [test-plan-template](../testing/test-plan-template.md) |
| 9 | CLI komutu (ayrı `l7` yüzeyi) | [cli-command-template](../other/cli-command-template.md) |
| 10 | Sürüm notu / yayın | [readme-template](../documentation/readme-template.md) |

### §2.2 Kapsam İçi (Bu Şablonla Kilitlenen)

| # | Kapsam içi | Kanıt |
|---|------------|-------|
| 1 | Form sınıfı adı ve dosya yolu | §3.1 |
| 2 | Alan listesi, veri tipi, zorunluluk | §3.2 |
| 3 | Doğrulama sırası ve `ErrorProvider` davranışı | §3.3 |
| 4 | Kaydet / İptal akışı ve `FormClosing` kararı | §3.4 |
| 5 | `l2` handler çağrısı; doğrudan `l4` erişimi **yasak** | §3.5 |
| 6 | 5 satırlık test kimliği + `%90` coverage | §3.9 |
| 7 | 8+ hata modu ve küçültme stratejisi | §5.2 |
| 8 | Salt-okunur PowerShell denetimleri | §5.3 |
| 9 | Red Team §7.2 (≥10 satır) | §7.2 |

```text
+----------------------------------------------------------------+
|                     KAPSAM SINIRI                              |
+----------------------------------------------------------------+
|  BU SABLON                          BU SABLON DEGIL            |
|  ------------                       ----------------           |
|  form + designer.cs                 migration / schema         |
|  dogrulama + ErrorProvider          Dapper sorgusu             |
|  kaydet / iptal + FormClosing       DI registration            |
|  unit test kimligi + %90            ekran akisi (flow)         |
|  8+ hata modu                       CI pipeline                |
+----------------------------------------------------------------+
```
## §3 Mimari

### §3.1 Form Sınıfı Sözleşmesi

```csharp
// src/VersaCoder.UI/Forms/CustomerForm.cs
namespace VersaCoder.UI.Forms;

/// <summary>Müşteri kaydı formu — l7 UI; yalnız l2 handler çağırır.</summary>
public sealed partial class CustomerForm : Form
{
    private readonly ICustomerHandler _handler;
    private readonly ErrorProvider _errors;
    private int? _customerId;            // null = yeni kayıt

    public CustomerForm(ICustomerHandler handler)
    {
        _handler = handler;
        _errors = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };
        InitializeComponent();
        Shown += OnShown;
        FormClosing += OnFormClosing;
    }

    public int? CustomerId { get => _customerId; set => _customerId = value; }
}
```

**Kurallar:**

1. Sınıf adı = `{EkranAdı}Form`, dosya = `{EkranAdı}Form.cs` + `{EkranAdı}Form.Designer.cs`.
2. Constructor **yalnız** DI enjeksiyonu + `InitializeComponent` + olay bağlama yapar.
3. Form üzerinde **alan mantığı yok**; mantık `private` metotlarda, yazma `l2` handler'ındadır.
4. `partial` sınıf; `Designer.cs` elle düzenlenmez (G8).
5. Form **asla** `SqlConnection`, `Dapper` veya SQL dizesi görmez.

### §3.2 Alan Listesi ve Veri Tipi

| # | Alan | Tip | Zorunlu | Doğrulama | Hata mesajı |
|---|------|-----|---------|-----------|-------------|
| 1 | `CustomerId` | `int?` | Hayır (gizli) | — | — |
| 2 | `FullName` | `string` | Evet | 2–120 karakter, harf+boşluk | `Ad soyad 2-120 karakter olmalı.` |
| 3 | `Email` | `string` | Evet | tek `@`, nokta sonrası ≥2 | `Geçerli bir e-posta girin.` |
| 4 | `Phone` | `string` | Hayır | yalnız rakam, `+`, `-`, boşluk; ≤20 | `Telefon yalnız rakam olabilir.` |
| 5 | `BirthDate` | `DateOnly` | Hayır | `≥ 1900-01-01` ve `≤ bugün` | `Doğum tarihi aralık dışında.` |
| 6 | `IsActive` | `bool` | Evet | — | — |

**Tip kuralları:** metin için `string` (asla `null` yerine boş değil), tarih için `DateOnly`
(asla `DateTime` — saat bilgisi formda anlamsız), kimlik için `int?` (yeni kayıtta `null`).

### §3.3 Doğrulama Sırası ve ErrorProvider

Doğrulama **her zaman aynı sırayla** çalışır; sıra değişirse test kırılır (§6.1 madde 9).

```text
+----------------------------------------------------------------+
|                  DOGRULAMA SIRASI (6 ADIM)                     |
+----------------------------------------------------------------+
|  1 bos alan kontrolu      -> zorunlu alan hata mesaji          |
|  2 uzunluk kontrolu       -> min/max karakter                  |
|  3 format kontrolu        -> e-posta / telefon deseni          |
|  4 aralik kontrolu        -> tarih siniri, deger araligi       |
|  5 cakisma kontrolu       -> l2 handler UniqueAsync (sunucu)   |
|  6 hepsi gecti            -> kaydet (l2 handler SaveAsync)     |
+----------------------------------------------------------------+
```

1. Adım 1–4 **form tarafında** (çevrimdışı, anında); adım 5–6 **`l2` tarafında** (sunucu gerçeği).
2. Hata görüldüğünde odak **ilk hatalı alana** `Focus()` edilir.
3. `_errors.SetError(control, message)` yazılır; temizlenince `SetError(control, null)`.
4. Birden çok alan hatalıysa **hepsi** işaretlenir, sonra ilkine odaklanılır.
5. `ErrorProvider` `BlinkStyle = NeverBlink` (erişilebilirlik; yanıp sönme nöbet riski).

### §3.4 Kaydet / İptal ve FormClosing

```csharp
private async void OnSaveClick(object? sender, EventArgs e)
{
    if (!ValidateAll()) { FocusFirstError(); return; }

    var dto = MapToDto();                       // yalnız form -> DTO
    try
    {
        var result = await _handler.SaveAsync(dto, CancellationToken.None);
        if (!result.Ok) { ShowServerErrors(result.Errors); return; }

        _customerId = result.Id;
        DialogResult = DialogResult.OK;
        Close();
    }
    catch (OperationCanceledException) { ShowInfo("Kaydetme iptal edildi."); }
    catch (TimeoutException)           { ShowError("Kaydetme zaman aşımına uğradı."); }
    catch (Exception ex)               { ShowError($"Beklenmeyen hata: {ex.Message}"); }
}

private void OnFormClosing(object? sender, FormClosingEventArgs e)
{
    if (DialogResult != DialogResult.OK && IsDirty())
        e.Cancel = !ConfirmDiscard();           // "Kaydedilmemiş değişiklik var"
}
```

**Kurallar:** (a) `async void` **yalnız** olay işleyicilerinde serbesttir; diğer her yerde `async Task`.
(b) `CancellationToken.None` yerine `CancellationTokenSource` + `FormClosing`'de `Cancel()` tercih edilir.
(c) Kullanıcıya `MessageBox` ile **tek** soru sorulur; ikinci soru asla yok.
(d) İptal edilirse **hiçbir** yazma yapılmaz (`l2` hiç çağrılmaz) — rollback formda başlar.

### §3.5 Katman Erişimi (l0..l7)

```text
+----------------------------------------------------------------+
|  l7 UI  -->  l2 Application  -->  l1 Domain  -->  l0 Abstr.    |
|   |              |                                             |
|   |              +-->  l4 Infrastructure (repo + ADO.NET)      |
|   |              +-->  l3 Crosscutting (IClock, ILogger)       |
|   +-- YASAK: dogrudan l4 / l1 / SQL erisimi                    |
+----------------------------------------------------------------+
```

| Katman | Formdan erişim | Ne kullanır? |
|--------|----------------|--------------|
| `l0` | Arayüzler (port) | `IClock`, `IUnitOfWork`, DTO sözleşmeleri |
| `l1` | **Doğrudan yok** | Form yalnız `SaveAsync` sonucundaki DTO'yu görür |
| `l2` | **Evet (tek kapı)** | `ICustomerHandler`, validator |
| `l3` | Sınırlı | `IClock` (tarih sınırı testleri için) |
| `l4` | **Yasak** | Form asla repo çağırmaz |
| `l5` | **Yasak** | Form asla protokol çağırmaz |
| `l6` | Constructor DI | Composition root formu kurar |
| `l7` | Bu şablon | Form + Designer |

### §3.6 Dosya ve Proje Düzeni

```text
src/VersaCoder.UI/                                                
├── Forms/                                                        
│   ├── CustomerForm.cs            (bu şablon)                    
│   ├── CustomerForm.Designer.cs   (otomatik; elle yok)           
│   └── CustomerForm.README.md     (§3.11)                        
├── Controls/                     (yeniden kullanılabilir alanlar)
├── Program.cs                     (l6 composition root çağrısı)  
└── VersaCoder.UI.csproj                                          
tests/VersaCoder.Tests/                                           
└── Forms/CustomerFormTests.cs     (§3.9)                         
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

1. `VersaCoder.UI` → `VersaCoder.Application` (`l2`) **+** `VersaCoder.Abstractions` (`l0`) + `l3`.
2. `VersaCoder.UI` → `VersaCoder.Infrastructure` (**yasak** — dolaylı, `l6` üzerinden gelir).
3. Form → Handler arayüzü; form → **somut** repo (**yasak**).
4. Üçüncü taraf yalnız: WinForms BCL + (isteğe bağlı) DevExpress `26.1`.
5. `Moq`/`NSubstitute` yalnız `tests/**` altındadır; `src/**` test paketi görmez.

### §3.9 Test Kimliği (5 Satır Zorunlu)

Her form test metodu **tam beş satır** `///` ile başlar; başka yorum biçimi yoktur (G5):

```csharp
/// <Purpose>SaveAsync çağrıldığında geçerli DTO kalıcı yazar.</Purpose>
/// <Input>FullName="Ayşe Yılmaz", Email="a@b.co", IsActive=true</Input>
/// <Expected Result>Handler bir kez çağrılır, DialogResult.OK döner.</Expected Result>
/// <Failure Criteria>Handler hiç çağrılmaz veya DialogResult.Cancel döner.</Failure Criteria>
/// <Evidence>dotnet test --filter CustomerForm_Save_Valid_ReturnsOk</Evidence>
```

**İsimlendirme:** dosya/sınıf `{FormAdı}Tests`; metot `{Form}_{Eylem}_{Beklenen}` —
ör. `CustomerForm_Save_Valid_ReturnsOk`, `CustomerForm_Save_Invalid_FocusesFirstError`,
`CustomerForm_Closing_Dirty_ConfirmsDiscard`.

**Test sınıfları ve oran hedefi:**

| # | Test grubu | Örnek metot | Pay |
|---|------------|-------------|-----|
| 1 | Doğrulama (form tarafı) | `CustomerForm_Validate_Empty_ReportsError` | %35 |
| 2 | Kaydet akışı | `CustomerForm_Save_Valid_ReturnsOk` | %25 |
| 3 | Hata yolları | `CustomerForm_Save_Timeout_ShowsMessage` | %20 |
| 4 | İptal / FormClosing | `CustomerForm_Closing_Dirty_ConfirmsDiscard` | %10 |
| 5 | DTO eşleme | `CustomerForm_MapToDto_MapsAllFields` | %10 |

### §3.10 Sınır: Ne Test Edilmez?

| Test edilmez | Neden |
|--------------|-------|
| `InitializeComponent()` iç çizgisi | Designer üretimi; elde tutulması anlamsız |
| DevExpress `26.1` kontrol içi | 3. taraf; yalnız **kol** davranışı test edilir |
| Gerçek veritabanı | `l4` repo **sahte** ile; entegrasyon testi ayrı şablon |
| Ekran piksel hizası | Görsel doğrulama manuel; wireframe ile karşılaştırılır |
| `MessageBox.Show` metni kesinliği | yalnız **gösterilme kararı** (kez sayısı) test edilir |

### §3.11 Form README'si (Dosya Yanında)

Her form için bir `.README.md` **zorunlu değildir**; yazıldığında şu 5 başlığı taşır:
**Amaç**, **Alan tablosu**, **Doğrulama sırası**, **Bilinen sınırlar**, **Test kanıtı**.
README **repo köküne değil**, formun klasörüne yazılır; kök README'yi
[readme-template](../documentation/readme-template.md) yönetir.
## §4 Kurallar

### §4.1 Zorunlu (Bu Şablonda Asla Aşılmaz)

| # | Kural | İhlal Sonucu |
|---|-------|--------------|
| R1 | Form asla `l4`/SQL çağırmaz | G8 ihlali — PR reddi |
| R2 | Doğrulama **kaydetmeden önce** tamamlanır | Veri bozuk kayıt — GATE-005 BLOKE |
| R3 | `Designer.cs` elle düzenlenmez | Merge çakışması + G8 |
| R4 | Türkçe kaynak; mojibake `0` | G5 ihlali |
| R5 | `{{PLACEHOLDER}}` yalnız şablonda, üretilen koda geçmez | Kod derlenmez |
| R6 | Test kimliği 5 satır `///` | Test reddi (§3.9) |
| R7 | Coverage **%90**, uyarılar **hata** (G11) | CI kapalı |

### §4.2 Yapılmayan (Bilinçli Sınır)

- Formda **hiçbir** iş kuralı hesabı yapılmaz (o `l1`/`l2` işidir).
- Hiçbir `async void` metot **olay işleyicisi dışında** tanımlanmaz.
- Form, kendi kendine **yeniden çizim** (double-buffer dışında) tetiklemez.
- `Application.Exit()` form içinde çağrılmaz; yalnız `Program.cs`.
- Yazı tipi/renk **kod içinde** sabitlenmez; tema `l6`/Designer üzerinden gelir.

### §4.3 Yapılacak (Minimum)

1. `Shown` olayında ilk alana odak.
2. Her zorunlu alan için tek hata mesajı (Türkçe, eylem öneren).
3. `IsDirty()` için alan-bazlı takip (`_initial` kopyası).
4. Kaydetme sırasında buton **devre dışı** (`Save.Enabled = false`) — çift tıklama yok.
5. Form dispose: `ErrorProvider`, `CancellationTokenSource`, `IDataReader` (varsa).

### §4.4 G1..G12 Eşlemesi

| G | Bu şablondaki karşılığı | Kanıt |
|---|-------------------------|-------|
| G1 | CoreMusic **kopyalanmaz**; WinForms deseni buradan yazılır | §3.1 kodu özgün |
| G2 | EF Core **yasak**; form yalnız `l2` çağırır | §3.5 |
| G3 | Türkçe karakter `VARCHAR(260)` sınırı formda kontrol edilir | §3.2 `FullName ≤ 120` |
| G4 | Somut **değil**, arayüz üzerinden DI | `ICustomerHandler` |
| G5 | UTF-8 BOM'suz, Türkçe, `///` yorum biçimi | §5.3 komut 5–6 |
| G6 | Şablon ≥480 satır, kod 150–320 satır | §6.5 |
| G7 | Ürün adı **VersaCoder**; `CoreMusic` geçmez | §6.1 madde 29 |
| G8 | `src/**` yalnız `GATE-003` sonrası | WORKFLOW §4 |
| G9 | `l7` katmanı **DEĞİŞTİRİLMEZ** | §3.5 |
| G10 | Parametreleme `@p1..@pN` — form bunu **görmez**, `l4` görür | §3.5 |
| G11 | `%90` coverage + `TreatWarningsAsErrors=true` | §3.7 |
| G12 | Anahtar kural metni kök `CLAUDE.md`'de | [CLAUDE](../../../CLAUDE.md) |

### §4.5 G1..G12 Tam Tablo (Bu Şablonun Kırılma Noktaları)

| G | Kural | Kırılırsa | Yakalayan denetim |
|---|-------|-----------|-------------------|
| G1 | CoreMusic'ten kopya kod yasak | Telif + yanlış desen | Kod incelemesi, §6.1 m.29 |
| G2 | EF Core yasak (ADO.NET + Dapper) | Bağımlılık sızıntısı | `Select-String -Path *.csproj -Pattern 'EntityFramework'` |
| G3 | Türkçe karakter `VARCHAR(260)` sınırı | Kayıp veri | Uzunluk testi §6.1 m.8 |
| G4 | Somut sınıf değil arayüz | Test edilemezlik | `Select-String 'new CustomerRepository' src/VersaCoder.UI` |
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
| 2 | [screen-spec](../ui-design/screen-spec-template.md) + [wireframe](../ui-design/wireframe-template.md) onayı | Alan listesi | — |
| 3 | `GATE-003` onayı | Yazma izni | **HARD STOP** |
| 4 | Form + Designer oluştur (`l7`) | `*.cs` | — |
| 5 | Doğrulama + `ErrorProvider` bağla | §3.3 | — |
| 6 | `l2` handler çağrısını bağla | §3.4 | — |
| 7 | Test dosyasını yaz (5 satır `///`) | `%90` | G11 |
| 8 | §5.3 denetimlerini çalıştır | 8/8 | — |
| 9 | §6.1 ≥17 maddeyi işaretle | Red hattı | §6.2 |
| 10 | [test-plan](../testing/test-plan-template.md) + [unit-test](../testing/unit-test-template.md) ile hizala | Rapor | GATE-004 |

### §5.2 Hata Modları (≥8) ve Küçültme

| # | Hata modu | Belirti | Düzeltme | Küçültme |
|---|-----------|---------|----------|----------|
| 1 | Doğrulama atlandı | Kayıt bozuk satır | `ValidateAll()` zorunlu, §3.3 sırası | Alan başına tek validator |
| 2 | Çift tıklama iki kayıt | Tek tuşla iki satır | Kaydet sırasında `Enabled=false` | Tek `Save.Enabled` kilidi |
| 3 | `async void` patlaması | Yakalanmayan istisna | Yalnız olay işleyicisinde + try/catch | İş mantığını `async Task`'e taşı |
| 4 | İptal yine de yazdı | Kullanıcı kaybettiği veri | İptalde handler **çağrılmaz** | Tek `OnSaveClick` kapısı |
| 5 | `Designer.cs` çakışması | Merge savaşları | Designer **elle yok** | Alanları `InitializeComponent` dışında kurma |
| 6 | Mojibake (U+FFFD bozuk harf) | Turkce bozuk | UTF-8 **BOM'suz** kaydet | §5.3 komut 5 |
| 7 | Kırık iç link | `Test-Path False` | Yalnız diskte var olan link | §5.3 komut 7 |
| 8 | ASCII kutu 66 dışı | Tablo kayması | Kutu satırını 66'ya sabitle | §5.3 komut 8 |
| 9 | Coverage < %90 | CI kapalı | Eksik testi ekle; kodu değil testi | Sahne başına tek test |
| 10 | Form katman ihlali | `using VersaCoder.Infrastructure` | Yalnız `l2`/`l0` | Tek `using` listesi |
| 11 | `FormClosing`'de asma | Çift onay penceresi | Tek `ConfirmDiscard()` | Tek bayrak `_dirty` |
| 12 | Zaman aşımı donması | Kullanıcı kilidi | `TimeoutException` yakala + mesaj | Handler'da `CommandTimeout` |

**Küçültme ilkesi:** hata modu 1–3 **sözleşme**, 4–8 **denetim**, 9–12 **tasarım** ile çözülür;
asla tek seferlik `try/catch` yaması ile geçiştirilmez.

### §5.3 Salt-Okunur PowerShell Denetimleri (8)

> Hiçbiri dosya **yazmaz**; yalnız okur ve ölçer. Windows PowerShell 5.1 / pwsh uyumlu.

```powershell
# 1) Satir sayisi (>= 480)
(Get-Content -LiteralPath '.ai\.templates\other\winforms-form-template.md').Count

# 2) Boyut KB (>= 27.0)
[math]::Round((Get-Item '.ai\.templates\other\winforms-form-template.md').Length/1KB,1)

# 3) Bolum sayisi (## §[1-7] tam = 7)
(Select-String -LiteralPath '.ai\.templates\other\winforms-form-template.md' -Pattern '^## §[1-7] ').Count

# 4) Alt bolum sayisi
(Select-String -LiteralPath '.ai\.templates\other\winforms-form-template.md' -Pattern '^### §').Count

# 5) Mojibake taramasi (cikti 0 olmali)
(Select-String -LiteralPath '.ai\.templates\other\winforms-form-template.md' -Pattern '[\u00C3\u00C2]|\uFFFD').Count

# 6) UTF-8 BOM denetimi (cikti 0 olmali)
$b=[IO.File]::ReadAllBytes('.ai\.templates\other\winforms-form-template.md'); if($b[0]-eq 0xEF -and $b[1]-eq 0xBB -and $b[2]-eq 0xBF){1}else{0}

# 7) Kirik link denetimi (cikti 0 olmali)
$f='.ai\.templates\other\winforms-form-template.md'; $d=Split-Path $f; $bad=0
Select-String -LiteralPath $f -Pattern '\]\((\.\.?\/[^)#]+)\)' | ForEach-Object {
  foreach($m in [regex]::Matches($_.Line,'\]\((\.\.?\/[^)#]+)\)')){
    if(-not (Test-Path -LiteralPath (Join-Path $d $m.Groups[1].Value))){$bad++} } }; $bad

# 8) ASCII kutu genisligi (kutu satirlari tam 66 olmali)
$in=$false; $bad=0
foreach($l in Get-Content -LiteralPath '.ai\.templates\other\winforms-form-template.md'){
  if($l -match '^```text'){$in=$true; continue}
  if($l -match '^```' -and $in){$in=$false; continue}
  if($in -and $l -match '^[+|].*[+|]$' -and $l.Length -ne 66){$bad++} }; $bad
```

### §5.4 Quality Report (Zorunlu Şablon)

```text
+----------------------------------------------------------------+
|                     QUALITY REPORT                             |
+----------------------------------------------------------------+
|  dosya : .ai/.templates/other/winforms-form-template.md        |
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
| 30 | Ürün adı **VersaCoder** | doğru | elle |

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
+----------------------------------------------------------------+
```

### §6.3 Geçici (Sarı) — Düzeltilmeden Commitlenir mi?

| # | Durum | Karar |
|---|-------|-------|
| Y1 | §3.11 `.README.md` yazılmadı | **Geçici** — bir sonraki PR'da tamamlanır |
| Y2 | Hata modu 12'den sonrası eklenecek | **Geçici** — üretimde yeni mod görüldüğünde eklenir |
| Y3 | DevExpress kontrol listesi genişletilecek | **Geçici** — `26.1` sürüm notlarıyla |
| Y4 | `{{PLACEHOLDER}}` hâlâ dosyada | **Kabul** — şablon olduğu için serbest (üretilen kodda yasak) |

### §6.4 Kanıt Biçimi

**Kanıt satırı** iki biçimden biriyle yazılır:

```text
KANIT: <komut> -> <cikti>                                         
KANIT: <dosya> : <satir no> -> <soz>                              
```

Örnek: `KANIT: Get-Content ... | Measure-Object -> Count = 770`.
**Ekran görüntüsü kabul edilmez**; yalnız komut çıktısı ve satır numarası geçerlidir (G5).

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
| 2 | form (Blazor) | [form-template](../frontend/form-template.md) | Aynı doğrulama sözleşmesinin SPA karşılığı |
| 3 | component | [component-template](../frontend/component-template.md) | Bileşen/partial ayrımı |
| 4 | wireframe | [wireframe-template](../ui-design/wireframe-template.md) | Alan yerleşimi kaynağı |
| 5 | screen-spec | [screen-spec-template](../ui-design/screen-spec-template.md) | Alan spekülasyonu |
| 6 | cli-command | [cli-command-template](../other/cli-command-template.md) | `l7` CLI kardeşi (bu part) |
| 7 | unit-test | [unit-test-template](../testing/unit-test-template.md) | Test kimliği + coverage |
| 8 | test-plan | [test-plan-template](../testing/test-plan-template.md) | Gate ve strateji |
| 9 | layer-spec | [layer-spec-template](../architecture/layer-spec-template.md) | `l0..l7` tanımı |
| 10 | dependency-rule | [dependency-rule-template](../architecture/dependency-rule-template.md) | Form → `l2` kuralı |
| 11 | repository | [repository-template](../backend/repository-template.md) | `l4` (formun görmediği kapı) |
| 12 | service | [service-template](../backend/service-template.md) | Handler sözleşmesi |
| 13 | api-doc | [api-doc-template](../documentation/api-doc-template.md) | Uç nokta dokümanı |
| 14 | readme | [readme-template](../documentation/readme-template.md) | Form README formatı |
| 15 | adr-nygard | [adr-nygard-template](../adr/adr-nygard-template.md) | WinForms kararı ADR'si |
| 16 | adr-devexpress | [adr-devexpress-template](../adr/adr-devexpress-template.md) | DevExpress `26.1` kararı |
| 17 | configuration | [configuration-template](../infrastructure/configuration-template.md) | DI/`l6` yapılandırması |
| 18 | Query | [Query-Template](../query/Query-Template.md) | Sorgu sözleşmesi (form görmez) |
| 19 | CLAUDE | [CLAUDE](../../../CLAUDE.md) | SSOT: B5 katman, B6 G1..G12 |
| 20 | AGENTS | [AGENTS](../../../AGENTS.md) | Gölge bağlantı dosyası |
| 21 | WORKFLOW | [WORKFLOW](../../../WORKFLOW.md) | GATE-003 / GATE-004 |
| 22 | README | [README](../../../README.md) | Depo geneli tanıtım |

### §7.2 Red Team (≥10 Satır)

```text
+----------------------------------------------------------------+
|                      RED TEAM - 12 SATIR                       |
+----------------------------------------------------------------+
|  1  Dogrulama kaydetmeden once mi, sonra mi?  -> ONCE (3.3)    |
|  2  Form SQL goruyor mu?                      -> HAYIR (3.5)   |
|  3  Designer elle duzenlendi mi?              -> HAYIR (4.1)   |
|  4  async void olay disinda var mi?           -> HAYIR (3.4)   |
|  5  Cift tiklama iki kayit yapabilir mi?      -> BUTON KAPALI  |
|  6  Iptalde yazma olur mu?                    -> HAYIR (3.4)   |
|  7  Test kimligi 5 satir mi?                  -> EVET  (3.9)   |
|  8  Coverage < %90 gecilir mi?                -> HAYIR (G11)   |
|  9  Mojibake 0 mu?                            -> EVET  (5.3)   |
| 10  Kirik link 0 mu?                          -> EVET  (5.3)   |
| 11  ASCII kutu 66 mi?                         -> EVET  (5.3)   |
| 12  GATE-003 onayi alindi mi?                 -> ZORUNLU (G8)  |
+----------------------------------------------------------------+
```

**Soru–yanıt (özet):**

| # | Soru | Yanıt |
|---|------|-------|
| 1 | Doğrulama ne zaman? | Kaydetmeden **önce**, 6 adım (§3.3) |
| 2 | İptal ne yapar? | Handler **çağırmaz**, `IsDirty` sorar |
| 3 | Katman ihlali nasıl görünür? | `using VersaCoder.Infrastructure` satırı |
| 4 | `Designer.cs` neden elden geçmez? | Üretim dosyası; çakışma + G8 |
| 5 | Neden `ErrorProvider` + `Focus`? | Tek görsel geri bildirim kanalı |
| 6 | Neden `DateOnly`? | Saat bilgisi formda anlamsız, `G3` riski yok |
| 7 | Neden form tek başına test edilir? | `l2` sahte; gerçek yol entegrasyon şablonunda |
| 8 | Neden kutular 66 karakter? | Depo genelinde ASCII hizası (§6.1 m.21) |

---

**Sürüm:** `1.0.0` · **Authority:** `SSOT` · **Güncelleme:** `2026-10-05`
**Kapı:** `GATE-003` onayından önce `src/**` altına satır yazılmaz (G8 · G12).
