---
title: "Versa Coder — Kod Standartları"
type: rules
category: coding
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Kod Standartları

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]]

---

## 1. C# Naming Convention

| Öğe | Format | Örnek |
|-----|--------|-------|
| Namespace | `VersaCoder.{Layer}.{Module}` | `VersaCoder.Domain.Entities` |
| Class | PascalCase | `SessionManager` |
| Method | PascalCase | `CreateSession()` |
| Property | PascalCase | `SessionId` |
| Field | _camelCase | `_sessionRepository` |
| Parameter | camelCase | `sessionDto` |
| Local Variable | camelCase | `result` |
| Constant | PascalCase | `MaxRetryCount` |
| Enum | PascalCase | `AgentRole.Build` |
| Interface | I{PascalCase} | `ISessionManager` |

---

## 2. Kod Stili

| Kural | Açıklama |
|-------|----------|
| Nullable | `#nullable enable` |
| Async/Await | Asenkron metodlarda zorunlu |
| Pattern matching | `is`, `switch` expression tercih |
| LINQ | Loop yerine LINQ tercih |
| Null check | `?.` ve `??` kullanımı |
| Encapsulation | `{ get; set; }` — public alan yasak |

---

## 3. Dosya Yapısı

| Kural | Açıklama |
|-------|----------|
| Tek class / dosya | Her dosyada tek class |
| Namespace uyumu | Dosya yolu = namespace |
| Using sırası | System → 3rd party → Project |
| Max satır | 1000 satır (MO onayı ile genişletilebilir) |

---

## 4. Yasak Örüntüleri

| ❌ Yasak | ✅ Doğru |
|----------|----------|
| `public` alan | `{ get; set; }` |
| `var` kullanımı belirsizlikte | Explicit type |
| `eval()` | Guvenli alternatifler |
| Magic number | Sabit constant |
| Nested if > 3 seviye | Early return, guard clause |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
