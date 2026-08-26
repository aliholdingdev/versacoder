using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Application.Handlers;
using VersaCoder.Application.Services;
using VersaCoder.CrossCutting.Behaviors;
using VersaCoder.CrossCutting.Validation;
using VersaCoder.Infrastructure.AI;
using VersaCoder.Infrastructure.Data;
using MediatR;

namespace VersaCoder.Host;

public static class Startup
{
    public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // MediatR + Behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateSessionHandler).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Infrastructure.Data
        services.AddInfrastructureData(configuration);

        // Infrastructure.AI
        services.AddInfrastructureAI(configuration);

        // Application Services
        services.AddScoped<ISessionManager, SessionManagerService>();
        services.AddScoped<IContextManager, ContextManagerService>();
        services.AddScoped<IProjectAnalyzer, ProjectAnalyzerService>();
        services.AddScoped<ILearningService, LearningService>();
        services.AddScoped<IDiagramTeacher, DiagramTeacherService>();
        services.AddScoped<ITemplateService>(sp =>
            new TemplateService(Path.Combine(AppContext.BaseDirectory, "Templates")));
        services.AddScoped<IGitService, GitService>();

        return services;
    }

    public static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
