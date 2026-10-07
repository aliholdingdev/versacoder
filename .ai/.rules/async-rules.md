---
title: "Async/Await Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "async akış + CancellationToken; iptal ADR-009 sandbox ile uyumlu"
tags: [['rules', 'async', 'cancellation', 'threads']]
source: "Birleştirme: brain.md ADR-009 (iptal token) + ADR-007 (oturum) bağlamı; vault'ta ayrı async kural metni YOK"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Async/Await Kuralları

> **DOĞRULAMA GEREKLİ (kısmi):** Vault'ta ayrı bir "async-rules" metni kayıtlı
> değildi; bu dosya ADR-009 (iptal token) + ADR-007 (oturum) + genel C#
> pratiklerinden türetildi (G9). Kesin vault kaynağı bulunursa güncellenir.

## Kurallar

1. **UI thread'i bloklanmaz** (`async void` yalnız event handler).
2. Her uzun işlemde **`CancellationToken`** taşınır (ADR-009 iptal token ile
   aynı mekanizma); iptal = nazik `OperationCanceledException`, Error değil
   Info/Debug seviyesinde log.
3. `ConfigureAwait(false)` kütüphane kodunda; UI tarafında senkronizasyon
   context korunur.
4. `Task.Run` yalnız CPU-bound işlerde; IO-bound işlerde doğrudan `await`.
5. Deadlock riski: `.Result` / `.Wait()` **yasak**; her yerde `await`.

## İhlalde

RED — kod incelemesinde; iptal edilemeyen uzun işlem G11 ihlali sayılır.

## İlgili

- [ADR-009](../.decisions/accepted/adr-009-tool-execution-sandbox.md)
- [ADR-007](../.decisions/accepted/adr-007-agent-hierarchy-session.md)
