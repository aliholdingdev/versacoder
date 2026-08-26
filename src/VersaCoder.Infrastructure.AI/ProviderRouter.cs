using Microsoft.Extensions.Logging;
using VersaCoder.Abstractions.Providers;

namespace VersaCoder.Infrastructure.AI;

public class ProviderRouter
{
    private readonly Dictionary<string, ILLMProvider> _providers;
    private readonly ILogger<ProviderRouter> _logger;
    private string _defaultProvider = "OpenAI";

    public ProviderRouter(ILogger<ProviderRouter> logger)
    {
        _providers = new Dictionary<string, ILLMProvider>(StringComparer.OrdinalIgnoreCase);
        _logger = logger;
    }

    public void RegisterProvider(ILLMProvider provider)
    {
        _providers[provider.Name] = provider;
        _logger.LogInformation("Registered LLM provider: {Provider}", provider.Name);
    }

    public void SetDefault(string providerName)
    {
        if (_providers.ContainsKey(providerName))
            _defaultProvider = providerName;
    }

    public ILLMProvider GetProvider(string? providerName = null)
    {
        var name = providerName ?? _defaultProvider;
        if (_providers.TryGetValue(name, out var provider))
            return provider;

        _logger.LogWarning("Provider {Provider} not found, using default: {Default}", name, _defaultProvider);
        return _providers[_defaultProvider];
    }

    public List<string> GetAvailableProviders()
    {
        return _providers.Values
            .Where(p => p.IsAvailable)
            .Select(p => p.Name)
            .ToList();
    }

    public async Task<LLMResponse> RouteAsync(LLMRequest request, string? providerName = null, CancellationToken cancellationToken = default)
    {
        var provider = GetProvider(providerName);
        return await provider.SendMessageAsync(request, cancellationToken);
    }
}
