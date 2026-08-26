using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VersaCoder.Infrastructure.Config.Settings;

namespace VersaCoder.Infrastructure.Config;

/// <summary>
/// Infrastructure.Config DI genişletme metotları.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configuration servislerini kaydeder.
    /// </summary>
    public static IServiceCollection AddInfrastructureConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Options pattern ile ayarları bağla
        services.Configure<AppSettings>(configuration.GetSection("App"));
        services.Configure<AiSettings>(configuration.GetSection("AI"));
        services.Configure<OpenAiSettings>(configuration.GetSection("AI:OpenAI"));
        services.Configure<AnthropicSettings>(configuration.GetSection("AI:Anthropic"));
        services.Configure<OllamaSettings>(configuration.GetSection("AI:Ollama"));
        services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
        services.Configure<UiSettings>(configuration.GetSection("UI"));
        services.Configure<SecuritySettings>(configuration.GetSection("Security"));

        // ConfigurationManager'ı kaydet
        services.AddSingleton<IConfigurationManager, ConfigurationManager>();

        return services;
    }
}
