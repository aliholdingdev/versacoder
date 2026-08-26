---
title: "ADR-008 — Context Management Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-008 — Context Management Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.Context  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, **7 adımlık Context Assembly Pipeline** kullanan, **multi-source** bir context yönetim sistemi kullanacaktır.

## 2. Bağlam

AI ajanlarının doğru yanıt verebilmesi için kapsamlı bir bağlama ihtiyacı vardır:
- Proje yapısı ve dosyalar
- Mevcut kod içeriği
- Önceki oturum geçmişi
- Öğrenilen patterns ve düzeltmeler
- Diyagramlar ve mimari kararlar
- Vault kuralları ve politikaları

## 3. Seçenekler

| Seçenek | Artıları | Eksileri |
|---------|----------|----------|
| **Static Context** | Basit | Esnek değil |
| **Dynamic Assembly** | Esnek, kapsamlı | Karmaşık |
| **RAG (Retrieval)** | Akıllı seçim | Yavaş, kaynak yoğun |
| **Hybrid** | Dengeli | Orta karmaşıklık |

## 4. Karar

**Dynamic Assembly** + **Priority-based Selection** kombinasyonu seçildi.

## 5. Context Assembly Pipeline

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

## 6. Context Sources

| Kaynak | Amaç | Öncelik |
|--------|------|---------|
| **Vault** | Kurallar, politikalar, tanım | CRITICAL |
| **Project** | Dosya yapısı, class'lar | HIGH |
| **File** | Mevcut kod içeriği | HIGH |
| **Session** | Geçmiş oturum, conversation | MEDIUM |
| **Learning** | Patterns, düzeltmeler | MEDIUM |
| **Diagram** | Mimari diyagramlar | LOW |
| **User** | Kullanıcı tercihleri | MEDIUM |

## 7. ContextAssembler Tasarımı

```csharp
public class ContextAssembler
{
    private readonly List<IContextProvider> _providers;
    private readonly IContextPrioritizer _prioritizer;
    private readonly IContextCompressor _compressor;
    private readonly IOptions<ContextOptions> _options;
    
    public async Task<AssembledContext> AssembleAsync(
        AgentRole agentRole,
        SessionId sessionId,
        CancellationToken cancellationToken = default)
    {
        var context = new AssembledContext();
        
        // 1. Tüm provider'lardan context topla
        foreach (var provider in _providers)
        {
            if (provider.IsAvailableFor(agentRole))
            {
                var providerContext = await provider.GetContextAsync(
                    sessionId, cancellationToken);
                context.Merge(providerContext);
            }
        }
        
        // 2. Önceliklendir
        context = _prioritizer.Prioritize(context, agentRole);
        
        // 3. Sıkıştır (token limiti varsa)
        if (context.TokenCount > _options.Value.MaxTokens)
        {
            context = await _compressor.CompressAsync(
                context, _options.Value.MaxTokens);
        }
        
        return context;
    }
}
```

## 8. IContextProvider Arayüzü

```csharp
public interface IContextProvider
{
    string Name { get; }
    int Priority { get; }
    
    bool IsAvailableFor(AgentRole agentRole);
    
    Task<ContextData> GetContextAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default);
}
```

## 9. ContextProvider Örnekleri

### 9.1 ProjectContextProvider

```csharp
public class ProjectContextProvider : IContextProvider
{
    public string Name => "project";
    public int Priority => (int)ContextPriority.HIGH;
    
    public bool IsAvailableFor(AgentRole agentRole)
    {
        return agentRole != AgentRole.TITLE &&
               agentRole != AgentRole.SUMMARY;
    }
    
    public async Task<ContextData> GetContextAsync(
        SessionId sessionId,
        CancellationToken cancellationToken)
    {
        var projectAnalysis = await _analyzer.AnalyzeAsync(cancellationToken);
        
        return new ContextData
        {
            Source = Name,
            Content = new
            {
                projectAnalysis.Structure,
                projectAnalysis.Classes,
                projectAnalysis.Methods,
                projectAnalysis.Dependencies
            },
            TokenCount = CalculateTokens(projectAnalysis)
        };
    }
}
```

### 9.2 LearningContextProvider

```csharp
public class LearningContextProvider : IContextProvider
{
    public string Name => "learning";
    public int Priority => (int)ContextPriority.MEDIUM;
    
    public bool IsAvailableFor(AgentRole agentRole)
    {
        return agentRole == AgentRole.BUILD ||
               agentRole == AgentRole.PLAN;
    }
    
    public async Task<ContextData> GetContextAsync(
        SessionId sessionId,
        CancellationToken cancellationToken)
    {
        var patterns = await _patternStore.GetRelevantPatternsAsync(
            sessionId, cancellationToken);
        var corrections = await _correctionTracker.GetRecentCorrectionsAsync(
            cancellationToken);
        
        return new ContextData
        {
            Source = Name,
            Content = new
            {
                patterns,
                corrections
            },
            TokenCount = CalculateTokens(patterns, corrections)
        };
    }
}
```

## 10. ContextPrioritizer

```csharp
public class ContextPrioritizer : IContextPrioritizer
{
    public AssembledContext Prioritize(
        AssembledContext context,
        AgentRole agentRole)
    {
        var prioritized = new AssembledContext();
        
        // 1. CRITICAL: Vault kuralları (her zaman ilk sırada)
        prioritized.Merge(context.GetByPriority(
            ContextPriority.CRITICAL));
        
        // 2. HIGH: Proje ve dosya bağlamı
        prioritized.Merge(context.GetByPriority(
            ContextPriority.HIGH));
        
        // 3. MEDIUM: Session ve learning
        prioritized.Merge(context.GetByPriority(
            ContextPriority.MEDIUM));
        
        // 4. LOW: Diyagramlar (alan kalırsa)
        var remainingTokens = _options.Value.MaxTokens - 
            prioritized.TokenCount;
        
        if (remainingTokens > 0)
        {
            var lowPriority = context.GetByPriority(
                ContextPriority.LOW);
            lowPriority.LimitTokens(remainingTokens);
            prioritized.Merge(lowPriority);
        }
        
        return prioritized;
    }
}
```

## 11. ContextCompressor

```csharp
public class ContextCompressor : IContextCompressor
{
    public async Task<AssembledContext> CompressAsync(
        AssembledContext context,
        int maxTokens)
    {
        var compressed = new AssembledContext();
        
        // 1. Öncelik sırasına göre ekle
        foreach (var source in context.Sources
            .OrderBy(s => s.Priority))
        {
            if (compressed.TokenCount + source.TokenCount <= maxTokens)
            {
                compressed.Merge(source);
            }
            else
            {
                // 2. Kalan alanı hesapla
                var remaining = maxTokens - compressed.TokenCount;
                
                // 3. Kaynağı sıkıştır
                var compressedSource = await CompressSourceAsync(
                    source, remaining);
                compressed.Merge(compressedSource);
                
                break;
            }
        }
        
        return compressed;
    }
    
    private async Task<ContextData> CompressSourceAsync(
        ContextData source,
        int maxTokens)
    {
        // LLM kullanarak özeti sıkıştır
        var summary = await _llm.SummarizeAsync(
            source.Content.ToString(),
            maxTokens);
        
        return new ContextData
        {
            Source = source.Source + " (compressed)",
            Content = summary,
            TokenCount = maxTokens
        };
    }
}
```

## 12. Context Storage

```csharp
public class ContextStorage
{
    private readonly VersaCoderDbContext _context;
    
    public async Task SaveContextAsync(
        SessionId sessionId,
        AssembledContext context)
    {
        var contextEntity = new ContextEntity
        {
            SessionId = sessionId,
            Data = JsonSerializer.Serialize(context),
            TokenCount = context.TokenCount,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Contexts.Add(contextEntity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<AssembledContext?> LoadContextAsync(
        SessionId sessionId)
    {
        var entity = await _context.Contexts
            .Where(c => c.SessionId == sessionId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
        
        if (entity == null) return null;
        
        return JsonSerializer.Deserialize<AssembledContext>(entity.Data);
    }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
