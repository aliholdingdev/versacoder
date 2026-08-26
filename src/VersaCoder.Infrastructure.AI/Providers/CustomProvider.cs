using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VersaCoder.Abstractions.Providers;

namespace VersaCoder.Infrastructure.AI.Providers;

public class CustomProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomProvider> _logger;
    private readonly CustomProviderOptions _options;

    public string Name => _options.Name;
    public bool IsAvailable => !string.IsNullOrEmpty(_options.ApiKey) || _options.UseNoAuth;

    public CustomProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<CustomProvider> logger,
        IOptions<CustomProviderOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("Custom");
        _logger = logger;
        _options = options.Value;
    }

    public async Task<LLMResponse> SendMessageAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var payload = BuildPayload(request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.ChatEndpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        if (!string.IsNullOrEmpty(_options.ApiKey) && !_options.UseNoAuth)
            httpRequest.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        stopwatch.Stop();

        var content = result.TryGetProperty("choices", out var choices)
            ? choices[0].TryGetProperty("message", out var msg) ? msg.GetProperty("content").GetString() ?? string.Empty : string.Empty
            : string.Empty;

        return new LLMResponse
        {
            Content = content,
            Model = request.Model ?? _options.DefaultModel,
            Duration = stopwatch.Elapsed
        };
    }

    public async IAsyncEnumerable<LLMStreamChunk> SendStreamingMessageAsync(
        LLMRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = BuildPayload(request);
        payload["stream"] = true;

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.ChatEndpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        if (!string.IsNullOrEmpty(_options.ApiKey) && !_options.UseNoAuth)
            httpRequest.Headers.Add("Authorization", $"Bearer {_options.ApiKey}");

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("data: ")) line = line[6..];
            if (line == "[DONE]") break;

            var chunk = TryDeserializeStreamChunk(line);
            if (chunk.HasValue && chunk.Value.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var delta = choices[0].TryGetProperty("delta", out var d) ? d : default;
                if (delta.TryGetProperty("content", out var c))
                {
                    yield return new LLMStreamChunk
                    {
                        Content = c.GetString() ?? string.Empty,
                        IsComplete = false
                    };
                }
            }
        }

        yield return new LLMStreamChunk { IsComplete = true };
    }

    public Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_options.UseNoAuth || !string.IsNullOrEmpty(_options.ApiKey));
    }

    private Dictionary<string, object> BuildPayload(LLMRequest request)
    {
        var messages = new List<Dictionary<string, string>>();
        if (!string.IsNullOrEmpty(request.SystemPrompt))
            messages.Add(new() { ["role"] = "system", ["content"] = request.SystemPrompt });

        foreach (var msg in request.Messages)
            messages.Add(new() { ["role"] = msg.Role, ["content"] = msg.Content });

        return new Dictionary<string, object>
        {
            ["model"] = request.Model ?? _options.DefaultModel,
            ["messages"] = messages,
            ["temperature"] = request.Temperature,
            ["max_tokens"] = request.MaxTokens
        };
    }

    private JsonElement? TryDeserializeStreamChunk(string line)
    {
        try { return JsonSerializer.Deserialize<JsonElement>(line); }
        catch { return null; }
    }
}

public class CustomProviderOptions
{
    public string Name { get; set; } = "Custom";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ChatEndpoint { get; set; } = "/v1/chat/completions";
    public string DefaultModel { get; set; } = "default";
    public bool UseNoAuth { get; set; }
}
