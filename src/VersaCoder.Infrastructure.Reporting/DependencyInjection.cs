using Microsoft.Extensions.DependencyInjection;

namespace VersaCoder.Infrastructure.Reporting;

/// <summary>
/// Infrastructure.Reporting DI kaydı — ExcelExporter ve PdfExporter servisleri.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureReporting(this IServiceCollection services)
    {
        services.AddSingleton<ExcelExporter>();
        services.AddSingleton<PdfExporter>();

        return services;
    }
}
