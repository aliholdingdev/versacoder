---
title: "exception-boundary-ui - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms'da uc kuresel hata yakalayici (ThreadException, UnhandledException, UnobservedTaskException), kullaniciya gosterim ve net48 + net10.0-windows davranis farklari"
---

# exception-boundary-ui — Kuresel hata siniri (3 yakalayici)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [testing-rules](../../.rules/testing-rules.md) · [logging-integration](logging-integration.md) · [startup-pipeline-ui](startup-pipeline-ui.md)

> **Tek cumlelik sorumluluk:** *Yakalanmayan hata uygulamayi nasil durdurur, kullaniciya ne gosterilir, kayit nereye dusurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Uygulama **uc noktada** hata yakalar ve hepsi **ayni tek yere** (log + tek hata ekrani) gider: UI thread (`Application.ThreadException`), process geneli (`AppDomain.CurrentDomain.UnhandledException`), task geneli (`TaskScheduler.UnobservedTaskException`). **Hata yutulmaz**: yakalayici islettikten sonra is akisina devam etmez / uygulamayi guvenli kapatir; "devam et" modu yalniz geri alinabilir tek is icin. Kullaniciya gosterilen metin **insan dili + hata kodu**, teknik stacak yalniz loga gider. Kurulum sirasi onemlidir: **handler'lar `Main`'in ilk satirlarinda**, ilk form olusturulmadan once baglanir.

---

## §1 Uc yakalayici

| Yakalayici | Ne yakalar | Tipik kaynak | Sonrasinda |
|---|---|---|---|
| `Application.ThreadException` | UI thread hatalari | olay handler, veri binding | log + dialog; **devam mumkunse** devam |
| `AppDomain.CurrentDomain.UnhandledException` | islenmemis, genelde olumcul | olcu disi thread, reflection | log + guvenli kapat (`Environment.FailFast` secenegi) |
| `TaskScheduler.UnobservedTaskException` | gorulmemis task hatalari | fire-and-forget `async` | log + `SetObserved()` |

> **UYARI:** `ThreadException` yalniz **UI thread**'i kapsar; arkadan gelen `Task` hatalari oraya dusmez. Ikisini ayni handler'a baglamis gibi davranmak, sessiz kayip uretir.

## §2 Kurulum ve tek cikti noktasi (kod)

```csharp
[STAThread]
static void Main()
{
    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    Application.ThreadException += OnUiError;
    AppDomain.CurrentDomain.UnhandledException += OnFatal;
    TaskScheduler.UnobservedTaskException += OnObservedLate;

    // ... baslatma pipe'i (startup-pipeline-ui §1)
    Application.Run(new MainForm());
}

static void OnUiError(object sender, ThreadExceptionEventArgs e)
{
    Logger.Error("UI", e.Exception);              // once kayit
    MessageBox.Show(UserText(e.Exception),        // sonra insan dili
        "VersaCoder", MessageBoxButtons.OK, MessageBoxIcon.Error);
    // yutma YOK: hata loglandi, kullanici bilgilendirildi, akis durdu
}
```

| Katman | Kullaniciya gosterilen | Loga giden |
|---|---|---|
| Beklenen is hatasi | kisa mesaj + cozum onerisi | `WARN` + baglam |
| Beklenmeyen hata | "Beklenmeyen hata (E-1041)" | `ERROR` + tam stacak |
| Olumcul hata | "Uygulama kapatiliyor" + log yolu | `FATAL` + stacak |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `Application.ThreadException` | ayni API | ayni API |
| `SetUnhandledExceptionMode` | `CatchException` varsayilan degil - **acikca cagir** | ayni cagri gerekir |
| `UnhandledException` bilgisi | `Exception` + `IsTerminating` | `Exception` + `TerminatingProcess` |
| Olumcul hata davranisi | log + kapat; `FailFast` bayrakla | ayni - fark **log yolu degil**, process modeli |
| C# dili | **7.3** - `async`/`await` var; `IAsyncEnumerable` yok | C# 14 |

`#if` bu tabloda **gerekmez**; fark davranis dokumani seviyesindedir (G6: `#if` yalniz platform/API farkinda).

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `catch { }` bos blok | kayip hata, G5/G9 riski | logla + gerekirse goster |
| 2 | `catch (Exception)` + devam | bozuk durumla is yapma | durumu dogrula ya da kapat |
| 3 | Hata mesajinda ham SQL/stack | kullaniciyi bogar + siza | kod + kisa metin; stack loga |
| 4 | Handler'i ilk formdan sonra baglamak | ilk hata kaçar | `Main`'in ilk satirlari |
| 5 | Fire-and-forget `async void` | `UnobservedTaskException` | `async Task` + takip (command-pattern-mvvm) |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Log hatti | [logging-integration](logging-integration.md) |
| Acilis sirasi (handler nereye) | [startup-pipeline-ui](startup-pipeline-ui.md) |
| Async komutlarin hata politikasi | [command-pattern-mvvm](command-pattern-mvvm.md) |
| Test kaniti | [index](../index.md) |
