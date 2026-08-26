---
title: "Versa Coder — Learning Index"
type: learning-index
date: 2026-08-25
version: 1.0.0
---

# Versa Coder — Learning Index

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]]

---

## 1. Öğrenme Sistemi Tanımı

Versa Coder, kullanıcının düzeltmelerinden ve tercihlerinden öğrenen bir sistem kullanır.

### 1.1 Öğrenme Modülleri

| Modül | Amaç | Konum |
|-------|------|-------|
| **Patterns** | Kod kalıplarını öğren | `.ai/learning/patterns/` |
| **Corrections** | Düzeltmeleri kaydet | `.ai/learning/corrections/` |
| **Knowledge** | Bilgi tabanını genişlet | `.ai/learning/knowledge/` |
| **Rules** | Öğrenilen kurallar | `.ai/learning/rules/` |

---

## 2. Patterns

### 2.1 Kod Kalıpları

| Pattern | Tanım | Kullanım |
|---------|-------|----------|
| **Singleton** | Tek instance | Service'ler |
| **Factory** | Obje oluşturma | Repository'ler |
| **Strategy** | Algoritma seçimi | Provider'lar |
| **Observer** | Event dinleme | Event handler'lar |
| **Decorator** | Davranış ekleme | Middleware'ler |
| **Repository** | Veri erişim | Data access |
| **Unit of Work** | Transaction | DbContext |
| **CQRS** | Komut/sorgu ayrımı | MediatR |
| **Mediator** | Arabulucu | MediatR |
| **Builder** | Adım adım oluşturma | Config'ler |

### 2.2 Mimari Kalıplar

| Pattern | Tanım | Kullanım |
|---------|-------|----------|
| **Clean Architecture** | Katmanlı mimari | Versa Coder |
| **DDD** | Domain-driven design | Domain katmanı |
| **MVVM** | Model-View-ViewModel | UI katmanı |
| **Microservices** | Servis ayrımı | Infrastructure |
| **Event Sourcing** | Event geçmişi | Messaging |
| **CQRS** | Komut/sorgu ayrımı | Application |

---

## 3. Corrections

### 3.1 Düzeltme Türleri

| Tür | Tanım | Örnek |
|-----|-------|-------|
| **Naming** | İsimlendirme düzeltmesi | `GetUser` → `GetUserById` |
| **Architecture** | Mimari düzeltme | Tek dosya → modüler yapı |
| **Performance** | Performans düzeltmesi | N+1 query → JOIN |
| **Security** | Güvenlik düzeltmesi | Hardcoded → IConfiguration |
| **Style** | Stil düzeltmesi | Tabs → Spaces |
| **Logic** | Mantık düzeltmesi | Yanlış hesaplama |

### 3.2 Düzeltme Kaydı

```json
{
  "id": "correction-001",
  "date": "2026-08-25",
  "user": "developer",
  "type": "naming",
  "before": "GetUser",
  "after": "GetUserById",
  "reason": "Method takes an ID parameter, name should reflect this",
  "file": "src/VersaCoder.Application/Services/UserService.cs",
  "line": 45
}
```

---

## 4. Knowledge

### 4.1 Bilgi Kategorileri

| Kategori | Tanım | Kaynak |
|----------|-------|--------|
| **Project** | Proje yapısı | .csproj, .sln |
| **API** | API endpoint'leri | Controller'lar |
| **Database** | Veritabanı yapısı | EF Core DbContext |
| **UI** | UI yapısı | Form'lar, View'lar |
| **Config** | Konfigürasyon | appsettings.json |
| **Dependencies** | Bağımlılıklar | NuGet paketleri |

### 4.2 Bilgi Kaydı

```json
{
  "id": "knowledge-001",
  "category": "project",
  "key": "solution_structure",
  "value": {
    "layers": ["Domain", "Abstractions", "Application", "Infrastructure"],
    "namespaces": ["VersaCoder.*"],
    "test_framework": "xunit"
  },
  "source": "project_analysis",
  "confidence": 0.95
}
```

---

## 5. Rules

### 5.1 Öğrenilen Kurallar

| # | Kural | Kaynak | Güven |
|---|-------|--------|-------|
| 1 | Her entity için repository gerekli | Düzeltme #3 | 0.95 |
| 2 | ViewModel'de async command kullan | Düzeltme #7 | 0.90 |
| 3 | Config value'ları IConfiguration'dan oku | Düzeltme #12 | 0.98 |
| 4 | Test'de mock kullan, real DB kullanma | Düzeltme #15 | 0.92 |
| 5 | Method isimleri verb ile başlamalı | Düzeltme #22 | 0.88 |

### 5.2 Kural Çatışması

```json
{
  "conflict_id": "rule-conflict-001",
  "rules": ["rule-3", "rule-7"],
  "description": "Rule 3 says use IConfiguration, Rule 7 says use direct file read",
  "resolution": "Use IConfiguration for app settings, direct read for vault files",
  "resolved_by": "Master Orchestrator",
  "date": "2026-08-25"
}
```

---

## 6. Learning Akışı

```
Kullanıcı Düzeltmesi
    → [1. Tespit] Düzeltmeyi algıla
    → [2. Analiz] Düzeltme türünü belirle
    → [3. Kaydet] Patterns/Corrections/Knowledge/Rules'a kaydet
    → [4. Güncelle] İlgili dosyaları güncelle
    → [5. Doğrula] Öğrenmeyi doğrula
    → [6. Uygula] Gelecekteki görevlerde kullan
```

---

## 7. Learning Storage

### 7.1 Dosya Yapısı

```
.ai/learning/
├── patterns/
│   ├── code-patterns.json
│   ├── architecture-patterns.json
│   └── ui-patterns.json
├── corrections/
│   ├── correction-log.json
│   └── correction-details/
├── knowledge/
│   ├── project-knowledge.json
│   ├── api-knowledge.json
│   └── database-knowledge.json
└── rules/
    ├── learned-rules.json
    └── rule-conflicts.json
```

### 7.2 JSON Formatı

```json
{
  "version": "1.0.0",
  "lastUpdated": "2026-08-25T05:00:00Z",
  "entries": [
    {
      "id": "pattern-001",
      "type": "code",
      "name": "Repository Pattern",
      "description": "Use repository pattern for data access",
      "example": "...",
      "confidence": 0.95,
      "source": "user_correction",
      "applied_count": 15
    }
  ]
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
