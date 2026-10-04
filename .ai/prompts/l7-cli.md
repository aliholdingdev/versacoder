---
title: "prompt - l7 Desktop + CLI (Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l7 Desktop (RibbonForm + 9 ekran) + CLI (6 komut) katmanini TAM KOD olarak ureten prompt - 3 kanit zorunlu, GATE-003 onayi zorunlu, DevExpress v26.1 + Office2010"
---

# prompt — l7 Desktop + CLI (Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [ui-shell-spec](../spec/ui-shell-spec.md) · [cli-spec](../spec/cli-spec.md) · [terminal-spec](../spec/terminal-spec.md) · [cli-console-spec](../spec/cli-console-spec.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [layer-boundary-check](../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** l7 `Desktop` (RibbonForm + 9 ekran) ve `CLI` (6 komut)
> katmanini **tam kod** olarak ureten, **GATE-003 onayi** ve **3 kanitla** kapanan is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l7` — `VersaCoder.Desktop` + `VersaCoder.CLI` |
| K bandı | `K0350-K0399` (Desktop) + `K0750-K0799` (CLI) |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-PRD-001..NNN` (versacoder-spec §4) + `VC-U-0901..0912` (cli-console-spec) + `VC-CLI-001..016` |
| Ajan | `ui-designer` (ekran) → `developer` (kod) → `tester` |
| Kapı | **GATE-003 (09-terminal madde 14 ONAY BEKLIYOR)** → GATE-004 → GATE-005 |
| Kanıt | 3 tane: build + ekran smoke + NetArchTest |

**YASAK:** iş mantığı · `.Designer.cs`/`.resx` elle müdahale (U5) · DevExpress **v26.1 dışı** ·
ribbon stili `Office2010` dışı · `l7` → `l1`/`l4` doğrudan ok (R8).

---

## §1 Baglam

1. `TODOS/05 §0` satır 8: **l7 = 9 ekran (onaylı wireframe)**, kapı `GATE-003 +`.
2. `ui-rules U1..U10`: `RibbonForm` + `Office2010` + v26.1 + U5 sahipliği.
3. `ui-design/README.md`: **11 wireframe** (01..07 + 08-ascii + 08-terminal + 09-cli-console + 10-ai-chat-console).
4. `cli-spec §2.1`: 6 komut (`new/build/run/layer/agent/vault`) + exit 0-5.
5. **`GATE-003` madde 14 (`09-terminal`) = ONAY BEKLIYOR** → bu prompt **kod izni vermez** (G12).

---

## §2 Görev — TAM KOD

### §2.1 CLI komutları (`VersaCoder.CLI`, `K0750-K0764`)

```csharp
// src/VersaCoder.CLI/Program.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Host.Composition;

namespace VersaCoder.CLI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };   // VC-CLI-015 Ctrl+C
        await using var host = AppHost.Create(args);
        return await host.RunAsync(args, cts.Token);   // exit 0..5 (cli-spec §4.1)
    }
}
```

### §2.2 MainForm (`RibbonForm`, `K0350-K0364`)

```csharp
// src/VersaCoder.Desktop/MainForm.cs
using DevExpress.XtraBars.Ribbon;
using System.Windows.Forms;

namespace VersaCoder.Desktop;

/// <summary>Ana pencere — U1 RibbonForm + U2 Office2010 (ui-rules).</summary>
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        ribbon.RibbonStyle = RibbonControlStyle.Office2010;   // U2, tek stil (ADR-004)
    }
}
```

> **U5:** `.Designer.cs` / `.resx` **yalnız `ui-designer`** — bu prompt `developer`a elle
> müdahale **yaptırmaz**; Designer üretimi `ui-designer` işidir.

### §2.3 Terminal dock (`09-terminal`, `K0365-K0379`)

```csharp
// src/VersaCoder.Desktop/Terminal/TerminalPanel.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Desktop.Terminal;

/// <summary>09-terminal — MemoEdit (ReadOnly) + TextEdit + StatusBarControl (terminal-spec §2.1).</summary>
public partial class TerminalPanel : XtraUserControl, IAgentStreamSink
{
    private readonly MemoEdit _output;
    private readonly TextEdit _input;
    public TerminalPanel()
    {
        _output = new MemoEdit { ReadOnly = true, Dock = System.Windows.Forms.DockStyle.Fill };
        _input = new TextEdit { Dock = System.Windows.Forms.DockStyle.Bottom };
        _input.KeyPress += (_, e) => { if (e.KeyChar == '\r') { RunCommand(_input.Text); e.Handled = true; } };
        Controls.Add(_output); Controls.Add(_input);
    }

    public Task OnLineAsync(string line, CancellationToken ct)
    {
        _output.Invoke((Action)(() => _output.Text += line + Environment.NewLine));   // UI thread marshal
        return Task.CompletedTask;
    }
    public Task OnCompletedAsync(int exitCode, CancellationToken ct) { /* durum: exit */ return Task.CompletedTask; }
    public Task OnErrorAsync(string message, CancellationToken ct) { /* hata */ return Task.CompletedTask; }
    private void RunCommand(string text) { /* CliRouter'a gonder */ }
}
```

---

## §3 Kabul Kriterleri

- [ ] **GATE-003 madde 14 (`09-terminal`) ONAYLI** — değilse **`.cs` yazılmaz** (G12)
- [ ] `MainForm : RibbonForm` + `RibbonControlStyle.Office2010` (U1+U2)
- [ ] DevExpress **yalnız v26.1** (U3, DX-01)
- [ ] `.Designer.cs` elle dokunulmadı (U5)
- [ ] CLI 6 komut + exit 0-5 (cli-spec §4.1)
- [ ] `l7` → `l1`/`l4` doğrudan ok = **0** (R8)
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet run --project VersaCoder.CLI -- --version -> exit 0 (smoke)
KANIT 3: NetArchTest LayerRulesTests -> 0 ihlal (R8 = 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l7 Desktop + CLI yazildi                |
| KAPSAM : src/VersaCoder.Desktop/** + CLI/**      |
| KARAR  : RibbonForm + Office2010 + v26.1 (U1-U3) |
| RISK   : GATE-003 madde 14 acik -> 09-terminal   |
|          icin kod YASAK (G12)                    |
| KANIT  : build x2 = 0 + CLI --version=0 + 0 ihlal|
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `ui-designer` | `.Designer.cs` / `.resx` (U5) |
| 2 | `developer` | MainForm + TerminalPanel + CLI |
| 3 | `tester` | Smoke + NetArchTest |
| 4 | `reviewer` | U1..U10 + R8 |

---

**Olculen kanit:** `.ai/prompts/l7-cli.md` kaydedildi; `prompts/` = 39 dosya.
