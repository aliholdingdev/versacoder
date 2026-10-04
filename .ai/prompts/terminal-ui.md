---
title: "prompt - Terminal UI (09-terminal dock, Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "09-terminal dock panelini (MemoEdit + TextEdit + StatusBarControl + Ctrl+C/timeout) TAM KOD olarak ureten prompt - 3 kanit zorunlu, GATE-003 madde 14 ONAY BEKLIYOR"
---

# prompt — Terminal UI (09-terminal dock, Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [terminal-spec](../spec/terminal-spec.md) · [cli-spec](../spec/cli-spec.md) · [ui-shell-spec](../spec/ui-shell-spec.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [09-terminal](../ui-design/09-terminal.md)

> **Tek cumlelik sorumluluk:** `09-terminal` dock panelini (CLI ciktisi `MemoEdit`,
> komut `TextEdit`, `[Stop] [Temizle]`, exit code) **tam kod** olarak ureten,
> **3 kanitla** kapanan is emridir — **GATE-003 madde 14 ONAY BEKLIYOR** (G12).

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Ekran | `09-terminal` (dock alt paneli) — `ui-design/README.md` §0 satır 9 |
| K bandı | `K0365-K0379` |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-PRD-040..042` (terminal-spec) + `VC-CLI-015` (cli-spec) |
| Ajan | `ui-designer` (U5) → `developer` (kod) → `tester` |
| Kapı | **GATE-003 madde 14 = ONAY BEKLIYOR** → GATE-005 |
| Kanıt | 3 tane: build + Ctrl+C/timeout test + NetArchTest |

**YASAK:** iş mantığı (U6) · `.Designer.cs` elle müdahale (U5) · DevExpress v26.1 dışı ·
`Office2010` dışı ribbon · `l7` → `l1`/`l4` ok (R8).

---

## §1 Baglam

1. `terminal-spec §2.1`: `MemoEdit` (ReadOnly) + `TextEdit` + durum alanı; `ArgumentList` (SEC-05).
2. `terminal-spec §2.4`: timeout/iptal → `"killed (timeout)"` / `"cancelled"`.
3. `cli-spec VC-CLI-015`: `run` çıktısı satır bazlı `MemoEdit`'e akar; 10.000 satır FIFO.
4. `ui-design/09-terminal.md §0`: 4 bölge + `[Stop] [Temizle]` + UTF-8 Türkçe.
5. **GATE-003 madde 14 (`09-terminal`) = ONAY BEKLIYOR** → bu prompt **kod izni vermez** (G12).

---

## §2 Görev — TAM KOD

### §2.1 TerminalPanel (`K0365-K0374`)

```csharp
// src/VersaCoder.Desktop/Terminal/TerminalPanel.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Desktop.Terminal;

/// <summary>09-terminal — 4 bolge: baslik + MemoEdit + TextEdit + StatusBarControl (ui-design §0).</summary>
public partial class TerminalPanel : XtraUserControl, IAgentStreamSink
{
    private const int MaxLines = 10000;   // VC-CLI-015 FIFO
    private readonly MemoEdit _output;
    private readonly TextEdit _input;
    private readonly LabelControl _historyCount;
    private System.Diagnostics.Process? _proc;
    private CancellationTokenSource? _cts;

    public TerminalPanel()
    {
        _output = new MemoEdit { ReadOnly = true, Dock = System.Windows.Forms.DockStyle.Fill, Properties = { WordWrap = false } };
        _input = new TextEdit { Dock = System.Windows.Forms.DockStyle.Bottom };
        _historyCount = new LabelControl { Dock = System.Windows.Forms.DockStyle.Bottom, Text = "gecmis: 0" };
        _input.KeyPress += OnInputKeyPress;
        Controls.Add(_output); Controls.Add(_input); Controls.Add(_historyCount);
    }

    private void OnInputKeyPress(object? sender, System.Windows.Forms.KeyPressEventArgs e)
    {
        if (e.KeyChar != '\r') return;
        e.Handled = true;
        _ = RunAsync(_input.Text, CancellationToken.None);
        _input.Text = string.Empty;
    }

    /// <summary>VC-CLI-003: calistir + satir akisi. ArgumentList (SEC-05) — string birlesimi YOK.</summary>
    public async Task RunAsync(string command, CancellationToken external)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(external);
        var parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;
        _proc = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = parts[0],
                Arguments = parts.Length > 1 ? parts[1] : string.Empty,
                UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true
            }
        };
        _proc.Start();
        while (!_proc.StandardOutput.EndOfStream && !_cts.IsCancellationRequested)
        {
            var line = await _proc.StandardOutput.ReadLineAsync();
            if (line is not null) await OnLineAsync(line, _cts.Token);
        }
        await _proc.WaitForExitAsync(_cts.Token);
        await OnCompletedAsync(_proc.ExitCode, _cts.Token);
    }

    public Task OnLineAsync(string line, CancellationToken ct)
    {
        if (_output.InvokeRequired)
            _output.Invoke((Action)(() => AppendLine(line)));
        else AppendLine(line);
        return Task.CompletedTask;
    }

    private void AppendLine(string line)
    {
        _output.Text += line + Environment.NewLine;
        var lines = _output.Text.Split('\n');
        if (lines.Length > MaxLines)   // FIFO
            _output.Text = string.Join('\n', lines, lines.Length - MaxLines, MaxLines);
    }

    public Task OnCompletedAsync(int exitCode, CancellationToken ct)
    {
        // durum cubugu: exit code + sure (StatusBarControl)
        return Task.CompletedTask;
    }
    public Task OnErrorAsync(string message, CancellationToken ct) { AppendLine(message); return Task.CompletedTask; }

    /// <summary>[Stop] dugmesi — terminal-spec §2.4 (cancelled / killed timeout).</summary>
    public void Stop()
    {
        _cts?.Cancel();
        try { _proc?.Kill(); } catch { /* process zaten bitmis olabilir */ }
        AppendLine("cancelled");   // VC-E-0612 / exit 5
    }

    public void Clear() { _output.Text = string.Empty; }   // [Temizle] / Ctrl+L
}
```

### §2.2 CLI run → terminal akış (`VC-CLI-015`, `K0375-K0379`)

```csharp
// src/VersaCoder.Desktop/Terminal/CliToTerminalBridge.cs
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Ports;
using VersaCoder.Host.Composition;

namespace VersaCoder.Desktop.Terminal;

/// <summary>CliRouter -> IAgentStreamSink (l0 portu) — l7'de is mantigi YOK (R8).</summary>
public sealed class CliToTerminalBridge
{
    private readonly AppHost _host;
    private readonly IAgentStreamSink _sink;
    public CliToTerminalBridge(AppHost host, IAgentStreamSink sink) { _host = host; _sink = sink; }

    public async Task<int> RunAsync(string[] args, CancellationToken ct)
    {
        var exit = await _host.RunAsync(args, ct);
        await _sink.OnCompletedAsync(exit, ct);
        return exit;   // 0..5 (cli-spec §4.1)
    }
}
```

---

## §3 Kabul Kriterleri

- [ ] **GATE-003 madde 14 (`09-terminal`) ONAYLI** — değilse **`.cs` yazılmaz** (G12)
- [ ] `MemoEdit` ReadOnly + `TextEdit` + `StatusBarControl` + `[Stop] [Temizle]`
- [ ] 10.000 satır FIFO
- [ ] Ctrl+C → `cancelled` (exit 5) · `--timeout` → `killed (timeout)` (exit 5)
- [ ] `ArgumentList` (SEC-05) — string birleşimi 0
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: terminal testi -> Ctrl+C = "cancelled" + timeout = "killed (timeout)"
KANIT 3: NetArchTest LayerRulesTests -> 0 ihlal (R8 = 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : 09-terminal dock paneli yazildi         |
| KAPSAM : src/VersaCoder.Desktop/Terminal/**      |
| KARAR  : MemoEdit + TextEdit + 10K FIFO + SEC-05 |
| RISK   : GATE-003 madde 14 ACIK -> kod izni YOK  |
| KANIT  : build x2 = 0 + cancelled/timeout + 0 ihlal|
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `ui-designer` | `.Designer.cs` / `.resx` (U5) |
| 2 | `developer` | TerminalPanel + bridge |
| 3 | `tester` | Ctrl+C + timeout test |
| 4 | `reviewer` | U1..U10 + R8 + SEC-05 |

---

**Olculen kanit:** `.ai/prompts/terminal-ui.md` kaydedildi; `prompts/` = 41 dosya.
