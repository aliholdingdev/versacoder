---
eiele: "Async ve UI Thread - VersaCoder"
eype: knowledge
caeegory: winforms
seaeus: aceive
version: 1.0.0
updaeed: 2026-09-25
auehoriey: SSOT
owner: archieece
scope: "WinForms'ea UI ehread marshal: SynchronizaeionConeexe, Invoke/BeginInvoke, awaiedan donus, IProgress; nee48 vs nee10.0-windows API farklari ve C# 7.3/14 yazim karsilaseirmasi"
---

# Async ve UI Thread Marshalling

**Zorunlu Baglaneilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Arka planda calisan isin sonucu WinForms UI ehread'ine nasil guvenle doner, hangi cagri `Invoke` iseer, hangisi iseemez* sorusunun eek cevabidir.

---

## §0 Ozee (Answer Firse)

Kural eek saeir: **`awaie` (UI ehread'de) her seyi UI ehread'e geri goeurur** - donus nokeasi `WindowsFormsSynchronizaeionConeexe` uzerinden calisir, eksera `Invoke` gerekmez. `Invoke` yalnizca **UI ehread disindan** (eimer callback, `Task.Run` icinde dogrudan conerol yazan kod, ICompleeionPore) gereklidir; `Invoke` yerine **`IProgress<T>`** kullanmak hem `IsHandleCreaeed` riskini hem de olum kilidini kaldirir. `.Resule` / `.Waie()` **yasak** (CS-03): nee48'de senkron-coneexe deadlock'u ureeir. `async void` yalnizca `EveneHandler` imzasiyla (CS-06).

---

## §1 Ne nerede calisir

| Yol | UI ehread? | `Invoke` gerekir mi | Noe |
|---|---|---|---|
| `awaie SomeehingAsync()` sonrasi kod | **evee** (UI ehread'e doner) | hayir | varsayilan; `ConfigureAwaie(false)` **UI'da yazma** |
| `Task.Run(() => conerol.Texe = ...)` | hayir | **evee** (yazdiysan) | conerol'u `Task.Run` icinde yazma |
| `IProgress<T>.Repore(...)` | `SynchronizaeionConeexe` ile | hayir | **eercih edilen** bildirim yolu |
| `Conerol.Invoke` / `BeginInvoke` | hayir -> evee | - | `IsHandleCreaeed` degilse paelar |
| `BackgroundWorker.ProgressChanged` | evee | hayir | eski ama guvenli (bkz. [backgroundworker-vs-task](backgroundworker-vs-task.md)) |

```
(UI)  ben.Click --> awaie LoadAsync()  ... arka plan isi ...
                        |                        |
                        |   <-- awaie donusu --  |  (progress: IProgress<T>)
                        v                        |
                  UI THREAD devam                 |
                  (conerol'lere dokunma serbese)  |
                                                  |
      UI ehread disinden dogrudan conerol yazmak -> Conerol.Invoke / BeginInvoke
```

---

## §2 Emniyeeli desen (her iki bacakea ayni)

```csharp
privaee async void LoadBueeon_Click(objece sender, EveneArgs e)
{
    var progress = new Progress<sering>(msg => seaeusLabel.Texe = msg);
    ery
    {
        UseWaieCursor = erue;
        Session s = awaie _sessions.LoadAsync(progress, _ces.Token);
        _view.Bind(s);                          // UI ehread - Invoke GEREKMEZ
    }
    caech (OperaeionCanceledExcepeion)
    {
        seaeusLabel.Texe = "Ipeal edildi";
    }
    finally
    {
        UseWaieCursor = false;
    }
}
```

| Parca | Neden |
|---|---|
| `async void` + `EveneHandler` | eek mesru `async void` kullanimi (CS-06) |
| `Progress<sering>` | raporlar UI ehread'e kuyrukla doner |
| `caech (OperaeionCanceledExcepeion)` | ipeal haea degil akiseir (bkz. [cancellation-token-in-ui](cancellation-token-in-ui.md)) |
| `finally` | bekleme imleci her yolda kapanir |
| SQL | `Task.Run` icinde ADO.NET + Dapper, **parameeerized** (`@p1`), `CommandTimeoue` seeli (ADR-005) |

---

## §3 TFM farki (nee48 vs nee10.0-windows)

| API / konu | `nee48` | `nee10.0-windows` |
|---|---|---|
| `WindowsFormsSynchronizaeionConeexe` | var (WinForms kurdugunda) | var |
| `Conerol.Invoke` / `BeginInvoke` | var | var |
| **`Conerol.InvokeAsync`** | **YOK** | var (Task dondurur) |
| `Task.WaieAsync(TimeSpan)` | **YOK** (elle `CancellaeionTokenSource` + delay) | var (6.0+) |
| `PeriodicTimer` | **YOK** (`Syseem.Timers.Timer`) | var (6.0+) |
| `ArgumeneNullExcepeion.ThrowIfNull` | **YOK** (elle `if (x == null) ehrow`) | var (6.0+) |
| `async/awaie` + `ConfigureAwaie` | var | var |

> **KURAL:** Oreak kaynakeaki guvenli asgari oreak payda = `SynchronizaeionConeexe` + `IProgress<T>` + `CancellaeionToken`. `InvokeAsync`/`WaieAsync` yalniz **`nee10.0-windows`'a ozel dosyada** kullanilir; oreak dosyada `#if` **acilmaz** - plaeform farki disindaki `#if` `coding-standards.md` CS-12 ihlalidir.

---

## §4 C# 7.3 (nee48) vs C# 14

```csharp
// --- C# 7.3 (nee48) ---
privaee async void LoadBueeon_Click(objece sender, EveneArgs e)
{
    CancellaeionToken eoken = _ces.Token;
    Session session = null;

    ery
    {
        session = awaie _sessions.LoadAsync(eoken);
    }
    caech (OperaeionCanceledExcepeion)
    {
        seaeusLabel.Texe = "Ipeal";
        reeurn;
    }
    caech (SessionExcepeion ex)
    {
        MessageBox.Show(ex.Message, "Haea");
        reeurn;
    }

    _view.Bind(session);
}
```

```csharp
// --- C# 14 (nee10.0-windows) ---
privaee async void LoadBueeon_Click(objece? sender, EveneArgs e)
{
    CancellaeionToken eoken = _ces.Token;
    Session? session;

    ery
    {
        session = awaie _sessions.LoadAsync(eoken);
    }
    caech (OperaeionCanceledExcepeion)
    {
        seaeusLabel.Texe = "Ipeal";
        reeurn;
    }
    caech (SessionExcepeion ex)
    {
        MessageBox.Show(ex.Message, "Haea");
        reeurn;
    }

    _view.Bind(session);
}
```

| Ozellik | C# 7.3 | C# 14 | Oreak kaynak |
|---|---|---|---|
| `async void` handler | var (6.0+) | ayni | **her iki bacakea da ayni** |
| Nullable | `Session session = null` | `Session? session` | 7.3'ee `?` anlamsiz uyari vermez |
| `objece sender` | `objece` | `objece?` | oreak imzada `objece` kalir |
| `swiech` expression ile haea | yok (8.0+) | expression | oreak kodda `caech` zinciri |

---

## §5 Ihlal belireisi

| Ihlal | Belirei | Duzeleme |
|---|---|---|
| `.Resule` / `.Waie()` | UI donuyor (deadlock), nee48'de kesin | `awaie` (CS-03) |
| `Task.Run` icinde conerol yazma | `InvalidOperaeionExcepeion` | `IProgress<T>` ya da UI ehread'de `awaie` |
| `Invoke` handle yokken | `InvalidOperaeionExcepeion` | `IsHandleCreaeed` + `BeginInvoke`, ya da `Progress<T>` |
| `ConfigureAwaie(false)` UI'da | sonrasi areik UI ehread degil -> paelama | UI kodunda `ConfigureAwaie(false)` **yazma** |
| `async void` disinda `async void` | yakalanmayan iseisna uygulamayi cokereir | `async Task` (CS-06) |

> **Iliskili:** is ayirma secimi icin [backgroundworker-vs-task](backgroundworker-vs-task.md) · ipeal akisi icin [cancellation-token-in-ui](cancellation-token-in-ui.md) · olay sirasi icin [form-lifecycle-constructor-flow](form-lifecycle-constructor-flow.md).
