---
title: "engine — Bilgi Kümesi İndeksi"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "Agentic orkestrasyon: router, SSE, checkpoint, tool-schema, diff onay — 8 dosyanin tek satir ozeti ve tam baglanti listesi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Engine — Bilgi Kümesi İndeksi

**Zorunlu Bağlantılar:** [[knowledge/index|knowledge index]] · [[index|vault index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Küme + Hub:** [[knowledge/engine/index|engine index]]

> **Tek cümlelik sorumluluk:** Bu klasördeki **8 bilgi dosyasının** hangisini
> ne zaman açacağını tek tabloda söyler; cevabın kendisi o dosyadadır.

## §0 Özet (Answer First)

- **8 dosya** · 11 alanlı frontmatter · 3–8 KB (ölçüm 2026-10-07)
- **Kullanım:** konuyu tablada bul → dosya adına tıkla → § bölümlerine in.
- **Küme içi tam bağlantı:** her dosya, aşağıdaki tabloda listelenen diğer 7 kardeşine linklidir.

## §1 Dosya Listesi (8)

| # | Dosya | Tek satır (frontmatter scope) |
|---|---|---|
| 1 | [[knowledge/engine/agent-streaming-sse|agent-streaming-sse.md]] | Saglayici SSE akisi -> l5 IAgentStreamSink -> 05-ai-chat ekrani: 5 adimli kullanici akisi, … |
| 2 | [[knowledge/engine/diff-approval-gate|diff-approval-gate.md]] | Diff onay blogu: l2 durum makinesi, 05 -> onay -> 06 -> 04 akisi, red edilirse dosya etkisi … |
| 3 | [[knowledge/engine/free-model-router|free-model-router.md]] | Ucretsiz model router: saglayici sirasi (OpenRouter /free -> Ollama -> OpenAI-uyumlu uc), … |
| 4 | [[knowledge/engine/parallel-batch-limit|parallel-batch-limit.md]] | Paralel ajan siniri (<= 3), blok/sira karar matrisi, ezme (write conflict) cozumu ve … |
| 5 | [[knowledge/engine/protocol-tool-schema|protocol-tool-schema.md]] | l5 Protocol arac sozlesmesi: ITool/ToolCall/ToolResult, JSON sema + tool_schema_version, … |
| 6 | [[knowledge/engine/session-checkpoint-persistence|session-checkpoint-persistence.md]] | Checkpoint (gate karsilastirmasi), teslim kutusu, hibrit bellek (oturum ici + vault) ve … |
| 7 | [[knowledge/engine/terminal-cli-surface|terminal-cli-surface.md]] | VersaCoder.Cli konsol yuzeyi: l0..l6 ortek cekirdek (ADR-010), ekran karsiliklari, … |
| 8 | [[knowledge/engine/wizard-approval-flow|wizard-approval-flow.md]] | Wizard: niyet -> 5 adim -> kademeli onay; onay verilmeden tool cagrisi ve dosya yazimi yok … |

> **Hub:** [[knowledge/index|knowledge index]] diğer 4 kümeye (csharp · data · engine · winforms · devexpress) açılır.
