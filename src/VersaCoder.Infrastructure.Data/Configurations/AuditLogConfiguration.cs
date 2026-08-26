using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Configurations;

/// <summary>
/// AuditLog EF Core yapılandırması — Append-only log yapısı için.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.Level)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(al => al.Agent)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(al => al.Message)
            .HasMaxLength(5000);

        builder.Property(al => al.MetadataJson)
            .HasMaxLength(10000);

        builder.Property(al => al.StackTrace)
            .HasMaxLength(10000);

        builder.Property(al => al.CurrentFile)
            .HasMaxLength(500);

        builder.Property(al => al.MethodName)
            .HasMaxLength(200);

        builder.Property(al => al.ClassName)
            .HasMaxLength(200);

        builder.Property(al => al.ErrorCode)
            .HasMaxLength(100);

        builder.Property(al => al.InnerExceptionMessage)
            .HasMaxLength(5000);

        // Indexes
        builder.HasIndex(al => al.Timestamp);
        builder.HasIndex(al => al.Level);
        builder.HasIndex(al => al.Agent);
        builder.HasIndex(al => al.Action);
        builder.HasIndex(al => al.SessionId);
        builder.HasIndex(al => al.TaskId);
    }
}
