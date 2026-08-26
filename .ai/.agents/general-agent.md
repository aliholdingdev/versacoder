---
title: "Versa Coder — General Agent Profile"
type: agent
agent: general
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — General Agent Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `general` |
| Rol | Genel amaçlı görevler |
| Katman | Tümü |
| Mod | subagent |
| Model | gpt-4o |

---

## 2. Yetkiler

| Tool | İzin |
|------|------|
| read | ✅ Allow |
| write | ✅ Allow |
| edit | ✅ Allow |
| glob | ✅ Allow |
| grep | ✅ Allow |
| bash | ✅ Allow |
| todowrite | ❌ Deny |

---

## 3. Kullanım Senaryoları

| Senaryo | Açıklama |
|---------|----------|
| Karmaşık arama | Çoklu kaynakta araştırma |
| Paralel görev | Eşzamanlı iş birimleri |
| Karşılaştırma | Fazla dosya karşılaştırma |
| Doğrulama | Çapraz referans kontrolü |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
