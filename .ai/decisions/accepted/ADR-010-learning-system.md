---
title: "ADR-010 — Learning System Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-010 — Learning System Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.Learning  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, **4 modüllü** (Patterns, Corrections, Knowledge, Rules) bir öğrenme sistemi kullanacaktır.

## 2. Bağlam

AI ajanlarının kullanıcı düzeltmelerinden ve tercihlerinden öğrenmesi gerekmektedir. Bu, tekrarlayan hataları önlemek ve kaliteyi artırmak için kritiktir.

## 3. Öğrenme Modülleri

| Modül | Amaç | Veri Tipi | Saklama |
|-------|------|-----------|---------|
| **Patterns** | Kod kalıplarını öğren | JSON | .ai/learning/patterns/ |
| **Corrections** | Düzeltmeleri kaydet | JSON | .ai/learning/corrections/ |
| **Knowledge** | Bilgi tabanını genişlet | JSON | .ai/learning/knowledge/ |
| **Rules** | Öğrenilen kurallar | JSON | .ai/learning/rules/ |

## 4. Learning Akışı

```
Kullanıcı Düzeltmesi
    → [1. Tespit] Düzeltmeyi algıla
    → [2. Analiz] Düzeltme türünü belirle
    → [3. Kaydet] Patterns/Corrections/Knowledge/Rules'a kaydet
    → [4. Güncelle] İlgili dosyaları güncelle
    → [5. Doğrula] Öğrenmeyi doğrula
    → [6. Uygula] Gelecekteki görevlerde kullan
```

## 5. Pattern Recognition

```csharp
public class PatternRecognizer
{
    private readonly List<Pattern> _knownPatterns;
    
    public Pattern? RecognizePattern(CodeChange change)
    {
        foreach (var pattern in _knownPatterns)
        {
            if (pattern.Matches(change))
            {
                pattern.Confidence += 0.1f; // Güven artır
                return pattern;
            }
        }
        
        // Yeni pattern olarak kaydet
        var newPattern = Pattern.FromChange(change);
        _knownPatterns.Add(newPattern);
        return newPattern;
    }
}
```

## 6. Correction Tracker

```csharp
public class CorrectionTracker
{
    public async Task TrackCorrectionAsync(
        Correction correction)
    {
        // 1. Düzeltmeyi kaydet
        await SaveCorrectionAsync(correction);
        
        // 2. İlgili kuralı güncelle
        var rule = await FindRelatedRuleAsync(correction);
        if (rule != null)
        {
            rule.Confidence = Math.Min(1.0f, rule.Confidence + 0.05f);
            await UpdateRuleAsync(rule);
        }
        
        // 3. Pattern güncelle
        await UpdatePatternAsync(correction);
    }
}
```

## 7. Rule Engine

```csharp
public class RuleEngine
{
    private readonly List<LearningRule> _rules;
    
    public async Task<List<RuleViolation>> ValidateAsync(
        CodeChange change)
    {
        var violations = new List<RuleViolation>();
        
        foreach (var rule in _rules.Where(r => r.IsEnabled))
        {
            if (rule.IsViolatedBy(change))
            {
                violations.Add(new RuleViolation
                {
                    Rule = rule,
                    Severity = rule.Severity,
                    Message = rule.GetMessage(change)
                });
            }
        }
        
        return violations;
    }
}
```

## 8. Konfigürasyon

```json
{
  "Learning": {
    "Enabled": true,
    "StoragePath": ".ai/learning",
    "MaxPatterns": 1000,
    "MaxCorrections": 5000,
    "MinConfidence": 0.3,
    "AutoApply": false,
    "RequireApproval": true
  }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
