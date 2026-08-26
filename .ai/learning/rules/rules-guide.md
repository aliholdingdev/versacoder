---
title: "Versa Coder — Learning Rules Guide"
type: learning
category: rules
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Learning Rules Guide

---

## 1. Amaç

AI'ın **öğrenme sürecini yöneten kurallar** tanımlar.

---

## 2. Öğrenme Kuralları

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Her düzeltme kaydedilmeli | Aynı hata tekrarlanmaz |
| 2 | Pattern'ler güncellenmeli | Yeni kalıplar öğrenilmeli |
| 3 | Güven skoru güncellenmeli | Doğruluk arttıkça güven artsın |
| 4 | Eski bilgiler temizlenmeli | 90+ gün güncellenmeyen bilgi |
| 5 | Kaynak gösterilmeli | Her bilginin kaynağı olmalı |

---

## 3. Güven Skoru Hesaplama

```
confidence = (success_count / total_count) * recency_factor
```

| Faktör | Değer |
|--------|-------|
| Başarı oranı | 0.0 - 1.0 |
| Yakınlık | Son 30 gün: 1.0, 60 gün: 0.8, 90 gün: 0.5 |
| Minimum güven | 0.3 (eşik altı silinir) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
