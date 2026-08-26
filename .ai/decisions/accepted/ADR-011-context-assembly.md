---
title: "ADR-011 — Context Assembly Pipeline"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-011 — Context Assembly Pipeline

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.Context  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, **7 adımlık** bir Context Assembly Pipeline kullanacaktır.

## 2. Pipeline Adımları

| Adım | Amaç | Kaynak | Max Süre |
|------|------|--------|----------|
| 1 | Vault oku | .ai/ CLAUDE.md, AGENTS.md, WORKFLOW.md, brain.md | 3s |
| 2 | Learning yükle | .ai/learning/ patterns, corrections, knowledge | 2s |
| 3 | Session yükle | .ai/memory/ session state | 2s |
| 4 | Proje analiz | .ai/project/ structure, analysis | 3s |
| 5 | Context assembly | .ai/context/ sources, rules, priorities | 2s |
| 6 | Prompt al | Kullanıcı girdisi | Anlık |
| 7 | AI'a sun | Birleştirilmiş context + prompt | Anlık |

**Toplam max süre:** 12s

## 3. Context Priority

| Kaynak | Öncelik | Token Budget |
|--------|---------|--------------|
| Vault (rules) | CRITICAL | Sabit 2000 |
| Project structure | HIGH | Max 3000 |
| File content | HIGH | Max 5000 |
| Session history | MEDIUM | Max 3000 |
| Learning data | MEDIUM | Max 2000 |
| Diagrams | LOW | Max 1000 |
| User prefs | MEDIUM | Max 500 |

**Toplam token budget:** ~16,500

## 4. Context Assembly Akışı

```
Session Başlangıcı
    → [1. Vault Oku] .ai/ CLAUDE.md, AGENTS.md, WORKFLOW.md, brain.md
    → [2. Learning Yükle] patterns, corrections, knowledge, rules
    → [3. Session Yükle] geçmiş oturum, branch, fork
    → [4. Proje Analiz] dosya yapısı, class'lar, method'lar
    → [5. Context Assembly] tüm kaynakları birleştir
    → [6. Prompt Al] kullanıcı girdisi
    → [7. AI'a Sun] birleştirilmiş context + prompt
```

## 5. Context Compressor

```csharp
public class ContextCompressor
{
    public async Task<AssembledContext> CompressAsync(
        AssembledContext context,
        int maxTokens)
    {
        // 1. Öncelik sırasına göre ekle
        // 2. Token bütçesini aşarsa sıkıştır
        // 3. LLM kullanarak özet oluştur
    }
}
```

## 6. Context Storage

```csharp
public class ContextStorage
{
    public async Task SaveContextAsync(
        SessionId sessionId,
        AssembledContext context)
    {
        // Context'i JSON olarak kaydet
    }
    
    public async Task<AssembledContext?> LoadContextAsync(
        SessionId sessionId)
    {
        // Context'i yükle
    }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
