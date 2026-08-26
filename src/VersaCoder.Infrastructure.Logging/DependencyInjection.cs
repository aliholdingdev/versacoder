using Microsoft.Extensions.DependencyInjection;

namespace VersaCoder.Infrastructure.Logging;

/// <summary>
/// Infrastructure.Logging DI kaydı — JsonFileLogger servisi.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLogging(this IServiceCollection services,
        string? logDirectory = null)
    {
        services.AddSingleton(sp => new JsonFileLogger(logDirectory));

        return services;
    }
}
