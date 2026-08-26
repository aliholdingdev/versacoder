using Microsoft.EntityFrameworkCore;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data;

public class VersaCoderDbContext : DbContext
{
    public VersaCoderDbContext(DbContextOptions<VersaCoderDbContext> options) : base(options)
    {
    }

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<FileEntry> Files => Set<FileEntry>();
    public DbSet<LearningEntry> LearningEntries => Set<LearningEntry>();
    public DbSet<Setting> Settings => Set<Setting>();

    // Task Management
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
    public DbSet<TaskReminder> TaskReminders => Set<TaskReminder>();

    // Audit Logging
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VersaCoderDbContext).Assembly);
    }
}
