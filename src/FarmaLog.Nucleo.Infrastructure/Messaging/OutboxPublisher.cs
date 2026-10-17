using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed class OutboxPublisher(
        NucleoDbContext context, IOutboxMessageSender messageSender, TimeProvider timeProvider)
    {
        public async Task PublishPendingAsync(CancellationToken cancellationToken)
        {
            List<OutboxEntry> pendingEntries = await context
                .Set<OutboxEntry>()
                .Where(e => e.ProcessedAt == null)
                .ToListAsync(cancellationToken);

            foreach (OutboxEntry entry in pendingEntries)
            {
                await messageSender.SendAsync(
                    entry.Destination, entry.MessageId, entry.Payload, cancellationToken);

                entry.MarkAsProcessed(timeProvider.GetUtcNow());
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
