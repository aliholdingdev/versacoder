---
title: "WinForms — Yerelleştirilmiş Metin Kaynakları (.resx)"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Form metinlerinin nerede saklandığını, kültür değişiminde nasıl çözümlendiğini ve kod içinde sabit kullanıcı metni yazmanın yasak olmasını tanımlar"
---

# Yerelleştirilmiş Metin Kaynakları

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Tüm **görünen** kullanıcı metni (başlık, buton, kolon, uyarı) `.resx` içindedir; `.Form.cs`'e sabit Türkçe/İngilizce metin **yazılmaz**. Metin, `.Designer.cs` içindeki `ApplyResources` çağrılarıyla **form açılırken** otomatik bağlanır; kültür (`tr-TR`, `en-US`) `CurrentUICulture`'dan okunur ve bulunamazsa **varsayılan (nötr)** kaynağa düşer.

| # | Katman | Dosya | İçerik |
|---|---|---|---|
| 1 | Form kaynakları | `XxxForm.resx` | `$this.Caption`, `btnSave.Caption`, kolon başlıkları |
| 2 | Kültür varyantı | `XxxForm.tr-TR.resx` | farklı çeviriler |
| 3 | Proje geneli | `Properties/Resources.resx` | uygulama genelindeki metin/ikon |
| 4 | Kod içi sabit metin | `.Form.cs` | **yasak** (hata mesajı bile kaynakta) |

### §1.1 Akış (ASCII)

```
CurrentUICulture = tr-TR
        |
        v
ComponentResourceManager(typeof(SettingsForm))
        |
        +--> SettingsForm.tr-TR.resources   (varsa)
        +--> SettingsForm.resources         (yoksa)
        |
        v
ApplyResources(this.btnSave, "btnSave")  ->  buton metni
```

### §1.2 Kural tablosu

| # | Kural | Neden |
|---|---|---|
| 1 | Kullanıcıya görünen her metin `.resx`'te | çeviri + tek noktadan güncelleme |
| 2 | Anahtar adı kontrol adıyla eşleşir (`btnSave.Caption`) | `.Designer.cs` ile otomatik bağ |
| 3 | Elle `new ResourceManager(...)` yok | `ComponentResourceManager` tekel |
| 4 | Hata mesajları da kaynakta | kod içi sabit = sonradan çevrilemez |
| 5 | `.resx` elle düzenlenmez | XML bozulursa tasarımcı açılmaz |

### §1.3 `.Form.cs`'e yazılmayan metinler

```csharp
// YANLIS
MessageBox.Show(this, "Ayarlar kaydedilemedi.");     // sabit metin

// DOĞRU
MessageBox.Show(this, Properties.Resources.MsgSettingsSaveFailed,
    Properties.Resources.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
```

| # | Metin | Nereye |
|---|---|---|
| 1 | Buton/etiket başlıkları | form `.resx` (`btnSave.Caption`) |
| 2 | Pencere başlığı | form `.resx` (`$this.Caption`) |
| 3 | Uyarı/hata mesajları | `Properties/Resources.resx` |
| 4 | Log mesajları | **kodda** (kullanıcıya görünmez, çevrilemez) |
| 5 | Teknik terim (dosya yolu, SQL) | kodda |

### §1.4 Bozulma belirtileri

| # | Belirti | Neden | Düzeltme | Sorumlu |
|---|---|---|---|---|
| 1 | Butonda ham anahtar görünüyor (`btnSave.Caption`) | `ApplyResources` çalışmadı | `.Designer.cs` ↔ `.resx` anahtarını eşleştir | `ui-designer` |
| 2 | Hepsi İngilizce kalıyor | `tr-TR` varyantı yok | `.resx` varyantı ekle | `ui-designer` |
| 3 | Form açılmıyor | XML bozuk | dosyayı tasarımcıda yeniden kaydet | `ui-designer` |
| 4 | Bir formda metin eksik | elle `Text` atanmış | kaynağı geri koy | `ui-designer` |
| 5 | `developer` `.resx`'i değiştirmiş | yetki ihlali | RED → devret | `reviewer` |

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | `.Form.cs` içinde sabit kullanıcı metni yok | gözden geçirme |
| 2 | `.resx` XML geçerli, anahtarlar eşleşiyor | dosya kontrolü |
| 3 | `tr-TR` varyantı mevcut ya da gerekçesi yazılı | dosya listesi |
| 4 | `new ResourceManager(...)` elle çağrı yok | grep = 0 |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · `.resx`'i yetkisiz ellemek | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
