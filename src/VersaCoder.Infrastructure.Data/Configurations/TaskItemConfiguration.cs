using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Configurations;

/// <summary>
/// TaskItem EF Core yapılandırması — SQLite uyumlu,全面索izaleme desteği.
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .HasMaxLength(5000);

        builder.Property(t => t.Notes)
            .HasMaxLength(10000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.DurationType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.AssignedTo)
            .HasMaxLength(200);

        builder.Property(t => t.ReminderMessage)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TaskList)
            .WithMany(tl => tl.Tasks)
            .HasForeignKey(t => t.TaskListId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Tags)
            .WithMany(t => t.Tasks);

        builder.HasMany(t => t.Dependencies)
            .WithOne(d => d.Task)
            .HasForeignKey(d => d.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Dependents)
            .WithOne(d => d.DependsOnTask)
            .HasForeignKey(d => d.DependsOnTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Reminders)
            .WithOne(r => r.Task)
            .HasForeignKey(r => r.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.TaskListId);
        builder.HasIndex(t => t.SessionId);
        builder.HasIndex(t => t.ParentTaskId);
        builder.HasIndex(t => t.DueDate);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.IsMilestone);

        // Ignore computed properties
        builder.Ignore(t => t.Metadata);
    }
}
