---
title: "mvvm-pattern-devexpress - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 MVVM deseni: ViewModelBase + DelegateCommand + Messenger, ince handler siniri, DI siniri ve iki bacak farki"
---

# mvvm-pattern-devexpress — DevExpress MVVM Kalıbı (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [designer-runtime-customization](designer-runtime-customization.md) · [dx-validation-provider](dx-validation-provider.md)

> **Tek cumlelik sorumluluk:** *DevExpress MVVM'de View, ViewModel ve servis sınırı nerede çizilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

MVVM üç katmandır: **View** (`.Designer.cs` + ince code-behind), **ViewModel** (duran veri + komut), **servis** (Dapper repository, dosya, ağ). DevExpress bunun için `ViewModelBase` (değişiklik bildirimi), `DelegateCommand` (komut) ve `Messenger` (ViewModel'ler arası haberleşme) verir. Kodda kalan tek şey **binding ve komut çağrısı**; iş servis katmanına gider (SOL-02). Handler'lar tek çağrı kalır (DX-13), `.resx`/Designer'e MVVM kodu yazılmaz (DX-10).

---

## §1 Katman Siniri

```
View (.Form.cs + .Designer.cs + .resx)
   |   binding / Command
   v
ViewModel (ViewModelBase)  --- Messenger ---> diger ViewModel
   |
   |   arayuz uzerinden (I*)
   v
Servis / Repository (ADO.NET + Dapper - EF Core YASAK)
```

| Katman | Ne taşır | Ne TAŞIMAZ |
|---|---|---|
| View | binding, komut çağrısı, olay yönlendirici | SQL, iş kuralı, ağ |
| ViewModel | durum, komut, doğrulama mesajı | `Control` referansı, `.resx` |
| Servis | sorgu (parameterized `@p1`), dosya | UI nesnesi, tema |

---

## §2 Anahtar API'ler

| API | Ne is yarar | Not |
|---|---|---|
| `ViewModelBase.GetValue<T>() / SetValue(...)` | özellik değişikliği + `PropertyChanged` | depolama hazır |
| `DelegateCommand` / `DelegateCommand<T>` | `ICommand` sarıcı | handler **ince** kalmalı |
| `Messenger.Default` | ViewModel'ler arası mesaj | abonelik `Dispose`'da |
| `RaisePropertyChanged(name)` | elle bildirim (yol gerekirse) | `CallerMemberName` |
| `ISupportSplashScreen` | uzun işte bekleme göstergesi | `RibbonForm` ile |

> **NEEDS_VALIDATION:** `AsyncCommand`/hazır async komut sınıfının v26.1 tam adı derleme ile doğrulanır; doğrulanana kadar async iş `Task` + `await` içinde, `async void` **yok**.

---

## §3 Kod (ViewModel + ince handler)

```csharp
using DevExpress.Mvvm;
using System.Windows.Input;

public class SessionViewModel : ViewModelBase
{
    public string SelectedName
    {
        get => GetValue<string>();          // ViewModelBase depolama
        set => SetValue(value);             // PropertyChanged yayini
    }

    public ICommand NewCommand => new DelegateCommand(OnNew);

    private void OnNew() => _sessions.Create(SelectedName);  // tek cagri (DX-13)
}
```

| | |
|---|---|
| **GÖRÜNÜMDE** | `gridControl1.DataBindings.Add(...)`, `button1.Command = vm.NewCommand` |
| **GÖRÜNÜMDE YOK** | SQL, `SqlConnection`, `MessageBox` ile iş kararı |
| **VIEWMODEL'DE YOK** | `Control`, `GridControl`, tema kodu |

---

## §4 Kurallar (en fazla 5 madde)

1. Binding **tek yönlü** kural: View → ViewModel; ViewModel asla View'ı çağırmaz.
2. `DelegateCommand` handler'ı **tek satır çağrı** (DX-13); akış serviste.
3. Repository çağrısı **arayüz üzerinden** (test edilebilirlik); EF Core **0** (G2).
4. Messenger abonelikleri `Dispose`'da kaldırılır (sızıntı yok).
5. MVVM kodu `.Designer.cs`'e girmez (DX-10); yapı Designer'da, bağlama `.Form.cs`'de.

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| MVVM assembly | v26.1 DevExpress paketi (net48 bacağı) | v26.1 aynı paketin net10 bacağı |
| C# | **7.3** — record, `using var`, target-typed `new` **yasak** | C# 14 yazılabilir; **ortak** `.cs` dosyaları 7.3'te kalır |
| Binding | WinForms `DataBindings` | aynı |
| DI | el ile kurulum (`Program.cs`) | `Host`/`IServiceCollection` — `#if` yalnız burada |

> **YASAK:** EF Core, `v26.1` dışı sürüm, `RibbonControlStyle.Office2010` dışı ribbon (G2 / DX-02 / DX-03).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Form/Designer sınırı | [designer-runtime-customization](designer-runtime-customization.md) |
| Doğrulama (View katmanı) | [dx-validation-provider](dx-validation-provider.md) |
| Veri kaynağı kalıbı (Dapper) | [gridcontrol-basics](gridcontrol-basics.md) |
| Paket/CPM | [nuget-references-cpm](nuget-references-cpm.md) |
