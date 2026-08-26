using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Configurations;

/// <summary>
/// TaskReminder EF Core yapılandırması — SQLite uyumlu.
/// </summary>
public class TaskReminderConfiguration : IEntityTypeConfiguration<TaskReminder>
{
    public void Configure(EntityTypeBuilder<TaskReminder> builder)
    {
        builder.ToTable("TaskReminders");

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Message)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(tr => tr.Task)
            .WithMany(t => t.Reminders)
            .HasForeignKey(tr => tr.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(tr => tr.TaskId);
        builder.HasIndex(tr => tr.ReminderDate);
        builder.HasIndex(tr => tr.IsSent);
    }
}
