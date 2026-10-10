---
title: "Dağıtım Kılavuzu"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Desktop + CLI dağıtımı; packaging spec'e köprü"
tags: [rules, deployment, packaging, release]
source: "Birleştirme: spec/deployment-spec.md + spec/packaging-spec.md + spec/release-spec.md (requirement SSOT D33)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

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

- [[spec/deployment-spec|deployment-spec]] — hedef ortam/kurulum
- [[spec/packaging-spec|packaging-spec]] — paket/kilit sürümler
- [[spec/release-spec|release-spec]] — sürüm/not akışı

## İlgili

- [[.decisions/accepted/adr-010-cli-desktop-shared-core|ADR-010]]
- [[.rules/configuration-rules|configuration-rules]]
