using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Interfaces;
using VersaCoder.Infrastructure.Data.Repositories;

namespace VersaCoder.Infrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=versacoder.db";

        services.AddDbContext<VersaCoderDbContext>(options =>
            options.UseSqlite(connectionString, b =>
            {
                b.MigrationsAssembly(typeof(VersaCoderDbContext).Assembly.FullName);
                b.CommandTimeout(30);
            }));

        // WAL mode
        AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Sqlite.UseEncryption", false);

        // Existing repositories
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<ILearningRepository, LearningRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();

        // Task Management repositories
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskListRepository, TaskListRepository>();

        // Audit Logging repository
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }
}
