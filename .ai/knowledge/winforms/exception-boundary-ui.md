---
title: "exception-boundary-ui - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "Uygulamadaki uc kuresel hata yakalayicisini, kullaniciya gosterim politikasini ve iki bacak (net48 / net10.0-windows) davranis farklarini tanimlar."
---

# Kuresel Hata Siniri (3 Yakalayici)

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [logging-integration](logging-integration.md) · [composition-root-host-startup](composition-root-host-startup.md) · [async-void-event-handlers](async-void-event-handlers.md) · [error-handling-rules](../../.rules/error-handling-rules.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (winforms · 4/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Yakalanmayan hata uygulamayi nasil durdurur, kullaniciya ne gosterilir, kayit nereye dusurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Uygulama **uc noktada** hata yakalar ve hepsi **ayni tek yere** (log + tek hata ekrani) gider: UI thread (`Application.ThreadException`), process geneli (`AppDomain.CurrentDomain.UnhandledException`), task geneli (`TaskScheduler.UnobservedTaskException`). **Hata yutulmaz**: yakalayici islettikten sonra is akisina devam etmez / uygulamayi guvenli kapatir; "devam et" modu yalniz geri alinabilir tek is icin. Kullaniciya gosterilen metin **insan dili + hata kodu**, teknik stacak yalniz loga gider. **Handler'lar `Main`'in ilk satirlarinda**, ilk form olusturulmadan once baglanir.

```text
+------------------------------------------------------------------+
| UI thread     : Application.ThreadException  -> log + dialog     |
| process       : UnhandledException           -> log + guvenli kapatis |
| task          : UnobservedTaskException       -> log + SetObserved |
| hepsi         : AYNI cikti -> log + tek hata ekrani              |
| kurulum       : Main'in ilk satirlari, ilk formdan ONCE          |
+------------------------------------------------------------------+
```

---

## §1 Uc yakalayici

| Yakalayici | Ne yakalar | Tipik kaynak | Sonrasinda |
|---|---|---|---|
| `Application.ThreadException` | UI thread hatalari | olay handler, veri baglama | log + dialog; **devam mumkunse** devam |
| `AppDomain.CurrentDomain.UnhandledException` | islenmemis, genelde olumcul | olcu disi thread, reflection | log + guvenli kapat (`Environment.FailFast` secenegi) |
| `TaskScheduler.UnobservedTaskException` | gorulmemis task hatalari | fire-and-forget `async` | log + `SetObserved()` |

> **UYARI:** `ThreadException` yalniz **UI thread**'i kapsar; arkadan gelen `Task` hatalari oraya dusmez. Ikisini ayni handler'a baglamis gibi davranmak, sessiz kayip uretir.

---

## §2 Kurulum ve tek cikti noktasi

```csharp
[STAThread]
static void Main()
{
    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    Application.ThreadException += OnUiError;
    AppDomain.CurrentDomain.UnhandledException += OnFatal;
    TaskScheduler.UnobservedTaskException += OnObservedLate;

    // ... baslatma pipe'i (bkz. composition-root-host-startup)
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

### §2.1 Kullaniciya vs loga

| Katman | Kullaniciya gosterilen | Loga giden |
|---|---|---|
| Beklenen is hatasi | kisa mesaj + cozum onerisi | `WARN` + baglam |
| Beklenmeyen hata | "Beklenmeyen hata (E-1041)" | `ERROR` + tam stacak |
| Olumcul hata | "Uygulama kapatiliyor" + log yolu | `FATAL` + stacak |

---

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Application.ThreadException` | ayni API | ayni API |
| `SetUnhandledExceptionMode` | `CatchException` varsayilan degil - **acikca cagir** | ayni cagri gerekir |
| `UnhandledException` bilgisi | `Exception` + `IsTerminating` | `Exception` + `TerminatingProcess` |
| Olumcul hata davranisi | log + kapat; `FailFast` bayrakla | ayni - fark **log yolu degil**, process modeli |
| C# dili | **7.3** - `async`/`await` var; `IAsyncEnumerable` yok | C# 14 |

`#if` bu tabloda **gerekmez**; fark davranis dokumani seviyesindedir (`#if` yalniz platform / API farkinda).

---

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `catch { }` bos blok | kayip hata | logla + gerekirse goster |
| 2 | `catch (Exception)` + devam | bozuk durumla is yapma | durumu dogrula ya da kapat |
| 3 | Hata mesajinda ham SQL / stack | kullaniciyi bogar + siza | kod + kisa metin; stack loga |
| 4 | Handler'i ilk formdan sonra baglamak | ilk hata kacar | `Main`'in ilk satirlari |
| 5 | Fire-and-forget `async void` | `UnobservedTaskException` | `async Task` + takip |

---

## §5 Kim neyi degistirir

| Kim | Ne yapar | Kanit |
|---|---|---|
| `developer` | `Main` icine uc handler bagini sirayla koyar | kod incelemesi |
| `reviewer` | bos `catch { }` ve "devam et" modunu tarar | grep hedefi: 0 eslesme |
| `tester` | beklenmeyen + olumcul iki yolu sinar | test ciktisi |
| `logging-rules` sahibi | `WARN` / `ERROR` / `FATAL` seviye eslesmesi | log ornegi |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Uc handler da `Main`'in ilk satirlarinda bagli | kod incelemesi |
| 2 | Tek cikti noktasi (log + tek ekran) | kod incelemesi |
| 3 | Kullaniciya gosterilen metinde ham stacak yok | kod incelemesi |
| 4 | `SetUnhandledExceptionMode(CatchException)` acikca cagrilmis | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\exception-boundary-ui.md` (kaynak vault) | uc yakalayici tablosu, Main kurulum kodu, kullanici/log katman tablosu, iki bacak farki, yapilmayanlar |
| `.rules\error-handling-rules.md` | yutma yasagi ve hata gosterim kurali |
| `.rules\logging-rules.md` | seviye (WARN / ERROR / FATAL) ve stacak kaydi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
