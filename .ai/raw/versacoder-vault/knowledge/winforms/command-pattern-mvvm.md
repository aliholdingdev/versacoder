---
title: "command-pattern-mvvm - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "El yazimi hafif MVVM komutlari: RelayCommand + AsyncRelayCommand, CanExecute yonetimi, hata politikasi ve net48 + net10.0-windows farklari (DevExpress MVVM degil)"
---

# command-pattern-mvvm — El yazimi komutlar (RelayCommand)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [viewmodel-inotify](viewmodel-inotify.md) · [messenger-eventaggregator](messenger-eventaggregator.md) · [exception-boundary-ui](exception-boundary-ui.md)

> **Tek cumlelik sorumluluk:** *Buton/menu eylemi ViewModel'e nasil gider, CanExecute ne zaman yenilenir, async hata nereye duser* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Komut **el yazimidir**: `RelayCommand` (senkron) + `AsyncRelayCommand` (async, iptal destekli) - DevExpress Mvvm paketine bagimlilik yok (G8'siz). WinForms'ta **`CommandManager` yoktur** (WPF ozelligi): `CanExecuteChanged` el ile tetiklenir; buton `Enabled` durumu o an hesaplanir ve deger degistiginde `RaiseCanExecuteChanged()` cagrilir. `async void` **yalniz olay baglayicida** ve icinde `try/catch` vardir; hata yutulmaz -> log + hata siniri (exception-boundary-ui). Buton/XAML `ICommand` baglamasi WinForms'ta `Button.Tag` + tek genel handler ya da olay -> komut yonlendiricisi ile yapilir.

---

## §1 Komut siniflari

| Sinif | Ne yapar | Ne zaman |
|---|---|---|
| `ICommand` | sozlesme: `Execute` / `CanExecute` / `CanExecuteChanged` | buton, menu, klavye |
| `RelayCommand` | delegeyi `Execute`'a baglar | senkron is |
| `AsyncRelayCommand` | `async Task` yurutur, calisirken `CanExecute=false` | ag/dosya islemi |
| `RelayCommand<T>` | parametre alir (secili satir) | satir bazli eylem |

```csharp
public sealed class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute; _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter) { _execute(); }

    public event EventHandler CanExecuteChanged;          // WinForms: el ile
    public void RaiseCanExecuteChanged()
    {
        var h = CanExecuteChanged;
        if (h != null) h(this, EventArgs.Empty);
    }
}
```

> **KURAL:** `CanExecuteChanged` **UI thread'inde** ve **durum degistiginde** kaldirilir; kaldirilmazsa buton silik kalir - en sik gorulen hata.

## §2 Async komut + hata politikasi

```csharp
public ICommand KaydetCmd { get; private set; }

KaydetCmd = new AsyncRelayCommand(KaydetAsync, () => !Busy && IsDirty);

async Task KaydetAsync()
{
    Busy = true; KaydetCmd.RaiseCanExecuteChanged();
    try { await _repository.SaveAsync(_model, _cts.Token); IsDirty = false; }
    catch (OperationCanceledException) { /* iptal: bilgi mesaji */ }
    catch (Exception ex) { Logger.Error("kayit", ex); HataGoster(ex); }  // yutma YOK
    finally { Busy = false; KaydetCmd.RaiseCanExecuteChanged(); }
}
```

| Durum | Davranis |
|---|---|
| Calisirken tekrar tik | `CanExecute=false` -> buton pasif |
| Iptal | `CancellationToken` + `OperationCanceledException` ayri ele alinir |
| Hata | log + kullanici mesaji; akis `finally` ile normale doner |
| Is bitti | `RaiseCanExecuteChanged()` zorunlu |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `ICommand` | `System.Windows.Input` - kutuda | ayni - kutuda |
| `async` / `await` | C# 7.3 + `Microsoft.Bcl.Async` **gerekmez** (`net48` yeterli) | kutuda |
| `CancellationToken` | ayni | ayni |
| Nullable anotasyonu | **yok** (dil 7.3) | ayni dosya 7.3'te kaldigi icin kullanilmaz |
| DevExpress Mvvm | paket ister = **G8** | paket ister = **G8** |

**DevExpress Mvvm notu (kisa):** kütüphanede `DelegateCommand`, `MvvmHelper`, `ViewModelSource` hazir gelir; bu vault'ta **el yazimi tercih edilir** (daha az bagimlilik, net48/net10 farki olmadan ayni kod). Paket eklemek `architect` + kullanici onayi ister (G8).

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `RaiseCanExecuteChanged()` unutmak | pasif kalan buton | §2 `finally` |
| 2 | Komut icinde `async void` | yakalanmayan hata | `AsyncRelayCommand` + catch |
| 3 | Is mantigini komut ctor'una gommek | test edilemez | servise cagri |
| 4 | DevExpress Mvvm paketi eklemek (onaysiz) | G8 ihlali | el yazimi §1 |
| 5 | `CanExecute` icinde agir is | UI donmasi | sadece durum kontrolu |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Durum bildirimi (`INotifyPropertyChanged`) | [viewmodel-inotify](viewmodel-inotify.md) |
| Ekranlar arasi olay | [messenger-eventaggregator](messenger-eventaggregator.md) |
| Hata siniri | [exception-boundary-ui](exception-boundary-ui.md) |
| Ribbon'dan tetikleme | [ribbonform-basics](ribbonform-basics.md) |
