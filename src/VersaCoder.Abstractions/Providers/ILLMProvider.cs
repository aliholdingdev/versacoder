namespace VersaCoder.Abstractions.Providers;

public interface ILLMProvider
{
    string Name { get; }
    bool IsAvailable { get; }
    
    Task<LLMResponse> SendMessageAsync(LLMRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<LLMStreamChunk> SendStreamingMessageAsync(LLMRequest request, CancellationToken cancellationToken = default);
    Task<bool> ValidateApiKeyAsync(CancellationToken cancellationToken = default);
}

public class LLMRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public List<LLMMessage> Messages { get; set; } = new();
    public string? Model { get; set; }
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 4096;
    public List<LLMTool>? Tools { get; set; }
}

public class LLMMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class LLMTool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class LLMResponse
{
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public List<LLMToolCall>? ToolCalls { get; set; }
    public TimeSpan Duration { get; set; }
}

public class LLMStreamChunk
{
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

public class LLMToolCall
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Arguments { get; set; } = new();
}
