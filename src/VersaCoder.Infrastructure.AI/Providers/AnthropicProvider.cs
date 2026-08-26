using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VersaCoder.Abstractions.Providers;

namespace VersaCoder.Infrastructure.AI.Providers;

public class AnthropicProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AnthropicProvider> _logger;
    private readonly AnthropicOptions _options;

    public string Name => "Anthropic";
    public bool IsAvailable => !string.IsNullOrEmpty(_options.ApiKey);

    public AnthropicProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<AnthropicProvider> logger,
        IOptions<AnthropicOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("Anthropic");
        _logger = logger;
        _options = options.Value;
    }

    public async Task<LLMResponse> SendMessageAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var payload = BuildPayload(request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/messages")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        httpRequest.Headers.Add("x-api-key", _options.ApiKey);
        httpRequest.Headers.Add("anthropic-version", "2023-06-01");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>(cancellationToken: cancellationToken);
        stopwatch.Stop();

        var content = result?.Content?.FirstOrDefault(c => c.Type == "text")?.Text ?? string.Empty;

        return new LLMResponse
        {
            Content = content,
            Model = result?.Model ?? _options.DefaultModel,
            PromptTokens = result?.Usage?.InputTokens ?? 0,
            CompletionTokens = result?.Usage?.OutputTokens ?? 0,
            TotalTokens = (result?.Usage?.InputTokens ?? 0) + (result?.Usage?.OutputTokens ?? 0),
            Duration = stopwatch.Elapsed
        };
    }

    public async IAsyncEnumerable<LLMStreamChunk> SendStreamingMessageAsync(
        LLMRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = BuildPayload(request);
        payload["stream"] = true;

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/messages")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        httpRequest.Headers.Add("x-api-key", _options.ApiKey);
        httpRequest.Headers.Add("anthropic-version", "2023-06-01");

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
            if (chunk?.Type == "content_block_delta" && chunk.Delta?.Text != null)
            {
                yield return new LLMStreamChunk
                {
                    Content = chunk.Delta.Text,
                    IsComplete = false
                };
            }
        }

        yield return new LLMStreamChunk { IsComplete = true };
    }

    public Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!string.IsNullOrEmpty(_options.ApiKey));
    }

    private Dictionary<string, object> BuildPayload(LLMRequest request)
    {
        var messages = request.Messages.Select(m => new Dictionary<string, string>
        {
            ["role"] = m.Role,
            ["content"] = m.Content
        }).ToList();

        var payload = new Dictionary<string, object>
        {
            ["model"] = request.Model ?? _options.DefaultModel,
            ["messages"] = messages,
            ["max_tokens"] = request.MaxTokens
        };

        if (!string.IsNullOrEmpty(request.SystemPrompt))
            payload["system"] = request.SystemPrompt;

        return payload;
    }

    private AnthropicStreamChunk? TryDeserializeStreamChunk(string line)
    {
        try { return JsonSerializer.Deserialize<AnthropicStreamChunk>(line); }
        catch { return null; }
    }
}

public class AnthropicOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.anthropic.com";
    public string DefaultModel { get; set; } = "claude-sonnet-4-20250514";
}

internal class AnthropicResponse
{
    public string? Model { get; set; }
    public List<AnthropicContent>? Content { get; set; }
    public AnthropicUsage? Usage { get; set; }
}

internal class AnthropicContent
{
    public string? Type { get; set; }
    public string? Text { get; set; }
}

internal class AnthropicUsage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
}

internal class AnthropicStreamChunk
{
    public string? Type { get; set; }
    public AnthropicStreamDelta? Delta { get; set; }
}

internal class AnthropicStreamDelta
{
    public string? Text { get; set; }
}
