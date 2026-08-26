---
title: "Versa Coder — Diyagram Bağlamı"
type: context
category: sources
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Diyagram Bağlamı

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/00-overview/architecture-master]]

---

## 1. Diyagram Türleri

| Tür | Format | Kullanım |
|-----|--------|----------|
| Mimari | Mermaid, PlantUML | Katman diyagramları |
| Akış | Mermaid | Workflow diyagramları |
| Sıra | Mermaid | Mesaj akışı diyagramları |
| Veri | Mermaid | Şema diyagramları |

---

## 2. Diyagram Öğretme Protokolü

```
Diyagram Çiz → AI'a Öğret → Context'e Kaydet → AI Anlasın → Kod Üret
```

| Adım | Araç |
|------|------|
| 1. Çiz | .diagram/ dizininde Mermaid/PlantUML |
| 2. Öğret | DiagramAITeacher → context/sources/ |
| 3. Kaydet | .ai/context/sources/diagram-context.md |
| 4. Oku | AgentRunner context assembly |
| 5. Üret | AI diyagrama göre kod yazar |

---

## 3. Mevcut Diyagramlar

| Diyagram | Konum | Tanım |
|----------|-------|-------|
| Architecture Overview | .diagram/architecture/ | Ana mimari yapı |
| Agent Flow | .diagram/flow/ | Agent çalışma akışı |
| Provider Flow | .diagram/sequence/ | LLM provider iletişimi |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
