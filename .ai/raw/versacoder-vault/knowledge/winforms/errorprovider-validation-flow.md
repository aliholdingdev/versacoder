---
title: "WinForms — ErrorProvider ile Doğrulama Akışı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Alan bazlı doğrulamanın ne zaman çalıştığını, `ErrorProvider`'ın nerede kurulduğunu, hata temizleme kuralını ve doğrulamanın Application katmanıyla nasıl tekrar kullanıldığını"
---

# `ErrorProvider` ile Doğrulama Akışı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`ErrorProvider` **bir formda birden fazla olamaz, tek instance** kullanılır; kurulum `.Designer.cs`'te, **hata atama/temizleme** `.Form.cs`'tedir. Doğrulama **iki katmandır**: (1) girdi biçimi alanda (`ErrorProvider`), (2) iş kuralı Application katmanında. Kural ihlali geldiğinde **önce tüm alan hataları temizlenir, sonra yeni hata yazılır** — aksi halde eski hata ekranda kalır.

| # | Adım | Ne olur | Dosya |
|---|---|---|---|
| 1 | `_errorProvider = new ErrorProvider(this.components)` | tek instance | `.Designer.cs` |
| 2 | Alan `Validating` olayı | biçim kontrolü | bağ `.Designer.cs`, gövde `.cs` |
| 3 | Hata varsa `e.Cancel = true` | sekme ilerlemez | `.cs` |
| 4 | `_errorProvider.SetError(control, msg)` | kırmızı işaret + ipucu | `.cs` |
| 5 | Temizlik | `Clear()` ya da `SetError(control, null)` | `.cs` |

### §1.1 Kalıp

```csharp
private void txtPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
{
    if (int.TryParse(txtPort.Text, out var port) is false || port is < 1 or > 65535)
    {
        _errorProvider.SetError(txtPort, "Port 1-65535 aralığında olmalı.");
        e.Cancel = true;                 // odak alandan CIKMAZ
        return;
    }
    _errorProvider.SetError(txtPort, null);
}
```

```csharp
private async void btnSave_Click(object sender, EventArgs e)
{
    _errorProvider.Clear();              // 1) TUM hatalari temizle

    if (!ValidateChildren(ValidationConstraints.Enabled)) return;   // 2) alan dogrulama

    try
    {
        await _settingsService.SaveAsync();                          // 3) is kurali
    }
    catch (ValidationException vex)
    {
        _errorProvider.SetError(grpGenel, vex.Message);             // 4) konteyner'a hata
        return;
    }
    catch (Exception ex)
    {
        _errors.Report(ex);
    }

    DialogResult = DialogResult.OK;
    Close();
}
```

### §1.2 Kurulum tablosu

| # | Özellik | Değer | Neden |
|---|---|---|---|
| 1 | `ContainerControl` | `this` | form kapanınca işaretler de kapanır |
| 2 | `BlinkStyle` | `NeverBlink` | dikkat dağıtan yanıp sönme yok |
| 3 | `ContainerControl` alanı | `.Designer.cs` | tasarım kodu |
| 4 | `SetError` çağrıları | `.Form.cs` | davranış |
| 5 | Kaynak metin | `.resx` | çevrilebilir |

### §1.3 Yaygın hatalar

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Her alan için ayrı `ErrorProvider` | işaretler üst üste biner | tek instance |
| 2 | `SetError` ile hata, ama `Clear()` yok | eski hata kalır | tıklamada önce `Clear()` |
| 3 | `Validating`'te `e.Cancel` yok | hatalı alanla geçilir | `CancelEventArgs` kullan |
| 4 | `ValidateChildren` çağrılmıyor | buton tıklanınca alanlar doğrulanmaz | `btnSave_Click` başında |
| 5 | İş kuralı hatası alanda gösterilmiyor | kullanıcı nedenini göremiyor | konteyner `Control`'e `SetError` |

### §1.4 Doğrulamanın iki katmanı

| Katman | Ne kontrol eder | Nerede | Örnek |
|---|---|---|---|
| Alansal | biçim, aralık, zorunlu | `.Form.cs` (`Validating`) | port 1–65535 |
| İş kuralı | tutarlılık, çakışma | **Application** katmanı | "proje adı zaten var" |

> **KURAL:** İş kuralı asla yalnız UI'da doğrulanmaz; UI **hızlı geri bildirim** verir, **son söz** Application katmanındadır. Aksi halde CLI (`VersaCoder.Cli`) aynı kuralı kaçırır.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Formda tek `ErrorProvider` var | grep `new ErrorProvider` = 1 |
| 2 | Kaydet tıklaması `Clear()` + `ValidateChildren()` içeriyor | kod incelemesi |
| 3 | `Validating` gövdeleri `e.Cancel` kullanıyor | kod incelemesi |
| 4 | Hata metinleri `.resx`'te | dosya kontrolü |
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
| 4 | `.Form.cs`'e `ErrorProvider` üretimi / kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
