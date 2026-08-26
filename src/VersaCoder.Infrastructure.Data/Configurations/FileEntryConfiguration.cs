using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Configurations;

public class FileEntryConfiguration : IEntityTypeConfiguration<FileEntry>
{
    public void Configure(EntityTypeBuilder<FileEntry> builder)
    {
        builder.ToTable("Files");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.RelativePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.Extension)
            .HasMaxLength(50);

        builder.HasOne(f => f.Project)
            .WithMany(p => p.Files)
            .HasForeignKey(f => f.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(f => f.Metadata);
    }
}
