---
title: "backgroundworker-vs-task - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Arka plan isi icin BackgroundWorker ile Task/async arasindaki secimi: olaylar, iptal, hata yonetimi, test edilebilirlik ve iki bacak (net48 / net10.0-windows) farklari."
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# `BackgroundWorker` mi, `Task` mi?

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [cancellation-in-ui](cancellation-in-ui.md) · [testing-rules](../../.rules/testing-rules.md) · [async-rules](../../.rules/async-rules.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms 3/32):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *WinForms'ta arka plan isi icin hangi yapilandirma secilir, `BackgroundWorker` neyi ucuza cozer, `Task` neyi daha test edilebilir yapar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Varsayilan: `async Task` + `CancellationToken`** (`await` zaten UI thread'e doner, ek olay gerekmez). `BackgroundWorker` yalnizca uc durumda hala yerinde: (1) `ProgressChanged` / `RunWorkerCompleted` **otomatik UI thread'e kuyruklanir**, (2) eski `DoWork` tabanli kodun uzerine sarilir, (3) `RunWorkerAsync` calistirilip dispose edilen kisa isler. `BackgroundWorker`'da **iptal `CancellationPending` sorgulamasiyla** yapilir (token yok), hatalar `RunWorkerCompleted.Error` icindedir ve **yeniden deneme yoktur**. Test edilebilirlik acisindan `Task` + enjekte edilen servis kazanir.

```text
+---------------------------------------------------------------+
| varsayilan : async Task + CancellationToken                   |
| BW kalan 3 : otomatik UI olayi / eski kod sarilmasi / kisa is |
| BW iptal   : CancellationPending (token yok)                  |
| BW hata    : RunWorkerCompleted.Error - yeniden deneme YOK    |
+---------------------------------------------------------------+
```

---

## §1 Karar matrisi (once secim, sonra ayrinti)

| Durum | Secim | Gerekce |
|---|---|---|
| Tek seferlik agir is + iptal | `async Task` + `CancellationToken` | token tabanli iptal, test edilebilir |
| Cok adimli is + yuzde raporu | `async Task` + `IProgress<T>` | ayni kazanim, daha basit |
| Eski kod uzerine sarma | `BackgroundWorker` | olaylari UI'a otomatik |
| `.Result` / `.Wait()` | **YASAK** | sync-over-async deadlock |
| `catch { }` ile `Error` gizlemek | **YASAK** | sessiz veri kaybi |

---

## §2 Kararlastirma tablosu

| Konu | `BackgroundWorker` | `Task` + `async/await` |
|---|---|---|
| UI thread'e donus | `ProgressChanged` / `RunWorkerCompleted` **otomatik** | `await` (`SynchronizationContext`) |
| Iptal | `WorkerReportsCancellation` + `CancellationPending` | `CancellationToken` (her async public imzada) |
| Hata | `RunWorkerCompleted.Error` | `await` -> `try/catch` |
| Sonuc | `RunWorkerCompleted.Result` | `return` degeri (`Task<T>`) |
| Birlestirme | yok (zincir elle) | `Task.WhenAll` / `Task.WhenAny` |
| Test | olay tabanli, sarmalama gerekir | enjeksiyon + sahte servis kolay |
| Tavsiye | eski kod sarilmasi / uctan uca UI olayi | **varsayilan** |

---

## §3 `BackgroundWorker` kalibi (sarinilacaksa)

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

### §3.1 Kalip notlari

| Ozellik | Not |
|---|---|
| `WorkerReportsCancellation = true` | yoksa `CancellationPending` hep `false` |
| `e.Cancel = true` | yalnizca `DoWork` icinde set edilir |
| `RunWorkerCompleted.Error` | bos `try/catch` **yasak** |
| `IDisposable` | `worker.Dispose()` ile serbest birakilir |

---

## §4 Iki bacak ve dil farki

| API / konu | `net48` | `net10.0-windows` |
|---|---|---|
| `BackgroundWorker` | `System.dll` (yerlesik) | `System.ComponentModel.EventBasedAsync` |
| `Task` / `async` | 4.5+ yerlesik | yerlesik |
| `Task.Run` / `WhenAll` | var | var |
| `Task.WaitAsync` | **YOK** | var (6.0+) |
| `System.Threading.Tasks.Extensions` | paket gerekebilir (`ValueTask`) | yerlesik |
| Nullable anotasyonu | elle (`#nullable enable`) | proje sablonu |
| Dil surumu | C# 7.3 | C# 14 |

`BackgroundWorker` **iki bacakta da mevcuttur**; `#if` gerekmez. Fark yalnizca **test yuzeyi** ve dil surumudur. Test edilebilirligi artirmak icin `DoWork` govdesi `IJobRunner` gibi enjekte edilebilir bir servise cekilir.

---

## §5 C# 7.3 (net48) vs C# 14 ayni is

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

## §6 Kim neyi degistirir

| Kim | Ne yapar | Kanit |
|---|---|---|
| `developer` | yeni isi `async Task` + token ile yazar | kod incelemesi |
| `reviewer` | `.Result` / `.Wait()` ve bos `catch { }` tarar | grep hedefi: 0 eslesme |
| `tester` | enjekte edilen servisle sahte test | test ciktisi |
| `architect` | `BackgroundWorker` sarmalamasina izin verip gerekceler | karar notu |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\backgroundworker-vs-task.md` (kaynak vault) | karsilastirma tablosu, BW kalibi, TFM tablosu, C# 7.3/14 ornekleri, karar matrisi |
| `.rules\testing-rules.md` | enjeksiyon ve sahte servis ile test kurali |
| `.rules\async-rules.md` | async Task varsayilani, sync-over-async yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
