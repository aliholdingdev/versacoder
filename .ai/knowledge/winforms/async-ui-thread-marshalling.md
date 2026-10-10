---
title: "async-ui-thread-marshalling - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Arka planda calisan isin sonucunun UI thread'e donusu: SynchronizationContext, Invoke/BeginInvoke, IProgress, .Result/.Wait yasagi ve iki bacak API farklari"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Async ve UI Thread Marshalling

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/async-rules|async-rules]] · [[.rules/error-handling-rules|error-handling-rules]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 3/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]]

> **Tek cumlelik sorumluluk:** *Arka plandaki isin sonucu UI thread'e nasil guvenle doner, hangi cagri `Invoke` ister* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kural tek satir: **`await` (UI thread'de) her seyi UI thread'e geri goturur** - donus noktasi `WindowsFormsSynchronizationContext` uzerinden calisir, eksera `Invoke` gerekmez. `Invoke` yalnizca **UI thread disindan** (timer callback, `Task.Run` icinde dogrudan kontrol yazan kod) gereklidir; onun yerine **`IProgress<T>`** tercih edilir - hem `IsHandleCreated` riskini hem de olum kilidini kaldirir. `.Result` / `.Wait()` **yasaktir**: `net48`'de senkron-kilit deadlock uretir. `async void` yalnizca `EventHandler` imzasiyla kullanilir.

```text
+---------------------------------------------------------------+
| (UI)  btn.Click --> await LoadAsync()   ... arka plan isi ... |
|                         |                                     |
|                         +-- await donusu --> UI THREAD devam  |
|                         |   (kontrollere dokunma serbest)     |
|                         +-- ilerleme: IProgress<T> (kuyruk)   |
|                                                                   |
| UI thread disinden kontrol yazmak -> Control.Invoke/BeginInvoke |
+---------------------------------------------------------------+
```

---

## §1 Ne nerede calisir

| Yol | UI thread? | `Invoke` gerekir mi | Not |
|---|---|---|---|
| `await SomethingAsync()` sonrasi kod | **evet** (UI thread'e doner) | hayir | varsayilan; `ConfigureAwait(false)` **UI'da yazma** |
| `Task.Run(() => kontrol.Text = ...)` | hayir | **evet** (yazdiysan) | kontrol'u `Task.Run` icinde yazma |
| `IProgress<T>.Report(...)` | `SynchronizationContext` ile | hayir | **tercih edilen** bildirim yolu |
| `Control.Invoke` / `BeginInvoke` | hayir -> evet | - | `IsHandleCreated` degilse patlar |
| `BackgroundWorker.ProgressChanged` | evet | hayir | eski ama guvenli |

---

## §2 Emniyetli desen (iki bacakta ayni)

```csharp
private async void LoadButton_Click(object sender, EventArgs e)
{
    var progress = new Progress<string>(msg => statusLabel.Text = msg);
    try
    {
        UseWaitCursor = true;
        Session s = await _sessions.LoadAsync(progress, _cts.Token);
        _view.Bind(s);                          // UI thread - Invoke GEREKMEZ
    }
    catch (OperationCanceledException)
    {
        statusLabel.Text = "Iptal edildi";      // iptal hata degil akis
    }
    finally
    {
        UseWaitCursor = false;                  // bekleme imleci her yolda kapanir
    }
}
```

| Parca | Neden |
|---|---|
| `async void` + `EventHandler` | tek mesru `async void` kullanimi |
| `Progress<string>` | raporlar UI thread'e kuyrukla doner |
| `catch (OperationCanceledException)` | iptal hata degil akis olarak islenir |
| `finally` | imleci her yolda geri alir |
| Veri erisimi | `Task.Run` icinde ADO.NET + Dapper, **parameterized** (`@p1`) |

---

## §3 TFM farki: net48 vs net10.0-windows

| API / konu | `net48` | `net10.0-windows` |
|---|---|---|
| `WindowsFormsSynchronizationContext` | var | var |
| `Control.Invoke` / `BeginInvoke` | var | var |
| `Control.InvokeAsync` | **YOK** | var (Task dondurur) |
| `Task.WaitAsync(TimeSpan)` | **YOK** (elle `CancellationTokenSource` + delay) | var |
| `PeriodicTimer` | **YOK** (`System.Timers.Timer`) | var |
| `ArgumentNullException.ThrowIfNull` | **YOK** (elle `if (x == null) throw`) | var |
| `async/await` + `ConfigureAwait` | var | var |

Ortak kaynaktaki guvenli asgari payda: `SynchronizationContext` + `IProgress<T>` + `CancellationToken`. `InvokeAsync`/`WaitAsync` yalniz **`net10.0-windows`'a ozel dosyada** kullanilir; ortak dosyada `#if` acilmaz (yalniz platform API farki serbest).

---

## §4 Ihlal belirtileri

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `.Result` / `.Wait()` | UI donuyor; `net48`'de kesin | `await` |
| `Task.Run` icinde kontrol yazma | `InvalidOperationException` | `IProgress<T>` ya da UI thread'de `await` |
| `Invoke` handle yokken | `InvalidOperationException` | `IsHandleCreated` + `BeginInvoke`, ya da `Progress<T>` |
| `ConfigureAwait(false)` UI'da | sonrasi artik UI thread degil -> patlama | UI kodunda yazma |
| `async void` disinda `async void` | yakalanmayan istisna uygulamayi cukurur | `async Task` |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\async-ui-thread-marshalling.md` (kaynak vault) | marshalling kurali, yol tablosu, emniyetli desen, TFM fark tablosu, 5 ihlal belirtisi |
| `.rules\async-rules.md` | asenkron kural ve `async void` siniri |
| `.rules\error-handling-rules.md` | iptal istisnasinin hata sayilmamasi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur. Kaynak dosyanin govdesi kodlamada bozuk geldigi icin metin dogrulanip yeniden yazildi (ham metin kullanilmadi).
