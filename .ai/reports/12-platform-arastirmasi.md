---
title: "Platform Arastirmasi - net48 / net10 / DevExpress / WinForms Designer"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Web kaynakli platform bulgulari: net48 icin Windows siniri, net10 icin destekli Windows surumleri, ribbon stilleri, Form.Designer.cs uretim kurali ve 25.2.8/26.1 cakismasi"
---

# Platform Arastirmasi

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu projenin iki bacakli TFM karari
> (net48 + net10.0-windows) web kanitlariyla dogru mu, DevExpress ve
> Designer.cs kurallari nedir* sorusunun kaynakli cevabidir.

---

## §0 Ozet (Answer First)

- **net48 bacaği yaşar ama OS sınırı var:** .NET Framework 4.8, ana işletim
  sistemi yaşam döngüsüne tabidir; Windows 8/8.1 **destek dışıdır** (Win 8'e
  gelen en son .NET FW = 4.6.1).
- **net10 bacağı Win 10/11 ister:** .NET 10 yalnız Windows 11 (23H2/24H2/25H2/26H1)
  ve Windows 10'un **LTSC/Enterprise** sürümlerinde desteklenir; Windows 8.1'de **desteklenmez**.
- **Ribbon kuralı doğru:** `RibbonControlStyle.Office2010` geçerli bir enum
  üyesidir; Ribbon'ın en iyi taşıyıcısı `RibbonForm`'dur (DevExpress resmi dokümanı).
- **Form UI kodu `Form.Designer.cs` içindedir:** `InitializeComponent()` tek
  yöntemdir; el ile düzenleme yapılsa da tasarımcı yeniden yazar — vault U5 kuralı
  (UI kodu `ui-designer` sahipliğinde) bu gerçeğe dayanır.
- **ÇAKIŞMA:** istekte `DevExpress 25.2.8`, vault'ta (CLAUDE G3) `v26.1` yazıyor
  → `CONFLICT DETECTED`, tek soru §6'da.

---

## §1 net48 — destek sınırı (bacak 1)

| # | Bulgu | Kaynak etiketi |
|---|---|---|
| 1 | .NET Framework 4.8, Windows bileşeni olarak **ana OS yaşam döngüsünü izler** (2019-04-18 çıkış; bitiş tarihi OS'a bağlı) | `WEB_VERIFIED — learn.microsoft.com/en-us/lifecycle/faq/dotnet-framework (2026-10-04)` |
| 2 | Windows 8.1'e gelen en son .NET FW **4.8**'dir ama **OS destek dışı**; kurulumda eski indirme bağlantıları bulunmayabilir | `WEB_VERIFIED — learn.microsoft.com/en-us/dotnet/framework/install/on-windows-and-server (2026-10-04)` |
| 3 | Windows 8 için en son desteklenen .NET FW **4.6.1**'dir (4.8 DEĞİL) | `WEB_VERIFIED — aynı sayfa (2026-10-04)` |
| 4 | .NET Framework 4.8.1 yalnız Windows 10 20H2+ ve Windows 11'de; Windows on Arm **yalnız Windows 11** | `WEB_VERIFIED — lifecycle FAQ (2026-10-04)` |

**Proje kararı için anlamı:** `net48` bacağı, Win 8/8.1 hedeflemiyorsa sorun
değildir; hedef **Windows 10/11** ise `net48` desteklenir ve vault kararı
(G6 çift bacak) geçerli kalır. Windows 8 hedefi olsaydı `net48` bile
kurulamazdı (kutu dışına düşer).

## §2 net10 — desteklenen Windows sürümleri (bacak 2)

| # | Sürüm | .NET 10 | Not |
|---|---|---|---|
| 1 | Windows 11 (26H1, 25H2, 24H2, 23H2 E/Edu) | ✔ x64, x86, Arm64 | tam destek |
| 2 | Windows 10 (21H2, 1809, 1607 **LTSC/Enterprise**) | ✔ x64, x86, Arm64 | Home/Pro **dışı** (EOS 2025-10-14, ESU ile uzatılabilir) |
| 3 | Windows 8.1 | ❌ | hiçbir .NET sürümünde yok |
| 4 | Windows Server 2012/R2 (ESU) | ✔ x64 | ESU 2026-10'da biter |

Kaynak: `WEB_VERIFIED — github.com/dotnet/core release-notes/10.0/supported-os.md + learn.microsoft.com/en-us/dotnet/core/install/windows (2026-10-04)`.

**Proje kararı için anlamı:** `net10.0-windows` bacağının gerçek asgari
gereksinimi **Windows 10 1809/21H2 Enterprise-LTSC** ya da **Windows 11**'dir.
Kutuya (installer) yazılacak satır: `Destek: Windows 10 (LTSC/Enterprise 1809+) | Windows 11`.

## §3 DevExpress — ribbon stili ve form tabanı

| # | Bulgu | Kaynak etiketi |
|---|---|---|
| 1 | `RibbonControl.RibbonStyle` enum'ı: `Default, Office2007, Office2010, Office2013, MacOffice, TabletOffice, TabletOfficeEx, OfficeUniversal, Office2019, Office365` | `WEB_VERIFIED — docs.devexpress.com/WindowsForms/DevExpress.XtraBars.Ribbon.RibbonControlStyle (2026-10-04)` |
| 2 | v17.1+'da `Default = Office2013`; **Office2010 seçimi açıkça yapılmalıdır** (varsayılan değil) | `WEB_VERIFIED — RibbonControl.RibbonStyle Property (2026-10-04)` |
| 3 | Ribbon için en iyi form **`RibbonForm`**'dur; Office2010 stili BackstageViewControl kullanır | `WEB_VERIFIED — docs.devexpress.com/WindowsForms/2492/.../ribbon-control (2026-10-04)` |
| 4 | Örnek kod: `public partial class Form1 : RibbonForm` + `InitializeComponent()` | `WEB_VERIFIED — github.com/DevExpress-Examples/...e2759 Form1.cs (2026-10-04)` |

**Vault etkisi:** CLAUDE G3 (`RibbonForm` + `RibbonControlStyle.Office2010`)
web ile **doğrulandı**; sürümnumarası çelişkisi §6'da.

## §4 Form UI kodu — `Form.Designer.cs` kuralı

| # | Kural | Kaynak etiketi |
|---|---|---|
| 1 | Form üçlüsü: `Form.cs` (mantık) + `Form.Designer.cs` (UI) + `Form.resx` (kaynak) | `WEB_VERIFIED — github.com/dotnet/winforms docs/designer (2026-10-04)` |
| 2 | Tüm tasarım kodu **tek yöntemde** yaşar: `InitializeComponent()`; constructor'da koşulsuz çağrılır | `WEB_VERIFIED — devblogs.microsoft.com/dotnet/winforms-codegen-update (2026-02-23 arşiv)` |
| 3 | VS 17.5+ Roslyn tabanlı üretim: `this.` niteliklendirici varsayılan olarak **kalkar**; `.editorconfig` ile eski stil korunabilir (`[*.designer.cs]` bloğu) | `WEB_VERIFIED — dotnet/winforms modernization-of-code-behind (2026-10-04)` |
| 4 | Tasarımcı dosyayı her açılışta yeniden yazar; el ile değişiklik **silinir** → olay işleyicileri `Form.cs`'e yazılır | `WEB_VERIFIED — learn.microsoft.com/en-us/answers/questions/5840857 + StackOverflow 3786512 (2026-10-04)` |

**Vault etkisi:** istekteki "*tüm form UI design kodları `***form***.designer.cs`'e
yazılır" kuralı doğru; buna ek kural: **event handler ve iş mantığı asla
`.Designer.cs`'e yazılmaz** (yeniden üretimde kaybolur) — U5 ile uyumlu.

## §5 Kaynak listesi

| # | Kaynak | URL |
|---|---|---|
| 1 | MS Lifecycle FAQ (.NET FW) | `https://learn.microsoft.com/en-us/lifecycle/faq/dotnet-framework` |
| 2 | MS .NET Framework system requirements | `https://learn.microsoft.com/en-us/dotnet/framework/get-started/system-requirements` |
| 3 | .NET 10 supported-os | `https://github.com/dotnet/core/blob/main/release-notes/10.0/supported-os.md` |
| 4 | DevExpress RibbonControlStyle enum | `https://docs.devexpress.com/WindowsForms/DevExpress.XtraBars.Ribbon.RibbonControlStyle` |
| 5 | DevExpress Ribbon Styles | `https://docs.devexpress.com/WindowsForms/14624/.../ribbon-styles` |
| 6 | WinForms designer codegen (Roslyn) | `https://github.com/dotnet/winforms/blob/main/docs/designer/modernization-of-code-behind-in-OOP-designer/` |

## §6 CONFLICT DETECTED — DevExpress sürümü

```text
KAYNAK A : istek metni       -> "DevExpress 25.2.8"
KAYNAK B : CLAUDE.md G3/G8   -> "DevExpress v26.1 tek surum"
ETIKET   : CONFLICT DETECTED (G10) - sessiz secim YASAK
SORU     : Hangisi gecerli: A) 25.2.8  B) 26.1  C) bagimsiz arastirma ile karar
```

> Cevap gelmeden `Directory.Packages.props` satırı yazılmaz ve bu rapor
> §6'sı `DOĞRULAMA GEREKLI` olarak kalır (G9).
