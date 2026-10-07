---
title: "BackgroundWorker mi Task mi - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "BackgroundWorker ile Task/async karsilastirmasi: olaylari, iptali, hata yonetimi, tasinabilirlik; net48 vs net10.0-windows farklari ve C# 7.3/14 yazim ornegi"
---

# `BackgroundWorker` mi, `Task` mi?

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [cancellation-in-ui](cancellation-in-ui.md) · [testing-rules](../../.rules/testing-rules.md)

> **Tek cumlelik sorumluluk:** *WinForms'ta arka plan isi icin hangi yapilandirma secilir, `BackgroundWorker` neyi ucuza cozer, `Task` neyi daha test edilebilir yapar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Varsayilan: `async Task` + `CancellationToken`** (`await` zaten UI thread'e doner, ek event gerekmez). `BackgroundWorker` yalnizca uc durumda hala yerinde: (1) `ProgressChanged`/`RunWorkerCompleted` **otomatik UI thread'e kuyruklanir**, (2) eski `DoWork` tabanli kodun uzerine sarilir, (3) `RunWorkerAsync` calistirilip dispose edilen kisa isler. `BackgroundWorker`'da **iptal `CancellationPending` sorgulamasiyla** yapilir (token yok), hatalar `RunWorkerCompleted.Error` icindedir ve **yeniden deneme yoktur**. Test edilebilirlik acisindan `Task` + enjekte edilen servis (NSubstitute) kazanir; `BackgroundWorker` test icin sarmalanmasi gerekir.

---

## §1 Karsilastirma

| Konu | `BackgroundWorker` | `Task` + `async/await` |
|---|---|---|
| UI thread'e donus | `ProgressChanged` / `RunWorkerCompleted` **otomatik** | `await` (SynchronizationContext) |
| Iptal | `WorkerReportsCancellation` + `CancellationPending` | `CancellationToken` (zorunlu, CS-04) |
| Hata | `RunWorkerCompleted.Error` | `await` → `try/catch` (CS-05) |
| Sonuc | `RunWorkerCompleted.Result` | `return` degeri (`Task<T>`) |
| Birlestirme | yok (zincir elle) | `Task.WhenAll` / `Task.WhenAny` |
| Test | olay tabanli, sarmalama gerekir | enjeksiyon + sahte servis kolay |
| Tavsiye | eski kod sarilmasi / uctan uca UI event | **varsayilan** |

---

## §2 `BackgroundWorker` kalibi (sarinilacaksa)

```csharp
var worker = new BackgroundWorker { WorkerReportsCancellation = true };
worker.DoWork += (s, e) =>
{
    for (int i = 0; i < 100; i++)
    {
        if (worker.CancellationPending) { e.Cancel = true; return; }
        // agir is
    }
    e.Result = "ok";
};
worker.ProgressChanged += (s, e) => statusLabel.Text = e.ProgressPercentage + "%";
worker.RunWorkerCompleted += (s, e) =>
{
    if (e.Error != null) { MessageBox.Show(e.Error.Message); return; }
    if (e.Cancelled)     { statusLabel.Text = "Iptal";       return; }
    statusLabel.Text = (string)e.Result;
};
worker.RunWorkerAsync();
```

| Ozellik | Not |
|---|---|
| `WorkerReportsCancellation = true` | yoksa `CancellationPending` hep `false` |
| `e.Cancel = true` | yalnizca `DoWork` icinde set edilir |
| `RunWorkerCompleted.Error` | bos `try/catch` **yasak** (CS-05) |
| `IDisposable` | `worker.Dispose()` ile serbest birakilir |

---

## §3 TFM farki (net48 vs net10.0-windows)

| API / konu | `net48` | `net10.0-windows` |
|---|---|---|
| `BackgroundWorker` | `System.dll` (yerlesik) | `System.ComponentModel.EventBasedAsync` (Desktop framework icinde) |
| `Task` / `async` | 4.5+ yerlesik | yerlesik |
| `Task.Run` / `WhenAll` | var | var |
| `Task.WaitAsync` | **YOK** | var (6.0+) |
| `System.Threading.Tasks.Extensions` | paket gerekebilir (`ValueTask`) | yerlesik |
| Nullable anotasyonu | elle (`#nullable enable`) | proje sablonu |

> **Not:** `BackgroundWorker` **iki bacakta da mevcuttur**; `#if` gerekmez. Fark yalnizca **test yuzeyi** ve dil surumudur. Test edilebilirligi artirmak icin `DoWork` govdesi `IJobRunner` gibi enjekte edilebilir bir servise cekilir (bkz. [testing-rules](../../.rules/testing-rules.md)).

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48): Task yolu ---
public async Task<string> RunJobAsync(CancellationToken token)
{
    string result = null;
    try
    {
        result = await _runner.ExecuteAsync(token);
    }
    catch (SessionException ex)
    {
        logger.Error(ex.Message);
        throw;
    }
    return result;
}
```

```csharp
// --- C# 14 (net10.0-windows): ayni is, kisa yazim ---
public async Task<string> RunJobAsync(CancellationToken cancellationToken = default)
{
    try
    {
        return await _runner.ExecuteAsync(cancellationToken);
    }
    catch (SessionException ex)
    {
        logger.Error(ex.Message);
        throw;
    }
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `async Task<T>` | var (5.0+) | ayni | **fark yok** |
| `return` async'te | `return result;` | dogrudan `return await` | her ikisi 7.3'te de derlenir |
| Varsayilan parametre | `CancellationToken token` | `= default` | `= default` **7.3'te de var** |
| Nullable donus | `string result = null` | `string?` | 7.3'te `?` yok |

---

## §5 Karar ve ihlal

| Durum | Secim | Gerekce |
|---|---|---|
| Tek seferlik agir is + iptal | `async Task` + `CancellationToken` | CS-04, test edilebilir |
| Cok adimli is + yuzde raporu | `async Task` + `IProgress<T>` | ayni kazanim, daha basit |
| Eski kod uzerine sarma | `BackgroundWorker` | olaylari UI'a otomatik |
| `.Result` / `.Wait()` | **YASAK** | CS-03 deadlock |
| `catch { }` ile `Error` gizlemek | **YASAK** | CS-05 sessiz veri kaybi |

> **Iliskili:** UI thread donusu icin [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · iptal zinciri icin [cancellation-in-ui](cancellation-in-ui.md) · veri erisimi kurallari icin [data-access-rules](../../.rules/data-access-rules.md).
