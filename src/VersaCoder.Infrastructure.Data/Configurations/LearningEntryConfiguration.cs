using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Configurations;

public class LearningEntryConfiguration : IEntityTypeConfiguration<LearningEntry>
{
    public void Configure(EntityTypeBuilder<LearningEntry> builder)
    {
        builder.ToTable("LearningEntries");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(l => l.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Value)
            .IsRequired();

        builder.Property(l => l.Source)
            .HasMaxLength(500);
    }
}
