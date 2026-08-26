using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VersaCoder.Abstractions.Providers;

namespace VersaCoder.Infrastructure.AI.Providers;

public class OpenAIProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly OpenAIOptions _options;

    public string Name => "OpenAI";
    public bool IsAvailable => !string.IsNullOrEmpty(_options.ApiKey);

    public OpenAIProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAIProvider> logger,
        IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient("OpenAI");
        _logger = logger;
        _options = options.Value;
    }

    public async Task<LLMResponse> SendMessageAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var payload = BuildPayload(request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(cancellationToken: cancellationToken);
        stopwatch.Stop();

        var choice = result?.Choices?.FirstOrDefault();
        return new LLMResponse
        {
            Content = choice?.Message?.Content ?? string.Empty,
            Model = result?.Model ?? _options.DefaultModel,
            PromptTokens = result?.Usage?.PromptTokens ?? 0,
            CompletionTokens = result?.Usage?.CompletionTokens ?? 0,
            TotalTokens = result?.Usage?.TotalTokens ?? 0,
            Duration = stopwatch.Elapsed
        };
    }

    public async IAsyncEnumerable<LLMStreamChunk> SendStreamingMessageAsync(
        LLMRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = BuildPayload(request);
        payload["stream"] = true;

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions")
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
            if (line.StartsWith("data: ")) line = line[6..];
            if (line == "[DONE]") break;

            var chunk = TryDeserializeStreamChunk(line);
            if (chunk != null)
            {
                var content = chunk.Choices?.FirstOrDefault()?.Delta?.Content;
                if (!string.IsNullOrEmpty(content))
                {
                    yield return new LLMStreamChunk
                    {
                        Content = content,
                        IsComplete = false
                    };
                }
            }
        }

        yield return new LLMStreamChunk { IsComplete = true };
    }

    public async Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "v1/models");
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private Dictionary<string, object> BuildPayload(LLMRequest request)
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
            ["temperature"] = request.Temperature,
            ["max_tokens"] = request.MaxTokens
        };

        return payload;
    }

    private OpenAIStreamChunk? TryDeserializeStreamChunk(string line)
    {
        try { return JsonSerializer.Deserialize<OpenAIStreamChunk>(line); }
        catch { return null; }
    }
}

public class OpenAIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com";
    public string DefaultModel { get; set; } = "gpt-4o";
}

internal class OpenAIResponse
{
    public string? Model { get; set; }
    public List<OpenAIChoice>? Choices { get; set; }
    public OpenAIUsage? Usage { get; set; }
}

internal class OpenAIChoice
{
    public OpenAIMessage? Message { get; set; }
}

internal class OpenAIMessage
{
    public string? Content { get; set; }
}

internal class OpenAIUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

internal class OpenAIStreamChunk
{
    public List<OpenAIStreamChoice>? Choices { get; set; }
}

internal class OpenAIStreamChoice
{
    public OpenAIStreamDelta? Delta { get; set; }
}

internal class OpenAIStreamDelta
{
    public string? Content { get; set; }
}
