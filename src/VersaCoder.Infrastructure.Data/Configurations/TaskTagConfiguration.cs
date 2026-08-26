using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Configurations;

/// <summary>
/// TaskTag EF Core yapılandırması — SQLite uyumlu.
/// </summary>
public class TaskTagConfiguration : IEntityTypeConfiguration<TaskTag>
{
    public void Configure(EntityTypeBuilder<TaskTag> builder)
    {
        builder.ToTable("TaskTags");

        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tt => tt.Color)
            .HasMaxLength(20);

        // Index
        builder.HasIndex(tt => tt.Name).IsUnique();
    }
}
