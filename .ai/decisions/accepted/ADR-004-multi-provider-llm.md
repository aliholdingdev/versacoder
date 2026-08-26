---
title: "ADR-004 — Multi-Provider LLM Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-004 — Multi-Provider LLM Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.AI  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, birden fazla LLM sağlayıcısını destekleyen **Provider Router** mimarisi kullanacaktır.

## 2. Bağlam

Kullanıcılar farklı LLM sağlayıcılarını kullanmak istemektedir:
- OpenAI (GPT-4o, GPT-4.1, o3)
- Anthropic (Claude Opus 4, Sonnet 4)
- Google (Gemini 2.5 Pro/Flash)
- Ollama (Llama, Qwen, vb.)
- Özel modeller (OpenAI uyumlu API)

## 3. Seçenekler

| Seçenek | Artıları | Eksileri |
|---------|----------|----------|
| **Tek Provider** | Basit | Tek sağlayıcıya bağımlılık |
| **Strategy Pattern** | Esnek, genişletilebilir | Karmaşık test |
| **Adapter Pattern** | Her sağlayıcı için adaptör | Fazla kod |
| **Provider Router** | Tek noktadan erişim, rotalama | Orta karmaşıklık |

## 4. Karar

**Provider Router** + **Strategy Pattern** kombinasyonu seçildi.

## 5. Tasarım

```
┌─────────────────────────────────────────────────────────────┐
│                      AgentRunner                            │
│  (Agent'ı başlatır, prompt yollar, tool'ları çalıştırır)    │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    ProviderRouter                            │
│  (Sağlayıcı seçimini yapar, fallback yönetir)              │
│  - Default provider: configurasyondan                      │
│  - Model-specific routing                                  │
│  - Fallback zinciri (hata durumunda)                       │
└──────┬──────────┬──────────┬──────────┬──────────┬─────────┘
       │          │          │          │          │
       ▼          ▼          ▼          ▼          ▼
   ┌───────┐ ┌───────┐ ┌───────┐ ┌───────┐ ┌───────┐
   │OpenAI │ │Anthro │ │Google │ │Ollama │ │Custom │
   │Provider│ │Provider│ │Provider│ │Provider│ │Provider│
   └───────┘ └───────┘ └───────┘ └───────┘ └───────┘
```

## 6. ILLMProvider Arayüzü

```csharp
public interface ILLMProvider
{
    string Name { get; }
    bool IsAvailable { get; }
    
    Task<LLMResponse> SendMessageAsync(
        LLMRequest request,
        CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<LLMStreamChunk> SendStreamingMessageAsync(
        LLMRequest request,
        CancellationToken cancellationToken = default);
    
    Task<bool> ValidateApiKeyAsync(
        CancellationToken cancellationToken = default);
}
```

## 7. ProviderRouter Tasarımı

```csharp
public class ProviderRouter
{
    private readonly Dictionary<string, ILLMProvider> _providers;
    private readonly IOptions<AIOptions> _options;
    
    public async Task<ILLMProvider> GetProviderAsync(
        string? preferredProvider = null,
        string? model = null)
    {
        // 1. Tercih edilen sağlayıcı varsa kullan
        // 2. Model-specific routing yap
        // 3. Fallback zincirinden ilk uygun olanı seç
        // 4. Hata durumunda bir sonraki sağlayıcıya geç
    }
    
    public async Task<FallbackResult> ExecuteWithFallbackAsync(
        Func<ILLMProvider, Task<LLMResponse>> operation)
    {
        // Her sağlayıcı için dene
        // Başarısız olursa bir sonrakine geç
        // Tümü başarısızsa hata fırlat
    }
}
```

## 8. Konfigürasyon

```json
{
  "AI": {
    "DefaultProvider": "openai",
    "DefaultModel": "gpt-4o",
    "Providers": {
      "openai": {
        "Enabled": true,
        "ApiKey": "${OPENAI_API_KEY}",
        "BaseUrl": "https://api.openai.com/v1",
        "Models": ["gpt-4o", "gpt-4.1", "o3"]
      },
      "anthropic": {
        "Enabled": true,
        "ApiKey": "${ANTHROPIC_API_KEY}",
        "BaseUrl": "https://api.anthropic.com",
        "Models": ["claude-opus-4", "claude-sonnet-4"]
      },
      "google": {
        "Enabled": false,
        "ApiKey": "${GOOGLE_API_KEY}",
        "Models": ["gemini-2.5-pro", "gemini-2.5-flash"]
      },
      "ollama": {
        "Enabled": false,
        "BaseUrl": "http://localhost:11434",
        "Models": ["llama3", "qwen2"]
      },
      "custom": {
        "Enabled": false,
        "ApiKey": "${CUSTOM_API_KEY}",
        "BaseUrl": "${CUSTOM_BASE_URL}",
        "Models": ["custom-model"]
      }
    },
    "FallbackChain": ["openai", "anthropic", "ollama"],
    "TimeoutSeconds": 30,
    "MaxRetries": 3
  }
}
```

## 9. Akış

```
1. Kullanıcı prompt yollar
2. AgentRunner, ProviderRouter'dan sağlayıcı ister
3. ProviderRouter:
   a. Tercih edilen sağlayıcıyı kontrol et
   b. Model-specific routing kontrol et
   c. Fallback zincirinden uygun olanı seç
4. Seçilen sağlayıcıya istek gönder
5. Yanıt al ve döndür
6. Hata olursa:
   a. Bir sonraki sağlayıcıya geç
   b. Retry sayısını kontrol et
   c. Tümü başarısızsa hata fırlat
```

## 10. Uygulama Notları

- Her sağlayıcı ayrı bir sınıfta (`OpenAIProvider`, `AnthropicProvider`, vb.)
- Tüm provider'lar `ILLMProvider` arayüzünü implemente eder
- Provider'lar constructor injection ile eklenir
- Konfigürasyon `IOptions<AIOptions>` ile okunur
- API key'leri `Infrastructure.Auth` modülünden güvenli şekilde alınır

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
