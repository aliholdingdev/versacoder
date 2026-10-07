---
title: "Performans Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Timeout'lar (ADR-009), ölçüm tek kaynaktan (ADR-027), bütçe disiplini"
tags: [['rules', 'performance', 'budget', 'timeout']]
source: "Birleştirme: brain.md ADR-009/027 + spec/performance-spec.md bağlamı"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../CLAUDE.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../brain.md) · [WORKFLOW](../governance/WORKFLOW.md) · [VAULT-GATE](../governance/VAULT-GATE.md)

# Performans Kuralları

## Timeout

- Araç çağrısı **60 sn**; `bash` **300 sn** (ADR-009). Uzayan iş = iptal token.
- SQL `CommandTimeout` **açık set** (varsayılan 15 sn); karanlık timeout yasak.

## Ölçüm

- Sayısal iddia **elle yazılmaz**; tek kanıt `count-components.ps1` /
  benchmark çıktısı (ADR-027, G5).
- "Hızlı" iddiası = süre ölçümü olmadan yazılamaz; `REPOSITORY_VERIFIED`
  etiketi + komut çıktısı gerekir.

## Bütçe

- Performans bütçeleri `spec/performance-spec.md` kapsamındadır (o dosya
  requirement SSOT'udur — D33); bu kural dosyası yalnız **davranış** kuralını
  koyar: bütçeyi aşan değişiklik GATE-005'te raporlanır.

## İhlalde

RED — ölçüm olmadan "bitti" denemez (G5).

## İlgili

- [ADR-009](../.decisions/accepted/ADR-009-tool-execution-sandbox.md)
- [ADR-027](../.decisions/accepted/ADR-027-count-components-script.md)
