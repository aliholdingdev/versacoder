---
title: "cancellation-in-ui - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "WinForms'ta iptal akisini: CancellationToken zincirini, iptal butonu ve FormClosing ayrimitini ve iptal edilen isin UI'ya donus seklini tanimlar."
---

# Iptal (Cancellation) UI Akisi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Kullanici "Iptal"e bastiginda calisan is nasil durur, hangi olay iptal edilir, iptal bir hata midir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Iptal **hata degildir**: `ThrowIfCancellationRequested()` -> `OperationCanceledException` yakalanir ve kullaniciya "iptal edildi" gosterilir, `MessageBox` ile hata sunulmaz. Zincir tek yonludur: `CancellationTokenSource` (form seviyesi) -> `CancellationToken` (servis imzasi) -> her `await`'e gecirilir; **token bir kez kullanildiktan sonra CTS yeniden kullanilamaz**, yeni is = yeni CTS. Escape / `X` iki ayri seydir: `FormClosing` `e.Cancel` ile **kapatmayi** iptal eder; calisan isi **durdurmak** icin ayrica `Cancel()` cagrisi gerekir.

```text
+---------------------------------------------------------------------+
| iptal  : hata DEGILDIR -> durum cubugu, hata kutusu YOK             |
| zincir : CTS -> CancellationToken -> her await                      |
| CTS    : is basinda yeni; aynisi tekrar kullanilamaz                 |
| X / Escape: kapatmayi durdurur, calisan isi DURDURMAZ               |
+---------------------------------------------------------------------+
```

---

## §1 Iptal ucgeni

| Katman | Nesne | Ne yapar | Ne YAPMAZ |
|---|---|---|---|
| Kaynak | `CancellationTokenSource` | `Cancel()` / `CancelAfter(ms)` | token'i tekrar kullanmak |
| Tasima | `CancellationToken` | imzada `= default` | kontrolde saklamak, servise gecirmemek |
| Tuketici | `await ... (token)` | `ThrowIfCancellationRequested` | `IsCancellationRequested` ile sessizce yarida kesmek |

```text
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

### §2.1 Kalip kurallari

| Kural | Neden |
|---|---|
| `finally` = `Dispose` + `null` | sizintisiz CTS, yeniden calistirilabilir |
| `OperationCanceledException` ayri `catch` | iptal ile gercek hata ayrilir |
| `CancelAfter(ms)` | zaman asimi da ayni token uzerinden |
| Kapatma onayi `FormClosing` | `e.Cancel` sadece "kapatma"yi durdurur |

---

## §3 Iki bacak farki

| API / konu | `net48` | `net10.0-windows` |
|---|---|---|
| `CancellationTokenSource` | 4.0+ yerlesik | yerlesik |
| `CancelAfter` / `TryReset` | `CancelAfter` var; `TryReset` **YOK** | `TryReset` (6.0+) |
| `WaitAsync(token)` | **YOK** | var (6.0+) |
| `IAsyncEnumerable` + token | paket gerekir | `AsyncEnumerable` (10.0+) yerlesik |
| `async/await` iptal davranisi | ayni | ayni |
| `OperationCanceledException` | ayni | ayni |

Iptal **mantigi** iki bacakta da birebir aynidir; fark yalnizca kisa yazim yardimcilari (`TryReset`, `WaitAsync`) icindir. Ortak kodda bu yardimcilar kullanilmaz -> `#if` **gerekmez**.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
private CancellationTokenSource cts;

public void Start()
{
    if (cts != null) { cts.Dispose(); }
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
| Iptal kontrolu | elle `if` | `ThrowIfCancellationRequested()` | ikisi de 4.0+ -> **her iki bacakta var** |
| Null guvenli zincir | `if (cts != null)` | `?.` | `?.` ortak kodda da var |
| Discard (`_ =`) | yok | `_ = RunAsync(...)` | 7.3'te atama zorunlu |
| Varsayilan parametre | `CancellationToken token` | `= default` | `= default` 7.3'te de var |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| Iptal = hata kutusu | kullanici iptalde "Hata olustu" gorur | `OperationCanceledException` ayri catch |
| Tek CTS tekrar tekrar | 2. calistirmada token zaten iptal | her baslarda yeni `CancellationTokenSource` |
| `FormClosing` = is iptali | kapanis engellenince is hala kosuyor | iki ayri mekanizma (§1 diyagrami) |
| Token imzada yok | iptal edilemeyen is, uygulama kapaninca sizar | her async public metot token alir |
| `catch { }` ile iptal gizleme | sessiz yarim is | tipine gore yakala, goster |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Tek calisma guvencesi (ikinci tik giris yapmaz) | davranis testi |
| 2 | `finally` icinde CTS `Dispose` + `null` | kod incelemesi |
| 3 | `FormClosing` ile is iptali karistirilmamis | kod incelemesi |
| 4 | Token tum async zincirde ilerletiliyor | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\cancellation-in-ui.md` (kaynak vault) | iptal ucgeni, diyagram, emniyetli kalip, TFM ve C# surum tablolari, ihlal belirtileri |
| `.rules\coding-standards.md` | hata siniflandirmasi ve bos catch yasagi |
| `.rules\async-rules.md` | token imzalama ve iptal zinciri kurali |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
