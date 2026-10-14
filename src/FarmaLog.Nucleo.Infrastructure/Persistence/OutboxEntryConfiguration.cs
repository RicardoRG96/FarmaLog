using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class OutboxEntryConfiguration : IEntityTypeConfiguration<OutboxEntry>
    {
        public void Configure(EntityTypeBuilder<OutboxEntry> builder)
        {
            builder.ToTable("Outbox");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Destination).HasMaxLength(260).IsRequired();
            builder.Property(e => e.MessageId).HasMaxLength(128).IsRequired();
            builder.Property(e => e.Payload).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();
        }
    }
}
