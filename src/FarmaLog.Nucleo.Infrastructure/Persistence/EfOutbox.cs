using FarmaLog.Nucleo.Application.Ports;
using System.Text.Json;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class EfOutbox(NucleoDbContext context, TimeProvider timeProvider) : IOutbox
    {
        public void Add(OutboxMessage message)
        {
            OutboxEntry entry = new(
                id: Guid.CreateVersion7(),
                destination: message.Destination,
                messageId: message.MessageId,
                payload: JsonSerializer.Serialize(message.Payload, message.Payload.GetType()),
                createdAt: timeProvider.GetUtcNow());

            context.Set<OutboxEntry>().Add(entry);
        }
    }
}
