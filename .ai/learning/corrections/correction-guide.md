---
title: "Versa Coder — Correction Learning Guide"
type: learning
category: corrections
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Correction Learning Guide

---

## 1. Amaç

AI'ın **yaptığı hatalardan öğrenmesini** ve aynı hataları tekrarlamamasını sağlayan modül.

---

## 2. Correction Türleri

| Tür | Tanım | Örnek |
|-----|-------|-------|
| Syntax Error | Yazım hatası | Yanlış namespace |
| Logic Error | Mantık hatası | Yanlış condition |
| Architecture Error | Mimari ihlal | Layer Violation |
| Style Error | Stil hatası | Yanlış naming |
| Security Error | Güvenlik hatası | SQL injection |

---

## 3. Correction Kayıt Formatı

```markdown
## Correction: [Hata Adı]
- **Tür:** [Syntax/Logic/Architecture/Style/Security]
- **Hata:** [Ne yanlış]
- **Düzeltme:** [Nasıl düzeltildi)
- **Neden:** [Kök neden]
- **Önleme:** [Gelecekte nasıl önlenir]
- **Güven:** [0.0-1.0]
```

---

## 4. Öğrenme Akışı

```
Hata Tespiti → Kök Neden Analizi → Düzeltme Kaydetme → Kuralları Güncelle → Tekrar Engelleme
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
