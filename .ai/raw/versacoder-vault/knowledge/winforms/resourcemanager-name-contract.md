---
title: "WinForms — ResourceManager Ad Sözleşmesi ve Kaynak Çözümleme"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "`ResourceManager`'ın tip adından kaynak adını nasıl türettiğini, ad bozulduğunda ne olduğunu ve doğru adın nasıl doğrulanacağını anlatır"
---

# `ResourceManager` Ad Sözleşmesi

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`ComponentResourceManager(typeof(SettingsForm))` kaynak adını **tipin namespace'i + adı** ile türetir: `VersaCoder.UI.Forms.SettingsForm.resources`. `.resx` dosyasının adı, namespace'i veya tip adı değişirse kaynak adı da değişir ve form `MissingManifestResourceException` ile **açılmaz**. Bu yüzden tip adı, dosya adı ve namespace **birlikte ve tek seferde** taşınır; parçasını değiştirmek `G3`/`G4` ihlalidir.

| # | Öğe | Örnek | Etkilediği |
|---|---|---|---|
| 1 | Tip adı | `SettingsForm` | kaynak adının son parçası |
| 2 | Namespace | `VersaCoder.UI.Forms` | kaynak adının ilk parçası |
| 3 | `.resx` dosya adı | `SettingsForm.resx` | proje `EmbeddedResource` tanımı |
| 4 | Kaynak anahtarı | `btnSave.Caption` | `.Designer.cs` içindeki `ApplyResources` |
| 5 | Kültür | `tr-TR` | `SettingsForm.tr-TR.resources` |

### §1.1 Ad türetimi (ASCII)

```
typeof(VersaCoder.UI.Forms.SettingsForm)
        |                    |       |
        |                    |       +-- tip adi  -> SettingsForm.resources
        |                    +---------- namespace -> VersaCoder.UI.Forms.
        +------------------------------- asm adi   -> VersaCoder.UI.

Beklenen GOMME: VersaCoder.UI.Forms.SettingsForm.resources
```

### §1.2 Doğrulama komutları (kanıt üretir)

```powershell
# 1) Gömülü kaynak adlarını listele (asm içinde gerçekten ne var)
[System.Reflection.Assembly]::LoadFile("$pwd\bin\Debug\net10.0-windows\VersaCoder.UI.dll") `
  .GetManifestResourceNames() | Select-String SettingsForm

# 2) Beklenen ad satırı mı?
#    VersaCoder.UI.Forms.SettingsForm.resources   -> OK
#    (hiç çıktı yok)                              -> .resx EmbeddedResource değil
```

| # | Çıktı | Anlam | Düzeltme |
|---|---|---|---|
| 1 | Beklenen ad var | kaynak çözümleniyor | — |
| 2 | Hiç çıktı yok | `.resx` gömülmemiş | `.csproj` `EmbeddedResource` kontrolü |
| 3 | Farklı namespace | tip taşındı | tip + dosya + namespace **birlikte** |
| 4 | Çalışma zamanı `MissingManifestResourceException` | ad uyuşmuyor | §1.1 tablosunu satır satır karşılaştır |

### §1.3 Ad değiştirmenin güvenli sırası

| # | Adım | Neden sıra bu |
|---|---|---|
| 1 | Tasarımcıyı kapat | yazma çakışması |
| 2 | `.resx`'i **tasi rcı ile** yeniden adlandır (değil elle) | `EmbeddedResource` otomatik güncellenir |
| 3 | Tip adını yeniden adlandır (refactor) | `typeof(...)` çağrıları kendiliğinden güncellenir |
| 4 | Namespace'i güncelle | `using`'ler refactor ile |
| 5 | Derle + formu **aç** | tek kanıt bu |

> **UYARI:** Sıra bozulursa (örneğin önce tip adı) geçici olarak `MissingManifestResourceException` çıkar; bu **tahminle** düzeltilemez, adım adım geri alınır.

### §1.4 Yetki sınırı

| İşlem | `ui-designer` | `developer` |
|---|---|---|
| `.resx` içinde anahtar/valü ekleme | ✓ son söz | ✗ |
| `ComponentResourceManager` tip adını değiştirmek | ✓ | ✗ |
| `.Designer.cs` içinde `ApplyResources` | ✓ | ✗ |
| Olay **gövdesinde** kaynak okumak (`Resources.X`) | — | ✓ |
| `ResourceManager`'ı kod içinde elle `new`lemek | ✗ | ✗ (herkes için yasak) |

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | `GetManifestResourceNames()` beklenen adı veriyor | §1.2 çıktısı |
| 2 | Tip ↔ `.resx` ↔ namespace üçlüsü eşleşiyor | refactor kaydı |
| 3 | `new ResourceManager(...)` elle çağrı yok | grep = 0 |
| 4 | Form açılışında kaynak istisnası yok | çalıştırma kanıtı |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · elle `ResourceManager` kurmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
