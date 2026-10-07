---
title: "UI Iptal Akisi - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms'ta iptal: CancellationToken akisi, iptal butonu/Escape, FormClosing iptali, iptal edilen isin UI'ya donusu; net48 vs net10.0-windows farklari"
---

# Iptal (Cancellation) UI Akisi

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [form-lifecycle](form-lifecycle.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Kullanici "Iptal"e bastiginda calisan is nasil durur, hangi olay iptal edilir, iptal bir hata midir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Iptal **hata degildir**: `CancellationToken.ThrowIfCancellationRequested()` → `OperationCanceledException` yakalanir ve kullaniciya "iptal edildi" gosterilir, `MessageBox` ile hata sunulmaz. Zincir tek yonludur: `CancellationTokenSource` (form seviyesi) → `CancellationToken` (servis imzasi, CS-04) → her `await`'e gecirilir; **token bir kez kullanildiktan sonra CTS yeniden kullanilamaz**, yeni is = yeni CTS. Escape/`X` iki ayri seydir: `FormClosing` `e.Cancel` ile **kapatmayi** iptal eder; calisan isi **durdirmak** icin ayri `Cancel()` cagrisi gerekir.

---

## §1 Iptal ucgeni

| Katman | Nesne | Ne yapar | Ne YAPMAZ |
|---|---|---|---|
| Kaynak | `CancellationTokenSource` | `Cancel()` / `CancelAfter(ms)` | token'i tekrar kullan |
| Tasima | `CancellationToken` | imzada `= default` | kontrolde sakla, service gecir |
| Tuketici | `await ... (token)` | `ThrowIfCancellationRequested` | `token.IsCancellationRequested` ile sessizce yarida kes |

```
[btnIptal.Click] ----> _cts.Cancel() -----+
                                          |
[FormClosing e.Cancel=true] --> kapatmayi geri al  (calisan isi DURDIRMAZ)
                                          |
                                          v
   _sessions.LoadAsync(token)  -->  OperationCanceledException
                                          |
                                          v
                            statusLabel = "Iptal edildi"   (hata kutusu YOK)
```

---

## §2 Emniyetli kalip

```csharp
private CancellationTokenSource _cts;

private async void RunButton_Click(object sender, EventArgs e)
{
    if (_cts != null) { return; }                 // zaten calisiyor
    _cts = new CancellationTokenSource();
    cancelButton.Enabled = true;
    try
    {
        await _runner.RunAsync(_cts.Token);
        statusLabel.Text = "Tamam";
    }
    catch (OperationCanceledException)
    {
        statusLabel.Text = "Iptal edildi";
    }
    finally
    {
        cancelButton.Enabled = false;
        _cts.Dispose();
        _cts = null;
    }
}

private void CancelButton_Click(object sender, EventArgs e)
{
    if (_cts != null) { _cts.Cancel(); }
}
```

| Kural | Neden |
|---|---|
| `finally` = `Dispose` + `null` | sızıntısız CTS, yeniden calistirilabilir |
| `OperationCanceledException` ayri `catch` | iptal ile gercek hata ayrisir (CS-05) |
| `CancelAfter(ms)` | zaman asimi da ayni token uzerinden |
| Kapatma onayi `FormClosing` | `e.Cancel` sadece "kapatma"yi durdurur (bkz. [form-lifecycle](form-lifecycle.md)) |

---

## §3 TFM farki (net48 vs net10.0-windows)

| API / konu | `net48` | `net10.0-windows` |
|---|---|---|
| `CancellationTokenSource` | 4.0+ yerlesik | yerlesik |
| `CancelAfter` / `TryReset` | `CancelAfter` var; `TryReset` **YOK** | `TryReset` (6.0+) |
| `WaitAsync(token)` | **YOK** | var (6.0+) |
| `IAsyncEnumerable` + token | paket (`System.Linq.Async` yok) | `AsyncEnumerable` (10.0+) yerlesik |
| `async/await` iptal davranisi | ayni | ayni |
| `OperationCanceledException` | ayni | ayni |

> **Sonuc:** iptal **mantigi** iki bacakta da birebir aynidir; fark yalnizca kisa yazim yardimcilari (`TryReset`, `WaitAsync`) icindir. Ortak kodda bu yardimcilar kullanilmaz → `#if` **gerekmez**.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
private CancellationTokenSource cts;

public void Start()
{
    if (cts != null)
    {
        cts.Dispose();
    }
    cts = new CancellationTokenSource();
    cts.CancelAfter(TimeSpan.FromSeconds(30));
    RunAsync(cts.Token);
}

private async Task RunAsync(CancellationToken token)
{
    if (token.IsCancellationRequested)
    {
        throw new OperationCanceledException(token);
    }
    await Task.Delay(1000, token);
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
private CancellationTokenSource? _cts;

public void Start()
{
    _cts?.Dispose();
    _cts = new CancellationTokenSource();
    _cts.CancelAfter(TimeSpan.FromSeconds(30));
    _ = RunAsync(_cts.Token);
}

private async Task RunAsync(CancellationToken token = default)
{
    token.ThrowIfCancellationRequested();
    await Task.Delay(1000, token);
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `IsCancellationRequested` kontrolu | elle `if` | `ThrowIfCancellationRequested()` | ikisi de 4.0+ -> **her iki bacakta var** |
| Null guvenli zincir | `if (cts != null)` | `?.` | `?.` 6.0+ -> ortak kodda da var |
| Discard (`_ =`) | yok | `_ = RunAsync(...)` | 7.3'te atama zorunlu |
| Varsayilan parametre | `CancellationToken token` | `= default` | `= default` 7.3'te de var |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| Iptal = hata kutusu | kullanici iptalde "Hata olustu" gorur | `OperationCanceledException` ayri catch |
| Tek CTS tekrar tekrar | 2. calistirmada token zaten iptal | her baslarda yeni `CancellationTokenSource` |
| `FormClosing` = is iptali | kapanis engellenince is hala kosuyor | iki ayri mekanizma (§1 diyagrami) |
| Token imzada yok | iptal edilemeyen is, uygulama kapaninca sızar | CS-04: her async public metot token alir |
| `catch { }` ile iptal gizleme | CS-05, sessiz yarim is | tipine gore yakala, goster |

> **Iliskili:** UI thread donusu icin [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · yapilandirma secimi icin [backgroundworker-vs-task](backgroundworker-vs-task.md) · dogrulama iptali icin [validation-form-level](validation-form-level.md).
