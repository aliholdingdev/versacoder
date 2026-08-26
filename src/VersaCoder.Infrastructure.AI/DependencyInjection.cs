using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VersaCoder.Abstractions.Providers;
using VersaCoder.Abstractions.Services;
using VersaCoder.Infrastructure.AI.Providers;

namespace VersaCoder.Infrastructure.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureAI(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HttpClient factory
        services.AddHttpClient();

        // Register Options
        services.Configure<OpenAIOptions>(configuration.GetSection("AI:OpenAI"));
        services.Configure<AnthropicOptions>(configuration.GetSection("AI:Anthropic"));
        services.Configure<OllamaOptions>(configuration.GetSection("AI:Ollama"));
        services.Configure<CustomProviderOptions>(configuration.GetSection("AI:Custom"));

        // Register Providers
        services.AddSingleton<ILLMProvider, OpenAIProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<OpenAIProvider>>();
            var options = Microsoft.Extensions.Options.Options.Create(new OpenAIOptions
            {
                ApiKey = configuration["AI:OpenAI:ApiKey"] ?? "",
                BaseUrl = configuration["AI:OpenAI:BaseUrl"] ?? "https://api.openai.com",
                DefaultModel = configuration["AI:OpenAI:DefaultModel"] ?? "gpt-4o"
            });
            return new OpenAIProvider(factory, logger, options);
        });

        services.AddSingleton<ILLMProvider, AnthropicProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<AnthropicProvider>>();
            var options = Microsoft.Extensions.Options.Options.Create(new AnthropicOptions
            {
                ApiKey = configuration["AI:Anthropic:ApiKey"] ?? "",
                BaseUrl = configuration["AI:Anthropic:BaseUrl"] ?? "https://api.anthropic.com",
                DefaultModel = configuration["AI:Anthropic:DefaultModel"] ?? "claude-sonnet-4-20250514"
            });
            return new AnthropicProvider(factory, logger, options);
        });

        services.AddSingleton<ILLMProvider, OllamaProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<OllamaProvider>>();
            var options = Microsoft.Extensions.Options.Options.Create(new OllamaOptions
            {
                BaseUrl = configuration["AI:Ollama:BaseUrl"] ?? "http://localhost:11434",
                DefaultModel = configuration["AI:Ollama:DefaultModel"] ?? "llama3.1"
            });
            return new OllamaProvider(factory, logger, options);
        });

        services.AddSingleton<ILLMProvider, CustomProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<CustomProvider>>();
            var options = Microsoft.Extensions.Options.Options.Create(new CustomProviderOptions
            {
                Name = configuration["AI:Custom:Name"] ?? "Custom",
                ApiKey = configuration["AI:Custom:ApiKey"] ?? "",
                BaseUrl = configuration["AI:Custom:BaseUrl"] ?? "",
                DefaultModel = configuration["AI:Custom:DefaultModel"] ?? "default",
                UseNoAuth = configuration.GetValue<bool>("AI:Custom:UseNoAuth")
            });
            return new CustomProvider(factory, logger, options);
        });

        // Register Router, AgentRunner, ToolRegistry
        services.AddSingleton<ProviderRouter>();
        services.AddScoped<IAgentRunner, AgentRunner>();
        services.AddSingleton<IToolExecutor, ToolRegistry>();

        return services;
    }
}
