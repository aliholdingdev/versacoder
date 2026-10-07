---
title: "Loglama Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Structured log, 5 seviye, tek şablon (ADR-026)"
tags: [['rules', 'logging', 'observability', 'structured']]
source: "Birleştirme: brain.md §3 kural 2 + §3.7/3 + ADR-026"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Loglama Kuralları (ADR-026)

## Kural

- **Tek structured şablon**; sabit alan adları; iki bacakta aynı isimler.
- **5 seviye:** `Trace / Debug / Info / Warn / Error` — dışında isim yok.
- **Serbest metin log yasak**:

```text
KOTU : logger.LogInformation("dosya acilamadi " + path);
IYI  : logger.LogInformation("file.open_failed {Path} {ErrorCode}", path, "E_ACCESS");
```

- Log olay adı `alan.olay` biçimindedir (`setting.save_ok`).
- Vault günlüğü `.ai/ops/log.md` append-only'dir (D12); uygulama logları
  diske ADR-026 şablonuyla yazılır.

## İhlalde

RED (format) — kod incelemesi; iki bacakta alan adları ayrışması = çözümleme kaybı.

## İlgili

- [ADR-026](../.decisions/accepted/adr-026-structured-log-5-levels.md)
- [error-handling-rules](error-handling-rules.md)
