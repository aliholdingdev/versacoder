---
title: "Versa Coder — Explore Agent Profile"
type: agent
agent: explore
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Explore Agent Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `explore` |
| Rol | Kod analizi, dosya tarama, bilgi toplama |
| Katman | L1-L4 |
| Teknoloji | Roslyn, AST |
| Mod | subagent |
| Model | gpt-4o-mini |

---

## 2. Yetkiler

| Tool | İzin |
|------|------|
| read | ✅ Allow |
| write | ❌ Deny |
| edit | ❌ Deny |
| glob | ✅ Allow |
| grep | ✅ Allow |
| bash | ✅ Allow (read-only) |

---

## 3. Keyword'ler

```
analiz, tarama, grep, glob, dosya bul, oku, incele, ara, keşfet, yapı
```

---

## 4. Çıktı Formatı

```markdown
## Analiz Sonucu
### Bulunan Dosyalar
### Kod Kalıpları
### Bağımlılıklar
### Öneriler
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
