---
title: "WinForms — async void Olay İşleyicileri ve Hata Yüzeyi"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "`async void`'un yalnız nerede kullanılacağını, kaçan exception'ların nereye gittiğini ve her olay işleyicisinde bulunması gereken hata kafesini tanımlar"
---

# `async void` Olay İşleyicileri ve Hata Yüzeyi

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`async void` **yalnız** WinForms olay işleyicisinde ve **yalnız** tam kapsamlı `try/catch/finally` ile kullanılır. Kapsanmayan bir `async void` exception'ı `SynchronizationContext.Post` üzerinden **uygulamayı düşürebilir**; bu yüzden her `async void` metot kendi kafesine sahiptir ve ayrıca global handler'a (bkz. `global-exception-handling`) haber verir.

| # | Metot türü | Dönüş | Kural |
|---|---|---|---|
| 1 | Olay işleyici (`Click`, `Load`, `Shown`) | `async void` | zorunlu `try/catch/finally` |
| 2 | Servis/uç metot | `async Task` | `await` edilebilir, test edilebilir |
| 3 | `override` metot | `async Task` (mümkünse) | `async void` yalnız `OnXxx` istisnası |
| 4 | Yapıcı (constructor) | **asla** `async` | `async` kurucu yoktur |
| 5 | `IDisposable.Dispose` | **asla** `async` | `FormClosed`'da `await` ayrı metodda |

### §1.1 Zorunlu kafes kalıbı

```csharp
private async void btnRun_Click(object sender, EventArgs e)
{
    try
    {
        await RunAsync();
    }
    catch (OperationCanceledException)
    {
        // iptal hata değildir — yalnız durum satırını güncelle
        status.Caption = "İptal edildi.";
    }
    catch (Exception ex)
    {
        _errors.Report(ex);                    // hata yüzeyi tek yerde
        MessageBox.Show(this, ex.Message, "Hata",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnRun.Enabled = true;                 // UI her koşulda toparlanır
    }
}
```

### §1.2 Hata yüzeyi kararı

| # | Hata tipi | Ne yapılır | Nerede gösterilir |
|---|---|---|---|
| 1 | Kullanıcı girdisi hatalı | doğrula, tekrar dene | `ErrorProvider` (alan üstünde) |
| 2 | Beklenen iş kuralı ihlali | kullanıcıya açıklanır | `MessageBox` / durum çubuğu |
| 3 | Altyapı (DB, ağ, dosya) | raporlanır + günlüklere yazılır | hata diyaloğu + `ILogger` |
| 4 | `OperationCanceledException` | **hata sayılma** | durum çubuğu |
| 5 | Bilinmeyen / `NullReference` | `UNKNOWN` etiketi ile rapor | global handler |

### §1.3 Kaçak exception yolları

| # | Yol | Sonuç | Kapatma |
|---|---|---|---|
| 1 | `async void` içinde `catch` yok | uygulama düşebilir | her işleyicide kafes |
| 2 | `Task.Run` içinde fırlatılıp `await` edilmeyen task | sessiz kayıp | her `Task` `await` edilir |
| 3 | `ContinueWith` içinde exception | sessiz | `await` tercih et |
| 4 | Timer/olay içinde async | kafes yok | işleyicide kafes |
| 5 | `FormClosed` sonrası UI'a yazma | `ObjectDisposedException` | iptal + durum kontrolü |

```powershell
# Kafesiz async void taramasi
Select-String -Path "src\**\*.cs" -Pattern "private async void" -Context 0,4
# Beklenen: her eslesmenin ardindan try {  satiri gelir
```

### §1.4 `Exception` fırlatmak mı, yutmak mı

| Durum | Davranış |
|---|---|
| Kullanıcıya gösterilebildi | `catch` + rapor + **yut** (uygulama yaşar) |
| Gösterilemedi ama log'a yazıldı | `catch` + rapor + **yut** |
| Programcı hatası, yakalanmalı | `catch` + rapor + **yeniden fırlat** (global handler görsün) |
| Asla | `catch { }` — boş kafes (G9 ihlali: nedensiz sessizlik) |

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her `async void` içinde `try/catch/finally` var | §1.3 komutu |
| 2 | Boş `catch { }` yok | grep = 0 |
| 3 | Hata gösterimi tek yüzeyde toplanmış | kod incelemesi |
| 4 | İptal `OperationCanceledException` ayrı ele alınıyor | kod incelemesi |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · boş `catch` | form üçlüsü / RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) · nedensiz sessizlik (G9) | RED |
