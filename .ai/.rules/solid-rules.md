---
title: "SOLID Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "5 ilke + niyet gizleme; vault'a çakılmış hali (brain §2)"
tags: [['rules', 'solid', 'design']]
source: "Birleştirme: brain.md §2 (SOLID vault'a çakılmış hâli) + ADR-005/006/011/016/019/021/028"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../CLAUDE.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../brain.md) · [WORKFLOW](../governance/WORKFLOW.md) · [VAULT-GATE](../governance/VAULT-GATE.md)

# SOLID Kuralları

| İlke | Kural (vault karşılığı) | Yakalama |
|---|---|---|
| **S** Tek sorumluluk | Tek dosya tek konu (ADR-021); sınıf tek değişiklik nedeni | `reviewer` bölme önerisi |
| **O** Açık/kapalı | Yeni sağlayıcı = yeni adaptör; mevcut zincir değişmez (ADR-011) | `else if` genişlemesi = adaptör iste |
| **L** Liskov | İki TFM aynı sözleşmeyi aynı davranışla sunar (ADR-001/019) | tek bacak sapması RED |
| **I** Arayüz ayrımı | İnce arayüzler; çağırlmayan üye yok (ADR-006) | boş üye = uyarı |
| **D** Bağımlılık tersine | Port l0'da, uygulama l4'te; üst katman `new Store` yapmaz (ADR-005/016/028) | NetArchTest + `new SqlConnection` taraması |
| **I** Niyet gizleme (ek) | "Ne başlattığını" söyler; `And` ile üç eylem yan yana = SRP işareti | `reviewer` raporu |

## Kural cümlesi

Yön = aşağı (L4→L0); authority = yukarı (L0). Ters referans = NetArchTest RED.

## İlgili

- [layer-rules](layer-rules.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)
- [brain §2](../brain.md)
