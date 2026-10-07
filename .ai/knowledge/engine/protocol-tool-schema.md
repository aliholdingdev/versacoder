---
title: "protocol-tool-schema - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "l5 Protocol arac sozlesmesi: ITool/ToolCall/ToolResult, JSON sema + tool_schema_version, hata degerdir, iki bacak kontrakt testi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# l5 Protocol - Arac Sozlemesi ve Sema

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../definitions/vision.md) · [ADR-006](../../.decisions/accepted/adr-006-opencode-tool-contract.md) · [ADR-028](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md)
**Küme + Hub:** [engine index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (engine · 1/8):** [agent-streaming-sse](agent-streaming-sse.md) · [diff-approval-gate](diff-approval-gate.md) · [free-model-router](free-model-router.md) · [parallel-batch-limit](parallel-batch-limit.md) · [session-checkpoint-persistence](session-checkpoint-persistence.md) · [terminal-cli-surface](terminal-cli-surface.md) · [wizard-approval-flow](wizard-approval-flow.md)

> **Tek cumlelik sorumluluk:** *Bir arac cagrisi hangi sozlesmeden gecer, hata nerede deger olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Arac sozlemesi **tek yerdedir**: `VersaCoder.Protocol` (l5) altinda `ITool` · `ToolCall` · `ToolResult` · `ToolError`. Yedi arac (`read` `write` `edit` `bash` `grep` `glob` `list`) yalniz **ad ve sema** tarafinda ayrisir; cagri ve sonuc ortaktir. Arac **hata firlatmaz** — basarisizlik `ToolResult.Success = false` + `ToolError` olarak **degerdir**, boylece ajan dongusu bir arac yuzunden cokmez. Sema JSON'dur, `tool_schema_version` ile surumlenir; sema uyusmazliginda arac **cagrilmaz**, donguye `ToolError` doner.

```text
+--------------------------------------------------------------+
| l5 VersaCoder.Protocol   <- ITool / ToolCall / ToolResult     |
|     |                                          |              |
|     v                                          v              |
| l4 tool-runner (calistirir)          hata = DEGER (Success=false)
|     |                                          |
|     v                                          v |
| l0 port: IHttpGateway, IAgentStreamSink   dongu COKMEZ         |
+--------------------------------------------------------------+
```

---

## §1 Sozlesme tablosu

| # | Karar | Deger | Dayanak |
|---|---|---|---|
| D1 | Sozlesme evi | `VersaCoder.Protocol` (l5); implementasyon `l4 tool-runner` | ADR-006 D1 |
| D2 | Arac kumesi v1 | `read` `write` `edit` `bash` `grep` `glob` `list` (7 adet) | ADR-006 D2 |
| D3 | Cagri imzasi | `Task<ToolResult> ExecuteAsync(ToolCall, CancellationToken)` | ADR-006 D3 |
| D4 | Sonuc bicimi | `ToolResult { Success, Stdout, Stderr, ExitCode, DurationMs, Error }` | ADR-006 D4 |
| D5 | Sema | JSON Schema + `tool_schema_version: 1` | ADR-006 D5 |
| D6 | Uyumluluk | Kontrakt testleri **her iki bacakta** ayni beklenti | ADR-001 / ADR-028 |

---

## §2 Neden hata degerdir

| Durum | Davranis | Sonuc |
|---|---|---|
| Arac basarisiz | `Success = false` + `ToolError` | dongu devam eder |
| Sema uyusmuyor | arac **cagrilmaz**, `ToolError` doner | girdi siniri korunur |
| Iptal / timeout | `CancellationToken` + arac sonucu | ajan arizasi sayilmaz |
| Ag/arac saglayici hatasi | deger olarak tasinir | 3-STRIKE sayaci isletmez (AGENTS §4.3) |

Arac girdisi **LLM ciktisidir**, yani dogrulanmamis sinirdir; bu yuzden hata firlatmak donguyu kirar, deger olarak tasimak arac arizasi ile ajan arizasini ayirir.

---

## §3 Katman yonu (ADR-028)

| Katman | Gorev |
|---|---|
| `l5 Protocol` | N33-N37: mesaj semasi, tool sozlesmesi, SSE streaming |
| `l4 Infrastructure` | `tool-runner` uygulamasi, timeout/izin calisma zamani (ADR-009) |
| `l0 Abstractions` | `IHttpGateway`, `IAgentStreamSink`, `IPermissionPrompt`, `IWorkbenchContract` portlari |
| `l2 Application` | diff uretimi ve onay durum makinesi |

Yon kurali: `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l2` ile `l4` arasinda **dogrudan ok YOK**.

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | 7 arac `ITool` uyguluyor | kod incelemesi |
| 2 | Arac kaynakli hata testi: `Success=false` -> dongu devam | `dotnet test` |
| 3 | Kontrakt testleri iki bacakta yesil | test ciktisi |
| 4 | G8 onayi alinmadan JSON Schema paketi eklenmemis | paket listesi |
| 5 | `tool_schema_version` alani semada var | sema dosyasi |

> **DOĞRULAMA GEREKLI:** JSON Schema dogrulama kutuphanesinin secimi ve iki bacakta restore edilebilirligi G8 kapsamindadir; onay alinana kadar sema **yalniz belge seviyesindedir**, paket referansi eklenmez.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `.decisions\accepted\ADR-006-opencode-tool-contract.md` | 6 karar tablosu, hata-deger ilkesi, kabul kriterleri |
| `.decisions\accepted\ADR-028-layered-baseline-9-projects.md` | l5 N33-N37, l0 portlari, yon kurali |
| `VISION.md` §2.7 | tool satirinin ekranda gorunmesi (gizli islem yok) |
| `AGENTS.md` §4.3 | dissal kesinti strike sayilmaz |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; sema kutuphanesi secimi bu etiketle kalir.
