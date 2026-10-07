---
title: "Önbellek Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Minyal: önbellek vault'ta yeni; ilke = geçici veri ≠ kalıcı truth"
tags: [['rules', 'caching', 'memory']]
source: "Türetme: D9 (memory ≠ knowledge ayrımı) + genel önbellek ilkeleri; vault kaynağı sınırlı (G9)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../CLAUDE.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../brain.md) · [WORKFLOW](../governance/WORKFLOW.md) · [VAULT-GATE](../governance/VAULT-GATE.md)

# Önbellek Kuralları

> **DOĞRULAMA GEREKLİ (minyal):** Vault'ta ayrı "caching-rules" metni yoktu;
> bu dosya D9 (memory ≠ knowledge) + genel ilkelerden türetildi (G9).

## Kurallar

1. **Önbellek = derived.** Önbelleğe alınan her şey `authority: derived` sayılır;
   kaynağı (SSOT) her zaman önbelleğin üstündedir (D5).
2. **TTL zorunlu**; süresiz önbellek yasak (stale veri = yanlış karar).
3. Önbellek bozulursa kaynaktan yeniden kurulur; önbellek **asla** tek doğruluk
   kaynağı yapılmaz.
4. Vault tarafında: `.ai` içindeki her "özet/derived" dosya (brain, MEMORY)
   aynı kurala tabidir — stale taraması (D10) onları işaretler.

## İlgili

- [rules-index](index.md) · [ADR-032](../.decisions/accepted/ADR-032-ai-control-plane.md) §2.10
