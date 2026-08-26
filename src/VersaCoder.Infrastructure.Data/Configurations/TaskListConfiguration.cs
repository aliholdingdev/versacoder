using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Configurations;

/// <summary>
/// TaskList EF Core yapılandırması — SQLite uyumlu.
/// </summary>
public class TaskListConfiguration : IEntityTypeConfiguration<TaskList>
{
    public void Configure(EntityTypeBuilder<TaskList> builder)
    {
        builder.ToTable("TaskLists");

        builder.HasKey(tl => tl.Id);

        builder.Property(tl => tl.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tl => tl.Description)
            .HasMaxLength(1000);

        builder.Property(tl => tl.Color)
            .HasMaxLength(20);

        builder.Property(tl => tl.Icon)
            .HasMaxLength(50);

        builder.Property(tl => tl.DefaultPriority)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(tl => tl.Name);
        builder.HasIndex(tl => tl.IsArchived);
        builder.HasIndex(tl => tl.SortOrder);
    }
}
