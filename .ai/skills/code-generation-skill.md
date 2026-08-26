---
title: "Versa Coder — Code Generation Skill"
type: skill
category: code-generation
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Code Generation Skill

---

## 1. Amaç

Kod üretimi görevleri için **özel skill**.

---

## 2. Kullanım Senaryoları

| Senaryo | Komut |
|---------|-------|
| Yeni entity oluştur | `/skill code-gen entity [isim]` |
| Yeni handler oluştur | `/skill code-gen handler [command]` |
| Yeni repository oluştur | `/skill code-gen repo [entity]` |
| Yeni test oluştur | `/skill code-gen test [sınıf]` |

---

## 3. Kod Üretim Kuralları

| # | Kural |
|---|-------|
| 1 | Template uyumluluğu zorunlu |
| 2 | Naming convention'a uy |
| 3 | Layer bağımlılıklarına dikkat |
| 4 | Validation ekle |
| 5 | XML doc comment ekle |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
