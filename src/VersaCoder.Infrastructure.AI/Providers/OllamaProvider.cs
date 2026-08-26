using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VersaCoder.Abstractions.Providers;

namespace VersaCoder.Infrastructure.AI.Providers;

public class OllamaProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaProvider> _logger;
    private readonly OllamaOptions _options;

    public string Name => "Ollama";
    public bool IsAvailable => true;

    public OllamaProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<OllamaProvider> logger,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("Ollama");
        _logger = logger;
        _options = options.Value;
    }

    public async Task<LLMResponse> SendMessageAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var messages = new List<Dictionary<string, string>>();
        if (!string.IsNullOrEmpty(request.SystemPrompt))
            messages.Add(new() { ["role"] = "system", ["content"] = request.SystemPrompt });

        foreach (var msg in request.Messages)
            messages.Add(new() { ["role"] = msg.Role, ["content"] = msg.Content });

        var payload = new Dictionary<string, object>
        {
            ["model"] = request.Model ?? _options.DefaultModel,
            ["messages"] = messages,
            ["stream"] = false
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/chat")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: cancellationToken);
        stopwatch.Stop();

        return new LLMResponse
        {
            Content = result?.Message?.Content ?? string.Empty,
            Model = result?.Model ?? _options.DefaultModel,
            Duration = stopwatch.Elapsed
        };
    }

    public async IAsyncEnumerable<LLMStreamChunk> SendStreamingMessageAsync(
        LLMRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var messages = new List<Dictionary<string, string>>();
        if (!string.IsNullOrEmpty(request.SystemPrompt))
            messages.Add(new() { ["role"] = "system", ["content"] = request.SystemPrompt });

        foreach (var msg in request.Messages)
            messages.Add(new() { ["role"] = msg.Role, ["content"] = msg.Content });

        var payload = new Dictionary<string, object>
        {
            ["model"] = request.Model ?? _options.DefaultModel,
            ["messages"] = messages,
            ["stream"] = true
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/chat")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrEmpty(line)) continue;

            var chunk = TryDeserializeStreamChunk(line);
            if (chunk?.Message?.Content != null)
            {
                yield return new LLMStreamChunk
                {
                    Content = chunk.Message.Content,
                    IsComplete = chunk.Done ?? false
                };
            }
        }

        yield return new LLMStreamChunk { IsComplete = true };
    }

    public async Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/tags", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private OllamaStreamChunk? TryDeserializeStreamChunk(string line)
    {
        try { return JsonSerializer.Deserialize<OllamaStreamChunk>(line); }
        catch { return null; }
    }
}

public class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultModel { get; set; } = "llama3.1";
}

internal class OllamaResponse
{
    public string? Model { get; set; }
    public OllamaMessage? Message { get; set; }
}

internal class OllamaMessage
{
    public string? Content { get; set; }
}

internal class OllamaStreamChunk
{
    public OllamaMessage? Message { get; set; }
    public bool? Done { get; set; }
}
