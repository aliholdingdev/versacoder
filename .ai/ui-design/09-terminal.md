---
title: "UI Wireframe 09 - Terminal (CLI Dock Paneli)"
type: spec
category: ui
status: active
version: 1.0.0
updated: 2026-10-02
authority: reference
scope: "09-terminal ekraninin wireframe'i: dock alt paneli MemoEdit (ReadOnly) + TextEdit komut girdisi + gecmis + durum alani + Stop/Temizle dugmeleri - CLI sistemi terminalden kullanilir, VC-CLI-015 baglantisi ve GATE-003 onay alani - onaysiz tek satir .cs yazilmaz"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 09 — Terminal (CLI Dock Paneli)

**Zorunlu Baglantilar:** [ui-design/README](readme.md) · [02-main-ide](02-main-ide.md) · [06-agent-panel](06-agent-panel.md) · [01-start-page](01-start-page.md) · [terminal-spec](../spec/terminal-spec.md) · [cli-spec](../spec/cli-spec.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [WORKFLOW](../governance/workflow.md)

> **Tek cumlelik sorumluluk:** *CLI ciktisinin dock alt panelinde nasil goruntulendigini, komut girdisi ve gecmisin nasil calistigini, Ctrl+C/timeout durumlarinda ne oldugunu tanimlar.*

---

## §0 Ozet (Answer First)

`09-terminal` = **1 dock paneli, 3 bolge, 1 komut cubugu**: ust baslik + `MemoEdit` (ReadOnly, satir akisi) + `TextEdit` (komut girdisi, â†‘/â†“ gecmis) + alt durum alani + `[Stop] [Temizle]` dugmeleri.

**Karakter seti UTF-8 + Turkce** (`ç ğ ı ö ş ü` ve `| - + +` kutular) — terminal-spec §2.1 ile ayni.

**Durum: GATE-003 ONAY BEKLIYOR** — bu wireframe kod izni icin **girdidir**; `checklists/gate-003-ui-onay.md` 13/13 tamamlaninca acilir (G12).

| Bolge | Kontrol (teklif) | Yerlesim |
|---|---|---|
| Ust | Baslik `Terminal — running/bitti` + cwd + `[Stop] [Temizle]` | Panel ustu (bar) |
| Merkez | `MemoEdit` (ReadOnly, satir bazli akis, scroll-down otomatik) | Dock Bottom, ~%70 yukseklik |
| Alt-1 | `TextEdit` (komut girdisi) + `Label` (gecmis sayaci) | `MemoEdit` alti, ~40 px |
| Alt-2 | `StatusBarControl` (exit code, sure, kalan satir) | Panel en alti |

> **Kontrol notu (ui-designer):** `MemoEdit` mi `RichEditControl` mu — terminal-spec §7/5'te **DOGRULAMA GEREKLI** (renkli cikti icin `RichEditControl` gerekebilir). Bu wireframe `MemoEdit` ile cizildi; renk lazimsa `RichEditControl`'e cevrilir, **onay bu satirda istenir**.

---

## §1 Wireframe (ASCII)

```text
+--------------------------------------------------------------+
| [>] Terminal — running   cwd: C:\versacoder\MyApp  [Stop] [Temizle] |
+--------------------------------------------------------------+
| $ versacoder build                                            |
|   Determining projects to restore...                          |
|   Restored C:\versacoder\MyApp\MyApp.csproj (in 1.2 sec).     |
|   MyApp -> bin\Debug\net10.0-windows\MyApp.dll                |
|   MyApp -> bin\Debug\net48\MyApp.dll                          |
|                                                                |
|   Build succeeded.                                             |
|   0 Warning(s) 0 Error(s)                                      |
|                                                                |
| $ versacoder layer list --band Foundation                      |
|   K0000  VersaCoder.Abstractions      l0  architect  planned  |
|   K0050  VersaCoder.Domain            l1  developer  planned  |
|   ...                                                          |
|                                                                |
| $ versacoder run dotnet -- test                                |
|   Starting test run...                                         |
|   Passed!  - Failed: 0, Passed: 12, Skipped: 0 (1.4 sec)       |
|                                                                |
| _                                      <- imlec (TextEdit)    |
+--------------------------------------------------------------+
| > versacoder [gecmis: 12]                                      |
+--------------------------------------------------------------+
| exit: 0   sure: 3.4s   satir: 10.000/10.000   UTF-8          |
+--------------------------------------------------------------+
```

**Komut girdisi ornegi (TextEdite yazilan):**

```text
> versacoder build --format json
```

**Timeout ornegi (Ctrl+C / Stop):**

```text
$ versacoder run dotnet -- build --timeout 5
  ... derleme devam ediyor ...
  killed (timeout) after 5s     <- terminal-spec §2.4
  exit: 5
```

**Izin reddi ornegi (VC-CLI-016):**

```text
$ versacoder run rm -rf bin
  permission denied (tool-spec deny)
  exit: 4
```

---

## §2 Davranis

### §2.1 Akis (satir bazli)

| # | Olay | Ne olur | Kanit |
|---|---|---|---|
| 1 | `run` komutu girilir (Enter / CLI cagrisi) | `Process.Start` (ArgumentList, SEC-05) | CLI process baslar |
| 2 | stdout satiri gelir | `MemoEdit`'e **append** (async, UI thread marshal) | satir sayisi +1 |
| 3 | 10.000 satir asilir | eski satir dus (FIFO) | MemoEdit 10.000 satir |
| 4 | Ctrl+C / `[Stop]` | `CancellationToken` + process kill | exit 5 + "cancelled" |
| 5 | `--timeout` asilir | kill + "killed (timeout)" | exit 5 |
| 6 | Bitti | durum alani "bitti" + exit code | exit 0..4 |

### §2.2 Komut girdisi + gecmis

| # | Tus | Ne yapar |
|---|---|---|
| 1 | `Enter` | `TextEdit`'teki satiri calistir (`VC-CLI-003`) |
| 2 | `â†‘` / `â†“` | Gecmisten bir onceki / sonraki (`history.txt`, 1000 satir) |
| 3 | `Tab` | Komut tamamlama (6 ana komut) |
| 4 | `Ctrl+L` | `MemoEdit` temizle (`[Temizle]` ile ayni) |
| 5 | `Ctrl+C` (girdide) | calisan komutu iptal (exit 5) |

### §2.3 Bolgeler (dock)

| Bolge | Sirada | Komsu |
|---|---|---|
| Sol | Solution Explorer (`03`) | — |
| Merkez | Editor (`04`) | — |
| Sag-ust | AI Chat (`05`) | — |
| Sag-alt | Agent Panel (`06`) | AI Chat altinda |
| **Alt (BU EKRAN)** | **Terminal (`08`)** | tum panel altinda, ~%20 |

> **02-main-ide ile uyum:** `ui-shell-spec §0` "Terminal alt" der — bu wireframe o bolgeyi doldurur.

---

## §3 DevExpress Kontrolleri (devexpress-rules DX-09)

| # | Kontrol | Rol | Neden |
|---|---|---|---|
| 1 | `MemoEdit` (ReadOnly) | Cikti goruntusu | satir bazli ekleme + scroll (terminal-spec) |
| 2 | `TextEdit` | Komut girdisi | tek satir, Enter tetikler |
| 3 | `LabelControl` | Gecmis sayaci | `gecmis: 12` |
| 4 | `SimpleButton` Ã—2 | `[Stop]` `[Temizle]` | iptal + temizleme |
| 5 | `StatusBarControl` | exit code / sure / satir | alt cubuk |
| 6 | `DefaultLookAndFeel` | tema | theming-spec |

**YASAK:** `TextBox` (yetersiz), `RichTextBox` (WinForms), elle `Cursor`/`Thread` (ui-rules).

---

## §4 Iliskiler (karsi ekranlar)

| Iliski | Yon | Ne tasir |
|---|---|---|
| `09 -> 02-main-ide` | dock | Terminal alt bolgede yasar |
| `09 -> 06-agent-panel` | task | `run` ciktisi task-queue'ya duser (terminal-spec §5) |
| `09 -> 04-code-editor` | stdout | `build` hatasi editor'de satir isaretler |
| `09 -> 05-ai-chat` | agent | `agent run` ajan sohbeti akar |
| `09 -> cli-spec` | contract | `VC-CLI-015` (run -> terminal) |
| `09 -> tool-spec` | izin | `run` + `deny` = exit 4 (VC-CLI-016) |

---

## §5 GATE-003 Onay Alani

- [ ] Wireframe `09-terminal` 100% (`ui-rules` U1)
- [ ] `MemoEdit` mi `RichEditControl` mu **onaylandi** (§0 kontrol notu)
- [ ] `cli-spec VC-CLI-015/016` ile uyumlu (§2.1-2.3)
- [ ] `terminal-spec §2.1-2.4` ile uyumlu (bolge + akis + timeout)
- [ ] `ui-design/README.md` 8. satir eklendi (bu dosya)
- [ ] GATE-003 checklist 13/13 — **onay = kod izni**

**Onay bekleniyorsa:** `GATE-OPEN` yazilir, `src/**`'ye tek satir `.cs` YAZILMAZ (G12).

---

**Olculen kanit (2026-10-02, revize):** bu dosya diskte `09-terminal.md` adiyla yasar (G10 celiski notu: govde `08-terminal` referanslari -> `09-terminal`; README §0 satir 9 = 08-terminal satiri bu dosyayi isaret eder). Toplam `ui-design/` = 11 wireframe + README. `check-links` = 0 bozuk link (toplam 527 MD, 5996+ iyi).
