---
title: "startup-pipeline-ui - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Program.cs acilis boru hatti: hata handler, tek ornek kilifi, DPI/gorsel baslangic, ayar+log kurulumu, kabuk olusturma ve net48 + net10.0-windows farklari"
---

# startup-pipeline-ui — `Program.cs` acilis boru hatti

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [exception-boundary-ui](exception-boundary-ui.md) · [single-instance-app](single-instance-app.md)

> **Tek cumlelik sorumluluk:** *`Main` icinde islemler hangi sirla yapilir, sira bozulursa ne kirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Acilis sirasi **sabit 5 adimdir** ve siranin disina cikmak sessiz hata uretir: (1) kuresel hata handler, (2) tek ornek kilifi, (3) DPI + gorsel baslangic, (4) log + ayar yukleme, (5) kabuk formu `Application.Run`. Handler'lar **ilk** olmalidir; kilif, form olusturulmadan **once** baslamalidir; gorsel baslangic, herhangi bir `Control` new'lenmeden **once** cagrilmalidir. Tek platform farki 3. adimdir: `net48` iki klasik cagri, `net10.0-windows` tek `ApplicationConfiguration.Initialize()` - `#if` yalnizca burada (G6).

---

## §1 Boru hatti (5 adim)

| # | Adim | net48 | net10.0-windows |
|---|---|---|---|
| 1 | Hata handler | `Application.ThreadException` + `AppDomain.UnhandledException` + `TaskScheduler.UnobservedTaskException` | ayni |
| 2 | Tek ornek kilifi | named `Mutex` + pipe dinleyici | ayni (`single-instance-app`) |
| 3 | DPI + gorsel | `EnableVisualStyles()` + `SetCompatibleTextRenderingDefault(false)` | `ApplicationConfiguration.Initialize()` |
| 4 | Log + ayar | `Trace` listener + `ISettingsStore.Load()` | ayni (`logging-integration`, `settings-user-config`) |
| 5 | Kabuk | `Application.Run(new MainForm())` | ayni |

```csharp
[STAThread]
static void Main()
{
    Application.ThreadException += OnUiError;            // (1)
    AppDomain.CurrentDomain.UnhandledException += OnFatal;

    bool createdNew;                                     // (2)
    var mutex = new Mutex(false, @"Local\VersaCoder.SingleInstance", out createdNew);
    if (!createdNew) { NotifyFirstInstance(); return; }

#if NET48                                                 // (3)
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
#else
    ApplicationConfiguration.Initialize();
#endif

    Logger.Init();                                       // (4)
    var settings = Settings.Load();

    try { Application.Run(new MainForm(settings)); }      // (5)
    finally { mutex.ReleaseMutex(); mutex.Dispose(); }
}
```

> **YASAK:** `#if` icine is mantigi koymak (G6) · handler'lari 5. adima koymak (ilk hata kacar) · `Run`'dan sonra yeni form olusturmak (kabuk disi ikinci kabuk).

## §2 Sira ihlali belirtileri

| Sira hataliysa | Belirti | Duzeltme |
|---|---|---|
| (1) gec | ilk hata penceresiz dusuyor | handler'lari en uste |
| (2) gec | iki kabuk ayni anda | kilif, formdan once |
| (3) gec | DPI kaymasi / renk farki | herhangi bir `Control`'den once |
| (4) gec | acilis hatalari logsuz | `Run`'dan once |
| (5) gec | `Main` disinda ikinci `Run` | tek `Run` |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| Gorsel baslangic | 2 klasik cagri (`#if NET48`) | `ApplicationConfiguration.Initialize()` |
| `[STAThread]` | zorunlu | zorunlu |
| Binding redirect | `App.config` 26.1 (DX-06) | gerekmez |
| Klasik config (`ConfigurationManager`) | kutuda | paket ister |
| C# dili | **7.3** - `out var` yasak | C# 14 (ayni dosya 7.3'te) |

DevExpress versiyonu ve ribbon stili acilista **degistirilmez**: `Directory.Packages.props` 26.1 + `RibbonControlStyle.Office2010` (ADR-004, G3).

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Sira disina cikmak | gizli DPI/hata sorunu | §1 sablonu |
| 2 | `#if` icinde is mantigi | bakim imkansizi (G6) | yalniz platform cagrisi |
| 3 | Handler'larin olmemesi | yakalanmayan ilk hata | [exception-boundary-ui](exception-boundary-ui.md) |
| 4 | Kilifsiz acilis | cift kabuk | [single-instance-app](single-instance-app.md) |
| 5 | Acilista agir is (ag, db) | beyaz ekran + donme | splash + arka plan is |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Adim 1 detayi | [exception-boundary-ui](exception-boundary-ui.md) |
| Adim 2 detayi | [single-instance-app](single-instance-app.md) |
| Adim 4 detayi (log / ayar) | [logging-integration](logging-integration.md) |
| Adim 5 kabugu | [ribbonform-basics](ribbonform-basics.md) |
