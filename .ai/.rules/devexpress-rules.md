---
title: "DevExpress Kuralları (G3)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Tek sürüm v26.1; ribbon Office2010; tasarımcı sözleşmesi"
tags: [['rules', 'devexpress', 'ui', 'g3']]
source: "Birleştirme: .ai/CLAUDE.md G3 + brain.md §4.2/3 + ADR-004/015"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# DevExpress Kuralları (G3)

## G3 — Tek sürüm + tek stil

- DevExpress **tek sürüm: v26.1** (sürüm ADR'den doğrulanır — G9).
  `24.2`, `23.x`, `25.x` vb. = otomatik **RED**.
- Ribbon stili yalnız **`RibbonControlStyle.Office2010`**;
  `Office2007/Office2010Silver/Office2010Blue` = **RED**.
- Ana pencere `RibbonForm`; `XtraForm` = **RED**.
- Paket satırları **yalnız** `Directory.Packages.props` (CPM, ADR-014).

## Tasarımcı sözleşmesi

- Her ekran `.Form.cs + .Form.Designer.cs + .resx` üçlüsü (ADR-015).
- Designer/resx **yalnız tasarımcıdan**; el ile düzenleme = RED.
- Son söz: `ui-designer` + GATE-003 onayı (ADR-025).

## İhlalde

CRITICAL — RED; sürüm/stil geri alınır.

## Kanıt

- `Directory.Packages.props` tek DevExpress satırı = `26.1`
- `ribbon-style-check`, `devexpress-control-check` yeşil

## İlgili

- [ADR-004](../.decisions/accepted/adr-004-devexpress-universal-v261-office2010.md)
- [ADR-015](../.decisions/accepted/adr-015-winforms-form-triple-designer.md)
- [ui-rules](ui-rules.md)
