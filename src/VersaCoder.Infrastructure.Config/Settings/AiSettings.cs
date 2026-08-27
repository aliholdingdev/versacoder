namespace VersaCoder.Infrastructure.Config.Settings;

/// <summary>
/// AI sağlayıcı ayarları.
/// </summary>
public class AiSettings
{
    /// <summary>Varsayılan sağlayıcı adı.</summary>
    public string DefaultProvider { get; set; } = "OpenAI";

    /// <summary>Varsayılan model.</summary>
    public string DefaultModel { get; set; } = "gpt-4o";

    /// <summary>Maksimum token.</summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>Temperature.</summary>
    public float Temperature { get; set; } = 0.7f;

    /// <summary>Timeout (saniye).</summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>Maksimum retry sayısı.</summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>Streaming aktif mi?</summary>
    public bool EnableStreaming { get; set; } = true;
}

/// <summary>
/// OpenAI sağlayıcı ayarları.
/// </summary>
public class OpenAiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com";
    public string DefaultModel { get; set; } = "gpt-4o";
}

/// <summary>
/// Anthropic sağlayıcı ayarları.
/// </summary>
public class AnthropicSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.anthropic.com";
    public string DefaultModel { get; set; } = "claude-sonnet-4-20250514";
}

/// <summary>
/// Ollama sağlayıcı ayarları.
/// </summary>
public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultModel { get; set; } = "llama3.1";
}
