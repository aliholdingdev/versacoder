---
title: "form-navigation-patterns - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms'da ekranlar arasi gecis: modal/modeless Show, tek pencere + panel navigasyonu, form havuzu ve iki bacakta (net48 + net10.0-windows) acilis farklari"
---

# form-navigation-patterns — Ekranlar arasi gecis (modal / modeless / panel)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ribbonform-basics](ribbonform-basics.md) · [startup-pipeline-ui](startup-pipeline-ui.md)

> **Tek cumlelik sorumluluk:** *Bir ekrandan baska ekrana hangi yolla gecilir, form ne zaman acilir ne zaman gizlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault'ta navigasyonun varsayilani **tek `RibbonForm` kabugu + panel gecisidir** (`DockManager` / `XtraTabControl` icinde); ayri pencere **istisnadir**. Modal isler (`ShowDialog`) yalniz geri donus zorunluysa (onay, ayar, dosya secimi), modeless isler (`Show`) paralel calisan arka plan ekranlari icin kullanilir. Bir tip icin **tek ornek** havuz tutulur; ayni form iki kez `new` edilirse veri tutarsizligi dogar. Form kapaninca **dispose** edilir, gizleniyorsa dispose edilmez (`Hide`).

---

## §1 Gecis modeli secimi

| Model | Ne zaman | Cagri | Dispose |
|---|---|---|---|
| Panel gecisi (kabuk icinde) | IDE benzeri ekranlar arasi (varsayilan) | kabuk servisi `ShowView(name)` | panel gecildiginde |
| Modal dialog | onay / ayar / dosya secimi | `form.ShowDialog(this)` | kapaninca (`using`) |
| Modeless pencere | paralel yardimci ekran | `form.Show(this)` | `FormClosed`'da |
| Tray'e gizleme | pencere kapatilinca | `Hide()` + `ShowInTaskbar=false` | uygulama cikisinda |

> **KURAL:** Sahip pencere (`owner`) her zaman gecilir; sahipsiz modeless pencere, sahibinin onunde kalir ve kilitlenmis gibi gorunur.

## §2 Tek ornek havuzu (kod)

```csharp
private readonly Dictionary<Type, Form> _pool = new Dictionary<Type, Form>();

public void ShowView<T>() where T : Form, new()
{
    Form f;
    if (!_pool.TryGetValue(typeof(T), out f) || f.IsDisposed)
    {
        f = new T();
        f.FormClosed += delegate(object s, FormClosedEventArgs e)
        {
            _pool.Remove(typeof(T));   // dispose sonrasi havuzdan dus
        };
        _pool[typeof(T)] = f;
    }

    f.Show(this);
    f.BringToFront();
}
```

Modal ornek (kisa omurlu form icin `using`):

```csharp
using (var dlg = new AyarForm())
{
    if (dlg.ShowDialog(this) == DialogResult.OK)
        _settings.Save(dlg.Result);
}
```

> **UYARI:** `new T()` siniflar parametresiz kurucu ister; agir bagimliliklari kurucuya degil `InitializeComponent` sonrasi bir `Initialize(IServiceLocator)` metoduna koyun.

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| Kabuk baslangici | `Application.EnableVisualStyles()` + `SetCompatibleTextRenderingDefault(false)` + `Run(mainForm)` | `ApplicationConfiguration.Initialize()` + `Run(mainForm)` (`#if` platform farki) |
| `Show` / `ShowDialog` | ayni API | ayni API |
| DPI koordinatlari | manifest yoksa isletim sistemi olcekleme devreye girer, `PointToClient` sonucu kayabilir | high-DPI varsayilan acik |
| Kultur / `.resx` | `App.config` `uiCulture` | `CurrentUICulture` runtime ayari |
| C# dili | **7.3** - `using` deyimi, local fonksiyon var; `using var` yok | C# 14 |

`#if` yalnizca ilk satirdaki baslangic farkinda; geri kalan navigasyon kodu ortaktir (G6).

## §4 Sik yapilan hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Her tiklamada `new Form()` | handle sizintisi, ayni ekran 2 kez acilir | §2 havuzu |
| 2 | `Hide()` yerine `Close()` | geri acilista form disposed | gizle -> `Show()` |
| 3 | `ShowDialog()` sonucu okunmadan devam | yarim kalmis is akisi | `DialogResult` kontrolu |
| 4 | Sahipsiz modeless pencere | onde kalir, gorunmez | `Show(owner)` |
| 5 | Navigasyon kodunu handler'a gommek | code-behind 200+ satir (SOL-02) | kabuk navigasyon servisi |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Ribbon kabuğunun kurulumu | [ribbonform-basics](ribbonform-basics.md) |
| Ayarlarin saklanmasi | [settings-user-config](settings-user-config.md) |
| Tray'e gizleme | [tray-icon-lifecycle](tray-icon-lifecycle.md) |
| Tek ornek kilidi | [single-instance-app](single-instance-app.md) |
