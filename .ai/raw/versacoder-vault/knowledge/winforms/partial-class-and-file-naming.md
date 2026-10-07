---
title: "partial-class-and-file-naming - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Form dosya adı = sınıf adı kuralı, partial class bütünlüğü, tek sınıf/dosya, #region yasağı ve namespace eşlemesi"
---

# partial-class-and-file-naming — Dosya Adı, Sınıf Adı, `partial`

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [code-behind-zero-designer-code](code-behind-zero-designer-code.md)

> **Tek cümlelik sorumluluk:** *Form dosyalarının adlandırılması, `partial` sınıf bütünlüğü ve dosya-içi düzen yasağı (`#region`) hangi kurallara bağlıdır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Bir formun üç dosyası da **aynı sınıf adını** taşır: `SessionListForm.cs` + `SessionListForm.Designer.cs` + `SessionListForm.resx` ve üçü de **aynı namespace**'dedir. `partial` anahtarı iki dosyada da **yazılıdır**; birinde eksikse derleme `CS0260` ile patlar. Dosya içinde **ikinci sınıf tanımlanmaz** (tek sınıf = tek dosya), `#region` **kullanılmaz** (developer DoD madde 6). Adlar ASCII, PascalCase'dir; dosya sistemi adı ile sınıf adı birebir aynı olduğu için `typeof(X)` → `X.resx` eşlemesi kendiliğinden doğru çalışır.

---

## §1 Dosya -> sinif eslemesi

| Dosya | Sınıf | `partial` | İçerik sahibi |
|---|---|---|---|
| `SessionListForm.cs` | `SessionListForm` | ✅ | developer (iş mantığı) |
| `SessionListForm.Designer.cs` | `SessionListForm` | ✅ | ui-designer (tek `InitializeComponent`) |
| `SessionListForm.resx` | — (sınıf adına adanmış) | — | ui-designer (ResourceManager anahtarları) |
| `SessionEditForm.cs` | `SessionEditForm` | ✅ | developer |
| `Program.cs` | `Program` | tek dosya | composition root ([composition-root-host-startup](composition-root-host-startup.md)) |

> **KURAL:** Dosya adı ile sınıf adı aynı değilse `CS0260`/tasarımcı "class not found" ile karşılaşırsın; düzeltme **dosya adını sınıf adına getirmektir**, sınıfı değiştirmek değil.

---

## §2 Kural tablosu

| # | Kural | Yasak | Denetim |
|---|---|---|---|
| 1 | Dosya adı = sınıf adı (PascalCase, ASCII) | `form-ana.cs`, `Form1.cs` yerine gerçek ad | klasör listesi |
| 2 | `partial` iki dosyada da | Tek taraflı `partial` | kod taraması: `partial class` = 2 |
| 3 | Dosyada tek sınıf | Yardımcı sınıf gömmek | kod incelemesi |
| 4 | `#region` yok | Görsel bölümleme (designer dosyası da hariç) | kod taraması: `#region` = 0 |
| 5 | Namespace = klasör yolu | `namespace VersaCoder.UI` dışı sürüklenme | kod incelemesi |
| 6 | C# 7.3 uyumluluğu | `record`, target-typed `new`, `using var` | net48 build |

---

## §3 Ornek (her iki dosya)

```csharp
// SessionListForm.cs
namespace VersaCoder.UI.Forms
{
    public partial class SessionListForm : RibbonForm          // partial YAZILI
    {
        private readonly ISessionQuery _sessions;
        public SessionListForm() { InitializeComponent(); }
        public SessionListForm(ISessionQuery s) : this() { _sessions = s; }
    }
}
```

```csharp
// SessionListForm.Designer.cs (parca)
namespace VersaCoder.UI.Forms
{
    partial class SessionListForm                               // partial YAZILI
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { ... components?.Dispose(); }
        private void InitializeComponent() { ... }             // TEK KAYNAK
    }
}
```

| Aykırılık | Sonuç |
|---|---|
| Bir tarafta `partial` yok | `CS0260` — build 0 hata sağlanamaz |
| `#region` eklendi | reviewer `LOW` → kuyruk; kod stili reddi |
| İkinci sınıf `.cs`'e gizlendi | SRP + test edilebilirlik ihlali |
| Namespace sapması | `typeof` → yanlış manifest adı, resx bulunamaz |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` (C# 14) |
|---|---|---|
| `partial` davranışı | **aynı** | aynı |
| Partial metot kuralları | Geri dönüş/değişken olmayan, özel erişimsiz | Kapsam genişler — **paylaşılan dosyada yeni özellik kullanma** |
| `CS0260` / derleme hataları | **aynı** | aynı |
| `#if` | Yalnız platform API farkı | Yalnız platform API farkı |

> **NOT:** Ortak dosyada C# 8+ sözdizimi (`using var`, `??=`, switch expression) net48 bacağını kırar; dil özellikleri **ayrılmış dosyada** ya da `#if NET` arkasında kullanılır (yalnız platform farkı ise).

---

## §5 Kardesler

[designer-cs-anatomy](designer-cs-anatomy.md) · [code-behind-zero-designer-code](code-behind-zero-designer-code.md) · [form-triple-consistency-check](form-triple-consistency-check.md) · [designer-editing-rules](designer-editing-rules.md)
