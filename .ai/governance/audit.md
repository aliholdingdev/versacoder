---
title: "audit — Onay Kayıtları (append-only)"
type: audit
category: "governance"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her onaylı .ai yazımı için tek satır: kim/tarih/ne/etki/onay ID (D12)"
tags: [audit, approval, d12, append-only]
source: "D12 + D18 kararları (2026-10-07 onaylı plan)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [[governance/VAULT-GATE|VAULT-GATE]] · [[ops/log|log]] · [[governance/WORKFLOW|WORKFLOW]]

# audit — Onay Kayıtları

> **Tek cümlelik sorumluluk:** Onaylı her `.ai` değişikliğinin kim/tarih/ne/etki
> kaydını **append-only** tutar. Satır silinmez; düzeltme yeni satırdır (log.md
> ile aynı prensip). Onaysız yazım = VAULT-GATE RED.

## §1 Kayıt Şeması (D12/D18)

```text
| TARIH | KADEME | DOSYA(I) | ISLEM | ETKI (geri alinir mi) | ONAY ID | SONUC |
```

- **Kademe (D18):** düşük (paket) · orta (dosya) · yüksek (etki + tek tek).
- **Onay ID:** `A-YYYYMMDD-NN` — her onayın benzersiz numarası.
- **Etki:** geri alınabilir (git) / geri alınamaz (açıklamalı).

## §2 Kayıtlar

| TARIH | KADEME | DOSYA(I) | ISLEM | ETKI | ONAY ID | SONUC |
|---|---|---|---|---|---|---|
| 2026-10-07 | yüksek | (plan) | `.ai` Enterprise mimarisi onayı — 56 karar D1..D56 + 6 faz planı | geri alınabilir (git tag vault-pre-restructure) | A-20261007-01 | ONAYLANDI (ExitPlanMode) |
| 2026-10-07 | orta | `.ai.zip` · `.opencode/**` | D47 artık temizliği (önce arşivlendi) | geri alınabilir (b7e56a7) | A-20261007-02 | ONAYLANDI (D47) |
| 2026-10-07 | paket | `.decisions/accepted/**` | Faz 1 ADR-001..030+032 üretimi | geri alınabilir (199d0cd, ab7b801) | A-20261007-03 | ONAYLANDI (plan Faz 1) |
| 2026-10-07 | paket | `.rules/**` · `architecture/**` | Faz 2 zorunlu 2 dizin üretimi | geri alınabilir (2a6879a, 30cdfb4) | A-20261007-04 | ONAYLANDI (plan Faz 2) |
| 2026-10-07 | orta | 19 dosya taşındı + link rewrite | Faz 3 katman dağıtımı (D38/D49) | geri alınabilir (tag + commit) | A-20261007-05 | ONAYLANDI (plan Faz 3) |

> Yeni satırlar **altına** eklenir; mevcut satırlar değiştirilmez.

## §3 İlgili

- [[ops/log|ops/log]] — ayrı append-only iş günlüğü
- [[governance/VAULT-GATE|VAULT-GATE]]
