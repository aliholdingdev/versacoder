---
title: "WinForms — Erişilebilirlik: Tab Sırası, Mnemonik ve Odak"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Klavye ile tam gezinme, `TabIndex`/`TabStop` ayarı, Alt-tuşu kısayolları (mnemonic), okunabilir ad ve `AccessibleDescription` alanlarının nasıl kurulacağını tanımlar"
---

# Erişilebilirlik: Tab Sırası, Mnemonik ve Odak

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Tüm iş akışı **fare olmadan** yürütülebilmelidir: `Tab`/`Shift+Tab` ile gezinme, `Alt+harf` ile eylem, `Enter`/`Esc` ile onay/iptal. `TabIndex`, `TabStop`, okunabilir `Text`/`AccessibleName` ve `AccessibleDescription` **`.Designer.cs`'te** tanımlanır; `.Form.cs`'e yalnız **odak davranışı** (giriş yapılan ilk alan, `Select()`) ait olabilir.

| # | Özellik | Ne işe yarar | Nerede |
|---|---|---|---|
| 1 | `TabIndex` | `Tab` sırası | `.Designer.cs` |
| 2 | `TabStop` | sekme duraklaması (grid/dahili kontrol: `false`) | `.Designer.cs` |
| 3 | `Text = "&Kaydet"` | `Alt+K` mnemoniği | `.resx` / `.Designer.cs` |
| 4 | `AccessibleName` | ekran okuyucu adı | `.Designer.cs` |
| 5 | `AccessibleDescription` | ne yaptığı | `.Designer.cs` |

### §1.1 Kurallar

| # | Kural | Neden |
|---|---|---|
| 1 | `TabIndex` soldan sağa, yukarıdan aşağı **sıfırdan** başlar | karmaşık sıralar sessizce bozulur |
| 2 | Ribbon içi öğeler `TabStop = false` | sekme ribbon'u **atlamalı**, içeriğe gitmeli |
| 3 | `GridControl` kendi gezinmesini yönetir | `TabStop = false`, içi ok tuşları |
| 4 | Her görünür metin `&` ile mnemonic içerir | `Alt` erişimi |
| 5 | Yalnız ikon butonda `AccessibleName` **zorunlu** | okuyucu "buton" der, anlam kaybolur |

### §1.2 Örnek (`.Designer.cs`)

```csharp
this.btnKaydet.TabIndex = 3;
this.btnKaydet.TabStop = true;
this.btnKaydet.Text = "&Kaydet";
this.btnKaydet.AccessibleName = "Kaydet";
this.btnKaydet.AccessibleDescription = "Ayarları kaydeder ve kapatır.";
this.gridResults.TabIndex = 5;
this.gridResults.TabStop = false;      // grid ic gezinme kendi icinde
```

### §1.3 Kod-behind'daki odak davranışı

```csharp
private void SettingsForm_Shown(object sender, EventArgs e)
{
    txtName.Focus();                 // giris alani -> odak
    txtName.SelectAll();
}
```

| # | Durum | Ne yapılır | `.cs`'e yazar mı |
|---|---|---|---|
| 1 | İlk açılışta odak | `Shown` içinde `Focus()` | ✓ |
| 2 | Doğrulama hatası sonrası | hatalı alana `Focus()` + `Select()` | ✓ |
| 3 | Mnemonic eşleşmesi | otomatik | ✗ (özellik) |
| 4 | Tab sırası | `TabIndex` | ✗ (`.Designer.cs`) |
| 5 | Klavye kısayolu bağlama | `Shortcuts` / `ItemClick` | ✗ (`.Designer.cs`) |

### §1.4 Denetim komutları (kanıt üretir)

```powershell
# TabIndex tutarliligi: ayni konteynerde tekrar eden TabIndex var mi?
Select-String -Path "src\**\*.Designer.cs" -Pattern "\.TabIndex = (\d+);" |
  ForEach-Object { $_.Matches[0].Groups[1].Value } | Group-Object |
  Where-Object { $_.Count -gt 5 }    # belirgin coklama -> incele
# Sade ikon butonlarin AccessibleName'i var mi?
Select-String -Path "src\**\*.Designer.cs" -Pattern "AccessibleName" | Measure-Object
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Form baştan sona `Tab` ile gezilebiliyor | manuel test |
| 2 | `Alt` mnemonicleri çakışmıyor | manuel test |
| 3 | Sade ikon butonlarda `AccessibleName` var | §1.4 çıktısı |
| 4 | Hata sonrası odak hatalı alana gidiyor | davranış testi |
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
| 4 | `.Form.cs`'e `TabIndex`/`TabStop`/kontrol üretimi/`SuspendLayout`/`ResumeLayout`/`BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
