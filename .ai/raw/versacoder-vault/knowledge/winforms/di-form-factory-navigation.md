---
title: "di-form-factory-navigation - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Formu DI container'dan uretme (IFormFactory / ActivatorUtilities), uretilen formun sahiplik ve Dispose kurallari, navigasyon servisi siniri ve iki bacak farki"
---

# di-form-factory-navigation — Formu DI'dan Üretmek ve Geçiş Yapmak

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [coding-standards](../../.rules/coding-standards.md) · [form-showdialog-result](form-showdialog-result.md) · [form-single-instance-reuse](form-single-instance-reuse.md) · [constructor-injection-in-forms](constructor-injection-in-forms.md) · [composition-root-host-startup](composition-root-host-startup.md) · [di-container-lifetimes](di-container-lifetimes.md) · [form-navigation-patterns](form-navigation-patterns.md)

> **Tek cümlelik sorumluluk:** *Yeni bir form `new` ile mi, container'dan mı üretilir; üreten taraf kim, formu kim Dispose eder, navigasyon kararı nerede durur* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Form **`new SomeForm()` ile değil, `IFormFactory.Create<T>()` ile** üretilir; factory arka planda `ActivatorUtilities` ile kurucudaki `interface` bağımlılıklarını container'dan çözer. **Üreten Dispose eder**: modalda `using`, modelessa `Close()`/`FormClosed` zinciri. Navigasyon **kararı** (hangi form, modal mı, tek örnek mi) navigasyon/factory katmanında kalır; çağıran yalnız **amaç** söyler.

| # | Katman | Ne yapar | Ne yapmaz |
|---|---|---|---|
| 1 | `composition-root` (`Program.cs`) | `IServiceProvider` kurar, factory'yi kaydeder | form üretmez, iş mantığı yazmaz |
| 2 | `IFormFactory` / `FormFactory` | `Create<T>()` → `ActivatorUtilities.CreateInstance` | navigasyon kararı, `Show` çağırmaz |
| 3 | Navigasyon servisi | modal/modeless/tek-örnek **kararı** + sahiplik | bağımlılık çözmez (`new` yok) |
| 4 | Çağran form (`.Form.cs`) | bağımlılık alır, işini görür | kendini `new`'lemez, servis locatör kullanmaz |
| 5 | Çağıran (UI olayı) | `using` / `Close()` ile ömrü bitirir | formun içine karışmaz |

---

## §1 Kalıp: factory + çağrı

```csharp
// 1) Sozlesme (UI katmani)
public interface IFormFactory
{
    T Create<T>() where T : Form;      // tek sorumluluk: uret
}

// 2) Implementasyon (composition-root yakininda; 3-5 satir)
public sealed class FormFactory : IFormFactory
{
    private readonly IServiceProvider _sp;
    public FormFactory(IServiceProvider sp) { _sp = sp; }

    public T Create<T>() where T : Form
    {
        return (T)ActivatorUtilities.CreateInstance(_sp, typeof(T));   // kurucu cozumlenir
    }
}

// 3) Cagiran (yalnizca olay yonlendirici)
private async void btnNewSeries_Click(object sender, EventArgs e)
{
    using (var dlg = _forms.Create<SeriesEditForm>())      // ureten Dispose eder
    {
        dlg.Owner = this;
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;                                        // iptal: arayuze dokunma

        await _series.SaveAsync(dlg.Model, _cts.Token);    // is mantigi serviste
    }
}
```

| Satır | Kural | İhlalde |
|---|---|---|
| `Create<T>()` | tek üretim noktası | gizli `new` (test edilemez) |
| `using` | modal ömrü çağıranındır | olay sızıntısı, handle birikimi |
| `dlg.Owner = this` | sahipli modal | odak kayması |
| `DialogResult.OK` kontrolü | iptalde iş yapılmaz | yarım iş |
| `_cts.Token` | iptal ilerletilir | form kapanınca iş devam |

---

## §2 Kim neyi Dispose eder (sahiplik tablosu)

| Üretim yolu | Ömür | Dispose eden | Not |
|---|---|---|---|
| `factory.Create<T>()` + `ShowDialog` | kısa | **çağıran `using`** | tipik modal |
| `factory.Create<T>()` + `Show` | pencere ömrü | `Close()` → `FormClosed` zinciri | çağıran `FormClosed`'da referansı bırakır |
| Navigasyon servisi tek-örnek cache'i | uzun | **cache sahibi** (`di-container-lifetimes`) | `IsDisposed` kontrolü şart |
| Container'a kayıtlı `Form` (`AddTransient<Form>`) | çağrı başına | çağıran | container **formu cache'lemez** (transient) |
| `Application.OpenForms` ile bulunan form | açık kalmış | açan | bulmak Dispose etmek değildir |

---

## §3 Tasarımcı üçlüsü ile çakışması (kritik)

| Durum | Sonuç | Çözüm |
|---|---|---|
| Tek kurucu: `SettingsForm(ISettingsService s)` | tasarımcı **açılmaz** ("no parameterless constructor") | parametresiz kurucu `.Form.cs`'te kalır, DI kurucusu ikinci olarak eklenir |
| `ActivatorUtilities` iki kurucu gördü | en uygun olanı seçer; `null` servis verirse hata | imzalar net, opsiyonel bağımlılık `?` ile |
| Kurucu içinde UI kodu (`MessageBox`, `Show`) | tasarım zamanı çöker | yalnız `InitializeComponent()` + alan atama |
| Kontrol üretimi `.Form.cs`'e taşındı | üçlü RED (`XxxForm.cs` / `.Designer.cs` / `.resx`) | tasarım `.Designer.cs`'te kalır |
| `.resx` elle oynandı | `ResourceManager` adı kayar, metinler `null` | `.resx` yalnız tasarımcıyla değişir |

**Doğru üçlü:**

```
SettingsForm.cs            -> DI kurucusu + olay yonlendirici + is cagrisi
SettingsForm.Designer.cs   -> parametresiz kurucu + InitializeComponent + tum kontroller
SettingsForm.resx          -> metinler, ikonlar ($this.*, btn*.*) - tasarımcı ile
```

> `ActivatorUtilities` kurucusu **imzadan** seçer; `.Designer.cs`'teki parametresiz kurucu **silinmez** (bkz. [constructor-injection-in-forms](constructor-injection-in-forms.md)) — üçlü aksi halde RED olur.

---

## §4 Navigasyon kararı nerede

| Soru | Sahibi | Örnek |
|---|---|---|
| Hangi form açılır? | çağıran (amaç) | `Create<SeriesEditForm>()` |
| Modal mı, modeless mı? | navigasyon servisi / kural | onay → modal |
| Tek örnek mi, her seferinde yeni mi? | navigasyon servisi | [form-single-instance-reuse](form-single-instance-reuse.md) |
| Bağımlılıklar nasıl çözülür? | factory + container | [composition-root-host-startup](composition-root-host-startup.md) |

`if (karo) new XForm().Show()` gibi **doğrudan üretim** yalnız composition-root dışında kalmaz; UI kodunda `new Form()` aranır ve 0 satır beklenir:

```powershell
# KANIT: UI katmaninda gizli form uretimi kalmadi mi?  -> 0 satir
Select-String -Path "src\**\*.cs" -Pattern "new\s+\w+Form\s*\(" |
  Where-Object { $_.Path -notmatch '\\Forms\\' }
```

---

## §5 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `ActivatorUtilities` / `IServiceProvider` | **NuGet**: `Microsoft.Extensions.DependencyInjection.Abstractions` (+ `...DependencyInjection`) | runtime ile birlikte gelir (paket referansı gerekmez — `NEEDS_VALIDATION`: bacak bazlı paket listesiyle doğrulanır) |
| `IServiceProvider.GetRequiredService<T>` | uzantı metod (paket) | aynı imza |
| Kurucu çözümleme davranışı | **aynı** | aynı |
| Dil | **C# 7.3**: `where T : Form` kısıtı var; `using var` yok | C# 14: `using var` yazılabilir — **ortak dosyada blok biçimi kalır** |
| `#if` | yalnız paket/API varlığı | — |

---

## §6 Kardesler

[form-showdialog-result](form-showdialog-result.md) · [form-single-instance-reuse](form-single-instance-reuse.md) · [di-container-lifetimes](di-container-lifetimes.md) · [form-navigation-patterns](form-navigation-patterns.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md)

---

## §7 Yasaklar

1. UI kodunda `new SomeForm()` — üretim yalnız factory üzerinden (tarama §4 = 0 satır).
2. Service locator / statik `ServiceProvider` erişimi (UI katmanında mimari RED).
3. Factory'nin form cache'lemesi; cache + Dispose sorumluluğu tek sahipte.
4. `.Form.cs`'e tasarım kodu (kontrol üretimi, `SuspendLayout`, `InitializeComponent` dışı) yazmak; `.resx`'i elle düzenlemek.
5. EF Core — veri erişimi **ADO.NET + Dapper**, sorgu `@p1` parametreli; kanıtsız "bitti" (G5) — kanıt: §4 tarama çıktısı + `dotnet build` (iki bacak).
