---
title: "WinForms — Load, Shown ve Activated Olayları"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Form açılırken tetiklenen üç olayın farkı, hangi işin nereye konacağı ve ilk yükleme mantığının nasıl korunacağı"
---

# `Load`, `Shown` ve `Activated` Olayları

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`Load` **görünmeden hemen önce** ve **her** `Show`/`ShowDialog`'ta tetiklenir; `Shown` yalnızca pencere **ilk kez** görünür olduğunda; `Activated` **her odak** kazanışında. Veri yükleme `Load`'a, ilk-kez işleri `Shown`'a, odak bağlı tazelemeler `Activated`'e konur. Hepsinde de iş mantığı **Application katmanına** çağrılır, kod-behind yalnızca yönlendirir.

| # | Olay | Kaç kez tetiklenir | Zaman | Kullanım |
|---|---|---|---|---|
| 1 | `Load` | her `Show`/`ShowDialog` | handle hazır, pencere görünmeden önce | veri yükleme, `async` doldurma |
| 2 | `Shown` | form ömründe **1 kez** | ilk kez görünür | ilk odak, giriş animasyonu, "bir daha sorma" kutusu |
| 3 | `Activated` | her odak kazanışı | pencere görünürken | listeyi tazele, zamanlayıcı başlat |
| 4 | `Deactivate` | odak kaybı | pencere görünürken | zamanlayıcı durdur, Draft kaydet |

### §1.1 Bağlama biçimi (`.Designer.cs` içinde)

Olay bağlama `InitializeComponent` içindedir; `.Form.cs`'e `this.Load += ...` **yazılmaz**:

```csharp
// SettingsForm.Designer.cs — tek InitializeComponent kaynağı
this.Load += new System.EventHandler(this.SettingsForm_Load);
this.Shown += new System.EventHandler(this.SettingsForm_Shown);
this.Activated += new System.EventHandler(this.SettingsForm_Activated);
```

```csharp
// SettingsForm.cs — yalnız olay yönlendiriciler
private async void SettingsForm_Load(object sender, EventArgs e)
{
    await _mediator.SendAsync(new LoadSettingsQuery(), CancellationToken.None);
}

private void SettingsForm_Shown(object sender, EventArgs e)
{
    if (_firstShown) return;      // Shown yalnız 1 kez gelir; yine de koruyucu yaz
    _firstShown = true;
    gridSettings.Focus();
}

private void SettingsForm_Activated(object sender, EventArgs e)
{
    _clock.Start();               // odak geldi → canlı tazeleme
}
```

### §1.2 Sık kırılan davranışlar

| # | Belirti | Kök neden | Düzeltme |
|---|---|---|---|
| 1 | İkinci açılışta eski veri | doldurma `Load`'ta, form yeniden kullanılıyor | doldurmayı `Load`'tan `public async Task ReloadAsync()`'a taşı |
| 2 | `Shown` iki kez çalışıyor | form gizlenip yeniden gösteriliyor (`Hide`+`Show`) | korucuk bayrak + olay `.Designer.cs`'ten kaldırma |
| 3 | `Load` içinde `MessageBox` döngüsü | olay yeniden girişe iz veriyor | `Shown`'a taşı ya da bayrak koy |
| 4 | Form görünmeden ölçü hatası | `Load`'ta `Width` okunuyor | `Shown`'a taşı |
| 5 | `async void Load` exception'ı uygulamayı düşürüyor | yakalama yok | `try/catch` + global handler |

### §1.3 `async void Load` kalıbı

```csharp
private async void SettingsForm_Load(object sender, EventArgs e)
{
    try
    {
        _loading.Visible = true;
        await _settingsService.RefreshAsync();
    }
    catch (Exception ex)
    {
        _errorReporter.Report(ex);          // hatayı yutma
        throw;                              // global handler da görsün
    }
    finally
    {
        _loading.Visible = false;
    }
}
```

> **KURAL:** `async void` **yalnız** olay işleyicisinde ve **yalnız** tam kapsamlı `try/catch/finally` ile kullanılır; kütüphane metodu asla `async void` olmaz.

### §1.4 İki bacak notu

`Shown`/`Activated` davranışı her iki bacakta aynıdır; yalnızca DPI varsayılanları farklıdır (`net48` → `app.config`, `net10.0-windows` → `ApplicationConfiguration.Initialize()`). Bu fark **platform farkıdır**, `#if` ile izole edilir ve iş mantığına girmez (G6).

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Olay `+=` satırları yalnız `.Designer.cs` içinde | grep = 0 eşleşme `.Form.cs` |
| 2 | `async void` işleyicide tam `try/catch/finally` var | kod incelemesi |
| 3 | Yeniden kullanılan formda doldurma `Load`'tan taşındı | davranış testi |
| 4 | Hata yolu test edildi (ağ yok senaryosu) | test çıktısı |
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
