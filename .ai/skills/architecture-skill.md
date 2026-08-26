---
title: "Versa Coder — Architecture Skill"
type: skill
category: architecture
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Architecture Skill

---

## 1. Amaç

Mimari planlama ve tasarım görevleri için **özel skill**.

---

## 2. Kullanım Senaryoları

| Senaryo | Komut |
|---------|-------|
| Yeni modül tasarla | `/skill architecture modul-tasarla` |
| Mimari review | `/skill architecture review` |
| Layer violation kontrol | `/skill architecture layer-check` |
| Bağımlılık analizi | `/skill architecture dependency` |

---

## 3. Mimari Kontrol Listesi

| # | Kontrol |
|---|---------|
| 1 | Layer bağımlılık kuralları uygun mu? |
| 2 | SOLID prensiplerine uygun mu? |
| 3 | Domain'de iş mantığı var mı? |
| 4 | Interface Segregation uygulanmış mı? |
| 5 | Dependency Inversion doğru mu? |
| 6 | Template uyumluluğu var mı? |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
