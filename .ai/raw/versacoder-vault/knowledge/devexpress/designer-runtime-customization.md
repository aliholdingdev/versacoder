---
title: "designer-runtime-customization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress Form uclusu, Designer dosyasi anatomiisi, nerede ne yazilir (DX-10/DX-11) ve runtime ozellestirme siniri - iki bacak farki"
---

# designer-runtime-customization — Designer Dosyası ve Runtime Özelleştirme (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [serialization-layout-persistence](serialization-layout-persistence.md)

> **Tek cumlelik sorumluluk:** *Form üçlüsünde hangi satır nerede yaşar, runtime'da ne özelleştirilebilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Her DevExpress formu **üç dosyadan** oluşur: `.cs` (olay/akış), `.Designer.cs` (`InitializeComponent`), `.resx` (metin/DPI kaynağı). Yapısal her değişiklik **tasarımcıdan** yapılır; `.Designer.cs`'e elle yazılan satır yeniden üretimde silinir (DX-10), iş mantığı oraya konursa kaybolur (DX-11). Runtime özelleştirme **kod dosyasında** ve **kontrol eklemek/özellik atamak** düzeyinde kalır; kalıcı düzen `.resx`/layout XML'e gider (bkz. [serialization-layout-persistence](serialization-layout-persistence.md)).

---

## §1 Form Uclusu Anatomisi

```
SessionForm.cs              | olaylar, akis, servis cagrisi      | elde yazilir
SessionForm.Designer.cs     | InitializeComponent + Suspend/     | tasarimcida uretilir
                            | Resume + BeginInit/EndInit + Name  | elle edit YASAK (DX-10)
SessionForm.resx            | metin, ikon, DPI kaynagi           | uretilir/duzenlenir
        |
        +--> ayni klasor + ayni sinif adi (partial) - DX-05
```

| Bölüm | Ne var | Elle edit |
|---|---|---|
| `InitializeComponent()` | kontrol yaratımı, özellik atama, `resources.ApplyResources` | **YASAK** |
| `SuspendLayout/ResumeLayout` çifti | layout kilidi | **YASAK** |
| `BeginInit/EndInit` çifti | grid/kolon nesneleri | **YASAK** |
| Olay bağlantıları (`+= ...`) | handler kaydı | Olaylar penceresinden |
| `FormName.cs` handler gövdesi | iş akışı | **serbest** (ince kalmalı) |

---

## §2 Kod (yasal runtime kodu)

```csharp
public partial class SessionForm : RibbonForm   // RibbonForm - DX-04
{
    private readonly ISessionService _sessions;  // DI - arayuz (SOL-05)

    public SessionForm(ISessionService sessions)
    {
        _sessions = sessions;
        InitializeComponent();                   // Designer'in urettigi tek cagri
    }

    private void OnBarNewItemClick(object sender, ItemClickEventArgs e)
        => _sessions.Create();                   // ince handler - DX-13
}
```

| Runtime özelleştirme | Yapılabilir mi | Not |
|---|---|---|
| Kodda `Controls.Add(yeniControl)` | evet | sahiplik Designer'a bırakılır (DX-12) |
| Kodda `gridView1.Columns[k].Visible = ...` | evet | **dizilim** Designer'da, durum kodda |
| Kodda `ribbonControl1.RibbonStyle = ...` | **hayır** | sabit `Office2010` (DX-03) |
| `.Designer.cs`'e iş mantığı | **hayır** | DX-11 — silinir/kaybolur |
| Elle `control.Dispose()` | **hayır** | DX-12 — `ObjectDisposedException` |

---

## §3 Ozellik Tablosu (Designer'in ürettiği alanlar)

| Alan | Tipik değer | Kim korur |
|---|---|---|
| `Name` | `"ribbonControl1"` | `ui-designer` |
| `TabIndex` / `TabStop` | sıralı | `ui-designer` (erişilebilirlik) |
| `Dock` / `Anchor` | `DockStyle.Fill` | `ui-designer` (DX-14) |
| `Resources.ApplyResources(c, "name")` | `.resx` anahtarı | `ui-designer` |
| `AutoScaleMode` | `Dpi` / `Font` (Designer değeri) | **dokunulmaz** (DX-14) |

> **KURAL:** `.Designer.cs` diff'i yalnız tasarımcı kaynaklı olmalı; PR'da elle yazılmış satır gördüysen **RED** (DX-10).

---

## §4 Runtime Ozellestirme Akisi

| Adım | Nerede | Kanıt |
|---|---|---|
| Yeni kontrol/kolon ekleme | **Designer** | 3 dosya birlikte değişti |
| Durum değiştirme (`Visible`, `Caption`) | `.Form.cs` (handler/servis) | ince handler |
| Kalıcı düzen | `.resx` / layout XML | [serialization-layout-persistence](serialization-layout-persistence.md) |
| Metin çevirisi | `.resx` kultur dosyasi | [localizing-ui-text](localizing-ui-text.md) |

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `.Designer.cs` | **aynı dosya** iki bacakta da derlenir | aynı |
| Üretilen dil sözdizimi | **C# 7.3** ile sınırlı tut (LangVersion 7.3) | C# 14 yutabilir ama ortak dosya 7.3 kalır |
| `#if` | yalnız platform farkı (ör. DPI/interop) | aynı |
| Designer desteği | Visual Studio WinForms designer | designer desteğine `NEEDS_VALIDATION` (VS sürümüne bağlı) |

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| MVVM (code-behind sınırı) | [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) |
| Düzen kaydetme | [serialization-layout-persistence](serialization-layout-persistence.md) |
| Yerelleştirme (resx) | [localizing-ui-text](localizing-ui-text.md) |
| Ribbon stili (sabit) | [theme-manager-office2010](theme-manager-office2010.md) |
