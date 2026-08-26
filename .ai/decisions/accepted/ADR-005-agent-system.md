---
title: "ADR-005 — Agent System Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-005 — Agent System Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.AI  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, 7 agent'tan oluşan **Master Orchestrator + Specialist** mimarisi kullanacaktır.

## 2. Bağlam

Kullanıcılar çeşitli görevleri yerine getirmek için farklı uzmanlık alanlarına sahip agent'lar istemektedir:
- Kod yazma/düzenleme
- Mimari planlama
- Kod analizi/tarama
- Genel amaçlı görevler
- Özetleme/dokümantasyon
- Başlık oluşturma
- Context sıkıştırma

## 3. Seçenekler

| Seçenek | Artıları | Eksileri |
|---------|----------|----------|
| **Tek Agent** | Basit | Tüm yük tek ajanda |
| **Multi-Agent** | Uzmanlaşma | Karmaşık koordinasyon |
| **Orchestrator Pattern** | Merkezi kontrol | Tek nokta hatası |
| **Peer-to-Peer** | Esnek | Kontrolsüz iletişim |

## 4. Karar

**Master Orchestrator + Specialist Agents** pattern'i seçildi.

## 5. Agent Tanımları

| # | Agent | Kod Adı | Görev | Katman |
|---|-------|---------|-------|--------|
| 1 | Master Orchestrator | `mo` | Görev dağıtımı, koordinasyon | Koordinasyon |
| 2 | Build Agent | `build` | Kod yazma, dosya oluşturma | L2-L4 |
| 3 | Plan Agent | `plan` | Mimari planlama, task dağıtımı | L2 |
| 4 | Explore Agent | `explore` | Kod analizi, dosya tarama | L1-L4 |
| 5 | General Agent | `general` | Genel amaçlı görevler | Tümü |
| 6 | Summary Agent | `summary` | Özetleme, dokümantasyon | L22 |
| 7 | Title Agent | `title` | Başlık oluşturma, isimlendirme | L2 |

## 6. Akış

```
Kullanıcı İsteği
    → [MO] Keyword Analizi
        → [MO] Domain Eşleme
            → [MO] Doğru Agent'a Gönder
                → [Agent] Görevi Yürütür
                    → [Agent] Gerekirse Handover Yap
                        → [Diğer Agent] Görevi Devralır
                    ← [Agent] Sonuç Döndür
                ← [MO] Sonucu Doğrula
            ← [MO] Kullanıcıya Sun
```

## 7. IAgentRunner Arayüzü

```csharp
public interface IAgentRunner
{
    Task<AgentResponse> RunAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<AgentStreamChunk> RunStreamingAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default);
}
```

## 8. AgentRunner Tasarımı

```csharp
public class AgentRunner : IAgentRunner
{
    private readonly AgentSelector _selector;
    private readonly ProviderRouter _router;
    private readonly ToolExecutor _toolExecutor;
    private readonly ContextAssembler _contextAssembler;
    
    public async Task<AgentResponse> RunAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Agent seç
        var agent = _selector.SelectAgent(request);
        
        // 2. Context hazırla
        var context = await _contextAssembler.AssembleAsync(
            agent, request.SessionId);
        
        // 3. Prompt hazırla (system prompt + context + user prompt)
        var fullPrompt = PreparePrompt(agent, context, request);
        
        // 4. LLM'den yanıt al
        var provider = await _router.GetProviderAsync();
        var response = await provider.SendMessageAsync(fullPrompt);
        
        // 5. Tool calls varsa çalıştır
        if (response.ToolCalls.Any())
        {
            var toolResults = await _toolExecutor.ExecuteAsync(
                response.ToolCalls);
            
            // 6. Tool sonuçlarını LLM'e gönder
            response = await provider.SendMessageAsync(
                toolResults, cancellationToken);
        }
        
        // 7. Sonucu döndür
        return response;
    }
}
```

## 9. Agent Selection Algorithm

```csharp
public class AgentSelector
{
    private readonly Dictionary<string, AgentProfile> _agents;
    
    public AgentProfile SelectAgent(AgentRequest request)
    {
        var keywords = ExtractKeywords(request.Prompt);
        
        // 1. Keyword → Agent eşleme
        foreach (var keyword in keywords)
        {
            if (_keywordToAgent.TryGetValue(keyword, out var agent))
                return agent;
        }
        
        // 2. Context bazlı seçim
        if (request.HasCodeContext)
            return _agents["build"];
        
        if (request.HasArchitectureContext)
            return _agents["plan"];
        
        // 3. Default: General Agent
        return _agents["general"];
    }
}
```

## 10. Handover Protokolü

```csharp
public class AgentHandover
{
    public string SourceAgent { get; set; }
    public string TargetAgent { get; set; }
    public string Reason { get; set; }
    public AgentRequest Request { get; set; }
    public Priority Priority { get; set; }
}
```

**Handover Senaryoları:**

| Kaynak | Hedef | Senaryo |
|--------|-------|---------|
| Build | Plan | Mimari karar gerekti |
| Build | Explore | Kod analizi gerekti |
| Explore | Build | Dosya bulundu, düzenleme gerekli |
| Plan | Build | Plan hazır, kodlama başlayabilir |
| General | Summary | Uzun çıktı özetlenmeli |
| General | Title | Başlık oluşturulmalı |
| Summary | Title | Özet hazır, başlık gerekli |

## 11. Context Lock

```csharp
public class ContextLockManager
{
    private readonly Dictionary<string, LockInfo> _locks;
    
    public async Task<bool> AcquireLockAsync(
        string resourceId,
        string agentId,
        Priority priority,
        TimeSpan timeout)
    {
        // 1. Kilidi kontrol et
        // 2. Öncelik kontrolü (düşük öncelik yüksek önceliği kırabilir)
        // 3. Deadlock kontrolü (MO en eski kilidi kırar)
        // 4. Kilidi al ve logla
    }
    
    public async Task ReleaseLockAsync(
        string resourceId,
        string agentId)
    {
        // 1. Kilidi serbest bırak
        // 2. Logla
    }
}
```

## 12. Sağlık Kontrolü

| Durum | Kod | Tanım |
|-------|-----|-------|
| Healthy | 200 | Görev tamamlandı |
| Degraded | 301 | Yavaş yanıt (>15s) |
| Retry | 408 | Timeout, yeniden deneniyor |
| Failed | 500 | 3 retry başarısız |
| Dead | 503 | Yanıt yok, escalation |

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
