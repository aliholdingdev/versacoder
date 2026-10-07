---
title: "WinForms — Kod-Behind: Olay Yönlendiricisi Sınırı ve Event Wiring"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Kod-behind dosyasının ne kadar uzun olabileceği, olay bağlamasının nerede durduğu ve iş mantığının Application katmanına nasıl taşınacağı"
---

# Kod-Behind: Olay Yönlendiricisi Sınırı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Kod-behind (`XxxForm.cs`) **yalnızca olay yönlendiricisidir**: olayı alır, kullanıcı girdisini doğrular, bir **Application katmanı** çağrısı yapar, sonucu ekrana yazar. Bağlantı (`+=`) `.Designer.cs`'te, gövde `.Form.cs`'tedir. Kod-behind **200 satırı** aşarsa iş mantığı UI'a gömülmüştür → taşınır (`ui-designer` iş mantığı yazar bulgusu: code-behind 200+ satır).

| # | Durum | Nerede | Örnek |
|---|---|---|---|
| 1 | `control.Event += handler` | `.Designer.cs` | `this.btnSave.Click += new EventHandler(this.btnSave_Click);` |
| 2 | Olay gövdesi (5–15 satır) | `.Form.cs` | `private async void btnSave_Click(...)` |
| 3 | Doğrulama + hata gösterimi | `.Form.cs` | `ErrorProvider` ayarı |
| 4 | Uzun iş (I/O, SQL, dosya) | **Application katmanı** | `await _projectService.SaveAsync(...)` |
| 5 | Kontrol üretimi / layout | `.Designer.cs` | `InitializeComponent` gövdesi |

### §1.1 İnce kod-behind örneği

```csharp
// NewProjectForm.cs — tasarım kodu 0 satır
public partial class NewProjectForm : RibbonForm
{
    private readonly IProjectCreator _creator;
    private readonly IErrorReporter _errors;

    public NewProjectForm(IProjectCreator creator, IErrorReporter errors)
    {
        InitializeComponent();
        _creator = creator;
        _errors = errors;
    }

    private async void btnCreate_Click(object sender, EventArgs e)
    {
        var name = txtName.Text.Trim();
        if (name.Length == 0)                     // 1) girdi doğrulama
        {
            _errorProvider.SetError(txtName, "Proje adı boş olamaz.");
            return;
        }
        _errorProvider.Clear();

        try                                       // 2) Application çağrısı
        {
            UseWaitCursor = true;
            await _creator.CreateAsync(name);     // I/O orada
        }
        catch (Exception ex)
        {
            _errors.Report(ex);                   // 3) hata yüzeyi
        }
        finally
        {
            UseWaitCursor = false;
        }

        DialogResult = DialogResult.OK;           // 4) sonucu ekrana yaz
        Close();
    }
}
```

### §1.2 Olay bağlama neden `.Designer.cs`'te

| # | `.Form.cs`'e `+=` yazılırsa | Sonuç |
|---|---|---|
| 1 | Tasarımcı dosyayı yeniden üretir | bağ **kaybolur** ya da **çift** bağ oluşur |
| 2 | `InitializeComponent` iki kez bağlar | olay **2 kez** çalışır |
| 3 | Yetki sınırı kalkar | `ui-designer` X alanı ihlali |
| 4 | Kod-behind şişer | SRP ihlali |

### §1.3 Taşıma kuralı

| Bulgu | Eylem | Sorumlu |
|---|---|---|
| `XxxForm.cs` > 200 satır | mantığı `IYyyService`'e taşı | `developer` |
| Olay içinde SQL/`SELECT` | ADO.NET + Dapper + parametreli sorgu, `CommandTimeout` setli; **EF Core yasak** | `developer` |
| Olay içinde `Thread.Sleep` | `await Task.Run(...)` + `CancellationToken` | `developer` |
| Aynı mantık 2 formda | ortak Application servisi çıkar | `architect` sözleşmesi |
| `try/catch` içinde `e.Cancel` | yalnız `FormClosing` | `developer` |

### §1.4 Bağımlılık yönü (kısa)

```
XxxForm.cs  --->  IProjectCreator (interface, l0-abstractions)
                      ^
                      |
        VersaCoder.Application  --->  VersaCoder.Infrastructure (Dapper)
```

UI katmanı (`l7-ui`) **yalnız** `Application` + `Host`'a bağımlıdır; `Infrastructure`'ı, `Domain`'i ve SQL'i **tanımaz**.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kod-behind satır sayısı < 200 | dosya ölçümü |
| 2 | `.Form.cs` içinde `+=` yok (bağ `.Designer.cs`'te) | grep = 0 |
| 3 | Olay içinde doğrudan I/O yok | kod incelemesi |
| 4 | EF Core referansı 0 · SQL parametreli | G2 taraması |
| 5 | İki bacakta `dotnet build` → 0 hata | build çıktısı |

## §3 İzinli Link Hedefleri

| # | Hedef | Bu dosyada |
|---|---|---|
| 1 | Kardeş dosya adları (bu dizindeki 30 dosya) | var |
| 2 | [index.md](index.md) — kayıt defteri | var |
| 3 | [../../CLAUDE.md](../../CLAUDE.md) — anayasa G1..G12 | var |
| 4 | `../../index.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/*.md` · `../index.md` | **yok → link verilmez** (`NEEDS_VALIDATION`) |

## §4 Yasaklar

| # | Yasak | İhlalde |
|---|---|---|
| 1 | DevExpress **v26.1** dışı sürüm referansı (G3/G8) | iş RED |
| 2 | `RibbonControlStyle.Office2010` dışı ribbon stili (G3) | iş RED |
| 3 | EF Core · `DbContext` · string-concat SQL (G2) | CRITICAL |
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
