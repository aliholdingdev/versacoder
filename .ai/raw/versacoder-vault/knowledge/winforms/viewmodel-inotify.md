---
title: "viewmodel-inotify - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "El yazimi ObservableObject/INotifyPropertyChanged temeli, WinForms DataBindings ile guncelleme ve net48 + net10.0-windows farklari (DevExpress MVVM degil)"
---

# viewmodel-inotify — El yazimi ObservableObject (INotifyPropertyChanged)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [command-pattern-mvvm](command-pattern-mvvm.md) · [settings-user-config](settings-user-config.md) · [form-navigation-patterns](form-navigation-patterns.md)

> **Tek cumlelik sorumluluk:** *Bir ViewModel ozelligi degistiginde ekranda nasil guncellenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Temel sinif tek satirdir: `SetProperty(ref _alan, deger)` -> deger **gercekten degistiyse** `PropertyChanged` yayinlar, `[CallerMemberName]` ile alan adi elle yazilmaz. WinForms'ta ViewModel'i ekrana baglamanin iki yolu vardir: **`Control.DataBindings.Add`** (tek tek, `DataSourceUpdateMode.OnPropertyChanged`) ya da ozellik degisince **el ile** kontrole deger yazmak (karma binding'de). DevExpress Mvvm kullanilmaz; `ViewModelSource`/`GenerateViewModel` yerine el yazimi (G8'siz, iki bacakta ayni kod). ViewModel **`Control` bilmez** - yalniz durum + komut + event yayar.

---

## §1 Temel sinif (kod)

```csharp
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void Raise(string name)
    {
        var h = PropertyChanged;
        if (h != null) h(this, new PropertyChangedEventArgs(name));
    }

    protected bool SetProperty<T>(ref T field, T value, string name)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        Raise(name);          // ayni degerde gereksiz cizim yok
        return true;
    }
}

public sealed class AyarViewModel : ObservableObject
{
    private string _tema = "Açık";
    public string Tema
    {
        get { return _tema; }
        set { SetProperty(ref _tema, value); }   // CallerMemberName: "Tema"
    }
}
```

> **KURAL:** Ayni degerde `PropertyChanged` **yayilmaz**; yayarsan `DataBindings` dongusu olusur ve UI donar.

## §2 WinForms'a baglama

| Yol | Nerede | Ne zaman |
|---|---|---|
| `control.DataBindings.Add("Text", vm, "Tema", false, DataSourceUpdateMode.OnPropertyChanged)` | form kurulumu | tek yon + geri yazim |
| Olay -> komut (`command-pattern-mvvm`) | `Click` handler | eylem |
| `PropertyChanged` -> el ile yazim | tek genel handler | karma/ozel kontroller |

```csharp
public MainForm()
{
    InitializeComponent();
    var vm = new AyarViewModel();
    cmbTema.DataBindings.Add("SelectedItem", vm, "Tema", false,
        DataSourceUpdateMode.OnPropertyChanged);
    btnKaydet.DataBindings.Add("Enabled", vm, "IsDirty");
}
```

| Yapilmayan | Sonuc |
|---|---|
| ViewModel'de `Control` alan tutmak | test edilemez, dongu |
| Binding'i her `Show()`'da tekrar eklemek | ayni binding 2 kez = cift yazim |
| Alan adini elle yazmak (`"Tema"`) | refactor'de kirilir - `[CallerMemberName]` |
| Her degisiklikte `Refresh()` | tum form yeniden cizilir |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `INotifyPropertyChanged` | `System.ComponentModel` - kutuda | ayni - kutuda |
| `[CallerMemberName]` | kutuda (C# 5+) | ayni |
| `Control.DataBindings` | `System.Windows.Forms` - ayni | ayni API |
| Nullable (`#nullable`) | **yok** (dil 7.3) | ayni kaynak 7.3'te kalir |
| DevExpress Mvvm | paket = **G8** | paket = **G8** |

`#if` **gerekmez**; API iki bacakta birebir aynidir. Fark yalniz dil surumunde: `??=` / `using var` / target-typed `new` **yazilmaz**.

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Degisiklikten sonra `Raise` cagirmamak | ekran gec guncellenir | `SetProperty` icinde |
| 2 | Binding'i kodda gizli yere koymak | neden guncellenmiyor bulunamaz | form kurulumunda tek yer |
| 3 | ViewModel icinde `MessageBox.Show` | test edilemez | event -> kabuk |
| 4 | DevExpress Mvvm paketi eklemek (onaysiz) | G8 ihlali | el yazimi §1 |
| 5 | Ayni degerde tekrar yayim | binding dongusu | esitlik kontrolu |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Eylem tarafı (komut) | [command-pattern-mvvm](command-pattern-mvvm.md) |
| Ayar ViewModel'inin kalıcılığı | [settings-user-config](settings-user-config.md) |
| Ekranlar arasi olay | [messenger-eventaggregator](messenger-eventaggregator.md) |
| Form baglama noktasi | [form-navigation-patterns](form-navigation-patterns.md) |
