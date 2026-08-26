---
title: "Versa Coder — Template Kataloğu"
type: template
category: index
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Template Kataloğu

**Zorunlu Bağlantılar:** [[CLAUDE.md]] §16 · [[brain.md]]

---

## 1. Amaç

Versa Coder'daki tüm **şablonların listesi ve kullanım kılavuzu**. Guardrail #6'ya göre yeni dosya için template zorunludur.

---

## 2. Template Kategorileri

### 2.1 Domain Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Entity | `entity-template.cs` | Yeni entity oluştur |
| ValueObject | `valueobject-template.cs` | Yeni değer objesi |
| Enum | `enum-template.cs` | Yeni enum |
| Interface | `interface-template.cs` | Yeni arayüz |

### 2.2 Application Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Command | `command-template.cs` | Yeni CQRS command |
| Query | `query-template.cs` | Yeni CQRS query |
| Handler | `handler-template.cs` | Yeni MediatR handler |
| DTO | `dto-template.cs` | Yeni DTO |

### 2.3 Infrastructure Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Repository | `repository-template.cs` | Yeni repository |
| Configuration | `config-template.cs` | Yeni EF config |
| Provider | `provider-template.cs` | Yeni LLM provider |

### 2.4 Test Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| UnitTest | `unittest-template.cs` | Yeni unit test |
| IntegrationTest | `integrationtest-template.cs` | Yeni integration test |

### 2.5 UI Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Form | `form-template.cs` | Yeni DevExpress form |
| ViewModel | `viewmodel-template.cs` | Yeni MVVM ViewModel |

---

## 3. Kullanım Kuralları

| # | Kural |
|---|-------|
| 1 | Yeni dosya için template zorunlu (Guardrail #6) |
| 2 | Template'ler `.ai/.templates/` dizininde saklanır |
| 3 | Template'ler güncellenebilir |
| 4 | Yeni template eklenebilir |
| 5 | Template seçimi `index.md`'den yapılır |

---

## 4. Template Kullanım Akışı

```
İhtiyaç → index.md'den template seç → Kopyala → Personalizasyon → Kaydet
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
