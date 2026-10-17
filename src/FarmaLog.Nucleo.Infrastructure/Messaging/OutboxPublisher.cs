using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed class OutboxPublisher(
        NucleoDbContext context, 
        IOutboxMessageSender messageSender, 
        TimeProvider timeProvider, 
        ILogger<OutboxPublisher> logger)
    {
        public async Task PublishPendingAsync(CancellationToken cancellationToken)
        {
            List<OutboxEntry> pendingEntries = await context
                .Set<OutboxEntry>()
                .Where(e => e.ProcessedAt == null)
                .ToListAsync(cancellationToken);

            foreach (OutboxEntry entry in pendingEntries)
            {
                try
                {
                    await messageSender.SendAsync(
                        entry.Destination, entry.MessageId, entry.Payload, cancellationToken);

                    entry.MarkAsProcessed(timeProvider.GetUtcNow());
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }

            }
        }
    }
}
