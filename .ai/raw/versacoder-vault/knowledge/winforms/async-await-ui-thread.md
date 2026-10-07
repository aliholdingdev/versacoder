---
title: "WinForms — async/await ile UI Thread Kuralı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "WinForms olay işleyicilerinde `async` kullanımının kurallarını, UI thread'e dönüş ve `Invoke` ihtiyacını, donmuş pencere belirtisini tanımlar"
---

# `async`/`await` ile UI Thread Kuralı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Olay işleyicisi (`Xxx_Click`, `Xxx_Load`) `async void` olabilir ve olmalıdır; **diğer her metot** `async Task` döner. UI thread'de `Thread.Sleep`, senkron I/O ve `Task.Result`/`.Wait()` **yasaktır** — hepsi pencereyi dondurur. `await` sonrası devam, WinForms `SynchronizationContext` sayesinde **otomatik olarak UI thread'e** döner; ayrı bir `Invoke` gerekmez.

| # | Desen | Karar | Neden |
|---|---|---|---|
| 1 | `async void Xxx_Click(...)` | ✓ tek istisna | olay sistemi `Task` bekleyemez |
| 2 | `async Task XxxAsync(...)` servis metodu | ✓ zorunlu | test edilebilir, `await` edilebilir |
| 3 | `Thread.Sleep(...)` UI'da | ✗ yasak | thread'i bloklar, pencere "Yanıt Vermiyor" |
| 4 | `task.Result` / `task.Wait()` | ✗ yasak | deadlock riski (sync-over-async) |
| 5 | Arka plandan kontrole yazmak | `Invoke`/`BeginInvoke` **gerekli** | kontrol thread-affine |

### §1.1 Doğru / yanlış

```csharp
// YANLIS — pencere donar
private void btnRun_Click(object sender, EventArgs e)
{
    var data = _repository.LoadAll();   // senkron I/O
    Thread.Sleep(1000);
    grid.DataSource = data;
}
```

```csharp
// DOĞRU
private async void btnRun_Click(object sender, EventArgs e)
{
    try
    {
        btnRun.Enabled = false;
        UseWaitCursor = true;
        var data = await _repository.LoadAllAsync();   // UI thread serbest
        grid.DataSource = data;
    }
    catch (Exception ex)
    {
        _errors.Report(ex);
    }
    finally
    {
        btnRun.Enabled = true;
        UseWaitCursor = false;
    }
}
```

### §1.2 Sıra tablosu

| # | Adım | Thread | Not |
|---|---|---|---|
| 1 | Olay tetiklenir | UI | |
| 2 | `await` öncesi kod | UI | kontroller güvenli |
| 3 | I/O / `Task.Run` | havuz | UI thread bırakılır |
| 4 | `await` sonrası | **UI** (otomatik) | `SynchronizationContext` |
| 5 | Kontrol `DataSource` ataması | UI | güvensiz değil |

### §1.3 `Invoke` gereken tek durum

`Task.Run` bloğu içinde **doğrudan** kontrole dokunmak yasaktır; kontrole yalnız `BeginInvoke` ile dönülür:

```csharp
await Task.Run(() =>
{
    var rows = ComputeRows();                 // ağır hesap, UI yok
    BeginInvoke(new Action(() => grid.DataSource = rows));  // UI'a dönüş
});
```

> **KURAL:** `await Task.Run(...)` sonrası **otomatik** UI thread'e dönersiniz; blok **içinde** dokunacaksanız `BeginInvoke` zorunludur. İkisini karıştırmak "cross-thread operation" hatası üretir.

### §1.4 İki bacak notu

`WindowsFormsSynchronizationContext` her iki bacakta da vardır. `net48`'de `async void` olay işleyicilerinde analiz kuralı daha katı olabilir; `net10.0-windows` tarafında aynı kod derlenir. `#if` **yalnız** API farkı için kullanılır — `async` mantığı asla `#if` içine alınmaz (G6).

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | UI'da `Thread.Sleep` / `.Result` / `.Wait()` yok | grep = 0 |
| 2 | `async void` yalnız olay işleyicilerinde | kod incelemesi |
| 3 | `Task.Run` bloğu içinde kontrole dokunulmuyor | kod incelemesi |
| 4 | Her `async void` içinde `try/catch/finally` var | kod incelemesi |
| 5 | İki bacakta `dotnet build` → 0 hata | build çıktısı |

## §3 İzinli Link Hedefleri

| # | Hedef | Bu dosyada |
|---|---|---|
| 1 | Kardeş dosya adları (bu dizindeki 30 dosya) | var |
| 2 | [index.md](index.md) — kayıt defteri | var |
| 3 | [../../CLAUDE.md](../../CLAUDE.md) — anayasa G1..G12 | var |
| 4 | `../../index.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/*.md` · `../index.md` | **yok → link verilmez** (`NEEDS_VALIDATION`) |

## §4 Yasaklar

| # | Yasak | İhlalde |
|---|---|---|
| 1 | DevExpress **v26.1** dışı sürüm referansı (G3/G8) | iş RED |
| 2 | `RibbonControlStyle.Office2010` dışı ribbon stili (G3) | iş RED |
| 3 | EF Core · `DbContext` · string-concat SQL (G2) | CRITICAL |
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · UI thread'de `Sleep`/`Wait` | form üçlüsü / HIGH RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
