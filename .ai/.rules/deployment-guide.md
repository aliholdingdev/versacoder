---
title: "Dağıtım Kılavuzu"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Desktop + CLI dağıtımı; packaging spec'e köprü"
tags: [['rules', 'deployment', 'packaging', 'release']]
source: "Birleştirme: spec/deployment-spec.md + spec/packaging-spec.md + spec/release-spec.md (requirement SSOT D33)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Dağıtım Kılavuzu

> Bu dosya **köprüdür**: dağıtım gereksinimlerinin SSOT'u `spec/` altındaki
> spec dosyalarıdır (D33); bu kural dosyası vault kuralını taşır.

## Kurallar

1. **İki hedef:** Desktop (`VersaCoder.Desktop`, WinForms/DevExpress) + CLI
   (`VersaCoder.Cli`, headless). İkisi de aynı çekirdeği paylaşır (ADR-010).
2. **Çift TFM dağıtımı:** her iki TFM çıktısı da paketlenir (ADR-001);
   tek bacaklı paket RED.
3. **Sürüm bilgisi** tek noktadan gelir (CPM/AssemblyInfo senkron — ADR-014
   ruhu); elle sürüm yazımı yasak (G9/G5).
4. **Secret paketlenmez** (ADR-018); ilk çalıştırmada DPAPI deposu kurulur.
5. **CLI headless çıktısı ASCII** kalır (D22 istisnası); TUI çıktısı Unicode
   olabilir.

## Spec köprüleri

- [deployment-spec](../spec/deployment-spec.md) — hedef ortam/kurulum
- [packaging-spec](../spec/packaging-spec.md) — paket/kilit sürümler
- [release-spec](../spec/release-spec.md) — sürüm/not akışı

## İlgili

- [ADR-010](../.decisions/accepted/adr-010-cli-desktop-shared-core.md)
- [configuration-rules](configuration-rules.md)
