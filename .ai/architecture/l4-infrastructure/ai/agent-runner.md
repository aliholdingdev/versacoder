---
title: "Versa Coder — Agent Runner Mimarisi"
type: architecture
category: ai
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Agent Runner Mimarisi

**Zorunlu Bağlantılar:** [[architecture/l4-infrastructure/ai/provider-router]] · [[architecture/l4-infrastructure/ai/tool-system]] · [[AGENTS.md]]

---

## 1. Amaç

Tüm agent'ların **çalıştırılmasını, context assembly'ini, tool seçimini ve yönetimini** sağlayan merkezi agent motoru.

---

## 2. Agent Runner Akışı

```
User Prompt
    ↓
[1. Agent Seçimi] → AgentSelectorService → Doğru Agent
    ↓
[2. Context Assembly] → Vault + Learning + Session bilgisi
    ↓
[3. Model Seçimi] → ProviderRouter → LLM Provider
    ↓
[4. LLM Çağrısı] → Streaming yanıt
    ↓
[5. Tool Çağrıları] → Tool Registry → Execute → Result
    ↓
[6. Devam/Durdur] → finish_reason kontrolü
    ↓
[7. Sonuç Kaydet] → Session + Message + Event
```

---

## 3. Agent Seçim Algoritması

```csharp
public AgentRole SelectAgent(string userPrompt)
{
    var prompt = userPrompt.ToLowerInvariant();

    // Priority 1: Build Agent
    if (ContainsAny(prompt, BuildKeywords))  // kod, class, method, property, service
        return AgentRole.Build;

    // Priority 2: Plan Agent
    if (ContainsAny(prompt, PlanKeywords))   // plan, mimari, task, phase, milestone
        return AgentRole.Plan;

    // Priority 3: Explore Agent
    if (ContainsAny(prompt, ExploreKeywords)) // analiz, tarama, grep, glob, bul, oku
        return AgentRole.Explore;

    // Priority 4: Summary Agent
    if (ContainsAny(prompt, SummaryKeywords)) // doc, özet, dokümantasyon, markdown
        return AgentRole.Summary;

    // Priority 5: Title Agent
    if (ContainsAny(prompt, TitleKeywords))   // başlık, isim, naming, convention
        return AgentRole.Title;

    // Default: General Agent
    return AgentRole.General;
}
```

---

## 4. Context Assembly

```
┌──────────────────────────────────────────┐
│           CONTEXT ASSEMBLY               │
│                                          │
│  1. Vault Oku                           │
│     ├── CLAUDE.md (guardrails)          │
│     ├── AGENTS.md (agent sınırları)     │
│     ├── WORKFLOW.md (süreçler)          │
│     └── brain.md (mimari kararlar)      │
│                                          │
│  2. Learning Yükle                       │
│     ├── Patterns (kalıplar)             │
│     ├── Corrections (düzeltmeler)       │
│     └── Knowledge (bilgi)               │
│                                          │
│  3. Session Yükle                        │
│     ├── Önceki mesajlar                 │
│     ├── Mevcut durum                    │
│     └── Branch geçmişi                  │
│                                          │
│  4. Proje Analiz                         │
│     ├── Dosya yapısı                    │
│     ├── Bağımlılıklar                   │
│     └── Mevcut kod kalıpları            │
│                                          │
│  5. Birleştir                            │
│     └── Tek context object oluştur      │
└──────────────────────────────────────────┘
```

---

## 5. Tool Çağrı Döngüsü

```
LLM Yanıtı
    ↓
finish_reason == "tool_calls" ?
    ↓ Evet
Tool'ları Çözümle
    ↓
Her tool için:
    ├── Tool Registry'den bul
    ├── Permission kontrolü
    ├── Input doğrula
    ├── Çalıştır
    └── Sonucu kaydet
    ↓
Sonuçları LLM'a gönder
    ↓
Tekrar et (loop)
```

---

## 6. Agent Sistemi (7 Agent)

| Agent | Mod | Tools | Model |
|-------|-----|-------|-------|
| Build | primary | Tümü | gpt-4o |
| Plan | primary | Read-only | gpt-4o |
| Explore | subagent | Read, Glob, Grep | gpt-4o-mini |
| General | subagent | Tümü | gpt-4o |
| Summary | hidden | Yok | gpt-4o-mini |
| Title | hidden | Yok | gpt-4o-mini (temp:0.5) |
| Compaction | hidden | Yok | gpt-4o-mini |

---

## 7. Streaming Yönetimi

| Event | Tanım |
|-------|-------|
| `text_delta` | Metin parçası geldi |
| `reasoning_delta` | Düşünce parçası geldi |
| `tool_call_start` | Tool çağrısı başladı |
| `tool_call_delta` | Tool çağrısı verisi |
| `tool_call_complete` | Tool çağrısı tamamlandı |
| `turn_complete` | Turlar tamamlandı |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
