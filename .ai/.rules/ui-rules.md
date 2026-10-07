---
title: "UI/UX Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Ribbon, ekran üçlüsü, çizim politikası (D22 Unicode kazandı), erişilebilirlik köprüsü"
tags: [['rules', 'ui', 'ux', 'ribbon', 'd22', 'd46']]
source: "D46 kararı + ADR-015/025 + D22 (Unicode politikası revizyonu → ADR-032 §2.16)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# UI/UX Kuralları

## Çizim politikası (D22 — ADR-029 revizyonu)

**Unicode kazanır.** Vault içi diyagram/ekran çizimlerinde U+2500..U+257F
**izinlidir ve tercih edilir**; GATE-004a'nın ASCII kriteri revize edilmiştir
(ADR-032 §2.16). İstisna: **CLI/headless çıktısı ASCII kalır** (`+ - |`).
Mermaid diyagramı varsa yanında düz metin karşılığı zorunludur (ADR-020 D3).

## Ekran üçlüsü

- Her ekran = `.Form.cs + .Form.Designer.cs + .resx` (ADR-015).
- Designer elle düzenlenmez; son söz `ui-designer` + GATE-003 (ADR-025).
- Wireframe dosyaları `ui-design/` altındadır; spec eşleşmesi zorunlu
  (`01-start-page ↔ start-page-spec`, D37).

## Ribbon düzeni

- `RibbonControlStyle.Office2010` tek stil (G3); grup/sekme düzeni
  `ui-design/component-inventory.md` ile eşleşir.

## Erişilebilirlik

- Tam kural metni vault'ta **sınırlı** → [accessibility-rules](accessibility-rules.md)
  dosyasındaki `DOĞRULAMA GEREKLİ` bölümünü doldurmak şartıyla uygulanır (G9).

## İhlalde

RED — tasarım tekrarlanır; onaysız kod silinir (GATE-003).

## Kanıt

- `ribbon-style-check`, `winforms-designer-check`, `gate-003-ui-onay` yeşil

## İlgili

- [ADR-025](../.decisions/accepted/adr-025-gate003-ui-hard-stop.md)
- [devexpress-rules](devexpress-rules.md) · [accessibility-rules](accessibility-rules.md)
