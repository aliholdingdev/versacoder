---
title: "Form Yasam Dongusu - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Form olusturma-gosterme-kapatma sirasi: ctor, Load, Shown, FormClosing/Closed, Dispose; ShowDialog vs Show; iki bacak (net48 / net10.0-windows) farklari"
---

# Form Yasam Dongusu (Form Lifecycle)

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bir `Form` ne zaman olusur, hangi olay hangi sirada gelir, hangisi iptal edilebilir ve kapanista ne dispose edilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Form yasami iki evrede calisir: **tasarim evresi** (`new Form()` + `InitializeComponent()`) ve **calisma evresi** (olaylar). Sira sabittir: `ctor` -> `Load` -> `Shown` -> `Activated` -> `FormClosing` -> `FormClosed` -> `Dispose`. `Load` yalnizca **ilk** gosterimden once bir kez yanar; `FormClosing` `e.Cancel = true` ile geri alinabilir, `FormClosed` geri alinamaz. `ShowDialog()` kapanisinda handle yok olur (ayni instance tekrar gosterilmez); `Show()` ile acilan formu **acan kapatir**, field olarak tutulmazsa gorunmezken ayakta kalir. Form ici veri yukleme `Load` icinde ve **ADO.NET + Dapper** ile yapilir (EF Core yasak, `CLAUDE.md` G2).

---

## §1 Yasam sirasi

| # | Asama | Tetikleyen | Iptal | Ne yapilir |
|---|---|---|---|---|
| 1 | `ctor` + `InitializeComponent()` | `new Form(...)` | - | Control uretimi, olay baglayicilari (bkz. [designer-cs-anatomy](designer-cs-anatomy.md)) |
| 2 | `OnHandleCreated` / `OnHandleDestroyed` | pencere handle'i | - | Handle'a bagli kaynak (timer, hook) |
| 3 | `Load` | ilk gosterimden ONCE, **1 kez** | - | Veri yukleme, varsayilan odak |
| 4 | `Shown` | ilk gosterimden SONRA | - | Animasyon, ilk odak, ipucu |
| 5 | `Activated` / `Deactivate` | odak degisimi (her gecis) | - | **Veri yukleme YAPMAZ**, yalnizca odak senkronu |
| 6 | `FormClosing` | X, `Close()`, uygulama kapanisi | **`e.Cancel = true`** | Kaydedilmis is var mi onayi |
| 7 | `FormClosed` | kapanis kesin | hayir | Kayit/dispose edilecek nesneleri temizle |
| 8 | `Dispose()` | `Close()` sonrasi | - | Handle + child control + `components` serbest |

```
ctor --> [handle] --> Load --> Shown --> (Activated <-> Deactivate)*
                                             |
                                             v
                                    FormClosing --(Cancel?)--> geri don
                                             |
                                             v
                                    FormClosed --> Dispose --> (GC)
```

---

## §2 ShowDialog vs Show

| Konu | `ShowDialog()` | `Show()` |
|---|---|---|
| Mod | modal (diger formlari bloklar) | modalsiz |
| Donus degeri | `DialogResult` | `void` |
| Kapanis | kapaninca handle yok olur, **ayni instance tekrar gosterilmez** | acan taraf kapatir |
| Kimlik | `Owner` + `StartPosition` ile sahibine gore konum | `Owner` verilirse ustte kalir |
| Tipik kullanim | ayar, ongoru, dogrulama penceresi | ana pencere altinda alt pencere |

**KURAL:** `ShowDialog()` sonrasi `DialogResult` okunur, nesne **tekrar** gosterilmez (yenisini `new`'lersiniz). `Show()` ile acilan her form bir field'ta tutulur ve `FormClosed` olayinda `null`'lanir; aksi halde kapatilmis form bellekte reference sayesiyle ayakta kalir.

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Baslangic cagrisi | `Application.EnableVisualStyles()` + `Application.SetCompatibleTextRenderingDefault(false)` + `Application.Run(new MainForm())` | `ApplicationConfiguration.Initialize()` (source-generated, MSBuild ozelliklerinden) |
| Dil varsayilani (LangVersion) | C# **7.3** | C# **14** (vault `latest` ile iki bacakta da 14 kullanilir) |
| Nullable | proje acilisinda `#nullable enable` elle gerekir | `<Nullable>enable</Nullable>` sablonu |
| `Task.WaitAsync` / `ArgumentNullException.ThrowIfNull` | **YOK** (paket/elle yazim) | var |
| `IAsyncDisposable` / `await using` | `Microsoft.Bcl.AsyncInterfaces` gerekir | yerlesik |

---

## §4 C# 7.3 (net48) vs C# 14 (net10)

```csharp
// --- C# 7.3 (net48 varsayilan dili) ---
public sealed class MainForm : Form
{
    private readonly ISessionService _sessions;
    private MainFormView _view;

    public MainForm(ISessionService sessions)
    {
        _sessions = sessions;
        InitializeComponent();
        _view = new MainFormView();          // tip elle yazilir
        Load += MainForm_Load;
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        Session session = null;              // nullable anotasyonu yok
        using (var cts = new CancellationTokenSource())
        {
            session = await _sessions.LoadAsync(cts.Token);
        }
        _view.Bind(session);
    }
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
public sealed partial class MainForm : Form
{
    private readonly ISessionService _sessions;
    private readonly MainFormView _view = new();

    public MainForm(ISessionService sessions)
    {
        _sessions = sessions;
        InitializeComponent();
        Load += MainForm_Load;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        Session? session = null;
        using var cts = new CancellationTokenSource();
        session = await _sessions.LoadAsync(cts.Token);
        _view.Bind(session);
    }
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynaktaki etkisi |
|---|---|---|---|
| Nullable anotasyonu | yok | `?` + NRT | `object? sender` yazmak 7.3'te de derlenir (anlamsiz uyari) |
| `using var` | `using (...) { }` | `using var ...` | 7.3'te blok zorunlu |
| Target-typed `new()` | yok | `new()` | 7.3'te tip yazilir |
| `partial` + file-scoped namespace | `partial` var, file-scoped yok | `namespace X;` zorunlu (CS-01) | vault `LangVersion=latest` -> her iki bacakta da 14 |

> **KURAL:** Vault `LangVersion` iki bacakta da `latest` tutar; 7.3 sutunu yalnizca **dil surumu sabitlenirse** ne kaybolacagini gosterir. `#if` **yalniz platform farkinda** (`coding-standards.md` CS-12); dil surumu ile kod bolunmez.

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `Load` her gosterimde tekrar | `Shown`'da `Load` yeniden calisiyor (form gizlenip acilinca) | `Shown` kullan ya da `bool _loaded` kilidi koy |
| `FormClosing`'de iptal unutulmus | Kullanici "evet" deyip kapanmayan pencere | `e.Cancel` sadece "hayir" yolunda set edilir |
| `FormClosed` = Dispose | Dispose'dan sonra hizmet cagrisi (ObjectDisposedException) | Temizligi `FormClosed`'da, kaynak serbest birakmayi `Dispose`/`using`'e birak |
| Modalsiz formun reference'i yok | GC kapatilmis formu tutuyor, olay hala dinliyor | field + `FormClosed` = `null` + unsubscribe |
| `Load` icinde string-concat SQL | `DA-03` ihlali (G2) | Dapper + parameterized (`@p1`), `CommandTimeout` setli |

> **Iliskili:** olay sirasi devami icin [controls-lifecycle-events](controls-lifecycle-events.md) · dosya yapisi icin [designer-cs-anatomy](designer-cs-anatomy.md) · kapatma onayi akisi icin [cancellation-in-ui](cancellation-in-ui.md).
