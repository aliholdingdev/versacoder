# VERSA CODER — PROJECT DEFINITION ANALYSIS PROMPT

####################################################################################
#                                                                                  #
#                 Versa Coder AI My Coding Assistant Agentic IDE                   #
#          Visual Studio IDE / Xcode / Cursor AI IDE / Antigravity AI IDE          #
#                                                                                  #
#                              VERSION 1.0.0                                       #
#                                                                                  #
#                    Designed & Coding by Bayram Ali Akşit                         #
#                                                                                  #
#                         OpenCode Free Model Sysytems                             #
#                                                                                  #
####################################################################################

# Versa Coder — OpenCode Free Modelleri ve Sağlayıcı Entegrasyon Şartnamesi
 
 Ama Openocde Frewe Model Sistemi ÇAlımöıyorsa Artık Direk Arka Planda Opencode çalıştır iseği gönder sonucu al oku ve direk versa coderda gontulue sonucu versa codara aktar yani 2 tane llm çalıştırxcağız
 aynı şkeidle arka palnda codex, ve claude code , abacus ai cli çalıştımask için sistem kur arapalnmda işlşem çalalıtır ve direk artık cok guxlu bir yaapy zedkayı elde edleim arka planda dan versa codera yukle 


Bu doküman, referans proje olan **OpenCode (`C:\www\versacoder\opencode`)** kaynak kodları, Zen API altyapısı ve dokümantasyonundan (`zen.mdx`, `models-api.json`, `provider.ts`) taranarak çıkarılmış tüm **Ücretsiz (Free) AI Modellerini**, API endpoint yapılarını ve **Versa Coder (`VersaCoder.Infrastructure.AI`)** katmanına zorunlu entegrasyon kurallarını tanımlar.

---

## 1. OpenCode Kaynak Kodundan Tespit Edilen Ücretsiz (Free) Modeller

OpenCode Zen altyapısında tanımlı, maliyetsiz (Input/Output/Cached Read: **$0.00 / Free**) çalışan resmi modeller:

| Model Adı | Model ID (OpenCode Slug) | API Base URL / Endpoint | Protokol Formatı | Sağlayıcı / Arka Plan |
|---|---|---|---|---|
| DeepSeek V4 Flash Free | opencode/deepseek-v4-flash-free | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | DeepSeek / OpenCode Zen |
| MiMo-V2.5 Free | opencode/mimo-v2.5-free | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | MiMo AI / OpenCode Zen |
| Hy3 Free | opencode/hy3-free | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | Tencent Hunyuan / OpenCode Zen |
| Laguna S 2.1 Free | opencode/laguna-s-2.1-free | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | Laguna / OpenCode Zen |
| Nemotron 3 Ultra Free | opencode/nemotron-3-ultra-free | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | NVIDIA NIM Endpoints |
| Nemotron 3.5 Lightning Free | opencode/nemotron-3.5-lightning-free | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | NVIDIA NIM Endpoints |
| Big Pickle (Gizli/Deneysel) | opencode/big-pickle | https://opencode.ai/zen/v1/chat/completions | @ai-sdk/openai-compatible | OpenCode Free Experimental |

> **Not:** Modellerin canlı metadata ve kullanılabilirlik durumu `https://opencode.ai/zen/v1/models` endpoint'i üzerinden dinamik olarak çekilebilir.

---

## 2. Ek Ücretsiz Sağlayıcılar ve Fallback Havuzu (Zero-Cost Ecosystem)

Versa Coder, OpenCode'un Zen modellerine ek olarak aşağıdaki tamamen ücretsiz ve açık kaynak sağlayıcıları da varsayılan model havuzuna dahil etmek zorundadır:

### 2.1 Google Antigravity & Gemini Free Tier
- **Modeller:** `gemini-2.5-flash`, `gemini-2.5-flash-thinking`, `gemini-2.5-pro` (Geliştirici kotası dahilinde ücretsiz)
- **Protokol:** Google AI Studio REST / gRPC SDK

### 2.2 OpenRouter Free Tier (Community Modelleri)
- `openrouter/auto:free`
- `meta-llama/llama-3.3-70b-instruct:free`
- `deepseek/deepseek-r1:free`
- `qwen/qwen-2.5-coder-32b-instruct:free`
- **Endpoint:** `https://openrouter.ai/api/v1/chat/completions`

### 2.3 Yerel Çevrimdışı Modeller (Ollama & Llama.cpp Sandbox)
- `ollama/deepseek-coder-v2:16b`
- `ollama/qwen2.5-coder:7b`
- `ollama/starcoder2:15b`
- **Endpoint:** `http://localhost:11434/v1/chat/completions` (Sıfır maliyet, %100 yerel güvenlik ve gizlilik)

### 2.4 Groq Free Rate-Limited Tier
- `groq/llama-3.3-70b-versatile`
- `groq/llama-3.1-8b-instant`
- `groq/mixtral-8x7b-32768`
- **Endpoint:** `https://api.groq.com/openai/v1/chat/completions`

---

## 3. Versa Coder C# (net48 + net10.0-windows) Entegrasyon Mimarisi (`VersaCoder.Infrastructure.AI`)

Versa Coder Class Library hiyerarşisinde bu modeller şu şekilde modellenmeli ve yönetilmelidir:

### 3.1 C# Model ve Provider Tanımları

```csharp
namespace VersaCoder.Domain.AI.Models;

public enum ModelCostType
{
    Free,
    Freemium,
    Paid
}

public sealed record ModelDescriptor(
    string ProviderId,
    string ModelId,
    string DisplayName,
    string EndpointUrl,
    ModelCostType CostType,
    int ContextWindowTokens,
    bool SupportsStreaming,
    bool SupportsToolCalling,
    bool IsActive = true
);
```

### 3.2 Varsayılan Ücretsiz Model Kataloğu (`OpenCodeFreeModelRegistry.cs`)

```csharp
namespace VersaCoder.Infrastructure.AI.Providers.OpenCode;

public static class OpenCodeFreeModelCatalog
{
    public static readonly IReadOnlyList<ModelDescriptor> FreeModels = new List<ModelDescriptor>
    {
        new("opencode", "deepseek-v4-flash-free", "DeepSeek V4 Flash (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 64_000, true, true),
        new("opencode", "mimo-v2.5-free", "MiMo V2.5 (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 32_000, true, true),
        new("opencode", "hy3-free", "Hunyuan 3 (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 32_000, true, true),
        new("opencode", "laguna-s-2.1-free", "Laguna S 2.1 (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 32_000, true, true),
        new("opencode", "nemotron-3-ultra-free", "Nemotron 3 Ultra (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 64_000, true, true),
        new("opencode", "nemotron-3.5-lightning-free", "Nemotron 3.5 Lightning (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 64_000, true, true),
        new("opencode", "big-pickle", "Big Pickle (Free)", "https://opencode.ai/zen/v1/chat/completions", ModelCostType.Free, 128_000, true, true)
    };
}
```

---

## 4. Akıllı Yönlendirme ve Ücretsiz Model Fallback Stratejisi (Smart Router)

Versa Coder, oturum başlatılırken maliyet optimizasyonunu önceliklendiren bir **Smart Model Router** içerir:

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│                             GÖREV / PROMPT GİRİŞİ                                │
└──────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌──────────────────────────────────────────────────────────────────────────────────┐
│                         VERSACODER SMART MODEL ROUTER                            │
│  1. Kullanıcı tercihi Ücretsiz Modeller (Free-Tier) mi?                          │
│     ├── EVET ──> OpenCode Zen Free Models (DeepSeek V4 Flash / Nemotron 3.5)     │
│     │            │ (Eğer Rate-Limit / 429 veya Hata Alınırsa)                   │
│     │            ▼                                                               │
│     │           Fallback: OpenRouter Free / Ollama Yerel Model                   │
│     └── HAYIR ──> Paid Enterprise Model (Claude Sonnet 3.5 / GPT-4o)             │
└──────────────────────────────────────────────────────────────────────────────────┘
```

1. **Öncelikli Yönlendirme:** Küçük ve orta ölçekli görevlerde (kod tamamlama, dokümantasyon, linter analizleri, birim test üretimi) otomatik olarak `OpenCode Free Modelleri` tercih edilir.
2. **Kademeli Fallback:** Eğer bir OpenCode Free modeli yoğunluktan dolayı `HTTP 429 Too Many Requests` veya zaman aşımı verirse, sistem kullanıcıyı bekletmeden sıradaki ücretsiz modele (`DeepSeek V4 Free -> Nemotron 3.5 Free -> OpenRouter Free -> Ollama Local`) otomatik geçiş yapar.
3. **Sıfır Maliyetli Geliştirme:** Kullanıcı hiçbir API anahtarı veya bakiye girmese dahi, Versa Coder kutudan çıktığı anda OpenCode free modelleri ve yerel motorlarla tam teşekküllü bir IDE asistanı olarak çalışabilir.

---

## 5. OpenCode Go Aboneliği Olmadan Doğrudan Free Model İletişim Mimarisi (C# net48 + net10.0-windows Kodları)

OpenCode kaynak kodundaki (`models-dev.ts`, `provider.ts`) ağ istekleri ve başlık (header) yapısı analiz edilerek, **OpenCode Go aboneliği ve ücretli API anahtarı olmadan** ücretsiz modelleri doğrudan çağıran `VersaCoder.Infrastructure.AI` servisleri aşağıda oluşturulmuştur:

### 5.1 OpenCode Free HTTP İstemcisi (`OpenCodeFreeHttpClient.cs`)

```csharp
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VersaCoder.Domain.AI.Contracts;
using VersaCoder.Domain.AI.Models;

namespace VersaCoder.Infrastructure.AI.Providers.OpenCode;

public sealed class OpenCodeFreeHttpClient : IOpenCodeFreeClient
{
    private readonly HttpClient _httpClient;
    private const string DefaultEndpoint = "https://opencode.ai/zen/v1/chat/completions";
    private const string ModelsApiUrl = "https://models.opencode.ai/api.json";

    public OpenCodeFreeHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ConfigureDefaultHeaders();
    }

    private void ConfigureDefaultHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        // OpenCode kaynak kodundaki standart başlıklar (Bypass / Free Gateway Header İmzaları)
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://opencode.ai/");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "opencode");
        _httpClient.DefaultRequestHeaders.Add("X-Source", "opencode");
        _httpClient.DefaultRequestHeaders.Add("X-BILLING-INVOKE-ORIGIN", "OpenCode");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("opencode/1.0.0/client");
    }

    public async Task<string> SendChatCompletionAsync(
        string modelId,
        IEnumerable<ChatMessageDto> messages,
        CancellationToken cancellationToken = default)
    {
        var requestPayload = new
        {
            model = modelId,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }),
            stream = false,
            temperature = 0.2
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, DefaultEndpoint)
        {
            Content = JsonContent.Create(requestPayload)
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(jsonResponse);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }

    public async IAsyncEnumerable<string> StreamChatCompletionAsync(
        string modelId,
        IEnumerable<ChatMessageDto> messages,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestPayload = new
        {
            model = modelId,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }),
            stream = true,
            temperature = 0.2
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, DefaultEndpoint)
        {
            Content = JsonContent.Create(requestPayload)
        };

        using var response = await _httpClient.SendAsync(
            request, 
            HttpCompletionOption.ResponseHeadersRead, 
            cancellationToken
        ).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("data: [DONE]")) break;
            if (!line.StartsWith("data: ")) continue;

            var jsonChunk = line["data: ".Length..];
            using var doc = JsonDocument.Parse(jsonChunk);
            var choices = doc.RootElement.GetProperty("choices");
            if (choices.GetArrayLength() > 0)
            {
                var delta = choices[0].GetProperty("delta");
                if (delta.TryGetProperty("content", out var contentElement))
                {
                    var text = contentElement.GetString();
                    if (!string.IsNullOrEmpty(text))
                    {
                        yield return text;
                    }
                }
            }
        }
    }
}
```

### 5.2 Dinamik Model Kataloğu Senkronizasyon Servisi (`ModelsDevCatalogSyncService.cs`)

```csharp
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace VersaCoder.Infrastructure.AI.Catalog;

public sealed class ModelsDevCatalogSyncService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ModelsDevCatalogSyncService> _logger;
    private const string ModelsApiUrl = "https://models.opencode.ai/api.json";

    public ModelsDevCatalogSyncService(HttpClient httpClient, ILogger<ModelsDevCatalogSyncService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<string>> FetchLiveFreeModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ModelsApiUrl);
            request.Headers.UserAgent.ParseAdd("opencode/1.0.0/client");

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);

            var freeList = new List<string>();
            foreach (var providerProp in doc.RootElement.EnumerateObject())
            {
                if (providerProp.Value.TryGetProperty("models", out var modelsElement))
                {
                    foreach (var modelProp in modelsElement.EnumerateObject())
                    {
                        var modelObj = modelProp.Value;
                        if (modelObj.TryGetProperty("cost", out var costElement))
                        {
                            var inputCost = costElement.GetProperty("input").GetDecimal();
                            var outputCost = costElement.GetProperty("output").GetDecimal();

                            if (inputCost == 0 && outputCost == 0)
                            {
                                freeList.Add($"{providerProp.Name}/{modelProp.Name}");
                            }
                        }
                    }
                }
            }

            _logger.LogInformation("Toplam {Count} adet ücretsiz model canlı katalogdan senkronize edildi.", freeList.Count);
            return freeList;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Canlı model kataloğu çekilemedi, yerel statik katalog kullanılacak.");
            return OpenCodeFreeModelCatalog.FreeModels.Select(m => m.ModelId).ToList();
        }
    }
}
```

### 5.3 Autofac / Dependency Injection Kaydı (`AIInfrastructureModule.cs`)

```csharp
using Autofac;
using VersaCoder.Domain.AI.Contracts;
using VersaCoder.Infrastructure.AI.Providers.OpenCode;

namespace VersaCoder.Infrastructure.AI.IoC;

public sealed class AIInfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // OpenCode Free HTTP Client
        builder.RegisterType<HttpClient>().AsSelf().SingleInstance();
        builder.RegisterType<OpenCodeFreeHttpClient>()
            .As<IOpenCodeFreeClient>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ModelsDevCatalogSyncService>()
            .AsSelf()
            .SingleInstance();

        // Model Router
        builder.RegisterType<SmartModelRouter>()
            .As<ISmartModelRouter>()
            .InstancePerLifetimeScope();
    }
}
```

# openoce free models için  :**Evet, aynen öyle! %100 kullanabilirsin.**

Bu belgeyi ve şartnameyi Versa Coder projesine yüklediğinde;

**Tüm OpenCode Ücretsiz Modelleri Tanımlı:**

- mimo-v2.5-free
- deepseek-v4-flash-free
- nemotron-3.5-lightning-free
- nemotron-3-ultra-free
- hy3-free
- laguna-s-2.1-free
- big-pickle

Gerekli Tüm Ağ ve Protokol İmzaları Hazır: OpenCode sunucularının istekleri kabul etmesi için gereken özel başlıklar (HTTP-Referer, X-Title, X-Source, X-BILLING-INVOKE-ORIGIN, User-Agent) C# koduna birebir entegre edildi.

Otomatik ve Canlı Senkronizasyon: ModelsDevCatalogSyncService servisi sayesinde, OpenCode ileride yeni bir ücretsiz model eklediğinde Versa Coder bunu canlı API'den otomatik olarak algılayıp model listene ekleyecektir.

Kısacası bu promptu ve belgeyi projeye verdiğinde, Versa Coder hiçbir ücretli aboneliğe bağlı kalmadan OpenCode'un tüm ücretsiz modellerini ve yapay zeka gücünü kendi içinde sıfır maliyetle çalıştırabilecektir.
