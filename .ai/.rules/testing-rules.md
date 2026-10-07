---
title: "Test Kuralları (G6 G16)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Çift TFM, TDD, coverage %90, TreatWarningsAsErrors, NetArchTest"
tags: [['rules', 'testing', 'tdd', 'g6', 'g16', 'coverage']]
source: "Birleştirme: .ai/CLAUDE.md G6 + kök CLAUDE.md G11 + brain.md §3/3 + ADR-019"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Test Kuralları (G6 G16)

## G6 — Çift TFM

Her karar **iki bacakta**: `net48` + `net10.0-windows`. `#if` yalnız platform/API
farkında. Tek bacakta yeşil = **bitmemiştir**. İhlalde: mimari RED.
Kanıt: iki ayrı derleme, 0 hata.

## G16 — TDD + kalite kapıları

- **TDD**: test önce, implementasyon sonra (kırmızı → yeşil → sadeleştir).
- **`TreatWarningsAsErrors=true`**: derleme uyarısı = derleme hatası.
- **%90+ coverage hedefi**; eşik altı = tamamlanmadı
  (`gate-005-test-kapsami`, `coverage-gate-check`).
- Mimari test: **NetArchTest** `LayerRulesTests` — 8×8 matris; elle çizilmiş
  matris kanıt değildir (ADR-028).

## Sonuç taşıma

Testlerde beklenen sonuç `Result<T>` üzerinden assert edilir; exception testi
yalnız beklenmeyen çökme içindir ([error-handling-rules](error-handling-rules.md)).

## İhlalde

HIGH — "bitti" denemez (G5); iş tamamlanmadı sayılır.

## Kanıt

- iki bacakta `dotnet test` = 0 başarısız
- coverage raporu ≥ %90

## İlgili

- [ADR-001](../.decisions/accepted/adr-001-dual-target-net48-net10.md)
- [ADR-019](../.decisions/accepted/adr-019-dual-leg-build-test.md)
