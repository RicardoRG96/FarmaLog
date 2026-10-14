namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class OutboxEntry
    {
        public Guid Id { get; private set; }
        public string Destination { get; private set; }
        public string MessageId { get; private set; }
        public string Payload { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ProcessedAt { get; private set; }

        public OutboxEntry(
            Guid id, string destination, string messageId, string payload, DateTimeOffset createdAt)
        {
            Id = id;
            Destination = destination;
            MessageId = messageId;
            Payload = payload;
            CreatedAt = createdAt;
        }
    }
}
