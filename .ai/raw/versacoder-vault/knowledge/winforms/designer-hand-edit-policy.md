---
title: "WinForms — .Designer.cs ve .resx Elle Düzenleme Politikası"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "`ui-designer`'ın `.Designer.cs`/`.resx` üzerindeki son söz hakkını, elle müdahalenin geri alınma biçimini ve tasarımcı dosyasının elle okunurken izlenecek kuralı"
---

# `.Designer.cs` / `.resx` Elle Düzenleme Politikası

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`.Designer.cs` ve `.resx` üzerinde **son söz `ui-designer`'ındadır**; `developer`, `architect`, `tester` bu dosyaları değiştirirse iş **RED** edilir ve değişiklik geri alınır. Elle müdahale **yalnız** tasarımcının çözemediği durumda ve **yalnız** `ui-designer` tarafından yapılır; sonraki tasarımcı kaydı bu müdahaleyi **silmez** (silineceği için tesadüfi düzeltmeler para kaybıdır).

| # | Dosya | X sahibi | Elle müdahale | Değişiklik sonrası |
|---|---|---|---|---|
| 1 | `XxxForm.Designer.cs` | `ui-designer` | yalnız `ui-designer`, gerekçeli | form tasarımcıda yeniden açılmalı |
| 2 | `XxxForm.resx` | `ui-designer` | yalnız `ui-designer`, anahtar korunarak | `.Designer.cs` ile birlikte |
| 3 | `XxxForm.cs` | `developer` | serbest (SRP içinde) | build + review |
| 4 | `Properties/Resources.resx` | `ui-designer` | `ui-designer` | proje geneli etki → iki bacak kontrol |

### §1.1 İhlal tespiti (ne zaman RED)

| # | Belirti | Bulgusu yapan | Sonuç |
|---|---|---|---|
| 1 | Tasarımcı dosyayı açmıyor | `ui-designer` | diff alınır, geri alınır |
| 2 | `InitializeComponent` içinde `// TODO` / iş mantığı | `reviewer` | `developer` RED |
| 3 | `.Form.cs` içinde `Controls.Add` | `reviewer` | form üçlüsü RED |
| 4 | `.resx` elle değiştirilmiş (`git diff` görünür) | `reviewer` | yetki ihlali → `ui-designer` |
| 5 | İki `InitializeComponent` tanımı | derleyici (CS0111) | build RED |

```powershell
# Elle müdahale taraması — kanıt üretir
git diff --name-only -- "*.Designer.cs" "*.resx"
Select-String -Path "src\**\*.cs" -Pattern "SuspendLayout\(|ResumeLayout\(|BeginInit\(|Controls\.Add\(" |
  Where-Object { $_.Path -notlike "*.Designer.cs" }
# Beklenen: 0 satır
```

### §1.2 Elle müdahale izi (ne zaman gerekir)

| # | Durum | Doğru davranış |
|---|---|---|
| 1 | DevExpress sürüm yükseltmesi `.Designer.cs`'i bozdu | `ui-designer` dosyayı tasarımcıda yeniden üretir |
| 2 | `.resx` bozuk XML | `ui-designer` dosyayı yeniden kaydeder |
| 3 | Olay `+=` yanlış bağlanmış | `ui-designer` tasarımcıdan bağlar |
| 4 | `developer` "küçük düzeltme" yapmak istiyor | **yapmaz**, `ui-designer`'a devreder |
| 5 | Acil hotfix, `ui-designer` yok | `DECISION BLOCKED` → kullanıcıya sor |

### §1.3 Tasarımcı dosyasını okuma kuralı

`.Designer.cs` genellikle 500+ satırdır; **tamamı bağlama yüklenmez**:

| # | Yöntem | Ne için |
|---|---|---|
| 1 | `grep` ile `this.btnXxx` ara | tek kontrolün tanımı |
| 2 | `offset`/`limit` ile yarısına kadar oku | `InitializeComponent` başlangıcı |
| 3 | `Dispose` bloğunu ayrı oku | temizlik doğrulaması |
| 4 | `.resx`'i `Select-String name=` ile tara | anahtar envanteri |

### §1.4 Geri alma

Elle yapılan ve kabul edilmeyen bir değişiklik `git checkout -- <dosya>` ile geri alınır; bu **yıkıcı işlem (G7)** sayılır ve geri alma **onay** gerektirir. Onay kaydı `log.md`'ye yazılır.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | `.Designer.cs`/`.resx` diff'i boş ya da `ui-designer` imzalı | `git diff` |
| 2 | `.Form.cs` taraması 0 eşleşme | §1.1 komutu |
| 3 | Tasarımcı dosyayı açıyor | tasarımcı kanıtı |
| 4 | Geri alma varsa onay kaydı | `log.md` |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · yetkisiz `.Designer.cs`/`.resx` değişikliği | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) · onaysız geri alma (G7) | RED |
