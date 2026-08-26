using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.AgentName)
            .HasMaxLength(100);

        builder.Ignore(m => m.Metadata);
    }
}
