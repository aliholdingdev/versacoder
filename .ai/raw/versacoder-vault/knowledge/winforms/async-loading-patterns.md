---
title: "WinForms — Asenkron Yükleme, Busy Durumu ve Aşamalı Doldurma"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Form açılışında ve sorgu değişiminde verinin nasıl yükleneceği, yüklenirken arayüzün nasıl meşgul gösterileceği ve veri art arda geldiğinde kazananın nasıl belirlendiği"
---

# Asenkron Yükleme, Busy Durumu ve Aşamalı Doldurma

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Yükleme **her zaman `async`** ve **her zaman kullanıcıya görünür bir busy durumuyla** (progress, `UseWaitCursor`, devre dışı buton) yapılır. Aynı anda birden çok yükleme tetikleniyorsa **kazanan en son başlayan yüklemedir**: öncekiler `CancellationTokenSource` ile iptal edilir (bkz. `cancellation-token-in-ui`). Veri arka planda getirilir, **kontrol ataması UI thread'de** yapılır.

| # | Senaryo | Kalıp | Tuzağ |
|---|---|---|---|
| 1 | Form ilk açılış | `Load` → `await InitializeAsync()` | `Load`'ta senkron I/O |
| 2 | Kutu değişimi | debounce + iptal + yeniden yükle | her tuşta yeni task |
| 3 | Butonla çalıştır | butonu devre dışı bırak → `await` → geri aç | çift tıklama = 2 iş |
| 4 | Uzun rapor | `Task.Run` + ilerleme raporu | UI thread'de hesap |
| 5 | Grid doldurma | `BindingSource` + `ResetBindings` | her satırda tek tek `Add` |

### §1.1 Busy durumu kalıbı

```csharp
private async Task LoadAsync(CancellationToken token)
{
    _busy.Visible = true;
    btnRefresh.Enabled = false;
    UseWaitCursor = true;
    try
    {
        var rows = await _repo.LoadAsync(token);     // arka plan
        grid.BeginUpdate();                          // tek toplu güncelleme
        try { grid.DataSource = rows; }
        finally { grid.EndUpdate(); }
        status.Caption = $"{rows.Count} kayıt";
    }
    catch (OperationCanceledException)
    {
        status.Caption = "Yükleme iptal edildi.";
    }
    catch (Exception ex)
    {
        _errors.Report(ex);
    }
    finally
    {
        _busy.Visible = false;
        btnRefresh.Enabled = true;
        UseWaitCursor = false;
    }
}
```

### §1.2 Busy yüzeyi kontrol listesi

| # | Yüzey | Ne gösterilir | Kapatılma |
|---|---|---|---|
| 1 | `UseWaitCursor` | genel imleç | `finally` |
| 2 | ProgressBar / `Marquee` | süre bilinmiyorsa marquee | `finally` |
| 3 | Devre dışı butonlar | çift tıklama engeli | `finally` |
| 4 | Durum çubuğu metni | "Yükleniyor…" → "N kayıt" | `finally` |
| 5 | Grid `BeginUpdate/EndUpdate` | satır başına yeniden çizim yok | `finally` |

### §1.3 En son kazanır (last-write-wins)

```csharp
private CancellationTokenSource _searchCts;

private async void txtSearch_TextChanged(object sender, EventArgs e)
{
    _searchCts?.Cancel();
    _searchCts?.Dispose();
    _searchCts = new CancellationTokenSource();
    await LoadAsync(_searchCts.Token);
}
```

| # | Kural | Neden |
|---|---|---|
| 1 | Yeni arama başlarken eskisini **iptal et** | bayat veri ekranı kirletir |
| 2 | `DataSource` ataması **tek seferde** | bin satırda bin çizim |
| 3 | `await` her zaman `UI`'ya döner | cross-thread hatası yok |
| 4 | Aşamalı (`IAsyncEnumerable`) yükleme uzun listede | ilk satır erken görünür |

### §1.4 Performans notu (bkz. `ui-performance-and-gc`)

| İşlem | Yavaş | Hızlı |
|---|---|---|
| Grid doldurma | satır satır `Add` | `DataSource = liste` / `BeginUpdate` |
| Ağ + hesap | UI thread'de | `Task.Run` |
| Aynı anda N sorgu | hepsi tamamlanana kadar | iptal + en son |
| `Text` güncelleme | her karakterde tam `Text` | `StringBuilder` + tek atama |

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her yükleme `async` + busy durumu | kod incelemesi |
| 2 | Busy yüzeyleri `finally`'da kapanıyor | kod incelemesi |
| 3 | Arka plan dönüşünde UI'a `Invoke`/`await` var | kod incelemesi |
| 4 | Yeniden tetiklemede iptal var | davranış testi |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · senkron I/O | form üçlüsü / HIGH RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
