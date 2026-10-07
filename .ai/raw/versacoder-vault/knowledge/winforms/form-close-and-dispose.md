---
title: "WinForms — FormClosing, FormClosed ve Dispose Temizliği"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Formun kapanış kararının nerede verildiği, iptal edilebildiği ve tüm yönetilen/yonetilmeyen kaynakların ne zaman bırakılacağı"
---

# `FormClosing`, `FormClosed` ve `Dispose` Temizliği

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Kapatma kararı **`FormClosing`**'te verilir (`e.Cancel = true` ile engellenir), temizlik **`FormClosed`**'da yapılır, kaynak bırakımı **`Dispose`**'da. `Dispose` gövdesi `.Designer.cs`'tir ve elle **dokunulmaz**; kod-behind'a ek temizlik `FormClosed`'a ya da `Dispose(bool disposing)` aşırı yüklemesine konur. `IDisposable` tutulan her servis, `FormClosed` içinde serbest bırakılır — GC'ye bırakılmaz.

| # | Olay/metot | Ne yapar | `e.Cancel` mümkün mü | Dosya |
|---|---|---|---|---|
| 1 | `FormClosing` | kapatma kararı öncesi | **evet** | `.Form.cs` |
| 2 | `FormClosed` | kapandıktan sonra, hâlâ erişilebilir | hayır | `.Form.cs` |
| 3 | `Dispose(bool)` | tutamaçlar, `components`, timer serbest bırakımı | — | `.Designer.cs` |
| 4 | `DialogResult` | `ShowDialog` dönüş değeri | — | `.Form.cs` |

### §1.1 Onaylı kapatma (`.Form.cs`)

```csharp
private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
{
    if (_dirty == false) return;

    var result = MessageBox.Show(
        this,
        "Kaydedilmemiş değişiklikler var. Çıkılsın mı?",
        "Onay",
        MessageBoxButtons.YesNoCancel,
        MessageBoxIcon.Warning);

    if (result == DialogResult.Cancel) { e.Cancel = true; return; }
    if (result == DialogResult.Yes)     { _ = SaveAsync(); return; }
    // DialogResult.No → kapatmaya izin ver
}
```

> **KURAL:** `async` kaydetme `FormClosing` içinde **await edilemez** (form kapanırken beklenemez). Ya senkron kaydet, ya da `FormClosed`'da `Task.Run` ile arkaya al ve sonucu raporla — bu, `NEEDS_VALIDATION` gerektiren bir davranış değil, kalıptır.

### §1.2 Temizlik sırası

```csharp
// SettingsForm.cs
private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
{
    _settingsService.Changed -= OnSettingsChanged;   // 1) abonelik kaldır
    _autoSaveTimer.Stop();                            // 2) zamanlayıcı durdur
    _autoSaveTimer.Dispose();                         // 3) IDisposable bırak
    _cts.Cancel();                                    // 4) bekleyen işi iptal et
    _cts.Dispose();
}
```

| # | Bırakılacak | Neden | Bırakılmazsa |
|---|---|---|---|
| 1 | Olay abonelikleri | sahibi uzun ömürlü servis | form GC'den kurtulur → **bellek sızıntısı** |
| 2 | `Timer` | unmanaged-timer eşdeğeri | olay tetiklenmeye devam eder |
| 3 | `CancellationTokenSource` | bekleyen `Task` | iş form kapandıktan sonra UI'a dokunur |
| 4 | `HttpClient`/`DbConnection` **yalnız sahiplik varsa** | socket/ bağlantı havuzu | bağlantı sızıntısı |

### §1.3 `Dispose`'a elle dokunmak yasak

```csharp
// SettingsForm.Designer.cs — bu gövde designer'ın ürettiği koddur
protected override void Dispose(bool disposing)
{
    if (disposing && (components != null))
    {
        components.Dispose();
    }
    base.Dispose(disposing);
}
```

`.Form.cs` içinde ikinci bir `Dispose(bool)` aşırı yüklemesi **yazılmaz**; fazla temizlik `FormClosed`'a konur. Böylece `.Designer.cs` ↔ `.resx` tutarlılığı (G3/`ui-designer` yetkisi) bozulmaz.

### §1.4 `using` ile modal form

```csharp
using (var dlg = new ConfirmDialogForm())
{
    if (dlg.ShowDialog(this) != DialogResult.OK) return;
    // dlg.Dispose() blok sonunda otomatik
}
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kirli formda `FormClosing` onayı var | davranış testi |
| 2 | Her `IDisposable` alan `FormClosed`'da bırakılıyor | kod incelemesi |
| 3 | `.Form.cs`'te `Dispose(bool)` aşırı yüklemesi yok | grep = 0 |
| 4 | Olay abonelikleri kaldırılıyor (uyzun ömürlü serviste) | kod incelemesi |
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
