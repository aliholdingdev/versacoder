---
title: "Versa Coder — Plan Agent Profile"
type: agent
agent: plan
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Plan Agent Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]] · [[brain.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `plan` |
| Rol | Mimari planlama, task dağıtımı |
| Katman | L2 |
| Teknoloji | MediatR, CQRS |
| Mod | primary |
| Model | gpt-4o |

---

## 2. Yetkiler

| Tool | İzin |
|------|------|
| read | ✅ Allow |
| write | ❌ Deny (sadece plan dosyaları) |
| edit | ❌ Deny |
| glob | ✅ Allow |
| grep | ✅ Allow |
| bash | ❌ Deny |
| git | ✅ Allow (status, diff) |

---

## 3. Keyword'ler

```
plan, mimari, task, phase, milestone, tasarım, yapı, bağımlılık, modül, katman
```

---

## 4. Çıktı Formatı

```markdown
## Mimari Plan
### 1. Gereksinimler
### 2. Modül Yapısı
### 3. Bağımlılıklar
### 4. Uygulama Sırası
### 5. Riskler
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
