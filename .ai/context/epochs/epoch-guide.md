---
title: "Versa Coder — Epoch Yönetimi"
type: context
category: epochs
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Epoch Yönetimi

---

## 1. Amaç

Context'in **zaman dilimlerini** (epoch) yöneterek, her oturumun hangi bağlamda çalıştığını takip eder.

---

## 2. Epoch Tanımları

| Epoch | Tanım | Tetikleyici |
|-------|-------|-------------|
| E0 | Başlangıç | Uygulama açılışı |
| E1 | Vault yüklendi | Session init |
| E2 | Proje yüklendi | Proje açılışı |
| E3 | Session başladı | Yeni oturum |
| E4 | Agent çalıştı | Görev başlangıcı |
| E5 | Tamamlandı | Görev sonu |

---

## 3. Epoch Değişiklik Kuralları

| Kural | Açıklama |
|-------|----------|
| Geriye dönük epoch | epoch sequence artarak gider |
| Context snapshot | Her epoch başında context snapshot alınır |
| Cleanup | Eski epoch'lar 7 gün sonra temizlenir |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
