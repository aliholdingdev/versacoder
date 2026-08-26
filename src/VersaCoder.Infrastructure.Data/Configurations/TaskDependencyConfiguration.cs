using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Configurations;

/// <summary>
/// TaskDependency EF Core yapılandırması — Gantt chart desteği için.
/// </summary>
public class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("TaskDependencies");

        builder.HasKey(td => td.Id);

        builder.Property(td => td.DependencyType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(td => td.Task)
            .WithMany(t => t.Dependencies)
            .HasForeignKey(td => td.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(td => td.DependsOnTask)
            .WithMany(t => t.Dependents)
            .HasForeignKey(td => td.DependsOnTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(td => td.TaskId);
        builder.HasIndex(td => td.DependsOnTaskId);
        builder.HasIndex(td => new { td.TaskId, td.DependsOnTaskId }).IsUnique();
    }
}
