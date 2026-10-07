---
title: "engine — Bilgi Kümesi İndeksi"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "Agentic orkestrasyon: router, SSE, checkpoint, tool-schema, diff onay — 8 dosyanin tek satir ozeti ve tam baglanti listesi"
---

# Engine — Bilgi Kümesi İndeksi

**Zorunlu Bağlantılar:** [knowledge index](../index.md) · [vault index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Küme + Hub:** [engine index](index.md)

> **Tek cümlelik sorumluluk:** Bu klasördeki **8 bilgi dosyasının** hangisini
> ne zaman açacağını tek tabloda söyler; cevabın kendisi o dosyadadır.

## §0 Özet (Answer First)

- **8 dosya** · 8 alanlı frontmatter · 3–8 KB (ölçüm 2026-10-07)
- **Kullanım:** konuyu tablada bul → dosya adına tıkla → § bölümlerine in.
- **Küme içi tam bağlantı:** her dosya, aşağıdaki tabloda listelenen diğer 7 kardeşine linklidir.

## §1 Dosya Listesi (8)

| # | Dosya | Tek satır (frontmatter scope) |
|---|---|---|
| 1 | [agent-streaming-sse.md](agent-streaming-sse.md) | Saglayici SSE akisi -> l5 IAgentStreamSink -> 05-ai-chat ekrani: 5 adimli kullanici akisi, … |
| 2 | [diff-approval-gate.md](diff-approval-gate.md) | Diff onay blogu: l2 durum makinesi, 05 -> onay -> 06 -> 04 akisi, red edilirse dosya etkisi … |
| 3 | [free-model-router.md](free-model-router.md) | Ucretsiz model router: saglayici sirasi (OpenRouter /free -> Ollama -> OpenAI-uyumlu uc), … |
| 4 | [parallel-batch-limit.md](parallel-batch-limit.md) | Paralel ajan siniri (<= 3), blok/sira karar matrisi, ezme (write conflict) cozumu ve … |
| 5 | [protocol-tool-schema.md](protocol-tool-schema.md) | l5 Protocol arac sozlesmesi: ITool/ToolCall/ToolResult, JSON sema + tool_schema_version, … |
| 6 | [session-checkpoint-persistence.md](session-checkpoint-persistence.md) | Checkpoint (gate karsilastirmasi), teslim kutusu, hibrit bellek (oturum ici + vault) ve … |
| 7 | [terminal-cli-surface.md](terminal-cli-surface.md) | VersaCoder.Cli konsol yuzeyi: l0..l6 ortek cekirdek (ADR-010), ekran karsiliklari, … |
| 8 | [wizard-approval-flow.md](wizard-approval-flow.md) | Wizard: niyet -> 5 adim -> kademeli onay; onay verilmeden tool cagrisi ve dosya yazimi yok … |

> **Hub:** [knowledge index](../index.md) diğer 4 kümeye (csharp · data · engine · winforms · devexpress) açılır.
