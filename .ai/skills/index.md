---
title: "Versa Coder — Skills Index"
type: skill-index
date: 2026-08-25
version: 1.0.0
---

# Versa Coder — Skills Index

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]]

---

## 1. Skill Tanımları

| # | Skill | Kod Adı | Amaç | Kullanım |
|---|-------|---------|------|----------|
| 1 | **Code Generation** | `code-gen` | Kod üretimi ve düzenleme | Yeni dosya, method, class oluşturma |
| 2 | **Architecture Planning** | `arch-plan` | Mimari planlama ve kararlar | ADR oluşturma, katman tasarımı |
| 3 | **Code Analysis** | `code-analyze` | Kod analizi ve optimizasyon | Code smell, complexity, quality |
| 4 | **Testing** | `testing` | Test yazımı ve çalıştırma | Unit, integration, coverage |
| 5 | **Documentation** | `doc-gen` | Dokümantasyon oluşturma | README, API doc, changelog |
| 6 | **Diagram Teaching** | `diagram-teach` | Diyagramları AI'a öğretme | Mermaid, PlantUML → context |

---

## 2. Skill Detayları

### 2.1 Code Generation Skill (`code-gen`)

**Trigger Keywords:** kod yaz, class oluştur, method ekle, dosya oluştur, refactoring yap

**Workflow:**
1. Vault oku (CLAUDE.md, AGENTS.md, WORKFLOW.md)
2. Template seç (entity, repository, service, vb.)
3. Mevcut kodu analiz et
4. Kodu üret
5. Uyumluluğu doğrula
6. Logla

**Tools:** read_file, write_file, edit_file, glob, grep

**Guardrails:** #1 (Zero Code Before Plan), #4 (Template Mandatory), #6 (In-Place Refactoring)

---

### 2.2 Architecture Planning Skill (`arch-plan`)

**Trigger Keywords:** plan oluştur, mimari karar, ADR yaz, katman tasarla

**Workflow:**
1. Proje analizini yükle
2. Mevcut mimariyi kontrol et
3. Karar seçeneklerini değerlendir
4. ADR oluştur
5. Onay için sun
6. Uygulamaya başla

**Tools:** read_file, write_file, grep, glob

**Guardrails:** #8 (Human Approval Gate), #5 (Single Source of Truth)

---

### 2.3 Code Analysis Skill (`code-analyze`)

**Trigger Keywords:** kod analiz et, code smell bul, complexity hesapla, kalite raporu

**Workflow:**
1. Hedef dosyaları seç
2. AST analizi yap
3. Code smell'leri tespit et
4. Complexity hesapla
5. Rapor oluştur
6. Öneriler sun

**Tools:** read_file, grep, glob, analyze_code

**Guardrails:** #3 (Zero Hallucination)

---

### 2.4 Testing Skill (`testing`)

**Trigger Keywords:** test yaz, test çalıştır, coverage hesapla

**Workflow:**
1. Hedef kodu analiz et
2. Test senaryolarını belirle
3. Mock'ları oluştur
4. Test kodunu yaz
5. Test'leri çalıştır
6. Coverage raporunu oluştur

**Tools:** read_file, write_file, bash, analyze_code

**Guardrails:** #4 (Template Mandatory)

---

### 2.5 Documentation Skill (`doc-gen`)

**Trigger Keywords:** doc oluştur, README yaz, changelog güncelle, API dokümantasyonu

**Workflow:**
1. Kaynak kodu analiz et
2. Public API'leri çıkar
3. Doc template'ini seç
4. Dokümantasyonu oluştur
5. Format'ı doğrula
6. Kaydet

**Tools:** read_file, write_file, markdown_parser

**Guardrails:** #4 (Template Mandatory)

---

### 2.6 Diagram Teaching Skill (`diagram-teach`)

**Trigger Keywords:** diyagram öğret, diagram oku, Mermaid'den kod üret

**Workflow:**
1. Diyagramı oku (Mermaid/PlantUML)
2. AI'a öğret (DiagramAITeacher)
3. Context'e kaydet
4. AI diyagrama göre kod yazar
5. Doğruluk kontrolü yap

**Tools:** read_file, write_file, diagram_parser, ai_query

**Guardrails:** #11 (Diagram Teaching)

---

## 3. Skill Seçim Algoritması

```
Kullanıcı Promptu
    → [1. Keyword Analizi] → Keyword çıkar
    → [2. Skill Eşleme] → En uygun skill'i seç
    → [3. Guardrail Kontrolü] → İzinli mi?
    → [4. Tool Seçimi] → Hangi araçlar gerekli?
    → [5. Execution] → Skill'ı çalıştır
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
