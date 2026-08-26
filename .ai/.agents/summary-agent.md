---
title: "Versa Coder — Summary Agent Profile"
type: agent
agent: summary
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Summary Agent Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `summary` |
| Rol | Özetleme, dokümantasyon |
| Katman | L22 |
| Teknoloji | Markdig |
| Mod | hidden |
| Model | gpt-4o-mini |

---

## 2. Yetkiler

| Tool | İzin |
|------|------|
| read | ✅ Allow |
| write | ❌ Deny |
| edit | ❌ Deny |
| bash | ❌ Deny |

---

## 3. Keyword'ler

```
doc, özet, dokümantasyon, markdown, açıkla, rapor, analiz raporu
```

---

## 4. Çıktı Formatı

```markdown
## Özet
### Ana Noktalar
### Detaylar
### Öneriler
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
