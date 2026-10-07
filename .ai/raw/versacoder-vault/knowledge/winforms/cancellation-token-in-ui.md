---
title: "WinForms — UI'da CancellationToken Kullanımı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Form kapanırken veya yeni arama başlarken eski işin iptal edilmesi, `CancellationTokenSource` ömrü ve iptalin UI'a nasıl yansıtıldığı"
---

# UI'da `CancellationToken` Kullanımı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Her forma ait bir `CancellationTokenSource` **form ömrüne bağlıdır**: yeni bir iş başlarken eski iptal edilir (`Cancel` + `Dispose` + yenisi), form kapanırken (`FormClosed`) iptal edilir ve `Dispose` edilir. İptal **hata değildir**: `OperationCanceledException` yakalanır, kullanıcıya hata diyaloğu **çıkarılmaz**, durum çubuğu güncellenir.

| # | Zaman | Ne yapılır | Nerede |
|---|---|---|---|
| 1 | Form kurulurken | `_cts = new CancellationTokenSource()` | kurucu (`.Form.cs`) |
| 2 | Yeni iş başlarken | eski `Cancel()` + `Dispose()` + yenisi | olay işleyici |
| 3 | İş çağrıya | `token` ilerletilir (`PassAsync`) | `.Form.cs` → Application |
| 4 | "İptal" butonu | `_cts.Cancel()` | olay işleyici |
| 5 | Form kapanırken | `Cancel()` + `Dispose()` | `FormClosed` |

### §1.1 Kalıp

```csharp
public partial class ChatForm : RibbonForm
{
    private CancellationTokenSource? _searchCts;

    private async void txtQuery_TextChanged(object sender, EventArgs e)
    {
        _searchCts?.Cancel();                 // 1) eski işi iptal et
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;         // 2) yenisini al

        try
        {
            var hits = await _searcher.SearchAsync(txtQuery.Text, token);
            grid.DataSource = hits;
        }
        catch (OperationCanceledException)
        {
            status.Caption = "Arama iptal edildi.";   // hata diyaloğu YOK
        }
        catch (Exception ex)
        {
            _errors.Report(ex);
        }
    }

    private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;
    }
}
```

### §1.2 İptal vs hata

| Olay | Kullanıcıya ne görünür | Kod |
|---|---|---|
| `OperationCanceledException` | durum çubuğu: "İptal" | `catch (OperationCanceledException)` |
| Ağ hatası | hata diyaloğu + rapor | `catch (Exception ex)` |
| Zaman aşımı (`TimeoutException`) | "Zaman aşımı" ayrı satır | ayrı `catch` |
| Form kapandıktan sonra dönüş | **hiçbir şey** (kontroldisposed) | `token.IsCancellationRequested` kontrolü |

### §1.3 Yaygın hatalar

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | `_cts` hiç `Dispose` edilmiyor | GC bekçisi artar | her `Cancel` sonrası `Dispose` |
| 2 | Aynı `_cts` iki işte paylaşılıyor | biri diğerini iptal eder | iş başına yeni `CancellationTokenSource` |
| 3 | İptal exception'ı hata diyaloğu gösteriyor | kullanıcı şoka girer | ayrı `catch` |
| 4 | `FormClosed`'da iptal yok | form kapandıktan sonra iş devam | `FormClosed`'da iptal |
| 5 | `token` hiç ilerletilmiyor | iptal çalışmaz | tüm async zincire `token` geçir |

### §1.4 İki bacak notu

`CancellationTokenSource` her iki bacakta da aynı API'yi kullanır. `#if` yalnız platforma özgü API farkına (ör. `WaitAsync` yardımcı metodu yokluğu) izin verir; iptal mantığı asla `#if` içine alınmaz (G6).

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her `_cts` `Cancel` + `Dispose` ediliyor | kod incelemesi |
| 2 | `FormClosed` içinde iptal var | kod incelemesi |
| 3 | `OperationCanceledException` ayrı yakalanıyor | grep |
| 4 | Token async zincirin tamamında ilerletiliyor | kod incelemesi |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
