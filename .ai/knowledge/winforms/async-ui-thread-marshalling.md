---
title: "async-ui-thread-marshalling - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Arka planda calisan isin sonucunun UI thread'e donusu: SynchronizationContext, Invoke/BeginInvoke, IProgress, .Result/.Wait yasagi ve iki bacak API farklari"
---

# Async ve UI Thread Marshalling

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [async-rules](../../.rules/async-rules.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [composition-root-host-startup](composition-root-host-startup.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

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
