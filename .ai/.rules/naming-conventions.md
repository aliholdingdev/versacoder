---
title: "Adlandırma Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "C#/vault adlandırma; DOĞRULAMA GEREKLİ bölümü var"
tags: [rules, naming, conventions]
source: "Türetme: genel C# konvansiyonları + vault desenleri; vault'ta ayrı kaynak YOK (G9)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

# Adlandırma Kuralları

> **DOĞRULAMA GEREKLİ (kısmi):** Vault'ta ayrı bir "naming-conventions" metni
> kayıtlı değildi; aşağıdaki kurallar genel C# konvansiyonları + vault'taki
> tutarlı desenlerden türetildi (G9). Kesin vault kaynağı bulunursa güncellenir.

## C# / .NET

- **PascalCase:** sınıf, arayüz (`I` öneki), metot, özellik, olay.
- **camelCase:** yerel değişken, parametre, özel alan (`_` öneki seçilebilir —
  vault'ta `readonly IRowWriter _db` deseni tutarlıdır).
- **Arayüz:** `I` + isim; `IFileReader`, `IRowWriter` (niyet gizleme — brain §2.6).
- **Async metot:** `VerbAsync` (`LoadAsync`, `SaveAsync`).
- **Test metodu:** `Senaryo_beklenen_davranış` (xUnit deseni).

## Vault dosyaları

- Dosya adı: **kebab-case** + `.md` (`coding-standards.md`, `ADR-028-...md`).
- ADR dosya adı: `ADR-NNN-slug.md` — slug **İngilizce**, vault'taki link
  hedefleriyle birebir (Faz 1 kuralı).
- Klasör adı: küçük harf, tire (`.rules/`, `ui-design/`); nokta öneki yalnız
  gizli/enstrümantasyon klasörleri (`.rules/`, `.decisions/`, `.templates/`).

## İhlalde

RED — yeniden adlandırılır; linkler güncellenir (SRP/ADR-021).

## İlgili

- [[.rules/index|rules-index]] · [[.rules/coding-standards|coding-standards]]
