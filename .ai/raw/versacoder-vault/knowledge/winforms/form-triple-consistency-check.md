---
title: "form-triple-consistency-check - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "XxxForm.cs + XxxForm.Designer.cs + XxxForm.resx üçlüsünün karşılıklı tutarlılığının 5 adımda denetimi ve hata eşlemesi"
---

# form-triple-consistency-check — Üçlü Tutarlılık Denetimi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [resx-file-structure](resx-file-structure.md)

> **Tek cümlelik sorumluluk:** *Bir formun `.cs` + `.Designer.cs` + `.resx` üçlüsünün birbirine tutarlı olduğu, 5 adımlık bir denetimle nasıl kanıtlanır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Her form **3 dosyadır** ve üçü birbirini doğrular: sınıf adı = dosya adı = `ComponentResourceManager(typeof(X))` tipi = `.resx` adı. Tutarlılık kanıtı **5 adımlık denetimle** üretilir: dosya varlığı, sınıf adı eşleşmesi, tek `InitializeComponent`, resx anahtar–üye eşleşmesi ve **çalıştırıp gözle doğrulama**. Denetim sonunda derleme 0 hata, form açılışında kaynak hatası yok, ekran designer ile aynı olmalıdır — "bitti" iddiası bu kanıtsız **RED**'dir (G5).

---

## §1 Bes adim

```text
1. 3 dosya ayni klasorde mi?        XxxForm.cs / XxxForm.Designer.cs / XxxForm.resx
2. sinif adi == dosya adi mi?        partial class XxxForm (iki dosyada da ayni)
3. Tek InitializeComponent mi?      .cs'te GOUDE var mi? (CS0102 -> varsa BOZUK)
4. resx anahtarlari uyeli tes mi?   <data name="uyye.Ozellik"> <-> designer'da o uyeyi ureten satir
5. calistir ve gozle dogrula        kaynak hatasi yok, layout designer ile ayni
```

| # | Adım | Komut / yöntem | Geçme ölçütü |
|---|---|---|---|
| 1 | Dosya varlığı | klasör listesi | 3 dosya yan yana |
| 2 | Sınıf adı | `partial class` satırı karşılaştırma | iki dosyada da aynı ad |
| 3 | Tek kaynak | kod taraması: `void InitializeComponent()` | **1** eşleşme (yalnız `.Designer.cs`) |
| 4 | Anahtar eşleşmesi | resx `<data name>` ↔ designer `resources.Get*` | 0 eksik anahtar |
| 5 | Kanıt | `dotnet build` + çalıştırma | 0 hata + ekran doğrulaması |

---

## §2 Hata esleme tablosu

| Gözlenen hata | Kırık halka | Düzeltme |
|---|---|---|
| `CS0102: 'X' zaten tanım içeriyor` | Adım 3 — `.cs`'e `InitializeComponent` sızmış | Gövdeyi `.Designer.cs`'e taşı / dosyayı geri al |
| `CS0246: 'XxxForm' tipi bulunamadı` | Adım 2 — sınıf adı ≠ dosya adı | Adı eşleştir ([partial-class-and-file-naming](partial-class-and-file-naming.md)) |
| `MissingManifestResourceException` | Adım 1/4 — resx adı ya da manifest adı | Dosyayı sınıfın yanına al, adı eşleştir |
| Tasarımcı "resource ... not found" | Adım 4 — anahtar kaymış | Tasarımcıda formu açıp kaydet |
| Form açılıyor ama etiketler boş | Adım 4 — `<data>` değeri boş | Properties'ten metni gir |
| Ekran tasarım ile farklı | Adım 5 — layout kodda elle oynanmış | [code-behind-zero-designer-code](code-behind-zero-designer-code.md) taraması |

---

## §3 Otomatiklestirilebilir denetim (kabuk)

```powershell
# Ayni klasorde uc dosyanin varligini ve tek InitializeComponent'i dogrula
Get-ChildItem .\Forms\*.Designer.cs | ForEach-Object {
  $base = $_.Name -replace '\.Designer\.cs$',''
  $ok = (Test-Path ".\Forms\$base.cs") -and (Test-Path ".\Forms\$base.resx")
  if (-not $ok) { Write-Warning "EKSIK UCLU: $base" }
}
# Beklenen: uyari 0
```

| Kanıt türü | Zorunlu mu |
|---|---|
| Uyarı = 0 (üçlü varlık) | Evet |
| `dotnet build` → 0 hata (net48 + net10.0-windows) | Evet |
| Form çalıştırılıp gözle doğrulandı | Evet (G5) |
| Kod taraması: `.cs`'te `SuspendLayout` = 0 | Evet |

---

## §4 net48 / net10.0-windows farki

| Denetim | `net48` | `net10.0-windows` |
|---|---|---|
| Üçlü dosya kuralı | **aynı** | aynı |
| Manifest adlandırma | **aynı** | aynı |
| Derleme kanıtı | ayrı build gerekir | ayrı build gerekir |
| Tasarımcı | VS tasarım zamanı | .NET designer host |

---

## §5 Kardesler

[designer-cs-anatomy](designer-cs-anatomy.md) · [resx-file-structure](resx-file-structure.md) · [designer-editing-rules](designer-editing-rules.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md)
